using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lw_common;

namespace LogWizard {

    internal class history {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private log_settings_string settings_ = new log_settings_string("");

        public log_type type {
            get {
                return settings_.type;
            }
        }

        public string name {
            get { return settings.name; }
        }

        // uniquely identifies this entry (across all history)
        public string guid {
            get {
                string guid = settings.guid;
                Debug.Assert(guid != "");
                return guid;
            }
        }

        public string friendly_name {
            get { return settings.friendly_name; }
        }

        public string ui_friendly_name {
            get {
                if (friendly_name != "")
                    return friendly_name;

                switch ( type) {
                case log_type.file: 
                    var fi = new FileInfo(name);
                    string ui = fi.Name + " - " + util.friendly_size(fi.Length) + " (" + fi.DirectoryName + ")";
                    return ui;

                //case log_type.shmem: return "Shared Memory: " + name;

                case log_type.event_log: return name;
                case log_type.debug_print: return "Debug: " + name;
                case log_type.db:
                    return "Database: " + settings.db_connection_string;
                default: Debug.Assert(false); break;
                }

                return name;
            }
        }

        public string ui_short_friendly_name {
            get {
                if (friendly_name != "")
                    return friendly_name;

                switch ( type) {
                case log_type.file: 
                    var fi = new FileInfo(name);
                    string ui = fi.Name;
                    return ui;

                //case log_type.shmem: return "Shared Memory: " + name;

                case log_type.event_log: return name;
                case log_type.debug_print: return "Debug: " + name;
                default: Debug.Assert(false); break;
                }

                return name;
            }
        }

        public log_settings_string_readonly settings {
            get { return settings_; }
        }
        public log_settings_string write_settings {
            get { return settings_; }
        }

        public void from_text_reader(text_reader reader) {
            settings_ = reader.write_settings;
        }

        public void from_settings(log_settings_string sett) {
            settings_ = sett;
        }
    }

    internal class history_list {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // 1.6.25b+ - note: we ONLY ADD to this array (we don't remove)
        private static List<history> static_history_ = new List<history>();

        // if Value = true, we already have built the history for this form
        private Dictionary<Form, bool> lw_to_history_exists_ = new Dictionary<Form, bool>(); 

        private Dictionary<Form, List<history> > lw_to_history_ = new Dictionary<Form, List<history>>();

        public void add_history(history hist) {
            // ... just in case existed already
            static_history_.Remove(hist);

            static_history_.Add(hist);
            
            // force rebuilding
            lw_to_history_exists_.Clear();
        }

        public List<history> get_history(Form form, ComboBox box, ui_info info, int custom_ui_idx) {
            if (!lw_to_history_exists_.ContainsKey(form)) {
                // needs rebuilding
                lw_to_history_exists_.Add(form, true);
                var old_sel = lw_to_history_.ContainsKey(form) && box.SelectedIndex >= 0 ? lw_to_history_[form][box.SelectedIndex] : null;

                if ( !lw_to_history_.ContainsKey(form))
                    lw_to_history_.Add(form, null);
                lw_to_history_[form] = static_history_.ToList();

                int guid_idx = lw_to_history_[form].FindIndex(x => x.guid == info.last_log_guid );
                if (guid_idx >= 0) {
                    // care about the last log (show it last)
                    var last = lw_to_history_[form][guid_idx];
                    lw_to_history_[form].RemoveAt(guid_idx);
                    lw_to_history_[form].Add(last);
                }

                logger.Debug("recreating history combo for " + custom_ui_idx);
                update_history(box, lw_to_history_[form], old_sel);
            }

            return lw_to_history_[form];
        } 

        private void update_history(ComboBox box, List<history> history_, history old_sel ) {
            box.Items.Clear();
            foreach (history hist in history_)
                box.Items.Add(hist.ui_friendly_name);

            box.SelectedIndex = old_sel != null ? history_.IndexOf(old_sel) : -1;
        }

        public void recreate_combo(Form form, ComboBox box, ui_info info, int custom_ui_idx) {
            lw_to_history_exists_.Remove(form);
            get_history(form, box, info, custom_ui_idx);
        }

        public void load() {
            var sett = app.inst.sett;

            int history_count = int.Parse( sett.get("history_count", "0"));
            for (int idx = 0; idx < history_count; ++idx) {
                history hist = new history();
                string guid = sett.get("history." + idx + ".guid");
                if (guid != "") {
                    // 1.5.6+ - guid points to the whole settings
                    string settings = sett.get("guid." + guid);
                    if (settings == "") {
                        logger.Debug("history guid removed " + guid);
                        continue; // entry removed
                    }
                    Debug.Assert(settings.Contains(guid));
                    hist.from_settings(new log_settings_string(settings));  
                } else {
                    // old code (pre 1.5.6)
                    string type_str = sett.get("history." + idx + "type", "file");
                    if (type_str == "0")
                        type_str = "file";
                    string name = sett.get("history." + idx + "name");
                    string friendly_name = sett.get("history." + idx + "friendly_name");

                    var history_sett = new log_settings_string("");
                    history_sett.type.set( (log_type) Enum.Parse(typeof (log_type), type_str));
                    history_sett.name.set(name);
                    history_sett.friendly_name.set(friendly_name);
                    // create a guid now
                    history_sett.guid.set(Guid.NewGuid().ToString());
                    hist.from_settings(history_sett);
                }

                static_history_ .Add( hist );
            }
            static_history_ = static_history_ .Where(h => {
                if (h.type == log_type.file) {
                    // 1.5.11 - don't include this into the list next time the user opens the app
                    //          (so that he'll see the "Drop me like it's hot" huge message)
                    if (h.name.ToLower().EndsWith("logwizardsetup.sample.log"))
                        return false;
                    // old name of this sample file
                    if (h.name.ToLower().EndsWith("logwizardsetupsample.log"))
                        return false;

                    if (File.Exists(h.name))
                        // 1.1.5+ - compute md5s for this
                        md5_log_keeper.inst.compute_default_md5s_for_file(h.name);
                    else
                        return false;
                }
                return true;
            }).ToList();            
        }

        public void save() {
            var sett = app.inst.sett;
            sett.set( "history_count", "" + static_history_.Count);
            for ( int idx = 0; idx < static_history_.Count; ++idx) {
                // 1.5.6+ - save settings-per-log
                var settings = static_history_[idx].settings.ToString();
                string guid = static_history_[idx].settings.guid;
                Debug.Assert(guid != "");
                var file = static_history_[idx].settings.type == log_type.file ? static_history_[idx].settings.name : "";

                // note : the way I have settings point to guid is this: I may actually allow user to delete static_history
                //        however, if after deleting static_history, user opens a file he opened before, we want those settings to still take effect.
                sett.set("history." + idx + ".guid", guid);
                sett.set("guid." + guid, settings);
                // this way, even if we clear the history, and later on we add the same file, we have its settings
                if ( file != "")
                    sett.set("file_to_guid." + file, guid);
            }            
        }

    }
}
