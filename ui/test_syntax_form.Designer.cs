namespace LogWizard.ui {
    partial class test_syntax_form {
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
            this.lines = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.result = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn9 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn10 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn11 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn12 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn13 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn14 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn15 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.test = new System.Windows.Forms.Button();
            this.use = new System.Windows.Forms.Button();
            this.help = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.syntax = new System.Windows.Forms.TextBox();
            this.cancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.result)).BeginInit();
            this.SuspendLayout();
            // 
            // lines
            // 
            this.lines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lines.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lines.Location = new System.Drawing.Point(16, 73);
            this.lines.Margin = new System.Windows.Forms.Padding(4);
            this.lines.Multiline = true;
            this.lines.Name = "lines";
            this.lines.ReadOnly = true;
            this.lines.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.lines.Size = new System.Drawing.Size(981, 269);
            this.lines.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(713, 34);
            this.label1.TabIndex = 1;
            this.label1.Text = "To test, copy the beginning at least 10 lines of your log, and come back here. \r\n" +
    "We will automatically paste them and try to guess the syntax. You can then tweak" +
    " it and instantly see the results.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(624, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "The Log Syntax is the syntax of each line of your log. This is how we split each " +
    "line into columns.";
            // 
            // result
            // 
            this.result.AllColumns.Add(this.olvColumn1);
            this.result.AllColumns.Add(this.olvColumn2);
            this.result.AllColumns.Add(this.olvColumn3);
            this.result.AllColumns.Add(this.olvColumn4);
            this.result.AllColumns.Add(this.olvColumn5);
            this.result.AllColumns.Add(this.olvColumn6);
            this.result.AllColumns.Add(this.olvColumn7);
            this.result.AllColumns.Add(this.olvColumn8);
            this.result.AllColumns.Add(this.olvColumn9);
            this.result.AllColumns.Add(this.olvColumn10);
            this.result.AllColumns.Add(this.olvColumn11);
            this.result.AllColumns.Add(this.olvColumn12);
            this.result.AllColumns.Add(this.olvColumn13);
            this.result.AllColumns.Add(this.olvColumn14);
            this.result.AllColumns.Add(this.olvColumn15);
            this.result.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.result.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3,
            this.olvColumn4,
            this.olvColumn5,
            this.olvColumn6,
            this.olvColumn7,
            this.olvColumn8,
            this.olvColumn9,
            this.olvColumn10,
            this.olvColumn11,
            this.olvColumn12,
            this.olvColumn13,
            this.olvColumn14,
            this.olvColumn15});
            this.result.FullRowSelect = true;
            this.result.Location = new System.Drawing.Point(16, 402);
            this.result.MultiSelect = false;
            this.result.Name = "result";
            this.result.ShowGroups = false;
            this.result.Size = new System.Drawing.Size(981, 220);
            this.result.TabIndex = 3;
            this.result.UseCompatibleStateImageBehavior = false;
            this.result.View = System.Windows.Forms.View.Details;
            this.result.SelectedIndexChanged += new System.EventHandler(this.result_SelectedIndexChanged);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "t0";
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "t1";
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "t2";
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "t3";
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "t4";
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "t5";
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "t6";
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "t7";
            // 
            // olvColumn9
            // 
            this.olvColumn9.AspectName = "t8";
            // 
            // olvColumn10
            // 
            this.olvColumn10.AspectName = "t9";
            // 
            // olvColumn11
            // 
            this.olvColumn11.AspectName = "t10";
            // 
            // olvColumn12
            // 
            this.olvColumn12.AspectName = "t11";
            // 
            // olvColumn13
            // 
            this.olvColumn13.AspectName = "t12";
            // 
            // olvColumn14
            // 
            this.olvColumn14.AspectName = "t13";
            // 
            // olvColumn15
            // 
            this.olvColumn15.AspectName = "t14";
            // 
            // test
            // 
            this.test.Location = new System.Drawing.Point(868, 9);
            this.test.Name = "test";
            this.test.Size = new System.Drawing.Size(68, 26);
            this.test.TabIndex = 4;
            this.test.Text = "Test";
            this.test.UseVisualStyleBackColor = true;
            this.test.Click += new System.EventHandler(this.test_Click);
            // 
            // use
            // 
            this.use.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.use.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.use.Location = new System.Drawing.Point(922, 351);
            this.use.Name = "use";
            this.use.Size = new System.Drawing.Size(75, 44);
            this.use.TabIndex = 5;
            this.use.Text = "Use it!";
            this.use.UseVisualStyleBackColor = true;
            this.use.Click += new System.EventHandler(this.use_Click);
            // 
            // help
            // 
            this.help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.help.AutoSize = true;
            this.help.Location = new System.Drawing.Point(877, 356);
            this.help.Name = "help";
            this.help.Size = new System.Drawing.Size(37, 17);
            this.help.TabIndex = 6;
            this.help.TabStop = true;
            this.help.Text = "Help";
            this.help.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.help_LinkClicked);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 355);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Syntax:";
            // 
            // syntax
            // 
            this.syntax.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.syntax.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.syntax.Location = new System.Drawing.Point(80, 353);
            this.syntax.Name = "syntax";
            this.syntax.Size = new System.Drawing.Size(791, 21);
            this.syntax.TabIndex = 8;
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(942, 10);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(68, 26);
            this.cancel.TabIndex = 9;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(81, 377);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(220, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Press Enter to Test, Escape to exit this dialog.";
            // 
            // test_syntax_form
            // 
            this.AcceptButton = this.test;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(1015, 634);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.syntax);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.help);
            this.Controls.Add(this.use);
            this.Controls.Add(this.test);
            this.Controls.Add(this.result);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lines);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "test_syntax_form";
            this.Text = "Test Log Syntax";
            this.Activated += new System.EventHandler(this.test_syntax_form_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.result)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox lines;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private BrightIdeasSoftware.ObjectListView result;
        private System.Windows.Forms.Button test;
        private System.Windows.Forms.Button use;
        private System.Windows.Forms.LinkLabel help;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox syntax;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private BrightIdeasSoftware.OLVColumn olvColumn9;
        private BrightIdeasSoftware.OLVColumn olvColumn10;
        private BrightIdeasSoftware.OLVColumn olvColumn11;
        private BrightIdeasSoftware.OLVColumn olvColumn12;
        private BrightIdeasSoftware.OLVColumn olvColumn13;
        private BrightIdeasSoftware.OLVColumn olvColumn14;
        private BrightIdeasSoftware.OLVColumn olvColumn15;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label label4;
    }
}