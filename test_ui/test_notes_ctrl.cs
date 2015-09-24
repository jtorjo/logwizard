using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace test_ui {
    public partial class test_notes_ctrl : Form {
        public test_notes_ctrl() {
            InitializeComponent();
            notes.set_author("John Torjo", "jt", Color.Blue);
        }
    }
}
