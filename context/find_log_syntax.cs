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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using lw_common;

namespace LogWizard.readers {
    class find_log_syntax {
        public const string UNKNOWN_SYNTAX = "$msg[0]";

        public const int READ_TO_GUESS_SYNTAX = 8192;

        public string try_to_find_log_syntax(string file) {
            try {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    string found = new find_log_syntax().try_find_log_syntax(fs);
                    if (found != UNKNOWN_SYNTAX)
                        return  found;
                }
            } catch {
            }            
            return UNKNOWN_SYNTAX;
        }

        public string try_find_log_syntax(FileStream fs) {
            try {
                var encoding = util.file_encoding(fs);
                if (encoding == null)
                    encoding = Encoding.Default;
                long pos = fs.Position;
                fs.Seek(0, SeekOrigin.Begin);
            
                // read a few lines from the beginning
                byte[] readBuffer = new byte[READ_TO_GUESS_SYNTAX];
                int bytes = fs.Read(readBuffer, 0, READ_TO_GUESS_SYNTAX);
                string now = encoding.GetString(readBuffer, 0, bytes);
                string[] lines = now.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                // go back to where we were
                fs.Seek(pos, SeekOrigin.Begin);
                string found = try_find_log_syntax(lines);
                return found;
            } catch {
                return UNKNOWN_SYNTAX;
            }
        }

        public string try_find_log_syntax(string[] lines) {
            if ( lines.Length < 5)
                return UNKNOWN_SYNTAX;

            string syntax = "";
            // see if we have $(file) first
            int pos1 = lines[0].IndexOf("\\"), pos2 = lines[0].IndexOf("\\");
            if ( pos1 < 20 && pos2 < 20)
                syntax += "$(file) ";

            List<int> times = new List<int>(), dates = new List<int>(), levels = new List<int>();
            foreach ( string line in lines) {
                times.Add( time_or_date_pos(line, ':'));
                
                int by_minus = time_or_date_pos(line, '-');
                int by_div = time_or_date_pos(line, '/');
                dates.Add(by_minus > 0 ? by_minus : by_div);

                int info = line.IndexOf(" INFO ");
                int err = line.IndexOf(" ERROR ");
                int fatal = line.IndexOf(" FATAL ");
                int dbg = line.IndexOf(" DEBUG ");
                int warn = line.IndexOf(" WARN ");
                if ( info > 0) levels.Add(info);
                else if ( err > 0) levels.Add(err);
                else if ( fatal > 0) levels.Add(fatal);
                else if ( dbg > 0) levels.Add(dbg);
                else if ( warn > 0) levels.Add(warn);
            }

            int[] all = new[] { times.Count, dates.Count, levels.Count };
            int min = all.Min(), max = all.Max();
            if (max - min > 1)
                // one of the times/dates/levels did not work                
                return UNKNOWN_SYNTAX;

            // at this point, we might have one line that was not fully parsed (thus, not all : time/date/level are parsed -> thus, we should ignore it)
            if (max - min == 1) {
                if ( times.Count == max)
                    times.RemoveAt(min);
                if ( dates.Count == max)
                    dates.RemoveAt(min);
                if ( levels.Count == max)
                    levels.RemoveAt(min);
            }

            bool ok = is_consistently_sorted(times,dates) != 0 && is_consistently_sorted(times,levels) != 0 && is_consistently_sorted(dates,levels) != 0;
            if ( !ok)
                // full line is a message
                return UNKNOWN_SYNTAX;

            List< Tuple<string,List<int>> > sorted = new List<Tuple<string,List<int>>>();
            // ... better to take the second line, since the first line might not be complete
            string first_line = lines.Length > 1 ? lines[1] : lines[0];
            List<int> end_indexes = new List<int>();
            if (is_consistently_present(times)) {
                int len = end_of_time_index(first_line, times[0]) - times[0];
                end_indexes.Add(times[0] + len);
                sorted.Add(new Tuple<string, List<int>>("$time[" + times[0] + "," + len + "]", times));
            }
            if (is_consistently_present(dates)) {
                int len = end_of_date_index(first_line, dates[0]) - dates[0];
                end_indexes.Add(dates[0] + len);
                sorted.Add(new Tuple<string, List<int>>("$date[" + dates[0] + "," + len + "]", dates));
            }
            if (is_consistently_present(levels)) {
                int len = 5;
                end_indexes.Add(levels[0] + 1 + len);
                sorted.Add(new Tuple<string, List<int>>("$level[" + (levels[0] + 1) + "," + len + "]", levels));
            }
            sorted.Sort( (x,y) => is_consistently_sorted(x.Item2,y.Item2) );

            foreach ( Tuple<string,List<int>> sli in sorted) 
                syntax += sli.Item1 + " ";

            end_indexes.Sort();
            if (end_indexes.Count > 0) {
                int start_msg = end_indexes.Last() + 1;
                // account for logs that have a starting line at beginning of msg
                if (first_line[start_msg] == '-')
                    ++start_msg;
                while (start_msg < first_line.Length && Char.IsWhiteSpace(first_line[start_msg]))
                    ++start_msg;
                syntax += "$msg[" + start_msg + "]";
            } else
                syntax += UNKNOWN_SYNTAX;
            return syntax;
        }

        private static bool is_ds(char c) {
            return Char.IsDigit(c) || Char.IsWhiteSpace(c);
        }

        // tries to find if we have a time/date here - if yes, returns pos, otherwise -1
        private int time_or_date_pos(string line, char separator) {
            // we're looking for this config:
            // ddSddSdd - d = digit, S = separator
            List<int> positions = new List<int>();
            int pos = -1;
            while ( line.IndexOf(separator, pos+1) > 0) {
                pos = line.IndexOf(separator, pos+1);
                positions.Add(pos);
            }

            for ( int i = 0; i < positions.Count - 1; ++i) 
                if ( positions[i] + 3 == positions[i+1]) 
                    if ( positions[i] >= 2 && positions[i] + 5 < line.Length) {
                        int at = positions[i];
                        bool ok = is_ds( line[at-2]) && is_ds( line[at-1]) && is_ds( line[at+1]) && is_ds( line[at+2]) && is_ds( line[at+4]) && is_ds( line[at+5]);
                        if ( ok)
                            return at - 2;
                    }
            return -1;
        }

        // returns true if the first list is consistently bigger than the second or consistently less
        // note that -1 values don't count
        //
        // returns: 0 - inconstency, -1 = less, 1 = bigger
        private static int is_consistently_sorted(List<int> a, List<int> b) {
            if (a.Count != b.Count)
                return 0; // inconsistent

            double less = 0, invalid = 0, bigger = 0;
            for ( int i = 0; i < a.Count; ++i) {
                if ( a[i] < 0 || b[i] < 0) ++invalid;
                else if ( a[i] < b[i]) ++less;
                else ++bigger;
            }

            if ( invalid == a.Count)
                // one of the arrays contains only invalid values - that means we don't have that pattern in the log_line_parser
                // for instane, we don't have times in the log_line_parser lines
                return a[0] == -1 ? -1 : 1;

            bool valid_less = less / (bigger + less) > .7;
            bool valid_bigger = bigger / (bigger + less) > .7;

            if ( valid_less) return -1;
            else if ( valid_bigger) return 1;
            else return 0;
        }

        private bool is_consistently_present(List<int> list) {
            if (list.Count == 0)
                return false;
            int present = 0;
            foreach ( int i in list)
                if ( i >= 0) 
                    ++present;
            // make sure it's more than 70%
            return present >= (double)list.Count * .7;
        }

        // find out if "HH:MM:SS" or "HH:MM:SS,ZZZ"
        private int end_of_time_index(string line, int start_of_time) {
            Debug.Assert(line.Length > start_of_time + 8);
            int end_of_time = start_of_time + 8;
            if (line[end_of_time] == ',' || line[end_of_time] == '.') {
                // it contains milliseconds as well
                ++end_of_time;
                while (Char.IsDigit(line[end_of_time]))
                    ++end_of_time;
            }
            return end_of_time;
        }

        // assume DD:MM:YY or DD:MM:YYYY
        private int end_of_date_index(string line, int start_of_date) {
            int end_of_date = start_of_date + 8;
            if (Char.IsDigit(line[end_of_date]))
                // year is 4-digit
                end_of_date += 2;
            return end_of_date;
        }

    }
}
