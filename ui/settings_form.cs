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
using lw_common;

namespace LogWizard.ui {
    partial class settings_form : Form {

        private class item {
            public string key = "";
            public string value = "";
        }

        private const int MAX_ITEMS = 50;

        public settings_form(log_wizard parent) {
            InitializeComponent();
            load();
            TopMost = parent.TopMost;
        }

        private void load() {
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

            logBgSingleColor.Checked = !app.inst.use_bg_gradient;
            logBgGradient.Checked = app.inst.use_bg_gradient;

            logBg.SelectedItem = app.inst.bg;
            logBgFrom.SelectedItem = app.inst.bg_from;
            logBgTo.SelectedItem = app.inst.bg_to;

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

            app.inst.use_bg_gradient = logBgGradient.Checked;
            app.inst.bg = logBg.SelectedItem;
            app.inst.bg_from = logBgFrom.SelectedItem;
            app.inst.bg_to = logBgTo.SelectedItem;

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

            foreach (var lw in log_wizard.forms) {
                lw.stop_saving();
                lw.Visible = false;
            }

            string dir = Program.local_dir();
            try {
                File.Copy(dir + "\\logwizard.txt", dir + "\\logwizard_user.txt", true);
            } catch {
            }

            util.restart_app();
        }

        private void hotkeyslink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/jtorjo/logwizard/wiki/Hotkeys");
        }
    }
}
