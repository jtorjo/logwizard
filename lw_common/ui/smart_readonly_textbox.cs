using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class smart_readonly_textbox : TextBox {
        private log_view parent_;

        public smart_readonly_textbox() {
            InitializeComponent();
            // initially, until initialized,should be hidden
            util.postpone(() => Left = -200, 1);
        }

        public void init(log_view parent) {
            parent_ = parent;
            util.postpone(update_ui, 10);
        }

        public void update_ui() {
            if (parent_ == null)
                return;

            int sel = parent_.sel_row_idx;
            var bounds = parent_.sel_subrect_bounds;
            Visible = sel >= 0 && bounds.Width > 0 && bounds.Height > 0;
            if (!Visible) 
                return;

            int offset_x = parent_.list.Left;
            int offset_y = parent_.list.Top;
            var location = new Point(offset_x + bounds.X, offset_y + bounds.Y);
            if (location.Y + bounds.Height > parent_.Height)
                // it was the last row when user is moving down (down arrow) - we'll get notified again
                return;
            int header_height = parent_.list.HeaderControl.ClientRectangle.Height;
            if (location.Y < offset_y + header_height)
                // it was the first row when user is moving up (up arrow), we'll get notified again
                return;


            Location = location;
            Size = new Size(bounds.Width, bounds.Height);
            Text = parent_.sel_subitem_text;
            Font = parent_.list.Font;

            log_view.item i = parent_.sel;
            ForeColor = i.fg(parent_);
            BackColor = i.sel_bg(parent_);
        }

        public void go_to_char(int char_idx) {
            if (char_idx <= TextLength) {
                SelectionStart = char_idx;
                SelectionLength = 0;
            }    
        }

    }
}
