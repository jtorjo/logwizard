namespace test_ui {
    partial class test_categories_ctrl {
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
            this.categories = new lw_common.ui.categories_ctrl();
            this.SuspendLayout();
            // 
            // categories
            // 
            this.categories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.categories.Location = new System.Drawing.Point(12, 12);
            this.categories.Name = "categories";
            this.categories.Size = new System.Drawing.Size(632, 691);
            this.categories.TabIndex = 0;
            // 
            // test_categories_ctrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 715);
            this.Controls.Add(this.categories);
            this.Name = "test_categories_ctrl";
            this.Text = "test_categories_ctrl";
            this.ResumeLayout(false);

        }

        #endregion

        private lw_common.ui.categories_ctrl categories;
    }
}