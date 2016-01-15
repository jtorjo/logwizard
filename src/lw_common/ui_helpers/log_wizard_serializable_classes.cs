/* 
 * Copyright (C) 2014-2016 John Torjo
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
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using LogWizard;

namespace lw_common {
    [Serializable]
    public class ui_filter {
        // the filter itself
        public string text = "";
        // if true, it's enabled
        public bool enabled = true;
        // if !enabled, but dimmed, the filter acts the same, only that it shows the lines pertaining to it as gray (dimmed)
        public bool dimmed = false;

        public bool apply_to_existing_lines = false;

        internal void load_save(bool load, string prefix) {
            app.load_save(load, ref text, prefix + "text");
            app.load_save(load, ref enabled, prefix + "enabled", true);
            app.load_save(load, ref dimmed, prefix + "dimmed", false);
            app.load_save(load, ref apply_to_existing_lines,prefix + "apply_to_existing_lines", false);
        }

        public void load(string prefix) {
            load_save(true, prefix);
        }

        public void save(string prefix) {
            load_save(false, prefix);
        }

    }

    [Serializable]
    public class ui_view {
        // friendlly name
        public string name = "";
        // if true, this is the default name (user hasn't manually changed it yet)
        public bool is_default_name = false;
        // the filters
        public List< ui_filter > filters = new List<ui_filter>();

        internal void load_save(bool load, string prefix) {
            app.load_save(load, ref is_default_name, prefix + "is_default_name", false);
            app.load_save(load, ref name, prefix + "name");

            int filter_count = filters.Count;
            app.load_save(load, ref filter_count, prefix + "filter_count", 0);
            if (load) {
                filters.Clear();
                while ( filters.Count < filter_count)
                    filters.Add( new ui_filter());
            }

            for ( int i = 0; i < filter_count; ++i)
                filters[i].load_save(load, prefix + "filt" + i + ".");
        }

        public void load(string prefix) {
            load_save(true, prefix);
        }

        public void save(string prefix) {
            load_save(false, prefix);
        }

    }

    [Serializable]
    public class ui_context {
        public string name  = "";
        private log_settings_string default_settings_ = new log_settings_string("");

        public List<ui_view> views = new List<ui_view>();

        public void merge_settings(log_settings_string_readonly other_sett, bool edited_syntax_now) {
            if ( edited_syntax_now)
                if ( default_settings_.syntax == "")
                    default_settings_.syntax .set(other_sett.syntax);

            default_settings_.aliases.set(other_sett.aliases);
            default_settings_.description_template.set(other_sett.description_template);
        }

        public bool has_views {
            get {
                // never allow "no view" whatsoever
                if (name != "Default")
                    return true;
                if (views.Count < 1)
                    return false;
                // in this case, we have a single view
                if (views[0].is_default_name && views[0].filters.Count < 1)
                    return false;

                return true;
            }
        }

        public bool has_not_empty_views {
            get {
                if (default_settings_.syntax != "" && default_settings_.syntax != find_log_syntax.UNKNOWN_SYNTAX)
                    // user just set the syntax - very likely he'll use this file in the future
                    return true;

                if (views.Count < 1)
                    return false;
                // in this case, we have a single view
                if (views[0].is_default_name && views[0].filters.Count < 1)
                    return false;

                return true;                
            }
        }

        public log_settings_string_readonly default_settings {
            get { return default_settings_; }
        }

        public void copy_from(ui_context other) {
            default_settings_ = other.default_settings_;
            name = other.name;
            views = other.views.ToList();
        }

        private void load_save(bool load, string prefix) {
            app.load_save(load, ref name, prefix + ".name", "Default" );

            string settings_str = default_settings_.ToString();
            if (load) {
                app.load_save(load, ref settings_str, prefix + ".default_settings");
                default_settings_ = new log_settings_string(settings_str);

                if (settings_str == "") {
                    // ... 1.4.8- kept the old name for persistenting
                    app.load_save(load, ref settings_str, prefix + ".default_syntax");
                    if (settings_str != "")
                        default_settings_.syntax. set(settings_str);
                }
            }
            else
                app.load_save(load, ref settings_str, prefix + ".default_settings");

            int view_count = views.Count;
            app.load_save(load, ref view_count, prefix + ".view_count", 0);
            if (load) {
                views.Clear();
                while ( views.Count < view_count)
                    views.Add( new ui_view());
            }

            for (int i = 0; i < view_count; ++i) 
                views[i].load_save(load, prefix + ".view" + i + ".");

            if ( load && views.Count == 0)
                views.Add( new ui_view() { is_default_name = true, name = "View_1" });
        }

        public void load(string prefix) {
            load_save(true, prefix);
        }

        public void save(string prefix) {
            load_save(false, prefix);
        }
    }
}
