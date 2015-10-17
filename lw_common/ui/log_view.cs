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
using lw_common.ui;
using LogWizard;

namespace lw_common
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

        // the reason we derive from filter.match is memory efficiency; otherwise, ew would have two lists - one in filter,
        // and one here, full of pointers
        //
        // for large files, this can add up to a LOT of memory
        internal class item : filter.match {

            public Color override_bg = util.transparent, override_fg = util.transparent;

            public item(BitArray matches, filter_line.font_info font, line line, int lineIdx, log_view parent) : base(matches, font, line, lineIdx) {
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

            public string ctx4 {
                get { return base.line.part(info_type.ctx4); }
            }
            public string ctx5 {
                get { return base.line.part(info_type.ctx5); }
            }
            public string ctx6 {
                get { return base.line.part(info_type.ctx6); }
            }
            public string ctx7 {
                get { return base.line.part(info_type.ctx7); }
            }
            public string ctx8 {
                get { return base.line.part(info_type.ctx8); }
            }
            public string ctx9 {
                get { return base.line.part(info_type.ctx9); }
            }
            public string ctx10 {
                get { return base.line.part(info_type.ctx10); }
            }
            public string ctx11 {
                get { return base.line.part(info_type.ctx11); }
            }
            public string ctx12 {
                get { return base.line.part(info_type.ctx12); }
            }
            public string ctx13 {
                get { return base.line.part(info_type.ctx13); }
            }
            public string ctx14 {
                get { return base.line.part(info_type.ctx14); }
            }
            public string ctx15 {
                get { return base.line.part(info_type.ctx15); }
            }


            public string thread {
                get { return base.line.part(info_type.thread); }
            }

            public Color sel_bg(log_view parent) {
                var bg = this.bg(parent);
                Color dark_bg = util.darker_color(bg);
                Color darker_bg = util.darker_color(dark_bg);
                var focus = win32.focused_ctrl();
                bool is_focused = focus == parent.list || focus == parent.edit;
                return is_focused ? darker_bg : dark_bg;
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

            private List<Tuple<int, int, print_info>> override_print_from_all_places(log_view parent, string text, int col_idx) {
                List<Tuple<int, int, print_info>> print = new List<Tuple<int, int, print_info>>();

                // 1.2.6 - for now, just for msg do match-color
                if (col_idx == parent.msgCol.Index) {
                    var from_filter = parent.filter_.match_indexes(base.line, info_type.msg);
                    foreach ( var ff in from_filter)
                        print.Add( new Tuple<int, int, print_info>( ff.start, ff.len, new print_info {
                            bg = ff.bg, fg = ff.fg, text = new string('?', ff.len), bold = true
                        } ));
                }

                string sel = parent.edit.sel_text.ToLower();
                if (col_idx == parent.cur_col_ && sel != "") {
                    // look for the text typed by the user
                    var matches = util.find_all_matches(text.ToLower(), sel);
                    if (matches.Count > 0) {
                        print_info print_sel = new print_info { bold = true, text = sel };
                        foreach ( var match in matches)
                            print.Add( new Tuple<int, int, print_info>(match, sel.Length, print_sel));
                    }
                }

                string find = parent.cur_search_ != null ? parent.cur_search_.text : "";
                if (col_idx == parent.msgCol.Index && find != "") {
                    var matches = string_search.match_indexes(text, parent.cur_search_);
                    if (matches.Count > 0) {
                        // if we're showing both selected text and the results of a find, differentiate them visually
                        bool italic = sel != "";
                        print_info print_sel = new print_info { text = find, bg = parent.cur_search_.bg, fg = parent.cur_search_.fg, bold = true, italic = italic };
                        foreach ( var match in matches)
                            print.Add( new Tuple<int, int, print_info>(match.Item1, match.Item2, print_sel));
                    }                    
                }
                
                if ( util.is_debug)
                    foreach ( var p in print)
                        Debug.Assert(p.Item1 >= 0 && p.Item2 >= 0);

                return print;
            }

            // returns the overrides, sorted by index in the string to print
            public List<Tuple<int, int, print_info>> override_print(log_view parent, string text, int col_idx) {
                var print = override_print_from_all_places(parent, text, col_idx);

                // for testing only
                var old_print = util.is_debug ? print.ToList() : null;

                // check for collitions
                bool collitions_found = true;
                while (collitions_found) {
                    // sort it
                    // note: I need to sort it after each collision is solved, since in imbricated prints, we can get un-sorted
                    print.Sort((x, y) => {
                        if (x.Item1 != y.Item1)
                            return x.Item1 - y.Item1;
                        // if two items at same index - first will be the one with larger len
                        return - (x.Item2 - y.Item2);
                    });

                    collitions_found = false;
                    for (int idx = 0; !collitions_found && idx < print.Count - 1; ++idx) {
                        var now = print[idx];
                        var next = print[idx + 1];

                        // special case - we split something into 3, but one of the parts was empty
                        if (now.Item2 == 0) {
                            print.RemoveAt(idx);
                            collitions_found = true;
                            continue;
                        }
                        if (next.Item2 == 0) {
                            print.RemoveAt(idx + 1);
                            collitions_found = true;
                            continue;
                        }

                        if (now.Item1 + now.Item2 > next.Item1)
                            collitions_found = true;

                        if (collitions_found) {
                            // first, see what type of collision it is
                            bool exactly_same = now.Item1 == next.Item1 && now.Item2 == next.Item2;
                            if (exactly_same)
                                // doesn't matter - just keep one
                                print.RemoveAt(idx + 1);
                            else {
                                // here - either one completely contains the other, or they just intersect
                                bool contains_fully = now.Item1 + now.Item2 >= next.Item1 + next.Item2;
                                if (contains_fully) {
                                    bool starts_at_same_idx = now.Item1 == next.Item1;
                                    if (starts_at_same_idx) {
                                        print[idx] = next;
                                        int len = next.Item2;
                                        int second_len = now.Item2 - len;
                                        Debug.Assert(second_len >= 0);
                                        print[idx + 1] = new Tuple<int, int, print_info>(now.Item1 + len, second_len, now.Item3);
                                    } else {
                                        // in this case, we need to split in 3
                                        int len1 = next.Item1 - now.Item1;
                                        int len2 = now.Item2 - len1 - next.Item2;
                                        var now1 = new Tuple<int, int, print_info>(now.Item1, len1, now.Item3);
                                        var now2 = new Tuple<int, int, print_info>(next.Item1 + next.Item2, len2, now.Item3);
                                        Debug.Assert(len1 >= 0 && len2 >= 0);
                                        print[idx] = now1;
                                        print.Insert(idx + 2, now2);
                                    }
                                } else {
                                    // they just intersect
                                    int intersect_count = now.Item1 + now.Item2 - next.Item1;
                                    Debug.Assert( intersect_count > 0);
                                    int interesect_len = now.Item2 - intersect_count;
                                    Debug.Assert(interesect_len >= 0);
                                    now = new Tuple<int, int, print_info>(now.Item1, interesect_len, now.Item3);
                                    print[idx] = now;
                                }
                            }
                        }
                    }
                }

                if ( util.is_debug)
                    foreach ( var p in print)
                        Debug.Assert(p.Item1 >= 0 && p.Item2 >= 0);

                return print;
            } 

            public filter.match match {
                get { return this;  }
            }
        };

        // the reason we have this class - is for memory eficiency - since all views (except full log) don't need the info here
        private class full_log_item : item {
            private readonly log_view parent_ = null;

            public full_log_item(BitArray matches, filter_line.font_info font, line line, int lineIdx, log_view parent) : base(matches, font, line, lineIdx, parent) {
                Debug.Assert(parent != null);
                parent_ = parent;
            }

            public override string view {
                get {
                    return parent_.lv_parent.matched_logs(line_idx);
                }
            }
        }


        private log_view_render render_;

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
        private int visible_columns_refreshed_ = 0;

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

        // it's the subitem column that is currently selected (for smart readonly editing)
        private int cur_col_ = 0;

        private bool is_editing_ = false;
        private int editing_row_ = -1;

        private search_form.search_for cur_search_ = null;
        private int cur_filter_row_idx_ = -1;

        private log_view_right_click right_click_;

        // in case the user edits and presses Escape
        private string old_view_name_ = "";

        public log_view(Form parent, string name)
        {
            Debug.Assert(parent is log_view_parent);

            filter_ = new filter(this.create_match_object);
            filter_.on_new_lines = on_new_lines;
            InitializeComponent();
            this.parent = parent;
            viewName.Text = name;
            model_ = new list_data_source(this.list, filter_.matches ) { name = name };
            list.VirtualListDataSource = model_;
            list.RowHeight = 18;

            load_font();
            lv_parent.handle_subcontrol_keys(this);

            render_ = new log_view_render(this);
            foreach (var col in list.Columns)
                (col as OLVColumn).Renderer = render_;
            right_click_ = new log_view_right_click(this);

            // just an example:
            //render_.set_override("settings", new log_view_render.print_info { fg = Color.Blue, bold = true });
            cur_col_ = msgCol.Index;
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

        private filter.match create_match_object(BitArray matches, filter_line.font_info font, line line, int lineIdx) {
            return is_full_log ? new full_log_item(matches, font, line, lineIdx, this) : new item(matches, font, line, lineIdx, this);
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

        internal item sel {
            get {
                int idx = sel_row_idx;
                if (idx >= 0)
                    return match_at(idx);
                else
                    return null;
            }
        }
        internal Rectangle sel_subrect_bounds {
            get {
                Debug.Assert(cur_col_ >= 0 && cur_col_ < list.Columns.Count);

                int sel = sel_row_idx;
                if (sel >= 0) {
                    var r = list.GetItem(sel).GetSubItemBounds(cur_col_);
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
                    return list.GetItem(sel).GetSubItem(cur_col_).Text;
                else
                    return "";
            }
        }

        // returns the line index of the current selection
        public int sel_line_idx {
            get {
                int sel = sel_row_idx;
                if (sel >= 0)
                    return match_at(sel).match.line_idx;
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
                    return match_at(sel) .match.line.part(info_type.msg);
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
        // fg and bg
        public Tuple<Color,Color> sel_line_colors {
            get {
                int sel = sel_row_idx;
                if (sel >= 0) {
                    var i = match_at(sel) ;
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
            if (m.Item1 != null)
                return list.GetItem(m.Item2);
            /*
            int count = filter_.match_count;
            for (int idx = 0; idx < count; ++idx) {
                item i = match_at(idx) as item;
                if (i.match.line_idx == line_idx)
                    return list.GetItem(idx);
            }*/
            return null;
        }

        private item match_at(int idx) {
            return filter_.matches.match_at(idx) as item;
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
            int count = filter_.match_count;
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
                        ensure_line_visible(existing);
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
                        ensure_line_visible(existing);                        
                    }
                    return;
                }
                break;

            }
            if (sel >= 0 && sel_row_idx != sel) {
                select_idx( sel, select_type.notify_parent);
                ensure_line_visible(sel);
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
            if (filter_.match_count < 1)
                return true;
            if (sel_row_idx < 0)
                return true;
            if (old_item_count_ > 0 && sel_row_idx == old_item_count_ - 1)
                return true;
            return false;
        }

        string cell_value(item i, int column_idx) {
            return log_view_cell.cell_value(i, column_idx);
        }

        private string cell_value_by_type(item i, info_type type) {
            return log_view_cell.cell_value_by_type(i, type);
        }

        OLVColumn column(info_type type) {
            return log_view_cell.column(this, type);
        }


        private bool has_value_at_column(info_type type, int max_rows_to_check) {
            int value_count = 0;
            for (int idx = 0; idx < max_rows_to_check; ++idx) {
                var i = match_at(idx) ;
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
            const int MIN_ROWS = 10;
            if (visible_columns_refreshed_ >= MIN_ROWS)
                return;

            bool needs_refresh = filter_.match_count != visible_columns_refreshed_;
            if (needs_refresh) {
                int row_count = Math.Min(filter_.match_count, MIN_ROWS);
                for (int type_as_int = 0; type_as_int < (int) info_type.max; ++type_as_int) {
                    info_type type = (info_type) type_as_int;
                    bool was_visible = column(type).Width > 0;
                    bool is_visible = false;
                    if (type == info_type.msg)
                        is_visible = true;
                    else  
                        is_visible = has_value_at_column(type, row_count);

                    if (is_visible != was_visible)
                        column(type).Width = is_visible ? 80 : 0;
                }
                visible_columns_refreshed_ = filter_.match_count;
            }

            if (filter_.match_count >= MIN_ROWS) 
                visible_columns_refreshed_ = MIN_ROWS;
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
            string y = "" + filter_.match_count;
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

        private void force_select_first_item() {
            if ( filter_.match_count > 0)
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
            last_item_count_while_current_view_ = filter_.match_count;
            tab_parent.Text += " ";
        }

        private void on_new_lines() {
            logger.Debug("[view] new lines on " + name);
            this.async_call(refresh);
        }

        public void refresh() {
            // comment the following line to easily test the UI
            //return;

            if (log_ == null)
                return; // not set yet

            bool needs_scroll = needs_scroll_to_last();
            int new_item_count = filter_.match_count;
            filter_.compute_matches(log_);

            if (is_current_view)
                last_item_count_while_current_view_ = new_item_count;

            if (old_item_count_ == new_item_count)
                return; // nothing changed
            bool more_items = old_item_count_ < new_item_count;
            old_item_count_ = new_item_count;

            model_.refresh();
            refresh_visible_columns();
            update_x_of_y();
            
            list.Refresh();
            if( needs_scroll && more_items)
                go_last();
        }



        private bool has_found_colors(int row_idx, log_view other_log, bool is_sel) {
            var i = match_at(row_idx) as full_log_item;

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

            var i = match_at(row_idx) as full_log_item;
            i.override_bg = filter_line.font_info.full_log_gray.bg;
            i.override_fg = filter_line.font_info.full_log_gray.fg;

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

        // returns the rows that are visible
        private Tuple<int, int> visible_row_indexes() {
            int PAD = 5;
            var top = list.GetItemAt(PAD, list.HeaderControl.ClientRectangle.Height + PAD);
            if (top == null)
                return new Tuple<int, int>(0,0);
            
            int top_idx = top.Index;
            int height = list.Height - list.HeaderControl.ClientRectangle.Height;
            int row_height = top.Bounds.Height;
            int rows_per_page = height / row_height;

            int bottom_idx = top_idx + rows_per_page;
            if (top_idx < 0)
                top_idx = 0;

            return new Tuple<int, int>(top_idx, bottom_idx);
        }

        public void update_colors(List<log_view> other_logs, int sel_log_view_idx, bool force_refresh = false) {
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

            if ( needs_refresh || force_refresh)
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
            var count = filter_.match_count;
            if (count > 0 ) {
                if ( ensure_line_visible(count - 1))
                    select_idx(count - 1, select_type.do_not_notify_parent);
                else 
                    // in this case, we failed to go to the last item - try again ASAP
                    util.postpone( go_last, 10);
            }
        }

        // returns true if it needed to refresh
        // FIXME 1.2.7+ probably not needed anymore, we're doing all this when rendering
        private bool update_line_color(int idx) {
            return false;
#if old_code
            if (idx >= list.GetItemCount())
                // in this case, the list hasn't fully refreshed - the filter contains more items than the list
                return false;

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
#endif
        }

        private void update_line_highlight_color(int idx) {
            /* 1.0.88+ - we have the log_view_renderer for this

            item i = match_at(idx) ;
            if (i == null)
                return;
            list.UnfocusedHighlightBackgroundColor = util.darker_color(i.bg(this));
            list.UnfocusedHighlightForegroundColor = i.fg(this);

            list.HighlightBackgroundColor = util.darker_color(util.darker_color(i.bg(this)));
            list.HighlightForegroundColor = i.fg(this);
            */
        }

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

        private void select_idx(int row_idx, select_type notify) {
            if (sel_row_idx == row_idx)
                return; // already selected

            // 1.0.67+ - there's a bug in objectlistview - if we're not the current view, we can't really select a row
            if (!is_current_view && !is_full_log)
                return;

            select_nofify_ = notify;
            if (row_idx >= 0 && row_idx < filter_.match_count) {
                logger.Debug("[view] " + name + " sel=" + row_idx);
                list.SelectedIndex = row_idx;
                update_line_highlight_color(row_idx);
                update_x_of_y();
            }
            select_nofify_ = select_type.notify_parent;
        }

        // only called by smart edit on backspace
        internal void select_cell(int row_idx, int cell_idx) {
            cur_col_ = cell_idx;
            select_idx(row_idx, select_type.notify_parent);
            lv_parent.after_search();
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e) {
            edit.update_ui();
            int sel = sel_row_idx;
            if (sel < 0)
                return;

            if (select_nofify_ == select_type.notify_parent) {
                int line_idx = match_at(sel) .match.line_idx;
                lv_parent.on_sel_line(this, line_idx);
            }

            if (is_editing_ && app.inst.edit_mode == app.edit_mode_type.with_right_arrow) {
                if (sel != editing_row_) {
                    is_editing_ = false;
                    edit.update_ui();
                }                
            }
        }

        private bool is_line_visible(int line_idx) {
            var visible = visible_row_indexes();
            // 1.1.20+ - if it's the last visible line (it's only shown partially - in that case, force it into view
            return (visible.Item1 <= line_idx && visible.Item2 - 1 >= line_idx);
        }


        private bool ensure_line_visible(int line_idx) {
            if (is_line_visible(line_idx))
                return true;
            var visible = visible_row_indexes();
            logger.Debug("[view] visible indexes for " + name + " : " + visible.Item1 + " - " + visible.Item2);
            // 1.1.15+ note : this sometimes flickers, we want to avoid this as much as possible

            if (line_idx >= list.GetItemCount())
                // can happen if list isn't fully refreshed
                return false;

            try {
                list.EnsureVisible(line_idx);
                return true;
            } catch {
                return false;
            }
        }

        public void go_to_row(int row_idx, select_type notify) {
            if (row_idx >= filter_.match_count)
                return;

            select_idx(row_idx, notify);
            if (is_line_visible(row_idx))
                // already visible
                return;

            int rows = list.Height / list.RowHeight;
            int bottom_idx = row_idx + rows / 2;
            if (bottom_idx >= filter_.match_count)
                bottom_idx = filter_.match_count - 1;
            int top_idx = bottom_idx - rows;
            if (top_idx < 0)
                top_idx = 0;
            // we want to show the line in the *middle* of the control (height-wise)
            if( top_idx < list.GetItemCount())
                ensure_line_visible(top_idx);
            if( bottom_idx < list.GetItemCount())
                ensure_line_visible(bottom_idx);
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
            return filter_.matches.binary_search(line_idx).Item1 != null;
        }

        public void go_to_closest_line(int line_idx, select_type notify) {
            if (filter_.match_count < 1)
                return;
            
            var closest = filter_.matches.binary_search_closest(line_idx);
            if ( closest.Item2 >= 0)
                go_to_row(closest.Item2, notify);
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
                go_to_row(closest.Item2, select_type.notify_parent);
        }

        public void offset_closest_time(int time_ms, bool forward) {
            if (filter_.match_count < 1)
                return;
            int sel = sel_row_idx;
            if (sel < 0)
                sel = 0; // just in case we haven't selected anything - start from beginning
            var i = match_at(sel);
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
            get { return filter_.match_count; }
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
                item i = match_at(idx) ;
                i.override_fg = util.transparent;
                i.override_bg = util.transparent;
            }
            list.Refresh();
        }

        private msg_details_ctrl msg_details {
            get {
                foreach ( var ctrl in parent.Controls)
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
            else if ( cur_filter_row_idx_ >= 0)
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
            else if ( cur_filter_row_idx_ >= 0)
                search_for_prev_match(cur_filter_row_idx_);
            lv_parent.after_search();
        }

        // note: starts from the next row, or, if on row zero -> starts from row zero
        public void search_for_text_first() {
            if (filter_.match_count < 1)
                return;
            Debug.Assert(cur_search_ != null);
            if (cur_search_ == null)
                return;

            // make sure f3/shift-f3 will work on the current search (cur_search_), not on the currently selected word(s)
            edit.escape();

            select_idx(0, select_type.notify_parent);
            item i = match_at(0) ;
            bool include_row_zero = sel_row_idx == 0 || sel_row_idx == -1;
            if ( include_row_zero && string_search.matches( i.match.line.part(info_type.msg), cur_search_)) {
                // line zero contains the text already
                ensure_line_visible(0);
                lv_parent.after_search();
            } else
                search_for_text_next();
        }

        private void search_for_text_next() {
            int count = filter_.match_count;
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
                    string cur = list.GetItem(idx).GetSubItem(cur_col_).Text;
                    if (cur.ToLower().Contains(sel_text))
                        found = idx;
                    continue;
                }

                item i = match_at(idx) ;
                if (string_search.matches( i.match.line.part(info_type.msg), cur_search_))
                    found = idx;
            }

            if (found >= 0) {
                select_idx(found, select_type.notify_parent);
                ensure_line_visible(found);
            } else
                util.beep(util.beep_type.err);
        }

        private void search_for_text_prev() {
            int count = filter_.match_count;
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
                    string cur = list.GetItem(idx).GetSubItem(cur_col_).Text;
                    if (cur.ToLower().Contains(sel_text))
                        found = idx;
                    continue;
                }

                item i = match_at(idx) ;
                if (string_search.matches( i.match.line.part(info_type.msg), cur_search_))
                    found = idx;
            }
            if (found >= 0) {
                select_idx(found, select_type.notify_parent);
                ensure_line_visible(found);
            } else
                util.beep(util.beep_type.err);
        }

        public void mark_match(int filter_row_idx, Color fg, Color bg) {
            cur_filter_row_idx_ = filter_row_idx;
            bool needs_refresh = false;
            int count = filter_.match_count;
            for (int idx = 0; idx < count; ++idx) {
                item i = match_at(idx) ;
                bool is_match = filter_row_idx >= 0 && i.match.matches.Length > filter_row_idx && i.match.matches[filter_row_idx];
                bool needs_change = 
                    (is_match && (i.override_fg.ToArgb() != fg.ToArgb() || i.override_bg.ToArgb() != bg.ToArgb())) || 
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

        private void search_for_next_match(int filter_row_idx) {
            int count = filter_.match_count;
            if (count < 1)
                return;
            int found = -1;
            int next_row = sel_row_idx >= 0 ? sel_row_idx + 1 : 0;
            for (int idx = next_row; idx < count && found < 0; ++idx) {
                item i = match_at(idx) ;
                bool is_match = i.match.matches.Length > filter_row_idx && i.match.matches[filter_row_idx];
                if (is_match)
                    found = idx;
            }

            if (found >= 0) {
                select_idx(found, select_type.notify_parent);
                ensure_line_visible(found);
            } else
                util.beep(util.beep_type.err);
        }

        private void search_for_prev_match(int filter_row_idx) {
            if (filter_.match_count < 1)
                return;
            int found = -1;
            int prev_row = sel_row_idx >= 0 ? sel_row_idx - 1 : filter_.match_count - 1;
            for (int idx = prev_row; idx >= 0 && found < 0; --idx) {
                item i = match_at(idx) ;
                bool is_match = i.match.matches.Length > filter_row_idx && i.match.matches[filter_row_idx];
                if (is_match)
                    found = idx;
            }

            if (found >= 0) {
                select_idx(found, select_type.notify_parent);
                ensure_line_visible(found);
            } else
                util.beep(util.beep_type.err);
        }

        public export_text export(List<int> indices, bool msg_only) {
            export_text export = new export_text();

            int row_idx = 0;
            foreach ( int idx in indices) {
                item i = match_at(idx) ;

                int visible_idx = 0;
                string font = list.Font.Name;
                for (int column_idx = 0; column_idx < list.Columns.Count; ++column_idx) {
                    bool do_print = list.Columns[column_idx].Width > 0;
                    if (msg_only)
                        do_print = column_idx == msgCol.Index;
                    if (do_print) {
                        string txt = cell_value(i, column_idx);
                        export_text.cell c = new export_text.cell(row_idx, visible_idx, txt) { fg = i.fg(this), bg = i.bg(this), font = font, font_size = 7 };
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
            clipboard_util.copy( html, text );

#if old_code
            string full = "";
            foreach (int row in sel) {
                item i = match_at(row) ;
                if (full != "")
                    full += "\r\n";
                full += i.match.line.part(info_type.msg);
            }

            try {
                Clipboard.SetText(full);
            } catch {
            }
#endif
        }

        public void copy_full_line_to_clipboard() {
            var sel = selected_indices_array();
            if (sel.Count < 1)
                return;

            var export = this.export(sel, false);
            string html = export.to_html(), text = export.to_text();
            clipboard_util.copy( html, text );

#if old_code
            string full = "";
            foreach (int row in sel) {
                item i = match_at(row) ;
                if (full != "")
                    full += "\r\n";
                full += i.match.line.full_line;
            }

            try {
                Clipboard.SetText(full);
            } catch {
            }
#endif
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
                ensure_line_visible(idx);
            } else
                util.beep(util.beep_type.err);
        }

        public void prev_bookmark() {
            if (filter_.match_count < 1)
                return;

            int start = sel_row_idx >= 0 ? sel_line_idx - 1 : match_at(sel_row_idx).match.line_idx;
            int mark = bookmarks_.LastOrDefault(line => line <= start && row_by_line_idx(line) != null);
            if (mark == 0)
                if (!bookmarks_.Contains(mark))
                    // in this case, we did not find anything and got returned default (0)
                    mark = -1;

            if (mark >= 0) {
                int idx = row_by_line_idx(mark).Index;
                select_idx(idx, select_type.notify_parent);
                ensure_line_visible(idx);
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
                // save_to them now
                for (int idx = 0; idx < list.Columns.Count; ++idx)
                    full_widths_.Add(list.Columns[idx].Width);                
            
            logger.Info("[view] showing rows - " + name + " = " + show );

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
            if (filter_.match_count < 1)
                return;

            int sel = sel_row_idx;
            if (sel < 0)
                sel = 0; // assume we start from the top

            var r = list.GetItem(sel).Bounds;
            if (r.Bottom + r.Height < list.ClientRectangle.Height) {
                list.LowLevelScroll(0, -r.Height);
                ensure_line_visible(sel);
            } else
                on_action(action_type.arrow_up);
        }

        public void scroll_down() {
            if (filter_.match_count < 1)
                return;

            int sel = sel_row_idx;
            if (sel < 0)
                sel = 0; // assume we start from the top

            var r = list.GetItem(sel).Bounds;
            if (r.Bottom + r.Height < list.ClientRectangle.Height) {
                list.LowLevelScroll(0, r.Height);
                ensure_line_visible(sel);
            } else
                on_action(action_type.arrow_down);
        }

        private void list_Enter(object sender, EventArgs e) {
            update_background();
            edit.update_ui();
        }

        private void list_Leave(object sender, EventArgs e) {
            // note: we might actually lose focus to the edit box - in this case, we don't really need to update anything
            util.postpone(update_background,10);
        }

        internal void update_background() {
            var focus = win32.focused_ctrl();
            bool here = focus == list || focus == edit;

            var color = here ? Color.DarkSlateGray : Color.White;
            if ( BackColor.ToArgb() != color.ToArgb())
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
                item i = match_at(idx) ;

                int visible_idx = 0;
                string font = list.Font.Name;
                for (int column_idx = 0; column_idx < list.Columns.Count; ++column_idx) {
                    if (list.Columns[column_idx].Width > 0) {
                        string txt = cell_value(i, column_idx);
                        export_text.cell c = new export_text.cell(idx, visible_idx, txt) { fg = i.fg(this), bg = i.bg(this), font = font, font_size = 7 };
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

        internal log_view_right_click right_click {
            get { return right_click_; }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            bool is_renaming = win32.focused_ctrl() == viewName;

            if (!is_editing) {
                // see if the current key will start editing
                if (keyData == Keys.Space && app.inst.edit_mode == app.edit_mode_type.with_space) {
                    is_editing_ = true;
                    cur_col_ = msgCol.Index;
                    edit.update_ui();
                    return true;
                } 
                else if (keyData == Keys.Right && app.inst.edit_mode == app.edit_mode_type.with_right_arrow) {
                    is_editing_ = true;
                    editing_row_ = sel_row_idx;
                    cur_col_ = msgCol.Index;
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
                    if ( is_renaming)
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
                            for (int column_idx = 0; column_idx < list.Columns.Count; ++column_idx) {
                                int next = is_left ? (cur_col_ - column_idx - 1 + list.Columns.Count) % list.Columns.Count : (cur_col_ + column_idx + 1) % list.Columns.Count;
                                if (list.Columns[next].Width > 0) {
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
            string col_text = list.GetItem(row).GetSubItem(col).Text.ToLower();
            int pos = col_text.IndexOf(txt);
            return pos >= 0;            
        }

        // note: searches in the current column
        private void search_ahead(string txt) {
            Debug.Assert(txt == txt.ToLower());

            int count = filter_.match_count;
            int max = Math.Min(sel_row_idx + SEARCH_AROUND_ROWS, count);
            int min = Math.Max(sel_row_idx - SEARCH_AROUND_ROWS, 0);

            // note: even if we search all columns, we first search ahead/before for the selected column - then, the rest
            int found_row = -1;
            if ( app.inst.edit_search_after)
                for (int cur_row = sel_row_idx + 1; cur_row < max && found_row < 0; ++cur_row)
                    if (can_find_text_at_row(txt, cur_row, cur_col_))
                        found_row = cur_row;
                
            if ( app.inst.edit_search_before)
                for (int cur_row = sel_row_idx - 1; cur_row >= min && found_row < 0; --cur_row) 
                    if (can_find_text_at_row(txt, cur_row, cur_col_)) 
                        found_row = cur_row;

            if (app.inst.edit_search_all_columns) {
                if ( app.inst.edit_search_after)
                    for (int cur_row = sel_row_idx + 1; cur_row < max && found_row < 0; ++cur_row)
                        for ( int col_idx = 0; col_idx < list.Columns.Count && found_row < 0; ++col_idx)
                            if ( col_idx != cur_col_ && list.Columns[col_idx].Width > 0)
                                if (can_find_text_at_row(txt, cur_row, col_idx)) {
                                    found_row = cur_row;
                                    cur_col_ = col_idx;
                                }

                if ( app.inst.edit_search_before)
                    for (int cur_row = sel_row_idx - 1; cur_row >= min && found_row < 0; --cur_row) 
                        for ( int col_idx = 0; col_idx < list.Columns.Count && found_row < 0; ++col_idx)
                            if ( col_idx != cur_col_ && list.Columns[col_idx].Width > 0)
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
            if ( (e.Button & MouseButtons.Right) == MouseButtons.Right)
                right_click_.right_click();
        }
    }
}
