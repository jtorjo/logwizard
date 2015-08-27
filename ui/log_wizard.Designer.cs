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
            this.delFilter = new System.Windows.Forms.Button();
            this.addFilter = new System.Windows.Forms.Button();
            this.filterCtrl = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.filterCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.filterContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.curFilterCtrl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.newView = new System.Windows.Forms.Button();
            this.logHistory = new System.Windows.Forms.ComboBox();
            this.newFilteredView = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.friendlyNameCtrl = new System.Windows.Forms.TextBox();
            this.toggleFilters = new System.Windows.Forms.Button();
            this.toggleSource = new System.Windows.Forms.Button();
            this.toggleFullLog = new System.Windows.Forms.Button();
            this.logSyntaxCtrl = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.delFilteredView = new System.Windows.Forms.Button();
            this.curContextCtrl = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.addContext = new System.Windows.Forms.Button();
            this.delContext = new System.Windows.Forms.Button();
            this.synchronizedWithFullLog = new System.Windows.Forms.CheckBox();
            this.synchronizeWithExistingLogs = new System.Windows.Forms.CheckBox();
            this.viewFromClipboard = new System.Windows.Forms.Button();
            this.viewToClipboard = new System.Windows.Forms.Button();
            this.contextFromClipboard = new System.Windows.Forms.Button();
            this.contextToClipboard = new System.Windows.Forms.Button();
            this.settingsCtrl = new System.Windows.Forms.Button();
            this.about = new System.Windows.Forms.Button();
            this.main = new System.Windows.Forms.SplitContainer();
            this.leftPane = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.sourceUp = new System.Windows.Forms.SplitContainer();
            this.sourceNameCtrl = new System.Windows.Forms.TextBox();
            this.sourceTypeCtrl = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.filteredLeft = new System.Windows.Forms.SplitContainer();
            this.viewsTab = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dropHere = new System.Windows.Forms.Label();
            this.refreshFilter = new System.Windows.Forms.Button();
            this.tipsHotkeys = new System.Windows.Forms.LinkLabel();
            this.refresh = new System.Windows.Forms.Timer(this.components);
            this.focusOnFilterCtrl = new System.Windows.Forms.Timer(this.components);
            this.postFocus = new System.Windows.Forms.Timer(this.components);
            this.monitor = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.filterCtrl)).BeginInit();
            this.filterContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.main)).BeginInit();
            this.main.Panel1.SuspendLayout();
            this.main.Panel2.SuspendLayout();
            this.main.SuspendLayout();
            this.leftPane.SuspendLayout();
            this.tabPage2.SuspendLayout();
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
            this.SuspendLayout();
            // 
            // delFilter
            // 
            this.delFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.delFilter.Location = new System.Drawing.Point(239, 471);
            this.delFilter.Name = "delFilter";
            this.delFilter.Size = new System.Drawing.Size(24, 23);
            this.delFilter.TabIndex = 10;
            this.delFilter.Text = "-";
            this.tip.SetToolTip(this.delFilter, "Delete Filter");
            this.delFilter.UseVisualStyleBackColor = true;
            this.delFilter.Click += new System.EventHandler(this.delFilter_Click);
            // 
            // addFilter
            // 
            this.addFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addFilter.Location = new System.Drawing.Point(211, 471);
            this.addFilter.Name = "addFilter";
            this.addFilter.Size = new System.Drawing.Size(24, 23);
            this.addFilter.TabIndex = 9;
            this.addFilter.Text = "+";
            this.tip.SetToolTip(this.addFilter, "Add Filter");
            this.addFilter.UseVisualStyleBackColor = true;
            this.addFilter.Click += new System.EventHandler(this.addFilter_Click);
            // 
            // filterCtrl
            // 
            this.filterCtrl.AllColumns.Add(this.olvColumn4);
            this.filterCtrl.AllColumns.Add(this.olvColumn1);
            this.filterCtrl.AllColumns.Add(this.olvColumn2);
            this.filterCtrl.AllColumns.Add(this.filterCol);
            this.filterCtrl.AllColumns.Add(this.olvColumn3);
            this.filterCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterCtrl.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.filterCtrl.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn4,
            this.olvColumn1,
            this.olvColumn2,
            this.filterCol,
            this.olvColumn3});
            this.filterCtrl.ContextMenuStrip = this.filterContextMenu;
            this.filterCtrl.FullRowSelect = true;
            this.filterCtrl.HeaderWordWrap = true;
            this.filterCtrl.HideSelection = false;
            this.filterCtrl.HighlightBackgroundColor = System.Drawing.Color.Silver;
            this.filterCtrl.Location = new System.Drawing.Point(5, 21);
            this.filterCtrl.MultiSelect = false;
            this.filterCtrl.Name = "filterCtrl";
            this.filterCtrl.ShowFilterMenuOnRightClick = false;
            this.filterCtrl.ShowGroups = false;
            this.filterCtrl.ShowImagesOnSubItems = true;
            this.filterCtrl.Size = new System.Drawing.Size(258, 311);
            this.filterCtrl.TabIndex = 4;
            this.filterCtrl.UseAlternatingBackColors = true;
            this.filterCtrl.UseCellFormatEvents = true;
            this.filterCtrl.UseCompatibleStateImageBehavior = false;
            this.filterCtrl.UseCustomSelectionColors = true;
            this.filterCtrl.UseSubItemCheckBoxes = true;
            this.filterCtrl.View = System.Windows.Forms.View.Details;
            this.filterCtrl.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.enabledCtrl_CellEditFinishing);
            this.filterCtrl.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.enabledCtrl_CellEditStarting);
            this.filterCtrl.ItemsChanged += new System.EventHandler<BrightIdeasSoftware.ItemsChangedEventArgs>(this.enabledCtrl_ItemsChanged);
            this.filterCtrl.SelectionChanged += new System.EventHandler(this.enabledCtrl_SelectionChanged);
            this.filterCtrl.SelectedIndexChanged += new System.EventHandler(this.filterCtrl_SelectedIndexChanged);
            this.filterCtrl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterCtrl_KeyPress);
            this.filterCtrl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.enabledCtrl_MouseDown);
            // 
            // olvColumn4
            // 
            this.olvColumn4.MaximumWidth = 0;
            this.olvColumn4.MinimumWidth = 0;
            this.olvColumn4.Width = 0;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "enabled";
            this.olvColumn1.CheckBoxes = true;
            this.olvColumn1.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn1.Text = "Enb";
            this.olvColumn1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn1.ToolTipText = "Enabled";
            this.olvColumn1.Width = 35;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "dimmed";
            this.olvColumn2.CheckBoxes = true;
            this.olvColumn2.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn2.Text = "Dim";
            this.olvColumn2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn2.ToolTipText = "Dimmed (Show filtered lines, but dimmed)";
            this.olvColumn2.Width = 35;
            // 
            // filterCol
            // 
            this.filterCol.AspectName = "name";
            this.filterCol.AutoCompleteEditor = false;
            this.filterCol.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.filterCol.FillsFreeSpace = true;
            this.filterCol.Text = "Filter";
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "found_count";
            this.olvColumn3.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn3.IsEditable = false;
            this.olvColumn3.Text = "Found";
            this.olvColumn3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn3.ToolTipText = "Shows how many log lines this specific filter has found";
            // 
            // filterContextMenu
            // 
            this.filterContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.moveToTopToolStripMenuItem,
            this.moveToBottomToolStripMenuItem});
            this.filterContextMenu.Name = "filterContextMenu";
            this.filterContextMenu.Size = new System.Drawing.Size(165, 92);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // moveToTopToolStripMenuItem
            // 
            this.moveToTopToolStripMenuItem.Name = "moveToTopToolStripMenuItem";
            this.moveToTopToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.moveToTopToolStripMenuItem.Text = "Move To Top";
            this.moveToTopToolStripMenuItem.Click += new System.EventHandler(this.moveToTopToolStripMenuItem_Click);
            // 
            // moveToBottomToolStripMenuItem
            // 
            this.moveToBottomToolStripMenuItem.Name = "moveToBottomToolStripMenuItem";
            this.moveToBottomToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.moveToBottomToolStripMenuItem.Text = "Move To Bottom";
            this.moveToBottomToolStripMenuItem.Click += new System.EventHandler(this.moveToBottomToolStripMenuItem_Click);
            // 
            // curFilterCtrl
            // 
            this.curFilterCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.curFilterCtrl.Enabled = false;
            this.curFilterCtrl.Location = new System.Drawing.Point(44, 337);
            this.curFilterCtrl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.curFilterCtrl.Multiline = true;
            this.curFilterCtrl.Name = "curFilterCtrl";
            this.curFilterCtrl.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.curFilterCtrl.Size = new System.Drawing.Size(219, 104);
            this.curFilterCtrl.TabIndex = 3;
            this.curFilterCtrl.TextChanged += new System.EventHandler(this.curFilter_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 340);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Filter";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filters";
            // 
            // tip
            // 
            this.tip.AutoPopDelay = 32000;
            this.tip.InitialDelay = 500;
            this.tip.ReshowDelay = 100;
            // 
            // newView
            // 
            this.newView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newView.Location = new System.Drawing.Point(1192, 536);
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
            this.logHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logHistory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.logHistory.FormattingEnabled = true;
            this.logHistory.Location = new System.Drawing.Point(188, 536);
            this.logHistory.Name = "logHistory";
            this.logHistory.Size = new System.Drawing.Size(692, 23);
            this.logHistory.TabIndex = 7;
            this.tip.SetToolTip(this.logHistory, "History - just select any of the previous logs, and they instantly load");
            this.logHistory.SelectedIndexChanged += new System.EventHandler(this.logHistory_SelectedIndexChanged);
            this.logHistory.DropDownClosed += new System.EventHandler(this.logHistory_DropDownClosed);
            // 
            // newFilteredView
            // 
            this.newFilteredView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newFilteredView.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFilteredView.Location = new System.Drawing.Point(422, 3);
            this.newFilteredView.Name = "newFilteredView";
            this.newFilteredView.Size = new System.Drawing.Size(18, 24);
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
            // toggleFilters
            // 
            this.toggleFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.toggleFilters.Location = new System.Drawing.Point(8, 535);
            this.toggleFilters.Name = "toggleFilters";
            this.toggleFilters.Size = new System.Drawing.Size(32, 23);
            this.toggleFilters.TabIndex = 8;
            this.toggleFilters.Text = "+F";
            this.tip.SetToolTip(this.toggleFilters, "Show/Hide Filters");
            this.toggleFilters.UseVisualStyleBackColor = true;
            this.toggleFilters.Click += new System.EventHandler(this.toggleFilters_Click);
            // 
            // toggleSource
            // 
            this.toggleSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.toggleSource.Location = new System.Drawing.Point(46, 535);
            this.toggleSource.Name = "toggleSource";
            this.toggleSource.Size = new System.Drawing.Size(32, 23);
            this.toggleSource.TabIndex = 9;
            this.toggleSource.Text = "+S";
            this.tip.SetToolTip(this.toggleSource, "Show/Hide Source");
            this.toggleSource.UseVisualStyleBackColor = true;
            this.toggleSource.Click += new System.EventHandler(this.toggleSource_Click);
            // 
            // toggleFullLog
            // 
            this.toggleFullLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.toggleFullLog.Location = new System.Drawing.Point(84, 535);
            this.toggleFullLog.Name = "toggleFullLog";
            this.toggleFullLog.Size = new System.Drawing.Size(32, 23);
            this.toggleFullLog.TabIndex = 10;
            this.toggleFullLog.Text = "+L";
            this.tip.SetToolTip(this.toggleFullLog, "Show/Hide Full Log");
            this.toggleFullLog.UseVisualStyleBackColor = true;
            this.toggleFullLog.Click += new System.EventHandler(this.toggleFullLog_Click);
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
            this.delFilteredView.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delFilteredView.Location = new System.Drawing.Point(440, 3);
            this.delFilteredView.Name = "delFilteredView";
            this.delFilteredView.Size = new System.Drawing.Size(18, 24);
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
            this.synchronizedWithFullLog.Location = new System.Drawing.Point(509, 3);
            this.synchronizedWithFullLog.Name = "synchronizedWithFullLog";
            this.synchronizedWithFullLog.Size = new System.Drawing.Size(46, 24);
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
            this.synchronizeWithExistingLogs.Location = new System.Drawing.Point(463, 3);
            this.synchronizeWithExistingLogs.Name = "synchronizeWithExistingLogs";
            this.synchronizeWithExistingLogs.Size = new System.Drawing.Size(46, 24);
            this.synchronizeWithExistingLogs.TabIndex = 3;
            this.synchronizeWithExistingLogs.Text = "<-V->";
            this.tip.SetToolTip(this.synchronizeWithExistingLogs, "Synchronized with the rest of the Views\r\n(when you change the line, the other vie" +
        "ws will \r\ngo to the closest line as you)");
            this.synchronizeWithExistingLogs.UseVisualStyleBackColor = true;
            this.synchronizeWithExistingLogs.CheckedChanged += new System.EventHandler(this.synchronizeWithExistingLogs_CheckedChanged);
            // 
            // viewFromClipboard
            // 
            this.viewFromClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.viewFromClipboard.Location = new System.Drawing.Point(45, 471);
            this.viewFromClipboard.Name = "viewFromClipboard";
            this.viewFromClipboard.Size = new System.Drawing.Size(52, 23);
            this.viewFromClipboard.TabIndex = 14;
            this.viewFromClipboard.Text = "FromC";
            this.tip.SetToolTip(this.viewFromClipboard, "Paste Full Set of Filters From Clipboard");
            this.viewFromClipboard.UseVisualStyleBackColor = true;
            this.viewFromClipboard.Click += new System.EventHandler(this.viewFromClipboard_Click);
            // 
            // viewToClipboard
            // 
            this.viewToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.viewToClipboard.Location = new System.Drawing.Point(3, 471);
            this.viewToClipboard.Name = "viewToClipboard";
            this.viewToClipboard.Size = new System.Drawing.Size(38, 23);
            this.viewToClipboard.TabIndex = 13;
            this.viewToClipboard.Text = "ToC";
            this.tip.SetToolTip(this.viewToClipboard, "Copy This Set of Filters to Clipboard");
            this.viewToClipboard.UseVisualStyleBackColor = true;
            this.viewToClipboard.Click += new System.EventHandler(this.viewToClipboard_Click);
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
            // settingsCtrl
            // 
            this.settingsCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsCtrl.Location = new System.Drawing.Point(1122, 536);
            this.settingsCtrl.Name = "settingsCtrl";
            this.settingsCtrl.Size = new System.Drawing.Size(68, 23);
            this.settingsCtrl.TabIndex = 12;
            this.settingsCtrl.Text = "Settings";
            this.settingsCtrl.UseVisualStyleBackColor = true;
            this.settingsCtrl.Click += new System.EventHandler(this.settingsCtrl_Click);
            // 
            // about
            // 
            this.about.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.about.Location = new System.Drawing.Point(974, 536);
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
            this.main.Location = new System.Drawing.Point(2, 1);
            this.main.Name = "main";
            // 
            // main.Panel1
            // 
            this.main.Panel1.Controls.Add(this.leftPane);
            // 
            // main.Panel2
            // 
            this.main.Panel2.Controls.Add(this.sourceUp);
            this.main.Size = new System.Drawing.Size(1258, 526);
            this.main.SplitterDistance = 273;
            this.main.TabIndex = 4;
            // 
            // leftPane
            // 
            this.leftPane.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.leftPane.Controls.Add(this.tabPage2);
            this.leftPane.Controls.Add(this.tabPage3);
            this.leftPane.Controls.Add(this.tabPage4);
            this.leftPane.Location = new System.Drawing.Point(-1, 4);
            this.leftPane.Name = "leftPane";
            this.leftPane.SelectedIndex = 0;
            this.leftPane.Size = new System.Drawing.Size(277, 526);
            this.leftPane.TabIndex = 13;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.viewFromClipboard);
            this.tabPage2.Controls.Add(this.viewToClipboard);
            this.tabPage2.Controls.Add(this.filterCtrl);
            this.tabPage2.Controls.Add(this.checkBox1);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.curFilterCtrl);
            this.tabPage2.Controls.Add(this.delFilter);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.addFilter);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(269, 498);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Filters";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(44, 445);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(144, 19);
            this.checkBox1.TabIndex = 12;
            this.checkBox1.Text = "Apply to existing Lines";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listBox1);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(269, 500);
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
            this.tabPage4.Controls.Add(this.label4);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(269, 500);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "Notes / Bookmarks";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(151, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "allow notes on bookmarks?";
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
            this.sourceUp.Size = new System.Drawing.Size(981, 526);
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
            this.filteredLeft.Size = new System.Drawing.Size(981, 457);
            this.filteredLeft.SplitterDistance = 558;
            this.filteredLeft.TabIndex = 0;
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
            this.viewsTab.Size = new System.Drawing.Size(553, 454);
            this.viewsTab.TabIndex = 0;
            this.viewsTab.SelectedIndexChanged += new System.EventHandler(this.filteredViews_SelectedIndexChanged);
            this.viewsTab.DragDrop += new System.Windows.Forms.DragEventHandler(this.filteredViews_DragDrop);
            this.viewsTab.DragEnter += new System.Windows.Forms.DragEventHandler(this.filteredViews_DragEnter);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dropHere);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(545, 426);
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
            this.dropHere.Location = new System.Drawing.Point(8, 0);
            this.dropHere.Name = "dropHere";
            this.dropHere.Size = new System.Drawing.Size(534, 453);
            this.dropHere.TabIndex = 0;
            this.dropHere.Text = "Drop it Like it\'s Hot!\r\nJust drop a file here, and get to work!\r\n";
            this.dropHere.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.dropHere.DragDrop += new System.Windows.Forms.DragEventHandler(this.dropHere_DragDrop);
            this.dropHere.DragEnter += new System.Windows.Forms.DragEventHandler(this.dropHere_DragEnter);
            // 
            // refreshFilter
            // 
            this.refreshFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.refreshFilter.Location = new System.Drawing.Point(122, 535);
            this.refreshFilter.Name = "refreshFilter";
            this.refreshFilter.Size = new System.Drawing.Size(60, 23);
            this.refreshFilter.TabIndex = 11;
            this.refreshFilter.Text = "Refresh";
            this.refreshFilter.UseVisualStyleBackColor = true;
            this.refreshFilter.Click += new System.EventHandler(this.refreshFilter_Click);
            // 
            // tipsHotkeys
            // 
            this.tipsHotkeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tipsHotkeys.AutoSize = true;
            this.tipsHotkeys.Location = new System.Drawing.Point(886, 541);
            this.tipsHotkeys.Name = "tipsHotkeys";
            this.tipsHotkeys.Size = new System.Drawing.Size(83, 15);
            this.tipsHotkeys.TabIndex = 12;
            this.tipsHotkeys.TabStop = true;
            this.tipsHotkeys.Text = "Tips / Hotkeys";
            this.tipsHotkeys.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.tipsHotkeys_LinkClicked);
            // 
            // refresh
            // 
            this.refresh.Enabled = true;
            this.refresh.Interval = 500;
            this.refresh.Tick += new System.EventHandler(this.refresh_Tick);
            // 
            // focusOnFilterCtrl
            // 
            this.focusOnFilterCtrl.Interval = 10;
            this.focusOnFilterCtrl.Tick += new System.EventHandler(this.focusOnFilterCtrl_Tick);
            // 
            // postFocus
            // 
            this.postFocus.Interval = 1;
            this.postFocus.Tick += new System.EventHandler(this.postFocus_Tick);
            // 
            // monitor
            // 
            this.monitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.monitor.Location = new System.Drawing.Point(1044, 536);
            this.monitor.Name = "monitor";
            this.monitor.Size = new System.Drawing.Size(76, 23);
            this.monitor.TabIndex = 14;
            this.monitor.Text = "Monitor";
            this.monitor.UseVisualStyleBackColor = true;
            this.monitor.Click += new System.EventHandler(this.monitor_Click);
            // 
            // log_wizard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1255, 568);
            this.Controls.Add(this.monitor);
            this.Controls.Add(this.about);
            this.Controls.Add(this.tipsHotkeys);
            this.Controls.Add(this.settingsCtrl);
            this.Controls.Add(this.refreshFilter);
            this.Controls.Add(this.toggleFullLog);
            this.Controls.Add(this.toggleSource);
            this.Controls.Add(this.toggleFilters);
            this.Controls.Add(this.logHistory);
            this.Controls.Add(this.newView);
            this.Controls.Add(this.main);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "log_wizard";
            this.Text = "Log Wizard";
            this.Deactivate += new System.EventHandler(this.log_wizard_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogWizard_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogNinja_FormClosed);
            this.SizeChanged += new System.EventHandler(this.log_wizard_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.filterCtrl)).EndInit();
            this.filterContextMenu.ResumeLayout(false);
            this.main.Panel1.ResumeLayout(false);
            this.main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.main)).EndInit();
            this.main.ResumeLayout(false);
            this.leftPane.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox curFilterCtrl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip tip;
        private BrightIdeasSoftware.ObjectListView filterCtrl;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn filterCol;
        private System.Windows.Forms.Button delFilter;
        private System.Windows.Forms.Button addFilter;
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
        private System.Windows.Forms.Button toggleFilters;
        private System.Windows.Forms.Button toggleSource;
        private System.Windows.Forms.Button toggleFullLog;
        private System.Windows.Forms.TextBox logSyntaxCtrl;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label dropHere;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private System.Windows.Forms.Timer refresh;
        private System.Windows.Forms.Button delFilteredView;
        private System.Windows.Forms.Button delContext;
        private System.Windows.Forms.Button addContext;
        private System.Windows.Forms.Timer focusOnFilterCtrl;
        private System.Windows.Forms.ContextMenuStrip filterContextMenu;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.Button refreshFilter;
        private System.Windows.Forms.Button settingsCtrl;
        private System.Windows.Forms.LinkLabel tipsHotkeys;
        private System.Windows.Forms.Button about;
        private System.Windows.Forms.Timer postFocus;
        private System.Windows.Forms.CheckBox synchronizedWithFullLog;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TabControl leftPane;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox synchronizeWithExistingLogs;
        private System.Windows.Forms.ToolStripMenuItem moveToTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToBottomToolStripMenuItem;
        private System.Windows.Forms.Button viewFromClipboard;
        private System.Windows.Forms.Button viewToClipboard;
        private System.Windows.Forms.Button contextFromClipboard;
        private System.Windows.Forms.Button contextToClipboard;
        private System.Windows.Forms.Button monitor;
    }
}

