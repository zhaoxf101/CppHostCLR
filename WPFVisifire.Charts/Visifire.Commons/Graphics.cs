using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Visifire.Charts;

namespace Visifire.Commons
{
	public class Graphics
	{
		internal static Random RAND = new Random(DateTime.Now.Millisecond);

		private static Dictionary<Color, Brush> _3dLightingTopBrushs = new Dictionary<Color, Brush>();

		private static Dictionary<Color, Brush> _3dLightingRightBrushs = new Dictionary<Color, Brush>();

		private static Dictionary<Color, Brush> _3dLightingFrontBrushs = new Dictionary<Color, Brush>();

		public static double[] DefaultFontSizes = new double[]
		{
			8.0,
			10.0,
			12.0,
			14.0,
			16.0,
			18.0,
			20.0,
			24.0,
			28.0,
			32.0,
			36.0,
			40.0
		};

		public static Brush AUTO_WHITE_FONT_BRUSH
		{
			get
			{
				return new SolidColorBrush(Color.FromArgb(255, 239, 239, 239));
			}
		}

		public static Brush AUTO_BLACK_FONT_BRUSH
		{
			get
			{
				return new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
			}
		}

		public static Brush GRAY_BRUSH
		{
			get
			{
				return new SolidColorBrush(Colors.Gray);
			}
		}

		public static Brush BLACK_BRUSH
		{
			get
			{
				return new SolidColorBrush(Colors.Black);
			}
		}

		public static Brush RED_BRUSH
		{
			get
			{
				return new SolidColorBrush(Colors.Red);
			}
		}

		public static Brush ORANGE_BRUSH
		{
			get
			{
				return new SolidColorBrush(Colors.Orange);
			}
		}

		public static Brush WHITE_BRUSH
		{
			get
			{
				return new SolidColorBrush(Colors.White);
			}
		}

		public static Brush TRANSPARENT_BRUSH
		{
			get
			{
				return new SolidColorBrush(Colors.Transparent);
			}
		}

		public static void SetImageSource(Image image, string imageResourcePath)
		{
			using (Stream manifestResourceStream = typeof(Chart).Assembly.GetManifestResourceStream(imageResourcePath))
			{
				if (manifestResourceStream != null)
				{
					BitmapImage bitmapImage = new BitmapImage();
					bitmapImage.BeginInit();
					bitmapImage.StreamSource = manifestResourceStream;
					bitmapImage.EndInit();
					image.Source = bitmapImage;
				}
			}
		}

		public static Brush GetRandomColor()
		{
			return new SolidColorBrush(Color.FromArgb(255, (byte)Graphics.RAND.Next(255), (byte)Graphics.RAND.Next(255), (byte)Graphics.RAND.Next(255)));
		}

		public static void DrawPointAt(Point point, Canvas visual, Color fillColor)
		{
			Ellipse ellipse = new Ellipse
			{
				Height = 4.0,
				Width = 4.0,
				Fill = new SolidColorBrush(fillColor),
				Stroke = new SolidColorBrush(Colors.Red),
				StrokeThickness = 0.25
			};
			ellipse.SetValue(Canvas.LeftProperty, point.X - ellipse.Height / 2.0);
			ellipse.SetValue(Canvas.TopProperty, point.Y - ellipse.Width / 2.0);
			ellipse.SetValue(Panel.ZIndexProperty, 10001);
			visual.Children.Add(ellipse);
		}

		internal static bool IntersectionOfTwoStraightLines(Point line1Point1, Point line1Point2, Point line2Point1, Point line2Point2, ref Point intersection)
		{
			double num = (line1Point1.Y - line2Point1.Y) * (line2Point2.X - line2Point1.X) - (line1Point1.X - line2Point1.X) * (line2Point2.Y - line2Point1.Y);
			double num2 = (line1Point2.X - line1Point1.X) * (line2Point2.Y - line2Point1.Y) - (line1Point2.Y - line1Point1.Y) * (line2Point2.X - line2Point1.X);
			if (num2 == 0.0)
			{
				return false;
			}
			double num3 = num / num2;
			num = (line1Point1.Y - line2Point1.Y) * (line1Point2.X - line1Point1.X) - (line1Point1.X - line2Point1.X) * (line1Point2.Y - line1Point1.Y);
			double num4 = num / num2;
			if (num3 < 0.0 || num3 > 1.0 || num4 < 0.0 || num4 > 1.0)
			{
				return false;
			}
			intersection.X = line1Point1.X + (double)((int)(0.5 + num3 * (line1Point2.X - line1Point1.X)));
			intersection.Y = line1Point1.Y + (double)((int)(0.5 + num3 * (line1Point2.Y - line1Point1.Y)));
			return true;
		}

		internal static Point MidPointOfALine(Point point1, Point point2)
		{
			return new Point((point1.X + point2.X) / 2.0, (point1.Y + point2.Y) / 2.0);
		}

		internal static double DistanceBetweenTwoPoints(Point point1, Point point2)
		{
			return Math.Sqrt(Math.Pow(point1.X - point2.X, 2.0) + Math.Pow(point1.Y - point2.Y, 2.0));
		}

		internal static Point IntersectingPointOfTwoLines(Point p1, Point p2, Point p3, Point p4)
		{
			double num = (p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X);
			num /= (p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y);
			double num2 = (p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X);
			num2 /= (p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y);
			double x = p1.X + num * (p2.X - p1.X);
			double y = p1.X + num2 * (p2.Y - p1.Y);
			return new Point(x, y);
		}

		public static Size CalculateVisualSize(FrameworkElement visual)
		{
			Size desiredSize = new Size(0.0, 0.0);
			if (visual != null)
			{
				visual.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				desiredSize = visual.DesiredSize;
			}
			return desiredSize;
		}

		internal static Brush Get3DBrushLighting(Brush solidColorBrush, bool lightingEnabled)
		{
			if (solidColorBrush.GetType().Equals(typeof(SolidColorBrush)))
			{
				Color color = (solidColorBrush as SolidColorBrush).Color;
				RadialGradientBrush radialGradientBrush = new RadialGradientBrush
				{
					Center = new Point(0.3, 0.3),
					RadiusX = 0.93,
					RadiusY = 1.0,
					GradientOrigin = new Point(0.2, 0.2)
				};
				if (color == Colors.Black)
				{
					if (lightingEnabled)
					{
						radialGradientBrush.GradientStops.Add(new GradientStop
						{
							Color = Colors.White,
							Offset = 0.0
						});
					}
					radialGradientBrush.GradientStops.Add(new GradientStop
					{
						Color = Colors.Gray,
						Offset = 0.1
					});
					radialGradientBrush.GradientStops.Add(new GradientStop
					{
						Color = Colors.Black,
						Offset = 1.0
					});
				}
				else
				{
					Color darkerColor = Graphics.GetDarkerColor(color, 0.2);
					if (lightingEnabled)
					{
						radialGradientBrush.GradientStops.Add(new GradientStop
						{
							Color = Colors.White,
							Offset = 0.0
						});
					}
					radialGradientBrush.GradientStops.Add(new GradientStop
					{
						Color = color,
						Offset = 0.1
					});
					radialGradientBrush.GradientStops.Add(new GradientStop
					{
						Color = darkerColor,
						Offset = 1.0
					});
				}
				return radialGradientBrush;
			}
			return solidColorBrush;
		}

		internal static Size CalculateAngularSize(double radianAngle, double actualWidth, double actualHeight)
		{
			if (radianAngle != 0.0)
			{
				double num = Math.Sqrt(Math.Pow(actualHeight, 2.0) + Math.Pow(actualWidth, 2.0));
				double num2 = Math.Atan(actualHeight / actualWidth);
				double value = num * Math.Sin(radianAngle + num2);
				double value2 = num * Math.Sin(radianAngle - num2);
				double value3 = num * Math.Cos(radianAngle + num2);
				double value4 = num * Math.Cos(radianAngle - num2);
				actualHeight = Math.Max(Math.Abs(value), Math.Abs(value2));
				actualWidth = Math.Max(Math.Abs(value3), Math.Abs(value4));
			}
			return new Size(actualWidth, actualHeight);
		}

		internal static Size CalculateTextBlockSize(double radianAngle, TextBlock textBlock)
		{
			if (!textBlock.IsMeasureValid)
			{
				textBlock.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			}
			double height = textBlock.DesiredSize.Height;
			double width = textBlock.DesiredSize.Width;
			return Graphics.CalculateAngularSize(radianAngle, width, height);
		}

		internal static DoubleCollection GenerateDoubleCollection(params double[] values)
		{
			DoubleCollection doubleCollection = new DoubleCollection();
			for (int i = 0; i < values.Length; i++)
			{
				double value = values[i];
				doubleCollection.Add(value);
			}
			return doubleCollection;
		}

		internal static PointCollection GeneratePointCollection(params Point[] points)
		{
			PointCollection pointCollection = new PointCollection();
			for (int i = 0; i < points.Length; i++)
			{
				Point value = points[i];
				pointCollection.Add(value);
			}
			return pointCollection;
		}

		internal static Brush GetRightGradianceBrush(int alpha)
		{
			return new LinearGradientBrush
			{
				GradientStops = new GradientStopCollection(),
				StartPoint = new Point(1.0, 1.0),
				EndPoint = new Point(0.0, 0.0),
				GradientStops = 
				{
					Graphics.GetGradientStop(Color.FromArgb((byte)alpha, 0, 0, 0), 0.0),
					Graphics.GetGradientStop(Color.FromArgb(0, 0, 0, 0), 0.0)
				}
			};
		}

		internal static Brush GetTopFaceBrush(Brush brush)
		{
			if (brush == null)
			{
				return null;
			}
			if (typeof(SolidColorBrush).Equals(brush.GetType()))
			{
				SolidColorBrush solidColorBrush = brush as SolidColorBrush;
				if (!Graphics._3dLightingTopBrushs.ContainsKey(solidColorBrush.Color))
				{
					brush = Graphics.GetTopFaceGradientBrush(solidColorBrush);
					Graphics._3dLightingTopBrushs.Add(solidColorBrush.Color, brush);
					return brush;
				}
				if (Thread.CurrentThread.IsBackground)
				{
					brush = Graphics.GetTopFaceGradientBrush(solidColorBrush);
					if (Graphics._3dLightingTopBrushs.ContainsKey(solidColorBrush.Color))
					{
						Graphics._3dLightingTopBrushs[solidColorBrush.Color] = brush;
					}
					else
					{
						Graphics._3dLightingTopBrushs.Add(solidColorBrush.Color, brush);
					}
					return brush;
				}
				return Graphics._3dLightingTopBrushs[solidColorBrush.Color];
			}
			else
			{
				if (!(brush is GradientBrush))
				{
					return brush;
				}
				GradientBrush gradientBrush = brush as GradientBrush;
				List<Color> list = new List<Color>();
				List<double> list2 = new List<double>();
				foreach (GradientStop current in gradientBrush.GradientStops)
				{
					list.Add(Graphics.GetLighterColor(current.Color, 0.85));
					list2.Add(current.Offset);
				}
				if (brush is LinearGradientBrush)
				{
					return Graphics.CreateLinearGradientBrush(-45.0, new Point(-0.5, 1.5), new Point(0.5, 0.0), list, list2);
				}
				return Graphics.CreateRadialGradientBrush(list, list2);
			}
		}

		private static Brush GetTopFaceGradientBrush(SolidColorBrush solidBrush)
		{
			List<Color> list = new List<Color>();
			List<double> list2 = new List<double>();
			list.Add(Graphics.GetDarkerColor(solidBrush.Color, 0.85));
			list2.Add(0.0);
			list.Add(Graphics.GetLighterColor(solidBrush.Color, 0.35));
			list2.Add(1.0);
			return Graphics.CreateLinearGradientBrush(-45.0, new Point(0.0, 0.5), new Point(1.0, 0.5), list, list2);
		}

		internal static Brush GetRightFaceBrush(Brush brush)
		{
			if (brush == null)
			{
				return null;
			}
			if (typeof(SolidColorBrush).Equals(brush.GetType()))
			{
				SolidColorBrush solidColorBrush = brush as SolidColorBrush;
				if (!Graphics._3dLightingRightBrushs.ContainsKey(solidColorBrush.Color))
				{
					brush = Graphics.GetRightFaceGradientBrush(solidColorBrush);
					Graphics._3dLightingRightBrushs.Add(solidColorBrush.Color, brush);
					return brush;
				}
				if (Thread.CurrentThread.IsBackground)
				{
					brush = Graphics.GetRightFaceGradientBrush(solidColorBrush);
					if (Graphics._3dLightingRightBrushs.ContainsKey(solidColorBrush.Color))
					{
						Graphics._3dLightingRightBrushs[solidColorBrush.Color] = brush;
					}
					else
					{
						Graphics._3dLightingRightBrushs.Add(solidColorBrush.Color, brush);
					}
					return brush;
				}
				return Graphics._3dLightingRightBrushs[solidColorBrush.Color];
			}
			else
			{
				if (!(brush is GradientBrush))
				{
					return brush;
				}
				GradientBrush gradientBrush = brush as GradientBrush;
				List<Color> list = new List<Color>();
				List<double> list2 = new List<double>();
				foreach (GradientStop current in gradientBrush.GradientStops)
				{
					list.Add(Graphics.GetDarkerColor(current.Color, 0.75));
					list2.Add(current.Offset);
				}
				if (brush is LinearGradientBrush)
				{
					return Graphics.CreateLinearGradientBrush(0.0, new Point(0.0, 1.0), new Point(1.0, 0.0), list, list2);
				}
				return Graphics.CreateRadialGradientBrush(list, list2);
			}
		}

		internal static Brush GetRightFaceBrush4Column3D(Brush brush)
		{
			if (brush == null)
			{
				return null;
			}
			if (typeof(SolidColorBrush).Equals(brush.GetType()))
			{
				SolidColorBrush solidColorBrush = brush as SolidColorBrush;
				if (!Graphics._3dLightingRightBrushs.ContainsKey(solidColorBrush.Color))
				{
					brush = Graphics.GetRightFaceGradientBrush4Column3D(solidColorBrush);
					Graphics._3dLightingRightBrushs.Add(solidColorBrush.Color, brush);
					return brush;
				}
				if (Thread.CurrentThread.IsBackground)
				{
					brush = Graphics.GetRightFaceGradientBrush(solidColorBrush);
					if (Graphics._3dLightingRightBrushs.ContainsKey(solidColorBrush.Color))
					{
						Graphics._3dLightingRightBrushs[solidColorBrush.Color] = brush;
					}
					else
					{
						Graphics._3dLightingRightBrushs.Add(solidColorBrush.Color, brush);
					}
					return brush;
				}
				return Graphics._3dLightingRightBrushs[solidColorBrush.Color];
			}
			else
			{
				if (!(brush is GradientBrush))
				{
					return brush;
				}
				GradientBrush gradientBrush = brush as GradientBrush;
				List<Color> list = new List<Color>();
				List<double> list2 = new List<double>();
				foreach (GradientStop current in gradientBrush.GradientStops)
				{
					list.Add(Graphics.GetDarkerColor(current.Color, 0.75));
					list2.Add(current.Offset);
				}
				if (brush is LinearGradientBrush)
				{
					return Graphics.CreateLinearGradientBrush(0.0, new Point(0.0, 1.0), new Point(1.0, 0.0), list, list2);
				}
				return Graphics.CreateRadialGradientBrush(list, list2);
			}
		}

		private static Brush GetRightFaceGradientBrush4Column3D(SolidColorBrush solidBrush)
		{
			List<Color> list = new List<Color>();
			List<double> list2 = new List<double>();
			list.Add(Graphics.GetDarkerColor(solidBrush.Color, 0.35));
			list2.Add(1.0);
			list.Add(Graphics.GetDarkerColor(solidBrush.Color, 0.75));
			list2.Add(0.0);
			return Graphics.CreateLinearGradientBrush4Column3D(-120.0, new Point(0.5, 0.0), new Point(0.5, 1.0), list, list2);
		}

		private static Brush GetRightFaceGradientBrush(SolidColorBrush solidBrush)
		{
			List<Color> list = new List<Color>();
			List<double> list2 = new List<double>();
			list.Add(Graphics.GetDarkerColor(solidBrush.Color, 0.35));
			list2.Add(0.0);
			list.Add(Graphics.GetDarkerColor(solidBrush.Color, 0.75));
			list2.Add(1.0);
			return Graphics.CreateLinearGradientBrush(-120.0, new Point(0.0, 0.5), new Point(1.0, 0.5), list, list2);
		}

		internal static Brush GetFrontFaceBrush(Brush brush)
		{
			if (brush == null)
			{
				return null;
			}
			if (!typeof(SolidColorBrush).Equals(brush.GetType()))
			{
				return brush;
			}
			SolidColorBrush solidColorBrush = brush as SolidColorBrush;
			if (!Graphics._3dLightingFrontBrushs.ContainsKey(solidColorBrush.Color))
			{
				brush = Graphics.GetFrontFaceGradientBrush(solidColorBrush);
				Graphics._3dLightingFrontBrushs.Add(solidColorBrush.Color, brush);
				return brush;
			}
			if (Thread.CurrentThread.IsBackground)
			{
				brush = Graphics.GetFrontFaceGradientBrush(solidColorBrush);
				if (Graphics._3dLightingFrontBrushs.ContainsKey(solidColorBrush.Color))
				{
					Graphics._3dLightingFrontBrushs[solidColorBrush.Color] = brush;
				}
				else
				{
					Graphics._3dLightingFrontBrushs.Add(solidColorBrush.Color, brush);
				}
				return brush;
			}
			return Graphics._3dLightingFrontBrushs[solidColorBrush.Color];
		}

		internal static Brush GetFrontFaceBrush4Area3D(Brush brush)
		{
			if (brush == null)
			{
				return null;
			}
			if (!typeof(SolidColorBrush).Equals(brush.GetType()))
			{
				return brush;
			}
			SolidColorBrush solidColorBrush = brush as SolidColorBrush;
			if (!Graphics._3dLightingFrontBrushs.ContainsKey(solidColorBrush.Color))
			{
				brush = Graphics.GetFrontFaceGradientBrush4Area3D(solidColorBrush);
				Graphics._3dLightingFrontBrushs.Add(solidColorBrush.Color, brush);
				return brush;
			}
			if (Thread.CurrentThread.IsBackground)
			{
				brush = Graphics.GetFrontFaceGradientBrush(solidColorBrush);
				if (Graphics._3dLightingFrontBrushs.ContainsKey(solidColorBrush.Color))
				{
					Graphics._3dLightingFrontBrushs[solidColorBrush.Color] = brush;
				}
				else
				{
					Graphics._3dLightingFrontBrushs.Add(solidColorBrush.Color, brush);
				}
				return brush;
			}
			return Graphics._3dLightingFrontBrushs[solidColorBrush.Color];
		}

		internal static Brush GetFrontFaceBrush4Column3D(Brush brush)
		{
			if (brush == null)
			{
				return null;
			}
			if (!typeof(SolidColorBrush).Equals(brush.GetType()))
			{
				return brush;
			}
			SolidColorBrush solidColorBrush = brush as SolidColorBrush;
			if (!Graphics._3dLightingFrontBrushs.ContainsKey(solidColorBrush.Color))
			{
				brush = Graphics.GetFrontFaceGradientBrush4Column3D(solidColorBrush);
				Graphics._3dLightingFrontBrushs.Add(solidColorBrush.Color, brush);
				return brush;
			}
			if (Thread.CurrentThread.IsBackground)
			{
				brush = Graphics.GetFrontFaceGradientBrush(solidColorBrush);
				if (Graphics._3dLightingFrontBrushs.ContainsKey(solidColorBrush.Color))
				{
					Graphics._3dLightingFrontBrushs[solidColorBrush.Color] = brush;
				}
				else
				{
					Graphics._3dLightingFrontBrushs.Add(solidColorBrush.Color, brush);
				}
				return brush;
			}
			return Graphics._3dLightingFrontBrushs[solidColorBrush.Color];
		}

		private static Brush GetFrontFaceGradientBrush4Column3D(SolidColorBrush solidBrush)
		{
			List<Color> list = new List<Color>();
			List<double> list2 = new List<double>();
			list.Add(Graphics.GetDarkerColor(solidBrush.Color, 0.65));
			list2.Add(1.0);
			list.Add(Graphics.GetLighterColor(solidBrush.Color, 0.55));
			list2.Add(0.0);
			return Graphics.CreateLinearGradientBrush4Column3D(-90.0, new Point(0.5, 0.0), new Point(0.5, 1.0), list, list2);
		}

		private static Brush GetFrontFaceGradientBrush4Area3D(SolidColorBrush solidBrush)
		{
			List<Color> list = new List<Color>();
			List<double> list2 = new List<double>();
			list.Add(Graphics.GetDarkerColor(solidBrush.Color, 0.7));
			list2.Add(1.0);
			list.Add(Graphics.GetLighterColor(solidBrush.Color, 0.5));
			list2.Add(0.0);
			return Graphics.CreateLinearGradientBrush4Area3D(-90.0, new Point(0.5, 0.0), new Point(0.5, 1.0), list, list2);
		}

		private static Brush GetFrontFaceGradientBrush(SolidColorBrush solidBrush)
		{
			List<Color> list = new List<Color>();
			List<double> list2 = new List<double>();
			list.Add(Graphics.GetDarkerColor(solidBrush.Color, 0.65));
			list2.Add(0.0);
			list.Add(Graphics.GetLighterColor(solidBrush.Color, 0.55));
			list2.Add(1.0);
			return Graphics.CreateLinearGradientBrush(-90.0, new Point(0.0, 0.5), new Point(1.0, 0.5), list, list2);
		}

		internal static Brush GetLeftGradianceBrush(int alpha)
		{
			return new LinearGradientBrush
			{
				GradientStops = new GradientStopCollection(),
				StartPoint = new Point(0.0, 1.0),
				EndPoint = new Point(1.0, 0.0),
				GradientStops = 
				{
					Graphics.GetGradientStop(Color.FromArgb((byte)alpha, 0, 0, 0), 0.0),
					Graphics.GetGradientStop(Color.FromArgb(0, 0, 0, 0), 0.0)
				}
			};
		}

		public static LineSegment GetLineSegment(Point point)
		{
			return new LineSegment
			{
				Point = point
			};
		}

		public static ArcSegment GetArcSegment(Point point, Size size, double rotation, SweepDirection sweep)
		{
			return new ArcSegment
			{
				Point = point,
				Size = size,
				RotationAngle = rotation,
				SweepDirection = SweepDirection.Clockwise
			};
		}

		public static double ConvertScale(double fromScaleMin, double fromScaleMax, double fromValue, double toScaleMin, double toScaleMax)
		{
			return (fromValue - fromScaleMin) * (toScaleMax - toScaleMin) / (fromScaleMax - fromScaleMin) + toScaleMin;
		}

		public static double ValueToPixelPosition(double positionMin, double positionMax, double valueMin, double valueMax, double value)
		{
			return (value - valueMin) / (valueMax - valueMin) * (positionMax - positionMin) + positionMin;
		}

		public static double PixelPositionToValue(double positionMin, double positionMax, double valueMin, double valueMax, double position)
		{
			return (position - positionMin) / (positionMax - positionMin) * (valueMax - valueMin) + valueMin;
		}

		public static GradientStop GetGradientStop(Color color, double stop)
		{
			return new GradientStop
			{
				Color = color,
				Offset = stop
			};
		}

		public static Brush CreateLinearGradientBrush4Area3D(double angle, Point start, Point end, List<Color> colors, List<double> stops)
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
			if (colors.Count != stops.Count)
			{
				throw new Exception("Colors and Stops arrays don't match");
			}
			linearGradientBrush.StartPoint = start;
			linearGradientBrush.EndPoint = end;
			linearGradientBrush.GradientStops = new GradientStopCollection();
			for (int i = 0; i < colors.Count; i++)
			{
				linearGradientBrush.GradientStops.Add(Graphics.GetGradientStop(colors[i], stops[i]));
			}
			return linearGradientBrush;
		}

		public static Brush CreateLinearGradientBrush(double angle, Point start, Point end, List<Color> colors, List<double> stops)
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
			if (colors.Count != stops.Count)
			{
				throw new Exception("Colors and Stops arrays don't match");
			}
			linearGradientBrush.StartPoint = start;
			linearGradientBrush.EndPoint = end;
			linearGradientBrush.GradientStops = new GradientStopCollection();
			for (int i = 0; i < colors.Count; i++)
			{
				linearGradientBrush.GradientStops.Add(Graphics.GetGradientStop(colors[i], stops[i]));
			}
			linearGradientBrush.RelativeTransform = new RotateTransform
			{
				Angle = angle,
				CenterX = 0.5,
				CenterY = 0.5
			};
			return linearGradientBrush;
		}

		public static Brush CreateLinearGradientBrush4Column3D(double angle, Point start, Point end, List<Color> colors, List<double> stops)
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
			if (colors.Count != stops.Count)
			{
				throw new Exception("Colors and Stops arrays don't match");
			}
			linearGradientBrush.StartPoint = start;
			linearGradientBrush.EndPoint = end;
			linearGradientBrush.GradientStops = new GradientStopCollection();
			for (int i = 0; i < colors.Count; i++)
			{
				linearGradientBrush.GradientStops.Add(Graphics.GetGradientStop(colors[i], stops[i]));
			}
			return linearGradientBrush;
		}

		public static Brush CreateSolidColorBrush(Color color)
		{
			return new SolidColorBrush
			{
				Color = color
			};
		}

		internal static Brush GetBevelTopBrush(Brush brush, double angle)
		{
			if (brush == null)
			{
				return null;
			}
			if (typeof(SolidColorBrush).Equals(brush.GetType()))
			{
				SolidColorBrush solidColorBrush = brush as SolidColorBrush;
				List<Color> list = new List<Color>();
				List<double> list2 = new List<double>();
				double num = (double)solidColorBrush.Color.R / 255.0 * 0.9999;
				double num2 = (double)solidColorBrush.Color.G / 255.0 * 0.9999;
				double num3 = (double)solidColorBrush.Color.B / 255.0 * 0.9999;
				list.Add(Graphics.GetLighterColor(solidColorBrush.Color, 0.99));
				list2.Add(0.0);
				list.Add(Graphics.GetLighterColor(solidColorBrush.Color, 1.0 - num, 1.0 - num2, 1.0 - num3));
				list2.Add(0.2);
				list.Add(Graphics.GetLighterColor(solidColorBrush.Color, 1.0 - num, 1.0 - num2, 1.0 - num3));
				list2.Add(0.6);
				list.Add(Graphics.GetLighterColor(solidColorBrush.Color, 0.99));
				list2.Add(1.0);
				return Graphics.CreateLinearGradientBrush(angle, new Point(0.0, 0.5), new Point(1.0, 0.5), list, list2);
			}
			return brush;
		}

		internal static Brush GetBevelSideBrush(double angle, Brush brush)
		{
			return Graphics.GetLightingEnabledBrush(brush, angle, "Linear", new double[]
			{
				0.75,
				0.97
			});
		}

		internal static Brush GetBevelTopBrush(Brush brush)
		{
			return Graphics.GetBevelTopBrush(brush, 90.0);
		}

		internal static Brush GetLightingEnabledBrush3D(Brush brush)
		{
			return Graphics.GetLightingEnabledBrush(brush, "Linear", new double[]
			{
				0.65,
				0.55
			});
		}

		internal static Brush GetLightingEnabledBrush(Brush brush, double angle, string type, double[] colorIntensies)
		{
			if (brush == null)
			{
				return brush;
			}
			SolidColorBrush solidColorBrush = brush as SolidColorBrush;
			if (solidColorBrush != null)
			{
				if (colorIntensies == null)
				{
					colorIntensies = new double[]
					{
						0.745,
						0.99
					};
				}
				SolidColorBrush solidColorBrush2 = brush as SolidColorBrush;
				List<Color> list = new List<Color>();
				List<double> list2 = new List<double>();
				list.Add(Graphics.GetDarkerColor(solidColorBrush2.Color, colorIntensies[0]));
				list2.Add(0.0);
				list.Add(Graphics.GetDarkerColor(solidColorBrush2.Color, colorIntensies[1]));
				list2.Add(1.0);
				if (type == "Radial")
				{
					brush = Graphics.CreateRadialGradientBrush(list, list2);
				}
				else
				{
					brush = Graphics.CreateLinearGradientBrush(angle, new Point(0.0, 0.5), new Point(1.0, 0.5), list, list2);
				}
				return brush;
			}
			return brush;
		}

		internal static Brush GetLightingEnabledBrush(Brush brush, string type, double[] colorIntensies)
		{
			return Graphics.GetLightingEnabledBrush(brush, -90.0, type, colorIntensies);
		}

		public static Brush CreateRadialGradientBrush(List<Color> colors, List<double> stops)
		{
			RadialGradientBrush radialGradientBrush = new RadialGradientBrush();
			if (colors.Count != stops.Count)
			{
				throw new Exception("Colors and Stops arrays don't match");
			}
			radialGradientBrush.GradientStops = new GradientStopCollection();
			for (int i = 0; i < colors.Count; i++)
			{
				radialGradientBrush.GradientStops.Add(Graphics.GetGradientStop(colors[i], stops[i]));
			}
			return radialGradientBrush;
		}

		public static double GetBrushIntensity(Brush brush)
		{
			Color color = default(Color);
			double num = 0.0;
			if (brush == null)
			{
				return 1.0;
			}
			if (brush.GetType().Name == "SolidColorBrush")
			{
				color = (brush as SolidColorBrush).Color;
				num = (double)(color.R + color.G + color.B) / 765.0;
			}
			else if (brush.GetType().Name == "LinearGradientBrush" || brush.GetType().Name == "RadialGradientBrush")
			{
				foreach (GradientStop current in (brush as GradientBrush).GradientStops)
				{
					color = current.Color;
					num += (double)(color.R + color.G + color.B) / 765.0;
				}
				num /= (double)(brush as GradientBrush).GradientStops.Count;
			}
			else
			{
				num = 1.0;
			}
			return num;
		}

		public static KeyValuePair<Color, double> GetColorIntensity(Color color)
		{
			double value = (double)(color.R + color.G + color.B) / 765.0;
			return new KeyValuePair<Color, double>(color, value);
		}

		public static Brush GetDefaultFontColor(double intensity)
		{
			Brush result;
			if (intensity < 0.6)
			{
				result = Graphics.AUTO_WHITE_FONT_BRUSH;
			}
			else
			{
				result = Graphics.AUTO_BLACK_FONT_BRUSH;
			}
			return result;
		}

		internal static bool AreBrushesEqual(Brush first, Brush second)
		{
			if (object.Equals(first, second))
			{
				return true;
			}
			SolidColorBrush solidColorBrush = first as SolidColorBrush;
			if (solidColorBrush != null)
			{
				SolidColorBrush solidColorBrush2 = second as SolidColorBrush;
				if (solidColorBrush2 != null)
				{
					return object.Equals(solidColorBrush.Color, solidColorBrush2.Color);
				}
			}
			return false;
		}

		public static Brush ParseSolidColor(string colorCode)
		{
			return (Brush)XamlReader.Load(new XmlTextReader(new StringReader(string.Format(CultureInfo.InvariantCulture, "<SolidColorBrush xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Color=\"{0}\"></SolidColorBrush>", new object[]
			{
				colorCode
			}))));
		}

		public static Brush GetLighterBrush(Brush brush, double intensity)
		{
			SolidColorBrush solidColorBrush = brush as SolidColorBrush;
			if (solidColorBrush != null)
			{
				return new SolidColorBrush(Graphics.GetLighterColor(solidColorBrush.Color, intensity));
			}
			return brush;
		}

		public static Brush GetDarkerBrush(Brush brush, double intensity)
		{
			SolidColorBrush solidColorBrush = brush as SolidColorBrush;
			if (solidColorBrush != null)
			{
				return new SolidColorBrush(Graphics.GetDarkerColor(solidColorBrush.Color, intensity));
			}
			return brush;
		}

		public static Color GetDarkerColor(Color color, double intensity)
		{
			Color result = default(Color);
			intensity = ((intensity < 0.0 || intensity > 1.0) ? 1.0 : intensity);
			result.R = (byte)((double)color.R * intensity);
			result.G = (byte)((double)color.G * intensity);
			result.B = (byte)((double)color.B * intensity);
			result.A = color.A;
			return result;
		}

		public static Color GetLighterColor(Color color, double intensity)
		{
			Color result = default(Color);
			intensity = ((intensity < 0.0 || intensity > 1.0) ? 1.0 : intensity);
			result.R = (byte)(256.0 - (double)(256 - (int)color.R) * intensity);
			result.G = (byte)(256.0 - (double)(256 - (int)color.G) * intensity);
			result.B = (byte)(256.0 - (double)(256 - (int)color.B) * intensity);
			result.A = color.A;
			return result;
		}

		public static Color GetLighterColor(Color color, double intensityR, double intensityG, double intensityB)
		{
			Color result = default(Color);
			intensityR = ((intensityR < 0.0 || intensityR > 1.0) ? 1.0 : intensityR);
			intensityG = ((intensityG < 0.0 || intensityG > 1.0) ? 1.0 : intensityG);
			intensityB = ((intensityB < 0.0 || intensityB > 1.0) ? 1.0 : intensityB);
			result.R = (byte)(256.0 - (double)(256 - (int)color.R) * intensityR);
			result.G = (byte)(256.0 - (double)(256 - (int)color.G) * intensityG);
			result.B = (byte)(256.0 - (double)(256 - (int)color.B) * intensityB);
			result.A = color.A;
			return result;
		}

		public static DoubleCollection LineStyleToStrokeDashArray(string lineStyle)
		{
			DoubleCollection result = null;
			if (lineStyle != null)
			{
				if (!(lineStyle == "Solid"))
				{
					if (!(lineStyle == "Dashed"))
					{
						if (lineStyle == "Dotted")
						{
							result = new DoubleCollection
							{
								1.0,
								2.0,
								1.0,
								2.0
							};
						}
					}
					else
					{
						result = new DoubleCollection
						{
							4.0,
							4.0,
							4.0,
							4.0
						};
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static Brush LightingBrush(bool lightingEnabled)
		{
			Brush result;
			if (lightingEnabled)
			{
				string s = string.Format("<LinearGradientBrush EndPoint=\"0.5,1\" StartPoint=\"0.5,0\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n                                                <GradientStop Color=\"#A0FFFFFF\" Offset=\"0\"/>\r\n                                                <GradientStop Color=\"#00FFFFFF\" Offset=\"1\"/>\r\n                                          </LinearGradientBrush>", new object[0]);
				result = (Brush)XamlReader.Load(new XmlTextReader(new StringReader(s)));
			}
			else
			{
				result = Graphics.TRANSPARENT_BRUSH;
			}
			return result;
		}
	}
}
