namespace test_ui {
    partial class test_description_ctrl {
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
            this.components = new System.ComponentModel.Container();
            this.description_ctrl1 = new lw_common.ui.description_ctrl();
            this.SuspendLayout();
            // 
            // description_ctrl1
            // 
            this.description_ctrl1.BackColor = System.Drawing.SystemColors.Control;
            this.description_ctrl1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.description_ctrl1.Location = new System.Drawing.Point(32, 42);
            this.description_ctrl1.Name = "description_ctrl1";
            this.description_ctrl1.Size = new System.Drawing.Size(609, 352);
            this.description_ctrl1.TabIndex = 0;
            // 
            // test_description_ctrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(697, 455);
            this.Controls.Add(this.description_ctrl1);
            this.Name = "test_description_ctrl";
            this.Text = "test_description_ctrl";
            this.ResumeLayout(false);

        }

        #endregion

        private lw_common.ui.description_ctrl description_ctrl1;
    }
}