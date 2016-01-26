/* 
 * Copyright (C) 2014-2016 John Torjo
 *
 * This file is part of LogWizard
 *
 * LogWizard is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * LogWizard is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact john.code@torjo.com 
 *  * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using lw_common.ui.format;

namespace lw_common.ui {
    class search_renderer : BaseRenderer {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private log_view_item_draw_ui drawer_ = null;
        private search_form search_;

        private solid_brush_list brush_ = new solid_brush_list();


        private formatted_text override_print_ = null;
        // search line - contains the colors for showing the parts of the line where there searched text is not found
        text_part default_, search_text_, around_search_;

        public search_renderer(log_view parent, search_form search) {
            search_ = search;
            drawer_ = new log_view_item_draw_ui(parent);
            drawer_.set_font(parent.list.Font);

            Color normal_fg_ = app.inst.fg, normal_bg_ = app.inst.bg;
            default_ = new text_part(0,0) { fg = normal_fg_, bg = normal_bg_, };
            
            Color search_line_fg_ = app.inst.search_found_full_line_fg, search_line_bg_ = app.inst.bg;
            around_search_ = new text_part(0,0) { fg = search_line_fg_, bg = search_line_bg_ };

            Color search_fg_ = app.inst.search_found_fg, search_bg_ = util.darker_color(app.inst.bg);
            search_text_ = new text_part(0,0) { text = "not important", fg = search_fg_, bg = search_bg_, bold = true, italic = false };
        }


        private void draw_sub_string(int left, string sub, Graphics g, Brush b, Rectangle r, StringFormat fmt, text_part print) {
            int width = text_width(g, sub);
            if (print != default_) {
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
            var parts = override_print_.parts(around_search_);
            if (parts.Count == 1) {
                // no overrides at all (search not found)
                draw_sub_string(left, s, g, b, r, fmt, default_);
                return;
            }

            foreach (var part in parts) {
                int left_offset = left + text_width(g, s.Substring(0, part.start) );
                draw_sub_string(left_offset, part.text, g, b, r, fmt, part);
            }
        }

        private int text_width(Graphics g, string text) {
            return drawer_.text_width(g, text);
        }

        public override void Render(Graphics g, Rectangle r) {
            DrawBackground(g, r);

            string text = GetText();
            override_print_ = override_print_from_search(text);

            g.FillRectangle(brush_.brush(default_.bg), r);

            StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap);
            fmt.LineAlignment = StringAlignment.Center;
            fmt.Trimming = StringTrimming.EllipsisCharacter;
            switch (this.Column.TextAlign) {
                case HorizontalAlignment.Center: fmt.Alignment = StringAlignment.Center; break;
                case HorizontalAlignment.Left: fmt.Alignment = StringAlignment.Near; break;
                case HorizontalAlignment.Right: fmt.Alignment = StringAlignment.Far; break;
            }

            draw_string(0, text, g, brush_.brush(default_.bg), r, fmt);
        }

        private formatted_text override_print_from_search(string text) {
            List<text_part> print = new List<text_part>();
            var matches = string_search.match_indexes(text, search_.running_search);
            if (matches.Count > 0) {
                foreach ( var match in matches)
                    print.Add( new text_part(match.Item1, match.Item2, search_text_));
            }

            formatted_text format = new formatted_text(text);
            format.add_parts(print);
            return format;
        }
    }
}
