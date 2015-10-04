using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class smart_readonly_textbox : TextBox {
        private log_view parent_;

        public smart_readonly_textbox() {
            InitializeComponent();
        }

        public void init(log_view parent) {
            parent_ = parent;
        }

        public void update_ui() {
            if (parent_ == null)
                return;
            
        }
    }
}
