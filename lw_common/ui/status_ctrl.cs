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

        // in case there's a link - at this time, we only handle at most one link per line
        private int link_start_ = 0, link_len_ = 0;

        private font_list fonts_ = new font_list();
        private solid_brush_list brushes_ = new solid_brush_list();

        public status_ctrl() {
            InitializeComponent();
        }

        public int time_per_line_ms {
            set { goToNextLine.Interval = value; }
        }

        private List<part> cur_line() {
            int line_idx = 0;
            List<part> cur_line = new List<part>();
            foreach (var p in parts_) {
                if (line_idx == cur_line_idx_) 
                    cur_line.Add(p);

                if (p.ends_line)
                    ++line_idx;
            }
            return cur_line;
        } 

        private void update_cur_line_text(Graphics g) {
            g.FillRectangle(brushes_.brush(BackColor), ClientRectangle );
            string all_chars = "qwertyuiop[]\';lkjhgfdsazxcvbnm,./QWERTYUIOPLKJHGFDSAZXCVBNM";
            var height = (int)(g.MeasureString(all_chars, Font).Height + .5);
            int height_offset = (Height - height) / 2;

            List<part> cur_line = this.cur_line();
            link_start_ = link_len_ = 0;
            int start = 0;
            var rect = ClientRectangle;
            foreach (var p in cur_line) {
                var fg = (p.print.fg != util.transparent) ? p.print.fg : ForeColor;
                var bg = (p.print.bg != util.transparent) ? p.print.bg : BackColor;
                string font_name = p.print.font_name != "" ? p.print.font_name : Font.Name;
                var font = fonts_.get_font(font_name, (int)Font.Size, p.print.bold, p.print.italic, p.print.underline ) ;

                var width = (int)g.MeasureString(p.text, font).Width + 1;
                g.FillRectangle(brushes_.brush(bg), start, rect.Top, width, rect.Height);
                g.DrawString(p.text, font, brushes_.brush(fg), start, height_offset);
                if (p.link != "") {
                    link_start_ = start;
                    link_len_ = width;
                }
                start += width;
            }
        }

        protected override void OnPrint(PaintEventArgs e) {
            var g = e.Graphics;
            update_cur_line_text(g);
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
            parts_.Clear();
            while (text != "") {
                int delimeter = text.IndexOf("<");
                if (delimeter > 0) {
                    var new_ = new part { text = text.Substring(0, delimeter), link = cur_link };
                    cur_link = "";
                    new_.print.copy_from(cur_print);
                    if ( new_.link != "")
                        new_.print.fg = Color.DodgerBlue;
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
                    if (after != "") {
                        var new_ = new part() {text = after, link = parts_[i].link};
                        new_.print.copy_from(parts_[i].print);
                        parts_.Insert(i + 1, new_);
                    }
                }
            }

            cur_line_idx_ = 0;
            Invalidate();
            Update();
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
            if (Cursor == Cursors.Hand) {
                if ( RectangleToScreen(ClientRectangle).Contains(Cursor.Position))
                    // user is over a link - don't move to next line at this time
                    return;
            }

            int line_count = parts_.Count(p => p.ends_line) + 1;
            int old_idx = cur_line_idx_;
            cur_line_idx_ = (cur_line_idx_ + 1) % line_count;
            if (cur_line_idx_ != old_idx) {
                Invalidate();
                Update();
            }
        }

        private void status_ctrl_Paint(object sender, PaintEventArgs e) {
            var g = e.Graphics;
            update_cur_line_text(g);
        }

        private void status_ctrl_MouseMove(object sender, MouseEventArgs e) {
            Cursor = e.X >= link_start_ && e.X < link_start_ + link_len_ ? Cursors.Hand : Cursors.Default;
        }

        private void status_ctrl_MouseUp(object sender, MouseEventArgs e) {
            if (Cursor == Cursors.Hand) {
                // user wants to go to the link
                List<part> cur_line = this.cur_line();
                var link_part = cur_line.FirstOrDefault(x => x.link != "");
                Debug.Assert(link_part != null);
                try {
                    Process.Start(link_part.link);
                } catch {}

            }
        }

    }

}
