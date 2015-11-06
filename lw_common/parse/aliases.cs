using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common.parse {
    /* 
        Examples:

             convert index to info-type
            _0 = file
            _1 = thread


            convert index to info type + friendly name
            _2 = ctx1{Static IP}
                
            convert name from log into info-type
            source_ip=ctx1

            convert name from log into info-type + friendly name
            source_ip=ctx1{Source IP}

            info type to friendly string
            ctx1=Country
            ctx2=City
    */
    public class aliases {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // each line is like "x=y"
        private settings_as_string sett_;
        private string separator_ = "|@#@|";

        // if true, something that is non-friendly-name has changed - (an alias -> info-type)
        // thus, if true, this means we need to rebuild the whole log
        private bool has_non_friendly_name_info_changed_ = false;

        public aliases(string aliases_string) {
            sett_ = new settings_as_string(aliases_string.Replace(separator_, "\r\n"));
        }

        public void set(string column, string alias) {
            if (sett_.get(column) == alias)
                return; // nothing changed

            sett_.set(column, alias);
            // FIXME see if alias is xx{yy}, thus, see if only {yy} has changed, or it's index-to-info-type
            // thus, need to update has_non_friendly_name_info_changed
        }

        public string get(string column) {
            string alias = sett_.get(column);
            return alias == "" ? column : alias;
        }

        public override string ToString() {
            return sett_.ToString().Replace("\r\n", separator_);
        }

        public string friendly_name(info_type info, List<string> names) {
            string as_string = info.ToString();
            var found = sett_.get(as_string);
            if (found != "")
                // "ctx3 = City"
                return found.Trim();

            var found_as_idx = sett_.get("_" + (int) info);
            if (found_as_idx != "") {
                // we have a "_index = value" entry, lets parse it
                // the 'value' syntax is "name[{Friendly Name}]"
                if (found_as_idx.IndexOf("{") >= 0) 
                    return get_friendly_name_part(found_as_idx);
            }

            return as_string;
        }

        // parses "value{Friendly Name}" strings
        private static string get_value_part(string s) {
            int friendly_idx = s.IndexOf("{");
            if (friendly_idx >= 0)
                return s.Substring(0, friendly_idx).Trim();

            return s.Trim();
        }
        // parses "value{Friendly Name}" strings
        private static string get_friendly_name_part(string s) {
            int friendly_idx = s.IndexOf("{");
            if (friendly_idx >= 0) {
                s = s.Substring(friendly_idx + 1, s.Length - friendly_idx - 2);
                if (s != "")
                    return s;
            }

            return s;
        }

        private info_type index_to_info_type(int idx) {
            string alias = sett_.get("_" + idx);
            if (alias != "") {
                // we have a "_index = value" entry, lets parse it
                // the 'value' syntax is "name[{Friendly Name}]"
                alias = get_value_part(alias);
                var found = string_to_info_type(alias);
                if (found != info_type.max)
                    return found;
            }

            // just convert it to info-type
            if (idx >= 0 && idx < (int) info_type.max)
                return (info_type) idx;

            return info_type.max;
        }

        private info_type string_to_info_type(string str) {
            switch (str) {
            case "time" :   return info_type.time;
            case "date":    return info_type.date;
            case "level":   return info_type.level;
            case "thread":  return info_type.thread;
            case "file":    return info_type.file;
            case "func":    return info_type.func;
            case "class":   return info_type.class_;
            case "msg":     return info_type.msg;

            case "ctx1":    return info_type.ctx1;
            case "ctx2":    return info_type.ctx2;
            case "ctx3":    return info_type.ctx3;
            case "ctx4":    return info_type.ctx4;
            case "ctx5":    return info_type.ctx5;
            case "ctx6":    return info_type.ctx6;
            case "ctx7":    return info_type.ctx7;
            case "ctx8":    return info_type.ctx8;
            case "ctx9":    return info_type.ctx9;
            case "ctx10":    return info_type.ctx10;

            case "ctx11":    return info_type.ctx11;
            case "ctx12":    return info_type.ctx12;
            case "ctx13":    return info_type.ctx13;
            case "ctx14":    return info_type.ctx14;
            case "ctx15":    return info_type.ctx15;
            }
            // unknown
            return info_type.max;
        }

        // if it returns max, the alias was invalid
        public info_type to_info_type(string alias, List<string> names ) {
            try {
                // it's an index, like, _7
                if (alias.StartsWith("_")) {
                    // Example: _0 = file{File Name}
                    int pos = int.Parse(alias.Substring(1));
                    return index_to_info_type(pos);
                }

                var found = string_to_info_type(alias);
                if (found != info_type.max)
                    return found;

                string to_info_str = sett_.get(alias);
                if (to_info_str != "") {
                    // Example: source_ip=ctx1{Source IP}
                    var value = get_value_part(to_info_str);
                    var to_info = string_to_info_type(value);
                    if (to_info != info_type.max)
                        return to_info;
                }

                // if we get here it's another alias - look into existing names
                int idx = names.IndexOf(alias);
                if (idx >= 0)
                    // Example: _0 = file{File Name}
                    return index_to_info_type(idx);
            } catch (Exception e) {
                logger.Info("invalid alias " + alias + " : " + e.Message);
            }
            return info_type.max;
        }
    }
}
