using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using lw_common.ui.format.column_formatters.helper;

namespace lw_common.ui.format.column_formatters {
    /* 
	- regex to color/font/etc.
  	    - things in brackets - perhaps show them in slightly lighter color
	    - directory names in slighly different color
	    - strings (quotes and double quotes -> show them in different color, default = brown) 

    - allow for date/time as well (color_date/color_time)

    */
    class format : column_formatter_base {
        private multiline multi_ = new multiline();
        private cell color_ = new cell();
        private color_date date_ = new color_date();
        private color_time time_ = new color_time();
        private compare_number compare_number_ = new compare_number();
        private format_number format_number_ = new format_number();
        private alternate_bg_color alternate_bg_ = new alternate_bg_color();

        private List<column_formatter_base> sub_ = new List<column_formatter_base>(); 


        public format() {
            sub_.Add(multi_);
            sub_.Add(color_);
            sub_.Add(date_);
            sub_.Add(time_);
            sub_.Add(compare_number_);
            sub_.Add(format_number_);
            sub_.Add(alternate_bg_);
        }

        internal override void toggle_number_base() {
            format_number_.toggle_number_base();
        }

        internal override void toggle_abbreviation() {
            foreach ( var formatter in sub_)
                if ( formatter is abbreviation)
                    formatter.toggle_abbreviation();
        }

        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);

            load_sub_syntax(sett, multi_, "multiline", ref error);
            load_sub_syntax(sett, color_, "all", ref error);
            load_sub_syntax(sett, date_, "date", ref error);
            load_sub_syntax(sett, time_, "time", ref error);
            load_sub_syntax(sett, compare_number_, "compare-n", ref error);
            load_sub_syntax(sett, format_number_, "number", ref error);
            load_sub_syntax(sett, alternate_bg_, "alternate", ref error);

            for (int idx = 0;; idx++) {
                string prefix = "regex" + (idx > 0 ? "" + (idx + 1) : "");
                string regex_expr = sett.get(prefix + ".expr");
                if (regex_expr != "") {
                    regex_color regex = new regex_color();
                    load_sub_syntax(sett, regex, prefix, ref error);
                    sub_.Add(regex);
                } else break;
            }

            for (int idx = 0;; idx++) {
                string prefix = "abb" + (idx > 0 ? "" + (idx + 1) : "");
                string abbr_expr = sett.get(prefix + ".expr");
                if (abbr_expr != "") {
                    var abb = new abbreviation();
                    load_sub_syntax(sett, abb, prefix, ref error);
                    sub_.Add(abb);
                } else break;
            }
        }

        private void load_sub_syntax(settings_as_string sett, column_formatter_base sub, string prefix, ref string error) {
            prefix += ".";
            settings_as_string sub_sett = new settings_as_string("");
            foreach ( var name in sett.names())
                if ( name.StartsWith(prefix)) 
                    sub_sett.set(name.Substring(prefix.Length), sett.get(name));

            sub.load_syntax(sub_sett, ref error);
        }

        internal override void format_before_do_replace(format_cell cell) {
            foreach ( var formatter in sub_)
                formatter.format_before_do_replace(cell);
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
