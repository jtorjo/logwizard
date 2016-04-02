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
 *  * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using lw_common;
using lw_common.ui;

namespace test_ui {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            log4net.Config.XmlConfigurator.Configure( new FileInfo("test_ui.exe.config"));
            util.force_break_into_debugger();
            util.init_exceptions();            

            //test_export.test();
            /* works
            var git = new read_github_release( File.ReadAllText("github.txt"));
            var git = new read_github_release( "jtorjo", "logwizard");
            var s1 = util.concatenate( git.stable_releases("1.0"), "\r\n");
            var s2 = util.concatenate( git.beta_releases("1.2"), "\r\n");
            var s3 = util.concatenate( git.beta_releases(), "\r\n");
            */


            app.inst.init(new settings_file( @"C:\john\code\logwiz\logwizard\bin\x64\Dbg64\logwizard_debug.txt"));

//            new edit_log_settings_form("", edit_log_settings_form.edit_type.add).ShowDialog();

//            Application.Run(new test_description_ctrl());
//            Application.Run(new test_status_ctrl());
//            Application.Run(new test_olv());
//            Application.Run(new test_edit_ctrl());
//            Application.Run(new test_log_view());
//            Application.Run(new test_notes_ctrl());
//            Application.Run(new test_filter_ctrl());
//            Application.Run(new test_categories_ctrl());
            Application.Run(new test_snoop_form());
        }
    }
}
