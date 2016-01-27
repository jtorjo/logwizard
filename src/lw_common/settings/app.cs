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
 *  * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using log4net.Repository.Hierarchy;
using LogWizard;

namespace lw_common {
    // application settings
    public class app {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static app inst_= new app();

        public static app inst {
            get { return inst_; }
        }

        private settings_file sett_ = null;

        public settings_file sett {
            get {
                Debug.Assert(sett_ != null);
                return sett_;
            }
        }

        public const string DEFAULT_COLUMN_SYNTAX = "[line]\r\n" +
                                                    "color-all\r\n" +
                                                    "color=#2B91AF\r\n" +
                                                    "\r\n" +
                                                    "[all]\r\n" +
                                                    "format\r\n" +
                                                    "date.color=blue\r\n" +
                                                    "date.light_color=#adc7e8\r\n" +
                                                    "time.color=blue\r\n" +
                                                    "time.light_color=#adc7e8";

        // these are settings that are NOT shown in the UI
        public class no_ui_ {
            //public ulong file_max_read_in_one_go = (ulong) (util.is_debug ? 128 * 1024 : 16 * 1024 * 1024);
            public ulong file_max_read_in_one_go = (ulong) (util.is_debug ? 2 * 1024 * 1024 : 16 * 1024 * 1024);
            // ... after the file has been fully read, we don't expect that much info to be written in a few milliseconds
            //     thus, make the muffer MUCH smaller
            public ulong file_max_read_in_one_go_after_fully_read = (ulong) (util.is_debug ? 8 * 1024 : 128 * 1024);

            public int min_filter_capacity = 50000;
            public int min_list_data_source_capacity = 75000;
            public int min_matched_lines_capacity = 20000;
            public int min_lines_capacity = 50000;

            // if true, we read the full log first (then compute filters) - so far (1.0.76d), this seems to use less memory
            // if false, we compute filters in paralel as the log is being read
            public bool read_full_log_first = true;
        }
        public no_ui_ no_ui = new no_ui_();


        // ... for file-to-file settings
        private string selected_log_file_name_ = "";

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

        // if true, we show the Topmost button (top-left)
        public bool show_topmost_toggle = false;

        public enum synchronize_colors_type {
            none, with_current_view, with_all_views
        }

        public synchronize_colors_type syncronize_colors = synchronize_colors_type.none;
        public bool sync_colors_all_views_gray_non_active = false;

        public bool use_bg_gradient = false;
        public Color bg = Color.White;
        public Color fg = Color.Black;

        public Color full_log_gray_fg = Color.LightSlateGray;        
        public Color full_log_gray_bg = Color.White;

        public Color dimmed_fg = Color.LightGray;
        public Color dimmed_bg = Color.White;

        public Color bookmark_bg = Color.Blue, bookmark_fg = Color.White;

        // 1.3.29+ - hide this - it was mainly for testing - i know it's a bit cool, but let just ignore it for now
        public Color bg_from = Color.White;
        public Color bg_to = Color.AntiqueWhite;

        // 1.3.35+ - note - at this time, I don't allow changing these
        public Color search_found_fg = Color.Blue;
        public Color search_found_full_line_fg = Color.OrangeRed;

        // what extensions to look at - when parsing a zip file (in other words, what files do we consider "probable" logs)
        public string look_into_zip_files_str = "";

        public List<string> look_into_zip_files {
            get { return look_into_zip_files_str.Split(';').ToList(); }
        } 

        // when creating/modifying notes - here is what identifies this author
        public string notes_author_name = "";
        public string notes_initials = "";
        public Color notes_color = Color.Blue;

        // how do we uniquely identify the notes file for a certain log?
        public md5_log_keeper.md5_type identify_notes_files = md5_log_keeper.md5_type.fast;

        public bool use_hotkeys = true;

        public Dictionary<string, string> file_to_context = new Dictionary<string, string>();
        public Dictionary<string, string> file_to_syntax = new Dictionary<string, string>();
        
        // 1.5.6+ - obsolete - it's used only to match files-to-context in the old history combo
        // 1.1.5+ - forced contexts (for instance, when imported from .logwizard files)
        public Dictionary<string,string> forced_file_to_context = new Dictionary<string, string>();

        public enum edit_mode_type {
            always, with_space, with_right_arrow
        }
        public edit_mode_type edit_mode = edit_mode_type.always;

        // if true, when the user types something, and we can't find the word, search ahead
        public bool edit_search_after = true;
        // if true, when the user types something, and we can't find the word ahead, search before as well
        public bool edit_search_before = true;
        // if true, when searching something, search all columns (current column first!)
        // 1.6.6 - made the default to true
        public bool edit_search_all_columns = true;
        // if true, clicking a word selects it
        public bool edit_click_word_selects_it = true;

        public bool show_filter_row_in_filter_color = true;

        // at what interval should we check for new lines?
        public int check_new_lines_interval_ms = 50;

        public string font_name = "";
        // note: we can keep several font names, just in case one is not present on the user's machine (read-only)
        private string[] default_font_names_;
        public int font_size = 9;

        public bool use_file_monitoring_api = false;

        public bool show_beta_releases = true;
        public bool show_variable_fonts_as_well = false;

        public bool show_tips = true;

        public int run_count = 0;

        // 1.5.9+
        public List<string> description_layouts_ = new List<string>();
        public int description_layout_idx_ = 0;

        // (for testing only) if true, we simulate that text is multi-line (to test selection of relevant line)
        public readonly bool force_text_as_multi_line = false; // util.is_debug;

        // 1.5.10+ - if true, and text is multi-line, in log-view: depending on what line is shown, I will show wether there are lines before and after
        // note: at this time, there's no UI setting for this
        public readonly bool show_paragraph_sign = true;

        // 1.5.18+ - first time we have a multi-line column, we show the details pane
        public bool has_shown_details_pane = false;

        // 1.6.6+ - allow showing a horizontal scrollbar in log-view
        public bool show_horizontal_scrollbar = false;

        // 1.6.17+ - if true, we associate common extensions to LogWizard (.log, .txt and .zip - for now)
        public bool associate_common_extensions = false;

        // 1.6.25+ - if true, we auto open last log (only on the default window, not on custom1-9)
        public bool auto_open_last_log;

        // 1.7.2+ - not in UI yet - in case we're reading from C:\Windows\System32\winevt\Logs, how many logs should we return?
        public int max_event_log_files = 50;

        // 1.7.8 - column formatting
        public string default_column_format = "";

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // file-by-file
        public bool bring_to_top_on_restart = false;
        public bool make_topmost_on_restart = false;

        public void set_log_file(string file) {
            if (selected_log_file_name_ != "") 
                // save_to old settings
                load_save_file_by_file(false);

            selected_log_file_name_ = file;
            load_save_file_by_file(true);
        }

        private void load_save_file_by_file(bool load) {
            var sett = inst.sett;
            if (load) {
                string[] words = sett.get("settings_by_file." + selected_log_file_name_).Split(',');
                bring_to_top_on_restart = false;
                make_topmost_on_restart = true; // ... default
                foreach (string word in words)
                    switch (word) {
                    case "bring_to_top_on_restart":
                        bring_to_top_on_restart = true;
                        break;
                    case "not_make_topmost_on_restart":
                        make_topmost_on_restart = false;
                        break;
                    }
            } else {
                string words = "";
                if (bring_to_top_on_restart)
                    words += "bring_to_top_on_restart,";
                if (!make_topmost_on_restart)
                    words += "not_make_topmost_on_restart,";
                sett.set("settings_by_file." + selected_log_file_name_, words);
                sett.save();
            }
        }

        public void init(settings_file sett_file) {
            Debug.Assert(sett_ == null);
            sett_ = sett_file;
            load();
            ++run_count;
        }

        internal static void load_save(bool load, ref bool prop, string name, bool default_ = false) {
            var sett = inst.sett;
            if (load)
                prop = sett.get(name, default_ ? "1" : "0") != "0";
            else 
                sett.set(name, prop ? "1" : "0");
        }

        internal static void load_save<T>(bool load, ref T prop, string name, T default_) {
            var sett = inst.sett;
            if (load) {
                string val = sett.get(name);
                if (val == "")
                    prop = default_;
                else 
                    prop = (T) (object) int.Parse( sett.get(name));

            } else
                sett.set(name, "" + (int)(object)prop);
        }
        internal static void load_save(bool load, ref string prop, string name, string default_ = "") {
            var sett = inst.sett;
            if (load)
                prop = sett.get(name, default_);
            else 
                sett.set(name, prop);
        }
        internal static void load_save(bool load, ref Color prop, string name, Color default_) {
            var sett = inst.sett;
            if (load) {
                string def_str = util.color_to_str(default_);
                string as_str = sett.get(name, def_str);
                prop = util.str_to_color(as_str);
            } else
                sett.set(name, util.color_to_str( prop));
        }

        internal static void load_save(bool load, ref Dictionary<string,string> prop, string name) {
            var sett = inst.sett;
            if (load) {
                prop.Clear();
                int count = int.Parse( sett.get(name + ".count", "0"));
                for (int i = 0; i < count; ++i) {
                    string key = sett.get(name + "." + i + ".key");
                    string value = sett.get(name + "." + i + ".value");
                    if ( !prop.ContainsKey(key))
                        prop.Add(key, value);
                    else 
                        logger.Error("invalid settings " + key + "/" + value);
                }
            } else {
                sett.set(name + ".count", "" + prop.Count);
                int i = 0;
                foreach (var val in prop) {
                    sett.set(name + "." + i + ".key", val.Key);
                    sett.set(name + "." + i + ".value", val.Value);
                    ++i;
                }
            }
        }

        internal static void load_save(bool load, ref List<string> prop, string name) {
            var sett = inst.sett;
            if (load) {
                prop.Clear();
                int count = int.Parse( sett.get(name + ".count", "0"));
                for (int i = 0; i < count; ++i) 
                    prop.Add( sett.get(name + "." + i));
            } else {
                sett.set(name + ".count", "" + prop.Count);
                for ( int i = 0 ; i < prop.Count; ++i)
                    sett.set(name + "." + i, prop[i]);
            }
        }

        public Font font {
            get {
                if ( font_name != "")
                    try {
                        return new Font(font_name, font_size); 
                    } catch {
                    }
                foreach ( var name in default_font_names_)
                    try {
                        return new Font(name, font_size); 
                    } catch {
                    }

                Debug.Assert(false);
                return new Font(FontFamily.GenericMonospace, font_size);
            }
        }

        private void load_save(bool load) {
            load_save(load, ref show_view_line_count, "show_view_line_count", true);
            load_save(load, ref show_view_selected_line, "show_view_selected_line", false);
            load_save(load, ref show_view_selected_index, "show_view_selected_index", true);
            load_save(load, ref sync_all_views, "sync_all_views");
            load_save(load, ref sync_full_log_view, "sync_full_log_view", true);

            load_save(load, ref show_topmost_toggle, "show_topmost_toggle");

            load_save(load, ref syncronize_colors, "synchronize_colors", synchronize_colors_type.with_all_views);
            load_save(load, ref sync_colors_all_views_gray_non_active, "synchronize_colors_gray_non_active", false);

            load_save(load, ref use_bg_gradient, "use_bg_gradient", false);

            load_save(load, ref fg, "fg", Color.Black);
            load_save(load, ref bg, "bg", Color.White);

            load_save(load, ref bookmark_fg, "bookmark_fg", Color.White);
            load_save(load, ref bookmark_bg, "bookmark_bg", Color.Blue);

            load_save(load, ref dimmed_fg, "dimmed_fg", Color.LightGray);
            load_save(load, ref dimmed_bg, "dimmed_bg", Color.White);

            load_save(load, ref full_log_gray_fg, "full_log_gray_fg", Color.LightSlateGray);
            load_save(load, ref full_log_gray_bg, "full_log_gray_bg", Color.White);

            load_save(load, ref bg_from, "bg_from", Color.White);
            load_save(load, ref bg_to, "bg_to", util.str_to_color("#FEFBF8") );

            load_save(load, ref look_into_zip_files_str, "look_into_zip_files", ".log;.txt");
            load_save(load, ref notes_author_name, "notes_author_name", Environment.UserName);
            load_save(load, ref notes_initials, "notes_initials", initials(notes_author_name));
            load_save(load, ref notes_color, "notes_color", Color.Blue);
            load_save(load, ref identify_notes_files, "identify_notes_files", md5_log_keeper.md5_type.fast);

            load_save(load, ref use_hotkeys, "use_hotkeys", true);

            load_save(load, ref file_to_context, "file_to_context");
            load_save(load, ref file_to_syntax, "file_to_syntax");
            load_save(load, ref forced_file_to_context, "forced_file_to_context");

            load_save(load, ref edit_mode, "edit_mode", edit_mode_type.always);

            load_save(load, ref edit_search_after, "edit_search_after", true);
            load_save(load, ref edit_search_before, "edit_search_before", true);
            load_save(load, ref edit_search_all_columns, "edit_search_all_columns", true);

            load_save(load, ref show_filter_row_in_filter_color, "show_filter_row_in_filter_color", true);

            load_save(load, ref font_name, "font_name");
            load_save(load, ref font_size, "font_size", 9);

            // note: default font names are read-only
            if (load) {
                string default_font_names = "";
                load_save(load, ref default_font_names, "default_font_names");
                if (default_font_names == "")
                    // older version used this name
                    load_save(load, ref default_font_names, "font_names");
                default_font_names_ = default_font_names.Split(',');
            }

            load_save(load, ref use_file_monitoring_api, "use_file_monitoring_api", false);
            load_save(load, ref show_beta_releases, "show_beta_releases", true);
            load_save(load, ref show_variable_fonts_as_well, "show_variable_fonts_as_well", false);
            load_save(load, ref show_tips, "show_tips", true);
            load_save(load, ref run_count, "run_count", 0);

            load_save(load, ref description_layouts_, "description_layouts");
            load_save(load, ref description_layout_idx_, "description_layout_idx", 0);

            load_save(load, ref has_shown_details_pane, "has_shown_details_pane", false);
            load_save(load, ref show_horizontal_scrollbar, "show_horizontal_scrollbar", false);
            load_save(load, ref edit_click_word_selects_it, "edit_click_word_selects_it", true);

            load_save(load, ref associate_common_extensions, "associate_common_extensions", false);
            load_save(load, ref auto_open_last_log, "auto_open_last_log", true);
            load_save(load, ref default_column_format, "default_column_format", DEFAULT_COLUMN_SYNTAX);
        }

        private string initials(string name) {
            var by_char = name.Select(c => new string(c,1));
            
            string camel = by_char.Select(c => c == c.ToUpper() ? c : "").Aggregate("", (d,s) => d + s);
            string by_sep = name.Length > 0 ? "" + name[0] : "";
            for ( int i = 1; i < name.Length; ++i)
                if ( !Char.IsWhiteSpace(name[i]) && !Char.IsPunctuation(name[i]))
                    if (Char.IsWhiteSpace(name[i - 1]) || Char.IsPunctuation(name[i - 1]))
                        by_sep += name[i];
            if (by_sep.Length == 1)
                by_sep = "";

            if (camel != "" && by_sep != "")
                // ... take the shortest
                return by_sep.Length < camel.Length ? by_sep : camel;

            // JohnTorjo -> JT
            if (camel != "")
                return camel;
            // john_torjo_is_awesome -> jtia
            if (by_sep != "")
                return by_sep.ToUpper();

            // take the first 3 letters
            string ini = name.Length > 3 ? name.Substring(0, 3) : name;
            return ini.ToUpper();
        }

        private void load() {
            load_save(true);
            if (notes_author_name == "")
                notes_author_name = Environment.UserName;
            if (notes_initials == "")
                notes_initials = initials(notes_author_name);
        }

        public void save() {
            load_save(false);
            if ( selected_log_file_name_ != "")
                load_save_file_by_file(false);
            var sett = inst.sett;
            sett.save();            
        }


    }

}
