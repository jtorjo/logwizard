namespace test_ui
{
    partial class test_snoop_form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            this.a = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.d = new System.Windows.Forms.TextBox();
            this.b = new System.Windows.Forms.TextBox();
            this.c = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // a
            // 
            this.a.Location = new System.Drawing.Point(48, 42);
            this.a.Name = "a";
            this.a.Size = new System.Drawing.Size(272, 31);
            this.a.TabIndex = 0;
            this.a.Text = "a";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(48, 79);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(272, 31);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = "unused";
            // 
            // d
            // 
            this.d.Location = new System.Drawing.Point(48, 116);
            this.d.Name = "d";
            this.d.Size = new System.Drawing.Size(272, 31);
            this.d.TabIndex = 2;
            this.d.Text = "d";
            // 
            // b
            // 
            this.b.Location = new System.Drawing.Point(326, 42);
            this.b.Name = "b";
            this.b.Size = new System.Drawing.Size(272, 31);
            this.b.TabIndex = 3;
            this.b.Text = "b";
            // 
            // c
            // 
            this.c.Location = new System.Drawing.Point(604, 42);
            this.c.Name = "c";
            this.c.Size = new System.Drawing.Size(272, 31);
            this.c.TabIndex = 4;
            this.c.Text = "c";
            // 
            // test_snoop_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1469, 634);
            this.Controls.Add(this.c);
            this.Controls.Add(this.b);
            this.Controls.Add(this.d);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.a);
            this.Name = "test_snoop_form";
            this.Text = "test_snoop_form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox a;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox d;
        private System.Windows.Forms.TextBox b;
        private System.Windows.Forms.TextBox c;
    }
}