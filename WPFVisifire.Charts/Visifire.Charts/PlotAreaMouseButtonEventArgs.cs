using System;
using System.Windows.Input;

namespace Visifire.Charts
{
	public class PlotAreaMouseButtonEventArgs : EventArgs
	{
		public MouseButtonEventArgs MouseButtonEventArgs
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

		public PlotAreaMouseButtonEventArgs(MouseButtonEventArgs e)
		{
			this.XValue = double.NaN;
			this.YValue = double.NaN;
			this.MouseButtonEventArgs = e;
		}

		public object GetXValue(Axis axis)
		{
			if (axis.Chart == null || (axis.Chart as Chart).ChartArea == null)
			{
				return null;
			}
			MouseButtonEventArgs mouseButtonEventArgs = this.MouseButtonEventArgs;
			AxisOrientation axisOrientation = axis.AxisOrientation;
			double num = (axisOrientation == AxisOrientation.Horizontal) ? mouseButtonEventArgs.GetPosition((axis.Chart as Chart).ChartArea.PlottingCanvas).X : mouseButtonEventArgs.GetPosition((axis.Chart as Chart).ChartArea.PlottingCanvas).Y;
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
			MouseButtonEventArgs mouseButtonEventArgs = this.MouseButtonEventArgs;
			AxisOrientation axisOrientation = axis.AxisOrientation;
			double num = (axisOrientation == AxisOrientation.Vertical) ? mouseButtonEventArgs.GetPosition((axis.Chart as Chart).ChartArea.PlottingCanvas).Y : mouseButtonEventArgs.GetPosition((axis.Chart as Chart).ChartArea.PlottingCanvas).X;
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
