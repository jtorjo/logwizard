using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lw_common.parse;

namespace test_ui {
    public partial class test_description_ctrl : Form {
        public test_description_ctrl() {
            InitializeComponent();

            aliases a = new aliases("");
            a.on_column_names(new List<string>() { "thread", "time", "msg", "ctx1", "ctx2", "ctx3" });
            description_ctrl1.set_aliases(a);
        }
    }
}
