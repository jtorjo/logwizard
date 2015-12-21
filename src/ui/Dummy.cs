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
 *  * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using lw_common;

namespace LogWizard
{
    public partial class Dummy : Form
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Dummy()
        {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;

            // so that we can match log files to this (syntax/context)
            logger.Info("Started LogWizard v" + log_wizard.version());

            // the point of this is that we don't close when the first Form is closed
            new log_wizard().Show();
        }

        private void Dummy_VisibleChanged(object sender, EventArgs e) {
            Visible = false;
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == win32.WM_COPYDATA) {
            }

            base.WndProc(ref m);
        }
    }
}
