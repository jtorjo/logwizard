using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using lw_common;

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

            Application.Run(new test_notes_ctrl());
//            Application.Run(new test_filter_ctrl());
        }
    }
}
