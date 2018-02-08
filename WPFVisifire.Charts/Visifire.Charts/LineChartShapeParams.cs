using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Visifire.Charts
{
	internal class LineChartShapeParams
	{
		internal List<IDataPoint> Points
		{
			get;
			set;
		}

		internal List<IDataPoint> ShadowPoints
		{
			get;
			set;
		}

		internal GeometryGroup LineGeometryGroup
		{
			get;
			set;
		}

		internal GeometryGroup LineShadowGeometryGroup
		{
			get;
			set;
		}

		internal Brush LineColor
		{
			get;
			set;
		}

		internal Brush LineFill
		{
			get;
			set;
		}

		internal double LineThickness
		{
			get;
			set;
		}

		internal bool Lighting
		{
			get;
			set;
		}

		internal DoubleCollection LineStyle
		{
			get;
			set;
		}

		internal bool ShadowEnabled
		{
			get;
			set;
		}

		internal double Opacity
		{
			get;
			set;
		}
	}
}
