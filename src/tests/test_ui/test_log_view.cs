using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lw_common;
using lw_common.parse;
using lw_common.ui;
using LogWizard;

namespace test_ui {
    public partial class test_log_view : Form, log_view_parent {
        private log_view lv_;

        public test_log_view() {
            InitializeComponent();

            string file = @"C:\john\code\buff\lw-tests\small.log";
            string syntax = "$time[0,12] $ctx1[13,10] $level[24,5] $class[' ','- '] $msg";

            var sett = new log_settings_string("");
            sett.type.set(log_type.file);
            sett.name.set(file);
            sett.syntax.set(syntax);

            lv_ = new log_view(this, "testing 123");
            lv_.Dock = DockStyle.Fill;
            this.Controls.Add(lv_);
            lv_.show_name = false;

            lv_.set_log( new log_reader( new log_parser(new file_text_reader(sett))) );
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

        public void on_edit_aliases() {
        }

        public string matched_logs(int line_idx) {
            return "-matched";
        }

        public Rectangle client_rect_no_filter {
            get {
                return new Rectangle();
            }
        }

        public ui_info global_ui_copy {
            get {
                return new ui_info();
            }
        }

        public log_view full_log { get; private set; }

        public void simple_action(log_view_right_click.simple_action simple) {
        }

        public void add_or_edit_filter(string filter_str, string filter_id, bool apply_to_existing_lines) {
        }

        public void sel_changed(log_view_sel_change_type change) {
        }

        public void select_filter_rows(List<int> filter_row_indexes) {
        }

        public void edit_filter_row(int filter_row_idx) {
        }

        public List<Tuple<string, int>> other_views_containing_this_line(int row_idx) {
            return new List<Tuple<string, int>>();
        }

        public void go_to_view(int view_idx) {
        }

        public Tuple<Color, Color> full_log_row_colors(int line_idx) {
            return new Tuple<Color, Color>(util.transparent, util.transparent);
        }

        public void after_set_filter_update() {
        }

        public int selected_filter_row_index {
            get { return -1; }
        }

        public bool can_edit_context {
            get { return true; }
        }

        public bool is_showing_single_view { get; private set; }

        public void edit_log_settings() {
        }

        public void edit_column_formatting() {
        }

        public void after_column_positions_change() {
        }

        public void needs_details_pane() {
        }

        public void on_available_columns_known() {
        }

        public List<info_type> description_columns() {
            return new List<info_type>();
        }

        private void refresh_Tick(object sender, EventArgs e) {
            //refresh.Enabled = false;
            //new settings_form(this).ShowDialog();

            lv_.refresh();
        }
    }
}
