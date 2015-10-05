using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net.Repository.Hierarchy;

namespace lw_common.ui {
    public partial class smart_readonly_textbox : RichTextBox {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int ON_CLICK_WAIT_BEFORE_SELECTING_WORD_MS = 450;

        private log_view parent_;
        private int sel_start_ = -1, sel_len_ = -1, sel_col_ = -1;

        private int after_click_sel_start_ = -1, after_click_sel_len_ = -1, after_click_sel_col_ = -1;

        private bool changed_sel_background_ = false;

        private int ignore_change_ = 0;

        public delegate void void_func();

        public delegate void search_func(string text);

        // called when selection has changed
        public void_func on_sel_changed;

        public search_func on_search_ahead;

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

            var old_sel = sel_text;

            ++ignore_change_;

            Location = location;
            Size = new Size(bounds.Width, bounds.Height);
            Font = parent_.list.Font;

            log_view.item i = parent_.sel;
            ForeColor = i.fg(parent_);
            BackColor = i.sel_bg(parent_);

            set_text( parent_.sel_subitem_text, false);
            SelectionBackColor = BackColor;
            SelectionColor = ForeColor;
            changed_sel_background_ = false;

            if (sel_start_ >= 0 && sel_start_ <= TextLength)
                SelectionStart = sel_start_;
            else if (TextLength > 0) {
                // our selection is bigger than what we have in the current cell
                SelectionStart = TextLength;
            }

            if (sel_start_ >= 0 && sel_start_ <= TextLength)
                if (sel_len_ >= 0 && sel_len_ + sel_start_ <= TextLength)
                    SelectionLength = sel_len_;

            --ignore_change_;

            if (old_sel != sel_text)
                on_sel_changed();
        }

        private void set_text(string txt, bool force) {
            if (Text == txt && !force)
                return;
            ++ignore_change_;
            Clear();
            AppendText(txt);
            --ignore_change_;
        }

        public void go_to_char(int char_idx) {
            if (char_idx <= TextLength) {
                ++ignore_change_;
                SelectionStart = char_idx;
                SelectionLength = 0;
                sel_col_ = parent_.sel_col;
                sel_start_ = SelectionStart;
                sel_len_ = 0;
                --ignore_change_;

                update_ui();
            } 
        }

        public void after_click() {
            after_click_sel_col_ = sel_col_;
            after_click_sel_len_ = sel_len_;
            after_click_sel_start_ = sel_start_;
            util.postpone(check_if_user_hasnt_moved, ON_CLICK_WAIT_BEFORE_SELECTING_WORD_MS );
        }

        private void check_if_user_hasnt_moved() {
            if (sel_col_ == after_click_sel_col_ && sel_len_ == after_click_sel_len_ && sel_start_ == after_click_sel_start_) {
                // user hasn't moved after he clicked
                string txt = Text;
                int space = txt.LastIndexOf(' ', sel_start_, sel_start_);
                if (space <= 0)
                    // it's the first word
                    space = 0;
                else
                    ++space; // ignore the space itself

                // we found the word the user clicked on
                int next_space = txt.IndexOf(' ', space );
                int len = next_space >= 0 ? next_space - space : txt.Length - space;

                sel_start_ = space;
                sel_len_ = len;
                update_selected_text();
            }
            after_click_sel_col_ = -1;
        }

        public string sel_text {
            get {
                if (sel_len_ < 1)
                    return "";

                if (sel_start_ + sel_len_ <= TextLength)
                    return Text.Substring(sel_start_, sel_len_);
                if (sel_start_ < TextLength)
                    return Text.Substring(sel_start_);

                return "";
            }
        }

        public void force_update_sel() {
//            logger.Debug("[smart] sel =" + parent_.sel_row_idx + "," + parent_.sel_col + " [" + SelectionStart + "," + SelectionLength + "]");

            var old_sel = sel_text;
            sel_col_ = parent_.sel_col;
            sel_start_ = SelectionStart;
            sel_len_ = SelectionLength;

            if (old_sel != sel_text)
                on_sel_changed();
        }

        public void clear_sel() {
            if (sel_len_ > 0) {
                sel_len_ = 0;
                on_sel_changed();
            }
        }

        public void go_to_text(string text_to_select) {
            update_ui();

            int pos = Text.ToLower().IndexOf(text_to_select);
            Debug.Assert(pos >= 0);

            sel_start_ = pos;
            sel_len_ = text_to_select.Length;
            Focus();

            util.postpone(() => {
                readd_all_text();
                update_selected_text();
                on_sel_changed();
            }, 20);
        }

        public void update_sel() {
//            logger.Debug("[smart] sel =" + parent_.sel_row_idx + "," + parent_.sel_col + " [" + SelectionStart + "," + SelectionLength + "]");

            var old_sel = sel_text;

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

            if (old_sel != sel_text)
                on_sel_changed();
        }

        public void sel_to_left() {
            if (sel_start_ > 0) {
                ++sel_len_;
                --sel_start_;
                update_selected_text();
            }
        }

        public void sel_to_right() {
            if (sel_start_ + sel_len_ < TextLength) {
                ++sel_len_;
                update_selected_text();
            }
        }

        private void update_selected_text() {
            Select(sel_start_, sel_len_);
            util.postpone(() => on_sel_changed(), 1);
        }

        private void smart_readonly_textbox_SelectionChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;

//            logger.Debug("[smart] before sel =" + parent_.sel_row_idx + "," + parent_.sel_col + " [" + SelectionStart + "," + SelectionLength + "]");

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
            else 
                force_update_sel();
           
//            logger.Debug("[smart] after  sel =" + parent_.sel_row_idx + "," + parent_.sel_col + " [" + SelectionStart + "," + SelectionLength + "]");
        }

        private void check_empty_sel() {
            if (!changed_sel_background_) {
                return;
            }

            if (SelectedText == "") {
                // the idea is that we need to re-add the whole text, otherwise, the former selection will remain
                changed_sel_background_ = false;

                readd_all_text();
                force_update_sel();
            }
        }

        // the richedit control does not clear selection, even if we went somewhere else (thus, selection length would become zero)
        private void readd_all_text() {
            // ... this would actually be funny if it wasn't so fucking stupid...
            ++ignore_change_;

            int sel = SelectionStart;
            set_text( Text, true);
            SelectionStart = sel;
            
            --ignore_change_;
        }

        private void smart_readonly_textbox_KeyPress(object sender, KeyPressEventArgs e) {
            if ( e.KeyChar == '\x8') {
                // backspace
                if (sel_len_ > 0) {
                    --sel_len_;
                    // delete last space, if any
                    if (sel_text.EndsWith(" "))
                        --sel_len_;
                    readd_all_text();
                    update_selected_text();
                }
                return;
            }

            if ( e.KeyChar == '\x1b') {
                // escape
                if (sel_len_ > 0) {
                    sel_len_ = 0;
                    readd_all_text();
                    update_selected_text();
                }
                return;
            }

            string new_text = sel_text + e.KeyChar, new_text_with_space = "";
            if (sel_text != "")
                new_text_with_space = sel_text + " " + e.KeyChar;

            // all searches are case-insensitive
            new_text_with_space = new_text_with_space.ToLower();
            new_text = new_text.ToLower();

            // first, search ahead
            string txt = Text.ToLower();
            int pos_ahead = sel_start_ < txt.Length ? txt.IndexOf(new_text, sel_start_) : -1;
            int pos_ahead_space = new_text_with_space != "" && sel_start_ < txt.Length ? txt.IndexOf(new_text_with_space, sel_start_) : -1;

            if (pos_ahead >= 0 || pos_ahead_space >= 0) {
                int new_sel_start = pos_ahead >= 0 ? pos_ahead : pos_ahead_space;
                bool moving = new_sel_start != sel_start_;
                sel_start_ = new_sel_start;
                sel_len_ = pos_ahead >= 0 ? sel_len_ + 1 : sel_len_ + 2;

                if (moving)
                    readd_all_text();

                update_selected_text();
                return;
            }

            //second, search from start
            pos_ahead = txt.IndexOf(new_text);
            pos_ahead_space = new_text_with_space != "" ? txt.IndexOf(new_text_with_space) : -1;

            if (pos_ahead >= 0 || pos_ahead_space >= 0) {
                int new_sel_start = pos_ahead >= 0 ? pos_ahead : pos_ahead_space;
                bool moving = new_sel_start != sel_start_;
                sel_start_ = new_sel_start;
                sel_len_ = pos_ahead >= 0 ? sel_len_ + 1 : sel_len_ + 2;

                if (moving)
                    readd_all_text();

                update_selected_text();
                return;                
            }

            // did not find anything, let parent know
            on_search_ahead(new_text);
        }

    }
}
