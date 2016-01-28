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
    Syntax:


    // I compare the number with "value"
    // before, equal and after will specify color/font information for each of the cases
    // a "-" means don't do anything
    //
    // note that you can compare the number to several values
    compare=value,before,equal,after
    compare2=value2,before2,equal2,after2
    ...

    // if not set, it's int ; valid values = int, double
    value=type

    // where do we find the number ; if not specified, it's the full cell
    // location is like this: "before*after" -> this means that the number is in between "before" and "after"
    where=location






- comparing numbers: allow comparing for several values, such as:
  // compare against 100, then against 200
  compare=100,green,-,orange
  compare=200,-,-,red

    */
    class compare_number : column_formatter {
        private bool is_int_ = true;
        private string before_ = "", after_ = "";

        private class compare {
            public long compare_int = 0;
            public double compare_double = 0;

            public text_part less = null, equal = null, greater = null;
        }
        private List<compare> compare_ = new List<compare>();

        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);

            var where = sett.get("where");
            if (where != "") {
                int delim = where.IndexOf("*");
                if (delim >= 0) {
                    before_ = where.Substring(0, delim);
                    after_ = where.Substring(delim + 1);
                } else
                    before_ = where;
            }
            var value_type = sett.get("value", "int");
            is_int_ = value_type != "double";

            for (int idx = 0;; ++idx) {
                var compare_str = sett.get("compare" + (idx > 0 ? "" + (idx + 1) : ""));
                if (compare_str == "")
                    break;
                var compare_parts = compare_str.Split(',');
                if (compare_parts.Length != 4) {
                    error = "Invalid compare string: " + compare_str;
                    break;
                }
                compare comp = new compare();
                // read the number
                bool ok1 = double.TryParse(compare_parts[0], out comp.compare_double);
                bool ok2 = long.TryParse(compare_parts[0], out comp.compare_int);
                if (!ok1 && !ok2) {
                    error = "Invalid compare string: " + compare_str;
                    break;                    
                }
                if (ok1 && !ok2)
                    try {
                        comp.compare_int = (long) comp.compare_double;
                    } catch {}

                if ( compare_parts[1] != "-")
                    comp.less = text_part.from_friendly_string(compare_parts[1]);
                if ( compare_parts[2] != "-")
                    comp.equal = text_part.from_friendly_string(compare_parts[2]);
                if ( compare_parts[3] != "-")
                    comp.greater = text_part.from_friendly_string(compare_parts[3]);
                compare_.Add(comp);
            }
        }

        internal override void format_before(format_cell cell) {
        }

        internal override void format_after(format_cell cell) {
            if (compare_.Count < 1)
                return; // nothing to compare

            var text = cell.format_text.text;

            int before_idx = before_ != "" ? text.IndexOf(before_) : 0;
            if (before_idx >= 0) {
                before_idx += before_.Length;
                int after_idx = after_ != "" ? text.IndexOf(after_, before_idx) : text.Length;
                if (after_idx >= 0) {
                    var number = text.Substring(before_idx, after_idx - before_idx).Trim();
                    double val;
                    if (double.TryParse(number, out val)) {
                        // found the number - at this point, do all comparisons
                        text_part format_number = new text_part(0,0);
                        foreach (var comp in compare_) {
                            bool equal = false, less = false, greater = false;
                            try {
                                if (is_int_) {
                                    equal = (long) val == comp.compare_int;
                                    less = (long) val < comp.compare_int;
                                    greater = (long) val > comp.compare_int;
                                } else {
                                    equal = val == comp.compare_double;
                                    less = val < comp.compare_double;
                                    greater = val > comp.compare_double;
                                }
                            } catch {
                                // this can happen only if number is longer than 'long'
                                equal = less = greater = false;
                            }

                            if (equal && comp.equal != null)
                                format_number = format_number.merge_copy(comp.equal);
                            else if (less && comp.less != null)
                                format_number = format_number.merge_copy(comp.less);
                            else if (greater && comp.greater != null)
                                format_number = format_number.merge_copy(comp.greater);
                        }

                        cell.format_text.add_part( new text_part(before_idx, after_idx - before_idx, format_number));
                    }
                }
            }
        }
    }
}
