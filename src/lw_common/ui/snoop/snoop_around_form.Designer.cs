namespace lw_common.ui
{
    partial class snoop_around_form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            this.list = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.all = new System.Windows.Forms.Button();
            this.none = new System.Windows.Forms.Button();
            this.negate = new System.Windows.Forms.Button();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.clearFilter = new System.Windows.Forms.Button();
            this.run = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.Label();
            this.updateStatus = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.list)).BeginInit();
            this.SuspendLayout();
            // 
            // list
            // 
            this.list.AllColumns.Add(this.olvColumn1);
            this.list.AllColumns.Add(this.olvColumn4);
            this.list.AllColumns.Add(this.olvColumn2);
            this.list.AllColumns.Add(this.olvColumn3);
            this.list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn4,
            this.olvColumn2,
            this.olvColumn3});
            this.list.FullRowSelect = true;
            this.list.Location = new System.Drawing.Point(0, 0);
            this.list.MultiSelect = false;
            this.list.Name = "list";
            this.list.OwnerDraw = true;
            this.list.ShowGroups = false;
            this.list.Size = new System.Drawing.Size(731, 355);
            this.list.TabIndex = 0;
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "number";
            this.olvColumn1.IsEditable = false;
            this.olvColumn1.Searchable = false;
            this.olvColumn1.Sortable = false;
            this.olvColumn1.Text = " ";
            this.olvColumn1.Width = 30;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "is_checked";
            this.olvColumn4.CheckBoxes = true;
            this.olvColumn4.Text = " ";
            this.olvColumn4.Width = 20;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "value";
            this.olvColumn2.FillsFreeSpace = true;
            this.olvColumn2.IsEditable = false;
            this.olvColumn2.Sortable = false;
            this.olvColumn2.Text = "Value";
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "count";
            this.olvColumn3.IsEditable = false;
            this.olvColumn3.Sortable = false;
            this.olvColumn3.Text = "Count";
            this.olvColumn3.Width = 40;
            // 
            // all
            // 
            this.all.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.all.Location = new System.Drawing.Point(327, 361);
            this.all.Name = "all";
            this.all.Size = new System.Drawing.Size(90, 42);
            this.all.TabIndex = 1;
            this.all.Text = "All";
            this.tip.SetToolTip(this.all, "Select All Items");
            this.all.UseVisualStyleBackColor = true;
            this.all.Click += new System.EventHandler(this.all_Click);
            // 
            // none
            // 
            this.none.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.none.Location = new System.Drawing.Point(416, 361);
            this.none.Name = "none";
            this.none.Size = new System.Drawing.Size(90, 42);
            this.none.TabIndex = 2;
            this.none.Text = "None";
            this.tip.SetToolTip(this.none, "Clear All Items");
            this.none.UseVisualStyleBackColor = true;
            this.none.Click += new System.EventHandler(this.none_Click);
            // 
            // negate
            // 
            this.negate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.negate.Location = new System.Drawing.Point(505, 361);
            this.negate.Name = "negate";
            this.negate.Size = new System.Drawing.Size(90, 42);
            this.negate.TabIndex = 3;
            this.negate.Text = "Neg";
            this.tip.SetToolTip(this.negate, "Negate Existing Selection");
            this.negate.UseVisualStyleBackColor = true;
            this.negate.Click += new System.EventHandler(this.negate_Click);
            // 
            // tip
            // 
            this.tip.ShowAlways = true;
            // 
            // clearFilter
            // 
            this.clearFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearFilter.Image = global::lw_common.Properties.Resources.clear_filter;
            this.clearFilter.Location = new System.Drawing.Point(688, 361);
            this.clearFilter.Name = "clearFilter";
            this.clearFilter.Size = new System.Drawing.Size(44, 42);
            this.clearFilter.TabIndex = 5;
            this.tip.SetToolTip(this.clearFilter, "Clear This Filter");
            this.clearFilter.UseVisualStyleBackColor = true;
            this.clearFilter.Click += new System.EventHandler(this.clear_Click);
            // 
            // run
            // 
            this.run.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.run.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.run.Image = global::lw_common.Properties.Resources.filter;
            this.run.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.run.Location = new System.Drawing.Point(594, 361);
            this.run.Name = "run";
            this.run.Size = new System.Drawing.Size(95, 42);
            this.run.TabIndex = 4;
            this.run.Text = "Run";
            this.run.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tip.SetToolTip(this.run, "Run This Filter");
            this.run.UseVisualStyleBackColor = true;
            this.run.Click += new System.EventHandler(this.run_Click);
            // 
            // status
            // 
            this.status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.status.AutoSize = true;
            this.status.Location = new System.Drawing.Point(0, 400);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(18, 26);
            this.status.TabIndex = 6;
            this.status.Text = " ";
            // 
            // updateStatus
            // 
            this.updateStatus.Enabled = true;
            this.updateStatus.Interval = 250;
            this.updateStatus.Tick += new System.EventHandler(this.updateStatus_Tick);
            // 
            // snoop_around_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(732, 436);
            this.ControlBox = false;
            this.Controls.Add(this.status);
            this.Controls.Add(this.clearFilter);
            this.Controls.Add(this.run);
            this.Controls.Add(this.negate);
            this.Controls.Add(this.none);
            this.Controls.Add(this.all);
            this.Controls.Add(this.list);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "snoop_around_form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "snoop_around_form";
            this.Deactivate += new System.EventHandler(this.snoop_around_form_Deactivate);
            this.VisibleChanged += new System.EventHandler(this.snoop_around_form_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.list)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView list;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.Button all;
        private System.Windows.Forms.Button none;
        private System.Windows.Forms.Button negate;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.Button run;
        private System.Windows.Forms.Button clearFilter;
        private System.Windows.Forms.Label status;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private System.Windows.Forms.Timer updateStatus;
    }
}