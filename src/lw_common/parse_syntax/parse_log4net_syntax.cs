using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.parse_syntax
{
    public static class parse_log4net_syntax
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static bool is_nlog_syntax(string syntax) {
            if (syntax.Contains("%newline"))
                return true;
            int idx1 = syntax.IndexOf("%");
            int idx2 = idx1 >= 0 ? syntax.IndexOf("%", idx1 + 1) : -1;
            return idx1 >= 0 && idx2 >= 0;
        }

        // empty string - we could not convert it
        // if it starts with '{', that will be the friendly name to give to the column, but we still can't match it to an existing LW column
        private static string column_to_lw_column(string column) {
            switch (column) {
            case "a":
            case "appdomain":
            case "aspnet-cache":
            case "aspnet-context":
            case "aspnet-request":
            case "aspnet-session":

            case "c":
            case "logger":

            case "C":
            case "class":
            case "type":

            case "d":
            case "date":
            case "utcdate":

            case "exception":

            case "F":
            case "file":

            case "u":
            case "identity":

            case "l":
            case "location":

            case "L":
            case "line":

            case "p":
            case "level":

            case "m":
            case "message":

            case "M":
            case "method":

            case "n":
            case "newline":

            case "x":
            case "ndc":

            case "X":
            case "mdc":
            case "P":
            case "properties":
            case "property":

            case "r":
            case "timestamp":

            case "stacktrace":
            case "stacktracedetail":

            case "t":
            case "thread":

            case "username":
            case "w":

                break;
            }
            return "";
        }

        // returning an empty string means we could not parse it
        public static string parse(string syntax) {
            string lw_syntax = "";
            try {
                var stripped = strip_details(syntax);
                var formats = stripped.Split('%');
                // we will see if each format can be parsed in a fixed manner - if not, we care about the suffix
                // if ends up being -1, it means from that point on, we can't count on a fixed index
                int fixed_start_index = 0;
                string prev_suffix = "";
                int ctx_index = 0;
                foreach (string format_and_suffix in formats) {
                    var format = new string( format_and_suffix.TakeWhile(x => Char.IsLetterOrDigit(x) || x == '-' || x == '.').ToArray());
                    var suffix = format_and_suffix.Substring(format.Length);

                    // https://logging.apache.org/log4net/log4net-1.2.13/release/sdk/log4net.Layout.PatternLayout.html
                    // '-' or '.' -> they are padding
                    var pad = new string( format.TakeWhile(x => Char.IsDigit(x) || x == '-' || x == '.').ToArray() );
                    format = format.Substring(pad.Length);

                    if (pad.StartsWith("-"))
                        pad = pad.Substring(1);
                    // it can contain padding - but only "." specifies exact number of characters
                    bool fixed_now = pad.Contains(".");
                    if (fixed_now)
                        pad = pad.Substring(pad.IndexOf(".") + 1);

                    int fixed_len = pad != "" ? int.Parse(pad) : -1;

                    var lw_column = column_to_lw_column(format);
                    bool recognized = lw_column.Length > 0 && lw_column[0] != '{';
                    if (!recognized)
                        lw_column = "" + (++ctx_index) + lw_column;
                    // transform into LogWizard syntax
                    lw_syntax += lw_column + "[";
                    lw_syntax += fixed_start_index >= 0 ? "" + fixed_start_index : "'" + prev_suffix + "'";
                    lw_syntax += ",";
                    lw_syntax += fixed_len >= 0 && fixed_now ? "" + fixed_len : suffix;
                    if (fixed_len >= 0 && !fixed_now)
                        // this last parameter is the minimum number of characters - in other words, look for a suffix, but only after X chars
                        lw_syntax += "," + fixed_len;
                    lw_syntax += "] ";

                    if (pad == "" || !fixed_now)
                        fixed_start_index = -1;
                    else if (fixed_start_index >= 0)
                        fixed_start_index += fixed_len + suffix.Length;
                    prev_suffix = suffix;
                }
            } catch (Exception e) {
                logger.Error("Invalid log4net syntax [" + syntax + "] : " + e.Message);
                lw_syntax = "";
            }
            return lw_syntax;
        }

        // %date{HH:mm:ss,fff} -> %date
        // note: I assume {} can also appear as separators in the line itself, so I don't want to assume that anything between {} is to be automatically stripped out
        private static string strip_details(string syntax) {
            int start = -1, end = -1;
            bool inside_formatter = false;
            for (int i = 0; i < syntax.Length && (start < 0 || end < 0); i++) {
                char ch = syntax[i];
                if (ch == '%')
                    inside_formatter = true;

                if (ch == '{' && inside_formatter)
                    start = i;
                else if (ch == '}' && inside_formatter)
                    end = i;
                else if (!Char.IsLetterOrDigit(ch) && ch != '-' && ch != '.') {
                    inside_formatter = false;
                    start = end = -1;
                }
            }

            if (start >= 0 && end >= 0) {
                syntax = syntax.Substring(0, start) + syntax.Substring(end + 1);
                return strip_details(syntax);
            }
            else
                return syntax;
        }
    }
}
