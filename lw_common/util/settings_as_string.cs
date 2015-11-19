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
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace lw_common {

    // thread-safe
    public class settings_as_string_readonly {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected Dictionary<string,string> sett_ = new Dictionary<string, string>();

        public delegate void on_changed_func(string name);
        // 1.5.6+ get notified of changes
        public on_changed_func on_changed;

        public settings_as_string_readonly(string str) {
            var lines = str.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines) {
                if (line.Trim() == "")
                    continue;

                int idx = line.IndexOf("=");
                if (idx >= 0) {
                    string name = line.Substring(0, idx);
                    string value = line.Substring(idx + 1);
                    sett_.Add(name, value);
                } else {
                    logger.Warn("invalid settings_as_string_readonly line, ignoring " + line);
                }
            }
        }

        public string get(string name, string default_ = "") {
            lock(this)
                return sett_.ContainsKey(name) ? sett_[name] : default_;
        }

        public string[] names() {
            lock (this)
                return sett_.Keys.ToArray();
        }

        public override string ToString() {
            string str = "";
            lock(this)
                foreach (var kv in sett_) {
                    if (str != "")
                        str += "\r\n";
                    str += kv.Key + "=" + kv.Value;
                }
            return str;
        }

        public settings_as_string sub(string[] names) {
            var names_to_return = this.names();
            names_to_return = names_to_return.Where(names.Contains).ToArray();
            settings_as_string other = new settings_as_string("");
            lock (this)
                other.sett_ = sett_.Where(x => names_to_return.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            return other;
        }

        // 'other' overrides all we have
        public settings_as_string merge_copy(settings_as_string_readonly other) {
            settings_as_string merged = new settings_as_string(ToString());

            var other_names = other.names();
            foreach ( string name in other_names)
                merged.set(name, other.get(name));

            return merged;
        }
    }

    // thread-safe
    public class settings_as_string : settings_as_string_readonly {
        public settings_as_string(string str) : base(str) {
        }

        public void set(string name, string val) {
            lock (this) {
                bool exists = sett_.ContainsKey(name);
                if (exists && sett_[name] == val)
                    return; // nothing changed
                if (!exists && val == "")
                    return; // nothing changed

                if (exists)
                    sett_[name] = val;
                else
                    sett_.Add(name, val);

                if (val == "")
                    sett_.Remove(name);
            }
            if (on_changed != null)
                on_changed(name);
        }

        public void merge(string other) {
            settings_as_string_readonly other_sett = new settings_as_string_readonly(other);
            // note: i set it like this, so that in case of any change, I call the on_change delegate
            foreach ( var name in other_sett.names())
                set( name, other_sett.get(name));
        }

    }
}
