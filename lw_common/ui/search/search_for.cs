using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace lw_common.ui {
    public class search_for {
        protected bool Equals(search_for other) {
            bool equals = case_sensitive == other.case_sensitive && 
                          full_word == other.full_word && 
                          String.Equals(text, other.text) && 
                          use_regex == other.use_regex;

            if (use_regex && @equals) {
                // compare regexes
                if (regex == null || other.regex == null)
                    // equal if they are both null
                    return regex == other.regex;
                // both are non-null
                @equals = Equals(regex.ToString(), other.regex.ToString());
            }

            return @equals;
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

        public bool case_sensitive = false;
        public bool full_word = false;
        public string text = "";
            
        public bool use_regex = false;
        public Regex regex = null;

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

        public Color fg = util.transparent, bg = util.transparent;
        public bool mark_lines_with_color = false;
    }
}
