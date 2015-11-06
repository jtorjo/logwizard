using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace lw_common {

    // thread-safe
    public class settings_as_string {
        private Dictionary<string,string> sett_ = new Dictionary<string, string>();

        public settings_as_string(string str) {
            var lines = str.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines) {
                int idx = line.IndexOf("=");
                if (idx >= 0) {
                    string name = line.Substring(0, idx);
                    string value = line.Substring(idx + 1);
                    sett_.Add(name,value);
                }
                else 
                    Debug.Assert(false);
            }
        }

        public string get(string name, string default_ = "") {
            lock(this)
                return sett_.ContainsKey(name) ? sett_[name] : default_;
        }

        public void set(string name, string val) {
            lock(this)
                if (sett_.ContainsKey(name))
                    sett_[name] = val;
                else 
                    sett_.Add(name, val);
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

        // other overrides all we have
        public void merge(settings_as_string other) {
            var other_names = other.names();
            foreach ( string name in other_names)
                set(name, other.get(name));
        }
    }
}
