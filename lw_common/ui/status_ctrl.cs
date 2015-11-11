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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class status_ctrl : UserControl {
        private class part {
            public string text = "";
            public print_info print = new print_info();
            public string link = "";
            public bool ends_line = false;
        }
        private List<part> parts_ = new List<part>();
        // the currently shown line (in case there are several)
        private int cur_line_idx_ = 0;

        private font_list fonts_ = new font_list();

        public status_ctrl() {
            InitializeComponent();
        }

        private void update_cur_line_text() {
            int line_idx = 0;
            List<part> cur_line = new List<part>();
            foreach ( var p in parts_)
                if (line_idx == cur_line_idx_) {
                    cur_line.Add(p);
                    if (p.ends_line)
                        ++line_idx;
                }

            status.Clear();
            string text = util.concatenate(cur_line.Select(x => x.text), "");
            status.AppendText(text);
            int start = 0;
            foreach (var p in cur_line) {
                status.Select(start, p.text.Length);
                if (p.print.fg != util.transparent)
                    status.SelectionColor = p.print.fg;
                if (p.print.bg != util.transparent)
                    status.SelectionBackColor = p.print.bg;
                string font = p.print.font_name != "" ? p.print.font_name : status.Font.Name;
                status.SelectionFont = fonts_.get_font(font, (int)status.Font.Size, p.print.bold, p.print.italic, p.print.underline ) ;
            }
        }

        // <a link> ... </a>
        // <b> ... </b>
        // <i> ... </i>
        // <fg #col>.... </fg>
        // <bg #col>.... </bg>
        // <font name>... </font>
        public void set_text(string text) {
            print_info cur_print = new print_info();
            string cur_link = "";
            while (text != "") {
                int delimeter = text.IndexOf("<");
                if (delimeter > 0) {
                    var new_ = new part { text = text.Substring(0, delimeter), link = cur_link };
                    cur_link = "";
                    new_.print.copy_from(cur_print);
                    parts_.Add(new_);
                    int end_delimeter = text.IndexOf(">", delimeter);
                    string format = text.Substring(delimeter + 1, end_delimeter - delimeter - 1);
                    parse_format(format, ref cur_print, ref cur_link);
                    text = text.Substring(end_delimeter + 1);
                } else {
                    // it was the last part
                    var last = new part { text = text };
                    last.print.copy_from(cur_print);
                    parts_.Add(last);
                    text = "";
                }
            }

            // also, it must allow for multiline (if so, print one line, and scroll at a give time)
            for (int i = 0; i < parts_.Count; ++i) {
                int enter = parts_[i].text.IndexOf("\r\n");
                if (enter >= 0) {
                    string before = parts_[i].text.Substring(0, enter);
                    string after = parts_[i].text.Substring(enter + 2);
                    parts_[i].text = before;
                    parts_[i].ends_line = true;
                    var new_ = new part() {text = after, link = parts_[i].link};
                    new_.print.copy_from(parts_[i].print);
                    parts_.Insert(i + 1, new_);
                }
            }

            cur_line_idx_ = 0;
            update_cur_line_text();
        }

        private void parse_format(string format, ref print_info print, ref string link) {
            switch (format) {
            case "b":
                print.bold = true;
                return;
            case "/b":
                print.bold = false;
                return;
            case "i":
                print.italic = true;
                return;
            case "/i":
                print.italic = false;
                return;

            case "/a":
                link = "";
                print.underline = false;
                return;

            case "/fg":
                print.fg = util.transparent;
                return;
            case "/bg":
                print.bg = util.transparent;
                return;

            case "/font":
                print.font_name = "";
                break;
            }

            if (format.StartsWith("a ")) {
                format = format.Substring(2).Trim();
                print.underline = true;
                link = format;
            }
            else if (format.StartsWith("fg ")) {
                format = format.Substring(3).Trim();
                print.fg = util.str_to_color(format);
            }
            else if (format.StartsWith("bg ")) {
                format = format.Substring(3).Trim();
                print.bg = util.str_to_color(format);
            }
            else if (format.StartsWith("font ")) {
                format = format.Substring(5).Trim();
                print.font_name = format;
            }
            else 
                Debug.Assert(false);
        }

        private void goToNextLine_Tick(object sender, EventArgs e) {
            int line_count = parts_.Count(p => p.ends_line) + 1;
            int old_idx = cur_line_idx_;
            cur_line_idx_ = (cur_line_idx_ + 1) % line_count;
            if ( cur_line_idx_ != old_idx)
                update_cur_line_text();
        }

        private void status_MouseMove(object sender, MouseEventArgs e) {
            // of course it doesn't work, it would have been too simple
            status.Cursor = Cursors.Arrow;
        }

    }

}
