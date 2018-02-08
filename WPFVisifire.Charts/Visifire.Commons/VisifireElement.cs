using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Visifire.Charts;

namespace Visifire.Commons
{
	public abstract class VisifireElement : Control, IHref, IToolTip, IElement
	{
		public static readonly DependencyProperty ToolTipTextProperty = DependencyProperty.Register("ToolTipText", typeof(string), typeof(VisifireElement), new PropertyMetadata("", new PropertyChangedCallback(VisifireElement.ToolTipTextPropertyChanged)));

		private string _watermarkHref;

		private VisifireControl _control;

		private IToolTip _element;

		private FrameworkElement _visualObject;

		private string _tempHref;

		private HrefTargets _tempHrefTarget;

		public new event MouseButtonEventHandler MouseLeftButtonDown
		{
			add
			{
				this._onMouseLeftButtonDown += value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
			remove
			{
				this._onMouseLeftButtonDown -= value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
		}

		public new event MouseButtonEventHandler MouseLeftButtonUp
		{
			add
			{
				this._onMouseLeftButtonUp += value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
			remove
			{
				this._onMouseLeftButtonUp -= value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
		}

		public new event MouseButtonEventHandler MouseRightButtonDown
		{
			add
			{
				this._onMouseRightButtonDown += value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
			remove
			{
				this._onMouseRightButtonDown -= value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
		}

		public new event MouseButtonEventHandler MouseRightButtonUp
		{
			add
			{
				this._onMouseRightButtonUp += value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
			remove
			{
				this._onMouseRightButtonUp -= value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
		}

		public new event EventHandler<MouseEventArgs> MouseEnter
		{
			add
			{
				this._onMouseEnter += value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
			remove
			{
				this._onMouseEnter -= value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
		}

		public new event EventHandler<MouseEventArgs> MouseLeave
		{
			add
			{
				this._onMouseLeave += value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
			remove
			{
				this._onMouseLeave -= value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
		}

		public new event EventHandler<MouseEventArgs> MouseMove
		{
			add
			{
				this._onMouseMove += value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
			remove
			{
				this._onMouseMove -= value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
		}

		private event MouseButtonEventHandler _onMouseLeftButtonDown;

		private event MouseButtonEventHandler _onMouseLeftButtonUp;

		private event MouseButtonEventHandler _onMouseRightButtonDown;

		private event MouseButtonEventHandler _onMouseRightButtonUp;

		private event EventHandler<MouseEventArgs> _onMouseEnter;

		private event EventHandler<MouseEventArgs> _onMouseLeave;

		private event EventHandler<MouseEventArgs> _onMouseMove;

		internal event EventHandler EventChanged;

		public virtual string ToolTipText
		{
			get
			{
				return (string)base.GetValue(VisifireElement.ToolTipTextProperty);
			}
			set
			{
				base.SetValue(VisifireElement.ToolTipTextProperty, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AttachToolTip(VisifireControl control, IToolTip element, List<FrameworkElement> visualElements)
		{
			foreach (FrameworkElement current in visualElements)
			{
				this.AttachToolTip(control, element, current);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AttachToolTip(VisifireControl control, IToolTip element, FrameworkElement visualObject)
		{
			if (visualObject == null || control == null || element == null)
			{
				return;
			}
			this._control = control;
			this._element = element;
			this._visualObject = visualObject;
			visualObject.MouseMove += new MouseEventHandler(this.VisualObject_MouseMove4ToolTip);
			visualObject.MouseEnter += new MouseEventHandler(this.VisualObject_MouseEnter4ToolTip);
			visualObject.MouseLeave += new MouseEventHandler(this.VisualObject_MouseLeave4ToolTip);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void DetachToolTip(FrameworkElement visualObject)
		{
			if (visualObject != null)
			{
				visualObject.MouseMove -= new MouseEventHandler(this.VisualObject_MouseMove4ToolTip);
				visualObject.MouseEnter -= new MouseEventHandler(this.VisualObject_MouseEnter4ToolTip);
				visualObject.MouseLeave -= new MouseEventHandler(this.VisualObject_MouseLeave4ToolTip);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string TextParser(string unParsed)
		{
			return unParsed;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string ParseToolTipText(string unParsed)
		{
			return this.TextParser(unParsed);
		}

		protected void UpdateToolTip(object sender, MouseEventArgs e)
		{
			if (!string.IsNullOrEmpty(this._control.ToolTipText) && this._control != this._element)
			{
				return;
			}
			if (this._element.ToolTipText == null)
			{
				return;
			}
			string text = this._element.ParseToolTipText(this._element.ToolTipText);
			if (!string.IsNullOrEmpty(text) && this._control._toolTip != null)
			{
				this._control._toolTip.Text = text;
				if (this._control.ToolTipEnabled)
				{
					this._control._toolTip.Show();
				}
				(this._control as Chart).UpdateToolTipPosition(sender, e);
			}
		}

		private void Element_MouseEnter(object sender, MouseEventArgs e)
		{
			if (base.Cursor == null)
			{
				(sender as FrameworkElement).Cursor = Cursors.Hand;
			}
		}

		private void Element_MouseLeftButtonUp4Href(object sender, MouseButtonEventArgs e)
		{
			string arguments = this._tempHref;
			if (sender.GetType().Equals(typeof(TextBlock)) && (sender as FrameworkElement).Tag != null && (sender as FrameworkElement).Tag.Equals("Watermark") && !string.IsNullOrEmpty(this._watermarkHref))
			{
				arguments = this._watermarkHref;
			}
			Process.Start("explorer.exe", arguments);
		}

		private void VisualObject_MouseLeave4ToolTip(object sender, MouseEventArgs e)
		{
			string b = this._element.ParseToolTipText(this._element.ToolTipText);
			if (this._control._toolTip.Text == b)
			{
				this._control._toolTip.Hide();
			}
		}

		private void VisualObject_MouseEnter4ToolTip(object sender, MouseEventArgs e)
		{
			if (!string.IsNullOrEmpty(this._control.ToolTipText) && this._control != this._element)
			{
				return;
			}
			this._control._toolTip.CallOutVisiblity = Visibility.Collapsed;
			this.UpdateToolTip(sender, e);
		}

		private void VisualObject_MouseMove4ToolTip(object sender, MouseEventArgs e)
		{
			if (!string.IsNullOrEmpty(this._control.ToolTipText) && this._control != this._element)
			{
				return;
			}
			this._control._toolTip.CallOutVisiblity = Visibility.Collapsed;
			this.UpdateToolTip(sender, e);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AttachEvents2Visual(VisifireElement obj, FrameworkElement visual)
		{
			if (visual != null)
			{
				this.AttachEvents2Visual(obj, obj, visual);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AttachEvents2Visual(VisifireElement chartElement, IElement senderElement, FrameworkElement visual)
		{
			if (visual == null)
			{
				return;
			}
			object obj = chartElement.GetMouseEnterEventHandler();
			if (obj != null)
			{
				visual.MouseEnter += delegate(object sender, MouseEventArgs e)
				{
					chartElement.FireMouseEnterEvent(senderElement, e);
				};
			}
			obj = chartElement.GetMouseLeaveEventHandler();
			if (obj != null)
			{
				visual.MouseLeave += delegate(object sender, MouseEventArgs e)
				{
					chartElement.FireMouseLeaveEvent(senderElement, e);
				};
			}
			obj = chartElement.GetMouseLeftButtonDownEventHandler();
			if (obj != null)
			{
				visual.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
				{
					chartElement.FireMouseLeftButtonDownEvent(senderElement, e);
				};
			}
			obj = chartElement.GetMouseLeftButtonUpEventHandler();
			if (obj != null)
			{
				visual.MouseLeftButtonUp += delegate(object sender, MouseButtonEventArgs e)
				{
					chartElement.FireMouseLeftButtonUpEvent(senderElement, e);
				};
			}
			obj = chartElement.GetMouseMoveEventHandler();
			if (obj != null)
			{
				visual.MouseMove += delegate(object sender, MouseEventArgs e)
				{
					chartElement.FireMouseMoveEvent(senderElement, e);
				};
			}
			obj = chartElement.GetMouseRightButtonDownEventHandler();
			if (obj != null)
			{
				visual.MouseRightButtonDown += delegate(object sender, MouseButtonEventArgs e)
				{
					chartElement.FireMouseRightButtonDownEvent(senderElement, e);
				};
			}
			obj = chartElement.GetMouseRightButtonUpEventHandler();
			if (obj != null)
			{
				visual.MouseRightButtonUp += delegate(object sender, MouseButtonEventArgs e)
				{
					chartElement.FireMouseRightButtonUpEvent(senderElement, e);
				};
			}
		}

		internal virtual void FireMouseRightButtonDownEvent(object sender, object e)
		{
			if (this._onMouseRightButtonDown != null)
			{
				this._onMouseRightButtonDown(sender, e as MouseButtonEventArgs);
			}
		}

		internal virtual void FireMouseRightButtonUpEvent(object sender, object e)
		{
			if (this._onMouseRightButtonUp != null)
			{
				this._onMouseRightButtonUp(sender, e as MouseButtonEventArgs);
			}
		}

		internal virtual void FireMouseLeftButtonDownEvent(object sender, object e)
		{
			if (this._onMouseLeftButtonDown != null)
			{
				this._onMouseLeftButtonDown(sender, e as MouseButtonEventArgs);
			}
		}

		internal virtual void FireMouseLeftButtonUpEvent(object sender, object e)
		{
			if (this._onMouseLeftButtonUp != null)
			{
				this._onMouseLeftButtonUp(sender, e as MouseButtonEventArgs);
			}
		}

		internal virtual void FireMouseMoveEvent(object sender, object e)
		{
			if (this._onMouseMove != null)
			{
				this._onMouseMove(sender, e as MouseEventArgs);
			}
		}

		internal virtual void FireMouseEnterEvent(object sender, object e)
		{
			if (this._onMouseEnter != null)
			{
				this._onMouseEnter(sender, e as MouseEventArgs);
			}
		}

		internal virtual void FireMouseLeaveEvent(object sender, object e)
		{
			if (this._onMouseLeave != null)
			{
				this._onMouseLeave(sender, e as MouseEventArgs);
			}
		}

		internal virtual object GetMouseLeftButtonDownEventHandler()
		{
			return this._onMouseLeftButtonDown;
		}

		internal virtual object GetMouseLeftButtonUpEventHandler()
		{
			return this._onMouseLeftButtonUp;
		}

		internal virtual object GetMouseMoveEventHandler()
		{
			return this._onMouseMove;
		}

		internal virtual object GetMouseEnterEventHandler()
		{
			return this._onMouseEnter;
		}

		internal virtual object GetMouseLeaveEventHandler()
		{
			return this._onMouseLeave;
		}

		internal virtual object GetMouseRightButtonDownEventHandler()
		{
			return this._onMouseRightButtonDown;
		}

		internal virtual object GetMouseRightButtonUpEventHandler()
		{
			return this._onMouseRightButtonUp;
		}

		internal void AttachEventWithHighPriority(string eventType, Delegate dlg)
		{
			if (this._onMouseLeftButtonUp != null)
			{
				Delegate[] invocationList = this._onMouseLeftButtonUp.GetInvocationList();
				Delegate[] array = invocationList;
				for (int i = 0; i < array.Length; i++)
				{
					Delegate @delegate = array[i];
					this.MouseLeftButtonUp -= (MouseButtonEventHandler)@delegate;
				}
				this.MouseLeftButtonUp += (MouseButtonEventHandler)dlg;
				Delegate[] array2 = invocationList;
				for (int j = 0; j < array2.Length; j++)
				{
					Delegate delegate2 = array2[j];
					this.MouseLeftButtonUp += (MouseButtonEventHandler)delegate2;
				}
				return;
			}
			this.MouseLeftButtonUp += (MouseButtonEventHandler)dlg;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AttachEvents2Visual4MouseDownEvent(IElement obj, IElement senderElement, FrameworkElement visual)
		{
		}

		internal static void ToolTipTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as VisifireElement).OnToolTipTextPropertyChanged((string)e.NewValue);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OnToolTipTextPropertyChanged(string newValue)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AttachHref(VisifireControl control, List<FrameworkElement> visualElements, string href, HrefTargets hrefTarget)
		{
			foreach (FrameworkElement current in visualElements)
			{
				this.AttachHref(control, current, href, hrefTarget);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AttachHref(VisifireControl control, FrameworkElement visualElement, string href, HrefTargets hrefTarget)
		{
			if (visualElement == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(href))
			{
				bool flag = false;
				if (visualElement.GetType().Equals(typeof(TextBlock)) && visualElement.Tag != null && visualElement.Tag.Equals("Watermark"))
				{
					flag = true;
				}
				if (!flag)
				{
					this._tempHref = href;
				}
				else
				{
					this._watermarkHref = href;
				}
				this._tempHrefTarget = hrefTarget;
				visualElement.MouseEnter -= new MouseEventHandler(this.Element_MouseEnter);
				visualElement.MouseEnter += new MouseEventHandler(this.Element_MouseEnter);
				visualElement.MouseLeftButtonUp -= new MouseButtonEventHandler(this.Element_MouseLeftButtonUp4Href);
				visualElement.MouseLeftButtonUp += new MouseButtonEventHandler(this.Element_MouseLeftButtonUp4Href);
			}
		}

		internal virtual void ClearInstanceRefs()
		{
		}
	}
}
