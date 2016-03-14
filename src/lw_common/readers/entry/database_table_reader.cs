using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using lw_common.parse;

namespace lw_common.readers.entry
{
    class database_table_reader : entry_text_reader_base {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DbConnection conn_ = null;
        private DbCommand cmd_ = null;
        private DbDataReader reader_ = null;

        // how many rows to retrieve at once?
        private int block_size_ = 2000;

        private const int MAX_FAILED_CONNECTIONS = 20;
        private int failed_conn_idx_ = 0;

        // ... in case we have an ID column
        private long last_id_ = 0;
        // ... in case we don't have the ID column, but we have a timestamp column, and we sort by it
        private string last_time_str_ = "";

        private bool fully_read_once_ = false;

        private List<Tuple<string, info_type>> mappings_ = null;

        // if true, we're reading the time column as string - on sqlite, sometimes we can't read the time column as DateTime - however, it should work correctly now
        private bool read_time_as_string_ = false;

        // if this gets set to false, we don't read anything anymore - either we met a fatal error, or we fully read everything, and there's no way to 
        // continue reading, because we don't know how to sort the rows
        private bool continue_reading_ = true;

        public database_table_reader(log_settings_string sett) : base(sett) {
            settings.on_changed += on_settings_changed;
            mappings_ = get_db_mappings();
        }

        private void on_settings_changed(string name) {
            if (name.StartsWith("db_")) {
                dispose_db();
                mappings_ = get_db_mappings();
            }
        }

        public List<Tuple<string, info_type>> column_db_mappings {
            get { return get_db_mappings(); }
        }

        private List<Tuple<string, info_type>> get_db_mappings() {
            List<Tuple<string,info_type>> user_typed_mappings = new List<Tuple<string, info_type>>();
            foreach (var line in settings.db_fields.get() .Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries)) {
                int delim = line.IndexOf('=');
                if (delim >= 0) {
                    string name = line.Substring(0, delim).Trim(), value = line.Substring(delim + 1).Trim();
                    var info_value = info_type_io.from_str(value);
                    user_typed_mappings.Add( new Tuple<string, info_type>(name, info_value));
                }
                else 
                    user_typed_mappings.Add(new Tuple<string, info_type>(line.Trim(), info_type.max));
            }
            var lw_mappings = info_type_io.match_db_column_to_lw_column(user_typed_mappings);
            return lw_mappings;
        }

        public override bool fully_read_once {
            get { return fully_read_once_; }
        }


        private string order_by_column() {
            var order_by_column = settings.db_id_field.get();
            if (order_by_column == "") {
                var time_column = mappings_.FirstOrDefault(x => x.Item2 == info_type.time);
                if (time_column != null)
                    order_by_column = time_column.Item1;
            }
            return order_by_column;
        }

        private string get_sql_command() {
            string sql = "select " + util.concatenate(mappings_.Select(x => x.Item1), ",");
            if (settings.db_id_field != "")
                // the ID column - is hte last
                sql += "," + settings.db_id_field;

            sql += " from " + settings.db_table_name;
            var order_by_column = this.order_by_column();
            var id_field = settings.db_id_field.get();
            if (fully_read_once_)
                sql += " where " + (id_field != "" ? id_field + " > " + last_id_ : order_by_column + " > '" + last_time_str_ + "'" );
            if ( order_by_column != "")
                sql += " order by " + order_by_column;
            return sql;
        }

        public override void on_dispose() {
            base.on_dispose();
            dispose_db();
        }

        public override void force_reload(string reason) {
            base.force_reload(reason);
            
            dispose_db();

            last_id_ = 0;
            last_time_str_ = "";
            fully_read_once_ = false;
        }

        private void dispose_db() {
            dispose_reader();
            try {
                if ( conn_ != null)
                    conn_.Dispose();
            } catch (Exception e) {
                logger.Error("disposing database connection: " + e.Message);
            }

            conn_ = null;
        }
        private void dispose_reader() {
            try {
                if ( reader_ != null)
                    reader_.Dispose();
                if ( cmd_ != null)
                    cmd_.Dispose();
            } catch (Exception e) {
                logger.Error("disposing database connection: " + e.Message);
            }

            reader_ = null;
            cmd_ = null;
        }


        private void create_connection() {
            if (conn_ == null && failed_conn_idx_ < MAX_FAILED_CONNECTIONS)
                try {
                    dispose_db();
                    conn_ = db_util.create_db_connection(settings.db_provider, settings.db_connection_string);
                    conn_.Open();
                } catch (Exception e) {
                    logger.Error("can't connect to db " + settings.db_provider + " / " + settings.db_connection_string);
                    if ( failed_conn_idx_ % 10 == 0)
                        errors_.add("Cannot connect to database " + e.Message);
                    ++failed_conn_idx_;
                    dispose_db();
                }

            if (conn_ != null) {
                if ( reader_ == null)
                    try {
                        if ( cmd_ == null)
                            cmd_ = conn_.CreateCommand();
                        cmd_.CommandText = get_sql_command();
                        reader_ = cmd_.ExecuteReader();
                    } catch (Exception e) {
                        logger.Error("can't create db reader " + e.Message);
                        cmd_ = null;
                        reader_ = null;
                        ++failed_conn_idx_;
                    }

                if ( reader_ != null)
                    failed_conn_idx_ = 0;
            }
        }

        protected override List<log_entry_line> read_next_lines() {
            List<log_entry_line> lines = new List<log_entry_line>();
            create_connection();
            if (reader_ != null && continue_reading_) {
                try {
                    bool last_row_read = true;
                    for (int read_idx = 0; read_idx < block_size_ && last_row_read && continue_reading_; ++read_idx) {
                        last_row_read = reader_.Read();
                        if (last_row_read) 
                            lines.Add(line_from_reader());                         
                    }
                    if (!last_row_read && continue_reading_) {
                        if (!fully_read_once_) {
                            fully_read_once_ = true;
                            // we'll reconnect, and start tailing
                            dispose_db();
                        } else
                            dispose_reader();
                        if (order_by_column() == "")
                            // in this case, we've read the full db table - but for tailing, we just don't know what to do,
                            // because we don't have a way to enumerate only the last records
                            continue_reading_ = false;
                    }
                } catch (Exception e) {
                    logger.Error("error reading db " + e.Message);
                }
            }
            return lines;
        }

        private log_entry_line line_from_reader() {
            log_entry_line row = new log_entry_line();

            int i = 0;
            try {
                for (; i < mappings_.Count; ++i) {
                    bool is_time_ = mappings_[i].Item2 == info_type.time;
                    if (is_time_) {
                        if (read_time_as_string_)
                            row.add_time(reader_.GetString(i));
                        else {
                            // read time column as datetime
                            try {
                                // if this throws, read it as string
                                row.add_time(reader_.GetDateTime(i));
                            } catch (Exception e) {
                                logger.Error("can't read time column as datetime (will try reading as string) :" + e.Message);
                                read_time_as_string_ = true;
                                --i;
                            }
                        }

                        if (settings.db_id_field == "")
                            // sorting, for when we'll need to do tailing
                            last_time_str_ = reader_.GetString(i);
                    } else
                        // non-time column
                        // FIXME perhaps I could speed this up - perhaps I might even be able to create a 'line' object directly. But for now, lets leave it like this
                        row.add(mappings_[i].Item1, reader_.GetString(i));
                }

                if (settings.db_id_field != "")
                    last_id_ = reader_.GetInt64(mappings_.Count);
            } catch (Exception ee) {
                logger.Error("can't read db row " + ee.Message);
                errors_.add("Cannot read db field " + mappings_[i].Item1 + " : " + ee.Message);
                continue_reading_ = false;
            }

            // update last_id and time
            return row;
        }
    }
}
