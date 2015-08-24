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
        // if true, we show the selected line of each view (the real line in the view)
        public bool show_view_selected_line = true;

        // if true, synchronize all views with existing view
        // (that is, when we selected line changes, the other views should go to the closest line to the selected line in this view)
        public bool sync_all_views = true;
        // if true, synchronize Full Log with existing view (that is, the selected line)
        public bool sync_full_log_view = true;


    }

}
