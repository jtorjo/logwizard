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
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lw_common;

namespace LogWizard {
    class filter_row : raw_filter_row {
        public filter_row(string text, bool apply_to_existing_lines) : base(text, apply_to_existing_lines) {
        }

        public filter_row(raw_filter_row other) : base(other) {
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // CACHED DATA 
        //
        // what is below, it's cached running the existing filter row on a log

        private HashSet<int> line_matches_ = new HashSet<int>();
        // cached - so that what we computed once, we don't ask again
        private log_line_reader old_line_matches_log_ = null;
        private int old_line_count_ = 0;

        public HashSet<int> line_matches {
            get { return line_matches_; }
        }

        public void refresh() {
            lock (this) {
                line_matches_.Clear();
                old_line_count_ = 0;                
            }
        }

        // computes the line matches - does not care about colors or the additions - just to know which lines actually match
        public void compute_line_matches(log_line_reader log) {
            log.refresh();
            if (old_line_matches_log_ != log) {
                old_line_matches_log_ = log;
                line_matches_.Clear();
                old_line_count_ = 0;
            }

            // note: in order to match, all lines must match
            int new_line_count = log.line_count;
            for (int i = old_line_count_; i < new_line_count; ++i) {
                bool matches = true;
                foreach (filter_line fi in items_)
                    if ( fi.part != filter_line.part_type.font)
                        if ( !fi.matches(log.line_at(i))) {
                            matches = false;
                            break;
                        }
                if ( matches)
                    line_matches_.Add(i);
            }
            // if we have at least one line - we'll recheck this last line next time - just in case we did not fully read it last time
            old_line_count_ = new_line_count > 0 ? new_line_count -1 : new_line_count;
        }

        // returns exactly how we match this line
        public match get_match(int line_idx) {
            Debug.Assert(line_matches_.Contains(line_idx));
            Debug.Assert(old_line_matches_log_ != null);

            return new match { font = font_ };
        }


    }
}
