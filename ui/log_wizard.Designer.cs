namespace LogWizard
{
    partial class log_wizard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being dimmed.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(log_wizard));
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.newView = new System.Windows.Forms.Button();
            this.logHistory = new System.Windows.Forms.ComboBox();
            this.newFilteredView = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.friendlyNameCtrl = new System.Windows.Forms.TextBox();
            this.logSyntaxCtrl = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.delFilteredView = new System.Windows.Forms.Button();
            this.curContextCtrl = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.addContext = new System.Windows.Forms.Button();
            this.delContext = new System.Windows.Forms.Button();
            this.synchronizedWithFullLog = new System.Windows.Forms.CheckBox();
            this.synchronizeWithExistingLogs = new System.Windows.Forms.CheckBox();
            this.contextFromClipboard = new System.Windows.Forms.Button();
            this.contextToClipboard = new System.Windows.Forms.Button();
            this.toggleTopmost = new System.Windows.Forms.PictureBox();
            this.toggles = new System.Windows.Forms.Button();
            this.toggleMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.currentViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.titleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterPaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notesPaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourcePanetopmostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topmostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.whatIsThisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsCtrl = new System.Windows.Forms.Button();
            this.about = new System.Windows.Forms.Button();
            this.main = new System.Windows.Forms.SplitContainer();
            this.leftPane = new System.Windows.Forms.TabControl();
            this.filtersTab = new System.Windows.Forms.TabPage();
            this.filtCtrl = new lw_common.ui.filter_ctrl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.notes = new lw_common.ui.note_ctrl();
            this.sourceUp = new System.Windows.Forms.SplitContainer();
            this.sourceNameCtrl = new System.Windows.Forms.TextBox();
            this.sourceTypeCtrl = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.filteredLeft = new System.Windows.Forms.SplitContainer();
            this.viewsTab = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dropHere = new System.Windows.Forms.Label();
            this.refreshFilter = new System.Windows.Forms.Button();
            this.refresh = new System.Windows.Forms.Timer(this.components);
            this.postFocus = new System.Windows.Forms.Timer(this.components);
            this.monitor = new System.Windows.Forms.Button();
            this.lower = new System.Windows.Forms.Panel();
            this.export = new System.Windows.Forms.Button();
            this.exportMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportLogNotestoLogWizardFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportNotestotxtAndhtmlFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hotkeys = new System.Windows.Forms.LinkLabel();
            this.status = new System.Windows.Forms.Label();
            this.saveTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.toggleTopmost)).BeginInit();
            this.toggleMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.main)).BeginInit();
            this.main.Panel1.SuspendLayout();
            this.main.Panel2.SuspendLayout();
            this.main.SuspendLayout();
            this.leftPane.SuspendLayout();
            this.filtersTab.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sourceUp)).BeginInit();
            this.sourceUp.Panel1.SuspendLayout();
            this.sourceUp.Panel2.SuspendLayout();
            this.sourceUp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.filteredLeft)).BeginInit();
            this.filteredLeft.Panel1.SuspendLayout();
            this.filteredLeft.SuspendLayout();
            this.viewsTab.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.lower.SuspendLayout();
            this.exportMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tip
            // 
            this.tip.AutoPopDelay = 32000;
            this.tip.InitialDelay = 500;
            this.tip.ReshowDelay = 100;
            // 
            // newView
            // 
            this.newView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newView.Location = new System.Drawing.Point(1193, 3);
            this.newView.Name = "newView";
            this.newView.Size = new System.Drawing.Size(51, 23);
            this.newView.TabIndex = 6;
            this.newView.Text = "New";
            this.tip.SetToolTip(this.newView, "Opens a new view, in which you can view another log");
            this.newView.UseVisualStyleBackColor = true;
            this.newView.Click += new System.EventHandler(this.newView_Click);
            // 
            // logHistory
            // 
            this.logHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logHistory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.logHistory.FormattingEnabled = true;
            this.logHistory.Location = new System.Drawing.Point(134, 3);
            this.logHistory.Name = "logHistory";
            this.logHistory.Size = new System.Drawing.Size(708, 23);
            this.logHistory.TabIndex = 7;
            this.tip.SetToolTip(this.logHistory, "History - just select any of the previous logs, and they instantly load");
            this.logHistory.SelectedIndexChanged += new System.EventHandler(this.logHistory_SelectedIndexChanged);
            this.logHistory.DropDownClosed += new System.EventHandler(this.logHistory_DropDownClosed);
            // 
            // newFilteredView
            // 
            this.newFilteredView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newFilteredView.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFilteredView.Location = new System.Drawing.Point(387, 29);
            this.newFilteredView.Name = "newFilteredView";
            this.newFilteredView.Size = new System.Drawing.Size(18, 20);
            this.newFilteredView.TabIndex = 1;
            this.newFilteredView.Text = "+";
            this.tip.SetToolTip(this.newFilteredView, "New Filtered View of the same Log");
            this.newFilteredView.UseVisualStyleBackColor = true;
            this.newFilteredView.Click += new System.EventHandler(this.newFilteredView_Click);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(743, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 15);
            this.label7.TabIndex = 7;
            this.label7.Text = "Friendly Name (opt)";
            this.tip.SetToolTip(this.label7, "You can assign a friendlier name to this log_line_parser, so you can easier locat" +
        "e it in history");
            // 
            // friendlyNameCtrl
            // 
            this.friendlyNameCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.friendlyNameCtrl.Location = new System.Drawing.Point(860, 9);
            this.friendlyNameCtrl.Name = "friendlyNameCtrl";
            this.friendlyNameCtrl.Size = new System.Drawing.Size(111, 23);
            this.friendlyNameCtrl.TabIndex = 8;
            this.tip.SetToolTip(this.friendlyNameCtrl, "You can assign a friendlier name to this log_line_parser, so you can easier locat" +
        "e it in history");
            this.friendlyNameCtrl.TextChanged += new System.EventHandler(this.friendlyName_TextChanged);
            // 
            // logSyntaxCtrl
            // 
            this.logSyntaxCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logSyntaxCtrl.Location = new System.Drawing.Point(206, 35);
            this.logSyntaxCtrl.Name = "logSyntaxCtrl";
            this.logSyntaxCtrl.Size = new System.Drawing.Size(765, 23);
            this.logSyntaxCtrl.TabIndex = 10;
            this.tip.SetToolTip(this.logSyntaxCtrl, "The syntax of each line");
            this.logSyntaxCtrl.TextChanged += new System.EventHandler(this.logSyntax_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 39);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 15);
            this.label8.TabIndex = 9;
            this.label8.Text = "Log Line Syntax";
            this.tip.SetToolTip(this.label8, "The syntax of each line");
            // 
            // delFilteredView
            // 
            this.delFilteredView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.delFilteredView.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delFilteredView.Location = new System.Drawing.Point(405, 29);
            this.delFilteredView.Name = "delFilteredView";
            this.delFilteredView.Size = new System.Drawing.Size(18, 20);
            this.delFilteredView.TabIndex = 2;
            this.delFilteredView.Text = "-";
            this.tip.SetToolTip(this.delFilteredView, "Delete this View");
            this.delFilteredView.UseVisualStyleBackColor = true;
            this.delFilteredView.Click += new System.EventHandler(this.delFilteredView_Click);
            // 
            // curContextCtrl
            // 
            this.curContextCtrl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.curContextCtrl.FormattingEnabled = true;
            this.curContextCtrl.Location = new System.Drawing.Point(66, 6);
            this.curContextCtrl.Name = "curContextCtrl";
            this.curContextCtrl.Size = new System.Drawing.Size(134, 23);
            this.curContextCtrl.TabIndex = 4;
            this.tip.SetToolTip(this.curContextCtrl, "The template saves the current Filters and the current Views (tabs)");
            this.curContextCtrl.DropDown += new System.EventHandler(this.curContextCtrl_DropDown);
            this.curContextCtrl.SelectedIndexChanged += new System.EventHandler(this.curContextCtrl_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 15);
            this.label5.TabIndex = 3;
            this.label5.Text = "Template";
            this.tip.SetToolTip(this.label5, "\r\n");
            // 
            // addContext
            // 
            this.addContext.Location = new System.Drawing.Point(200, 7);
            this.addContext.Name = "addContext";
            this.addContext.Size = new System.Drawing.Size(23, 22);
            this.addContext.TabIndex = 11;
            this.addContext.Text = "+";
            this.tip.SetToolTip(this.addContext, "Add a template");
            this.addContext.UseVisualStyleBackColor = true;
            this.addContext.Click += new System.EventHandler(this.addContext_Click);
            // 
            // delContext
            // 
            this.delContext.Location = new System.Drawing.Point(221, 7);
            this.delContext.Name = "delContext";
            this.delContext.Size = new System.Drawing.Size(25, 22);
            this.delContext.TabIndex = 12;
            this.delContext.Text = "-";
            this.tip.SetToolTip(this.delContext, "Delete current template");
            this.delContext.UseVisualStyleBackColor = true;
            this.delContext.Click += new System.EventHandler(this.delContext_Click);
            // 
            // synchronizedWithFullLog
            // 
            this.synchronizedWithFullLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.synchronizedWithFullLog.Appearance = System.Windows.Forms.Appearance.Button;
            this.synchronizedWithFullLog.Checked = true;
            this.synchronizedWithFullLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.synchronizedWithFullLog.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.synchronizedWithFullLog.Location = new System.Drawing.Point(474, 29);
            this.synchronizedWithFullLog.Name = "synchronizedWithFullLog";
            this.synchronizedWithFullLog.Size = new System.Drawing.Size(46, 20);
            this.synchronizedWithFullLog.TabIndex = 1;
            this.synchronizedWithFullLog.Text = "<-FL->";
            this.tip.SetToolTip(this.synchronizedWithFullLog, "Synchronized with the Full Log");
            this.synchronizedWithFullLog.UseVisualStyleBackColor = true;
            this.synchronizedWithFullLog.CheckedChanged += new System.EventHandler(this.synchronizedWithFullLog_CheckedChanged);
            // 
            // synchronizeWithExistingLogs
            // 
            this.synchronizeWithExistingLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.synchronizeWithExistingLogs.Appearance = System.Windows.Forms.Appearance.Button;
            this.synchronizeWithExistingLogs.Checked = true;
            this.synchronizeWithExistingLogs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.synchronizeWithExistingLogs.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.synchronizeWithExistingLogs.Location = new System.Drawing.Point(428, 29);
            this.synchronizeWithExistingLogs.Name = "synchronizeWithExistingLogs";
            this.synchronizeWithExistingLogs.Size = new System.Drawing.Size(46, 20);
            this.synchronizeWithExistingLogs.TabIndex = 3;
            this.synchronizeWithExistingLogs.Text = "<-V->";
            this.tip.SetToolTip(this.synchronizeWithExistingLogs, "Synchronized with the rest of the Views\r\n(when you change the line, the other vie" +
        "ws will \r\ngo to the closest line as you)");
            this.synchronizeWithExistingLogs.UseVisualStyleBackColor = true;
            this.synchronizeWithExistingLogs.CheckedChanged += new System.EventHandler(this.synchronizeWithExistingLogs_CheckedChanged);
            // 
            // contextFromClipboard
            // 
            this.contextFromClipboard.Location = new System.Drawing.Point(282, 8);
            this.contextFromClipboard.Name = "contextFromClipboard";
            this.contextFromClipboard.Size = new System.Drawing.Size(53, 22);
            this.contextFromClipboard.TabIndex = 14;
            this.contextFromClipboard.Text = "FromC";
            this.tip.SetToolTip(this.contextFromClipboard, "Paste Context From Clipboard");
            this.contextFromClipboard.UseVisualStyleBackColor = true;
            this.contextFromClipboard.Click += new System.EventHandler(this.contextFromClipboard_Click);
            // 
            // contextToClipboard
            // 
            this.contextToClipboard.Location = new System.Drawing.Point(247, 8);
            this.contextToClipboard.Name = "contextToClipboard";
            this.contextToClipboard.Size = new System.Drawing.Size(38, 22);
            this.contextToClipboard.TabIndex = 13;
            this.contextToClipboard.Text = "ToC";
            this.tip.SetToolTip(this.contextToClipboard, "Copy Context To Clipboard");
            this.contextToClipboard.UseVisualStyleBackColor = true;
            this.contextToClipboard.Click += new System.EventHandler(this.contextToClipboard_Click);
            // 
            // toggleTopmost
            // 
            this.toggleTopmost.BackColor = System.Drawing.Color.Transparent;
            this.toggleTopmost.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleTopmost.Image = global::LogWizard.Properties.Resources.bug;
            this.toggleTopmost.Location = new System.Drawing.Point(0, 0);
            this.toggleTopmost.Name = "toggleTopmost";
            this.toggleTopmost.Size = new System.Drawing.Size(24, 24);
            this.toggleTopmost.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.toggleTopmost.TabIndex = 17;
            this.toggleTopmost.TabStop = false;
            this.tip.SetToolTip(this.toggleTopmost, "Click it to toggle LogWizard\'s TopMost state\r\nRight-Click to show the Toggles Men" +
        "u");
            this.toggleTopmost.Visible = false;
            this.toggleTopmost.Click += new System.EventHandler(this.toggleTopmost_Click);
            this.toggleTopmost.MouseClick += new System.Windows.Forms.MouseEventHandler(this.toggleTopmost_MouseClick);
            // 
            // toggles
            // 
            this.toggles.ContextMenuStrip = this.toggleMenu;
            this.toggles.Location = new System.Drawing.Point(4, 2);
            this.toggles.Name = "toggles";
            this.toggles.Size = new System.Drawing.Size(60, 23);
            this.toggles.TabIndex = 17;
            this.toggles.Text = "Toggles";
            this.tip.SetToolTip(this.toggles, "When Title is not visible, this menu is available by right-clicking the bug on to" +
        "p-left");
            this.toggles.UseVisualStyleBackColor = true;
            this.toggles.Click += new System.EventHandler(this.toggles_Click);
            // 
            // toggleMenu
            // 
            this.toggleMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentViewToolStripMenuItem,
            this.fullLogToolStripMenuItem,
            this.tableHeaderToolStripMenuItem,
            this.tabsToolStripMenuItem,
            this.titleToolStripMenuItem,
            this.statusToolStripMenuItem,
            this.filterPaneToolStripMenuItem,
            this.notesPaneToolStripMenuItem,
            this.sourcePanetopmostToolStripMenuItem,
            this.topmostToolStripMenuItem,
            this.detailsToolStripMenuItem,
            this.toolStripSeparator1,
            this.whatIsThisToolStripMenuItem});
            this.toggleMenu.Name = "toggleMenu";
            this.toggleMenu.Size = new System.Drawing.Size(178, 274);
            // 
            // currentViewToolStripMenuItem
            // 
            this.currentViewToolStripMenuItem.Name = "currentViewToolStripMenuItem";
            this.currentViewToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.currentViewToolStripMenuItem.Text = "Current View";
            this.currentViewToolStripMenuItem.Click += new System.EventHandler(this.currentViewToolStripMenuItem_Click);
            // 
            // fullLogToolStripMenuItem
            // 
            this.fullLogToolStripMenuItem.Name = "fullLogToolStripMenuItem";
            this.fullLogToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.fullLogToolStripMenuItem.Text = "Full Log View";
            this.fullLogToolStripMenuItem.Click += new System.EventHandler(this.fullLogToolStripMenuItem_Click);
            // 
            // tableHeaderToolStripMenuItem
            // 
            this.tableHeaderToolStripMenuItem.Name = "tableHeaderToolStripMenuItem";
            this.tableHeaderToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.tableHeaderToolStripMenuItem.Text = "Table [H]eader";
            this.tableHeaderToolStripMenuItem.Click += new System.EventHandler(this.tableHeaderToolStripMenuItem_Click);
            // 
            // tabsToolStripMenuItem
            // 
            this.tabsToolStripMenuItem.Name = "tabsToolStripMenuItem";
            this.tabsToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.tabsToolStripMenuItem.Text = "[V]iew Tabs";
            this.tabsToolStripMenuItem.Click += new System.EventHandler(this.tabsToolStripMenuItem_Click);
            // 
            // titleToolStripMenuItem
            // 
            this.titleToolStripMenuItem.Name = "titleToolStripMenuItem";
            this.titleToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.titleToolStripMenuItem.Text = "[T]itle";
            this.titleToolStripMenuItem.Click += new System.EventHandler(this.titleToolStripMenuItem_Click);
            // 
            // statusToolStripMenuItem
            // 
            this.statusToolStripMenuItem.Name = "statusToolStripMenuItem";
            this.statusToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.statusToolStripMenuItem.Text = "[S]tatus";
            this.statusToolStripMenuItem.Click += new System.EventHandler(this.statusToolStripMenuItem_Click);
            // 
            // filterPaneToolStripMenuItem
            // 
            this.filterPaneToolStripMenuItem.Name = "filterPaneToolStripMenuItem";
            this.filterPaneToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.filterPaneToolStripMenuItem.Text = "[F]ilter Pane (left)";
            this.filterPaneToolStripMenuItem.Click += new System.EventHandler(this.filterPaneToolStripMenuItem_Click);
            // 
            // notesPaneToolStripMenuItem
            // 
            this.notesPaneToolStripMenuItem.Name = "notesPaneToolStripMenuItem";
            this.notesPaneToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.notesPaneToolStripMenuItem.Text = "[N]otes Pane (left)";
            this.notesPaneToolStripMenuItem.Click += new System.EventHandler(this.notesPaneToolStripMenuItem_Click);
            // 
            // sourcePanetopmostToolStripMenuItem
            // 
            this.sourcePanetopmostToolStripMenuItem.Name = "sourcePanetopmostToolStripMenuItem";
            this.sourcePanetopmostToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.sourcePanetopmostToolStripMenuItem.Text = "s[O]urce Pane (top)";
            this.sourcePanetopmostToolStripMenuItem.Click += new System.EventHandler(this.sourcePanetopmostToolStripMenuItem_Click);
            // 
            // topmostToolStripMenuItem
            // 
            this.topmostToolStripMenuItem.Name = "topmostToolStripMenuItem";
            this.topmostToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.topmostToolStripMenuItem.Text = "Topmost";
            this.topmostToolStripMenuItem.Click += new System.EventHandler(this.topmostToolStripMenuItem_Click);
            // 
            // detailsToolStripMenuItem
            // 
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            this.detailsToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.detailsToolStripMenuItem.Text = "[D]etails";
            this.detailsToolStripMenuItem.Click += new System.EventHandler(this.detailsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(174, 6);
            // 
            // whatIsThisToolStripMenuItem
            // 
            this.whatIsThisToolStripMenuItem.Name = "whatIsThisToolStripMenuItem";
            this.whatIsThisToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.whatIsThisToolStripMenuItem.Text = "What is this?";
            this.whatIsThisToolStripMenuItem.Click += new System.EventHandler(this.whatIsThisToolStripMenuItem_Click);
            // 
            // settingsCtrl
            // 
            this.settingsCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsCtrl.Location = new System.Drawing.Point(1112, 3);
            this.settingsCtrl.Name = "settingsCtrl";
            this.settingsCtrl.Size = new System.Drawing.Size(78, 23);
            this.settingsCtrl.TabIndex = 12;
            this.settingsCtrl.Text = "Preferences";
            this.settingsCtrl.UseVisualStyleBackColor = true;
            this.settingsCtrl.Click += new System.EventHandler(this.settingsCtrl_Click);
            // 
            // about
            // 
            this.about.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.about.Location = new System.Drawing.Point(904, 3);
            this.about.Name = "about";
            this.about.Size = new System.Drawing.Size(68, 23);
            this.about.TabIndex = 13;
            this.about.Text = "About";
            this.about.UseVisualStyleBackColor = true;
            this.about.Click += new System.EventHandler(this.about_Click);
            // 
            // main
            // 
            this.main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main.IsSplitterFixed = true;
            this.main.Location = new System.Drawing.Point(2, 0);
            this.main.Name = "main";
            // 
            // main.Panel1
            // 
            this.main.Panel1.Controls.Add(this.leftPane);
            // 
            // main.Panel2
            // 
            this.main.Panel2.Controls.Add(this.sourceUp);
            this.main.Size = new System.Drawing.Size(1258, 520);
            this.main.SplitterDistance = 273;
            this.main.TabIndex = 4;
            // 
            // leftPane
            // 
            this.leftPane.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.leftPane.Controls.Add(this.filtersTab);
            this.leftPane.Controls.Add(this.tabPage3);
            this.leftPane.Controls.Add(this.tabPage4);
            this.leftPane.Location = new System.Drawing.Point(-1, 4);
            this.leftPane.Name = "leftPane";
            this.leftPane.SelectedIndex = 0;
            this.leftPane.Size = new System.Drawing.Size(277, 513);
            this.leftPane.TabIndex = 13;
            this.leftPane.SizeChanged += new System.EventHandler(this.leftPane_SizeChanged);
            // 
            // filtersTab
            // 
            this.filtersTab.Controls.Add(this.filtCtrl);
            this.filtersTab.Location = new System.Drawing.Point(4, 24);
            this.filtersTab.Name = "filtersTab";
            this.filtersTab.Padding = new System.Windows.Forms.Padding(3);
            this.filtersTab.Size = new System.Drawing.Size(269, 485);
            this.filtersTab.TabIndex = 0;
            this.filtersTab.Text = "Filters";
            this.filtersTab.UseVisualStyleBackColor = true;
            // 
            // filtCtrl
            // 
            this.filtCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filtCtrl.Location = new System.Drawing.Point(3, 2);
            this.filtCtrl.Name = "filtCtrl";
            this.filtCtrl.Size = new System.Drawing.Size(264, 485);
            this.filtCtrl.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listBox1);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(269, 487);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "By Threads / By Context";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(7, 30);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(256, 259);
            this.listBox1.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "Quick Filter";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.notes);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(269, 487);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "Notes / Bookmarks";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // notes
            // 
            this.notes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.notes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notes.Location = new System.Drawing.Point(4, 3);
            this.notes.Margin = new System.Windows.Forms.Padding(4);
            this.notes.Name = "notes";
            this.notes.Size = new System.Drawing.Size(258, 477);
            this.notes.TabIndex = 1;
            // 
            // sourceUp
            // 
            this.sourceUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceUp.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.sourceUp.IsSplitterFixed = true;
            this.sourceUp.Location = new System.Drawing.Point(0, 0);
            this.sourceUp.Name = "sourceUp";
            this.sourceUp.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // sourceUp.Panel1
            // 
            this.sourceUp.Panel1.Controls.Add(this.contextFromClipboard);
            this.sourceUp.Panel1.Controls.Add(this.contextToClipboard);
            this.sourceUp.Panel1.Controls.Add(this.delContext);
            this.sourceUp.Panel1.Controls.Add(this.addContext);
            this.sourceUp.Panel1.Controls.Add(this.logSyntaxCtrl);
            this.sourceUp.Panel1.Controls.Add(this.label8);
            this.sourceUp.Panel1.Controls.Add(this.friendlyNameCtrl);
            this.sourceUp.Panel1.Controls.Add(this.label7);
            this.sourceUp.Panel1.Controls.Add(this.curContextCtrl);
            this.sourceUp.Panel1.Controls.Add(this.label5);
            this.sourceUp.Panel1.Controls.Add(this.sourceNameCtrl);
            this.sourceUp.Panel1.Controls.Add(this.sourceTypeCtrl);
            this.sourceUp.Panel1.Controls.Add(this.label3);
            // 
            // sourceUp.Panel2
            // 
            this.sourceUp.Panel2.Controls.Add(this.filteredLeft);
            this.sourceUp.Size = new System.Drawing.Size(981, 520);
            this.sourceUp.SplitterDistance = 65;
            this.sourceUp.TabIndex = 0;
            // 
            // sourceNameCtrl
            // 
            this.sourceNameCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceNameCtrl.Location = new System.Drawing.Point(486, 6);
            this.sourceNameCtrl.Name = "sourceNameCtrl";
            this.sourceNameCtrl.Size = new System.Drawing.Size(255, 23);
            this.sourceNameCtrl.TabIndex = 2;
            this.sourceNameCtrl.TextChanged += new System.EventHandler(this.sourceName_TextChanged);
            // 
            // sourceTypeCtrl
            // 
            this.sourceTypeCtrl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sourceTypeCtrl.FormattingEnabled = true;
            this.sourceTypeCtrl.Items.AddRange(new object[] {
            "File",
            "Shared Memory",
            "Debug Window"});
            this.sourceTypeCtrl.Location = new System.Drawing.Point(393, 6);
            this.sourceTypeCtrl.Name = "sourceTypeCtrl";
            this.sourceTypeCtrl.Size = new System.Drawing.Size(87, 23);
            this.sourceTypeCtrl.TabIndex = 1;
            this.sourceTypeCtrl.SelectedIndexChanged += new System.EventHandler(this.sourceType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(347, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Source";
            // 
            // filteredLeft
            // 
            this.filteredLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filteredLeft.Location = new System.Drawing.Point(0, 0);
            this.filteredLeft.Name = "filteredLeft";
            // 
            // filteredLeft.Panel1
            // 
            this.filteredLeft.Panel1.Controls.Add(this.synchronizeWithExistingLogs);
            this.filteredLeft.Panel1.Controls.Add(this.synchronizedWithFullLog);
            this.filteredLeft.Panel1.Controls.Add(this.delFilteredView);
            this.filteredLeft.Panel1.Controls.Add(this.newFilteredView);
            this.filteredLeft.Panel1.Controls.Add(this.viewsTab);
            this.filteredLeft.Size = new System.Drawing.Size(981, 451);
            this.filteredLeft.SplitterDistance = 558;
            this.filteredLeft.TabIndex = 0;
            this.filteredLeft.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.filteredLeft_SplitterMoved);
            // 
            // viewsTab
            // 
            this.viewsTab.AllowDrop = true;
            this.viewsTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewsTab.Controls.Add(this.tabPage1);
            this.viewsTab.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewsTab.Location = new System.Drawing.Point(0, 3);
            this.viewsTab.Name = "viewsTab";
            this.viewsTab.SelectedIndex = 0;
            this.viewsTab.Size = new System.Drawing.Size(553, 449);
            this.viewsTab.TabIndex = 0;
            this.viewsTab.SelectedIndexChanged += new System.EventHandler(this.viewsTab_SelectedIndexChanged);
            this.viewsTab.DragDrop += new System.Windows.Forms.DragEventHandler(this.filteredViews_DragDrop);
            this.viewsTab.DragEnter += new System.Windows.Forms.DragEventHandler(this.filteredViews_DragEnter);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dropHere);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(545, 421);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "View";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dropHere
            // 
            this.dropHere.AllowDrop = true;
            this.dropHere.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dropHere.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dropHere.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dropHere.Location = new System.Drawing.Point(3, 3);
            this.dropHere.Name = "dropHere";
            this.dropHere.Size = new System.Drawing.Size(534, 421);
            this.dropHere.TabIndex = 0;
            this.dropHere.Text = "Drop it Like it\'s Hot!\r\nJust drop a file here, and get to work!\r\n";
            this.dropHere.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.dropHere.DragDrop += new System.Windows.Forms.DragEventHandler(this.dropHere_DragDrop);
            this.dropHere.DragEnter += new System.Windows.Forms.DragEventHandler(this.dropHere_DragEnter);
            // 
            // refreshFilter
            // 
            this.refreshFilter.Location = new System.Drawing.Point(68, 2);
            this.refreshFilter.Name = "refreshFilter";
            this.refreshFilter.Size = new System.Drawing.Size(60, 23);
            this.refreshFilter.TabIndex = 11;
            this.refreshFilter.Text = "Refresh";
            this.refreshFilter.UseVisualStyleBackColor = true;
            this.refreshFilter.Click += new System.EventHandler(this.refreshFilter_Click);
            // 
            // refresh
            // 
            this.refresh.Enabled = true;
            this.refresh.Interval = 500;
            this.refresh.Tick += new System.EventHandler(this.refresh_Tick);
            // 
            // postFocus
            // 
            this.postFocus.Interval = 1;
            this.postFocus.Tick += new System.EventHandler(this.postFocus_Tick);
            // 
            // monitor
            // 
            this.monitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.monitor.Location = new System.Drawing.Point(1045, 3);
            this.monitor.Name = "monitor";
            this.monitor.Size = new System.Drawing.Size(64, 23);
            this.monitor.TabIndex = 14;
            this.monitor.Text = "Monitor";
            this.monitor.UseVisualStyleBackColor = true;
            this.monitor.Click += new System.EventHandler(this.monitor_Click);
            // 
            // lower
            // 
            this.lower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lower.Controls.Add(this.toggles);
            this.lower.Controls.Add(this.export);
            this.lower.Controls.Add(this.hotkeys);
            this.lower.Controls.Add(this.logHistory);
            this.lower.Controls.Add(this.newView);
            this.lower.Controls.Add(this.monitor);
            this.lower.Controls.Add(this.about);
            this.lower.Controls.Add(this.settingsCtrl);
            this.lower.Controls.Add(this.refreshFilter);
            this.lower.Location = new System.Drawing.Point(0, 520);
            this.lower.Name = "lower";
            this.lower.Size = new System.Drawing.Size(1261, 27);
            this.lower.TabIndex = 15;
            // 
            // export
            // 
            this.export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.export.ContextMenuStrip = this.exportMenu;
            this.export.Location = new System.Drawing.Point(974, 3);
            this.export.Name = "export";
            this.export.Size = new System.Drawing.Size(68, 23);
            this.export.TabIndex = 16;
            this.export.Text = "Export";
            this.export.UseVisualStyleBackColor = true;
            this.export.Click += new System.EventHandler(this.export_Click);
            // 
            // exportMenu
            // 
            this.exportMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportLogNotestoLogWizardFileToolStripMenuItem,
            this.exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem,
            this.exportNotestotxtAndhtmlFilesToolStripMenuItem});
            this.exportMenu.Name = "exportMenu";
            this.exportMenu.Size = new System.Drawing.Size(298, 70);
            // 
            // exportLogNotestoLogWizardFileToolStripMenuItem
            // 
            this.exportLogNotestoLogWizardFileToolStripMenuItem.Name = "exportLogNotestoLogWizardFileToolStripMenuItem";
            this.exportLogNotestoLogWizardFileToolStripMenuItem.Size = new System.Drawing.Size(297, 22);
            this.exportLogNotestoLogWizardFileToolStripMenuItem.Text = "Export Log + Notes (to .LogWizard file)";
            this.exportLogNotestoLogWizardFileToolStripMenuItem.Click += new System.EventHandler(this.exportLogNotestoLogWizardFileToolStripMenuItem_Click);
            // 
            // exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem
            // 
            this.exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem.Name = "exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem";
            this.exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem.Size = new System.Drawing.Size(297, 22);
            this.exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem.Text = "Export Current View (to .txt and .html files)";
            this.exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem.Click += new System.EventHandler(this.exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem_Click);
            // 
            // exportNotestotxtAndhtmlFilesToolStripMenuItem
            // 
            this.exportNotestotxtAndhtmlFilesToolStripMenuItem.Name = "exportNotestotxtAndhtmlFilesToolStripMenuItem";
            this.exportNotestotxtAndhtmlFilesToolStripMenuItem.Size = new System.Drawing.Size(297, 22);
            this.exportNotestotxtAndhtmlFilesToolStripMenuItem.Text = "Export Notes (to .txt and .html files)";
            this.exportNotestotxtAndhtmlFilesToolStripMenuItem.Click += new System.EventHandler(this.exportNotestotxtAndhtmlFilesToolStripMenuItem_Click);
            // 
            // hotkeys
            // 
            this.hotkeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.hotkeys.AutoSize = true;
            this.hotkeys.Location = new System.Drawing.Point(848, 8);
            this.hotkeys.Name = "hotkeys";
            this.hotkeys.Size = new System.Drawing.Size(50, 15);
            this.hotkeys.TabIndex = 15;
            this.hotkeys.TabStop = true;
            this.hotkeys.Text = "Hotkeys";
            this.hotkeys.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.hotkeys_LinkClicked);
            // 
            // status
            // 
            this.status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.status.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status.Location = new System.Drawing.Point(-4, 549);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(1264, 19);
            this.status.TabIndex = 18;
            this.status.Text = " ";
            // 
            // saveTimer
            // 
            this.saveTimer.Enabled = true;
            this.saveTimer.Interval = 15000;
            this.saveTimer.Tick += new System.EventHandler(this.saveTimer_Tick);
            // 
            // log_wizard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1255, 568);
            this.Controls.Add(this.status);
            this.Controls.Add(this.toggleTopmost);
            this.Controls.Add(this.main);
            this.Controls.Add(this.lower);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "log_wizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Log Wizard";
            this.Activated += new System.EventHandler(this.log_wizard_Activated);
            this.Deactivate += new System.EventHandler(this.log_wizard_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogWizard_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogNinja_FormClosed);
            this.LocationChanged += new System.EventHandler(this.log_wizard_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.log_wizard_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.toggleTopmost)).EndInit();
            this.toggleMenu.ResumeLayout(false);
            this.main.Panel1.ResumeLayout(false);
            this.main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.main)).EndInit();
            this.main.ResumeLayout(false);
            this.leftPane.ResumeLayout(false);
            this.filtersTab.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.sourceUp.Panel1.ResumeLayout(false);
            this.sourceUp.Panel1.PerformLayout();
            this.sourceUp.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sourceUp)).EndInit();
            this.sourceUp.ResumeLayout(false);
            this.filteredLeft.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.filteredLeft)).EndInit();
            this.filteredLeft.ResumeLayout(false);
            this.viewsTab.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.lower.ResumeLayout(false);
            this.lower.PerformLayout();
            this.exportMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.SplitContainer main;
        private System.Windows.Forms.SplitContainer sourceUp;
        private System.Windows.Forms.TextBox sourceNameCtrl;
        private System.Windows.Forms.ComboBox sourceTypeCtrl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox curContextCtrl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button newView;
        private System.Windows.Forms.ComboBox logHistory;
        private System.Windows.Forms.SplitContainer filteredLeft;
        private System.Windows.Forms.TextBox friendlyNameCtrl;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button newFilteredView;
        private System.Windows.Forms.TabControl viewsTab;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox logSyntaxCtrl;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label dropHere;
        private System.Windows.Forms.Timer refresh;
        private System.Windows.Forms.Button delFilteredView;
        private System.Windows.Forms.Button delContext;
        private System.Windows.Forms.Button addContext;
        private System.Windows.Forms.Button refreshFilter;
        private System.Windows.Forms.Button settingsCtrl;
        private System.Windows.Forms.Button about;
        private System.Windows.Forms.Timer postFocus;
        private System.Windows.Forms.CheckBox synchronizedWithFullLog;
        private System.Windows.Forms.TabControl leftPane;
        private System.Windows.Forms.TabPage filtersTab;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox synchronizeWithExistingLogs;
        private System.Windows.Forms.Button contextFromClipboard;
        private System.Windows.Forms.Button contextToClipboard;
        private System.Windows.Forms.Button monitor;
        private System.Windows.Forms.Panel lower;
        private System.Windows.Forms.PictureBox toggleTopmost;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Timer saveTimer;
        private System.Windows.Forms.LinkLabel hotkeys;
        private lw_common.ui.filter_ctrl filtCtrl;
        private lw_common.ui.note_ctrl notes;
        private System.Windows.Forms.Button export;
        private System.Windows.Forms.ContextMenuStrip exportMenu;
        private System.Windows.Forms.ToolStripMenuItem exportLogNotestoLogWizardFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportNotestotxtAndhtmlFilesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip toggleMenu;
        private System.Windows.Forms.ToolStripMenuItem currentViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tableHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tabsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem titleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterPaneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notesPaneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sourcePanetopmostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topmostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
        private System.Windows.Forms.Button toggles;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem whatIsThisToolStripMenuItem;
    }
}

