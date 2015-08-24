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
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;

namespace LogWizard
{
        // ctxX -> context about the message (other than file/func/class)
    enum info_type { time, date, level, 
        // not implemented yet
        thread, 
        
        class_, file, func, ctx1, ctx2, ctx3, 

        msg,
        max }

    class line {
        // pos in log - probably useless
        public readonly ulong pos_in_log;

        private string[] parts = new string[(int)info_type.max];

        public string full_line {
            get {
                string full = "";
                foreach ( string p in parts)
                    if (p != null)
                        full += p;
                return full;
            }
        }

        public line(ulong pos_in_log, string msg, Tuple<int, int>[] idx_in_line) {
            Debug.Assert(idx_in_line.Length == (int)info_type.max);
            this.pos_in_log = pos_in_log;

            for (int part_idx = 0; part_idx < idx_in_line.Length; ++part_idx) {
                var index = idx_in_line[part_idx];
                if ( index.Item1 >= 0)
                    if ((index.Item2 >= 0 && msg.Length >= index.Item1 + index.Item2) || (index.Item2 < 0 && msg.Length >= index.Item1)) {
                        string result = index.Item2 >= 0 ? msg.Substring(index.Item1, index.Item2) : msg.Substring(index.Item1);
                        parts[part_idx] = part_idx == (int)info_type.msg ? result : result.Trim();
                    }
            }
        }

        public string part(info_type i) {
            Debug.Assert(i < info_type.max);
            var result = parts[(int) i];
            return result ?? "";
        }
    }

    /* reads everything in the log_line_parser, and allows easy access to its lines
    */
    class log_line_parser : IDisposable {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        // if true, each line needs to be parsed (the positions of each part are relative)
        private bool relative_syntax_ = false;

        // [index, length]
        private Tuple<int,int>[] idx_in_line_ = new Tuple<int, int>[ (int)info_type.max];
        private readonly Tuple<int,int>[] line_contains_msg_only_ = new Tuple<int, int>[] {
            new Tuple<int,int>(0, -1),
            new Tuple<int,int>(-1, -1),
            new Tuple<int,int>(-1, -1),
            new Tuple<int,int>(-1, -1),
            new Tuple<int,int>(-1, -1),
            new Tuple<int,int>(-1, -1),
            new Tuple<int,int>(-1, -1),
            new Tuple<int,int>(-1, -1),
            new Tuple<int,int>(-1, -1),
            new Tuple<int,int>(-1, -1),
            new Tuple<int,int>(-1, -1)
        };

        private class relative_pos {
            public info_type type = info_type.max;
            // if >= 0, they are fixed
            public int start = -1, len = -1;
            // if != null, we find the start via the finding a string, or end via finding of a string
            public string start_str = null, end_str = null;

            public bool is_end_of_string {
                get { return start < 0 && len < 0 && start_str == null && end_str == null; }
            }
        }

        private const string LINE_SEP = "\r\n";

        private readonly text_reader text_reader_ = null;

        // readonly - set in constructor
        private List<relative_pos> relative_idx_in_line_ = new List<relative_pos>();

        private bool disposed_ = false;

        private List<line> lines_ = new List<line>();

        // for each of these readers, we have returned a "yes" to forced_reload
        private HashSet<log_line_reader> forced_reload_ = new HashSet<log_line_reader>();

        private ulong last_pos_ = 0;
        private bool was_last_line_incomplete_ = false;


        public log_line_parser(text_reader reader, string syntax) {
            Debug.Assert(reader != null);
            parse_syntax(syntax);
            text_reader_ = reader;
            force_reload();
            new Thread(refresh_thread) {IsBackground = true}.Start();
        }

        // once we've been forced to reload - we should return true once per each reader
        public bool forced_reload(log_line_reader reader) {
            lock (this) {
                if (forced_reload_.Contains(reader))
                    return false;
                forced_reload_.Add(reader);
                return true;
            }
        }

        private void refresh_thread() {
            while (!disposed_) {
                Thread.Sleep(100);
                read_to_end();
            }
        }

        public int line_count { get { lock(this) return lines_.Count; } }

        public string name {
            get { return text_reader_.name; }
        }

        public line line_at(int idx) {
            lock (this) {
                Debug.Assert(idx < lines_.Count);
                return lines_[idx];
            }
        }


        private Tuple<int, int> parse_syntax_pos(string syntax, string prefix) {
            try {
                int start_idx = syntax.IndexOf(prefix);
                if (start_idx >= 0) {
                    start_idx += prefix.Length;
                    int end_idx = syntax.IndexOf("]", start_idx);
                    string positions = syntax.Substring(start_idx, end_idx - start_idx);
                    int separator = positions.IndexOf(",");
                    if (separator >= 0) {
                        string[] pos = positions.Split(',');
                        return new Tuple<int, int>( int.Parse(pos[0]), int.Parse(pos[1]) );
                    }
                    else 
                        return new Tuple<int, int>( int.Parse(positions), -1 );
                }
            } catch {}

            return new Tuple<int, int>(prefix.StartsWith("msg") ? 0 : -1, -1);
        }

        private void parse_syntax(string syntax) {
            try {
                if (syntax.Contains("'")) {
                    parse_relative_syntax(syntax);
                    return;
                }
                idx_in_line_[(int) info_type.time] = parse_syntax_pos(syntax, "time[");
                idx_in_line_[(int) info_type.date] = parse_syntax_pos(syntax, "date[");
                idx_in_line_[(int) info_type.level] = parse_syntax_pos(syntax, "level[");
                idx_in_line_[(int) info_type.msg] = parse_syntax_pos(syntax, "msg[");
                idx_in_line_[(int) info_type.class_] = parse_syntax_pos(syntax, "class[");
                idx_in_line_[(int) info_type.file] = parse_syntax_pos(syntax, "file[");

                idx_in_line_[(int) info_type.func] = parse_syntax_pos(syntax, "func[");
                idx_in_line_[(int) info_type.ctx1] = parse_syntax_pos(syntax, "ctx1[");
                idx_in_line_[(int) info_type.ctx2] = parse_syntax_pos(syntax, "ctx2[");
                idx_in_line_[(int) info_type.ctx3] = parse_syntax_pos(syntax, "ctx3[");
                idx_in_line_[(int) info_type.thread] = parse_syntax_pos(syntax, "thread[");

                Debug.Assert(idx_in_line_.Length == line_contains_msg_only_.Length);
            } catch {
                // invalid syntax
            }
        }

        // it can be a number (index) or a string (to search for)
        private Tuple<int, string> parse_sub_relative_syntax(ref string syntax) {
            if ( syntax == "")
                // invalid syntax
                return new Tuple<int, string>(-1,null);

            if (Char.IsDigit(syntax[0])) {
                int end = 1;
                while (Char.IsDigit(syntax[end]) && end < syntax.Length)
                    ++end;
                int number = int.Parse(syntax.Substring(0, end));
                syntax = syntax.Substring(end);
                if (syntax != "")
                    // ignore the delimeter after the number
                    syntax = syntax.Substring(1);
                return new Tuple<int, string>(number,null);
            }
            else if (syntax[0] == '\'') {
                syntax = syntax.Substring(1);
                // at this point, we assume we're not searching for ' inside the string (which would be double-quoted or something)
                int end = syntax.IndexOf('\'');
                if (end != -1) {
                    string str = syntax.Substring(0, end);
                    syntax = syntax.Substring(end + 1);
                    if (syntax != "")
                        // ignore the delimeter after the quoted string
                        syntax = syntax.Substring(1);
                    return new Tuple<int, string>(-1, str);
                }
            } 

            // invalid syntax
            syntax = "";
            return new Tuple<int, string>(-1, null);            
        }

        private void parse_relative_syntax(string syntax) {
            relative_syntax_ = true;
            relative_idx_in_line_.Clear();

            // Example: "$time[0,12] $ctx1['[','-'] $func[' ',']'] $ctx2['[[',' ] ]'] $msg"
            syntax = syntax.Trim();
            while (syntax.Length > 0) {
                if (syntax[0] != '$')
                    // invalid syntax
                    break;

                syntax = syntax.Substring(1);
                string type_str = syntax.Split('[')[0];
                info_type type = info_type.max;
                switch (type_str) {
                case "time":
                    type = info_type.time;
                    break;
                case "date":
                    type = info_type.date;
                    break;
                case "level":
                    type = info_type.level;
                    break;
                case "msg":
                    type = info_type.msg;
                    break;
                case "class":
                    type = info_type.class_;
                    break;
                case "file":
                    type = info_type.file;
                    break;
                case "func":
                    type = info_type.func;
                    break;
                case "ctx1":
                    type = info_type.ctx1;
                    break;
                case "ctx2":
                    type = info_type.ctx2;
                    break;
                case "ctx3":
                    type = info_type.ctx3;
                    break;
                default:
                    break;
                }
                if (type == info_type.max)
                    // invalid syntax
                    break;
                int bracket = syntax.IndexOf("[");
                syntax = bracket >= 0 ? syntax.Substring(bracket + 1).Trim() : "";
                if (syntax == "") {
                    // this was the last item (the remainder of the string)
                    relative_idx_in_line_.Add( new relative_pos {
                        type = type, start = -1, start_str = null, len = -1, end_str = null
                    });
                    break;
                }

                var start = parse_sub_relative_syntax(ref syntax);
                var end = parse_sub_relative_syntax(ref syntax);
                relative_idx_in_line_.Add( new relative_pos {
                    type = type, start = start.Item1, start_str = start.Item2, len = end.Item1, end_str = end.Item2
                });

                syntax = syntax.Trim();
            }
        }

        // forces the WHOLE FILE to be reloaded
        public void force_reload() {
            lock (this) {
                last_pos_ = 0;
                was_last_line_incomplete_ = false;
                forced_reload_.Clear();
                lines_.Clear();
                text_reader_.pos = 0;
                logger.Info("log reloaded: " + text_reader_.name);
            }
        }


        private void read_to_end() {
            ulong old_len = text_reader_.full_len;
            text_reader_.compute_full_length();
            ulong new_len = text_reader_.full_len;
            if (old_len > new_len) 
                // file got re-written
                force_reload();

            ulong pos_in_log;
            lock(this)
                pos_in_log = last_pos_;
            text_reader_.pos = pos_in_log;
            int remaining = (int)(text_reader_.full_len - text_reader_.pos);
            if (remaining < 1)
                return;
            string[] cur_lines = text_reader_.read_next_text(remaining).Split(new string[] { LINE_SEP }, StringSplitOptions.None);
            if (cur_lines.Length < 1)
                return;

            int start_idx = 0;
            var first_line = parse_line(cur_lines[0], pos_in_log);
            lock(this)
                if ( lines_.Count > 0 && was_last_line_incomplete_) {
                    // we re-parse the last line (which was previously incomplete)
                    lines_[ lines_.Count - 1] = first_line;
                    pos_in_log += (ulong)cur_lines[0].Length + (ulong)LINE_SEP.Length;
                    start_idx = 1;
                }

            int end_idx = cur_lines.Length;
            // find the first non-empty line
            for (; end_idx > 0 && cur_lines[end_idx - 1] == ""; --end_idx) {
            }

            List<line> now = new List<line>();
            for ( int i = start_idx; i < end_idx; ++i) {
                now.Add( parse_line( cur_lines[i], pos_in_log));
                pos_in_log += (ulong)cur_lines[i].Length + (ulong)LINE_SEP.Length;
            }

            lock (this) {
                lines_.AddRange(now);
                was_last_line_incomplete_ = cur_lines[cur_lines.Length - 1] != "";
                last_pos_ = pos_in_log;
            }
        }

        private bool parse_time(string line, Tuple<int,int> idx) {
            if (idx.Item1 < 0)
                return true;
            if (line.Length < idx.Item2)
                // we don't have enough line to hold the time/date/level
                return false;
            string sub = line.Substring(idx.Item1, idx.Item2 - idx.Item1);
            int max = Math.Min(idx.Item2, 8); // ignore milis
            bool ok = true;
            for ( int i = 0; i < max; ++i)
                if ((i + 1) % 3 == 0)
                    ok = ok && sub[i] == ':';
                else
                    ok = Char.IsDigit(sub[i]);
            return ok;
        }
        private bool parse_date(string line, Tuple<int,int> idx) {
            if (idx.Item1 < 0)
                return true;
            if (line.Length < idx.Item2)
                // we don't have enough line to hold the time/date/level
                return false;

            string sub = line.Substring(idx.Item1, idx.Item2 - idx.Item1);
            int digits = 0, sep = 0;
            foreach ( char c in sub)
                if (Char.IsDigit(c) || Char.IsWhiteSpace(c))
                    ++digits;
                else if (c == '-' || c == '/')
                    ++sep;
                else
                    return false;
            return sep == 2;
        }

        private bool parse_level(string line, Tuple<int,int> idx) {
            if (idx.Item1 < 0)
                return true;
            if (line.Length < idx.Item2)
                // we don't have enough line to hold the time/date/level
                return false;
            
            string sub = line.Substring(idx.Item1, idx.Item2 - idx.Item1).TrimEnd();
            return sub == "INFO" || sub == "ERROR" || sub == "FATAL" || sub == "DEBUG" || sub == "WARN";
        }


        // if at exit of function, idx < 0, the line could not be parsed
        private Tuple<int, int> parse_relative_part(string l, relative_pos part, ref int idx) {
            if (part.is_end_of_string) {
                // return remainder of the string
                int at = idx;
                idx = l.Length;
                return new Tuple<int, int>(at, l.Length - at);
            }

            int start = -1, end = -1;
            if (part.start >= 0)
                start = part.start;
            else {
                if ( idx >= l.Length)
                    // passed the end of string
                    return new Tuple<int, int>(-1, -1);

                start = l.IndexOf(part.start_str, idx);
                idx = start >= 0 ? start + part.start_str.Length : -1;
                if (start >= 0)
                    start += part.start_str.Length;
            }

            if ( idx >= 0)
                if (part.len >= 0) {
                    end = start + part.len;
                    idx = end;
                } else {
                    end = l.IndexOf(part.end_str, idx);
                    idx = end >= 0 ? end + part.end_str.Length : -1;
                }

            return new Tuple<int, int>(start, end - start);
        }

        private line parse_relative_line(string l, ulong pos_in_log) {
            List< Tuple<int,int> > indexes = new List<Tuple<int, int>>();
            for ( int i = 0; i < (int)info_type.max; ++i)
                indexes.Add(new Tuple<int,int>(-1,-1));

            int cur_idx = 0;
            int correct_count = 0;
            foreach (var rel in relative_idx_in_line_) {
                if (cur_idx < 0)
                    break;
                var index = parse_relative_part(l, rel, ref cur_idx);
                if (index.Item1 >= 0 && index.Item2 >= 0) {
                    indexes[(int) rel.type] = index;
                    ++correct_count;
                }
            }

            // if we could parse time or date, we consider it an OK line
            bool normal_line = correct_count == relative_idx_in_line_.Count;
            line new_ = new line(pos_in_log, l, normal_line ? indexes.ToArray() : line_contains_msg_only_);
            return new_;
        }


        private line parse_line(string l, ulong pos_in_log) {
            if (relative_syntax_)
                return parse_relative_line(l, pos_in_log);

            bool normal_line = parse_time(l, idx_in_line_[(int) info_type.time]) && parse_date(l, idx_in_line_[(int) info_type.date]);
            if ( idx_in_line_[ (int)info_type.time].Item1 < 0 && idx_in_line_[ (int)info_type.date].Item1 < 0)
                // in this case, we don't have time & date - see that the level matches
                // note: we can't rely on level too much, since the user might have additional levels that our defaults - so we could get false negatives
                normal_line = parse_level(l, idx_in_line_[(int) info_type.level]);

            line new_ = new line(pos_in_log, l, normal_line ? idx_in_line_ : line_contains_msg_only_);
            return new_;
        }

        public void Dispose() {
            disposed_ = true;
        }
    }
}
