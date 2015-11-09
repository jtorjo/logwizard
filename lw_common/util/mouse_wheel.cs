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
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace lw_common {

    public static class mouse_wheel
    {
        public delegate void on_wheel_func(Message m);

        // http://stackoverflow.com/questions/7852824/usercontrol-how-to-add-mousewheel-listener
        // used the one from jeromerg - modified the hell out of it
        private class wheel_filter : IMessageFilter
        {
            private readonly Control ctrl_;

            private on_wheel_func on_wheel_;

            private bool ignore_message_ = false;

            public wheel_filter(Control ctrl, on_wheel_func on_wheel)
            {
                ctrl_ = ctrl;
                on_wheel_ = on_wheel;
            }

            public bool PreFilterMessage(ref Message m)
            {
                // handle only mouse wheel messages
                if (m.Msg != 0x20a)
                    return false;

                if (win32.focused_ctrl() != ctrl_)
                    return false;

                if (ignore_message_)
                    return false;

                ignore_message_ = true;
                on_wheel_(m);
                ignore_message_ = false;

                return true;
            }
        }

        public static void add(Control ctrl, on_wheel_func on_wheel)
        {
            var filter = new wheel_filter(ctrl, on_wheel);
            Application.AddMessageFilter(filter);
            ctrl.Disposed += (s, e) => Application.RemoveMessageFilter(filter);
        }
    }
}
