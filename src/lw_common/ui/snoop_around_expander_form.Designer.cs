namespace lw_common.ui
{
    internal partial class snoop_around_expander_form
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
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.reapply = new System.Windows.Forms.CheckBox();
            this.expand = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tip
            // 
            this.tip.ShowAlways = true;
            // 
            // reapply
            // 
            this.reapply.Appearance = System.Windows.Forms.Appearance.Button;
            this.reapply.BackgroundImage = global::lw_common.Properties.Resources.filter;
            this.reapply.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.reapply.Cursor = System.Windows.Forms.Cursors.Hand;
            this.reapply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reapply.Location = new System.Drawing.Point(0, 0);
            this.reapply.Margin = new System.Windows.Forms.Padding(4);
            this.reapply.Name = "reapply";
            this.reapply.Size = new System.Drawing.Size(32, 32);
            this.reapply.TabIndex = 0;
            this.tip.SetToolTip(this.reapply, "Click to Toggle the Last Filter you\'ve set.");
            this.reapply.UseVisualStyleBackColor = true;
            this.reapply.CheckedChanged += new System.EventHandler(this.reapply_CheckedChanged);
            // 
            // expand
            // 
            this.expand.BackgroundImage = global::lw_common.Properties.Resources.down;
            this.expand.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.expand.Cursor = System.Windows.Forms.Cursors.Hand;
            this.expand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.expand.Location = new System.Drawing.Point(32, 0);
            this.expand.Margin = new System.Windows.Forms.Padding(4);
            this.expand.Name = "expand";
            this.expand.Size = new System.Drawing.Size(32, 32);
            this.expand.TabIndex = 1;
            this.tip.SetToolTip(this.expand, "Click to Snoop Around\r\n(Filter by values from this column)");
            this.expand.UseVisualStyleBackColor = true;
            this.expand.Click += new System.EventHandler(this.expand_Click);
            // 
            // snoop_around_expander_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(232, 45);
            this.ControlBox = false;
            this.Controls.Add(this.expand);
            this.Controls.Add(this.reapply);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "snoop_around_expander_form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "snoop_around_expander_form";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox reapply;
        private System.Windows.Forms.Button expand;
        private System.Windows.Forms.ToolTip tip;
    }
}