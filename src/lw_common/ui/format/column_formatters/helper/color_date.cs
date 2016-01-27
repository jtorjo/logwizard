using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    class color_date : color_time {
        public color_date() {
            expected_type = info_type.date;
        }
    }
}
