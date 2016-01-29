using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using lw_common.ui.format;

namespace lw_common.ui {
    public partial class edit_column_formatters_form : Form {

        private const int MAX_PREVIEW_ROWS = 100;

        private log_view lv_;
        private column_formatter_renderer render_ ;

        private string prev_syntax_ = "";

        public edit_column_formatters_form(log_view lv) {
            lv_ = lv;
            InitializeComponent();
            list.VirtualMode = false;
            list.Font = lv.list.Font;

            var formatter = new column_formatter_array();
            formatter.load(lv_.formatter.syntax);
            render_ = new column_formatter_renderer(lv, list);
            render_.formatter = formatter;

            syntax.Text = prev_syntax_ = render_.formatter.syntax;
                        
            update_column_visibility();
            load_surrounding_rows();
        }

        private void update_column_visibility() {
            for (int i = 0; i < lv_.list.AllColumns.Count; ++i) {
                var col = list.AllColumns[i];
                col.IsVisible = lv_.list.AllColumns[i].IsVisible;
                col.Width = lv_.list.AllColumns[i].Width;
                col.LastDisplayIndex = lv_.list.AllColumns[i].LastDisplayIndex;
                col.Tag = new log_view_column_tag(lv_);
                col.Renderer = render_;
            }
            list.RebuildColumns();
        }

        private void load_surrounding_rows() {
            int sel = lv_.sel_row_idx;
            if (sel < 0)
                sel = 0;
            // get as many rows as possible, in both directions
            int max_count = Math.Min(MAX_PREVIEW_ROWS, lv_.item_count);
            int min = sel - max_count / 2, max = sel + max_count / 2;
            if (min < 0) {
                max += -min;
                min = 0;
            }
            if (max > lv_.item_count) {
                min -= max - lv_.item_count;
                max = lv_.item_count;
            }
            if (min < 0)
                min = 0;
            if (max > lv_.item_count)
                max = lv_.item_count;
            // at this point, we know the start and end
            List<match_item> preview_items = new List<match_item>();
            for (int idx = min; idx < max; ++idx) {
                var i = lv_.item_at(idx);
                preview_items.Add(i);
            }

            list.AddObjects(preview_items);
        }

        private void help_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/jtorjo/logwizard/wiki/Formatters");
        }

        private void ok_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
        }

        private void cancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }

        private void refresh_Tick(object sender, EventArgs e) {
            var cur_syntax = syntax.Text;
            if (prev_syntax_ != cur_syntax) {
                prev_syntax_ = cur_syntax;
                var new_formater = new column_formatter_array();
                string errors = "";
                new_formater.load(cur_syntax, ref errors);
                if (errors == "") {
                    render_.formatter = new_formater;
                    list.Refresh();
                    previewStatus.Text = "Previewing " + list.GetItemCount() + " items.";
                    previewStatus.ForeColor = Color.Blue;
                } else {
                    previewStatus.Text = errors;
                    previewStatus.ForeColor = Color.Red;
                }
            }
        }

    }
}
