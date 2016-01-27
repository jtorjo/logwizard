using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    // specify type: int (default) or double
    // specify where: before*after [|before*after |...]
    //  - if not specified, assume all column - if not found, don't do nothing
    //
    // specify: compare_to: what number to compare to
    //          colors for before, equal, after (color/bold/italic)
    /* 
        compare=value
        compare2=value2
        ...
    */
    class compare_number : column_formatter {
    }
}
