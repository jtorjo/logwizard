using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net.Repository.Hierarchy;

namespace lw_common.ui {
    public partial class smart_readonly_textbox : RichTextBox {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private log_view parent_;
        private int sel_start_ = -1, sel_len_ = -1, sel_col_ = -1;

        private bool changed_sel_background_ = false;

        private int ignore_change_ = 0;

        public smart_readonly_textbox() {
            InitializeComponent();
            BorderStyle = BorderStyle.None;

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
            bool is_multi_selection = parent_.list.SelectedIndices != null && parent_.list.SelectedIndices.Count > 1;
            Visible = sel >= 0 && bounds.Width > 0 && bounds.Height > 0 && parent_.is_editing && !is_multi_selection;
            if (!Visible) 
                return;

            int offset_x = parent_.list.Left;
            int offset_y = parent_.list.Top;
            var location = new Point(offset_x + bounds.X, offset_y + bounds.Y + 2);
            if (location.Y + bounds.Height > parent_.Height) {
                // it was the last row when user is moving down (down arrow) - we'll get notified again
                Visible = false;
                return;
            }
            int header_height = parent_.list.HeaderControl.ClientRectangle.Height;
            if (location.Y < offset_y + header_height) {
                // it was the first row when user is moving up (up arrow), we'll get notified again
                Visible = false;
                return;
            }

            Location = location;
            Size = new Size(bounds.Width, bounds.Height);
            Font = parent_.list.Font;

            log_view.item i = parent_.sel;
            ForeColor = i.fg(parent_);
            BackColor = i.sel_bg(parent_);

            Text = parent_.sel_subitem_text;
            SelectionBackColor = util.darker_color(BackColor);
            SelectionColor = util.darker_color(ForeColor);
            changed_sel_background_ = false;

            if (sel_start_ >= 0 && sel_start_ <= TextLength)
                SelectionStart = sel_start_;
            else if (TextLength > 0) {
                // our selection is bigger than what we have in the current cell
                SelectionStart = TextLength;
                return;
            }

            if (sel_start_ >= 0 && sel_start_ <= TextLength)
                if (sel_len_ >= 0 && sel_len_ + sel_start_ <= TextLength)
                    SelectionLength = sel_len_;
        }

        public void go_to_char(int char_idx) {
            if (char_idx <= TextLength) {
                SelectionStart = char_idx;
                SelectionLength = 0;
                sel_col_ = parent_.sel_col;
                sel_start_ = SelectionStart;
                sel_len_ = 0;
            }    
        }

        public void force_update_sel() {
            sel_col_ = parent_.sel_col;
            sel_start_ = SelectionStart;
            sel_len_ = SelectionLength;            
        }

        public void update_sel() {
            logger.Debug("[smart] sel =" + parent_.sel_row_idx + "," + parent_.sel_col + " [" + SelectionStart + "," + SelectionLength + "]");

            if (sel_col_ == parent_.sel_col) {
                // at this point - see if we already have a selection higher than what we selected now
                if (sel_start_ <= TextLength) {
                    sel_start_ = SelectionStart;
                    sel_len_ = SelectionLength;
                } else {
                    // we're bigger than current row
                    SelectionStart = TextLength;
                    SelectionLength = 0;
                }
            } else {
                sel_col_ = parent_.sel_col;
                sel_start_ = SelectionStart;
                sel_len_ = SelectionLength;
            }
        }

        private void smart_readonly_textbox_SelectionChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;

            if (SelectionLength > 0) {
                ++ignore_change_;
                SelectionBackColor = util.darker_color(BackColor);
                SelectionColor = util.darker_color(ForeColor);
                string txt = SelectedText;
                SelectedText = txt;
                changed_sel_background_ = true;
                --ignore_change_;
            }
            else if (changed_sel_background_) 
                util.postpone(check_empty_sel, 10);
        }

        private void check_empty_sel() {
            if (!changed_sel_background_)
                return;

            if (SelectedText == "") {
                // the idea is that we need to re-add the whole text, otherwise, the former selection will remain
                changed_sel_background_ = false;
                int sel = SelectionStart;
                string txt = Text;
                Text = txt;
                SelectionStart = sel;
            }
        }

    }
}
