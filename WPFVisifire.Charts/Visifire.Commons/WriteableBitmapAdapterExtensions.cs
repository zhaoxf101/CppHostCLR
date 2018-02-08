using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Visifire.Commons
{
	public static class WriteableBitmapAdapterExtensions
	{
		private const int SizeOfArgb = 4;

		private const float StepFactor = 2f;

		public static void DrawLineBresenham(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.DrawLineBresenham(x1, y1, x2, y2, color2);
		}

		public static void DrawLineBresenham(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int color)
		{
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			int[] pixels = bmp.Pixels;
			int num = x2 - x1;
			int num2 = y2 - y1;
			int num3 = 0;
			if (num < 0)
			{
				num = -num;
				num3 = -1;
			}
			else if (num > 0)
			{
				num3 = 1;
			}
			int num4 = 0;
			if (num2 < 0)
			{
				num2 = -num2;
				num4 = -1;
			}
			else if (num2 > 0)
			{
				num4 = 1;
			}
			int num5;
			int num6;
			int num7;
			int num8;
			int num9;
			int num10;
			if (num > num2)
			{
				num5 = num3;
				num6 = 0;
				num7 = num3;
				num8 = num4;
				num9 = num2;
				num10 = num;
			}
			else
			{
				num5 = 0;
				num6 = num4;
				num7 = num3;
				num8 = num4;
				num9 = num;
				num10 = num2;
			}
			int num11 = x1;
			int num12 = y1;
			int num13 = num10 >> 1;
			if (num12 < pixelHeight && num12 >= 0 && num11 < pixelWidth && num11 >= 0)
			{
				pixels[num12 * pixelWidth + num11] = color;
			}
			for (int i = 0; i < num10; i++)
			{
				num13 -= num9;
				if (num13 < 0)
				{
					num13 += num10;
					num11 += num7;
					num12 += num8;
				}
				else
				{
					num11 += num5;
					num12 += num6;
				}
				if (num12 < pixelHeight && num12 >= 0 && num11 < pixelWidth && num11 >= 0)
				{
					pixels[num12 * pixelWidth + num11] = color;
				}
			}
		}

		public static void DrawLineDDA(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.DrawLineDDA(x1, y1, x2, y2, color2);
		}

		public static void DrawLineDDA(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int color)
		{
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			int[] pixels = bmp.Pixels;
			int num = x2 - x1;
			int num2 = y2 - y1;
			int num3 = (num2 >= 0) ? num2 : (-num2);
			int num4 = (num >= 0) ? num : (-num);
			if (num4 > num3)
			{
				num3 = num4;
			}
			if (num3 != 0)
			{
				float num5 = (float)num / (float)num3;
				float num6 = (float)num2 / (float)num3;
				float num7 = (float)x1;
				float num8 = (float)y1;
				for (int i = 0; i < num3; i++)
				{
					if (num8 < (float)pixelHeight && num8 >= 0f && num7 < (float)pixelWidth && num7 >= 0f)
					{
						pixels[(int)num8 * pixelWidth + (int)num7] = color;
					}
					num7 += num5;
					num8 += num6;
				}
			}
		}

		public static void DrawLine(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.DrawLine(x1, y1, x2, y2, color2);
		}

		public static void DrawLine(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int color)
		{
			WriteableBitmapAdapterExtensions.DrawLine(bmp.Pixels, bmp.PixelWidth, bmp.PixelHeight, x1, y1, x2, y2, color);
		}

		public static void DrawLine(int[] pixels, int pixelWidth, int pixelHeight, int x1, int y1, int x2, int y2, int color)
		{
			int num = x2 - x1;
			int num2 = y2 - y1;
			int num3;
			if (num2 >= 0)
			{
				num3 = num2;
			}
			else
			{
				num3 = -num2;
			}
			int num4;
			if (num >= 0)
			{
				num4 = num;
			}
			else
			{
				num4 = -num;
			}
			if (num4 > num3)
			{
				if (num < 0)
				{
					int num5 = x1;
					x1 = x2;
					x2 = num5;
					num5 = y1;
					y1 = y2;
					y2 = num5;
				}
				int num6 = (num2 << 8) / num;
				int num7 = y1 << 8;
				int num8 = y2 << 8;
				int num9 = pixelHeight << 8;
				if (y1 < y2)
				{
					if (y1 >= pixelHeight || y2 < 0)
					{
						return;
					}
					if (num7 < 0)
					{
						if (num6 == 0)
						{
							return;
						}
						int num10 = num7;
						num7 = num6 - 1 + (num7 + 1) % num6;
						x1 += (num7 - num10) / num6;
					}
					if (num8 >= num9 && num6 != 0)
					{
						num8 = num9 - 1 - (num9 - 1 - num7) % num6;
						x2 = x1 + (num8 - num7) / num6;
					}
				}
				else
				{
					if (y2 >= pixelHeight || y1 < 0)
					{
						return;
					}
					if (num7 >= num9)
					{
						if (num6 == 0)
						{
							return;
						}
						int num11 = num7;
						num7 = num9 - 1 + (num6 - (num9 - 1 - num11) % num6);
						x1 += (num7 - num11) / num6;
					}
					if (num8 < 0 && num6 != 0)
					{
						num8 = num7 % num6;
						x2 = x1 + (num8 - num7) / num6;
					}
				}
				if (x1 < 0)
				{
					num7 -= num6 * x1;
					x1 = 0;
				}
				if (x2 >= pixelWidth)
				{
					x2 = pixelWidth - 1;
				}
				int num12 = num7;
				int num13 = num12 >> 8;
				int num14 = num13;
				int num15 = x1 + num13 * pixelWidth;
				int num16 = (num6 < 0) ? (1 - pixelWidth) : (1 + pixelWidth);
				for (int i = x1; i <= x2; i++)
				{
					pixels[num15] = color;
					num12 += num6;
					num13 = num12 >> 8;
					if (num13 != num14)
					{
						num14 = num13;
						num15 += num16;
					}
					else
					{
						num15++;
					}
				}
				return;
			}
			if (num3 == 0)
			{
				return;
			}
			if (num2 < 0)
			{
				int num17 = x1;
				x1 = x2;
				x2 = num17;
				num17 = y1;
				y1 = y2;
				y2 = num17;
			}
			int num18 = x1 << 8;
			int num19 = x2 << 8;
			int num20 = pixelWidth << 8;
			int num21 = (num << 8) / num2;
			if (x1 < x2)
			{
				if (x1 >= pixelWidth || x2 < 0)
				{
					return;
				}
				if (num18 < 0)
				{
					if (num21 == 0)
					{
						return;
					}
					int num22 = num18;
					num18 = num21 - 1 + (num18 + 1) % num21;
					y1 += (num18 - num22) / num21;
				}
				if (num19 >= num20 && num21 != 0)
				{
					num19 = num20 - 1 - (num20 - 1 - num18) % num21;
					y2 = y1 + (num19 - num18) / num21;
				}
			}
			else
			{
				if (x2 >= pixelWidth || x1 < 0)
				{
					return;
				}
				if (num18 >= num20)
				{
					if (num21 == 0)
					{
						return;
					}
					int num23 = num18;
					num18 = num20 - 1 + (num21 - (num20 - 1 - num23) % num21);
					y1 += (num18 - num23) / num21;
				}
				if (num19 < 0 && num21 != 0)
				{
					num19 = num18 % num21;
					y2 = y1 + (num19 - num18) / num21;
				}
			}
			if (y1 < 0)
			{
				num18 -= num21 * y1;
				y1 = 0;
			}
			if (y2 >= pixelHeight)
			{
				y2 = pixelHeight - 1;
			}
			int num24 = num18 + (y1 * pixelWidth << 8);
			int num25 = (pixelWidth << 8) + num21;
			for (int j = y1; j <= y2; j++)
			{
				pixels[num24 >> 8] = color;
				num24 += num25;
			}
		}

		public static void DrawLineAa(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.DrawLineAa(x1, y1, x2, y2, color2);
		}

		public static void DrawLineAa(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int color)
		{
			WriteableBitmapAdapterExtensions.DrawLineAa(bmp.Pixels, bmp.PixelWidth, bmp.PixelHeight, x1, y1, x2, y2, color);
		}

		public static void DrawLineAa(int[] pixels, int pixelWidth, int pixelHeight, int x1, int y1, int x2, int y2, int color)
		{
			if (x1 == x2 && y1 == y2)
			{
				return;
			}
			if (x1 < 1)
			{
				x1 = 1;
			}
			if (x1 > pixelWidth - 2)
			{
				x1 = pixelWidth - 2;
			}
			if (y1 < 1)
			{
				y1 = 1;
			}
			if (y1 > pixelHeight - 2)
			{
				y1 = pixelHeight - 2;
			}
			if (x2 < 1)
			{
				x2 = 1;
			}
			if (x2 > pixelWidth - 2)
			{
				x2 = pixelWidth - 2;
			}
			if (y2 < 1)
			{
				y2 = 1;
			}
			if (y2 > pixelHeight - 2)
			{
				y2 = pixelHeight - 2;
			}
			int num = y1 * pixelWidth + x1;
			int num2 = x2 - x1;
			int num3 = y2 - y1;
			int num4 = color >> 24 & 255;
			uint srb = (uint)(color & 16711935);
			uint sg = (uint)(color >> 8 & 255);
			int num5 = num2;
			int num6 = num3;
			if (num2 < 0)
			{
				num5 = -num2;
			}
			if (num3 < 0)
			{
				num6 = -num3;
			}
			int num7;
			int num8;
			int num9;
			int num10;
			int num11;
			int num12;
			if (num5 > num6)
			{
				num7 = num5;
				num8 = num6;
				num9 = x2;
				num10 = y2;
				num11 = 1;
				num12 = pixelWidth;
				if (num2 < 0)
				{
					num11 = -num11;
				}
				if (num3 < 0)
				{
					num12 = -num12;
				}
			}
			else
			{
				num7 = num6;
				num8 = num5;
				num9 = y2;
				num10 = x2;
				num11 = pixelWidth;
				num12 = 1;
				if (num3 < 0)
				{
					num11 = -num11;
				}
				if (num2 < 0)
				{
					num12 = -num12;
				}
			}
			int num13 = num9 + num7;
			int num14 = (num8 << 1) - num7;
			int num15 = num8 << 1;
			int num16 = num8 - num7 << 1;
			double num17 = 1.0 / (4.0 * Math.Sqrt((double)(num7 * num7 + num8 * num8)));
			double num18 = 0.75 - 2.0 * ((double)num7 * num17);
			int num19 = (int)(num17 * 1024.0);
			int num20 = (int)(num18 * 1024.0 * (double)num4);
			int num21 = (int)(768.0 * (double)num4);
			int num22 = num19 * num4;
			int num23 = num7 * num22;
			int num24 = num14 * num22;
			int num25 = 0;
			int num26 = num15 * num22;
			int num27 = num16 * num22;
			do
			{
				WriteableBitmapAdapterExtensions.AlphaBlendNormalOnPremultiplied(pixels, num, num21 - num25 >> 10, srb, sg);
				WriteableBitmapAdapterExtensions.AlphaBlendNormalOnPremultiplied(pixels, num + num12, num20 + num25 >> 10, srb, sg);
				WriteableBitmapAdapterExtensions.AlphaBlendNormalOnPremultiplied(pixels, num - num12, num20 - num25 >> 10, srb, sg);
				if (num14 < 0)
				{
					num25 = num24 + num23;
					num14 += num15;
					num24 += num26;
				}
				else
				{
					num25 = num24 - num23;
					num14 += num16;
					num24 += num27;
					num10++;
					num += num12;
				}
				num9++;
				num += num11;
			}
			while (num9 < num13);
		}

		private static void AlphaBlendNormalOnPremultiplied(int[] pixels, int index, int sa, uint srb, uint sg)
		{
			uint num = (uint)pixels[index];
			uint num2 = num >> 24;
			uint num3 = num >> 8 & 255u;
			uint num4 = num & 16711935u;
			pixels[index] = (int)((long)sa + (long)((ulong)num2 * (ulong)((long)(255 - sa)) * 32897uL >> 23) << 24 | (long)((ulong)(sg - num3) * (ulong)((long)sa) + (ulong)((ulong)num3 << 8) & (ulong)-256) | (long)(((ulong)(srb - num4) * (ulong)((long)sa) >> 8) + (ulong)num4 & 16711935uL));
		}

		public static void DrawPolyline(this WriteableBitmapAdapter bmp, int[] points, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.DrawPolyline(points, color2);
		}

		public static void DrawPolyline(this WriteableBitmapAdapter bmp, int[] points, int color)
		{
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			int[] pixels = bmp.Pixels;
			int num = points[0];
			int num2 = points[1];
			if (num < 0)
			{
				num = 0;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num > pixelWidth)
			{
				num = pixelWidth;
			}
			if (num2 > pixelHeight)
			{
				num2 = pixelHeight;
			}
			for (int i = 2; i < points.Length; i += 2)
			{
				int num3 = points[i];
				int num4 = points[i + 1];
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 < 0)
				{
					num4 = 0;
				}
				if (num3 > pixelWidth)
				{
					num3 = pixelWidth;
				}
				if (num4 > pixelHeight)
				{
					num4 = pixelHeight;
				}
				WriteableBitmapAdapterExtensions.DrawLine(pixels, pixelWidth, pixelHeight, num, num2, num3, num4, color);
				num = num3;
				num2 = num4;
			}
		}

		public static void DrawTriangle(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int x3, int y3, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.DrawTriangle(x1, y1, x2, y2, x3, y3, color2);
		}

		public static void DrawTriangle(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int x3, int y3, int color)
		{
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			int[] pixels = bmp.Pixels;
			WriteableBitmapAdapterExtensions.DrawLine(pixels, pixelWidth, pixelHeight, x1, y1, x2, y2, color);
			WriteableBitmapAdapterExtensions.DrawLine(pixels, pixelWidth, pixelHeight, x2, y2, x3, y3, color);
			WriteableBitmapAdapterExtensions.DrawLine(pixels, pixelWidth, pixelHeight, x3, y3, x1, y1, color);
		}

		public static void DrawQuad(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.DrawQuad(x1, y1, x2, y2, x3, y3, x4, y4, color2);
		}

		public static void DrawQuad(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4, int color)
		{
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			int[] pixels = bmp.Pixels;
			WriteableBitmapAdapterExtensions.DrawLine(pixels, pixelWidth, pixelHeight, x1, y1, x2, y2, color);
			WriteableBitmapAdapterExtensions.DrawLine(pixels, pixelWidth, pixelHeight, x2, y2, x3, y3, color);
			WriteableBitmapAdapterExtensions.DrawLine(pixels, pixelWidth, pixelHeight, x3, y3, x4, y4, color);
			WriteableBitmapAdapterExtensions.DrawLine(pixels, pixelWidth, pixelHeight, x4, y4, x1, y1, color);
		}

		public static void DrawRectangle(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.DrawRectangle(x1, y1, x2, y2, color2);
		}

		public static void DrawRectangle(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int color)
		{
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			int[] pixels = bmp.Pixels;
			if ((x1 < 0 && x2 < 0) || (y1 < 0 && y2 < 0) || (x1 >= pixelWidth && x2 >= pixelWidth) || (y1 >= pixelHeight && y2 >= pixelHeight))
			{
				return;
			}
			if (x1 < 0)
			{
				x1 = 0;
			}
			if (y1 < 0)
			{
				y1 = 0;
			}
			if (x2 < 0)
			{
				x2 = 0;
			}
			if (y2 < 0)
			{
				y2 = 0;
			}
			if (x1 > pixelWidth)
			{
				x1 = pixelWidth;
			}
			if (y1 > pixelHeight)
			{
				y1 = pixelHeight;
			}
			if (x2 > pixelWidth)
			{
				x2 = pixelWidth;
			}
			if (y2 > pixelHeight)
			{
				y2 = pixelHeight;
			}
			int num = y1 * pixelWidth;
			int num2 = y2 * pixelWidth;
			int num3 = num2 - pixelWidth + x1;
			int num4 = num + x2;
			int num5 = num + x1;
			for (int i = num5; i < num4; i++)
			{
				pixels[i] = color;
				pixels[num3] = color;
				num3++;
			}
			num4 = num5 + pixelWidth;
			num3 -= pixelWidth;
			for (int j = num + x2 - 1 + pixelWidth; j < num3; j += pixelWidth)
			{
				pixels[j] = color;
				pixels[num4] = color;
				num4 += pixelWidth;
			}
		}

		public static void DrawEllipse(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.DrawEllipse(x1, y1, x2, y2, color2);
		}

		public static void DrawEllipse(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int color)
		{
			int num = x2 - x1 >> 1;
			int num2 = y2 - y1 >> 1;
			int xc = x1 + num;
			int yc = y1 + num2;
			bmp.DrawEllipseCentered(xc, yc, num, num2, color);
		}

		public static void DrawEllipseCentered(this WriteableBitmapAdapter bmp, int xc, int yc, int xr, int yr, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.DrawEllipseCentered(xc, yc, xr, yr, color2);
		}

		public static void DrawEllipseCentered(this WriteableBitmapAdapter bmp, int xc, int yc, int xr, int yr, int color)
		{
			int[] pixels = bmp.Pixels;
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			if (xr < 1 || yr < 1)
			{
				return;
			}
			int num = xr;
			int num2 = 0;
			int num3 = xr * xr << 1;
			int num4 = yr * yr << 1;
			int num5 = yr * yr * (1 - (xr << 1));
			int num6 = xr * xr;
			int num7 = 0;
			int i = num4 * xr;
			int num8 = 0;
			int num9;
			int num10;
			int num11;
			int num12;
			while (i >= num8)
			{
				num9 = yc + num2;
				num10 = yc - num2;
				if (num9 < 0)
				{
					num9 = 0;
				}
				if (num9 >= pixelHeight)
				{
					num9 = pixelHeight - 1;
				}
				if (num10 < 0)
				{
					num10 = 0;
				}
				if (num10 >= pixelHeight)
				{
					num10 = pixelHeight - 1;
				}
				num11 = num9 * pixelWidth;
				num12 = num10 * pixelWidth;
				int num13 = xc + num;
				int num14 = xc - num;
				if (num13 < 0)
				{
					num13 = 0;
				}
				if (num13 >= pixelWidth)
				{
					num13 = pixelWidth - 1;
				}
				if (num14 < 0)
				{
					num14 = 0;
				}
				if (num14 >= pixelWidth)
				{
					num14 = pixelWidth - 1;
				}
				pixels[num13 + num11] = color;
				pixels[num14 + num11] = color;
				pixels[num14 + num12] = color;
				pixels[num13 + num12] = color;
				num2++;
				num8 += num3;
				num7 += num6;
				num6 += num3;
				if (num5 + (num7 << 1) > 0)
				{
					num--;
					i -= num4;
					num7 += num5;
					num5 += num4;
				}
			}
			num = 0;
			num2 = yr;
			num9 = yc + num2;
			num10 = yc - num2;
			if (num9 < 0)
			{
				num9 = 0;
			}
			if (num9 >= pixelHeight)
			{
				num9 = pixelHeight - 1;
			}
			if (num10 < 0)
			{
				num10 = 0;
			}
			if (num10 >= pixelHeight)
			{
				num10 = pixelHeight - 1;
			}
			num11 = num9 * pixelWidth;
			num12 = num10 * pixelWidth;
			num5 = yr * yr;
			num6 = xr * xr * (1 - (yr << 1));
			num7 = 0;
			i = 0;
			num8 = num3 * yr;
			while (i <= num8)
			{
				int num13 = xc + num;
				int num14 = xc - num;
				if (num13 < 0)
				{
					num13 = 0;
				}
				if (num13 >= pixelWidth)
				{
					num13 = pixelWidth - 1;
				}
				if (num14 < 0)
				{
					num14 = 0;
				}
				if (num14 >= pixelWidth)
				{
					num14 = pixelWidth - 1;
				}
				pixels[num13 + num11] = color;
				pixels[num14 + num11] = color;
				pixels[num14 + num12] = color;
				pixels[num13 + num12] = color;
				num++;
				i += num4;
				num7 += num5;
				num5 += num4;
				if (num6 + (num7 << 1) > 0)
				{
					num2--;
					num9 = yc + num2;
					num10 = yc - num2;
					if (num9 < 0)
					{
						num9 = 0;
					}
					if (num9 >= pixelHeight)
					{
						num9 = pixelHeight - 1;
					}
					if (num10 < 0)
					{
						num10 = 0;
					}
					if (num10 >= pixelHeight)
					{
						num10 = pixelHeight - 1;
					}
					num11 = num9 * pixelWidth;
					num12 = num10 * pixelWidth;
					num8 -= num3;
					num7 += num6;
					num6 += num3;
				}
			}
		}

		public static void FillRectangle(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.FillRectangle(x1, y1, x2, y2, color2);
		}

		public static void FillRectangle(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int color)
		{
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			int[] pixels = bmp.Pixels;
			if ((x1 < 0 && x2 < 0) || (y1 < 0 && y2 < 0) || (x1 >= pixelWidth && x2 >= pixelWidth) || (y1 >= pixelHeight && y2 >= pixelHeight))
			{
				return;
			}
			if (x1 < 0)
			{
				x1 = 0;
			}
			if (y1 < 0)
			{
				y1 = 0;
			}
			if (x2 < 0)
			{
				x2 = 0;
			}
			if (y2 < 0)
			{
				y2 = 0;
			}
			if (x1 > pixelWidth)
			{
				x1 = pixelWidth;
			}
			if (y1 > pixelHeight)
			{
				y1 = pixelHeight;
			}
			if (x2 > pixelWidth)
			{
				x2 = pixelWidth;
			}
			if (y2 > pixelHeight)
			{
				y2 = pixelHeight;
			}
			int num = y1 * pixelWidth;
			int num2 = num + x1;
			int num3 = num + x2;
			for (int i = num2; i < num3; i++)
			{
				pixels[i] = color;
			}
			int count = (x2 - x1) * 4;
			int srcOffset = num2 * 4;
			int num4 = y2 * pixelWidth + x1;
			for (int j = num2 + pixelWidth; j <= num4; j += pixelWidth)
			{
				if (j < pixels.Length)
				{
					Buffer.BlockCopy(pixels, srcOffset, pixels, j * 4, count);
				}
			}
		}

		public static void FillEllipse(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.FillEllipse(x1, y1, x2, y2, color2);
		}

		public static void FillEllipse(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int color)
		{
			int num = x2 - x1 >> 1;
			int num2 = y2 - y1 >> 1;
			int xc = x1 + num;
			int yc = y1 + num2;
			bmp.FillEllipseCentered(xc, yc, num, num2, color);
		}

		public static void FillEllipseCentered(this WriteableBitmapAdapter bmp, int xc, int yc, int xr, int yr, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.FillEllipseCentered(xc, yc, xr, yr, color2);
		}

		public static void FillEllipseCentered(this WriteableBitmapAdapter bmp, int xc, int yc, int xr, int yr, int color)
		{
			int[] pixels = bmp.Pixels;
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			if (xr < 1 || yr < 1)
			{
				return;
			}
			int num = xr;
			int num2 = 0;
			int num3 = xr * xr << 1;
			int num4 = yr * yr << 1;
			int num5 = yr * yr * (1 - (xr << 1));
			int num6 = xr * xr;
			int num7 = 0;
			int i = num4 * xr;
			int num8 = 0;
			int num9;
			int num10;
			int num11;
			int num12;
			while (i >= num8)
			{
				num9 = yc + num2;
				num10 = yc - num2;
				if (num9 < 0)
				{
					num9 = 0;
				}
				if (num9 >= pixelHeight)
				{
					num9 = pixelHeight - 1;
				}
				if (num10 < 0)
				{
					num10 = 0;
				}
				if (num10 >= pixelHeight)
				{
					num10 = pixelHeight - 1;
				}
				num11 = num9 * pixelWidth;
				num12 = num10 * pixelWidth;
				int num13 = xc + num;
				int num14 = xc - num;
				if (num13 < 0)
				{
					num13 = 0;
				}
				if (num13 >= pixelWidth)
				{
					num13 = pixelWidth - 1;
				}
				if (num14 < 0)
				{
					num14 = 0;
				}
				if (num14 >= pixelWidth)
				{
					num14 = pixelWidth - 1;
				}
				for (int j = num14; j <= num13; j++)
				{
					pixels[j + num11] = color;
					pixels[j + num12] = color;
				}
				num2++;
				num8 += num3;
				num7 += num6;
				num6 += num3;
				if (num5 + (num7 << 1) > 0)
				{
					num--;
					i -= num4;
					num7 += num5;
					num5 += num4;
				}
			}
			num = 0;
			num2 = yr;
			num9 = yc + num2;
			num10 = yc - num2;
			if (num9 < 0)
			{
				num9 = 0;
			}
			if (num9 >= pixelHeight)
			{
				num9 = pixelHeight - 1;
			}
			if (num10 < 0)
			{
				num10 = 0;
			}
			if (num10 >= pixelHeight)
			{
				num10 = pixelHeight - 1;
			}
			num11 = num9 * pixelWidth;
			num12 = num10 * pixelWidth;
			num5 = yr * yr;
			num6 = xr * xr * (1 - (yr << 1));
			num7 = 0;
			i = 0;
			num8 = num3 * yr;
			while (i <= num8)
			{
				int num13 = xc + num;
				int num14 = xc - num;
				if (num13 < 0)
				{
					num13 = 0;
				}
				if (num13 >= pixelWidth)
				{
					num13 = pixelWidth - 1;
				}
				if (num14 < 0)
				{
					num14 = 0;
				}
				if (num14 >= pixelWidth)
				{
					num14 = pixelWidth - 1;
				}
				for (int k = num14; k <= num13; k++)
				{
					pixels[k + num11] = color;
					pixels[k + num12] = color;
				}
				num++;
				i += num4;
				num7 += num5;
				num5 += num4;
				if (num6 + (num7 << 1) > 0)
				{
					num2--;
					num9 = yc + num2;
					num10 = yc - num2;
					if (num9 < 0)
					{
						num9 = 0;
					}
					if (num9 >= pixelHeight)
					{
						num9 = pixelHeight - 1;
					}
					if (num10 < 0)
					{
						num10 = 0;
					}
					if (num10 >= pixelHeight)
					{
						num10 = pixelHeight - 1;
					}
					num11 = num9 * pixelWidth;
					num12 = num10 * pixelWidth;
					num8 -= num3;
					num7 += num6;
					num6 += num3;
				}
			}
		}

		public static void FillPolygon(this WriteableBitmapAdapter bmp, int[] points, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.FillPolygon(points, color2);
		}

		public static void FillPolygon(this WriteableBitmapAdapter bmp, int[] points, int color)
		{
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			int[] pixels = bmp.Pixels;
			int num = points.Length;
			int num2 = points.Length >> 1;
			int[] array = new int[num2];
			int num3 = pixelHeight;
			int num4 = 0;
			for (int i = 1; i < num; i += 2)
			{
				int num5 = points[i];
				if (num5 < num3)
				{
					num3 = num5;
				}
				if (num5 > num4)
				{
					num4 = num5;
				}
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 >= pixelHeight)
			{
				num4 = pixelHeight - 1;
			}
			for (int j = num3; j <= num4; j++)
			{
				float num6 = (float)points[0];
				float num7 = (float)points[1];
				int num8 = 0;
				for (int k = 2; k < num; k += 2)
				{
					float num9 = (float)points[k];
					float num10 = (float)points[k + 1];
					if ((num7 < (float)j && num10 >= (float)j) || (num10 < (float)j && num7 >= (float)j))
					{
						array[num8++] = (int)(num6 + ((float)j - num7) / (num10 - num7) * (num9 - num6));
					}
					num6 = num9;
					num7 = num10;
				}
				for (int l = 1; l < num8; l++)
				{
					int num11 = array[l];
					int num12 = l;
					while (num12 > 0 && array[num12 - 1] > num11)
					{
						array[num12] = array[num12 - 1];
						num12--;
					}
					array[num12] = num11;
				}
				for (int m = 0; m < num8 - 1; m += 2)
				{
					int num13 = array[m];
					int num14 = array[m + 1];
					if (num14 > 0 && num13 < pixelWidth)
					{
						if (num13 < 0)
						{
							num13 = 0;
						}
						if (num14 >= pixelWidth)
						{
							num14 = pixelWidth - 1;
						}
						for (int n = num13; n <= num14; n++)
						{
							pixels[j * pixelWidth + n] = color;
						}
					}
				}
			}
		}

		public static void FillQuad(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.FillQuad(x1, y1, x2, y2, x3, y3, x4, y4, color2);
		}

		public static void FillQuad(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4, int color)
		{
			bmp.FillPolygon(new int[]
			{
				x1,
				y1,
				x2,
				y2,
				x3,
				y3,
				x4,
				y4,
				x1,
				y1
			}, color);
		}

		public static void FillTriangle(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int x3, int y3, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.FillTriangle(x1, y1, x2, y2, x3, y3, color2);
		}

		public static void FillTriangle(this WriteableBitmapAdapter bmp, int x1, int y1, int x2, int y2, int x3, int y3, int color)
		{
			bmp.FillPolygon(new int[]
			{
				x1,
				y1,
				x2,
				y2,
				x3,
				y3,
				x1,
				y1
			}, color);
		}

		private static List<int> ComputeBezierPoints(int x1, int y1, int cx1, int cy1, int cx2, int cy2, int x2, int y2, int color, int[] pixels, int w, int h)
		{
			int num = Math.Min(x1, Math.Min(cx1, Math.Min(cx2, x2)));
			int num2 = Math.Min(y1, Math.Min(cy1, Math.Min(cy2, y2)));
			int num3 = Math.Max(x1, Math.Max(cx1, Math.Max(cx2, x2)));
			int num4 = Math.Max(y1, Math.Max(cy1, Math.Max(cy2, y2)));
			int num5 = num3 - num;
			int num6 = num4 - num2;
			if (num5 > num6)
			{
				num6 = num5;
			}
			List<int> list = new List<int>();
			if (num6 != 0)
			{
				float num7 = 2f / (float)num6;
				for (float num8 = 0f; num8 <= 1f; num8 += num7)
				{
					float num9 = num8 * num8;
					float num10 = 1f - num8;
					float num11 = num10 * num10;
					int item = (int)(num10 * num11 * (float)x1 + 3f * num8 * num11 * (float)cx1 + 3f * num10 * num9 * (float)cx2 + num8 * num9 * (float)x2);
					int item2 = (int)(num10 * num11 * (float)y1 + 3f * num8 * num11 * (float)cy1 + 3f * num10 * num9 * (float)cy2 + num8 * num9 * (float)y2);
					list.Add(item);
					list.Add(item2);
				}
				list.Add(x2);
				list.Add(y2);
			}
			return list;
		}

		public static void FillBeziers(this WriteableBitmapAdapter bmp, int[] points, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.FillBeziers(points, color2);
		}

		public static void FillBeziers(this WriteableBitmapAdapter bmp, int[] points, int color)
		{
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			int[] pixels = bmp.Pixels;
			int x = points[0];
			int y = points[1];
			List<int> list = new List<int>();
			int num = 2;
			while (num + 5 < points.Length)
			{
				int num2 = points[num + 4];
				int num3 = points[num + 5];
				list.AddRange(WriteableBitmapAdapterExtensions.ComputeBezierPoints(x, y, points[num], points[num + 1], points[num + 2], points[num + 3], num2, num3, color, pixels, pixelWidth, pixelHeight));
				x = num2;
				y = num3;
				num += 6;
			}
			bmp.FillPolygon(list.ToArray(), color);
		}

		private static List<int> ComputeSegmentPoints(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4, float tension, int color, int[] pixels, int w, int h)
		{
			int num = Math.Min(x1, Math.Min(x2, Math.Min(x3, x4)));
			int num2 = Math.Min(y1, Math.Min(y2, Math.Min(y3, y4)));
			int num3 = Math.Max(x1, Math.Max(x2, Math.Max(x3, x4)));
			int num4 = Math.Max(y1, Math.Max(y2, Math.Max(y3, y4)));
			int num5 = num3 - num;
			int num6 = num4 - num2;
			if (num5 > num6)
			{
				num6 = num5;
			}
			List<int> list = new List<int>();
			if (num6 != 0)
			{
				float num7 = 2f / (float)num6;
				float num8 = tension * (float)(x3 - x1);
				float num9 = tension * (float)(y3 - y1);
				float num10 = tension * (float)(x4 - x2);
				float num11 = tension * (float)(y4 - y2);
				float num12 = num8 + num10 + (float)(2 * x2) - (float)(2 * x3);
				float num13 = num9 + num11 + (float)(2 * y2) - (float)(2 * y3);
				float num14 = -2f * num8 - num10 - (float)(3 * x2) + (float)(3 * x3);
				float num15 = -2f * num9 - num11 - (float)(3 * y2) + (float)(3 * y3);
				for (float num16 = 0f; num16 <= 1f; num16 += num7)
				{
					float num17 = num16 * num16;
					int item = (int)(num12 * num17 * num16 + num14 * num17 + num8 * num16 + (float)x2);
					int item2 = (int)(num13 * num17 * num16 + num15 * num17 + num9 * num16 + (float)y2);
					list.Add(item);
					list.Add(item2);
				}
				list.Add(x3);
				list.Add(y3);
			}
			return list;
		}

		public static void FillCurve(this WriteableBitmapAdapter bmp, int[] points, float tension, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.FillCurve(points, tension, color2);
		}

		public static void FillCurve(this WriteableBitmapAdapter bmp, int[] points, float tension, int color)
		{
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			int[] pixels = bmp.Pixels;
			List<int> list = WriteableBitmapAdapterExtensions.ComputeSegmentPoints(points[0], points[1], points[0], points[1], points[2], points[3], points[4], points[5], tension, color, pixels, pixelWidth, pixelHeight);
			int i;
			for (i = 2; i < points.Length - 4; i += 2)
			{
				list.AddRange(WriteableBitmapAdapterExtensions.ComputeSegmentPoints(points[i - 2], points[i - 1], points[i], points[i + 1], points[i + 2], points[i + 3], points[i + 4], points[i + 5], tension, color, pixels, pixelWidth, pixelHeight));
			}
			list.AddRange(WriteableBitmapAdapterExtensions.ComputeSegmentPoints(points[i - 2], points[i - 1], points[i], points[i + 1], points[i + 2], points[i + 3], points[i + 2], points[i + 3], tension, color, pixels, pixelWidth, pixelHeight));
			bmp.FillPolygon(list.ToArray(), color);
		}

		public static void FillCurveClosed(this WriteableBitmapAdapter bmp, int[] points, float tension, Color color)
		{
			int num = (int)(color.A + 1);
			int color2 = (int)color.A << 24 | (int)((byte)((int)color.R * num >> 8)) << 16 | (int)((byte)((int)color.G * num >> 8)) << 8 | (int)((byte)((int)color.B * num >> 8));
			bmp.FillCurveClosed(points, tension, color2);
		}

		public static void FillCurveClosed(this WriteableBitmapAdapter bmp, int[] points, float tension, int color)
		{
			int pixelWidth = bmp.PixelWidth;
			int pixelHeight = bmp.PixelHeight;
			int[] pixels = bmp.Pixels;
			int num = points.Length;
			List<int> list = WriteableBitmapAdapterExtensions.ComputeSegmentPoints(points[num - 2], points[num - 1], points[0], points[1], points[2], points[3], points[4], points[5], tension, color, pixels, pixelWidth, pixelHeight);
			int i;
			for (i = 2; i < num - 4; i += 2)
			{
				list.AddRange(WriteableBitmapAdapterExtensions.ComputeSegmentPoints(points[i - 2], points[i - 1], points[i], points[i + 1], points[i + 2], points[i + 3], points[i + 4], points[i + 5], tension, color, pixels, pixelWidth, pixelHeight));
			}
			list.AddRange(WriteableBitmapAdapterExtensions.ComputeSegmentPoints(points[i - 2], points[i - 1], points[i], points[i + 1], points[i + 2], points[i + 3], points[0], points[1], tension, color, pixels, pixelWidth, pixelHeight));
			list.AddRange(WriteableBitmapAdapterExtensions.ComputeSegmentPoints(points[i], points[i + 1], points[i + 2], points[i + 3], points[0], points[1], points[2], points[3], tension, color, pixels, pixelWidth, pixelHeight));
			bmp.FillPolygon(list.ToArray(), color);
		}
	}
}
