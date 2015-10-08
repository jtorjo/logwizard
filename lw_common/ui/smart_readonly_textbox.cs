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

        private const int TRY_GAIN_FOCUS_AFTER_FORCE_INVISIBLE = 500;

        private log_view parent_;

        private int sel_row_ = -1;
        private int sel_start_ = 0, sel_len_ = 0, sel_col_ = -1;

        private int after_click_sel_start_ = -1, after_click_sel_row_ = -1, after_click_sel_col_ = -1;

        private string cached_sel_text_ = "";

        private bool changed_sel_background_ = false;

        private int ignore_change_ = 0;

        private string all_text_ = "";

        // sometimes when the user is moving, we need to become invisible
        private DateTime last_force_invisible_ = DateTime.MinValue;

        public delegate void void_func();

        public delegate void search_func(string text);

        // called when selection has changed
        public void_func on_sel_changed;

        public search_func on_search_ahead;

        private log_view_item_draw_ui drawer_ = null;
        print_info default_print_ = new print_info();

        public smart_readonly_textbox() {
            InitializeComponent();
            BorderStyle = BorderStyle.None;

            // initially, until initialized,should be hidden
            util.postpone(() => Left = -200, 1);
        }

        public void init(log_view parent) {
            parent_ = parent;
            drawer_ = new log_view_item_draw_ui(parent);
            util.postpone(update_ui, 10);

            // ... easier testing
            if ( util.is_debug)
                BorderStyle = BorderStyle.FixedSingle;
        }

        private bool should_be_visible() {
            int sel = parent_.sel_row_idx;
            var bounds = parent_.sel_subrect_bounds;
            bool is_multi_selection = parent_.list.SelectedIndices != null && parent_.list.SelectedIndices.Count > 1;
            bool should_be = sel >= 0 && bounds.Width > 0 && bounds.Height > 0 && parent_.is_editing && !is_multi_selection;
            if (should_be) {
                var focus = win32.focused_ctrl();
                should_be = focus == parent_.list || focus == this;
            }
            return should_be;
        }

        public void update_ui() {
            if (parent_ == null)
                return;

            var bounds = parent_.sel_subrect_bounds;
            bool visible = should_be_visible();
            if (!visible) {
                Visible = false;
                return;
            }

            int offset_x = parent_.list.Left;
            int offset_y = parent_.list.Top;
            var location = new Point(offset_x + bounds.X, offset_y + bounds.Y + 2);
            if (location.Y + bounds.Height > parent_.Height) {
                // it was the last row when user is moving down (down arrow) - we'll get notified again
                last_force_invisible_ = DateTime.Now;
                Visible = false;
                return;
            }
            int header_height = parent_.list.HeaderControl.ClientRectangle.Height;
            if (location.Y < offset_y + header_height) {
                // it was the first row when user is moving up (up arrow), we'll get notified again
                last_force_invisible_ = DateTime.Now;
                Visible = false;
                return;
            }

            ++ignore_change_;

            Location = location;
            Size = new Size(bounds.Width, bounds.Height);
            Font = parent_.list.Font;

            log_view.item i = parent_.sel;
            ForeColor = i.fg(parent_);
            BackColor = i.sel_bg(parent_);

            set_text(false);
            SelectionBackColor = BackColor;
            SelectionColor = ForeColor;
            changed_sel_background_ = false;

            // make visible only after we've properly set the location (otherwise, we would flicker)
            Visible = true;

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
        }

        private void set_text(bool force) {
            if (drawer_ == null)
                return;
            if (sel_row_ == parent_.sel_row_idx && sel_col_ == parent_.sel_col_idx && !force)
                return;

            sel_row_ = parent_.sel_row_idx;
            sel_col_ = parent_.sel_col_idx;
            string txt = parent_.sel_subitem_text;

            ++ignore_change_;
            Clear();

            AppendText(txt);
            var prints = parent_.sel.override_print(parent_, txt, sel_col_);
            var full_row = parent_.list.GetItem(sel_row_);
            int last_idx = 0;
            for (int print_idx = 0; print_idx < prints.Count; ++print_idx) {
                int cur_idx = prints[print_idx].Item1, cur_len = prints[print_idx].Item2;
                string before = txt.Substring(last_idx, cur_idx - last_idx);
                if (before != "") {
                    Select(last_idx, cur_idx - last_idx);
                    SelectionColor = drawer_.print_fg_color(full_row, default_print_);
                    SelectionBackColor = drawer_.bg_color(full_row, sel_col_);
                }
                Select(cur_idx, cur_len);
                SelectionColor = drawer_.print_fg_color(full_row, prints[print_idx].Item3);
                SelectionBackColor = drawer_.print_bg_color(full_row, prints[print_idx].Item3);
                last_idx = cur_idx + cur_len;
            }
            last_idx = prints.Count > 0 ? prints.Last().Item1 + prints.Last().Item2 : 0;
            if (last_idx < txt.Length) {
                Select(last_idx, txt.Length - last_idx);
                SelectionColor = drawer_.print_fg_color(full_row, default_print_);
                SelectionBackColor = drawer_.bg_color(full_row, sel_col_);
            }
            // ... safety net
            SelectionStart = 0;
            SelectionLength = 0;

            --ignore_change_;
        }

        public void go_to_char(int char_idx) {
            if (char_idx <= TextLength) {
                ++ignore_change_;
                SelectionStart = char_idx;
                SelectionLength = 0;
                sel_start_ = SelectionStart;
                sel_len_ = 0;
                --ignore_change_;

                update_ui();
            }
        }

        public void after_click() {
            after_click_sel_col_ = sel_col_;
            after_click_sel_row_ = parent_.sel_row_idx;
            after_click_sel_start_ = sel_start_;
            if ( sel_len_ == 0)
                util.postpone(check_if_user_hasnt_moved, ON_CLICK_WAIT_BEFORE_SELECTING_WORD_MS );
        }

        private void check_if_user_hasnt_moved() {
            if (sel_col_ == after_click_sel_col_ && sel_len_ == 0 && sel_row_ == after_click_sel_row_ && sel_start_ == after_click_sel_start_) {
                // user hasn't moved after he clicked
                string txt = Text;
                int space = sel_start_ < txt.Length ? txt.LastIndexOf(' ', sel_start_, sel_start_) : txt.LastIndexOf(' ');
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
                update_cached_sel_text();
                update_selected_text();
            }
            after_click_sel_col_ = -1;
        }

        public string sel_text {
            get {
                return cached_sel_text_;
            }
        }

        public void force_update_sel() {
//            logger.Debug("[smart] sel =" + parent_.sel_row_idx + "," + parent_.sel_col + " [" + SelectionStart + "," + SelectionLength + "]");

            Debug.Assert( sel_col_ == parent_.sel_col_idx);
            
            sel_start_ = SelectionStart;
            sel_len_ = SelectionLength;
        }

        public void clear_sel() {
            sel_len_ = 0;
        }

        public void update_sel() {
//            logger.Debug("[smart] sel =" + parent_.sel_row_idx + "," + parent_.sel_col + " [" + SelectionStart + "," + SelectionLength + "]");

            if (sel_col_ == parent_.sel_col_idx) {
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
                sel_start_ = SelectionStart;
                sel_len_ = SelectionLength;
                // force showing the text for this column
                update_ui();
            }
        }

        public void go_to_text(string text_to_select) {
            update_ui();

            int pos = Text.ToLower().IndexOf(text_to_select);
            Debug.Assert(pos >= 0);

            sel_start_ = pos;
            sel_len_ = text_to_select.Length;
            update_cached_sel_text();

            util.postpone(() => {
                readd_all_text();
                update_selected_text();
            }, 20);
        }

        public void sel_to_left() {
            if (sel_start_ > 0) {
                ++sel_len_;
                --sel_start_;
                update_cached_sel_text();
                update_selected_text();
            }
        }

        public void sel_to_right() {
            if (sel_start_ + sel_len_ < TextLength) {
                ++sel_len_;
                update_cached_sel_text();
                update_selected_text();
            }
        }

        private string raw_sel_text() {
            if (sel_len_ < 1)
                return "";
            else if (sel_start_ + sel_len_ <= TextLength)
                return Text.Substring(sel_start_, sel_len_);
            else if (sel_start_ < TextLength)
                return Text.Substring(sel_start_);
            else
                return "";
        }

        // 1.2.7+ we cache the selection, so that the user will see what he last selected in a different color anyway, even after he goes somewhere else
        private void update_cached_sel_text() {
            var old_sel = sel_text;

            cached_sel_text_ = raw_sel_text();

            if (old_sel != sel_text)
                on_sel_changed();
        }

        private void update_selected_text() {
            Select(sel_start_, sel_len_);
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
            set_text( true);
            SelectionStart = sel;
            
            --ignore_change_;
        }

        public void escape() {
            // escape
            if (sel_len_ > 0) {
                sel_len_ = 0;
                readd_all_text();
                update_selected_text();
            }

            update_cached_sel_text();
        }

        public void backspace() {
            if (sel_len_ > 0) {
                --sel_len_;
                // delete last space, if any
                if (raw_sel_text().EndsWith(" "))
                    --sel_len_;
                update_cached_sel_text();
                readd_all_text();
                update_selected_text();
            }            
        }

        private void smart_readonly_textbox_KeyPress(object sender, KeyPressEventArgs e) {
            e.Handled = true;

            if ( e.KeyChar == '\x1b') {
                // escape
                escape();
                return;
            }

            string new_text = raw_sel_text() + e.KeyChar, new_text_with_space = "";
            if (raw_sel_text() != "")
                new_text_with_space = raw_sel_text() + " " + e.KeyChar;

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

                update_cached_sel_text();
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

                update_cached_sel_text();
                update_selected_text();
                return;                
            }

            // did not find anything, let parent know
            on_search_ahead(new_text);
        }

        private void stealFocus_Tick(object sender, EventArgs e) {
            if ( parent_ == null)
                // in design mode
                return;

            if (!Visible && last_force_invisible_.AddMilliseconds(TRY_GAIN_FOCUS_AFTER_FORCE_INVISIBLE) >= DateTime.Now)
                if (should_be_visible()) 
                    update_ui();

            if (win32.focused_ctrl() == parent_.list && parent_.is_editing && Visible)
                Focus();
            else 
                // note: we need to update background when I lose focus, or the parent loses focus
                //       however, dealing with those cases would still involve create a timer to check when focus is going to another control, etc.
                //
                //       doing it here is the simple solution
                parent_.update_background();

            if (Visible && !parent_.has_focus)
                // user has moved focus somewhere else
                Visible = false;
        }

    }
}
