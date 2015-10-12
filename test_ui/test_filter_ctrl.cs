using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lw_common;

namespace test_ui {
    public partial class test_filter_ctrl : Form {
        public test_filter_ctrl() {
            InitializeComponent();

            filtCtrl.design_mode = false;
            filtCtrl.on_save = save;
            filtCtrl.ui_to_view = ui_to_view;
            filtCtrl.on_rerun_view = rerun;
            filtCtrl.on_refresh_view = refresh_view;
            filtCtrl.mark_match = mark_match;

            ui_view dummy = new ui_view();
            dummy.filters.Add(new ui_filter() { text = "$msg contains [x]", enabled = true, dimmed = false, apply_to_existing_lines = false});
            dummy.filters.Add(new ui_filter() { text = "$level = ERROR", enabled = true, dimmed = false, apply_to_existing_lines = false});
            dummy.filters.Add(new ui_filter() { text = "$msg contains pot", enabled = true, dimmed = false, apply_to_existing_lines = true});
            filtCtrl.view_to_ui(dummy,0);
        }

        private void save() {
            add_callback("save_to ");
        }

        private void rerun(int view_idx) {
            add_callback("rerun ");
        }
        private void refresh_view(int view_idx) {
            add_callback("refresh ");
        }

        private void mark_match(int idx) {
            add_callback("match " + idx + " ");
        }

        private void ui_to_view(int view_idx) {
            var rows = filtCtrl.to_filter_row_list();
            string status = "";
            foreach (var row in rows) 
                status += "[" + (row.enabled ? "1" : "0") + "/" + (row.dimmed ? "1" : "0") + "] " + (row.apply_to_existing_lines ? "[e]" : "[ ]") + " " +
                          row.unique_id.Replace("\r\n", " | ") + " ; " + util.color_to_str(row.fg) + " " + util.color_to_str(row.bg) + "\r\n";
            existingRows.Text = status;

            add_callback("ui-to-view ");
        }

        private void add_callback(string cb) {
            callbacks.AppendText(cb);
            callbacks.ScrollToCaret();
        }

        private void refresh_Tick(object sender, EventArgs e) {
            if ( !callbacks.Text.EndsWith("----\r\n")) 
                add_callback(" ----\r\n");
        }
    }
}
