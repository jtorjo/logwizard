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

namespace LogWizard.ui {
    partial class settings_form : Form {
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
        }

        private void save() {
            app.inst.show_view_line_count = viewLineCount.Checked;
            app.inst.show_view_selected_line = viewLine.Checked;
            app.inst.show_view_selected_index = viewIndex.Checked;
            app.inst.bring_to_top_on_restart = bringToTopOnRestart.Checked;
            app.inst.make_topmost_on_restart = makeTopmostOnRestart.Checked;

            if (syncColorsNone.Checked) app.inst.syncronize_colors = app.synchronize_colors_type.none;
            else if (syncColorsCurView.Checked) app.inst.syncronize_colors = app.synchronize_colors_type.with_current_view;
            else if ( syncColorsAllViews.Checked) app.inst.syncronize_colors = app.synchronize_colors_type.with_all_views;
            else Debug.Assert(false);
            app.inst.sync_colors_all_views_gray_non_active = syncColorsGrayOutNonActive.Checked;

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
    }
}
