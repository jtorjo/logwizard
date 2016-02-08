using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    class category_formatter {
        public bool running = false;

        private info_type column_ = info_type.max;
        private Dictionary<string, categories_ctrl.category_colors> colors_ = null;

        public void set_colors(List<categories_ctrl.category_colors> colors, info_type column) {
            
        }
        internal void format(formatted_text text, string sel_text) {
            if (!running || colors_ == null || colors_.Count < 1)
                return;
            categories_ctrl.category_colors category_col;
            if (colors_.TryGetValue(text.text, out category_col)) {
                var color = text.text == sel_text ? category_col.this_category_bg : category_col.same_category_bg;
                text.bg = color;
            }

        }
    }
}
