using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net.Repository.Hierarchy;

namespace LogWizard
{
    // holds the lines in a huge string
    //
    // note: at this time, we assume the enter is formed of 2 chars - either \r\n or \n\r
    class large_string
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string LINE_SEP = "\r\n";
        private StringBuilder string_ = new StringBuilder();

        private List<int> indexes_ = new List<int>();

        private bool test_we_computed_lines_correctly = false; //util.is_debug;

        // tests to see we've computed the lines correctly
        private void test_compute_lines() {
            string[] lines = string_.ToString().Split(new string[] {LINE_SEP}, StringSplitOptions.None);
            Debug.Assert(lines.Length == line_count);
            for (int i = 0; i < lines.Length; ++i) {
                string cur = line_at(i);
                Debug.Assert(lines[i] == cur);
            }
        }

        public void expect_bytes(ulong byte_count) {
            Debug.Assert( byte_count < int.MaxValue);
            string_.Capacity = (int)byte_count;
        }

        public void add_lines(string lines, ref int added_line_count, ref bool was_last_line_incomplete, ref bool is_last_line_incomplete) {
            if (lines == "")
                return;

            was_last_line_incomplete = false;
            if (string_.Length > 0 && lines.Length > 0) {
                
                bool might_be_incomplete = line_count > indexes_.Count;
                bool starts_with_enter = LINE_SEP.Contains(lines[0]);
                was_last_line_incomplete = might_be_incomplete && !starts_with_enter;
            }

            int len = string_.Length;
            string_.Append(lines);
            int old_line_count = line_count;
            compute_indexes(len);
            added_line_count = line_count - old_line_count;

            is_last_line_incomplete = line_count > indexes_.Count;
            logger.Debug("[line] we have read " + line_count + " lines");

            if (test_we_computed_lines_correctly)
                test_compute_lines();
        }

        public void set_lines(string lines, ref int line_count) {
            indexes_.Clear();
            string_.Clear();
            string_.Append(lines);
            compute_indexes(0);
            line_count = this.line_count;

            if (test_we_computed_lines_correctly)
                test_compute_lines();
        }

        public void clear() {
            indexes_.Clear();
            string_.Clear();            
        }

        public int line_count {
            get {
                if (string_.Length == 0)
                    return 0;

                int count = indexes_.Count + 1;
                if (indexes_.Count > 0) {
                    bool ends_in_enter = indexes_.Last() + 2 >= string_.Length;
                    if (ends_in_enter)
                        --count;
                }
                return count;
            }
        }

        public string line_at(int idx) {
            Debug.Assert(idx < line_count);

            if (idx == 0) 
                return (indexes_.Count > 0) ? string_.ToString(0, indexes_[0]) : "";

            if (idx < indexes_.Count) {
                int start = indexes_[idx - 1] + 2;
                int end = indexes_[idx];
                return string_.ToString(start, end - start);
            } else if (idx == indexes_.Count) {
                // last line
                int start = indexes_.Last() + 2;
                int end = string_.Length;
                if (end <= start)
                    return "";
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
