using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogWizard.ui;

namespace LogWizard {
    class string_search {
        private static bool is_delim_or_does_not_exist(string line, int idx) {
            if (idx >= 0 && idx < line.Length)
                return !Char.IsLetterOrDigit(line[idx]);
            else
                return true;
        }

        private static bool matches_full_word(string line, string txt) {
            int find = line.IndexOf(txt);
            while (find >= 0) {
                if (is_delim_or_does_not_exist(line, find - 1) && is_delim_or_does_not_exist(line, find + txt.Length))
                    return true;

                find = line.IndexOf(txt, find + 1);
            }

            return false;
        }

        public static bool matches(string line, search_form.search_for search) {
            if (search.use_regex && search.regex == null)
                // the regex is invalid
                return true;

            if (search.use_regex) {
                return search.regex.IsMatch(line);
            } else {
                // case sensitive and/or full word
                string search_for = search.case_sensitive ? search.text : search.text.ToLower();
                string seach_line = search.case_sensitive ? line : line.ToLower();

                if (search.full_word)
                    return matches_full_word(seach_line, search_for);
                else
                    return seach_line.Contains(search_for);
            }

            return false;
        }
    }
}
