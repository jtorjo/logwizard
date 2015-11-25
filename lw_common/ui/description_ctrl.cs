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

            public bool multi_line = false;
            // if multi line, do I auto resize this as the control resizes itself?
            // on a column, you can have at most one part that auto-resizes
            public bool auto_resize = true;
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
            public string name_ = "Layout";
        }

        private const int MAX_LABEL_WIDTH = 100;

        private static List<parts_layout_template> layouts_ = new List<parts_layout_template>();
        private int cur_layout_idx_ = 0;

        private List<info_type> visible_columns_ = new List<info_type>(); 
        private Dictionary<info_type, string> names_ = new Dictionary<info_type, string>(); 

        private bool is_editing_ = false;

        private int ignore_change_ = 0;

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

        private static string to_string(parts_layout_template layout) {
            string rows = util.concatenate( layout.rows_.Select(x => "" + x.label_width_ + "," + x.row_width_ + "," + x.parts_.Count), ";");
            string parts = "";
            for (int i = 0; i < layout.rows_.Count; ++i) {
                var r = layout.rows_[i];
                if (parts != "")
                    parts += ";";
                parts += util.concatenate(r.parts_.Select(x => "" + i + "," + (int)x.type + "," + (x.multi_line ? "1" : "0") + "," + (x.auto_resize ? "1" : "0") + "," + x.line_count), ";");
            }
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
                var cur_part = part_data.Split(',');
                Debug.Assert(cur_part.Length == 5);
                row r = layout.rows_[int.Parse(cur_part[0]) ];
                part p = new part();
                p.type = (info_type) int.Parse(cur_part[1]);
                p.multi_line = cur_part[2] == "1";
                p.auto_resize = cur_part[3] == "1";
                p.line_count = int.Parse(cur_part[4]);
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
            // - on first row, I have everything but level, time, date, line, msg
            // - on second row I have msg, multi-line, auto-resize
            parts_layout_template def_ = new parts_layout_template();
            def_.rows_.Add( new row { row_width_ = 250});
            def_.rows_.Add( new row());
            foreach ( info_type type in Enum.GetValues(typeof(info_type)))
                switch (type) {
                case info_type.max:
                case info_type.level:
                case info_type.date:
                case info_type.time:
                case info_type.view:
                case info_type.line:
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
            int non_fixed_count = layout.rows_.Count(x => x.row_width_ >= 0);
            int width_per_row = non_fixed_count > 0 ? remaining_width / non_fixed_count : 0;

            int split_idx = 0;
            while (splits_[split_idx].Panel1Collapsed && split_idx < splits_.Count)
                ++split_idx;

            ++ignore_change_;
            for (int row_idx = 0; row_idx < panels_.Count; ++row_idx) {
                int width = layout.rows_[row_idx].row_width_ >= 0 ? layout.rows_[row_idx].row_width_ : width_per_row;;
                if (split_idx < splits_.Count) {
                    splits_[split_idx].SplitterDistance = width;
                    ++split_idx;
                }
            }
            --ignore_change_;
        }

        private void splitter_widths_to_row_widths() {
            if (layouts_.Count < 1 || cur_layout_idx_ < 0)
                return;
            
            int split_idx = 0;
            while (splits_[split_idx].Panel1Collapsed && split_idx < splits_.Count)
                ++split_idx;

            var layout = layouts_[cur_layout_idx_];
            for (int row_idx = 0; row_idx < panels_.Count; ++row_idx) {
                if (split_idx >= splits_.Count) 
                    // last one - doesn't count
                    break;
                int width = splits_[split_idx].SplitterDistance;
                ++split_idx;

                if (layout.rows_[row_idx].row_width_ >= 0)
                    layout.rows_[row_idx].row_width_ = width;
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
            update_label_widths();
            update_ui();
        }

        private void update_label_widths() {
            // we should have loaded layouts by now
            Debug.Assert(layouts_.Count >= 1);
            if (layouts_.Count < 1 || cur_layout_idx_ < 0)
                return;

            var layout = layouts_[cur_layout_idx_];
            for (int row_idx = 0; row_idx < layout.rows_.Count; ++row_idx) {
                var visible_parts = layout.rows_[row_idx].parts_.Where(x => visible_columns_.Contains(x.type)).ToList();
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

            remove_all_controls();
            var layout = layouts_[cur_layout_idx_];
            set_rows( layout.rows_.Count );
            row_widths_to_splitter_widths();

            const int COLUMN_HEIGHT = 19, COLUMN_PAD = 4;

            for (int row_idx = 0; row_idx < layout.rows_.Count; ++row_idx) {
                int top = COLUMN_PAD;
                var visible_parts = layout.rows_[row_idx].parts_.Where(x => visible_columns_.Contains(x.type) );
                foreach (var part in visible_parts) {
                    var label = new Label();
                    label.Text = names_[part.type];
                    label.AutoSize = false;
                    label.Left = COLUMN_PAD;
                    label.Top = top;
                    label.Height = COLUMN_HEIGHT;
                    label.Width = layout.rows_[row_idx].label_width_;

                    panels_[row_idx].Controls.Add(label);

                    var text = new RichTextBox();
                    text.Left = layout.rows_[row_idx].label_width_ + COLUMN_PAD;
                    text.Top = top;
                    text.Multiline = part.multi_line;
                    // note: rich text treats fixedsingle as 3d
                    text.BorderStyle = is_editing_ ? BorderStyle.FixedSingle : BorderStyle.None;
                    text.ReadOnly = true;
                    text.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right ;
                    text.Width = panels_[row_idx].Width - layout.rows_[row_idx].label_width_ - COLUMN_PAD * 2;
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
                }
            }
            
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
            if (!is_editing_) {
                // save - after user edit
                save();
                app.inst.save();
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
            splitter_widths_to_row_widths();
        }

        private void split5_SplitterMoved(object sender, SplitterEventArgs e) {
            if (ignore_change_ > 0)
                return;
            splitter_widths_to_row_widths();
        }

        private void split4_SplitterMoved(object sender, SplitterEventArgs e) {
            if (ignore_change_ > 0)
                return;
            splitter_widths_to_row_widths();
        }

        private void split3_SplitterMoved(object sender, SplitterEventArgs e) {
            if (ignore_change_ > 0)
                return;
            splitter_widths_to_row_widths();
        }

        private void split2_SplitterMoved(object sender, SplitterEventArgs e) {
            if (ignore_change_ > 0)
                return;
            splitter_widths_to_row_widths();
        }

        private void split1_SplitterMoved(object sender, SplitterEventArgs e) {
            if (ignore_change_ > 0)
                return;
            splitter_widths_to_row_widths();
        }

        private void visibleColumns_Click(object sender, EventArgs e) {

        }

        private void editingColumn_Click(object sender, EventArgs e) {

        }

        private void moveLeft_Click(object sender, EventArgs e) {

        }

        private void moveRight_Click(object sender, EventArgs e) {

        }

        private void moveUp_Click(object sender, EventArgs e) {

        }

        private void moveDown_Click(object sender, EventArgs e) {

        }

        private void saveLayout_Click(object sender, EventArgs e) {

        }

        private void loadLayout_Click(object sender, EventArgs e) {

        }

    }
}
