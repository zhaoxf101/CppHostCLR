using System;
using System.Windows;

namespace Visifire.Charts
{
	internal class ElementPositionData
	{
		public FrameworkElement Element
		{
			get;
			set;
		}

		public double StartAngle
		{
			get;
			set;
		}

		public double StopAngle
		{
			get;
			set;
		}

		public ElementPositionData()
		{
		}

		public ElementPositionData(FrameworkElement element, double angle1, double angle2)
		{
			this.Element = element;
			this.StartAngle = angle1;
			this.StopAngle = angle2;
		}

		public ElementPositionData(ElementPositionData elementPosition)
		{
			this.Element = elementPosition.Element;
			this.StartAngle = elementPosition.StartAngle;
			this.StopAngle = elementPosition.StopAngle;
		}

		public static int CompareAngle(ElementPositionData elementPosition1, ElementPositionData elementPosition2)
		{
			double num = (elementPosition1.StartAngle + elementPosition1.StopAngle) / 2.0;
			double value = (elementPosition2.StartAngle + elementPosition2.StopAngle) / 2.0;
			return num.CompareTo(value);
		}
	}
}
