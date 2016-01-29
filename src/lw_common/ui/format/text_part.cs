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
using lw_common.ui.format;

namespace lw_common.ui {
    internal class text_part {
        protected bool Equals(text_part other) {
            return start_ == other.start_ && len_ == other.len_ && 
                fg.Equals(other.fg) && bg.Equals(other.bg) && bold == other.bold && italic == other.italic && String.Equals(font_name, other.font_name);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((text_part) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = fg.GetHashCode();

                hashCode = (hashCode * 397) ^ start_.GetHashCode();
                hashCode = (hashCode * 397) ^ len_.GetHashCode();

                hashCode = (hashCode * 397) ^ bg.GetHashCode();
                hashCode = (hashCode * 397) ^ bold.GetHashCode();
                hashCode = (hashCode * 397) ^ italic.GetHashCode();
                hashCode = (hashCode * 397) ^ (font_name != null ? font_name.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(text_part left, text_part right) {
            return Equals(left, right);
        }

        public static bool operator !=(text_part left, text_part right) {
            return !Equals(left, right);
        }

        public void copy_from(text_part other) {
            start_ = other.start_;
            len_ = other.len_;

            fg = other.fg;
            bg = other.bg;
            underline = other.underline;
            bold = other.bold;
            italic = other.italic;
            font_name = other.font_name;
            text = other.text;
            is_typed_search = other.is_typed_search;
            is_find_search = other.is_find_search;
            font_size = other.font_size;
            modify_bg = other.modify_bg;
            modify_fg = other.modify_fg;
        }

        // constructs a new object as a merge of this and other
        // other overrides everything from this
        // 
        // note: start/len/text are taken from other (overridden)
        public text_part merge_copy(text_part other) {
            var copy = new text_part(other.start, other.len, this) { text = other.text };

            if (other.fg != util.transparent)
                copy.fg = other.fg;
            if (other.bg != util.transparent)
                copy.bg = other.bg;
            if (other.underline)
                copy.underline = true;
            if (other.bold)
                copy.bold = true;
            if (other.italic)
                copy.italic = true;

            if (other.font_name != "")
                copy.font_name = other.font_name;
            if (other.font_size > 0)
                copy.font_size = other.font_size;

            if (other.is_find_search)
                copy.is_find_search = true;
            if (other.is_typed_search)
                copy.is_typed_search = true;
            
            if (other.modify_fg != modify_color_type.same)
                copy.modify_fg = other.modify_fg;
            if (other.modify_bg != modify_color_type.same)
                copy.modify_bg = other.modify_bg;

            return copy;
        }
        

        public Color fg = util.transparent;
        public Color bg = util.transparent;
        public bool bold = false, italic = false, underline = false;

        public string font_name = "";

        // 1.7.9+ if > 0, the font' size
        public int font_size = 0;

        public enum modify_color_type {
            
            same,  // do nothing
            darker, lighter
        }

        public modify_color_type modify_fg = modify_color_type.same;
        public modify_color_type modify_bg = modify_color_type.same;

        public void update_colors(column_formatter.format_cell cell) {
            if (modify_fg != modify_color_type.same)
                fg = modify_fg == modify_color_type.darker ? util.darker_color(cell.fg_color) : util.grayer_color(cell.fg_color);
            if (modify_bg != modify_color_type.same)
                bg = modify_bg == modify_color_type.darker ? util.darker_color(cell.bg_color) : util.grayer_color(cell.bg_color);
        }


        /*  string is internally separated by '/'

            every entry can be any of: 
            - font name, font size, foreground color, background color, bold, italic, underline
            The above is the precendence order as well

            If you want to force having a background color without a foreground color, just prepend an extra #, like
            ##aabbcc
        */
        public static text_part from_friendly_string(string str) {
            text_part friendly = new text_part(0,0);

            foreach (string word in str.Split('/').Select(x => x.ToLower())) {
                if (friendly.font_name == "") {
                    var possible_font = util.font_families().FirstOrDefault(x => x.ToLower() == word);
                    if (possible_font != null) {
                        friendly.font_name = possible_font;
                        continue;
                    }
                }

                int possible_size = 0;
                if ( int.TryParse(word, out possible_size))
                    // don't allow too huge sizes
                    if (possible_size > 5 && possible_size < 24) {
                        friendly.font_size = possible_size;
                        continue;
                    }

                bool force_bg = word.StartsWith("##");
                Color col = util.str_to_color(force_bg ? word.Substring(1) : word);
                if (col != util.transparent) {
                    bool is_bg = force_bg || friendly.fg != util.transparent;
                    if (is_bg)
                        friendly.bg = col;
                    else
                        friendly.fg = col;
                    continue;
                }

                switch (word) {
                case "bold":
                    friendly.bold = true;
                    break;
                case "italic":
                    friendly.italic = true;
                    break;
                case "underline":
                    friendly.underline = true;
                    break;
                case "darker":
                    friendly.modify_fg = modify_color_type.darker;
                    break;
                case "lighter":
                    friendly.modify_fg = modify_color_type.lighter;
                    break;
                case "darker-bg":
                    friendly.modify_bg = modify_color_type.darker;
                    break;
                case "lighter-bg":
                    friendly.modify_bg = modify_color_type.lighter;
                    break;
                }
            }

            return friendly;
        }

        //////////////////////////////////////////////////////////////////////////////////
        // From here on -> attributes that don't relate to printing this on screen

        // 1.7.5+ - keep the offsets from the original text as well
        private int start_;
        private int len_;

        // 1.5.10+ - if true, it's the result of what the user typed
        public bool is_typed_search = false;

        // 1.7.5+ - if true, it's the result of a find (ctrl-f)
        public bool is_find_search = false;
            
        // useful when sorting - to avoid collisions
        public string text = "";

        public text_part(int start, int len, text_part src = null) {
            if ( src != null)
                copy_from(src);

            start_ = start;
            len_ = len;
        }

        public int start {
            get { return start_; }
        }
        public int end {
            get { return start_ + len_; }
        }

        public int len {
            get { return len_; }
        }
    }
}
