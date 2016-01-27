using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    class color : column_formatter {
        private string color_ = "";

        internal override void load_syntax(settings_as_string sett, ref string error) {
            color_ = sett.get("color");
            if (!is_color_str_valid(color_))
                error = "Invalid color: " + color_;
        }

        internal override void format_before(format_cell cell) {
        }

        internal override void format_after(format_cell cell) {
            var text = cell.format_text.text;
            if ( color_ != "")
                cell.format_text.add_part( new text_part(0, text.Length) { fg = parse_fg_color(color_, cell) });
        }
    }
}
