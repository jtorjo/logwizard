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

        private List<column_formatter> sub_ = new List<column_formatter>(); 
        public format() {
            sub_.Add(color_);
            sub_.Add(date_);
            sub_.Add(time_);
            sub_.Add(compare_number_);
            sub_.Add(format_number_);
            sub_.Add(alternate_bg_);
            sub_.Add(regex_color_);
        }

        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);

            load_sub_syntax(sett, color_, "all", ref error);
            load_sub_syntax(sett, date_, "date", ref error);
            load_sub_syntax(sett, time_, "time", ref error);
            load_sub_syntax(sett, compare_number_, "compare-n", ref error);
            load_sub_syntax(sett, format_number_, "number", ref error);
            load_sub_syntax(sett, alternate_bg_, "alternate", ref error);
            load_sub_syntax(sett, regex_color_, "regex", ref error);
        }

        private void load_sub_syntax(settings_as_string sett, column_formatter sub, string prefix, ref string error) {
            prefix += ".";
            settings_as_string sub_sett = new settings_as_string("");
            foreach ( var name in sett.names())
                if ( name.StartsWith(prefix)) 
                    sub_sett.set(name.Substring(prefix.Length), sett.get(name));

            sub.load_syntax(sub_sett, ref error);
        }

        internal override void format_before(format_cell cell) {
            foreach ( var formatter in sub_)
                formatter.format_before(cell);
        }

        internal override void format_after(format_cell cell) {
            foreach ( var formatter in sub_)
                formatter.format_after(cell);
        }
    }
}
