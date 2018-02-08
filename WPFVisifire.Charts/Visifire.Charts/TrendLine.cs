using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class TrendLine : ObservableObject
	{
		internal const double VERTICALPADDING4LABEL = 6.0;

		internal const double HORIZONTALPADDING4LABEL = 10.0;

		public static readonly DependencyProperty EnabledProperty;

		public new static readonly DependencyProperty OpacityProperty;

		public static readonly DependencyProperty LabelTextProperty;

		public static readonly DependencyProperty LabelFontFamilyProperty;

		public static readonly DependencyProperty LabelFontSizeProperty;

		public static readonly DependencyProperty LabelFontColorProperty;

		public static readonly DependencyProperty LabelFontWeightProperty;

		public static readonly DependencyProperty LabelFontStyleProperty;

		public static readonly DependencyProperty LineColorProperty;

		public static readonly DependencyProperty LineThicknessProperty;

		public static readonly DependencyProperty LineStyleProperty;

		public static readonly DependencyProperty ShadowEnabledProperty;

		public static readonly DependencyProperty ValueProperty;

		public static readonly DependencyProperty StartValueProperty;

		public static readonly DependencyProperty EndValueProperty;

		public static readonly DependencyProperty HrefTargetProperty;

		public static readonly DependencyProperty HrefProperty;

		public static readonly DependencyProperty AxisTypeProperty;

		public static readonly DependencyProperty OrientationProperty;

		private double _internalNumericValue;

		private double _internalStartNumericValue;

		private double _internalEndNumericValue;

		private static bool _defaultStyleKeyApplied;

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Enabled
		{
			get
			{
				if (!((bool?)base.GetValue(TrendLine.EnabledProperty)).HasValue)
				{
					return new bool?(true);
				}
				return (bool?)base.GetValue(TrendLine.EnabledProperty);
			}
			set
			{
				base.SetValue(TrendLine.EnabledProperty, value);
			}
		}

		public new double Opacity
		{
			get
			{
				return (double)base.GetValue(TrendLine.OpacityProperty);
			}
			set
			{
				base.SetValue(TrendLine.OpacityProperty, value);
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

		public string LabelText
		{
			get
			{
				return (string)base.GetValue(TrendLine.LabelTextProperty);
			}
			set
			{
				base.SetValue(TrendLine.LabelTextProperty, value);
			}
		}

		public FontFamily LabelFontFamily
		{
			get
			{
				return (FontFamily)base.GetValue(TrendLine.LabelFontFamilyProperty);
			}
			set
			{
				base.SetValue(TrendLine.LabelFontFamilyProperty, value);
			}
		}

		public double LabelFontSize
		{
			get
			{
				return (double)base.GetValue(TrendLine.LabelFontSizeProperty);
			}
			set
			{
				base.SetValue(TrendLine.LabelFontSizeProperty, value);
			}
		}

		public Brush LabelFontColor
		{
			get
			{
				return (Brush)base.GetValue(TrendLine.LabelFontColorProperty);
			}
			set
			{
				base.SetValue(TrendLine.LabelFontColorProperty, value);
			}
		}

		public FontWeight LabelFontWeight
		{
			get
			{
				return (FontWeight)base.GetValue(TrendLine.LabelFontWeightProperty);
			}
			set
			{
				base.SetValue(TrendLine.LabelFontWeightProperty, value);
			}
		}

		public FontStyle LabelFontStyle
		{
			get
			{
				return (FontStyle)base.GetValue(TrendLine.LabelFontStyleProperty);
			}
			set
			{
				base.SetValue(TrendLine.LabelFontStyleProperty, value);
			}
		}

		public Brush LineColor
		{
			get
			{
				if ((Brush)base.GetValue(TrendLine.LineColorProperty) != null)
				{
					return (Brush)base.GetValue(TrendLine.LineColorProperty);
				}
				if (this.StartValue != null && this.EndValue != null)
				{
					return new SolidColorBrush(Color.FromArgb(255, 255, 228, 196));
				}
				return new SolidColorBrush(Colors.Red);
			}
			set
			{
				base.SetValue(TrendLine.LineColorProperty, value);
			}
		}

		public double LineThickness
		{
			get
			{
				if ((double)base.GetValue(TrendLine.LineThicknessProperty) != 0.0)
				{
					return (double)base.GetValue(TrendLine.LineThicknessProperty);
				}
				return 2.0;
			}
			set
			{
				base.SetValue(TrendLine.LineThicknessProperty, value);
			}
		}

		public LineStyles LineStyle
		{
			get
			{
				return (LineStyles)base.GetValue(TrendLine.LineStyleProperty);
			}
			set
			{
				base.SetValue(TrendLine.LineStyleProperty, value);
			}
		}

		public bool ShadowEnabled
		{
			get
			{
				return (bool)base.GetValue(TrendLine.ShadowEnabledProperty);
			}
			set
			{
				base.SetValue(TrendLine.ShadowEnabledProperty, value);
			}
		}

		public object Value
		{
			get
			{
				return base.GetValue(TrendLine.ValueProperty);
			}
			set
			{
				base.SetValue(TrendLine.ValueProperty, value);
			}
		}

		public object StartValue
		{
			get
			{
				return base.GetValue(TrendLine.StartValueProperty);
			}
			set
			{
				base.SetValue(TrendLine.StartValueProperty, value);
			}
		}

		public object EndValue
		{
			get
			{
				return base.GetValue(TrendLine.EndValueProperty);
			}
			set
			{
				base.SetValue(TrendLine.EndValueProperty, value);
			}
		}

		public AxisTypes AxisType
		{
			get
			{
				return (AxisTypes)base.GetValue(TrendLine.AxisTypeProperty);
			}
			set
			{
				base.SetValue(TrendLine.AxisTypeProperty, value);
			}
		}

		public Orientation Orientation
		{
			get
			{
				return (Orientation)base.GetValue(TrendLine.OrientationProperty);
			}
			set
			{
				base.SetValue(TrendLine.OrientationProperty, value);
			}
		}

		public HrefTargets HrefTarget
		{
			get
			{
				return (HrefTargets)base.GetValue(TrendLine.HrefTargetProperty);
			}
			set
			{
				base.SetValue(TrendLine.HrefTargetProperty, value);
			}
		}

		public string Href
		{
			get
			{
				return (string)base.GetValue(TrendLine.HrefProperty);
			}
			set
			{
				base.SetValue(TrendLine.HrefProperty, value);
			}
		}

		internal TextBlock LabelTextBlock
		{
			get;
			set;
		}

		internal Canvas LabelVisual
		{
			get;
			set;
		}

		internal DateTime InternalDateValue
		{
			get;
			set;
		}

		internal DateTime InternalDateStartValue
		{
			get;
			set;
		}

		internal DateTime InternalDateEndValue
		{
			get;
			set;
		}

		internal double InternalNumericValue
		{
			get
			{
				if (this.ReferingAxis.AxisRepresentation == AxisRepresentations.AxisY && this.ReferingAxis.Logarithmic)
				{
					return DataPointHelper.ConvertYValue2LogarithmicValue(base.Chart as Chart, this._internalNumericValue, this.AxisType);
				}
				if (this.ReferingAxis.AxisRepresentation == AxisRepresentations.AxisX && this.ReferingAxis.Logarithmic)
				{
					return DataPointHelper.ConvertXValue2LogarithmicValue(base.Chart as Chart, this._internalNumericValue, this.AxisType);
				}
				return this._internalNumericValue;
			}
			set
			{
				this._internalNumericValue = value;
			}
		}

		internal double InternalNumericStartValue
		{
			get
			{
				if (this.ReferingAxis.AxisRepresentation == AxisRepresentations.AxisY && this.ReferingAxis.Logarithmic)
				{
					return DataPointHelper.ConvertYValue2LogarithmicValue(base.Chart as Chart, this._internalStartNumericValue, this.AxisType);
				}
				if (this.ReferingAxis.AxisRepresentation == AxisRepresentations.AxisX && this.ReferingAxis.Logarithmic)
				{
					return DataPointHelper.ConvertXValue2LogarithmicValue(base.Chart as Chart, this._internalStartNumericValue, this.AxisType);
				}
				return this._internalStartNumericValue;
			}
			set
			{
				this._internalStartNumericValue = value;
			}
		}

		internal double InternalNumericEndValue
		{
			get
			{
				if (this.ReferingAxis.AxisRepresentation == AxisRepresentations.AxisY && this.ReferingAxis.Logarithmic)
				{
					return DataPointHelper.ConvertYValue2LogarithmicValue(base.Chart as Chart, this._internalEndNumericValue, this.AxisType);
				}
				if (this.ReferingAxis.AxisRepresentation == AxisRepresentations.AxisX && this.ReferingAxis.Logarithmic)
				{
					return DataPointHelper.ConvertXValue2LogarithmicValue(base.Chart as Chart, this._internalEndNumericValue, this.AxisType);
				}
				return this._internalEndNumericValue;
			}
			set
			{
				this._internalEndNumericValue = value;
			}
		}

		internal Axis ReferingAxis
		{
			get;
			set;
		}

		internal Canvas Visual
		{
			get;
			set;
		}

		private Line Line
		{
			get;
			set;
		}

		private Rectangle Rectangle
		{
			get;
			set;
		}

		private Line Shadow
		{
			get;
			set;
		}

		private Rectangle ShadowRectangle
		{
			get;
			set;
		}

		static TrendLine()
		{
			TrendLine.EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool?), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnEnabledPropertyChanged)));
			TrendLine.OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(TrendLine), new PropertyMetadata(1.0, new PropertyChangedCallback(TrendLine.OnOpacityPropertyChanged)));
			TrendLine.LabelTextProperty = DependencyProperty.Register("LabelText", typeof(string), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnLabelTextPropertyChanged)));
			TrendLine.LabelFontFamilyProperty = DependencyProperty.Register("LabelFontFamily", typeof(FontFamily), typeof(TrendLine), new PropertyMetadata(new FontFamily("Verdana"), new PropertyChangedCallback(TrendLine.OnLabelFontFamilyPropertyChanged)));
			TrendLine.LabelFontSizeProperty = DependencyProperty.Register("LabelFontSize", typeof(double), typeof(TrendLine), new PropertyMetadata(11.0, new PropertyChangedCallback(TrendLine.OnLabelFontSizePropertyChanged)));
			TrendLine.LabelFontColorProperty = DependencyProperty.Register("LabelFontColor", typeof(Brush), typeof(TrendLine), new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(TrendLine.OnLabelFontColorPropertyChanged)));
			TrendLine.LabelFontWeightProperty = DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnLabelFontWeightPropertyChanged)));
			TrendLine.LabelFontStyleProperty = DependencyProperty.Register("LabelFontStyle", typeof(FontStyle), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnLabelFontStylePropertyChanged)));
			TrendLine.LineColorProperty = DependencyProperty.Register("LineColor", typeof(Brush), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnLineColorPropertyChanged)));
			TrendLine.LineThicknessProperty = DependencyProperty.Register("LineThickness", typeof(double), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnLineThicknessPropertyChanged)));
			TrendLine.LineStyleProperty = DependencyProperty.Register("LineStyle", typeof(LineStyles), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnLineStylePropertyChanged)));
			TrendLine.ShadowEnabledProperty = DependencyProperty.Register("ShadowEnabled", typeof(bool), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnShadowEnabledPropertyChanged)));
			TrendLine.ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnValuePropertyChanged)));
			TrendLine.StartValueProperty = DependencyProperty.Register("StartValue", typeof(object), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnStartValuePropertyChanged)));
			TrendLine.EndValueProperty = DependencyProperty.Register("EndValue", typeof(object), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnEndValuePropertyChanged)));
			TrendLine.HrefTargetProperty = DependencyProperty.Register("HrefTarget", typeof(HrefTargets), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnHrefTargetChanged)));
			TrendLine.HrefProperty = DependencyProperty.Register("Href", typeof(string), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnHrefChanged)));
			TrendLine.AxisTypeProperty = DependencyProperty.Register("AxisType", typeof(AxisTypes), typeof(TrendLine), new PropertyMetadata(new PropertyChangedCallback(TrendLine.OnAxisTypePropertyChanged)));
			TrendLine.OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(TrendLine), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(TrendLine.OnOrientationPropertyChanged)));
			if (!TrendLine._defaultStyleKeyApplied)
			{
				FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TrendLine), new FrameworkPropertyMetadata(typeof(TrendLine)));
				TrendLine._defaultStyleKeyApplied = true;
			}
		}

		private static void OnEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.FirePropertyChanged(VcProperties.Enabled);
		}

		private static void OnOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.FirePropertyChanged(VcProperties.Opacity);
		}

		private static void OnLabelTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.UpdateVisual(VcProperties.LabelText, e.NewValue);
		}

		private static void OnLabelFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.UpdateVisual(VcProperties.LabelFontFamily, e.NewValue);
		}

		private static void OnLabelFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.UpdateVisual(VcProperties.LabelFontSize, e.NewValue);
		}

		private static void OnLabelFontColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.UpdateVisual(VcProperties.LabelFontColor, e.NewValue);
		}

		private static void OnLabelFontWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.UpdateVisual(VcProperties.LabelFontWeight, e.NewValue);
		}

		private static void OnLabelFontStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.UpdateVisual(VcProperties.LabelFontStyle, e.NewValue);
		}

		private static void OnLineColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.UpdateVisual(VcProperties.LineColor, e.NewValue);
		}

		private static void OnLineThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.UpdateVisual(VcProperties.LineThickness, e.NewValue);
		}

		private static void OnLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.UpdateVisual(VcProperties.LineStyle, e.NewValue);
		}

		private static void OnShadowEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.UpdateVisual(VcProperties.ShadowEnabled, e.NewValue);
		}

		private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			if (trendLine.Value != null)
			{
				if (e.NewValue.GetType().Equals(typeof(double)) || e.NewValue.GetType().Equals(typeof(int)))
				{
					if (double.IsNaN(Convert.ToDouble(e.NewValue)))
					{
						trendLine.RemoveVisual(trendLine);
						return;
					}
					trendLine.InternalNumericValue = Convert.ToDouble(e.NewValue);
				}
				else if (e.NewValue.GetType().Equals(typeof(DateTime)))
				{
					trendLine.InternalDateValue = (DateTime)e.NewValue;
				}
				else
				{
					if (!e.NewValue.GetType().Equals(typeof(string)))
					{
						throw new Exception("Invalid Input for AxisMaximum");
					}
					if (string.IsNullOrEmpty(e.NewValue.ToString()))
					{
						trendLine.RemoveVisual(trendLine);
						return;
					}
					double internalNumericValue;
					if (double.TryParse((string)e.NewValue, NumberStyles.Number, CultureInfo.InvariantCulture, out internalNumericValue))
					{
						trendLine.InternalNumericValue = internalNumericValue;
					}
					else
					{
						DateTime internalDateValue;
						if (!DateTime.TryParse((string)e.NewValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out internalDateValue))
						{
							throw new Exception("Invalid Input for AxisMaximum");
						}
						trendLine.InternalDateValue = internalDateValue;
					}
				}
				trendLine.UpdateVisual(VcProperties.Value, e.NewValue);
				return;
			}
			if (trendLine.Visual == null)
			{
				return;
			}
			trendLine.RemoveVisual(trendLine);
		}

		private void RemoveVisual(TrendLine trendLine)
		{
			if (trendLine.Visual != null)
			{
				Panel panel = trendLine.Visual.Parent as Panel;
				if (panel != null)
				{
					panel.Children.Remove(trendLine.Visual);
					trendLine.Visual = null;
					trendLine.Rectangle = null;
					trendLine.ShadowRectangle = null;
					trendLine.Line = null;
					trendLine.Shadow = null;
				}
			}
		}

		private static void OnStartValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			if (trendLine.StartValue != null)
			{
				if (e.NewValue.GetType().Equals(typeof(double)) || e.NewValue.GetType().Equals(typeof(int)))
				{
					if (double.IsNaN(Convert.ToDouble(e.NewValue)))
					{
						trendLine.RemoveVisual(trendLine);
						return;
					}
					trendLine.InternalNumericStartValue = Convert.ToDouble(e.NewValue);
				}
				else if (e.NewValue.GetType().Equals(typeof(DateTime)))
				{
					trendLine.InternalDateStartValue = (DateTime)e.NewValue;
				}
				else
				{
					if (!e.NewValue.GetType().Equals(typeof(string)))
					{
						throw new Exception("Invalid Input for AxisMaximum");
					}
					if (string.IsNullOrEmpty(e.NewValue.ToString()))
					{
						trendLine.RemoveVisual(trendLine);
						return;
					}
					double internalNumericStartValue;
					if (double.TryParse((string)e.NewValue, NumberStyles.Number, CultureInfo.InvariantCulture, out internalNumericStartValue))
					{
						trendLine.InternalNumericStartValue = internalNumericStartValue;
					}
					else
					{
						DateTime internalDateStartValue;
						if (!DateTime.TryParse((string)e.NewValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out internalDateStartValue))
						{
							throw new Exception("Invalid Input for AxisMaximum");
						}
						trendLine.InternalDateStartValue = internalDateStartValue;
					}
				}
				trendLine.UpdateVisual(VcProperties.StartValue, e.NewValue);
				return;
			}
			if (trendLine.Visual == null)
			{
				return;
			}
			trendLine.RemoveVisual(trendLine);
		}

		private static void OnEndValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			if (trendLine.EndValue != null)
			{
				if (e.NewValue.GetType().Equals(typeof(double)) || e.NewValue.GetType().Equals(typeof(int)))
				{
					if (double.IsNaN(Convert.ToDouble(e.NewValue)))
					{
						trendLine.RemoveVisual(trendLine);
						return;
					}
					trendLine.InternalNumericEndValue = Convert.ToDouble(e.NewValue);
				}
				else if (e.NewValue.GetType().Equals(typeof(DateTime)))
				{
					trendLine.InternalDateEndValue = (DateTime)e.NewValue;
				}
				else
				{
					if (!e.NewValue.GetType().Equals(typeof(string)))
					{
						throw new Exception("Invalid Input for AxisMaximum");
					}
					if (string.IsNullOrEmpty(e.NewValue.ToString()))
					{
						trendLine.RemoveVisual(trendLine);
						return;
					}
					double internalNumericEndValue;
					if (double.TryParse((string)e.NewValue, NumberStyles.Number, CultureInfo.InvariantCulture, out internalNumericEndValue))
					{
						trendLine.InternalNumericEndValue = internalNumericEndValue;
					}
					else
					{
						DateTime internalDateEndValue;
						if (!DateTime.TryParse((string)e.NewValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out internalDateEndValue))
						{
							throw new Exception("Invalid Input for AxisMaximum");
						}
						trendLine.InternalDateEndValue = internalDateEndValue;
					}
				}
				trendLine.UpdateVisual(VcProperties.EndValue, e.NewValue);
				return;
			}
			if (trendLine.Visual == null)
			{
				return;
			}
			trendLine.RemoveVisual(trendLine);
		}

		private static void OnAxisTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.FirePropertyChanged(VcProperties.AxisType);
		}

		private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.FirePropertyChanged(VcProperties.Orientation);
		}

		private static void OnHrefTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.FirePropertyChanged(VcProperties.HrefTarget);
		}

		private static void OnHrefChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TrendLine trendLine = d as TrendLine;
			trendLine.FirePropertyChanged(VcProperties.Href);
		}

		internal void ApplyLabelFontProperties(TextBlock tb)
		{
			if (tb != null)
			{
				tb.Text = this.LabelText;
				tb.FontFamily = this.LabelFontFamily;
				tb.FontSize = this.LabelFontSize;
				tb.FontStyle = this.LabelFontStyle;
				tb.FontWeight = this.LabelFontWeight;
				tb.Foreground = this.LabelFontColor;
			}
		}

		private void ApplyProperties()
		{
			if (this.Line != null)
			{
				this.Line.Stroke = this.LineColor;
				this.Line.StrokeThickness = this.LineThickness;
				this.Line.StrokeDashArray = ExtendedGraphics.GetDashArray(this.LineStyle);
			}
			if (!VisifireControl.IsMediaEffectsEnabled)
			{
				this.ApplyShadow4XBAP();
				return;
			}
			this.ApplyShadow();
		}

		private void ApplyRectangleProperties()
		{
			if (this.Rectangle != null)
			{
				this.Rectangle.Fill = this.LineColor;
			}
			if (!VisifireControl.IsMediaEffectsEnabled)
			{
				this.ApplyShadow4XBAP();
				return;
			}
			this.ApplyShadow();
		}

		private void ApplyShadow4XBAP()
		{
			if (this.Shadow != null)
			{
				this.Shadow.StrokeThickness = this.LineThickness + 2.0;
				this.Shadow.StrokeDashArray = ExtendedGraphics.GetDashArray(this.LineStyle);
				this.Shadow.Stroke = new SolidColorBrush(Colors.LightGray);
				this.Shadow.Opacity = 0.7;
				this.Shadow.StrokeDashCap = PenLineCap.Round;
				this.Shadow.StrokeLineJoin = PenLineJoin.Round;
				this.Shadow.Visibility = (this.ShadowEnabled ? Visibility.Visible : Visibility.Collapsed);
			}
			if (this.ShadowRectangle != null)
			{
				this.ShadowRectangle.Fill = new SolidColorBrush(Colors.LightGray);
				this.ShadowRectangle.Opacity = 0.7;
				this.ShadowRectangle.Visibility = (this.ShadowEnabled ? Visibility.Visible : Visibility.Collapsed);
			}
		}

		private void ApplyShadow()
		{
			if (this.ShadowEnabled)
			{
				if (this.Line != null)
				{
					this.Line.Effect = new DropShadowEffect
					{
						Opacity = 0.5
					};
				}
				if (this.Rectangle != null)
				{
					this.Rectangle.Effect = new DropShadowEffect
					{
						Opacity = 0.5
					};
					return;
				}
			}
			else
			{
				if (this.Line != null)
				{
					this.Line.Effect = null;
				}
				if (this.Rectangle != null)
				{
					this.Rectangle.Effect = null;
				}
			}
		}

		internal override void UpdateVisual(VcProperties propertyName, object value)
		{
			if (base.Chart == null)
			{
				return;
			}
			if (this.StartValue != null && this.EndValue != null && this.Value != null)
			{
				throw new Exception("Value property cannot be set with StartValue and EndValue in TrendLine");
			}
			if (this.Visual == null || (this.Value != null && this.Line == null) || (this.StartValue != null && this.EndValue != null && this.Rectangle == null))
			{
				base.FirePropertyChanged(propertyName);
				return;
			}
			if (!VisifireControl.IsMediaEffectsEnabled && propertyName == VcProperties.ShadowEnabled)
			{
				base.FirePropertyChanged(propertyName);
				return;
			}
			if (propertyName == VcProperties.Value || propertyName == VcProperties.StartValue || propertyName == VcProperties.EndValue)
			{
				Chart chart = base.Chart as Chart;
				Axis axisXFromChart = chart.PlotDetails.GetAxisXFromChart(chart, this.AxisType);
				chart.PlotDetails.SetTrendLineValue(this, axisXFromChart);
				chart.PlotDetails.SetTrendLineStartAndEndValue(this, axisXFromChart);
				Canvas chartVisualCanvas = (base.Chart as Chart).ChartArea.ChartVisualCanvas;
				this.PositionTrendLineLabel(chartVisualCanvas.Width, chartVisualCanvas.Height);
				this.PositionTheLine(chartVisualCanvas.Width, chartVisualCanvas.Height);
				this.PositionTheStartEndRectangle(chartVisualCanvas.Width, chartVisualCanvas.Height);
				return;
			}
			if (propertyName == VcProperties.LabelText && (this.LabelTextBlock == null || value == null))
			{
				base.FirePropertyChanged(propertyName);
				return;
			}
			this.ApplyProperties();
			this.ApplyRectangleProperties();
			this.ApplyLabelFontProperties(this.LabelTextBlock);
			this.PositionTrendLineLabel((base.Chart as Chart).ChartArea.ChartVisualCanvas.Width, (base.Chart as Chart).ChartArea.ChartVisualCanvas.Height);
		}

		internal void UpdateTrendLineLabelPosition(double width, double height)
		{
			this.PositionTrendLineLabel(width, height);
		}

		private void PositionTrendLineLabel(double width, double height)
		{
			if (this.LabelTextBlock != null)
			{
				Chart chart = base.Chart as Chart;
				if (this.ReferingAxis == null)
				{
					return;
				}
				if (this.Value == null)
				{
					this.InternalNumericValue = (this.InternalNumericStartValue + this.InternalNumericEndValue) / 2.0;
				}
				double x = Graphics.ValueToPixelPosition(0.0, width, this.ReferingAxis.InternalAxisMinimum, this.ReferingAxis.InternalAxisMaximum, this.InternalNumericValue);
				double y = Graphics.ValueToPixelPosition(height, 0.0, this.ReferingAxis.InternalAxisMinimum, this.ReferingAxis.InternalAxisMaximum, this.InternalNumericValue);
				double num = chart.ChartArea.AxisX.GetScrollBarValueFromOffset(chart.ChartArea.AxisX.CurrentScrollScrollBarOffset);
				num = chart.ChartArea.GetScrollingOffsetOfAxis(chart.ChartArea.AxisX, num);
				Size size = Graphics.CalculateVisualSize(this.LabelTextBlock);
				if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					if (this.AxisType == AxisTypes.Primary)
					{
						Point positionWithRespect2ChartArea;
						if (this.Orientation == Orientation.Horizontal)
						{
							positionWithRespect2ChartArea = chart.ChartArea.GetPositionWithRespect2ChartArea(new Point(0.0, y));
							this.LabelTextBlock.SetValue(Canvas.LeftProperty, chart.PlotArea.GetPlotAreaStartPosition().X - 10.0 - size.Width);
							this.LabelTextBlock.SetValue(Canvas.TopProperty, positionWithRespect2ChartArea.Y - size.Height / 2.0);
							return;
						}
						positionWithRespect2ChartArea = chart.ChartArea.GetPositionWithRespect2ChartArea(new Point(x, 0.0));
						this.LabelTextBlock.SetValue(Canvas.LeftProperty, positionWithRespect2ChartArea.X - num - size.Width / 2.0);
						double num2 = Axis.GetAxisTop(chart.ChartArea.AxisX) + ((chart.IsScrollingActivated || chart.ZoomingEnabled) ? chart.ChartArea.AxisX.ScrollBarSize : 0.0);
						this.LabelTextBlock.SetValue(Canvas.TopProperty, num2 + 6.0);
						return;
					}
					else if (this.Orientation == Orientation.Horizontal)
					{
						Point positionWithRespect2ChartArea = chart.ChartArea.GetPositionWithRespect2ChartArea(new Point(0.0, y));
						double axisLeft = Axis.GetAxisLeft(chart.ChartArea.AxisY2);
						this.LabelTextBlock.SetValue(Canvas.LeftProperty, axisLeft + 10.0);
						this.LabelTextBlock.SetValue(Canvas.TopProperty, positionWithRespect2ChartArea.Y - size.Height / 2.0);
						return;
					}
				}
				else if (this.AxisType == AxisTypes.Primary)
				{
					Point positionWithRespect2ChartArea;
					if (this.Orientation == Orientation.Horizontal)
					{
						positionWithRespect2ChartArea = chart.ChartArea.GetPositionWithRespect2ChartArea(new Point(0.0, y));
						this.LabelTextBlock.SetValue(Canvas.LeftProperty, chart.PlotArea.GetPlotAreaStartPosition().X - 10.0 - ((chart.IsScrollingActivated || chart.ZoomingEnabled) ? chart.ChartArea.AxisX.ScrollBarSize : 0.0) - size.Width);
						this.LabelTextBlock.SetValue(Canvas.TopProperty, positionWithRespect2ChartArea.Y - num - size.Height / 2.0);
						return;
					}
					positionWithRespect2ChartArea = chart.ChartArea.GetPositionWithRespect2ChartArea(new Point(x, 0.0));
					this.LabelTextBlock.SetValue(Canvas.LeftProperty, positionWithRespect2ChartArea.X - size.Width / 2.0);
					double axisTop = Axis.GetAxisTop(chart.ChartArea.AxisY);
					this.LabelTextBlock.SetValue(Canvas.TopProperty, axisTop + 6.0);
					return;
				}
				else if (this.Orientation == Orientation.Vertical)
				{
					Point positionWithRespect2ChartArea = chart.ChartArea.GetPositionWithRespect2ChartArea(new Point(x, 0.0));
					this.LabelTextBlock.SetValue(Canvas.LeftProperty, positionWithRespect2ChartArea.X - size.Width / 2.0);
					double axisTop2 = Axis.GetAxisTop(chart.ChartArea.AxisY2);
					this.LabelTextBlock.SetValue(Canvas.TopProperty, axisTop2);
				}
			}
		}

		private void PositionTheStartEndRectangle(double width, double height)
		{
			double arg_06_0 = this.LineThickness;
			(base.Chart as Chart).PlotDetails.SetTrendLineStartAndEndValues(this.ReferingAxis);
			double num = this.InternalNumericStartValue;
			double num2 = this.InternalNumericEndValue;
			if (this.StartValue != null && this.EndValue != null && num > num2)
			{
				double num3 = num2;
				num2 = num;
				num = num3;
			}
			if (this.Rectangle != null)
			{
				switch (this.Orientation)
				{
				case Orientation.Horizontal:
				{
					double num4 = Graphics.ValueToPixelPosition(height, 0.0, this.ReferingAxis.InternalAxisMinimum, this.ReferingAxis.InternalAxisMaximum, num2);
					double num5 = Graphics.ValueToPixelPosition(height, 0.0, this.ReferingAxis.InternalAxisMinimum, this.ReferingAxis.InternalAxisMaximum, num);
					this.Rectangle.SetValue(Canvas.LeftProperty, 0.0);
					this.Rectangle.SetValue(Canvas.TopProperty, num4);
					this.Rectangle.Width = width;
					this.Rectangle.Height = num5 - num4;
					this.Visual.Width = width;
					break;
				}
				case Orientation.Vertical:
				{
					double num6 = Graphics.ValueToPixelPosition(0.0, width, this.ReferingAxis.InternalAxisMinimum, this.ReferingAxis.InternalAxisMaximum, num);
					double num7 = Graphics.ValueToPixelPosition(0.0, width, this.ReferingAxis.InternalAxisMinimum, this.ReferingAxis.InternalAxisMaximum, num2);
					this.Rectangle.SetValue(Canvas.LeftProperty, num6);
					this.Rectangle.SetValue(Canvas.TopProperty, 0.0);
					this.Rectangle.Width = num7 - num6;
					this.Rectangle.Height = height;
					this.Visual.Height = height;
					break;
				}
				}
			}
			if (this.ShadowRectangle != null)
			{
				if (this.Orientation == Orientation.Horizontal)
				{
					this.ShadowRectangle.SetValue(Canvas.LeftProperty, (double)this.Rectangle.GetValue(Canvas.LeftProperty));
					this.ShadowRectangle.SetValue(Canvas.TopProperty, (double)this.Rectangle.GetValue(Canvas.TopProperty) + 3.0);
					this.ShadowRectangle.Width = this.Rectangle.Width + 3.0;
					this.ShadowRectangle.Height = this.Rectangle.Height + 3.0;
					return;
				}
				this.ShadowRectangle.SetValue(Canvas.LeftProperty, (double)this.Rectangle.GetValue(Canvas.LeftProperty) + 3.0);
				this.ShadowRectangle.SetValue(Canvas.TopProperty, (double)this.Rectangle.GetValue(Canvas.TopProperty) + 3.0);
				this.ShadowRectangle.Width = this.Rectangle.Width + 3.0;
				this.ShadowRectangle.Height = this.Rectangle.Height;
			}
		}

		private void PositionTheLine(double width, double height)
		{
			double num = this.LineThickness + 2.0;
			(base.Chart as Chart).PlotDetails.SetTrendLineValues(this.ReferingAxis);
			if (this.Line != null)
			{
				switch (this.Orientation)
				{
				case Orientation.Horizontal:
					this.Line.X1 = 0.0;
					this.Line.X2 = width;
					this.Line.Y1 = Graphics.ValueToPixelPosition(height, 0.0, this.ReferingAxis.InternalAxisMinimum, this.ReferingAxis.InternalAxisMaximum, this.InternalNumericValue);
					this.Line.Y2 = this.Line.Y1;
					this.Visual.Height = num;
					this.Visual.Width = width;
					break;
				case Orientation.Vertical:
					this.Line.Y1 = 0.0;
					this.Line.Y2 = height;
					this.Line.X1 = Graphics.ValueToPixelPosition(0.0, width, this.ReferingAxis.InternalAxisMinimum, this.ReferingAxis.InternalAxisMaximum, this.InternalNumericValue);
					this.Line.X2 = this.Line.X1;
					this.Visual.Height = height;
					this.Visual.Width = num;
					break;
				}
			}
			if (this.Shadow != null)
			{
				if (this.Orientation == Orientation.Horizontal)
				{
					this.Shadow.X1 = this.Line.X1;
					this.Shadow.X2 = this.Line.X2 + 3.0;
					this.Shadow.Y1 = this.Line.Y1 + 3.0;
					this.Shadow.Y2 = this.Line.Y2 + 3.0;
					return;
				}
				this.Shadow.X1 = this.Line.X1 + 3.0;
				this.Shadow.X2 = this.Line.X2 + 3.0;
				this.Shadow.Y1 = this.Line.Y1 + 3.0;
				this.Shadow.Y2 = this.Line.Y2;
			}
		}

		internal void CreateVisualObject(double width, double height)
		{
			if (this.ReferingAxis == null || !this.Enabled.Value)
			{
				this.Visual = null;
				return;
			}
			if (this.Visual == null)
			{
				this.Visual = new Canvas();
			}
			else
			{
				this.Visual.Children.Clear();
			}
			this.Visual.Opacity = this.Opacity;
			this.Visual.Cursor = this.Cursor;
			if (this.StartValue != null && this.EndValue != null && this.Value != null)
			{
				throw new Exception("Value property cannot be set with StartValue and EndValue in TrendLine");
			}
			if (this.Value != null)
			{
				bool flag = (!this.Value.GetType().Equals(typeof(double)) && !this.Value.GetType().Equals(typeof(int))) || !double.IsNaN(Convert.ToDouble(this.Value));
				if (flag)
				{
					this.Line = new Line
					{
						Tag = new ElementData
						{
							Element = this
						}
					};
					this.Shadow = new Line
					{
						IsHitTestVisible = false
					};
					this.PositionTheLine(width, height);
					this.ApplyProperties();
					this.Visual.Children.Add(this.Shadow);
					this.Visual.Children.Add(this.Line);
					this.PositionTrendLineLabel(width, height);
				}
			}
			if (this.StartValue != null && this.EndValue != null)
			{
				bool flag2 = (!this.StartValue.GetType().Equals(typeof(double)) && !this.StartValue.GetType().Equals(typeof(int))) || (!this.EndValue.GetType().Equals(typeof(double)) && !this.EndValue.GetType().Equals(typeof(int))) || (!double.IsNaN(Convert.ToDouble(this.StartValue)) && !double.IsNaN(Convert.ToDouble(this.EndValue)));
				if (flag2)
				{
					this.Rectangle = new Rectangle
					{
						Tag = new ElementData
						{
							Element = this
						}
					};
					this.ShadowRectangle = new Rectangle
					{
						IsHitTestVisible = false
					};
					this.Visual.Children.Add(this.ShadowRectangle);
					this.Visual.Children.Add(this.Rectangle);
					this.PositionTheStartEndRectangle(width, height);
					this.ApplyRectangleProperties();
					this.PositionTrendLineLabel(width, height);
				}
			}
			if (this.Value != null)
			{
				base.AttachToolTip(this.ReferingAxis.Chart, this, this.Line);
				base.AttachHref(this.ReferingAxis.Chart, this.Line, this.Href, this.HrefTarget);
				base.AttachEvents2Visual(this, this.Line);
				return;
			}
			base.AttachToolTip(this.ReferingAxis.Chart, this, this.Rectangle);
			base.AttachHref(this.ReferingAxis.Chart, this.Rectangle, this.Href, this.HrefTarget);
			base.AttachEvents2Visual(this, this.Rectangle);
		}
	}
}
