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
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace lw_common {

    public class win32 {
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;
            public int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        // http://stackoverflow.com/questions/6779731/c-sharp-using-sendmessage-problem-with-wm-copydata
        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT {
          public IntPtr dwData;
          public int cbData;
          [MarshalAs(UnmanagedType.LPStr)] public string lpData;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr p);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalAlloc(int flag, int size);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Point p_point);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        public static Point GetMousePos()
        {
            Point pt = new Point();
            GetCursorPos(ref pt);
            return pt;
        }

        public static void SetMousePos(Point p) {
            SetMousePos(p.x, p.y);
        }
        public static bool SetMousePos(int x, int y) {
            return SetCursorPos(x, y);
        }

        [DllImport("user32.dll", SetLastError = true)] 
        public static extern bool BringWindowToTop(IntPtr hWnd); 
        [DllImport("user32")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("kernel32.dll")]
        public static extern IntPtr FindFirstChangeNotification(string lpPathName, bool bWatchSubtree, uint dwNotifyFilter);

        [DllImport("kernel32.dll")]
        public static extern bool FindNextChangeNotification(IntPtr hChangeHandle);

        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);


        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        public const int WM_LBUTTONDOWN = 0x0201;
        const UInt32 SWP_NOSIZE = 0x0001;

        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        public const int WM_COPYDATA = 0x004A;

        public const uint FILE_NOTIFY_CHANGE_FILE_NAME = 0x00000001;
        public const uint FILE_NOTIFY_CHANGE_SIZE = 0x00000008 ;

        public const UInt32 INFINITE = 0xFFFFFFFF;
        public const UInt32 WAIT_ABANDONED = 0x00000080;
        public const UInt32 WAIT_OBJECT_0 = 0x00000000;
        public const UInt32 WAIT_TIMEOUT = 0x00000102;

        public static void BringToTop(Form form) {
            IntPtr hWnd = form.Handle;

            // backup - hopefully we don't need to do this, but just in case
            BringWindowToTop(hWnd);
            const int SW_SHOW = 5;
            ShowWindow(hWnd, SW_SHOW);

            SetForegroundWindow(hWnd);
            SetForegroundWindow(hWnd);
        }

        public static void MakeTopMost(Form form) {
            BringToTop(form);
            SetWindowPos(form.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }

        [DllImport("user32.dll")]
        private extern static short GetAsyncKeyState(System.Windows.Forms.Keys vKey); 

        public static bool IsKeyPushedDown(System.Windows.Forms.Keys vKey) {
            return 0 != (GetAsyncKeyState(vKey) & 0x8000);
        }

        // http://stackoverflow.com/questions/435433/what-is-the-preferred-way-to-find-focused-control-in-winforms-app
        [DllImport("user32.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)]
        private static extern IntPtr GetFocus();
        public static Control focused_ctrl() {
            Control focusedControl = null;
            IntPtr focusedHandle = GetFocus();
            if(focusedHandle != IntPtr.Zero)
                // Note that if the focused Control is not a .Net control, then this will return null.
                focusedControl = Control.FromHandle(focusedHandle);
            return focusedControl;
        }

    }
}
