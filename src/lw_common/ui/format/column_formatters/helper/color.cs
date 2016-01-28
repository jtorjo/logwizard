using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    class color : column_formatter {
        private string color_ = "";

        // extra formatting, if any
        private text_part formatting_ = null;

        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);
            var format = sett.get("format");
            if ( format != "")
                formatting_ = text_part.from_friendly_string(format);
        }

        internal override void format_before(format_cell cell) {
            var text = cell.format_text.text;
            if (formatting_ != null && formatting_.bg != util.transparent)
                cell.format_text.bg = formatting_.bg;
        }

        internal override void format_after(format_cell cell) {
            var text = cell.format_text.text;
            if ( formatting_ != null)
                cell.format_text.add_part(new text_part(0, text.Length, formatting_) );
        }
    }
}
