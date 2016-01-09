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
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lw_common.ui;

namespace lw_common {
    internal class string_search {
        public static bool is_delim_or_does_not_exist(string line, int idx) {
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

        public static List< Tuple<int,int>> match_indexes(string line, search_for search) {
            if (search.use_regex && search.regex == null)
                // the regex is invalid
                return new List<Tuple<int, int>>();

            if (search.use_regex) {
                var matches = search.regex.Match(line);
                if ( !matches.Success)
                    return new List<Tuple<int, int>>();

                List<Tuple<int, int>> result = new List<Tuple<int, int>>();
                while (matches.Success) {
                    result.Add( new Tuple<int, int>(matches.Index, matches.Length));
                    matches = matches.NextMatch();
                }

                return result;
            } else {
                // case sensitive and/or full word
                string search_for = search.case_sensitive ? search.text : search.text.ToLower();
                string search_line = search.case_sensitive ? line : line.ToLower();

                if (search.full_word) 
                    return util.find_all_matches(search_line, search_for).Where( 
                        x => is_delim_or_does_not_exist(search_line, x - 1) && is_delim_or_does_not_exist(search_line, x + search_for.Length) 
                            ).Select(x => new Tuple<int,int>(x, search_for.Length)). ToList();

                else
                    return util.find_all_matches(search_line, search_for).Select(x => new Tuple<int,int>(x, search_for.Length)). ToList();
            }

        }

        public static bool matches(filter.match item, search_for search) {
            if (search.all_columns) 
                return info_type_io.searchable.Any(x => matches_cell(item.line.part(x), search));
            else
                return matches_cell(item.line.part(info_type.msg), search);
        }

        public static bool matches(IEnumerable<string> cells, search_for search) {
            return cells.Any(cell => matches_cell(cell, search));
        }


        private static bool matches_cell(string line, search_for search) {
            if (search.use_regex && search.regex == null)
                // the regex is invalid
                return true;

            if (line == "")
                // optimization
                return false;

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
        }
    }
}
