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

namespace lw_common.ui {
    class log_view_render : BaseRenderer {

        private solid_brush_list brush_ = new solid_brush_list();
        private gradient_brush_list gradient_ = new gradient_brush_list();

        private Font font, b_font, bi_font, i_font;

        private log_view parent_;
        public log_view_render(log_view parent) {
            parent_ = parent;
            
            font = parent_.list.Font;
            b_font = new Font(font.FontFamily, font.Size, FontStyle.Bold);
            bi_font = new Font(font.FontFamily, font.Size, FontStyle.Bold | FontStyle.Italic);
            i_font = new Font(font.FontFamily, font.Size, FontStyle.Italic);
        }

        private void build_fonts() {
            b_font = new Font(font.FontFamily, font.Size, FontStyle.Bold);
            bi_font = new Font(font.FontFamily, font.Size, FontStyle.Bold | FontStyle.Italic);
            i_font = new Font(font.FontFamily, font.Size, FontStyle.Italic);            
        }

        public void set_font(Font f) {
            font = f;
            build_fonts();
        }

        /*
        public override bool OptionalRender(Graphics g, Rectangle r) {
            if ( SubItem == null)
                gradient(g,r);
            return false;
        }*/

        public class print_info {
            protected bool Equals(print_info other) {
                return fg.Equals(other.fg) && bg.Equals(other.bg) && bold == other.bold && italic == other.italic && string.Equals(font_name, other.font_name);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((print_info) obj);
            }

            public override int GetHashCode() {
                unchecked {
                    var hashCode = fg.GetHashCode();
                    hashCode = (hashCode * 397) ^ bg.GetHashCode();
                    hashCode = (hashCode * 397) ^ bold.GetHashCode();
                    hashCode = (hashCode * 397) ^ italic.GetHashCode();
                    hashCode = (hashCode * 397) ^ (font_name != null ? font_name.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public static bool operator ==(print_info left, print_info right) {
                return Equals(left, right);
            }

            public static bool operator !=(print_info left, print_info right) {
                return !Equals(left, right);
            }

            public Color fg = util.transparent;
            public Color bg = util.transparent;
            public bool bold = false, italic = false;
            public string font_name = "";
            
            // useful when sorting - to avoid collisions
            public string text = "";
        }

        private List<Tuple<int, int, log_view_render.print_info>> override_print_ = new List<Tuple<int, int, log_view_render.print_info>> ();
        print_info default_ = new print_info();

        private void draw_sub_string(int left, string sub, Graphics g, Brush b, Rectangle r, StringFormat fmt, print_info print) {
            Font f = print.bold ? (print.italic ? bi_font : b_font) : (print.italic ? i_font : font);
            var i = ListItem.RowObject as log_view.item;
            int width = text_width(g, sub);
            if (print != default_) {
                Color bg = print.bg != util.transparent ? print.bg : util.darker_color( i.bg(parent_) );
                Rectangle here = new Rectangle(r.Location, r.Size);
                here.X += left;
                here.Width = width + 1;
                g.FillRectangle(brush_.brush(bg), here);
            }

            Color fg = i.fg(parent_);
            Brush brush = print.fg != util.transparent ? brush_.brush(print.fg) : brush_.brush( fg);

            Rectangle sub_r = new Rectangle(r.Location, r.Size);
            sub_r.X += left;
            sub_r.Width -= left;
            g.DrawString(sub, f, brush, sub_r, fmt);
        }


        private void draw_string(int left, string s, Graphics g, Brush b, Rectangle r, StringFormat fmt) {
            if ( override_print_.Count < 1) {
                // no overrides at all
                draw_sub_string(left, s, g, b, r, fmt, default_);
                return;
            }

            // here, we have at least one override
            for (int idx = 0; idx < override_print_.Count; ++idx) {
                int start_normal = idx > 0 ? override_print_[idx - 1].Item1 + override_print_[idx - 1].Item2 : 0;
                int normal_len = override_print_[idx].Item1 - start_normal;

                string up_to_prev = s.Substring(0, start_normal);
                string up_to_now = s.Substring(0, override_print_[idx].Item1);
                int left_normal = left + text_width(g, up_to_prev);
                int left2 = left + text_width(g, up_to_now);

                // first, draw the normal text
                draw_sub_string(left_normal, s.Substring(start_normal, normal_len), g, b, r, fmt, default_);
                draw_sub_string(left2, s.Substring( override_print_[idx].Item1, override_print_[idx].Item2 ), g, b, r, fmt, override_print_[idx].Item3);
            }

            var last_override = override_print_.Last();
            int last = last_override.Item1 + last_override.Item2;
            string last_normal = s.Substring(last);
            if (last_normal != "") {
                string up_to_now = s.Substring(0, last_override.Item1 + last_override.Item2);
                int last_left = left + text_width(g, up_to_now);
                draw_sub_string(last_left, last_normal, g, b, r, fmt, default_);
            }
        }

        private int text_width(Graphics g, string text) {
            // IMPORTANT: at this time, we assume we have a fixed font
            bool ends_in_space = text.EndsWith(" ");
            if (ends_in_space)
                // just append any character, so that the spaces are taken into account
                text += "_";

            int width = (int)g.MeasureString(text, font).Width + 1;
            if (ends_in_space) {
                int avg_per_char = width / text.Length;
                width -= avg_per_char;
            }
            return width;
//            return char_size(g) * text.Length;
        }


        private int char_size(Graphics g) {
            // IMPORTANT: at this time, we assume we have a fixed font
            var ab = g.MeasureString("ab", font).Width;
            var abc = g.MeasureString("abc", font).Width;
            var abcd = g.MeasureString("abcd", font).Width;
            Debug.Assert( (int)((abc - ab) * 100) == (int)((abcd - abc) * 100)) ;

            return (int)(abc - ab);
        }

        // for each character of the printed text, see how many pixels it takes
        public List<int> text_widths(Graphics g ,string text) {
            var char_size = this.char_size(g);
            List<int> widths = new List<int>();
            for ( int i = 0; i < text.Length; ++i)
                widths.Add((int)(char_size * i));
            return widths;
        } 

        public override void Render(Graphics g, Rectangle r) {
            var i = ListItem.RowObject as log_view.item;
            if (i == null)
                return;

            var col_idx = Column.Index;
            string text = GetText();
            override_print_ = i.override_print(parent_, text, col_idx);
            Color bg = i.bg(parent_);
            Color dark_bg = i.sel_bg(parent_);
            bool is_sel = IsItemSelected;


            Brush brush;
            if (col_idx == parent_.msgCol.Index) {
                if (is_sel) 
                    brush = brush_.brush(is_sel ? dark_bg : bg);
                else if (app.inst.use_bg_gradient)
                    brush = gradient_.brush(r, app.inst.bg_from, app.inst.bg_to);
                else
                    brush = brush_.brush(bg);
            } else 
                brush = brush_.brush(is_sel ? dark_bg : bg);
            g.FillRectangle(brush, r);

            StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap);
            fmt.LineAlignment = StringAlignment.Center;
            fmt.Trimming = StringTrimming.EllipsisCharacter;
            switch (this.Column.TextAlign) {
                case HorizontalAlignment.Center: fmt.Alignment = StringAlignment.Center; break;
                case HorizontalAlignment.Left: fmt.Alignment = StringAlignment.Near; break;
                case HorizontalAlignment.Right: fmt.Alignment = StringAlignment.Far; break;
            }

            draw_string(0, text, g, brush, r, fmt);
        }
    }
}
