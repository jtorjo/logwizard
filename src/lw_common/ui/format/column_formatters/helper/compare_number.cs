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




- comparing numbers: allow comparing for several values, such as:
  // compare against 100, then against 200
  compare=100,green,-,orange
  compare=200,-,-,red

    */
    class compare_number : column_formatter {
    }
}
