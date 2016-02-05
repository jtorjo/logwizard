namespace lw_common.ui {
    public partial class about_form {
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
            this.stableGroup = new System.Windows.Forms.GroupBox();
            this.downloadStable32 = new System.Windows.Forms.LinkLabel();
            this.downloadStable64 = new System.Windows.Forms.LinkLabel();
            this.stable = new System.Windows.Forms.TextBox();
            this.betaGroup = new System.Windows.Forms.GroupBox();
            this.downloadBeta32 = new System.Windows.Forms.LinkLabel();
            this.downloadBeta64 = new System.Windows.Forms.LinkLabel();
            this.beta = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.currentGroup = new System.Windows.Forms.GroupBox();
            this.current = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.stableGroup.SuspendLayout();
            this.betaGroup.SuspendLayout();
            this.currentGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(103, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Author:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(101, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Source code:  ";
            // 
            // jt
            // 
            this.jt.AutoSize = true;
            this.jt.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.jt.Location = new System.Drawing.Point(251, 11);
            this.jt.Name = "jt";
            this.jt.Size = new System.Drawing.Size(75, 16);
            this.jt.TabIndex = 2;
            this.jt.TabStop = true;
            this.jt.Text = "John Torjo";
            this.jt.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.jt_LinkClicked);
            // 
            // link
            // 
            this.link.AutoSize = true;
            this.link.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.link.Location = new System.Drawing.Point(251, 32);
            this.link.Name = "link";
            this.link.Size = new System.Drawing.Size(210, 16);
            this.link.TabIndex = 3;
            this.link.TabStop = true;
            this.link.Text = "https://github.com/jtorjo/logwizard";
            this.link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(102, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(216, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Got suggestions? Drop       an email!";
            // 
            // jt2
            // 
            this.jt2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.jt2.Location = new System.Drawing.Point(234, 58);
            this.jt2.Name = "jt2";
            this.jt2.Size = new System.Drawing.Size(26, 16);
            this.jt2.TabIndex = 5;
            this.jt2.TabStop = true;
            this.jt2.Text = "me";
            this.jt2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.jt2_LinkClicked);
            // 
            // stableGroup
            // 
            this.stableGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stableGroup.Controls.Add(this.downloadStable32);
            this.stableGroup.Controls.Add(this.downloadStable64);
            this.stableGroup.Controls.Add(this.stable);
            this.stableGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stableGroup.Location = new System.Drawing.Point(9, 88);
            this.stableGroup.Name = "stableGroup";
            this.stableGroup.Size = new System.Drawing.Size(554, 128);
            this.stableGroup.TabIndex = 6;
            this.stableGroup.TabStop = false;
            this.stableGroup.Text = "Latest Stable";
            // 
            // downloadStable32
            // 
            this.downloadStable32.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadStable32.AutoSize = true;
            this.downloadStable32.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadStable32.Location = new System.Drawing.Point(295, 104);
            this.downloadStable32.Name = "downloadStable32";
            this.downloadStable32.Size = new System.Drawing.Size(121, 19);
            this.downloadStable32.TabIndex = 2;
            this.downloadStable32.TabStop = true;
            this.downloadStable32.Text = "Download (32-bit)";
            this.downloadStable32.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.downloadStable32_LinkClicked);
            // 
            // downloadStable64
            // 
            this.downloadStable64.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadStable64.AutoSize = true;
            this.downloadStable64.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadStable64.Location = new System.Drawing.Point(427, 104);
            this.downloadStable64.Name = "downloadStable64";
            this.downloadStable64.Size = new System.Drawing.Size(121, 19);
            this.downloadStable64.TabIndex = 1;
            this.downloadStable64.TabStop = true;
            this.downloadStable64.Text = "Download (64-bit)";
            this.downloadStable64.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.downloadStable64_LinkClicked);
            // 
            // stable
            // 
            this.stable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stable.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stable.Location = new System.Drawing.Point(7, 22);
            this.stable.Multiline = true;
            this.stable.Name = "stable";
            this.stable.ReadOnly = true;
            this.stable.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.stable.Size = new System.Drawing.Size(541, 79);
            this.stable.TabIndex = 0;
            // 
            // betaGroup
            // 
            this.betaGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.betaGroup.Controls.Add(this.downloadBeta32);
            this.betaGroup.Controls.Add(this.downloadBeta64);
            this.betaGroup.Controls.Add(this.beta);
            this.betaGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.betaGroup.Location = new System.Drawing.Point(9, 219);
            this.betaGroup.Name = "betaGroup";
            this.betaGroup.Size = new System.Drawing.Size(554, 124);
            this.betaGroup.TabIndex = 7;
            this.betaGroup.TabStop = false;
            this.betaGroup.Text = "Latest Beta";
            // 
            // downloadBeta32
            // 
            this.downloadBeta32.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadBeta32.AutoSize = true;
            this.downloadBeta32.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadBeta32.Location = new System.Drawing.Point(295, 101);
            this.downloadBeta32.Name = "downloadBeta32";
            this.downloadBeta32.Size = new System.Drawing.Size(121, 19);
            this.downloadBeta32.TabIndex = 3;
            this.downloadBeta32.TabStop = true;
            this.downloadBeta32.Text = "Download (32-bit)";
            this.downloadBeta32.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.downloadBeta32_LinkClicked);
            // 
            // downloadBeta64
            // 
            this.downloadBeta64.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadBeta64.AutoSize = true;
            this.downloadBeta64.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadBeta64.Location = new System.Drawing.Point(426, 101);
            this.downloadBeta64.Name = "downloadBeta64";
            this.downloadBeta64.Size = new System.Drawing.Size(121, 19);
            this.downloadBeta64.TabIndex = 2;
            this.downloadBeta64.TabStop = true;
            this.downloadBeta64.Text = "Download (64-bit)";
            this.downloadBeta64.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.downloadBeta64_LinkClicked);
            // 
            // beta
            // 
            this.beta.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.beta.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.beta.Location = new System.Drawing.Point(6, 23);
            this.beta.Multiline = true;
            this.beta.Name = "beta";
            this.beta.ReadOnly = true;
            this.beta.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.beta.Size = new System.Drawing.Size(541, 75);
            this.beta.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(367, 11);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "License:           GPL3";
            // 
            // currentGroup
            // 
            this.currentGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.currentGroup.Controls.Add(this.current);
            this.currentGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentGroup.Location = new System.Drawing.Point(9, 348);
            this.currentGroup.Name = "currentGroup";
            this.currentGroup.Size = new System.Drawing.Size(554, 218);
            this.currentGroup.TabIndex = 9;
            this.currentGroup.TabStop = false;
            this.currentGroup.Text = "History";
            // 
            // current
            // 
            this.current.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.current.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.current.Location = new System.Drawing.Point(6, 28);
            this.current.Multiline = true;
            this.current.Name = "current";
            this.current.ReadOnly = true;
            this.current.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.current.Size = new System.Drawing.Size(541, 184);
            this.current.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Image = global::lw_common.Properties.Resources.buggy;
            this.pictureBox1.Location = new System.Drawing.Point(1, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(80, 80);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // about_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(575, 574);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.currentGroup);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.betaGroup);
            this.Controls.Add(this.stableGroup);
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
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About LogWizard";
            this.stableGroup.ResumeLayout(false);
            this.stableGroup.PerformLayout();
            this.betaGroup.ResumeLayout(false);
            this.betaGroup.PerformLayout();
            this.currentGroup.ResumeLayout(false);
            this.currentGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private System.Windows.Forms.GroupBox stableGroup;
        private System.Windows.Forms.GroupBox betaGroup;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox currentGroup;
        private System.Windows.Forms.TextBox stable;
        private System.Windows.Forms.TextBox beta;
        private System.Windows.Forms.TextBox current;
        private System.Windows.Forms.LinkLabel downloadStable64;
        private System.Windows.Forms.LinkLabel downloadBeta64;
        private System.Windows.Forms.LinkLabel downloadStable32;
        private System.Windows.Forms.LinkLabel downloadBeta32;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}