using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class PieChart
	{
		internal class PathGeometryParams
		{
			public Point EndPoint
			{
				get;
				set;
			}

			public PathGeometryParams(Point endPoint)
			{
				this.EndPoint = endPoint;
			}
		}

		internal class LineSegmentParams : PieChart.PathGeometryParams
		{
			public LineSegmentParams(Point endPoint) : base(endPoint)
			{
			}
		}

		internal class ArcSegmentParams : PieChart.PathGeometryParams
		{
			public Size Size
			{
				get;
				set;
			}

			public double RotationAngle
			{
				get;
				set;
			}

			public bool IsLargeArc
			{
				get;
				set;
			}

			public SweepDirection SweepDirection
			{
				get;
				set;
			}

			public ArcSegmentParams(Size size, double rotation, bool isLargeArc, SweepDirection sweepDirection, Point endPoint) : base(endPoint)
			{
				this.Size = size;
				this.RotationAngle = rotation;
				this.IsLargeArc = isLargeArc;
				this.SweepDirection = sweepDirection;
			}
		}

		public const double LABEL_LINE_LENGTH = 18.0;

		private static List<ElementPositionData> _elementPositionData;

		private static double FixAngle(double angle)
		{
			while (angle > 6.2831853071795862)
			{
				angle -= 6.2831853071795862;
			}
			while (angle < 0.0)
			{
				angle += 6.2831853071795862;
			}
			return angle;
		}

		private static Canvas CreateLabel(IDataPoint dataPoint)
		{
			LabelStyles labelStyle = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			Brush labelFontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
			FontStyle fontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
			Brush background = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
			FontFamily fontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
			double fontSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontSize);
			FontWeight fontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
			TextBlock textBlock = new TextBlock
			{
				FlowDirection = ((dataPoint.Chart != null) ? dataPoint.Chart.FlowDirection : FlowDirection.LeftToRight),
				FontFamily = fontFamily,
				FontSize = fontSize,
				FontStyle = fontStyle,
				FontWeight = fontWeight,
				Foreground = Chart.CalculateDataPointLabelFontColor(dataPoint.Chart as Chart, dataPoint, labelFontColor, labelStyle),
				Text = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText)),
				Tag = new ElementData
				{
					Element = dataPoint
				}
			};
			Size size = Graphics.CalculateVisualSize(textBlock);
			if (size.Width == 0.0 && size.Width == 0.0)
			{
				size.Width = textBlock.ActualWidth;
				size.Height = textBlock.ActualHeight;
			}
			Canvas canvas = new Canvas
			{
				Height = size.Height,
				Width = size.Width,
				Background = background,
				Tag = new ElementData
				{
					Element = dataPoint
				}
			};
			if (dataPoint.IsLightDataPoint)
			{
				canvas.Opacity = dataPoint.Parent.InternalOpacity;
			}
			else
			{
				canvas.Opacity = (dataPoint as DataPoint).InternalOpacity * dataPoint.Parent.InternalOpacity;
			}
			canvas.Children.Add(textBlock);
			dataPoint.DpInfo.LabelVisual = canvas;
			return canvas;
		}

		private static void PositionLabels(Canvas visual, double totalSum, List<IDataPoint> dataPoints, Size pieSize, Size referenceEllipseSize, Size visualCanvasSize, double scaleY, bool is3D)
		{
			double xRadiusLabel = referenceEllipseSize.Width / 2.0;
			double yRadiusLabel = referenceEllipseSize.Height / 2.0;
			double xRadiusChart = pieSize.Width / 2.0;
			double yRadiusChart = pieSize.Height / 2.0;
			Chart chart = null;
			Point center = new Point(visualCanvasSize.Width / 2.0, visualCanvasSize.Height / 2.0);
			double num = PieChart.FixAngle(dataPoints[0].Parent.InternalStartAngle);
			CircularLabel circularLabel = null;
			CircularLabel circularLabel2 = null;
			Rect rect = new Rect(0.0, 0.0, visual.Width, visual.Height);
			List<CircularLabel> list = new List<CircularLabel>();
			foreach (IDataPoint current in dataPoints)
			{
				chart = (current.Chart as Chart);
				double num2 = num + 6.2831853071795862 * Math.Abs(current.DpInfo.InternalYValue) / totalSum;
				double meanAngle = (num + num2) / 2.0;
				if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LabelStyle) == LabelStyles.Inside)
				{
					meanAngle = CircularLabel.ResetMeanAngle(meanAngle);
					PieChart.PlaceLabelInside(current, center, meanAngle, pieSize, current.DpInfo.ClippedWidth4Pie, referenceEllipseSize, scaleY, is3D);
					num = num2;
				}
				else
				{
					circularLabel2 = new CircularLabel(current.DpInfo.LabelVisual, center, meanAngle, xRadiusLabel, yRadiusLabel, xRadiusChart, yRadiusChart, visual);
					circularLabel2.Boundary = rect;
					if (circularLabel != null)
					{
						circularLabel.NextLabel = circularLabel2;
					}
					circularLabel2.PreviusLabel = circularLabel;
					circularLabel = circularLabel2;
					list.Add(circularLabel2);
					num = num2;
				}
			}
			if (list.Count > 0)
			{
				list[0].PreviusLabel = circularLabel2;
				circularLabel2.NextLabel = list[0];
				LabelPlacementHelper.CircularLabelPlacment(rect, list, chart != null && chart.SmartLabelEnabled);
			}
		}

		private static void RearrangeLabels(List<IDataPoint> dataPoints, double leftOfArea, double topOfArea, double areaHeight, double areaWidth)
		{
			Rect[] array = new Rect[dataPoints.Count];
			int num = 0;
			foreach (IDataPoint current in dataPoints)
			{
				if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LabelStyle) == LabelStyles.OutSide)
				{
					double x = (double)current.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
					double y = (double)current.DpInfo.LabelVisual.GetValue(Canvas.TopProperty);
					array[num++] = new Rect(x, y, current.DpInfo.LabelVisual.Width, current.DpInfo.LabelVisual.Height);
				}
			}
			LabelPlacementHelper.VerticalLabelPlacement(new Rect(leftOfArea, topOfArea, areaWidth, areaHeight), ref array);
			num = 0;
			foreach (IDataPoint current2 in dataPoints)
			{
				if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(current2, VcProperties.LabelStyle) == LabelStyles.OutSide)
				{
					current2.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, array[num].Left);
					current2.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, array[num].Top + topOfArea);
					num++;
				}
			}
		}

		private static void PlaceLabelInside(IDataPoint dataPoint, Point center, double meanAngle, Size pieSize, double clippedWidth, Size referenceEllipseSize, double scaleY, bool is3D)
		{
			double num = pieSize.Width / (double)(is3D ? 1 : 2) * 0.7;
			double num2 = pieSize.Height / (double)(is3D ? 1 : 2) * 0.7 * scaleY;
			double num3 = Math.Min(pieSize.Width, pieSize.Height) / (double)(is3D ? 1 : 2);
			double num4 = -10.0;
			RenderAs renderAs = dataPoint.Parent.RenderAs;
			Point point = Graphics.MidPointOfALine(new Point
			{
				X = center.X + 1.7 * (num3 / 1.8) * Math.Cos(meanAngle),
				Y = center.Y + 1.7 * (num3 / 1.8) * Math.Sin(meanAngle)
			}, center);
			Point point2 = default(Point);
			point2.X = center.X + 1.8 * (num3 / 2.0) * Math.Cos(meanAngle);
			point2.Y = center.Y + num4 + 1.8 * (num3 / 2.0) * Math.Sin(meanAngle) * scaleY;
			Point point3 = Graphics.MidPointOfALine(new Point
			{
				X = center.X,
				Y = center.Y + num4
			}, point2);
			double num5;
			double num6;
			if (is3D)
			{
				num5 = center.X + num * Math.Cos(meanAngle) - clippedWidth;
				num6 = center.Y + num2 * Math.Sin(meanAngle);
			}
			else if (renderAs == RenderAs.Doughnut)
			{
				num5 = center.X + 1.8 * (num3 / 3.0) * Math.Cos(meanAngle) - clippedWidth;
				num6 = center.Y + 1.8 * (num3 / 3.0) * Math.Sin(meanAngle);
			}
			else
			{
				num5 = center.X + 1.7 * (num3 / 3.0) * Math.Cos(meanAngle) - clippedWidth;
				num6 = center.Y + 1.7 * (num3 / 3.0) * Math.Sin(meanAngle);
			}
			if (renderAs == RenderAs.Doughnut)
			{
				num5 -= dataPoint.DpInfo.LabelVisual.Width / 2.0;
				num6 -= dataPoint.DpInfo.LabelVisual.Height / 2.0;
			}
			else if (is3D)
			{
				num5 = point3.X - dataPoint.DpInfo.LabelVisual.Width / 2.0;
				num6 = point3.Y - dataPoint.DpInfo.LabelVisual.Height / 2.0;
			}
			else
			{
				num5 = point.X - dataPoint.DpInfo.LabelVisual.Width / 2.0;
				num6 = point.Y - dataPoint.DpInfo.LabelVisual.Height / 2.0;
			}
			dataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, num6);
			dataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, num5);
		}

		private static int ComparePointY(Point a, Point b)
		{
			return a.Y.CompareTo(b.Y);
		}

		private static int ComparePointX(Point a, Point b)
		{
			return a.X.CompareTo(b.X);
		}

		private static int CompareRectY(Rect a, Rect b)
		{
			return a.Y.CompareTo(b.Y);
		}

		private static double LineSlope(Point point1, Point point2)
		{
			return (point2.Y - point1.Y) / (point2.X - point1.X);
		}

		private static Canvas CreateAndPositionLabels(double totalSum, List<IDataPoint> dataPoints, double width, double height, double scaleY, bool is3D, ref Size size)
		{
			Canvas canvas = new Canvas
			{
				Height = height,
				Width = width
			};
			List<IDataPoint> list = new List<IDataPoint>();
			double num = 18.0;
			bool flag = false;
			bool flag2 = false;
			double num2 = 0.0;
			double val = 0.0;
			foreach (IDataPoint current in dataPoints)
			{
				Canvas canvas2 = PieChart.CreateLabel(current);
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LabelEnabled))
				{
					num2 = Math.Max(num2, canvas2.Width);
					val = Math.Max(val, canvas2.Height);
					flag = true;
					if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LabelStyle) == LabelStyles.OutSide)
					{
						flag2 = true;
					}
				}
				else
				{
					canvas2.Visibility = Visibility.Collapsed;
				}
				list.Add(current);
				if (flag)
				{
					canvas.Children.Add(canvas2);
				}
				current.DpInfo.ClippedWidth4Pie = 0.0;
			}
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			double value = 0.0;
			double num6 = (width - num2) * scaleY;
			if (num6 > height)
			{
				num6 = Math.Min(num6, height);
			}
			if (flag)
			{
				if (flag2)
				{
					if (is3D)
					{
						num3 = num6 - num * 2.0;
						if (num3 < Math.Min(width, height) / 4.0)
						{
							num3 = Math.Min(width, height) / 4.0;
						}
						num4 = num3;
						num5 = num3 * 2.0 + num * 2.0;
						value = num5 * scaleY + num * 2.0;
					}
					else
					{
						if (num2 > num)
						{
							num3 = num6 - num * 2.5;
						}
						else
						{
							num3 = num6 - num2 - 18.0;
						}
						if (num3 < Math.Min(width, height) / 4.0)
						{
							num3 = Math.Min(width, height) / 4.0;
						}
						num4 = Math.Abs(num3);
						num5 = Math.Abs(num3) + 18.0;
						value = num5;
					}
					PieChart.PositionLabels(canvas, totalSum, dataPoints, new Size(Math.Abs(num3), Math.Abs(num4)), new Size(Math.Abs(num5), Math.Abs(value)), new Size(width, height), scaleY, is3D);
				}
				else
				{
					num6 = (width - 10.0) * scaleY;
					if (width - 10.0 > height)
					{
						num6 = Math.Min(num6, height);
					}
					num3 = num6;
					num4 = num6;
					num5 = num3;
					value = num4;
					PieChart.PositionLabels(canvas, totalSum, dataPoints, new Size(Math.Abs(num3), Math.Abs(num4)), new Size(Math.Abs(num5), Math.Abs(value)), new Size(width, height), scaleY, is3D);
					List<IDataPoint> list2 = new List<IDataPoint>();
					foreach (IDataPoint current2 in dataPoints)
					{
						double num7 = (double)current2.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
						if (num7 < 0.0 || num7 + current2.DpInfo.LabelVisual.Width > width)
						{
							list2.Add(current2);
						}
					}
					if (list2.Count > 0)
					{
						double val2 = -1.7976931348623157E+308;
						foreach (IDataPoint current3 in list2)
						{
							double num8 = (double)current3.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
							if (num8 < 0.0)
							{
								current3.DpInfo.ClippedWidth4Pie = num8;
							}
							else
							{
								current3.DpInfo.ClippedWidth4Pie = num8 + current3.DpInfo.LabelVisual.Width - width;
							}
							val2 = Math.Max(val2, Math.Abs(current3.DpInfo.ClippedWidth4Pie));
						}
						PieChart.PositionLabels(canvas, totalSum, dataPoints, new Size(Math.Abs(num3), Math.Abs(num4)), new Size(Math.Abs(num5), Math.Abs(value)), new Size(width, height), scaleY, is3D);
					}
				}
			}
			else
			{
				num3 = num6;
				num4 = num6;
			}
			num3 -= ((num3 > 10.0) ? (num3 * 0.1) : 0.0);
			num4 -= ((num4 > 10.0) ? (num3 * 0.1) : 0.0);
			size = new Size(Math.Abs(num3), Math.Abs(num4));
			if (flag)
			{
				return canvas;
			}
			return null;
		}

		private static Canvas GetPie2D(DataSeries currentDataSeries, ref Faces faces, SectorChartShapeParams pieParams, ref PieDoughnut2DPoints unExplodedPoints, ref PieDoughnut2DPoints explodedPoints, ref Path labelLinePath, List<IDataPoint> enabledDataPoints)
		{
			IEnumerable<IDataPoint> source = from dp in enabledDataPoints
			where dp.DpInfo.InternalYValue != 0.0
			select dp;
			Canvas canvas = new Canvas();
			double num = pieParams.OuterRadius * 2.0;
			double num2 = pieParams.OuterRadius * 2.0;
			canvas.Width = num;
			canvas.Height = num2;
			Point point = new Point(num / 2.0, num2 / 2.0);
			double num3 = pieParams.OuterRadius * pieParams.ExplodeRatio * Math.Cos(pieParams.MeanAngle);
			double num4 = pieParams.OuterRadius * pieParams.ExplodeRatio * Math.Sin(pieParams.MeanAngle);
			if (pieParams.StartAngle != pieParams.StopAngle || !pieParams.IsZero)
			{
				Ellipse ellipse = new Ellipse
				{
					Tag = new ElementData
					{
						Element = pieParams.TagReference
					}
				};
				ellipse.Width = num;
				ellipse.Height = num2;
				ellipse.Fill = (pieParams.Lighting ? Graphics.GetLightingEnabledBrush(pieParams.Background, "Radial", new double[]
				{
					0.99,
					0.745
				}) : pieParams.Background);
				Point point2 = default(Point);
				Point point3 = default(Point);
				Point point4 = default(Point);
				point2.X = point.X + pieParams.OuterRadius * Math.Cos(pieParams.StartAngle);
				point2.Y = point.Y + pieParams.OuterRadius * Math.Sin(pieParams.StartAngle);
				point3.X = point.X + pieParams.OuterRadius * Math.Cos(pieParams.StopAngle);
				point3.Y = point.Y + pieParams.OuterRadius * Math.Sin(pieParams.StopAngle);
				point4.X = point.X + pieParams.OuterRadius * Math.Cos(pieParams.MeanAngle);
				point4.Y = point.Y + pieParams.OuterRadius * Math.Sin(pieParams.MeanAngle);
				List<PieChart.PathGeometryParams> list = new List<PieChart.PathGeometryParams>();
				list.Add(new PieChart.LineSegmentParams(point2));
				list.Add(new PieChart.ArcSegmentParams(new Size(pieParams.OuterRadius, pieParams.OuterRadius), 0.0, pieParams.StopAngle - pieParams.StartAngle > 3.1415926535897931, SweepDirection.Clockwise, pieParams.AnimationEnabled ? point2 : point4));
				list.Add(new PieChart.ArcSegmentParams(new Size(pieParams.OuterRadius, pieParams.OuterRadius), 0.0, pieParams.StopAngle - pieParams.StartAngle > 3.1415926535897931, SweepDirection.Clockwise, pieParams.AnimationEnabled ? point2 : point3));
				list.Add(new PieChart.LineSegmentParams(point));
				ellipse.Clip = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point, list, true);
				PathSegmentCollection segments = (ellipse.Clip as PathGeometry).Figures[0].Segments;
				if (pieParams.AnimationEnabled)
				{
					pieParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, segments[1], point, pieParams.OuterRadius, currentDataSeries.InternalStartAngle, pieParams.MeanAngle);
					pieParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, segments[2], point, pieParams.OuterRadius, currentDataSeries.InternalStartAngle, pieParams.StopAngle);
					pieParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, segments[0], point, pieParams.OuterRadius, currentDataSeries.InternalStartAngle, pieParams.StartAngle);
				}
				faces.Parts.Add(ellipse);
				canvas.Children.Add(ellipse);
				unExplodedPoints.Center = point;
				unExplodedPoints.OuterArcStart = point2;
				unExplodedPoints.OuterArcMid = point4;
				unExplodedPoints.OuterArcEnd = point3;
				explodedPoints.Center = new Point(point.X + num3, point.Y + num4);
				explodedPoints.OuterArcStart = new Point(point2.X + num3, point2.Y + num4);
				explodedPoints.OuterArcMid = new Point(point4.X + num3, point4.Y + num4);
				explodedPoints.OuterArcEnd = new Point(point3.X + num3, point3.Y + num4);
				if (enabledDataPoints.Count == 1 || source.Count<IDataPoint>() == 1)
				{
					Ellipse ellipse2 = new Ellipse
					{
						IsHitTestVisible = false,
						Height = ellipse.Height,
						Width = ellipse.Width
					};
					ellipse2.SetValue(Panel.ZIndexProperty, 10000);
					canvas.Children.Add(ellipse2);
					faces.BorderElements.Add(ellipse2);
				}
			}
			if ((pieParams.TagReference as IDataPoint).DpInfo.InternalYValue != 0.0 && pieParams.Lighting && (pieParams.StartAngle != pieParams.StopAngle || !pieParams.IsZero))
			{
				Ellipse ellipse3 = new Ellipse
				{
					Tag = new ElementData
					{
						Element = pieParams.TagReference
					}
				};
				ellipse3.Width = num;
				ellipse3.Height = num2;
				ellipse3.IsHitTestVisible = false;
				ellipse3.Fill = PieChart.GetPieGradianceBrush();
				Point point5 = default(Point);
				Point point6 = default(Point);
				Point point7 = default(Point);
				point5.X = point.X + pieParams.OuterRadius * Math.Cos(pieParams.StartAngle);
				point5.Y = point.Y + pieParams.OuterRadius * Math.Sin(pieParams.StartAngle);
				point6.X = point.X + pieParams.OuterRadius * Math.Cos(pieParams.StopAngle);
				point6.Y = point.Y + pieParams.OuterRadius * Math.Sin(pieParams.StopAngle);
				point7.X = point.X + pieParams.OuterRadius * Math.Cos(pieParams.MeanAngle);
				point7.Y = point.Y + pieParams.OuterRadius * Math.Sin(pieParams.MeanAngle);
				List<PieChart.PathGeometryParams> list2 = new List<PieChart.PathGeometryParams>();
				list2.Add(new PieChart.LineSegmentParams(point5));
				list2.Add(new PieChart.ArcSegmentParams(new Size(pieParams.OuterRadius, pieParams.OuterRadius), 0.0, pieParams.StopAngle - pieParams.StartAngle > 3.1415926535897931, SweepDirection.Clockwise, pieParams.AnimationEnabled ? point5 : point7));
				list2.Add(new PieChart.ArcSegmentParams(new Size(pieParams.OuterRadius, pieParams.OuterRadius), 0.0, pieParams.StopAngle - pieParams.StartAngle > 3.1415926535897931, SweepDirection.Clockwise, pieParams.AnimationEnabled ? point5 : point6));
				list2.Add(new PieChart.LineSegmentParams(point));
				ellipse3.Clip = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point, list2, true);
				PathSegmentCollection segments2 = (ellipse3.Clip as PathGeometry).Figures[0].Segments;
				if (pieParams.AnimationEnabled)
				{
					pieParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, segments2[1], point, pieParams.OuterRadius, currentDataSeries.InternalStartAngle, pieParams.MeanAngle);
					pieParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, segments2[2], point, pieParams.OuterRadius, currentDataSeries.InternalStartAngle, pieParams.StopAngle);
					pieParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, segments2[0], point, pieParams.OuterRadius, currentDataSeries.InternalStartAngle, pieParams.StartAngle);
				}
				canvas.Children.Add(ellipse3);
			}
			if (pieParams.LabelLineEnabled)
			{
				Path path = new Path
				{
					Tag = new ElementData
					{
						Element = pieParams.TagReference
					}
				};
				double meanAngle = pieParams.MeanAngle;
				path.SetValue(Panel.ZIndexProperty, -100000);
				Point point8 = default(Point);
				point8.X = point.X + pieParams.OuterRadius * Math.Cos(meanAngle);
				point8.Y = point.Y + pieParams.OuterRadius * Math.Sin(meanAngle);
				Point point9 = default(Point);
				point9.X = point.X + pieParams.LabelPoint.X - pieParams.Width / 2.0;
				point9.Y = point.Y + pieParams.LabelPoint.Y - pieParams.Height / 2.0;
				Point point10 = default(Point);
				if (pieParams.LabelLineTargetToRight)
				{
					point10.X = point9.X + 10.0;
				}
				else
				{
					point10.X = point9.X - 10.0;
				}
				point10.Y = point9.Y;
				List<PieChart.PathGeometryParams> list3 = new List<PieChart.PathGeometryParams>();
				list3.Add(new PieChart.LineSegmentParams(pieParams.AnimationEnabled ? point8 : point10));
				list3.Add(new PieChart.LineSegmentParams(pieParams.AnimationEnabled ? point8 : point9));
				path.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point8, list3, true);
				PathFigure pathFigure = (path.Data as PathGeometry).Figures[0];
				PathSegmentCollection segments3 = pathFigure.Segments;
				pathFigure.IsClosed = false;
				pathFigure.IsFilled = false;
				if (pieParams.AnimationEnabled)
				{
					pieParams.Storyboard = PieChart.CreateLabelLineAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, segments3[0], new Point[]
					{
						point8,
						point10
					});
					pieParams.Storyboard = PieChart.CreateLabelLineAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, segments3[1], new Point[]
					{
						point8,
						point10,
						point9
					});
				}
				path.Stroke = pieParams.LabelLineColor;
				path.StrokeDashArray = pieParams.LabelLineStyle;
				path.StrokeThickness = pieParams.LabelLineThickness;
				labelLinePath = path;
				if ((pieParams.TagReference as IDataPoint).DpInfo.InternalYValue == 0.0)
				{
					Line line = new Line();
					line.X1 = point.X;
					line.Y1 = point.Y;
					line.X2 = point8.X;
					line.Y2 = point8.Y;
					line.Stroke = pieParams.LabelLineColor;
					line.StrokeThickness = 0.25;
					line.IsHitTestVisible = false;
					canvas.Children.Add(line);
					if (pieParams.AnimationEnabled)
					{
						double beginTime = 2.0;
						pieParams.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, line, beginTime, line.Opacity, 0.5);
						line.Opacity = 0.0;
					}
				}
				canvas.Children.Add(path);
				unExplodedPoints.LabelLineEndPoint = point9;
				unExplodedPoints.LabelLineMidPoint = point10;
				unExplodedPoints.LabelLineStartPoint = point8;
				explodedPoints.LabelLineEndPoint = new Point(point9.X, point9.Y - num4);
				explodedPoints.LabelLineMidPoint = new Point(point10.X, point10.Y - num4);
				explodedPoints.LabelLineStartPoint = new Point(point8.X + num3, point8.Y + num4);
				if ((pieParams.TagReference as IDataPoint).DpInfo.InternalYValue == 0.0)
				{
					path.IsHitTestVisible = false;
				}
			}
			Point endPoint = default(Point);
			Point endPoint2 = default(Point);
			endPoint.X = point.X + pieParams.OuterRadius * Math.Cos(pieParams.StartAngle);
			endPoint.Y = point.Y + pieParams.OuterRadius * Math.Sin(pieParams.StartAngle);
			endPoint2.X = point.X + pieParams.OuterRadius * Math.Cos(pieParams.StopAngle);
			endPoint2.Y = point.Y + pieParams.OuterRadius * Math.Sin(pieParams.StopAngle);
			List<PieChart.PathGeometryParams> list4 = new List<PieChart.PathGeometryParams>();
			list4.Add(new PieChart.LineSegmentParams(point));
			list4.Add(new PieChart.LineSegmentParams(endPoint));
			list4.Add(new PieChart.ArcSegmentParams(new Size(pieParams.OuterRadius, pieParams.OuterRadius), 0.0, pieParams.IsLargerArc, SweepDirection.Clockwise, endPoint2));
			if ((pieParams.TagReference as IDataPoint).DpInfo.InternalYValue != 0.0)
			{
				Path path2 = new Path
				{
					IsHitTestVisible = false
				};
				path2.SetValue(Panel.ZIndexProperty, 10000);
				path2.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point, list4, true);
				canvas.Children.Add(path2);
				faces.BorderElements.Add(path2);
			}
			if (pieParams.Bevel && Math.Abs(pieParams.StartAngle - pieParams.StopAngle) > 0.03 && pieParams.StartAngle != pieParams.StopAngle)
			{
				Point point11 = default(Point);
				Point endPoint3 = default(Point);
				Point point12 = default(Point);
				double num5 = 4.0;
				double num6 = Math.Abs(pieParams.OuterRadius - num5);
				point11.X = point.X + num5 * Math.Cos(pieParams.MeanAngle);
				point11.Y = point.Y + num5 * Math.Sin(pieParams.MeanAngle);
				endPoint3.X = point.X + num6 * Math.Cos(pieParams.StartAngle + 0.03);
				endPoint3.Y = point.Y + num6 * Math.Sin(pieParams.StartAngle + 0.03);
				point12.X = point.X + num6 * Math.Cos(pieParams.StopAngle - 0.03);
				point12.Y = point.Y + num6 * Math.Sin(pieParams.StopAngle - 0.03);
				list4 = new List<PieChart.PathGeometryParams>();
				list4.Add(new PieChart.LineSegmentParams(point));
				list4.Add(new PieChart.LineSegmentParams(endPoint));
				list4.Add(new PieChart.LineSegmentParams(endPoint3));
				list4.Add(new PieChart.LineSegmentParams(point11));
				Path path3 = new Path
				{
					Tag = new ElementData
					{
						Element = pieParams.TagReference
					}
				};
				path3.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point11, list4, true);
				if (pieParams.StartAngle > 1.5707963267948966 && pieParams.StartAngle <= 4.71238898038469)
				{
					path3.Fill = PieChart.GetDarkerBevelBrush(pieParams.Background, pieParams.StartAngle * 180.0 / 3.1415926535897931 + 135.0);
				}
				else
				{
					path3.Fill = PieChart.GetLighterBevelBrush(pieParams.Background, -pieParams.StartAngle * 180.0 / 3.1415926535897931);
				}
				if (pieParams.AnimationEnabled)
				{
					double beginTime2 = 1.0;
					pieParams.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, path3, beginTime2, 1.0, 1.0);
					path3.Opacity = 0.0;
				}
				faces.Parts.Add(path3);
				canvas.Children.Add(path3);
				list4 = new List<PieChart.PathGeometryParams>();
				list4.Add(new PieChart.LineSegmentParams(point));
				list4.Add(new PieChart.LineSegmentParams(endPoint2));
				list4.Add(new PieChart.LineSegmentParams(point12));
				list4.Add(new PieChart.LineSegmentParams(point11));
				path3 = new Path
				{
					Tag = new ElementData
					{
						Element = pieParams.TagReference
					}
				};
				path3.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point11, list4, true);
				if (pieParams.StopAngle > 1.5707963267948966 && pieParams.StopAngle <= 4.71238898038469)
				{
					path3.Fill = PieChart.GetLighterBevelBrush(pieParams.Background, pieParams.StopAngle * 180.0 / 3.1415926535897931 + 135.0);
				}
				else
				{
					path3.Fill = PieChart.GetDarkerBevelBrush(pieParams.Background, -pieParams.StopAngle * 180.0 / 3.1415926535897931);
				}
				if (pieParams.AnimationEnabled)
				{
					double beginTime3 = 1.0;
					pieParams.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, path3, beginTime3, 1.0, 1.0);
					path3.Opacity = 0.0;
				}
				faces.Parts.Add(path3);
				canvas.Children.Add(path3);
				Shape shape;
				if (enabledDataPoints.Count == 1 || source.Count<IDataPoint>() == 1)
				{
					shape = new Ellipse
					{
						Height = pieParams.OuterRadius * 2.0,
						Width = pieParams.OuterRadius * 2.0,
						Tag = new ElementData
						{
							Element = pieParams.TagReference
						}
					};
					shape.Clip = new GeometryGroup
					{
						Children = 
						{
							new EllipseGeometry
							{
								Center = new Point(pieParams.OuterRadius, pieParams.OuterRadius),
								RadiusX = pieParams.OuterRadius,
								RadiusY = pieParams.OuterRadius
							},
							new EllipseGeometry
							{
								Center = new Point(pieParams.OuterRadius, pieParams.OuterRadius),
								RadiusX = num6,
								RadiusY = num6
							}
						}
					};
				}
				else
				{
					list4 = new List<PieChart.PathGeometryParams>();
					list4.Add(new PieChart.LineSegmentParams(endPoint2));
					list4.Add(new PieChart.ArcSegmentParams(new Size(pieParams.OuterRadius, pieParams.OuterRadius), 0.0, pieParams.IsLargerArc, SweepDirection.Counterclockwise, endPoint));
					list4.Add(new PieChart.LineSegmentParams(endPoint3));
					list4.Add(new PieChart.ArcSegmentParams(new Size(num6, num6), 0.0, pieParams.IsLargerArc, SweepDirection.Clockwise, point12));
					shape = new Path
					{
						Tag = new ElementData
						{
							Element = pieParams.TagReference
						}
					};
					(shape as Path).Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point12, list4, true);
				}
				if (pieParams.MeanAngle > 0.0 && pieParams.MeanAngle < 3.1415926535897931)
				{
					shape.Fill = PieChart.GetCurvedBevelBrush(pieParams.Background, pieParams.MeanAngle * 180.0 / 3.1415926535897931 + 90.0, Graphics.GenerateDoubleCollection(new double[]
					{
						-0.745,
						-0.85
					}), Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						1.0
					}));
				}
				else
				{
					shape.Fill = PieChart.GetCurvedBevelBrush(pieParams.Background, pieParams.MeanAngle * 180.0 / 3.1415926535897931 + 90.0, Graphics.GenerateDoubleCollection(new double[]
					{
						0.745,
						-0.99
					}), Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						1.0
					}));
				}
				if (pieParams.AnimationEnabled)
				{
					double beginTime4 = 1.0;
					pieParams.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, shape, beginTime4, 1.0, 1.0);
					shape.Opacity = 0.0;
				}
				faces.Parts.Add(shape);
				canvas.Children.Add(shape);
			}
			else
			{
				faces.Parts.Add(null);
				faces.Parts.Add(null);
				faces.Parts.Add(null);
			}
			return canvas;
		}

		private static Canvas GetDoughnut2D(DataSeries currentDataSeries, ref Faces faces, SectorChartShapeParams doughnutParams, ref PieDoughnut2DPoints unExplodedPoints, ref PieDoughnut2DPoints explodedPoints, ref Path labelLinePath, List<IDataPoint> enabledDataPoints)
		{
			IEnumerable<IDataPoint> source = from dp in enabledDataPoints
			where dp.DpInfo.InternalYValue != 0.0
			select dp;
			Canvas canvas = new Canvas
			{
				Tag = new ElementData
				{
					Element = doughnutParams.TagReference
				}
			};
			Canvas canvas2 = new Canvas
			{
				Tag = new ElementData
				{
					Element = doughnutParams.TagReference
				}
			};
			double num = doughnutParams.OuterRadius * 2.0;
			double num2 = doughnutParams.OuterRadius * 2.0;
			canvas.Width = num;
			canvas.Height = num2;
			canvas2.Width = num;
			canvas2.Height = num2;
			canvas.Children.Add(canvas2);
			Point center = new Point(num / 2.0, num2 / 2.0);
			double num3 = doughnutParams.OuterRadius * doughnutParams.ExplodeRatio * Math.Cos(doughnutParams.MeanAngle);
			double num4 = doughnutParams.OuterRadius * doughnutParams.ExplodeRatio * Math.Sin(doughnutParams.MeanAngle);
			if (doughnutParams.StartAngle != doughnutParams.StopAngle || !doughnutParams.IsZero)
			{
				Ellipse ellipse = new Ellipse
				{
					Tag = new ElementData
					{
						Element = doughnutParams.TagReference
					}
				};
				ellipse.Width = num;
				ellipse.Height = num2;
				ellipse.Fill = (doughnutParams.Lighting ? Graphics.GetLightingEnabledBrush(doughnutParams.Background, "Radial", new double[]
				{
					0.99,
					0.745
				}) : doughnutParams.Background);
				Point point = default(Point);
				Point point2 = default(Point);
				Point point3 = default(Point);
				Point point4 = default(Point);
				Point point5 = default(Point);
				Point point6 = default(Point);
				point.X = center.X + doughnutParams.OuterRadius * Math.Cos(doughnutParams.StartAngle);
				point.Y = center.Y + doughnutParams.OuterRadius * Math.Sin(doughnutParams.StartAngle);
				point2.X = center.X + doughnutParams.OuterRadius * Math.Cos(doughnutParams.StopAngle);
				point2.Y = center.Y + doughnutParams.OuterRadius * Math.Sin(doughnutParams.StopAngle);
				point3.X = center.X + doughnutParams.OuterRadius * Math.Cos(doughnutParams.MeanAngle);
				point3.Y = center.Y + doughnutParams.OuterRadius * Math.Sin(doughnutParams.MeanAngle);
				point4.X = center.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.StartAngle);
				point4.Y = center.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.StartAngle);
				point5.X = center.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.StopAngle);
				point5.Y = center.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.StopAngle);
				point6.X = center.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.MeanAngle);
				point6.Y = center.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.MeanAngle);
				List<PieChart.PathGeometryParams> list = new List<PieChart.PathGeometryParams>();
				list.Add(new PieChart.LineSegmentParams(point));
				list.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.OuterRadius, doughnutParams.OuterRadius), 0.0, doughnutParams.StopAngle - doughnutParams.StartAngle > 3.1415926535897931, SweepDirection.Clockwise, doughnutParams.AnimationEnabled ? point : point3));
				list.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.OuterRadius, doughnutParams.OuterRadius), 0.0, doughnutParams.StopAngle - doughnutParams.StartAngle > 3.1415926535897931, SweepDirection.Clockwise, doughnutParams.AnimationEnabled ? point : point2));
				list.Add(new PieChart.LineSegmentParams(doughnutParams.AnimationEnabled ? point4 : point5));
				list.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.InnerRadius, doughnutParams.InnerRadius), 0.0, doughnutParams.StopAngle - doughnutParams.StartAngle <= 3.1415926535897931 && false, SweepDirection.Counterclockwise, doughnutParams.AnimationEnabled ? point4 : point6));
				list.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.InnerRadius, doughnutParams.InnerRadius), 0.0, doughnutParams.StopAngle - doughnutParams.StartAngle <= 3.1415926535897931 && false, SweepDirection.Counterclockwise, doughnutParams.AnimationEnabled ? point4 : point4));
				ellipse.Clip = PieChart.GetPathGeometryFromList(FillRule.EvenOdd, point4, list, true);
				PathFigure pathFigure = (ellipse.Clip as PathGeometry).Figures[0];
				PathSegmentCollection segments = pathFigure.Segments;
				canvas2.Clip = new GeometryGroup
				{
					Children = 
					{
						new EllipseGeometry
						{
							Center = center,
							RadiusX = doughnutParams.OuterRadius,
							RadiusY = doughnutParams.OuterRadius
						},
						new EllipseGeometry
						{
							Center = center,
							RadiusX = doughnutParams.InnerRadius,
							RadiusY = doughnutParams.InnerRadius
						}
					}
				};
				List<PieChart.PathGeometryParams> list2 = new List<PieChart.PathGeometryParams>();
				list2.Add(new PieChart.LineSegmentParams(point2));
				list2.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.OuterRadius, doughnutParams.OuterRadius), 0.0, doughnutParams.IsLargerArc, SweepDirection.Counterclockwise, point));
				list2.Add(new PieChart.LineSegmentParams(point4));
				list2.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.InnerRadius, doughnutParams.InnerRadius), 0.0, doughnutParams.IsLargerArc, SweepDirection.Clockwise, point5));
				list2.Add(new PieChart.LineSegmentParams(point5));
				list2.Add(new PieChart.LineSegmentParams(point2));
				if ((doughnutParams.TagReference as IDataPoint).DpInfo.InternalYValue != 0.0)
				{
					Path path = new Path
					{
						IsHitTestVisible = false
					};
					path.SetValue(Panel.ZIndexProperty, 100000);
					path.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point2, list2, false);
					canvas.Children.Add(path);
					faces.BorderElements.Add(path);
				}
				if (doughnutParams.AnimationEnabled)
				{
					doughnutParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments[0], center, doughnutParams.OuterRadius, currentDataSeries.InternalStartAngle, doughnutParams.StartAngle);
					doughnutParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments[1], center, doughnutParams.OuterRadius, currentDataSeries.InternalStartAngle, doughnutParams.MeanAngle);
					doughnutParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments[2], center, doughnutParams.OuterRadius, currentDataSeries.InternalStartAngle, doughnutParams.StopAngle);
					doughnutParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments[3], center, doughnutParams.InnerRadius, currentDataSeries.InternalStartAngle, doughnutParams.StopAngle);
					doughnutParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments[4], center, doughnutParams.InnerRadius, currentDataSeries.InternalStartAngle, doughnutParams.MeanAngle);
					doughnutParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments[5], center, doughnutParams.InnerRadius, currentDataSeries.InternalStartAngle, doughnutParams.StartAngle);
					doughnutParams.Storyboard = PieChart.CreatePathFigureAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, pathFigure, center, doughnutParams.InnerRadius, currentDataSeries.InternalStartAngle, doughnutParams.StartAngle);
				}
				faces.Parts.Add(ellipse);
				canvas2.Children.Add(ellipse);
				unExplodedPoints.Center = center;
				unExplodedPoints.OuterArcStart = point;
				unExplodedPoints.OuterArcMid = point3;
				unExplodedPoints.OuterArcEnd = point2;
				unExplodedPoints.InnerArcStart = point4;
				unExplodedPoints.InnerArcMid = point6;
				unExplodedPoints.InnerArcEnd = point5;
				explodedPoints.Center = new Point(center.X + num3, center.Y + num4);
				explodedPoints.OuterArcStart = new Point(point.X + num3, point.Y + num4);
				explodedPoints.OuterArcMid = new Point(point3.X + num3, point3.Y + num4);
				explodedPoints.OuterArcEnd = new Point(point2.X + num3, point2.Y + num4);
				explodedPoints.InnerArcStart = new Point(point4.X + num3, point4.Y + num4);
				explodedPoints.InnerArcMid = new Point(point6.X + num3, point6.Y + num4);
				explodedPoints.InnerArcEnd = new Point(point5.X + num3, point5.Y + num4);
				if ((enabledDataPoints.Count == 1 || source.Count<IDataPoint>() == 1) && (doughnutParams.TagReference as IDataPoint).DpInfo.InternalYValue != 0.0)
				{
					Ellipse ellipse2 = new Ellipse
					{
						IsHitTestVisible = false,
						Height = ellipse.Height,
						Width = ellipse.Width
					};
					ellipse2.SetValue(Panel.ZIndexProperty, 10000);
					canvas.Children.Add(ellipse2);
					faces.BorderElements.Add(ellipse2);
					Ellipse ellipse3 = new Ellipse
					{
						IsHitTestVisible = false,
						Height = doughnutParams.InnerRadius * 2.0,
						Width = doughnutParams.InnerRadius * 2.0
					};
					ellipse3.SetValue(Panel.ZIndexProperty, 10000);
					ellipse3.SetValue(Canvas.TopProperty, ellipse3.Height / 2.0);
					ellipse3.SetValue(Canvas.LeftProperty, ellipse3.Width / 2.0);
					canvas.Children.Add(ellipse3);
					faces.BorderElements.Add(ellipse3);
				}
			}
			if (doughnutParams.Lighting)
			{
				Ellipse ellipse4 = new Ellipse
				{
					Tag = new ElementData
					{
						Element = doughnutParams.TagReference
					}
				};
				ellipse4.Width = num;
				ellipse4.Height = num2;
				ellipse4.IsHitTestVisible = false;
				ellipse4.Fill = PieChart.GetDoughnutGradianceBrush();
				if (doughnutParams.StartAngle != doughnutParams.StopAngle || !doughnutParams.IsZero)
				{
					Point point7 = default(Point);
					Point point8 = default(Point);
					Point point9 = default(Point);
					Point point10 = default(Point);
					Point point11 = default(Point);
					Point point12 = default(Point);
					point7.X = center.X + doughnutParams.OuterRadius * Math.Cos(doughnutParams.StartAngle);
					point7.Y = center.Y + doughnutParams.OuterRadius * Math.Sin(doughnutParams.StartAngle);
					point8.X = center.X + doughnutParams.OuterRadius * Math.Cos(doughnutParams.StopAngle);
					point8.Y = center.Y + doughnutParams.OuterRadius * Math.Sin(doughnutParams.StopAngle);
					point9.X = center.X + doughnutParams.OuterRadius * Math.Cos(doughnutParams.MeanAngle);
					point9.Y = center.Y + doughnutParams.OuterRadius * Math.Sin(doughnutParams.MeanAngle);
					point10.X = center.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.StartAngle);
					point10.Y = center.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.StartAngle);
					point11.X = center.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.StopAngle);
					point11.Y = center.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.StopAngle);
					point12.X = center.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.MeanAngle);
					point12.Y = center.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.MeanAngle);
					List<PieChart.PathGeometryParams> list3 = new List<PieChart.PathGeometryParams>();
					list3.Add(new PieChart.LineSegmentParams(point7));
					list3.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.OuterRadius, doughnutParams.OuterRadius), 0.0, doughnutParams.StopAngle - doughnutParams.StartAngle > 3.1415926535897931, SweepDirection.Clockwise, doughnutParams.AnimationEnabled ? point7 : point9));
					list3.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.OuterRadius, doughnutParams.OuterRadius), 0.0, doughnutParams.StopAngle - doughnutParams.StartAngle > 3.1415926535897931, SweepDirection.Clockwise, doughnutParams.AnimationEnabled ? point7 : point8));
					list3.Add(new PieChart.LineSegmentParams(doughnutParams.AnimationEnabled ? point10 : point11));
					list3.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.InnerRadius, doughnutParams.InnerRadius), 0.0, doughnutParams.StopAngle - doughnutParams.StartAngle <= 3.1415926535897931 && false, SweepDirection.Counterclockwise, doughnutParams.AnimationEnabled ? point10 : point12));
					list3.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.InnerRadius, doughnutParams.InnerRadius), 0.0, doughnutParams.StopAngle - doughnutParams.StartAngle <= 3.1415926535897931 && false, SweepDirection.Counterclockwise, doughnutParams.AnimationEnabled ? point10 : point10));
					ellipse4.Clip = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point10, list3, true);
					PathFigure pathFigure2 = (ellipse4.Clip as PathGeometry).Figures[0];
					PathSegmentCollection segments2 = pathFigure2.Segments;
					if (doughnutParams.AnimationEnabled)
					{
						doughnutParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments2[0], center, doughnutParams.OuterRadius, currentDataSeries.InternalStartAngle, doughnutParams.StartAngle);
						doughnutParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments2[1], center, doughnutParams.OuterRadius, currentDataSeries.InternalStartAngle, doughnutParams.MeanAngle);
						doughnutParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments2[2], center, doughnutParams.OuterRadius, currentDataSeries.InternalStartAngle, doughnutParams.StopAngle);
						doughnutParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments2[3], center, doughnutParams.InnerRadius, currentDataSeries.InternalStartAngle, doughnutParams.StopAngle);
						doughnutParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments2[4], center, doughnutParams.InnerRadius, currentDataSeries.InternalStartAngle, doughnutParams.MeanAngle);
						doughnutParams.Storyboard = PieChart.CreatePathSegmentAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments2[5], center, doughnutParams.InnerRadius, currentDataSeries.InternalStartAngle, doughnutParams.StartAngle);
						doughnutParams.Storyboard = PieChart.CreatePathFigureAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, pathFigure2, center, doughnutParams.InnerRadius, currentDataSeries.InternalStartAngle, doughnutParams.StartAngle);
					}
				}
				canvas2.Children.Add(ellipse4);
			}
			if (doughnutParams.LabelLineEnabled)
			{
				Path path2 = new Path
				{
					Tag = new ElementData
					{
						Element = doughnutParams.TagReference
					}
				};
				double meanAngle = doughnutParams.MeanAngle;
				Point point13 = default(Point);
				point13.X = center.X + doughnutParams.OuterRadius * Math.Cos(meanAngle);
				point13.Y = center.Y + doughnutParams.OuterRadius * Math.Sin(meanAngle);
				Point point14 = default(Point);
				point14.X = center.X + doughnutParams.LabelPoint.X - doughnutParams.Width / 2.0;
				point14.Y = center.Y + doughnutParams.LabelPoint.Y - doughnutParams.Height / 2.0;
				Point point15 = default(Point);
				if (doughnutParams.LabelLineTargetToRight)
				{
					point15.X = point14.X + 10.0;
				}
				else
				{
					point15.X = point14.X - 10.0;
				}
				point15.Y = point14.Y;
				List<PieChart.PathGeometryParams> list4 = new List<PieChart.PathGeometryParams>();
				list4.Add(new PieChart.LineSegmentParams(doughnutParams.AnimationEnabled ? point13 : point15));
				list4.Add(new PieChart.LineSegmentParams(doughnutParams.AnimationEnabled ? point13 : point14));
				path2.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point13, list4, true);
				PathFigure pathFigure3 = (path2.Data as PathGeometry).Figures[0];
				PathSegmentCollection segments3 = pathFigure3.Segments;
				pathFigure3.IsClosed = false;
				pathFigure3.IsFilled = false;
				if (doughnutParams.AnimationEnabled)
				{
					doughnutParams.Storyboard = PieChart.CreateLabelLineAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments3[0], new Point[]
					{
						point13,
						point15
					});
					doughnutParams.Storyboard = PieChart.CreateLabelLineAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, segments3[1], new Point[]
					{
						point13,
						point15,
						point14
					});
				}
				path2.Stroke = doughnutParams.LabelLineColor;
				path2.StrokeDashArray = doughnutParams.LabelLineStyle;
				path2.StrokeThickness = doughnutParams.LabelLineThickness;
				if ((doughnutParams.TagReference as IDataPoint).DpInfo.InternalYValue == 0.0)
				{
					Line line = new Line();
					line.X1 = center.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.MeanAngle);
					line.Y1 = center.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.MeanAngle);
					line.X2 = point13.X;
					line.Y2 = point13.Y;
					line.Stroke = doughnutParams.LabelLineColor;
					line.StrokeThickness = 0.25;
					line.IsHitTestVisible = false;
					canvas.Children.Add(line);
					if (doughnutParams.AnimationEnabled)
					{
						double beginTime = 2.0;
						doughnutParams.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, line, beginTime, line.Opacity, 0.5);
						line.Opacity = 0.0;
					}
				}
				canvas.Children.Add(path2);
				labelLinePath = path2;
				unExplodedPoints.LabelLineEndPoint = point14;
				unExplodedPoints.LabelLineMidPoint = point15;
				unExplodedPoints.LabelLineStartPoint = point13;
				explodedPoints.LabelLineEndPoint = new Point(point14.X, point14.Y - num4);
				explodedPoints.LabelLineMidPoint = new Point(point15.X, point15.Y - num4);
				explodedPoints.LabelLineStartPoint = new Point(point13.X + num3, point13.Y + num4);
				if ((doughnutParams.TagReference as IDataPoint).DpInfo.InternalYValue == 0.0)
				{
					path2.IsHitTestVisible = false;
				}
			}
			if (doughnutParams.Bevel && Math.Abs(doughnutParams.StartAngle - doughnutParams.StopAngle) > 0.03 && (doughnutParams.TagReference as IDataPoint).DpInfo.InternalYValue != 0.0)
			{
				Point endPoint = default(Point);
				Point endPoint2 = default(Point);
				Point endPoint3 = default(Point);
				Point endPoint4 = default(Point);
				endPoint.X = center.X + doughnutParams.OuterRadius * Math.Cos(doughnutParams.StartAngle);
				endPoint.Y = center.Y + doughnutParams.OuterRadius * Math.Sin(doughnutParams.StartAngle);
				endPoint2.X = center.X + doughnutParams.OuterRadius * Math.Cos(doughnutParams.StopAngle);
				endPoint2.Y = center.Y + doughnutParams.OuterRadius * Math.Sin(doughnutParams.StopAngle);
				endPoint3.X = center.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.StartAngle);
				endPoint3.Y = center.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.StartAngle);
				endPoint4.X = center.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.StopAngle);
				endPoint4.Y = center.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.StopAngle);
				Point point16 = default(Point);
				Point point17 = default(Point);
				Point point18 = default(Point);
				Point point19 = default(Point);
				Point point20 = default(Point);
				double num5 = 4.0;
				double num6 = Math.Abs(doughnutParams.OuterRadius - num5);
				double num7 = Math.Abs(doughnutParams.InnerRadius + num5);
				point16.X = center.X + num5 * Math.Cos(doughnutParams.MeanAngle);
				point16.Y = center.Y + num5 * Math.Sin(doughnutParams.MeanAngle);
				point17.X = center.X + num6 * Math.Cos(doughnutParams.StartAngle + 0.03);
				point17.Y = center.Y + num6 * Math.Sin(doughnutParams.StartAngle + 0.03);
				point18.X = center.X + num6 * Math.Cos(doughnutParams.StopAngle - 0.03);
				point18.Y = center.Y + num6 * Math.Sin(doughnutParams.StopAngle - 0.03);
				point19.X = center.X + num7 * Math.Cos(doughnutParams.StartAngle + 0.03);
				point19.Y = center.Y + num7 * Math.Sin(doughnutParams.StartAngle + 0.03);
				point20.X = center.X + num7 * Math.Cos(doughnutParams.StopAngle - 0.03);
				point20.Y = center.Y + num7 * Math.Sin(doughnutParams.StopAngle - 0.03);
				List<PieChart.PathGeometryParams> list5 = new List<PieChart.PathGeometryParams>();
				list5.Add(new PieChart.LineSegmentParams(endPoint3));
				list5.Add(new PieChart.LineSegmentParams(endPoint));
				list5.Add(new PieChart.LineSegmentParams(point17));
				list5.Add(new PieChart.LineSegmentParams(point19));
				Path path3 = new Path
				{
					Tag = new ElementData
					{
						Element = doughnutParams.TagReference
					}
				};
				path3.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point19, list5, true);
				if (doughnutParams.StartAngle > 1.5707963267948966 && doughnutParams.StartAngle <= 4.71238898038469)
				{
					path3.Fill = PieChart.GetDarkerBevelBrush(doughnutParams.Background, doughnutParams.StartAngle * 180.0 / 3.1415926535897931 + 135.0);
				}
				else
				{
					path3.Fill = PieChart.GetLighterBevelBrush(doughnutParams.Background, -doughnutParams.StartAngle * 180.0 / 3.1415926535897931);
				}
				if (doughnutParams.AnimationEnabled)
				{
					double beginTime2 = 1.0;
					doughnutParams.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, path3, beginTime2, 1.0, 1.0);
					path3.Opacity = 0.0;
				}
				faces.Parts.Add(path3);
				canvas.Children.Add(path3);
				list5 = new List<PieChart.PathGeometryParams>();
				list5.Add(new PieChart.LineSegmentParams(endPoint4));
				list5.Add(new PieChart.LineSegmentParams(endPoint2));
				list5.Add(new PieChart.LineSegmentParams(point18));
				list5.Add(new PieChart.LineSegmentParams(point20));
				path3 = new Path
				{
					Tag = new ElementData
					{
						Element = doughnutParams.TagReference
					}
				};
				path3.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point20, list5, true);
				if (doughnutParams.StopAngle > 1.5707963267948966 && doughnutParams.StopAngle <= 4.71238898038469)
				{
					path3.Fill = PieChart.GetLighterBevelBrush(doughnutParams.Background, doughnutParams.StopAngle * 180.0 / 3.1415926535897931 + 135.0);
				}
				else
				{
					path3.Fill = PieChart.GetDarkerBevelBrush(doughnutParams.Background, -doughnutParams.StopAngle * 180.0 / 3.1415926535897931);
				}
				if (doughnutParams.AnimationEnabled)
				{
					double beginTime3 = 1.0;
					doughnutParams.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, path3, beginTime3, 1.0, 1.0);
					path3.Opacity = 0.0;
				}
				faces.Parts.Add(path3);
				canvas.Children.Add(path3);
				Shape shape;
				if (enabledDataPoints.Count == 1 || source.Count<IDataPoint>() == 1)
				{
					list5 = new List<PieChart.PathGeometryParams>();
					list5.Add(new PieChart.ArcSegmentParams(new Size(num6, num6), 0.0, doughnutParams.IsLargerArc, SweepDirection.Clockwise, point17));
					list5.Add(new PieChart.LineSegmentParams(point18));
					list5.Add(new PieChart.ArcSegmentParams(new Size(num6, num6), 0.0, doughnutParams.IsLargerArc, SweepDirection.Counterclockwise, point18));
					shape = new Path
					{
						Tag = new ElementData
						{
							Element = doughnutParams.TagReference
						}
					};
					(shape as Path).Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point17, list5, true);
				}
				else
				{
					list5 = new List<PieChart.PathGeometryParams>();
					list5.Add(new PieChart.LineSegmentParams(endPoint2));
					list5.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.OuterRadius, doughnutParams.OuterRadius), 0.0, doughnutParams.IsLargerArc, SweepDirection.Counterclockwise, endPoint));
					list5.Add(new PieChart.LineSegmentParams(point17));
					list5.Add(new PieChart.ArcSegmentParams(new Size(num6, num6), 0.0, doughnutParams.IsLargerArc, SweepDirection.Clockwise, point18));
					shape = new Path
					{
						Tag = new ElementData
						{
							Element = doughnutParams.TagReference
						}
					};
					(shape as Path).Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point18, list5, true);
				}
				if (doughnutParams.MeanAngle > 0.0 && doughnutParams.MeanAngle < 3.1415926535897931)
				{
					shape.Fill = PieChart.GetCurvedBevelBrush(doughnutParams.Background, doughnutParams.MeanAngle * 180.0 / 3.1415926535897931 + 90.0, Graphics.GenerateDoubleCollection(new double[]
					{
						-0.745,
						-0.85
					}), Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						1.0
					}));
				}
				else
				{
					shape.Fill = PieChart.GetCurvedBevelBrush(doughnutParams.Background, doughnutParams.MeanAngle * 180.0 / 3.1415926535897931 + 90.0, Graphics.GenerateDoubleCollection(new double[]
					{
						0.745,
						-0.99
					}), Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						1.0
					}));
				}
				if (doughnutParams.AnimationEnabled)
				{
					double beginTime4 = 1.0;
					doughnutParams.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, shape, beginTime4, 1.0, 1.0);
					shape.Opacity = 0.0;
				}
				faces.Parts.Add(shape);
				canvas.Children.Add(shape);
				Shape shape2;
				if (enabledDataPoints.Count == 1 || source.Count<IDataPoint>() == 1)
				{
					list5 = new List<PieChart.PathGeometryParams>();
					list5.Add(new PieChart.ArcSegmentParams(new Size(num6, num6), 0.0, doughnutParams.IsLargerArc, SweepDirection.Clockwise, point19));
					list5.Add(new PieChart.LineSegmentParams(point20));
					list5.Add(new PieChart.ArcSegmentParams(new Size(num6, num6), 0.0, doughnutParams.IsLargerArc, SweepDirection.Counterclockwise, point20));
					shape2 = new Path
					{
						Tag = new ElementData
						{
							Element = doughnutParams.TagReference
						}
					};
					(shape2 as Path).Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point19, list5, true);
				}
				else
				{
					list5 = new List<PieChart.PathGeometryParams>();
					list5.Add(new PieChart.LineSegmentParams(endPoint4));
					list5.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.InnerRadius, doughnutParams.InnerRadius), 0.0, doughnutParams.IsLargerArc, SweepDirection.Counterclockwise, endPoint3));
					list5.Add(new PieChart.LineSegmentParams(point19));
					list5.Add(new PieChart.ArcSegmentParams(new Size(num7, num7), 0.0, doughnutParams.IsLargerArc, SweepDirection.Clockwise, point20));
					shape2 = new Path
					{
						Tag = new ElementData
						{
							Element = doughnutParams.TagReference
						}
					};
					(shape2 as Path).Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point20, list5, true);
				}
				if (doughnutParams.MeanAngle > 0.0 && doughnutParams.MeanAngle < 3.1415926535897931)
				{
					shape2.Fill = PieChart.GetCurvedBevelBrush(doughnutParams.Background, doughnutParams.MeanAngle * 180.0 / 3.1415926535897931 + 90.0, Graphics.GenerateDoubleCollection(new double[]
					{
						0.745,
						-0.99
					}), Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						1.0
					}));
				}
				else
				{
					shape2.Fill = PieChart.GetCurvedBevelBrush(doughnutParams.Background, doughnutParams.MeanAngle * 180.0 / 3.1415926535897931 + 90.0, Graphics.GenerateDoubleCollection(new double[]
					{
						-0.745,
						-0.85
					}), Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						1.0
					}));
				}
				if (doughnutParams.AnimationEnabled)
				{
					double beginTime5 = 1.0;
					doughnutParams.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, doughnutParams.DataPoint, doughnutParams.Storyboard, shape2, beginTime5, 1.0, 1.0);
					shape2.Opacity = 0.0;
				}
				faces.Parts.Add(shape2);
				canvas.Children.Add(shape2);
			}
			else
			{
				faces.Parts.Add(null);
				faces.Parts.Add(null);
				faces.Parts.Add(null);
				faces.Parts.Add(null);
			}
			return canvas;
		}

		private static Path CreateLabelLine(DataSeries currentDataSeries, SectorChartShapeParams pieParams, Point centerOfPie, ref PieDoughnut3DPoints unExplodedPoints, ref PieDoughnut3DPoints explodedPoints)
		{
			Path path = null;
			if (pieParams.LabelLineEnabled)
			{
				path = new Path
				{
					Tag = new ElementData
					{
						Element = pieParams.TagReference
					}
				};
				double meanAngle = pieParams.MeanAngle;
				Point point = default(Point);
				point.X = centerOfPie.X + pieParams.OuterRadius * Math.Cos(meanAngle);
				point.Y = centerOfPie.Y + pieParams.OuterRadius * Math.Sin(meanAngle) * pieParams.YAxisScaling;
				Point point2 = default(Point);
				point2.X = centerOfPie.X + pieParams.LabelPoint.X - pieParams.Width / 2.0;
				point2.Y = centerOfPie.Y + pieParams.LabelPoint.Y - pieParams.Height / 2.0;
				Point point3 = default(Point);
				if (pieParams.LabelLineTargetToRight)
				{
					point3.X = point2.X + 10.0;
				}
				else
				{
					point3.X = point2.X - 10.0;
				}
				point3.Y = point2.Y;
				List<PieChart.PathGeometryParams> list = new List<PieChart.PathGeometryParams>();
				list.Add(new PieChart.LineSegmentParams(pieParams.AnimationEnabled ? point : point3));
				list.Add(new PieChart.LineSegmentParams(pieParams.AnimationEnabled ? point : point2));
				path.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, point, list, true);
				PathFigure pathFigure = (path.Data as PathGeometry).Figures[0];
				PathSegmentCollection segments = pathFigure.Segments;
				pathFigure.IsClosed = false;
				pathFigure.IsFilled = false;
				if (pieParams.AnimationEnabled)
				{
					pieParams.Storyboard = PieChart.CreateLabelLineAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, segments[0], new Point[]
					{
						point,
						point3
					});
					pieParams.Storyboard = PieChart.CreateLabelLineAnimation(currentDataSeries, pieParams.DataPoint, pieParams.Storyboard, segments[1], new Point[]
					{
						point,
						point3,
						point2
					});
				}
				path.Stroke = pieParams.LabelLineColor;
				path.StrokeDashArray = pieParams.LabelLineStyle;
				path.StrokeThickness = pieParams.LabelLineThickness;
				unExplodedPoints.LabelLineEndPoint = point2;
				unExplodedPoints.LabelLineMidPoint = point3;
				unExplodedPoints.LabelLineStartPoint = point;
				explodedPoints.LabelLineEndPoint = new Point(point2.X, point2.Y - pieParams.OffsetY);
				explodedPoints.LabelLineMidPoint = new Point(point3.X, point3.Y - pieParams.OffsetY);
				explodedPoints.LabelLineStartPoint = new Point(point.X + pieParams.OffsetX, point.Y + pieParams.OffsetY);
				if ((pieParams.TagReference as IDataPoint).DpInfo.InternalYValue == 0.0)
				{
					path.IsHitTestVisible = false;
				}
			}
			return path;
		}

		private static void UpdatePositionLabelInsidePie(SectorChartShapeParams pieParams, double yOffset)
		{
			IDataPoint dataPoint = pieParams.DataPoint;
			RenderAs renderAs = dataPoint.Parent.RenderAs;
			if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle) == LabelStyles.Inside)
			{
				Point point = default(Point);
				point.X = pieParams.Width / 2.0;
				point.Y = pieParams.Height / 2.0;
				double num = 3.0 * (pieParams.OuterRadius / 4.0);
				double num2 = 3.0 * (pieParams.OuterRadius / 4.0) * pieParams.YAxisScaling;
				double num3 = point.X + num * Math.Cos(pieParams.MeanAngle);
				double num4 = point.Y + num2 * Math.Sin(pieParams.MeanAngle) + yOffset;
				Point point2 = default(Point);
				point2.X = point.X + pieParams.OuterRadius * Math.Cos(pieParams.MeanAngle);
				point2.Y = point.Y + yOffset + pieParams.OuterRadius * Math.Sin(pieParams.MeanAngle) * pieParams.YAxisScaling;
				Point point3 = Graphics.MidPointOfALine(new Point
				{
					X = point.X,
					Y = point.Y + yOffset
				}, point2);
				if (renderAs == RenderAs.Doughnut)
				{
					dataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, num3 - dataPoint.DpInfo.LabelVisual.Width / 2.0 - dataPoint.DpInfo.ClippedWidth4Pie);
					dataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, num4 - dataPoint.DpInfo.LabelVisual.Height / 2.0);
					return;
				}
				if (renderAs == RenderAs.Pie)
				{
					dataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, point3.X - dataPoint.DpInfo.LabelVisual.Width / 2.0 - dataPoint.DpInfo.ClippedWidth4Pie);
					dataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, point3.Y - dataPoint.DpInfo.LabelVisual.Height / 2.0);
				}
			}
		}

		private static List<Shape> GetPie3D(DataSeries currentDataSeries, ref Canvas visual, ref Faces faces, SectorChartShapeParams pieParams, ref int zindex, ref PieDoughnut3DPoints unExplodedPoints, ref PieDoughnut3DPoints explodedPoints, ref Path labelLinePath, List<IDataPoint> enabledDataPoints)
		{
			List<Shape> list = new List<Shape>();
			Shape shape = null;
			Shape shape2 = null;
			Shape shape3 = null;
			Shape shape4 = null;
			Point centerOfPie = default(Point);
			centerOfPie.X = pieParams.Width / 2.0;
			centerOfPie.Y = pieParams.Height / 2.0;
			double num = -pieParams.Depth / 2.0 * pieParams.ZAxisScaling;
			Point3D point3D = default(Point3D);
			point3D.X = centerOfPie.X;
			point3D.Y = centerOfPie.Y + num;
			point3D.Z = pieParams.OffsetY * Math.Sin(pieParams.StartAngle) * Math.Cos(pieParams.TiltAngle) + pieParams.Depth * Math.Cos(1.5707963267948966 - pieParams.TiltAngle);
			Point3D point3D2 = default(Point3D);
			point3D2.X = point3D.X + pieParams.OuterRadius * Math.Cos(pieParams.StartAngle);
			point3D2.Y = point3D.Y + pieParams.OuterRadius * Math.Sin(pieParams.StartAngle) * pieParams.YAxisScaling;
			point3D2.Z = (point3D.Y + pieParams.OuterRadius) * Math.Sin(pieParams.StartAngle) * Math.Cos(pieParams.TiltAngle) + pieParams.Depth * Math.Cos(1.5707963267948966 - pieParams.TiltAngle);
			Point3D point3D3 = default(Point3D);
			point3D3.X = point3D.X + pieParams.OuterRadius * Math.Cos(pieParams.StopAngle);
			point3D3.Y = point3D.Y + pieParams.OuterRadius * Math.Sin(pieParams.StopAngle) * pieParams.YAxisScaling;
			point3D3.Z = (point3D.Y + pieParams.OuterRadius) * Math.Sin(pieParams.StopAngle) * Math.Cos(pieParams.TiltAngle) + pieParams.Depth * Math.Cos(1.5707963267948966 - pieParams.TiltAngle);
			Point3D point3D4 = default(Point3D);
			point3D4.X = centerOfPie.X;
			point3D4.Y = centerOfPie.Y - num;
			point3D4.Z = pieParams.OffsetY * Math.Sin(pieParams.StartAngle) * Math.Cos(pieParams.TiltAngle) - pieParams.Depth * Math.Cos(1.5707963267948966 - pieParams.TiltAngle);
			Point3D point3D5 = default(Point3D);
			point3D5.X = point3D4.X + pieParams.OuterRadius * Math.Cos(pieParams.StartAngle);
			point3D5.Y = point3D4.Y + pieParams.OuterRadius * Math.Sin(pieParams.StartAngle) * pieParams.YAxisScaling;
			point3D5.Z = (point3D4.Y + pieParams.OuterRadius) * Math.Sin(pieParams.StartAngle) * Math.Cos(pieParams.TiltAngle) - pieParams.Depth * Math.Cos(1.5707963267948966 - pieParams.TiltAngle);
			Point3D point3D6 = default(Point3D);
			point3D6.X = point3D4.X + pieParams.OuterRadius * Math.Cos(pieParams.StopAngle);
			point3D6.Y = point3D4.Y + pieParams.OuterRadius * Math.Sin(pieParams.StopAngle) * pieParams.YAxisScaling;
			point3D6.Z = (point3D4.Y + pieParams.OuterRadius) * Math.Sin(pieParams.StopAngle) * Math.Cos(pieParams.TiltAngle) - pieParams.Depth * Math.Cos(1.5707963267948966 - pieParams.TiltAngle);
			Point3D centroid = PieChart.GetCentroid(new Point3D[]
			{
				point3D,
				point3D2,
				point3D3,
				point3D4,
				point3D5,
				point3D6
			});
			PieChart.UpdatePositionLabelInsidePie(pieParams, num);
			if ((pieParams.StartAngle == pieParams.StopAngle && pieParams.IsLargerArc) || enabledDataPoints.Count == 1)
			{
				shape = new Ellipse
				{
					Tag = new ElementData
					{
						Element = pieParams.TagReference
					}
				};
				shape.Fill = (pieParams.Lighting ? Graphics.GetLightingEnabledBrush(pieParams.Background, "Linear", new double[]
				{
					0.99,
					0.854
				}) : pieParams.Background);
				shape.Width = 2.0 * pieParams.OuterRadius;
				shape.Height = 2.0 * pieParams.OuterRadius * pieParams.YAxisScaling;
				shape.SetValue(Canvas.LeftProperty, pieParams.Center.X - shape.Width / 2.0);
				shape.SetValue(Canvas.TopProperty, pieParams.Center.Y - shape.Height / 2.0 + num);
				list.Add(shape);
				faces.Parts.Add(shape);
				shape2 = new Ellipse
				{
					Tag = new ElementData
					{
						Element = pieParams.TagReference
					}
				};
				shape2.Fill = (pieParams.Lighting ? Graphics.GetLightingEnabledBrush(pieParams.Background, "Radial", new double[]
				{
					0.99,
					0.745
				}) : pieParams.Background);
				shape2.Width = 2.0 * pieParams.OuterRadius;
				shape2.Height = 2.0 * pieParams.OuterRadius * pieParams.YAxisScaling;
				shape2.SetValue(Canvas.LeftProperty, pieParams.Center.X - shape.Width / 2.0);
				shape2.SetValue(Canvas.TopProperty, pieParams.Center.Y - shape.Height / 2.0 + num);
				list.Add(shape2);
				faces.Parts.Add(shape2);
			}
			else
			{
				shape = PieChart.GetPieFace(pieParams, centroid, point3D, point3D2, point3D3);
				list.Add(shape);
				faces.Parts.Add(shape);
				shape2 = PieChart.GetPieFace(pieParams, centroid, point3D4, point3D5, point3D6);
				list.Add(shape2);
				faces.Parts.Add(shape2);
				shape3 = PieChart.GetPieSide(pieParams, centroid, point3D, point3D4, point3D2, point3D5);
				list.Add(shape3);
				faces.Parts.Add(shape3);
				shape4 = PieChart.GetPieSide(pieParams, centroid, point3D, point3D4, point3D3, point3D6);
				list.Add(shape4);
				faces.Parts.Add(shape4);
			}
			labelLinePath = PieChart.CreateLabelLine(currentDataSeries, pieParams, centerOfPie, ref unExplodedPoints, ref explodedPoints);
			if ((pieParams.TagReference as IDataPoint).DpInfo.InternalYValue == 0.0)
			{
				return new List<Shape>();
			}
			List<Shape> pieOuterCurvedFace = PieChart.GetPieOuterCurvedFace(pieParams, centroid, point3D, point3D4);
			list.InsertRange(list.Count, pieOuterCurvedFace.ToList<Shape>());
			foreach (FrameworkElement current in pieOuterCurvedFace)
			{
				faces.Parts.Add(current);
			}
			shape.SetValue(Panel.ZIndexProperty, 50000);
			shape2.SetValue(Panel.ZIndexProperty, -50000);
			if (pieParams.StartAngle != pieParams.StopAngle || !pieParams.IsLargerArc)
			{
				if (pieParams.StartAngle >= 3.1415926535897931 && pieParams.StartAngle <= 6.2831853071795862 && pieParams.StopAngle >= 3.1415926535897931 && pieParams.StopAngle <= 6.2831853071795862 && pieParams.IsLargerArc)
				{
					PieChart._elementPositionData.Add(new ElementPositionData(shape3, pieParams.StartAngle, pieParams.StartAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(pieOuterCurvedFace[0], 0.0, 3.1415926535897931));
					PieChart._elementPositionData.Add(new ElementPositionData(shape4, pieParams.StopAngle, pieParams.StopAngle));
					if (labelLinePath != null)
					{
						PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, 0.0, 3.1415926535897931));
					}
				}
				else if (pieParams.StartAngle >= 0.0 && pieParams.StartAngle <= 3.1415926535897931 && pieParams.StopAngle >= 0.0 && pieParams.StopAngle <= 3.1415926535897931 && pieParams.IsLargerArc)
				{
					PieChart._elementPositionData.Add(new ElementPositionData(shape3, pieParams.StartAngle, pieParams.StartAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(pieOuterCurvedFace[0], pieParams.StartAngle, 3.1415926535897931));
					PieChart._elementPositionData.Add(new ElementPositionData(pieOuterCurvedFace[1], 0.0, pieParams.StopAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(shape4, pieParams.StopAngle, pieParams.StopAngle));
					if (labelLinePath != null)
					{
						labelLinePath.SetValue(Panel.ZIndexProperty, -50000);
					}
				}
				else if (pieParams.StartAngle >= 0.0 && pieParams.StartAngle <= 3.1415926535897931 && pieParams.StopAngle >= 3.1415926535897931 && pieParams.StopAngle <= 6.2831853071795862)
				{
					PieChart._elementPositionData.Add(new ElementPositionData(shape3, pieParams.StartAngle, pieParams.StartAngle));
					if (labelLinePath != null)
					{
						if (pieParams.StartAngle >= 1.5707963267948966 && pieParams.StopAngle <= 3.1415926535897931)
						{
							labelLinePath.SetValue(Panel.ZIndexProperty, 50000);
						}
						else
						{
							PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, pieParams.StartAngle, 3.1415926535897931));
						}
					}
					PieChart._elementPositionData.Add(new ElementPositionData(shape4, pieParams.StopAngle, pieParams.StopAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(pieOuterCurvedFace[0], pieParams.StartAngle, 3.1415926535897931));
				}
				else if (pieParams.StartAngle >= 3.1415926535897931 && pieParams.StartAngle <= 6.2831853071795862 && pieParams.StopAngle >= 0.0 && pieParams.StopAngle <= 3.1415926535897931)
				{
					PieChart._elementPositionData.Add(new ElementPositionData(shape3, pieParams.StartAngle, pieParams.StartAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(pieOuterCurvedFace[0], 0.0, pieParams.StopAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(shape4, pieParams.StopAngle, pieParams.StopAngle));
					if (labelLinePath != null)
					{
						PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, pieParams.StopAngle, pieParams.StopAngle));
					}
				}
				else
				{
					PieChart._elementPositionData.Add(new ElementPositionData(shape3, pieParams.StartAngle, pieParams.StartAngle));
					if (pieParams.StartAngle >= 0.0 && pieParams.StartAngle < 1.5707963267948966 && pieParams.StopAngle >= 0.0 && pieParams.StopAngle < 1.5707963267948966)
					{
						if (labelLinePath != null)
						{
							PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, pieParams.StopAngle, pieParams.StopAngle));
						}
						PieChart._elementPositionData.Add(new ElementPositionData(pieOuterCurvedFace[0], pieParams.StartAngle, pieParams.StopAngle));
					}
					else if (pieParams.StartAngle >= 0.0 && pieParams.StartAngle < 1.5707963267948966 && pieParams.StopAngle >= 1.5707963267948966 && pieParams.StopAngle < 3.1415926535897931)
					{
						if (labelLinePath != null)
						{
							labelLinePath.SetValue(Panel.ZIndexProperty, 40000);
						}
						pieOuterCurvedFace[0].SetValue(Panel.ZIndexProperty, 35000);
					}
					else if (pieParams.StartAngle >= 1.5707963267948966 && pieParams.StartAngle < 3.1415926535897931 && pieParams.StopAngle >= 1.5707963267948966 && pieParams.StopAngle < 3.1415926535897931)
					{
						if (labelLinePath != null)
						{
							PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, pieParams.StartAngle, pieParams.StartAngle));
						}
						PieChart._elementPositionData.Add(new ElementPositionData(pieOuterCurvedFace[0], pieParams.StartAngle, pieParams.StopAngle));
					}
					else if (pieParams.StartAngle >= 3.1415926535897931 && pieParams.StartAngle < 4.71238898038469 && pieParams.StopAngle >= 3.1415926535897931 && pieParams.StopAngle < 4.71238898038469)
					{
						PieChart._elementPositionData.Add(new ElementPositionData(pieOuterCurvedFace[0], pieParams.StartAngle, pieParams.StopAngle));
						if (labelLinePath != null)
						{
							PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, pieParams.StartAngle, pieParams.StartAngle));
						}
					}
					else if (pieParams.StartAngle >= 4.71238898038469 && pieParams.StartAngle < 6.2831853071795862 && pieParams.StopAngle >= 4.71238898038469 && pieParams.StopAngle < 6.2831853071795862)
					{
						PieChart._elementPositionData.Add(new ElementPositionData(pieOuterCurvedFace[0], pieParams.StartAngle, pieParams.StopAngle));
						if (labelLinePath != null)
						{
							PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, pieParams.StartAngle, pieParams.StopAngle));
						}
					}
					else
					{
						if (labelLinePath != null)
						{
							PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, pieParams.StartAngle, pieParams.StartAngle));
						}
						PieChart._elementPositionData.Add(new ElementPositionData(pieOuterCurvedFace[0], pieParams.StartAngle, pieParams.StopAngle));
					}
					PieChart._elementPositionData.Add(new ElementPositionData(shape4, pieParams.StopAngle, pieParams.StopAngle));
				}
			}
			return list;
		}

		internal static Brush Get3DFaceColor(RenderAs renderAs, bool lightingEnabled, Brush color, PieFaceTypes faceType, double startAngle, double stopAngle, double tiltAngle)
		{
			Brush result = null;
			switch (faceType)
			{
			case PieFaceTypes.Top:
			case PieFaceTypes.Bottom:
				if (renderAs == RenderAs.Pie)
				{
					result = (lightingEnabled ? Graphics.GetLightingEnabledBrush(color, "Linear", new double[]
					{
						0.99,
						0.789
					}) : color);
				}
				else
				{
					result = (lightingEnabled ? Graphics.GetLightingEnabledBrush(color, -75.0, "Linear", new double[]
					{
						0.99,
						0.854
					}) : color);
				}
				break;
			case PieFaceTypes.Left:
			case PieFaceTypes.Right:
				result = (lightingEnabled ? Graphics.GetLightingEnabledBrush(color, "Linear", new double[]
				{
					0.845,
					0.79
				}) : color);
				break;
			case PieFaceTypes.CurvedSurface:
				if (renderAs == RenderAs.Pie)
				{
					result = (lightingEnabled ? Graphics.GetLightingEnabledBrush(color, startAngle * 180.0 / 3.1415926535897931, "Linear", new double[]
					{
						0.99,
						0.789
					}) : color);
				}
				else if (stopAngle > 3.1415926535897931)
				{
					result = (lightingEnabled ? Graphics.GetLightingEnabledBrush(color, tiltAngle * 180.0 / 3.1415926535897931, "Linear", new double[]
					{
						0.99,
						0.845
					}) : color);
				}
				else
				{
					result = (lightingEnabled ? Graphics.GetLightingEnabledBrush(color, startAngle * 180.0 / 3.1415926535897931, "Linear", new double[]
					{
						0.99,
						0.845
					}) : color);
				}
				break;
			}
			return result;
		}

		private static List<Shape> GetDoughnut3D(DataSeries currentDataSeries, ref Faces faces, SectorChartShapeParams doughnutParams, ref PieDoughnut3DPoints unExplodedPoints, ref PieDoughnut3DPoints explodedPoints, ref Path labelLinePath, List<IDataPoint> enabledDataPoints)
		{
			List<Shape> list = new List<Shape>();
			Shape shape = null;
			Shape shape2 = null;
			Shape shape3 = null;
			Shape shape4 = null;
			double num = -doughnutParams.Depth / 2.0 * doughnutParams.ZAxisScaling;
			Point centerOfPie = default(Point);
			centerOfPie.X += doughnutParams.Width / 2.0;
			centerOfPie.Y += doughnutParams.Height / 2.0;
			Point3D topFaceCenter = default(Point3D);
			topFaceCenter.X = centerOfPie.X;
			topFaceCenter.Y = centerOfPie.Y + num;
			topFaceCenter.Z = doughnutParams.OffsetY * Math.Sin(doughnutParams.StartAngle) * Math.Cos(doughnutParams.TiltAngle) + doughnutParams.Depth * Math.Cos(1.5707963267948966 - doughnutParams.TiltAngle);
			Point3D point3D = default(Point3D);
			point3D.X = topFaceCenter.X + doughnutParams.OuterRadius * Math.Cos(doughnutParams.StartAngle);
			point3D.Y = topFaceCenter.Y + doughnutParams.OuterRadius * Math.Sin(doughnutParams.StartAngle) * doughnutParams.YAxisScaling;
			point3D.Z = (topFaceCenter.Y + doughnutParams.OuterRadius) * Math.Sin(doughnutParams.StartAngle) * Math.Cos(doughnutParams.TiltAngle) + doughnutParams.Depth * Math.Cos(1.5707963267948966 - doughnutParams.TiltAngle);
			Point3D point3D2 = default(Point3D);
			point3D2.X = topFaceCenter.X + doughnutParams.OuterRadius * Math.Cos(doughnutParams.StopAngle);
			point3D2.Y = topFaceCenter.Y + doughnutParams.OuterRadius * Math.Sin(doughnutParams.StopAngle) * doughnutParams.YAxisScaling;
			point3D2.Z = (topFaceCenter.Y + doughnutParams.OuterRadius) * Math.Sin(doughnutParams.StopAngle) * Math.Cos(doughnutParams.TiltAngle) + doughnutParams.Depth * Math.Cos(1.5707963267948966 - doughnutParams.TiltAngle);
			Point3D point3D3 = default(Point3D);
			point3D3.X = topFaceCenter.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.StartAngle);
			point3D3.Y = topFaceCenter.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.StartAngle) * doughnutParams.YAxisScaling;
			point3D3.Z = (topFaceCenter.Y + doughnutParams.InnerRadius) * Math.Sin(doughnutParams.StartAngle) * Math.Cos(doughnutParams.TiltAngle) + doughnutParams.Depth * Math.Cos(1.5707963267948966 - doughnutParams.TiltAngle);
			Point3D point3D4 = default(Point3D);
			point3D4.X = topFaceCenter.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.StopAngle);
			point3D4.Y = topFaceCenter.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.StopAngle) * doughnutParams.YAxisScaling;
			point3D4.Z = (topFaceCenter.Y + doughnutParams.InnerRadius) * Math.Sin(doughnutParams.StopAngle) * Math.Cos(doughnutParams.TiltAngle) + doughnutParams.Depth * Math.Cos(1.5707963267948966 - doughnutParams.TiltAngle);
			Point3D bottomFaceCenter = default(Point3D);
			bottomFaceCenter.X = centerOfPie.X;
			bottomFaceCenter.Y = centerOfPie.Y - num;
			bottomFaceCenter.Z = doughnutParams.OffsetY * Math.Sin(doughnutParams.StartAngle) * Math.Cos(doughnutParams.TiltAngle) - doughnutParams.Depth * Math.Cos(1.5707963267948966 - doughnutParams.TiltAngle);
			Point3D point3D5 = default(Point3D);
			point3D5.X = bottomFaceCenter.X + doughnutParams.OuterRadius * Math.Cos(doughnutParams.StartAngle);
			point3D5.Y = bottomFaceCenter.Y + doughnutParams.OuterRadius * Math.Sin(doughnutParams.StartAngle) * doughnutParams.YAxisScaling;
			point3D5.Z = (bottomFaceCenter.Y + doughnutParams.OuterRadius) * Math.Sin(doughnutParams.StartAngle) * Math.Cos(doughnutParams.TiltAngle) - doughnutParams.Depth * Math.Cos(1.5707963267948966 - doughnutParams.TiltAngle);
			Point3D point3D6 = default(Point3D);
			point3D6.X = bottomFaceCenter.X + doughnutParams.OuterRadius * Math.Cos(doughnutParams.StopAngle);
			point3D6.Y = bottomFaceCenter.Y + doughnutParams.OuterRadius * Math.Sin(doughnutParams.StopAngle) * doughnutParams.YAxisScaling;
			point3D6.Z = (bottomFaceCenter.Y + doughnutParams.OuterRadius) * Math.Sin(doughnutParams.StopAngle) * Math.Cos(doughnutParams.TiltAngle) - doughnutParams.Depth * Math.Cos(1.5707963267948966 - doughnutParams.TiltAngle);
			Point3D point3D7 = default(Point3D);
			point3D7.X = bottomFaceCenter.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.StartAngle);
			point3D7.Y = bottomFaceCenter.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.StartAngle) * doughnutParams.YAxisScaling;
			point3D7.Z = (bottomFaceCenter.Y + doughnutParams.InnerRadius) * Math.Sin(doughnutParams.StartAngle) * Math.Cos(doughnutParams.TiltAngle) - doughnutParams.Depth * Math.Cos(1.5707963267948966 - doughnutParams.TiltAngle);
			Point3D point3D8 = default(Point3D);
			point3D8.X = bottomFaceCenter.X + doughnutParams.InnerRadius * Math.Cos(doughnutParams.StopAngle);
			point3D8.Y = bottomFaceCenter.Y + doughnutParams.InnerRadius * Math.Sin(doughnutParams.StopAngle) * doughnutParams.YAxisScaling;
			point3D8.Z = (bottomFaceCenter.Y + doughnutParams.InnerRadius) * Math.Sin(doughnutParams.StopAngle) * Math.Cos(doughnutParams.TiltAngle) - doughnutParams.Depth * Math.Cos(1.5707963267948966 - doughnutParams.TiltAngle);
			Point3D centroid = PieChart.GetCentroid(new Point3D[]
			{
				point3D3,
				point3D4,
				point3D,
				point3D2,
				point3D7,
				point3D8,
				point3D5,
				point3D6
			});
			PieChart.UpdatePositionLabelInsidePie(doughnutParams, num);
			if ((doughnutParams.StartAngle == doughnutParams.StopAngle && doughnutParams.IsLargerArc) || enabledDataPoints.Count == 1)
			{
				shape = new Ellipse
				{
					Tag = new ElementData
					{
						Element = doughnutParams.TagReference
					}
				};
				shape.Fill = (doughnutParams.Lighting ? Graphics.GetLightingEnabledBrush(doughnutParams.Background, "Linear", new double[]
				{
					0.99,
					0.854
				}) : doughnutParams.Background);
				shape.Width = 2.0 * doughnutParams.OuterRadius;
				shape.Height = 2.0 * doughnutParams.OuterRadius * doughnutParams.YAxisScaling;
				shape.SetValue(Canvas.LeftProperty, doughnutParams.Center.X - shape.Width / 2.0);
				shape.SetValue(Canvas.TopProperty, doughnutParams.Center.Y - shape.Height / 2.0 + num);
				shape.Clip = new GeometryGroup
				{
					Children = 
					{
						new EllipseGeometry
						{
							Center = new Point(shape.Width / 2.0, shape.Height / 2.0),
							RadiusX = shape.Width,
							RadiusY = shape.Height
						},
						new EllipseGeometry
						{
							Center = new Point(shape.Width / 2.0, shape.Height / 2.0),
							RadiusX = doughnutParams.InnerRadius,
							RadiusY = shape.Height - 2.0 * (doughnutParams.OuterRadius - doughnutParams.InnerRadius)
						}
					}
				};
				list.Add(shape);
				faces.Parts.Add(shape);
				shape2 = new Ellipse
				{
					Tag = new ElementData
					{
						Element = doughnutParams.TagReference
					}
				};
				shape2.Fill = (doughnutParams.Lighting ? Graphics.GetLightingEnabledBrush(doughnutParams.Background, "Radial", new double[]
				{
					0.99,
					0.745
				}) : doughnutParams.Background);
				shape2.Width = 2.0 * doughnutParams.OuterRadius;
				shape2.Height = 2.0 * doughnutParams.OuterRadius * doughnutParams.YAxisScaling;
				shape2.SetValue(Canvas.LeftProperty, doughnutParams.Center.X - shape.Width / 2.0);
				shape2.SetValue(Canvas.TopProperty, doughnutParams.Center.Y - shape.Height / 2.0);
				shape2.Clip = new GeometryGroup
				{
					Children = 
					{
						new EllipseGeometry
						{
							Center = new Point(shape.Width / 2.0, shape.Height / 2.0),
							RadiusX = shape.Width,
							RadiusY = shape.Height
						},
						new EllipseGeometry
						{
							Center = new Point(shape.Width / 2.0, shape.Height / 2.0),
							RadiusX = doughnutParams.InnerRadius,
							RadiusY = shape.Height - 2.0 * (doughnutParams.OuterRadius - doughnutParams.InnerRadius)
						}
					}
				};
				list.Add(shape2);
				faces.Parts.Add(shape2);
			}
			else
			{
				shape = PieChart.GetDoughnutFace(doughnutParams, centroid, point3D3, point3D4, point3D, point3D2, true);
				list.Add(shape);
				faces.Parts.Add(shape);
				shape2 = PieChart.GetDoughnutFace(doughnutParams, centroid, point3D7, point3D8, point3D5, point3D6, false);
				list.Add(shape2);
				faces.Parts.Add(shape2);
				shape3 = PieChart.GetPieSide(doughnutParams, centroid, point3D3, point3D7, point3D, point3D5);
				list.Add(shape3);
				faces.Parts.Add(shape3);
				shape4 = PieChart.GetPieSide(doughnutParams, centroid, point3D4, point3D8, point3D2, point3D6);
				list.Add(shape4);
				faces.Parts.Add(shape4);
			}
			List<Shape> doughnutCurvedFace = PieChart.GetDoughnutCurvedFace(doughnutParams, centroid, topFaceCenter, bottomFaceCenter);
			list.InsertRange(list.Count, doughnutCurvedFace);
			foreach (FrameworkElement current in doughnutCurvedFace)
			{
				faces.Parts.Add(current);
			}
			labelLinePath = PieChart.CreateLabelLine(currentDataSeries, doughnutParams, centerOfPie, ref unExplodedPoints, ref explodedPoints);
			if ((doughnutParams.TagReference as IDataPoint).DpInfo.InternalYValue == 0.0)
			{
				return new List<Shape>();
			}
			shape.SetValue(Panel.ZIndexProperty, 50000);
			shape2.SetValue(Panel.ZIndexProperty, -50000);
			if (doughnutParams.StartAngle != doughnutParams.StopAngle || !doughnutParams.IsLargerArc)
			{
				if (doughnutParams.StartAngle >= 3.1415926535897931 && doughnutParams.StartAngle <= 6.2831853071795862 && doughnutParams.StopAngle >= 3.1415926535897931 && doughnutParams.StopAngle <= 6.2831853071795862 && doughnutParams.IsLargerArc)
				{
					PieChart._elementPositionData.Add(new ElementPositionData(shape3, doughnutParams.StartAngle, doughnutParams.StartAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[0], 0.0, 3.1415926535897931));
					PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[1], doughnutParams.StartAngle, 0.0));
					PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[2], 3.1415926535897931, doughnutParams.StopAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(shape4, doughnutParams.StopAngle, doughnutParams.StopAngle));
					if (labelLinePath != null)
					{
						PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, 0.0, 3.1415926535897931));
					}
				}
				else if (doughnutParams.StartAngle >= 0.0 && doughnutParams.StartAngle <= 3.1415926535897931 && doughnutParams.StopAngle >= 0.0 && doughnutParams.StopAngle <= 3.1415926535897931 && doughnutParams.IsLargerArc)
				{
					PieChart._elementPositionData.Add(new ElementPositionData(shape3, doughnutParams.StartAngle, doughnutParams.StartAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[0], doughnutParams.StartAngle, 3.1415926535897931));
					PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[1], 6.2831853071795862, doughnutParams.StopAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[2], 3.1415926535897931, 6.2831853071795862));
					PieChart._elementPositionData.Add(new ElementPositionData(shape4, doughnutParams.StopAngle, doughnutParams.StopAngle));
					if (labelLinePath != null)
					{
						labelLinePath.SetValue(Panel.ZIndexProperty, -50000);
					}
				}
				else if (doughnutParams.StartAngle >= 0.0 && doughnutParams.StartAngle <= 3.1415926535897931 && doughnutParams.StopAngle >= 3.1415926535897931 && doughnutParams.StopAngle <= 6.2831853071795862)
				{
					PieChart._elementPositionData.Add(new ElementPositionData(shape3, doughnutParams.StartAngle, doughnutParams.StartAngle));
					if (labelLinePath != null)
					{
						PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, doughnutParams.StartAngle, 3.1415926535897931));
					}
					PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[0], doughnutParams.StartAngle, 3.1415926535897931));
					PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[1], 3.1415926535897931, doughnutParams.StopAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(shape4, doughnutParams.StopAngle, doughnutParams.StopAngle));
				}
				else if (doughnutParams.StartAngle >= 3.1415926535897931 && doughnutParams.StartAngle <= 6.2831853071795862 && doughnutParams.StopAngle >= 0.0 && doughnutParams.StopAngle <= 3.1415926535897931)
				{
					PieChart._elementPositionData.Add(new ElementPositionData(shape3, doughnutParams.StartAngle, doughnutParams.StartAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[0], 0.0, doughnutParams.StopAngle));
					PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[1], doughnutParams.StartAngle, 0.0));
					PieChart._elementPositionData.Add(new ElementPositionData(shape4, doughnutParams.StopAngle, doughnutParams.StopAngle));
					if (labelLinePath != null)
					{
						PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, doughnutParams.StopAngle, doughnutParams.StopAngle));
					}
				}
				else
				{
					PieChart._elementPositionData.Add(new ElementPositionData(shape3, doughnutParams.StartAngle, doughnutParams.StartAngle));
					if (doughnutParams.StartAngle >= 0.0 && doughnutParams.StartAngle < 1.5707963267948966 && doughnutParams.StopAngle >= 0.0 && doughnutParams.StopAngle < 1.5707963267948966)
					{
						if (labelLinePath != null)
						{
							PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, doughnutParams.StopAngle, doughnutParams.StopAngle));
						}
						PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[0], doughnutParams.StartAngle, doughnutParams.StopAngle));
					}
					else if (doughnutParams.StartAngle >= 0.0 && doughnutParams.StartAngle < 1.5707963267948966 && doughnutParams.StopAngle >= 1.5707963267948966 && doughnutParams.StopAngle < 3.1415926535897931)
					{
						if (labelLinePath != null)
						{
							labelLinePath.SetValue(Panel.ZIndexProperty, 40000);
						}
						PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[0], doughnutParams.StartAngle, doughnutParams.StopAngle));
					}
					else if (doughnutParams.StartAngle >= 1.5707963267948966 && doughnutParams.StartAngle < 3.1415926535897931 && doughnutParams.StopAngle >= 1.5707963267948966 && doughnutParams.StopAngle < 3.1415926535897931)
					{
						if (labelLinePath != null)
						{
							PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, doughnutParams.StartAngle, doughnutParams.StartAngle));
						}
						PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[0], doughnutParams.StartAngle, doughnutParams.StopAngle));
					}
					else if (doughnutParams.StartAngle >= 3.1415926535897931 && doughnutParams.StartAngle < 4.71238898038469 && doughnutParams.StopAngle >= 3.1415926535897931 && doughnutParams.StopAngle < 4.71238898038469)
					{
						PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[0], doughnutParams.StartAngle, doughnutParams.StopAngle));
						if (labelLinePath != null)
						{
							PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, doughnutParams.StartAngle, doughnutParams.StartAngle));
						}
					}
					else if (doughnutParams.StartAngle >= 4.71238898038469 && doughnutParams.StartAngle < 6.2831853071795862 && doughnutParams.StopAngle >= 4.71238898038469 && doughnutParams.StopAngle < 6.2831853071795862)
					{
						PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[0], doughnutParams.StartAngle, doughnutParams.StopAngle));
						if (labelLinePath != null)
						{
							PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, doughnutParams.StartAngle, doughnutParams.StopAngle));
						}
					}
					else
					{
						if (labelLinePath != null)
						{
							PieChart._elementPositionData.Add(new ElementPositionData(labelLinePath, doughnutParams.StartAngle, doughnutParams.StartAngle));
						}
						PieChart._elementPositionData.Add(new ElementPositionData(doughnutCurvedFace[0], doughnutParams.StartAngle, doughnutParams.StopAngle));
					}
					PieChart._elementPositionData.Add(new ElementPositionData(shape4, doughnutParams.StopAngle, doughnutParams.StopAngle));
				}
			}
			return list;
		}

		private static void SetZIndex(FrameworkElement element, ref int zindex1, ref int zindex2, double angle)
		{
			if (element == null)
			{
				return;
			}
			if (angle >= 0.0 && angle <= 1.5707963267948966)
			{
				element.SetValue(Panel.ZIndexProperty, ++zindex1);
				return;
			}
			if (angle > 1.5707963267948966 && angle <= 3.1415926535897931)
			{
				element.SetValue(Panel.ZIndexProperty, --zindex1);
				return;
			}
			if (angle > 3.1415926535897931 && angle <= 4.71238898038469)
			{
				element.SetValue(Panel.ZIndexProperty, --zindex2);
				return;
			}
			element.SetValue(Panel.ZIndexProperty, ++zindex2);
		}

		private static List<Shape> GetPieOuterCurvedFace(SectorChartShapeParams pieParams, Point3D centroid, Point3D topFaceCenter, Point3D bottomFaceCenter)
		{
			List<Shape> list = new List<Shape>();
			if (pieParams.StartAngle >= 3.1415926535897931 && pieParams.StartAngle <= 6.2831853071795862 && pieParams.StopAngle >= 3.1415926535897931 && pieParams.StopAngle <= 6.2831853071795862 && pieParams.IsLargerArc)
			{
				Path curvedSegment = PieChart.GetCurvedSegment(pieParams, pieParams.OuterRadius, 0.0, 3.1415926535897931, topFaceCenter, bottomFaceCenter, centroid, true, RenderAs.Pie);
				list.Add(curvedSegment);
			}
			else if (pieParams.StartAngle >= 0.0 && pieParams.StartAngle <= 3.1415926535897931 && pieParams.StopAngle >= 0.0 && pieParams.StopAngle <= 3.1415926535897931 && pieParams.IsLargerArc)
			{
				Path curvedSegment2 = PieChart.GetCurvedSegment(pieParams, pieParams.OuterRadius, pieParams.StartAngle, 3.1415926535897931, topFaceCenter, bottomFaceCenter, centroid, true, RenderAs.Pie);
				list.Add(curvedSegment2);
				curvedSegment2 = PieChart.GetCurvedSegment(pieParams, pieParams.OuterRadius, 6.2831853071795862, pieParams.StopAngle, topFaceCenter, bottomFaceCenter, centroid, true, RenderAs.Pie);
				list.Add(curvedSegment2);
			}
			else if (pieParams.StartAngle >= 0.0 && pieParams.StartAngle <= 3.1415926535897931 && pieParams.StopAngle >= 3.1415926535897931 && pieParams.StopAngle <= 6.2831853071795862)
			{
				Path curvedSegment3 = PieChart.GetCurvedSegment(pieParams, pieParams.OuterRadius, pieParams.StartAngle, 3.1415926535897931, topFaceCenter, bottomFaceCenter, centroid, true, RenderAs.Pie);
				list.Add(curvedSegment3);
			}
			else if (pieParams.StartAngle >= 3.1415926535897931 && pieParams.StartAngle <= 6.2831853071795862 && pieParams.StopAngle >= 0.0 && pieParams.StopAngle <= 3.1415926535897931)
			{
				Path curvedSegment4 = PieChart.GetCurvedSegment(pieParams, pieParams.OuterRadius, 0.0, pieParams.StopAngle, topFaceCenter, bottomFaceCenter, centroid, true, RenderAs.Pie);
				list.Add(curvedSegment4);
			}
			else
			{
				Path curvedSegment5 = PieChart.GetCurvedSegment(pieParams, pieParams.OuterRadius, pieParams.StartAngle, pieParams.StopAngle, topFaceCenter, bottomFaceCenter, centroid, true, RenderAs.Pie);
				list.Add(curvedSegment5);
			}
			return list;
		}

		private static Path GetCurvedSegment(SectorChartShapeParams pieParams, double radius, double startAngle, double stopAngle, Point3D topFaceCenter, Point3D bottomFaceCenter, Point3D centroid, bool isOuterSide, RenderAs renderAs)
		{
			Point3D point3D = default(Point3D);
			point3D.X = topFaceCenter.X + radius * Math.Cos(startAngle);
			point3D.Y = topFaceCenter.Y + radius * Math.Sin(startAngle) * pieParams.YAxisScaling;
			point3D.Z = (topFaceCenter.Z + radius) * Math.Sin(startAngle) * Math.Cos(pieParams.TiltAngle) + pieParams.Depth * Math.Cos(1.5707963267948966 - pieParams.TiltAngle);
			Point3D point3D2 = default(Point3D);
			point3D2.X = topFaceCenter.X + radius * Math.Cos(stopAngle);
			point3D2.Y = topFaceCenter.Y + radius * Math.Sin(stopAngle) * pieParams.YAxisScaling;
			point3D2.Z = (topFaceCenter.Z + radius) * Math.Sin(stopAngle) * Math.Cos(pieParams.TiltAngle) + pieParams.Depth * Math.Cos(1.5707963267948966 - pieParams.TiltAngle);
			Point3D point3D3 = default(Point3D);
			point3D3.X = bottomFaceCenter.X + radius * Math.Cos(startAngle);
			point3D3.Y = bottomFaceCenter.Y + radius * Math.Sin(startAngle) * pieParams.YAxisScaling;
			point3D3.Z = (bottomFaceCenter.Z + radius) * Math.Sin(startAngle) * Math.Cos(pieParams.TiltAngle) - pieParams.Depth * Math.Cos(1.5707963267948966 - pieParams.TiltAngle);
			Point3D point3D4 = default(Point3D);
			point3D4.X = bottomFaceCenter.X + radius * Math.Cos(stopAngle);
			point3D4.Y = bottomFaceCenter.Y + radius * Math.Sin(stopAngle) * pieParams.YAxisScaling;
			point3D4.Z = (bottomFaceCenter.Z + radius) * Math.Sin(stopAngle) * Math.Cos(pieParams.TiltAngle) - pieParams.Depth * Math.Cos(1.5707963267948966 - pieParams.TiltAngle);
			Path path = new Path
			{
				Tag = new ElementData
				{
					Element = pieParams.TagReference
				}
			};
			path.Fill = PieChart.Get3DFaceColor(RenderAs.Doughnut, pieParams.Lighting, pieParams.Background, PieFaceTypes.CurvedSurface, startAngle, stopAngle, pieParams.TiltAngle);
			List<PieChart.PathGeometryParams> list = new List<PieChart.PathGeometryParams>();
			bool isLargeArc = Math.Abs(stopAngle - startAngle) > 3.1415926535897931;
			if (stopAngle < startAngle)
			{
				isLargeArc = (Math.Abs(stopAngle + 6.2831853071795862 - startAngle) > 3.1415926535897931);
			}
			list.Add(new PieChart.LineSegmentParams(new Point(point3D2.X, point3D2.Y)));
			list.Add(new PieChart.ArcSegmentParams(new Size(radius, radius * pieParams.YAxisScaling), 0.0, isLargeArc, SweepDirection.Counterclockwise, new Point(point3D.X, point3D.Y)));
			list.Add(new PieChart.LineSegmentParams(new Point(point3D3.X, point3D3.Y)));
			list.Add(new PieChart.ArcSegmentParams(new Size(radius, radius * pieParams.YAxisScaling), 0.0, isLargeArc, SweepDirection.Clockwise, new Point(point3D4.X, point3D4.Y)));
			path.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, new Point(point3D4.X, point3D4.Y), list, true);
			PathFigure pathFigure = (path.Data as PathGeometry).Figures[0];
			PathSegmentCollection arg_378_0 = pathFigure.Segments;
			return path;
		}

		private static Path GetPieFace(SectorChartShapeParams pieParams, Point3D centroid, Point3D center, Point3D arcStart, Point3D arcStop)
		{
			Path path = new Path
			{
				Tag = new ElementData
				{
					Element = pieParams.TagReference
				}
			};
			path.Fill = PieChart.Get3DFaceColor(RenderAs.Pie, pieParams.Lighting, pieParams.Background, PieFaceTypes.Top, double.NaN, double.NaN, double.NaN);
			List<PieChart.PathGeometryParams> list = new List<PieChart.PathGeometryParams>();
			list.Add(new PieChart.LineSegmentParams(new Point(arcStop.X, arcStop.Y)));
			list.Add(new PieChart.ArcSegmentParams(new Size(pieParams.OuterRadius, pieParams.OuterRadius * pieParams.YAxisScaling), 0.0, pieParams.IsLargerArc, SweepDirection.Counterclockwise, new Point(arcStart.X, arcStart.Y)));
			list.Add(new PieChart.LineSegmentParams(new Point(center.X, center.Y)));
			path.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, new Point(center.X, center.Y), list, true);
			PathFigure pathFigure = (path.Data as PathGeometry).Figures[0];
			PathSegmentCollection arg_11E_0 = pathFigure.Segments;
			return path;
		}

		private static Path GetPieSide(SectorChartShapeParams pieParams, Point3D centroid, Point3D centerTop, Point3D centerBottom, Point3D outerTop, Point3D outerBottom)
		{
			Path path = new Path
			{
				Tag = new ElementData
				{
					Element = pieParams.TagReference
				}
			};
			path.Fill = PieChart.Get3DFaceColor(RenderAs.Pie, pieParams.Lighting, pieParams.Background, PieFaceTypes.Left, double.NaN, double.NaN, double.NaN);
			List<PieChart.PathGeometryParams> list = new List<PieChart.PathGeometryParams>();
			list.Add(new PieChart.LineSegmentParams(new Point(centerTop.X, centerTop.Y)));
			list.Add(new PieChart.LineSegmentParams(new Point(outerTop.X, outerTop.Y)));
			list.Add(new PieChart.LineSegmentParams(new Point(outerBottom.X, outerBottom.Y)));
			list.Add(new PieChart.LineSegmentParams(new Point(centerBottom.X, centerBottom.Y)));
			path.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, new Point(centerBottom.X, centerBottom.Y), list, true);
			PathFigure pathFigure = (path.Data as PathGeometry).Figures[0];
			PathSegmentCollection arg_114_0 = pathFigure.Segments;
			return path;
		}

		private static Path GetDoughnutFace(SectorChartShapeParams doughnutParams, Point3D centroid, Point3D arcInnerStart, Point3D arcInnerStop, Point3D arcOuterStart, Point3D arcOuterStop, bool isTopFace)
		{
			Path path = new Path
			{
				Tag = new ElementData
				{
					Element = doughnutParams.TagReference
				}
			};
			path.Fill = PieChart.Get3DFaceColor(RenderAs.Doughnut, doughnutParams.Lighting, doughnutParams.Background, PieFaceTypes.Top, double.NaN, double.NaN, double.NaN);
			List<PieChart.PathGeometryParams> list = new List<PieChart.PathGeometryParams>();
			list.Add(new PieChart.LineSegmentParams(new Point(arcOuterStop.X, arcOuterStop.Y)));
			list.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.OuterRadius, doughnutParams.OuterRadius * doughnutParams.YAxisScaling), 0.0, doughnutParams.IsLargerArc, SweepDirection.Counterclockwise, new Point(arcOuterStart.X, arcOuterStart.Y)));
			list.Add(new PieChart.LineSegmentParams(new Point(arcInnerStart.X, arcInnerStart.Y)));
			list.Add(new PieChart.ArcSegmentParams(new Size(doughnutParams.InnerRadius, doughnutParams.InnerRadius * doughnutParams.YAxisScaling), 0.0, doughnutParams.IsLargerArc, SweepDirection.Clockwise, new Point(arcInnerStop.X, arcInnerStop.Y)));
			path.Data = PieChart.GetPathGeometryFromList(FillRule.Nonzero, new Point(arcInnerStop.X, arcInnerStop.Y), list, true);
			PieChart.GetFaceZIndex(new Point3D[]
			{
				arcInnerStart,
				arcInnerStop,
				arcOuterStart,
				arcOuterStop
			});
			if (isTopFace)
			{
				path.SetValue(Panel.ZIndexProperty, (int)(doughnutParams.Height * 200.0));
			}
			else
			{
				path.SetValue(Panel.ZIndexProperty, (int)(-doughnutParams.Height * 200.0));
			}
			return path;
		}

		private static List<Shape> GetDoughnutCurvedFace(SectorChartShapeParams doughnutParams, Point3D centroid, Point3D topFaceCenter, Point3D bottomFaceCenter)
		{
			List<Shape> list = new List<Shape>();
			if (doughnutParams.StartAngle >= 3.1415926535897931 && doughnutParams.StartAngle <= 6.2831853071795862 && doughnutParams.StopAngle >= 3.1415926535897931 && doughnutParams.StopAngle <= 6.2831853071795862 && doughnutParams.IsLargerArc)
			{
				Path curvedSegment = PieChart.GetCurvedSegment(doughnutParams, doughnutParams.OuterRadius, 0.0, 3.1415926535897931, topFaceCenter, bottomFaceCenter, centroid, true, RenderAs.Doughnut);
				list.Add(curvedSegment);
				curvedSegment = PieChart.GetCurvedSegment(doughnutParams, doughnutParams.InnerRadius, doughnutParams.StartAngle, 0.0, topFaceCenter, bottomFaceCenter, centroid, false, RenderAs.Doughnut);
				list.Add(curvedSegment);
				curvedSegment = PieChart.GetCurvedSegment(doughnutParams, doughnutParams.InnerRadius, 3.1415926535897931, doughnutParams.StopAngle, topFaceCenter, bottomFaceCenter, centroid, false, RenderAs.Doughnut);
				list.Add(curvedSegment);
			}
			else if (doughnutParams.StartAngle >= 0.0 && doughnutParams.StartAngle <= 3.1415926535897931 && doughnutParams.StopAngle >= 0.0 && doughnutParams.StopAngle <= 3.1415926535897931 && doughnutParams.IsLargerArc)
			{
				Path curvedSegment2 = PieChart.GetCurvedSegment(doughnutParams, doughnutParams.OuterRadius, doughnutParams.StartAngle, 3.1415926535897931, topFaceCenter, bottomFaceCenter, centroid, true, RenderAs.Doughnut);
				list.Add(curvedSegment2);
				curvedSegment2 = PieChart.GetCurvedSegment(doughnutParams, doughnutParams.OuterRadius, 6.2831853071795862, doughnutParams.StopAngle, topFaceCenter, bottomFaceCenter, centroid, true, RenderAs.Doughnut);
				list.Add(curvedSegment2);
				curvedSegment2 = PieChart.GetCurvedSegment(doughnutParams, doughnutParams.InnerRadius, 3.1415926535897931, 6.2831853071795862, topFaceCenter, bottomFaceCenter, centroid, false, RenderAs.Doughnut);
				list.Add(curvedSegment2);
			}
			else if (doughnutParams.StartAngle >= 0.0 && doughnutParams.StartAngle <= 3.1415926535897931 && doughnutParams.StopAngle >= 3.1415926535897931 && doughnutParams.StopAngle <= 6.2831853071795862)
			{
				Path curvedSegment3 = PieChart.GetCurvedSegment(doughnutParams, doughnutParams.OuterRadius, doughnutParams.StartAngle, 3.1415926535897931, topFaceCenter, bottomFaceCenter, centroid, true, RenderAs.Doughnut);
				list.Add(curvedSegment3);
				curvedSegment3 = PieChart.GetCurvedSegment(doughnutParams, doughnutParams.InnerRadius, 3.1415926535897931, doughnutParams.StopAngle, topFaceCenter, bottomFaceCenter, centroid, false, RenderAs.Doughnut);
				list.Add(curvedSegment3);
			}
			else if (doughnutParams.StartAngle >= 3.1415926535897931 && doughnutParams.StartAngle <= 6.2831853071795862 && doughnutParams.StopAngle >= 0.0 && doughnutParams.StopAngle <= 3.1415926535897931)
			{
				Path curvedSegment4 = PieChart.GetCurvedSegment(doughnutParams, doughnutParams.OuterRadius, 0.0, doughnutParams.StopAngle, topFaceCenter, bottomFaceCenter, centroid, true, RenderAs.Doughnut);
				list.Add(curvedSegment4);
				curvedSegment4 = PieChart.GetCurvedSegment(doughnutParams, doughnutParams.InnerRadius, doughnutParams.StartAngle, 0.0, topFaceCenter, bottomFaceCenter, centroid, false, RenderAs.Doughnut);
				list.Add(curvedSegment4);
			}
			else if (doughnutParams.StartAngle >= 0.0 && doughnutParams.StopAngle <= 3.1415926535897931)
			{
				Path curvedSegment5 = PieChart.GetCurvedSegment(doughnutParams, doughnutParams.OuterRadius, doughnutParams.StartAngle, doughnutParams.StopAngle, topFaceCenter, bottomFaceCenter, centroid, true, RenderAs.Doughnut);
				list.Add(curvedSegment5);
			}
			else
			{
				Path curvedSegment6 = PieChart.GetCurvedSegment(doughnutParams, doughnutParams.InnerRadius, doughnutParams.StartAngle, doughnutParams.StopAngle, topFaceCenter, bottomFaceCenter, centroid, false, RenderAs.Doughnut);
				list.Add(curvedSegment6);
			}
			return list;
		}

		private static Point3D GetCentroid(params Point3D[] points)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < points.Length; i++)
			{
				Point3D point3D = points[i];
				num += point3D.X;
				num2 += point3D.Y;
				num3 += point3D.Z;
			}
			return new Point3D
			{
				X = num / (double)points.Length,
				Y = num2 / (double)points.Length,
				Z = num3 / (double)points.Length
			};
		}

		private static Point3D GetFaceZIndex(params Point3D[] points)
		{
			return PieChart.GetCentroid(points);
		}

		private static PathGeometry GetPathGeometryFromList(FillRule fillRule, Point startPoint, List<PieChart.PathGeometryParams> pathGeometryParams, bool isClosed)
		{
			PathGeometry pathGeometry = new PathGeometry();
			pathGeometry.FillRule = fillRule;
			PathFigure pathFigure = new PathFigure();
			pathFigure.StartPoint = startPoint;
			pathFigure.IsClosed = isClosed;
			foreach (PieChart.PathGeometryParams current in pathGeometryParams)
			{
				string name;
				if ((name = current.GetType().Name) != null)
				{
					if (!(name == "LineSegmentParams"))
					{
						if (name == "ArcSegmentParams")
						{
							ArcSegment arcSegment = new ArcSegment();
							arcSegment.Point = current.EndPoint;
							arcSegment.IsLargeArc = (current as PieChart.ArcSegmentParams).IsLargeArc;
							arcSegment.RotationAngle = (current as PieChart.ArcSegmentParams).RotationAngle;
							arcSegment.SweepDirection = (current as PieChart.ArcSegmentParams).SweepDirection;
							arcSegment.Size = (current as PieChart.ArcSegmentParams).Size;
							pathFigure.Segments.Add(arcSegment);
						}
					}
					else
					{
						LineSegment lineSegment = new LineSegment();
						lineSegment.Point = current.EndPoint;
						pathFigure.Segments.Add(lineSegment);
					}
				}
			}
			pathGeometry.Figures.Add(pathFigure);
			return pathGeometry;
		}

		private static Brush GetPieGradianceBrush()
		{
			RadialGradientBrush radialGradientBrush = new RadialGradientBrush
			{
				GradientOrigin = new Point(0.5, 0.5)
			};
			radialGradientBrush.GradientStops = new GradientStopCollection();
			radialGradientBrush.GradientStops.Add(new GradientStop
			{
				Offset = 0.0,
				Color = Color.FromArgb(0, 0, 0, 0)
			});
			radialGradientBrush.GradientStops.Add(new GradientStop
			{
				Offset = 0.7,
				Color = Color.FromArgb(34, 0, 0, 0)
			});
			radialGradientBrush.GradientStops.Add(new GradientStop
			{
				Offset = 1.0,
				Color = Color.FromArgb(127, 0, 0, 0)
			});
			return radialGradientBrush;
		}

		private static Brush GetDoughnutGradianceBrush()
		{
			RadialGradientBrush radialGradientBrush = new RadialGradientBrush
			{
				GradientOrigin = new Point(0.5, 0.5)
			};
			radialGradientBrush.GradientStops = new GradientStopCollection();
			radialGradientBrush.GradientStops.Add(new GradientStop
			{
				Offset = 0.0,
				Color = Color.FromArgb(0, 0, 0, 0)
			});
			radialGradientBrush.GradientStops.Add(new GradientStop
			{
				Offset = 0.5,
				Color = Color.FromArgb(127, 0, 0, 0)
			});
			radialGradientBrush.GradientStops.Add(new GradientStop
			{
				Offset = 0.75,
				Color = Color.FromArgb(0, 0, 0, 0)
			});
			radialGradientBrush.GradientStops.Add(new GradientStop
			{
				Offset = 1.0,
				Color = Color.FromArgb(127, 0, 0, 0)
			});
			return radialGradientBrush;
		}

		private static PointCollection GenerateDoubleCollection(params Point[] values)
		{
			PointCollection pointCollection = new PointCollection();
			for (int i = 0; i < values.Length; i++)
			{
				Point value = values[i];
				pointCollection.Add(value);
			}
			return pointCollection;
		}

		private static PointAnimationUsingKeyFrames CreatePointAnimation(DataSeries currentDataSeries, IDataPoint currentDataPoint, DependencyObject target, string property, double beginTime, List<double> frameTimes, List<Point> values, List<KeySpline> splines)
		{
			PointAnimationUsingKeyFrames pointAnimationUsingKeyFrames = new PointAnimationUsingKeyFrames();
			target.SetValue(FrameworkElement.NameProperty, target.GetType().Name + Guid.NewGuid().ToString().Replace('-', '_'));
			Storyboard.SetTargetName(pointAnimationUsingKeyFrames, target.GetValue(FrameworkElement.NameProperty).ToString());
			currentDataSeries.Chart._rootElement.RegisterName((string)target.GetValue(FrameworkElement.NameProperty), target);
			currentDataPoint.Chart._rootElement.RegisterName((string)target.GetValue(FrameworkElement.NameProperty), target);
			Storyboard.SetTargetProperty(pointAnimationUsingKeyFrames, new PropertyPath(property, new object[0]));
			pointAnimationUsingKeyFrames.BeginTime = new TimeSpan?(TimeSpan.FromSeconds(beginTime));
			for (int i = 0; i < splines.Count; i++)
			{
				SplinePointKeyFrame splinePointKeyFrame = new SplinePointKeyFrame();
				splinePointKeyFrame.KeySpline = splines[i];
				splinePointKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(frameTimes[i]));
				splinePointKeyFrame.Value = values[i];
				pointAnimationUsingKeyFrames.KeyFrames.Add(splinePointKeyFrame);
			}
			return pointAnimationUsingKeyFrames;
		}

		private static List<Point> GenerateAnimationPoints(Point center, double radius, double startAngle, double stopAngle)
		{
			List<Point> list = new List<Point>();
			double num = Math.Abs(startAngle - stopAngle) / 100.0;
			if (num <= 0.0)
			{
				return list;
			}
			for (double num2 = startAngle; num2 <= stopAngle; num2 += num)
			{
				list.Add(new Point(center.X + radius * Math.Cos(num2), center.Y + radius * Math.Sin(num2)));
			}
			list.Add(new Point(center.X + radius * Math.Cos(stopAngle), center.Y + radius * Math.Sin(stopAngle)));
			return list;
		}

		private static List<double> GenerateAnimationFrames(int count, double maxTime)
		{
			List<double> list = new List<double>();
			for (int i = 0; i < count; i++)
			{
				list.Add(maxTime * (double)i / (double)(count - 1));
			}
			return list;
		}

		private static Storyboard CreatePathSegmentAnimation(DataSeries currentDataSeries, IDataPoint currentDataPoint, Storyboard storyboard, PathSegment target, Point center, double radius, double startAngle, double stopAngle)
		{
			List<Point> list = PieChart.GenerateAnimationPoints(center, radius, startAngle, stopAngle);
			List<double> frameTimes = PieChart.GenerateAnimationFrames(list.Count, 1.0);
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(list.Count);
			double beginTime = 0.5;
			PointAnimationUsingKeyFrames value;
			if (typeof(ArcSegment).IsInstanceOfType(target))
			{
				value = PieChart.CreatePointAnimation(currentDataSeries, currentDataPoint, target, "(ArcSegment.Point)", beginTime, frameTimes, list, splines);
			}
			else
			{
				value = PieChart.CreatePointAnimation(currentDataSeries, currentDataPoint, target, "(LineSegment.Point)", beginTime, frameTimes, list, splines);
			}
			storyboard.Children.Add(value);
			return storyboard;
		}

		private static Storyboard CreatePathFigureAnimation(DataSeries currentDataSeries, IDataPoint currentDataPoint, Storyboard storyboard, PathFigure target, Point center, double radius, double startAngle, double stopAngle)
		{
			List<Point> list = PieChart.GenerateAnimationPoints(center, radius, startAngle, stopAngle);
			List<double> frameTimes = PieChart.GenerateAnimationFrames(list.Count, 1.0);
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(list.Count);
			double beginTime = 0.5;
			PointAnimationUsingKeyFrames value = PieChart.CreatePointAnimation(currentDataSeries, currentDataPoint, target, "(PathFigure.StartPoint)", beginTime, frameTimes, list, splines);
			storyboard.Children.Add(value);
			return storyboard;
		}

		private static Storyboard CreateLabelLineAnimation(DataSeries currentDataSeries, IDataPoint currentDataPoint, Storyboard storyboard, PathSegment target, params Point[] points)
		{
			List<Point> list = points.ToList<Point>();
			List<double> frameTimes = PieChart.GenerateAnimationFrames(list.Count, 1.0);
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(list.Count);
			double num = 1.0;
			PointAnimationUsingKeyFrames value = PieChart.CreatePointAnimation(currentDataSeries, currentDataPoint, target, "(LineSegment.Point)", num + 0.5, frameTimes, list, splines);
			storyboard.Children.Add(value);
			return storyboard;
		}

		internal static DoubleAnimationUsingKeyFrames CreateDoubleAnimation(DataSeries dataSeries, IDataPoint dataPoint, DependencyObject target, string property, double beginTime, DoubleCollection frameTime, DoubleCollection values, List<KeySpline> splines)
		{
			DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
			target.SetValue(FrameworkElement.NameProperty, target.GetType().Name + Guid.NewGuid().ToString().Replace('-', '_'));
			Storyboard.SetTargetName(doubleAnimationUsingKeyFrames, target.GetValue(FrameworkElement.NameProperty).ToString());
			dataSeries.Chart._rootElement.RegisterName((string)target.GetValue(FrameworkElement.NameProperty), target);
			dataPoint.Chart._rootElement.RegisterName((string)target.GetValue(FrameworkElement.NameProperty), target);
			Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath(property, new object[0]));
			doubleAnimationUsingKeyFrames.BeginTime = new TimeSpan?(TimeSpan.FromSeconds(beginTime));
			for (int i = 0; i < splines.Count; i++)
			{
				SplineDoubleKeyFrame splineDoubleKeyFrame = new SplineDoubleKeyFrame();
				splineDoubleKeyFrame.KeySpline = splines[i];
				splineDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(frameTime[i]));
				splineDoubleKeyFrame.Value = values[i];
				doubleAnimationUsingKeyFrames.KeyFrames.Add(splineDoubleKeyFrame);
			}
			return doubleAnimationUsingKeyFrames;
		}

		private static Storyboard CreateOpacityAnimation(DataSeries currentDataSeries, IDataPoint currentDataPoint, Storyboard storyboard, DependencyObject target, double beginTime, double opacity, double duration)
		{
			DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				opacity
			});
			DoubleCollection doubleCollection = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				duration
			});
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(doubleCollection.Count);
			DoubleAnimationUsingKeyFrames value = PieChart.CreateDoubleAnimation(currentDataSeries, currentDataPoint, target, "(UIElement.Opacity)", beginTime + 0.5, doubleCollection, values, splines);
			storyboard.Children.Add(value);
			return storyboard;
		}

		private static Storyboard CreateLabelLineInteractivityAnimation(DataSeries currentDataSeries, IDataPoint currentDataPoint, Storyboard storyboard, PathSegment target, params Point[] points)
		{
			List<Point> list = points.ToList<Point>();
			List<double> frameTimes = PieChart.GenerateAnimationFrames(list.Count, 0.4);
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(list.Count);
			PointAnimationUsingKeyFrames value = PieChart.CreatePointAnimation(currentDataSeries, currentDataPoint, target, "(LineSegment.Point)", 0.0, frameTimes, list, splines);
			storyboard.Children.Add(value);
			return storyboard;
		}

		private static Storyboard CreateExplodingOut2DAnimation(DataSeries currentDataSeries, IDataPoint dataPoint, double plotWidth, Storyboard storyboard, Panel visual, Panel label, Path labelLine, TranslateTransform translateTransform, PieDoughnut2DPoints unExplodedPoints, PieDoughnut2DPoints explodedPoints, double xOffset, double yOffset)
		{
			DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				xOffset
			});
			DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				0.4
			});
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(0.0, 1.0)
			});
			DoubleAnimationUsingKeyFrames value = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, translateTransform, "(TranslateTransform.X)", 0.0, frameTime, values, splines);
			values = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				yOffset
			});
			frameTime = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				0.4
			});
			splines = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(0.0, 1.0)
			});
			DoubleAnimationUsingKeyFrames value2 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, translateTransform, "(TranslateTransform.Y)", 0.0, frameTime, values, splines);
			storyboard.Children.Add(value);
			storyboard.Children.Add(value2);
			if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle) == LabelStyles.Inside)
			{
				double num = (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
				if (num + xOffset >= 0.0 && num + dataPoint.DpInfo.LabelVisual.Width + xOffset <= plotWidth && label != null)
				{
					translateTransform = new TranslateTransform();
					label.RenderTransform = translateTransform;
					values = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						xOffset
					});
					frameTime = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						0.4
					});
					splines = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 0.0),
						new Point(0.0, 1.0)
					});
					DoubleAnimationUsingKeyFrames value3 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, translateTransform, "(TranslateTransform.X)", 0.0, frameTime, values, splines);
					values = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						yOffset
					});
					frameTime = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						0.4
					});
					splines = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 0.0),
						new Point(0.0, 1.0)
					});
					DoubleAnimationUsingKeyFrames value4 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, translateTransform, "(TranslateTransform.Y)", 0.0, frameTime, values, splines);
					storyboard.Children.Add(value3);
					storyboard.Children.Add(value4);
				}
			}
			else
			{
				values = Graphics.GenerateDoubleCollection(new double[]
				{
					unExplodedPoints.LabelPosition.X,
					explodedPoints.LabelPosition.X
				});
				frameTime = Graphics.GenerateDoubleCollection(new double[]
				{
					0.0,
					0.4
				});
				splines = AnimationHelper.GenerateKeySplineList(new Point[]
				{
					new Point(0.0, 0.0),
					new Point(1.0, 1.0),
					new Point(0.0, 0.0),
					new Point(0.0, 1.0)
				});
				DoubleAnimationUsingKeyFrames value5 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, label, "(Canvas.Left)", 0.0, frameTime, values, splines);
				storyboard.Children.Add(value5);
			}
			if (labelLine != null)
			{
				PathFigure pathFigure = (labelLine.Data as PathGeometry).Figures[0];
				PathSegmentCollection segments = pathFigure.Segments;
				storyboard = PieChart.CreateLabelLineInteractivityAnimation(currentDataSeries, dataPoint, storyboard, segments[0], new Point[]
				{
					unExplodedPoints.LabelLineMidPoint,
					explodedPoints.LabelLineMidPoint
				});
				storyboard = PieChart.CreateLabelLineInteractivityAnimation(currentDataSeries, dataPoint, storyboard, segments[1], new Point[]
				{
					unExplodedPoints.LabelLineEndPoint,
					explodedPoints.LabelLineEndPoint
				});
			}
			return storyboard;
		}

		private static Storyboard CreateExplodingIn2DAnimation(DataSeries currentDataSeries, IDataPoint dataPoint, double plotWidth, Storyboard storyboard, Panel visual, Panel label, Path labelLine, TranslateTransform translateTransform, PieDoughnut2DPoints unExplodedPoints, PieDoughnut2DPoints explodedPoints, double xOffset, double yOffset)
		{
			double[] array = new double[2];
			array[0] = xOffset;
			DoubleCollection values = Graphics.GenerateDoubleCollection(array);
			DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				0.4
			});
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(0.0, 1.0)
			});
			DoubleAnimationUsingKeyFrames value = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, translateTransform, "(TranslateTransform.X)", 0.0, frameTime, values, splines);
			double[] array2 = new double[2];
			array2[0] = yOffset;
			values = Graphics.GenerateDoubleCollection(array2);
			frameTime = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				0.4
			});
			splines = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(0.0, 1.0)
			});
			DoubleAnimationUsingKeyFrames value2 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, translateTransform, "(TranslateTransform.Y)", 0.0, frameTime, values, splines);
			storyboard.Children.Add(value);
			storyboard.Children.Add(value2);
			if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle) == LabelStyles.Inside)
			{
				double num = (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
				if (num + xOffset >= 0.0 && num + dataPoint.DpInfo.LabelVisual.Width + xOffset <= plotWidth && label != null)
				{
					translateTransform = (label.RenderTransform as TranslateTransform);
					double[] array3 = new double[2];
					array3[0] = xOffset;
					values = Graphics.GenerateDoubleCollection(array3);
					frameTime = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						0.4
					});
					splines = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 0.0),
						new Point(0.0, 1.0)
					});
					DoubleAnimationUsingKeyFrames value3 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, translateTransform, "(TranslateTransform.X)", 0.0, frameTime, values, splines);
					double[] array4 = new double[2];
					array4[0] = yOffset;
					values = Graphics.GenerateDoubleCollection(array4);
					frameTime = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						0.4
					});
					splines = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 0.0),
						new Point(0.0, 1.0)
					});
					DoubleAnimationUsingKeyFrames value4 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, translateTransform, "(TranslateTransform.Y)", 0.0, frameTime, values, splines);
					storyboard.Children.Add(value3);
					storyboard.Children.Add(value4);
				}
			}
			else
			{
				values = Graphics.GenerateDoubleCollection(new double[]
				{
					explodedPoints.LabelPosition.X,
					unExplodedPoints.LabelPosition.X
				});
				frameTime = Graphics.GenerateDoubleCollection(new double[]
				{
					0.0,
					0.4
				});
				splines = AnimationHelper.GenerateKeySplineList(new Point[]
				{
					new Point(0.0, 0.0),
					new Point(1.0, 1.0),
					new Point(0.0, 0.0),
					new Point(0.0, 1.0)
				});
				DoubleAnimationUsingKeyFrames value5 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, label, "(Canvas.Left)", 0.0, frameTime, values, splines);
				storyboard.Children.Add(value5);
			}
			if (labelLine != null)
			{
				PathFigure pathFigure = (labelLine.Data as PathGeometry).Figures[0];
				PathSegmentCollection segments = pathFigure.Segments;
				storyboard = PieChart.CreateLabelLineInteractivityAnimation(currentDataSeries, dataPoint, storyboard, segments[0], new Point[]
				{
					explodedPoints.LabelLineMidPoint,
					unExplodedPoints.LabelLineMidPoint
				});
				storyboard = PieChart.CreateLabelLineInteractivityAnimation(currentDataSeries, dataPoint, storyboard, segments[1], new Point[]
				{
					explodedPoints.LabelLineEndPoint,
					unExplodedPoints.LabelLineEndPoint
				});
			}
			return storyboard;
		}

		private static Storyboard CreateExplodingOut3DAnimation(DataSeries currentDataSeries, IDataPoint dataPoint, double plotWidth, Storyboard storyboard, List<Shape> pathElements, Panel label, Path labelLine, PieDoughnut3DPoints unExplodedPoints, PieDoughnut3DPoints explodedPoints, double xOffset, double yOffset)
		{
			foreach (Shape current in pathElements)
			{
				if (current != null)
				{
					TranslateTransform target = current.RenderTransform as TranslateTransform;
					DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						xOffset
					});
					DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						0.4
					});
					List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 0.0),
						new Point(0.0, 1.0)
					});
					DoubleAnimationUsingKeyFrames value = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, target, "(TranslateTransform.X)", 0.0, frameTime, values, splines);
					values = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						yOffset
					});
					frameTime = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						0.4
					});
					splines = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 0.0),
						new Point(0.0, 1.0)
					});
					DoubleAnimationUsingKeyFrames value2 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, target, "(TranslateTransform.Y)", 0.0, frameTime, values, splines);
					storyboard.Children.Add(value);
					storyboard.Children.Add(value2);
				}
			}
			if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle) == LabelStyles.Inside)
			{
				double num = (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
				if (num + xOffset >= 0.0 && num + dataPoint.DpInfo.LabelVisual.Width + xOffset <= plotWidth && label != null)
				{
					TranslateTransform translateTransform = new TranslateTransform();
					label.RenderTransform = translateTransform;
					DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						xOffset
					});
					DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						0.4
					});
					List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 0.0),
						new Point(0.0, 1.0)
					});
					DoubleAnimationUsingKeyFrames value3 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, translateTransform, "(TranslateTransform.X)", 0.0, frameTime, values, splines);
					values = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						yOffset
					});
					frameTime = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						0.4
					});
					splines = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 0.0),
						new Point(0.0, 1.0)
					});
					DoubleAnimationUsingKeyFrames value4 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, translateTransform, "(TranslateTransform.Y)", 0.0, frameTime, values, splines);
					storyboard.Children.Add(value3);
					storyboard.Children.Add(value4);
				}
			}
			else
			{
				DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
				{
					unExplodedPoints.LabelPosition.X,
					explodedPoints.LabelPosition.X
				});
				DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
				{
					0.0,
					0.4
				});
				List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
				{
					new Point(0.0, 0.0),
					new Point(1.0, 1.0),
					new Point(0.0, 0.0),
					new Point(0.0, 1.0)
				});
				DoubleAnimationUsingKeyFrames value5 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, label, "(Canvas.Left)", 0.0, frameTime, values, splines);
				storyboard.Children.Add(value5);
			}
			if (labelLine != null)
			{
				TranslateTransform target2 = labelLine.RenderTransform as TranslateTransform;
				DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
				{
					0.0,
					xOffset
				});
				DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
				{
					0.0,
					0.4
				});
				List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
				{
					new Point(0.0, 0.0),
					new Point(1.0, 1.0),
					new Point(0.0, 0.0),
					new Point(0.0, 1.0)
				});
				DoubleAnimationUsingKeyFrames value6 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, target2, "(TranslateTransform.X)", 0.0, frameTime, values, splines);
				values = Graphics.GenerateDoubleCollection(new double[]
				{
					0.0,
					yOffset
				});
				frameTime = Graphics.GenerateDoubleCollection(new double[]
				{
					0.0,
					0.4
				});
				splines = AnimationHelper.GenerateKeySplineList(new Point[]
				{
					new Point(0.0, 0.0),
					new Point(1.0, 1.0),
					new Point(0.0, 0.0),
					new Point(0.0, 1.0)
				});
				DoubleAnimationUsingKeyFrames value7 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, target2, "(TranslateTransform.Y)", 0.0, frameTime, values, splines);
				storyboard.Children.Add(value6);
				storyboard.Children.Add(value7);
				PathFigure pathFigure = (labelLine.Data as PathGeometry).Figures[0];
				PathSegmentCollection segments = pathFigure.Segments;
				storyboard = PieChart.CreateLabelLineInteractivityAnimation(currentDataSeries, dataPoint, storyboard, segments[0], new Point[]
				{
					unExplodedPoints.LabelLineMidPoint,
					explodedPoints.LabelLineMidPoint
				});
				storyboard = PieChart.CreateLabelLineInteractivityAnimation(currentDataSeries, dataPoint, storyboard, segments[1], new Point[]
				{
					unExplodedPoints.LabelLineEndPoint,
					explodedPoints.LabelLineEndPoint
				});
			}
			return storyboard;
		}

		private static Storyboard CreateExplodingIn3DAnimation(DataSeries currentDataSeries, IDataPoint dataPoint, double plotWidth, Storyboard storyboard, List<Shape> pathElements, Panel label, Path labelLine, PieDoughnut3DPoints unExplodedPoints, PieDoughnut3DPoints explodedPoints, double xOffset, double yOffset)
		{
			if (storyboard != null && storyboard.GetValue(Storyboard.TargetProperty) != null)
			{
				storyboard.Stop();
			}
			foreach (Shape current in pathElements)
			{
				if (current != null)
				{
					TranslateTransform target = current.RenderTransform as TranslateTransform;
					double[] array = new double[2];
					array[0] = xOffset;
					DoubleCollection values = Graphics.GenerateDoubleCollection(array);
					DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						0.4
					});
					List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 0.0),
						new Point(0.0, 1.0)
					});
					DoubleAnimationUsingKeyFrames value = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, target, "(TranslateTransform.X)", 0.0, frameTime, values, splines);
					double[] array2 = new double[2];
					array2[0] = yOffset;
					values = Graphics.GenerateDoubleCollection(array2);
					frameTime = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						0.4
					});
					splines = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 0.0),
						new Point(0.0, 1.0)
					});
					DoubleAnimationUsingKeyFrames value2 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, target, "(TranslateTransform.Y)", 0.0, frameTime, values, splines);
					storyboard.Children.Add(value);
					storyboard.Children.Add(value2);
				}
			}
			if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle) == LabelStyles.Inside)
			{
				double num = (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
				if (num + xOffset >= 0.0 && num + dataPoint.DpInfo.LabelVisual.Width + xOffset <= plotWidth && label != null)
				{
					TranslateTransform target2 = label.RenderTransform as TranslateTransform;
					double[] array3 = new double[2];
					array3[0] = xOffset;
					DoubleCollection values = Graphics.GenerateDoubleCollection(array3);
					DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						0.4
					});
					List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 0.0),
						new Point(0.0, 1.0)
					});
					DoubleAnimationUsingKeyFrames value3 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, target2, "(TranslateTransform.X)", 0.0, frameTime, values, splines);
					double[] array4 = new double[2];
					array4[0] = yOffset;
					values = Graphics.GenerateDoubleCollection(array4);
					frameTime = Graphics.GenerateDoubleCollection(new double[]
					{
						0.0,
						0.4
					});
					splines = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 0.0),
						new Point(0.0, 1.0)
					});
					DoubleAnimationUsingKeyFrames value4 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, target2, "(TranslateTransform.Y)", 0.0, frameTime, values, splines);
					storyboard.Children.Add(value3);
					storyboard.Children.Add(value4);
				}
			}
			else
			{
				DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
				{
					explodedPoints.LabelPosition.X,
					unExplodedPoints.LabelPosition.X
				});
				DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
				{
					0.0,
					0.4
				});
				List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
				{
					new Point(0.0, 0.0),
					new Point(1.0, 1.0),
					new Point(0.0, 0.0),
					new Point(0.0, 1.0)
				});
				DoubleAnimationUsingKeyFrames value5 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, label, "(Canvas.Left)", 0.0, frameTime, values, splines);
				storyboard.Children.Add(value5);
			}
			if (labelLine != null)
			{
				TranslateTransform target3 = labelLine.RenderTransform as TranslateTransform;
				double[] array5 = new double[2];
				array5[0] = xOffset;
				DoubleCollection values = Graphics.GenerateDoubleCollection(array5);
				DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
				{
					0.0,
					0.4
				});
				List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
				{
					new Point(0.0, 0.0),
					new Point(1.0, 1.0),
					new Point(0.0, 0.0),
					new Point(0.0, 1.0)
				});
				DoubleAnimationUsingKeyFrames value6 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, target3, "(TranslateTransform.X)", 0.0, frameTime, values, splines);
				double[] array6 = new double[2];
				array6[0] = yOffset;
				values = Graphics.GenerateDoubleCollection(array6);
				frameTime = Graphics.GenerateDoubleCollection(new double[]
				{
					0.0,
					0.4
				});
				splines = AnimationHelper.GenerateKeySplineList(new Point[]
				{
					new Point(0.0, 0.0),
					new Point(1.0, 1.0),
					new Point(0.0, 0.0),
					new Point(0.0, 1.0)
				});
				DoubleAnimationUsingKeyFrames value7 = PieChart.CreateDoubleAnimation(currentDataSeries, dataPoint, target3, "(TranslateTransform.Y)", 0.0, frameTime, values, splines);
				storyboard.Children.Add(value6);
				storyboard.Children.Add(value7);
				PathFigure pathFigure = (labelLine.Data as PathGeometry).Figures[0];
				PathSegmentCollection segments = pathFigure.Segments;
				storyboard = PieChart.CreateLabelLineInteractivityAnimation(currentDataSeries, dataPoint, storyboard, segments[0], new Point[]
				{
					explodedPoints.LabelLineMidPoint,
					unExplodedPoints.LabelLineMidPoint
				});
				storyboard = PieChart.CreateLabelLineInteractivityAnimation(currentDataSeries, dataPoint, storyboard, segments[1], new Point[]
				{
					explodedPoints.LabelLineEndPoint,
					unExplodedPoints.LabelLineEndPoint
				});
			}
			return storyboard;
		}

		private static void Create3DPie(DataSeries currentDataSeries, double widthOfPlotArea, double height, DataSeries series, List<IDataPoint> enabledDataPoints, IDataPoint dataPoint, ref Canvas visual, ref Faces faces, ref SectorChartShapeParams pieParams, ref double offsetX, ref int zindex, bool isAnimationEnabled)
		{
			PieDoughnut3DPoints pieDoughnut3DPoints = new PieDoughnut3DPoints();
			PieDoughnut3DPoints pieDoughnut3DPoints2 = new PieDoughnut3DPoints();
			pieParams.TagReference = dataPoint;
			List<Shape> pie3D = PieChart.GetPie3D(currentDataSeries, ref visual, ref faces, pieParams, ref zindex, ref pieDoughnut3DPoints, ref pieDoughnut3DPoints2, ref dataPoint.DpInfo.LabelLine, enabledDataPoints);
			foreach (Shape current in pie3D)
			{
				if (current != null)
				{
					visual.Children.Add(current);
					faces.VisualComponents.Add(current);
					faces.BorderElements.Add(current);
					current.RenderTransform = new TranslateTransform();
					if (isAnimationEnabled)
					{
						double num = 0.0;
						if (dataPoint.IsLightDataPoint)
						{
							series.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, dataPoint, series.Storyboard, current, 1.0 / (double)series.InternalDataPoints.Count * (double)series.InternalDataPoints.IndexOf(dataPoint) + num, (dataPoint as LightDataPoint).Parent.InternalOpacity, 0.5);
						}
						else
						{
							series.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, dataPoint, series.Storyboard, current, 1.0 / (double)series.InternalDataPoints.Count * (double)series.InternalDataPoints.IndexOf(dataPoint) + num, (dataPoint as DataPoint).InternalOpacity, 0.5);
						}
						current.Opacity = 0.0;
					}
				}
			}
			if (dataPoint.DpInfo.LabelLine != null && pieParams.LabelLineEnabled)
			{
				dataPoint.DpInfo.LabelLine.RenderTransform = new TranslateTransform();
				if (dataPoint.DpInfo.InternalYValue == 0.0)
				{
					Line line = new Line();
					line.X1 = pieParams.Center.X;
					line.Y1 = pieParams.Center.Y;
					line.X2 = pieDoughnut3DPoints.LabelLineStartPoint.X;
					line.Y2 = pieDoughnut3DPoints.LabelLineStartPoint.Y;
					line.Stroke = pieParams.LabelLineColor;
					line.StrokeThickness = 0.25;
					line.IsHitTestVisible = false;
					visual.Children.Add(line);
					if (isAnimationEnabled)
					{
						double beginTime = 2.0;
						series.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, dataPoint, series.Storyboard, line, beginTime, line.Opacity, 0.5);
						line.Opacity = 0.0;
					}
				}
				visual.Children.Add(dataPoint.DpInfo.LabelLine);
				faces.VisualComponents.Add(dataPoint.DpInfo.LabelLine);
			}
			faces.Visual = visual;
			PieChart.UpdateExplodedPosition(pieParams, dataPoint, offsetX, pieDoughnut3DPoints, pieDoughnut3DPoints2, widthOfPlotArea);
			pieParams.ExplodedPoints = pieDoughnut3DPoints2;
			pieParams.UnExplodedPoints = pieDoughnut3DPoints;
			if ((dataPoint.Chart as Chart).AnimatedUpdate.Value)
			{
				dataPoint.DpInfo.ExplodeAnimation = new Storyboard();
				dataPoint.DpInfo.ExplodeAnimation = PieChart.CreateExplodingOut3DAnimation(currentDataSeries, dataPoint, widthOfPlotArea, dataPoint.DpInfo.ExplodeAnimation, pie3D, dataPoint.DpInfo.LabelVisual as Canvas, dataPoint.DpInfo.LabelLine, pieDoughnut3DPoints, pieDoughnut3DPoints2, pieParams.OffsetX, pieParams.OffsetY);
				dataPoint.DpInfo.UnExplodeAnimation = new Storyboard();
				dataPoint.DpInfo.UnExplodeAnimation = PieChart.CreateExplodingIn3DAnimation(currentDataSeries, dataPoint, widthOfPlotArea, dataPoint.DpInfo.UnExplodeAnimation, pie3D, dataPoint.DpInfo.LabelVisual as Canvas, dataPoint.DpInfo.LabelLine, pieDoughnut3DPoints, pieDoughnut3DPoints2, pieParams.OffsetX, pieParams.OffsetY);
				return;
			}
			if (!(dataPoint.Chart as Chart).InternalAnimationEnabled && dataPoint.Exploded.Value)
			{
				foreach (Shape current2 in pie3D)
				{
					if (current2 != null)
					{
						(current2.RenderTransform as TranslateTransform).X = pieParams.OffsetX;
						(current2.RenderTransform as TranslateTransform).Y = pieParams.OffsetY;
					}
				}
				if (dataPoint.DpInfo.LabelVisual != null)
				{
					if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle) == LabelStyles.Inside)
					{
						TranslateTransform translateTransform = new TranslateTransform();
						(dataPoint.DpInfo.LabelVisual as Canvas).RenderTransform = translateTransform;
						translateTransform.X = pieParams.OffsetX;
						translateTransform.Y = pieParams.OffsetY;
					}
					else
					{
						(dataPoint.DpInfo.LabelVisual as Canvas).SetValue(Canvas.LeftProperty, pieDoughnut3DPoints2.LabelPosition.X);
					}
				}
				if (dataPoint.DpInfo.LabelLine != null)
				{
					(dataPoint.DpInfo.LabelLine.RenderTransform as TranslateTransform).X = pieParams.OffsetX;
					(dataPoint.DpInfo.LabelLine.RenderTransform as TranslateTransform).Y = pieParams.OffsetY;
					PathFigure pathFigure = (dataPoint.DpInfo.LabelLine.Data as PathGeometry).Figures[0];
					PathSegmentCollection segments = pathFigure.Segments;
					(segments[0] as LineSegment).Point = pieDoughnut3DPoints2.LabelLineMidPoint;
					(segments[1] as LineSegment).Point = pieDoughnut3DPoints2.LabelLineEndPoint;
				}
				dataPoint.DpInfo.InteractiveExplodeState = true;
			}
		}

		private static void Create2DPie(DataSeries currentDataSeries, double widthOfPlotArea, double height, DataSeries series, List<IDataPoint> enabledDataPoints, IDataPoint dataPoint, ref Canvas visual, ref Faces faces, ref SectorChartShapeParams pieParams, ref double offsetX, ref double offsetY, ref int zindex, bool isAnimationEnabled, int labelStateCount)
		{
			PieDoughnut2DPoints unExplodedPoints = new PieDoughnut2DPoints();
			PieDoughnut2DPoints pieDoughnut2DPoints = new PieDoughnut2DPoints();
			pieParams.TagReference = dataPoint;
			if (labelStateCount == enabledDataPoints.Count)
			{
				pieParams.OuterRadius -= pieParams.OuterRadius * pieParams.ExplodeRatio;
			}
			Canvas pie2D = PieChart.GetPie2D(currentDataSeries, ref faces, pieParams, ref unExplodedPoints, ref pieDoughnut2DPoints, ref dataPoint.DpInfo.LabelLine, enabledDataPoints);
			PieChart.UpdateExplodedPosition(pieParams, dataPoint, offsetX, unExplodedPoints, pieDoughnut2DPoints, widthOfPlotArea);
			pieParams.ExplodedPoints = pieDoughnut2DPoints;
			pieParams.UnExplodedPoints = unExplodedPoints;
			TranslateTransform translateTransform = new TranslateTransform();
			pie2D.RenderTransform = translateTransform;
			if ((currentDataSeries.Chart as Chart).AnimatedUpdate.Value)
			{
				dataPoint.DpInfo.ExplodeAnimation = new Storyboard();
				dataPoint.DpInfo.ExplodeAnimation = PieChart.CreateExplodingOut2DAnimation(currentDataSeries, dataPoint, widthOfPlotArea, dataPoint.DpInfo.ExplodeAnimation, pie2D, dataPoint.DpInfo.LabelVisual as Canvas, dataPoint.DpInfo.LabelLine, translateTransform, unExplodedPoints, pieDoughnut2DPoints, offsetX, offsetY);
				dataPoint.DpInfo.UnExplodeAnimation = new Storyboard();
				dataPoint.DpInfo.UnExplodeAnimation = PieChart.CreateExplodingIn2DAnimation(currentDataSeries, dataPoint, widthOfPlotArea, dataPoint.DpInfo.UnExplodeAnimation, pie2D, dataPoint.DpInfo.LabelVisual as Canvas, dataPoint.DpInfo.LabelLine, translateTransform, unExplodedPoints, pieDoughnut2DPoints, offsetX, offsetY);
			}
			else if (!(currentDataSeries.Chart as Chart).InternalAnimationEnabled && dataPoint.Exploded.Value)
			{
				translateTransform.X = offsetX;
				translateTransform.Y = offsetY;
				if (dataPoint.DpInfo.LabelVisual != null)
				{
					if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle) == LabelStyles.Inside)
					{
						translateTransform = new TranslateTransform();
						(dataPoint.DpInfo.LabelVisual as Canvas).RenderTransform = translateTransform;
						translateTransform.X = offsetX;
						translateTransform.Y = offsetY;
					}
					else
					{
						(dataPoint.DpInfo.LabelVisual as Canvas).SetValue(Canvas.LeftProperty, pieDoughnut2DPoints.LabelPosition.X);
					}
				}
				if (dataPoint.DpInfo.LabelLine != null)
				{
					PathFigure pathFigure = (dataPoint.DpInfo.LabelLine.Data as PathGeometry).Figures[0];
					PathSegmentCollection segments = pathFigure.Segments;
					(segments[0] as LineSegment).Point = pieDoughnut2DPoints.LabelLineMidPoint;
					(segments[1] as LineSegment).Point = pieDoughnut2DPoints.LabelLineEndPoint;
				}
				dataPoint.DpInfo.InteractiveExplodeState = true;
			}
			pie2D.SetValue(Canvas.TopProperty, height / 2.0 - pie2D.Height / 2.0);
			pie2D.SetValue(Canvas.LeftProperty, widthOfPlotArea / 2.0 - pie2D.Width / 2.0);
			visual.Children.Add(pie2D);
			faces.VisualComponents.Add(pie2D);
			faces.Visual = pie2D;
		}

		internal static Brush GetLighterBevelBrush(Brush brush, double angle)
		{
			return Graphics.GetBevelTopBrush(brush, angle);
		}

		internal static Brush GetDarkerBevelBrush(Brush brush, double angle)
		{
			return Graphics.GetLightingEnabledBrush(brush, "Linear", new double[]
			{
				0.35,
				0.65
			});
		}

		internal static Brush GetCurvedBevelBrush(Brush brush, double angle, DoubleCollection shade, DoubleCollection offset)
		{
			if (typeof(SolidColorBrush).Equals(brush.GetType()))
			{
				SolidColorBrush solidColorBrush = brush as SolidColorBrush;
				List<Color> list = new List<Color>();
				List<double> list2 = new List<double>();
				for (int i = 0; i < shade.Count; i++)
				{
					Color item = (shade[i] < 0.0) ? Graphics.GetDarkerColor(solidColorBrush.Color, Math.Abs(shade[i])) : Graphics.GetLighterColor(solidColorBrush.Color, Math.Abs(shade[i]));
					list.Add(item);
				}
				for (int j = 0; j < offset.Count; j++)
				{
					list2.Add(offset[j]);
				}
				return Graphics.CreateLinearGradientBrush(angle, new Point(0.0, 0.5), new Point(1.0, 0.5), list, list2);
			}
			return brush;
		}

		internal static Size UpdatePieSize(Size sizeOfPie, Canvas visual, IDataPoint dataPoint, double left, double top, double width, double height)
		{
			sizeOfPie = new Size
			{
				Width = sizeOfPie.Width,
				Height = sizeOfPie.Height
			};
			double width2 = sizeOfPie.Width;
			double num = width / 2.0;
			double arg_49_0 = height / 2.0;
			double num2 = (double)visual.GetValue(Canvas.LeftProperty);
			double arg_6F_0 = sizeOfPie.Width / 2.0;
			if (left < num)
			{
				left = (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
				if (left <= num2)
				{
					double num3 = Math.Abs((num2 - left) * 2.0);
					if (width2 > num3)
					{
						sizeOfPie.Width = Math.Abs(sizeOfPie.Width - num3);
					}
					sizeOfPie.Height = Math.Abs(sizeOfPie.Width);
					if (sizeOfPie.Width < width / 4.0)
					{
						sizeOfPie.Width = width / 4.0;
						sizeOfPie.Height = sizeOfPie.Width;
					}
				}
			}
			else
			{
				left = (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty) + dataPoint.DpInfo.LabelVisual.Width;
				if (left >= width)
				{
					double num3 = Math.Abs((left - width) * 2.0);
					if (width2 > num3)
					{
						sizeOfPie.Width = Math.Abs(sizeOfPie.Width - num3);
					}
					sizeOfPie.Height = Math.Abs(sizeOfPie.Width);
					if (sizeOfPie.Width < width / 4.0)
					{
						sizeOfPie.Width = width / 4.0;
						sizeOfPie.Height = sizeOfPie.Width;
					}
				}
			}
			return sizeOfPie;
		}

		internal static Canvas GetVisualObjectForPieChart(double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, bool isAnimationEnabled)
		{
			if (double.IsNaN(width) || double.IsNaN(height) || width <= 0.0 || height <= 0.0)
			{
				return null;
			}
			DataSeries currentDataSeries = null;
			Canvas canvas = new Canvas
			{
				Width = width,
				Height = height
			};
			double arg_56_0 = (double)canvas.GetValue(Canvas.LeftProperty);
			DataSeries dataSeries = seriesList[0];
			dataSeries.Faces = null;
			if (dataSeries.Enabled == false)
			{
				return canvas;
			}
			List<IDataPoint> list = (from datapoint in dataSeries.InternalDataPoints
			where datapoint.Enabled == true && !double.IsNaN(datapoint.DpInfo.InternalYValue)
			select datapoint).ToList<IDataPoint>();
			if ((from dp in list
			select dp.DpInfo.InternalYValue).Sum() == 0.0)
			{
				list.Clear();
			}
			double num = plotDetails.GetAbsoluteSumOfDataPoints(list);
			num = ((num == 0.0) ? 1.0 : num);
			double x = width / 2.0;
			double y = height / 2.0;
			double offsetX = 0.0;
			double num2 = 0.0;
			Size size = default(Size);
			Canvas canvas2 = PieChart.CreateAndPositionLabels(num, list, width, height, chart.View3D ? 0.4 : 1.0, chart.View3D, ref size);
			bool flag;
			if (canvas2 == null)
			{
				flag = false;
			}
			else
			{
				flag = true;
				canvas2.SetValue(Panel.ZIndexProperty, 50001);
			}
			if (flag && list.Count > 0)
			{
				List<double> list2 = new List<double>();
				List<double> list3 = new List<double>();
				foreach (IDataPoint current in list)
				{
					list2.Add(current.DpInfo.LabelVisual.Width);
					list3.Add(current.DpInfo.LabelVisual.Height);
				}
				double num3 = list2.Max();
				double num4 = list3.Max();
				if (!chart.View3D && num3 > 36.0)
				{
					double num5 = Math.Abs(size.Width) + 18.0;
					double value = num5;
					if (!chart.SmartLabelEnabled)
					{
						PieChart.PositionLabels(canvas, num, list, size, new Size(Math.Abs(num5), Math.Abs(value)), new Size(width, height), chart.View3D ? 0.4 : 1.0, chart.View3D);
					}
					foreach (IDataPoint current2 in list)
					{
						double top = (double)current2.DpInfo.LabelVisual.GetValue(Canvas.TopProperty) + current2.DpInfo.LabelVisual.Height / 2.0;
						double left = (double)current2.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
						size = PieChart.UpdatePieSize(size, canvas, current2, left, top, width, height);
					}
					num5 = Math.Abs(size.Width) + 18.0;
					value = num5;
					PieChart.PositionLabels(canvas, num, list, size, new Size(Math.Abs(num5), Math.Abs(value)), new Size(width, height), chart.View3D ? 0.4 : 1.0, chart.View3D);
					size.Width -= ((size.Width > 10.0) ? (size.Width * 0.1) : 0.0);
					size.Height -= ((size.Height > 10.0) ? (size.Height * 0.1) : 0.0);
				}
				else if (chart.View3D)
				{
					if (height < size.Height + num4 * 2.0 + 36.0)
					{
						double num6 = size.Height + num4 * 2.0 + 36.0 - height;
						double num7 = size.Height - num6;
						if (num7 < Math.Min(width, height) / 4.0)
						{
							num7 = Math.Min(width, height) / 4.0;
						}
						double num8 = num7;
						double num9 = num8 * 2.0 + 36.0;
						double value2 = num9 * 0.4 + 36.0;
						PieChart.PositionLabels(canvas, num, list, new Size(Math.Abs(num8), Math.Abs(num7)), new Size(Math.Abs(num9), Math.Abs(value2)), new Size(width, height), chart.View3D ? 0.4 : 1.0, chart.View3D);
						num8 -= ((num8 > 10.0) ? (num8 * 0.1) : 0.0);
						num7 -= ((num7 > 10.0) ? (num7 * 0.1) : 0.0);
						size = new Size(Math.Abs(num8), Math.Abs(num7));
					}
					else if (width < size.Width + num3 * 2.0 + 36.0)
					{
						double num10 = size.Width + num3 * 2.0 + 36.0 - width;
						double num11 = size.Width - num10;
						if (num11 < Math.Min(width, height) / 4.0)
						{
							num11 = Math.Min(width, height) / 4.0;
						}
						double num12 = num11;
						double num13 = num11 * 2.0 + 36.0;
						double value3 = num13 * 0.4 + 36.0;
						PieChart.PositionLabels(canvas, num, list, new Size(Math.Abs(num11), Math.Abs(num12)), new Size(Math.Abs(num13), Math.Abs(value3)), new Size(width, height), chart.View3D ? 0.4 : 1.0, chart.View3D);
						num11 -= ((num11 > 10.0) ? (num11 * 0.1) : 0.0);
						num12 -= ((num12 > 10.0) ? (num12 * 0.1) : 0.0);
						size = new Size(Math.Abs(num11), Math.Abs(num12));
					}
				}
			}
			double num14 = Math.Min(size.Width, size.Height) / (double)(chart.View3D ? 1 : 2);
			double num15 = dataSeries.InternalStartAngle;
			int num16 = 0;
			if (chart.View3D)
			{
				PieChart._elementPositionData = new List<ElementPositionData>();
			}
			if (dataSeries.Storyboard == null)
			{
				dataSeries.Storyboard = new Storyboard();
			}
			currentDataSeries = dataSeries;
			SectorChartShapeParams sectorChartShapeParams = null;
			int num17 = 0;
			if (!chart.View3D)
			{
				foreach (IDataPoint current3 in list)
				{
					if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(current3, VcProperties.LabelStyle) == LabelStyles.Inside || !(bool)DataPointHelper.GetDataPointValueFromProperty(current3, VcProperties.LabelEnabled))
					{
						num17++;
					}
				}
			}
			double num18 = -1.7976931348623157E+308;
			foreach (IDataPoint current4 in list)
			{
				double internalYValue = current4.DpInfo.InternalYValue;
				if (!double.IsNaN(internalYValue))
				{
					num18 = Math.Max(num18, Math.Abs(internalYValue));
				}
			}
			double num19 = num18 * 0.2 / 100.0;
			foreach (IDataPoint current5 in list)
			{
				double internalYValue2 = current5.DpInfo.InternalYValue;
				if (!double.IsNaN(internalYValue2) && Math.Abs(internalYValue2) < num19)
				{
					num += num19;
				}
			}
			foreach (IDataPoint current6 in list)
			{
				double internalYValue3 = current6.DpInfo.InternalYValue;
				if (!double.IsNaN(internalYValue3))
				{
					double num20 = Math.Abs(internalYValue3);
					if (num20 < num19)
					{
						num20 = num19;
					}
					double num21 = num20 / num;
					double num22 = num21 * 3.1415926535897931 * 2.0;
					double num23 = num15 + num22;
					double num24 = (num15 + num23) / 2.0;
					sectorChartShapeParams = new SectorChartShapeParams();
					current6.DpInfo.VisualParams = sectorChartShapeParams;
					sectorChartShapeParams.Storyboard = dataSeries.Storyboard;
					sectorChartShapeParams.AnimationEnabled = isAnimationEnabled;
					sectorChartShapeParams.Center = new Point(x, y);
					sectorChartShapeParams.ExplodeRatio = (chart.View3D ? 0.2 : 0.1);
					sectorChartShapeParams.InnerRadius = 0.0;
					sectorChartShapeParams.OuterRadius = num14;
					sectorChartShapeParams.DataPoint = current6;
					if (chart.View3D)
					{
						sectorChartShapeParams.StartAngle = sectorChartShapeParams.FixAngle(num15 % 6.2831853071795862);
						sectorChartShapeParams.StopAngle = sectorChartShapeParams.FixAngle(num23 % 6.2831853071795862);
					}
					else
					{
						sectorChartShapeParams.StartAngle = num15;
						sectorChartShapeParams.StopAngle = num23;
					}
					sectorChartShapeParams.Lighting = (bool)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LightingEnabled);
					sectorChartShapeParams.Bevel = dataSeries.Bevel.Value;
					sectorChartShapeParams.IsLargerArc = (num22 / 3.1415926535897931 > 1.0);
					sectorChartShapeParams.Background = current6.Color;
					sectorChartShapeParams.Width = width;
					sectorChartShapeParams.Height = height;
					sectorChartShapeParams.TiltAngle = Math.Asin(0.4);
					sectorChartShapeParams.Depth = 20.0 / sectorChartShapeParams.YAxisScaling;
					sectorChartShapeParams.MeanAngle = num24;
					sectorChartShapeParams.LabelLineEnabled = (bool)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LabelLineEnabled);
					sectorChartShapeParams.LabelLineColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LabelLineColor);
					sectorChartShapeParams.LabelLineThickness = (double)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LabelLineThickness);
					sectorChartShapeParams.LabelLineStyle = ExtendedGraphics.GetDashArray((LineStyles)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LabelLineStyle));
					sectorChartShapeParams.IsZero = (current6.DpInfo.InternalYValue == 0.0);
					offsetX = num14 * sectorChartShapeParams.ExplodeRatio * Math.Cos(num24);
					num2 = num14 * sectorChartShapeParams.ExplodeRatio * Math.Sin(num24);
					sectorChartShapeParams.OffsetX = offsetX;
					sectorChartShapeParams.OffsetY = num2 * (chart.View3D ? sectorChartShapeParams.YAxisScaling : 1.0);
					if (current6.DpInfo.LabelVisual != null)
					{
						if (current6.DpInfo.LabelVisual.Visibility == Visibility.Collapsed)
						{
							sectorChartShapeParams.LabelLineEnabled = false;
						}
						double num25 = (double)current6.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
						if (num25 < width / 2.0)
						{
							sectorChartShapeParams.LabelLineTargetToRight = true;
							sectorChartShapeParams.LabelPoint = new Point(num25 + current6.DpInfo.LabelVisual.Width + LabelPlacementHelper.LABEL_LINE_GAP, (double)current6.DpInfo.LabelVisual.GetValue(Canvas.TopProperty) + current6.DpInfo.LabelVisual.Height / 2.0);
							Point point = default(Point);
							point.X = num25;
							point.Y = (double)current6.DpInfo.LabelVisual.GetValue(Canvas.TopProperty) + current6.DpInfo.LabelVisual.Height / 2.0;
						}
						else
						{
							sectorChartShapeParams.LabelLineTargetToRight = false;
							sectorChartShapeParams.LabelPoint = new Point(num25 - LabelPlacementHelper.LABEL_LINE_GAP, (double)current6.DpInfo.LabelVisual.GetValue(Canvas.TopProperty) + current6.DpInfo.LabelVisual.Height / 2.0);
							Point point2 = default(Point);
							point2.X = num25 + current6.DpInfo.LabelVisual.Width - LabelPlacementHelper.LABEL_LINE_GAP;
							point2.Y = (double)current6.DpInfo.LabelVisual.GetValue(Canvas.TopProperty) + current6.DpInfo.LabelVisual.Height / 2.0;
						}
						if (isAnimationEnabled)
						{
							double beginTime = 2.0;
							if (current6.IsLightDataPoint)
							{
								dataSeries.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, current6, dataSeries.Storyboard, current6.DpInfo.LabelVisual, beginTime, current6.Parent.InternalOpacity, 0.5);
							}
							else
							{
								dataSeries.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, current6, dataSeries.Storyboard, current6.DpInfo.LabelVisual, beginTime, (current6 as DataPoint).InternalOpacity * current6.Parent.InternalOpacity, 0.5);
							}
							current6.DpInfo.LabelVisual.Opacity = 0.0;
						}
					}
					if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LabelStyle) == LabelStyles.Inside && internalYValue3 == 0.0)
					{
						sectorChartShapeParams.LabelLineEnabled = false;
					}
					Faces faces = new Faces();
					if (chart.View3D)
					{
						PieChart.Create3DPie(currentDataSeries, width, height, dataSeries, list, current6, ref canvas, ref faces, ref sectorChartShapeParams, ref offsetX, ref num16, isAnimationEnabled);
					}
					else
					{
						PieChart.Create2DPie(currentDataSeries, width, height, dataSeries, list, current6, ref canvas, ref faces, ref sectorChartShapeParams, ref offsetX, ref num2, ref num16, isAnimationEnabled, num17);
					}
					current6.DpInfo.Faces = faces;
					num15 = num23;
					if ((!chart.InternalAnimationEnabled || chart.IsInDesignMode || !chart.ChartArea._isFirstTimeRender) && current6.DpInfo.Faces != null)
					{
						foreach (Shape current7 in current6.DpInfo.Faces.BorderElements)
						{
							if (current6.IsLightDataPoint)
							{
								InteractivityHelper.ApplyBorderEffect(current7, (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.BorderStyle), (current6 as LightDataPoint).Parent.BorderThickness.Left, (Brush)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.BorderColor));
							}
							else
							{
								InteractivityHelper.ApplyBorderEffect(current7, (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.BorderStyle), (current6 as DataPoint).InternalBorderThickness.Left, (Brush)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.BorderColor));
							}
						}
					}
				}
			}
			if (chart.View3D)
			{
				PieChart._elementPositionData.Sort(new Comparison<ElementPositionData>(ElementPositionData.CompareAngle));
				int num26 = 1000;
				int num27 = -1000;
				for (int i = 0; i < PieChart._elementPositionData.Count; i++)
				{
					PieChart.SetZIndex(PieChart._elementPositionData[i].Element, ref num26, ref num27, PieChart._elementPositionData[i].StartAngle);
				}
			}
			if (flag && canvas2 != null)
			{
				canvas.Children.Add(canvas2);
			}
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			rectangleGeometry.Rect = new Rect(0.0, 0.0, width, height);
			canvas.Clip = rectangleGeometry;
			return canvas;
		}

		internal static void UpdateExplodedPosition(SectorChartShapeParams pieParams, IDataPoint dataPoint, double offsetX, PieDoughnutPoints unExplodedPoints, PieDoughnutPoints explodedPoints, double widthOfPlotArea)
		{
			if (dataPoint.DpInfo.LabelVisual != null)
			{
				unExplodedPoints.LabelPosition = new Point((double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty), (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.TopProperty));
				double num = (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
				if (num < widthOfPlotArea / 2.0)
				{
					if (unExplodedPoints.LabelPosition.X + offsetX < 0.0)
					{
						explodedPoints.LabelPosition = new Point(unExplodedPoints.LabelPosition.X, unExplodedPoints.LabelPosition.Y);
						explodedPoints.LabelLineEndPoint = new Point(explodedPoints.LabelLineEndPoint.X - offsetX, explodedPoints.LabelLineEndPoint.Y);
						explodedPoints.LabelLineMidPoint = new Point(explodedPoints.LabelLineMidPoint.X - offsetX, explodedPoints.LabelLineMidPoint.Y);
						return;
					}
					explodedPoints.LabelPosition = new Point(unExplodedPoints.LabelPosition.X + offsetX, unExplodedPoints.LabelPosition.Y);
					return;
				}
				else
				{
					if (unExplodedPoints.LabelPosition.X + offsetX + dataPoint.DpInfo.LabelVisual.Width > widthOfPlotArea)
					{
						explodedPoints.LabelPosition = new Point(unExplodedPoints.LabelPosition.X, unExplodedPoints.LabelPosition.Y);
						explodedPoints.LabelLineEndPoint = new Point(explodedPoints.LabelLineEndPoint.X - offsetX, explodedPoints.LabelLineEndPoint.Y);
						explodedPoints.LabelLineMidPoint = new Point(explodedPoints.LabelLineMidPoint.X - offsetX, explodedPoints.LabelLineMidPoint.Y);
						return;
					}
					explodedPoints.LabelPosition = new Point(unExplodedPoints.LabelPosition.X + offsetX, unExplodedPoints.LabelPosition.Y);
				}
			}
		}

		internal static Canvas GetVisualObjectForDoughnutChart(double widthOfPlotArea, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, bool animationEnabled)
		{
			if (double.IsNaN(widthOfPlotArea) || double.IsNaN(height) || widthOfPlotArea <= 0.0 || height <= 0.0)
			{
				return null;
			}
			DataSeries currentDataSeries = null;
			Canvas canvas = new Canvas();
			canvas.Width = widthOfPlotArea;
			canvas.Height = height;
			DataSeries dataSeries = seriesList[0];
			dataSeries.Faces = null;
			if (dataSeries.Enabled == false)
			{
				return canvas;
			}
			List<IDataPoint> list = (from datapoint in dataSeries.InternalDataPoints
			where datapoint.Enabled == true && !double.IsNaN(datapoint.DpInfo.InternalYValue)
			select datapoint).ToList<IDataPoint>();
			if ((from dp in list
			select dp.DpInfo.InternalYValue).Sum() == 0.0)
			{
				list.Clear();
			}
			double num = plotDetails.GetAbsoluteSumOfDataPoints(list);
			num = ((num == 0.0) ? 1.0 : num);
			double x = widthOfPlotArea / 2.0;
			double y = height / 2.0;
			double num2 = 0.0;
			double num3 = 0.0;
			Size size = default(Size);
			Canvas canvas2 = PieChart.CreateAndPositionLabels(num, list, widthOfPlotArea, height, chart.View3D ? 0.4 : 1.0, chart.View3D, ref size);
			bool flag;
			if (canvas2 == null)
			{
				flag = false;
			}
			else
			{
				flag = true;
				canvas2.SetValue(Panel.ZIndexProperty, 50001);
			}
			if (flag && list.Count > 0)
			{
				List<double> list2 = new List<double>();
				List<double> list3 = new List<double>();
				foreach (IDataPoint current in list)
				{
					list2.Add(current.DpInfo.LabelVisual.Width);
					list3.Add(current.DpInfo.LabelVisual.Height);
				}
				double num4 = list2.Max();
				double num5 = list3.Max();
				if (!chart.View3D && num4 > 36.0)
				{
					double num6 = Math.Abs(size.Width) + 18.0;
					double value = num6;
					if (!chart.SmartLabelEnabled)
					{
						PieChart.PositionLabels(canvas, num, list, size, new Size(Math.Abs(num6), Math.Abs(value)), new Size(widthOfPlotArea, height), chart.View3D ? 0.4 : 1.0, chart.View3D);
					}
					foreach (IDataPoint current2 in list)
					{
						double top = (double)current2.DpInfo.LabelVisual.GetValue(Canvas.TopProperty) + current2.DpInfo.LabelVisual.Height / 2.0;
						double left = (double)current2.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
						size = PieChart.UpdatePieSize(size, canvas, current2, left, top, widthOfPlotArea, height);
					}
					num6 = Math.Abs(size.Width) + 18.0;
					value = num6;
					PieChart.PositionLabels(canvas, num, list, size, new Size(Math.Abs(num6), Math.Abs(value)), new Size(widthOfPlotArea, height), chart.View3D ? 0.4 : 1.0, chart.View3D);
					size.Width -= ((size.Width > 10.0) ? (size.Width * 0.1) : 0.0);
					size.Height -= ((size.Height > 10.0) ? (size.Height * 0.1) : 0.0);
				}
				else if (chart.View3D)
				{
					double num7 = 2.0;
					if (height < size.Height + num5 * 2.0 + 18.0 * num7)
					{
						double num8 = size.Height + num5 * 2.0 + 18.0 * num7 - height;
						double num9 = size.Height - num8;
						if (num9 < Math.Min(widthOfPlotArea, height) / 4.0)
						{
							num9 = Math.Min(widthOfPlotArea, height) / 4.0;
						}
						double num10 = num9;
						double num11 = num10 * 2.0 + 36.0;
						double value2 = num11 * 0.4 + 36.0;
						PieChart.PositionLabels(canvas, num, list, new Size(Math.Abs(num10), Math.Abs(num9)), new Size(Math.Abs(num11), Math.Abs(value2)), new Size(widthOfPlotArea, height), chart.View3D ? 0.4 : 1.0, chart.View3D);
						num10 -= ((num10 > 10.0) ? (num10 * 0.1) : 0.0);
						num9 -= ((num9 > 10.0) ? (num10 * 0.1) : 0.0);
						size = new Size(Math.Abs(num10), Math.Abs(num9));
					}
					else if (widthOfPlotArea < size.Width + num4 * 2.0 + 18.0 * num7)
					{
						double num12 = size.Width + num4 * 2.0 + 18.0 * num7 - widthOfPlotArea;
						double num13 = size.Width - num12;
						if (num13 < Math.Min(widthOfPlotArea, height) / 4.0)
						{
							num13 = Math.Min(widthOfPlotArea, height) / 4.0;
						}
						double num14 = num13;
						double num15 = num13 * 2.0 + 36.0;
						double value3 = num15 * 0.4 + 36.0;
						PieChart.PositionLabels(canvas, num, list, new Size(Math.Abs(num13), Math.Abs(num14)), new Size(Math.Abs(num15), Math.Abs(value3)), new Size(widthOfPlotArea, height), chart.View3D ? 0.4 : 1.0, chart.View3D);
						num13 -= ((num13 > 10.0) ? (num13 * 0.1) : 0.0);
						num14 -= ((num14 > 10.0) ? (num13 * 0.1) : 0.0);
						size = new Size(Math.Abs(num13), Math.Abs(num14));
					}
				}
			}
			double num16 = Math.Min(size.Width, size.Height) / (double)(chart.View3D ? 1 : 2);
			double num17 = dataSeries.InternalStartAngle;
			double num18 = 0.0;
			IEnumerable<IDataPoint> source = from datapoint in dataSeries.InternalDataPoints
			where datapoint.Exploded == true && datapoint.DpInfo.InternalYValue != 0.0
			select datapoint;
			source.Any<IDataPoint>();
			if (chart.View3D)
			{
				PieChart._elementPositionData = new List<ElementPositionData>();
			}
			if (canvas2 != null)
			{
				canvas2.SetValue(Panel.ZIndexProperty, 50001);
			}
			if (dataSeries.Storyboard == null)
			{
				dataSeries.Storyboard = new Storyboard();
			}
			currentDataSeries = dataSeries;
			SectorChartShapeParams sectorChartShapeParams = null;
			int num19 = 0;
			if (!chart.View3D)
			{
				foreach (IDataPoint current3 in list)
				{
					if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(current3, VcProperties.LabelStyle) == LabelStyles.Inside || !(bool)DataPointHelper.GetDataPointValueFromProperty(current3, VcProperties.LabelEnabled))
					{
						num19++;
					}
				}
			}
			double num20 = -1.7976931348623157E+308;
			foreach (IDataPoint current4 in list)
			{
				double internalYValue = current4.DpInfo.InternalYValue;
				if (!double.IsNaN(internalYValue))
				{
					num20 = Math.Max(num20, Math.Abs(internalYValue));
				}
			}
			double num21 = num20 * 0.2 / 100.0;
			foreach (IDataPoint current5 in list)
			{
				double internalYValue2 = current5.DpInfo.InternalYValue;
				if (!double.IsNaN(internalYValue2) && Math.Abs(internalYValue2) < num21)
				{
					num += num21;
				}
			}
			foreach (IDataPoint current6 in list)
			{
				double internalYValue3 = current6.DpInfo.InternalYValue;
				if (!double.IsNaN(internalYValue3))
				{
					double num22 = Math.Abs(internalYValue3);
					if (num22 < num21)
					{
						num22 = num21;
					}
					double num23 = num22 / num;
					double num24 = num23 * 3.1415926535897931 * 2.0;
					num18 = num17 + num24;
					double num25 = (num17 + num18) / 2.0;
					sectorChartShapeParams = new SectorChartShapeParams();
					current6.DpInfo.VisualParams = sectorChartShapeParams;
					sectorChartShapeParams.AnimationEnabled = animationEnabled;
					sectorChartShapeParams.Storyboard = dataSeries.Storyboard;
					sectorChartShapeParams.ExplodeRatio = (chart.View3D ? 0.2 : 0.1);
					sectorChartShapeParams.Center = new Point(x, y);
					sectorChartShapeParams.DataPoint = current6;
					sectorChartShapeParams.InnerRadius = num16 / 2.0;
					sectorChartShapeParams.OuterRadius = num16;
					if (chart.View3D)
					{
						sectorChartShapeParams.StartAngle = sectorChartShapeParams.FixAngle(num17 % 6.2831853071795862);
						sectorChartShapeParams.StopAngle = sectorChartShapeParams.FixAngle(num18 % 6.2831853071795862);
					}
					else
					{
						sectorChartShapeParams.StartAngle = num17;
						sectorChartShapeParams.StopAngle = num18;
					}
					sectorChartShapeParams.Lighting = (bool)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LightingEnabled);
					sectorChartShapeParams.Bevel = dataSeries.Bevel.Value;
					sectorChartShapeParams.IsLargerArc = (num24 / 3.1415926535897931 > 1.0);
					sectorChartShapeParams.Background = current6.Color;
					sectorChartShapeParams.Width = widthOfPlotArea;
					sectorChartShapeParams.Height = height;
					sectorChartShapeParams.TiltAngle = Math.Asin(0.4);
					sectorChartShapeParams.Depth = 20.0 / sectorChartShapeParams.YAxisScaling;
					sectorChartShapeParams.MeanAngle = num25;
					sectorChartShapeParams.LabelLineEnabled = (bool)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LabelLineEnabled);
					sectorChartShapeParams.LabelLineColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LabelLineColor);
					sectorChartShapeParams.LabelLineThickness = (double)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LabelLineThickness);
					sectorChartShapeParams.LabelLineStyle = ExtendedGraphics.GetDashArray((LineStyles)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LabelLineStyle));
					num2 = num16 * sectorChartShapeParams.ExplodeRatio * Math.Cos(num25);
					num3 = num16 * sectorChartShapeParams.ExplodeRatio * Math.Sin(num25);
					sectorChartShapeParams.OffsetX = num2;
					sectorChartShapeParams.OffsetY = num3 * (chart.View3D ? sectorChartShapeParams.YAxisScaling : 1.0);
					if (current6.DpInfo.LabelVisual != null)
					{
						if (current6.DpInfo.LabelVisual.Visibility == Visibility.Collapsed)
						{
							sectorChartShapeParams.LabelLineEnabled = false;
						}
						double num26 = (double)current6.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
						if (num26 < widthOfPlotArea / 2.0)
						{
							sectorChartShapeParams.LabelLineTargetToRight = true;
							sectorChartShapeParams.LabelPoint = new Point(num26 + current6.DpInfo.LabelVisual.Width + LabelPlacementHelper.LABEL_LINE_GAP, (double)current6.DpInfo.LabelVisual.GetValue(Canvas.TopProperty) + current6.DpInfo.LabelVisual.Height / 2.0);
						}
						else
						{
							sectorChartShapeParams.LabelLineTargetToRight = false;
							sectorChartShapeParams.LabelPoint = new Point(num26 - LabelPlacementHelper.LABEL_LINE_GAP, (double)current6.DpInfo.LabelVisual.GetValue(Canvas.TopProperty) + current6.DpInfo.LabelVisual.Height / 2.0);
						}
						if (animationEnabled)
						{
							double beginTime = 2.0;
							dataSeries.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, sectorChartShapeParams.DataPoint, dataSeries.Storyboard, current6.DpInfo.LabelVisual, beginTime, 1.0, 0.5);
							current6.DpInfo.LabelVisual.Opacity = 0.0;
						}
					}
					if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LabelStyle) == LabelStyles.Inside && current6.DpInfo.InternalYValue == 0.0)
					{
						sectorChartShapeParams.LabelLineEnabled = false;
					}
					Faces faces = new Faces();
					faces.Parts = new List<DependencyObject>();
					sectorChartShapeParams.TagReference = current6;
					if (chart.View3D)
					{
						PieDoughnut3DPoints pieDoughnut3DPoints = new PieDoughnut3DPoints();
						PieDoughnut3DPoints pieDoughnut3DPoints2 = new PieDoughnut3DPoints();
						List<Shape> doughnut3D = PieChart.GetDoughnut3D(currentDataSeries, ref faces, sectorChartShapeParams, ref pieDoughnut3DPoints, ref pieDoughnut3DPoints2, ref current6.DpInfo.LabelLine, list);
						foreach (Shape current7 in doughnut3D)
						{
							if (current7 != null)
							{
								canvas.Children.Add(current7);
								faces.VisualComponents.Add(current7);
								faces.BorderElements.Add(current7);
								current7.RenderTransform = new TranslateTransform();
								if (animationEnabled)
								{
									double num27 = 0.0;
									if (current6.IsLightDataPoint)
									{
										dataSeries.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, sectorChartShapeParams.DataPoint, dataSeries.Storyboard, current7, 1.0 / (double)dataSeries.InternalDataPoints.Count * (double)dataSeries.InternalDataPoints.IndexOf(current6) + num27, (current6 as LightDataPoint).Parent.InternalOpacity, 0.5);
									}
									else
									{
										dataSeries.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, sectorChartShapeParams.DataPoint, dataSeries.Storyboard, current7, 1.0 / (double)dataSeries.InternalDataPoints.Count * (double)dataSeries.InternalDataPoints.IndexOf(current6) + num27, (current6 as DataPoint).InternalOpacity, 0.5);
									}
									current7.Opacity = 0.0;
								}
							}
						}
						if (current6.DpInfo.LabelLine != null && sectorChartShapeParams.LabelLineEnabled)
						{
							current6.DpInfo.LabelLine.RenderTransform = new TranslateTransform();
							canvas.Children.Add(current6.DpInfo.LabelLine);
							if (current6.DpInfo.InternalYValue == 0.0)
							{
								double arg_F4E_0 = sectorChartShapeParams.YAxisScaling;
								Line line = new Line();
								line.X1 = sectorChartShapeParams.Center.X + sectorChartShapeParams.InnerRadius * Math.Cos(sectorChartShapeParams.MeanAngle);
								line.Y1 = sectorChartShapeParams.Center.Y + sectorChartShapeParams.InnerRadius * Math.Sin(sectorChartShapeParams.MeanAngle);
								line.Y1 -= num3;
								line.Y1 += sectorChartShapeParams.Depth / 2.0 * sectorChartShapeParams.ZAxisScaling;
								line.X2 = pieDoughnut3DPoints.LabelLineStartPoint.X;
								line.Y2 = pieDoughnut3DPoints.LabelLineStartPoint.Y;
								line.Stroke = sectorChartShapeParams.LabelLineColor;
								line.StrokeThickness = 0.25;
								line.IsHitTestVisible = false;
								canvas.Children.Add(line);
								if (animationEnabled)
								{
									double beginTime2 = 2.0;
									dataSeries.Storyboard = PieChart.CreateOpacityAnimation(currentDataSeries, sectorChartShapeParams.DataPoint, dataSeries.Storyboard, line, beginTime2, line.Opacity, 0.5);
									line.Opacity = 0.0;
								}
							}
							faces.VisualComponents.Add(current6.DpInfo.LabelLine);
						}
						faces.Visual = canvas;
						PieChart.UpdateExplodedPosition(sectorChartShapeParams, current6, num2, pieDoughnut3DPoints, pieDoughnut3DPoints2, widthOfPlotArea);
						sectorChartShapeParams.ExplodedPoints = pieDoughnut3DPoints2;
						sectorChartShapeParams.UnExplodedPoints = pieDoughnut3DPoints;
						if (chart.AnimatedUpdate.Value)
						{
							current6.DpInfo.ExplodeAnimation = new Storyboard();
							current6.DpInfo.ExplodeAnimation = PieChart.CreateExplodingOut3DAnimation(currentDataSeries, current6, widthOfPlotArea, current6.DpInfo.ExplodeAnimation, doughnut3D, current6.DpInfo.LabelVisual as Canvas, current6.DpInfo.LabelLine, pieDoughnut3DPoints, pieDoughnut3DPoints2, sectorChartShapeParams.OffsetX, sectorChartShapeParams.OffsetY);
							current6.DpInfo.UnExplodeAnimation = new Storyboard();
							current6.DpInfo.UnExplodeAnimation = PieChart.CreateExplodingIn3DAnimation(currentDataSeries, current6, widthOfPlotArea, current6.DpInfo.UnExplodeAnimation, doughnut3D, current6.DpInfo.LabelVisual as Canvas, current6.DpInfo.LabelLine, pieDoughnut3DPoints, pieDoughnut3DPoints2, sectorChartShapeParams.OffsetX, sectorChartShapeParams.OffsetY);
						}
						else if (!chart.InternalAnimationEnabled && current6.Exploded.Value)
						{
							foreach (Shape current8 in doughnut3D)
							{
								if (current8 != null)
								{
									(current8.RenderTransform as TranslateTransform).X = num2;
									(current8.RenderTransform as TranslateTransform).Y = num3;
								}
							}
							if (current6.DpInfo.LabelVisual != null)
							{
								if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LabelStyle) == LabelStyles.Inside)
								{
									TranslateTransform translateTransform = new TranslateTransform();
									(current6.DpInfo.LabelVisual as Canvas).RenderTransform = translateTransform;
									translateTransform.X = num2;
									translateTransform.Y = num3;
								}
								else
								{
									(current6.DpInfo.LabelVisual as Canvas).SetValue(Canvas.LeftProperty, pieDoughnut3DPoints2.LabelPosition.X);
								}
							}
							if (current6.DpInfo.LabelLine != null)
							{
								(current6.DpInfo.LabelLine.RenderTransform as TranslateTransform).X = num2;
								(current6.DpInfo.LabelLine.RenderTransform as TranslateTransform).Y = num3;
								PathFigure pathFigure = (current6.DpInfo.LabelLine.Data as PathGeometry).Figures[0];
								PathSegmentCollection segments = pathFigure.Segments;
								(segments[0] as LineSegment).Point = pieDoughnut3DPoints2.LabelLineMidPoint;
								(segments[1] as LineSegment).Point = pieDoughnut3DPoints2.LabelLineEndPoint;
							}
							current6.DpInfo.InteractiveExplodeState = true;
						}
					}
					else
					{
						PieDoughnut2DPoints unExplodedPoints = new PieDoughnut2DPoints();
						PieDoughnut2DPoints pieDoughnut2DPoints = new PieDoughnut2DPoints();
						if (num19 == list.Count)
						{
							sectorChartShapeParams.OuterRadius -= sectorChartShapeParams.OuterRadius * sectorChartShapeParams.ExplodeRatio;
							sectorChartShapeParams.InnerRadius = sectorChartShapeParams.OuterRadius / 2.0;
						}
						Canvas doughnut2D = PieChart.GetDoughnut2D(currentDataSeries, ref faces, sectorChartShapeParams, ref unExplodedPoints, ref pieDoughnut2DPoints, ref current6.DpInfo.LabelLine, list);
						PieChart.UpdateExplodedPosition(sectorChartShapeParams, current6, num2, unExplodedPoints, pieDoughnut2DPoints, widthOfPlotArea);
						sectorChartShapeParams.ExplodedPoints = pieDoughnut2DPoints;
						sectorChartShapeParams.UnExplodedPoints = unExplodedPoints;
						TranslateTransform translateTransform2 = new TranslateTransform();
						doughnut2D.RenderTransform = translateTransform2;
						if (chart.AnimatedUpdate.Value)
						{
							current6.DpInfo.ExplodeAnimation = new Storyboard();
							current6.DpInfo.ExplodeAnimation = PieChart.CreateExplodingOut2DAnimation(currentDataSeries, current6, widthOfPlotArea, current6.DpInfo.ExplodeAnimation, doughnut2D, current6.DpInfo.LabelVisual as Canvas, current6.DpInfo.LabelLine, translateTransform2, unExplodedPoints, pieDoughnut2DPoints, num2, num3);
							current6.DpInfo.UnExplodeAnimation = new Storyboard();
							current6.DpInfo.UnExplodeAnimation = PieChart.CreateExplodingIn2DAnimation(currentDataSeries, current6, widthOfPlotArea, current6.DpInfo.UnExplodeAnimation, doughnut2D, current6.DpInfo.LabelVisual as Canvas, current6.DpInfo.LabelLine, translateTransform2, unExplodedPoints, pieDoughnut2DPoints, num2, num3);
						}
						else if (!chart.InternalAnimationEnabled && current6.Exploded.Value)
						{
							translateTransform2.X = num2;
							translateTransform2.Y = num3;
							if (current6.DpInfo.LabelVisual != null)
							{
								if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.LabelStyle) == LabelStyles.Inside)
								{
									translateTransform2 = new TranslateTransform();
									(current6.DpInfo.LabelVisual as Canvas).RenderTransform = translateTransform2;
									translateTransform2.X = num2;
									translateTransform2.Y = num3;
								}
								else
								{
									(current6.DpInfo.LabelVisual as Canvas).SetValue(Canvas.LeftProperty, pieDoughnut2DPoints.LabelPosition.X);
								}
							}
							if (current6.DpInfo.LabelLine != null)
							{
								PathFigure pathFigure2 = (current6.DpInfo.LabelLine.Data as PathGeometry).Figures[0];
								PathSegmentCollection segments2 = pathFigure2.Segments;
								(segments2[0] as LineSegment).Point = pieDoughnut2DPoints.LabelLineMidPoint;
								(segments2[1] as LineSegment).Point = pieDoughnut2DPoints.LabelLineEndPoint;
							}
							current6.DpInfo.InteractiveExplodeState = true;
						}
						doughnut2D.SetValue(Canvas.TopProperty, height / 2.0 - doughnut2D.Height / 2.0);
						doughnut2D.SetValue(Canvas.LeftProperty, widthOfPlotArea / 2.0 - doughnut2D.Width / 2.0);
						canvas.Children.Add(doughnut2D);
						faces.VisualComponents.Add(doughnut2D);
						faces.Visual = doughnut2D;
					}
					current6.DpInfo.Faces = faces;
					num17 = num18;
					if ((!chart.InternalAnimationEnabled || chart.IsInDesignMode || !chart.ChartArea._isFirstTimeRender) && current6.DpInfo.Faces != null)
					{
						foreach (Shape current9 in current6.DpInfo.Faces.BorderElements)
						{
							if (current6.IsLightDataPoint)
							{
								InteractivityHelper.ApplyBorderEffect(current9, (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.BorderStyle), (current6 as LightDataPoint).Parent.BorderThickness.Left, (Brush)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.BorderColor));
							}
							else
							{
								InteractivityHelper.ApplyBorderEffect(current9, (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.BorderStyle), (current6 as DataPoint).InternalBorderThickness.Left, (Brush)DataPointHelper.GetDataPointValueFromProperty(current6, VcProperties.BorderColor));
							}
						}
					}
				}
			}
			if (chart.View3D)
			{
				PieChart._elementPositionData.Sort(new Comparison<ElementPositionData>(ElementPositionData.CompareAngle));
				int num28 = 1000;
				int num29 = -1000;
				for (int i = 0; i < PieChart._elementPositionData.Count; i++)
				{
					PieChart.SetZIndex(PieChart._elementPositionData[i].Element, ref num28, ref num29, PieChart._elementPositionData[i].StartAngle);
				}
			}
			if (canvas2 != null)
			{
				canvas.Children.Add(canvas2);
			}
			canvas.Clip = new RectangleGeometry
			{
				Rect = new Rect(0.0, 0.0, widthOfPlotArea, height)
			};
			return canvas;
		}
	}
}
