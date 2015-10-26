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
