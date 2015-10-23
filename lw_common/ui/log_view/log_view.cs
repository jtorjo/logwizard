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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using lw_common;
using LogWizard;

namespace lw_common.ui
{
    public partial class log_view : UserControl
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // how many rows to search ahead (smart search)
        private const int SEARCH_AROUND_ROWS = 100;

        public const string FULLLOG_NAME = "__all_this_is_fulllog__";

        private Form parent;
        private readonly filter filter_ ;
        private log_reader log_ = null;

        private string selected_view_ = null;

        private int last_item_count_while_current_view_ = 0;

        private Font non_bold_ = null, bold_ = null;


        // the reason we have this class - is for memory eficiency - since all views (except full log) don't need the info here


        private log_view_render render_;

        private const int MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS = 10;

        private log_view_data_source model_ = null;
        private int visible_columns_refreshed_ = 0;

        // lines that are bookmarks (sorted by index)
        private List<int> bookmarks_ = new List<int>();

        private int font_size_ = 9; // default font size

        private List<int> full_widths_ = new List<int>();

        private bool pad_name_on_left_ = false;

        private bool show_name_in_header_ = false;
        private bool show_header_ = true;
        private bool show_name_ = true;

        private int old_item_count_ = 0;

        // it's the subitem column that is currently selected (for smart readonly editing)
        private int cur_col_ = 0;

        private bool is_editing_ = false;
        private int editing_row_ = -1;

        private search_form.search_for cur_search_ = null;
        private int cur_filter_row_idx_ = -1;

        private log_view_right_click right_click_;

        // in case the user edits and presses Escape
        private string old_view_name_ = "";

        private ui_info.show_row_type show_row_ = ui_info.show_row_type.full_row;

        private int sel_y_offset_before_set_filter_ = 0;
        private int sel_line_idx_before_set_filter_ = 0;

        public log_view(Form parent, string name)
        {
            Debug.Assert(parent is log_view_parent);

            filter_ = new filter(this.create_match_object);
            filter_.on_change = on_change;

            InitializeComponent();
            this.parent = parent;
            viewName.Text = name;
            model_ = new log_view_data_source(this.list, this ) { name = name };
            list.VirtualListDataSource = model_;
            list.RowHeight = 18;

            load_font();
            lv_parent.handle_subcontrol_keys(this);

            render_ = new log_view_render(this);
            foreach (var col in list.AllColumns)
                (col as OLVColumn).Renderer = render_;
            right_click_ = new log_view_right_click(this);

            // just an example:
            //render_.set_override("settings", new log_view_render.print_info { fg = Color.Blue, bold = true });
            cur_col_ = msgCol.fixed_index();
            edit.on_sel_changed = on_edit_sel_changed;
            edit.on_search_ahead = search_ahead;
            edit.init(this);
            edit.BringToFront();
        }

        internal log_view_parent lv_parent {
            get {
                return parent as log_view_parent;
            }
        }

        internal filter filter {
            get { return filter_; }
        }

        private filter.match create_match_object(BitArray matches, filter_line.font_info font, line line, int lineIdx) {
            return is_full_log ? new full_log_match_item(matches, font, line, lineIdx, this) : new match_item(matches, font, line, lineIdx, this);
        }

        public override string ToString() {
            return name;
        }

        public void force_refresh_visible_columns() {
            visible_columns_refreshed_ = 0;
            refresh_visible_columns();
        }

        private void load_font() {
            string[] font_names = app.inst.sett.get("font_names").Split(',');
            foreach ( string name in font_names)
                try {
                    var f = new Font(name, font_size_); 
                    list.Font = f;
                    break;
                } catch {
                }
        }


        private void filterName_TextChanged(object sender, EventArgs e)
        {
            lv_parent.on_view_name_changed(this, viewName.Text);
        }

        public bool is_filter_set {
            get { return filter_.row_count > 0; }
        }
        public bool is_filter_up_to_date {
            get {
                if (!is_filter_set)
                    return true;
                return filter_.is_up_to_date;
            }
        }

        public int view_idx {
            get {
                if (is_full_log)
                    return -1;

                var parent = tab_control;
                var page = tab_parent;
                // note: .TabPages is not really a collection, so I don't want to use IndexOf - RemoveAt works incorrectly, so, for me, all bets are off
                for ( int idx = 0; idx < parent.TabPages.Count; ++idx)
                    if (parent.TabPages[idx] == page)
                        return idx;

                Debug.Assert(false);
                return 0;
            }
        }

        // 1.0.77+ - care about multiple selection - returns the first sel
        public int sel_row_idx {
            get {
                int sel = list.SelectedIndex;
                if (sel >= 0)
                    return sel;
                
                var multi = list.SelectedIndices;
                if ( multi != null)
                    if (multi.Count > 0)
                        return multi[0];

                return -1;
            }
        }

        public List<int> multi_sel_idx {
            get {
                List<int> sel = new List<int>();
                var multi = list.SelectedIndices;
                if ( multi != null)
                    for ( int i = 0; i < multi.Count; ++i)
                        sel.Add(multi[i]);
                
                var cur_row = sel_row_idx;
                if ( sel.Count == 0 && cur_row >= 0)
                    // in this case, a single selection
                    sel.Add(cur_row);
                return sel;
            }
        }

        internal match_item sel {
            get {
                int idx = sel_row_idx;
                if (idx >= 0)
                    return item_at(idx);
                else
                    return null;
            }
        }

        private int visible_column(int col_idx) {
            var col = list.AllColumns[col_idx];
            return list.Columns.IndexOf(col);
        }

        internal Rectangle sel_subrect_bounds {
            get {
                Debug.Assert(cur_col_ >= 0 && cur_col_ < list.AllColumns.Count);

                int sel = sel_row_idx;
                if (sel >= 0) {
                    var r = list.GetItem(sel).GetSubItemBounds( visible_column( cur_col_));
                    if (r.Height == 0) {
                        // this can happen when the message is not visible at all
                        r = list.GetItem(sel).Bounds;
                        r.Width = 0;
                    }
                    return r;
                }
                return new Rectangle();
            }
        }

        internal int sel_col_idx {
            get { return cur_col_; }
        }

        internal string sel_subitem_text {
            get {
                int sel = sel_row_idx;
                if (sel >= 0)
                    return list.GetItem(sel).GetSubItem( visible_column( cur_col_)).Text;
                else
                    return "";
            }
        }

        // returns the line index of the current selection
        public int sel_line_idx {
            get {
                int sel = sel_row_idx;
                if (sel >= 0)
                    return item_at(sel).match.line_idx;
                return -1;
            }
        }

        // returns the text the user has selected via the smart edit
        public string smart_edit_sel_text {
            get { return edit.sel_text; }
        }

        public string sel_line_text {
            get {
                int sel = sel_row_idx;
                if (sel >= 0)
                    return item_at(sel) .match.line.part(info_type.msg);
                return "";
            }
        }
        // returns the sel rectangle, in screen coordinates
        public Rectangle sel_rect_screen {
            get {
                int sel = sel_row_idx;
                if (sel >= 0) {
                    var r = list.RectangleToScreen(list.GetItem(sel).GetSubItemBounds(msgCol.Index));
                    if (r.Height == 0) {
                        // this can happen when the message is not visible at all
                        r = list.RectangleToScreen(list.GetItem(sel).Bounds);
                        r.Width = 0;
                    }
                    return r;
                }
                return new Rectangle();
            }
        }

        private bool is_current_view {
            get {
                var parent = Parent as TabPage;
                if (parent != null) {
                    var tab = parent.Parent as TabControl;
                    bool is_ = tab.SelectedTab == parent;
                    return is_;
                }
                return false;
            }
        }

        private TabPage tab_parent {
            get {
                return Parent as TabPage;
            }
        }
        private TabControl tab_control {
            get {
                Debug.Assert(tab_parent != null);
                var tab = tab_parent.Parent as TabControl;
                Debug.Assert(tab != null);
                return tab;
            }
        }
        public Font title_font {
            get {
                if (non_bold_ == null)
                    return tab_parent.Font;
                return has_anything_changed ? bold_ : non_bold_;
            }
        }

        public bool filter_view {
            get { return model_.filter_view; }
        }

        public bool show_full_log {
            get { return model_.show_full_log; }
        }

        public void set_filter(bool filter_view, bool show_full_log) {
            if (is_full_log)
                // on full log - don't allow any toggling (even though theoretically it could be possible)
                // the idea is that the full log should always be FULL LOG - the existing views, you can abuse them in any way :)
                return;

            if (!Enabled)
                // we're already in the process of computing a filter
                return;

            if (model_.filter_view == filter_view && model_.show_full_log == show_full_log)
                // nothing changed
                return;

            // until we finish running the filter, don't allow any more toggling
            Enabled = false;
            Cursor = Cursors.WaitCursor;

            sel_y_offset_before_set_filter_ = visible_row_y_offset(sel_row_idx);
            sel_line_idx_before_set_filter_ = sel_line_idx;

            bool needs_to_set_item_filter_now = filter_view && !model_.filter_view;
            if (needs_to_set_item_filter_now) {
                string sel_text = edit.sel_text;
                if (cur_search_ != null || sel_text != "")
                    model_.item_filter = item_search_filter;
                else {
                    // search by current filter 
                    if (lv_parent.selected_row_index >= 0) {
                        // search by selected filter (we're focused on teh filters pane)
                        List<int> filters = new List<int>();
                        filters.Add(lv_parent.selected_row_index);
                        model_.item_filter = (i, a) => item_run_several_filters(i, filters);
                    } else {
                        // search by filters matching this line
                        var filters = filters_matching_sel_line();
                        model_.item_filter = (i, a) => item_run_several_filters(i, filters);
                    }
                }
            }
            else if ( filter_view) 
                Debug.Assert(model_.item_filter != null);
            model_.set_filter(filter_view, show_full_log);
        }

        private List<int> filters_matching_sel_line() {
            List<int> filter_row_indexes = new List<int>();
            if ( sel_row_idx < 0)
                return filter_row_indexes;

            int row_idx = 0;
            foreach (var match in util.to_list(sel.matches)) {
                if ( match)
                    filter_row_indexes.Add(row_idx);
                ++row_idx;
            }

            return filter_row_indexes;
        }


        // note: even if applied on the full log, or just on the view itself, this will yield exactly the same results
        //
        // it's because the filters are already applied on the full log, thus yielding a specific set of results
        private bool item_run_several_filters(match_item i, List<int> row_indexes) {
            if (row_indexes.Count < 1)
                // no filter(s) to apply
                return true;

            int count = i.matches.Count;
            if (count < 1)
                // it's not from our view, it's from the full log
                return false;

            foreach ( int idx in row_indexes)
                if (idx < count) {
                    if (!i.matches[idx])
                        return false;
                } else {
                    // our filter has less rows than are selectected from the current line ???
                    Debug.Assert(false);
                    return false;
                }

            return true;
        }


        // further filtering (toggle)
        private bool item_search_filter(match_item item, bool applied_on_full_log) {
            string sel_text = edit.sel_text;
            if (cur_search_ == null && sel_text == "")
                // no search - nothing matches
                return false;

            if (sel_text != "") {
                sel_text = sel_text.ToLower();
                string cur = log_view_cell.cell_value(item, cur_col_);
                return cur.ToLower().Contains(sel_text);
            }

            // current search
            return string_search.matches(item.match.line.part(info_type.msg), cur_search_);
        }

        public int item_count {
            get { return model_.item_count; }
        }

        public string filter_friendly_name {
            get {
                string sel_text = edit.sel_text;
                if ( sel_text != "")
                    return sel_text + " (case-insensitive)";

                if (cur_search_ != null)
                    return cur_search_.friendly_name;

                if (sel_row_idx >= 0)
                    return "Filter(s) matching this line";

                return "";
            }
        }


        internal log_view_right_click right_click {
            get { return right_click_; }
        }

        internal Color bookmark_fg {
            get { return app.inst.bookmark_fg; }
        }

        internal Color bookmark_bg {
            get { return app.inst.bookmark_bg; }
        }


        // while this is true, has anything_changed always returns false (basically, we want this during loading,
        // since it happens on a different thread - it's pretty complicated to find out when the complete loading has occured)
        private bool turn_off_has_anying_changed_ = true;

        public bool turn_off_has_anying_changed {
            get { return turn_off_has_anying_changed_; }
            set {
                turn_off_has_anying_changed_ = value;
                if (!turn_off_has_anying_changed_) {
                    last_item_count_while_current_view_ = filter_.match_count;
                    update_x_of_y();
                }
            }
        }

        public bool has_anything_changed {
            get {
                if (turn_off_has_anying_changed)
                    return false;
                return !is_current_view && (last_item_count_while_current_view_ != filter_.match_count);
            }
        }

        internal bool is_full_log {
            get {
                // 1.2.19+ - when opening a new file for the first time (thus, no views/filters at all), we don't want to mistakengly think it's the full-log
                //return filter_ != null ? filter_.row_count < 1 : true;
                return viewName != null ? name == FULLLOG_NAME : true;
            }
        }


        public void set_filter(List<raw_filter_row> filter) {
            filter_.name = name;
            filter_.update_rows(filter);
        }

        // called when this log view is not used anymore (like, when it's removed from its tab page)
        public void mark_as_not_used() {
            if (log_ != null) {
                log_.Dispose();
                log_ = null;
            }
            model_.Dispose();
        }

        internal bool has_bookmark(int line_idx) {
            return bookmarks_.Contains(line_idx);
        }

        public void set_log(log_reader log) {
            Debug.Assert(log != null);
            if (log_ != log) {
                if (log_ != null)
                    log_.Dispose();

                log_ = log;
                log_.tab_name = name;
                log_.on_new_lines += filter_.on_new_reader_lines;

                last_item_count_while_current_view_ = 0;
                visible_columns_refreshed_ = 0;
                logger.Debug("[view] new log for " + name + " - " + log.log_name);
                update_x_of_y();
            }
        }

        public string name {
            get { return viewName.Text; }
            set {
                viewName.Text = value;
                model_.name = value;
                filter_.name = value;
                if (log_ != null)
                    log_.tab_name = value;
            }
        }

        public void update_show_name() {            
            viewName.Visible = show_name_;
            labelName.Visible = show_name_;
 
            int height = viewName.Height + 2;
            list.Top = !show_name_ ? list.Top - height : list.Top + height;
            list.Height = !show_name_ ? list.Height + height : list.Height - height;
        }

        public int header_height {
            get {
                int height = list.HeaderControl.ClientRectangle.Height;
                Debug.Assert(height > 0);
                return height;
            }
        }

        private void update_show_header() {
            // note: if name is shown, we can't hide header
            bool show = show_name_ || show_header_;

            viewName.Visible = show;
            labelName.Visible = show;
 
            int height = list.HeaderControl.ClientRectangle.Height;
            list.Top = !show ? list.Top - height : list.Top + height;
            list.Height = !show ? list.Height + height : list.Height - height;
        }

        public void show_view(bool show) {
            viewCol.Width = show ? 100 : 0;
        }

        private OLVListItem row_by_line_idx(int line_idx) {
            var m = filter_.matches.binary_search(line_idx);
            return m.Item1 != null ? list.GetItem(m.Item2) : null;
        }

        private match_item item_at(int idx) {
            return model_.item_at(idx);
#if old_code
            return filter_.matches.match_at(idx) as match_item;
#endif
        }


        public void on_action(action_type action) {
            switch (action) {
            case action_type.home:
            case action_type.end:
            case action_type.pageup:
            case action_type.pagedown:
            case action_type.arrow_up:
            case action_type.arrow_down:
            case action_type.shift_arrow_down:
            case action_type.shift_arrow_up:
                break;
            default:
                Debug.Assert(false);
                return;
            }
            edit.clear_sel();

            int sel = sel_row_idx;
            int count = item_count;
            if (sel < 0 && count > 0)
                sel = 0; // assume we start from the top

            // int rows_per_page = list.RowsPerPage;
            int height = list.Height - list.HeaderControl.ClientRectangle.Height;
            switch (action) {
            case action_type.home:
                sel = 0;
                break;
            case action_type.end:
                sel = count - 1;
                break;
            case action_type.pageup:
                if (sel >= 0) {
                    var r = list.GetItem(sel).Bounds;
                    var middle = new Point( r.Left + r.Width / 2, r.Top + r.Height / 2 );
                    list.LowLevelScroll(0, -height);
                    int new_sel = list.HitTest(middle).Item.Index;
                    if (new_sel != sel)
                        sel = new_sel;
                    else
                        sel = 0; // reached top
                }
                break;
            case action_type.pagedown:
                if (sel < count - 1) {
                    var r = list.GetItem(sel).Bounds;
                    var middle = new Point( r.Left + r.Width / 2, r.Top + r.Height / 2 );
                    list.LowLevelScroll(0, height);
                    int new_sel = list.HitTest(middle).Item.Index;
                    if (new_sel != sel)
                        sel = new_sel;
                    else
                        sel = count - 1; // reached bottom
                }
                break;
            case action_type.arrow_up:
                if ( sel > 0)
                    --sel;
                break;
            case action_type.arrow_down:
                if ( sel < count - 1)
                    ++sel;
                break;

            // default list behavior is wrong, we need to override
            case action_type.shift_arrow_up:                
                if (sel > 0) {
                    int existing = selected_indices_array().Min();
                    if (existing > 0) {
                        list.SelectedIndices.Add(existing - 1);
                        ensure_row_visible(existing);
                    }
                    return;
                }
                break;
            // default list behavior is wrong, we need to override
            case action_type.shift_arrow_down:
                if (sel < count - 1) {
                    int existing = selected_indices_array().Max();
                    if (existing < count - 1) {
                        list.SelectedIndices.Add(existing + 1);
                        ensure_row_visible(existing);                        
                    }
                    return;
                }
                break;

            }
            if (sel >= 0 && sel_row_idx != sel) {
                select_row_idx( sel, select_type.notify_parent);
                ensure_row_visible(sel);
            }
            edit.update_sel();
        }

        private List<int> selected_indices_array() {
            List<int> sel = new List<int>();
            if ( list.SelectedIndices != null)
                for ( int i = 0; i < list.SelectedIndices.Count; ++i)
                    sel.Add(list.SelectedIndices[i]);
            return sel;
        } 

        private bool needs_scroll_to_last() {
            if (item_count < 1)
                return true;
            if (sel_row_idx < 0)
                return true;
            if (old_item_count_ > 0 && sel_row_idx == old_item_count_ - 1)
                return true;
            return false;
        }

        string cell_value(match_item i, int column_idx) {
            return log_view_cell.cell_value(i, column_idx);
        }

        private string cell_value_by_type(match_item i, info_type type) {
            return log_view_cell.cell_value_by_type(i, type);
        }

        OLVColumn column(info_type type) {
            return log_view_cell.column(this, type);
        }


        private bool has_value_at_column(info_type type, int max_rows_to_check) {
            int value_count = 0;
            for (int idx = 0; idx < max_rows_to_check; ++idx) {
                var i = item_at(idx) ;
                if (i.line_idx < 0)
                    continue;
                if (cell_value_by_type(i, type) != "")
                    ++value_count;
            }
            bool has_values = (value_count > 0);
            return has_values;
        }

        // when we have a number of columns - based on the info on each column, we hide or show them
        private void refresh_visible_columns() {
            if (visible_columns_refreshed_ >= MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS)
                return;

            const int DEFAULT_COL_WIDTH = 80;
            int count = item_count;
            bool needs_refresh = count != visible_columns_refreshed_;
            if (needs_refresh) {
                int row_count = Math.Min(count, MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS);
                for (int type_as_int = 0; type_as_int < (int) info_type.max; ++type_as_int) {
                    info_type type = (info_type) type_as_int;
                    bool is_visible = type == info_type.msg || has_value_at_column(type, row_count);
                    show_column(column(type), (is_visible ? DEFAULT_COL_WIDTH : 0), is_visible);
                }
                visible_columns_refreshed_ = count;
            }

            list.RebuildColumns();
            if (count >= MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS) {
                visible_columns_refreshed_ = MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS;
                full_widths_.Clear();
                show_row_impl(show_row_);
            }
        }

        private void show_column(OLVColumn col, int width, bool show) {
            if (col.IsVisible == show)
                return;

            col.Width = width;
            col.IsVisible = show;
        }

        private void compute_title_fonts() {
            if (tab_parent == null)
                return;
            if (non_bold_ != null)
                return; // already computed
            // we need this - so that we have enough space in order to draw the title in bold/non-bold
            Debug.Assert(tab_parent.Font.Bold);
            bold_ = tab_parent.Font;
            non_bold_ = new Font(bold_.FontFamily, bold_.Size, FontStyle.Regular);
        }

        public void update_x_of_y() {
            compute_title_fonts();

            string x_idx =  sel_row_idx >= 0 ? "" + (sel_row_idx+1) : "";
            string x_line =  sel_line_idx >= 0 ? "" + (sel_line_idx + 1) : "";
            string y = "" + item_count;
            string header = (app.inst.show_view_line_count || app.inst.show_view_selected_line || app.inst.show_view_selected_index ? (x_idx != "" ? x_idx + " of " + y : "(" + y + ")") : "");
            string x_of_y_msg = (show_name_in_header ? "[" + name + "] " : "") + "Message " + header;
            string x_of_y_title = "";
            if (app.inst.show_view_line_count && app.inst.show_view_selected_index)
                x_of_y_title = " (" + (x_idx != "" ? x_idx + "/" : "") + y + ")";
            else if (!app.inst.show_view_line_count && app.inst.show_view_selected_index) {
                if ( x_idx != "")
                    x_of_y_title = " (" + x_idx + ")";
            } else if (app.inst.show_view_line_count && !app.inst.show_view_selected_index)
                x_of_y_title = " (" + y + ")";

            if ( x_line != "" && app.inst.show_view_selected_line && !is_current_view)
                x_of_y_title = " [" + x_line + "] " + x_of_y_title;
            x_of_y_title = x_of_y_title.TrimEnd();

            string tab_text = name + x_of_y_title;
            tab_text += "  ";
            if (pad_name_on_left_)
                // this is so that the "Toggle topmost" does not obscure the first tab's name
                tab_text = "    " + tab_text;
            if (tab_parent != null) {
                if ( tab_parent.Text != tab_text)
                    tab_parent.Text = tab_text;
            }

            msgCol.Text = x_of_y_msg;
        }

        // called when we've been selected as current view
        public void on_selected() {
            if (tab_parent == null)
                return;

            // we want the tab to be repainted
            last_item_count_while_current_view_ = item_count;
            tab_parent.Text += " ";
        }

        private void on_change(filter.change_type change) {
            logger.Debug("[view] change: " + change + "on " + name);
            switch (change) {
            case filter.change_type.new_lines:
                break;
            case filter.change_type.changed_filter:
                break;
            default:
                throw new ArgumentOutOfRangeException("change", change, null);
            }
            this.async_call(refresh);
        }

        public void refresh() {
            if (log_ == null)
                return; // not set yet

            bool needs_ui_update = model_.needs_ui_update;
            bool needs_scroll = needs_scroll_to_last() && !needs_ui_update;

            model_.refresh();
            int new_item_count = item_count;
            filter_.compute_matches(log_);

            if (is_current_view)
                last_item_count_while_current_view_ = new_item_count;

            if (old_item_count_ == new_item_count && !needs_ui_update)
                return; // nothing changed

            if (needs_ui_update) {
                Enabled = true;
                Cursor = Cursors.IBeam;
                Focus();
            }

            model_.needs_ui_update = false;
            bool more_items = old_item_count_ < new_item_count;
            old_item_count_ = new_item_count;

            refresh_visible_columns();
            update_x_of_y();

            if (needs_ui_update) {
                go_to_closest_line(sel_line_idx_before_set_filter_, select_type.notify_parent);
                try_set_visible_row_y_offset(sel_row_idx, sel_y_offset_before_set_filter_);
                edit.update_ui();
                lv_parent.after_set_filter_update();
            }

            list.Refresh();
            if (needs_scroll && more_items && !needs_ui_update)
                go_last();
        }


        // if first item is true, we found colors, and the colors are item2 & 3
        private Tuple<bool,Color,Color> has_found_colors(int row_idx, log_view other_log, bool is_sel) {
            var i = item_at(row_idx) as full_log_match_item;

            int line_idx = i.match.line_idx;
            match_item found_line = null;
            switch (app.inst.syncronize_colors) {
            case app.synchronize_colors_type.none: // nothing to do
                return new Tuple<bool, Color, Color>(true, filter_line.font_info.default_font.fg, filter_line.font_info.default_font.bg);
            case app.synchronize_colors_type.with_current_view:
                found_line = other_log.filter_.matches.binary_search(line_idx).Item1 as match_item;
                if (found_line != null) 
                    return new Tuple<bool, Color, Color>(true, found_line.fg(this), found_line.bg(this));
                break;
            case app.synchronize_colors_type.with_all_views:
                found_line = other_log.filter_.matches.binary_search(line_idx).Item1 as match_item;
                if (found_line != null) {
                    Color bg = found_line.bg(this), fg = found_line.fg(this);
                    if (app.inst.sync_colors_all_views_gray_non_active && !is_sel)
                        fg = util.grayer_color(fg);
                    return new Tuple<bool, Color, Color>(true, fg, bg);
                }
                break;
            default:
                Debug.Assert(false);
                break;
            }
            return new Tuple<bool, Color, Color>(false, util.transparent, util.transparent);
        }

        public Tuple<Color,Color> update_colors_for_line(int row_idx, List<log_view> other_logs, int sel_idx) {
            Debug.Assert(other_logs.Count > 0 && sel_idx < other_logs.Count);

            Tuple<bool, Color, Color> found_colors = null;
            switch (app.inst.syncronize_colors) {
            case app.synchronize_colors_type.none:
                found_colors = has_found_colors(row_idx, other_logs[0], false);
                break;
            case app.synchronize_colors_type.with_current_view:
                found_colors = has_found_colors(row_idx, other_logs[sel_idx], true);
                break;
            case app.synchronize_colors_type.with_all_views:
                found_colors = has_found_colors(row_idx, other_logs[sel_idx], true);
                if (found_colors.Item1)
                    break;
                for (int idx = 0; idx < other_logs.Count; ++idx)
                    if (idx != sel_idx) {
                        found_colors = has_found_colors(row_idx, other_logs[idx], false);
                        if (found_colors.Item1)
                            break;
                    }
                break;
            default:
                Debug.Assert(false);
                break;
            }

            if ( found_colors != null && found_colors.Item1)
                return new Tuple<Color, Color>(found_colors.Item2, found_colors.Item3);

            return new Tuple<Color, Color>( filter_line.font_info.full_log_gray.fg, filter_line.font_info.full_log_gray.bg );
        }

        // returns the rows that are visible
        private Tuple<int, int> visible_row_indexes() {
            int PAD = 5;
            var top = list.GetItemAt(PAD, list.HeaderControl.ClientRectangle.Height + PAD);
            if (top == null)
                return new Tuple<int, int>(0, 0);

            int top_idx = top.Index;
            int height = list.Height - list.HeaderControl.ClientRectangle.Height;
            int row_height = top.Bounds.Height;
            int rows_per_page = height / row_height;

            int bottom_idx = top_idx + rows_per_page;
            if (top_idx < 0)
                top_idx = 0;

            return new Tuple<int, int>(top_idx, bottom_idx);
        }

        // y offset - how many rows are visible, on top of this row?
        private int visible_row_y_offset(int row_idx) {
            if (model_.item_count < 1)
                return 0;

            const int PAD = 5;
            var top = list.GetItemAt(PAD, list.HeaderControl.ClientRectangle.Height + PAD);
            int row_height = top.Bounds.Height;

            var row = list.GetItem(row_idx).Bounds;
            if (row.Height == 0 || row.Width == 0) {
                // in this case, this row is not visible at this time
                Debug.Assert(false);
                return 0;                
            }

            int offset = (row.Y - top.Bounds.Y) / row_height;
            Debug.Assert(offset >= 0);
            return offset;
        }

        // tries to set the visible "y offset"
        // y offset - how many rows are visible, on top of this row?
        private void try_set_visible_row_y_offset(int row_idx, int y_offset) {
            if (row_idx < 0)
                // there was nothing selected
                return;

            if (row_idx <= y_offset) {
                // in this case, we can't make this possible, since there are not enough rows before this
                ensure_row_visible(0);
                return;
            }

            var visible = visible_row_indexes();
            int top_idx = row_idx - y_offset;
            int bottom_idx = top_idx + visible.Item2 - visible.Item1 - 1;
            ensure_row_visible(top_idx);
            ensure_row_visible(bottom_idx);
        }

        // specifies the name of selected view (on the right pane)
        public void set_view_selected_view_name(string name) {
            if (selected_view_ == name)
                // we already have this set correctly
                return;
            selected_view_ = name;
            list.Refresh();
        }

        private void go_last() {
            var count = item_count;
            if (count > 0) {
                if (ensure_row_visible(count - 1))
                    select_row_idx(count - 1, select_type.do_not_notify_parent);
                else
                // in this case, we failed to go to the last item - try again ASAP
                    util.postpone(go_last, 10);
            }
        }

        private void list_FormatCell_1(object sender, FormatCellEventArgs e) {
        }

        private void list_FormatRow_1(object sender, FormatRowEventArgs e) {
            match_item i = e.Item.RowObject as match_item;
            if (i != null) {
                e.Item.BackColor = i.bg(this);
                e.Item.ForeColor = i.fg(this);
            }
        }

        public enum select_type {
            notify_parent,
            do_not_notify_parent
        }

        // by default, notify parent
        private select_type select_nofify_ = select_type.notify_parent;

        private void select_row_idx(int row_idx, select_type notify) {
            if (sel_row_idx == row_idx)
                return; // already selected

            // 1.0.67+ - there's a bug in objectlistview - if we're not the current view, we can't really select a row
            if (!is_current_view && !is_full_log)
                return;

            select_nofify_ = notify;
            if (row_idx >= 0 && row_idx < item_count) {
                logger.Debug("[view] " + name + " sel=" + row_idx);
                list.SelectedIndex = row_idx;
                update_x_of_y();
            }
            select_nofify_ = select_type.notify_parent;
        }

        // only called by smart edit on backspace
        internal void select_cell(int row_idx, int cell_idx) {
            cur_col_ = cell_idx;
            select_row_idx(row_idx, select_type.notify_parent);
            lv_parent.after_search();
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e) {
            edit.update_ui();
            int sel = sel_row_idx;
            if (sel < 0)
                return;

            if (select_nofify_ == select_type.notify_parent) {
                int line_idx = item_at(sel).match.line_idx;
                lv_parent.on_sel_line(this, line_idx);
            }

            if (is_editing_ && app.inst.edit_mode == app.edit_mode_type.with_right_arrow) {
                if (sel != editing_row_) {
                    is_editing_ = false;
                    edit.update_ui();
                }
            }
        }

        private bool is_row_visible(int row_idx) {
            var visible = visible_row_indexes();
            // 1.1.20+ - if it's the last visible line (it's only shown partially - in that case, force it into view
            return (visible.Item1 <= row_idx && visible.Item2 - 1 >= row_idx);
        }


        private bool ensure_row_visible(int row_idx) {
            if (is_row_visible(row_idx))
                return true;
            var visible = visible_row_indexes();
            logger.Debug("[view] visible indexes for " + name + " : " + visible.Item1 + " - " + visible.Item2);
            // 1.1.15+ note : this sometimes flickers, we want to avoid this as much as possible

            if (row_idx >= list.GetItemCount())
                // can happen if list isn't fully refreshed
                return false;

            try {
                list.EnsureVisible(row_idx);
                return true;
            } catch {
                return false;
            }
        }

        public void go_to_row(int row_idx, select_type notify) {
            if (row_idx >= item_count)
                return;

            select_row_idx(row_idx, notify);
            if (is_row_visible(row_idx))
                // already visible
                return;

            int rows = list.Height / list.RowHeight;
            int bottom_idx = row_idx + rows / 2;
            if (bottom_idx >= item_count)
                bottom_idx = item_count - 1;
            int top_idx = bottom_idx - rows;
            if (top_idx < 0)
                top_idx = 0;
            // we want to show the line in the *middle* of the control (height-wise)
            if (top_idx < list.GetItemCount())
                ensure_row_visible(top_idx);
            if (bottom_idx < list.GetItemCount())
                ensure_row_visible(bottom_idx);
        }

        public void rename_view() {
            old_view_name_ = name;
            show_name = true;
            viewName.Focus();
        }

        private void cancel_rename() {
            name = old_view_name_;
            show_name = false;
        }

        public bool contains_line(int line_idx) {
            return line_to_row(line_idx) >= 0;
        }
        // -1 means it's not found
        public int line_to_row(int line_idx) {
            return filter_.matches.binary_search(line_idx).Item2;
        }

        public void go_to_closest_line(int line_idx, select_type notify) {
            if (filter_.match_count < 1)
                return;

            var closest = model_.binary_search_closest(line_idx);
            if (closest.Item2 >= 0)
                go_to_row(closest.Item2, notify);
        }

        public bool filter_matches_line(int line_idx) {
            // important: here, we actually want to use the filter, instead of the model
            var closest = filter_.matches.binary_search_closest(line_idx);
            if (closest.Item2 >= 0)
                return closest.Item1.line_idx == line_idx;
            else
                return false;
        }

        public void go_to_closest_time(DateTime time) {
            var closest = filter_.matches.binary_search_closest(time);
            if (closest.Item2 >= 0)
                go_to_row(closest.Item2, select_type.notify_parent);
        }

        public void offset_closest_time(int time_ms, bool forward) {
            if (filter_.match_count < 1)
                return;
            int sel = sel_row_idx;
            if (sel < 0)
                sel = 0; // just in case we haven't selected anything - start from beginning
            var i = item_at(sel);
            var time = i.match.line.time;

            if (time != DateTime.MinValue) {
                time = time.AddMilliseconds(forward ? time_ms : -time_ms);
                go_to_closest_time(time);
            } else
            // in this case, there is no time logged, or it was invalid
                util.beep(util.beep_type.err);
        }

        public void offset_closest_line(int offset, bool forward) {
            int line_idx = sel_line_idx;
            if (line_idx < 0)
                line_idx = 0; // just in case we haven't selected anything - start from beginning
            line_idx += forward ? offset : -offset;
            go_to_closest_line(line_idx, select_type.notify_parent);
        }

        public int line_count {
            get { return item_count; }
        }

        public int filter_row_count {
            get { return filter_.row_count; }
        }

        public bool pad_name_on_left {
            get { return pad_name_on_left_; }
            set {
                if (pad_name_on_left_ != value) {
                    pad_name_on_left_ = value;
                    update_x_of_y();
                }
            }
        }

        public bool show_name_in_header {
            get { return show_name_in_header_; }
            set {
                show_name_in_header_ = value;
                update_x_of_y();
            }
        }

        public bool show_header {
            get { return show_header_; }
            set {
                bool old_show_header = show_name_ || show_header_;
                show_header_ = value;
                bool new_show_header = show_name_ || show_header_;
                if (old_show_header != new_show_header)
                    update_show_header();
            }
        }

        public bool show_name {
            get { return show_name_; }
            set {
                bool old_show_header = show_name_ || show_header_;

                if (value == true && name == FULLLOG_NAME)
                    // we never allow showing the name of the full log
                    return;

                if (show_name_ != value) {
                    show_name_ = value;
                    update_show_name();

                    bool new_show_header = show_name_ || show_header_;
                    if (old_show_header != new_show_header)
                        update_show_header();
                }
            }
        }


        // returns how many lines this filter has found
        public int filter_row_match_count(int row_idx) {
            Debug.Assert(row_idx < filter_.row_count);
            int count = 0;
            int match_count = filter_.match_count;
            for (int line_idx = 0; line_idx < match_count; ++line_idx) {
                var match = filter_.match_at(line_idx);
                if (match == null)
                    break; // filter got updated on the other thread
                if (match.matches.Length > row_idx)
                    if (match.matches[row_idx])
                        ++count;
            }
            return count;
        }

        [DllImport("user32")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        private void list_CellToolTipShowing(object sender, ToolTipShowingEventArgs e) {
            if (e.ColumnIndex == msgCol.fixed_index())
                ShowWindow(e.ToolTipControl.Handle, 0);
        }

        private void log_view_Load(object sender, EventArgs e) {
        }

        private void list_KeyPress(object sender, KeyPressEventArgs e) {
            // suppress sound
            e.Handled = true;
        }

        public void search_for_text(search_form.search_for search) {
            cur_search_ = search;
            // as of 1.2.6, we mark the words visually
            list.Refresh();
        }

        // unmarks any previously marked rows
        public void unmark() {
            cur_filter_row_idx_ = -1;
            int count = filter_.match_count;
            for (int idx = 0; idx < count; ++idx) {
                match_item i = item_at(idx);
                i.override_fg = util.transparent;
                i.override_bg = util.transparent;
            }
            list.Refresh();
        }

        private msg_details_ctrl msg_details {
            get {
                foreach (var ctrl in parent.Controls)
                    if (ctrl is msg_details_ctrl)
                        return ctrl as msg_details_ctrl;
                return null;
            }
        }

        public void escape() {
            var msg_details = this.msg_details;
            if (msg_details != null && msg_details.visible()) {
                msg_details.force_hide(this);
                return;
            }

            if (edit.sel_text != "") {
                edit.escape();
                return;
            }

            if (cur_search_ != null) {
                cur_search_ = null;
                list.Refresh();
                return;
            }

            if (cur_filter_row_idx_ >= 0) {
                unmark();
                return;
            }

            if (app.inst.edit_mode != app.edit_mode_type.always && is_editing) {
                is_editing_ = false;
                edit.update_ui();
                return;
            }
        }


        public void search_next() {
            /* 1.2.7+   implemented f3/shift-f3 on smart-edit first
                        note: this will never replace search form -> since there you can have extra settings: regexes, case-sensitivity ,full word
            */

            // if user has selected something (smart edit), search for that
            // if user has made a find (ctrl-f), search for that
            // otherwise, search for selected filter (if any)

            string sel_text = edit.sel_text.ToLower();
            if (cur_search_ != null || sel_text != "")
                search_for_text_next();
            else if (cur_filter_row_idx_ >= 0)
                search_for_next_match(cur_filter_row_idx_);
            lv_parent.after_search();
        }

        public void search_prev() {
            /* 1.2.7+   implemented f3/shift-f3 on smart-edit first
                        note: this will never replace search form -> since there you can have extra settings: regexes, case-sensitivity ,full word
            */

            // if user has selected something (smart edit), search for that
            // if user has made a find (ctrl-f), search for that
            // otherwise, search for selected filter (if any)

            string sel_text = edit.sel_text.ToLower();
            if (cur_search_ != null || sel_text != "")
                search_for_text_prev();
            else if (cur_filter_row_idx_ >= 0)
                search_for_prev_match(cur_filter_row_idx_);
            lv_parent.after_search();
        }

        // note: starts from the next row, or, if on row zero -> starts from row zero
        public void search_for_text_first() {
            if (item_count < 1)
                return;
            Debug.Assert(cur_search_ != null);
            if (cur_search_ == null)
                return;

            // make sure f3/shift-f3 will work on the current search (cur_search_), not on the currently selected word(s)
            edit.escape();

            select_row_idx(0, select_type.notify_parent);
            match_item i = item_at(0);
            bool include_row_zero = sel_row_idx == 0 || sel_row_idx == -1;
            if (include_row_zero && string_search.matches(i.match.line.part(info_type.msg), cur_search_)) {
                // line zero contains the text already
                ensure_row_visible(0);
                lv_parent.after_search();
            } else
                search_for_text_next();
        }

        private void search_for_text_next() {
            int count = item_count;
            if (count < 1)
                return;

            string sel_text = edit.sel_text.ToLower();
            Debug.Assert(cur_search_ != null || sel_text != "");
            if (cur_search_ == null && sel_text == "")
                return;

            int found = -1;
            int next_row = sel_row_idx >= 0 ? sel_row_idx + 1 : 0;
            for (int idx = next_row; idx < count && found < 0; ++idx) {
                // 1.2.7+ - if user has selected something, search for that
                if (sel_text != "") {
                    string cur = list.GetItem(idx).GetSubItem( visible_column(cur_col_)).Text;
                    if (cur.ToLower().Contains(sel_text))
                        found = idx;
                    continue;
                }

                match_item i = item_at(idx);
                if (string_search.matches(i.match.line.part(info_type.msg), cur_search_))
                    found = idx;
            }

            if (found >= 0) {
                select_row_idx(found, select_type.notify_parent);
                ensure_row_visible(found);
            } else
                util.beep(util.beep_type.err);
        }

        private void search_for_text_prev() {
            int count = item_count;
            if (count < 1)
                return;
            string sel_text = edit.sel_text.ToLower();
            Debug.Assert(cur_search_ != null || sel_text != "");
            if (cur_search_ == null && sel_text == "")
                return;

            int found = -1;
            int prev_row = sel_row_idx >= 0 ? sel_row_idx - 1 : count - 1;
            for (int idx = prev_row; idx >= 0 && found < 0; --idx) {
                // 1.2.7+ - if user has selected something, search for that
                if (sel_text != "") {
                    string cur = list.GetItem(idx).GetSubItem( visible_column(cur_col_)).Text;
                    if (cur.ToLower().Contains(sel_text))
                        found = idx;
                    continue;
                }

                match_item i = item_at(idx);
                if (string_search.matches(i.match.line.part(info_type.msg), cur_search_))
                    found = idx;
            }
            if (found >= 0) {
                select_row_idx(found, select_type.notify_parent);
                ensure_row_visible(found);
            } else
                util.beep(util.beep_type.err);
        }

        public void mark_match(int filter_row_idx, Color fg, Color bg) {
            cur_filter_row_idx_ = filter_row_idx;
            bool needs_refresh = false;
            int count = filter_.match_count;
            for (int idx = 0; idx < count; ++idx) {
                match_item i = item_at(idx);
                bool is_match = filter_row_idx >= 0 && i.match.matches.Length > filter_row_idx && i.match.matches[filter_row_idx];
                bool needs_change = (is_match && (i.override_fg.ToArgb() != fg.ToArgb() || i.override_bg.ToArgb() != bg.ToArgb())) || (!is_match && (i.override_fg != util.transparent || i.override_bg != util.transparent));

                if (needs_change) {
                    i.override_fg = is_match ? fg : util.transparent;
                    i.override_bg = is_match ? bg : util.transparent;
                    needs_refresh = true;
                }
            }
            if (needs_refresh)
                list.Refresh();
        }

        private void search_for_next_match(int filter_row_idx) {
            int count = filter_.match_count;
            if (count < 1)
                return;
            int found = -1;
            int next_row = sel_row_idx >= 0 ? sel_row_idx + 1 : 0;
            for (int idx = next_row; idx < count && found < 0; ++idx) {
                match_item i = item_at(idx);
                bool is_match = i.match.matches.Length > filter_row_idx && i.match.matches[filter_row_idx];
                if (is_match)
                    found = idx;
            }

            if (found >= 0) {
                select_row_idx(found, select_type.notify_parent);
                ensure_row_visible(found);
            } else
                util.beep(util.beep_type.err);
        }

        private void search_for_prev_match(int filter_row_idx) {
            if (filter_.match_count < 1)
                return;
            int found = -1;
            int prev_row = sel_row_idx >= 0 ? sel_row_idx - 1 : filter_.match_count - 1;
            for (int idx = prev_row; idx >= 0 && found < 0; --idx) {
                match_item i = item_at(idx);
                bool is_match = i.match.matches.Length > filter_row_idx && i.match.matches[filter_row_idx];
                if (is_match)
                    found = idx;
            }

            if (found >= 0) {
                select_row_idx(found, select_type.notify_parent);
                ensure_row_visible(found);
            } else
                util.beep(util.beep_type.err);
        }

        public export_text export(List<int> indices, bool msg_only) {
            export_text export = new export_text();

            int row_idx = 0;
            foreach (int idx in indices) {
                match_item i = item_at(idx);

                int visible_idx = 0;
                string font = list.Font.Name;
                for (int column_idx = 0; column_idx < list.AllColumns.Count; ++column_idx) {
                    bool do_print = list.AllColumns[column_idx].IsVisible;
                    if (msg_only)
                        do_print = column_idx == msgCol.fixed_index();
                    if (do_print) {
                        string txt = cell_value(i, column_idx);
                        export_text.cell c = new export_text.cell(row_idx, visible_idx, txt) {fg = i.fg(this), bg = i.bg(this), font = font, font_size = 7};
                        export.add_cell(c);
                        ++visible_idx;
                    }
                }
                ++row_idx;
            }

            return export;
        }

        public void copy_to_clipboard() {
            var sel = selected_indices_array();
            if (sel.Count < 1)
                return;

            var export = this.export(sel, true);
            string html = export.to_html(), text = export.to_text();
            clipboard_util.copy(html, text);
        }

        public void copy_full_line_to_clipboard() {
            var sel = selected_indices_array();
            if (sel.Count < 1)
                return;

            var export = this.export(sel, false);
            string html = export.to_html(), text = export.to_text();
            clipboard_util.copy(html, text);
        }

        public void set_bookmarks(List<int> line_idxs) {
            var old = bookmarks_.Except(line_idxs);
            var new_ = line_idxs.Except(bookmarks_);

            bookmarks_ = line_idxs;
            bookmarks_.Sort();

            foreach (int idx in old) {
                var row = row_by_line_idx(idx);
                if (row != null) 
                    list.RefreshItem(row);
            }
            foreach (int idx in new_) {
                var row = row_by_line_idx(idx);
                if (row != null) 
                    list.RefreshItem(row);                
            }
        }

        public void next_bookmark() {
            int start = sel_row_idx >= 0 ? sel_line_idx + 1 : 0;
            int mark = bookmarks_.FirstOrDefault(line => line >= start && row_by_line_idx(line) != null);
            if (mark == 0)
                if (mark < start || !bookmarks_.Contains(mark))
                    // in this case, we did not find anything and got returned default (0)
                    mark = -1;

            if (mark >= 0) {
                int idx = row_by_line_idx(mark).Index;
                select_row_idx(idx, select_type.notify_parent);
                ensure_row_visible(idx);
            } else
                util.beep(util.beep_type.err);
        }

        public void prev_bookmark() {
            if (filter_.match_count < 1)
                return;

            int start = sel_row_idx >= 0 ? sel_line_idx - 1 : item_at(sel_row_idx).match.line_idx;
            int mark = bookmarks_.LastOrDefault(line => line <= start && row_by_line_idx(line) != null);
            if (mark == 0)
                if (!bookmarks_.Contains(mark))
                    // in this case, we did not find anything and got returned default (0)
                    mark = -1;

            if (mark >= 0) {
                int idx = row_by_line_idx(mark).Index;
                select_row_idx(idx, select_type.notify_parent);
                ensure_row_visible(idx);
            } else
                util.beep(util.beep_type.err);
        }

        private void list_CellOver(object sender, CellOverEventArgs e) {
            e.Handled = true;
        }

        public void increase_font(int size) {
            int new_size = font_size_ + size;
            if (new_size >= 6 && new_size < 20) {
                font_size_ = new_size;
                load_font();
            }
        }


        public void show_row(ui_info.show_row_type show) {
            show_row_ = show;
            if ( visible_columns_refreshed_ >= MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS)
                show_row_impl(show);
        }

        private void show_row_impl(ui_info.show_row_type show) {
            if (full_widths_.Count < 1)
                // save_to them now
                for (int idx = 0; idx < list.AllColumns.Count; ++idx)
                    full_widths_.Add(list.AllColumns[idx].Width);

            logger.Info("[view] showing rows - " + name + " = " + show);

            switch (show) {
            case ui_info.show_row_type.msg_only:
                for (int idx = 0; idx < list.AllColumns.Count; ++idx)
                    show_column( list.AllColumns[idx], full_widths_[idx], idx == msgCol.fixed_index() || idx == lineCol.fixed_index() );
                break;
            case ui_info.show_row_type.msg_and_view_only:
                for (int idx = 0; idx < list.AllColumns.Count; ++idx)
                    show_column( list.AllColumns[idx], full_widths_[idx], idx == msgCol.fixed_index() || idx == lineCol.fixed_index() || idx == viewCol.fixed_index() );
                break;
            case ui_info.show_row_type.full_row:
                for (int idx = 0; idx < list.AllColumns.Count; ++idx)
                    show_column( list.AllColumns[idx], full_widths_[idx], full_widths_[idx] > 0 );
                break;
            default:
                Debug.Assert(false);
                break;
            }

            list.RebuildColumns();
        }


        public void scroll_up() {
            if (item_count < 1)
                return;

            int sel = sel_row_idx;
            if (sel < 0)
                sel = 0; // assume we start from the top

            var r = list.GetItem(sel).Bounds;
            if (r.Bottom + r.Height < list.ClientRectangle.Height) {
                list.LowLevelScroll(0, -r.Height);
                ensure_row_visible(sel);
            } else
                on_action(action_type.arrow_up);
        }

        public void scroll_down() {
            if (item_count < 1)
                return;

            int sel = sel_row_idx;
            if (sel < 0)
                sel = 0; // assume we start from the top

            var r = list.GetItem(sel).Bounds;
            if (r.Bottom + r.Height < list.ClientRectangle.Height) {
                list.LowLevelScroll(0, r.Height);
                ensure_row_visible(sel);
            } else
                on_action(action_type.arrow_down);
        }

        private void list_Enter(object sender, EventArgs e) {
            update_background();
            edit.update_ui();
        }

        private void list_Leave(object sender, EventArgs e) {
            // note: we might actually lose focus to the edit box - in this case, we don't really need to update anything
            util.postpone(update_background, 10);
        }

        internal void update_background() {
            var focus = win32.focused_ctrl();
            bool here = focus == list || focus == edit;

            var color = here ? Color.DarkSlateGray : Color.White;
            if (BackColor.ToArgb() != color.ToArgb())
                BackColor = color;
        }

        public void set_focus() {
            update_background();
            list.Focus();
        }

        public bool has_focus {
            get {
                var focus = win32.focused_ctrl();
                bool here = focus == list || focus == edit || focus == viewName;
                return here;
            }
        }

        public bool has_find {
            get {
                if (smart_edit_sel_text != "")
                    return true;
                if (cur_search_ != null)
                    return true;
                if (cur_filter_row_idx_ >= 0)
                    return true;
                return false;
            }
        }

        public export_text export() {
            export_text export = new export_text();

            int count = filter_.match_count;
            for (int idx = 0; idx < count; ++idx) {
                match_item i = item_at(idx);

                int visible_idx = 0;
                string font = list.Font.Name;
                for (int column_idx = 0; column_idx < list.AllColumns.Count; ++column_idx) {
                    if (list.AllColumns[column_idx].IsVisible) {
                        string txt = cell_value(i, column_idx);
                        export_text.cell c = new export_text.cell(idx, visible_idx, txt) {fg = i.fg(this), bg = i.bg(this), font = font, font_size = 7};
                        export.add_cell(c);
                        ++visible_idx;
                    }
                }
            }

            return export;
        }

        public bool is_editing {
            get {
                switch (app.inst.edit_mode) {
                case app.edit_mode_type.always:
                    return true;

                case app.edit_mode_type.with_space:
                    return is_editing_;

                case app.edit_mode_type.with_right_arrow:
                    return is_editing_;
                default:
                    Debug.Assert(false);
                    return false;
                }
            }
        }

        internal search_form.search_for cur_search {
            get { return cur_search_; }
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            bool is_renaming = win32.focused_ctrl() == viewName;

            if (!is_editing) {
                // see if the current key will start editing
                if (keyData == Keys.Space && app.inst.edit_mode == app.edit_mode_type.with_space) {
                    is_editing_ = true;
                    cur_col_ = msgCol.fixed_index();
                    edit.update_ui();
                    return true;
                } else if (keyData == Keys.Right && app.inst.edit_mode == app.edit_mode_type.with_right_arrow) {
                    is_editing_ = true;
                    editing_row_ = sel_row_idx;
                    cur_col_ = msgCol.fixed_index();
                    edit.go_to_char(0);
                    return true;
                }
            }

            if (is_editing) {
                if (keyData == Keys.Back) {
                    if (!is_renaming) {
                        edit.backspace();
                        return true;
                    }
                }
                if (keyData == Keys.Escape) {
                    if (is_renaming)
                        cancel_rename();
                    else
                        escape();
                    return true;
                }

                if (keyData == Keys.Return) {
                    if (is_renaming)
                        show_name = false;
                    return true;
                }

                // see if user wants to turn off editing
                switch (app.inst.edit_mode) {
                case app.edit_mode_type.always:
                    // in this case, we're always in edit-mode
                    break;

                // toggle this edit mode OFF
                case app.edit_mode_type.with_space:
                    if (keyData == Keys.Space) {
                        is_editing_ = false;
                        edit.update_ui();
                        return true;
                    }
                    break;

                // moving to another line will toggle this edit mode off
                case app.edit_mode_type.with_right_arrow:
                    switch (keyData) {
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.PageUp:
                    case Keys.PageDown:
                    case Keys.Home:
                    case Keys.End:
                        is_editing_ = false;
                        edit.update_ui();
                        break;
                    }
                    break;
                }
            }

            if (is_editing) {
                bool is_left_or_right = keyData == Keys.Left || keyData == Keys.Right || keyData == (Keys.Left | Keys.Alt) || keyData == (Keys.Right | Keys.Alt);
                if (is_left_or_right) {
                    bool is_alt_left_or_right = keyData == (Keys.Left | Keys.Alt) || keyData == (Keys.Right | Keys.Alt);
                    bool is_left = keyData == Keys.Left || keyData == (Keys.Left | Keys.Alt);
                    if (edit.Visible && (edit.SelectionLength == 0 || is_alt_left_or_right)) {
                        bool can_move = (is_left && edit.SelectionStart == 0 && edit.SelectionLength == 0) || (!is_left && edit.SelectionStart == edit.TextLength) || is_alt_left_or_right;

                        if (can_move) {
                            for (int column_idx = 0; column_idx < list.AllColumns.Count; ++column_idx) {
                                int next = is_left ? (cur_col_ - column_idx - 1 + list.AllColumns.Count) % list.AllColumns.Count : (cur_col_ + column_idx + 1) % list.AllColumns.Count;
                                if (list.AllColumns[next].IsVisible) {
                                    cur_col_ = next;
                                    break;
                                }
                            }
                            util.postpone(() => edit.go_to_char(0), 1);
                            return true;
                        }
                    }

                    util.postpone(edit.force_update_sel, 1);
                }

                bool is_shift_right = keyData == (Keys.Right | Keys.Shift);
                bool is_shift_left = keyData == (Keys.Left | Keys.Shift);
                if (is_shift_left && edit.Visible) {
                    edit.sel_to_left();
                    return true;
                }
                if (is_shift_right && edit.Visible) {
                    edit.sel_to_right();
                    return true;
                }

                switch (keyData) {
                case Keys.Up:
                    on_action(action_type.arrow_up);
                    return true;
                case Keys.Down:
                    on_action(action_type.arrow_down);
                    return true;
                case Keys.PageUp:
                    on_action(action_type.pageup);
                    return true;
                case Keys.PageDown:
                    on_action(action_type.pagedown);
                    return true;

                case Keys.Home:
                    if (edit.SelectionStart == 0 && edit.SelectionLength == 0) {
                        on_action(action_type.home);
                        return true;
                    }
                    break;
                case Keys.End:
                    if (edit.SelectionStart == edit.TextLength) {
                        on_action(action_type.end);
                        return true;
                    }
                    break;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void list_CellClick(object sender, CellClickEventArgs e) {
            if (!is_editing)
                return;

            int col_idx = e.ColumnIndex;
            if (col_idx >= 0 && e.RowIndex >= 0) {
                var mouse = list.PointToClient(Cursor.Position);
                using (Graphics g = CreateGraphics()) {
                    string text = list.GetItem(e.RowIndex).GetSubItem(e.ColumnIndex).Text;
                    var widths = render_.text_widths(g, text);
                    int offset_x = list.GetItem(e.RowIndex).GetSubItemBounds(e.ColumnIndex).X;

                    for (int i = 0; i < widths.Count; ++i)
                        widths[i] += offset_x;

                    int char_idx = widths.FindLastIndex(x => x < mouse.X);
                    if (widths.Last() < mouse.X)
                        char_idx = widths.Count;

                    cur_col_ = col_idx;
                    if (char_idx < 0)
                        char_idx = 0;
                    edit.go_to_char(char_idx);
                    edit.after_click();
                }
            }
        }

        private void on_edit_sel_changed() {
            // logger.Debug("[lv] sel= [" + edit.sel_text + "]");
            list.Refresh();
        }

        // returns the row, or -1 on failure
        private bool can_find_text_at_row(string txt, int row, int col) {
            string col_text = list.GetItem(row).GetSubItem( visible_column(col)).Text.ToLower();
            int pos = col_text.IndexOf(txt);
            return pos >= 0;
        }

        // note: searches in the current column
        private void search_ahead(string txt) {
            Debug.Assert(txt == txt.ToLower());

            int count = item_count;
            int max = Math.Min(sel_row_idx + SEARCH_AROUND_ROWS, count);
            int min = Math.Max(sel_row_idx - SEARCH_AROUND_ROWS, 0);

            // note: even if we search all columns, we first search ahead/before for the selected column - then, the rest
            int found_row = -1;
            if (app.inst.edit_search_after)
                for (int cur_row = sel_row_idx + 1; cur_row < max && found_row < 0; ++cur_row)
                    if (can_find_text_at_row(txt, cur_row, cur_col_))
                        found_row = cur_row;

            if (app.inst.edit_search_before)
                for (int cur_row = sel_row_idx - 1; cur_row >= min && found_row < 0; --cur_row)
                    if (can_find_text_at_row(txt, cur_row, cur_col_))
                        found_row = cur_row;

            if (app.inst.edit_search_all_columns) {
                if (app.inst.edit_search_after)
                    for (int cur_row = sel_row_idx + 1; cur_row < max && found_row < 0; ++cur_row)
                        for (int col_idx = 0; col_idx < list.AllColumns.Count && found_row < 0; ++col_idx)
                            if (col_idx != cur_col_ && list.AllColumns[col_idx].IsVisible)
                                if (can_find_text_at_row(txt, cur_row, col_idx)) {
                                    found_row = cur_row;
                                    cur_col_ = col_idx;
                                }

                if (app.inst.edit_search_before)
                    for (int cur_row = sel_row_idx - 1; cur_row >= min && found_row < 0; --cur_row)
                        for (int col_idx = 0; col_idx < list.AllColumns.Count && found_row < 0; ++col_idx)
                            if (col_idx != cur_col_ && list.AllColumns[col_idx].Width > 0)
                                if (can_find_text_at_row(txt, cur_row, col_idx)) {
                                    found_row = cur_row;
                                    cur_col_ = col_idx;
                                }
            }

            if (found_row >= 0) {
                go_to_row(found_row, select_type.notify_parent);
                edit.update_ui();
                edit.go_to_text(txt);
                lv_parent.after_search();
            }
        }

        private void list_Scroll(object sender, ScrollEventArgs e) {
            util.postpone(edit.update_ui, 1);
        }

        private void list_MouseClick(object sender, MouseEventArgs e) {
        }

        public void do_right_click_via_key() {
            right_click_.right_click_at_caret();
        }

        private void list_MouseDown(object sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
                right_click_.right_click();
        }
    }

}
