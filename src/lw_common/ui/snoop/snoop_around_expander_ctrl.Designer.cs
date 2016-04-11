namespace lw_common.ui.snoop {
    internal partial class snoop_around_expander_ctrl {
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
            this.refreshZorder = new System.Windows.Forms.Timer(this.components);
            this.expand = new System.Windows.Forms.Panel();
            this.reapplyFilter = new System.Windows.Forms.Panel();
            this.refreshIcons = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // refreshZorder
            // 
            this.refreshZorder.Enabled = true;
            this.refreshZorder.Interval = 50;
            this.refreshZorder.Tick += new System.EventHandler(this.refreshZorder_Tick);
            // 
            // expand
            // 
            this.expand.BackgroundImage = global::lw_common.Properties.Resources.down;
            this.expand.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.expand.Cursor = System.Windows.Forms.Cursors.Hand;
            this.expand.Location = new System.Drawing.Point(24, 0);
            this.expand.Name = "expand";
            this.expand.Size = new System.Drawing.Size(24, 24);
            this.expand.TabIndex = 4;
            this.expand.Click += new System.EventHandler(this.expand_Click_1);
            // 
            // reapplyFilter
            // 
            this.reapplyFilter.BackgroundImage = global::lw_common.Properties.Resources.filter;
            this.reapplyFilter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.reapplyFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.reapplyFilter.Location = new System.Drawing.Point(0, 0);
            this.reapplyFilter.Name = "reapplyFilter";
            this.reapplyFilter.Size = new System.Drawing.Size(24, 24);
            this.reapplyFilter.TabIndex = 5;
            this.reapplyFilter.Click += new System.EventHandler(this.reapplyFilter_Click);
            // 
            // refreshIcons
            // 
            this.refreshIcons.Enabled = true;
            this.refreshIcons.Tick += new System.EventHandler(this.refreshIcons_Tick);
            // 
            // snoop_around_expander_ctrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.reapplyFilter);
            this.Controls.Add(this.expand);
            this.Name = "snoop_around_expander_ctrl";
            this.Size = new System.Drawing.Size(213, 39);
            this.VisibleChanged += new System.EventHandler(this.snoop_around_expander_ctrl_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer refreshZorder;
        private System.Windows.Forms.Panel expand;
        private System.Windows.Forms.Panel reapplyFilter;
        private System.Windows.Forms.Timer refreshIcons;
    }
}
