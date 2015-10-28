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
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using log4net.Repository.Hierarchy;

namespace lw_common {
    public enum part_type {
        date, time, level, message, file, func, thread, 
            
        ctx1, ctx2, ctx3, 
            
        // added in 1.3.8
        ctx4, ctx5, ctx6, ctx7, ctx8, ctx9, ctx10, ctx11, ctx12, ctx13, ctx14, ctx15,

        class_, font, case_sensitive_info,
        
        invalid
    }

    internal static class part_type_io {
        internal static part_type from_str(string str) {
            part_type part = part_type.invalid;
            switch ( str) {
            case "$msg": part = part_type.message; break;

            case "$date": part = part_type.date; break;
            case "$time": part = part_type.time; break;
            case "$level": part = part_type.level; break;
            case "$file": part = part_type.file; break;
            case "$func": part = part_type.func; break;
            case "$class": part = part_type.class_; break;
            case "$thread": part = part_type.thread; break;

            case "$ctx1": part = part_type.ctx1; break;
            case "$ctx2": part = part_type.ctx2; break;
            case "$ctx3": part = part_type.ctx3; break;

            case "$ctx4": part = part_type.ctx4; break;
            case "$ctx5": part = part_type.ctx5; break;
            case "$ctx6": part = part_type.ctx6; break;
            case "$ctx7": part = part_type.ctx7; break;
            case "$ctx8": part = part_type.ctx8; break;
            case "$ctx9": part = part_type.ctx9; break;
            case "$ctx10": part = part_type.ctx10; break;

            case "$ctx11": part = part_type.ctx11; break;
            case "$ctx12": part = part_type.ctx12; break;
            case "$ctx13": part = part_type.ctx13; break;
            case "$ctx14": part = part_type.ctx14; break;
            case "$ctx15": part = part_type.ctx15; break;
            
            }
            return part;
        }

        internal static info_type to_info_type(part_type part) {
            switch (part) {
            case part_type.message: return info_type.msg;

            case part_type.date:    return info_type.date;
            case part_type.time:    return info_type.time;
            case part_type.level:   return info_type.level;
            case part_type.file:    return info_type.file;
            case part_type.func:    return info_type.func;
            case part_type.class_:  return info_type.class_;
            case part_type.thread:  return info_type.thread;

            case part_type.ctx1:    return info_type.ctx1;
            case part_type.ctx2:    return info_type.ctx2;
            case part_type.ctx3:    return info_type.ctx3;

            case part_type.ctx4:    return info_type.ctx4;
            case part_type.ctx5:    return info_type.ctx5;
            case part_type.ctx6:    return info_type.ctx6;
            case part_type.ctx7:    return info_type.ctx7;
            case part_type.ctx8:    return info_type.ctx8;
            case part_type.ctx9:    return info_type.ctx9;
            case part_type.ctx10:    return info_type.ctx10;

            case part_type.ctx11:    return info_type.ctx11;
            case part_type.ctx12:    return info_type.ctx12;
            case part_type.ctx13:    return info_type.ctx13;
            case part_type.ctx14:    return info_type.ctx14;
            case part_type.ctx15:    return info_type.ctx15;

            default:
                Debug.Assert(false);
                return info_type.msg;
            }
            
        }
        
    }


    public class filter_line {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        protected bool Equals(filter_line other) {
            return Equals(fi, other.fi) && case_sensitive_ == other.case_sensitive_ && 
                string.Equals(lo_text, other.lo_text) && string.Equals(text, other.text) && 
                comparison == other.comparison && part == other.part;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((filter_line) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (fi != null ? fi.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ case_sensitive_.GetHashCode();
                hashCode = (hashCode * 397) ^ (lo_words != null ? lo_words.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (lo_text != null ? lo_text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (words != null ? words.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (text != null ? text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) comparison;
                hashCode = (hashCode * 397) ^ (int) part;
                return hashCode;
            }
        }

        private filter_line(string original_line) {
            this.original_line = original_line.Trim();
        }

        public static bool operator ==(filter_line left, filter_line right) {
            return Equals(left, right);
        }

        public static bool operator !=(filter_line left, filter_line right) {
            return !Equals(left, right);
        }

        // ctxX -> context about the message (other than file/func/class)

        public enum comparison_type {
            equal, not_equal, starts_with, does_not_start_with, contains, does_not_contain,
            // 1.0.38+
            contains_any, contains_none,
            // 1.0.66+
            regex
        }
        public part_type part;
        private comparison_type comparison;
        public string text = "";

        // in case we're looking for ANY/NONE
        public string[] words = null;

        // used only for case-insensitive compare
        private string lo_text = "";
        private string[] lo_words = null;

        private bool case_sensitive_ = true;
        private Regex regex_ = null;
        public font_info fi = new font_info();

        public readonly string original_line;

        public bool case_sensitive {
            get { return case_sensitive_; }
            set {
                case_sensitive_ = value;
                if (regex_ != null)
                    create_regex();
            }
        }



        // font -> contains details about the font; for now, the colors

        // 1.2.6g+ - contains an match within a line -one that matches the match color
        public class match_index {
            public int start = 0, len = 0;
            public Color bg = util.transparent, fg = util.transparent;
        }



        public static bool is_color_or_font_line(string line) {
            line = line.Trim();
            return line.StartsWith("color") || line.StartsWith("match_color");
        }

        public static filter_line parse(string line) {
            try {
                return parse_impl(line);
            } catch (Exception) {
                return null;
            }
        }

        // note: in the future, we might allow for more "font" data - at that point, I'll think about the syntax
        //
        // at this point, we allow a simple "color" line:
        // color fg [bg]
        private static filter_line parse_font(string line) {
            // future: if "font" -> account for that as well
            Debug.Assert( is_color_or_font_line( line));
            bool is_color = line.StartsWith("color");

            string[] colors = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            filter_line fi = new filter_line(line) { part = part_type.font };
            if (colors.Length >= 2)
                if ( is_color)
                    fi.fi.fg = util.str_to_color(colors[1]);
                else 
                    fi.fi.match_fg = util.str_to_color(colors[1]);
            if (colors.Length >= 3)
                if (is_color)
                    fi.fi.bg = util.str_to_color(colors[2]);
                else  
                    fi.fi.match_bg = util.str_to_color(colors[2]);
            return fi;
        }

        // guess if this is a regex
        private static bool is_regex_expression(string expr) {
            return expr.IndexOfAny(new char[] { '*', '\\', '.', '^', '$' }) >= 0 ;
        }

        private void create_regex() {
            try {
                if (comparison == comparison_type.regex)
                    regex_ = case_sensitive ? new Regex(text, RegexOptions.Singleline) : new Regex(text, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            } catch (Exception e) {
                logger.Error("invalid regex: " + text + " : " + e.Message);
            }
        }

        // tries to parse a line - if it fails, it will return null
        private static filter_line parse_impl(string line) {
            line = line.Trim();
            if (line.StartsWith("#"))
                // allow comments
                return null;

            if (line.StartsWith("font") || line.StartsWith("color") || line.StartsWith("match_color"))
                return parse_font(line);

            if (line == "case-insensitive")
                return new filter_line(line) {part = part_type.case_sensitive_info, case_sensitive = false };

            if ( !line.StartsWith("$"))
                return null;

            string[] words = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            
            // Syntax:
            // $ColumnName Comparison Text
            bool ok = words.Length >= 3 || (words.Length == 2 && is_regex_expression(words[1]));
            if (!ok)
                // we need at least $c compare word(s)
                return null;

            filter_line fi = new filter_line(line);
            fi.part = part_type_io.from_str(words[0]);
            if (fi.part == part_type.invalid)
                return null;

            if (words.Length == 2) {
                // comparison is not present; inferred as regex
                fi.text = words[1];
                fi.comparison = comparison_type.regex;
                fi.lo_text = fi.text.ToLower();
                fi.create_regex();
                return fi.regex_ != null ? fi : null ;
            }

            switch ( words[1].ToLower() ) {
            case "!=": fi.comparison = comparison_type.not_equal; break;

            case "==": 
            case "=": 
                fi.comparison = comparison_type.equal; break;

            case "+": 
            case "startswith":
                fi.comparison = comparison_type.starts_with; break;

            case "-": 
            case "!startswith":
                fi.comparison = comparison_type.does_not_start_with; break;

            case "++": 
            case "contains":
                fi.comparison = comparison_type.contains; break;

            case "--": 
            case "!contains":
                fi.comparison = comparison_type.does_not_contain; break;

            case "containsany":
            case "any":
                fi.comparison = comparison_type.contains_any; break;

            case "containsnone":
            case "none":
                fi.comparison = comparison_type.contains_none;
                break;

            case "matches":
            case "match":
                fi.comparison = comparison_type.regex;
                break;
            default:
                return null;
            }

            // take the rest of the text
            int compare_idx = line.IndexOf(words[1]);
            if (compare_idx + words[1].Length + 1 >= line.Length)
                // nothing following the comparison
                return null;
            // 1.2.20+ - don't trim the end! - very likely the user wants the beginning/ending spaces to be in the comparison
            line = line.Substring(compare_idx + words[1].Length + 1);
            if (fi.comparison == comparison_type.contains_any || fi.comparison == comparison_type.contains_none) {
                fi.words = line.Split('|');
                fi.lo_words = fi.words.Select(w => w.ToLower()).ToArray();
            }
            fi.text = line;
            fi.lo_text = line.ToLower();
            fi.create_regex();
            if (fi.comparison == comparison_type.regex && fi.regex_ == null)
                // the regex is invalid at this point
                return null;
            return fi;
        }

        private string line_part(line l) {
            return l.part(part_type_io.to_info_type(part));
        }

        private bool compare(string line_part, string text, string[] words) {
            bool result = true;
            switch (comparison) {
            case comparison_type.equal:
                result = line_part == text;
                break;
            case comparison_type.not_equal:
                result = line_part != text;
                break;
            case comparison_type.starts_with:
                result = line_part.StartsWith(text);
                break;
            case comparison_type.does_not_start_with:
                result = !line_part.StartsWith(text);
                break;
            case comparison_type.contains:
                result = line_part.Contains(text);
                break;
            case comparison_type.does_not_contain:
                result = !line_part.Contains(text);
                break;

            case comparison_type.contains_any:
                result = words.Any(line_part.Contains);
                break;
            case comparison_type.contains_none:
                if (words.Any(line_part.Contains)) 
                    result = false;                
                break;

            case comparison_type.regex:
                result = false;
                if (regex_ != null)
                    result = regex_.IsMatch(line_part);
                break;

            default:
                Debug.Assert(false);
                break;
            }

            return result;            
        }

        private List<match_index> compare_with_indexes(string line_part, string text, string[] words) {
            List<match_index> indexes = new List<match_index>();
            switch (comparison) {
            case comparison_type.does_not_start_with:
            case comparison_type.does_not_contain:
            case comparison_type.contains_none:
                Debug.Assert(false);
                return empty_;

            case comparison_type.equal:
                if ( line_part == text)
                    indexes.Add(new match_index { start = 0, len = text.Length });
                break;
            case comparison_type.not_equal:
                if ( line_part != text)
                    indexes.Add(new match_index { start = 0, len = text.Length });
                break;
            case comparison_type.starts_with:
                if ( line_part.StartsWith(text))
                    indexes.Add(new match_index { start = 0, len = text.Length });
                break;
            case comparison_type.contains:
                int pos = line_part.IndexOf(text);
                if ( pos >= 0)
                    indexes.Add(new match_index { start = pos, len = text.Length });
                break;

            case comparison_type.contains_any:
                foreach (string word in words) {
                    int word_pos = line_part.IndexOf(word);
                    if ( word_pos >= 0)
                        indexes.Add(new match_index { start = word_pos, len = word.Length });
                }
                break;

            case comparison_type.regex:
                var matches = regex_.Match(line_part);
                while (matches.Success) {
                    indexes.Add( new match_index { start = matches.Index, len = matches.Length } );
                    matches = matches.NextMatch();
                }                    
                break;
            default:
                Debug.Assert(false);
                break;
            }

            return indexes;
        }

        private bool matches_case_sensitive(line l) {
            Debug.Assert( part != part_type.font );
            string line_part = this.line_part(l);
            return compare(line_part, text, words);
        }

        private bool matches_case_insensitive(line l) {
            Debug.Assert( part != part_type.font );
            string line_part = this.line_part(l).ToLower();
            return compare(line_part, lo_text, lo_words);
        }

        public bool matches(line l) {
            return case_sensitive ? matches_case_sensitive(l) : matches_case_insensitive(l);
        }

        private static readonly List<match_index> empty_ = new List<match_index>();
        public List<match_index> match_indexes(line l, info_type type) {
            if (part == part_type.font || part == part_type.case_sensitive_info)
                return empty_;

            // 1.2.6 for now, care about match indexes only for msg column
            if (type != info_type.msg)
                return empty_;

            // we only care about "positive" matches - those that have "containing", "startswith", regex, etc.
            switch (comparison) {
            case comparison_type.does_not_start_with:
            case comparison_type.does_not_contain:
            case comparison_type.contains_none:
                return empty_;

            case comparison_type.equal:
            case comparison_type.not_equal:
            case comparison_type.starts_with:
            case comparison_type.contains:
            case comparison_type.contains_any:
            case comparison_type.regex:
                break;
            default:
                Debug.Assert(false);
                return empty_;
            }

            return case_sensitive ? matches_case_sensitive_with_indexes(l, type) : matches_case_insensitive_with_indexes(l, type);
        }


        private List<match_index> matches_case_sensitive_with_indexes(line l, info_type type) {
            string line_part = l.part(type);
            return compare_with_indexes(line_part, text, words);
        }

        private List<match_index> matches_case_insensitive_with_indexes(line l, info_type type) {
            string line_part = l.part(type).ToLower();
            return compare_with_indexes(line_part, lo_text, lo_words);
        }

    }

}