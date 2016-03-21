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

        // example:
        //  <connectionType value="System.Data.SqlClient.SqlConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
        public static string db_connection_type_to_db_provider(string connection_type) {
            connection_type = connection_type.ToLower();
            if (connection_type.Contains(".sqlclient"))
                return "System.Data.SqlClient";
            if (connection_type.Contains(".oledb"))
                return "System.Data.OleDb";
            if (connection_type.Contains(".odbc"))
                return "System.Data.Odbc";
            if (connection_type.Contains(".oracleclient"))
                return "System.Data.OracleClient";
            if (connection_type.Contains(".sqlite"))
                return "System.Data.SQLite";

            return "";
        }

        private static string to_db_field_name(string name) {
            name = name.Trim().ToLower();
            if (name.StartsWith("[") && name.EndsWith("]"))
                name = name.Substring(1, name.Length - 2);
            return name;
        }
        public static List<string> insert_into_to_db_fields(string sql) {
            List<string> fields = new List<string>();
            if (sql.StartsWith("insert into")) {
                sql = sql.Substring(11).Trim();
                int separator = sql.IndexOf("(");
                int end_separator = sql.IndexOf(")");
                if ( separator > 0 && end_separator > 0) {
                    ++separator;
                    string fields_str = sql.Substring(separator, end_separator - separator);
                    fields = fields_str.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(to_db_field_name).ToList();
                }
            }
            return fields;
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
