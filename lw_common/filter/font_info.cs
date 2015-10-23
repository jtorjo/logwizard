using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace lw_common {

    public class font_info {
        protected bool Equals(font_info other) {
            return fg.ToArgb() == other.fg.ToArgb() && bg.ToArgb() == other.bg.ToArgb() 
                   && match_fg.ToArgb() == other.match_fg.ToArgb() && match_bg.ToArgb() == other.match_bg.ToArgb();
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((font_info) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = match_fg.GetHashCode();
                hashCode = (hashCode * 397) ^ match_bg.GetHashCode();
                hashCode = (hashCode * 397) ^ fg.GetHashCode();
                hashCode = (hashCode * 397) ^ bg.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(font_info left, font_info right) {
            return Equals(left, right);
        }

        public static bool operator !=(font_info left, font_info right) {
            return !Equals(left, right);
        }

        public void copy_from(font_info other) {
            fg = other.fg;
            bg = other.bg;
            match_fg = other.match_fg;
            match_bg = other.match_bg;
        }

        public override string ToString() {
            return "fg=" + util.color_to_str(fg) + ", bg=" + util.color_to_str(bg) +
                   ", m_fg=" + util.color_to_str(match_fg) + ", m_bg=" + util.color_to_str(match_bg);
        }

        public Color fg = util.transparent, bg = util.transparent;

        // 1.2.6g+
        public Color match_fg = util.transparent, match_bg = util.transparent;

        private static readonly font_info default_font_ = new font_info {  };
        private static readonly font_info full_log_gray_ = new font_info { fg = app.inst.full_log_gray_fg, bg = app.inst.full_log_gray_bg };

        public static font_info default_font {
            get { return default_font_; }
        }

        public static font_info default_font_copy {
            get {
                font_info new_ = new font_info();
                new_.copy_from(default_font_);
                return new_;
            }
        }

        public static font_info full_log_gray {
            get {
                font_info new_ = new font_info();
                new_.copy_from(full_log_gray_);
                return new_;                    
            }
        }
    }
}
