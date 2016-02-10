using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lw_common.ui.format;

namespace lw_common.ui {
    class category_formatter {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool running = false;

        private info_type column_ = info_type.max;
        private Dictionary<string, category_colors> colors_ = null;

        public void set_colors(List<category_colors> colors, info_type column) {
            try {
                colors_ = colors.ToDictionary(x => x.name, x => x);
            } catch {
                colors_ = null;
                logger.Error("invalid color names " + util.concatenate(colors.Select(x => x.name), ", ") );
            }
            column_ = column;
        }
        internal void format(formatted_text text, match_item row, match_item sel, info_type col_type) {
            if (!running || colors_ == null || colors_.Count < 1 || column_ == info_type.max)
                return;
            if (col_type == info_type.line)
                // don't category format the line
                return;

            var row_text = log_view_cell.cell_value_by_type(row, column_);
            var sel_text = log_view_cell.cell_value_by_type(sel, column_);

            category_colors category_col;
            if (colors_.TryGetValue(row_text, out category_col)) {
                var color = row_text == sel_text ? category_col.this_category_bg : category_col.same_category_bg;
                text.bg = color;
                text.update_parts();
            }

        }
    }
}
