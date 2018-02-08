using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class Ticks : ObservableObject
	{
		public static readonly DependencyProperty TickLengthProperty;

		public static readonly DependencyProperty LineStyleProperty;

		public static readonly DependencyProperty LineThicknessProperty;

		public static readonly DependencyProperty LineColorProperty;

		public static readonly DependencyProperty IntervalProperty;

		public static readonly DependencyProperty EnabledProperty;

		private new static readonly DependencyProperty ToolTipTextProperty;

		private double _width;

		private double _height;

		private static bool _defaultStyleKeyApplied;

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Enabled
		{
			get
			{
				if (!((bool?)base.GetValue(Ticks.EnabledProperty)).HasValue)
				{
					return new bool?(true);
				}
				return (bool?)base.GetValue(Ticks.EnabledProperty);
			}
			set
			{
				base.SetValue(Ticks.EnabledProperty, value);
			}
		}

		public double? Interval
		{
			get
			{
				if (!((double?)base.GetValue(Ticks.IntervalProperty)).HasValue)
				{
					return new double?(this.ParentAxis.InternalInterval);
				}
				return (double?)base.GetValue(Ticks.IntervalProperty);
			}
			set
			{
				base.SetValue(Ticks.IntervalProperty, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToolTipText
		{
			get
			{
				throw new NotImplementedException("ToolTipText property for Ticks is not implemented");
			}
			set
			{
				throw new NotImplementedException("ToolTipText property for Ticks is not implemented");
			}
		}

		public Brush LineColor
		{
			get
			{
				return (Brush)base.GetValue(Ticks.LineColorProperty);
			}
			set
			{
				base.SetValue(Ticks.LineColorProperty, value);
			}
		}

		public double LineThickness
		{
			get
			{
				return (double)base.GetValue(Ticks.LineThicknessProperty);
			}
			set
			{
				base.SetValue(Ticks.LineThicknessProperty, value);
			}
		}

		public LineStyles LineStyle
		{
			get
			{
				return (LineStyles)base.GetValue(Ticks.LineStyleProperty);
			}
			set
			{
				base.SetValue(Ticks.LineStyleProperty, value);
			}
		}

		public double? TickLength
		{
			get
			{
				if (base.GetValue(Ticks.TickLengthProperty) == null)
				{
					return new double?(5.0);
				}
				return (double?)base.GetValue(Ticks.TickLengthProperty);
			}
			set
			{
				base.SetValue(Ticks.TickLengthProperty, value);
			}
		}

		private new Brush Background
		{
			get;
			set;
		}

		private new Brush BorderBrush
		{
			get;
			set;
		}

		private new Thickness BorderThickness
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

		private new FontStyle FontStyle
		{
			get;
			set;
		}

		private new FontStretch FontStretch
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

		private List<Line> CachedLines
		{
			get;
			set;
		}

		internal Canvas Visual
		{
			get;
			private set;
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
			get
			{
				return this._width;
			}
			set
			{
				this._width = value;
			}
		}

		internal new double Height
		{
			get
			{
				return this._height;
			}
			set
			{
				this._height = value;
			}
		}

		internal PlacementTypes Placement
		{
			get;
			set;
		}

		internal Dictionary<double, string> AxisLabelsDictionary
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

		private ChartOrientationType OldChartOrientation
		{
			get;
			set;
		}

		static Ticks()
		{
			Ticks.TickLengthProperty = DependencyProperty.Register("TickLength", typeof(double?), typeof(Ticks), new PropertyMetadata(new PropertyChangedCallback(Ticks.OnTickLengthPropertyChanged)));
			Ticks.LineStyleProperty = DependencyProperty.Register("LineStyle", typeof(LineStyles), typeof(Ticks), new PropertyMetadata(new PropertyChangedCallback(Ticks.OnLineStylePropertyChanged)));
			Ticks.LineThicknessProperty = DependencyProperty.Register("LineThickness", typeof(double), typeof(Ticks), new PropertyMetadata(0.5, new PropertyChangedCallback(Ticks.OnLineThicknessPropertyChanged)));
			Ticks.LineColorProperty = DependencyProperty.Register("LineColor", typeof(Brush), typeof(Ticks), new PropertyMetadata(new SolidColorBrush(Colors.Gray), new PropertyChangedCallback(Ticks.OnLineColorPropertyChanged)));
			Ticks.IntervalProperty = DependencyProperty.Register("Interval", typeof(double?), typeof(Ticks), new PropertyMetadata(new PropertyChangedCallback(Ticks.OnIntervalPropertyChanged)));
			Ticks.EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool?), typeof(Ticks), new PropertyMetadata(new PropertyChangedCallback(Ticks.OnEnabledPropertyChanged)));
			Ticks.ToolTipTextProperty = DependencyProperty.Register("ToolTipText", typeof(string), typeof(Ticks), null);
			if (!Ticks._defaultStyleKeyApplied)
			{
				FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Ticks), new FrameworkPropertyMetadata(typeof(Ticks)));
				Ticks._defaultStyleKeyApplied = true;
			}
		}

		private static void OnEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Ticks ticks = d as Ticks;
			ticks.FirePropertyChanged(VcProperties.Enabled);
		}

		private static void OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Ticks ticks = d as Ticks;
			ticks.FirePropertyChanged(VcProperties.Interval);
		}

		private static void OnLineColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Ticks ticks = d as Ticks;
			ticks.UpdateVisual(VcProperties.LineColor, e.NewValue);
		}

		private static void OnLineThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Ticks ticks = d as Ticks;
			ticks.UpdateVisual(VcProperties.LineThickness, e.NewValue);
		}

		private static void OnLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Ticks ticks = d as Ticks;
			ticks.UpdateVisual(VcProperties.LineStyle, e.NewValue);
		}

		private static void OnTickLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Ticks ticks = d as Ticks;
			ticks.FirePropertyChanged(VcProperties.TickLength);
		}

		private void UpdatePosition4MajorTicks()
		{
			double num = double.IsNaN(this.ParentAxis.StartOffset) ? 0.0 : this.ParentAxis.StartOffset;
			double num2 = double.IsNaN(this.ParentAxis.EndOffset) ? 0.0 : this.ParentAxis.EndOffset;
			double num3 = this.Interval.Value;
			decimal d = 0m;
			decimal num4 = (decimal)this.Minimum;
			decimal d2 = (decimal)this.Maximum;
			decimal d3 = (decimal)num3;
			if (this.ParentAxis.AxisRepresentation == AxisRepresentations.AxisX)
			{
				if (double.IsNaN(this.ParentAxis.AxisMinimumNumeric))
				{
					if (this.ParentAxis.XValueType != ChartValueTypes.Numeric)
					{
						num4 = (decimal)this.ParentAxis.FirstLabelPosition;
					}
					else if ((this.DataMinimum - this.Minimum) / num3 >= 1.0)
					{
						num4 = (decimal)(this.DataMinimum - Math.Floor((this.DataMinimum - this.Minimum) / num3) * num3);
					}
					else
					{
						num4 = (decimal)this.DataMinimum;
					}
				}
				Chart chart = base.Chart as Chart;
				if ((chart.ChartArea.AxisY != null && chart.ChartArea.AxisY.ViewportRangeEnabled) || (chart.ChartArea.AxisY2 != null && chart.ChartArea.AxisY2.ViewportRangeEnabled))
				{
					List<double> list = new List<double>();
					decimal num5 = num4;
					while (num5 <= d2)
					{
						list.Add((double)num5);
						d += this.ParentAxis.SkipOffset + 1;
						if (this.ParentAxis.IsDateTimeAxis)
						{
							DateTime dateTime = DateTimeHelper.UpdateDate(this.ParentAxis.FirstLabelDate, (double)(d * d3), this.ParentAxis.InternalIntervalType);
							decimal d4 = (decimal)DateTimeHelper.DateDiff(dateTime, this.ParentAxis.FirstLabelDate, this.ParentAxis.MinDateRange, this.ParentAxis.MaxDateRange, this.ParentAxis.InternalIntervalType, this.ParentAxis.XValueType);
							num5 = num4 + d4;
						}
						else
						{
							num5 = num4 + d * d3;
						}
					}
					double[] valuesUnderViewPort = Axis.GetValuesUnderViewPort(list, this.ParentAxis);
					num4 = (decimal)valuesUnderViewPort[0];
					d2 = (decimal)valuesUnderViewPort[1];
					d = 0m;
				}
			}
			foreach (Line current in this.CachedLines)
			{
				current.Visibility = Visibility.Collapsed;
			}
			int num6 = 0;
			if (num4 != d2)
			{
				decimal num7 = num4;
				while (num7 <= d2)
				{
					Line line = this.CachedLines[num6];
					line.Visibility = Visibility.Visible;
					switch (this.Placement)
					{
					case PlacementTypes.Top:
					{
						double num8 = Graphics.ValueToPixelPosition(num, this.Width - num2, this.Minimum, this.Maximum, (double)num7);
						if (double.IsNaN(num8))
						{
							return;
						}
						line.X1 = num8;
						line.X2 = num8;
						line.Y1 = 0.0;
						line.Y2 = this.TickLength.Value;
						break;
					}
					case PlacementTypes.Left:
					case PlacementTypes.Right:
					{
						double num8 = Graphics.ValueToPixelPosition(this.Height - num2, num, this.Minimum, this.Maximum, (double)num7);
						if (double.IsNaN(num8))
						{
							return;
						}
						line.X1 = 0.0;
						line.X2 = this.TickLength.Value;
						line.Y1 = num8;
						line.Y2 = num8;
						break;
					}
					case PlacementTypes.Bottom:
					{
						double num8 = Graphics.ValueToPixelPosition(num, this.Width - num2, this.Minimum, this.Maximum, (double)num7);
						if (double.IsNaN(num8))
						{
							return;
						}
						line.X1 = num8;
						line.X2 = num8;
						line.Y1 = 0.0;
						line.Y2 = this.TickLength.Value;
						break;
					}
					}
					num6++;
					d += this.ParentAxis.SkipOffset + 1;
					if (this.ParentAxis.IsDateTimeAxis)
					{
						DateTime dateTime2 = DateTimeHelper.UpdateDate(this.ParentAxis.FirstLabelDate, (double)(d * d3), this.ParentAxis.InternalIntervalType);
						decimal d5 = (decimal)DateTimeHelper.DateDiff(dateTime2, this.ParentAxis.FirstLabelDate, this.ParentAxis.MinDateRange, this.ParentAxis.MaxDateRange, this.ParentAxis.InternalIntervalType, this.ParentAxis.XValueType);
						num7 = Math.Round(num4 + d5, 7);
					}
					else
					{
						num7 = Math.Round(num4 + d * d3, 7);
					}
				}
			}
			switch (this.Placement)
			{
			case PlacementTypes.Top:
			case PlacementTypes.Bottom:
				this.Visual.Width = this.Width;
				this.Visual.Height = this.TickLength.Value;
				if (this.Width == 0.0)
				{
					this.Visual.Visibility = Visibility.Collapsed;
					return;
				}
				break;
			case PlacementTypes.Left:
			case PlacementTypes.Right:
				this.Visual.Height = this.Height;
				this.Visual.Width = this.TickLength.Value;
				if (this.Height == 0.0)
				{
					this.Visual.Visibility = Visibility.Collapsed;
				}
				break;
			default:
				return;
			}
		}

		private void CreateAndPositionMajorTicks()
		{
			double num = double.IsNaN(this.ParentAxis.StartOffset) ? 0.0 : this.ParentAxis.StartOffset;
			double num2 = double.IsNaN(this.ParentAxis.EndOffset) ? 0.0 : this.ParentAxis.EndOffset;
			double num3 = this.Interval.Value;
			decimal d = 0m;
			decimal num4 = (decimal)this.Minimum;
			decimal d2 = (decimal)this.Maximum;
			decimal d3 = (decimal)num3;
			if (this.ParentAxis.AxisRepresentation == AxisRepresentations.AxisX)
			{
				if (double.IsNaN(this.ParentAxis.AxisMinimumNumeric))
				{
					if (this.ParentAxis.XValueType != ChartValueTypes.Numeric)
					{
						num4 = (decimal)this.ParentAxis.FirstLabelPosition;
					}
					else if ((this.DataMinimum - this.Minimum) / num3 >= 1.0)
					{
						num4 = (decimal)(this.DataMinimum - Math.Floor((this.DataMinimum - this.Minimum) / num3) * num3);
					}
					else
					{
						num4 = (decimal)this.DataMinimum;
					}
				}
				Chart chart = base.Chart as Chart;
				if ((chart.ScrollingEnabled.Value || chart.ZoomingEnabled) && ((chart.ChartArea.AxisY != null && chart.ChartArea.AxisY.ViewportRangeEnabled) || (chart.ChartArea.AxisY2 != null && chart.ChartArea.AxisY2.ViewportRangeEnabled)))
				{
					int num5 = 0;
					List<double> list = new List<double>();
					decimal num6 = num4;
					while (num6 <= d2)
					{
						list.Add((double)num6);
						if (this.ParentAxis.IsDateTimeAxis)
						{
							num5++;
							DateTime dateTime = DateTimeHelper.UpdateDate(this.ParentAxis.FirstLabelDate, (double)(num5 * d3), this.ParentAxis.InternalIntervalType);
							decimal d4 = (decimal)DateTimeHelper.DateDiff(dateTime, this.ParentAxis.FirstLabelDate, this.ParentAxis.MinDateRange, this.ParentAxis.MaxDateRange, this.ParentAxis.InternalIntervalType, this.ParentAxis.XValueType);
							num6 = num4 + d4;
						}
						else
						{
							num6 = num4 + ++num5 * d3;
						}
					}
					double[] valuesUnderViewPort = Axis.GetValuesUnderViewPort(list, this.ParentAxis);
					num4 = (decimal)valuesUnderViewPort[0];
					d2 = (decimal)valuesUnderViewPort[1];
					d = 0m;
				}
			}
			int num7 = 0;
			List<Line> list2 = new List<Line>();
			if (num4 != d2)
			{
				decimal num8 = num4;
				while (num8 <= d2)
				{
					Line line = null;
					if (this.CachedLines.Count > num7)
					{
						line = this.CachedLines[num7];
					}
					if (line == null)
					{
						line = new Line();
						line.Stroke = this.LineColor;
						line.StrokeThickness = this.LineThickness;
						if (this.LineStyle != LineStyles.Solid)
						{
							line.StrokeDashArray = ExtendedGraphics.GetDashArray(this.LineStyle);
						}
						this.Visual.Children.Add(line);
						this.CachedLines.Add(line);
					}
					num7++;
					switch (this.Placement)
					{
					case PlacementTypes.Top:
					{
						double num9 = Graphics.ValueToPixelPosition(num, this.Width - num2, this.Minimum, this.Maximum, (double)num8);
						if (double.IsNaN(num9))
						{
							return;
						}
						line.X1 = num9;
						line.X2 = num9;
						line.Y1 = 0.0;
						line.Y2 = this.TickLength.Value;
						break;
					}
					case PlacementTypes.Left:
					case PlacementTypes.Right:
					{
						double num9 = Graphics.ValueToPixelPosition(this.Height - num2, num, this.Minimum, this.Maximum, (double)num8);
						if (double.IsNaN(num9))
						{
							return;
						}
						line.X1 = 0.0;
						line.X2 = this.TickLength.Value;
						line.Y1 = num9;
						line.Y2 = num9;
						break;
					}
					case PlacementTypes.Bottom:
					{
						double num9 = Graphics.ValueToPixelPosition(num, this.Width - num2, this.Minimum, this.Maximum, (double)num8);
						if (double.IsNaN(num9))
						{
							return;
						}
						line.X1 = num9;
						line.X2 = num9;
						line.Y1 = 0.0;
						line.Y2 = this.TickLength.Value;
						break;
					}
					}
					d += this.ParentAxis.SkipOffset + 1;
					if (this.ParentAxis.IsDateTimeAxis)
					{
						DateTime dateTime2 = DateTimeHelper.UpdateDate(this.ParentAxis.FirstLabelDate, (double)(d * d3), this.ParentAxis.InternalIntervalType);
						decimal d5 = (decimal)DateTimeHelper.DateDiff(dateTime2, this.ParentAxis.FirstLabelDate, this.ParentAxis.MinDateRange, this.ParentAxis.MaxDateRange, this.ParentAxis.InternalIntervalType, this.ParentAxis.XValueType);
						num8 = Math.Round(num4 + d5, 7);
					}
					else
					{
						num8 = Math.Round(num4 + d * d3, 7);
					}
				}
			}
			switch (this.Placement)
			{
			case PlacementTypes.Top:
			case PlacementTypes.Bottom:
				this.Visual.Width = this.Width;
				this.Visual.Height = this.TickLength.Value;
				break;
			case PlacementTypes.Left:
			case PlacementTypes.Right:
				this.Visual.Height = this.Height;
				this.Visual.Width = this.TickLength.Value;
				break;
			}
			for (int i = num7; i < this.CachedLines.Count; i++)
			{
				list2.Add(this.CachedLines[i]);
			}
			this.CachedLines.RemoveRange(num7, list2.Count);
			foreach (Line current in list2)
			{
				this.Visual.Children.Remove(current);
			}
		}

		internal void SetParms(PlacementTypes placementTypes, double width, double height)
		{
			this.Placement = placementTypes;
			if (!double.IsNaN(width))
			{
				this.Width = width;
			}
			if (!double.IsNaN(height))
			{
				this.Height = height;
			}
		}

		internal void UpdateVisualObject()
		{
			if (!this.ParentAxis.Enabled.Value)
			{
				return;
			}
			if (!this.Enabled.Value)
			{
				this.Visual = null;
				return;
			}
			this.UpdatePosition4MajorTicks();
		}

		internal void CreateVisualObject()
		{
			if (!this.ParentAxis.Enabled.Value && (base.Chart as Chart).PlotDetails.ChartOrientation != ChartOrientationType.Circular)
			{
				return;
			}
			if (!this.Enabled.Value)
			{
				this.Visual = null;
				return;
			}
			if ((base.Chart as Chart).PlotDetails.ChartOrientation == ChartOrientationType.Circular || (base.Chart as Chart).PlotDetails.ChartOrientation != this.OldChartOrientation)
			{
				this.Visual = null;
			}
			if (this.Visual == null)
			{
				this.Visual = new Canvas();
				this.CachedLines = new List<Line>();
			}
			if (this.Visual.Visibility == Visibility.Collapsed)
			{
				this.Visual.Visibility = Visibility.Visible;
			}
			this.Visual.Opacity = base.Opacity;
			this.CreateAndPositionMajorTicks();
			this.OldChartOrientation = (base.Chart as Chart).PlotDetails.ChartOrientation;
		}

		internal override void UpdateVisual(VcProperties property, object value)
		{
			if (this.Visual != null)
			{
				using (IEnumerator enumerator = this.Visual.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Line line = (Line)enumerator.Current;
						line.Stroke = this.LineColor;
						line.StrokeThickness = this.LineThickness;
						line.StrokeDashArray = ExtendedGraphics.GetDashArray(this.LineStyle);
					}
					return;
				}
			}
			base.FirePropertyChanged(property);
		}
	}
}
