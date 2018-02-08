using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using Visifire.Commons;
using Visifire.Commons.Controls;

namespace Visifire.Charts
{
	[TemplatePart(Name = "PlotAreaGrid", Type = typeof(Grid)), TemplatePart(Name = "LeftOuterPanel", Type = typeof(StackPanel)), TemplatePart(Name = "RightOuterLegendPanel", Type = typeof(StackPanel)), TemplatePart(Name = "RightOuterTitlePanel", Type = typeof(StackPanel)), TemplatePart(Name = "CenterOuterGrid", Type = typeof(Grid)), TemplatePart(Name = "TopAxisGrid", Type = typeof(Grid)), TemplatePart(Name = "TopAxisContainer", Type = typeof(StackPanel)), TemplatePart(Name = "TopAxisPanel", Type = typeof(StackPanel)), TemplatePart(Name = "TopInnerLegendPanel", Type = typeof(StackPanel)), TemplatePart(Name = "LeftAxisContainer", Type = typeof(StackPanel)), TemplatePart(Name = "LeftAxisPanel", Type = typeof(StackPanel)), TemplatePart(Name = "BottomOuterTitlePanel", Type = typeof(StackPanel)), TemplatePart(Name = "RightAxisGrid", Type = typeof(Grid)), TemplatePart(Name = "RightAxisContainer", Type = typeof(StackPanel)), TemplatePart(Name = "RightAxisScrollBar", Type = typeof(ScrollBar)), TemplatePart(Name = "TopOuterLegendPanel", Type = typeof(StackPanel)), TemplatePart(Name = "BottomAxisGrid", Type = typeof(Grid)), TemplatePart(Name = "BottomAxisContainer", Type = typeof(StackPanel)), TemplatePart(Name = "BottomAxisScrollBar", Type = typeof(ScrollBar)), TemplatePart(Name = "CenterGrid", Type = typeof(Grid)), TemplatePart(Name = "InnerGrid", Type = typeof(Grid)), TemplatePart(Name = "TopInnerPanel", Type = typeof(StackPanel)), TemplatePart(Name = "TopInnerTitlePanel", Type = typeof(StackPanel)), TemplatePart(Name = "RightInnerLegendPanel", Type = typeof(StackPanel)), TemplatePart(Name = "CenterDockInsidePlotAreaPanel", Type = typeof(StackPanel)), TemplatePart(Name = "BottomInnerLegendPanel", Type = typeof(StackPanel)), TemplatePart(Name = "BottomInnerTitlePanel", Type = typeof(StackPanel)), TemplatePart(Name = "LeftInnerPanel", Type = typeof(StackPanel)), TemplatePart(Name = "LeftInnerTitlePanel", Type = typeof(StackPanel)), TemplatePart(Name = "LeftInnerLegendPanel", Type = typeof(StackPanel)), TemplatePart(Name = "RightInnerPanel", Type = typeof(StackPanel)), TemplatePart(Name = "BottomInnerPanel", Type = typeof(StackPanel)), TemplatePart(Name = "RightInnerTitlePanel", Type = typeof(StackPanel)), TemplatePart(Name = "CenterInnerGrid", Type = typeof(Grid)), TemplatePart(Name = "LeftOuterLegendPanel", Type = typeof(StackPanel)), TemplatePart(Name = "PlotAreaScrollViewer", Type = typeof(ScrollViewer)), TemplatePart(Name = "PlotCanvas", Type = typeof(Canvas)), TemplatePart(Name = "PlotAreaShadowCanvas", Type = typeof(Canvas)), TemplatePart(Name = "DrawingCanvas", Type = typeof(Canvas)), TemplatePart(Name = "RightAxisPanel", Type = typeof(StackPanel)), TemplatePart(Name = "LeftOuterTitlePanel", Type = typeof(StackPanel)), TemplatePart(Name = "BottomAxisPanel", Type = typeof(StackPanel)), TemplatePart(Name = "RightOuterPanel", Type = typeof(StackPanel)), TemplatePart(Name = "BottomOuterPanel", Type = typeof(StackPanel)), TemplatePart(Name = "TopOuterTitlePanel", Type = typeof(StackPanel)), TemplatePart(Name = "LeftAxisScrollBar", Type = typeof(ScrollBar)), TemplatePart(Name = "ToolTipCanvas", Type = typeof(Canvas)), TemplatePart(Name = "TopAxisScrollBar", Type = typeof(ScrollBar)), TemplatePart(Name = "LeftAxisGrid", Type = typeof(Grid)), TemplatePart(Name = "CenterDockOutsidePlotAreaPanel", Type = typeof(StackPanel)), TemplatePart(Name = "RootElement", Type = typeof(Grid)), TemplatePart(Name = "BottomOuterLegendPanel", Type = typeof(StackPanel)), TemplatePart(Name = "ShadowGrid", Type = typeof(Grid)), TemplatePart(Name = "ChartBorder", Type = typeof(Border)), TemplatePart(Name = "ChartLightingBorder", Type = typeof(Rectangle)), TemplatePart(Name = "BevelCanvas", Type = typeof(Canvas)), TemplatePart(Name = "ChartAreaGrid", Type = typeof(Grid)), TemplatePart(Name = "TopOuterPanel", Type = typeof(StackPanel))]
	public class Chart : VisifireControl
	{
		internal delegate void RenderDelegate();

		internal const double SHADOW_DEPTH = 5.0;

		internal const double BEVEL_DEPTH = 5.0;

		internal Image CustomCursor;

		public static DependencyProperty ZoomingModeProperty;

		public static DependencyProperty PanningModeProperty;

		public static DependencyProperty SamplingThresholdProperty;

		public static DependencyProperty ThemeEnabledProperty;

		public static DependencyProperty ZoomingEnabledProperty;

		public static readonly DependencyProperty IndicatorEnabledProperty;

		public static readonly DependencyProperty IndicatorLineColorProperty;

		public static readonly DependencyProperty AxisIndicatorBackgroundProperty;

		public static readonly DependencyProperty AxisIndicatorFontSizeProperty;

		public static readonly DependencyProperty AxisIndicatorFontColorProperty;

		public static readonly DependencyProperty SmartLabelEnabledProperty;

		public static readonly DependencyProperty DataPointWidthProperty;

		public static readonly DependencyProperty MaxDataPointWidthProperty;

		public static readonly DependencyProperty UniqueColorsProperty;

		public static readonly DependencyProperty ScrollingEnabledProperty;

		public static readonly DependencyProperty View3DProperty;

		public static readonly DependencyProperty HrefTargetProperty;

		public static readonly DependencyProperty HrefProperty;

		public static readonly DependencyProperty ThemeProperty;

		public static readonly DependencyProperty AnimationEnabledProperty;

		public static readonly DependencyProperty AnimatedUpdateProperty;

		private static readonly DependencyProperty InternalBorderThicknessProperty;

		private static readonly DependencyProperty InternalBackgroundProperty;

		public static readonly DependencyProperty BevelProperty;

		public static readonly DependencyProperty ColorSetProperty;

		public static readonly DependencyProperty ColorSetsProperty;

		public static readonly DependencyProperty LightingEnabledProperty;

		public static readonly DependencyProperty CornerRadiusProperty;

		public static readonly DependencyProperty ShadowEnabledProperty;

		public static readonly DependencyProperty PlotAreaProperty;

		[Obsolete]
		public static readonly DependencyProperty MinimumGapProperty;

		internal bool _internalPartialUpdateEnabled;

		internal bool IS_FULL_RENDER_PENDING;

		private EventHandler _rendered;

		private bool _isShadowApplied;

		private Thickness _chartAreaMargin;

		internal double _partialRenderBlockedCount;

		internal Dictionary<IDataPoint, VcProperties> _datapoint2UpdatePartially;

		internal bool _forcedRedraw;

		internal bool _clearAndResetZoomState;

		internal int _renderLapsedCounter;

		internal bool _internalAnimationEnabled;

		internal bool _isThemeChanged;

		internal bool RENDER_LOCK;

		internal bool FULL_CHART_DRAW_ENDED;

		internal bool PARTIAL_DP_RENDER_LOCK;

		private Storyboard _st;

		private bool _isFirstTime = false;

		private int _licenseInfoCounter = 3;

		private int _bannerDisplayCounter = 1;

		private DispatcherTimer _animationTimer = new DispatcherTimer();

		private DispatcherTimer _bannerDisplayTimer = new DispatcherTimer();

		private static bool _defaultStyleKeyApplied;

		private Visibility _currentVisibility;

		public event EventHandler Rendered
		{
			add
			{
				this._rendered = (EventHandler)Delegate.Combine(this._rendered, value);
			}
			remove
			{
				this._rendered = (EventHandler)Delegate.Remove(this._rendered, value);
			}
		}

		public ZoomingMode ZoomingMode
		{
			get
			{
				return (ZoomingMode)base.GetValue(Chart.ZoomingModeProperty);
			}
			set
			{
				base.SetValue(Chart.ZoomingModeProperty, value);
			}
		}

		public PanningMode PanningMode
		{
			get
			{
				return (PanningMode)base.GetValue(Chart.PanningModeProperty);
			}
			set
			{
				base.SetValue(Chart.PanningModeProperty, value);
			}
		}

		public bool ThemeEnabled
		{
			get
			{
				return (bool)base.GetValue(Chart.ThemeEnabledProperty);
			}
			set
			{
				base.SetValue(Chart.ThemeEnabledProperty, value);
			}
		}

		internal bool IsSamplingEnabled
		{
			get;
			set;
		}

		public int SamplingThreshold
		{
			get
			{
				return (int)base.GetValue(Chart.SamplingThresholdProperty);
			}
			set
			{
				base.SetValue(Chart.SamplingThresholdProperty, value);
			}
		}

		public bool ZoomingEnabled
		{
			get
			{
				return (bool)base.GetValue(Chart.ZoomingEnabledProperty);
			}
			set
			{
				base.SetValue(Chart.ZoomingEnabledProperty, value);
			}
		}

		public bool UniqueColors
		{
			get
			{
				return (bool)base.GetValue(Chart.UniqueColorsProperty);
			}
			set
			{
				base.SetValue(Chart.UniqueColorsProperty, value);
			}
		}

		public bool SmartLabelEnabled
		{
			get
			{
				return (bool)base.GetValue(Chart.SmartLabelEnabledProperty);
			}
			set
			{
				base.SetValue(Chart.SmartLabelEnabledProperty, value);
			}
		}

		public double DataPointWidth
		{
			get
			{
				return (double)base.GetValue(Chart.DataPointWidthProperty);
			}
			set
			{
				base.SetValue(Chart.DataPointWidthProperty, value);
			}
		}

		public double MaxDataPointWidth
		{
			get
			{
				return (double)base.GetValue(Chart.MaxDataPointWidthProperty);
			}
			set
			{
				base.SetValue(Chart.MaxDataPointWidthProperty, value);
			}
		}

		[Obsolete]
		public double? MinimumGap
		{
			get
			{
				return (double?)base.GetValue(Chart.MinimumGapProperty);
			}
			set
			{
				base.SetValue(Chart.MinimumGapProperty, value);
			}
		}

		public bool? ScrollingEnabled
		{
			get
			{
				if (!((bool?)base.GetValue(Chart.ScrollingEnabledProperty)).HasValue || this.ZoomingEnabled)
				{
					return new bool?(true);
				}
				return (bool?)base.GetValue(Chart.ScrollingEnabledProperty);
			}
			set
			{
				base.SetValue(Chart.ScrollingEnabledProperty, value);
			}
		}

		public bool View3D
		{
			get
			{
				return (bool)base.GetValue(Chart.View3DProperty);
			}
			set
			{
				base.SetValue(Chart.View3DProperty, value);
			}
		}

		public HrefTargets HrefTarget
		{
			get
			{
				return (HrefTargets)base.GetValue(Chart.HrefTargetProperty);
			}
			set
			{
				base.SetValue(Chart.HrefTargetProperty, value);
			}
		}

		public string Href
		{
			get
			{
				return (string)base.GetValue(Chart.HrefProperty);
			}
			set
			{
				base.SetValue(Chart.HrefProperty, value);
			}
		}

		public string Theme
		{
			get
			{
				return (string)base.GetValue(Chart.ThemeProperty);
			}
			set
			{
				base.SetValue(Chart.ThemeProperty, value);
			}
		}

		internal bool InternalAnimationEnabled
		{
			get
			{
				return !base.IsInDesignMode && this.AnimationEnabled.Value;
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? AnimationEnabled
		{
			get
			{
				if (!((bool?)base.GetValue(Chart.AnimationEnabledProperty)).HasValue)
				{
					if (this.PlotDetails == null || this.PlotDetails.ListOfAllDataPoints == null)
					{
						return new bool?(true);
					}
					if (this.PlotDetails.ListOfAllDataPoints.Count > 1000)
					{
						return new bool?(false);
					}
					return new bool?(true);
				}
				else
				{
					if (base.IsInDesignMode)
					{
						return (bool?)base.GetValue(Chart.AnimationEnabledProperty);
					}
					if (string.IsNullOrEmpty(Chart.AnimationEnabledProperty.ToString()))
					{
						return new bool?(false);
					}
					return (bool?)base.GetValue(Chart.AnimationEnabledProperty);
				}
			}
			set
			{
				base.SetValue(Chart.AnimationEnabledProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? AnimatedUpdate
		{
			get
			{
				if (((bool?)base.GetValue(Chart.AnimatedUpdateProperty)).HasValue)
				{
					return (bool?)base.GetValue(Chart.AnimatedUpdateProperty);
				}
				if (this.Series.Count == 1 && (this.Series[0].RenderAs == RenderAs.Pie || this.Series[0].RenderAs == RenderAs.Doughnut))
				{
					return new bool?(true);
				}
				return new bool?(false);
			}
			set
			{
				base.SetValue(Chart.AnimatedUpdateProperty, value);
			}
		}

		public bool IndicatorEnabled
		{
			get
			{
				return (bool)base.GetValue(Chart.IndicatorEnabledProperty);
			}
			set
			{
				base.SetValue(Chart.IndicatorEnabledProperty, value);
			}
		}

		public Brush IndicatorLineColor
		{
			get
			{
				return (Brush)base.GetValue(Chart.IndicatorLineColorProperty);
			}
			set
			{
				base.SetValue(Chart.IndicatorLineColorProperty, value);
			}
		}

		public double AxisIndicatorFontSize
		{
			get
			{
				return (double)base.GetValue(Chart.AxisIndicatorFontSizeProperty);
			}
			set
			{
				base.SetValue(Chart.AxisIndicatorFontSizeProperty, value);
			}
		}

		public Brush AxisIndicatorBackground
		{
			get
			{
				return (Brush)base.GetValue(Chart.AxisIndicatorBackgroundProperty);
			}
			set
			{
				base.SetValue(Chart.AxisIndicatorBackgroundProperty, value);
			}
		}

		public Brush AxisIndicatorFontColor
		{
			get
			{
				return (Brush)base.GetValue(Chart.AxisIndicatorFontColorProperty);
			}
			set
			{
				base.SetValue(Chart.AxisIndicatorFontColorProperty, value);
			}
		}

		public new Thickness BorderThickness
		{
			get
			{
				return (Thickness)base.GetValue(Control.BorderThicknessProperty);
			}
			set
			{
				base.SetValue(Control.BorderThicknessProperty, value);
				base.SetValue(Chart.InternalBorderThicknessProperty, value);
			}
		}

		public new Brush Background
		{
			get
			{
				return (Brush)base.GetValue(Control.BackgroundProperty);
			}
			set
			{
				base.SetValue(Control.BackgroundProperty, value);
				base.SetValue(Chart.InternalBackgroundProperty, value);
			}
		}

		public bool Bevel
		{
			get
			{
				return (bool)base.GetValue(Chart.BevelProperty);
			}
			set
			{
				base.SetValue(Chart.BevelProperty, value);
			}
		}

		public string ColorSet
		{
			get
			{
				return (string)base.GetValue(Chart.ColorSetProperty);
			}
			set
			{
				base.SetValue(Chart.ColorSetProperty, value);
			}
		}

		public ColorSets ColorSets
		{
			get
			{
				return (ColorSets)base.GetValue(Chart.ColorSetsProperty);
			}
			set
			{
				base.SetValue(Chart.ColorSetsProperty, value);
			}
		}

		public bool LightingEnabled
		{
			get
			{
				return (bool)base.GetValue(Chart.LightingEnabledProperty);
			}
			set
			{
				base.SetValue(Chart.LightingEnabledProperty, value);
			}
		}

		[TypeConverter(typeof(CornerRadiusConverter))]
		public CornerRadius CornerRadius
		{
			get
			{
				return (CornerRadius)base.GetValue(Chart.CornerRadiusProperty);
			}
			set
			{
				base.SetValue(Chart.CornerRadiusProperty, value);
			}
		}

		public bool ShadowEnabled
		{
			get
			{
				return (bool)base.GetValue(Chart.ShadowEnabledProperty);
			}
			set
			{
				base.SetValue(Chart.ShadowEnabledProperty, value);
			}
		}

		public PlotArea PlotArea
		{
			get
			{
				return (PlotArea)base.GetValue(Chart.PlotAreaProperty);
			}
			set
			{
				base.SetValue(Chart.PlotAreaProperty, value);
				this.PlotArea.Chart = this;
				this.PlotArea.PropertyChanged -= new PropertyChangedEventHandler(this.Element_PropertyChanged);
				this.PlotArea.PropertyChanged += new PropertyChangedEventHandler(this.Element_PropertyChanged);
			}
		}

		public AxisCollection AxesX
		{
			get;
			set;
		}

		public AxisCollection AxesY
		{
			get;
			set;
		}

		public TitleCollection Titles
		{
			get;
			set;
		}

		public LegendCollection Legends
		{
			get;
			set;
		}

		public ToolTipCollection ToolTips
		{
			get;
			set;
		}

		public TrendLineCollection TrendLines
		{
			get;
			set;
		}

		public DataSeriesCollection Series
		{
			get;
			set;
		}

		private ColorSets EmbeddedColorSets
		{
			get;
			set;
		}

		private Grid ChartShadowLayer
		{
			get;
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		private new Brush Foreground
		{
			get;
			set;
		}

		internal bool IsScrollingActivated
		{
			get;
			set;
		}

		internal bool ScrollingActivatedForcefully
		{
			get;
			set;
		}

		internal List<DataSeries> InternalSeries
		{
			get;
			set;
		}

		internal Canvas BevelVisual
		{
			get;
			set;
		}

		internal PlotDetails PlotDetails
		{
			get;
			set;
		}

		internal ChartArea ChartArea
		{
			get;
			set;
		}

		internal ResourceDictionary StyleDictionary
		{
			get;
			set;
		}

		internal IList<Axis> InternalAxesX
		{
			get;
			set;
		}

		internal IList<Axis> InternalAxesY
		{
			get;
			set;
		}

		public void SetMarginOfElements()
		{
			if (this.ShadowEnabled && !VisifireControl.IsMediaEffectsEnabled)
			{
				this._chartBorder.Margin = new Thickness(0.0, 0.0, 5.0, 5.0);
				this._bevelCanvas.Margin = new Thickness(0.0, 0.0, 5.0, 5.0);
			}
			this._chartAreaMargin = new Thickness(this._chartAreaGrid.Margin.Left, this._chartAreaGrid.Margin.Top, this._chartAreaGrid.Margin.Right, this._chartAreaGrid.Margin.Bottom);
			if (this.Bevel)
			{
				this._chartAreaGrid.Margin = new Thickness(this._chartAreaMargin.Left + 5.0, this._chartAreaMargin.Top + 5.0, this._chartAreaMargin.Right + 5.0, this._chartAreaMargin.Bottom + 5.0);
			}
		}

		public override void Print()
		{
			base.Print();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.LoadControlsFromTemplate();
			if (this._rootElement == null)
			{
				throw new Exception("Chart template element is missing. Default template is either not loaded or does not exist.");
			}
			base.LoadToolBar();
			base.LoadZoomIcons();
			if (this.StyleDictionary == null)
			{
				this.LoadTheme("Theme1", false);
			}
			this.LoadOrUpdateColorSets();
			this.LoadDefaultToolTip();
			this.SetMarginOfElements();
			this.ApplyLighting();
			this.AttachToolTipAndEvents();
			this.BindProperties();
			this._internalAnimationEnabled = this.InternalAnimationEnabled;
			if (this._internalAnimationEnabled)
			{
				this._rootElement.IsHitTestVisible = false;
			}
			this._isTemplateApplied = true;
			this._zoomRectangle.Visibility = Visibility.Collapsed;
			NameScope.SetNameScope(this._rootElement, new NameScope());
			this.AddChartElementsToRootElement();
			this.AddCustomCursor();
		}

		private void AddCustomCursor()
		{
			this.CustomCursor = new Image
			{
				Height = 20.0,
				Width = 20.0,
				IsHitTestVisible = false,
				Visibility = Visibility.Collapsed
			};
			Graphics.SetImageSource(this.CustomCursor, "Visifire.Charts.cursor_drag_hand.png");
			this._toolTipCanvas.Children.Add(this.CustomCursor);
		}

		internal void SetCustomCursorPosition(MouseEventArgs e)
		{
			if (this.CustomCursor.Visibility == Visibility.Visible)
			{
				double num = e.GetPosition(this._toolTipCanvas).X;
				double num2 = e.GetPosition(this._toolTipCanvas).Y;
				num -= this.CustomCursor.Width / 2.0;
				num2 -= this.CustomCursor.Height / 2.0;
				this.CustomCursor.SetValue(Canvas.LeftProperty, num);
				this.CustomCursor.SetValue(Canvas.TopProperty, num2);
			}
		}

		private void AddChartElementsToRootElement()
		{
			this.AddAxesXToChartRootElement();
			this.AddAxesYToChartRootElement();
			foreach (DataSeries current in this.Series)
			{
				this.AddDataSeriesToRootElement(current);
			}
			foreach (Title current2 in this.Titles)
			{
				this.AddTitleToRootElement(current2);
			}
			foreach (TrendLine current3 in this.TrendLines)
			{
				this.AddTrendLineToRootElement(current3);
			}
			this.AddLegendsToChartRootElement();
			this.AddPlotAreaToChartRootElement();
		}

		private void AddDataSeriesToRootElement(DataSeries ds)
		{
			if (this._rootElement == null)
			{
				return;
			}
			if (base.IsInDesignMode)
			{
				ObservableObject.RemoveElementFromElementTree(ds);
			}
			if (ds.Parent == null)
			{
				this._rootElement.Children.Insert(0, ds);
			}
			ds.IsTabStop = false;
		}

		private void AddTitleToRootElement(Title title)
		{
			if (this._rootElement == null)
			{
				return;
			}
			title.IsNotificationEnable = false;
			if (base.IsInDesignMode)
			{
				ObservableObject.RemoveElementFromElementTree(title);
			}
			if (title.Parent == null)
			{
				this._rootElement.Children.Add(title);
			}
			title.IsNotificationEnable = true;
			title.IsTabStop = false;
		}

		private void AddTrendLineToRootElement(TrendLine trendLine)
		{
			if (this._rootElement == null)
			{
				return;
			}
			trendLine.IsNotificationEnable = false;
			if (base.IsInDesignMode)
			{
				ObservableObject.RemoveElementFromElementTree(trendLine);
			}
			if (trendLine.Parent == null)
			{
				this._rootElement.Children.Add(trendLine);
			}
			trendLine.IsNotificationEnable = true;
			trendLine.IsTabStop = false;
		}

		internal void AddAxesYToChartRootElement()
		{
			foreach (Axis current in this.AxesY)
			{
				this.AddAxisToChartRootElement(current);
			}
		}

		internal void AddAxesXToChartRootElement()
		{
			foreach (Axis current in this.AxesX)
			{
				this.AddAxisToChartRootElement(current);
			}
		}

		internal void AddAxisToChartRootElement(Axis axis)
		{
			if (this._rootElement == null)
			{
				return;
			}
			axis.IsNotificationEnable = false;
			if (base.IsInDesignMode)
			{
				ObservableObject.RemoveElementFromElementTree(axis);
			}
			if (axis.Parent == null)
			{
				this._rootElement.Children.Add(axis);
			}
			axis.IsNotificationEnable = true;
			axis.IsTabStop = false;
		}

		internal void AddLegendsToChartRootElement()
		{
			if (this._rootElement != null)
			{
				foreach (Legend current in this.Legends)
				{
					this.AddLegendToRootElement(current);
				}
			}
		}

		private void AddLegendToRootElement(Legend legend)
		{
			if (this._rootElement == null)
			{
				return;
			}
			legend.IsNotificationEnable = false;
			if (base.IsInDesignMode)
			{
				ObservableObject.RemoveElementFromElementTree(legend);
			}
			if (legend.Parent == null)
			{
				this._rootElement.Children.Add(legend);
			}
			legend.IsNotificationEnable = true;
			legend.IsTabStop = false;
		}

		internal void AddPlotAreaToChartRootElement()
		{
			if (this.PlotArea != null && this._rootElement != null)
			{
				this.PlotArea.IsNotificationEnable = false;
				if (base.IsInDesignMode)
				{
					ObservableObject.RemoveElementFromElementTree(this.PlotArea);
				}
				if (this.PlotArea.Parent == null)
				{
					this._rootElement.Children.Add(this.PlotArea);
				}
				this.PlotArea.IsNotificationEnable = true;
				this.PlotArea.IsTabStop = false;
			}
		}

		public void HideIndicator()
		{
			if (this.ChartArea != null)
			{
				base.Dispatcher.BeginInvoke(new Action(this.ChartArea.HideIndicator), new object[0]);
			}
		}

		public void ShowIndicator(object xValue, double yValue)
		{
			if (this.ChartArea != null && this.ChartArea.AxisX != null && this.IndicatorEnabled)
			{
				Axis axisX = this.ChartArea.AxisX;
				Axis yAxis = this.ChartArea.AxisY;
				if (this.ChartArea.AxisY != null)
				{
					yAxis = this.ChartArea.AxisY;
				}
				else if (this.ChartArea.AxisY2 != null)
				{
					yAxis = this.ChartArea.AxisY2;
				}
				double num;
				double num2;
				RenderHelper.UserValueToInternalValues(axisX, yAxis, xValue, yValue, out num, out num2);
				if (!double.IsNaN(yValue) && xValue != null)
				{
					base.Dispatcher.BeginInvoke(new Action<Chart, double, double>(this.ChartArea.PositionIndicator), new object[]
					{
						this,
						num,
						yValue
					});
				}
			}
		}

		public void Export()
		{
			base.Save(null, ExportType.Jpg, true);
		}

		public void Export(string path, ExportType exportType)
		{
			base.Save(path, exportType, false);
		}

		private static void OnZoomingModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.InvokeRender();
		}

		private static void OnPanningModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.InvokeRender();
		}

		private static void OnZoomingEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			if (chart._zoomIconContainer != null)
			{
				if ((bool)e.NewValue)
				{
					chart._zoomIconContainer.Visibility = Visibility.Visible;
				}
				else
				{
					chart._zoomIconContainer.Visibility = Visibility.Collapsed;
				}
			}
			chart._clearAndResetZoomState = true;
			chart.InvokeRender();
		}

		private static void OnThemeEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.InvokeRender();
		}

		private static void OnSamplingThresholdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((int)e.NewValue < 0)
			{
				throw new ArgumentOutOfRangeException("SamplingThreshold should be always greater than or equals to 0.");
			}
			Chart chart = d as Chart;
			if (e.NewValue == null)
			{
				chart.IsSamplingEnabled = false;
			}
			else if (e.NewValue != null && Convert.ToDouble(e.NewValue) == 0.0)
			{
				chart.IsSamplingEnabled = false;
			}
			else
			{
				chart.IsSamplingEnabled = true;
			}
			chart.InvokeRender();
		}

		private void LoadControlsFromTemplate()
		{
			this._rootElement = (base.GetTemplateChild("RootElement") as Grid);
			this._shadowGrid = (base.GetTemplateChild("ShadowGrid") as Grid);
			this._chartBorder = (base.GetTemplateChild("ChartBorder") as Border);
			this._chartLightingBorder = (base.GetTemplateChild("ChartLightingBorder") as Rectangle);
			this._bevelCanvas = (base.GetTemplateChild("BevelCanvas") as Canvas);
			this._chartAreaGrid = (base.GetTemplateChild("ChartAreaGrid") as Grid);
			this._topOuterPanel = (base.GetTemplateChild("TopOuterPanel") as StackPanel);
			this._topOuterTitlePanel = (base.GetTemplateChild("TopOuterTitlePanel") as StackPanel);
			this._topOuterLegendPanel = (base.GetTemplateChild("TopOuterLegendPanel") as StackPanel);
			this._bottomOuterPanel = (base.GetTemplateChild("BottomOuterPanel") as StackPanel);
			this._bottomOuterLegendPanel = (base.GetTemplateChild("BottomOuterLegendPanel") as StackPanel);
			this._bottomOuterTitlePanel = (base.GetTemplateChild("BottomOuterTitlePanel") as StackPanel);
			this._leftOuterPanel = (base.GetTemplateChild("LeftOuterPanel") as StackPanel);
			this._leftOuterTitlePanel = (base.GetTemplateChild("LeftOuterTitlePanel") as StackPanel);
			this._leftOuterLegendPanel = (base.GetTemplateChild("LeftOuterLegendPanel") as StackPanel);
			this._rightOuterPanel = (base.GetTemplateChild("RightOuterPanel") as StackPanel);
			this._rightOuterLegendPanel = (base.GetTemplateChild("RightOuterLegendPanel") as StackPanel);
			this._rightOuterTitlePanel = (base.GetTemplateChild("RightOuterTitlePanel") as StackPanel);
			this._centerGrid = (base.GetTemplateChild("CenterGrid") as Grid);
			this._centerOuterGrid = (base.GetTemplateChild("CenterOuterGrid") as Grid);
			this._topOffsetGrid = (base.GetTemplateChild("TopOffsetGrid") as Grid);
			this._bottomOffsetGrid = (base.GetTemplateChild("BottomOffsetGrid") as Grid);
			this._leftOffsetGrid = (base.GetTemplateChild("LeftOffsetGrid") as Grid);
			this._rightOffsetGrid = (base.GetTemplateChild("RightOffsetGrid") as Grid);
			this._topAxisGrid = (base.GetTemplateChild("TopAxisGrid") as Grid);
			this._topAxisContainer = (base.GetTemplateChild("TopAxisContainer") as StackPanel);
			this._topAxisPanel = (base.GetTemplateChild("TopAxisPanel") as StackPanel);
			this._topAxisScrollBar = (base.GetTemplateChild("TopAxisScrollBar") as ZoomBar);
			this._leftAxisGrid = (base.GetTemplateChild("LeftAxisGrid") as Grid);
			this._leftAxisContainer = (base.GetTemplateChild("LeftAxisContainer") as StackPanel);
			this._leftAxisPanel = (base.GetTemplateChild("LeftAxisPanel") as StackPanel);
			this._leftAxisScrollBar = (base.GetTemplateChild("LeftAxisScrollBar") as ZoomBar);
			this._rightAxisGrid = (base.GetTemplateChild("RightAxisGrid") as Grid);
			this._rightAxisContainer = (base.GetTemplateChild("RightAxisContainer") as StackPanel);
			this._rightAxisScrollBar = (base.GetTemplateChild("RightAxisScrollBar") as ZoomBar);
			this._rightAxisPanel = (base.GetTemplateChild("RightAxisPanel") as StackPanel);
			this._bottomAxisGrid = (base.GetTemplateChild("BottomAxisGrid") as Grid);
			this._bottomAxisContainer = (base.GetTemplateChild("BottomAxisContainer") as StackPanel);
			this._bottomAxisScrollBar = (base.GetTemplateChild("BottomAxisScrollBar") as ZoomBar);
			this._bottomAxisPanel = (base.GetTemplateChild("BottomAxisPanel") as StackPanel);
			this._centerInnerGrid = (base.GetTemplateChild("CenterInnerGrid") as Grid);
			this._innerGrid = (base.GetTemplateChild("InnerGrid") as Grid);
			this._topInnerPanel = (base.GetTemplateChild("TopInnerPanel") as StackPanel);
			this._topInnerTitlePanel = (base.GetTemplateChild("TopInnerTitlePanel") as StackPanel);
			this._topInnerLegendPanel = (base.GetTemplateChild("TopInnerLegendPanel") as StackPanel);
			this._bottomInnerPanel = (base.GetTemplateChild("BottomInnerPanel") as StackPanel);
			this._bottomInnerLegendPanel = (base.GetTemplateChild("BottomInnerLegendPanel") as StackPanel);
			this._bottomInnerTitlePanel = (base.GetTemplateChild("BottomInnerTitlePanel") as StackPanel);
			this._leftInnerPanel = (base.GetTemplateChild("LeftInnerPanel") as StackPanel);
			this._leftInnerTitlePanel = (base.GetTemplateChild("LeftInnerTitlePanel") as StackPanel);
			this._leftInnerLegendPanel = (base.GetTemplateChild("LeftInnerLegendPanel") as StackPanel);
			this._rightInnerPanel = (base.GetTemplateChild("RightInnerPanel") as StackPanel);
			this._rightInnerLegendPanel = (base.GetTemplateChild("RightInnerLegendPanel") as StackPanel);
			this._rightInnerTitlePanel = (base.GetTemplateChild("RightInnerTitlePanel") as StackPanel);
			this._plotAreaGrid = (base.GetTemplateChild("PlotAreaGrid") as Grid);
			this._plotAreaScrollViewer = (base.GetTemplateChild("PlotAreaScrollViewer") as Panel);
			if (this._plotAreaScrollViewer != null)
			{
				this._plotAreaScrollViewer.ClipToBounds = true;
			}
			this._plotCanvas = (base.GetTemplateChild("PlotCanvas") as Panel);
			this._plotAreaShadowCanvas = (base.GetTemplateChild("PlotAreaShadowCanvas") as Canvas);
			this._drawingCanvas = (base.GetTemplateChild("DrawingCanvas") as Canvas);
			this._centerDockInsidePlotAreaPanel = (base.GetTemplateChild("CenterDockInsidePlotAreaPanel") as StackPanel);
			this._centerDockOutsidePlotAreaPanel = (base.GetTemplateChild("CenterDockOutsidePlotAreaPanel") as StackPanel);
			this._toolTipCanvas = (base.GetTemplateChild("ToolTipCanvas") as Canvas);
			this._zoomRectangle = (base.GetTemplateChild("ZoomRectangle") as Border);
			this._zoomAreaCanvas = (base.GetTemplateChild("ZoomAreaCanvas") as Canvas);
			this._elementCanvas = (base.GetTemplateChild("ElementCanvas") as Canvas);
			this._licenseInfoElement = (base.GetTemplateChild("LicenseInfoElement") as Border);
		}

		private void Init()
		{
			this.ToolTips = new ToolTipCollection();
			this.Titles = new TitleCollection();
			this.Legends = new LegendCollection();
			this.TrendLines = new TrendLineCollection();
			this.AxesX = new AxisCollection();
			this.AxesY = new AxisCollection();
			this.Series = new DataSeriesCollection();
			this.ToolTips.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ToolTips_CollectionChanged);
			this.Titles.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Titles_CollectionChanged);
			this.Legends.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Legends_CollectionChanged);
			this.TrendLines.CollectionChanged += new NotifyCollectionChangedEventHandler(this.TrendLines_CollectionChanged);
			this.Series.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Series_CollectionChanged);
			this.AxesX.CollectionChanged += new NotifyCollectionChangedEventHandler(this.AxesX_CollectionChanged);
			this.AxesY.CollectionChanged += new NotifyCollectionChangedEventHandler(this.AxesY_CollectionChanged);
		}

		private void Chart_MouseLeave(object sender, MouseEventArgs e)
		{
			if (base.ToolTipEnabled)
			{
				if (this._toolTip.Visibility == Visibility.Visible)
				{
					this._toolTip.Hide();
				}
			}
			else
			{
				this._toolTip.Hide();
			}
			this._toolTip.Text = "";
		}

		private void Chart_MouseMove(object sender, MouseEventArgs e)
		{
			base.UpdateToolTipPosition(sender, e);
		}

		private void ToolTips_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems == null)
				{
					return;
				}
				using (IEnumerator enumerator = e.NewItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ToolTip toolTip = (ToolTip)enumerator.Current;
						if (toolTip.Style == null && this.StyleDictionary != null)
						{
							Style style = this.StyleDictionary["ToolTip"] as Style;
							if (style != null)
							{
								toolTip.Style = style;
							}
						}
						if (string.IsNullOrEmpty((string)toolTip.GetValue(FrameworkElement.NameProperty)))
						{
							toolTip.Name = string.Concat(new object[]
							{
								"ToolTip",
								this.ToolTips.IndexOf(toolTip),
								"_",
								Guid.NewGuid().ToString().Replace('-', '_')
							});
						}
						toolTip.Chart = this;
					}
					return;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (ToolTip toolTip2 in e.OldItems)
				{
					Chart chart = toolTip2.Chart as Chart;
					if (chart != null && chart._toolTipCanvas != null)
					{
						chart._toolTipCanvas.Children.Remove(toolTip2);
					}
				}
			}
		}

		private void AxesX_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			bool flag = false;
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems == null)
				{
					goto IL_2A0;
				}
				using (IEnumerator enumerator = e.NewItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Axis axis = (Axis)enumerator.Current;
						axis.Chart = this;
						if (axis._isAutoGenerated)
						{
							flag = true;
						}
						if (axis.AxisLabels != null)
						{
							axis.AxisLabels.Chart = this;
						}
						foreach (ChartGrid current in axis.Grids)
						{
							current.Chart = this;
						}
						foreach (Ticks current2 in axis.Ticks)
						{
							current2.Chart = this;
						}
						axis.IsNotificationEnable = false;
						if (!axis.StartFromZero.HasValue)
						{
							axis.StartFromZero = new bool?(false);
						}
						axis.IsNotificationEnable = true;
						axis.PropertyChanged -= new PropertyChangedEventHandler(this.Element_PropertyChanged);
						axis.PropertyChanged += new PropertyChangedEventHandler(this.Element_PropertyChanged);
						this.AddAxisToChartRootElement(axis);
					}
					goto IL_2A0;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				if (e.OldItems == null)
				{
					goto IL_2A0;
				}
				using (IEnumerator enumerator4 = e.OldItems.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						Axis axis2 = (Axis)enumerator4.Current;
						axis2.ClearEventHandlers();
						axis2.Chart = null;
						if (this._rootElement != null && axis2.Parent != null)
						{
							this._rootElement.Children.Remove(axis2);
						}
					}
					goto IL_2A0;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				if (this.InternalAxesX != null)
				{
					foreach (Axis current3 in this.InternalAxesX)
					{
						current3.ClearEventHandlers();
						current3.Chart = null;
						if (this._rootElement != null && current3.Parent != null)
						{
							this._rootElement.Children.Remove(current3);
						}
					}
					this.InternalAxesX.Clear();
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems != null)
			{
				foreach (Axis axis3 in e.NewItems)
				{
					axis3.Chart = this;
				}
			}
			IL_2A0:
			this._forcedRedraw = true;
			if (!flag)
			{
				this.InvokeRender();
			}
		}

		private void AxesY_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			bool flag = false;
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems == null)
				{
					goto IL_2A7;
				}
				using (IEnumerator enumerator = e.NewItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Axis axis = (Axis)enumerator.Current;
						axis.Chart = this;
						if (axis._isAutoGenerated)
						{
							flag = true;
						}
						axis.AxisRepresentation = AxisRepresentations.AxisY;
						if (axis.AxisLabels != null)
						{
							axis.AxisLabels.Chart = this;
						}
						foreach (ChartGrid current in axis.Grids)
						{
							current.Chart = this;
						}
						foreach (Ticks current2 in axis.Ticks)
						{
							current2.Chart = this;
						}
						axis.IsNotificationEnable = false;
						if (!axis.StartFromZero.HasValue)
						{
							axis.StartFromZero = new bool?(true);
						}
						axis.IsNotificationEnable = true;
						axis.PropertyChanged -= new PropertyChangedEventHandler(this.Element_PropertyChanged);
						axis.PropertyChanged += new PropertyChangedEventHandler(this.Element_PropertyChanged);
						this.AddAxisToChartRootElement(axis);
					}
					goto IL_2A7;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				if (e.OldItems == null)
				{
					goto IL_2A7;
				}
				using (IEnumerator enumerator4 = e.OldItems.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						Axis axis2 = (Axis)enumerator4.Current;
						axis2.ClearEventHandlers();
						axis2.Chart = null;
						if (this._rootElement != null && axis2.Parent != null)
						{
							this._rootElement.Children.Remove(axis2);
						}
					}
					goto IL_2A7;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				if (this.InternalAxesY != null)
				{
					foreach (Axis current3 in this.InternalAxesY)
					{
						current3.ClearEventHandlers();
						current3.Chart = null;
						if (this._rootElement != null && current3.Parent != null)
						{
							this._rootElement.Children.Remove(current3);
						}
					}
					this.InternalAxesY.Clear();
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems != null)
			{
				foreach (Axis axis3 in e.NewItems)
				{
					axis3.Chart = this;
				}
			}
			IL_2A7:
			this._forcedRedraw = true;
			if (!flag)
			{
				this.InvokeRender();
			}
		}

		private void Titles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems == null)
				{
					goto IL_141;
				}
				using (IEnumerator enumerator = e.NewItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Title title = (Title)enumerator.Current;
						title.Chart = this;
						title.PropertyChanged -= new PropertyChangedEventHandler(this.Element_PropertyChanged);
						title.PropertyChanged += new PropertyChangedEventHandler(this.Element_PropertyChanged);
						this.AddTitleToRootElement(title);
					}
					goto IL_141;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				using (IEnumerator enumerator2 = e.OldItems.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Title title2 = (Title)enumerator2.Current;
						title2.Chart = null;
						if (this._rootElement != null && title2.Parent != null)
						{
							this._rootElement.Children.Remove(title2);
						}
					}
					goto IL_141;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems != null)
			{
				foreach (Title title3 in e.NewItems)
				{
					title3.Chart = this;
				}
			}
			IL_141:
			this.InvokeRender();
		}

		private void Legends_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			bool flag = false;
			if (e.NewItems != null)
			{
				foreach (Legend legend in e.NewItems)
				{
					if (legend._isAutoGenerated)
					{
						flag = true;
					}
					if (this.Legends.Count > 0)
					{
						if (string.IsNullOrEmpty((string)legend.GetValue(FrameworkElement.NameProperty)))
						{
							if (flag)
							{
								legend.InternalName = "Legend" + this.Legends.IndexOf(legend);
							}
							else
							{
								legend.InternalName = string.Concat(new object[]
								{
									"Legend",
									this.Legends.IndexOf(legend),
									"_",
									Guid.NewGuid().ToString().Replace('-', '_')
								});
							}
							legend._isAutoName = true;
						}
						else if (!flag)
						{
							legend._isAutoName = false;
						}
						else
						{
							legend.InternalName = (string)legend.GetValue(FrameworkElement.NameProperty);
						}
					}
					legend.Chart = this;
					legend.PropertyChanged += new PropertyChangedEventHandler(this.Element_PropertyChanged);
					this.AddLegendToRootElement(legend);
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems != null)
			{
				foreach (Legend legend2 in e.NewItems)
				{
					legend2.Chart = this;
				}
			}
			if (!flag)
			{
				this.InvokeRender();
			}
		}

		private void TrendLines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems == null)
				{
					goto IL_186;
				}
				using (IEnumerator enumerator = e.NewItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TrendLine trendLine = (TrendLine)enumerator.Current;
						trendLine.Chart = this;
						if (!string.IsNullOrEmpty(this.Theme))
						{
							trendLine.ApplyStyleFromTheme(this, "TrendLine");
						}
						trendLine.PropertyChanged -= new PropertyChangedEventHandler(this.Element_PropertyChanged);
						trendLine.PropertyChanged += new PropertyChangedEventHandler(this.Element_PropertyChanged);
						this.AddTrendLineToRootElement(trendLine);
					}
					goto IL_186;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
			{
				if (e.OldItems == null)
				{
					goto IL_186;
				}
				using (IEnumerator enumerator2 = e.OldItems.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TrendLine trendLine2 = (TrendLine)enumerator2.Current;
						trendLine2.Chart = null;
						trendLine2.PropertyChanged -= new PropertyChangedEventHandler(this.Element_PropertyChanged);
						if (this._rootElement != null && trendLine2.Parent != null)
						{
							this._rootElement.Children.Remove(trendLine2);
						}
					}
					goto IL_186;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems != null)
			{
				foreach (TrendLine trendLine3 in e.NewItems)
				{
					trendLine3.Chart = this;
				}
			}
			IL_186:
			this._forcedRedraw = true;
			this.InvokeRender();
		}

		private void Series_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems == null)
				{
					goto IL_571;
				}
				using (IEnumerator enumerator = e.NewItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DataSeries dataSeries = (DataSeries)enumerator.Current;
						dataSeries.Chart = this;
						if (dataSeries.DataPoints != null)
						{
							foreach (IDataPoint current in dataSeries.DataPoints)
							{
								current.Chart = this;
							}
						}
						if (!string.IsNullOrEmpty(this.Theme))
						{
							dataSeries.ApplyStyleFromTheme(this, "DataSeries");
						}
						if (string.IsNullOrEmpty((string)dataSeries.GetValue(DataSeries.NameProperty)))
						{
							dataSeries.Name = "DataSeries" + (this.Series.Count - 1).ToString() + "_" + Guid.NewGuid().ToString().Replace('-', '_');
							dataSeries._isAutoName = true;
						}
						else
						{
							dataSeries._isAutoName = false;
						}
						dataSeries.PropertyChanged -= new PropertyChangedEventHandler(this.Element_PropertyChanged);
						dataSeries.PropertyChanged += new PropertyChangedEventHandler(this.Element_PropertyChanged);
						this.AddDataSeriesToRootElement(dataSeries);
					}
					goto IL_571;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				if (e.OldItems == null)
				{
					goto IL_571;
				}
				List<DataSeries> source = (from x in this.Series
				where !e.OldItems.Contains(x)
				select x).ToList<DataSeries>();
				List<Panel> list = (from can in source
				select can.Visual).ToList<Panel>();
				using (IEnumerator enumerator3 = e.OldItems.GetEnumerator())
				{
					DataSeries ds;
					while (enumerator3.MoveNext())
					{
						ds = (DataSeries)enumerator3.Current;
						IEnumerable<ToolTip> source2 = from entry in this.ToolTips
						where entry.Name == ds.ToolTipName
						select entry;
						if (source2.Count<ToolTip>() == 0)
						{
							ds.RemoveToolTip();
						}
						else if (ds.ToolTipElement != null)
						{
							if (ds.ToolTipElement._isAutoGenerated)
							{
								ds.RemoveToolTip();
							}
							else
							{
								ds.ToolTipElement.Hide();
								ds.ToolTipElement = null;
							}
						}
						if (this._rootElement != null && ds.Parent != null)
						{
							this._rootElement.Children.Remove(ds);
						}
						if (ds.Visual != null)
						{
							if (list.Contains(ds.Visual))
							{
								continue;
							}
							Panel visual = ds.Visual;
							if (visual != null && visual.Parent != null)
							{
								Panel panel = visual.Parent as Panel;
								panel.Children.Remove(visual);
							}
							ds.Visual = null;
						}
						ds.DataSource = null;
						ds.Chart = null;
					}
					goto IL_571;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				if (this.InternalSeries != null)
				{
					foreach (DataSeries ds in this.InternalSeries)
					{
						Panel visual2 = ds.Visual;
						if (visual2 != null && visual2.Parent != null)
						{
							Panel panel2 = visual2.Parent as Panel;
							panel2.Children.Remove(visual2);
						}
						IEnumerable<ToolTip> source3 = from entry in this.ToolTips
						where entry.Name == ds.ToolTipName
						select entry;
						if (source3.Count<ToolTip>() == 0)
						{
							ds.RemoveToolTip();
						}
						else if (ds.ToolTipElement != null)
						{
							if (ds.ToolTipElement._isAutoGenerated)
							{
								ds.RemoveToolTip();
							}
							else
							{
								ds.ToolTipElement.Hide();
								ds.ToolTipElement = null;
							}
						}
						ds.DataSource = null;
						ds.Visual = null;
						ds.Chart = null;
						if (this._rootElement != null && ds.Parent != null)
						{
							this._rootElement.Children.Remove(ds);
						}
					}
					this.InternalSeries.Clear();
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems != null)
			{
				foreach (DataSeries dataSeries2 in e.NewItems)
				{
					dataSeries2.Chart = this;
				}
			}
			IL_571:
			this._datapoint2UpdatePartially = null;
			this.InvokeRender();
		}

		private void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.InvokeRender();
		}

		private void ResetInternalFlags()
		{
			if (this.ChartArea != null)
			{
				this.ChartArea._isFirstTimeRender = true;
				this.ChartArea._isAnimationFired = false;
			}
			this._forcedRedraw = true;
			this._internalAnimationEnabled = true;
		}

		private void LoadOrUpdateColorSets()
		{
			if (this.EmbeddedColorSets == null)
			{
				string name = "Visifire.Charts.ColorSets.xaml";
				using (Stream manifestResourceStream = typeof(Chart).Assembly.GetManifestResourceStream(name))
				{
					if (manifestResourceStream != null)
					{
						StreamReader streamReader = new StreamReader(manifestResourceStream);
						string s = streamReader.ReadToEnd();
						this.EmbeddedColorSets = (XamlReader.Load(new XmlTextReader(new StringReader(s))) as ColorSets);
						streamReader.Close();
						manifestResourceStream.Close();
					}
				}
			}
		}

		private void BindProperties()
		{
			Binding binding = new Binding("Background");
			binding.Source = this;
			binding.Mode = BindingMode.OneWay;
			base.SetBinding(Chart.InternalBackgroundProperty, binding);
			Binding binding2 = new Binding("BorderThickness");
			binding2.Source = this;
			binding2.Mode = BindingMode.OneWay;
			base.SetBinding(Chart.InternalBorderThicknessProperty, binding2);
		}

		private void PrepareChartAreaForDrawing()
		{
			if (this.ChartArea == null)
			{
				this.ChartArea = new ChartArea(this);
			}
			NameScope.SetNameScope(this._rootElement, new NameScope());
			this.ApplyChartBevel();
			this.ApplyChartShadow(base.ActualWidth, base.ActualHeight);
			this._renderLapsedCounter = 0;
		}

		private bool ApplyChartBevel()
		{
			if (this.Bevel && this._rootElement != null)
			{
				if (this._chartBorder.ActualWidth == 0.0 && this._chartBorder.ActualHeight == 0.0)
				{
					return false;
				}
				this._chartAreaGrid.Margin = new Thickness(this._chartAreaMargin.Left + 5.0, this._chartAreaMargin.Top + 5.0, this._chartAreaMargin.Right, this._chartAreaMargin.Bottom);
				this._chartAreaGrid.UpdateLayout();
				this._bevelCanvas.Children.Clear();
				Brush bevelTopBrush = Graphics.GetBevelTopBrush(this.Background);
				Brush bevelSideBrush = Graphics.GetBevelSideBrush(0.0, this.Background);
				Brush bevelSideBrush2 = Graphics.GetBevelSideBrush(170.0, this.Background);
				Brush bevelSideBrush3 = Graphics.GetBevelSideBrush(180.0, this.Background);
				this.BevelVisual = ExtendedGraphics.Get2DRectangleBevel(null, this._chartBorder.ActualWidth - this._chartBorder.BorderThickness.Left - this._chartBorder.BorderThickness.Right - this._chartAreaMargin.Right - this._chartAreaMargin.Left, this._chartBorder.ActualHeight - this._chartBorder.BorderThickness.Top - this._chartBorder.BorderThickness.Bottom - this._chartAreaMargin.Top - this._chartAreaMargin.Bottom, 5.0, 5.0, bevelTopBrush, bevelSideBrush, bevelSideBrush2, bevelSideBrush3);
				if (this.LightingEnabled)
				{
					this._chartLightingBorder.Opacity = 0.4;
				}
				this.BevelVisual.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
				this.BevelVisual.IsHitTestVisible = false;
				this.BevelVisual.SetValue(Canvas.TopProperty, this._chartAreaMargin.Top + this._chartBorder.BorderThickness.Top);
				this.BevelVisual.SetValue(Canvas.LeftProperty, this._chartAreaMargin.Left + this._chartBorder.BorderThickness.Left);
				this._bevelCanvas.Children.Add(this.BevelVisual);
			}
			return true;
		}

		private void RemoveChartBevel()
		{
			if (this._isTemplateApplied && this.BevelVisual != null && this._bevelCanvas != null)
			{
				this._bevelCanvas.Children.Clear();
				this._chartAreaGrid.Margin = new Thickness(0.0);
			}
		}

		private void ApplyLighting()
		{
			if (this._chartLightingBorder != null)
			{
				if (this.LightingEnabled)
				{
					this._chartLightingBorder.Visibility = Visibility.Visible;
					this._chartLightingBorder.Opacity = 0.4;
					return;
				}
				this._chartLightingBorder.Visibility = Visibility.Collapsed;
			}
		}

		private void ApplyChartShadow(double width, double height)
		{
			if (!this._isShadowApplied && this.ShadowEnabled && !double.IsNaN(height) && height != 0.0 && !double.IsNaN(width) && width != 0.0)
			{
				if (!VisifireControl.IsMediaEffectsEnabled)
				{
					this._shadowGrid.Children.Clear();
					if (this._rootElement != null)
					{
						this.ChartShadowLayer = ExtendedGraphics.Get2DRectangleShadow(null, width - 5.0, height - 5.0, new CornerRadius(6.0), new CornerRadius(6.0), 6.0);
						this.ChartShadowLayer.Width = width - 5.0;
						this.ChartShadowLayer.Height = height - 5.0;
						this.ChartShadowLayer.IsHitTestVisible = false;
						this.ChartShadowLayer.SetValue(Panel.ZIndexProperty, 0);
						this._shadowGrid.Children.Add(this.ChartShadowLayer);
						this.ChartShadowLayer.Margin = new Thickness(5.0, 5.0, 0.0, 0.0);
						if (this._chartBorder != null)
						{
							this._chartBorder.Margin = new Thickness(0.0, 0.0, 5.0, 5.0);
						}
						this._isShadowApplied = true;
					}
				}
				else
				{
					if (this._rootElement != null)
					{
						this._rootElement.Effect = ExtendedGraphics.GetShadowEffect(315.0, 4.0, 0.95);
					}
					this._isShadowApplied = true;
				}
			}
			if (!VisifireControl.IsMediaEffectsEnabled && this.ShadowEnabled && this.ChartShadowLayer != null)
			{
				this.ChartShadowLayer.Visibility = Visibility.Visible;
			}
		}

		private void RemoveShadow()
		{
			if (this._isTemplateApplied && !this.ShadowEnabled)
			{
				this._chartBorder.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
				this._bevelCanvas.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
				this._isShadowApplied = false;
				if (!VisifireControl.IsMediaEffectsEnabled)
				{
					if (this.ChartShadowLayer != null)
					{
						this.ChartShadowLayer.Visibility = Visibility.Collapsed;
						return;
					}
				}
				else if (this._rootElement != null)
				{
					this._rootElement.Effect = null;
				}
			}
		}

		private void LoadTheme(string themeName, bool isThemePropertyChanged)
		{
			if (string.IsNullOrEmpty(themeName))
			{
				return;
			}
			this.StyleDictionary = null;
			string name = "Visifire.Charts." + themeName + ".xaml";
			using (Stream manifestResourceStream = typeof(Chart).Assembly.GetManifestResourceStream(name))
			{
				if (manifestResourceStream == null)
				{
					ResourceDictionary styleDictionary = (ResourceDictionary)Application.LoadComponent(new Uri(themeName + ".xaml", UriKind.RelativeOrAbsolute));
					this.StyleDictionary = styleDictionary;
				}
				else
				{
					StreamReader streamReader = new StreamReader(manifestResourceStream);
					string s = streamReader.ReadToEnd();
					this.StyleDictionary = (ResourceDictionary)XamlReader.Load(new XmlTextReader(new StringReader(s)));
					streamReader.Close();
					manifestResourceStream.Close();
				}
			}
			if (this.StyleDictionary != null)
			{
				Style style = this.StyleDictionary["Chart"] as Style;
				this._isThemeChanged = isThemePropertyChanged;
				if (style != null)
				{
					if (isThemePropertyChanged)
					{
						base.Style = style;
						return;
					}
					if (base.Style == null)
					{
						base.Style = style;
						return;
					}
				}
				return;
			}
			throw new Exception("Theme file " + themeName + ".xaml not found..");
		}

		private void LoadDefaultToolTip()
		{
			if (this.ToolTips.Count == 0)
			{
				ToolTip toolTip = new ToolTip
				{
					_isAutoGenerated = true,
					Visibility = Visibility.Collapsed
				};
				this.ToolTips.Add(toolTip);
				this._toolTip = toolTip;
				if (base.IsInDesignMode)
				{
					if (this._toolTip.Parent != null && this._toolTipCanvas != this._toolTip.Parent)
					{
						ObservableObject.RemoveElementFromElementTree(this._toolTip);
					}
					if (this._toolTip.Parent == null)
					{
						this._toolTipCanvas.Children.Add(this._toolTip);
					}
				}
				else if (this._toolTip.Parent == null)
				{
					this._toolTipCanvas.Children.Add(this._toolTip);
				}
			}
			this._rootElement.MouseLeave += new MouseEventHandler(this.Chart_MouseLeave);
		}

		private void AttachToolTipAndEvents()
		{
			if (!string.IsNullOrEmpty(this.ToolTipText))
			{
				base.AttachToolTip(this, this, this);
			}
			base.AttachEvents2Visual(this, this, this._rootElement);
			base.AttachEvents2Visual4MouseDownEvent(this, this, this._plotCanvas);
		}

		private static void OnSmartLabelEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.InvokeRender();
		}

		private static void OnIndicatorEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.InvokeRender();
		}

		private static void OnIndicatorLineColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			if (chart.ChartArea != null && chart.ChartArea._verticalLineIndicator != null)
			{
				chart.ChartArea._verticalLineIndicator.Stroke = (Brush)e.NewValue;
			}
		}

		private static void OnAxisIndicatorBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			if (chart.ChartArea != null)
			{
				if (chart.ChartArea._axisIndicatorBorderElement != null)
				{
					chart.ChartArea._axisIndicatorBorderElement.Background = (Brush)e.NewValue;
				}
				if (chart.ChartArea._callOutPath4AxisIndicator != null)
				{
					chart.ChartArea._callOutPath4AxisIndicator.Fill = (Brush)e.NewValue;
				}
			}
		}

		private static void OnAxisIndicatorFontSizePropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			if (chart.ChartArea != null && chart.ChartArea._axisIndicatorTextBlock != null && e.NewValue != null)
			{
				chart.ChartArea._axisIndicatorTextBlock.FontSize = (double)e.NewValue;
			}
		}

		private static void OnAxisIndicatorFontColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			if (chart.ChartArea != null && chart.ChartArea._axisIndicatorTextBlock != null)
			{
				chart.ChartArea._axisIndicatorTextBlock.Foreground = (Brush)e.NewValue;
			}
		}

		private static void OnDataPointWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.InvokeRender();
		}

		private static void OnMaxDataPointWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((double)e.NewValue < 0.0)
			{
				throw new ArgumentOutOfRangeException("MaxDataPointWidth should be always greater than or equals to 0.");
			}
			Chart chart = d as Chart;
			chart.InvokeRender();
		}

		private static void OnScrollingEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.InvokeRender();
		}

		private static void OnUniqueColorsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.InvokeRender();
		}

		private static void OnView3DPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart._forcedRedraw = true;
			chart.InvokeRender();
		}

		private static void OnHrefTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.AttachHref(chart, chart, chart.Href, (HrefTargets)e.NewValue);
		}

		private static void OnHrefChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.AttachHref(chart, chart, e.NewValue.ToString(), chart.HrefTarget);
		}

		private static void OnThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.LoadTheme((string)e.NewValue, e.OldValue != null);
			chart.InvokeRender();
		}

		private static void OnInternalBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.RemoveChartBevel();
			chart.InvokeRender();
		}

		private static void OnInternalBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.RemoveChartBevel();
			chart.ApplyChartBevel();
			chart.InvokeRender();
		}

		private static void OnInternalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			if (chart.ApplyChartBevel())
			{
				if (chart.Bevel)
				{
					chart.InvokeRender();
					return;
				}
				chart.RemoveChartBevel();
			}
		}

		private static void OnBevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.RemoveChartBevel();
			chart.InvokeRender();
		}

		private static void OnColorSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.InvokeRender();
		}

		private static void OnLightingEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.ApplyLighting();
		}

		private static void OnShadowEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			if ((bool)e.NewValue)
			{
				chart.ApplyChartShadow(chart.ActualWidth, chart.ActualHeight);
			}
			else
			{
				chart.RemoveShadow();
			}
			chart.ApplyChartBevel();
		}

		private static void OnPlotAreaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			PlotArea plotArea = (PlotArea)e.NewValue;
			plotArea.Chart = chart;
			plotArea.PropertyChanged -= new PropertyChangedEventHandler(chart.Element_PropertyChanged);
			plotArea.PropertyChanged += new PropertyChangedEventHandler(chart.Element_PropertyChanged);
			if (!plotArea.IsDefault)
			{
				chart.InvokeRender();
			}
		}

		private static void OnMinimumGapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Chart chart = d as Chart;
			chart.InvokeRender();
		}

		internal override void OnToolTipFontSizePropertyValueChanged()
		{
			base.OnToolTipFontSizePropertyValueChanged();
			if (this.ChartArea != null && !this.ChartArea._isFirstTimeRender)
			{
				this.ChartArea.PositionIndicator(this, this.ChartArea._storedIndicatorEventArgs, double.NaN, double.NaN);
			}
		}

		internal static void SelectDataPoints(Chart chart)
		{
			if (chart == null || chart.InternalSeries == null)
			{
				return;
			}
			foreach (DataSeries current in chart.InternalSeries)
			{
				if (current.SelectionEnabled && current.ListOfSelectedDataPoints != null)
				{
					foreach (IDataPoint current2 in current.ListOfSelectedDataPoints)
					{
						if (!current2.IsLightDataPoint)
						{
							if ((current2 as DataPoint).Selected)
							{
								DataPointHelper.Select(current2, true);
								if (current.SelectionMode == SelectionModes.Single)
								{
									DataPointHelper.DeSelectOthers(current2);
								}
							}
							else
							{
								DataPointHelper.DeSelect(current2, true, true);
							}
						}
					}
				}
			}
		}

		internal ColorSet GetColorSetByName(string id)
		{
			ColorSet colorSet = null;
			if (this.ColorSets != null && this.ColorSets.Count > 0)
			{
				colorSet = this.ColorSets.GetColorSetByName(id);
			}
			if (colorSet == null)
			{
				colorSet = this.EmbeddedColorSets.GetColorSetByName(id);
			}
			return colorSet;
		}

		internal void FireRenderedEvent()
		{
			if (this._rendered != null)
			{
				this._rendered(this, null);
			}
		}

		internal bool CheckSizeError(ArgumentException e)
		{
			if (e == null)
			{
				return false;
			}
			string text = e.StackTrace.TrimStart(new char[0]);
			return text.Contains("set_Height(Double value)") || text.Contains("set_Width(Double value)") || e.Message == "Height must be non-negative." || e.Message == "Width must be non-negative." || e.Message == "Size must be non-negative." || e.Message == "Internal Size Error" || e.Message.ToLower().Contains("width") || e.Message.ToLower().Contains("height");
		}

		public override void OnToolTipTextPropertyChanged(string newValue)
		{
			base.OnToolTipTextPropertyChanged(newValue);
			base.DetachToolTip(this._toolTipCanvas);
			if (!string.IsNullOrEmpty(newValue))
			{
				base.AttachToolTip(this, this, this);
			}
		}

		internal List<Title> GetTitlesDockedInsidePlotArea()
		{
			if (this.Titles != null)
			{
				IEnumerable<Title> source = from title in this.Titles
				where title.DockInsidePlotArea
				select title;
				return source.ToList<Title>();
			}
			return null;
		}

		internal List<Title> GetTitlesDockedOutSidePlotArea()
		{
			if (this.Titles != null)
			{
				IEnumerable<Title> source = from title in this.Titles
				where !title.DockInsidePlotArea
				select title;
				return source.ToList<Title>();
			}
			return null;
		}

		internal void InvokeRender()
		{
			this.InvokeRenderOfChart();
		}

		private void InvokeRenderOfChart()
		{
			if (this._isTemplateApplied)
			{
				if (this.RENDER_LOCK)
				{
					if (this.ChartArea != null && this.ChartArea._isFirstTimeRender)
					{
						this._renderLapsedCounter++;
						return;
					}
					if (Application.Current != null && Application.Current.Dispatcher.Thread == Thread.CurrentThread)
					{
						this._renderLapsedCounter++;
						return;
					}
					this._renderLapsedCounter = 0;
					return;
				}
				else
				{
					if (Application.Current != null && Application.Current.Dispatcher.Thread != Thread.CurrentThread)
					{
						base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Chart.RenderDelegate(this.Render));
					}
					else
					{
						base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Chart.RenderDelegate(this.Render));
					}
					this.IS_FULL_RENDER_PENDING = true;
				}
			}
		}

		internal void Render()
		{
			if (this._isTemplateApplied && !this.RENDER_LOCK && this._rootElement != null)
			{
				if (double.IsNaN(base.ActualWidth) || double.IsNaN(base.ActualHeight) || base.ActualWidth == 0.0 || base.ActualHeight == 0.0)
				{
					return;
				}
				this.RENDER_LOCK = true;
				try
				{
					this.PrepareChartAreaForDrawing();
					this.ChartArea.Draw(this);
					this.IS_FULL_RENDER_PENDING = false;
				}
				catch (Exception ex)
				{
					this.RENDER_LOCK = false;
					this.IS_FULL_RENDER_PENDING = false;
					if (!this.CheckSizeError(ex as ArgumentException))
					{
						throw new Exception(ex.Message, ex);
					}
				}
			}
		}

		internal void UnlockRender()
		{
			this.RENDER_LOCK = false;
			if (this.ChartArea != null && !this.ChartArea._isFirstTimeRender && this._renderLapsedCounter >= 1)
			{
				this.Render();
				return;
			}
			if (this.ChartArea._isFirstTimeRender && !this.InternalAnimationEnabled && this._renderLapsedCounter >= 1)
			{
				this.Render();
			}
		}

		internal static Brush CalculateDataPointLabelFontColor(Chart chart, IDataPoint dataPoint, Brush labelFontColor, LabelStyles labelStyle)
		{
			Brush result = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
			if (labelFontColor == null)
			{
				if (labelStyle == LabelStyles.Inside && !RenderHelper.IsLineCType(dataPoint.Parent))
				{
					double brushIntensity = Graphics.GetBrushIntensity(dataPoint.Color);
					result = Graphics.GetDefaultFontColor(brushIntensity);
				}
				else if (chart.PlotArea.InternalBackground == null)
				{
					if (chart.Background == null)
					{
						result = Graphics.AUTO_BLACK_FONT_BRUSH;
					}
					else if (chart.Background.GetType().Name == "ImageBrush")
					{
						result = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));
					}
					else
					{
						double brushIntensity = Graphics.GetBrushIntensity(chart.Background);
						result = Graphics.GetDefaultFontColor(brushIntensity);
					}
				}
				else
				{
					double brushIntensity = Graphics.GetBrushIntensity(chart.PlotArea.InternalBackground);
					result = Graphics.GetDefaultFontColor(brushIntensity);
				}
			}
			return result;
		}

		internal static Brush CalculateFontColor(Chart chart, Brush backgroundColor, Brush fontColor, bool dockInsidePlotArea)
		{
			Brush result = fontColor;
			if (fontColor == null)
			{
				if (backgroundColor == null || Graphics.AreBrushesEqual(backgroundColor, new SolidColorBrush(Colors.Transparent)))
				{
					if (!dockInsidePlotArea)
					{
						if (chart != null)
						{
							if (Graphics.AreBrushesEqual(chart.Background, Graphics.TRANSPARENT_BRUSH) || chart.Background == null)
							{
								result = Graphics.BLACK_BRUSH;
							}
							else
							{
								double brushIntensity = Graphics.GetBrushIntensity(chart.Background);
								result = Graphics.GetDefaultFontColor(brushIntensity);
							}
						}
					}
					else if (chart.PlotArea != null)
					{
						if (Graphics.AreBrushesEqual(chart.PlotArea.InternalBackground, Graphics.TRANSPARENT_BRUSH) || chart.PlotArea.InternalBackground == null)
						{
							if (Graphics.AreBrushesEqual(chart.Background, Graphics.TRANSPARENT_BRUSH) || chart.Background == null)
							{
								result = Graphics.BLACK_BRUSH;
							}
							else
							{
								double brushIntensity = Graphics.GetBrushIntensity(chart.Background);
								result = Graphics.GetDefaultFontColor(brushIntensity);
							}
						}
						else
						{
							double brushIntensity = Graphics.GetBrushIntensity(chart.PlotArea.InternalBackground);
							result = Graphics.GetDefaultFontColor(brushIntensity);
						}
					}
				}
				else
				{
					double brushIntensity = Graphics.GetBrushIntensity(backgroundColor);
					result = Graphics.GetDefaultFontColor(brushIntensity);
				}
			}
			return result;
		}

		private void ShowLicenseInfo()
		{
			this._licenseInfoElement.Visibility = Visibility.Visible;
			this.ApplyDownAnimation(this._licenseInfoElement);
		}

		internal void DisplayLicenseInfoBanner()
		{
			if (this._isFirstTime)
			{
				this._licenseInfoElement.CornerRadius = new CornerRadius(this.CornerRadius.TopLeft, this.CornerRadius.TopRight, 0.0, 0.0);
				this._licenseInfoElement.Cursor = Cursors.Hand;
				this._licenseInfoElement.MouseLeftButtonUp += new MouseButtonEventHandler(this._licenseInfoElement_MouseLeftButtonUp);
				this._bannerDisplayTimer.Tick += new EventHandler(this._bannerDisplayTimer_Tick);
				this._bannerDisplayTimer.Start();
				this._bannerDisplayTimer.Interval = TimeSpan.FromSeconds(1.0);
			}
			this._isFirstTime = false;
		}

		private void _licenseInfoElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			string arguments = "http://www.visifire.com/license.php";
			Process.Start("explorer.exe", arguments);
		}

		private void _bannerDisplayTimer_Tick(object sender, EventArgs e)
		{
			if (this._bannerDisplayCounter == 10)
			{
				this._bannerDisplayCounter = 1;
				base.Dispatcher.BeginInvoke(new Action(delegate
				{
					this._bannerDisplayTimer.Stop();
					this._bannerDisplayTimer.Tick -= new EventHandler(this._bannerDisplayTimer_Tick);
					this.ShowLicenseInfo();
				}), new object[0]);
				return;
			}
			this._bannerDisplayCounter++;
		}

		private void ApplyUpAnimation(FrameworkElement target)
		{
			this._st = new Storyboard();
			DoubleAnimation doubleAnimation = new DoubleAnimation();
			string text = target.GetType().Name + Guid.NewGuid().ToString().Replace('-', '_');
			target.SetValue(FrameworkElement.NameProperty, text);
			Storyboard.SetTargetName(doubleAnimation, target.GetValue(FrameworkElement.NameProperty).ToString());
			this._rootElement.RegisterName(text, target);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(UIElement.OpacityProperty));
			doubleAnimation.BeginTime = new TimeSpan?(TimeSpan.FromSeconds(0.0));
			doubleAnimation.SpeedRatio = 1.0;
			doubleAnimation.From = new double?(0.8);
			doubleAnimation.To = new double?(0.0);
			this._st.Children.Add(doubleAnimation);
			this._st.Completed += delegate(object sender, EventArgs e)
			{
				this._licenseInfoElement.Visibility = Visibility.Collapsed;
			};
			this._st.Begin(this._rootElement, true);
		}

		private void ApplyDownAnimation(FrameworkElement target)
		{
			this._st = new Storyboard();
			DoubleAnimation doubleAnimation = new DoubleAnimation();
			string text = target.GetType().Name + Guid.NewGuid().ToString().Replace('-', '_');
			target.SetValue(FrameworkElement.NameProperty, text);
			Storyboard.SetTargetName(doubleAnimation, target.GetValue(FrameworkElement.NameProperty).ToString());
			this._rootElement.RegisterName(text, target);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(UIElement.OpacityProperty));
			doubleAnimation.BeginTime = new TimeSpan?(TimeSpan.FromSeconds(0.0));
			doubleAnimation.SpeedRatio = 1.0;
			doubleAnimation.From = new double?(0.0);
			doubleAnimation.To = new double?(0.8);
			this._st.Children.Add(doubleAnimation);
			this._st.Begin(this._rootElement, true);
			this._animationTimer.Tick += new EventHandler(this._timer_Tick);
			this._animationTimer.Interval = TimeSpan.FromSeconds(1.0);
			this._animationTimer.Start();
		}

		private void _timer_Tick(object sender, EventArgs e)
		{
			if (this._licenseInfoCounter == 0)
			{
				this._animationTimer.Stop();
				this._animationTimer.Tick -= new EventHandler(this._timer_Tick);
				this._licenseInfoCounter = 3;
				this.ApplyUpAnimation(this._licenseInfoElement);
				return;
			}
			this._licenseInfoCounter--;
		}

		public Chart()
		{
			this.Init();
			base.Loaded += new RoutedEventHandler(this.Chart_Loaded);
			base.SizeChanged += new SizeChangedEventHandler(this.Chart_SizeChanged);
			base.LayoutUpdated += new EventHandler(this.Chart_LayoutUpdated);
			base.Unloaded += new RoutedEventHandler(this.Chart_Unloaded);
		}

		static Chart()
		{
			Chart.ZoomingModeProperty = DependencyProperty.Register("ZoomingMode", typeof(ZoomingMode), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnZoomingModePropertyChanged)));
			Chart.PanningModeProperty = DependencyProperty.Register("PanningMode", typeof(PanningMode), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnPanningModePropertyChanged)));
			Chart.SamplingThresholdProperty = DependencyProperty.Register("SamplingThreshold", typeof(int), typeof(Chart), new PropertyMetadata(0, new PropertyChangedCallback(Chart.OnSamplingThresholdPropertyChanged)));
			Chart.ThemeEnabledProperty = DependencyProperty.Register("ThemeEnabled", typeof(bool), typeof(Chart), new PropertyMetadata(true, new PropertyChangedCallback(Chart.OnThemeEnabledPropertyChanged)));
			Chart.ZoomingEnabledProperty = DependencyProperty.Register("ZoomingEnabled", typeof(bool), typeof(Chart), new PropertyMetadata(false, new PropertyChangedCallback(Chart.OnZoomingEnabledPropertyChanged)));
			Chart.IndicatorEnabledProperty = DependencyProperty.Register("IndicatorEnabled", typeof(bool), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnIndicatorEnabledPropertyChanged)));
			Chart.IndicatorLineColorProperty = DependencyProperty.Register("IndicatorLineColor", typeof(Brush), typeof(Chart), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(153, 153, 153, 153)), new PropertyChangedCallback(Chart.OnIndicatorLineColorPropertyChanged)));
			Chart.AxisIndicatorBackgroundProperty = DependencyProperty.Register("AxisIndicatorBackground", typeof(Brush), typeof(Chart), new PropertyMetadata(new SolidColorBrush(Colors.LightGray), new PropertyChangedCallback(Chart.OnAxisIndicatorBackgroundPropertyChanged)));
			Chart.AxisIndicatorFontSizeProperty = DependencyProperty.Register("AxisIndicatorFontSize", typeof(double), typeof(Chart), new PropertyMetadata(11.0, new PropertyChangedCallback(Chart.OnAxisIndicatorFontSizePropertyPropertyChanged)));
			Chart.AxisIndicatorFontColorProperty = DependencyProperty.Register("AxisIndicatorFontColor", typeof(Brush), typeof(Chart), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(Chart.OnAxisIndicatorFontColorPropertyChanged)));
			Chart.SmartLabelEnabledProperty = DependencyProperty.Register("SmartLabelEnabled", typeof(bool), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnSmartLabelEnabledPropertyChanged)));
			Chart.DataPointWidthProperty = DependencyProperty.Register("DataPointWidth", typeof(double), typeof(Chart), new PropertyMetadata(double.NaN, new PropertyChangedCallback(Chart.OnDataPointWidthPropertyChanged)));
			Chart.MaxDataPointWidthProperty = DependencyProperty.Register("MaxDataPointWidth", typeof(double), typeof(Chart), new PropertyMetadata(double.PositiveInfinity, new PropertyChangedCallback(Chart.OnMaxDataPointWidthPropertyChanged)));
			Chart.UniqueColorsProperty = DependencyProperty.Register("UniqueColors", typeof(bool), typeof(Chart), new PropertyMetadata(true, new PropertyChangedCallback(Chart.OnUniqueColorsPropertyChanged)));
			Chart.ScrollingEnabledProperty = DependencyProperty.Register("ScrollingEnabled", typeof(bool?), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnScrollingEnabledPropertyChanged)));
			Chart.View3DProperty = DependencyProperty.Register("View3D", typeof(bool), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnView3DPropertyChanged)));
			Chart.HrefTargetProperty = DependencyProperty.Register("HrefTarget", typeof(HrefTargets), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnHrefTargetChanged)));
			Chart.HrefProperty = DependencyProperty.Register("Href", typeof(string), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnHrefChanged)));
			Chart.ThemeProperty = DependencyProperty.Register("Theme", typeof(string), typeof(Chart), new PropertyMetadata("Theme1", new PropertyChangedCallback(Chart.OnThemePropertyChanged)));
			Chart.AnimationEnabledProperty = DependencyProperty.Register("AnimationEnabled", typeof(bool?), typeof(Chart), null);
			Chart.AnimatedUpdateProperty = DependencyProperty.Register("AnimatedUpdate", typeof(bool?), typeof(Chart), null);
			Chart.InternalBorderThicknessProperty = DependencyProperty.Register("InternalBorderThickness", typeof(Thickness), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnInternalBorderThicknessChanged)));
			Chart.InternalBackgroundProperty = DependencyProperty.Register("InternalBackground", typeof(Brush), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnInternalBackgroundPropertyChanged)));
			Chart.BevelProperty = DependencyProperty.Register("Bevel", typeof(bool), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnBevelPropertyChanged)));
			Chart.ColorSetProperty = DependencyProperty.Register("ColorSet", typeof(string), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnColorSetPropertyChanged)));
			Chart.ColorSetsProperty = DependencyProperty.Register("ColorSets", typeof(ColorSets), typeof(Chart), new PropertyMetadata(new ColorSets(), null));
			Chart.LightingEnabledProperty = DependencyProperty.Register("LightingEnabled", typeof(bool), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnLightingEnabledPropertyChanged)));
			Chart.CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Chart), null);
			Chart.ShadowEnabledProperty = DependencyProperty.Register("ShadowEnabled", typeof(bool), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnShadowEnabledPropertyChanged)));
			Chart.PlotAreaProperty = DependencyProperty.Register("PlotArea", typeof(PlotArea), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnPlotAreaPropertyChanged)));
			Chart.MinimumGapProperty = DependencyProperty.Register("MinimumGap", typeof(double?), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnMinimumGapPropertyChanged)));
			if (!Chart._defaultStyleKeyApplied)
			{
				FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Chart), new FrameworkPropertyMetadata(typeof(Chart)));
				Chart._defaultStyleKeyApplied = true;
			}
		}

		private void Chart_Unloaded(object sender, RoutedEventArgs e)
		{
			if (base.Parent != null)
			{
				return;
			}
			if (this.InternalSeries != null)
			{
				foreach (DataSeries current in this.InternalSeries)
				{
					foreach (IDataPoint current2 in current.InternalDataPoints)
					{
						DataPointHelper.ClearInstanceRefs(current2);
					}
					current.ClearInstanceRefs();
				}
			}
			if (this.ChartArea != null)
			{
				this.ChartArea.ClearInstanceRefs();
			}
			foreach (Axis current3 in this.AxesX)
			{
				current3.ClearInstanceRefs();
			}
			foreach (Axis current4 in this.AxesY)
			{
				current4.ClearInstanceRefs();
			}
			if (this._rootElement != null)
			{
				this._rootElement.IsHitTestVisible = true;
			}
		}

		private void Chart_LayoutUpdated(object sender, EventArgs e)
		{
			if (this._currentVisibility == Visibility.Collapsed && base.Visibility == Visibility.Visible)
			{
				if (base.IsInDesignMode)
				{
					this.InvokeRender();
				}
				else
				{
					this.Render();
				}
			}
			this._currentVisibility = base.Visibility;
		}

		private void Chart_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!double.IsNaN(e.NewSize.Width) && !double.IsNaN(e.NewSize.Height) && e.NewSize.Width != 0.0 && e.NewSize.Height != 0.0)
			{
				if (base.IsInDesignMode)
				{
					this.InvokeRender();
					return;
				}
				this.Render();
			}
		}

		private void Chart_Loaded(object sender, RoutedEventArgs e)
		{
			if (base.IsInDesignMode)
			{
				this.InvokeRender();
				return;
			}
			this.Render();
		}
	}
}
