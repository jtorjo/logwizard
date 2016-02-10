using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class categories_ctrl : UserControl {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string[] default_color_names_ = {
            "red", "blue", "green", "brown", "yellow", "cyan", "pink", "violet", "orange", "coral",
            "darkred", "darkblue", "darkgreen", "darkbrown", "darkyellow", "darkcyan", "darkpink", "darkviolet", "darkorange", "darkcoral",
            "lightred", "lightblue", "lightgreen", "lightbrown", "lightyellow", "lightcyan", "lightpink", "lightviolet", "lightorange", "lightcoral",
        };

        private class item {
            private readonly category_colors colors_;

            public item(category_colors colors) {
                colors_ = colors;
            }

            public string name {
                get { return colors_.name; }
            }

            public Color color_by_index(int idx) {
                switch (idx) {
                case 0:
                    return colors_.same_category_bg;
                case 1:
                    return colors_.bg_color;
                case 2:
                    return colors_.same_category_bg;
                case 3:
                    return colors_.this_category_bg;
                default:
                    Debug.Assert(false);
                    return util.transparent;
                }
            }
            public void color_by_index(int idx, Color c) {
                switch (idx) {
                case 0:
                    colors_.raw_same_category_bg = c;
                    break;
                case 1:
                    colors_.bg_color = c;
                    break;
                case 2:
                    colors_.raw_same_category_bg = c;
                    break;
                case 3:
                    colors_.raw_this_category_bg = c;
                    break;
                default:
                    Debug.Assert(false);
                    break;
                }
            }

            public string color {
                get { return util.color_to_str( colors_.bg_color); }
                set { colors_.bg_color = util.str_to_color( value); }
            }

            public string same_color {
                get {
                    return util.color_to_str(colors_.same_category_bg) ;
                }
                set {
                    colors_.raw_same_category_bg = util.str_to_color( value);
                }
            }

            public string this_color {
                get {
                    return util.color_to_str(colors_.this_category_bg) ;
                }
                set {
                    colors_.raw_this_category_bg = util.str_to_color( value);
                }
            }

            public category_colors colors {
                get { return colors_; }
            }
        }

        private class preview_item {
            private string date_;
            private string level_;
            private string thread_;
            private string message_;

            public preview_item(string date, string level, string thread, string message) {
                date_ = date;
                level_ = level;
                thread_ = thread;
                message_ = message;
            }

            public string date {
                get { return date_; }
            }

            public string level {
                get { return level_; }
            }

            public string thread {
                get { return thread_; }
                set { thread_ = value; }
            }

            public string message {
                get { return message_; }
            }
        }

        public delegate void on_change_category_type_func(string category_type);
        public on_change_category_type_func on_change_category_type;

        public delegate void on_category_colors_change_func(List<category_colors> colors);
        public on_category_colors_change_func on_category_colors_change;

        public delegate void on_running_changed_func(bool running);
        public on_running_changed_func on_running_changed;

        private List<category_colors> colors_ = new List<category_colors>();
        private List<Color> unused_ = new List<Color>(); 

        public categories_ctrl() {
            InitializeComponent();
            errorStatus.Visible = false;
            update_is_running_text();
            initialize_preview();
        }

        private void isRunning_CheckedChanged(object sender, EventArgs e) {
            update_is_running_text();
        }

        private void update_is_running_text() {
            isRunning.Text = isRunning.Checked ? "Stop" : "Start";
            if ( on_running_changed != null)
                on_running_changed(isRunning.Checked);
        }

        private void objectListView1_CellToolTipShowing(object sender, BrightIdeasSoftware.ToolTipShowingEventArgs e) {

        }

        private void categories_ctrl_Load(object sender, EventArgs e) {

        }

        public void set_category_types(List<string> category_strings, string default_category) {
            categoryTypes.Items.Clear();
            foreach (var str in category_strings)
                categoryTypes.Items.Add(str);
            int sel = category_strings.FindIndex(x => x.ToLower() == default_category.ToLower());
            if (sel >= 0)
                categoryTypes.SelectedIndex = sel;
            else if ( category_strings.Count > 0)
                categoryTypes.SelectedIndex = 0;

            // at this point, we wait to be notified of the possible categories
            categories.Items.Clear();
            set_error("");
        }

        public void set_error(string err) {
            bool has_error = err != "";
            errorStatus.Text = err;
            errorStatus.Visible = has_error;
            isRunning.Visible = !has_error;
            if ( has_error)
                isRunning.Checked = false;
            categories.Visible = !has_error;
            preview.Visible = !has_error;
            previewLabel.Visible = !has_error;
        }

        public void set_categories(List<category_colors> colors) {
            set_error("");

            colors_ = colors.ToList();
            var items = colors_.Select(x => new item(x)).ToList();

            var used = colors_.Select(x => x.bg_color).Where(x => x != util.transparent).Select(x => x.ToArgb()).ToList();
            unused_ = default_color_names_.Select(util.str_to_color).Where(x => used.FindIndex(y => y == x.ToArgb()) < 0 ) .ToList();
            fill_categories_with_default_colors();

            categories.Items.Clear();
            categories.SetObjects(items);

            update_preview();
        }

        private void fill_categories_with_default_colors() {
            // need to assign the FIRST Unused color each time (in case color not set yet)
            foreach ( var category in colors_)
                if (category.bg_color == util.transparent && unused_.Count > 0) {
                    category.bg_color = unused_[0];
                    unused_.RemoveAt(0);
                }            
        }

        private void update_row_color(int idx) {
            var row = categories.GetItem(idx);
            var category = (row.RowObject as item);
            for ( int col = 0; col < categories.Columns.Count; ++col)
                row.GetSubItem(col).BackColor = category.color_by_index(col);
        }

        private void initialize_preview() {
            // this needs to be synchronized with preview_FormatRow + preview_indexes_array !!!
            var preview = new[] {
                new preview_item("11:02:32.012", "Info", "one", "This is a simple preview list."), 
                new preview_item("11:02:37.023", "Debug", "sel", "The normal color is used"), 
                new preview_item("11:02:38.012", "Error", "two", "to compute the 'same' and 'this' colors"), 
                new preview_item("11:05:38.115", "Info", "main", "------"), 
                new preview_item("11:17:32.219", "Info", "sel", "THIS IS THE SELECTED ROW"), 
                new preview_item("12:02:32.518", "Debug", "main", "------"), 
                new preview_item("12:09:32.012", "Info", "one", "You can override both 'same' and 'this'"), 
                new preview_item("14:12:32.012", "Warn", "one", "'same' = color the rows of a category value"), 
                new preview_item("15:05:32.012", "Info", "two", "'this' = color the rows of the SELECTED row"), 
            };
            this.preview.AddObjects(preview);
        }

        // what we consider selection - in the Preview
        private int preview_selected_index() {
            var sel_index = categories.SelectedIndex;
            if (sel_index < 0 && categories.GetItemCount() > 0)
                sel_index = 0;
            return sel_index;
        }

        private int[] preview_indexes_array() {
            // 0 - one, 1- two, 2 - main,  3 - sel, 4 -selected-sel
            var indexes = new[] { 0, 3, 1, 2, 4, 2, 0, 0, 1 };
            return indexes;
        }

        private void update_preview() {
            List<int> category_types = new List<int>();
            var sel_index = preview_selected_index();
            int color_index = 0;
            for (int i = 0; i < 3; ++i) {
                if (color_index == sel_index)
                    ++color_index;
                category_types.Add( color_index < colors_.Count ? color_index : -1);
                ++color_index;
            }
            category_types.Add(sel_index);
            category_types.Add(sel_index);
            var indexes = preview_indexes_array();

            for (int i = 0; i < preview.GetItemCount(); ++i) {
                var row = preview.GetItem(i).RowObject as preview_item;
                if ( category_types[ indexes[i]] >= 0 )
                row.thread = colors_[ category_types[ indexes[i]]].name;
                preview.RefreshItem(preview.GetItem(i));
            }
        }

        private void categories_SelectedIndexChanged(object sender, EventArgs e) {
            update_preview();
        }

        private void categoryTypes_SelectedIndexChanged(object sender, EventArgs e) {
            if (categoryTypes.DroppedDown)
                return;

            if (categoryTypes.SelectedIndex >= 0)
                util.postpone(() => 
                    on_change_category_type( categoryTypes.Items[categoryTypes.SelectedIndex].ToString()), 1);
        }

        private void categories_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e) {
            update_row_color(e.RowIndex);            
        }

        private void categories_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e) {
        }

        private void categories_CellOver(object sender, BrightIdeasSoftware.CellOverEventArgs e) {
            Cursor = e.ColumnIndex > 0 ? Cursors.Hand : Cursors.Default;
        }

        private void categories_CellClick(object sender, BrightIdeasSoftware.CellClickEventArgs e) {
            // logger.Debug("cell click " + e.ColumnIndex);
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex < 1)
                return;

            var row = categories.GetItem( e.RowIndex).RowObject as item;
            var color = row.color_by_index(e.ColumnIndex); 
            var sel = new select_color_form("Choose color...", color);
            if (sel.ShowDialog() == DialogResult.OK) {
                var old_color = row.color_by_index(e.ColumnIndex);
                row.color_by_index(e.ColumnIndex, sel.SelectedColor);
                categories.RefreshObject(row);
                bool click_on_bg_color = e.ColumnIndex == 1;
                if (click_on_bg_color) {
                    bool color_already_used = false;
                    for (int i = 0; i < categories.GetItemCount() && !color_already_used; ++i)
                        if (i != e.RowIndex)
                            if ((categories.GetItem(i).RowObject as item).colors.bg_color.ToArgb() == sel.SelectedColor.ToArgb()) {
                                // in this case, the user selected a color that is already used
                                // have the former row use our old color
                                (categories.GetItem(i).RowObject as item).colors.bg_color = old_color;
                                color_already_used = true;
                                categories.RefreshItem( categories.GetItem(i) );
                            }

                    if ( !color_already_used)
                        unused_.Add(old_color);
                }
                update_preview();
                on_category_colors_change(colors_);
            }
        }


        private void preview_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e) {
            if (e.RowIndex < 0)
                return;
            var row = preview.GetItem(e.RowIndex);
            var indexes = preview_indexes_array();
            Debug.Assert(e.RowIndex < indexes.Length);
            var sel_index = preview_selected_index();
            List<category_colors> colors = new List<category_colors>();
            int color_index = 0;
            for (int i = 0; i < 3; ++i) {
                if (color_index == sel_index)
                    ++color_index;
                colors.Add( color_index < colors_.Count ? colors_[color_index] : null);
                ++color_index;
            }
            // add sel + selected -sel now
            category_colors sel_color = sel_index >= 0 ? colors_[sel_index] : null;
            colors.Add(sel_color);
            colors.Add( sel_color);

            Color default_ = Color.White;
            var index = indexes[e.RowIndex];
            Color bg = default_;
            if (colors[index] != null) {
                if (index == 4)
                    bg = util.darker_color(colors[index].this_category_bg, app.inst.selection_dark_effect);
                else if (index == 3)
                    bg = colors[index].this_category_bg;
                else
                    bg = colors[index].same_category_bg;
            }
            row.BackColor = bg;
        }

        private void categoryTypes_DropDownClosed(object sender, EventArgs e) {
            categoryTypes_SelectedIndexChanged(null, null);
        }

    }

}
