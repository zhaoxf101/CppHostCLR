using System;
using System.Collections.Generic;
using System.Windows;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class CircularPlotDetails
	{
		public double Radius
		{
			get;
			set;
		}

		public Point Center
		{
			get;
			set;
		}

		public List<Point> ListOfPoints4CircularAxis
		{
			get;
			set;
		}

		public List<double> AnglesInRadian
		{
			get;
			set;
		}

		public List<double> AnglesInDegree
		{
			get;
			set;
		}

		public double MinAngleInDegree
		{
			get;
			set;
		}

		public ChartOrientationType ChartOrientation
		{
			get;
			set;
		}

		public RenderAs CircularChartType
		{
			get;
			set;
		}

		public CircularPlotDetails(ChartOrientationType chartOrientation, RenderAs renderAs)
		{
			this.ChartOrientation = chartOrientation;
			this.CircularChartType = renderAs;
			this.ListOfPoints4CircularAxis = new List<Point>();
		}

		internal void UpdateCircularPlotDetails(List<CircularAxisLabel> circularLabels, double radius)
		{
			this.ListOfPoints4CircularAxis.Clear();
			foreach (CircularAxisLabel current in circularLabels)
			{
				this.ListOfPoints4CircularAxis.Add(current.Position);
			}
			this.Radius = radius;
		}

		internal void CalculateAxisXLabelsPoints4Polar(double width, double height, bool isAxisLabelsEnabled, List<double> angles, double minValue, double maxValue)
		{
			double num = Math.Min(width, height) / 2.0;
			double num2 = 10.0;
			if (isAxisLabelsEnabled)
			{
				num2 = 20.0;
			}
			this.Radius = num - num * num2 / 100.0;
			this.Center = new Point(width / 2.0, height / 2.0);
			this.AnglesInRadian = new List<double>();
			double num3;
			if (minValue != 0.0)
			{
				num3 = AxisLabel.GetRadians(minValue - 90.0);
			}
			else
			{
				num3 = 4.71238898038469;
			}
			double num4 = Graphics.ValueToPixelPosition(num3, 6.2831853071795862 + num3, AxisLabel.GetRadians(minValue), AxisLabel.GetRadians(maxValue), AxisLabel.GetRadians(minValue));
			this.MinAngleInDegree = num4 * 180.0 / 3.1415926535897931;
			for (int i = 0; i < angles.Count; i++)
			{
				double num5 = Graphics.ValueToPixelPosition(num3, 6.2831853071795862 + num3, AxisLabel.GetRadians(minValue), AxisLabel.GetRadians(maxValue), AxisLabel.GetRadians(angles[i]));
				double x = this.Radius * Math.Cos(num5) + this.Center.X;
				double y = this.Radius * Math.Sin(num5) + this.Center.Y;
				this.ListOfPoints4CircularAxis.Add(new Point(x, y));
				this.AnglesInRadian.Add(num5);
			}
		}

		internal void CalculateAxisXLabelsPoints4Radar(double width, double height, bool isAxisLabelsEnabled, int maxDataPointsCount)
		{
			double num = -1.5707963267948966;
			double num2 = num;
			int num3 = 1;
			double num4 = Math.Min(width, height) / 2.0;
			double num5 = 10.0;
			if (isAxisLabelsEnabled)
			{
				num5 = 20.0;
			}
			this.Radius = num4 - num4 * num5 / 100.0;
			this.Center = new Point(width / 2.0, height / 2.0);
			this.AnglesInRadian = new List<double>();
			this.MinAngleInDegree = 360.0 / (double)maxDataPointsCount;
			for (int i = 0; i < maxDataPointsCount; i++)
			{
				double x = this.Radius * Math.Cos(num2) + this.Center.X;
				double y = this.Radius * Math.Sin(num2) + this.Center.Y;
				this.ListOfPoints4CircularAxis.Add(new Point(x, y));
				this.AnglesInRadian.Add(num2);
				double angle = this.MinAngleInDegree * (double)num3++;
				num2 = AxisLabel.GetRadians(angle) - 1.5707963267948966;
			}
		}
	}
}
