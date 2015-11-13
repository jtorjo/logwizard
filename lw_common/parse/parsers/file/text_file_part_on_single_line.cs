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
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using lw_common.parse.parsers.file;
using LogWizard;

namespace lw_common.parse.parsers {


    class text_file_part_on_single_line  : file_parser_base {

        private log_entry_line last_entry_ = new log_entry_line();
        private int valid_line_count_ = 0;

        private char separator_char_ = ':';

        // this contains the last lines that were read - the reason i have this is for correct parsing of Enters, regardless of how they are
        // ('\r', '\r\n', \n\r', '\n')
        //
        // used ONLY in read_to_end
        private large_string last_lines_string_ = new large_string();

        public text_file_part_on_single_line(file_text_reader reader, settings_as_string sett) : base(reader,sett) {
        }

        protected override void on_updated_settings() {
            base.on_updated_settings();
            separator_char_ = sett_.get("part.separator", ":")[0];

            //if ( util.is_debug)
              //  aliases_ = new aliases("_0=file|@#@|first=ctx1{Firsty}");
        }

        public static bool is_single_line(string file, settings_as_string sett) {
            string[] lines = util.read_beginning_of_file(file, 16834).Split( '\n' );
            for (int index = 0; index < lines.Length; index++) 
                lines[index] = lines[index].Replace("\r", "");

            int empty_lines = 0;
            string separator = sett.get("part.separator");
            if (separator == "")
                separator = ":";
            int contains_separator = 0;
            foreach ( string line in lines)
                if (line == "")
                    ++empty_lines;
                else if (line.Contains(separator))
                    ++contains_separator;

            // at least 3 entries
            if ( empty_lines > 3)
                if (contains_separator + empty_lines == lines.Length)
                    return true;

            return false;
        }


        // forces the WHOLE FILE to be reloaded
        //
        // be VERY careful calling this - I should call this only when the syntax has changed
        public override void force_reload() {
            base.force_reload();
            lock (this) {
                last_entry_ = new log_entry_line();
                valid_line_count_ = 0;
            }
        }

        protected override void on_new_lines(string new_lines) {
            // assume text is written line by line (thus, we read full lines)
            
            int line_count = 0;
            last_lines_string_.set_lines(new_lines, ref line_count);
            List<log_entry_line> entries_now = new List<log_entry_line>(line_count);
            log_entry_line last_entry;
            lock (this)
                last_entry = last_entry_;
            for (int i = 0; i < line_count; ++i) {
                string cur = last_lines_string_.line_at(i);
                int separator = cur.IndexOf(separator_char_);
                if (separator >= 0) {
                    string name = cur.Substring(0, separator).Trim();
                    string value = cur.Substring(separator + 1).Trim();
                    last_entry.add(name, value);
                    ++valid_line_count_;
                }
                else if (cur.Trim() != "")
                    last_entry.append_to_last(cur);
                else {
                    // empty line signals end of entry
                    entries_now.Add(last_entry);
                    last_entry = new log_entry_line();
                }
            }
            int entry_count;
            lock (this) entry_count = entries_.Count + entries_now.Count;
            if (entries_now.Count > 0)                
                --entry_count; // ...ignore last entry from computing avg - it may not be full
            int avg_entry_count = valid_line_count_ / entry_count;

            if (last_entry.ToString() != "" && last_entry.entry_count >= avg_entry_count) {
                // in this case, we guess the last entry was full
                entries_now.Add(last_entry);
                last_entry = new log_entry_line();
            }

            lock (this) {
                if (entries_now.Count > 0 && column_names_.Count == 0)
                    column_names_ = entries_now[0].names;
                last_entry_ = last_entry;
                foreach ( var entry in entries_now)
                    string_.add_preparsed_line( entry.ToString());
                entries_.AddRange(entries_now);
            }
        }
    }
}
