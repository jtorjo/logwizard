using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using lw_common;

namespace LogWizard.ui {
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
        }

        private Dictionary<string, print_info> override_print_ = new Dictionary<string, print_info>();
        print_info default_ = new print_info();

        public void set_override(string txt, print_info print) {
            if (override_print_.ContainsKey(txt))
                override_print_.Remove(txt);

            override_print_.Add(txt, print);
        }

        private int draw_sub_string(int left, string sub, Graphics g, Brush b, Rectangle r, StringFormat fmt, print_info print) {
            Font f = print.bold ? (print.italic ? bi_font : b_font) : (print.italic ? i_font : font);
            var i = ListItem.RowObject as log_view.item;
            if (print != default_) {
                Color bg = i.bg(parent_);
                bg = print.bg != util.transparent ? print.bg : util.darker_color(bg);
                Rectangle here = new Rectangle(r.Location, r.Size);
                int cur_width = (int)g.MeasureString(sub, f).Width + 1;
                here.X += left;
                here.Width = cur_width;
                g.FillRectangle(brush_.brush(bg), here);
            }

            Color fg = i.fg(parent_);
            Brush brush = print.fg != util.transparent ? brush_.brush(print.fg) : brush_.brush( fg);

            Rectangle sub_r = new Rectangle(r.Location, r.Size);
            sub_r.X += left;
            sub_r.Width -= left;
            int width = (int)g.MeasureString(sub, f).Width + 1;
            g.DrawString(sub, f, brush, sub_r, fmt);
            return left + width;
        }


        private void draw_string(int left, string s, Graphics g, Brush b, Rectangle r, StringFormat fmt) {
            if ( override_print_.Count < 1) {
                // no overrides at all
                draw_sub_string(left, s, g, b, r, fmt, default_);
                return;
            }

            int least = override_print_.Keys.Min(op => {
                int idx = s.IndexOf(op);
                return idx != -1 ? idx : int.MaxValue;
            });

            if (least == int.MaxValue) {
                // nothing to override
                draw_sub_string(left, s, g, b, r, fmt, default_);
                return;                
            }

            // here, we have at least one override
            foreach (var op in override_print_) {
                int idx = s.IndexOf(op.Key);
                if (idx == least) {
                    int next = draw_sub_string(left, s.Substring(0, idx), g, b, r, fmt, default_);
                    int next2 = draw_sub_string(next, s.Substring(idx, op.Key.Length), g, b, r, fmt, op.Value);
                    draw_string(next2, s.Substring(idx + op.Key.Length), g, b, r, fmt);
                    return;
                }
            }
        }

        public override void Render(Graphics g, Rectangle r) {
            var i = ListItem.RowObject as log_view.item;
            if (i == null)
                return;

            Color bg = i.bg(parent_);
            bool is_focused = win32.focused_ctrl() == parent_.list;

            Color dark_bg = util.darker_color(bg);
            Color darker_bg = util.darker_color(dark_bg);
            bool is_sel = IsItemSelected;

            var col_idx = Column.Index;

            Brush brush;
            if (col_idx == parent_.msgCol.Index) {
                if (is_sel) 
                    brush = brush_.brush(is_sel ? (is_focused ? darker_bg : dark_bg) : bg);
                else if (app.inst.use_bg_gradient)
                    brush = gradient_.brush(r, app.inst.bg_from, app.inst.bg_to);
                else
                    brush = brush_.brush(bg);
            } else 
                brush = brush_.brush(is_sel ? (is_focused ? darker_bg : dark_bg) : bg);
            g.FillRectangle(brush, r);

            StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap);
            fmt.LineAlignment = StringAlignment.Center;
            fmt.Trimming = StringTrimming.EllipsisCharacter;
            switch (this.Column.TextAlign) {
                case HorizontalAlignment.Center: fmt.Alignment = StringAlignment.Center; break;
                case HorizontalAlignment.Left: fmt.Alignment = StringAlignment.Near; break;
                case HorizontalAlignment.Right: fmt.Alignment = StringAlignment.Far; break;
            }

            //g.DrawString(this.GetText(), this.Font, this.TextBrush, r, fmt);
            draw_string(0, GetText(), g, brush, r, fmt);
        }
    }
}
