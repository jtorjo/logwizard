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
        }

        public test_syntax_form(string initial_lines) {
            InitializeComponent();
            test.Left = -100;
            cancel.Left = -100;
            use_lines(initial_lines);
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
            if (text.Split('\r').Length < LEAST_LINES)
                return;

            if (text == lines.Text)
                return;

            use_lines(text);
        }

        private void help_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/jtorjo/logwizard/wiki/Syntax");
        }

        private void use_lines(string lines_str) {
            lines.Text = lines_str;
            syntax.Text = new find_log_syntax().try_find_log_syntax(lines_str.Split( new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries) );
            syntax.SelectionStart = syntax.TextLength;

            test_Click(null,null);
            util.postpone(() => syntax.Focus(),1);
        }

        private string column_name(int idx) {
            info_type i = (info_type) idx;
            switch (i) {
            case info_type.time:
                return "Time";
            case info_type.date:
                return "Date";
            case info_type.level:
                return "Level";
            case info_type.thread:
                return "Thread";
            case info_type.class_:
                return "Class";
            case info_type.file:
                return "File";
            case info_type.func:
                return "Func";
            case info_type.ctx1:
                return "Ctx1";
            case info_type.ctx2:
                return "Ctx2";
            case info_type.ctx3:
                return "Ctx3";
            case info_type.msg:
                return "Message";
            case info_type.max: 
                break;
            default:
                Debug.Assert(false);
                break;
            }

            return "";
        }

        private void test_Click(object sender, EventArgs e) {
            result.ClearObjects();

            // parse each line
            log_line_parser parse = new log_line_parser(new inmem_text_reader(lines.Text), syntax.Text);
            while (!parse.up_to_date)
                Thread.Sleep(10);

            foreach (var col in result.Columns) {
                var header = col as OLVColumn;
                header.Width = header.Index == (int) info_type.msg ? 90 : 0;
                header.FillsFreeSpace = header.Index == (int) info_type.msg;
            }

            for (int idx = 0; idx < parse.line_count; ++idx) {
                var l = parse.line_at(idx);
                List<string> cols = new List<string>();
                for (int col_idx = 0; col_idx < (int) info_type.max; ++col_idx)
                    cols.Add(l.part((info_type) col_idx));

                result.AddObject(new item(cols));
            }

            foreach (var col in result.Columns) {
                var header = col as OLVColumn;
                bool column_visible = false;
                for (int idx = 0; idx < result.GetItemCount() && !column_visible; ++idx)
                    if ((result.GetItem(idx).RowObject as item).part(header.Index) != "")
                        column_visible = true;

                if (column_visible) {
                    if ( header.Width < 1)
                        header.Width = 90;
                    header.Text = column_name(header.Index);
                }
            }
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
