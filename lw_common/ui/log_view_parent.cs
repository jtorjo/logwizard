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
    }
}
