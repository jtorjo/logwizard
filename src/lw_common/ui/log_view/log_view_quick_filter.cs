using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui {

    // this filter is very fast 
    internal class log_view_quick_filter {
        private readonly snoop_filter snoop_filter_;

        public log_view_quick_filter(snoop_filter snoop_filter) {
            snoop_filter_ = snoop_filter;
        }

        public bool matches(match_item item) {
            return snoop_filter_.matches(item);
        }

        // if this returns true, then the quick filter matches all items, so no point in running it at all
        public bool matches_all() {
            // IMPORTANT: in the future, for the quick filter based on date/time or start/end
            //            if it matches all items, this should return true!
            return snoop_filter_.matches_all();
        }
    }
}
