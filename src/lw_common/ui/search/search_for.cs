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
 *  * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;

namespace lw_common.ui {
    public class search_for {
        internal const int MAX_LAST_VIEW_NAMES = 15;

        protected bool Equals(search_for other) {
            bool equals = case_sensitive == other.case_sensitive && 
                          full_word == other.full_word && 
                          String.Equals(text, other.text) && 
                          type == other.type && all_columns == other.all_columns;
            return equals;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((search_for) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = case_sensitive.GetHashCode();
                hashCode = (hashCode * 397) ^ full_word.GetHashCode();
                hashCode = (hashCode * 397) ^ (text != null ? text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ use_regex.GetHashCode();
                hashCode = (hashCode * 397) ^ (regex != null ? regex.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(search_for left, search_for right) {
            return Equals(left, right);
        }
        public static bool operator !=(search_for left, search_for right) {
            return !Equals(left, right);
        }
        public bool use_regex {
            get {
                if (type == 0)
                    return is_auto_regex(text);
                if (type == 2)
                    return true;
                return false;
            }
        }
        public Regex regex {
            get {
                if (regexs_initalized_)
                    return regex_;

                regexs_initalized_ = true;
                bool use_regex = type == 2;
                if (type == 0)
                    use_regex = is_auto_regex(text);
                if ( use_regex)
                    try {
                        // 1.7.2+ - for multi-line logs, we should not rely on single-line. Perhaps in the future, I may revisit this, 
                        regex_ = case_sensitive ? new Regex(text) : new Regex(text, RegexOptions.IgnoreCase);
                    } catch {
                        regex_  = null;
                    }
                return regex_ ;
            }
        }

        public string friendly_name {
            get {
                List<string> attr = new List<string>();
                if ( !case_sensitive)
                    attr.Add("case-insensitive");
                if ( full_word)
                    attr.Add("whole word");
                var extra = util.concatenate(attr, ",");
                if (!use_regex)
                    return text + (extra != "" ? " (" + extra + ")" : "");
                return "Regex " + regex.ToString() + (extra != "" ? " (" + extra + ")" : "");
            }
        }

        // FIXME use load_save
        internal void save(string prefix) {
            var sett = app.inst.sett;
            sett.set(prefix + ".fg", util.color_to_str(fg));
            sett.set(prefix + ".bg", util.color_to_str(bg));
            sett.set(prefix + ".case_sensitive", case_sensitive ? "1" : "0");
            sett.set(prefix + ".full_word", full_word ? "1" : "0");
            sett.set(prefix + ".mark_lines_with_color", mark_lines_with_color ? "1" : "0");
            sett.set(prefix + ".text", text);
            sett.set(prefix + ".type", "" + type);
            sett.set(prefix + ".friendly_regex_name", friendly_regex_name);
            sett.set(prefix + ".last_view_names", util.concatenate(last_view_names,"|"));
            sett.set(prefix + ".all_columns", all_columns ? "1" : "0");
            sett.set(prefix + ".negate", negate ? "1" : "0");
            // FIXME i need more testing on split class
            //sett.set(prefix + ".last_view_names", split.from_list(last_view_names, ",", split.type.use_any_quotes));
        }

        // FIXME use load_save
        internal static search_for load(string prefix) {
            var sett = app.inst.sett;
            int type = int.Parse(sett.get(prefix + ".type", "0"));
            Debug.Assert( type >= 0 && type <= 2);
            search_for cur = new search_for {
                fg = util.str_to_color( sett.get(prefix + ".fg", "transparent")),
                bg = util.str_to_color( sett.get(prefix + ".bg", "#faebd7") ),
                case_sensitive = sett.get(prefix + ".case_sensitive", "0") != "0",
                full_word = sett.get(prefix + ".full_word", "0") != "0",
                mark_lines_with_color = sett.get(prefix + ".mark_lines_with_color", "1") != "0",
                text = sett.get(prefix + ".text"), 
                type = type, 
                friendly_regex_name = sett.get(prefix + ".friendly_regex_name"),
                // FIXME i need more testing on split class
                //last_view_names = split.to_list( sett.get(prefix + ".last_view_names"), ",", split.type.use_any_quotes ).ToArray()
                last_view_names = sett.get(prefix + ".last_view_names").Split('|'),
                // 1.6.23+ - by default, don't search all columns - did profiling, and can take a looot of time (example: Event Log)
                all_columns = sett.get(prefix + ".all_columns", "0") != "0",
                negate = sett.get(prefix + ".negate", "0") != "0"
            };
            return cur;
        }

        internal static bool is_auto_regex(string text) {
            // 1.7.2+ pretty sure that's a regex :)
            if (text.Contains(".*"))
                return true;
            bool is_regex = text.IndexOfAny(new char[] {'[', ']', '(', ')', '\\'}) >= 0;
            return is_regex;
        }

        public bool is_column_searchable(info_type col) {
            if (all_columns)
                return info_type_io.is_searchable(col);
            else
                return col == info_type.msg;
        }

        // in case I'm converting to a filter
        public class filter_info {
            // the filter itself
            public readonly string text ;
            public readonly string id ;
            public readonly bool apply_to_existing_lines ;

            public filter_info(string text, string id, bool apply_to_existing_lines) {
                this.text = text;
                this.id = id;
                this.apply_to_existing_lines = apply_to_existing_lines;
            }
        }

        public filter_info to_filter() {
            bool is_negated = !use_regex && negate;
            bool is_color_filter = fg != util.transparent || bg != util.transparent;
            bool is_exclude_filter = is_negated;

            string filter_str = "";

            string do_not_edit = "DONOT CHANGE this line -";
            string id = raw_filter_row.FILTER_ID_PREFIX + do_not_edit 
                + (is_color_filter ? "color " : "") 
                + (is_exclude_filter ? "exclude " : "")
                + " #" + text;
            filter_str += id + "\r\n";
            filter_str += "## " + (all_columns ? "Any Column" : "Message") + " " + (!is_negated ? "Contains" : "Does not contain") + " " + (use_regex ? "regex" : "text") + " " + text + "\r\n";
            if (!case_sensitive)
                filter_str += "case-insensitive\r\n";
            if (full_word)
                filter_str += "full-word\r\n";

            filter_str += all_columns ? "$any" : "$msg";
            filter_str += " " + (use_regex ? text : (negate ? "!" : "") + "contains '" + text + "'") + "\r\n";


            if (is_color_filter)
                // note: when the filter is negated, we're simply excluding lines - thus, there's no color to assign
                if ( !is_exclude_filter)
                    filter_str += "match_color " + util.color_to_str(fg) + " " + (bg != util.transparent ? util.color_to_str(bg) : "") + "\r\n";

            bool apply_to_existing_lines = is_color_filter || is_exclude_filter;
            return new filter_info(filter_str, id, apply_to_existing_lines);
        }


        public Color fg = util.transparent, bg = util.transparent;

        // uniquely identify a search in history (NOT saved!)
        public int unique_id = 0;

        public bool case_sensitive = false;
        public bool full_word = false;
        public bool mark_lines_with_color = false;
        public string text = "";

        // 0 - auto recognize, 1 - text, 2 - regex
        public int type = 0;

        private Regex regex_ = null;
        private bool regexs_initalized_ = false;

        public string friendly_regex_name = "";
        public string[] last_view_names ;

        public bool all_columns = true;

        // only for text 
        public bool negate = false;
    }
}
