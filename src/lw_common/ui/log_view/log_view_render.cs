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
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using lw_common;
using lw_common.ui.format;

namespace lw_common.ui {
    class log_view_render : BaseRenderer {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private log_view parent_;
        private log_view_item_draw_ui drawer_ = null;

        private solid_brush_list brush_ = new solid_brush_list();

        private formatted_text  override_print_ = null;
        text_part default_ = new text_part(0, 0);
        private Color bg_color_ = util.transparent;

        private formatted_text_cache cache_;

        public log_view_render(log_view parent) {
            parent_ = parent;
            drawer_ = new log_view_item_draw_ui(parent_);
            cache_ = new formatted_text_cache(parent_, column_formatter.format_cell.location_type.view);
        }

        private void draw_sub_string(int left, string sub, Graphics g, Brush b, Rectangle r, StringFormat fmt, text_part print) {
            int width = drawer_.text_width(g, sub, drawer_.font(print));
            Color print_bg = drawer_.print_bg_color(ListItem, print);
            if (print_bg.ToArgb() != bg_color_.ToArgb()) {
                Rectangle here = new Rectangle(r.Location, r.Size);
                here.X += left;
                here.Width = width + 1;
                g.FillRectangle( brush_.brush( print_bg) , here);
            }

            Rectangle sub_r = new Rectangle(r.Location, r.Size);
            sub_r.X += left;
            sub_r.Width -= left;
            g.DrawString(sub, drawer_.font(print), brush_.brush( drawer_.print_fg_color(ListItem, print)) , sub_r, fmt);
        }


        private void draw_string(int left, string s, Graphics g, Brush b, Rectangle r, StringFormat fmt) {
            var prints = override_print_.parts(default_);
            foreach (var part in prints) {
                int left_offset = left + drawer_.text_offset(g, s.Substring(0, part.start), drawer_.font(part) );
                if (left_offset > r.Right)
                    // nothing to actually draw
                    // assuming we're going left to right, we're reached passed the end
                    break; 
                draw_sub_string(left_offset, part.text, g, b, r, fmt, part);
            }
        }

        // for each character of the printed text, see how many pixels it takes
        public List<int> text_widths(Graphics g ,string text) {
            List<int> widths = new List<int>();
            for ( int i = 0; i < text.Length; ++i)
                widths.Add( i > 0 ? drawer_.text_width(g, text.Substring(0, i)) : 0);
            return widths;
        }

        // 1.7.15 - make sure the line cell is fully drawn (height wize)
        protected override void DrawBackground(Graphics g, Rectangle r) {
            if (!this.IsDrawBackground)
                return;

            int PAD = 2;
            Color backgroundColor = this.GetBackgroundColor();

            using (Brush brush = new SolidBrush(backgroundColor)) {
                g.FillRectangle(brush, r.X - PAD, r.Y - PAD, r.Width + 2 * PAD, r.Height + 2 * PAD);
            }
        }

        public void clear_format_cache(string reason) {
            cache_.clear(reason);
        }

        public override void Render(Graphics g, Rectangle r) {
            // 1.3.30+ solved rendering issue :)
            DrawBackground(g, r);

            var i = ListItem.RowObject as match_item;
            if (i == null)
                return;

            var col_idx = Column.fixed_index();
            drawer_.cached_sel = parent_.multi_sel_idx;
            override_print_ = cache_.override_print(i, GetText(), col_idx);
            var text = override_print_.text;

            bg_color_ = drawer_.bg_color(ListItem, col_idx, override_print_);
            Brush brush = brush_.brush( bg_color_);
            g.FillRectangle(brush, r);

            StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap);
            fmt.LineAlignment = StringAlignment.Center;
            fmt.Trimming = override_print_.align == HorizontalAlignment.Left ? StringTrimming.EllipsisCharacter : StringTrimming.None;
            fmt.Alignment = StringAlignment.Near;

            int left = 0;
            if (override_print_.align != HorizontalAlignment.Left) {
                var full_text_size = drawer_.text_width(g, text, drawer_.font(override_print_.merge_parts)) + image_width();
                int width = r.Width;
                int extra = width - full_text_size;
                left = override_print_.align == HorizontalAlignment.Right ? extra - 5 : extra / 2;
            }
            left += image_width();

            draw_string(left, text, g, brush, r, fmt);
            draw_image(g, r);
        }

        private int image_width() {
            return override_print_.image != null ? override_print_.image.Width : 0;
        }

        private void draw_image(Graphics g, Rectangle r) {
            if (override_print_.image == null)
                return;

            string text = override_print_.text;
            int left = 0;
            if (override_print_.align != HorizontalAlignment.Left) {
                var full_text_size = drawer_.text_width(g, text, drawer_.font(override_print_.merge_parts)) + image_width();
                int width = r.Width;
                int extra = width - full_text_size;
                left = override_print_.align == HorizontalAlignment.Right ? extra - 5 : extra / 2;
            }
            g.DrawImage( override_print_.image, new Point(r.X + left, r.Y ));
        }
    }

}
