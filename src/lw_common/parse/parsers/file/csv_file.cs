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
using System.Diagnostics;
using System.Linq;
using System.Text;
using lw_common.parse.parsers.file;
using LogWizard;

namespace lw_common.parse.parsers {
    class csv_file : file_parser_base {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // this contains the last lines that were read - the reason i have this is for correct parsing of Enters, regardless of how they are
        // ('\r', '\r\n', \n\r', '\n')
        //
        // used ONLY in read_to_end
        private large_string last_lines_string_ = new large_string();

        // if true, the first line is the header (containing column names)
        private bool has_header_line_ = true;

        // 1.6.28+ - for now, assume this, since I also assume when separator is found in a  cell, I use quotes
        private string separator_ = ",";

        public csv_file(file_text_reader reader) : base(reader) {
        }

        protected override void on_updated_settings() {
            base.on_updated_settings();
            has_header_line_ = sett_.cvs_has_header;
            separator_ = sett_.cvs_separator_char;

            // 1.6.28+ if I don't have a header, I can't compute the number of columns, thus I don't know when a line is finished
            // no biggie, I just need to actually implement that code
            Debug.Assert(has_header_line_);
        }

        public override void force_reload() {
            lock (this) {
                last_lines_string_.clear();
            }
        }

        public override bool has_multi_line_columns {
            get { return false; }
        }

        private static List<string> parse_csv(string line) {
            List<string> csv = new List<string>();
            csv.Add("");

            // 1.8.6+ - account for empty lines - which can precede a valid line
            line = line.Trim();

            StringBuilder last = new StringBuilder();
            bool inside_quote = false;
            char prev_ch = '\0';
            foreach (char ch in line) {
                if (ch == ',') {
                    if (inside_quote)
                        last.Append(ch);
                    else {
                        csv[csv.Count - 1] = last.ToString();
                        last.Clear();
                        csv.Add("");
                    }
                } else if (ch == '"') {
                    if (!inside_quote && prev_ch == '"')
                        // double quote - append just one (the last one)
                        last.Append(ch);
                    inside_quote = !inside_quote;
                } else
                    last.Append( ch);
                prev_ch = ch;
            }
            csv[csv.Count - 1] = last.ToString();
            if ( inside_quote)
                // in this case, the last cell is not finished
                csv.RemoveAt(csv.Count - 1);
            return csv;
        }


        private bool is_header_present(List<string> names ) {
            var unique = names;
            bool present = true;
            if (unique.Any(x => x == "")) 
                present = false;
            else if (names.Distinct().Count() != names.Count) 
                // in this case, we'd have two equal values - we can't have that in the header
                present = false;
            if (!present)
                unique = util.unique_names(new string[names.Count], "Unnamed");
            this.column_names = unique;
            return present;
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
                    if (this.column_names.Count < 1 && entries_.Count == 0) 
                        start_idx = is_header_present( parse_csv( last_lines_string_.line_at(0))) ? 1 : 0;

            List<log_entry_line> entries_now = new List<log_entry_line>();
            var column_names = this.column_names;
            string before = "";
            for (int i = start_idx; i < line_count; ++i) {
                var cur_line = last_lines_string_.line_at(i);
                var list = parse_csv(before + cur_line);
                if (list.Count < column_names.Count) {
                    before += cur_line + "\r\n";
                    continue;
                }
                before = "";
                if ( list.Count > column_names.Count)
                    logger.Warn("invalid csv line, too many cells: " + list.Count + " , instead of " + column_names.Count);
                log_entry_line entry = new log_entry_line();
                for ( int j = 0; j < column_names.Count; ++j)
                    entry.add( column_names[j], list[j]);
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
