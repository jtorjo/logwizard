namespace LogWizard
{
    partial class log_view
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelName = new System.Windows.Forms.Label();
            this.viewName = new System.Windows.Forms.TextBox();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.list = new BrightIdeasSoftware.VirtualObjectListView();
            this.lineCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.viewCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.dateCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.timeCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.levelCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.fileCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.funcCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.classCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx1Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx2Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx3Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.msgCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            ((System.ComponentModel.ISupportInitialize)(this.list)).BeginInit();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(3, 3);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(30, 13);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "View";
            this.tip.SetToolTip(this.labelName, "Name of the Filtered View (shows up as the tab name)");
            // 
            // viewName
            // 
            this.viewName.Location = new System.Drawing.Point(51, 0);
            this.viewName.Name = "viewName";
            this.viewName.Size = new System.Drawing.Size(265, 20);
            this.viewName.TabIndex = 2;
            this.tip.SetToolTip(this.viewName, "Name of the Filtered View (shows up as the tab name)");
            this.viewName.TextChanged += new System.EventHandler(this.filterName_TextChanged);
            // 
            // list
            // 
            this.list.AllColumns.Add(this.lineCol);
            this.list.AllColumns.Add(this.viewCol);
            this.list.AllColumns.Add(this.dateCol);
            this.list.AllColumns.Add(this.timeCol);
            this.list.AllColumns.Add(this.levelCol);
            this.list.AllColumns.Add(this.fileCol);
            this.list.AllColumns.Add(this.funcCol);
            this.list.AllColumns.Add(this.classCol);
            this.list.AllColumns.Add(this.ctx1Col);
            this.list.AllColumns.Add(this.ctx2Col);
            this.list.AllColumns.Add(this.ctx3Col);
            this.list.AllColumns.Add(this.msgCol);
            this.list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lineCol,
            this.viewCol,
            this.dateCol,
            this.timeCol,
            this.levelCol,
            this.fileCol,
            this.funcCol,
            this.classCol,
            this.ctx1Col,
            this.ctx2Col,
            this.ctx3Col,
            this.msgCol});
            this.list.FullRowSelect = true;
            this.list.HideSelection = false;
            this.list.Location = new System.Drawing.Point(2, 22);
            this.list.Name = "list";
            this.list.OwnerDraw = true;
            this.list.ShowGroups = false;
            this.list.ShowItemToolTips = true;
            this.list.Size = new System.Drawing.Size(693, 404);
            this.list.TabIndex = 3;
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.View = System.Windows.Forms.View.Details;
            this.list.VirtualMode = true;
            this.list.CellOver += new System.EventHandler<BrightIdeasSoftware.CellOverEventArgs>(this.list_CellOver);
            this.list.CellToolTipShowing += new System.EventHandler<BrightIdeasSoftware.ToolTipShowingEventArgs>(this.list_CellToolTipShowing);
            this.list.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.list_FormatCell_1);
            this.list.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.list_FormatRow_1);
            this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
            this.list.Enter += new System.EventHandler(this.list_Enter);
            this.list.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.list_KeyPress);
            this.list.Leave += new System.EventHandler(this.list_Leave);
            // 
            // lineCol
            // 
            this.lineCol.AspectName = "line";
            this.lineCol.IsEditable = false;
            this.lineCol.Text = "Line";
            this.lineCol.Width = 70;
            // 
            // viewCol
            // 
            this.viewCol.AspectName = "view";
            this.viewCol.IsEditable = false;
            this.viewCol.Text = "View(s)";
            this.viewCol.Width = 0;
            // 
            // dateCol
            // 
            this.dateCol.AspectName = "date";
            this.dateCol.IsEditable = false;
            this.dateCol.Text = "Date";
            // 
            // timeCol
            // 
            this.timeCol.AspectName = "time";
            this.timeCol.IsEditable = false;
            this.timeCol.Text = "Time";
            this.timeCol.Width = 100;
            // 
            // levelCol
            // 
            this.levelCol.AspectName = "level";
            this.levelCol.IsEditable = false;
            this.levelCol.Text = "Level";
            // 
            // fileCol
            // 
            this.fileCol.AspectName = "file";
            this.fileCol.IsEditable = false;
            this.fileCol.Text = "File";
            this.fileCol.Width = 0;
            // 
            // funcCol
            // 
            this.funcCol.AspectName = "func";
            this.funcCol.IsEditable = false;
            this.funcCol.Text = "Func";
            this.funcCol.Width = 0;
            // 
            // classCol
            // 
            this.classCol.AspectName = "class_";
            this.classCol.IsEditable = false;
            this.classCol.Text = "Class";
            this.classCol.Width = 0;
            // 
            // ctx1Col
            // 
            this.ctx1Col.AspectName = "ctx1";
            this.ctx1Col.IsEditable = false;
            this.ctx1Col.Text = "Ctx1";
            this.ctx1Col.Width = 0;
            // 
            // ctx2Col
            // 
            this.ctx2Col.AspectName = "ctx2";
            this.ctx2Col.IsEditable = false;
            this.ctx2Col.Text = "Ctx2";
            this.ctx2Col.Width = 0;
            // 
            // ctx3Col
            // 
            this.ctx3Col.AspectName = "ctx3";
            this.ctx3Col.IsEditable = false;
            this.ctx3Col.Text = "Ctx3";
            this.ctx3Col.Width = 0;
            // 
            // msgCol
            // 
            this.msgCol.AspectName = "msg";
            this.msgCol.FillsFreeSpace = true;
            this.msgCol.Text = "Message";
            // 
            // log_view
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.list);
            this.Controls.Add(this.viewName);
            this.Controls.Add(this.labelName);
            this.Name = "log_view";
            this.Size = new System.Drawing.Size(697, 427);
            this.Load += new System.EventHandler(this.log_view_Load);
            ((System.ComponentModel.ISupportInitialize)(this.list)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox viewName;
        private System.Windows.Forms.ToolTip tip;
        private BrightIdeasSoftware.OLVColumn lineCol;
        private BrightIdeasSoftware.OLVColumn dateCol;
        private BrightIdeasSoftware.OLVColumn timeCol;
        private BrightIdeasSoftware.OLVColumn levelCol;
        private BrightIdeasSoftware.OLVColumn fileCol;
        private BrightIdeasSoftware.OLVColumn funcCol;
        private BrightIdeasSoftware.OLVColumn classCol;
        private BrightIdeasSoftware.OLVColumn ctx1Col;
        private BrightIdeasSoftware.OLVColumn ctx2Col;
        private BrightIdeasSoftware.OLVColumn ctx3Col;
        private BrightIdeasSoftware.OLVColumn viewCol;
        internal BrightIdeasSoftware.VirtualObjectListView list;
        internal BrightIdeasSoftware.OLVColumn msgCol;
    }
}
