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
using System.Net.Configuration;
using System.Text;
using log4net.Repository.Hierarchy;
using lw_common;

namespace lw_common
{
    // holds the lines in a huge string
    //
    // note: at this time, we assume the enter is formed of 2 chars - either \r\n or \n\r
    public class large_string
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const int COMPUTE_AVG_LINE_AFTER = 2000;

        private StringBuilder string_ = new StringBuilder();

        // index of each enter
        private memory_optimized_list<int> indexes_ = new memory_optimized_list<int>() { name = "large_string_indexes", min_capacity = app.inst.no_ui.min_lines_capacity };

        private bool test_we_computed_lines_correctly = false; //util.is_debug;

        private bool computed_avg_line_ = false;
        private bool expected_bytes_set_ = false;

        // tests to see we've computed the lines correctly
        private void test_compute_lines() {
            string[] lines = string_.ToString().Split(new string[] {"\r\n"}, StringSplitOptions.None);
            Debug.Assert(lines.Length == line_count);
            for (int i = 0; i < lines.Length; ++i) {
                string cur = line_at(i);
                Debug.Assert(lines[i] == cur);
            }
        }

        public void expect_bytes(ulong byte_count) {
            Debug.Assert( byte_count < int.MaxValue);
            string_.Capacity = (int)byte_count;
            expected_bytes_set_ = true;
        }

        public void add_lines(string lines, ref int added_line_count, ref bool was_last_line_incomplete, ref bool is_last_line_incomplete) {
            if (lines == "")
                return;

            // 1.3.31+ - the reason i log this - is that in case there's any error parsing the log, I should be able to reproduce,
            //           by fully rereading the original log + re-adding the exact sequences
            logger.Debug("[line] added lines: " + string_.Length + "," + lines.Length);

            was_last_line_incomplete = false;
            if (string_.Length > 0 && lines.Length > 0) {
                
                bool might_be_incomplete = line_count > indexes_.Count;
                const string LINE_SEP = "\r\n";
                bool starts_with_enter = LINE_SEP.Contains(lines[0]);
                was_last_line_incomplete = might_be_incomplete && !starts_with_enter;
            }

            int len = string_.Length;
            string_.Append(lines);
            int old_line_count = line_count;
            compute_indexes(len);
            added_line_count = line_count - old_line_count;
            // 1.3.31b+
            if (ends_in_enter())
                ++added_line_count;

            is_last_line_incomplete = line_count > indexes_.Count;
            logger.Debug("[line] we have read " + line_count + " lines");
            update_indexes_capacity();

            if (test_we_computed_lines_correctly)
                test_compute_lines();
        }

        public void set_lines(string lines, ref int line_count) {
            indexes_.Clear();
            string_.Clear();
            string_.Append(lines);
            compute_indexes(0);
            line_count = this.line_count;
            update_indexes_capacity();

            if (test_we_computed_lines_correctly)
                test_compute_lines();
        }

        // 1.4.8+
        public void add_preparsed_line(string line) {
            string_.Append(line);
            indexes_.Add( string_.Length);
            string_.Append("\r\n");
        }

        public void clear() {
            indexes_.Clear();
            string_.Clear();            
        }

        private void update_indexes_capacity() {
            if (line_count < COMPUTE_AVG_LINE_AFTER)
                return;
            if (computed_avg_line_)
                return;
            if (!expected_bytes_set_)
                return;

            computed_avg_line_ = true;
            int avg_line = char_count / line_count ;
            indexes_.min_capacity = (string_.Capacity / avg_line);
        }


        public int char_count {
            get { return string_.Length;  }
        }

        public int line_count {
            get {
                if (string_.Length == 0)
                    return 0;

                int count = indexes_.Count + 1;
                if (indexes_.Count > 0) {
                    if (ends_in_enter())
                        --count;
                }
                return count;
            }
        }

        private bool ends_in_enter() {
            if (indexes_.Count > 0) {
                bool ends_in_enter = indexes_.Last() + 2 >= string_.Length;
                return ends_in_enter;
            } else
                return false;
        }

        public string line_at(int idx) {
            // note : it's possible to ask for an invalid line, while refreshing on the other thread
            //Debug.Assert(idx < line_count);

            if (idx == 0) 
                return (indexes_.Count > 0) ? string_.ToString(0, indexes_[0]) : "";

            if (idx < indexes_.Count) {
                int start = indexes_[idx - 1] + 1;
                // 1.3.11+ account for enters in all cases : '\r', '\n', '\r\n', '\n\r'
                if (string_[start] == '\r' || string_[start] == '\n')
                    ++start;

                int end = indexes_[idx];
                return string_.ToString(start, end - start);
            } else if (idx == indexes_.Count) {
                // last line
                int start = indexes_.Last() + 1;
                int end = string_.Length;
                if (end <= start)
                    return "";

                // 1.3.11+ account for enters in all cases : '\r', '\n', '\r\n', '\n\r'
                if (string_[start] == '\r' || string_[start] == '\n')
                    ++start;

                return string_.ToString(start, end - start);
            } else
                return "";
        }

        private int next_enter(int start_pos) {
            if (start_pos >= string_.Length)
                return -1;

            // special case - ended in '\r' and started with '\n' or vice versa
            if (start_pos > 0 && indexes_.Count > 0 && start_pos == indexes_.Last() + 1 ) {
                bool is_enter = (string_[start_pos - 1] == '\r' && string_[start_pos] == '\n') ||
                                      (string_[start_pos - 1] == '\n' && string_[start_pos] == '\r');
                if (is_enter)
                    return start_pos + 1;
            }

            int len = string_.Length;
            while ( start_pos < len)
                if (string_[start_pos] == '\r' || string_[start_pos] == '\n')
                    return start_pos;
                else
                    ++start_pos;

            return -1;
        }


        private void compute_indexes(int start_pos) {
            // never return the last empty line(s)
            while (true) {
                int next_pos = next_enter(start_pos);
                if (next_pos == -1)
                    break;

                indexes_.Add(next_pos);
                start_pos = next_pos + 2;
            }
        }
    }
}
