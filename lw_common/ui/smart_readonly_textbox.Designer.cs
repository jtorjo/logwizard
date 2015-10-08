namespace lw_common.ui {
    partial class smart_readonly_textbox {
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
            this.stealFocus = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // stealFocus
            // 
            this.stealFocus.Enabled = true;
            this.stealFocus.Interval = 25;
            this.stealFocus.Tick += new System.EventHandler(this.stealFocus_Tick);
            // 
            // smart_readonly_textbox
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Multiline = false;
            this.ReadOnly = true;
            this.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Size = new System.Drawing.Size(493, 20);
            this.SelectionChanged += new System.EventHandler(this.smart_readonly_textbox_SelectionChanged);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.smart_readonly_textbox_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer stealFocus;
    }
}
