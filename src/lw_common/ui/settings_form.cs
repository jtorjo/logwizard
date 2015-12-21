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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class settings_form : Form {

        private class item {
            public string key = "";
            public string value = "";
        }

        private const int MAX_ITEMS = 50;

        private bool wants_reset_settings_ = false;
        private bool needs_restart_ = false;

        public settings_form(Form parent) {
            InitializeComponent();
            load();
            TopMost = parent.TopMost;
        }

        public bool wants_reset_settings {
            get { return wants_reset_settings_; }
        }

        public bool needs_restart {
            get { return needs_restart_; }
        }

        private void load() {
            var font = app.inst.font;
            fontLabel.Text = "Font: [" + font.Name + ", " + (int) Math.Round(font.Size) + "pt]";

            viewLineCount.Checked = app.inst.show_view_line_count;
            viewLine.Checked = app.inst.show_view_selected_line;
            viewIndex.Checked = app.inst.show_view_selected_index;

            toggleTopmost.Checked = app.inst.show_topmost_toggle;
            bringToTopOnRestart.Checked = app.inst.bring_to_top_on_restart;
            makeTopmostOnRestart.Checked = app.inst.make_topmost_on_restart;
            makeTopmostOnRestart.Enabled = bringToTopOnRestart.Checked;

            switch (app.inst.syncronize_colors) {
            case app.synchronize_colors_type.none:
                syncColorsNone.Checked = true;
                break;
            case app.synchronize_colors_type.with_current_view:
                syncColorsCurView.Checked = true;
                break;
            case app.synchronize_colors_type.with_all_views:
                syncColorsAllViews.Checked = true;
                break;
            default:
                Debug.Assert(false);
                break;
            }
            syncColorsGrayOutNonActive.Checked = app.inst.sync_colors_all_views_gray_non_active;


            logFg.SelectedItem = app.inst.fg;
            logBg.SelectedItem = app.inst.bg;
            dimmedFg.SelectedItem = app.inst.dimmed_fg;
            dimmedBg.SelectedItem = app.inst.dimmed_bg;
            bookmarkFg.SelectedItem = app.inst.bookmark_fg;
            bookmarkBg.SelectedItem = app.inst.bookmark_bg;
            fullLogGrayFg.SelectedItem = app.inst.full_log_gray_fg;
            fullLogGrayBg.SelectedItem = app.inst.full_log_gray_bg;
            // 1.3.29+ - hide this - it was mainly for testing - i know it's a bit cool, but let just ignore it for now
            //logBgSingleColor.Checked = !app.inst.use_bg_gradient;
            //logBgGradient.Checked = app.inst.use_bg_gradient;
            //logBgFrom.SelectedItem = app.inst.bg_from;
            //logBgTo.SelectedItem = app.inst.bg_to;

            noteAuthorName.Text = app.inst.notes_author_name;
            noteInitials.Text = app.inst.notes_initials;
            noteColor.SelectedItem = app.inst.notes_color;

            zipExtensions.Text = app.inst.look_into_zip_files_str;

            switch (app.inst.identify_notes_files) {
            case md5_log_keeper.md5_type.fast:
                noteFast.Checked = true;
                break;
            case md5_log_keeper.md5_type.slow:
                noteSlow.Checked = true;
                break;
            case md5_log_keeper.md5_type.by_file_name:
                noteByFileName.Checked = true;
                break;
            default:
                Debug.Assert(false);
                break;
            }

            useHotkeys.Checked = app.inst.use_hotkeys;

            foreach ( var ftc in app.inst.file_to_context)
                fileToContext.AddObject( new item { key = ftc.Key, value = ftc.Value });
            foreach ( var fts in app.inst.file_to_syntax)
                fileToSyntax.AddObject( new item { key = fts.Key, value = fts.Value } );

            while ( fileToContext.GetItemCount() < MAX_ITEMS)
                fileToContext.AddObject(new item());
            while ( fileToSyntax.GetItemCount() < MAX_ITEMS)
                fileToSyntax.AddObject(new item());

            editMode.SelectedIndex = (int)app.inst.edit_mode;

            editSearchAfter.Checked = app.inst.edit_search_after;
            editSearchBelow.Checked = app.inst.edit_search_before;
            if (app.inst.edit_search_all_columns)
                editSearchAllColumns.Checked = true;
            else
                editSearchCurColumnOnly.Checked = true;

            filterShowFilterRowInFilterColor.Checked = app.inst.show_filter_row_in_filter_color;
            useFileMonitoringApi.Checked = app.inst.use_file_monitoring_api;
            showBetaUpdates.Checked = app.inst.show_beta_releases;
            showVariableFontsAsWell.Checked = app.inst.show_variable_fonts_as_well;
            showTips.Checked = app.inst.show_tips;
        }

        private void save() {
            app.inst.show_view_line_count = viewLineCount.Checked;
            app.inst.show_view_selected_line = viewLine.Checked;
            app.inst.show_view_selected_index = viewIndex.Checked;
            app.inst.bring_to_top_on_restart = bringToTopOnRestart.Checked;
            app.inst.make_topmost_on_restart = makeTopmostOnRestart.Checked;

            if (syncColorsNone.Checked) app.inst.syncronize_colors = app.synchronize_colors_type.none;
            else if (syncColorsCurView.Checked) app.inst.syncronize_colors = app.synchronize_colors_type.with_current_view;
            else if (syncColorsAllViews.Checked) app.inst.syncronize_colors = app.synchronize_colors_type.with_all_views;
            else Debug.Assert(false);
            app.inst.sync_colors_all_views_gray_non_active = syncColorsGrayOutNonActive.Checked;

            app.inst.fg = logFg.SelectedItem;
            app.inst.bg = logBg.SelectedItem;
            app.inst.dimmed_fg = dimmedFg.SelectedItem;
            app.inst.dimmed_bg = dimmedBg.SelectedItem;
            app.inst.bookmark_fg = bookmarkFg.SelectedItem;
            app.inst.bookmark_bg = bookmarkBg.SelectedItem;
            app.inst.full_log_gray_fg = fullLogGrayFg.SelectedItem;
            app.inst.full_log_gray_bg = fullLogGrayBg.SelectedItem;
            // 1.3.29+ - hide this - it was mainly for testing - i know it's a bit cool, but let just ignore it for now
            // app.inst.use_bg_gradient = logBgGradient.Checked;
            //app.inst.bg_from = logBgFrom.SelectedItem;
            //app.inst.bg_to = logBgTo.SelectedItem;

            app.inst.notes_author_name = noteAuthorName.Text;
            app.inst.notes_initials = noteInitials.Text;
            app.inst.notes_color = noteColor.SelectedItem;

            app.inst.look_into_zip_files_str = zipExtensions.Text;

            if (noteFast.Checked) app.inst.identify_notes_files = md5_log_keeper.md5_type.fast;
            else if ( noteSlow.Checked) app.inst.identify_notes_files = md5_log_keeper.md5_type.slow;
            else if ( noteByFileName.Checked) app.inst.identify_notes_files = md5_log_keeper.md5_type.by_file_name;
            else Debug.Assert(false);

            app.inst.use_hotkeys = useHotkeys.Checked;

            bool error = false;
            app.inst.file_to_context.Clear();
            for (int idx = 0; idx < fileToContext.GetItemCount(); ++idx) {
                var i = fileToContext.GetItem(idx).RowObject as item;
                if ( i.key != "" && i.value != "")
                    if (!app.inst.file_to_context.ContainsKey(i.key))
                        app.inst.file_to_context.Add(i.key, i.value);
                    else {
                        // user wrote the same key twice - take the last
                        error = true;
                        app.inst.file_to_context[i.key] = i.value;
                    }
            }
            app.inst.file_to_syntax.Clear();
            for (int idx = 0; idx < fileToSyntax.GetItemCount(); ++idx) {
                var i = fileToSyntax.GetItem(idx).RowObject as item;
                if ( i.key != "" && i.value != "")
                    if (!app.inst.file_to_syntax.ContainsKey(i.key))
                        app.inst.file_to_syntax.Add(i.key, i.value);
                    else {
                        // user wrote the same key twice - take the last
                        error = true;
                        app.inst.file_to_syntax[i.key] = i.value;
                    }
            }
            if ( error)
                util.beep(util.beep_type.err);

            app.inst.edit_mode = (app.edit_mode_type) editMode.SelectedIndex;

            app.inst.edit_search_after = editSearchAfter.Checked;
            app.inst.edit_search_before = editSearchBelow.Checked;
            app.inst.edit_search_all_columns = editSearchAllColumns.Checked;

            app.inst.show_filter_row_in_filter_color = filterShowFilterRowInFilterColor.Checked;
            app.inst.use_file_monitoring_api = useFileMonitoringApi.Checked;
            app.inst.show_beta_releases = showBetaUpdates.Checked;
            app.inst.show_variable_fonts_as_well = showVariableFontsAsWell.Checked;
            app.inst.show_tips = showTips.Checked;

            app.inst.save();
        }

        private void settings_form_FormClosed(object sender, FormClosedEventArgs e) {
            save();
        }

        private void close_Click(object sender, EventArgs e) {
            save();
            DialogResult = DialogResult.OK;
        }

        private void bringToTopOnRestart_CheckedChanged(object sender, EventArgs e) {
            makeTopmostOnRestart.Enabled = bringToTopOnRestart.Checked;
        }

        private void reset_Click(object sender, EventArgs e) {
            bool want_reset = MessageBox.Show("Do you want to reset ALL the Settings to their Defaults?", "LogWizard", MessageBoxButtons.YesNo) == DialogResult.Yes;
            if (!want_reset)
                return;

            wants_reset_settings_ = true;
            needs_restart_ = true;
            DialogResult = DialogResult.OK;
        }

        private void hotkeyslink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/jtorjo/logwizard/wiki/Hotkeys");
        }

        private void browseFont_Click(object sender, EventArgs e) {
            var dlg = new FontDialog();
            dlg.ScriptsOnly = true;
            dlg.FixedPitchOnly = !showVariableFontsAsWell.Checked;
            dlg.FontMustExist = true;
            dlg.Font = app.inst.font;
            dlg.ShowEffects = false;
            if (dlg.ShowDialog() == DialogResult.OK) {
                var font = dlg.Font;
                int new_font_size = (int)Math.Round(font.Size);
                bool any_change = app.inst.font_name != font.Name || app.inst.font_size != new_font_size;
                app.inst.font_name = font.Name;
                app.inst.font_size = new_font_size;
                if (any_change) {
                    app.inst.save();
                    needs_restart_ = true;
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void useFileMonitoringApi_CheckedChanged(object sender, EventArgs e) {

        }
    }
}
