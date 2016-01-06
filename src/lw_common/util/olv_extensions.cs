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

        public static bool is_visible(this OLVColumn col) {
            Debug.Assert(col.Tag != null);

            bool is_line_col = col.fixed_index() == 0;
            bool visible = is_line_col ? col.Width > 1 : col.IsVisible;
            return visible;
        }

        public static void is_visible(this OLVColumn col, bool show) {
            Debug.Assert(col.Tag != null);

            if (col.is_visible() == show)
                return;
            // 1.6.6 for Line column - we can't hide it (due to some weird b_ug in ListView);  so we just set its width to 1
            bool is_line_col = col.fixed_index() == 0;
            if (is_line_col) {
                // for line column - simple trick - save the old width in "Tag" property
                if (show) {
                    col.MaximumWidth = -1;
                    col.Width = col.lv_tag().line_width > 0 ? col.lv_tag().line_width : 80;
                } else {
                    col.lv_tag().line_width = col.Width;
                    col.MaximumWidth = col.Width = 1;
                }
                col.IsVisible = true;
            } else
                col.IsVisible = show;
        }

        public static int col_width(this OLVColumn col) {
            Debug.Assert(col.Tag != null);
            bool is_line_col = col.fixed_index() == 0;
            if (is_line_col) {
                if (col.is_visible())
                    return col.Width;
                else
                    return col.lv_tag().line_width;
            } else
                return col.Width;
        }

        public static void col_width(this OLVColumn col, int width) {
            Debug.Assert(col.Tag != null);

            bool is_line_col = col.fixed_index() == 0;
            bool show = col.is_visible();
            if (is_line_col) {
                if (show) {
                    col.MaximumWidth = -1;
                    col.Width = width;
                } else {
                    col.lv_tag().line_width = width;
                    // ... don't allow resizing
                    col.MaximumWidth = col.Width = 1;
                }
                col.IsVisible = true;
            } else {
                col.Width = width;
                col.IsVisible = show;
            }
        }


        public static int fixed_index(this OLVColumn c) {
            Debug.Assert(c.Tag != null);
            if (c.ListView == null) {
                // 1.6.6
                var tag = c.Tag as log_view_column_tag;
                return tag.parent.list.AllColumns.IndexOf(c);
            }

            // theoretically, we should reference LastDisplayIndex
            // but by looking at the implementation, it looks really fishy...
            return (c.ListView as ObjectListView).AllColumns.IndexOf(c);             
        }
    }
}
