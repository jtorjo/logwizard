namespace LogWizard.ui {
    partial class about_form {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(about_form));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.jt = new System.Windows.Forms.LinkLabel();
            this.link = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.jt2 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Author:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 52);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(208, 32);
            this.label2.TabIndex = 1;
            this.label2.Text = "License:           GPL v3\r\nSource code:  ";
            // 
            // jt
            // 
            this.jt.AutoSize = true;
            this.jt.Location = new System.Drawing.Point(161, 17);
            this.jt.Name = "jt";
            this.jt.Size = new System.Drawing.Size(88, 16);
            this.jt.TabIndex = 2;
            this.jt.TabStop = true;
            this.jt.Text = "John Torjo";
            this.jt.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.jt_LinkClicked);
            // 
            // link
            // 
            this.link.AutoSize = true;
            this.link.Location = new System.Drawing.Point(161, 68);
            this.link.Name = "link";
            this.link.Size = new System.Drawing.Size(328, 16);
            this.link.TabIndex = 3;
            this.link.TabStop = true;
            this.link.Text = "https://github.com/jtorjo/logwizard/wiki";
            this.link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(280, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Got suggestions? Drop    an email!";
            // 
            // jt2
            // 
            this.jt2.AutoSize = true;
            this.jt2.Location = new System.Drawing.Point(187, 108);
            this.jt2.Name = "jt2";
            this.jt2.Size = new System.Drawing.Size(24, 16);
            this.jt2.TabIndex = 5;
            this.jt2.TabStop = true;
            this.jt2.Text = "me";
            this.jt2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.jt2_LinkClicked);
            // 
            // about_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 136);
            this.Controls.Add(this.jt2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.link);
            this.Controls.Add(this.jt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "about_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About LogWizard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel jt;
        private System.Windows.Forms.LinkLabel link;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel jt2;
    }
}