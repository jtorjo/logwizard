namespace lw_common.ui {
    partial class test_event_logs_sizes_form {
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
            this.components = new System.ComponentModel.Container();
            this.close = new System.Windows.Forms.Button();
            this.readStatus = new System.Windows.Forms.TextBox();
            this.refresh = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.close.Location = new System.Drawing.Point(368, 231);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 23);
            this.close.TabIndex = 0;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // readStatus
            // 
            this.readStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.readStatus.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readStatus.Location = new System.Drawing.Point(12, 12);
            this.readStatus.Multiline = true;
            this.readStatus.Name = "readStatus";
            this.readStatus.ReadOnly = true;
            this.readStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.readStatus.Size = new System.Drawing.Size(426, 213);
            this.readStatus.TabIndex = 1;
            this.readStatus.WordWrap = false;
            // 
            // refresh
            // 
            this.refresh.Enabled = true;
            this.refresh.Interval = 250;
            this.refresh.Tick += new System.EventHandler(this.refresh_Tick);
            // 
            // test_event_logs_sizes_form
            // 
            this.AcceptButton = this.close;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.close;
            this.ClientSize = new System.Drawing.Size(450, 261);
            this.Controls.Add(this.readStatus);
            this.Controls.Add(this.close);
            this.Name = "test_event_logs_sizes_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Do Event Logs contain any events?";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button close;
        private System.Windows.Forms.TextBox readStatus;
        private System.Windows.Forms.Timer refresh;
    }
}