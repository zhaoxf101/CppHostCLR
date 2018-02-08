using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Visifire.Commons;
using Visifire.Commons.Controls;

namespace Visifire.Charts
{
	public class Axis : ObservableObject, IElement
	{
		internal class ZoomState
		{
			public object MinXValue;

			public object MaxXValue;

			public double NumericMinXValue;

			public double NumericMaxXValue;

			public ZoomState(object minValue, object maxValue)
			{
				this.MinXValue = minValue;
				this.MaxXValue = maxValue;
			}
		}

		internal const string RootElementName = "RootElement";

		private const double INNER_MARGIN = 5.0;

		private const double LABEL_INNER_MARGIN = 4.0;

		public static DependencyProperty ViewportRangeEnabledProperty;

		public static DependencyProperty AxisLabelsProperty;

		public static readonly DependencyProperty HrefTargetProperty;

		public static readonly DependencyProperty HrefProperty;

		public new static readonly DependencyProperty MaxHeightProperty;

		public new static readonly DependencyProperty MinHeightProperty;

		public new static readonly DependencyProperty MinWidthProperty;

		public new static readonly DependencyProperty MaxWidthProperty;

		public new static readonly DependencyProperty PaddingProperty;

		public new static readonly DependencyProperty BackgroundProperty;

		public new static readonly DependencyProperty OpacityProperty;

		public static readonly DependencyProperty IntervalProperty;

		public static readonly DependencyProperty LineColorProperty;

		public static readonly DependencyProperty LineThicknessProperty;

		public static readonly DependencyProperty LineStyleProperty;

		public static readonly DependencyProperty TitleProperty;

		public static readonly DependencyProperty TitleFontColorProperty;

		public static readonly DependencyProperty TitleFontFamilyProperty;

		public static readonly DependencyProperty TitleFontSizeProperty;

		public static readonly DependencyProperty TitleFontStyleProperty;

		public static readonly DependencyProperty TitleFontWeightProperty;

		public static readonly DependencyProperty TitleTextDecorationsProperty;

		public static readonly DependencyProperty AxisTypeProperty;

		public static readonly DependencyProperty AxisMaximumProperty;

		public static readonly DependencyProperty AxisMinimumProperty;

		public static readonly DependencyProperty IncludeZeroProperty;

		public static readonly DependencyProperty StartFromZeroProperty;

		public static readonly DependencyProperty PrefixProperty;

		public static readonly DependencyProperty SuffixProperty;

		public static readonly DependencyProperty ScalingSetProperty;

		public static readonly DependencyProperty ValueFormatStringProperty;

		public static readonly DependencyProperty ScrollBarOffsetProperty;

		public static readonly DependencyProperty ScrollBarScaleProperty;

		public static readonly DependencyProperty ClosestPlotDistanceProperty;

		public static readonly DependencyProperty ScrollBarSizeProperty;

		public static readonly DependencyProperty EnabledProperty;

		public static readonly DependencyProperty DisplayAutoAxisLabelsProperty;

		public static readonly DependencyProperty IntervalTypeProperty;

		public static readonly DependencyProperty LogarithmicProperty;

		public static readonly DependencyProperty LogarithmBaseProperty;

		public static readonly DependencyProperty AxisOffsetProperty;

		internal double AxisMinimumNumeric = double.NaN;

		internal DateTime AxisMinimumDateTime;

		internal double AxisMaximumNumeric = double.NaN;

		internal DateTime AxisMaximumDateTime;

		internal double _oldInternalAxisMinimum;

		internal double _oldInternalAxisMaximum;

		private new static DependencyProperty FontFamilyProperty;

		private new static DependencyProperty FontSizeProperty;

		private new static DependencyProperty FontStretchProperty;

		private new static DependencyProperty FontStyleProperty;

		private new static DependencyProperty FontWeightProperty;

		private new static DependencyProperty ForegroundProperty;

		private new static DependencyProperty BorderThicknessProperty;

		private new static DependencyProperty BorderBrushProperty;

		private bool _dragCompletedLock;

		private bool _renderAfterDrag;

		internal Canvas _rootElement;

		internal Axis.ZoomState _oldZoomState = new Axis.ZoomState(null, null);

		internal Axis.ZoomState _zoomState = new Axis.ZoomState(null, null);

		internal Axis.ZoomState _initialState = new Axis.ZoomState(null, null);

		internal double _zeroBaseLinePixPosition;

		private double _oldScrollBarScale = double.NaN;

		internal ChartValueTypes _axisMinimumValueType;

		internal ChartValueTypes _axisMaximumValueType;

		internal bool _isScrollToOffsetEnabled = true;

		internal bool _isDateTimeAutoInterval;

		internal bool _isAllXValueZero = true;

		private AxisOrientation _orientation;

		private List<string> _scaleUnits;

		private List<double> _scaleValues;

		private double _axisIntervalOverride;

		internal double _oldScrollBarOffsetInPixel = double.NaN;

		internal Stack<Axis.ZoomState> _zoomStateStack = new Stack<Axis.ZoomState>();

		internal double _numericViewMinimum;

		internal double _numericViewMaximum;

		private bool _isScrollEventFiredFirstTime = true;

		private double _internalOpacity = double.NaN;

		private Brush _internalBackground;

		private Thickness? _internalPadding = null;

		private double _internalMaxHeight = double.NaN;

		private double _internalMaxWidth = double.NaN;

		private double _internalMinHeight = double.NaN;

		private double _internalMinWidth = double.NaN;

		internal double _internalZoomingScale;

		internal double _internalOldZoomBarValue;

		internal double _internalOldZoomingScale = double.NaN;

		internal static readonly double INTERNAL_MINIMUM_ZOOMING_SCALE;

		internal bool _showAllState;

		private static bool _defaultStyleKeyApplied;

		internal event EventHandler ZoomingScaleChanged;

		internal event ScrollEventHandler ScrollBarOffsetChanged;

		public event EventHandler<AxisScrollEventArgs> Scroll
		{
			add
			{
				this._onScroll += value;
			}
			remove
			{
				this._onScroll -= value;
			}
		}

		public event EventHandler<AxisZoomEventArgs> OnZoom
		{
			add
			{
				this._onZoom += value;
			}
			remove
			{
				this._onZoom -= value;
			}
		}

		private event EventHandler<AxisScrollEventArgs> _onScroll;

		internal event EventHandler<AxisZoomEventArgs> _onZoom;

		public double AxisOffset
		{
			get
			{
				return (double)base.GetValue(Axis.AxisOffsetProperty);
			}
			set
			{
				base.SetValue(Axis.AxisOffsetProperty, value);
			}
		}

		public object ViewMaximum
		{
			get;
			internal set;
		}

		public object ViewMinimum
		{
			get;
			internal set;
		}

		public new double Height
		{
			get;
			internal set;
		}

		public new double Width
		{
			get;
			internal set;
		}

		public new double MaxHeight
		{
			get
			{
				return (double)base.GetValue(Axis.MaxHeightProperty);
			}
			set
			{
				base.SetValue(Axis.MaxHeightProperty, value);
			}
		}

		public new double MaxWidth
		{
			get
			{
				return (double)base.GetValue(Axis.MaxWidthProperty);
			}
			set
			{
				base.SetValue(Axis.MaxWidthProperty, value);
			}
		}

		public new double MinHeight
		{
			get
			{
				return (double)base.GetValue(Axis.MinHeightProperty);
			}
			set
			{
				base.SetValue(Axis.MinHeightProperty, value);
			}
		}

		public new double MinWidth
		{
			get
			{
				return (double)base.GetValue(Axis.MinWidthProperty);
			}
			set
			{
				base.SetValue(Axis.MinWidthProperty, value);
			}
		}

		public bool Logarithmic
		{
			get
			{
				return (bool)base.GetValue(Axis.LogarithmicProperty);
			}
			set
			{
				base.SetValue(Axis.LogarithmicProperty, value);
			}
		}

		public double LogarithmBase
		{
			get
			{
				return (double)base.GetValue(Axis.LogarithmBaseProperty);
			}
			set
			{
				base.SetValue(Axis.LogarithmBaseProperty, value);
			}
		}

		public IntervalTypes IntervalType
		{
			get
			{
				return (IntervalTypes)base.GetValue(Axis.IntervalTypeProperty);
			}
			set
			{
				base.SetValue(Axis.IntervalTypeProperty, value);
			}
		}

		public bool ViewportRangeEnabled
		{
			get
			{
				return (bool)base.GetValue(Axis.ViewportRangeEnabledProperty);
			}
			set
			{
				base.SetValue(Axis.ViewportRangeEnabledProperty, value);
			}
		}

		public AxisLabels AxisLabels
		{
			get
			{
				return (AxisLabels)base.GetValue(Axis.AxisLabelsProperty);
			}
			set
			{
				base.SetValue(Axis.AxisLabelsProperty, value);
			}
		}

		public new double Opacity
		{
			get
			{
				return (double)base.GetValue(Axis.OpacityProperty);
			}
			set
			{
				base.SetValue(Axis.OpacityProperty, value);
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
					base.FirePropertyChanged(VcProperties.Cursor);
				}
			}
		}

		public HrefTargets HrefTarget
		{
			get
			{
				return (HrefTargets)base.GetValue(Axis.HrefTargetProperty);
			}
			set
			{
				base.SetValue(Axis.HrefTargetProperty, value);
			}
		}

		public string Href
		{
			get
			{
				return (string)base.GetValue(Axis.HrefProperty);
			}
			set
			{
				base.SetValue(Axis.HrefProperty, value);
			}
		}

		public new Brush Background
		{
			get
			{
				return (Brush)base.GetValue(Axis.BackgroundProperty);
			}
			set
			{
				base.SetValue(Axis.BackgroundProperty, value);
			}
		}

		public double? Interval
		{
			get
			{
				if (!((double?)base.GetValue(Axis.IntervalProperty)).HasValue)
				{
					return new double?(double.NaN);
				}
				return (double?)base.GetValue(Axis.IntervalProperty);
			}
			set
			{
				base.SetValue(Axis.IntervalProperty, value);
				this._axisIntervalOverride = value.Value;
			}
		}

		public double ActualInterval
		{
			get
			{
				return this.InternalInterval;
			}
		}

		public IntervalTypes ActualIntervalType
		{
			get
			{
				return this.InternalIntervalType;
			}
		}

		public Brush LineColor
		{
			get
			{
				return (Brush)base.GetValue(Axis.LineColorProperty);
			}
			set
			{
				base.SetValue(Axis.LineColorProperty, value);
			}
		}

		public double? LineThickness
		{
			get
			{
				if (base.GetValue(Axis.LineThicknessProperty) != null)
				{
					return new double?((double)base.GetValue(Axis.LineThicknessProperty));
				}
				if (base.Chart != null && (base.Chart as Chart).PlotDetails != null && (base.Chart as Chart).PlotDetails.ChartOrientation == ChartOrientationType.Circular)
				{
					return new double?(1.0);
				}
				if (base.Chart != null && ((base.Chart as Chart).Theme == "Theme4" || (base.Chart as Chart).Theme == "Theme5"))
				{
					return new double?(1.0);
				}
				return new double?(0.5);
			}
			set
			{
				base.SetValue(Axis.LineThicknessProperty, value);
			}
		}

		public LineStyles LineStyle
		{
			get
			{
				return (LineStyles)base.GetValue(Axis.LineStyleProperty);
			}
			set
			{
				base.SetValue(Axis.LineStyleProperty, value);
			}
		}

		public new Thickness Padding
		{
			get
			{
				return (Thickness)base.GetValue(Axis.PaddingProperty);
			}
			set
			{
				base.SetValue(Axis.PaddingProperty, value);
			}
		}

		public string Title
		{
			get
			{
				return (string)base.GetValue(Axis.TitleProperty);
			}
			set
			{
				base.SetValue(Axis.TitleProperty, value);
			}
		}

		public Brush TitleFontColor
		{
			get
			{
				return (Brush)base.GetValue(Axis.TitleFontColorProperty);
			}
			set
			{
				base.SetValue(Axis.TitleFontColorProperty, value);
			}
		}

		public FontFamily TitleFontFamily
		{
			get
			{
				return (FontFamily)base.GetValue(Axis.TitleFontFamilyProperty);
			}
			set
			{
				base.SetValue(Axis.TitleFontFamilyProperty, value);
			}
		}

		public double TitleFontSize
		{
			get
			{
				return (double)base.GetValue(Axis.TitleFontSizeProperty);
			}
			set
			{
				base.SetValue(Axis.TitleFontSizeProperty, value);
			}
		}

		public FontStyle TitleFontStyle
		{
			get
			{
				return (FontStyle)base.GetValue(Axis.TitleFontStyleProperty);
			}
			set
			{
				base.SetValue(Axis.TitleFontStyleProperty, value);
			}
		}

		public FontWeight TitleFontWeight
		{
			get
			{
				return (FontWeight)base.GetValue(Axis.TitleFontWeightProperty);
			}
			set
			{
				base.SetValue(Axis.TitleFontWeightProperty, value);
			}
		}

		public TextDecorationCollection TitleTextDecorations
		{
			get
			{
				return (TextDecorationCollection)base.GetValue(Axis.TitleTextDecorationsProperty);
			}
			set
			{
				base.SetValue(Axis.TitleTextDecorationsProperty, value);
			}
		}

		public AxisTypes AxisType
		{
			get
			{
				return (AxisTypes)base.GetValue(Axis.AxisTypeProperty);
			}
			set
			{
				base.SetValue(Axis.AxisTypeProperty, value);
			}
		}

		public object AxisMaximum
		{
			get
			{
				return base.GetValue(Axis.AxisMaximumProperty);
			}
			set
			{
				base.SetValue(Axis.AxisMaximumProperty, value);
			}
		}

		public object AxisMinimum
		{
			get
			{
				return base.GetValue(Axis.AxisMinimumProperty);
			}
			set
			{
				base.SetValue(Axis.AxisMinimumProperty, value);
			}
		}

		public bool IncludeZero
		{
			get
			{
				return (bool)base.GetValue(Axis.IncludeZeroProperty);
			}
			set
			{
				base.SetValue(Axis.IncludeZeroProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? StartFromZero
		{
			get
			{
				if (((bool?)base.GetValue(Axis.StartFromZeroProperty)).HasValue)
				{
					return (bool?)base.GetValue(Axis.StartFromZeroProperty);
				}
				if (this.AxisRepresentation == AxisRepresentations.AxisY)
				{
					if (this.ViewportRangeEnabled)
					{
						return new bool?(false);
					}
					return new bool?(true);
				}
				else
				{
					if (this.AxisOrientation == AxisOrientation.Circular)
					{
						return new bool?(true);
					}
					return new bool?(false);
				}
			}
			set
			{
				base.SetValue(Axis.StartFromZeroProperty, value);
			}
		}

		public string Prefix
		{
			get
			{
				return (string)base.GetValue(Axis.PrefixProperty);
			}
			set
			{
				base.SetValue(Axis.PrefixProperty, value);
			}
		}

		public string Suffix
		{
			get
			{
				return (string)base.GetValue(Axis.SuffixProperty);
			}
			set
			{
				base.SetValue(Axis.SuffixProperty, value);
			}
		}

		public string ScalingSet
		{
			get
			{
				return (string)base.GetValue(Axis.ScalingSetProperty);
			}
			set
			{
				base.SetValue(Axis.ScalingSetProperty, value);
			}
		}

		public string ValueFormatString
		{
			get
			{
				if (!string.IsNullOrEmpty((string)base.GetValue(Axis.ValueFormatStringProperty)))
				{
					return (string)base.GetValue(Axis.ValueFormatStringProperty);
				}
				return "###,##0.##";
			}
			set
			{
				base.SetValue(Axis.ValueFormatStringProperty, value);
			}
		}

		public double ScrollBarOffset
		{
			get
			{
				return (double)base.GetValue(Axis.ScrollBarOffsetProperty);
			}
			set
			{
				if (value < 0.0 || value > 1.0)
				{
					throw new Exception("Value does not fall under the expected range. ScrollBarOffset always varies from 0 to 1.");
				}
				base.SetValue(Axis.ScrollBarOffsetProperty, value);
			}
		}

		public double ScrollBarScale
		{
			get
			{
				return (double)base.GetValue(Axis.ScrollBarScaleProperty);
			}
			set
			{
				if (value <= 0.0 || value > 1.0)
				{
					throw new Exception("Value does not fall under the expected range. ScrollBarScale always varies from 0 to 1.");
				}
				base.SetValue(Axis.ScrollBarScaleProperty, value);
			}
		}

		public double ClosestPlotDistance
		{
			get
			{
				return (double)base.GetValue(Axis.ClosestPlotDistanceProperty);
			}
			set
			{
				base.SetValue(Axis.ClosestPlotDistanceProperty, value);
			}
		}

		public double ScrollBarSize
		{
			get
			{
				return (double)base.GetValue(Axis.ScrollBarSizeProperty);
			}
			set
			{
				base.SetValue(Axis.ScrollBarSizeProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Enabled
		{
			get
			{
				if (!((bool?)base.GetValue(Axis.EnabledProperty)).HasValue)
				{
					return new bool?(true);
				}
				return (bool?)base.GetValue(Axis.EnabledProperty);
			}
			set
			{
				base.SetValue(Axis.EnabledProperty, value);
			}
		}

		public bool DisplayAutoAxisLabels
		{
			get
			{
				return (bool)base.GetValue(Axis.DisplayAutoAxisLabelsProperty);
			}
			set
			{
				base.SetValue(Axis.DisplayAutoAxisLabelsProperty, value);
			}
		}

		public CustomAxisLabelsCollection CustomAxisLabels
		{
			get;
			set;
		}

		public ChartGridCollection Grids
		{
			get;
			set;
		}

		public TicksCollection Ticks
		{
			get;
			set;
		}

		internal double InternalMaxHeight
		{
			get
			{
				return (double)(double.IsNaN(this._internalMaxHeight) ? base.GetValue(Axis.MaxHeightProperty) : this._internalMaxHeight);
			}
			set
			{
				this._internalMaxHeight = value;
			}
		}

		internal double InternalMaxWidth
		{
			get
			{
				return (double)(double.IsNaN(this._internalMaxWidth) ? base.GetValue(Axis.MaxWidthProperty) : this._internalMaxWidth);
			}
			set
			{
				this._internalMaxWidth = value;
			}
		}

		internal double InternalMinHeight
		{
			get
			{
				return (double)(double.IsNaN(this._internalMinHeight) ? base.GetValue(Axis.MinHeightProperty) : this._internalMinHeight);
			}
			set
			{
				this._internalMinHeight = value;
			}
		}

		internal double InternalMinWidth
		{
			get
			{
				return (double)(double.IsNaN(this._internalMinWidth) ? base.GetValue(Axis.MinWidthProperty) : this._internalMinWidth);
			}
			set
			{
				this._internalMinWidth = value;
			}
		}

		internal Brush InternalBackground
		{
			get
			{
				return (Brush)((this._internalBackground == null) ? base.GetValue(Axis.BackgroundProperty) : this._internalBackground);
			}
			set
			{
				this._internalBackground = value;
			}
		}

		internal Thickness InternalPadding
		{
			get
			{
				return (Thickness)((!this._internalPadding.HasValue) ? base.GetValue(Axis.PaddingProperty) : this._internalPadding);
			}
			set
			{
				this._internalPadding = new Thickness?(value);
			}
		}

		internal double InternalOpacity
		{
			get
			{
				return (double)(double.IsNaN(this._internalOpacity) ? base.GetValue(Axis.OpacityProperty) : this._internalOpacity);
			}
			set
			{
				this._internalOpacity = value;
			}
		}

		internal IntervalTypes InternalIntervalType
		{
			get;
			set;
		}

		internal bool IsDateTimeAxis
		{
			get;
			set;
		}

		internal ChartValueTypes XValueType
		{
			get;
			set;
		}

		internal DateTime MinDate
		{
			get;
			set;
		}

		internal DateTime MaxDate
		{
			get;
			set;
		}

		internal TimeSpan MinDateRange
		{
			get;
			set;
		}

		internal TimeSpan MaxDateRange
		{
			get;
			set;
		}

		internal StackPanel Visual
		{
			get;
			set;
		}

		internal StackPanel AxisElementsContainer
		{
			get;
			set;
		}

		public object ActualAxisMinimum
		{
			get
			{
				if (this.IsDateTimeAxis)
				{
					if (base.Chart != null && (base.Chart as Chart).PlotDetails.ListOfAllDataPoints.Count != 0)
					{
						return DateTimeHelper.XValueToDateTime(this.MinDate, this.InternalAxisMinimum, this.InternalIntervalType);
					}
					return this.MinDate;
				}
				else
				{
					if (this.AxisRepresentation == AxisRepresentations.AxisY && this.Logarithmic)
					{
						return DataPointHelper.ConvertLogarithmicValue2ActualValue(base.Chart as Chart, this.InternalAxisMinimum, this.AxisType);
					}
					return this.InternalAxisMinimum;
				}
			}
		}

		public object ActualAxisMaximum
		{
			get
			{
				if (this.IsDateTimeAxis)
				{
					if (base.Chart != null && (base.Chart as Chart).PlotDetails.ListOfAllDataPoints.Count != 0)
					{
						return DateTimeHelper.XValueToDateTime(this.MinDate, this.InternalAxisMaximum, this.InternalIntervalType);
					}
					return this.MaxDate;
				}
				else
				{
					if (this.AxisRepresentation == AxisRepresentations.AxisY && this.Logarithmic)
					{
						return DataPointHelper.ConvertLogarithmicValue2ActualValue(base.Chart as Chart, this.InternalAxisMaximum, this.AxisType);
					}
					return this.InternalAxisMaximum;
				}
			}
		}

		public double ScrollableLength
		{
			get
			{
				if (this.AxisRepresentation == AxisRepresentations.AxisX)
				{
					return this.ScrollableSize;
				}
				return double.NaN;
			}
		}

		internal double InternalAxisMinimum
		{
			get;
			private set;
		}

		internal double InternalAxisMaximum
		{
			get;
			private set;
		}

		internal double InternalInterval
		{
			get;
			set;
		}

		internal double CurrentScrollScrollBarOffset
		{
			get;
			set;
		}

		internal double Maximum
		{
			get;
			set;
		}

		internal double Minimum
		{
			get;
			set;
		}

		internal ChartGrid MajorGridsElement
		{
			get;
			set;
		}

		public ZoomBar ScrollBarElement
		{
			get;
			set;
		}

		internal Canvas ScrollViewerElement
		{
			get;
			set;
		}

		internal Ticks MajorTicksElement
		{
			get;
			set;
		}

		internal Title AxisTitleElement
		{
			get;
			set;
		}

		internal AxisManager AxisManager
		{
			get;
			set;
		}

		internal double ScrollableSize
		{
			get;
			set;
		}

		internal Line AxisLine
		{
			get;
			set;
		}

		internal AxisOrientation AxisOrientation
		{
			get
			{
				return this._orientation;
			}
			set
			{
				this._orientation = value;
			}
		}

		internal AxisRepresentations AxisRepresentation
		{
			get;
			set;
		}

		internal PlotDetails PlotDetails
		{
			get;
			set;
		}

		internal List<double> ScaleValues
		{
			get
			{
				return this._scaleValues;
			}
		}

		internal List<string> ScaleUnits
		{
			get
			{
				return this._scaleUnits;
			}
		}

		internal double StartOffset
		{
			get;
			set;
		}

		internal double EndOffset
		{
			get;
			set;
		}

		internal int SkipOffset
		{
			get;
			set;
		}

		internal int SkipOffset4DateTime
		{
			get;
			set;
		}

		private Canvas InternalStackPanel
		{
			get;
			set;
		}

		private new FontFamily FontFamily
		{
			get;
			set;
		}

		private new double FontSize
		{
			get;
			set;
		}

		private new FontStretch FontStretch
		{
			get;
			set;
		}

		private new FontStyle FontStyle
		{
			get;
			set;
		}

		private new FontWeight FontWeight
		{
			get;
			set;
		}

		private new Brush Foreground
		{
			get;
			set;
		}

		private new Thickness BorderThickness
		{
			get;
			set;
		}

		private new Brush BorderBrush
		{
			get;
			set;
		}

		internal double FirstLabelPosition
		{
			get;
			set;
		}

		internal DateTime FirstLabelDate
		{
			get;
			set;
		}

		internal Canvas CircularAxisVisual
		{
			get;
			set;
		}

		internal Path CircularPath
		{
			get;
			set;
		}

		internal CircularPlotDetails CircularPlotDetails
		{
			get;
			set;
		}

		internal Storyboard Storyboard
		{
			get;
			set;
		}

		public Axis()
		{
			this.Grids = new ChartGridCollection();
			this.Ticks = new TicksCollection();
			this.CustomAxisLabels = new CustomAxisLabelsCollection();
			this.Grids.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Grids_CollectionChanged);
			this.Ticks.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Ticks_CollectionChanged);
			this.CustomAxisLabels.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CustomLabels_CollectionChanged);
			this.InternalAxisMinimum = double.NaN;
			this.InternalAxisMaximum = double.NaN;
		}

		static Axis()
		{
			Axis.ViewportRangeEnabledProperty = DependencyProperty.Register("ViewportRangeEnabled", typeof(bool), typeof(Axis), new PropertyMetadata(false, new PropertyChangedCallback(Axis.OnViewportRangeEnabledPropertyChanged)));
			Axis.AxisLabelsProperty = DependencyProperty.Register("AxisLabels", typeof(AxisLabels), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnAxisLabelsPropertyChanged)));
			Axis.HrefTargetProperty = DependencyProperty.Register("HrefTarget", typeof(HrefTargets), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnHrefTargetChanged)));
			Axis.HrefProperty = DependencyProperty.Register("Href", typeof(string), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnHrefChanged)));
			Axis.MaxHeightProperty = DependencyProperty.Register("MaxHeight", typeof(double), typeof(Axis), new PropertyMetadata(double.PositiveInfinity, new PropertyChangedCallback(Axis.OnMaxHeightPropertyChanged)));
			Axis.MinHeightProperty = DependencyProperty.Register("MinHeight", typeof(double), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnMinHeightPropertyChanged)));
			Axis.MinWidthProperty = DependencyProperty.Register("MinWidth", typeof(double), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnMinWidthPropertyChanged)));
			Axis.MaxWidthProperty = DependencyProperty.Register("MaxWidth", typeof(double), typeof(Axis), new PropertyMetadata(double.PositiveInfinity, new PropertyChangedCallback(Axis.OnMaxWidthPropertyChanged)));
			Axis.PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnPaddingPropertyChanged)));
			Axis.BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnBackgroundPropertyChanged)));
			Axis.OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(Axis), new PropertyMetadata(1.0, new PropertyChangedCallback(Axis.OnOpacityPropertyChanged)));
			Axis.IntervalProperty = DependencyProperty.Register("Interval", typeof(double?), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnIntervalPropertyChanged)));
			Axis.LineColorProperty = DependencyProperty.Register("LineColor", typeof(Brush), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnLineColorPropertyChanged)));
			Axis.LineThicknessProperty = DependencyProperty.Register("LineThickness", typeof(double?), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnLineThicknessPropertyChanged)));
			Axis.LineStyleProperty = DependencyProperty.Register("LineStyle", typeof(LineStyles), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnLineStylePropertyChanged)));
			Axis.TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnTitlePropertyChanged)));
			Axis.TitleFontColorProperty = DependencyProperty.Register("TitleFontColor", typeof(Brush), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnTitleFontColorPropertyChanged)));
			Axis.TitleFontFamilyProperty = DependencyProperty.Register("TitleFontFamily", typeof(FontFamily), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnTitleFontFamilyPropertyChanged)));
			Axis.TitleFontSizeProperty = DependencyProperty.Register("TitleFontSize", typeof(double), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnTitleFontSizePropertyChanged)));
			Axis.TitleFontStyleProperty = DependencyProperty.Register("TitleFontStyle", typeof(FontStyle), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnTitleFontStylePropertyChanged)));
			Axis.TitleFontWeightProperty = DependencyProperty.Register("TitleFontWeight", typeof(FontWeight), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnTitleFontWeightPropertyChanged)));
			Axis.TitleTextDecorationsProperty = DependencyProperty.Register("TitleTextDecorations", typeof(TextDecorationCollection), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnTitleTextDecorationsPropertyChanged)));
			Axis.AxisTypeProperty = DependencyProperty.Register("AxisType", typeof(AxisTypes), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnAxisTypePropertyChanged)));
			Axis.AxisMaximumProperty = DependencyProperty.Register("AxisMaximum", typeof(object), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnAxisMaximumPropertyChanged)));
			Axis.AxisMinimumProperty = DependencyProperty.Register("AxisMinimum", typeof(object), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnAxisMinimumPropertyChanged)));
			Axis.IncludeZeroProperty = DependencyProperty.Register("IncludeZero", typeof(bool), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnIncludeZeroPropertyChanged)));
			Axis.StartFromZeroProperty = DependencyProperty.Register("StartFromZero", typeof(bool?), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnStartFromZeroPropertyChanged)));
			Axis.PrefixProperty = DependencyProperty.Register("Prefix", typeof(string), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnPrefixPropertyChanged)));
			Axis.SuffixProperty = DependencyProperty.Register("Suffix", typeof(string), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnSuffixPropertyChanged)));
			Axis.ScalingSetProperty = DependencyProperty.Register("ScalingSet", typeof(string), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnScalingSetPropertyChanged)));
			Axis.ValueFormatStringProperty = DependencyProperty.Register("ValueFormatString", typeof(string), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnValueFormatStringPropertyChanged)));
			Axis.ScrollBarOffsetProperty = DependencyProperty.Register("ScrollBarOffset", typeof(double), typeof(Axis), new PropertyMetadata(double.NaN, new PropertyChangedCallback(Axis.OnScrollBarOffsetChanged)));
			Axis.ScrollBarScaleProperty = DependencyProperty.Register("ScrollBarScale", typeof(double), typeof(Axis), new PropertyMetadata(double.NaN, new PropertyChangedCallback(Axis.OnScrollBarScalePropertyChanged)));
			Axis.ClosestPlotDistanceProperty = DependencyProperty.Register("ClosestPlotDistance", typeof(double), typeof(Axis), new PropertyMetadata(double.NaN, new PropertyChangedCallback(Axis.OnClosestPlotDistancePropertyChanged)));
			Axis.ScrollBarSizeProperty = DependencyProperty.Register("ScrollBarSize", typeof(double), typeof(Axis), new PropertyMetadata(double.NaN, new PropertyChangedCallback(Axis.OnScrollBarSizeChanged)));
			Axis.EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool?), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnEnabledPropertyChanged)));
			Axis.DisplayAutoAxisLabelsProperty = DependencyProperty.Register("DisplayAutoAxisLabels", typeof(bool), typeof(Axis), new PropertyMetadata(true, new PropertyChangedCallback(Axis.OnDisplayAutoAxisLabelsPropertyChanged)));
			Axis.IntervalTypeProperty = DependencyProperty.Register("IntervalType", typeof(IntervalTypes), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnIntervalTypePropertyChanged)));
			Axis.LogarithmicProperty = DependencyProperty.Register("Logarithmic", typeof(bool), typeof(Axis), new PropertyMetadata(false, new PropertyChangedCallback(Axis.OnLogarithmicPropertyChanged)));
			Axis.LogarithmBaseProperty = DependencyProperty.Register("LogarithmBase", typeof(double), typeof(Axis), new PropertyMetadata(10.0, new PropertyChangedCallback(Axis.OnLogarithmBasePropertyChanged)));
			Axis.AxisOffsetProperty = DependencyProperty.Register("AxisOffSet", typeof(double), typeof(Axis), new PropertyMetadata(double.NaN, new PropertyChangedCallback(Axis.OnAxisOffsetPropertyChanged)));
			Axis.FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(Axis), null);
			Axis.FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(Axis), null);
			Axis.FontStretchProperty = DependencyProperty.Register("FontStretch", typeof(FontStretch), typeof(Axis), null);
			Axis.FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(Axis), null);
			Axis.FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(Axis), null);
			Axis.ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(Axis), null);
			Axis.BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(Axis), null);
			Axis.BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Axis), null);
			Axis.INTERNAL_MINIMUM_ZOOMING_SCALE = 1E-07;
			if (!Axis._defaultStyleKeyApplied)
			{
				FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Axis), new FrameworkPropertyMetadata(typeof(Axis)));
				Axis._defaultStyleKeyApplied = true;
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this._rootElement = (base.GetTemplateChild("RootElement") as Canvas);
			this.AddAxisLabelsIntoRootElement();
			foreach (CustomAxisLabels current in this.CustomAxisLabels)
			{
				this.AddCustomAxisLabelsToRootElement(current);
			}
			foreach (ChartGrid current2 in this.Grids)
			{
				this.AddGridsIntoRootElement(current2);
			}
			foreach (Ticks current3 in this.Ticks)
			{
				this.AddTickElementIntoRootElement(current3);
			}
		}

		private void AddCustomAxisLabelsToRootElement(CustomAxisLabels labels)
		{
			if (this._rootElement == null)
			{
				return;
			}
			labels.IsNotificationEnable = false;
			if (base.IsInDesignMode)
			{
				ObservableObject.RemoveElementFromElementTree(labels);
			}
			if (labels.ParentTree == null)
			{
				this._rootElement.Children.Add(labels);
			}
			labels.IsNotificationEnable = true;
			labels.IsTabStop = false;
		}

		private void AddTickElementIntoRootElement(Ticks ticks)
		{
			if (this._rootElement == null)
			{
				return;
			}
			ticks.IsNotificationEnable = false;
			if (base.IsInDesignMode)
			{
				ObservableObject.RemoveElementFromElementTree(ticks);
			}
			if (ticks.Parent == null)
			{
				this._rootElement.Children.Add(ticks);
			}
			ticks.IsNotificationEnable = true;
			ticks.IsTabStop = false;
		}

		private void AddGridsIntoRootElement(ChartGrid grid)
		{
			if (this._rootElement == null)
			{
				return;
			}
			grid.IsNotificationEnable = false;
			if (base.IsInDesignMode)
			{
				ObservableObject.RemoveElementFromElementTree(grid);
			}
			if (grid.Parent == null)
			{
				this._rootElement.Children.Add(grid);
			}
			grid.IsNotificationEnable = true;
			grid.IsTabStop = false;
		}

		private void AddAxisLabelsIntoRootElement()
		{
			if (this.AxisLabels != null && this._rootElement != null)
			{
				this.AxisLabels.IsNotificationEnable = false;
				if (base.IsInDesignMode)
				{
					ObservableObject.RemoveElementFromElementTree(this.AxisLabels);
				}
				if (this.AxisLabels.ParentTree == null)
				{
					this._rootElement.Children.Add(this.AxisLabels);
				}
				this.AxisLabels.IsNotificationEnable = true;
				this.AxisLabels.IsTabStop = false;
			}
		}

		internal override void Bind()
		{
		}

		public double XValueToScrollBarOffset(double xValue)
		{
			if (this.AxisRepresentation != AxisRepresentations.AxisX)
			{
				return double.NaN;
			}
			double num2;
			if (this.AxisOrientation == AxisOrientation.Horizontal)
			{
				double num = Graphics.ValueToPixelPosition(0.0, this.ScrollableSize, this.InternalAxisMinimum, this.InternalAxisMaximum, xValue);
				num2 = num / (this.ScrollableSize - this.Width);
			}
			else
			{
				double num3 = Graphics.ValueToPixelPosition(this.ScrollableSize, 0.0, this.InternalAxisMinimum, this.InternalAxisMaximum, xValue);
				num2 = num3 / (this.ScrollableSize - this.Height);
				num2 = 1.0 - num2;
			}
			if (num2 > 1.0)
			{
				return 1.0;
			}
			if (num2 >= 0.0)
			{
				return num2;
			}
			return 0.0;
		}

		internal double GetAxisVisualLeftAsOffset()
		{
			if (this.ScrollViewerElement != null && this.ScrollViewerElement.Children.Count > 0)
			{
				return (double)(this.ScrollViewerElement.Children[0] as Canvas).GetValue(Canvas.LeftProperty);
			}
			return double.NaN;
		}

		internal double XValueToInternalXValue(object xValue)
		{
			if (this.IsDateTimeAxis)
			{
				return DateTimeHelper.DateDiff(Convert.ToDateTime(xValue), this.MinDate, this.MinDateRange, this.MaxDateRange, this.InternalIntervalType, this.XValueType);
			}
			return Convert.ToDouble(xValue);
		}

		public double XValueToPixelXPosition(object xValue)
		{
			Chart chart = base.Chart as Chart;
			if (this.AxisRepresentation != AxisRepresentations.AxisX || chart == null || chart.ChartArea == null || this.AxisManager == null)
			{
				return double.NaN;
			}
			double pLANK_DEPTH = chart.ChartArea.PLANK_DEPTH;
			double value = this.XValueToInternalXValue(xValue);
			if (this.AxisOrientation != AxisOrientation.Horizontal)
			{
				double arg_54_0 = this.Height;
			}
			else
			{
				double arg_5D_0 = this.Width;
			}
			return Graphics.ValueToPixelPosition(0.0, this.ScrollableSize - pLANK_DEPTH, this.AxisManager.AxisMinimumValue, this.AxisManager.AxisMaximumValue, value);
		}

		private double CalculateZoomingScaleFromXValueRange(object minXValue, object maxXValue)
		{
			Chart chart = base.Chart as Chart;
			double num = this.XValueToInternalXValue(minXValue);
			double num2 = this.XValueToInternalXValue(maxXValue);
			chart.ChartArea.AxisX._numericViewMinimum = Math.Min(num2, num);
			chart.ChartArea.AxisX._numericViewMaximum = Math.Max(num2, num);
			double num3;
			double num4;
			if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				num3 = Graphics.ValueToPixelPosition(0.0, this.ScrollableSize, this.InternalAxisMinimum, this.InternalAxisMaximum, num);
				num4 = Graphics.ValueToPixelPosition(0.0, this.ScrollableSize, this.InternalAxisMinimum, this.InternalAxisMaximum, num2);
			}
			else
			{
				num3 = Graphics.ValueToPixelPosition(this.ScrollableSize, 0.0, this.InternalAxisMinimum, this.InternalAxisMaximum, num);
				num4 = Graphics.ValueToPixelPosition(this.ScrollableSize, 0.0, this.InternalAxisMinimum, this.InternalAxisMaximum, num2);
			}
			double num5 = this.ScrollableSize;
			double num6 = Math.Abs(num4 - num3);
			if (num5 < num6)
			{
				num5 = num6;
			}
			double viewPortSize;
			if (this.AxisOrientation == AxisOrientation.Horizontal)
			{
				viewPortSize = this.Width;
			}
			else
			{
				viewPortSize = this.Height;
			}
			double zoomingScale = num6 / num5;
			return this.ValidateAndUpdateMaxZoomingScale(zoomingScale, viewPortSize);
		}

		private double ValidateAndUpdateMaxZoomingScale(double zoomingScale, double viewPortSize)
		{
			if (viewPortSize / zoomingScale > ChartArea.MAX_PLOTAREA_SIZE)
			{
				zoomingScale = viewPortSize / ChartArea.MAX_PLOTAREA_SIZE;
			}
			return zoomingScale;
		}

		internal void ZoomIn(object minXValue, object maxXValue, ZoomingAction action)
		{
			Chart chart = base.Chart as Chart;
			if (chart == null)
			{
				return;
			}
			this._internalZoomingScale = this.CalculateZoomingScaleFromXValueRange(minXValue, maxXValue);
			this._oldScrollBarScale = this._internalZoomingScale;
			this.ScrollBarElement.Scale = this._internalZoomingScale;
			chart.ChartArea.AxisX.ScrollBarElement._isZoomedUsingZoomRect = true;
			chart.ChartArea.OnScrollBarScaleChanged(chart);
			chart.Dispatcher.BeginInvoke(new Action(delegate
			{
				double num = this.XValueToInternalXValue(minXValue);
				double num2 = this.XValueToInternalXValue(maxXValue);
				if (num > num2)
				{
					double num3 = num;
					num = num2;
					num2 = num3;
				}
				chart.ChartArea.AxisX.ScrollBarElement.UpdateScale(this._internalZoomingScale);
				double xValue = (this.AxisOrientation == AxisOrientation.Horizontal) ? num : num2;
				double num4 = this.XValueToScrollBarOffset(xValue);
				bool flag = false;
				if ((chart.ChartArea.AxisX.ScrollBarOffset == 0.0 && num4 == 0.0) || (chart.ChartArea.AxisX.ScrollBarOffset == 1.0 && num4 == 1.0))
				{
					flag = true;
				}
				chart.ChartArea.AxisX.ScrollBarOffset = num4;
				if (flag && this._isScrollToOffsetEnabled)
				{
					this.SetScrollBarValueFromOffset(num4, true);
				}
				if (Math.Round(this.ScrollBarElement.Scale, 4) >= 1.0)
				{
					this.ResetZoomState(this.Chart as Chart, true);
				}
			}), new object[0]);
		}

		internal void Zoom(object minXValue, object maxXValue, ZoomingAction action)
		{
			if (base.Chart != null && (base.Chart as Chart).PlotDetails != null && (base.Chart as Chart).PlotDetails.ChartOrientation == ChartOrientationType.Circular)
			{
				return;
			}
			Chart chart = base.Chart as Chart;
			if (chart.ZoomingEnabled)
			{
				if (!this.IsZoomingAllowed(minXValue, maxXValue, ref action))
				{
					return;
				}
				if (this._oldZoomState.MinXValue != null && this._oldZoomState.MaxXValue != null)
				{
					this._zoomStateStack.Push(new Axis.ZoomState(this._oldZoomState.MinXValue, this._oldZoomState.MaxXValue));
				}
				this.ZoomIn(minXValue, maxXValue, action);
				this._oldZoomState.MinXValue = minXValue;
				this._oldZoomState.MaxXValue = maxXValue;
				chart.ChartArea.EnableZoomIcons(chart);
			}
		}

		public void Zoom(object minXValue, object maxXValue)
		{
			this.Zoom(minXValue, maxXValue, ZoomingAction.Unknown);
		}

		private bool IsZoomingAllowed(object minXValue, object maxXValue, ref ZoomingAction action)
		{
			if (action == ZoomingAction.Unknown)
			{
				action = this.GetZoomingProcess(minXValue, maxXValue);
			}
			if (this._oldZoomState.MinXValue == null || this._oldZoomState.MaxXValue == null)
			{
				return true;
			}
			if (this.IsDateTimeAxis)
			{
				if ((DateTime)minXValue == (DateTime)this._oldZoomState.MinXValue && (DateTime)maxXValue == (DateTime)this._oldZoomState.MaxXValue)
				{
					return false;
				}
			}
			else if (Convert.ToDouble(minXValue) == Convert.ToDouble(this._oldZoomState.MinXValue) && Convert.ToDouble(maxXValue) == Convert.ToDouble(this._oldZoomState.MaxXValue))
			{
				return false;
			}
			if (action == ZoomingAction.In)
			{
				if (this.IsDateTimeAxis)
				{
					if ((DateTime)this._oldZoomState.MinXValue == (DateTime)this._oldZoomState.MaxXValue)
					{
						return false;
					}
				}
				else if (Convert.ToDouble(this._oldZoomState.MinXValue) == Convert.ToDouble(this._oldZoomState.MaxXValue))
				{
					return false;
				}
				return Math.Round(this.ScrollableLength, 2) < ChartArea.MAX_PLOTAREA_SIZE;
			}
			return true;
		}

		private ZoomingAction GetZoomingProcess(object minXValue, object maxXValue)
		{
			if (this._oldZoomState == null || this._oldZoomState.MinXValue == null)
			{
				return ZoomingAction.In;
			}
			if (this.IsDateTimeAxis)
			{
				if ((DateTime)minXValue > (DateTime)this._oldZoomState.MinXValue || (DateTime)maxXValue < (DateTime)this._oldZoomState.MaxXValue)
				{
					return ZoomingAction.In;
				}
				if ((DateTime)minXValue < (DateTime)this._oldZoomState.MinXValue || (DateTime)maxXValue > (DateTime)this._oldZoomState.MaxXValue)
				{
					return ZoomingAction.Out;
				}
				return ZoomingAction.In;
			}
			else
			{
				if (Convert.ToDouble(minXValue) > Convert.ToDouble(this._oldZoomState.MinXValue) || Convert.ToDouble(maxXValue) < Convert.ToDouble(this._oldZoomState.MaxXValue))
				{
					return ZoomingAction.In;
				}
				if (Convert.ToDouble(minXValue) < Convert.ToDouble(this._oldZoomState.MinXValue) || Convert.ToDouble(maxXValue) > Convert.ToDouble(this._oldZoomState.MaxXValue))
				{
					return ZoomingAction.Out;
				}
				return ZoomingAction.In;
			}
		}

		internal static double GetAxisTop(Axis axis)
		{
			double result = 0.0;
			Chart chart = axis.Chart as Chart;
			chart._topOuterPanel.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			chart._topOffsetGrid.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			chart._topAxisGrid.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			double num = 0.0;
			if (chart.Bevel)
			{
				num = 5.0;
			}
			if (axis.AxisRepresentation == AxisRepresentations.AxisY)
			{
				if (axis.AxisOrientation == AxisOrientation.Vertical)
				{
					if (axis.AxisType == AxisTypes.Primary || axis.AxisType == AxisTypes.Secondary)
					{
						result = num + chart.Padding.Top + chart._topOuterPanel.DesiredSize.Height + chart._topAxisGrid.DesiredSize.Height + chart._topOffsetGrid.DesiredSize.Height + chart.BorderThickness.Top;
					}
				}
				else if (axis.AxisType == AxisTypes.Primary)
				{
					result = num + chart.Padding.Top + chart._topOuterPanel.DesiredSize.Height + chart._topAxisGrid.DesiredSize.Height + chart.ChartArea.PlotAreaCanvas.Height + chart._topOffsetGrid.DesiredSize.Height + chart.BorderThickness.Top;
				}
				else
				{
					result = num + chart.Padding.Top + chart._topOuterPanel.DesiredSize.Height + chart._topOffsetGrid.DesiredSize.Height + chart.BorderThickness.Top;
				}
			}
			else if (axis.AxisOrientation == AxisOrientation.Vertical)
			{
				if (axis.AxisType == AxisTypes.Primary)
				{
					result = num + chart.Padding.Top + chart._topOuterPanel.DesiredSize.Height + chart._topAxisGrid.DesiredSize.Height + chart._topOffsetGrid.DesiredSize.Height + chart.BorderThickness.Top;
				}
			}
			else if (axis.AxisType == AxisTypes.Primary)
			{
				result = num + chart.Padding.Top + chart._topOuterPanel.DesiredSize.Height + chart._topAxisGrid.DesiredSize.Height + chart.ChartArea.PlotAreaCanvas.Height + chart._topOffsetGrid.DesiredSize.Height + chart.BorderThickness.Top;
			}
			else
			{
				result = num + chart.Padding.Top + chart._topOuterPanel.DesiredSize.Height + chart._topOffsetGrid.DesiredSize.Height + chart.BorderThickness.Top;
			}
			return result;
		}

		internal object InternalXValue2XValue(double xValue)
		{
			Chart chart = base.Chart as Chart;
			object result;
			if (chart.ChartArea.AxisX.IsDateTimeAxis)
			{
				try
				{
					result = DateTimeHelper.XValueToDateTime(chart.ChartArea.AxisX.MinDate, xValue, chart.ChartArea.AxisX.InternalIntervalType);
					return result;
				}
				catch
				{
					result = chart.ChartArea.AxisX.MinDate;
					return result;
				}
			}
			result = xValue;
			return result;
		}

		internal static double GetAxisLeft(Axis axis)
		{
			double result = 0.0;
			Chart chart = axis.Chart as Chart;
			chart._leftOffsetGrid.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			chart._leftOuterPanel.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			double num = 0.0;
			if (chart.Bevel)
			{
				num = 5.0;
			}
			if (axis.AxisRepresentation == AxisRepresentations.AxisY)
			{
				if (axis.AxisOrientation == AxisOrientation.Vertical)
				{
					if (axis.AxisType == AxisTypes.Primary)
					{
						result = num + chart.Padding.Left + chart._leftOuterPanel.DesiredSize.Width + chart._leftOffsetGrid.DesiredSize.Width + chart.BorderThickness.Left;
					}
					else
					{
						chart._leftAxisGrid.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
						result = num + chart.Padding.Left + chart._leftOuterPanel.DesiredSize.Width + chart._leftAxisGrid.DesiredSize.Width + chart.ChartArea.PlotAreaCanvas.Width + chart._leftOffsetGrid.DesiredSize.Width + chart.BorderThickness.Left;
					}
				}
				else if (axis.AxisType == AxisTypes.Primary || axis.AxisType == AxisTypes.Secondary)
				{
					chart._leftAxisGrid.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
					result = num + chart.Padding.Left + chart._leftOuterPanel.DesiredSize.Width + chart._leftAxisGrid.DesiredSize.Width + chart._leftOffsetGrid.DesiredSize.Width + chart.BorderThickness.Left;
				}
			}
			else if (axis.AxisOrientation == AxisOrientation.Vertical)
			{
				if (axis.AxisType == AxisTypes.Primary)
				{
					result = num + chart.Padding.Left + chart._leftOuterPanel.DesiredSize.Width + chart._leftOffsetGrid.DesiredSize.Width + chart.BorderThickness.Left;
				}
			}
			else if (axis.AxisType == AxisTypes.Primary || axis.AxisType == AxisTypes.Secondary)
			{
				chart._leftAxisGrid.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				result = num + chart.Padding.Left + chart._leftOuterPanel.DesiredSize.Width + chart._leftAxisGrid.DesiredSize.Width + chart._leftOffsetGrid.DesiredSize.Width + chart.BorderThickness.Left;
			}
			return result;
		}

		private static void OnPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.InternalPadding = (Thickness)e.NewValue;
			axis.FirePropertyChanged(VcProperties.Padding);
		}

		private static void OnBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.InternalBackground = (Brush)e.NewValue;
			axis.FirePropertyChanged(VcProperties.Background);
		}

		private static void OnOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.InternalOpacity = (double)e.NewValue;
			axis.FirePropertyChanged(VcProperties.Opacity);
		}

		private static void OnViewportRangeEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.ViewportRangeEnabled);
		}

		private static void OnAxisLabelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			if (axis.Chart != null)
			{
				axis.AxisLabels.Chart = axis.Chart;
			}
			axis.AxisLabels.Parent = axis;
			axis.AxisLabels.PropertyChanged += new PropertyChangedEventHandler(Axis.AxisLabels_PropertyChanged);
			if (!axis.AxisLabels.IsDefault)
			{
				axis.FirePropertyChanged(VcProperties.AxisLabels);
			}
		}

		private static void AxisLabels_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			(sender as AxisLabels).Parent.FirePropertyChanged((VcProperties)Enum.Parse(typeof(VcProperties), e.PropertyName, true));
		}

		private static void OnHrefTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.HrefTarget);
		}

		private static void OnHrefChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.Href);
		}

		private static void OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			if (e.NewValue == null)
			{
				axis.ResetAxisInterval();
			}
			axis.FirePropertyChanged(VcProperties.Interval);
		}

		private static void OnLineColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.LineColor);
		}

		private static void OnLineThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.LineThickness);
		}

		private static void OnLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.LineStyle);
		}

		private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.Title);
		}

		private static void OnTitleFontColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.TitleFontColor);
		}

		private static void OnTitleFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.TitleFontFamily);
		}

		private static void OnTitleFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.TitleFontSize);
		}

		private static void OnTitleFontStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.TitleFontStyle);
		}

		private static void OnTitleFontWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.TitleFontWeight);
		}

		private static void OnTitleTextDecorationsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.TitleTextDecorations);
		}

		private static void OnAxisTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.AxisType);
		}

		private static void OnMaxHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.InternalMaxHeight = (double)e.NewValue;
			axis.FirePropertyChanged(VcProperties.MaxHeight);
		}

		private static void OnMinHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.InternalMinHeight = (double)e.NewValue;
			axis.FirePropertyChanged(VcProperties.MinHeight);
		}

		private static void OnMaxWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.InternalMaxWidth = (double)e.NewValue;
			axis.FirePropertyChanged(VcProperties.MaxWidth);
		}

		private static void OnMinWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.InternalMinWidth = (double)e.NewValue;
			axis.FirePropertyChanged(VcProperties.MinWidth);
		}

		private static void OnAxisMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			if (e.NewValue == null)
			{
				axis.ResetAxisMaximum();
				axis.FirePropertyChanged(VcProperties.AxisMaximum);
				return;
			}
			double axisMaximumNumeric = axis.AxisMaximumNumeric;
			DateTime axisMaximumDateTime = axis.AxisMaximumDateTime;
			Axis.ConvertValueToDateTimeOrNumeric("AxisMaximum", e.NewValue, ref axisMaximumNumeric, ref axisMaximumDateTime, out axis._axisMaximumValueType);
			axis.AxisMaximumNumeric = axisMaximumNumeric;
			axis.AxisMaximumDateTime = axisMaximumDateTime;
			if (axis.AxisRepresentation == AxisRepresentations.AxisY)
			{
				if (axis.Chart != null && (axis.Chart as Chart).Series.Count > 0)
				{
					Dispatcher arg_C1_0 = (axis.Chart as Chart).Dispatcher;
					Delegate arg_C1_1 = new Action<VcProperties, object>((axis.Chart as Chart).Series[0].UpdateVisual);
					object[] array = new object[2];
					array[0] = VcProperties.AxisMaximum;
					arg_C1_0.BeginInvoke(arg_C1_1, array);
					return;
				}
			}
			else
			{
				axis.FirePropertyChanged(VcProperties.AxisMaximum);
			}
		}

		private static void OnAxisMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			if (e.NewValue == null)
			{
				axis.ResetAxisMinimum();
				axis.FirePropertyChanged(VcProperties.AxisMinimum);
				return;
			}
			double axisMinimumNumeric = axis.AxisMinimumNumeric;
			DateTime axisMinimumDateTime = axis.AxisMinimumDateTime;
			Axis.ConvertValueToDateTimeOrNumeric("AxisMinimum", e.NewValue, ref axisMinimumNumeric, ref axisMinimumDateTime, out axis._axisMinimumValueType);
			axis.AxisMinimumNumeric = axisMinimumNumeric;
			axis.AxisMinimumDateTime = axisMinimumDateTime;
			if (axis.AxisRepresentation == AxisRepresentations.AxisY)
			{
				if (axis.Chart != null && (axis.Chart as Chart).Series.Count > 0)
				{
					Dispatcher arg_C1_0 = (axis.Chart as Chart).Dispatcher;
					Delegate arg_C1_1 = new Action<VcProperties, object>((axis.Chart as Chart).Series[0].UpdateVisual);
					object[] array = new object[2];
					array[0] = VcProperties.AxisMinimum;
					arg_C1_0.BeginInvoke(arg_C1_1, array);
					return;
				}
			}
			else
			{
				axis.FirePropertyChanged(VcProperties.AxisMinimum);
			}
		}

		private void ResetAxisMinimum()
		{
			this.AxisMinimumNumeric = double.NaN;
			this.AxisMinimumDateTime = default(DateTime);
			this.InternalAxisMinimum = double.NaN;
			this._numericViewMinimum = 0.0;
			this._oldInternalAxisMinimum = 0.0;
		}

		private void ResetAxisMaximum()
		{
			this.AxisMaximumNumeric = double.NaN;
			this.AxisMaximumDateTime = default(DateTime);
			this.InternalAxisMaximum = double.NaN;
			this._numericViewMaximum = 0.0;
			this._oldInternalAxisMaximum = 0.0;
		}

		private void ResetAxisInterval()
		{
			this.InternalInterval = double.NaN;
		}

		private static void ConvertValueToDateTimeOrNumeric(string propertyName, object newValue, ref double numericVal, ref DateTime dateTimeValue, out ChartValueTypes valueType)
		{
			if (newValue.GetType().Equals(typeof(double)) || newValue.GetType().Equals(typeof(int)))
			{
				numericVal = Convert.ToDouble(newValue);
				valueType = ChartValueTypes.Numeric;
				return;
			}
			if (newValue.GetType().Equals(typeof(DateTime)))
			{
				dateTimeValue = (DateTime)newValue;
				valueType = ChartValueTypes.DateTime;
				return;
			}
			if (!newValue.GetType().Equals(typeof(string)))
			{
				throw new Exception("Invalid Input for " + propertyName);
			}
			if (string.IsNullOrEmpty(newValue.ToString()))
			{
				numericVal = double.NaN;
				valueType = ChartValueTypes.Numeric;
				return;
			}
			double num;
			if (double.TryParse((string)newValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num))
			{
				numericVal = num;
				valueType = ChartValueTypes.Numeric;
				return;
			}
			DateTime dateTime;
			if (DateTime.TryParse((string)newValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
			{
				dateTimeValue = dateTime;
				valueType = ChartValueTypes.DateTime;
				return;
			}
			throw new Exception("Invalid Input for " + propertyName);
		}

		private static void OnIncludeZeroPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.IncludeZero);
		}

		private static void OnStartFromZeroPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.StartFromZero);
		}

		private static void OnPrefixPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.Prefix);
		}

		private static void OnSuffixPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.Suffix);
		}

		private static void OnScalingSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.ParseScalingSets((string)e.NewValue);
			axis.FirePropertyChanged(VcProperties.ScalingSet);
		}

		private static void OnValueFormatStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.ValueFormatString);
		}

		private static void OnScrollBarOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			if (axis._isScrollToOffsetEnabled)
			{
				axis.SetScrollBarValueFromOffset((double)e.NewValue, true);
			}
		}

		private static void OnScrollBarSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.ScrollBarSize);
		}

		private static void OnScrollBarScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.ScrollBarScale);
			if (axis.Chart != null && (axis.Chart as Chart).ChartArea != null)
			{
				if (axis.IsNotificationEnable)
				{
					(axis.Chart as Chart).ChartArea.IsAutoCalculatedScrollBarScale = false;
					return;
				}
				(axis.Chart as Chart).ChartArea.IsAutoCalculatedScrollBarScale = true;
			}
		}

		private static void OnClosestPlotDistancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.ClosestPlotDistance);
		}

		private static void OnEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.Enabled);
		}

		private static void OnDisplayAutoAxisLabelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.DisplayAutoAxisLabels);
		}

		private static void OnIntervalTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.InternalIntervalType = (IntervalTypes)e.NewValue;
			axis.FirePropertyChanged(VcProperties.IntervalType);
		}

		private static void OnLogarithmicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.Logarithmic);
		}

		private static void OnAxisOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			double? num = (double?)e.NewValue;
			if (num.HasValue && !double.IsNaN(num.Value))
			{
				double? num2 = num;
				if (num2.GetValueOrDefault() > 0.0 || !num2.HasValue)
				{
					double? num3 = num;
					if (num3.GetValueOrDefault() < 0.5 || !num3.HasValue)
					{
						goto IL_78;
					}
				}
				throw new ArgumentException("AxisOffset property should be greater than 0 and less than .5");
			}
			IL_78:
			axis.FirePropertyChanged(VcProperties.AxisOffSet);
		}

		private static void OnLogarithmBasePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Axis axis = d as Axis;
			axis.FirePropertyChanged(VcProperties.LogarithmBase);
		}

		internal void SetUpAxisManager()
		{
			this.Minimum = ((this.AxisRepresentation == AxisRepresentations.AxisX) ? this.PlotDetails.GetAxisXMinimumDataValue(this) : this.PlotDetails.GetAxisYMinimumDataValue(this));
			this.Maximum = ((this.AxisRepresentation == AxisRepresentations.AxisX) ? this.PlotDetails.GetAxisXMaximumDataValue(this) : this.PlotDetails.GetAxisYMaximumDataValue(this));
			if (this.Logarithmic)
			{
				double num;
				if (this.AxisRepresentation == AxisRepresentations.AxisY)
				{
					num = this.PlotDetails.GetAxisYMinimumDataValueFromAllDataSeries(this);
				}
				else
				{
					if (this.IsDateTimeAxis)
					{
						throw new Exception("DateTime values cannot be plotted correctly on logarithmic charts");
					}
					num = this.PlotDetails.GetAxisXMinimumDataValueFromAllDataSeries(this);
				}
				if (!this.PlotDetails.CheckIfAnyDataPointVisualExistsInChart())
				{
					num = 1.0;
					this.Maximum = Math.Pow(this.LogarithmBase, 1.0);
					this.Minimum = num;
				}
				bool flag = this.AxisRepresentation != AxisRepresentations.AxisX || this.PlotDetails.ChartOrientation != ChartOrientationType.Circular;
				if (flag && (num <= 0.0 || this.Maximum <= 0.0))
				{
					throw new Exception("Negative or zero values cannot be plotted correctly on logarithmic charts.");
				}
			}
			bool allowLimitOverflow = this.AxisRepresentation == AxisRepresentations.AxisX;
			bool stacked100OverrideState = this.PlotDetails.GetStacked100OverrideState();
			if (stacked100OverrideState)
			{
				bool flag2 = this.IsAllDataSeriesEmpty();
				if (flag2)
				{
					this.Maximum = 100.0;
					this.Minimum = 0.0;
				}
			}
			bool startFromMinimumValue4LogScale = false;
			if (this.AxisRepresentation == AxisRepresentations.AxisY && this.ViewportRangeEnabled && this.Logarithmic)
			{
				startFromMinimumValue4LogScale = true;
			}
			bool isCircularAxis = this.AxisOrientation == AxisOrientation.Circular;
			this.AxisManager = new AxisManager(this.Maximum, this.Minimum, this.StartFromZero.Value, allowLimitOverflow, stacked100OverrideState, isCircularAxis, this.AxisRepresentation, this.Logarithmic, this.LogarithmBase, startFromMinimumValue4LogScale, this.IsDateTimeAxis);
			this.AxisManager.IncludeZero = this.IncludeZero;
			if (this.AxisRepresentation == AxisRepresentations.AxisX && this.AxisOrientation != AxisOrientation.Circular)
			{
				double num2 = this.GenerateDefaultInterval();
				if (this.IsDateTimeAxis)
				{
					if (num2 > 0.0 || !double.IsNaN(num2))
					{
						this.AxisManager.Interval = num2;
						this.InternalInterval = num2;
					}
					else
					{
						double? interval = this.Interval;
						if ((interval.GetValueOrDefault() > 0.0 && interval.HasValue) || !double.IsNaN(this.Interval.Value))
						{
							this.AxisManager.Interval = this.Interval.Value;
							this.InternalInterval = this.Interval.Value;
						}
					}
				}
				else if (num2 > 0.0 || !double.IsNaN(num2))
				{
					this.AxisManager.Interval = num2;
					this.InternalInterval = num2;
				}
			}
			else
			{
				double? interval2 = this.Interval;
				if ((interval2.GetValueOrDefault() > 0.0 && interval2.HasValue) || !double.IsNaN(this.Interval.Value))
				{
					this.AxisManager.Interval = this.Interval.Value;
					this.InternalInterval = this.Interval.Value;
				}
			}
			if (!double.IsNaN(this.AxisMaximumNumeric))
			{
				this.AxisManager.AxisMaximumValue = this.AxisMaximumNumeric;
				this.InternalAxisMaximum = this.AxisMaximumNumeric;
			}
			else if (this.AxisOrientation == AxisOrientation.Circular && this.IsDateTimeAxis && this.AxisMaximum != null)
			{
				double num3;
				if (this.XValueType != ChartValueTypes.Time)
				{
					num3 = DateTimeHelper.DateDiff(this.AxisMaximumDateTime, this.MinDate, this.MinDateRange, this.MaxDateRange, this.InternalIntervalType, this.XValueType);
				}
				else
				{
					num3 = DateTimeHelper.DateDiff(Convert.ToDateTime(this.AxisMaximum), this.MinDate, this.MinDateRange, this.MaxDateRange, this.InternalIntervalType, this.XValueType);
				}
				this.AxisManager.AxisMaximumValue = num3;
				this.InternalAxisMaximum = num3;
			}
			if (!double.IsNaN(this.AxisMinimumNumeric))
			{
				this.AxisManager.AxisMinimumValue = this.AxisMinimumNumeric;
				this.InternalAxisMinimum = this.AxisMinimumNumeric;
			}
			else if (this.AxisOrientation == AxisOrientation.Circular && this.IsDateTimeAxis && this.AxisMinimum != null)
			{
				double num4;
				if (this.XValueType != ChartValueTypes.Time)
				{
					num4 = DateTimeHelper.DateDiff(this.AxisMinimumDateTime, this.MinDate, this.MinDateRange, this.MaxDateRange, this.InternalIntervalType, this.XValueType);
				}
				else
				{
					num4 = DateTimeHelper.DateDiff(Convert.ToDateTime(this.AxisMinimum), this.MinDate, this.MinDateRange, this.MaxDateRange, this.InternalIntervalType, this.XValueType);
				}
				this.AxisManager.AxisMinimumValue = num4;
				this.InternalAxisMinimum = num4;
			}
			this.AxisManager.Calculate();
			if (this.AxisRepresentation == AxisRepresentations.AxisX && this.AxisOrientation != AxisOrientation.Circular)
			{
				if (!this.SetAxesXLimits())
				{
					return;
				}
				if (!double.IsNaN(this.Interval.Value) && this.IntervalType != IntervalTypes.Auto && this.IsDateTimeAxis)
				{
					this.AxisManager.Interval = this.Interval.Value;
				}
			}
			if (this.AxisRepresentation == AxisRepresentations.AxisY && this.AxisType == AxisTypes.Primary)
			{
				this.InternalAxisMaximum = this.AxisManager.AxisMaximumValue;
				this.InternalAxisMinimum = this.AxisManager.AxisMinimumValue;
			}
			else if (this.AxisRepresentation == AxisRepresentations.AxisY && this.AxisType == AxisTypes.Secondary)
			{
				IEnumerable<Axis> source = from axis in (base.Chart as Chart).InternalAxesY
				where axis.AxisRepresentation == AxisRepresentations.AxisY && axis.AxisType == AxisTypes.Primary
				select axis;
				Axis axis2 = null;
				if (source.Any<Axis>())
				{
					axis2 = source.First<Axis>();
				}
				if (double.IsNaN(this.Interval.Value) && axis2 != null)
				{
					double num5 = (axis2.InternalAxisMaximum - axis2.InternalAxisMinimum) / axis2.InternalInterval;
					if (!double.IsNaN(num5))
					{
						this.AxisManager.AxisMinimumValue = this.AxisManager.AxisMinimumValue;
						if (this.AxisManager.AxisMinimumValue == 0.0 && this.Logarithmic)
						{
							this.AxisManager.AxisMinimumValue = Math.Pow(this.LogarithmBase, 0.0);
						}
						if (this.Logarithmic)
						{
							this.AxisManager.AxisMaximumValue = Math.Pow(this.LogarithmBase, this.AxisManager.AxisMaximumValue);
							this.AxisManager.Interval = (Math.Log(this.AxisManager.AxisMaximumValue, this.LogarithmBase) - Math.Log(this.AxisManager.AxisMinimumValue, this.LogarithmBase)) / num5;
						}
						else
						{
							this.AxisManager.AxisMaximumValue = this.AxisManager.AxisMaximumValue;
							this.AxisManager.Interval = (this.AxisManager.AxisMaximumValue - this.AxisManager.AxisMinimumValue) / num5;
						}
						this.AxisManager.Calculate();
					}
				}
				this.InternalAxisMaximum = this.AxisManager.AxisMaximumValue;
				this.InternalAxisMinimum = this.AxisManager.AxisMinimumValue;
			}
			else
			{
				this.InternalAxisMaximum = this.AxisManager.AxisMaximumValue;
				this.InternalAxisMinimum = this.AxisManager.AxisMinimumValue;
			}
			this.InternalInterval = this.AxisManager.Interval;
		}

		private bool IsAllDataSeriesEmpty()
		{
			Chart chart = base.Chart as Chart;
			if (chart.InternalSeries.Count != 0)
			{
				List<DataSeries> list = (from ds in chart.InternalSeries
				where ds.InternalDataPoints.Count == 0
				select ds).ToList<DataSeries>();
				return list.Count != 0;
			}
			return false;
		}

		private void UpdateVerticalAxisSettings()
		{
			switch (this.AxisType)
			{
			case AxisTypes.Primary:
				this.UpdateVerticalPrimaryAxisSettings();
				return;
			case AxisTypes.Secondary:
				this.UpdateVerticalSecondaryAxisSettings();
				return;
			default:
				return;
			}
		}

		private void ApplyVerticalAxisSettings()
		{
			switch (this.AxisType)
			{
			case AxisTypes.Primary:
				this.ApplyVerticalPrimaryAxisSettings();
				return;
			case AxisTypes.Secondary:
				this.ApplyVerticalSecondaryAxisSettings();
				return;
			default:
				return;
			}
		}

		private void ApplyTitleProperties()
		{
			if (this.AxisTitleElement != null)
			{
				this.AxisTitleElement.IsNotificationEnable = false;
				if (this.TitleFontFamily != null)
				{
					this.AxisTitleElement.InternalFontFamily = this.TitleFontFamily;
				}
				if (this.TitleFontSize != 0.0)
				{
					this.AxisTitleElement.InternalFontSize = this.TitleFontSize;
				}
				FontStyle arg_58_0 = this.TitleFontStyle;
				this.AxisTitleElement.InternalFontStyle = this.TitleFontStyle;
				FontWeight arg_70_0 = this.TitleFontWeight;
				this.AxisTitleElement.InternalFontWeight = this.TitleFontWeight;
				if (this.TitleTextDecorations != null)
				{
					this.AxisTitleElement.TextDecorations = this.TitleTextDecorations;
				}
				this.AxisTitleElement.Text = ObservableObject.GetFormattedMultilineText(this.Title);
				this.AxisTitleElement.InternalFontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.Background, this.TitleFontColor, false);
				this.AxisTitleElement.IsNotificationEnable = true;
			}
		}

		private void CreateAxisLine(double y1, double y2, double x1, double x2, double width, double height)
		{
			this.AxisLine = new Line
			{
				Y1 = y1,
				Y2 = y2,
				X1 = x1,
				X2 = x2,
				Width = width,
				Height = height
			};
			this.AxisLine.StrokeThickness = this.LineThickness.Value;
			this.AxisLine.Stroke = this.LineColor;
			this.AxisLine.StrokeDashArray = ExtendedGraphics.GetDashArray(this.LineStyle);
		}

		private void ClipVerticalAxis(double ticksWidth)
		{
			if (this.Height != this.ScrollableSize)
			{
				double num = 4.0;
				PathGeometry pathGeometry = new PathGeometry();
				pathGeometry.Figures = new PathFigureCollection();
				PathFigure pathFigure = new PathFigure();
				pathFigure.StartPoint = new Point(0.0, -(num - 1.0));
				pathFigure.Segments = new PathSegmentCollection();
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(this.ScrollViewerElement.Width - ticksWidth, -(num - 1.0))));
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(this.ScrollViewerElement.Width - ticksWidth, 0.0)));
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(this.ScrollViewerElement.Width, 0.0)));
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(this.ScrollViewerElement.Width, this.Height)));
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(this.ScrollViewerElement.Width - ticksWidth, this.Height)));
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(this.ScrollViewerElement.Width - ticksWidth, this.Height + num)));
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(0.0, this.Height + num)));
				pathGeometry.Figures.Add(pathFigure);
				this.ScrollViewerElement.Clip = pathGeometry;
			}
		}

		private double GetSizeFromTrendLineLabels(Axis axis)
		{
			Chart chart = base.Chart as Chart;
			List<TrendLine> trendLines;
			List<TrendLine> trendLines2;
			List<TrendLine> trendLines3;
			List<TrendLine> trendLines4;
			if (this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				trendLines = (from trendline in chart.TrendLines
				where trendline.Orientation == Orientation.Vertical && trendline.AxisType == AxisTypes.Primary
				select trendline).ToList<TrendLine>();
				trendLines2 = (from trendline in chart.TrendLines
				where trendline.Orientation == Orientation.Horizontal && trendline.AxisType == AxisTypes.Primary
				select trendline).ToList<TrendLine>();
				trendLines3 = (from trendline in chart.TrendLines
				where trendline.Orientation == Orientation.Vertical && trendline.AxisType == AxisTypes.Secondary
				select trendline).ToList<TrendLine>();
				trendLines4 = (from trendline in chart.TrendLines
				where trendline.Orientation == Orientation.Horizontal && trendline.AxisType == AxisTypes.Secondary
				select trendline).ToList<TrendLine>();
			}
			else
			{
				trendLines = (from trendline in chart.TrendLines
				where trendline.Orientation == Orientation.Horizontal && trendline.AxisType == AxisTypes.Primary
				select trendline).ToList<TrendLine>();
				trendLines2 = (from trendline in chart.TrendLines
				where trendline.Orientation == Orientation.Vertical && trendline.AxisType == AxisTypes.Primary
				select trendline).ToList<TrendLine>();
				trendLines3 = (from trendline in chart.TrendLines
				where trendline.Orientation == Orientation.Horizontal && trendline.AxisType == AxisTypes.Secondary
				select trendline).ToList<TrendLine>();
				trendLines4 = (from trendline in chart.TrendLines
				where trendline.Orientation == Orientation.Vertical && trendline.AxisType == AxisTypes.Secondary
				select trendline).ToList<TrendLine>();
			}
			double trendLineLabelSize;
			if (axis.AxisRepresentation == AxisRepresentations.AxisX)
			{
				if (axis.AxisType == AxisTypes.Primary)
				{
					trendLineLabelSize = this.GetTrendLineLabelSize(trendLines, axis);
				}
				else
				{
					trendLineLabelSize = this.GetTrendLineLabelSize(trendLines3, axis);
				}
			}
			else if (axis.AxisType == AxisTypes.Primary)
			{
				trendLineLabelSize = this.GetTrendLineLabelSize(trendLines2, axis);
			}
			else
			{
				trendLineLabelSize = this.GetTrendLineLabelSize(trendLines4, axis);
			}
			return trendLineLabelSize;
		}

		private double GetTrendLineLabelSize(List<TrendLine> trendLines, Axis axis)
		{
			double num = -1.7976931348623157E+308;
			foreach (TrendLine current in trendLines)
			{
				if (current.Enabled.Value && current.LabelTextBlock != null)
				{
					Size size = Graphics.CalculateVisualSize(current.LabelTextBlock);
					if (this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
					{
						if (axis.AxisOrientation == AxisOrientation.Horizontal)
						{
							if (axis.AxisType == AxisTypes.Primary)
							{
								num = Math.Max(num, size.Height + 6.0);
							}
						}
						else if (axis.AxisType == AxisTypes.Primary)
						{
							num = Math.Max(num, size.Width + 10.0);
						}
						else
						{
							num = Math.Max(num, size.Width + 10.0);
						}
					}
					else if (axis.AxisOrientation == AxisOrientation.Vertical)
					{
						if (axis.AxisType == AxisTypes.Primary)
						{
							num = Math.Max(num, size.Width + 10.0);
						}
					}
					else if (axis.AxisType == AxisTypes.Primary)
					{
						num = Math.Max(num, size.Height + 6.0);
					}
					else
					{
						num = Math.Max(num, size.Height);
					}
				}
			}
			return num;
		}

		private void ApplyVerticalPrimaryAxisSettings()
		{
			this.Visual.Children.Add(new Border
			{
				Width = this.InternalPadding.Left
			});
			this.Visual.HorizontalAlignment = HorizontalAlignment.Left;
			this.Visual.VerticalAlignment = VerticalAlignment.Stretch;
			this.AxisElementsContainer.HorizontalAlignment = HorizontalAlignment.Right;
			this.AxisElementsContainer.VerticalAlignment = VerticalAlignment.Stretch;
			this.AxisElementsContainer.Orientation = Orientation.Horizontal;
			this.InternalStackPanel.Width = 0.0;
			this.InternalStackPanel.HorizontalAlignment = HorizontalAlignment.Left;
			this.InternalStackPanel.VerticalAlignment = VerticalAlignment.Stretch;
			this.ScrollViewerElement.VerticalAlignment = VerticalAlignment.Stretch;
			this.ScrollBarElement.Orientation = Orientation.Vertical;
			if (double.IsNaN(this.ScrollBarSize))
			{
				base.IsNotificationEnable = false;
				this.ScrollBarSize = this.ScrollBarElement.Width;
				base.IsNotificationEnable = true;
			}
			else
			{
				this.ScrollBarElement.Width = this.ScrollBarSize;
			}
			this.AxisLabels.Placement = PlacementTypes.Left;
			this.AxisLabels.Height = this.ScrollableSize;
			this.CreateAxisLine(this.StartOffset, this.Height - this.EndOffset, this.LineThickness.Value / 2.0, this.LineThickness.Value / 2.0, this.LineThickness.Value, this.Height);
			foreach (ChartGrid current in this.Grids)
			{
				current.Placement = PlacementTypes.Left;
			}
			this.AxisLabels.CreateVisualObject();
			foreach (CustomAxisLabels current2 in this.CustomAxisLabels)
			{
				current2.Placement = PlacementTypes.Left;
				current2.Height = this.ScrollableSize;
				current2.CreateVisualObject();
			}
			if (this.AxisTitleElement != null)
			{
				this.AxisTitleElement.InternalHorizontalAlignment = HorizontalAlignment.Left;
				this.AxisTitleElement.InternalVerticalAlignment = VerticalAlignment.Center;
			}
			this.CreateAxisTitleVisual(new Thickness(0.0, 0.0, 5.0, 0.0));
			if (this.AxisTitleElement != null)
			{
				this.AxisElementsContainer.Children.Add(this.AxisTitleElement.Visual);
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			if (this.AxisLabels.Visual != null)
			{
				double num5;
				if (this.AxisLabels.InternalMinWidth != 0.0 && this.AxisLabels.InternalMinWidth > this.AxisLabels.Visual.Width)
				{
					num5 = this.AxisLabels.InternalMinWidth;
				}
				else
				{
					num5 = this.AxisLabels.Visual.Width;
				}
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxWidth) && this.AxisLabels.InternalMaxWidth < num5)
				{
					this.AxisLabels.Visual.Width = this.AxisLabels.InternalMaxWidth;
				}
				else
				{
					this.AxisLabels.Visual.Width = num5;
				}
				num = Math.Max(num, this.AxisLabels.TopOverflow);
				num2 = Math.Max(num2, this.AxisLabels.BottomOverflow);
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxWidth) && this.AxisLabels.InternalMaxWidth <= num5)
				{
					this.GetNewOverflow4LeftLabels(num, num2, ref num3, ref num4, this.AxisLabels.Visual.Width);
				}
				else
				{
					num4 = num2;
					num3 = num;
				}
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = new Rect(-1.0, -4.0 - num3, this.AxisLabels.Visual.Width + 2.0, this.AxisLabels.Visual.Height + num3 + num4 + 8.0);
				this.AxisLabels.Visual.Clip = rectangleGeometry;
			}
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 0.0;
			double num9 = 0.0;
			List<CustomAxisLabels> list = null;
			if (this.CustomAxisLabels.Count > 0)
			{
				list = this.CustomAxisLabels.ToList<CustomAxisLabels>();
				list.Reverse();
				foreach (CustomAxisLabels current3 in list)
				{
					if (current3.Visual != null)
					{
						double num10;
						if (current3.InternalMinHeight != 0.0 && current3.InternalMinWidth > current3.Visual.Width)
						{
							num10 = current3.InternalMinWidth;
						}
						else
						{
							num10 = current3.Visual.Width;
						}
						if (!double.IsPositiveInfinity(current3.InternalMaxWidth) && current3.InternalMaxWidth < num10)
						{
							current3.Visual.Width = current3.InternalMaxWidth;
						}
						else
						{
							current3.Visual.Width = num10;
						}
						num8 = Math.Max(num8, current3.TopOverflow);
						num9 = Math.Max(num9, current3.BottomOverflow);
						if (!double.IsPositiveInfinity(current3.InternalMaxWidth) && current3.InternalMaxWidth <= num10)
						{
							this.GetNewOverflow4LeftCustomLabels(current3, num8, num9, ref num6, ref num7, current3.Visual.Width);
						}
						else
						{
							num6 = num8;
							num7 = num9;
						}
						RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
						rectangleGeometry2.Rect = new Rect(-1.0, -4.0 - num6, current3.Visual.Width + 2.0, current3.Visual.Height + num6 + num7 + 8.0);
						current3.Visual.Clip = rectangleGeometry2;
					}
				}
			}
			if (num < num8)
			{
				num = num8;
			}
			if (num2 < num9)
			{
				num2 = num9;
			}
			if (list != null)
			{
				foreach (CustomAxisLabels current4 in list)
				{
					if (current4.Visual != null)
					{
						this.InternalStackPanel.Width += current4.Visual.Width;
						if (this.Height == this.ScrollableSize)
						{
							this.AxisElementsContainer.Children.Add(current4.Visual);
						}
						else
						{
							this.InternalStackPanel.Children.Add(current4.Visual);
							current4.Visual.SetValue(Canvas.LeftProperty, this.InternalStackPanel.Width - current4.Visual.Width);
						}
					}
				}
			}
			if (this.AxisLabels.Visual != null)
			{
				this.AxisLabels.Visual.Margin = new Thickness(0.0, 0.0, 4.0, 0.0);
				if (this.Height == this.ScrollableSize)
				{
					this.AxisElementsContainer.Children.Add(this.AxisLabels.Visual);
				}
				else
				{
					this.AxisLabels.Visual.Width += 4.0;
					this.InternalStackPanel.Width += this.AxisLabels.Visual.Width;
					this.InternalStackPanel.Children.Add(this.AxisLabels.Visual);
					this.AxisLabels.Visual.SetValue(Canvas.LeftProperty, this.InternalStackPanel.Width - this.AxisLabels.Visual.Width);
				}
			}
			double num11 = 0.0;
			List<Ticks> list2 = this.Ticks.Reverse<Ticks>().ToList<Ticks>();
			foreach (Ticks current5 in list2)
			{
				current5.SetParms(PlacementTypes.Left, double.NaN, this.ScrollableSize);
				current5.CreateVisualObject();
				if (current5.Visual != null)
				{
					if (this.Height == this.ScrollableSize)
					{
						if (!this.AxisElementsContainer.Children.Contains(current5.Visual))
						{
							this.AxisElementsContainer.Children.Add(current5.Visual);
						}
					}
					else
					{
						if (!this.InternalStackPanel.Children.Contains(current5.Visual))
						{
							this.InternalStackPanel.Children.Add(current5.Visual);
						}
						current5.Visual.SetValue(Canvas.LeftProperty, this.InternalStackPanel.Width + num11);
						num11 += current5.Visual.Width;
					}
				}
			}
			this.InternalStackPanel.Width += num11;
			if (this.Height != this.ScrollableSize)
			{
				this.ScrollViewerElement.Children.Add(this.InternalStackPanel);
				this.AxisElementsContainer.Children.Add(this.ScrollViewerElement);
			}
			this.AxisElementsContainer.Children.Add(this.AxisLine);
			this.InternalStackPanel.Width += this.AxisLine.Width;
			this.ScrollViewerElement.Width = this.InternalStackPanel.Width;
			this.ClipVerticalAxis(num11);
			Size size = Graphics.CalculateVisualSize(this.AxisElementsContainer);
			double num12;
			if (this.InternalMinWidth != 0.0 && this.InternalMinWidth > size.Width)
			{
				num12 = this.InternalMinWidth;
			}
			else
			{
				num12 = size.Width;
			}
			if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth < num12)
			{
				num12 = this.InternalMaxWidth;
			}
			Canvas canvas = new Canvas();
			canvas.HorizontalAlignment = HorizontalAlignment.Right;
			canvas.VerticalAlignment = VerticalAlignment.Stretch;
			canvas.Width = num12;
			canvas.Children.Add(this.AxisElementsContainer);
			this.AxisElementsContainer.SetValue(Canvas.LeftProperty, num12 - size.Width);
			if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth <= num12)
			{
				if (this.AxisLabels.Visual != null)
				{
					this.GetNewOverflow4LeftLabels(num, num2, ref num3, ref num4, canvas.Width);
				}
				if (list != null)
				{
					foreach (CustomAxisLabels current6 in list)
					{
						if (current6.Visual != null)
						{
							this.GetNewOverflow4LeftCustomLabels(current6, num8, num9, ref num6, ref num7, canvas.Width);
						}
					}
				}
				if (num3 < num6)
				{
					num3 = num6;
				}
				if (num4 < num7)
				{
					num4 = num7;
				}
			}
			canvas.Clip = new RectangleGeometry
			{
				Rect = new Rect(-1.0, -4.0 - num3, canvas.Width + 2.0, size.Height + num3 + num4 + 8.0)
			};
			Size size2 = Graphics.CalculateVisualSize(this.Visual);
			this.Visual.Children.Add(canvas);
			size2.Width += canvas.Width;
			double sizeFromTrendLineLabels = this.GetSizeFromTrendLineLabels(this);
			if (size2.Width < sizeFromTrendLineLabels)
			{
				(this.Visual.Children[0] as Border).Width = sizeFromTrendLineLabels - size2.Width;
				size2.Width += (this.Visual.Children[0] as Border).Width - this.InternalPadding.Left;
			}
			this.Visual.Width = size2.Width;
		}

		private void GetNewOverflow4LeftLabels(double topOverflow, double bottomOverflow, ref double newTopOverflow, ref double newBottomOverflow, double visualWidth)
		{
			if (this.AxisLabels.InternalAngle.Value > 0.0)
			{
				double y = this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].Position.Y;
				Point point = new Point(this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].Position.X, this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].Position.Y);
				Point point2 = new Point(this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].ActualLeft - this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].ActualWidth, this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].ActualTop);
				Point point3 = new Point(point.X, point2.Y);
				double num = Graphics.DistanceBetweenTwoPoints(point3, point2);
				double num2 = Graphics.DistanceBetweenTwoPoints(point3, point);
				double a = Math.Atan(num / num2);
				if (num > visualWidth)
				{
					num = visualWidth;
				}
				topOverflow = num / Math.Tan(a);
				topOverflow -= y;
				newTopOverflow = topOverflow;
				this.AxisLabels.TopOverflow = newTopOverflow;
				return;
			}
			if (this.AxisLabels.InternalAngle.Value < 0.0)
			{
				double y2 = this.AxisLabels.AxisLabelList[0].Position.Y;
				Point point4 = new Point(this.AxisLabels.AxisLabelList[0].Position.X, this.AxisLabels.AxisLabelList[0].Position.Y);
				Point point5 = new Point(this.AxisLabels.AxisLabelList[0].ActualLeft, this.AxisLabels.AxisLabelList[0].ActualTop + this.AxisLabels.AxisLabelList[0].ActualHeight);
				Point point6 = new Point(point4.X, point5.Y);
				double num3 = Graphics.DistanceBetweenTwoPoints(point6, point5);
				double num4 = Graphics.DistanceBetweenTwoPoints(point6, point4);
				double a2 = Math.Atan(num3 / num4);
				if (num3 > visualWidth)
				{
					num3 = visualWidth;
				}
				bottomOverflow = num3 / Math.Tan(a2);
				bottomOverflow += y2;
				newBottomOverflow = bottomOverflow - this.Height;
				this.AxisLabels.BottomOverflow = newBottomOverflow;
			}
		}

		private void GetNewOverflow4LeftCustomLabels(CustomAxisLabels customLabels, double topOverflow, double bottomOverflow, ref double newTopOverflow, ref double newBottomOverflow, double visualWidth)
		{
			if (customLabels.InternalAngle.Value > 0.0)
			{
				double y = customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].Position.Y;
				Point point = new Point(customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].Position.X, customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].Position.Y);
				Point point2 = new Point(customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].ActualLeft - customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].ActualWidth, customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].ActualTop);
				Point point3 = new Point(point.X, point2.Y);
				double num = Graphics.DistanceBetweenTwoPoints(point3, point2);
				double num2 = Graphics.DistanceBetweenTwoPoints(point3, point);
				double a = Math.Atan(num / num2);
				if (num > visualWidth)
				{
					num = visualWidth;
				}
				topOverflow = num / Math.Tan(a);
				topOverflow -= y;
				newTopOverflow = topOverflow;
				customLabels.TopOverflow = newTopOverflow;
				return;
			}
			if (customLabels.InternalAngle.Value < 0.0)
			{
				double y2 = customLabels.AxisLabelList[0].Position.Y;
				Point point4 = new Point(customLabels.AxisLabelList[0].Position.X, customLabels.AxisLabelList[0].Position.Y);
				Point point5 = new Point(customLabels.AxisLabelList[0].ActualLeft, customLabels.AxisLabelList[0].ActualTop + customLabels.AxisLabelList[0].ActualHeight);
				Point point6 = new Point(point4.X, point5.Y);
				double num3 = Graphics.DistanceBetweenTwoPoints(point6, point5);
				double num4 = Graphics.DistanceBetweenTwoPoints(point6, point4);
				double a2 = Math.Atan(num3 / num4);
				if (num3 > visualWidth)
				{
					num3 = visualWidth;
				}
				bottomOverflow = num3 / Math.Tan(a2);
				bottomOverflow += y2;
				newBottomOverflow = bottomOverflow - this.Height;
				customLabels.BottomOverflow = newBottomOverflow;
			}
		}

		private void ApplyVerticalSecondaryAxisSettings()
		{
			this.AxisElementsContainer.HorizontalAlignment = HorizontalAlignment.Left;
			this.AxisElementsContainer.VerticalAlignment = VerticalAlignment.Stretch;
			this.AxisElementsContainer.Orientation = Orientation.Horizontal;
			this.InternalStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
			this.InternalStackPanel.VerticalAlignment = VerticalAlignment.Stretch;
			this.InternalStackPanel.SizeChanged += delegate(object sender, SizeChangedEventArgs e)
			{
				this.ScrollViewerElement.Width = e.NewSize.Width;
			};
			this.ScrollViewerElement.VerticalAlignment = VerticalAlignment.Stretch;
			this.ScrollBarElement.Orientation = Orientation.Vertical;
			if (double.IsNaN(this.ScrollBarSize))
			{
				base.IsNotificationEnable = false;
				this.ScrollBarSize = this.ScrollBarElement.Width;
				base.IsNotificationEnable = true;
			}
			else
			{
				this.ScrollBarElement.Width = this.ScrollBarSize;
			}
			this.AxisLabels.Placement = PlacementTypes.Right;
			this.AxisLabels.Height = this.ScrollableSize;
			this.CreateAxisLine(this.StartOffset, this.Height - this.EndOffset, this.LineThickness.Value / 2.0, this.LineThickness.Value / 2.0, this.LineThickness.Value, this.Height);
			foreach (ChartGrid current in this.Grids)
			{
				current.Placement = PlacementTypes.Right;
			}
			if (this.AxisTitleElement != null)
			{
				this.AxisTitleElement.InternalHorizontalAlignment = HorizontalAlignment.Right;
				this.AxisTitleElement.InternalVerticalAlignment = VerticalAlignment.Center;
			}
			this.AxisLabels.CreateVisualObject();
			foreach (CustomAxisLabels current2 in this.CustomAxisLabels)
			{
				current2.Placement = PlacementTypes.Right;
				current2.Height = this.ScrollableSize;
				current2.CreateVisualObject();
			}
			this.AxisElementsContainer.Children.Add(this.AxisLine);
			foreach (Ticks current3 in this.Ticks)
			{
				current3.SetParms(PlacementTypes.Right, double.NaN, this.ScrollableSize);
				current3.CreateVisualObject();
				if (current3.Visual != null)
				{
					if (this.Height == this.ScrollableSize)
					{
						if (!this.AxisElementsContainer.Children.Contains(current3.Visual))
						{
							this.AxisElementsContainer.Children.Add(current3.Visual);
						}
					}
					else if (!this.InternalStackPanel.Children.Contains(current3.Visual))
					{
						this.InternalStackPanel.Children.Add(current3.Visual);
					}
				}
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			if (this.AxisLabels.Visual != null)
			{
				double num5;
				if (this.AxisLabels.InternalMinHeight != 0.0 && this.AxisLabels.InternalMinWidth > this.AxisLabels.Visual.Width)
				{
					num5 = this.AxisLabels.InternalMinWidth;
				}
				else
				{
					num5 = this.AxisLabels.Visual.Width;
				}
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxWidth) && this.AxisLabels.InternalMaxWidth < num5)
				{
					this.AxisLabels.Visual.Width = this.AxisLabels.InternalMaxWidth;
				}
				else
				{
					this.AxisLabels.Visual.Width = num5;
				}
				num = Math.Max(num, this.AxisLabels.TopOverflow);
				num2 = Math.Max(num2, this.AxisLabels.BottomOverflow);
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxWidth) && this.AxisLabels.InternalMaxWidth <= num5)
				{
					this.GetNewOverflow4RightLabels(num, num2, ref num3, ref num4, this.AxisLabels.Visual.Width);
				}
				else
				{
					num4 = num2;
					num3 = num;
				}
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = new Rect(-1.0, -4.0 - num3, this.AxisLabels.Visual.Width + 2.0, this.AxisLabels.Visual.Height + num3 + num4 + 8.0);
				this.AxisLabels.Visual.Clip = rectangleGeometry;
			}
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 0.0;
			double num9 = 0.0;
			foreach (CustomAxisLabels current4 in this.CustomAxisLabels)
			{
				if (current4.Visual != null)
				{
					double num10;
					if (current4.InternalMinHeight != 0.0 && current4.InternalMinWidth > current4.Visual.Width)
					{
						num10 = current4.InternalMinWidth;
					}
					else
					{
						num10 = current4.Visual.Width;
					}
					if (!double.IsPositiveInfinity(current4.InternalMaxWidth) && current4.InternalMaxWidth < num10)
					{
						current4.Visual.Width = current4.InternalMaxWidth;
					}
					else
					{
						current4.Visual.Width = num10;
					}
					num6 = Math.Max(num6, current4.TopOverflow);
					num7 = Math.Max(num7, current4.BottomOverflow);
					if (!double.IsPositiveInfinity(current4.InternalMaxWidth) && current4.InternalMaxWidth <= num10)
					{
						this.GetNewOverflow4RightCustomLabels(current4, num6, num7, ref num8, ref num9, current4.Visual.Width);
					}
					else
					{
						num9 = num7;
						num8 = num6;
					}
					RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
					rectangleGeometry2.Rect = new Rect(-1.0, -4.0 - num8, current4.Visual.Width + 2.0, current4.Visual.Height + num8 + num9 + 8.0);
					current4.Visual.Clip = rectangleGeometry2;
				}
			}
			if (num < num6)
			{
				num = num6;
			}
			if (num2 < num7)
			{
				num2 = num7;
			}
			if (this.Height == this.ScrollableSize)
			{
				if (this.AxisLabels.Visual != null)
				{
					this.AxisLabels.Visual.Margin = new Thickness(4.0, 0.0, 0.0, 0.0);
					this.AxisElementsContainer.Children.Add(this.AxisLabels.Visual);
				}
				using (IEnumerator<CustomAxisLabels> enumerator5 = this.CustomAxisLabels.GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						CustomAxisLabels current5 = enumerator5.Current;
						if (current5.Visual != null)
						{
							this.AxisElementsContainer.Children.Add(current5.Visual);
						}
					}
					goto IL_7EC;
				}
			}
			if (this.AxisLabels.Visual != null)
			{
				this.AxisLabels.Visual.Margin = new Thickness(4.0, 0.0, 0.0, 0.0);
			}
			this.InternalStackPanel.Children.Add(this.AxisLabels.Visual);
			foreach (CustomAxisLabels current6 in this.CustomAxisLabels)
			{
				if (current6.Visual != null)
				{
					this.InternalStackPanel.Children.Add(current6.Visual);
				}
			}
			this.ScrollViewerElement.Children.Add(this.InternalStackPanel);
			this.AxisElementsContainer.Children.Add(this.ScrollViewerElement);
			IL_7EC:
			this.CreateAxisTitleVisual(new Thickness(5.0, 0.0, 0.0, 0.0));
			if (this.AxisTitleElement != null)
			{
				this.AxisElementsContainer.Children.Add(this.AxisTitleElement.Visual);
			}
			Size size = Graphics.CalculateVisualSize(this.AxisElementsContainer);
			double num11;
			if (this.InternalMinWidth != 0.0 && this.InternalMinWidth > size.Width)
			{
				num11 = this.InternalMinWidth;
			}
			else
			{
				num11 = size.Width;
			}
			if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth < num11)
			{
				num11 = this.InternalMaxWidth;
			}
			Canvas canvas = new Canvas();
			canvas.HorizontalAlignment = HorizontalAlignment.Left;
			canvas.VerticalAlignment = VerticalAlignment.Stretch;
			canvas.Width = num11;
			canvas.Children.Add(this.AxisElementsContainer);
			if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth <= num11)
			{
				if (this.AxisLabels.Visual != null)
				{
					this.GetNewOverflow4RightLabels(num, num2, ref num3, ref num4, canvas.Width);
				}
				foreach (CustomAxisLabels current7 in this.CustomAxisLabels)
				{
					if (current7.Visual != null)
					{
						this.GetNewOverflow4LeftCustomLabels(current7, num6, num7, ref num8, ref num9, canvas.Width);
					}
				}
				if (num3 < num8)
				{
					num3 = num8;
				}
				if (num4 < num9)
				{
					num4 = num9;
				}
			}
			canvas.Clip = new RectangleGeometry
			{
				Rect = new Rect(-1.0, -4.0 - num3, canvas.Width + 2.0, size.Height + num3 + num4 + 8.0)
			};
			this.Visual.Children.Add(canvas);
			Size size2 = Graphics.CalculateVisualSize(this.Visual);
			this.Visual.Children.Add(new Border
			{
				Width = this.InternalPadding.Right
			});
			size2.Width += this.InternalPadding.Right;
			double sizeFromTrendLineLabels = this.GetSizeFromTrendLineLabels(this);
			if (size2.Width < sizeFromTrendLineLabels)
			{
				(this.Visual.Children[this.Visual.Children.Count - 1] as Border).Width = sizeFromTrendLineLabels - size2.Width;
				size2.Width += (this.Visual.Children[this.Visual.Children.Count - 1] as Border).Width - this.InternalPadding.Right;
			}
			this.Visual.Width = size2.Width;
		}

		private void GetNewOverflow4RightLabels(double topOverflow, double bottomOverflow, ref double newTopOverflow, ref double newBottomOverflow, double visualWidth)
		{
			if (this.AxisLabels.InternalAngle.Value < 0.0)
			{
				double y = this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].Position.Y;
				Point point = new Point(this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].Position.X, this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].Position.Y);
				Point point2 = new Point(this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].ActualLeft - this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].ActualWidth, this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].ActualTop);
				Point point3 = new Point(point.X, point2.Y);
				double num = Graphics.DistanceBetweenTwoPoints(point3, point2);
				double num2 = Graphics.DistanceBetweenTwoPoints(point3, point);
				double a = Math.Atan(num / num2);
				if (num > visualWidth)
				{
					num = visualWidth;
				}
				topOverflow = num / Math.Tan(a);
				topOverflow -= y;
				newTopOverflow = topOverflow;
				this.AxisLabels.TopOverflow = newTopOverflow;
				return;
			}
			if (this.AxisLabels.InternalAngle.Value > 0.0)
			{
				double y2 = this.AxisLabels.AxisLabelList[0].Position.Y;
				Point point4 = new Point(this.AxisLabels.AxisLabelList[0].Position.X, this.AxisLabels.AxisLabelList[0].Position.Y);
				Point point5 = new Point(this.AxisLabels.AxisLabelList[0].ActualLeft, this.AxisLabels.AxisLabelList[0].ActualTop + this.AxisLabels.AxisLabelList[0].ActualHeight);
				Point point6 = new Point(point4.X, point5.Y);
				double num3 = Graphics.DistanceBetweenTwoPoints(point6, point5);
				double num4 = Graphics.DistanceBetweenTwoPoints(point6, point4);
				double a2 = Math.Atan(num3 / num4);
				if (num3 > visualWidth)
				{
					num3 = visualWidth;
				}
				bottomOverflow = num3 / Math.Tan(a2);
				bottomOverflow += y2;
				newBottomOverflow = bottomOverflow - this.Height;
				this.AxisLabels.BottomOverflow = newBottomOverflow;
			}
		}

		private void GetNewOverflow4RightCustomLabels(CustomAxisLabels customLabels, double topOverflow, double bottomOverflow, ref double newTopOverflow, ref double newBottomOverflow, double visualWidth)
		{
			if (customLabels.InternalAngle.Value < 0.0)
			{
				double y = customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].Position.Y;
				Point point = new Point(customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].Position.X, customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].Position.Y);
				Point point2 = new Point(customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].ActualLeft - customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].ActualWidth, customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].ActualTop);
				Point point3 = new Point(point.X, point2.Y);
				double num = Graphics.DistanceBetweenTwoPoints(point3, point2);
				double num2 = Graphics.DistanceBetweenTwoPoints(point3, point);
				double a = Math.Atan(num / num2);
				if (num > visualWidth)
				{
					num = visualWidth;
				}
				topOverflow = num / Math.Tan(a);
				topOverflow -= y;
				newTopOverflow = topOverflow;
				customLabels.TopOverflow = newTopOverflow;
				return;
			}
			if (customLabels.InternalAngle.Value > 0.0)
			{
				double y2 = customLabels.AxisLabelList[0].Position.Y;
				Point point4 = new Point(customLabels.AxisLabelList[0].Position.X, customLabels.AxisLabelList[0].Position.Y);
				Point point5 = new Point(customLabels.AxisLabelList[0].ActualLeft, customLabels.AxisLabelList[0].ActualTop + customLabels.AxisLabelList[0].ActualHeight);
				Point point6 = new Point(point4.X, point5.Y);
				double num3 = Graphics.DistanceBetweenTwoPoints(point6, point5);
				double num4 = Graphics.DistanceBetweenTwoPoints(point6, point4);
				double a2 = Math.Atan(num3 / num4);
				if (num3 > visualWidth)
				{
					num3 = visualWidth;
				}
				bottomOverflow = num3 / Math.Tan(a2);
				bottomOverflow += y2;
				newBottomOverflow = bottomOverflow - this.Height;
				customLabels.BottomOverflow = newBottomOverflow;
			}
		}

		internal static double GetNearestValue(List<double> axisLabelsValues, double targetValue)
		{
			double? num = null;
			double num2 = Math.Abs(targetValue - axisLabelsValues[0]);
			foreach (double num3 in axisLabelsValues)
			{
				if (Math.Abs(targetValue - num3) <= num2)
				{
					num2 = Math.Abs(targetValue - num3);
					num = new double?(num3);
				}
			}
			if (!num.HasValue)
			{
				return 0.0;
			}
			return num.Value;
		}

		internal static double[] GetValuesUnderViewPort(List<double> axisElementValues, Axis axis)
		{
			List<double> list = new List<double>();
			double targetValue;
			double targetValue2;
			if (axis.Logarithmic)
			{
				targetValue = Math.Log(axis._numericViewMinimum, axis.LogarithmBase);
				targetValue2 = Math.Log(axis._numericViewMaximum, axis.LogarithmBase);
			}
			else
			{
				targetValue = axis._numericViewMinimum;
				targetValue2 = axis._numericViewMaximum;
			}
			double nearestValue = Axis.GetNearestValue(axisElementValues, targetValue);
			list.Add(nearestValue);
			double nearestValue2 = Axis.GetNearestValue(axisElementValues, targetValue2);
			list.Add(nearestValue2);
			return list.ToArray();
		}

		private void UpdateAxisLine(double y1, double y2, double x1, double x2, double width, double height)
		{
			this.AxisLine.X1 = x1;
			this.AxisLine.X2 = x2;
			this.AxisLine.Y1 = y1;
			this.AxisLine.Y2 = y2;
			this.AxisLine.Width = width;
			this.AxisLine.Height = height;
		}

		private void UpdateVerticalPrimaryAxisSettings()
		{
			this.AxisLabels.Placement = PlacementTypes.Left;
			this.AxisLabels.Height = this.ScrollableSize;
			this.AxisLabels.UpdateVisualObject();
			foreach (CustomAxisLabels current in this.CustomAxisLabels)
			{
				current.Placement = PlacementTypes.Left;
				current.Height = this.ScrollableSize;
				current.UpdateVisualObject();
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			if (this.AxisLabels.Visual != null)
			{
				double num5;
				if (this.AxisLabels.InternalMinWidth != 0.0 && this.AxisLabels.InternalMinWidth > this.AxisLabels.Visual.Width)
				{
					num5 = this.AxisLabels.InternalMinWidth;
				}
				else
				{
					num5 = this.AxisLabels.Visual.Width;
				}
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxWidth) && this.AxisLabels.InternalMaxWidth < num5)
				{
					this.AxisLabels.Visual.Width = this.AxisLabels.InternalMaxWidth;
				}
				else
				{
					this.AxisLabels.Visual.Width = num5;
				}
				num = Math.Max(num, this.AxisLabels.TopOverflow);
				num2 = Math.Max(num2, this.AxisLabels.BottomOverflow);
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxWidth) && this.AxisLabels.InternalMaxWidth <= num5)
				{
					this.GetNewOverflow4LeftLabels(num, num2, ref num3, ref num4, this.AxisLabels.Visual.Width);
				}
				else
				{
					num4 = num2;
					num3 = num;
				}
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = new Rect(-1.0, -4.0 - num3, this.AxisLabels.Visual.Width + 2.0, this.AxisLabels.Visual.Height + num3 + num4 + 8.0);
				this.AxisLabels.Visual.Clip = rectangleGeometry;
			}
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 0.0;
			double num9 = 0.0;
			List<CustomAxisLabels> list = null;
			if (this.CustomAxisLabels.Count > 0)
			{
				list = this.CustomAxisLabels.ToList<CustomAxisLabels>();
				list.Reverse();
				foreach (CustomAxisLabels current2 in list)
				{
					if (current2.Visual != null)
					{
						double num10;
						if (current2.InternalMinHeight != 0.0 && current2.InternalMinWidth > current2.Visual.Width)
						{
							num10 = current2.InternalMinWidth;
						}
						else
						{
							num10 = current2.Visual.Width;
						}
						if (!double.IsPositiveInfinity(current2.InternalMaxWidth) && current2.InternalMaxWidth < num10)
						{
							current2.Visual.Width = current2.InternalMaxWidth;
						}
						else
						{
							current2.Visual.Width = num10;
						}
						num8 = Math.Max(num8, current2.TopOverflow);
						num9 = Math.Max(num9, current2.BottomOverflow);
						if (!double.IsPositiveInfinity(current2.InternalMaxWidth) && current2.InternalMaxWidth <= num10)
						{
							this.GetNewOverflow4LeftCustomLabels(current2, num8, num9, ref num6, ref num7, current2.Visual.Width);
						}
						else
						{
							num6 = num8;
							num7 = num9;
						}
						RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
						rectangleGeometry2.Rect = new Rect(-1.0, -4.0 - num6, current2.Visual.Width + 2.0, current2.Visual.Height + num6 + num7 + 8.0);
						current2.Visual.Clip = rectangleGeometry2;
					}
				}
			}
			if (num < num8)
			{
				num = num8;
			}
			if (num2 < num9)
			{
				num2 = num9;
			}
			this.UpdateAxisLine(this.StartOffset, this.Height - this.EndOffset, this.LineThickness.Value / 2.0, this.LineThickness.Value / 2.0, this.LineThickness.Value, this.Height);
			foreach (Ticks current3 in this.Ticks)
			{
				current3.SetParms(PlacementTypes.Left, double.NaN, this.ScrollableSize);
				current3.UpdateVisualObject();
			}
			if (this.AxisTitleElement != null)
			{
				if (this.AxisElementsContainer.Children.Contains(this.AxisTitleElement.Visual))
				{
					this.AxisElementsContainer.Children.Remove(this.AxisTitleElement.Visual);
				}
				this.AxisTitleElement.InternalHorizontalAlignment = HorizontalAlignment.Left;
				this.AxisTitleElement.InternalVerticalAlignment = VerticalAlignment.Center;
			}
			this.CreateAxisTitleVisual(new Thickness(0.0, 0.0, 5.0, 0.0));
			if (this.AxisTitleElement != null)
			{
				this.AxisElementsContainer.Children.Insert(0, this.AxisTitleElement.Visual);
			}
			Size size = Graphics.CalculateVisualSize(this.AxisElementsContainer);
			double num11;
			if (this.InternalMinWidth != 0.0 && this.InternalMinWidth > size.Width)
			{
				num11 = this.InternalMinWidth;
			}
			else
			{
				num11 = size.Width;
			}
			if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth < num11)
			{
				num11 = this.InternalMaxWidth;
			}
			(this.AxisElementsContainer.Parent as Canvas).Width = num11;
			if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth <= num11)
			{
				if (this.AxisLabels.Visual != null)
				{
					this.GetNewOverflow4LeftLabels(num, num2, ref num3, ref num4, (this.AxisElementsContainer.Parent as Canvas).Width);
				}
				if (list != null)
				{
					foreach (CustomAxisLabels current4 in list)
					{
						if (current4.Visual != null)
						{
							this.GetNewOverflow4LeftCustomLabels(current4, num8, num9, ref num6, ref num7, (this.AxisElementsContainer.Parent as Canvas).Width);
						}
					}
				}
				if (num3 < num6)
				{
					num3 = num6;
				}
				if (num4 < num7)
				{
					num4 = num7;
				}
			}
			RectangleGeometry rectangleGeometry3 = new RectangleGeometry();
			rectangleGeometry3.Rect = new Rect(-1.0, -4.0 - num3, (this.AxisElementsContainer.Parent as Canvas).Width + 2.0, size.Height + num3 + num4 + 8.0);
			(this.AxisElementsContainer.Parent as Canvas).Clip = rectangleGeometry3;
			Size size2 = default(Size);
			size2.Width = this.InternalPadding.Left + (this.AxisElementsContainer.Parent as Canvas).Width;
			double sizeFromTrendLineLabels = this.GetSizeFromTrendLineLabels(this);
			if (size2.Width < sizeFromTrendLineLabels)
			{
				(this.Visual.Children[0] as Border).Width = sizeFromTrendLineLabels - size2.Width;
				size2.Width += (this.Visual.Children[0] as Border).Width - this.InternalPadding.Left;
			}
			this.Visual.Width = size2.Width;
		}

		private void UpdateVerticalSecondaryAxisSettings()
		{
			this.AxisLabels.Placement = PlacementTypes.Right;
			this.AxisLabels.Height = this.ScrollableSize;
			this.UpdateAxisLine(this.StartOffset, this.Height - this.EndOffset, this.LineThickness.Value / 2.0, this.LineThickness.Value / 2.0, this.LineThickness.Value, this.Height);
			this.AxisLabels.UpdateVisualObject();
			foreach (CustomAxisLabels current in this.CustomAxisLabels)
			{
				current.Placement = PlacementTypes.Right;
				current.Height = this.ScrollableSize;
				current.UpdateVisualObject();
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			if (this.AxisLabels.Visual != null)
			{
				double num5;
				if (this.AxisLabels.InternalMinHeight != 0.0 && this.AxisLabels.InternalMinWidth > this.AxisLabels.Visual.Width)
				{
					num5 = this.AxisLabels.InternalMinWidth;
				}
				else
				{
					num5 = this.AxisLabels.Visual.Width;
				}
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxWidth) && this.AxisLabels.InternalMaxWidth < num5)
				{
					this.AxisLabels.Visual.Width = this.AxisLabels.InternalMaxWidth;
				}
				else
				{
					this.AxisLabels.Visual.Width = num5;
				}
				num = Math.Max(num, this.AxisLabels.TopOverflow);
				num2 = Math.Max(num2, this.AxisLabels.BottomOverflow);
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxWidth) && this.AxisLabels.InternalMaxWidth <= num5)
				{
					this.GetNewOverflow4RightLabels(num, num2, ref num3, ref num4, this.AxisLabels.Visual.Width);
				}
				else
				{
					num4 = num2;
					num3 = num;
				}
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = new Rect(-1.0, -4.0 - num3, this.AxisLabels.Visual.Width + 2.0, this.AxisLabels.Visual.Height + num3 + num4 + 8.0);
				this.AxisLabels.Visual.Clip = rectangleGeometry;
			}
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 0.0;
			double num9 = 0.0;
			foreach (CustomAxisLabels current2 in this.CustomAxisLabels)
			{
				if (current2.Visual != null)
				{
					double num10;
					if (current2.InternalMinHeight != 0.0 && current2.InternalMinWidth > current2.Visual.Width)
					{
						num10 = current2.InternalMinWidth;
					}
					else
					{
						num10 = current2.Visual.Width;
					}
					if (!double.IsPositiveInfinity(current2.InternalMaxWidth) && current2.InternalMaxWidth < num10)
					{
						current2.Visual.Width = current2.InternalMaxWidth;
					}
					else
					{
						current2.Visual.Width = num10;
					}
					num6 = Math.Max(num6, current2.TopOverflow);
					num7 = Math.Max(num7, current2.BottomOverflow);
					if (!double.IsPositiveInfinity(current2.InternalMaxWidth) && current2.InternalMaxWidth <= num10)
					{
						this.GetNewOverflow4RightCustomLabels(current2, num6, num7, ref num8, ref num9, current2.Visual.Width);
					}
					else
					{
						num9 = num7;
						num8 = num6;
					}
					RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
					rectangleGeometry2.Rect = new Rect(-1.0, -4.0 - num8, current2.Visual.Width + 2.0, current2.Visual.Height + num8 + num9 + 8.0);
					current2.Visual.Clip = rectangleGeometry2;
				}
			}
			if (num < num6)
			{
				num = num6;
			}
			if (num2 < num7)
			{
				num2 = num7;
			}
			foreach (Ticks current3 in this.Ticks)
			{
				current3.SetParms(PlacementTypes.Right, double.NaN, this.ScrollableSize);
				current3.UpdateVisualObject();
			}
			if (this.AxisTitleElement != null)
			{
				if (this.AxisElementsContainer.Children.Contains(this.AxisTitleElement.Visual))
				{
					this.AxisElementsContainer.Children.Remove(this.AxisTitleElement.Visual);
				}
				this.AxisTitleElement.InternalHorizontalAlignment = HorizontalAlignment.Right;
				this.AxisTitleElement.InternalVerticalAlignment = VerticalAlignment.Center;
			}
			this.CreateAxisTitleVisual(new Thickness(5.0, 0.0, 0.0, 0.0));
			if (this.AxisTitleElement != null)
			{
				this.AxisElementsContainer.Children.Add(this.AxisTitleElement.Visual);
			}
			Size size = Graphics.CalculateVisualSize(this.AxisElementsContainer);
			double num11;
			if (this.InternalMinWidth != 0.0 && this.InternalMinWidth > size.Width)
			{
				num11 = this.InternalMinWidth;
			}
			else
			{
				num11 = size.Width;
			}
			if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth < num11)
			{
				num11 = this.InternalMaxWidth;
			}
			(this.AxisElementsContainer.Parent as Canvas).Width = num11;
			if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth <= num11)
			{
				if (this.AxisLabels.Visual != null)
				{
					this.GetNewOverflow4RightLabels(num, num2, ref num3, ref num4, (this.AxisElementsContainer.Parent as Canvas).Width);
				}
				foreach (CustomAxisLabels current4 in this.CustomAxisLabels)
				{
					if (current4.Visual != null)
					{
						this.GetNewOverflow4LeftCustomLabels(current4, num6, num7, ref num8, ref num9, (this.AxisElementsContainer.Parent as Canvas).Width);
					}
				}
				if (num3 < num8)
				{
					num3 = num8;
				}
				if (num4 < num9)
				{
					num4 = num9;
				}
			}
			RectangleGeometry rectangleGeometry3 = new RectangleGeometry();
			rectangleGeometry3.Rect = new Rect(-1.0, -4.0 - num3, (this.AxisElementsContainer.Parent as Canvas).Width + 2.0, size.Height + num3 + num4 + 8.0);
			(this.AxisElementsContainer.Parent as Canvas).Clip = rectangleGeometry3;
			Size size2 = default(Size);
			size2.Width = (this.AxisElementsContainer.Parent as Canvas).Width + this.InternalPadding.Right;
			double sizeFromTrendLineLabels = this.GetSizeFromTrendLineLabels(this);
			if (size2.Width < sizeFromTrendLineLabels)
			{
				(this.Visual.Children[this.Visual.Children.Count - 1] as Border).Width = sizeFromTrendLineLabels - size2.Width;
				size2.Width += (this.Visual.Children[this.Visual.Children.Count - 1] as Border).Width - this.InternalPadding.Right;
			}
			this.Visual.Width = size2.Width;
		}

		private void UpdateHorizontalPrimaryAxisSettings()
		{
			this.AxisLabels.Placement = PlacementTypes.Bottom;
			this.AxisLabels.Width = this.ScrollableSize;
			this.AxisLabels.UpdateVisualObject();
			foreach (CustomAxisLabels current in this.CustomAxisLabels)
			{
				current.Placement = PlacementTypes.Bottom;
				current.Width = this.ScrollableSize;
				current.UpdateVisualObject();
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			if (this.AxisLabels.Visual != null)
			{
				double num5;
				if (this.AxisLabels.InternalMinHeight != 0.0 && this.AxisLabels.InternalMinHeight > this.AxisLabels.Visual.Height)
				{
					num5 = this.AxisLabels.InternalMinHeight;
				}
				else
				{
					num5 = this.AxisLabels.Visual.Height;
				}
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxHeight) && this.AxisLabels.InternalMaxHeight < num5)
				{
					this.AxisLabels.Visual.Height = this.AxisLabels.InternalMaxHeight;
				}
				else
				{
					this.AxisLabels.Visual.Height = num5;
				}
				num3 = Math.Max(num3, this.AxisLabels.LeftOverflow);
				num4 = Math.Max(num4, this.AxisLabels.RightOverflow);
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxHeight) && this.AxisLabels.InternalMaxHeight <= num5)
				{
					this.GetNewOverflow4BottomLabels(num3, num4, ref num, ref num2, this.AxisLabels.Visual.Height);
				}
				else
				{
					num = num3;
					num2 = num4;
				}
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = new Rect(-4.0 - num, 0.0, this.AxisLabels.Visual.Width + num + num2 + 8.0, this.AxisLabels.Visual.Height);
				this.AxisLabels.Visual.Clip = rectangleGeometry;
			}
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 0.0;
			double num9 = 0.0;
			foreach (CustomAxisLabels current2 in this.CustomAxisLabels)
			{
				if (current2.Visual != null)
				{
					double num10;
					if (current2.InternalMinHeight != 0.0 && current2.InternalMinHeight > current2.Visual.Height)
					{
						num10 = current2.InternalMinHeight;
					}
					else
					{
						num10 = current2.Visual.Height;
					}
					if (!double.IsPositiveInfinity(current2.InternalMaxHeight) && current2.InternalMaxHeight < num10)
					{
						current2.Visual.Height = current2.InternalMaxHeight;
					}
					else
					{
						current2.Visual.Height = num10;
					}
					num8 = Math.Max(num8, current2.LeftOverflow);
					num9 = Math.Max(num9, current2.RightOverflow);
					if (!double.IsPositiveInfinity(current2.InternalMaxHeight) && current2.InternalMaxHeight <= num10)
					{
						this.GetNewOverflow4BottomCustomLabels(current2, num8, num9, ref num6, ref num7, current2.Visual.Height);
					}
					else
					{
						num6 = num8;
						num7 = num9;
					}
					RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
					rectangleGeometry2.Rect = new Rect(-4.0 - num6, 0.0, current2.Visual.Width + num6 + num7 + 8.0, current2.Visual.Height);
					current2.Visual.Clip = rectangleGeometry2;
				}
			}
			if (num3 < num8)
			{
				num3 = num8;
			}
			if (num4 < num9)
			{
				num4 = num9;
			}
			this.UpdateAxisLine(this.LineThickness.Value / 2.0, this.LineThickness.Value / 2.0, this.StartOffset, this.Width - this.EndOffset, this.Width, this.LineThickness.Value);
			foreach (Ticks current3 in this.Ticks)
			{
				current3.SetParms(PlacementTypes.Bottom, this.ScrollableSize, double.NaN);
				current3.UpdateVisualObject();
			}
			if (this.AxisTitleElement != null)
			{
				if (this.AxisElementsContainer.Children.Contains(this.AxisTitleElement.Visual))
				{
					this.AxisElementsContainer.Children.Remove(this.AxisTitleElement.Visual);
				}
				this.AxisTitleElement.InternalHorizontalAlignment = HorizontalAlignment.Center;
				this.AxisTitleElement.InternalVerticalAlignment = VerticalAlignment.Bottom;
			}
			this.CreateAxisTitleVisual(new Thickness(0.0, 5.0, 0.0, 0.0));
			if (this.AxisTitleElement != null)
			{
				this.AxisElementsContainer.Children.Add(this.AxisTitleElement.Visual);
			}
			Size size = Graphics.CalculateVisualSize(this.AxisElementsContainer);
			double num11;
			if (this.InternalMinHeight != 0.0 && this.InternalMinHeight > size.Height)
			{
				num11 = this.InternalMinHeight;
			}
			else
			{
				num11 = size.Height;
			}
			if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight < num11)
			{
				num11 = this.InternalMaxHeight;
			}
			(this.AxisElementsContainer.Parent as Canvas).Height = num11;
			if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight <= num11)
			{
				if (this.AxisLabels.Visual != null)
				{
					this.GetNewOverflow4BottomLabels(num3, num4, ref num, ref num2, (this.AxisElementsContainer.Parent as Canvas).Height);
				}
				foreach (CustomAxisLabels current4 in this.CustomAxisLabels)
				{
					if (current4.Visual != null)
					{
						this.GetNewOverflow4BottomCustomLabels(current4, num8, num9, ref num6, ref num7, (this.AxisElementsContainer.Parent as Canvas).Height);
					}
				}
				if (num < num6)
				{
					num = num6;
				}
				if (num2 < num7)
				{
					num2 = num7;
				}
			}
			else
			{
				num = num3;
				num2 = num4;
			}
			RectangleGeometry rectangleGeometry3 = new RectangleGeometry();
			rectangleGeometry3.Rect = new Rect(-4.0 - num, 0.0, (this.AxisElementsContainer.Parent as Canvas).Width + num + num2 + 8.0, (this.AxisElementsContainer.Parent as Canvas).Height);
			(this.AxisElementsContainer.Parent as Canvas).Clip = rectangleGeometry3;
			Size size2 = default(Size);
			size2.Height = (this.AxisElementsContainer.Parent as Canvas).Height + this.InternalPadding.Bottom;
			double sizeFromTrendLineLabels = this.GetSizeFromTrendLineLabels(this);
			if (size2.Height < sizeFromTrendLineLabels)
			{
				(this.Visual.Children[this.Visual.Children.Count - 1] as Border).Height = sizeFromTrendLineLabels - size2.Height;
				size2.Height += (this.Visual.Children[this.Visual.Children.Count - 1] as Border).Height - this.InternalPadding.Bottom;
			}
			this.Visual.Height = size2.Height;
		}

		private void UpdateHorizontalSecondaryAxisSettings()
		{
			this.AxisLabels.Placement = PlacementTypes.Top;
			this.AxisLabels.Width = this.ScrollableSize;
			this.UpdateAxisLine(this.LineThickness.Value / 2.0, this.LineThickness.Value / 2.0, this.StartOffset, this.Width - this.EndOffset, this.Width, this.LineThickness.Value);
			this.AxisLabels.UpdateVisualObject();
			foreach (CustomAxisLabels current in this.CustomAxisLabels)
			{
				current.Placement = PlacementTypes.Top;
				current.Width = this.ScrollableSize;
				current.UpdateVisualObject();
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			if (this.AxisLabels.Visual != null)
			{
				double num5;
				if (this.AxisLabels.InternalMinHeight != 0.0 && this.AxisLabels.InternalMinHeight > this.AxisLabels.Visual.Height)
				{
					num5 = this.AxisLabels.InternalMinHeight;
				}
				else
				{
					num5 = this.AxisLabels.Visual.Height;
				}
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxHeight) && this.AxisLabels.InternalMaxHeight < num5)
				{
					this.AxisLabels.Visual.Height = this.AxisLabels.InternalMaxHeight;
				}
				else
				{
					this.AxisLabels.Visual.Height = num5;
				}
				num = Math.Max(num, this.AxisLabels.LeftOverflow);
				num2 = Math.Max(num2, this.AxisLabels.RightOverflow);
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxHeight) && this.AxisLabels.InternalMaxHeight <= num5)
				{
					this.GetNewOverflow4TopLabels(num, num2, ref num3, ref num4, this.AxisLabels.Visual.Height);
				}
				else
				{
					num3 = num;
					num4 = num2;
				}
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = new Rect(-4.0 - num3, 0.0, this.AxisLabels.Visual.Width + num3 + num4 + 8.0, this.AxisLabels.Visual.Height);
				this.AxisLabels.Visual.Clip = rectangleGeometry;
			}
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 0.0;
			double num9 = 0.0;
			List<CustomAxisLabels> list = null;
			if (this.CustomAxisLabels.Count > 0)
			{
				list = this.CustomAxisLabels.ToList<CustomAxisLabels>();
				list.Reverse();
				foreach (CustomAxisLabels current2 in list)
				{
					if (current2.Visual != null)
					{
						double num10;
						if (current2.InternalMinHeight != 0.0 && current2.InternalMinHeight > current2.Visual.Height)
						{
							num10 = current2.InternalMinHeight;
						}
						else
						{
							num10 = current2.Visual.Height;
						}
						if (!double.IsPositiveInfinity(current2.InternalMaxHeight) && current2.InternalMaxHeight < num10)
						{
							current2.Visual.Height = current2.InternalMaxHeight;
						}
						else
						{
							current2.Visual.Height = num10;
						}
						num6 = Math.Max(num6, current2.LeftOverflow);
						num7 = Math.Max(num7, current2.RightOverflow);
						if (!double.IsPositiveInfinity(current2.InternalMaxHeight) && current2.InternalMaxHeight <= num10)
						{
							this.GetNewOverflow4TopCustomLabels(current2, num6, num7, ref num8, ref num9, current2.Visual.Height);
						}
						else
						{
							num8 = num6;
							num9 = num7;
						}
						RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
						rectangleGeometry2.Rect = new Rect(-4.0 - num8, -1.0, current2.Visual.Width + num8 + num9 + 8.0, current2.Visual.Height + 2.0);
						current2.Visual.Clip = rectangleGeometry2;
					}
				}
			}
			if (num < num6)
			{
				num = num6;
			}
			if (num2 < num7)
			{
				num2 = num7;
			}
			List<Ticks> list2 = this.Ticks.Reverse<Ticks>().ToList<Ticks>();
			foreach (Ticks current3 in list2)
			{
				current3.SetParms(PlacementTypes.Top, this.ScrollableSize, double.NaN);
				current3.UpdateVisualObject();
			}
			if (this.AxisTitleElement != null)
			{
				if (this.AxisElementsContainer.Children.Contains(this.AxisTitleElement.Visual))
				{
					this.AxisElementsContainer.Children.Remove(this.AxisTitleElement.Visual);
				}
				this.AxisTitleElement.InternalHorizontalAlignment = HorizontalAlignment.Center;
				this.AxisTitleElement.InternalVerticalAlignment = VerticalAlignment.Top;
			}
			this.CreateAxisTitleVisual(new Thickness(0.0, 0.0, 0.0, 5.0));
			if (this.AxisTitleElement != null)
			{
				this.AxisElementsContainer.Children.Insert(0, this.AxisTitleElement.Visual);
			}
			Size size = Graphics.CalculateVisualSize(this.AxisElementsContainer);
			double num11;
			if (this.InternalMinHeight != 0.0 && this.InternalMinHeight > size.Height)
			{
				num11 = this.InternalMinHeight;
			}
			else
			{
				num11 = size.Height;
			}
			if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight < num11)
			{
				num11 = this.InternalMaxHeight;
			}
			(this.AxisElementsContainer.Parent as Canvas).Height = num11;
			if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight <= num11)
			{
				if (this.AxisLabels.Visual != null)
				{
					this.GetNewOverflow4TopLabels(num, num2, ref num3, ref num4, (this.AxisElementsContainer.Parent as Canvas).Height);
				}
				if (list != null)
				{
					foreach (CustomAxisLabels current4 in list)
					{
						if (current4.Visual != null)
						{
							this.GetNewOverflow4TopCustomLabels(current4, num6, num7, ref num8, ref num9, (this.AxisElementsContainer.Parent as Canvas).Height);
						}
					}
				}
				if (num3 < num8)
				{
					num3 = num8;
				}
				if (num4 < num9)
				{
					num4 = num9;
				}
			}
			else
			{
				num3 = num;
				num4 = num2;
			}
			RectangleGeometry rectangleGeometry3 = new RectangleGeometry();
			rectangleGeometry3.Rect = new Rect(-4.0 - num3, 0.0, (this.AxisElementsContainer.Parent as Canvas).Width + num3 + num4 + 8.0, size.Height);
			(this.AxisElementsContainer.Parent as Canvas).Clip = rectangleGeometry3;
			Size size2 = default(Size);
			size2.Height = this.InternalPadding.Top + (this.AxisElementsContainer.Parent as Canvas).Height;
			double sizeFromTrendLineLabels = this.GetSizeFromTrendLineLabels(this);
			if (size2.Height < sizeFromTrendLineLabels)
			{
				(this.Visual.Children[0] as Border).Height = sizeFromTrendLineLabels - size2.Height;
			}
			size2 = Graphics.CalculateVisualSize(this.Visual);
			this.Visual.Height = size2.Height;
		}

		private void ApplyHorizontalPrimaryAxisSettings()
		{
			this.AxisElementsContainer.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.AxisElementsContainer.VerticalAlignment = VerticalAlignment.Bottom;
			this.AxisElementsContainer.Orientation = Orientation.Vertical;
			this.InternalStackPanel.Height = 0.0;
			this.InternalStackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.InternalStackPanel.VerticalAlignment = VerticalAlignment.Bottom;
			this.ScrollViewerElement.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.ScrollBarElement.Orientation = Orientation.Horizontal;
			if (double.IsNaN(this.ScrollBarSize))
			{
				base.IsNotificationEnable = false;
				this.ScrollBarSize = this.ScrollBarElement.Height;
				base.IsNotificationEnable = true;
			}
			else
			{
				this.ScrollBarElement.Height = this.ScrollBarSize;
			}
			this.AxisLabels.Placement = PlacementTypes.Bottom;
			this.AxisLabels.Width = this.ScrollableSize;
			this.CreateAxisLine(this.LineThickness.Value / 2.0, this.LineThickness.Value / 2.0, this.StartOffset, this.Width - this.EndOffset, this.Width, this.LineThickness.Value);
			foreach (ChartGrid current in this.Grids)
			{
				current.Placement = PlacementTypes.Bottom;
			}
			if (this.AxisTitleElement != null)
			{
				this.AxisTitleElement.InternalHorizontalAlignment = HorizontalAlignment.Center;
				this.AxisTitleElement.InternalVerticalAlignment = VerticalAlignment.Bottom;
			}
			this.AxisLabels.CreateVisualObject();
			foreach (CustomAxisLabels current2 in this.CustomAxisLabels)
			{
				current2.Placement = PlacementTypes.Bottom;
				current2.Width = this.ScrollableSize;
				current2.CreateVisualObject();
			}
			this.AxisElementsContainer.Children.Add(this.AxisLine);
			double num = 0.0;
			foreach (Ticks current3 in this.Ticks)
			{
				current3.SetParms(PlacementTypes.Bottom, this.ScrollableSize, double.NaN);
				current3.CreateVisualObject();
				if (current3.Visual != null)
				{
					if (this.Width == this.ScrollableSize)
					{
						if (!this.AxisElementsContainer.Children.Contains(current3.Visual))
						{
							this.AxisElementsContainer.Children.Add(current3.Visual);
						}
					}
					else
					{
						if (!this.InternalStackPanel.Children.Contains(current3.Visual))
						{
							this.InternalStackPanel.Children.Add(current3.Visual);
						}
						current3.Visual.SetValue(Canvas.TopProperty, num);
						num += current3.Visual.Height;
					}
				}
			}
			this.InternalStackPanel.Height += num;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			if (this.AxisLabels.Visual != null)
			{
				double num6;
				if (this.AxisLabels.InternalMinHeight != 0.0 && this.AxisLabels.InternalMinHeight > this.AxisLabels.Visual.Height)
				{
					num6 = this.AxisLabels.InternalMinHeight;
				}
				else
				{
					num6 = this.AxisLabels.Visual.Height;
				}
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxHeight) && this.AxisLabels.InternalMaxHeight < num6)
				{
					this.AxisLabels.Visual.Height = this.AxisLabels.InternalMaxHeight;
				}
				else
				{
					this.AxisLabels.Visual.Height = num6;
				}
				num4 = Math.Max(num4, this.AxisLabels.LeftOverflow);
				num5 = Math.Max(num5, this.AxisLabels.RightOverflow);
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxHeight) && this.AxisLabels.InternalMaxHeight <= num6)
				{
					this.GetNewOverflow4BottomLabels(num4, num5, ref num2, ref num3, this.AxisLabels.Visual.Height);
				}
				else
				{
					num2 = num4;
					num3 = num5;
				}
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = new Rect(-4.0 - num2, 0.0, this.AxisLabels.Visual.Width + num2 + num3 + 8.0, this.AxisLabels.Visual.Height);
				this.AxisLabels.Visual.Clip = rectangleGeometry;
			}
			double num7 = 0.0;
			double num8 = 0.0;
			double num9 = 0.0;
			double num10 = 0.0;
			foreach (CustomAxisLabels current4 in this.CustomAxisLabels)
			{
				if (current4.Visual != null)
				{
					double num11;
					if (current4.InternalMinHeight != 0.0 && current4.InternalMinHeight > current4.Visual.Height)
					{
						num11 = current4.InternalMinHeight;
					}
					else
					{
						num11 = current4.Visual.Height;
					}
					if (!double.IsPositiveInfinity(current4.InternalMaxHeight) && current4.InternalMaxHeight < num11)
					{
						current4.Visual.Height = current4.InternalMaxHeight;
					}
					else
					{
						current4.Visual.Height = num11;
					}
					num9 = Math.Max(num9, current4.LeftOverflow);
					num10 = Math.Max(num10, current4.RightOverflow);
					if (!double.IsPositiveInfinity(current4.InternalMaxHeight) && current4.InternalMaxHeight <= num11)
					{
						this.GetNewOverflow4BottomCustomLabels(current4, num9, num10, ref num7, ref num8, current4.Visual.Height);
					}
					else
					{
						num7 = num9;
						num8 = num10;
					}
					RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
					rectangleGeometry2.Rect = new Rect(-4.0 - num7, 0.0, current4.Visual.Width + num7 + num8 + 8.0, current4.Visual.Height);
					current4.Visual.Clip = rectangleGeometry2;
				}
			}
			if (num4 < num9)
			{
				num4 = num9;
			}
			if (num5 < num10)
			{
				num5 = num10;
			}
			if (this.Width == this.ScrollableSize)
			{
				if (this.AxisLabels.Visual != null)
				{
					this.AxisLabels.Visual.Margin = new Thickness(0.0, 4.0, 0.0, 0.0);
					this.AxisElementsContainer.Children.Add(this.AxisLabels.Visual);
				}
				using (IEnumerator<CustomAxisLabels> enumerator5 = this.CustomAxisLabels.GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						CustomAxisLabels current5 = enumerator5.Current;
						if (current5.Visual != null)
						{
							this.AxisElementsContainer.Children.Add(current5.Visual);
						}
					}
					goto IL_901;
				}
			}
			if (this.AxisLabels.Visual != null)
			{
				this.AxisLabels.Visual.Margin = new Thickness(0.0, 4.0, 0.0, 0.0);
				this.AxisLabels.Visual.Height += 4.0;
				this.InternalStackPanel.Width = this.AxisLabels.Visual.Width;
				this.AxisLabels.Visual.SetValue(Canvas.TopProperty, this.InternalStackPanel.Height);
				this.InternalStackPanel.Children.Add(this.AxisLabels.Visual);
				this.InternalStackPanel.Height += this.AxisLabels.Visual.Height;
			}
			foreach (CustomAxisLabels current6 in this.CustomAxisLabels)
			{
				if (current6.Visual != null)
				{
					this.InternalStackPanel.Width = current6.Visual.Width;
					current6.Visual.SetValue(Canvas.TopProperty, this.InternalStackPanel.Height);
					this.InternalStackPanel.Children.Add(current6.Visual);
					this.InternalStackPanel.Height += current6.Visual.Height;
				}
			}
			this.ScrollViewerElement.Children.Add(this.InternalStackPanel);
			this.AxisElementsContainer.Children.Add(this.ScrollViewerElement);
			IL_901:
			this.ScrollViewerElement.Height = this.InternalStackPanel.Height;
			this.ClipHorizontalAxis(num);
			this.CreateAxisTitleVisual(new Thickness(0.0, 5.0, 0.0, 0.0));
			if (this.AxisTitleElement != null)
			{
				this.AxisElementsContainer.Children.Add(this.AxisTitleElement.Visual);
			}
			Size size = Graphics.CalculateVisualSize(this.AxisElementsContainer);
			double num12;
			if (this.InternalMinHeight != 0.0 && this.InternalMinHeight > size.Height)
			{
				num12 = this.InternalMinHeight;
			}
			else
			{
				num12 = size.Height;
			}
			if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight < num12)
			{
				num12 = this.InternalMaxHeight;
			}
			Canvas canvas = new Canvas();
			canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
			canvas.VerticalAlignment = VerticalAlignment.Bottom;
			canvas.Height = num12;
			canvas.Width = this.Width;
			canvas.Children.Add(this.AxisElementsContainer);
			if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight <= num12)
			{
				if (this.AxisLabels.Visual != null)
				{
					this.GetNewOverflow4BottomLabels(num4, num5, ref num2, ref num3, canvas.Height);
				}
				foreach (CustomAxisLabels current7 in this.CustomAxisLabels)
				{
					if (current7.Visual != null)
					{
						this.GetNewOverflow4BottomCustomLabels(current7, num9, num10, ref num7, ref num8, canvas.Height);
					}
				}
				if (num2 < num7)
				{
					num2 = num7;
				}
				if (num3 < num8)
				{
					num3 = num8;
				}
			}
			else
			{
				num2 = num4;
				num3 = num5;
			}
			canvas.Clip = new RectangleGeometry
			{
				Rect = new Rect(-4.0 - num2, 0.0, canvas.Width + num2 + num3 + 8.0, canvas.Height)
			};
			this.Visual.Children.Add(canvas);
			this.Visual.Children.Add(new Border
			{
				Height = this.InternalPadding.Bottom
			});
			Size size2 = Graphics.CalculateVisualSize(this.Visual);
			double sizeFromTrendLineLabels = this.GetSizeFromTrendLineLabels(this);
			if (size2.Height < sizeFromTrendLineLabels)
			{
				(this.Visual.Children[this.Visual.Children.Count - 1] as Border).Height = sizeFromTrendLineLabels - size2.Height;
				size2.Height += (this.Visual.Children[this.Visual.Children.Count - 1] as Border).Height - this.InternalPadding.Bottom;
			}
			this.Visual.Height = size2.Height;
		}

		private void GetNewOverflow4BottomLabels(double leftOverflow, double rightOverflow, ref double newLeftOverflow, ref double newRightOverflow, double visualHeight)
		{
			if (this.AxisLabels.InternalAngle.Value < 0.0)
			{
				double x = this.AxisLabels.AxisLabelList[0].Position.X;
				Point point = new Point(this.AxisLabels.AxisLabelList[0].Position.X, this.AxisLabels.AxisLabelList[0].Position.Y);
				Point point2 = new Point(this.AxisLabels.AxisLabelList[0].ActualLeft, this.AxisLabels.AxisLabelList[0].ActualHeight);
				Point point3 = new Point(point2.X, point.Y);
				double num = Graphics.DistanceBetweenTwoPoints(point3, point2);
				double num2 = Graphics.DistanceBetweenTwoPoints(point3, point);
				double a = Math.Atan(num / num2);
				if (num > visualHeight)
				{
					num = visualHeight;
				}
				leftOverflow = num / Math.Tan(a);
				leftOverflow -= x;
				newLeftOverflow = leftOverflow;
				this.AxisLabels.LeftOverflow = newLeftOverflow;
				return;
			}
			if (this.AxisLabels.InternalAngle.Value > 0.0)
			{
				double x2 = this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].Position.X;
				Point point4 = new Point(this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].Position.X, this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].Position.Y);
				Point point5 = new Point(this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].ActualLeft + this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].ActualWidth, this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].ActualHeight);
				Point point6 = new Point(point5.X, point4.Y);
				double num3 = Graphics.DistanceBetweenTwoPoints(point6, point5);
				double num4 = Graphics.DistanceBetweenTwoPoints(point6, point4);
				double a2 = Math.Atan(num3 / num4);
				if (num3 > visualHeight)
				{
					num3 = visualHeight;
				}
				rightOverflow = num3 / Math.Tan(a2);
				rightOverflow += x2;
				rightOverflow -= this.Width;
				newRightOverflow = rightOverflow;
				this.AxisLabels.RightOverflow = newRightOverflow;
			}
		}

		private void GetNewOverflow4BottomCustomLabels(CustomAxisLabels customLabels, double leftOverflow, double rightOverflow, ref double newLeftOverflow, ref double newRightOverflow, double visualHeight)
		{
			if (customLabels.InternalAngle.Value < 0.0)
			{
				double x = customLabels.AxisLabelList[0].Position.X;
				Point point = new Point(customLabels.AxisLabelList[0].Position.X, customLabels.AxisLabelList[0].Position.Y);
				Point point2 = new Point(customLabels.AxisLabelList[0].ActualLeft, customLabels.AxisLabelList[0].ActualHeight);
				Point point3 = new Point(point2.X, point.Y);
				double num = Graphics.DistanceBetweenTwoPoints(point3, point2);
				double num2 = Graphics.DistanceBetweenTwoPoints(point3, point);
				double a = Math.Atan(num / num2);
				if (num > visualHeight)
				{
					num = visualHeight;
				}
				leftOverflow = num / Math.Tan(a);
				leftOverflow -= x;
				newLeftOverflow = leftOverflow;
				customLabels.LeftOverflow = newLeftOverflow;
				return;
			}
			if (customLabels.InternalAngle.Value > 0.0)
			{
				double x2 = customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].Position.X;
				Point point4 = new Point(customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].Position.X, customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].Position.Y);
				Point point5 = new Point(customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].ActualLeft + customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].ActualWidth, customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].ActualHeight);
				Point point6 = new Point(point5.X, point4.Y);
				double num3 = Graphics.DistanceBetweenTwoPoints(point6, point5);
				double num4 = Graphics.DistanceBetweenTwoPoints(point6, point4);
				double a2 = Math.Atan(num3 / num4);
				if (num3 > visualHeight)
				{
					num3 = visualHeight;
				}
				rightOverflow = num3 / Math.Tan(a2);
				rightOverflow += x2;
				rightOverflow -= this.Width;
				newRightOverflow = rightOverflow;
				customLabels.RightOverflow = newRightOverflow;
			}
		}

		private void ClipHorizontalAxis(double ticksHeight)
		{
			if (this.Width != this.ScrollableSize)
			{
				double num = 4.0;
				PathGeometry pathGeometry = new PathGeometry();
				pathGeometry.Figures = new PathFigureCollection();
				PathFigure pathFigure = new PathFigure();
				pathFigure.StartPoint = new Point(0.0, 0.0);
				pathFigure.Segments = new PathSegmentCollection();
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(this.Width, 0.0)));
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(this.Width, ticksHeight)));
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(this.Width + num, ticksHeight)));
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(this.Width + num, this.ScrollViewerElement.Height)));
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(-num, this.ScrollViewerElement.Height)));
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(-num, ticksHeight)));
				pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(0.0, ticksHeight)));
				pathGeometry.Figures.Add(pathFigure);
				this.ScrollViewerElement.Clip = pathGeometry;
			}
		}

		private void ApplyHorizontalSecondaryAxisSettings()
		{
			this.Visual.Children.Add(new Border
			{
				Height = this.InternalPadding.Top
			});
			this.AxisElementsContainer.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.AxisElementsContainer.VerticalAlignment = VerticalAlignment.Top;
			this.AxisElementsContainer.Orientation = Orientation.Vertical;
			this.InternalStackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.InternalStackPanel.VerticalAlignment = VerticalAlignment.Top;
			this.InternalStackPanel.SizeChanged += delegate(object sender, SizeChangedEventArgs e)
			{
				this.ScrollViewerElement.Height = e.NewSize.Height;
			};
			this.ScrollViewerElement.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.ScrollBarElement.Orientation = Orientation.Horizontal;
			if (double.IsNaN(this.ScrollBarSize))
			{
				base.IsNotificationEnable = false;
				this.ScrollBarSize = this.ScrollBarElement.Height;
				base.IsNotificationEnable = true;
			}
			else
			{
				this.ScrollBarElement.Height = this.ScrollBarSize;
			}
			this.AxisLabels.Placement = PlacementTypes.Top;
			this.AxisLabels.Width = this.ScrollableSize;
			this.CreateAxisLine(this.LineThickness.Value / 2.0, this.LineThickness.Value / 2.0, this.StartOffset, this.Width - this.EndOffset, this.Width, this.LineThickness.Value);
			foreach (ChartGrid current in this.Grids)
			{
				current.Placement = PlacementTypes.Top;
			}
			if (this.AxisTitleElement != null)
			{
				this.AxisTitleElement.InternalHorizontalAlignment = HorizontalAlignment.Center;
				this.AxisTitleElement.InternalVerticalAlignment = VerticalAlignment.Top;
			}
			this.AxisLabels.CreateVisualObject();
			foreach (CustomAxisLabels current2 in this.CustomAxisLabels)
			{
				current2.Placement = PlacementTypes.Top;
				current2.Width = this.ScrollableSize;
				current2.CreateVisualObject();
			}
			this.CreateAxisTitleVisual(new Thickness(0.0, 0.0, 0.0, 5.0));
			if (this.AxisTitleElement != null)
			{
				this.AxisElementsContainer.Children.Add(this.AxisTitleElement.Visual);
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			if (this.AxisLabels.Visual != null)
			{
				double num5;
				if (this.AxisLabels.InternalMinHeight != 0.0 && this.AxisLabels.InternalMinHeight > this.AxisLabels.Visual.Height)
				{
					num5 = this.AxisLabels.InternalMinHeight;
				}
				else
				{
					num5 = this.AxisLabels.Visual.Height;
				}
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxHeight) && this.AxisLabels.InternalMaxHeight < num5)
				{
					this.AxisLabels.Visual.Height = this.AxisLabels.InternalMaxHeight;
				}
				else
				{
					this.AxisLabels.Visual.Height = num5;
				}
				num = Math.Max(num, this.AxisLabels.LeftOverflow);
				num2 = Math.Max(num2, this.AxisLabels.RightOverflow);
				if (!double.IsPositiveInfinity(this.AxisLabels.InternalMaxHeight) && this.AxisLabels.InternalMaxHeight <= num5)
				{
					this.GetNewOverflow4TopLabels(num, num2, ref num3, ref num4, this.AxisLabels.Visual.Height);
				}
				else
				{
					num3 = num;
					num4 = num2;
				}
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = new Rect(-4.0 - num3, 0.0, this.AxisLabels.Visual.Width + num3 + num4 + 8.0, this.AxisLabels.Visual.Height);
				this.AxisLabels.Visual.Clip = rectangleGeometry;
			}
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 0.0;
			double num9 = 0.0;
			List<CustomAxisLabels> list = null;
			if (this.CustomAxisLabels.Count > 0)
			{
				list = this.CustomAxisLabels.ToList<CustomAxisLabels>();
				list.Reverse();
				foreach (CustomAxisLabels current3 in list)
				{
					if (current3.Visual != null)
					{
						double num10;
						if (current3.InternalMinHeight != 0.0 && current3.InternalMinHeight > current3.Visual.Height)
						{
							num10 = current3.InternalMinHeight;
						}
						else
						{
							num10 = current3.Visual.Height;
						}
						if (!double.IsPositiveInfinity(current3.InternalMaxHeight) && current3.InternalMaxHeight < num10)
						{
							current3.Visual.Height = current3.InternalMaxHeight;
						}
						else
						{
							current3.Visual.Height = num10;
						}
						num6 = Math.Max(num6, current3.LeftOverflow);
						num7 = Math.Max(num7, current3.RightOverflow);
						if (!double.IsPositiveInfinity(current3.InternalMaxHeight) && current3.InternalMaxHeight <= num10)
						{
							this.GetNewOverflow4TopCustomLabels(current3, num6, num7, ref num8, ref num9, current3.Visual.Height);
						}
						else
						{
							num8 = num6;
							num9 = num7;
						}
						RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
						rectangleGeometry2.Rect = new Rect(-4.0 - num8, -1.0, current3.Visual.Width + num8 + num9 + 8.0, current3.Visual.Height + 2.0);
						current3.Visual.Clip = rectangleGeometry2;
					}
				}
			}
			if (num < num6)
			{
				num = num6;
			}
			if (num2 < num7)
			{
				num2 = num7;
			}
			if (list != null)
			{
				foreach (CustomAxisLabels current4 in list)
				{
					if (current4.Visual != null)
					{
						if (this.Width == this.ScrollableSize)
						{
							this.AxisElementsContainer.Children.Add(current4.Visual);
						}
						else
						{
							this.InternalStackPanel.Children.Add(current4.Visual);
						}
					}
				}
			}
			if (this.AxisLabels.Visual != null)
			{
				this.AxisLabels.Visual.Margin = new Thickness(0.0, 0.0, 0.0, 4.0);
				if (this.Width == this.ScrollableSize)
				{
					this.AxisElementsContainer.Children.Add(this.AxisLabels.Visual);
				}
				else
				{
					this.InternalStackPanel.Children.Add(this.AxisLabels.Visual);
				}
			}
			List<Ticks> list2 = this.Ticks.Reverse<Ticks>().ToList<Ticks>();
			foreach (Ticks current5 in list2)
			{
				current5.SetParms(PlacementTypes.Top, this.ScrollableSize, double.NaN);
				current5.CreateVisualObject();
				if (current5.Visual != null)
				{
					if (this.Width == this.ScrollableSize)
					{
						if (!this.AxisElementsContainer.Children.Contains(current5.Visual))
						{
							this.AxisElementsContainer.Children.Add(current5.Visual);
						}
					}
					else if (!this.InternalStackPanel.Children.Contains(current5.Visual))
					{
						this.InternalStackPanel.Children.Add(current5.Visual);
					}
				}
			}
			if (this.Width != this.ScrollableSize)
			{
				this.ScrollViewerElement.Children.Add(this.InternalStackPanel);
				this.AxisElementsContainer.Children.Add(this.ScrollViewerElement);
			}
			this.AxisElementsContainer.Children.Add(this.AxisLine);
			Size size = Graphics.CalculateVisualSize(this.AxisElementsContainer);
			double num11;
			if (this.InternalMinHeight != 0.0 && this.InternalMinHeight > size.Height)
			{
				num11 = this.InternalMinHeight;
			}
			else
			{
				num11 = size.Height;
			}
			if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight < num11)
			{
				num11 = this.InternalMaxHeight;
			}
			Canvas canvas = new Canvas();
			canvas.Height = num11;
			canvas.Width = this.Width;
			canvas.Children.Add(this.AxisElementsContainer);
			this.AxisElementsContainer.SetValue(Canvas.TopProperty, num11 - size.Height);
			if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight <= num11)
			{
				if (this.AxisLabels.Visual != null)
				{
					this.GetNewOverflow4TopLabels(num, num2, ref num3, ref num4, canvas.Height);
				}
				if (list != null)
				{
					foreach (CustomAxisLabels current6 in list)
					{
						if (current6.Visual != null)
						{
							this.GetNewOverflow4TopCustomLabels(current6, num6, num7, ref num8, ref num9, canvas.Height);
						}
					}
				}
				if (num3 < num8)
				{
					num3 = num8;
				}
				if (num4 < num9)
				{
					num4 = num9;
				}
			}
			else
			{
				num3 = num;
				num4 = num2;
			}
			canvas.Clip = new RectangleGeometry
			{
				Rect = new Rect(-4.0 - num3, 0.0, canvas.Width + num3 + num4 + 8.0, size.Height)
			};
			this.Visual.Children.Add(canvas);
			Size size2 = Graphics.CalculateVisualSize(this.Visual);
			double sizeFromTrendLineLabels = this.GetSizeFromTrendLineLabels(this);
			if (size2.Height < sizeFromTrendLineLabels)
			{
				(this.Visual.Children[0] as Border).Height = sizeFromTrendLineLabels - size2.Height;
			}
			size2 = Graphics.CalculateVisualSize(this.Visual);
			this.Visual.Height = size2.Height;
		}

		private void GetNewOverflow4TopLabels(double leftOverflow, double rightOverflow, ref double newLeftOverflow, ref double newRightOverflow, double visualHeight)
		{
			if (this.AxisLabels.InternalAngle.Value > 0.0)
			{
				double x = this.AxisLabels.AxisLabelList[0].Position.X;
				Point point = new Point(this.AxisLabels.AxisLabelList[0].Position.X, this.AxisLabels.AxisLabelList[0].Position.Y);
				Point point2 = new Point(this.AxisLabels.AxisLabelList[0].ActualLeft - this.AxisLabels.AxisLabelList[0].ActualWidth, this.AxisLabels.AxisLabelList[0].ActualTop);
				Point point3 = new Point(point2.X, point.Y);
				double num = Graphics.DistanceBetweenTwoPoints(point3, point2);
				double num2 = Graphics.DistanceBetweenTwoPoints(point3, point);
				double a = Math.Atan(num / num2);
				if (num > visualHeight)
				{
					num = visualHeight;
				}
				leftOverflow = num / Math.Tan(a);
				leftOverflow -= x;
				newLeftOverflow = leftOverflow;
				this.AxisLabels.LeftOverflow = newLeftOverflow;
				return;
			}
			if (this.AxisLabels.InternalAngle.Value < 0.0)
			{
				double x2 = this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].Position.X;
				Point point4 = new Point(this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].Position.X, this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].Position.Y);
				Point point5 = new Point(this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].ActualLeft + this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].ActualWidth, this.AxisLabels.AxisLabelList[this.AxisLabels.AxisLabelList.Count - 1].ActualHeight);
				Point point6 = new Point(point5.X, point4.Y);
				double num3 = Graphics.DistanceBetweenTwoPoints(point6, point5);
				double num4 = Graphics.DistanceBetweenTwoPoints(point6, point4);
				double a2 = Math.Atan(num3 / num4);
				if (num3 > visualHeight)
				{
					num3 = visualHeight;
				}
				rightOverflow = num3 / Math.Tan(a2);
				rightOverflow += x2;
				rightOverflow -= this.Width;
				newRightOverflow = rightOverflow;
				this.AxisLabels.RightOverflow = newRightOverflow;
			}
		}

		private void GetNewOverflow4TopCustomLabels(CustomAxisLabels customLabels, double leftOverflow, double rightOverflow, ref double newLeftOverflow, ref double newRightOverflow, double visualHeight)
		{
			if (customLabels.InternalAngle.Value > 0.0)
			{
				double x = customLabels.AxisLabelList[0].Position.X;
				Point point = new Point(customLabels.AxisLabelList[0].Position.X, customLabels.AxisLabelList[0].Position.Y);
				Point point2 = new Point(customLabels.AxisLabelList[0].ActualLeft - customLabels.AxisLabelList[0].ActualWidth, customLabels.AxisLabelList[0].ActualTop);
				Point point3 = new Point(point2.X, point.Y);
				double num = Graphics.DistanceBetweenTwoPoints(point3, point2);
				double num2 = Graphics.DistanceBetweenTwoPoints(point3, point);
				double a = Math.Atan(num / num2);
				if (num > visualHeight)
				{
					num = visualHeight;
				}
				leftOverflow = num / Math.Tan(a);
				leftOverflow -= x;
				newLeftOverflow = leftOverflow;
				customLabels.LeftOverflow = newLeftOverflow;
				return;
			}
			if (this.AxisLabels.InternalAngle.Value < 0.0)
			{
				double x2 = customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].Position.X;
				Point point4 = new Point(customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].Position.X, customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].Position.Y);
				Point point5 = new Point(customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].ActualLeft + customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].ActualWidth, customLabels.AxisLabelList[customLabels.AxisLabelList.Count - 1].ActualHeight);
				Point point6 = new Point(point5.X, point4.Y);
				double num3 = Graphics.DistanceBetweenTwoPoints(point6, point5);
				double num4 = Graphics.DistanceBetweenTwoPoints(point6, point4);
				double a2 = Math.Atan(num3 / num4);
				if (num3 > visualHeight)
				{
					num3 = visualHeight;
				}
				rightOverflow = num3 / Math.Tan(a2);
				rightOverflow += x2;
				rightOverflow -= this.Width;
				newRightOverflow = rightOverflow;
				customLabels.RightOverflow = newRightOverflow;
			}
		}

		private void CreateAxisTitleVisual(Thickness margin)
		{
			try
			{
				if (this.AxisTitleElement != null)
				{
					this.AxisTitleElement.InternalMargin = margin;
					this.AxisTitleElement.IsNotificationEnable = false;
					while (true)
					{
						this.AxisTitleElement.CreateVisualObject(new ElementData
						{
							Element = this
						}, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
						Size size = Graphics.CalculateVisualSize(this.AxisTitleElement.Visual);
						if (this.AxisOrientation == AxisOrientation.Horizontal)
						{
							if (size.Width <= this.Width || this.AxisTitleElement.InternalFontSize == 0.2)
							{
								break;
							}
							double num = this.AxisTitleElement.InternalFontSize;
							num -= 0.2;
							this.AxisTitleElement.InternalFontSize = Math.Round(num, 2);
						}
						else
						{
							if (size.Height <= this.Height || this.AxisTitleElement.InternalFontSize == 0.2)
							{
								break;
							}
							double num = this.AxisTitleElement.InternalFontSize;
							num -= 0.2;
							this.AxisTitleElement.InternalFontSize = Math.Round(num, 2);
						}
					}
					this.AxisTitleElement.IsNotificationEnable = true;
				}
			}
			catch
			{
				throw new ArgumentException("Internal Size Error");
			}
		}

		private bool SetAxesXLimits()
		{
			if (!this.SetAxisLimitForMinimumGap())
			{
				return false;
			}
			this.MatchLeftAndRightGaps();
			return true;
		}

		private double GetIncrement(double minDifference)
		{
			double result;
			if (this.PlotDetails.DrawingDivisionFactor != 0)
			{
				result = minDifference / 2.0 * 1.1;
			}
			else
			{
				result = minDifference / 2.0 * 0.4;
			}
			return result;
		}

		private double GetLeftAndRightIncrement4NonZeroDivisionFactor(double minDifference)
		{
			double num;
			if (double.IsNaN(this.AxisOffset))
			{
				num = this.GetIncrement(minDifference);
			}
			else if (this.AxisOrientation == AxisOrientation.Horizontal)
			{
				if (this.Width != 0.0)
				{
					double num2 = this.Width * this.AxisOffset;
					num = Graphics.PixelPositionToValue(0.0, this.Width - 2.0 * num2, this.AxisManager.MinimumValue, this.AxisManager.MaximumValue, num2);
					if (!this.IsDateTimeAxis)
					{
						if (num > this.AxisManager.MinimumValue)
						{
							num -= this.AxisManager.MinimumValue;
						}
						else if (this.AxisManager.MinimumValue == this.AxisManager.MaximumValue)
						{
							num = this.GetIncrement(minDifference);
						}
					}
				}
				else
				{
					num = this.GetIncrement(minDifference);
				}
			}
			else if (this.Height != 0.0)
			{
				double num3 = this.Height * this.AxisOffset;
				num = Graphics.PixelPositionToValue(0.0, this.Height - 2.0 * num3, this.AxisManager.MinimumValue, this.AxisManager.MaximumValue, num3);
				if (!this.IsDateTimeAxis)
				{
					if (num > this.AxisManager.MinimumValue)
					{
						num -= this.AxisManager.MinimumValue;
					}
					else if (this.AxisManager.MinimumValue == this.AxisManager.MaximumValue)
					{
						num = this.GetIncrement(minDifference);
					}
				}
			}
			else
			{
				num = this.GetIncrement(minDifference);
			}
			return num;
		}

		private bool SetAxisLimitForMinimumGap()
		{
			double maxOfMinDifferencesForXValue = this.PlotDetails.GetMaxOfMinDifferencesForXValue();
			double num = maxOfMinDifferencesForXValue;
			double arg_19_0 = this.AxisManager.AxisMinimumValue;
			double arg_25_0 = this.AxisManager.AxisMaximumValue;
			if (double.IsInfinity(num))
			{
				num = (this.AxisManager.AxisMaximumValue - this.AxisManager.AxisMinimumValue) * 0.8;
			}
			if (this.AxisMinimum != null && this.IsDateTimeAxis)
			{
				this.AxisMinimumNumeric = DateTimeHelper.DateDiff(this.AxisMinimumDateTime, this.MinDate, this.MinDateRange, this.MaxDateRange, this.InternalIntervalType, this.XValueType);
				this.AxisManager.AxisMinimumValue = this.AxisMinimumNumeric;
				if (this.AxisManager.AxisMinimumValue > this.AxisManager.AxisMaximumValue && this.AxisMaximum != null && this.IsDateTimeAxis)
				{
					this.AxisMaximumNumeric = DateTimeHelper.DateDiff(this.AxisMaximumDateTime, this.MinDate, this.MinDateRange, this.MaxDateRange, this.InternalIntervalType, this.XValueType);
					this.AxisManager.AxisMaximumValue = this.AxisMaximumNumeric;
				}
				this.AxisManager.Calculate();
			}
			if (this.AxisMaximum != null && this.IsDateTimeAxis)
			{
				this.AxisMaximumNumeric = DateTimeHelper.DateDiff(this.AxisMaximumDateTime, this.MinDate, this.MinDateRange, this.MaxDateRange, this.InternalIntervalType, this.XValueType);
				this.AxisManager.AxisMaximumValue = this.AxisMaximumNumeric;
				if (this.AxisManager.AxisMaximumValue < this.AxisManager.AxisMinimumValue && this.AxisMinimum != null && this.IsDateTimeAxis)
				{
					this.AxisMinimumNumeric = DateTimeHelper.DateDiff(this.AxisMinimumDateTime, this.MinDate, this.MinDateRange, this.MaxDateRange, this.InternalIntervalType, this.XValueType);
					this.AxisManager.AxisMinimumValue = this.AxisMinimumNumeric;
				}
				this.AxisManager.Calculate();
			}
			double leftAndRightIncrement4NonZeroDivisionFactor = this.GetLeftAndRightIncrement4NonZeroDivisionFactor(num);
			if (double.IsNaN(this.AxisMinimumNumeric) && !this.StartFromZero.Value)
			{
				if (this.Logarithmic)
				{
					this.AxisManager.AxisMinimumValue = Math.Log(this.AxisManager.MinimumValue, this.LogarithmBase) - leftAndRightIncrement4NonZeroDivisionFactor;
				}
				else
				{
					this.AxisManager.AxisMinimumValue = this.AxisManager.MinimumValue - leftAndRightIncrement4NonZeroDivisionFactor;
				}
				if (this.XValueType != ChartValueTypes.Numeric)
				{
					DateTime dateTime = DateTimeHelper.AlignDateTime(this.MinDate, 1.0, this.InternalIntervalType);
					double num2 = DateTimeHelper.DateDiff(dateTime, this.MinDate, this.MinDateRange, this.MaxDateRange, this.InternalIntervalType, this.XValueType);
					if (this.AxisManager.AxisMinimumValue > num2)
					{
						this.AxisManager.AxisMinimumValue = num2;
					}
					else
					{
						double num3 = Math.Floor((num2 - this.AxisManager.AxisMinimumValue) / this.InternalInterval);
						if (!double.IsInfinity(num3) && num3 >= 1.0)
						{
							num2 -= Math.Floor(num3) * this.InternalInterval;
						}
					}
					DateTime dateTime2 = DateTimeHelper.XValueToDateTime(this.MinDate, num2, this.InternalIntervalType);
					this.FirstLabelDate = DateTimeHelper.AlignDateTime(dateTime2, (this.InternalInterval < 1.0) ? this.InternalInterval : 1.0, this.InternalIntervalType);
					this.FirstLabelPosition = DateTimeHelper.DateDiff(this.FirstLabelDate, this.MinDate, this.MinDateRange, this.MaxDateRange, this.InternalIntervalType, this.XValueType);
					if (this.AxisManager.AxisMinimumValue > this.FirstLabelPosition)
					{
						this.FirstLabelDate = dateTime2;
						this.FirstLabelPosition = DateTimeHelper.DateDiff(this.FirstLabelDate, this.MinDate, this.MinDateRange, this.MaxDateRange, this.InternalIntervalType, this.XValueType);
					}
				}
			}
			else
			{
				if (!double.IsNaN(this.AxisMinimumNumeric))
				{
					if (this.Logarithmic)
					{
						this.AxisManager.AxisMinimumValue = Math.Log(this.AxisMinimumNumeric, this.LogarithmBase);
					}
					else
					{
						this.AxisManager.AxisMinimumValue = this.AxisMinimumNumeric;
					}
				}
				this.FirstLabelPosition = this.AxisManager.AxisMinimumValue;
				if (this.XValueType != ChartValueTypes.Numeric)
				{
					if (this.AxisMinimum != null)
					{
						this.FirstLabelDate = this.AxisMinimumDateTime;
					}
					else
					{
						this.FirstLabelDate = DateTimeHelper.XValueToDateTime(this.MinDate, this.AxisManager.AxisMinimumValue, this.InternalIntervalType);
					}
				}
			}
			if (double.IsNaN(this.AxisMaximumNumeric))
			{
				if (this.Logarithmic)
				{
					this.AxisManager.AxisMaximumValue = Math.Log(this.AxisManager.MaximumValue, this.LogarithmBase) + leftAndRightIncrement4NonZeroDivisionFactor;
				}
				else
				{
					this.AxisManager.AxisMaximumValue = this.AxisManager.MaximumValue + leftAndRightIncrement4NonZeroDivisionFactor;
				}
			}
			else if (this.Logarithmic)
			{
				this.AxisManager.AxisMaximumValue = Math.Log(this.AxisMaximumNumeric, this.LogarithmBase);
			}
			else
			{
				this.AxisManager.AxisMaximumValue = this.AxisMaximumNumeric;
			}
			return true;
		}

		private void MatchLeftAndRightGaps()
		{
			if (double.IsNaN(this.AxisMinimumNumeric))
			{
				if (double.IsNaN(this.AxisMaximumNumeric))
				{
					if (this.Logarithmic)
					{
						if (Math.Pow(this.LogarithmBase, this.AxisManager.AxisMaximumValue) - this.Maximum <= this.Minimum - Math.Pow(this.LogarithmBase, this.AxisManager.AxisMinimumValue))
						{
							this.AxisManager.AxisMaximumValue = Math.Log(this.Maximum, this.LogarithmBase) + Math.Log(this.Minimum, this.LogarithmBase) - this.AxisManager.AxisMinimumValue;
							return;
						}
					}
					else if (this.AxisManager.AxisMaximumValue - this.Maximum <= this.Minimum - this.AxisManager.AxisMinimumValue)
					{
						this.AxisManager.AxisMaximumValue = this.Maximum + this.Minimum - this.AxisManager.AxisMinimumValue;
						return;
					}
				}
				else
				{
					if (this.Logarithmic)
					{
						this.AxisManager.AxisMaximumValue = Math.Log(this.AxisMaximumNumeric, this.LogarithmBase);
						return;
					}
					this.AxisManager.AxisMaximumValue = this.AxisMaximumNumeric;
				}
			}
		}

		private void ParseScalingSets(string scalingSets)
		{
			if (string.IsNullOrEmpty(scalingSets))
			{
				return;
			}
			string[] array = scalingSets.Split(new char[]
			{
				';'
			});
			double num = 1.0;
			this._scaleUnits = new List<string>();
			this._scaleValues = new List<double>();
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2.Length != 2)
				{
					throw new Exception("Invalid scaling set parameters. should be of the form value,unit;value,unit;...");
				}
				double num2 = double.Parse(array2[0], CultureInfo.InvariantCulture);
				num *= num2;
				this._scaleValues.Add(num);
				this._scaleUnits.Add(array2[1]);
			}
		}

		internal object CalculateDateTimeFromNumericXValue(double xValue)
		{
			if (this.IsDateTimeAxis)
			{
				return DateTimeHelper.XValueToDateTime(this.MinDate, xValue, this.InternalIntervalType);
			}
			return xValue;
		}

		private void ApplyVisualProperty()
		{
			this.Visual.Cursor = ((this.Cursor == null) ? null : this.Cursor);
			base.AttachHref(base.Chart, this.Visual, this.Href, this.HrefTarget);
			base.AttachToolTip(base.Chart, this, this.Visual);
			base.AttachEvents2Visual(this, this.Visual);
			this.Visual.Opacity = this.InternalOpacity;
		}

		private double GenerateDefaultInterval()
		{
			if (this._isDateTimeAutoInterval || (this.XValueType != ChartValueTypes.Numeric && this.IntervalType != IntervalTypes.Years && double.IsNaN(this.Interval.Value) && double.IsNaN(this.AxisLabels.Interval.Value) && !double.IsNaN(this.InternalInterval) && this.InternalInterval >= 1.0))
			{
				return this.InternalInterval;
			}
			if (this.AxisType == AxisTypes.Primary)
			{
				if (double.IsNaN(this.Interval.Value) && double.IsNaN(this.AxisLabels.Interval.Value) && this.PlotDetails.IsAllPrimaryAxisXLabelsPresent && !this.Logarithmic)
				{
					List<double> list = (from entry in this.PlotDetails.AxisXPrimaryLabels
					orderby entry.Key
					select entry.Key).ToList<double>();
					if (list.Count > 0)
					{
						double num = 1.7976931348623157E+308;
						for (int i = 0; i < list.Count - 1; i++)
						{
							num = Math.Min(num, Math.Abs(list[i] - list[i + 1]));
						}
						if (num != 1.7976931348623157E+308)
						{
							return num;
						}
					}
				}
			}
			else if (double.IsNaN(this.Interval.Value) && double.IsNaN(this.AxisLabels.Interval.Value) && this.PlotDetails.IsAllSecondaryAxisXLabelsPresent)
			{
				List<double> list2 = (from entry in this.PlotDetails.AxisXSecondaryLabels
				orderby entry.Key
				select entry.Key).ToList<double>();
				if (list2.Count > 0)
				{
					double num2 = 1.7976931348623157E+308;
					for (int j = 0; j < list2.Count - 1; j++)
					{
						num2 = Math.Min(num2, Math.Abs(list2[j] - list2[j + 1]));
					}
					if (num2 != 1.7976931348623157E+308)
					{
						return num2;
					}
				}
			}
			return this.Interval.Value;
		}

		private void CustomLabels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems == null)
				{
					goto IL_FE;
				}
				using (IEnumerator enumerator = e.NewItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CustomAxisLabels customAxisLabels = (CustomAxisLabels)enumerator.Current;
						if (base.Chart != null)
						{
							customAxisLabels.Chart = base.Chart;
						}
						customAxisLabels.Parent = this;
						customAxisLabels.PropertyChanged -= new PropertyChangedEventHandler(this.customLabels_PropertyChanged);
						customAxisLabels.PropertyChanged += new PropertyChangedEventHandler(this.customLabels_PropertyChanged);
						this.AddCustomAxisLabelsToRootElement(customAxisLabels);
					}
					goto IL_FE;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (CustomAxisLabels customAxisLabels2 in e.OldItems)
				{
					customAxisLabels2.Chart = null;
					if (this._rootElement != null && customAxisLabels2.ParentTree != null)
					{
						this._rootElement.Children.Remove(customAxisLabels2);
					}
				}
			}
			IL_FE:
			base.FirePropertyChanged(VcProperties.CustomAxisLabels);
		}

		private void Ticks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			bool flag = false;
			if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
			{
				foreach (Ticks ticks in e.NewItems)
				{
					if (ticks._isAutoGenerated)
					{
						flag = true;
					}
					if (base.Chart != null)
					{
						ticks.Chart = base.Chart;
					}
					ticks.PropertyChanged -= new PropertyChangedEventHandler(this.tick_PropertyChanged);
					ticks.PropertyChanged += new PropertyChangedEventHandler(this.tick_PropertyChanged);
					this.AddTickElementIntoRootElement(ticks);
				}
			}
			if (!flag)
			{
				base.FirePropertyChanged(VcProperties.Ticks);
			}
		}

		private void Grids_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			bool flag = false;
			if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
			{
				foreach (ChartGrid chartGrid in e.NewItems)
				{
					if (chartGrid._isAutoGenerated)
					{
						flag = true;
					}
					if (base.Chart != null)
					{
						chartGrid.Chart = base.Chart;
					}
					chartGrid.PropertyChanged -= new PropertyChangedEventHandler(this.grid_PropertyChanged);
					chartGrid.PropertyChanged += new PropertyChangedEventHandler(this.grid_PropertyChanged);
					this.AddGridsIntoRootElement(chartGrid);
				}
			}
			if (!flag)
			{
				base.FirePropertyChanged(VcProperties.Grids);
			}
		}

		private void grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.FirePropertyChanged((VcProperties)Enum.Parse(typeof(VcProperties), e.PropertyName, true));
		}

		private void tick_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.FirePropertyChanged((VcProperties)Enum.Parse(typeof(VcProperties), e.PropertyName, true));
		}

		private void customLabels_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.FirePropertyChanged((VcProperties)Enum.Parse(typeof(VcProperties), e.PropertyName, true));
		}

		private void UpdateHorizontalAxisSettings()
		{
			switch (this.AxisType)
			{
			case AxisTypes.Primary:
				this.UpdateHorizontalPrimaryAxisSettings();
				return;
			case AxisTypes.Secondary:
				this.UpdateHorizontalSecondaryAxisSettings();
				return;
			default:
				return;
			}
		}

		private void ApplyHorizontalAxisSettings()
		{
			switch (this.AxisType)
			{
			case AxisTypes.Primary:
				this.ApplyHorizontalPrimaryAxisSettings();
				return;
			case AxisTypes.Secondary:
				this.ApplyHorizontalSecondaryAxisSettings();
				return;
			default:
				return;
			}
		}

		private void SetUpTicks()
		{
			if (this.Ticks.Count == 0)
			{
				Ticks item = new Ticks
				{
					_isAutoGenerated = true
				};
				this.Ticks.Add(item);
			}
			foreach (Ticks current in this.Ticks)
			{
				current.IsNotificationEnable = false;
				if (this.AxisRepresentation.ToString() == "AxisX")
				{
					current.ApplyStyleFromTheme(base.Chart, "AxisXTicks");
				}
				else if (this.AxisRepresentation.ToString() == "AxisY")
				{
					current.ApplyStyleFromTheme(base.Chart, "AxisYTicks");
				}
				current.Maximum = this.AxisManager.AxisMaximumValue;
				current.Minimum = this.AxisManager.AxisMinimumValue;
				if (this.Logarithmic)
				{
					current.DataMaximum = Math.Log(this.Maximum, this.LogarithmBase);
					current.DataMinimum = Math.Log(this.Minimum, this.LogarithmBase);
				}
				else
				{
					current.DataMaximum = this.Maximum;
					current.DataMinimum = this.Minimum;
				}
				current.ParentAxis = this;
				if (this.PlotDetails.ChartOrientation == ChartOrientationType.Circular && current.GetValue(Visifire.Charts.Ticks.TickLengthProperty) == null)
				{
					current.TickLength = new double?(2.3);
				}
				current.IsNotificationEnable = true;
			}
		}

		private void SetUpGrids()
		{
			if (this.Grids.Count == 0 && this.AxisRepresentation.ToString() != "AxisX")
			{
				ChartGrid chartGrid = new ChartGrid
				{
					_isAutoGenerated = true
				};
				this.Grids.Add(chartGrid);
				this.AddGridsIntoRootElement(chartGrid);
			}
			if (this.AxisRepresentation.ToString() == "AxisX")
			{
				if (this.Grids.Count == 0)
				{
					if (base.Chart != null && ((base.Chart as Chart).Theme == "Theme4" || (base.Chart as Chart).Theme == "Theme5"))
					{
						ChartGrid chartGrid2 = new ChartGrid
						{
							_isAutoGenerated = true
						};
						this.Grids.Add(chartGrid2);
						this.AddGridsIntoRootElement(chartGrid2);
					}
				}
				else if (base.Chart != null && (base.Chart as Chart).Theme != "Theme4" && (base.Chart as Chart).Theme != "Theme5")
				{
					ChartGrid chartGrid3 = null;
					foreach (ChartGrid current in this.Grids)
					{
						if (current._isAutoGenerated)
						{
							chartGrid3 = current;
							break;
						}
					}
					if (chartGrid3 != null)
					{
						chartGrid3.CleanUpGrid();
						this.Grids.Remove(chartGrid3);
						if (this._rootElement != null)
						{
							this._rootElement.Children.Remove(chartGrid3);
						}
					}
				}
			}
			foreach (ChartGrid current2 in this.Grids)
			{
				current2.IsNotificationEnable = false;
				if (this.AxisRepresentation.ToString() == "AxisX")
				{
					current2.ApplyStyleFromTheme(base.Chart, "AxisXGrids");
				}
				else if (this.AxisRepresentation.ToString() == "AxisY")
				{
					current2.ApplyStyleFromTheme(base.Chart, "AxisYGrids");
				}
				current2.Maximum = this.AxisManager.AxisMaximumValue;
				current2.Minimum = this.AxisManager.AxisMinimumValue;
				if (this.AxisRepresentation == AxisRepresentations.AxisX)
				{
					if (this.AxisOrientation == AxisOrientation.Horizontal)
					{
						if (this.AxisType == AxisTypes.Primary)
						{
							current2.Placement = PlacementTypes.Bottom;
						}
						else
						{
							current2.Placement = PlacementTypes.Top;
						}
					}
					else if (this.AxisType == AxisTypes.Primary)
					{
						current2.Placement = PlacementTypes.Left;
					}
					else
					{
						current2.Placement = PlacementTypes.Right;
					}
				}
				else if (this.AxisOrientation == AxisOrientation.Vertical)
				{
					if (this.AxisType == AxisTypes.Primary)
					{
						current2.Placement = PlacementTypes.Left;
					}
					else
					{
						current2.Placement = PlacementTypes.Right;
					}
				}
				else if (this.AxisType == AxisTypes.Primary)
				{
					current2.Placement = PlacementTypes.Bottom;
				}
				else
				{
					current2.Placement = PlacementTypes.Top;
				}
				if (this.Logarithmic)
				{
					current2.DataMaximum = Math.Log(this.Maximum, this.LogarithmBase);
					current2.DataMinimum = Math.Log(this.Minimum, this.LogarithmBase);
				}
				else
				{
					current2.DataMaximum = this.Maximum;
					current2.DataMinimum = this.Minimum;
				}
				current2.ParentAxis = this;
				if (this.PlotDetails.ChartOrientation == ChartOrientationType.Circular && current2.GetValue(ChartGrid.LineThicknessProperty) == null)
				{
					current2.LineThickness = new double?(0.5);
				}
				current2.IsNotificationEnable = true;
			}
		}

		private void SetUpCustomAxisLabels()
		{
			foreach (CustomAxisLabels current in this.CustomAxisLabels)
			{
				current.Maximum = this.AxisManager.AxisMaximumValue;
				current.Minimum = this.AxisManager.AxisMinimumValue;
				current.ParentAxis = this;
			}
		}

		private void SetUpAxisLabels()
		{
			this.AxisLabels.Maximum = this.AxisManager.AxisMaximumValue;
			this.AxisLabels.Minimum = this.AxisManager.AxisMinimumValue;
			if (this.Logarithmic)
			{
				this.AxisLabels.DataMaximum = Math.Log(this.Maximum, this.LogarithmBase);
				this.AxisLabels.DataMinimum = Math.Log(this.Minimum, this.LogarithmBase);
			}
			else
			{
				this.AxisLabels.DataMaximum = this.Maximum;
				this.AxisLabels.DataMinimum = this.Minimum;
			}
			this.AxisLabels.ParentAxis = this;
			this.AxisLabels.InternalRows = this.AxisLabels.Rows.Value;
			if (this.AxisRepresentation == AxisRepresentations.AxisX)
			{
				if (this.AxisType == AxisTypes.Primary)
				{
					this.AxisLabels.AxisLabelContentDictionary = this.PlotDetails.AxisXPrimaryLabels;
					this.AxisLabels.AllAxisLabels = this.PlotDetails.IsAllPrimaryAxisXLabelsPresent;
				}
				else
				{
					this.AxisLabels.AxisLabelContentDictionary = this.PlotDetails.AxisXSecondaryLabels;
					this.AxisLabels.AllAxisLabels = this.PlotDetails.IsAllSecondaryAxisXLabelsPresent;
				}
			}
			foreach (Ticks current in this.Ticks)
			{
				current.IsNotificationEnable = false;
				current.AllAxisLabels = this.AxisLabels.AllAxisLabels;
				current.AxisLabelsDictionary = this.AxisLabels.AxisLabelContentDictionary;
				current.IsNotificationEnable = true;
			}
		}

		internal static void CalculateTotalTickLength(Chart chart, ref double tickLengthOfAxisX, ref double tickLengthOfPrimaryAxisY, ref double tickLengthOfSecondaryAxisY)
		{
			tickLengthOfAxisX = (from tick in chart.AxesX[0].Ticks
			where chart.AxesX[0].Enabled.Value && tick.Enabled.Value
			select tick.TickLength.Value).Sum();
			if (tickLengthOfAxisX == 0.0)
			{
				tickLengthOfAxisX = 5.0;
			}
			tickLengthOfPrimaryAxisY = (from axis in chart.AxesY
			where axis.AxisType == AxisTypes.Primary
			from tick in axis.Ticks
			where axis.Enabled.Value && tick.Enabled.Value
			select tick.TickLength.Value).Sum();
			if (tickLengthOfPrimaryAxisY == 0.0)
			{
				tickLengthOfPrimaryAxisY = 8.0;
			}
			tickLengthOfSecondaryAxisY = (from axis in chart.AxesY
			where axis.AxisType == AxisTypes.Secondary
			from tick in axis.Ticks
			where axis.Enabled.Value && tick.Enabled.Value
			select tick.TickLength.Value).Sum();
			if (tickLengthOfSecondaryAxisY == 0.0)
			{
				tickLengthOfSecondaryAxisY = 8.0;
			}
		}

		public object PixelPositionToXValue(double pixelPosition)
		{
			Chart chart = base.Chart as Chart;
			if (this.AxisRepresentation != AxisRepresentations.AxisX || chart == null || chart.ChartArea == null || chart.ChartArea.ChartVisualCanvas == null)
			{
				return null;
			}
			object result = null;
			double num = (this.AxisOrientation == AxisOrientation.Horizontal) ? chart.ChartArea.ChartVisualCanvas.Width : chart.ChartArea.ChartVisualCanvas.Height;
			double num2 = this.PixelPositionToXValue(num, (this.AxisOrientation == AxisOrientation.Horizontal) ? pixelPosition : (num - pixelPosition));
			if (this.IsDateTimeAxis)
			{
				try
				{
					result = DateTimeHelper.XValueToDateTime(this.MinDate, num2, this.InternalIntervalType);
					return result;
				}
				catch
				{
					result = chart.ChartArea.AxisX.MinDate;
					return result;
				}
			}
			if (this.Logarithmic)
			{
				result = DataPointHelper.ConvertLogarithmicValue2ActualValue(chart, num2, this.AxisType);
			}
			else
			{
				result = num2;
			}
			return result;
		}

		internal double PixelPositionToXValue(double maxWidth, double pixelPosition)
		{
			return Graphics.PixelPositionToValue(0.0, maxWidth, this.InternalAxisMinimum, this.InternalAxisMaximum, pixelPosition);
		}

		internal double PixelPositionToYValue(double maxHeight, double pixelPosition)
		{
			return Graphics.PixelPositionToValue(maxHeight, 0.0, this.InternalAxisMinimum, this.InternalAxisMaximum, pixelPosition);
		}

		internal string AddPrefixAndSuffix(string str)
		{
			if (base.GetValue(Axis.PrefixProperty) == null && base.GetValue(Axis.SuffixProperty) == null)
			{
				return str;
			}
			return this.Prefix + str + this.Suffix;
		}

		internal string GetFormattedString(double value)
		{
			string str = value.ToString();
			if (this.ScaleValues != null && this.ScaleUnits != null)
			{
				string str2 = this.ScaleUnits[0];
				double num = this.ScaleValues[0];
				int num2 = 0;
				while (num2 < this.ScaleValues.Count && Math.Abs(value) / this.ScaleValues[num2] >= 1.0)
				{
					num = this.ScaleValues[num2];
					str2 = this.ScaleUnits[num2];
					num2++;
				}
				if (this.IsDateTimeAxis)
				{
					str = (value / num).ToString() + str2;
				}
				else
				{
					str = (value / num).ToString(this.ValueFormatString) + str2;
				}
			}
			else if (this.IsDateTimeAxis)
			{
				str = value.ToString();
			}
			else
			{
				str = value.ToString(this.ValueFormatString);
			}
			return this.AddPrefixAndSuffix(str);
		}

		internal void SetScrollBar()
		{
			if (this.AxisOrientation == AxisOrientation.Vertical)
			{
				this.ScrollBarElement = ((this.AxisType == AxisTypes.Primary) ? base.Chart._leftAxisScrollBar : base.Chart._rightAxisScrollBar);
				return;
			}
			this.ScrollBarElement = ((this.AxisType == AxisTypes.Primary) ? base.Chart._bottomAxisScrollBar : base.Chart._topAxisScrollBar);
		}

		internal void SetScrollBarValueFromOffset(double offset, bool isScrollBarOffsetPropertyChanged)
		{
			if (this.ScrollBarElement != null && !this.ScrollBarElement.IsStretching)
			{
				double scrollBarValueFromOffset = this.GetScrollBarValueFromOffset(offset);
				this.ScrollBarElement.SetValue(RangeBase.ValueProperty, scrollBarValueFromOffset);
				if (this.ScrollBarOffsetChanged != null)
				{
					this.ScrollBarOffsetChanged(this.ScrollBarElement, new ScrollEventArgs(isScrollBarOffsetPropertyChanged ? ScrollEventType.ThumbTrack : ScrollEventType.First, scrollBarValueFromOffset));
				}
			}
		}

		internal double GetScrollBarValueFromOffset(double offset)
		{
			if (double.IsNaN(offset))
			{
				offset = 0.0;
			}
			offset = (this.ScrollBarElement.Maximum - this.ScrollBarElement.Minimum) * offset + this.ScrollBarElement.Minimum;
			if (this.PlotDetails != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				offset = this.ScrollBarElement.Maximum - offset;
			}
			return offset;
		}

		internal double GetScrollBarValueFromOffsetAtInitialRender(double offset)
		{
			if (double.IsNaN(offset))
			{
				offset = 0.0;
			}
			offset = (this.ScrollBarElement.Maximum - this.ScrollBarElement.Minimum) * offset + this.ScrollBarElement.Minimum;
			return offset;
		}

		internal void CalculateViewMinimumAndMaximum(Chart chart, double offsetInPixel, double plotAreaWidth, double plotAreaHeight)
		{
			AxisOrientation axisOrientation = chart.ChartArea.AxisX.AxisOrientation;
			double num;
			double num2;
			double num3;
			if (axisOrientation == AxisOrientation.Horizontal)
			{
				offsetInPixel = chart.ChartArea.GetScrollingOffsetOfAxis(chart.ChartArea.AxisX, offsetInPixel);
				num = plotAreaWidth;
				num2 = offsetInPixel;
				num3 = num2 + this.Width;
			}
			else
			{
				offsetInPixel = chart.ChartArea.GetScrollingOffsetOfAxis(chart.ChartArea.AxisX, offsetInPixel);
				num = plotAreaHeight;
				num3 = num - offsetInPixel;
				num2 = num3 - this.Height;
			}
			double num4 = this.PixelPositionToXValue(num, num2);
			if (this.Logarithmic)
			{
				this._numericViewMinimum = Math.Pow(this.LogarithmBase, num4);
			}
			else
			{
				this._numericViewMinimum = num4;
			}
			this.ViewMinimum = this.GetViewMinimum(chart, this._numericViewMinimum);
			num4 = this.PixelPositionToXValue(num, num3);
			if (this.Logarithmic)
			{
				this._numericViewMaximum = Math.Pow(this.LogarithmBase, num4);
			}
			else
			{
				this._numericViewMaximum = num4;
			}
			this.ViewMaximum = this.GetViewMaximum(chart, this._numericViewMaximum);
		}

		private object GetViewMinimum(Chart chart, double minValue)
		{
			object result;
			if (chart.ChartArea.AxisX.IsDateTimeAxis)
			{
				if (chart.PlotDetails.ListOfAllDataPoints.Count != 0 && !double.IsInfinity(minValue) && !double.IsNaN(minValue))
				{
					result = DateTimeHelper.XValueToDateTime(chart.ChartArea.AxisX.MinDate, minValue, chart.ChartArea.AxisX.InternalIntervalType);
				}
				else
				{
					result = chart.ChartArea.AxisX.MinDate;
				}
			}
			else
			{
				result = minValue;
			}
			return result;
		}

		private object GetViewMaximum(Chart chart, double maxValue)
		{
			object result;
			if (chart.ChartArea.AxisX.IsDateTimeAxis)
			{
				if (chart.PlotDetails.ListOfAllDataPoints.Count != 0 && !double.IsInfinity(maxValue) && !double.IsNaN(maxValue))
				{
					result = DateTimeHelper.XValueToDateTime(chart.ChartArea.AxisX.MinDate, maxValue, chart.ChartArea.AxisX.InternalIntervalType);
				}
				else
				{
					result = chart.ChartArea.AxisX.MaxDate;
				}
			}
			else
			{
				result = maxValue;
			}
			return result;
		}

		internal void FireScrollEvent(ScrollEventArgs e, double offsetInPixel)
		{
			if (base.Chart != null && (base.Chart as Chart).Series.Count > 0)
			{
				if (this.PlotDetails.ListOfAllDataPoints.Count == 0)
				{
					this.CalculateAndSetZoomStates4EmptySeries(offsetInPixel);
					return;
				}
				if (this._oldScrollBarOffsetInPixel == offsetInPixel && !(base.Chart as Chart).ChartArea._isDragging)
				{
					if ((base.Chart as Chart)._clearAndResetZoomState)
					{
						this.ResetZoomState(base.Chart as Chart, true);
					}
					if (this._initialState.MinXValue == null && this._initialState.MaxXValue == null)
					{
						this._initialState.MinXValue = this.ViewMinimum;
						this._initialState.MaxXValue = this.ViewMaximum;
						return;
					}
					this._initialState.MinXValue = this.GetViewMinimum(base.Chart as Chart, this.InternalAxisMinimum);
					this._initialState.NumericMinXValue = this.InternalAxisMinimum;
					this._initialState.MaxXValue = this.GetViewMaximum(base.Chart as Chart, this.InternalAxisMaximum);
					this._initialState.NumericMaxXValue = this.InternalAxisMaximum;
				}
				this._oldScrollBarOffsetInPixel = offsetInPixel;
				if (this.AxisRepresentation == AxisRepresentations.AxisX)
				{
					Chart chart = base.Chart as Chart;
					if (chart.ChartArea.ChartVisualCanvas != null)
					{
						this.CalculateViewMinimumAndMaximum(chart, offsetInPixel, chart.ChartArea.ChartVisualCanvas.Width, chart.ChartArea.ChartVisualCanvas.Height);
					}
					if (chart.ZoomingEnabled)
					{
						if (this._zoomStateStack.Count != 0 && (this._zoomStateStack.First<Axis.ZoomState>().MinXValue.GetType() != this.ViewMinimum.GetType() || this._zoomStateStack.First<Axis.ZoomState>().MaxXValue.GetType() != this.ViewMaximum.GetType()))
						{
							this._zoomStateStack.Clear();
						}
						if (this._zoomStateStack.Count == 0)
						{
							this._zoomStateStack.Push(new Axis.ZoomState(this.ViewMinimum, this.ViewMaximum));
							this._oldZoomState.MinXValue = null;
							this._oldZoomState.MaxXValue = null;
						}
						if (this._showAllState)
						{
							this._zoomStateStack.Clear();
							this._zoomStateStack.Push(new Axis.ZoomState(this.ViewMinimum, this.ViewMaximum));
							this._oldZoomState.MinXValue = null;
							this._oldZoomState.MaxXValue = null;
							this._showAllState = false;
						}
						if (chart.ChartArea._isFirstTimeRender || this._isScrollEventFiredFirstTime || (this._initialState.MinXValue == null && this._initialState.MaxXValue == null))
						{
							this._initialState.MinXValue = this.ViewMinimum;
							this._initialState.MaxXValue = this.ViewMaximum;
							this._initialState.NumericMinXValue = this._numericViewMinimum;
							this._initialState.NumericMaxXValue = this._numericViewMaximum;
						}
						else
						{
							this._initialState.MinXValue = this.GetViewMinimum(base.Chart as Chart, this.InternalAxisMinimum);
							this._initialState.NumericMinXValue = this.InternalAxisMinimum;
							this._initialState.MaxXValue = this.GetViewMaximum(base.Chart as Chart, this.InternalAxisMaximum);
							this._initialState.NumericMaxXValue = this.InternalAxisMaximum;
						}
					}
					if (this._onScroll != null && !chart.ChartArea._isFirstTimeRender)
					{
						this._onScroll(this, new AxisScrollEventArgs(e));
					}
					this._isScrollEventFiredFirstTime = false;
					return;
				}
			}
			else
			{
				this.CalculateAndSetZoomStates4EmptySeries(offsetInPixel);
			}
		}

		private void CalculateAndSetZoomStates4EmptySeries(double offsetInPixel)
		{
			if (this.AxisRepresentation == AxisRepresentations.AxisX)
			{
				Chart chart = base.Chart as Chart;
				if (chart.ChartArea.ChartVisualCanvas != null)
				{
					this.CalculateViewMinimumAndMaximum(chart, offsetInPixel, chart.ChartArea.ChartVisualCanvas.Width, chart.ChartArea.ChartVisualCanvas.Height);
				}
				if (chart.ZoomingEnabled)
				{
					object viewMinimum = this.GetViewMinimum(base.Chart as Chart, this.InternalAxisMinimum);
					object viewMaximum = this.GetViewMaximum(base.Chart as Chart, this.InternalAxisMaximum);
					this._zoomStateStack.Push(new Axis.ZoomState(viewMinimum, viewMaximum));
					this._oldZoomState.MinXValue = null;
					this._oldZoomState.MaxXValue = null;
					if (this._showAllState)
					{
						this._zoomStateStack.Clear();
						this._zoomStateStack.Push(new Axis.ZoomState(viewMinimum, viewMaximum));
						this._oldZoomState.MinXValue = null;
						this._oldZoomState.MaxXValue = null;
						this._showAllState = false;
					}
					this._initialState.MinXValue = viewMinimum;
					this._initialState.NumericMinXValue = this.InternalAxisMinimum;
					this._initialState.MaxXValue = viewMaximum;
					this._initialState.NumericMaxXValue = this.InternalAxisMaximum;
				}
			}
		}

		internal void CreateDefaultElements()
		{
			if (this.AxisLabels == null)
			{
				this.AxisLabels = new AxisLabels
				{
					IsDefault = true
				};
				this.AddAxisLabelsIntoRootElement();
			}
			base.IsNotificationEnable = false;
			this.AxisLabels.IsNotificationEnable = false;
			this.AxisLabels.Chart = base.Chart;
			if (this.AxisRepresentation == AxisRepresentations.AxisX)
			{
				this.AxisLabels.ApplyStyleFromTheme(base.Chart, "AxisXLabels");
			}
			else if (this.AxisRepresentation == AxisRepresentations.AxisY)
			{
				this.AxisLabels.ApplyStyleFromTheme(base.Chart, "AxisYLabels");
			}
			base.IsNotificationEnable = true;
			this.AxisLabels.IsNotificationEnable = true;
		}

		internal void SetUpPropertiesOfElements()
		{
			this.SetUpTicks();
			this.SetUpGrids();
			this.SetUpAxisLabels();
			this.SetUpCustomAxisLabels();
		}

		internal void UpdateAxisVisual()
		{
			switch (this.AxisOrientation)
			{
			case AxisOrientation.Vertical:
				this.UpdateVerticalAxisSettings();
				this._zeroBaseLinePixPosition = this.Height - Graphics.ValueToPixelPosition(0.0, this.Height, this.InternalAxisMinimum, this.InternalAxisMaximum, 0.0);
				return;
			case AxisOrientation.Horizontal:
				this.UpdateHorizontalAxisSettings();
				this._zeroBaseLinePixPosition = Graphics.ValueToPixelPosition(0.0, this.Width, this.InternalAxisMinimum, this.InternalAxisMaximum, 0.0);
				return;
			default:
				return;
			}
		}

		internal void CreateVisual()
		{
			Chart chart = base.Chart as Chart;
			if (!double.IsNaN(this.ClosestPlotDistance) && !chart.ChartArea.IsAutoCalculatedScrollBarScale && chart.IsScrollingActivated)
			{
				throw new ArgumentException(" ScrollBarScale property and ClosestPlotDistance property in Axis cannot be set together.");
			}
			base.IsNotificationEnable = false;
			foreach (CustomAxisLabels current in this.CustomAxisLabels)
			{
				current.Chart = base.Chart;
				if (this.AxisRepresentation == AxisRepresentations.AxisX)
				{
					current.ApplyStyleFromTheme(base.Chart, "CustomAxisXLabels");
				}
				else if (this.AxisRepresentation == AxisRepresentations.AxisY)
				{
					current.ApplyStyleFromTheme(base.Chart, "CustomAxisYLabels");
				}
			}
			if (this.Visual != null)
			{
				base.DetachToolTip(this.Visual);
			}
			this.Visual = new StackPanel
			{
				Background = this.InternalBackground
			};
			this.AxisElementsContainer = new StackPanel();
			this.InternalStackPanel = new Canvas();
			this.ScrollViewerElement = new Canvas();
			if (!string.IsNullOrEmpty(this.Title))
			{
				this.AxisTitleElement = new Title();
				this.ApplyTitleProperties();
			}
			else
			{
				this.AxisTitleElement = null;
			}
			this.ApplyVisualProperty();
			switch (this.AxisOrientation)
			{
			case AxisOrientation.Vertical:
				this.ApplyVerticalAxisSettings();
				this._zeroBaseLinePixPosition = this.Height - Graphics.ValueToPixelPosition(0.0, this.Height, this.InternalAxisMinimum, this.InternalAxisMaximum, 0.0);
				break;
			case AxisOrientation.Horizontal:
				this.ApplyHorizontalAxisSettings();
				this._zeroBaseLinePixPosition = Graphics.ValueToPixelPosition(0.0, this.Width, this.InternalAxisMinimum, this.InternalAxisMaximum, 0.0);
				break;
			}
			base.IsNotificationEnable = true;
			if (!this.Enabled.Value)
			{
				this.Visual.Visibility = Visibility.Collapsed;
			}
			this.AttachEventForZoomBar();
		}

		internal void CreateVisualObject(Chart Chart)
		{
			if (this.AxisLabels == null)
			{
				this.AxisLabels = new AxisLabels
				{
					IsDefault = true
				};
				this.AddAxisLabelsIntoRootElement();
			}
			base.IsNotificationEnable = false;
			this.AxisLabels.IsNotificationEnable = false;
			this.AxisLabels.Chart = Chart;
			if (!double.IsNaN(this.ClosestPlotDistance) && !Chart.ChartArea.IsAutoCalculatedScrollBarScale && Chart.IsScrollingActivated)
			{
				throw new ArgumentException(" ScrollBarScale property and ClosestPlotDistance property in Axis cannot be set together.");
			}
			if (this.AxisRepresentation == AxisRepresentations.AxisX)
			{
				this.AxisLabels.ApplyStyleFromTheme(Chart, "AxisXLabels");
			}
			else if (this.AxisRepresentation == AxisRepresentations.AxisY)
			{
				this.AxisLabels.ApplyStyleFromTheme(Chart, "AxisYLabels");
			}
			foreach (CustomAxisLabels current in this.CustomAxisLabels)
			{
				current.Chart = Chart;
				if (this.AxisRepresentation == AxisRepresentations.AxisX)
				{
					current.ApplyStyleFromTheme(Chart, "CustomAxisXLabels");
				}
				else if (this.AxisRepresentation == AxisRepresentations.AxisY)
				{
					current.ApplyStyleFromTheme(Chart, "CustomAxisYLabels");
				}
			}
			if (Chart.PlotDetails.ChartOrientation != ChartOrientationType.Circular)
			{
				if (this.Visual != null)
				{
					base.DetachToolTip(this.Visual);
				}
				this.Visual = new StackPanel
				{
					Background = this.InternalBackground
				};
				this.AxisElementsContainer = new StackPanel();
				this.InternalStackPanel = new Canvas();
				this.ScrollViewerElement = new Canvas();
				if (!string.IsNullOrEmpty(this.Title))
				{
					this.AxisTitleElement = new Title();
					this.ApplyTitleProperties();
				}
				else
				{
					this.AxisTitleElement = null;
				}
				this.ApplyVisualProperty();
				this.SetUpAxisManager();
				this.SetUpTicks();
				this.SetUpGrids();
				this.SetUpAxisLabels();
				this.SetUpCustomAxisLabels();
				switch (this.AxisOrientation)
				{
				case AxisOrientation.Vertical:
					this.ApplyVerticalAxisSettings();
					this._zeroBaseLinePixPosition = this.Height - Graphics.ValueToPixelPosition(0.0, this.Height, this.InternalAxisMinimum, this.InternalAxisMaximum, 0.0);
					break;
				case AxisOrientation.Horizontal:
					this.ApplyHorizontalAxisSettings();
					this._zeroBaseLinePixPosition = Graphics.ValueToPixelPosition(0.0, this.Width, this.InternalAxisMinimum, this.InternalAxisMaximum, 0.0);
					break;
				}
				base.IsNotificationEnable = true;
				this.AxisLabels.IsNotificationEnable = true;
				if (!this.Enabled.Value)
				{
					this.Visual.Visibility = Visibility.Collapsed;
				}
				this.AttachEventForZoomBar();
				return;
			}
			this.CreateAxesForCircularChart();
		}

		private void AttachEventForZoomBar()
		{
			this.ScrollBarElement.ScaleChanged -= new EventHandler(this.OnZoomingScaleChanged);
			this.ScrollBarElement.DragStarted -= new EventHandler(this.ScrollBarElement_DragStarted);
			this.ScrollBarElement.DragCompleted -= new EventHandler(this.ScrollBarElement_DragCompleted);
			Chart chart = base.Chart as Chart;
			bool flag = (from ds in chart.Series
			where RenderHelper.IsLineCType(ds) || ds.RenderAs == RenderAs.Area
			select ds).Count<DataSeries>() == chart.Series.Count && chart.PlotDetails.ListOfAllDataPoints.Count <= 500;
			if (!chart.PlotDetails.IsOverrideViewPortRangeEnabled && (flag || chart.PlotDetails.ListOfAllDataPoints.Count < 500))
			{
				this.ScrollBarElement.ScaleChanged += new EventHandler(this.OnZoomingScaleChanged);
				this.ScrollBarElement.DragCompleted += new EventHandler(this.ScrollBarElement_DragCompleted);
				this._renderAfterDrag = false;
				return;
			}
			this.ScrollBarElement.DragStarted += new EventHandler(this.ScrollBarElement_DragStarted);
			this.ScrollBarElement.DragCompleted += new EventHandler(this.ScrollBarElement_DragCompleted);
			this._renderAfterDrag = true;
		}

		private void ScrollBarElement_DragCompleted(object sender, EventArgs e)
		{
			if (!this._renderAfterDrag)
			{
				this.SetZoomState();
				return;
			}
			this.OnZoomingScaleChanged(sender, e);
			(base.Chart as Chart).Dispatcher.BeginInvoke(new Action(this.SetZoomState), new object[0]);
		}

		private void SetZoomState()
		{
			Chart chart = base.Chart as Chart;
			if (chart == null)
			{
				return;
			}
			if (!this._dragCompletedLock && this.ScrollBarElement.Scale < 1.0)
			{
				this._dragCompletedLock = true;
				if (this._oldZoomState.MinXValue != null && this._oldZoomState.MaxXValue != null)
				{
					this._zoomStateStack.Push(new Axis.ZoomState(this._oldZoomState.MinXValue, this._oldZoomState.MaxXValue));
				}
				this._oldZoomState.MinXValue = this.ViewMinimum;
				this._oldZoomState.MaxXValue = this.ViewMaximum;
				chart._zoomOutTextBlock.Visibility = Visibility.Visible;
				chart._zoomIconSeparater.Visibility = Visibility.Visible;
				chart._showAllTextBlock.Visibility = Visibility.Visible;
			}
			if (this.ScrollBarElement.Scale == 1.0)
			{
				this.ResetZoomState(chart, true);
			}
		}

		internal void ResetZoomState(Chart chart, bool initializeStack)
		{
			chart._showAllTextBlock.Visibility = Visibility.Collapsed;
			chart._zoomIconSeparater.Visibility = Visibility.Collapsed;
			chart._zoomOutTextBlock.Visibility = Visibility.Collapsed;
			this._zoomStateStack.Clear();
			if (initializeStack)
			{
				if (this.ViewMinimum != null && this.ViewMaximum != null)
				{
					this._zoomStateStack.Push(new Axis.ZoomState(this.ViewMinimum, this.ViewMaximum));
				}
			}
			else
			{
				this._initialState.MinXValue = null;
				this._initialState.MaxXValue = null;
			}
			this._oldZoomState.MinXValue = null;
			this._oldZoomState.MaxXValue = null;
		}

		private void ScrollBarElement_DragStarted(object sender, EventArgs e)
		{
			ZoomBar zoomBar = sender as ZoomBar;
			zoomBar.ScrollEventFireEnabled = false;
		}

		private void OnZoomingScaleChanged(object sender, EventArgs e)
		{
			ZoomBar zoomBar = sender as ZoomBar;
			zoomBar.ScrollEventFireEnabled = true;
			if (this.ZoomingScaleChanged != null)
			{
				this.ZoomingScaleChanged(this, e);
			}
			this._dragCompletedLock = false;
		}

		internal void FireZoomEvent(Axis.ZoomState zoomState, EventArgs e)
		{
			if (this._onZoom != null)
			{
				this._onZoom(this, this.CreateAxisZoomEventArgs(zoomState, e));
			}
		}

		internal AxisZoomEventArgs CreateAxisZoomEventArgs(Axis.ZoomState zoomState, EventArgs e)
		{
			return new AxisZoomEventArgs(e)
			{
				MinValue = zoomState.MinXValue,
				MaxValue = zoomState.MaxXValue
			};
		}

		internal static void SaveOldValueOfAxisRange(Axis axis)
		{
			if (axis != null)
			{
				axis._oldInternalAxisMaximum = axis.InternalAxisMaximum;
				axis._oldInternalAxisMinimum = axis.InternalAxisMinimum;
			}
		}

		internal void ClearEventHandlers()
		{
			if (this.ScrollBarElement != null)
			{
				this.ScrollBarElement.ScaleChanged -= new EventHandler(this.OnZoomingScaleChanged);
				this.ScrollBarElement.DragStarted -= new EventHandler(this.ScrollBarElement_DragStarted);
				this.ScrollBarElement.DragCompleted -= new EventHandler(this.ScrollBarElement_DragCompleted);
			}
		}

		internal void ClearReferences()
		{
			if (this.Visual != null)
			{
				if (this.AxisElementsContainer != null)
				{
					this.AxisElementsContainer.Children.Clear();
				}
				if (this.InternalStackPanel != null)
				{
					this.InternalStackPanel.Children.Clear();
				}
				if (this.ScrollViewerElement != null)
				{
					this.ScrollViewerElement.Children.Clear();
				}
				this.AxisTitleElement = null;
				this.Visual.Children.Clear();
				this.Visual.UpdateLayout();
			}
			if (this.CircularAxisVisual != null)
			{
				if (this.AxisElementsContainer != null)
				{
					this.AxisElementsContainer.Children.Clear();
				}
				if (this.InternalStackPanel != null)
				{
					this.InternalStackPanel.Children.Clear();
				}
				if (this.ScrollViewerElement != null)
				{
					this.ScrollViewerElement.Children.Clear();
				}
				this.AxisTitleElement = null;
				this.CircularAxisVisual.Children.Clear();
				this.CircularAxisVisual.UpdateLayout();
			}
		}

		internal override void ClearInstanceRefs()
		{
			foreach (ChartGrid current in this.Grids)
			{
				if (current.Storyboard != null)
				{
					current.Storyboard.FillBehavior = FillBehavior.Stop;
					current.Storyboard.Stop();
					current.Storyboard = null;
				}
			}
			if (this.Storyboard != null)
			{
				this.Storyboard.FillBehavior = FillBehavior.Stop;
				this.Storyboard.Stop();
				this.Storyboard = null;
			}
		}

		private void ApplyAxisProperties4CircularChart()
		{
			this.AxisElementsContainer.Cursor = ((this.Cursor == null) ? Cursors.Arrow : this.Cursor);
			base.AttachHref(base.Chart, this.AxisElementsContainer, this.Href, this.HrefTarget);
			base.AttachToolTip(base.Chart, this, this.AxisElementsContainer);
			base.AttachEvents2Visual(this, this.AxisElementsContainer);
			this.AxisElementsContainer.Opacity = this.InternalOpacity;
		}

		private void ApplyCircularPathProperties()
		{
			this.CircularPath.Stroke = this.LineColor;
			this.CircularPath.StrokeThickness = this.LineThickness.Value;
			this.CircularPath.Fill = this.Background;
		}

		private void CreateAxesForCircularChart()
		{
			if (this.AxisOrientation == AxisOrientation.Circular)
			{
				this.CircularAxisVisual = new Canvas
				{
					Width = this.Width,
					Height = this.Height
				};
				this.SetUpAxisManager();
				this.SetUpAxisLabels();
				base.IsNotificationEnable = true;
				this.AxisLabels.IsNotificationEnable = true;
				this.ApplyAxisSettings4CircularChart();
				if (!this.Enabled.Value)
				{
					this.CircularAxisVisual.Visibility = Visibility.Collapsed;
				}
				if ((base.Chart as Chart)._internalAnimationEnabled)
				{
					this.Storyboard = new Storyboard();
					this.CircularAxisVisual.RenderTransformOrigin = new Point(0.5, 0.5);
					double num = 0.2;
					AnimationHelper.ApplyScaleAnimation(ScaleDirection.ScaleX, this.Storyboard, this.CircularAxisVisual, 0.0, 1.0, new TimeSpan(0, 0, 0, 1, 200), num, true);
					AnimationHelper.ApplyScaleAnimation(ScaleDirection.ScaleY, this.Storyboard, this.CircularAxisVisual, 0.0, 1.0, new TimeSpan(0, 0, 0, 1, 200), num, true);
					if (this.AxisLabels.Visual != null)
					{
						Canvas visual = this.AxisLabels.Visual;
						AnimationHelper.ApplyOpacityAnimation(visual, this.AxisLabels, this.Storyboard, num + 0.5, 1.0, 0.0, 1.0);
						return;
					}
				}
			}
			else
			{
				this.CircularAxisVisual = new Canvas
				{
					Width = this.Width,
					Height = this.Height
				};
				this.SetUpAxisManager();
				this.SetUpTicks();
				this.SetUpGrids();
				this.SetUpAxisLabels();
				this.ApplyAxisSettings4CircularChart();
				if (!this.Enabled.Value)
				{
					this.CircularAxisVisual.Visibility = Visibility.Collapsed;
				}
				if ((base.Chart as Chart)._internalAnimationEnabled)
				{
					this.Storyboard = new Storyboard();
					this.CircularAxisVisual.RenderTransformOrigin = new Point(0.5, 0.5);
					double num2 = 0.2;
					AnimationHelper.ApplyScaleAnimation(ScaleDirection.ScaleX, this.Storyboard, this.CircularAxisVisual, 0.0, 1.0, new TimeSpan(0, 0, 0, 1, 200), num2, true);
					AnimationHelper.ApplyScaleAnimation(ScaleDirection.ScaleY, this.Storyboard, this.CircularAxisVisual, 0.0, 1.0, new TimeSpan(0, 0, 0, 1, 200), num2, true);
					if (this.AxisLabels.Visual != null)
					{
						Canvas visual2 = this.AxisLabels.Visual;
						AnimationHelper.ApplyOpacityAnimation(visual2, this.AxisLabels, this.Storyboard, num2 + 0.5, 1.0, 0.0, 1.0);
					}
				}
			}
		}

		private void ApplyAxisSettings4CircularChart()
		{
			if (this.AxisOrientation == AxisOrientation.Circular)
			{
				this.CircularPath = new Path();
				this.AxisLabels.Placement = PlacementTypes.Circular;
				this.AxisLabels.Width = this.Width;
				this.AxisLabels.Height = this.Height;
				this.AxisLabels.CreateVisualObject();
				if (this.AxisLabels.Visual != null)
				{
					this.CircularAxisVisual.Children.Add(this.AxisLabels.Visual);
				}
				this.ApplyCircularPathProperties();
				if (this.CircularPlotDetails.ListOfPoints4CircularAxis.Count > 0)
				{
					this.CircularPath.Data = this.GetPathGeometry(this.CircularPlotDetails.ListOfPoints4CircularAxis);
				}
				this.CircularAxisVisual.Children.Add(this.CircularPath);
				this.CircularAxisVisual.Cursor = ((this.Cursor == null) ? Cursors.Arrow : this.Cursor);
				base.AttachHref(base.Chart, this.CircularAxisVisual, this.Href, this.HrefTarget);
				base.AttachToolTip(base.Chart, this, this.CircularAxisVisual);
				base.AttachEvents2Visual(this, this.CircularAxisVisual);
				this.CircularAxisVisual.Opacity = this.InternalOpacity;
				return;
			}
			if (this.CircularPlotDetails.ListOfPoints4CircularAxis.Count == 0)
			{
				return;
			}
			double height = Math.Max(this.CircularPlotDetails.Radius, 0.0);
			this.AxisLabels.Placement = PlacementTypes.Left;
			this.AxisLabels.Height = height;
			for (int i = 0; i < this.CircularPlotDetails.ListOfPoints4CircularAxis.Count; i++)
			{
				if (this.CircularPlotDetails.CircularChartType == RenderAs.Radar)
				{
					if (i == 0)
					{
						this.CreateAxisYElements4RadarChart(height, this.CircularPlotDetails.ListOfPoints4CircularAxis[i], this.CircularPlotDetails.MinAngleInDegree, (double)i, this.CircularPlotDetails.Center, true);
					}
					else
					{
						this.CreateAxisYElements4RadarChart(height, this.CircularPlotDetails.ListOfPoints4CircularAxis[i], this.CircularPlotDetails.MinAngleInDegree, (double)i, this.CircularPlotDetails.Center, false);
					}
				}
				else
				{
					double minAngleInDegree;
					if (this.CircularPlotDetails.MinAngleInDegree > 360.0)
					{
						double num = 0.0;
						int num2 = Convert.ToInt32(Math.Floor(this.CircularPlotDetails.MinAngleInDegree / 360.0));
						for (int j = 1; j <= num2; j++)
						{
							num = Math.Max(num, (double)(j * 360));
						}
						minAngleInDegree = this.CircularPlotDetails.MinAngleInDegree - num;
					}
					else
					{
						minAngleInDegree = this.CircularPlotDetails.MinAngleInDegree;
					}
					this.CreateAxisYElements4PolarChart(height, minAngleInDegree, this.CircularPlotDetails.ListOfPoints4CircularAxis[i], this.CircularPlotDetails.AnglesInRadian[i], (double)i, this.CircularPlotDetails.Center);
				}
			}
			this.CleanUpGrids();
			foreach (ChartGrid current in this.Grids)
			{
				current.IsNotificationEnable = false;
				current.Chart = base.Chart;
				current.ApplyStyleFromTheme(base.Chart, "Grid");
				if (current.Visual == null)
				{
					current.CreateVisualObject(this.Width, height, (base.Chart as Chart)._internalAnimationEnabled, ChartArea.GRID_ANIMATION_DURATION);
					if (current.Visual != null)
					{
						this.CircularAxisVisual.Children.Add(current.Visual);
					}
				}
				else
				{
					current.CreateVisualObject(this.Width, height, (base.Chart as Chart)._internalAnimationEnabled, ChartArea.GRID_ANIMATION_DURATION);
				}
				if (current.Visual != null)
				{
					current.Visual.SetValue(Panel.ZIndexProperty, -1000);
				}
				current.IsNotificationEnable = true;
			}
		}

		private void CleanUpGrids()
		{
			foreach (ChartGrid current in this.Grids)
			{
				if (current.Visual != null)
				{
					this.CircularAxisVisual.Children.Remove(current.Visual);
					current.Visual.Children.Clear();
					current.CachedGridLines.Clear();
					current.CachedGridRectangles.Clear();
					current.Visual = null;
				}
			}
		}

		internal double CalculateAngleByCoordinate(Point position, Point center)
		{
			return Math.Atan2(position.Y - center.Y, position.X - center.X);
		}

		private void CreateAxisYElements4PolarChart(double height, double minAngleInDegree, Point point, double angle, double index, Point center)
		{
			this.AxisElementsContainer = new StackPanel
			{
				Background = this.InternalBackground
			};
			this.AxisElementsContainer.Orientation = Orientation.Horizontal;
			this.ApplyAxisProperties4CircularChart();
			this.CreateAxisLine(0.0, height, this.LineThickness.Value / 2.0, this.LineThickness.Value / 2.0, this.LineThickness.Value, height);
			RotateTransform rotateTransform = new RotateTransform();
			double num = CircularLabel.ResetMeanAngle(this.CalculateAngleByCoordinate(point, center)) * 180.0 / 3.1415926535897931;
			rotateTransform.Angle = num + 90.0;
			double num2 = 0.0;
			if (Math.Round(minAngleInDegree) == Math.Round(num))
			{
				this.AxisLabels.CreateVisualObject();
				if (this.AxisLabels.Visual != null)
				{
					this.AxisElementsContainer.Children.Add(this.AxisLabels.Visual);
					if (!double.IsNaN(this.AxisLabels.Visual.Width))
					{
						num2 = this.AxisLabels.Visual.Width;
					}
				}
			}
			double num3 = 0.0;
			List<Ticks> list = this.Ticks.Reverse<Ticks>().ToList<Ticks>();
			foreach (Ticks current in list)
			{
				current.SetParms(PlacementTypes.Left, double.NaN, height);
				current.CreateVisualObject();
				if (current.Visual != null)
				{
					if (!this.AxisElementsContainer.Children.Contains(current.Visual))
					{
						this.AxisElementsContainer.Children.Add(current.Visual);
					}
					num3 += current.Visual.Width;
				}
			}
			if (this.AxisLine != null)
			{
				this.AxisElementsContainer.Children.Add(this.AxisLine);
			}
			this.AxisElementsContainer.SetValue(Canvas.LeftProperty, point.X - num2 - num3);
			this.AxisElementsContainer.SetValue(Canvas.TopProperty, point.Y);
			if (rotateTransform != null)
			{
				this.AxisElementsContainer.RenderTransformOrigin = new Point(1.0, 0.0);
				this.AxisElementsContainer.RenderTransform = rotateTransform;
			}
			this.CircularAxisVisual.Children.Add(this.AxisElementsContainer);
		}

		private void CreateAxisYElements4RadarChart(double height, Point point, double minAngle, double index, Point center, bool isAxisLabelsEnabled)
		{
			this.AxisElementsContainer = new StackPanel
			{
				Background = this.InternalBackground
			};
			this.AxisElementsContainer.Orientation = Orientation.Horizontal;
			this.ApplyAxisProperties4CircularChart();
			this.CreateAxisLine(0.0, height, this.LineThickness.Value / 2.0, this.LineThickness.Value / 2.0, this.LineThickness.Value, height);
			RotateTransform rotateTransform = null;
			if (isAxisLabelsEnabled)
			{
				this.AxisLabels.CreateVisualObject();
				if (this.AxisLabels.Visual != null)
				{
					this.AxisElementsContainer.Children.Add(this.AxisLabels.Visual);
				}
			}
			else
			{
				rotateTransform = new RotateTransform();
				rotateTransform.Angle = minAngle * index;
			}
			double num = 0.0;
			List<Ticks> list = this.Ticks.Reverse<Ticks>().ToList<Ticks>();
			foreach (Ticks current in list)
			{
				current.SetParms(PlacementTypes.Left, double.NaN, height);
				current.CreateVisualObject();
				if (current.Visual != null)
				{
					if (!this.AxisElementsContainer.Children.Contains(current.Visual))
					{
						this.AxisElementsContainer.Children.Add(current.Visual);
					}
					num += current.Visual.Width;
				}
			}
			if (this.AxisLine != null)
			{
				this.AxisElementsContainer.Children.Add(this.AxisLine);
			}
			double num2 = 0.0;
			if (isAxisLabelsEnabled && this.AxisLabels.Visual != null && !double.IsNaN(this.AxisLabels.Visual.Width))
			{
				num2 = this.AxisLabels.Visual.Width;
			}
			this.AxisElementsContainer.SetValue(Canvas.LeftProperty, point.X - num2 - num);
			this.AxisElementsContainer.SetValue(Canvas.TopProperty, point.Y);
			if (!isAxisLabelsEnabled && rotateTransform != null)
			{
				this.AxisElementsContainer.RenderTransformOrigin = new Point(0.75, 0.0);
				this.AxisElementsContainer.RenderTransform = rotateTransform;
			}
			this.CircularAxisVisual.Children.Add(this.AxisElementsContainer);
		}

		internal Geometry GetPathGeometry(List<Point> listOfCircularPoints)
		{
			PathGeometry pathGeometry = new PathGeometry();
			PathFigure pathFigure = new PathFigure();
			pathFigure.StartPoint = listOfCircularPoints[0];
			for (int i = 1; i < listOfCircularPoints.Count; i++)
			{
				LineSegment lineSegment = new LineSegment();
				lineSegment.Point = listOfCircularPoints[i];
				pathFigure.Segments.Add(lineSegment);
			}
			LineSegment lineSegment2 = new LineSegment();
			lineSegment2.Point = listOfCircularPoints[0];
			pathFigure.Segments.Add(lineSegment2);
			pathGeometry.Figures.Add(pathFigure);
			return pathGeometry;
		}

		internal static void GetLeftAndRightLabels(List<CircularAxisLabel> labels, out List<CircularAxisLabel> labelsAtLeft, out List<CircularAxisLabel> labelsAtRight)
		{
			List<CircularAxisLabel> collection = (from cl in labels
			where cl.Angle >= 4.71238898038469
			orderby cl.Angle
			select cl).ToList<CircularAxisLabel>();
			List<CircularAxisLabel> collection2 = (from cl in labels
			where cl.Angle >= 0.0 && cl.Angle <= 1.5707963267948966
			orderby cl.Angle
			select cl).ToList<CircularAxisLabel>();
			labelsAtRight = new List<CircularAxisLabel>();
			labelsAtRight.AddRange(collection);
			labelsAtRight.AddRange(collection2);
			labelsAtLeft = (from cl in labels
			where cl.Angle > 1.5707963267948966 && cl.Angle < 4.71238898038469
			orderby cl.Angle
			select cl).ToList<CircularAxisLabel>();
		}
	}
}
