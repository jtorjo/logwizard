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
using LogWizard.context;
using LogWizard.Properties;
using LogWizard.ui;

namespace LogWizard
{
    partial class log_wizard : Form
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static List<log_wizard> forms_ = new List<log_wizard>();

        public enum action_type {
            search, search_prev, search_next, clear_search,

            next_view, prev_view,
            home, end,
            pageup, pagedown,
            arrow_up, arrow_down,
            toggle_filters, toggle_source, toggle_fulllog, 

            toggle_bookmark, next_bookmark, prev_bookmark, clear_bookmarks,

            // 1.0.25+
            copy_to_clipboard, copy_full_line_to_clipboard,

            // 1.0.28+
            pane_next, pane_prev,

            // 1.0.28+
            toggle_history_dropdown,
            new_log_wizard, show_preferences,

            // 1.0.35+
            increase_font, decrease_font,
            toggle_show_msg_only,
            // ... equivalent of ctrl-up/down
            scroll_up, scroll_down,

            // 1.0.52+
            go_to_line,

            // 1.0.53
            refresh,

            // 1.0.56
            toggle_title,

            // 1.0.77+ - (show/hide the tabs themselves) ; show the tab name in the "Message" column itself
            toggle_view_tabs,

            // 1.0.70+ - toggle the header (show/hide)
            toggle_view_header,

            // 1.0.77 - show/hide details view - note: I've not implemented "details" view yet
            toggle_details,

            toggle_status,

            // 1.0.67
            open_in_explorer,

            // 1.0.77+ default behavior does not work correctly
            shift_arrow_up, shift_arrow_down,

            // 1.0.80+ - allow saving the current position + toggles + current view
            save_position_1, 
            save_position_2, 
            save_position_3, 
            save_position_4, 
            save_position_5,

            // 1.0.80+ - switch to a saved position and back
            toggle_position_1, 
            toggle_position_2, 
            toggle_position_3, 
            toggle_position_4, 
            toggle_position_5,
 

            none,
        }

        private class history {
            public enum entry_type {
                file = 0, shmem = 1
            }

            // 0->file, 1->shmem
            public entry_type type;
            public string name = "";
            public string friendly_name = "";
            public string log_syntax  = "";

            public string combo_name { get {
                if ( friendly_name != "") return friendly_name;
                switch ( type) {
                    case entry_type.file: return name; // the name of the type
                    case entry_type.shmem: return "Shared Memory: " + name;
                    default: Debug.Assert(false); break;
                }
                return "invalid";
            }}
        }
        
        private settings_file sett = Program.sett;

        private static List<ui_context> contexts_ = new List<ui_context>();
        private static List<history> history_ = new List<history>();

        private text_reader text_ = null;
        private log_line_parser log_parser_ = null;
        private log_view full_log_ctrl = null;

        private int old_line_count_ = 0;

        const int MAX_HISTORY_ENTRIES = 100;

        private int ignore_change_ = 0;

        // while the user is updating the text, don't update the filter right away
        private const int IGNORE_FILTER_UDPATE_MS = 1000;
        private DateTime last_filter_text_change_ = DateTime.MinValue;

        // in case we're searching for some text in the current tab, this is non-null
        private search_form.search_for search_for_text_ = null;

        private List<int> bookmarks_ = new List<int>();
        private msg_details_ctrl msg_details_ = null;

        private Control pane_to_focus_ = null;

        private action_type last_action_ = action_type.none;

        // the status(es) to be shown
        private List< Tuple<string,DateTime>> statuses_ = new List<Tuple<string, DateTime>>();
        // what to be shown behind ALL statuses
        private string status_prefix_ = "";

        private int toggled_to_custom_ui_ = -1;
        private ui_info default_ui_ = new ui_info();
        private ui_info[] custom_ui_ = new ui_info[] { new ui_info(), new ui_info(), new ui_info(), new ui_info(), new ui_info() };

        private enum show_full_log_type {
            both, just_view, just_full_log
        }

        class filter_item {
            public bool enabled = true;
            public bool dimmed = false;
            public string text = "";

            public bool apply_to_existing_lines = false;

            public string found_count = "";

            // "name" is just a friendly name for the text
            public string name {
                get {
                    string[] lines = text.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                    var filter_name = lines.FirstOrDefault(l => l.Trim().StartsWith("## "));
                    if (filter_name != null)
                        return filter_name.Trim().Substring(3).Trim();

                    return lines.Aggregate("", (current, l) => current + (l + " | "));
                }
            } 
        }

        public log_wizard()
        {
            InitializeComponent();
            forms_.Add(this);
            Text += " " + version();
            sourceTypeCtrl.SelectedIndex = 0;
            bool first_time = contexts_.Count == 0;
            if (first_time) {
                app.inst.load();
                load_contexts();
            }

            ++ignore_change_;

            foreach ( ui_context ctx in contexts_)
                curContextCtrl.Items.Add(ctx.name);
            // just select something
            curContextCtrl.SelectedIndex = 0;

            foreach ( history hist in history_)
                logHistory.Items.Add(hist.combo_name);

            --ignore_change_;
            load();
            ++ignore_change_;

            full_log_ctrl = new log_view( this, "[All]");
            full_log_ctrl.Dock = DockStyle.Fill;
            filteredLeft.Panel2.Controls.Add(full_log_ctrl);
            full_log_ctrl.show_name = false;
            full_log_ctrl.show_view(true);

            set_tabs_visible(leftPane, false);

            msg_details_ = new msg_details_ctrl(this);
            Controls.Add(msg_details_);
            handle_subcontrol_keys(this);

            viewsTab.DrawMode = TabDrawMode.OwnerDrawFixed;
            viewsTab.DrawItem += ViewsTabOnDrawItem;

            update_topmost_image();
            update_toggle_topmost_visibility();

            load_location();
            --ignore_change_;

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

        private void load_location() {
            int left = int.Parse(sett.get("location.left", "-1"));
            int top = int.Parse(sett.get("location.top", "-1"));

            int width = int.Parse(sett.get("location.width", "-1"));
            int height = int.Parse(sett.get("location.height", "-1"));

            if ( left != -1)
                Location = new Point(left, top);

            if ( width > 0)
                Size = new Size(width, height);
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

        private ui_info cur_ui {
            get {
                Debug.Assert( toggled_to_custom_ui_ >= -1 && toggled_to_custom_ui_ < 5);
                return toggled_to_custom_ui_ < 0 ? default_ui_ : custom_ui_[toggled_to_custom_ui_];
            }
        }

        // returns true if the tabs are visible
        private bool are_tabs_visible(TabControl tab) {
            return tab.Top >= 0;
        }
        private void set_tabs_visible(TabControl tab, bool show) {
            int extra = tab.Height - tab.TabPages[0].Height;
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

        private void load_contexts() {
            logger.Debug("loading contexts");

            int history_count = int.Parse( sett.get("history_count", "0"));
            for (int idx = 0; idx < history_count; ++idx) {
                history hist = new history();
                hist.type = (history.entry_type) int.Parse( sett.get("history." + idx + "type", "0"));
                hist.name = sett.get("history." + idx + "name");
                hist.friendly_name = sett.get("history." + idx + "friendly_name");
                hist.log_syntax = sett.get("history." + idx + "log_syntax");
                history_.Add( hist );
            }
            history_ = history_.Where(h => {
                if (h.type == history.entry_type.file)
                    if( !File.Exists(h.name))
                        return false;
                return true;
            }).ToList();

            int count = int.Parse( sett.get("context_count", "1"));
            for ( int i = 0; i < count ; ++i) {
                ui_context ctx = new ui_context();
                ctx.name = sett.get("context." + i + ".name", "Default");
                ctx.auto_match = sett.get("context." + i + ".auto_match");

                ctx.ui.load("context." + i);

                int view_count = int.Parse( sett.get("context." + i + ".view_count", "1"));
                for ( int v = 0; v < view_count; ++v) {
                    ui_view lv = new ui_view();
                    lv.name = sett.get("context." + i + ".view" + v + ".name");
                    int filter_count = int.Parse( sett.get("context." + i  + ".view" + v + ".filter_count", "0"));
                    for ( int f = 0; f < filter_count; ++f) {
                        string prefix = "context." + i + ".view" + v + ".filt" + f + ".";
                        bool enabled = int.Parse( sett.get(prefix + "enabled", "1")) != 0;
                        bool dimmed = int.Parse( sett.get(prefix + "dimmed", "0")) != 0;
                        string text = sett.get(prefix + "text");
                        bool apply_to_existing_lines = int.Parse( sett.get(prefix + "apply_to_existing_lines", "0")) != 0;
                        lv.filters.Add( new ui_filter { enabled = enabled, dimmed = dimmed, text = text, apply_to_existing_lines = apply_to_existing_lines } );
                    }
                    ctx.views.Add(lv);
                }
                contexts_.Add(ctx);
            }
        }

        private void save_contexts() {
            sett.set( "history_count", "" + history_.Count);
            for ( int idx = 0; idx < history_.Count; ++idx) {
                sett.set("history." + idx + "type", "" + (int)history_[idx].type);
                sett.set("history." + idx + "name", history_[idx].name);
                sett.set("history." + idx + "friendly_name", history_[idx].friendly_name);
                sett.set("history." + idx + "log_syntax", history_[idx].log_syntax);
            }

            sett.set("context_count", "" + contexts_.Count);
            for ( int i = 0; i < contexts_.Count; ++i) {
                sett.set("context." + i + ".name", contexts_[i].name);
                sett.set("context." + i + ".auto_match", contexts_[i].auto_match);

                if (toggled_to_custom_ui_ < 0) 
                    contexts_[i].ui.save("context." + i);

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
            }
        }

        private bool filters_shown { get { return toggleFilters.Text[0] == '-'; }}
        private bool source_shown { get { return toggleSource.Text[0] == '-'; }}

        private void show_filters(bool show) {
            toggleFilters.Text = show ? "-F" : "+F";
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
            cur_context().ui.show_filter = show;
        }
        private void show_source(bool show) {
            toggleSource.Text = show ? "-S" : "+S";
            if ( show) {
                sourceUp.Panel1Collapsed = false;
                sourceUp.Panel1.Show();
            }
            else {
                sourceUp.Panel1Collapsed = true;
                sourceUp.Panel1.Hide();
            }
            if ( curContextCtrl.SelectedIndex >= 0)
                for ( int i = 0; i < cur_context().views.Count; ++i) {
                    log_view lv = log_view_for_tab(i);
                    if ( lv != null)
                        lv.show_name = show;
                }
            cur_context().ui.show_source = show;
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
                cur_context().ui.show_fulllog = true;
                cur_context().ui.show_current_view = true;
                to_focus = full_log_ctrl;
                break;
            case show_full_log_type.just_view:
                show_filteredleft_pane1(true);
                show_filteredleft_pane2(false);
                cur_context().ui.show_fulllog = false;
                cur_context().ui.show_current_view = true;
                to_focus = ensure_we_have_log_view_for_tab(viewsTab.SelectedIndex);
                break;
            case show_full_log_type.just_full_log:
                show_filteredleft_pane1(false);
                show_filteredleft_pane2(true);
                cur_context().ui.show_fulllog = true;
                cur_context().ui.show_current_view = false;
                to_focus = full_log_ctrl;
                break;
            default:
                Debug.Assert(false);
                break;
            }
            toggleFullLog.Text = show_full_log_type_to_str(show);

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

        private show_full_log_type str_to_show_full_log_type(string s) {
            return s == "+L"
                ? show_full_log_type.just_view
                : s == "-L" ? show_full_log_type.both : show_full_log_type.just_full_log;            
        }

        private string show_full_log_type_to_str(show_full_log_type type) {
            switch (type) {
            case show_full_log_type.both:
                return "-L";
            case show_full_log_type.just_view:
                return "+L";
            case show_full_log_type.just_full_log:
                return "L";
            default:
                Debug.Assert(false);
                return "-L";
            }
        }
        private void toggle_full_log() {
            show_full_log_type now = str_to_show_full_log_type(toggleFullLog.Text);
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
            string from_header = log_to_default_context.file_to_context(name);
            if (from_header != null) {
                var context_from_header = contexts_.FirstOrDefault(x => x.name == from_header);
                if (context_from_header != null)
                    // return it, only if we have a specific Template for it
                    return context_from_header;
            }

            string file_name_no_dir = new FileInfo(name).Name;
            var found = contexts_.FirstOrDefault(x => file_name_no_dir.Contains( x.name));
            if (found != null)
                return found;

            var default_ = contexts_.FirstOrDefault(x => x.name == "Default");           
            return default_ ?? contexts_[0];
        }

        private void on_file_drop(string file) {
            ++ignore_change_;
            sourceNameCtrl.Text = file;
            sourceTypeCtrl.SelectedIndex = 0;
            friendlyNameCtrl.Text = "";
            logSyntaxCtrl.Text = "";
            --ignore_change_;
            // this will force us to process this change
            last_sel = -2;

            history_select(file);
            on_new_file_log(file);
            util.bring_to_top(this);
        }

        // to show/hide the "details" view - the details view contains all information in the message (that is not usually shown in the list)
        private void show_details(bool show) {
            // not implmented yet
        }

        private void toggle_details() {
            bool show = !cur_context().ui.show_details;
            show_details( show);
            cur_context().ui.show_details = show;
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
            cur_context().ui.show_status = !shown;
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
            cur_context().ui.show_title = !shown;
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
            cur_context().ui.show_tabs = visible_now;
            save();
        }

        private void show_header(bool show) {
            foreach (var lv in all_log_views_and_full_log())
                lv.show_header = show;
        }

        private void toggle_view_header() {
            bool show = !all_log_views()[0].show_header;
            show_header( show);
            cur_context().ui.show_header = show;
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

        private void toggleFilters_Click(object sender, EventArgs e)
        {
            show_filters( toggleFilters.Text[0] == '+');
            save();
            update_msg_details(true);
        }

        private void toggleSource_Click(object sender, EventArgs e)
        {
            show_source( toggleSource.Text[0] == '+');
            save();
            update_msg_details(true);
        }

        private void toggleFullLog_Click(object sender, EventArgs e)
        {
            toggle_full_log();
            save();
            update_msg_details(true);
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

        private void load_filters() {
            // filter_row Enabled / Used - are context dependent!

            ++ignore_change_;
            List<object> items = new List<object>();
            ui_context cur = cur_context();
            int cur_view = viewsTab.SelectedIndex;
            if (cur_view < cur.views.Count) {
                var filters = cur.views[cur_view].filters;
                for (int idx = 0; idx < filters.Count; ++idx) {
                    filter_item i = new filter_item { text = filters[idx].text, enabled = filters[idx].enabled, 
                                                      dimmed = filters[idx].dimmed, apply_to_existing_lines = filters[idx].apply_to_existing_lines };
                    items.Add(i);
                }
            }
            filterCtrl.SetObjects(items);

            curFilterCtrl.Text = "";
            applyToExistingLines.Checked = false;
            --ignore_change_;
            sync_full_log_colors();
        }

        private void sync_full_log_colors() {
            if (full_log_ctrl != null && cur_context().ui.show_fulllog ) 
                full_log_ctrl.update_colors(all_log_views(), viewsTab.SelectedIndex);
        }

        private void save_filters() {
            // filter text / Enabled / Used - are context dependent!
            ui_context cur = cur_context();

            // now, update enabled / dimmed
            int cur_view = viewsTab.SelectedIndex;

            cur.views[cur_view].filters.Clear();
            for ( int idx = 0; idx < filterCtrl.GetItemCount(); ++idx) {
                filter_item i = filterCtrl.GetItem(idx).RowObject as filter_item;
                cur.views[cur_view].filters.Add(  new ui_filter { enabled = i.enabled, dimmed = i.dimmed, text = i.text, apply_to_existing_lines = i.apply_to_existing_lines} );
            }
        }

        private log_view ensure_we_have_log_view_for_tab(int idx) {
            //if ( text_ == null)
              //  return;
            TabPage tab = viewsTab.TabPages[idx];
            foreach ( Control c in tab.Controls)
                if ( c is log_view)
                    return c as log_view; // we have it

            foreach ( Control c in tab.Controls)
                c.Visible = false;

            log_view new_ = new log_view( this, viewsTab.TabPages[idx].Text );
            new_.Dock = DockStyle.Fill;
            tab.Controls.Add(new_);
            new_.show_name = source_shown;
            new_.set_bookmarks(bookmarks_.ToList());
            if ( log_parser_ != null)
                new_.set_log( new log_line_reader(log_parser_));
            return new_;
        }

        private List<log_view> all_log_views() {
            List<log_view> other_logs = new List<log_view>();
            for (int idx = 0; idx < viewsTab.TabCount; ++idx) {
                var other = ensure_we_have_log_view_for_tab(idx);
                other_logs.Add(other);
            }
            return other_logs;
        }

        private List<log_view> all_log_views_and_full_log() {
            var all = all_log_views();
            if ( full_log_ctrl != null)
                all.Add(full_log_ctrl);
            return all;
        }

        public void on_view_name_changed(log_view view, string name) {
            ui_context cur = cur_context();
            for ( int i = 0; i < cur_context().views.Count; ++i)
                if ( log_view_for_tab(i) == view) {
                    viewsTab.TabPages[i].Text = name;
                    view.name = name;
                    cur.views[i].name = name;
                }
        }

        private log_view log_view_for_tab(int idx) {
            TabPage tab = viewsTab.TabPages[idx];
            foreach ( Control c in tab.Controls)
                if ( c is log_view)
                    return (log_view)c; // we have it
            return null;
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

        public static List<log_wizard> forms {
            get { return forms_; }
        }

        private void load_tabs() {
            // note: we only add the inner view when there's some source to read from
            ui_context cur = cur_context();

            // never allow "no view" whatsoever
            bool has_views = cur.views.Count > 0 || cur.name != "Default";
            if (cur.views.Count < 1) 
                cur.views.Add(new ui_view() { name = "New_1", filters = new List<ui_filter>() });

            for ( int idx = 0; idx < cur.views.Count; ++idx) 
                if ( viewsTab.TabCount < idx + 1) 
                    viewsTab.TabPages.Add(cur.views[idx].name);

            for ( int idx = 0; idx < cur.views.Count; ++idx) {
                viewsTab.TabPages[idx].Text = cur.views[idx].name;
                ensure_we_have_log_view_for_tab(idx);
            }

            while (viewsTab.TabCount > cur.views.Count) {
                // TabControl.RemoveAt is buggy
                var page = viewsTab.TabPages[cur.views.Count];
                viewsTab.TabPages.Remove(page);
            }

            if (!has_views) {
                log_view_for_tab(0).Visible = false;
                dropHere.Visible = true;
            }
        }

        private void load_ui() {
            var cur = cur_context();
            if (!cur.has_views)
                return;

            show_filters(cur.ui.show_filter);
            show_source(cur.ui.show_source);

            if ( cur.ui.show_fulllog && cur.ui.show_current_view)
                show_full_log(show_full_log_type.both);
            else
                show_full_log(cur.ui.show_current_view ? show_full_log_type.just_view : show_full_log_type.just_full_log);

            show_header(cur.ui.show_header);
            show_title(cur.ui.show_title);
            show_details(cur.ui.show_details);
            show_status(cur.ui.show_status);

            ++ignore_change_;
            if (toggled_to_custom_ui_ < 0 && cur_ui.width > 0) {
                Left = cur_ui.left;
                Top = cur_ui.top;
                Width = cur_ui.width;
                Height = cur_ui.height;
                WindowState = cur_ui.maximized ? FormWindowState.Maximized : FormWindowState.Normal;
            }

            if ( cur.ui.full_log_splitter_pos >= 0)
                filteredLeft.SplitterDistance = cur.ui.full_log_splitter_pos;
            --ignore_change_;

            util.postpone( () => show_tabs(cur.ui.show_tabs), 100);
            /* not tested
            if (cur.topmost) {
                util.bring_to_topmost(this);
                update_topmost_image();
                update_toggle_topmost_visibility();                
            }*/
        }

        private void load_global_settings() {
            synchronizeWithExistingLogs.Checked = app.inst.sync_all_views;
            synchronizedWithFullLog.Checked = app.inst.sync_full_log_view;
            update_sync_texts();

            default_ui_.load("ui.default");
            for (int i = 0; i < custom_ui_.Length; ++i)
                custom_ui_[i].load("ui.custom" + i);
        }

        private void remove_log_view_from_tab(int idx) {
            Debug.Assert(idx < viewsTab.TabCount);
            TabPage tab = viewsTab.TabPages[idx];
            log_view lv = log_view_for_tab(idx);
            if ( lv != null)
                tab.Controls.Remove(lv);
        }

        private void remove_all_log_views() {
            for ( int idx = 0; idx < viewsTab.TabCount; ++idx)
                remove_log_view_from_tab(idx);
        }

        private void newFilteredView_Click(object sender, EventArgs e)
        {
            ui_context cur = cur_context();
            int cur_view = viewsTab.SelectedIndex;
            var filters = cur_view >= 0 ? cur.views[cur_view].filters : new List<ui_filter>();

            ui_view new_ = new ui_view() { name = "New_" + cur.views.Count, filters = filters.ToList() };
            cur.views.Insert(cur_view + 1, new_);

            viewsTab.TabPages.Insert(cur_view + 1, new_.name);
            viewsTab.SelectedIndex = cur_view + 1;
            ensure_we_have_log_view_for_tab( cur_view + 1);
            save();
        }

        private void delFilteredView_Click(object sender, EventArgs e)
        {
            int idx = viewsTab.SelectedIndex;
            if ( idx < 0)
                return;

            ui_context cur = cur_context();
            if (cur.views.Count > 1) {
                cur.views.RemoveAt(idx);
                // 1.0.51+ - yeah - RemoveAt() has a bug and quite often removes a different tab
                //viewsTab.TabPages.RemoveAt(idx);
                var page = viewsTab.TabPages[idx];
                viewsTab.TabPages.Remove(page);
            } else {
                // it's the last tab, clear the filter
                cur.views[0].name = "New_1";
                cur.views[0].filters = new List<ui_filter>();
                on_view_name_changed( ensure_we_have_log_view_for_tab(0) , cur.views[0].name);
                load_filters();
                save();
            }
        }

        private void select_filter(string name) {
            for ( int idx = 0; idx < filterCtrl.GetItemCount(); ++idx) {
                filter_item i = (filter_item)filterCtrl.GetItem(idx).RowObject;
                if ( i.name == name)
                    filterCtrl.SelectedIndex = idx;
            }
        }

        private void load() {
            load_tabs();
            load_filters();
            load_global_settings();
            load_ui();
        }

        public void stop_saving() {
            ++ignore_change_;
        }

        private void save() {
            if (ignore_change_ > 0)
                return;

            ui_context cur = cur_context();
            //cur.auto_match = contextMatch.Text;

            save_filters();
            save_contexts();

            default_ui_.save("ui.default");
            for (int i = 0; i < custom_ui_.Length; ++i)
                custom_ui_[i].save("ui.custom" + i);

            if ( !Program.sett.dirty)
                // no change
                return;

            Program.sett.save();
            foreach ( log_wizard lw in forms_)
                if ( lw != this)
                    lw.load();
        }



        private void addFilter_Click(object sender, EventArgs e)
        {
            filter_item new_ = new filter_item { enabled = true, dimmed = false, text = "", apply_to_existing_lines = false};

            ui_context cur = cur_context();
            cur.views[ viewsTab.SelectedIndex ].filters.Add( new ui_filter() );

            filterCtrl.AddObject(new_);
            filterCtrl.SelectObject(new_);
            curFilterCtrl.Text = "";

            save();

            focusOnFilterCtrl.Enabled = true;
        }

        private void delFilter_Click(object sender, EventArgs e)
        {
            ui_context cur = cur_context();
            var sel = filterCtrl.SelectedObject as filter_item;
            if ( sel != null) {
                int idx = filterCtrl.SelectedIndex;
                var filters = cur.views[viewsTab.SelectedIndex].filters;
                filters.RemoveAt(idx);
                filterCtrl.RemoveObject(sel);

                int new_sel = filters.Count > idx ? idx : filters.Count > 0 ? filters.Count - 1 : -1;
                if (new_sel >= 0)
                    filterCtrl.SelectedIndex = new_sel;
            }
            save();
        }


        private void refresh_Tick(object sender, EventArgs e)
        {
            if ( curContextCtrl.DroppedDown)
                return;

            refresh_cur_log_view();
            update_status_text();
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
            log_view lv = log_view_for_tab(viewsTab.SelectedIndex);
            if (lv == null)
                return;

            if (app.inst.instant_refresh_all_views) {
                refresh_all_views();
            } else {
                // optimized - refresh only current view
                update_filter(lv);
                lv.refresh();
                if (cur_context().ui.show_fulllog) {
                    for (int idx = 0; idx < viewsTab.TabCount; ++idx) {
                        var other = ensure_we_have_log_view_for_tab(idx);
                        if (other != lv)
                            update_non_active_filter(idx);
                    }
                    full_log_ctrl.refresh();
                    full_log_ctrl.set_view_selected_view_name(lv.name);
                }

                update_msg_details(false);
                refresh_filter_found();
            }
            sync_full_log_colors();

            if (text_.has_it_been_rewritten)
                on_rewritten_log();
        }

        private void on_rewritten_log() {
            if (app.inst.bring_to_top_on_restart) {
                if (app.inst.make_topmost_on_restart) {
                    util.bring_to_topmost(this);
                    update_topmost_image();
                    update_toggle_topmost_visibility();
                }
                else
                    util.bring_to_top(this);
            }
        }

        private void update_msg_details(bool force_update) {
            if (selected_view() != null && msg_details_ != null) {
                int top_offset = 40;
                log_view any_lv = log_view_for_tab(0);
                if (any_lv != null) {
                    top_offset = any_lv.RectangleToScreen(any_lv.ClientRectangle).Top - RectangleToScreen(ClientRectangle).Top + 5;
                    if (cur_context().ui.show_header)
                        top_offset += any_lv.list.HeaderControl.ClientRectangle.Height;
                }
                int bottom_offset = ClientRectangle.Height - lower.Top;
                msg_details_.update(selected_view(), top_offset, bottom_offset, force_update);
            }
        }

        private void update_non_active_filter(int idx) {
            var view = ensure_we_have_log_view_for_tab(idx);
            if (view.is_filter_set)
                return;

            ui_context ctx = cur_context();
            List<filter_row> lvf = new List<filter_row>();
            foreach (ui_filter filt in ctx.views[idx].filters) {
                filter_row row = new filter_row(filt.text);
                row.enabled = filt.enabled;
                row.dimmed = filt.dimmed;
                row.apply_to_existing_lines = filt.apply_to_existing_lines;
                if ( row.is_valid)
                    lvf.Add(row);
            }
            view.set_filter( lvf);
        }

        private void refresh_filter_found() {
            log_view lv = log_view_for_tab(viewsTab.SelectedIndex);
            Debug.Assert(lv != null);
            if (old_line_count_ == lv.line_count)
                return;
            if (filterCtrl.GetItemCount() != lv.filter_row_count)
                // we can get here if one of the filter rows' text is invalid
                return;

            // recompute line count
            for (int i = 0; i < filterCtrl.GetItemCount(); ++i) {
                int count = lv.filter_row_match_count(i);
                (filterCtrl.GetItem(i).RowObject as filter_item).found_count = count > 0 ? "" + count : "";
                filterCtrl.RefreshItem( filterCtrl.GetItem(i) );
            }

            old_line_count_ = lv.line_count;
        }

        private void update_filter( log_view lv) {
            if (last_filter_text_change_.AddMilliseconds(IGNORE_FILTER_UDPATE_MS) > DateTime.Now)
                return;

            List<filter_row> lvf = new List<filter_row>();
            int count = filterCtrl.GetItemCount();
            for ( int idx = 0; idx < count; ++idx) {
                filter_item i = filterCtrl.GetItem(idx).RowObject as filter_item;
                filter_row filt = new filter_row(i.text);
                filt.enabled = i.enabled;
                filt.dimmed = i.dimmed;
                filt.apply_to_existing_lines = i.apply_to_existing_lines;

                if ( filt.is_valid)
                    lvf.Add(filt);
            }

            lv.set_filter(lvf);
        }

        private void on_move_key_end() {
            var sel = selected_view();
            if (sel == full_log_ctrl)
                return;

            go_to_line(sel.sel_line_idx, sel);
        }

        public void go_to_line(int line_idx, log_view from) {
            if (cur_context().ui.show_fulllog && from != full_log_ctrl && app.inst.sync_full_log_view) {
                full_log_ctrl.go_to_line(line_idx, log_view.select_type.do_not_notify_parent);
                sync_full_log_colors();
            }

            bool keep_all_in_sync = (from != full_log_ctrl && app.inst.sync_all_views) ||
                // if the current log is full log, we will synchronize all views only if both checks are checked
                // (note: this is always a bit time consuming as well)
                (from == full_log_ctrl&& app.inst.sync_all_views && app.inst.sync_full_log_view);
            if ( keep_all_in_sync)
                keep_logs_in_sync(from);
        }

        private void sourceName_TextChanged(object sender, EventArgs e)
        {
            if (ignore_change_ > 0)
                return;
            if ( logHistory.DroppedDown)
                // user is going through the history, hasn't made up his mind yet
                return;

            if ( sourceTypeCtrl.SelectedIndex == 0 && File.Exists(sourceNameCtrl.Text)) 
                on_new_file_log(sourceNameCtrl.Text);            
            else if ( sourceTypeCtrl.SelectedIndex == 1) 
                on_new_shared_log(sourceNameCtrl.Text);            
        }

        private void sourceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( sourceTypeCtrl.SelectedIndex == 2) 
                on_new_debug_log();
        }


        private void on_new_file_log(string name) {
            if (text_ != null && text_.name == name)
                return;
            
            if ( text_ != null)
                text_.Dispose();

            text_ = new file_text_reader(name);
            on_new_log();

            ui_context file_ctx = file_to_context(name);
            if (file_ctx != cur_context())
                // update context based on file name
                curContextCtrl.SelectedIndex = contexts_.IndexOf(file_ctx);

            force_initial_refresh_of_all_views();
        }

        private void refresh_all_views() {
            if (ignore_change_ > 0)
                return;
            if (text_ == null)
                // no log yet
                return;
            log_view lv = log_view_for_tab(viewsTab.SelectedIndex);
            if (lv == null)
                return;

            update_filter(lv);
            lv.refresh();

            for (int idx = 0; idx < viewsTab.TabCount; ++idx) {
                var other = ensure_we_have_log_view_for_tab(idx);
                if (other != lv) {
                    update_non_active_filter(idx);
                    other.refresh();
                }
            }

            if (cur_context().ui.show_fulllog) {
                full_log_ctrl.refresh();
                full_log_ctrl.set_view_selected_view_name(lv.name);
            }

            update_msg_details(false);
            refresh_filter_found();            
        }

        private void force_initial_refresh_of_all_views() {
            // note: refreshing happens on different threads, so we're not sure when it's complete
            //       just take a guess and refresh in a bit

            foreach (log_view lv in all_log_views_and_full_log())
                lv.turn_off_has_anying_changed = true;
            refresh_all_views();
            
            util.add_timer(
                (has_terminated) => {
                    refresh_all_views();
                    if ( has_terminated)
                        foreach (log_view lv in all_log_views_and_full_log())
                            lv.turn_off_has_anying_changed = false;
                },
                () =>
                    {
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
            if ( text_ != null)
                text_.Dispose();

            if ( text_ != null && !(text_ is shared_memory_text_reader)) 
                text_ = new shared_memory_text_reader();
            ((shared_memory_text_reader)text_).set_memory_name( name);
            on_new_log();            
        }

        private void on_new_debug_log() {
            if ( text_ != null)
                text_.Dispose();

            text_ = new debug_text_reader();
            on_new_log();
        }

        private string reader_title() {
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
            full_log_ctrl.set_log(new log_line_reader(log_parser_));
            for (int i = 0; i < viewsTab.TabCount; ++i) {
                var lv = log_view_for_tab(i);
                if ( lv != null)
                    lv.set_log( new log_line_reader(log_parser_));
            }
        }

        private int history_select(string name) {
            ++ignore_change_;
            bool needs_save = false;
            logHistory.SelectedIndex = -1;
            for ( int i = 0; i < logHistory.Items.Count; ++i)
                if (logHistory.Items[i].ToString() == name) {
                    bool is_sample = name.ToLower().EndsWith("logwizardsetupsample.log");
                    if (is_sample)
                        logHistory.SelectedIndex = i;
                    else {
                        // whatever the user selects, move it to the end
                        history h = history_[i];
                        history_.RemoveAt(i);
                        history_.Add(h);
                        logHistory.Items.RemoveAt(i);
                        logHistory.Items.Add(name);
                        logHistory.SelectedIndex = logHistory.Items.Count - 1;
                        needs_save = true;
                    }
                    break;
                }

            if (logHistory.SelectedIndex < 0) {
                history_.Add(new history {name = name, type = 0});
                logHistory.Items.Add(name);
                logHistory.SelectedIndex = logHistory.Items.Count - 1;
            }
            --ignore_change_;
            if (needs_save)
                save();
            return logHistory.SelectedIndex;
        }

        private int last_sel = -1;
        private void on_new_log() {
            string size = text_ is file_text_reader ? " (" + new FileInfo((text_ as file_text_reader).name).Length + " bytes)" : "";
            set_status_forever("Log: " + text_.name + size);
            dropHere.Visible = false;

            // by default - try to find the syntax by reading the header info
            //              otherwise, try to parse it
            string syntax = null;
            if (text_ is file_text_reader) 
                syntax = log_to_default_syntax.file_to_syntax(text_.name);
            if ( syntax == null)
                syntax = text_.try_to_find_log_syntax();
            string name = text_.name;

            if (history_.Count < 1) 
                history_select(name);

            if ( logSyntaxCtrl.Text == "")
                logSyntaxCtrl.Text = syntax;
            if ( util.is_debug) 
                logSyntaxCtrl.Text = syntax;

            // note: we recreate the log, so that cached filters know to rebuild
            log_parser_ = new log_line_parser(text_, logSyntaxCtrl.Text);
            on_new_log_parser();

            full_log_ctrl.set_filter(new List<filter_row>());

            Text = reader_title() + " - Log Wizard " + version();
            if ( logHistory.SelectedIndex == last_sel)
                // note: sometimes this gets called twice - for instance, when user drops the combo and then selects an entry with the mouse
                return;
            last_sel = logHistory.SelectedIndex;
            add_reader_to_history();
            ui_context cur = cur_context();
            for ( int idx = 0; idx < cur.views.Count; ++idx)
                ensure_we_have_log_view_for_tab(idx);
            load_bookmarks();
            logger.Info("new reader_ " + history_[logHistory.SelectedIndex].name);

            // at this point, some file has been dropped
            log_view_for_tab(0).Visible = true;
        }

        private void add_reader_to_history() {
            if ( text_ is debug_text_reader)
                return;
            history new_ = new history();
            if ( text_ is file_text_reader) {
                new_.name = ((file_text_reader)text_).name;
                new_.type = history.entry_type.file;
            }
            else if ( text_ is shared_memory_text_reader) {
                new_.name = ((shared_memory_text_reader)text_).name;
                new_.type = history.entry_type.shmem;
            }
            else
                Debug.Assert(false);

            int history_idx = -1;
            for ( int i = 0; i < history_.Count && history_idx < 0; ++i)
                if ( new_.name == history_[i].name && new_.type == history_[i].type) 
                    history_idx = i;
            if ( history_idx < 0) {
                history_.Add(new_);
                history_idx = history_.Count - 1;
                logHistory.Items.Add(new_.combo_name);
            }
            else {
                ++ignore_change_;
                friendlyNameCtrl.Text = history_[ history_idx].friendly_name;
                logSyntaxCtrl.Text = history_[ history_idx].log_syntax;
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
            foreach ( history hist in history_)
                logHistory.Items.Add(hist.combo_name);
            logHistory.SelectedIndex = history_idx;
            --ignore_change_;
        }

        private void logHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( ignore_change_ > 0 || logHistory.SelectedIndex < 0)
                return;
            if (logHistory.DroppedDown)
                return;
            on_log_listory_changed();
        }

        private void on_log_listory_changed() {
            if (logHistory.SelectedIndex >= 0) {
                history_select(history_[logHistory.SelectedIndex].name);
                app.inst.set_log_file( history_[ logHistory.SelectedIndex].name );
            }

            sourceTypeCtrl.SelectedIndex = (int) history_[ logHistory.SelectedIndex].type;
            sourceNameCtrl.Text = history_[ logHistory.SelectedIndex].name;
            friendlyNameCtrl.Text = history_[ logHistory.SelectedIndex].friendly_name;
            logSyntaxCtrl.Text = history_[ logHistory.SelectedIndex].log_syntax;
            sourceName_TextChanged(null,null);
            if (pane_to_focus_ != null)
                postFocus.Enabled = true;
        }

        private void friendlyName_TextChanged(object sender, EventArgs e)
        {
            if (ignore_change_ > 0)
                return;
            history_[ logHistory.SelectedIndex].friendly_name = friendlyNameCtrl.Text;
            update_history();
            save();
        }

        private void logSyntax_TextChanged(object sender, EventArgs e)
        {
            if (ignore_change_ > 0)
                return;
            history_[ logHistory.SelectedIndex].log_syntax = logSyntaxCtrl.Text;
            save();
        }

        private void LogWizard_FormClosing(object sender, FormClosingEventArgs e) {
            last_filter_text_change_ = DateTime.MinValue;
            save();
        }

        private void logHistory_DropDownClosed(object sender, EventArgs e) {
            if (logHistory.Items.Count < 1)
                return; // nothing is in history

            //sourceName_TextChanged(null,null);
            on_log_listory_changed();
        }

        private void filteredViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            load_filters();
        }


        private void addContext_Click(object sender, EventArgs e)
        {
            new_context_form new_ = new new_context_form(this);
            new_.Location = Cursor.Position;
            if ( new_.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                ui_context new_ctx = new ui_context();
                if ( new_.basedOnExisting.Checked)
                    new_ctx.copy_from( cur_context());
                new_ctx.name = new_.name.Text;
                contexts_.Add(new_ctx);
                curContextCtrl.Items.Add( new_ctx.name);
                curContextCtrl.SelectedIndex = curContextCtrl.Items.Count - 1;
            }
        }

        private void delContext_Click(object sender, EventArgs e)
        {
            // make sure we have at least one, after deleting the current one
            if (curContextCtrl.Items.Count < 2)
                return;

            int sel = curContextCtrl.SelectedIndex;
            contexts_.RemoveAt(sel);
            curContextCtrl.Items.RemoveAt( sel);
            curContextCtrl.SelectedIndex = curContextCtrl.Items.Count > sel ? sel : 0;
        }

        private void curContextCtrl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // first, remove all log views, so that the new filters (from the new context) are loaded
            remove_all_log_views();

            load();
            viewsTab.SelectedIndex = 0;
            if ( cur_context().ui.show_fulllog && full_log_ctrl != null)
                full_log_ctrl.refresh();
            refresh_cur_log_view();
            save();
        }

        private void curContextCtrl_DropDown(object sender, EventArgs e)
        {
            // saving after the selection is changed would be too late
            save();
        }


        private void dropHere_DragDrop(object sender, DragEventArgs e) {
            if ( e.Data.GetDataPresent( DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if ( files.Length == 1)
                    on_file_drop(files[0]);
            }
        }

        private void dropHere_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = e.AllowedEffect;
            else
                e.Effect = DragDropEffects.None;
        }

        private void enabledCtrl_SelectionChanged(object sender, EventArgs e) {
            ++ignore_change_;
            var sel = filterCtrl.SelectedObject as filter_item;
            curFilterCtrl.Text = sel != null ? sel.text : "";
            curFilterCtrl.Enabled = sel != null;
            --ignore_change_;
        }

        private void enabledCtrl_ItemsChanged(object sender, BrightIdeasSoftware.ItemsChangedEventArgs e)
        {
            save();
        }

        private void enabledCtrl_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e) {
            if (e.SubItemIndex == filterCol.Index) {
                e.Cancel = true;

                var sel = filterCtrl.SelectedObject as filter_item;
                // we must be editing a filter row!
                Debug.Assert(sel != null);
                focusOnFilterCtrl.Enabled = true;
            }
        }

        private void enabledCtrl_CellEditFinishing(object sender, BrightIdeasSoftware.CellEditEventArgs e) {

            if (e.SubItemIndex == filterCol.Index) {
                // 1.0.66+ we should not end up here - we're modifying this in the current-filter edit
                Debug.Assert(false);

                var sel = e.RowObject as filter_item;
                // in this case, the user has edited the filter
                curFilterCtrl.Text = sel.text;
                last_filter_text_change_ = DateTime.Now;
            }
        }

        private void applyToExistingLines_CheckedChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;
            if (filterCtrl.GetItemCount() == 0)
                return;

            var sel = filterCtrl.SelectedObject as filter_item;
            // we must be editing a filter row!
            Debug.Assert(sel != null);
            if (sel == null) 
                return;

            if (sel.apply_to_existing_lines != applyToExistingLines.Checked) {
                sel.apply_to_existing_lines = applyToExistingLines.Checked;
                save();
            }
        }

        private void curFilter_TextChanged(object sender, EventArgs e)
        {
            if (ignore_change_ > 0)
                return;

            if ( filterCtrl.GetItemCount() == 0) {
                // this will in turn call us again
                addFilter_Click(null,null);
                return;
            }
            var sel = filterCtrl.SelectedObject as filter_item;
            // we must be editing a filter row!
            Debug.Assert(sel != null);
            if (sel == null) 
                return;

            if (sel.text != curFilterCtrl.Text) {
                sel.text = curFilterCtrl.Text;
                filterCtrl.RefreshObject(sel);
                last_filter_text_change_ = DateTime.Now;
            }

            var row = new filter_row(curFilterCtrl.Text);
            bool is_valid = row.is_valid;
            Color bg = is_valid ? Color.White : Color.LightPink;
            if (curFilterCtrl.BackColor.ToArgb() != bg.ToArgb())
                curFilterCtrl.BackColor = bg;

            if (row.is_valid) {
                if (filterLabel.BackColor.ToArgb() != row.bg.ToArgb())
                    filterLabel.BackColor = row.bg;
                if (filterLabel.ForeColor.ToArgb() != row.fg.ToArgb())
                    filterLabel.ForeColor = row.fg;
            }
        }

        private void enabledCtrl_MouseDown(object sender, MouseEventArgs e) {
            // for some very fucked up strange reason, if FullRowSelect is on, "on mouse up" doesn't get called - simulating a mouse move will trigger it
            // ... note: there's a bug when clicking on a combo or on a checkbox, and then clicking on the same type of control on another row
            var mouse = win32.GetMousePos();
            win32.SetMousePos( mouse.x+1, mouse.y);
        }

        private void focusOnFilterCtrl_Tick(object sender, EventArgs e) {
            focusOnFilterCtrl.Enabled = false;
            curFilterCtrl.Focus();
            curFilterCtrl.SelectionStart = curFilterCtrl.TextLength;
        }

        private void contextMatch_TextChanged(object sender, EventArgs e) {
            // not implemented yet

        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e) {
            int sel = filterCtrl.SelectedIndex;
            if (sel < 1)
                return;
            ui_context ctx = cur_context();
            int cur_view = viewsTab.SelectedIndex;
            var filters = ctx.views[cur_view].filters;
            var cur = filters[sel];
            filters.RemoveAt(sel);
            filters.Insert(sel - 1, cur);
            load_filters();
        }
        private void moveToTopToolStripMenuItem_Click(object sender, EventArgs e) {
            int sel = filterCtrl.SelectedIndex;
            if (sel < 1)
                return;
            ui_context ctx = cur_context();
            int cur_view = viewsTab.SelectedIndex;
            var filters = ctx.views[cur_view].filters;
            var cur = filters[sel];
            filters.RemoveAt(sel);
            filters.Insert(0, cur);
            load_filters();
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e) {
            int sel = filterCtrl.SelectedIndex;
            if (sel < 0)
                return;
            if (sel == filterCtrl.GetItemCount() - 1)
                return;
            ui_context ctx = cur_context();
            int cur_view = viewsTab.SelectedIndex;
            var filters = ctx.views[cur_view].filters;
            var cur = filters[sel];
            filters.RemoveAt(sel);
            filters.Insert(sel + 1, cur);
            load_filters();
        }
        private void moveToBottomToolStripMenuItem_Click(object sender, EventArgs e) {
            int sel = filterCtrl.SelectedIndex;
            if (sel < 0)
                return;
            if (sel == filterCtrl.GetItemCount() - 1)
                return;
            ui_context ctx = cur_context();
            int cur_view = viewsTab.SelectedIndex;
            var filters = ctx.views[cur_view].filters;
            var cur = filters[sel];
            filters.RemoveAt(sel);
            filters.Add(cur);
            load_filters();
        }


        private void refreshFilter_Click(object sender, EventArgs e) {
            if (text_ != null)
                log_parser_.force_reload();
            refresh_filter_found();

            util.add_timer(
                (has_ended) => {
                    refreshFilter.Enabled = has_ended;
                    refreshFilter.Text = has_ended ? "Refresh" : util.add_dots(refreshFilter.Text, 3);
                }, 2500, 250);
        }

        // http://stackoverflow.com/questions/91778/how-to-remove-all-event-handlers-from-a-control
        private void remove_event_handler(Control c, string event_name) {
            FieldInfo f1 = typeof(Control).GetField("Event" + event_name, BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(c);
            PropertyInfo pi = c.GetType().GetProperty("Events",  BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(c, null);
            list.RemoveHandler(obj, list[obj]);            
        }

        private int handled_key_idx_ = 0;
        private void LogWizard_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (key_to_action(e) != action_type.none) 
                e.IsInputKey = true;
        }
        private void LogWizard_KeyDown(object sender, KeyEventArgs e) {
            ++handled_key_idx_;
            //logger.Debug("key pressed - " + e.KeyCode + " sender " + sender);
            var action = key_to_action(e);
            if (key_to_action(e) != action_type.none) {
                e.Handled = true;
                e.SuppressKeyPress = true;
                // note: some hotkeys are sent twice
                bool handle_now = !is_key_sent_twice() || (handled_key_idx_ % 2 == 0);
                if (handle_now) {
                    last_action_ = action;
                    handle_action(action);
                    logger.Info("action by key - " + action); // + " from " + sender);
                }
            }
        }
        private void log_wizard_KeyUp(object sender, KeyEventArgs e) {
            // see if any of the moving keys is still down
            bool any_moving_key_still_down = win32.IsKeyPushedDown(Keys.Up) || win32.IsKeyPushedDown(Keys.Down) || win32.IsKeyPushedDown(Keys.PageUp) ||
                                        win32.IsKeyPushedDown(Keys.PageDown) || win32.IsKeyPushedDown(Keys.Home) || win32.IsKeyPushedDown(Keys.End);
            if ( !any_moving_key_still_down)
                switch (last_action_) {
                case log_wizard.action_type.home:
                case log_wizard.action_type.end:
                case log_wizard.action_type.pageup:
                case log_wizard.action_type.pagedown:
                case log_wizard.action_type.arrow_up:
                case log_wizard.action_type.arrow_down:
                    on_move_key_end();
                    break;
                }
        }


        private bool is_key_sent_twice() {
            return is_focus_on_full_log();
        }

        public void handle_subcontrol_keys(Control c) {
            /* seems ctrl-tab/ctrl-shift-tab are still caught 
            if (c == viewsTab) {
                remove_event_handler(c, "PreviewKeyDown");
                remove_event_handler(c, "KeyDown");
                remove_event_handler(c, "KeyPress");
                remove_event_handler(c, "KeyUp");
            }
            */
            c.PreviewKeyDown += LogWizard_PreviewKeyDown;
            c.KeyDown += LogWizard_KeyDown;
            c.KeyUp += log_wizard_KeyUp;

            foreach ( Control sub in c.Controls)
                handle_subcontrol_keys(sub);
        }

        private action_type key_to_action(Keys code, string prefix) {
            string s = code.ToString().ToLower();
            return key_to_action(prefix + s);
        }

        private action_type key_to_action(PreviewKeyDownEventArgs e) {
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu || e.KeyCode == Keys.ShiftKey)
                return action_type.none;
            string prefix = "";
            if (e.Control)
                prefix += "ctrl-";
            if (e.Shift)
                prefix += "shift-";
            if (e.Alt)
                prefix += "alt-";
            return key_to_action(e.KeyCode, prefix);
        }
        private action_type key_to_action(KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu || e.KeyCode == Keys.ShiftKey)
                return action_type.none;
            string prefix = "";
            if (e.Control)
                prefix += "ctrl-";
            if (e.Shift)
                prefix += "shift-";
            if (e.Alt)
                prefix += "alt-";
            return key_to_action(e.KeyCode, prefix);
        }

        // http://stackoverflow.com/questions/435433/what-is-the-preferred-way-to-find-focused-control-in-winforms-app
        [DllImport("user32.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)]
        private static extern IntPtr GetFocus();
        private Control focused_ctrl() {
            Control focusedControl = null;
            IntPtr focusedHandle = GetFocus();
            if(focusedHandle != IntPtr.Zero)
                // Note that if the focused Control is not a .Net control, then this will return null.
                focusedControl = Control.FromHandle(focusedHandle);
            return focusedControl;
        }

        private bool is_focus_on_edit() {
            /* doesn't work
            var active = ActiveControl;
            if ( active != null)
                if (active is TextBox)
                    return true;
            return false;
            */
            var focused = focused_ctrl();
            return focused != null && focused is TextBox;
        }

        private bool allow_arrow_to_function_normally() {
            if (is_focus_on_filter_panel())
                return true;
            if (is_focus_on_edit())
                return true;
            var focused = focused_ctrl();
            if (focused == logHistory)
                return true;
            return false;
        }

        // in case the Filter is selected, make sure we remove focus from it
        private void unfocus_filter_panel() {
            if ( is_focus_on_filter_panel())
                viewsTab.Focus();            
        }

        private bool is_focus_on_full_log() {
            var focus_ctrl = focused_ctrl();
            return focus_ctrl != null && focus_ctrl is VirtualObjectListView && focus_ctrl.Parent == full_log_ctrl;
        }

        private bool is_focus_on_filter_panel() {
            var focus = focused_ctrl();
            return focus == filterCtrl || focus == curFilterCtrl;            
        }

        private action_type key_to_action(string key_code) {
            switch (key_code) {
            case "up":
            case "down":
                if (allow_arrow_to_function_normally())
                    return action_type.none;
                break;
            case "ctrl-right":
            case "ctrl-left":
                if ( is_focus_on_edit())
                    return action_type.none;
                break;
            }

            bool has_modifiers = key_code.Contains("ctrl-") || key_code.Contains("alt-") || key_code.Contains("shift");
            if (!has_modifiers && key_code != "tab" && is_focus_on_edit())
                // key down - in edit -> don't have it as hotkey
                return action_type.none;

            switch (key_code) {
            case "ctrl-f": 
                return action_type.search ;
            case "f3":
                return action_type.search_next;
            case "shift-f3":
                return action_type.search_prev;
            case "escape":
                return action_type.clear_search;

            case "ctrl-f2":
                return action_type.toggle_bookmark;
            case "f2":
                return action_type.next_bookmark;
            case "shift-f2":
                return action_type.prev_bookmark;
            case "ctrl-shift-f2":
                return action_type.clear_bookmarks;

            case "ctrl-shift-c":
                return action_type.copy_full_line_to_clipboard;
            case "ctrl-c":
                return action_type.copy_to_clipboard;

                // for some strange reason, ctrl-tab/ctrl-shift-tab are caught by the viewsTab - even if I remove the event handlers
                // http://stackoverflow.com/questions/91778/how-to-remove-all-event-handlers-from-a-control
            case "ctrl-right":
                return action_type.next_view;
            case "ctrl-left":
                return action_type.prev_view;
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

            case "alt-f":
                return action_type.toggle_filters;
            case "alt-o":
                return action_type.toggle_source;
            case "alt-l":
                return action_type.toggle_fulllog;

            case "alt-m":
                return action_type.toggle_show_msg_only;
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

            case "ctrl-shift-1":
                return action_type.save_position_1;
            case "ctrl-shift-2":
                return action_type.save_position_2;
            case "ctrl-shift-3":
                return action_type.save_position_3;
            case "ctrl-shift-4":
                return action_type.save_position_4;
            case "ctrl-shift-5":
                return action_type.save_position_5;

            case "ctrl-1":
                return action_type.toggle_position_1;
            case "ctrl-2":
                return action_type.toggle_position_2;
            case "ctrl-3":
                return action_type.toggle_position_3;
            case "ctrl-4":
                return action_type.toggle_position_4;
            case "ctrl-5":
                return action_type.toggle_position_5;
            }

            return action_type.none;
        }

        private void handle_action(action_type action) {
            int sel = viewsTab.SelectedIndex;
            var lv = selected_view();

            switch (action) {
            case action_type.search:
                var searcher = new search_form(this);
                if (searcher.ShowDialog() == DialogResult.OK) {
                    search_for_text_ = searcher.search;
                    // remove focus from the Filters tab, just in case (otherwise, search prev/next would end up working on that)
                    unfocus_filter_panel();

                    if ( search_for_text_.mark_lines_with_color)
                        lv.mark_text(search_for_text_, search_for_text_.fg, search_for_text_.bg);
                    lv.search_for_text_first(search_for_text_);
                }
                break;
            case action_type.search_prev:
                search_prev();
                break;
            case action_type.search_next:
                search_next();
                break;
            case action_type.clear_search:
                clear_search();
                break;

            case action_type.next_view: {
                int prev_idx = viewsTab.SelectedIndex;
                int next_idx = viewsTab.TabCount > 0 ? (sel + 1) % viewsTab.TabCount : -1;
                if (next_idx >= 0) {
                    viewsTab.SelectedIndex = next_idx;
                    log_view_for_tab(next_idx).on_selected();
                }
                if ( prev_idx >= 0)
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
                if ( prev_idx >= 0)
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
                if ( lv != null)
                    lv.on_action(action);
                break;

            case action_type.toggle_filters:
                toggleFilters_Click(null,null);
                break;
            case action_type.toggle_fulllog:
                toggleFullLog_Click(null,null);
                break;
            case action_type.toggle_source:
                toggleSource_Click(null,null);
                break;

            case action_type.copy_to_clipboard:
                if (lv != null)
                    lv.copy_to_clipboard();
                break;
            case action_type.copy_full_line_to_clipboard:
                if (lv != null)
                    lv.copy_full_line_to_clipboard();
                break;

            case action_type.toggle_bookmark:
                int line_idx = lv != null ? lv.sel_line_idx : -1;
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
                if (lv != null)
                    lv.next_bookmark();
                break;
            case action_type.prev_bookmark:
                if (lv != null)
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
                    logHistory.DroppedDown = false;
                else {
                    var panes = this.panes();
                    if (panes.Contains(focused_ctrl()))
                        pane_to_focus_ = focused_ctrl();
                    logHistory.Focus();
                    logHistory.DroppedDown = true;
                }
                break;
            case action_type.new_log_wizard:
                newView_Click(null,null);
                break;
            case action_type.show_preferences:
                settingsCtrl_Click(null,null);
                break;

            case action_type.increase_font:
                foreach (log_view view in all_log_views_and_full_log())
                    view.increase_font(1);
                break;
            case action_type.decrease_font:
                foreach (log_view view in all_log_views_and_full_log())
                    view.increase_font(-1);
                break;

            case action_type.toggle_show_msg_only:
                if (lv != null)
                    lv.toggle_show_msg_only();
                break;
            case action_type.scroll_up:
                if (lv != null)
                    lv.scroll_up();
                break;
            case action_type.scroll_down:
                if (lv != null)
                    lv.scroll_down();
                break;

            case action_type.go_to_line:
                if (lv != null) {
                    var dlg = new go_to_line_time_form(this);
                    if (dlg.ShowDialog() == DialogResult.OK) {
                        if (dlg.is_number()) {
                            if ( dlg.has_offset != '\0')
                                lv.offset_closest_line(dlg.number, dlg.has_offset == '+');
                            else
                                lv.go_to_closest_line(dlg.number - 1, log_view.select_type.notify_parent);
                        } else if (dlg.has_offset != '\0')
                            lv.offset_closest_time(dlg.time_milliseconds, dlg.has_offset == '+');
                        else
                            lv.go_to_closest_time(dlg.normalized_time);
                    }
                }
                break;
            case action_type.refresh:
                refreshFilter_Click(null,null);
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
                try {
                    string name = text_ != null ? text_.name : "";
                    Process.Start("explorer", "/select,\"" + name + "\"");
                } catch (Exception e) {
                    logger.Error("can't open in explorer " + e.Message);
                }
                break;

            case action_type.none:
                break;

            case action_type.save_position_1:
                save_custom_ui(0);
                break;
            case action_type.save_position_2:
                save_custom_ui(1);
                break;
            case action_type.save_position_3:
                save_custom_ui(2);
                break;
            case action_type.save_position_4:
                save_custom_ui(3);
                break;
            case action_type.save_position_5:
                save_custom_ui(4);
                break;
            case action_type.toggle_position_1:
                toggle_custom_ui(0);
                break;
            case action_type.toggle_position_2:
                toggle_custom_ui(1);
                break;
            case action_type.toggle_position_3:
                toggle_custom_ui(2);
                break;
            case action_type.toggle_position_4:
                toggle_custom_ui(3);
                 break;
           case action_type.toggle_position_5:
                toggle_custom_ui(4);
                break;
            default:
                Debug.Assert(false);
                break;
            }
        }

        private void clear_search() {
            if (msg_details_.visible()) {
                msg_details_.show(false);
                return;
            }

            if (selected_view() != null)
                selected_view().unmark();
            search_for_text_ = null;
        }

        private void save_custom_ui(int idx) {
            toggled_to_custom_ui_ = -1;
        }
        private void toggle_custom_ui(int idx) {
            toggled_to_custom_ui_ = idx == toggled_to_custom_ui_ ? -1 : idx;
            load_ui();
        }

        private List<Control> panes() {
            List<Control> panes = new List<Control>();

            // first pane - the current view (tab)
            int sel = viewsTab.SelectedIndex;
            if ( sel >= 0 && log_view_for_tab(sel) != null)
                panes.Add( log_view_for_tab(sel).list);

            // second pane - the full log (if shown)
            if( cur_context().ui.show_fulllog)
                panes.Add(full_log_ctrl.list);

            // third pane - the filters control (if visible)
            if ( cur_context().ui.show_filter)
                panes.Add(filterCtrl);

            // fourth pane - the edit box (if enabled)
            if ( cur_context().ui.show_filter && curFilterCtrl.Enabled)
                panes.Add(curFilterCtrl);
            return panes;
        }

        // keeps the other logs in sync with this one - if needed
        private void keep_logs_in_sync(log_view src) {
            int line_idx = src.sel_line_idx;
            if (line_idx < 0)
                return;
            foreach ( log_view lv in all_log_views_and_full_log())
                if (lv != src) {
                    if (cur_context().ui.show_fulllog && lv == full_log_ctrl && app.inst.sync_full_log_view)
                        // in this case, we already synched the full log
                        continue;

                    lv.go_to_closest_line(line_idx, log_view.select_type.do_not_notify_parent);
                }
        }

        private void switch_pane(bool forward) {
            List<Control> panes = this.panes();
            Control focus_ctrl = focused_ctrl();
            if (focus_ctrl == filterCtrl && filterCtrl.SelectedIndex < 0)
                // no filter selected
                focus_ctrl = null;
            int idx = panes.IndexOf(focus_ctrl);
            if (idx >= 0)
                // move to next control
                idx = forward ? idx + 1 : idx + panes.Count - 1;
            else 
                // move to first / last
                idx = forward ? 0 : panes.Count - 1;
            // note: can't focus now, since the "next/prev" pane event might be triggered twice if from Full-Log
            pane_to_focus_ = panes[ idx % panes.Count ];
            postFocus.Enabled = true;
        }

        private void filterCtrl_KeyPress(object sender, KeyPressEventArgs e) {
            e.Handled = true;
        }

        private void filterCtrl_SelectedIndexChanged(object sender, EventArgs e) {
            int sel_filter = filterCtrl.SelectedIndex;
            int sel_view = viewsTab.SelectedIndex;
            bool has_sel = sel_filter >= 0 && sel_view >= 0;
            if (!has_sel) {
                ++ignore_change_;
                curFilterCtrl.Text = "";
                applyToExistingLines.Checked = false;
                --ignore_change_;
                return;
            }

            filter_item i = filterCtrl.GetItem(sel_filter).RowObject as filter_item;
            filter_row filt = new filter_row(i.text);
            if (filt.is_valid) {
                var lv = ensure_we_have_log_view_for_tab(sel_view);
                Color fg = util.str_to_color(sett.get("filter_fg", "transparent"));
                Color bg = util.str_to_color(sett.get("filter_bg", "#faebd7"));
                lv.mark_match(sel_filter, fg, bg);
            }

            ++ignore_change_;
            applyToExistingLines.Checked = i.apply_to_existing_lines;
            --ignore_change_;
        }

        private void settingsCtrl_Click(object sender, EventArgs e) {
            var old_sync_colors = app.inst.syncronize_colors;
            var old_sync_gray = app.inst.sync_colors_all_views_gray_non_active;
            new settings_form(this).ShowDialog();

            bool sync_changed = app.inst.syncronize_colors != old_sync_colors || old_sync_gray != app.inst.sync_colors_all_views_gray_non_active;
            if ( sync_changed)
                sync_full_log_colors();
        }

        private log_view selected_view() {
            int sel = viewsTab.SelectedIndex;
            if (sel < 0)
                return null;
            if (is_focus_on_full_log())
                return full_log_ctrl;

            var lv = ensure_we_have_log_view_for_tab(sel);
            return lv;
        }

        public string matched_logs(int line_idx) {
            List<string> matched = new List<string>();
            foreach ( var lv in all_log_views())
                if ( lv.matches_line(line_idx))
                    matched.Add(lv.name);

            string selected = log_view_for_tab(viewsTab.SelectedIndex).name;
            bool removed = matched.Remove(selected);
            if ( removed)
                matched.Insert(0, selected);

            string txt = "";
            foreach (string m in matched) {
                if (txt != "")
                    txt += ",";
                txt += m;
            }
            return txt;
        }

        private void search_next() {
            var lv = selected_view();
            if (lv == null)
                return;
            if ( search_for_text_ != null)
                lv.search_for_text_next(search_for_text_);
            else if ( filterCtrl.SelectedIndex >= 0)
                lv.search_for_next_match( filterCtrl.SelectedIndex);
        }

        private void search_prev() {
            var lv = selected_view();
            if (lv == null)
                return;
            if ( search_for_text_ != null)
                lv.search_for_text_prev(search_for_text_);            
            else if ( filterCtrl.SelectedIndex >= 0)
                lv.search_for_prev_match( filterCtrl.SelectedIndex);
        }

        private void tipsHotkeys_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            new help_form(this).Show();
        }

        private void load_bookmarks() {
            bookmarks_.Clear();
            string bookmarks_key = text_.name.Replace("=", "_").Replace("\\", "_");
            string[] bookmarks = sett.get("bookmarks." + bookmarks_key).Split(',');
            foreach (var b in bookmarks) {
                int line_idx;
                if ( int.TryParse(b, out line_idx))
                    bookmarks_.Add(line_idx);
            }
            notify_views_of_bookmarks();
        }

        // notifies the views of our bookmarks
        private void notify_views_of_bookmarks() {
            for (int i = 0; i < viewsTab.TabCount; ++i)
                // always send a copy of the list - this way, the views can see which bookmarks are new/deleted
                log_view_for_tab(i).set_bookmarks(bookmarks_.ToList());
            full_log_ctrl.set_bookmarks(bookmarks_.ToList());
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

        private void postFocus_Tick(object sender, EventArgs e) {
            postFocus.Enabled = false;
            if (pane_to_focus_ != null) {
                pane_to_focus_.Focus();
                var list = pane_to_focus_ as ObjectListView;
                // select the first item - if nothing is selected
                if (list != null && list.SelectedIndex < 0 && list.GetItemCount() > 0)
                    list.SelectedIndex = 0;
                pane_to_focus_ = null;
            }
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


        private void viewToClipboard_Click(object sender, EventArgs e) {
            ui_context cur = cur_context();
            int cur_view = viewsTab.SelectedIndex;
            if (cur_view < 0)
                return;
            var view = cur.views[cur_view];
            if (view.filters.Count < 1)
                return; // nothing to copy
            var formatter = new XmlSerializer( typeof(ui_view));
            string to_copy = "";
            using (var stream = new MemoryStream()) {
                formatter.Serialize(stream, view);
                stream.Flush();
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                    to_copy = reader.ReadToEnd();
            }
            Clipboard.SetText(to_copy);
        }
        

        private void viewFromClipboard_Click(object sender, EventArgs ea) {
            try {
                string txt = Clipboard.GetText();
                ui_context cur = cur_context();
                int cur_view = viewsTab.SelectedIndex;
                if (cur_view < 0)
                    return;
                var view = cur.views[cur_view];
                var formatter = new XmlSerializer( typeof(ui_view));
                using (var stream = new MemoryStream()) {
                    using (var writer = new StreamWriter(stream)) {
                        writer.Write(txt);
                        writer.Flush();
                        stream.Position = 0;
                        using (var reader = new StreamReader(stream)) {
                            var new_view = (ui_view) formatter.Deserialize(reader);
                            // we don't care about the name, just the filters
                            new_view.filters.ForEach(f => f.text = util.normalize_enters(f.text));
                            view.filters = new_view.filters;
                        }
                    }
                }
                load_filters();
                refreshFilter_Click(null, null);
                save();
            } catch(Exception e) {
                logger.Error("can't copy from clipboard: " + e.Message);
                util.beep(util.beep_type.err);
            }
        }

        private void contextToClipboard_Click(object sender, EventArgs e) {
            ui_context cur = cur_context();
            if (cur.views.Count < 1)
                return; // no views
            var formatter = new XmlSerializer( typeof(ui_context));
            string to_copy = "";
            using (var stream = new MemoryStream()) {
                formatter.Serialize(stream, cur);
                stream.Flush();
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                    to_copy = reader.ReadToEnd();
            }
            Clipboard.SetText(to_copy);
        }

        private void contextFromClipboard_Click(object sender, EventArgs ea) {
            try {
                string txt = Clipboard.GetText();
                ui_context cur = cur_context();

                var formatter = new XmlSerializer( typeof(ui_context));
                using (var stream = new MemoryStream()) {
                    using (var writer = new StreamWriter(stream)) {
                        writer.Write(txt);
                        writer.Flush();
                        stream.Position = 0;
                        using (var reader = new StreamReader(stream)) {
                            var new_ctx = (ui_context) formatter.Deserialize(reader);
                            // we don't care about the name, just the filters
                            foreach ( var view in new_ctx.views)
                                view.filters.ForEach(f => f.text = util.normalize_enters(f.text));
                            // ... preserve existing context name
                            string ctx_name = cur.name;
                            cur.copy_from(new_ctx);
                            cur.name = ctx_name;
                        }
                    }
                }
                curContextCtrl_SelectedIndexChanged(null, null);
            } catch(Exception e) {
                logger.Error("can't copy from clipboard: " + e.Message);
                util.beep(util.beep_type.err);
            }
        }




        // sets the status for a given period - after that ends, the previous status is shown
        // if < 0, it's forever
        private void set_status(string msg, int set_status_for_ms = 5000) {
            if (set_status_for_ms <= 0)
                statuses_.Clear();

            statuses_.Add( new Tuple<string, DateTime>(msg,  set_status_for_ms > 0 ? DateTime.Now.AddMilliseconds(set_status_for_ms) : DateTime.MaxValue));
            status.Text = status_prefix_ + msg;
            status.ForeColor = msg.ToLower().StartsWith("err") ? Color.Red : Color.Black;
        }

        private void set_status_forever(string msg) {
            set_status(msg, -1);
        }

        private void update_status_text(bool force = false) {
            bool needs_update = false;
            while (statuses_.Count > 0 && statuses_.Last().Item2 < DateTime.Now) {
                statuses_.RemoveAt(statuses_.Count - 1);
                needs_update = true;
            }

            if (needs_update || force)
                status.Text = statuses_.Count > 0 ? status_prefix_ + statuses_.Last().Item1 : "";
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
            if ( File.Exists(tn2_file()))
                items.Add( new MenuItem("TableNinja II", (o,args) => on_file_drop(tn2_file())));
            if ( File.Exists(hm2_file()))
                items.Add( new MenuItem("HM2", (o,args) => on_file_drop(hm2_file())));
            if ( File.Exists(hm3_file()))
                items.Add( new MenuItem("HM3", (o,args) => on_file_drop(hm3_file())));

            monitor.ContextMenu = new ContextMenu(items.ToArray());
            monitor.ContextMenu.Show(monitor, monitor.PointToClient(Cursor.Position) );
        }

        private void toggleTopmost_Click(object sender, EventArgs e) {
            TopMost = !TopMost;
            update_topmost_image();
        }

        private void log_wizard_SizeChanged(object sender, EventArgs e) {
            update_msg_details(true);
            if (ignore_change_ > 0)
                return;

            // remember position - if Visible
            save_location();
        }

        private void log_wizard_LocationChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;

            save_location();
        }

        private void save_location() {
            if (toggled_to_custom_ui_ >= 0)
                return;
            if (!Visible)
                return;
            if (WindowState == FormWindowState.Minimized)
                return;

            if (WindowState == FormWindowState.Maximized)
                cur_ui.maximized = true;
            else {
                cur_ui.width = Width;
                cur_ui.height = Height;
                cur_ui.left = Left;
                cur_ui.top = Top;
                cur_ui.maximized = false;
            }
            save();            
        }

        private void log_wizard_Activated(object sender, EventArgs e) {

        }

        private void filteredLeft_SplitterMoved(object sender, SplitterEventArgs e) {
            if (ignore_change_ > 0)
                return;
            //logger.Debug("[splitter] filteredleft=" + filteredLeft.SplitterDistance  );
            if (filteredLeft.SplitterDistance >= 0) {
                cur_context().ui.full_log_splitter_pos = filteredLeft.SplitterDistance;
                save();
            }
            else
                Debug.Assert(false);
        }


    }
}
