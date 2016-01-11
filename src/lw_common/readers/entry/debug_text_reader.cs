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
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using lw_common.parse;
using lw_common.readers.entry;

namespace lw_common
{
    public class debug_text_reader : entry_text_reader_base {
        private int last_event_id_ = -1;

        public debug_text_reader(log_settings_string sett) : base(sett) {
            settings.on_changed += (a) => force_reload();
        }

        // Global Win32 Messages vs Win32 Messages
        private bool use_global {
            get { return settings.debug_global; }
        }

        private string lo_process_name {
            get { return settings.debug_process_name.get().ToLower(); }
        }

        public override bool fully_read_once {
            // ... if we read at least one event
            get { return last_event_id_ >= 0; }
        }

        public override string friendly_name {
            get { return lo_process_name != "" ? "[" + settings.debug_process_name + "]" : "From All Processes" ; }
        }


        public override void force_reload() {
            lock (this) {
                last_event_id_ = -1;
            }
            errors_.clear();
        }

        public override bool is_up_to_date() {
            return true;
        }

        protected override List<log_entry_line> read_next_lines() {
            var reader = use_global ? capture_all_debug_events.capture_global : capture_all_debug_events.capture_local;
            var last = reader.get_events(last_event_id_ + 1);
            if (last.Count > 0) {
                string lo_process_name = this.lo_process_name;
                last_event_id_ = last.Last().unique_id;
                return last.Where(x => x.lo_process_name == lo_process_name || lo_process_name == "").Select(debug_entry).ToList();
            }
            return new List<log_entry_line>();
        }

        private log_entry_line debug_entry(capture_all_debug_events.debug_event evt) {
            log_entry_line entry = new log_entry_line();
            entry.add("date", evt.date.ToString("dd-MM-yyyy"));
            entry.add("time", evt.date.ToString("HH:mm:ss.fff"));
            entry.add("Pid", "" + evt.process_id);
            entry.add("Process Name", evt.lo_process_name);
            entry.add("msg", evt.msg);
            return entry;
        }
    }
}
