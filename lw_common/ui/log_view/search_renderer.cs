using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace lw_common.ui {
    class search_renderer : BaseRenderer {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private log_view_item_draw_ui drawer_ = null;
        private search_form search_;

        private solid_brush_list brush_ = new solid_brush_list();


        private List<Tuple<int, int, print_info>> override_print_ = new List<Tuple<int, int, print_info>> ();
        print_info default_print_, search_print_, search_line_print_;

        public search_renderer(log_view parent, search_form search) {
            search_ = search;
            drawer_ = new log_view_item_draw_ui(parent);
            drawer_.set_font(parent.list.Font);

            Color normal_fg_ = app.inst.fg, normal_bg_ = app.inst.bg;
            default_print_ = new print_info { fg = normal_fg_, bg = normal_bg_, };
            
            Color search_line_fg_ = app.inst.search_found_full_line_fg, search_line_bg_ = app.inst.bg;
            search_line_print_ = new print_info { fg = search_line_fg_, bg = search_line_bg_ };

            Color search_fg_ = app.inst.search_found_fg, search_bg_ = util.darker_color(app.inst.bg);
            search_print_ = new print_info { text = "not important", fg = search_fg_, bg = search_bg_, bold = true, italic = false };
        }


        private void draw_sub_string(int left, string sub, Graphics g, Brush b, Rectangle r, StringFormat fmt, print_info print) {
            int width = text_width(g, sub);
            if (print != default_print_) {
                Rectangle here = new Rectangle(r.Location, r.Size);
                here.X += left;
                here.Width = width + 1;
                g.FillRectangle( brush_.brush(print.bg), here);
            }

            Rectangle sub_r = new Rectangle(r.Location, r.Size);
            sub_r.X += left;
            sub_r.Width -= left;
            g.DrawString(sub, drawer_.font(print), brush_.brush( print.fg) , sub_r, fmt);
        }


        private void draw_string(int left, string s, Graphics g, Brush b, Rectangle r, StringFormat fmt) {
            if ( override_print_.Count < 1) {
                // no overrides at all (search not found)
                draw_sub_string(left, s, g, b, r, fmt, default_print_);
                return;
            }

            // here, we have at least one override (thus, the search matched at least one item)
            for (int idx = 0; idx < override_print_.Count; ++idx) {
                int start_normal = idx > 0 ? override_print_[idx - 1].Item1 + override_print_[idx - 1].Item2 : 0;
                int normal_len = override_print_[idx].Item1 - start_normal;

                string up_to_prev = s.Substring(0, start_normal);
                string up_to_now = s.Substring(0, override_print_[idx].Item1);
                int left_normal = left + text_width(g, up_to_prev);
                int left2 = left + text_width(g, up_to_now);

                // first, draw the normal text
                draw_sub_string(left_normal, s.Substring(start_normal, normal_len), g, b, r, fmt, search_line_print_);
                draw_sub_string(left2, s.Substring( override_print_[idx].Item1, override_print_[idx].Item2 ), g, b, r, fmt, override_print_[idx].Item3);
            }

            var last_override = override_print_.Last();
            int last = last_override.Item1 + last_override.Item2;
            string last_normal = s.Substring(last);
            if (last_normal != "") {
                string up_to_now = s.Substring(0, last_override.Item1 + last_override.Item2);
                int last_left = left + text_width(g, up_to_now);
                draw_sub_string(last_left, last_normal, g, b, r, fmt, search_line_print_);
            }
        }

        private int text_width(Graphics g, string text) {
            return drawer_.text_width(g, text);
        }

        public override void Render(Graphics g, Rectangle r) {
            DrawBackground(g, r);

            string text = GetText();
            override_print_ = override_print_from_search(text);

            g.FillRectangle(brush_.brush(default_print_.bg), r);

            StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap);
            fmt.LineAlignment = StringAlignment.Center;
            fmt.Trimming = StringTrimming.EllipsisCharacter;
            switch (this.Column.TextAlign) {
                case HorizontalAlignment.Center: fmt.Alignment = StringAlignment.Center; break;
                case HorizontalAlignment.Left: fmt.Alignment = StringAlignment.Near; break;
                case HorizontalAlignment.Right: fmt.Alignment = StringAlignment.Far; break;
            }

            draw_string(0, text, g, brush_.brush(default_print_.bg), r, fmt);
        }

        private List<Tuple<int, int, print_info>> override_print_from_search(string text) {
            List<Tuple<int, int, print_info>> print = new List<Tuple<int, int, print_info>>();
            var matches = string_search.match_indexes(text, search_.running_search);
            if (matches.Count > 0) {
                foreach ( var match in matches)
                    print.Add( new Tuple<int, int, print_info>(match.Item1, match.Item2, search_print_));
            }
            return print;
        }
    }
}
