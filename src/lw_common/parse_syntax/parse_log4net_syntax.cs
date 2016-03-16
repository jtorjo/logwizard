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
                return "{appdomain}";
            case "appdomain":
                return "";

            case "aspnet-cache":
            case "aspnet-context":
            case "aspnet-request":
            case "aspnet-session":
                return "";

            case "c":
            case "logger":
                    // I consider it the equivalent of class_
                return "class{Logger}";

            case "C":
            case "class":
            case "type":
                return "class";

            case "d":
            case "date":
            case "utcdate":
                return "time";

            case "exception":
                return "";

            case "F":
            case "file":
                return "file";

            case "u":
                return "{identity}";
            case "identity":
                return "";

            case "l":
                return "{location}";
            case "location":
                return "";

            case "L":
                return "{line}";
            case "line":
                return "";

            case "p":
            case "level":
                return "level";

            case "m":
            case "message":
                return "msg";

            case "M":
                return "{method}";
            case "method":
                return "";

            case "n":
            case "newline":
                return "newline";

            case "x":
                return "{ndc}";
            case "ndc":
                return "";

            case "X":
            case "mdc":
            case "P":
            case "properties":
            case "property":
                return "{property}";

            case "r":
            case "timestamp":
                return "time";

            case "stacktrace":
            case "stacktracedetail":
                return "{stack}";

            case "t":
            case "thread":
                return "thread";

            case "username":
            case "w":
                return "{username}";
            }
            return "";
        }

        // returning an empty string means we could not parse it
        //
        // note: for now, I ignore double %%
        public static string parse(string syntax) {
            string lw_syntax = "";
            try {
                var stripped = strip_details(syntax);
                var formats = stripped.Split( new [] {"%"}, StringSplitOptions.RemoveEmptyEntries);
                // we will see if each format can be parsed in a fixed manner - if not, we care about the suffix
                // if ends up being -1, it means from that point on, we can't count on a fixed index
                int fixed_start_index = 0;
                int ctx_index = 0;
                string prev_suffix = "";
                bool was_last_column_fixed = false;
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
                    if (lw_column == "newline")
                        continue;
                    bool recognized = lw_column.Length > 0 && lw_column[0] != '{';
                    if (!recognized)
                        lw_column = "ctx" + (++ctx_index) + lw_column;
                    if (!recognized && !lw_column.StartsWith("{"))
                        lw_column += "{" + format + "}"; // alias
                    // transform into LogWizard syntax
                    lw_syntax += lw_column + "[";
                    lw_syntax += fixed_start_index >= 0 ? "" + fixed_start_index : (was_last_column_fixed ? "'" + prev_suffix + "'" : "''");
                    string end_of_format = fixed_len >= 0 && fixed_now ? "," + fixed_len : (suffix != "" ? ",'" + suffix + "'" : "");
                    // if I don't know the end - the only time I allow this is when the line ends with the message
                    // otherwise, the syntax is invalid
                    if ( end_of_format == "")
                        if (!lw_column.StartsWith("msg")) {
                            logger.Error("Invalid log4net syntax [" + syntax + "] ");
                            return "";
                        }
                    lw_syntax += end_of_format;
                    if (fixed_len >= 0 && !fixed_now)
                        // this last parameter is the minimum number of characters - in other words, look for a suffix, but only after X chars
                        lw_syntax += ";" + fixed_len;
                    lw_syntax += "] ";

                    if (pad == "" || !fixed_now)
                        fixed_start_index = -1;
                    else if (fixed_start_index >= 0)
                        fixed_start_index += fixed_len + suffix.Length;
                    prev_suffix = suffix;
                    was_last_column_fixed = fixed_now;
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
                else if (ch == '{' && inside_formatter)
                    start = i;
                else if (ch == '}' && inside_formatter)
                    end = i;
                else if (!Char.IsLetterOrDigit(ch) && ch != '-' && ch != '.' && start < 0) {
                    inside_formatter = false;
                    end = -1;
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
