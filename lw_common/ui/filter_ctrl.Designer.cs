namespace lw_common.ui {
    partial class filter_ctrl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.viewFromClipboard = new System.Windows.Forms.Button();
            this.viewToClipboard = new System.Windows.Forms.Button();
            this.filterCtrl = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.filterCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.applyToExistingLines = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.curFilterCtrl = new System.Windows.Forms.TextBox();
            this.delFilter = new System.Windows.Forms.Button();
            this.filterLabel = new System.Windows.Forms.Label();
            this.addFilter = new System.Windows.Forms.Button();
            this.selectColor = new System.Windows.Forms.Button();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.tipsHotkeys = new System.Windows.Forms.LinkLabel();
            this.filterContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.filterCtrl)).BeginInit();
            this.filterContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewFromClipboard
            // 
            this.viewFromClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.viewFromClipboard.Location = new System.Drawing.Point(81, 480);
            this.viewFromClipboard.Name = "viewFromClipboard";
            this.viewFromClipboard.Size = new System.Drawing.Size(52, 23);
            this.viewFromClipboard.TabIndex = 23;
            this.viewFromClipboard.Text = "FromC";
            this.viewFromClipboard.UseVisualStyleBackColor = true;
            this.viewFromClipboard.Click += new System.EventHandler(this.viewFromClipboard_Click);
            // 
            // viewToClipboard
            // 
            this.viewToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.viewToClipboard.Location = new System.Drawing.Point(43, 480);
            this.viewToClipboard.Name = "viewToClipboard";
            this.viewToClipboard.Size = new System.Drawing.Size(38, 23);
            this.viewToClipboard.TabIndex = 22;
            this.viewToClipboard.Text = "ToC";
            this.viewToClipboard.UseVisualStyleBackColor = true;
            this.viewToClipboard.Click += new System.EventHandler(this.viewToClipboard_Click);
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
            this.filterCtrl.FullRowSelect = true;
            this.filterCtrl.HeaderWordWrap = true;
            this.filterCtrl.HideSelection = false;
            this.filterCtrl.HighlightBackgroundColor = System.Drawing.Color.Silver;
            this.filterCtrl.Location = new System.Drawing.Point(0, 21);
            this.filterCtrl.MultiSelect = false;
            this.filterCtrl.Name = "filterCtrl";
            this.filterCtrl.ShowFilterMenuOnRightClick = false;
            this.filterCtrl.ShowGroups = false;
            this.filterCtrl.ShowImagesOnSubItems = true;
            this.filterCtrl.Size = new System.Drawing.Size(353, 327);
            this.filterCtrl.TabIndex = 18;
            this.filterCtrl.UseAlternatingBackColors = true;
            this.filterCtrl.UseCellFormatEvents = true;
            this.filterCtrl.UseCompatibleStateImageBehavior = false;
            this.filterCtrl.UseCustomSelectionColors = true;
            this.filterCtrl.UseSubItemCheckBoxes = true;
            this.filterCtrl.View = System.Windows.Forms.View.Details;
            this.filterCtrl.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.filterCtrl_CellEditStarting);
            this.filterCtrl.ItemsChanged += new System.EventHandler<BrightIdeasSoftware.ItemsChangedEventArgs>(this.filterCtrl_ItemsChanged);
            this.filterCtrl.SelectionChanged += new System.EventHandler(this.filterCtrl_SelectionChanged);
            this.filterCtrl.SelectedIndexChanged += new System.EventHandler(this.filterCtrl_SelectedIndexChanged);
            this.filterCtrl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterCtrl_KeyPress);
            this.filterCtrl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.filterCtrl_MouseDown);
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
            // applyToExistingLines
            // 
            this.applyToExistingLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.applyToExistingLines.AutoSize = true;
            this.applyToExistingLines.Location = new System.Drawing.Point(43, 462);
            this.applyToExistingLines.Name = "applyToExistingLines";
            this.applyToExistingLines.Size = new System.Drawing.Size(130, 17);
            this.applyToExistingLines.TabIndex = 21;
            this.applyToExistingLines.Text = "Apply to existing Lines";
            this.applyToExistingLines.UseVisualStyleBackColor = true;
            this.applyToExistingLines.CheckedChanged += new System.EventHandler(this.applyToExistingLines_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 17);
            this.label1.TabIndex = 15;
            this.label1.Text = "Filters";
            // 
            // curFilterCtrl
            // 
            this.curFilterCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.curFilterCtrl.Enabled = false;
            this.curFilterCtrl.Location = new System.Drawing.Point(41, 355);
            this.curFilterCtrl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.curFilterCtrl.Multiline = true;
            this.curFilterCtrl.Name = "curFilterCtrl";
            this.curFilterCtrl.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.curFilterCtrl.Size = new System.Drawing.Size(312, 104);
            this.curFilterCtrl.TabIndex = 17;
            this.curFilterCtrl.TextChanged += new System.EventHandler(this.curFilterCtrl_TextChanged);
            this.curFilterCtrl.Leave += new System.EventHandler(this.curFilterCtrl_Leave);
            // 
            // delFilter
            // 
            this.delFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.delFilter.Location = new System.Drawing.Point(327, 480);
            this.delFilter.Name = "delFilter";
            this.delFilter.Size = new System.Drawing.Size(24, 23);
            this.delFilter.TabIndex = 20;
            this.delFilter.Text = "-";
            this.delFilter.UseVisualStyleBackColor = true;
            this.delFilter.Click += new System.EventHandler(this.delFilter_Click);
            // 
            // filterLabel
            // 
            this.filterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.filterLabel.AutoSize = true;
            this.filterLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filterLabel.Location = new System.Drawing.Point(0, 358);
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Size = new System.Drawing.Size(43, 19);
            this.filterLabel.TabIndex = 16;
            this.filterLabel.Text = "Filter";
            // 
            // addFilter
            // 
            this.addFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addFilter.Location = new System.Drawing.Point(299, 480);
            this.addFilter.Name = "addFilter";
            this.addFilter.Size = new System.Drawing.Size(24, 23);
            this.addFilter.TabIndex = 19;
            this.addFilter.Text = "+";
            this.addFilter.UseVisualStyleBackColor = true;
            this.addFilter.Click += new System.EventHandler(this.addFilter_Click);
            // 
            // selectColor
            // 
            this.selectColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.selectColor.BackgroundImage = global::lw_common.Properties.Resources.eyedropper;
            this.selectColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.selectColor.Location = new System.Drawing.Point(15, 380);
            this.selectColor.Name = "selectColor";
            this.selectColor.Size = new System.Drawing.Size(22, 22);
            this.selectColor.TabIndex = 24;
            this.selectColor.UseVisualStyleBackColor = true;
            this.selectColor.Click += new System.EventHandler(this.selectColor_Click);
            // 
            // tipsHotkeys
            // 
            this.tipsHotkeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tipsHotkeys.AutoSize = true;
            this.tipsHotkeys.Location = new System.Drawing.Point(324, 462);
            this.tipsHotkeys.Name = "tipsHotkeys";
            this.tipsHotkeys.Size = new System.Drawing.Size(27, 13);
            this.tipsHotkeys.TabIndex = 25;
            this.tipsHotkeys.TabStop = true;
            this.tipsHotkeys.Text = "Tips";
            this.tipsHotkeys.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.tipsHotkeys_LinkClicked);
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
            // filter_ctrl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.tipsHotkeys);
            this.Controls.Add(this.selectColor);
            this.Controls.Add(this.viewFromClipboard);
            this.Controls.Add(this.viewToClipboard);
            this.Controls.Add(this.filterCtrl);
            this.Controls.Add(this.applyToExistingLines);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.curFilterCtrl);
            this.Controls.Add(this.delFilter);
            this.Controls.Add(this.filterLabel);
            this.Controls.Add(this.addFilter);
            this.Name = "filter_ctrl";
            this.Size = new System.Drawing.Size(354, 505);
            this.Load += new System.EventHandler(this.filter_ctrl_Load);
            this.SizeChanged += new System.EventHandler(this.filter_ctrl_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.filterCtrl)).EndInit();
            this.filterContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button viewFromClipboard;
        private System.Windows.Forms.Button viewToClipboard;
        private BrightIdeasSoftware.ObjectListView filterCtrl;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn filterCol;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.CheckBox applyToExistingLines;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox curFilterCtrl;
        private System.Windows.Forms.Button delFilter;
        private System.Windows.Forms.Label filterLabel;
        private System.Windows.Forms.Button addFilter;
        private System.Windows.Forms.Button selectColor;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.LinkLabel tipsHotkeys;
        private System.Windows.Forms.ContextMenuStrip filterContextMenu;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToBottomToolStripMenuItem;
    }
}
