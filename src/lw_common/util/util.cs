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
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ColorSchemeExtension;
using href.Utils;
using log4net;
using Microsoft.Win32;
using Timer = System.Windows.Forms.Timer;

namespace lw_common {

    // http://stackoverflow.com/questions/3874134/cleaning-up-code-littered-with-invokerequired
    public static class ControlHelpers
    {
        public static void async_call_and_wait(this Control control, Action action)
        {
            if (control == null)
                return;

            try {
                if (control.InvokeRequired)
                    control.Invoke( new Action(() => { action(); })  );
                else
                    action();
            } catch (ObjectDisposedException) {
                // can happen when calling something from thread X while UI thread is being closed
            }
        }

        public static void async_call(this Control control, Action action) {
            if (control == null)
                return;

            try {
                if (control.InvokeRequired)
                    control.BeginInvoke(new Action(() => { action(); }));
                else
                    action();
            } catch (ObjectDisposedException) {
                // can happen when calling something from thread X while UI thread is being closed
            }
        }
    }

    public class util {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // made rather huge, just in case users forget to send us the logs, but at a rather later time when they mention a bug,
        // we really want everything - by default, we'll send much less logs.
        private const int MAX_OLD_LOGS = 25;

        public readonly static char[] any_enter_char = new[] {'\r', '\n'};

        // this way I can emulate "release" behavior in debug mode - I want to avoid using #ifs as much as possible
#if DEBUG
        private static bool is_debug_ = true;
#else
        private static bool is_debug_ = false;
#endif
        public static bool is_debug {
            get { return is_debug_; }
            set { is_debug_ = value; }
        }

        /* I want a failed assertion to break into debugger right away.
         * 
         * I don't want a stack trace since I can easily get one. The problem with showing a message box
         * is that that windows loop keeps going while showing hte assertion, which can cause further assertions, and all
         * sorts of other problems, making close to impossible to see what the original problem was
         */
        private class break_into_debugger : DefaultTraceListener {
            public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
            {
                Debugger.Break();
            }
            public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
            {
                Debugger.Break();
            }
            public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
            {
                Debugger.Break();
            }
            public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
            {
                Debugger.Break();
            }
            public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
            {
                Debugger.Break();
            }
            public override void Fail(string message)
            {
                 Debugger.Break();
            }
            public override void Fail(string message, string detailMessage)
            {
                Debugger.Break();
            }            
        }

        public static void force_break_into_debugger() {
#if DEBUG
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new break_into_debugger());
#endif
        }

        public static DateTime str_to_time(string str) {
            string format = "HH:mm:ss";
            if (str.Length > 8)
                format += str[8];
            if ( str.Length > 9)
                format += new string('f', str.Length - 9);
            DateTime dt = DateTime.Now;
            if (!DateTime.TryParseExact(str, format, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dt))
                dt = DateTime.Now;
            return dt;
        }

        public static Color grayer_color(Color col) {
            var argb = col.ToArgb();
            if (argb == Color.Blue.ToArgb())
                return Color.DodgerBlue;
            if (argb == Color.Green.ToArgb())
                return Color.LightGreen;
            if (argb == Color.Red.ToArgb())
                return Color.LightCoral;
            if (argb == Color.Pink.ToArgb())
                return Color.LightPink;
            if ( argb == Color.Black.ToArgb())
                return Color.Gray;
            if (argb  == Color.Gray.ToArgb())
                return Color.DarkGray;

            ColorEx ex = new ColorEx(col);
            ex.S = (byte)(ex.S / 5);
            return ex.Color;
        }

        public static Color darker_color(Color col) {
            var argb = col.ToArgb();
            if (argb  == Color.White.ToArgb())
                return Color.WhiteSmoke;
            if (argb  == Color.WhiteSmoke.ToArgb())
                return Color.LightGray;
            if (argb  == Color.LightGray.ToArgb())
                return Color.DarkGray;
            if (argb  == Color.DarkGray.ToArgb())
                return Color.Gray;

            ColorEx ex = new ColorEx(col);
            const double mul_by = 2.3;
            ex.S = (byte)(ex.S * mul_by > 100 ? 100 : ex.S * mul_by);
            Color darker = ex.Color;

            if ( darker.ToArgb() == col.ToArgb())
                // basically, we never want to return the same color
                return grayer_color(col);

            return darker;
        }

        public static string read_beginning_of_file(string file, int len) {
            try {
                var encoding = file_encoding(file);
                if (encoding == null)
                    return "";

                var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fs.Seek(0, SeekOrigin.Begin);
            
                // read a few lines from the beginning
                byte[] readBuffer = new byte[len];
                int bytes = fs.Read(readBuffer, 0, len);
                
                string now = encoding.GetString(readBuffer, 0, bytes);
                return now;
            } catch {
                return "";
            }
        }

        public static readonly Color transparent = Color.FromArgb(0,0,0,0);

        private static Color str_to_namecolor(string s) {
            s = s.ToLower();
            bool light = s.StartsWith("light");
            bool dark = s.StartsWith("dark");
            if (light)
                s = s.Substring(5);
            if (dark)
                s = s.Substring(4);

            switch (s.ToLower()) {
            case "red":
                return light ? transparent : dark ? Color.DarkRed : Color.Red;
            case "blue":
                return light ? Color.LightBlue : dark ? Color.DarkBlue : Color.Blue;
            case "yellow":
                return light ? Color.LightYellow : dark ? transparent : Color.Yellow;
            case "green":
                return light ? Color.LightGreen : dark ? Color.DarkGreen : Color.Green;
            case "white":
                return light ? transparent : dark ? transparent : Color.White;
            case "black":
                return light ? transparent : dark ? transparent : Color.Black;
            case "orange":
                return light ? transparent : dark ? Color.DarkOrange : Color.Orange;
            case "coral":
                return light ? Color.LightCoral : dark ? transparent : Color.Coral;
            case "gray":
                return light ? Color.LightGray : dark ? Color.DarkGray : Color.Gray;
            case "cyan":
                return light ? Color.LightCyan : dark ? Color.DarkCyan : Color.Cyan;
            case "violet":
                return light ? transparent : dark ? Color.DarkViolet : Color.Violet;
            case "pink": 
                return light ? Color.LightPink : dark ? transparent : Color.Pink;
            case "whitesmoke":
                return light ? transparent : dark ? transparent : Color.WhiteSmoke;

            case "brown": 
                return light ? Color.SandyBrown : dark ? Color.SaddleBrown : Color.Brown;

//            case "green":
  //              return light ? Color.LightGreen : dark ? Color.DarkGreen : Color.Green;


            case "transparent":
                return transparent;
            }

            return transparent;
        }

        public static Color str_to_color(string s) {
            Color name = str_to_namecolor(s);
            if (name != transparent)
                return name;

            if (s[0] == '#')
                s = s.Substring(1);

            if (s.Length == 6) {
                try {
                    string r = s.Substring(0, 2);
                    string g = s.Substring(2, 2);
                    string b = s.Substring(4, 2);
                    int ri = Convert.ToInt16(r, 16);
                    int gi = Convert.ToInt16(g, 16);
                    int bi = Convert.ToInt16(b, 16);
                    return Color.FromArgb(ri, gi, bi);
                } catch {}
            }

            return transparent;
        }

        public static string color_to_str(Color c) {
            if (c == transparent)
                return "transparent";
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static Color select_color_via_dlg() {
            var select = new ColorDialog();
            select.Color = transparent;
            if ( select.ShowDialog() == DialogResult.OK)
                if (select.Color.ToArgb() != transparent.ToArgb())
                    return select.Color;

            return transparent;
        }

        public enum beep_type {
            err, question
        }

        public static void beep(beep_type type) {
            switch (type) {
            case beep_type.err:
                System.Media.SystemSounds.Asterisk.Play();
                break;
            case beep_type.question:
                System.Media.SystemSounds.Question.Play();
                break;
            default:
                Debug.Assert(false);
                break;
            }
        }

        // sometimes we get bad-written enters (mostly from xml deserializing) - normalize them 
        public static string normalize_deserialized_enters(string value) {
            if (value.Contains("\r"))
                return value.Replace("\n", "").Replace("\r", "\r\n");
            else
                return value.Replace("\n", "\r\n");
        }


        private static char[] time_ms_separators = new[] { '.', ',', ':' };
        // normalizes the time, so that no matter what input, it will take out hh:mm:ss.zzz
        //
        // assumes time is in correct form: [h]h:[m]m[:[s]s[.z[z[z]]]]
        public static string normalize_time_str(string time) {
            // milliseconds after "."
            time = time.Replace(',', '.');

            // 1.3.11+ recognizes times such as 15:30:46(031) or 15:30:46[043]
            int sep_bracket = time.IndexOf('('), sep_square_bracket = time.IndexOf('[');
            if (sep_bracket >= 0 || sep_square_bracket >= 0) 
                time = time.Replace(sep_bracket >= 0 ? '(' : '[', '.').Replace(sep_bracket >= 0 ? ")" : "]" , "");
            
            // in this case, just hh:mm
            if (time.Count(c => c == ':') == 1) {
                int sep0 = time.IndexOf(':');
                bool has_zzz_sep = time.IndexOfAny(time_ms_separators, sep0 + 1) >= 0;
                if (has_zzz_sep)
                    // weird, it's an mm:ss.zzz ?
                    time = "00:" + time;
                else
                    // it's hh:mm
                    time = time + ":00";
            }

            int sep1 = time.IndexOf(':');
            bool ok = sep1 >= 1 && sep1 <= 2;
            if (!ok)
                // invalid time string
                return time;

            if (sep1 == 1)
                time = "0" + time;
            // hh:mm:ss or hh:m:s
            int sep2 = time.IndexOf(':', 3);
            if (!(sep2 == -1 || sep2 == 4 || sep2 == 5))
                // invalid string
                return time;

            if (sep2 == 4)
                time = time.Substring(0, 3) + "0" + time.Substring(3);
            if (sep2 > 0) {
                // look for seconds:  hh:mm:ss.zzz or hh:mm:s.zzz
                int sep3 = time.IndexOfAny( time_ms_separators, 6);
                ok = (sep3 == -1 || sep3 == 7 || sep3 == 8);
                if (!ok)
                    return time;
                if (sep3 == 7 || time.Length == 7)
                    time = time.Substring(0, 6) + "0" + time.Substring(6);
            } else
                // in this case, it was just mm:ss - should never happen
                Debug.Assert(false);

            switch (time.Length) {
            case 8:
                time = time + ".000";
                break;
            case 10:
                time = time + "00";
                break;
            case 11:
                time = time + "0";
                break;
            case 12:
                break;
            default:
                Debug.Assert(false);
                break;
            }
            
            if ( time.Length > 8 && time[8] != '.')
                time = time.Substring(0,8) + "." + time.Substring(9);
            
            return time;
        }

        public static DateTime str_to_normalized_time(string time_str) {
            Debug.Assert(time_str != null && time_str != "");
            DateTime time = DateTime.MinValue;
            time_str = normalize_time_str(time_str);
            DateTime.TryParseExact( time_str, "HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out time);
            return time;
        }

        // checks really fast if this is a timestamp (does not perform a full check, just looks for some key facts)
        //
        // log4net: 2015-11-07T01:15:54.3091773+02:00 or 2015-11-07T01:15:54.3091773
        public static bool is_timestamp_fast(string str) {
            bool is_ = str.Length == 27 || str.Length == 33;
            if (is_)
                is_ = str[10] == 'T';
            if (is_)
                is_ = str[13] == ':' && str[16] == ':';
            return is_;
        }

        // splits timestamp into date and time
        public static Tuple<string, string> split_timestamp(string str) {
            Debug.Assert(is_timestamp_fast(str));

            string date = str.Substring(0, 10);
            // 1.4.8+ note: at this time, we only process/allow 3 digits passed second
            string time = str.Substring(11, 12); 
            return new Tuple<string, string>(date, time);
        }

        /*
        private static void test_normalize_time(string a, string b) {
            Console.WriteLine("Testing " + normalize_time(a) + " / " + b);
            Debug.Assert( normalize_time(a) == b);
        }
        public static void test_normalized_times() {
            test_normalize_time("3:5", "00:03:05.000");
            test_normalize_time("11:5", "00:11:05.000");
            test_normalize_time("3:15", "00:03:15.000");
            test_normalize_time("3:5.7", "00:03:05.700");

            test_normalize_time("12:32:1", "12:32:01.000");
            test_normalize_time("2:3:5", "02:03:05.000");
            test_normalize_time("02:3:5", "02:03:05.000");
            test_normalize_time("2:03:5", "02:03:05.000");
            test_normalize_time("2:03:05", "02:03:05.000");

            test_normalize_time("2:3:5.0", "02:03:05.000");
            test_normalize_time("2:3:5.01", "02:03:05.010");
            test_normalize_time("2:3:5.011", "02:03:05.011");

            test_normalize_time("2:3:5,0", "02:03:05.000");
            test_normalize_time("2:3:5,01", "02:03:05.010");
            test_normalize_time("2:3:5,011", "02:03:05.011");

            test_normalize_time("2:3:5,1", "02:03:05.100");
            test_normalize_time("2:3:5,12", "02:03:05.120");
            test_normalize_time("2:3:5,123", "02:03:05.123");
        }*/

        public delegate bool update_control_func();
        public delegate void void_func();

        public static void add_timer(update_control_func updater, int refresh_ms = 100) {
            if (!updater())
                return;

            Timer t = new Timer(){ Interval = refresh_ms };
            t.Tick += (sender, args) => {
                if ( !updater()) {
                    t.Enabled = false;
                    t.Dispose();
                }
            };
            t.Enabled = true;
        }

        // update_ms = how long to set_aliases the control visually
        public static void add_timer(update_control_func updater, int update_ms, int refresh_ms = 100) {
            DateTime end = DateTime.Now.AddMilliseconds(update_ms);
            add_timer( () => updater() || (DateTime.Now > end), refresh_ms );
        }

        // postpones executing this function
        public static void postpone(void_func f, int time_ms) {
            Timer t = new Timer(){ Interval = time_ms };
            t.Tick += (sender, args) => {
                t.Enabled = false;
                t.Dispose();
                f();
            };
            t.Enabled = true;
        }

        public static string add_dots(string s, int max_dots) {
            string max_dots_str = new string('.', max_dots);
            if (s.EndsWith(max_dots_str))
                s = s.Substring(0, s.Length - max_dots);
            else
                s += ".";
            return s;
        }


        /*
        private static void FormOnActivated(object sender, EventArgs eventArgs) {
            var form = sender as Form;
            form.Activated -= FormOnActivated;
            form.TopMost = true;
            win32.MakeTopMost(form);
        }*/

        // taken from http://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
        // + return null if cant' read header
        //
        // alternatives: http://www.architectshack.com/TextFileEncodingDetector.ashx (don't like the licensing)
        //               http://www.codeproject.com/Articles/17201/Detect-Encoding-for-In-and-Outgoing-Text    
        public static Encoding file_encoding(string filename)
        {
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
                return file_encoding(file);
        }

        public static Encoding file_encoding(FileStream file) {
            try {
                // Read the BOM
                var bom = new byte[4];
                int read = 0;
                var pos = file.Position;
                file.Seek(0, SeekOrigin.Begin);
                read = file.Read(bom, 0, 4);
                file.Seek(pos, SeekOrigin.Begin);

                if (read < 4)
                    return null;

                // Analyze the BOM
                if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
                if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
                if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
                if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
                if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

                
                long len = Math.Min(8192, file.Length);
                byte[] buff = new byte[len];
                file.Read(buff, 0, (int)len);

                var detected = EncodingTools.DetectInputCodepage(buff);
                if (!detected.Equals( Encoding.Default))
                    return detected;

                // use user's default
                return Encoding.Default;
            } catch {
                return null;
            }            
        }


        // http://stackoverflow.com/questions/2681878/associate-file-extension-with-application
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
        public static void set_association(string Extension, string KeyName, string OpenWith, string FileDescription)
        {
            try {
                RegistryKey BaseKey;
                RegistryKey OpenMethod;
                RegistryKey Shell;
                RegistryKey CurrentUser;

                BaseKey = Registry.ClassesRoot.CreateSubKey(Extension);
                BaseKey.SetValue("", KeyName);

                OpenMethod = Registry.ClassesRoot.CreateSubKey(KeyName);
                OpenMethod.SetValue("", FileDescription);
                OpenMethod.CreateSubKey("DefaultIcon").SetValue("", "\"" + OpenWith + "\",0");
                Shell = OpenMethod.CreateSubKey("Shell");
                Shell.CreateSubKey("edit").CreateSubKey("command").SetValue("", "\"" + OpenWith + "\"" + " \"%1\"");
                Shell.CreateSubKey("open").CreateSubKey("command").SetValue("", "\"" + OpenWith + "\"" + " \"%1\"");
                BaseKey.Close();
                OpenMethod.Close();
                Shell.Close();

                /*
                CurrentUser = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.ucs");
                CurrentUser = CurrentUser.OpenSubKey("UserChoice", RegistryKeyPermissionCheck.ReadWriteSubTree,
                    System.Security.AccessControl.RegistryRights.FullControl);
                CurrentUser.SetValue("Progid", KeyName, RegistryValueKind.String);
                CurrentUser.Close();
                */
                // Delete the key instead of trying to change it
                CurrentUser = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.ucs", true);
                CurrentUser.DeleteSubKey("UserChoice", false);
                CurrentUser.Close();

                // Tell explorer the file association has been changed
                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
            } catch (Exception e) {
                logger.Error("can't set association: " + e.Message);
            }
        }

        public static void create_shortcut(string name, string directory, string description, string iconLocation, string targetPath, string targetArgs)
        {
            try {
                Directory.CreateDirectory(directory);
                string shortcutLocation = System.IO.Path.Combine(directory, name + ".lnk");
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut) shell.CreateShortcut(shortcutLocation);

                shortcut.Description = description; // The description of the shortcut
                if ( iconLocation != null)
                    shortcut.IconLocation = iconLocation; // The icon of the shortcut
                shortcut.TargetPath = targetPath; // The path of the file that will launch when the shortcut is run
                if (targetArgs != null)
                    shortcut.Arguments = targetArgs;
                shortcut.Save(); // Save the shortcut
            } catch(Exception e) {
                MessageBox.Show("Could not create shortcut " + e.Message);
            }
        }

        public static void restart_app() {
            string app_name = Assembly.GetEntryAssembly().Location;
            Application.Exit();
            Thread.Sleep(500);
            Process.Start(app_name);
        }

        // taken from http://msdn.microsoft.com/en-us/library/system.windows.forms.application.setunhandledexceptionmode%28v=vs.110%29.aspx
        //[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public static void init_exceptions() {
            if ( !util.is_debug) {
                Application.ThreadException += new ThreadExceptionEventHandler(on_thread_exc);
                // 2.2.186+ - http://msdn.microsoft.com/en-us/library/system.windows.forms.application.setunhandledexceptionmode%28v=vs.110%29.aspx
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException +=new UnhandledExceptionEventHandler(on_unhandled_exc);
            }
        }
        private static void on_thread_exc(object sender, ThreadExceptionEventArgs e) {
            logger.Fatal("Thread Exception: " + e.Exception.Message + "\r\n" + e.Exception.StackTrace);
        }
        private static void on_unhandled_exc(object sender, UnhandledExceptionEventArgs ue) {
            Exception e = ue.ExceptionObject as Exception;
            logger.Fatal("Unhandled Exception: " + e.Message + "\r\n" + e.StackTrace);
        }

        public static string concatenate<T>(IEnumerable<T> vals, string between) {
            string all = "";
            foreach (var val in vals) {
                if (all != "")
                    all += between;
                all += val;
            }
            return all;
        }

        // if at the end of the string, we consider it's not editing anything
        public static int index_to_line(string s, int test_idx) {
            if (test_idx >= s.Length)
                return -1;

            string[] lines = s.Split(new string[] { "\r\n" }, StringSplitOptions.None );
            int cur_idx = 0;
            for (int i = 0; i < lines.Length; ++i) {
                if (cur_idx + lines[i].Length > test_idx)
                    return i;
                cur_idx += lines[i].Length + 2;
            }
            return lines.Length - 1;
        }











        private static string key_to_action(Keys code, string prefix) {
            if (code == Keys.None)
                return "";

            string s = code.ToString().ToLower();
            if ( code >= Keys.D0 && code <= Keys.D9)
                s = code.ToString().ToLower().Substring(1);
            return prefix + s;
        }

        public static string key_to_action(Keys code) {
            string prefix = "";
            if ((code & Keys.Control) ==  Keys.Control)
                prefix += "ctrl-";
            if ((code & Keys.Shift) ==  Keys.Shift)
                prefix += "shift-";
            if ((code & Keys.Alt) ==  Keys.Alt)
                prefix += "alt-";

            code = code & ~(Keys.Control | Keys.Shift | Keys.Alt);
            return key_to_action(code, prefix);
        }

        public static string key_to_action(PreviewKeyDownEventArgs e) {
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu || e.KeyCode == Keys.ShiftKey)
                return "";
            string prefix = "";
            if (e.Control)
                prefix += "ctrl-";
            if (e.Shift)
                prefix += "shift-";
            if (e.Alt)
                prefix += "alt-";
            return key_to_action(e.KeyCode, prefix);
        }
        public static string key_to_action(KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu || e.KeyCode == Keys.ShiftKey)
                return "";
            string prefix = "";
            if (e.Control)
                prefix += "ctrl-";
            if (e.Shift)
                prefix += "shift-";
            if (e.Alt)
                prefix += "alt-";
            return key_to_action(e.KeyCode, prefix);
        }

        public static string md5_hash(string msg) {
            return md5_hash(Encoding.ASCII.GetBytes(msg));
        }

        public static string md5_hash(byte[] msg)
        {
            var md5 = new MD5CryptoServiceProvider();
            var hashBytes = md5.ComputeHash(msg);
            var hash = new StringBuilder();

            foreach (var b in hashBytes)
                hash.AppendFormat("{0:x2}", b);
            return hash.ToString();
        }

        public static void del_dir(string dir) {
            try {
                Directory.Delete(dir, true);
            } catch {
            }            
        }

        public static string create_temp_dir(string parent_dir) {
            string dir = Path.Combine( parent_dir , "" + DateTime.Now.Ticks);
            try {
                Directory.CreateDirectory(dir);
            } catch(Exception e) {
                logger.Fatal("can't create dir " + dir + " : " + e.Message);
            }
            return dir;
        }

        public static bool create_dir(string dir) {
            try {
                Directory.CreateDirectory(dir);
                return true;
            } catch(Exception e) {
                logger.Fatal("can't create dir " + dir + " : " + e.Message);
                return false;
            }
        }

        public static void create_file(string file, string text) {

            try {
                create_dir(new FileInfo(file).DirectoryName);
                File.WriteAllText(file, text);
            } catch (Exception e) {
                logger.Fatal("can't create file " + file + " : " + e.Message);
            }
        }

        /*  my solution does not work . however, the code in teh below link might work. I may import it someday
        http://stackoverflow.com/questions/3018002/c-how-to-use-shopenfolderandselectitems

        public static void open_in_explorer(List<string> names) {
            Debug.Assert(names.Count > 0);

            try {
                string cmd = "\"" + concatenate(names, "\",\"") + "\"";
                Process.Start("explorer", "/select," + cmd );
            } catch (Exception e) {
                logger.Error("can't open in explorer " + e.Message);
            }            
        }
        */
        public static void open_in_explorer(string name) {
            try {
                Process.Start("explorer", "/select,\"" + name + "\"");
            } catch (Exception e) {
                logger.Error("can't open in explorer " + e.Message);
            }            
        }

        public static string remove_disallowed_filename_chars(string file_name) {
            foreach ( char c in "|\\/:*?\"<>|")
                file_name = file_name.Replace("" + c, "");

            return file_name;
        }

        public static string friendly_size(long size) {
            if (size > 2 * 1024 * 1024)
                return (size / (1024 * 1024)) + " MB";
            if (size > 10 * 1024 )
                return "" + (size / (1024 )) + " kb";
            return "" + size + " bytes";
        }

        public static int matched_string_index(string msg, List<string> search) {
            int idx = 0;
            foreach (string s in search) {
                if (msg.Contains(s))
                    return idx;

                ++idx;
            }

            return -1;
        }

        public static void suspend_layout(Control c, bool suspend) {
            bool suspend_this = 
                c is Form 
                || c is ContextMenuStrip 
                || c is SplitterPanel
                || c is SplitContainer
                || c is TabControl
                || c is TabPage
                ;

            if (suspend_this && suspend)
                c.SuspendLayout();

            foreach ( Control child in c.Controls)
                suspend_layout(child, suspend);

            if (suspend_this && !suspend)
                c.ResumeLayout(true);
        }

        public static List<int> find_all_matches(string text, string search_for) {
            Debug.Assert(search_for != null);
            List<int> all = new List<int>();
            if (search_for == "")
                return all;

            int at = 0;
            while (true) {
                int next = text.IndexOf(search_for, at);
                if (next != -1) {
                    all.Add(next);
                    at = next + search_for.Length;
                } else 
                    break;
            }
            return all;
        }

        public static ToolStripDropDownDirection menu_direction(ContextMenuStrip menu, Point mouse) {
            var screen = Screen.AllScreens.FirstOrDefault(s => s.Bounds.Contains(mouse));
            if (screen == null)
                // normally we should never get here
                return ToolStripDropDownDirection.Default;

            Rectangle r = screen.WorkingArea;
            int w = menu.Width, h = menu.Height;
            bool to_right = mouse.X + w <= r.Right;
            bool to_bottom = mouse.Y + h <= r.Bottom;

            if (to_right)
                return to_bottom ? ToolStripDropDownDirection.BelowRight : ToolStripDropDownDirection.AboveRight;
            else
                return to_bottom ? ToolStripDropDownDirection.BelowLeft : ToolStripDropDownDirection.AboveLeft;
        }

        public static List<bool> to_list(BitArray ba) {
            List<bool> list = new List<bool>();
            for (int i = 0; i < ba.Count; ++i)
                list.Add(ba[i]);
            return list;
        } 

        public static void append_line(ref string txt, string line) {
            if (!txt.EndsWith("\r\n"))
                txt += "\r\n";
            txt += line;
        }

        // not very efficient or pretty, but for now gets the job done
        public static string normalize_enters(string txt) {
            const string SHOULD_NEVER_EXIST = "$$$$----$$$$----$$$$%%%%$$$$----$$$$$$$$----$$$$";
            txt =
                txt.Replace("\r\n", SHOULD_NEVER_EXIST)
                   .Replace("\n\r", SHOULD_NEVER_EXIST)
                   .Replace("\r", SHOULD_NEVER_EXIST)
                   .Replace("\n", SHOULD_NEVER_EXIST)
                   .Replace(SHOULD_NEVER_EXIST, "\r\n");
            return txt;
        }


        public static void bring_to_top(Form form) {
            win32.BringToTop(form);
            /*
            form.BringToFront();
            form.Focus();
            form.Activate();

            form.TopMost = true;
            form.TopMost = false;
            */
        }

        private static List<string> copy_logs_errors = new List<string>();
        private static void copy_former_logs(string prefix) {
            // at this point, logging is NOT initialized yet
            string suffix = ".log";
            prefix += suffix;

            try {
                string last = prefix + "." + (MAX_OLD_LOGS+1) + suffix;
                if ( File.Exists(last))
                    File.Delete(last);
            }
            catch (Exception e) {
                copy_logs_errors.Add("Error while erasing last file :" + e.Message); 
            }
            for ( int i = MAX_OLD_LOGS; i >= 0; --i)
                try {
                    string now = prefix + (i > 0 ? "." + i + suffix : "") ;
                    string next = prefix + "." + (i+1) + suffix;
                    if ( File.Exists(now))
                        File.Move(now, next);
                }
                catch(Exception e) {
                    copy_logs_errors.Add("Error while copying log " + i + ":" + e.Message); 
                }
            try {
                string last = prefix + "." + (MAX_OLD_LOGS+1);
                if ( File.Exists(last))
                    File.Delete(last);
            }
            catch (Exception e) {
                copy_logs_errors.Add("Error while erasing last file :" + e.Message); 
            }
        }

        public static void create_backup(string prefix, string suffix, int count) {
            prefix += suffix;

            try {
                string last = prefix + "." + (count+1) + suffix;
                if ( File.Exists(last))
                    File.Delete(last);
            }
            catch (Exception e) {
                logger.Error("Error while backing up last file :" + e.Message); 
            }
            for ( int i = count; i >= 0; --i)
                try {
                    string now = prefix + (i > 0 ? "." + i + suffix : "") ;
                    string next = prefix + "." + (i+1) + suffix;
                    if ( File.Exists(now))
                        if ( i > 0)
                            File.Move(now, next);
                        else 
                            File.Copy(now, next);
                }
                catch(Exception e) {
                    logger.Error("Error while backin file " + i + ":" + e.Message); 
                }
            try {
                string last = prefix + "." + (count+1);
                if ( File.Exists(last))
                    File.Delete(last);
            }
            catch (Exception e) {
                logger.Error("Error while erasing last file :" + e.Message); 
            }
        }

        public static string personal_dir() {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\";
            return path;
        }
        public static string roaming_dir() {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\";
            return path;
        }

        public static string appdata_dir() {
            // 1.6.5+ use the Local dir, instead of Roaming - https://github.com/jtorjo/logwizard/issues/3
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) ;
            return path;
        }

        public static string local_dir() {
            string path = appdata_dir() + "\\LogWizard\\";
            return path;
        }
        private static string old_local_dir() {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LogWizard\\";
            return path;
        }


        static public void set_current_dir() {
             if (!is_debug) {
                 try {
                     Directory.CreateDirectory(local_dir());
                     // 1.6.10+ use the Local dir, instead of Roaming - https://github.com/jtorjo/logwizard/issues/3
                     if (File.Exists(old_local_dir() + "logwizard_user.txt")) 
                         File.Move( old_local_dir() + "logwizard_user.txt", local_dir() + "logwizard_user.txt");
                     
                     // 1.6.11+ - right now, these files are copied into the Documents dir
                     File.Copy(personal_dir() + "LogWizard\\logwizard.txt", local_dir() + "logwizard.txt", true);
                     File.Copy(personal_dir() + "LogWizard\\lw.config", local_dir() + "lw.config", true);
                 } catch {
                 }

                 Environment.CurrentDirectory = local_dir();
                try {
                    if (!File.Exists("logwizard_user.txt"))
                        File.Copy("logwizard.txt", "logwizard_user.txt");
                } catch {}
            }            
        }

        static public void init_log() {
            copy_former_logs("LogWizard");

            log4net.Config.XmlConfigurator.Configure( new FileInfo(util.is_debug ? "LogWizard.exe.config" : "lw.config"));
            var logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            logger.Info("logging initialized, cur dir is " + Environment.CurrentDirectory );
            foreach ( string err in copy_logs_errors)
                logger.Error("Copy_logs Error: " + err);             
        }

        // makes sure the returned names does not match any of the existing names
        public static string unique_name(IEnumerable<string> existing_names, string name, string name_suffix = "_") {
            if (!existing_names.Contains(name))
                return name;

            // at this point, we know name exists, find a suffix
            int idx = 2;
            while (true) {
                string copy = name + name_suffix + idx;
                if (!existing_names.Contains(copy))
                    return copy;
                ++idx;
            }
        }

        public enum split_into_lines_type {
            include_enter_chars_in_returned_lines, exclude_enter_chars_in_returned_lines
        }

        public static string[] split_into_lines(string text, split_into_lines_type split_type) {
            List<string> lines = new List<string>();

            while (text.Length > 0) {
                int found = text.IndexOfAny(any_enter_char);
                if (found < 0) {
                    lines.Add(text);
                    break;
                }
                // we have a line
                int next = text.IndexOfAny(any_enter_char, found + 1);
                bool double_char_enter = next == found + 1 && text[found] != text[next];
                string cur = text.Substring(0, split_type == split_into_lines_type.include_enter_chars_in_returned_lines ? found + (double_char_enter ? 2 : 1) : found);
                lines.Add(cur);

                text = text.Substring(found + (double_char_enter ? 2 : 1));
            }

            return lines.ToArray();
        }

        // forces a text into multiple lines, each line having line_char_count characters
        public static string split_into_multiple_fixed_lines(string txt, int line_char_count) {
            string result = "";
            for (int i = 0; i < txt.Length; i += line_char_count) {
                int len = i + line_char_count <= txt.Length ? line_char_count : txt.Length - i;
                result += txt.Substring(i, len) + ( i + line_char_count < txt.Length ? "\r\n" : "");
            }
            return result;
        }

    }
}
