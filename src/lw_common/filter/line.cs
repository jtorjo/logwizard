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

namespace lw_common {

    public class line {
        public class exception : Exception {
            public exception(string message = "invalid line") : base(message) {
            }
        }

        private sub_string sub_;
        // note: I could theoretically keep the length as a ubyte - and only the length of the message as a short
        //       however, I don't think it's worth doing, since if in the future, I would parse more complicated logs,
        //       we could end up with several parts of the message being bigger than 255 chars, so we'd have to come back to this again
        //
        //      thus, not worth doing
        private short[] parts = new short[(int) info_type.max * 2];

        // if not minvalue, it's the time this message was written
        public DateTime time = DateTime.MinValue;

        // for debugging
        public override string ToString() {
            return sub_.msg;
        }

        // for ***fast*** comparing to see if it contains some text (used in search)
        // if the comparison returns true, you will need to query each part to see where the text was found
        public string raw_full_msg() {
            return sub_.msg;
        }

        public string full_line {
            get {
                string full = "";
                foreach (info_type i in Enum.GetValues(typeof (info_type)))
                    if (i != info_type.max) {
                        string sub = part(i);
                        if (sub != "")
                            full += sub + " ";
                    }
                return full;
            }
        }

        private line() {
            sub_ = new sub_string(null, 0);
        }

        public static line empty_line() {
            line l = new line();
            for (int i = 0; i < l.parts.Length / 2; ++i) {
                l.parts[i * 2] = -1;
                l.parts[i * 2 + 1] = 0;
            }
            return l;
        }

        public line(sub_string sub, Tuple<int, int>[] idx_in_line) : this(sub, idx_in_line, DateTime.MinValue) {
        }

        public line(sub_string sub, Tuple<int, int>[] idx_in_line, DateTime time ) {
            sub_ = sub;
            Debug.Assert(idx_in_line.Length == (int) info_type.max);
            string msg = sub.msg;
            // ... indexes a short can hold
            Debug.Assert(msg.Length < 32768);

            for (int part_idx = 0; part_idx < idx_in_line.Length; ++part_idx) {
                var index = idx_in_line[part_idx];
                parts[part_idx * 2] = parts[part_idx * 2 + 1] = -1;

                if (index.Item1 >= 0)
                    if ((index.Item2 >= 0 && msg.Length >= index.Item1 + index.Item2) || (index.Item2 < 0 && msg.Length >= index.Item1)) {
                        short start, len;
                        if (index.Item2 >= 0) {
                            start = (short) index.Item1;
                            len = (short) index.Item2;
                        } else {
                            start = (short) index.Item1;
                            len = (short) (msg.Length - index.Item1);
                        }
                        if ( part_idx == (int)info_type.msg)
                            if (index.Item2 == -1)
                                // 1.8.4 - allow merging several lines into one (that is, merging line X+1 to X; in this case,
                                //         the length needs to be "variable")
                                len = -1;

                        bool needs_trim = part_idx != (int) info_type.msg;
                        if (needs_trim)
                            while (len > 0)
                                if (Char.IsWhiteSpace(msg[start + len - 1]))
                                    --len;
                                else
                                    break;

                        parts[part_idx * 2] = start;
                        parts[part_idx * 2 + 1] = len;
                    }
            }

            if (time != DateTime.MinValue) 
                this.time = time;
            else {
                // normalize time - so that we can do proper comparisons when "Go to Line"
                var time_str = part(info_type.time);
                var date_str = part(info_type.date);
                if (time_str != "")
                    this.time = util.str_to_normalized_datetime(date_str, time_str);
            }
        }

        public string part(info_type i) {
            Debug.Assert(i < info_type.max);
            if (parts[(int) i * 2] < 0 )
                return "";

            string result = "";
            string msg = sub_.msg;
            try {
                short start = parts[(int) i * 2], len = parts[(int) i * 2 + 1];
                if (start < msg.Length && start + len <= msg.Length)
                    result = len < 0 ? msg.Substring(start) : msg.Substring(start, len);
            } catch {
                // this can happen when the log has changed or has been re-written, thus, the sub_ has become suddenly empty
            }

            if (app.inst.force_text_as_multi_line)
                result = util.split_into_multiple_fixed_lines(result, 15);

            return result;
        }
    }
}
