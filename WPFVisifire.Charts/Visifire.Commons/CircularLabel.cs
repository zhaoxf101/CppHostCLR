using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Visifire.Commons
{
	public class CircularLabel
	{
		public double BaseMeanAngle;

		public double CurrentMeanAngle;

		public Point Center;

		public double XRadiusLabel;

		public double YRadiusLabel;

		public double XRadiusChart;

		public double YRadiusChart;

		private Canvas Visual;

		public FrameworkElement LabelVisual;

		public CircularLabel PreviusLabel;

		public CircularLabel NextLabel;

		public bool IsLast;

		public bool IsFirst;

		public bool IsSkiped;

		public Rect Boundary;

		public double MaxXPosition;

		public double MaxYPosition;

		public double MinXPosition;

		public double MinYPosition;

		public Point Position
		{
			get;
			set;
		}

		public double Height
		{
			get
			{
				return this.LabelVisual.Height;
			}
		}

		public double Width
		{
			get
			{
				return this.LabelVisual.Width;
			}
		}

		public CircularLabel(FrameworkElement labelVisual, Point center, double meanAngle, double xRadiusLabel, double yRadiusLabel, double xRadiusChart, double yRadiusChart, Canvas canvas)
		{
			this.LabelVisual = labelVisual;
			this.BaseMeanAngle = CircularLabel.ResetMeanAngle(meanAngle);
			this.CurrentMeanAngle = this.BaseMeanAngle;
			this.XRadiusLabel = xRadiusLabel;
			this.YRadiusLabel = yRadiusLabel;
			this.XRadiusChart = xRadiusChart;
			this.YRadiusChart = yRadiusChart;
			this.Visual = canvas;
			this.Center = center;
			this.Position = this.GetCartesianCoordinates4Labels(meanAngle);
			this.UpdateLabelVisualPosition();
		}

		public OverlapTypes CheckOverLap(SweepDirection direction, out double verticalOffset, out double horizontalOffset)
		{
			verticalOffset = 0.0;
			horizontalOffset = 0.0;
			if (direction == SweepDirection.Clockwise)
			{
				if (this.NextLabel != null)
				{
					return this.CheckOverlap(this.NextLabel, out verticalOffset, out horizontalOffset);
				}
				return OverlapTypes.None;
			}
			else
			{
				if (this.PreviusLabel != null)
				{
					return this.CheckOverlap(this.PreviusLabel, out verticalOffset, out horizontalOffset);
				}
				return OverlapTypes.None;
			}
		}

		public void PlaceLabel(bool skip)
		{
			int noOfIteration = 2;
			double num;
			double num2;
			OverlapTypes overlapTypes = this.CheckOverLap(SweepDirection.Clockwise, out num, out num2);
			bool flag3;
			if (overlapTypes == OverlapTypes.Both)
			{
				bool flag = CircularLabel.AskSpaceToPreviusLabel(this, num / 2.0, noOfIteration);
				bool flag2 = CircularLabel.AskSpaceToNextLabel(this, num / 2.0, noOfIteration);
				if (flag && flag2)
				{
					flag3 = true;
				}
				else if (flag && !flag2)
				{
					flag = CircularLabel.AskSpaceToPreviusLabel(this, num / 2.0, noOfIteration);
					flag3 = flag;
				}
				else if (!flag && flag2)
				{
					flag2 = CircularLabel.AskSpaceToNextLabel(this, num / 2.0, noOfIteration);
					flag3 = flag;
				}
				else
				{
					flag3 = false;
				}
			}
			else
			{
				OverlapTypes overlapTypes2 = this.CheckOverLap(SweepDirection.Counterclockwise, out num, out num2);
				if (overlapTypes2 == OverlapTypes.Both)
				{
					bool flag2 = CircularLabel.AskSpaceToNextLabel(this, num / 2.0, noOfIteration);
					bool flag = CircularLabel.AskSpaceToPreviusLabel(this, num / 2.0, noOfIteration);
					if (flag && flag2)
					{
						flag3 = true;
					}
					else if (flag && !flag2)
					{
						flag = CircularLabel.AskSpaceToNextLabel(this, num / 2.0, noOfIteration);
						flag3 = flag;
					}
					else if (!flag && flag2)
					{
						flag2 = CircularLabel.AskSpaceToPreviusLabel(this, num / 2.0, noOfIteration);
						flag3 = flag2;
					}
					else
					{
						flag3 = false;
					}
				}
				else
				{
					flag3 = true;
				}
			}
			if (flag3)
			{
				return;
			}
			if (skip)
			{
				this.SkipLabel();
			}
		}

		private static bool AskSpaceToNextLabel(CircularLabel label, double requiredVerticalOffset, int noOfIteration)
		{
			if (label.IsLast)
			{
				return label.Position.Y + label.Height < label.Boundary.Height && label.Boundary.Height - (label.Position.Y + label.Height) > requiredVerticalOffset && label.RotateByVerticalOffset(requiredVerticalOffset, SweepDirection.Clockwise);
			}
			double num = CircularLabel.VerticalSpcaeBetweenLabels(label, label.NextLabel);
			if (num > requiredVerticalOffset)
			{
				return label.RotateByVerticalOffset(requiredVerticalOffset, SweepDirection.Clockwise);
			}
			bool flag = CircularLabel.AskSpaceToNextLabel(label.NextLabel, requiredVerticalOffset, noOfIteration);
			return flag && label.RotateByVerticalOffset(requiredVerticalOffset, SweepDirection.Clockwise);
		}

		private static bool AskSpaceToPreviusLabel(CircularLabel labelA, double requiredVerticalOffset, int noOfIteration)
		{
			if (labelA.IsFirst)
			{
				return labelA.Position.Y > requiredVerticalOffset && labelA.RotateByVerticalOffset(requiredVerticalOffset, SweepDirection.Counterclockwise);
			}
			double num = CircularLabel.VerticalSpcaeBetweenLabels(labelA, labelA.PreviusLabel);
			if (num > requiredVerticalOffset)
			{
				return labelA.RotateByVerticalOffset(requiredVerticalOffset, SweepDirection.Counterclockwise);
			}
			bool flag = CircularLabel.AskSpaceToPreviusLabel(labelA.PreviusLabel, requiredVerticalOffset, noOfIteration);
			return flag && labelA.RotateByVerticalOffset(requiredVerticalOffset, SweepDirection.Counterclockwise);
		}

		private static double VerticalSpcaeBetweenLabels(CircularLabel labelA, CircularLabel labelB)
		{
			double num;
			if (labelA.Position.Y < labelB.Position.Y)
			{
				num = labelB.Position.Y - (labelA.Position.Y + labelA.Height);
			}
			else
			{
				num = labelA.Position.Y - (labelB.Position.Y + labelB.Height);
			}
			if (num < 0.0)
			{
				return 0.0;
			}
			return num;
		}

		public void SkipLabel()
		{
			this.LabelVisual.Visibility = Visibility.Collapsed;
			this.IsSkiped = true;
			this.PreviusLabel.NextLabel = this.NextLabel;
			this.NextLabel.PreviusLabel = this.PreviusLabel;
			if (this.IsFirst)
			{
				this.NextLabel.IsFirst = true;
			}
			if (this.IsLast)
			{
				this.PreviusLabel.IsLast = true;
			}
		}

		public double CalculateAngleByYCoordinate(double y)
		{
			return Math.Asin((y - this.Center.Y) / this.YRadiusLabel);
		}

		public double CalculateAngleByXCoordinate(double x)
		{
			return Math.Acos((x - this.Center.X) / this.XRadiusLabel);
		}

		public bool RotateByVerticalOffset(double verticalOffset, SweepDirection direction)
		{
			double num = Math.Atan(verticalOffset / this.YRadiusLabel);
			if (direction == SweepDirection.Counterclockwise)
			{
				num *= -1.0;
			}
			double num2 = this.CurrentMeanAngle + num;
			num2 = CircularLabel.ResetMeanAngle(num2);
			Point cartesianCoordinates4Labels = this.GetCartesianCoordinates4Labels(num2);
			if (Math.Abs(this.BaseMeanAngle - CircularLabel.ResetMeanAngle(this.CurrentMeanAngle + num)) > 0.62831853071795862)
			{
				return false;
			}
			this.CurrentMeanAngle = num2;
			this.Position = cartesianCoordinates4Labels;
			return true;
		}

		public Point GetCartesianCoordinates4Labels(double meanAngle)
		{
			meanAngle = CircularLabel.ResetMeanAngle(meanAngle);
			double x = this.Center.X + this.XRadiusLabel * Math.Cos(meanAngle);
			double y = this.Center.Y + this.YRadiusLabel * Math.Sin(meanAngle);
			return new Point(x, y);
		}

		public Point GetCartesianCoordinates4Chart(double meanAngle)
		{
			meanAngle = CircularLabel.ResetMeanAngle(meanAngle);
			double x = this.Center.X + this.XRadiusChart * Math.Cos(meanAngle);
			double y = this.Center.Y + this.YRadiusChart * Math.Sin(meanAngle);
			return new Point(x, y);
		}

		private OverlapTypes CheckOverlap(CircularLabel label, out double verticalOffset, out double horizontalOffset)
		{
			OverlapTypes overlapTypes = OverlapTypes.None;
			verticalOffset = 0.0;
			horizontalOffset = 0.0;
			if (label.Position.Y > this.Position.Y)
			{
				if (label.Position.Y < this.Position.Y + this.Height)
				{
					verticalOffset = this.Position.Y + this.Height - label.Position.Y;
					overlapTypes = OverlapTypes.Vertical;
				}
			}
			else if (this.Position.Y > label.Position.Y && this.Position.Y < label.Position.Y + label.Height)
			{
				verticalOffset = label.Position.Y + label.Height - this.Position.Y;
				overlapTypes = OverlapTypes.Vertical;
			}
			if (label.Position.X > this.Position.X)
			{
				if (label.Position.X < this.Position.X + this.Width)
				{
					horizontalOffset = this.Position.X + this.Width - label.Position.X;
					overlapTypes = ((overlapTypes == OverlapTypes.Vertical) ? OverlapTypes.Both : OverlapTypes.Horizontal);
				}
			}
			else if (this.Position.X > label.Position.X && this.Position.X < label.Position.X + label.Width)
			{
				horizontalOffset = label.Position.X + label.Width - this.Position.X;
				overlapTypes = ((overlapTypes == OverlapTypes.Vertical) ? OverlapTypes.Both : OverlapTypes.Horizontal);
			}
			return overlapTypes;
		}

		public static double ResetMeanAngle(double meanAngle)
		{
			if (meanAngle > 6.2831853071795862)
			{
				meanAngle -= 6.2831853071795862;
			}
			if (meanAngle < 0.0)
			{
				meanAngle = 6.2831853071795862 + meanAngle;
			}
			return meanAngle;
		}

		public bool CheckOutOfBounds()
		{
			double num = (double)this.LabelVisual.GetValue(Canvas.LeftProperty);
			double num2 = (double)this.LabelVisual.GetValue(Canvas.TopProperty);
			double x;
			if (num < this.Center.X)
			{
				x = num + this.Width + LabelPlacementHelper.LABEL_LINE_GAP;
			}
			else
			{
				x = num - LabelPlacementHelper.LABEL_LINE_GAP;
			}
			double y = num2 + this.Height / 2.0;
			Point cartesianCoordinates4Chart = this.GetCartesianCoordinates4Chart(this.BaseMeanAngle);
			Point p = new Point(x, y);
			Point point = Graphics.IntersectingPointOfTwoLines(this.Center, p, cartesianCoordinates4Chart, new Point(p.X, this.Center.Y));
			double num3 = Graphics.DistanceBetweenTwoPoints(this.Center, point);
			double num4 = Math.Atan(num3 / this.XRadiusChart);
			num4 = 1.5707963267948966 - num4;
			return num4 > 1.5707963267948966;
		}

		public void UpdateLabelVisualPosition()
		{
			double num = this.Position.X;
			double y = this.Position.Y;
			if (num < this.Center.X)
			{
				if (num - this.LabelVisual.Width < 0.0)
				{
					num -= this.LabelVisual.Width;
				}
				else
				{
					num -= this.LabelVisual.Width;
				}
			}
			this.LabelVisual.SetValue(Canvas.LeftProperty, num);
			this.LabelVisual.SetValue(Canvas.TopProperty, y);
		}

		internal void ColorIt(Color color)
		{
			(this.LabelVisual as Canvas).Background = new SolidColorBrush(color);
		}
	}
}
