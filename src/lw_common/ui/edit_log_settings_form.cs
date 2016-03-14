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
using System.Threading.Tasks;
using System.Windows.Forms;
using lw_common.parse;
using LogWizard.context;

namespace lw_common.ui {
    public partial class edit_log_settings_form : Form {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private log_settings_string old_settings_;
        private log_settings_string settings_;

        private bool needs_restart_ = false;

        private double_dictionary<log_type,int> type_to_index_ = new double_dictionary<log_type, int>( new Dictionary<log_type, int>() {
            { log_type.file, 0 }, { log_type.event_log, 1 }, { log_type.debug_print, 2}, { log_type.db, 3}, {log_type.multi, 4}
        });

        private double_dictionary<file_log_type, int> file_type_to_index_ = new double_dictionary<file_log_type, int>(new Dictionary<file_log_type, int>() {
            { file_log_type.best_guess, 0}, { file_log_type.line_by_line, 1}, {file_log_type.part_to_line, 2}, { file_log_type.xml, 3}, { file_log_type.csv, 4}
        });

        public enum edit_type {
            edit,
            add
        };

        private edit_type edit_;
        private bool closed_ = false;
        private List<string> available_event_logs_ = new List<string>();

        private string successful_machine_ = "";
        private bool edited_syntax_now_ = false;

        private bool first_select_event_log_type_ = true;

        private int ignore_change_ = 0;

        // file name is set only if it's a file
        public edit_log_settings_form(string settings, edit_type edit = edit_type.edit) {
            old_settings_ = new log_settings_string(settings);
            settings_ = new log_settings_string(settings);
            edit_ = edit;
            InitializeComponent();
            fileName.Text =  settings_.type == log_type.file ? settings_.name : "" ;
            type.Enabled = edit == edit_type.add;
            browserFile.Enabled = edit == edit_type.add;

            hide_tabs(typeTab);
            hide_tabs(fileTypeTab);
            cancel.Left = -100;
            friendlyName.Text = settings_.friendly_name;
            fileType.SelectedIndex = file_type_to_index( settings_.file_type );
            reversed.Checked = settings_.reverse;

            update_syntax();
            ifLine.Checked = settings_.line_if_line_does_not_match_syntax;
            ifLineStartsWithTab.Checked = settings_.line_if_line_starts_with_tab;
            
            partSeparator.Text = settings_.part_separator;

            xmlDelimeter.Text = settings_.xml_delimiter;

            csvHasHeader.Checked = settings_.cvs_has_header;
            csvSeparator.Text = settings_.cvs_separator_char;

            remoteMachineName.Text = settings_.event_remote_machine_name;
            remoteDomain.Text = settings_.event_remote_domain;
            remoteUserName.Text = settings_.event_remote_user_name;
            remotePassword.Text = settings_.event_remote_password;
            selectedEventLogs.Text = settings_.event_log_type.get() .Replace("|", "\r\n");

            dbProvider.SelectedIndex = db_provider_string_to_index( settings_.db_provider);
            dbConnectionString.Text = settings_.db_connection_string;
            dbTableName.Text = settings_.db_table_name;
            dbFields.Text = settings_.db_fields;
            dbUniqueIdField.Text = settings_.db_id_field;
            update_db_mappings();

            type.SelectedIndex = type_to_index();
            if (edit == edit_type.add) {
                Text = "Open Log";
                settings_.guid .set( Guid.NewGuid().ToString());
                // 1.8.7+ if it's anything else than file, we have preset some settings - just let the user see them 
                //        (such as, when user drops an sqlite file, and we fill pretty much all details)
                if (settings_.type.get() == log_type.file) {
                    util.postpone(() => type.Focus(), 1);
                    util.postpone(() => type.DroppedDown = true, 200);
                }
            }
            if (edit == edit_type.edit && typeTab.SelectedIndex == 1 && remoteMachineName.Text.Trim() != "")
                util.postpone(() => remotePassword.Focus(), 1);

            new Thread(check_event_log_thread) {IsBackground = true}.Start();
        }

        private int db_provider_string_to_index(string provider) {
            for ( int i = 0; i < dbProvider.Items.Count; ++i)
                if (dbProvider.Items[i].ToString().Contains("(" + provider + ")"))
                    return i;
            return 0;
        }

        private string db_provider_index_to_string() {
            var str = dbProvider.Items[dbProvider.SelectedIndex].ToString();
            Debug.Assert(str.Contains("(") && str.Contains(")"));
            int start = str.IndexOf("(") + 1, end = str.IndexOf(")");
            return str.Substring(start, end - start);
        }

        private List<Tuple<string, info_type>> get_db_mappings() {
            List<Tuple<string,info_type>> user_typed_mappings = new List<Tuple<string, info_type>>();
            foreach (var line in dbFields.Text.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries)) {
                int delim = line.IndexOf('=');
                if (delim >= 0) {
                    string name = line.Substring(0, delim).Trim(), value = line.Substring(delim + 1).Trim();
                    var info_value = info_type_io.from_str(value);
                    user_typed_mappings.Add( new Tuple<string, info_type>(name, info_value));
                }
                else 
                    user_typed_mappings.Add(new Tuple<string, info_type>(line.Trim(), info_type.max));
            }
            var lw_mappings = info_type_io.match_db_column_to_lw_column(user_typed_mappings);
            return lw_mappings;
        }

        private void update_db_mappings() {
            var lw_mappings = get_db_mappings();
            dbMappings.Text = util.concatenate( lw_mappings.Select(x => x.Item1 + "=" + info_type_io.to_friendly_str(x.Item2)), "\r\n" );
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
            return type_to_index_.key_to_value(settings_.type);
        }

        private log_type index_to_type() {
            return type_to_index_.value_to_key(type.SelectedIndex);
        }

        private int file_type_to_index(file_log_type file_type) {
            return file_type_to_index_.key_to_value(file_type);
        }

        private file_log_type index_to_file_type() {
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

        public bool edited_syntax_now {
            get { return edited_syntax_now_; }
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
            settings_.type .set(index_to_type());
            settings_.file_type .set(index_to_file_type());

            if ( index_to_type() == log_type.file && edit_ == edit_type.add)
                settings_.name .set( fileName.Text);

            settings_.friendly_name.set( friendlyName.Text);

            settings_.syntax.set( syntax.Text);
            settings_.reverse.set( reversed.Checked );

            settings_.line_if_line_does_not_match_syntax.set( ifLine.Checked );
            settings_.line_if_line_starts_with_tab.set( ifLineStartsWithTab.Checked);
            settings_.part_separator.set(partSeparator.Text);
            settings_.xml_delimiter.set(xmlDelimeter.Text);
            settings_.cvs_has_header.set(csvHasHeader.Checked);
            settings_.cvs_separator_char.set(csvSeparator.Text);            

            settings_.event_remote_machine_name.set(remoteMachineName.Text);
            settings_.event_remote_domain.set(remoteDomain.Text);
            settings_.event_remote_user_name.set(remoteUserName.Text);
            settings_.event_remote_password.set(remotePassword.Text);
            settings_.event_log_type.set(selectedEventLogs.Text.Trim().Replace("\r\n", "|"));

            settings_.debug_global.set(debugGlobal.Checked );
            settings_.debug_process_name.set(debugProcessName.Text);

            settings_.db_provider.set( db_provider_index_to_string());
            settings_.db_connection_string.set( dbConnectionString.Text);
            settings_.db_table_name.set(dbTableName.Text);
            settings_.db_fields.set( dbFields.Text);
            settings_.db_id_field.set( dbUniqueIdField.Text);

            edited_syntax_now_ = settings_.syntax != old_settings_.syntax;
        }

        private bool change_needs_restart() {
            if (edit_ == edit_type.add)
                return false;

            if (old_settings_.type != settings_.type)
                return true;
            if ( settings_.type == log_type.file)
                if (old_settings_.file_type != settings_.file_type)
                    // user changed format, like, from XML to CSV
                    return true;
            return false;
        }

        private void type_SelectedIndexChanged(object sender, EventArgs e) {
            if (type.DroppedDown)
                return;

            typeTab.SelectedIndex = type.SelectedIndex;

            bool is_event_log = index_to_type() == log_type.event_log;
            if (is_event_log) {
                if (first_select_event_log_type_ && edit_ == edit_type.add)
                    reversed.Checked = is_event_log;
                first_select_event_log_type_ = false;
            }
            if (is_event_log)
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
                fileTypeTab.SelectedIndex = file_type_to_index(factory.guess_file_type(fileName.Text)) - 1;            
        }

        private void editSyntax_Click(object sender, EventArgs e) {
            string guess = "";
            try {
                using (var fs = new FileStream(fileName.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    // read a few lines from the beginning
                    byte[] readBuffer = new byte[find_log_syntax.READ_TO_GUESS_SYNTAX];
                    int bytes = fs.Read(readBuffer, 0, find_log_syntax.READ_TO_GUESS_SYNTAX);
                    var encoding = util.file_encoding(fileName.Text);
                    if (encoding == null)
                        encoding = Encoding.Default;
                    guess = encoding.GetString(readBuffer, 0, bytes);
                }
            } catch {
            }

            // 1.3.24+ - use the old syntax when we're modifying
            var test = new test_syntax_form(guess, settings_.syntax);
            if (test.ShowDialog() == DialogResult.OK) {
                settings_.syntax.set( test.found_syntax);
                update_syntax();
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

        private async void load_available_event_logs() {
            eventLogs.Items.Clear();

            string machine_name = remoteMachineName.Text;
            await Task.Run(() => load_available_event_logs_async(machine_name) );
        }

        private void load_available_event_logs_async(string machine_name) {
            // name, friendly name
            List< Tuple<string, string> > available_logs = new List<Tuple<string, string>>();

            try {
                var logs = machine_name == "" ? EventLog.GetEventLogs() : EventLog.GetEventLogs(machine_name);
                foreach (var log in logs) 
                    available_logs.Add( new Tuple<string, string>(log.Log, friendly_event_log_name(log)));

                // if it's local logs, try to go to C:\Windows\System32\winevt\Logs as well
                if (machine_name == "") {
                    string dir = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\winevt\\Logs";
                    var files = (new DirectoryInfo(dir).EnumerateFiles("*.evtx")).OrderBy(x => -x.LastWriteTime.Ticks).Select(x => {
                        var name = x.Name.Substring(0, x.Name.Length - 5).Replace("%4", "/");
                        string desc = "(? events, size: " + (x.Length / 1024) + " KB)";
                        return new Tuple<string, string>(name, name + desc);
                    }).ToList();
                    if (files.Count > app.inst.max_event_log_files)
                        files = files.GetRange(0, app.inst.max_event_log_files);
                    // remove dumplicates (entries we already have
                    files = files.Where(x => !available_logs.Any(y => y.Item1 == x.Item1) ).ToList();
                    available_logs.AddRange( files );
                }

            } catch (Exception e) {
                logger.Error("update event log list: " + e.Message);
            }

            available_event_logs_ = available_logs.Select(x => x.Item1).ToList();
            
            this.async_call(() => {
                ++ignore_change_;
                eventLogs.Items.Clear();
                foreach (var log in available_logs) {
                    bool is_checked = selectedEventLogs.Text.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).Contains(log.Item1);
                    eventLogs.Items.Add(log.Item2, is_checked);
                }
                --ignore_change_;
            });

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

        private void browserFile_Click(object sender, EventArgs e) {
            if (ofd.ShowDialog(this) == DialogResult.OK) {
                fileName.Text = ofd.FileName;
                save_settings();
                // best guess
                fileType.SelectedIndex = 0;
                if (fileTypeTab.SelectedIndex == 0) {
                    // line-by-line , try to find syntax
                    string file_syntax = log_to.file_to_syntax(fileName.Text);
                    if ( file_syntax == "")
                        file_syntax = new find_log_syntax().try_find_log_syntax_file(fileName.Text);
                    settings_.syntax.set(file_syntax);
                }
                update_syntax();
            }
        }

        private void update_syntax() {            
            syntax.Text = settings_.syntax;
            syntax.ForeColor = syntax.Text == find_log_syntax.UNKNOWN_SYNTAX ? Color.Red : Color.Black;
        }

        private void eventLogs_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (ignore_change_ > 0)
                return;
            if (e.Index < 0)
                return;

            bool is_checked = e.NewValue == CheckState.Checked;
            string name = eventLogs.Items[e.Index].ToString();
            int pos = name.IndexOf("(");
            if (pos >= 0)
                name = name.Substring(0, pos).Trim();

            if (is_checked) {
                selectedEventLogs.Text += "\r\n" + name;
            } else {
                // remove it
                var new_logs = selectedEventLogs.Text.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).Where(x => x != name);
                selectedEventLogs.Text = util.concatenate(new_logs, "\r\n");
            }
        }

        private void testLogSizes_Click(object sender, EventArgs e) {
            var log_names = selectedEventLogs.Text.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).ToList();
            var test = new test_event_logs_sizes_form(log_names, remoteMachineName.Text, remoteDomain.Text, remoteUserName.Text, remotePassword.Text);
            test.ShowDialog(this);
        }

        private void refresh_Tick(object sender, EventArgs e) {
            update_db_mappings();
        }

        private void testDb_Click(object sender, EventArgs ea) {
            typeTab.Enabled = false;
            Cursor = Cursors.WaitCursor;
            dbTestStatus.Text = "Testing Now... Please Wait...";
            // update UI
            Application.DoEvents();

            var lw_mappings = get_db_mappings();
            var fields = util.concatenate(lw_mappings.Select(x => x.Item1), ", ");
            string sql = "select " + fields + " from " + dbTableName.Text;
            // sort by date/time if possible
            var date_mapping = lw_mappings.FirstOrDefault(x => x.Item2 == info_type.date);
            var time_mapping = lw_mappings.FirstOrDefault(x => x.Item2 == info_type.time);
            if (date_mapping != null)
                sql += " order by " + date_mapping.Item1;
            if (time_mapping != null) {
                sql += sql.Contains(" order by ") ? ", " : " order by ";
                sql += time_mapping.Item1;
            }
            logger.Debug("executing db command [" + sql + "]" + " on [" + dbConnectionString.Text + "]");
            
            // test connecting to database + and reading 1 record + read everything we need from that record (according to the fields)
            string status = "Congratulations! Successful connection.";
            try {
                using (var conn = db_util.create_db_connection(db_provider_index_to_string(), dbConnectionString.Text))
                    using (var cmd = conn.CreateCommand()) {
                        conn.Open();
                        cmd.CommandText = sql;
                        using (var rs = cmd.ExecuteReader()) {
                            if (rs.Read()) {
                                // at this point, I will read all fields
                                for (int i = 0; i < lw_mappings.Count; ++i) {
                                    bool is_time = lw_mappings[i].Item2 == info_type.date || lw_mappings[i].Item2 == info_type.time;
                                    // here, I'm just testing for exceptions
                                    if (is_time) {
                                        // http://stackoverflow.com/questions/11414399/sqlite-throwing-a-string-not-recognized-as-a-valid-datetime
                                        // normally, GetDateTime should work. But just in case it doesn't, read it as a string, and then,
                                        //           later, we'll parse the date/time with util.normalize_* functions
                                        bool ok = true;
                                        try {
                                            rs.GetDateTime(i);
                                        } catch {
                                            ok = false;
                                        }
                                        if (!ok)
                                            rs.GetString(i);
                                    } else
                                        rs.GetString(i);
                                }
                            } else
                                status = "ERROR: Log Table is empty.";
                        }
                            
                    }
            } catch (Exception e) {
                status = "ERROR: " + e.Message;
            }
            dbTestStatus.ForeColor = status.StartsWith("ERROR") ? Color.Red : Color.LightSeaGreen;
            dbTestStatus.Text = status;

            typeTab.Enabled = true;
            Cursor = Cursors.Default;
        }

    }
}
