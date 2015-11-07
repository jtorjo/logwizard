/* 
 * Copyright (C) 2014-2015 John Torjo
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
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using log4net.Repository.Hierarchy;

namespace lw_common.ui {
    internal partial class smart_readonly_textbox : RichTextBox {
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

        private int mouse_down_start_ = -1;

        private enum move_direction_type {
            left, right, none
        }
        private move_direction_type moving_ = move_direction_type.none;

        private class position {
            // the exact cell
            public int row = 0, col = 0;

            // the selection
            public int sel_start = 0, len = 0;
        }
        private Dictionary<string, position> last_positions_ = new Dictionary<string, position>(); 

        public smart_readonly_textbox() {
            InitializeComponent();
            BorderStyle = BorderStyle.None;

            // initially, until initialized,should be hidden
            util.postpone(() => Left = -200, 1);
            mouse_wheel.add(this, wheel);
        }

        public void init(log_view parent) {
            parent_ = parent;
            drawer_ = new log_view_item_draw_ui(parent);
            util.postpone(update_ui, 10);

            // ... easier testing
//            if ( util.is_debug)
  //              BorderStyle = BorderStyle.FixedSingle;
        }

        public int caret_left_pos {
            get {
                int start = SelectionStart;
                int pos = 0;
                using (Graphics g = CreateGraphics())
                    pos = drawer_.text_width(g, Text.Substring(0, start));
                return pos;
            }
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

        internal void force_refresh() {
            if (!Visible)
                return;
            if (parent_.sel_row_idx < 0)
                return;

            last_positions_.Clear();
            sel_len_ = 0;

            readd_all_text();
            update_selected_text();
            update_cached_sel_text();

            update_ui();
        }

        public void update_ui() {
            if (parent_ == null)
                return;
            if (ignore_change_ > 0)
                return;

            var bounds = parent_.sel_subrect_bounds;
            bool visible = should_be_visible();
            if (!visible) {
                Visible = false;
                return;
            }

            int offset_x = parent_.list.Left + 4;
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

            match_item i = parent_.sel;
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
                logger.Debug("[smart] go to " + char_idx + "/" + SelectionStart );

                update_ui();
            }
        }

        private int char_at_mouse() {
            return char_at_mouse(Cursor.Position);
        }

        private int char_at_mouse(Point mouse_screen) {
            var mouse = parent_.list.PointToClient(mouse_screen);
            var i = parent_.list.OlvHitTest(mouse.X, mouse.Y);
            if (i.Item == null || i.SubItem == null)
                return 0;

            int row_idx = i.RowIndex;
            int col_idx = i.ColumnIndex;
            Debug.Assert(col_idx >= 0 && row_idx >= 0);

            using (Graphics g = CreateGraphics()) {
                string text = parent_.list.GetItem(row_idx).GetSubItem(i.ColumnIndex).Text;
                var widths = parent_.render.text_widths(g, text);
                int offset_x = parent_.list.GetItem(row_idx).GetSubItemBounds(i.ColumnIndex).X;

                for (int idx = 0; idx < widths.Count; ++idx)
                    widths[idx] += offset_x;

                int char_idx = widths.FindLastIndex(x => x < mouse.X);
                if (widths.Count == 0 || widths.Last() < mouse.X)
                    char_idx = widths.Count;

                if (char_idx < 0)
                    char_idx = 0;
                return char_idx;
            }
        }

        public void on_mouse_click(Point mouse_screen) {
            var mouse = parent_.list.PointToClient(mouse_screen);
            var i = parent_.list.OlvHitTest(mouse.X, mouse.Y);

            Debug.Assert(i.Item != null && i.SubItem != null);
            int row_idx = i.RowIndex;
            Debug.Assert(parent_.is_editing);

            mouse_down_start_ = char_at_mouse(mouse_screen);
            go_to_char( mouse_down_start_);
            after_click(row_idx);

            Focus();
        }

        private void after_click(int row_idx) {
            logger.Debug("[smart] after click " + SelectionStart + "," + SelectionLength);
            after_click_sel_col_ = sel_col_;
            after_click_sel_row_ = row_idx;
            after_click_sel_start_ = sel_start_;
            if (sel_len_ == 0) 
                util.postpone(check_if_user_hasnt_moved, ON_CLICK_WAIT_BEFORE_SELECTING_WORD_MS);

            // note: sending/posting a wm_lbuttondown to the control does not really work
        }

        private void check_if_user_hasnt_moved() {
            if (sel_col_ == after_click_sel_col_ && sel_len_ == 0 && sel_row_ == after_click_sel_row_ && sel_start_ == after_click_sel_start_ 
                // 1.3.32+ - if selection length > 0 - user already started selecting... (via mouse maybe)
                && SelectionLength == 0) {
                // user hasn't moved after he clicked
                string txt = Text;
                bool clicked_in_middle_of_row = SelectionStart < TextLength;
                if (clicked_in_middle_of_row && sel_start_ < txt.Length) {
                    int start = sel_start_, end = sel_start_;

                    while ( start >= 0 && !string_search.is_delim_or_does_not_exist(txt, start))
                        --start;
                    while (end < txt.Length && !string_search.is_delim_or_does_not_exist(txt, end))
                        ++end;

                    // if false, clicked on space - don't do anything
                    bool clicked_in_middle_of_word = (start < end);
                    if (clicked_in_middle_of_word) {
                        ++start; // ... ignore the delimeter itself
                        sel_start_ = start;
                        sel_len_ = end - start;
                        update_cached_sel_text();
                        update_selected_text();
                    }
                }
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
            last_positions_.Clear();
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
            if ( sel_len_ == 0)
                moving_ = move_direction_type.left;

            if (sel_start_ > 0) {
                if (moving_ == move_direction_type.left) {
                    ++sel_len_;
                    --sel_start_;
                } else {
                    --sel_len_;
                    readd_all_text();
                }
                update_cached_sel_text();
                update_selected_text();
            }
        }

        public void sel_to_right() {
            if ( sel_len_ == 0)
                moving_ = move_direction_type.right;

            if (sel_start_ + sel_len_ < TextLength) {
                if (moving_ == move_direction_type.right)
                    ++sel_len_;
                else {
                    ++sel_start_;
                    --sel_len_;
                    readd_all_text();
                }
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
            add_cur_text_to_positions();

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
            last_positions_.Clear();

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

                string txt = raw_sel_text().ToLower();
                if (last_positions_.ContainsKey(txt))
                    go_to_backspace_text(txt);

                update_cached_sel_text();
                readd_all_text();
                update_selected_text();
            }            
        }

        // goes back to the text selected before typing a new letter - EVEN if within the same line
        private void go_to_backspace_text(string txt) {
            Debug.Assert(last_positions_.ContainsKey(txt));
            position pos = last_positions_[txt];
            // remove unused entries - just in case
            last_positions_ = last_positions_.Where(x => txt.StartsWith(x.Key)).ToDictionary(x => x.Key, x => x.Value);

            bool needs_update_ui = (pos.col != sel_col_) || (pos.row != sel_row_);
            ++ignore_change_;
            parent_.select_cell(pos.row, pos.col);
            --ignore_change_;
            if ( needs_update_ui)
                update_ui();

            sel_start_ = pos.sel_start;
            sel_len_ = pos.len;
        }

        private void add_cur_text_to_positions() {
            if (sel_len_ < 1)
                return;
            string txt = Text.Substring(sel_start_, sel_len_).ToLower();
            if ( !last_positions_.ContainsKey(txt))
                last_positions_.Add(txt, new position());
            position pos = last_positions_[txt];
            pos.col = sel_col_;
            pos.row = sel_row_;
            pos.sel_start = sel_start_;
            pos.len = sel_len_;
            logger.Debug("[smart] new pos - " + pos.row + "," + pos.col + "," + pos.sel_start + "," + pos.len);
        }

        private void smart_readonly_textbox_KeyPress(object sender, KeyPressEventArgs e) {
            e.Handled = true;

            if ( e.KeyChar == '\x1b') {
                // escape
                escape();
                return;
            }
            moving_ = move_direction_type.right;

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

        private void smart_readonly_textbox_MouseClick(object sender, MouseEventArgs e) {
        }

        private void smart_readonly_textbox_MouseDown(object sender, MouseEventArgs e) {
        }

        private void smart_readonly_textbox_MouseUp(object sender, MouseEventArgs e) {
            on_mouse_up();
            if (e.Button == MouseButtons.Right)
                parent_.right_click.right_click();
        }

        public void on_mouse_up() {
            moving_ = move_direction_type.none;
            mouse_down_start_ = -1;
            parent_.lv_parent.sel_changed(log_view_sel_change_type.click);
        }

        //private int mouse_idx = 0;
        private void wheel(Message m) {
            //logger.Info("wheel on " + "/" + mouse_idx++ + " on " + parent_.name );
            win32.SendMessage(parent_.list.Handle, m.Msg, m.WParam, m.LParam);
        }

        private void smart_readonly_textbox_MouseMove(object sender, MouseEventArgs e) {
            // note: at this point, this won't handle horizontal scrolling correctly (in other words, if text bigger than what's shown)
            //       lets all thank MS for such an awesome control!
            if (mouse_down_start_ >= 0) {
                int now = char_at_mouse();
                int old_len = sel_len_;
                if (now > mouse_down_start_) {
                    sel_start_ = mouse_down_start_;
                    sel_len_ = now - mouse_down_start_;
                } else {
                    sel_start_ = now;
                    sel_len_ = mouse_down_start_ - now;
                }
                if (old_len > sel_len_)
                    readd_all_text();
                update_cached_sel_text();
                update_selected_text();
            }
        }

    }
}
