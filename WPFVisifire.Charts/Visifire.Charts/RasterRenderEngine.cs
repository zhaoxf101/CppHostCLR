using System;
using System.Windows;
using System.Windows.Media;
using Visifire.Commons;

namespace Visifire.Charts
{
	public static class RasterRenderEngine
	{
		private static int[] DottedArray;

		private static int[] DashArray;

		private static Point[] GetParallel(Point pointA, Point pointB, int offsetPixels, AlignmentY alignment)
		{
			double num = Math.Sqrt((pointA.X - pointB.X) * (pointA.X - pointB.X) + (pointA.Y - pointB.Y) * (pointA.Y - pointB.Y));
			Point point = default(Point);
			Point point2 = default(Point);
			if (alignment == AlignmentY.Top)
			{
				point.X = (double)((int)(pointA.X + (double)offsetPixels * (pointB.Y - pointA.Y) / num));
				point2.X = (double)((int)(pointB.X + (double)offsetPixels * (pointB.Y - pointA.Y) / num));
				point.Y = (double)((int)(pointA.Y + (double)offsetPixels * (pointA.X - pointB.X) / num));
				point2.Y = (double)((int)(pointB.Y + (double)offsetPixels * (pointA.X - pointB.X) / num));
			}
			else
			{
				point.X = (double)((int)(pointA.X - (double)offsetPixels * (pointB.Y - pointA.Y) / num));
				point2.X = (double)((int)(pointB.X - (double)offsetPixels * (pointB.Y - pointA.Y) / num));
				point.Y = (double)((int)(pointA.Y - (double)offsetPixels * (pointA.X - pointB.X) / num));
				point2.Y = (double)((int)(pointB.Y - (double)offsetPixels * (pointA.X - pointB.X) / num));
			}
			return new Point[]
			{
				point,
				point2
			};
		}

		public static void DrawLine(WriteableBitmapAdapter wb, Point pointA, Point pointB, RasterLineStyle style)
		{
			style.Stroke.A = (byte)((int)((double)style.Stroke.A * style.Opacity));
			if (style.StrokeStyle != LineStyles.Solid)
			{
				if (style.StrokeStyle != LineStyles.Dashed)
				{
					int[] arg_36_0 = RasterRenderEngine.DottedArray;
				}
				else
				{
					int[] arg_3E_0 = RasterRenderEngine.DashArray;
				}
			}
			if (style.StrokeThickness <= 1.0)
			{
				wb.DrawLine((int)pointA.X, (int)pointA.Y, (int)pointB.X, (int)pointB.Y, style.Stroke);
				return;
			}
			if (style.StrokeThickness != 2.0)
			{
				int num = (int)(style.StrokeThickness / 2.0);
				Point[] parallel = RasterRenderEngine.GetParallel(pointA, pointB, num, AlignmentY.Top);
				Point[] parallel2 = RasterRenderEngine.GetParallel(pointA, pointB, num, AlignmentY.Bottom);
				Color stroke = style.Stroke;
				stroke.A = (byte)((double)stroke.A * style.Opacity);
				if (style.IsAntiAliased)
				{
					wb.DrawLineAa((int)parallel[0].X, (int)parallel[0].Y, (int)parallel[1].X, (int)parallel[1].Y, stroke);
					wb.DrawLineAa((int)parallel2[0].X, (int)parallel2[0].Y, (int)parallel2[1].X, (int)parallel2[1].Y, stroke);
					if (style.PenLineJoinA.HasValue)
					{
						wb.FillEllipseCentered((int)pointA.X, (int)pointA.Y, num, num, stroke);
					}
					if (style.PenLineJoinB.HasValue)
					{
						wb.FillEllipseCentered((int)pointB.X, (int)pointB.Y, num, num, stroke);
					}
				}
				wb.FillPolygon(new int[]
				{
					(int)parallel[0].X,
					(int)parallel[0].Y,
					(int)parallel[1].X,
					(int)parallel[1].Y,
					(int)parallel2[1].X,
					(int)parallel2[1].Y,
					(int)parallel2[0].X,
					(int)parallel2[0].Y,
					(int)parallel[0].X,
					(int)parallel[0].Y
				}, stroke);
				return;
			}
			if (style.IsAntiAliased)
			{
				wb.DrawLineAa((int)pointA.X, (int)pointA.Y, (int)pointB.X, (int)pointB.Y, style.Stroke);
				return;
			}
			if (pointA.Y == pointB.Y)
			{
				wb.DrawRectangle((int)pointA.X, (int)pointA.Y - 1, (int)pointB.X, (int)pointB.Y + 1, style.Stroke);
				return;
			}
			Point[] parallel3 = RasterRenderEngine.GetParallel(pointA, pointB, 1, AlignmentY.Top);
			wb.FillPolygon(new int[]
			{
				(int)pointA.X,
				(int)pointA.Y,
				(int)pointB.X,
				(int)pointB.Y,
				(int)parallel3[1].X,
				(int)parallel3[1].Y,
				(int)parallel3[0].X,
				(int)parallel3[0].Y,
				(int)pointA.X,
				(int)pointA.Y
			}, style.Stroke);
		}

		public static void DrawMarkerCentered(WriteableBitmapAdapter wb, IDataPoint dataPoint, RasterMarkerStyle parentMarkerStyle, bool isLineChart)
		{
			if (dataPoint != null)
			{
				if (isLineChart)
				{
					Brush brush = (dataPoint.DpInfo.InternalColor == null) ? dataPoint.Parent._internalColor : dataPoint.DpInfo.InternalColor;
					parentMarkerStyle.Stroke = (brush as SolidColorBrush).Color;
				}
				RasterRenderEngine.DrawMarkerCentered(wb, dataPoint.DpInfo.RasterVisualPosition, parentMarkerStyle);
			}
		}

		public static void DrawMarkerCentered(WriteableBitmapAdapter wb, Point pointA, RasterMarkerStyle style)
		{
			switch (style.MarkerType)
			{
			case MarkerTypes.Circle:
				RasterRenderEngine.DrawEllipse(wb, pointA, style);
				return;
			case MarkerTypes.Square:
				RasterRenderEngine.DrawRectCentered(wb, pointA, style);
				return;
			case MarkerTypes.Triangle:
			{
				double num = style.Width / 2.0;
				wb.FillTriangle((int)pointA.X, (int)(pointA.Y - num), (int)(pointA.X - num), (int)(pointA.Y + num), (int)(pointA.X + num), (int)(pointA.Y + num), style.Fill);
				if (style.StrokeThickness > 0.0 && style.Stroke != Colors.Transparent)
				{
					wb.DrawTriangle((int)pointA.X, (int)(pointA.Y - num), (int)(pointA.X - num), (int)(pointA.Y + num), (int)(pointA.X + num), (int)(pointA.Y + num), style.Stroke);
					return;
				}
				break;
			}
			case MarkerTypes.Cross:
			{
				double num = style.Width / 2.0;
				pointA.X -= num;
				pointA.Y -= num;
				RasterRenderEngine.DrawLine(wb, pointA, new Point(pointA.X + style.Width, pointA.Y + style.Width), new RasterLineStyle(style));
				RasterRenderEngine.DrawLine(wb, new Point(pointA.X, pointA.Y + style.Height), new Point(pointA.X + style.Width, pointA.Y), new RasterLineStyle(style));
				return;
			}
			case MarkerTypes.Diamond:
			{
				double num = style.Width / 2.0;
				wb.FillPolygon(new int[]
				{
					(int)pointA.X,
					(int)(pointA.Y - num),
					(int)(pointA.X - num),
					(int)pointA.Y,
					(int)pointA.X,
					(int)(pointA.Y + num),
					(int)(pointA.X + num),
					(int)pointA.Y,
					(int)pointA.X,
					(int)(pointA.Y - num)
				}, style.Fill);
				if (style.StrokeThickness > 0.0 && style.Stroke != Colors.Transparent)
				{
					wb.DrawPolyline(new int[]
					{
						(int)pointA.X,
						(int)(pointA.Y - num),
						(int)(pointA.X - num),
						(int)pointA.Y,
						(int)pointA.X,
						(int)(pointA.Y + num),
						(int)(pointA.X + num),
						(int)pointA.Y,
						(int)pointA.X,
						(int)(pointA.Y - num)
					}, style.Stroke);
					return;
				}
				break;
			}
			case MarkerTypes.Line:
			{
				double num2 = (style.StrokeThickness == 0.0) ? 4.0 : style.StrokeThickness;
				double num = style.Width / 2.0;
				wb.FillPolygon(new int[]
				{
					(int)pointA.X,
					(int)(pointA.Y - num2),
					(int)pointA.X,
					(int)(pointA.Y + num2),
					(int)(pointA.X + num),
					(int)(pointA.Y + num2),
					(int)(pointA.X + num),
					(int)(pointA.Y - num2),
					(int)pointA.X,
					(int)(pointA.Y - num)
				}, style.Fill);
				break;
			}
			default:
				return;
			}
		}

		public static void DrawRect(WriteableBitmapAdapter wb, Point leftTopPoint, RasterShapeStyle style)
		{
			Color fill = style.Fill;
			fill.A = (byte)((double)fill.A * style.Opacity);
			int num = (int)leftTopPoint.X;
			int num2 = (int)leftTopPoint.Y;
			int num3 = (int)(leftTopPoint.X + style.Width);
			int num4 = (int)(leftTopPoint.Y + style.Height);
			wb.FillRectangle(num, num2, num3, num4, fill);
			if (style.StrokeThickness > 0.0 && style.Stroke != Colors.Transparent)
			{
				Color stroke = style.Stroke;
				stroke.A = (byte)((double)stroke.A * style.Opacity);
				int num5 = (int)style.StrokeThickness / 2;
				if (num5 == 0)
				{
					wb.DrawRectangle(num - 1, num2 - 1, num3 + 1, num4 + 1, stroke);
					return;
				}
				for (int i = num5; i > -num5; i--)
				{
					wb.DrawRectangle(num - i, num2 - i, num3 + i, num4 + i, stroke);
				}
			}
		}

		public static void DrawRectCentered(WriteableBitmapAdapter wb, Point centerPoint, RasterShapeStyle style)
		{
			Color fill = style.Fill;
			fill.A = (byte)((double)fill.A * style.Opacity);
			double num = style.Height / 2.0;
			double num2 = style.Width / 2.0;
			int num3 = (int)(centerPoint.X - num2);
			int num4 = (int)(centerPoint.Y - num);
			int num5 = (int)(centerPoint.X + num2);
			int num6 = (int)(centerPoint.Y + num);
			wb.FillRectangle(num3, num4, num5, num6, fill);
			if (style.StrokeThickness > 0.0 && style.Stroke != Colors.Transparent)
			{
				Color stroke = style.Stroke;
				stroke.A = (byte)((double)stroke.A * style.Opacity);
				int num7 = (int)style.StrokeThickness / 2;
				if (num7 == 0)
				{
					wb.DrawRectangle(num3 - 1, num4 - 1, num5 + 1, num6 + 1, stroke);
					return;
				}
				for (int i = num7; i > -num7; i--)
				{
					wb.DrawRectangle(num3 - i, num4 - i, num5 + i, num6 + i, stroke);
				}
			}
		}

		private static void DrawEllipse(WriteableBitmapAdapter wb, Point center, RasterShapeStyle style)
		{
			Color fill = style.Fill;
			fill.A = (byte)((double)fill.A * style.Opacity);
			int num = (int)style.Width / 2;
			int num2 = (int)style.Height / 2;
			wb.FillEllipseCentered((int)center.X, (int)center.Y, num, num2, fill);
			if (style.StrokeThickness > 0.0 && style.Stroke != Colors.Transparent)
			{
				Color stroke = style.Stroke;
				stroke.A = (byte)((double)stroke.A * style.Opacity);
				int num3 = (int)style.StrokeThickness / 2;
				for (int i = -num3; i <= num3; i++)
				{
					wb.DrawEllipseCentered((int)center.X, (int)center.Y, num + i, num2 + i, stroke);
				}
			}
		}

		static RasterRenderEngine()
		{
			// Note: this type is marked as 'beforefieldinit'.
			int[] array = new int[2];
			array[0] = 3;
			RasterRenderEngine.DottedArray = array;
			RasterRenderEngine.DashArray = new int[]
			{
				4,
				6
			};
		}
	}
}
