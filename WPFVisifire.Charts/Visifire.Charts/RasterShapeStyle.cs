using System;
using System.Windows.Media;

namespace Visifire.Charts
{
	public abstract class RasterShapeStyle
	{
		public bool Enabled;

		public double StrokeThickness;

		public LineStyles StrokeStyle;

		public Color Stroke;

		public Color Fill;

		public double Width;

		public double Height;

		public double Opacity = 1.0;

		public bool IsAntiAliased;

		public RasterShapeStyle()
		{
		}

		public RasterShapeStyle(RasterShapeStyle style)
		{
			this.Enabled = style.Enabled;
			this.StrokeThickness = style.StrokeThickness;
			this.StrokeStyle = style.StrokeStyle;
			this.Stroke = style.Stroke;
			this.Fill = style.Fill;
			this.Width = style.Width;
			this.Height = style.Height;
			this.Opacity = style.Opacity;
			this.IsAntiAliased = style.IsAntiAliased;
		}
	}
}
