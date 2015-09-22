namespace test_ui {
    partial class test_notes_ctrl {
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
            this.note_ctrl1 = new lw_common.ui.note_ctrl();
            this.SuspendLayout();
            // 
            // note_ctrl1
            // 
            this.note_ctrl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.note_ctrl1.Location = new System.Drawing.Point(13, 3);
            this.note_ctrl1.Margin = new System.Windows.Forms.Padding(4);
            this.note_ctrl1.Name = "note_ctrl1";
            this.note_ctrl1.Size = new System.Drawing.Size(411, 460);
            this.note_ctrl1.TabIndex = 0;
            // 
            // test_notes_ctrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 476);
            this.Controls.Add(this.note_ctrl1);
            this.Name = "test_notes_ctrl";
            this.Text = "Test Notes control";
            this.ResumeLayout(false);

        }

        #endregion

        private lw_common.ui.note_ctrl note_ctrl1;
    }
}

