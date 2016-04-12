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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using lw_common.Properties;

namespace lw_common.ui.snoop {
    // opacity - taken from http://stackoverflow.com/questions/9358500/making-a-control-transparent
    //           however, it was useless, since it would not apply to my buttons
    internal partial class snoop_around_expander_ctrl : UserControl {
        // if true, we show the filter as well (the user can press it to enable/disable the last filter)
        private bool show_filter_ = false;

        private snoop_around_form parent_;
        
        private int control_width_, control_height_;

        private bool reapply_ = false;

        private Bitmap filter_applied = Resources.filter_applied;
        private Bitmap filter_not_applied = Resources.filter_not_applied;

        private bool prev_close_by_vertically_ = true;

        public snoop_around_expander_ctrl(snoop_around_form parent) {
            parent_ = parent;
            InitializeComponent();
            
            // don't ever steal focus http://stackoverflow.com/questions/785672/is-there-a-way-to-make-a-usercontrol-unfocussable
            SetStyle(ControlStyles.Selectable, false);

            control_width_ = expand.Width;
            control_height_ = expand.Height;

            BackColor = Color.White;
            reapplyFilter.BackgroundImage = reapply_ ? filter_applied : filter_not_applied;
            update_pos();
        }

        public bool show_filter {
            get { return show_filter_; }
            set {
                show_filter_ = value;
                update_pos();
                update_tooltips();
            }
        }

        // if true, filter is ON
        public bool filter_pressed {
            get { return reapply_; }
            set {
                if (reapply_ != value) {
                    reapply_ = value;
                    reapplyFilter.BackgroundImage = reapply_ ? filter_applied : filter_not_applied;
                    update_tooltips();
                    parent_.on_click_apply();
                }
            }
        }

        private void update_tooltips() {
            tip.SetToolTip(reapplyFilter, reapply_ ? "Filter is Running\r\nClick to Disable" : "Filter is NOT Running\r\nClick to Enable");
            tip.SetToolTip(expand, reapply_ ? "Filter is Running\r\nClick to Refine and/or Re-apply your Filter" : "Click to Filter (Snoop Around)");
        }

        public void update_pos() {
            bool show_filter_button = show_filter_ && is_close_by_vertically();
            int width = show_filter_button ? control_width_ * 2 : control_width_;
            int height = control_width_;

            if (show_filter_button) {
                reapplyFilter.Visible = true;
                reapplyFilter.Location = new Point(0, 0);
                expand.Location = new Point(control_width_, 0);
            } else {
                reapplyFilter.Visible = false;
                expand.Location = new Point(0,0);
            }
            // forget it for now
            //Opacity = parent_.expanded ? 100 : 20;
            Width = show_filter_button ? control_width_ * 2 : control_width_;
            Height = control_height_;

            // note: not using the parent form's position, since it might be hidden
            Rectangle parent_rect = parent_.logical_parent_rect;
            Point low_right = new Point(parent_rect.Right, parent_rect.Bottom);
            Point top_left = new Point(low_right.X - width, low_right.Y - height);
            Location = top_left;
            Size = new Size(width, height);
        }

        private bool is_close_by_vertically() {
            var screen_rect = RectangleToScreen(ClientRectangle);
            var mouse = Cursor.Position;
            int PAD = 15;
            bool inside_vertically = screen_rect.Top <= mouse.Y && screen_rect.Bottom >= mouse.Y;
            bool close_by_vertically = inside_vertically || Math.Abs(screen_rect.Top - mouse.Y) < PAD || Math.Abs(screen_rect.Bottom - mouse.Y) < PAD;
            return close_by_vertically;
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
            }
        }

        private void expand_Click_1(object sender, EventArgs e) {
            parent_.on_click_expand();
        }

        private void reapplyFilter_Click(object sender, EventArgs e) {
            filter_pressed = !filter_pressed;
        }

        private void refreshIcons_Tick(object sender, EventArgs e) {
            bool close_by_vertically = is_close_by_vertically();
            var expand_icon = close_by_vertically ? Resources.down_applied : Resources.down;
            if (close_by_vertically != prev_close_by_vertically_) {
                expand.BackgroundImage = expand_icon;
                update_pos();
                update_tooltips();
            }
            prev_close_by_vertically_ = close_by_vertically;
        }
    }
}
