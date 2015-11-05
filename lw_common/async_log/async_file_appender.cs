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
using System.Linq;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Core;
using System.Threading;

namespace lw_common
{
    public class async_file_appender : FileAppender
    {
        private readonly ManualResetEvent close_event_;
        private bool closing_;

        private Queue<LoggingEvent> pending_ = new Queue<LoggingEvent>(32768); 

        // lock(this) does not work here!!! (problem at OnClose())
        object locker_ = new object();

        public async_file_appender()
        {
            close_event_ = new ManualResetEvent(false);
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            new Thread(append_thread) { IsBackground = true }.Start();
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            foreach ( var e in loggingEvents)
                Append(e);
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (FilterEvent(loggingEvent))
            {
                loggingEvent.Fix = FixFlags.ThreadName | FixFlags.LocationInfo;
                lock(locker_)
                    pending_.Enqueue(loggingEvent);
            }
        }

        protected override void OnClose()
        {
            closing_ = true;
            close_event_.WaitOne(2500);

            base.OnClose();
        }

        private void append_thread() {
            while (true) {
                LoggingEvent to_append = null;
                lock(locker_)
                    if (pending_.Count > 0)
                        to_append = pending_.Dequeue();
                    else if (closing_)
                        break;

                if (to_append == null) {
                    Thread.Sleep(10);
                    continue;
                }

                try {
                    base.Append(to_append);
                }
                catch
                {}
            }

            close_event_.Set();
        }

    }
}
