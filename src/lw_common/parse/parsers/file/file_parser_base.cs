/* 
 * Copyright (C) 2014-2016 John Torjo
 *
 * This file is part of LogWizard
 *
 * LogWizard is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * LogWizard is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact john.code@torjo.com 
 *  * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
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

        public file_parser_base(file_text_reader reader) : base(reader.settings) {
            reader_ = reader;
        }

        protected file_text_reader reader {
            get { return reader_; }
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
                    var l = new line( new sub_string(string_, idx), entry.idx_in_line(aliases), entry.time );
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
            get { return up_to_date_; }
        }

        public override bool has_multi_line_columns {
            get { return false; }
        }
    }
}
