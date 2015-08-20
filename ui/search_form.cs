using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogWizard.ui {
    public partial class search_form : Form {

        public class search_for {
            public string text = "";
            public Color fg = util.transparent, bg = util.transparent;
            public bool mark_lines_with_color = false;
        }
        private search_for search_ = new search_for();
        private settings_file sett = Program.sett;

        public search_form() {
            InitializeComponent();
            fg.BackColor = util.str_to_color( sett.get("search_fg", "transparent"));
            bg.BackColor = util.str_to_color( sett.get("search_bg", "#faebd7") ); // antiquewhite
            txt.Text = sett.get("search_text");
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
                sett.set("search_text", txt.Text);
                sett.save();
                search_ = new search_for {text = txt.Text, bg = bg.BackColor, fg = fg.BackColor, mark_lines_with_color = mark.Checked};
                DialogResult = DialogResult.OK;
            }
        }

        private void cancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }
    }
}
