using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters.helper {
    /* 
    Syntax:
        
    // base to write on - default=10
    base=value

    // padding, similar to string.format, such as "0.00" or stuff
    pad=value

    // the color to show the numbers on - default = red
    // '-' - means no color
    color=value

    // if true, we look for hexa values - i will just write them with the given color, won't tranform them into any base or something
    look_for_hex=0 or 1 (1 by default)

    */
    class format_number : column_formatter {
        private int base_ = 10;
        private string pad_ = "";
        private string color_ = "";
        private bool look_for_hex_ = true;

        // ... not perfect, but a good start
        //private Regex regex_hex_ = new Regex(@"(?<=[\s=,{\(\[<>/])[0-9a-fA-F]{4,30}(?=[\s.,<>/=\-}\]\)+*])");
        // 1.7.12 - limit the possibility of false positives - search only for only capital letters (a-f)
        private Regex regex_hex_ = new Regex(@"(?<=[\s=,{\(\[<>/])[0-9A-F]{4,30}(?=[\s.,<>/=\-}\]\)+*])");

        private Regex regex_decimal_ = new Regex(@"(?<=[\s=,{\(\[<>/])\d*[,.]?\d*(?=[\s.,<>/=\-}\]\)+*])");

        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);
            base_ = int.Parse(sett.get("base", "10"));
            pad_ = sett.get("pad");
            color_ = sett.get("color", "red");
            if (color_ == "-")
                color_ = "";
            look_for_hex_ = sett.get("look_for_hex", "1") == "1";
        }

        // we know the number **can** be hex. I want to know that it is hex, and not decimal
        private static bool double_check_is_hex(string s) {
            return s.Any(x => !Char.IsDigit(x));
        }

        // returning "" means we could not convert
        private string convert_number(string str) {
            long n;
            double d;
            bool ok1 = long.TryParse(str, out n);
            bool ok2 = double.TryParse(str, out d);

            if (!ok1 && !ok2)
                // either this was a double (which we don't transorm into another base), or we could just not interpret it as a number
                return "";

            try {
                if ( !ok1)
                    // in this case, it's a double - we can only perform padding
                    return pad_ != "" ? String.Format("{0:" + pad_ + "}", d) : "";

                // here, it's a decimal
                if ( base_ == 10)
                    // perform padding, if any
                    return pad_ != "" ? String.Format("{0:" + pad_ + "}", n) : "";

                // here, it's in a different base. Note: we can't have both base and padding. Base overrides padding
                return Convert.ToString(n, base_).ToUpper();
            } catch {
                return "";
            }
        }

        internal override void format_before(format_cell cell) {
            if (color_ == "" && base_ == 10 && pad_ == "" && !look_for_hex_)
                // we don't need to do anything
                return;

            // if number too big, don't do anything
            // also, doubles -> don't care about base
            var text = cell.format_text.text;

            Color col = parse_color(color_, cell);
            if (look_for_hex_) {
                // ... note: we don't want the delimeters included
                var hex_numbers =
                    util.regex_matches(regex_hex_, text).Where(x => double_check_is_hex(text.Substring(x.Item1, x.Item2))).ToList();
                cell.format_text.add_parts( hex_numbers.Select(x => new text_part(x.Item1, x.Item2) { fg = col }).ToList() );
            }

            /* IMPORTANT: we have the following assumption: the hexa numbers NEVER interfere with the decimal numbers.

                Here's the rationale: the decimal numbers can be transformed, while the hex numbers will NEVER be transformed. In other words, we can transform
                something like "Number 255 is awesome." into "Number FF is awesome". Thus, we're replacing sub-text in a a text that also has formatting.

                Normally, that should work ok (formatted_text.replace_text), but if texts got to be imbricated one another, things could get ugly 

            */
            var dec_numbers = util.regex_matches(regex_decimal_, text).OrderBy(x => x.Item1).ToList();
            // see if i need to replace with written in another base
            cell.format_text.add_parts( dec_numbers.Select(x => new text_part(x.Item1, x.Item2) { fg = col }).ToList());

            int start_offset = 0;
            foreach (var cur_number_offset in dec_numbers) {
                int start = cur_number_offset.Item1 + start_offset, len = cur_number_offset.Item2;
                var new_number = convert_number(cell.format_text.text.Substring(start, len));
                if (new_number != "") {
                    cell.format_text.replace_text(start, len, new_number);
                    int diff = new_number.Length - len;
                    start_offset += diff;
                }            
            }
        }

        internal override void format_after(format_cell cell) {
        }
    }
}
