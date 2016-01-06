using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common.ui {
    // placed in the .Tag of each lv column
    internal class log_view_column_tag {

        public readonly log_view parent;

        public int line_width = -1;

        public log_view_column_tag(log_view parent) {
            this.parent = parent;
        }

    }
}
