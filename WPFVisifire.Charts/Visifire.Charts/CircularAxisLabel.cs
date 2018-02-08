using System;
using System.Windows;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class CircularAxisLabel
	{
		internal Point Center
		{
			get;
			set;
		}

		internal double XRadius
		{
			get;
			set;
		}

		internal double YRadius
		{
			get;
			set;
		}

		internal AxisLabel AxisLabel
		{
			get;
			set;
		}

		internal double Angle
		{
			get;
			set;
		}

		internal double Index
		{
			get;
			set;
		}

		internal Point Position
		{
			get;
			set;
		}

		internal Point NextPosition
		{
			get;
			set;
		}

		internal CircularAxisLabel(Point position, Point center, double xRadius, double yRadius, AxisLabel label, double index)
		{
			this.Center = center;
			this.XRadius = xRadius;
			this.YRadius = yRadius;
			this.Position = position;
			this.AxisLabel = label;
			this.Index = index;
			this.Angle = CircularLabel.ResetMeanAngle(this.CalculateAngleByCoordinate(this.Position));
		}

		public double CalculateAngleByCoordinate(Point position)
		{
			return Math.Atan2(position.Y - this.Center.Y, position.X - this.Center.X);
		}
	}
}
