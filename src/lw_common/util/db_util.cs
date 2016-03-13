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
using Oracle.ManagedDataAccess.Client;

namespace lw_common
{
    class db_util
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
    }
}
