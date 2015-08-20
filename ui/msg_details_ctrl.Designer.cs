namespace LogWizard {
    partial class msg_details_ctrl {
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
            this.txt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txt
            // 
            this.txt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt.Location = new System.Drawing.Point(3, 3);
            this.txt.Multiline = true;
            this.txt.Name = "txt";
            this.txt.ReadOnly = true;
            this.txt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt.Size = new System.Drawing.Size(646, 165);
            this.txt.TabIndex = 0;
            // 
            // msg_details_ctrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txt);
            this.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "msg_details_ctrl";
            this.Size = new System.Drawing.Size(652, 170);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt;

    }
}
