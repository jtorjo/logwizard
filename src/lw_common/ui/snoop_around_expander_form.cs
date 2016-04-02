using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lw_common.ui
{
    // when hovered - it shows filter - when moving away from the form, doesn't show filter
    internal partial class snoop_around_expander_form : Form {
        
        // if true, we show the filter as well (the user can press it to enable/disable the last filter)
        private bool show_filter_ = false;

        private snoop_around_form parent_;

        private int control_width_, control_height_;

        public snoop_around_expander_form(snoop_around_form parent)
        {
            parent_ = parent;
            InitializeComponent();
            control_width_ = expand.Width;
            control_height_ = expand.Height;
            TopMost = true;
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

        protected override bool ShowWithoutActivation { get { return true; } } // stops the window from stealing focus
        const int WS_EX_NOACTIVATE = 0x08000000;
        protected override CreateParams CreateParams {
            get {
                var Params = base.CreateParams;
                Params.ExStyle |= WS_EX_NOACTIVATE ;
                return Params;                
            }
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
            Opacity = parent_.expanded ? 1 : 0.2;
            Width = show_filter ? control_width_ * 2 : control_width_;
            Height = control_height_;

            // note: not using the parent form's position, since it might be hidden
            Rectangle parent_rect = parent_.screen_logical_parent_rect;
            Point low_right = new Point(parent_rect.Right, parent_rect.Bottom);
            Point top_left = new Point(low_right.X - width, low_right.Y - height);
            Location = top_left;
            Size = new Size(width, height);
        }

        private void reapply_CheckedChanged(object sender, EventArgs e)
        {
            parent_.on_click_apply();
        }

        private void expand_Click(object sender, EventArgs e)
        {
            parent_.on_click_expand();
        }
    }
}
