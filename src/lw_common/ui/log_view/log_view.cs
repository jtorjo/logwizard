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
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using lw_common;
using lw_common.ui.format;
using LogWizard;

namespace lw_common.ui
{
    public partial class log_view : UserControl, IDisposable
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // how many rows to search ahead (smart search)
        private const int SEARCH_AROUND_ROWS = 100;

        public const string FULLLOG_NAME = "__all_this_is_fulllog__";
        // 1.6.13+ for settings - this contains the settings for all views
        private const string ALL_VIEWS = "__all_views__";

        private Form parent;
        private readonly filter filter_ ;
        private log_reader log_ = null;

        // only for showing the View(s) column correctly
        private string selected_view_ = null;

        private int last_item_count_while_current_view_ = 0;

        private Font non_bold_ = null, bold_ = null;


        // the reason we have this class - is for memory eficiency - since all views (except full log) don't need the info here


        private readonly log_view_render render_;

        private log_view_data_source model_ = null;

        // lines that are bookmarks (sorted by index)
        private List<int> bookmarks_ = new List<int>();

        private int font_size_ = 9; // default font size

        private bool pad_name_on_left_ = false;

        private bool show_name_in_header_ = false;
        private bool show_header_ = true;
        private bool show_name_ = true;

        private int old_item_count_ = 0;

        // it's the subitem column that is currently selected (for smart readonly editing)
        private int cur_col_ = 0;

        // in case the search yielded a column that is visible only on the description pane
        private int search_found_col_ = -1;

        private bool is_editing_ = false;
        private int editing_row_ = -1;

        private search_for cur_search_ = null;
        private int cur_filter_row_idx_ = -1;

        private log_view_right_click right_click_;

        // in case the user edits and presses Escape
        private string old_view_name_ = "";

        private int sel_y_offset_before_set_filter_ = 0;
        private int sel_line_idx_before_set_filter_ = 0;

        private int ignore_change_ = 0;

        private bool is_changing_column_width_ = false;

        private bool needs_scroll_ = false;

        // 1.5.4+ - refresh first time as well (even when there are no rows)
        // 1.6.13+ - used only in the full log
        internal int available_columns_refreshed_ = -1;
        internal bool use_previous_available_columns_ = false;

        // last time we received the last scroll event
        private DateTime scrolling_time_ = DateTime.MinValue;

        private int is_searching_ = 0;

        private column_formatter_array formatter_ = new column_formatter_array();

        public log_view(Form parent, string name)
        {
            Debug.Assert(parent is log_view_parent);

            logger.Debug("new log view " + name);
            filter_ = new filter(this.create_match_object);
            filter_.on_change = on_change;

            InitializeComponent();
            this.parent = parent;
            ++ignore_change_;
            viewName.Text = name;
            --ignore_change_;
            model_ = new log_view_data_source(this.list, this ) { name = name };
            list.VirtualListDataSource = model_;

            load_font();
            lv_parent.handle_subcontrol_keys(this);

            render_ = new log_view_render(this);
            foreach (var col in list.AllColumns) {
                col.Renderer = render_;
                col.Tag = new log_view_column_tag(this);
            }
            right_click_ = new log_view_right_click(this);

            cur_col_ = msgCol.fixed_index();
            edit.on_sel_changed = on_edit_sel_changed;
            edit.on_search_ahead = search_ahead;
            edit.init(this);
            edit.BringToFront();

            list.ColumnRightClick += list_ColumnRightClick;
            list.ColumnWidthChanged += List_on_column_width_changed;
            list.ColumnWidthChanging += List_on_column_width_changing;
            msgCol.FillsFreeSpace = !app.inst.show_horizontal_scrollbar;

            if ( util.is_debug)
                // testing
                app.inst.default_column_format = app.DEFAULT_COLUMN_SYNTAX;
            formatter_.load(app.inst.default_column_format);
        }

        private void List_on_column_width_changing(object sender, ColumnWidthChangingEventArgs e) {
            if (e.ColumnIndex == msgCol.Index)
                return;
            edit.Visible = false;
            is_changing_column_width_ = true;
        }

        // if the current column just got hidden, go to the closest to it
        private void update_cur_col() {
            int count = list.AllColumns.Count;
            if (list.AllColumns[cur_col_].is_visible())
                return;

            for (int offset = 0; offset < count; ++offset) {
                int before_idx = (cur_col_ + count - offset - 1) % count, after_idx = (cur_col_ + offset + 1) % count;
                var before = list.AllColumns[before_idx];
                var after = list.AllColumns[after_idx];
                if (after.is_visible()) {
                    cur_col_ = after_idx;
                    break;
                }
                if (before.is_visible()) {
                    cur_col_ = before_idx;
                    break;
                }
            }
        }

        private void List_on_column_width_changed(object sender, ColumnWidthChangedEventArgs e) {
            if (!is_changing_column_width_)
                return;
            if (e.ColumnIndex == msgCol.Index)
                return;

            is_changing_column_width_ = false;
            
            edit.update_ui();
            column_positions = log_view_show_columns.column_positions_as_string(this);
        }

        private void list_ColumnRightClick(object sender, ColumnClickEventArgs e) {
            int col_idx = e.Column; // ... index within the visible columns
            var cur_col = col_idx >= 0 ? list.GetColumn(col_idx) : msgCol;

            // show the Filter actions first (the rest will be shown in their respective categories)
            ContextMenuStrip menu = new ContextMenuStrip();
            for (int cur_col_idx = 0; cur_col_idx < list.AllColumns.Count; cur_col_idx++) {
                var col = list.AllColumns[cur_col_idx];
                if (col.Width > 0) {
                    bool is_visible = available_columns.Contains(log_view_cell.cell_idx_to_type(cur_col_idx));
                    if (!is_visible)
                        continue;
                    ToolStripMenuItem sub = new ToolStripMenuItem(col.Text);
                    sub.Checked = col.is_visible();
                    var c = col;
                    sub.Click += (a, ee) => toggle_column_visible(c, sub);
                    menu.Items.Add(sub);
                }
            }

            string cur_col_text = cur_col == msgCol ? "Message" : cur_col.Text;
            menu.Items.Add(new ToolStripSeparator());
            ToolStripMenuItem apply_to_all = new ToolStripMenuItem("Apply Settings To All Views");
            // for full log - always applies to self-only
            if (is_full_log)
                apply_column_settings_only_to_me = true;
            apply_to_all.Checked = !apply_column_settings_only_to_me;
            apply_to_all.Click += (a, ee) => toggle_apply_column_settings_to_all(apply_to_all);
            apply_to_all.Enabled = !is_full_log;
            menu.Items.Add(apply_to_all);
            menu.Items.Add(new ToolStripSeparator());
            var to_left = new ToolStripMenuItem("Move [" + cur_col_text + "] to Left (<-)");
            var to_right = new ToolStripMenuItem("Move [" + cur_col_text + "] to Right (->)");
            to_left.Click += (a,ee) => move_column_to_left(cur_col);
            to_right.Click += (a,ee) => move_column_to_right(cur_col);

            menu.Items.Add(to_left);
            menu.Items.Add(to_right);

            menu.Items.Add(new ToolStripSeparator());
            var edit_aliases = new ToolStripMenuItem("Edit Aliases...");
            menu.Items.Add(edit_aliases);
            edit_aliases.Click += (a,ee) => this.edit_aliases();

            var edit_log_settings = new ToolStripMenuItem("Edit Log Settings...");
            menu.Items.Add(edit_log_settings);
            edit_log_settings.Click += (a,ee) => lv_parent.edit_log_settings();

            menu.Closing += menu_Closing;
            edit.Visible = false;
            menu.Show(list, list.PointToClient(Cursor.Position));
        }

        private void edit_aliases() {
            lv_parent.on_edit_aliases();
            update_column_names();
        }

        internal void update_column_names() {
            if (filter.log == null)
                return;

            foreach (info_type i in Enum.GetValues(typeof (info_type))) {
                // for msg column - we use the default name
                if (i == info_type.max || i == info_type.msg || i == info_type.line || i == info_type.view)
                    continue;
                var col = log_view_cell.column(this, i);
                col.Text = filter_.log.aliases.friendly_name(i);
            }
        }

        private void menu_Closing(object sender, ToolStripDropDownClosingEventArgs e) {
            e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked;
            if (!e.Cancel) {
                edit.update_ui();
                column_positions = log_view_show_columns.column_positions_as_string(this);
                lv_parent.after_column_positions_change();
            }
        }

        public void on_column_positions_change() {
            log_view_show_columns.apply_column_positions(this);
        }

        private void toggle_apply_column_settings_to_all(ToolStripMenuItem sub) {
            apply_column_settings_only_to_me = !apply_column_settings_only_to_me;
            sub.Checked = !apply_column_settings_only_to_me;
        }

        private void toggle_column_visible(OLVColumn col, ToolStripMenuItem sub) {
            col.is_visible(!col.is_visible());
            sub.Checked = col.is_visible();
            list.RebuildColumns();
            update_cur_col();
        }

        private void move_column_to_left(OLVColumn col) {
            if ( col.DisplayIndex > 0)
                col.DisplayIndex = col.DisplayIndex - 1;
            foreach ( var c in list.AllColumns)
                c.LastDisplayIndex = c.DisplayIndex;
            list.Refresh();
        }
        private void move_column_to_right(OLVColumn col) {
            if ( col.DisplayIndex < list.Columns.Count - 1)
                col.DisplayIndex = col.DisplayIndex + 1;
            foreach ( var c in list.AllColumns)
                c.LastDisplayIndex = c.DisplayIndex;
            list.Refresh();
        }

        private void save_column_positions(string positions_str) {
            log_.write_settings.column_positions.set( apply_column_settings_only_to_me ? name : ALL_VIEWS, positions_str);
            update_cur_col();
        }

        // this is set while refreshing visible columns (log_view_show_columns)
        //
        // it contains the columns that are available for showing
        internal List<info_type> available_columns {
            get { 
                return log_ != null ? log_.settings.available_columns.get().Split(new [] { ","}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => (info_type) int.Parse(x)).ToList() : new List<info_type>(); 
            }
            set {
                if ( log_ != null)
                    log_.write_settings.available_columns.set( util.concatenate(value.Select(x => (int)x), ",") );
            }
        }

        internal bool apply_column_settings_only_to_me {
            get { return log_ != null && log_.settings.apply_column_positions_to_me.get(name); }
            set {
                if (log_ != null) {
                    log_.write_settings.apply_column_positions_to_me.set(name, value);
                    save_column_positions( column_positions);
                }
            }
        }

        // this depends on apply_column_settings_only_to_me!
        internal string column_positions {
            get {
                if (log_ == null)
                    return "";
                return log_.settings.column_positions.get( apply_column_settings_only_to_me ? name : ALL_VIEWS ) ;
            }
            set {
                if (column_positions == value)
                    return; // nothing changed

                if (log_ != null && value != "") 
                    save_column_positions(value);
            }
        }

        internal log_view_parent lv_parent {
            get {
                return parent as log_view_parent;
            }
        }

        internal filter filter {
            get { return filter_; }
        }

        private string last_search_status_ = "";
        public string search_status {
            get {
                string status = "";
                if (is_searching_ > 0)
                    status = "Seaching " + cur_search.friendly_name;
                else if (model_.is_running_filter)
                    status = "Running Filter";

                if (status != "") {
                    string suffix = new string('.', DateTime.Now.Second % 5);
                    status += " " + suffix;
                }
                bool was_seaching = last_search_status_ != "";
                last_search_status_ = status;
                // after search complete, return ' ', so that we clear the search status visually
                return was_seaching && status == "" ? " " : status;
            }
        }
        private filter.match create_match_object(BitArray matches, font_info font, line line, int line_idx) {
            return is_full_log ? new full_log_match_item(matches, font, line, line_idx, this) : new match_item(matches, font, line, line_idx, this);
        }

        public override string ToString() {
            return name;
        }

        public void force_refresh_visible_columns(List<log_view> all_views) {
            available_columns_refreshed_ = -1;
            use_previous_available_columns_ = false;
            log_.write_settings.apply_column_positions_to_me.reset();
            log_.write_settings.column_positions.reset();
            log_.write_settings.available_columns.reset();
            log_view_show_columns.refresh_visible_columns(all_views, this);
        }

        private void load_font() {
            list.Font = app.inst.font;
        }


        private void filterName_TextChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;
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

        // 1.6.31+ - made thread-safe
        // 1.0.77+ - care about multiple selection - returns the first sel
        public int sel_row_idx {
            get {
                int sel = -1;
                this.async_call_and_wait(() => sel = list.SelectedIndex);
                if (sel >= 0)
                    return sel;
                
                ListView.SelectedIndexCollection multi = null;
                this.async_call_and_wait(() => multi = list.SelectedIndices);
                if ( multi != null)
                    if (multi.Count > 0)
                        return multi[0];

                return -1;
            }
        }

        internal int cur_col_idx {
            get { return cur_col_; }
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
        internal int search_found_col_idx {
            get { return search_found_col_; }
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
                    if (lv_parent.selected_filter_row_index >= 0) {
                        // search by selected filter (we're focused on teh filters pane)
                        List<int> filters = new List<int>();
                        filters.Add(lv_parent.selected_filter_row_index);
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
            return string_search.matches(item.match, cur_search_);
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
        public new void Dispose() {
            if (log_ != null) {
                log_.Dispose();
                log_ = null;
            }
            model_.Dispose();
            filter_.Dispose();
        }

        internal bool has_bookmark(int line_idx) {
            return bookmarks_.Contains(line_idx);
        }

        public void set_log(log_reader log) {
            Debug.Assert(log != null);
            if (log_ != log) {
                if (log_ != null)
                    log_.Dispose();

                bool was_null = log_ == null;
                log_ = log;
                log_.tab_name = name;
                log_.on_new_lines += filter_.on_new_reader_lines;

                last_item_count_while_current_view_ = 0;
                available_columns_refreshed_ = -1;
                use_previous_available_columns_ = false;
                if ( !was_null)
                    clear();
                logger.Debug("[view] new log for " + name + " - " + log.log_name);
                update_x_of_y();
            }
        }

        public void update_edit() {
            edit.force_refresh();            
        }

        public string name {
            get { return viewName.Text; }
            set {
                viewName.Text = value;
                model_.name = value;
                filter_.name = value;
                if (log_ != null)
                    log_.tab_name = value;
                update_x_of_y();
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

        internal match_item item_at(int idx) {
            return model_.item_at(idx);
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

            var visible_rows = visible_row_indexes();
            int rows_per_page = visible_rows.Item2 - visible_rows.Item1;
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
                    int new_sel = sel - rows_per_page;
                    sel = new_sel >= 0 ? new_sel : 0;
                }
                break;
            case action_type.pagedown:
                if (sel < count - 1) {
                    int new_sel = sel + rows_per_page;
                    sel = new_sel <= count - 1 ? new_sel : count - 1;
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
            bool reversed_order = log_ != null && log_.reverse_order;
            if (old_item_count_ > 0 && sel_row_idx == old_item_count_ - 1 && !reversed_order)
                return true;
            if (reversed_order && sel_row_idx <= 0)
                return true;
            return false;
        }

        string cell_value(match_item i, int column_idx) {
            return log_view_cell.cell_value(i, column_idx);
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
            if (app.inst.show_view_line_count || app.inst.show_view_selected_line || app.inst.show_view_selected_index) {
                bool show_full_count = !is_full_log && item_count < filter_.full_count;
                if (show_full_count)
                    header += " (" + filter_.full_count +  ")";
            }
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

            // 1.5.8+ the main reason for not updating the title header is when monitoring live and new records arrive:
            //        this would end up causing quite a bit of flicker, due to the fact that I'd always be updating the title
            //
            //        also, another good reason not to show it is that when i navigate, this would again cause some flickering
            bool title_needs_x_of_y = !show_header || !is_current_view;
            string tab_text = title_needs_x_of_y ? name + x_of_y_title : name;
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
            logger.Debug("[view] change: " + change + " on " + name);
            switch (change) {
            case filter.change_type.new_lines:
            case filter.change_type.changed_filter:
                this.async_call(refresh);
                break;

            case filter.change_type.file_rewritten:
                this.async_call(clear);
                break;

            default:
                Debug.Assert(false);
                break;
            }
        }

        public void clear() {
            filter_.clear();
            refresh();
        }

        public void refresh() {
            if (log_ == null)
                return; // not set yet

            bool needs_ui_update = model_.needs_ui_update;
            needs_scroll_ = needs_scroll_to_last() && !needs_ui_update;

            model_.refresh();
            int new_item_count = item_count;
            filter_.compute_matches(log_);

            if (is_current_view)
                last_item_count_while_current_view_ = new_item_count;

            if (old_item_count_ == new_item_count && !needs_ui_update) {
                // nothing changed
                if ( model_.item_count > 0 && sel_row_idx == -1 && !is_full_log && is_current_view)
                    // just switched to another view where nothing was selected - go to end by default
                    go_last();
                return; 
            }

            if (needs_ui_update) {
                Enabled = true;
                Cursor = Cursors.IBeam;
                Focus();
            }

            model_.needs_ui_update = false;
            bool more_items = old_item_count_ < new_item_count;
            old_item_count_ = new_item_count;

            update_x_of_y();

            if (needs_ui_update) {
                go_to_closest_line(sel_line_idx_before_set_filter_, select_type.notify_parent);
                try_set_visible_row_y_offset(sel_row_idx, sel_y_offset_before_set_filter_);
                edit.update_ui();
                lv_parent.after_set_filter_update();
            }

            list.Refresh();
            if (needs_scroll_ && more_items && !needs_ui_update)
                go_last();
        }


        // if first item is true, we found colors, and the colors are item2 & 3
        private Tuple<bool,Color,Color> has_found_colors(int row_idx, log_view other_log, bool is_sel) {
            var i = item_at(row_idx) as full_log_match_item;

            int line_idx = i.match.line_idx;
            match_item found_line = null;
            switch (app.inst.syncronize_colors) {
            case app.synchronize_colors_type.none: // nothing to do
                return new Tuple<bool, Color, Color>(true, font_info.default_font.fg, font_info.default_font.bg);
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

            return new Tuple<Color, Color>( font_info.full_log_gray.fg, font_info.full_log_gray.bg );
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
            Debug.Assert(is_full_log);
            if (selected_view_ == name)
                // we already have this set correctly
                return;
            selected_view_ = name;
            list.Refresh();
        }

        private void go_last() {
            var count = item_count;
            if (count > 0) {
                if (!log_.reverse_order) {
                    if (ensure_row_visible(count - 1))
                        select_row_idx(count - 1, select_type.do_not_notify_parent);
                    else
                        // in this case, we failed to go to the last item - try again ASAP
                        util.postpone(go_last, 10);
                } else {
                    // reversed
                    if (ensure_row_visible(0))
                        select_row_idx(0, select_type.do_not_notify_parent);
                    else
                        // in this case, we failed to go to the last item - try again ASAP
                        util.postpone(go_last, 10);                    
                }
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
            if (!is_current_view && !is_full_log && list.SelectedIndex == -1)
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
            lv_parent.sel_changed(log_view_sel_change_type.backspace);
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
            if (row_idx < 0)
                return false;
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
            if (row_idx < 0)
                return;
            if (row_idx >= item_count)
                return;

            select_row_idx(row_idx, notify);
            if (is_row_visible(row_idx))
                // already visible
                return;

            // 1.3.30+ use RowHeightEffective (don't manually set RowHeight)
            int rows = list.Height / list.RowHeightEffective;
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
            edit.Visible = false;
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

        public void offset_closest_time(long time_ms, bool forward) {
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

                    // the idea is to notify the parent of the change only after the change has been complete
                    if (!show_name_) {
                        update_x_of_y();
                        lv_parent.on_view_name_changed(this, viewName.Text);
                    }
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
            // 1.6.6+ - I think this is wrong!
            if (e.ColumnIndex == msgCol.fixed_index())
                ShowWindow(e.ToolTipControl.Handle, 0);
        }

        private void log_view_Load(object sender, EventArgs e) {
        }

        private void list_KeyPress(object sender, KeyPressEventArgs e) {
            // suppress sound
            e.Handled = true;
        }

        public void set_search_for_text(search_for search) {
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
                msg_details.force_temporary_hide(this);
            }
            else if (edit.sel_text != "") {
                edit.escape();
            }
            else if (cur_search_ != null) {
                cur_search_ = null;
                list.Refresh();
            }
            else if (cur_filter_row_idx_ >= 0) {
                unmark();
            }
            else if (app.inst.edit_mode != app.edit_mode_type.always && is_editing) {
                is_editing_ = false;
                edit.update_ui();
            }

            if ( edit.sel_text == "")
                edit.force_refresh();
        }


        public async void search_next() {
            if (is_searching_ > 0) {
                // we're already searching (asynchronously)
                util.beep(util.beep_type.err);
                return;
            }
            /* 1.2.7+   implemented f3/shift-f3 on smart-edit first
                        note: this will never replace search form -> since there you can have extra settings: regexes, case-sensitivity ,full word
            */

            // if user has selected something (smart edit), search for that
            // if user has made a find (ctrl-f), search for that
            // otherwise, search for selected filter (if any)

            string sel_text = edit.sel_text.ToLower();
            await Task.Run((() => {
                ++is_searching_ ;
                if (cur_search_ != null || sel_text != "")
                    search_for_text_next();
                else if (cur_filter_row_idx_ >= 0)
                    search_for_next_match(cur_filter_row_idx_);
                --is_searching_ ;
            }));
            lv_parent.sel_changed(log_view_sel_change_type.search);
        }

        public async void search_prev() {
            if (is_searching_ > 0) {
                // we're already searching (asynchronously)
                util.beep(util.beep_type.err);
                return;
            }
            /* 1.2.7+   implemented f3/shift-f3 on smart-edit first
                        note: this will never replace search form -> since there you can have extra settings: regexes, case-sensitivity ,full word
            */

            // if user has selected something (smart edit), search for that
            // if user has made a find (ctrl-f), search for that
            // otherwise, search for selected filter (if any)

            string sel_text = edit.sel_text.ToLower();
            await Task.Run((() => {
                ++is_searching_;
                if (cur_search_ != null || sel_text != "")
                    search_for_text_prev();
                else if (cur_filter_row_idx_ >= 0)
                    search_for_prev_match(cur_filter_row_idx_);
                --is_searching_;
            }));
            lv_parent.sel_changed(log_view_sel_change_type.search);
        }

        // note: starts from the next row, or, if on row zero -> starts from row zero
        public async void search_for_text_first() {
            if (is_searching_ > 0) {
                // we're already searching (asynchronously)
                util.beep(util.beep_type.err);
                return;
            }

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
            if (include_row_zero && string_search.matches(i.match, cur_search_)) {
                // line zero contains the text already
                ensure_row_visible(0);
                lv_parent.sel_changed(log_view_sel_change_type.search);
            } else
                await Task.Run(() => {
                    ++is_searching_;
                    search_for_text_next();
                    --is_searching_;
                });
        }

        private bool row_contains_search_text(int row_idx, List<int> visible_indexes) {
            string sel_text = edit.sel_text.ToLower();
            Debug.Assert(sel_text != "");

            if (app.inst.edit_search_all_columns) {
                foreach (var col in visible_indexes)
                    if (can_find_text_at_row(sel_text, row_idx, col))
                        return true;
            } else if (can_find_text_at_row(sel_text, row_idx, cur_col_))
                return true;

            return false;
        }

        internal bool is_searching() {
            return (cur_search_ != null || edit.sel_text != "");
        }

        private void search_for_text_next() {
            int count = item_count;
            if (count < 1)
                return;

            string sel_text = edit.sel_text.ToLower();
            Debug.Assert(cur_search_ != null || sel_text != "");
            if (cur_search_ == null && sel_text == "")
                return;

            var all_visible_column_indexes = all_visible_column_types().Select(log_view_cell.info_type_to_cell_idx).ToList();
            int found = -1;
            int next_row = sel_row_idx >= 0 ? sel_row_idx + 1 : 0;
            for (int idx = next_row; idx < count && found < 0; ++idx) {
                // 1.2.7+ - if user has selected something, search for that
                if (sel_text != "") {
                    if ( row_contains_search_text(idx, all_visible_column_indexes))
                        found = idx;
                    continue;
                }

                match_item i = item_at(idx);
                if (string_search.matches(i.match, cur_search_))
                    found = idx;
            }

            if (found >= 0) {
                this.async_call(() => {
                    edit.clear_sel();
                    go_to_row(found, select_type.notify_parent);
                });
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

            var all_visible_column_indexes = all_visible_column_types().Select(log_view_cell.info_type_to_cell_idx).ToList();
            int found = -1;
            int prev_row = sel_row_idx >= 0 ? sel_row_idx - 1 : count - 1;
            for (int idx = prev_row; idx >= 0 && found < 0; --idx) {
                // 1.2.7+ - if user has selected something, search for that
                if (sel_text != "") {
                    if ( row_contains_search_text(idx, all_visible_column_indexes))
                        found = idx;
                    continue;
                }

                match_item i = item_at(idx);
                if (string_search.matches(i.match, cur_search_))
                    found = idx;
            }
            if (found >= 0) {
                this.async_call(() => {
                    edit.clear_sel();
                    go_to_row(found, select_type.notify_parent);
                });
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
                this.async_call(() => {
                    edit.clear_sel();
                    go_to_row(found, select_type.notify_parent);
                });
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
                this.async_call(() => {
                    edit.clear_sel();
                    go_to_row(found, select_type.notify_parent);
                });
            } else
                util.beep(util.beep_type.err);
        }

        private export_text export_current_sel() {
            export_text export = new export_text();
            string sel_text = edit.currently_selected_text;
            int row = sel_row_idx;
            Debug.Assert(sel_text != "" && row >= 0);
            match_item i = item_at(row);

            string font = list.Font.Name;
            export_text.cell c = new export_text.cell(0, 0, sel_text) {fg = i.fg(this), bg = i.bg(this), font = font, font_size = 7};
            export.add_cell(c);
            return export;
        }

        private export_text export(List<int> indices, bool msg_only) {
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

            string sel_text = edit.currently_selected_text;
            var export = sel_text == "" ? this.export(sel, true) : export_current_sel();
            string html = export.to_html(), text = export.to_text();
            clipboard_util.copy(html, text);
        }

        public void copy_full_line_to_clipboard() {
            var sel = selected_indices_array();
            if (sel.Count < 1)
                return;

            string sel_text = edit.currently_selected_text;
            var export = sel_text == "" ? this.export(sel, false) : export_current_sel();

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
            edit.force_refresh();
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
                edit.clear_sel();
                go_to_row(idx, select_type.notify_parent);
                lv_parent.sel_changed(log_view_sel_change_type.bookmark);
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
                edit.clear_sel();
                go_to_row(idx, select_type.notify_parent);
                lv_parent.sel_changed(log_view_sel_change_type.bookmark);
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
            // note: we might actually lose focus to the edit box - in this case, we don't really need to set_aliases anything
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

        public enum export_type {
            export_line_column, do_not_export_line_column, 
            // in this case, it will export it if there's at least one gap between two rows (like, Line 325 and 328 - two lines missing)
            export_line_column_if_needed
        }

        public export_text export_all_columns(export_type type = export_type.export_line_column_if_needed) {
            export_text export = new export_text();
            
            int count = filter_.match_count;
            if (count < 1)
                // nothing to export
                return export;

            bool export_line = true;
            switch (type) {
            case export_type.export_line_column:                export_line = true; break;
            case export_type.do_not_export_line_column:         export_line = false; break;
            case export_type.export_line_column_if_needed:      
                // ... note: we could have it in reverse order
                export_line = Math.Abs( item_at(count - 1).line_idx - item_at(0).line_idx ) != count - 1; break;
            default:                                            Debug.Assert(false); break;
            }

            int visible_idx = 0;
            string font = list.Font.Name;
            var export_columns = available_columns.Where(x => {
                if (x != info_type.line && x != info_type.view)
                    return true;
                if (x == info_type.line)
                    return export_line;
                if (x == info_type.view)
                    return is_full_log;
                Debug.Assert(false);
                return false;
            }).ToList();

            foreach (var col in export_columns) {
                match_item i = item_at(0);
                var txt = filter_.log.aliases.friendly_name(col) ;

                export_text.cell c = new export_text.cell(0, visible_idx, txt) { fg = i.fg(this), bg = i.bg(this), font = font, font_size = 7 };
                export.add_cell(c);
                ++visible_idx;
            }

            for (int idx = 0; idx < count; ++idx) {
                match_item i = item_at(idx);

                visible_idx = 0;
                foreach (var col in export_columns) {
                    string txt = log_view_cell.cell_value_by_type(i, col);
                    export_text.cell c = new export_text.cell(idx + 1, visible_idx, txt) { fg = i.fg(this), bg = i.bg(this), font = font, font_size = 7 };
                    export.add_cell(c);
                    ++visible_idx;
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

        internal search_for cur_search {
            get { return cur_search_; }
        }

        internal log_view_render render {
            get { return render_; }
        }

        internal bool needs_scroll {
            get { return needs_scroll_; }
        }

        public column_formatter_array formatter {
            get { return formatter_; }
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            bool is_renaming = win32.focused_ctrl() == viewName;

            if (any_moving_key_down() && is_editing)
                // while scrolling, don't show edit
                hide_edit_while_scrolling();

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
                bool is_ctrl_shift_right = keyData == (Keys.Right | Keys.Control | Keys.Shift);
                bool is_ctrl_shift_left = keyData == (Keys.Left | Keys.Control | Keys.Shift);
                if (edit.Visible) {
                    if (is_shift_left) {
                        edit.sel_to_left();
                        return true;
                    } else if (is_shift_right) {
                        edit.sel_to_right();
                        return true;
                    } else if (is_ctrl_shift_left) {
                        edit.sel_to_word_left();
                        return true;
                    } else if (is_ctrl_shift_right) {
                        edit.sel_to_word_right();
                        return true;
                    }
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


        private void on_edit_sel_changed() {
            // logger.Debug("[lv] sel= [" + edit.sel_text + "]");
            list.Refresh();
        }

        // returns the row, or -1 on failure
        private bool can_find_text_at_row(string txt, int row, int col) {
            string col_text = log_view_cell.cell_value(list.GetItem(row).RowObject as match_item, col).ToLower();
            int pos = col_text.IndexOf(txt);
            return pos >= 0;
        }

        // returns all visible column types - including those from the description control
        internal List<info_type> all_visible_column_types() {
            List<info_type> visible = lv_parent.description_columns();

            for (int col_idx = 0; col_idx < list.AllColumns.Count; ++col_idx)
                if (list.AllColumns[col_idx].IsVisible) {
                    info_type type = log_view_cell.cell_idx_to_type(col_idx);
                    if (!visible.Contains(type))
                        visible.Add(type);
                }

            return visible;
        }

        internal List<info_type> view_visible_column_types() {
            List<info_type> visible = new List<info_type>();

            for (int col_idx = 0; col_idx < list.AllColumns.Count; ++col_idx)
                if (list.AllColumns[col_idx].IsVisible) {
                    info_type type = log_view_cell.cell_idx_to_type(col_idx);
                    if (!visible.Contains(type))
                        visible.Add(type);
                }

            return visible;
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

            var all_visible_column_indexes = all_visible_column_types().Select(log_view_cell.info_type_to_cell_idx).ToList();
            // this contains the visible indexes within our view
            var view_visible_column_indexes = view_visible_column_types().Select(log_view_cell.info_type_to_cell_idx).ToList();

            search_found_col_ = -1;
            if (app.inst.edit_search_all_columns) {
                if (app.inst.edit_search_after)
                    for (int cur_row = sel_row_idx + 1; cur_row < max && found_row < 0; ++cur_row)
                        for (int col_idx = 0; col_idx < list.AllColumns.Count && found_row < 0; ++col_idx)
                            if (col_idx != cur_col_ && all_visible_column_indexes.Contains(col_idx))
                                if (can_find_text_at_row(txt, cur_row, col_idx)) {
                                    found_row = cur_row;
                                    // note: where we find the result - might only be visible in the description pane
                                    if (view_visible_column_indexes.Contains(col_idx))
                                        cur_col_ = col_idx;
                                    else
                                        search_found_col_ = col_idx;
                                }

                if (app.inst.edit_search_before)
                    for (int cur_row = sel_row_idx - 1; cur_row >= min && found_row < 0; --cur_row)
                        for (int col_idx = 0; col_idx < list.AllColumns.Count && found_row < 0; ++col_idx)
                            if (col_idx != cur_col_ && all_visible_column_indexes.Contains(col_idx))
                                if (can_find_text_at_row(txt, cur_row, col_idx)) {
                                    found_row = cur_row;
                                    // note: where we find the result - might only be visible in the description pane
                                    if (view_visible_column_indexes.Contains(col_idx))
                                        cur_col_ = col_idx;
                                    else
                                        search_found_col_ = col_idx;
                                }
            }

            if (found_row >= 0) {
                go_to_row(found_row, select_type.notify_parent);
                if (search_found_col_ < 0) {
                    edit.update_ui();
                    edit.go_to_text(txt);
                } else
                // the search result is visible only on the description pane
                    edit.force_sel_text(txt);
                lv_parent.sel_changed(log_view_sel_change_type.search);
            }
        }

        private bool any_moving_key_down() {
            return win32.IsKeyPushedDown(Keys.Up) || win32.IsKeyPushedDown(Keys.Down) || win32.IsKeyPushedDown(Keys.PageUp) || win32.IsKeyPushedDown(Keys.PageDown) || win32.IsKeyPushedDown(Keys.Home) || win32.IsKeyPushedDown(Keys.End);
        }

        private void list_Scroll(object sender, ScrollEventArgs e) {
            scrolling_time_ = DateTime.Now;
            hide_edit_while_scrolling();
        }

        public void hide_edit_while_scrolling() {
            edit.force_hide = true;
            util.add_timer(() => {
                if (any_moving_key_down())
                    return false;
                if (scrolling_time_.AddMilliseconds(250) >= DateTime.Now)
                    return false;

                edit.force_hide = false;
                return true;
            });
        }

        private void list_CellClick(object sender, CellClickEventArgs e) {
            //on_mouse_click();
        }

        private void list_MouseClick(object sender, MouseEventArgs e) {
        }

        private void list_MouseUp(object sender, MouseEventArgs e) {
            edit.on_mouse_up();
        }

        private void list_MouseDown(object sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
                right_click_.right_click();
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                on_mouse_click(e.Location);
        }

        private void on_mouse_click(Point mouse) {
            if (!is_editing)
                return;

            var i = list.OlvHitTest(mouse.X, mouse.Y);
            if (i.Item == null || i.SubItem == null)
                return;

            int row_idx = i.RowIndex;
            int col_idx = i.ColumnIndex;
            if (col_idx >= 0 && row_idx >= 0) {
                // find the index within all columns (not just the visible ones)
                col_idx = list.AllColumns.FindIndex(x => x == i.Column);
                Debug.Assert(col_idx >= 0);
                cur_col_ = col_idx;

                edit.on_mouse_click(list.PointToScreen(mouse));
            }
        }

        public void do_right_click_via_key() {
            right_click_.right_click_at_caret();
        }


        private void viewName_Leave(object sender, EventArgs e) {
            show_name = false;
        }

        private void log_view_SizeChanged(object sender, EventArgs e) {
            edit.update_ui();
        }

        private void log_view_LocationChanged(object sender, EventArgs e) {
            edit.update_ui();
        }

        private void updateCursor_Tick(object sender, EventArgs e) {
            var focus = win32.focused_ctrl();
            bool here = focus == list || focus == edit;
            if (!here)
                return;
            // see if hovering header
            var mouse = list.PointToClient( Cursor.Position);
            bool hovers_header = show_header && list.HeaderControl.ClientRectangle.Contains(mouse);
            if (hovers_header)
                return;
            
            bool busy = is_searching_ > 0 || model_.is_running_filter;
            list.Cursor = busy ? Cursors.WaitCursor : Cursors.IBeam;
        }
    }
}
