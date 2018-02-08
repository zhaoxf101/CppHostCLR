using System;
using System.Collections.Generic;
using System.Windows;

namespace Visifire.Charts
{
	internal class TriangularChartSliceParms
	{
		public int Index;

		public IDataPoint DataPoint;

		public double Height;

		public double TopRadius;

		public double BottomRadius;

		public double TopAngle;

		public double BottomAngle;

		public double Top;

		public double TopGap;

		public double BottomGap;

		public Point RightMidPoint;

		public Point LabelLineEndPoint;

		public Point OrginalLabelLineEndPoint;

		public FillType FillType;

		public List<Point> ExplodedPoints
		{
			get;
			set;
		}
	}
}
