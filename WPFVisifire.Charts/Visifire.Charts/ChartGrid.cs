using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class ChartGrid : ObservableObject
	{
		public static readonly DependencyProperty IntervalProperty;

		public static readonly DependencyProperty EnabledProperty;

		public static readonly DependencyProperty LineColorProperty;

		public static readonly DependencyProperty LineStyleProperty;

		public static readonly DependencyProperty LineThicknessProperty;

		public static readonly DependencyProperty InterlacedColorProperty;

		public static readonly DependencyProperty AnimationEnabledProperty;

		private static readonly DependencyProperty InternalOpacityProperty;

		public new static readonly DependencyProperty OpacityProperty;

		private new static readonly DependencyProperty ToolTipTextProperty;

		private double _width;

		private double _height;

		private double _internalOpacity = double.NaN;

		private static bool _defaultStyleKeyApplied;

		public bool AnimationEnabled
		{
			get
			{
				return (bool)base.GetValue(ChartGrid.AnimationEnabledProperty);
			}
			set
			{
				base.SetValue(ChartGrid.AnimationEnabledProperty, value);
			}
		}

		public double? Interval
		{
			get
			{
				if (!((double?)base.GetValue(ChartGrid.IntervalProperty)).HasValue && this.ParentAxis != null)
				{
					return new double?(this.ParentAxis.InternalInterval);
				}
				return (double?)base.GetValue(ChartGrid.IntervalProperty);
			}
			set
			{
				base.SetValue(ChartGrid.IntervalProperty, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToolTipText
		{
			get
			{
				throw new NotImplementedException("ToolTipText property for ChartGrid is not implemented");
			}
			set
			{
				throw new NotImplementedException("ToolTipText property for ChartGrid is not implemented");
			}
		}

		public LineStyles LineStyle
		{
			get
			{
				return (LineStyles)base.GetValue(ChartGrid.LineStyleProperty);
			}
			set
			{
				base.SetValue(ChartGrid.LineStyleProperty, value);
			}
		}

		public Brush InterlacedColor
		{
			get
			{
				return (Brush)base.GetValue(ChartGrid.InterlacedColorProperty);
			}
			set
			{
				base.SetValue(ChartGrid.InterlacedColorProperty, value);
			}
		}

		public double? LineThickness
		{
			get
			{
				if (base.GetValue(ChartGrid.LineThicknessProperty) == null)
				{
					return new double?(0.25);
				}
				return (double?)base.GetValue(ChartGrid.LineThicknessProperty);
			}
			set
			{
				base.SetValue(ChartGrid.LineThicknessProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Enabled
		{
			get
			{
				if (!((bool?)base.GetValue(ChartGrid.EnabledProperty)).HasValue)
				{
					return new bool?(true);
				}
				return (bool?)base.GetValue(ChartGrid.EnabledProperty);
			}
			set
			{
				base.SetValue(ChartGrid.EnabledProperty, value);
			}
		}

		public Brush LineColor
		{
			get
			{
				if (base.GetValue(ChartGrid.LineColorProperty) != null)
				{
					return (Brush)base.GetValue(ChartGrid.LineColorProperty);
				}
				return Graphics.GRAY_BRUSH;
			}
			set
			{
				base.SetValue(ChartGrid.LineColorProperty, value);
			}
		}

		public new double Opacity
		{
			get
			{
				return (double)base.GetValue(ChartGrid.OpacityProperty);
			}
			set
			{
				base.SetValue(ChartGrid.OpacityProperty, value);
			}
		}

		internal List<Line> CachedGridLines
		{
			get;
			set;
		}

		internal List<Rectangle> CachedGridRectangles
		{
			get;
			set;
		}

		internal Canvas Visual
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

		internal Axis ParentAxis
		{
			get;
			set;
		}

		internal Storyboard Storyboard
		{
			get;
			set;
		}

		internal double InternalOpacity
		{
			get
			{
				return (double)(double.IsNaN(this._internalOpacity) ? base.GetValue(ChartGrid.OpacityProperty) : this._internalOpacity);
			}
			set
			{
				this._internalOpacity = value;
			}
		}

		private List<Rectangle> InterlacedRectangles
		{
			get;
			set;
		}

		private List<Line> InterlacedLines
		{
			get;
			set;
		}

		private List<Path> InterlacedPaths
		{
			get;
			set;
		}

		private ChartOrientationType OldChartOrientation
		{
			get;
			set;
		}

		static ChartGrid()
		{
			ChartGrid.IntervalProperty = DependencyProperty.Register("Interval", typeof(double?), typeof(ChartGrid), new PropertyMetadata(new PropertyChangedCallback(ChartGrid.OnIntervalPropertyChanged)));
			ChartGrid.EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool?), typeof(ChartGrid), new PropertyMetadata(new PropertyChangedCallback(ChartGrid.OnEnabledPropertyChanged)));
			ChartGrid.LineColorProperty = DependencyProperty.Register("LineColor", typeof(Brush), typeof(ChartGrid), new PropertyMetadata(new PropertyChangedCallback(ChartGrid.OnLineColorPropertyChanged)));
			ChartGrid.LineStyleProperty = DependencyProperty.Register("LineStyle", typeof(LineStyles), typeof(ChartGrid), new PropertyMetadata(new PropertyChangedCallback(ChartGrid.OnLineStylePropertyChanged)));
			ChartGrid.LineThicknessProperty = DependencyProperty.Register("LineThickness", typeof(double?), typeof(ChartGrid), new PropertyMetadata(new PropertyChangedCallback(ChartGrid.OnLineThicknessPropertyChanged)));
			ChartGrid.InterlacedColorProperty = DependencyProperty.Register("InterlacedColor", typeof(Brush), typeof(ChartGrid), new PropertyMetadata(new PropertyChangedCallback(ChartGrid.OnInterlacedColorPropertyChanged)));
			ChartGrid.AnimationEnabledProperty = DependencyProperty.Register("AnimationEnabled", typeof(bool), typeof(ChartGrid), new PropertyMetadata(true, null));
			ChartGrid.InternalOpacityProperty = DependencyProperty.Register("InternalOpacity", typeof(double), typeof(ChartGrid), new PropertyMetadata(1.0, new PropertyChangedCallback(ChartGrid.OnOpacityPropertyChanged)));
			ChartGrid.OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(ChartGrid), new PropertyMetadata(1.0, new PropertyChangedCallback(ChartGrid.OnOpacityPropertyChanged)));
			ChartGrid.ToolTipTextProperty = DependencyProperty.Register("ToolTipText", typeof(string), typeof(ChartGrid), null);
			if (!ChartGrid._defaultStyleKeyApplied)
			{
				FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartGrid), new FrameworkPropertyMetadata(typeof(ChartGrid)));
				ChartGrid._defaultStyleKeyApplied = true;
			}
		}

		private void CreateAndPositionChartGrid(bool animationEnabled, double animationDuration)
		{
			double num = this.Interval.Value;
			decimal d = 0m;
			int num2 = 0;
			decimal num3 = (decimal)this.Minimum;
			decimal d2 = (decimal)this.Maximum;
			decimal d3 = (decimal)num;
			int num4 = 0;
			double num5 = 0.0;
			double num6 = 0.0;
			if (this.ParentAxis.AxisRepresentation == AxisRepresentations.AxisX)
			{
				if (double.IsNaN(this.ParentAxis.AxisMinimumNumeric))
				{
					if (this.ParentAxis.XValueType != ChartValueTypes.Numeric)
					{
						num3 = (decimal)this.ParentAxis.FirstLabelPosition;
					}
					else if ((this.DataMinimum - this.Minimum) / num >= 1.0)
					{
						num3 = (decimal)(this.DataMinimum - Math.Floor((this.DataMinimum - this.Minimum) / num) * num);
					}
					else
					{
						num3 = (decimal)this.DataMinimum;
					}
				}
				Chart chart = base.Chart as Chart;
				if ((chart.ScrollingEnabled.Value || chart.ZoomingEnabled) && ((chart.ChartArea.AxisY != null && chart.ChartArea.AxisY.ViewportRangeEnabled) || (chart.ChartArea.AxisY2 != null && chart.ChartArea.AxisY2.ViewportRangeEnabled)))
				{
					List<double> list = new List<double>();
					int num7 = 0;
					decimal num8 = num3;
					while (num8 <= d2)
					{
						list.Add((double)num8);
						if (this.ParentAxis.IsDateTimeAxis)
						{
							num7++;
							DateTime dateTime = DateTimeHelper.UpdateDate(this.ParentAxis.FirstLabelDate, (double)(num7 * d3), this.ParentAxis.InternalIntervalType);
							decimal d4 = (decimal)DateTimeHelper.DateDiff(dateTime, this.ParentAxis.FirstLabelDate, this.ParentAxis.MinDateRange, this.ParentAxis.MaxDateRange, this.ParentAxis.InternalIntervalType, this.ParentAxis.XValueType);
							num8 = Math.Round(num3 + d4, 7);
						}
						else
						{
							num8 = Math.Round(num3 + ++num7 * d3, 7);
						}
					}
					double[] valuesUnderViewPort = Axis.GetValuesUnderViewPort(list, this.ParentAxis);
					num3 = (decimal)valuesUnderViewPort[0];
					d2 = (decimal)valuesUnderViewPort[1];
					d = 0m;
				}
			}
			if (this.Storyboard != null && this.Storyboard.GetValue(Storyboard.TargetProperty) != null)
			{
				this.Storyboard.Stop();
			}
			this.InterlacedRectangles = new List<Rectangle>();
			this.InterlacedLines = new List<Line>();
			int num9 = 0;
			List<Line> list2 = new List<Line>();
			List<Rectangle> list3 = new List<Rectangle>();
			if (num3 != d2)
			{
				decimal num10 = num3;
				while (num10 <= d2)
				{
					Line line = null;
					if (this.CachedGridLines.Count > num9)
					{
						line = this.CachedGridLines[num9];
					}
					if (line == null)
					{
						line = new Line();
						line.Stroke = this.LineColor;
						line.StrokeThickness = this.LineThickness.Value;
						if (this.LineStyle != LineStyles.Solid)
						{
							line.StrokeDashArray = ExtendedGraphics.GetDashArray(this.LineStyle);
						}
						this.Visual.Children.Add(line);
						this.CachedGridLines.Add(line);
					}
					this.InterlacedLines.Add(line);
					num9++;
					line.Width = this.Width;
					line.Height = this.Height;
					switch (this.Placement)
					{
					case PlacementTypes.Top:
					case PlacementTypes.Bottom:
						num5 = Graphics.ValueToPixelPosition(0.0, this.Width, this.Minimum, this.Maximum, (double)num10);
						line.X1 = num5;
						line.X2 = num5;
						line.Y1 = 0.0;
						line.Y2 = this.Height;
						if (num2 % 2 == 1)
						{
							Rectangle rectangle = null;
							if (this.CachedGridRectangles.Count > num4)
							{
								rectangle = this.CachedGridRectangles[num4];
							}
							if (rectangle == null)
							{
								rectangle = new Rectangle();
								rectangle.Fill = this.InterlacedColor;
								this.Visual.Children.Add(rectangle);
								this.CachedGridRectangles.Add(rectangle);
							}
							if (animationEnabled)
							{
								ScaleTransform scaleTransform;
								if (num4 % 2 == 0)
								{
									scaleTransform = new ScaleTransform
									{
										ScaleX = 1.0,
										ScaleY = 0.0
									};
									rectangle.RenderTransformOrigin = new Point(0.5, 1.0);
									this.Storyboard.Children.Add(this.CreateDoubleAnimation(scaleTransform, "(ScaleTransform.ScaleY)", 0.0, 1.0, 0.5, animationDuration));
								}
								else
								{
									scaleTransform = new ScaleTransform
									{
										ScaleX = 1.0,
										ScaleY = 0.0
									};
									rectangle.RenderTransformOrigin = new Point(0.5, 0.0);
									this.Storyboard.Children.Add(this.CreateDoubleAnimation(scaleTransform, "(ScaleTransform.ScaleY)", 0.0, 1.0, 0.5, animationDuration));
								}
								rectangle.RenderTransform = scaleTransform;
							}
							else
							{
								rectangle.RenderTransform = null;
							}
							num4++;
							rectangle.Width = Math.Abs(num5 - num6);
							rectangle.Height = this.Height;
							rectangle.SetValue(Canvas.LeftProperty, num6);
							rectangle.SetValue(Canvas.TopProperty, 0.0);
							rectangle.SetValue(Panel.ZIndexProperty, -num4);
							this.InterlacedRectangles.Add(rectangle);
						}
						break;
					case PlacementTypes.Left:
					case PlacementTypes.Right:
						num5 = Graphics.ValueToPixelPosition(this.Height, 0.0, this.Minimum, this.Maximum, (double)num10);
						if (num5 == 0.0)
						{
							num5 += this.LineThickness.Value;
						}
						line.X1 = 0.0;
						line.X2 = this.Width;
						line.Y1 = num5;
						line.Y2 = num5;
						if (num2 % 2 == 1)
						{
							Rectangle rectangle2 = null;
							if (this.CachedGridRectangles.Count > num4)
							{
								rectangle2 = this.CachedGridRectangles[num4];
							}
							if (rectangle2 == null)
							{
								rectangle2 = new Rectangle();
								rectangle2.Fill = this.InterlacedColor;
								this.Visual.Children.Add(rectangle2);
								this.CachedGridRectangles.Add(rectangle2);
							}
							if (animationEnabled)
							{
								ScaleTransform scaleTransform2;
								if (num4 % 2 == 0)
								{
									scaleTransform2 = new ScaleTransform
									{
										ScaleX = 0.0,
										ScaleY = 1.0
									};
									rectangle2.RenderTransformOrigin = new Point(0.0, 0.5);
									this.Storyboard.Children.Add(this.CreateDoubleAnimation(scaleTransform2, "(ScaleTransform.ScaleX)", 0.0, 1.0, 0.5, animationDuration));
								}
								else
								{
									scaleTransform2 = new ScaleTransform
									{
										ScaleX = 0.0,
										ScaleY = 1.0
									};
									rectangle2.RenderTransformOrigin = new Point(1.0, 0.5);
									this.Storyboard.Children.Add(this.CreateDoubleAnimation(scaleTransform2, "(ScaleTransform.ScaleX)", 0.0, 1.0, 0.5, animationDuration));
								}
								rectangle2.RenderTransform = scaleTransform2;
							}
							else
							{
								rectangle2.RenderTransform = null;
							}
							num4++;
							rectangle2.Width = this.Width;
							rectangle2.Height = Math.Abs(num5 - num6);
							rectangle2.SetValue(Canvas.LeftProperty, 0.0);
							rectangle2.SetValue(Canvas.TopProperty, num5);
							rectangle2.SetValue(Panel.ZIndexProperty, -num4);
							this.InterlacedRectangles.Add(rectangle2);
						}
						break;
					}
					num6 = num5;
					d += this.ParentAxis.SkipOffset + 1;
					num2++;
					if (this.ParentAxis.IsDateTimeAxis)
					{
						DateTime dateTime2 = DateTimeHelper.UpdateDate(this.ParentAxis.FirstLabelDate, (double)(d * d3), this.ParentAxis.InternalIntervalType);
						decimal d5 = (decimal)DateTimeHelper.DateDiff(dateTime2, this.ParentAxis.FirstLabelDate, this.ParentAxis.MinDateRange, this.ParentAxis.MaxDateRange, this.ParentAxis.InternalIntervalType, this.ParentAxis.XValueType);
						num10 = Math.Round(num3 + d5, 7);
					}
					else
					{
						num10 = Math.Round(num3 + d * d3, 7);
					}
				}
				if (num2 % 2 == 1)
				{
					Rectangle rectangle3 = null;
					if (this.CachedGridRectangles.Count > num4)
					{
						rectangle3 = this.CachedGridRectangles[num4];
					}
					if (rectangle3 == null)
					{
						rectangle3 = new Rectangle();
						rectangle3.Fill = this.InterlacedColor;
						this.Visual.Children.Add(rectangle3);
						this.CachedGridRectangles.Add(rectangle3);
					}
					switch (this.Placement)
					{
					case PlacementTypes.Top:
					case PlacementTypes.Bottom:
						rectangle3.Width = Math.Abs(this.Width - num5);
						rectangle3.Height = this.Height;
						if (animationEnabled)
						{
							ScaleTransform scaleTransform3;
							if (num4 % 2 == 0)
							{
								scaleTransform3 = new ScaleTransform
								{
									ScaleX = 1.0,
									ScaleY = 0.0
								};
								rectangle3.RenderTransformOrigin = new Point(0.5, 1.0);
								this.Storyboard.Children.Add(this.CreateDoubleAnimation(scaleTransform3, "(ScaleTransform.ScaleY)", 0.0, 1.0, 0.5, animationDuration));
							}
							else
							{
								scaleTransform3 = new ScaleTransform
								{
									ScaleX = 1.0,
									ScaleY = 0.0
								};
								rectangle3.RenderTransformOrigin = new Point(0.5, 0.0);
								this.Storyboard.Children.Add(this.CreateDoubleAnimation(scaleTransform3, "(ScaleTransform.ScaleY)", 0.0, 1.0, 0.5, animationDuration));
							}
							rectangle3.RenderTransform = scaleTransform3;
						}
						else
						{
							rectangle3.RenderTransform = null;
						}
						rectangle3.SetValue(Canvas.LeftProperty, num5);
						rectangle3.SetValue(Canvas.TopProperty, 0.0);
						break;
					case PlacementTypes.Left:
					case PlacementTypes.Right:
						rectangle3.Width = this.Width;
						rectangle3.Height = Math.Abs(num5);
						if (animationEnabled)
						{
							ScaleTransform scaleTransform3;
							if (num4 % 2 == 0)
							{
								scaleTransform3 = new ScaleTransform
								{
									ScaleX = 0.0,
									ScaleY = 1.0
								};
								rectangle3.RenderTransformOrigin = new Point(0.0, 0.5);
								this.Storyboard.Children.Add(this.CreateDoubleAnimation(scaleTransform3, "(ScaleTransform.ScaleX)", 0.0, 1.0, 0.5, animationDuration));
							}
							else
							{
								scaleTransform3 = new ScaleTransform
								{
									ScaleX = 0.0,
									ScaleY = 1.0
								};
								rectangle3.RenderTransformOrigin = new Point(1.0, 0.5);
								this.Storyboard.Children.Add(this.CreateDoubleAnimation(scaleTransform3, "(ScaleTransform.ScaleX)", 0.0, 1.0, 0.5, animationDuration));
							}
							rectangle3.RenderTransform = scaleTransform3;
						}
						else
						{
							rectangle3.RenderTransform = null;
						}
						rectangle3.SetValue(Canvas.LeftProperty, 0.0);
						rectangle3.SetValue(Canvas.TopProperty, 0.0);
						break;
					}
					num4++;
					rectangle3.SetValue(Panel.ZIndexProperty, -num4);
					this.InterlacedRectangles.Add(rectangle3);
				}
			}
			for (int i = num9; i < this.CachedGridLines.Count; i++)
			{
				list2.Add(this.CachedGridLines[i]);
			}
			for (int j = num4; j < this.CachedGridRectangles.Count; j++)
			{
				list3.Add(this.CachedGridRectangles[j]);
			}
			this.CachedGridLines.RemoveRange(num9, list2.Count);
			this.CachedGridRectangles.RemoveRange(num4, list3.Count);
			foreach (Line current in list2)
			{
				this.Visual.Children.Remove(current);
			}
			foreach (Rectangle current2 in list3)
			{
				this.Visual.Children.Remove(current2);
			}
			this.Visual.Width = this.Width;
			this.Visual.Height = this.Height;
		}

		internal DoubleAnimationUsingKeyFrames CreateDoubleAnimation(DependencyObject target, string property, double from, double to, double begin, double duration)
		{
			DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
			doubleAnimationUsingKeyFrames.BeginTime = new TimeSpan?(TimeSpan.FromSeconds(begin));
			target.SetValue(FrameworkElement.NameProperty, target.GetType().Name + Guid.NewGuid().ToString().Replace('-', '_'));
			Storyboard.SetTargetName(doubleAnimationUsingKeyFrames, target.GetValue(FrameworkElement.NameProperty).ToString());
			base.Chart._rootElement.RegisterName((string)target.GetValue(FrameworkElement.NameProperty), target);
			Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath(property, new object[0]));
			SplineDoubleKeyFrame splineDoubleKeyFrame = new SplineDoubleKeyFrame();
			splineDoubleKeyFrame.KeySpline = new KeySpline
			{
				ControlPoint1 = new Point(0.0, 0.0),
				ControlPoint2 = new Point(1.0, 1.0)
			};
			splineDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0));
			splineDoubleKeyFrame.Value = from;
			doubleAnimationUsingKeyFrames.KeyFrames.Add(splineDoubleKeyFrame);
			splineDoubleKeyFrame = new SplineDoubleKeyFrame();
			splineDoubleKeyFrame.KeySpline = new KeySpline
			{
				ControlPoint1 = new Point(0.5, 0.0),
				ControlPoint2 = new Point(0.5, 1.0)
			};
			splineDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration));
			splineDoubleKeyFrame.Value = to;
			doubleAnimationUsingKeyFrames.KeyFrames.Add(splineDoubleKeyFrame);
			return doubleAnimationUsingKeyFrames;
		}

		private static void OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChartGrid chartGrid = d as ChartGrid;
			chartGrid.FirePropertyChanged(VcProperties.Interval);
		}

		private static void OnEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChartGrid chartGrid = d as ChartGrid;
			chartGrid.FirePropertyChanged(VcProperties.Enabled);
		}

		private static void OnLineColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChartGrid chartGrid = d as ChartGrid;
			chartGrid.UpdateVisual(VcProperties.LineColor, e.NewValue);
		}

		private static void OnLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChartGrid chartGrid = d as ChartGrid;
			chartGrid.UpdateVisual(VcProperties.LineStyle, e.NewValue);
		}

		private static void OnLineThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChartGrid chartGrid = d as ChartGrid;
			chartGrid.UpdateVisual(VcProperties.LineThickness, e.NewValue);
		}

		private static void OnInterlacedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChartGrid chartGrid = d as ChartGrid;
			chartGrid.UpdateVisual(VcProperties.InterlacedColor, e.NewValue);
		}

		private static void OnOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChartGrid chartGrid = d as ChartGrid;
			chartGrid.InternalOpacity = (double)e.NewValue;
			chartGrid.FirePropertyChanged(VcProperties.Opacity);
		}

		internal void CleanUpGrid()
		{
			if (this.Visual != null)
			{
				this.Visual.Children.Clear();
				this.Visual = null;
			}
		}

		private List<Point> GetGridPoints4Radar(double radius, Point center, double spikesCount)
		{
			double num = -1.5707963267948966;
			double num2 = num;
			int num3 = 1;
			List<Point> list = new List<Point>();
			double num4 = 360.0 / spikesCount;
			int num5 = 0;
			while ((double)num5 < spikesCount)
			{
				double x = radius * Math.Cos(num2) + center.X;
				double y = radius * Math.Sin(num2) + center.Y;
				list.Add(new Point(x, y));
				double angle = num4 * (double)num3++;
				num2 = AxisLabel.GetRadians(angle) - 1.5707963267948966;
				num5++;
			}
			return list;
		}

		private List<Point> GetGridPoints4Polar(double radius, Point center, List<double> angles)
		{
			List<Point> list = new List<Point>();
			for (int i = 0; i < angles.Count; i++)
			{
				double x = radius * Math.Cos(angles[i]) + center.X;
				double y = radius * Math.Sin(angles[i]) + center.Y;
				list.Add(new Point(x, y));
			}
			return list;
		}

		internal void CreateAndPositionChartGrid4CircularAxesY()
		{
			double num = this.Interval.Value;
			decimal d = 0m;
			decimal num2 = (decimal)this.Minimum;
			decimal d2 = (decimal)this.Maximum;
			decimal d3 = (decimal)num;
			int num3 = 0;
			this.InterlacedPaths = new List<Path>();
			this.InterlacedLines = new List<Line>();
			List<Point> list = new List<Point>();
			List<Point> list2 = new List<Point>();
			CircularPlotDetails circularPlotDetails = this.ParentAxis.CircularPlotDetails;
			if (num2 != d2)
			{
				decimal num4 = num2;
				while (num4 <= d2)
				{
					double num5 = Graphics.ValueToPixelPosition(this.Height, 0.0, this.Minimum, this.Maximum, (double)num4);
					double radius = this.Height - num5;
					List<Point> list3;
					if (circularPlotDetails.CircularChartType == RenderAs.Radar)
					{
						list3 = this.GetGridPoints4Radar(radius, circularPlotDetails.Center, (double)circularPlotDetails.ListOfPoints4CircularAxis.Count);
					}
					else
					{
						list3 = this.GetGridPoints4Polar(radius, circularPlotDetails.Center, circularPlotDetails.AnglesInRadian);
					}
					list2 = list3;
					int i;
					for (i = 0; i < list3.Count - 1; i++)
					{
						Line line = new Line();
						this.InterlacedLines.Add(line);
						line.Stroke = this.LineColor;
						line.StrokeThickness = this.LineThickness.Value;
						line.StrokeDashArray = ExtendedGraphics.GetDashArray(this.LineStyle);
						line.X1 = list3[i].X;
						line.X2 = list3[i + 1].X;
						line.Y1 = list3[i].Y;
						line.Y2 = list3[i + 1].Y;
						this.Visual.Children.Add(line);
					}
					Line line2 = new Line();
					this.InterlacedLines.Add(line2);
					line2.Stroke = this.LineColor;
					line2.StrokeThickness = this.LineThickness.Value;
					line2.StrokeDashArray = ExtendedGraphics.GetDashArray(this.LineStyle);
					line2.X1 = list3[i].X;
					line2.X2 = list3[0].X;
					line2.Y1 = list3[i].Y;
					line2.Y2 = list3[0].Y;
					this.Visual.Children.Add(line2);
					if (d % 2m == 1m)
					{
						Path path = new Path();
						path.StrokeThickness = 0.0;
						List<Point> list4 = new List<Point>();
						for (int j = 0; j < list2.Count; j++)
						{
							list4.Add(list2[j]);
						}
						list4.Add(list2[0]);
						for (int k = 0; k < list.Count; k++)
						{
							list4.Add(list[k]);
						}
						list4.Add(list[0]);
						list4.Add(list2[0]);
						path.Data = this.ParentAxis.GetPathGeometry(list4);
						num3++;
						path.Fill = this.InterlacedColor;
						path.SetValue(Panel.ZIndexProperty, 10);
						this.Visual.Children.Add(path);
						this.InterlacedPaths.Add(path);
					}
					list = list2;
					d += this.ParentAxis.SkipOffset + 1;
					num4 = num2 + d * d3;
				}
			}
		}

		internal void CreateVisualObject(double width, double height, bool animationEnabled, double animationDuration)
		{
			animationEnabled = (animationEnabled && this.AnimationEnabled);
			if (!this.Enabled.Value || (double.IsNaN(width) && double.IsNaN(height)))
			{
				this.Visual = null;
				return;
			}
			if (this.Visual == null)
			{
				this.Visual = new Canvas();
				this.CachedGridLines = new List<Line>();
				this.CachedGridRectangles = new List<Rectangle>();
			}
			else if ((base.Chart as Chart).PlotDetails.ChartOrientation != this.OldChartOrientation)
			{
				this.Visual.Children.Clear();
				this.CachedGridLines.Clear();
				this.CachedGridRectangles.Clear();
			}
			this.Width = width;
			this.Height = height;
			if (animationEnabled && (base.Chart as Chart).PlotDetails.ChartOrientation != ChartOrientationType.Circular)
			{
				this.Storyboard = new Storyboard();
				ScaleTransform scaleTransform = new ScaleTransform
				{
					ScaleX = 1.0,
					ScaleY = 1.0
				};
				this.Visual.RenderTransformOrigin = new Point(0.5, 0.5);
				this.Visual.RenderTransform = scaleTransform;
				if (this.Placement == PlacementTypes.Top || this.Placement == PlacementTypes.Bottom)
				{
					this.Storyboard.Children.Add(this.CreateDoubleAnimation(scaleTransform, "(ScaleTransform.ScaleY)", 0.0, 1.0, 0.0, animationDuration));
				}
				else
				{
					this.Storyboard.Children.Add(this.CreateDoubleAnimation(scaleTransform, "(ScaleTransform.ScaleX)", 0.0, 1.0, 0.0, animationDuration));
				}
			}
			if ((base.Chart as Chart).PlotDetails.ChartOrientation == ChartOrientationType.Circular)
			{
				this.CreateAndPositionChartGrid4CircularAxesY();
			}
			else
			{
				this.CreateAndPositionChartGrid(animationEnabled, animationDuration);
				PlotArea plotArea = (base.Chart as Chart).PlotArea;
				this.Visual.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				Size size = new Size(this.Visual.DesiredSize.Width, this.Visual.DesiredSize.Height);
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = new Rect(plotArea.BorderThickness.Left, plotArea.BorderThickness.Top, size.Width - plotArea.BorderThickness.Left - plotArea.BorderThickness.Right, size.Height - plotArea.BorderThickness.Top - plotArea.BorderThickness.Bottom);
				rectangleGeometry.RadiusX = plotArea.CornerRadius.TopRight;
				rectangleGeometry.RadiusY = plotArea.CornerRadius.TopRight;
				this.Visual.Clip = rectangleGeometry;
			}
			this.Visual.Opacity = this.Opacity;
			this.OldChartOrientation = (base.Chart as Chart).PlotDetails.ChartOrientation;
		}

		internal override void UpdateVisual(VcProperties propertyName, object value)
		{
			if (this.Visual != null)
			{
				if (this.InterlacedRectangles != null)
				{
					foreach (Rectangle current in this.InterlacedRectangles)
					{
						current.Fill = this.InterlacedColor;
					}
				}
				if (this.InterlacedPaths != null)
				{
					foreach (Path current2 in this.InterlacedPaths)
					{
						current2.Fill = this.InterlacedColor;
					}
				}
				if (this.InterlacedLines != null)
				{
					foreach (Line current3 in this.InterlacedLines)
					{
						current3.Stroke = this.LineColor;
						current3.StrokeThickness = this.LineThickness.Value;
						current3.StrokeDashArray = ExtendedGraphics.GetDashArray(this.LineStyle);
					}
				}
				Chart chart = base.Chart as Chart;
				if (base.Chart == null || this.ParentAxis == null || chart.ChartArea == null || this.ParentAxis.AxisRepresentation != AxisRepresentations.AxisY || this.ParentAxis.AxisType != AxisTypes.Primary)
				{
					return;
				}
				List<Line> interlacedLinesOverVerticalPlank = chart.ChartArea.InterlacedLinesOverVerticalPlank;
				if (interlacedLinesOverVerticalPlank != null)
				{
					foreach (Line current4 in interlacedLinesOverVerticalPlank)
					{
						current4.Stroke = this.LineColor;
						current4.StrokeThickness = this.LineThickness.Value;
						current4.StrokeDashArray = ExtendedGraphics.GetDashArray(this.LineStyle);
					}
				}
				List<Path> interlacedPathsOverVerticalPlank = chart.ChartArea.InterlacedPathsOverVerticalPlank;
				if (interlacedPathsOverVerticalPlank == null)
				{
					return;
				}
				using (List<Path>.Enumerator enumerator5 = interlacedPathsOverVerticalPlank.GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						Path current5 = enumerator5.Current;
						current5.Fill = this.InterlacedColor;
					}
					return;
				}
			}
			base.FirePropertyChanged(propertyName);
		}
	}
}
