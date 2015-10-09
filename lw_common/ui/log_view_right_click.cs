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

        public enum simple_action {
            none, 
            
            view_to_left, view_to_right, view_add_copy, view_add_new, view_delete,
            
            button_toggles, button_preferences,

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


        private void add_filter_two_actions(List<action> actions, string prefix, action.void_func color, action.void_func match_color) {
            actions.Add(new action { category = "Filter/" + prefix, name = "Color the Full Line", on_click = color });
            actions.Add(new action { category = "Filter/" + prefix, name = "Match Color (color only what matches)", on_click = color });

            if (!parent_.lv_parent.current_ui.show_filter) {
                actions.Add(new action {category = "Filter/" + prefix, name = "Color the Full Line + Take Me to Edit", on_click = () => { 
                    color();
                    parent_.lv_parent.simple_action(simple_action.edit_last_filter);
                } });
                actions.Add(new action {category = "Filter/" + prefix, name = "Match Color (color only what matches) + Take Me to Edit", on_click = () => {
                    match_color();
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

        private void append_filter_fulllog_actions(List<action> actions) {
            string sel = parent_.smart_edit_sel_text;
            if (sel == "")
                return;
            if (parent_.sel_col_idx != parent_.msgCol.Index)
                // at this time (1.2), we only care about filtering the message column
                return;

            bool sel_at_start = parent_.sel_subitem_text.StartsWith(sel);
            if ( sel_at_start)
                add_filter_two_actions(actions, "Include Lines Starting With " + small_sel(), null, null );
            add_filter_two_actions(actions, "Include Lines Containing " + small_sel(), null, null );
        }

        private void append_current_view_filter_actions(List<action> actions) {
            string sel = parent_.smart_edit_sel_text;
            if (sel == "")
                return;
            if (parent_.sel_col_idx != parent_.msgCol.Index)
                // at this time (1.2), we only care about filtering the message column
                return;

            bool sel_at_start = parent_.sel_subitem_text.StartsWith(sel);
            if ( sel_at_start)
                add_filter_two_actions(actions, "Exclude Lines Starting With " + small_sel(), null, null );
            add_filter_two_actions(actions, "Exclude Lines Containing " + small_sel(), null, null );
        }

        private void append_filter_goto_actions(List<action> actions) {
            Debug.Assert(!parent_.is_full_log);

            // see if there's a single filter that matched this or more
            //
            //  - locate the filter(s) that matched this (and open filter view if hidden)
            //   - Show all filters that matched this (a bit harder, since we need to select several rows in filter_ctrl; should check that it does not mess with .SelectedIndex)
            actions.Add(new action { category = "Filter/Go to...", name = "Show Filter that matched this Line", on_click = null });
            actions.Add(new action { category = "Filter/Go to...", name = "Show All Filters that matched this Line", on_click = null });
            actions.Add(new action { category = "Filter/Go to...", name = "Edit First Filter that matched this Line", on_click = null });

            //   - show what other views contain this line - "Go to Other View Containing this line" -> and show which other views contain it
            actions.Add(new action { category = "Filter/Go to...", name = "Other View Containing this Line", on_click = null });
            actions.Add(new action { category = "Filter/Go to.../[menu-for-each-such-view]", name = "[view-name]", on_click = null });
        }

        private void append_filter_create_actions(List<action> actions) {
            string sel = parent_.smart_edit_sel_text;
            if (sel == "")
                return;
            if (parent_.sel_col_idx != parent_.msgCol.Index)
                // at this time (1.2), we only care about filtering the message column
                return;

            bool sel_at_start = parent_.sel_subitem_text.StartsWith(sel);
            if ( sel_at_start)
                add_filter_two_actions(actions, "Set Color Of Lines Starting With " + small_sel(), null, null );
            add_filter_two_actions(actions, "Set Color Of Lines Including " + small_sel(), null, null );
        }

        // only when there's a filter that matched this line
        private void append_filter_existing_actions(List<action> actions) {
            string sel = parent_.smart_edit_sel_text;
            if (sel == "")
                return;
            if (parent_.sel_col_idx != parent_.msgCol.Index)
                // at this time (1.2), we only care about filtering the message column
                return;

            bool sel_at_start = parent_.sel_subitem_text.StartsWith(sel);
            if ( sel_at_start)
                add_filter_two_actions(actions, "Change Color Of Lines Starting With " + small_sel(), null, null );
            add_filter_two_actions(actions, "Change Color Of Lines Including " + small_sel(), null, null );
        }

        private void append_filter_actions(List<action> actions) {
            if (parent_.is_full_log)
                append_filter_fulllog_actions(actions);
            else
                append_current_view_filter_actions(actions);

            if (!parent_.is_full_log) {
                var i = parent_.sel;
                Debug.Assert(i != null);
                bool is_default = i.match.font.fg == filter_line.font_info.default_font.fg;
                if (is_default)
                    append_filter_create_actions(actions);
                else
                    append_filter_existing_actions(actions);
            }

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

        private void append_view_actions(List<action> actions) {
            actions.Add(new action { category = "View", name = "Rename View", on_click = do_rename_view });
            
            // not implemented yet
            actions.Add(new action { category = "View", name = "Move View to Left", simple = simple_action.view_to_left, enabled = false });
            actions.Add(new action { category = "View", name = "Move View to Right", simple = simple_action.view_to_right, enabled = false });

            actions.Add(new action { category = "View", separator = true  });

            actions.Add(new action { category = "View", name = "Add View (Copy of This)", simple = simple_action.view_add_copy });
            actions.Add(new action { category = "View", name = "Add View (From Scratch)", simple = simple_action.view_add_new });
            actions.Add(new action { category = "View", name = "Delete Current View", simple = simple_action.view_delete });
            
        }

        // appends Buttons such as Toggles,Preferences..etc
        private void append_button_actions(List<action> actions) {
            actions.Add(new action { category = "", name = "Update Toggles", simple = simple_action.button_toggles });

            if (!parent_.lv_parent.current_ui.show_title) {
                actions.Add(new action { category = "", name = "Refresh...", on_click = do_refresh });
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
                    Debug.Assert(parents.Count > 1);
                    parents[0] = "Filter: " + parents[1];
                    parents.RemoveAt(1);
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

        public void right_click_at_pos(Point point) {
            var actions = available_actions();
            int first_non_filter = actions.FindIndex(x => !x.category.StartsWith( "Filter"));
            int first_non_category = actions.FindIndex(x => x.category == "");
            actions.Insert(first_non_filter, new action { separator = true});
            actions.Insert(first_non_category, new action { separator = true});

            // show the Filter actions first (the rest will be shown in their respective categories)
            ContextMenuStrip menu = new ContextMenuStrip();

            foreach (action a in actions) {
                var item = create_menu(menu, a.name, a.category, a.separator);
                if ( !a.separator)
                    item.Enabled = a.enabled;
            }

            // check if showing menu would be too big at this position

            menu.Show(parent_, point);
        }


        public void right_click() {
            right_click_at_pos( parent_.PointToClient( Cursor.Position));
        }

        public void right_click_at_caret() {
            var bounds = parent_.sel_subrect_bounds;
            int left = bounds.Left + parent_.edit.caret_left_pos;
            if (parent_.edit.caret_left_pos > bounds.Width)
                // in this case, the user scrolled to a very left location (and we have more text that can be shown at a time)
                left = bounds.Left + 5;
            int top = bounds.Bottom + 5;
            right_click_at_pos( new Point(left, top));
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        private void do_rename_view() {
//  - Rename View -> to rename the view, show source pane, after user presses enter, hide source pane (if it was not visible)
  //  - when editing view name, show "Press Enter to save, Esc to Exit"
            
        }
        private void do_refresh() {
            
        }

        private void do_new_filter(action a) {
            // base the color on what color is on the current line!
        }


    }
}
