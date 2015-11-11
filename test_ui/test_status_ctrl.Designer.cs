namespace test_ui {
    partial class test_status_ctrl {
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
            this.animated = new lw_common.ui.animated_button();
            this.status = new lw_common.ui.status_ctrl();
            this.SuspendLayout();
            // 
            // animated
            // 
            this.animated.animate = true;
            this.animated.animate_count = 5;
            this.animated.animate_interval_ms = 5000;
            this.animated.animate_speed_ms = 100;
            this.animated.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.animated.Location = new System.Drawing.Point(302, 167);
            this.animated.Name = "animated";
            this.animated.Size = new System.Drawing.Size(124, 23);
            this.animated.TabIndex = 1;
            this.animated.Text = "What\'s up, doc?";
            this.animated.UseVisualStyleBackColor = true;
            // 
            // status
            // 
            this.status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.status.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.status.Location = new System.Drawing.Point(12, 27);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(613, 117);
            this.status.TabIndex = 0;
            this.status.MouseMove += new System.Windows.Forms.MouseEventHandler(this.status_MouseMove);
            // 
            // test_status_ctrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 261);
            this.Controls.Add(this.animated);
            this.Controls.Add(this.status);
            this.Name = "test_status_ctrl";
            this.Text = "test_status_ctrl";
            this.ResumeLayout(false);

        }

        #endregion

        private lw_common.ui.status_ctrl status;
        private lw_common.ui.animated_button animated;
    }
}