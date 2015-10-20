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
using System.Threading;

namespace lw_common {
    class easy_mutex {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Mutex mutex_ = null;

        private int wait_ms = app.inst.check_new_lines_interval_ms;

        public string friendly_name;

        // it defers creation - since maybe another thread might be the "owner"
        public easy_mutex(string name) {
            friendly_name = name;
        }

        // marks this thread as owner
        public void current_thread_is_owner() {
            Debug.Assert(mutex_ == null);
            release_and_reaquire();
        }

        // returns true on success
        public bool release_and_reaquire() {
            lock(this)
                if ( mutex_ == null)
                    mutex_ = new Mutex(true);
            // this will wake up the refresh thread
            mutex_.ReleaseMutex();
            // now, reaquire - should be instant
            int timeout = 1000;
            bool received = mutex_.WaitOne(timeout);

            if ( !received)
                logger.Fatal("[log] " + friendly_name + " - could not reaquire lock ");
            return received;
        }

        // returns true if event was received
        public bool wait_and_release() {
            bool wait_event = mutex_ != null;
            bool new_lines = false;
            if (wait_event) {
                new_lines = mutex_.WaitOne(wait_ms);
                if (new_lines) {
                    mutex_.ReleaseMutex();
                    return true;
                }
            }
            else 
                Thread.Sleep(wait_ms);

            return false;
        }
    }
}
