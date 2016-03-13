using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Internal;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace test_nlog
{
    class Program
    {
        static void nlog_to_file_and_archive() {
            FileTarget target = new FileTarget();
            target.Layout = "${longdate} ${logger} ${message}";
            target.FileName = "${basedir}/logs/fileandarchive.txt";
            target.ArchiveFileName = "${basedir}/archives/log.{#####}.txt";
            target.ArchiveAboveSize = 1000 * 1024; 
            //target.ArchiveNumbering = FileTarget.ArchiveNumberingMode.Sequence;

            // this speeds up things when no other processes are writing to the file
            target.ConcurrentWrites = true;

            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Debug);            
        }

        static void nlog_to_simple_file() {
            FileTarget target = new FileTarget();
            target.Layout = "${longdate} ${logger} ${message}";
            target.FileName = "${basedir}/logs/simple.txt";
            target.KeepFileOpen = false;
            target.Encoding = Encoding.UTF8;

            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Debug);            
        }

        static void nlog_to_csv_file() {
            FileTarget target = new FileTarget();
            target.FileName = "${basedir}/logs/csv_file.csv";

            CsvLayout layout = new CsvLayout();

            layout.Columns.Add(new CsvColumn("time", "${longdate}"));
            layout.Columns.Add(new CsvColumn("message", "${message}"));
            layout.Columns.Add(new CsvColumn("logger", "${logger}"));
            layout.Columns.Add(new CsvColumn("level", "${level}"));

            target.Layout = layout;

            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Debug);            
        }

        static void nlog_to_console() {
            ConsoleTarget target = new ConsoleTarget();
            target.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";

            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Debug);            
        }


        // http://stackoverflow.com/questions/26582818/how-to-use-sqlite-with-nlog-in-net
        private static void ensure_db_exists()
        {
            if (File.Exists("Log.db3"))
                return;

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=Log.db3;Version=3;"))
                using (SQLiteCommand command = new SQLiteCommand(
                    "CREATE TABLE Log (time_stamp TEXT, level TEXT, logger TEXT, message TEXT)",
                    connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
        }

        public static List<string> sqlite_db_tables(string db_name) {
            // has to be a file
            Debug.Assert(File.Exists(db_name));

            List<string> tables = new List<string>();
            try {
                using (var conn = new SQLiteConnection("Data Source=\"" + db_name + "\";Version=3;")) {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;", conn))
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            tables.Add(reader.GetString(0));
                }
            } catch  {
                tables.Clear();
            }
            return tables;
        }

        public static List<string> sqlite_db_table_fields(string db_name, string table_name) {
            // has to be a file
            Debug.Assert(File.Exists(db_name));

            List<string> fields = new List<string>();
            try {
                using (var conn = new SQLiteConnection("Data Source=\"" + db_name + "\";Version=3;")) {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("pragma table_info(" + table_name + ");", conn))
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read())
                                fields.Add(reader.GetString(1));
                }
            }
            catch {
                fields.Clear();
            }
            return fields;
        } 

        static void nlog_to_db() {
            ensure_db_exists();

            DatabaseTarget target = new DatabaseTarget();
            DatabaseParameterInfo param;

            // just in case issues with db.
            LogManager.ThrowExceptions = true;

//            target.DBProvider = "System.Data.SQLite";
            target.DBProvider = "System.Data.SQLite.SQLiteConnection, System.Data.SQLite";
            //target.ConnectionString = "Data Source=${basedir}\\Log.db3;Version=3;";
            target.ConnectionString = "Data Source=Log.db3;Version=3;";
            target.CommandType = CommandType.Text;
            target.CommandText = "insert into Log(time_stamp,level,logger,message) values(@time_stamp, @level, @logger, @message);";

            param = new DatabaseParameterInfo();
            param.Name = "@time_stamp";
            param.Layout = "${date}";
            target.Parameters.Add(param);

            param = new DatabaseParameterInfo();
            param.Name = "@level";
            param.Layout = "${level}";
            target.Parameters.Add(param);

            param = new DatabaseParameterInfo();
            param.Name = "@logger";
            param.Layout = "${logger}";
            target.Parameters.Add(param);

            param = new DatabaseParameterInfo();
            param.Name = "@message";
            param.Layout = "${message}";
            target.Parameters.Add(param);

            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Debug);
            
        }

        static void Main(string[] args) {
            //nlog_to_file_and_archive();
            //nlog_to_simple_file();
            //nlog_to_csv_file();
            //nlog_to_console();
            nlog_to_db();

            Logger logger = LogManager.GetLogger("Example");
            for (int i = 0; i < 1000; ++i)
                logger.Debug("log message {0}", i);

        }
    }
}
