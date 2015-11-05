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

    // this can only be used from two threads :
    // 1. owner         - the one owning and "signaling" the event
    // 2. listener      - the one waiting constantly for the event
    class easy_mutex {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Mutex mutex_ = null;

        private int wait_ms = app.inst.check_new_lines_interval_ms;

        private bool abandoned_ = false;

        private bool signaled_ = false;

        public string friendly_name;

        private int owner_thread_id_ = 0;
        private int listener_thread_id_ = 0;

        // it defers creation - since maybe another thread might be the "owner"
        public easy_mutex(string name) {
            friendly_name = name;
        }

        // marks this thread as owner
        public void current_thread_is_owner() {
            Debug.Assert(mutex_ == null);            
            signal();
        }

        // returns true on success
        public void signal() {
            if (abandoned_)
                return ;
            
            Debug.Assert(owner_thread_id_ == 0 || owner_thread_id_ == Thread.CurrentThread.ManagedThreadId);
            lock (this)
                signaled_ = true;

            try {
                lock (this)
                    if (mutex_ == null) {
                        mutex_ = new Mutex(true);
                        owner_thread_id_ = Thread.CurrentThread.ManagedThreadId;
                    }
                // this will wake up the refresh thread
                mutex_.ReleaseMutex();
                // now, reaquire - should be instant
                int timeout = 1000;
                bool received = mutex_.WaitOne(timeout);

                if (!received)
                    logger.Fatal("[log] " + friendly_name + " - could not reaquire lock ");
                return ;
            } catch (AbandonedMutexException ame) {
                // this should never happen - we are the acquire thread!!!
                logger.Fatal("[log] " + friendly_name + " - abandoned on releaseandacquire " + ame);
                abandoned_ = true;
            }
            catch(Exception e) {
                logger.Fatal("[log] " + friendly_name + " - exception on easy mutex " + e);
                abandoned_ = true;
            }
            return ;
        }

        // returns true if event was received
        public bool wait() {
            if (util.is_debug && listener_thread_id_ == 0)
                listener_thread_id_ = Thread.CurrentThread.ManagedThreadId;
            Debug.Assert(listener_thread_id_ == Thread.CurrentThread.ManagedThreadId);

            if (abandoned_) {
                Thread.Sleep(wait_ms);
                return false;
            }

            lock (this) 
                if ( signaled_) {
                    // in this case it got signaled while we were working
                    signaled_ = false;
                    return true;
                }

            try {
                bool wait_event = mutex_ != null;
                bool new_lines = false;
                if (wait_event) {
                    new_lines = mutex_.WaitOne(wait_ms);
                    if (new_lines) {
                        lock (this)
                            signaled_ = false;
                        mutex_.ReleaseMutex();
                        return true;
                    }
                } else
                    Thread.Sleep(wait_ms);

                return false;
            } catch (AbandonedMutexException ame) {
                if ( abandoned_)
                    logger.Fatal("[log] " + friendly_name + " - exception on easy mutex " + ame);
                abandoned_ = true;
            }
            catch(Exception e) {
                logger.Fatal("[log] " + friendly_name + " - exception on easy mutex " + e);
                abandoned_ = true;
            }
            return false;
        }
    }
}
