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

namespace LogWizard {
    /* 1.0.20+ we need a different reader for each log_view. This way, when the file we're monitoring is appended to,
               all the readers get correctly refreshed

       1.0.42 made thread-safe
    */
    class log_line_reader {

        private log_line_parser parser_;
        private int line_count_ = 0;

        public log_line_reader(log_line_parser parser) {
            Debug.Assert(parser != null);
            parser_ = parser;
        }

        public string name {
            get { return parser_.name; }
        }

        public bool forced_reload {
            get { return parser_.forced_reload(this); }
        }

        public int line_count {
            get { lock(this) return line_count_;  }
        }

        public void refresh() {
            int lc = parser_.line_count;
            lock (this) 
                line_count_ = lc;
        }

        public line line_at(int idx) {
            return parser_.line_at(idx);
        }

        public void force_reload() {
            parser_.force_reload();
        }

    }
}
