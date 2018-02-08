using System;
using System.Windows.Media;

namespace Visifire.Charts
{
	public class RasterLineStyle : RasterShapeStyle
	{
		public PenLineJoin? PenLineJoinA;

		public PenLineJoin? PenLineJoinB;

		public RasterLineStyle()
		{
		}

		public RasterLineStyle(RasterShapeStyle style) : base(style)
		{
		}
	}
}
