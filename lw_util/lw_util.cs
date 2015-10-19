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
