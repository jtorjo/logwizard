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
            status.set_text("te<fg red>sti<bg yellow>ng<a http://torjo.com>[--linky--blinky]</a>  12</fg>3\r\n45<a http://www.codeproject.com>second linky</a> hihi6\r\n78</bg>9");
            animated.animate = true;
            animated.animate_speed_ms = 100;
            animated.animate_interval_ms = 20000;
        }

        private void status_MouseMove(object sender, MouseEventArgs e) {

        }
    }
}
