/* 
 * Copyright (C) 2014-2015 John Torjo
 *
 * This file is part of LogWizard
 *
 * LogWizard is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * LogWizard is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact john.code@torjo.com 
 *  * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;
using lw_common.ui;

namespace lw_common {
    internal static class olv_extensions {

        public static log_view_column_tag lv_tag(this OLVColumn c) {
            var tag = c.Tag as log_view_column_tag;
            Debug.Assert(tag != null);
            return tag;
        }

        public static int fixed_index(this OLVColumn c) {
            if (c.ListView == null) {
                // 1.6.6
                var tag = c.Tag as log_view_column_tag;
                if (tag != null)
                    return tag.parent.list.AllColumns.IndexOf(c);

                // why would listview EVER become null? beats the hell out of me; first, I was extremely nervous at looking at this, but then, I realized:
                // this can happpen if the column is not visible at all (like, for msg column, when it's completely invisible - I'm assuming OLV will completely remove it in this case)
                return c.LastDisplayIndex;
            }

            // theoretically, we should reference LastDisplayIndex
            // but by looking at the implementation, it looks really fishy...
            return (c.ListView as ObjectListView).AllColumns.IndexOf(c);             
        }
    }
}
