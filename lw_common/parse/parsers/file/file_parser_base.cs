using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogWizard;

namespace lw_common.parse.parsers.file {
    abstract class file_parser_base : log_parser_base {
        private file_text_reader reader_;
        private bool up_to_date_ = false;
        

        // this contains the full string
        protected large_string string_ = new large_string();

        // 1.4.8 - the reason I keep this is that the user might decide to change the aliases - thus, we want to easily be able to recompute all lines
        //         probably in the future I may do some optimizations, but for now, leave it as is
        //
        //         eventually, after a few mins, if no modifications of aliases, erase it (and keep another list with the pre-parsed lines)
        protected memory_optimized_list<log_entry_line> entries_ = new memory_optimized_list<log_entry_line>() { name = "parser-entries-fpb"};

        protected  List<string> column_names_ = new List<string>(); 

        public file_parser_base(file_text_reader reader, settings_as_string sett) : base(sett) {
            reader_ = reader;
        }

        protected abstract void on_new_lines(string new_lines);

        public override void read_to_end() {
            ulong old_len = reader_.full_len;
            reader_.compute_full_length();
            ulong new_len = reader_.full_len;
            // when reader's position is zero -> it's either the first time, or file was re-rewritten
            if (old_len > new_len || reader_.pos == 0) 
                // file got re-written
                force_reload();

            bool fully_read = old_len == new_len && reader_.is_up_to_date();

            if ( !reader_.has_more_cached_text()) {
                lock (this) 
                    up_to_date_ = fully_read;
                return;
            }
            
            lock (this)
                up_to_date_ = false;

            on_new_lines( reader_.read_next_text());
        }


        public override int line_count {
            get { lock(this)  return entries_.Count; }
        }

        public override line line_at(int idx) {
            lock (this) {
                if (idx < entries_.Count) {
                    var entry = entries_[idx];
                    var l = new line( new sub_string(string_, idx), entry.idx_in_line(aliases_)  );
                    return l;
                } else {
                    // this can happen, when the log has been re-written, and everything is being refreshed
                    throw new line.exception("invalid line request " + idx + " / " + entries_.Count);
                }
            }
        }

        public override List<string> column_names {
            get { lock(this) return column_names_.ToList(); }
        }

        public override void force_reload() {
            lock (this) {
                entries_.Clear();
                string_.clear();
                column_names_.Clear();
            }
        }

        public override bool up_to_date {
            get { return up_to_date_; }
        }
    }
}
