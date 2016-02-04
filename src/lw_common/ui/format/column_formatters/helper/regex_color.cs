using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    /* 
	- regex to color/font/etc.
  	    - things in brackets - perhaps show them in slightly lighter color
	    - directory names in slighly different color
	    - strings (quotes and double quotes -> show them in different color, default = brown) 

    Brackets
    (?<=\[).*(?=\])|(?<=\().*(?=\))|(?<=\{).*(?=\})

    Directory/file names
    http://stackoverflow.com/questions/6416065/c-sharp-regex-for-file-paths-e-g-c-test-test-exe
    Pretty much everything is not that 100% perfect, just need to use one and stick with it
    (?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+

    Strings
    (?<=")(?:\\.|[^"\\])*(?=")|(?<=')(?:\\.|[^'\\])*(?=')

    Syntax:
        expr=regex_expression
        format=color

    FIXME Later, parse the format using text_part.from_friendly_string
    */
    class regex_color : column_formatter_base {
        private string color_ = "";
        private string expr_ = "";
        private Regex regex_ = null;

        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);
            color_ = sett.get("format");
            if (color_ != "" && !is_color_str_valid(color_))
                error = "Invalid color: " + color_;
            expr_ = sett.get("expr");
            try {
                regex_ = new Regex(expr_);
            } catch {
                error = "Invalid regex: " + expr_;
                regex_ = null;
            }
        }

        internal override void format_before(format_cell cell) {

        }

        internal override void format_after(format_cell cell) {
            if (expr_ == "" || regex_ == null)
                return;

            var text = cell.format_text.text;
            Color col = parse_color(color_, cell.fg_color);

            var found = util.regex_matches(regex_, text).OrderBy(x => x.Item1).ToList();
            // see if i need to replace with written in another base
            cell.format_text.add_parts( found.Select(x => new text_part(x.Item1, x.Item2) { fg = col }).ToList());
        }
    }
}
