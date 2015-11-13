using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lw_common.parse;

namespace lw_common.ui {
    public partial class edit_log_settings_form : Form {
        private settings_as_string old_settings_;
        private settings_as_string settings_;
        private string file_name_;

        private bool needs_restart_ = false;

        private double_dictionary<string,int> type_to_index_ = new double_dictionary<string, int>( new Dictionary<string, int>() {
            { "file", 0 }, { "event_log", 1 }, { "debug_print", 2}, { "db", 3}, {"multi", 4}
        });

        private double_dictionary<string, int> file_type_to_index_ = new double_dictionary<string, int>(new Dictionary<string, int>() {
            { "", 0}, { "line-by-line", 1}, { "part-by-line", 2}, { "xml", 3}, { "csv", 4}
        });
        // file name is set only if it's a file
        public edit_log_settings_form(string settings, string file_name, string friendly_name) {
            old_settings_ = new settings_as_string(settings);
            settings_ = new settings_as_string(settings);
            file_name_ = file_name;

            InitializeComponent();
            
            hide_tabs(typeTab);
            hide_tabs(fileTypeTab);
            cancel.Left = -100;
            friendlyName.Text = friendly_name;

            type.SelectedIndex = type_to_index();
            fileType.SelectedIndex = file_type_to_index( settings_.get("file_type") );

            syntax.Text = settings_.get("syntax");
            ifLine.Checked = settings_.get("line.if_line", "0") != "0";
            
            partSeparator.Text = settings_.get("part.separator");

            xmlDelimeter.Text = settings_.get("xml.delimeter");

            csvHasHeader.Checked = settings_.get("csv.has_header", "1") != "0";
            csvSeparator.Text = settings_.get("csv.separator", ",");
        }

        private int type_to_index() {
            return type_to_index_.key_to_value(settings_.get("type", "file"));
        }

        private string index_to_type() {
            return type_to_index_.value_to_key(type.SelectedIndex);
        }

        private int file_type_to_index(string file_type) {
            return file_type_to_index_.key_to_value(file_type);
        }

        private string index_to_file_type() {
            return file_type_to_index_.value_to_key(fileType.SelectedIndex);
        }

        public string friendly_name {
            get { return friendlyName.Text; }
        }

        public string settings {
            get { return settings_.ToString(); }
        }

        public bool needs_restart {
            get { return needs_restart_; }
        }

        private void hide_tabs(TabControl tab) {
            int page_height = tab.SelectedTab != null ? tab.SelectedTab.Height : tab.TabPages[0].Height;
            int extra = tab.Height - page_height;
            tab.Top -= extra;
            tab.Height += extra;
        }

        private void cancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }

        private void ok_Click(object sender, EventArgs e) {
            settings_.set("type", index_to_type());
            settings_.set("file_type", index_to_file_type());

            settings_.set("syntax", syntax.Text);
            settings_.set("line.if_line", ifLine.Checked ? "1" : "0");
            settings_.set("part.separator", partSeparator.Text);
            settings_.set("xml.delimeter", xmlDelimeter.Text);
            settings_.set("csv.has_header", csvHasHeader.Checked ? "1" : "0");
            settings_.set("csv.separator", csvSeparator.Text);

            if (old_settings_.get("type", "file") != settings_.get("type"))
                needs_restart_ = true;
            if ( settings_.get("type") == "file")
                if (old_settings_.get("file_type") != settings_.get("file_type"))
                    // user changed format, like, from XML to CSV
                    needs_restart_ = true;

            DialogResult = DialogResult.OK;
        }

        private void type_SelectedIndexChanged(object sender, EventArgs e) {
            typeTab.SelectedIndex = type.SelectedIndex;
        }

        private void fileType_SelectedIndexChanged(object sender, EventArgs e) {
            if (fileType.SelectedIndex > 0)
                fileTypeTab.SelectedIndex = fileType.SelectedIndex - 1;
            else {
                // best guess
                fileTypeTab.SelectedIndex = file_type_to_index(factory.guess_file_type(file_name_));
            }
        }
    }
}
