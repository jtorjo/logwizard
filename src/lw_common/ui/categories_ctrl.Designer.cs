namespace lw_common.ui {
    partial class categories_ctrl {
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
            this.categoryTypes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.categories = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.previewLabel = new System.Windows.Forms.Label();
            this.isRunning = new System.Windows.Forms.CheckBox();
            this.preview = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.errorStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.categories)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
            this.SuspendLayout();
            // 
            // categoryTypes
            // 
            this.categoryTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.categoryTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.categoryTypes.FormattingEnabled = true;
            this.categoryTypes.Location = new System.Drawing.Point(77, 5);
            this.categoryTypes.Margin = new System.Windows.Forms.Padding(4);
            this.categoryTypes.Name = "categoryTypes";
            this.categoryTypes.Size = new System.Drawing.Size(495, 24);
            this.categoryTypes.TabIndex = 0;
            this.categoryTypes.SelectedIndexChanged += new System.EventHandler(this.categoryTypes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Category";
            // 
            // categories
            // 
            this.categories.AllColumns.Add(this.olvColumn1);
            this.categories.AllColumns.Add(this.olvColumn2);
            this.categories.AllColumns.Add(this.olvColumn3);
            this.categories.AllColumns.Add(this.olvColumn4);
            this.categories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.categories.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3,
            this.olvColumn4});
            this.categories.Location = new System.Drawing.Point(4, 36);
            this.categories.Name = "categories";
            this.categories.OwnerDraw = true;
            this.categories.ShowGroups = false;
            this.categories.Size = new System.Drawing.Size(568, 452);
            this.categories.TabIndex = 2;
            this.categories.UseCellFormatEvents = true;
            this.categories.UseCompatibleStateImageBehavior = false;
            this.categories.UseCustomSelectionColors = true;
            this.categories.View = System.Windows.Forms.View.Details;
            this.categories.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.categories_CellClick);
            this.categories.CellOver += new System.EventHandler<BrightIdeasSoftware.CellOverEventArgs>(this.categories_CellOver);
            this.categories.CellToolTipShowing += new System.EventHandler<BrightIdeasSoftware.ToolTipShowingEventArgs>(this.objectListView1_CellToolTipShowing);
            this.categories.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.categories_FormatCell);
            this.categories.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.categories_FormatRow);
            this.categories.SelectedIndexChanged += new System.EventHandler(this.categories_SelectedIndexChanged);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "name";
            this.olvColumn1.FillsFreeSpace = true;
            this.olvColumn1.IsEditable = false;
            this.olvColumn1.Text = "Name";
            this.olvColumn1.Width = 150;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "color";
            this.olvColumn2.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn2.Text = "Color";
            this.olvColumn2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn2.Width = 80;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "same_color";
            this.olvColumn3.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn3.Text = "Same";
            this.olvColumn3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn3.Width = 80;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "this_color";
            this.olvColumn4.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn4.Text = "This (Sel)";
            this.olvColumn4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn4.Width = 80;
            // 
            // previewLabel
            // 
            this.previewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.previewLabel.AutoSize = true;
            this.previewLabel.Location = new System.Drawing.Point(3, 491);
            this.previewLabel.Name = "previewLabel";
            this.previewLabel.Size = new System.Drawing.Size(57, 17);
            this.previewLabel.TabIndex = 4;
            this.previewLabel.Text = "Preview";
            // 
            // isRunning
            // 
            this.isRunning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.isRunning.Appearance = System.Windows.Forms.Appearance.Button;
            this.isRunning.Location = new System.Drawing.Point(505, 681);
            this.isRunning.Name = "isRunning";
            this.isRunning.Size = new System.Drawing.Size(67, 26);
            this.isRunning.TabIndex = 5;
            this.isRunning.Text = "Start";
            this.isRunning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.isRunning.UseVisualStyleBackColor = true;
            this.isRunning.CheckedChanged += new System.EventHandler(this.isRunning_CheckedChanged);
            // 
            // preview
            // 
            this.preview.AllColumns.Add(this.olvColumn5);
            this.preview.AllColumns.Add(this.olvColumn6);
            this.preview.AllColumns.Add(this.olvColumn7);
            this.preview.AllColumns.Add(this.olvColumn8);
            this.preview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.preview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn5,
            this.olvColumn6,
            this.olvColumn7,
            this.olvColumn8});
            this.preview.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.preview.FullRowSelect = true;
            this.preview.Location = new System.Drawing.Point(4, 511);
            this.preview.Name = "preview";
            this.preview.OwnerDraw = true;
            this.preview.ShowGroups = false;
            this.preview.Size = new System.Drawing.Size(495, 193);
            this.preview.TabIndex = 6;
            this.preview.UseCellFormatEvents = true;
            this.preview.UseCompatibleStateImageBehavior = false;
            this.preview.View = System.Windows.Forms.View.Details;
            this.preview.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.preview_FormatRow);
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "date";
            this.olvColumn5.IsEditable = false;
            this.olvColumn5.Text = "Time";
            this.olvColumn5.Width = 95;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "level";
            this.olvColumn6.IsEditable = false;
            this.olvColumn6.Text = "Level";
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "thread";
            this.olvColumn7.IsEditable = false;
            this.olvColumn7.Text = "Category";
            this.olvColumn7.Width = 80;
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "message";
            this.olvColumn8.FillsFreeSpace = true;
            this.olvColumn8.IsEditable = false;
            this.olvColumn8.Text = "Message";
            // 
            // errorStatus
            // 
            this.errorStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errorStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorStatus.Location = new System.Drawing.Point(3, 66);
            this.errorStatus.Name = "errorStatus";
            this.errorStatus.Size = new System.Drawing.Size(569, 95);
            this.errorStatus.TabIndex = 7;
            this.errorStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // categories_ctrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.errorStatus);
            this.Controls.Add(this.preview);
            this.Controls.Add(this.isRunning);
            this.Controls.Add(this.previewLabel);
            this.Controls.Add(this.categories);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.categoryTypes);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "categories_ctrl";
            this.Size = new System.Drawing.Size(580, 710);
            this.Load += new System.EventHandler(this.categories_ctrl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.categories)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox categoryTypes;
        private System.Windows.Forms.Label label1;
        private BrightIdeasSoftware.ObjectListView categories;
        private System.Windows.Forms.Label previewLabel;
        private System.Windows.Forms.CheckBox isRunning;
        private BrightIdeasSoftware.ObjectListView preview;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private System.Windows.Forms.Label errorStatus;
    }
}
