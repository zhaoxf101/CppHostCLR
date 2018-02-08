using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class PlotDetails
	{
		private List<IDataPoint> ListOfAllDataPointsPrimary = new List<IDataPoint>();

		private List<IDataPoint> ListOfAllDataPointsSecondary = new List<IDataPoint>();

		public bool IsOverrideViewPortRangeEnabled;

		private Axis _axisXPrimary;

		private List<IDataPoint> _listOfAllDataPoints;

		internal List<IDataPoint> ListOfAllDataPoints
		{
			get
			{
				return this._listOfAllDataPoints;
			}
		}

		internal bool AutoFitToPlotArea
		{
			get;
			private set;
		}

		internal List<PlotGroup> PlotGroups
		{
			get;
			private set;
		}

		internal Chart Chart
		{
			get;
			private set;
		}

		internal ChartOrientationType ChartOrientation
		{
			get;
			set;
		}

		internal int DrawingDivisionFactor
		{
			get;
			private set;
		}

		internal int Layer3DCount
		{
			get;
			private set;
		}

		internal Dictionary<DataSeries, int> SeriesDrawingIndex
		{
			get;
			set;
		}

		internal Dictionary<double, string> AxisXPrimaryLabels
		{
			get;
			private set;
		}

		internal Dictionary<double, string> AxisXSecondaryLabels
		{
			get;
			private set;
		}

		internal bool IsAllPrimaryAxisXLabelsPresent
		{
			get;
			set;
		}

		internal bool IsAllSecondaryAxisXLabelsPresent
		{
			get;
			set;
		}

		public PlotDetails(Chart chart)
		{
			this.PlotGroups = new List<PlotGroup>();
			this.AxisXPrimaryLabels = new Dictionary<double, string>();
			this.AxisXSecondaryLabels = new Dictionary<double, string>();
			this.Chart = chart;
			this.ChartOrientation = ChartOrientationType.Undefined;
		}

		public void ReCreate(IElement element, ElementTypes elementType, VcProperties property, object oldValue, object newValue)
		{
			this.CalculateInternalXValue4NumericAxis(this.Chart);
			if ((elementType == ElementTypes.Chart && property == VcProperties.Series) || (elementType == ElementTypes.DataSeries && property == VcProperties.RenderAs))
			{
				this.ChartOrientation = ChartOrientationType.Undefined;
			}
			if ((elementType == ElementTypes.Chart && property == VcProperties.Series) || (elementType == ElementTypes.DataSeries && property == VcProperties.RenderAs) || (elementType == ElementTypes.DataPoint && property == VcProperties.XValue) || (elementType == ElementTypes.Chart && property == VcProperties.None))
			{
				this.SetDataPointsNameAndValidateDataPointXValueType();
			}
			this.Calculate(element, elementType, property, oldValue, newValue);
		}

		private void SetDataPointsNameAndValidateDataPointXValueType()
		{
			int num = 0;
			foreach (DataSeries current in this.Chart.InternalSeries)
			{
				ChartValueTypes xValueType = current.XValueType;
				RenderAs renderAs = current.RenderAs;
				foreach (IDataPoint current2 in current.InternalDataPoints)
				{
					ChartValueTypes xValueType2 = current2.DpInfo.XValueType;
					if (xValueType == ChartValueTypes.Auto)
					{
						if (renderAs == RenderAs.Pie || renderAs == RenderAs.Doughnut || renderAs == RenderAs.SectionFunnel || renderAs == RenderAs.StreamLineFunnel || renderAs == RenderAs.Pyramid)
						{
							current.InternalXValueType = ChartValueTypes.Numeric;
						}
						else
						{
							current.InternalXValueType = ((xValueType2 == ChartValueTypes.DateTime) ? ChartValueTypes.Date : xValueType2);
						}
					}
					else
					{
						if ((xValueType == ChartValueTypes.Date || xValueType == ChartValueTypes.DateTime || xValueType == ChartValueTypes.Time) && xValueType2 != ChartValueTypes.DateTime)
						{
							throw new Exception("Error occurred due to incorrect XValue format. XValue can be Double or DateTime for all DataPoints in a DataSeries.");
						}
						if (xValueType == ChartValueTypes.Numeric && xValueType2 != ChartValueTypes.Numeric)
						{
							throw new Exception("Error occurred due to incorrect XValue format. XValue can be Double or DateTime for all DataPoints in a DataSeries according to XValueType of the DataSeries.");
						}
					}
				}
				num++;
			}
		}

		private bool CalculateOverrideViewPortRangeEnabled(Chart chart)
		{
			foreach (DataSeries current in this.Chart.InternalSeries)
			{
				if (RenderHelper.IsRasterRenderSupported(current))
				{
					return true;
				}
			}
			return false;
		}

		private void Calculate(IElement element, ElementTypes elementType, VcProperties property, object oldValue, object newValue)
		{
			this.AutoFitToPlotArea = PlotDetails.CalculateAutoFitToPlotArea(this.Chart);
			if (elementType == ElementTypes.Chart && property == VcProperties.Series)
			{
				this.CreateMissingAxes();
				this.IsOverrideViewPortRangeEnabled = this.CalculateOverrideViewPortRangeEnabled(this.Chart);
			}
			if (elementType == ElementTypes.Chart && property == VcProperties.AxesX)
			{
				Axis axisXFromChart = this.GetAxisXFromChart(this.Chart, AxisTypes.Secondary);
				if (axisXFromChart != null)
				{
					throw new Exception("Note: Secondary AxisX is not yet supported in Visifire.");
				}
			}
			if ((elementType == ElementTypes.DataSeries && (property == VcProperties.DataPoints || property == VcProperties.XValueType)) || (elementType == ElementTypes.DataPoint && property == VcProperties.XValue) || property == VcProperties.None)
			{
				if (this._axisXPrimary == null)
				{
					this._axisXPrimary = this.GetAxisXFromChart(this.Chart, AxisTypes.Primary);
				}
				this.GenerateXValueForDateTimeAxis(this._axisXPrimary);
				this.IsOverrideViewPortRangeEnabled = this.CalculateOverrideViewPortRangeEnabled(this.Chart);
			}
			else if (elementType == ElementTypes.Chart && property == VcProperties.AxesX)
			{
				this._axisXPrimary = this.GetAxisXFromChart(this.Chart, AxisTypes.Primary);
				this.GenerateXValueForDateTimeAxis(this._axisXPrimary);
			}
			if ((elementType == ElementTypes.Chart && property == VcProperties.Series) || (elementType == ElementTypes.DataSeries && (property == VcProperties.DataPoints || property == VcProperties.Enabled)))
			{
				this.CreateListOfDataPointsDependingOnXAxis();
			}
			if ((elementType == ElementTypes.Chart && property == VcProperties.None) || (elementType == ElementTypes.Chart && property == VcProperties.Series) || (elementType == ElementTypes.DataSeries && (property == VcProperties.RenderAs || property == VcProperties.Enabled)))
			{
				this.PopulatePlotGroups();
			}
			else if (elementType == ElementTypes.DataSeries && property == VcProperties.DataPoints)
			{
				(element as DataSeries).PlotGroup.Update(property, oldValue, newValue);
			}
			else if (elementType == ElementTypes.DataPoint && (property == VcProperties.XValue || property == VcProperties.YValue || property == VcProperties.YValues))
			{
				(element as IDataPoint).Parent.PlotGroup.Update(property, oldValue, newValue);
			}
			if (elementType == ElementTypes.Chart && property == VcProperties.TrendLines)
			{
				this.SetTrendLineValues(this._axisXPrimary);
				this.SetTrendLineStartAndEndValues(this._axisXPrimary);
			}
			if ((elementType == ElementTypes.Chart && property == VcProperties.Series) || (elementType == ElementTypes.DataSeries && property == VcProperties.RenderAs) || (elementType == ElementTypes.DataSeries && property == VcProperties.DataPoints))
			{
				this.SeriesDrawingIndex = this.GenerateDrawingOrder();
			}
			if ((elementType == ElementTypes.Chart && property == VcProperties.Series) || (elementType == ElementTypes.DataPoint && property == VcProperties.AxisXLabel))
			{
				this.AxisXPrimaryLabels.Clear();
				this.AxisXPrimaryLabels = null;
				this.AxisXSecondaryLabels.Clear();
				this.AxisXSecondaryLabels = null;
				this.AxisXPrimaryLabels = this.GetAxisXLabels(AxisTypes.Primary);
				this.AxisXSecondaryLabels = this.GetAxisXLabels(AxisTypes.Secondary);
			}
			else if (elementType == ElementTypes.DataSeries && property == VcProperties.DataPoints)
			{
				DataSeries dataSeries = element as DataSeries;
				if (dataSeries.AxisXType == AxisTypes.Primary)
				{
					this.AxisXPrimaryLabels.Clear();
					this.AxisXPrimaryLabels = null;
					this.AxisXPrimaryLabels = this.GetAxisXLabels(AxisTypes.Primary);
				}
				else
				{
					this.AxisXSecondaryLabels.Clear();
					this.AxisXSecondaryLabels = null;
					this.AxisXSecondaryLabels = this.GetAxisXLabels(AxisTypes.Secondary);
				}
			}
			if ((elementType == ElementTypes.Chart && property == VcProperties.Series) || (elementType == ElementTypes.DataSeries && property == VcProperties.DataPoints) || (elementType == ElementTypes.DataPoint && property == VcProperties.AxisXLabel))
			{
				this.SetLabelsCountState();
			}
			if (elementType == ElementTypes.DataPoint && (property == VcProperties.YValue || property == VcProperties.YValues || property == VcProperties.Enabled))
			{
				(element as IDataPoint).DpInfo.InternalYValue = DataPointHelper.ConvertYValue2LogarithmicValue(element as IDataPoint, (element as IDataPoint).Parent.PlotGroup.AxisY.Logarithmic, (element as IDataPoint).Parent.PlotGroup.AxisY.LogarithmBase);
				(element as IDataPoint).DpInfo.InternalYValues = DataPointHelper.ConvertYValues2LogarithmicValue(element as IDataPoint, (element as IDataPoint).Parent.PlotGroup.AxisY.Logarithmic, (element as IDataPoint).Parent.PlotGroup.AxisY.LogarithmBase);
			}
			else
			{
				if (elementType == ElementTypes.DataSeries)
				{
					DataSeries dataSeries2 = element as DataSeries;
					RenderAs renderAs = dataSeries2.RenderAs;
					bool flag = false;
					double logBase = 10.0;
					if (this.Chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis)
					{
						flag = dataSeries2.PlotGroup.AxisY.Logarithmic;
						if (flag)
						{
							logBase = dataSeries2.PlotGroup.AxisY.LogarithmBase;
						}
					}
					using (List<IDataPoint>.Enumerator enumerator = dataSeries2.InternalDataPoints.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IDataPoint current = enumerator.Current;
							if (renderAs == RenderAs.CandleStick || renderAs == RenderAs.Stock)
							{
								current.DpInfo.InternalYValues = DataPointHelper.ConvertYValues2LogarithmicValue(current, flag, logBase);
							}
							else
							{
								current.DpInfo.InternalYValue = DataPointHelper.ConvertYValue2LogarithmicValue(current, flag, logBase);
							}
						}
						goto IL_42B;
					}
				}
				if (elementType == ElementTypes.Chart && (property == VcProperties.Series || property == VcProperties.None))
				{
					this.ConvertYValue2LogarithmicValue4DataPoints(this.Chart);
					this.ConvertYValues2LogarithmicValue4DataPoints(this.Chart);
				}
			}
			IL_42B:
			if (elementType == ElementTypes.DataPoint && (property == VcProperties.XValue || property == VcProperties.Enabled))
			{
				(element as IDataPoint).DpInfo.InternalXValue = DataPointHelper.ConvertXValue2LogarithmicValue(element as IDataPoint, (element as IDataPoint).Parent.PlotGroup.AxisX.Logarithmic, (element as IDataPoint).Parent.PlotGroup.AxisX.LogarithmBase);
				return;
			}
			if (elementType == ElementTypes.DataSeries)
			{
				DataSeries dataSeries3 = element as DataSeries;
				bool flag2 = false;
				double logBase2 = 10.0;
				if (this.Chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis)
				{
					flag2 = dataSeries3.PlotGroup.AxisX.Logarithmic;
					if (flag2)
					{
						logBase2 = dataSeries3.PlotGroup.AxisX.LogarithmBase;
					}
				}
				using (List<IDataPoint>.Enumerator enumerator2 = dataSeries3.InternalDataPoints.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						IDataPoint current2 = enumerator2.Current;
						current2.DpInfo.InternalXValue = DataPointHelper.ConvertXValue2LogarithmicValue(current2, flag2, logBase2);
					}
					return;
				}
			}
			if (elementType == ElementTypes.Chart && (property == VcProperties.Series || property == VcProperties.None))
			{
				this.ConvertXValue2LogarithmicValue4DataPoints(this.Chart);
			}
		}

		internal void GenerateInternalXValueForCircularChart(Axis axisX)
		{
			List<DataSeries> list = this.Chart.InternalSeries.ToList<DataSeries>();
			foreach (DataSeries current in list)
			{
				if (current.RenderAs != RenderAs.Polar)
				{
					int num = 0;
					using (IEnumerator<IDataPoint> enumerator2 = current.DataPoints.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							IDataPoint current2 = enumerator2.Current;
							current2.DpInfo.ActualNumericXValue = (double)num++;
						}
						continue;
					}
				}
				axisX.IsDateTimeAxis = this.CheckIsDateTimeAxis(axisX);
				if (axisX.IsDateTimeAxis)
				{
					this.GenerateXValueForDateTimeAxis(axisX);
					axisX.MinDate = DateTime.Parse("12/30/1899", CultureInfo.InvariantCulture);
				}
			}
		}

		internal static bool CalculateAutoFitToPlotArea(Chart chart)
		{
			if (chart != null && chart.InternalSeries != null)
			{
				foreach (DataSeries current in chart.InternalSeries)
				{
					if (current.RenderAs == RenderAs.Bubble && current.AutoFitToPlotArea)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private void ConvertXValue2LogarithmicValue4DataPoints(Chart chart)
		{
			foreach (DataSeries current in chart.InternalSeries)
			{
				bool flag = false;
				double logBase = 10.0;
				if (chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis)
				{
					flag = current.PlotGroup.AxisX.Logarithmic;
					if (flag)
					{
						logBase = current.PlotGroup.AxisX.LogarithmBase;
					}
				}
				foreach (IDataPoint current2 in current.InternalDataPoints)
				{
					current2.DpInfo.InternalXValue = DataPointHelper.ConvertXValue2LogarithmicValue(current2, flag, logBase);
				}
			}
		}

		private void ConvertYValue2LogarithmicValue4DataPoints(Chart chart)
		{
			foreach (DataSeries current in chart.InternalSeries)
			{
				if (!RenderHelper.IsFinancialCType(current))
				{
					bool flag = false;
					double logBase = 10.0;
					if (chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis)
					{
						flag = current.PlotGroup.AxisY.Logarithmic;
						if (flag)
						{
							logBase = current.PlotGroup.AxisY.LogarithmBase;
						}
					}
					foreach (IDataPoint current2 in current.InternalDataPoints)
					{
						current2.DpInfo.InternalYValue = DataPointHelper.ConvertYValue2LogarithmicValue(current2, flag, logBase);
					}
				}
			}
		}

		private void ConvertYValues2LogarithmicValue4DataPoints(Chart chart)
		{
			foreach (DataSeries current in chart.InternalSeries)
			{
				if (RenderHelper.IsFinancialCType(current))
				{
					bool flag = false;
					double logBase = 10.0;
					if (chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis)
					{
						flag = current.PlotGroup.AxisY.Logarithmic;
						if (flag)
						{
							logBase = current.PlotGroup.AxisY.LogarithmBase;
						}
					}
					foreach (IDataPoint current2 in current.InternalDataPoints)
					{
						current2.DpInfo.InternalYValues = DataPointHelper.ConvertYValues2LogarithmicValue(current2, flag, logBase);
					}
				}
			}
		}

		private void CalculateInternalXValue4NumericAxis(Chart chart)
		{
			foreach (DataSeries current in chart.InternalSeries)
			{
				int num = 1;
				if (current.IsXValueNull4AllDataPoints())
				{
					using (List<IDataPoint>.Enumerator enumerator2 = current.InternalDataPoints.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							IDataPoint current2 = enumerator2.Current;
							current2.DpInfo.ActualNumericXValue = (double)num++;
						}
						continue;
					}
				}
				double num2 = 0.0;
				foreach (IDataPoint current3 in current.InternalDataPoints)
				{
					if (current3.XValue == null && double.IsNaN(current3.DpInfo.ActualNumericXValue))
					{
						num2 = (current3.DpInfo.ActualNumericXValue = num2 + 1.0);
					}
					else if (!double.IsNaN(current3.DpInfo.ActualNumericXValue))
					{
						num2 = current3.DpInfo.ActualNumericXValue;
					}
					else
					{
						current3.DpInfo.ActualNumericXValue = (double)num;
						num2 = (double)num;
					}
					num++;
				}
			}
		}

		internal void CalculateInternalXValuesOfDataPoints()
		{
			this._axisXPrimary = this.GetAxisXFromChart(this.Chart, AxisTypes.Primary);
			this.CalculateInternalXValue4NumericAxis(this.Chart);
			if (this.GetChartOrientation() != ChartOrientationType.Circular)
			{
				this.GenerateXValueForDateTimeAxis(this._axisXPrimary);
			}
		}

		public void Calculate(bool isUpdateAxisLabelsList)
		{
			this.AutoFitToPlotArea = PlotDetails.CalculateAutoFitToPlotArea(this.Chart);
			this.SetDataPointsNameAndValidateDataPointXValueType();
			this.CreateMissingAxes();
			Axis axisXFromChart = this.GetAxisXFromChart(this.Chart, AxisTypes.Secondary);
			if (axisXFromChart != null)
			{
				throw new Exception("Note: Secondary AxisX is not yet supported in Visifire.");
			}
			this._axisXPrimary = this.GetAxisXFromChart(this.Chart, AxisTypes.Primary);
			this.CalculateInternalXValuesOfDataPoints();
			this.CreateListOfDataPointsDependingOnXAxis();
			this.IsOverrideViewPortRangeEnabled = this.CalculateOverrideViewPortRangeEnabled(this.Chart);
			this.PopulatePlotGroups();
			this.ConvertYValue2LogarithmicValue4DataPoints(this.Chart);
			this.ConvertXValue2LogarithmicValue4DataPoints(this.Chart);
			this.ConvertYValues2LogarithmicValue4DataPoints(this.Chart);
			if (this.ChartOrientation == ChartOrientationType.Circular)
			{
				this.GenerateInternalXValueForCircularChart(this._axisXPrimary);
			}
			this.SetTrendLineValues(this._axisXPrimary);
			this.SetTrendLineStartAndEndValues(this._axisXPrimary);
			this.SeriesDrawingIndex = this.GenerateDrawingOrder();
			if (this.ChartOrientation == ChartOrientationType.Circular)
			{
				if (!this.CheckWhetherAllDataSeriesArePrimary(this.Chart.InternalSeries))
				{
					throw new Exception("Circular charts does not support Secondary AxisY");
				}
				this.AxisXPrimaryLabels = this.GetAxisXLabels4CircularAxisType();
			}
			else
			{
				if (isUpdateAxisLabelsList)
				{
					this.AxisXPrimaryLabels = this.GetAxisXLabels(AxisTypes.Primary);
				}
				this.AxisXSecondaryLabels = this.GetAxisXLabels(AxisTypes.Secondary);
			}
			this.SetLabelsCountState();
		}

		private bool CheckWhetherAllDataSeriesArePrimary(List<DataSeries> seriesList)
		{
			IEnumerable<DataSeries> source = from ds in seriesList
			where ds.AxisYType == AxisTypes.Primary
			select ds;
			return source.Any<DataSeries>() && source.Count<DataSeries>() == seriesList.Count;
		}

		private List<DateTime> GetListOfXValue(Axis axis)
		{
			List<DateTime> list = new List<DateTime>();
			int num = 0;
			int num2 = 0;
			foreach (DataSeries current in this.Chart.InternalSeries)
			{
				num2 = 0;
				foreach (IDataPoint current2 in current.InternalDataPoints)
				{
					DateTime arg_52_0 = current2.DpInfo.InternalXValueAsDateTime;
					try
					{
						if (current2.DpInfo.XValueType == ChartValueTypes.Numeric)
						{
							throw new Exception();
						}
						if (current.InternalXValueType == ChartValueTypes.Auto || current.InternalXValueType == ChartValueTypes.Date)
						{
							current2.DpInfo.InternalXValueAsDateTime = new DateTime(current2.DpInfo.ActualXValueAsDateTime.Date.Year, current2.DpInfo.ActualXValueAsDateTime.Date.Month, current2.DpInfo.ActualXValueAsDateTime.Date.Day);
							list.Add(current2.DpInfo.InternalXValueAsDateTime);
						}
						else if (current.InternalXValueType == ChartValueTypes.DateTime)
						{
							current2.DpInfo.InternalXValueAsDateTime = current2.DpInfo.ActualXValueAsDateTime;
							list.Add(current2.DpInfo.ActualXValueAsDateTime);
						}
						else if (current.InternalXValueType == ChartValueTypes.Time)
						{
							current2.DpInfo.InternalXValueAsDateTime = DateTime.Parse("12/30/1899 " + current2.DpInfo.ActualXValueAsDateTime.TimeOfDay.ToString(), CultureInfo.InvariantCulture);
							list.Add(current2.DpInfo.InternalXValueAsDateTime);
						}
					}
					catch
					{
						throw new Exception("Wrong property value as date for XValue in DataPoint position " + num2.ToString() + " and DataSeries position " + num.ToString());
					}
					num2++;
				}
				num++;
			}
			return list;
		}

		private bool CheckIsDateTimeAxis(Axis axis)
		{
			bool flag = false;
			axis.XValueType = ChartValueTypes.Numeric;
			bool flag2 = false;
			if (this.Chart.InternalSeries.Count == 0)
			{
				return axis.IsDateTimeAxis;
			}
			int num = (from ds in this.Chart.InternalSeries
			where ds.DataPoints != null && ds.DataPoints.Count > 0
			select ds).Count<DataSeries>();
			if (num == 0)
			{
				using (List<DataSeries>.Enumerator enumerator = this.Chart.InternalSeries.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DataSeries current = enumerator.Current;
						if (current.InternalXValueType == ChartValueTypes.Date)
						{
							flag = true;
							axis.XValueType = ChartValueTypes.Date;
							bool result = flag;
							return result;
						}
						if (current.InternalXValueType == ChartValueTypes.DateTime)
						{
							flag = true;
							axis.XValueType = ChartValueTypes.DateTime;
							bool result = flag;
							return result;
						}
						if (current.InternalXValueType == ChartValueTypes.Time)
						{
							flag = true;
							axis.XValueType = ChartValueTypes.Time;
							bool result = flag;
							return result;
						}
						if (current.InternalXValueType == ChartValueTypes.Auto)
						{
							flag = false;
							axis.XValueType = ChartValueTypes.Numeric;
							bool result = flag;
							return result;
						}
					}
					goto IL_217;
				}
			}
			IEnumerable<DataSeries> source = from dataSeries in this.Chart.InternalSeries
			where dataSeries.InternalDataPoints.Count > 0 && (dataSeries.InternalXValueType == ChartValueTypes.Date && dataSeries.XValueType != ChartValueTypes.DateTime && dataSeries.XValueType != ChartValueTypes.Time) && dataSeries.XValueType != ChartValueTypes.Numeric
			select dataSeries;
			int num2 = source.Any<DataSeries>() ? source.Count<DataSeries>() : 0;
			flag2 = (num2 > 0);
			if (num2 == num)
			{
				flag = true;
				axis.XValueType = ChartValueTypes.Date;
			}
			else
			{
				source = from dataSeries in this.Chart.InternalSeries
				where dataSeries.InternalDataPoints.Count > 0 && (dataSeries.InternalXValueType == ChartValueTypes.DateTime && dataSeries.XValueType != ChartValueTypes.Date && dataSeries.XValueType != ChartValueTypes.Time) && dataSeries.XValueType != ChartValueTypes.Numeric
				select dataSeries;
				num2 = (source.Any<DataSeries>() ? source.Count<DataSeries>() : 0);
				flag2 = (num2 > 0 || flag2);
				if (num2 == num)
				{
					flag = true;
					axis.XValueType = ChartValueTypes.DateTime;
				}
				else
				{
					source = from dataSeries in this.Chart.InternalSeries
					where dataSeries.InternalDataPoints.Count > 0 && (dataSeries.InternalXValueType == ChartValueTypes.Time && dataSeries.XValueType != ChartValueTypes.Date && dataSeries.XValueType != ChartValueTypes.DateTime) && dataSeries.XValueType != ChartValueTypes.Numeric
					select dataSeries;
					num2 = (source.Any<DataSeries>() ? source.Count<DataSeries>() : 0);
					flag2 = (num2 > 0 || flag2);
					if (num2 == num)
					{
						flag = true;
						axis.XValueType = ChartValueTypes.Time;
					}
				}
			}
			IL_217:
			if (!flag && flag2)
			{
				throw new Exception("Error occurred, different types of XValue type found in DataSeries. Check for property value applied for XValueType property of different DataSeries.");
			}
			return flag;
		}

		internal void SetTrendLineValues(Axis axisX)
		{
			if (axisX.IsDateTimeAxis)
			{
				foreach (TrendLine current in this.Chart.TrendLines)
				{
					if (current.Enabled.Value && ((current.Orientation == Orientation.Vertical && axisX.PlotDetails.ChartOrientation == ChartOrientationType.Vertical) || (current.Orientation == Orientation.Horizontal && axisX.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)))
					{
						if (axisX.XValueType == ChartValueTypes.Time)
						{
							DateTime dateTime = DateTime.Parse("12/30/1899 " + current.InternalDateValue.TimeOfDay.ToString(), CultureInfo.InvariantCulture);
							current.InternalNumericValue = DateTimeHelper.DateDiff(dateTime, axisX.MinDate, axisX.MinDateRange, axisX.MaxDateRange, axisX.InternalIntervalType, axisX.XValueType);
						}
						else
						{
							current.InternalNumericValue = DateTimeHelper.DateDiff(current.InternalDateValue, axisX.MinDate, axisX.MinDateRange, axisX.MaxDateRange, axisX.InternalIntervalType, axisX.XValueType);
						}
					}
				}
			}
		}

		internal void SetTrendLineStartAndEndValues(Axis axisX)
		{
			if (axisX.IsDateTimeAxis)
			{
				foreach (TrendLine current in this.Chart.TrendLines)
				{
					if (current.Enabled.Value && ((current.Orientation == Orientation.Vertical && axisX.PlotDetails.ChartOrientation == ChartOrientationType.Vertical) || (current.Orientation == Orientation.Horizontal && axisX.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)))
					{
						DateTime arg_75_0 = current.InternalDateStartValue;
						DateTime arg_7C_0 = current.InternalDateEndValue;
						if (axisX.XValueType == ChartValueTypes.Time)
						{
							DateTime dateTime = DateTime.Parse("12/30/1899 " + current.InternalDateStartValue.TimeOfDay.ToString(), CultureInfo.InvariantCulture);
							current.InternalNumericStartValue = DateTimeHelper.DateDiff(dateTime, axisX.MinDate, axisX.MinDateRange, axisX.MaxDateRange, axisX.InternalIntervalType, axisX.XValueType);
							DateTime dateTime2 = DateTime.Parse("12/30/1899 " + current.InternalDateEndValue.TimeOfDay.ToString(), CultureInfo.InvariantCulture);
							current.InternalNumericEndValue = DateTimeHelper.DateDiff(dateTime2, axisX.MinDate, axisX.MinDateRange, axisX.MaxDateRange, axisX.InternalIntervalType, axisX.XValueType);
						}
						else
						{
							current.InternalNumericStartValue = DateTimeHelper.DateDiff(current.InternalDateStartValue, axisX.MinDate, axisX.MinDateRange, axisX.MaxDateRange, axisX.InternalIntervalType, axisX.XValueType);
							current.InternalNumericEndValue = DateTimeHelper.DateDiff(current.InternalDateEndValue, axisX.MinDate, axisX.MinDateRange, axisX.MaxDateRange, axisX.InternalIntervalType, axisX.XValueType);
						}
					}
				}
			}
		}

		private void ResetDateTime4AxisMinimumAndMaximum(Axis axis)
		{
			if (axis.AxisMinimum != null)
			{
				if (axis._axisMinimumValueType == ChartValueTypes.Numeric)
				{
					throw new Exception("AxisMinimum should have a value of type Date/DateTime/Time");
				}
				if (axis.XValueType == ChartValueTypes.Date)
				{
					axis.AxisMinimumDateTime = new DateTime(axis.AxisMinimumDateTime.Year, axis.AxisMinimumDateTime.Month, axis.AxisMinimumDateTime.Day);
				}
				else if (axis.XValueType == ChartValueTypes.Time)
				{
					axis.AxisMinimumDateTime = DateTime.Parse("12/30/1899 " + axis.AxisMinimumDateTime.TimeOfDay.ToString(), CultureInfo.InvariantCulture);
				}
			}
			if (axis.AxisMaximum != null)
			{
				if (axis._axisMaximumValueType == ChartValueTypes.Numeric)
				{
					throw new Exception("AxisMaximum should have a value of type Date/DateTime/Time");
				}
				if (axis.XValueType == ChartValueTypes.Date)
				{
					axis.AxisMaximumDateTime = new DateTime(axis.AxisMaximumDateTime.Year, axis.AxisMaximumDateTime.Month, axis.AxisMaximumDateTime.Day);
					return;
				}
				if (axis.XValueType == ChartValueTypes.Time)
				{
					axis.AxisMaximumDateTime = DateTime.Parse("12/30/1899 " + axis.AxisMaximumDateTime.TimeOfDay.ToString(), CultureInfo.InvariantCulture);
				}
			}
		}

		private void GenerateXValueForDateTimeAxis(Axis axisX)
		{
			if (axisX != null)
			{
				axisX._isDateTimeAutoInterval = false;
				axisX.IsDateTimeAxis = this.CheckIsDateTimeAxis(axisX);
				if (axisX.IsDateTimeAxis)
				{
					List<DateTime> listOfXValue = this.GetListOfXValue(axisX);
					this.ResetDateTime4AxisMinimumAndMaximum(axisX);
					if (listOfXValue.Count != 0)
					{
						DateTime dateTime;
						DateTime dateTime2;
						TimeSpan timeSpan;
						TimeSpan timeSpan2;
						if (this.ChartOrientation == ChartOrientationType.Circular)
						{
							DateTimeHelper.CalculateMinMaxDate4PolarChart(listOfXValue, axisX, out dateTime, out dateTime2, out timeSpan, out timeSpan2);
						}
						else
						{
							DateTimeHelper.CalculateMinMaxDate(listOfXValue, out dateTime, out dateTime2, out timeSpan, out timeSpan2);
						}
						IntervalTypes intervalTypes = axisX.InternalIntervalType;
						double maxInterval = (double)((axisX.XValueType == ChartValueTypes.Date || axisX.XValueType == ChartValueTypes.Time) ? 8 : 8);
						if (dateTime != dateTime2)
						{
							if (this.Chart.ChartArea._isFirstTimeRender)
							{
								ChartOrientationType chartOrientationType = ChartOrientationType.Undefined;
								using (IEnumerator<DataSeries> enumerator = this.Chart.Series.GetEnumerator())
								{
									if (enumerator.MoveNext())
									{
										DataSeries current = enumerator.Current;
										if (current.RenderAs == RenderAs.Bar || current.RenderAs == RenderAs.StackedBar || current.RenderAs == RenderAs.StackedBar100)
										{
											chartOrientationType = ChartOrientationType.Horizontal;
										}
										else if (current.RenderAs == RenderAs.Radar || current.RenderAs == RenderAs.Polar)
										{
											chartOrientationType = ChartOrientationType.Circular;
										}
										else if (current.RenderAs == RenderAs.Pie || current.RenderAs == RenderAs.Doughnut || current.RenderAs == RenderAs.StreamLineFunnel || current.RenderAs == RenderAs.SectionFunnel || current.RenderAs == RenderAs.Pyramid)
										{
											chartOrientationType = ChartOrientationType.NoAxis;
										}
										else
										{
											chartOrientationType = ChartOrientationType.Vertical;
										}
									}
								}
								if (chartOrientationType == ChartOrientationType.Vertical)
								{
									axisX.InternalInterval = DateTimeHelper.CalculateAutoInterval(31000.0, this.Chart.ActualHeight, axisX.AxisOrientation, dateTime, dateTime2, out intervalTypes, maxInterval, axisX.XValueType);
								}
								else if (chartOrientationType == ChartOrientationType.Horizontal)
								{
									axisX.InternalInterval = DateTimeHelper.CalculateAutoInterval(this.Chart.ActualWidth, 31000.0, axisX.AxisOrientation, dateTime, dateTime2, out intervalTypes, maxInterval, axisX.XValueType);
								}
								else
								{
									axisX.InternalInterval = DateTimeHelper.CalculateAutoInterval(this.Chart.ActualWidth, this.Chart.ActualHeight, axisX.AxisOrientation, dateTime, dateTime2, out intervalTypes, maxInterval, axisX.XValueType);
								}
							}
							else
							{
								if (this.Chart.ChartArea.ChartVisualCanvas == null)
								{
									return;
								}
								ChartOrientationType chartOrientationType2 = ChartOrientationType.Undefined;
								using (IEnumerator<DataSeries> enumerator2 = this.Chart.Series.GetEnumerator())
								{
									if (enumerator2.MoveNext())
									{
										DataSeries current2 = enumerator2.Current;
										RenderAs renderAs = current2.RenderAs;
										if (renderAs == RenderAs.Bar || renderAs == RenderAs.StackedBar || renderAs == RenderAs.StackedBar100)
										{
											chartOrientationType2 = ChartOrientationType.Horizontal;
										}
										else if (renderAs == RenderAs.Radar || renderAs == RenderAs.Polar)
										{
											chartOrientationType2 = ChartOrientationType.Circular;
										}
										else if (renderAs == RenderAs.Pie || renderAs == RenderAs.Doughnut || renderAs == RenderAs.StreamLineFunnel || renderAs == RenderAs.SectionFunnel || renderAs == RenderAs.Pyramid)
										{
											chartOrientationType2 = ChartOrientationType.NoAxis;
										}
										else
										{
											chartOrientationType2 = ChartOrientationType.Vertical;
										}
									}
								}
								if (chartOrientationType2 == ChartOrientationType.Vertical)
								{
									axisX.InternalInterval = DateTimeHelper.CalculateAutoInterval(31000.0, this.Chart.ChartArea.ChartVisualCanvas.Height, axisX.AxisOrientation, dateTime, dateTime2, out intervalTypes, maxInterval, axisX.XValueType);
								}
								else if (chartOrientationType2 == ChartOrientationType.Horizontal)
								{
									axisX.InternalInterval = DateTimeHelper.CalculateAutoInterval(this.Chart.ChartArea.ChartVisualCanvas.Width, 31000.0, axisX.AxisOrientation, dateTime, dateTime2, out intervalTypes, maxInterval, axisX.XValueType);
								}
								else
								{
									axisX.InternalInterval = DateTimeHelper.CalculateAutoInterval(this.Chart.ChartArea.ChartVisualCanvas.Width, this.Chart.ChartArea.ChartVisualCanvas.Height, axisX.AxisOrientation, dateTime, dateTime2, out intervalTypes, maxInterval, axisX.XValueType);
								}
							}
						}
						else
						{
							if (axisX.XValueType != ChartValueTypes.Time)
							{
								if (axisX.AxisMinimum == null)
								{
									if (axisX.InternalIntervalType == IntervalTypes.Hours)
									{
										dateTime = dateTime.AddHours(-1.0);
										intervalTypes = axisX.InternalIntervalType;
									}
									else if (axisX.InternalIntervalType == IntervalTypes.Minutes)
									{
										dateTime = dateTime.AddMinutes(-1.0);
										intervalTypes = axisX.InternalIntervalType;
									}
									else if (axisX.InternalIntervalType == IntervalTypes.Seconds)
									{
										dateTime = dateTime.AddSeconds(-1.0);
										intervalTypes = axisX.InternalIntervalType;
									}
									else if (axisX.InternalIntervalType == IntervalTypes.Milliseconds)
									{
										dateTime = dateTime.AddMilliseconds(-1.0);
										intervalTypes = axisX.InternalIntervalType;
									}
									else
									{
										dateTime = dateTime.AddDays(-1.0);
										intervalTypes = IntervalTypes.Days;
									}
								}
								else
								{
									TimeSpan timeSpan3 = dateTime.Subtract(axisX.AxisMinimumDateTime);
									if (timeSpan3.TotalDays <= 30.0)
									{
										intervalTypes = IntervalTypes.Days;
										dateTime = dateTime.AddDays(-1.0);
									}
									else if (timeSpan3.TotalDays / 30.0 < 12.0)
									{
										intervalTypes = IntervalTypes.Months;
										dateTime = dateTime.AddMonths(-1);
									}
									else if (timeSpan3.TotalDays / 365.242199 > 0.0)
									{
										intervalTypes = IntervalTypes.Years;
										dateTime = dateTime.AddYears(-1);
									}
									else if (timeSpan3.TotalHours > 0.0)
									{
										intervalTypes = IntervalTypes.Hours;
										dateTime = dateTime.AddHours(-1.0);
									}
									else if (timeSpan3.TotalMinutes > 0.0)
									{
										intervalTypes = IntervalTypes.Minutes;
										dateTime = dateTime.AddMinutes(-1.0);
									}
									else if (timeSpan3.TotalSeconds > 0.0)
									{
										intervalTypes = IntervalTypes.Seconds;
										dateTime = dateTime.AddSeconds(-1.0);
									}
									else if (timeSpan3.TotalMilliseconds > 0.0)
									{
										intervalTypes = IntervalTypes.Milliseconds;
										dateTime = dateTime.AddMilliseconds(-1.0);
									}
								}
							}
							else if (axisX.AxisMinimum == null)
							{
								if (axisX.InternalIntervalType == IntervalTypes.Hours)
								{
									dateTime = dateTime.AddHours(-1.0);
									intervalTypes = axisX.InternalIntervalType;
								}
								else if (axisX.InternalIntervalType == IntervalTypes.Minutes)
								{
									dateTime = dateTime.AddMinutes(-1.0);
									intervalTypes = axisX.InternalIntervalType;
								}
								else if (axisX.InternalIntervalType == IntervalTypes.Seconds)
								{
									dateTime = dateTime.AddSeconds(-1.0);
									intervalTypes = axisX.InternalIntervalType;
								}
								else if (axisX.InternalIntervalType == IntervalTypes.Milliseconds)
								{
									dateTime = dateTime.AddMilliseconds(-1.0);
									intervalTypes = axisX.InternalIntervalType;
								}
							}
							else
							{
								TimeSpan timeSpan4 = dateTime.Subtract(axisX.AxisMinimumDateTime);
								if (timeSpan4.TotalHours > 0.0)
								{
									intervalTypes = IntervalTypes.Hours;
									dateTime = dateTime.AddHours(-1.0);
								}
								else if (timeSpan4.TotalMinutes > 0.0)
								{
									intervalTypes = IntervalTypes.Minutes;
									dateTime = dateTime.AddMinutes(-1.0);
								}
								else if (timeSpan4.TotalSeconds > 0.0)
								{
									intervalTypes = IntervalTypes.Seconds;
									dateTime = dateTime.AddSeconds(-1.0);
								}
								else if (timeSpan4.TotalMilliseconds > 0.0)
								{
									intervalTypes = IntervalTypes.Milliseconds;
									dateTime = dateTime.AddMilliseconds(-1.0);
								}
							}
							axisX.InternalInterval = 1.0;
							axisX._isDateTimeAutoInterval = true;
						}
						if (axisX.XValueType == ChartValueTypes.DateTime || axisX.XValueType == ChartValueTypes.Date)
						{
							if (axisX.IntervalType == IntervalTypes.Auto || double.IsNaN(axisX.Interval.Value))
							{
								axisX._isDateTimeAutoInterval = true;
								axisX.InternalIntervalType = intervalTypes;
							}
							else
							{
								bool flag = false;
								if (axisX.XValueType == ChartValueTypes.Date && (axisX.IntervalType == IntervalTypes.Hours || axisX.IntervalType == IntervalTypes.Minutes || axisX.IntervalType == IntervalTypes.Seconds || axisX.IntervalType == IntervalTypes.Milliseconds || axisX.IntervalType == IntervalTypes.Auto))
								{
									flag = true;
								}
								if (axisX.IntervalType == IntervalTypes.Auto)
								{
									flag = true;
								}
								if (!axisX.Interval.HasValue || double.IsNaN(axisX.Interval.Value))
								{
									if (axisX.IntervalType == IntervalTypes.Years)
									{
										if (intervalTypes == IntervalTypes.Months || intervalTypes == IntervalTypes.Weeks || intervalTypes == IntervalTypes.Days || intervalTypes == IntervalTypes.Hours || intervalTypes == IntervalTypes.Minutes || intervalTypes == IntervalTypes.Seconds || intervalTypes == IntervalTypes.Milliseconds)
										{
											flag = true;
										}
									}
									else if (axisX.IntervalType == IntervalTypes.Months)
									{
										if (intervalTypes == IntervalTypes.Weeks || intervalTypes == IntervalTypes.Days || intervalTypes == IntervalTypes.Hours || intervalTypes == IntervalTypes.Minutes || intervalTypes == IntervalTypes.Seconds || intervalTypes == IntervalTypes.Milliseconds)
										{
											flag = true;
										}
									}
									else if (axisX.IntervalType == IntervalTypes.Weeks)
									{
										if (intervalTypes == IntervalTypes.Days || intervalTypes == IntervalTypes.Hours || intervalTypes == IntervalTypes.Minutes || intervalTypes == IntervalTypes.Seconds || intervalTypes == IntervalTypes.Milliseconds)
										{
											flag = true;
										}
									}
									else if (axisX.IntervalType == IntervalTypes.Days)
									{
										if (intervalTypes == IntervalTypes.Days || intervalTypes == IntervalTypes.Hours || intervalTypes == IntervalTypes.Minutes || intervalTypes == IntervalTypes.Seconds || intervalTypes == IntervalTypes.Milliseconds)
										{
											flag = true;
										}
									}
									else if (axisX.IntervalType == IntervalTypes.Hours)
									{
										if (intervalTypes == IntervalTypes.Minutes || intervalTypes == IntervalTypes.Seconds || intervalTypes == IntervalTypes.Milliseconds)
										{
											flag = true;
										}
									}
									else if (axisX.IntervalType == IntervalTypes.Minutes)
									{
										if (intervalTypes == IntervalTypes.Seconds || intervalTypes == IntervalTypes.Milliseconds)
										{
											flag = true;
										}
									}
									else if (axisX.IntervalType == IntervalTypes.Seconds)
									{
										if (intervalTypes == IntervalTypes.Milliseconds)
										{
											flag = true;
										}
									}
									else if (axisX.IntervalType == IntervalTypes.Milliseconds && intervalTypes == IntervalTypes.Milliseconds)
									{
										flag = true;
									}
									axisX._isDateTimeAutoInterval = true;
								}
								else
								{
									axisX.InternalInterval = axisX.Interval.Value;
								}
								if (axisX.InternalIntervalType != intervalTypes)
								{
									axisX.InternalInterval = this.UpdateTimeIterval(intervalTypes, axisX.InternalIntervalType, axisX.InternalInterval);
								}
								if (flag)
								{
									axisX.InternalIntervalType = intervalTypes;
								}
							}
							object arg_939_0 = axisX.AxisMinimum;
						}
						if (axisX.XValueType == ChartValueTypes.Time)
						{
							if (axisX.IntervalType == IntervalTypes.Auto || double.IsNaN(axisX.Interval.Value))
							{
								axisX._isDateTimeAutoInterval = true;
								axisX.InternalIntervalType = intervalTypes;
							}
							else
							{
								if (axisX.IntervalType == IntervalTypes.Years || axisX.IntervalType == IntervalTypes.Months || axisX.IntervalType == IntervalTypes.Weeks || axisX.IntervalType == IntervalTypes.Days || axisX.IntervalType == IntervalTypes.Auto)
								{
									if (intervalTypes == IntervalTypes.Years || intervalTypes == IntervalTypes.Months || intervalTypes == IntervalTypes.Weeks || intervalTypes == IntervalTypes.Days)
									{
										axisX.InternalIntervalType = IntervalTypes.Hours;
									}
									else
									{
										axisX.InternalIntervalType = intervalTypes;
									}
								}
								else if ((!axisX.Interval.HasValue || double.IsNaN(axisX.Interval.Value)) && ((axisX.InternalIntervalType == IntervalTypes.Hours && timeSpan2.Hours == 0) || (axisX.InternalIntervalType == IntervalTypes.Minutes && timeSpan2.Minutes == 0) || (axisX.InternalIntervalType == IntervalTypes.Seconds && timeSpan2.Seconds == 0)))
								{
									axisX.InternalIntervalType = intervalTypes;
								}
								if (!axisX.Interval.HasValue || double.IsNaN(axisX.Interval.Value))
								{
									if (axisX.InternalIntervalType != intervalTypes && dateTime.TimeOfDay != dateTime2.TimeOfDay)
									{
										axisX.InternalInterval = this.UpdateTimeIterval(intervalTypes, axisX.InternalIntervalType, axisX.InternalInterval);
									}
									else
									{
										axisX.InternalInterval = 1.0;
									}
									axisX._isDateTimeAutoInterval = true;
								}
								else
								{
									axisX.InternalInterval = axisX.Interval.Value;
								}
							}
							if (axisX.InternalIntervalType == IntervalTypes.Hours || axisX.InternalIntervalType == IntervalTypes.Minutes || axisX.InternalIntervalType == IntervalTypes.Seconds || axisX.InternalIntervalType == IntervalTypes.Milliseconds)
							{
								object arg_AEE_0 = axisX.AxisMinimum;
							}
						}
						if (this.ChartOrientation == ChartOrientationType.Circular && axisX.AxisMaximum == null)
						{
							axisX.InternalIntervalType = IntervalTypes.Days;
						}
						axisX.MinDate = dateTime;
						axisX.MaxDate = dateTime2;
						axisX.MinDateRange = timeSpan;
						axisX.MaxDateRange = timeSpan2;
						if (this.Chart.InternalSeries.Count == 1 && this.Chart.InternalSeries[0].InternalDataPoints.Count == 1)
						{
							axisX._isAllXValueZero = false;
							IDataPoint dataPoint = this.Chart.InternalSeries[0].InternalDataPoints[0];
							dataPoint.DpInfo.ActualNumericXValue = DateTimeHelper.DateDiff(dataPoint.DpInfo.InternalXValueAsDateTime, dateTime, timeSpan, timeSpan2, axisX.InternalIntervalType, axisX.XValueType);
							if (dataPoint.DpInfo.ActualNumericXValue == 0.0)
							{
								axisX._isAllXValueZero = true;
								dataPoint.DpInfo.ActualNumericXValue = 1.0;
								return;
							}
							return;
						}
						else
						{
							axisX._isAllXValueZero = true;
							foreach (DataSeries current3 in this.Chart.InternalSeries)
							{
								RenderAs renderAs2 = current3.RenderAs;
								foreach (IDataPoint current4 in current3.InternalDataPoints)
								{
									if (renderAs2 == RenderAs.Polar)
									{
										dateTime = DateTime.Parse("12/30/1899", CultureInfo.InvariantCulture);
										current4.DpInfo.ActualNumericXValue = DateTimeHelper.DateDiff(current4.DpInfo.InternalXValueAsDateTime, dateTime, timeSpan, timeSpan2, axisX.InternalIntervalType, axisX.XValueType);
									}
									else
									{
										current4.DpInfo.ActualNumericXValue = DateTimeHelper.DateDiff(current4.DpInfo.InternalXValueAsDateTime, dateTime, timeSpan, timeSpan2, axisX.InternalIntervalType, axisX.XValueType);
									}
									if (current4.DpInfo.ActualNumericXValue == 0.0 && axisX._isAllXValueZero)
									{
										axisX._isAllXValueZero = true;
									}
									else
									{
										axisX._isAllXValueZero = false;
									}
								}
							}
							if (!axisX._isAllXValueZero)
							{
								return;
							}
							using (List<DataSeries>.Enumerator enumerator5 = this.Chart.InternalSeries.GetEnumerator())
							{
								while (enumerator5.MoveNext())
								{
									DataSeries current5 = enumerator5.Current;
									foreach (IDataPoint current6 in current5.InternalDataPoints)
									{
										current6.DpInfo.ActualNumericXValue = 1.0;
									}
								}
								return;
							}
						}
					}
					if (axisX.AxisMinimum != null)
					{
						axisX.MinDate = DateTime.Parse(axisX.AxisMinimum.ToString());
					}
					if (axisX.AxisMaximum != null)
					{
						axisX.MaxDate = DateTime.Parse(axisX.AxisMaximum.ToString());
					}
				}
			}
		}

		private double UpdateTimeIterval(IntervalTypes autoIntervalType, IntervalTypes currentIntervalTypes, double intervalToUpdate)
		{
			if (autoIntervalType == IntervalTypes.Hours)
			{
				if (currentIntervalTypes == IntervalTypes.Minutes)
				{
					intervalToUpdate *= 60.0;
				}
				else if (currentIntervalTypes == IntervalTypes.Seconds)
				{
					intervalToUpdate = intervalToUpdate * 60.0 * 60.0;
				}
				else if (currentIntervalTypes == IntervalTypes.Milliseconds)
				{
					intervalToUpdate = intervalToUpdate * 60.0 * 60.0 * 1000.0;
				}
			}
			else if (autoIntervalType == IntervalTypes.Minutes)
			{
				if (currentIntervalTypes == IntervalTypes.Hours)
				{
					intervalToUpdate = Math.Ceiling(intervalToUpdate / 60.0);
				}
				if (currentIntervalTypes == IntervalTypes.Seconds)
				{
					intervalToUpdate = Math.Ceiling(intervalToUpdate * 60.0);
				}
				else if (currentIntervalTypes == IntervalTypes.Milliseconds)
				{
					intervalToUpdate = Math.Ceiling(intervalToUpdate * 60.0 * 1000.0);
				}
			}
			else if (autoIntervalType == IntervalTypes.Seconds)
			{
				if (currentIntervalTypes == IntervalTypes.Hours)
				{
					intervalToUpdate = Math.Ceiling(intervalToUpdate / 3600.0);
				}
				if (currentIntervalTypes == IntervalTypes.Minutes)
				{
					intervalToUpdate = Math.Ceiling(intervalToUpdate / 60.0);
				}
				else if (currentIntervalTypes == IntervalTypes.Milliseconds)
				{
					intervalToUpdate = Math.Ceiling(intervalToUpdate * 1000.0);
				}
			}
			else if (autoIntervalType == IntervalTypes.Milliseconds)
			{
				if (currentIntervalTypes == IntervalTypes.Hours)
				{
					intervalToUpdate = Math.Ceiling(intervalToUpdate / 3600000.0);
				}
				if (currentIntervalTypes == IntervalTypes.Minutes)
				{
					intervalToUpdate = Math.Ceiling(intervalToUpdate / 60000.0);
				}
				else if (currentIntervalTypes == IntervalTypes.Seconds)
				{
					intervalToUpdate = Math.Ceiling(intervalToUpdate / 1000.0);
				}
			}
			return intervalToUpdate;
		}

		private void CreateListOfDataPointsDependingOnXAxis()
		{
			this._listOfAllDataPoints = new List<IDataPoint>();
			this.ListOfAllDataPointsPrimary = new List<IDataPoint>();
			this.ListOfAllDataPointsSecondary = new List<IDataPoint>();
			foreach (DataSeries current in this.Chart.InternalSeries)
			{
				if (current.Enabled == true)
				{
					IEnumerable<IDataPoint> collection = from datapoint in current.InternalDataPoints
					select datapoint;
					this._listOfAllDataPoints.InsertRange(this._listOfAllDataPoints.Count, collection);
					if (current.AxisXType == AxisTypes.Primary)
					{
						this.ListOfAllDataPointsPrimary.InsertRange(this.ListOfAllDataPointsPrimary.Count, this._listOfAllDataPoints);
					}
					else
					{
						this.ListOfAllDataPointsSecondary.InsertRange(this.ListOfAllDataPointsSecondary.Count, this._listOfAllDataPoints);
					}
				}
			}
		}

		private void SetLabelsCountState()
		{
			if (this.AxisXPrimaryLabels.Any<KeyValuePair<double, string>>())
			{
				IEnumerable<IGrouping<double, IDataPoint>> source = from dataPoint in this.ListOfAllDataPointsPrimary
				group dataPoint by dataPoint.DpInfo.ActualNumericXValue;
				this.IsAllPrimaryAxisXLabelsPresent = (source.Any<IGrouping<double, IDataPoint>>() && this.AxisXPrimaryLabels.Count == source.Count<IGrouping<double, IDataPoint>>());
			}
			else
			{
				this.IsAllPrimaryAxisXLabelsPresent = false;
			}
			if (this.AxisXSecondaryLabels.Any<KeyValuePair<double, string>>())
			{
				IEnumerable<IGrouping<double, IDataPoint>> source2 = from dataPoint in this.ListOfAllDataPointsSecondary
				group dataPoint by dataPoint.DpInfo.ActualNumericXValue;
				this.IsAllSecondaryAxisXLabelsPresent = (source2.Any<IGrouping<double, IDataPoint>>() && this.AxisXSecondaryLabels.Count == source2.Count<IGrouping<double, IDataPoint>>());
				return;
			}
			this.IsAllSecondaryAxisXLabelsPresent = false;
		}

		private void CreateMissingAxes()
		{
			ChartOrientationType chartOrientation = this.GetChartOrientation();
			AxisOrientation axisOrientation;
			AxisOrientation axisOrientation2;
			if (chartOrientation != ChartOrientationType.Circular)
			{
				axisOrientation = ((chartOrientation == ChartOrientationType.Vertical) ? AxisOrientation.Horizontal : AxisOrientation.Vertical);
				axisOrientation2 = ((chartOrientation == ChartOrientationType.Vertical) ? AxisOrientation.Vertical : AxisOrientation.Horizontal);
			}
			else
			{
				axisOrientation = AxisOrientation.Circular;
				axisOrientation2 = AxisOrientation.Vertical;
			}
			if (this.GetCountOfSeriesUsingAxisXPrimary() > 0)
			{
				Axis axis = this.GetAxisXFromChart(this.Chart, AxisTypes.Primary);
				if (axis == null)
				{
					axis = new Axis();
					axis._isAutoGenerated = true;
					axis.Chart = this.Chart;
					axis.AxisOrientation = axisOrientation;
					axis.AxisType = AxisTypes.Primary;
					axis.PlotDetails = this;
					axis.AxisRepresentation = AxisRepresentations.AxisX;
					this.Chart.InternalAxesX.Add(axis);
					this.Chart.AxesX.Add(axis);
					this.Chart.AddAxisToChartRootElement(axis);
				}
				else
				{
					axis.AxisOrientation = axisOrientation;
					axis.AxisType = AxisTypes.Primary;
					axis.PlotDetails = this;
					axis.AxisRepresentation = AxisRepresentations.AxisX;
				}
			}
			else
			{
				for (Axis axisXFromChart = this.GetAxisXFromChart(this.Chart, AxisTypes.Primary); axisXFromChart != null; axisXFromChart = this.GetAxisXFromChart(this.Chart, AxisTypes.Primary))
				{
					this.Chart.InternalAxesX.Remove(axisXFromChart);
				}
			}
			if (this.GetCountOfSeriesUsingAxisXSecondary() > 0)
			{
				Axis axis2 = this.GetAxisXFromChart(this.Chart, AxisTypes.Secondary);
				if (axis2 == null)
				{
					axis2 = new Axis();
					axis2._isAutoGenerated = true;
					axis2.Chart = this.Chart;
					axis2.AxisOrientation = axisOrientation;
					axis2.AxisType = AxisTypes.Secondary;
					axis2.PlotDetails = this;
					axis2.AxisRepresentation = AxisRepresentations.AxisX;
					this.Chart.InternalAxesX.Add(axis2);
					this.Chart.AxesX.Add(axis2);
					this.Chart.AddAxisToChartRootElement(axis2);
				}
				else
				{
					axis2.AxisOrientation = axisOrientation;
					axis2.AxisType = AxisTypes.Secondary;
					axis2.PlotDetails = this;
					axis2.AxisRepresentation = AxisRepresentations.AxisX;
				}
			}
			else
			{
				for (Axis axisXFromChart2 = this.GetAxisXFromChart(this.Chart, AxisTypes.Secondary); axisXFromChart2 != null; axisXFromChart2 = this.GetAxisXFromChart(this.Chart, AxisTypes.Secondary))
				{
					this.Chart.InternalAxesX.Remove(axisXFromChart2);
				}
			}
			if (this.GetCountOfSeriesUsingAxisYPrimary() > 0)
			{
				Axis axis3 = this.GetAxisYFromChart(this.Chart, AxisTypes.Primary);
				if (axis3 == null)
				{
					axis3 = new Axis();
					axis3._isAutoGenerated = true;
					axis3.Chart = this.Chart;
					axis3.AxisOrientation = axisOrientation2;
					axis3.AxisType = AxisTypes.Primary;
					axis3.PlotDetails = this;
					axis3.AxisRepresentation = AxisRepresentations.AxisY;
					this.Chart.InternalAxesY.Add(axis3);
					this.Chart.AxesY.Add(axis3);
					this.Chart.AddAxisToChartRootElement(axis3);
				}
				else
				{
					axis3.AxisOrientation = axisOrientation2;
					axis3.AxisType = AxisTypes.Primary;
					axis3.PlotDetails = this;
					axis3.AxisRepresentation = AxisRepresentations.AxisY;
				}
			}
			else
			{
				for (Axis axisYFromChart = this.GetAxisYFromChart(this.Chart, AxisTypes.Primary); axisYFromChart != null; axisYFromChart = this.GetAxisYFromChart(this.Chart, AxisTypes.Primary))
				{
					this.Chart.InternalAxesY.Remove(axisYFromChart);
				}
			}
			if (this.GetCountOfSeriesUsingAxisYSecondary() <= 0)
			{
				Axis axisYFromChart2 = this.GetAxisYFromChart(this.Chart, AxisTypes.Secondary);
				if (axisYFromChart2 != null)
				{
					this.Chart.InternalAxesY.Remove(axisYFromChart2);
					axisYFromChart2 = this.GetAxisYFromChart(this.Chart, AxisTypes.Secondary);
				}
				return;
			}
			Axis axis4 = this.GetAxisYFromChart(this.Chart, AxisTypes.Secondary);
			if (axis4 == null)
			{
				axis4 = new Axis
				{
					AxisOrientation = axisOrientation2,
					AxisType = AxisTypes.Secondary
				};
				axis4._isAutoGenerated = true;
				axis4.Chart = this.Chart;
				axis4.AxisOrientation = axisOrientation2;
				axis4.AxisType = AxisTypes.Secondary;
				axis4.PlotDetails = this;
				axis4.AxisRepresentation = AxisRepresentations.AxisY;
				this.Chart.InternalAxesY.Add(axis4);
				this.Chart.AxesY.Add(axis4);
				this.Chart.AddAxisToChartRootElement(axis4);
				return;
			}
			axis4.AxisOrientation = axisOrientation2;
			axis4.AxisType = AxisTypes.Secondary;
			axis4.PlotDetails = this;
			axis4.AxisRepresentation = AxisRepresentations.AxisY;
		}

		private void CreateLegends()
		{
			foreach (Legend current in this.Chart.Legends)
			{
				current.Entries.Clear();
				current.Visual = null;
			}
			List<DataSeries> list;
			if (this.Chart.InternalSeries.Count > 1)
			{
				list = (from entry in this.Chart.InternalSeries
				where entry.Enabled == true && (entry.ShowInLegend == true || entry.IncludeDataPointsInLegend == true)
				select entry).ToList<DataSeries>();
			}
			else
			{
				list = this.Chart.InternalSeries;
			}
			if (list.Count > 0)
			{
				List<DataSeries> list2 = (from entry in list
				where !string.IsNullOrEmpty(entry.Legend)
				select entry).ToList<DataSeries>();
				if (list2.Count > 0)
				{
					bool flag = false;
					foreach (DataSeries series in list2)
					{
						IEnumerable<Legend> source = from entry in this.Chart.Legends
						where entry.GetLegendName() == entry.GetLegendName4Series(series.Legend)
						select entry;
						IEnumerable<Legend> source2 = from entry in this.Chart.Legends
						where !entry._isAutoGenerated && string.IsNullOrEmpty(entry.Name)
						select entry;
						if (!source.Any<Legend>() && !source2.Any<Legend>())
						{
							Legend legend = new Legend
							{
								_isAutoGenerated = true
							};
							legend.Chart = this.Chart;
							legend.SetValue(FrameworkElement.NameProperty, series.Legend + "_" + Guid.NewGuid().ToString().Replace('-', '_'));
							this.Chart.Legends.Add(legend);
							flag = true;
						}
					}
					if (flag)
					{
						this.Chart.AddLegendsToChartRootElement();
					}
				}
			}
		}

		private ChartOrientationType GetChartOrientation()
		{
			ChartOrientationType chartOrientationType = ChartOrientationType.Undefined;
			foreach (DataSeries current in this.Chart.InternalSeries)
			{
				if (chartOrientationType == ChartOrientationType.Undefined)
				{
					chartOrientationType = this.GetChartOrientation(current.RenderAs);
				}
				else if (chartOrientationType != this.GetChartOrientation(current.RenderAs))
				{
					throw new Exception("Invalid chart combination");
				}
			}
			return chartOrientationType;
		}

		private void PopulatePlotGroups()
		{
			if (this.PlotGroups.Count > 0)
			{
				this.PlotGroups.Clear();
			}
			this.CreateLegends();
			var enumerable = from dataSeries in this.Chart.InternalSeries
			group dataSeries by new
			{
				dataSeries.RenderAs,
				dataSeries.AxisXType,
				dataSeries.AxisYType
			};
			foreach (var current in enumerable)
			{
				ChartOrientationType chartOrientation = this.GetChartOrientation(current.Key.RenderAs);
				Axis axisXFromChart = this.GetAxisXFromChart(this.Chart, current.Key.AxisXType);
				Axis axisYFromChart = this.GetAxisYFromChart(this.Chart, current.Key.AxisYType);
				if (this.ChartOrientation == ChartOrientationType.Undefined)
				{
					this.ChartOrientation = chartOrientation;
					this.AddToPlotGroupsList(current.Key.RenderAs, axisXFromChart, axisYFromChart, current.ToList<DataSeries>(), ref this._listOfAllDataPoints);
				}
				else
				{
					if (this.ChartOrientation != chartOrientation)
					{
						throw new Exception("Invalid chart combination");
					}
					this.AddToPlotGroupsList(current.Key.RenderAs, axisXFromChart, axisYFromChart, current.ToList<DataSeries>(), ref this._listOfAllDataPoints);
				}
			}
			IEnumerable<IGrouping<RenderAs, DataSeries>> enumerable2 = from series in this.Chart.InternalSeries
			group series by series.RenderAs;
			foreach (IGrouping<RenderAs, DataSeries> current2 in enumerable2)
			{
				List<DataSeries> list = current2.ToList<DataSeries>();
				int seriesCount = list.Count;
				list.ForEach(delegate(DataSeries dataSeries)
				{
					dataSeries.SeriesCountOfSameRenderAs = seriesCount;
				});
			}
			if (this.ChartOrientation == ChartOrientationType.Horizontal)
			{
				int seriesCountByRenderAs = this.GetSeriesCountByRenderAs(RenderAs.Bar);
				int plotGroupCountByRenderAs = this.GetPlotGroupCountByRenderAs(RenderAs.StackedBar);
				int plotGroupCountByRenderAs2 = this.GetPlotGroupCountByRenderAs(RenderAs.StackedBar100);
				this.DrawingDivisionFactor = Math.Max(seriesCountByRenderAs, Math.Max(plotGroupCountByRenderAs, plotGroupCountByRenderAs2));
				return;
			}
			if (this.ChartOrientation == ChartOrientationType.Vertical)
			{
				int seriesCountByRenderAs2 = this.GetSeriesCountByRenderAs(RenderAs.Column);
				int plotGroupCountByRenderAs3 = this.GetPlotGroupCountByRenderAs(RenderAs.StackedColumn);
				int plotGroupCountByRenderAs4 = this.GetPlotGroupCountByRenderAs(RenderAs.StackedColumn100);
				int num = this.GetPlotGroupCountByRenderAs(RenderAs.Stock);
				if (num > 1)
				{
					num = 1;
				}
				int num2 = this.GetPlotGroupCountByRenderAs(RenderAs.CandleStick);
				if (num2 > 1)
				{
					num2 = 1;
				}
				num = ((num == 0) ? num2 : num);
				this.DrawingDivisionFactor = Math.Max(seriesCountByRenderAs2, Math.Max(plotGroupCountByRenderAs3, plotGroupCountByRenderAs4));
				this.DrawingDivisionFactor = Math.Max(this.DrawingDivisionFactor, num);
				return;
			}
			if (this.ChartOrientation == ChartOrientationType.NoAxis || this.ChartOrientation == ChartOrientationType.Circular)
			{
				this.DrawingDivisionFactor = 0;
			}
		}

		private void AddToPlotGroupsList(RenderAs renderAs, Axis axisX, Axis axisY, List<DataSeries> series, ref List<IDataPoint> listOfDataPointsFromAllSeries)
		{
			PlotGroup plotGroup = new PlotGroup(renderAs, axisX, axisY);
			plotGroup.DataSeriesList = series;
			plotGroup.Update(VcProperties.None, null, null);
			this.PlotGroups.Add(plotGroup);
		}

		private ChartOrientationType GetChartOrientation(RenderAs renderAs)
		{
			ChartOrientationType result;
			switch (renderAs)
			{
			case RenderAs.Column:
			case RenderAs.Line:
			case RenderAs.Area:
			case RenderAs.StackedColumn:
			case RenderAs.StackedColumn100:
			case RenderAs.StackedArea:
			case RenderAs.StackedArea100:
			case RenderAs.Bubble:
			case RenderAs.Point:
			case RenderAs.Stock:
			case RenderAs.CandleStick:
			case RenderAs.StepLine:
			case RenderAs.Spline:
			case RenderAs.QuickLine:
				result = ChartOrientationType.Vertical;
				break;
			case RenderAs.Pie:
			case RenderAs.Doughnut:
			case RenderAs.StreamLineFunnel:
			case RenderAs.SectionFunnel:
			case RenderAs.Pyramid:
				result = ChartOrientationType.NoAxis;
				break;
			case RenderAs.Bar:
			case RenderAs.StackedBar:
			case RenderAs.StackedBar100:
				result = ChartOrientationType.Horizontal;
				break;
			case RenderAs.Radar:
			case RenderAs.Polar:
				result = ChartOrientationType.Circular;
				break;
			default:
				result = ChartOrientationType.Undefined;
				break;
			}
			return result;
		}

		internal Axis GetAxisXFromChart(Chart chart, AxisTypes axisType)
		{
			IEnumerable<Axis> source = from axis in chart.InternalAxesX
			where axis.AxisType == axisType
			select axis;
			if (source.Any<Axis>())
			{
				return source.Last<Axis>();
			}
			return null;
		}

		internal Axis GetAxisYFromChart(Chart chart, AxisTypes axisType)
		{
			IEnumerable<Axis> source = from axis in chart.InternalAxesY
			where axis.AxisType == axisType
			select axis;
			if (source.Any<Axis>())
			{
				return source.Last<Axis>();
			}
			return null;
		}

		private void SetIncrementalZIndexForSeries()
		{
			int num = 0;
			foreach (DataSeries current in this.Chart.InternalSeries)
			{
				current.InternalZIndex = num++;
			}
		}

		private Dictionary<DataSeries, int> GenerateDrawingOrder()
		{
			this.SetIncrementalZIndexForSeries();
			int num = 0;
			num += ((this.GetSeriesCountByRenderAs(RenderAs.Column) > 0) ? 1 : 0);
			num += ((this.GetSeriesCountByRenderAs(RenderAs.StackedColumn) > 0) ? 1 : 0);
			num += ((this.GetSeriesCountByRenderAs(RenderAs.StackedColumn100) > 0) ? 1 : 0);
			num += ((this.GetSeriesCountByRenderAs(RenderAs.Bar) > 0) ? 1 : 0);
			num += ((this.GetSeriesCountByRenderAs(RenderAs.StackedBar) > 0) ? 1 : 0);
			num += ((this.GetSeriesCountByRenderAs(RenderAs.StackedBar100) > 0) ? 1 : 0);
			num += this.GetSeriesCountByRenderAs(RenderAs.Area);
			num += this.GetPlotGroupCountByRenderAs(RenderAs.StackedArea);
			num += this.GetPlotGroupCountByRenderAs(RenderAs.StackedArea100);
			this.Layer3DCount = num;
			Func<IGrouping<DataSeries, int>, DataSeries> keySelector = (IGrouping<DataSeries, int> entry) => entry.Key;
			Func<IGrouping<DataSeries, int>, int> elementSelector = (IGrouping<DataSeries, int> entry) => entry.Last<int>();
			Dictionary<DataSeries, int> dictionary = (from series in this.Chart.InternalSeries
			orderby series.InternalZIndex
			group series.InternalZIndex by series).ToDictionary(keySelector, elementSelector);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.Column, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StackedColumn, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StackedColumn100, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.Bar, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StackedBar, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StackedBar100, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.Line, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StepLine, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.Spline, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.QuickLine, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.Point, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.Stock, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.CandleStick, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.Bubble, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StackedArea, AxisTypes.Primary, AxisTypes.Primary, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StackedArea, AxisTypes.Primary, AxisTypes.Secondary, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StackedArea, AxisTypes.Secondary, AxisTypes.Primary, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StackedArea, AxisTypes.Secondary, AxisTypes.Secondary, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StackedArea100, AxisTypes.Primary, AxisTypes.Primary, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StackedArea100, AxisTypes.Primary, AxisTypes.Secondary, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StackedArea100, AxisTypes.Secondary, AxisTypes.Primary, dictionary);
			dictionary = this.GenerateIndexByRenderAs(RenderAs.StackedArea100, AxisTypes.Secondary, AxisTypes.Secondary, dictionary);
			List<KeyValuePair<DataSeries, int>> list = (from entry in dictionary
			orderby entry.Value
			select entry).ToList<KeyValuePair<DataSeries, int>>();
			List<KeyValuePair<DataSeries, int>> list2 = new List<KeyValuePair<DataSeries, int>>();
			List<KeyValuePair<DataSeries, int>> list3 = new List<KeyValuePair<DataSeries, int>>();
			int drawingIndex = 0;
			RenderAs[] ignorableChartsFixedAtDrawingIndex0 = new RenderAs[]
			{
				RenderAs.Line,
				RenderAs.StepLine,
				RenderAs.QuickLine,
				RenderAs.Spline,
				RenderAs.Point,
				RenderAs.Bubble,
				RenderAs.Stock,
				RenderAs.CandleStick
			};
			int lowestIndex;
			while (list.Count > 0)
			{
				lowestIndex = (from series in list
				select series.Value).Min();
				IEnumerable<DataSeries> source = from series in list
				where series.Value == lowestIndex
				select series.Key;
				list3.InsertRange(list3.Count, (from series in source
				select new KeyValuePair<DataSeries, int>(series, drawingIndex)).ToList<KeyValuePair<DataSeries, int>>());
				bool flag = (from series in source
				where ignorableChartsFixedAtDrawingIndex0.Contains(series.RenderAs)
				select series.RenderAs).Any<RenderAs>();
				IEnumerable<KeyValuePair<DataSeries, int>> source2 = from series in list
				where series.Value != lowestIndex
				select series;
				list2 = source2.ToList<KeyValuePair<DataSeries, int>>();
				list.Clear();
				list = list2;
				if (!flag)
				{
					drawingIndex++;
				}
			}
			Func<KeyValuePair<DataSeries, int>, DataSeries> keySelector2 = (KeyValuePair<DataSeries, int> entry) => entry.Key;
			Func<KeyValuePair<DataSeries, int>, int> elementSelector2 = (KeyValuePair<DataSeries, int> entry) => entry.Value;
			return list3.ToDictionary(keySelector2, elementSelector2);
		}

		private Dictionary<DataSeries, int> GenerateIndexByRenderAs(RenderAs renderAs, Dictionary<DataSeries, int> seriesIndexDictionary)
		{
			IEnumerable<KeyValuePair<DataSeries, int>> source = from entry in seriesIndexDictionary
			where entry.Key.RenderAs == renderAs
			select entry;
			if (source.Any<KeyValuePair<DataSeries, int>>())
			{
				int value = (from entry in source
				select entry.Value).Max();
				KeyValuePair<DataSeries, int>[] array = source.ToArray<KeyValuePair<DataSeries, int>>();
				for (int i = 0; i < array.Length; i++)
				{
					seriesIndexDictionary[array[i].Key] = value;
				}
			}
			return seriesIndexDictionary;
		}

		private Dictionary<DataSeries, int> GenerateIndexByRenderAs(RenderAs renderAs, AxisTypes axisXType, AxisTypes axisYType, Dictionary<DataSeries, int> seriesIndexDictionary)
		{
			IEnumerable<KeyValuePair<DataSeries, int>> source = from entry in seriesIndexDictionary
			where entry.Key.RenderAs == renderAs && entry.Key.AxisXType == axisXType && entry.Key.AxisYType == axisYType
			select entry;
			if (source.Any<KeyValuePair<DataSeries, int>>())
			{
				int value = (from entry in source
				select entry.Value).Max();
				KeyValuePair<DataSeries, int>[] array = source.ToArray<KeyValuePair<DataSeries, int>>();
				for (int i = 0; i < array.Length; i++)
				{
					seriesIndexDictionary[array[i].Key] = value;
				}
			}
			return seriesIndexDictionary;
		}

		internal int GetSeriesCountByRenderAs(RenderAs renderAs)
		{
			IEnumerable<DataSeries> source = from series in this.Chart.InternalSeries
			where series.RenderAs == renderAs
			select series;
			if (!source.Any<DataSeries>())
			{
				return 0;
			}
			return source.Count<DataSeries>();
		}

		internal int GetPlotGroupCountByRenderAs(RenderAs renderAs)
		{
			IEnumerable<PlotGroup> source = from plotGroup in this.PlotGroups
			where plotGroup.RenderAs == renderAs && plotGroup.IsEnabled
			select plotGroup;
			if (!source.Any<PlotGroup>())
			{
				return 0;
			}
			return source.Count<PlotGroup>();
		}

		private Dictionary<double, string> GetAxisXLabels4CircularAxisType()
		{
			List<IDataPoint> source = (from dataPoint in this._listOfAllDataPoints
			select dataPoint).ToList<IDataPoint>();
			IEnumerable<IGrouping<double, string>> source2 = from dataPoint in source
			where !string.IsNullOrEmpty(dataPoint.AxisXLabel)
			orderby dataPoint.DpInfo.ActualNumericXValue
			group dataPoint.AxisXLabel by dataPoint.DpInfo.ActualNumericXValue;
			Func<IGrouping<double, string>, double> keySelector = (IGrouping<double, string> entry) => entry.Key;
			Func<IGrouping<double, string>, string> elementSelector = (IGrouping<double, string> entry) => entry.Last<string>();
			return source2.ToDictionary(keySelector, elementSelector);
		}

		private Dictionary<double, string> GetAxisXLabels(AxisTypes axisXType)
		{
			if (axisXType == AxisTypes.Secondary)
			{
				return new Dictionary<double, string>();
			}
			IEnumerable<IGrouping<double, string>> source = from dataPoint in this._listOfAllDataPoints
			where !string.IsNullOrEmpty(dataPoint.AxisXLabel)
			orderby dataPoint.DpInfo.ActualNumericXValue
			group dataPoint.AxisXLabel by dataPoint.DpInfo.ActualNumericXValue;
			Func<IGrouping<double, string>, double> keySelector = (IGrouping<double, string> entry) => entry.Key;
			Func<IGrouping<double, string>, string> elementSelector = (IGrouping<double, string> entry) => entry.Last<string>();
			return source.ToDictionary(keySelector, elementSelector);
		}

		private bool ValidateChartCombination(params RenderAs[] renderTypes)
		{
			ChartOrientationType chartOrientationType = ChartOrientationType.Undefined;
			for (int i = 0; i < renderTypes.Length; i++)
			{
				RenderAs renderAs = renderTypes[i];
				if (chartOrientationType == ChartOrientationType.Undefined)
				{
					chartOrientationType = this.GetChartOrientation(renderAs);
				}
				else if (chartOrientationType != this.GetChartOrientation(renderAs))
				{
					return false;
				}
			}
			return true;
		}

		internal double GetAxisXMaximumDataValue(Axis axisX)
		{
			IEnumerable<double> source = from plotData in this.PlotGroups
			where !double.IsNaN(plotData.MaximumX) && plotData.AxisX == axisX
			select plotData.MaximumX;
			if (source.Any<double>())
			{
				return source.Max();
			}
			return 0.0;
		}

		internal double GetAxisYMaximumDataValue(Axis axisY)
		{
			Axis axisXFromChart = this.GetAxisXFromChart(this.Chart, AxisTypes.Primary);
			double num = double.NegativeInfinity;
			if ((this.Chart.ScrollingEnabled.Value || this.Chart.ZoomingEnabled) && this.ListOfAllDataPoints.Count > 0 && axisY.ViewportRangeEnabled && axisXFromChart.ViewMinimum != null && axisXFromChart.ViewMaximum != null)
			{
				using (List<PlotGroup>.Enumerator enumerator = this.PlotGroups.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PlotGroup current = enumerator.Current;
						if (axisY == current.AxisY)
						{
							double num2;
							current.CalculateMaxYValueWithInAXValueRange(axisXFromChart._numericViewMinimum, axisXFromChart._numericViewMaximum, out num2);
							if (!double.IsNaN(num2))
							{
								num = Math.Max(num, num2);
							}
						}
					}
					goto IL_137;
				}
			}
			IEnumerable<double> source = from plotData in this.PlotGroups
			where plotData.AxisY == axisY
			select plotData.MaximumY;
			if (source.Any<double>())
			{
				num = source.Max();
			}
			IL_137:
			num = ((double.IsNaN(num) || double.IsInfinity(num)) ? (axisY.Logarithmic ? axisY.LogarithmBase : 1.0) : num);
			return num;
		}

		internal double GetAxisXMinimumDataValue(Axis axisX)
		{
			double num = double.PositiveInfinity;
			foreach (PlotGroup current in this.PlotGroups)
			{
				IEnumerable<DataSeries> source = from dataSeries in current.DataSeriesList
				where dataSeries.DataPoints != null && dataSeries.DataPoints.Count > 0
				select dataSeries;
				if (source.Any<DataSeries>() && !double.IsNaN(current.MinimumX) && current.AxisX == axisX)
				{
					num = Math.Min(num, current.MinimumX);
				}
			}
			if (!double.IsPositiveInfinity(num))
			{
				return num;
			}
			return 0.0;
		}

		internal double GetAxisYMinimumDataValue(Axis axisY)
		{
			double num = double.PositiveInfinity;
			Axis axisXFromChart = this.GetAxisXFromChart(this.Chart, AxisTypes.Primary);
			if ((this.Chart.ScrollingEnabled.Value || this.Chart.ZoomingEnabled) && this.ListOfAllDataPoints.Count > 0 && axisY.ViewportRangeEnabled && axisXFromChart.ViewMinimum != null && axisXFromChart.ViewMaximum != null)
			{
				using (List<PlotGroup>.Enumerator enumerator = this.PlotGroups.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PlotGroup current = enumerator.Current;
						if (axisY == current.AxisY)
						{
							double num2;
							current.CalculateMinYValueWithInAXValueRange(axisXFromChart._numericViewMinimum, axisXFromChart._numericViewMaximum, out num2);
							if (!double.IsNaN(num2))
							{
								num = Math.Min(num, num2);
							}
						}
					}
					goto IL_14F;
				}
			}
			foreach (PlotGroup current2 in this.PlotGroups)
			{
				IEnumerable<DataSeries> source = from dataSeries in current2.DataSeriesList
				where dataSeries.DataPoints != null && dataSeries.DataPoints.Count > 0
				select dataSeries;
				if (source.Any<DataSeries>() && !double.IsNaN(current2.MinimumY) && current2.AxisY == axisY)
				{
					num = Math.Min(num, current2.MinimumY);
				}
			}
			IL_14F:
			if (!double.IsInfinity(num) && !double.IsNaN(num))
			{
				return num;
			}
			return 0.0;
		}

		internal double GetAxisXMinimumDataValueFromAllDataSeries(Axis axisX)
		{
			double num = double.PositiveInfinity;
			IEnumerable<double> enumerable = from dp in this.ListOfAllDataPoints
			where dp.Enabled == true && axisX.AxisType == dp.Parent.AxisXType
			select dp.DpInfo.ActualNumericXValue;
			if (enumerable != null && enumerable.Any<double>())
			{
				num = Math.Min(num, enumerable.Min());
			}
			return num;
		}

		internal double GetAxisYMinimumDataValueFromAllDataSeries(Axis axisY)
		{
			double num = double.PositiveInfinity;
			List<IDataPoint> source = (from dp in this.ListOfAllDataPoints
			where dp.Enabled == true
			select dp).ToList<IDataPoint>();
			IEnumerable<double> source2 = from dp in source
			where axisY.AxisType == dp.Parent.AxisYType && dp.Parent.RenderAs != RenderAs.CandleStick && dp.Parent.RenderAs != RenderAs.Stock && !double.IsNaN(dp.YValue)
			select dp.YValue;
			if (source2.Any<double>())
			{
				num = Math.Min(num, source2.Min());
			}
			IEnumerable<double[]> source3 = from dp in source
			where axisY.AxisType == dp.Parent.AxisYType && (dp.Parent.RenderAs == RenderAs.CandleStick || dp.Parent.RenderAs == RenderAs.Stock) && dp.YValues != null
			select dp.YValues;
			if (source3.Any<double[]>())
			{
				IEnumerable<double> source4 = from yValue in source3
				select yValue.Min();
				if (source4.Any<double>())
				{
					num = Math.Min(num, source4.Min());
				}
			}
			return num;
		}

		internal bool CheckIfAnyDataPointVisualExistsInChart()
		{
			if (this.Chart != null && this.Chart.ChartArea != null && this.Chart.ChartArea._isDefaultSeriesSet)
			{
				return false;
			}
			double num = -1.7976931348623157E+308;
			foreach (PlotGroup current in this.PlotGroups)
			{
				foreach (DataSeries ds in current.DataSeriesList)
				{
					if (ds.DataPoints != null)
					{
						IEnumerable<IDataPoint> source = from dp in ds.DataPoints
						where dp.Enabled.Value && (((ds.RenderAs == RenderAs.CandleStick || ds.RenderAs == RenderAs.Stock) && dp.YValues != null) || (ds.RenderAs != RenderAs.CandleStick && ds.RenderAs != RenderAs.Stock && !double.IsNaN(dp.YValue)))
						select dp;
						if (source.Any<IDataPoint>())
						{
							num = Math.Max(num, (double)source.Count<IDataPoint>());
						}
					}
				}
			}
			return num > 0.0;
		}

		internal double GetMaximumZValue()
		{
			return (from plotData in this.PlotGroups
			where !double.IsNaN(plotData.MinimumZ)
			select plotData.MaximumZ).Max();
		}

		internal double GetMinimumZValue()
		{
			return (from plotData in this.PlotGroups
			where !double.IsNaN(plotData.MinimumZ)
			select plotData.MinimumZ).Min();
		}

		internal double GetMaxOfMinDifferencesForXValue()
		{
			double num = double.NegativeInfinity;
			foreach (PlotGroup current in this.PlotGroups)
			{
				if (current.MinimumX != current.MaximumX)
				{
					IEnumerable<DataSeries> enumerable = from dataSeries in current.DataSeriesList
					where dataSeries.DataPoints != null && dataSeries.DataPoints.Count > 0
					select dataSeries;
					if (enumerable != null && enumerable.Any<DataSeries>())
					{
						IEnumerable<PlotGroup> enumerable2 = from d in enumerable
						select d.PlotGroup;
						if (enumerable2 != null && enumerable2.Any<PlotGroup>())
						{
							double num2 = (from plotData in enumerable2
							where !double.IsNaN(plotData.MinDifferenceX)
							select plotData.MinDifferenceX).Max();
							if (num2 > num)
							{
								num = num2;
							}
						}
					}
				}
			}
			if (!double.IsNegativeInfinity(num))
			{
				return num;
			}
			return double.PositiveInfinity;
		}

		internal double GetMaxOfMinDifferencesForXValue(RenderAs renderAs)
		{
			return (from plotData in this.PlotGroups
			where !double.IsNaN(plotData.MinDifferenceX) && plotData.RenderAs == renderAs
			select plotData.MinDifferenceX).Max();
		}

		internal double GetMinOfMinDifferencesForXValue()
		{
			return (from plotData in this.PlotGroups
			where !double.IsNaN(plotData.MinDifferenceX)
			select plotData.MinDifferenceX).Min();
		}

		internal double GetMinOfMinDifferencesForXValue(RenderAs renderAs)
		{
			return (from plotData in this.PlotGroups
			where !double.IsNaN(plotData.MinDifferenceX) && plotData.RenderAs == renderAs
			select plotData.MinDifferenceX).Min();
		}

		internal double GetMinOfMinDifferencesForXValue(params RenderAs[] renderAs)
		{
			return (from plotData in this.PlotGroups
			where !double.IsNaN(plotData.MinDifferenceX) && renderAs.Contains(plotData.RenderAs)
			select plotData.MinDifferenceX).Min();
		}

		internal List<DataSeries> GetSeriesListByRenderAs(RenderAs renderAs)
		{
			return (from series in this.Chart.InternalSeries
			where series.Enabled == true && series.RenderAs == renderAs
			select series).ToList<DataSeries>();
		}

		internal void SetTrendLineValue(TrendLine trendLine, Axis axisX)
		{
			if (axisX != null && axisX.IsDateTimeAxis && trendLine.Enabled.Value && ((trendLine.Orientation == Orientation.Vertical && axisX.PlotDetails.ChartOrientation == ChartOrientationType.Vertical) || (trendLine.Orientation == Orientation.Horizontal && axisX.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)))
			{
				if (axisX.XValueType == ChartValueTypes.Time)
				{
					DateTime dateTime = DateTime.Parse("12/30/1899 " + trendLine.InternalDateValue.TimeOfDay.ToString(), CultureInfo.InvariantCulture);
					trendLine.InternalNumericValue = DateTimeHelper.DateDiff(dateTime, axisX.MinDate, axisX.MinDateRange, axisX.MaxDateRange, axisX.InternalIntervalType, axisX.XValueType);
					return;
				}
				trendLine.InternalNumericValue = DateTimeHelper.DateDiff(trendLine.InternalDateValue, axisX.MinDate, axisX.MinDateRange, axisX.MaxDateRange, axisX.InternalIntervalType, axisX.XValueType);
			}
		}

		internal void SetTrendLineStartAndEndValue(TrendLine trendLine, Axis axisX)
		{
			if (axisX != null && axisX.IsDateTimeAxis && trendLine.Enabled.Value && ((trendLine.Orientation == Orientation.Vertical && axisX.PlotDetails.ChartOrientation == ChartOrientationType.Vertical) || (trendLine.Orientation == Orientation.Horizontal && axisX.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)))
			{
				DateTime arg_5D_0 = trendLine.InternalDateStartValue;
				DateTime arg_64_0 = trendLine.InternalDateEndValue;
				if (axisX.XValueType == ChartValueTypes.Time)
				{
					DateTime dateTime = DateTime.Parse("12/30/1899 " + trendLine.InternalDateStartValue.TimeOfDay.ToString(), CultureInfo.InvariantCulture);
					trendLine.InternalNumericStartValue = DateTimeHelper.DateDiff(dateTime, axisX.MinDate, axisX.MinDateRange, axisX.MaxDateRange, axisX.InternalIntervalType, axisX.XValueType);
					DateTime dateTime2 = DateTime.Parse("12/30/1899 " + trendLine.InternalDateEndValue.TimeOfDay.ToString(), CultureInfo.InvariantCulture);
					trendLine.InternalNumericEndValue = DateTimeHelper.DateDiff(dateTime2, axisX.MinDate, axisX.MinDateRange, axisX.MaxDateRange, axisX.InternalIntervalType, axisX.XValueType);
				}
				trendLine.InternalNumericStartValue = DateTimeHelper.DateDiff(trendLine.InternalDateStartValue, axisX.MinDate, axisX.MinDateRange, axisX.MaxDateRange, axisX.InternalIntervalType, axisX.XValueType);
				trendLine.InternalNumericEndValue = DateTimeHelper.DateDiff(trendLine.InternalDateEndValue, axisX.MinDate, axisX.MinDateRange, axisX.MaxDateRange, axisX.InternalIntervalType, axisX.XValueType);
			}
		}

		internal int GetCountOfSeriesUsingAxisXSecondary()
		{
			IEnumerable<DataSeries> source = from series in this.Chart.InternalSeries
			where series.AxisXType == AxisTypes.Secondary
			select series;
			if (!source.Any<DataSeries>())
			{
				return 0;
			}
			return source.Count<DataSeries>();
		}

		internal int GetCountOfSeriesUsingAxisXPrimary()
		{
			IEnumerable<DataSeries> source = from series in this.Chart.InternalSeries
			where series.AxisXType == AxisTypes.Primary
			select series;
			if (!source.Any<DataSeries>())
			{
				return 0;
			}
			return source.Count<DataSeries>();
		}

		internal int GetCountOfSeriesUsingAxisYSecondary()
		{
			IEnumerable<DataSeries> source = from series in this.Chart.InternalSeries
			where series.AxisYType == AxisTypes.Secondary
			select series;
			if (!source.Any<DataSeries>())
			{
				return 0;
			}
			return source.Count<DataSeries>();
		}

		internal int GetCountOfSeriesUsingAxisYPrimary()
		{
			return (from series in this.Chart.InternalSeries
			where series.AxisYType == AxisTypes.Primary
			select series).Count<DataSeries>();
		}

		internal bool GetStacked100OverrideState()
		{
			RenderAs[] stacked100Types = new RenderAs[]
			{
				RenderAs.StackedArea100,
				RenderAs.StackedBar100,
				RenderAs.StackedColumn100
			};
			int num = (from series in this.Chart.InternalSeries
			where stacked100Types.Contains(series.RenderAs)
			select series).Count<DataSeries>();
			return num == this.Chart.InternalSeries.Count;
		}

		internal Dictionary<double, SortDataPoints> GetDataPointsGroupedByXValue(RenderAs renderAs)
		{
			List<PlotGroup> list = (from plotGroup in this.PlotGroups
			where plotGroup.RenderAs == renderAs
			select plotGroup).ToList<PlotGroup>();
			List<IDataPoint> list2 = new List<IDataPoint>();
			foreach (PlotGroup current in list)
			{
				foreach (DataSeries current2 in current.DataSeriesList)
				{
					if (current2.Enabled == true)
					{
						list2.InsertRange(list2.Count, current2.InternalDataPoints);
					}
				}
			}
			IEnumerable<IGrouping<double, IDataPoint>> enumerable = from datapoint in list2
			group datapoint by datapoint.DpInfo.InternalXValue;
			Dictionary<double, SortDataPoints> dictionary = new Dictionary<double, SortDataPoints>();
			foreach (IGrouping<double, IDataPoint> current3 in enumerable)
			{
				List<IDataPoint> list3 = new List<IDataPoint>();
				List<IDataPoint> list4 = new List<IDataPoint>();
				foreach (IDataPoint current4 in current3)
				{
					if (current4.DpInfo.InternalYValue > 0.0)
					{
						list3.Add(current4);
					}
					else
					{
						list4.Add(current4);
					}
				}
				dictionary.Add(current3.Key, new SortDataPoints(list3, list4));
			}
			return dictionary;
		}

		internal Dictionary<double, SortDataPoints> GetDataPointsGroupedByXValue(RenderAs renderAs, Axis axisX, Axis axisY)
		{
			List<PlotGroup> list = (from plotGroup in this.PlotGroups
			where plotGroup.RenderAs == renderAs && plotGroup.AxisX == axisX && plotGroup.AxisY == axisY
			select plotGroup).ToList<PlotGroup>();
			List<IDataPoint> list2 = new List<IDataPoint>();
			foreach (PlotGroup current in list)
			{
				foreach (DataSeries current2 in current.DataSeriesList)
				{
					if (current2.Enabled == true)
					{
						List<IDataPoint> collection = (from datapoint in current2.InternalDataPoints
						where datapoint.Enabled == true
						select datapoint).ToList<IDataPoint>();
						list2.InsertRange(list2.Count, collection);
					}
				}
			}
			IEnumerable<IGrouping<double, IDataPoint>> enumerable = from datapoint in list2
			group datapoint by datapoint.DpInfo.ActualNumericXValue;
			Dictionary<double, SortDataPoints> dictionary = new Dictionary<double, SortDataPoints>();
			foreach (IGrouping<double, IDataPoint> current3 in enumerable)
			{
				List<IDataPoint> positive = (from data in current3
				where data.DpInfo.InternalYValue > 0.0
				select data).ToList<IDataPoint>();
				List<IDataPoint> negative = (from data in current3
				where data.DpInfo.InternalYValue <= 0.0
				select data).ToList<IDataPoint>();
				dictionary.Add(current3.Key, new SortDataPoints(positive, negative));
			}
			return dictionary;
		}

		internal Axis GetAxisXMinimumInterval()
		{
			double minInterval = (from axis in this.Chart.InternalAxesX
			select axis.InternalInterval).Min();
			return (from axis in this.Chart.InternalAxesX
			where axis.InternalInterval == minInterval
			select axis).First<Axis>();
		}

		internal int GetDivisionFactor(SortDataPoints sortedSet)
		{
			List<IDataPoint> list = new List<IDataPoint>();
			list.InsertRange(list.Count, sortedSet.Positive);
			list.InsertRange(list.Count, sortedSet.Negative);
			return (from entry in list
			group entry by entry.Parent).Count<IGrouping<DataSeries, IDataPoint>>();
		}

		internal int GetMaxDataPointsCountFromInternalSeriesList(List<DataSeries> seriesList)
		{
			IEnumerable<int> source = from ds in seriesList
			select ds.DataPoints.Count;
			if (!source.Any<int>())
			{
				return 0;
			}
			return source.Max();
		}

		internal List<DataSeries> GetSeriesFromSortedPoints(SortDataPoints sortedSet)
		{
			List<IDataPoint> list = new List<IDataPoint>();
			list.InsertRange(list.Count, sortedSet.Positive);
			list.InsertRange(list.Count, sortedSet.Negative);
			return (from entry in list
			select entry.Parent).Distinct<DataSeries>().ToList<DataSeries>();
		}

		internal List<DataSeries> GetSeriesFromDataPoint(IDataPoint dataPoint)
		{
			List<DataSeries> list = new List<DataSeries>();
			foreach (DataSeries current in this.Chart.InternalSeries)
			{
				if (current.RenderAs == dataPoint.Parent.RenderAs && current.Enabled == true && current.InternalDataPoints.Count > 0)
				{
					list.Add(current);
				}
			}
			return list;
		}

		internal int GetMaxDivision(Dictionary<double, SortDataPoints> sortedDataPointList)
		{
			List<double> list = sortedDataPointList.Keys.ToList<double>();
			int num = 0;
			foreach (double key in list)
			{
				num = Math.Max(num, this.GetDivisionFactor(sortedDataPointList[key]));
			}
			return num;
		}

		internal double GetAbsoluteSumOfDataPoints(List<IDataPoint> dataPoints)
		{
			if (dataPoints.Count > 0 && (dataPoints[0].Parent.RenderAs == RenderAs.SectionFunnel || dataPoints[0].Parent.RenderAs == RenderAs.StreamLineFunnel || dataPoints[0].Parent.RenderAs == RenderAs.Pyramid))
			{
				return (from dataPoint in dataPoints
				where !double.IsNaN(dataPoint.YValue) && dataPoint.YValue >= 0.0
				select Math.Abs(dataPoint.YValue)).Sum();
			}
			if (dataPoints.Count <= 0 || (dataPoints[0].Parent.RenderAs != RenderAs.CandleStick && dataPoints[0].Parent.RenderAs != RenderAs.Stock))
			{
				return (from dataPoint in dataPoints
				where !double.IsNaN(dataPoint.YValue)
				select Math.Abs(dataPoint.YValue)).Sum();
			}
			IEnumerable<double> source = from dataPoint in dataPoints
			where dataPoint.YValues != null && dataPoint.YValues.Length > 1
			select Math.Abs(dataPoint.YValues[1]);
			if (source.Any<double>())
			{
				return source.Sum();
			}
			return 0.0;
		}

		internal Dictionary<double, List<double>> GetDataPointValuesInStackedOrder4StackedArea(PlotGroup plotGroup)
		{
			double[] array = plotGroup.XWiseStackedDataList.Keys.ToArray<double>();
			Array.Sort<double>(array);
			Dictionary<double, List<double>> dictionary = new Dictionary<double, List<double>>();
			for (int i = 0; i < array.Length; i++)
			{
				IEnumerable<IGrouping<DataSeries, double>> enumerable = from entry in plotGroup.XWiseStackedDataList[array[i]].Positive
				group AreaChart.GetInternalYValue4StackedArea(entry) by entry.Parent;
				IEnumerable<IGrouping<DataSeries, double>> enumerable2 = from entry in plotGroup.XWiseStackedDataList[array[i]].Negative
				group AreaChart.GetInternalYValue4StackedArea(entry) by entry.Parent;
				double[] array2 = new double[enumerable.Count<IGrouping<DataSeries, double>>() + enumerable2.Count<IGrouping<DataSeries, double>>()];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = 0.0;
				}
				foreach (IGrouping<DataSeries, double> current in enumerable)
				{
					int num = plotGroup.DataSeriesList.IndexOf(current.Key);
					if (num < array2.Count<double>())
					{
						array2[num] += current.Sum();
					}
				}
				foreach (IGrouping<DataSeries, double> current2 in enumerable2)
				{
					int num2 = plotGroup.DataSeriesList.IndexOf(current2.Key);
					if (num2 < array2.Count<double>())
					{
						array2[num2] += current2.Sum();
					}
				}
				dictionary.Add(array[i], array2.ToList<double>());
			}
			return dictionary;
		}

		internal Dictionary<double, List<IDataPoint>> GetDataPointInStackOrder4StackedArea(PlotGroup plotGroup)
		{
			double[] array = plotGroup.XWiseStackedDataList.Keys.ToArray<double>();
			Array.Sort<double>(array);
			Dictionary<double, List<IDataPoint>> dictionary = new Dictionary<double, List<IDataPoint>>();
			for (int i = 0; i < array.Length; i++)
			{
				IEnumerable<IGrouping<DataSeries, IDataPoint>> enumerable = from entry in plotGroup.XWiseStackedDataList[array[i]].Positive
				group entry by entry.Parent;
				IEnumerable<IGrouping<DataSeries, IDataPoint>> enumerable2 = from entry in plotGroup.XWiseStackedDataList[array[i]].Negative
				group entry by entry.Parent;
				IDataPoint[] array2 = new IDataPoint[enumerable.Count<IGrouping<DataSeries, IDataPoint>>() + enumerable2.Count<IGrouping<DataSeries, IDataPoint>>()];
				foreach (IGrouping<DataSeries, IDataPoint> current in enumerable)
				{
					int num = plotGroup.DataSeriesList.IndexOf(current.Key);
					if (num < array2.Count<IDataPoint>())
					{
						array2[num] = current.First<IDataPoint>();
					}
				}
				foreach (IGrouping<DataSeries, IDataPoint> current2 in enumerable2)
				{
					int num2 = plotGroup.DataSeriesList.IndexOf(current2.Key);
					if (num2 < array2.Count<IDataPoint>())
					{
						array2[num2] = current2.First<IDataPoint>();
					}
				}
				dictionary.Add(array[i], array2.ToList<IDataPoint>());
			}
			return dictionary;
		}

		internal void ClearInstanceRefs()
		{
			if (this._listOfAllDataPoints != null)
			{
				this._listOfAllDataPoints.Clear();
			}
			this._listOfAllDataPoints = null;
			if (this.AxisXPrimaryLabels != null)
			{
				this.AxisXPrimaryLabels.Clear();
			}
			this.AxisXPrimaryLabels = null;
			if (this.AxisXSecondaryLabels != null)
			{
				this.AxisXSecondaryLabels.Clear();
			}
			this.AxisXSecondaryLabels = null;
			if (this.ListOfAllDataPointsPrimary != null)
			{
				this.ListOfAllDataPointsPrimary.Clear();
			}
			this.ListOfAllDataPointsPrimary = null;
			if (this.ListOfAllDataPointsSecondary != null)
			{
				this.ListOfAllDataPointsSecondary.Clear();
			}
			this.ListOfAllDataPointsSecondary = null;
			foreach (PlotGroup current in this.PlotGroups)
			{
				current.ClearInstanceRefs();
			}
			this.PlotGroups.Clear();
		}
	}
}
