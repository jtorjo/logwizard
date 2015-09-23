using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LogWizard.ui {
    public partial class search_form : Form {

        public class search_for {
            public bool case_sensitive = false;
            public bool full_word = false;
            public string text = "";
            
            public bool use_regex = false;
            public Regex regex = null;

            public Color fg = util.transparent, bg = util.transparent;
            public bool mark_lines_with_color = false;
        }
        private search_for search_ = new search_for();
        private settings_file sett;

        public search_form(Form parent) {
            this.sett = app.inst.sett;
            InitializeComponent();
            fg.BackColor = util.str_to_color( sett.get("search_fg", "transparent"));
            bg.BackColor = util.str_to_color( sett.get("search_bg", "#faebd7") ); // antiquewhite

            caseSensitive.Checked = sett.get("search_case_sensitive", "0") != "0";
            fullWord.Checked = sett.get("search_full_word", "0") != "0";
            int type = int.Parse(sett.get("search_type", "0"));
            switch (type) {
            case 0:
                radioAutoRecognize.Checked = true;
                break;
            case 1:
                radioText.Checked = true;
                break;
            case 2:
                radioRegex.Checked = true;
                break;
                default: Debug.Assert(false);
                break;
            }

            txt.Text = sett.get("search_text");
            update_autorecognize_radio();
            TopMost = parent.TopMost;
        }

        public search_for search {
            get { return search_; }
        }

        private void fg_Click(object sender, EventArgs e) {
            var color = util.select_color_via_dlg();
            if (color.ToArgb() != util.transparent.ToArgb())
                fg.BackColor = color;
        }

        private void bg_Click(object sender, EventArgs e) {
            var color = util.select_color_via_dlg();
            if (color.ToArgb() != util.transparent.ToArgb())
                bg.BackColor = color;
        }

        private void ok_Click(object sender, EventArgs e) {
            if (txt.Text != "") {
                sett.set("search_bg", util.color_to_str(bg.BackColor));
                sett.set("search_fg", util.color_to_str(fg.BackColor));
                sett.set("search_case_sensitive", caseSensitive.Checked ? "1" : "0");
                sett.set("search_full_word", fullWord.Checked ? "1" : "0");
                sett.set("search_text", txt.Text);

                int type = 0;
                if (radioAutoRecognize.Checked) type = 0;
                else if (radioText.Checked) type = 1;
                else if (radioRegex.Checked) type = 2;
                else Debug.Assert(false);

                sett.set("search_type", "" + type);

                sett.save();

                bool use_regex = radioRegex.Checked;
                if (radioAutoRecognize.Checked)
                    use_regex = is_auto_regex();
                Regex regex = null;
                if ( use_regex)
                    try {
                        regex = new Regex(txt.Text);
                    } catch {
                        regex = null;
                    }
                search_ = new search_for {use_regex = use_regex, regex = regex, case_sensitive = caseSensitive.Checked, full_word = fullWord.Checked, text = txt.Text, bg = bg.BackColor, fg = fg.BackColor, mark_lines_with_color = mark.Checked};
                DialogResult = DialogResult.OK;
            }
        }

        private void cancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }

        private void txt_TextChanged(object sender, EventArgs e) {
            update_autorecognize_radio();
        }

        private bool is_auto_regex() {
            bool is_regex = txt.Text.IndexOfAny(new char[] {'[', ']', '(', ')', '\\'}) >= 0;
            return is_regex;
        }

        private void update_autorecognize_radio() {
            radioAutoRecognize.Text = "Auto recognized (" + (is_auto_regex() ? "Regex" : "Text") + ")";
        }
    }
}
