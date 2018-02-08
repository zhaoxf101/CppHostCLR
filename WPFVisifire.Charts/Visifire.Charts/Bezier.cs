using System;
using System.Windows;
using System.Windows.Media;

namespace Visifire.Charts
{
	public static class Bezier
	{
		public static PointCollection GetBezierPoints(PointCollection knots, double tension, out PointCollection firstControlPoints, out PointCollection secondControlPoints)
		{
			PointCollection pointCollection = new PointCollection();
			firstControlPoints = new PointCollection();
			secondControlPoints = new PointCollection();
			for (int i = 1; i < knots.Count; i++)
			{
				firstControlPoints.Add(Bezier.GetFirstControlPoints(knots, i - 1, tension));
				secondControlPoints.Add(Bezier.GetSecondControlPoints(knots, i - 1, tension));
				pointCollection.Add(Bezier.GetFirstControlPoints(knots, i - 1, tension));
				pointCollection.Add(Bezier.GetSecondControlPoints(knots, i - 1, tension));
				pointCollection.Add(knots[i]);
			}
			return pointCollection;
		}

		public static Point GetDerivative(PointCollection knots, int i, double t)
		{
			if (knots.Count < 2)
			{
				throw new ArgumentOutOfRangeException("Atleast two points required");
			}
			if (i == 0)
			{
				return new Point((knots[1].X - knots[0].X) / t, (knots[1].Y - knots[0].Y) / t);
			}
			if (i == knots.Count - 1)
			{
				return new Point((knots[i].X - knots[i - 1].X) / t, (knots[i].Y - knots[i - 1].Y) / t);
			}
			return new Point((knots[i + 1].X - knots[i - 1].X) / t, (knots[i + 1].Y - knots[i - 1].Y) / t);
		}

		public static Point GetFirstControlPoints(PointCollection knots, int i, double t)
		{
			Point derivative = Bezier.GetDerivative(knots, i, t);
			return new Point(knots[i].X + derivative.X / 3.0, knots[i].Y + derivative.Y / 3.0);
		}

		public static Point GetSecondControlPoints(PointCollection knots, int i, double t)
		{
			Point derivative = Bezier.GetDerivative(knots, i + 1, t);
			return new Point(knots[i + 1].X - derivative.X / 3.0, knots[i + 1].Y - derivative.Y / 3.0);
		}

		public static void GetCurveControlPoints(double tension, Point[] knots, out Point[] firstControlPoints, out Point[] secondControlPoints)
		{
			if (knots == null)
			{
				throw new ArgumentNullException("knots");
			}
			int num = knots.Length - 1;
			if (num < 1)
			{
				firstControlPoints = null;
				secondControlPoints = null;
				return;
			}
			if (num == 1)
			{
				firstControlPoints = new Point[1];
				firstControlPoints[0].X = (2.0 * knots[0].X + knots[1].X) / 3.0;
				firstControlPoints[0].Y = (2.0 * knots[0].Y + knots[1].Y) / 3.0;
				secondControlPoints = new Point[1];
				secondControlPoints[0].X = 2.0 * firstControlPoints[0].X - knots[0].X;
				secondControlPoints[0].Y = 2.0 * firstControlPoints[0].Y - knots[0].Y;
				return;
			}
			double[] array = new double[num];
			for (int i = 1; i < num - 1; i++)
			{
				array[i] = 4.0 * knots[i].X + 2.0 * knots[i + 1].X;
			}
			array[0] = knots[0].X + 2.0 * knots[1].X;
			array[num - 1] = (8.0 * knots[num - 1].X + knots[num].X) / 2.0;
			double[] firstControlPoints2 = Bezier.GetFirstControlPoints(array, tension);
			for (int j = 1; j < num - 1; j++)
			{
				array[j] = 4.0 * knots[j].Y + 2.0 * knots[j + 1].Y;
			}
			array[0] = knots[0].Y + 2.0 * knots[1].Y;
			array[num - 1] = (8.0 * knots[num - 1].Y + knots[num].Y) / 2.0;
			double[] firstControlPoints3 = Bezier.GetFirstControlPoints(array, tension);
			firstControlPoints = new Point[num];
			secondControlPoints = new Point[num];
			for (int k = 0; k < num; k++)
			{
				firstControlPoints[k] = new Point(firstControlPoints2[k], firstControlPoints3[k]);
				if (k < num - 1)
				{
					secondControlPoints[k] = new Point(2.0 * knots[k + 1].X - firstControlPoints2[k + 1], 2.0 * knots[k + 1].Y - firstControlPoints3[k + 1]);
				}
				else
				{
					secondControlPoints[k] = new Point((knots[num].X + firstControlPoints2[num - 1]) / 2.0, (knots[num].Y + firstControlPoints3[num - 1]) / 2.0);
				}
			}
		}

		private static double[] GetFirstControlPoints(double[] rhs, double tension)
		{
			int num = rhs.Length;
			double[] array = new double[num];
			double[] array2 = new double[num];
			double num2 = tension;
			array[0] = rhs[0] / num2;
			for (int i = 1; i < num; i++)
			{
				array2[i] = 1.0 / num2;
				num2 = ((i < num - 1) ? 4.0 : 3.5) - array2[i];
				array[i] = (rhs[i] - array[i - 1]) / num2;
			}
			for (int j = 1; j < num; j++)
			{
				array[num - j - 1] -= array2[num - j] * array[num - j];
			}
			return array;
		}
	}
}
