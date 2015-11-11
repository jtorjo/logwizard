namespace lw_common.ui {
    public partial class status_ctrl {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.status = new System.Windows.Forms.RichTextBox();
            this.goToNextLine = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // status
            // 
            this.status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.status.BackColor = System.Drawing.Color.White;
            this.status.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.status.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.status.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status.Location = new System.Drawing.Point(0, 0);
            this.status.Multiline = false;
            this.status.Name = "status";
            this.status.ReadOnly = true;
            this.status.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.status.ShortcutsEnabled = false;
            this.status.Size = new System.Drawing.Size(613, 24);
            this.status.TabIndex = 0;
            this.status.TabStop = false;
            this.status.Text = "";
            this.status.WordWrap = false;
            this.status.MouseMove += new System.Windows.Forms.MouseEventHandler(this.status_MouseMove);
            // 
            // goToNextLine
            // 
            this.goToNextLine.Enabled = true;
            this.goToNextLine.Interval = 2500;
            this.goToNextLine.Tick += new System.EventHandler(this.goToNextLine_Tick);
            // 
            // status_ctrl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.status);
            this.Name = "status_ctrl";
            this.Size = new System.Drawing.Size(613, 24);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox status;
        private System.Windows.Forms.Timer goToNextLine;
    }
}
