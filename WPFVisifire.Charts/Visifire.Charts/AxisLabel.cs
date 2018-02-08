using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class AxisLabel
	{
		private FontStyle _oldFontStyle;

		private FontWeight _oldFontWeight;

		private TextAlignment _oldTextAlignment;

		private double _angle;

		private Point _position;

		private string _text;

		private double _from;

		private double _to;

		private double _actualLeft;

		private double _actualTop;

		private double _actualWidth;

		private double _actualHeight;

		private double _actualTextHeight;

		private double _actualTextWidth;

		internal double _padding4AxisXLabelInCircularChart;

		internal double _padding4AxisYLabelInCircularChart;

		public double MaxWidth
		{
			get;
			set;
		}

		internal bool IsVisible
		{
			get;
			set;
		}

		internal object ActualLabelPosition
		{
			get;
			set;
		}

		internal double Angle
		{
			get
			{
				return this._angle;
			}
			set
			{
				if (value >= -90.0 && value <= 90.0)
				{
					this._angle = value;
					return;
				}
				if (double.IsNaN(value))
				{
					this._angle = 0.0;
				}
			}
		}

		internal Point Position
		{
			get
			{
				return this._position;
			}
			set
			{
				this._position = value;
			}
		}

		internal string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				this._text = value;
			}
		}

		internal double From
		{
			get
			{
				return this._from;
			}
			set
			{
				this._from = value;
			}
		}

		internal double To
		{
			get
			{
				return this._to;
			}
			set
			{
				this._to = value;
			}
		}

		internal PlacementTypes Placement
		{
			get;
			set;
		}

		internal ChartOrientationType AxisOrientation4CircularChart
		{
			get;
			set;
		}

		internal TextBlock Visual
		{
			get;
			set;
		}

		internal double ActualTop
		{
			get
			{
				return this._actualTop;
			}
			private set
			{
				this._actualTop = value;
			}
		}

		internal double ActualLeft
		{
			get
			{
				return this._actualLeft;
			}
			private set
			{
				this._actualLeft = value;
			}
		}

		internal double ActualHeight
		{
			get
			{
				return this._actualHeight;
			}
			set
			{
				this._actualHeight = value;
			}
		}

		internal double ActualWidth
		{
			get
			{
				return this._actualWidth;
			}
			set
			{
				this._actualWidth = value;
			}
		}

		internal double ActualTextWidth
		{
			get
			{
				return this._actualTextWidth;
			}
			private set
			{
				this._actualTextWidth = value;
			}
		}

		internal double ActualTextHeight
		{
			get
			{
				return this._actualTextHeight;
			}
			private set
			{
				this._actualTextHeight = value;
			}
		}

		internal TextAlignment TextAlignment
		{
			get;
			set;
		}

		internal double FontSize
		{
			get;
			set;
		}

		internal FontFamily FontFamily
		{
			get;
			set;
		}

		internal Brush FontColor
		{
			get;
			set;
		}

		internal FontStyle FontStyle
		{
			get;
			set;
		}

		internal FontWeight FontWeight
		{
			get;
			set;
		}

		internal TextBlock TextElement
		{
			get;
			set;
		}

		private RotateTransform Rotation
		{
			get;
			set;
		}

		public AxisLabel()
		{
			this.MaxWidth = double.NaN;
		}

		private void SetPosition4LeftCircularLabel(CircularAxisLabel label)
		{
			Point point = new Point(label.Position.X, label.Position.Y);
			if (Math.Round(label.Angle, 1) == Math.Round(1.5707963267948966, 1))
			{
				this.SetPositionBottom(point);
				return;
			}
			if (Math.Round(label.Angle, 1) == Math.Round(4.71238898038469, 1))
			{
				this.SetPositionTop(point);
				return;
			}
			this.SetPositionLeft(point);
		}

		private void SetPosition4RightCircularLabel(CircularAxisLabel label)
		{
			Point point = new Point(label.Position.X, label.Position.Y);
			if (Math.Round(label.Angle, 1) == Math.Round(1.5707963267948966, 1))
			{
				this.SetPositionBottom(point);
				return;
			}
			if (Math.Round(label.Angle, 1) == Math.Round(4.71238898038469, 1))
			{
				this.SetPositionTop(point);
				return;
			}
			this.SetPositionRight(point);
		}

		internal void RearrangeLabelsAtLeft(CircularAxisLabel label, Size baseArea, out Point newPos)
		{
			double num = (double)this.Visual.GetValue(Canvas.LeftProperty);
			double num2 = (double)this.Visual.GetValue(Canvas.TopProperty);
			if (label.Angle == 1.5707963267948966 && num2 + this.ActualHeight > baseArea.Height)
			{
				newPos = new Point(label.Position.X, label.Position.Y - (num2 + this.ActualHeight - baseArea.Height));
				return;
			}
			if (label.Angle == 4.71238898038469 && num2 < 0.0)
			{
				newPos = new Point(label.Position.X, label.Position.Y + Math.Abs(num2));
				return;
			}
			if (num < 0.0)
			{
				newPos = new Point(label.Position.X + Math.Abs(num), label.Position.Y);
				return;
			}
			newPos = label.Position;
		}

		internal void RearrangeLabelsAtRight(CircularAxisLabel label, Size baseArea, out Point newPos)
		{
			double num = (double)this.Visual.GetValue(Canvas.LeftProperty);
			double num2 = (double)this.Visual.GetValue(Canvas.TopProperty);
			if (label.Angle == 1.5707963267948966 && num2 + this.ActualHeight > baseArea.Height)
			{
				newPos = new Point(label.Position.X, label.Position.Y - (num2 + this.ActualHeight - baseArea.Height));
				return;
			}
			if (label.Angle == 4.71238898038469 && num2 < 0.0)
			{
				newPos = new Point(label.Position.X, label.Position.Y + Math.Abs(num2));
				return;
			}
			if (num + this.ActualWidth > baseArea.Width)
			{
				newPos = new Point(label.Position.X - Math.Abs(num + this.ActualWidth - baseArea.Width), label.Position.Y);
				return;
			}
			newPos = label.Position;
		}

		internal void SetPosition4CircularLabel(CircularAxisLabel label, bool isLeft)
		{
			if (isLeft)
			{
				this.SetPosition4LeftCircularLabel(label);
				return;
			}
			this.SetPosition4RightCircularLabel(label);
		}

		internal void SetPosition()
		{
			switch (this.Placement)
			{
			case PlacementTypes.Top:
				this.SetPositionTop(this.Position);
				return;
			case PlacementTypes.Left:
				this.SetPositionLeft(this.Position);
				return;
			case PlacementTypes.Right:
				this.SetPositionRight(this.Position);
				return;
			case PlacementTypes.Bottom:
				this.SetPositionBottom(this.Position);
				return;
			default:
				return;
			}
		}

		private void SetPositionRight(Point newPos)
		{
			double num = Math.Atan(this.ActualTextHeight / (2.0 * this.ActualTextWidth));
			double num2 = Math.Sqrt(Math.Pow(this.ActualTextHeight / 2.0, 2.0) + Math.Pow(this.ActualTextWidth, 2.0));
			double angle = AxisLabel.GetAngle(this.Angle);
			this.Rotation.Angle = angle;
			this.Rotation.CenterX = 0.0;
			this.Rotation.CenterY = 0.5;
			double num3 = this.ActualTextHeight / 2.0 * Math.Cos(AxisLabel.GetRadians(angle)) + this._padding4AxisYLabelInCircularChart;
			double num4 = -(this.ActualTextHeight / 2.0) * Math.Sin(AxisLabel.GetRadians(angle)) - this._padding4AxisXLabelInCircularChart;
			this.Visual.SetValue(Canvas.LeftProperty, newPos.X - num4);
			this.Visual.SetValue(Canvas.TopProperty, newPos.Y - num3);
			if (angle > 90.0)
			{
				this.ActualTop = (double)this.Visual.GetValue(Canvas.TopProperty) - num3 + num2 * Math.Sin(AxisLabel.GetRadians(angle) + num);
				this.ActualLeft = (double)this.Visual.GetValue(Canvas.LeftProperty);
				return;
			}
			this.ActualTop = (double)this.Visual.GetValue(Canvas.TopProperty);
			this.ActualLeft = (double)this.Visual.GetValue(Canvas.LeftProperty) + num4 - this.ActualTextHeight / 2.0 * Math.Cos(AxisLabel.GetRadians(90.0 - angle));
		}

		private void SetPositionLeft(Point newPos)
		{
			double radians = AxisLabel.GetRadians(this.Angle);
			double num = Math.Atan(this.ActualTextHeight / (2.0 * this.ActualTextWidth));
			double num2 = Math.Sqrt(Math.Pow(this.ActualTextHeight / 2.0, 2.0) + Math.Pow(this.ActualTextWidth, 2.0));
			double angle = AxisLabel.GetAngle(this.Angle);
			this.Rotation.Angle = angle;
			this.Rotation.CenterX = 0.0;
			this.Rotation.CenterY = 0.5;
			double num3 = num2 * Math.Cos(radians + num) + this._padding4AxisXLabelInCircularChart;
			double num4 = num2 * Math.Sin(radians + num) + this._padding4AxisYLabelInCircularChart;
			this.Visual.SetValue(Canvas.LeftProperty, newPos.X - num3);
			this.Visual.SetValue(Canvas.TopProperty, newPos.Y - num4);
			if (angle > 90.0)
			{
				this.ActualTop = (double)this.Visual.GetValue(Canvas.TopProperty) + num4 - this.ActualTextHeight / 2.0 * Math.Sin(AxisLabel.GetRadians(90.0 - angle));
				this.ActualLeft = (double)this.Visual.GetValue(Canvas.LeftProperty);
				return;
			}
			this.ActualTop = (double)this.Visual.GetValue(Canvas.TopProperty);
			this.ActualLeft = (double)this.Visual.GetValue(Canvas.LeftProperty) + num3 - this.ActualTextHeight / 2.0 * Math.Cos(AxisLabel.GetRadians(90.0 - this.Angle));
		}

		private void SetPositionBottom(Point newPos)
		{
			double angle = AxisLabel.GetAngle(this.Angle);
			if (angle == 0.0)
			{
				double num = this.ActualTextWidth / 2.0;
				double num2 = 0.0;
				this.Rotation.Angle = 0.0;
				this.Visual.SetValue(Canvas.LeftProperty, newPos.X - num);
				this.Visual.SetValue(Canvas.TopProperty, newPos.Y + num2 + this._padding4AxisXLabelInCircularChart);
				this.ActualTop = (double)this.Visual.GetValue(Canvas.TopProperty);
				this.ActualLeft = (double)this.Visual.GetValue(Canvas.LeftProperty);
				return;
			}
			if (angle > 90.0)
			{
				this.SetPositionLeft(newPos);
				return;
			}
			this.SetPositionRight(newPos);
		}

		private void SetPositionTop(Point newPos)
		{
			double angle = AxisLabel.GetAngle(this.Angle);
			if (angle == 0.0)
			{
				double num = this.ActualTextWidth / 2.0;
				double actualTextHeight = this.ActualTextHeight;
				this.Rotation.Angle = 0.0;
				this.Visual.SetValue(Canvas.LeftProperty, newPos.X - num);
				this.Visual.SetValue(Canvas.TopProperty, newPos.Y - actualTextHeight - this._padding4AxisXLabelInCircularChart);
				this.ActualTop = (double)this.Visual.GetValue(Canvas.TopProperty);
				this.ActualLeft = (double)this.Visual.GetValue(Canvas.LeftProperty);
				return;
			}
			if (angle > 90.0)
			{
				this.SetPositionRight(newPos);
				return;
			}
			this.SetPositionLeft(newPos);
		}

		private void CalculateSize(double radianAngle)
		{
			double num = Math.Sqrt(Math.Pow(this.ActualTextHeight, 2.0) + Math.Pow(this.ActualTextWidth, 2.0));
			double num2 = Math.Atan(this.ActualTextHeight / this.ActualTextWidth);
			double value = num * Math.Sin(radianAngle + num2);
			double value2 = num * Math.Sin(radianAngle - num2);
			double value3 = num * Math.Cos(radianAngle + num2);
			double value4 = num * Math.Cos(radianAngle - num2);
			this.ActualHeight = Math.Max(Math.Abs(value), Math.Abs(value2));
			this.ActualWidth = Math.Max(Math.Abs(value3), Math.Abs(value4));
		}

		private void CalculateTextElementSize()
		{
			if (!this.TextElement.IsMeasureValid)
			{
				this.TextElement.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			}
			this.ActualTextHeight = this.TextElement.DesiredSize.Height;
			this.ActualTextWidth = this.TextElement.DesiredSize.Width;
		}

		internal void ApplyProperties(AxisLabel axisLabel, FlowDirection flowDirection)
		{
			this.TextElement.Text = axisLabel.Text;
			this.TextElement.Foreground = this.FontColor;
			this.TextElement.FontSize = this.FontSize;
			this.TextElement.LineHeight = this.FontSize;
			this.TextElement.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
			this.TextElement.FlowDirection = flowDirection;
			this.TextElement.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
			if (this.FontFamily != null)
			{
				this.TextElement.FontFamily = this.FontFamily;
			}
			if (this.FontStyle != this._oldFontStyle)
			{
				this.TextElement.FontStyle = this.FontStyle;
			}
			if (this.FontWeight != this._oldFontWeight)
			{
				this.TextElement.FontWeight = this.FontWeight;
			}
			if (this.TextAlignment != this._oldTextAlignment)
			{
				this.TextElement.TextAlignment = this.TextAlignment;
			}
			this._oldFontStyle = this.FontStyle;
			this._oldFontWeight = this.FontWeight;
			this._oldTextAlignment = this.TextAlignment;
			if (!double.IsNaN(this.MaxWidth))
			{
				this.TextElement.TextWrapping = TextWrapping.Wrap;
				this.TextElement.MaxWidth = this.MaxWidth;
			}
		}

		internal void UpdateAxisLabel()
		{
			this.CalculateTextElementSize();
			this.CalculateSize(AxisLabel.GetRadians(this.Angle));
		}

		internal void CreateVisualObject(ObservableObject parentElement, FlowDirection flowDirection)
		{
			AxisLabelData tag = new AxisLabelData
			{
				Element = parentElement,
				LabelPosition = this.ActualLabelPosition
			};
			this.TextElement = new TextBlock
			{
				Tag = tag
			};
			this.Rotation = new RotateTransform();
			this.TextElement.RenderTransform = this.Rotation;
			this.Visual = this.TextElement;
			this.ApplyProperties(this, flowDirection);
			this.CalculateTextElementSize();
			this.CalculateSize(AxisLabel.GetRadians(this.Angle));
		}

		internal static double GetAngle(double angle)
		{
			if (angle < 0.0)
			{
				return 360.0 + angle;
			}
			return angle;
		}

		internal static double GetRadians(double angle)
		{
			return 0.017453292519943295 * AxisLabel.GetAngle(angle);
		}
	}
}
