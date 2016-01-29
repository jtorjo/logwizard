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
using System.Linq;
using System.Text;
using BrightIdeasSoftware;
using lw_common.ui.format;

namespace lw_common.ui {
    internal class log_view_item_draw_ui {

        private log_view parent_;

        private font_list fonts_ = new font_list();

        // if true, we consider none of the items is selected
        public bool ignore_selection = false;

        public log_view_item_draw_ui(log_view parent) {
            parent_ = parent;
        }

        public Font font(text_part print) {
            string font_name = print != null && print.font_name != "" ? print.font_name : default_font.FontFamily.Name;
            int font_size = print != null && print.font_size > 0 ? print.font_size : (int)(default_font.Size + .5);
            Font f = fonts_.get_font(font_name, font_size, print.bold, print.italic, print.underline);
            return f;
        }

        public Font default_font {
            get { return parent_.list.Font; }
        }

        // allow overriding the parent  
        public void set_parent(log_view parent) {
            Debug.Assert(parent != null);
            if (parent == parent_)
                return;

            parent_ = parent;
        }

        private int measure_text_width(Graphics g, string text, Font override_font ) {
            // IMPORTANT: at this time, we assume we have a fixed font
            return (int)(g.MeasureString(text, override_font ?? default_font).Width + .7);
        }

        public int text_width(Graphics g, string text, Font override_font = null) {
            // IMPORTANT: at this time, we assume we have a fixed font
            bool ends_in_space = text.EndsWith(" ");
            int width = measure_text_width(g, text, override_font);
            if (ends_in_space) {
                // we need to find out the size of "_" - only reliable way is to print it twice
                int width1 = measure_text_width(g, text + "_", override_font);
                int width2 = measure_text_width(g, text + "__", override_font);
                int underscore_size = width2 - width1;
                width = width1 - underscore_size;
            }

            return width;
        }

        // 1.7.12+ a few pixels are simply ignored when starting to draw. However, when drawing one thing after another, we don't want gaps
        //         now I compute this offset correctly regardless of font
        private const string dummy_extra_ = "ABCDEFGZYX";
        public int text_offset(Graphics g, string text, Font override_font = null) {
            // IMPORTANT: at this time, we assume we have a fixed font
            bool ends_in_space = text.EndsWith(" ");
            int width = measure_text_width(g, text, override_font);
            if (ends_in_space) {
                // we need to find out the size of "_" - only reliable way is to print it twice
                int width1 = measure_text_width(g, text + "_", override_font);
                int width2 = measure_text_width(g, text + "__", override_font);
                int underscore_size = width2 - width1;
                width = width1 - underscore_size;
            }

            int text_and_extra_width = measure_text_width(g, text + dummy_extra_, override_font);
            int extra_width = measure_text_width(g, dummy_extra_, override_font);

            // normally, this should be zero, but always, the OS pads a few pixels - we need to ignore them
            int offset = width + extra_width - text_and_extra_width;
            return width - offset;
        }

        public int char_size(Graphics g) {
            // IMPORTANT: at this time, we assume we have a fixed font
            var ab = g.MeasureString("ab", default_font).Width;
            var abc = g.MeasureString("abc", default_font).Width;
            var abcd = g.MeasureString("abcd", default_font).Width;
            Debug.Assert( (int)((abc - ab) * 100) == (int)((abcd - abc) * 100)) ;

            return (int)(abc - ab);
        }

        public Color print_bg_color(OLVListItem item, text_part print) {
            match_item i = item.RowObject as match_item;            
            bool is_sel = !ignore_selection ? parent_.multi_sel_idx.Contains(item.Index) : false;

            Color default_bg = is_sel ? i.sel_bg(parent_) : i.bg(parent_);            
            Color bg = print.bg != util.transparent ? print.bg : default_bg;
            if (bg == util.transparent)
                bg = app.inst.bg;
            // selection overrides everything
            if (is_sel)
                bg = default_bg;

            if (print.is_typed_search && !print.is_find_search)
                bg = util.darker_color(bg);
            return bg;
        }


        public Color print_fg_color(OLVListItem item, text_part print) {
            match_item i = item.RowObject as match_item;            
            Color fg = i.fg(parent_);
            Color print_fg = print.fg != util.transparent ? print.fg : fg;
            if (print_fg == util.transparent)
                print_fg = app.inst.fg;
            return print_fg;
        }


        public Color bg_color(OLVListItem item, int col_idx) {
            match_item i = item.RowObject as match_item;
            int row_idx = item.Index;

            Color color;
            bool is_sel = !ignore_selection ? parent_.multi_sel_idx.Contains(row_idx) : false;

            Color bg = i.bg(parent_);
            Color dark_bg = i.sel_bg(parent_);

            if (col_idx == parent_.msgCol.fixed_index()) {
                if (is_sel) 
                    color = is_sel ? dark_bg : bg;
                else if (app.inst.use_bg_gradient) {
                    Rectangle r = item.GetSubItemBounds(col_idx);
                    if (r.Width > 0 && r.Height > 0)
                        // it's a gradient
                        color = util.transparent;
                    else
                        color = bg;
                } else
                    color = bg;
            } else 
                color = is_sel ? dark_bg : bg;

            if (color == util.transparent)
                color = app.inst.bg;
            return color;
        }

        public Color bg_color(OLVListItem item, int col_idx, formatted_text format) {
            match_item i = item.RowObject as match_item;
            int row_idx = item.Index;
            bool is_sel = !ignore_selection ? parent_.multi_sel_idx.Contains(row_idx) : false;
            if ( !is_sel)
                if (format.bg != util.transparent)
                    return format.bg;

            return bg_color(item, col_idx);
        }
    }
}
