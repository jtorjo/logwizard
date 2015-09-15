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
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;

namespace LogWizard
{
    static class Program
    {
        private static settings_file sett_ = new settings_file(util.is_debug ? "logwizard_debug.txt" : "logwizard.txt");

        public static settings_file sett {
            get { return sett_; }
        }

        public static string open_file_name {
            get { return open_file_name_; }
        }

        private static string appdata_dir() {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\";
            return path;
        }

        public static string local_dir() {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LogWizard\\";
            return path;
        }

        private static string open_file_name_ = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            //util.test_normalized_times();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            log4net.Config.XmlConfigurator.Configure( new FileInfo("LogWizard.exe.config"));
            util.force_break_into_debugger();

            if (!util.is_debug) {
                Environment.CurrentDirectory = local_dir();
                if ( !File.Exists("logwizard_user.txt"))
                    File.Copy("logwizard.txt", "logwizard_user.txt");
            }
            sett_ = new settings_file(util.is_debug ? "logwizard_debug.txt" : "logwizard_user.txt");

            if (args.Length > 0 && args[0] == "showsample") {
                try {
                    Process.Start( Assembly.GetExecutingAssembly().Location, new FileInfo("LogWizardSetupSample.log").FullName);
                } catch(Exception e) {
                    MessageBox.Show("Exception " + e.Message);
                }
                return;
            }

            if ( args.Length > 0 && File.Exists(args[0]))
                open_file_name_ = args[0];

//            if (args.Length > 0)
  //              MessageBox.Show(args[0]);

            if (open_file_name_ != null)
                wait_for_setup_kit_to_complete();

            util.set_association(".log", "Log_file", Application.ExecutablePath, "Log File");
            util.set_association(".txt", "Text_file", Application.ExecutablePath, "Text File");
            util.create_shortcut("LogWizard", appdata_dir() + @"Microsoft\Windows\SendTo", "Send To LogWizard", null, Application.ExecutablePath, null);

            Application.Run(new Dummy());
        }

        private static void wait_for_setup_kit_to_complete() {
            Process setup = find_kit("Log Wizard Setup");
            while ( setup != null && !setup.HasExited)
                Thread.Sleep(100);
        }

        private static Process find_kit(string title) {
            foreach (Process p in Process.GetProcesses())
                if (p.MainWindowTitle.StartsWith(title)) 
                    return p;
            return null;
        }
    }
}
