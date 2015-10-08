using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;

namespace lw_common.ui {
    internal class log_view_item_draw_ui {
        private solid_brush_list brush_ = new solid_brush_list();
        private gradient_brush_list gradient_ = new gradient_brush_list();

        private log_view parent_;

        private Font font_, b_font, bi_font, i_font;


        public log_view_item_draw_ui(log_view parent) {
            parent_ = parent;

            font_ = parent_.list.Font;
            b_font = new Font(font_.FontFamily, font_.Size, FontStyle.Bold);
            bi_font = new Font(font_.FontFamily, font_.Size, FontStyle.Bold | FontStyle.Italic);
            i_font = new Font(font_.FontFamily, font_.Size, FontStyle.Italic);
        }

        private void build_fonts() {
            b_font = new Font(font_.FontFamily, font_.Size, FontStyle.Bold);
            bi_font = new Font(font_.FontFamily, font_.Size, FontStyle.Bold | FontStyle.Italic);
            i_font = new Font(font_.FontFamily, font_.Size, FontStyle.Italic);            
        }

        public void set_font(Font f) {
            font_ = f;
            build_fonts();
        }
        public Font font(print_info print) {
            Font f = print.bold ? (print.italic ? bi_font : b_font) : (print.italic ? i_font : font_);
            return f;
        }

        public int text_width(Graphics g, string text) {
            // IMPORTANT: at this time, we assume we have a fixed font
            bool ends_in_space = text.EndsWith(" ");
            if (ends_in_space)
                // just append any character, so that the spaces are taken into account
                text += "_";

            int width = (int)g.MeasureString(text, font_).Width + 1;
            if (ends_in_space) {
                int avg_per_char = width / text.Length;
                width -= avg_per_char;
            }
            return width;
//            return char_size(g) * text.Length;
        }


        public int char_size(Graphics g) {
            // IMPORTANT: at this time, we assume we have a fixed font
            var ab = g.MeasureString("ab", font_).Width;
            var abc = g.MeasureString("abc", font_).Width;
            var abcd = g.MeasureString("abcd", font_).Width;
            Debug.Assert( (int)((abc - ab) * 100) == (int)((abcd - abc) * 100)) ;

            return (int)(abc - ab);
        }


        public Brush bg_brush(OLVListItem item, int col_idx) {
            log_view.item i = item.RowObject as log_view.item;
            int row_idx = item.Index;

            Brush brush;
            bool is_sel = parent_.multi_sel_idx.Contains(row_idx);

            Color bg = i.bg(parent_);
            Color dark_bg = i.sel_bg(parent_);

            if (col_idx == parent_.msgCol.Index) {
                if (is_sel) 
                    brush = brush_.brush(is_sel ? dark_bg : bg);
                else if (app.inst.use_bg_gradient) {
                    Rectangle r = item.GetSubItemBounds(col_idx);
                    if ( r.Width > 0 && r.Height > 0)
                        brush = gradient_.brush(r, app.inst.bg_from, app.inst.bg_to);
                    else 
                        brush = brush_.brush(bg);
                } else
                    brush = brush_.brush(bg);
            } else 
                brush = brush_.brush(is_sel ? dark_bg : bg);

            return brush;
        }
    }
}
