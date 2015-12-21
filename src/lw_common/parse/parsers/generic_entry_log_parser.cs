using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common.parse.parsers {
    class generic_entry_log_parser : log_parser_base {
        private entry_text_reader_base reader_;

        // this contains the full string
        protected large_string string_ = new large_string();

        protected memory_optimized_list<log_entry_line> entries_ = new memory_optimized_list<log_entry_line>() { name = "parser-entries-gp"};

        public generic_entry_log_parser(entry_text_reader_base reader) : base(reader.settings) {
            reader_ = reader;
        }

        public override void read_to_end() {
            var entries_now = reader_.read_available_lines();
            if (entries_now == null)
                return;
            lock (this) {
                foreach ( var entry in entries_now)
                    string_.add_preparsed_line(entry.ToString());
                entries_.AddRange(entries_now);

                if (column_names.Count < 1 && entries_now.Count > 0)
                    column_names = entries_now[0].names;
            }
        }

        public override int line_count {
            get { lock(this)  return entries_.Count; }
        }

        public override line line_at(int idx) {
            lock (this) {
                if (idx < entries_.Count) {
                    var entry = entries_[idx];
                    var l = new line( new sub_string(string_, idx), entry.idx_in_line(aliases)  );
                    return l;
                } else {
                    // this can happen, when the log has been re-written, and everything is being refreshed
                    throw new line.exception("invalid line request " + idx + " / " + entries_.Count);
                }
            }
        }

        public override void force_reload() {
            lock (this) {
                entries_.Clear();
                string_.clear();
            }
        }

        public override bool up_to_date {
            get { return reader_.is_up_to_date(); }
        }
    }
}
