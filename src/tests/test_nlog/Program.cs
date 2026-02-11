using Microsoft.Data.Sqlite;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace test_nlog
{
    class Program
    {
        private static void ConfigureLogging(Target target)
        {
            LogManager.Setup().LoadConfiguration(cfg =>
            {
                cfg.ForLogger(LogLevel.Debug).WriteTo(target);
            });
        }

        static void nlog_to_file_and_archive() {
            FileTarget target = new FileTarget();
            target.Layout = "${longdate} ${logger} ${message}";
            target.FileName = "${basedir}/logs/fileandarchive.txt";
            target.ArchiveFileName = "${basedir}/archives/log.{#####}.txt";
            target.ArchiveAboveSize = 1000 * 1024;
            //target.ArchiveNumbering = FileTarget.ArchiveNumberingMode.Sequence;

            ConfigureLogging(target);
        }

        static void nlog_to_simple_file() {
            FileTarget target = new FileTarget();
            target.Layout = "${longdate} ${logger} ${message}";
            target.FileName = "${basedir}/logs/simple.txt";
            target.KeepFileOpen = false;
            target.Encoding = Encoding.UTF8;

            ConfigureLogging(target);
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

            ConfigureLogging(target);
        }

        static void nlog_to_console() {
            ConsoleTarget target = new ConsoleTarget();
            target.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";

            ConfigureLogging(target);
        }

        private static void ensure_db_exists()
        {
            if (File.Exists("Log.db3"))
                return;

            using (var connection = new SqliteConnection("Data Source=Log.db3"))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE Log (time_stamp TEXT, level TEXT, logger TEXT, message TEXT)";
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<string> sqlite_db_tables(string db_name)
        {
            Debug.Assert(File.Exists(db_name));

            List<string> tables = new List<string>();
            try
            {
                using (var conn = new SqliteConnection($"Data Source=\"{db_name}\""))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read())
                                tables.Add(reader.GetString(0));
                    }
                }
            }
            catch
            {
                tables.Clear();
            }
            return tables;
        }

        public static List<string> sqlite_db_table_fields(string db_name, string table_name)
        {
            Debug.Assert(File.Exists(db_name));

            List<string> fields = new List<string>();
            try
            {
                using (var conn = new SqliteConnection($"Data Source=\"{db_name}\""))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"pragma table_info({table_name});";
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read())
                                fields.Add(reader.GetString(1));
                    }
                }
            }
            catch
            {
                fields.Clear();
            }
            return fields;
        }


        static void nlog_to_db()
        {
            ensure_db_exists();

            var target = new DatabaseTarget("sqlite")
            {
                DBProvider = "Microsoft.Data.Sqlite.SqliteConnection, Microsoft.Data.Sqlite",
                ConnectionString = "Data Source=Log.db3",
                CommandType = CommandType.Text,
                CommandText = "insert into Log(time_stamp,level,logger,message) values(@time_stamp, @level, @logger, @message);"
            };

            target.Parameters.Add(new DatabaseParameterInfo("@time_stamp", "${date}"));
            target.Parameters.Add(new DatabaseParameterInfo("@level", "${level}"));
            target.Parameters.Add(new DatabaseParameterInfo("@logger", "${logger}"));
            target.Parameters.Add(new DatabaseParameterInfo("@message", "${message}"));

            // just in case issues with db.
            LogManager.ThrowExceptions = true;

            ConfigureLogging(target);
        }

        private static void log_fast() {
            Logger logger = LogManager.GetLogger("Example");
            for (int i = 0; i < 1000; ++i)
                logger.Debug("log message {0}", i);            
        }
        private static void log_slow() {
            Logger logger = LogManager.GetLogger("Example");
            for (int i = 1000; i < 2000; ++i) {
                logger.Debug("log message {0}", i);
                Thread.Sleep(150);
            }
        }

        static void Main(string[] args) {
            //nlog_to_file_and_archive();
            //nlog_to_simple_file();
            //nlog_to_csv_file();
            //nlog_to_console();
            nlog_to_db();

            log_slow();
        }
    }
}
