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
	internal class RadarChart
	{
		private static void CalculateRadarPoints(DataSeries series, ref List<Point> listOfRadarPoints, PlotGroup plotGroup, CircularPlotDetails circularPlotDetails)
		{
			IDataPoint dataPoint = null;
			IDataPoint dataPoint2 = series.InternalDataPoints[0];
			IDataPoint dataPoint3 = series.InternalDataPoints[series.InternalDataPoints.Count - 1];
			for (int i = 0; i < series.InternalDataPoints.Count - 1; i++)
			{
				IDataPoint dataPoint4 = series.InternalDataPoints[i];
				double dataPointIndex = (double)series.InternalDataPoints.IndexOf(dataPoint4);
				if (dataPoint4.Enabled.Value)
				{
					IDataPoint dataPoint5 = series.InternalDataPoints[i + 1];
					Point radarPoint = RadarChart.GetRadarPoint(circularPlotDetails, plotGroup, dataPoint4, dataPointIndex);
					double internalYValue = dataPoint4.DpInfo.InternalYValue;
					double internalYValue2 = dataPoint5.DpInfo.InternalYValue;
					double internalYValue3 = dataPoint3.DpInfo.InternalYValue;
					if (!double.IsNaN(internalYValue) && (internalYValue > plotGroup.AxisY.InternalAxisMaximum || internalYValue < plotGroup.AxisY.InternalAxisMinimum))
					{
						dataPoint4.DpInfo.VisualPosition = new Point(double.NaN, double.NaN);
						listOfRadarPoints.Add(new Point(circularPlotDetails.Center.X, circularPlotDetails.Center.Y));
					}
					else if (dataPoint4 == dataPoint2 && dataPoint5 != null && double.IsNaN(internalYValue2) && dataPoint3 != null && !double.IsNaN(internalYValue3))
					{
						listOfRadarPoints.Add(radarPoint);
						dataPoint4.DpInfo.VisualPosition = radarPoint;
					}
					else if (!double.IsNaN(internalYValue) && ((dataPoint != null && !double.IsNaN(dataPoint.DpInfo.InternalYValue)) || (dataPoint5 != null && !double.IsNaN(internalYValue2))))
					{
						listOfRadarPoints.Add(radarPoint);
						dataPoint4.DpInfo.VisualPosition = radarPoint;
					}
					else
					{
						dataPoint4.DpInfo.VisualPosition = new Point(double.NaN, double.NaN);
						listOfRadarPoints.Add(new Point(circularPlotDetails.Center.X, circularPlotDetails.Center.Y));
					}
					dataPoint = series.InternalDataPoints[i];
					double internalYValue4 = dataPoint.DpInfo.InternalYValue;
					if (i == series.InternalDataPoints.Count - 2)
					{
						dataPointIndex = (double)series.InternalDataPoints.IndexOf(dataPoint5);
						radarPoint = RadarChart.GetRadarPoint(circularPlotDetails, plotGroup, dataPoint5, dataPointIndex);
						if (!double.IsNaN(internalYValue2) && (internalYValue2 > plotGroup.AxisY.InternalAxisMaximum || internalYValue2 < plotGroup.AxisY.InternalAxisMinimum))
						{
							dataPoint5.DpInfo.VisualPosition = new Point(double.NaN, double.NaN);
							listOfRadarPoints.Add(new Point(circularPlotDetails.Center.X, circularPlotDetails.Center.Y));
						}
						else if ((!double.IsNaN(internalYValue2) && dataPoint != null && !double.IsNaN(internalYValue4)) || (dataPoint3 != null && !double.IsNaN(internalYValue3)))
						{
							listOfRadarPoints.Add(radarPoint);
							dataPoint5.DpInfo.VisualPosition = radarPoint;
						}
						else
						{
							dataPoint5.DpInfo.VisualPosition = new Point(double.NaN, double.NaN);
							listOfRadarPoints.Add(new Point(circularPlotDetails.Center.X, circularPlotDetails.Center.Y));
						}
						if (series.InternalDataPoints.Count < circularPlotDetails.ListOfPoints4CircularAxis.Count)
						{
							listOfRadarPoints.Add(new Point(circularPlotDetails.Center.X, circularPlotDetails.Center.Y));
						}
					}
				}
			}
		}

		private static void DrawMarkers(DataSeries series, Canvas labelCanvas, Chart chart, double width, double height, Point center)
		{
			foreach (IDataPoint current in series.InternalDataPoints)
			{
				if (!double.IsNaN(current.DpInfo.VisualPosition.X) && !double.IsNaN(current.DpInfo.VisualPosition.Y))
				{
					double num = CircularLabel.ResetMeanAngle(RadarChart.CalculateAngleByCoordinate(current.DpInfo.VisualPosition, center));
					Marker markerForDataPoint;
					if (num > 0.0 && num < 3.1415926535897931)
					{
						markerForDataPoint = RadarChart.GetMarkerForDataPoint(chart, width, height, current.DpInfo.VisualPosition.Y, current, false);
					}
					else
					{
						markerForDataPoint = RadarChart.GetMarkerForDataPoint(chart, width, height, current.DpInfo.VisualPosition.Y, current, true);
					}
					if (markerForDataPoint != null)
					{
						markerForDataPoint.AddToParent(labelCanvas, current.DpInfo.VisualPosition.X, current.DpInfo.VisualPosition.Y, new Point(0.5, 0.5));
					}
				}
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
					RadarChart.SetPositionForDataPointLabel(chart, isPositionTop, dataPoint, yPosition);
					dataPoint.DpInfo.Marker.LabelStyle = labelStyle;
				}
				dataPoint.DpInfo.Marker.CreateVisual((chart != null) ? chart.FlowDirection : FlowDirection.LeftToRight);
				if (double.IsNaN(num) || num == 0.0)
				{
					dataPoint.DpInfo.Marker.TextAlignmentX = AlignmentX.Center;
					RadarChart.SetPositionForDataPointLabel(chart, isPositionTop, dataPoint, yPosition);
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

		private static void DrawDataSeriesPolygon(List<Point> listOfRadarPoints, DataSeries series, Canvas radarCanvas)
		{
			if (listOfRadarPoints.Count > 0)
			{
				Polygon polygon = new Polygon
				{
					Tag = new ElementData
					{
						Element = series,
						VisualElementName = "RadarVisual"
					}
				};
				polygon.Fill = (series.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(series.Color, "Linear", null) : series.Color);
				polygon.IsHitTestVisible = false;
				polygon.Stroke = ((series.BorderColor == null) ? RadarChart.GetDarkerColor(series.Color) : series.BorderColor);
				polygon.StrokeDashArray = ExtendedGraphics.GetDashArray(series.BorderStyle);
				polygon.StrokeThickness = ((series.BorderThickness.Left == 0.0) ? 0.5 : series.BorderThickness.Left);
				polygon.StrokeMiterLimit = 1.0;
				polygon.Opacity = series.Opacity;
				PointCollection pointCollection = new PointCollection();
				foreach (Point current in listOfRadarPoints)
				{
					pointCollection.Add(current);
				}
				Rect bounds = AreaChart.GetBounds(pointCollection);
				polygon.Width = bounds.Width;
				polygon.Height = bounds.Height;
				polygon.SetValue(Canvas.TopProperty, bounds.Y);
				polygon.SetValue(Canvas.LeftProperty, bounds.X);
				polygon.Points = pointCollection;
				polygon.Stretch = Stretch.Fill;
				series.Faces.Visual = polygon;
				radarCanvas.Children.Add(polygon);
			}
		}

		private static Canvas GetDataSeriesVisual(Chart chart, double width, double height, DataSeries series, CircularPlotDetails circularPlotDetails)
		{
			Canvas canvas = new Canvas();
			Canvas canvas2 = new Canvas();
			Canvas canvas3 = new Canvas();
			if (series.Enabled.Value)
			{
				PlotGroup plotGroup = series.PlotGroup;
				List<Point> listOfRadarPoints = new List<Point>();
				series.Faces = new Faces();
				RadarChart.CalculateRadarPoints(series, ref listOfRadarPoints, plotGroup, circularPlotDetails);
				RadarChart.DrawMarkers(series, canvas3, chart, width, height, circularPlotDetails.Center);
				RadarChart.DrawDataSeriesPolygon(listOfRadarPoints, series, canvas2);
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

		private static Brush GetDarkerColor(Brush seriesColor)
		{
			if (seriesColor != null && seriesColor.GetType().Equals(typeof(SolidColorBrush)))
			{
				double brushIntensity = Graphics.GetBrushIntensity(seriesColor);
				Color darkerColor = Graphics.GetDarkerColor(Color.FromArgb(255, (seriesColor as SolidColorBrush).Color.R, (seriesColor as SolidColorBrush).Color.G, (seriesColor as SolidColorBrush).Color.B), brushIntensity);
				return Graphics.CreateSolidColorBrush(darkerColor);
			}
			return seriesColor;
		}

		private static Point GetRadarPoint(CircularPlotDetails circularPlotDetails, PlotGroup plotGroup, IDataPoint dp, double dataPointIndex)
		{
			double value;
			if (double.IsNaN(dp.DpInfo.InternalYValue))
			{
				value = 0.0;
			}
			else
			{
				value = dp.DpInfo.InternalYValue;
			}
			double num = Graphics.ValueToPixelPosition(circularPlotDetails.Radius, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, value);
			double num2 = circularPlotDetails.Radius - num;
			double angle = circularPlotDetails.MinAngleInDegree * dataPointIndex;
			double num3 = AxisLabel.GetRadians(angle) - 1.5707963267948966;
			double x = num2 * Math.Cos(num3) + circularPlotDetails.Center.X;
			double y = num2 * Math.Sin(num3) + circularPlotDetails.Center.Y;
			return new Point(x, y);
		}

		private static double CalculateAngleByCoordinate(Point position, Point center)
		{
			return Math.Atan2(position.Y - center.Y, position.X - center.X);
		}

		private static double GetOpacity4Radar(DataSeries series)
		{
			if (series.GetValue(DataSeries.OpacityProperty) == null)
			{
				return 0.65;
			}
			return series.Opacity;
		}

		internal static Canvas GetVisualObjectForRadarChart(double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, bool isAnimationEnabled)
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
					Canvas dataSeriesVisual = RadarChart.GetDataSeriesVisual(chart, size.Width, size.Height, current, axisX.CircularPlotDetails);
					canvas.Children.Add(dataSeriesVisual);
				}
			}
			return canvas;
		}
	}
}
