using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;

namespace lw_common {
    public static class olv_extensions {

        public static int fixed_index(this OLVColumn c) {
            if (c.ListView == null)
                // why would listview EVER become null? beats the hell out of me; first, I was extremely nervous at looking at this, but then, I realized:
                // this can happpen if the column is not visible at all (like, for msg column, when it's completely invisible - I'm assuming OLV will completely remove it in this case)
                return c.LastDisplayIndex;

            // theoretically, we should reference LastDisplayIndex
            // but by looking at the implementation, it looks really fishy...
            return (c.ListView as ObjectListView).AllColumns.IndexOf(c);             
        }
    }
}
