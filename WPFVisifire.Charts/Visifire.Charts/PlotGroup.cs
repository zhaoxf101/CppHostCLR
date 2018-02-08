using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Visifire.Charts
{
	internal class PlotGroup
	{
		internal double DrawingIndex;

		private List<double> _xValues;

		private double[] _zValues;

		private List<double> _yValues;

		internal List<IDataPoint> _dataPointsInCurrentPlotGroup;

		internal double _intialAxisXWidth;

		internal double _intialAxisYHeight;

		internal double _initialAxisXMin;

		internal double _initialAxisXMax;

		internal double _initialAxisYMin;

		internal double _initialAxisYMax;

		internal bool IsEnabled
		{
			get
			{
				foreach (DataSeries current in this.DataSeriesList)
				{
					if (current.Enabled.Value)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal Axis AxisX
		{
			get;
			private set;
		}

		internal Axis AxisY
		{
			get;
			private set;
		}

		internal RenderAs RenderAs
		{
			get;
			private set;
		}

		internal List<DataSeries> DataSeriesList
		{
			get;
			set;
		}

		internal Dictionary<double, XWiseStackedData> XWiseStackedDataList
		{
			get;
			private set;
		}

		internal double MaximumX
		{
			get;
			private set;
		}

		internal double MaximumY
		{
			get;
			private set;
		}

		internal double MaximumZ
		{
			get;
			private set;
		}

		internal double MinimumX
		{
			get;
			private set;
		}

		internal double MinimumY
		{
			get;
			private set;
		}

		internal double MinimumZ
		{
			get;
			private set;
		}

		internal double MinDifferenceX
		{
			get;
			private set;
		}

		public PlotGroup(RenderAs renderAs, Axis axisX, Axis axisY)
		{
			this.DataSeriesList = new List<DataSeries>();
			this.XWiseStackedDataList = new Dictionary<double, XWiseStackedData>();
			this.RenderAs = renderAs;
			this.AxisX = axisX;
			this.AxisY = axisY;
		}

		public double GetLimitingYValue()
		{
			double result = 0.0;
			if (this.AxisY != null)
			{
				if (this.AxisY.InternalAxisMinimum > 0.0)
				{
					result = this.AxisY.InternalAxisMinimum;
				}
				if (this.AxisY.InternalAxisMaximum < 0.0)
				{
					result = this.AxisY.InternalAxisMaximum;
				}
			}
			return result;
		}

		private void AddXWiseStackedDataEntry(ref XWiseStackedData xWiseData, IDataPoint dataPoint)
		{
			if (dataPoint.YValue >= 0.0)
			{
				xWiseData.Positive.Add(dataPoint);
				return;
			}
			xWiseData.Negative.Add(dataPoint);
		}

		private double GetMinDifference(List<double> xValues)
		{
			double num = 1.7976931348623157E+308;
			double[] array = xValues.ToArray();
			Array.Sort<double>(array);
			if (array.Length <= 1)
			{
				return double.PositiveInfinity;
			}
			if (this.AxisX.Logarithmic)
			{
				double logarithmBase = this.AxisX.LogarithmBase;
				for (int i = 0; i < array.Length - 1; i++)
				{
					if (array[i] <= 0.0)
					{
						throw new Exception("Negative or zero values cannot be plotted correctly on logarithmic charts");
					}
					if (Math.Log(array[i], logarithmBase) != Math.Log(array[i + 1], logarithmBase))
					{
						num = Math.Min(num, Math.Abs(Math.Log(array[i], logarithmBase) - Math.Log(array[i + 1], logarithmBase)));
					}
				}
			}
			else
			{
				for (int j = 0; j < array.Length - 1; j++)
				{
					if (array[j] != array[j + 1])
					{
						num = Math.Min(num, Math.Abs(array[j] - array[j + 1]));
					}
				}
			}
			if (num == 1.7976931348623157E+308)
			{
				num = double.PositiveInfinity;
			}
			return num;
		}

		private void CreateXWiseStackedDataEntry(ref List<IDataPoint> dataPointsInCurrentPlotGroup, params RenderAs[] chartTypes)
		{
			this.XWiseStackedDataList.Clear();
			bool logarithmic = this.AxisX.Logarithmic;
			double logBase = 10.0;
			if (logarithmic)
			{
				logBase = this.AxisX.LogarithmBase;
			}
			foreach (IDataPoint dataPoint in dataPointsInCurrentPlotGroup)
			{
				if (dataPoint.Parent != null)
				{
					dataPoint.DpInfo.InternalXValue = DataPointHelper.ConvertXValue2LogarithmicValue(dataPoint, logarithmic, logBase);
					if (chartTypes.Any((RenderAs w) => w == dataPoint.Parent.RenderAs))
					{
						XWiseStackedData value;
						if (this.XWiseStackedDataList.ContainsKey(dataPoint.DpInfo.InternalXValue))
						{
							value = this.XWiseStackedDataList[dataPoint.DpInfo.InternalXValue];
						}
						else
						{
							value = new XWiseStackedData();
							this.XWiseStackedDataList.Add(dataPoint.DpInfo.InternalXValue, value);
						}
						this.AddXWiseStackedDataEntry(ref value, dataPoint);
					}
				}
			}
		}

		internal List<Type> GetDependentVariableTypes()
		{
			List<Type> list = new List<Type>();
			int num = (from dataSeries in this.DataSeriesList
			where dataSeries.RenderAs == RenderAs.CandleStick || dataSeries.RenderAs == RenderAs.Stock
			select dataSeries).Count<DataSeries>();
			if (num == this.DataSeriesList.Count)
			{
				list.Add(typeof(List<double>));
			}
			else
			{
				list.Add(typeof(double));
				int num2 = (from dataSeries in this.DataSeriesList
				where dataSeries.RenderAs == RenderAs.Bubble
				select dataSeries).Count<DataSeries>();
				if (num2 > 0)
				{
					list.Add(typeof(double));
				}
			}
			return list;
		}

		public void LoadYValues(List<DataSeries> DataSeriesList)
		{
			foreach (DataSeries current in DataSeriesList)
			{
				current.PlotGroup = this;
				if (current.RenderAs == RenderAs.Stock || current.RenderAs == RenderAs.CandleStick)
				{
					foreach (IDataPoint current2 in current.InternalDataPoints)
					{
						if (current2.YValues != null)
						{
							this._yValues.AddRange(current2.YValues);
						}
					}
				}
			}
		}

		public void CalculateMinYValueWithInAXValueRange(double minXValue, double maxXValue, out double minimumY)
		{
			switch (this.RenderAs)
			{
			case RenderAs.StackedColumn:
			case RenderAs.StackedBar:
			case RenderAs.StackedArea:
			{
				this.CreateXWiseStackedDataEntry(ref this._dataPointsInCurrentPlotGroup, new RenderAs[]
				{
					RenderAs.StackedColumn,
					RenderAs.StackedBar,
					RenderAs.StackedArea
				});
				double[] xValuesInViewPort = RenderHelper.GetXValuesUnderViewPort(this.XWiseStackedDataList.Keys.ToList<double>(), this.AxisX, this.AxisY, false);
				IEnumerable<XWiseStackedData> source = from xWiseData in this.XWiseStackedDataList
				where xValuesInViewPort.Contains(xWiseData.Key)
				select xWiseData.Value;
				IEnumerable<double> source2 = from xwisedata in source
				select xwisedata.NegativeYValueSum;
				minimumY = (source2.Any<double>() ? source2.Min() : 0.0);
				return;
			}
			case RenderAs.StackedColumn100:
			case RenderAs.StackedBar100:
			case RenderAs.StackedArea100:
			{
				this.CreateXWiseStackedDataEntry(ref this._dataPointsInCurrentPlotGroup, new RenderAs[]
				{
					RenderAs.StackedColumn100,
					RenderAs.StackedBar100,
					RenderAs.StackedArea100
				});
				double[] xValuesInViewPort = RenderHelper.GetXValuesUnderViewPort(this.XWiseStackedDataList.Keys.ToList<double>(), this.AxisX, this.AxisY, false);
				IEnumerable<XWiseStackedData> source3 = from xWiseData in this.XWiseStackedDataList
				where xValuesInViewPort.Contains(xWiseData.Key)
				select xWiseData.Value;
				IEnumerable<double> source4 = from xwisedata in source3
				select xwisedata.NegativeYValueSum;
				minimumY = (source4.Any<double>() ? source4.Min() : 0.0);
				minimumY = (double)((minimumY >= 0.0) ? 0 : -100);
				return;
			}
			case RenderAs.Stock:
			case RenderAs.CandleStick:
			{
				List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(this, this._dataPointsInCurrentPlotGroup, true, 1);
				List<double> list = new List<double>();
				foreach (IDataPoint current in dataPointsUnderViewPort)
				{
					double[] yValues = current.YValues;
					if (yValues != null)
					{
						list.Add(yValues.Min());
					}
				}
				double num = 0.0;
				if (list.Any<double>())
				{
					num = list.Min();
				}
				minimumY = num;
				return;
			}
			}
			List<IDataPoint> dataPointsUnderViewPort2 = RenderHelper.GetDataPointsUnderViewPort(this, this._dataPointsInCurrentPlotGroup, true, (this.RenderAs == RenderAs.Spline) ? 3 : 1);
			double num2 = double.NaN;
			if (this.RenderAs == RenderAs.Spline)
			{
				using (List<DataSeries>.Enumerator enumerator2 = this.DataSeriesList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						DataSeries arg_2CA_0 = enumerator2.Current;
						double num3 = double.NaN;
						double num4 = double.NaN;
						double num5 = double.NaN;
						double num6 = double.NaN;
						this.CalculateMinMaxOfASplineCurve(RenderAs.Spline, dataPointsUnderViewPort2, ref num3, ref num4, ref num5, ref num6);
						num2 = (double.IsNaN(num2) ? num5 : Math.Min(num2, num5));
					}
					goto IL_3E5;
				}
			}
			if (this.RenderAs == RenderAs.Bubble)
			{
				List<IDataPoint> list2 = new List<IDataPoint>();
				double num7 = double.NaN;
				double num8 = double.NaN;
				double num9 = double.NaN;
				double num10 = double.NaN;
				foreach (DataSeries current2 in this.DataSeriesList)
				{
					list2.InsertRange(0, current2.InternalDataPoints);
				}
				PlotGroup.CalculateMinMaxOfABubbleChart(this, list2, ref num7, ref num8, ref num9, ref num10);
				num2 = (double.IsNaN(num2) ? num9 : Math.Min(num2, num9));
			}
			IL_3E5:
			List<double> list3 = new List<double>();
			foreach (IDataPoint current3 in dataPointsUnderViewPort2)
			{
				double yValue = current3.YValue;
				if (!double.IsNaN(yValue))
				{
					list3.Add(yValue);
				}
			}
			if (list3.Any<double>())
			{
				minimumY = (double.IsNaN(num2) ? list3.Min() : Math.Min(num2, list3.Min()));
				return;
			}
			minimumY = double.NaN;
		}

		public void CalculateMaxYValueWithInAXValueRange(double minXValue, double maxXValue, out double maximumY)
		{
			switch (this.RenderAs)
			{
			case RenderAs.StackedColumn:
			case RenderAs.StackedBar:
			case RenderAs.StackedArea:
			{
				this.CreateXWiseStackedDataEntry(ref this._dataPointsInCurrentPlotGroup, new RenderAs[]
				{
					RenderAs.StackedColumn,
					RenderAs.StackedBar,
					RenderAs.StackedArea
				});
				double[] xValuesInViewPort = RenderHelper.GetXValuesUnderViewPort(this.XWiseStackedDataList.Keys.ToList<double>(), this.AxisX, this.AxisY, true);
				IEnumerable<XWiseStackedData> source = from xWiseData in this.XWiseStackedDataList
				where xValuesInViewPort.Contains(xWiseData.Key)
				select xWiseData.Value;
				IEnumerable<double> source2 = from xwisedata in source
				select xwisedata.PositiveYValueSum;
				maximumY = (source2.Any<double>() ? source2.Max() : 0.0);
				return;
			}
			case RenderAs.StackedColumn100:
			case RenderAs.StackedBar100:
			case RenderAs.StackedArea100:
			{
				this.CreateXWiseStackedDataEntry(ref this._dataPointsInCurrentPlotGroup, new RenderAs[]
				{
					RenderAs.StackedColumn100,
					RenderAs.StackedBar100,
					RenderAs.StackedArea100
				});
				double[] xValuesInViewPort = RenderHelper.GetXValuesUnderViewPort(this.XWiseStackedDataList.Keys.ToList<double>(), this.AxisX, this.AxisY, true);
				IEnumerable<XWiseStackedData> source3 = from xWiseData in this.XWiseStackedDataList
				where xValuesInViewPort.Contains(xWiseData.Key)
				select xWiseData.Value;
				IEnumerable<double> source4 = from xwisedata in source3
				select xwisedata.PositiveYValueSum;
				maximumY = (source4.Any<double>() ? source4.Max() : 0.0);
				maximumY = (double)((maximumY > 0.0) ? 100 : 0);
				return;
			}
			case RenderAs.Stock:
			case RenderAs.CandleStick:
			{
				List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(this, this._dataPointsInCurrentPlotGroup, true, 3);
				List<double> list = new List<double>();
				foreach (IDataPoint current in dataPointsUnderViewPort)
				{
					double[] yValues = current.YValues;
					if (yValues != null)
					{
						list.Add(yValues.Max());
					}
				}
				double num = 0.0;
				if (list.Any<double>())
				{
					num = list.Max();
				}
				maximumY = num;
				return;
			}
			}
			List<IDataPoint> dataPointsUnderViewPort2 = RenderHelper.GetDataPointsUnderViewPort(this, this._dataPointsInCurrentPlotGroup, true, (this.RenderAs == RenderAs.Spline) ? 3 : 1);
			double num2 = double.NaN;
			if (this.RenderAs == RenderAs.Spline)
			{
				using (List<DataSeries>.Enumerator enumerator2 = this.DataSeriesList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						DataSeries arg_2CA_0 = enumerator2.Current;
						double num3 = double.NaN;
						double num4 = double.NaN;
						double num5 = double.NaN;
						double num6 = double.NaN;
						this.CalculateMinMaxOfASplineCurve(RenderAs.Spline, dataPointsUnderViewPort2, ref num3, ref num4, ref num5, ref num6);
						num2 = (double.IsNaN(num2) ? num6 : Math.Min(num2, num6));
					}
					goto IL_3E5;
				}
			}
			if (this.RenderAs == RenderAs.Bubble)
			{
				List<IDataPoint> list2 = new List<IDataPoint>();
				double num7 = double.NaN;
				double num8 = double.NaN;
				double num9 = double.NaN;
				double num10 = double.NaN;
				foreach (DataSeries current2 in this.DataSeriesList)
				{
					list2.InsertRange(0, current2.InternalDataPoints);
				}
				PlotGroup.CalculateMinMaxOfABubbleChart(this, list2, ref num7, ref num8, ref num9, ref num10);
				num2 = (double.IsNaN(num2) ? num10 : Math.Min(num2, num10));
			}
			IL_3E5:
			List<double> list3 = new List<double>();
			foreach (IDataPoint current3 in dataPointsUnderViewPort2)
			{
				double yValue = current3.YValue;
				if (!double.IsNaN(yValue))
				{
					list3.Add(yValue);
				}
			}
			if (list3.Any<double>())
			{
				maximumY = (double.IsNaN(num2) ? list3.Max() : Math.Max(num2, list3.Max()));
				return;
			}
			maximumY = double.NaN;
		}

		private void CalculateMinMaxOfASplineCurve(RenderAs renderAs, List<IDataPoint> dataPoints, ref double chartSpecificMinX, ref double chartSpecificMaxX, ref double chartSpecificMinY, ref double chartSpecificMaxY)
		{
			if (renderAs == RenderAs.Spline)
			{
				List<Point> list = (from dataPoint in dataPoints
				where !double.IsNaN(dataPoint.DpInfo.ActualNumericXValue) && !double.IsNaN(dataPoint.YValue)
				select new Point(dataPoint.DpInfo.ActualNumericXValue, dataPoint.YValue)).ToList<Point>();
				double num = 0.0;
				foreach (IDataPoint current in dataPoints)
				{
					if (current.Parent != null)
					{
						num = current.Parent.LineTension;
						break;
					}
				}
				num = 1.0 / num;
				if (list.Any<Point>())
				{
					PointCollection pointCollection = new PointCollection();
					for (int i = 0; i < list.Count; i++)
					{
						pointCollection.Add(list[i]);
					}
					new PointCollection();
					PointCollection pointCollection2;
					PointCollection pointCollection3;
					Bezier.GetBezierPoints(pointCollection, num, out pointCollection2, out pointCollection3);
					Point[] array = new Point[pointCollection2.Count];
					Point[] array2 = new Point[pointCollection3.Count];
					for (int j = 0; j < pointCollection2.Count; j++)
					{
						array[j] = pointCollection2[j];
					}
					for (int k = 0; k < pointCollection3.Count; k++)
					{
						array2[k] = pointCollection3[k];
					}
					if (this.AxisY.Logarithmic)
					{
						chartSpecificMinX = double.NaN;
						chartSpecificMaxX = double.NaN;
						chartSpecificMinY = double.NaN;
						chartSpecificMaxY = double.NaN;
						return;
					}
					chartSpecificMinX = double.NaN;
					chartSpecificMaxX = double.NaN;
					if (array != null && array2 != null)
					{
						if (array.Any<Point>())
						{
							chartSpecificMinY = Math.Min(double.IsNaN(chartSpecificMinY) ? 1.7976931348623157E+308 : chartSpecificMinY, (from point in array
							select point.Y).Min());
							chartSpecificMaxY = Math.Max(double.IsNaN(chartSpecificMaxY) ? -1.7976931348623157E+308 : chartSpecificMaxY, (from point in array
							select point.Y).Max());
						}
						if (array2.Any<Point>())
						{
							chartSpecificMinY = Math.Min(chartSpecificMinY, (from point in array2
							select point.Y).Min());
							chartSpecificMaxY = Math.Max(chartSpecificMaxY, (from point in array2
							select point.Y).Max());
							return;
						}
					}
				}
			}
			else
			{
				chartSpecificMinX = double.NaN;
				chartSpecificMaxX = double.NaN;
				chartSpecificMinY = double.NaN;
				chartSpecificMaxY = double.NaN;
			}
		}

		private static void CalculateMinMaxOfABubbleCurve(RenderAs renderAs, List<IDataPoint> dataPoints, Chart chart, PlotGroup plotGroup, ref double chartSpecificMinX, ref double chartSpecificMaxX, ref double chartSpecificMinY, ref double chartSpecificMaxY)
		{
			if (plotGroup != null && chart != null && chart.PlotDetails != null && chart.PlotDetails.AutoFitToPlotArea)
			{
				List<IDataPoint> list = (from dataPoint in dataPoints
				where !double.IsNaN(dataPoint.DpInfo.ActualNumericXValue) && !double.IsNaN(dataPoint.YValue) && !double.IsNaN(dataPoint.ZValue)
				select dataPoint).ToList<IDataPoint>();
				if (list.Any<IDataPoint>())
				{
					Point[] array = BubbleChart.CalculateControlPointsOfADataSeries(chart, plotGroup, list);
					if (array != null && array.Any<Point>())
					{
						chartSpecificMinX = Math.Min(double.IsNaN(chartSpecificMinX) ? 1.7976931348623157E+308 : chartSpecificMinX, (from point in array
						select point.X).Min());
						chartSpecificMinY = Math.Min(double.IsNaN(chartSpecificMinY) ? 1.7976931348623157E+308 : chartSpecificMinY, (from point in array
						select point.Y).Min());
						chartSpecificMaxX = Math.Max(double.IsNaN(chartSpecificMaxX) ? -1.7976931348623157E+308 : chartSpecificMaxX, (from point in array
						select point.X).Max());
						chartSpecificMaxY = Math.Max(double.IsNaN(chartSpecificMaxY) ? -1.7976931348623157E+308 : chartSpecificMaxY, (from point in array
						select point.Y).Max());
						return;
					}
				}
			}
			else
			{
				chartSpecificMinX = double.NaN;
				chartSpecificMaxX = double.NaN;
				chartSpecificMinY = double.NaN;
				chartSpecificMaxY = double.NaN;
			}
		}

		private static void CalculateMinMaxOfABubbleChart(PlotGroup plotGroup, List<IDataPoint> dataPoints, ref double chartSpecificMinX, ref double chartSpecificMaxX, ref double chartSpecificMinY, ref double chartSpecificMaxY)
		{
			if (plotGroup.RenderAs == RenderAs.Bubble && plotGroup.AxisX != null && plotGroup.AxisY != null && plotGroup.AxisY.Visual != null && plotGroup.AxisX.Visual != null)
			{
				PlotGroup.CalculateMinMaxOfABubbleCurve(plotGroup.RenderAs, dataPoints, (Chart)plotGroup.AxisX.Chart, plotGroup, ref chartSpecificMinX, ref chartSpecificMaxX, ref chartSpecificMinY, ref chartSpecificMaxY);
				return;
			}
			chartSpecificMinX = double.NaN;
			chartSpecificMaxX = double.NaN;
			chartSpecificMinY = double.NaN;
			chartSpecificMaxY = double.NaN;
		}

		public void Update(VcProperties property, object oldValue, object newValue)
		{
			double num = double.NaN;
			double num2 = double.NaN;
			double num3 = double.NaN;
			double num4 = double.NaN;
			RenderAs renderAs = this.RenderAs;
			if (property == VcProperties.None || property == VcProperties.DataPoints)
			{
				this._dataPointsInCurrentPlotGroup = new List<IDataPoint>();
				if (renderAs == RenderAs.Bubble)
				{
					List<IDataPoint> list = new List<IDataPoint>();
					foreach (DataSeries current in this.DataSeriesList)
					{
						List<IDataPoint> list2 = (from dp in current.InternalDataPoints
						where dp.Enabled == true
						select dp).ToList<IDataPoint>();
						list.InsertRange(0, list2);
						if (list2.Count == current.InternalDataPoints.Count)
						{
							current.IsAllDataPointsEnabled = true;
						}
						else
						{
							current.IsAllDataPointsEnabled = false;
						}
						current.PlotGroup = this;
					}
					PlotGroup.CalculateMinMaxOfABubbleChart(this, list, ref num, ref num2, ref num3, ref num4);
					this._dataPointsInCurrentPlotGroup.InsertRange(this._dataPointsInCurrentPlotGroup.Count, list);
				}
				else
				{
					foreach (DataSeries current2 in this.DataSeriesList)
					{
						List<IDataPoint> list3 = new List<IDataPoint>();
						list3 = (from dp in current2.InternalDataPoints
						where dp.Enabled == true
						select dp).ToList<IDataPoint>();
						if (list3.Count == current2.InternalDataPoints.Count)
						{
							current2.IsAllDataPointsEnabled = true;
						}
						else
						{
							current2.IsAllDataPointsEnabled = false;
						}
						this.CalculateMinMaxOfASplineCurve(renderAs, list3, ref num, ref num2, ref num3, ref num4);
						this._dataPointsInCurrentPlotGroup.InsertRange(this._dataPointsInCurrentPlotGroup.Count, list3);
						current2.PlotGroup = this;
					}
				}
			}
			if (property == VcProperties.None || property == VcProperties.XValue || property == VcProperties.DataPoints || property == VcProperties.Series)
			{
				this._xValues = new List<double>();
				foreach (IDataPoint current3 in this._dataPointsInCurrentPlotGroup)
				{
					double actualNumericXValue = current3.DpInfo.ActualNumericXValue;
					if (!double.IsNaN(actualNumericXValue))
					{
						this._xValues.Add(actualNumericXValue);
					}
				}
				if (!double.IsNaN(num))
				{
					this._xValues.Add(num);
				}
				if (!double.IsNaN(num2))
				{
					this._xValues.Add(num2);
				}
				this.MaximumX = (this._xValues.Any<double>() ? this._xValues.Max() : 0.0);
				this.MinimumX = (this._xValues.Any<double>() ? this._xValues.Min() : 0.0);
				if (renderAs == RenderAs.Polar)
				{
					if (this._xValues.Count > 0 && this.MaximumX < 360.0)
					{
						this.MaximumX = 360.0;
					}
					if (this._xValues.Count > 0 && this.MinimumX < 0.0)
					{
						this.MinimumX = 0.0;
					}
				}
				this.MinDifferenceX = this.GetMinDifference(this._xValues);
			}
			if (property == VcProperties.None || property == VcProperties.DataPoints || property == VcProperties.Series || property == VcProperties.ZValue)
			{
				if (property == VcProperties.ZValue)
				{
					double num5 = (double)newValue;
					this.MaximumZ = ((num5 > this.MaximumZ) ? num5 : this.MaximumZ);
					this.MinimumZ = ((num5 < this.MinimumZ) ? num5 : this.MinimumZ);
				}
				else
				{
					if (this.GetDependentVariableTypes().Count == 2)
					{
						this._zValues = (from dataPoint in this._dataPointsInCurrentPlotGroup
						where !double.IsNaN(dataPoint.ZValue)
						select dataPoint.ZValue).Distinct<double>().ToArray<double>();
					}
					this.MaximumZ = ((this._zValues != null && this._zValues.Any<double>()) ? this._zValues.Max() : 0.0);
					this.MinimumZ = ((this._zValues != null && this._zValues.Any<double>()) ? this._zValues.Min() : 0.0);
				}
			}
			if (property == VcProperties.None || property == VcProperties.DataPoints || property == VcProperties.YValues || property == VcProperties.YValue || property == VcProperties.XValue)
			{
				double num6 = double.NaN;
				double num7 = double.NaN;
				if (renderAs == RenderAs.Spline || property == VcProperties.None || property == VcProperties.DataPoints)
				{
					List<Type> dependentVariableTypes = this.GetDependentVariableTypes();
					if (dependentVariableTypes.Count == 1 && dependentVariableTypes[0] == typeof(List<double>))
					{
						this._yValues = new List<double>();
					}
					else
					{
						this._yValues = new List<double>();
						foreach (IDataPoint current4 in this._dataPointsInCurrentPlotGroup)
						{
							double yValue = current4.YValue;
							if (!double.IsNaN(yValue))
							{
								this._yValues.Add(yValue);
							}
						}
					}
					if (!double.IsNaN(num3))
					{
						this._yValues.Add(num3);
					}
					if (!double.IsNaN(num4))
					{
						this._yValues.Add(num4);
					}
					using (List<DataSeries>.Enumerator enumerator5 = this.DataSeriesList.GetEnumerator())
					{
						while (enumerator5.MoveNext())
						{
							DataSeries current5 = enumerator5.Current;
							RenderAs renderAs2 = current5.RenderAs;
							if (renderAs2 == RenderAs.CandleStick || renderAs2 == RenderAs.Stock)
							{
								foreach (IDataPoint current6 in current5.InternalDataPoints)
								{
									double[] yValues = current6.YValues;
									if (yValues != null)
									{
										this._yValues.Add(yValues.Max());
										this._yValues.Add(yValues.Min());
									}
								}
							}
						}
						goto IL_6D7;
					}
				}
				if (property == VcProperties.YValue)
				{
					this._yValues.Remove((double)oldValue);
					if (!double.IsNaN((double)newValue))
					{
						this._yValues.Add((double)newValue);
					}
				}
				else if (property == VcProperties.YValues)
				{
					if (oldValue != null)
					{
						this._yValues.Remove(((double[])oldValue).Max());
						this._yValues.Remove(((double[])oldValue).Min());
					}
					if (newValue != null)
					{
						this._yValues.Add(num6 = ((double[])newValue).Max());
						this._yValues.Add(num7 = ((double[])newValue).Min());
					}
				}
				IL_6D7:
				switch (renderAs)
				{
				case RenderAs.Column:
				case RenderAs.Line:
				case RenderAs.Pie:
				case RenderAs.Bar:
				case RenderAs.Area:
				case RenderAs.Doughnut:
				case RenderAs.Bubble:
				case RenderAs.Point:
				case RenderAs.StreamLineFunnel:
				case RenderAs.SectionFunnel:
				case RenderAs.Stock:
				case RenderAs.CandleStick:
				case RenderAs.StepLine:
				case RenderAs.Spline:
				case RenderAs.Radar:
				case RenderAs.Polar:
				case RenderAs.Pyramid:
				case RenderAs.QuickLine:
					if (property == VcProperties.YValue)
					{
						double num8 = (double)newValue;
						if (num8 > this.MaximumY)
						{
							this.MaximumY = num8;
							if (this._yValues.Any<double>())
							{
								this.MinimumY = this._yValues.Min();
								return;
							}
						}
						else
						{
							if (num8 >= this.MinimumY)
							{
								if (this._yValues.Any<double>())
								{
									this.MaximumY = this._yValues.Max();
									this.MinimumY = this._yValues.Min();
								}
								else
								{
									this.MaximumY = 0.0;
									this.MinimumY = 0.0;
								}
								this.MaximumY = this.MaximumY;
								return;
							}
							this.MinimumY = num8;
							if (this._yValues.Any<double>())
							{
								this.MaximumY = this._yValues.Max();
								return;
							}
						}
					}
					else
					{
						if (property != VcProperties.YValues)
						{
							this.MaximumY = (this._yValues.Any<double>() ? this._yValues.Max() : 0.0);
							this.MinimumY = (this._yValues.Any<double>() ? this._yValues.Min() : 0.0);
							return;
						}
						if (num6 > this.MaximumY)
						{
							this.MaximumY = num6;
							return;
						}
						if (num7 < this.MinimumY)
						{
							this.MinimumY = num7;
							return;
						}
						if (this._yValues.Any<double>())
						{
							this.MaximumY = this._yValues.Max();
							this.MinimumY = this._yValues.Min();
							return;
						}
						this.MaximumY = 0.0;
						this.MinimumY = 0.0;
						return;
					}
					break;
				case RenderAs.StackedColumn:
				case RenderAs.StackedBar:
				case RenderAs.StackedArea:
				{
					this.CreateXWiseStackedDataEntry(ref this._dataPointsInCurrentPlotGroup, new RenderAs[]
					{
						RenderAs.StackedColumn,
						RenderAs.StackedBar,
						RenderAs.StackedArea
					});
					IEnumerable<double> source = from xwisedata in this.XWiseStackedDataList.Values
					select xwisedata.PositiveYValueSum;
					IEnumerable<double> source2 = from xwisedata in this.XWiseStackedDataList.Values
					select xwisedata.NegativeYValueSum;
					if (source2.Any<double>() && source2.Min() == source2.Max() && source2.Min() == 0.0)
					{
						this.MaximumY = (source.Any<double>() ? source.Max() : source2.Max());
						this.MinimumY = this.GetFirstDPMinFromPositiveXWiseStackedDataPointList();
						return;
					}
					if (source.Any<double>() && source.Min() == source.Max() && source.Min() == 0.0)
					{
						this.MaximumY = this.GetFirstDPMaxFromNegativeXWiseStackedDataPointList();
						this.MinimumY = (source2.Any<double>() ? source2.Min() : source.Min());
						return;
					}
					this.MaximumY = (source.Any<double>() ? source.Max() : 0.0);
					this.MinimumY = (source2.Any<double>() ? source2.Min() : 0.0);
					return;
				}
				case RenderAs.StackedColumn100:
				case RenderAs.StackedBar100:
				case RenderAs.StackedArea100:
				{
					this.CreateXWiseStackedDataEntry(ref this._dataPointsInCurrentPlotGroup, new RenderAs[]
					{
						RenderAs.StackedColumn100,
						RenderAs.StackedBar100,
						RenderAs.StackedArea100
					});
					IEnumerable<double> source3 = from xwisedata in this.XWiseStackedDataList.Values
					select xwisedata.PositiveYValueSum;
					IEnumerable<double> source4 = from xwisedata in this.XWiseStackedDataList.Values
					select xwisedata.NegativeYValueSum;
					this.MaximumY = (source3.Any<double>() ? source3.Max() : 0.0);
					this.MinimumY = (source4.Any<double>() ? source4.Min() : 0.0);
					this.MaximumY = (double)((this.MaximumY >= 0.0) ? 100 : 0);
					this.MinimumY = (double)((this.MinimumY >= 0.0) ? 0 : -100);
					break;
				}
				default:
					return;
				}
			}
		}

		private double GetFirstDPMinFromPositiveXWiseStackedDataPointList()
		{
			IEnumerable<double> source = from data in this.XWiseStackedDataList.Values
			where data.Positive.Count > 0
			select data.Positive.First<IDataPoint>().YValue;
			if (source.Count<double>() > 0)
			{
				return source.Min();
			}
			return 0.0;
		}

		private double GetFirstDPMaxFromNegativeXWiseStackedDataPointList()
		{
			IEnumerable<double> source = from data in this.XWiseStackedDataList.Values
			where data.Negative.Count > 0
			select data.Negative.First<IDataPoint>().YValue;
			if (source.Count<double>() > 0)
			{
				return source.Max();
			}
			return 0.0;
		}

		internal void ClearInstanceRefs()
		{
			this._dataPointsInCurrentPlotGroup.Clear();
			this._dataPointsInCurrentPlotGroup = null;
			this.DataSeriesList.Clear();
			this.DataSeriesList = null;
			this.XWiseStackedDataList.Clear();
			this.XWiseStackedDataList = null;
			this.AxisX = null;
			this.AxisY = null;
		}
	}
}
