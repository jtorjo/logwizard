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
using System.Linq;
using System.Text;

namespace lw_common {
    public class split {
        public class split_exception : Exception {
            public split_exception(string message) : base(message) {
            }
        }

        [Flags]
        public enum type : int {
            // a,,b => {a,b}
            ignore_empty_entries               = 1,
            // a,'b',"c" -> {a,b,c}
            use_any_quotes                     = 6,
            // a,'b',"c" -> {a,b,"c"}
            use_simple_quote                   = 2,
            // a,'b',"c" -> {a,'b',c}
            use_double_quote                   = 4,
            // a, b ,c -> {a,b,c}
            ignore_trailing_spaces             = 16,
            // true:  a,b\,c,d -> {a b,c ,d}
            // false: a,b\,c,d -> {a, b\, c, d}
            escape_via_backslash   = 32,

            // if true, we throw on an invalid string (example: when using quotes, but the string is not terminated correctly
            on_error_throw                     = 64,
        }

        public enum dictionary_type {
            on_error_keep_first, on_error_keep_last, on_error_throw
        }

        private static string escape(string str, string delimiter, type split_type) {
            if ((split_type & type.ignore_trailing_spaces) == type.ignore_trailing_spaces)
                str = str.Trim();

            bool use_simple_quote = (split_type & type.use_simple_quote) == type.use_simple_quote;
            bool use_double_quote = (split_type & type.use_double_quote) == type.use_double_quote;
            bool use_escape = (split_type & type.escape_via_backslash) == type.escape_via_backslash;
            bool has_delimeter = str.Contains(delimiter);

            bool needs_escape = use_simple_quote && str.Contains('\'');
            if ( !needs_escape)
                if (use_double_quote)
                    needs_escape = str.Contains('"');
            if (!needs_escape)
                if ( use_escape)
                    needs_escape = str.Contains('\\');
            if (!needs_escape)
                needs_escape = has_delimeter;

            if ( !needs_escape)
                if (use_simple_quote || use_double_quote)
                    needs_escape = str.Contains(' ');

            if (!needs_escape)
                return str;

            string q = new string(use_simple_quote ? '\'' : '"', 1);
            if (use_escape) {
                // escape everything with \
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < str.Length;) {
                    if (is_delimeter_at(str, delimiter, i)) {
                        sb.Append('\\');
                        sb.Append(delimiter);
                        i += delimiter.Length;
                        continue;
                    }
                    bool needs_escape_now = use_simple_quote && str[i] == '\'';
                    if (!needs_escape_now)
                        needs_escape_now = use_double_quote && str[i] == '"';
                    if (!needs_escape_now)
                        needs_escape_now = str[i] == '\\';

                    if (needs_escape_now)
                        sb.Append('\\');
                    sb.Append(str[i]);
                    ++i;
                }
                str = sb.ToString();
            } else 
                // double all quotes
                str = str.Replace(q, q + q);

            if (use_simple_quote || use_double_quote)
                str = q + str + q;
            return str;
        }

        private static string un_escape(string str, type split_type) {
            if ((split_type & type.ignore_trailing_spaces) == type.ignore_trailing_spaces)
                str = str.Trim();

            bool use_simple_quote = (split_type & type.use_simple_quote) == type.use_simple_quote;
            bool use_double_quote = (split_type & type.use_double_quote) == type.use_double_quote;
            bool escape = (split_type & type.escape_via_backslash) == type.escape_via_backslash;
            bool use_quotes = use_simple_quote || use_double_quote;
            bool do_throw = (split_type & type.on_error_throw) == type.on_error_throw;

            char quoted = '\0';
            if ( use_simple_quote)
                if (str.Length > 2 && str[0] == '\'') {
                    str = str.Substring(1, str.Length - 2);
                    quoted = '\'';
                }
            if ( use_double_quote)
                if (str.Length > 2 && str[0] == '"') {
                    str = str.Substring(1, str.Length - 2);
                    quoted = '"';
                }

            // if using escapes, we unescape them here
            if ( escape && do_throw)
                if ( str.Length > 0 && str.Last() == '\\')
                    throw new split_exception("invalid substring " + str);

            if (escape)
                str = str.ToCharArray().Where(c => c != '\\').ToString();

            if (use_quotes && !escape) {
                if ( quoted == '\0' && do_throw)
                    if ( str.Contains('\'') || str.Contains('"'))
                        throw new split_exception("invalid substring " + str);

                if (quoted != '\0') {
                    string q = new string(quoted, 1), qq = new string(quoted, 2);
                    str = str.Replace(qq, q);
                }
            }

            return str;
        }

        private static bool is_delimeter_at(string str, string delimiter, int i) {
            bool is_delimiter = i >= 0 && i + delimiter.Length <= str.Length && str.Substring(i, delimiter.Length) == delimiter;
            return is_delimiter;
        }

        public static List<string> to_list(string str, string delimiter, type split_type = type.use_any_quotes | type.ignore_trailing_spaces) {
            List<string> list = new List<string>();

            bool inside = false;
            StringBuilder last = new StringBuilder();

            bool use_simple_quote = (split_type & type.use_simple_quote) == type.use_simple_quote;
            bool use_double_quote = (split_type & type.use_double_quote) == type.use_double_quote;
            bool escape = (split_type & type.escape_via_backslash) == type.escape_via_backslash;
            bool use_quotes = use_simple_quote || use_double_quote;
            bool do_throw = (split_type & type.on_error_throw) == type.on_error_throw;

            // if using quotes, the delimiter can't contain them
            Debug.Assert(!(use_simple_quote && delimiter.Contains('\'')));
            Debug.Assert(!(use_double_quote && delimiter.Contains('"')));

            char last_quote = '\0';
            for (int i = 0; i < str.Length; ) {
                if (escape && str[i] == '\\') {
                    // see what we're escaping - if anything
                    if (is_delimeter_at(str, delimiter, i + 1)) {
                        last.Append('\\');
                        last.Append(delimiter);
                        i += delimiter.Length + 1;
                        continue;
                    }
                    if (i + 1 < str.Length) {
                        // just use what follows the escape
                        last.Append( str[i]);
                        last.Append( str[i+1]);
                        i += 2;
                        continue;
                    }

                    // here, we have an escape exactly at end of string
                    if ( do_throw)
                        throw new split_exception("string not escaped correctly: " + str);

                    ++i;
                    continue;
                }

                bool is_delimiter = is_delimeter_at(str, delimiter, i);
                if (is_delimiter && !escape) {
                    bool ignore_delimiter = use_quotes && inside;
                    // at this point, we know if whether to ignore delimeter or not
                    if (ignore_delimiter)
                        last.Append(delimiter);
                    else {
                        // we got a new entry
                        list.Add( un_escape( last.ToString(), split_type));
                        last.Clear();
                        inside = false;
                    }
                    i += delimiter.Length;
                    continue;
                }

                // at this point, I know it's not a delimeter, see if it's a quote
                //
                // note: if escape is true, we're only escaping via \
                bool is_quote = (use_simple_quote && str[i] == '\'') || (use_double_quote && str[i] == '"');
                bool is_next_quote = !escape && is_quote && (i+1 < str.Length) && ((use_simple_quote && str[i+1] == '\'') || (use_double_quote && str[i+1] == '"'));

                if (inside)
                    // if using simple & double quotes - if string starts with ' no need to escape " and vice-versa
                    is_quote = is_quote && str[i] == last_quote;

                if (is_quote) {
                    if (inside && is_next_quote) {
                        // it's a quote that has been doubled -> thus, escaped
                        last.Append(str.Substring(i, 2));
                        i += 2;
                        continue;
                    }

                    if (!inside && is_next_quote) {
                        // this can be an empty entry; however,
                        // here, we're not sure if it's an empty entry or not - for instance, it could be an empty entry followed by spaces
                        // lets just continue for now
                        last.Append(str.Substring(i, 2));
                        i += 2;
                        continue;
                    }

                    Debug.Assert(!is_next_quote);
                    last.Append(str[i]);
                    inside = !inside;
                    last_quote = str[i];
                    ++i;
                    continue;
                }

                // here, it's not a quote, nor a delimeter, nor an escape
                last.Append(str[i]);
                ++i;
            }

            if (last.Length > 0) {
                if ( use_quotes && inside && do_throw)
                    // last entry - not finished
                    throw new split_exception("invalid ending: " + str);

                list.Add(un_escape(last.ToString(), split_type));
            } else {
                // see if we have an empty last entry
                bool last_char_is_delimeter = is_delimeter_at(str, delimiter, str.Length - delimiter.Length);
                if (last_char_is_delimeter) 
                    // last entry was empty
                    list.Add("");
            }

            if ((split_type & type.ignore_empty_entries) == type.ignore_empty_entries)
                list = list.Where(x => x.Length > 0).ToList();

            return list;
        }

        public static string from_list(IList< string> list, string delimeter, type split_type = type.use_any_quotes | type.ignore_trailing_spaces) {
            if ((split_type & type.ignore_empty_entries) == type.ignore_empty_entries)
                list = list.Where(x => x.Length > 0).ToList();

            string str = "";
            for (int i = 0; i < list.Count; ++i) {
                if (i > 0)
                    str += delimeter;
                str += escape(list[i], delimeter, split_type);
            }
            return str;
        }

        public static Dictionary<string,string> to_dictionary(string str, string delimiter_list, string delimiter_dictionary, type split_type = type.use_any_quotes | type.ignore_trailing_spaces, dictionary_type dict_type = dictionary_type.on_error_keep_last) {
            var list = to_list(str, delimiter_list, split_type);
            return new Dictionary<string, string>();
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // tests

        private static void test_list_str(string str, string delimiter, type split_type, string[] result) {
            Debug.Assert( to_list(str, delimiter, split_type).SequenceEqual(result) );
            Debug.Assert( from_list(result.ToList(),delimiter,split_type) == str );
        }

        public static void test_list() {
            test_list_str( "a,b,'c '", ",", type.use_simple_quote, new []{ "a", "b", "c "} );
        }
        public static void test_dictionary() {
            
        }
    }
}
