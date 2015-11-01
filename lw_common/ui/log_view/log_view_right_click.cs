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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Windows.Forms;
using LogWizard;

namespace lw_common.ui {
    public class log_view_right_click {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private log_view parent_;

        private bool via_apps_hotkey_ = false;

        private delegate bool bool_func();

        private List<Tuple<string, int>> other_views_ = null; 

        public enum simple_action {
            none, 
            
            view_to_left, view_to_right, view_add_copy, view_add_new, view_delete,
            
            button_toggles, button_preferences, button_refresh,

            export_log_and_notes, export_view, export_notes,

            note_create_note, note_show_notes,

            find_find, find_find_next, find_find_prev,

            copy_msg, copy_full_line,            

            // so that the user can edit what he just created
            edit_last_filter,
        }

        public class action {
            public string category = "";
            public string name = "";

            public bool separator = false;

            public delegate void void_func();
            public void_func on_click;

            public bool enabled = true;

            // just in case we have a simple action to be handled by the parent
            public simple_action simple = simple_action.none;
        }
        
        public log_view_right_click(log_view parent) {
            parent_ = parent;
        }


        private void add_filter_color_actions(List<action> actions, string prefix, bool_func color, bool_func match_color, bool_func no_color = null) {
            if ( no_color != null)
                actions.Add(new action { category = "Filter/" + prefix, name = "Default Color", on_click = () => no_color() });

            actions.Add(new action { category = "Filter/" + prefix, name = "Color the Full Line", on_click = () => color() });
            actions.Add(new action { category = "Filter/" + prefix, name = "Match Color (color only what matches)", on_click = () => match_color() });

            if (!parent_.lv_parent.current_ui.show_filter) {
                if ( no_color != null)
                    actions.Add(new action {category = "Filter/" + prefix, name = "Default Color + Take Me to Edit", on_click = () => { 
                        // note: the user can cancel on the color dialog
                        if ( no_color())
                            parent_.lv_parent.simple_action(simple_action.edit_last_filter);
                    } });

                actions.Add(new action {category = "Filter/" + prefix, name = "Color the Full Line + Take Me to Edit", on_click = () => { 
                    // note: the user can cancel on the color dialog
                    if ( color())
                        parent_.lv_parent.simple_action(simple_action.edit_last_filter);
                } });
                actions.Add(new action {category = "Filter/" + prefix, name = "Match Color (color only what matches) + Take Me to Edit", on_click = () => {
                    // note: the user can cancel on the color dialog
                    if (match_color())
                        parent_.lv_parent.simple_action(simple_action.edit_last_filter);
                }});
            }
        }

        // the selection - as to be shown on the menus
        private string small_sel() {
            string sel = parent_.smart_edit_sel_text;
            if (sel == "")
                return "";
            if (sel.Length > 10)
                // don't clutter the menu (that is, don't make it too wide)
                sel = "...";
            else
                sel = "[" + sel + "]";
            return sel;
        }

        private void append_filter_include_actions(List<action> actions) {
            string sel = parent_.smart_edit_sel_text;
            if (sel == "")
                return;
            if (parent_.sel_col_idx != parent_.msgCol.fixed_index())
                // at this time (1.2), we only care about filtering the message column
                return;

            bool does_belong_to_view = parent_.sel.has_matches_via_include(parent_.filter) ;
            bool allow_include = parent_.is_full_log || !parent_.filter.has_include_filters || !does_belong_to_view;
            if (!allow_include)
                return;

            // if this is a view with no filters so far, append ONLY to the "Include" message
            bool is_only = !parent_.is_full_log && parent_.filter.row_count == 0;
            string only = is_only ? "ONLY " : "";

            bool sel_at_start = parent_.sel_subitem_text.StartsWith(sel);
            if ( sel_at_start)
                add_filter_color_actions(actions, "Include " + only + "Lines Starting With " + small_sel(), 
                    () => include_lines(true, true, true), () => include_lines(true, false, true), () => include_lines(true, false, true, true) );
            add_filter_color_actions(actions, "Include " + only + "Lines Containing " + small_sel(), 
                () => include_lines(false, true, true), () => include_lines(false, false, true),  () => include_lines(false, false, true, true) );

            if ( sel_at_start)
                add_filter_color_actions(actions, "Include " + only + "Lines Starting With " + small_sel() + " (case-INsensitive)", 
                    () => include_lines(true, true, false), () => include_lines(true, false, false), () => include_lines(true, false, false, true) );
            add_filter_color_actions(actions, "Include " + only + "Lines Containing " + small_sel() + " (case-INsensitive)", 
                () => include_lines(false, true, false), () => include_lines(false, false, false) , () => include_lines(false, false, false, true));
        }

        private void append_filter_fulllog_actions(List<action> actions) {
            append_filter_include_actions(actions);
        }

        private void append_current_view_filter_actions(List<action> actions) {
            string sel = parent_.smart_edit_sel_text;
            if (sel == "")
                return;
            if (parent_.sel_col_idx != parent_.msgCol.fixed_index())
                // at this time (1.2), we only care about filtering the message column
                return;
            // 1.4.2 - allow excluding lines: this should work at all times - if it's matched by the filter, 
            //                                or i don't have any include filters (thus, by default, all lines are included)
            bool belongs_to_view = parent_.sel.has_matches_via_include(parent_.filter);
            if (!belongs_to_view)
                return;

            bool sel_at_start = parent_.sel_subitem_text.StartsWith(sel);
            if (sel_at_start) {
                actions.Add(new action { category = "Filter", name = "Exclude Lines Starting With " + small_sel(), on_click = () => exclude_lines(true, true) });
                actions.Add(new action { category = "Filter", name = "Exclude Lines Starting With " + small_sel() + " (case-INsensitive)", on_click = () => exclude_lines(true, false) });
            }
            actions.Add(new action { category = "Filter", name = "Exclude Lines Containing " + small_sel(), on_click = () => exclude_lines(false, true) });
            actions.Add(new action { category = "Filter", name = "Exclude Lines Containing " + small_sel() + " (case-INsensitive)", on_click = () => exclude_lines(false, false) });
        }

        private void append_filter_disabled_actions(List<action> actions) {
            Debug.Assert(!parent_.lv_parent.can_edit_context);
            actions.Add(new action { category = "Filter", name = "Can't edit Filter here", enabled = false});
        }

        private void append_filter_goto_actions(List<action> actions) {
            Debug.Assert(!parent_.is_full_log);

            if (parent_.sel.matches.Count == 0)
                // this is a line that does not belong to the current view
                return;

            bool is_single_filter = util.to_list(parent_.sel.match.matches).Count(x => x) == 1;
            if ( is_single_filter)
                actions.Add(new action { category = "Filter/Go to...", name = "Select Filter that matched this Line", on_click = find_filter_matching_line });
            else
                actions.Add(new action { category = "Filter/Go to...", name = "Select All Filters that matched this Line", on_click = find_filter_matching_line });

            if ( parent_.lv_parent.can_edit_context)
                actions.Add(new action { category = "Filter/Go to...", name = "Edit " + (is_single_filter ? "" : "First ") + "Filter that matched this Line", on_click = edit_first_filter_matching_line });

            //   - show what other views contain this line - "Go to Other View Containing this line" -> and show which other views contain it
            var other_views = parent_.lv_parent.other_views_containing_this_line(parent_.sel_row_idx);
            string prefix = "Filter/Go to.../Other View Containing this Line";
            if ( other_views.Count > 0)
                foreach (var other in other_views) {
                    int idx = other.Item2;
                    actions.Add(new action {category = prefix, name = other.Item1, on_click = () => go_to_view(idx)});
                }
        }

        private void append_filter_create_actions(List<action> actions) {
            string sel = parent_.smart_edit_sel_text;
            if (sel == "")
                return;
            if (parent_.sel_col_idx != parent_.msgCol.fixed_index())
                // at this time (1.2), we only care about filtering the message column
                return;

            bool sel_at_start = parent_.sel_subitem_text.StartsWith(sel);
            if ( sel_at_start)
                add_filter_color_actions(actions, "Set Color Of Lines Starting With " + small_sel(), () => set_color(true,true,true), () => set_color(true, false, true) );
            add_filter_color_actions(actions, "Set Color Of Lines Including " + small_sel(), () => set_color(false, true, true), () => set_color(false, false, true) );

            if ( sel_at_start)
                add_filter_color_actions(actions, "Set Color Of Lines Starting With " + small_sel() + " (case-INsensitive)", () => set_color(true,true,false), () => set_color(true, false, false) );
            add_filter_color_actions(actions, "Set Color Of Lines Including " + small_sel() + " (case-INsensitive)", () => set_color(false, true, false), () => set_color(false, false, false) );
        }

        // only when there's a filter that matched this line
        private void append_filter_existing_actions(List<action> actions) {
            string sel = parent_.smart_edit_sel_text;
            if (sel == "")
                return;
            if (parent_.sel_col_idx != parent_.msgCol.fixed_index())
                // at this time (1.2), we only care about filtering the message column
                return;

            bool sel_at_start = parent_.sel_subitem_text.StartsWith(sel);
            if ( sel_at_start)
                add_filter_color_actions(actions, "Change Color Of Lines Starting With " + small_sel(), () => set_color(true,true,true), () => set_color(true, false, true) );
            add_filter_color_actions(actions, "Change Color Of Lines Including " + small_sel(), () => set_color(false, true, true), () => set_color(false, false, true) );

            if ( sel_at_start)
                add_filter_color_actions(actions, "Change Color Of Lines Starting With " + small_sel() + " (case-INsensitive)", () => set_color(true,true,false), () => set_color(true, false, false) );
            add_filter_color_actions(actions, "Change Color Of Lines Including " + small_sel() + " (case-INsensitive)", () => set_color(false, true, false), () => set_color(false, false, false) );
        
        }

        private void append_filter_actions(List<action> actions) {
            if (parent_.lv_parent.can_edit_context) {
                if (parent_.is_full_log)
                    append_filter_fulllog_actions(actions);
                else {
                    append_filter_include_actions(actions);
                    append_current_view_filter_actions(actions);
                }

                if (!parent_.is_full_log) {
                    bool belongs_to_view = parent_.sel.matches.Count > 0;
                    if (belongs_to_view) {
                        var i = parent_.sel;
                        Debug.Assert(i != null);
                        bool is_default = i.match.font.fg == font_info.default_font.fg;
                        if (is_default)
                            append_filter_create_actions(actions);
                        else
                            append_filter_existing_actions(actions);
                    }
                }
            }
            else 
                append_filter_disabled_actions(actions);

            if ( parent_.filter_row_count > 0 && !parent_.is_full_log)
                // here, I know there's at least a filter
                append_filter_goto_actions(actions);
        }

        private void append_notes_actions(List<action> actions) {
            actions.Add(new action { category = "Note", name = "Create Note On This Line", simple = simple_action.note_create_note });
            if ( !parent_.lv_parent.current_ui.show_notes)
                actions.Add(new action { category = "Note", name = "Show notes", simple = simple_action.note_show_notes});
        }

        private void append_copy_actions(List<action> actions) {
            bool multi = parent_.multi_sel_idx.Count > 1;
            actions.Add(new action { category = "Copy", name = "Selected Line" + (multi ? "s" : "") + " To Clipboard (Just Message)", simple = simple_action.copy_msg });
            actions.Add(new action { category = "Copy", name = "Selected Line" + (multi ? "s" : "") + " To Clipboard (Full Line)", simple = simple_action.copy_full_line });
        }

        private void append_find_actions(List<action> actions) {
            actions.Add(new action { category = "Find", name = "Find Text (Ctrl-F)", simple = simple_action.find_find });

            if (!parent_.has_find) {
                actions.Add(new action { category = "Find", name = "Find Next (F3)", simple = simple_action.find_find_next });
                actions.Add(new action { category = "Find", name = "Find Prev (Shift-F3)", simple = simple_action.find_find_prev });                
            }
        }

        private void append_view_disabled_actions(List<action> actions) {
            Debug.Assert(!parent_.lv_parent.can_edit_context);
            actions.Add(new action { category = "View", name = "Can't edit View here", enabled = false});
        }
        private void append_view_actions(List<action> actions) {
            if (parent_.lv_parent.can_edit_context) {
                actions.Add(new action {category = "View", name = "Rename View", on_click = do_rename_view});

                // not implemented yet
                actions.Add(new action {category = "View", name = "Move View to Left", simple = simple_action.view_to_left, enabled = false});
                actions.Add(new action {category = "View", name = "Move View to Right", simple = simple_action.view_to_right, enabled = false});

                actions.Add(new action {category = "View", separator = true});

                actions.Add(new action {category = "View", name = "Add View (Copy of This)", simple = simple_action.view_add_copy});
                actions.Add(new action {category = "View", name = "Add View (From Scratch)", simple = simple_action.view_add_new});
                actions.Add(new action {category = "View", name = "Delete Current View", simple = simple_action.view_delete});
            }
            else 
                append_view_disabled_actions(actions);
        }

        // appends Buttons such as Toggles,Preferences..etc
        private void append_button_actions(List<action> actions) {
            actions.Add(new action { category = "", name = "Edit Toggles", simple = simple_action.button_toggles });

            if (!parent_.lv_parent.current_ui.show_title) {
                actions.Add(new action { category = "", name = "Refresh...", simple = simple_action.button_refresh });
                actions.Add(new action { category = "", name = "Preferences...", simple = simple_action.button_preferences });                
            }
        }

        private void append_export_actions(List<action> actions) {
            if (!parent_.lv_parent.current_ui.show_title) {
                actions.Add(new action {category = "Export", name = "Log + Notes (to .LogWizard file)", simple = simple_action.export_log_and_notes});
                actions.Add(new action {category = "Export", name = "Current View (to .txt and .html files)", simple = simple_action.export_view});
                actions.Add(new action {category = "Export", name = "Notes (to .txt and .html files)", simple = simple_action.export_notes });
            }
        }



        private List<action> available_actions_no_sel() {
            List<action> actions = new List<action>();
            if ( !parent_.is_full_log)
                append_view_actions(actions);
            append_button_actions(actions);
            return actions;
        }

        private List<action> available_actions_multi_sel() {
            List<action> actions = new List<action>();
            append_copy_actions(actions);

            append_button_actions(actions);
            append_export_actions(actions);

            return actions;
        }


        public List<action> available_actions() {
            var multi = parent_.multi_sel_idx;
            if (multi.Count == 0)
                return available_actions_no_sel();
            else if (multi.Count > 1)
                return available_actions_multi_sel();

            List<action> actions = new List<action>();

            append_filter_actions(actions);

            if ( !parent_.is_full_log)
                append_view_actions(actions);

            append_copy_actions(actions);
            append_find_actions(actions);
            append_notes_actions(actions);

            append_export_actions(actions);
            append_button_actions(actions);

            return actions;
        }

        private ToolStripMenuItem create_menu(ContextMenuStrip root, string name, string category, bool separator) {
            var parents = category != "" ? category.Split('/').ToList() : new List<string>();
            if (!separator) {
                if (parents.Count > 0 && parents[0] == "Filter") {
                    if (parents.Count > 1) {
                        parents[0] = "Filter: " + parents[1];
                        parents.RemoveAt(1);
                    } else {
                        name = "Filter: " + name;
                        parents.Clear();
                    }
                } 
                parents.Add(name);
            } else 
                // it's a separator
                Debug.Assert(name == "");

            object cur = root;
            while (parents.Count > 0) {
                string parent_name = parents[0];
                parents.RemoveAt(0);

                ToolStripMenuItem found = null;
                if (cur is ContextMenuStrip) {
                    foreach (var child in root.Items)
                        if ( child is ToolStripMenuItem)
                            if ((child as ToolStripMenuItem).Text == parent_name) {
                                found = child as ToolStripMenuItem;                                
                                break;
                            }
                } else {
                    var strip = cur as ToolStripMenuItem;
                    foreach ( var child in strip.DropDownItems)
                        if ( child is ToolStripMenuItem)
                            if ((child as ToolStripMenuItem).Text == parent_name) {
                                found = child as ToolStripMenuItem;
                                break;
                            }
                }

                if (found == null) {
                    // create one now
                    found = new ToolStripMenuItem(parent_name);
                    if (cur is ContextMenuStrip)
                        root.Items.Add(found);
                    else
                        (cur as ToolStripMenuItem).DropDownItems.Add(found);
                }
                cur = found;
            }

            if (separator) {
                if (cur is ContextMenuStrip)
                    root.Items.Add( new ToolStripSeparator());
                else
                    (cur as ToolStripMenuItem).DropDownItems.Add(new ToolStripSeparator());
                return null;
            }

            return cur as ToolStripMenuItem;
        }

        private void right_click_at_pos(Point point) {
            var actions = available_actions();
            int first_non_filter = actions.FindIndex(x => !x.category.StartsWith( "Filter"));
            int first_non_category = actions.FindIndex(x => x.category == "");
            actions.Insert(first_non_filter, new action { separator = true});
            actions.Insert(first_non_category, new action { separator = true});

            // show the Filter actions first (the rest will be shown in their respective categories)
            ContextMenuStrip menu = new ContextMenuStrip();

            foreach (action a in actions) {
                var item = create_menu(menu, a.name, a.category, a.separator);
                if (!a.separator) {
                    item.Enabled = a.enabled;
                    var to_do = a.simple;
                    var to_act = a.on_click;
                    if ( to_do != simple_action.none)
                        item.Click += (sender, args) => do_simple(to_do);
                    else 
                        item.Click += (sender, args) => to_act();
                }
            }

            menu.Show(parent_, point, util.menu_direction(menu, parent_.PointToScreen(point)) );
        }


        public void right_click() {
            via_apps_hotkey_ = false;
            right_click_at_pos( parent_.PointToClient( Cursor.Position));
        }

        private Point carent_pos {
            get {
                var bounds = parent_.sel_subrect_bounds;
                int left = bounds.Left + parent_.edit.caret_left_pos;
                if (parent_.edit.caret_left_pos > bounds.Width)
                    // in this case, the user scrolled to a very left location (and we have more text that can be shown at a time)
                    left = bounds.Left + 5;
                int top = bounds.Bottom + 5;
                return new Point(left, top);
            }
        }

        public void right_click_at_caret() {
            via_apps_hotkey_ = true;
            right_click_at_pos( carent_pos);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////

        private void do_simple(simple_action simple) {
            logger.Info("[right-click] action " + simple);
            parent_.lv_parent.simple_action(simple);
        }
        
        private void do_add_filter(string filter_str, string filter_description, bool is_color_filter, bool is_include_lines_filter, bool is_exclude_lines_filter) {
            // exclude lines - always apply to existing filters!

            string id = easy_filter_prefix(is_color_filter, is_include_lines_filter, is_exclude_lines_filter);
            filter_str = id + "\r\n## " + filter_description + "\r\n" + filter_str;
            bool apply_to_existing_lines = is_color_filter || is_exclude_lines_filter;
            if (is_include_lines_filter)
                apply_to_existing_lines = false;

            parent_.lv_parent.add_or_edit_filter(filter_str, id, apply_to_existing_lines);
        }

        // this is an easy way for me to later find the filter - in case it needs changing
        // (for instance, when changing the colors of a line - go to edit a filter, instead of creating a new one)
        private string easy_filter_prefix(bool is_color_filter, bool is_include_lines_filter, bool is_exclude_lines_filter) {
            string do_not_edit = "DONOT CHANGE this line -";
            string prefix = raw_filter_row.FILTER_ID_PREFIX + do_not_edit 
                + (is_color_filter ? "color " : "") 
                + (is_include_lines_filter ? "include " : "") 
                + (is_exclude_lines_filter ? "exclude " : "")
                + " #" + parent_.smart_edit_sel_text;
            return prefix;
        }

        private bool set_color(bool starting_with, bool color_full_line, bool case_sensitive) {
            string header = "Select " + (color_full_line ? "Color" : "Match Color") + " for Lines " + (starting_with ? "Starting With " : "Containing ") 
                + parent_.smart_edit_sel_text + " " + (case_sensitive ? "" : "(case-INsensitive)") ;
            var location = parent_.PointToScreen( carent_pos);
            var sel = new select_color_form(header, parent_.sel.fg(parent_), location );
            bool ok = sel.ShowDialog() == DialogResult.OK;
            if (!ok)
                return false;

            bool is_color_filter = true;
            bool is_include_lines = false;
            bool is_exclude_lines = false;
            string filter_str = "$msg " + (starting_with ? "startswith" : "contains") + " " + parent_.smart_edit_sel_text 
                + "\r\n" + (color_full_line ? "color" : "match_color") + " " + util.color_to_str(sel.SelectedColor)
                + (case_sensitive ? "" : "\r\ncase-insensitive");
            string filter_description = (starting_with ? "starts with " : "contains") + " " + parent_.smart_edit_sel_text + " - " + (color_full_line ? "(color)" : "(match)");
            do_add_filter(filter_str, filter_description, is_color_filter, is_include_lines, is_exclude_lines);
            return true;
        }

        // if ignore_color, it's a "Default Color" include filter
        private bool include_lines(bool starting_with, bool color_full_line, bool case_sensitive, bool ignore_color = false) {
            Color col = util.transparent;
            if (!ignore_color) {
                string header = (color_full_line ? "Color" : "Match Color") + " for Lines " + (starting_with ? "Starting With " : "Containing ") 
                    + parent_.smart_edit_sel_text + " " + (case_sensitive ? "" : "(case-INsensitive)") ;
                var location = parent_.PointToScreen( carent_pos);
                var sel = new select_color_form(header, parent_.sel.fg(parent_), location );
                bool ok = sel.ShowDialog() == DialogResult.OK;
                if (!ok)
                    return false;
                col = sel.SelectedColor;
            }

            bool is_color_filter = col != util.transparent;
            bool is_include_lines = true;
            bool is_exclude_lines = false;

            string filter_str = "$msg " + (starting_with ? "startswith" : "contains") + " " + parent_.smart_edit_sel_text 
                + (is_color_filter ? "\r\n" + (color_full_line ? "color" : "match_color") + " " + util.color_to_str(col) : "")
                + (case_sensitive ? "" : "\r\ncase-insensitive");
            string filter_description = "include lines " + (starting_with ? "starting with " : "containing") + " " + parent_.smart_edit_sel_text;

            do_add_filter(filter_str, filter_description, is_color_filter, is_include_lines, is_exclude_lines);
            return true;
        }
        private bool exclude_lines(bool starting_with, bool case_sensitive) {
            bool is_color_filter = false;
            bool is_include_lines = false;
            bool is_exclude_lines = true;

            // exclude lines - always apply to existing filters!
            string filter_str = "$msg " + (starting_with ? "!startswith" : "!contains") + " " + parent_.smart_edit_sel_text 
                + (case_sensitive ? "" : "\r\ncase-insensitive");
            string filter_description = "exclude lines " + (starting_with ? "starting with " : "containing") + " " + parent_.smart_edit_sel_text;
            do_add_filter(filter_str, filter_description, is_color_filter, is_include_lines, is_exclude_lines);
            return true;
        }



        private void find_filter_matching_line() {
            int row_idx = 0;
            List<int> filter_row_indexes = new List<int>();
            foreach (var match in util.to_list(parent_.sel.match.matches)) {
                if ( match)
                    filter_row_indexes.Add(row_idx);
                ++row_idx;
            }
            Debug.Assert(filter_row_indexes.Count > 0);
            
            parent_.lv_parent.select_filter_rows(filter_row_indexes);
        }

        private void edit_first_filter_matching_line() {
            int row_idx = 0;
            List<int> filter_row_indexes = new List<int>();
            foreach (var match in util.to_list(parent_.sel.match.matches)) {
                if ( match)
                    filter_row_indexes.Add(row_idx);
                ++row_idx;
            }
            Debug.Assert(filter_row_indexes.Count > 0);

            parent_.lv_parent.edit_filter_row(filter_row_indexes[0]);
        }

        private void go_to_view(int view_idx) {
            parent_.lv_parent.go_to_view(view_idx);
        }


        private void do_rename_view() {
            parent_.rename_view();
        }



    }
}
