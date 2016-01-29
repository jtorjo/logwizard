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
        alternate_format=format
        separator_format=format
    */
    class multiline : column_formatter {
        private bool show_multi_into_single_line_ = false;

        private string separator_ = "";

        private text_part alternate_format_;
        private text_part separator_format_;

        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);
            show_multi_into_single_line_ = sett.get("multi", "0") != "0";
            separator_ = sett.get("separator", " ¶ ");

            var alternate = sett.get("alternate_format");
            var separator = sett.get("separator_format");
            if ( alternate != "")
                alternate_format_ = text_part.from_friendly_string(alternate);
            if ( separator != "")
                separator_format_ = text_part.from_friendly_string(separator);
        }

        internal override void format_before_do_replace(format_cell cell) {
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

            if (alternate_format_ != null || separator_format_ != null) {
                if ( alternate_format_ != null)
                    alternate_format_.update_colors(cell);
                if ( separator_format_ != null)
                    separator_format_.update_colors(cell);

                int cur_idx = 0;
                var line_indexes = new List<int>();
                foreach (var line in lines) {
                    line_indexes.Add(cur_idx);
                    cur_idx += line.Length + separator_.Length;
                }
                List<text_part> parts = new List<text_part>();
                for (int i = 0; i < line_indexes.Count; i++) {
                    bool even = i % 2 == 0;
                    if ( separator_format_ != null && i < line_indexes.Count - 1)
                        parts.Add( new text_part(line_indexes[i] + lines[i].Length, separator_.Length, separator_format_) );
                    if (!even) {
                        // odd - show in alternate
                        if ( alternate_format_ != null)
                            parts.Add( new text_part( line_indexes[i], lines[i].Length, alternate_format_));
                    }
                }
                cell.format_text.add_parts(parts);
            }
        }

        internal override void format_after(format_cell cell) {
        }
    }
}
