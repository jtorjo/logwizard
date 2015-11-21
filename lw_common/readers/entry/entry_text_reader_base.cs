using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using lw_common.parse;

namespace lw_common {
    public abstract class entry_text_reader_base : text_reader {
        
        private List<log_entry_line> lines_now_ = new List<log_entry_line>(); 
        protected abstract List<log_entry_line> read_next_lines();

        private bool reloaded_ = false;
        private bool was_rewritten_ = false;

        protected entry_text_reader_base(settings_as_string sett) : base(sett) {
            new Thread(read_entries_thread) {IsBackground = true}.Start();
        }

        // if null, we don't have anything
        public List<log_entry_line> read_available_lines() {
            lock (this) 
                if (lines_now_.Count > 0) {
                    var now = lines_now_;
                    lines_now_ = new List<log_entry_line>();
                    return now;
                }
            
            return null;
        } 

        public override void force_reload() {
            lock (this) {
                reloaded_ = true;
                was_rewritten_ = true;
                lines_now_.Clear();
            }
        }

        public override bool has_it_been_rewritten {
            get {
                lock (this) {
                    bool has = was_rewritten_;
                    was_rewritten_ = false;
                    return has;
                } 
            }
        }

        public override bool is_up_to_date() {
            return true;
        }

        private void read_entries_thread() {
            while (!disposed) {
                Thread.Sleep( app.inst.check_new_lines_interval_ms);
                bool reloaded;
                lock (this) {
                    reloaded = reloaded_;
                    reloaded_ = false;
                }

                var lines = read_next_lines();
                if (lines.Count > 0 || reloaded) {
                    lock(this)
                        lines_now_.AddRange(lines);
                    parser.on_log_has_new_lines(reloaded);
                }
            }
        }

    }
}
