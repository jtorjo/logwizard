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
        // http://stackoverflow.com/questions/7852824/usercontrol-how-to-add-mousewheel-listener
        // used the one from jeromerg
        private class MouseWheelMessageFilter : IMessageFilter
        {
            [DllImport("user32.dll")]
            private static extern IntPtr WindowFromPoint(Point pt);

            private readonly Control ctrl_;
            private readonly Action<MouseEventArgs> mOnMouseWheel;

            public MouseWheelMessageFilter(Control ctrl, Action<MouseEventArgs> onMouseWheel)
            {
                ctrl_ = ctrl;
                mOnMouseWheel = onMouseWheel;
            }

            public bool PreFilterMessage(ref Message m)
            {
                // handle only mouse wheel messages
                if (m.Msg != 0x20a)
                    return false;

                if (win32.focused_ctrl() != ctrl_)
                    return false;

                MouseButtons buttons = GetMouseButtons( m.WParam.ToInt64());
                long delta = m.WParam.ToInt64() >> 16;

                var e = new MouseEventArgs(buttons, 0, 0, 0, (int)delta);

                mOnMouseWheel(e);

                return true;
            }

            private static MouseButtons GetMouseButtons(long wParam)
            {
                MouseButtons buttons = MouseButtons.None;

                if(HasFlag(wParam, 0x0001)) buttons |= MouseButtons.Left;
                if(HasFlag(wParam, 0x0010)) buttons |= MouseButtons.Middle;
                if(HasFlag(wParam, 0x0002)) buttons |= MouseButtons.Right;
                if(HasFlag(wParam, 0x0020)) buttons |= MouseButtons.XButton1;
                if(HasFlag(wParam, 0x0040)) buttons |= MouseButtons.XButton2;

                return buttons;
            }

            private static bool HasFlag(long input, long flag)
            {
                return (input & flag) == flag;
            }
        }

        public static void add(Control ctrl, Action<MouseEventArgs> onMouseWheel)
        {
            if (ctrl == null || onMouseWheel == null)
                throw new ArgumentNullException();

            var filter = new MouseWheelMessageFilter(ctrl, onMouseWheel);
            Application.AddMessageFilter(filter);
            ctrl.Disposed += (s, e) => Application.RemoveMessageFilter(filter);
        }
    }
}
