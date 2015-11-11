using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace test_ui {
    public partial class test_status_ctrl : Form {
        public test_status_ctrl() {
            InitializeComponent();
            status.set_text("te<fg red>sti<bg yellow>ng<a http://jtorjo.com>  12</fg>3\r\n456\r\n78</bg>9");
        }

        private void status_MouseMove(object sender, MouseEventArgs e) {

        }
    }
}
