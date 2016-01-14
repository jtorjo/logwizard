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
using System.Linq;
using System.Text;

namespace lw_common.parse.parsers.system {
    class event_viewer : generic_entry_log_parser {

        public event_viewer(event_log_reader reader) : base(reader) {
        }

        // description is multi-line
        public override bool has_multi_line_columns {
            get { return true; }
        }

        public override void read_to_end() {
            var entries_now = reader_.read_available_lines();
            if (entries_now == null)
                return;
            lock (this) {
                foreach ( var entry in entries_now)
                    string_.add_preparsed_line(entry.ToString());
                entries_.AddRange(entries_now);

                if (column_names.Count < 1 && entries_now.Count > 0)
                    column_names = entries_now[0].names;
            }
        }


        // line at 0        - is the oldest
        // line at Last     - is the newest
        public override line line_at(int idx) {
            lock (this) {
                if (idx < entries_.Count) {
                    // the only reason I have this is so that I append things correctly to the large-string, since 
                    // we're adding always prepending lines when we're in reverse order
                    //
                    // thus, the index in the large string should be valid all the time (even after new lines are added)
                    int old_entries = (reader_ as event_log_reader).old_event_count;
                    if (idx < old_entries || old_entries < 0) {
                        if (reader_.are_elements_in_reverse_order) {
                            if (old_entries < 0)
                                old_entries = entries_.Count;
                            idx = old_entries - idx - 1;
                        }
                        var entry = entries_[idx];
                        var l = new line(new sub_string(string_, idx), entry.idx_in_line(aliases), entry.time);
                        return l;
                    } else {
                        // it's new entries
                        var entry = entries_[idx];
                        var l = new line(new sub_string(string_, idx), entry.idx_in_line(aliases), entry.time);
                        return l;
                    }
                } 

                // this can happen, when the log has been re-written, and everything is being refreshed
                throw new line.exception("invalid line request " + idx + " / " + entries_.Count);
            }
        }
    }
}
