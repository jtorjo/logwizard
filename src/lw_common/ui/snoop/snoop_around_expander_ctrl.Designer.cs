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
            this.expand = new System.Windows.Forms.Button();
            this.reapply = new System.Windows.Forms.CheckBox();
            this.refreshZorder = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // expand
            // 
            this.expand.BackgroundImage = global::lw_common.Properties.Resources.down;
            this.expand.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.expand.Cursor = System.Windows.Forms.Cursors.Hand;
            this.expand.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.expand.Location = new System.Drawing.Point(33, 4);
            this.expand.Margin = new System.Windows.Forms.Padding(4);
            this.expand.Name = "expand";
            this.expand.Size = new System.Drawing.Size(32, 32);
            this.expand.TabIndex = 3;
            this.expand.UseVisualStyleBackColor = true;
            this.expand.Click += new System.EventHandler(this.expand_Click);
            // 
            // reapply
            // 
            this.reapply.Appearance = System.Windows.Forms.Appearance.Button;
            this.reapply.BackgroundImage = global::lw_common.Properties.Resources.filter;
            this.reapply.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.reapply.Checked = true;
            this.reapply.CheckState = System.Windows.Forms.CheckState.Checked;
            this.reapply.Cursor = System.Windows.Forms.Cursors.Hand;
            this.reapply.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.reapply.Location = new System.Drawing.Point(1, 4);
            this.reapply.Margin = new System.Windows.Forms.Padding(4);
            this.reapply.Name = "reapply";
            this.reapply.Size = new System.Drawing.Size(32, 32);
            this.reapply.TabIndex = 2;
            this.reapply.UseVisualStyleBackColor = true;
            this.reapply.CheckedChanged += new System.EventHandler(this.reapply_CheckedChanged);
            // 
            // refreshZorder
            // 
            this.refreshZorder.Enabled = true;
            this.refreshZorder.Interval = 50;
            this.refreshZorder.Tick += new System.EventHandler(this.refreshZorder_Tick);
            // 
            // snoop_around_expander_ctrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.expand);
            this.Controls.Add(this.reapply);
            this.Name = "snoop_around_expander_ctrl";
            this.Size = new System.Drawing.Size(213, 39);
            this.VisibleChanged += new System.EventHandler(this.snoop_around_expander_ctrl_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button expand;
        internal System.Windows.Forms.CheckBox reapply;
        private System.Windows.Forms.Timer refreshZorder;
    }
}
