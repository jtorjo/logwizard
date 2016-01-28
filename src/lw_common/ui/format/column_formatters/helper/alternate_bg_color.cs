using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    // alternates the background slightly - for every X rows
    class alternate_bg_color : column_formatter {
        // if <= 0, don't alternate. Otherwise, alternate every X rows
        private int row_count_ = 0;
        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);
            int.TryParse(sett.get("row_count"), out row_count_);
        }

        internal override void format_before(format_cell cell) {
        }

        internal override void format_after(format_cell cell) {
            if (row_count_ < 1)
                return;
            int per_row = cell.row_index / row_count_;

            Color dark = util.str_to_color("#fbfdf9");// util.darker_color(cell.bg_color, 1.1);
            if (per_row % 2 == 1)
                cell.format_text.bg = dark;
        }
    }
}
