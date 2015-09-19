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
using System.Threading.Tasks;

namespace LogWizard {
    // information about the UI of LogWizard - location, toggles, etc.
    public class ui_info {
        public int left, top, width, height = 0;
        public bool maximized = false;

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

        public string selected_view = "";
        public string log_name = ""; // the name of the log 
        public int selected_row_idx = -1;
        public int full_log_splitter_pos = -1;


        public void copy_from(ui_info other) {
            left = other.left;
            top = other.top;
            width = other.width;
            height = other.height;

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
            log_name = other.log_name;

            selected_view = other.selected_view;
            selected_row_idx = other.selected_row_idx;
            full_log_splitter_pos = other.full_log_splitter_pos;
        }


        private void load_save(bool load, string prefix) {
            app.load_save(load, ref left, prefix + ".left", -1);
            app.load_save(load, ref top, prefix + ".top", -1);
            app.load_save(load, ref width, prefix + ".width", -1);
            app.load_save(load, ref height, prefix + ".height", -1);
            app.load_save(load, ref maximized, prefix + "maximized", false);

            app.load_save(load, ref show_filter, prefix + ".show_filter", true);
            app.load_save(load, ref show_source, prefix + ".show_source", true);
            app.load_save(load, ref show_fulllog, prefix + ".show_fulllog", false);

            app.load_save(load, ref show_current_view, prefix + ".show_current_view", true);
            app.load_save(load, ref show_header, prefix + ".show_header", true);
            app.load_save(load, ref show_status, prefix + ".show_status", true);
            app.load_save(load, ref show_tabs, prefix + ".show_tabs", true);
            app.load_save(load, ref show_title, prefix + ".show_title", true);
            app.load_save(load, ref show_details, prefix + ".show_details", false);
            app.load_save(load, ref topmost, prefix + ".topmost", false);

            app.load_save(load, ref selected_view, prefix + "selected_view", selected_view);
            app.load_save(load, ref log_name, prefix + "log_name");
            app.load_save(load, ref selected_row_idx, prefix + "selected_row_idx", -1);
            app.load_save(load, ref full_log_splitter_pos, prefix + "full_log_splitter_pos", -1);            
        }

        public void load(string prefix) {
            load_save(true, prefix);
        }

        public void save(string prefix) {
            load_save(false, prefix);
        }
    }
}
