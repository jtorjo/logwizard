using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    /* 
    Settings:

    format=format_string (text_part)
    show_diff=0 or 1 (1 = default)
    time_format=formatting for showing time, a' la https://msdn.microsoft.com/en-us/library/8kb3ddd4%28v=vs.110%29.aspx

    // what to show as "lighter" color
    light=color
    */
    class color_time : column_formatter {
        private string light_color_ = "";
        private bool show_diff_ = true;

        protected info_type expected_type = info_type.time;
        // extra formatting, if any
        private text_part formatting_ = null;

        private string format_time_ = "";

        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);
            light_color_ = sett.get("light");
            if (light_color_ != "" && !is_color_str_valid(light_color_))
                error = "Invalid color: " + light_color_;

            show_diff_ = sett.get("show_diff", "1") == "1";
            var format = sett.get("format");
            if ( format != "")
                formatting_ = text_part.from_friendly_string(format);

            format_time_ = sett.get("format_time");
            try {
                if (format_time_ != "")
                    DateTime.Now.ToString(format_time_);
            } catch {
                error = "Invalid time format: " + format_time_;
                format_time_ = "";
            }
        }

        internal override void format_before(format_cell cell) {
            if (cell.col_type != expected_type)
                return; // only color time column

            var time = (cell.item as filter.match).line.time;
            if (format_time_ != "" && time != DateTime.MinValue)
                cell.format_text.replace_text(0, cell.format_text.text.Length, time.ToString(format_time_));
        }

        private string prev_text(format_cell cell) {
            string prev = cell.prev_text;
            if (cell.row_index > 0 && format_time_ != "") {
                var prev_item = cell.parent.item_at(cell.row_index - 1);
                var time = (prev_item as filter.match).line.time;
                if (time != DateTime.MinValue)
                    prev = time.ToString(format_time_);
            }
            return prev;
        }


        internal override void format_after(format_cell cell) {
            if (formatting_ == null && light_color_ == "" && !show_diff_)
                // nothing to do
                return;
            if (cell.col_type != expected_type)
                return; // only color time column

            Color col = formatting_ != null ? formatting_.fg : util.transparent;
            if (col == util.transparent)
                col = cell.fg_color;

            var text = cell.format_text.text;
            if ( formatting_ != null)
                cell.format_text.add_part(new text_part(0, text.Length, formatting_) );

            bool needs_show_diff_now = show_diff_ && cell.row_index != cell.top_row_index;
            if (needs_show_diff_now) {
                int offset = util.datetime_difference_offset( prev_text(cell), text);
                Color lighter_col = light_color_ != "" ? parse_color(light_color_, cell.fg_color) : util.grayer_color(col);
                var parts = new List<text_part>() { new text_part(0, offset) { fg = lighter_col }, new text_part(offset, text.Length - offset) { fg = col } };
                cell.format_text.add_parts( parts);
            }
            else 
                cell.format_text.add_part( new text_part(0, text.Length) { fg = col });
        }
    }
}
