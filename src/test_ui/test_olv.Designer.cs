namespace test_ui {
    partial class test_olv {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.list = new BrightIdeasSoftware.ObjectListView();
            this.idxCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.dateCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.timeCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.hiddenCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.msgCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            ((System.ComponentModel.ISupportInitialize)(this.list)).BeginInit();
            this.SuspendLayout();
            // 
            // list
            // 
            this.list.AllColumns.Add(this.idxCol);
            this.list.AllColumns.Add(this.dateCol);
            this.list.AllColumns.Add(this.timeCol);
            this.list.AllColumns.Add(this.hiddenCol);
            this.list.AllColumns.Add(this.msgCol);
            this.list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.idxCol,
            this.dateCol,
            this.timeCol,
            this.hiddenCol,
            this.msgCol});
            this.list.Font = new System.Drawing.Font("Consolas", 9F);
            this.list.FullRowSelect = true;
            this.list.Location = new System.Drawing.Point(12, 12);
            this.list.Name = "list";
            this.list.ShowGroups = false;
            this.list.Size = new System.Drawing.Size(660, 385);
            this.list.TabIndex = 0;
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.View = System.Windows.Forms.View.Details;
            // 
            // idxCol
            // 
            this.idxCol.AspectName = "idx";
            this.idxCol.Text = "Line";
            // 
            // dateCol
            // 
            this.dateCol.AspectName = "date";
            this.dateCol.Text = "Date";
            // 
            // timeCol
            // 
            this.timeCol.AspectName = "time";
            this.timeCol.Text = "Time";
            this.timeCol.Width = 90;
            // 
            // hiddenCol
            // 
            this.hiddenCol.AspectName = "hidden";
            this.hiddenCol.Text = "Hidden";
            // 
            // msgCol
            // 
            this.msgCol.AspectName = "msg";
            this.msgCol.FillsFreeSpace = true;
            this.msgCol.Text = "Msg";
            // 
            // test_olv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 409);
            this.Controls.Add(this.list);
            this.Name = "test_olv";
            this.Text = "test_olv";
            ((System.ComponentModel.ISupportInitialize)(this.list)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView list;
        private BrightIdeasSoftware.OLVColumn idxCol;
        private BrightIdeasSoftware.OLVColumn dateCol;
        private BrightIdeasSoftware.OLVColumn timeCol;
        private BrightIdeasSoftware.OLVColumn hiddenCol;
        private BrightIdeasSoftware.OLVColumn msgCol;
    }
}