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
using log4net.Repository.Hierarchy;
using lw_common.parse;

namespace lw_common {
    /* 1.0.20+ we need a different reader for each log_view. This way, when the file we're monitoring is appended to,
               all the readers get correctly refreshed

       1.0.42 made thread-safe
    */

    public class log_reader : IDisposable {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private log_parser parser_;
        private int line_count_ = 0;

        private bool disposed_ = false;
        // for testing/debugging
        private string tab_name_ = "";

        public delegate void on_new_lines_func(bool file_rewritten);

        public on_new_lines_func on_new_lines;

        public log_reader(log_parser parser) {
            Debug.Assert(parser != null);
            parser_ = parser;
            parser_.on_new_lines += on_parser_new_lines;
        }

        private void on_parser_new_lines(bool file_rewritten) {
            Debug.Assert(!disposed_);
            if (disposed_)
                return;

            logger.Debug("[parse] new lines on tab " + tab_name + " / rewritten=" + file_rewritten);
            if (on_new_lines != null)
                on_new_lines(file_rewritten);
        }

        public string tab_name {
            get { return tab_name_; }
            set { tab_name_ = value; }
        }

        public string log_name {
            get { return parser_.name; }
        }

        public override string ToString() {
            return tab_name + " / " + log_name;
        }

        public bool forced_reload {
            get { return parser_.forced_reload(this); }
        }

        public aliases aliases {
            get { return parser_.aliases; }
        }

        public bool up_to_date {
            get {
                if (!parser_.up_to_date)
                    return false;
                int parser_lines = parser_.line_count;
                lock(this)
                return line_count_ == parser_lines;
            }
        }

        public bool parser_up_to_date {
            get { return parser_.up_to_date; }
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

        public void Dispose() {
            if ( parser_.on_new_lines != null)
                parser_.on_new_lines -= on_parser_new_lines;
            disposed_ = true;
        }
    }
}
