using System;
using System.Windows;
using System.Windows.Media;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class RectangularChartShapeParams
	{
		public Size Size
		{
			get;
			set;
		}

		public CornerRadius XRadius
		{
			get;
			set;
		}

		public CornerRadius YRadius
		{
			get;
			set;
		}

		public Brush BackgroundBrush
		{
			get;
			set;
		}

		public Brush BorderBrush
		{
			get;
			set;
		}

		public bool Bevel
		{
			get;
			set;
		}

		public bool Lighting
		{
			get;
			set;
		}

		public bool Shadow
		{
			get;
			set;
		}

		public double ShadowOffset
		{
			get;
			set;
		}

		public DoubleCollection BorderStyle
		{
			get;
			set;
		}

		public double BorderThickness
		{
			get;
			set;
		}

		public bool IsPositive
		{
			get;
			set;
		}

		public double Depth
		{
			get;
			set;
		}

		public bool IsTopOfStack
		{
			get;
			set;
		}

		public bool IsStacked
		{
			get;
			set;
		}

		public bool IsMarkerEnabled
		{
			get;
			set;
		}

		public Brush MarkerColor
		{
			get;
			set;
		}

		public Brush MarkerBorderColor
		{
			get;
			set;
		}

		public double MarkerSize
		{
			get;
			set;
		}

		public Thickness MarkerBorderThickness
		{
			get;
			set;
		}

		public double MarkerScale
		{
			get;
			set;
		}

		public MarkerTypes MarkerType
		{
			get;
			set;
		}

		public bool IsLabelEnabled
		{
			get;
			set;
		}

		public Brush LabelBackground
		{
			get;
			set;
		}

		public Brush LabelFontColor
		{
			get;
			set;
		}

		public FontFamily LabelFontFamily
		{
			get;
			set;
		}

		public double LabelFontSize
		{
			get;
			set;
		}

		public FontStyle LabelFontStyle
		{
			get;
			set;
		}

		public FontWeight LabelFontWeight
		{
			get;
			set;
		}

		public LabelStyles? LabelStyle
		{
			get;
			set;
		}

		public string LabelText
		{
			get;
			set;
		}

		public IElement TagReference
		{
			get;
			set;
		}
	}
}
