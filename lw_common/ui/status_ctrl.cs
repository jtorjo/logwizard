using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class status_ctrl : UserControl {
        public status_ctrl() {
            InitializeComponent();
        }

        public void set_text(string text) {
            // convert <a> to links etc., color?
        }

        internal void set_text(List<Tuple<int, int, print_info>> text) {
            
        }
    }

}
