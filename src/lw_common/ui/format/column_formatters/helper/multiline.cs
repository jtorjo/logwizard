using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    /* 
	multiline 
	  - when on view, show the paragraph sign
	  - allow showing everything as a single line (replace enter by space)

    Settings:
        // if 1, show multiple lines on the same line
        multi=0 or 1 (0=default)
        
        // what to show to visually separate the lines, by default,  ¶
        separator=what_to_show_between_lines

        // what to show each alternate line
        alternate=color
        separator_color=color
    */
    class multiline : column_formatter {
        private bool show_multi_into_single_line_ = false;

        private string separator_ = "";

        private string alternate_color_ = "";
        private string separator_color_ = "";

        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);
            show_multi_into_single_line_ = sett.get("multi", "0") != "0";
            separator_ = sett.get("separator", " ¶ ");
            alternate_color_ = sett.get("alternate");
            separator_color_ = sett.get("separator_color");
        }

        internal override void format_before(format_cell cell) {
            // see if on view
            if (cell.location != format_cell.location_type.view)
                return;
            if (!show_multi_into_single_line_)
                return;

            // ... make sure we catch all lines
            var lines = cell.format_text.text.Replace("\r\n", "\r").Replace("\n", "\r").Split(new[] {"\r"}, StringSplitOptions.None);
            if (lines.Length <= 1)
                return; // single line

            var new_text = util.concatenate(lines, separator_);
            cell.format_text.replace_text(0, cell.format_text.text.Length, new_text);

            if (alternate_color_ != "" || separator_color_ != "") {
                int cur_idx = 0;
                var line_indexes = new List<int>();
                foreach (var line in lines) {
                    line_indexes.Add(cur_idx);
                    cur_idx += line.Length + separator_.Length;
                }
                List<text_part> parts = new List<text_part>();
                Color alternate_col = parse_color(alternate_color_, cell);
                Color separator_col = parse_color(separator_color_, cell);
                for (int i = 0; i < line_indexes.Count; i++) {
                    bool even = i % 2 == 0;
                    if (even) {
                        if ( separator_color_ != "")
                            parts.Add( new text_part(line_indexes[i] + lines[i].Length, separator_.Length) { fg = separator_col } );
                    } else {
                        // odd - show in alternate
                        if ( alternate_color_ != "")
                            parts.Add( new text_part( line_indexes[i], lines[i].Length) { fg = alternate_col });
                    }
                }
                cell.format_text.add_parts(parts);
            }
        }

        internal override void format_after(format_cell cell) {
        }
    }
}
