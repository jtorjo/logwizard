using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lw_common.parse;

namespace lw_common {
    public class event_log_reader : entry_text_reader_base {

        private bool fully_read_once_ = false;
        private bool up_to_date_ = false;

        public string[] log_types {
            get {
                return settings.get("log_type", "Application,System").Split(',');                 
            }
        }

        public string machine_name {
            get {
                return settings.get("machine_name", ".");                 
            }
        }

        public override bool fully_read_once {
            get { return fully_read_once_; }
        }

        public override string name {
            get { return "_event_log_"; }
        }

        public override void force_reload() {
            // FIXME
        }

        internal override List<log_entry_line> read_next_lines() {
            // create event log if not yet
            // if too many errors, don't do anything

            // update fully read once + up to date
            // catch exceptions as well
            // FIXME in case of errors, add them somewhere! - like - in text_reader - have an array or something - make it simple and have a max limit of errors
            return null;
        }

        public override bool has_it_been_rewritten {
            get { return false; }
        }

        public override bool is_up_to_date() {
            return fully_read_once_ && up_to_date_;
        }
    }
}
