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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class go_to_line_time_form : Form {
        public go_to_line_time_form(Form parent) {
            InitializeComponent();
            TopMost = parent.TopMost;
        }

        // in this case, it's an offset
        public int time_milliseconds {
            get {
                Debug.Assert( has_offset != '\0');
                Debug.Assert( !is_number() );

                string str = txt_no_offset();

                if (str.EndsWith("s") || str.EndsWith("ms") || str.EndsWith("h") || str.EndsWith("m")) {
                    string last = str.Substring(str.EndsWith("ms") ? str.Length - 2 : str.Length - 1);
                    str = str.Substring(0, str.EndsWith("ms") ? str.Length - 2 : str.Length - 1);
                    double n = double.Parse(str);
                    switch (last) {
                    case "ms":
                        break; 
                    case "s":
                        n *= 1000;
                        break;
                    case "m":
                        n *= 60 * 1000;
                        break;
                    case "h":
                        n *= 60 * 60 * 1000;
                        break;
                    default: Debug.Assert(false);
                        break;
                    }
                    return (int) n;
                }
                    

                int seconds = 0;
                int ms = 0;
                if (str.Contains(("."))) {
                    int sep = str.IndexOf(".");
                    ms = int.Parse( str.Substring(sep + 1));
                    str = str.Substring(0, sep);
                }
                // at this point - just hh:mm:ss
                string[] times = str.Split(':');
                foreach (string time in times) {
                    seconds *= 60;
                    int t = int.Parse(time);
                    seconds += t;
                }

                return seconds * 1000 + ms;
            }
        }

        public DateTime normalized_time {
            get { return util.str_to_normalized_time(txt_no_offset()); }
        }

        private string txt_no_offset() {
            string str = txt.Text;
            str = str.Trim();
            if (str.Length > 0 && (str[0] == '+' || str[0] == '-'))
                str = str.Substring(1);

            // interpret "," as "." - like, 08:22:32,234 is same as 08:22:32.234
            str = str.Replace(",", ".");
            return str;
        }

        // + or - are offsets, '\0' means there is no offset
        public char has_offset {
            get {
                string str = txt.Text;
                str = str.Trim();
                if (str.Length > 0 && (str[0] == '+' || str[0] == '-'))
                    return str[0];
                return '\0';
            }
        }

        public int number {
            get {
                Debug.Assert( is_number() );
                string str = txt_no_offset();
                return int.Parse(str);
            }
        }

        public bool is_number() {
            string str = txt_no_offset();
            if (str == "")
                return false;

            int sep_count = str.Count(c => c == ':');
            int millis_sep_count = str.Count(c => c == '.' || c == ',');

            if (sep_count == 0 && millis_sep_count == 0) {
                // it's a number - it should only have digits
                int ignore = 0;
                return int.TryParse(str, out ignore);
            } else {
                return false;
            }
        }

        private bool is_valid() {
            string str = txt_no_offset();
            if (str == "")
                return false;

            if (str.EndsWith("s") || str.EndsWith("ms") || str.EndsWith("h") || str.EndsWith("m")) {
                // offset in ms/s/h/m
                str = str.Substring(0, str.EndsWith("ms") ? str.Length - 2 : str.Length - 1);
                double ignore;
                return double.TryParse(str, out ignore);
            }

            int digit_count = str.Count(Char.IsDigit);
            int sep_count = str.Count(c => c == ':');
            int millis_sep_count = str.Count(c => c == '.' || c == ',');

            if (sep_count == 0 && millis_sep_count == 0) {
                // it's a number - it should only have digits
                int ignore = 0;
                return int.TryParse(str, out ignore);
            }

            // it's a time or time offset
            if (sep_count == 0)
                // at least one ':' needed
                return false;
            if (sep_count > 2)
                // we can at most have hh:mm:ss
                return false;
            if (digit_count + sep_count + millis_sep_count != str.Length)
                // illegal chars
                return false;
            if (millis_sep_count > 1)
                return false; // only one millis separator at most
            if (Char.IsPunctuation(str.Last()))
                return false;

            return true;
        }

        private void txt_TextChanged(object sender, EventArgs e) {
            ok.Enabled = is_valid();
        }

        private void ok_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
        }

        private void cancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }
    }
}
