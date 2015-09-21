using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ColorPicker
{
	public partial class ColorPickerCtrl : UserControl
	{
		Color m_selectedColor = Color.AntiqueWhite;
		public Color SelectedColor
		{
			get { return Color.FromArgb((int)Math.Floor(255f*m_opacity), m_selectedColor);; }
			set
			{
				m_opacity = (float)value.A / 255f;
				value = Color.FromArgb(255, value);
				m_colorWheel.SelectedColor = value;
				if (m_colorTable.ColorExist(value) == false)
					m_colorTable.SetCustomColor(value);
				m_colorTable.SelectedItem = value;
				m_opacitySlider.Percent = m_opacity;
			}
		}
		public ColorPickerCtrl()
		{
			InitializeComponent();

			List<Color> colors = new List<Color>();
			float step = 100/8;
			for (float x = 0; x <= 100; x += step)
			{
				float v = 255 * x/100;
				colors.Add(Color.FromArgb(255, (int)v, (int)v, (int)v));
			}
			colors.Add(Color.White);

			colors.Add(Color.FromArgb(255, 255, 000, 000));
			colors.Add(Color.FromArgb(255, 255, 255, 000));
			colors.Add(Color.FromArgb(255, 000, 255, 000));
			colors.Add(Color.FromArgb(255, 000, 255, 255));
			colors.Add(Color.FromArgb(255, 000, 000, 255));
			colors.Add(Color.FromArgb(255, 255, 000, 255));

			int cols = 16;
			int rows = 3;
			float cnt = (rows * cols);
			float huestep = 360 / cnt;
			float hue = 0;
			while (hue < 360)
			{
				colors.Add(new HSLColor(hue, 1, 0.5).Color);
				hue += huestep;
			}
			hue = 0;
			while (hue < 360)
			{
				colors.Add(new HSLColor(hue, 0.5, 0.5).Color);
				hue += huestep;
			}
			m_colorTable.Colors = colors.ToArray();
			m_colorTable.Cols = cols;
			m_colorTable.SelectedIndexChanged += new EventHandler(OnColorTableSelectionChanged);

			m_colorWheel.SelectedColorChanged += new EventHandler(OnColorWheelSelectionChanged);
			m_opacitySlider.SelectedValueChanged += new EventHandler(OnOpacityValueChanged);
			m_eyedropColorPicker.SelectedColorChanged += new EventHandler(OnEyeDropperSelectionChanged);
			m_colorSample.Paint += new PaintEventHandler(OnColorSamplePaint);
		}
		void OnEyeDropperSelectionChanged(object sender, EventArgs e)
		{
			m_colorWheel.SelectedColor = m_eyedropColorPicker.SelectedColor;
		}
		float m_opacity = 1;
		void OnOpacityValueChanged(object sender, EventArgs e)
		{
			m_opacity = Math.Max(0, m_opacitySlider.Percent);
			m_opacity = Math.Min(1, m_opacitySlider.Percent);
			m_colorSample.Refresh();
			UpdateInfo();
		}
		void OnColorWheelSelectionChanged(object sender, EventArgs e)
		{
			Color selcol = m_colorWheel.SelectedColor;
			if (selcol != null && selcol != m_selectedColor)
			{
				m_selectedColor = selcol;
				m_colorSample.Refresh();
				if (lockColorTable == false && selcol != m_colorTable.SelectedItem)
					m_colorTable.SetCustomColor(selcol);
			}
			UpdateInfo();
		}
		void UpdateInfo()
		{
			Color c = Color.FromArgb((int)Math.Floor(255f*m_opacity), m_selectedColor);
			string info = string.Format("{0} aRGB({1}, {2}, {3}, {4})", m_colorWheel.SelectedHSLColor.ToString(), c.A, c.R, c.G, c.B);
			m_infoLabel.Text = info;
		}
		void OnColorSamplePaint(object sender, PaintEventArgs e)
		{
			Rectangle r = m_colorSample.ClientRectangle;
			r.Inflate(-4,-4);

			int width = r.Width;
			r.Width /= 2;
			
			Color c = Color.FromArgb((int)Math.Floor(255f*m_opacity), m_selectedColor);
			SolidBrush b = new SolidBrush(c);
			e.Graphics.FillRectangle(b, r);

			r.X += r.Width;

			e.Graphics.FillRectangle(Brushes.White, r);
			c = Color.FromArgb(255, m_selectedColor);
			b = new SolidBrush(c);
			e.Graphics.FillRectangle(b, r);
		}

		bool lockColorTable = false;
		void OnColorTableSelectionChanged(object sender, EventArgs e)
		{
			Color selcol = (Color)m_colorTable.SelectedItem;
			if (selcol != null && selcol != m_selectedColor)
			{
				lockColorTable = true;
				m_colorWheel.SelectedColor = selcol;
				lockColorTable = false;
				m_colorSample.Invalidate();
			}
		}
	}
	public class ColorListBox : ListBox
	{
		int m_knownColorCount = 0;
		public ColorListBox()
		{
			DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			PropertyInfo[] propinfos = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
			foreach (PropertyInfo info in propinfos)
			{
				if (info.PropertyType == typeof(Color))
				{
					Color c = (Color)info.GetValue(typeof(Color), null);
					if (c.A == 0) // skip transparent
						continue;
					Items.Add(c);
				}
			}
			m_knownColorCount = Items.Count;
		}
		int ColorIndex(Color color)
		{
			// have to search all colors by value;
			int argb = color.ToArgb();
			for (int index = 0; index < Items.Count; index++)
			{
				Color c = (Color)Items[index];
				if (c.ToArgb() == argb)
					return index;
			}
			return -1;
		}
		public void SelectColor(Color color)
		{
			int index = ColorIndex(color);
			if (index < 0)
				index = SetCustomColor(color);
			SelectedItem = Items[index];
		}
		void RemoveCustomColor()
		{
			if (Items.Count > m_knownColorCount)
				Items.RemoveAt(Items.Count - 1);
		}
		int SetCustomColor(Color col)
		{
			RemoveCustomColor();
			Items.Add(col);
			return Items.Count - 1;
		}
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (e.Index > -1)
			{
				e.DrawBackground();
				Rectangle rect = e.Bounds;
				rect.Inflate(-2,-2);
				rect.Width = 50;
				Color textColor = Color.Empty;
				if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
					textColor = SystemColors.HighlightText;
				else
					textColor = this.ForeColor;
				Color color = (Color)Items[e.Index];
				using (Brush brush = new SolidBrush(color))
				{
					e.Graphics.FillRectangle(brush, rect);
				}
				if ((e.State & DrawItemState.Selected) != DrawItemState.Selected)
				{
					using (Pen pen = new Pen(e.ForeColor))
					{
						e.Graphics.DrawRectangle(pen, rect);
					}
				}
				using (Brush brush = new SolidBrush(e.ForeColor))
				{
					string name = color.Name + string.Format("({0})", e.Index);
					if (color.IsKnownColor == false)
						name = "<custom>";
					
					StringFormat format = new StringFormat();
					format.LineAlignment = StringAlignment.Center;
					rect = e.Bounds;
					rect.X += 60; 
					e.Graphics.DrawString(name, Font, brush, rect, format);
				}
				e.DrawFocusRectangle();
			}
		}
	}
}
