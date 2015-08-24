using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogWizard.ui {
    public partial class about_form : Form {
        public about_form() {
            InitializeComponent();
        }

        private void jt2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                Process.Start("mailto:john.code@torjo.com");
            } catch {}
        }

        private void jt_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                Process.Start("mailto:john.code@torjo.com");
            } catch {}
        }


        private void link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                Process.Start("https://github.com/jtorjo/logwizard/wiki");
            } catch {}
            
        }
    }
}
