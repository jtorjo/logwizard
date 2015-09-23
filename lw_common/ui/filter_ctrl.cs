using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace lw_common.ui {
    public partial class filter_ctrl : UserControl {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ui_view view_;
        private int ignore_change_ = 0;

        public delegate void void_func();
        public void_func do_save;
        public void_func ui_to_view;
        // ... this means the view has changed drastically, and filters need to be re-run
        public void_func rerun_view;
        // ... this means the UI of the view needs refresh
        public void_func refresh_view;

        public filter_ctrl() {
            InitializeComponent();
        }

        class filter_item {
            public bool enabled = true;
            public bool dimmed = false;
            public string text = "";

            public bool apply_to_existing_lines = false;

            public string found_count = "";

            // "name" is just a friendly name for the text
            public string name {
                get {
                    string[] lines = text.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                    var filter_name = lines.FirstOrDefault(l => l.Trim().StartsWith("## "));
                    if (filter_name != null)
                        return filter_name.Trim().Substring(3).Trim();

                    return lines.Aggregate("", (current, l) => current + (l + " | "));
                }
            } 
        }

        private void filterCtrl_SelectedIndexChanged(object sender, EventArgs e) {
            int sel_filter = filterCtrl.SelectedIndex;
            bool has_sel = sel_filter >= 0;
            if (!has_sel) {
                ++ignore_change_;
                curFilterCtrl.Text = "";
                applyToExistingLines.Checked = false;
                --ignore_change_;
                return;
            }

            filter_item i = filterCtrl.GetItem(sel_filter).RowObject as filter_item;
            /*
            raw_filter_row filt = new raw_filter_row(i.text, i.apply_to_existing_lines);
            if (filt.is_valid) {
                var lv = ensure_we_have_log_view_for_tab(sel_view);
                Color fg = util.str_to_color(sett.get("filter_fg", "transparent"));
                Color bg = util.str_to_color(sett.get("filter_bg", "#faebd7"));
                lv.mark_match(sel_filter, fg, bg);
            }*/

            ++ignore_change_;
            applyToExistingLines.Checked = i.apply_to_existing_lines;
            --ignore_change_;

        }

        private void filterCtrl_KeyPress(object sender, KeyPressEventArgs e) {
            e.Handled = true;
        }

        private void filterCtrl_MouseDown(object sender, MouseEventArgs e) {
            // for some very fucked up strange reason, if FullRowSelect is on, "on mouse up" doesn't get called - simulating a mouse move will trigger it
            // ... note: there's a bug when clicking on a combo or on a checkbox, and then clicking on the same type of control on another row
            var mouse = win32.GetMousePos();
            win32.SetMousePos( mouse.x+1, mouse.y);
        }

        private void filterCtrl_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e) {
            if (e.SubItemIndex == filterCol.Index) {
                e.Cancel = true;

                var sel = filterCtrl.SelectedObject as filter_item;
                // we must be editing a filter row!
                Debug.Assert(sel != null);

                util.postpone(() => {
                    curFilterCtrl.Focus();
                    curFilterCtrl.SelectionStart = curFilterCtrl.TextLength;                    
                }, 10);
            }
        }

        private void filterCtrl_ItemsChanged(object sender, BrightIdeasSoftware.ItemsChangedEventArgs e) {
            do_save();
        }

        private void filterCtrl_SelectionChanged(object sender, EventArgs e) {
            ++ignore_change_;
            var sel = filterCtrl.SelectedObject as filter_item;
            curFilterCtrl.Text = sel != null ? sel.text : "";
            curFilterCtrl.Enabled = sel != null;
            --ignore_change_;
        }

        private void curFilterCtrl_TextChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;

            if ( filterCtrl.GetItemCount() == 0) {
                // this will in turn call us again
                addFilter_Click(null,null);
                return;
            }
            var sel = filterCtrl.SelectedObject as filter_item;
            // we must be editing a filter row!
            Debug.Assert(sel != null);
            if (sel == null) 
                return;

            if (sel.text != curFilterCtrl.Text) {
                sel.text = curFilterCtrl.Text;
                filterCtrl.RefreshObject(sel);
            }

            var row = new raw_filter_row(curFilterCtrl.Text, applyToExistingLines.Checked);
            bool is_valid = row.is_valid;
            Color bg = is_valid ? Color.White : Color.LightPink;
            if (curFilterCtrl.BackColor.ToArgb() != bg.ToArgb())
                curFilterCtrl.BackColor = bg;

            if (row.is_valid) {
                if (filterLabel.BackColor.ToArgb() != row.bg.ToArgb())
                    filterLabel.BackColor = row.bg;
                if (filterLabel.ForeColor.ToArgb() != row.fg.ToArgb())
                    filterLabel.ForeColor = row.fg;
            }
        }

        private void applyToExistingLines_CheckedChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;
            if (filterCtrl.GetItemCount() == 0)
                return;

            var sel = filterCtrl.SelectedObject as filter_item;
            // we must be editing a filter row!
            Debug.Assert(sel != null);
            if (sel == null) 
                return;

            if (sel.apply_to_existing_lines != applyToExistingLines.Checked) {
                sel.apply_to_existing_lines = applyToExistingLines.Checked;
                do_save();
            }
        }

        private void addFilter_Click(object sender, EventArgs e) {
            if (view_ == null) {
                Debug.Assert(false);
                return;
            }

            filter_item new_ = new filter_item { enabled = true, dimmed = false, text = "", apply_to_existing_lines = false};

            view_.filters.Add( new ui_filter() );

            filterCtrl.AddObject(new_);
            filterCtrl.SelectObject(new_);
            curFilterCtrl.Text = "";

            do_save();

            util.postpone(() => {
                curFilterCtrl.Focus();
                curFilterCtrl.SelectionStart = curFilterCtrl.TextLength;                    
            }, 10);
        }

        private void delFilter_Click(object sender, EventArgs e) {
            if (view_ == null) {
                Debug.Assert(false);
                return;
            }

            var sel = filterCtrl.SelectedObject as filter_item;
            if ( sel != null) {
                int idx = filterCtrl.SelectedIndex;
                view_.filters.RemoveAt(idx);
                filterCtrl.RemoveObject(sel);

                int new_sel = view_.filters.Count > idx ? idx : view_.filters.Count > 0 ? view_.filters.Count - 1 : -1;
                if (new_sel >= 0)
                    filterCtrl.SelectedIndex = new_sel;
            }

            do_save();
        }

        private void tipsHotkeys_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/jtorjo/logwizard/wiki/Filters");
        }

        private void viewToClipboard_Click(object sender, EventArgs e) {
            if (view_ == null)
                return;

            if (view_.filters.Count < 1)
                return; // nothing to copy
            var formatter = new XmlSerializer( typeof(ui_view));
            string to_copy = "";
            using (var stream = new MemoryStream()) {
                formatter.Serialize(stream, view_);
                stream.Flush();
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                    to_copy = reader.ReadToEnd();
            }
            Clipboard.SetText(to_copy);                
        }

        private void viewFromClipboard_Click(object sender, EventArgs ea) {
            if (view_ == null)
                return;

            try {
                string txt = Clipboard.GetText();
                var formatter = new XmlSerializer( typeof(ui_view));
                using (var stream = new MemoryStream()) {
                    using (var writer = new StreamWriter(stream)) {
                        writer.Write(txt);
                        writer.Flush();
                        stream.Position = 0;
                        using (var reader = new StreamReader(stream)) {
                            var new_view = (ui_view) formatter.Deserialize(reader);
                            // we don't care about the name, just the filters
                            new_view.filters.ForEach(f => f.text = util.normalize_enters(f.text));
                            view_.filters = new_view.filters;
                        }
                    }
                }
                /*
                load_filters();
                refreshFilter_Click(null, null);
                */
                ui_to_view();
                do_save();
                rerun_view();
            } catch(Exception e) {
                logger.Error("can't copy from clipboard: " + e.Message);
                util.beep(util.beep_type.err);
            }
        }

        private void selectColor_Click(object sender, EventArgs e) {
            if (filterCtrl.SelectedIndex < 0)
                return; // there's nothing selected

            var sel = new select_color_form();
            if (sel.ShowDialog() == DialogResult.OK) {
                string sel_color = util.color_to_str( sel.SelectedColor);

                var lines = curFilterCtrl.Text.Split( new string[] { "\r\n" }, StringSplitOptions.None ).ToList();
                int sel_start = curFilterCtrl.SelectionStart;
                int edited_line = util.index_to_line(curFilterCtrl.Text, sel_start);
                if (edited_line == -1) {
                    // it's not with the cursor on a line - find the first line that would actually be a color
                    for ( int i = 0; i < lines.Count && edited_line == -1; ++i)
                        if (lines[i].Trim().StartsWith("color") || lines[i].Trim().StartsWith("match_color"))
                            edited_line = i;
                }
                if (edited_line != -1) {
                    // in this case, he's editing the color from a given line
                    bool is_color_line = lines[edited_line].Trim().StartsWith("color ") || lines[edited_line].Trim().StartsWith("match_color ");
                    bool is_replacing = lines[edited_line].Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries).Length == 2 &&
                                        lines[edited_line].TrimEnd() == lines[edited_line];
                    if (is_color_line) {
                        if (is_replacing)
                            lines[edited_line] = (lines[edited_line].Trim().StartsWith("color ") ? "color" : "match_color") + " " + sel_color;
                        else
                            lines[edited_line] += " " + sel_color;
                    } else
                        // the edited line does not contain any color, thus, we append the color line
                        edited_line = -1;
                }


                if (edited_line == -1) {
                    lines.Add("color " + sel_color);
                    sel_start = -1;
                }

                curFilterCtrl.Text = util.concatenate(lines, "\r\n");
                if ( sel_start >= 0 && sel_start < curFilterCtrl.TextLength)
                    curFilterCtrl.SelectionStart = sel_start;


                ui_to_view();
                do_save();
                refresh_view();
                /*
                selected_view().Refresh();
                log_view_for_tab(viewsTab.SelectedIndex).Refresh();
                */
            }

        }

        private void filter_ctrl_Load(object sender, EventArgs e) {
            Debug.Assert(do_save != null && rerun_view != null && refresh_view != null && ui_to_view != null);
        }
    }
}
