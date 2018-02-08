using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class SectorChartShapeParams
	{
		public IDataPoint DataPoint;

		private double _startAngle;

		private double _stopAngle;

		public bool LabelLineTargetToRight
		{
			get;
			set;
		}

		public double OuterRadius
		{
			get;
			set;
		}

		public double InnerRadius
		{
			get;
			set;
		}

		public double StartAngle
		{
			get
			{
				return this._startAngle;
			}
			set
			{
				this._startAngle = value;
			}
		}

		public double StopAngle
		{
			get
			{
				return this._stopAngle;
			}
			set
			{
				this._stopAngle = value;
			}
		}

		public Point Center
		{
			get;
			set;
		}

		public double OffsetX
		{
			get;
			set;
		}

		public double OffsetY
		{
			get;
			set;
		}

		public bool IsLargerArc
		{
			get;
			set;
		}

		public bool Lighting
		{
			get;
			set;
		}

		public bool Bevel
		{
			get;
			set;
		}

		public Brush Background
		{
			get;
			set;
		}

		public bool IsZero
		{
			get;
			set;
		}

		public double YAxisScaling
		{
			get
			{
				return Math.Sin(this.TiltAngle);
			}
		}

		public double ZAxisScaling
		{
			get
			{
				return Math.Cos(1.5707963267948966 - this.TiltAngle);
			}
		}

		public double Depth
		{
			get;
			set;
		}

		public double ExplodeRatio
		{
			get;
			set;
		}

		public double Width
		{
			get;
			set;
		}

		public double Height
		{
			get;
			set;
		}

		public double TiltAngle
		{
			get;
			set;
		}

		public Point LabelPoint
		{
			get;
			set;
		}

		public Brush LabelLineColor
		{
			get;
			set;
		}

		public double LabelLineThickness
		{
			get;
			set;
		}

		public DoubleCollection LabelLineStyle
		{
			get;
			set;
		}

		public bool LabelLineEnabled
		{
			get;
			set;
		}

		public double MeanAngle
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

		public IElement TagReference
		{
			get;
			set;
		}

		public PieDoughnutPoints ExplodedPoints
		{
			get;
			set;
		}

		public PieDoughnutPoints UnExplodedPoints
		{
			get;
			set;
		}

		public double FixAngle(double angle)
		{
			while (angle > 6.2831853071795862)
			{
				angle -= 3.1415926535897931;
			}
			while (angle < 0.0)
			{
				angle += 3.1415926535897931;
			}
			return angle;
		}
	}
}
