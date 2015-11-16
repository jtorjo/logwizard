using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common.parse.parsers {
    class generic_entry_log_parser : log_parser_base {
        private entry_text_reader_base reader_;
        public generic_entry_log_parser(entry_text_reader_base reader, settings_as_string sett) : base(sett) {
            reader_ = reader;
        }

        public override void read_to_end() {
        }

        public override int line_count {
            get { return 0; }
        }

        public override line line_at(int idx) {
            return null;
        }

        public override void force_reload() {
            lock (this) {
                // clear all entries
            }
        }

        public override bool up_to_date {
            get { return false; }
        }
    }
}
