using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lw_common.ui.snoop {
    // opacity - taken from http://stackoverflow.com/questions/9358500/making-a-control-transparent
    //           however, it was useless, since it would not apply to my buttons
    internal partial class snoop_around_expander_ctrl : UserControl {
        // if true, we show the filter as well (the user can press it to enable/disable the last filter)
        private bool show_filter_ = false;

        private snoop_around_form parent_;
        
        private int control_width_, control_height_;

        public snoop_around_expander_ctrl(snoop_around_form parent) {
            parent_ = parent;
            InitializeComponent();
            control_width_ = expand.Width;
            control_height_ = expand.Height;

            BackColor = Color.White;

            update_pos();
        }

        public bool show_filter {
            get { return show_filter_; }
            set {
                show_filter_ = value;
                update_pos();
            }
        }

        // if true, filter is ON
        public bool filter_pressed {
            get { return reapply.Checked; }
            set {
                reapply.Checked = value; 
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            if (this.Parent != null)
                Parent.Invalidate(this.Bounds, true);
            base.OnBackColorChanged(e);
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnParentBackColorChanged(e);
        }

        public void update_pos() {
            int width = show_filter ? control_width_ * 2 : control_width_;
            int height = control_width_;

            if (show_filter) {
                reapply.Visible = true;
                reapply.Location = new Point(0, 0);
                expand.Location = new Point(control_width_, 0);
            } else {
                reapply.Visible = false;
                expand.Location = new Point(0,0);
            }
            // forget it for now
            //Opacity = parent_.expanded ? 100 : 20;
            Width = show_filter ? control_width_ * 2 : control_width_;
            Height = control_height_;

            // note: not using the parent form's position, since it might be hidden
            Rectangle parent_rect = parent_.logical_parent_rect;
            Point low_right = new Point(parent_rect.Right, parent_rect.Bottom);
            Point top_left = new Point(low_right.X - width, low_right.Y - height);
            Location = top_left;
            Size = new Size(width, height);
        }

        private void reapply_CheckedChanged(object sender, EventArgs e) {
            parent_.on_click_apply();
        }

        private void expand_Click(object sender, EventArgs e) {
            parent_.on_click_expand();
        }

        private void snoop_around_expander_ctrl_VisibleChanged(object sender, EventArgs e) {
        }

        private void refreshZorder_Tick(object sender, EventArgs e) {
            if (Visible && Parent != null) {
                // note: we need to allow several such controls to exist, and not try to take over the zorder from one another
                var other_expanders_zorder = Parent.Controls.OfType<snoop_around_expander_ctrl>()
                    .Where(x => x != this).Select(x => Parent.Controls.GetChildIndex(x)).ToList();
                int zorder = 0;
                while (other_expanders_zorder.Contains(zorder))
                    ++zorder;

                if ( Parent.Controls.GetChildIndex(this) != zorder)
                    Parent.Controls.SetChildIndex(this, zorder);

                // the issue was because i had created the control transparently, cause i wanted opacity :D
#if old_code
                var screen_rect = RectangleToScreen(ClientRectangle);
                var intersecting_controls = Parent.Controls.OfType<TextBoxBase>().Where(x => x.Visible).Where(x => x.RectangleToScreen(x.ClientRectangle).IntersectsWith(screen_rect) );
                if (intersecting_controls.Any()) {
                    var rect = Parent.RectangleToClient(screen_rect);
                    foreach (Control c in intersecting_controls) {
                        var control_rect = new Rectangle(c.Location, c.Size);
                        if (control_rect.Right > rect.Left && control_rect.Right < rect.Right)
                            // control overlaps with our left side
                            c.Width -= control_rect.Right - rect.Left;
                        else if (control_rect.Left < rect.Right && control_rect.Left > rect.Left)
                            // control overlaps with our right side
                            c.Location = new Point(rect.Right, control_rect.Top);
                        else if (control_rect.Left <= rect.Left && control_rect.Right >= rect.Right)
                            // the other control overlaps totally - we can safey assume this is on the right side
                            c.Width = rect.Left - control_rect.Left;
                    }
                }
#endif
            }
        }
    }
}
