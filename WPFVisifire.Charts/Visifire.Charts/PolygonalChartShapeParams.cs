using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Visifire.Charts
{
	internal class PolygonalChartShapeParams
	{
		public PointCollection Points
		{
			get;
			set;
		}

		public Brush Background
		{
			get;
			set;
		}

		public Brush BorderColor
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

		public double Depth3D
		{
			get;
			set;
		}

		public Storyboard Storyboard
		{
			get;
			set;
		}

		public bool AnimationEnabled
		{
			get;
			set;
		}

		public Size Size
		{
			get;
			set;
		}

		public FrameworkElement TagReference
		{
			get;
			set;
		}
	}
}
