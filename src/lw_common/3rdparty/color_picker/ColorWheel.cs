using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ColorPicker
{
	class ColorWheel : Control
	{
		public event EventHandler SelectedColorChanged;
		
		Color m_frameColor = Color.CadetBlue;
		HSLColor m_selectedColor = new HSLColor(Color.BlanchedAlmond);
		PathGradientBrush m_brush = null;
		List<PointF> m_path = new List<PointF>();
		List<Color> m_colors = new List<Color>();
		double m_wheelLightness = 0.5;

		public HSLColor SelectedHSLColor
		{
			get { return m_selectedColor; }
			set 
			{
				if (m_selectedColor == value)
					return;
				Invalidate(Util.Rect(ColorSelectorRectangle));
				m_selectedColor = value;
				if (SelectedColorChanged != null)
					SelectedColorChanged(this, null);
				Refresh();//Invalidate(Util.Rect(ColorSelectorRectangle));
			}
		}
		public ColorWheel()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			DoubleBuffered = true;
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
		}
		
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			Invalidate();
		}
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			Invalidate();
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			using (SolidBrush b = new SolidBrush(BackColor))
			{
				e.Graphics.FillRectangle(b, ClientRectangle);
			}
			RectangleF wheelrect = WheelRectangle;
			Util.DrawFrame(e.Graphics, wheelrect, 6, m_frameColor);
			
			wheelrect = ColorWheelRectangle;
			PointF center = Util.Center(wheelrect);
			e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
			if (m_brush == null)
			{
				m_brush = new PathGradientBrush(m_path.ToArray(), WrapMode.Clamp);
				m_brush.CenterPoint = center;
				m_brush.CenterColor = Color.White;
				m_brush.SurroundColors = m_colors.ToArray();
			}
			e.Graphics.FillPie(m_brush, Util.Rect(wheelrect), 0, 360);
			DrawColorSelector(e.Graphics);

			if (Focused)
			{
				RectangleF r = WheelRectangle;
				r.Inflate(-2,-2);
				ControlPaint.DrawFocusRectangle(e.Graphics, Util.Rect(r));
			}
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (m_brush != null)
				m_brush.Dispose();
			m_brush = null;
			RecalcWheelPoints();
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			PointF mousepoint = new PointF(e.X, e.Y);
			if (e.Button == MouseButtons.Left)
				SetColor(mousepoint);
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Focus();
			PointF mousepoint = new PointF(e.X, e.Y);
			if (e.Button == MouseButtons.Left)
				SetColor(mousepoint);
		}
		protected override bool ProcessDialogKey(Keys keyData)
		{
			HSLColor c = SelectedHSLColor;
			double hue = c.Hue;
			int step = 1;
			if ((keyData & Keys.Control) == Keys.Control)
				step = 5;

			if ((keyData & Keys.Up) == Keys.Up)
				hue += step;
			if ((keyData & Keys.Down) == Keys.Down)
				hue -= step;
			if (hue >= 360)
				hue = 0;
			if (hue < 0)
				hue = 359;

			if (hue != c.Hue)
			{
				c.Hue = hue;
				SelectedHSLColor = c;
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		
		RectangleF ColorSelectorRectangle
		{
			get
			{
				HSLColor color = m_selectedColor;
				double angleR = color.Hue * Math.PI / 180;
				PointF center = Util.Center(ColorWheelRectangle);
				double radius = Radius(ColorWheelRectangle);
				radius *= color.Saturation;
				double x = center.X + Math.Cos(angleR) * radius;
				double y = center.Y - Math.Sin(angleR) * radius;
				Rectangle colrect = new Rectangle(new Point((int)x, (int)y), new Size(0, 0));
				colrect.Inflate(12, 12);
				return colrect;
			}
		}
		void DrawColorSelector(Graphics dc)
		{
			Rectangle r = Util.Rect(ColorSelectorRectangle);
			PointF center = Util.Center(r);
			Image image = SelectorImages.Image(SelectorImages.eIndexes.Donut);
			dc.DrawImageUnscaled(image, (int)(center.X-image.Width/2), (int)(center.Y-image.Height/2));
		}
		RectangleF WheelRectangle
		{
			get 
			{ 
				Rectangle r = ClientRectangle;
				r.Width -= 1;
				r.Height -= 1;
				return r; 
			}
		}
		RectangleF ColorWheelRectangle
		{
			get
			{
				RectangleF r = WheelRectangle;
				r.Inflate(-5, -5);
				return r;
			}
		}
		float Radius(RectangleF r)
		{
			PointF center = Util.Center(r);
			float radius = Math.Min((r.Width / 2), (r.Height / 2));
			return radius;
		}
		void RecalcWheelPoints()
		{
			m_path.Clear();
			m_colors.Clear();

			PointF center = Util.Center(ColorWheelRectangle);
			float radius = Radius(ColorWheelRectangle);
			double angle = 0;
			double fullcircle = 360;
			double step = 5;
			while (angle < fullcircle)
			{
				double angleR = angle * (Math.PI/180);
				double x = center.X + Math.Cos(angleR) * radius;
				double y = center.Y - Math.Sin(angleR) * radius;
				m_path.Add(new PointF((float)x,(float)y));
				m_colors.Add(new HSLColor(angle, 1, m_wheelLightness).Color);
				angle += step; 
			}
		}
		void SetColor(PointF mousepoint)
		{
			if (WheelRectangle.Contains(mousepoint) == false)
				return;

			PointF center = Util.Center(ColorWheelRectangle);
			double radius = Radius(ColorWheelRectangle);
			double dx = Math.Abs(mousepoint.X - center.X);
			double dy = Math.Abs(mousepoint.Y - center.Y);
			double angle = Math.Atan(dy / dx) / Math.PI * 180;
			double dist = Math.Pow((Math.Pow(dx, 2) + (Math.Pow(dy, 2))), 0.5);
			double saturation = dist/radius;
			//if (dist > radius + 5) // give 5 pixels slack
			//	return;
			if (dist < 6)
				saturation = 0; // snap to center

			if (mousepoint.X < center.X)
				angle = 180 - angle;
			if (mousepoint.Y > center.Y)
				angle = 360 - angle;

			SelectedHSLColor = new HSLColor(angle, saturation, SelectedHSLColor.Lightness);
		}
	}
}
