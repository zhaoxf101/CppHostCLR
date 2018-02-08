using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class BubbleChart
	{
		private static Storyboard ApplyBubbleChartAnimation(DataSeries currentDataSeries, Panel bubbleGrid, Storyboard storyboard, double width, double height)
		{
			TranslateTransform translateTransform = new TranslateTransform
			{
				X = 0.0,
				Y = -height
			};
			bubbleGrid.RenderTransform = translateTransform;
			Random random = new Random((int)DateTime.Now.Ticks);
			double num = random.NextDouble();
			double num2 = width / 5.0;
			double[] array = new double[10];
			array[0] = 1.0;
			array[1] = 0.65217391304347827;
			array[2] = 0.47826086956521741;
			array[3] = 0.30434782608695654;
			array[4] = 0.21739130434782608;
			array[5] = 0.13043478260869565;
			array[6] = 0.086956521739130432;
			array[7] = 0.043478260869565216;
			array[8] = 0.021739130434782608;
			DoubleCollection doubleCollection = Graphics.GenerateDoubleCollection(array);
			DoubleCollection doubleCollection2 = new DoubleCollection();
			double num3;
			if (random.NextDouble() > 0.5)
			{
				if ((double)bubbleGrid.GetValue(Canvas.LeftProperty) > width / 2.0)
				{
					num3 = -1.0;
				}
				else
				{
					num3 = 1.0;
				}
			}
			else if ((double)bubbleGrid.GetValue(Canvas.LeftProperty) > width / 2.0)
			{
				num3 = 1.0;
			}
			else
			{
				num3 = -1.0;
			}
			foreach (double num4 in doubleCollection)
			{
				doubleCollection2.Add(num3 * num2 * num4);
			}
			double[] array2 = new double[10];
			array2[0] = -height;
			array2[2] = -height * 0.5;
			array2[4] = -height * 0.25;
			array2[6] = -height * 0.125;
			array2[8] = -height * 0.0625;
			DoubleCollection values = Graphics.GenerateDoubleCollection(array2);
			double num5 = 0.5;
			DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
			{
				0.1,
				0.2,
				0.3,
				0.4,
				0.5,
				0.6,
				0.7,
				0.8,
				0.9,
				1.0
			});
			DoubleCollection frameTime2 = Graphics.GenerateDoubleCollection(new double[]
			{
				0.1,
				0.2,
				0.3,
				0.4,
				0.5,
				0.6,
				0.7,
				0.8,
				0.9,
				1.0
			});
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0)
			});
			List<KeySpline> splines2 = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.5, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(0.5, 1.0),
				new Point(0.5, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(0.5, 1.0),
				new Point(0.5, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(0.5, 1.0),
				new Point(0.5, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(0.5, 1.0),
				new Point(0.5, 0.0),
				new Point(1.0, 1.0)
			});
			DoubleAnimationUsingKeyFrames value = AnimationHelper.CreateDoubleAnimation(currentDataSeries, translateTransform, "(TranslateTransform.X)", num * 0.5 + num5, frameTime, doubleCollection2, splines);
			DoubleAnimationUsingKeyFrames value2 = AnimationHelper.CreateDoubleAnimation(currentDataSeries, translateTransform, "(TranslateTransform.Y)", num * 0.5 + num5, frameTime2, values, splines2);
			storyboard.Children.Add(value);
			storyboard.Children.Add(value2);
			return storyboard;
		}

		private static void CalculateMaxAndMinZValueFromAllSeries(ref List<DataSeries> seriesList, out double minimumZVal, out double maximumZVal)
		{
			List<double> list = new List<double>();
			List<double> list2 = new List<double>();
			minimumZVal = 0.0;
			maximumZVal = 1.0;
			foreach (DataSeries current in seriesList)
			{
				if (!(current.Enabled == false))
				{
					BubbleChart.CalculateMaxAndMinZValue(current, out minimumZVal, out maximumZVal);
					list.Add(minimumZVal);
					list2.Add(maximumZVal);
				}
			}
			if (list.Count > 0)
			{
				minimumZVal = list.Min();
			}
			if (list2.Count > 0)
			{
				maximumZVal = list2.Max();
			}
		}

		internal static Canvas GetVisualObjectForBubbleChart(Panel preExistingPanel, double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, double plankDepth, bool animationEnabled)
		{
			if (double.IsNaN(width) || double.IsNaN(height) || width <= 0.0 || height <= 0.0)
			{
				return null;
			}
			Canvas canvas;
			Canvas canvas2;
			RenderHelper.PrepareCanvas4Drawing(preExistingPanel as Canvas, out canvas, out canvas2, width, height);
			double num = plankDepth / (double)((plotDetails.Layer3DCount == 0) ? 1 : plotDetails.Layer3DCount) * (double)(chart.View3D ? 1 : 0);
			double num2 = num * (double)(plotDetails.SeriesDrawingIndex[seriesList[0]] + 1 - ((plotDetails.Layer3DCount == 0) ? 0 : 1));
			canvas.SetValue(Canvas.TopProperty, num2);
			canvas.SetValue(Canvas.LeftProperty, -num2);
			double minimumZVal;
			double maximumZVal;
			BubbleChart.CalculateMaxAndMinZValueFromAllSeries(ref seriesList, out minimumZVal, out maximumZVal);
			foreach (DataSeries current in seriesList)
			{
				Faces faces = new Faces
				{
					Visual = canvas2
				};
				current.Faces = faces;
				if (!(current.Enabled == false))
				{
					PlotGroup arg_10F_0 = current.PlotGroup;
					List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(current, false, false);
					foreach (IDataPoint current2 in dataPointsUnderViewPort)
					{
						if (current2.Parent != null)
						{
							BubbleChart.CreateOrUpdateAPointDataPoint(canvas2, current2, minimumZVal, maximumZVal, width, height);
							if (current2.Parent.PlotGroup.AxisY.AxisMinimum != null)
							{
								double limitingYValue = current2.Parent.PlotGroup.GetLimitingYValue();
								if (current2.DpInfo.InternalYValue > current2.Parent.PlotGroup.AxisY.InternalAxisMaximum || (current2.DpInfo.InternalYValue < limitingYValue && limitingYValue > 0.0) || (current2.DpInfo.InternalYValue < limitingYValue && current2.Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || current2.DpInfo.InternalYValue < current2.Parent.PlotGroup.AxisY.InternalAxisMinimum)
								{
									current2.DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
								}
							}
							if (animationEnabled && current2.DpInfo.Marker != null)
							{
								if (current2.Parent.Storyboard == null)
								{
									current2.Parent.Storyboard = new Storyboard();
								}
								current2.Parent.Storyboard = BubbleChart.ApplyBubbleChartAnimation(current2.Parent, current2.DpInfo.Marker.Visual, current2.Parent.Storyboard, width, height);
							}
						}
					}
				}
			}
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			rectangleGeometry.Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_OFFSET, height + chart.ChartArea.PLANK_DEPTH);
			canvas.Clip = rectangleGeometry;
			return canvas;
		}

		private static void CreateOrUpdateAPointDataPoint(Canvas bubleChartCanvas, IDataPoint dataPoint, double minimumZVal, double maximumZVal, double plotWidth, double plotHeight)
		{
			Faces faces = dataPoint.DpInfo.Faces;
			if (faces != null && faces.Visual != null && bubleChartCanvas == faces.Visual.Parent)
			{
				bubleChartCanvas.Children.Remove(dataPoint.DpInfo.Faces.Visual);
			}
			dataPoint.DpInfo.Faces = null;
			double internalYValue = dataPoint.DpInfo.InternalYValue;
			if (double.IsNaN(internalYValue) || dataPoint.Enabled == false || dataPoint.Parent == null)
			{
				return;
			}
			Chart chart = dataPoint.Chart as Chart;
			PlotGroup plotGroup = dataPoint.Parent.PlotGroup;
			new Canvas();
			Faces faces2 = new Faces();
			double num = Graphics.ValueToPixelPosition(0.0, plotWidth, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
			double num2 = Graphics.ValueToPixelPosition(plotHeight, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, internalYValue);
			LabelStyles labelStyle = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			double num3 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelAngle);
			Brush labelFontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
			FontStyle fontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
			Brush textBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
			FontFamily fontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
			double fontSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontSize);
			FontWeight fontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
			double num4 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
			double num5 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
			MarkerTypes markerType = (MarkerTypes)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerType);
			bool arg_1BD_0 = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled);
			double num6 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity);
			string arg_1DA_0 = (string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href);
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled);
			bool shadowEnabled = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled);
			Thickness thickness = (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderThickness);
			Brush brush = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor);
			Brush brush2 = dataPoint.Color;
			if (dataPoint.Parent.BubbleStyle == BubbleStyles.Style2)
			{
				brush2 = Graphics.Get3DBrushLighting(brush2, flag);
			}
			else
			{
				brush2 = (flag ? Graphics.GetLightingEnabledBrush(brush2, "Linear", null) : brush2);
			}
			string text = ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled)) ? dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText)) : "";
			Marker marker = new Marker();
			marker.AssignPropertiesValue(markerType, 1.0, new Size(6.0, 6.0), false, brush2, text);
			dataPoint.DpInfo.Marker = marker;
			BubbleChart.ApplyZValue(dataPoint, minimumZVal, maximumZVal, plotWidth, plotHeight);
			if (VisifireControl.IsMediaEffectsEnabled)
			{
				if ((Effect)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect) == null)
				{
					marker.ShadowEnabled = shadowEnabled;
				}
				else
				{
					marker.Effect = (Effect)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect);
				}
			}
			else
			{
				marker.ShadowEnabled = shadowEnabled;
			}
			if (brush != null)
			{
				marker.BorderColor = brush;
			}
			marker.TextBackground = textBackground;
			marker.BorderThickness = thickness.Left;
			marker.TextAlignmentX = AlignmentX.Center;
			marker.TextAlignmentY = AlignmentY.Center;
			marker.Tag = new ElementData
			{
				Element = dataPoint
			};
			double num7 = num5 * num4 / 2.0;
			if (!string.IsNullOrEmpty(text))
			{
				marker.FontColor = Chart.CalculateDataPointLabelFontColor(chart, dataPoint, labelFontColor, LabelStyles.OutSide);
				marker.FontSize = fontSize;
				marker.FontWeight = fontWeight;
				marker.FontFamily = fontFamily;
				marker.FontStyle = fontStyle;
				marker.TextBackground = textBackground;
				marker.TextAlignmentX = AlignmentX.Center;
				marker.TextAlignmentY = AlignmentY.Center;
				if (!double.IsNaN(num3) && num3 != 0.0)
				{
					marker.LabelAngle = num3;
					marker.TextOrientation = Orientation.Vertical;
					marker.TextAlignmentX = AlignmentX.Center;
					marker.TextAlignmentY = AlignmentY.Center;
					marker.LabelStyle = labelStyle;
				}
				marker.CreateVisual((chart != null) ? chart.FlowDirection : FlowDirection.LeftToRight);
				if (double.IsNaN(num3) || num3 == 0.0)
				{
					if (num2 - num7 < 0.0 && num2 - marker.TextBlockSize.Height / 2.0 < 0.0)
					{
						marker.TextAlignmentY = AlignmentY.Bottom;
					}
					else if (num2 + num7 > plotHeight && num2 + marker.TextBlockSize.Height / 2.0 > plotHeight)
					{
						marker.TextAlignmentY = AlignmentY.Top;
					}
					if (num - num7 < 0.0 && num - marker.TextBlockSize.Width / 2.0 < 0.0)
					{
						marker.TextAlignmentX = AlignmentX.Right;
					}
					else if (num + num7 > plotWidth && num + marker.TextBlockSize.Width / 2.0 > plotWidth)
					{
						marker.TextAlignmentX = AlignmentX.Left;
					}
				}
			}
			if (!VisifireControl.IsMediaEffectsEnabled)
			{
				marker.PixelLevelShadow = false;
			}
			else
			{
				marker.PixelLevelShadow = true;
			}
			marker.CreateVisual((chart != null) ? chart.FlowDirection : FlowDirection.LeftToRight);
			BubbleChart.UpdateBubblePositionAccording2XandYValue(dataPoint, plotWidth, plotHeight, false, 0.0, 0.0);
			bubleChartCanvas.Children.Add(marker.Visual);
			faces2.Parts.Add(marker.MarkerShape);
			faces2.VisualComponents.Add(marker.Visual);
			faces2.BorderElements.Add(marker.MarkerShape);
			faces2.Visual = marker.Visual;
			dataPoint.DpInfo.Faces = faces2;
			dataPoint.DpInfo.Faces.Visual.Opacity = num6 * dataPoint.Parent.Opacity;
			if (!dataPoint.IsLightDataPoint)
			{
				PointChart.AttachEvents2Visual(dataPoint as DataPoint, dataPoint);
			}
			PointChart.AttachEvents2Visual(dataPoint.Parent, dataPoint);
			if (!chart.IndicatorEnabled)
			{
				dataPoint.AttachToolTip(chart, dataPoint, dataPoint.DpInfo.Faces.VisualComponents);
			}
			if (!dataPoint.IsLightDataPoint)
			{
				(dataPoint as DataPoint).AttachHref(chart, dataPoint.DpInfo.Faces.VisualComponents, (dataPoint as DataPoint).ParsedHref, (HrefTargets)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.HrefTarget));
			}
			DataPointHelper.SetCursor2DataPointVisualFaces(dataPoint);
			if (!dataPoint.IsLightDataPoint && dataPoint.Parent.SelectionEnabled && (dataPoint as DataPoint).Selected)
			{
				DataPointHelper.Select(dataPoint, true);
			}
		}

		internal static void UpdateBubblePositionAccording2XandYValue(IDataPoint dataPoint, double drawingAreaWidth, double drawingAreaHeight, bool animatedUpdate, double oldSize, double newSize)
		{
			dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
			if (!dataPoint.IsLightDataPoint)
			{
				(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
			}
			Marker marker = dataPoint.DpInfo.Marker;
			PlotGroup plotGroup = dataPoint.Parent.PlotGroup;
			double num = Graphics.ValueToPixelPosition(0.0, drawingAreaWidth, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
			double num2 = Graphics.ValueToPixelPosition(drawingAreaHeight, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, dataPoint.DpInfo.InternalYValue);
			if (animatedUpdate)
			{
				Point newPosition = marker.CalculateActualPosition(num, num2, new Point(0.5, 0.5));
				BubbleChart.ApplyAnimation4XYZUpdate(dataPoint, newPosition, oldSize, newSize);
			}
			else
			{
				marker.SetPosition(num, num2, new Point(0.5, 0.5));
			}
			if (dataPoint.Parent.ToolTipElement != null)
			{
				dataPoint.Parent.ToolTipElement.Hide();
			}
			(dataPoint.Chart as Chart).ChartArea.DisableIndicators();
			dataPoint.DpInfo.VisualPosition = new Point(num, num2);
		}

		private static void ApplyAnimation4XYZUpdate(IDataPoint dataPoint, Point newPosition, double oldSize, double newSize)
		{
			Marker marker = dataPoint.DpInfo.Marker;
			FrameworkElement visual = marker.Visual;
			double x;
			double y;
			if (dataPoint.DpInfo.Storyboard != null)
			{
				dataPoint.DpInfo.Storyboard.Pause();
				x = (double)marker.Visual.GetValue(Canvas.LeftProperty);
				y = (double)marker.Visual.GetValue(Canvas.TopProperty);
				dataPoint.DpInfo.Storyboard.Resume();
				dataPoint.DpInfo.Storyboard = null;
			}
			else
			{
				x = (double)marker.Visual.GetValue(Canvas.LeftProperty);
				y = (double)marker.Visual.GetValue(Canvas.TopProperty);
			}
			Point point = new Point(x, y);
			Storyboard storyboard = new Storyboard();
			if (point.X != newPosition.X)
			{
				storyboard = AnimationHelper.ApplyPropertyAnimation(visual, "(Canvas.Left)", dataPoint, storyboard, 0.0, new double[]
				{
					0.0,
					1.0
				}, new double[]
				{
					point.X,
					newPosition.X
				}, null);
			}
			if (point.Y != newPosition.Y)
			{
				storyboard = AnimationHelper.ApplyPropertyAnimation(visual, "(Canvas.Top)", dataPoint, storyboard, 0.0, new double[]
				{
					0.0,
					1.0
				}, new double[]
				{
					point.Y,
					newPosition.Y
				}, null);
			}
			if (dataPoint.Parent.RenderAs == RenderAs.Bubble && oldSize != newSize)
			{
				storyboard = AnimationHelper.ApplyPropertyAnimation(marker.MarkerShape, "Height", dataPoint, storyboard, 0.0, new double[]
				{
					0.0,
					1.0
				}, new double[]
				{
					oldSize,
					newSize
				}, null);
				storyboard = AnimationHelper.ApplyPropertyAnimation(marker.MarkerShape, "Width", dataPoint, storyboard, 0.0, new double[]
				{
					0.0,
					1.0
				}, new double[]
				{
					oldSize,
					newSize
				}, null);
				if (marker.MarkerShadow != null)
				{
					storyboard = AnimationHelper.ApplyPropertyAnimation(marker.MarkerShadow, "Height", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new double[]
					{
						oldSize,
						newSize
					}, null);
					storyboard = AnimationHelper.ApplyPropertyAnimation(marker.MarkerShadow, "Width", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new double[]
					{
						oldSize,
						newSize
					}, null);
				}
			}
			Random random = new Random(DateTime.Now.Millisecond);
			storyboard.SpeedRatio = 2.0 - random.NextDouble();
			dataPoint.DpInfo.Storyboard = storyboard;
			storyboard.Begin(dataPoint.Chart._rootElement, true);
		}

		private static void CalculateMaxAndMinZValue(DataSeries series, out double minimumZVal, out double maximumZVal)
		{
			IEnumerable<double> source = from dp in series.InternalDataPoints
			where !double.IsNaN(dp.ZValue) && dp.Enabled == true
			select dp.ZValue;
			minimumZVal = 0.0;
			maximumZVal = 1.0;
			if (source.Any<double>())
			{
				minimumZVal = source.Min();
				maximumZVal = source.Max();
			}
		}

		internal static Point[] CalculateControlPointsOfADataSeries(Chart chart, PlotGroup plotGroup, List<IDataPoint> dataPoints)
		{
			List<Point> list = new List<Point>();
			if (chart != null && chart.InternalSeries != null && plotGroup != null && dataPoints.Count > 0)
			{
				List<DataSeries> internalSeries = chart.InternalSeries;
				double minimumZVal;
				double maximumZVal;
				BubbleChart.CalculateMaxAndMinZValueFromAllSeries(ref internalSeries, out minimumZVal, out maximumZVal);
				double arg_41_0 = plotGroup._initialAxisXMin;
				double arg_48_0 = plotGroup._initialAxisXMax;
				dataPoints = (from a in dataPoints
				orderby a.ZValue
				select a).ToList<IDataPoint>();
				IDataPoint dataPoint = dataPoints.Last<IDataPoint>();
				double maxBubbleRadius = BubbleChart.CalculateBubbleSize(dataPoint, minimumZVal, maximumZVal);
				foreach (IDataPoint current in dataPoints)
				{
					List<Point> collection = BubbleChart.CalculateControlPointsOfDataPoint(current, plotGroup, maxBubbleRadius, minimumZVal, maximumZVal, plotGroup.AxisX, plotGroup.AxisY);
					list.InsertRange(0, collection);
				}
				if (list.Count > 0)
				{
					list = (from x in list
					orderby x.X
					select x).ToList<Point>();
				}
			}
			return list.ToArray();
		}

		private static List<Point> CalculateControlPointsOfDataPoint(IDataPoint dataPoint, PlotGroup plotGroup, double maxBubbleRadius, double minimumZVal, double maximumZVal, Axis axisX, Axis axisY)
		{
			Chart chart = dataPoint.Chart as Chart;
			List<Point> list = new List<Point>();
			if (chart != null && axisX != null && axisY != null)
			{
				double num = 4.0;
				double num2 = 4.0;
				Size size = new Size(plotGroup._intialAxisXWidth, plotGroup._intialAxisYHeight);
				double num3 = Graphics.ValueToPixelPosition(0.0, size.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, dataPoint.DpInfo.InternalXValue);
				double num4 = Graphics.ValueToPixelPosition(size.Height, 0.0, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, dataPoint.DpInfo.InternalYValue);
				if (axisY.Logarithmic)
				{
					double logValue = Graphics.PixelPositionToValue(size.Height, 0.0, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, num4);
					double y = DataPointHelper.ConvertLogarithmicValue2ActualValue(chart, logValue, axisY.AxisType);
					Point item = new Point(Graphics.PixelPositionToValue(0.0, size.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, num3 - (maxBubbleRadius + num)), y);
					Point item2 = new Point(Graphics.PixelPositionToValue(0.0, size.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, num3 + (maxBubbleRadius + num)), y);
					logValue = Graphics.PixelPositionToValue(size.Height, 0.0, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, num4 - (maxBubbleRadius + num2));
					y = DataPointHelper.ConvertLogarithmicValue2ActualValue(chart, logValue, axisY.AxisType);
					Point item3 = new Point(Graphics.PixelPositionToValue(0.0, size.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, num3), y);
					logValue = Graphics.PixelPositionToValue(size.Height, 0.0, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, num4 + (maxBubbleRadius + num2));
					y = DataPointHelper.ConvertLogarithmicValue2ActualValue(chart, logValue, axisY.AxisType);
					Point item4 = new Point(Graphics.PixelPositionToValue(0.0, size.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, num3), y);
					list.Add(item);
					list.Add(item2);
					list.Add(item3);
					list.Add(item4);
				}
				else
				{
					Point item5 = new Point(Graphics.PixelPositionToValue(0.0, size.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, num3 - (maxBubbleRadius + num)), Graphics.PixelPositionToValue(0.0, size.Height, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, num4));
					Point item6 = new Point(Graphics.PixelPositionToValue(0.0, size.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, num3 + (maxBubbleRadius + num)), Graphics.PixelPositionToValue(0.0, size.Height, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, num4));
					Point item7 = new Point(Graphics.PixelPositionToValue(0.0, size.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, num3), Graphics.PixelPositionToValue(size.Height, 0.0, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, num4 - (maxBubbleRadius + num2)));
					Point item8 = new Point(Graphics.PixelPositionToValue(0.0, size.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, num3), Graphics.PixelPositionToValue(size.Height, 0.0, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, num4 + (maxBubbleRadius + num2)));
					list.Add(item5);
					list.Add(item6);
					list.Add(item7);
					list.Add(item8);
				}
			}
			return list;
		}

		private static double CalculateBubbleSize(IDataPoint dataPoint, double minimumZVal, double maximumZVal)
		{
			double num = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
			double num2 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
			double fromValue = (!double.IsNaN(dataPoint.ZValue)) ? dataPoint.ZValue : ((minimumZVal + maximumZVal) / 2.0);
			double num3 = Graphics.ConvertScale(minimumZVal, maximumZVal, fromValue, 1.0, num2);
			Size size = new Size(num, num);
			double num4 = num3 * num2;
			return size.Height * num4;
		}

		private static void ApplyZValue(IDataPoint dataPoint, double minimumZVal, double maximumZVal, double drawingAreaWidth, double drawingAreaHeight)
		{
			bool flag = true;
			double num = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
			double num2 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
			double num3 = num2;
			double fromValue = (!double.IsNaN(dataPoint.ZValue)) ? dataPoint.ZValue : ((minimumZVal + maximumZVal) / 2.0);
			double num4 = Graphics.ConvertScale(minimumZVal, maximumZVal, fromValue, 1.0, num3);
			Marker marker = dataPoint.DpInfo.Marker;
			marker.ScaleFactor = num4 * num3;
			marker.MarkerSize = new Size(num, num);
			if (marker.MarkerShape != null)
			{
				double num5 = marker.MarkerSize.Height * marker.ScaleFactor;
				if (flag)
				{
					double height;
					if (dataPoint.DpInfo.Storyboard != null)
					{
						dataPoint.DpInfo.Storyboard.Pause();
						height = marker.MarkerShape.Height;
						dataPoint.DpInfo.Storyboard.Resume();
					}
					else
					{
						height = marker.MarkerShape.Height;
					}
					marker.MarkerShape.Width = (marker.MarkerShape.Height = num5);
					if (marker.MarkerShadow != null)
					{
						marker.MarkerShadow.Width = (marker.MarkerShadow.Height = num5);
					}
					BubbleChart.UpdateBubblePositionAccording2XandYValue(dataPoint, drawingAreaWidth, drawingAreaHeight, flag, height, num5);
					marker.MarkerShape.Width = (marker.MarkerShape.Height = height);
					if (marker.MarkerShadow != null)
					{
						marker.MarkerShadow.Width = (marker.MarkerShadow.Height = height);
						return;
					}
				}
				else
				{
					marker.MarkerShadow.Width = (marker.MarkerShadow.Height = num5);
					if (marker.MarkerShadow != null)
					{
						marker.MarkerShadow.Width = (marker.MarkerShadow.Height = num5);
					}
				}
			}
		}

		public static void Update(IElement sender, ElementTypes elementType, VcProperties property, object newValue, bool isAxisChanged)
		{
			bool flag = elementType == ElementTypes.DataPoint;
			if (flag)
			{
				BubbleChart.UpdateDataPoint(sender as IDataPoint, property, newValue, isAxisChanged);
				return;
			}
			BubbleChart.UpdateDataSeries(sender as DataSeries, property, newValue, isAxisChanged);
		}

		private static void UpdateDataSeries(DataSeries dataSeries, VcProperties property, object newValue, bool isAxisChanged)
		{
			if (dataSeries == null)
			{
				return;
			}
			Chart chart = dataSeries.Chart as Chart;
			if (chart == null)
			{
				return;
			}
			if (property == VcProperties.DataPoints)
			{
				chart.ChartArea.RenderSeries();
				return;
			}
			List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(dataSeries, false, false);
			foreach (IDataPoint current in dataPointsUnderViewPort)
			{
				BubbleChart.UpdateDataPoint(current, property, newValue, isAxisChanged);
			}
		}

		internal static void Update(Chart chart, RenderAs currentRenderAs, List<DataSeries> selectedDataSeries4Rendering, VcProperties property, object newValue)
		{
			bool arg_06_0 = chart.View3D;
			ChartArea chartArea = chart.ChartArea;
			Canvas chartVisualCanvas = chart.ChartArea.ChartVisualCanvas;
			Panel panel = null;
			Dictionary<RenderAs, Panel> renderedCanvasList = chart.ChartArea.RenderedCanvasList;
			if (chartArea.RenderedCanvasList.ContainsKey(currentRenderAs))
			{
				panel = renderedCanvasList[currentRenderAs];
			}
			Panel panel2 = chartArea.RenderSeriesFromList(panel, selectedDataSeries4Rendering);
			if (panel == null)
			{
				chartArea.RenderedCanvasList.Add(currentRenderAs, panel2);
				chartVisualCanvas.Children.Add(panel2);
			}
		}

		private static void UpdateDataPoint(IDataPoint dataPoint, VcProperties property, object newValue, bool isAxisChanged)
		{
			Chart chart = dataPoint.Chart as Chart;
			if (chart == null)
			{
				return;
			}
			PlotDetails arg_16_0 = chart.PlotDetails;
			Marker marker = dataPoint.DpInfo.Marker;
			DataSeries parent = dataPoint.Parent;
			if (parent.Faces == null)
			{
				return;
			}
			Canvas canvas = parent.Faces.Visual as Canvas;
			double height = chart.ChartArea.ChartVisualCanvas.Height;
			double width = chart.ChartArea.ChartVisualCanvas.Width;
			RenderHelper.UpdateParentVisualCanvasSize(chart, canvas);
			if (property == VcProperties.Enabled || ((dataPoint.DpInfo.Faces == null || double.IsNaN(dataPoint.DpInfo.InternalYValue)) && (property == VcProperties.XValue || property == VcProperties.YValue)))
			{
				dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
				if (!dataPoint.IsLightDataPoint)
				{
					(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
				}
				List<DataSeries> list = (from ds in chart.InternalSeries
				where ds.RenderAs == RenderAs.Bubble && ds.Enabled.Value
				select ds).ToList<DataSeries>();
				double minimumZVal;
				double maximumZVal;
				BubbleChart.CalculateMaxAndMinZValueFromAllSeries(ref list, out minimumZVal, out maximumZVal);
				BubbleChart.CreateOrUpdateAPointDataPoint(canvas, dataPoint, minimumZVal, maximumZVal, width, height);
				return;
			}
			if (dataPoint.DpInfo.Faces == null)
			{
				return;
			}
			FrameworkElement arg_14E_0 = dataPoint.DpInfo.Faces.Visual;
			if (property > VcProperties.Effect)
			{
				double minimumZVal;
				double maximumZVal;
				if (property <= VcProperties.MarkerType)
				{
					switch (property)
					{
					case VcProperties.Href:
						if (!dataPoint.IsLightDataPoint)
						{
							(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
							DataPointHelper.SetHref2DataPointVisualFaces(dataPoint);
							goto IL_7DA;
						}
						goto IL_7DA;
					case VcProperties.HrefTarget:
						DataPointHelper.SetHref2DataPointVisualFaces(dataPoint);
						goto IL_7DA;
					case VcProperties.Inlines:
					case VcProperties.IncludeZero:
					case VcProperties.InterlacedColor:
					case VcProperties.Interval:
					case VcProperties.IntervalType:
					case VcProperties.IncludeYValueInLegend:
					case VcProperties.IncludePercentageInLegend:
					case VcProperties.IncludeDataPointsInLegend:
						goto IL_7DA;
					case VcProperties.LabelBackground:
						if (marker != null)
						{
							marker.TextBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
							goto IL_7DA;
						}
						goto IL_7DA;
					case VcProperties.LabelEnabled:
						BubbleChart.CalculateMaxAndMinZValue(dataPoint.Parent, out minimumZVal, out maximumZVal);
						BubbleChart.CreateOrUpdateAPointDataPoint(canvas, dataPoint, minimumZVal, maximumZVal, width, height);
						goto IL_7DA;
					case VcProperties.LabelFontColor:
						if (marker != null)
						{
							marker.FontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
							goto IL_7DA;
						}
						goto IL_7DA;
					case VcProperties.LabelFontFamily:
						if (marker != null)
						{
							marker.FontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
							goto IL_7DA;
						}
						goto IL_7DA;
					case VcProperties.LabelFontSize:
						break;
					case VcProperties.LabelFontStyle:
						if (marker != null)
						{
							marker.FontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
							goto IL_7DA;
						}
						goto IL_7DA;
					case VcProperties.LabelFontWeight:
						if (marker != null)
						{
							marker.FontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
							goto IL_7DA;
						}
						goto IL_7DA;
					default:
						switch (property)
						{
						case VcProperties.LabelStyle:
						case VcProperties.LabelText:
							break;
						case VcProperties.LabelAngle:
							if (marker != null)
							{
								marker.FontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
								goto IL_7DA;
							}
							goto IL_7DA;
						case VcProperties.Legend:
						case VcProperties.LegendMarkerColor:
						case VcProperties.LegendMarkerType:
							goto IL_7DA;
						case VcProperties.LegendText:
							chart.InvokeRender();
							goto IL_7DA;
						case VcProperties.LightingEnabled:
							goto IL_45D;
						default:
							switch (property)
							{
							case VcProperties.MarkerBorderColor:
								if (marker != null)
								{
									marker.BorderColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor);
									goto IL_7DA;
								}
								goto IL_7DA;
							case VcProperties.MarkerBorderThickness:
								if (marker != null)
								{
									marker.BorderThickness = ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderThickness)).Left;
									goto IL_7DA;
								}
								goto IL_7DA;
							case VcProperties.MarkerColor:
							case VcProperties.MarkerEnabled:
								goto IL_7DA;
							case VcProperties.MarkerScale:
							case VcProperties.MarkerSize:
							case VcProperties.MarkerType:
								break;
							default:
								goto IL_7DA;
							}
							break;
						}
						break;
					}
				}
				else if (property != VcProperties.Opacity)
				{
					switch (property)
					{
					case VcProperties.ShadowEnabled:
						if (marker != null)
						{
							marker.ShadowEnabled = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled);
							marker.ApplyRemoveShadow();
							goto IL_7DA;
						}
						goto IL_7DA;
					case VcProperties.ShowInLegend:
						chart.InvokeRender();
						goto IL_7DA;
					default:
						switch (property)
						{
						case VcProperties.ToolTipText:
							dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
							goto IL_7DA;
						case VcProperties.To:
						case VcProperties.TrendLines:
						case VcProperties.Value:
						case VcProperties.ValueFormatString:
						case VcProperties.VerticalAlignment:
						case VcProperties.XValues:
						case VcProperties.ZIndex:
							goto IL_7DA;
						case VcProperties.XValue:
						case VcProperties.YValue:
						case VcProperties.YValues:
							goto IL_611;
						case VcProperties.XValueFormatString:
						case VcProperties.YValueFormatString:
							break;
						case VcProperties.XValueType:
							chart.InvokeRender();
							goto IL_7DA;
						case VcProperties.ZValue:
						{
							dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
							if (!dataPoint.IsLightDataPoint)
							{
								(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
							}
							List<DataSeries> list2 = (from ds in chart.InternalSeries
							where ds.RenderAs == RenderAs.Bubble && ds.Enabled.Value
							select ds).ToList<DataSeries>();
							BubbleChart.CalculateMaxAndMinZValueFromAllSeries(ref list2, out minimumZVal, out maximumZVal);
							foreach (DataSeries current in list2)
							{
								foreach (IDataPoint current2 in current.InternalDataPoints)
								{
									if (!double.IsNaN(current2.DpInfo.InternalYValue) && !(current2.Enabled == false))
									{
										BubbleChart.ApplyZValue(current2, minimumZVal, maximumZVal, width, height);
									}
								}
							}
							goto IL_7DA;
						}
						default:
							goto IL_7DA;
						}
						break;
					}
				}
				else
				{
					if (marker != null)
					{
						marker.Visual.Opacity = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity) * parent.Opacity;
						goto IL_7DA;
					}
					goto IL_7DA;
				}
				dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
				if (!dataPoint.IsLightDataPoint)
				{
					(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
				}
				BubbleChart.CalculateMaxAndMinZValue(dataPoint.Parent, out minimumZVal, out maximumZVal);
				BubbleChart.CreateOrUpdateAPointDataPoint(canvas, dataPoint, minimumZVal, maximumZVal, width, height);
				goto IL_7DA;
			}
			if (property <= VcProperties.Color)
			{
				if (property == VcProperties.Bevel)
				{
					goto IL_7DA;
				}
				switch (property)
				{
				case VcProperties.BubbleStyle:
					if (marker == null)
					{
						goto IL_7DA;
					}
					if (dataPoint.Parent.BubbleStyle == BubbleStyles.Style2)
					{
						marker.MarkerShape.Fill = Graphics.Get3DBrushLighting(dataPoint.Color, (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled));
						goto IL_7DA;
					}
					marker.MarkerShape.Fill = (((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) ? Graphics.GetLightingEnabledBrush(dataPoint.Color, "Linear", null) : dataPoint.Color);
					goto IL_7DA;
				case VcProperties.Color:
					break;
				default:
					goto IL_7DA;
				}
			}
			else
			{
				if (property == VcProperties.Cursor)
				{
					goto IL_7DA;
				}
				if (property == VcProperties.DataPoints)
				{
					goto IL_611;
				}
				if (property != VcProperties.Effect)
				{
					goto IL_7DA;
				}
				if (marker != null && VisifireControl.IsMediaEffectsEnabled)
				{
					marker.Effect = (Effect)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect);
					marker.ApplyEffects();
					goto IL_7DA;
				}
				goto IL_7DA;
			}
			IL_45D:
			if (marker != null)
			{
				marker.MarkerShape.Fill = (chart.View3D ? Graphics.Get3DBrushLighting(dataPoint.Color, (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) : (((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) ? Graphics.GetLightingEnabledBrush(dataPoint.Color, "Linear", null) : dataPoint.Color));
				goto IL_7DA;
			}
			goto IL_7DA;
			IL_611:
			if (isAxisChanged)
			{
				BubbleChart.UpdateDataSeries(parent, property, newValue, false);
			}
			else if (marker != null)
			{
				dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
				if (!dataPoint.IsLightDataPoint)
				{
					(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
				}
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled))
				{
					marker.Text = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText));
				}
				BubbleChart.UpdateBubblePositionAccording2XandYValue(dataPoint, width, height, chart.AnimatedUpdate.Value, marker.MarkerShape.Width, marker.MarkerShape.Width);
			}
			IL_7DA:
			List<IDataPoint> list3 = (from datapoint in chart.PlotDetails.ListOfAllDataPoints
			where datapoint.Enabled == true
			select datapoint).ToList<IDataPoint>();
			for (int i = 0; i < list3.Count; i++)
			{
				if (list3[i].Parent.PlotGroup.AxisY.AxisMinimum != null && !double.IsNaN(list3[i].DpInfo.InternalYValue))
				{
					double limitingYValue = list3[i].Parent.PlotGroup.GetLimitingYValue();
					if ((list3[i].DpInfo.InternalYValue < limitingYValue && limitingYValue > 0.0) || (list3[i].DpInfo.InternalYValue < limitingYValue && list3[i].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || list3[i].DpInfo.InternalYValue < list3[i].Parent.PlotGroup.AxisY.InternalAxisMinimum)
					{
						if (list3[i].DpInfo.Marker != null && list3[i].DpInfo.Marker.Visual != null)
						{
							list3[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
						}
					}
					else if (list3[i].DpInfo.Marker != null && list3[i].DpInfo.Marker.Visual != null)
					{
						list3[i].DpInfo.Marker.Visual.Visibility = Visibility.Visible;
					}
				}
			}
			if (canvas.Parent != null)
			{
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = new Rect(0.0, -(dataPoint.Chart as Chart).ChartArea.PLANK_DEPTH, width + (dataPoint.Chart as Chart).ChartArea.PLANK_OFFSET, height + (dataPoint.Chart as Chart).ChartArea.PLANK_DEPTH);
				(canvas.Parent as Canvas).Clip = rectangleGeometry;
			}
		}
	}
}
