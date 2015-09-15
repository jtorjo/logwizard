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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.makeTopmostOnRestart = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bringToTopOnRestart = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.toggleTopmost = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.syncColorsGrayOutNonActive = new System.Windows.Forms.CheckBox();
            this.syncColorsAllViews = new System.Windows.Forms.RadioButton();
            this.syncColorsCurView = new System.Windows.Forms.RadioButton();
            this.syncColorsNone = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.reset = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
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
            this.groupBox1.Location = new System.Drawing.Point(12, 208);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(380, 122);
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
            this.close.Location = new System.Drawing.Point(565, 488);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 25);
            this.close.TabIndex = 3;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.makeTopmostOnRestart);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.bringToTopOnRestart);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.toggleTopmost);
            this.groupBox2.Location = new System.Drawing.Point(12, 41);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(628, 161);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // makeTopmostOnRestart
            // 
            this.makeTopmostOnRestart.AutoSize = true;
            this.makeTopmostOnRestart.Enabled = false;
            this.makeTopmostOnRestart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.makeTopmostOnRestart.Location = new System.Drawing.Point(256, 87);
            this.makeTopmostOnRestart.Name = "makeTopmostOnRestart";
            this.makeTopmostOnRestart.Size = new System.Drawing.Size(185, 21);
            this.makeTopmostOnRestart.TabIndex = 4;
            this.makeTopmostOnRestart.Text = "... And Make Topmost";
            this.makeTopmostOnRestart.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(22, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(481, 39);
            this.label4.TabIndex = 3;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // bringToTopOnRestart
            // 
            this.bringToTopOnRestart.AutoSize = true;
            this.bringToTopOnRestart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bringToTopOnRestart.Location = new System.Drawing.Point(6, 87);
            this.bringToTopOnRestart.Name = "bringToTopOnRestart";
            this.bringToTopOnRestart.Size = new System.Drawing.Size(206, 21);
            this.bringToTopOnRestart.TabIndex = 2;
            this.bringToTopOnRestart.Text = "Bring To Top On Restart";
            this.bringToTopOnRestart.UseVisualStyleBackColor = true;
            this.bringToTopOnRestart.CheckedChanged += new System.EventHandler(this.bringToTopOnRestart_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(22, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(280, 26);
            this.label3.TabIndex = 1;
            this.label3.Text = "If selected, an icon is shown on top-left\r\nWhe blue - we\'re topmost. When gray - " +
    "we\'re not topmost.";
            // 
            // toggleTopmost
            // 
            this.toggleTopmost.AutoSize = true;
            this.toggleTopmost.Location = new System.Drawing.Point(10, 22);
            this.toggleTopmost.Name = "toggleTopmost";
            this.toggleTopmost.Size = new System.Drawing.Size(304, 21);
            this.toggleTopmost.TabIndex = 0;
            this.toggleTopmost.Text = "Allow toggling whether we\'re Topmost or not";
            this.toggleTopmost.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.syncColorsGrayOutNonActive);
            this.groupBox3.Controls.Add(this.syncColorsAllViews);
            this.groupBox3.Controls.Add(this.syncColorsCurView);
            this.groupBox3.Controls.Add(this.syncColorsNone);
            this.groupBox3.Location = new System.Drawing.Point(12, 336);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(628, 146);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Synchronize Colors in Full Log";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(218, 84);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(334, 52);
            this.label8.TabIndex = 7;
            this.label8.Text = resources.GetString("label8.Text");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(218, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(162, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Full Log Lines are shown in black";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(61, 121);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(146, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "from non Active Views";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(218, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(334, 26);
            this.label5.TabIndex = 4;
            this.label5.Text = "Lines from the Active View are shown in the same color in the Full Log\r\nLines tha" +
    "t are not in the Active View, are shown in dark gray.";
            // 
            // syncColorsGrayOutNonActive
            // 
            this.syncColorsGrayOutNonActive.AutoSize = true;
            this.syncColorsGrayOutNonActive.Location = new System.Drawing.Point(41, 101);
            this.syncColorsGrayOutNonActive.Name = "syncColorsGrayOutNonActive";
            this.syncColorsGrayOutNonActive.Size = new System.Drawing.Size(124, 21);
            this.syncColorsGrayOutNonActive.TabIndex = 3;
            this.syncColorsGrayOutNonActive.Text = "Gray out colors";
            this.syncColorsGrayOutNonActive.UseVisualStyleBackColor = true;
            // 
            // syncColorsAllViews
            // 
            this.syncColorsAllViews.AutoSize = true;
            this.syncColorsAllViews.Location = new System.Drawing.Point(19, 79);
            this.syncColorsAllViews.Name = "syncColorsAllViews";
            this.syncColorsAllViews.Size = new System.Drawing.Size(113, 21);
            this.syncColorsAllViews.TabIndex = 2;
            this.syncColorsAllViews.TabStop = true;
            this.syncColorsAllViews.Text = "With All Views";
            this.syncColorsAllViews.UseVisualStyleBackColor = true;
            // 
            // syncColorsCurView
            // 
            this.syncColorsCurView.AutoSize = true;
            this.syncColorsCurView.Location = new System.Drawing.Point(19, 47);
            this.syncColorsCurView.Name = "syncColorsCurView";
            this.syncColorsCurView.Size = new System.Drawing.Size(129, 21);
            this.syncColorsCurView.TabIndex = 1;
            this.syncColorsCurView.TabStop = true;
            this.syncColorsCurView.Text = "With Active View";
            this.syncColorsCurView.UseVisualStyleBackColor = true;
            // 
            // syncColorsNone
            // 
            this.syncColorsNone.AutoSize = true;
            this.syncColorsNone.Location = new System.Drawing.Point(19, 23);
            this.syncColorsNone.Name = "syncColorsNone";
            this.syncColorsNone.Size = new System.Drawing.Size(60, 21);
            this.syncColorsNone.TabIndex = 0;
            this.syncColorsNone.TabStop = true;
            this.syncColorsNone.Text = "None";
            this.syncColorsNone.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.reset);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Location = new System.Drawing.Point(398, 208);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(242, 122);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Reset All Settings";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(229, 51);
            this.label9.TabIndex = 0;
            this.label9.Text = "Resets all settings to their defaults.\r\nYou will lose your history,\r\nand LogWizar" +
    "d will restart.";
            // 
            // reset
            // 
            this.reset.Location = new System.Drawing.Point(153, 87);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(75, 25);
            this.reset.TabIndex = 1;
            this.reset.Text = "Reset";
            this.reset.UseVisualStyleBackColor = true;
            this.reset.Click += new System.EventHandler(this.reset_Click);
            // 
            // settings_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 523);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.close);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "settings_form";
            this.Text = "Preferences";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.settings_form_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox bringToTopOnRestart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox toggleTopmost;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox makeTopmostOnRestart;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton syncColorsAllViews;
        private System.Windows.Forms.RadioButton syncColorsCurView;
        private System.Windows.Forms.RadioButton syncColorsNone;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox syncColorsGrayOutNonActive;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button reset;
        private System.Windows.Forms.Label label9;

    }
}