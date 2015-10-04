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

            //test_export.test();

            app.inst.init(new settings_file( @"C:\john\code\logwiz\logwizard\bin\x64\Dbg64\logwizard_debug.txt"));

            Application.Run(new test_log_view());
//            Application.Run(new test_notes_ctrl());
//            Application.Run(new test_filter_ctrl());
        }
    }
}
