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


        If nothing is set, everything matches to ctx1, ctx2, ... and the last one matches to msg.
    */
    public class aliases : IDisposable {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // each line is like "x=y"
        private settings_as_string sett_;
        private string separator_ = "|@#@|";

        // these are the names that have not been assigned to aliases
        //
        // if I have something like "target_url=ctx2", then ctx2 becomes used (so that I don't map two log column names for the same logwizard column)
        private List<info_type> all_columns_ = new[] {
            info_type.ctx1,
            info_type.ctx2, info_type.ctx3, info_type.ctx4,
            info_type.ctx5, info_type.ctx6, info_type.ctx7, info_type.ctx8, info_type.ctx9, info_type.ctx10, info_type.ctx11, info_type.ctx12, info_type.ctx13,
            info_type.ctx14, info_type.ctx15, 

            info_type.date, info_type.time, info_type.level, info_type.thread,
            info_type.file,
            info_type.func, info_type.class_, 
            
            info_type.msg,

        }.ToList();

        private double_dictionary<string, info_type> name_to_column_ = new double_dictionary<string, info_type>();

        public util.void_func on_column_names_changed;

        public aliases(string aliases_string) {
            sett_ = new settings_as_string(aliases_string.Replace(separator_, "\r\n"));
            init();
        }

        private aliases() {
            
        }

        private void init() {
            foreach (string name in sett_.names()) {
                var value_str = get_value_part( sett_.get(name));
                var used_value = string_to_info_type(value_str);
                if (used_value != info_type.max)
                    name_to_column_.set( name, used_value);
            }            
        }

        // the idea is to be able to match each column to an info-type 
        public void on_column_names(List<string> column_names) {
            Debug.Assert(on_column_names_changed != null);

            name_to_column_.clear();
            foreach (string col in column_names) {
                info_type type;
                if (Enum.TryParse(col, true, out type)) {
                    // in this case, it's the column itself
                    // we want to know this, so that has_column(this-type) will return true
                    name_to_column_.set(col, type);
                    continue;
                }
                to_info_type(col, column_names);
            }

            if ( on_column_names_changed != null)
                on_column_names_changed();
        }

        public string get(string column) {
            return sett_.get(column, column);
        }

        public override string ToString() {
            return sett_.ToString().Replace("\r\n", separator_);
        }

        public void Dispose() {
            on_column_names_changed = null;
        }

        public string to_enter_separated_string() {
            return sett_.ToString();
        }

        public static aliases from_enter_separated_string(string s) {
            var result = new aliases() { sett_ = new settings_as_string(s) };
            result.init();
            // this will force matching all log columns to our Logwizard columns
            var names = result.sett_.names().ToList();
            foreach (var name in names)
                result.to_info_type(name, names);
            return result;
        }


        private List<string> sorted_non_friendly_name_lines() {
            List<string> non_friendly = new List<string>();
            foreach (string name in sett_.names()) {
                if (is_known_column_name(name))
                    // it's just setting a title for a known column 
                    continue;
                string value = get_value_part( sett_.get(name));
                if ( value != "")
                    non_friendly.Add(name + "=" + value);
            }
            non_friendly.Sort();
            return non_friendly;
        } 

        // if the non-friendly info changes, this requires a re-parse of the whole log
        // for now (1.4.8), just restart LogWizard
        public bool is_non_friendly_name_info_the_same(aliases other) {
            return sorted_non_friendly_name_lines().SequenceEqual(other.sorted_non_friendly_name_lines());
        }


        public string friendly_name(info_type info) {
            string as_string = info.ToString();
            var found = sett_.get(as_string);

            if (found == as_string)
                return info_type_io.to_friendly_str(info);

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

            if (name_to_column_.has_value(info)) {
                string name = name_to_column_.value_to_key(info);
                found = sett_.get(name);
                if (found != "") {
                    // hand=ctx1{Some Awesome Title}
                    // ctx1=Some Awesome Title
                    if ( found != get_friendly_name_part(found))
                        return get_friendly_name_part(found);
                }
                if ( name == as_string)
                    return info_type_io.to_friendly_str(info);
                return name;
            }

            return info_type_io.to_friendly_str(info);
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

        // If nothing is set, everything matches to ctx1, ctx2, ... and the last one matches to msg.
        private info_type first_non_used_column(string name) {
            foreach ( info_type col in all_columns_)
                if (!name_to_column_.has_value(col)) {
                    name_to_column_.set(name, col);
                    return col;
                }

            return info_type.max;
        }

        public bool is_known_column_name(string colum_name) {
            return string_to_info_type(colum_name) != info_type.max;
        }

        // converts a "log" column name into what we use internally ("time", "file", etc)
        public string to_logwizard_column_name(string name) {
            return get_value_part(sett_.get(name, name));
        }
        // converts a "log" column name into what we use internally - info_type.time, info_type.file, etc.
        public info_type to_info_type(string column_name) {
            if (name_to_column_.has_key(column_name))
                return name_to_column_.key_to_value(column_name);

            return string_to_info_type( get_value_part(sett_.get(column_name, column_name)));
        }

        public bool has_column(info_type type) {
            return name_to_column_.has_value(type);
        }

        public string dump_resolve_names() {
            return util.concatenate(sett_.names().Where(n => to_logwizard_column_name(n) != n).Select(n => n + "=" + to_logwizard_column_name(n)) , ", ");
        }

        private static info_type string_to_info_type(string str) {
            return info_type_io.from_str(str);
        }

        // if it returns max, the alias was invalid
        private info_type to_info_type(string alias, List<string> column_names ) {
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

                if (name_to_column_.has_key(alias))
                    return name_to_column_.key_to_value(alias);

                // if we get here it's another alias - look into existing names
                if (column_names.Count > 0 && alias == column_names.Last() && !name_to_column_.has_value(info_type.msg))
                    // by default, the last name is the msg
                    return info_type.msg;

                int idx = column_names.IndexOf(alias);
                if (idx >= 0)
                    return first_non_used_column(alias);
            } catch (Exception e) {
                logger.Info("invalid alias " + alias + " : " + e.Message);
            }
            return info_type.max;
        }
    }
}
