using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Visifire.Commons.Controls
{
	[TemplatePart(Name = "VerticalLargeIncrease", Type = typeof(RepeatButton)), TemplatePart(Name = "VerticalLargeDecrease", Type = typeof(RepeatButton)), TemplatePart(Name = "VerticalThumb", Type = typeof(Thumb)), TemplatePart(Name = "HorizontalThumb", Type = typeof(Thumb)), TemplatePart(Name = "HorizontalSmallDecrease", Type = typeof(RepeatButton)), TemplatePart(Name = "VerticalRoot", Type = typeof(FrameworkElement)), TemplatePart(Name = "HorizontalGripLeftElement", Type = typeof(Thumb)), TemplatePart(Name = "HorizontalLargeIncrease", Type = typeof(RepeatButton)), TemplatePart(Name = "HorizontalSmallIncrease", Type = typeof(RepeatButton)), TemplatePart(Name = "VerticalSmallDecrease", Type = typeof(RepeatButton)), TemplatePart(Name = "VerticalSmallIncrease", Type = typeof(RepeatButton)), TemplatePart(Name = "HorizontalRoot", Type = typeof(FrameworkElement)), TemplatePart(Name = "HorizontalLargeDecrease", Type = typeof(RepeatButton)), TemplateVisualState(Name = "Normal", GroupName = "CommonStates"), TemplateVisualState(Name = "Disabled", GroupName = "CommonStates"), TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
	public class ZoomBar : RangeBase
	{
		internal const string ElementHorizontalLargeDecreaseName = "HorizontalLargeDecrease";

		internal const string ElementHorizontalLargeIncreaseName = "HorizontalLargeIncrease";

		internal const string ElementHorizontalSmallDecreaseName = "HorizontalSmallDecrease";

		internal const string ElementHorizontalSmallIncreaseName = "HorizontalSmallIncrease";

		internal const string ElementHorizontalTemplateName = "HorizontalRoot";

		internal const string ElementHorizontalThumbName = "HorizontalThumb";

		internal const string ElementLeftGripName = "ElementLeftGrip";

		internal const string ElementRightGripName = "ElementRightGrip";

		internal const string ElementVerticalLargeDecreaseName = "VerticalLargeDecrease";

		internal const string ElementVerticalLargeIncreaseName = "VerticalLargeIncrease";

		internal const string ElementVerticalSmallDecreaseName = "VerticalSmallDecrease";

		internal const string ElementVerticalSmallIncreaseName = "VerticalSmallIncrease";

		internal const string ElementVerticalTemplateName = "VerticalRoot";

		internal const string ElementVerticalThumbName = "VerticalThumb";

		internal const string ElementTopGripName = "ElementTopGrip";

		internal const string ElementBottomGripName = "ElementBottomGrip";

		internal const string GroupCommon = "CommonStates";

		internal const string StateDisabled = "Disabled";

		internal const string StateMouseOver = "MouseOver";

		internal const string StateNormal = "Normal";

		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ZoomBar), new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(ZoomBar.OnOrientationPropertyChanged)));

		public static readonly DependencyProperty ViewportSizeProperty = DependencyProperty.Register("ViewportSize", typeof(double), typeof(ZoomBar), new PropertyMetadata(0.0, new PropertyChangedCallback(ZoomBar.OnViewportSizeChanged)));

		public static readonly DependencyProperty ZoomingEnabledProperty = DependencyProperty.Register("ZoomingEnabled", typeof(bool), typeof(ZoomBar), new PropertyMetadata(false, new PropertyChangedCallback(ZoomBar.OnZoomingEnabledChanged)));

		public static readonly DependencyProperty ScrollEventFireEnabledProperty = DependencyProperty.Register("ScrollEventFireEnabled", typeof(bool), typeof(ZoomBar), new PropertyMetadata(true, new PropertyChangedCallback(ZoomBar.OnScrollEventFireEnabledPropertyChanged)));

		private Border ElementCenterBorder;

		internal RepeatButton _elementHorizontalLargeDecrease;

		internal RepeatButton _elementHorizontalLargeIncrease;

		internal RepeatButton _elementHorizontalSmallDecrease;

		internal RepeatButton _elementHorizontalSmallIncrease;

		internal FrameworkElement _elementHorizontalTemplate;

		internal Thumb _elementHorizontalThumb;

		internal Thumb _elementLeftGrip;

		internal Thumb _elementRightGrip;

		internal RepeatButton _elementVerticalLargeDecrease;

		internal RepeatButton _elementVerticalLargeIncrease;

		internal RepeatButton _elementVerticalSmallDecrease;

		internal RepeatButton _elementVerticalSmallIncrease;

		internal FrameworkElement _elementVerticalTemplate;

		internal Thumb _elementVerticalThumb;

		internal Thumb _elementTopGrip;

		internal Thumb _elementBottomGrip;

		private double _dragValue;

		internal bool _isZoomedUsingZoomRect;

		private double _currentThumbSize = double.NaN;

		private double _oldTrackLength = double.NaN;

		internal bool _isNotificationEnabled = true;

		public event ScrollEventHandler Scroll;

		public event EventHandler ScaleChanged;

		public event EventHandler DragStarted;

		public event EventHandler DragCompleted;

		public event EventHandler Drag;

		public Orientation Orientation
		{
			get
			{
				return (Orientation)base.GetValue(ZoomBar.OrientationProperty);
			}
			set
			{
				base.SetValue(ZoomBar.OrientationProperty, value);
			}
		}

		public double ViewportSize
		{
			get
			{
				return (double)base.GetValue(ZoomBar.ViewportSizeProperty);
			}
			set
			{
				base.SetValue(ZoomBar.ViewportSizeProperty, value);
			}
		}

		public bool IsStretchable
		{
			get
			{
				return (bool)base.GetValue(ZoomBar.ZoomingEnabledProperty);
			}
			set
			{
				base.SetValue(ZoomBar.ZoomingEnabledProperty, value);
			}
		}

		internal double Scale
		{
			get;
			set;
		}

		internal double ZoomBarStartPosition
		{
			get;
			private set;
		}

		internal double ZoomBarEndPosition
		{
			get;
			private set;
		}

		public bool ScrollEventFireEnabled
		{
			get
			{
				return (bool)base.GetValue(ZoomBar.ScrollEventFireEnabledProperty);
			}
			set
			{
				base.SetValue(ZoomBar.ScrollEventFireEnabledProperty, value);
			}
		}

		internal bool IsStretching
		{
			get;
			private set;
		}

		internal double ThumbSize
		{
			get
			{
				return this._currentThumbSize;
			}
		}

		internal bool IsDragging
		{
			get
			{
				if (this.Orientation == Orientation.Horizontal && this._elementHorizontalThumb != null)
				{
					return this._elementHorizontalThumb.IsDragging;
				}
				return this.Orientation == Orientation.Vertical && this._elementVerticalThumb != null && this._elementVerticalThumb.IsDragging;
			}
		}

		private double MinThumbSize
		{
			get;
			set;
		}

		public ZoomBar()
		{
			this.ScrollEventFireEnabled = true;
			base.SizeChanged += delegate(object A_1, SizeChangedEventArgs A_2)
			{
				this.UpdateTrackLayout(this.GetTrackLength());
			};
			this._currentThumbSize = double.NaN;
			base.DefaultStyleKey = typeof(ZoomBar);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this._elementHorizontalTemplate = (base.GetTemplateChild("HorizontalRoot") as FrameworkElement);
			this._elementHorizontalLargeIncrease = (base.GetTemplateChild("HorizontalLargeIncrease") as RepeatButton);
			this._elementHorizontalLargeDecrease = (base.GetTemplateChild("HorizontalLargeDecrease") as RepeatButton);
			this._elementHorizontalSmallIncrease = (base.GetTemplateChild("HorizontalSmallIncrease") as RepeatButton);
			this._elementHorizontalSmallDecrease = (base.GetTemplateChild("HorizontalSmallDecrease") as RepeatButton);
			this._elementHorizontalThumb = (base.GetTemplateChild("HorizontalThumb") as Thumb);
			this._elementVerticalTemplate = (base.GetTemplateChild("VerticalRoot") as FrameworkElement);
			this._elementVerticalLargeIncrease = (base.GetTemplateChild("VerticalLargeIncrease") as RepeatButton);
			this._elementVerticalLargeDecrease = (base.GetTemplateChild("VerticalLargeDecrease") as RepeatButton);
			this._elementVerticalSmallIncrease = (base.GetTemplateChild("VerticalSmallIncrease") as RepeatButton);
			this._elementVerticalSmallDecrease = (base.GetTemplateChild("VerticalSmallDecrease") as RepeatButton);
			this._elementVerticalThumb = (base.GetTemplateChild("VerticalThumb") as Thumb);
			this._elementHorizontalThumb.Height = double.NaN;
			this._elementVerticalThumb.Width = double.NaN;
			if (this.Orientation == Orientation.Horizontal)
			{
				this.MinThumbSize = 1.5;
				this._elementHorizontalThumb.MinWidth = this.MinThumbSize;
			}
			else
			{
				this.MinThumbSize = 1.0;
				this._elementVerticalThumb.MinHeight = this.MinThumbSize;
			}
			this._elementHorizontalThumb.LayoutUpdated += new EventHandler(this.ElementLeftGrip_LayoutUpdated);
			if (this._elementHorizontalThumb != null)
			{
				this._elementHorizontalThumb.DragStarted += delegate(object sender, DragStartedEventArgs e)
				{
					this.OnThumbDragStarted();
				};
				this._elementHorizontalThumb.DragDelta += delegate(object sender, DragDeltaEventArgs e)
				{
					this.OnThumbDragDelta(e);
				};
				this._elementHorizontalThumb.DragCompleted += delegate(object sender, DragCompletedEventArgs e)
				{
					this.OnThumbDragCompleted();
				};
			}
			if (this._elementHorizontalLargeDecrease != null)
			{
				this._elementHorizontalLargeDecrease.Click += delegate(object sender, RoutedEventArgs e)
				{
					this.LargeDecrement();
				};
			}
			if (this._elementHorizontalLargeIncrease != null)
			{
				this._elementHorizontalLargeIncrease.Click += delegate(object sender, RoutedEventArgs e)
				{
					this.LargeIncrement();
				};
			}
			if (this._elementHorizontalSmallDecrease != null)
			{
				this._elementHorizontalSmallDecrease.Click += delegate(object sender, RoutedEventArgs e)
				{
					this.SmallDecrement();
				};
			}
			if (this._elementHorizontalSmallIncrease != null)
			{
				this._elementHorizontalSmallIncrease.Click += delegate(object sender, RoutedEventArgs e)
				{
					this.SmallIncrement();
				};
			}
			if (this._elementVerticalThumb != null)
			{
				this._elementVerticalThumb.DragStarted += delegate(object sender, DragStartedEventArgs e)
				{
					this.OnThumbDragStarted();
				};
				this._elementVerticalThumb.DragDelta += delegate(object sender, DragDeltaEventArgs e)
				{
					this.OnThumbDragDelta(e);
				};
				this._elementVerticalThumb.DragCompleted += delegate(object sender, DragCompletedEventArgs e)
				{
					this.OnThumbDragCompleted();
				};
			}
			if (this._elementVerticalLargeDecrease != null)
			{
				this._elementVerticalLargeDecrease.Click += delegate(object sender, RoutedEventArgs e)
				{
					this.LargeDecrement();
				};
			}
			if (this._elementVerticalLargeIncrease != null)
			{
				this._elementVerticalLargeIncrease.Click += delegate(object sender, RoutedEventArgs e)
				{
					this.LargeIncrement();
				};
			}
			if (this._elementVerticalSmallDecrease != null)
			{
				this._elementVerticalSmallDecrease.Click += delegate(object sender, RoutedEventArgs e)
				{
					this.SmallDecrement();
				};
			}
			if (this._elementVerticalSmallIncrease != null)
			{
				this._elementVerticalSmallIncrease.Click += delegate(object sender, RoutedEventArgs e)
				{
					this.SmallIncrement();
				};
			}
			this.OnOrientationChanged();
			this.UpdateVisualState();
		}

		public static IEnumerable<DependencyObject> GetVisuals(DependencyObject root)
		{
			int childrenCount = VisualTreeHelper.GetChildrenCount(root);
			for (int i = 0; i < childrenCount; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(root, i);
				yield return child;
				foreach (DependencyObject current in ZoomBar.GetVisuals(child))
				{
					yield return current;
				}
			}
			yield break;
		}

		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			this.UpdateVisualState();
		}

		protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
		{
			if (!this.IsStretching)
			{
				double trackLength = this.GetTrackLength();
				base.OnMaximumChanged(oldMaximum, newMaximum);
				this.UpdateTrackLayout(trackLength);
			}
		}

		protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
		{
			if (!this.IsStretching)
			{
				double trackLength = this.GetTrackLength();
				base.OnMinimumChanged(oldMinimum, newMinimum);
				this.UpdateTrackLayout(trackLength);
			}
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			if ((this.Orientation == Orientation.Horizontal && this._elementHorizontalThumb != null && !this._elementHorizontalThumb.IsDragging) || (this.Orientation == Orientation.Vertical && this._elementVerticalThumb != null && !this._elementVerticalThumb.IsDragging))
			{
				this.UpdateVisualState();
			}
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			if ((this.Orientation == Orientation.Horizontal && this._elementHorizontalThumb != null && !this._elementHorizontalThumb.IsDragging) || (this.Orientation == Orientation.Vertical && this._elementVerticalThumb != null && !this._elementVerticalThumb.IsDragging))
			{
				this.UpdateVisualState();
			}
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			if (!e.Handled)
			{
				e.Handled = true;
				base.CaptureMouse();
			}
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);
			if (!e.Handled)
			{
				e.Handled = true;
			}
		}

		protected override void OnValueChanged(double oldValue, double newValue)
		{
			if (this._isNotificationEnabled)
			{
				double trackLength = this.GetTrackLength();
				if (trackLength >= 0.0)
				{
					base.OnValueChanged(oldValue, newValue);
					this.UpdateTrackLayout(trackLength);
				}
			}
		}

		private double ConvertViewportSizeToDisplayUnits(double trackLength)
		{
			if (!this.IsStretchable)
			{
				double num = base.Maximum - base.Minimum;
				return trackLength * this.ViewportSize / (this.ViewportSize + num);
			}
			if (double.IsNaN(this._currentThumbSize))
			{
				this._currentThumbSize = trackLength;
				this._oldTrackLength = trackLength;
				return this._currentThumbSize;
			}
			if (this._oldTrackLength != trackLength)
			{
				if (trackLength < this._oldTrackLength)
				{
					this._currentThumbSize -= Math.Abs(trackLength - this._oldTrackLength);
				}
				else
				{
					this._currentThumbSize += Math.Abs(trackLength - this._oldTrackLength);
				}
				this._oldTrackLength = trackLength;
				return this._currentThumbSize;
			}
			return this._currentThumbSize;
		}

		private void LargeDecrement()
		{
			if (!this.IsStretching)
			{
				double num = Math.Max(base.Value - base.LargeChange, base.Minimum);
				if (base.Value != num)
				{
					base.Value = num;
					this.RaiseScrollEvent(ScrollEventType.LargeDecrement);
				}
			}
		}

		private void LargeIncrement()
		{
			if (!this.IsStretching)
			{
				double num = Math.Min(base.Value + base.LargeChange, base.Maximum);
				if (base.Value != num)
				{
					base.Value = num;
					this.RaiseScrollEvent(ScrollEventType.LargeIncrement);
				}
			}
		}

		private void ElementLeftGrip_LayoutUpdated(object sender, EventArgs e)
		{
			this.SetReferenceOfGrips();
		}

		private void OnOrientationChanged()
		{
			double trackLength = this.GetTrackLength();
			if (this._elementHorizontalTemplate != null)
			{
				this._elementHorizontalTemplate.Visibility = ((this.Orientation == Orientation.Horizontal) ? Visibility.Visible : Visibility.Collapsed);
			}
			if (this._elementVerticalTemplate != null)
			{
				this._elementVerticalTemplate.Visibility = ((this.Orientation == Orientation.Horizontal) ? Visibility.Collapsed : Visibility.Visible);
			}
			this.UpdateTrackLayout(trackLength);
		}

		private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as ZoomBar).OnOrientationChanged();
		}

		private void OnThumbDragCompleted()
		{
			this.RaiseScrollEvent(ScrollEventType.EndScroll);
		}

		private void OnThumbDragDelta(DragDeltaEventArgs e)
		{
			if (!this.IsStretching)
			{
				double num = 0.0;
				if (this.Orientation == Orientation.Horizontal && this._elementHorizontalThumb != null)
				{
					num = e.HorizontalChange / (this.GetTrackLength() - this._elementHorizontalThumb.ActualWidth) * (base.Maximum - base.Minimum);
				}
				else if (this.Orientation == Orientation.Vertical && this._elementVerticalThumb != null)
				{
					num = e.VerticalChange / (this.GetTrackLength() - this._elementVerticalThumb.ActualHeight) * (base.Maximum - base.Minimum);
				}
				if (!double.IsNaN(num) && !double.IsInfinity(num))
				{
					this._dragValue += num;
					double num2 = Math.Min(base.Maximum, Math.Max(base.Minimum, this._dragValue));
					if (num2 != base.Value)
					{
						base.Value = num2;
						this.RaiseScrollEvent(ScrollEventType.ThumbTrack);
					}
				}
			}
		}

		private void OnThumbDragStarted()
		{
			this._dragValue = base.Value;
		}

		private static void OnViewportSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ZoomBar zoomBar = d as ZoomBar;
			if (!zoomBar.IsStretchable)
			{
				zoomBar.UpdateTrackLayout(zoomBar.GetTrackLength());
			}
		}

		private static void OnScrollEventFireEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ZoomBar zoomBar = d as ZoomBar;
			if (!(bool)e.OldValue && (bool)e.NewValue)
			{
				zoomBar.RaiseScrollEvent(ScrollEventType.ThumbPosition);
			}
		}

		private static void OnZoomingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Visibility visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
			ZoomBar zoomBar = d as ZoomBar;
			if (zoomBar._elementLeftGrip != null)
			{
				zoomBar._elementLeftGrip.Visibility = visibility;
			}
			if (zoomBar._elementRightGrip != null)
			{
				zoomBar._elementRightGrip.Visibility = visibility;
			}
			if (zoomBar._elementTopGrip != null)
			{
				zoomBar._elementTopGrip.Visibility = visibility;
			}
			if (zoomBar._elementBottomGrip != null)
			{
				zoomBar._elementBottomGrip.Visibility = visibility;
			}
		}

		private void SmallDecrement()
		{
			if (!this.IsStretching)
			{
				double num = Math.Max(base.Value - base.SmallChange, base.Minimum);
				if (base.Value != num)
				{
					base.Value = num;
					this.RaiseScrollEvent(ScrollEventType.SmallDecrement);
				}
			}
		}

		private void SmallIncrement()
		{
			if (!this.IsStretching)
			{
				double num = Math.Min(base.Value + base.SmallChange, base.Maximum);
				if (base.Value != num)
				{
					base.Value = num;
					this.RaiseScrollEvent(ScrollEventType.SmallIncrement);
				}
			}
		}

		private void CalculateZoomStartPosition()
		{
			if (this.Orientation == Orientation.Horizontal)
			{
				this.ZoomBarStartPosition = this._elementHorizontalLargeDecrease.Width + this._elementHorizontalSmallDecrease.ActualWidth;
				return;
			}
			this.ZoomBarStartPosition = this._elementVerticalLargeDecrease.Height + this._elementVerticalSmallDecrease.ActualHeight;
		}

		private void CalculateZoomEndPosition()
		{
			if (this.Orientation == Orientation.Horizontal)
			{
				this.ZoomBarEndPosition = base.ActualWidth - (this._elementHorizontalLargeDecrease.Width + this._elementHorizontalSmallDecrease.ActualWidth + this._currentThumbSize);
				return;
			}
			this.ZoomBarEndPosition = base.ActualHeight - (this._elementVerticalLargeDecrease.Height + this._elementVerticalSmallDecrease.ActualHeight + this._currentThumbSize);
		}

		private void SetReferenceOfGrips()
		{
			DragStartedEventHandler value = delegate(object sender, DragStartedEventArgs e)
			{
				this.IsStretching = true;
				this.CalculateZoomStartPosition();
				this.CalculateZoomEndPosition();
				if (this.DragStarted != null)
				{
					this.DragStarted(this, null);
				}
			};
			DragStartedEventHandler value2 = delegate(object sender, DragStartedEventArgs e)
			{
				this.IsStretching = true;
				this.CalculateZoomStartPosition();
				this.CalculateZoomEndPosition();
				if (this.DragStarted != null)
				{
					this.DragStarted(this, null);
				}
			};
			DragCompletedEventHandler value3 = delegate(object sender1, DragCompletedEventArgs e1)
			{
				this.IsStretching = false;
				if (this.DragCompleted != null)
				{
					this.DragCompleted(this, null);
				}
			};
			if (this.Orientation == Orientation.Horizontal)
			{
				Func<Thumb, bool> predicate = (Thumb thumb) => thumb.Name == "ElementLeftGrip";
				IEnumerable<Thumb> source = ZoomBar.GetVisuals(this._elementHorizontalThumb).OfType<Thumb>();
				IEnumerable<Border> source2 = ZoomBar.GetVisuals(this._elementHorizontalThumb).OfType<Border>();
				if (source.Count<Thumb>() >= 2)
				{
					this._elementLeftGrip = source.Where(predicate).First<Thumb>();
					this._elementLeftGrip.Height = double.NaN;
					this._elementLeftGrip.DragDelta += new DragDeltaEventHandler(this.ElementLeftGrip_DragDelta);
					this._elementLeftGrip.DragStarted += value;
					this._elementLeftGrip.DragCompleted += value3;
					predicate = ((Thumb thumb) => thumb.Name == "ElementRightGrip");
					this._elementRightGrip = source.Where(predicate).First<Thumb>();
					this._elementRightGrip.Height = double.NaN;
					this._elementRightGrip.DragDelta += new DragDeltaEventHandler(this.ElementRightGrip_DragDelta);
					this._elementRightGrip.DragStarted += value2;
					this._elementRightGrip.DragCompleted += value3;
					if (!this.IsStretchable)
					{
						this._elementLeftGrip.Visibility = Visibility.Collapsed;
						this._elementRightGrip.Visibility = Visibility.Collapsed;
					}
					this._elementHorizontalThumb.LayoutUpdated -= new EventHandler(this.ElementLeftGrip_LayoutUpdated);
				}
				if (source2.Count<Border>() >= 1)
				{
					Func<Border, bool> predicate2 = (Border border) => border.Name == "ElementCenterBorder";
					this.ElementCenterBorder = source2.Where(predicate2).First<Border>();
				}
			}
			else
			{
				Func<Thumb, bool> predicate3 = (Thumb thumb) => thumb.Name == "ElementTopGrip";
				IEnumerable<Thumb> source3 = ZoomBar.GetVisuals(this._elementVerticalThumb).OfType<Thumb>();
				IEnumerable<Border> source4 = ZoomBar.GetVisuals(this._elementVerticalThumb).OfType<Border>();
				if (source3.Count<Thumb>() >= 2)
				{
					this._elementTopGrip = source3.Where(predicate3).First<Thumb>();
					this._elementTopGrip.Width = double.NaN;
					this._elementTopGrip.DragDelta += new DragDeltaEventHandler(this.ElementLeftGrip_DragDelta);
					this._elementTopGrip.DragStarted += value;
					this._elementTopGrip.DragCompleted += value3;
					predicate3 = ((Thumb thumb) => thumb.Name == "ElementBottomGrip");
					this._elementBottomGrip = source3.Where(predicate3).First<Thumb>();
					this._elementBottomGrip.Width = double.NaN;
					this._elementBottomGrip.DragDelta += new DragDeltaEventHandler(this.ElementRightGrip_DragDelta);
					this._elementBottomGrip.DragStarted += value;
					this._elementBottomGrip.DragCompleted += value3;
					if (!this.IsStretchable)
					{
						this._elementTopGrip.Visibility = Visibility.Collapsed;
						this._elementBottomGrip.Visibility = Visibility.Collapsed;
					}
					this._elementHorizontalThumb.LayoutUpdated -= new EventHandler(this.ElementLeftGrip_LayoutUpdated);
				}
				if (source4.Count<Border>() >= 1)
				{
					Func<Border, bool> predicate4 = (Border border) => border.Name == "ElementCenterBorder";
					this.ElementCenterBorder = source4.Where(predicate4).First<Border>();
				}
			}
			if (this.ElementCenterBorder != null)
			{
				this.ElementCenterBorder.Visibility = (this.IsStretchable ? Visibility.Visible : Visibility.Collapsed);
			}
		}

		private void CalculateAndUpdateValues4LeftGrip(double trackLength)
		{
			double num = trackLength - this._currentThumbSize;
			double num2;
			if (num <= 0.0)
			{
				if (this.Orientation == Orientation.Horizontal)
				{
					this._elementHorizontalLargeDecrease.Width = 0.0;
				}
				else
				{
					this._elementVerticalLargeDecrease.Height = 0.0;
				}
				num2 = (base.Maximum - base.Minimum) / 2.0;
			}
			else
			{
				double num3 = (this.Orientation == Orientation.Horizontal) ? this._elementHorizontalLargeDecrease.Width : this._elementVerticalLargeDecrease.Height;
				double num4 = trackLength - this._currentThumbSize / 2.0;
				double num5 = num3 + this._currentThumbSize / 2.0;
				double num6 = this._currentThumbSize / 2.0;
				double num7 = (num5 - num6) / (num4 - num6);
				num2 = (base.Maximum - base.Minimum) * num7;
			}
			if (base.Value != num2)
			{
				this._isNotificationEnabled = false;
				base.Value = num2;
				this._isNotificationEnabled = true;
			}
			this.Scale = this._currentThumbSize / trackLength;
			if (this.ScaleChanged != null)
			{
				this.ScaleChanged(this, new EventArgs());
			}
			this.RaiseScrollEvent(ScrollEventType.ThumbPosition);
		}

		private void CalculateAndUpdateValues4RightGrip(double trackLength)
		{
			double num = trackLength - this._currentThumbSize;
			if (num <= 0.0)
			{
				if (this.Orientation == Orientation.Horizontal)
				{
					this._elementHorizontalLargeDecrease.Width = 0.0;
				}
				else
				{
					this._elementVerticalLargeDecrease.Height = 0.0;
				}
				this._isNotificationEnabled = false;
				base.Value = 0.0;
				this.Scale = this._currentThumbSize / trackLength;
				if (this.ScaleChanged != null)
				{
					this.ScaleChanged(this, new EventArgs());
				}
				this._isNotificationEnabled = true;
				this.RaiseScrollEvent(ScrollEventType.ThumbPosition);
				return;
			}
			this._isNotificationEnabled = false;
			if (this.Orientation == Orientation.Horizontal)
			{
				double num2 = this._elementHorizontalLargeDecrease.ActualWidth / (trackLength - this._currentThumbSize) * (base.Maximum - base.Minimum) - base.Minimum;
				if (!double.IsNaN(num2) && base.Value != num2)
				{
					base.Value = num2;
				}
			}
			else
			{
				double num3 = this._elementVerticalLargeDecrease.Height / (trackLength - this._currentThumbSize) * (base.Maximum - base.Minimum) - base.Minimum;
				if (!double.IsNaN(num3) && base.Value != num3)
				{
					base.Value = num3;
				}
			}
			this._isNotificationEnabled = true;
			this.Scale = this._currentThumbSize / trackLength;
			if (this.ScaleChanged != null)
			{
				this.ScaleChanged(this, new EventArgs());
			}
			this.RaiseScrollEvent(ScrollEventType.ThumbPosition);
		}

		private void ElementRightGrip_DragDelta(object sender, DragDeltaEventArgs e)
		{
			double trackLength = this.GetTrackLength();
			if (this.Orientation == Orientation.Horizontal)
			{
				this._currentThumbSize += e.HorizontalChange;
				if (this._currentThumbSize > trackLength - this._elementHorizontalLargeDecrease.ActualWidth)
				{
					this._currentThumbSize = trackLength - this._elementHorizontalLargeDecrease.ActualWidth;
				}
			}
			else
			{
				this._currentThumbSize += e.VerticalChange;
				if (this._currentThumbSize > trackLength - this._elementVerticalLargeDecrease.ActualHeight)
				{
					this._currentThumbSize = trackLength - this._elementVerticalLargeDecrease.ActualHeight;
				}
			}
			if (this._currentThumbSize < this.MinThumbSize)
			{
				return;
			}
			this.UpdateThumbSize(trackLength);
			this.CalculateAndUpdateValues4RightGrip(trackLength);
			this.CalculateZoomEndPosition();
			if (this.Drag != null)
			{
				this.Drag(this, null);
			}
		}

		internal void UpdateScale(double scale)
		{
			double trackLength = this.GetTrackLength();
			this.Scale = scale;
			this._currentThumbSize = scale * trackLength;
			this.UpdateTrackLayout(trackLength);
			this.CalculateZoomEndPosition();
		}

		private void ElementLeftGrip_DragDelta(object sender, DragDeltaEventArgs e)
		{
			double currentThumbSize = this._currentThumbSize;
			double trackLength = this.GetTrackLength();
			double num = (this.Orientation == Orientation.Horizontal) ? e.HorizontalChange : e.VerticalChange;
			if (num < 0.0)
			{
				double num2 = this._currentThumbSize;
				num2 -= num;
				if (this.Orientation == Orientation.Horizontal)
				{
					double val = trackLength;
					num2 = Math.Max(this._elementHorizontalThumb.MinWidth, Math.Min(val, num2));
					double num3 = this._elementHorizontalLargeDecrease.Width - Math.Abs(currentThumbSize - num2);
					if (num3 < 0.0)
					{
						num3 = 0.0;
						this._currentThumbSize += this._elementHorizontalLargeDecrease.Width;
					}
					else
					{
						this._currentThumbSize = num2;
					}
					this._elementHorizontalLargeDecrease.Width = num3;
				}
				else
				{
					double val2 = trackLength;
					num2 = Math.Max(this._elementVerticalThumb.MinHeight, Math.Min(val2, num2));
					double num4 = this._elementVerticalLargeDecrease.Height - Math.Abs(currentThumbSize - num2);
					if (num4 < 0.0)
					{
						num4 = 0.0;
						this._currentThumbSize += this._elementVerticalLargeDecrease.Height;
					}
					else
					{
						this._currentThumbSize = num2;
					}
					this._elementVerticalLargeDecrease.Height = num4;
				}
			}
			else if (num > 0.0)
			{
				if (this.Orientation == Orientation.Horizontal)
				{
					if ((this._elementHorizontalLargeDecrease.Width != 0.0 || this._currentThumbSize != this._elementHorizontalThumb.MinWidth) && this._currentThumbSize - num > this._elementHorizontalThumb.MinWidth)
					{
						this._currentThumbSize -= num;
						double width = this._elementHorizontalLargeDecrease.Width + Math.Abs(currentThumbSize - this._currentThumbSize);
						this._elementHorizontalLargeDecrease.Width = width;
					}
				}
				else if ((this._elementVerticalLargeDecrease.Height != 0.0 || this._currentThumbSize != this._elementVerticalThumb.MinHeight) && this._currentThumbSize - num > this._elementVerticalThumb.MinHeight)
				{
					this._currentThumbSize -= num;
					double height = this._elementVerticalLargeDecrease.Height + Math.Abs(currentThumbSize - this._currentThumbSize);
					this._elementVerticalLargeDecrease.Height = height;
				}
			}
			if (this._currentThumbSize < this.MinThumbSize)
			{
				return;
			}
			this.UpdateThumbSize(trackLength);
			this.CalculateAndUpdateValues4LeftGrip(trackLength);
			this.CalculateZoomStartPosition();
			if (this.Drag != null)
			{
				this.Drag(this, null);
			}
		}

		private void UpdateTrackLayout(double trackLength)
		{
			double maximum = base.Maximum;
			double minimum = base.Minimum;
			double num = (base.Value - minimum) / (maximum - minimum);
			double num2 = this.UpdateThumbSize(trackLength);
			if (this.Orientation == Orientation.Horizontal && this._elementHorizontalLargeDecrease != null && this._elementHorizontalThumb != null)
			{
				this._elementHorizontalLargeDecrease.Width = Math.Max(0.0, num * (trackLength - num2));
			}
			else if (this.Orientation == Orientation.Vertical && this._elementVerticalLargeDecrease != null && this._elementVerticalThumb != null)
			{
				this._elementVerticalLargeDecrease.Height = Math.Max(0.0, num * (trackLength - num2));
			}
			if (this.ElementCenterBorder != null)
			{
				this.ElementCenterBorder.Visibility = (this.IsStretchable ? Visibility.Visible : Visibility.Collapsed);
			}
		}

		internal void UpdateTrackLayout()
		{
			this.UpdateTrackLayout(this.GetTrackLength());
		}

		internal void ResetThumSize()
		{
			this._currentThumbSize = double.NaN;
		}

		private double GetTrackLength()
		{
			double num;
			if (this.Orientation == Orientation.Horizontal)
			{
				num = base.ActualWidth;
				if (this._elementHorizontalSmallDecrease != null)
				{
					num -= this._elementHorizontalSmallDecrease.ActualWidth + this._elementHorizontalSmallDecrease.Margin.Left + this._elementHorizontalSmallDecrease.Margin.Right;
				}
				if (this._elementHorizontalSmallIncrease != null)
				{
					num -= this._elementHorizontalSmallIncrease.ActualWidth + this._elementHorizontalSmallIncrease.Margin.Left + this._elementHorizontalSmallIncrease.Margin.Right;
				}
				return num;
			}
			num = base.ActualHeight;
			if (this._elementVerticalSmallDecrease != null)
			{
				num -= this._elementVerticalSmallDecrease.ActualHeight + this._elementVerticalSmallDecrease.Margin.Top + this._elementVerticalSmallDecrease.Margin.Bottom;
			}
			if (this._elementVerticalSmallIncrease != null)
			{
				num -= this._elementVerticalSmallIncrease.ActualHeight + this._elementVerticalSmallIncrease.Margin.Top + this._elementVerticalSmallIncrease.Margin.Bottom;
			}
			return num;
		}

		internal double UpdateThumbSize(double trackLength)
		{
			double num = double.NaN;
			bool flag = trackLength <= 0.0;
			if (trackLength > 0.0)
			{
				if (this.Orientation == Orientation.Horizontal && this._elementHorizontalThumb != null)
				{
					if (base.Maximum - base.Minimum != 0.0)
					{
						double val = this.ConvertViewportSizeToDisplayUnits(trackLength);
						num = Math.Max(this._elementHorizontalThumb.MinWidth, val);
					}
					if (base.Maximum - base.Minimum == 0.0 || num > base.ActualWidth || trackLength <= this._elementHorizontalThumb.MinWidth)
					{
						flag = true;
					}
					else
					{
						this._elementHorizontalThumb.Visibility = Visibility.Visible;
						this._elementHorizontalThumb.Width = num;
					}
				}
				else if (this.Orientation == Orientation.Vertical && this._elementVerticalThumb != null)
				{
					if (base.Maximum - base.Minimum != 0.0)
					{
						double val = this.ConvertViewportSizeToDisplayUnits(trackLength);
						num = Math.Max(this._elementVerticalThumb.MinHeight, val);
					}
					if (base.Maximum - base.Minimum == 0.0 || num > base.ActualHeight || trackLength <= this._elementVerticalThumb.MinHeight)
					{
						flag = true;
					}
					else
					{
						this._elementVerticalThumb.Visibility = Visibility.Visible;
						this._elementVerticalThumb.Height = num;
					}
				}
			}
			if (flag && !this.IsStretchable)
			{
				if (this._elementHorizontalThumb != null)
				{
					this._elementHorizontalThumb.Visibility = Visibility.Collapsed;
				}
				if (this._elementVerticalThumb != null)
				{
					this._elementVerticalThumb.Visibility = Visibility.Collapsed;
				}
			}
			this._currentThumbSize = num;
			this.Scale = num / trackLength;
			return num;
		}

		internal void UpdateVisualState()
		{
		}

		private void RaiseScrollEvent(ScrollEventType scrollEventType)
		{
			ScrollEventArgs e = new ScrollEventArgs(scrollEventType, base.Value);
			if (this.Scroll != null && this.ScrollEventFireEnabled)
			{
				this.Scroll(this, e);
			}
		}
	}
}
