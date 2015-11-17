using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net.Repository.Hierarchy;
using lw_common.parse;
using MultiLanguage;

namespace lw_common {
    /* inspiration

- event log -- https://msdn.microsoft.com/en-us/library/74e2ybbs%28v=vs.110%29.aspx - allow as many as the user wants...

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
        private List<EventLog> event_logs_ = new List<EventLog>();

        private bool read_first_lines_ = false;

        public string[] log_types {
            get {
                if (util.is_debug)
                    // for testing - very few entries
                    return new [] { "Windows PowerShell" };

                return settings.get("event.log_type", "Application|System").Split('|');
            }
        }

        public string machine_name {
            get {
                return settings.get("event.machine_name", ".");                 
            }
        }

        public override bool fully_read_once {
            get { return fully_read_once_; }
        }

        public override string name {
            get { return "_event_log_"; }
        }

        public override void force_reload() {
            fully_read_once_ = false;
            logs_created_ = false;
            up_to_date_ = false;
            errors_.clear();
        }

        internal override void on_settings_changed() {
            base.on_settings_changed();
            force_reload();
        }

        private void create_logs() {
            if (logs_created_)
                return;
            logs_created_ = true;
            read_first_lines_ = false;
            event_logs_.Clear();
            foreach (var type in log_types) {
                try {
                    event_logs_.Add( new EventLog(type, machine_name));
                } catch (Exception e) {
                    logger.Error("can't create event log " + type + "/" + machine_name + " : " + e.Message);
                    errors_.add("Can't create Log " + type + " on machine " + machine_name + ", Reason=" + e.Message);
                }
            }
        }

        internal override List<log_entry_line> read_next_lines() {
            create_logs();

            if (!read_first_lines_) {
                read_first_lines_ = true;
                return read_next_lines_first_time();
            }
            List<log_entry_line> next = new memory_optimized_list<log_entry_line>();

            return next;
        }

        internal List<log_entry_line> read_next_lines_first_time() {

            List<EventLogEntry> entries = new List<EventLogEntry>();
            foreach ( var log in event_logs_)
                try { 
                    foreach ( EventLogEntry entry in log.Entries)
                        entries.Add(entry);
                } catch (Exception e) {
                    logger.Error("Can't read log entry " + e.Message);
                }
            if (event_logs_.Count > 1)
                // in this case, we read from several logs - need to sort the entries
                entries = entries.OrderBy(x => x.TimeGenerated).ToList();

            List<log_entry_line> next = new memory_optimized_list<log_entry_line>();
            foreach (var event_entry in entries) {
                var entry = new log_entry_line();
                entry.add("Category", event_entry.Category);
                entry.add("msg", event_entry.Message);
                entry.add("Machine Name", event_entry.MachineName);
                entry.add("level", event_level( event_entry.EntryType));
                entry.add("EventID", "" + event_entry.InstanceId);
                entry.add("Source", "" + event_entry.Source);
                entry.add("date", event_entry.TimeGenerated.ToString("DD-MM-YYYY"));
                entry.add("time", event_entry.TimeGenerated.ToString("hh:mm:ss.fff"));
                entry.add("User Name", event_entry.UserName);
                next.Add(entry);
            }

            // catch exceptions as well
            up_to_date_ = true;
            fully_read_once_ = true;
            return next;
        }

        private static string event_level(EventLogEntryType type) {
            switch (type) {
            case EventLogEntryType.Error:
                return "ERROR";
            case EventLogEntryType.Warning:
                return "WARN";
            case EventLogEntryType.Information:
                return "INFO";
            case EventLogEntryType.SuccessAudit:
                return "SUCCESS";
            case EventLogEntryType.FailureAudit:
                return "FAIL";
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
