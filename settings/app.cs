using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogWizard {
    // application settings
    class app {
        private static app inst_= new app();

        public static app inst {
            get { return inst_; }
        }

        // if true, we show how many lines each view has
        public bool show_view_line_count = true;
        // if true, we show the selected line index of each view
        public bool show_view_selected_index = true;
        // if true, we show the selected line of each view (the real line in the log)
        public bool show_view_selected_line = true;

        // if true, synchronize all views with existing view
        // (that is, when we selected line changes, the other views should go to the closest line to the selected line in this view)
        //
        // 1.0.56+ - don't have it by default, for large files (20+Mb) it's slow
        public bool sync_all_views = false;
        // if true, synchronize Full Log with existing view (that is, the selected line)
        public bool sync_full_log_view = true;

        // if true, I instantly refresh all views all the time
        // if false, I refresh only the current view
        public bool instant_refresh_all_views = true;

        public void load() {
            var sett = Program.sett;
            show_view_line_count = sett.get("show_view_line_count", "1") != "0";
            show_view_selected_index = sett.get("show_view_selected_index", "1") != "0";
            show_view_selected_line = sett.get("show_view_selected_line", "1") != "0";

            sync_all_views = sett.get("sync_all_views", "0") != "0";
            sync_full_log_view = sett.get("sync_full_log_view", "1") != "0";
        }

        public void save() {
            var sett = Program.sett;
            sett.set("show_view_line_count", show_view_line_count ? "1" : "0");
            sett.set("show_view_selected_line", show_view_selected_line ? "1" : "0");
            sett.set("show_view_selected_index", show_view_selected_index ? "1" : "0");

            sett.set("sync_full_log_view", "" + (sync_full_log_view ? "1" : "0"));
            sett.set("sync_all_views", "" + (sync_all_views ? "1" : "0"));

            sett.save();            
        }


    }

}
