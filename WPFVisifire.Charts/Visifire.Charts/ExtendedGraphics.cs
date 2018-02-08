using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class ExtendedGraphics
	{
		private enum Corners
		{
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight
		}

		private enum Directions
		{
			Top,
			Left,
			Right,
			Bottom
		}

		internal static void GetBrushesForPlank(Chart chart, out Brush frontBrush, out Brush topBrush, out Brush rightBrush, bool zeroPlank)
		{
			List<Color> list = new List<Color>();
			if (zeroPlank)
			{
				list.Add(Colors.White);
				list.Add(Color.FromArgb(255, 223, 223, 223));
				frontBrush = Graphics.CreateLinearGradientBrush(0.0, new Point(0.5, 1.0), new Point(0.5, 0.0), list, new List<double>
				{
					0.0,
					1.0
				});
			}
			else
			{
				list.Add(Color.FromArgb(125, 144, 144, 144));
				list.Add(Color.FromArgb(255, 220, 220, 220));
				list.Add(Color.FromArgb(255, 245, 245, 245));
				list.Add(Color.FromArgb(255, 220, 220, 220));
				frontBrush = Graphics.CreateLinearGradientBrush(0.0, new Point(0.5, 1.0), new Point(0.5, 0.0), list, new List<double>
				{
					0.0,
					1.844,
					1.0,
					0.442
				});
			}
			list = new List<Color>();
			list.Add(Color.FromArgb(255, 155, 155, 155));
			list.Add(Color.FromArgb(255, 232, 232, 232));
			rightBrush = Graphics.CreateLinearGradientBrush(45.0, new Point(0.0, 0.5), new Point(1.0, 0.5), list, new List<double>
			{
				1.0,
				0.0
			});
			list = new List<Color>();
			list.Add(Color.FromArgb(255, 180, 180, 180));
			list.Add(Color.FromArgb(255, 240, 240, 240));
			topBrush = Graphics.CreateLinearGradientBrush(0.0, new Point(0.5, 1.0), new Point(0.5, 0.0), list, new List<double>
			{
				1.0,
				0.0
			});
		}

		internal static DoubleCollection GetDashArray(BorderStyles borderStyle)
		{
			return Graphics.LineStyleToStrokeDashArray(borderStyle.ToString());
		}

		internal static DoubleCollection GetDashArray(LineStyles lineStyle)
		{
			return Graphics.LineStyleToStrokeDashArray(lineStyle.ToString());
		}

		private static PathGeometry GetRectanglePathGeometry(double width, double height, CornerRadius xRadius, CornerRadius yRadius)
		{
			PathGeometry pathGeometry = new PathGeometry();
			pathGeometry.Figures = new PathFigureCollection();
			PathFigure pathFigure = new PathFigure();
			pathFigure.StartPoint = new Point(xRadius.TopLeft, 0.0);
			pathFigure.Segments = new PathSegmentCollection();
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(width - xRadius.TopRight, 0.0)));
			pathFigure.Segments.Add(Graphics.GetArcSegment(new Point(width, yRadius.TopRight), new Size(xRadius.TopRight, yRadius.TopRight), 0.0, SweepDirection.Clockwise));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(width, height - yRadius.BottomRight)));
			pathFigure.Segments.Add(Graphics.GetArcSegment(new Point(width - xRadius.BottomRight, height), new Size(xRadius.BottomRight, yRadius.BottomRight), 0.0, SweepDirection.Clockwise));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(xRadius.BottomLeft, height)));
			pathFigure.Segments.Add(Graphics.GetArcSegment(new Point(0.0, height - yRadius.BottomLeft), new Size(xRadius.BottomLeft, yRadius.BottomLeft), 0.0, SweepDirection.Clockwise));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(0.0, yRadius.TopLeft)));
			pathFigure.Segments.Add(Graphics.GetArcSegment(new Point(xRadius.TopLeft, 0.0), new Size(xRadius.TopLeft, yRadius.TopLeft), 0.0, SweepDirection.Clockwise));
			pathGeometry.Figures.Add(pathFigure);
			return pathGeometry;
		}

		private static CornerRadius GetCorrectedRadius(CornerRadius radius, double limit)
		{
			return new CornerRadius((radius.TopLeft > limit) ? (limit / 2.0) : radius.TopLeft, (radius.TopRight > limit) ? (limit / 2.0) : radius.TopRight, (radius.BottomRight > limit) ? (limit / 2.0) : radius.BottomRight, (radius.BottomLeft > limit) ? (limit / 2.0) : radius.BottomLeft);
		}

		private static Brush GetCornerShadowGradientBrush(ExtendedGraphics.Corners corner)
		{
			RadialGradientBrush radialGradientBrush = new RadialGradientBrush();
			radialGradientBrush.GradientStops = new GradientStopCollection();
			radialGradientBrush.GradientStops.Add(Graphics.GetGradientStop(Color.FromArgb(191, 0, 0, 0), 0.0));
			radialGradientBrush.GradientStops.Add(Graphics.GetGradientStop(Color.FromArgb(0, 0, 0, 0), 1.0));
			TransformGroup transformGroup = new TransformGroup();
			ScaleTransform value = new ScaleTransform
			{
				ScaleX = 2.0,
				ScaleY = 2.0,
				CenterX = 0.5,
				CenterY = 0.5
			};
			TranslateTransform value2 = null;
			switch (corner)
			{
			case ExtendedGraphics.Corners.TopLeft:
				value2 = new TranslateTransform
				{
					X = 0.5,
					Y = 0.5
				};
				break;
			case ExtendedGraphics.Corners.TopRight:
				value2 = new TranslateTransform
				{
					X = -0.5,
					Y = 0.5
				};
				break;
			case ExtendedGraphics.Corners.BottomLeft:
				value2 = new TranslateTransform
				{
					X = 0.5,
					Y = -0.5
				};
				break;
			case ExtendedGraphics.Corners.BottomRight:
				value2 = new TranslateTransform
				{
					X = -0.5,
					Y = -0.5
				};
				break;
			}
			transformGroup.Children.Add(value);
			transformGroup.Children.Add(value2);
			radialGradientBrush.RelativeTransform = transformGroup;
			return radialGradientBrush;
		}

		private static Brush GetSideShadowGradientBrush(ExtendedGraphics.Directions direction)
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
			linearGradientBrush.GradientStops = new GradientStopCollection();
			linearGradientBrush.GradientStops.Add(Graphics.GetGradientStop(Color.FromArgb(191, 0, 0, 0), 0.0));
			linearGradientBrush.GradientStops.Add(Graphics.GetGradientStop(Color.FromArgb(0, 0, 0, 0), 1.0));
			switch (direction)
			{
			case ExtendedGraphics.Directions.Top:
				linearGradientBrush.StartPoint = new Point(0.5, 1.0);
				linearGradientBrush.EndPoint = new Point(0.5, 0.0);
				break;
			case ExtendedGraphics.Directions.Left:
				linearGradientBrush.StartPoint = new Point(1.0, 0.5);
				linearGradientBrush.EndPoint = new Point(0.0, 0.5);
				break;
			case ExtendedGraphics.Directions.Right:
				linearGradientBrush.StartPoint = new Point(0.0, 0.5);
				linearGradientBrush.EndPoint = new Point(1.0, 0.5);
				break;
			case ExtendedGraphics.Directions.Bottom:
				linearGradientBrush.StartPoint = new Point(0.5, 0.0);
				linearGradientBrush.EndPoint = new Point(0.5, 1.0);
				break;
			}
			return linearGradientBrush;
		}

		public static DoubleCollection CloneCollection(DoubleCollection collection)
		{
			if (collection == null)
			{
				return null;
			}
			DoubleCollection doubleCollection = new DoubleCollection();
			foreach (double value in collection)
			{
				doubleCollection.Add(value);
			}
			return doubleCollection;
		}

		public static Rectangle Get3DRectangle(IElement tagReference, double width, double height, double strokeThickness, DoubleCollection strokeDashArray, Brush stroke, Brush fill)
		{
			Rectangle rectangle = new Rectangle
			{
				Width = width,
				Height = height,
				Tag = new ElementData
				{
					Element = tagReference
				}
			};
			rectangle.StrokeLineJoin = PenLineJoin.Bevel;
			ExtendedGraphics.UpdateBorderOf3DRectangle(rectangle, strokeThickness, ExtendedGraphics.CloneCollection(strokeDashArray), stroke);
			rectangle.Fill = fill;
			return rectangle;
		}

		public static void UpdateBorderOf3DRectangle(Shape rectangle, double strokeThickness, DoubleCollection strokeDashArray, Brush stroke)
		{
			if (strokeDashArray != null)
			{
				rectangle.StrokeDashArray = strokeDashArray;
			}
			rectangle.StrokeThickness = strokeThickness;
			if (strokeThickness != 0.0)
			{
				rectangle.Stroke = stroke;
			}
		}

		public static Path Get2DRectangle(IElement tagReference, double width, double height, double strokeThickness, DoubleCollection strokeDashArray, Brush stroke, Brush fill, CornerRadius xRadius, CornerRadius yRadius, bool? isRadiusApplicable, bool isPositive)
		{
			Path path = new Path
			{
				Width = width,
				Height = height,
				Tag = new ElementData
				{
					Element = tagReference
				}
			};
			path.StrokeLineJoin = PenLineJoin.Bevel;
			if ((xRadius == new CornerRadius(0.0) || xRadius == new CornerRadius(0.0, 0.0, 0.0, 0.0)) && (yRadius == new CornerRadius(0.0) || yRadius == new CornerRadius(0.0, 0.0, 0.0, 0.0)))
			{
				isRadiusApplicable = new bool?(false);
			}
			path.Data = ExtendedGraphics.GetBasicPathDataForColumn(width, height, isPositive, isRadiusApplicable, xRadius.TopLeft, yRadius.TopLeft);
			ExtendedGraphics.UpdateBorderOf2DRectangle(path, strokeThickness, ExtendedGraphics.CloneCollection(strokeDashArray), stroke, xRadius, yRadius, isRadiusApplicable);
			path.Fill = fill;
			return path;
		}

		public static Geometry GetBasicPathDataForColumn(double width, double height, bool isPositive, bool? isCornerRadiusAllowed, double xRadius, double yRadius)
		{
			if (isCornerRadiusAllowed == true)
			{
				PathGeometry pathGeometry = new PathGeometry();
				PathFigure pathFigure = new PathFigure
				{
					StartPoint = new Point(0.0, height)
				};
				pathFigure.Segments.Add(new LineSegment());
				pathFigure.Segments.Add(new ArcSegment());
				pathFigure.Segments.Add(new LineSegment());
				pathFigure.Segments.Add(new ArcSegment());
				pathFigure.Segments.Add(new LineSegment());
				pathFigure.Segments.Add(new LineSegment());
				pathGeometry.Figures.Add(pathFigure);
				return pathGeometry;
			}
			return new RectangleGeometry
			{
				Rect = new Rect(0.0, 0.0, width, height)
			};
		}

		public static void UpdateBorderOf2DRectangle(Shape shape, double strokeThickness, DoubleCollection strokeDashArray, Brush stroke, CornerRadius xRadius, CornerRadius yRadius, bool? isRadiusApplicable)
		{
			Path path = shape as Path;
			IDataPoint dataPoint = (path.Tag as ElementData).Element as IDataPoint;
			if (dataPoint != null)
			{
				double height = path.Height;
				double width = path.Width;
				if (isRadiusApplicable.Value)
				{
					Chart chart = dataPoint.Chart as Chart;
					bool flag = dataPoint.DpInfo.InternalYValue >= 0.0;
					double num = 0.0;
					double num2 = 0.0;
					if (!dataPoint.Parent.Bevel.Value)
					{
						num = ExtendedGraphics.GetCorrectedRadius(xRadius, width).TopLeft;
						num2 = ExtendedGraphics.GetCorrectedRadius(yRadius, height).TopLeft;
					}
					RectangleGeometry rectangleGeometry = path.Data as RectangleGeometry;
					PathGeometry pathGeometry = path.Data as PathGeometry;
					if (rectangleGeometry != null)
					{
						rectangleGeometry.Rect = new Rect(0.0, 0.0, width, height);
					}
					else
					{
						PathFigure pathFigure = pathGeometry.Figures[0];
						if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
						{
							if (flag)
							{
								pathFigure.StartPoint = new Point(0.0, height);
								(pathFigure.Segments[0] as LineSegment).Point = new Point(0.0, num2);
								(pathFigure.Segments[1] as ArcSegment).Point = new Point(num, 0.0);
								(pathFigure.Segments[1] as ArcSegment).SweepDirection = SweepDirection.Clockwise;
								(pathFigure.Segments[1] as ArcSegment).Size = new Size(num, num2);
								(pathFigure.Segments[2] as LineSegment).Point = new Point(width - num, 0.0);
								(pathFigure.Segments[3] as ArcSegment).Point = new Point(width, num2);
								(pathFigure.Segments[3] as ArcSegment).SweepDirection = SweepDirection.Clockwise;
								(pathFigure.Segments[3] as ArcSegment).Size = new Size(num, num2);
								(pathFigure.Segments[4] as LineSegment).Point = new Point(width, height);
								(pathFigure.Segments[5] as LineSegment).Point = new Point(0.0, height);
							}
							else
							{
								pathFigure.StartPoint = new Point(0.0, 0.0);
								(pathFigure.Segments[0] as LineSegment).Point = new Point(0.0, height - num2);
								(pathFigure.Segments[1] as ArcSegment).Point = new Point(num, height);
								(pathFigure.Segments[1] as ArcSegment).SweepDirection = SweepDirection.Counterclockwise;
								(pathFigure.Segments[1] as ArcSegment).Size = new Size(num, num2);
								(pathFigure.Segments[2] as LineSegment).Point = new Point(width - num, height);
								(pathFigure.Segments[3] as ArcSegment).Point = new Point(width, height - num2);
								(pathFigure.Segments[3] as ArcSegment).SweepDirection = SweepDirection.Counterclockwise;
								(pathFigure.Segments[3] as ArcSegment).Size = new Size(num, num2);
								(pathFigure.Segments[4] as LineSegment).Point = new Point(width, 0.0);
								(pathFigure.Segments[5] as LineSegment).Point = new Point(0.0, 0.0);
							}
						}
						else if (flag)
						{
							pathFigure.StartPoint = new Point(0.0, 0.0);
							(pathFigure.Segments[0] as LineSegment).Point = new Point(width - num2, 0.0);
							(pathFigure.Segments[1] as ArcSegment).Point = new Point(width, num);
							(pathFigure.Segments[1] as ArcSegment).SweepDirection = SweepDirection.Clockwise;
							(pathFigure.Segments[1] as ArcSegment).Size = new Size(num2, num);
							(pathFigure.Segments[2] as LineSegment).Point = new Point(width, height - num2);
							(pathFigure.Segments[3] as ArcSegment).Point = new Point(width - num2, height);
							(pathFigure.Segments[3] as ArcSegment).SweepDirection = SweepDirection.Clockwise;
							(pathFigure.Segments[3] as ArcSegment).Size = new Size(num, num2);
							(pathFigure.Segments[4] as LineSegment).Point = new Point(0.0, height);
							(pathFigure.Segments[5] as LineSegment).Point = new Point(0.0, 0.0);
						}
						else
						{
							pathFigure.StartPoint = new Point(width, height);
							(pathFigure.Segments[0] as LineSegment).Point = new Point(num2, height);
							(pathFigure.Segments[1] as ArcSegment).Point = new Point(0.0, height - num);
							(pathFigure.Segments[1] as ArcSegment).SweepDirection = SweepDirection.Clockwise;
							(pathFigure.Segments[1] as ArcSegment).Size = new Size(num2, num);
							(pathFigure.Segments[2] as LineSegment).Point = new Point(0.0, num);
							(pathFigure.Segments[3] as ArcSegment).Point = new Point(num2, 0.0);
							(pathFigure.Segments[3] as ArcSegment).SweepDirection = SweepDirection.Clockwise;
							(pathFigure.Segments[3] as ArcSegment).Size = new Size(num2, num);
							(pathFigure.Segments[4] as LineSegment).Point = new Point(width, 0.0);
							(pathFigure.Segments[5] as LineSegment).Point = new Point(width, height);
						}
					}
				}
			}
			path.StrokeDashArray = strokeDashArray;
			path.StrokeThickness = strokeThickness;
			if (strokeThickness != 0.0)
			{
				path.Stroke = stroke;
			}
		}

		public static Canvas Get2DRectangleBevel(object tagReference, double width, double height, double bevelX, double bevelY, Brush topBrush, Brush leftBrush, Brush rightBrush, Brush bottomBrush)
		{
			Canvas canvas = new Canvas
			{
				IsHitTestVisible = false,
				Tag = new ElementData
				{
					Element = tagReference,
					VisualElementName = "Bevel"
				}
			};
			if (width < 0.0 || height < 0.0)
			{
				return canvas;
			}
			canvas.Width = width;
			canvas.Height = height;
			Polygon polygon = new Polygon
			{
				Tag = new ElementData
				{
					Element = tagReference,
					VisualElementName = "TopBevel"
				}
			};
			polygon.Points = new PointCollection();
			polygon.Points.Add(new Point(0.0, 0.0));
			polygon.Points.Add(new Point(width, 0.0));
			polygon.Points.Add(new Point(width - bevelX, bevelY));
			polygon.Points.Add(new Point(bevelX, bevelY));
			polygon.Fill = topBrush;
			canvas.Children.Add(polygon);
			Polygon polygon2 = new Polygon
			{
				Tag = new ElementData
				{
					Element = tagReference,
					VisualElementName = "LeftBevel"
				}
			};
			polygon2.Points = new PointCollection();
			polygon2.Points.Add(new Point(0.0, 0.0));
			polygon2.Points.Add(new Point(bevelX, bevelY));
			polygon2.Points.Add(new Point(bevelX, height - bevelY));
			polygon2.Points.Add(new Point(0.0, height));
			polygon2.Fill = leftBrush;
			canvas.Children.Add(polygon2);
			Polygon polygon3 = new Polygon
			{
				Tag = new ElementData
				{
					Element = tagReference,
					VisualElementName = "RightBevel"
				}
			};
			polygon3.Points = new PointCollection();
			polygon3.Points.Add(new Point(width, 0.0));
			polygon3.Points.Add(new Point(width, height));
			polygon3.Points.Add(new Point(width - bevelX, height - bevelY));
			polygon3.Points.Add(new Point(width - bevelX, bevelY));
			polygon3.Fill = rightBrush;
			canvas.Children.Add(polygon3);
			Polygon polygon4 = new Polygon
			{
				Tag = new ElementData
				{
					Element = tagReference,
					VisualElementName = "BottomBevel"
				}
			};
			polygon4.Points = new PointCollection();
			polygon4.Points.Add(new Point(0.0, height));
			polygon4.Points.Add(new Point(bevelX, height - bevelY));
			polygon4.Points.Add(new Point(width - bevelX, height - bevelY));
			polygon4.Points.Add(new Point(width, height));
			polygon4.Fill = bottomBrush;
			canvas.Children.Add(polygon4);
			return canvas;
		}

		public static Canvas Get2DRectangleGradiance(double width, double height, Brush brush1, Brush brush2, Orientation orientation)
		{
			Canvas canvas = new Canvas
			{
				IsHitTestVisible = false,
				Tag = new ElementData
				{
					VisualElementName = "LightingCanvas"
				}
			};
			canvas.Width = width;
			canvas.Height = height;
			if (orientation == Orientation.Vertical)
			{
				Rectangle rectangle = new Rectangle();
				rectangle.Width = width / 2.0;
				rectangle.Height = height;
				rectangle.SetValue(Canvas.TopProperty, 0.0);
				rectangle.SetValue(Canvas.LeftProperty, 0.0);
				rectangle.Fill = brush1;
				canvas.Children.Add(rectangle);
				Rectangle rectangle2 = new Rectangle();
				rectangle2.Width = width / 2.0;
				rectangle2.Height = height;
				rectangle2.SetValue(Canvas.TopProperty, 0.0);
				rectangle2.SetValue(Canvas.LeftProperty, width / 2.0);
				rectangle2.Fill = brush2;
				canvas.Children.Add(rectangle2);
			}
			else
			{
				Rectangle rectangle3 = new Rectangle();
				rectangle3.Width = width;
				rectangle3.Height = height / 2.0;
				rectangle3.SetValue(Canvas.TopProperty, 0.0);
				rectangle3.SetValue(Canvas.LeftProperty, 0.0);
				rectangle3.Fill = brush1;
				canvas.Children.Add(rectangle3);
				Rectangle rectangle4 = new Rectangle();
				rectangle4.Width = width;
				rectangle4.Height = height / 2.0;
				rectangle4.SetValue(Canvas.TopProperty, height / 2.0);
				rectangle4.SetValue(Canvas.LeftProperty, 0.0);
				rectangle4.Fill = brush2;
				canvas.Children.Add(rectangle4);
			}
			return canvas;
		}

		public static Path GetPathFromPoints(Brush fillColor, params Point[] points)
		{
			Path path = new Path();
			PathGeometry pathGeometry = new PathGeometry();
			PathFigure pathFigure = new PathFigure();
			pathFigure.StartPoint = points[0];
			LineSegment value;
			for (int i = 0; i < points.Length; i++)
			{
				Point point = points[i];
				value = new LineSegment
				{
					Point = point
				};
				pathFigure.Segments.Add(value);
			}
			value = new LineSegment
			{
				Point = points[0]
			};
			pathFigure.Segments.Add(value);
			pathGeometry.Figures.Add(pathFigure);
			path.Data = pathGeometry;
			path.Fill = fillColor;
			return path;
		}

		public static DropShadowEffect GetShadowEffect(double direction, double shadowDepth, double opacity)
		{
			return new DropShadowEffect
			{
				Direction = direction,
				ShadowDepth = shadowDepth,
				Opacity = opacity,
				Color = Color.FromArgb(255, 200, 200, 200),
				RenderingBias = RenderingBias.Quality,
				BlurRadius = 5.0
			};
		}

		internal static PathGeometry GetShadowClip(Size clipSize, CornerRadius radius)
		{
			PathGeometry pathGeometry = new PathGeometry();
			pathGeometry.FillRule = FillRule.EvenOdd;
			pathGeometry.Figures = new PathFigureCollection();
			PathFigure pathFigure = new PathFigure();
			pathFigure.StartPoint = new Point(0.0, clipSize.Height - 5.0);
			pathFigure.Segments = new PathSegmentCollection();
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(0.0, clipSize.Height - 5.0)));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(0.0, clipSize.Height)));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(clipSize.Width, clipSize.Height)));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(clipSize.Width, 0.0)));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(clipSize.Width - 5.0, 0.0)));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(clipSize.Width - 5.0, clipSize.Height - 5.0 - radius.BottomRight)));
			pathFigure.Segments.Add(Graphics.GetArcSegment(new Point(clipSize.Width - 5.0 - radius.BottomRight, clipSize.Height - 5.0), new Size(radius.BottomRight, radius.BottomRight), 90.0, SweepDirection.Clockwise));
			pathGeometry.Figures.Add(pathFigure);
			return pathGeometry;
		}

		internal static PathGeometry GetShadowClip(Size clipSize)
		{
			PathGeometry pathGeometry = new PathGeometry();
			pathGeometry.FillRule = FillRule.EvenOdd;
			pathGeometry.Figures = new PathFigureCollection();
			PathFigure pathFigure = new PathFigure();
			pathFigure.StartPoint = new Point(0.0, clipSize.Height - 5.0);
			pathFigure.Segments = new PathSegmentCollection();
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(clipSize.Width - 5.0, clipSize.Height - 5.0)));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(clipSize.Width - 5.0, 0.0)));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(clipSize.Width, 0.0)));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(clipSize.Width, clipSize.Height)));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(0.0, clipSize.Height)));
			pathFigure.Segments.Add(Graphics.GetLineSegment(new Point(0.0, clipSize.Height - 5.0)));
			pathGeometry.Figures.Add(pathFigure);
			return pathGeometry;
		}

		public static Grid Get2DRectangleShadow(FrameworkElement tagReference, double width, double height, CornerRadius xRadius, CornerRadius yRadius, double minCurvature)
		{
			CornerRadius radius = new CornerRadius(Math.Max(xRadius.TopLeft, minCurvature), Math.Max(xRadius.TopRight, minCurvature), Math.Max(xRadius.BottomRight, minCurvature), Math.Max(xRadius.BottomLeft, minCurvature));
			CornerRadius radius2 = new CornerRadius(Math.Max(yRadius.TopLeft, minCurvature), Math.Max(yRadius.TopRight, minCurvature), Math.Max(yRadius.BottomRight, minCurvature), Math.Max(yRadius.BottomLeft, minCurvature));
			CornerRadius correctedRadius = ExtendedGraphics.GetCorrectedRadius(radius, width / 2.0);
			CornerRadius correctedRadius2 = ExtendedGraphics.GetCorrectedRadius(radius2, height / 2.0);
			Grid grid = new Grid
			{
				IsHitTestVisible = false
			};
			grid.Height = height;
			grid.Width = width;
			for (int i = 0; i < 3; i++)
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition
				{
					Width = new GridLength(0.0, GridUnitType.Auto)
				});
				grid.RowDefinitions.Add(new RowDefinition
				{
					Height = new GridLength(0.0, GridUnitType.Auto)
				});
			}
			Rectangle rectangle = new Rectangle
			{
				Width = correctedRadius.TopLeft,
				Height = correctedRadius2.TopLeft,
				Fill = ExtendedGraphics.GetCornerShadowGradientBrush(ExtendedGraphics.Corners.TopLeft),
				Tag = ((tagReference != null) ? new ElementData
				{
					Element = tagReference
				} : null)
			};
			Rectangle rectangle2 = new Rectangle
			{
				Width = correctedRadius.TopRight,
				Height = correctedRadius2.TopRight,
				Fill = ExtendedGraphics.GetCornerShadowGradientBrush(ExtendedGraphics.Corners.TopRight),
				Tag = ((tagReference != null) ? new ElementData
				{
					Element = tagReference
				} : null)
			};
			Rectangle rectangle3 = new Rectangle
			{
				Width = correctedRadius.BottomLeft,
				Height = correctedRadius2.BottomLeft,
				Fill = ExtendedGraphics.GetCornerShadowGradientBrush(ExtendedGraphics.Corners.BottomLeft),
				Tag = ((tagReference != null) ? new ElementData
				{
					Element = tagReference
				} : null)
			};
			Rectangle rectangle4 = new Rectangle
			{
				Width = correctedRadius.BottomRight,
				Height = correctedRadius2.BottomRight,
				Fill = ExtendedGraphics.GetCornerShadowGradientBrush(ExtendedGraphics.Corners.BottomRight),
				Tag = ((tagReference != null) ? new ElementData
				{
					Element = tagReference
				} : null)
			};
			Rectangle rectangle5 = new Rectangle
			{
				Width = width - correctedRadius.TopLeft - correctedRadius.TopRight,
				Height = height - correctedRadius2.TopLeft - correctedRadius2.BottomLeft,
				Fill = new SolidColorBrush(Color.FromArgb(191, 0, 0, 0)),
				Tag = ((tagReference != null) ? new ElementData
				{
					Element = tagReference
				} : null)
			};
			Rectangle rectangle6 = new Rectangle
			{
				Width = width - correctedRadius.TopLeft - correctedRadius.TopRight,
				Height = Math.Max(correctedRadius2.TopLeft, correctedRadius2.TopRight),
				Fill = ExtendedGraphics.GetSideShadowGradientBrush(ExtendedGraphics.Directions.Top),
				Tag = ((tagReference != null) ? new ElementData
				{
					Element = tagReference
				} : null)
			};
			Rectangle rectangle7 = new Rectangle
			{
				Width = Math.Max(correctedRadius.TopRight, correctedRadius.BottomRight),
				Height = height - correctedRadius2.TopRight - correctedRadius2.BottomRight,
				Fill = ExtendedGraphics.GetSideShadowGradientBrush(ExtendedGraphics.Directions.Right),
				Tag = ((tagReference != null) ? new ElementData
				{
					Element = tagReference
				} : null)
			};
			Rectangle rectangle8 = new Rectangle
			{
				Width = Math.Max(correctedRadius.TopLeft, correctedRadius.BottomLeft),
				Height = height - correctedRadius2.TopLeft - correctedRadius2.BottomLeft,
				Fill = ExtendedGraphics.GetSideShadowGradientBrush(ExtendedGraphics.Directions.Left),
				Tag = ((tagReference != null) ? new ElementData
				{
					Element = tagReference
				} : null)
			};
			Rectangle rectangle9 = new Rectangle
			{
				Width = width - correctedRadius.BottomLeft - correctedRadius.BottomRight,
				Height = Math.Max(correctedRadius2.BottomLeft, correctedRadius2.BottomRight),
				Fill = ExtendedGraphics.GetSideShadowGradientBrush(ExtendedGraphics.Directions.Bottom),
				Tag = ((tagReference != null) ? new ElementData
				{
					Element = tagReference
				} : null)
			};
			rectangle.SetValue(Grid.RowProperty, 0);
			rectangle.SetValue(Grid.ColumnProperty, 0);
			rectangle6.SetValue(Grid.RowProperty, 0);
			rectangle6.SetValue(Grid.ColumnProperty, 1);
			rectangle2.SetValue(Grid.RowProperty, 0);
			rectangle2.SetValue(Grid.ColumnProperty, 2);
			rectangle8.SetValue(Grid.RowProperty, 1);
			rectangle.SetValue(Grid.ColumnProperty, 0);
			rectangle5.SetValue(Grid.RowProperty, 1);
			rectangle5.SetValue(Grid.ColumnProperty, 1);
			rectangle7.SetValue(Grid.RowProperty, 1);
			rectangle7.SetValue(Grid.ColumnProperty, 2);
			rectangle3.SetValue(Grid.RowProperty, 2);
			rectangle3.SetValue(Grid.ColumnProperty, 0);
			rectangle9.SetValue(Grid.RowProperty, 2);
			rectangle9.SetValue(Grid.ColumnProperty, 1);
			rectangle4.SetValue(Grid.RowProperty, 2);
			rectangle4.SetValue(Grid.ColumnProperty, 2);
			grid.Children.Add(rectangle);
			grid.Children.Add(rectangle6);
			grid.Children.Add(rectangle2);
			grid.Children.Add(rectangle8);
			grid.Children.Add(rectangle5);
			grid.Children.Add(rectangle7);
			grid.Children.Add(rectangle3);
			grid.Children.Add(rectangle9);
			grid.Children.Add(rectangle4);
			return grid;
		}

		public static Grid GetRectangle4PlotAreaEdge(FrameworkElement tagReference, double width, double height, CornerRadius xRadius, CornerRadius yRadius, double minCurvature, Brush brush)
		{
			CornerRadius radius = new CornerRadius(Math.Max(xRadius.TopLeft, minCurvature), Math.Max(xRadius.TopRight, minCurvature), Math.Max(xRadius.BottomRight, minCurvature), Math.Max(xRadius.BottomLeft, minCurvature));
			CornerRadius radius2 = new CornerRadius(Math.Max(yRadius.TopLeft, minCurvature), Math.Max(yRadius.TopRight, minCurvature), Math.Max(yRadius.BottomRight, minCurvature), Math.Max(yRadius.BottomLeft, minCurvature));
			ExtendedGraphics.GetCorrectedRadius(radius, width / 2.0);
			ExtendedGraphics.GetCorrectedRadius(radius2, height / 2.0);
			Grid grid = new Grid
			{
				IsHitTestVisible = false
			};
			grid.Height = height;
			grid.Width = width;
			Rectangle element = new Rectangle
			{
				Width = width,
				Height = height,
				RadiusX = 5.0,
				RadiusY = 5.0,
				Fill = new SolidColorBrush(Color.FromArgb(255, 167, 171, 171)),
				Opacity = 0.9,
				Tag = ((tagReference != null) ? new ElementData
				{
					Element = tagReference
				} : null)
			};
			grid.Children.Add(element);
			return grid;
		}
	}
}
