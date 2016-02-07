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
            this.m_colorList.Items.AddRange(new object[] {
            System.Drawing.Color.WhiteSmoke,
            System.Drawing.Color.Silver,
            System.Drawing.Color.Gray,
            System.Drawing.Color.White,
            System.Drawing.Color.Black,
            System.Drawing.Color.AliceBlue,
            System.Drawing.Color.AntiqueWhite,
            System.Drawing.Color.Aqua,
            System.Drawing.Color.Aquamarine,
            System.Drawing.Color.Azure,
            System.Drawing.Color.Beige,
            System.Drawing.Color.Bisque,
            System.Drawing.Color.BlanchedAlmond,
            System.Drawing.Color.Blue,
            System.Drawing.Color.BlueViolet,
            System.Drawing.Color.Brown,
            System.Drawing.Color.BurlyWood,
            System.Drawing.Color.CadetBlue,
            System.Drawing.Color.Chartreuse,
            System.Drawing.Color.Chocolate,
            System.Drawing.Color.Coral,
            System.Drawing.Color.CornflowerBlue,
            System.Drawing.Color.Cornsilk,
            System.Drawing.Color.Crimson,
            System.Drawing.Color.Cyan,
            System.Drawing.Color.DarkBlue,
            System.Drawing.Color.DarkCyan,
            System.Drawing.Color.DarkGoldenrod,
            System.Drawing.Color.DarkGray,
            System.Drawing.Color.DarkGreen,
            System.Drawing.Color.DarkKhaki,
            System.Drawing.Color.DarkMagenta,
            System.Drawing.Color.DarkOliveGreen,
            System.Drawing.Color.DarkOrange,
            System.Drawing.Color.DarkOrchid,
            System.Drawing.Color.DarkRed,
            System.Drawing.Color.DarkSalmon,
            System.Drawing.Color.DarkSeaGreen,
            System.Drawing.Color.DarkSlateBlue,
            System.Drawing.Color.DarkSlateGray,
            System.Drawing.Color.DarkTurquoise,
            System.Drawing.Color.DarkViolet,
            System.Drawing.Color.DeepPink,
            System.Drawing.Color.DeepSkyBlue,
            System.Drawing.Color.DimGray,
            System.Drawing.Color.DodgerBlue,
            System.Drawing.Color.Firebrick,
            System.Drawing.Color.FloralWhite,
            System.Drawing.Color.ForestGreen,
            System.Drawing.Color.Fuchsia,
            System.Drawing.Color.Gainsboro,
            System.Drawing.Color.GhostWhite,
            System.Drawing.Color.Gold,
            System.Drawing.Color.Goldenrod,
            System.Drawing.Color.Green,
            System.Drawing.Color.GreenYellow,
            System.Drawing.Color.Honeydew,
            System.Drawing.Color.HotPink,
            System.Drawing.Color.IndianRed,
            System.Drawing.Color.Indigo,
            System.Drawing.Color.Ivory,
            System.Drawing.Color.Khaki,
            System.Drawing.Color.Lavender,
            System.Drawing.Color.LavenderBlush,
            System.Drawing.Color.LawnGreen,
            System.Drawing.Color.LemonChiffon,
            System.Drawing.Color.LightBlue,
            System.Drawing.Color.LightCoral,
            System.Drawing.Color.LightCyan,
            System.Drawing.Color.LightGoldenrodYellow,
            System.Drawing.Color.LightGreen,
            System.Drawing.Color.LightGray,
            System.Drawing.Color.LightPink,
            System.Drawing.Color.LightSalmon,
            System.Drawing.Color.LightSeaGreen,
            System.Drawing.Color.LightSkyBlue,
            System.Drawing.Color.LightSlateGray,
            System.Drawing.Color.LightSteelBlue,
            System.Drawing.Color.LightYellow,
            System.Drawing.Color.Lime,
            System.Drawing.Color.LimeGreen,
            System.Drawing.Color.Linen,
            System.Drawing.Color.Magenta,
            System.Drawing.Color.Maroon,
            System.Drawing.Color.MediumAquamarine,
            System.Drawing.Color.MediumBlue,
            System.Drawing.Color.MediumOrchid,
            System.Drawing.Color.MediumPurple,
            System.Drawing.Color.MediumSeaGreen,
            System.Drawing.Color.MediumSlateBlue,
            System.Drawing.Color.MediumSpringGreen,
            System.Drawing.Color.MediumTurquoise,
            System.Drawing.Color.MediumVioletRed,
            System.Drawing.Color.MidnightBlue,
            System.Drawing.Color.MintCream,
            System.Drawing.Color.MistyRose,
            System.Drawing.Color.Moccasin,
            System.Drawing.Color.NavajoWhite,
            System.Drawing.Color.Navy,
            System.Drawing.Color.OldLace,
            System.Drawing.Color.Olive,
            System.Drawing.Color.OliveDrab,
            System.Drawing.Color.Orange,
            System.Drawing.Color.OrangeRed,
            System.Drawing.Color.Orchid,
            System.Drawing.Color.PaleGoldenrod,
            System.Drawing.Color.PaleGreen,
            System.Drawing.Color.PaleTurquoise,
            System.Drawing.Color.PaleVioletRed,
            System.Drawing.Color.PapayaWhip,
            System.Drawing.Color.PeachPuff,
            System.Drawing.Color.Peru,
            System.Drawing.Color.Pink,
            System.Drawing.Color.Plum,
            System.Drawing.Color.PowderBlue,
            System.Drawing.Color.Purple,
            System.Drawing.Color.Red,
            System.Drawing.Color.RosyBrown,
            System.Drawing.Color.RoyalBlue,
            System.Drawing.Color.SaddleBrown,
            System.Drawing.Color.Salmon,
            System.Drawing.Color.SandyBrown,
            System.Drawing.Color.SeaGreen,
            System.Drawing.Color.SeaShell,
            System.Drawing.Color.Sienna,
            System.Drawing.Color.SkyBlue,
            System.Drawing.Color.SlateBlue,
            System.Drawing.Color.SlateGray,
            System.Drawing.Color.Snow,
            System.Drawing.Color.SpringGreen,
            System.Drawing.Color.SteelBlue,
            System.Drawing.Color.Tan,
            System.Drawing.Color.Teal,
            System.Drawing.Color.Thistle,
            System.Drawing.Color.Tomato,
            System.Drawing.Color.Turquoise,
            System.Drawing.Color.Violet,
            System.Drawing.Color.Wheat,
            System.Drawing.Color.Yellow,
            System.Drawing.Color.YellowGreen});
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
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

