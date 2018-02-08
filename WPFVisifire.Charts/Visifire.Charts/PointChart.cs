using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class PointChart
	{
		private static Storyboard ApplyPointChartAnimation(DataSeries currentDataSeries, Panel pointGrid, Storyboard storyboard, double width, double height)
		{
			if (storyboard != null && storyboard.GetValue(Storyboard.TargetProperty) != null)
			{
				storyboard.Stop();
			}
			TransformGroup transformGroup = new TransformGroup();
			ScaleTransform scaleTransform = new ScaleTransform
			{
				ScaleX = 0.0,
				ScaleY = 0.0,
				CenterX = 0.5,
				CenterY = 0.5
			};
			TranslateTransform translateTransform = new TranslateTransform
			{
				X = 0.0,
				Y = 0.0
			};
			transformGroup.Children.Add(scaleTransform);
			transformGroup.Children.Add(translateTransform);
			pointGrid.RenderTransform = transformGroup;
			Random random = new Random((int)DateTime.Now.Ticks);
			double num = random.NextDouble();
			pointGrid.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			double num2 = 0.5;
			DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				1.0,
				0.5,
				1.0
			});
			double[] array = new double[4];
			array[0] = pointGrid.DesiredSize.Width / 2.0;
			array[2] = pointGrid.DesiredSize.Width / 4.0;
			DoubleCollection values2 = Graphics.GenerateDoubleCollection(array);
			double[] array2 = new double[4];
			array2[0] = pointGrid.DesiredSize.Height / 2.0;
			array2[2] = pointGrid.DesiredSize.Height / 4.0;
			DoubleCollection values3 = Graphics.GenerateDoubleCollection(array2);
			DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				0.5,
				0.75,
				1.0
			});
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.5),
				new Point(0.5, 1.0),
				new Point(0.0, 0.5),
				new Point(0.5, 1.0),
				new Point(0.0, 0.5),
				new Point(0.5, 1.0),
				new Point(0.0, 0.5),
				new Point(0.5, 1.0)
			});
			List<KeySpline> splines2 = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.5),
				new Point(0.5, 1.0),
				new Point(0.0, 0.5),
				new Point(0.5, 1.0),
				new Point(0.0, 0.5),
				new Point(0.5, 1.0),
				new Point(0.0, 0.5),
				new Point(0.5, 1.0)
			});
			List<KeySpline> splines3 = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.5),
				new Point(0.5, 1.0),
				new Point(0.0, 0.5),
				new Point(0.5, 1.0),
				new Point(0.0, 0.5),
				new Point(0.5, 1.0),
				new Point(0.0, 0.5),
				new Point(0.5, 1.0)
			});
			List<KeySpline> splines4 = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.5),
				new Point(0.5, 1.0),
				new Point(0.0, 0.5),
				new Point(0.5, 1.0),
				new Point(0.0, 0.5),
				new Point(0.5, 1.0),
				new Point(0.0, 0.5),
				new Point(0.5, 1.0)
			});
			DoubleAnimationUsingKeyFrames value = AnimationHelper.CreateDoubleAnimation(currentDataSeries, scaleTransform, "(ScaleTransform.ScaleX)", num + num2, frameTime, values, splines);
			DoubleAnimationUsingKeyFrames value2 = AnimationHelper.CreateDoubleAnimation(currentDataSeries, scaleTransform, "(ScaleTransform.ScaleY)", num + num2, frameTime, values, splines2);
			DoubleAnimationUsingKeyFrames value3 = AnimationHelper.CreateDoubleAnimation(currentDataSeries, translateTransform, "(TranslateTransform.X)", num + num2, frameTime, values2, splines3);
			DoubleAnimationUsingKeyFrames value4 = AnimationHelper.CreateDoubleAnimation(currentDataSeries, translateTransform, "(TranslateTransform.Y)", num + num2, frameTime, values3, splines4);
			storyboard.Children.Add(value);
			storyboard.Children.Add(value2);
			storyboard.Children.Add(value3);
			storyboard.Children.Add(value4);
			return storyboard;
		}

		private static void CreateOrUpdateARasterPointDataPoint(WriteableBitmapAdapter wba, IDataPoint dataPoint, double width, double height, double scrollOffsetAsPixel, RasterMarkerStyle parentMarkerStyle)
		{
			double internalYValue = dataPoint.DpInfo.InternalYValue;
			if (double.IsNaN(internalYValue))
			{
				return;
			}
			if (!dataPoint.Parent.IsAllDataPointsEnabled && dataPoint.Enabled == false)
			{
				return;
			}
			PlotGroup plotGroup = dataPoint.Parent.PlotGroup;
			dataPoint.DpInfo.VisualPosition = new Point(Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue), Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, internalYValue));
			Point visualPosition = dataPoint.DpInfo.VisualPosition;
			visualPosition.X -= Math.Abs(scrollOffsetAsPixel);
			dataPoint.DpInfo.RasterVisualPosition = visualPosition;
			Brush color = dataPoint.Color;
			parentMarkerStyle.Fill = (color as SolidColorBrush).Color;
			parentMarkerStyle.Stroke = (color as SolidColorBrush).Color;
			RasterRenderEngine.DrawMarkerCentered(wba, dataPoint, parentMarkerStyle, false);
		}

		private static void CreateOrUpdateAPointDataPoint(Canvas pointChartCanvas, IDataPoint dataPoint, double plotAreaWidth, double plotAreaHeight)
		{
			if (dataPoint.DpInfo.Faces != null && dataPoint.DpInfo.Faces.Visual != null && pointChartCanvas == dataPoint.DpInfo.Faces.Visual.Parent)
			{
				pointChartCanvas.Children.Remove(dataPoint.DpInfo.Faces.Visual);
			}
			dataPoint.DpInfo.Faces = null;
			Faces faces = new Faces();
			double internalYValue = dataPoint.DpInfo.InternalYValue;
			if (double.IsNaN(internalYValue) || dataPoint.Enabled == false)
			{
				return;
			}
			Chart chart = dataPoint.Chart as Chart;
			if (chart == null)
			{
				return;
			}
			PlotGroup plotGroup = dataPoint.Parent.PlotGroup;
			double num = Graphics.ValueToPixelPosition(0.0, plotAreaWidth, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
			double num2 = Graphics.ValueToPixelPosition(plotAreaHeight, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, internalYValue);
			if (num > plotAreaWidth || num < 0.0)
			{
				return;
			}
			LabelStyles labelStyle = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			double num3 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelAngle);
			Brush labelFontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
			FontStyle fontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
			Brush textBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
			FontFamily fontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
			double fontSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontSize);
			FontWeight fontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
			double num4 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
			double scaleFactor = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
			MarkerTypes markerType = (MarkerTypes)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerType);
			bool arg_1D5_0 = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled);
			double num5 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity);
			string str = (string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href);
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled);
			bool shadowEnabled = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled);
			Thickness thickness = (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderThickness);
			Brush brush = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor);
			Brush brush2 = chart.View3D ? Graphics.Get3DBrushLighting(dataPoint.Color, flag) : (flag ? Graphics.GetLightingEnabledBrush(dataPoint.Color, "Linear", null) : dataPoint.Color);
			Size markerSize = new Size(num4, num4);
			bool markerBevel = false;
			string text = ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled)) ? dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText)) : "";
			Marker marker = dataPoint.DpInfo.Marker;
			if (marker != null && marker.Visual != null)
			{
				Panel panel = marker.Visual.Parent as Panel;
				if (panel != null)
				{
					panel.Children.Remove(marker.Visual);
				}
			}
			marker = new Marker();
			marker.AssignPropertiesValue(markerType, scaleFactor, markerSize, markerBevel, brush2, text);
			marker.Tag = new ElementData
			{
				Element = dataPoint
			};
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
			marker.PixelLevelShadow = VisifireControl.IsMediaEffectsEnabled;
			marker.BorderThickness = thickness.Left;
			if (marker.MarkerType != MarkerTypes.Cross)
			{
				if (brush != null)
				{
					marker.BorderColor = brush;
				}
			}
			else
			{
				marker.BorderColor = brush2;
			}
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
					if (num2 - marker.TextBlockSize.Height / 2.0 < 0.0)
					{
						marker.TextAlignmentY = AlignmentY.Bottom;
					}
					else if (num2 + marker.TextBlockSize.Height / 2.0 > plotAreaHeight)
					{
						marker.TextAlignmentY = AlignmentY.Top;
					}
					if (num - marker.TextBlockSize.Width / 2.0 < 0.0)
					{
						marker.TextAlignmentX = AlignmentX.Right;
					}
					else if (num + marker.TextBlockSize.Width / 2.0 > plotAreaWidth)
					{
						marker.TextAlignmentX = AlignmentX.Left;
					}
				}
			}
			marker.CreateVisual((chart != null) ? chart.FlowDirection : FlowDirection.LeftToRight);
			marker.Visual.Opacity = num5 * dataPoint.Parent.Opacity;
			marker.AddToParent(pointChartCanvas, num, num2, new Point(0.5, 0.5));
			dataPoint.DpInfo.VisualPosition = new Point(num, num2);
			faces.VisualComponents.Add(marker.Visual);
			faces.Visual = marker.Visual;
			faces.BorderElements.Add(marker.MarkerShape);
			dataPoint.DpInfo.Marker = marker;
			dataPoint.DpInfo.Faces = faces;
			dataPoint.DpInfo.Faces.Visual.Opacity = num5 * dataPoint.Parent.Opacity;
			if (!dataPoint.IsLightDataPoint)
			{
				PointChart.AttachEvents2Visual(dataPoint as DataPoint, dataPoint);
			}
			PointChart.AttachEvents2Visual(dataPoint.Parent, dataPoint);
			dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
			if (!dataPoint.IsLightDataPoint)
			{
				(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser(str);
			}
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

		internal static void AttachEvents2Visual(VisifireElement obj, IDataPoint dataPoint)
		{
			foreach (FrameworkElement current in dataPoint.DpInfo.Faces.VisualComponents)
			{
				dataPoint.AttachEvents2Visual(obj, dataPoint, current);
			}
		}

		internal static Canvas GetVisualObjectForPointChart(Panel preExistingPanel, double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, double plankDepth, bool animationEnabled)
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
			List<IDataPoint> list = new List<IDataPoint>();
			bool indicatorEnabled = chart.IndicatorEnabled;
			foreach (DataSeries current in seriesList)
			{
				Canvas canvas3 = new Canvas
				{
					Height = height,
					Width = width,
					Tag = new ElementData
					{
						Element = current
					}
				};
				canvas2.Children.Add(canvas3);
				current.Faces = new Faces
				{
					Visual = canvas3
				};
				if (!(current.Enabled == false))
				{
					Canvas plotAreaCanvas = chart.ChartArea.PlotAreaCanvas;
					int num3 = (int)plotAreaCanvas.Width;
					int num4 = (int)plotAreaCanvas.Height;
					if (num3 == 0 || num4 == 0)
					{
						return null;
					}
					WriteableBitmapAdapter writeableBitmapAdapter = null;
					double num5 = 0.0;
					if (current.LightWeight.Value)
					{
						writeableBitmapAdapter = new WriteableBitmapAdapter(num3, num4);
						num5 = current.PlotGroup.AxisX.GetAxisVisualLeftAsOffset();
						if (double.IsNaN(num5))
						{
							num5 = 0.0;
						}
					}
					List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(current, false, RenderHelper.IsRasterRenderSupported(current));
					list.InsertRange(list.Count, dataPointsUnderViewPort);
					RasterMarkerStyle rasterMarkerStyle = new RasterMarkerStyle();
					double num6 = current.MarkerSize.Value;
					rasterMarkerStyle.Width = num6;
					rasterMarkerStyle.Height = num6;
					rasterMarkerStyle.MarkerType = current.MarkerType;
					if (((Thickness?)current.GetValue(DataSeries.MarkerBorderThicknessProperty)).HasValue)
					{
						rasterMarkerStyle.StrokeThickness = current.MarkerBorderThickness.Value.Left;
					}
					if (current.MarkerBorderColor != null)
					{
						rasterMarkerStyle.Stroke = ((SolidColorBrush)current.MarkerBorderColor).Color;
					}
					foreach (IDataPoint current2 in dataPointsUnderViewPort)
					{
						if (current2.Parent != null)
						{
							if (writeableBitmapAdapter != null)
							{
								PointChart.CreateOrUpdateARasterPointDataPoint(writeableBitmapAdapter, current2, width, height, num5, rasterMarkerStyle);
								if (indicatorEnabled)
								{
									current2.DpInfo.ParsedToolTipText = current2.TextParser(current2.ToolTipText);
								}
							}
							else
							{
								PointChart.CreateOrUpdateAPointDataPoint(canvas2, current2, width, height);
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
									current2.Parent.Storyboard = PointChart.ApplyPointChartAnimation(current2.Parent, current2.DpInfo.Marker.Visual, current2.Parent.Storyboard, width, height);
								}
							}
						}
					}
					if (writeableBitmapAdapter != null)
					{
						Image image = new Image
						{
							Height = (double)num4,
							Width = (double)num3,
							IsHitTestVisible = false
						};
						image.Source = writeableBitmapAdapter.GetImageSource();
						image.SetValue(Canvas.LeftProperty, Math.Abs(num5));
						canvas3.Children.Add(image);
					}
				}
			}
			PointChart.Clip(chart, list, canvas, width, height);
			return canvas;
		}

		private static void Clip(Chart chart, List<IDataPoint> dataPoints, Canvas visual, double width, double height)
		{
			bool flag = true;
			double num = (from tick in chart.AxesX[0].Ticks
			where chart.AxesX[0].Enabled.Value && tick.Enabled.Value
			select tick.TickLength.Value).Sum();
			if (num == 0.0)
			{
				num = 5.0;
			}
			double num2 = (from axis in chart.AxesY
			where axis.AxisType == AxisTypes.Primary
			from tick in axis.Ticks
			where axis.Enabled.Value && tick.Enabled.Value
			select tick.TickLength.Value).Sum();
			if (num2 == 0.0)
			{
				num2 = 8.0;
			}
			double num3 = (from axis in chart.AxesY
			where axis.AxisType == AxisTypes.Secondary
			from tick in axis.Ticks
			where axis.Enabled.Value && tick.Enabled.Value
			select tick.TickLength.Value).Sum();
			if (num3 == 0.0)
			{
				num3 = 8.0;
			}
			double num4 = (double)(from c in chart.PlotDetails.PlotGroups
			where c.AxisY.AxisType == AxisTypes.Secondary
			select c).Count<PlotGroup>();
			List<IDataPoint> list = (from datapoint in dataPoints
			where datapoint.Enabled == true
			select datapoint).ToList<IDataPoint>();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					double limitingYValue = list[i].Parent.PlotGroup.GetLimitingYValue();
					if (list[i].DpInfo.InternalYValue < limitingYValue)
					{
						flag = false;
						break;
					}
				}
			}
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			if (flag)
			{
				rectangleGeometry.Rect = new Rect(-4.0, -chart.ChartArea.PLANK_DEPTH - 4.0, width + num3 + ((num4 > 0.0) ? num2 : 0.0) + chart.ChartArea.PLANK_OFFSET, height + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + num + 4.0);
			}
			else
			{
				rectangleGeometry.Rect = new Rect(-num2, -chart.ChartArea.PLANK_DEPTH - 4.0, width + num3 + ((num4 > 0.0) ? num2 : 0.0) + chart.ChartArea.PLANK_OFFSET, height + chart.ChartArea.PLANK_DEPTH + num + 4.0);
			}
			visual.Clip = rectangleGeometry;
		}

		public static void Update(IElement sender, ElementTypes elementType, VcProperties property, object newValue, bool isAxisChanged)
		{
			bool flag = elementType == ElementTypes.DataPoint;
			if (flag)
			{
				PointChart.UpdateDataPoint(sender as IDataPoint, property, newValue, isAxisChanged);
				return;
			}
			PointChart.UpdateDataSeries(sender as DataSeries, property, newValue, isAxisChanged);
		}

		private static void UpdateRasterPointSeries(DataSeries series, double width, double height)
		{
			if (series == null)
			{
				return;
			}
			if (series.Faces.Visual == null)
			{
				return;
			}
			Canvas canvas = series.Faces.Visual as Canvas;
			Chart chart = series.Chart as Chart;
			Canvas plotAreaCanvas = chart.ChartArea.PlotAreaCanvas;
			int num = (int)plotAreaCanvas.Width;
			int num2 = (int)plotAreaCanvas.Height;
			if (num == 0 || num2 == 0)
			{
				return;
			}
			WriteableBitmapAdapter writeableBitmapAdapter = new WriteableBitmapAdapter(num, num2);
			List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(series, false, RenderHelper.IsRasterRenderSupported(series));
			bool indicatorEnabled = chart.IndicatorEnabled;
			double num3 = series.PlotGroup.AxisX.GetAxisVisualLeftAsOffset();
			if (double.IsNaN(num3))
			{
				num3 = 0.0;
			}
			RasterMarkerStyle rasterMarkerStyle = new RasterMarkerStyle();
			double num4 = series.MarkerSize.Value;
			rasterMarkerStyle.Width = num4;
			rasterMarkerStyle.Height = num4;
			rasterMarkerStyle.MarkerType = series.MarkerType;
			if (((Thickness?)series.GetValue(DataSeries.MarkerBorderThicknessProperty)).HasValue)
			{
				rasterMarkerStyle.StrokeThickness = series.MarkerBorderThickness.Value.Left;
			}
			foreach (IDataPoint current in dataPointsUnderViewPort)
			{
				if (series.LightWeight.Value)
				{
					PointChart.CreateOrUpdateARasterPointDataPoint(writeableBitmapAdapter, current, width, height, num3, rasterMarkerStyle);
					if (indicatorEnabled)
					{
						current.DpInfo.ParsedToolTipText = current.TextParser(current.ToolTipText);
						if (series.ToolTipElement != null)
						{
							series.ToolTipElement.Hide();
						}
						chart.ChartArea.DisableIndicators();
					}
				}
			}
			if (canvas.Children.Count > 0)
			{
				Image image = canvas.Children[0] as Image;
				image.Width = (double)num;
				image.Height = (double)num2;
				image.Source = null;
				image.Source = writeableBitmapAdapter.GetImageSource();
				image.SetValue(Canvas.LeftProperty, Math.Abs(num3));
			}
			else
			{
				Image image2 = new Image
				{
					Height = (double)num2,
					Width = (double)num,
					IsHitTestVisible = false
				};
				image2.Source = writeableBitmapAdapter.GetImageSource();
				image2.SetValue(Canvas.LeftProperty, Math.Abs(num3));
				canvas.Children.Add(image2);
			}
			if (canvas.Parent != null && (canvas.Parent as Canvas).Parent != null)
			{
				Canvas visual = (canvas.Parent as Canvas).Parent as Canvas;
				PointChart.Clip(chart, dataPointsUnderViewPort, visual, width, height);
			}
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
			if (dataSeries.LightWeight.Value)
			{
				double height = chart.ChartArea.ChartVisualCanvas.Height;
				double width = chart.ChartArea.ChartVisualCanvas.Width;
				Canvas canvas = dataSeries.Faces.Visual as Canvas;
				RenderHelper.UpdateParentVisualCanvasSize(chart, canvas);
				PointChart.UpdateRasterPointSeries(dataSeries, width, height);
				return;
			}
			if (property == VcProperties.DataPoints)
			{
				chart.ChartArea.RenderSeries();
				return;
			}
			foreach (IDataPoint current in dataSeries.InternalDataPoints)
			{
				PointChart.UpdateDataPoint(current, property, newValue, isAxisChanged);
			}
		}

		private static void UpdateRasterDataPoint(IDataPoint dataPoint)
		{
			DataSeries parent = dataPoint.Parent;
			Faces faces = parent.Faces;
			if (faces == null)
			{
				return;
			}
			Canvas canvas = faces.Visual as Canvas;
			double num = dataPoint.Parent.PlotGroup.AxisX.GetAxisVisualLeftAsOffset();
			if (double.IsNaN(num))
			{
				num = 0.0;
			}
			RasterMarkerStyle rasterMarkerStyle = new RasterMarkerStyle();
			double num2 = dataPoint.Parent.MarkerSize.Value;
			rasterMarkerStyle.Width = num2;
			rasterMarkerStyle.Height = num2;
			rasterMarkerStyle.MarkerType = dataPoint.Parent.MarkerType;
			if (((Thickness?)dataPoint.Parent.GetValue(DataSeries.MarkerBorderThicknessProperty)).HasValue)
			{
				rasterMarkerStyle.StrokeThickness = dataPoint.Parent.MarkerBorderThickness.Value.Left;
			}
			using (WriteableBitmapAdapter writeableBitmapAdapter = new WriteableBitmapAdapter((canvas.Children[0] as Image).Source as BitmapSource))
			{
				PointChart.CreateOrUpdateARasterPointDataPoint(writeableBitmapAdapter, dataPoint, canvas.Width, canvas.Height, num, rasterMarkerStyle);
				if (canvas.Children.Count > 0)
				{
					Image image = canvas.Children[0] as Image;
					image.Source = null;
					image.Source = writeableBitmapAdapter.GetImageSource();
				}
			}
		}

		private static void UpdateDataPoint(IDataPoint dataPoint, VcProperties property, object newValue, bool isAxisChanged)
		{
			Chart chart = dataPoint.Chart as Chart;
			bool flag = true;
			if (chart == null)
			{
				return;
			}
			PlotDetails arg_34_0 = chart.PlotDetails;
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
			if (dataPoint.DpInfo.LightWeight)
			{
				if (property == VcProperties.Color)
				{
					dataPoint.DpInfo.InternalColor = (newValue as Brush);
				}
				if (property == VcProperties.Color || property == VcProperties.MarkerColor || property == VcProperties.MarkerBorderColor)
				{
					PointChart.UpdateRasterDataPoint(dataPoint);
					return;
				}
				PointChart.UpdateRasterPointSeries(parent, width, height);
				return;
			}
			else
			{
				if (property == VcProperties.Enabled || ((dataPoint.DpInfo.Faces == null || double.IsNaN(dataPoint.DpInfo.InternalYValue)) && (property == VcProperties.XValue || property == VcProperties.YValue)))
				{
					PointChart.CreateOrUpdateAPointDataPoint(canvas, dataPoint, width, height);
					return;
				}
				if (dataPoint.DpInfo.Faces == null)
				{
					return;
				}
				if (property <= VcProperties.Effect)
				{
					if (property <= VcProperties.Color)
					{
						if (property == VcProperties.Bevel)
						{
							goto IL_62E;
						}
						if (property != VcProperties.Color)
						{
							goto IL_62E;
						}
					}
					else
					{
						if (property == VcProperties.Cursor)
						{
							goto IL_62E;
						}
						if (property == VcProperties.DataPoints)
						{
							goto IL_579;
						}
						if (property != VcProperties.Effect)
						{
							goto IL_62E;
						}
						if (marker != null && VisifireControl.IsMediaEffectsEnabled)
						{
							marker.Effect = (Effect)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect);
							marker.ApplyEffects();
							goto IL_62E;
						}
						goto IL_62E;
					}
				}
				else
				{
					if (property <= VcProperties.MarkerType)
					{
						switch (property)
						{
						case VcProperties.Href:
							if (!dataPoint.IsLightDataPoint)
							{
								(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
								DataPointHelper.SetHref2DataPointVisualFaces(dataPoint);
								goto IL_62E;
							}
							goto IL_62E;
						case VcProperties.HrefTarget:
							DataPointHelper.SetHref2DataPointVisualFaces(dataPoint);
							goto IL_62E;
						case VcProperties.Inlines:
						case VcProperties.IncludeZero:
						case VcProperties.InterlacedColor:
						case VcProperties.Interval:
						case VcProperties.IntervalType:
						case VcProperties.IncludeYValueInLegend:
						case VcProperties.IncludePercentageInLegend:
						case VcProperties.IncludeDataPointsInLegend:
							goto IL_62E;
						case VcProperties.LabelBackground:
							if (marker != null)
							{
								marker.TextBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
								goto IL_62E;
							}
							goto IL_62E;
						case VcProperties.LabelEnabled:
							PointChart.CreateOrUpdateAPointDataPoint(canvas, dataPoint, width, height);
							goto IL_62E;
						case VcProperties.LabelFontColor:
							if (marker != null)
							{
								marker.FontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
								goto IL_62E;
							}
							goto IL_62E;
						case VcProperties.LabelFontFamily:
							if (marker != null)
							{
								marker.FontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
								goto IL_62E;
							}
							goto IL_62E;
						case VcProperties.LabelFontSize:
							break;
						case VcProperties.LabelFontStyle:
							if (marker != null)
							{
								marker.FontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
								goto IL_62E;
							}
							goto IL_62E;
						case VcProperties.LabelFontWeight:
							if (marker != null)
							{
								marker.FontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
								goto IL_62E;
							}
							goto IL_62E;
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
									goto IL_62E;
								}
								goto IL_62E;
							case VcProperties.Legend:
							case VcProperties.LegendMarkerColor:
							case VcProperties.LegendMarkerType:
								goto IL_62E;
							case VcProperties.LegendText:
								chart.InvokeRender();
								goto IL_62E;
							case VcProperties.LightingEnabled:
								goto IL_398;
							default:
								switch (property)
								{
								case VcProperties.MarkerBorderColor:
									if (marker != null)
									{
										marker.BorderColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor);
										goto IL_62E;
									}
									goto IL_62E;
								case VcProperties.MarkerBorderThickness:
									if (marker != null)
									{
										marker.BorderThickness = ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderThickness)).Left;
										goto IL_62E;
									}
									goto IL_62E;
								case VcProperties.MarkerColor:
									if (marker != null)
									{
										marker.MarkerFillColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerColor);
										goto IL_62E;
									}
									goto IL_62E;
								case VcProperties.MarkerEnabled:
									goto IL_62E;
								case VcProperties.MarkerScale:
								case VcProperties.MarkerSize:
								case VcProperties.MarkerType:
									break;
								default:
									goto IL_62E;
								}
								break;
							}
							break;
						}
						PointChart.CreateOrUpdateAPointDataPoint(canvas, dataPoint, width, height);
						goto IL_62E;
					}
					if (property != VcProperties.Opacity)
					{
						switch (property)
						{
						case VcProperties.ShadowEnabled:
							if (marker != null)
							{
								marker.ShadowEnabled = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled);
								marker.ApplyRemoveShadow();
								goto IL_62E;
							}
							goto IL_62E;
						case VcProperties.ShowInLegend:
							chart.InvokeRender();
							goto IL_62E;
						default:
							switch (property)
							{
							case VcProperties.ToolTipText:
								dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
								goto IL_62E;
							case VcProperties.To:
							case VcProperties.TrendLines:
							case VcProperties.Value:
							case VcProperties.ValueFormatString:
							case VcProperties.VerticalAlignment:
							case VcProperties.XValues:
								goto IL_62E;
							case VcProperties.XValue:
							case VcProperties.YValue:
							case VcProperties.YValues:
								goto IL_579;
							case VcProperties.XValueFormatString:
							case VcProperties.YValueFormatString:
								dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
								if (!dataPoint.IsLightDataPoint)
								{
									(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
								}
								PointChart.CreateOrUpdateAPointDataPoint(canvas, dataPoint, width, height);
								goto IL_62E;
							case VcProperties.XValueType:
								chart.InvokeRender();
								goto IL_62E;
							default:
								goto IL_62E;
							}
							break;
						}
					}
					else
					{
						if (marker != null)
						{
							marker.Visual.Opacity = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity) * parent.Opacity;
							goto IL_62E;
						}
						goto IL_62E;
					}
				}
				IL_398:
				if (marker != null)
				{
					marker.MarkerShape.Fill = (chart.View3D ? Graphics.Get3DBrushLighting(dataPoint.Color, (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) : (((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) ? Graphics.GetLightingEnabledBrush(dataPoint.Color, "Linear", null) : dataPoint.Color));
					goto IL_62E;
				}
				goto IL_62E;
				IL_579:
				if (isAxisChanged)
				{
					PointChart.UpdateDataSeries(parent, property, newValue, false);
				}
				else if (marker != null)
				{
					dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
					if (!dataPoint.IsLightDataPoint)
					{
						(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
					}
					bool flag2 = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled);
					if (flag2)
					{
						marker.Text = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText));
					}
					BubbleChart.UpdateBubblePositionAccording2XandYValue(dataPoint, width, height, chart.AnimatedUpdate.Value, marker.MarkerShape.Width, marker.MarkerShape.Width);
				}
				IL_62E:
				if (canvas.Parent != null)
				{
					double num = (from tick in chart.AxesX[0].Ticks
					where chart.AxesX[0].Enabled.Value && tick.Enabled.Value
					select tick.TickLength.Value).Sum();
					if (num == 0.0)
					{
						num = 5.0;
					}
					double num2 = (from axis in chart.AxesY
					where axis.AxisType == AxisTypes.Primary
					from tick in axis.Ticks
					where axis.Enabled.Value && tick.Enabled.Value
					select tick.TickLength.Value).Sum();
					if (num2 == 0.0)
					{
						num2 = 8.0;
					}
					double num3 = (from axis in chart.AxesY
					where axis.AxisType == AxisTypes.Secondary
					from tick in axis.Ticks
					where axis.Enabled.Value && tick.Enabled.Value
					select tick.TickLength.Value).Sum();
					if (num3 == 0.0)
					{
						num3 = 8.0;
					}
					double num4 = (double)(from c in chart.PlotDetails.PlotGroups
					where c.AxisY.AxisType == AxisTypes.Secondary
					select c).Count<PlotGroup>();
					List<IDataPoint> list = (from datapoint in chart.PlotDetails.ListOfAllDataPoints
					where datapoint.Enabled == true
					select datapoint).ToList<IDataPoint>();
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].Parent.PlotGroup.AxisY.AxisMinimum != null && !double.IsNaN(list[i].DpInfo.InternalYValue))
						{
							double limitingYValue = list[i].Parent.PlotGroup.GetLimitingYValue();
							if ((list[i].DpInfo.InternalYValue < limitingYValue && limitingYValue > 0.0) || (list[i].DpInfo.InternalYValue < limitingYValue && list[i].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || list[i].DpInfo.InternalYValue < list[i].Parent.PlotGroup.AxisY.InternalAxisMinimum)
							{
								if (list[i].DpInfo.Marker != null && list[i].DpInfo.Marker.Visual != null)
								{
									list[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
								}
							}
							else if (list[i].DpInfo.Marker != null && list[i].DpInfo.Marker.Visual != null)
							{
								list[i].DpInfo.Marker.Visual.Visibility = Visibility.Visible;
							}
						}
					}
					for (int j = 0; j < list.Count; j++)
					{
						if (list[j].Parent.PlotGroup.AxisY.AxisMinimum != null)
						{
							double limitingYValue2 = list[j].Parent.PlotGroup.GetLimitingYValue();
							if (list[j].DpInfo.InternalYValue < limitingYValue2)
							{
								flag = false;
								break;
							}
						}
					}
					RectangleGeometry rectangleGeometry = new RectangleGeometry();
					if (flag)
					{
						rectangleGeometry.Rect = new Rect(-num2, -chart.ChartArea.PLANK_DEPTH - 4.0, width + num3 + ((num4 > 0.0) ? num2 : 8.0) + chart.ChartArea.PLANK_OFFSET, height + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + num + 4.0);
					}
					else
					{
						rectangleGeometry.Rect = new Rect(-num2, -chart.ChartArea.PLANK_DEPTH - 4.0, width + num3 + ((num4 > 0.0) ? num2 : 8.0) + chart.ChartArea.PLANK_OFFSET, height + chart.ChartArea.PLANK_DEPTH + num + 4.0);
					}
					(canvas.Parent as Canvas).Clip = rectangleGeometry;
				}
				return;
			}
		}
	}
}
