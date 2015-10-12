using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LogWizard;

namespace lw_common.ui {
    public interface log_view_parent {
        void handle_subcontrol_keys(Control c);
        void on_view_name_changed(log_view view, string name);
        void on_sel_line(log_view lv, int line_idx);

        string matched_logs(int line_idx);
        Rectangle client_rect_no_filter { get; }

        // read-only
        ui_info current_ui { get; }

        void simple_action(log_view_right_click.simple_action simple);

        void add_or_edit_filter(string filter_str, string filter_id , bool apply_to_existing_lines);

        // called after we've searched to something (thus, changed the current line)
        void after_search();

        void select_filter_rows(List<int> filter_row_indexes);

        void edit_filter_row(int filter_row_idx);
    }
}
