using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class test_event_logs_sizes_form : Form {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // convention - the value is positive only when we fully read the log
        private Dictionary<string,int> log_names_;
        private string remote_machine_name_;
        private string remote_domain_;
        private string remote_username_;
        private string remote_passw_;

        public test_event_logs_sizes_form(List<string> log_names, string remote_machine_name, string remote_domain, string remote_username, string remote_passw) {
            log_names_ = log_names.ToDictionary(x => x, x => 0);
            remote_machine_name_ = remote_machine_name;
            remote_domain_ = remote_domain;
            remote_username_ = remote_username;
            remote_passw_ = remote_passw;
            InitializeComponent();

            foreach (var name in log_names_.Keys) {
                string name_copy = name;
                Task.Run(() => check_log(name_copy));
            }
        }

        private void check_log(string name) {
            try {
                SecureString pwd = new SecureString();
                foreach ( char c in remote_passw_)
                    pwd.AppendChar(c);
                EventLogSession session = remote_machine_name_ != "" ? new EventLogSession(remote_machine_name_, remote_domain_, remote_username_, pwd, SessionAuthentication.Default) : null;
                pwd.Dispose();
                string query_string = "*";
                EventLogQuery query = new EventLogQuery(name, PathType.LogName, query_string);

                using (EventLogReader reader = new EventLogReader(query))
                    for (EventRecord rec = reader.ReadEvent(); rec != null; rec = reader.ReadEvent())
                        lock (this) 
                            --log_names_[name];

            } catch (Exception e) {
                logger.Error("error checking log " + name + " on " + remote_machine_name_ + " : " + e.Message);
            }

            // mark log as fully read
            lock (this) {
                log_names_[name] = -log_names_[name];
                if (log_names_[name] == 0)
                    // convention - 0 entries
                    log_names_[name] = int.MinValue;
            }
        }

        private void refresh_Tick(object sender, EventArgs e) {
            Dictionary<string, int> log_names;
            lock (this)
                log_names = log_names_.ToDictionary(x => x.Key, x => x.Value);
            if (log_names.Count < 1)
                return;

            int max_len = log_names.Keys.Max(x => x.Length);
            string status = "";
            foreach (var log in log_names) {
                status += log.Key + new string(' ', max_len - log.Key.Length) + " - ";
                if (log.Value == int.MinValue)
                    status += "NO ENTRIES";
                else if (log.Value <= 0)
                    status += "reading... - " + Math.Abs(log.Value) + " entries so far";
                else 
                    status += "exactly    - " + log.Value + " entries";
                status += "\r\n";
            }
            readStatus.Text = status;
        }
    }
}
