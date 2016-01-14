using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using lw_common;

namespace LwFileAssociations {
    internal static class file_associations_Program {

        /* old
        util.set_association(".log", "Log_file", Application.ExecutablePath, "Log File");
        util.set_association(".txt", "Text_file", Application.ExecutablePath, "Text File");
        // 1.1.5+
        util.set_association(".zip", "Zip_file", Application.ExecutablePath, "Zip File");
        util.set_association(".logwizard", "LogWizard_file", Application.ExecutablePath, "LogWizard File");
        util.create_shortcut("LogWizard", util.roaming_dir() + @"Microsoft\Windows\SendTo", "Send To LogWizard", null, Application.ExecutablePath, null);
        */

        private static string app_name() {
            return util.lw_full_app_name();
        }

        private static void set_default_associations() {
            util.set_association(".logwizard", "Lw_LogWizard_file", app_name(), "LogWizard File", true);

            util.create_shortcut("LogWizard", util.roaming_dir() + @"Microsoft\Windows\SendTo", "Send To LogWizard", null, app_name(), null);            
        }

        private static void set_associations() {
            util.set_association(".log", "Lw_Log_file", app_name(), "Log File", false);
            util.set_association(".txt", "Lw_Text_file", app_name(), "Text File", false);
            // 1.1.5+
            util.set_association(".zip", "Lw_Zip_file", app_name(), "Zip File", false);

            set_default_associations();
        }

        private static void unset_associations() {
            util.un_set_association(".log", "Lw_Log_file", app_name(), "Log File");
            util.un_set_association(".txt", "Lw_Text_file", app_name(), "Text File");
            util.un_set_association(".zip", "Lw_Zip_file", app_name(), "Zip File");

            // old names
            util.un_set_association(".log", "Log_file", app_name(), "Log File");
            util.un_set_association(".txt", "Text_file", app_name(), "Text File");
            util.un_set_association(".zip", "Zip_file", app_name(), "Zip File");
        }

    /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            int count = args.Length > 0 ? int.Parse(args[0]) : 0;
            switch (count) {
            case 0:
                set_default_associations();
                break;

            case 1:
                set_associations();
                break;

            case 2:
                unset_associations();
                break;
            }
        }
    }
}
