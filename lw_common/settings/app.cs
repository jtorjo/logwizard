using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using LogWizard;

namespace lw_common {
    // application settings
    public class app {
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

        public bool use_bg_gradient = true;
        public Color bg = Color.White;
        public Color bg_from = Color.White;
        public Color bg_to = Color.AntiqueWhite;


        // what extensions to look at - when parsing a zip file (in other words, what files do we consider "probable" logs)
        public string look_into_zip_files = "";

        // when creating/modifying notes - here is what identifies this author
        public string notes_author_name = "";
        public string notes_initials = "";
        public Color notes_color = Color.Blue;

        // how do we uniquely identify the notes file for a certain log?
        public md5_log_keeper.md5_type identify_notes_files = md5_log_keeper.md5_type.fast;


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

        private void load_save(bool load) {
            load_save(load, ref show_view_line_count, "show_view_line_count", true);
            load_save(load, ref show_view_selected_line, "show_view_selected_line", true);
            load_save(load, ref show_view_selected_index, "show_view_selected_index", true);
            load_save(load, ref sync_all_views, "sync_all_views");
            load_save(load, ref sync_full_log_view, "sync_full_log_view", true);

            load_save(load, ref show_topmost_toggle, "show_topmost_toggle");

            load_save(load, ref syncronize_colors, "synchronize_colors", synchronize_colors_type.with_all_views);
            load_save(load, ref sync_colors_all_views_gray_non_active, "synchronize_colors_gray_non_active", false);

            load_save(load, ref use_bg_gradient, "use_bg_gradient", true);
            load_save(load, ref bg, "bg", Color.White);
            load_save(load, ref bg_from, "bg_from", Color.White);
            load_save(load, ref bg_to, "bg_to", util.str_to_color("#FEFBF8") );

            load_save(load, ref look_into_zip_files, "look_into_zip_files", ".log;.txt");
            load_save(load, ref notes_author_name, "notes_author_name", Environment.UserName);
            load_save(load, ref notes_initials, "notes_initials", initials(notes_author_name));
            load_save(load, ref notes_color, "notes_color", Color.Blue);
            load_save(load, ref identify_notes_files, "identify_notes_files", md5_log_keeper.md5_type.fast);
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

        public void load() {
            load_save(true);
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
