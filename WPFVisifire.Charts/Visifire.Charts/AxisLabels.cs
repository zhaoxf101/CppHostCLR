using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class AxisLabels : ObservableObject, IElement
	{
		private const double CONSTANT_GAP_BETWEEN_AXISLABELS = 4.0;

		private const double EXTRA_CONSTANT_GAP_BETWEEN_AXISLABELS = 10.0;

		public static readonly DependencyProperty IntervalProperty;

		public static readonly DependencyProperty AngleProperty;

		public static readonly DependencyProperty EnabledProperty;

		public static readonly DependencyProperty TextAlignmentProperty;

		public new static readonly DependencyProperty MaxHeightProperty;

		public new static readonly DependencyProperty MinHeightProperty;

		public new static readonly DependencyProperty MinWidthProperty;

		public new static readonly DependencyProperty MaxWidthProperty;

		public new static readonly DependencyProperty FontFamilyProperty;

		public new static readonly DependencyProperty OpacityProperty;

		public static readonly DependencyProperty FontColorProperty;

		public new static readonly DependencyProperty FontStyleProperty;

		public new static readonly DependencyProperty FontWeightProperty;

		public new static readonly DependencyProperty FontSizeProperty;

		public static readonly DependencyProperty TextWrapProperty;

		public static readonly DependencyProperty RowsProperty;

		private new static readonly DependencyProperty ToolTipTextProperty;

		private double _maxRowHeight;

		private double _savedFontSize;

		private bool _isRedraw;

		private Axis _parent;

		private double _internalFontSize = double.NaN;

		private FontFamily _internalFontFamily;

		private FontStyle? _internalFontStyle = null;

		private FontWeight? _internalFontWeight = null;

		private double _internalOpacity = double.NaN;

		private double _internalMaxHeight = double.NaN;

		private double _internalMaxWidth = double.NaN;

		private double _internalMinHeight = double.NaN;

		private double _internalMinWidth = double.NaN;

		private static bool _defaultStyleKeyApplied;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToolTipText
		{
			get
			{
				throw new NotImplementedException("ToolTipText property for AxisLabels is not implemented");
			}
			set
			{
				throw new NotImplementedException("ToolTipText property for AxisLabels is not implemented");
			}
		}

		public new double MaxHeight
		{
			get
			{
				return (double)base.GetValue(AxisLabels.MaxHeightProperty);
			}
			set
			{
				base.SetValue(AxisLabels.MaxHeightProperty, value);
			}
		}

		public new double MaxWidth
		{
			get
			{
				return (double)base.GetValue(AxisLabels.MaxWidthProperty);
			}
			set
			{
				base.SetValue(AxisLabels.MaxWidthProperty, value);
			}
		}

		public new double MinHeight
		{
			get
			{
				return (double)base.GetValue(AxisLabels.MinHeightProperty);
			}
			set
			{
				base.SetValue(AxisLabels.MinHeightProperty, value);
			}
		}

		public new double MinWidth
		{
			get
			{
				return (double)base.GetValue(AxisLabels.MinWidthProperty);
			}
			set
			{
				base.SetValue(AxisLabels.MinWidthProperty, value);
			}
		}

		public double? Interval
		{
			get
			{
				if (!((double?)base.GetValue(AxisLabels.IntervalProperty)).HasValue)
				{
					return new double?(double.NaN);
				}
				return (double?)base.GetValue(AxisLabels.IntervalProperty);
			}
			set
			{
				base.SetValue(AxisLabels.IntervalProperty, value);
			}
		}

		public double? Angle
		{
			get
			{
				if (!((double?)base.GetValue(AxisLabels.AngleProperty)).HasValue)
				{
					return this.InternalAngle;
				}
				return (double?)base.GetValue(AxisLabels.AngleProperty);
			}
			set
			{
				base.SetValue(AxisLabels.AngleProperty, value);
			}
		}

		public new double Opacity
		{
			get
			{
				return (double)base.GetValue(AxisLabels.OpacityProperty);
			}
			set
			{
				base.SetValue(AxisLabels.OpacityProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Enabled
		{
			get
			{
				if (!((bool?)base.GetValue(AxisLabels.EnabledProperty)).HasValue)
				{
					return new bool?(true);
				}
				return (bool?)base.GetValue(AxisLabels.EnabledProperty);
			}
			set
			{
				base.SetValue(AxisLabels.EnabledProperty, value);
			}
		}

		public TextAlignment TextAlignment
		{
			get
			{
				return (TextAlignment)base.GetValue(AxisLabels.TextAlignmentProperty);
			}
			set
			{
				base.SetValue(AxisLabels.TextAlignmentProperty, value);
			}
		}

		public new FontFamily FontFamily
		{
			get
			{
				if ((FontFamily)base.GetValue(AxisLabels.FontFamilyProperty) == null)
				{
					return new FontFamily("Arial");
				}
				return (FontFamily)base.GetValue(AxisLabels.FontFamilyProperty);
			}
			set
			{
				base.SetValue(AxisLabels.FontFamilyProperty, value);
			}
		}

		public Brush FontColor
		{
			get
			{
				return (Brush)base.GetValue(AxisLabels.FontColorProperty);
			}
			set
			{
				base.SetValue(AxisLabels.FontColorProperty, value);
			}
		}

		[TypeConverter(typeof(FontStyleConverter))]
		public new FontStyle FontStyle
		{
			get
			{
				return (FontStyle)base.GetValue(AxisLabels.FontStyleProperty);
			}
			set
			{
				base.SetValue(AxisLabels.FontStyleProperty, value);
			}
		}

		[TypeConverter(typeof(FontWeightConverter))]
		public new FontWeight FontWeight
		{
			get
			{
				return (FontWeight)base.GetValue(AxisLabels.FontWeightProperty);
			}
			set
			{
				base.SetValue(AxisLabels.FontWeightProperty, value);
			}
		}

		public new double FontSize
		{
			get
			{
				return (double)base.GetValue(AxisLabels.FontSizeProperty);
			}
			set
			{
				base.SetValue(AxisLabels.FontSizeProperty, value);
			}
		}

		public int? Rows
		{
			get
			{
				if (((int?)base.GetValue(AxisLabels.RowsProperty)).HasValue)
				{
					return (int?)base.GetValue(AxisLabels.RowsProperty);
				}
				return new int?(0);
			}
			set
			{
				this.InternalRows = ((!value.HasValue) ? new int?(0) : value).Value;
				base.SetValue(AxisLabels.RowsProperty, value);
			}
		}

		internal DependencyObject ParentTree
		{
			get
			{
				return base.Parent;
			}
		}

		public new Axis Parent
		{
			get
			{
				return this._parent;
			}
			internal set
			{
				this._parent = value;
			}
		}

		internal double InternalMaxHeight
		{
			get
			{
				return (double)(double.IsNaN(this._internalMaxHeight) ? base.GetValue(AxisLabels.MaxHeightProperty) : this._internalMaxHeight);
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
				return (double)(double.IsNaN(this._internalMaxWidth) ? base.GetValue(AxisLabels.MaxWidthProperty) : this._internalMaxWidth);
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
				return (double)(double.IsNaN(this._internalMinHeight) ? base.GetValue(AxisLabels.MinHeightProperty) : this._internalMinHeight);
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
				return (double)(double.IsNaN(this._internalMinWidth) ? base.GetValue(AxisLabels.MinWidthProperty) : this._internalMinWidth);
			}
			set
			{
				this._internalMinWidth = value;
			}
		}

		internal FontFamily InternalFontFamily
		{
			get
			{
				FontFamily fontFamily;
				if (this._internalFontFamily == null)
				{
					fontFamily = (FontFamily)base.GetValue(AxisLabels.FontFamilyProperty);
				}
				else
				{
					fontFamily = this._internalFontFamily;
				}
				if (fontFamily != null)
				{
					return fontFamily;
				}
				return new FontFamily("Verdana");
			}
			set
			{
				this._internalFontFamily = value;
			}
		}

		internal double InternalFontSize
		{
			get
			{
				return (double)(double.IsNaN(this._internalFontSize) ? base.GetValue(AxisLabels.FontSizeProperty) : this._internalFontSize);
			}
			set
			{
				this._internalFontSize = value;
			}
		}

		[TypeConverter(typeof(FontStyleConverter))]
		internal FontStyle InternalFontStyle
		{
			get
			{
				return (FontStyle)((!this._internalFontStyle.HasValue) ? base.GetValue(AxisLabels.FontStyleProperty) : this._internalFontStyle);
			}
			set
			{
				this._internalFontStyle = new FontStyle?(value);
			}
		}

		[TypeConverter(typeof(FontWeightConverter))]
		internal FontWeight InternalFontWeight
		{
			get
			{
				return (FontWeight)((!this._internalFontWeight.HasValue) ? base.GetValue(AxisLabels.FontWeightProperty) : this._internalFontWeight);
			}
			set
			{
				this._internalFontWeight = new FontWeight?(value);
			}
		}

		internal double InternalOpacity
		{
			get
			{
				return (double)(double.IsNaN(this._internalOpacity) ? base.GetValue(AxisLabels.OpacityProperty) : this._internalOpacity);
			}
			set
			{
				this._internalOpacity = value;
			}
		}

		internal double WidthOfACharacter
		{
			get;
			set;
		}

		internal double? InternalAngle
		{
			get;
			set;
		}

		internal Canvas Visual
		{
			get;
			private set;
		}

		public double TextWrap
		{
			get
			{
				return (double)base.GetValue(AxisLabels.TextWrapProperty);
			}
			set
			{
				base.SetValue(AxisLabels.TextWrapProperty, value);
			}
		}

		internal int InternalRows
		{
			get;
			set;
		}

		internal double Minimum
		{
			get;
			set;
		}

		internal double Maximum
		{
			get;
			set;
		}

		internal double DataMinimum
		{
			get;
			set;
		}

		internal double DataMaximum
		{
			get;
			set;
		}

		internal new double Width
		{
			get;
			set;
		}

		internal new double Height
		{
			get;
			set;
		}

		internal PlacementTypes Placement
		{
			get;
			set;
		}

		internal Dictionary<double, string> AxisLabelContentDictionary
		{
			get;
			set;
		}

		internal bool AllAxisLabels
		{
			get;
			set;
		}

		internal Axis ParentAxis
		{
			get;
			set;
		}

		internal double TopOverflow
		{
			get;
			set;
		}

		internal double BottomOverflow
		{
			get;
			set;
		}

		internal double LeftOverflow
		{
			get;
			set;
		}

		internal double RightOverflow
		{
			get;
			set;
		}

		internal List<AxisLabel> AxisLabelList
		{
			get;
			set;
		}

		internal List<double> LabelValues
		{
			get;
			set;
		}

		private ChartOrientationType OldChartOrientation
		{
			get;
			set;
		}

		public AxisLabels()
		{
			this.WidthOfACharacter = double.NaN;
			this.InternalAngle = new double?(double.NaN);
		}

		static AxisLabels()
		{
			AxisLabels.IntervalProperty = DependencyProperty.Register("Interval", typeof(double?), typeof(AxisLabels), new PropertyMetadata(new PropertyChangedCallback(AxisLabels.OnIntervalPropertyChanged)));
			AxisLabels.AngleProperty = DependencyProperty.Register("Angle", typeof(double?), typeof(AxisLabels), new PropertyMetadata(new PropertyChangedCallback(AxisLabels.OnAnglePropertyChanged)));
			AxisLabels.EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool?), typeof(AxisLabels), new PropertyMetadata(new PropertyChangedCallback(AxisLabels.OnEnabledPropertyChanged)));
			AxisLabels.TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(AxisLabels), new PropertyMetadata(TextAlignment.Left, new PropertyChangedCallback(AxisLabels.OnTextAlignmentPropertyChanged)));
			AxisLabels.MaxHeightProperty = DependencyProperty.Register("MaxHeight", typeof(double), typeof(AxisLabels), new PropertyMetadata(double.PositiveInfinity, new PropertyChangedCallback(AxisLabels.OnMaxHeightPropertyChanged)));
			AxisLabels.MinHeightProperty = DependencyProperty.Register("MinHeight", typeof(double), typeof(AxisLabels), new PropertyMetadata(new PropertyChangedCallback(AxisLabels.OnMinHeightPropertyChanged)));
			AxisLabels.MinWidthProperty = DependencyProperty.Register("MinWidth", typeof(double), typeof(AxisLabels), new PropertyMetadata(new PropertyChangedCallback(AxisLabels.OnMinWidthPropertyChanged)));
			AxisLabels.MaxWidthProperty = DependencyProperty.Register("MaxWidth", typeof(double), typeof(AxisLabels), new PropertyMetadata(double.PositiveInfinity, new PropertyChangedCallback(AxisLabels.OnMaxWidthPropertyChanged)));
			AxisLabels.FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(AxisLabels), new PropertyMetadata(new PropertyChangedCallback(AxisLabels.OnFontFamilyPropertyChanged)));
			AxisLabels.OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(AxisLabels), new PropertyMetadata(1.0, new PropertyChangedCallback(AxisLabels.OnOpacityPropertyChanged)));
			AxisLabels.FontColorProperty = DependencyProperty.Register("FontColor", typeof(Brush), typeof(AxisLabels), new PropertyMetadata(new PropertyChangedCallback(AxisLabels.OnFontColorPropertyChanged)));
			AxisLabels.FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(AxisLabels), new PropertyMetadata(new PropertyChangedCallback(AxisLabels.OnFontStylePropertyChanged)));
			AxisLabels.FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(AxisLabels), new PropertyMetadata(new PropertyChangedCallback(AxisLabels.OnFontWeightPropertyChanged)));
			AxisLabels.FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(AxisLabels), new PropertyMetadata(new PropertyChangedCallback(AxisLabels.OnFontSizePropertyChanged)));
			AxisLabels.TextWrapProperty = DependencyProperty.Register("TextWrap", typeof(double), typeof(AxisLabels), new PropertyMetadata(double.NaN, new PropertyChangedCallback(AxisLabels.OnTextWrapPropertyChanged)));
			AxisLabels.RowsProperty = DependencyProperty.Register("Rows", typeof(int?), typeof(AxisLabels), new PropertyMetadata(new PropertyChangedCallback(AxisLabels.OnRowsPropertyChanged)));
			AxisLabels.ToolTipTextProperty = DependencyProperty.Register("ToolTipText", typeof(string), typeof(AxisLabels), null);
			if (!AxisLabels._defaultStyleKeyApplied)
			{
				FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AxisLabels), new FrameworkPropertyMetadata(typeof(AxisLabels)));
				AxisLabels._defaultStyleKeyApplied = true;
			}
		}

		internal override void Bind()
		{
		}

		private static void OnFontColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.UpdateVisual(VcProperties.FontColor, e.NewValue);
		}

		private static void OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.FirePropertyChanged(VcProperties.Interval);
		}

		private static void OnAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.InternalAngle = (double?)e.NewValue;
			axisLabels.FirePropertyChanged(VcProperties.Angle);
		}

		private static void OnEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.FirePropertyChanged(VcProperties.Enabled);
		}

		private static void OnTextAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.FirePropertyChanged(VcProperties.TextAlignment);
		}

		private static void OnFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			if (e.NewValue == null || e.OldValue == null)
			{
				axisLabels.InternalFontFamily = (FontFamily)e.NewValue;
				axisLabels.FirePropertyChanged(VcProperties.FontFamily);
				return;
			}
			if (e.NewValue.ToString() != e.OldValue.ToString())
			{
				axisLabels.InternalFontFamily = (FontFamily)e.NewValue;
				axisLabels.FirePropertyChanged(VcProperties.FontFamily);
			}
		}

		private static void OnFontStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.InternalFontStyle = (FontStyle)e.NewValue;
			axisLabels.UpdateVisual(VcProperties.FontStyle, e.NewValue);
		}

		private static void OnFontWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.InternalFontWeight = (FontWeight)e.NewValue;
			axisLabels.UpdateVisual(VcProperties.FontWeight, e.NewValue);
		}

		private static void OnFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.InternalFontSize = (double)e.NewValue;
			axisLabels.FirePropertyChanged(VcProperties.FontSize);
		}

		private static void OnOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.InternalOpacity = (double)e.NewValue;
			axisLabels.FirePropertyChanged(VcProperties.Opacity);
		}

		private static void OnMaxHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.InternalMaxHeight = (double)e.NewValue;
			axisLabels.FirePropertyChanged(VcProperties.MaxHeight);
		}

		private static void OnMinHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.InternalMinHeight = (double)e.NewValue;
			axisLabels.FirePropertyChanged(VcProperties.MinHeight);
		}

		private static void OnMaxWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.InternalMaxWidth = (double)e.NewValue;
			axisLabels.FirePropertyChanged(VcProperties.MaxWidth);
		}

		private static void OnMinWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.InternalMinWidth = (double)e.NewValue;
			axisLabels.FirePropertyChanged(VcProperties.MinWidth);
		}

		private static void OnTextWrapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			if ((double)e.NewValue < 0.0 || (double)e.NewValue > 1.0)
			{
				throw new Exception("Wrong property value. Range of TextWrapProperty varies from 0 to 1.");
			}
			axisLabels.FirePropertyChanged(VcProperties.TextWrap);
		}

		private static void OnRowsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisLabels axisLabels = d as AxisLabels;
			axisLabels.FirePropertyChanged(VcProperties.Rows);
		}

		private AxisLabel CreateLabel(string text)
		{
			AxisLabel axisLabel = new AxisLabel();
			axisLabel.Text = text;
			axisLabel.Placement = this.Placement;
			if (!double.IsNaN(this.TextWrap))
			{
				double num = (this.ParentAxis.PlotDetails.ChartOrientation == ChartOrientationType.Vertical) ? base.Chart.ActualHeight : base.Chart.ActualWidth;
				axisLabel.MaxWidth = num * this.TextWrap;
			}
			else
			{
				axisLabel.MaxWidth = double.NaN;
			}
			return axisLabel;
		}

		private double CalculateAutoInterval(double CurrentInterval, double AxisWidth, int NoOfLabels, double Angle, int Rows)
		{
			double result = 1.0;
			Angle = (double.IsNaN(Angle) ? 0.0 : Angle);
			this.CalculateHorizontalOverflow();
			return result;
		}

		private bool CreateLabels()
		{
			double num = this.Interval.Value;
			if (double.IsNaN(num) || num <= 0.0)
			{
				num = this.ParentAxis.InternalInterval;
			}
			decimal num2 = (decimal)this.Minimum;
			decimal num3 = (decimal)this.Minimum;
			decimal num4 = (decimal)this.Maximum;
			decimal d = (decimal)num;
			int num5 = 0;
			if (this.ParentAxis.AxisRepresentation == AxisRepresentations.AxisX)
			{
				if (this.ParentAxis.AxisOrientation != AxisOrientation.Circular && double.IsNaN(this.Parent.AxisMinimumNumeric))
				{
					if (this.ParentAxis.XValueType != ChartValueTypes.Numeric)
					{
						num2 = (decimal)this.ParentAxis.FirstLabelPosition;
					}
					else if ((this.DataMinimum - this.Minimum) / num >= 1.0)
					{
						num2 = (decimal)(this.DataMinimum - Math.Floor((this.DataMinimum - this.Minimum) / num) * num);
					}
					else
					{
						num2 = (decimal)this.DataMinimum;
					}
				}
				num3 = num2;
				if (!(num3 != num4))
				{
					return false;
				}
				if (!double.IsNaN(this.TextWrap))
				{
					this.CalculatAvgWidthOfAChar();
				}
				if (this.ParentAxis.AxisOrientation != AxisOrientation.Circular)
				{
					int num6 = 0;
					Chart chart = base.Chart as Chart;
					if ((chart.ScrollingEnabled.Value || chart.ZoomingEnabled) && ((chart.ChartArea.AxisY != null && chart.ChartArea.AxisY.ViewportRangeEnabled) || (chart.ChartArea.AxisY2 != null && chart.ChartArea.AxisY2.ViewportRangeEnabled)))
					{
						List<double> list = new List<double>();
						while (num2 <= num4)
						{
							list.Add((double)num2);
							if (this.ParentAxis.IsDateTimeAxis)
							{
								num6++;
								DateTime dateTime = DateTimeHelper.UpdateDate(this.Parent.FirstLabelDate, (double)(num6 * d), this.ParentAxis.InternalIntervalType);
								decimal d2 = (decimal)DateTimeHelper.DateDiff(dateTime, this.Parent.FirstLabelDate, this.ParentAxis.MinDateRange, this.ParentAxis.MaxDateRange, this.ParentAxis.InternalIntervalType, this.ParentAxis.XValueType);
								num2 = Math.Round(num3 + d2, 7);
							}
							else
							{
								num2 = Math.Round(num3 + ++num6 * d, 7);
							}
						}
						double[] valuesUnderViewPort = Axis.GetValuesUnderViewPort(list, this.ParentAxis);
						num2 = (decimal)valuesUnderViewPort[0];
						num6 = 0;
						if (this.ParentAxis.IsDateTimeAxis && num3 != num2)
						{
							decimal d3 = num3;
							while (d3 < num2)
							{
								if (this.ParentAxis.IsDateTimeAxis)
								{
									num5++;
									DateTime dateTime2 = DateTimeHelper.UpdateDate(this.Parent.FirstLabelDate, (double)(num5 * d), this.ParentAxis.InternalIntervalType);
									decimal d4 = (decimal)DateTimeHelper.DateDiff(dateTime2, this.Parent.FirstLabelDate, this.ParentAxis.MinDateRange, this.ParentAxis.MaxDateRange, this.ParentAxis.InternalIntervalType, this.ParentAxis.XValueType);
									d3 = Math.Round(num3 + d4, 7);
								}
							}
						}
						num3 = num2;
						num4 = (decimal)valuesUnderViewPort[1];
					}
					if (!this.ParentAxis.Logarithmic)
					{
						bool flag = true;
						this.ParentAxis.SkipOffset4DateTime = 0;
						while (num2 <= num4)
						{
							object actualLabelPosition = null;
							string text;
							if (this.AxisLabelContentDictionary.ContainsKey((double)num2))
							{
								text = ObservableObject.GetFormattedMultilineText(this.AxisLabelContentDictionary[(double)num2]);
								actualLabelPosition = num2;
							}
							else if (this.ParentAxis.XValueType == ChartValueTypes.Date)
							{
								DateTime dateTime3 = this.ParentAxis.MinDate;
								decimal d5 = num2;
								if (this.ParentAxis._isAllXValueZero)
								{
									d5 = --d5;
								}
								dateTime3 = DateTimeHelper.UpdateDate(this.Parent.FirstLabelDate, num * (double)num5, this.ParentAxis.InternalIntervalType);
								text = AxisLabels.FormatDate(dateTime3, this.ParentAxis);
								actualLabelPosition = dateTime3;
							}
							else if (this.ParentAxis.XValueType == ChartValueTypes.Time)
							{
								DateTime dateTime4 = this.ParentAxis.MinDate;
								decimal d6 = num2;
								if (this.ParentAxis._isAllXValueZero)
								{
									d6 = --d6;
								}
								dateTime4 = DateTimeHelper.UpdateDate(this.Parent.FirstLabelDate, num * (double)num5, this.ParentAxis.InternalIntervalType);
								text = AxisLabels.FormatDate(dateTime4, this.ParentAxis);
								actualLabelPosition = dateTime4;
							}
							else if (this.ParentAxis.XValueType == ChartValueTypes.DateTime)
							{
								DateTime dateTime5 = this.ParentAxis.MinDate;
								decimal d7 = num2;
								if (this.ParentAxis._isAllXValueZero)
								{
									d7 = --d7;
								}
								dateTime5 = DateTimeHelper.UpdateDate(this.Parent.FirstLabelDate, num * (double)num5, this.ParentAxis.InternalIntervalType);
								if (this.ParentAxis.IntervalType == IntervalTypes.Years || this.ParentAxis.InternalIntervalType == IntervalTypes.Years || this.ParentAxis.IntervalType == IntervalTypes.Months || this.ParentAxis.InternalIntervalType == IntervalTypes.Months || this.ParentAxis.IntervalType == IntervalTypes.Weeks || this.ParentAxis.InternalIntervalType == IntervalTypes.Weeks || this.ParentAxis.IntervalType == IntervalTypes.Days || this.ParentAxis.InternalIntervalType == IntervalTypes.Days)
								{
									dateTime5 = new DateTime(dateTime5.Year, dateTime5.Month, dateTime5.Day, 0, 0, 0, 0);
								}
								text = AxisLabels.FormatDate(dateTime5, this.ParentAxis);
								actualLabelPosition = dateTime5;
							}
							else if (!this.ParentAxis.DisplayAutoAxisLabels && this.ParentAxis.PlotDetails.IsAllPrimaryAxisXLabelsPresent)
							{
								text = "";
							}
							else
							{
								text = this.GetFormattedString((double)num2);
								actualLabelPosition = num2;
							}
							AxisLabel axisLabel = this.CreateLabel(text);
							axisLabel.ActualLabelPosition = actualLabelPosition;
							this.AxisLabelList.Add(axisLabel);
							this.LabelValues.Add((double)num2);
							if (this.ParentAxis.IsDateTimeAxis && flag)
							{
								if (this.ParentAxis.ScrollableLength != 0.0 && double.IsNaN(this.ParentAxis.Interval.Value) && double.IsNaN(this.Interval.Value))
								{
									this.ParentAxis.SkipOffset4DateTime = this.CalculateSkipOffset(this.InternalRows, this.InternalAngle.Value, this.ParentAxis.ScrollableLength);
								}
								flag = false;
							}
							if (this.ParentAxis.IsDateTimeAxis)
							{
								num6++;
								num5++;
								num6 += this.ParentAxis.SkipOffset4DateTime;
								num5 += this.ParentAxis.SkipOffset4DateTime;
								DateTime dateTime6 = DateTimeHelper.UpdateDate(this.Parent.FirstLabelDate, (double)(num6 * d), this.ParentAxis.InternalIntervalType);
								decimal d8 = (decimal)DateTimeHelper.DateDiff(dateTime6, this.Parent.FirstLabelDate, this.ParentAxis.MinDateRange, this.ParentAxis.MaxDateRange, this.ParentAxis.InternalIntervalType, this.ParentAxis.XValueType);
								num2 = Math.Round(num3 + d8, 7);
							}
							else
							{
								num2 = Math.Round(num3 + ++num6 * d, 7);
							}
						}
					}
					else
					{
						AxisLabel axisLabel2 = this.CreateLabel(this.GetFormattedString(Math.Pow(this.ParentAxis.LogarithmBase, (double)num3)));
						axisLabel2.ActualLabelPosition = num3;
						this.AxisLabelList.Add(axisLabel2);
						this.LabelValues.Add((double)num3);
						num2 = (decimal)Math.Pow(this.ParentAxis.LogarithmBase, (double)(num3 + ++num6 * d));
						while (num2 < (decimal)Math.Pow(this.ParentAxis.LogarithmBase, (double)num4))
						{
							axisLabel2 = this.CreateLabel(this.GetFormattedString((double)num2));
							axisLabel2.ActualLabelPosition = num2;
							this.AxisLabelList.Add(axisLabel2);
							this.LabelValues.Add(Math.Log((double)num2, this.ParentAxis.LogarithmBase));
							num2 = (decimal)Math.Pow(this.ParentAxis.LogarithmBase, (double)(num3 + ++num6 * d));
						}
						double num7 = (double)num2;
						if (num7 <= Math.Pow(this.ParentAxis.LogarithmBase, this.Maximum))
						{
							axisLabel2 = this.CreateLabel(this.GetFormattedString(num7));
							axisLabel2.ActualLabelPosition = num7;
							this.AxisLabelList.Add(axisLabel2);
							this.LabelValues.Add(Math.Log(num7, this.ParentAxis.LogarithmBase));
						}
					}
				}
				else if (this.ParentAxis.CircularPlotDetails != null && this.ParentAxis.CircularPlotDetails.CircularChartType == RenderAs.Radar)
				{
					int maxDataPointsCountFromInternalSeriesList = this.ParentAxis.PlotDetails.GetMaxDataPointsCountFromInternalSeriesList((this.ParentAxis.Chart as Chart).InternalSeries);
					this.ParentAxis.CircularPlotDetails.CalculateAxisXLabelsPoints4Radar(this.ParentAxis.Width, this.ParentAxis.Height, this.Enabled.Value, maxDataPointsCountFromInternalSeriesList);
					for (int i = 0; i < this.ParentAxis.CircularPlotDetails.ListOfPoints4CircularAxis.Count; i++)
					{
						string text2;
						if (this.AxisLabelContentDictionary.ContainsKey((double)i))
						{
							text2 = ObservableObject.GetFormattedMultilineText(this.AxisLabelContentDictionary[(double)i]);
						}
						else
						{
							text2 = this.GetFormattedString((double)(i + 1));
						}
						AxisLabel axisLabel3 = this.CreateLabel(text2);
						this.AxisLabelList.Add(axisLabel3);
						this.LabelValues.Add((double)num2);
						axisLabel3.ActualLabelPosition = num2;
					}
				}
				else
				{
					while (num2 < num4)
					{
						DateTime dateTime7 = DateTimeHelper.XValueToDateTime(this.Parent.MinDate, this.Minimum, this.ParentAxis.InternalIntervalType);
						string text3;
						object actualLabelPosition2;
						if (this.ParentAxis.XValueType == ChartValueTypes.Date || this.ParentAxis.XValueType == ChartValueTypes.Time)
						{
							DateTime dateTime8 = this.ParentAxis.MinDate;
							decimal d9 = num2;
							if (this.ParentAxis._isAllXValueZero)
							{
								d9 = --d9;
							}
							dateTime8 = DateTimeHelper.UpdateDate(dateTime7, num * (double)num5, this.ParentAxis.InternalIntervalType);
							text3 = AxisLabels.FormatDate(dateTime8, this.ParentAxis);
							actualLabelPosition2 = dateTime8;
						}
						else if (this.ParentAxis.XValueType == ChartValueTypes.DateTime)
						{
							DateTime dateTime9 = this.ParentAxis.MinDate;
							decimal d10 = num2;
							if (this.ParentAxis._isAllXValueZero)
							{
								d10 = --d10;
							}
							dateTime9 = DateTimeHelper.UpdateDate(dateTime7, num * (double)num5, this.ParentAxis.InternalIntervalType);
							string format = string.IsNullOrEmpty((string)this.ParentAxis.GetValue(Axis.ValueFormatStringProperty)) ? "M/d/yyyy" : this.ParentAxis.ValueFormatString;
							string text4 = this.ParentAxis.AddPrefixAndSuffix(dateTime9.ToString(format, CultureInfo.CurrentCulture));
							text3 = text4;
							actualLabelPosition2 = dateTime9;
						}
						else
						{
							text3 = this.GetFormattedString((double)num2);
							actualLabelPosition2 = num2;
						}
						AxisLabel axisLabel4 = this.CreateLabel(text3);
						axisLabel4.ActualLabelPosition = actualLabelPosition2;
						this.AxisLabelList.Add(axisLabel4);
						this.LabelValues.Add((double)num2);
						if (this.ParentAxis.IsDateTimeAxis)
						{
							num5++;
							DateTime dateTime10 = DateTimeHelper.UpdateDate(dateTime7, (double)(num5 * d), this.ParentAxis.InternalIntervalType);
							decimal d11 = (decimal)DateTimeHelper.DateDiff(dateTime10, dateTime7, this.ParentAxis.MinDateRange, this.ParentAxis.MaxDateRange, this.ParentAxis.InternalIntervalType, this.ParentAxis.XValueType);
							num2 = Math.Round(num3 + d11, 7);
						}
						else
						{
							num2 = Math.Round(num3 + ++num5 * d, 7);
						}
					}
					if (this.ParentAxis.CircularPlotDetails != null && this.LabelValues.Count > 0)
					{
						this.ParentAxis.CircularPlotDetails.CalculateAxisXLabelsPoints4Polar(this.ParentAxis.Width, this.ParentAxis.Height, this.Enabled.Value, this.LabelValues, (double)num3, (double)num4);
					}
				}
			}
			else
			{
				if (!(num3 != num4))
				{
					return false;
				}
				if (!this.ParentAxis.Logarithmic)
				{
					AxisLabel axisLabel5 = this.CreateLabel(this.GetFormattedString(this.Minimum));
					axisLabel5.ActualLabelPosition = this.Minimum;
					this.AxisLabelList.Add(axisLabel5);
					this.LabelValues.Add(this.Minimum);
					num4 = (decimal)(this.Maximum - num / 2.0);
					num2 = Math.Round(num3 + ++num5 * d, 7);
					while (num2 <= num4)
					{
						axisLabel5 = this.CreateLabel(this.GetFormattedString((double)num2));
						axisLabel5.ActualLabelPosition = num2;
						this.AxisLabelList.Add(axisLabel5);
						this.LabelValues.Add((double)num2);
						num2 = Math.Round(num3 + ++num5 * d, 7);
					}
					double num8 = (double)num2;
					if (num8 <= this.Maximum)
					{
						axisLabel5 = this.CreateLabel(this.GetFormattedString(num8));
						axisLabel5.ActualLabelPosition = num8;
						this.AxisLabelList.Add(axisLabel5);
						this.LabelValues.Add(num8);
					}
				}
				else
				{
					AxisLabel axisLabel5 = this.CreateLabel(this.GetFormattedString(Math.Pow(this.ParentAxis.LogarithmBase, this.Minimum)));
					axisLabel5.ActualLabelPosition = this.Minimum;
					this.AxisLabelList.Add(axisLabel5);
					this.LabelValues.Add(this.Minimum);
					num4 = (decimal)(this.Maximum - num / 2.0);
					num2 = (decimal)Math.Pow(this.ParentAxis.LogarithmBase, (double)Math.Round(num3 + ++num5 * d, 7));
					while (num2 < (decimal)Math.Pow(this.ParentAxis.LogarithmBase, (double)num4))
					{
						axisLabel5 = this.CreateLabel(this.GetFormattedString((double)num2));
						axisLabel5.ActualLabelPosition = num2;
						this.AxisLabelList.Add(axisLabel5);
						this.LabelValues.Add(Math.Log((double)num2, this.ParentAxis.LogarithmBase));
						num2 = (decimal)Math.Pow(this.ParentAxis.LogarithmBase, (double)Math.Round(num3 + ++num5 * d, 7));
					}
					double num9 = (double)num2;
					if (num9 <= Math.Pow(this.ParentAxis.LogarithmBase, this.Maximum))
					{
						axisLabel5 = this.CreateLabel(this.GetFormattedString(num9));
						axisLabel5.ActualLabelPosition = num9;
						this.AxisLabelList.Add(axisLabel5);
						this.LabelValues.Add(Math.Log(num9, this.ParentAxis.LogarithmBase));
					}
				}
			}
			return true;
		}

		private void CalculatAvgWidthOfAChar()
		{
			AxisLabel axisLabel = new AxisLabel();
			axisLabel.Text = "ABCDabcd01";
			this.ApplyAxisLabelFontProperties(axisLabel);
			axisLabel.CreateVisualObject(this, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
			this.WidthOfACharacter = axisLabel.ActualTextWidth / 10.0;
		}

		internal static string FormatDate(DateTime dt, Axis axis)
		{
			string text = (axis.XValueType == ChartValueTypes.Date) ? "d" : ((axis.XValueType == ChartValueTypes.Time) ? "t" : "G");
			text = (string.IsNullOrEmpty((string)axis.GetValue(Axis.ValueFormatStringProperty)) ? text : axis.ValueFormatString);
			return axis.AddPrefixAndSuffix(dt.ToString(text, CultureInfo.CurrentCulture));
		}

		private void UpdateLabelPosition()
		{
			switch (this.Placement)
			{
			case PlacementTypes.Top:
				this.UpdatePositionLabelsTop();
				return;
			case PlacementTypes.Left:
				this.UpdatePositionLabelsLeft();
				return;
			case PlacementTypes.Right:
				this.UpdatePositionLabelsRight();
				return;
			case PlacementTypes.Bottom:
				this.UpdatePositionLabelsBottom();
				return;
			default:
				return;
			}
		}

		private void SetLabelPosition()
		{
			switch (this.Placement)
			{
			case PlacementTypes.Top:
				this.PositionLabelsTop();
				return;
			case PlacementTypes.Left:
				this.PositionLabelsLeft();
				return;
			case PlacementTypes.Right:
				this.PositionLabelsRight();
				return;
			case PlacementTypes.Bottom:
				this.PositionLabelsBottom();
				return;
			case PlacementTypes.Circular:
				this.PositionLabelsCircular();
				return;
			default:
				return;
			}
		}

		private void UpdatePositionLabelsTop()
		{
			double positionMin = double.IsNaN(this.ParentAxis.StartOffset) ? 0.0 : this.ParentAxis.StartOffset;
			double num = double.IsNaN(this.ParentAxis.EndOffset) ? 0.0 : this.ParentAxis.EndOffset;
			if (double.IsNaN(this.Width) || this.Width <= 0.0)
			{
				foreach (AxisLabel current in this.AxisLabelList)
				{
					if (current.Visual != null)
					{
						current.Visual.Visibility = Visibility.Collapsed;
					}
				}
				return;
			}
			this.Visual.Width = this.Width;
			double num2 = 0.0;
			List<AxisLabel> list = new List<AxisLabel>();
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				axisLabel.IsVisible = true;
				list.Add(axisLabel);
				axisLabel.Position = new Point(0.0, 0.0);
				double num3;
				if (this.InternalMinHeight != 0.0 && this.InternalMinHeight > axisLabel.ActualHeight)
				{
					num3 = this.InternalMinHeight;
				}
				else
				{
					num3 = axisLabel.ActualHeight;
				}
				if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight < num3)
				{
					axisLabel.ActualHeight = this.InternalMaxHeight;
				}
				else
				{
					axisLabel.ActualHeight = num3;
				}
				num2 = Math.Max(num2, axisLabel.ActualHeight);
				if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight >= axisLabel.ActualHeight && this.InternalMaxHeight > this._maxRowHeight)
				{
					num2 = Math.Max(num2, this._maxRowHeight);
				}
			}
			for (int j = 0; j < this.AxisLabelList.Count; j++)
			{
				AxisLabel axisLabel2 = this.AxisLabelList[j];
				if (axisLabel2.Visual != null)
				{
					double x = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, this.LabelValues[j]);
					double num4 = 0.0;
					if (this.GetAngle() != 0.0)
					{
						num4 = Math.Abs(axisLabel2.ActualTextHeight / 2.0 * Math.Sin(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
					}
					axisLabel2.Position = new Point(x, num2 * (double)this.InternalRows - num4 - (double)(j % this.InternalRows) * this._maxRowHeight + base.Padding.Top);
					axisLabel2.SetPosition();
				}
			}
			foreach (AxisLabel current2 in this.AxisLabelList)
			{
				if (!list.Contains(current2))
				{
					current2.IsVisible = false;
				}
				if (!current2.IsVisible && current2.Visual != null)
				{
					current2.Visual.Visibility = Visibility.Collapsed;
				}
			}
			list.Clear();
			this.Visual.Height = num2 * (double)this.InternalRows + base.Padding.Top;
			this.CalculateHorizontalOverflow();
		}

		private void PositionLabelsTop()
		{
			double positionMin = double.IsNaN(this.ParentAxis.StartOffset) ? 0.0 : this.ParentAxis.StartOffset;
			double num = double.IsNaN(this.ParentAxis.EndOffset) ? 0.0 : this.ParentAxis.EndOffset;
			if (double.IsNaN(this.Width) || this.Width <= 0.0)
			{
				return;
			}
			this.Visual.Width = this.Width;
			double num2 = 0.0;
			this.CalculateHorizontalDefaults(this.Width, this.Height);
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				axisLabel.Position = new Point(0.0, 0.0);
				if (axisLabel.Visual == null)
				{
					this.ApplyAxisLabelFontProperties(axisLabel);
					axisLabel.CreateVisualObject(this, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				}
				axisLabel.IsVisible = true;
				double num3;
				if (this.InternalMinHeight != 0.0 && this.InternalMinHeight > axisLabel.ActualHeight)
				{
					num3 = this.InternalMinHeight;
				}
				else
				{
					num3 = axisLabel.ActualHeight;
				}
				if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight < num3)
				{
					axisLabel.ActualHeight = this.InternalMaxHeight;
				}
				else
				{
					axisLabel.ActualHeight = num3;
				}
				num2 = Math.Max(num2, axisLabel.ActualHeight);
				if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight >= axisLabel.ActualHeight && this.InternalMaxHeight > this._maxRowHeight)
				{
					num2 = Math.Max(num2, this._maxRowHeight);
				}
			}
			for (int j = 0; j < this.AxisLabelList.Count; j++)
			{
				AxisLabel axisLabel2 = this.AxisLabelList[j];
				double x = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, this.LabelValues[j]);
				double num4 = 0.0;
				if (this.GetAngle() != 0.0)
				{
					num4 = Math.Abs(axisLabel2.ActualTextHeight / 2.0 * Math.Sin(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
				}
				axisLabel2.Position = new Point(x, num2 * (double)this.InternalRows - num4 - (double)(j % this.InternalRows) * this._maxRowHeight + base.Padding.Top);
				axisLabel2.SetPosition();
				this.Visual.Children.Add(axisLabel2.Visual);
			}
			this.Visual.Height = num2 * (double)this.InternalRows + base.Padding.Top;
			this.CalculateHorizontalOverflow();
		}

		private void UpdatePositionLabelsLeft()
		{
			double positionMax = double.IsNaN(this.ParentAxis.StartOffset) ? 0.0 : this.ParentAxis.StartOffset;
			double num = double.IsNaN(this.ParentAxis.EndOffset) ? 0.0 : this.ParentAxis.EndOffset;
			if (double.IsNaN(this.Height) || this.Height <= 0.0)
			{
				foreach (AxisLabel current in this.AxisLabelList)
				{
					if (current.Visual != null)
					{
						current.Visual.Visibility = Visibility.Collapsed;
					}
				}
				return;
			}
			this.Visual.Height = this.Height;
			double num2 = 0.0;
			List<AxisLabel> list = new List<AxisLabel>();
			int num3;
			if (this.ParentAxis.IsDateTimeAxis && this.ParentAxis.SkipOffset4DateTime != 0)
			{
				num3 = 0;
			}
			else
			{
				num3 = this.ParentAxis.SkipOffset;
			}
			for (int i = 0; i < this.AxisLabelList.Count; i += num3 + 1)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				axisLabel.IsVisible = true;
				list.Add(axisLabel);
				axisLabel.Position = new Point(0.0, 0.0);
				double num4;
				if (this.InternalMinWidth != 0.0 && this.InternalMinWidth > axisLabel.ActualWidth)
				{
					num4 = this.InternalMinWidth;
				}
				else
				{
					num4 = axisLabel.ActualWidth;
				}
				if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth < num4)
				{
					axisLabel.ActualWidth = this.InternalMaxWidth;
				}
				else
				{
					axisLabel.ActualWidth = num4;
				}
				num2 = Math.Max(num2, axisLabel.ActualWidth);
			}
			for (int j = 0; j < this.AxisLabelList.Count; j += num3 + 1)
			{
				AxisLabel axisLabel2 = this.AxisLabelList[j];
				if (axisLabel2.Visual != null)
				{
					double y = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, this.LabelValues[j]);
					double num5 = 1.0;
					if (this.GetAngle() != 0.0)
					{
						num5 = Math.Abs(axisLabel2.ActualTextHeight / 2.0 * Math.Cos(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
					}
					axisLabel2.Position = new Point(num2 - num5 + base.Padding.Left, y);
					axisLabel2.SetPosition();
				}
			}
			foreach (AxisLabel current2 in this.AxisLabelList)
			{
				if (!list.Contains(current2))
				{
					current2.IsVisible = false;
				}
				if (!current2.IsVisible && current2.Visual != null)
				{
					current2.Visual.Visibility = Visibility.Collapsed;
				}
			}
			list.Clear();
			this.Visual.Width = num2 + base.Padding.Left;
			this.CalculateVerticalOverflow();
		}

		private void PositionLabelsLeft()
		{
			double positionMax = double.IsNaN(this.ParentAxis.StartOffset) ? 0.0 : this.ParentAxis.StartOffset;
			double num = double.IsNaN(this.ParentAxis.EndOffset) ? 0.0 : this.ParentAxis.EndOffset;
			if (double.IsNaN(this.Height) || this.Height <= 0.0)
			{
				return;
			}
			this.Visual.Height = this.Height;
			double num2 = 0.0;
			if (this.ParentAxis.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				this.CalculateHorizontalDefaults(this.Height, this.Width);
			}
			else
			{
				this.CalculateVerticalDefaults();
			}
			bool flag = false;
			if (this.ParentAxis.Grids.Count > 0 && this.ParentAxis.Grids[0].Enabled.Value)
			{
				flag = true;
			}
			if (this.ParentAxis.CircularPlotDetails != null && this.ParentAxis.CircularPlotDetails.CircularChartType == RenderAs.Polar && double.IsNaN(this.InternalAngle.Value))
			{
				double num3 = AxisLabel.GetRadians(this.ParentAxis.CircularPlotDetails.MinAngleInDegree);
				if (num3 > 3.1415926535897931)
				{
					num3 -= 4.71238898038469;
				}
				else
				{
					num3 += 1.5707963267948966;
				}
				if (num3 >= 0.78539816339744828 && num3 <= 3.1415926535897931)
				{
					this.InternalAngle = new double?(-90.0);
				}
				else if (num3 > 3.1415926535897931 && num3 <= 5.497787143782138)
				{
					this.InternalAngle = new double?(90.0);
				}
			}
			Brush fontColor;
			if (this.Placement != PlacementTypes.Circular)
			{
				fontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.ParentAxis.Background, this.FontColor, false);
			}
			else
			{
				fontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, null, this.FontColor, false);
			}
			FontStyle internalFontStyle = this.InternalFontStyle;
			FontWeight internalFontWeight = this.InternalFontWeight;
			FontFamily internalFontFamily = this.InternalFontFamily;
			double internalFontSize = this.InternalFontSize;
			TextAlignment textAlignment = this.TextAlignment;
			int num4;
			if (this.ParentAxis.IsDateTimeAxis && this.ParentAxis.SkipOffset4DateTime != 0)
			{
				num4 = 0;
			}
			else
			{
				num4 = this.ParentAxis.SkipOffset;
			}
			for (int i = 0; i < this.AxisLabelList.Count; i += num4 + 1)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				axisLabel.Position = new Point(0.0, 0.0);
				if (axisLabel.Visual == null)
				{
					axisLabel.Angle = this.GetAngle();
					axisLabel.FontSize = internalFontSize;
					axisLabel.FontFamily = internalFontFamily;
					axisLabel.FontColor = fontColor;
					axisLabel.FontStyle = internalFontStyle;
					axisLabel.FontWeight = internalFontWeight;
					axisLabel.TextAlignment = textAlignment;
					axisLabel.CreateVisualObject(this, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				}
				axisLabel.IsVisible = true;
				double num5;
				if (this.InternalMinWidth != 0.0 && this.InternalMinWidth > axisLabel.ActualWidth)
				{
					num5 = this.InternalMinWidth;
				}
				else
				{
					num5 = axisLabel.ActualWidth;
				}
				if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth < num5)
				{
					axisLabel.ActualWidth = this.InternalMaxWidth;
				}
				else
				{
					axisLabel.ActualWidth = num5;
				}
				num2 = Math.Max(num2, axisLabel.ActualWidth);
			}
			for (int j = 0; j < this.AxisLabelList.Count; j += num4 + 1)
			{
				AxisLabel axisLabel2 = this.AxisLabelList[j];
				double y = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, this.LabelValues[j]);
				double num6 = 1.0;
				if (this.GetAngle() != 0.0)
				{
					num6 = Math.Abs(axisLabel2.ActualTextHeight / 2.0 * Math.Cos(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
				}
				axisLabel2.Position = new Point(num2 - num6 + base.Padding.Left, y);
				if (this.ParentAxis.CircularPlotDetails != null && this.ParentAxis.CircularPlotDetails.ChartOrientation == ChartOrientationType.Circular && flag)
				{
					axisLabel2._padding4AxisYLabelInCircularChart = 3.5;
				}
				axisLabel2.SetPosition();
				this.Visual.Children.Add(axisLabel2.Visual);
			}
			this.Visual.Width = num2 + base.Padding.Left;
			this.CalculateVerticalOverflow();
		}

		private void UpdatePositionLabelsRight()
		{
			double positionMax = double.IsNaN(this.ParentAxis.StartOffset) ? 0.0 : this.ParentAxis.StartOffset;
			double num = double.IsNaN(this.ParentAxis.EndOffset) ? 0.0 : this.ParentAxis.EndOffset;
			if (double.IsNaN(this.Height) || this.Height <= 0.0)
			{
				foreach (AxisLabel current in this.AxisLabelList)
				{
					if (current.Visual != null)
					{
						current.Visual.Visibility = Visibility.Collapsed;
					}
				}
				return;
			}
			this.Visual.Height = this.Height;
			double num2 = 0.0;
			List<AxisLabel> list = new List<AxisLabel>();
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				if (axisLabel.Visual != null)
				{
					axisLabel.IsVisible = true;
					list.Add(axisLabel);
					double y = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, this.LabelValues[i]);
					double x = 1.0;
					if (this.GetAngle() != 0.0)
					{
						x = Math.Abs(axisLabel.ActualTextHeight / 2.0 * Math.Cos(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
					}
					axisLabel.Position = new Point(x, y);
					axisLabel.SetPosition();
					double num3;
					if (this.InternalMinWidth != 0.0 && this.InternalMinWidth > axisLabel.ActualWidth)
					{
						num3 = this.InternalMinWidth;
					}
					else
					{
						num3 = axisLabel.ActualWidth;
					}
					if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth < num3)
					{
						axisLabel.ActualWidth = this.InternalMaxWidth;
					}
					else
					{
						axisLabel.ActualWidth = num3;
					}
					num2 = Math.Max(num2, axisLabel.ActualWidth);
				}
			}
			foreach (AxisLabel current2 in this.AxisLabelList)
			{
				if (!list.Contains(current2))
				{
					current2.IsVisible = false;
				}
				if (!current2.IsVisible && current2.Visual != null)
				{
					current2.Visual.Visibility = Visibility.Collapsed;
				}
			}
			list.Clear();
			this.Visual.Width = num2 + base.Padding.Right;
			this.CalculateVerticalOverflow();
		}

		private void PositionLabelsRight()
		{
			double positionMax = double.IsNaN(this.ParentAxis.StartOffset) ? 0.0 : this.ParentAxis.StartOffset;
			double num = double.IsNaN(this.ParentAxis.EndOffset) ? 0.0 : this.ParentAxis.EndOffset;
			if (double.IsNaN(this.Height) || this.Height <= 0.0)
			{
				return;
			}
			this.Visual.Height = this.Height;
			double num2 = 0.0;
			this.CalculateVerticalDefaults();
			Brush fontColor;
			if (this.Placement != PlacementTypes.Circular)
			{
				fontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.ParentAxis.Background, this.FontColor, false);
			}
			else
			{
				fontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, null, this.FontColor, false);
			}
			FontStyle internalFontStyle = this.InternalFontStyle;
			FontWeight internalFontWeight = this.InternalFontWeight;
			FontFamily internalFontFamily = this.InternalFontFamily;
			double internalFontSize = this.InternalFontSize;
			TextAlignment textAlignment = this.TextAlignment;
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				double y = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, this.LabelValues[i]);
				if (axisLabel.Visual == null)
				{
					axisLabel.Angle = this.GetAngle();
					axisLabel.FontSize = internalFontSize;
					axisLabel.FontFamily = internalFontFamily;
					axisLabel.FontColor = fontColor;
					axisLabel.FontStyle = internalFontStyle;
					axisLabel.FontWeight = internalFontWeight;
					axisLabel.TextAlignment = textAlignment;
					axisLabel.CreateVisualObject(this, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				}
				axisLabel.IsVisible = true;
				double x = 1.0;
				if (this.GetAngle() != 0.0)
				{
					x = Math.Abs(axisLabel.ActualTextHeight / 2.0 * Math.Cos(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
				}
				axisLabel.Position = new Point(x, y);
				axisLabel.SetPosition();
				this.Visual.Children.Add(axisLabel.Visual);
				double num3;
				if (this.InternalMinWidth != 0.0 && this.InternalMinWidth > axisLabel.ActualWidth)
				{
					num3 = this.InternalMinWidth;
				}
				else
				{
					num3 = axisLabel.ActualWidth;
				}
				if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth < num3)
				{
					axisLabel.ActualWidth = this.InternalMaxWidth;
				}
				else
				{
					axisLabel.ActualWidth = num3;
				}
				num2 = Math.Max(num2, axisLabel.ActualWidth);
			}
			this.Visual.Width = num2 + base.Padding.Right;
			this.CalculateVerticalOverflow();
		}

		private void ApplyAxisLabelFontProperties(AxisLabel label)
		{
			label.FontSize = this.InternalFontSize;
			if (this.Placement != PlacementTypes.Circular)
			{
				label.FontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.ParentAxis.Background, this.FontColor, false);
			}
			else
			{
				label.FontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, null, this.FontColor, false);
			}
			label.FontFamily = this.InternalFontFamily;
			label.FontStyle = this.InternalFontStyle;
			label.FontWeight = this.InternalFontWeight;
			label.TextAlignment = this.TextAlignment;
			label.Angle = this.GetAngle();
		}

		private void CalculateHorizontalDefaults4CircularAxis(double width, double height)
		{
			base.IsNotificationEnable = false;
			width = (double.IsNaN(width) ? 0.0 : width);
			height = (double.IsNaN(height) ? 0.0 : height);
			double area = Math.Max(width, height);
			if (double.IsNaN(this.InternalFontSize) || this.InternalFontSize <= 0.0)
			{
				double internalFontSize = this.CalculateFontSize(area);
				this.InternalFontSize = internalFontSize;
			}
			this.ParentAxis.SkipOffset = 0;
			this._maxRowHeight = this.CalculateMaxHeightOfLabels();
			base.IsNotificationEnable = true;
		}

		private void PositionLabelsCircular()
		{
			this.Visual.Width = this.Width;
			this.Visual.Height = this.Height;
			this.CalculateHorizontalDefaults4CircularAxis(this.Width, this.Height);
			List<Point> listOfPoints4CircularAxis = this.ParentAxis.CircularPlotDetails.ListOfPoints4CircularAxis;
			double radius = this.ParentAxis.CircularPlotDetails.Radius;
			Point center = this.ParentAxis.CircularPlotDetails.Center;
			List<CircularAxisLabel> list = new List<CircularAxisLabel>();
			for (int i = 0; i < this.AxisLabelList.Count; i += this.ParentAxis.SkipOffset + 1)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				axisLabel.IsVisible = true;
				CircularAxisLabel circularAxisLabel = new CircularAxisLabel(listOfPoints4CircularAxis[i], center, radius, radius, axisLabel, (double)i);
				axisLabel._padding4AxisXLabelInCircularChart = 8.0;
				if (circularAxisLabel.Angle == 4.71238898038469)
				{
					axisLabel._padding4AxisXLabelInCircularChart = 10.0;
				}
				else if ((circularAxisLabel.Angle > 4.71238898038469 && circularAxisLabel.Angle < 5.9331853071795866) || (circularAxisLabel.Angle > 3.4915926535897932 && circularAxisLabel.Angle < 4.71238898038469))
				{
					axisLabel._padding4AxisYLabelInCircularChart = 4.0;
				}
				else if ((circularAxisLabel.Angle > 0.35 && circularAxisLabel.Angle < 1.5707963267948966) || (circularAxisLabel.Angle > 1.5707963267948966 && circularAxisLabel.Angle < 2.791592653589793))
				{
					axisLabel._padding4AxisYLabelInCircularChart = -4.0;
				}
				list.Add(circularAxisLabel);
			}
			List<CircularAxisLabel> list2;
			List<CircularAxisLabel> list3;
			Axis.GetLeftAndRightLabels(list, out list2, out list3);
			foreach (CircularAxisLabel current in list2)
			{
				current.AxisLabel.SetPosition4CircularLabel(current, true);
				Point nextPosition;
				current.AxisLabel.RearrangeLabelsAtLeft(current, new Size(this.Width, this.Height), out nextPosition);
				current.NextPosition = nextPosition;
				this.Visual.Children.Add(current.AxisLabel.Visual);
			}
			foreach (CircularAxisLabel current2 in list3)
			{
				current2.AxisLabel.SetPosition4CircularLabel(current2, false);
				Point nextPosition2;
				current2.AxisLabel.RearrangeLabelsAtRight(current2, new Size(this.Width, this.Height), out nextPosition2);
				current2.NextPosition = nextPosition2;
				this.Visual.Children.Add(current2.AxisLabel.Visual);
			}
			this.UpdatePosition4LabelAtMinimumDistance(list2, list3);
		}

		private void UpdatePosition4LabelAtMinimumDistance(List<CircularAxisLabel> labelsAtLeft, List<CircularAxisLabel> labelsAtRight)
		{
			List<CircularAxisLabel> list = new List<CircularAxisLabel>();
			list.AddRange(labelsAtRight);
			list.AddRange(labelsAtLeft);
			double num = 1.7976931348623157E+308;
			double val = 1.7976931348623157E+308;
			double val2 = 1.7976931348623157E+308;
			foreach (CircularAxisLabel current in list)
			{
				if (Math.Round(current.NextPosition.X) != Math.Round(current.Center.X))
				{
					val = (current.NextPosition.X - current.Center.X) / Math.Cos(current.Angle);
				}
				if (Math.Round(current.NextPosition.Y) != Math.Round(current.Center.Y))
				{
					val2 = (current.NextPosition.Y - current.Center.Y) / Math.Sin(current.Angle);
				}
				double val3 = Math.Min(val, val2);
				num = Math.Min(num, val3);
			}
			foreach (CircularAxisLabel current2 in labelsAtLeft)
			{
				current2.XRadius = num;
				current2.YRadius = num;
				double x = current2.Center.X + current2.XRadius * Math.Cos(current2.Angle);
				double y = current2.Center.Y + current2.YRadius * Math.Sin(current2.Angle);
				current2.Position = new Point(x, y);
				current2.AxisLabel.SetPosition4CircularLabel(current2, true);
			}
			foreach (CircularAxisLabel current3 in labelsAtRight)
			{
				current3.XRadius = num;
				current3.YRadius = num;
				double x2 = current3.Center.X + current3.XRadius * Math.Cos(current3.Angle);
				double y2 = current3.Center.Y + current3.YRadius * Math.Sin(current3.Angle);
				current3.Position = new Point(x2, y2);
				current3.AxisLabel.SetPosition4CircularLabel(current3, false);
			}
			this.ParentAxis.CircularPlotDetails.UpdateCircularPlotDetails(list, num);
		}

		private void UpdatePositionLabelsBottom()
		{
			double positionMin = double.IsNaN(this.ParentAxis.StartOffset) ? 0.0 : this.ParentAxis.StartOffset;
			double num = double.IsNaN(this.ParentAxis.EndOffset) ? 0.0 : this.ParentAxis.EndOffset;
			if (double.IsNaN(this.Width) || this.Width <= 0.0)
			{
				foreach (AxisLabel current in this.AxisLabelList)
				{
					if (current.Visual != null)
					{
						current.Visual.Visibility = Visibility.Collapsed;
					}
				}
				return;
			}
			this.Visual.Width = this.Width;
			double num2 = 0.0;
			if (this.ParentAxis.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal && this.ParentAxis.AxisOrientation == AxisOrientation.Horizontal)
			{
				this.InternalRows = this.Rows.Value;
				this.CalculateHorizontalDefaults(this.Width, this.Height);
			}
			List<AxisLabel> list = new List<AxisLabel>();
			int num3;
			if (this.ParentAxis.IsDateTimeAxis && this.ParentAxis.SkipOffset4DateTime != 0)
			{
				num3 = 0;
			}
			else
			{
				num3 = this.ParentAxis.SkipOffset;
			}
			for (int i = 0; i < this.AxisLabelList.Count; i += num3 + 1)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				if (axisLabel.Visual != null)
				{
					axisLabel.IsVisible = true;
					list.Add(axisLabel);
					double x = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, this.LabelValues[i]);
					double num4 = 0.0;
					if (this.GetAngle() != 0.0)
					{
						num4 = Math.Abs(axisLabel.ActualTextHeight / 2.0 * Math.Sin(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
					}
					double num5;
					if (this.InternalMinHeight != 0.0 && this.InternalMinHeight > axisLabel.ActualHeight)
					{
						num5 = this.InternalMinHeight;
					}
					else
					{
						num5 = axisLabel.ActualHeight;
					}
					if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight < num5)
					{
						axisLabel.ActualHeight = this.InternalMaxHeight;
					}
					else
					{
						axisLabel.ActualHeight = num5;
					}
					num2 = Math.Max(num2, axisLabel.ActualHeight);
					if (double.IsPositiveInfinity(this.InternalMaxHeight))
					{
						num2 = Math.Max(num2, this._maxRowHeight);
					}
					if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight >= axisLabel.ActualHeight && this.InternalMaxHeight > this._maxRowHeight)
					{
						num2 = Math.Max(num2, this._maxRowHeight);
					}
					axisLabel.Position = new Point(x, num4 + (double)(i % this.InternalRows) * this._maxRowHeight);
					axisLabel.SetPosition();
				}
			}
			foreach (AxisLabel current2 in this.AxisLabelList)
			{
				if (!list.Contains(current2))
				{
					current2.IsVisible = false;
				}
				if (!current2.IsVisible && current2.Visual != null)
				{
					current2.Visual.Visibility = Visibility.Collapsed;
				}
			}
			list.Clear();
			this.Visual.Height = num2 * (double)this.InternalRows + base.Padding.Bottom;
			this.CalculateHorizontalOverflow();
		}

		private void PositionLabelsBottom()
		{
			double positionMin = double.IsNaN(this.ParentAxis.StartOffset) ? 0.0 : this.ParentAxis.StartOffset;
			double num = double.IsNaN(this.ParentAxis.EndOffset) ? 0.0 : this.ParentAxis.EndOffset;
			if (double.IsNaN(this.Width) || this.Width <= 0.0)
			{
				return;
			}
			this.Visual.Width = this.Width;
			double num2 = 0.0;
			this.CalculateHorizontalDefaults(this.Width, this.Height);
			int num3;
			if (this.ParentAxis.IsDateTimeAxis && this.ParentAxis.SkipOffset4DateTime != 0)
			{
				num3 = 0;
			}
			else
			{
				num3 = this.ParentAxis.SkipOffset;
			}
			for (int i = 0; i < this.AxisLabelList.Count; i += num3 + 1)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				axisLabel.IsVisible = true;
				double x = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, this.LabelValues[i]);
				double num4 = 0.0;
				if (this.GetAngle() != 0.0)
				{
					num4 = Math.Abs(axisLabel.ActualTextHeight / 2.0 * Math.Sin(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
				}
				double num5;
				if (this.InternalMinHeight != 0.0 && this.InternalMinHeight > axisLabel.ActualHeight)
				{
					num5 = this.InternalMinHeight;
				}
				else
				{
					num5 = axisLabel.ActualHeight;
				}
				if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight < num5)
				{
					axisLabel.ActualHeight = this.InternalMaxHeight;
				}
				else
				{
					axisLabel.ActualHeight = num5;
				}
				num2 = Math.Max(num2, axisLabel.ActualHeight);
				if (double.IsPositiveInfinity(this.InternalMaxHeight))
				{
					num2 = Math.Max(num2, this._maxRowHeight);
				}
				if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight >= axisLabel.ActualHeight && this.InternalMaxHeight > this._maxRowHeight)
				{
					num2 = Math.Max(num2, this._maxRowHeight);
				}
				axisLabel.Position = new Point(x, num4 + (double)(i % this.InternalRows) * this._maxRowHeight);
				axisLabel.SetPosition();
				this.Visual.Children.Add(axisLabel.Visual);
			}
			this.Visual.Height = num2 * (double)this.InternalRows + base.Padding.Bottom;
			this.CalculateHorizontalOverflow();
		}

		private void CalculateHorizontalOverflow()
		{
			if (this.AxisLabelList.Count > 0)
			{
				this.LeftOverflow = (from axisLabel in this.AxisLabelList
				where axisLabel.IsVisible
				select axisLabel.ActualLeft).Min();
				this.RightOverflow = (from axisLabel in this.AxisLabelList
				where axisLabel.IsVisible
				select axisLabel.ActualLeft + axisLabel.ActualWidth).Max() - this.Width;
			}
			else
			{
				this.LeftOverflow = 0.0;
				this.RightOverflow = 0.0;
			}
			if (this.ParentAxis.Enabled.Value)
			{
				this.LeftOverflow = ((this.LeftOverflow > 0.0) ? 0.0 : Math.Abs(this.LeftOverflow));
			}
			else
			{
				this.LeftOverflow = 0.0;
			}
			this.RightOverflow = ((this.RightOverflow < 0.0) ? 0.0 : this.RightOverflow);
			this.TopOverflow = 0.0;
			this.BottomOverflow = 0.0;
			if ((base.Chart as Chart).IsScrollingActivated && this.Width > this.ParentAxis.Width)
			{
				this.LeftOverflow = 0.0;
				this.RightOverflow = 0.0;
			}
		}

		private void CalculateVerticalOverflow()
		{
			if (this.AxisLabelList.Count > 0)
			{
				this.TopOverflow = (from axisLabel in this.AxisLabelList
				where axisLabel.IsVisible
				select axisLabel.ActualTop).Min();
				this.BottomOverflow = (from axisLabel in this.AxisLabelList
				where axisLabel.IsVisible
				select axisLabel.ActualTop + axisLabel.ActualHeight).Max() - this.Height;
			}
			else
			{
				this.TopOverflow = 0.0;
				this.BottomOverflow = 0.0;
			}
			this.TopOverflow = ((this.TopOverflow > 0.0) ? 0.0 : Math.Abs(this.TopOverflow));
			this.BottomOverflow = ((this.BottomOverflow < 0.0) ? 0.0 : this.BottomOverflow);
			this.LeftOverflow = 0.0;
			this.RightOverflow = 0.0;
			if ((base.Chart as Chart).ScrollingEnabled.Value && this.Height > this.ParentAxis.Height)
			{
				this.TopOverflow = 0.0;
				this.BottomOverflow = 0.0;
			}
		}

		private double GetAngle()
		{
			if (!double.IsNaN(this.InternalAngle.Value))
			{
				return this.InternalAngle.Value;
			}
			return 0.0;
		}

		private double CalculateFontSize(double area)
		{
			if (double.IsNaN(this.InternalFontSize) || this.InternalFontSize <= 0.0)
			{
				return Graphics.DefaultFontSizes[1];
			}
			return this.InternalFontSize;
		}

		private double CalculateMaxHeightOfLabels()
		{
			return this.CalculateMaxHeightOfLabels(null);
		}

		private double CalculateMaxHeightOfLabels(List<double> labelWidths)
		{
			double num = 0.0;
			Brush fontColor;
			if (this.Placement != PlacementTypes.Circular)
			{
				fontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.ParentAxis.Background, this.FontColor, false);
			}
			else
			{
				fontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, null, this.FontColor, false);
			}
			FontStyle internalFontStyle = this.InternalFontStyle;
			FontWeight internalFontWeight = this.InternalFontWeight;
			FontFamily internalFontFamily = this.InternalFontFamily;
			double internalFontSize = this.InternalFontSize;
			TextAlignment textAlignment = this.TextAlignment;
			int num2;
			if (this.ParentAxis.IsDateTimeAxis && this.Placement != PlacementTypes.Circular && this.ParentAxis.SkipOffset4DateTime != 0)
			{
				num2 = 0;
			}
			else
			{
				num2 = this.ParentAxis.SkipOffset;
			}
			for (int i = 0; i < this.AxisLabelList.Count; i += num2 + 1)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				if (axisLabel.Visual == null)
				{
					axisLabel.Angle = this.GetAngle();
					axisLabel.FontSize = internalFontSize;
					axisLabel.FontFamily = internalFontFamily;
					axisLabel.FontColor = fontColor;
					axisLabel.FontStyle = internalFontStyle;
					axisLabel.FontWeight = internalFontWeight;
					axisLabel.TextAlignment = textAlignment;
					axisLabel.CreateVisualObject(this, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				}
				else
				{
					axisLabel.Angle = this.GetAngle();
				}
				axisLabel.IsVisible = false;
				if (this.ParentAxis.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal && this.ParentAxis.AxisOrientation == AxisOrientation.Horizontal)
				{
					axisLabel.ApplyProperties(axisLabel, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				}
				axisLabel.UpdateAxisLabel();
				num = Math.Max(num, axisLabel.ActualHeight);
				if (labelWidths != null)
				{
					labelWidths.Add(axisLabel.ActualWidth);
				}
			}
			return num;
		}

		private int CalculateRows(double widthOfAxis)
		{
			TextBlock textBlock = this.SetFontProperties(new TextBlock
			{
				FlowDirection = ((base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight)
			});
			double? arg_80_0;
			if (double.IsNaN(this.Interval.Value))
			{
				double? interval = this.Interval;
				if (interval.GetValueOrDefault() <= 0.0 && interval.HasValue)
				{
					arg_80_0 = this.Interval;
					goto IL_80;
				}
			}
			arg_80_0 = new double?(this.ParentAxis.InternalInterval);
			IL_80:
			double? num = arg_80_0;
			double num2 = num.Value;
			double num3 = Graphics.ValueToPixelPosition(0.0, widthOfAxis, this.Minimum, this.Maximum, num2 + this.Minimum);
			List<double> list = new List<double>();
			double num4 = 0.0;
			Size size = default(Size);
			Brush fontColor;
			if (this.Placement != PlacementTypes.Circular)
			{
				fontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.ParentAxis.Background, this.FontColor, false);
			}
			else
			{
				fontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, null, this.FontColor, false);
			}
			FontStyle internalFontStyle = this.InternalFontStyle;
			FontWeight internalFontWeight = this.InternalFontWeight;
			FontFamily internalFontFamily = this.InternalFontFamily;
			double internalFontSize = this.InternalFontSize;
			TextAlignment textAlignment = this.TextAlignment;
			foreach (AxisLabel current in this.AxisLabelList)
			{
				if (current.Visual == null)
				{
					bool flag = false;
					if (this.ParentAxis.IsDateTimeAxis)
					{
						DateTime dateTime;
						flag = DateTime.TryParse(current.Text, out dateTime);
					}
					if (!this.ParentAxis.IsDateTimeAxis || (this.ParentAxis.IsDateTimeAxis && !flag))
					{
						current.Angle = this.GetAngle();
						current.FontSize = internalFontSize;
						current.FontFamily = internalFontFamily;
						current.FontColor = fontColor;
						current.FontStyle = internalFontStyle;
						current.FontWeight = internalFontWeight;
						current.TextAlignment = textAlignment;
						current.CreateVisualObject(this, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
					}
				}
				current.IsVisible = false;
				textBlock.Text = " " + current.Text + " ";
				double radians = AxisLabel.GetRadians(this.GetAngle());
				size = Graphics.CalculateTextBlockSize(radians, textBlock);
				if (!double.IsNaN(this.InternalAngle.Value))
				{
					size = Graphics.CalculateTextBlockSize(radians, textBlock);
					size.Width = Math.Cos(AxisLabel.GetRadians(this.GetAngle())) * textBlock.DesiredSize.Height + textBlock.DesiredSize.Height / 2.0;
				}
				num4 = Math.Max(num4, size.Height);
				list.Add(size.Width);
			}
			this._maxRowHeight = num4;
			int i;
			for (i = 1; i <= 3; i++)
			{
				bool flag2 = false;
				for (int j = 0; j < list.Count - i; j++)
				{
					double num5 = list[j] / 2.0 + list[j + i] / 2.0;
					if (num5 > num3 * (double)i)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					break;
				}
			}
			return i;
		}

		private int CalculateNumberOfRows(double widthOfAxis)
		{
			if (this.InternalRows <= 0)
			{
				return this.CalculateRows(widthOfAxis);
			}
			return this.InternalRows;
		}

		private TextBlock SetFontProperties(TextBlock textBlock)
		{
			textBlock.FontSize = this.InternalFontSize;
			textBlock.FontFamily = this.InternalFontFamily;
			textBlock.FontStyle = this.InternalFontStyle;
			textBlock.FontWeight = this.InternalFontWeight;
			return textBlock;
		}

		private void CalculateSkipOffSet4DisabledAxisLabels(double widthOfAxis, double heightOfAxis)
		{
			base.IsNotificationEnable = false;
			widthOfAxis = (double.IsNaN(widthOfAxis) ? 0.0 : widthOfAxis);
			heightOfAxis = (double.IsNaN(heightOfAxis) ? 0.0 : heightOfAxis);
			double area = Math.Max(widthOfAxis, heightOfAxis);
			if (double.IsNaN(this.InternalFontSize) || this.InternalFontSize <= 0.0)
			{
				double internalFontSize = this.CalculateFontSize(area);
				this.InternalFontSize = internalFontSize;
			}
			if (((double.IsNaN(this.ParentAxis.Interval.Value) && double.IsNaN(this.Interval.Value)) || (this.ParentAxis.IntervalType == IntervalTypes.Auto && this.ParentAxis.IsDateTimeAxis)) && widthOfAxis != 0.0)
			{
				this.ParentAxis.SkipOffset = this.CalculateSkipOffSetUsingSingleAxisLabel(widthOfAxis);
			}
			base.IsNotificationEnable = true;
		}

		private void CalculateHorizontalDefaults(double widthOfAxis, double heightOfAxis)
		{
			base.IsNotificationEnable = false;
			widthOfAxis = (double.IsNaN(widthOfAxis) ? 0.0 : widthOfAxis);
			heightOfAxis = (double.IsNaN(heightOfAxis) ? 0.0 : heightOfAxis);
			double area = Math.Max(widthOfAxis, heightOfAxis);
			if (double.IsNaN(this.InternalFontSize) || this.InternalFontSize <= 0.0)
			{
				double internalFontSize = this.CalculateFontSize(area);
				this.InternalFontSize = internalFontSize;
			}
			if (this.InternalRows <= 0)
			{
				int num = 0;
				if (this.ParentAxis.PlotDetails.ChartOrientation == ChartOrientationType.Vertical || (this.ParentAxis.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal && this.ParentAxis.AxisOrientation == AxisOrientation.Horizontal))
				{
					num = this.CalculateNumberOfRows(widthOfAxis);
				}
				if (num > 2 && double.IsNaN(this.InternalAngle.Value))
				{
					this.InternalRows = 1;
					if (this.ParentAxis.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
					{
						this.InternalAngle = new double?(0.0);
					}
					else
					{
						this.InternalAngle = new double?((double)(((base.Chart as Chart).IsScrollingActivated && this.ParentAxis.XValueType != ChartValueTypes.Numeric) ? -90 : -45));
					}
					if ((double.IsNaN(this.ParentAxis.Interval.Value) && double.IsNaN(this.Interval.Value)) || (this.ParentAxis.IntervalType == IntervalTypes.Auto && this.ParentAxis.IsDateTimeAxis))
					{
						this.ParentAxis.SkipOffset = this.CalculateSkipOffset(this.InternalRows, this.InternalAngle.Value, widthOfAxis);
					}
					else
					{
						this.ParentAxis.SkipOffset = 0;
						if (this.ParentAxis.PlotDetails.ChartOrientation == ChartOrientationType.Vertical || (this.ParentAxis.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal && this.ParentAxis.AxisOrientation == AxisOrientation.Horizontal))
						{
							num = this.CalculateRows(widthOfAxis);
							this.InternalRows = num;
						}
					}
				}
				else if (num >= 2 && !double.IsNaN(this.InternalAngle.Value) && ((double.IsNaN(this.ParentAxis.Interval.Value) && double.IsNaN(this.Interval.Value)) || (this.ParentAxis.IntervalType == IntervalTypes.Auto && this.ParentAxis.IsDateTimeAxis)))
				{
					this.InternalRows = 1;
					this.ParentAxis.SkipOffset = this.CalculateSkipOffset(this.InternalRows, this.InternalAngle.Value, widthOfAxis);
				}
				else
				{
					this.InternalRows = num;
					this.ParentAxis.SkipOffset = 0;
					if (!this.ParentAxis.Interval.HasValue || double.IsNaN(this.ParentAxis.Interval.Value))
					{
						this.ParentAxis.SkipOffset = this.CalculateSkipOffset(this.InternalRows, this.InternalAngle.Value, widthOfAxis);
					}
				}
			}
			else
			{
				int num2 = this.CalculateNumberOfRows(widthOfAxis);
				if (num2 > 2 && double.IsNaN(this.InternalAngle.Value))
				{
					if (this.ParentAxis.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
					{
						this.InternalAngle = new double?(0.0);
					}
					else
					{
						this.InternalAngle = new double?((double)(((base.Chart as Chart).IsScrollingActivated && this.ParentAxis.XValueType != ChartValueTypes.Numeric) ? -90 : -45));
					}
				}
				if (!this.ParentAxis.Interval.HasValue || double.IsNaN(this.ParentAxis.Interval.Value))
				{
					this.ParentAxis.SkipOffset = this.CalculateSkipOffset(this.InternalRows, this.InternalAngle.Value, widthOfAxis);
				}
			}
			this._maxRowHeight = this.CalculateMaxHeightOfLabels();
			base.IsNotificationEnable = true;
		}

		private int CalculateSkipOffSetUsingSingleAxisLabel(double axisWidth)
		{
			int num = 0;
			bool flag = true;
			double? arg_5A_0;
			if (double.IsNaN(this.Interval.Value))
			{
				double? interval = this.Interval;
				if (interval.GetValueOrDefault() <= 0.0 && interval.HasValue)
				{
					arg_5A_0 = this.Interval;
					goto IL_5A;
				}
			}
			arg_5A_0 = new double?(this.ParentAxis.InternalInterval);
			IL_5A:
			double? num2 = arg_5A_0;
			double num3 = num2.Value;
			if (this.AxisLabelList.Count > 0)
			{
				AxisLabel axisLabel = this.AxisLabelList[0];
				if (axisLabel.Visual == null)
				{
					this.ApplyAxisLabelFontProperties(axisLabel);
					axisLabel.CreateVisualObject(null, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				}
				double num4;
				if (this.ParentAxis.IsDateTimeAxis)
				{
					num4 = (from lb in this.AxisLabelList
					select lb.ActualTextHeight).Max() + 4.0 + 10.0;
				}
				else
				{
					num4 = (from lb in this.AxisLabelList
					select lb.ActualTextHeight).Max() + 4.0;
				}
				double num5 = num3;
				while (flag)
				{
					double num6 = Graphics.ValueToPixelPosition(0.0, axisWidth, this.Minimum, this.Maximum, num3 + this.Minimum);
					double num7 = Graphics.ValueToPixelPosition(0.0, axisWidth, this.Minimum, this.Maximum, num3 + num5 + this.Minimum);
					if (Math.Abs(num6 - num7) >= num4)
					{
						flag = false;
					}
					else
					{
						num++;
						num5 += num3;
					}
				}
				return num;
			}
			return num;
		}

		private int CalculateSkipOffset(int noOfRows, double angle, double axisWidth)
		{
			int num = 0;
			bool flag = true;
			double? arg_5A_0;
			if (double.IsNaN(this.Interval.Value))
			{
				double? interval = this.Interval;
				if (interval.GetValueOrDefault() <= 0.0 && interval.HasValue)
				{
					arg_5A_0 = this.Interval;
					goto IL_5A;
				}
			}
			arg_5A_0 = new double?(this.ParentAxis.InternalInterval);
			IL_5A:
			double? num2 = arg_5A_0;
			double num3 = num2.Value;
			if (this.AxisLabelList.Count > 0)
			{
				double num4;
				if (this.ParentAxis.IsDateTimeAxis)
				{
					AxisLabel axisLabel = this.AxisLabelList[0];
					if (axisLabel.Visual == null)
					{
						this.ApplyAxisLabelFontProperties(axisLabel);
						axisLabel.CreateVisualObject(null, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
					}
					num4 = (from lb in this.AxisLabelList
					select lb.ActualTextHeight).Max() + 4.0 + 10.0;
				}
				else
				{
					num4 = (from lb in this.AxisLabelList
					select lb.ActualTextHeight).Max() + 4.0;
				}
				double num5 = num3;
				while (flag)
				{
					double num6 = Graphics.ValueToPixelPosition(0.0, axisWidth, this.Minimum, this.Maximum, num3 + this.Minimum);
					double num7 = Graphics.ValueToPixelPosition(0.0, axisWidth, this.Minimum, this.Maximum, num3 + num5 + this.Minimum);
					if (Math.Abs(num6 - num7) >= num4)
					{
						flag = false;
					}
					else
					{
						num++;
						num5 += num3;
					}
				}
				return num;
			}
			return num;
		}

		internal string GetFormattedString(double value)
		{
			if (this.ParentAxis == null)
			{
				return value.ToString();
			}
			return this.ParentAxis.GetFormattedString(value);
		}

		private void CalculateVerticalDefaults()
		{
			double val = double.IsNaN(this.Width) ? 0.0 : this.Width;
			double val2 = double.IsNaN(this.Height) ? 0.0 : this.Height;
			double area = Math.Max(val, val2);
			if (double.IsNaN(this.InternalFontSize) || this.InternalFontSize <= 0.0)
			{
				this.InternalFontSize = this.CalculateFontSize(area);
			}
		}

		internal override void UpdateVisual(VcProperties propertyName, object value)
		{
			if (this.Visual != null)
			{
				using (List<AxisLabel>.Enumerator enumerator = this.AxisLabelList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AxisLabel current = enumerator.Current;
						this.ApplyAxisLabelFontProperties(current);
						current.ApplyProperties(current, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
					}
					return;
				}
			}
			base.FirePropertyChanged(propertyName);
		}

		internal void UpdateVisualObject()
		{
			if (!this.ParentAxis.Enabled.Value)
			{
				return;
			}
			if (this.ParentAxis.AxisOrientation != AxisOrientation.Circular && !this.Enabled.Value)
			{
				this.Visual = null;
				return;
			}
			this.UpdateLabelPosition();
		}

		private void ResetOverFlows()
		{
			if (this.ParentAxis.AxisRepresentation == AxisRepresentations.AxisX)
			{
				this.LeftOverflow = 0.0;
				this.RightOverflow = 0.0;
				return;
			}
			this.TopOverflow = 0.0;
			this.BottomOverflow = 0.0;
		}

		internal void CreateVisualObject()
		{
			if (!this.ParentAxis.Enabled.Value && (base.Chart as Chart).PlotDetails.ChartOrientation != ChartOrientationType.Circular)
			{
				this.ResetOverFlows();
				return;
			}
			this.Visual = new Canvas();
			this.AxisLabelList = new List<AxisLabel>();
			this.LabelValues = new List<double>();
			if (this.ParentAxis.AxisOrientation != AxisOrientation.Circular && !this.Enabled.Value)
			{
				if (this.ParentAxis.AxisRepresentation == AxisRepresentations.AxisX)
				{
					this.CreateLabels();
					if (this.ParentAxis.AxisOrientation == AxisOrientation.Horizontal)
					{
						this.CalculateSkipOffSet4DisabledAxisLabels(this.Width, this.Height);
					}
					else
					{
						this.CalculateSkipOffSet4DisabledAxisLabels(this.Height, this.Width);
					}
				}
				this.ResetOverFlows();
				this.Visual = null;
				this.AxisLabelList.Clear();
				this.LabelValues.Clear();
				return;
			}
			if (this.ParentAxis.AxisOrientation != AxisOrientation.Circular)
			{
				if (this.InternalFontSize == this._savedFontSize)
				{
					double? internalAngle = this.InternalAngle;
					if (internalAngle.GetValueOrDefault() == 0.0 && internalAngle.HasValue && this.InternalRows == 0)
					{
						goto IL_136;
					}
				}
				this._isRedraw = false;
				IL_136:
				if (this._isRedraw)
				{
					this.InternalFontSize = this._savedFontSize;
				}
				else
				{
					this._savedFontSize = this.InternalFontSize;
					this._isRedraw = true;
				}
				this.InternalAngle = new double?(double.NaN);
				this.InternalAngle = this.Angle;
				this.InternalRows = ((!this.Rows.HasValue) ? 0 : this.Rows.Value);
			}
			else
			{
				if (this.InternalFontSize == this._savedFontSize)
				{
					double? internalAngle2 = this.InternalAngle;
					if (internalAngle2.GetValueOrDefault() == 0.0 && internalAngle2.HasValue)
					{
						goto IL_1EB;
					}
				}
				this._isRedraw = false;
				IL_1EB:
				this.InternalRows = 1;
				if (this._isRedraw)
				{
					this.InternalFontSize = this._savedFontSize;
				}
				else
				{
					this._savedFontSize = this.InternalFontSize;
					this._isRedraw = true;
				}
				this.InternalAngle = new double?(double.NaN);
				this.InternalAngle = this.Angle;
			}
			this.CreateLabels();
			this.SetLabelPosition();
			this.Visual.Opacity = this.InternalOpacity;
			if (this.ParentAxis.AxisOrientation == AxisOrientation.Circular && !this.Enabled.Value)
			{
				this.Visual.Visibility = Visibility.Collapsed;
			}
			this.OldChartOrientation = (base.Chart as Chart).PlotDetails.ChartOrientation;
		}
	}
}
