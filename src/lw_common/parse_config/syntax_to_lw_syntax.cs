using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace lw_common
{
    internal class syntax_to_lw_syntax {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string original_syntax_ = "";
        private string lw_syntax_ = "";
        private bool error_ = false;

        private string parse_type_;
        // we will see if each format can be parsed in a fixed manner - if not, we care about the suffix
        // if ends up being -1, it means from that point on, we can't count on a fixed index
        private int fixed_start_index = 0;
        private int ctx_index = 0;
        private string prev_suffix = "";
        private bool was_last_column_fixed = false;

        public syntax_to_lw_syntax(string syntax, string parse_type) {
            original_syntax_ = syntax;
            parse_type_ = parse_type;
        }

        // min_len - minimum length if any ; if -1 - no min length
        // fixed_now - whether this pattern is a fixed number of characters (=min_len)
        // pattern - the original name of the pattern
        // suffix - fixed string coming after pattern
        // lw_column - the logwizard column name. If empty (or "{something}"), we'll assign it the first possible unused column name (ctxX)
        public void add_column(int min_len, bool fixed_now, string pattern, string suffix, string lw_column) {
            bool recognized = lw_column.Length > 0 && lw_column[0] != '{';
            if (!recognized)
                lw_column = "ctx" + (++ctx_index) + lw_column;
            if (!recognized && !lw_column.StartsWith("{"))
                lw_column += "{" + pattern + "}"; // alias
            // transform into LogWizard syntax
            lw_syntax_ += lw_column + "[";
            lw_syntax_ += fixed_start_index >= 0 ? "" + fixed_start_index : (was_last_column_fixed ? "'" + prev_suffix + "'" : "''");
            if (suffix.Trim() == "" && lw_column.StartsWith("msg"))
                // special case for nlog:
                // ${message} ${onexception...}
                suffix = "";
            string end_of_format = min_len > 0 && fixed_now ? "," + min_len : (suffix != "" ? ",'" + suffix + "'" : "");
            // if I don't know the end - the only time I allow this is when the line ends with the message
            // otherwise, the syntax is invalid
            if ( end_of_format == "")
                if (!lw_column.StartsWith("msg")) {
                    on_error("invalid msg ending");
                    return;
                }
            lw_syntax_ += end_of_format;
            if (min_len >= 0 && !fixed_now)
                // this last parameter is the minimum number of characters - in other words, look for a suffix, but only after X chars
                lw_syntax_ += ";" + min_len;
            lw_syntax_ += "] ";

            if (!fixed_now)
                fixed_start_index = -1;
            else if (fixed_start_index >= 0)
                fixed_start_index += min_len + suffix.Length;
            prev_suffix = suffix;
            was_last_column_fixed = fixed_now;            
        }

        public string lw_syntax {
            get { return error_ ? "" : lw_syntax_.Trim(); }
        }

        public void on_error(string error) {
            logger.Error("Invalid " + parse_type_ + " syntax [" + original_syntax_  + "] : " + error);
            error_ = true;
        }

    }
}
