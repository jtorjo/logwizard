using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    /* 
	show time - perhaps show only diff to previous (if any?) 
	 ----------> in this case, i need to always find out the first visible index (because the first visible index is always to be shown)
	             basically, hours, mins, seconds that are the same, show them in a lighter color
				 - have a different date/time font by default  - agency fb - bold,SimSun, DengXian


    Settings:
    color=color_string
    show_diff=0 or 1 (1 = default)
    */
    class color_time : column_formatter {
        private string color_ = "";
        private string light_color_ = "";
        private bool show_diff_ = true;

        protected info_type expected_type = info_type.time;

        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);
            color_ = sett.get("color");
            if (color_ != "" && !is_color_str_valid(color_))
                error = "Invalid color: " + color_;

            light_color_ = sett.get("light_color");
            if (light_color_ != "" && !is_color_str_valid(light_color_))
                error = "Invalid color: " + light_color_;

            show_diff_ = sett.get("show_diff", "1") == "1";
        }

        internal override void format_before(format_cell cell) {
        }


        internal override void format_after(format_cell cell) {
            if (color_ == "" && !show_diff_)
                // nothing to do
                return;
            if (cell.col_type != expected_type)
                return; // only color time column

            Color col = parse_color(color_, cell);
            if (col == util.transparent)
                col = cell.fg_color;

            var text = cell.format_text.text;
            bool needs_show_diff_now = show_diff_ && cell.row_index != cell.top_row_index;
            if (needs_show_diff_now) {
                int offset = util.datetime_difference_offset(cell.prev_text, text);
                Color lighter_col = light_color_ != "" ? parse_color(light_color_, cell) : util.grayer_color(col);
                var parts = new List<text_part>() { new text_part(0, offset) { fg = lighter_col }, new text_part(offset, text.Length - offset) { fg = col } };
                cell.format_text.add_parts( parts);
            }
            else 
                cell.format_text.add_part( new text_part(0, text.Length) { fg = col });
        }
    }
}
