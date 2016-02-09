using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui {

    public class category_format_settings {

        private const string delim = "^^^^";
        private const string color_delim = "|";
        private const string inside_color_delim = "@@@@";

        private settings_as_string sett_;

                // for a given log
                // I need to know the default category type (default: thread)
                // for each category type -> have the default category colors 

                // settings_as_string ??? have delimeter instead of "\r\n"

        public category_format_settings(string str) {
            sett_ = new settings_as_string(str, delim);
        }

        public string default_category_type {
            get { return sett_.get("default_category_type", "thread"); }
            set {
                sett_.set("default_category_type", value);
            }
        }

        public List<category_colors> get_colors(info_type type, List<string> possible_values ) {
            List<category_colors> existing = new List<category_colors>();
            var colors_now = sett_.get(type.ToString()).Split( new [] { color_delim }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var str in colors_now) {
                var cur_col = str.Split(new[] {inside_color_delim}, StringSplitOptions.None);
                Debug.Assert(cur_col.Length == 4);
                category_colors cur_colors = new category_colors { name = cur_col[0], bg_color = util.str_to_color(cur_col[1]) };
                if (cur_col[2] != "")
                    cur_colors.raw_same_category_bg = util.str_to_color(cur_col[2]);
                if (cur_col[3] != "")
                    cur_colors.raw_this_category_bg = util.str_to_color((cur_col[3]));
                existing.Add(cur_colors);
            }


            List<category_colors> result = new List<category_colors>();
            foreach (var value in possible_values) {
                var found = existing.FirstOrDefault(x => x.name == value);
                if ( found != null)
                    result.Add(found);
                else 
                    result.Add(new category_colors { name = value });
            }
            return result;
        }

        public void set_colors(info_type type, List<category_colors> colors) {
            string str = "";
            foreach (var color in colors) {
                var same = color.raw_same_category_bg != util.transparent ? util.color_to_str(color.raw_same_category_bg) : "";
                var this_ = color.raw_this_category_bg != util.transparent ? util.color_to_str(color.raw_this_category_bg) : "";
                str += color.name + inside_color_delim + util.color_to_str(color.bg_color) + inside_color_delim + same + inside_color_delim + this_ +
                       color_delim;
            }
            sett_.set(type.ToString(), str);
        } 

        public override string ToString() {
            return sett_.ToString();
        }
    }
}
