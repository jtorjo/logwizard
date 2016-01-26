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
using lw_common.parse;

namespace lw_common.ui {
    public partial class description_ctrl : UserControl {
        private List<SplitContainer> splits_ = new List<SplitContainer>(); 

        private List<Panel> panels_ = new List<Panel>();

        private class part {

            public info_type type = info_type.max;

            public bool visible = true;

            public bool multi_line = false;
            // if multi line, do I auto resize this as the control resizes itself?
            // on a column, you can have at most one part that auto-resizes
            public bool auto_resize = false;
            // if it's multi-line, and we don't auto resize, how many lines should we have?
            public int line_count = 3;
        }

        private class row {
            public List<part> parts_ = new List<part>();
            public int label_width_ = 60;
            // if < 0, we auto-size it. if > 0, it's fixed
            public int row_width_ = -1;
        }

        // this contains everything needed to show all layout
        private class parts_layout_template {
            public List<row> rows_ = new List<row>();
            // this needs to be unique - we can bind a specific layout template to a log and/or context
            public string name_ = "Layout";
        }

        private const int MAX_LABEL_WIDTH = 100;

        private static List<parts_layout_template> layouts_ = new List<parts_layout_template>();
        private int cur_layout_idx_ = 0;

        private List<info_type> visible_columns_ = new List<info_type>(); 
        private Dictionary<info_type, string> names_ = new Dictionary<info_type, string>(); 

        private bool is_editing_ = false;

        private int ignore_change_ = 0;

        private info_type edit_column_ = info_type.max;

        private Dictionary<info_type, Tuple<Label,RichTextBox> > column_to_controls_ = new Dictionary<info_type, Tuple<Label, RichTextBox>>();

        private font_list fonts_ = new font_list();

        private log_view_item_draw_ui drawer_ = null;
        text_part default_print_ = new text_part(0, 0);

        public delegate void on_description_template_changed_func(string description);
        public on_description_template_changed_func on_description_changed;

        public description_ctrl() {
            InitializeComponent();
            splits_.Add(split1);
            splits_.Add(split2);
            splits_.Add(split3);
            splits_.Add(split4);
            splits_.Add(split5);
            splits_.Add(split6);

            update_is_editing_ui();
            load();
            update_ui();
        }

        const int WS_EX_NOACTIVATE = 0x08000000;
        const int WS_EX_TOOLWINDOW = 0x80;
        protected override CreateParams CreateParams { get {
            var Params = base.CreateParams;
            Params.ExStyle |= WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
            return Params;
        }}

        public List<info_type> shown_columns {
            get { return column_to_controls_.Select(x => x.Key).ToList(); }
        } 

        private static string to_string(parts_layout_template layout) {
            string rows = util.concatenate( layout.rows_.Select(x => "" + x.label_width_ + "," + x.row_width_ + "," + x.parts_.Count), ";");
            string parts = "";
            for (int i = 0; i < layout.rows_.Count; ++i) {
                var r = layout.rows_[i];
                if (parts != "")
                    parts += ";";
                parts += util.concatenate(r.parts_.Select(x => "" + i + "," + (int)x.type 
                    + "," + (x.visible ? "1" : "0") + "," + (x.multi_line ? "1" : "0") + "," + (x.auto_resize ? "1" : "0") + "," + x.line_count), ";");
            }
            // note: when too many settings here, I should use a log_settings_string-like or something - at this point, it's manageable, but perhaps later
            settings_as_string sett = new settings_as_string("");
            sett.set("rows", rows);
            sett.set("parts", parts);
            sett.set("name", layout.name_);
            return sett.ToString();
        }

        private static parts_layout_template from_string(string s) {
            settings_as_string sett = new settings_as_string(s);
            string rows = sett.get("rows"), parts = sett.get("parts");
            parts_layout_template layout = new parts_layout_template();
            foreach (var row_data in rows.Split(';')) {
                var cur_row = row_data.Split(',');
                Debug.Assert(cur_row.Length == 3);
                row r = new row();
                r.label_width_ = int.Parse(cur_row[0]);
                r.row_width_ = int.Parse(cur_row[1]);
                int count = int.Parse(cur_row[2]);
                layout.rows_.Add(r);
            }

            foreach (var part_data in parts.Split(';')) {
                if (part_data == "")
                    continue;
                var cur_part = part_data.Split(',');
                Debug.Assert(cur_part.Length == 6);
                row r = layout.rows_[int.Parse(cur_part[0]) ];
                part p = new part();
                p.type = (info_type) int.Parse(cur_part[1]);
                p.visible = cur_part[2] == "1";
                p.multi_line = cur_part[3] == "1";
                p.auto_resize = cur_part[4] == "1";
                p.line_count = int.Parse(cur_part[5]);
                r.parts_.Add(p);
            }
            layout.name_ = sett.get("name");
            return layout;
        }


        private void load() {
            if ( layouts_.Count < 1)
                foreach ( var str in app.inst.description_layouts_)
                    layouts_.Add( from_string(str) );
            cur_layout_idx_ = app.inst.description_layout_idx_;

            if ( layouts_.Count == 0)
                layouts_.Add( set_defaults());
        }

        private void save() {
            // set_aliases teh strings in app.
            app.inst.description_layout_idx_ = cur_layout_idx_;
            app.inst.description_layouts_ = layouts_.Select(to_string).ToList();
        }

        private parts_layout_template set_defaults() {
            // default: 2 rows 
            // - on first row, I have everything but level, time, date, line, thread, msg
            // - on second row I have msg, multi-line, auto-resize
            parts_layout_template def_ = new parts_layout_template();
            def_.rows_.Add( new row { row_width_ = 250});
            def_.rows_.Add( new row());
            foreach ( info_type type in Enum.GetValues(typeof(info_type)))
                switch (type) {
                case info_type.max:
                    break;

                    // 1.6.9+ by default, hide them - in other words, normally they are shown in the view. But the user can choose to show them here
                case info_type.level:
                case info_type.date:
                case info_type.time:
                case info_type.view:
                case info_type.line:
                case info_type.thread:
                    def_.rows_[0].parts_.Add( new part { type = type, visible = false } );
                    break;

                case info_type.msg:
                    def_.rows_[1].parts_.Add( new part { type = type, auto_resize = true, multi_line = true, line_count = 0 } );
                    break;
                default:
                    def_.rows_[0].parts_.Add( new part { type = type } );
                    break;
                }
            return def_;
        }

        private void set_rows(int cols) {
            if (cols < 1 || cols > splits_.Count) {
                Debug.Assert(false);
                return;
            }
            if (panels_.Count == cols)
                // already set
                return;

            for (int idx = 0; idx < splits_.Count; ++idx) {
                bool show_both = idx > splits_.Count - cols;
                show_both_splitters(splits_[idx], show_both);
            }

            panels_.Clear();
            for (int idx = 0; idx < splits_.Count; ++idx) 
                if ( !splits_[idx].Panel1Collapsed)
                    panels_.Add(splits_[idx].Panel1);
            panels_.Add(splits_.Last().Panel2);
            Debug.Assert(panels_.Count == cols);

            ++ignore_change_;
            rowCount.SelectedIndex = cols - 1;
            --ignore_change_;
        }

        private void remove_all_controls() {
            foreach (var split in splits_) {
                remove_all_controls(split.Panel1);
                remove_all_controls(split.Panel2);
            }
        }
        private void remove_all_controls(Control parent) {
            var to_remove = new List<Control>();
            foreach (Control c in parent.Controls)
                if ( c is Label || c is TextBoxBase)
                    to_remove.Add(c);
            to_remove.ForEach((c) => parent.Controls.Remove(c));
        }

        private void row_widths_to_splitter_widths() {
            if (layouts_.Count < 1 || cur_layout_idx_ < 0)
                return;

            var layout = layouts_[cur_layout_idx_];
            int fixed_width = layout.rows_.Sum(x => x.row_width_ >= 0 ? x.row_width_ : 0);
            int splitter_width = (panels_.Count - 1) * split1.SplitterWidth;
            int remaining_width = Width - fixed_width - splitter_width;
            int non_fixed_count = layout.rows_.Count(x => x.row_width_ < 0);
            int width_per_row = non_fixed_count > 0 ? remaining_width / non_fixed_count : 0;

            int split_idx = 0;
            while (split_idx < splits_.Count && splits_[split_idx].Panel1Collapsed)
                ++split_idx;

            ++ignore_change_;
            for (int row_idx = 0; row_idx < panels_.Count; ++row_idx) {
                int width = layout.rows_[row_idx].row_width_ >= 0 ? layout.rows_[row_idx].row_width_ : width_per_row;;
                if (split_idx < splits_.Count)
                    splits_[split_idx].SplitterDistance = width;
                ++split_idx;
            }
            --ignore_change_;
        }

        private void splitter_widths_to_row_widths(int moved_splitter_idx) {
            if (layouts_.Count < 1 || cur_layout_idx_ < 0)
                return;
            
            int split_idx = 0;
            while (split_idx < splits_.Count && splits_[split_idx].Panel1Collapsed )
                ++split_idx;

            var layout = layouts_[cur_layout_idx_];
            for (int row_idx = 0; row_idx < panels_.Count; ++row_idx) {
                if (split_idx >= splits_.Count) 
                    // last one - doesn't count
                    break;
                int width = splits_[split_idx].SplitterDistance;
                if (layout.rows_[row_idx].row_width_ >= 0 || split_idx == moved_splitter_idx)
                    layout.rows_[row_idx].row_width_ = width;

                ++split_idx;
            }
            save();
        }

        // the aliases tell us which columns are visible
        public void set_aliases(aliases aliases) {
            // set_aliases names as well
            visible_columns_.Clear();
            names_.Clear();
            foreach ( info_type type in Enum.GetValues(typeof(info_type)))
                if (aliases.has_column(type)) {
                    string name = aliases.friendly_name(type);
                    visible_columns_.Add(type);
                    names_.Add(type, name);
                }
            update_ui();
        }

        public void set_layout(string name) {
            load_layout_by_name(name);
        }

        private void update_label_widths() {
            // we should have loaded layouts by now
            Debug.Assert(layouts_.Count >= 1);
            if (layouts_.Count < 1 || cur_layout_idx_ < 0)
                return;

            var layout = layouts_[cur_layout_idx_];
            for (int row_idx = 0; row_idx < layout.rows_.Count; ++row_idx) {
                var visible_parts = layout.rows_[row_idx].parts_.Where(x => visible_columns_.Contains(x.type) && x.visible).ToList();
                if (visible_parts.Count < 1)
                    continue;
                List<int> widths = new List<int>();
                using (var g = CreateGraphics()) {
                    foreach (var part in visible_parts)
                        widths.Add((int) g.MeasureString(names_[part.type], Font).Width + 1);
                }
                // when showing the label, it does pad a few pixels to left/right
                const int LABEL_PAD = 7;
                int max_width = Math.Min( widths.Max() + LABEL_PAD, MAX_LABEL_WIDTH);
                layout.rows_[row_idx].label_width_ = max_width;
            }
        }

        private void update_ui() {
            if (layouts_.Count < 1 || cur_layout_idx_ < 0)
                return;

            SuspendLayout();

            ++ignore_change_;
            remove_all_controls();
            var layout = layouts_[cur_layout_idx_];
            set_rows( layout.rows_.Count );
            row_widths_to_splitter_widths();
            update_label_widths();
            column_to_controls_.Clear();

            const int COLUMN_HEIGHT = 19, COLUMN_PAD = 4;

            for (int row_idx = 0; row_idx < layout.rows_.Count; ++row_idx) {
                int top = COLUMN_PAD;
                var visible_parts = layout.rows_[row_idx].parts_.Where(x => visible_columns_.Contains(x.type) && x.visible );
                foreach (var part in visible_parts) {
                    var label = new Label();
                    label.Text = names_[part.type];
                    label.AutoSize = false;
                    label.Left = COLUMN_PAD;
                    label.Top = top;
                    label.Height = COLUMN_HEIGHT;
                    label.Width = layout.rows_[row_idx].label_width_;
                    label.Font = fonts_.get_font(Font.Name, (int)Font.Size - 1, true, false, false);
                    label.TextAlign = ContentAlignment.MiddleLeft;

                    panels_[row_idx].Controls.Add(label);

                    var text = new RichTextBox();
                    text.Left = layout.rows_[row_idx].label_width_ + COLUMN_PAD;
                    text.Top = top;
                    text.Multiline = part.multi_line;
                    // note: rich text treats fixedsingle as 3d
                    text.BorderStyle = is_editing_ ? BorderStyle.FixedSingle : BorderStyle.None;
                    text.ReadOnly = true;
                    text.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right ;
                    // ... have a min width as well, just in case the rows become too small
                    text.Width = Math.Max( panels_[row_idx].Width - layout.rows_[row_idx].label_width_ - COLUMN_PAD * 2, 50);
                    if (part.multi_line) {
                        text.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
                        if (part.auto_resize) {
                            text.Height = panels_[row_idx].Height - text.Top - COLUMN_PAD;
                            text.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                        }
                        else
                            text.Height = COLUMN_HEIGHT * part.line_count;
                    } else
                        text.Height = COLUMN_HEIGHT;
                    panels_[row_idx].Controls.Add(text);
                    top += text.Height + COLUMN_PAD;

                    column_to_controls_.Add( part.type, new Tuple<Label, RichTextBox>(label, text));
                }
            }

            foreach (var panel in panels_)
                panel.BackColor = Color.White;

            description.Text = layouts_[cur_layout_idx_].name_;
            update_edit_column();
            --ignore_change_;
            ResumeLayout(true);
        }

        private void show_both_splitters(SplitContainer split, bool show) {
            split.Panel1Collapsed = !show;
            if (show)
                split.Panel1.Show();
            else
                split.Panel1.Hide();
        }

        private void description_ctrl_SizeChanged(object sender, EventArgs e) {
            row_widths_to_splitter_widths();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e) {
            is_editing_ = !is_editing_;
            update_is_editing_ui();
            make_sure_description_is_unique();
            if (!is_editing_) {
                // save - after user edit
                save();
                app.inst.save();
            }
        }

        private void make_sure_description_is_unique() {
            Debug.Assert(layouts_.Count > 0);
            var layout = layouts_[cur_layout_idx_];
            string prefix = layout.name_;
            int suffix_idx = 0;
            string name = "";
            while (true) {
                name = prefix + (suffix_idx > 0 ? "" + (suffix_idx + 1) : "");
                layout.name_ = name;
                if (layouts_.Count(x => x.name_ == name) <= 1)
                    break;
                ++suffix_idx;
            }
        }

        private void update_is_editing_ui() {
            editPanel.Top = is_editing_ ? 0 : -editPanel.Height;
            split1.Top = is_editing_ ? editPanel.Height : 0;
            split1.Height += is_editing_ ? -editPanel.Height : editPanel.Height;
            editToolStripMenuItem.Text = is_editing_ ? "Finish Editing Description Layout" : "Edit Description Layout";
            update_ui();
        }

        private void split6_SplitterMoved(object sender, SplitterEventArgs e) {
            if (ignore_change_ > 0)
                return;
            splitter_widths_to_row_widths(5);
        }

        private void split5_SplitterMoved(object sender, SplitterEventArgs e) {
            if (ignore_change_ > 0)
                return;
            splitter_widths_to_row_widths(4);
        }

        private void split4_SplitterMoved(object sender, SplitterEventArgs e) {
            if (ignore_change_ > 0)
                return;
            splitter_widths_to_row_widths(3);
        }

        private void split3_SplitterMoved(object sender, SplitterEventArgs e) {
            if (ignore_change_ > 0)
                return;
            splitter_widths_to_row_widths(2);
        }

        private void split2_SplitterMoved(object sender, SplitterEventArgs e) {
            if (ignore_change_ > 0)
                return;
            splitter_widths_to_row_widths(1);
        }

        private void split1_SplitterMoved(object sender, SplitterEventArgs e) {
            if (ignore_change_ > 0)
                return;
            splitter_widths_to_row_widths(0);
        }

        private List<part> get_visible_parts() {
            Debug.Assert(layouts_.Count > 0);
            var layout = layouts_[cur_layout_idx_];

            List<part> visible = new List<part>();
            foreach (var row in layout.rows_) {
                var cur_visible = row.parts_.Where(x => visible_columns_.Contains(x.type) && x.visible);
                visible.AddRange(cur_visible);
            }
            // should always have at least something visible
            Debug.Assert(visible.Count >= 1);
            return visible;
        }

        private part column_name_to_part(string column_name) {
            Debug.Assert(layouts_.Count > 0);
            var layout = layouts_[cur_layout_idx_];
            foreach (var row in layout.rows_)
                foreach ( var part in row.parts_)
                    if (names_.ContainsKey(part.type) && names_[part.type] == column_name)
                        return part;
            Debug.Assert(false);
            return null;
        }

        private void update_edit_column() {
            if (visible_columns_.Count < 1)
                // did not set aliases yet
                return;

            var visible = get_visible_parts().Select(x => x.type).ToList();
            if (edit_column_ == info_type.max || !visible.Contains(edit_column_)) 
                edit_column_ = visible[0];

            if (!is_editing_)
                return;

            editColumnName.Text = names_[edit_column_];
            foreach (var column in column_to_controls_) {
                bool edited = column.Key == edit_column_;
                column.Value.Item1.Font = fonts_.get_font(Font, edited, false, false);
                column.Value.Item2.Font = fonts_.get_font(Font, edited, false, false);
            }
            ++ignore_change_;
            var cur = get_visible_parts().FirstOrDefault(x => x.type == edit_column_);
            isMultiline.Checked = cur.multi_line;
            lineCount.Visible = cur.multi_line;
            if (cur.multi_line) {
                if (cur.auto_resize)
                    lineCount.SelectedIndex = 0;
                else
                    lineCount.SelectedIndex = cur.line_count - 1;
            }
            --ignore_change_;
        }

        private void visibleColumns_Click(object sender, EventArgs e) {
            Debug.Assert(layouts_.Count > 0);
            var layout = layouts_[cur_layout_idx_];
            List<string> names = new List<string>();
            HashSet<string> visible = new HashSet<string>();
            foreach (var row in layout.rows_) {
                var cur_visible = row.parts_.Where(x => visible_columns_.Contains(x.type));
                names.AddRange(cur_visible.Select(x => names_[x.type]));
                foreach ( var part in cur_visible)
                    if (part.visible)
                        visible.Add(names_[part.type]);
            }
            ContextMenuStrip menu = new ContextMenuStrip();
            foreach (string name in names) {
                ToolStripMenuItem item = new ToolStripMenuItem(name);
                item.Checked = visible.Contains(name);
                item.Click += (o, ee) => toggle_visible_column(name);
                menu.Items.Add(item);
            }
            menu.Show(Cursor.Position, util.menu_direction(menu, Cursor.Position));
        }

        private void toggle_visible_column(string column_name) {
            var part = column_name_to_part(column_name);
            if (part.visible && get_visible_parts().Count == 1)
                // don't allow toggling off the last visible column
                return;

            part.visible = !part.visible;
            update_ui();            
        }

        private void editingColumn_Click(object sender, EventArgs e) {
            var visible = get_visible_parts();
            ContextMenuStrip menu = new ContextMenuStrip();
            foreach (part col in visible) {
                var part = col;
                ToolStripMenuItem item = new ToolStripMenuItem(names_[part.type]);
                item.Click += (o, ee) => edit_column(part.type);
                item.Checked = col.type == edit_column_;
                menu.Items.Add(item);
            }
            menu.Show(Cursor.Position, util.menu_direction(menu, Cursor.Position));        
        }

        private void edit_column(info_type column) {
            var visible = get_visible_parts();
            Debug.Assert(visible.Select(x => x.type).Contains(column));
            edit_column_ = column;
            update_edit_column();
        }


        private void moveLeft_Click(object sender, EventArgs e) {
            var layout = layouts_[cur_layout_idx_];
            var cur = get_visible_parts().FirstOrDefault(x => x.type == edit_column_);
            int idx = layout.rows_.FindIndex(x => x.parts_.Contains(cur));
            if (idx > 0) {
                layout.rows_[idx].parts_.Remove(cur);
                var parts = layout.rows_[idx - 1].parts_;
                // the auto resize column is always last
                if (parts.Count > 0 && parts.Last().auto_resize) {
                    parts.Insert(parts.Count - 1, cur);
                    cur.auto_resize = false;
                } else
                    parts.Add(cur);
                update_ui();
            }
        }

        private void moveRight_Click(object sender, EventArgs e) {
            var layout = layouts_[cur_layout_idx_];
            var cur = get_visible_parts().FirstOrDefault(x => x.type == edit_column_);
            int idx = layout.rows_.FindIndex(x => x.parts_.Contains(cur));
            if (idx < layout.rows_.Count - 1) {
                layout.rows_[idx].parts_.Remove(cur);
                var parts = layout.rows_[idx + 1].parts_;
                // the auto resize column is always last
                if (parts.Count > 0 && parts.Last().auto_resize) {
                    parts.Insert(parts.Count - 1, cur);
                    cur.auto_resize = false;
                } else
                    parts.Add(cur);
                update_ui();
            }
        }

        private void moveUp_Click(object sender, EventArgs e) {
            var layout = layouts_[cur_layout_idx_];
            var cur = get_visible_parts().FirstOrDefault(x => x.type == edit_column_);
            int row_idx = layout.rows_.FindIndex(x => x.parts_.Contains(cur));
            var parts = layout.rows_[row_idx].parts_;
            var visible_parts = parts.Where(x => visible_columns_.Contains(x.type) && x.visible).ToList();
            var idx = visible_parts.IndexOf(cur);
            if (idx > 0) {
                cur.auto_resize = false;
                var prev_visible_idx = parts.IndexOf( visible_parts[idx - 1]);
                parts.Remove(cur);
                parts.Insert(prev_visible_idx, cur);
                update_ui();
            }
        }

        private void moveDown_Click(object sender, EventArgs e) {
            var layout = layouts_[cur_layout_idx_];
            var cur = get_visible_parts().FirstOrDefault(x => x.type == edit_column_);
            int row_idx = layout.rows_.FindIndex(x => x.parts_.Contains(cur));
            var parts = layout.rows_[row_idx].parts_;
            var visible_parts = parts.Where(x => visible_columns_.Contains(x.type) && x.visible).ToList();
            var idx = visible_parts.IndexOf(cur);
            if (idx < visible_parts.Count- 1) {
                var next_visible_idx = parts.IndexOf( visible_parts[idx + 1]);
                parts.Remove(cur);
                parts.Insert(next_visible_idx, cur);
                update_ui();
            }
        }

        private void saveLayout_Click(object sender, EventArgs e) {
            Debug.Assert(is_editing_);
            editToolStripMenuItem_Click(null, null);
        }

        private void copy_Click(object sender, EventArgs e) {
            Debug.Assert(layouts_.Count > 0);
            var layout = layouts_[cur_layout_idx_];
            var copy = new parts_layout_template { name_ = layout.name_ + "_Copy" };
            foreach (var r in layout.rows_) {
                copy.rows_.Add(new row { label_width_ = r.label_width_, row_width_ = r.row_width_ });
                foreach ( var p in r.parts_)
                    copy.rows_.Last().parts_.Add( new part { auto_resize = p.auto_resize, type = p.type, visible = p.visible, multi_line = p.multi_line, line_count = p.line_count } );
            } 
            layouts_.Add(copy);
            cur_layout_idx_ = layouts_.Count - 1;
            update_ui();
        }

        private void loadLayout_Click(object sender, EventArgs e) {
            ContextMenuStrip menu = new ContextMenuStrip();
            for (int idx = 0; idx < layouts_.Count; idx++) {
                var layout = layouts_[idx];
                ToolStripMenuItem item = new ToolStripMenuItem(layout.name_);
                item.Checked = idx == cur_layout_idx_;
                item.Click += (a, ee) => load_layout_right_click(layout.name_);
                menu.Items.Add(item);
            }
            menu.Show(Cursor.Position, util.menu_direction(menu, Cursor.Position));
        }

        private void load_layout_right_click(string name) {
            load_layout_by_name(name);
            if ( on_description_changed != null)
                on_description_changed(name);
        }

        private void load_layout_by_name(string name) {
            var layout_idx = layouts_.FindIndex(x => x.name_ == name);
            if (layout_idx < 0)
                return; // not found

            cur_layout_idx_ = layout_idx;
            update_ui();
        }

        private void isMultiline_CheckedChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;
            var cur = get_visible_parts().FirstOrDefault(x => x.type == edit_column_);
            cur.multi_line = !cur.multi_line;
            update_ui();
        }

        private void lineCount_SelectedIndexChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;
            if (lineCount.SelectedIndex < 0)
                return;

            var layout = layouts_[cur_layout_idx_];
            var cur = get_visible_parts().FirstOrDefault(x => x.type == edit_column_);
            if (lineCount.SelectedIndex == 0) {
                // there can only be one auto-resize column - that is the last column
                cur.auto_resize = true;
                int idx = layout.rows_.FindIndex(x => x.parts_.Contains(cur));
                layout.rows_[idx].parts_.Remove(cur);
                foreach (var part in layout.rows_[idx].parts_)
                    cur.auto_resize = false;
                layout.rows_[idx].parts_.Add(cur);
            } else {
                cur.auto_resize = false;
                cur.line_count = lineCount.SelectedIndex + 1;
            }

            update_ui();
        }

        private void rowCount_SelectedIndexChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;

            Debug.Assert(layouts_.Count > 0);
            var layout = layouts_[cur_layout_idx_];

            int new_count = rowCount.SelectedIndex + 1;
            while (new_count < layout.rows_.Count) {
                // just move everything to first row
                var last = layout.rows_.Last();
                layout.rows_[0].parts_.AddRange(last.parts_);
                layout.rows_.RemoveAt( layout.rows_.Count - 1);
            }

            while (new_count > layout.rows_.Count) 
                layout.rows_.Add( new row());

            update_ui();
        }

        private void description_TextChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;

            Debug.Assert(layouts_.Count > 0);
            var layout = layouts_[cur_layout_idx_];
            layout.name_ = description.Text;
        }

        private void show_sub_item(log_view lv, info_type type, RichTextBox text_ctrl) {
            match_item item = lv.sel;
            int row = lv.sel_row_idx;

            string txt = (item as filter.match).line.part(type);
            int col = log_view_cell.info_type_to_cell_idx(type);
            var prints = lv.sel.override_print(lv, txt, type).to_single_enter_char();
            // ... text has changed
            txt = prints.text;

            text_ctrl.Clear();
            text_ctrl.AppendText(txt);
             
            var full_row = lv.list.GetItem(row);

            text_ctrl.BackColor = drawer_.bg_color(full_row, col);

            var parts = prints.parts(default_print_);
            foreach (var part in parts) {
                text_ctrl.Select(part.start, part.len);
                text_ctrl.SelectionColor = drawer_.print_fg_color(full_row, part);
                text_ctrl.SelectionBackColor = drawer_.print_bg_color(full_row, part);
            }
        }

        public void show_cur_item(log_view lv) {
            if ( drawer_ == null)
                drawer_ = new log_view_item_draw_ui(lv) { ignore_selection = true };
            drawer_.set_parent(lv);

            if (lv.sel != null) {
                foreach (var col in column_to_controls_) 
                    show_sub_item(lv, col.Key, col.Value.Item2);
            }
        }


    }
}
