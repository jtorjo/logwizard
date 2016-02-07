namespace ColorPicker
{
	partial class ColorPickerCtrl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_colorWheel = new ColorPicker.ColorWheelCtrl();
            this.m_opacitySlider = new ColorPicker.ColorSlider();
            this.m_tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.m_eyedropColorPicker = new ColorPicker.EyedropColorPicker();
            this.m_colorSample = new ColorPicker.LabelRotate();
            this.m_infoLabel = new ColorPicker.LabelRotate();
            this.m_colorTable = new ColorPicker.ColorTable();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.m_colorWheel);
            this.panel1.Controls.Add(this.m_opacitySlider);
            this.panel1.Location = new System.Drawing.Point(257, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(254, 242);
            this.panel1.TabIndex = 9;
            // 
            // m_colorWheel
            // 
            this.m_colorWheel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_colorWheel.BackColor = System.Drawing.Color.Transparent;
            this.m_colorWheel.Location = new System.Drawing.Point(-1, 0);
            this.m_colorWheel.Name = "m_colorWheel";
            this.m_colorWheel.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(235)))), ((int)(((byte)(205)))));
            this.m_colorWheel.Size = new System.Drawing.Size(254, 209);
            this.m_colorWheel.TabIndex = 0;
            // 
            // m_opacitySlider
            // 
            this.m_opacitySlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_opacitySlider.BackColor = System.Drawing.Color.Transparent;
            this.m_opacitySlider.BarPadding = new System.Windows.Forms.Padding(60, 12, 80, 25);
            this.m_opacitySlider.Color1 = System.Drawing.Color.White;
            this.m_opacitySlider.Color2 = System.Drawing.Color.Black;
            this.m_opacitySlider.Color3 = System.Drawing.Color.Black;
            this.m_opacitySlider.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_opacitySlider.ForeColor = System.Drawing.Color.Black;
            this.m_opacitySlider.Location = new System.Drawing.Point(2, 213);
            this.m_opacitySlider.Name = "m_opacitySlider";
            this.m_opacitySlider.NumberOfColors = ColorPicker.ColorSlider.eNumberOfColors.Use2Colors;
            this.m_opacitySlider.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.m_opacitySlider.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.m_opacitySlider.Percent = 1F;
            this.m_opacitySlider.RotatePointAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.m_opacitySlider.Size = new System.Drawing.Size(248, 28);
            this.m_opacitySlider.TabIndex = 1;
            this.m_opacitySlider.Text = "Opacity";
            this.m_opacitySlider.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_opacitySlider.TextAngle = 0F;
            this.m_opacitySlider.ValueOrientation = ColorPicker.ColorSlider.eValueOrientation.MinToMax;
            // 
            // m_eyedropColorPicker
            // 
            this.m_eyedropColorPicker.BackColor = System.Drawing.SystemColors.Control;
            this.m_eyedropColorPicker.Location = new System.Drawing.Point(193, 150);
            this.m_eyedropColorPicker.Name = "m_eyedropColorPicker";
            this.m_eyedropColorPicker.SelectedColor = System.Drawing.Color.Empty;
            this.m_eyedropColorPicker.Size = new System.Drawing.Size(60, 60);
            this.m_eyedropColorPicker.TabIndex = 2;
            this.m_eyedropColorPicker.TabStop = false;
            this.m_tooltip.SetToolTip(this.m_eyedropColorPicker, "Color Selector. Click and Drag to pick a color from the screen");
            this.m_eyedropColorPicker.Zoom = 4;
            // 
            // m_colorSample
            // 
            this.m_colorSample.Location = new System.Drawing.Point(1, 150);
            this.m_colorSample.Name = "m_colorSample";
            this.m_colorSample.RotatePointAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.m_colorSample.Size = new System.Drawing.Size(186, 60);
            this.m_colorSample.TabIndex = 1;
            this.m_colorSample.TabStop = false;
            this.m_colorSample.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_colorSample.TextAngle = 0F;
            // 
            // m_infoLabel
            // 
            this.m_infoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_infoLabel.Location = new System.Drawing.Point(1, 217);
            this.m_infoLabel.Name = "m_infoLabel";
            this.m_infoLabel.RotatePointAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.m_infoLabel.Size = new System.Drawing.Size(252, 28);
            this.m_infoLabel.TabIndex = 3;
            this.m_infoLabel.TabStop = false;
            this.m_infoLabel.Text = "This is some sample text";
            this.m_infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.m_infoLabel.TextAngle = 0F;
            // 
            // m_colorTable
            // 
            this.m_colorTable.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.Black,
        System.Drawing.Color.DarkGreen,
        System.Drawing.Color.DarkSlateGray,
        System.Drawing.Color.Purple,
        System.Drawing.Color.Maroon,
        System.Drawing.Color.Teal,
        System.Drawing.Color.Green,
        System.Drawing.Color.Olive,
        System.Drawing.Color.Navy,
        System.Drawing.Color.Indigo,
        System.Drawing.Color.MidnightBlue,
        System.Drawing.Color.DarkRed,
        System.Drawing.Color.DarkMagenta,
        System.Drawing.Color.DarkBlue,
        System.Drawing.Color.DarkCyan,
        System.Drawing.Color.DarkOliveGreen,
        System.Drawing.Color.SaddleBrown,
        System.Drawing.Color.ForestGreen,
        System.Drawing.Color.OliveDrab,
        System.Drawing.Color.SeaGreen,
        System.Drawing.Color.DarkGoldenrod,
        System.Drawing.Color.DarkSlateBlue,
        System.Drawing.Color.MediumBlue,
        System.Drawing.Color.Sienna,
        System.Drawing.Color.Brown,
        System.Drawing.Color.DarkTurquoise,
        System.Drawing.Color.DimGray,
        System.Drawing.Color.LightSeaGreen,
        System.Drawing.Color.DarkViolet,
        System.Drawing.Color.Firebrick,
        System.Drawing.Color.MediumVioletRed,
        System.Drawing.Color.MediumSeaGreen,
        System.Drawing.Color.Crimson,
        System.Drawing.Color.Chocolate,
        System.Drawing.Color.MediumSpringGreen,
        System.Drawing.Color.Goldenrod,
        System.Drawing.Color.SteelBlue,
        System.Drawing.Color.LawnGreen,
        System.Drawing.Color.DarkOrchid,
        System.Drawing.Color.Orange,
        System.Drawing.Color.LimeGreen,
        System.Drawing.Color.Yellow,
        System.Drawing.Color.Gold,
        System.Drawing.Color.Red,
        System.Drawing.Color.Magenta,
        System.Drawing.Color.Lime,
        System.Drawing.Color.SpringGreen,
        System.Drawing.Color.YellowGreen,
        System.Drawing.Color.Chartreuse,
        System.Drawing.Color.DeepSkyBlue,
        System.Drawing.Color.Aqua,
        System.Drawing.Color.OrangeRed,
        System.Drawing.Color.Blue,
        System.Drawing.Color.DarkOrange,
        System.Drawing.Color.CadetBlue,
        System.Drawing.Color.Cyan,
        System.Drawing.Color.Fuchsia,
        System.Drawing.Color.Gray,
        System.Drawing.Color.SlateGray,
        System.Drawing.Color.Peru,
        System.Drawing.Color.BlueViolet,
        System.Drawing.Color.LightSlateGray,
        System.Drawing.Color.DeepPink,
        System.Drawing.Color.MediumTurquoise,
        System.Drawing.Color.DodgerBlue,
        System.Drawing.Color.Turquoise,
        System.Drawing.Color.RoyalBlue,
        System.Drawing.Color.SlateBlue,
        System.Drawing.Color.MediumOrchid,
        System.Drawing.Color.DarkKhaki,
        System.Drawing.Color.IndianRed,
        System.Drawing.Color.GreenYellow,
        System.Drawing.Color.MediumAquamarine,
        System.Drawing.Color.Tomato,
        System.Drawing.Color.DarkSeaGreen,
        System.Drawing.Color.Orchid,
        System.Drawing.Color.RosyBrown,
        System.Drawing.Color.PaleVioletRed,
        System.Drawing.Color.MediumPurple,
        System.Drawing.Color.Coral,
        System.Drawing.Color.CornflowerBlue,
        System.Drawing.Color.DarkGray,
        System.Drawing.Color.SandyBrown,
        System.Drawing.Color.MediumSlateBlue,
        System.Drawing.Color.Tan,
        System.Drawing.Color.DarkSalmon,
        System.Drawing.Color.BurlyWood,
        System.Drawing.Color.HotPink,
        System.Drawing.Color.Salmon,
        System.Drawing.Color.Violet,
        System.Drawing.Color.LightCoral,
        System.Drawing.Color.SkyBlue,
        System.Drawing.Color.LightSalmon,
        System.Drawing.Color.Khaki,
        System.Drawing.Color.Plum,
        System.Drawing.Color.LightGreen,
        System.Drawing.Color.Aquamarine,
        System.Drawing.Color.Silver,
        System.Drawing.Color.LightSkyBlue,
        System.Drawing.Color.LightSteelBlue,
        System.Drawing.Color.LightBlue,
        System.Drawing.Color.PaleGreen,
        System.Drawing.Color.PowderBlue,
        System.Drawing.Color.Thistle,
        System.Drawing.Color.PaleGoldenrod,
        System.Drawing.Color.PaleTurquoise,
        System.Drawing.Color.LightGray,
        System.Drawing.Color.Wheat,
        System.Drawing.Color.NavajoWhite,
        System.Drawing.Color.Moccasin,
        System.Drawing.Color.LightPink,
        System.Drawing.Color.PeachPuff,
        System.Drawing.Color.Gainsboro,
        System.Drawing.Color.Pink,
        System.Drawing.Color.Bisque,
        System.Drawing.Color.LightGoldenrodYellow,
        System.Drawing.Color.LemonChiffon,
        System.Drawing.Color.BlanchedAlmond,
        System.Drawing.Color.Beige,
        System.Drawing.Color.AntiqueWhite,
        System.Drawing.Color.PapayaWhip,
        System.Drawing.Color.Cornsilk,
        System.Drawing.Color.LightYellow,
        System.Drawing.Color.LightCyan,
        System.Drawing.Color.Lavender,
        System.Drawing.Color.Linen,
        System.Drawing.Color.MistyRose,
        System.Drawing.Color.OldLace,
        System.Drawing.Color.WhiteSmoke,
        System.Drawing.Color.SeaShell,
        System.Drawing.Color.Azure,
        System.Drawing.Color.Honeydew,
        System.Drawing.Color.Ivory,
        System.Drawing.Color.LavenderBlush,
        System.Drawing.Color.FloralWhite,
        System.Drawing.Color.AliceBlue,
        System.Drawing.Color.MintCream,
        System.Drawing.Color.GhostWhite,
        System.Drawing.Color.Snow,
        System.Drawing.Color.White};
            this.m_colorTable.Cols = 16;
            this.m_colorTable.FieldSize = new System.Drawing.Size(12, 12);
            this.m_colorTable.Location = new System.Drawing.Point(1, 7);
            this.m_colorTable.Name = "m_colorTable";
            this.m_colorTable.Padding = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.m_colorTable.RotatePointAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.m_colorTable.SelectedItem = System.Drawing.Color.Black;
            this.m_colorTable.Size = new System.Drawing.Size(252, 138);
            this.m_colorTable.TabIndex = 0;
            this.m_colorTable.Text = "m_colorTable";
            this.m_colorTable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_colorTable.TextAngle = 0F;
            // 
            // ColorPickerCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.m_colorSample);
            this.Controls.Add(this.m_infoLabel);
            this.Controls.Add(this.m_colorTable);
            this.Controls.Add(this.m_eyedropColorPicker);
            this.Controls.Add(this.panel1);
            this.Name = "ColorPickerCtrl";
            this.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.Size = new System.Drawing.Size(507, 250);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private ColorWheelCtrl m_colorWheel;
		private ColorSlider m_opacitySlider;
		private System.Windows.Forms.Panel panel1;
		private EyedropColorPicker m_eyedropColorPicker;
		private System.Windows.Forms.ToolTip m_tooltip;
		private ColorTable m_colorTable;
		private LabelRotate m_infoLabel;
		private LabelRotate m_colorSample;
	}
}
