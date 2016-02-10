using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    // alternates the background slightly - for every X rows
    class alternate_bg_color : column_formatter_base {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // if <= 0, don't alternate. Otherwise, alternate every X rows
        private int row_count_ = 0;
        private string alternate_color_ = "";
        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);
            int.TryParse(sett.get("row_count"), out row_count_);
            alternate_color_ = sett.get("color", "darker");
        }

        internal override void format_before(format_cell cell) {
        }

        internal override void format_after(format_cell cell) {
            if (row_count_ < 1)
                return;
            int per_row = cell.row_index / row_count_;
            logger.Debug("row " + cell.row_index + " - " + cell.format_text.text);

            if (per_row % 2 == 1) {
                Color dark = parse_color(alternate_color_, cell.bg_color); 
                cell.format_text.bg = dark;
            }
        }
    }
}
