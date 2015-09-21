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
	public class LabelRotate : Control
	{
		float m_textAngle = 0;
		ContentAlignment m_rotatePointAlignment = ContentAlignment.MiddleCenter;
		ContentAlignment m_textAlignment = ContentAlignment.MiddleLeft;

		public new string Text
		{
			get { return base.Text; }
			set
			{
				base.Text = value;
				Refresh();
			}
		}
		public float TextAngle
		{
			get { return m_textAngle; }
			set
			{
				m_textAngle = value;
				Invalidate();
			}
		}
		public ContentAlignment TextAlign
		{
			get { return m_textAlignment; }
			set
			{ 
				m_textAlignment = value; 
				Invalidate();
			}
		}
		public ContentAlignment RotatePointAlignment
		{
			get { return m_rotatePointAlignment; }
			set 
			{
				m_rotatePointAlignment = value;
				Invalidate();
			}
		}

		Color m_frameColor = Color.CadetBlue;
		public LabelRotate()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.Text = string.Empty;
			this.DoubleBuffered = true;
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			using (SolidBrush b = new SolidBrush(BackColor))
			{
				e.Graphics.FillRectangle(b, ClientRectangle);
			}

			RectangleF lr = ClientRectangleF;
			Pen framepen = new Pen(m_frameColor, 1);
			Util.DrawFrame(e.Graphics, lr, 6, m_frameColor);
			if (Text.Length > 0)
			{
				StringFormat format = new StringFormat();
				string alignment = TextAlign.ToString();
				
				if (((int)TextAlign & (int)(ContentAlignment.BottomLeft | ContentAlignment.MiddleLeft | ContentAlignment.TopLeft)) != 0)
					format.Alignment = StringAlignment.Near;

				if (((int)TextAlign & (int)(ContentAlignment.BottomCenter | ContentAlignment.MiddleCenter | ContentAlignment.TopCenter)) != 0)
					format.Alignment = StringAlignment.Center;

				if (((int)TextAlign & (int)(ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight)) != 0)
					format.Alignment = StringAlignment.Far;

				if (((int)TextAlign & (int)(ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight)) != 0)
					format.LineAlignment = StringAlignment.Far;

				if (((int)TextAlign & (int)(ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight)) != 0)
					format.LineAlignment = StringAlignment.Center;

				if (((int)TextAlign & (int)(ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight)) != 0)
					format.LineAlignment = StringAlignment.Near;
				
				Rectangle r = ClientRectangle;
				r.X += Padding.Left;
				r.Y += Padding.Top;
				r.Width -= Padding.Right;
				r.Height -= Padding.Bottom;
				
				using (SolidBrush b = new SolidBrush(ForeColor))
				{
					if (TextAngle == 0)
					{
						e.Graphics.DrawString(Text, Font, b, r, format);
					}
					else
					{
						PointF center = Util.Center(ClientRectangle);
						switch (RotatePointAlignment)
						{
							case ContentAlignment.TopLeft:
								center.X = r.Left;
								center.Y = r.Top;
								break;
							case ContentAlignment.TopCenter:
								center.Y = r.Top;
								break;
							case ContentAlignment.TopRight:
								center.X = r.Right;
								center.Y = r.Top;
								break;
							case ContentAlignment.MiddleLeft:
								center.X = r.Left;
								break;
							case ContentAlignment.MiddleCenter:
								break;
							case ContentAlignment.MiddleRight:
								center.X = r.Right;
								break;
							case ContentAlignment.BottomLeft:
								center.X = r.Left;
								center.Y = r.Bottom;
								break;
							case ContentAlignment.BottomCenter:
								center.Y = r.Bottom;
								break;
							case ContentAlignment.BottomRight:
								center.X = r.Right;
								center.Y = r.Bottom;
								break;
						}
						center.X += Padding.Left;
						center.Y += Padding.Top;
						center.X -= Padding.Right;
						center.Y -= Padding.Bottom;

						e.Graphics.TranslateTransform(center.X, center.Y);
						e.Graphics.RotateTransform(TextAngle);

						e.Graphics.DrawString(Text, Font, b, new PointF(0,0), format);
						e.Graphics.ResetTransform();
					}
				}
			}
			RaisePaintEvent(this, e);
		}
		protected RectangleF ClientRectangleF
		{
			get
			{
				RectangleF r = ClientRectangle;
				r.Width -= 1;
				r.Height -= 1;
				return r;
			}
		}
	}
}
