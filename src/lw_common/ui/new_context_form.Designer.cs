namespace lw_common.ui
{
    public partial class new_context_form
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
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Template Name";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(119, 7);
            this.name.Margin = new System.Windows.Forms.Padding(4);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(201, 23);
            this.name.TabIndex = 1;
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(112, 131);
            this.ok.Margin = new System.Windows.Forms.Padding(4);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(100, 28);
            this.ok.TabIndex = 2;
            this.ok.Text = "Ok";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(220, 131);
            this.cancel.Margin = new System.Windows.Forms.Padding(4);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(100, 28);
            this.cancel.TabIndex = 3;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // basedOnExisting
            // 
            this.basedOnExisting.AutoSize = true;
            this.basedOnExisting.Checked = true;
            this.basedOnExisting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.basedOnExisting.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.basedOnExisting.Location = new System.Drawing.Point(78, 38);
            this.basedOnExisting.Margin = new System.Windows.Forms.Padding(4);
            this.basedOnExisting.Name = "basedOnExisting";
            this.basedOnExisting.Size = new System.Drawing.Size(242, 17);
            this.basedOnExisting.TabIndex = 4;
            this.basedOnExisting.Text = "Copy Views and Filters From Current Template";
            this.basedOnExisting.UseVisualStyleBackColor = true;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 59);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(317, 68);
            this.label3.TabIndex = 7;
            this.label3.Text = "Note: \r\nChoose the name carefully, since any File Name \r\ncontaining the Template " +
    "Name, will automatically \r\nmatch to this Template.";
            // 
            // new_context_form
            // 
            this.AcceptButton = this.ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(330, 164);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.basedOnExisting);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.name);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
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
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label3;
    }
}