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

namespace lw_common.ui {
    public partial class edit_column_formatters_form : Form {

        private const int MAX_PREVIEW_ROWS = 100;
        private List<int> column_indexes_ = null;

        private log_view lv_;

        public edit_column_formatters_form(log_view lv) {
            lv_ = lv;
            InitializeComponent();

        }


        private void load_surrounding_rows(log_view lv) {
            int sel = lv.sel_row_idx;
            if (sel < 0)
                sel = 0;
            // get as many rows as possible, in both directions
            int max_count = Math.Min(MAX_PREVIEW_ROWS, lv.item_count);
            int min = sel - max_count / 2, max = sel + max_count / 2;
            if (min < 0) {
                max += -min;
                min = 0;
            }
            if (max > lv.item_count) {
                min -= max - lv.item_count;
                max = lv.item_count;
            }
            if (min < 0)
                min = 0;
            if (max > lv.item_count)
                max = lv.item_count;
            // at this point, we know the start and end
            List<match_item> preview_items = new List<match_item>();
            for (int idx = min; idx < max; ++idx) {
                var i = lv.item_at(idx);
                preview_items.Add(i);
            }

            list.AddObjects(preview_items);
        }

        private void help_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/jtorjo/logwizard/wiki/Formatters");
        }

    }
}
