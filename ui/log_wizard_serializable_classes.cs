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
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogWizard {
    [Serializable]
    public class ui_filter {
        // the filter itself
        public string text = "";
        // if true, it's enabled
        public bool enabled = true;
        // if !enabled, but dimmed, the filter acts the same, only that it shows the lines pertaining to it as gray (dimmed)
        public bool dimmed = false;

        public bool apply_to_existing_lines = false;
    }

    [Serializable]
    public class ui_view {
        // friendlly name
        public string name = "";
        // the filters
        public List< ui_filter > filters = new List<ui_filter>();

        // 1.0.77+ - not saved to settings at this time
        public bool show_msgonly = false;
    }

    [Serializable]
    public class ui_context {
        public string name  = "";
        public string auto_match = "";

        // show/hide toggles
        public bool show_filter = true;
        public bool show_source = true;
        public bool show_fulllog = false;

        public bool show_current_view = true;
        public bool show_header = true;
        public bool show_tabs = true;
        public bool show_title = true;
        public bool topmost = false;
        public bool show_status = true;

        // not implemented yet
        public bool show_details = false;

        public List<ui_view> views = new List<ui_view>();

        public bool has_views {
            get {
                // never allow "no view" whatsoever
                if (name != "Default")
                    return true;

                if (views.Count < 1)
                    return false;

                // in this case, we have a single view
                if (views[0].name == "New_1" && views[0].filters.Count < 1)
                    return false;

                return true;
            }
        }

        public void copy_from(ui_context other) {
            name = other.name;
            auto_match = other.auto_match;
            views = other.views.ToList();
            show_filter = other.show_filter;
            show_source = other.show_source;
            show_fulllog = other.show_fulllog;

            show_current_view = other.show_current_view;
            show_header = other.show_header;
            show_tabs = other.show_tabs;
            show_title = other.show_title;
            topmost = other.topmost;
            show_details = other.show_details;
            show_status = other.show_status;
        }
    }
}
