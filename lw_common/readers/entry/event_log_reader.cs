using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using log4net.Repository.Hierarchy;
using lw_common.parse;
using MultiLanguage;

namespace lw_common {
    /* inspiration

- event log -- https://msdn.microsoft.com/en-us/library/74e2ybbs%28v=vs.110%29.aspx - allow as many as the user wants...

    see about http://sanderstechnology.com/tag/net-framework-4-5/#.Vkvetb9JCHs (Group membership)

            https://msdn.microsoft.com/en-us/library/bb671200(v=vs.90).aspx#Y0


            http://michal.is/blog/query-the-event-log-with-c-net/
            http://stackoverflow.com/questions/8567368/eventlogquery-time-format-expected/8575390#8575390
            http://stackoverflow.com/questions/7966993/eventlogquery-reader-for-remote-computer
            http://stackoverflow.com/questions/12380189/eventlogquery-how-to-form-query-string

            http://codewala.net/2013/10/04/reading-event-logs-efficiently-using-c/
            http://codewala.net/2013/08/16/working-with-eventviewer-using-c/
            https://msdn.microsoft.com/en-us/library/74e2ybbs(v=vs.110).aspx




            http://www.aspheute.com/english/20000811.asp
            http://www.codeproject.com/Articles/14455/Eventlog-Viewer
            http://www.codeproject.com/Articles/91/WindowsNT-Event-Log-Viewer

    */
    public class event_log_reader : entry_text_reader_base {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool fully_read_once_ = false;
        private bool up_to_date_ = false;

        private bool logs_created_ = false;

        private const int MAX_DIFF_NEW_EVENT_MS = 2000;
        private const int MAX_ITEMS_PER_BLOCK = 100;

        private class log_info {
            public bool disposed_ = false;

            public string log_type = "";

            public string remove_machine_name = "";
            public string remove_user_name = "";
            public string remote_domain = "";
            public string remote_password = "";

            public List<EventRecord> last_events_ = new List<EventRecord>();
            public List<EventRecord> new_events_ = new List<EventRecord>();

            // if true, we've read all existing events and now we're listening for new ones
            public bool listening_for_new_events_ = false;
        }


        private List<log_info> event_logs_ = new List<log_info>();

        public event_log_reader() {
            settings.on_changed += (a) => force_reload();
        }

        public string[] log_types {
            get {
                //if (util.is_debug)
                    // for testing - very few entries
                  //  return new [] { "Windows PowerShell" };

                return settings.get("event.log_type", "Application|System").Split('|');
            }
        }

        public string provider_name {
            get { return settings.get("event.provider_name"); }
        } 
        public string remote_machine_name {
            get {
                return settings.get("event.remote_machine_name"); 
            }
        }
        public string remote_user_name {
            get {
                return settings.get("event.remote_user_name", ""); 
            }
        }
        public string remote_domain_name {
            get {
                return settings.get("event.remote_domain_name", ""); 
            }
        }
        public string remote_password_name {
            get {
                return settings.get("event.remote_password_name", ""); 
            }
        }

        public override bool fully_read_once {
            get { return fully_read_once_; }
        }

        public override void force_reload() {
            fully_read_once_ = false;
            logs_created_ = false;
            up_to_date_ = false;
            errors_.clear();
        }

        private void create_logs() {
            if (logs_created_)
                return;

            logs_created_ = true;
            lock (this) {
                foreach (var log in event_logs_)
                    log.disposed_ = true;
                event_logs_.Clear();
            }

            lock(this)
                foreach (var type in log_types) {
                    try {
                        var log = new log_info { log_type = type, remove_machine_name = remote_machine_name, remote_domain = remote_domain_name, remote_password = remote_password_name, remove_user_name = remote_user_name};
                        event_logs_.Add( log);
                        new Thread(() => read_single_log_thread(log)) {IsBackground = true }.Start();
                    } catch (Exception e) {
                        logger.Error("can't create event log " + type + "/" + remote_machine_name + " : " + e.Message);
                        errors_.add("Can't create Log " + type + " on machine " + remote_machine_name + ", Reason=" + e.Message);
                    }
                }
        }

        internal override List<log_entry_line> read_next_lines() {
            create_logs();

            up_to_date_ = true;
            lock (this) 
                fully_read_once_ = event_logs_.Count(x => x.listening_for_new_events_) == event_logs_.Count;

            // http://www.codeproject.com/Messages/5162204/sorting-information-from-several-threads-is-my-alg.aspx
            List<log_entry_line> next = new memory_optimized_list<log_entry_line>();
            lock (this) {
                // special case - a single log
                if (event_logs_.Count == 1) {
                    var events = event_logs_[0].last_events_.Count > 0
                        ? event_logs_[0].last_events_.Select(to_log_entry).ToList() : event_logs_[0].new_events_.Select(to_log_entry).ToList();
                    event_logs_[0].last_events_.Clear();
                    event_logs_[0].new_events_.Clear();
                    return events;
                }
                int listen_for_new_events = event_logs_.Count(x => x.listening_for_new_events_);
                // note: if we're listing for new events, first process all the last ones
                if (listen_for_new_events == event_logs_.Count && event_logs_.Count(x => x.last_events_.Count == 0) == event_logs_.Count) {
                    // we're listening for NEW EVENTS on all threads
                    List<EventRecord> all = new List<EventRecord>();
                    var now = DateTime.Now;
                    foreach ( var log in event_logs_)
                        // we're waiting a bit before returning new events - just in case different threads might come up with earlier entries 
                        // (because we can't really count on any speed at all)
                        all.AddRange( log.new_events_.Where(x => x.TimeCreated.Value.AddMilliseconds(MAX_DIFF_NEW_EVENT_MS) <= now));
                    all = all.OrderBy(x => x.TimeCreated).ToList();
                    foreach ( var log in event_logs_)
                        foreach (var entry in all)
                            log.new_events_.Remove(entry);
                    next = all.Select(to_log_entry).ToList();
                    return next;
                }

                while (true) {
                    // If there is one or more threads that doesn't have at least one element, return an empty list
                    int listen_for_last_events_and_have_no_events = event_logs_.Count(x => !x.listening_for_new_events_ && x.last_events_.Count == 0);
                    if (listen_for_last_events_and_have_no_events > 0)
                        // at least one thread listening for old events and got nothing
                        return next;

                    // ... the last bool -> if true, the item can be added right now ; if false, we can't add this item now
                    List < Tuple<EventRecord,int, bool> > last = new List<Tuple<EventRecord, int, bool>>();
                    for (int log_idx = 0; log_idx < event_logs_.Count; log_idx++) {
                        var log = event_logs_[log_idx];
                        if ( log.last_events_.Count > 0)
                            // if I'm listening for new events, this is ok - return this entry
                            last.Add(new Tuple<EventRecord, int, bool>( log.last_events_[0], log_idx, log.last_events_.Count > 1 || log.listening_for_new_events_));
                        else if (log.new_events_.Count > 0) 
                            last.Add(new Tuple<EventRecord, int, bool>( log.new_events_[0], log_idx, true));
                    }
                    if (last.Count < 1)
                        return next;
                    var min = last.Min(x => x.Item1.TimeCreated);
                    var item = last.Find(x => x.Item1.TimeCreated == min);
                    if (!item.Item3)
                        return next;

                    foreach (var log in event_logs_)
                        if (log.last_events_.Count > 0)
                            log.last_events_.Remove(item.Item1);
                        else
                            log.new_events_.Remove(item.Item1);
                    next.Add( to_log_entry(item.Item1));
                    if (next.Count >= MAX_ITEMS_PER_BLOCK)
                        return next;
                }
            }

            return next;
        }

        private void read_single_log_thread(log_info log) {
            string query_string = "*";
            if (provider_name != "")
                query_string = "*[System/Provider/@Name=\"" + provider_name + "\"]";

            try {
                SecureString pwd = new SecureString();
                foreach ( char c in remote_password_name)
                    pwd.AppendChar(c);
                EventLogSession session = remote_password_name != "" ? new EventLogSession(log.remove_machine_name, remote_domain_name, remote_user_name, pwd, SessionAuthentication.Default) : null;
                pwd.Dispose();

                EventLogQuery query = new EventLogQuery(log.log_type, PathType.LogName, query_string);
                if ( session != null)
                    query.Session = session;

                EventLogReader reader = new EventLogReader(query);

                for (EventRecord rec = reader.ReadEvent(); rec != null && !log.disposed_; rec = reader.ReadEvent()) 
                    lock(this)
                        log.last_events_.Add( rec);

                lock (this)
                    log.listening_for_new_events_ = true;

                // at this point, listen for new events
				using (var watcher = new EventLogWatcher(query))
				{
					watcher.EventRecordWritten += (o, e) => {
                        lock(this)
                            log.new_events_.Add(e.EventRecord);
					};
					watcher.Enabled = true;

                    while ( !log.disposed_)
                        Thread.Sleep(100);
				}

            } catch (Exception e) {
                logger.Error("can't create event log " + log.log_type + "/" + remote_machine_name + " : " + e.Message);
                errors_.add("Can't create Log " + log.log_type + " on machine " + remote_machine_name + ", Reason=" + e.Message);
            }
            
        }

        private log_entry_line to_log_entry(EventRecord rec) {
            log_entry_line entry = new log_entry_line();
            try {
                try {
                    entry.add("Category", rec.TaskDisplayName);
                } catch {
                    entry.add("Category", "");
                }
                try {
                    entry.add("msg", rec.FormatDescription());
                } catch {
                    entry.add("msg", "");
                }
                entry.add("Machine Name", rec.MachineName);
                entry.add("level", event_level((StandardEventLevel) rec.Level));
                entry.add("EventID", "" + rec.Id);
                entry.add("Source", "" + rec.ProviderName);
                entry.add("date", rec.TimeCreated.Value.ToString("DD-MM-YYYY"));
                entry.add("time", rec.TimeCreated.Value.ToString("hh:mm:ss.fff"));
                entry.add("User Name", rec.UserId != null ? rec.UserId.Value : "");
                try {
                    entry.add("Keywords", util.concatenate(rec.KeywordsDisplayNames, ","));
                } catch {
                    entry.add("Keywords", "");
                }
            } catch (Exception e) {
                logger.Fatal("can't convert EventRectord to entry " + e.Message);
            }
            return entry;
        }


        private static string event_level(StandardEventLevel type) {
            switch (type) {
            case StandardEventLevel.LogAlways:
                return "DEBUG";
            case StandardEventLevel.Critical:
                return "INFO";
            case StandardEventLevel.Error:
                return "ERROR";
            case StandardEventLevel.Warning:
                return "WARN";
            case StandardEventLevel.Informational:
                return "INFO";
            case StandardEventLevel.Verbose:
                return "VERB";
            default:
                return "";
            }
        }

        public override bool has_it_been_rewritten {
            get { return false; }
        }

        public override bool is_up_to_date() {
            return fully_read_once_ && up_to_date_;
        }
    }
}
