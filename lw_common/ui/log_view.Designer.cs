namespace lw_common
{
    public partial class log_view
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
            this.threadCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.fileCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.funcCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.classCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx1Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx2Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx3Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx4Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx5Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx6Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx7Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx8Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx9Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx10Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx11Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx12Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx13Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx14Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ctx15Col = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.msgCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.edit = new lw_common.ui.smart_readonly_textbox();
            ((System.ComponentModel.ISupportInitialize)(this.list)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.BackColor = System.Drawing.Color.WhiteSmoke;
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(3, 2);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(33, 15);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "View";
            this.tip.SetToolTip(this.labelName, "Name of the Filtered View (shows up as the tab name)");
            // 
            // viewName
            // 
            this.viewName.Location = new System.Drawing.Point(41, 0);
            this.viewName.Name = "viewName";
            this.viewName.Size = new System.Drawing.Size(145, 20);
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
            this.list.AllColumns.Add(this.threadCol);
            this.list.AllColumns.Add(this.fileCol);
            this.list.AllColumns.Add(this.funcCol);
            this.list.AllColumns.Add(this.classCol);
            this.list.AllColumns.Add(this.ctx1Col);
            this.list.AllColumns.Add(this.ctx2Col);
            this.list.AllColumns.Add(this.ctx3Col);
            this.list.AllColumns.Add(this.ctx4Col);
            this.list.AllColumns.Add(this.ctx5Col);
            this.list.AllColumns.Add(this.ctx6Col);
            this.list.AllColumns.Add(this.ctx7Col);
            this.list.AllColumns.Add(this.ctx8Col);
            this.list.AllColumns.Add(this.ctx9Col);
            this.list.AllColumns.Add(this.ctx10Col);
            this.list.AllColumns.Add(this.ctx11Col);
            this.list.AllColumns.Add(this.ctx12Col);
            this.list.AllColumns.Add(this.ctx13Col);
            this.list.AllColumns.Add(this.ctx14Col);
            this.list.AllColumns.Add(this.ctx15Col);
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
            this.threadCol,
            this.fileCol,
            this.funcCol,
            this.classCol,
            this.ctx1Col,
            this.ctx2Col,
            this.ctx3Col,
            this.ctx4Col,
            this.ctx5Col,
            this.ctx6Col,
            this.ctx7Col,
            this.ctx8Col,
            this.ctx9Col,
            this.ctx10Col,
            this.ctx11Col,
            this.ctx12Col,
            this.ctx13Col,
            this.ctx14Col,
            this.ctx15Col,
            this.msgCol});
            this.list.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.list.FullRowSelect = true;
            this.list.HideSelection = false;
            this.list.Location = new System.Drawing.Point(2, 22);
            this.list.Name = "list";
            this.list.OwnerDraw = true;
            this.list.ShowGroups = false;
            this.list.ShowItemToolTips = true;
            this.list.Size = new System.Drawing.Size(693, 403);
            this.list.TabIndex = 3;
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.UseOverlays = false;
            this.list.View = System.Windows.Forms.View.Details;
            this.list.VirtualMode = true;
            this.list.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.list_CellClick);
            this.list.CellOver += new System.EventHandler<BrightIdeasSoftware.CellOverEventArgs>(this.list_CellOver);
            this.list.CellToolTipShowing += new System.EventHandler<BrightIdeasSoftware.ToolTipShowingEventArgs>(this.list_CellToolTipShowing);
            this.list.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.list_FormatCell_1);
            this.list.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.list_FormatRow_1);
            this.list.Scroll += new System.EventHandler<System.Windows.Forms.ScrollEventArgs>(this.list_Scroll);
            this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
            this.list.Enter += new System.EventHandler(this.list_Enter);
            this.list.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.list_KeyPress);
            this.list.Leave += new System.EventHandler(this.list_Leave);
            this.list.MouseClick += new System.Windows.Forms.MouseEventHandler(this.list_MouseClick);
            this.list.MouseDown += new System.Windows.Forms.MouseEventHandler(this.list_MouseDown);
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
            // threadCol
            // 
            this.threadCol.AspectName = "thread";
            this.threadCol.IsEditable = false;
            this.threadCol.Text = "Thread";
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
            // ctx4Col
            // 
            this.ctx4Col.AspectName = "ctx4";
            this.ctx4Col.IsEditable = false;
            this.ctx4Col.Text = "Ctx4";
            this.ctx4Col.Width = 0;
            // 
            // ctx5Col
            // 
            this.ctx5Col.AspectName = "ctx5";
            this.ctx5Col.IsEditable = false;
            this.ctx5Col.Text = "Ctx5";
            this.ctx5Col.Width = 0;
            // 
            // ctx6Col
            // 
            this.ctx6Col.AspectName = "ctx6";
            this.ctx6Col.IsEditable = false;
            this.ctx6Col.Text = "Ctx6";
            this.ctx6Col.Width = 0;
            // 
            // ctx7Col
            // 
            this.ctx7Col.AspectName = "ctx7";
            this.ctx7Col.IsEditable = false;
            this.ctx7Col.Text = "Ctx7";
            this.ctx7Col.Width = 0;
            // 
            // ctx8Col
            // 
            this.ctx8Col.AspectName = "ctx8";
            this.ctx8Col.IsEditable = false;
            this.ctx8Col.Text = "Ctx8";
            this.ctx8Col.Width = 0;
            // 
            // ctx9Col
            // 
            this.ctx9Col.AspectName = "ctx9";
            this.ctx9Col.IsEditable = false;
            this.ctx9Col.Text = "Ctx9";
            this.ctx9Col.Width = 0;
            // 
            // ctx10Col
            // 
            this.ctx10Col.AspectName = "ctx10";
            this.ctx10Col.IsEditable = false;
            this.ctx10Col.Text = "Ctx10";
            this.ctx10Col.Width = 0;
            // 
            // ctx11Col
            // 
            this.ctx11Col.AspectName = "ctx11";
            this.ctx11Col.IsEditable = false;
            this.ctx11Col.Text = "Ctx11";
            this.ctx11Col.Width = 0;
            // 
            // ctx12Col
            // 
            this.ctx12Col.AspectName = "ctx12";
            this.ctx12Col.IsEditable = false;
            this.ctx12Col.Text = "Ctx12";
            this.ctx12Col.Width = 0;
            // 
            // ctx13Col
            // 
            this.ctx13Col.AspectName = "ctx13";
            this.ctx13Col.IsEditable = false;
            this.ctx13Col.Text = "Ctx13";
            this.ctx13Col.Width = 0;
            // 
            // ctx14Col
            // 
            this.ctx14Col.AspectName = "ctx14";
            this.ctx14Col.IsEditable = false;
            this.ctx14Col.Text = "Ctx14";
            this.ctx14Col.Width = 0;
            // 
            // ctx15Col
            // 
            this.ctx15Col.AspectName = "ctx15";
            this.ctx15Col.IsEditable = false;
            this.ctx15Col.Text = "Ctx15";
            this.ctx15Col.Width = 0;
            // 
            // msgCol
            // 
            this.msgCol.AspectName = "msg";
            this.msgCol.FillsFreeSpace = true;
            this.msgCol.IsEditable = false;
            this.msgCol.Text = "Message";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(2, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(693, 23);
            this.panel1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(189, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter to save, Esc to cancel.";
            // 
            // edit
            // 
            this.edit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edit.Location = new System.Drawing.Point(-200, 29);
            this.edit.Multiline = false;
            this.edit.Name = "edit";
            this.edit.ReadOnly = true;
            this.edit.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.edit.Size = new System.Drawing.Size(50, 20);
            this.edit.TabIndex = 5;
            this.edit.Text = "";
            // 
            // log_view
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.list);
            this.Controls.Add(this.edit);
            this.Controls.Add(this.viewName);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.panel1);
            this.Name = "log_view";
            this.Size = new System.Drawing.Size(697, 427);
            this.Load += new System.EventHandler(this.log_view_Load);
            ((System.ComponentModel.ISupportInitialize)(this.list)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox viewName;
        private System.Windows.Forms.ToolTip tip;
        internal BrightIdeasSoftware.OLVColumn msgCol;
        private System.Windows.Forms.Panel panel1;
        public BrightIdeasSoftware.VirtualObjectListView list;
        internal ui.smart_readonly_textbox edit;
        private System.Windows.Forms.Label label1;
        internal BrightIdeasSoftware.OLVColumn lineCol;
        internal BrightIdeasSoftware.OLVColumn dateCol;
        internal BrightIdeasSoftware.OLVColumn timeCol;
        internal BrightIdeasSoftware.OLVColumn levelCol;
        internal BrightIdeasSoftware.OLVColumn fileCol;
        internal BrightIdeasSoftware.OLVColumn funcCol;
        internal BrightIdeasSoftware.OLVColumn classCol;
        internal BrightIdeasSoftware.OLVColumn ctx1Col;
        internal BrightIdeasSoftware.OLVColumn ctx2Col;
        internal BrightIdeasSoftware.OLVColumn ctx3Col;
        internal BrightIdeasSoftware.OLVColumn viewCol;
        internal BrightIdeasSoftware.OLVColumn threadCol;
        internal BrightIdeasSoftware.OLVColumn ctx4Col;
        internal BrightIdeasSoftware.OLVColumn ctx5Col;
        internal BrightIdeasSoftware.OLVColumn ctx6Col;
        internal BrightIdeasSoftware.OLVColumn ctx7Col;
        internal BrightIdeasSoftware.OLVColumn ctx8Col;
        internal BrightIdeasSoftware.OLVColumn ctx9Col;
        internal BrightIdeasSoftware.OLVColumn ctx10Col;
        internal BrightIdeasSoftware.OLVColumn ctx11Col;
        internal BrightIdeasSoftware.OLVColumn ctx12Col;
        internal BrightIdeasSoftware.OLVColumn ctx13Col;
        internal BrightIdeasSoftware.OLVColumn ctx14Col;
        internal BrightIdeasSoftware.OLVColumn ctx15Col;
    }
}
