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
using System.Globalization;
using System.Linq;
using System.Text;

namespace lw_common.parse {
    // represents a "conceptual" line - contains a log entry, but it could be on several lines
    // however, we will compact it on a single line
    public class log_entry_line {
        private string entry_ = null;
        private readonly Dictionary<string ,string> infos_ = new Dictionary<string, string>();
        // .. so i know the order they were added
        private readonly List<string> names_ = new List<string>(); 

        private Dictionary<string, int> indexes_ = null; 

        private DateTime time_ = DateTime.MinValue;

        // performs an analysis of the text, and then adds it. looks for common patterns, such as whether it's a timestamp or not
        //
        // note: this can end up adding multiple parts
        public void analyze_and_add(string name, string value) {
            bool is_timestamp = name.Contains("timestamp");
            if (!is_timestamp)
                // check for possible timestamp entries
                is_timestamp = util.is_timestamp_fast(value);

            if ( is_timestamp)
                add_time(value);
            else 
                add(name, value);
        }

        public void add_time(DateTime value) {
            time_ = value;
        }

        public void add_time(string text) {
            time_ = util.str_to_normalized_datetime(text);
        }

        public void add(string name, string value) {
            Debug.Assert(value != null);

            switch (name.ToLower()) {
            case "message" :
                name = "msg";
                break;
            }

            if (!infos_.ContainsKey(name)) {
                infos_.Add(name, value);
                names_.Add(name);
            } else
                // append to the existing entry
                infos_[name] += "\r\n" + value;
        }

        private void compute_entry_and_indexes() {
            lock(this)
                if (entry_ != null && indexes_ != null)
                    return;
            
            Dictionary<string, int> indexes = new Dictionary<string, int>(); 
            string entry = "";
            int cur_idx = 0;
            foreach (var name in names_) {
                indexes.Add(name, cur_idx);
                entry += infos_[name];
                cur_idx += infos_[name].Length;
            }
            lock (this) {
                entry_ = entry;
                indexes_ = indexes;
            }
        }

        public override string ToString() {
            compute_entry_and_indexes();
            return entry_;
        }

        public int entry_count {
            get { return names_.Count; }
        }

        public List<string> names {
            get { return names_; }
        }

        public DateTime time {
            get { return time_; }
        }

        public Tuple<int, int>[] idx_in_line(aliases aliases) {
            compute_entry_and_indexes();

            var idx = new Tuple<int, int>[(int) info_type.max];
            for (int i = 0; i < idx.Length; ++i)
                idx[i] = new Tuple<int, int>(-1, -1);

            var sorted = indexes_.OrderBy(x => x.Value).Select(x => new Tuple<int, int>((int) aliases.to_info_type(x.Key) , x.Value)).ToList();

            for (int i = 0; i < sorted.Count; ++i) {
                int len = i < sorted.Count - 1 ? sorted[i + 1].Item2 - sorted[i].Item2 : -1;
                int cur_idx = sorted[i].Item1;
                // we can have too many columns - we will ignore the last ones
                if ( cur_idx < idx.Length)
                    idx[sorted[i].Item1] = new Tuple<int, int>(sorted[i].Item2, len);
            }

            return idx;
        }

    }
}
