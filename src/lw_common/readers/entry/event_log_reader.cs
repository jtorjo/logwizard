using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private bool logs_created_ = false;

        private const int MAX_DIFF_NEW_EVENT_MS = 2000;
        private readonly int MAX_ITEMS_PER_BLOCK = util.is_debug ? 200 : 5000;

        private class log_info {
            public bool disposed_ = false;

            public string log_type = "";

            public string remote_machine_name = "";
            public string remote_user_name = "";
            public string remote_domain = "";

            public List<EventRecord> last_events_ = new List<EventRecord>();
            public List<EventRecord> new_events_ = new List<EventRecord>();

            // if true, we've read all existing events and now we're listening for new ones
            public bool listening_for_new_events_ = false;

            // if >= 0, it's the number of total entries (we may not be able to compute this)
            public int full_log_count_ = -1;

            // how many log entries we've read so far
            public int cur_log_count_ = 0;
        }

        // IMPORTANT - we never add or remove from event_logs_ - whenever something gets added/removed, we create a new list
        //             (so that we don't have to lock() access to it
        private List<log_info> event_logs_ = new List<log_info>();

        private string remote_password_ = "";

        // events received through "last_events" ; if < 0, not known yet
        private int old_event_count_ = -1;

        private bool has_done_gc_collect_ = false;

        private int event_count_so_far_ = 0;

        public event_log_reader(log_settings_string sett) : base(sett) {
            settings.on_changed += on_settings_changed;
            sett.name. set(friendly_name);
        }

        public override bool are_elements_in_reverse_order {
            get { return show_elements_in_reverse_order; }
        }

        public string[] log_types {
            get {
                return settings.event_log_type.get().Split('|');
            }
        }

        public string provider_name {
            get { return settings.event_provider_name; }
        } 
        public string remote_machine_name {
            get {
                return settings.event_remote_machine_name.get().Trim(); 
            }
        }
        public string remote_user_name {
            get {
                return settings.event_remote_user_name; 
            }
        }
        public string remote_domain_name {
            get {
                return settings.event_remote_domain; 
            }
        }

        public override string friendly_name {
            get {
                string prefix = (remote_machine_name != "" ? "" : "Local ") + "Event Log(s) " + (remote_machine_name != "" ? " on machine [" + remote_machine_name + "]" : "") + ": ";
                return prefix + util.concatenate(log_types, ", ")  ;
            }
        }

        public override string progress {
            get {
                List<string> so_far = new List<string>();
                lock (this) {
                    foreach (var log in event_logs_) {
                        string name = log.log_type;
                        int len = log.full_log_count_ > 0 ? log.full_log_count_ : log.listening_for_new_events_ ? log.cur_log_count_ : -1;
                        int cur = log.cur_log_count_;
                        int processed = cur - log.last_events_.Count;
                        string percent = len > 0 ? string.Format(" ({0:0}%)", processed * 100 / len) : "";
                        string now = "<b>" + name + percent + "</b>" + " (" + processed + ", read = " + cur + (len >= 0 ? " of = " + len : "") + ")";
                        so_far.Add(now);
                    }
                }
                return "Reading " + util.concatenate(so_far, "; ");
            }
        }

        private void on_settings_changed(string name) {
            if (name == settings.event_remote_password) {
                if (settings.event_remote_password != "") {
                    remote_password_ = settings.event_remote_password;
                    write_settings.event_remote_password.set("");
                }
                return;
            }

            var needs_reload = new [] {
                settings.reverse.name,
                settings.event_log_type.name, settings.event_provider_name.name, 
                settings.event_remote_domain.name, settings.event_remote_machine_name.name, settings.event_remote_user_name.name,            
            };
            if ( needs_reload.Contains(name)) 
                force_reload("setting changed " + name);
        }

        public override bool are_settings_complete {
            get {
                if (remote_machine_name != "")
                    return remote_password_ != "" || settings.event_remote_password != "";
                return true;
            }
        }

        public override bool fully_read_once {
            get { return fully_read_once_; }
        }

        public int old_event_count {
            get { return old_event_count_; }
        }

        public override void force_reload(string reason) {
            foreach (var log in event_logs_)
                log.disposed_ = true;
            event_logs_ = new List<log_info>();

            base.force_reload(reason);
            fully_read_once_ = false;
            errors_.clear();
            logs_created_ = false;
        }

        private void create_logs() {
            if (logs_created_)
                return;            
            logs_created_ = true;
            logger.Debug("event_log - recreating logs ");

            foreach (var log in event_logs_)
                log.disposed_ = true;
            event_logs_ = new List<log_info>();
            lock (this) {
                has_done_gc_collect_ = false;

                if (lines_now_.Count > 0) {
                    logger.Warn("event log: did have elements on create_logs " + lines_now_.Count);
                    lines_now_.Clear();
                }
            }

            var new_event_logs = new List<log_info>();
            lock(this)
                foreach (var type in log_types) {
                    try {
                        var log = new log_info { log_type = type, remote_machine_name = remote_machine_name, remote_domain = remote_domain_name, remote_user_name = remote_user_name};
                        new_event_logs.Add( log);
                        new Thread(() => read_single_log_thread(log)) {IsBackground = true }.Start();
                    } catch (Exception e) {
                        logger.Error("can't create event log " + type + "/" + remote_machine_name + " : " + e.Message);
                        errors_.add("Can't create Log " + type + " on machine " + remote_machine_name + ", Reason=" + e.Message);
                    }
                }

            event_logs_ = new_event_logs;
        }

        private List<Tuple<EventRecord, string>> read_next_lines_single_log() {
            var next = (event_logs_[0].last_events_.Count > 0 ? event_logs_[0].last_events_ : event_logs_[0].new_events_)
                .Select(x => new Tuple<EventRecord,string>(x, event_logs_[0].log_type)).ToList() ;
            event_logs_[0].last_events_.Clear();
            event_logs_[0].new_events_.Clear();
            return next;
        }

        private List<Tuple<EventRecord, string>> read_next_lines_new_events_on_all_threads() {
            if (old_event_count_ < 0) 
                old_event_count_ = event_count_so_far_;
                        
            var all = new List<Tuple<EventRecord,string>>();
            var now = DateTime.Now;
            foreach (var log in event_logs_)
                // we're waiting a bit before returning new events - just in case different threads might come up with earlier entries 
                // (because we can't really count on any speed at all)
                all.AddRange(log.new_events_
                                    .Where(x => x.TimeCreated.Value.AddMilliseconds(MAX_DIFF_NEW_EVENT_MS) <= now)
                                    .Select(x => new Tuple<EventRecord, string>(x, log.log_type)) );
            all = all.OrderBy(x => x.Item1.TimeCreated).ToList();
            foreach (var log in event_logs_)
                foreach (var entry in all)
                    log.new_events_.Remove(entry.Item1);
            return  all;
        }

        private List<Tuple<EventRecord, string>> read_next_lines_existing_events() {
            bool needs_more_processing = true;
            // ... the last bool -> if true, the item can be added right now ; if false, we can't add this item now
            List < Tuple<EventRecord,string,int, bool> > last = new List<Tuple<EventRecord,string, int, bool>>();
            List< Tuple<EventRecord,string>>  next = new List<Tuple<EventRecord, string>>();
            while (needs_more_processing) {
                // 1.6.10+ at this point, we know AT LEAST one thread still has old events

                // If there is one or more threads that doesn't have at least one element, return an empty list
                int listen_for_last_events_and_have_no_events = event_logs_.Count(x => !x.listening_for_new_events_ && x.last_events_.Count == 0);
                if (listen_for_last_events_and_have_no_events > 0)
                    // at least one thread listening for old events and got nothing
                    break;

                // 1.6.10+ if I have new events, I will NOT return any, until all old events have been processed
                //         this is so that we correctly handle wether we're reversed or not
                last.Clear();
                for (int log_idx = 0; log_idx < event_logs_.Count; log_idx++) {
                    var log = event_logs_[log_idx];
                    if ( log.last_events_.Count > 0)
                        last.Add(new Tuple<EventRecord, string,int, bool>( log.last_events_[0], log.log_type, log_idx, log.last_events_.Count > 1 || log.listening_for_new_events_));
                    else 
                        Debug.Assert(event_logs_[log_idx].listening_for_new_events_);
                }
                if (last.Count < 1)
                    break;
                var min = are_elements_in_reverse_order ? last.Max(x => x.Item1.TimeCreated) :  last.Min(x => x.Item1.TimeCreated);
                var item = last.Find(x => x.Item1.TimeCreated == min);
                if (!item.Item4)
                    break;

                event_logs_[item.Item3].last_events_.RemoveAt(0);
                next.Add( new Tuple<EventRecord, string>(item.Item1, item.Item2) );
                if (next.Count >= MAX_ITEMS_PER_BLOCK)
                    needs_more_processing = false;
            }
            last.TrimExcess();
            return next;
        }

        protected override List<log_entry_line> read_next_lines() {
            create_logs();

            lock (this) 
                fully_read_once_ = event_logs_.Count(x => x.listening_for_new_events_ && x.last_events_.Count == 0) == event_logs_.Count;

            var old_logs = event_logs_;
            // http://www.codeproject.com/Messages/5162204/sorting-information-from-several-threads-is-my-alg.aspx
            List< Tuple<EventRecord,string>>  next = null;
            bool needs_more_processing = true;
            bool needs_gc_collect = false;
            lock (this) {
                // special case - a single log
                if (old_logs.Count == 1) {
                    next = read_next_lines_single_log();
                    needs_more_processing = false;
                }

                if (needs_more_processing) {
                    int listen_for_new_events = old_logs.Count(x => x.listening_for_new_events_);
                    // note: if we're listing for new events, first process all the last ones
                    if (listen_for_new_events == old_logs.Count && old_logs.Count(x => x.last_events_.Count == 0) == old_logs.Count) {
                        next = read_next_lines_new_events_on_all_threads();
                        needs_more_processing = false;
                        if (!has_done_gc_collect_) 
                            needs_gc_collect = has_done_gc_collect_ = true;
                    }
                }

                if (needs_more_processing)
                    next = read_next_lines_existing_events();
            } // lock


            /* note: this pretty much does not bring any speed improvement whatsoever
                     I'm guessing that inside EventRecord.GetFormatDescription - there's a global lock, so that even if I'm calling this 
                     from several threads, still no improvement happens
            var next_lines = new List<log_entry_line>(next.Count);
            for ( int i = 0; i < next.Count; ++i) next_lines.Add(null);
            Parallel.For(0, next.Count, (i) => next_lines[i] = to_log_entry(next[i].Item1, next[i].Item2) );
            */
            // 1.5.6t - to_log_entry can throw a lot of errors, we don't want that while being lock()ed
            var next_lines = next.Select( (x) => to_log_entry(x.Item1, x.Item2) ).ToList();
            event_count_so_far_ += next.Count;

            if (needs_gc_collect) 
                // 1.6.20+ we've created lots of objects (especially EventLogRecord) - lets dispose of them
                util.start_gc_collect( "event log");

            if (old_logs != event_logs_) {
                // 1.6.21+ in this case, we did a refresh of the event log while we were converting event log entries to our internal structure
                logger.Debug("event_log - ignoring " + next_lines.Count + " entries, because some event logs got disposed" );
                next_lines.Clear();
            }
            return next_lines;
        }

        private void read_single_log_thread(log_info log) {
            string query_string = "*";
            if (provider_name != "")
                query_string = "*[System/Provider/@Name=\"" + provider_name + "\"]";

            logger.Info("event_log new thread - reading from " + log.log_type );

            int max_event_count = int.MaxValue;
            // debugging - load much less, faster testing
            if (util.is_debug)
                //max_event_count = 250;
                max_event_count = 750;

            try {
                // we can read the number of entres only for local logs
                if (provider_name == "" && log.remote_machine_name == "") {
                    var dummy_log = new EventLog(log.log_type);
                    lock(this)
                        log.full_log_count_ = dummy_log.Entries.Count;
                    dummy_log.Dispose();
                }

                // waiting for user to set the password
                if ( log.remote_machine_name != "")
                    while ( remote_password_ == "")
                        Thread.Sleep(100);

                SecureString pwd = new SecureString();
                foreach ( char c in remote_password_)
                    pwd.AppendChar(c);
                EventLogSession session = log.remote_machine_name != "" ? new EventLogSession(log.remote_machine_name, remote_domain_name, remote_user_name, pwd, SessionAuthentication.Default) : null;
                pwd.Dispose();

                EventLogQuery query = new EventLogQuery(log.log_type, PathType.LogName, query_string);
                if ( are_elements_in_reverse_order)
                    query.ReverseDirection = are_elements_in_reverse_order;
                if ( session != null)
                    query.Session = session;

                int read_idx = 0;
                using (EventLogReader reader = new EventLogReader(query)) 
                    for (EventRecord rec = reader.ReadEvent(); rec != null && !log.disposed_ && read_idx++ < max_event_count; rec = reader.ReadEvent())
                        lock (this) {
                            log.last_events_.Add(rec);
                            ++log.cur_log_count_;
                        }

                logger.Info("event_log reading from " + log.log_type + " complete - read " + log.cur_log_count_ + " records");

                lock (this)
                    log.listening_for_new_events_ = true;

                // at this point, listen for new events
                if (are_elements_in_reverse_order) {
                    // if reverse, I need to create another query, or it won't allow watching
                    query = new EventLogQuery(log.log_type, PathType.LogName, query_string);                
                    if ( session != null)
                        query.Session = session; 
                }
				using (var watcher = new EventLogWatcher(query)) {
                    logger.Info("event_log reading from " + log.log_type + " watching for new events ");
					watcher.EventRecordWritten += (o, e) => {
                        lock(this)
                            log.new_events_.Add(e.EventRecord);
					};

					watcher.Enabled = true;
                    while ( !log.disposed_)
                        Thread.Sleep(100);
				    watcher.Enabled = false;
				}

            } catch (Exception e) {
                logger.Error("can't create event log " + log.log_type + "/" + remote_machine_name + " : " + e.Message);
                errors_.add("Can't create Log " + log.log_type + " on machine " + remote_machine_name + ", Reason=" + e.Message);
            }
            
            logger.Info("event_log reading from " + log.log_type + " watching for new events COMPLETE");
        }

        private log_entry_line to_log_entry(EventRecord rec, string log_name) {
            log_entry_line entry = new log_entry_line();
            try {
                entry.add("Log", log_name);
                entry.add("EventID", "" + rec.Id);

                entry.add("level", event_level((StandardEventLevel) rec.Level));
                entry.analyze_and_add("timestamp", rec.TimeCreated.Value);

                try {
                    var task = rec.Task != 0 ? rec.TaskDisplayName : "";
                    entry.add("Category", task ?? "");
                } catch {
                    entry.add("Category", "");
                }

                entry.add("Machine Name", rec.MachineName);
                entry.add("Source", "" + rec.ProviderName);
                string user_id = rec.UserId != null ? rec.UserId.Value : "";
                if (user_id != "")
                    user_id = new SecurityIdentifier(user_id).Translate(typeof(NTAccount)).ToString();;
                entry.add("User Name", user_id);
                /* 1.5.14+ this generates waaaay too many errors - just ignore for now
                try {
                    var keywords = rec.KeywordsDisplayNames;
                    entry.add("Keywords", keywords != null ? util.concatenate(keywords, ",") : "");
                } catch {
                    entry.add("Keywords", "");
                }*/


                // note: this throws a lot of exceptions; however, we don't have much of a choice here - just showing the raw properties is rather useless
                try {
                    var desc = rec.FormatDescription();
                    if ( desc == null)
                        desc = util.concatenate( rec.Properties.Select(x => x.Value.ToString()), "\r\n");
                    entry.add("msg", desc ?? "");
                } catch {
                    try {
                        string desc = util.concatenate( rec.Properties.Select(x => x.Value.ToString()), "\r\n");
                        entry.add("msg", desc);
                    } catch {
                        entry.add("msg", "");
                    }
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
        /*
        protected override void read_entries_thread() {
            // http://stackoverflow.com/questions/7531557/why-does-eventrecord-formatdescription-return-null
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            while (!disposed) {
                Thread.Sleep( app.inst.check_new_lines_interval_ms);
                bool reloaded;
                lock (this) {
                    reloaded = reloaded_;
                    reloaded_ = false;
                }

                var lines = read_next_lines();
                if (reverse_order)
                    lines.Reverse();
                if (lines.Count > 0 || reloaded) {
                    lock(this)
                        if ( !reverse_order)
                            lines_now_.AddRange(lines);
                        else 
                            lines_now_.InsertRange(0, lines);
                    parser.on_log_has_new_lines(reloaded);
                }
            }
        }*/

    }
}
