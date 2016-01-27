using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lw_common.ui.format.column_formatters.helper;

namespace lw_common.ui.format.column_formatters {
    /* 
	- regex to color/font/etc.
  	    - things in brackets - perhaps show them in slightly lighter color
	    - directory names in slighly different color
	    - strings (quotes and double quotes -> show them in different color, default = brown) 

    - allow for date/time as well (color_date/color_time)

    */
    class format : column_formatter {
        private color color_ = new color();
        private color_date date_ = new color_date();
        private color_time time_ = new color_time();
        private compare_number compare_number_ = new compare_number();
        private format_number format_number_ = new format_number();
        private alternate_bg_color alternate_bg_ = new alternate_bg_color();
        private regex_color regex_color_ = new regex_color();

    }
}
