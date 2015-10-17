using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace lw_common {
    class easy_mutex {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Mutex mutex_ = null;

        private int wait_ms = app.inst.check_new_lines_interval_ms;

        // note: eventually if issues found, I will have to have add a friendly name property

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
                logger.Fatal("[log] on new lines - could not reaquire lock ");
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
