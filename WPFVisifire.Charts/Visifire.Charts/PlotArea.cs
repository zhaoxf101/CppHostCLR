using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class PlotArea : ObservableObject
	{
		public new static readonly DependencyProperty BackgroundProperty;

		public new static readonly DependencyProperty OpacityProperty;

		public new static readonly DependencyProperty BorderThicknessProperty;

		public static readonly DependencyProperty SideFaceBackgroundProperty;

		public static readonly DependencyProperty HrefTargetProperty;

		public static readonly DependencyProperty HrefProperty;

		public static readonly DependencyProperty BevelProperty;

		public static readonly DependencyProperty BorderColorProperty;

		public static readonly DependencyProperty LightingEnabledProperty;

		public static readonly DependencyProperty CornerRadiusProperty;

		public static readonly DependencyProperty ShadowEnabledProperty;

		internal ZoomingPanningEventHandler DragEventHandler = new ZoomingPanningEventHandler();

		private Brush _internalBackground;

		private Thickness? _borderThickness = null;

		private double _internalOpacity = double.NaN;

		private Canvas _bevelCanvas;

		private static bool _defaultStyleKeyApplied;

		public new event EventHandler<PlotAreaMouseButtonEventArgs> MouseLeftButtonDown
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

		public new event EventHandler<PlotAreaMouseButtonEventArgs> MouseLeftButtonUp
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

		public new event EventHandler<PlotAreaMouseButtonEventArgs> MouseRightButtonDown
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

		public new event EventHandler<PlotAreaMouseButtonEventArgs> MouseRightButtonUp
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

		public new event EventHandler<PlotAreaMouseEventArgs> MouseMove
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

		internal new event EventHandler EventChanged;

		private event EventHandler<PlotAreaMouseButtonEventArgs> _onMouseLeftButtonDown;

		private event EventHandler<PlotAreaMouseButtonEventArgs> _onMouseLeftButtonUp;

		private event EventHandler<PlotAreaMouseButtonEventArgs> _onMouseRightButtonDown;

		private event EventHandler<PlotAreaMouseButtonEventArgs> _onMouseRightButtonUp;

		private event EventHandler<PlotAreaMouseEventArgs> _onMouseMove;

		public Brush SideFaceBackground
		{
			get
			{
				return (Brush)base.GetValue(PlotArea.SideFaceBackgroundProperty);
			}
			set
			{
				base.SetValue(PlotArea.SideFaceBackgroundProperty, value);
			}
		}

		public HrefTargets HrefTarget
		{
			get
			{
				return (HrefTargets)base.GetValue(PlotArea.HrefTargetProperty);
			}
			set
			{
				base.SetValue(PlotArea.HrefTargetProperty, value);
			}
		}

		public string Href
		{
			get
			{
				return (string)base.GetValue(PlotArea.HrefProperty);
			}
			set
			{
				base.SetValue(PlotArea.HrefProperty, value);
			}
		}

		public new double Opacity
		{
			get
			{
				return (double)base.GetValue(PlotArea.OpacityProperty);
			}
			set
			{
				base.SetValue(PlotArea.OpacityProperty, value);
			}
		}

		public new Cursor Cursor
		{
			get
			{
				return base.Cursor;
			}
			set
			{
				if (base.Cursor != value)
				{
					base.Cursor = value;
					if (this.Visual != null)
					{
						this.Visual.Cursor = ((this.Cursor == null) ? Cursors.Arrow : this.Cursor);
						return;
					}
					base.FirePropertyChanged(VcProperties.Cursor);
				}
			}
		}

		public bool Bevel
		{
			get
			{
				return (bool)base.GetValue(PlotArea.BevelProperty);
			}
			set
			{
				base.SetValue(PlotArea.BevelProperty, value);
			}
		}

		public Brush BorderColor
		{
			get
			{
				return (Brush)base.GetValue(PlotArea.BorderColorProperty);
			}
			set
			{
				base.SetValue(PlotArea.BorderColorProperty, value);
			}
		}

		public override string ToolTipText
		{
			get
			{
				if (base.Chart != null && !string.IsNullOrEmpty(base.Chart.ToolTipText))
				{
					return null;
				}
				return (string)base.GetValue(VisifireElement.ToolTipTextProperty);
			}
			set
			{
				base.SetValue(VisifireElement.ToolTipTextProperty, value);
			}
		}

		public new Thickness BorderThickness
		{
			get
			{
				return (Thickness)base.GetValue(PlotArea.BorderThicknessProperty);
			}
			set
			{
				base.SetValue(PlotArea.BorderThicknessProperty, value);
			}
		}

		internal Thickness InternalBorderThickness
		{
			get
			{
				return (Thickness)((!this._borderThickness.HasValue) ? base.GetValue(PlotArea.BorderThicknessProperty) : this._borderThickness);
			}
			set
			{
				this._borderThickness = new Thickness?(value);
			}
		}

		public bool LightingEnabled
		{
			get
			{
				return (bool)base.GetValue(PlotArea.LightingEnabledProperty);
			}
			set
			{
				base.SetValue(PlotArea.LightingEnabledProperty, value);
			}
		}

		[TypeConverter(typeof(CornerRadiusConverter))]
		public CornerRadius CornerRadius
		{
			get
			{
				return (CornerRadius)base.GetValue(PlotArea.CornerRadiusProperty);
			}
			set
			{
				base.SetValue(PlotArea.CornerRadiusProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? ShadowEnabled
		{
			get
			{
				if (((bool?)base.GetValue(PlotArea.ShadowEnabledProperty)).HasValue)
				{
					return (bool?)base.GetValue(PlotArea.ShadowEnabledProperty);
				}
				if (base.Chart == null)
				{
					return new bool?(false);
				}
				Chart chart = base.Chart as Chart;
				if (!chart.View3D && chart.ChartArea != null && chart.ChartArea.AxisY2 == null && chart.PlotDetails != null && chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					return new bool?(true);
				}
				return new bool?(false);
			}
			set
			{
				base.SetValue(PlotArea.ShadowEnabledProperty, value);
			}
		}

		public new Brush Background
		{
			get
			{
				return (Brush)base.GetValue(PlotArea.BackgroundProperty);
			}
			set
			{
				base.SetValue(PlotArea.BackgroundProperty, value);
			}
		}

		internal Brush InternalBackground
		{
			get
			{
				return (Brush)((this._internalBackground == null) ? base.GetValue(PlotArea.BackgroundProperty) : this._internalBackground);
			}
			set
			{
				this._internalBackground = value;
			}
		}

		internal double InternalOpacity
		{
			get
			{
				return (double)(double.IsNaN(this._internalOpacity) ? base.GetValue(PlotArea.OpacityProperty) : this._internalOpacity);
			}
			set
			{
				this._internalOpacity = value;
			}
		}

		internal Faces SideFace
		{
			get;
			set;
		}

		internal Border LightingBorder
		{
			get;
			set;
		}

		internal Grid BevelGrid
		{
			get;
			set;
		}

		internal FrameworkElement ShadowElement
		{
			get;
			set;
		}

		internal FrameworkElement InnerShadowElement
		{
			get;
			set;
		}

		internal Canvas Visual
		{
			get;
			set;
		}

		internal Border BorderElement
		{
			get;
			set;
		}

		public PlotArea()
		{
			this.EventChanged += delegate(object A_1, EventArgs A_2)
			{
				base.FirePropertyChanged(VcProperties.MouseEvent);
			};
		}

		static PlotArea()
		{
			PlotArea.BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(PlotArea), new PropertyMetadata(new PropertyChangedCallback(PlotArea.OnBackgroundPropertyChanged)));
			PlotArea.OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(PlotArea), new PropertyMetadata(1.0, new PropertyChangedCallback(PlotArea.OnOpacityPropertyChanged)));
			PlotArea.BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(PlotArea), new PropertyMetadata(new PropertyChangedCallback(PlotArea.OnBorderThicknessPropertyChanged)));
			PlotArea.SideFaceBackgroundProperty = DependencyProperty.Register("SideFaceBackground", typeof(Brush), typeof(PlotArea), new PropertyMetadata(new PropertyChangedCallback(PlotArea.OnSideFaceBackgroundChanged)));
			PlotArea.HrefTargetProperty = DependencyProperty.Register("HrefTarget", typeof(HrefTargets), typeof(PlotArea), new PropertyMetadata(new PropertyChangedCallback(PlotArea.OnHrefTargetChanged)));
			PlotArea.HrefProperty = DependencyProperty.Register("Href", typeof(string), typeof(PlotArea), new PropertyMetadata(new PropertyChangedCallback(PlotArea.OnHrefChanged)));
			PlotArea.BevelProperty = DependencyProperty.Register("Bevel", typeof(bool), typeof(PlotArea), new PropertyMetadata(new PropertyChangedCallback(PlotArea.OnBevelPropertyChanged)));
			PlotArea.BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(Brush), typeof(PlotArea), new PropertyMetadata(new PropertyChangedCallback(PlotArea.OnBorderColorPropertyChanged)));
			PlotArea.LightingEnabledProperty = DependencyProperty.Register("LightingEnabled", typeof(bool), typeof(PlotArea), new PropertyMetadata(new PropertyChangedCallback(PlotArea.OnLightingEnabledPropertyChanged)));
			PlotArea.CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PlotArea), new PropertyMetadata(new PropertyChangedCallback(PlotArea.OnCornerRadiusPropertyChanged)));
			PlotArea.ShadowEnabledProperty = DependencyProperty.Register("ShadowEnabled", typeof(bool?), typeof(PlotArea), new PropertyMetadata(new PropertyChangedCallback(PlotArea.OnShadowEnabledPropertyChanged)));
			if (!PlotArea._defaultStyleKeyApplied)
			{
				FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PlotArea), new FrameworkPropertyMetadata(typeof(PlotArea)));
				PlotArea._defaultStyleKeyApplied = true;
			}
		}

		internal override void AttachEventChanged()
		{
		}

		internal override void Bind()
		{
		}

		private static void OnBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PlotArea plotArea = d as PlotArea;
			plotArea.InternalBackground = (Brush)e.NewValue;
			plotArea.FirePropertyChanged(VcProperties.Background);
		}

		private static void OnOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PlotArea plotArea = d as PlotArea;
			plotArea.InternalOpacity = (double)e.NewValue;
			plotArea.FirePropertyChanged(VcProperties.Opacity);
		}

		private static void OnBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PlotArea plotArea = d as PlotArea;
			plotArea.InternalBorderThickness = (Thickness)e.NewValue;
			plotArea.FirePropertyChanged(VcProperties.BorderThickness);
		}

		private static void OnSideFaceBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as PlotArea).FirePropertyChanged(VcProperties.SideFaceBackground);
		}

		private static void OnHrefTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as PlotArea).FirePropertyChanged(VcProperties.HrefTarget);
		}

		private static void OnHrefChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as PlotArea).FirePropertyChanged(VcProperties.Href);
		}

		private static void OnBevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as PlotArea).FirePropertyChanged(VcProperties.Bevel);
		}

		private static void OnBorderColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as PlotArea).FirePropertyChanged(VcProperties.BorderColor);
		}

		private static void OnLightingEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as PlotArea).FirePropertyChanged(VcProperties.LightingEnabled);
		}

		private static void OnCornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as PlotArea).FirePropertyChanged(VcProperties.CornerRadius);
		}

		private static void OnShadowEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as PlotArea).FirePropertyChanged(VcProperties.ShadowEnabled);
		}

		internal void CreateVisualObject()
		{
			if (this.Visual == null)
			{
				this.Visual = new Canvas();
				this.Visual.Children.Add(this.GetNewBorderElement());
			}
			this.ApplyProperties();
			this.ApplyLighting();
		}

		internal Border GetNewBorderElement()
		{
			this.BorderElement = new Border
			{
				Tag = new ElementData
				{
					Element = this
				}
			};
			this.LightingBorder = new Border
			{
				Tag = new ElementData
				{
					Element = this
				}
			};
			this.BevelGrid = new Grid();
			this.BevelGrid.Children.Add(this.LightingBorder);
			this.BorderElement.Child = this.BevelGrid;
			this.ApplyProperties();
			return this.BorderElement;
		}

		internal void UpdateProperties()
		{
			this.BorderElement.SetValue(Canvas.TopProperty, 0.0);
			this.BorderElement.SetValue(Canvas.LeftProperty, 0.0);
			this.ApplyProperties();
			this.ApplyLighting();
		}

		internal Point GetPlotAreaStartPosition()
		{
			Chart chart = base.Chart as Chart;
			Axis axisX = chart.ChartArea.AxisX;
			Axis axisY = chart.ChartArea.AxisY;
			double num;
			double num2;
			if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (axisY != null)
				{
					num = Axis.GetAxisLeft(axisY) + axisY.Width;
					num2 = Axis.GetAxisTop(axisY);
				}
				else
				{
					num = base.Chart.Padding.Left + base.Chart._leftOuterPanel.ActualWidth + chart._leftOffsetGrid.ActualWidth + base.Chart.BorderThickness.Left;
					chart._topOuterPanel.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
					chart._topOffsetGrid.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
					chart._topAxisGrid.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
					num2 = base.Chart.Padding.Top + chart._topOffsetGrid.DesiredSize.Height + base.Chart._topOuterPanel.DesiredSize.Height + base.Chart._topAxisGrid.DesiredSize.Height + base.Chart.BorderThickness.Top;
				}
			}
			else if (axisX != null)
			{
				num = Axis.GetAxisLeft(axisX) + axisX.Width;
				num2 = Axis.GetAxisTop(axisX);
			}
			else
			{
				num = base.Chart.Padding.Left + base.Chart._leftOuterPanel.ActualWidth + chart._leftOffsetGrid.ActualWidth + base.Chart.BorderThickness.Left;
				chart._topOuterPanel.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				chart._topOffsetGrid.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				chart._topAxisGrid.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				num2 = base.Chart.Padding.Top + chart._topOffsetGrid.DesiredSize.Height + base.Chart._topOuterPanel.DesiredSize.Height + base.Chart._topAxisGrid.DesiredSize.Height + base.Chart.BorderThickness.Top;
			}
			base.SetValue(Canvas.LeftProperty, num);
			base.SetValue(Canvas.TopProperty, num2);
			return new Point(num, num2);
		}

		internal void ApplyProperties()
		{
			this.Visual.Margin = new Thickness(0.0);
			this.Visual.Cursor = ((this.Cursor == null) ? null : this.Cursor);
			this.BorderElement.Opacity = this.InternalOpacity;
			this.BorderElement.Background = this.InternalBackground;
			if (this.BorderColor != null)
			{
				this.BorderElement.BorderBrush = this.BorderColor;
			}
			Thickness arg_76_0 = this.InternalBorderThickness;
			this.BorderElement.BorderThickness = this.InternalBorderThickness;
			CornerRadius arg_8E_0 = this.CornerRadius;
			this.BorderElement.CornerRadius = this.CornerRadius;
			this.LightingBorder.CornerRadius = this.CornerRadius;
		}

		internal void ApplyBevel(double plankDepth, double plankThickness)
		{
			if (this._bevelCanvas != null)
			{
				this.BevelGrid.Children.Remove(this._bevelCanvas);
				this._bevelCanvas = null;
			}
			if (this.Bevel)
			{
				VisifireControl arg_36_0 = base.Chart;
				this._bevelCanvas = ExtendedGraphics.Get2DRectangleBevel(this, this.BorderElement.Width - this.InternalBorderThickness.Left - this.InternalBorderThickness.Right, this.BorderElement.Height - this.InternalBorderThickness.Top - this.InternalBorderThickness.Bottom, 5.0, 5.0, Graphics.GetBevelTopBrush(this.BorderElement.Background), Graphics.GetBevelSideBrush(0.0, this.BorderElement.Background), Graphics.GetBevelSideBrush(180.0, this.BorderElement.Background), Graphics.GetBevelSideBrush(90.0, this.BorderElement.Background));
				this._bevelCanvas.SetValue(Canvas.LeftProperty, this.InternalBorderThickness.Left);
				this._bevelCanvas.SetValue(Canvas.TopProperty, this.InternalBorderThickness.Top);
				this._bevelCanvas.IsHitTestVisible = false;
				this.BevelGrid.Children.Add(this._bevelCanvas);
			}
		}

		internal void ApplyLighting()
		{
			if (this.LightingBorder != null)
			{
				if (this.LightingEnabled)
				{
					if (this.Bevel)
					{
						this.LightingBorder.Background = Graphics.GetFrontFaceBrush(this.InternalBackground);
						this.LightingBorder.Background.Opacity = 0.6;
						return;
					}
					this.LightingBorder.Background = Graphics.LightingBrush(this.LightingEnabled);
					return;
				}
				else
				{
					this.LightingBorder.Background = new SolidColorBrush(Colors.Transparent);
				}
			}
		}

		internal void ApplyShadow(Size plotAreaViewPortSize, double plankOffset, double plankDepth, double plankThickness)
		{
			base.Chart._plotAreaShadowCanvas.Children.Clear();
			base.Chart._drawingCanvas.Children.Remove(this.InnerShadowElement);
			if (this.ShadowElement != null)
			{
				this.ShadowElement = null;
			}
			Chart chart = base.Chart as Chart;
			if (!VisifireControl.IsMediaEffectsEnabled)
			{
				this.GetShadow4XBAP(chart, plotAreaViewPortSize, plankOffset, plankDepth, plankThickness);
				return;
			}
			this.GetShadow(chart, plotAreaViewPortSize, plankOffset, plankDepth, plankThickness);
		}

		private void GetShadow4XBAP(Chart chart, Size plotAreaViewPortSize, double plankOffset, double plankDepth, double plankThickness)
		{
			if (chart.PlotArea.ShadowEnabled.Value || chart.View3D)
			{
				if (chart.PlotDetails.ChartOrientation == ChartOrientationType.NoAxis)
				{
					if (chart.PlotArea.ShadowEnabled.Value)
					{
						this.ShadowElement = ExtendedGraphics.Get2DRectangleShadow(this, this.BorderElement.Width + 5.0, this.BorderElement.Height + 5.0, this.CornerRadius, this.CornerRadius, 6.0);
						Size clipSize = new Size(this.ShadowElement.Width, this.ShadowElement.Height);
						this.ShadowElement.Clip = ExtendedGraphics.GetShadowClip(clipSize);
					}
				}
				else if (chart.View3D)
				{
					if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
					{
						if (chart.PlotArea.ShadowEnabled.Value)
						{
							this.ShadowElement = ExtendedGraphics.Get2DRectangleShadow(this, plotAreaViewPortSize.Width + 5.0 - plankThickness - plankDepth - ChartArea.SCROLLVIEWER_OFFSET4HORIZONTAL_CHART, plotAreaViewPortSize.Height - plankDepth + 5.0, this.CornerRadius, this.CornerRadius, 6.0);
							Size clipSize = new Size(this.ShadowElement.Width, this.ShadowElement.Height + 5.0);
							this.ShadowElement.SetValue(Canvas.LeftProperty, plankOffset);
							this.ShadowElement.Clip = ExtendedGraphics.GetShadowClip(clipSize);
							this.InnerShadowElement = ExtendedGraphics.Get2DRectangleShadow(this, plotAreaViewPortSize.Width + 5.0 - plankThickness - plankDepth - ChartArea.SCROLLVIEWER_OFFSET4HORIZONTAL_CHART, this.BorderElement.Height + 5.0, this.CornerRadius, this.CornerRadius, 6.0);
							this.InnerShadowElement.Clip = ExtendedGraphics.GetShadowClip(new Size(this.InnerShadowElement.Width + 5.0, this.InnerShadowElement.Height));
							this.InnerShadowElement.SetValue(Canvas.LeftProperty, plankOffset);
							base.Chart._drawingCanvas.Children.Add(this.InnerShadowElement);
						}
					}
					else if (chart.PlotArea.ShadowEnabled.Value)
					{
						this.ShadowElement = ExtendedGraphics.Get2DRectangleShadow(this, plotAreaViewPortSize.Width + 5.0 - plankThickness - plankDepth, plotAreaViewPortSize.Height + 5.0 - plankOffset, this.CornerRadius, this.CornerRadius, 6.0);
						Size clipSize = new Size(this.ShadowElement.Width, this.ShadowElement.Height);
						this.ShadowElement.SetValue(Canvas.LeftProperty, plankOffset);
						this.ShadowElement.Clip = ExtendedGraphics.GetShadowClip(clipSize);
					}
					else
					{
						double num = 3.5;
						if (this.Background != null && !Graphics.AreBrushesEqual(this.Background, new SolidColorBrush(Colors.Transparent)))
						{
							this.ShadowElement = ExtendedGraphics.GetRectangle4PlotAreaEdge(this, plotAreaViewPortSize.Width + num - plankThickness - plankDepth, plotAreaViewPortSize.Height - plankOffset, this.CornerRadius, this.CornerRadius, 6.0, this.InternalBackground);
							Size clipSize = new Size(this.ShadowElement.Width, this.ShadowElement.Height + 5.0);
							this.ShadowElement.SetValue(Canvas.LeftProperty, plankOffset);
							this.ShadowElement.Clip = ExtendedGraphics.GetShadowClip(clipSize);
						}
					}
				}
				else if (chart.PlotArea.ShadowEnabled.Value)
				{
					this.ShadowElement = ExtendedGraphics.Get2DRectangleShadow(this, plotAreaViewPortSize.Width + 5.0, plotAreaViewPortSize.Height - plankOffset + 5.0, this.CornerRadius, this.CornerRadius, 6.0);
					Size clipSize = new Size(this.ShadowElement.Width, this.ShadowElement.Height);
					this.ShadowElement.Clip = ExtendedGraphics.GetShadowClip(clipSize);
				}
				if (this.ShadowElement != null)
				{
					this.ShadowElement.IsHitTestVisible = false;
					base.Chart._plotAreaShadowCanvas.Children.Add(this.ShadowElement);
				}
			}
		}

		private void GetShadow(Chart chart, Size plotAreaViewPortSize, double plankOffset, double plankDepth, double plankThickness)
		{
			if (chart.PlotArea.ShadowEnabled.Value || chart.View3D)
			{
				if (chart.PlotDetails.ChartOrientation == ChartOrientationType.NoAxis)
				{
					if (chart.PlotArea.ShadowEnabled.Value)
					{
						this.ShadowElement = new Border
						{
							CornerRadius = this.CornerRadius
						};
						this.ShadowElement.Width = this.BorderElement.Width;
						this.ShadowElement.Height = this.BorderElement.Height;
						this.ShadowElement.Effect = ExtendedGraphics.GetShadowEffect(315.0, 4.0, 0.95);
						if (this.Background != null)
						{
							(this.ShadowElement as Border).Background = this.Background;
						}
						else
						{
							(this.ShadowElement as Border).Background = new SolidColorBrush(Colors.LightGray);
							Size clipSize = new Size(this.ShadowElement.Width + 5.0, this.ShadowElement.Height + 5.0);
							this.ShadowElement.Clip = ExtendedGraphics.GetShadowClip(clipSize);
						}
					}
				}
				else if (chart.View3D)
				{
					if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
					{
						if (chart.PlotArea.ShadowEnabled.Value)
						{
							this.ShadowElement = new Border
							{
								CornerRadius = this.CornerRadius
							};
							this.ShadowElement.Width = plotAreaViewPortSize.Width - plankThickness - plankDepth - ChartArea.SCROLLVIEWER_OFFSET4HORIZONTAL_CHART;
							this.ShadowElement.Height = plotAreaViewPortSize.Height - plankDepth;
							this.ShadowElement.Effect = ExtendedGraphics.GetShadowEffect(315.0, 4.0, 0.95);
							if (this.Background != null)
							{
								(this.ShadowElement as Border).Background = this.Background;
							}
							else
							{
								(this.ShadowElement as Border).Background = new SolidColorBrush(Colors.LightGray);
								Size clipSize = new Size(this.ShadowElement.Width + 5.0, this.ShadowElement.Height + 5.0);
								this.ShadowElement.Clip = ExtendedGraphics.GetShadowClip(clipSize);
							}
							this.ShadowElement.SetValue(Canvas.LeftProperty, plankOffset);
							this.InnerShadowElement = new Border
							{
								CornerRadius = this.CornerRadius
							};
							this.InnerShadowElement.Width = plotAreaViewPortSize.Width - plankThickness - plankDepth - ChartArea.SCROLLVIEWER_OFFSET4HORIZONTAL_CHART;
							this.InnerShadowElement.Height = this.BorderElement.Height;
							this.InnerShadowElement.Effect = ExtendedGraphics.GetShadowEffect(315.0, 4.0, 0.95);
							this.InnerShadowElement.SetValue(Canvas.LeftProperty, plankOffset);
							if (this.Background != null)
							{
								this.InnerShadowElement.Clip = ExtendedGraphics.GetShadowClip(new Size(this.InnerShadowElement.Width, this.InnerShadowElement.Height + 6.0));
								(this.InnerShadowElement as Border).Background = this.Background;
							}
							else
							{
								(this.InnerShadowElement as Border).Background = new SolidColorBrush(Colors.LightGray);
								Size clipSize = new Size(this.InnerShadowElement.Width + 5.0, this.InnerShadowElement.Height + 6.0);
								this.InnerShadowElement.Clip = ExtendedGraphics.GetShadowClip(clipSize);
							}
							base.Chart._drawingCanvas.Children.Add(this.InnerShadowElement);
						}
					}
					else if (chart.PlotArea.ShadowEnabled.Value)
					{
						this.ShadowElement = new Border
						{
							CornerRadius = this.CornerRadius
						};
						this.ShadowElement.Width = plotAreaViewPortSize.Width - plankThickness - plankDepth;
						this.ShadowElement.Height = plotAreaViewPortSize.Height - plankOffset;
						this.ShadowElement.SetValue(Canvas.LeftProperty, plankOffset);
						this.ShadowElement.Effect = ExtendedGraphics.GetShadowEffect(315.0, 4.0, 0.95);
						if (this.Background != null)
						{
							(this.ShadowElement as Border).Background = this.Background;
						}
						else
						{
							(this.ShadowElement as Border).Background = new SolidColorBrush(Colors.LightGray);
							Size clipSize = new Size(this.ShadowElement.Width + 5.0, this.ShadowElement.Height + 5.0);
							this.ShadowElement.Clip = ExtendedGraphics.GetShadowClip(clipSize);
						}
					}
					else
					{
						double num = 3.5;
						if (this.Background != null && !Graphics.AreBrushesEqual(this.Background, new SolidColorBrush(Colors.Transparent)))
						{
							this.ShadowElement = ExtendedGraphics.GetRectangle4PlotAreaEdge(this, plotAreaViewPortSize.Width + num - plankThickness - plankDepth, plotAreaViewPortSize.Height - plankOffset, this.CornerRadius, this.CornerRadius, 6.0, this.InternalBackground);
							Size clipSize = new Size(this.ShadowElement.Width, this.ShadowElement.Height + 5.0);
							this.ShadowElement.SetValue(Canvas.LeftProperty, plankOffset);
							this.ShadowElement.Clip = ExtendedGraphics.GetShadowClip(clipSize);
						}
					}
				}
				else if (chart.PlotArea.ShadowEnabled.Value)
				{
					this.ShadowElement = new Border
					{
						CornerRadius = this.CornerRadius
					};
					this.ShadowElement.Width = plotAreaViewPortSize.Width;
					this.ShadowElement.Height = plotAreaViewPortSize.Height - plankOffset;
					this.ShadowElement.Effect = ExtendedGraphics.GetShadowEffect(315.0, 4.0, 0.95);
					if (this.Background != null)
					{
						(this.ShadowElement as Border).Background = this.Background;
					}
					else if ((this.Background == null || Graphics.AreBrushesEqual(this.Background, new SolidColorBrush(Colors.Transparent))) && base.Chart.Background != null && !Graphics.AreBrushesEqual(base.Chart.Background, new SolidColorBrush(Colors.Transparent)))
					{
						(this.ShadowElement as Border).Background = base.Chart.Background;
					}
					else
					{
						(this.ShadowElement as Border).Background = new SolidColorBrush(Color.FromArgb(255, 254, 254, 254));
					}
				}
				if (this.ShadowElement != null)
				{
					this.ShadowElement.IsHitTestVisible = false;
					base.Chart._plotAreaShadowCanvas.Children.Add(this.ShadowElement);
					this.ShadowElement.UpdateLayout();
				}
			}
		}

		private void SetPlotAreaMouseButtonEventArgs4AxesY(Chart chart, Axis axis, MouseButtonEventArgs e, PlotAreaMouseButtonEventArgs eventArgs)
		{
			AxisOrientation axisOrientation = axis.AxisOrientation;
			double num = (axisOrientation == AxisOrientation.Vertical) ? e.GetPosition(chart.ChartArea.PlottingCanvas).Y : e.GetPosition(chart.ChartArea.PlottingCanvas).X;
			double num2 = (axisOrientation == AxisOrientation.Vertical) ? chart.ChartArea.ChartVisualCanvas.Height : chart.ChartArea.ChartVisualCanvas.Width;
			double num3 = axis.PixelPositionToYValue(num2, (axisOrientation == AxisOrientation.Vertical) ? num : (num2 - num));
			if (axis.Logarithmic)
			{
				eventArgs.YValue = DataPointHelper.ConvertLogarithmicValue2ActualValue(chart, num3, axis.AxisType);
				return;
			}
			eventArgs.YValue = num3;
		}

		private void SetPlotAreaMouseEventArgs4AxesY(Chart chart, Axis axis, MouseEventArgs e, PlotAreaMouseEventArgs eventArgs)
		{
			AxisOrientation axisOrientation = axis.AxisOrientation;
			double num = (axisOrientation == AxisOrientation.Vertical) ? e.GetPosition(chart.ChartArea.PlottingCanvas).Y : e.GetPosition(chart.ChartArea.PlottingCanvas).X;
			double num2 = (axisOrientation == AxisOrientation.Vertical) ? chart.ChartArea.ChartVisualCanvas.Height : chart.ChartArea.ChartVisualCanvas.Width;
			double num3 = axis.PixelPositionToYValue(num2, (axisOrientation == AxisOrientation.Vertical) ? num : (num2 - num));
			if (axis.Logarithmic)
			{
				eventArgs.YValue = DataPointHelper.ConvertLogarithmicValue2ActualValue(chart, num3, axis.AxisType);
				return;
			}
			eventArgs.YValue = num3;
		}

		private void SetPlotAreaMouseButtonEventArgs4AxesX(Chart chart, Axis axis, MouseButtonEventArgs e, PlotAreaMouseButtonEventArgs eventArgs)
		{
			AxisOrientation axisOrientation = chart.ChartArea.AxisX.AxisOrientation;
			double pixelPosition = (axisOrientation == AxisOrientation.Horizontal) ? e.GetPosition(chart.ChartArea.PlottingCanvas).X : e.GetPosition(chart.ChartArea.PlottingCanvas).Y;
			eventArgs.XValue = axis.PixelPositionToXValue(pixelPosition);
		}

		private void SetPlotAreaMouseEventArgs4AxesX(Chart chart, Axis axis, MouseEventArgs e, PlotAreaMouseEventArgs eventArgs)
		{
			AxisOrientation axisOrientation = chart.ChartArea.AxisX.AxisOrientation;
			double num = (axisOrientation == AxisOrientation.Horizontal) ? e.GetPosition(chart.ChartArea.PlottingCanvas).X : e.GetPosition(chart.ChartArea.PlottingCanvas).Y;
			double num2 = (axisOrientation == AxisOrientation.Horizontal) ? chart.ChartArea.ChartVisualCanvas.Width : chart.ChartArea.ChartVisualCanvas.Height;
			double num3 = chart.ChartArea.AxisX.PixelPositionToXValue(num2, (axisOrientation == AxisOrientation.Horizontal) ? num : (num2 - num));
			if (chart.ChartArea.AxisX.IsDateTimeAxis)
			{
				try
				{
					eventArgs.XValue = DateTimeHelper.XValueToDateTime(chart.ChartArea.AxisX.MinDate, num3, chart.ChartArea.AxisX.InternalIntervalType);
					return;
				}
				catch
				{
					eventArgs.XValue = chart.ChartArea.AxisX.MinDate;
					return;
				}
			}
			if (axis.Logarithmic)
			{
				eventArgs.XValue = DataPointHelper.ConvertLogarithmicValue2ActualValue(chart, num3, axis.AxisType);
				return;
			}
			eventArgs.XValue = num3;
		}

		internal PlotAreaMouseButtonEventArgs CreatePlotAreaMouseButtonEventArgs(MouseButtonEventArgs e)
		{
			Chart chart = base.Chart as Chart;
			PlotAreaMouseButtonEventArgs plotAreaMouseButtonEventArgs = new PlotAreaMouseButtonEventArgs(e);
			if (chart.ChartArea.AxisX != null)
			{
				this.SetPlotAreaMouseButtonEventArgs4AxesX(chart, chart.ChartArea.AxisX, e, plotAreaMouseButtonEventArgs);
			}
			if (chart.ChartArea.AxisY != null)
			{
				this.SetPlotAreaMouseButtonEventArgs4AxesY(chart, chart.ChartArea.AxisY, e, plotAreaMouseButtonEventArgs);
			}
			else if (chart.ChartArea.AxisY2 != null)
			{
				this.SetPlotAreaMouseButtonEventArgs4AxesY(chart, chart.ChartArea.AxisY2, e, plotAreaMouseButtonEventArgs);
			}
			return plotAreaMouseButtonEventArgs;
		}

		internal PlotAreaMouseEventArgs CreatePlotAreaMouseEventArgs(MouseEventArgs e)
		{
			Chart chart = base.Chart as Chart;
			PlotAreaMouseEventArgs plotAreaMouseEventArgs = new PlotAreaMouseEventArgs(e);
			if (chart.ChartArea.AxisX != null)
			{
				this.SetPlotAreaMouseEventArgs4AxesX(chart, chart.ChartArea.AxisX, e, plotAreaMouseEventArgs);
			}
			if (chart.ChartArea.AxisY != null)
			{
				this.SetPlotAreaMouseEventArgs4AxesY(chart, chart.ChartArea.AxisY, e, plotAreaMouseEventArgs);
			}
			else if (chart.ChartArea.AxisY2 != null)
			{
				this.SetPlotAreaMouseEventArgs4AxesY(chart, chart.ChartArea.AxisY2, e, plotAreaMouseEventArgs);
			}
			return plotAreaMouseEventArgs;
		}

		internal override void FireMouseLeftButtonDownEvent(object sender, object e)
		{
			if (this._onMouseLeftButtonDown != null)
			{
				this._onMouseLeftButtonDown(sender, this.CreatePlotAreaMouseButtonEventArgs(e as MouseButtonEventArgs));
			}
		}

		internal override void FireMouseLeftButtonUpEvent(object sender, object e)
		{
			if (this._onMouseLeftButtonUp != null)
			{
				this._onMouseLeftButtonUp(this, this.CreatePlotAreaMouseButtonEventArgs(e as MouseButtonEventArgs));
			}
		}

		internal override void FireMouseMoveEvent(object sender, object e)
		{
			if (this._onMouseMove != null)
			{
				this._onMouseMove(this, this.CreatePlotAreaMouseEventArgs(e as MouseEventArgs));
			}
		}

		internal override void FireMouseRightButtonDownEvent(object sender, object e)
		{
			if (this._onMouseRightButtonDown != null)
			{
				this._onMouseRightButtonDown(sender, this.CreatePlotAreaMouseButtonEventArgs(e as MouseButtonEventArgs));
			}
		}

		internal override void FireMouseRightButtonUpEvent(object sender, object e)
		{
			if (this._onMouseRightButtonUp != null)
			{
				this._onMouseRightButtonUp(this, this.CreatePlotAreaMouseButtonEventArgs(e as MouseButtonEventArgs));
			}
		}

		internal override object GetMouseLeftButtonDownEventHandler()
		{
			return this._onMouseLeftButtonDown;
		}

		internal override object GetMouseLeftButtonUpEventHandler()
		{
			return this._onMouseLeftButtonUp;
		}

		internal override object GetMouseMoveEventHandler()
		{
			return this._onMouseMove;
		}

		internal override object GetMouseRightButtonDownEventHandler()
		{
			return this._onMouseRightButtonDown;
		}

		internal override object GetMouseRightButtonUpEventHandler()
		{
			return this._onMouseRightButtonUp;
		}

		internal override void ClearInstanceRefs()
		{
			this._bevelCanvas = null;
		}

		internal void ScrollToVerticalOffset(double offset)
		{
			Chart chart = base.Chart as Chart;
			if (chart != null)
			{
				FrameworkElement frameworkElement = chart._plotAreaScrollViewer.Children[0] as FrameworkElement;
				frameworkElement.SetValue(Canvas.LeftProperty, 0.0);
				frameworkElement.SetValue(Canvas.TopProperty, 0.0);
				if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
				{
					frameworkElement.SetValue(Canvas.TopProperty, -offset);
				}
			}
		}

		internal void ScrollToHorizontalOffset(double offset)
		{
			Chart chart = base.Chart as Chart;
			if (chart != null)
			{
				FrameworkElement frameworkElement = chart._plotAreaScrollViewer.Children[0] as FrameworkElement;
				frameworkElement.SetValue(Canvas.LeftProperty, 0.0);
				frameworkElement.SetValue(Canvas.TopProperty, 0.0);
				if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					frameworkElement.SetValue(Canvas.LeftProperty, -offset);
				}
			}
		}
	}
}
