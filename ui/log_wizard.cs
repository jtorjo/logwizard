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
 *

 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using BrightIdeasSoftware;
using lw_common;
using lw_common.parse;
using lw_common.ui;
using LogWizard.context;
using LogWizard.Properties;

namespace LogWizard
{
    partial class log_wizard : Form, log_view_parent
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static List<log_wizard> forms_ = new List<log_wizard>();

        private class history {
            public enum entry_type {
                file = 0, shmem = 1
            }

            // 0->file, 1->shmem
            public entry_type type;
            public string name = "";
            public string friendly_name = "";
            //public string log_syntax  = "";

            // returns the file name, or empty if it's not a file
            public string file_name {
                get { return type == entry_type.file ? name : ""; }
            }

            public bool is_file {
                get { return type == entry_type.file; }
            }

            public string ui_friendly_name {
                get {
                    if (friendly_name != "")
                        return friendly_name;

                    switch ( type) {
                        case entry_type.file: 
                            var fi = new FileInfo(name);
                            string ui = fi.Name + " - " + util.friendly_size(fi.Length) + " (" + fi.DirectoryName + ")";
                            return ui;

                        case entry_type.shmem: return "Shared Memory: " + name;
                        default: Debug.Assert(false); break;
                    }

                    return name;
                }
            }

        }
        
        private settings_file sett = app.inst.sett;

        private static List<ui_context> contexts_ = new List<ui_context>();
        private static List<history> history_ = new List<history>();

        private text_reader text_ = null;
        private log_parser log_parser_ = null;
        private readonly log_view full_log_ctrl_ = null;

        private int old_line_count_ = 0;

        const int MAX_HISTORY_ENTRIES = 100;

        private int ignore_change_ = 0;

        private List<int> bookmarks_ = new List<int>();
        private msg_details_ctrl msg_details_ = null;


        // the status(es) to be shown
        private enum status_type {
            msg, warn, err
        }
        private List< Tuple<string, status_type, DateTime>> statuses_ = new List<Tuple<string, status_type, DateTime>>();
        // what to be shown behind ALL statuses
        private string status_prefix_ = "";

        private int toggled_to_custom_ui_ = -1;
        private static ui_info default_ui_ = new ui_info();
        private static ui_info[] custom_ui_ = new ui_info[] {
            new ui_info(), new ui_info(), new ui_info(), new ui_info(), new ui_info(), 
            new ui_info(), new ui_info(), new ui_info(), new ui_info(), new ui_info()
        };

        // if non-empty, we need to merge our notes with other notes
        private string post_merge_file_ = "";

        private Control active_pane_ = null;

        private int selected_history_on_dropdown_ = -1;

        // just in case the user added/edited a filter via right-click filter menu
        private string last_edited_filter_id_ = "";

        private enum show_full_log_type {
            both, just_view, just_full_log
        }

        private bool can_edit_context_ = true;

        public log_wizard()
        {
            InitializeComponent();
            toggled_to_custom_ui_ = first_available_toggle_custom_ui();
            forms_.Add(this);
            Text += " " + version();
            sourceTypeCtrl.SelectedIndex = 0;
            bool first_time = contexts_.Count == 0;
            if (first_time) {
                load_contexts(sett);
                notes_keeper.inst.init( util.is_debug ? "notes" : Program.local_dir() + "\\notes", app.inst.identify_notes_files);
            }
            notes.set_author( app.inst.notes_author_name, app.inst.notes_initials, app.inst.notes_color);
            notes.on_note_selected = on_note_selected;

            ++ignore_change_;

            full_log_ctrl_ = new log_view( this, log_view.FULLLOG_NAME);
            full_log_ctrl_.Dock = DockStyle.Fill;
            filteredLeft.Panel2.Controls.Add(full_log_ctrl_);
            full_log_ctrl_.show_name = false;
            full_log_ctrl_.show_view(true);

            filtCtrl.design_mode = false;
            filtCtrl.on_save = save;
            filtCtrl.ui_to_view = (view_idx) => log_view_for_tab(view_idx).set_filter(filtCtrl.to_filter_row_list());
            filtCtrl.on_rerun_view = (view_idx) => refreshFilter_Click(null, null);
            filtCtrl.on_refresh_view = (view_idx) => {
                log_view_for_tab(view_idx).Refresh();
                full_log.list.Refresh();
            };
            filtCtrl.mark_match = (filter_idx) => {
                var lv = log_view_for_tab(viewsTab.SelectedIndex);
                Color fg = util.str_to_color(sett.get("filter_fg", "transparent"));
                Color bg = util.str_to_color(sett.get("filter_bg", "#faebd7"));
                lv.mark_match(filter_idx, fg, bg);
            };

            recreate_contexts_combo();

            foreach ( history hist in history_)
                logHistory.Items.Add(hist.ui_friendly_name);

            --ignore_change_;
            load();
            ++ignore_change_;

            util.postpone(() => set_tabs_visible(leftPane, false), 1);

            msg_details_ = new msg_details_ctrl(this);
            Controls.Add(msg_details_);
            handle_subcontrol_keys(this);

            viewsTab.DrawMode = TabDrawMode.OwnerDrawFixed;
            viewsTab.DrawItem += ViewsTabOnDrawItem;

            update_topmost_image();
            update_toggle_topmost_visibility();
            --ignore_change_;

            update_status_prefix();

            // 1.0.80d+ - the reason we postpone this is so taht we don't set up all UI in the constructor - the splitters would get extra SplitterMove() events,
            //            and we would end up positioning them wrong
            util.postpone(() => {
                bool open_cmd_line_file = forms_.Count == 1 && Program.open_file_name != null;
                if (history_.Count > 0 && !open_cmd_line_file)
                    logHistory.SelectedIndex = history_.Count - 1;
                if (open_cmd_line_file)
                    on_file_drop(Program.open_file_name);
            }, 10);
        }

        // note: we don't allow two forms to use the same UI - that would be insane - if one would toggle somethin in Form A (or just move it), 
        //       we would need to synchronize Form B or just ignore everything in Form B - and then how would we decide where we save settings from?
        private int first_available_toggle_custom_ui() {
            if (forms.Count < 1)
                return -1;

            List<int> used = forms.Select(x => x.toggled_to_custom_ui_).ToList();

            for ( int idx = 0; idx < custom_ui_.Length; ++idx)
                if (!used.Contains(idx)) {
                    if (!custom_ui_[idx].was_set_at_least_once) {
                        // use the last form's UI
                        int last_ui = forms.Last().toggled_to_custom_ui_;
                        custom_ui_[idx].copy_from( last_ui >= 0 ? custom_ui_[last_ui] : default_ui_);
                        // just a bit lower than the last UI - so that both are visible
                        custom_ui_[idx].left += 50;
                        custom_ui_[idx].top += 50;
                        save();
                    }
                    return idx;
                }

            Debug.Assert(false);
            return -1;
        }

        private void recreate_contexts_combo() {
            ++ignore_change_;
            
            int old_sel = curContextCtrl.SelectedIndex;
            string old_name = old_sel >= 0 ? curContextCtrl.Items[old_sel].ToString() : "";
            curContextCtrl.Items.Clear();
            foreach ( ui_context ctx in contexts_)
                curContextCtrl.Items.Add(ctx.name);

            if (old_sel < 0)
                // just select something
                curContextCtrl.SelectedIndex = 0;
            else {
                bool found = false;
                for (int idx = 0; idx < contexts_.Count && !found; idx++) {
                    ui_context ctx = contexts_[idx];
                    if (ctx.name == old_name) {
                        curContextCtrl.SelectedIndex = idx;
                        found = true;
                    }
                }
                if ( !found)
                    // probably the former selection was erased - just select something
                    curContextCtrl.SelectedIndex = 0;
            }

            --ignore_change_;
        }

        public void after_column_positions_modified(log_view lv) {
            string positions = lv.column_positions;
            if (positions == "")
                // user changed nothing
                return;

            if ( !lv.apply_column_settings_only_to_me)
                foreach ( var other in all_log_views())
                    if ( other != lv && !other.apply_column_settings_only_to_me)
                        other.column_positions = positions ;

            // now, save them
            if (lv.apply_column_settings_only_to_me) {
                var old = global_ui.view(lv.name);
                global_ui.view( lv.name, new ui_info.view_info(lv.column_positions, old.show_full_log));
            } else
                global_ui.global_column_positions = lv.column_positions;

            save();
        }

        private void update_contexts_combos_in_all_forms() {
            foreach(var f in forms)
                f.recreate_contexts_combo();
        }

        /* IMPORTANT: 
                the idea here is not to allow the user to edit the same context from two different LogWizard forms
                In other words, he could be in form A, add a new view, and in form B, modify something else (and so on).

                While this theoretically could be possible, it's rather complicated to implement (namely, synchronizing all forms having the same context)
                For now, avoid this altogether: only the first form can edit the context. The other forms with the same context will have everything disabled.

                For two different forms each having different contexts, we don't care
        */
        private void on_context_changed() {
            List<string> used_contexts = new List<string>();
            foreach (var f in forms) {
                string cur_context = f.cur_context().name;
                f.can_edit_context_ = !used_contexts.Contains(cur_context);
                used_contexts.Add(cur_context);
            }

            foreach (var f in forms) {
                f.sourceUp.Panel1.Enabled = f.can_edit_context_;
                f.filtCtrl.Enabled = f.can_edit_context_;
                f.newFilteredView.Enabled = f.can_edit_context_;
                f.delFilteredView.Enabled = f.can_edit_context_;
            }
        }


        private void on_note_selected(int line_idx, string view_name) {
            if (line_idx >= 0) {
                ++ignore_change_;

                // select the view that has this note
                log_view lv = log_view_by_name(view_name);
                if (lv != null) {
                    int lv_idx = all_log_views().FindIndex(x => x.name == view_name);
                    Debug.Assert(lv_idx >= 0);
                    viewsTab.SelectedIndex = lv_idx;
                    lv.go_to_closest_line(line_idx, log_view.select_type.do_not_notify_parent);
                }
                else if (full_log_ctrl_ != null) {
                    // in this case, we don't have that view - go to the full log
                    if ( !global_ui.show_fulllog)
                        show_full_log(show_full_log_type.both);
                    full_log_ctrl_.go_to_closest_line(line_idx, log_view.select_type.do_not_notify_parent);
                } 

                --ignore_change_;
            }
        }

        private log_view log_view_by_name(string view_name) {
            foreach ( log_view lv in all_log_views())
                if (lv.name == view_name)
                    return lv;
            return null;
        }

        private Brush views_brush_ = new SolidBrush(Color.Black), views_something_changed_brush_ = new SolidBrush(Color.DarkRed);
        private void ViewsTabOnDrawItem(object sender, DrawItemEventArgs e) {            
            Graphics g = e.Graphics;

            // Get the item from the collection.
            TabPage tab = viewsTab.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle bounds = viewsTab.GetTabRect(e.Index);

            if (e.State == DrawItemState.Selected)
                // Draw a different background color, and don't paint a focus rectangle.
                g.FillRectangle(Brushes.LightGray, e.Bounds);

            var lv = log_view_for_tab(e.Index);
            Font font = lv != null ? lv.title_font : viewsTab.Font;
            Brush brush = lv != null && lv.has_anything_changed ? views_something_changed_brush_ : views_brush_;

            // Draw string. Center the text.
            StringFormat _StringFlags = new StringFormat();
            _StringFlags.Alignment = StringAlignment.Center;
            _StringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(viewsTab.TabPages[e.Index].Text, font, brush, bounds, new StringFormat(_StringFlags));
        }

        public bool can_edit_context {
            get { return can_edit_context_; }
        }

        private ui_info global_ui {
            get {
                Debug.Assert( toggled_to_custom_ui_ >= -1 && toggled_to_custom_ui_ < custom_ui_.Length);
                return toggled_to_custom_ui_ < 0 ? default_ui_ : custom_ui_[toggled_to_custom_ui_];
            }
        }

        // returns true if the tabs are visible
        private bool are_tabs_visible(TabControl tab) {
            return tab.Top >= 0;
        }
        private void set_tabs_visible(TabControl tab, bool show) {
            int page_height = tab.SelectedTab != null ? tab.SelectedTab.Height : tab.TabPages[0].Height;
            int extra = tab.Height - page_height;
            bool visible_now = are_tabs_visible(tab);
            if (show) {
                if (!visible_now) {
                    tab.Top += extra;
                    tab.Height -= extra;                    
                }
            } else {
                if (visible_now) {
                    tab.Top -= extra;
                    tab.Height += extra;
                }
            }
        }

        private ui_context cur_context() { 
            return contexts_[ curContextCtrl.SelectedIndex];
        }

        public static string version() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version_ = fileVersionInfo.ProductVersion;
            return version_;
        }

        private static void load_contexts(settings_file sett) {
            logger.Debug("loading contexts");

            int history_count = int.Parse( sett.get("history_count", "0"));
            for (int idx = 0; idx < history_count; ++idx) {
                history hist = new history();
                hist.type = (history.entry_type) int.Parse( sett.get("history." + idx + "type", "0"));
                hist.name = sett.get("history." + idx + "name");
                hist.friendly_name = sett.get("history." + idx + "friendly_name");
                history_.Add( hist );
            }
            history_ = history_.Where(h => {
                if (h.type == history.entry_type.file)
                    if( File.Exists(h.name))
                        // 1.1.5+ - compute md5s for this
                        md5_log_keeper.inst.compute_default_md5s_for_file(h.name);
                    else
                        return false;
                return true;
            }).ToList();


            int count = int.Parse( sett.get("context_count", "1"));
            for ( int i = 0; i < count ; ++i) {
                ui_context ctx = new ui_context();
                ctx.load("context." + i);
                contexts_.Add(ctx);
            }
            // 1.1.25 - at application start - remove empty contexts (like, the user may have dragged a file, not what he wanted, dragged another)
            contexts_ = contexts_.Where(x => x.has_not_empty_views || x.name == "Default").ToList();
        }

        private void save_contexts() {
            sett.set( "history_count", "" + history_.Count);
            for ( int idx = 0; idx < history_.Count; ++idx) {
                sett.set("history." + idx + "type", "" + (int)history_[idx].type);
                sett.set("history." + idx + "name", history_[idx].name);
                sett.set("history." + idx + "friendly_name", history_[idx].friendly_name);
            }

            sett.set("context_count", "" + contexts_.Count);
            for ( int i = 0; i < contexts_.Count; ++i) {
                contexts_[i].save("context." + i);

                /*
                int view_count = contexts_[i].views.Count;
                if ( view_count == 1)
                    if (contexts_[i].views[0].filters.Count < 1)
                        // in this case, the user has not set any filters at all
                        view_count = 0;
                sett.set("context." + i + ".view_count", "" + view_count);
                for ( int v = 0; v < contexts_[i].views.Count; ++v) {
                    ui_view lv = contexts_[i].views[v];
                    sett.set("context." + i + ".view" + v + ".name", lv.name);
                    sett.set("context." + i  + ".view" + v + ".filter_count", "" + lv.filters.Count);
                    for ( int f = 0; f < lv.filters.Count; ++f) {
                        string prefix = "context." + i + ".view" + v + ".filt" + f + ".";
                        sett.set(prefix + "enabled", lv.filters[f].enabled ? "1" : "0");
                        sett.set(prefix + "dimmed", lv.filters[f].dimmed ? "1" : "0");
                        sett.set(prefix + "apply_to_existing_lines", lv.filters[f].apply_to_existing_lines ? "1" : "0");
                        sett.set(prefix + "text", lv.filters[f].text);
                    }
                }
                */
            }
        }

        private void show_left_pane(bool show) {
            update_left_pane();

            bool shown = !main.Panel1Collapsed;
            if (show == shown)
                return;

            ++ignore_change_;
            if ( show) {
                main.Panel1Collapsed = false;
                main.Panel1.Show();
            }
            else {
                main.Panel1Collapsed = true;
                main.Panel1.Hide();
            }
            --ignore_change_;
        }
        private void show_source(bool show) {
            logger.Debug("showing source pane " + show);
            bool shown = !sourceUp.Panel1Collapsed;
            if (show == shown)
                return;

            if ( show) {
                sourceUp.Panel1Collapsed = false;
                sourceUp.Panel1.Show();
            }
            else {
                sourceUp.Panel1Collapsed = true;
                sourceUp.Panel1.Hide();
            }
        }

        private void show_filteredleft_pane1(bool show) {
            if (!show == filteredLeft.Panel1Collapsed)
                return;

            if (show) {
                filteredLeft.Panel1Collapsed = false;
                filteredLeft.Panel1.Show();
            } else {
                filteredLeft.Panel1Collapsed = true;
                filteredLeft.Panel1.Hide();
            }
        }
        private void show_filteredleft_pane2(bool show) {
            if (!show == filteredLeft.Panel2Collapsed)
                return;

            if (show) {
                filteredLeft.Panel2Collapsed = false;
                filteredLeft.Panel2.Show();
            } else {
                filteredLeft.Panel2Collapsed = true;
                filteredLeft.Panel2.Hide();
            }
        }

        private void show_full_log(show_full_log_type show) {
            Control to_focus = null;
            switch (show) {
            case show_full_log_type.both:
                show_filteredleft_pane1(true);
                show_filteredleft_pane2(true);
                global_ui.show_fulllog = true;
                global_ui.show_current_view = true;
                to_focus = log_view_for_tab(viewsTab.SelectedIndex);
                break;
            case show_full_log_type.just_view:
                show_filteredleft_pane1(true);
                show_filteredleft_pane2(false);
                global_ui.show_fulllog = false;
                global_ui.show_current_view = true;
                to_focus = log_view_for_tab(viewsTab.SelectedIndex);
                break;
            case show_full_log_type.just_full_log:
                show_filteredleft_pane1(false);
                show_filteredleft_pane2(true);
                global_ui.show_fulllog = true;
                global_ui.show_current_view = false;
                to_focus = full_log_ctrl_;
                break;
            default:
                Debug.Assert(false);
                break;
            }

            if (!global_ui.show_current_view)
                // 1.4.9 - so that on activate, we focus correctly
                active_pane_ = full_log_ctrl_;

            // can't focus now, it would sometimes get the hotkey ('L') to be sent twice, and we'd end up toggling twice with a single press
            if ( to_focus != null)
                util.postpone(() => {
                    var lv = to_focus as log_view;
                    if ( lv != null)
                        lv.set_focus();
                    else
                        to_focus.Focus();
                }, 100);
        }

        show_full_log_type shown_full_log_now() {
            if (global_ui.show_current_view && global_ui.show_fulllog)
                return show_full_log_type.both;
            return global_ui.show_current_view ? show_full_log_type.just_view : show_full_log_type.just_full_log;
        }
        
        private void toggle_full_log() {
            show_full_log_type now = shown_full_log_now();
            show_full_log_type next = now;
            switch (now) {
            case show_full_log_type.both: next = show_full_log_type.just_full_log; 
                break;
            case show_full_log_type.just_view: next = show_full_log_type.both;
                break;
            case show_full_log_type.just_full_log: next = show_full_log_type.just_view;
                break;
            default:
                Debug.Assert(false);
                break;
            }
            show_full_log(next);

            save();
            update_msg_details(true);
        }

        private void filteredViews_DragEnter(object sender, DragEventArgs e)
        {
            if ( e.Data.GetDataPresent( DataFormats.FileDrop))
                e.Effect = e.AllowedEffect;
            else
                e.Effect = DragDropEffects.None;
        }

        private void filteredViews_DragDrop(object sender, DragEventArgs e)
        {
            if ( e.Data.GetDataPresent( DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if ( files.Length == 1)
                    on_file_drop(files[0]);
            }
        }

        private ui_context file_to_context(string name) {
            var default_ = contexts_.FirstOrDefault(x => x.name == "Default");
            if (!File.Exists(name))
                return default_;

            if ( app.inst.forced_file_to_context.ContainsKey(name)) {
                string forced = app.inst.forced_file_to_context[name];
                var context_from_forced = contexts_.FirstOrDefault(x => x.name == forced);
                if (context_from_forced != null)
                    // return it, only if we have a specific Template for it
                    return context_from_forced;
            }

            string from_header = log_to.file_to_context(name);
            if (from_header != null) {
                // ... case-insensitive search (easier for user)
                var context_from_header = contexts_.FirstOrDefault(x => x.name.ToLower() == from_header.ToLower());
                if (context_from_header != null)
                    // return it, only if we have a specific Template for it
                    return context_from_header;
            }

            // 1.1.25+ - match the context that matches the name completely
            string name_no_ext = Path.GetFileNameWithoutExtension( new FileInfo(name).Name);
            var found = contexts_.FirstOrDefault(x => x.name == name_no_ext);
            if (found != null)
                return found;

            return default_ ?? contexts_[0];
        }

        private void on_file_drop(string file, string friendly_name = "") {
            if (file.ToLower().EndsWith(".zip")) {
                // allow the drag/drop operation to finish - thus the program we got this from can be responsive
                util.postpone(() => on_zip_drop(file), 1);
                return;
            }
            else if (file.ToLower().EndsWith(".logwizard")) {
                // allow the drag/drop operation to finish - thus the program we got this from can be responsive
                util.postpone(() => import_notes(file), 1);
                return;
            }

            ++ignore_change_;
            sourceNameCtrl.Text = file;
            sourceTypeCtrl.SelectedIndex = 0;
            friendlyNameCtrl.Text = "";
            --ignore_change_;

            history_select(file, friendly_name);
            on_new_file_log(file);
            util.bring_to_top(this);
        }

        // to show/hide the "details" view - the details view contains all information in the message (that is not usually shown in the list)
        private void show_details(bool show) {
            // not implmented yet
        }

        private void toggle_details() {
            bool show = !global_ui.show_details;
            show_details( show);
            global_ui.show_details = global_ui.show_details = show;
            save();
        }

        private int status_height_ = 0;
        private void show_status(bool show) {
            bool shown = status.Height > 1;
            if (show == shown)
                return;

            if (show) {
                Debug.Assert(status_height_ > 0);
                int height = status_height_ - 1;
                main.Height -= height;
                lower.Top -= height;
                status.Top -= height;
                status.Height = status_height_;
            } else {
                status_height_ = status.Height;
                status.Height = 1;
                int height = status_height_ - 1;
                main.Height += height;
                lower.Top += height;
                status.Top += height;
            }
        }

        private void toggle_status() {
            bool shown = status.Height > 1;
            show_status(!shown);
            global_ui.show_status = global_ui.show_status = !shown;
        }


        private int extra_width_ = 0;
        private void show_title(bool show) {
            bool shown = FormBorderStyle == FormBorderStyle.Sizable;
            if (show == shown)
                return; // nothing to do

            if (!show) {
                extra_width_ = Width - RectangleToScreen(ClientRectangle).Width;
                FormBorderStyle = FormBorderStyle.None;
                main.Height += lower.Height;
                Height += lower.Height;
                Width += extra_width_;
            } else {
                main.Height -= lower.Height;
                Height -= lower.Height;
                Width -= extra_width_;
                FormBorderStyle = FormBorderStyle.Sizable;
            }

            update_toggle_topmost_visibility();
        }

        private void toggle_title() {
            bool shown = FormBorderStyle == FormBorderStyle.Sizable;
            show_title(!shown);
            global_ui.show_title = global_ui.show_title = !shown;
            save();
        }

        private void show_tabs(bool show) {
            bool visible_now = show;
            set_tabs_visible(viewsTab, visible_now);
            // show the tab name in the "Message" column itself
            foreach (var lv in all_log_views())
                lv.show_name_in_header = !visible_now;
        }

        private void toggle_view_tabs() {
            bool visible_now = !are_tabs_visible(viewsTab);
            show_tabs(visible_now);
            global_ui.show_tabs = visible_now;
            save();
        }

        private void show_header(bool show) {
            bool shown = log_view_for_tab(0).show_header;
            foreach (var lv in all_log_views_and_full_log())
                lv.show_header = show;

            if (shown != show) {
                int header_height = log_view_for_tab(0).header_height;
                newFilteredView.Top += show ? header_height : -header_height;
                delFilteredView.Top += show ? header_height : -header_height;
                synchronizeWithExistingLogs.Top  += show ? header_height : -header_height;
                synchronizedWithFullLog.Top  += show ? header_height : -header_height;
            }
        }

        private void toggle_view_header() {
            bool show = !all_log_views()[0].show_header;
            show_header( show);
            global_ui.show_header = show;
            save();
        }

        internal void update_toggle_topmost_visibility() {
            bool show_toggle_topmost = (FormBorderStyle == FormBorderStyle.None) || app.inst.show_topmost_toggle || TopMost;
            toggleTopmost.Visible = show_toggle_topmost;
            var first_tab = viewsTab.TabCount >= 0 ? log_view_for_tab(0) : null;
            if (first_tab != null)
                first_tab.pad_name_on_left = show_toggle_topmost;            
            update_msg_details(true);
        }

        private void update_topmost_image() {
            toggleTopmost.Image = TopMost ? Resources.bug : Resources.bug_disabled;
        }

        private void toggle_notes() {
            global_ui.show_notes = !global_ui.show_notes;            
            show_left_pane( global_ui.show_left_pane);

            save();
            update_msg_details(true);            
        }

        private void toggle_filters() {
            global_ui.show_filter = !global_ui.show_filter;            
            show_left_pane( global_ui.show_left_pane);

            save();
            update_msg_details(true);
        }
        private void toggle_source() {
            global_ui.show_source = !global_ui.show_source;
            show_source( global_ui.show_source);
            save();
            update_msg_details(true);
        }

        private void toggleFullLog_Click(object sender, EventArgs e)
        {
            toggle_full_log();
        }

        private void newView_Click(object sender, EventArgs e)
        {
            new log_wizard( ).Show();
        }

        private void LogNinja_FormClosed(object sender, FormClosedEventArgs e)
        {
            forms_.Remove(this);
            if ( forms_.Count == 0)
                Application.Exit();
        }

        private ui_view cur_view() {
            ui_context cur = cur_context();
            int cur_view = viewsTab.SelectedIndex;
            Debug.Assert(cur_view < cur.views.Count);
            return cur.views[cur_view];

        }
        private void load_filters() {
            // filter_row Enabled / Used - are context dependent!

            ui_context cur = cur_context();
            int cur_view = viewsTab.SelectedIndex;
            if (cur_view < cur.views.Count) 
                filtCtrl.view_to_ui( cur.views[cur_view], cur_view);
        }

        private log_view log_view_for_tab(int idx) {
            TabPage tab = viewsTab.TabPages[idx];
            foreach ( Control c in tab.Controls)
                if ( c is log_view)
                    return (log_view)c; // we have it
            return null;
        }

        private log_view ensure_we_have_log_view_for_tab(int idx) {
            TabPage tab = viewsTab.TabPages[idx];
            foreach ( Control c in tab.Controls)
                if ( c is log_view)
                    return c as log_view; // we have it

            foreach ( Control c in tab.Controls)
                c.Visible = false;

            Debug.Assert( idx < cur_context().views.Count );
            string name = cur_context().views[idx].name;
            log_view new_ = new log_view( this, name );
            new_.Dock = DockStyle.Fill;
            tab.Controls.Add(new_);
            new_.show_name = false;
            new_.set_bookmarks(bookmarks_.ToList());
            if ( log_parser_ != null)
                new_.set_log( new log_reader(log_parser_));
            return new_;
        }

        private List<log_view> all_log_views() {
            List<log_view> other_logs = new List<log_view>();
            for (int idx = 0; idx < viewsTab.TabCount; ++idx) {
                var other = log_view_for_tab(idx);
                other_logs.Add(other);
            }
            return other_logs;
        }

        private List<log_view> all_log_views_and_full_log() {
            var all = all_log_views();
            if ( full_log_ctrl_ != null)
                all.Add(full_log_ctrl_);
            return all;
        }

        // this is called when the user has manually changed the view name
        public void on_view_name_changed(log_view view, string name) {
            if (ignore_change_ > 0)
                return;
            if (view == full_log)
                return;

            ui_context cur = cur_context();
            int view_idx = all_log_views().IndexOf(view);
            ui_view cur_view = cur.views[view_idx];
            string old_name = cur_view.name;
            if (name == old_name)
                // nothing changed
                return;

            var other_views = all_log_views();
            other_views.Remove(view);
            var other_view_names = other_views.Select(x => x.name);
            string new_name = util.unique_name(other_view_names, name);
            if (new_name != name) 
                // in this case, another view already had that name - we want names to be unique
                view.name = name = new_name;

            cur_view.name = name;
            global_ui.rename_view(old_name, name);
        }

        public Rectangle client_rect_no_filter {
            get {
                Rectangle r = ClientRectangle;
                Rectangle source_screen = sourceUp.RectangleToScreen(sourceUp.ClientRectangle);
                Rectangle main_rect = RectangleToClient(source_screen);
                int offset_x = main_rect.Left;
                r.X += offset_x;
                r.Width -= offset_x;
                return r;
            }
        }

        // returns current settings (read-only)
        public ui_info current_ui {
            get {
                ui_info ui = new ui_info();
                ui.copy_from(global_ui);
                return ui;
            }
        }

        public log_view full_log {
            get { return full_log_ctrl_; }
        }

        public int selected_filter_row_index {
            get {
                if (filtCtrl.is_editing_any_filter || filtCtrl.is_focus_on_filter_list)
                    return filtCtrl.sel;
                return -1;                
            }
        }

        public void simple_action(log_view_right_click.simple_action simple) {
            // - Adding a filter/changing a filter + Edit -> need to figure out if it's existing or not, to know what filter to select for later editing!
            switch (simple) {
            case log_view_right_click.simple_action.none:
                Debug.Assert(false);
                break;
            case log_view_right_click.simple_action.view_to_left:
                Debug.Assert(false);
                break;
            case log_view_right_click.simple_action.view_to_right:
                Debug.Assert(false);
                break;

            case log_view_right_click.simple_action.view_add_copy:
                createACopyOfTheExistingViewToolStripMenuItem_Click(null,null);
                break;
            case log_view_right_click.simple_action.view_add_new:
                createANewViewFromScratchToolStripMenuItem_Click(null,null);
                break;
            case log_view_right_click.simple_action.view_delete:
                delView_Click(null,null);
                break;

            case log_view_right_click.simple_action.button_toggles:
                toggles_Click(null,null);
                break;
            case log_view_right_click.simple_action.button_preferences:
                settingsCtrl_Click(null,null);
                break;
            case log_view_right_click.simple_action.button_refresh:
                refreshFilter_Click(null,null);
                break;

            case log_view_right_click.simple_action.export_log_and_notes:
                export_notes_to_logwizard_file();
                break;
            case log_view_right_click.simple_action.export_view:
                exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem_Click(null,null);
                break;
            case log_view_right_click.simple_action.export_notes:
                exportNotestotxtAndhtmlFilesToolStripMenuItem_Click(null,null);
                break;

            case log_view_right_click.simple_action.find_find:
                handle_action(action_type.search);
                break;
            case log_view_right_click.simple_action.find_find_next:
                handle_action(action_type.search_next);
                break;
            case log_view_right_click.simple_action.find_find_prev:
                handle_action(action_type.search_prev);
                break;

            case log_view_right_click.simple_action.copy_msg:
                handle_action(action_type.copy_to_clipboard);
                break;
            case log_view_right_click.simple_action.copy_full_line:
                handle_action(action_type.copy_full_line_to_clipboard);
                break;

            case log_view_right_click.simple_action.note_create_note:
                if (!global_ui.show_notes) {
                    global_ui.show_notes = true;
                    show_left_pane(global_ui.show_left_pane);
                }
                notes.focus_to_edit();
                break;
            case log_view_right_click.simple_action.note_show_notes:
                handle_action(action_type.toggle_notes);
                break;

            case log_view_right_click.simple_action.edit_last_filter:
                if (!global_ui.show_filter) {
                    global_ui.show_filter = true;
                    show_left_pane(global_ui.show_left_pane);
                }
                filtCtrl.edit_filter_row_by_filter_id(last_edited_filter_id_);
                break;

            default:
                Debug.Assert(false);
                break;
            }
        }

        public void add_or_edit_filter(string filter_str, string filter_id, bool apply_to_existing_lines) {
            filtCtrl.update_filter_row(filter_id, filter_str, apply_to_existing_lines);
            last_edited_filter_id_ = filter_id;
        }

        public void sel_changed(log_view_sel_change_type type) {
            on_log_changed_line();
        }

        public void select_filter_rows(List<int> filter_row_indexes) {
            if (!global_ui.show_filter) {
                global_ui.show_filter = true;
                show_left_pane(global_ui.show_left_pane);
            }
            filtCtrl.select_filter_rows(filter_row_indexes);
        }

        public void edit_filter_row(int filter_row_idx) {
            if (!global_ui.show_filter) {
                global_ui.show_filter = true;
                show_left_pane(global_ui.show_left_pane);
            }
            filtCtrl.edit_filter_row(filter_row_idx);
        }

        public List<Tuple<string, int>> other_views_containing_this_line(int row_idx) {            
            Debug.Assert( !is_focus_on_full_log());
            List<Tuple<string,int>> other = new List<Tuple<string, int>>();
            if (!is_focus_on_full_log()) {
                var lv = selected_view();
                // at this time - only for the selection
                Debug.Assert(lv.sel_row_idx == row_idx);
                int line_idx = lv.sel_line_idx;
                int view_idx = 0;
                foreach (var other_view in all_log_views()) {
                    if ( other_view != lv)
                        if ( other_view.contains_line(line_idx))
                            other.Add( new Tuple<string, int>(other_view.name, view_idx));
                    ++view_idx;
                }
            }
            return other;
        }

        public void go_to_view(int view_idx) {
            Debug.Assert(view_idx >= 0 && view_idx < viewsTab.TabCount);
            int line_idx = selected_view().sel_line_idx;
            viewsTab.SelectedIndex = view_idx;
            // wait for UI to completely go to the other tab
            util.postpone(() => {
                selected_view().go_to_closest_line(line_idx, log_view.select_type.do_not_notify_parent);
            }, 100);
        }

        public Tuple<Color, Color> full_log_row_colors(int line_idx) {
            int sel = viewsTab.SelectedIndex;
            int row_idx = full_log.line_to_row(line_idx);
            if (row_idx < 0)
                // this can happen when on a line not from our curent filter
                return new Tuple<Color, Color>( font_info.full_log_gray.fg, font_info.full_log_gray.bg );
            return full_log. update_colors_for_line(row_idx, all_log_views(), sel);
        }

        public void after_set_filter_update() {
            var lv = log_view_for_tab(viewsTab.SelectedIndex);
            string filter = lv.filter_view ? "Extra Filter: " + lv.filter_friendly_name : "";
            string fulllog = lv.show_full_log ? "All Lines" : "View";

            string status = lv.filter_view ? filter + " Applied On " + fulllog : "Showing " + fulllog;
            set_status(status);
        }


        public static List<log_wizard> forms {
            get { return forms_; }
        }

        private void load_tabs() {
            // note: we only add the inner view when there's some source to read from
            ui_context cur = cur_context();

            // never allow "no view" whatsoever
            bool has_views = cur.views.Count > 0 || cur.name != "Default";
            if (cur.views.Count < 1)
                cur.views.Add(new ui_view() {name = "View_1", is_default_name = true});

            for (int idx = 0; idx < cur.views.Count; ++idx)
                if (viewsTab.TabCount < idx + 1)
                    viewsTab.TabPages.Add(cur.views[idx].name);

            for (int idx = 0; idx < cur.views.Count; ++idx) {
                viewsTab.TabPages[idx].Text = cur.views[idx].name;
                ensure_we_have_log_view_for_tab(idx);
            }

            while (viewsTab.TabCount > cur.views.Count) 
                remove_log_view_tab(cur.views.Count);

            if (!has_views) {
                log_view_for_tab(0).Visible = false;
                dropHere.Visible = true;
            }
        }

        private void update_left_pane() {
            if (!global_ui.show_left_pane)
                return;

            if (global_ui.show_filter)
                leftPane.SelectedIndex = 0;
            else if (global_ui.show_notes)
                leftPane.SelectedIndex = 2;
            else {
                Debug.Assert(false);
                global_ui.show_filter = true;
                leftPane.SelectedIndex = 0;
            }
        }

        private void load_ui() {
            var cur = cur_context();
            if (!cur.has_views)
                return;

            util.suspend_layout(this, true);

            ++ignore_change_;
            show_left_pane(global_ui.show_left_pane);
            show_source(global_ui.show_source);

            if (global_ui.show_fulllog && global_ui.show_current_view)
                show_full_log(show_full_log_type.both);
            else
                show_full_log(global_ui.show_current_view ? show_full_log_type.just_view : show_full_log_type.just_full_log);

            show_header(global_ui.show_header);
            show_title(global_ui.show_title);
            show_details(global_ui.show_details);
            show_status(global_ui.show_status);

            if (global_ui.width > 0) {
                Left = global_ui.left;
                Top = global_ui.top;
                Width = global_ui.width;
                Height = global_ui.height;
                WindowState = global_ui.maximized ? FormWindowState.Maximized : FormWindowState.Normal;
            }

            bool view_name_found = false;
            if (global_ui.selected_view != "")
                for (int idx = 0; idx < viewsTab.TabCount && !view_name_found; ++idx)
                    if (log_view_for_tab(idx).name == global_ui.selected_view) {
                        viewsTab.SelectedIndex = idx;
                        view_name_found = true;
                    }
            if (!view_name_found)
                viewsTab.SelectedIndex = 0;

            --ignore_change_;

            util.suspend_layout(this, false);

            ++ignore_change_;
            if (global_ui.left_pane_pos >= 0)
                main.SplitterDistance = global_ui.left_pane_pos;
            if (global_ui.full_log_splitter_pos >= 0)
                filteredLeft.SplitterDistance = global_ui.full_log_splitter_pos;
            --ignore_change_;

            util.postpone(() => show_row_based_on_global_ui(), 100);

            if (global_ui.selected_row_idx > 0)
                if (selected_file_name() == global_ui.log_name)
                    util.postpone(() => try_to_go_to_selected_line(global_ui.selected_row_idx), 250);
            util.postpone(() => show_tabs(global_ui.show_tabs), 100);


            /* not tested
            if (cur.topmost) {
                util.bring_to_topmost(this);
                update_topmost_image();
                update_toggle_topmost_visibility();                
            }*/
        }

        private void update_list_view_edit() {
            var lv = log_view_for_tab(viewsTab.SelectedIndex);
            if (lv.is_filter_up_to_date) 
                lv.update_edit();
            else 
                util.postpone(update_list_view_edit, 250);
        }

        private void try_to_go_to_selected_line(int selected_row_idx) {
            var lv = log_view_for_tab(viewsTab.SelectedIndex);
            if (lv.is_filter_up_to_date) {
                lv.go_to_row(selected_row_idx, log_view.select_type.do_not_notify_parent);
                if (lv.sel_line_idx >= 0)
                    on_log_changed_line_do_sync(lv.sel_line_idx, lv);
            } else
                util.postpone(() => try_to_go_to_selected_line(selected_row_idx), 250);
        }

        private void show_row_based_on_global_ui() {
            // 1.3.11d+ - right now, we're showing this as soon as we have enough rows
            foreach (var lv in all_log_views_and_full_log()) {
                var view = global_ui.view(lv.name);
                lv.column_positions = view.column_positions != "" ? view.column_positions : global_ui.global_column_positions;
                lv.apply_column_settings_only_to_me = view.column_positions != "";
                lv.set_filter( false, view.show_full_log );
            }
        }


        private void load_global_settings() {
            synchronizeWithExistingLogs.Checked = app.inst.sync_all_views;
            synchronizedWithFullLog.Checked = app.inst.sync_full_log_view;
            update_sync_texts();

            default_ui_.load("ui.default");
            for (int i = 0; i < custom_ui_.Length; ++i)
                custom_ui_[i].load("ui.custom" + i);
        }

        private void remove_log_view_from_tab_page(int idx) {
            Debug.Assert(idx < viewsTab.TabCount);
            TabPage tab = viewsTab.TabPages[idx];
            log_view lv = log_view_for_tab(idx);
            if (lv != null) {
                lv.mark_as_not_used();
                tab.Controls.Remove(lv);
            }
        }

        private void remove_log_view_tab(int idx) {
            // 1.0.51+ - yeah - RemoveAt() has a bug and quite often removes a different tab
            //viewsTab.TabPages.RemoveAt(idx);
            remove_log_view_from_tab_page(idx);

            var page = viewsTab.TabPages[idx];
            viewsTab.TabPages.Remove(page);            
        }

        private void remove_all_log_views() {
            ++ignore_change_;

            for (int idx = 0; idx < viewsTab.TabCount; ++idx)
                remove_log_view_from_tab_page(idx);

            // 1.1.5+ - if we had too many tabs, remove them
            int new_count = Math.Max(cur_context().views.Count, 1);
            while (viewsTab.TabCount > new_count)
                remove_log_view_tab(viewsTab.TabCount - 1);
            --ignore_change_;
        }

        private void new_view_Click(object sender, EventArgs e) {
            newViewMenu.Show(Cursor.Position);
        }

        private void delView_Click(object sender, EventArgs e) {
            int idx = viewsTab.SelectedIndex;
            if (idx < 0)
                return;

            ui_context cur = cur_context();
            if (cur.views.Count > 1) {
                cur.views.RemoveAt(idx);
                remove_log_view_tab(idx);
            } else {
                // it's the last tab, clear the filter
                cur.views[0].name = "View_1";
                cur.views[0].filters = new List<ui_filter>();
                on_view_name_changed(log_view_for_tab(0), cur.views[0].name);
                load_filters();
                save();
            }
        }


        private void load() {
            load_tabs();
            load_global_settings();
            load_ui();
            load_filters();
        }

        public void stop_saving() {
            ++ignore_change_;
        }

        private void save() {
            if (ignore_change_ > 0)
                return;
            if (text_ == null)
                return;

            if (log_parser_ != null) {
                string sett = factory.get_context_dependent_settings(text_, log_parser_.settings);
                cur_context().default_settings = sett;
            }
            if ( !app.inst.file_to_settings.ContainsKey(text_.name))
                app.inst.file_to_settings.Add(text_.name, "");
            app.inst.file_to_settings[text_.name] = factory.get_log_dependent_settings(text_, log_parser_.settings);
            save_contexts();

            default_ui_.save("ui.default");
            for (int i = 0; i < custom_ui_.Length; ++i)
                custom_ui_[i].save("ui.custom" + i);

            app.inst.save();
        }


        private void refresh_Tick(object sender, EventArgs ea) {
            if (curContextCtrl.DroppedDown)
                return;

            try {
                refresh_cur_log_view();
                update_status_text();
            } catch (Exception e) {
                logger.Error("Refresh error" + e.Message);
            }
        }

        private void saveTimer_Tick(object sender, EventArgs e) {
            save();
        }

        private void refresh_cur_log_view() {
            if (ignore_change_ > 0)
                return;
            if (text_ == null)
                // no log yet
                return;

            log_view_show_columns.refresh_visible_columns(all_log_views(), full_log);
            log_view lv = log_view_for_tab(viewsTab.SelectedIndex);
            if (lv == null) {
                Debug.Assert(false);
                return;
            }

            if (app.inst.instant_refresh_all_views) {
                refresh_all_views();
            } else {
                // optimized - refresh only current view
                update_filter(lv);
                lv.refresh();
                if (global_ui.show_fulllog) {
                    for (int idx = 0; idx < viewsTab.TabCount; ++idx) {
                        var other = log_view_for_tab(idx);
                        if (other != lv)
                            update_non_active_filter(idx);
                    }
                    full_log_ctrl_.refresh();
                    full_log_ctrl_.set_view_selected_view_name(lv.name);
                } else
                    // 1.4.3+ we always refresh the full log - since we allow toggling "show all lines" in all views
                    //        we need the full log to contain all lines
                    full_log.refresh();

                update_msg_details(false);
                refresh_filter_found();
            }

            if (text_.has_it_been_rewritten)
                on_rewritten_log();
        }

        // ... just setting .TopMost sometimes does not work
        private static void bring_to_topmost(log_wizard form) {
            form.TopMost = true;
            form.update_toggle_topmost_visibility();
            win32.MakeTopMost(form);
            /*
            form.TopMost = false;
            form.Activated += FormOnActivated;

            form.BringToFront();
            form.Focus();
            form.Activate();
            */
        }

        private void on_rewritten_log() {
            // 1.4.7+ i have on_change evet
            //foreach ( var lv in all_log_views_and_full_log())
              //  lv.clear();
            
            if (app.inst.bring_to_top_on_restart) {
                if (app.inst.make_topmost_on_restart) {
                    bring_to_topmost(this);
                    update_topmost_image();
                    update_toggle_topmost_visibility();
                } else
                    util.bring_to_top(this);
            }
        }

        private void update_msg_details(bool force_update) {
            if (selected_view() != null && msg_details_ != null) {
                int top_offset = 40;
                log_view any_lv = log_view_for_tab(0);
                if (any_lv != null) {
                    top_offset = any_lv.RectangleToScreen(any_lv.ClientRectangle).Top - RectangleToScreen(ClientRectangle).Top + 5;
                    if (global_ui.show_header)
                        top_offset += any_lv.list.HeaderControl.ClientRectangle.Height;
                }
                int bottom_offset = ClientRectangle.Height - lower.Top;
                msg_details_.update(selected_view(), top_offset, bottom_offset, force_update);
            }
        }

        private void update_non_active_filter(int idx) {
            var lv = log_view_for_tab(idx);
            if (lv.is_filter_set)
                return;

            ui_context ctx = cur_context();
            List<raw_filter_row> lvf = new List<raw_filter_row>();
            foreach (ui_filter filt in ctx.views[idx].filters) {
                var row = new raw_filter_row(filt.text, filt.apply_to_existing_lines);
                row.enabled = filt.enabled;
                row.dimmed = filt.dimmed;
                if (row.is_valid)
                    lvf.Add(row);
            }
            lv.set_filter(lvf);
        }

        private void refresh_filter_found() {
            log_view lv = log_view_for_tab(viewsTab.SelectedIndex);
            Debug.Assert(lv != null);
            if (old_line_count_ == lv.line_count)
                return;
            if (cur_view().filters.Count != lv.filter_row_count)
                // we can get here if one of the filter rows' text is invalid
                return;

            // recompute line count
            int filter_count = cur_view().filters.Count;
            for (int i = 0; i < filter_count; ++i) {
                int count = lv.filter_row_match_count(i);
                filtCtrl.new_row_count(i, count);
            }

            old_line_count_ = lv.line_count;
        }

        private void update_filter(log_view lv) {
            return;
            /* 1.0.91+ - we get notified of filter changes

            // as long as we're editing the filter, don't update anything
            if (filtCtrl.is_editing_any_filter)
                return;

            lv.set_filter( filtCtrl.to_filter_row_list());
            */
        }

        private void on_move_key_end() {
            on_log_changed_line();
            // the reason we have this is because of a bug in the renderer
            // apparently, it draws the selected rectangle with one extra pixel - thus, leaving a "trail" - we don't want that
            var sel = selected_view();
            sel.list.Refresh();
        }


        private void on_log_changed_line() {
            var sel = selected_view();
            update_notes_current_line();

            if (sel == full_log_ctrl_)                 
                return;
            
            on_log_changed_line_do_sync(sel.sel_line_idx, sel);

            global_ui.selected_row_idx = global_ui.selected_row_idx = sel.sel_row_idx;
            if (logHistory.SelectedIndex >= 0)
                global_ui.log_name = global_ui.log_name = history_[logHistory.SelectedIndex].name;
        }

        private void update_notes_current_line() {
            var sel = selected_view();
            if (sel.sel_line_idx >= 0)
                notes.set_current_line(new note_ctrl.line {idx = sel.sel_line_idx, msg = sel.sel_line_text, view_name = sel.name});
            else
                notes.set_current_line(new note_ctrl.line {idx = -1, msg = "", view_name = ""});
        }

        public void on_sel_line(log_view lv, int line_idx) {
            if (any_moving_key_still_down())
                // user is still moving the selection
                return;

            logger.Debug("[log] new line " + lv.name + " = " + line_idx);
            update_notes_current_line();
        }

        public void on_edit_aliases() {
            var form = new alias_form(log_parser_.aliases, log_parser_.column_names);
            if (form.ShowDialog() == DialogResult.OK) {
                log_parser_.aliases = form.new_aliases;
                logger.Debug("changed aliases, resolving= " + form.new_aliases.dump_resolve_names());
                save();
                if ( form.needs_restart) 
                    if ( MessageBox.Show("Would you like to restart LogWizard now?", "LogWizard", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        util.restart_app();
            }
        }

        private void on_log_changed_line_do_sync(int line_idx, log_view from) {
            if (global_ui.show_fulllog) {
                if (from != full_log_ctrl_ && app.inst.sync_full_log_view)
                    full_log_ctrl_.go_to_row(line_idx, log_view.select_type.do_not_notify_parent);
            }

            bool keep_all_in_sync = (from != full_log_ctrl_ && app.inst.sync_all_views) ||
                                    // if the current log is full log, we will synchronize all views only if both checks are checked
                                    // (note: this is always a bit time consuming as well)
                                    (from == full_log_ctrl_ && app.inst.sync_all_views && app.inst.sync_full_log_view);
            if (keep_all_in_sync)
                keep_logs_in_sync(from);
        }

        private void sourceName_TextChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;
            if (logHistory.DroppedDown)
                // user is going through the history, hasn't made up his mind yet
                return;

            if (sourceTypeCtrl.SelectedIndex == 0 && File.Exists(sourceNameCtrl.Text))
                on_new_file_log(sourceNameCtrl.Text);
            else if (sourceTypeCtrl.SelectedIndex == 1)
                on_new_shared_log(sourceNameCtrl.Text);
        }

        private void sourceType_SelectedIndexChanged(object sender, EventArgs e) {
            if (sourceTypeCtrl.SelectedIndex == 2)
                on_new_debug_log();
        }

        /* 1.1.25+
            - on new file (that does not match any context)
              - we will create a context matching the name of the file (if one exists, we will automatically select it)
              - by default, we'll never go to the "Default" context
              - the idea is for the uesr not to have to mistakenly delete a context when he's selecting a different type of file,
                (since he would want new filters). thus, just create a new context where he can do anything
        */
        private void create_context_for_file(string name) {
            ui_context file_ctx = file_to_context(name);
            if (file_ctx.name != "Default")
                // we already have a context
                return;

            ui_context new_ctx = new ui_context();
            new_ctx.name = Path.GetFileNameWithoutExtension(new FileInfo(name).Name);
            contexts_.Add(new_ctx);
            var syntax = new find_log_syntax().try_find_log_syntax_file(name);
            if (syntax != find_log_syntax.UNKNOWN_SYNTAX)
                new_ctx.default_settings = syntax;

            update_contexts_combos_in_all_forms();
        }

        private void on_new_file_log(string name) {
            if (text_ != null && text_.name == name) {
                merge_notes();
                return;
            }

            if (text_ != null)
                text_.Dispose();

            create_context_for_file(name);

            text_ = new file_text_reader(name);
            on_new_log();

            ui_context file_ctx = file_to_context(name);
            if (file_ctx != cur_context())
                // update context based on file name
                curContextCtrl.SelectedIndex = contexts_.IndexOf(file_ctx);

            if (logSyntaxCtrl.Text == find_log_syntax.UNKNOWN_SYNTAX) {
                if (log_parser_.needs_text_syntax) {
                    set_status("We don't know the syntax of this Log File. We recommend you set it yourself. Press the 'Test' button on the top-right.",
                        status_type.err);
                    show_source(true);
                }
            } else if (!cur_context().has_not_empty_views)
                set_status("Don't the columns look ok? Perpaps LogWizard did not correctly parse them... If so, Toggle the Source Pane ON (Alt-O), anc click on 'Test'.", status_type.warn, 15000);

            force_initial_refresh_of_all_views();
        }

        private void refresh_all_views() {
            if (ignore_change_ > 0)
                return;
            if (text_ == null)
                // no log yet
                return;
            log_view lv = log_view_for_tab(viewsTab.SelectedIndex);
            if (lv == null) {
                Debug.Assert(false);
                return;
            }

            update_filter(lv);
            lv.refresh();

            for (int idx = 0; idx < viewsTab.TabCount; ++idx) {
                var other = log_view_for_tab(idx);
                if (other != lv) {
                    update_non_active_filter(idx);
                    other.refresh();
                }
            }

            if (global_ui.show_fulllog) {
                full_log_ctrl_.refresh();
                full_log_ctrl_.set_view_selected_view_name(lv.name);
            } else
                // 1.4.3+ we always refresh the full log - since we allow toggling "show all lines" in all views
                //        we need the full log to contain all lines
                full_log_ctrl_.refresh();

            update_msg_details(false);
            refresh_filter_found();
        }

        private void force_initial_refresh_of_all_views() {
            // note: refreshing happens on different threads, so we're not sure when it's complete
            //       just take a guess and refresh in a bit

            foreach (log_view lv in all_log_views_and_full_log())
                lv.turn_off_has_anying_changed = true;
            refresh_all_views();

            util.add_timer((has_terminated) => {
                refresh_all_views();
                if (has_terminated)
                    foreach (log_view lv in all_log_views_and_full_log())
                        lv.turn_off_has_anying_changed = false;
            }, () => {
                foreach (log_view lv in all_log_views_and_full_log())
                    if (!lv.is_filter_up_to_date)
                        return false;
                logger.Debug("[view] initial refresh complete");
                // we allocated a lot of interim objects
                GC.Collect();
                return true;
            }, 500);
        }

        private void on_new_shared_log(string name) {
            if (text_ != null)
                text_.Dispose();

            if (text_ != null && !(text_ is shared_memory_text_reader))
                text_ = new shared_memory_text_reader();
            ((shared_memory_text_reader) text_).set_memory_name(name);
            on_new_log();
        }

        private void on_new_debug_log() {
            if (text_ != null)
                text_.Dispose();

            text_ = new debug_text_reader();
            on_new_log();
        }

        private string reader_title() {
            int sel = logHistory.SelectedIndex;
            if (sel >= 0)
                return history_[sel].ui_friendly_name;

            var file = text_ as file_text_reader;
            if (file != null)
                return file.name;
            var sh = text_ as shared_memory_text_reader;
            if (sh != null)
                return "Shared " + sh.name;
            var dbg = text_ as debug_text_reader;
            if (dbg != null)
                return "Debug Window";
            Debug.Assert(false);
            return "Log";
        }

        private void on_new_log_parser() {
            full_log_ctrl_.set_log(new log_reader(log_parser_));
            for (int i = 0; i < viewsTab.TabCount; ++i) {
                var lv = log_view_for_tab(i);
                if (lv != null)
                    lv.set_log(new log_reader(log_parser_));
            }
        }

        private int history_select(string name, string friendly_name = "") {
            ++ignore_change_;
            bool needs_save = false;
            logHistory.SelectedIndex = -1;

            bool found = false;
            for (int i = 0; i < history_.Count && !found; ++i)
                if (history_[i].name == name) {
                    found = true;
                    bool is_sample = name.ToLower().EndsWith("logwizardsetupsample.log");
                    // if not default form - don't move the selection to the end
                    bool is_default_form = toggled_to_custom_ui_ < 0;
                    if (is_sample || !is_default_form)
                        logHistory.SelectedIndex = i;
                    else {
                        // whatever the user selects, move it to the end
                        history h = history_[i];
                        history_.RemoveAt(i);
                        history_.Add(h);
                        logHistory.Items.RemoveAt(i);
                        logHistory.Items.Add(h.ui_friendly_name);
                        logHistory.SelectedIndex = logHistory.Items.Count - 1;
                        needs_save = true;
                    }
                }

            if (logHistory.SelectedIndex < 0) {
                // FIXME (minor) if not on the default form -> i should not put it last (since that will be automatically loaded at restart)
                history_.Add(new history {name = name, type = 0, friendly_name = friendly_name});
                logHistory.Items.Add(history_.Last().ui_friendly_name);
                logHistory.SelectedIndex = logHistory.Items.Count - 1;
            }
            --ignore_change_;

            if (needs_save)
                save();
            return logHistory.SelectedIndex;
        }

        private void on_new_log() {
            string size = text_ is file_text_reader ? " (" + new FileInfo(text_.name).Length + " bytes)" : "";
            set_status_forever("Log: " + text_.name + size);
            dropHere.Visible = false;

            // by default - try to find the syntax by reading the header info
            //              otherwise, try to parse it
            string file_to_syntax = text_ is file_text_reader ? log_to.file_to_syntax(text_.name) : null;
            if (file_to_syntax == null)
                file_to_syntax = "";

            string name = text_.name;
            if (history_.Count < 1)
                history_select(name);

            // 1.1.25+ if I can't find the syntax from file-to-syntax, or by parsing the log, see if the context associated with this file has log-syntax
            string context_settings = file_to_context(name).default_settings;
            string file_default_syntax = "syntax=" + text_.try_to_find_log_syntax();
            // if user specifically overrides settings for a log, use that
            string log_settings = log_to.log_to_settings(name);
            if (file_to_syntax != "")
                log_settings = factory.merge_settings(log_settings, "syntax=" + file_to_syntax); // this overrides file settings
            else if (log_settings == "")
                if ( factory.get_single_setting(context_settings,"syntax") == "")
                    log_settings = file_default_syntax;

            string settings = factory.merge_settings(context_settings, log_settings);

            logSyntaxCtrl.Text = factory.get_single_setting(settings, "syntax");
            logSyntaxCtrl.ForeColor = logSyntaxCtrl.Text != find_log_syntax.UNKNOWN_SYNTAX ? Color.Black : Color.Red;

            // note: we recreate the log, so that cached filters know to rebuild
            log_parser_ = new log_parser(text_, settings);
            on_new_log_parser();

            full_log_ctrl_.set_filter(new List<raw_filter_row>());

            Text = reader_title() + " - Log Wizard " + version();
            add_reader_to_history();

            load_bookmarks();
            logger.Info("new reader " + history_[logHistory.SelectedIndex].name);

            // at this point, some file has been dropped
            log_view_for_tab(0).Visible = true;

            notes.Enabled = text_ is file_text_reader;
            if (notes.Enabled) {
                notes.load(notes_keeper.inst.notes_file_for_file(text_.name));
                merge_notes();
            }

            util.postpone(update_list_view_edit, 250);
        }

        private void merge_notes() {
            if (post_merge_file_ != "" && File.Exists(post_merge_file_)) {
                notes.merge(post_merge_file_);
                post_merge_file_ = "";
                // has merged notes?
                if (notes.has_merged_notes) {
                    if (!global_ui.show_notes) {
                        global_ui.show_notes = true;
                        show_left_pane(true);
                    }
                }
            }
        }

        private void add_reader_to_history() {
            if (text_ is debug_text_reader)
                return;
            history new_ = new history();
            if (text_ is file_text_reader) {
                new_.name = ((file_text_reader) text_).name;
                new_.type = history.entry_type.file;
            } else if (text_ is shared_memory_text_reader) {
                new_.name = ((shared_memory_text_reader) text_).name;
                new_.type = history.entry_type.shmem;
            } else
                Debug.Assert(false);

            int history_idx = -1;
            for (int i = 0; i < history_.Count && history_idx < 0; ++i)
                if (new_.name == history_[i].name && new_.type == history_[i].type)
                    history_idx = i;
            if (history_idx < 0) {
                history_.Add(new_);
                history_idx = history_.Count - 1;
                logHistory.Items.Add(new_.ui_friendly_name);
            } else {
                ++ignore_change_;
                friendlyNameCtrl.Text = history_[history_idx].friendly_name;
                --ignore_change_;
            }

            ++ignore_change_;
            logHistory.SelectedIndex = history_idx;
            --ignore_change_;
            update_history();
        }

        private void update_history() {
            int history_idx = logHistory.SelectedIndex;
            ++ignore_change_;
            logHistory.Items.Clear();
            foreach (history hist in history_)
                logHistory.Items.Add(hist.ui_friendly_name);
            logHistory.SelectedIndex = history_idx;
            --ignore_change_;
        }

        private void logHistory_DropDownClosed(object sender, EventArgs e) {
            if (logHistory.Items.Count < 1)
                return; // nothing is in history

            //sourceName_TextChanged(null,null);
            on_log_listory_changed();

            util.postpone(() => {
                if (global_ui.show_current_view)
                    log_view_for_tab(viewsTab.SelectedIndex).set_focus();
                else
                    full_log_ctrl_.set_focus();
            }, 100);
        }

        private void close_history_dropdown() {
            ++ignore_change_;
            if (selected_history_on_dropdown_ >= 0)
                logHistory.SelectedIndex = selected_history_on_dropdown_;
            --ignore_change_;
            logHistory.DroppedDown = false;
        }

        private void logHistory_DropDown(object sender, EventArgs e) {
            selected_history_on_dropdown_ = logHistory.SelectedIndex;
            set_status("To get back to where you were, press ESC.");
        }

        private void logHistory_SelectedIndexChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0 || logHistory.SelectedIndex < 0)
                return;
            if (logHistory.DroppedDown)
                return;
            on_log_listory_changed();
        }

        private void on_log_listory_changed() {
            if (logHistory.SelectedIndex >= 0) {
                history_select(history_[logHistory.SelectedIndex].name);
                app.inst.set_log_file(selected_file_name());
            }

            sourceTypeCtrl.SelectedIndex = (int) history_[logHistory.SelectedIndex].type;
            sourceNameCtrl.Text = history_[logHistory.SelectedIndex].name;
            friendlyNameCtrl.Text = history_[logHistory.SelectedIndex].friendly_name;
            sourceName_TextChanged(null, null);
        }

        private void friendlyName_TextChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;
            history_[logHistory.SelectedIndex].friendly_name = friendlyNameCtrl.Text;
            update_history();
            save();
        }

        private void logSyntax_TextChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;
        }

        private void LogWizard_FormClosing(object sender, FormClosingEventArgs e) {
            save();
        }

        private void viewsTab_SelectedIndexChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;
            // we should never arrive at "no tab"
            Debug.Assert(viewsTab.SelectedIndex >= 0);
            ensure_we_have_log_view_for_tab(viewsTab.SelectedIndex);
            load_filters();

            string name = log_view_for_tab(viewsTab.SelectedIndex).name;
            // in this case - even if in a custom UI, we still want to remember the last selected view
            global_ui.selected_view = global_ui.selected_view = name;
            update_notes_current_line();
        }


        private void addContext_Click(object sender, EventArgs e) {
            new_context_form new_ = new new_context_form(this);
            new_.Location = Cursor.Position;
            if (new_.ShowDialog() == DialogResult.OK) {
                ui_context new_ctx = new ui_context();
                if (new_.basedOnExisting.Checked)
                    new_ctx.copy_from(cur_context());
                // 1.3.33+ - we want unique context names!
                new_ctx.name = util.unique_name( contexts_.Select(x => x.name), new_.name.Text);
                contexts_.Add(new_ctx);
                update_contexts_combos_in_all_forms();
                curContextCtrl.SelectedIndex = curContextCtrl.Items.Count - 1;
            }
        }

        private void delContext_Click(object sender, EventArgs e) {
            // make sure we have at least one, after deleting the current one
            if (curContextCtrl.Items.Count < 2)
                return;

            int sel = curContextCtrl.SelectedIndex;
            contexts_.RemoveAt(sel);
            update_contexts_combos_in_all_forms();
            curContextCtrl.SelectedIndex = curContextCtrl.Items.Count > sel ? sel : 0;
        }

        private string selected_file_name() {
            if (logHistory.SelectedIndex >= 0)
                return history_[logHistory.SelectedIndex].file_name;
            else
                return "";
        }

        private void curContextCtrl_SelectedIndexChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;

            on_context_changed();

            // first, remove all log views, so that the new filters (from the new context) are loaded
            remove_all_log_views();

            string name = selected_file_name();
            int default_context = contexts_.IndexOf(file_to_context(name));
            if (name != "" && default_context != curContextCtrl.SelectedIndex) {
                if (!app.inst.forced_file_to_context.ContainsKey(name))
                    app.inst.forced_file_to_context.Add(name, "");
                app.inst.forced_file_to_context[name] = contexts_[curContextCtrl.SelectedIndex].name;
            }

            load();

            if (full_log_ctrl_ != null)
                full_log_ctrl_.refresh();
            refresh_cur_log_view();
            save();
        }

        private void curContextCtrl_DropDown(object sender, EventArgs e) {
            // saving after the selection is changed would be too late
            save();
        }


        private void dropHere_DragDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                    on_file_drop(files[0]);
            }
        }

        private void dropHere_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = e.AllowedEffect;
            else
                e.Effect = DragDropEffects.None;
        }


        private void contextMatch_TextChanged(object sender, EventArgs e) {
            // not implemented yet
        }


        private void refreshFilter_Click(object sender, EventArgs e) {
            if (text_ != null)
                log_parser_.reload();
            refresh_filter_found();

            util.add_timer((has_ended) => {
                refreshFilter.Enabled = has_ended;
                refreshFilter.Text = has_ended ? "Refresh" : util.add_dots(refreshFilter.Text, 3);
            }, 2500, 250);
        }


        private int handled_key_idx_ = 0;

        private void LogWizard_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (key_to_action(e) != action_type.none)
                e.IsInputKey = true;
        }

        private void LogWizard_KeyDown(object sender, KeyEventArgs e) {
            // processing stuff in ProcessCmdKey now
            ++handled_key_idx_;
            //logger.Debug("key pressed - " + e.KeyCode + " sender " + sender);
            var action = key_to_action(e);
            if (action != action_type.none) {
                e.Handled = true;
                e.SuppressKeyPress = true;
                // note: some hotkeys are sent twice
                bool handle_now = !is_key_sent_twice() || (handled_key_idx_ % 2 == 0);
                if (handle_now) {
                    handle_action(action);
                    logger.Info("action by key - " + action); // + " from " + sender);
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == (Keys.Control | Keys.Tab)) {
                handle_action(action_type.next_view);
                return true;
            }
            if (keyData == (Keys.Control | Keys.Shift | Keys.Tab)) {
                handle_action(action_type.prev_view);
                return true;
            }

            // 1.2.12 - tried, but doesn't work
#if old_code
            var action = key_to_action(keyData);
            if (action != action_type.none) {
                handle_action(action);
                logger.Info("action by key - " + action); // + " from " + sender);
                return true;
            }
#endif
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool any_moving_key_still_down() {
            return win32.IsKeyPushedDown(Keys.Up) || win32.IsKeyPushedDown(Keys.Down) || win32.IsKeyPushedDown(Keys.PageUp) || win32.IsKeyPushedDown(Keys.PageDown) || win32.IsKeyPushedDown(Keys.Home) || win32.IsKeyPushedDown(Keys.End);
        }

        private void log_wizard_KeyUp(object sender, KeyEventArgs e) {
            // see if any of the moving keys is still down
            if (!any_moving_key_still_down()) {
                if (notes.is_focus_on_notes_list)
                    return;

                switch (util.key_to_action(e)) {
                case "up":
                case "down":
                case "pageup":
                case "next":
                case "home":
                case "end":
                    on_move_key_end();
                    break;
                }
            }
        }


        private bool is_key_sent_twice() {
            return is_focus_on_full_log();
        }

        public void handle_subcontrol_keys(Control c) {
            c.PreviewKeyDown += LogWizard_PreviewKeyDown;
            c.KeyDown += LogWizard_KeyDown;
            c.KeyUp += log_wizard_KeyUp;

            foreach (Control sub in c.Controls)
                handle_subcontrol_keys(sub);
        }


        private action_type key_to_action(Keys e) {
            return key_to_action(util.key_to_action(e));
        }

        private action_type key_to_action(KeyEventArgs e) {
            return key_to_action(util.key_to_action(e));
        }

        private action_type key_to_action(PreviewKeyDownEventArgs e) {
            return key_to_action(util.key_to_action(e));
        }

        private Control focused_ctrl() {
            return win32.focused_ctrl();
        }

        // ... this is always editable
        private bool is_focus_on_editable_box() {
            var focused = focused_ctrl();
            var box = focused as TextBox;
            return box != null && !box.ReadOnly;
        }

        // ... this can be editable or non-editable (the richchtexbox is readonly)
        private bool is_focus_on_text_box() {
            var focused = focused_ctrl();
            return focused != null && (focused is TextBox || focused is RichTextBox);
        }

        private bool allow_arrow_to_function_normally() {
            if (is_focus_on_filter_panel())
                return true;
            if (is_focus_on_text_box())
                return true;
            var focused = focused_ctrl();
            if (focused == logHistory)
                return true;
            return false;
        }

        // in case the Filter is selected, make sure we remove focus from it
        private void unfocus_filter_panel() {
            if (is_focus_on_filter_panel())
                // 1.2.13+ - make sure to select something meaningful
                selected_view().set_focus();
            //viewsTab.Focus();            
        }

        private bool is_focus_on_full_log() {
            return full_log_ctrl_ != null && full_log_ctrl_.has_focus;
        }

        private bool is_focus_on_filter_panel() {
            var focus = focused_ctrl();
            return filtCtrl.tab_navigatable_controls.Contains(focus);
        }

        private action_type key_to_action(string key_code) {
            if (key_code == "")
                return action_type.none;
            if (!app.inst.use_hotkeys)
                return action_type.none;

            // 1.1.15+ - let notes take care of navigating keys
            if (notes.is_focus_on_notes_list)
                return action_type.none;

            if (any_moving_key_still_down())
                if (notes.is_focus_on_notes_list)
                    return action_type.none;

            switch (key_code) {
            case "up":
            case "down":
            case "pageup":
            case "next":
            case "home":
            case "end":
            case "space":
            case "return":
                if (key_code == "space" && filtCtrl.can_handle_toggle_enable_dimmed_now)
                    break;

                if (allow_arrow_to_function_normally())
                    return action_type.none;
                break;
            case "ctrl-right":
            case "ctrl-left":

            case "ctrl-c":
            case "ctrl-shift-c":
                if (is_focus_on_editable_box())
                    return action_type.none;
                break;
            }

            bool has_modifiers = key_code.Contains("ctrl-") || key_code.Contains("alt-") || key_code.Contains("shift-");
            if (!has_modifiers && key_code != "tab" && is_focus_on_editable_box())
                // key down - in edit -> don't have it as hotkey
                return action_type.none;

            switch (key_code) {
            case "f3":
                return action_type.search_next;
            case "shift-f3":
                return action_type.search_prev;
            case "escape":
                return action_type.escape;

            case "apps":
                return action_type.right_click_via_key;

            case "ctrl-f":
                return action_type.search;
            case "ctrl-shift-f":
                return action_type.default_search;
            case "ctrl-f2":
                return action_type.toggle_bookmark;
            case "f2":
                return action_type.next_bookmark;
            case "shift-f2":
                return action_type.prev_bookmark;
            case "ctrl-shift-f2":
                return action_type.clear_bookmarks;

            case "ctrl-c":
                return action_type.copy_full_line_to_clipboard;
            case "ctrl-shift-c":
                return action_type.copy_to_clipboard;

                /* 1.4.9+ use ctrl-tab, ctrl-shift-tab ; ctrl-left/right can be used for word moving inside smart edit
            case "ctrl-right":
                return action_type.next_view;
            case "ctrl-left":
                return action_type.prev_view;
                */
            case "home":
                return action_type.home;
            case "end":
                return action_type.end;
            case "pageup":
                return action_type.pageup;
            case "next":
                return action_type.pagedown;
            case "up":
                return action_type.arrow_up;
            case "down":
                return action_type.arrow_down;

            case "shift-up":
                return action_type.shift_arrow_up;
            case "shift-down":
                return action_type.shift_arrow_down;

            case "tab":
                return action_type.pane_next;
            case "shift-tab":
                return action_type.pane_prev;

            case "add":
                return action_type.increase_font;
            case "subtract":
                return action_type.decrease_font;
            case "space":
                if (filtCtrl.can_handle_toggle_enable_dimmed_now)
                    return action_type.toggle_enabled_dimmed;
                break;

            case "ctrl-h":
                return action_type.toggle_history_dropdown;
            case "ctrl-n":
                return action_type.new_log_wizard;
            case "ctrl-p":
                return action_type.show_preferences;
            case "ctrl-up":
                return action_type.scroll_up;
            case "ctrl-down":
                return action_type.scroll_down;
            case "ctrl-g":
                return action_type.go_to_line;
            case "f5":
                return action_type.refresh;
            case "ctrl-o":
                return action_type.open_in_explorer;

            case "ctrl-1":
                return action_type.goto_position_1;
            case "ctrl-2":
                return action_type.goto_position_2;
            case "ctrl-3":
                return action_type.goto_position_3;
            case "ctrl-4":
                return action_type.goto_position_4;
            case "ctrl-5":
                return action_type.goto_position_5;
            case "ctrl-e":
                return action_type.export_notes;
            case "ctrl-z":
                return action_type.undo;

            ////////////////////////////////////////////////////////////////////////////////
            // toggles via hotkey
            case "alt-f":
                return action_type.toggle_filters;
            case "alt-n":
                return action_type.toggle_notes;
            case "alt-o":
                return action_type.toggle_source;
            case "alt-l":
                return action_type.toggle_fulllog;

            case "alt-t":
                return action_type.toggle_title;
            case "alt-v":
                return action_type.toggle_view_tabs;
            case "alt-h":
                return action_type.toggle_view_header;
            case "alt-d":
                return action_type.toggle_details;
            case "alt-s":
                return action_type.toggle_status;

            case "ctrl-alt-f":
                return action_type.toggle_filter_view;
            case "ctrl-alt-l":
                return action_type.toggle_show_full_log;

            }

            return action_type.none;
        }

        private void handle_action(action_type action) {
            int sel = viewsTab.SelectedIndex;
            var lv = selected_view();
            Debug.Assert(lv != null);

            switch (action) {
            case action_type.search:
                if (lv.item_count < 1) {
                    set_status("Can't search - nothing to search", status_type.err);
                    break;
                }
                var searcher = new search_form(this, lv, lv.smart_edit_sel_text);
                if (searcher.ShowDialog() == DialogResult.OK) {
                    // remove focus from the Filters tab, just in case (otherwise, search prev/next would end up working on that)
                    unfocus_filter_panel();

                    lv.search_for_text(searcher.search);
                    lv.search_for_text_first();
                }
                break;

            case action_type.default_search:
                // remove focus from the Filters tab, just in case (otherwise, search prev/next would end up working on that)
                unfocus_filter_panel();
                var search = search_form.default_search;
                var sel_text = lv.smart_edit_sel_text;
                if (sel_text != "") {
                    search.text = sel_text;
                    search.type = 1; // text 
                }
                lv.search_for_text(search);
                lv.search_next();
                break;

            case action_type.search_prev:
                lv.search_prev();
                break;
            case action_type.search_next:
                lv.search_next();
                break;
            case action_type.escape:
                if (logHistory.DroppedDown)
                    close_history_dropdown();
                else
                    lv.escape();
                break;

            case action_type.right_click_via_key:
                lv.do_right_click_via_key();
                break;

            case action_type.next_view: {
                int prev_idx = viewsTab.SelectedIndex;
                int next_idx = viewsTab.TabCount > 0 ? (sel + 1) % viewsTab.TabCount : -1;
                if (next_idx >= 0) {
                    viewsTab.SelectedIndex = next_idx;
                    log_view_for_tab(next_idx).on_selected();
                }
                if (prev_idx >= 0)
                    log_view_for_tab(prev_idx).update_x_of_y();
            }
                break;
            case action_type.prev_view: {
                int prev_idx = viewsTab.SelectedIndex;
                int next_idx = viewsTab.TabCount > 0 ? (sel + viewsTab.TabCount - 1) % viewsTab.TabCount : -1;
                if (next_idx >= 0) {
                    viewsTab.SelectedIndex = next_idx;
                    log_view_for_tab(next_idx).on_selected();
                }
                if (prev_idx >= 0)
                    log_view_for_tab(prev_idx).update_x_of_y();
            }
                break;

            case action_type.home:
            case action_type.end:
            case action_type.pageup:
            case action_type.pagedown:
            case action_type.arrow_up:
            case action_type.arrow_down:
            case action_type.shift_arrow_down:
            case action_type.shift_arrow_up:
                lv.on_action(action);
                break;

            case action_type.toggle_filters:
                toggle_filters();
                break;
            case action_type.toggle_notes:
                toggle_notes();
                break;
            case action_type.toggle_fulllog:
                toggle_full_log();
                break;
            case action_type.toggle_source:
                toggle_source();
                break;

            case action_type.copy_to_clipboard:
                lv.copy_to_clipboard();
                set_status("Lines copied to Clipboard, as text and html");
                break;
            case action_type.copy_full_line_to_clipboard:
                lv.copy_full_line_to_clipboard();
                set_status("Full Lines copied to Clipboard, as text and html");
                break;

            case action_type.toggle_bookmark:
                int line_idx = lv.sel_line_idx;
                if (line_idx >= 0) {
                    if (bookmarks_.Contains(line_idx))
                        bookmarks_.Remove(line_idx);
                    else
                        bookmarks_.Add(line_idx);
                    save_bookmarks();
                    notify_views_of_bookmarks();
                }
                break;
            case action_type.clear_bookmarks:
                bookmarks_.Clear();
                save_bookmarks();
                notify_views_of_bookmarks();
                break;
            case action_type.next_bookmark:
                lv.next_bookmark();
                break;
            case action_type.prev_bookmark:
                lv.prev_bookmark();
                break;

            case action_type.pane_next:
                switch_pane(true);
                break;
            case action_type.pane_prev:
                switch_pane(false);
                break;

            case action_type.toggle_history_dropdown:
                if (logHistory.DroppedDown)
                    close_history_dropdown();
                else {
                    logHistory.Focus();
                    logHistory.DroppedDown = true;
                }
                break;
            case action_type.new_log_wizard:
                newView_Click(null, null);
                break;
            case action_type.show_preferences:
                settingsCtrl_Click(null, null);
                break;

            case action_type.increase_font:
                foreach (log_view view in all_log_views_and_full_log())
                    view.increase_font(1);
                break;
            case action_type.decrease_font:
                foreach (log_view view in all_log_views_and_full_log())
                    view.increase_font(-1);
                break;

            case action_type.scroll_up:
                lv.scroll_up();
                break;
            case action_type.scroll_down:
                lv.scroll_down();
                break;

            case action_type.go_to_line:
                var dlg = new go_to_line_time_form(this);
                if (dlg.ShowDialog() == DialogResult.OK) {
                    if (dlg.is_number()) {
                        if (dlg.has_offset != '\0')
                            lv.offset_closest_line(dlg.number, dlg.has_offset == '+');
                        else
                            lv.go_to_closest_line(dlg.number - 1, log_view.select_type.notify_parent);
                    } else if (dlg.has_offset != '\0')
                        lv.offset_closest_time(dlg.time_milliseconds, dlg.has_offset == '+');
                    else
                        lv.go_to_closest_time(dlg.normalized_time);
                }
                break;
            case action_type.refresh:
                refreshFilter_Click(null, null);
                break;
            case action_type.toggle_title:
                toggle_title();
                break;
            case action_type.toggle_status:
                toggle_status();
                break;

            case action_type.toggle_view_tabs:
                toggle_view_tabs();
                break;
            case action_type.toggle_view_header:
                toggle_view_header();
                break;
            case action_type.toggle_details:
                toggle_details();
                break;

            case action_type.open_in_explorer:
                if (text_ != null)
                    util.open_in_explorer(text_.name);
                break;

            case action_type.none:
                break;

            case action_type.goto_position_1:
                toggle_custom_ui(0);
                break;
            case action_type.goto_position_2:
                toggle_custom_ui(1);
                break;
            case action_type.goto_position_3:
                toggle_custom_ui(2);
                break;
            case action_type.goto_position_4:
                toggle_custom_ui(3);
                break;
            case action_type.goto_position_5:
                toggle_custom_ui(4);
                break;

            case action_type.goto_position_6:
                toggle_custom_ui(5);
                break;
            case action_type.goto_position_7:
                toggle_custom_ui(6);
                break;
            case action_type.goto_position_8:
                toggle_custom_ui(7);
                break;
            case action_type.goto_position_9:
                toggle_custom_ui(8);
                break;
            case action_type.goto_position_10:
                toggle_custom_ui(9);
                break;




            case action_type.toggle_enabled_dimmed:
                filtCtrl.toggle_enabled_dimmed();
                break;

            case action_type.export_notes:
                export_notes_to_logwizard_file();
                break;

            case action_type.undo:
                if (global_ui.show_filter)
                    filtCtrl.undo();
                else if (global_ui.show_notes)
                    notes.undo();
                break;

            case action_type.toggle_filter_view:
                lv.set_filter( !lv.filter_view, lv.show_full_log);
                break;
            case action_type.toggle_show_full_log: {
                lv.set_filter(lv.filter_view, !lv.show_full_log);
                var old = global_ui.view(lv.name);
                global_ui.view(lv.name, new ui_info.view_info(old.column_positions, !old.show_full_log) );
            }
                break;

            default:
                Debug.Assert(false);
                break;
            }
        }

        private void toggle_custom_ui(int idx) {
            foreach ( var f in forms)
                if ( f != this)
                    if (f.toggled_to_custom_ui_ == idx) {
                        // another form has this custom UI - just bring it to top
                        util.bring_to_top(f);
                        return;
                    }
            if ( toggled_to_custom_ui_ == idx) 
                // in this case, we're toggling off the custom UI for this form (moving to default location)
                // however, first - check if we already have a form to the default location. If so, go to that
                foreach ( var f in forms)
                    if ( f != this)
                        if (f.toggled_to_custom_ui_ == -1) {
                            // another form has this custom UI - just bring it to top
                            util.bring_to_top(f);
                            return;
                        }

            int new_ui = idx == toggled_to_custom_ui_ ? -1 : idx;
            if (new_ui != -1) {
                // going to a custom position
                if (!custom_ui_[idx].was_set_at_least_once) {
                    // 1.3.33+ - copy from the existing position - this way, we can create a "copy" of where we were
                    custom_ui_[idx].copy_from(toggled_to_custom_ui_ < 0 ? default_ui_ : custom_ui_[toggled_to_custom_ui_]);
                    set_status("Copied existing Settings from Position " + (toggled_to_custom_ui_ + 1));
                }
            } else {
                // going to default position (from a custom position)
            }
            toggled_to_custom_ui_ = new_ui;
            load_ui();
            update_status_prefix();
            save();
        }

        private void update_status_prefix() {
            status_prefix_ = toggled_to_custom_ui_ < 0 ? "" : "[Position " + (toggled_to_custom_ui_ + 1) + "]";
            force_udpate_status_text();            
        }

        private List<Control> panes() {
            List<Control> panes = new List<Control>();

            // first pane - the current view (tab)
            int sel = viewsTab.SelectedIndex;
            if (sel >= 0 && log_view_for_tab(sel) != null)
                panes.Add(log_view_for_tab(sel));

            // second pane - the full log (if shown)
            if (global_ui.show_fulllog)
                panes.Add(full_log_ctrl_);

            // third/fourth panes - the filters control and edit box (if visible)
            if (global_ui.show_filter)
                panes.AddRange(filtCtrl.tab_navigatable_controls);
            else if (global_ui.show_notes)
                panes.AddRange(notes.tab_navigatable_controls);
            return panes;
        }

        // keeps the other logs in sync with this one - if needed
        private void keep_logs_in_sync(log_view src) {
            int line_idx = src.sel_line_idx;
            if (line_idx < 0)
                return;
            foreach (log_view lv in all_log_views_and_full_log())
                if (lv != src) {
                    if (global_ui.show_fulllog && lv == full_log_ctrl_ && app.inst.sync_full_log_view)
                        // in this case, we already synched the full log
                        continue;

                    lv.go_to_closest_line(line_idx, log_view.select_type.do_not_notify_parent);
                }
        }

        private void switch_pane(bool forward) {
            List<Control> panes = this.panes();
            Control focus_ctrl = focused_ctrl();
            int idx = -1;
            while (idx < 0 && focus_ctrl != null) {
                idx = panes.IndexOf(focus_ctrl);
                if (idx < 0)
                    focus_ctrl = focus_ctrl.Parent;
            }

            if (idx >= 0)
                // move to next control
                idx = forward ? idx + 1 : idx + panes.Count - 1;
            else
            // move to first / last
                idx = forward ? 0 : panes.Count - 1;

            var cur_pane = panes[idx % panes.Count];
            activate_pane(cur_pane);
        }

        private void activate_pane(Control cur_pane) {
            logger.Debug("activating pane " + cur_pane);
            Debug.Assert(cur_pane != null);

            active_pane_ = cur_pane;
            var lv_pane = cur_pane as log_view;
            var list_pane = cur_pane as ObjectListView;

            util.postpone(() => {
                if (lv_pane != null)
                    lv_pane.set_focus();
                else if (list_pane != null) {
                    list_pane.Focus();
                    // maybe not such a good idea for notes pane???? TOTHINK
                    if (list_pane.SelectedIndex < 0 && list_pane.GetItemCount() > 0)
                        list_pane.SelectedIndex = 0;
                } else
                    cur_pane.Focus();
            }, 10);
        }

        private void on_activate() {
            if (active_pane_ != null) {
                if (active_pane_ is log_view)
                    active_pane_ = selected_view();
                if (!global_ui.show_current_view)
                    active_pane_ = full_log_ctrl_;
                activate_pane(active_pane_);
            } else
                activate_pane(panes()[0]);
        }


        private void settingsCtrl_Click(object sender, EventArgs e) {
            var old_sync_colors = app.inst.syncronize_colors;
            var old_sync_gray = app.inst.sync_colors_all_views_gray_non_active;
            var sett = new settings_form(this);
            sett.ShowDialog();
            notes.set_author(app.inst.notes_author_name, app.inst.notes_initials, app.inst.notes_color);

            bool sync_changed = app.inst.syncronize_colors != old_sync_colors || old_sync_gray != app.inst.sync_colors_all_views_gray_non_active;
            if (sync_changed)
                full_log.list.Refresh();

            filtCtrl.update_colors();

            if ( sett.wants_reset_settings)
                reset_settings();
            if ( sett.needs_restart)
                restart_app();
        }

        private void reset_settings() {
            foreach (var lw in log_wizard.forms) {
                lw.stop_saving();
                lw.Visible = false;
            }

            string dir = Program.local_dir();
            try {
                File.Copy(dir + "\\logwizard.txt", dir + "\\logwizard_user.txt", true);
            } catch {
            }
        }
        private void restart_app() {
            foreach (var lw in log_wizard.forms) {
                lw.stop_saving();
                lw.Visible = false;
            }
            util.restart_app();            
        }

        private log_view selected_view() {
            int sel = viewsTab.SelectedIndex;
            if (sel < 0)
                return null;
            if (is_focus_on_full_log())
                return full_log_ctrl_;

            var lv = log_view_for_tab(sel);
            return lv;
        }

        public string matched_logs(int line_idx) {
            List<string> matched = new List<string>();
            foreach (var lv in all_log_views())
                if (lv.filter_matches_line(line_idx))
                    matched.Add(lv.name);

            string selected = log_view_for_tab(viewsTab.SelectedIndex).name;
            bool removed = matched.Remove(selected);
            if (removed)
                matched.Insert(0, selected);

            string txt = "";
            foreach (string m in matched) {
                if (txt != "")
                    txt += ",";
                txt += m;
            }
            return txt;
        }


        private void load_bookmarks() {
            bookmarks_.Clear();
            string bookmarks_key = text_.name.Replace("=", "_").Replace("\\", "_");
            string[] bookmarks = sett.get("bookmarks." + bookmarks_key).Split(',');
            foreach (var b in bookmarks) {
                int line_idx;
                if (int.TryParse(b, out line_idx))
                    bookmarks_.Add(line_idx);
            }
            notify_views_of_bookmarks();
        }

        // notifies the views of our bookmarks
        private void notify_views_of_bookmarks() {
            foreach (var lv in all_log_views_and_full_log())
                // always send a copy of the list - this way, the views can see which bookmarks are new/deleted
                lv.set_bookmarks(bookmarks_.ToList());
        }

        private void save_bookmarks() {
            string str = bookmarks_.Aggregate("", (current, mark) => current + "," + mark);

            string bookmarks_key = text_.name.Replace("=", "_").Replace("\\", "_");
            sett.set("bookmarks." + bookmarks_key, str);
            sett.save();
        }

        private void about_Click(object sender, EventArgs e) {
            new about_form(this).Show();
        }

        private void log_wizard_Deactivate(object sender, EventArgs e) {
        }


        private void update_sync_texts() {
            synchronizedWithFullLog.Text = synchronizedWithFullLog.Checked ? "<-FL->" : "</FL/>";
            synchronizeWithExistingLogs.Text = synchronizeWithExistingLogs.Checked ? "<-V->" : "</V/>";
        }

        private void synchronizedWithFullLog_CheckedChanged(object sender, EventArgs e) {
            app.inst.sync_full_log_view = synchronizedWithFullLog.Checked;
            update_sync_texts();
            app.inst.save();
        }

        private void synchronizeWithExistingLogs_CheckedChanged(object sender, EventArgs e) {
            app.inst.sync_all_views = synchronizeWithExistingLogs.Checked;
            update_sync_texts();
            app.inst.save();
        }

        private string ui_context_to_str(ui_context cur) {
            if (cur.views.Count < 1)
                return ""; // no views
            var formatter = new XmlSerializer(typeof (ui_context));
            string str = "";
            using (var stream = new MemoryStream()) {
                formatter.Serialize(stream, cur);
                stream.Flush();
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                    str = reader.ReadToEnd();
            }
            return str;
        }

        private ui_context str_to_ui_context(string txt) {
            try {
                var formatter = new XmlSerializer(typeof (ui_context));
                using (var stream = new MemoryStream()) {
                    using (var writer = new StreamWriter(stream)) {
                        writer.Write(txt);
                        writer.Flush();
                        stream.Position = 0;
                        using (var reader = new StreamReader(stream)) {
                            var new_ctx = (ui_context) formatter.Deserialize(reader);
                            // we don't care about the name, just the filters
                            foreach (var view in new_ctx.views)
                                view.filters.ForEach(f => f.text = util.normalize_deserialized_enters(f.text));
                            return new_ctx;
                        }
                    }
                }
            } catch (Exception e) {
                logger.Error("can't convert to UI-context " + e.Message);
            }
            return null;
        }

        private void contextToClipboard_Click(object sender, EventArgs e) {
            string to_copy = ui_context_to_str(cur_context());
            if (to_copy != "")
                Clipboard.SetText(to_copy);
        }

        private void contextFromClipboard_Click(object sender, EventArgs ea) {
            try {
                string txt = Clipboard.GetText();
                ui_context cur = cur_context();

                var formatter = new XmlSerializer(typeof (ui_context));
                using (var stream = new MemoryStream()) {
                    using (var writer = new StreamWriter(stream)) {
                        writer.Write(txt);
                        writer.Flush();
                        stream.Position = 0;
                        using (var reader = new StreamReader(stream)) {
                            var new_ctx = (ui_context) formatter.Deserialize(reader);
                            // we don't care about the name, just the filters
                            foreach (var view in new_ctx.views)
                                view.filters.ForEach(f => f.text = util.normalize_deserialized_enters(f.text));
                            // ... preserve existing context name
                            string ctx_name = cur.name;
                            cur.copy_from(new_ctx);
                            cur.name = ctx_name;
                        }
                    }
                }
                curContextCtrl_SelectedIndexChanged(null, null);
            } catch (Exception e) {
                logger.Error("can't copy from clipboard: " + e.Message);
                util.beep(util.beep_type.err);
            }
        }


        // sets the status for a given period - after that ends, the previous status is shown
        // if < 0, it's forever
        private void set_status(string msg, status_type type = status_type.msg, int set_status_for_ms = 7500) {
            if (set_status_for_ms <= 0)
                statuses_.Clear();

            if (type == status_type.err)
                // show errors longer
                set_status_for_ms = Math.Max(set_status_for_ms, 15000);
            statuses_.Add(new Tuple<string, status_type, DateTime>(msg, type, set_status_for_ms > 0 ? DateTime.Now.AddMilliseconds(set_status_for_ms) : DateTime.MaxValue));
            status.Text = status_prefix_ + msg;
            status.ForeColor = status_color(type);
            status.BackColor = status_bg_color(type);
            if (type == status_type.err && !global_ui.show_status) {
                global_ui.temporarily_show_status = true;
                show_status(global_ui.temporarily_show_status);
            }

            if (type == status_type.err)
                util.beep(util.beep_type.err);
        }

        private Color status_color(status_type type) {
            return type == status_type.err ? Color.DarkRed : Color.Black;
        }

        private Color status_bg_color(status_type type) {
            return type == status_type.err ? Color.Yellow : Color.White;
        }

        private void set_status_forever(string msg) {
            set_status(msg, status_type.msg, -1);
        }

        private void update_status_text(bool force = false) {
            bool needs_update = false;
            while (statuses_.Count > 0 && statuses_.Last().Item3 < DateTime.Now) {
                statuses_.RemoveAt(statuses_.Count - 1);
                needs_update = true;
            }

            if (needs_update || force) {
                status.Text = statuses_.Count > 0 ? status_prefix_ + statuses_.Last().Item1 : "";
                if (statuses_.Count > 0) {
                    status.ForeColor = status_color(statuses_.Last().Item2);
                    status.BackColor = status_bg_color(statuses_.Last().Item2);
                }

                bool is_err = statuses_.Count > 0 && statuses_.Last().Item2 == status_type.err;
                if (!is_err)
                    if (global_ui.temporarily_show_status && !global_ui.show_status) {
                        global_ui.temporarily_show_status = false;
                        show_status(false);
                    }
            }
        }

        private void force_udpate_status_text() {
            update_status_text(true);
        }

        private static string tn2_file() {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TableNinja.v2\\TableNinja2.log";
        }

        private static string hm2_file() {
            // FIXME I think this is not the right file
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HoldemManager\\hm2.log";
        }

        private static string hm3_file() {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Max Value Software\\Holdem Manager\\3.0\\Logs\\holdemmanager3.log.txt";
        }

        private void monitor_Click(object sender, EventArgs e) {
            List<MenuItem> items = new List<MenuItem>();
            if (File.Exists(tn2_file()))
                items.Add(new MenuItem("TableNinja II", (o, args) => on_file_drop(tn2_file())));
            if (File.Exists(hm2_file()))
                items.Add(new MenuItem("HM2", (o, args) => on_file_drop(hm2_file())));
            if (File.Exists(hm3_file()))
                items.Add(new MenuItem("HM3", (o, args) => on_file_drop(hm3_file())));

            monitor.ContextMenu = new ContextMenu(items.ToArray());
            monitor.ContextMenu.Show(monitor, monitor.PointToClient(Cursor.Position));
        }

        private void toggleTopmost_MouseClick(object sender, MouseEventArgs e) {
            bool is_right_click = (e.Button & MouseButtons.Right) == MouseButtons.Right;
            if (!is_right_click) {
                TopMost = !TopMost;
                global_ui.topmost = TopMost;
                update_topmost_image();
            } else 
                show_toggles_menu();
        }

        private void toggleTopmost_Click(object sender, EventArgs e) {
        }

        private void log_wizard_SizeChanged(object sender, EventArgs e) {
            update_msg_details(true);
            if (ignore_change_ > 0)
                return;

            // remember position - if Visible
            save_location();
            selected_view().refresh();
        }

        private void log_wizard_LocationChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;

            save_location();
        }

        private void save_location() {
            if (!Visible)
                return;
            if (WindowState == FormWindowState.Minimized)
                return;

            if (WindowState == FormWindowState.Maximized)
                global_ui.maximized = true;
            else {
                global_ui.width = Width;
                global_ui.height = Height;
                global_ui.left = Left;
                global_ui.top = Top;
                global_ui.maximized = false;
            }
            save();
        }

        private void log_wizard_Activated(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;
            on_activate();
        }


        private void hotkeys_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/jtorjo/logwizard/wiki/Hotkeys");
        }

        private void leftPane_SizeChanged(object sender, EventArgs e) {
            logger.Info("left pane = " + leftPane.Width + " x " + leftPane.Height + " [" + filtersTab.Width + " x " + filtersTab.Height + "]");
        }

        private void export_Click(object sender, EventArgs e) {
            //export_notes();
            exportMenu.Show(Cursor.Position);
        }

        // within our .logwizard file - easily identify our files
        private const string log_wizard_zip_file_prefix = "___logwizard___";

        private void export_notes_to_logwizard_file() {
            if (!(text_ is file_text_reader)) {
                Debug.Assert(false);
                return;
            }

            string dir = util.create_temp_dir(Program.local_dir());
            notes.save_to(dir + "\\notes.txt");
            string ctx_as_string = ui_context_to_str(cur_context());
            util.create_file(dir + "\\context.txt", cur_context().name + "\r\n" + ctx_as_string);

            string full_name = text_.name;
            string file_name = new FileInfo(full_name).Name;

            // if the user wants slow md5s, we need to include that as well
            if (app.inst.identify_notes_files == md5_log_keeper.md5_type.slow)
                md5_log_keeper.inst.get_md5_for_file(full_name, md5_log_keeper.md5_type.slow);

            var md5s = md5_log_keeper.inst.local_md5s_for_file(full_name);
            util.create_file(dir + "\\md5.txt", util.concatenate(md5s, "\r\n"));

            // here, we have all needed files
            string zip_dir = Program.local_dir() + "export";
            util.create_dir(zip_dir);
            string prefix = zip_dir + "\\" + file_name + "." + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-fff") + ".";

            Dictionary<string, string> files = new Dictionary<string, string>();
            files.Add(dir + "\\notes.txt", log_wizard_zip_file_prefix + "notes.txt");
            files.Add(dir + "\\context.txt", log_wizard_zip_file_prefix + "context.txt");
            files.Add(dir + "\\md5.txt", log_wizard_zip_file_prefix + "md5.txt");
            files.Add(full_name, file_name);
            zip_util.create_zip(prefix + "long.logwizard", files);
            files.Remove(full_name);
            zip_util.create_zip(prefix + "short.logwizard", files);

            util.del_dir(dir);
            util.open_in_explorer(prefix + "long.logwizard");
        }

        private void import_notes(string file) {
            try {
                import_notes_impl(file);
            } catch (Exception e) {
                logger.Fatal("could not import file " + file + " : " + e.Message);
                set_status("An internal error occured. Please contact the author.", status_type.err);
            }
        }


        private void import_notes_impl(string file) {
            var files = zip_util.enum_file_names_in_zip(file);
            bool valid = files.Contains(log_wizard_zip_file_prefix + "md5.txt") && files.Contains(log_wizard_zip_file_prefix + "context.txt") && files.Contains(log_wizard_zip_file_prefix + "notes.txt");
            if (!valid) {
                set_status("Invalid .LogWizard file: " + file, status_type.err);
                return;
            }
            string import_dir = Program.local_dir() + "import";
            util.create_dir(import_dir);

            string dir = util.create_temp_dir(Program.local_dir());
            // ... extract our files (md5 / context / notes)
            zip_util.try_extract_file_names_in_zip(file, dir, files.Where(x => x.StartsWith(log_wizard_zip_file_prefix)).ToDictionary(x => x, x => x));

            // see if I know what the file is, locally - if not, see if it's in the .logwizard file
            var md5 = File.ReadAllLines(dir + "\\" + log_wizard_zip_file_prefix + "md5.txt").Where(x => x.Trim() != "").ToArray();
            List<string> local_files = md5_log_keeper.inst.find_files_with_md5(md5);
            // if we have the local file, we'll just go to it
            bool found_local_file = local_files.Count == 1;

            string notes_file = import_dir + "\\notes.txt." + DateTime.Now.Ticks + ".txt";
            File.Copy(dir + "\\" + log_wizard_zip_file_prefix + "notes.txt", notes_file);

            if (!found_local_file) {
                // at this point, try to load the file from the .zip
                int log_file_count = files.Count(x => !x.StartsWith(log_wizard_zip_file_prefix));
                if (log_file_count > 1) {
                    set_status("Invalid .LogWizard file: " + file, status_type.err);
                    return;
                }
                if (log_file_count == 0) {
                    set_status("Please ask your colleague to send you the LONG .LogWizard File - so we can auto-import and show you the Log together with the notes.", status_type.err);
                    return;
                }
                // at this point - we know we have the log file as well
                string name = files.First(x => !x.StartsWith(log_wizard_zip_file_prefix));
                // ...make it unique
                string unique_name = name + "." + DateTime.Now.Ticks + ".txt";
                string friendly_name = name + " (Imported)";
                zip_util.try_extract_file_names_in_zip(file, import_dir, new Dictionary<string, string>() {{name, unique_name}});

                // creating the context
                var imported_context_lines = File.ReadAllLines(dir + "\\" + log_wizard_zip_file_prefix + "context.txt").ToList();
                // first line - context name ; the others -> the context itself
                Debug.Assert(imported_context_lines.Count > 1);
                string imported_context_name = imported_context_lines[0] + " (Imported)";
                imported_context_lines.RemoveAt(0);
                var imported_context = str_to_ui_context(util.concatenate(imported_context_lines, "\r\n"));

                var can_we_find_context = file_to_context(import_dir + "\\" + unique_name);
                if (can_we_find_context.name == "Default") {
                    // at this point - we have to force the imported context for this file
                    var exists = contexts_.Find(x => x.name == imported_context_name);
                    if (exists == null) {
                        // we don't yet have this context - create it
                        imported_context.name = imported_context_name;
                        contexts_.Add(imported_context);
                        update_contexts_combos_in_all_forms();
                    }
                    app.inst.forced_file_to_context.Add(import_dir + "\\" + unique_name, imported_context_name);
                    save();
                }

                post_merge_file_ = notes_file;
                on_file_drop(import_dir + "\\" + unique_name, friendly_name);
            } else {
                post_merge_file_ = notes_file;
                on_file_drop(local_files[0]);
            }

            util.del_dir(dir);
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == win32.WM_COPYDATA) {
                var st = (win32.COPYDATASTRUCT) Marshal.PtrToStructure(m.LParam, typeof (win32.COPYDATASTRUCT));
                string open = st.lpData;
                util.postpone(() => on_file_drop(open), 100);
            }

            base.WndProc(ref m);
        }


        private static bool file_contains_pattern(string file, IEnumerable<string> pattern) {
            foreach (string patt in pattern)
                if (file.Contains(patt))
                    return true;
            return false;
        }

        private List<Tuple<string, long>> in_zip(string file) {
            var matches = app.inst.look_into_zip_files;
            var in_zip = zip_util.enum_file_names_and_sizes_in_zip(file).Where(x => file_contains_pattern(x.Item1, matches)).ToList();
            in_zip.Sort((x, y) => {
                int m1 = util.matched_string_index(x.Item1, matches), m2 = util.matched_string_index(y.Item1, matches);
                if (m1 != m2)
                    // by extension
                    return m1 - m2;

                return string.CompareOrdinal(x.Item1, y.Item1);
            });
            return in_zip;
        }

        private void on_zip_drop(string file) {
            bool shift = win32.IsKeyPushedDown(Keys.ShiftKey);
            if (shift) {
                on_shift_zip_drop(file);
                return;
            }

            // in this case, just take the first file that matches
            var in_zip = this.in_zip(file);
            if (in_zip.Count < 1)
                return; // no files

            util.bring_to_top(this);
            on_zip_file_drop(file, in_zip[0].Item1);
            set_status("Taking the first file that matches the [" + app.inst.look_into_zip_files_str + "] pattern. A Shift-[drag-and-drop] will show you a list of files.");
        }

        private void on_shift_zip_drop(string file) {
            var in_zip = this.in_zip(file);
            if (in_zip.Count < 1)
                return; // no files
            if (in_zip.Count == 1) {
                on_zip_file_drop(file, in_zip[0].Item1);
                return;
            }

            util.bring_to_top(this);
            select_zip_file_form sel = new select_zip_file_form(file, in_zip);
            if (sel.ShowDialog() == DialogResult.OK)
                on_zip_file_drop(file, sel.selected_file);
        }

        private void on_zip_file_drop(string zip_file, string sub_file_name) {
            string zip_dir = Program.local_dir() + "zip";
            util.create_dir(zip_dir);
            Dictionary<string, string> single_zip = new Dictionary<string, string>();
            string unique = sub_file_name + "." + DateTime.Now.Ticks + ".txt";
            single_zip.Add(sub_file_name, unique);
            zip_util.try_extract_file_names_in_zip(zip_file, zip_dir, single_zip);
            var fi = new FileInfo(zip_file);
            string friendly_name = sub_file_name + " From ZIP - " + util.friendly_size(fi.Length) + " (" + zip_file + ")";
            on_file_drop(zip_dir + "\\" + unique, friendly_name);
        }

        private void exportLogNotestoLogWizardFileToolStripMenuItem_Click(object sender, EventArgs e) {
            export_notes_to_logwizard_file();
        }

        private void exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem_Click(object sender, EventArgs ea) {
            var lv = selected_view();
            try {
                string prefix = Program.local_dir() + "exported_views";
                util.create_dir(prefix);
                prefix += "\\View " + util.remove_disallowed_filename_chars(lv.name) + " from " + new FileInfo(selected_file_name()).Name + " (" + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + ")";

                var export = selected_view().export();

                File.WriteAllText(prefix + ".txt", export.to_text());
                File.WriteAllText(prefix + ".html", export.to_html());

                util.open_in_explorer(prefix + ".html");
            } catch (Exception e) {
                logger.Error("can't export notes to txt/html " + e.Message);
            }
        }

        private void exportNotestotxtAndhtmlFilesToolStripMenuItem_Click(object sender, EventArgs ea) {
            try {
                string prefix = Program.local_dir() + "exported_notes";
                util.create_dir(prefix);
                prefix += "\\Notes on " + new FileInfo(selected_file_name()).Name + " (" + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + ")";
                var export = notes.export_notes();
                File.WriteAllText(prefix + ".txt", export.to_text());
                File.WriteAllText(prefix + ".html", export.to_html());

                util.open_in_explorer(prefix + ".html");
            } catch (Exception e) {
                logger.Error("can't export notes to txt/html " + e.Message);
            }
        }


        private void update_toggles() {
            logger.Debug("update toggles, " + global_ui.show_source);
            currentViewToolStripMenuItem.Checked = global_ui.show_current_view;
            fullLogToolStripMenuItem.Checked = global_ui.show_fulllog;
            tableHeaderToolStripMenuItem.Checked = global_ui.show_header;
            tabsToolStripMenuItem.Checked = global_ui.show_tabs;
            titleToolStripMenuItem.Checked = global_ui.show_title;
            statusToolStripMenuItem.Checked = global_ui.show_status;
            filterPaneToolStripMenuItem.Checked = global_ui.show_filter;
            notesPaneToolStripMenuItem.Checked = global_ui.show_notes;
            sourcePanetopmostToolStripMenuItem.Checked = global_ui.show_source;
            topmostToolStripMenuItem.Checked = global_ui.topmost;
            detailsToolStripMenuItem.Checked = global_ui.show_details;
            // we don't have this pane yet
            detailsToolStripMenuItem.Enabled = false;

            var lv = selected_view();
            bool on_full_log = is_focus_on_full_log();
            toolStripExtraFilter.Enabled = !on_full_log && lv.filter_friendly_name != "";
            toolStripShowAllLines.Enabled = !on_full_log;
            toolStripExtraFilter.Checked = lv.filter_view;
            toolStripShowAllLines.Checked = lv.show_full_log;
            toolStripExtraFilter.Text = lv.filter_friendly_name != "" ? "Extra Filter: " + lv.filter_friendly_name : "(no filter)";
        }


        private void currentViewToolStripMenuItem_Click(object sender, EventArgs e) {
            show_full_log_type now = shown_full_log_now();
            show_full_log_type next = now;
            switch (now) {
            case show_full_log_type.both:
                next = show_full_log_type.just_full_log;
                break;
            case show_full_log_type.just_view:
                next = show_full_log_type.just_full_log;
                break;
            case show_full_log_type.just_full_log:
                next = show_full_log_type.both;
                break;
            default:
                Debug.Assert(false);
                break;
            }
            show_full_log(next);
        }

        private void fullLogToolStripMenuItem_Click(object sender, EventArgs e) {
            show_full_log_type now = shown_full_log_now();
            show_full_log_type next = now;
            switch (now) {
            case show_full_log_type.both:
                next = show_full_log_type.just_view;
                break;
            case show_full_log_type.just_view:
                next = show_full_log_type.both;
                break;
            case show_full_log_type.just_full_log:
                next = show_full_log_type.just_view;
                break;
            default:
                Debug.Assert(false);
                break;
            }
            show_full_log(next);
        }

        private void tableHeaderToolStripMenuItem_Click(object sender, EventArgs e) {
            toggle_view_header();
        }

        private void tabsToolStripMenuItem_Click(object sender, EventArgs e) {
            toggle_view_tabs();
        }

        private void titleToolStripMenuItem_Click(object sender, EventArgs e) {
            toggle_title();
        }

        private void filterPaneToolStripMenuItem_Click(object sender, EventArgs e) {
            global_ui.show_filter = !global_ui.show_filter;
            show_left_pane(global_ui.show_left_pane);
        }

        private void notesPaneToolStripMenuItem_Click(object sender, EventArgs e) {
            global_ui.show_notes = !global_ui.show_notes;
            show_left_pane(global_ui.show_left_pane);
        }

        private void sourcePanetopmostToolStripMenuItem_Click(object sender, EventArgs e) {
            global_ui.show_source = !global_ui.show_source;
            show_source(global_ui.show_source);
        }

        private void statusToolStripMenuItem_Click(object sender, EventArgs e) {
            toggle_status();
        }

        private void topmostToolStripMenuItem_Click(object sender, EventArgs e) {
            TopMost = !TopMost;
            global_ui.topmost = TopMost;
            update_topmost_image();
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e) {
            toggle_details();
        }

        private void toggles_Click(object sender, EventArgs e) {
            show_toggles_menu();
        }

        private void show_toggles_menu() {
            update_toggles();
            toggleMenu.Closing += Toggle_menu_on_closing;            
            toggleMenu.Show(Cursor.Position, util.menu_direction(toggleMenu, Cursor.Position));
        }

        private void Toggle_menu_on_closing(object sender, ToolStripDropDownClosingEventArgs e) {
            e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked;
            if ( e.Cancel)
                util.postpone(update_toggles,1);
        }

        private void whatIsThisToolStripMenuItem_Click(object sender, EventArgs e) {
            Process.Start("https://github.com/jtorjo/logwizard/wiki/Toggles");
        }

        private void toolStripShowAllLines_Click(object sender, EventArgs e) {
            handle_action(action_type.toggle_show_full_log);
        }

        private void toolStripExtraFilter_Click(object sender, EventArgs e) {
            handle_action(action_type.toggle_filter_view);
        }

        // if copy_of_view is not null, we're creating a copy of that view
        private string new_view_name(ui_view copy_of_view = null) {
            if (copy_of_view != null)
                return copy_of_view.name + "_Copy";

            // I want to visually show to the user that we're dealing with views
            ui_context cur = cur_context();
            string name = name = "View_" + (cur.views.Count + 1);
            if (history_[logHistory.SelectedIndex].is_file) {
                name = Path.GetFileNameWithoutExtension(new FileInfo(selected_file_name()).Name) + "_View" + (cur.views.Count + 1);
            }

            return name;
        }

        private void createACopyOfTheExistingViewToolStripMenuItem_Click(object sender, EventArgs e) {
            ui_context cur = cur_context();
            int cur_view = viewsTab.SelectedIndex;
            var filters = cur_view >= 0 ? cur.views[cur_view].filters : new List<ui_filter>();

            ui_view new_ = new ui_view() {name = new_view_name(cur.views[cur_view]), filters = filters.ToList()};
            cur.views.Insert(cur_view + 1, new_);

            viewsTab.TabPages.Insert(cur_view + 1, new_.name);
            viewsTab.SelectedIndex = cur_view + 1;
            ensure_we_have_log_view_for_tab(cur_view + 1);
            save();
        }

        private void createANewViewFromScratchToolStripMenuItem_Click(object sender, EventArgs e) {
            ui_context cur = cur_context();
            int cur_view = viewsTab.SelectedIndex;
            ui_view new_ = new ui_view() {name = new_view_name(), filters = new List<ui_filter>()};
            cur.views.Insert(cur_view + 1, new_);

            viewsTab.TabPages.Insert(cur_view + 1, new_.name);
            viewsTab.SelectedIndex = cur_view + 1;
            ensure_we_have_log_view_for_tab(cur_view + 1);
            save();
        }

        private void filteredLeft_SplitterMoved(object sender, SplitterEventArgs e) {
            update_msg_details(true);
            if (ignore_change_ > 0)
                return;
            //logger.Debug("[splitter] filteredleft=" + filteredLeft.SplitterDistance  );
            if (filteredLeft.SplitterDistance >= 0) {
                global_ui.full_log_splitter_pos = filteredLeft.SplitterDistance;
                save();
            } else
                Debug.Assert(false);
        }

        private void main_SplitterMoved(object sender, SplitterEventArgs e) {
            update_msg_details(true);
            if (ignore_change_ > 0)
                return;
            //logger.Debug("[splitter] main=" + main.SplitterDistance  );
            if (main.SplitterDistance >= 0) {
                global_ui.left_pane_pos = main.SplitterDistance;
                save();
            } else
                Debug.Assert(false);
        }

        private void helpSyntax_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/jtorjo/logwizard/wiki/Syntax");
        }

        private void testSyntax_Click(object sender, EventArgs e) {
            string file = selected_file_name();

            string guess = "";
            try {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    // read a few lines from the beginning
                    byte[] readBuffer = new byte[find_log_syntax.READ_TO_GUESS_SYNTAX];
                    int bytes = fs.Read(readBuffer, 0, find_log_syntax.READ_TO_GUESS_SYNTAX);
                    var encoding = util.file_encoding(file);
                    if (encoding == null)
                        encoding = Encoding.Default;
                    guess = encoding.GetString(readBuffer, 0, bytes);
                }
            } catch {
            }

            // 1.3.24+ - use the old syntax when we're modifying
            var test = new test_syntax_form(guess, logSyntaxCtrl.Text);
            if (test.ShowDialog() == DialogResult.OK) {
                // use the syntax
                cur_context().default_settings = test.found_syntax;
                save();

                // force complete refresh
                text_.Dispose();
                text_ = null;
                remove_all_log_views();
                load();
                on_file_drop(selected_file_name());
                // we want to refresh it only after it's been loaded, so that it visually shows that
                util.postpone(() => full_log_ctrl_.force_refresh_visible_columns( all_log_views() ), 2000);
            }
        }


    }
}
