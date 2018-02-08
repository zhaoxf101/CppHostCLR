using System;
using System.Windows.Input;

namespace Visifire.Charts
{
	public class PlotAreaMouseEventArgs : EventArgs
	{
		public MouseEventArgs MouseEventArgs
		{
			get;
			internal set;
		}

		public object XValue
		{
			get;
			internal set;
		}

		public double YValue
		{
			get;
			internal set;
		}

		public PlotAreaMouseEventArgs(MouseEventArgs e)
		{
			this.XValue = double.NaN;
			this.YValue = double.NaN;
			this.MouseEventArgs = e;
		}

		public object GetXValue(Axis axis)
		{
			if (axis.Chart == null || (axis.Chart as Chart).ChartArea == null)
			{
				return null;
			}
			MouseEventArgs mouseEventArgs = this.MouseEventArgs;
			AxisOrientation axisOrientation = axis.AxisOrientation;
			double num = (axisOrientation == AxisOrientation.Horizontal) ? mouseEventArgs.GetPosition((axis.Chart as Chart).ChartArea.PlottingCanvas).X : mouseEventArgs.GetPosition((axis.Chart as Chart).ChartArea.PlottingCanvas).Y;
			double num2 = (axisOrientation == AxisOrientation.Horizontal) ? (axis.Chart as Chart).ChartArea.ChartVisualCanvas.Width : (axis.Chart as Chart).ChartArea.ChartVisualCanvas.Height;
			double num3 = axis.PixelPositionToXValue(num2, (axisOrientation == AxisOrientation.Horizontal) ? num : (num2 - num));
			object result;
			if (axis.IsDateTimeAxis)
			{
				try
				{
					result = DateTimeHelper.XValueToDateTime(axis.MinDate, num3, axis.InternalIntervalType);
					return result;
				}
				catch
				{
					result = axis.MinDate;
					return result;
				}
			}
			if (axis.Logarithmic)
			{
				result = DataPointHelper.ConvertLogarithmicValue2ActualValue(axis.Chart as Chart, num3, axis.AxisType);
			}
			else
			{
				result = num3;
			}
			return result;
		}

		public double GetYValue(Axis axis)
		{
			if (axis.Chart == null || (axis.Chart as Chart).ChartArea == null)
			{
				return double.NaN;
			}
			MouseEventArgs mouseEventArgs = this.MouseEventArgs;
			AxisOrientation axisOrientation = axis.AxisOrientation;
			double num = (axisOrientation == AxisOrientation.Vertical) ? mouseEventArgs.GetPosition((axis.Chart as Chart).ChartArea.PlottingCanvas).Y : mouseEventArgs.GetPosition((axis.Chart as Chart).ChartArea.PlottingCanvas).X;
			double num2 = (axisOrientation == AxisOrientation.Vertical) ? (axis.Chart as Chart).ChartArea.ChartVisualCanvas.Height : (axis.Chart as Chart).ChartArea.ChartVisualCanvas.Width;
			double num3 = axis.PixelPositionToYValue(num2, (axisOrientation == AxisOrientation.Vertical) ? num : (num2 - num));
			double result;
			if (axis.Logarithmic)
			{
				result = DataPointHelper.ConvertLogarithmicValue2ActualValue(axis.Chart as Chart, num3, axis.AxisType);
			}
			else
			{
				result = num3;
			}
			return result;
		}
	}
}
