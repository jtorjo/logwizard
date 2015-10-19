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
using System.Drawing;
using System.Linq;
using System.Text;

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

        public Color fg = util.transparent;
        public Color bg = util.transparent;
        public bool bold = false, italic = false;
        public string font_name = "";
            
        // useful when sorting - to avoid collisions
        public string text = "";
    }
}
