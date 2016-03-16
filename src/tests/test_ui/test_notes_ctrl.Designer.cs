namespace test_ui {
    partial class test_notes_ctrl {
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
            this.dummyView = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.undo = new System.Windows.Forms.Button();
            this.notes = new lw_common.ui.note_ctrl();
            ((System.ComponentModel.ISupportInitialize)(this.dummyView)).BeginInit();
            this.SuspendLayout();
            // 
            // dummyView
            // 
            this.dummyView.AllColumns.Add(this.olvColumn1);
            this.dummyView.AllColumns.Add(this.olvColumn2);
            this.dummyView.AllColumns.Add(this.olvColumn3);
            this.dummyView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dummyView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3});
            this.dummyView.FullRowSelect = true;
            this.dummyView.HideSelection = false;
            this.dummyView.Location = new System.Drawing.Point(431, 3);
            this.dummyView.Name = "dummyView";
            this.dummyView.ShowGroups = false;
            this.dummyView.Size = new System.Drawing.Size(332, 705);
            this.dummyView.TabIndex = 1;
            this.dummyView.UseCompatibleStateImageBehavior = false;
            this.dummyView.View = System.Windows.Forms.View.Details;
            this.dummyView.SelectedIndexChanged += new System.EventHandler(this.dummyView_SelectedIndexChanged);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "idx";
            this.olvColumn1.IsEditable = false;
            this.olvColumn1.Text = "idx";
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "view";
            this.olvColumn2.IsEditable = false;
            this.olvColumn2.Text = "view";
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "msg";
            this.olvColumn3.FillsFreeSpace = true;
            this.olvColumn3.IsEditable = false;
            this.olvColumn3.Text = "msg";
            // 
            // undo
            // 
            this.undo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.undo.Location = new System.Drawing.Point(688, 713);
            this.undo.Name = "undo";
            this.undo.Size = new System.Drawing.Size(75, 23);
            this.undo.TabIndex = 2;
            this.undo.Text = "Undo";
            this.undo.UseVisualStyleBackColor = true;
            this.undo.Click += new System.EventHandler(this.undo_Click);
            // 
            // notes
            // 
            this.notes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.notes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notes.Location = new System.Drawing.Point(13, 3);
            this.notes.Margin = new System.Windows.Forms.Padding(4);
            this.notes.Name = "notes";
            this.notes.Size = new System.Drawing.Size(411, 732);
            this.notes.TabIndex = 0;
            // 
            // test_notes_ctrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 748);
            this.Controls.Add(this.undo);
            this.Controls.Add(this.dummyView);
            this.Controls.Add(this.notes);
            this.Name = "test_notes_ctrl";
            this.Text = "Test Notes control";
            ((System.ComponentModel.ISupportInitialize)(this.dummyView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private lw_common.ui.note_ctrl notes;
        private BrightIdeasSoftware.ObjectListView dummyView;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.Button undo;
    }
}

