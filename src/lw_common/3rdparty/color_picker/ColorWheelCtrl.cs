using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ColorPicker
{
	public partial class ColorWheelCtrl : UserControl
	{
		public event EventHandler SelectedColorChanged;
		HSLColor m_selectedColor = new HSLColor(Color.Wheat);

		public ColorWheelCtrl()
		{
			InitializeComponent();
			m_colorWheel.SelectedColorChanged += new EventHandler(OnWheelColorChanged);
			m_colorBar.SelectedValueChanged += new EventHandler(OnLightnessColorChanged);
			m_colorBar.ValueOrientation = ColorSlider.eValueOrientation.MaxToMin;
		}

		void OnLightnessColorChanged(object sender, EventArgs e)
		{
			m_selectedColor.Lightness = m_colorBar.SelectedHSLColor.Lightness;
			SelectedHSLColor = m_selectedColor;
		}

		void OnWheelColorChanged(object sender, EventArgs e)
		{
			m_selectedColor.Hue = m_colorWheel.SelectedHSLColor.Hue;
			m_selectedColor.Saturation = m_colorWheel.SelectedHSLColor.Saturation;
			SelectedHSLColor = m_selectedColor;
		}
		public Color SelectedColor
		{
			get { return m_selectedColor.Color; }
			set
			{
				if (m_selectedColor.Color != value)
					SelectedHSLColor = new HSLColor(value);
			}
		}
		public HSLColor SelectedHSLColor
		{
			get { return m_selectedColor; }
			set
			{
				m_colorBar.SelectedHSLColor = value;
				m_colorWheel.SelectedHSLColor = value;
				m_selectedColor = value;
				if (SelectedColorChanged != null)
					SelectedColorChanged(this, null);
			}
		}
	}
}
