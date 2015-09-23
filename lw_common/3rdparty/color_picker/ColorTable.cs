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
	public class ColorTable : LabelRotate
	{
		public event EventHandler SelectedIndexChanged;
		public Color SelectedItem
		{
			get
			{
				if (m_selindex < 0 || m_selindex >= m_colors.Count)
					return Color.White;
				return m_colors[m_selindex];
			}
			set 
			{
				if (m_selindex < m_colors.Count && value == m_colors[m_selindex])
					return;
				int index = m_colors.IndexOf(value);
				if (index < 0)
					return;
				SetIndex(index);
			}
		}
		public bool ColorExist(Color c)
		{
			int index = m_colors.IndexOf(c);
			return index >= 0;
		}
		int m_cols = 0;
		int m_rows = 0;
		public int Cols
		{
			get { return m_cols; }
			set
			{
				m_cols = value;
				m_rows = m_colors.Count / m_cols; 
				if ((m_colors.Count % m_cols) != 0)
					m_rows++;
			}
		}	
		Size m_fieldSize = new Size(12, 12);
		public Size FieldSize
		{
			get { return m_fieldSize; }
			set { m_fieldSize = value; }
		}
		int CompareColorByValue(Color c1, Color c2)
		{
			int color1 = c1.R << 16 | c1.G << 8 | c1.B;
			int color2 = c2.R << 16 | c2.G << 8 | c2.B;
			if (color1 > color2)
				return -1;
			if (color1 < color2)
				return 1;
			return 0;
		}
		int CompareColorByHue(Color c1, Color c2)
		{
			float h1 = c1.GetHue();
			float h2 = c2.GetHue();
			if (h1 < h2)
				return -1;
			if (h1 > h2)
				return 1;
			return 0;
		}
		int CompareColorByBrightness(Color c1, Color c2)
		{
			float h1 = c1.GetBrightness();
			float h2 = c2.GetBrightness();
			if (h1 < h2)
				return -1;
			if (h1 > h2)
				return 1;
			return 0;
		}

		public void SortColorByValue()
		{
			m_colors.Sort(CompareColorByValue);
			Invalidate();
		}
		public void SortColorByHue()
		{
			m_colors.Sort(CompareColorByHue);
			Invalidate();
		}
		public void SortColorByBrightness()
		{
			m_colors.Sort(CompareColorByBrightness);
			Invalidate();
		}

		List<Color> m_colors = new List<Color>();
		public ColorTable(Color[] colors)
		{
			this.DoubleBuffered = true;
			if (colors != null)
				m_colors = new List<Color>(colors);
			Cols = 16;
			m_initialColorCount = m_colors.Count;
			Padding = new Padding(8,8,0,0);
		}
		public ColorTable()
		{
			this.DoubleBuffered = true;
			PropertyInfo[] propinfos = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
			foreach (PropertyInfo info in propinfos)
			{
				if (info.PropertyType == typeof(Color))
				{
					Color c = (Color)info.GetValue(typeof(Color), null);
					if (c.A == 0) // transparent
						continue;
					m_colors.Add(c);
				}
			}
			m_colors.Sort(CompareColorByBrightness);
			m_initialColorCount = m_colors.Count;
			Cols = 16;
		}
		public void RemoveCustomColor()
		{
			if (m_colors.Count > m_initialColorCount)
				m_colors.RemoveAt(m_colors.Count-1);
		}
		public void SetCustomColor(Color col)
		{
			RemoveCustomColor();
			if (m_colors.Contains(col) == false)
			{
				int rows = m_rows;
				m_colors.Add(col);
				Cols = Cols;
				if (m_rows != rows)
					Invalidate();
				else
					Invalidate(GetRectangle(m_colors.Count-1));
			}
		}
		public Color[] Colors
		{
			get { return m_colors.ToArray(); }
			set
			{
				m_colors = new List<Color>(value);
				Cols = 16;
				m_initialColorCount = m_colors.Count;
			}
		}
		int m_spacing = 3;
		int m_selindex = 0;
		int m_initialColorCount = 0;
		Rectangle GetSelectedItemRect()
		{
			Rectangle rect = GetRectangle(m_selindex);
			rect.Inflate(m_fieldSize.Width / 2, m_fieldSize.Height / 2);
			return rect;
		}
		Rectangle GetRectangle(int index)
		{
			int row = 0;
			int col = 0;
			GetRowCol(index, ref row, ref col);
			return GetRectangle(row, col);
		}
		void GetRowCol(int index, ref int row, ref int col)
		{
			row = index / m_cols;
			col = index - (row * m_cols);
		}
		Rectangle GetRectangle(int row, int col)
		{
			int x = Padding.Left + (col * (m_fieldSize.Width + m_spacing));
			int y = Padding.Top + (row * (m_fieldSize.Height + m_spacing));
			return new Rectangle(x,y,m_fieldSize.Width, m_fieldSize.Height);
		}
		int GetIndexFromMousePos(int x, int y)
		{
			int col = (x-Padding.Left) / (m_fieldSize.Width + m_spacing);
			int row = (y-Padding.Top) / (m_fieldSize.Height + m_spacing);
			return GetIndex(row, col);
		}
		int GetIndex(int row, int col)
		{
			if (col < 0 || col >= m_cols)
				return -1;
			if (row < 0 || row >= m_rows)
				return -1;
			return row * m_cols + col;
		}
		void SetIndex(int index)
		{
			if (index == m_selindex)
				return;
			Invalidate(GetSelectedItemRect());
			m_selindex = index;
			if (SelectedIndexChanged != null)
				SelectedIndexChanged(this, null);
			Invalidate(GetSelectedItemRect());
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Focus();
			if (GetSelectedItemRect().Contains(new Point(e.X, e.Y)))
				return;
			int index = GetIndexFromMousePos(e.X, e.Y);
			if (index != -1)
				SetIndex(index);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			int index = 0;
			int totalwidth = m_cols * (m_fieldSize.Width + m_spacing);
			int totalheight = m_rows * (m_fieldSize.Height + m_spacing);

			int offset = (m_spacing / 2 + 1);
			Rectangle r = new Rectangle(0, 0, totalwidth, totalheight);
			r.X += Padding.Left - offset;
			r.Y += Padding.Top - offset;
			e.Graphics.DrawRectangle(Pens.CadetBlue, r);
			r.X++;
			r.Y++;
			r.Width--;
			r.Height--;
			e.Graphics.FillRectangle(Brushes.White, r);

			for (int col = 1; col < m_cols; col++)
			{
				int x = Padding.Left - offset + (col * (m_fieldSize.Width + m_spacing));
				e.Graphics.DrawLine(Pens.CadetBlue, x, r.Y, x, r.Bottom - 1);
			}
			for (int row = 1; row < m_rows; row++)
			{
				int y = Padding.Top - offset + (row * (m_fieldSize.Height + m_spacing));
				e.Graphics.DrawLine(Pens.CadetBlue, r.X, y, r.Right - 1, y);
			}
			
			for (int row = 0; row < m_rows; row++)
			{
				for (int col = 0; col < m_cols; col++)
				{
					if (index >= m_colors.Count)
						break;
					Rectangle rect = GetRectangle(row, col);
					using (SolidBrush brush = new SolidBrush(m_colors[index++]))
					{
						e.Graphics.FillRectangle(brush, rect);
					}
				}
			}
			if (m_selindex >= 0)
			{
				Rectangle rect = GetSelectedItemRect();
				e.Graphics.FillRectangle(Brushes.White, rect);
				rect.Inflate(-3, -3);
				using (SolidBrush brush = new SolidBrush(SelectedItem))
				{
					e.Graphics.FillRectangle(brush, rect);
				}
				if (Focused)
				{
				rect.Inflate(2, 2);
					ControlPaint.DrawFocusRectangle(e.Graphics, rect);
				}
				else
				{
					rect.X -= 2;
					rect.Y -= 2;
					rect.Width += 3;
					rect.Height += 3;
					e.Graphics.DrawRectangle(Pens.CadetBlue, rect);
				}
			}
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
		protected override bool ProcessDialogKey(Keys keyData)
		{
			bool processed = false;
			int row = 0;
			int col = 0;
			GetRowCol(m_selindex, ref row, ref col);

			switch (keyData)
			{
				case Keys.Down:
					row++;
					processed = true;
					break;
				case Keys.Up:
					row--;
					processed = true;
					break;
				case Keys.Left:
					col--;
					if (col < 0)
					{
						col = m_cols-1;
						row--;
					}
					processed = true;
					break;
				case Keys.Right:
					col++;
					if (col >= m_cols)
					{
						col = 0;
						row++;
					}
					processed = true;
					break;
			}
			if (processed)
			{
				int index = GetIndex(row, col);
				if (index != -1)
					SetIndex(index);
				return false;
			}
			return base.ProcessDialogKey(keyData);
		}
	}
}
