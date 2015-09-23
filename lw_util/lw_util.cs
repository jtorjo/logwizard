using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lw_common;

namespace LogWizard {
    class lw_util {
        public static void bring_to_top(log_wizard form) {
            win32.BringToTop(form);
            /*
            form.BringToFront();
            form.Focus();
            form.Activate();

            form.TopMost = true;
            form.TopMost = false;
            */
        }

        // ... just setting .TopMost sometimes does not work

        public static void bring_to_topmost(log_wizard form) {
            form.TopMost = true;
            form.update_toggle_topmost_visibility();
            win32.MakeTopMost(form);
            /*
            form.TopMost = false;
            form.Activated += FormOnActivated;

            form.BringToFront();
            form.Focus();
            form.Activate();
            */
        }
    }
}
