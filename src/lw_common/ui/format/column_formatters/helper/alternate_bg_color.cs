using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    // alternates the background slightly - for every X rows
    class alternate_bg_color : column_formatter {
        // if <= 0, don't alternate. Otherwise, alternate every X rows
        public int row_count = 0;
    }
}
