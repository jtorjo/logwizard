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
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace lw_common.ui {
    internal class print_info {
        protected bool Equals(print_info other) {
            return fg.Equals(other.fg) && bg.Equals(other.bg) && bold == other.bold && italic == other.italic && String.Equals(font_name, other.font_name);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((print_info) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = fg.GetHashCode();
                hashCode = (hashCode * 397) ^ bg.GetHashCode();
                hashCode = (hashCode * 397) ^ bold.GetHashCode();
                hashCode = (hashCode * 397) ^ italic.GetHashCode();
                hashCode = (hashCode * 397) ^ (font_name != null ? font_name.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(print_info left, print_info right) {
            return Equals(left, right);
        }

        public static bool operator !=(print_info left, print_info right) {
            return !Equals(left, right);
        }

        public void copy_from(print_info other) {
            fg = other.fg;
            bg = other.bg;
            underline = other.underline;
            bold = other.bold;
            italic = other.italic;
            font_name = other.font_name;
            text = other.text;
            is_typed_search = other.is_typed_search;
        }

        
        // this updates text + infos, so that it will return a single line from a possible multi-line text
        // we want this, in case the text is multi-line, but we can only print A SINGLE LINE 
        public static void get_most_important_single_line(ref string text, ref List<Tuple<int, int, print_info>> infos) {
            if (!text.Contains('\r') && !text.Contains('\n'))
                // text is single line
                return;

            char more = '¶';
            var lines = util.split_into_lines(text, util.split_into_lines_type.include_enter_chars_in_returned_lines).ToList();
            if (infos.Count == 0) {
                // in this case, we don't have any custom printing - just return the first non empty line
                text = lines.FirstOrDefault(x => x.Length > 0 && !util.any_enter_char.Contains(x[0]));
                if (text == null)
                    // we only have empty lines
                    text = "";
                else {
                    int line_idx = lines.IndexOf(text);
                    text = (line_idx > 0 && app.inst.show_paragraph_sign ? more + " " : "") + text.Trim() + (line_idx < lines.Count - 1 && app.inst.show_paragraph_sign ? " " + more : "");
                }
                return;
            }
            // we have custom printing - first, see if we have typed search
            var relevant_print = infos.FirstOrDefault(x => x.Item3.is_typed_search);
            if (relevant_print == null)
                relevant_print = infos[0];

            // find the relevant line
            int start = 0;
            string relevant_line = null;
            foreach ( var line in lines)
                if (relevant_print.Item1 < start + line.Length) {
                    relevant_line = line;
                    break;
                } else
                    start += line.Length;
            Debug.Assert(relevant_line != null);
            bool line_before = start > 0;
            bool line_after = start + relevant_line.Length < text.Length;
            // at this point, ignore enters
            relevant_line = relevant_line.Trim();
            int len = relevant_line.Length;
            // ... just take the print infos for the relevant line
            infos = infos.Where(x => (start <= x.Item1 && x.Item1 < start + len) || (start <= x.Item1 + x.Item2 && x.Item1 + x.Item2 < start + len) ).ToList();
            if (infos.Count > 0) {
                // adjust first and last - they might be outside our line
                if (infos[0].Item1 < start) 
                    infos[0] = new Tuple<int, int, print_info>(start, infos[0].Item2 - (start - infos[0].Item1), infos[0].Item3);
                var last = infos[infos.Count - 1];
                if ( last.Item1 + last.Item2 > start + len) 
                    infos[infos.Count - 1] = new Tuple<int, int, print_info>(last.Item1, start + len - last.Item1, last.Item3);
            }

            if (!app.inst.show_paragraph_sign)
                line_before = line_after = false;

            // convert all the indexes into the relevant line
            for ( int i = 0; i < infos.Count; ++i)
                infos[i] = new Tuple<int, int, print_info>(infos[i].Item1 - start + (line_before ? 2 : 0), infos[i].Item2, infos[i].Item3);

            text = (line_before ? more + " " : "") + relevant_line + (line_after ? " " + more : "");
        }

        // converts '\r\n' to '\r' = this is a must for rich text box - because otherwise we'd end up having the wrong chars printed with different infos
        // (since rich text box considers "\r\n" as a single char, thus we would end up printing colored text off-by-one for each new line)
        public static void to_single_enter_char(ref string text, ref List<Tuple<int, int, print_info>> infos) {
            while (true) {
                int next_enter = text.IndexOf('\n');
                if (next_enter < 0)
                    break;

                int start = infos.FindIndex(x => x.Item1 > next_enter);
                if ( start >= 0)
                    for ( int i = start; i < infos.Count; ++i)
                        infos[i] = new Tuple<int, int, print_info>( infos[i].Item1 - 1, infos[i].Item2, infos[i].Item3 );
                text = text.Substring(0, next_enter) + text.Substring(next_enter + 1);
            }
        }

        public Color fg = util.transparent;
        public Color bg = util.transparent;
        public bool bold = false, italic = false, underline = false;

        // 1.5.10+ - if true, it's the result of what the user typed
        public bool is_typed_search = false;

        public string font_name = "";
            
        // useful when sorting - to avoid collisions
        public string text = "";
    }
}
