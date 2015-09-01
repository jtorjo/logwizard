namespace LogWizard.ui {
    partial class settings_form {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(settings_form));
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.viewLine = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.viewIndex = new System.Windows.Forms.CheckBox();
            this.viewLineCount = new System.Windows.Forms.CheckBox();
            this.close = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Font: [Courier New, 9pt]";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.viewLine);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.viewIndex);
            this.groupBox1.Controls.Add(this.viewLineCount);
            this.groupBox1.Location = new System.Drawing.Point(12, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(582, 122);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Views";
            // 
            // viewLine
            // 
            this.viewLine.AutoSize = true;
            this.viewLine.Location = new System.Drawing.Point(16, 95);
            this.viewLine.Name = "viewLine";
            this.viewLine.Size = new System.Drawing.Size(205, 21);
            this.viewLine.TabIndex = 3;
            this.viewLine.Text = "Show Log Line Index in View";
            this.viewLine.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(355, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "To Each View\'s name, append the following information";
            // 
            // viewIndex
            // 
            this.viewIndex.AutoSize = true;
            this.viewIndex.Location = new System.Drawing.Point(16, 73);
            this.viewIndex.Name = "viewIndex";
            this.viewIndex.Size = new System.Drawing.Size(195, 21);
            this.viewIndex.TabIndex = 1;
            this.viewIndex.Text = "Show current index in View";
            this.viewIndex.UseVisualStyleBackColor = true;
            // 
            // viewLineCount
            // 
            this.viewLineCount.AutoSize = true;
            this.viewLineCount.Location = new System.Drawing.Point(16, 50);
            this.viewLineCount.Name = "viewLineCount";
            this.viewLineCount.Size = new System.Drawing.Size(210, 21);
            this.viewLineCount.TabIndex = 0;
            this.viewLineCount.Text = "Show number of lines in View";
            this.viewLineCount.UseVisualStyleBackColor = true;
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(516, 407);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 25);
            this.close.TabIndex = 3;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // settings_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 443);
            this.Controls.Add(this.close);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "settings_form";
            this.Text = "Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.settings_form_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox viewLine;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox viewIndex;
        private System.Windows.Forms.CheckBox viewLineCount;
        private System.Windows.Forms.Button close;

    }
}