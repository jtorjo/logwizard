using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters.helper {
    class abbreviation : column_formatter_base {
        private bool is_on_ = true;
        private string find_ = "";
        // syntax: anything in-between [" and "] is formatting to be applied onwards
        private string original_replace_string_ = "";

        // ... we allow up to 99 sub-formatting expressions -which should be waaay more than enough
        private const string FORMAT_PREFIX = "__Log_Wizard_FORMAT__";
        private const int MAX_PREFIX = 99;

        private class abbreviation_part {
            public string prefix = "";
            public string format_prefix = "";
            public text_part format = null;
        }
        private List< abbreviation_part> replace_parts_ = new List<abbreviation_part>() ;
        private string format_replace_string_ = "";

        private class format_part {
            public string prefix = "";
            public int len = 0;
            // if null, don't apply any formatting
            public text_part format;
        }


        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);
            find_ = sett.get("find");
            original_replace_string_ = sett.get("replace");
            try {
                new Regex(find_);
            } catch {
                error = "Invalid Find regex: " + find_;
                is_on_ = false;
            }
            try {
                Regex.Replace("blablabla", find_, original_replace_string_);
            } catch {
                error = "Invalid Replace regex: " + original_replace_string_;
                is_on_ = false;
            }
            if ( error == "")
                parse_replace_format();
        }

        private void parse_replace_format() {
            string remaining = original_replace_string_;
            while (true) {
                int start = remaining.IndexOf("[\"");
                int end = start >= 0 ? remaining.IndexOf("\"]", start + 2) : -1;
                if (start < 0 || end < 0) {
                    if (remaining != "") {
                        replace_parts_.Add(new abbreviation_part { prefix = remaining, format_prefix = FORMAT_PREFIX + MAX_PREFIX });
                        break;
                    }
                }

                string format = remaining.Substring(start + 2, end - start - 2);
                string prefix = remaining.Substring(0, start);
                remaining = remaining.Substring(end + 2);

                replace_parts_.Add( new abbreviation_part { prefix = prefix, 
                    format_prefix = FORMAT_PREFIX + replace_parts_.Count.ToString("00"), format = text_part.from_friendly_string(format) } );
            }

            if (replace_parts_.Count == 1)
                // in this case, I'm not doing any formatting
                replace_parts_[0].format_prefix = "";

            format_replace_string_ = util.concatenate( replace_parts_.Select(x => x.prefix + x.format_prefix), "");
        }

        internal override void format_before_do_replace(format_cell cell) {
            if (find_ == "" || !is_on_)
                return;
            if (cell.location != format_cell.location_type.view && cell.location != format_cell.location_type.smart_edit)
                return;

            string txt = cell.format_text.text;
            txt = Regex.Replace(txt, find_, format_replace_string_, RegexOptions.IgnoreCase);
            if (txt != cell.format_text.text) {
                bool needs_formatting = replace_parts_.Count > 1;
                if ( needs_formatting) {
                    var parts = formatted_parts(txt);
                    txt = util.concatenate(parts.Select(x => x.prefix), "");
                    cell.format_text.replace_text(0, cell.format_text.text.Length, txt);
                    // and now, apply the actual formatting
                    apply_format(cell, parts);
                }
                else 
                    // in this case, we don't have any formatting to do
                    cell.format_text.replace_text(0, cell.format_text.text.Length, txt);
            }
        }

        private void apply_format(format_cell cell, List<format_part> format) {
            int cur_idx = 0;
            var apply_parts = new List<text_part>();
            foreach (var part in format) {
                cur_idx += part.prefix.Length;
                if ( part.format != null)
                    apply_parts.Add( new text_part(cur_idx, part.len, part.format));
            }
            cell.format_text.add_parts(apply_parts);
        }

        private List<format_part> formatted_parts(string txt) {
            List<int> indexes = new List<int>();
            int cur_idx = 0;
            while (true) {
                int next_idx = txt.IndexOf(FORMAT_PREFIX, cur_idx);
                if (next_idx < 0)
                    break;
                indexes.Add(next_idx);
                cur_idx = next_idx + FORMAT_PREFIX.Length + 2; // 2 = the suffix containing the formatting index
            }
            List<format_part> parts = new List<format_part>();
            if (indexes.Count < 1) {
                // no formatting whatsoever
                parts.Add( new format_part { prefix = txt });
                return parts;
            }

            int prev_idx = 0;
            foreach (var idx in indexes) {
                int part_index = int.Parse(txt.Substring(idx + FORMAT_PREFIX.Length, 2));
                var format = part_index < replace_parts_.Count ? replace_parts_[part_index].format : null;
                var prefix = txt.Substring(prev_idx, idx - prev_idx);
                if (parts.Count > 0)
                    parts.Last().len = prefix.Length;
                format_part part = new format_part { prefix = prefix, format = format };
                parts.Add(part);
                prev_idx = idx + FORMAT_PREFIX.Length + 2; // 2 = the suffix containing the formatting index
            }

            string last_part = txt.Substring(indexes.Last() + FORMAT_PREFIX.Length + 2);
            if (last_part != "") {
                parts.Last().len = last_part.Length;
                parts.Add(new format_part {prefix = last_part});
            }

            return parts;
        }

        internal override void toggle_abbreviation() {
            is_on_ = !is_on_;
        }
    }
}
