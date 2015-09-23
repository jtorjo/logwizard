namespace LogWizard.ui {
    public partial class search_form {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(search_form));
            this.label1 = new System.Windows.Forms.Label();
            this.txt = new System.Windows.Forms.TextBox();
            this.mark = new System.Windows.Forms.CheckBox();
            this.fg = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.bg = new System.Windows.Forms.Label();
            this.ok = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.radioAutoRecognize = new System.Windows.Forms.RadioButton();
            this.radioText = new System.Windows.Forms.RadioButton();
            this.radioRegex = new System.Windows.Forms.RadioButton();
            this.caseSensitive = new System.Windows.Forms.CheckBox();
            this.fullWord = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Text";
            // 
            // txt
            // 
            this.txt.Location = new System.Drawing.Point(65, 8);
            this.txt.Name = "txt";
            this.txt.Size = new System.Drawing.Size(412, 23);
            this.txt.TabIndex = 1;
            this.txt.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // mark
            // 
            this.mark.AutoSize = true;
            this.mark.Checked = true;
            this.mark.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mark.Location = new System.Drawing.Point(65, 90);
            this.mark.Name = "mark";
            this.mark.Size = new System.Drawing.Size(276, 21);
            this.mark.TabIndex = 2;
            this.mark.Text = "Mark lines where text is found with color";
            this.mark.UseVisualStyleBackColor = true;
            // 
            // fg
            // 
            this.fg.BackColor = System.Drawing.SystemColors.Control;
            this.fg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fg.Location = new System.Drawing.Point(385, 90);
            this.fg.Name = "fg";
            this.fg.Size = new System.Drawing.Size(28, 23);
            this.fg.TabIndex = 3;
            this.fg.Click += new System.EventHandler(this.fg_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(355, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Fg";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(419, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Bg";
            // 
            // bg
            // 
            this.bg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bg.Location = new System.Drawing.Point(449, 91);
            this.bg.Name = "bg";
            this.bg.Size = new System.Drawing.Size(28, 23);
            this.bg.TabIndex = 5;
            this.bg.Click += new System.EventHandler(this.bg_Click);
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(321, 123);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 25);
            this.ok.TabIndex = 7;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(402, 123);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 25);
            this.cancel.TabIndex = 8;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // radioAutoRecognize
            // 
            this.radioAutoRecognize.AutoSize = true;
            this.radioAutoRecognize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioAutoRecognize.Location = new System.Drawing.Point(68, 38);
            this.radioAutoRecognize.Name = "radioAutoRecognize";
            this.radioAutoRecognize.Size = new System.Drawing.Size(96, 17);
            this.radioAutoRecognize.TabIndex = 9;
            this.radioAutoRecognize.TabStop = true;
            this.radioAutoRecognize.Text = "Auto recognize";
            this.radioAutoRecognize.UseVisualStyleBackColor = true;
            // 
            // radioText
            // 
            this.radioText.AutoSize = true;
            this.radioText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioText.Location = new System.Drawing.Point(358, 38);
            this.radioText.Name = "radioText";
            this.radioText.Size = new System.Drawing.Size(46, 17);
            this.radioText.TabIndex = 10;
            this.radioText.TabStop = true;
            this.radioText.Text = "Text";
            this.radioText.UseVisualStyleBackColor = true;
            // 
            // radioRegex
            // 
            this.radioRegex.AutoSize = true;
            this.radioRegex.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioRegex.Location = new System.Drawing.Point(422, 38);
            this.radioRegex.Name = "radioRegex";
            this.radioRegex.Size = new System.Drawing.Size(56, 17);
            this.radioRegex.TabIndex = 11;
            this.radioRegex.TabStop = true;
            this.radioRegex.Text = "Regex";
            this.radioRegex.UseVisualStyleBackColor = true;
            // 
            // caseSensitive
            // 
            this.caseSensitive.AutoSize = true;
            this.caseSensitive.Location = new System.Drawing.Point(65, 63);
            this.caseSensitive.Name = "caseSensitive";
            this.caseSensitive.Size = new System.Drawing.Size(121, 21);
            this.caseSensitive.TabIndex = 12;
            this.caseSensitive.Text = "Case-Sensitive";
            this.caseSensitive.UseVisualStyleBackColor = true;
            // 
            // fullWord
            // 
            this.fullWord.AutoSize = true;
            this.fullWord.Location = new System.Drawing.Point(192, 63);
            this.fullWord.Name = "fullWord";
            this.fullWord.Size = new System.Drawing.Size(87, 21);
            this.fullWord.TabIndex = 13;
            this.fullWord.Text = "Full Word";
            this.fullWord.UseVisualStyleBackColor = true;
            // 
            // search_form
            // 
            this.AcceptButton = this.ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(486, 156);
            this.Controls.Add(this.fullWord);
            this.Controls.Add(this.caseSensitive);
            this.Controls.Add(this.radioRegex);
            this.Controls.Add(this.radioText);
            this.Controls.Add(this.radioAutoRecognize);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.bg);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fg);
            this.Controls.Add(this.mark);
            this.Controls.Add(this.txt);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "search_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search For...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt;
        private System.Windows.Forms.CheckBox mark;
        private System.Windows.Forms.Label fg;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label bg;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.RadioButton radioAutoRecognize;
        private System.Windows.Forms.RadioButton radioText;
        private System.Windows.Forms.RadioButton radioRegex;
        private System.Windows.Forms.CheckBox caseSensitive;
        private System.Windows.Forms.CheckBox fullWord;
    }
}