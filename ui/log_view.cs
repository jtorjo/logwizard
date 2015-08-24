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
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace LogWizard
{
    partial class log_view : UserControl
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private log_wizard parent;
        private filter filter_ = new filter();
        private log_line_reader log_ = null;

        private bool filter_changed_ = false;
        private string selected_view_ = null;

        private class item {
            private filter.match match_;
            private readonly log_view parent_ = null;
            public int row = 0;

            public Color override_bg = util.transparent, override_fg = util.transparent;

            public item(filter.match match, log_view parent) {
                match_ = match;
                parent_ = parent;
            }

            public int line {
                get { return match_.line_idx + 1; }
            }

            // if non-null, it contains the names of the logs it matches (set to a valid value only on the full-log)
            public List<string> matched_logs = null;

            public string view {
                get {
                    if ( matched_logs == null)
                        return "";
                    if (matched_logs.Count == 1)
                        return matched_logs[0]; // optimization

                    string views = "";
                    bool has_selected_view = false;
                    foreach (string name in matched_logs) {
                        if (name == parent_.selected_view_) {
                            has_selected_view = true;
                            continue;
                        }
                        if (views != "") views += ", ";
                        views += name;
                    }
                    if ( has_selected_view)
                        views = parent_.selected_view_ + ", " + views;
                    return views;
                }
            }

            public string date {
                get { return match_.line.part(info_type.date); }
            }
            public string time {
                get { return match_.line.part(info_type.time); }
            }
            public string level {
                get { return match_.line.part(info_type.level); }
            }
            public string msg {
                get { return match_.line.part(info_type.msg); }
            }

            public string file {
                get { return match_.line.part(info_type.file); }
            }
            public string func {
                get { return match_.line.part(info_type.func); }
            }
            public string class_ {
                get { return match_.line.part(info_type.class_); }
            }
            public string ctx1 {
                get { return match_.line.part(info_type.ctx1); }
            }
            public string ctx2 {
                get { return match_.line.part(info_type.ctx2); }
            }
            public string ctx3 {
                get { return match_.line.part(info_type.ctx3); }
            }
            public string thread {
                get { return match_.line.part(info_type.thread); }
            }

            public Color bg {
                get {
                    if (parent_.bookmarks_.Contains(match.line_idx))
                        return parent_.bookmark_bg_;
                    if (override_bg != util.transparent)
                        return override_bg;
                    return match_.font.bg;
                }
            }

            public Color fg {
                get {
                    if (parent_.bookmarks_.Contains(match.line_idx))
                        return parent_.bookmark_fg_;
                    if (override_fg != util.transparent)
                        return override_fg;
                    return match_.font.fg;
                }
            }

            public filter.match match {
                get { return match_; }
                set {
                    Debug.Assert(value != null);
                    match_ = value;
                }
            }
        };

        private class list_data_source : AbstractVirtualListDataSource {
            private List<item> items_ = new List<item>();
            private VirtualObjectListView lv_ = null;

            public list_data_source(VirtualObjectListView lv) : base(lv) {
                lv_ = lv;
            }

            public override int GetObjectIndex(object model) {
                return items_.IndexOf(model as item);
            }

            public override object GetNthObject(int n) {
                return n < items_.Count ? items_[n] : null;
            }

            public override int GetObjectCount() {
                return items_.Count;
            }

            public void add_matches(List<filter.match> matches, log_view parent) {
                foreach ( var m in matches)
                    items_.Add(new item(m,parent) { row = items_.Count + 1 });

                if ( matches.Count == 0)
                    lv_.ClearObjects();

                lv_.UpdateVirtualListSize();
            }

            public void set_matches(List<filter.match> matches, log_view parent) {
                items_.Clear();
                add_matches(matches, parent);
            }
        }

        private list_data_source model_ = null;
        private bool visible_columns_refreshed_ = false;

        private Font font_ = null;

        private int last_view_column_index_ = 0;

        // lines that are bookmarks (sorted by index)
        private List<int> bookmarks_ = new List<int>();

        private Color bookmark_bg_ = Color.Blue, bookmark_fg_ = Color.White;

        private int font_size_ = 9; // default font size

        private List<int> old_widths_ = new List<int>();

        // if true, we're waiting for the filter to read from the NEW log
        private bool wait_for_filter_to_read_from_new_log_ = false;

        public log_view(log_wizard parent, string name)
        {
            InitializeComponent();
            this.parent = parent;
            viewName.Text = name;
            model_ = new list_data_source(this.list);
            list.VirtualListDataSource = model_;
            list.RowHeight = 18;

            load_font();
            parent.handle_subcontrol_keys(this);
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

        // returns the line index of the current selection
        public int sel_line_idx {
            get {
                int sel = list.SelectedIndex;
                if (sel >= 0)
                    return (list.GetItem(sel).RowObject as item).match.line_idx;
                return -1;
            }
        }

        public string sel_line_text {
            get {
                int sel = list.SelectedIndex;
                if (sel >= 0)
                    return (list.GetItem(sel).RowObject as item).match.line.part(info_type.msg);
                return "";
            }
        }
        public Rectangle sel_rect {
            get {
                int sel = list.SelectedIndex;
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
                int sel = list.SelectedIndex;
                if (sel >= 0) {
                    var i = list.GetItem(sel).RowObject as item;
                    return new Tuple<Color, Color>(i.fg, i.bg);
                }
                return new Tuple<Color, Color>(Color.Black,Color.White);
            }
        }

        public void set_filter(List<filter_row> filter) {
            filter_.update_rows(filter);
            if (filter_.rows_changed)
                filter_changed_ = true;
        }

        internal void set_log(log_line_reader log) {
            if (log_ != log) {
                log_ = log;
                filter_changed_ = true;
                visible_columns_refreshed_ = false;
                last_view_column_index_ = 0;
                wait_for_filter_to_read_from_new_log_ = true;
                logger.Debug("[view] new log for " + name + " - " + log.name);
                model_.set_matches(new List<filter.match>(), this);
                update_x_of_y();
            }
        }

        public string name {
            get { return viewName.Text; }
            set { viewName.Text = value; }
        }

        public void show_name(bool show) {
            bool shown = list.Top - 10 > labelName.Top;
            if ( shown == show)
                return;
            
            viewName.Visible = show;
            labelName.Visible = show;
 
            int height = viewName.Height + 5;
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
                break;
            default:
                Debug.Assert(false);
                return;
            }

            int sel = list.SelectedIndex;
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
                    sel = list.HitTest(middle).Item.Index;
                }
                break;
            case log_wizard.action_type.pagedown:
                if (sel < list.GetItemCount() - 1) {
                    var r = list.GetItem(sel).Bounds;
                    var middle = new Point( r.Left + r.Width / 2, r.Top + r.Height / 2 );
                    list.LowLevelScroll(0, height);
                    sel = list.HitTest(middle).Item.Index;
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
            }
            if (sel >= 0 && list.SelectedIndex != sel) {
                select_idx( sel, select_type.notify_parent);
                list.EnsureVisible(sel);
            }
        }

        private bool needs_scroll_to_last() {
            if (list.GetItemCount() < 1)
                return true;
            if (list.SelectedIndex < 0)
                return true;
            if (list.SelectedIndex == list.GetItemCount() - 1)
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
                visible_columns_refreshed_ = true;

                for ( int type_as_int = 0; type_as_int < (int)info_type.max; ++type_as_int)
                    if (type_as_int != (int) info_type.msg) {
                        info_type type = (info_type) type_as_int;
                        int value_count = 0;
                        for (int idx = 0; idx < MIN_ROWS; ++idx)
                            if (column_value(list.GetItem(idx).RowObject as item, type) != "")
                                ++value_count;
                        bool has_values = (value_count > 0);
                        bool is_visible = column(type).Width > 0;
                        if (has_values != is_visible) 
                            column(type).Width = has_values ? 80 : 0;                        
                    }
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

        public void update_x_of_y() {
            string x_idx =  list.SelectedIndex >= 0 ? "" + (list.SelectedIndex+1) : "";
            string x_line =  sel_line_idx >= 0 ? "" + (sel_line_idx + 1) : "";
            string y = "" + list.GetItemCount();
            string header = (app.inst.show_view_line_count || app.inst.show_view_selected_line || app.inst.show_view_selected_index ? (x_idx != "" ? x_idx + " of " + y : "(" + y + ")") : "");
            string x_of_y_msg = "Message " + header;
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

            var parent = Parent as TabPage;
            if (parent != null)
                parent.Text = name + x_of_y_title;

            msgCol.Text = x_of_y_msg;
        }

        private void force_select_first_item() {
            if ( list.GetItemCount() > 0)
                if (list.SelectedIndex < 0) {
                    list.SelectedIndex = 0;
                    logger.Debug("[view] forcing sel zero for " + name);
                }
        }

        public void refresh() {
            // comment the following line to easily test the UI
            //return;

            if (log_ == null)
                return; // not set yet

            bool needs_scroll = needs_scroll_to_last();
            filter_.compute_matches(log_);

            if (wait_for_filter_to_read_from_new_log_) {
                if (filter_.log != log_)
                    return;
                logger.Debug("[view] filter refreshed after log changed " + name);
            }
            wait_for_filter_to_read_from_new_log_ = false;

            if (!filter_changed_) {
                int match_count = filter_.match_count;
                if (list.GetItemCount() == match_count)
                    // nothing changed
                    return;
                logger.Debug("[view] log " + viewName.Text + ": going from " + list.GetItemCount() + " to " + match_count + " entries.");
                if (list.GetItemCount() < match_count) {
                    // items have been added
                    List<filter.match> new_ = new List<filter.match>();
                    bool filter_reset = false;
                    for (int i = list.GetItemCount(); i < match_count && !filter_reset; ++i) {
                        var new_match = filter_.match_at(i);
                        if (new_match != null)
                            new_.Add(new_match);
                        else
                            filter_reset = true; // filter got reset in the other thread
                    }
                    if (filter_reset) {
                        logger.Debug("[view] filter reset on refresh " + name);
                        new_ = new List<filter.match>();
                    }
                    model_.add_matches(new_, this);
                    update_x_of_y();
                    force_select_first_item();
                    logger.Debug("[view] log " + viewName.Text + " has " + model_.GetObjectCount() + " entries.");
                } else
                    // less items than we have shown to the user? either the file has been erased, or cleared-and-re-written to, so we process it as a full-blown filter change
                    filter_changed_ = true;
            }

            if (filter_changed_)
                on_filter_changed();
            filter_changed_ = false;

            refresh_visible_columns();

            if( needs_scroll)
                go_last();
        }

        // returns all the lines that match this filter
        public List<int> matched_lines(int start_line_idx, int end_line_idx) {
            List<int> lines = new List<int>();
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                item i = list.GetItem(idx).RowObject as item;
                if ( i.match.line_idx >= start_line_idx && i.match.line_idx < end_line_idx)
                    lines.Add(i.match.line_idx);
            }
            return lines;
        }

        public void recompute_view_column() {
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                item i = list.GetItem(idx).RowObject as item;
                i.matched_logs = null;
            }
            last_view_column_index_ = 0;
        }

        public void update_view_column(List<log_view> other_logs) {
            // in this case, everything needs to be up to date
            foreach (log_view other in other_logs)
                other.refresh();

            int new_view_column_idx_ = list.GetItemCount();
            if (last_view_column_index_ == new_view_column_idx_)
                return;
            else if (last_view_column_index_ > new_view_column_idx_)
                // probably file got re-written
                last_view_column_index_ = 0;

            bool[,] matches = new bool[ new_view_column_idx_ - last_view_column_index_ , other_logs.Count];
            int log_idx = 0;
            foreach (log_view other in other_logs) {
                var matched = other.matched_lines(last_view_column_index_, new_view_column_idx_);
                foreach (int line_idx in matched)
                    matches[line_idx - last_view_column_index_, log_idx] = true ;
                ++log_idx;
            }

            for (int idx = last_view_column_index_; idx < new_view_column_idx_; ++idx) {
                item i = list.GetItem(idx).RowObject as item;
                List<string> log_names = null;
                for (int j = 0; j < other_logs.Count; ++j) 
                    if ( matches[idx - last_view_column_index_, j] ) {
                        if ( log_names == null)
                            log_names = new List<string>();
                        log_names.Add(other_logs[j].viewName.Text);
                    }
                i.matched_logs = log_names;
            }
            last_view_column_index_ = new_view_column_idx_;
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
                select_idx( count - 1, select_type.do_not_notify_parent);
            }
        }

        private void update_line_color(int idx) {
            var row = list.GetItem(idx);
            item i = row.RowObject as item;

            if ( row.BackColor.ToArgb() != i.bg.ToArgb())
                row.BackColor = i.bg;

            if ( row.ForeColor.ToArgb() != i.fg.ToArgb())
                row.ForeColor = i.fg;
        }

        private void update_line_highlight_color(int idx) {
            item i = list.GetItem(idx).RowObject as item;
            list.UnfocusedHighlightBackgroundColor = util.darker_color(i.bg);
            list.UnfocusedHighlightForegroundColor = i.fg;

            list.HighlightBackgroundColor = util.darker_color( util.darker_color(i.bg));
            list.HighlightForegroundColor = i.fg;
        }

        private void on_filter_changed() {
            logger.Debug("[view] filter changed on " + name + " - " + list.GetItemCount() + " items so far");
            bool needs_scroll = needs_scroll_to_last();

            // from this point on, we only append to the existing list
            List<filter.match> new_ = new List<filter.match>();
            int match_count = filter_.match_count;
            bool filter_reset = false;
            for (int i = 0; i < match_count && !filter_reset; ++i) {
                var new_match = filter_.match_at(i);
                if (new_match != null)
                    new_.Add(new_match);
                else
                    filter_reset = true; // filter got reset in the other thread
            }
            if ( !filter_reset)
                model_.set_matches(new_, this);

            // update colors
            for (int idx = 0; idx < list.GetItemCount() && !filter_reset; ++idx) {
                item i = list.GetItem(idx).RowObject as item;
                var match = filter_.match_at(idx);
                if (match != null)
                    i.match = match;
                else
                    filter_reset = true; // filter got reset in the other thread
            }

            if (!filter_reset) {
                for (int idx = 0; idx < list.GetItemCount(); ++idx)
                    update_line_color(idx);
                list.Refresh();
            } else {
                logger.Debug("[view] filter got reset on the other thread - " + name);
                model_.set_matches(new List<filter.match>(), this);
            }

            if( needs_scroll)
                go_last();
            else 
                force_select_first_item();
            update_x_of_y();
        }

        private void list_FormatCell_1(object sender, FormatCellEventArgs e) {

        }

        private void list_FormatRow_1(object sender, FormatRowEventArgs e) {
            item i = e.Item.RowObject as item;
            if (i != null) {
                e.Item.BackColor = i.bg;
                e.Item.ForeColor = i.fg;
            }
        }

        public enum select_type {
            notify_parent, do_not_notify_parent
        }
        // by default, notify parent
        private select_type select_nofify_ = select_type.notify_parent;
        private void select_idx(int idx, select_type notify) {
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
            int sel = list.SelectedIndex;
            if (sel < 0)
                return;

            if (select_nofify_ == select_type.notify_parent) {
                int line_idx = (list.GetItem(sel).RowObject as item).match.line_idx;
                parent.go_to_line(line_idx, this);
            }
        }

        public void go_to_line(int line_idx, select_type notify) {
            if (line_idx >= list.GetItemCount())
                return;

            select_idx( line_idx, notify);
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

        public void go_to_closest_line(int line_idx) {
            if (list.GetItemCount() < 1)
                return;

            // note: yeah - i could do binary search, but it's not that big of a time increase
            int last_line_idx = (list.GetItem(0).RowObject as item).match.line_idx;
            int found_idx = 0;
            for (int idx = 1; idx < list.GetItemCount(); ++idx) {
                var cur_line_idx = (list.GetItem(idx).RowObject as item).match.line_idx;
                int last_dist = Math.Abs(line_idx - last_line_idx);
                int cur_dist = Math.Abs(cur_line_idx - line_idx);
                if (cur_dist < last_dist) {
                    last_line_idx = cur_line_idx;
                    found_idx = idx;
                } else
                    // we found it
                    break;
            }
            go_to_line(found_idx, select_type.do_not_notify_parent);
        }

        public int line_count {
            get { return list.GetItemCount(); }
        }

        public int filter_row_count {
            get { return filter_.row_count; }
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

        public void mark_text(string txt, Color fg, Color bg) {
            Debug.Assert(list.GetItemCount() == filter_.match_count);

            // just in case a filter was selected
            unmark();

            bool needs_refresh = false;
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                item i = list.GetItem(idx).RowObject as item;
                if (i.match.line.part(info_type.msg).Contains(txt)) {
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

        public void search_for_text_first(string txt) {
            if (list.GetItemCount() < 1)
                return;

            select_idx( 0, select_type.notify_parent);
            item i = list.GetItem(0).RowObject as item;
            if (i.match.line.part(info_type.msg).Contains(txt)) {
                // line zero contains the text already
                list.EnsureVisible(0);
                return;
            }
            else 
                search_for_text_next(txt);
            
        }

        public void search_for_text_next(string txt) {
            if (list.GetItemCount() < 1)
                return;
            int found = -1;
            if ( list.SelectedIndex >= 0)
                for (int idx = list.SelectedIndex + 1; idx < list.GetItemCount() && found < 0; ++idx) {
                    item i = list.GetItem(idx).RowObject as item;
                    if (i.match.line.part(info_type.msg).Contains(txt))
                        found = idx;
                }
            if (found >= 0) {
                select_idx( found, select_type.notify_parent);
                list.EnsureVisible(found);
            } else
                System.Media.SystemSounds.Asterisk.Play();
        }

        public void search_for_text_prev(string txt) {
            if (list.GetItemCount() < 1)
                return;
            int found = -1;
            if ( list.SelectedIndex > 0)
                for (int idx = list.SelectedIndex - 1; idx >= 0 && found < 0; --idx) {
                    item i = list.GetItem(idx).RowObject as item;
                    if (i.match.line.part(info_type.msg).Contains(txt))
                        found = idx;
                }
            if (found >= 0) {
                select_idx( found, select_type.notify_parent);
                list.EnsureVisible(found);
            } else
                System.Media.SystemSounds.Asterisk.Play();            
        }

        public void mark_match(int filter_row_idx, Color fg, Color bg) {
            Debug.Assert(list.GetItemCount() == filter_.match_count);

            bool needs_refresh = false;
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                item i = list.GetItem(idx).RowObject as item;
                bool is_match = i.match.matches.Length > filter_row_idx && i.match.matches[filter_row_idx];
                bool needs_change = (is_match && (i.override_fg.ToArgb() != fg.ToArgb() || i.override_bg.ToArgb() != bg.ToArgb())) ||
                                    (!is_match && (i.override_fg != util.transparent || i.override_bg != util.transparent));

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
            if ( list.SelectedIndex >= 0)
                for (int idx = list.SelectedIndex + 1; idx < list.GetItemCount() && found < 0; ++idx) {
                    item i = list.GetItem(idx).RowObject as item;
                    bool is_match = i.match.matches.Length > filter_row_idx && i.match.matches[filter_row_idx];
                    if (is_match)
                        found = idx;
                }
            if (found >= 0) {
                select_idx( found, select_type.notify_parent);
                list.EnsureVisible(found);
            } else
                System.Media.SystemSounds.Asterisk.Play();
            
        }
        public void search_for_prev_match(int filter_row_idx) {
            if (list.GetItemCount() < 1)
                return;
            int found = -1;
            if ( list.SelectedIndex > 0)
                for (int idx = list.SelectedIndex - 1; idx >= 0 && found < 0; --idx) {
                    item i = list.GetItem(idx).RowObject as item;
                    bool is_match = i.match.matches.Length > filter_row_idx && i.match.matches[filter_row_idx];
                    if (is_match)
                        found = idx;
                }
            if (found >= 0) {
                select_idx( found, select_type.notify_parent);
                list.EnsureVisible(found);
            } else
                System.Media.SystemSounds.Asterisk.Play();
        }

        public void copy_to_clipboard() {
            int sel = list.SelectedIndex;
            if (sel < 0)
                return;
            item i = list.GetItem(sel).RowObject as item;
            try {
                Clipboard.SetText( i.match.line.part(info_type.msg));
            } catch {
            }
        }

        public void copy_full_line_to_clipboard() {
            int sel = list.SelectedIndex;
            if (sel < 0)
                return;
            item i = list.GetItem(sel).RowObject as item;
            try {
                Clipboard.SetText( i.match.line.full_line);
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
            if ( list.SelectedIndex >= 0)
                update_line_highlight_color(list.SelectedIndex);
        }

        public void next_bookmark() {
            int start = list.SelectedIndex >= 0 ? sel_line_idx + 1 : 0;
            int mark = bookmarks_.FirstOrDefault(line => line >= start && row_by_line_idx(line) != null );
            if ( mark == 0)
                if (mark < start || !bookmarks_.Contains(mark))
                    // in this case, we did not find anything and got returned default (0)
                    mark = -1;

            if (mark >= 0) {
                int idx = row_by_line_idx(mark).Index;
                select_idx(idx, select_type.notify_parent);
                list.EnsureVisible(idx);
            } else
                System.Media.SystemSounds.Asterisk.Play();
        }

        public void prev_bookmark() {
            if (list.GetItemCount() < 1)
                return;

            int start = list.SelectedIndex >= 0 ? sel_line_idx - 1 : (list.GetItem(list.SelectedIndex).RowObject as item).match.line_idx ;
            int mark = bookmarks_.LastOrDefault(line => line <= start && row_by_line_idx(line) != null);
            if ( mark == 0)
                if (!bookmarks_.Contains(mark))
                    // in this case, we did not find anything and got returned default (0)
                    mark = -1;

            if (mark >= 0) {
                int idx = row_by_line_idx(mark).Index;
                select_idx(idx, select_type.notify_parent);
                list.EnsureVisible(idx);
            }
            else
                System.Media.SystemSounds.Asterisk.Play();            
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

        public void toggle_show_msg_only() {
            if (old_widths_.Count < 1) {
                // hide them
                for (int idx = 0; idx < list.Columns.Count; ++idx)
                    old_widths_.Add(list.Columns[idx].Width);
                for (int idx = 0; idx < list.Columns.Count; ++idx)
                    if (idx != msgCol.Index)
                        list.Columns[idx].Width = 0;
            } else {
                // show them
                Debug.Assert(list.Columns.Count == old_widths_.Count);
                for (int idx = 0; idx < list.Columns.Count; ++idx)
                    if (idx != msgCol.Index)
                        list.Columns[idx].Width = old_widths_[idx];
                old_widths_.Clear();
            }
        }

        public void scroll_up() {
            if (list.GetItemCount() < 1)
                return;

            int sel = list.SelectedIndex;
            if (sel < 0)
                sel = 0; // assume we start from the top

            var r = list.GetItem(sel).Bounds;
            if (r.Bottom + r.Height < list.ClientRectangle.Height) {
                list.LowLevelScroll(0, -r.Height);
                list.EnsureVisible(sel);
            }
            else 
                on_action(log_wizard.action_type.arrow_up);
        }

        public void scroll_down() {
            if (list.GetItemCount() < 1)
                return;

            int sel = list.SelectedIndex;
            if (sel < 0)
                sel = 0; // assume we start from the top

            var r = list.GetItem(sel).Bounds;
            if (r.Bottom + r.Height < list.ClientRectangle.Height) {
                list.LowLevelScroll(0, r.Height);
                list.EnsureVisible(sel);
            }
            else 
                on_action(log_wizard.action_type.arrow_down);            
        }

        private void list_Enter(object sender, EventArgs e) {
            //logger.Info("[view] lv got focus " + name);
            BackColor = Color.LightGray;
        }

        private void list_Leave(object sender, EventArgs e) {
            //logger.Info("[view] lv lost focus " + name);
            BackColor = Color.White;
        }
    }

}
