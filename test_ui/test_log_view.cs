using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lw_common;
using lw_common.ui;
using LogWizard;

namespace test_ui {
    public partial class test_log_view : Form, log_view_parent {
        private log_view lv_;

        public test_log_view() {
            InitializeComponent();

            string file = @"C:\john\code\buff\lw-tests\small.log";
            string syntax = "$time[0,12] $ctx1[13,10] $level[24,5] $class[' ','- '] $msg";

            lv_ = new log_view(this, "testing 123");
            lv_.Dock = DockStyle.Fill;
            this.Controls.Add(lv_);
            lv_.show_name = false;

            lv_.set_log( new log_line_reader( new log_line_parser(new file_text_reader(file), syntax ) ));
            var filter = new List<raw_filter_row>();
            lv_.set_filter( filter  );

            app.inst.edit_mode = app.edit_mode_type.always;
//            app.inst.edit_mode = app.edit_mode_type.with_space;
        }

        public void handle_subcontrol_keys(Control c) {
        }

        public void on_view_name_changed(log_view view, string name) {
        }

        public void on_sel_line(log_view lv, int line_idx) {
        }

        public string matched_logs(int line_idx) {
            return "-matched";
        }

        public Rectangle client_rect_no_filter {
            get {
                return new Rectangle();
            }
        }

        private void refresh_Tick(object sender, EventArgs e) {
            lv_.refresh();
        }
    }
}
