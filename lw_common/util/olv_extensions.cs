using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;

namespace lw_common {
    public static class olv_extensions {

        public static int fixed_index(this OLVColumn c) {
            // theoretically, we should reference LastDisplayIndex
            // but by looking at the implementation, it looks really fishy...
            return (c.ListView as ObjectListView).AllColumns.IndexOf(c);             
        }

    }
}
