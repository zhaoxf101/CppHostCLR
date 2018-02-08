using System;
using System.Windows;

namespace Visifire.Charts
{
	internal struct LineStructure
	{
		public Point PointA;

		public Point PointB;

		public LineStructure(Point a, Point b)
		{
			this.PointA = a;
			this.PointB = b;
		}
	}
}
