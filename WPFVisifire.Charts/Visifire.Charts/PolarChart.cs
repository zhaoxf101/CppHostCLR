using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class PolarChart
	{
		private static void CreatePolarSeries(Chart chart, DataSeries series, Canvas polarCanvas, Canvas labelCanvas, double width, double height, PlotGroup plotGroup, CircularPlotDetails circularPlotDetails)
		{
			List<List<IDataPoint>> brokenLineDataPointsGroup = PolarChart.GetBrokenLineDataPointsGroup(series, circularPlotDetails, plotGroup);
			foreach (List<IDataPoint> current in brokenLineDataPointsGroup)
			{
				foreach (IDataPoint current2 in current)
				{
					PolarChart.DrawMarker(current2, labelCanvas, width, height, circularPlotDetails.Center);
				}
				PolarChart.DrawDataSeriesPath(series, current, polarCanvas);
			}
		}

		private static List<List<IDataPoint>> GetBrokenLineDataPointsGroup(DataSeries dataSeries, CircularPlotDetails circularPlotDetails, PlotGroup plotGroup)
		{
			bool flag = true;
			List<List<IDataPoint>> list = new List<List<IDataPoint>>();
			List<IDataPoint> list2 = new List<IDataPoint>();
			foreach (IDataPoint current in dataSeries.InternalDataPoints)
			{
				if (!(current.Enabled == false))
				{
					double internalYValue = current.DpInfo.InternalYValue;
					if (double.IsNaN(internalYValue) || internalYValue > plotGroup.AxisY.InternalAxisMaximum || internalYValue < plotGroup.AxisY.InternalAxisMinimum)
					{
						flag = true;
					}
					else
					{
						Point polarPoint = PolarChart.GetPolarPoint(circularPlotDetails, plotGroup, current);
						double x = polarPoint.X;
						double y = polarPoint.Y;
						if (flag)
						{
							flag = !flag;
							if (list2.Count > 0)
							{
								list.Add(list2);
							}
							list2 = new List<IDataPoint>();
						}
						else
						{
							new Point(x, y);
							flag = false;
						}
						current.DpInfo.VisualPosition = new Point(x, y);
						list2.Add(current);
					}
				}
			}
			list.Add(list2);
			return list;
		}

		private static void DrawMarker(IDataPoint dp, Canvas labelCanvas, double width, double height, Point center)
		{
			Chart chart = dp.Chart as Chart;
			double num = CircularLabel.ResetMeanAngle(PolarChart.CalculateAngleByCoordinate(dp.DpInfo.VisualPosition, center));
			Marker markerForDataPoint;
			if (num > 0.0 && num < 3.1415926535897931)
			{
				markerForDataPoint = PolarChart.GetMarkerForDataPoint(chart, width, height, dp.DpInfo.VisualPosition.Y, dp, false);
			}
			else
			{
				markerForDataPoint = PolarChart.GetMarkerForDataPoint(chart, width, height, dp.DpInfo.VisualPosition.Y, dp, true);
			}
			if (markerForDataPoint != null)
			{
				markerForDataPoint.AddToParent(labelCanvas, dp.DpInfo.VisualPosition.X, dp.DpInfo.VisualPosition.Y, new Point(0.5, 0.5));
			}
		}

		private static Marker GetMarkerForDataPoint(Chart chart, double plotWidth, double plotHeight, double yPosition, IDataPoint dataPoint, bool isPositionTop)
		{
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled);
			string text = flag ? dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText)) : "";
			bool markerBevel = false;
			Marker marker = dataPoint.DpInfo.Marker;
			if (marker != null && marker.Visual != null)
			{
				Panel panel = marker.Visual.Parent as Panel;
				if (panel != null)
				{
					panel.Children.Remove(marker.Visual);
				}
			}
			LabelStyles labelStyle = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			double num = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelAngle);
			double num2 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
			double scaleFactor = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
			MarkerTypes markerType = (MarkerTypes)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerType);
			Brush markerColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerColor);
			bool isEnabled = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled);
			double num3 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity);
			string str = (string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href);
			dataPoint.DpInfo.Marker = new Marker();
			dataPoint.DpInfo.Marker.AssignPropertiesValue(markerType, scaleFactor, new Size(num2, num2), markerBevel, markerColor, text);
			dataPoint.DpInfo.Marker.IsEnabled = isEnabled;
			LineChart.ApplyMarkerProperties(dataPoint);
			if (flag && !string.IsNullOrEmpty(text))
			{
				LineChart.ApplyLabelProperties(dataPoint);
				if (!double.IsNaN(num) && num != 0.0)
				{
					dataPoint.DpInfo.Marker.LabelAngle = num;
					dataPoint.DpInfo.Marker.TextOrientation = Orientation.Vertical;
					PolarChart.SetPositionForDataPointLabel(chart, isPositionTop, dataPoint, yPosition);
					dataPoint.DpInfo.Marker.LabelStyle = labelStyle;
				}
				dataPoint.DpInfo.Marker.CreateVisual((chart != null) ? chart.FlowDirection : FlowDirection.LeftToRight);
				if (double.IsNaN(num) || num == 0.0)
				{
					dataPoint.DpInfo.Marker.TextAlignmentX = AlignmentX.Center;
					PolarChart.SetPositionForDataPointLabel(chart, isPositionTop, dataPoint, yPosition);
				}
			}
			dataPoint.DpInfo.Marker.Control = chart;
			dataPoint.DpInfo.Marker.Tag = new ElementData
			{
				Element = dataPoint
			};
			dataPoint.DpInfo.Marker.CreateVisual((chart != null) ? chart.FlowDirection : FlowDirection.LeftToRight);
			dataPoint.DpInfo.Marker.Visual.Opacity = num3 * dataPoint.Parent.Opacity;
			LineChart.ApplyDefaultInteractivityForMarker(dataPoint);
			if (!dataPoint.IsLightDataPoint)
			{
				dataPoint.AttachEvents2Visual(dataPoint as DataPoint, dataPoint, dataPoint.DpInfo.Marker.Visual);
			}
			dataPoint.AttachEvents2Visual(dataPoint.Parent, dataPoint, dataPoint.DpInfo.Marker.Visual);
			dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
			if (!dataPoint.IsLightDataPoint)
			{
				(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser(str);
			}
			dataPoint.AttachToolTip(chart, dataPoint, dataPoint.DpInfo.Marker.Visual);
			dataPoint.DpFunc.AttachHref(chart, dataPoint.DpInfo.Marker.Visual);
			DataPointHelper.SetCursor2DataPointVisualFaces(dataPoint);
			return dataPoint.DpInfo.Marker;
		}

		private static void SetPositionForDataPointLabel(Chart chart, bool isPositionTop, IDataPoint dataPoint, double yPosition)
		{
			LabelStyles labelStyles = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			double num = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelAngle);
			if (!double.IsNaN(num) && num != 0.0)
			{
				if (isPositionTop)
				{
					dataPoint.DpInfo.Marker.TextAlignmentX = AlignmentX.Center;
					dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Top;
					return;
				}
				dataPoint.DpInfo.Marker.TextAlignmentX = AlignmentX.Center;
				dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Bottom;
				return;
			}
			else if (isPositionTop)
			{
				if (labelStyles == LabelStyles.OutSide && !dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
				{
					if (yPosition - dataPoint.DpInfo.Marker.MarkerActualSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 < 0.0 || labelStyles == LabelStyles.Inside)
					{
						dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Bottom;
						return;
					}
					dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Top;
					return;
				}
				else
				{
					if (labelStyles == LabelStyles.OutSide)
					{
						dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Top;
						return;
					}
					dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Bottom;
					return;
				}
			}
			else if (labelStyles == LabelStyles.OutSide && !dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
			{
				if (yPosition + dataPoint.DpInfo.Marker.MarkerActualSize.Height + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 > chart.PlotArea.BorderElement.Height || labelStyles == LabelStyles.Inside)
				{
					dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Top;
					return;
				}
				dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Bottom;
				return;
			}
			else
			{
				if (labelStyles == LabelStyles.OutSide)
				{
					dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Bottom;
					return;
				}
				dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Top;
				return;
			}
		}

		private static void DrawDataSeriesPath(DataSeries series, List<IDataPoint> pointCollection, Canvas polarCanvas)
		{
			if (series.InternalDataPoints.Count > 0)
			{
				Path path = new Path
				{
					Tag = new ElementData
					{
						Element = series,
						VisualElementName = "PolarVisual"
					}
				};
				path.Stroke = (series.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(series.Color, "Linear", null) : series.Color);
				path.StrokeDashArray = ExtendedGraphics.GetDashArray(series.LineStyle);
				path.StrokeThickness = series.LineThickness.Value;
				path.StrokeMiterLimit = 1.0;
				path.Opacity = series.Opacity;
				path.Data = PolarChart.GetPathGeometry(pointCollection);
				series.Faces.Parts.Add(path);
				polarCanvas.Children.Add(path);
			}
		}

		private static Geometry GetPathGeometry(List<IDataPoint> pointCollection)
		{
			PathGeometry pathGeometry = new PathGeometry();
			PathFigure pathFigure = new PathFigure();
			if (pointCollection.Count > 0)
			{
				pathFigure.StartPoint = pointCollection[0].DpInfo.VisualPosition;
				for (int i = 1; i < pointCollection.Count; i++)
				{
					LineSegment lineSegment = new LineSegment();
					lineSegment.Point = pointCollection[i].DpInfo.VisualPosition;
					new Faces();
					pathFigure.Segments.Add(lineSegment);
				}
			}
			pathGeometry.Figures.Add(pathFigure);
			return pathGeometry;
		}

		private static Canvas GetDataSeriesVisual(Chart chart, double width, double height, DataSeries series, CircularPlotDetails circularPlotDetails)
		{
			Canvas canvas = new Canvas();
			Canvas canvas2 = new Canvas();
			Canvas canvas3 = new Canvas();
			if (series.Enabled.Value)
			{
				PlotGroup plotGroup = series.PlotGroup;
				if (circularPlotDetails.ListOfPoints4CircularAxis.Count > 0)
				{
					series.Faces = new Faces();
					PolarChart.CreatePolarSeries(chart, series, canvas2, canvas3, width, height, plotGroup, circularPlotDetails);
				}
			}
			if (chart._internalAnimationEnabled)
			{
				if (series.Storyboard == null)
				{
					series.Storyboard = new Storyboard();
				}
				double num = 1.0;
				series.Storyboard = AnimationHelper.ApplyOpacityAnimation(canvas2, series, series.Storyboard, num, 1.0, 0.0, 1.0);
				series.Storyboard = AnimationHelper.ApplyOpacityAnimation(canvas3, series, series.Storyboard, num + 0.5, 1.0, 0.0, 1.0);
			}
			canvas.Children.Add(canvas2);
			canvas.Children.Add(canvas3);
			return canvas;
		}

		private static Point GetPolarPoint(CircularPlotDetails circularPlotDetails, PlotGroup plotGroup, IDataPoint dp)
		{
			double internalYValue = dp.DpInfo.InternalYValue;
			double num = Graphics.ValueToPixelPosition(circularPlotDetails.Radius, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, internalYValue);
			double num2 = circularPlotDetails.Radius - num;
			Axis axisX = plotGroup.AxisX;
			double num3;
			if (axisX.InternalAxisMinimum != 0.0)
			{
				num3 = AxisLabel.GetRadians(axisX.InternalAxisMinimum - 90.0);
			}
			else
			{
				num3 = 4.71238898038469;
			}
			double num4;
			if (dp.Parent.XValueType == ChartValueTypes.Time)
			{
				num4 = Graphics.ValueToPixelPosition(num3, 6.2831853071795862 + num3, AxisLabel.GetRadians(0.0), AxisLabel.GetRadians(360.0), AxisLabel.GetRadians(dp.DpInfo.InternalXValue));
			}
			else
			{
				num4 = Graphics.ValueToPixelPosition(num3, 6.2831853071795862 + num3, AxisLabel.GetRadians(axisX.InternalAxisMinimum), AxisLabel.GetRadians(axisX.InternalAxisMaximum), AxisLabel.GetRadians(axisX.InternalAxisMinimum + dp.DpInfo.ActualNumericXValue));
			}
			double x = num2 * Math.Cos(num4) + circularPlotDetails.Center.X;
			double y = num2 * Math.Sin(num4) + circularPlotDetails.Center.Y;
			return new Point(x, y);
		}

		private static double CalculateAngleByCoordinate(Point position, Point center)
		{
			return Math.Atan2(position.Y - center.Y, position.X - center.X);
		}

		internal static Canvas GetVisualObjectForPolarChart(double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, bool isAnimationEnabled)
		{
			if (double.IsNaN(width) || double.IsNaN(height) || width <= 0.0 || height <= 0.0)
			{
				return null;
			}
			Canvas canvas = new Canvas
			{
				Width = width,
				Height = height
			};
			Axis axisX = seriesList[0].PlotGroup.AxisX;
			Size size = new Size(width, height);
			foreach (DataSeries current in seriesList)
			{
				if (current.InternalDataPoints.Count != 0)
				{
					Canvas dataSeriesVisual = PolarChart.GetDataSeriesVisual(chart, size.Width, size.Height, current, axisX.CircularPlotDetails);
					canvas.Children.Add(dataSeriesVisual);
				}
			}
			return canvas;
		}
	}
}
