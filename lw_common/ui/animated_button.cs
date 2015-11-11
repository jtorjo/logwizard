using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class animated_button : Button {
        private int animate_count_ = 5;

        private int animate_idx_ = 0;
        private int animate_char_idx_ = 0;

        private bool wait_for_next_animate_ = false;

        private font_list fonts_ = new font_list();
        private solid_brush_list brushes_ = new  solid_brush_list();

        public animated_button() {
            InitializeComponent();
            animate_interval_ms = util.is_debug ? 5000 : 30000;
            animate = false;
        }

        public bool animate {
            get {
                if (drawNextChar == null)
                    // in design mode
                    return false;
                return drawNextChar.Enabled;
            }
            set {
                animate_char_idx_ = 0;
                animate_idx_ = 0;
                wait_for_next_animate_ = false;
                drawNextChar.Enabled = value;
                drawNextAnimation.Enabled = value;
            }
        }

        public int animate_count {
            get { return animate_count_; }
            set { animate_count_ = value; }
        }

        public int animate_interval_ms {
            get { return drawNextAnimation.Interval; }
            set { drawNextAnimation.Interval = value; }
        }

        public int animate_speed_ms {
            get { return drawNextChar.Interval; }
            set { drawNextChar.Interval = value; }
        }

        protected override void OnPaint(PaintEventArgs pevent) {
            if (animate && !wait_for_next_animate_) {
                Graphics g = pevent.Graphics;
                var sub = new[] { Text.Substring(0,animate_char_idx_), Text.Substring(animate_char_idx_, 1), Text.Substring(animate_char_idx_ + 1) }.ToList();
                List<int> widths = new List<int>();
                int width_so_far = 0;
                string up_to_now = "";
                foreach (string s in sub) {
                    up_to_now += s;
                    var width_now = (int) g.MeasureString(up_to_now.Replace(" ","_"), fonts_.get_font(Font, false, false, false)).Width;
                    widths.Add(width_now - width_so_far);
                    width_so_far = width_now;
                }
                int height = (int)g.MeasureString(Text, Font).Height;
                int offset_x = (Width - widths.Sum()) / 2;
                int offset_y = (Height - height) / 2;

                ControlPaint.DrawButton(g, ClientRectangle, ButtonState.Flat);
                bool bold = false;
                int start = offset_x;
                for (int i = 0; i < sub.Count; i++) {
                    string s = sub[i];
                    g.DrawString(s, fonts_.get_font(Font, false, false, false), brushes_.brush(bold ? Color.Red : ForeColor), start, offset_y);
                    start += widths[i];
                    bold = !bold;
                }
            }
            else
                base.OnPaint(pevent);
        }

        private void drawNext_Tick(object sender, EventArgs e) {
            if (wait_for_next_animate_)
                return;

            animate_char_idx_ = (animate_char_idx_ + 1) % Text.Length;
            if ( Text.Length > 0)
                while ( Text[animate_char_idx_] == ' ')
                    animate_char_idx_ = (animate_char_idx_ + 1) % Text.Length;

            if (animate_char_idx_ == Text.Length - 1) 
                wait_for_next_animate_ = true;
            Invalidate();
            Update();
        }

        private void drawNextAnimation_Tick(object sender, EventArgs e) {
            if (++animate_idx_ >= animate_count_)
                animate = false;
            else
                wait_for_next_animate_ = false;

            Invalidate();
            Update();
        }
    }
}
