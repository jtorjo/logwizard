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
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ColorSchemeExtension;

namespace LogWizard {
    class util {


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
            ColorEx ex = new ColorEx(col);
            ex.S = (byte)(ex.S / 5);
            return ex.Color;
        }
        public static Color darker_color(Color col) {
            if (col.ToArgb() == Color.White.ToArgb())
                return Color.WhiteSmoke;
            if (col.ToArgb() == Color.WhiteSmoke.ToArgb())
                return Color.LightGray;
            if (col.ToArgb() == Color.LightGray.ToArgb())
                return Color.DarkGray;
            if (col.ToArgb() == Color.DarkGray.ToArgb())
                return Color.Gray;

            ColorEx ex = new ColorEx(col);
            const double mul_by = 2.3;
            ex.S = (byte)(ex.S * mul_by > 100 ? 100 : ex.S * mul_by);
            return ex.Color;
        }

        public static string read_beginning_of_file(string file, int len) {
            try {
                var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fs.Seek(0, SeekOrigin.Begin);
            
                // read a few lines from the beginning
                byte[] readBuffer = new byte[len];
                int bytes = fs.Read(readBuffer, 0, len);
                string now = System.Text.Encoding.Default.GetString(readBuffer, 0, bytes);
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
            case "transparent":
                return transparent;
            }

            return transparent;
        }

        public static Color str_to_color(string s) {
            Color name = str_to_namecolor(s);
            if (name.ToArgb() != transparent.ToArgb())
                return name;

            if (s[0] == '#')
                s = s.Substring(1);

            if (s.Length == 6) {
                string r = s.Substring(0, 2);
                string g = s.Substring(2, 2);
                string b = s.Substring(4, 2);
                int ri = Convert.ToInt16(r, 16);
                int gi = Convert.ToInt16(g, 16);
                int bi = Convert.ToInt16(b, 16);
                return Color.FromArgb(ri, gi, bi);
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
        public static string normalize_enters(string value) {
            if (value.Contains("\r"))
                return value.Replace("\n", "").Replace("\r", "\r\n");
            else
                return value.Replace("\n", "\r\n");
        }


        // normalizes the time, so that no matter what input, it will take out hh:mm:ss.zzz
        //
        // assumes time is in correct form: [h]h:[m]m[:[s]s[.z[z[z]]]]
        public static string normalize_time_str(string time) {
            // milliseconds after "."
            time = time.Replace(',', '.');

            // in this case, just mm:ss[.zzz]
            if (time.Count(c => c == ':') == 1)
                time = "00:" + time;

            int sep1 = time.IndexOf(':');
            Debug.Assert(sep1 >= 1 && sep1 <= 2);
            if (sep1 == 1)
                time = "0" + time;
            // hh:mm:ss or hh:m:s
            int sep2 = time.IndexOf(':', 3);
            Debug.Assert(sep2 == -1 || sep2 == 4 || sep2 == 5);
            if (sep2 == 4)
                time = time.Substring(0, 3) + "0" + time.Substring(3);
            if (sep2 > 0) {
                // look for seconds:  hh:mm:ss.zzz or hh:mm:s.zzz
                int sep3 = time.IndexOf('.', 6);
                Debug.Assert(sep3 == -1 || sep3 == 7 || sep3 == 8);
                if (sep3 == 7 || time.Length == 7)
                    time = time.Substring(0, 6) + "0" + time.Substring(6);
            } else
                // in this case, it was just mm:ss - should never happen
                Debug.Assert(false);

            switch (time.Length) {
            case 8:
                return time + ".000";
            case 10:
                return time + "00";
            case 11:
                return time + "0";
            case 12:
                return time;
            default:
                Debug.Assert(false);
                return time;
            }
        }

        public static DateTime str_to_normalized_time(string time_str) {
            Debug.Assert(time_str != null && time_str != "");
            DateTime time = DateTime.MinValue;
            time_str = normalize_time_str(time_str);
            DateTime.TryParseExact( time_str, "HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out time);
            return time;
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

        public delegate void update_control_func(bool has_terminated);
        public delegate bool terminate_update_func();

        public static void add_timer(update_control_func updater, terminate_update_func terminator, int refresh_ms = 100) {
            updater(false);

            Timer t = new Timer(){ Interval = refresh_ms };
            t.Tick += (sender, args) => {
                bool has_terminated = terminator();
                updater(has_terminated);
                if (has_terminated) {
                    t.Enabled = false;
                    t.Dispose();
                }
            };
            t.Enabled = true;
        }

        // update_ms = how long to update the control visually
        public static void add_timer(update_control_func updater, int update_ms, int refresh_ms = 100) {
            DateTime end = DateTime.Now.AddMilliseconds(update_ms);
            add_timer( updater, () => (DateTime.Now > end), refresh_ms );
        }

        public void postpone_func(Action a, int postpone_ms) {
            Timer t = new Timer(){ Interval = postpone_ms };
            t.Tick += (sender, args) => {
                t.Enabled = false;
                a();
                t.Dispose();
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

        public static void bring_to_top(Form form) {
            form.BringToFront();
            form.TopMost = true;
            form.TopMost = false;
        }

        // ... just setting .TopMost sometimes does not work
        public static void bring_to_topmost(Form form) {
            form.BringToFront();
            form.TopMost = true;
        }

    }
}
