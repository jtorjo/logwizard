using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ColorPicker
{
	public class ColorPickerCombobox : DropdownContainerControl<Color>
	{
		static ColorPickerCtrl m_colorPicker;
		static DropdownContainer<Color>	m_container;
		public ColorPickerCombobox()
		{
			SelectedItem = Color.Wheat;
			if (m_colorPicker == null)
			{
				m_container = new DropdownContainer<Color>(this);
				m_colorPicker = new ColorPickerCtrl();
				m_container.SetControl(m_colorPicker);
			}
			DropdownContainer = m_container;
		}
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
		}
		protected override void ShowDropdown()
		{
			m_colorPicker.SelectedColor = SelectedItem;
			// only need to register / unregister because the dropdown is static and shared
			DropdownContainer.KeyDown += new KeyEventHandler(OnDropdownKeyDown);
			base.ShowDropdown();
		}
		public override void CloseDropdown(bool acceptValue)
		{
			DropdownContainer.KeyDown -= new KeyEventHandler(OnDropdownKeyDown);
			base.CloseDropdown(acceptValue);
			if (acceptValue)
				SelectedItem = m_colorPicker.SelectedColor;
		}
		protected override void DrawItem(Graphics dc, Rectangle itemrect)
		{
			Brush b = new SolidBrush(SelectedItem);
			dc.FillRectangle(b, itemrect);
		}
		void OnDropdownKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				e.Handled = true;
				CloseDropdown(true);
			}
		}
	}
	public class DropdownContainerControl<T> : Control
	{
		T m_selectedItem;
		bool					m_mouseIn = false;
		DropdownContainer<T>	m_container = null;

		public T SelectedItem
		{
			get { return m_selectedItem; } 
			set { m_selectedItem = value; }
		}

		public DropdownContainer<T> DropdownContainer
		{
			get { return m_container; }
			set { m_container = value; }
		}
		public bool DroppedDown
		{
			get { return m_container.Visible; }
		}

		public DropdownContainerControl()
		{
			this.DoubleBuffered = true;
			m_container = new DropdownContainer<T>(this);
		}
		public virtual void CloseDropdown(bool acceptValue)
		{
			HideDropdown();
		}
		public Rectangle ItemRectangle
		{
			get
			{
				Rectangle r = ClientRectangle;
				r.Y	+= 2;
				r.Height -= 4;
				r.X += 2;
				r.Width = ButtonRectangle.Left - 4;
				return r;
			}
		}
		public Rectangle ButtonRectangle
		{
			get
			{
				Rectangle r = ClientRectangle;
				r.Y += 1;
				r.Height -= 2;
				r.X = r.Right - 18;
				r.Width = 17;
				return r;
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			//base.OnPaint(e);
			Rectangle r = ClientRectangle;
			ComboBoxRenderer.DrawTextBox(e.Graphics, r, System.Windows.Forms.VisualStyles.ComboBoxState.Normal);
			r = ButtonRectangle;
			if (m_mouseIn)
				ComboBoxRenderer.DrawDropDownButton(e.Graphics, r, System.Windows.Forms.VisualStyles.ComboBoxState.Hot);
			else
				ComboBoxRenderer.DrawDropDownButton(e.Graphics, r, System.Windows.Forms.VisualStyles.ComboBoxState.Normal);
			r = ItemRectangle;
			r.Inflate(-1,-1);
			DrawItem(e.Graphics, ItemRectangle);
			if (Focused)
				ControlPaint.DrawFocusRectangle(e.Graphics, ItemRectangle);
			RaisePaintEvent(this, e);
		}
		protected virtual void DrawItem(Graphics dc, Rectangle itemrect)
		{
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
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button == MouseButtons.Left)
			{
				Focus();
				if (DroppedDown)
					HideDropdown();
				else
					ShowDropdown();
			}
		}
		protected virtual void ShowDropdown()
		{
			Point location = Parent.PointToScreen(Location);
			location.Y += Height + 2;
			// adjust dropdown location in case it goes off the screen;
			Rectangle r = Parent.RectangleToScreen(Bounds);
			Rectangle screen = Screen.GetWorkingArea(this);
			if (location.X + m_container.Width > screen.Right)
				location.X = r.Right - m_container.Width;
			if (location.X < 0)
				location.X = 0;
			
			if (location.Y + m_container.Height > screen.Bottom)
				location.Y = r.Top - m_container.Height;
			if (location.Y < 0)
				location.Y = 0;

			m_container.Location = location;
			m_container.ShowDropdown(this);
		}
		protected virtual void HideDropdown()
		{
			m_container.Hide();
		}
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			m_mouseIn = true;
			Invalidate();
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			m_mouseIn = false;
			Invalidate();
		}
		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == Keys.Down && DroppedDown == false)
			{
				ShowDropdown();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
	}
	public class DropdownContainer<T> : Form
	{
		int m_captionHeight = 18;
		int m_buttonAreaHeight = 24;
		int m_frameMargin = 4;
		Button m_acceptButton;
		Button m_cancelButton;
		DropdownContainerControl<T> m_owner;
		Hook m_hook = new Hook();
		public DropdownContainer(DropdownContainerControl<T> owner)
		{
			m_owner = owner;

			// this is for the window to show at correct location first time it is opened
			WinUtil.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, WinUtil.SWP_NOACTIVATE);
			
			FormBorderStyle = FormBorderStyle.None;
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

			SetTopLevel(true);
			ShowInTaskbar = false;
			Hide();


			Rectangle r = WindowRectangle;
			r.Inflate(-2,-2);
			r.Y--;
			m_cancelButton = new Button();
			m_cancelButton.Text = "Cancel";
			m_cancelButton.Size = new Size(60,m_buttonAreaHeight-2);
			m_cancelButton.Location = new Point(r.Right - m_cancelButton.Size.Width, r.Bottom - m_cancelButton.Height);
			m_cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			m_cancelButton.Click += new EventHandler(OnCancelClick);
			Controls.Add(m_cancelButton);
			
			m_acceptButton = new Button();
			m_acceptButton.Text = "Accept";
			m_acceptButton.Size = new Size(60,m_buttonAreaHeight-2);
			m_acceptButton.Location = new Point(m_cancelButton.Left - m_acceptButton.Size.Width - 1, r.Bottom - m_acceptButton.Height);
			m_acceptButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			m_acceptButton.Click += new EventHandler(OnAcceptClick);
			Controls.Add(m_acceptButton);

			m_hook.OnKeyDown += new Hook.KeyboardDelegate(OnHookKeyDown);
		}
		public virtual Rectangle WindowRectangle
		{
			get { return base.ClientRectangle; }
		}
		public virtual new Rectangle ClientRectangle
		{
			get 
			{ 
				Rectangle r = WindowRectangle;
				r.Y += m_frameMargin + m_captionHeight;
				r.Height -= m_frameMargin*2 + m_captionHeight + m_buttonAreaHeight;
				r.X	+= m_frameMargin;
				r.Width -= m_frameMargin*2;
				return r; 
			}
		}
		public void SetControl(Control ctrl)
		{
			Controls.Clear();
			if (m_acceptButton != null)
				Controls.Add(m_acceptButton);
			if (m_cancelButton != null)
				Controls.Add(m_cancelButton);
			
			Rectangle client = ClientRectangle;
			this.Width += ctrl.Width - client.Width;
			this.Height += ctrl.Height - client.Height;
			ctrl.Location = ClientRectangle.Location;
			Controls.Add(ctrl);

			ctrl.TabIndex = 0;
			m_acceptButton.TabIndex = 1;
			m_cancelButton.TabIndex = 2;
		}
		public virtual void Cancel()
		{
			m_hook.SetHook(false);
			Hide();
			if (m_owner != null)
				m_owner.CloseDropdown(false);
		}
		public virtual void Accept()
		{
			m_hook.SetHook(false);
			Hide();
			if (m_owner != null)
				m_owner.CloseDropdown(true);
		}
		public virtual void ShowDropdown(DropdownContainerControl<T> owner)
		{
			if (owner != null)
				m_owner = owner;

			Show();
			m_hook.SetHook(true);
		}
		protected override void OnDeactivate(EventArgs e)
		{
			base.OnDeactivate(e);
			Cancel();
		}
		protected virtual Rectangle CancelButtonRect
		{
			get
			{
				Rectangle r = WindowRectangle;
				r.Y += 2;
				r.Height = m_captionHeight;
				r.X = r.Width - (m_captionHeight + 2);
				r.Width = m_captionHeight;
				return r;
			}
		}
		protected virtual Rectangle AcceptButtonRect
		{
			get
			{
				Rectangle r = CancelButtonRect;
				r.X -= (m_captionHeight + 2);
				return r;
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			//base.OnPaint(e);
			Rectangle r = WindowRectangle;
			r.Width--;
			r.Height--;
			Util.DrawFrame(e.Graphics, r, 6, Color.CadetBlue);

			e.Graphics.DrawImage(PopupContainerImages.Image(PopupContainerImages.eIndexes.Close), CancelButtonRect);
			e.Graphics.DrawImage(PopupContainerImages.Image(PopupContainerImages.eIndexes.Check), AcceptButtonRect);
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Point p = new Point(e.X, e.Y);
			if (AcceptButtonRect.Contains(p))
				Accept();
			if (CancelButtonRect.Contains(p))
				Cancel();
		}
		void OnHookKeyDown(KeyEventArgs e)
		{
			OnKeyDown(e);
			if (e.Handled)
				return;
			if (e.KeyCode == Keys.Escape)
			{
				Cancel();
				e.Handled = true;
			}
			if (e.KeyCode == Keys.Return)
			{
				Accept();
				e.Handled = true;
			}
		}
		void OnAcceptClick(object sender, EventArgs e)
		{
			Accept();
		}
		void OnCancelClick(object sender, EventArgs e)
		{
			Cancel();
		}
	}

}
