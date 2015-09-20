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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using LogWizard.ui;

namespace LogWizard
{
    partial class log_view : UserControl
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private log_wizard parent;
        private filter filter_ ;
        private log_line_reader log_ = null;

        // by default - it's a new filter (thus, needing refresh)
        private bool filter_changed_ = true;

        private string selected_view_ = null;

        private int last_item_count_while_current_view_ = 0;

        private Font non_bold_ = null, bold_ = null;

        // the reason we derive from filter.match is memory efficiency; otherwise, ew would have two lists - one in filter,
        // and one here, full of pointers
        //
        // for large files, this can add up to a LOT of memory
        private class item : filter.match {

            public Color override_bg = util.transparent, override_fg = util.transparent;

            public item(log_view parent) {
            }

            public int line {
                get { return base.line_idx + 1; }
            }

            public virtual string view {
                get { return ""; }
            }

            public string date {
                get { return base.line.part(info_type.date); }
            }
            public string time {
                get { return base.line.part(info_type.time); }
            }
            public string level {
                get { return base.line.part(info_type.level); }
            }
            public string msg {
                get { return base.line.part(info_type.msg); }
            }

            public string file {
                get { return base.line.part(info_type.file); }
            }
            public string func {
                get { return base.line.part(info_type.func); }
            }
            public string class_ {
                get { return base.line.part(info_type.class_); }
            }
            public string ctx1 {
                get { return base.line.part(info_type.ctx1); }
            }
            public string ctx2 {
                get { return base.line.part(info_type.ctx2); }
            }
            public string ctx3 {
                get { return base.line.part(info_type.ctx3); }
            }
            public string thread {
                get { return base.line.part(info_type.thread); }
            }

            public Color bg(log_view parent) {
                if (parent.bookmarks_.Contains(base.line_idx))
                    return parent.bookmark_bg_;
                if (override_bg != util.transparent)
                    return override_bg;
                return base.font.bg;
            }

            public Color fg(log_view parent) {
                if (parent.bookmarks_.Contains(base.line_idx))
                    return parent.bookmark_fg_;
                if (override_fg != util.transparent)
                    return override_fg;
                return base.font.fg;
            }

            public filter.match match {
                get { return this;  }
            }
        };

        // the reason we have this class - is for memory eficiency - since all views (except full log) don't need the info here
        private class full_log_item : item {
            private readonly log_view parent_ = null;

            public full_log_item(log_view parent) : base(parent) {
                Debug.Assert(parent != null);
                parent_ = parent;
            }

            public override string view {
                get {
                    return parent_.parent.matched_logs(line_idx);
                }
            }
        }

        private class list_data_source : AbstractVirtualListDataSource {
            private VirtualObjectListView lv_ = null;

            private filter.match_list items_;

            public list_data_source(VirtualObjectListView lv, filter.match_list items ) : base(lv) {
                lv_ = lv;
                items_ = items;
            }

            public string name {
                set { items_.name = "list_data " + value; }
            }

            public override int GetObjectIndex(object model) {
                return items_.index_of(model as item);
            }

            public override object GetNthObject(int n) {
                return n < items_.count ? items_.match_at(n) : null;
            }

            public override int GetObjectCount() {
                return items_.count;
            }

            public void refresh() {
                if ( items_.count == 0)
                    lv_.ClearObjects();

                lv_.UpdateVirtualListSize();                
            }
        }

        private list_data_source model_ = null;
        private bool visible_columns_refreshed_ = false;

        // lines that are bookmarks (sorted by index)
        private List<int> bookmarks_ = new List<int>();

        private Color bookmark_bg_ = Color.Blue, bookmark_fg_ = Color.White;

        private int font_size_ = 9; // default font size

        private List<int> full_widths_ = new List<int>();

        private bool pad_name_on_left_ = false;

        private bool show_name_in_header_ = false;
        private bool show_header_ = true;
        private bool show_name_ = true;

        private int old_item_count_ = 0;

        public log_view(log_wizard parent, string name)
        {
            filter_ = new filter(this.create_match_object);
            InitializeComponent();
            this.parent = parent;
            viewName.Text = name;
            model_ = new list_data_source(this.list, filter_.matches ) { name = name };
            list.VirtualListDataSource = model_;
            list.RowHeight = 18;

            load_font();
            parent.handle_subcontrol_keys(this);
        }

        private filter.match create_match_object() {
            return is_full_log ? new full_log_item(this) : new item(this);
        }

        public override string ToString() {
            return name;
        }

        private void load_font() {
            string[] font_names = Program.sett.get("font_names").Split(',');
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
            parent.on_view_name_changed(this, viewName.Text);
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

        // returns the line index of the current selection
        public int sel_line_idx {
            get {
                int sel = sel_row_idx;
                if (sel >= 0)
                    return (list.GetItem(sel).RowObject as item).match.line_idx;
                return -1;
            }
        }



        public string sel_line_text {
            get {
                int sel = sel_row_idx;
                if (sel >= 0)
                    return (list.GetItem(sel).RowObject as item).match.line.part(info_type.msg);
                return "";
            }
        }
        public Rectangle sel_rect {
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
        // fg and bg
        public Tuple<Color,Color> sel_line_colors {
            get {
                int sel = sel_row_idx;
                if (sel >= 0) {
                    var i = list.GetItem(sel).RowObject as item;
                    return new Tuple<Color, Color>(i.fg(this), i.bg(this));
                }
                return new Tuple<Color, Color>(Color.Black,Color.White);
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

        // while this is true, has anything_changed always returns false (basically, we want this during loading,
        // since it happens on a different thread - it's pretty complicated to find out when the complete loading has occured)
        private bool turn_off_has_anying_changed_ = true;

        public bool turn_off_has_anying_changed {
            get { return turn_off_has_anying_changed_; }
            set {
                turn_off_has_anying_changed_ = value;
                if (!turn_off_has_anying_changed_) {
                    last_item_count_while_current_view_ = list.GetItemCount();
                    update_x_of_y();
                }
            }
        }

        public bool has_anything_changed {
            get {
                if (turn_off_has_anying_changed)
                    return false;
                return !is_current_view && (last_item_count_while_current_view_ != list.GetItemCount());
            }
        }

        private bool is_full_log {
            get {
                return filter_ != null ? filter_.row_count < 1 : true;
            }
        }


        public void set_filter(List<filter_row> filter) {
            filter_.name = name;
            filter_.update_rows(filter);
            if (filter_.rows_changed)
                filter_changed_ = true;
        }

        internal void set_log(log_line_reader log) {
            if (log_ != log) {
                log_ = log;
                filter_changed_ = true;
                last_item_count_while_current_view_ = 0;
                visible_columns_refreshed_ = false;
                logger.Debug("[view] new log for " + name + " - " + log.name);
                update_x_of_y();
            }
        }

        public string name {
            get { return viewName.Text; }
            set {
                viewName.Text = value;
                model_.name = value;
                filter_.name = value;
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
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                item i = list.GetItem(idx).RowObject as item;
                if (i.match.line_idx == line_idx)
                    return list.GetItem(idx);
            }
            return null;
        }


        public void on_action(log_wizard.action_type action) {
            switch (action) {
            case log_wizard.action_type.home:
            case log_wizard.action_type.end:
            case log_wizard.action_type.pageup:
            case log_wizard.action_type.pagedown:
            case log_wizard.action_type.arrow_up:
            case log_wizard.action_type.arrow_down:
            case log_wizard.action_type.shift_arrow_down:
            case log_wizard.action_type.shift_arrow_up:
                break;
            default:
                Debug.Assert(false);
                return;
            }

            int sel = sel_row_idx;
            if (sel < 0 && list.GetItemCount() > 0)
                sel = 0; // assume we start from the top

            // int rows_per_page = list.RowsPerPage;
            int height = list.Height - list.HeaderControl.ClientRectangle.Height;
            switch (action) {
            case log_wizard.action_type.home:
                sel = 0;
                break;
            case log_wizard.action_type.end:
                sel = list.GetItemCount() - 1;
                break;
            case log_wizard.action_type.pageup:
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
            case log_wizard.action_type.pagedown:
                if (sel < list.GetItemCount() - 1) {
                    var r = list.GetItem(sel).Bounds;
                    var middle = new Point( r.Left + r.Width / 2, r.Top + r.Height / 2 );
                    list.LowLevelScroll(0, height);
                    int new_sel = list.HitTest(middle).Item.Index;
                    if (new_sel != sel)
                        sel = new_sel;
                    else
                        sel = list.GetItemCount() - 1; // reached bottom
                }
                break;
            case log_wizard.action_type.arrow_up:
                if ( sel > 0)
                    --sel;
                break;
            case log_wizard.action_type.arrow_down:
                if ( sel < list.GetItemCount() - 1)
                    ++sel;
                break;

            // default list behavior is wrong, we need to override
            case log_wizard.action_type.shift_arrow_up:                
                if (sel > 0) {
                    int existing = selected_indices_array().Min();
                    if (existing > 0) {
                        list.SelectedIndices.Add(existing - 1);
                        list.EnsureVisible(existing);
                    }
                    return;
                }
                break;
            // default list behavior is wrong, we need to override
            case log_wizard.action_type.shift_arrow_down:
                if (sel < list.GetItemCount() - 1) {
                    int existing = selected_indices_array().Max();
                    if (existing < list.GetItemCount() - 1) {
                        list.SelectedIndices.Add(existing + 1);
                        list.EnsureVisible(existing);                        
                    }
                    return;
                }
                break;

            }
            if (sel >= 0 && sel_row_idx != sel) {
                select_idx( sel, select_type.notify_parent);
                list.EnsureVisible(sel);
            }
        }

        private List<int> selected_indices_array() {
            List<int> sel = new List<int>();
            if ( list.SelectedIndices != null)
                for ( int i = 0; i < list.SelectedIndices.Count; ++i)
                    sel.Add(list.SelectedIndices[i]);
            return sel;
        } 

        private bool needs_scroll_to_last() {
            if (filter_.match_count < 1)
                return true;
            if (sel_row_idx < 0)
                return true;
            if (old_item_count_ > 0 && sel_row_idx == old_item_count_ - 1)
                return true;
            return false;
        }


        private string column_value(item col, info_type type) {
            switch (type) {
            case info_type.msg:
                return col.msg;
            case info_type.time:
                return col.time;
            case info_type.date:
                return col.date;
            case info_type.level:
                return col.level;
            case info_type.class_:
                return col.class_;
            case info_type.file:
                return col.file;
            case info_type.func:
                return col.func;
            case info_type.ctx1:
                return col.ctx1;
            case info_type.ctx2:
                return col.ctx2;
            case info_type.ctx3:
                return col.ctx3;
            case info_type.thread:
                return col.thread;
            }
            Debug.Assert(false);
            return col.msg;
        }

        OLVColumn column(info_type type) {
            switch (type) {
            case info_type.msg:
                return msgCol;
            case info_type.time:
                return timeCol;
            case info_type.date:
                return dateCol;
            case info_type.level:
                return levelCol;
            case info_type.class_:
                return classCol;
            case info_type.file:
                return fileCol;
            case info_type.func:
                return funcCol;
            case info_type.ctx1:
                return ctx1Col;
            case info_type.ctx2:
                return ctx2Col;
            case info_type.ctx3:
                return ctx3Col;
            case info_type.thread:
                // FIXME have its own column!
                return ctx1Col;
            }
            Debug.Assert(false);
            return msgCol;
        }


        // when we have a number of columns - based on the info on each column, we hide or show them
        private void refresh_visible_columns() {
            if (visible_columns_refreshed_)
                return;
            int MIN_ROWS = 10;
            if (list.GetItemCount() >= MIN_ROWS) {

                for ( int type_as_int = 0; type_as_int < (int)info_type.max; ++type_as_int)
                    if (type_as_int != (int) info_type.msg) {
                        info_type type = (info_type) type_as_int;
                        int value_count = 0;
                        for (int idx = 0; idx < MIN_ROWS; ++idx) {
                            var i = list.GetItem(idx).RowObject as item;
                            if (i == null)
                                return;
                            if (column_value(i, type) != "")
                                ++value_count;
                        }
                        bool has_values = (value_count > 0);
                        bool is_visible = column(type).Width > 0;
                        if (has_values != is_visible) 
                            column(type).Width = has_values ? 80 : 0;                        
                    }
                visible_columns_refreshed_ = true;
            }
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
            string y = "" + list.GetItemCount();
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

            string tab_text = filter_changed_ ? name + (x_of_y_title != "" ? " (?)" : "") : name + x_of_y_title;
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

        private void force_select_first_item() {
            if ( list.GetItemCount() > 0)
                if (sel_row_idx < 0) {
                    list.SelectedIndex = 0;
                    logger.Debug("[view] forcing sel zero for " + name);
                }
        }

        // called when we've been selected as current view
        public void on_selected() {
            if (tab_parent == null)
                return;

            // we want the tab to be repainted
            last_item_count_while_current_view_ = list.GetItemCount();
            tab_parent.Text += " ";

            if (list.GetItemCount() > 0)
                return;
            // if we don't have any items, it's likely that we need to refresh
            if ( filter_changed_)
                util.add_timer(
                    (has_ended) => {
                        if ( !has_ended && filter_changed_)
                            tab_parent.Text = util.add_dots(tab_parent.Text, 3);
                        else 
                            update_x_of_y();
                    }, 2500);
        }

        public void refresh() {
            // comment the following line to easily test the UI
            //return;

            if (log_ == null)
                return; // not set yet

            bool needs_scroll = needs_scroll_to_last();
            old_item_count_ = filter_.match_count;
            filter_.compute_matches(log_);

            if (is_current_view)
                last_item_count_while_current_view_ = filter_.match_count;

            // FIXME call update_x_of_y(); and see if we need to scroll last correctly
            refresh_visible_columns();
            
            list.Refresh();
            model_.refresh();


            if( needs_scroll)
                go_last();

            if (is_current_view)
                last_item_count_while_current_view_ = list.GetItemCount();
        }

        // returns all the lines that match this filter
        public List<int> matched_lines(int start_line_idx, int end_line_idx) {
            memory_optimized_list<int> lines = new memory_optimized_list<int>() { min_capacity = app.inst.no_ui.min_matched_lines_capacity, name = "matched_lines " + name, increase_percentage = .6 };
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                item i = list.GetItem(idx).RowObject as item;
                if ( i.match.line_idx >= start_line_idx && i.match.line_idx < end_line_idx)
                    lines.Add(i.match.line_idx);
            }
            return lines;
        }


        private bool has_found_colors(int row_idx, log_view other_log, bool is_sel) {
            var i = list.GetItem(row_idx).RowObject as full_log_item;

            int line_idx = i.match.line_idx;
            item found_line = null;
            switch (app.inst.syncronize_colors) {
            case app.synchronize_colors_type.none: // nothing to do
                i.override_fg = Color.Black;
                return true;
            case app.synchronize_colors_type.with_current_view:
                found_line = other_log.filter_.matches.binary_search(line_idx).Item1 as item;
                if (found_line != null) {
                    i.override_bg = found_line.bg(this);
                    i.override_fg = found_line.fg(this);
                    return true;
                }
                break;
            case app.synchronize_colors_type.with_all_views:
                found_line = other_log.filter_.matches.binary_search(line_idx).Item1 as item;
                if (found_line != null) {
                    Color bg = found_line.bg(this), fg = found_line.fg(this);
                    if (app.inst.sync_colors_all_views_gray_non_active && !is_sel) 
                        fg = util.grayer_color(fg);
                    i.override_bg = bg;
                    i.override_fg = fg;
                    return true;
                }
                break;
            default: Debug.Assert(false);
                break;
            }
            return false;
        }

        private void update_colors_for_line(int row_idx, List<log_view> other_logs, int sel_idx, ref bool needed_refresh) {
            Debug.Assert(other_logs.Count > 0 && sel_idx < other_logs.Count);

            var i = list.GetItem(row_idx).RowObject as full_log_item;
            i.override_bg = filter_line.font_info.full_log_gray_.bg;
            i.override_fg = filter_line.font_info.full_log_gray_.fg;

            switch (app.inst.syncronize_colors) {
            case app.synchronize_colors_type.none:
                has_found_colors(row_idx, other_logs[0], false);
                break;
            case app.synchronize_colors_type.with_current_view:
                has_found_colors(row_idx, other_logs[sel_idx], true);
                break;
            case app.synchronize_colors_type.with_all_views:
                if (has_found_colors(row_idx, other_logs[sel_idx], true))
                    break;
                for (int idx = 0; idx < other_logs.Count; ++idx)
                    if ( idx != sel_idx)
                        if (has_found_colors(row_idx, other_logs[idx], false))
                            break;
                break;
            default:
                Debug.Assert(false);
                break;
            }
            if (update_line_color(row_idx))
                needed_refresh = true;
        }


        public void update_colors(List<log_view> other_logs, int sel_log_view_idx) {
            Debug.Assert(is_full_log);

            int PAD = 5;
            var top = list.GetItemAt(PAD, list.HeaderControl.ClientRectangle.Height + PAD);
            if (top == null)
                return;
            int top_idx = top.Index;
            int height = list.Height - list.HeaderControl.ClientRectangle.Height;
            int row_height = top.Bounds.Height;
            int rows_per_page = height / row_height;

            int JUST_IN_CASE = 3;
            int bottom_idx = top_idx + rows_per_page + JUST_IN_CASE;
            top_idx -= JUST_IN_CASE;
            if (top_idx < 0)
                top_idx = 0;

            bool needs_refresh = false;
            for ( int idx = top_idx; idx <= bottom_idx; ++idx)
                if ( idx < filter_.matches.count)
                    update_colors_for_line(idx, other_logs, sel_log_view_idx, ref needs_refresh);

            if ( needs_refresh)
                list.Refresh();
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
            var count = list.GetItemCount();
            if (count > 0) {
                list.EnsureVisible(count - 1);
                select_idx(count - 1, select_type.do_not_notify_parent);
            }
        }

        // returns true if it needed to refresh
        private bool update_line_color(int idx) {
            var row = list.GetItem(idx);
            item i = row.RowObject as item;

            bool needed = false;
            if (row.BackColor.ToArgb() != i.bg(this).ToArgb()) {
                row.BackColor = i.bg(this);
                needed = true;
            }

            if (row.ForeColor.ToArgb() != i.fg(this).ToArgb()) {
                row.ForeColor = i.fg(this);
                needed = true;
            }
            return needed;
        }

        private void update_line_highlight_color(int idx) {
            item i = list.GetItem(idx).RowObject as item;
            if (i == null)
                return;
            list.UnfocusedHighlightBackgroundColor = util.darker_color(i.bg(this));
            list.UnfocusedHighlightForegroundColor = i.fg(this);

            list.HighlightBackgroundColor = util.darker_color(util.darker_color(i.bg(this)));
            list.HighlightForegroundColor = i.fg(this);
        }
        /*
        private void on_filter_changed() {
            logger.Debug("[view] filter changed on " + name + " - " + list.GetItemCount() + " items so far");
            bool needs_scroll = needs_scroll_to_last();

            // from this point on, we only append to the existing list
            int match_count = filter_.match_count;
            memory_optimized_list<filter.match> new_ = new memory_optimized_list<filter.match>(match_count) {name = "view " + name};
            bool filter_reset = false;
            for (int i = 0; i < match_count && !filter_reset; ++i) {
                var new_match = filter_.match_at(i);
                if (new_match != null)
                    new_.Add(new_match);
                else
                    filter_reset = true; // filter got reset in the other thread
            }
            if (!filter_reset)
                model_.set_matches(new_, this);

            // update colors
            int count = filter_.matches.Count;
            for (int idx = 0; idx < count && !filter_reset; ++idx) {
                item i = list.GetItem(idx).RowObject as item;
                var match = filter_.match_at(idx);
                if (match != null)
                    i.match = match;
                else
                    filter_reset = true; // filter got reset in the other thread
            }

            if (!filter_reset) {
                for (int idx = 0; idx < count; ++idx)
                    update_line_color(idx);
                list.Refresh();
            } else {
                logger.Debug("[view] filter got reset on the other thread - " + name);
                model_.set_matches(new memory_optimized_list<filter.match>(), this);
                needs_scroll = false;
            }

            if (needs_scroll)
                go_last();
            else
                force_select_first_item();
            update_x_of_y();
        }*/

        private void list_FormatCell_1(object sender, FormatCellEventArgs e) {
        }

        private void list_FormatRow_1(object sender, FormatRowEventArgs e) {
            item i = e.Item.RowObject as item;
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

        private void select_idx(int idx, select_type notify) {
            if (sel_row_idx == idx)
                return; // already selected

            // 1.0.67+ - there's a bug in objectlistview - if we're not the current view, we can't really select a row
            if (!is_current_view && !is_full_log)
                return;

            select_nofify_ = notify;
            if (idx >= 0 && idx < list.GetItemCount()) {
                logger.Debug("[view] " + name + " sel=" + idx);
                list.SelectedIndex = idx;
                update_line_highlight_color(idx);
                update_x_of_y();
            }
            select_nofify_ = select_type.notify_parent;
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e) {
            int sel = sel_row_idx;
            if (sel < 0)
                return;

            if (select_nofify_ == select_type.notify_parent) {
                int line_idx = (list.GetItem(sel).RowObject as item).match.line_idx;
                //parent.go_to_line(line_idx, this);
            }
        }

        public void go_to_line(int line_idx, select_type notify) {
            if (line_idx >= list.GetItemCount())
                return;

            select_idx(line_idx, notify);
            //list.EnsureVisible(line_idx);

            int rows = list.Height / list.RowHeight;
            int bottom_idx = line_idx + rows / 2;
            if (bottom_idx >= list.GetItemCount())
                bottom_idx = list.GetItemCount() - 1;
            int top_idx = bottom_idx - rows;
            if (top_idx < 0)
                top_idx = 0;
            // we want to show the line in the *middle* of the control (height-wise)
            list.EnsureVisible(top_idx);
            list.EnsureVisible(bottom_idx);
        }


        public void go_to_closest_line(int line_idx, select_type notify) {
            if (list.GetItemCount() < 1)
                return;
            
            var closest = filter_.matches.binary_search_closest(line_idx);
            if ( closest.Item2 >= 0)
                go_to_line(closest.Item2, notify);
        }

        public bool matches_line(int line_idx) {
            var closest = filter_.matches.binary_search_closest(line_idx);
            if (closest.Item2 >= 0)
                return closest.Item1.line_idx == line_idx;
            else
                return false;
        }

        public void go_to_closest_time(DateTime time) {
            var closest = filter_.matches.binary_search_closest(time);
            if ( closest.Item2 >= 0)
                go_to_line(closest.Item2, select_type.notify_parent);
        }

        public void offset_closest_time(int time_ms, bool forward) {
            if (list.GetItemCount() < 1)
                return;
            int sel = sel_row_idx;
            if (sel < 0)
                sel = 0; // just in case we haven't selected anything - start from beginning
            var i = (list.GetItem(sel).RowObject as item);
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
            get { return list.GetItemCount(); }
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
                if (show_name_ != value) {
                    show_name_ = value;
                    update_show_name();

                    bool new_show_header = show_name_ || show_header_;
                    if( old_show_header != new_show_header)
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
            if (e.ColumnIndex == msgCol.Index)
                ShowWindow(e.ToolTipControl.Handle, 0);
        }

        private void log_view_Load(object sender, EventArgs e) {
        }

        private void list_KeyPress(object sender, KeyPressEventArgs e) {
            // suppress sound
            e.Handled = true;
        }

        public void mark_text(search_form.search_for search, Color fg, Color bg) {
            Debug.Assert(list.GetItemCount() == filter_.match_count);

            // just in case a filter was selected
            unmark();

            bool needs_refresh = false;
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                item i = list.GetItem(idx).RowObject as item;
                if (string_search.matches( i.match.line.part(info_type.msg), search)) {
                    i.override_fg = fg;
                    i.override_bg = bg;
                    needs_refresh = true;
                    update_line_color(idx);
                }
            }
            if (needs_refresh)
                list.Refresh();
        }

        // unmarks any previously marked rows
        public void unmark() {
            Debug.Assert(list.GetItemCount() == filter_.match_count);

            bool needs_refresh = false;
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                item i = list.GetItem(idx).RowObject as item;
                bool needs_unmark = i.override_bg != util.transparent || i.override_fg != util.transparent;
                if (needs_unmark) {
                    i.override_fg = util.transparent;
                    i.override_bg = util.transparent;
                    needs_refresh = true;
                    update_line_color(idx);
                }
            }
            if (needs_refresh)
                list.Refresh();
        }

        public void search_for_text_first(search_form.search_for search) {
            if (list.GetItemCount() < 1)
                return;

            select_idx(0, select_type.notify_parent);
            item i = list.GetItem(0).RowObject as item;
            if ( string_search.matches( i.match.line.part(info_type.msg), search)) {
                // line zero contains the text already
                list.EnsureVisible(0);
                return;
            } else
                search_for_text_next(search);
        }

        public void search_for_text_next(search_form.search_for search) {
            if (list.GetItemCount() < 1)
                return;

            int found = -1;
            if (sel_row_idx >= 0)
                for (int idx = sel_row_idx + 1; idx < list.GetItemCount() && found < 0; ++idx) {
                    item i = list.GetItem(idx).RowObject as item;
                    if (string_search.matches( i.match.line.part(info_type.msg), search))
                        found = idx;
                }
            if (found >= 0) {
                select_idx(found, select_type.notify_parent);
                list.EnsureVisible(found);
            } else
                util.beep(util.beep_type.err);
        }

        public void search_for_text_prev(search_form.search_for search) {
            if (list.GetItemCount() < 1)
                return;

            int found = -1;
            if (sel_row_idx > 0)
                for (int idx = sel_row_idx - 1; idx >= 0 && found < 0; --idx) {
                    item i = list.GetItem(idx).RowObject as item;
                    if (string_search.matches( i.match.line.part(info_type.msg), search))
                        found = idx;
                }
            if (found >= 0) {
                select_idx(found, select_type.notify_parent);
                list.EnsureVisible(found);
            } else
                util.beep(util.beep_type.err);
        }

        public void mark_match(int filter_row_idx, Color fg, Color bg) {
            Debug.Assert(list.GetItemCount() == filter_.match_count);

            bool needs_refresh = false;
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                item i = list.GetItem(idx).RowObject as item;
                bool is_match = i.match.matches.Length > filter_row_idx && i.match.matches[filter_row_idx];
                bool needs_change = (is_match && (i.override_fg.ToArgb() != fg.ToArgb() || i.override_bg.ToArgb() != bg.ToArgb())) || (!is_match && (i.override_fg != util.transparent || i.override_bg != util.transparent));

                if (needs_change) {
                    i.override_fg = is_match ? fg : util.transparent;
                    i.override_bg = is_match ? bg : util.transparent;
                    needs_refresh = true;
                    update_line_color(idx);
                }
            }
            if (needs_refresh)
                list.Refresh();
        }

        public void search_for_next_match(int filter_row_idx) {
            if (list.GetItemCount() < 1)
                return;
            int found = -1;
            if (sel_row_idx >= 0)
                for (int idx = sel_row_idx + 1; idx < list.GetItemCount() && found < 0; ++idx) {
                    item i = list.GetItem(idx).RowObject as item;
                    bool is_match = i.match.matches.Length > filter_row_idx && i.match.matches[filter_row_idx];
                    if (is_match)
                        found = idx;
                }
            if (found >= 0) {
                select_idx(found, select_type.notify_parent);
                list.EnsureVisible(found);
            } else
                util.beep(util.beep_type.err);
        }

        public void search_for_prev_match(int filter_row_idx) {
            if (list.GetItemCount() < 1)
                return;
            int found = -1;
            if (sel_row_idx > 0)
                for (int idx = sel_row_idx - 1; idx >= 0 && found < 0; --idx) {
                    item i = list.GetItem(idx).RowObject as item;
                    bool is_match = i.match.matches.Length > filter_row_idx && i.match.matches[filter_row_idx];
                    if (is_match)
                        found = idx;
                }
            if (found >= 0) {
                select_idx(found, select_type.notify_parent);
                list.EnsureVisible(found);
            } else
                util.beep(util.beep_type.err);
        }

        public void copy_to_clipboard() {
            var sel = selected_indices_array();
            if (sel.Count < 1)
                return;

            string full = "";
            foreach (int row in sel) {
                item i = list.GetItem(row).RowObject as item;
                if (full != "")
                    full += "\r\n";
                full += i.match.line.part(info_type.msg);
            }

            try {
                Clipboard.SetText(full);
            } catch {
            }
        }

        public void copy_full_line_to_clipboard() {
            var sel = selected_indices_array();
            if (sel.Count < 1)
                return;

            string full = "";
            foreach (int row in sel) {
                item i = list.GetItem(row).RowObject as item;
                if (full != "")
                    full += "\r\n";
                full += i.match.line.full_line;
            }

            try {
                Clipboard.SetText(full);
            } catch {
            }
        }

        public void set_bookmarks(List<int> line_idxs) {
            var old = bookmarks_.Except(line_idxs);
            var new_ = line_idxs.Except(bookmarks_);

            bookmarks_ = line_idxs;
            bookmarks_.Sort();

            foreach (int idx in old) {
                var row = row_by_line_idx(idx);
                if (row != null) {
                    update_line_color(row.Index);
                    list.RefreshItem(row);
                }
            }
            foreach (int idx in new_) {
                var row = row_by_line_idx(idx);
                if (row != null) {
                    update_line_color(row.Index);
                    list.RefreshItem(row);
                }
            }
            if (sel_row_idx >= 0)
                update_line_highlight_color(sel_row_idx);
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
                select_idx(idx, select_type.notify_parent);
                list.EnsureVisible(idx);
            } else
                util.beep(util.beep_type.err);
        }

        public void prev_bookmark() {
            if (list.GetItemCount() < 1)
                return;

            int start = sel_row_idx >= 0 ? sel_line_idx - 1 : (list.GetItem(sel_row_idx).RowObject as item).match.line_idx;
            int mark = bookmarks_.LastOrDefault(line => line <= start && row_by_line_idx(line) != null);
            if (mark == 0)
                if (!bookmarks_.Contains(mark))
                    // in this case, we did not find anything and got returned default (0)
                    mark = -1;

            if (mark >= 0) {
                int idx = row_by_line_idx(mark).Index;
                select_idx(idx, select_type.notify_parent);
                list.EnsureVisible(idx);
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
            if (full_widths_.Count < 1) 
                // save them now
                for (int idx = 0; idx < list.Columns.Count; ++idx)
                    full_widths_.Add(list.Columns[idx].Width);                
            
            logger.Info("[lv] showing rows - " + name + " = " + show );

            switch (show) {
            case ui_info.show_row_type.msg_only:
                for (int idx = 0; idx < list.Columns.Count; ++idx)
                    if (idx != msgCol.Index && idx != lineCol.Index)
                        list.Columns[idx].Width = 0;
                break;
            case ui_info.show_row_type.msg_and_view_only:
                for (int idx = 0; idx < list.Columns.Count; ++idx)
                    if (idx == viewCol.Index)
                        list.Columns[idx].Width = full_widths_[idx];
                    else if (idx != msgCol.Index && idx != lineCol.Index)
                        list.Columns[idx].Width = 0;
                break;
            case ui_info.show_row_type.full_row:
                for (int idx = 0; idx < list.Columns.Count; ++idx)
                    if (idx != msgCol.Index && idx != lineCol.Index)
                        list.Columns[idx].Width = full_widths_[idx];
                break;
            default:
                Debug.Assert(false);
                break;
            }
        }


        public void scroll_up() {
            if (list.GetItemCount() < 1)
                return;

            int sel = sel_row_idx;
            if (sel < 0)
                sel = 0; // assume we start from the top

            var r = list.GetItem(sel).Bounds;
            if (r.Bottom + r.Height < list.ClientRectangle.Height) {
                list.LowLevelScroll(0, -r.Height);
                list.EnsureVisible(sel);
            } else
                on_action(log_wizard.action_type.arrow_up);
        }

        public void scroll_down() {
            if (list.GetItemCount() < 1)
                return;

            int sel = sel_row_idx;
            if (sel < 0)
                sel = 0; // assume we start from the top

            var r = list.GetItem(sel).Bounds;
            if (r.Bottom + r.Height < list.ClientRectangle.Height) {
                list.LowLevelScroll(0, r.Height);
                list.EnsureVisible(sel);
            } else
                on_action(log_wizard.action_type.arrow_down);
        }

        private void list_Enter(object sender, EventArgs e) {
            //logger.Info("[view] lv got focus " + name);
            BackColor = Color.DarkSlateGray;
        }

        private void list_Leave(object sender, EventArgs e) {
            //logger.Info("[view] lv lost focus " + name);
            BackColor = Color.White;
        }

        public void set_focus() {
            list.Focus();
        }
    }
}
