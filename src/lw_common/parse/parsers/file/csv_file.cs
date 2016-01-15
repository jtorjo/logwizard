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
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lw_common.parse.parsers.file;
using LogWizard;

namespace lw_common.parse.parsers {
    class csv_file : file_parser_base {

        // this contains the last lines that were read - the reason i have this is for correct parsing of Enters, regardless of how they are
        // ('\r', '\r\n', \n\r', '\n')
        //
        // used ONLY in read_to_end
        private large_string last_lines_string_ = new large_string();

        // if true, the first line is the header (containing column names)
        private bool has_header_line_ = true;
        private string separator_ = ",";

        public csv_file(file_text_reader reader) : base(reader) {
        }

        protected override void on_updated_settings() {
            base.on_updated_settings();
            has_header_line_ = sett_.cvs_has_header;
            separator_ = sett_.cvs_separator_char;
        }

        public override void force_reload() {
            lock (this) {
                last_lines_string_.clear();
            }
        }

        public override bool has_multi_line_columns {
            get { return false; }
        }

        protected override void on_new_lines(string new_lines) {
            int line_count = 0;
            last_lines_string_.set_lines(new_lines, ref line_count);
            if (line_count < 1)
                return;

            int start_idx = 0;
            if (has_header_line_) 
                lock (this) 
                    // if at least one entry - can't read column names
                    if (this.column_names.Count < 1 && entries_.Count == 0) {
                        this.column_names = split.to_list(last_lines_string_.line_at(0), separator_);
                        start_idx = 1;
                    }

            List<log_entry_line> entries_now = new List<log_entry_line>();
            var column_names = this.column_names;
            for (int i = start_idx; i < line_count; ++i) {
                var list = split.to_list(last_lines_string_.line_at(i), separator_);
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
