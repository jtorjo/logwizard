/* 
 * Copyright (C) 2014-2015 John Torjo
 *
 * This file is part of LogWizard
 *
 * LogWizard is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * LogWizard is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact john.code@torjo.com 
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class select_zip_file_form : Form {
        private string selected_file_ = "";

        private class item {
            public string file = "";

            public string size_str {
                get { return util.friendly_size(size); }
            }

            public long size = 0;
        }

        public select_zip_file_form(string zip_file, IEnumerable< Tuple<string,long>> files ) {
            InitializeComponent();

            foreach (var file in files) {
                item i = new item() { file = file.Item1, size = file.Item2 };
                list.AddObject(i);
            }
            list.SelectedIndex = 0;
            util.postpone( () => list.Focus(), 10);
        }

        public string selected_file {
            get { return selected_file_; }
        }

        private void ok_Click(object sender, EventArgs e) {
            if (list.SelectedIndex >= 0)
                selected_file_ = (list.GetItem(list.SelectedIndex).RowObject as item).file;

            DialogResult = selected_file_ != "" ? DialogResult.OK : DialogResult.Cancel;
        }

        private void cancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e) {
            if (list.SelectedIndex >= 0)
                ok.Enabled = true;
        }
    }
}
