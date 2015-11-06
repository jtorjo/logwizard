/* 
 * Copyright (C) 2014-2015 John Torjo
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
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogWizard;

namespace lw_common.parse.parsers {
    class csv_file : log_parser_base {

        private file_text_reader reader_;
        private bool up_to_date_ = false;

        // this contains the full string
        private large_string string_ = new large_string();

        // this contains the last lines that were read - the reason i have this is for correct parsing of Enters, regardless of how they are
        // ('\r', '\r\n', \n\r', '\n')
        //
        // used ONLY in read_to_end
        private large_string last_lines_string_ = new large_string();

        private settings_as_string sett_;
        private aliases aliases_;

        private memory_optimized_list<log_entry_line> entries_ = new memory_optimized_list<log_entry_line>() { name = "parser-entries-csv"};

        private List<string> column_names_ = new List<string>();

        // if true, the first line is the header (containing column names)
        private bool has_header_line_ = true;

        public csv_file(file_text_reader reader, settings_as_string sett) {
            reader_ = reader;
            sett_ = sett;
            read_settings();
        }

        public override void on_settings_changed(string settings) {
            sett_ = new settings_as_string(settings);
            read_settings();
            // FIXME
        }

        private void read_settings() {            
            aliases_ = new aliases(sett_.get("aliases"));
            has_header_line_ = sett_.get("has_header", "1") == "1";
        }

        public override List<string> column_names {
            get { lock(this) return column_names_.ToList(); }
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

            int line_count = 0;
            last_lines_string_.set_lines(reader_.read_next_text(), ref line_count);
            if (line_count < 1)
                return;

            int start_idx = 0;
            if (has_header_line_) 
                lock (this) 
                    // if at least one entry - can't read column names
                    if (column_names_.Count < 1 && entries_.Count == 0) {
                        column_names_ = split.to_list(last_lines_string_.line_at(0), ",");
                        start_idx = 1;
                    }

            List<log_entry_line> entries_now = new List<log_entry_line>();
            var column_names = this.column_names;
            for (int i = start_idx; i < line_count; ++i) {
                var list = split.to_list(last_lines_string_.line_at(i), ",");
                log_entry_line entry = new log_entry_line();
                for ( int j = 0; j < column_names.Count; ++j)
                    entry.add( column_names[j], list.Count > j ? list[j] : "");
                entries_now.Add(entry);
            }

            lock (this) {
                foreach ( var entry in entries_now)
                    string_.add_preparsed_line(entry.ToString());
                entries_.AddRange(entries_now);
            }
        }

    }
}
