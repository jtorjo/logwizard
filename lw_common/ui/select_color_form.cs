using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lw_common {
    public partial class select_color_form : Form {
        public bool copy_to_clipboard = false;

        public select_color_form() {
            InitializeComponent();
            Location = Cursor.Position;
        }

        public Color SelectedColor {
            get { return picker.SelectedColor; }
            set { picker.SelectedColor = value; }
        }

        private void ok_Click(object sender, EventArgs e) {
            if ( copy_to_clipboard)
                Clipboard.SetText( util.color_to_str(SelectedColor) );
            DialogResult = DialogResult.OK;
        }

        private void cancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }
    }
}
