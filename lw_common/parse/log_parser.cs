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
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using lw_common.parse;
using lw_common.parse.parsers;
using LogWizard;

namespace lw_common
{

    /* reads everything in the log, and allows easy access to its lines
    */
    public class log_parser : IDisposable {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly text_reader reader_ = null;

        private bool disposed_ = false;

        // for each of these readers, we have returned a "yes" to forced_reload
        private readonly HashSet<log_reader> forced_reload_ = new HashSet<log_reader>();

        public delegate void on_new_lines_func();
        public on_new_lines_func on_new_lines;

        private readonly easy_mutex new_lines_event_ = new easy_mutex();

        private readonly log_parser_base forward_to_parser_ = null;

        public log_parser(text_reader reader, syntax_base syntax) {
            Debug.Assert(reader != null);
            reader_ = reader;

            reader.on_new_lines += on_log_has_new_lines;

            if (syntax is line_by_line_syntax) 
                forward_to_parser_ = new log_parser_line_by_line(reader, syntax as line_by_line_syntax);
            else 
                // don't know the real parser!
                Debug.Assert(false);

            force_reload();
            new Thread(refresh_thread) {IsBackground = true}.Start();
        }

        private void on_log_has_new_lines() {
            if (disposed_)
                return;
            new_lines_event_.release_and_reaquire();
        }


        // once we've been forced to reload - we should return true once per each reader
        public bool forced_reload(log_reader reader) {
            lock (this) {
                if (forced_reload_.Contains(reader))
                    return false;
                forced_reload_.Add(reader);
                return true;
            }
        }

        private void refresh_thread() {
            while (!disposed_) {
                bool wait_event = reader_.fully_read_once;
                bool new_lines_found = false;
                if (wait_event) {
                    new_lines_found = new_lines_event_.wait_and_release();
                    if (new_lines_found) 
                        logger.Debug("[log] new lines for " + reader_.name);
                }
                else 
                    Thread.Sleep(app.inst.check_new_lines_interval_ms);

                forward_to_parser_.read_to_end();

                if ( !disposed_&& new_lines_found && on_new_lines != null)
                    on_new_lines();
            }
        }

        public int line_count {
            get { return forward_to_parser_.line_count; }
        }

        public string name {
            get { return reader_.name; }
        }

        public bool up_to_date {
            get { return forward_to_parser_.up_to_date;  }
        }

        public line line_at(int idx) {
            return forward_to_parser_.line_at(idx);
        }

        // forces the WHOLE FILE to be reloaded
        //
        // be VERY careful calling this - I should call this only when the syntax has changed
        private void force_reload() {

            lock (this) {
                forced_reload_.Clear();
                logger.Info("[log] forced reload: " + reader_.name);
            }

            forward_to_parser_.force_reload();
            reader_.force_reload();
        }

        // forces readers to reload
        public void reload() {
            lock (this) {
                forced_reload_.Clear();
            }
        }

        public void Dispose() {
            disposed_ = true;
            forward_to_parser_.Dispose();
        }
    }
}
