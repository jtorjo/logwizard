/* 
 * Copyright (C) 2014-2016 John Torjo
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
using lw_common.parse;

namespace lw_common.ui {
    public partial class alias_form : Form {
        
        private aliases aliases_;
        private List<string> column_names_;

        // the column names that are unknown
        private string unknown_ = "";

        private bool needs_restart_ = false;

        public alias_form(aliases aliases, List<string> column_names) {
            aliases_ = aliases;
            column_names_ = column_names;
            InitializeComponent();

            string existing = aliases_.to_enter_separated_string();
            if (existing != "")
                text.Text = existing;
            else {
                // try our best guess
                unknown_ = util.concatenate(column_names.Where(x => !aliases.is_known_column_name(x)).Select(x => x + "="), "\r\n");
                text.Text = unknown_;
            }
        }

        public bool needs_restart {
            get { return needs_restart_; }
        }

        public aliases new_aliases {
            get { return aliases_; }
        }

        private void ok_Click(object sender, EventArgs e) {
            if (text.Text.Trim() == unknown_) {
                // user hasn't changed anything
                DialogResult = DialogResult.Cancel;
                return;
            }

            var old = aliases_;
            aliases_ = aliases.from_enter_separated_string(text.Text);

            if (!aliases_.is_non_friendly_name_info_the_same(old))
                needs_restart_ = true;
            DialogResult = DialogResult.OK;
        }

        private void cancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }

        private void text_TextChanged(object sender, EventArgs e) {
            bool needs_restart = !aliases_.is_non_friendly_name_info_the_same(aliases.from_enter_separated_string(text.Text));
            requiresRestart.Visible = needs_restart;
        }
    }
}
