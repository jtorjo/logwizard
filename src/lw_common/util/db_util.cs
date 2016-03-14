using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using Oracle.ManagedDataAccess.Client;

namespace lw_common
{
    public class db_util
    {
        public static DbConnection create_db_connection(string db_provider, string db_connection_string) {
            switch (db_provider) {
                case "System.Data.SqlClient":
                    return new SqlConnection(db_connection_string);
                case "System.Data.OleDb":
                    return new OleDbConnection(db_connection_string);
                case "System.Data.Odbc":
                    return new OdbcConnection(db_connection_string);
                case "System.Data.OracleClient":
                    return new OracleConnection(db_connection_string);
                case "System.Data.SQLite":
                    return new SQLiteConnection(db_connection_string);
            }
            Debug.Assert(false);
            return null;
        }

        public static string non_null_str(DbDataReader rs, int idx) {
            return rs.IsDBNull(idx) ? "" : rs.GetString(idx);
        }
        public static int non_null_int(DbDataReader rs, int idx) {
            return rs.IsDBNull(idx) ? 0 : rs.GetInt32(idx);
        }

        public static DateTime non_null_dt(DbDataReader rs, int idx) {
            return rs.IsDBNull(idx) ? DateTime.MinValue : rs.GetDateTime(idx);
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
    }
}
