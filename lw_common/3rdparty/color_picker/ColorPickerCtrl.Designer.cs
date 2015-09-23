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
			this.m_tooltip = new System.Windows.Forms.ToolTip(this.components);
			this.m_colorSample = new ColorPicker.LabelRotate();
			this.m_infoLabel = new ColorPicker.LabelRotate();
			this.m_colorTable = new ColorPicker.ColorTable();
			this.m_eyedropColorPicker = new ColorPicker.EyedropColorPicker();
			this.m_colorWheel = new ColorPicker.ColorWheelCtrl();
			this.m_opacitySlider = new ColorPicker.ColorSlider();
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
			// ColorPickerCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
