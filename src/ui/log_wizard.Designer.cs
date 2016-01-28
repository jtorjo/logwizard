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
            this.logHistory = new System.Windows.Forms.ComboBox();
            this.newFilteredView = new System.Windows.Forms.Button();
            this.newViewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createACopyOfTheExistingViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createANewViewFromScratchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripShowAllLines = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripExtraFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.whatIsThisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.editSettings = new System.Windows.Forms.Button();
            this.splitDescription = new System.Windows.Forms.SplitContainer();
            this.filteredLeft = new System.Windows.Forms.SplitContainer();
            this.viewsTab = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dropHere = new System.Windows.Forms.Label();
            this.description = new lw_common.ui.description_ctrl();
            this.refresh = new System.Windows.Forms.Timer(this.components);
            this.lower = new System.Windows.Forms.Panel();
            this.whatsup = new lw_common.ui.animated_button();
            this.status = new lw_common.ui.status_ctrl();
            this.exportMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportLogNotestoLogWizardFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCurrentViewToCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportNotestotxtAndhtmlFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTimer = new System.Windows.Forms.Timer(this.components);
            this.whatsupOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.whatsupNew = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.whatsupToggles = new System.Windows.Forms.ToolStripMenuItem();
            this.whatsupPreferences = new System.Windows.Forms.ToolStripMenuItem();
            this.historyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hotkeysHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.whatupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshAddViewButtons = new System.Windows.Forms.Timer(this.components);
            this.newViewMenu.SuspendLayout();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitDescription)).BeginInit();
            this.splitDescription.Panel1.SuspendLayout();
            this.splitDescription.Panel2.SuspendLayout();
            this.splitDescription.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.filteredLeft)).BeginInit();
            this.filteredLeft.Panel1.SuspendLayout();
            this.filteredLeft.SuspendLayout();
            this.viewsTab.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.lower.SuspendLayout();
            this.exportMenu.SuspendLayout();
            this.whatupMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tip
            // 
            this.tip.AutoPopDelay = 32000;
            this.tip.InitialDelay = 500;
            this.tip.ReshowDelay = 100;
            // 
            // logHistory
            // 
            this.logHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logHistory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.logHistory.FormattingEnabled = true;
            this.logHistory.Location = new System.Drawing.Point(285, 3);
            this.logHistory.Name = "logHistory";
            this.logHistory.Size = new System.Drawing.Size(892, 23);
            this.logHistory.TabIndex = 7;
            this.tip.SetToolTip(this.logHistory, "History - just select any of the previous logs, and they instantly load");
            this.logHistory.Visible = false;
            this.logHistory.DropDown += new System.EventHandler(this.logHistory_DropDown);
            this.logHistory.SelectedIndexChanged += new System.EventHandler(this.logHistory_SelectedIndexChanged);
            this.logHistory.DropDownClosed += new System.EventHandler(this.logHistory_DropDownClosed);
            // 
            // newFilteredView
            // 
            this.newFilteredView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newFilteredView.ContextMenuStrip = this.newViewMenu;
            this.newFilteredView.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFilteredView.Location = new System.Drawing.Point(304, 29);
            this.newFilteredView.Name = "newFilteredView";
            this.newFilteredView.Size = new System.Drawing.Size(18, 20);
            this.newFilteredView.TabIndex = 1;
            this.newFilteredView.Text = "+";
            this.tip.SetToolTip(this.newFilteredView, "New Filtered View of the same Log");
            this.newFilteredView.UseVisualStyleBackColor = true;
            this.newFilteredView.Click += new System.EventHandler(this.new_view_Click);
            // 
            // newViewMenu
            // 
            this.newViewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createACopyOfTheExistingViewToolStripMenuItem,
            this.createANewViewFromScratchToolStripMenuItem});
            this.newViewMenu.Name = "newViewMenu";
            this.newViewMenu.Size = new System.Drawing.Size(256, 48);
            // 
            // createACopyOfTheExistingViewToolStripMenuItem
            // 
            this.createACopyOfTheExistingViewToolStripMenuItem.Name = "createACopyOfTheExistingViewToolStripMenuItem";
            this.createACopyOfTheExistingViewToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.createACopyOfTheExistingViewToolStripMenuItem.Text = "Create A Copy of the Existing View";
            this.createACopyOfTheExistingViewToolStripMenuItem.Click += new System.EventHandler(this.createACopyOfTheExistingViewToolStripMenuItem_Click);
            // 
            // createANewViewFromScratchToolStripMenuItem
            // 
            this.createANewViewFromScratchToolStripMenuItem.Name = "createANewViewFromScratchToolStripMenuItem";
            this.createANewViewFromScratchToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.createANewViewFromScratchToolStripMenuItem.Text = "Create A New View From Scratch";
            this.createANewViewFromScratchToolStripMenuItem.Click += new System.EventHandler(this.createANewViewFromScratchToolStripMenuItem_Click);
            // 
            // delFilteredView
            // 
            this.delFilteredView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.delFilteredView.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delFilteredView.Location = new System.Drawing.Point(322, 29);
            this.delFilteredView.Name = "delFilteredView";
            this.delFilteredView.Size = new System.Drawing.Size(18, 20);
            this.delFilteredView.TabIndex = 2;
            this.delFilteredView.Text = "-";
            this.tip.SetToolTip(this.delFilteredView, "Delete this View");
            this.delFilteredView.UseVisualStyleBackColor = true;
            this.delFilteredView.Click += new System.EventHandler(this.delView_Click);
            // 
            // curContextCtrl
            // 
            this.curContextCtrl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.curContextCtrl.FormattingEnabled = true;
            this.curContextCtrl.Location = new System.Drawing.Point(75, 9);
            this.curContextCtrl.Name = "curContextCtrl";
            this.curContextCtrl.Size = new System.Drawing.Size(190, 23);
            this.curContextCtrl.TabIndex = 4;
            this.tip.SetToolTip(this.curContextCtrl, "The template saves the current Filters and the current Views (tabs)");
            this.curContextCtrl.DropDown += new System.EventHandler(this.curContextCtrl_DropDown);
            this.curContextCtrl.SelectedIndexChanged += new System.EventHandler(this.curContextCtrl_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(5, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 19);
            this.label5.TabIndex = 3;
            this.label5.Text = "Template";
            this.tip.SetToolTip(this.label5, "\r\n");
            // 
            // addContext
            // 
            this.addContext.Location = new System.Drawing.Point(271, 10);
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
            this.delContext.Location = new System.Drawing.Point(292, 10);
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
            this.synchronizedWithFullLog.Location = new System.Drawing.Point(391, 29);
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
            this.synchronizeWithExistingLogs.Location = new System.Drawing.Point(345, 29);
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
            this.contextFromClipboard.Location = new System.Drawing.Point(353, 11);
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
            this.contextToClipboard.Location = new System.Drawing.Point(318, 11);
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
            this.toolStripSeparator2,
            this.toolStripShowAllLines,
            this.toolStripExtraFilter,
            this.toolStripSeparator1,
            this.whatIsThisToolStripMenuItem});
            this.toggleMenu.Name = "toggleMenu";
            this.toggleMenu.Size = new System.Drawing.Size(178, 324);
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
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(174, 6);
            // 
            // toolStripShowAllLines
            // 
            this.toolStripShowAllLines.Name = "toolStripShowAllLines";
            this.toolStripShowAllLines.Size = new System.Drawing.Size(177, 22);
            this.toolStripShowAllLines.Text = "Show All Lines";
            this.toolStripShowAllLines.Click += new System.EventHandler(this.toolStripShowAllLines_Click);
            // 
            // toolStripExtraFilter
            // 
            this.toolStripExtraFilter.Name = "toolStripExtraFilter";
            this.toolStripExtraFilter.Size = new System.Drawing.Size(177, 22);
            this.toolStripExtraFilter.Text = "Extra Filter:";
            this.toolStripExtraFilter.Click += new System.EventHandler(this.toolStripExtraFilter_Click);
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
            // main
            // 
            this.main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.main.Location = new System.Drawing.Point(2, 0);
            this.main.Name = "main";
            // 
            // main.Panel1
            // 
            this.main.Panel1.Controls.Add(this.leftPane);
            this.main.Panel1MinSize = 100;
            // 
            // main.Panel2
            // 
            this.main.Panel2.Controls.Add(this.sourceUp);
            this.main.Panel2MinSize = 100;
            this.main.Size = new System.Drawing.Size(1253, 494);
            this.main.SplitterDistance = 273;
            this.main.SplitterWidth = 6;
            this.main.TabIndex = 4;
            this.main.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.main_SplitterMoved);
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
            this.leftPane.Size = new System.Drawing.Size(277, 487);
            this.leftPane.TabIndex = 13;
            this.leftPane.SizeChanged += new System.EventHandler(this.leftPane_SizeChanged);
            // 
            // filtersTab
            // 
            this.filtersTab.Controls.Add(this.filtCtrl);
            this.filtersTab.Location = new System.Drawing.Point(4, 24);
            this.filtersTab.Name = "filtersTab";
            this.filtersTab.Padding = new System.Windows.Forms.Padding(3);
            this.filtersTab.Size = new System.Drawing.Size(269, 459);
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
            this.filtCtrl.Size = new System.Drawing.Size(264, 459);
            this.filtCtrl.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listBox1);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(269, 461);
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
            this.tabPage4.Size = new System.Drawing.Size(269, 461);
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
            this.notes.Size = new System.Drawing.Size(258, 451);
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
            this.sourceUp.Panel1.Controls.Add(this.editSettings);
            this.sourceUp.Panel1.Controls.Add(this.contextFromClipboard);
            this.sourceUp.Panel1.Controls.Add(this.contextToClipboard);
            this.sourceUp.Panel1.Controls.Add(this.delContext);
            this.sourceUp.Panel1.Controls.Add(this.addContext);
            this.sourceUp.Panel1.Controls.Add(this.curContextCtrl);
            this.sourceUp.Panel1.Controls.Add(this.label5);
            // 
            // sourceUp.Panel2
            // 
            this.sourceUp.Panel2.Controls.Add(this.splitDescription);
            this.sourceUp.Size = new System.Drawing.Size(974, 494);
            this.sourceUp.SplitterDistance = 40;
            this.sourceUp.TabIndex = 0;
            // 
            // editSettings
            // 
            this.editSettings.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editSettings.Location = new System.Drawing.Point(426, 8);
            this.editSettings.Name = "editSettings";
            this.editSettings.Size = new System.Drawing.Size(125, 25);
            this.editSettings.TabIndex = 17;
            this.editSettings.Text = "Edit Log Settings";
            this.editSettings.UseVisualStyleBackColor = true;
            this.editSettings.Click += new System.EventHandler(this.editSettings_Click);
            // 
            // splitDescription
            // 
            this.splitDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitDescription.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitDescription.Location = new System.Drawing.Point(0, 0);
            this.splitDescription.Name = "splitDescription";
            this.splitDescription.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitDescription.Panel1
            // 
            this.splitDescription.Panel1.Controls.Add(this.filteredLeft);
            this.splitDescription.Panel1MinSize = 100;
            // 
            // splitDescription.Panel2
            // 
            this.splitDescription.Panel2.Controls.Add(this.description);
            this.splitDescription.Panel2MinSize = 100;
            this.splitDescription.Size = new System.Drawing.Size(974, 450);
            this.splitDescription.SplitterDistance = 314;
            this.splitDescription.SplitterWidth = 6;
            this.splitDescription.TabIndex = 18;
            this.splitDescription.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitDescription_SplitterMoved);
            // 
            // filteredLeft
            // 
            this.filteredLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filteredLeft.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
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
            this.filteredLeft.Panel1MinSize = 100;
            this.filteredLeft.Panel2MinSize = 100;
            this.filteredLeft.Size = new System.Drawing.Size(974, 314);
            this.filteredLeft.SplitterDistance = 475;
            this.filteredLeft.SplitterWidth = 6;
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
            this.viewsTab.Size = new System.Drawing.Size(470, 312);
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
            this.tabPage1.Size = new System.Drawing.Size(462, 284);
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
            this.dropHere.Size = new System.Drawing.Size(451, 284);
            this.dropHere.TabIndex = 0;
            this.dropHere.Text = "Drop a file here, and get to work!\r\n";
            this.dropHere.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.dropHere.DragDrop += new System.Windows.Forms.DragEventHandler(this.dropHere_DragDrop);
            this.dropHere.DragEnter += new System.Windows.Forms.DragEventHandler(this.dropHere_DragEnter);
            // 
            // description
            // 
            this.description.Dock = System.Windows.Forms.DockStyle.Fill;
            this.description.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.description.Location = new System.Drawing.Point(0, 0);
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(974, 130);
            this.description.TabIndex = 0;
            // 
            // refresh
            // 
            this.refresh.Enabled = true;
            this.refresh.Interval = 500;
            this.refresh.Tick += new System.EventHandler(this.refresh_Tick);
            // 
            // lower
            // 
            this.lower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lower.Controls.Add(this.logHistory);
            this.lower.Controls.Add(this.whatsup);
            this.lower.Controls.Add(this.status);
            this.lower.Location = new System.Drawing.Point(0, 495);
            this.lower.Name = "lower";
            this.lower.Size = new System.Drawing.Size(1261, 27);
            this.lower.TabIndex = 15;
            // 
            // whatsup
            // 
            this.whatsup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.whatsup.animate = false;
            this.whatsup.animate_count = 5;
            this.whatsup.animate_interval_ms = 5000;
            this.whatsup.animate_speed_ms = 100;
            this.whatsup.BackColor = System.Drawing.Color.Transparent;
            this.whatsup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.whatsup.Location = new System.Drawing.Point(1183, 2);
            this.whatsup.Name = "whatsup";
            this.whatsup.Size = new System.Drawing.Size(68, 24);
            this.whatsup.TabIndex = 19;
            this.whatsup.Text = "Actions";
            this.whatsup.UseVisualStyleBackColor = false;
            this.whatsup.Click += new System.EventHandler(this.whatsup_Click);
            // 
            // status
            // 
            this.status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.status.Location = new System.Drawing.Point(3, 2);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(1132, 24);
            this.status.TabIndex = 18;
            // 
            // exportMenu
            // 
            this.exportMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportLogNotestoLogWizardFileToolStripMenuItem,
            this.exportCurrentViewtotxtAndhtmlFilesToolStripMenuItem,
            this.exportCurrentViewToCSVToolStripMenuItem,
            this.exportNotestotxtAndhtmlFilesToolStripMenuItem});
            this.exportMenu.Name = "exportMenu";
            this.exportMenu.Size = new System.Drawing.Size(298, 92);
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
            // exportCurrentViewToCSVToolStripMenuItem
            // 
            this.exportCurrentViewToCSVToolStripMenuItem.Name = "exportCurrentViewToCSVToolStripMenuItem";
            this.exportCurrentViewToCSVToolStripMenuItem.Size = new System.Drawing.Size(297, 22);
            this.exportCurrentViewToCSVToolStripMenuItem.Text = "Export Current View to CSV";
            this.exportCurrentViewToCSVToolStripMenuItem.Click += new System.EventHandler(this.exportCurrentViewToCSVToolStripMenuItem_Click);
            // 
            // exportNotestotxtAndhtmlFilesToolStripMenuItem
            // 
            this.exportNotestotxtAndhtmlFilesToolStripMenuItem.Name = "exportNotestotxtAndhtmlFilesToolStripMenuItem";
            this.exportNotestotxtAndhtmlFilesToolStripMenuItem.Size = new System.Drawing.Size(297, 22);
            this.exportNotestotxtAndhtmlFilesToolStripMenuItem.Text = "Export Notes (to .txt and .html files)";
            this.exportNotestotxtAndhtmlFilesToolStripMenuItem.Click += new System.EventHandler(this.exportNotestotxtAndhtmlFilesToolStripMenuItem_Click);
            // 
            // saveTimer
            // 
            this.saveTimer.Enabled = true;
            this.saveTimer.Interval = 15000;
            this.saveTimer.Tick += new System.EventHandler(this.saveTimer_Tick);
            // 
            // whatsupOpen
            // 
            this.whatsupOpen.Name = "whatsupOpen";
            this.whatsupOpen.Size = new System.Drawing.Size(204, 22);
            this.whatsupOpen.Text = "Open Log";
            this.whatsupOpen.Click += new System.EventHandler(this.whatsupOpen_Click);
            // 
            // whatsupNew
            // 
            this.whatsupNew.Name = "whatsupNew";
            this.whatsupNew.Size = new System.Drawing.Size(204, 22);
            this.whatsupNew.Text = "New LogWizard Window";
            this.whatsupNew.Click += new System.EventHandler(this.whatsupNew_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(201, 6);
            // 
            // whatsupToggles
            // 
            this.whatsupToggles.Name = "whatsupToggles";
            this.whatsupToggles.Size = new System.Drawing.Size(204, 22);
            this.whatsupToggles.Text = "Show/Hide Information";
            this.whatsupToggles.Click += new System.EventHandler(this.whatsupToggles_Click);
            // 
            // whatsupPreferences
            // 
            this.whatsupPreferences.Name = "whatsupPreferences";
            this.whatsupPreferences.Size = new System.Drawing.Size(204, 22);
            this.whatsupPreferences.Text = "Preferences";
            this.whatsupPreferences.Click += new System.EventHandler(this.whatsupPreferences_Click);
            // 
            // historyToolStripMenuItem
            // 
            this.historyToolStripMenuItem.Name = "historyToolStripMenuItem";
            this.historyToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.historyToolStripMenuItem.Text = "Show Log History";
            this.historyToolStripMenuItem.Click += new System.EventHandler(this.historyToolStripMenuItem_Click);
            // 
            // hotkeysHelpToolStripMenuItem
            // 
            this.hotkeysHelpToolStripMenuItem.Name = "hotkeysHelpToolStripMenuItem";
            this.hotkeysHelpToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.hotkeysHelpToolStripMenuItem.Text = "Hotkeys Help";
            this.hotkeysHelpToolStripMenuItem.Click += new System.EventHandler(this.hotkeysHelpToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(201, 6);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // monitorToolStripMenuItem
            // 
            this.monitorToolStripMenuItem.Name = "monitorToolStripMenuItem";
            this.monitorToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.monitorToolStripMenuItem.Text = "Monitor";
            this.monitorToolStripMenuItem.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(201, 6);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(204, 22);
            this.aboutToolStripMenuItem1.Text = "About LogWizard";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // whatupMenu
            // 
            this.whatupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.whatsupOpen,
            this.whatsupNew,
            this.toolStripSeparator4,
            this.whatsupToggles,
            this.whatsupPreferences,
            this.historyToolStripMenuItem,
            this.toolStripSeparator3,
            this.refreshToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.monitorToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.hotkeysHelpToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.whatupMenu.Name = "whatupMenu";
            this.whatupMenu.Size = new System.Drawing.Size(205, 242);
            // 
            // refreshAddViewButtons
            // 
            this.refreshAddViewButtons.Enabled = true;
            this.refreshAddViewButtons.Interval = 250;
            this.refreshAddViewButtons.Tick += new System.EventHandler(this.refreshAddViewButtons_Tick);
            // 
            // log_wizard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1255, 522);
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
            this.newViewMenu.ResumeLayout(false);
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
            this.splitDescription.Panel1.ResumeLayout(false);
            this.splitDescription.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitDescription)).EndInit();
            this.splitDescription.ResumeLayout(false);
            this.filteredLeft.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.filteredLeft)).EndInit();
            this.filteredLeft.ResumeLayout(false);
            this.viewsTab.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.lower.ResumeLayout(false);
            this.exportMenu.ResumeLayout(false);
            this.whatupMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.SplitContainer main;
        private System.Windows.Forms.SplitContainer sourceUp;
        private System.Windows.Forms.ComboBox curContextCtrl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox logHistory;
        private System.Windows.Forms.SplitContainer filteredLeft;
        private System.Windows.Forms.Button newFilteredView;
        private System.Windows.Forms.TabControl viewsTab;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label dropHere;
        private System.Windows.Forms.Timer refresh;
        private System.Windows.Forms.Button delFilteredView;
        private System.Windows.Forms.Button delContext;
        private System.Windows.Forms.Button addContext;
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
        private System.Windows.Forms.Panel lower;
        private System.Windows.Forms.PictureBox toggleTopmost;
        private System.Windows.Forms.Timer saveTimer;
        private lw_common.ui.filter_ctrl filtCtrl;
        private lw_common.ui.note_ctrl notes;
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem whatIsThisToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip newViewMenu;
        private System.Windows.Forms.ToolStripMenuItem createACopyOfTheExistingViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createANewViewFromScratchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripShowAllLines;
        private System.Windows.Forms.ToolStripMenuItem toolStripExtraFilter;
        private lw_common.ui.status_ctrl status;
        private lw_common.ui.animated_button whatsup;
        private System.Windows.Forms.ToolStripMenuItem whatsupOpen;
        private System.Windows.Forms.ToolStripMenuItem whatsupNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem whatsupToggles;
        private System.Windows.Forms.ToolStripMenuItem whatsupPreferences;
        private System.Windows.Forms.ToolStripMenuItem historyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hotkeysHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem monitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip whatupMenu;
        private System.Windows.Forms.Timer refreshAddViewButtons;
        private System.Windows.Forms.Button editSettings;
        private System.Windows.Forms.SplitContainer splitDescription;
        private lw_common.ui.description_ctrl description;
        private System.Windows.Forms.ToolStripMenuItem exportCurrentViewToCSVToolStripMenuItem;
    }
}

