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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace lw_common
{
    /** similar to Java properties files 
     */
    public class settings_file
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // 1.0.15+ - make it easiser to look at settings
        //private Dictionary<string, string> data = new Dictionary<string, string>();
        private SortedDictionary<string, string> data = new SortedDictionary<string, string>();

        public readonly string file_name;
        public bool dirty = false;

        // if true, we log each time we modify something - for debugging only
        public bool log_each_set = true;

        private const string VERSION_NAME = "_settings_version";
        /* 1.0.66+ added versioning

        1 
        - old version (1.0.65 or less)

        2 
        - multiple lines are stored as multiple lines with prepending \t
        - lines that contain prepending or appending spaces -> surround with quotes

        */
        private int version_ = 1;
        private const int max_version_ = 2;

        public settings_file(string file_name) {
            this.file_name = file_name;
            if ( !File.Exists(file_name))
                // not found, that is fine - we can have a settings file that we're currently creating
                return; 
            try {
                string last_name = "";
                foreach (var row in File.ReadAllLines(file_name)) {
                    string line = row;
                    if ( line.Length > 1)
                        if ( line[0] == '\t') {
                            // special case - line that starts with '\t'
                            // it will be appended to the previous name
                            // so that we can have a property contain several concatenated "lines"
                            line = line.Trim();
                            if ( line.Length >= 2)
                                if ( line.StartsWith("\"") && line.EndsWith("\"")) 
                                    line = line.Substring(1, line.Length - 2);
                            data[last_name] += "\r\n" + line;
                            continue;
                        }
                    line = line.Trim();
                    if ( line.Length < 1)
                        continue;
                    if ( line[0] == '#')
                        continue; // comment
                    if ( !line.Contains('=')) {
                        logger.Error("invalid line " + line + " on " + file_name);
                        continue;
                    }

                    int pos = line.IndexOf('=');
                    string name = line.Substring(0,pos).Trim();
                    string value = line.Substring(pos+1).Trim();
                    // 1.0.65 - versioning
                    if (name == VERSION_NAME) {
                        version_ = int.Parse(value);
                        continue;
                    }

                    // any special value (containing starting or ending spaces), surround it with quotes
                    if ( value.Length > 2)
                        if ( value[0] == '"' && value[value.Length-1] == '"') 
                            value = value.Substring(1, value.Length-2);

                    // boolean -> integer
                    if ( value.ToLower() == "false")
                        value = "0";
                    else if ( value.ToLower() == "true")
                        value = "1";

                    if (version_ < 2) {
                        // old versions
                        value = value.Replace("\\n", "\n");
                        value = value.Replace("\\r", "\r");
                    }

                    if ( !data.ContainsKey(name))
                        data.Add(name,"");
                    data[name] = value;
                    last_name = name;
                }
            } catch(Exception e) {
                logger.Error("Could not read " + file_name + " : " + e.Message);
            }
        }

        public string get(string text, string default_ = "") {
            lock(this)
                if ( data.ContainsKey(text))
                    return data[text];
                else
                    return default_;
        }

        public void set(string text, string val) {
            lock(this)
                if ( !data.ContainsKey(text))
                    data.Add( text, "");

            lock(this)
                if ( data[text] != val) {
                    data[text] = val;
                    dirty = true;
                    if ( log_each_set)
                        logger.Debug("[sett] " + text + "=" + val);
                }
        }

        private static string surround_with_quotes_if_needed(string line) {
            return line != line.Trim() ? '"' + line + '"' : line;
        }

        // converts the string to be dumped to the settings file
        private string str_to_value(string str) {
            int count_n = str.Count(c => c == '\n'), count_r = str.Count(c => c == '\r');
            // invalid string
            if (count_n != count_r)
                logger.Error("invalid setings line [" + str + "]");

            if (count_n > 0) {
                // the string contains multiple lines
                // ... account for when there are invalid enters - we care only about \r
                str = str.Replace("\n", "");
                string[] lines = str.Split(new string[] {"\r"}, StringSplitOptions.None);
                for (int i = 0; i < lines.Length; ++i)
                    if (i > 0)
                        str += "\r\n\t" + surround_with_quotes_if_needed(lines[i]) ;
                    else
                        str = surround_with_quotes_if_needed( lines[i]);
            } else
                str = surround_with_quotes_if_needed(str);

            return str;
        }


        public void save() {
            string contents = "";
            lock(this) {
                if ( !dirty)
                    return;
                dirty = false;
                contents += VERSION_NAME + "=" + max_version_ + "\r\n";

                foreach( KeyValuePair<string,string> kv in data) {
                    string value = str_to_value( kv.Value);
                    contents += kv.Key + "=" + value + "\r\n";
                }
            }
            try {
                File.WriteAllText(file_name, contents);
            } catch (Exception e) {
                logger.Error("Could not write " + file_name + " : " + e.Message);
                lock (this)
                    dirty = true;
            }
            logger.Debug("[sett] Saved settings to " + file_name);
        }
    }
}
