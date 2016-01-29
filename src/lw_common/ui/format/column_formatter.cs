using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format {
    public abstract class column_formatter {
        internal class format_cell {
            public readonly match_item item;
            public readonly log_view parent;
            public readonly int col_idx;
            public readonly info_type col_type;
            public readonly formatted_text format_text;

            public readonly Color fg_color;
            public readonly Color bg_color;

            public readonly int row_index;
            public readonly int top_row_index;
            // the text from the cell above (if any)
            public readonly string prev_text;

            public enum location_type {
                view, details_pane, msg_description, smart_edit
            }

            public readonly location_type location;

            public format_cell(match_item item, log_view parent, int col_idx, info_type col_type, formatted_text text, int row_index, int top_row_index, string prev_text, location_type location) {
                this.item = item;
                this.parent = parent;
                this.col_idx = col_idx;
                this.col_type = col_type;
                this.format_text = text;
                this.row_index = row_index;
                this.top_row_index = top_row_index;
                this.prev_text = prev_text;
                this.location = location;

                fg_color = item.fg(parent);
                bg_color = item.bg(parent);
            }
        }

        // FIXME avoid this - I can use the text_part.from_friendly_string
        protected Color parse_color(string str, Color col) {
            switch (str) {
            case "darker":
                return util.darker_color(col);
            case "lighter":
                return util.grayer_color(col);
            default:
                return util.str_to_color(str);
            }
        }

        protected bool is_color_str_valid(string str) {
            switch (str) {
            case "darker":
            case "lighter":
                return true;
            }
            var col = util.str_to_color(str);
            if (col == util.transparent && str != "transparent")
                return false;
            return true;
        }

        internal virtual void load_syntax(settings_as_string sett, ref string error) {
        }

        /*  You should override this is you're replacing large parts of the text. The idea is to do this first,
            because replacing large text (thtat internally contains formatted parts) can go wrong. Namely, the formatted parts might point to the wrong location.

            Example: 
            say I'm replacig "I have 23 lines of awesome text." with "I have plenty of awesome text".
            Say the "23" part is formatted to show in red. What shall I do with this part in the replaced text? 
            I would end up showing "pl" characters in red - which makes no sense.
        */
        internal virtual void format_before_do_replace(format_cell cell) { 
        }

        internal virtual void format_before(format_cell cell) { 
        }
        internal virtual void format_after(format_cell cell) {            
        }
    }
}
