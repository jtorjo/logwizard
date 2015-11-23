using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using lw_common.parse;

namespace lw_common.ui {
    public partial class edit_log_settings_form : Form {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private settings_as_string old_settings_;
        private settings_as_string settings_;
        private string file_name_;

        private bool needs_restart_ = false;

        private double_dictionary<string,int> type_to_index_ = new double_dictionary<string, int>( new Dictionary<string, int>() {
            { "file", 0 }, { "event_log", 1 }, { "debug_print", 2}, { "db", 3}, {"multi", 4}
        });

        private double_dictionary<string, int> file_type_to_index_ = new double_dictionary<string, int>(new Dictionary<string, int>() {
            { "", 0}, { "line-by-line", 1}, { "part-by-line", 2}, { "xml", 3}, { "csv", 4}
        });

        public enum edit_type {
            edit,
            add
        };

        private edit_type edit_;
        private bool closed_ = false;
        private List<string> available_event_logs_ = new List<string>();

        private string successful_machine_ = "";

        // file name is set only if it's a file
        public edit_log_settings_form(string settings, edit_type edit = edit_type.edit) {
            old_settings_ = new settings_as_string(settings);
            settings_ = new settings_as_string(settings);
            file_name_ =  settings_.get("type") == "file" ? settings_.get("name") : "" ;

            edit_ = edit;
            InitializeComponent();
            type.Enabled = edit == edit_type.add;

            if (edit == edit_type.add) {
                settings_.set("event.log_type", "Application|System");
            }

            hide_tabs(typeTab);
            hide_tabs(fileTypeTab);
            cancel.Left = -100;
            friendlyName.Text = settings_.get("friendly_name");

            fileType.SelectedIndex = file_type_to_index( settings_.get("file_type") );

            syntax.Text = settings_.get("syntax");
            syntax.ForeColor = syntax.Text == find_log_syntax.UNKNOWN_SYNTAX ? Color.Red : Color.Black;
            ifLine.Checked = settings_.get("line.if_line", "0") != "0";
            
            partSeparator.Text = settings_.get("part.separator");

            xmlDelimeter.Text = settings_.get("xml.delimeter");

            csvHasHeader.Checked = settings_.get("csv.has_header", "1") != "0";
            csvSeparator.Text = settings_.get("csv.separator", ",");

            remoteMachineName.Text = settings_.get("event.remote_machine_name");
            remoteDomain.Text = settings_.get("event.remote_domain");
            remoteUserName.Text = settings_.get("event.remote_user_name");
            remotePassword.Text = settings_.get("event.remote_password");
            selectedEventLogs.Text = settings_.get("event.log_type").Replace("|", "\r\n");

            type.SelectedIndex = type_to_index();
            if (edit == edit_type.add) {
                Text = "Open Log";
                settings_.set("guid", Guid.NewGuid().ToString());
                util.postpone(() => type.Focus(), 1);
                util.postpone(() => type.DroppedDown = true, 200);
            }
            if (edit == edit_type.edit && typeTab.SelectedIndex == 1 && remoteMachineName.Text.Trim() != "")
                util.postpone(() => remotePassword.Focus(), 1);

            new Thread(check_event_log_thread) {IsBackground = true}.Start();
        }

        private void check_event_log_thread() {
            /* Examples of valid names on my machine:

                Microsoft-Windows-TWinUI/Operational
                Microsoft-Windows-AppxPackaging/Operational
                Microsoft-Windows-AppModel-Runtime/admin


            The remote machine:
            - It must be discoverable
            - Account you use must belong to "Event Log Readers"
            - You must enable the Remote Event Log Management exception in the Windows Firewall Settings on the remote computer to which you want to connect.
            - "Remote Registry" must be running on the remote computer
            */
            while (!closed_) {
                Thread.Sleep(250);
                bool needs_check = false;
                string[] event_logs = null;
                string remote_machine_name = null, remote_domain_name = null, remote_user_name = null, remote_password_name = null;
                this.async_call_and_wait(() => {
                    needs_check = typeTab.SelectedIndex == 1;
                    event_logs = selectedEventLogs.Text.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                    remote_machine_name = remoteMachineName.Text;
                    remote_domain_name = remoteDomain.Text;
                    remote_user_name = remoteUserName.Text;
                    remote_password_name = remotePassword.Text;
                    if (!eventLogs.Enabled)
                        // user hasn't connected to remote machine yet
                        needs_check = false;
                });
                if (!needs_check)
                    continue;
                // check all names typed by user
                List<string> dont_exist = new List<string>();
                foreach ( string log in event_logs)
                    try {
                        // note: for non standard logs, such as "Microsoft-Windows-TWinUI/Operational", Exists() returns false
                        bool exists = remote_machine_name != "" ? false : EventLog.Exists(log);
                        if ( !exists)
                            exists = remote_event_log_exists(log, remote_machine_name, remote_domain_name, remote_user_name, remote_password_name);
                        if ( !exists)
                            dont_exist.Add(log);
                    } catch {
                        dont_exist.Add(log);
                    }

                this.async_call_and_wait(() => {
                    if (dont_exist.Count < 1) {
                        eventLogCheckStatus.Text = "OK";
                        eventLogCheckStatus.ForeColor = Color.ForestGreen;
                    } else {
                        eventLogCheckStatus.Text = "Logs [" + util.concatenate(dont_exist, ", ") + "] don't exist.";
                        eventLogCheckStatus.ForeColor = Color.Red;
                    }
                });
            }
        }

        private static bool remote_event_log_exists(string log, string remote_machine_name, string remote_domain_name, string remote_user_name, string remote_password_name) {
            try {
                SecureString pwd = new SecureString();
                foreach (char c in remote_password_name)
                    pwd.AppendChar(c);
                EventLogSession session = remote_machine_name.Trim() != ""
                    ? new EventLogSession(remote_machine_name, remote_domain_name, remote_user_name, pwd, SessionAuthentication.Default)
                    : null;
                pwd.Dispose();
                EventLogQuery query = new EventLogQuery(log, PathType.LogName);
                if (session != null)
                    query.Session = session;

                EventLogReader reader = new EventLogReader(query);
                if (reader.ReadEvent(TimeSpan.FromMilliseconds(500)) != null)
                    return true;
            } catch(Exception e) {
                logger.Error("can't login " + e.Message);
            }
            return false;
        }

        private int type_to_index() {
            return type_to_index_.key_to_value(settings_.get("type", "file"));
        }

        private string index_to_type() {
            return type_to_index_.value_to_key(type.SelectedIndex);
        }

        private int file_type_to_index(string file_type) {
            return file_type_to_index_.key_to_value(file_type);
        }

        private string index_to_file_type() {
            return file_type_to_index_.value_to_key(fileType.SelectedIndex);
        }

        public string friendly_name {
            get { return friendlyName.Text; }
        }

        public string settings {
            get { return settings_.ToString(); }
        }

        public bool needs_restart {
            get { return needs_restart_; }
        }

        private void hide_tabs(TabControl tab) {
            int page_height = tab.SelectedTab != null ? tab.SelectedTab.Height : tab.TabPages[0].Height;
            int extra = tab.Height - page_height;
            tab.Top -= extra;
            tab.Height += extra;
        }

        private void cancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }

        private void ok_Click(object sender, EventArgs e) {
            save_settings();
            needs_restart_ = change_needs_restart();
            DialogResult = DialogResult.OK;
        }

        private void save_settings() {
            settings_.set("type", index_to_type());
            settings_.set("file_type", index_to_file_type());

            if ( index_to_type() == "file" && edit_ == edit_type.add)
                settings_.set("name", fileName.Text);

            settings_.set("friendly_name", friendlyName.Text);

            settings_.set("syntax", syntax.Text);
            settings_.set("line.if_line", ifLine.Checked ? "1" : "0");
            settings_.set("part.separator", partSeparator.Text);
            settings_.set("xml.delimeter", xmlDelimeter.Text);
            settings_.set("csv.has_header", csvHasHeader.Checked ? "1" : "0");
            settings_.set("csv.separator", csvSeparator.Text);            

            settings_.set("event.remote_machine_name", remoteMachineName.Text);
            settings_.set("event.remote_domain", remoteDomain.Text);
            settings_.set("event.remote_user_name", remoteUserName.Text);
            settings_.set("event.remote_password", remotePassword.Text);
            settings_.set("event.log_type", selectedEventLogs.Text.Trim().Replace("\r\n", "|"));

            settings_.set("debug.global", debugGlobal.Checked ? "1" : "0");
            settings_.set("debug.process_name", debugProcessName.Text);

            // syntax_type is used internally, to know if the user has changed the syntax
            settings_.set("syntax_type", settings_.get("syntax") != old_settings_.get("syntax") ? "edited_now" : "");
        }

        private bool change_needs_restart() {
            if (edit_ == edit_type.add)
                return false;

            if (old_settings_.get("type", "file") != settings_.get("type"))
                return true;
            if ( settings_.get("type") == "file")
                if (old_settings_.get("file_type") != settings_.get("file_type"))
                    // user changed format, like, from XML to CSV
                    return true;
            return false;
        }

        private void type_SelectedIndexChanged(object sender, EventArgs e) {
            if (type.DroppedDown)
                return;

            typeTab.SelectedIndex = type.SelectedIndex;

            if (index_to_type() == "event_log")
                update_event_log_list();
        }
        private void type_DropDownClosed(object sender, EventArgs e) {
            type_SelectedIndexChanged(null,null);
        }


        private void fileType_SelectedIndexChanged(object sender, EventArgs e) {
            if (fileType.SelectedIndex > 0)
                fileTypeTab.SelectedIndex = fileType.SelectedIndex - 1;
            else 
                // best guess
                fileTypeTab.SelectedIndex = file_type_to_index(factory.guess_file_type(file_name_)) - 1;            
        }

        private void editSyntax_Click(object sender, EventArgs e) {
            string guess = "";
            try {
                using (var fs = new FileStream(file_name_, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    // read a few lines from the beginning
                    byte[] readBuffer = new byte[find_log_syntax.READ_TO_GUESS_SYNTAX];
                    int bytes = fs.Read(readBuffer, 0, find_log_syntax.READ_TO_GUESS_SYNTAX);
                    var encoding = util.file_encoding(file_name_);
                    if (encoding == null)
                        encoding = Encoding.Default;
                    guess = encoding.GetString(readBuffer, 0, bytes);
                }
            } catch {
            }

            // 1.3.24+ - use the old syntax when we're modifying
            var test = new test_syntax_form(guess, settings_.get("syntax"));
            if (test.ShowDialog() == DialogResult.OK) {
                settings_.set("syntax", test.found_syntax);
                syntax.Text = test.found_syntax;
                syntax.ForeColor = syntax.Text == find_log_syntax.UNKNOWN_SYNTAX ? Color.Red : Color.Black;
            }

        }

        private void checkRequiresRestart_Tick(object sender, EventArgs e) {
            save_settings();
            needsRestart.Visible = change_needs_restart() && !fileType.DroppedDown;
        }

        private void syntaxLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/jtorjo/logwizard/wiki/Syntax");
        }

        private void update_event_log_list() {
            load_available_event_logs();

            try {
                if (remoteUserName.Text == "")
                    remoteUserName.Text = Environment.UserName;
                if (remoteDomain.Text == "")
                    remoteDomain.Text = Environment.UserDomainName;
            } catch {
            }

            remoteMachineName_TextChanged(null,null);
        }

        private void load_available_event_logs() {
            available_event_logs_.Clear();
            eventLogs.Items.Clear();

            try {
                string machine_name = remoteMachineName.Text;
                var logs = machine_name == "" ? EventLog.GetEventLogs() : EventLog.GetEventLogs(machine_name);
                foreach (var log in logs) {
                    available_event_logs_.Add(log.Log);
                    bool is_checked = selectedEventLogs.Text.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).Contains(log.Log);
                    eventLogs.Items.Add(friendly_event_log_name(log), is_checked);
                }

            } catch (Exception e) {
                logger.Error("update event log list: " + e.Message);
            }            
        }

        private string friendly_event_log_name(EventLog log) {
            return log.LogDisplayName + " (" + log.Entries.Count + " events, max size:" + log.MaximumKilobytes + " KB)";
        }

        private void remoteMachineName_TextChanged(object sender, EventArgs e) {
            bool is_remote = remoteMachineName.Text.Trim() != "";
            remoteDomain.Enabled = is_remote;
            remoteUserName.Enabled = is_remote;
            remotePassword.Enabled = is_remote;
            testRemote.Visible = is_remote;
            eventLogRemoteSstatus.Visible = is_remote;

            successful_machine_ = "";
            eventLogRemoteSstatus.Text = "NOT connected at this time";
            eventLogRemoteSstatus.ForeColor = Color.Red;

            selectedEventLogs.Enabled = !is_remote;
            eventLogs.Enabled = !is_remote;
            ok.Enabled = !is_remote;
        }

        private void edit_log_settings_form_FormClosing(object sender, FormClosingEventArgs e) {
            closed_ = true;
        }

        private void selectedEventLogs_Enter(object sender, EventArgs e) {
            AcceptButton = null;
        }

        private void selectedEventLogs_Leave(object sender, EventArgs e) {
            AcceptButton = ok;
        }

        private void testRemote_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            bool ok = remote_event_log_exists("Application", remoteMachineName.Text, remoteDomain.Text != "" ? remoteDomain.Text : null, remoteUserName.Text, remotePassword.Text);
            successful_machine_ = ok ? remoteMachineName.Text : "";
            eventLogRemoteSstatus.Text = ok ? "Success!" : "NOT connected at this time";
            eventLogRemoteSstatus.ForeColor =  ok ? Color.ForestGreen : Color.Red;
            this.ok.Enabled = ok;
            if (ok)
                load_available_event_logs();
            selectedEventLogs.Enabled = ok;
            eventLogs.Enabled = ok;
            Cursor = Cursors.Default;
        }

    }
}
