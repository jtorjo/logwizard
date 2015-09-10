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
using System.Windows.Forms;

namespace LogWizard
{

    /* reads everything in the log_line_parser, and allows easy access to its lines
    */
    class log_line_parser : IDisposable {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private class syntax_info {
            public syntax_info() {
                line_contains_msg_only_[(int) info_type.msg] = new Tuple<int, int>(0, -1);                
            }
            
            // if true, each line needs to be parsed (the positions of each part are relative)
            public bool relative_syntax_ = false;

            // [index, length]
            public Tuple<int,int>[] idx_in_line_ = new Tuple<int, int>[ (int)info_type.max];
            public readonly Tuple<int,int>[] line_contains_msg_only_ = new Tuple<int, int>[] {
                new Tuple<int,int>(-1, -1),
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

            public class relative_pos {
                public info_type type = info_type.max;
                // if >= 0, they are fixed
                public int start = -1, len = -1;
                // if != null, we find the start via the finding a string, or end via finding of a string
                public string start_str = null, end_str = null;

                public bool is_end_of_string {
                    get { return start < 0 && len < 0 && start_str == null && end_str == null; }
                }
            }

            // readonly - set in constructor
            public List<relative_pos> relative_idx_in_line_ = new List<relative_pos>();
        }

        private List<syntax_info> syntaxes_ = new List<syntax_info>();


        private const string LINE_SEP = "\r\n";
        // FIXME not a good idea
        private const int CACHE_LAST_INCOMPLETE_LINE_MS = 50000;

        private readonly text_reader text_reader_ = null;


        private bool disposed_ = false;

        private large_string string_ = new large_string();
        private List<line> lines_ = new List<line>();

        // for each of these readers, we have returned a "yes" to forced_reload
        private HashSet<log_line_reader> forced_reload_ = new HashSet<log_line_reader>();

        private DateTime was_last_line_incomplete_ = DateTime.MinValue;

        // if true, we've been fully read (thus, we're up to date)
        private bool up_to_date_ = false;

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

        public int line_count {
            get {
                lock (this) {
                    int count = lines_.Count;
                    if ( was_last_line_incomplete_ != DateTime.MinValue)
                        if (was_last_line_incomplete_.AddMilliseconds(CACHE_LAST_INCOMPLETE_LINE_MS) > DateTime.Now)
                            // we're not sure if the last line was fully read - assume not...
                            --count;
                    return count;
                }
            }
        }

        public string name {
            get { return text_reader_.name; }
        }

        public bool up_to_date {
            get { return up_to_date_;  }
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

        private syntax_info parse_single_syntax(string syntax) {
            try {
                if (syntax.Contains("'")) 
                    return parse_relative_syntax(syntax);

                syntax_info si = new syntax_info();
                si.idx_in_line_[(int) info_type.time] = parse_syntax_pos(syntax, "time[");
                si.idx_in_line_[(int) info_type.date] = parse_syntax_pos(syntax, "date[");
                si.idx_in_line_[(int) info_type.level] = parse_syntax_pos(syntax, "level[");
                si.idx_in_line_[(int) info_type.msg] = parse_syntax_pos(syntax, "msg[");
                si.idx_in_line_[(int) info_type.class_] = parse_syntax_pos(syntax, "class[");
                si.idx_in_line_[(int) info_type.file] = parse_syntax_pos(syntax, "file[");

                si.idx_in_line_[(int) info_type.func] = parse_syntax_pos(syntax, "func[");
                si.idx_in_line_[(int) info_type.ctx1] = parse_syntax_pos(syntax, "ctx1[");
                si.idx_in_line_[(int) info_type.ctx2] = parse_syntax_pos(syntax, "ctx2[");
                si.idx_in_line_[(int) info_type.ctx3] = parse_syntax_pos(syntax, "ctx3[");
                si.idx_in_line_[(int) info_type.thread] = parse_syntax_pos(syntax, "thread[");

                Debug.Assert(si.idx_in_line_.Length == si.line_contains_msg_only_.Length);
                return si;
            } catch {
                // invalid syntax
                return null;
            }
        }

        private void parse_syntax(string syntax) {
            try {
                string[] several = syntax.Split(new string[] {"|"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string single in several) {
                    var parsed = parse_single_syntax(single);
                    if ( parsed != null)
                        syntaxes_.Add(parsed);
                }
            } catch {
                // invalid syntax - use whatever works
            }
            if ( syntaxes_.Count < 1)
                // in this case - can't parse syntax - treat each line as msg-only
                syntaxes_.Add(new syntax_info());
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

        private syntax_info parse_relative_syntax(string syntax) {
            syntax_info si = new syntax_info();
            si.relative_syntax_ = true;
            si.relative_idx_in_line_.Clear();

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
                    si.relative_idx_in_line_.Add( new syntax_info.relative_pos {
                        type = type, start = -1, start_str = null, len = -1, end_str = null
                    });
                    break;
                }

                var start = parse_sub_relative_syntax(ref syntax);
                var end = parse_sub_relative_syntax(ref syntax);
                si.relative_idx_in_line_.Add( new syntax_info.relative_pos {
                    type = type, start = start.Item1, start_str = start.Item2, len = end.Item1, end_str = end.Item2
                });

                syntax = syntax.Trim();
            }
            return si;
        }

        // forces the WHOLE FILE to be reloaded
        public void force_reload() {
            lock (this) {
                was_last_line_incomplete_ = DateTime.MinValue;
                forced_reload_.Clear();
                lines_.Clear();
                up_to_date_ = false;
                logger.Info("[log] log reloaded: " + text_reader_.name);
            }
            string_.clear();
            text_reader_.force_reload();
        }


        private void read_to_end() {
            ulong old_len = text_reader_.full_len;
            text_reader_.compute_full_length();
            ulong new_len = text_reader_.full_len;
            // when reader's position is zero -> it's either the first time, or file was re-rewritten
            if (old_len > new_len || text_reader_.pos == 0) 
                // file got re-written
                force_reload();

            bool fully_read = old_len == new_len && text_reader_.is_up_to_date();

            int remaining = (int)(text_reader_.full_len - text_reader_.pos);
            if (remaining < 1) {
                lock (this) {
                    up_to_date_ = fully_read;
                    if ( up_to_date_)
                        // at this point, we're sure we read everything
                        was_last_line_incomplete_ = DateTime.MinValue;
                }
                return;
            }
            
            lock (this)
                up_to_date_ = false;

            string text = text_reader_.read_next_text();
            int added_line_count = 0;
            bool was_last_line_incomplete = false, is_last_line_incomplete = false;
            int old_line_count = string_.line_count;
            string_.add_lines(text, ref added_line_count, ref was_last_line_incomplete, ref is_last_line_incomplete);

            if (added_line_count < 1)
                return;

            bool needs_reparse_last_line;
            lock (this)
                needs_reparse_last_line = lines_.Count > 0 && was_last_line_incomplete ;
            int start_idx = old_line_count - (was_last_line_incomplete ? 1 : 0);
            int end_idx = string_.line_count;
            List<line> now = new List<line>();
            for ( int i = start_idx; i < end_idx; ++i) 
                now.Add( parse_line( new sub_string(string_,i) ));

            lock (this) {
                if (needs_reparse_last_line) {
                    // we re-parse the last line (which was previously incomplete)
                    logger.Debug("[line] reparsed line " + (old_line_count-1) );
                    lines_.RemoveAt( lines_.Count - 1);
                }

                int old_count = lines_.Count;
                lines_.AddRange(now);
                for ( int idx = old_count; idx < lines_.Count; ++idx)
                    adjust_line_time(idx);
                was_last_line_incomplete_ = was_last_line_incomplete ? DateTime.Now : DateTime.MinValue;
            }
            Debug.Assert( lines_.Count == string_.line_count);
        }

        // if the time isn't set - try to use it from the surroundings
        private void adjust_line_time(int idx) {
            if (lines_[idx].time != DateTime.MinValue)
                return;

            int src_idx = idx - 1;
            for ( ; src_idx >= 0; --src_idx)
                if (lines_[src_idx].time != DateTime.MinValue) {
                    lines_[idx].time = lines_[src_idx].time;
                    return;
                }

            for ( src_idx = idx + 1; src_idx < lines_.Count; ++src_idx)
                if (lines_[src_idx].time != DateTime.MinValue) {
                    lines_[idx].time = lines_[src_idx].time;
                    return;                    
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
        private Tuple<int, int> parse_relative_part(string l, syntax_info.relative_pos part, ref int idx) {
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
                    return null;

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
                    if (part.end_str != null)
                        end = l.IndexOf(part.end_str, idx);
                    else {
                        end = l.Length;
                        if ( part.start_str != null)
                            start -= part.start_str.Length;
                    }
                    idx = end >= 0 ? end + (part.end_str != null ? part.end_str.Length : 0) : -1;
                }

            return ( start < l.Length && end <= l.Length) ? new Tuple<int, int>(start, end - start) : null;
        }

        private line parse_relative_line(sub_string l, syntax_info si) {
            List< Tuple<int,int> > indexes = new List<Tuple<int, int>>();
            for ( int i = 0; i < (int)info_type.max; ++i)
                indexes.Add(new Tuple<int,int>(-1,-1));

            string sub = l.msg;
            int cur_idx = 0;
            int correct_count = 0;
            foreach (var rel in si.relative_idx_in_line_) {
                if (cur_idx < 0)
                    break;
                var index = parse_relative_part(sub, rel, ref cur_idx);
                if (index == null)
                    return null;
                if (index.Item1 >= 0 && index.Item2 >= 0) {
                    indexes[(int) rel.type] = index;
                    ++correct_count;
                }
            }

            // if we could parse time or date, we consider it an OK line
            bool normal_line = correct_count == si.relative_idx_in_line_.Count;
            return normal_line ? new line(l, indexes.ToArray()) : null ;
        }

        // returns null if it can't parse
        private line parse_line_with_syntax(sub_string l, syntax_info si) {
            if (si.relative_syntax_)
                return parse_relative_line(l, si);

            try {
                string sub = l.msg;
                bool normal_line = parse_time(sub, si.idx_in_line_[(int) info_type.time]) && parse_date(sub, si.idx_in_line_[(int) info_type.date]);
                if (si.idx_in_line_[(int) info_type.time].Item1 < 0 && si.idx_in_line_[(int) info_type.date].Item1 < 0)
                    // in this case, we don't have time & date - see that the level matches
                    // note: we can't rely on level too much, since the user might have additional levels that our defaults - so we could get false negatives
                    normal_line = parse_level(sub, si.idx_in_line_[(int) info_type.level]);

                return normal_line ? new line(l, si.idx_in_line_) : null;
            } catch(Exception e) {
                logger.Error("invalid line: " + l);
                //return new line(pos_in_log, l, line_contains_msg_only_);
                return null;
            }
        }

        private line parse_line(sub_string l) {
            Debug.Assert(syntaxes_.Count > 0);

            foreach (var si in syntaxes_) {
                line result = null;
                if (si.relative_syntax_)
                    result = parse_relative_line(l, si);
                else
                    result = parse_line_with_syntax(l, si);

                if (result != null)
                    return result;
            }

            // in this case, we can't parse the line at all - use default
            return new line(l, syntaxes_[0].line_contains_msg_only_);
        }

        public void Dispose() {
            disposed_ = true;
        }
    }
}
