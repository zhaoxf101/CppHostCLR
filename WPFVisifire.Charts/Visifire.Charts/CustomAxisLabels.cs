using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class CustomAxisLabels : ObservableObject
	{
		internal const string RootElementName = "RootElement";

		private const double CONSTANT_GAP_BETWEEN_AXISLABELS = 4.0;

		private const double LINE_HEIGHT = 5.0;

		private const double LINE_AND_LABEL_GAP = 5.0;

		private const double LEFT_TOP_PADDING = 3.0;

		public static readonly DependencyProperty EnabledProperty;

		public static readonly DependencyProperty LineEnabledProperty;

		public static readonly DependencyProperty LineThicknessProperty;

		public static readonly DependencyProperty LineColorProperty;

		public static readonly DependencyProperty TextAlignmentProperty;

		public static readonly DependencyProperty FontColorProperty;

		public static readonly DependencyProperty AngleProperty;

		public new static readonly DependencyProperty FontStyleProperty;

		public new static readonly DependencyProperty FontWeightProperty;

		public new static readonly DependencyProperty FontSizeProperty;

		public static readonly DependencyProperty TextWrapProperty;

		private new static readonly DependencyProperty ToolTipTextProperty;

		internal Canvas _rootElement;

		private double _maxRowHeight;

		private double _savedFontSize;

		private double _savedAngle;

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
				throw new NotImplementedException("ToolTipText property for CustomAxisLabels is not implemented");
			}
			set
			{
				throw new NotImplementedException("ToolTipText property for CustomAxisLabels is not implemented");
			}
		}

		public double TextWrap
		{
			get
			{
				return (double)base.GetValue(CustomAxisLabels.TextWrapProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabels.TextWrapProperty, value);
			}
		}

		public new double Opacity
		{
			get
			{
				return (double)base.GetValue(UIElement.OpacityProperty);
			}
			set
			{
				base.SetValue(UIElement.OpacityProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Enabled
		{
			get
			{
				if (!((bool?)base.GetValue(CustomAxisLabels.EnabledProperty)).HasValue)
				{
					return new bool?(true);
				}
				return (bool?)base.GetValue(CustomAxisLabels.EnabledProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabels.EnabledProperty, value);
			}
		}

		public bool LineEnabled
		{
			get
			{
				return (bool)base.GetValue(CustomAxisLabels.LineEnabledProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabels.LineEnabledProperty, value);
			}
		}

		public double LineThickness
		{
			get
			{
				return (double)base.GetValue(CustomAxisLabels.LineThicknessProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabels.LineThicknessProperty, value);
			}
		}

		public Brush LineColor
		{
			get
			{
				if (base.GetValue(CustomAxisLabels.LineColorProperty) == null)
				{
					return this.FontColor;
				}
				return (Brush)base.GetValue(CustomAxisLabels.LineColorProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabels.LineColorProperty, value);
			}
		}

		public TextAlignment TextAlignment
		{
			get
			{
				return (TextAlignment)base.GetValue(CustomAxisLabels.TextAlignmentProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabels.TextAlignmentProperty, value);
			}
		}

		public new FontFamily FontFamily
		{
			get
			{
				if ((FontFamily)base.GetValue(Control.FontFamilyProperty) == null)
				{
					return new FontFamily("Arial");
				}
				return (FontFamily)base.GetValue(Control.FontFamilyProperty);
			}
			set
			{
				base.SetValue(Control.FontFamilyProperty, value);
			}
		}

		public Brush FontColor
		{
			get
			{
				return (Brush)base.GetValue(CustomAxisLabels.FontColorProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabels.FontColorProperty, value);
			}
		}

		[TypeConverter(typeof(FontStyleConverter))]
		public new FontStyle FontStyle
		{
			get
			{
				return (FontStyle)base.GetValue(CustomAxisLabels.FontStyleProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabels.FontStyleProperty, value);
			}
		}

		[TypeConverter(typeof(FontWeightConverter))]
		public new FontWeight FontWeight
		{
			get
			{
				return (FontWeight)base.GetValue(CustomAxisLabels.FontWeightProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabels.FontWeightProperty, value);
			}
		}

		public new double FontSize
		{
			get
			{
				return (double)base.GetValue(CustomAxisLabels.FontSizeProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabels.FontSizeProperty, value);
			}
		}

		public double? Angle
		{
			get
			{
				if (!((double?)base.GetValue(CustomAxisLabels.AngleProperty)).HasValue)
				{
					return this.InternalAngle;
				}
				return (double?)base.GetValue(CustomAxisLabels.AngleProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabels.AngleProperty, value);
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

		public CustomAxisLabelCollection Labels
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

		internal double InternalMaxHeight
		{
			get
			{
				return (double)(double.IsNaN(this._internalMaxHeight) ? base.GetValue(FrameworkElement.MaxHeightProperty) : this._internalMaxHeight);
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
				return (double)(double.IsNaN(this._internalMaxWidth) ? base.GetValue(FrameworkElement.MaxWidthProperty) : this._internalMaxWidth);
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
				return (double)(double.IsNaN(this._internalMinHeight) ? base.GetValue(FrameworkElement.MinHeightProperty) : this._internalMinHeight);
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
				return (double)(double.IsNaN(this._internalMinWidth) ? base.GetValue(FrameworkElement.MinWidthProperty) : this._internalMinWidth);
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
					fontFamily = (FontFamily)base.GetValue(Control.FontFamilyProperty);
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
				return (double)(double.IsNaN(this._internalFontSize) ? base.GetValue(CustomAxisLabels.FontSizeProperty) : this._internalFontSize);
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
				return (FontStyle)((!this._internalFontStyle.HasValue) ? base.GetValue(CustomAxisLabels.FontStyleProperty) : this._internalFontStyle);
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
				return (FontWeight)((!this._internalFontWeight.HasValue) ? base.GetValue(CustomAxisLabels.FontWeightProperty) : this._internalFontWeight);
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
				return (double)(double.IsNaN(this._internalOpacity) ? base.GetValue(UIElement.OpacityProperty) : this._internalOpacity);
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

		internal Axis ParentAxis
		{
			get;
			set;
		}

		internal List<AxisLabel> AxisLabelList
		{
			get;
			set;
		}

		internal List<CustomAxisLabel> CustomAxisLabelList
		{
			get;
			set;
		}

		internal List<double> LabelValues
		{
			get;
			set;
		}

		public CustomAxisLabels()
		{
			this.WidthOfACharacter = double.NaN;
			this.InternalAngle = new double?(double.NaN);
			this.Labels = new CustomAxisLabelCollection();
			this.Labels.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Labels_CollectionChanged);
		}

		static CustomAxisLabels()
		{
			CustomAxisLabels.EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool?), typeof(CustomAxisLabels), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabels.OnEnabledPropertyChanged)));
			CustomAxisLabels.LineEnabledProperty = DependencyProperty.Register("LineEnabled", typeof(bool), typeof(CustomAxisLabels), new PropertyMetadata(true, new PropertyChangedCallback(CustomAxisLabels.OnLineEnabledPropertyChanged)));
			CustomAxisLabels.LineThicknessProperty = DependencyProperty.Register("LineThickness", typeof(double), typeof(CustomAxisLabels), new PropertyMetadata(0.25, new PropertyChangedCallback(CustomAxisLabels.OnLineThicknessPropertyChanged)));
			CustomAxisLabels.LineColorProperty = DependencyProperty.Register("LineColor", typeof(Brush), typeof(CustomAxisLabels), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabels.OnLineColorPropertyChanged)));
			CustomAxisLabels.TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(CustomAxisLabels), new PropertyMetadata(TextAlignment.Left, new PropertyChangedCallback(CustomAxisLabels.OnTextAlignmentPropertyChanged)));
			CustomAxisLabels.FontColorProperty = DependencyProperty.Register("FontColor", typeof(Brush), typeof(CustomAxisLabels), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabels.OnFontColorPropertyChanged)));
			CustomAxisLabels.AngleProperty = DependencyProperty.Register("Angle", typeof(double?), typeof(CustomAxisLabels), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabels.OnAnglePropertyChanged)));
			CustomAxisLabels.FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(CustomAxisLabels), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabels.OnFontStylePropertyChanged)));
			CustomAxisLabels.FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(CustomAxisLabels), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabels.OnFontWeightPropertyChanged)));
			CustomAxisLabels.FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(CustomAxisLabels), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabels.OnFontSizePropertyChanged)));
			CustomAxisLabels.TextWrapProperty = DependencyProperty.Register("TextWrap", typeof(double), typeof(CustomAxisLabels), new PropertyMetadata(double.NaN, new PropertyChangedCallback(CustomAxisLabels.OnTextWrapPropertyChanged)));
			CustomAxisLabels.ToolTipTextProperty = DependencyProperty.Register("ToolTipText", typeof(string), typeof(CustomAxisLabels), null);
			if (!CustomAxisLabels._defaultStyleKeyApplied)
			{
				FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomAxisLabels), new FrameworkPropertyMetadata(typeof(CustomAxisLabels)));
				CustomAxisLabels._defaultStyleKeyApplied = true;
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this._rootElement = (base.GetTemplateChild("RootElement") as Canvas);
			foreach (CustomAxisLabel current in this.Labels)
			{
				this.AddCustomAxisLabelToRootElement(current);
			}
		}

		private void AddCustomAxisLabelToRootElement(CustomAxisLabel label)
		{
			if (this._rootElement == null)
			{
				return;
			}
			label.IsNotificationEnable = false;
			if (base.IsInDesignMode)
			{
				ObservableObject.RemoveElementFromElementTree(label);
			}
			if (label.ParentTree == null)
			{
				this._rootElement.Children.Add(label);
			}
			label.IsNotificationEnable = true;
			label.IsTabStop = false;
		}

		internal override void Bind()
		{
		}

		private static void OnLineColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.UpdateVisual(VcProperties.LineColor, e.NewValue);
		}

		private static void OnLineThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.UpdateVisual(VcProperties.LineThickness, e.NewValue);
		}

		private static void OnFontColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.UpdateVisual(VcProperties.FontColor, e.NewValue);
		}

		private static void OnEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.FirePropertyChanged(VcProperties.Enabled);
		}

		private static void OnLineEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.FirePropertyChanged(VcProperties.LineEnabled);
		}

		private static void OnTextAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.FirePropertyChanged(VcProperties.TextAlignment);
		}

		private static void OnFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			if (e.NewValue == null || e.OldValue == null)
			{
				customAxisLabels.InternalFontFamily = (FontFamily)e.NewValue;
				customAxisLabels.FirePropertyChanged(VcProperties.FontFamily);
				return;
			}
			if (e.NewValue.ToString() != e.OldValue.ToString())
			{
				customAxisLabels.InternalFontFamily = (FontFamily)e.NewValue;
				customAxisLabels.FirePropertyChanged(VcProperties.FontFamily);
			}
		}

		private static void OnFontStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.InternalFontStyle = (FontStyle)e.NewValue;
			customAxisLabels.UpdateVisual(VcProperties.FontStyle, e.NewValue);
		}

		private static void OnFontWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.InternalFontWeight = (FontWeight)e.NewValue;
			customAxisLabels.UpdateVisual(VcProperties.FontWeight, e.NewValue);
		}

		private static void OnFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.InternalFontSize = (double)e.NewValue;
			customAxisLabels.FirePropertyChanged(VcProperties.FontSize);
		}

		private static void OnOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.InternalOpacity = (double)e.NewValue;
			customAxisLabels.FirePropertyChanged(VcProperties.Opacity);
		}

		private static void OnMaxHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.InternalMaxHeight = (double)e.NewValue;
			customAxisLabels.FirePropertyChanged(VcProperties.MaxHeight);
		}

		private static void OnMinHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.InternalMinHeight = (double)e.NewValue;
			customAxisLabels.FirePropertyChanged(VcProperties.MinHeight);
		}

		private static void OnMaxWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.InternalMaxWidth = (double)e.NewValue;
			customAxisLabels.FirePropertyChanged(VcProperties.MaxWidth);
		}

		private static void OnMinWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.InternalMinWidth = (double)e.NewValue;
			customAxisLabels.FirePropertyChanged(VcProperties.MinWidth);
		}

		private static void OnTextWrapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			if ((double)e.NewValue < 0.0 || (double)e.NewValue > 1.0)
			{
				throw new Exception("Wrong property value. Range of TextWrapProperty varies from 0 to 1.");
			}
			customAxisLabels.FirePropertyChanged(VcProperties.TextWrap);
		}

		private static void OnAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabels customAxisLabels = d as CustomAxisLabels;
			customAxisLabels.InternalAngle = (double?)e.NewValue;
			customAxisLabels.FirePropertyChanged(VcProperties.Angle);
		}

		private void Labels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems == null)
				{
					goto IL_15A;
				}

                foreach (CustomAxisLabel customAxisLabel in e.NewItems)
                {
                    customAxisLabel.Parent = this;
                    if (base.Chart != null)
                    {
                        customAxisLabel.Chart = base.Chart;
                    }
                    if (string.IsNullOrEmpty((string)customAxisLabel.GetValue(FrameworkElement.NameProperty)))
                    {
                        customAxisLabel.Name = "CustomAxisLabel" + (this.Labels.Count - 1).ToString() + "_" + Guid.NewGuid().ToString().Replace('-', '_');
                    }
                    customAxisLabel.PropertyChanged -= new PropertyChangedEventHandler(this.CustomLabel_PropertyChanged);
                    customAxisLabel.PropertyChanged += new PropertyChangedEventHandler(this.CustomLabel_PropertyChanged);
                    this.AddCustomAxisLabelToRootElement(customAxisLabel);
                }

                goto IL_15A;
                
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (CustomAxisLabel customAxisLabel2 in e.OldItems)
				{
					customAxisLabel2.Chart = null;
					if (this._rootElement != null && customAxisLabel2.ParentTree != null)
					{
						ObservableObject.RemoveElementFromElementTree(customAxisLabel2);
					}
				}
			}
			IL_15A:
			base.FirePropertyChanged(VcProperties.CustomAxisLabel);
		}

		private void CustomLabel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.FirePropertyChanged((VcProperties)Enum.Parse(typeof(VcProperties), e.PropertyName, true));
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
			return result;
		}

		private bool CreateLabels()
		{
			if (!double.IsNaN(this.TextWrap))
			{
				this.CalculatAvgWidthOfAChar();
			}
			foreach (CustomAxisLabel current in this.Labels)
			{
				string formattedMultilineText = ObservableObject.GetFormattedMultilineText(current.Text);
				AxisLabel axisLabel = this.CreateLabel(formattedMultilineText);
				current.Chart = base.Chart;
				double num3;
				if (this.ParentAxis.IsDateTimeAxis)
				{
					DateTime dateTime = Convert.ToDateTime(current.From);
					DateTime dateTime2 = Convert.ToDateTime(current.To);
					if (this.ParentAxis.XValueType == ChartValueTypes.Auto || this.ParentAxis.XValueType == ChartValueTypes.Date)
					{
						dateTime = new DateTime(dateTime.Date.Year, dateTime.Date.Month, dateTime.Date.Day);
						dateTime2 = new DateTime(dateTime2.Date.Year, dateTime2.Date.Month, dateTime2.Date.Day);
					}
					else if (this.ParentAxis.XValueType == ChartValueTypes.Time)
					{
						dateTime = DateTime.Parse("12/30/1899 " + dateTime.TimeOfDay.ToString(), CultureInfo.InvariantCulture);
						dateTime2 = DateTime.Parse("12/30/1899 " + dateTime2.TimeOfDay.ToString(), CultureInfo.InvariantCulture);
					}
					double num = DateTimeHelper.DateDiff(dateTime, this.Parent.MinDate, this.ParentAxis.MinDateRange, this.ParentAxis.MaxDateRange, this.ParentAxis.InternalIntervalType, this.ParentAxis.XValueType);
					double num2 = DateTimeHelper.DateDiff(dateTime2, this.Parent.MinDate, this.ParentAxis.MinDateRange, this.ParentAxis.MaxDateRange, this.ParentAxis.InternalIntervalType, this.ParentAxis.XValueType);
					num3 = (num + num2) / 2.0;
					axisLabel.From = num;
					axisLabel.To = num2;
				}
				else if (this.ParentAxis.Logarithmic)
				{
					num3 = (Math.Log(Convert.ToDouble(current.From), this.ParentAxis.LogarithmBase) + Math.Log(Convert.ToDouble(current.To), this.ParentAxis.LogarithmBase)) / 2.0;
					axisLabel.From = Math.Log(Convert.ToDouble(current.From), this.ParentAxis.LogarithmBase);
					axisLabel.To = Math.Log(Convert.ToDouble(current.To), this.ParentAxis.LogarithmBase);
				}
				else
				{
					num3 = (Convert.ToDouble(current.From) + Convert.ToDouble(current.To)) / 2.0;
					axisLabel.From = Convert.ToDouble(current.From);
					axisLabel.To = Convert.ToDouble(current.To);
				}
				if (axisLabel.From < axisLabel.To)
				{
					if (axisLabel.To > this.Parent.InternalAxisMaximum)
					{
						axisLabel.To = this.Parent.InternalAxisMaximum;
					}
					if (axisLabel.From < this.Parent.InternalAxisMinimum)
					{
						axisLabel.From = this.Parent.InternalAxisMinimum;
					}
					num3 = (axisLabel.From + axisLabel.To) / 2.0;
				}
				else
				{
					if (axisLabel.From > this.Parent.InternalAxisMaximum)
					{
						axisLabel.From = this.Parent.InternalAxisMaximum;
					}
					if (axisLabel.To < this.Parent.InternalAxisMinimum)
					{
						axisLabel.To = this.Parent.InternalAxisMinimum;
					}
					num3 = (axisLabel.From + axisLabel.To) / 2.0;
				}
				this.AxisLabelList.Add(axisLabel);
				this.CustomAxisLabelList.Add(current);
				this.LabelValues.Add(num3);
			}
			return true;
		}

		private void CalculatAvgWidthOfAChar()
		{
			AxisLabel axisLabel = new AxisLabel();
			axisLabel.Text = "ABCDabcd01";
			this.ApplyAxisLabelFontProperties(axisLabel, null);
			axisLabel.CreateVisualObject(null, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
			this.WidthOfACharacter = axisLabel.ActualTextWidth / 10.0;
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
			default:
				return;
			}
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
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				CustomAxisLabel customAxisLabel = this.CustomAxisLabelList[i];
				axisLabel.Position = new Point(0.0, 0.0);
				customAxisLabel.TextElement = axisLabel.Visual;
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
			num2 += (this.LineEnabled ? 10.0 : 5.0) + 3.0;
			for (int j = 0; j < this.AxisLabelList.Count; j++)
			{
				AxisLabel axisLabel2 = this.AxisLabelList[j];
				if (axisLabel2.Visual != null)
				{
					CustomAxisLabel customLabel = this.CustomAxisLabelList[j];
					double y = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, this.LabelValues[j]);
					double num4 = 1.0;
					if (this.GetAngle() != 0.0)
					{
						num4 = Math.Abs(axisLabel2.ActualTextHeight / 2.0 * Math.Cos(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
					}
					double num5 = num2 - num4 + base.Padding.Left - 3.0;
					axisLabel2.Position = new Point(num5 - (this.LineEnabled ? 10.0 : 5.0), y);
					axisLabel2.SetPosition();
					double y2 = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, axisLabel2.From);
					double y3 = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, axisLabel2.To);
					if (this.LineEnabled)
					{
						this.UpdatePath(new Point(num5, y2), new Point(num5, y3), this.Placement, customLabel);
					}
				}
			}
			this.Visual.Width = num2 + base.Padding.Left;
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
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				CustomAxisLabel customAxisLabel = this.CustomAxisLabelList[i];
				this.ApplyAxisLabelFontProperties(axisLabel, customAxisLabel);
				axisLabel.Position = new Point(0.0, 0.0);
				axisLabel.CreateVisualObject(customAxisLabel, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				axisLabel.IsVisible = true;
				customAxisLabel.TextElement = axisLabel.Visual;
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
			num2 += (this.LineEnabled ? 10.0 : 5.0) + 3.0;
			for (int j = 0; j < this.AxisLabelList.Count; j++)
			{
				AxisLabel axisLabel2 = this.AxisLabelList[j];
				CustomAxisLabel customAxisLabel2 = this.CustomAxisLabelList[j];
				double y = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, this.LabelValues[j]);
				double num4 = 1.0;
				if (this.GetAngle() != 0.0)
				{
					num4 = Math.Abs(axisLabel2.ActualTextHeight / 2.0 * Math.Cos(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
				}
				double num5 = num2 - num4 + base.Padding.Left - 3.0;
				axisLabel2.Position = new Point(num5 - (this.LineEnabled ? 10.0 : 5.0), y);
				axisLabel2.SetPosition();
				double y2 = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, axisLabel2.From);
				double y3 = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, axisLabel2.To);
				if (this.LineEnabled)
				{
					Path path = this.GetPath(new Point(num5, y2), new Point(num5, y3), this.Placement, customAxisLabel2);
					customAxisLabel2.CustomLabelPath = path;
					this.Visual.Children.Add(path);
				}
				this.Visual.Children.Add(axisLabel2.Visual);
			}
			this.Visual.Width = num2 + base.Padding.Left;
			this.CalculateVerticalOverflow();
		}

		private void ApplyAxisLabelFontProperties(AxisLabel label, CustomAxisLabel customAxisLabel)
		{
			label.FontSize = this.InternalFontSize;
			if (customAxisLabel != null)
			{
				label.FontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.ParentAxis.Background, customAxisLabel.FontColor, false);
			}
			label.FontFamily = this.InternalFontFamily;
			label.FontStyle = this.InternalFontStyle;
			label.FontWeight = this.InternalFontWeight;
			label.TextAlignment = this.TextAlignment;
			label.Angle = this.GetAngle();
		}

		private void UpdatePath(Point startPoint, Point endPoint, PlacementTypes placementType, CustomAxisLabel customLabel)
		{
			if (customLabel.CustomLabelPath != null)
			{
				customLabel.CustomLabelPath.Data = null;
			}
			PathGeometry pathGeometry = new PathGeometry();
			PathFigure pathFigure = new PathFigure();
			pathFigure.StartPoint = startPoint;
			PointCollection pointCollection = new PointCollection();
			if (placementType == PlacementTypes.Bottom)
			{
				pointCollection.Add(new Point(startPoint.X, startPoint.Y + (this.LineEnabled ? 5.0 : 0.0)));
				pointCollection.Add(new Point(endPoint.X, endPoint.Y + (this.LineEnabled ? 5.0 : 0.0)));
				pointCollection.Add(endPoint);
			}
			else if (placementType == PlacementTypes.Left)
			{
				pointCollection.Add(new Point(startPoint.X - (this.LineEnabled ? 5.0 : 0.0), startPoint.Y));
				pointCollection.Add(new Point(endPoint.X - (this.LineEnabled ? 5.0 : 0.0), endPoint.Y));
				pointCollection.Add(endPoint);
			}
			else if (placementType == PlacementTypes.Right)
			{
				pointCollection.Add(new Point(startPoint.X + (this.LineEnabled ? 5.0 : 0.0), startPoint.Y));
				pointCollection.Add(new Point(endPoint.X + (this.LineEnabled ? 5.0 : 0.0), endPoint.Y));
				pointCollection.Add(endPoint);
			}
			else if (placementType == PlacementTypes.Top)
			{
				pointCollection.Add(new Point(startPoint.X, startPoint.Y - (this.LineEnabled ? 5.0 : 0.0)));
				pointCollection.Add(new Point(endPoint.X, endPoint.Y - (this.LineEnabled ? 5.0 : 0.0)));
				pointCollection.Add(endPoint);
			}
			foreach (Point current in pointCollection)
			{
				LineSegment lineSegment = new LineSegment();
				lineSegment.Point = current;
				pathFigure.Segments.Add(lineSegment);
			}
			pathGeometry.Figures.Add(pathFigure);
			customLabel.CustomLabelPath.Data = pathGeometry;
		}

		private Path GetPath(Point startPoint, Point endPoint, PlacementTypes placementType, CustomAxisLabel customLabel)
		{
			Path path = new Path();
			path.Stroke = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.ParentAxis.Background, customLabel.LineColor, false);
			path.StrokeThickness = customLabel.LineThickness.Value;
			PathGeometry pathGeometry = new PathGeometry();
			PathFigure pathFigure = new PathFigure();
			pathFigure.StartPoint = startPoint;
			PointCollection pointCollection = new PointCollection();
			if (placementType == PlacementTypes.Bottom)
			{
				pointCollection.Add(new Point(startPoint.X, startPoint.Y + (this.LineEnabled ? 5.0 : 0.0)));
				pointCollection.Add(new Point(endPoint.X, endPoint.Y + (this.LineEnabled ? 5.0 : 0.0)));
				pointCollection.Add(endPoint);
			}
			else if (placementType == PlacementTypes.Left)
			{
				pointCollection.Add(new Point(startPoint.X - (this.LineEnabled ? 5.0 : 0.0), startPoint.Y));
				pointCollection.Add(new Point(endPoint.X - (this.LineEnabled ? 5.0 : 0.0), endPoint.Y));
				pointCollection.Add(endPoint);
			}
			else if (placementType == PlacementTypes.Right)
			{
				pointCollection.Add(new Point(startPoint.X + (this.LineEnabled ? 5.0 : 0.0), startPoint.Y));
				pointCollection.Add(new Point(endPoint.X + (this.LineEnabled ? 5.0 : 0.0), endPoint.Y));
				pointCollection.Add(endPoint);
			}
			else if (placementType == PlacementTypes.Top)
			{
				pointCollection.Add(new Point(startPoint.X, startPoint.Y - (this.LineEnabled ? 5.0 : 0.0)));
				pointCollection.Add(new Point(endPoint.X, endPoint.Y - (this.LineEnabled ? 5.0 : 0.0)));
				pointCollection.Add(endPoint);
			}
			foreach (Point current in pointCollection)
			{
				LineSegment lineSegment = new LineSegment();
				lineSegment.Point = current;
				pathFigure.Segments.Add(lineSegment);
			}
			pathGeometry.Figures.Add(pathFigure);
			path.Data = pathGeometry;
			return path;
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
			double num3 = 1.0;
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				CustomAxisLabel arg_FF_0 = this.CustomAxisLabelList[i];
				axisLabel.Position = new Point(0.0, 0.0);
				double num4;
				if (this.InternalMinHeight != 0.0 && this.InternalMinHeight > axisLabel.ActualHeight)
				{
					num4 = this.InternalMinHeight;
				}
				else
				{
					num4 = axisLabel.ActualHeight;
				}
				if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight < num4)
				{
					axisLabel.ActualHeight = this.InternalMaxHeight;
				}
				else
				{
					axisLabel.ActualHeight = num4;
				}
				num2 = Math.Max(num2, axisLabel.ActualHeight);
				if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight >= axisLabel.ActualHeight && this.InternalMaxHeight > this._maxRowHeight)
				{
					num2 = Math.Max(num2, this._maxRowHeight);
				}
			}
			num2 += (this.LineEnabled ? 10.0 : 5.0) + 3.0;
			for (int j = 0; j < this.AxisLabelList.Count; j++)
			{
				AxisLabel axisLabel2 = this.AxisLabelList[j];
				if (axisLabel2.Visual != null)
				{
					CustomAxisLabel customLabel = this.CustomAxisLabelList[j];
					double x = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, this.LabelValues[j]);
					double num5 = 0.0;
					if (this.GetAngle() != 0.0)
					{
						num5 = Math.Abs(axisLabel2.ActualTextHeight / 2.0 * Math.Sin(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
					}
					double num6 = num2 * (double)((int)num3) - num5 - (double)(j % (int)num3) * this._maxRowHeight + base.Padding.Top - 3.0;
					axisLabel2.Position = new Point(x, num6 - (this.LineEnabled ? 10.0 : 5.0));
					axisLabel2.SetPosition();
					double x2 = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, axisLabel2.From);
					double x3 = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, axisLabel2.To);
					if (this.LineEnabled)
					{
						this.UpdatePath(new Point(x2, num6), new Point(x3, num6), this.Placement, customLabel);
					}
				}
			}
			this.Visual.Height = num2 * (double)((int)num3) + base.Padding.Top;
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
			double num3 = 1.0;
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				CustomAxisLabel customAxisLabel = this.CustomAxisLabelList[i];
				this.ApplyAxisLabelFontProperties(axisLabel, customAxisLabel);
				axisLabel.Position = new Point(0.0, 0.0);
				axisLabel.CreateVisualObject(customAxisLabel, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				axisLabel.IsVisible = true;
				double num4;
				if (this.InternalMinHeight != 0.0 && this.InternalMinHeight > axisLabel.ActualHeight)
				{
					num4 = this.InternalMinHeight;
				}
				else
				{
					num4 = axisLabel.ActualHeight;
				}
				if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight < num4)
				{
					axisLabel.ActualHeight = this.InternalMaxHeight;
				}
				else
				{
					axisLabel.ActualHeight = num4;
				}
				num2 = Math.Max(num2, axisLabel.ActualHeight);
				if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight >= axisLabel.ActualHeight && this.InternalMaxHeight > this._maxRowHeight)
				{
					num2 = Math.Max(num2, this._maxRowHeight);
				}
			}
			num2 += (this.LineEnabled ? 10.0 : 5.0) + 3.0;
			for (int j = 0; j < this.AxisLabelList.Count; j++)
			{
				AxisLabel axisLabel2 = this.AxisLabelList[j];
				CustomAxisLabel customAxisLabel2 = this.CustomAxisLabelList[j];
				double x = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, this.LabelValues[j]);
				double num5 = 0.0;
				if (this.GetAngle() != 0.0)
				{
					num5 = Math.Abs(axisLabel2.ActualTextHeight / 2.0 * Math.Sin(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
				}
				double num6 = num2 * (double)((int)num3) - num5 - (double)(j % (int)num3) * this._maxRowHeight + base.Padding.Top - 3.0;
				axisLabel2.Position = new Point(x, num6 - (this.LineEnabled ? 10.0 : 5.0));
				axisLabel2.SetPosition();
				double x2 = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, axisLabel2.From);
				double x3 = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, axisLabel2.To);
				if (this.LineEnabled)
				{
					Path path = this.GetPath(new Point(x2, num6), new Point(x3, num6), this.Placement, customAxisLabel2);
					customAxisLabel2.CustomLabelPath = path;
					this.Visual.Children.Add(path);
				}
				this.Visual.Children.Add(axisLabel2.Visual);
			}
			this.Visual.Height = num2 * (double)((int)num3) + base.Padding.Top;
			this.CalculateHorizontalOverflow();
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
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				if (axisLabel.Visual != null)
				{
					CustomAxisLabel customAxisLabel = this.CustomAxisLabelList[i];
					double y = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, this.LabelValues[i]);
					customAxisLabel.TextElement = axisLabel.Visual;
					double num3 = 1.0;
					if (this.GetAngle() != 0.0)
					{
						num3 = Math.Abs(axisLabel.ActualTextHeight / 2.0 * Math.Cos(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
					}
					double num4 = num3 + base.Padding.Left + 3.0;
					axisLabel.Position = new Point(num4 + (this.LineEnabled ? 10.0 : 5.0), y);
					axisLabel.SetPosition();
					double y2 = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, axisLabel.From);
					double y3 = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, axisLabel.To);
					if (this.LineEnabled)
					{
						this.UpdatePath(new Point(num4, y2), new Point(num4, y3), this.Placement, customAxisLabel);
					}
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
			}
			this.Visual.Width = num2 + base.Padding.Right + (this.LineEnabled ? 10.0 : 5.0) + 3.0;
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
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				CustomAxisLabel customAxisLabel = this.CustomAxisLabelList[i];
				this.ApplyAxisLabelFontProperties(axisLabel, customAxisLabel);
				double y = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, this.LabelValues[i]);
				axisLabel.CreateVisualObject(customAxisLabel, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				axisLabel.IsVisible = true;
				customAxisLabel.TextElement = axisLabel.Visual;
				double num3 = 1.0;
				if (this.GetAngle() != 0.0)
				{
					num3 = Math.Abs(axisLabel.ActualTextHeight / 2.0 * Math.Cos(1.5707963267948966 - AxisLabel.GetRadians(this.GetAngle())));
				}
				double num4 = num3 + base.Padding.Left + 3.0;
				axisLabel.Position = new Point(num4 + (this.LineEnabled ? 10.0 : 5.0), y);
				axisLabel.SetPosition();
				double y2 = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, axisLabel.From);
				double y3 = Graphics.ValueToPixelPosition(this.Height - num, positionMax, this.Minimum, this.Maximum, axisLabel.To);
				if (this.LineEnabled)
				{
					Path path = this.GetPath(new Point(num4, y2), new Point(num4, y3), this.Placement, customAxisLabel);
					customAxisLabel.CustomLabelPath = path;
					this.Visual.Children.Add(path);
				}
				this.Visual.Children.Add(axisLabel.Visual);
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
			this.Visual.Width = num2 + base.Padding.Right + (this.LineEnabled ? 10.0 : 5.0) + 3.0;
			this.CalculateVerticalOverflow();
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
			double num3 = 1.0;
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				if (axisLabel.Visual != null)
				{
					CustomAxisLabel customLabel = this.CustomAxisLabelList[i];
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
					double num6 = num4 + (double)i % num3 * this._maxRowHeight + 3.0;
					axisLabel.Position = new Point(x, num6 + (this.LineEnabled ? 10.0 : 5.0));
					axisLabel.SetPosition();
					double x2 = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, axisLabel.From);
					double x3 = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, axisLabel.To);
					if (this.LineEnabled)
					{
						this.UpdatePath(new Point(x2, num6), new Point(x3, num6), this.Placement, customLabel);
					}
				}
			}
			this.Visual.Height = num2 + base.Padding.Bottom + (this.LineEnabled ? 10.0 : 5.0) + 3.0;
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
			double num3 = 1.0;
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				CustomAxisLabel customAxisLabel = this.CustomAxisLabelList[i];
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
				double num6 = num4 + (double)i % num3 * this._maxRowHeight + 3.0;
				axisLabel.Position = new Point(x, num6 + (this.LineEnabled ? 10.0 : 5.0));
				axisLabel.SetPosition();
				double x2 = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, axisLabel.From);
				double x3 = Graphics.ValueToPixelPosition(positionMin, this.Width - num, this.Minimum, this.Maximum, axisLabel.To);
				if (this.LineEnabled)
				{
					Path path = this.GetPath(new Point(x2, num6), new Point(x3, num6), this.Placement, customAxisLabel);
					customAxisLabel.CustomLabelPath = path;
					this.Visual.Children.Add(path);
				}
				this.Visual.Children.Add(axisLabel.Visual);
			}
			this.Visual.Height = num2 + base.Padding.Bottom + (this.LineEnabled ? 10.0 : 5.0) + 3.0;
			this.CalculateHorizontalOverflow();
		}

		private void CalculateHorizontalOverflow()
		{
			if (this.AxisLabelList.Count > 0)
			{
				this.LeftOverflow = (from axisLabel in this.AxisLabelList
				select axisLabel.ActualLeft).Min();
				this.RightOverflow = (from axisLabel in this.AxisLabelList
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
			if ((base.Chart as Chart).ScrollingEnabled.Value && this.Width > this.ParentAxis.Width)
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
				select axisLabel.ActualTop).Min();
				this.BottomOverflow = (from axisLabel in this.AxisLabelList
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
			for (int i = 0; i < this.AxisLabelList.Count; i++)
			{
				AxisLabel axisLabel = this.AxisLabelList[i];
				CustomAxisLabel customAxisLabel = this.CustomAxisLabelList[i];
				this.ApplyAxisLabelFontProperties(axisLabel, customAxisLabel);
				axisLabel.CreateVisualObject(customAxisLabel, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				axisLabel.IsVisible = true;
				customAxisLabel.TextElement = axisLabel.Visual;
				num = Math.Max(num, axisLabel.ActualHeight);
				if (labelWidths != null)
				{
					labelWidths.Add(axisLabel.ActualWidth);
				}
			}
			return num;
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
			this._maxRowHeight = this.CalculateMaxHeightOfLabels();
			base.IsNotificationEnable = true;
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
				for (int i = 0; i < this.AxisLabelList.Count; i++)
				{
					AxisLabel axisLabel = this.AxisLabelList[i];
					CustomAxisLabel customAxisLabel = this.CustomAxisLabelList[i];
					this.ApplyAxisLabelFontProperties(axisLabel, customAxisLabel);
					axisLabel.ApplyProperties(axisLabel, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				}
				return;
			}
			base.FirePropertyChanged(propertyName);
		}

		internal void UpdateVisualObject()
		{
			if (!this.Enabled.Value)
			{
				this.Visual = null;
				return;
			}
			this.UpdateLabelPosition();
		}

		internal void CreateVisualObject()
		{
			this.Visual = new Canvas();
			if (!this.Enabled.Value)
			{
				this.Visual = null;
				return;
			}
			this.AxisLabelList = new List<AxisLabel>();
			this.CustomAxisLabelList = new List<CustomAxisLabel>();
			this.LabelValues = new List<double>();
			if (this.InternalFontSize == this._savedFontSize)
			{
				double? internalAngle = this.InternalAngle;
				double savedAngle = this._savedAngle;
				if (internalAngle.GetValueOrDefault() == savedAngle && internalAngle.HasValue)
				{
					goto IL_81;
				}
			}
			this._isRedraw = false;
			IL_81:
			if (this._isRedraw)
			{
				this.InternalAngle = new double?(this._savedAngle);
				this.InternalFontSize = this._savedFontSize;
			}
			else
			{
				this._savedAngle = this.InternalAngle.Value;
				this._savedFontSize = this.InternalFontSize;
				this._isRedraw = true;
			}
			this.CreateLabels();
			this.SetLabelPosition();
			this.Visual.Opacity = this.InternalOpacity;
		}
	}
}
