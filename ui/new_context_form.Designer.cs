namespace LogWizard
{
    partial class new_context_form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being dimmed.
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
            this.label1 = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.TextBox();
            this.ok = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.basedOnExisting = new System.Windows.Forms.CheckBox();
            this.matchType = new System.Windows.Forms.ComboBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Template Name";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(102, 6);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(179, 20);
            this.name.TabIndex = 1;
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(125, 158);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 2;
            this.ok.Text = "Ok";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(206, 158);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 3;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // basedOnExisting
            // 
            this.basedOnExisting.AutoSize = true;
            this.basedOnExisting.Checked = true;
            this.basedOnExisting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.basedOnExisting.Location = new System.Drawing.Point(40, 35);
            this.basedOnExisting.Name = "basedOnExisting";
            this.basedOnExisting.Size = new System.Drawing.Size(242, 17);
            this.basedOnExisting.TabIndex = 4;
            this.basedOnExisting.Text = "Copy Views and Filters From Current Template";
            this.basedOnExisting.UseVisualStyleBackColor = true;
            // 
            // matchType
            // 
            this.matchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.matchType.Enabled = false;
            this.matchType.FormattingEnabled = true;
            this.matchType.Items.AddRange(new object[] {
            "File Name",
            "Full Path Name"});
            this.matchType.Location = new System.Drawing.Point(108, 58);
            this.matchType.Name = "matchType";
            this.matchType.Size = new System.Drawing.Size(171, 21);
            this.matchType.TabIndex = 5;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Pattern Matching";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(239, 52);
            this.label3.TabIndex = 7;
            this.label3.Text = "Note: \r\nChoose the name carefully, since any File Name \r\ncontaining the Template " +
    "Name, will automatically \r\nmatch to this Template.";
            // 
            // new_context_form
            // 
            this.AcceptButton = this.ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(295, 192);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.matchType);
            this.Controls.Add(this.basedOnExisting);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.name);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "new_context_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "New Template";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Button cancel;
        public System.Windows.Forms.TextBox name;
        public System.Windows.Forms.CheckBox basedOnExisting;
        private System.Windows.Forms.ComboBox matchType;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}