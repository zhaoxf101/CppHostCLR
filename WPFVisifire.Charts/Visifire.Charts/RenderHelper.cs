using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using Visifire.Commons;

namespace Visifire.Charts
{
	public static class RenderHelper
	{
		internal static void ResetMarkersForSeries(List<DataSeries> dataSeriesList4Rendering)
		{
			foreach (DataSeries current in dataSeriesList4Rendering)
			{
				if (current.RenderAs != RenderAs.QuickLine && current.DataPoints != null)
				{
					foreach (IDataPoint current2 in current.DataPoints)
					{
						current2.DpInfo.Marker = null;
					}
				}
			}
		}

		internal static void UpdateParentVisualCanvasSize(Chart chart, Canvas canvas)
		{
			if (canvas != null)
			{
				canvas.Width = chart.ChartArea.ChartVisualCanvas.Width;
				canvas.Height = chart.ChartArea.ChartVisualCanvas.Height;
			}
		}

		internal static Panel GetVisualObject(Panel preExistingPanel, RenderAs chartType, double width, double height, PlotDetails plotDetails, List<DataSeries> dataSeriesList4Rendering, Chart chart, double plankDepth, bool animationEnabled)
		{
			Panel result = null;
			RenderHelper.ResetMarkersForSeries(dataSeriesList4Rendering);
			switch (chartType)
			{
			case RenderAs.Column:
				result = ColumnChart.GetVisualObjectForColumnChart(preExistingPanel, width, height, plotDetails, dataSeriesList4Rendering, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.Line:
			case RenderAs.Spline:
				result = LineChart.GetVisualObjectForLineChart(preExistingPanel, width, height, plotDetails, dataSeriesList4Rendering, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.Pie:
				result = PieChart.GetVisualObjectForPieChart(width, height, plotDetails, dataSeriesList4Rendering, chart, animationEnabled);
				break;
			case RenderAs.Bar:
				result = ColumnChart.GetVisualObjectForColumnChart(preExistingPanel, width, height, plotDetails, dataSeriesList4Rendering, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.Area:
				result = AreaChart.GetVisualObjectForAreaChart(preExistingPanel, width, height, plotDetails, dataSeriesList4Rendering, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.Doughnut:
				result = PieChart.GetVisualObjectForDoughnutChart(width, height, plotDetails, dataSeriesList4Rendering, chart, animationEnabled);
				break;
			case RenderAs.StackedColumn:
				result = ColumnChart.GetVisualObjectForStackedColumnChart(chartType, preExistingPanel, width, height, plotDetails, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.StackedColumn100:
				result = ColumnChart.GetVisualObjectForStackedColumnChart(chartType, preExistingPanel, width, height, plotDetails, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.StackedBar:
				result = BarChart.GetVisualObjectForStackedBarChart(chartType, preExistingPanel, width, height, plotDetails, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.StackedBar100:
				result = BarChart.GetVisualObjectForStackedBarChart(chartType, preExistingPanel, width, height, plotDetails, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.StackedArea:
				result = AreaChart.GetVisualObjectForStackedAreaChart(preExistingPanel, width, height, plotDetails, dataSeriesList4Rendering, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.StackedArea100:
				result = AreaChart.GetVisualObjectForStackedArea100Chart(preExistingPanel, width, height, plotDetails, dataSeriesList4Rendering, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.Bubble:
				result = BubbleChart.GetVisualObjectForBubbleChart(preExistingPanel, width, height, plotDetails, dataSeriesList4Rendering, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.Point:
				result = PointChart.GetVisualObjectForPointChart(preExistingPanel, width, height, plotDetails, dataSeriesList4Rendering, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.StreamLineFunnel:
				result = FunnelChart.GetVisualObjectForFunnelChart(width, height, plotDetails, dataSeriesList4Rendering, chart, animationEnabled, true);
				break;
			case RenderAs.SectionFunnel:
				result = FunnelChart.GetVisualObjectForFunnelChart(width, height, plotDetails, dataSeriesList4Rendering, chart, animationEnabled, false);
				break;
			case RenderAs.Stock:
				result = StockChart.GetVisualObjectForStockChart(preExistingPanel, width, height, plotDetails, dataSeriesList4Rendering, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.CandleStick:
				result = CandleStick.GetVisualObjectForCandleStick(preExistingPanel, width, height, plotDetails, dataSeriesList4Rendering, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.StepLine:
				result = StepLineChart.GetVisualObjectForLineChart(preExistingPanel, width, height, plotDetails, dataSeriesList4Rendering, chart, plankDepth, animationEnabled);
				break;
			case RenderAs.Radar:
				result = RadarChart.GetVisualObjectForRadarChart(width, height, plotDetails, dataSeriesList4Rendering, chart, animationEnabled);
				break;
			case RenderAs.Polar:
				result = PolarChart.GetVisualObjectForPolarChart(width, height, plotDetails, dataSeriesList4Rendering, chart, animationEnabled);
				break;
			case RenderAs.Pyramid:
				result = PyramidChart.GetVisualObjectForPyramidChart(width, height, plotDetails, dataSeriesList4Rendering, chart, animationEnabled);
				break;
			case RenderAs.QuickLine:
				result = QuickLineChart.GetVisualObjectForQuickLineChart(preExistingPanel, width, height, plotDetails, dataSeriesList4Rendering, chart, plankDepth, animationEnabled);
				break;
			}
			return result;
		}

		internal static void PrepareCanvas4Drawing(Canvas preExistingPanel, out Canvas visual, out Canvas labelCanvas, out Canvas drawingCanvas, double width, double height)
		{
			if (preExistingPanel != null)
			{
				visual = preExistingPanel;
				labelCanvas = (preExistingPanel.Children[0] as Canvas);
				labelCanvas.Children.Clear();
			}
			else
			{
				visual = new Canvas();
				labelCanvas = new Canvas();
			}
			if (labelCanvas != null)
			{
				labelCanvas.Width = width;
				labelCanvas.Height = height;
			}
			visual.Width = width;
			visual.Height = height;
			drawingCanvas = new Canvas
			{
				Width = width,
				Height = height
			};
		}

		internal static void PrepareCanvas4Drawing(Canvas preExistingPanel, out Canvas visual, out Canvas drawingCanvas, double width, double height)
		{
			if (preExistingPanel != null)
			{
				visual = preExistingPanel;
				visual.Children.Clear();
			}
			else
			{
				visual = new Canvas();
			}
			visual.Width = width;
			visual.Height = height;
			drawingCanvas = new Canvas
			{
				Width = width,
				Height = height
			};
			visual.Children.Add(drawingCanvas);
		}

		internal static void UpdateVisualObject(RenderHelperInfo info)
		{
			Chart chart = info.Chart;
			VcProperties property = info.Property;
			object newValue = info.NewValue;
			bool isPartialUpdate = info.IsPartialUpdate;
			bool isZoomingOnProgress = info.IsZoomingOnProgress;
			bool isThroughRasterRender = info.IsThroughRasterRender;
			if (!chart._internalPartialUpdateEnabled || double.IsNaN(chart.ActualWidth) || double.IsNaN(chart.ActualHeight) || chart.ActualWidth == 0.0 || chart.ActualHeight == 0.0)
			{
				return;
			}
			bool isCalculatePlotDetails = true;
			if (isPartialUpdate && chart._datapoint2UpdatePartially != null && (isThroughRasterRender || chart._datapoint2UpdatePartially.Count <= 500))
			{
				chart.PARTIAL_DP_RENDER_LOCK = false;
				bool flag = false;
				if (isThroughRasterRender)
				{
					List<DataSeries> list = (from value in chart._datapoint2UpdatePartially
					where value.Value != VcProperties.XValue
					select value.Key.Parent).Distinct<DataSeries>().ToList<DataSeries>();
					if (list.Count > 0)
					{
						foreach (DataSeries current in list)
						{
							current.UpdateVisual(VcProperties.DataPointUpdate, null);
						}
						return;
					}
					flag = true;
				}
				else
				{
					List<DataSeries> _dataSeries2UpdateAtSeriesWise = (from value in chart._datapoint2UpdatePartially
					where value.Key.Parent != null && (value.Key.Parent.RenderAs == RenderAs.Spline || value.Key.Parent.RenderAs == RenderAs.QuickLine)
					select value.Key.Parent).Distinct<DataSeries>().ToList<DataSeries>();
					foreach (DataSeries current2 in _dataSeries2UpdateAtSeriesWise)
					{
						current2.UpdateVisual(VcProperties.DataPointUpdate, null);
					}
					List<KeyValuePair<IDataPoint, VcProperties>> list2 = (from dpInfo in chart._datapoint2UpdatePartially
					where !_dataSeries2UpdateAtSeriesWise.Contains(dpInfo.Key.Parent)
					select dpInfo).ToList<KeyValuePair<IDataPoint, VcProperties>>();
					if (list2.Count == 0)
					{
						return;
					}
					foreach (KeyValuePair<IDataPoint, VcProperties> current3 in list2)
					{
						IDataPoint key = current3.Key;
						if (key.Parent != null)
						{
							if (key.Parent.RenderAs == RenderAs.Spline || key.Parent.RenderAs == RenderAs.QuickLine)
							{
								flag = false;
							}
							else
							{
								if (current3.Value == VcProperties.XValue)
								{
									flag = true;
									break;
								}
								PropertyInfo property2 = key.GetType().GetProperty(current3.Value.ToString());
								newValue = property2.GetValue(key, null);
								flag = DataPointHelper.UpdateVisual(current3.Key, current3.Value, newValue, true);
								if (flag)
								{
									if (chart.ChartArea != null)
									{
										chart.ChartArea._axisUpdated = false;
										chart.ChartArea._plotDetailsReCreated = false;
										chart.ChartArea._seriesListUpdated = false;
										break;
									}
									break;
								}
							}
						}
					}
				}
				if (!flag)
				{
					return;
				}
			}
			else if (isZoomingOnProgress)
			{
				isCalculatePlotDetails = false;
			}
			PartialUpdateConfiguration partialUpdateConfiguration = new PartialUpdateConfiguration
			{
				Sender = chart,
				ElementType = ElementTypes.Chart,
				Property = VcProperties.None,
				OldValue = null,
				NewValue = null,
				IsUpdateLists = false,
				IsCalculatePlotDetails = isCalculatePlotDetails,
				IsUpdateAxis = true,
				RenderAxisType = AxisRepresentations.AxisY,
				IsPartialUpdate = true
			};
			if ((chart.PlotDetails != null && chart.PlotDetails.PlotGroups != null && chart.PlotDetails.PlotGroups.Count > 1) || property == VcProperties.DataPoints)
			{
				chart.ChartArea._plotDetailsReCreated = false;
			}
			chart.ChartArea.PrePartialUpdateConfiguration(partialUpdateConfiguration);
			int i = 0;
			List<DataSeries> list3 = chart.PlotDetails.SeriesDrawingIndex.Keys.ToList<DataSeries>();
			while (i < chart.InternalSeries.Count)
			{
				List<DataSeries> list4 = new List<DataSeries>();
				RenderAs renderAs = list3[i].RenderAs;
				int num = chart.PlotDetails.SeriesDrawingIndex[list3[i]];
				for (int j = i; j < chart.InternalSeries.Count; j++)
				{
					DataSeries dataSeries = list3[j];
					if (renderAs == dataSeries.RenderAs && num == chart.PlotDetails.SeriesDrawingIndex[dataSeries])
					{
						list4.Add(dataSeries);
					}
					dataSeries.ClearOlderVisuals();
				}
				if (list4.Count == 0)
				{
					break;
				}
				chart._toolTip.Hide();
				if (list4.Count > 0 && list4[0].ToolTipElement != null)
				{
					list4[0].ToolTipElement.Hide();
				}
				chart.ChartArea.DisableIndicators();
				switch (renderAs)
				{
				case RenderAs.Column:
				case RenderAs.Bar:
					if (property != VcProperties.Enabled)
					{
						using (List<DataSeries>.Enumerator enumerator4 = list4.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								DataSeries current4 = enumerator4.Current;
								ColumnChart.Update(current4, ElementTypes.DataSeries, property, newValue, false);
							}
							break;
						}
						goto IL_56E;
					}
					ColumnChart.Update(chart, renderAs, list4);
					break;
				case RenderAs.Line:
					goto IL_5CA;
				case RenderAs.Area:
					goto IL_706;
				case RenderAs.StackedColumn:
					ColumnChart.Update(chart, renderAs, list4);
					break;
				case RenderAs.StackedColumn100:
					ColumnChart.Update(chart, renderAs, list4);
					break;
				case RenderAs.StackedBar:
					ColumnChart.Update(chart, renderAs, list4);
					break;
				case RenderAs.StackedBar100:
					ColumnChart.Update(chart, renderAs, list4);
					break;
				case RenderAs.StackedArea:
					ColumnChart.Update(chart, renderAs, list4);
					chart.ChartArea.AttachEventsToolTipHref2DataSeries();
					break;
				case RenderAs.StackedArea100:
					ColumnChart.Update(chart, renderAs, list4);
					chart.ChartArea.AttachEventsToolTipHref2DataSeries();
					break;
				case RenderAs.Bubble:
					goto IL_6B7;
				case RenderAs.Point:
					goto IL_668;
				case RenderAs.Stock:
					if (property != VcProperties.ViewportRangeEnabled || chart.PlotDetails.IsOverrideViewPortRangeEnabled)
					{
						using (List<DataSeries>.Enumerator enumerator5 = list4.GetEnumerator())
						{
							while (enumerator5.MoveNext())
							{
								DataSeries current5 = enumerator5.Current;
								StockChart.Update(current5, ElementTypes.DataSeries, property, newValue, false);
							}
							break;
						}
						goto IL_7DE;
					}
					ColumnChart.Update(chart, renderAs, list4);
					break;
				case RenderAs.CandleStick:
					goto IL_7DE;
				case RenderAs.StepLine:
					goto IL_619;
				case RenderAs.Spline:
				case RenderAs.QuickLine:
					goto IL_56E;
				}
				IL_834:
				i += list4.Count;
				continue;
				IL_56E:
				if (property == VcProperties.Enabled)
				{
					ColumnChart.Update(chart, renderAs, list4);
					goto IL_834;
				}
				using (List<DataSeries>.Enumerator enumerator6 = list4.GetEnumerator())
				{
					while (enumerator6.MoveNext())
					{
						DataSeries current6 = enumerator6.Current;
						RenderHelper.UpdateVisualObject(current6.RenderAs, new RenderHelperInfo(chart, current6, ElementTypes.DataSeries, property, newValue, false));
					}
					goto IL_834;
				}
				IL_5CA:
				if (property == VcProperties.Enabled)
				{
					ColumnChart.Update(chart, renderAs, list4);
					goto IL_834;
				}
				using (List<DataSeries>.Enumerator enumerator7 = list4.GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						DataSeries current7 = enumerator7.Current;
						LineChart.Update(current7, ElementTypes.DataSeries, property, newValue, false);
					}
					goto IL_834;
				}
				IL_619:
				if (property == VcProperties.Enabled)
				{
					ColumnChart.Update(chart, renderAs, list4);
					goto IL_834;
				}
				using (List<DataSeries>.Enumerator enumerator8 = list4.GetEnumerator())
				{
					while (enumerator8.MoveNext())
					{
						DataSeries current8 = enumerator8.Current;
						StepLineChart.Update(current8, ElementTypes.DataSeries, property, newValue, false);
					}
					goto IL_834;
				}
				IL_668:
				if (property == VcProperties.ViewportRangeEnabled)
				{
					ColumnChart.Update(chart, renderAs, list4);
					goto IL_834;
				}
				using (List<DataSeries>.Enumerator enumerator9 = list4.GetEnumerator())
				{
					while (enumerator9.MoveNext())
					{
						DataSeries current9 = enumerator9.Current;
						PointChart.Update(current9, ElementTypes.DataSeries, property, newValue, false);
					}
					goto IL_834;
				}
				IL_6B7:
				if (property == VcProperties.ViewportRangeEnabled)
				{
					ColumnChart.Update(chart, renderAs, list4);
					goto IL_834;
				}
				using (List<DataSeries>.Enumerator enumerator10 = list4.GetEnumerator())
				{
					while (enumerator10.MoveNext())
					{
						DataSeries current10 = enumerator10.Current;
						BubbleChart.Update(current10, ElementTypes.DataSeries, property, newValue, false);
					}
					goto IL_834;
				}
				IL_706:
				ColumnChart.Update(chart, renderAs, list4);
				goto IL_834;
				IL_7DE:
				if (property == VcProperties.ViewportRangeEnabled && !chart.PlotDetails.IsOverrideViewPortRangeEnabled)
				{
					ColumnChart.Update(chart, renderAs, list4);
					goto IL_834;
				}
				foreach (DataSeries current11 in list4)
				{
					CandleStick.Update(current11, ElementTypes.DataSeries, property, newValue, false);
				}
				goto IL_834;
			}
			chart.ChartArea.AttachScrollBarOffsetChangedEventWithAxes();
			chart.ChartArea.AttachOrDetachIntaractivity(chart.InternalSeries);
			Chart.SelectDataPoints(chart);
		}

		internal static void UpdateVisualObject(RenderAs chartType, RenderHelperInfo info)
		{
			IElement sender = info.Sender;
			ElementTypes elementType = info.ElementType;
			VcProperties property = info.Property;
			object newValue = info.NewValue;
			bool isAxisChanged = info.IsAxisChanged;
			Chart chart = info.Chart;
			switch (chartType)
			{
			case RenderAs.Column:
			case RenderAs.Bar:
				ColumnChart.Update(sender, elementType, property, newValue, isAxisChanged);
				return;
			case RenderAs.Line:
			case RenderAs.Spline:
				LineChart.Update(sender, elementType, property, newValue, isAxisChanged);
				return;
			case RenderAs.Pie:
			case RenderAs.Doughnut:
			case RenderAs.StreamLineFunnel:
			case RenderAs.SectionFunnel:
			case RenderAs.Radar:
			case RenderAs.Polar:
			case RenderAs.Pyramid:
				break;
			case RenderAs.Area:
				AreaChart.Update(sender, elementType, property, newValue, isAxisChanged);
				return;
			case RenderAs.StackedColumn:
				ColumnChart.Update(sender, elementType, property, newValue, isAxisChanged);
				return;
			case RenderAs.StackedColumn100:
				ColumnChart.Update(sender, elementType, property, newValue, isAxisChanged);
				return;
			case RenderAs.StackedBar:
				ColumnChart.Update(sender, elementType, property, newValue, isAxisChanged);
				return;
			case RenderAs.StackedBar100:
				ColumnChart.Update(sender, elementType, property, newValue, isAxisChanged);
				return;
			case RenderAs.StackedArea:
			{
				List<DataSeries> list = (from ds in chart.Series
				where ds.RenderAs == RenderAs.StackedArea
				select ds).ToList<DataSeries>();
				if (list.Count > 0)
				{
					ColumnChart.Update(chart, list[0].RenderAs, list);
				}
				chart.ChartArea.AttachEventsToolTipHref2DataSeries();
				return;
			}
			case RenderAs.StackedArea100:
			{
				List<DataSeries> list2 = (from ds in chart.Series
				where ds.RenderAs == RenderAs.StackedArea
				select ds).ToList<DataSeries>();
				if (list2.Count > 0)
				{
					ColumnChart.Update(chart, list2[0].RenderAs, list2);
				}
				chart.ChartArea.AttachEventsToolTipHref2DataSeries();
				return;
			}
			case RenderAs.Bubble:
				BubbleChart.Update(sender, elementType, property, newValue, isAxisChanged);
				return;
			case RenderAs.Point:
				PointChart.Update(sender, elementType, property, newValue, isAxisChanged);
				return;
			case RenderAs.Stock:
				StockChart.Update(sender, elementType, property, newValue, isAxisChanged);
				return;
			case RenderAs.CandleStick:
				CandleStick.Update(sender, elementType, property, newValue, isAxisChanged);
				break;
			case RenderAs.StepLine:
				StepLineChart.Update(sender, elementType, property, newValue, isAxisChanged);
				return;
			case RenderAs.QuickLine:
				QuickLineChart.Update(sender, elementType, property, newValue, isAxisChanged);
				return;
			default:
				return;
			}
		}

		internal static List<IDataPoint> GetDataPointsUnderViewPort(PlotGroup plotGroup, List<IDataPoint> dataPoints, bool isUsedForAxisRange, int leftRightPointPadding)
		{
			if (plotGroup.AxisY.ViewportRangeEnabled)
			{
				Axis axisX = plotGroup.AxisX;
				double minXValueRangeOfViewPort = axisX._numericViewMinimum;
				double maxXValueRangeOfViewPort = axisX._numericViewMaximum;
				Math.Abs(minXValueRangeOfViewPort - maxXValueRangeOfViewPort);
				if (!isUsedForAxisRange)
				{
					IEnumerable<double> source = from dp in dataPoints
					where dp.DpInfo.ActualNumericXValue < minXValueRangeOfViewPort
					select dp.DpInfo.ActualNumericXValue;
					IEnumerable<double> source2 = from dp in dataPoints
					where dp.DpInfo.ActualNumericXValue > maxXValueRangeOfViewPort
					select dp.DpInfo.ActualNumericXValue;
					if (source.Any<double>())
					{
						minXValueRangeOfViewPort = source.Max();
					}
					if (source2.Any<double>())
					{
						maxXValueRangeOfViewPort = source2.Min();
					}
				}
				List<IDataPoint> list = (from d in dataPoints
				where d.DpInfo.ActualNumericXValue >= minXValueRangeOfViewPort && d.DpInfo.ActualNumericXValue <= maxXValueRangeOfViewPort
				select d).ToList<IDataPoint>();
				if (list.Count <= leftRightPointPadding)
				{
					IOrderedEnumerable<IDataPoint> source3 = from dp in dataPoints
					where dp.DpInfo.ActualNumericXValue < minXValueRangeOfViewPort
					orderby dp.DpInfo.ActualNumericXValue
					select dp;
					List<IDataPoint> list2 = (from dp in dataPoints
					where dp.DpInfo.ActualNumericXValue > maxXValueRangeOfViewPort
					orderby dp.DpInfo.ActualNumericXValue
					select dp).ToList<IDataPoint>();
					if (source3.Any<IDataPoint>())
					{
						list.Insert(0, source3.Last<IDataPoint>());
					}
					if (list2.Count > 0)
					{
						list.Add(list2[0]);
					}
					if (list2.Count > 1)
					{
						list.Add(list2[1]);
					}
					if (list2.Count > 2)
					{
						list.Add(list2[2]);
					}
				}
				return list;
			}
			return dataPoints;
		}

		internal static List<IDataPoint> GetDataPointsUnderViewPort(DataSeries dataSeries, bool isUsedForAxisRange, bool isOverrideViewportRangeEnabled = false)
		{
			if ((dataSeries.Chart as Chart).ZoomingEnabled && (dataSeries.Chart as Chart).ChartArea != null && (dataSeries.Chart as Chart).ChartArea._isFirstTimeRender)
			{
				return dataSeries.InternalDataPoints;
			}
			if (((dataSeries.Chart as Chart).ScrollingEnabled.Value || (dataSeries.Chart as Chart).ZoomingEnabled) && (dataSeries.PlotGroup.AxisY.ViewportRangeEnabled || isOverrideViewportRangeEnabled))
			{
				PlotGroup plotGroup = dataSeries.PlotGroup;
				Axis axisX = plotGroup.AxisX;
				double minXValueRangeOfViewPort = axisX._numericViewMinimum;
				double maxXValueRangeOfViewPort = axisX._numericViewMaximum;
				if (!isUsedForAxisRange)
				{
					IEnumerable<double> source = from dp in dataSeries.InternalDataPoints
					where dp.DpInfo.ActualNumericXValue < minXValueRangeOfViewPort
					select dp.DpInfo.ActualNumericXValue;
					IEnumerable<double> source2 = from dp in dataSeries.InternalDataPoints
					where dp.DpInfo.ActualNumericXValue > maxXValueRangeOfViewPort
					select dp.DpInfo.ActualNumericXValue;
					if (source.Any<double>())
					{
						minXValueRangeOfViewPort = source.Max();
					}
					if (source2.Any<double>())
					{
						maxXValueRangeOfViewPort = source2.Min();
					}
				}
				List<IDataPoint> list = (from d in dataSeries.InternalDataPoints
				where d.DpInfo.ActualNumericXValue >= minXValueRangeOfViewPort && d.DpInfo.ActualNumericXValue <= maxXValueRangeOfViewPort
				select d).ToList<IDataPoint>();
				if (list.Count <= 3)
				{
					IOrderedEnumerable<IDataPoint> source3 = from dp in dataSeries.InternalDataPoints
					where dp.DpInfo.ActualNumericXValue < minXValueRangeOfViewPort
					orderby dp.DpInfo.ActualNumericXValue
					select dp;
					List<IDataPoint> list2 = (from dp in dataSeries.InternalDataPoints
					where dp.DpInfo.ActualNumericXValue > maxXValueRangeOfViewPort
					orderby dp.DpInfo.ActualNumericXValue
					select dp).ToList<IDataPoint>();
					if (source3.Any<IDataPoint>())
					{
						list.Insert(0, source3.Last<IDataPoint>());
					}
					if (list2.Count > 0)
					{
						list.Add(list2[0]);
					}
					if (list2.Count > 1)
					{
						list.Add(list2[1]);
					}
					if (list2.Count > 2)
					{
						list.Add(list2[2]);
					}
				}
				return list;
			}
			return dataSeries.InternalDataPoints;
		}

		internal static double[] GetXValuesUnderViewPort(List<double> xValues, Axis axisX, Axis axisY, bool isUsedForAxisRange)
		{
			if (axisY != null && ((axisY.Chart as Chart).ScrollingEnabled.Value || (axisY.Chart as Chart).ZoomingEnabled) && axisY.ViewportRangeEnabled)
			{
				double minXValueRangeOfViewPort;
				double maxXValueRangeOfViewPort;
				if (axisX.Logarithmic)
				{
					minXValueRangeOfViewPort = Math.Log(axisX._numericViewMinimum, axisX.LogarithmBase);
					maxXValueRangeOfViewPort = Math.Log(axisX._numericViewMaximum, axisX.LogarithmBase);
				}
				else
				{
					minXValueRangeOfViewPort = axisX._numericViewMinimum;
					maxXValueRangeOfViewPort = axisX._numericViewMaximum;
				}
				if (!isUsedForAxisRange)
				{
					IEnumerable<double> source = from xValue in xValues
					where xValue < minXValueRangeOfViewPort
					select xValue;
					IEnumerable<double> source2 = from xValue in xValues
					where xValue > maxXValueRangeOfViewPort
					select xValue;
					if (source.Any<double>())
					{
						minXValueRangeOfViewPort = source.Max();
					}
					if (source2.Any<double>())
					{
						maxXValueRangeOfViewPort = source2.Min();
					}
				}
				List<double> list = (from d in xValues
				where d >= minXValueRangeOfViewPort && d <= maxXValueRangeOfViewPort
				select d).ToList<double>();
				if (list.Count <= 3)
				{
					IEnumerable<double> source3 = (from xValue in xValues
					where xValue < minXValueRangeOfViewPort
					select xValue).Distinct<double>();
					List<double> list2 = (from xValue in xValues
					where xValue > maxXValueRangeOfViewPort
					select xValue).Distinct<double>().ToList<double>();
					if (source3.Any<double>())
					{
						list.Insert(0, source3.Last<double>());
					}
					if (list2.Count > 0)
					{
						list.Add(list2[0]);
					}
					if (list2.Count > 1)
					{
						list.Add(list2[1]);
					}
					if (list2.Count > 2)
					{
						list.Add(list2[2]);
					}
				}
				return list.ToArray();
			}
			return xValues.ToArray();
		}

		internal static double CalculateInternalXValueFromPixelPos(Chart chart, Axis xAxis, MouseEventArgs e)
		{
			if (chart == null)
			{
				return double.NaN;
			}
			AxisOrientation axisOrientation = xAxis.AxisOrientation;
			double num = (axisOrientation == AxisOrientation.Horizontal) ? e.GetPosition(chart.ChartArea.PlottingCanvas).X : e.GetPosition(chart.ChartArea.PlottingCanvas).Y;
			double num2 = (axisOrientation == AxisOrientation.Horizontal) ? chart.ChartArea.ChartVisualCanvas.Width : chart.ChartArea.ChartVisualCanvas.Height;
			return xAxis.PixelPositionToXValue(num2, (axisOrientation == AxisOrientation.Horizontal) ? num : (num2 - num));
		}

		internal static double CalculateInternalYValueFromPixelPos(Chart chart, Axis yAxis, MouseEventArgs e)
		{
			if (chart == null)
			{
				return double.NaN;
			}
			AxisOrientation axisOrientation = yAxis.AxisOrientation;
			double num = (axisOrientation == AxisOrientation.Vertical) ? e.GetPosition(chart.ChartArea.PlottingCanvas).Y : e.GetPosition(chart.ChartArea.PlottingCanvas).X;
			double num2 = (axisOrientation == AxisOrientation.Vertical) ? chart.ChartArea.ChartVisualCanvas.Height : chart.ChartArea.ChartVisualCanvas.Width;
			return yAxis.PixelPositionToYValue(num2, (axisOrientation == AxisOrientation.Vertical) ? num : (num2 - num));
		}

		internal static IDataPoint GetNearestDataPointAlongXAxis(List<IDataPoint> dataPoints, Axis xAxis, double internalXValue)
		{
			List<IDataPoint> list = (from dp in dataPoints
			where !RenderHelper.IsFinancialCType(dp.Parent) || dp.DpInfo.InternalYValues != null || RenderHelper.IsFinancialCType(dp.Parent) || !double.IsNaN(dp.YValue)
			orderby Math.Abs(dp.DpInfo.InternalXValue - internalXValue)
			select dp).ToList<IDataPoint>();
			if (list.Count > 0)
			{
				return list.First<IDataPoint>();
			}
			return null;
		}

		internal static double YValueToInternalYValue(Axis yAxis, double userYValue)
		{
			double result;
			if (yAxis != null && yAxis.Logarithmic)
			{
				result = DataPointHelper.ConvertYValue2LogarithmicValue(yAxis.Chart as Chart, userYValue, yAxis.AxisType);
			}
			else
			{
				result = userYValue;
			}
			return result;
		}

		internal static double XValueToInternalXValue(Axis xAxis, object userXValue)
		{
			double result = double.NaN;
			if (xAxis != null && userXValue != null)
			{
				if (xAxis.IsDateTimeAxis)
				{
					try
					{
						DateTime dateTime = Convert.ToDateTime(userXValue);
						result = DateTimeHelper.DateDiff(dateTime, xAxis.MinDate, xAxis.MinDateRange, xAxis.MaxDateRange, xAxis.InternalIntervalType, xAxis.XValueType);
						return result;
					}
					catch
					{
						throw new ArgumentException("Incorrect DateTime value of XValue.");
					}
				}
				if (xAxis.Logarithmic)
				{
					result = Math.Log(Convert.ToDouble(userXValue), xAxis.LogarithmBase);
				}
				else
				{
					result = Convert.ToDouble(userXValue);
				}
			}
			return result;
		}

		internal static void UserValueToInternalValues(Axis xAxis, Axis yAxis, object userXValue, double userYValue, out double internalXValue, out double internalYValue)
		{
			internalYValue = userYValue;
			internalXValue = double.NaN;
			if (xAxis != null && userXValue != null)
			{
				if (xAxis.IsDateTimeAxis)
				{
					try
					{
						DateTime dateTime = Convert.ToDateTime(userXValue);
						internalXValue = DateTimeHelper.DateDiff(dateTime, xAxis.MinDate, xAxis.MinDateRange, xAxis.MaxDateRange, xAxis.InternalIntervalType, xAxis.XValueType);
						goto IL_63;
					}
					catch
					{
						throw new ArgumentException("Incorrect DateTime value of XValue.");
					}
				}
				internalXValue = Convert.ToDouble(userXValue);
			}
			IL_63:
			if (yAxis != null && yAxis.Logarithmic)
			{
				internalYValue = DataPointHelper.ConvertYValue2LogarithmicValue(xAxis.Chart as Chart, userYValue, yAxis.AxisType);
				return;
			}
			internalYValue = userYValue;
		}

		internal static IDataPoint GetNearestDataPoint(DataSeries dataSeries, double xValueAtMousePos, double yValueAtMousePos)
		{
			IDataPoint result = null;
			List<IDataPoint> list = (from dp in dataSeries.InternalDataPoints
			where !RenderHelper.IsFinancialCType(dp.Parent) || dp.DpInfo.InternalYValues != null || RenderHelper.IsFinancialCType(dp.Parent) || !double.IsNaN(dp.YValue)
			orderby Math.Abs(dp.DpInfo.InternalXValue - xValueAtMousePos)
			select dp).ToList<IDataPoint>();
			if (list.Count > 0)
			{
				double internalXValue = list[0].DpInfo.InternalXValue;
				List<IDataPoint> list2 = new List<IDataPoint>();
				foreach (IDataPoint current in list)
				{
					if (current.DpInfo.InternalXValue == internalXValue)
					{
						list2.Add(current);
					}
				}
				if (!double.IsNaN(yValueAtMousePos))
				{
					if (RenderHelper.IsFinancialCType(dataSeries))
					{
						list2 = (from dp in list2
						where dp.DpInfo.InternalYValues != null
						orderby Math.Abs(dp.DpInfo.InternalYValues.Max() - yValueAtMousePos)
						select dp).ToList<IDataPoint>();
					}
					else
					{
						list2 = (from dp in list2
						where !double.IsNaN(dp.DpInfo.InternalYValue)
						orderby Math.Abs(dp.DpInfo.InternalYValue - yValueAtMousePos)
						select dp).ToList<IDataPoint>();
					}
				}
				if (list2.Count > 0)
				{
					result = list2.First<IDataPoint>();
				}
			}
			return result;
		}

		public static bool IsFinancialCType(DataSeries dataSeries)
		{
			RenderAs renderAs = dataSeries.RenderAs;
			return renderAs == RenderAs.CandleStick || renderAs == RenderAs.Stock;
		}

		public static bool IsCircularCType(DataSeries dataSeries)
		{
			RenderAs renderAs = dataSeries.RenderAs;
			return renderAs == RenderAs.Radar || renderAs == RenderAs.Polar;
		}

		public static bool IsLineCType(DataSeries dataSeries)
		{
			RenderAs renderAs = dataSeries.RenderAs;
			return renderAs == RenderAs.Line || renderAs == RenderAs.Spline || renderAs == RenderAs.QuickLine || renderAs == RenderAs.StepLine;
		}

		public static bool IsRasterRenderSupported(DataSeries dataSeries)
		{
			if (dataSeries.LightWeight.Value)
			{
				RenderAs renderAs = dataSeries.RenderAs;
				return renderAs == RenderAs.Line || renderAs == RenderAs.QuickLine || renderAs == RenderAs.CandleStick || renderAs == RenderAs.Stock || renderAs == RenderAs.Point;
			}
			return false;
		}

		public static bool IsAreaCType(DataSeries dataSeries)
		{
			RenderAs renderAs = dataSeries.RenderAs;
			return renderAs == RenderAs.Area || renderAs == RenderAs.StackedArea || renderAs == RenderAs.StackedArea100;
		}

		public static bool IsIndependentCType(DataSeries dataSeries)
		{
			RenderAs renderAs = dataSeries.RenderAs;
			return renderAs == RenderAs.Area || renderAs == RenderAs.Bar || renderAs == RenderAs.Bubble || renderAs == RenderAs.Column || renderAs == RenderAs.Spline || renderAs == RenderAs.StepLine || renderAs == RenderAs.Line || renderAs == RenderAs.Point || renderAs == RenderAs.CandleStick || renderAs == RenderAs.Stock;
		}

		public static bool IsAxisIndependentCType(DataSeries dataSeries)
		{
			RenderAs renderAs = dataSeries.RenderAs;
			return renderAs == RenderAs.Pie || renderAs == RenderAs.Doughnut || renderAs == RenderAs.SectionFunnel || renderAs == RenderAs.StreamLineFunnel || renderAs == RenderAs.Pyramid;
		}
	}
}
