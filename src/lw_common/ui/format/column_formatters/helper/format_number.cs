using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters.helper {
    /* 
	- formatting numbers - allow padding etc. (like, xxx.yyy etc)
	  - allow specifying base (16, 8, 10, 2) - only for integers 
	     - allow printing base as well (0x, 08)
	  - q: how do i identify a number? (by default, look for separators)
	  - default: show them in red
	- catch hexadecimal numbers

    allow showing numbers normally (that is, not to override their color)

    */
    class format_number : column_formatter {
    }
}
