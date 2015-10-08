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
