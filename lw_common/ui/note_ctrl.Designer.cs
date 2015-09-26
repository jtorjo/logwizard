namespace lw_common.ui {
    partial class note_ctrl {
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
            this.notesCtrl = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.addNoteToLineLabel = new System.Windows.Forms.Label();
            this.addNoteToLine = new System.Windows.Forms.Label();
            this.showDeletedLines = new System.Windows.Forms.CheckBox();
            this.curNote = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.selectColor = new System.Windows.Forms.Button();
            this.sortByTime = new System.Windows.Forms.RadioButton();
            this.sortByLine = new System.Windows.Forms.RadioButton();
            this.saveTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.notesCtrl)).BeginInit();
            this.SuspendLayout();
            // 
            // notesCtrl
            // 
            this.notesCtrl.AllColumns.Add(this.olvColumn1);
            this.notesCtrl.AllColumns.Add(this.olvColumn2);
            this.notesCtrl.AllColumns.Add(this.olvColumn3);
            this.notesCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.notesCtrl.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3});
            this.notesCtrl.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notesCtrl.FullRowSelect = true;
            this.notesCtrl.HideSelection = false;
            this.notesCtrl.Location = new System.Drawing.Point(0, 0);
            this.notesCtrl.Margin = new System.Windows.Forms.Padding(4);
            this.notesCtrl.MultiSelect = false;
            this.notesCtrl.Name = "notesCtrl";
            this.notesCtrl.ShowGroups = false;
            this.notesCtrl.Size = new System.Drawing.Size(431, 443);
            this.notesCtrl.TabIndex = 0;
            this.notesCtrl.UseCompatibleStateImageBehavior = false;
            this.notesCtrl.View = System.Windows.Forms.View.Details;
            this.notesCtrl.SelectedIndexChanged += new System.EventHandler(this.notesCtrl_SelectedIndexChanged);
            this.notesCtrl.Enter += new System.EventHandler(this.notesCtrl_Enter);
            this.notesCtrl.Leave += new System.EventHandler(this.notesCtrl_Leave);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "the_idx";
            this.olvColumn1.IsEditable = false;
            this.olvColumn1.Text = " ";
            this.olvColumn1.Width = 40;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "the_line";
            this.olvColumn2.IsEditable = false;
            this.olvColumn2.Text = "Line";
            this.olvColumn2.Width = 50;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "the_text";
            this.olvColumn3.FillsFreeSpace = true;
            this.olvColumn3.IsEditable = false;
            this.olvColumn3.Text = "Note";
            // 
            // addNoteToLineLabel
            // 
            this.addNoteToLineLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addNoteToLineLabel.AutoSize = true;
            this.addNoteToLineLabel.Location = new System.Drawing.Point(-3, 478);
            this.addNoteToLineLabel.Name = "addNoteToLineLabel";
            this.addNoteToLineLabel.Size = new System.Drawing.Size(114, 17);
            this.addNoteToLineLabel.TabIndex = 1;
            this.addNoteToLineLabel.Text = "Add Note to Line";
            // 
            // addNoteToLine
            // 
            this.addNoteToLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addNoteToLine.AutoSize = true;
            this.addNoteToLine.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addNoteToLine.Location = new System.Drawing.Point(131, 478);
            this.addNoteToLine.Name = "addNoteToLine";
            this.addNoteToLine.Size = new System.Drawing.Size(18, 17);
            this.addNoteToLine.TabIndex = 2;
            this.addNoteToLine.Text = "[]";
            // 
            // showDeletedLines
            // 
            this.showDeletedLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.showDeletedLines.AutoSize = true;
            this.showDeletedLines.Location = new System.Drawing.Point(283, 481);
            this.showDeletedLines.Name = "showDeletedLines";
            this.showDeletedLines.Size = new System.Drawing.Size(152, 21);
            this.showDeletedLines.TabIndex = 3;
            this.showDeletedLines.Text = "Show Deleted Lines";
            this.showDeletedLines.UseVisualStyleBackColor = true;
            this.showDeletedLines.CheckedChanged += new System.EventHandler(this.showDeletedLines_CheckedChanged);
            // 
            // curNote
            // 
            this.curNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.curNote.Location = new System.Drawing.Point(23, 502);
            this.curNote.Multiline = true;
            this.curNote.Name = "curNote";
            this.curNote.Size = new System.Drawing.Size(406, 151);
            this.curNote.TabIndex = 4;
            this.curNote.KeyDown += new System.Windows.Forms.KeyEventHandler(this.curNote_KeyDown);
            this.curNote.KeyUp += new System.Windows.Forms.KeyEventHandler(this.curNote_KeyUp);
            this.curNote.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.curNote_PreviewKeyDown);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(197, 657);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(232, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "To add a note, write it, and press Enter. That\'s it!";
            // 
            // selectColor
            // 
            this.selectColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.selectColor.BackgroundImage = global::lw_common.Properties.Resources.eyedropper;
            this.selectColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.selectColor.Location = new System.Drawing.Point(0, 502);
            this.selectColor.Name = "selectColor";
            this.selectColor.Size = new System.Drawing.Size(22, 22);
            this.selectColor.TabIndex = 6;
            this.selectColor.UseVisualStyleBackColor = true;
            // 
            // sortByTime
            // 
            this.sortByTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sortByTime.Appearance = System.Windows.Forms.Appearance.Button;
            this.sortByTime.AutoSize = true;
            this.sortByTime.Enabled = false;
            this.sortByTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sortByTime.Location = new System.Drawing.Point(283, 450);
            this.sortByTime.Name = "sortByTime";
            this.sortByTime.Size = new System.Drawing.Size(74, 23);
            this.sortByTime.TabIndex = 7;
            this.sortByTime.Text = "Sort by Time";
            this.sortByTime.UseVisualStyleBackColor = true;
            // 
            // sortByLine
            // 
            this.sortByLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sortByLine.Appearance = System.Windows.Forms.Appearance.Button;
            this.sortByLine.AutoSize = true;
            this.sortByLine.Checked = true;
            this.sortByLine.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sortByLine.Location = new System.Drawing.Point(358, 450);
            this.sortByLine.Name = "sortByLine";
            this.sortByLine.Size = new System.Drawing.Size(73, 23);
            this.sortByLine.TabIndex = 8;
            this.sortByLine.TabStop = true;
            this.sortByLine.Text = "Sort by Line";
            this.sortByLine.UseVisualStyleBackColor = true;
            // 
            // saveTimer
            // 
            this.saveTimer.Enabled = true;
            this.saveTimer.Interval = 1000;
            this.saveTimer.Tick += new System.EventHandler(this.saveTimer_Tick);
            // 
            // note_ctrl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.sortByLine);
            this.Controls.Add(this.sortByTime);
            this.Controls.Add(this.selectColor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.curNote);
            this.Controls.Add(this.showDeletedLines);
            this.Controls.Add(this.addNoteToLine);
            this.Controls.Add(this.addNoteToLineLabel);
            this.Controls.Add(this.notesCtrl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "note_ctrl";
            this.Size = new System.Drawing.Size(432, 670);
            this.Load += new System.EventHandler(this.note_ctrl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.notesCtrl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView notesCtrl;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.Label addNoteToLineLabel;
        private System.Windows.Forms.Label addNoteToLine;
        private System.Windows.Forms.CheckBox showDeletedLines;
        private System.Windows.Forms.TextBox curNote;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button selectColor;
        private System.Windows.Forms.RadioButton sortByTime;
        private System.Windows.Forms.RadioButton sortByLine;
        private System.Windows.Forms.Timer saveTimer;
    }
}
