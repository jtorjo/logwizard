namespace ColorPicker
{
	partial class ColorPickerDialog
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
			this.m_tabControl = new System.Windows.Forms.TabControl();
			this.m_colorTabPage = new System.Windows.Forms.TabPage();
			this.m_knownColorsTabPage = new System.Windows.Forms.TabPage();
			this.m_colorList = new ColorPicker.ColorListBox();
			this.m_cancel = new System.Windows.Forms.Button();
			this.m_ok = new System.Windows.Forms.Button();
			this.m_tabControl.SuspendLayout();
			this.m_knownColorsTabPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_tabControl
			// 
			this.m_tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_tabControl.Controls.Add(this.m_colorTabPage);
			this.m_tabControl.Controls.Add(this.m_knownColorsTabPage);
			this.m_tabControl.Location = new System.Drawing.Point(4, 5);
			this.m_tabControl.Name = "m_tabControl";
			this.m_tabControl.SelectedIndex = 0;
			this.m_tabControl.Size = new System.Drawing.Size(527, 282);
			this.m_tabControl.TabIndex = 1;
			this.m_tabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.OnSelected);
			// 
			// m_colorTabPage
			// 
			this.m_colorTabPage.Location = new System.Drawing.Point(4, 22);
			this.m_colorTabPage.Name = "m_colorTabPage";
			this.m_colorTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.m_colorTabPage.Size = new System.Drawing.Size(519, 256);
			this.m_colorTabPage.TabIndex = 0;
			this.m_colorTabPage.Text = "Colors";
			this.m_colorTabPage.UseVisualStyleBackColor = true;
			// 
			// m_knownColorsTabPage
			// 
			this.m_knownColorsTabPage.Controls.Add(this.m_colorList);
			this.m_knownColorsTabPage.Location = new System.Drawing.Point(4, 22);
			this.m_knownColorsTabPage.Name = "m_knownColorsTabPage";
			this.m_knownColorsTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.m_knownColorsTabPage.Size = new System.Drawing.Size(519, 256);
			this.m_knownColorsTabPage.TabIndex = 1;
			this.m_knownColorsTabPage.Text = "Known Colors";
			this.m_knownColorsTabPage.UseVisualStyleBackColor = true;
			// 
			// m_colorList
			// 
			this.m_colorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_colorList.ColumnWidth = 170;
			this.m_colorList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.m_colorList.FormattingEnabled = true;
			this.m_colorList.ItemHeight = 26;
			this.m_colorList.Location = new System.Drawing.Point(6, 10);
			this.m_colorList.MultiColumn = true;
			this.m_colorList.Name = "m_colorList";
			this.m_colorList.Size = new System.Drawing.Size(506, 238);
			this.m_colorList.TabIndex = 0;
			// 
			// m_cancel
			// 
			this.m_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_cancel.Location = new System.Drawing.Point(452, 293);
			this.m_cancel.Name = "m_cancel";
			this.m_cancel.Size = new System.Drawing.Size(75, 23);
			this.m_cancel.TabIndex = 2;
			this.m_cancel.Text = "&Cancel";
			this.m_cancel.UseVisualStyleBackColor = true;
			// 
			// m_ok
			// 
			this.m_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_ok.Location = new System.Drawing.Point(371, 293);
			this.m_ok.Name = "m_ok";
			this.m_ok.Size = new System.Drawing.Size(75, 23);
			this.m_ok.TabIndex = 2;
			this.m_ok.Text = "&OK";
			this.m_ok.UseVisualStyleBackColor = true;
			// 
			// ColorPickerDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(534, 326);
			this.Controls.Add(this.m_ok);
			this.Controls.Add(this.m_cancel);
			this.Controls.Add(this.m_tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ColorPickerDialog";
			this.Text = "Color Picker";
			this.m_tabControl.ResumeLayout(false);
			this.m_knownColorsTabPage.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl m_tabControl;
		private System.Windows.Forms.TabPage m_knownColorsTabPage;
		private System.Windows.Forms.TabPage m_colorTabPage;
		private System.Windows.Forms.Button m_ok;
		private System.Windows.Forms.Button m_cancel;
		private ColorListBox m_colorList;
	}
}

