using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace lw_common
{
    public static class parse_config
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int MAX_CONFIG_FILE_SIZE = 100 * 1024;

        private static void add_config_files_from_dir(string dir, List<string> config_files) {
            // https://github.com/nlog/NLog/wiki/Configuration-file
            // https://logging.apache.org/log4net/release/manual/configuration.html

            // nlog preffered file
            if ( File.Exists(dir + "//nlog.config"))
                config_files.Add(dir + "//nlog.config");

            // log4net web preffered file
            if ( File.Exists(dir + "//log.config"))
                config_files.Add(dir + "//log.config");

            // nlog configuration files
            config_files.AddRange(Directory.EnumerateFiles(dir, "*.nlog", SearchOption.TopDirectoryOnly));

            // add all .config files - but the .exe.config will come first
            config_files.AddRange(Directory.EnumerateFiles(dir, "*.exe.config", SearchOption.TopDirectoryOnly));
            config_files.AddRange(Directory.EnumerateFiles(dir, "*.config", SearchOption.TopDirectoryOnly).Where(x => !x.ToLower().EndsWith(".exe.config"))); 
        }

        // given a file or directory, will try to find a .config file, to read log settings from
        // in other words, we try to find as many settings as possible related to this log as possible,
        // to pre-load them
        //
        // at this point, we're looking for nlog or log4net config files
        //
        // if nothing found, returns ""
        public static string find_config_file(string file_name_or_dir) {
            // when finding .config files, actually test if they are log4net or nlog and return the first one that is either nlog or log4net

            try {
                // ... if relative file, go to absolute
                if (!Path.IsPathRooted(file_name_or_dir))
                    file_name_or_dir = new FileInfo(file_name_or_dir).FullName;

                string dir = file_name_or_dir;
                if (File.Exists(file_name_or_dir))
                    dir = new FileInfo(file_name_or_dir).DirectoryName;

                // gather possible .config files - order counts!
                List<string> config_files = new List<string>();
                if (File.Exists(file_name_or_dir + ".config"))
                    config_files.Add(file_name_or_dir + ".config");

                add_config_files_from_dir(dir, config_files);
                if (config_files.Count < 1) {
                    // try the parent
                    var parent = new DirectoryInfo(dir).Parent;
                    if (parent != null) 
                        add_config_files_from_dir(parent.FullName, config_files);
                    // last resort - parent's parent
                    if (config_files.Count < 1 && parent != null && parent.Parent != null) 
                        add_config_files_from_dir(parent.Parent.FullName, config_files);
                }

                // I assume any .config file does not have more than 100K - because I will be reading them fully, 
                // and I don't want to end up reading 100 mega bytes by mistake
                config_files = config_files.Where(x => new FileInfo(x).Length <= MAX_CONFIG_FILE_SIZE).ToList();

                // read config files
                foreach (var config_file in config_files) {
                    if ( is_config_file(config_file))
                        return config_file;
                }
            } catch (Exception e) {
                logger.Error("can't find .config file " + file_name_or_dir + " : " + e.Message);
            }
            return "";
        }

        public static bool is_config_file(string config_file) {
            try {
                if (new FileInfo(config_file).Length < MAX_CONFIG_FILE_SIZE) {
                    var all = File.ReadAllText(config_file);
                    if (all.Contains("<log4net") || all.Contains("<nlog"))
                        return true;
                }
            } catch {
            }
            return false;
        }

        // reads the config file, and sets as many settings as possible
        //
        public static log_settings_string load_config_file(string config_file) {
            log_settings_string config = new log_settings_string("");
            try {
                var doc = new XmlDocument() ;
                using ( var sr = new StringReader( File.ReadAllText(config_file)))
                    using ( var xtr = new XmlTextReader(sr) { Namespaces = false })
                        doc.Load(xtr);

                var root = doc.DocumentElement;
                var log4net = root.SelectSingleNode("//log4net");
                var nlog = root.SelectSingleNode("//nlog");
                if (root.Name == "log4net")
                    log4net = root;
                else if (root.Name == "nlog")
                    nlog = root;

                if ( log4net != null)
                    parse_log4net_config_file(log4net, config_file, config);
                else if ( nlog != null)
                    parse_nlog_config_file(nlog, config_file, config);
            } catch (Exception e) {
                logger.Error("can't read config file " + config_file + " : " + e.Message);
            }
            return config;
        }

        private static XmlAttribute attribute_ending_with(XmlNode node, string name) {
            foreach ( XmlAttribute attr in node.Attributes)
                if (attr.Name.ToLower(). EndsWith(name.ToLower()))
                    return attr;
            return null;
        }

        private static void parse_nlog_config_file(XmlNode nlog_root, string config_file, log_settings_string config) {
            string dir = new FileInfo(config_file).DirectoryName;
            var appenders = nlog_root.SelectNodes(".//target");
            foreach (XmlNode appender in appenders) {
                var appender_type = attribute_ending_with(appender, "type") .Value.ToLower();
                if (appender_type.Contains("file")) {
                    var file_name = appender.Attributes["fileName"].Value;
                    if (file_name.StartsWith("${"))
                        file_name = file_name.Substring(file_name.IndexOf("}") + 1);
                    if (file_name.StartsWith("\\") || file_name.StartsWith("/"))
                        file_name = file_name.Substring(1);

                    if (!Path.IsPathRooted(file_name))
                        file_name = dir + "//" + file_name;

                    config.type.set( log_type.file);
                    config.name.set( file_name);

                    var layout = attribute_ending_with(appender, "layout");
                    if (layout != null) {
                        var layout_str = layout.Value;
                        config.syntax.set(layout_str);
                    }
                }
                else if (appender_type.Contains("database")) {
                    var db_type = appender.Attributes[ "dbProvider"].Value;
                    var connection_string = appender.Attributes[ "connectionString"].Value;
                    var cmd_text = appender.Attributes[ "commandText"];
                    // if we already have the configuration set, it means that we have a file as well - we prefer the file
                    if ( config.name == "")
                        config.type.set( log_type.db);
                    config.db_provider.set( db_util.db_connection_type_to_db_provider(db_type));
                    config.db_connection_string.set( connection_string);
                    if (cmd_text != null) {
                        string sql = cmd_text.Value.ToLower().Trim();
                        if (sql.StartsWith("insert into")) {
                            var fields = db_util.insert_into_to_db_fields(sql);
                            if (fields.Count > 0) 
                                config.db_fields.set( util.concatenate(fields, "\r\n") );
                            sql = sql.Substring(11).Trim();
                            int separator = sql.IndexOf("(");
                            if (separator > 0) {
                                string table_name = sql.Substring(0, separator).Trim();
                                config.db_table_name.set(table_name);
                            }
                        }
                    }                    
                }
            }
        }

        private static void parse_log4net_config_file(XmlNode log4net_root, string config_file, log_settings_string config) {
            string dir = new FileInfo(config_file).DirectoryName;
            var appenders = log4net_root.SelectNodes(".//appender");
            foreach (XmlNode appender in appenders) {
                var appender_type = appender.Attributes["type"].Value.ToLower();
                if (appender_type.Contains("file")) {
                    // it's a file
                    var file = appender.SelectSingleNode("./file");
                    if (file != null) {
                        string file_name = file.Attributes["value"].Value.Trim();
                        if (file_name.StartsWith("${"))
                            file_name = file_name.Substring(file_name.IndexOf("}") + 1);
                        if (file_name.StartsWith("\\"))
                            file_name = file_name.Substring(1);

                        if (!Path.IsPathRooted(file_name))
                            file_name = dir + "//" + file_name;

                        config.type.set( log_type.file);
                        config.name.set( file_name);
                        // force recomputing the syntax (in case it was already found)
                        config.syntax.set(file_text_reader.UNKNOWN_SYNTAX);
                    }

                }
                else if (appender_type.Contains("adonet")) {
                    // database
                    var type = appender.SelectSingleNode("./connectionType");
                    var connection_string = appender.SelectSingleNode("./connectionString");
                    var cmd_text = appender.SelectSingleNode("./commandText");
                    if (type != null) {
                        var db_type = type.Attributes["value"].Value;
                        // if we already have the configuration set, it means that we have a file as well - we prefer the file
                        if ( config.name == "")
                            config.type.set( log_type.db);
                        config.db_provider.set( db_util.db_connection_type_to_db_provider(db_type));
                        config.db_connection_string.set( connection_string.Attributes["value"].Value);

                        if (cmd_text != null) {
                            string sql = cmd_text.Attributes["value"].Value.ToLower().Trim();
                            if (sql.StartsWith("insert into")) {
                                var fields = db_util.insert_into_to_db_fields(sql);
                                if (fields.Count > 0) 
                                    config.db_fields.set( util.concatenate(fields, "\r\n") );
                                sql = sql.Substring(11).Trim();
                                int separator = sql.IndexOf("(");
                                if (separator > 0) {
                                    string table_name = sql.Substring(0, separator).Trim();
                                    config.db_table_name.set(table_name);
                                }
                            }
                        }
                    }
                }

                // find out pattern (syntax)
                bool syntax_found = config.syntax != file_text_reader.UNKNOWN_SYNTAX;
                if (!syntax_found) {
                    var pattern = appender.SelectSingleNode("./conversionpattern");
                    var layout = appender.SelectSingleNode("./layout");
                    if (pattern != null) {
                        var value = layout.Attributes["value"].Value;
                        if (value != null)
                            config.syntax.set(value);
                    } else if (layout != null) {
                        var value = layout.Attributes["value"].Value;
                        if (value != null) {
                            // here, the value might be for a non-standard layout - so, try to actually parse it
                            var parsed = parse_log4net_syntax.parse(value);
                            if (parsed != "")
                                config.syntax.set(value);
                        }
                    }
                }
            }
        }
    }
}
