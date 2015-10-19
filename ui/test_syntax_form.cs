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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using lw_common;
using lw_common.parse;

namespace LogWizard.ui {
    public partial class test_syntax_form : Form {
        private const int LEAST_LINES = 10;

        private bool ignore_ = true;

        private string found_syntax_ = "";

        private class item {
            public List<string> parts = new List<string>();

            public item(List<string> parts) {
                this.parts = parts;
            }

            public string part(int i) {
                return parts.Count > i ? parts[i] : "";
            }

            public string t0 { get { return part(0); }}
            public string t1 { get { return part(1); }}
            public string t2 { get { return part(2); }}
            public string t3 { get { return part(3); }}
            public string t4 { get { return part(4); }}
            public string t5 { get { return part(5); }}
            public string t6 { get { return part(6); }}
            public string t7 { get { return part(7); }}
            public string t8 { get { return part(8); }}
            public string t9 { get { return part(9); }}

            public string t10 { get { return part(10); }}
            public string t11 { get { return part(11); }}
            public string t12 { get { return part(12); }}
            public string t13 { get { return part(13); }}
            public string t14 { get { return part(14); }}
            public string t15 { get { return part(15); }}
            public string t16 { get { return part(16); }}
            public string t17 { get { return part(17); }}
            public string t18 { get { return part(18); }}
            public string t19 { get { return part(19); }}

            public string t20 { get { return part(20); }}
            public string t21 { get { return part(21); }}
            public string t22 { get { return part(22); }}
            public string t23 { get { return part(23); }}
            public string t24 { get { return part(24); }}
            public string t25 { get { return part(25); }}
            public string t26 { get { return part(26); }}
            public string t27 { get { return part(27); }}
            public string t28 { get { return part(28); }}
            public string t29 { get { return part(29); }}

            public string t30 { get { return part(30); }}
            public string t31 { get { return part(31); }}
            public string t32 { get { return part(32); }}
            public string t33 { get { return part(33); }}
            public string t34 { get { return part(34); }}
            public string t35 { get { return part(35); }}
            public string t36 { get { return part(36); }}
            public string t37 { get { return part(37); }}
            public string t38 { get { return part(38); }}
            public string t39 { get { return part(39); }}

            public string t40 { get { return part(40); }}
            public string t41 { get { return part(41); }}
            public string t42 { get { return part(42); }}
            public string t43 { get { return part(43); }}
            public string t44 { get { return part(44); }}
            public string t45 { get { return part(45); }}
            public string t46 { get { return part(46); }}
            public string t47 { get { return part(47); }}
            public string t48 { get { return part(48); }}
            public string t49 { get { return part(49); }}

            public string t50 { get { return part(50); }}
        }

        public test_syntax_form(string initial_lines, string guessed_syntax) {
            InitializeComponent();
            test.Left = -100;
            cancel.Left = -100;
            initial_lines = util.normalize_enters(initial_lines);

            if (guessed_syntax == find_log_syntax.UNKNOWN_SYNTAX)
                guessed_syntax = "";

            use_lines(initial_lines, guessed_syntax);
        }

        public string found_syntax {
            get { return found_syntax_; }
        }

        private void test_syntax_form_Activated(object sender, EventArgs e) {
            if (ignore_) {
                // ignore first time - we're using the log lines from the existing log
                ignore_ = false;
                return;
            }

            // check clipboard - if non-empty, and more than 10 lines, use it
            string text = "";
            try {
                text = Clipboard.GetText();
            } catch {
            }
            text = util.normalize_enters(text);
            if (text.Split('\r').Length < LEAST_LINES)
                return;

            if (text == lines.Text)
                return;

            use_lines(text, syntax.Text);
        }

        private void help_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/jtorjo/logwizard/wiki/Syntax");
        }

        private void use_lines(string lines_str, string guessed_syntax) {

            lines.Text = lines_str;
            syntax.Text = guessed_syntax != "" ? guessed_syntax : new find_log_syntax().try_find_log_syntax(lines_str.Split( new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries) );
            syntax.SelectionStart = syntax.TextLength;

            test_Click(null,null);
            util.postpone(() => syntax.Focus(),1);
        }

        private string column_name(int idx) {
            info_type i = (info_type) idx;
            return info_type_io.to_friendly_str(i);
        }

        private void test_Click(object sender, EventArgs e) {
            result.Enabled = false;
            result.ClearObjects();

            // parse each line
            log_parser parse = new log_parser(new inmem_text_reader(lines.Text), new line_by_line_syntax { line_syntax = syntax.Text } );
            while (!parse.up_to_date)
                Thread.Sleep(10);

            for (int col_idx = 0; col_idx < result.AllColumns.Count; col_idx++) {
                var header = result.AllColumns[col_idx];

                header.Width = col_idx > 0 ? 100 : 0;
                header.FillsFreeSpace = col_idx == (int) info_type.msg;
                header.IsVisible = col_idx == (int) info_type.msg;
            }
            result.RebuildColumns();

            for (int idx = 0; idx < parse.line_count; ++idx) {
                var l = parse.line_at(idx);
                List<string> cols = new List<string>();
                for (int col_idx = 0; col_idx < (int) info_type.max; ++col_idx)
                    cols.Add(l.part((info_type) col_idx));

                result.AddObject(new item(cols));
            }

            for (int col_idx = 0; col_idx < result.AllColumns.Count; col_idx++) {
                var header = result.AllColumns[col_idx];

                bool column_visible = false;
                for (int idx = 0; idx < result.GetItemCount() && !column_visible; ++idx)
                    if ((result.GetItem(idx).RowObject as item).part(col_idx) != "")
                        column_visible = true;

                if (column_visible) {
                    header.Width = 100;
                    header.IsVisible = true;
                    header.Text = column_name(col_idx);
                }
            }
            result.RebuildColumns();
            result.Enabled = true;
        }

        private void result_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void cancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }

        private void use_Click(object sender, EventArgs e) {
            found_syntax_ = syntax.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
