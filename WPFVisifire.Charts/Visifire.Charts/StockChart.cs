using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class StockChart
	{
		internal static Brush ReCalculateAndApplyTheNewBrush(Shape shape, Brush newBrush, bool isLightingEnabled)
		{
			shape.Stroke = (isLightingEnabled ? Graphics.GetLightingEnabledBrush(newBrush, "Linear", null) : newBrush);
			return shape.Stroke;
		}

		private static void UpdateYValueAndXValuePosition(IDataPoint dataPoint, double canvasWidth, double canvasHeight, double dataPointWidth)
		{
			Canvas canvas = dataPoint.DpInfo.Faces.Visual as Canvas;
			Faces faces = dataPoint.DpInfo.Faces;
			Line line = faces.VisualComponents[0] as Line;
			Line line2 = faces.VisualComponents[1] as Line;
			Line line3 = faces.VisualComponents[2] as Line;
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			PlotGroup plotGroup = dataPoint.Parent.PlotGroup;
			CandleStick.SetDataPointValues(dataPoint, ref num, ref num2, ref num3, ref num4);
			double num5 = Graphics.ValueToPixelPosition(0.0, canvasWidth, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
			num3 = Graphics.ValueToPixelPosition(canvasHeight, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num3);
			num4 = Graphics.ValueToPixelPosition(canvasHeight, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num4);
			num = Graphics.ValueToPixelPosition(canvasHeight, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num);
			num2 = Graphics.ValueToPixelPosition(canvasHeight, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num2);
			bool flag = true;
			if (dataPoint.Parent.PlotGroup.AxisY.AxisMinimum != null)
			{
				double limitingYValue = dataPoint.Parent.PlotGroup.GetLimitingYValue();
				double num6 = Graphics.ValueToPixelPosition(canvasHeight, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, limitingYValue);
				if (num6 < num3 && num6 < num4 && num6 < num && num6 < num2)
				{
					flag = false;
				}
			}
			double num7 = (num2 < num) ? num2 : num;
			num3 -= num7;
			num4 -= num7;
			canvas.Width = dataPointWidth;
			canvas.Height = Math.Abs(num2 - num);
			canvas.SetValue(Canvas.LeftProperty, num5 - dataPointWidth / 2.0);
			canvas.SetValue(Canvas.TopProperty, num7);
			line.X1 = canvas.Width / 2.0;
			line.X2 = canvas.Width / 2.0;
			line.Y1 = 0.0;
			line.Y2 = canvas.Height;
			line3.X1 = 0.0;
			line3.X2 = canvas.Width / 2.0;
			line3.Y1 = num3;
			line3.Y2 = num3;
			line2.X1 = canvas.Width / 2.0;
			line2.X2 = canvas.Width;
			line2.Y1 = num4;
			line2.Y2 = num4;
			StockChart.ApplyEffects(dataPoint);
			StockChart.ApplyOrUpdateShadow(dataPoint, canvas, line, line3, line2, dataPointWidth);
			if (flag)
			{
				CandleStick.CreateAndPositionLabel(dataPoint.Parent.Faces.LabelCanvas, dataPoint);
			}
			if (dataPoint.Parent.ToolTipElement != null)
			{
				dataPoint.Parent.ToolTipElement.Hide();
			}
			(dataPoint.Chart as Chart).ChartArea.DisableIndicators();
			dataPoint.DpInfo.VisualPosition = new Point((double)canvas.GetValue(Canvas.LeftProperty) + canvas.Width / 2.0, (double)canvas.GetValue(Canvas.TopProperty));
		}

		private static void ApplyEffects(IDataPoint dataPoint)
		{
			Canvas canvas = dataPoint.DpInfo.Faces.Visual as Canvas;
			if (VisifireControl.IsMediaEffectsEnabled)
			{
				if ((Effect)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect) != null)
				{
					canvas.Effect = (Effect)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect);
					return;
				}
				canvas.Effect = null;
			}
		}

		private static void ApplyBorderProperties(IDataPoint dataPoint, Line highLow, Line openLine, Line closeLine, double dataPointWidth)
		{
			highLow.StrokeThickness = ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left;
			if (highLow.StrokeThickness > dataPointWidth / 2.0)
			{
				highLow.StrokeThickness = dataPointWidth / 2.0;
			}
			else if (highLow.StrokeThickness > dataPointWidth)
			{
				highLow.StrokeThickness = dataPointWidth;
			}
			string lineStyle = DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle).ToString();
			highLow.StrokeDashArray = Graphics.LineStyleToStrokeDashArray(lineStyle);
			openLine.StrokeThickness = ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left;
			openLine.StrokeDashArray = Graphics.LineStyleToStrokeDashArray(lineStyle);
			closeLine.StrokeThickness = ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left;
			closeLine.StrokeDashArray = Graphics.LineStyleToStrokeDashArray(lineStyle);
		}

		private static void ApplyOrUpdateColorForAStockDp(IDataPoint dataPoint, Line highLow, Line openLine, Line closeLine)
		{
			StockChart.ReCalculateAndApplyTheNewBrush(highLow, dataPoint.Color, (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled));
			openLine.Stroke = highLow.Stroke;
			closeLine.Stroke = highLow.Stroke;
		}

		private static void ApplyOrUpdateShadow(IDataPoint dataPoint, Canvas dataPointVisual, Line highLow, Line openLine, Line closeLine, double dataPointWidth)
		{
			if (!VisifireControl.IsMediaEffectsEnabled)
			{
				Faces faces = dataPoint.DpInfo.Faces;
				faces.ClearList(ref faces.ShadowElements);
				bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled);
				Thickness thickness = (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness);
				BorderStyles borderStyles = (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle);
				if (flag)
				{
					Line line = new Line
					{
						IsHitTestVisible = false,
						X1 = dataPointVisual.Width / 2.0 + CandleStick._shadowDepth,
						X2 = dataPointVisual.Width / 2.0 + CandleStick._shadowDepth,
						Y1 = 0.0 + CandleStick._shadowDepth,
						Y2 = dataPointVisual.Height + CandleStick._shadowDepth,
						Stroke = CandleStick._shadowColor,
						StrokeThickness = thickness.Left,
						StrokeDashArray = Graphics.LineStyleToStrokeDashArray(borderStyles.ToString())
					};
					Line line2 = new Line
					{
						IsHitTestVisible = false,
						X1 = openLine.X1 + CandleStick._shadowDepth,
						X2 = openLine.X2 + CandleStick._shadowDepth,
						Y1 = openLine.Y1 + CandleStick._shadowDepth,
						Y2 = openLine.Y2 + CandleStick._shadowDepth,
						Stroke = CandleStick._shadowColor,
						StrokeThickness = thickness.Left,
						StrokeDashArray = Graphics.LineStyleToStrokeDashArray(borderStyles.ToString())
					};
					Line line3 = new Line
					{
						IsHitTestVisible = false,
						X1 = closeLine.X1 + CandleStick._shadowDepth,
						X2 = closeLine.X2 + CandleStick._shadowDepth,
						Y1 = closeLine.Y1 + CandleStick._shadowDepth,
						Y2 = closeLine.Y2 + CandleStick._shadowDepth,
						Stroke = CandleStick._shadowColor,
						StrokeThickness = thickness.Left,
						StrokeDashArray = Graphics.LineStyleToStrokeDashArray(borderStyles.ToString())
					};
					faces.ShadowElements.Add(line);
					faces.ShadowElements.Add(line2);
					faces.ShadowElements.Add(line3);
					dataPointVisual.Children.Add(line);
					dataPointVisual.Children.Add(line2);
					dataPointVisual.Children.Add(line3);
					return;
				}
			}
			else
			{
				bool flag2 = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled);
				if ((Effect)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect) == null)
				{
					if (flag2)
					{
						dataPointVisual.Effect = ExtendedGraphics.GetShadowEffect(315.0, 4.0, 0.95);
						return;
					}
					dataPointVisual.Effect = null;
				}
			}
		}

		internal static void CreateOrUpdateARasterStockDataPoint(WriteableBitmapAdapter wba, IDataPoint dataPoint, double width, double height, double dataPointWidth, double scrollOffsetAsPixel)
		{
			if (dataPoint.DpInfo.InternalYValues == null)
			{
				return;
			}
			if (!dataPoint.Parent.IsAllDataPointsEnabled && dataPoint.Enabled == false)
			{
				return;
			}
			double num = dataPointWidth / 2.0;
			DataSeries parent = dataPoint.Parent;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			CandleStick.SetDataPointValues(dataPoint, ref num2, ref num3, ref num4, ref num5);
			Axis axisY = parent.PlotGroup.AxisY;
			double valueMin = axisY.InternalAxisMinimum;
			double valueMax = axisY.InternalAxisMaximum;
			Axis axisX = parent.PlotGroup.AxisX;
			double x = Graphics.ValueToPixelPosition(0.0, width, axisX.InternalAxisMinimum, axisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
			num4 = Graphics.ValueToPixelPosition(height, 0.0, valueMin, valueMax, num4);
			num5 = Graphics.ValueToPixelPosition(height, 0.0, valueMin, valueMax, num5);
			num2 = Graphics.ValueToPixelPosition(height, 0.0, valueMin, valueMax, num2);
			num3 = Graphics.ValueToPixelPosition(height, 0.0, valueMin, valueMax, num3);
			double num6 = (num3 < num2) ? num3 : num2;
			num4 -= num6;
			num5 -= num6;
			dataPoint.DpInfo.VisualPosition = new Point(x, num6);
			Point visualPosition = dataPoint.DpInfo.VisualPosition;
			visualPosition.X -= Math.Abs(scrollOffsetAsPixel);
			dataPoint.DpInfo.RasterVisualPosition = visualPosition;
			double num7 = dataPoint.DpInfo.RasterVisualPosition.X - num;
			double num8 = Math.Abs(num3 - num2);
			LineStructure lineStructure = new LineStructure(new Point(num7 + num, num6), new Point(num7 + num, num6 + num8));
			LineStructure lineStructure2 = new LineStructure(new Point(num7, num6 + num4), new Point(num7 + num, num6 + num4));
			LineStructure lineStructure3 = new LineStructure(new Point(num7 + num, num6 + num5), new Point(num7 + dataPointWidth, num6 + num5));
			double strokeThickness;
			if (DataPointHelper.GetBorderThickness_DataPoint(dataPoint) == new Thickness(0.0, 0.0, 0.0, 0.0))
			{
				strokeThickness = 2.0;
			}
			else
			{
				strokeThickness = ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left;
			}
			RasterLineStyle style = new RasterLineStyle
			{
				Stroke = (dataPoint.Color as SolidColorBrush).Color,
				StrokeThickness = strokeThickness,
				IsAntiAliased = parent.IsAntiAliased,
				StrokeStyle = parent.LineStyle,
				Opacity = parent.Opacity * (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity)
			};
			RasterRenderEngine.DrawLine(wba, lineStructure.PointA, lineStructure.PointB, style);
			RasterRenderEngine.DrawLine(wba, lineStructure2.PointA, lineStructure2.PointB, style);
			RasterRenderEngine.DrawLine(wba, lineStructure3.PointA, lineStructure3.PointB, style);
		}

		internal static void CreateOrUpdateAStockDataPoint(IDataPoint dataPoint, Canvas stockChartCanvas, Canvas labelCanvas, double canvasWidth, double canvasHeight, double dataPointWidth)
		{
			Faces faces = dataPoint.DpInfo.Faces;
			if (faces != null && faces.Visual != null && stockChartCanvas == faces.Visual.Parent)
			{
				stockChartCanvas.Children.Remove(dataPoint.DpInfo.Faces.Visual);
			}
			if (dataPoint.DpInfo.LabelVisual != null && dataPoint.DpInfo.LabelVisual.Parent == labelCanvas)
			{
				labelCanvas.Children.Remove(dataPoint.DpInfo.LabelVisual);
			}
			dataPoint.DpInfo.Faces = null;
			if (dataPoint.DpInfo.InternalYValues == null || dataPoint.Enabled == false)
			{
				return;
			}
			dataPoint.DpInfo.Faces = new Faces();
			ElementData tag = new ElementData
			{
				Element = dataPoint
			};
			Canvas canvas = new Canvas();
			Line line = new Line
			{
				Tag = tag
			};
			Line line2 = new Line
			{
				Tag = tag
			};
			Line line3 = new Line
			{
				Tag = tag
			};
			dataPoint.DpInfo.Faces.Visual = canvas;
			dataPoint.DpInfo.Faces.VisualComponents.Add(line);
			dataPoint.DpInfo.Faces.VisualComponents.Add(line3);
			dataPoint.DpInfo.Faces.VisualComponents.Add(line2);
			dataPoint.DpInfo.Faces.BorderElements.Add(line);
			dataPoint.DpInfo.Faces.BorderElements.Add(line3);
			dataPoint.DpInfo.Faces.BorderElements.Add(line2);
			dataPoint.DpInfo.Faces.Visual = canvas;
			stockChartCanvas.Children.Add(canvas);
			StockChart.UpdateYValueAndXValuePosition(dataPoint, canvasWidth, canvasHeight, dataPointWidth);
			StockChart.ApplyBorderProperties(dataPoint, line, line3, line2, dataPointWidth);
			StockChart.ApplyOrUpdateColorForAStockDp(dataPoint, line, line3, line2);
			canvas.Children.Add(line);
			canvas.Children.Add(line3);
			canvas.Children.Add(line2);
			canvas.Opacity = dataPoint.Parent.Opacity * (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity);
			Chart chart = dataPoint.Chart as Chart;
			DataPointHelper.SetCursor2DataPointVisualFaces(dataPoint);
			if (!dataPoint.IsLightDataPoint)
			{
				CandleStick.AttachEvents2Visual(dataPoint as DataPoint, dataPoint);
			}
			CandleStick.AttachEvents2Visual(dataPoint.Parent, dataPoint);
			dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
			if (!dataPoint.IsLightDataPoint)
			{
				(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
			}
			if (!chart.IndicatorEnabled)
			{
				dataPoint.AttachToolTip(chart, dataPoint, dataPoint.DpInfo.Faces.VisualComponents);
				if (dataPoint.DpInfo.LabelVisual != null)
				{
					dataPoint.AttachToolTip(chart, dataPoint, dataPoint.DpInfo.LabelVisual);
				}
			}
			if (!dataPoint.IsLightDataPoint)
			{
				(dataPoint as DataPoint).AttachHref(chart, dataPoint.DpInfo.Faces.VisualComponents, (dataPoint as DataPoint).ParsedHref, (HrefTargets)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.HrefTarget));
			}
		}

		internal static Canvas GetVisualObjectForStockChart(Panel preExistingPanel, double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, double plankDepth, bool animationEnabled)
		{
			if (double.IsNaN(width) || double.IsNaN(height) || width <= 0.0 || height <= 0.0)
			{
				return null;
			}
			Canvas canvas;
			Canvas canvas2;
			Canvas canvas3;
			RenderHelper.PrepareCanvas4Drawing(preExistingPanel as Canvas, out canvas, out canvas2, out canvas3, width, height);
			double num = plankDepth / (double)((plotDetails.Layer3DCount == 0) ? 1 : plotDetails.Layer3DCount) * (double)(chart.View3D ? 1 : 0);
			double num2 = num * (double)(plotDetails.SeriesDrawingIndex[seriesList[0]] + 1 - ((plotDetails.Layer3DCount == 0) ? 0 : 1));
			canvas.SetValue(Canvas.TopProperty, num2);
			canvas.SetValue(Canvas.LeftProperty, -num2);
			double num3 = 0.0;
			DataSeries dataSeries = null;
			double dataPointWidth = CandleStick.CalculateDataPointWidth(width, height, chart);
			bool indicatorEnabled = chart.IndicatorEnabled;
			Canvas plotAreaCanvas = chart.ChartArea.PlotAreaCanvas;
			int num4 = (int)plotAreaCanvas.Width;
			int num5 = (int)plotAreaCanvas.Height;
			if (num5 == 0 || num4 == 0)
			{
				return null;
			}
			double num6 = 0.0;
			foreach (DataSeries current in seriesList)
			{
				if (!(current.Enabled == false))
				{
					Canvas canvas4 = new Canvas
					{
						Height = height,
						Width = width
					};
					Canvas canvas5 = new Canvas
					{
						Height = height,
						Width = width
					};
					WriteableBitmapAdapter writeableBitmapAdapter = new WriteableBitmapAdapter(num4, num5);
					Faces faces = new Faces
					{
						Visual = canvas4,
						LabelCanvas = canvas5
					};
					current.Faces = faces;
					dataSeries = current;
					List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(current, false, RenderHelper.IsRasterRenderSupported(current));
					foreach (IDataPoint current2 in dataPointsUnderViewPort)
					{
						if (current.LightWeight.Value)
						{
							StockChart.CreateOrUpdateARasterStockDataPoint(writeableBitmapAdapter, current2, width, height, dataPointWidth, num6);
							if (indicatorEnabled)
							{
								current2.DpInfo.ParsedToolTipText = current2.TextParser(current2.ToolTipText);
							}
						}
						else
						{
							StockChart.CreateOrUpdateAStockDataPoint(current2, canvas4, canvas5, width, height, dataPointWidth);
						}
					}
					if (current.LightWeight.Value)
					{
						Image image = new Image
						{
							Height = (double)num5,
							Width = (double)num4,
							IsHitTestVisible = false
						};
						image.Source = writeableBitmapAdapter.GetImageSource();
						image.SetValue(Canvas.LeftProperty, Math.Abs(num6));
						canvas4.Children.Add(image);
					}
					canvas3.Children.Add(canvas4);
					canvas2.Children.Add(canvas5);
				}
			}
			if (animationEnabled && dataSeries != null)
			{
				if (dataSeries.Storyboard == null)
				{
					dataSeries.Storyboard = new Storyboard();
				}
				dataSeries.Storyboard = AnimationHelper.ApplyOpacityAnimation(canvas3, dataSeries, dataSeries.Storyboard, num3, 1.0, 0.0, 1.0);
				num3 += 0.5;
			}
			if (animationEnabled && dataSeries != null)
			{
				dataSeries.Storyboard = AnimationHelper.ApplyOpacityAnimation(canvas2, dataSeries, dataSeries.Storyboard, num3, 1.0, 0.0, 1.0);
			}
			canvas3.Tag = null;
			if (preExistingPanel != null)
			{
				canvas.Children.RemoveAt(1);
				canvas.Children.Add(canvas3);
			}
			else
			{
				canvas2.SetValue(Panel.ZIndexProperty, 1);
				canvas.Children.Add(canvas2);
				canvas.Children.Add(canvas3);
			}
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			rectangleGeometry.Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_OFFSET, height + chart.ChartArea.PLANK_DEPTH);
			canvas.Clip = rectangleGeometry;
			return canvas;
		}

		private static void UpdateRasterStockSeries(DataSeries series, double width, double height)
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
			List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(series, false, RenderHelper.IsRasterRenderSupported(series));
			bool indicatorEnabled = chart.IndicatorEnabled;
			Canvas plotAreaCanvas = chart.ChartArea.PlotAreaCanvas;
			int num = (int)plotAreaCanvas.Width;
			int num2 = (int)plotAreaCanvas.Height;
			if (num == 0 || num2 == 0)
			{
				return;
			}
			WriteableBitmapAdapter writeableBitmapAdapter = new WriteableBitmapAdapter(num, num2);
			double num3 = series.PlotGroup.AxisX.GetAxisVisualLeftAsOffset();
			if (double.IsNaN(num3))
			{
				num3 = 0.0;
			}
			double dataPointWidth = CandleStick.CalculateDataPointWidth(width, height, chart);
			foreach (IDataPoint current in dataPointsUnderViewPort)
			{
				if (series.LightWeight.Value)
				{
					StockChart.CreateOrUpdateARasterStockDataPoint(writeableBitmapAdapter, current, width, height, dataPointWidth, num3);
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
				Canvas canvas2 = (canvas.Parent as Canvas).Parent as Canvas;
				canvas2.Clip = new RectangleGeometry
				{
					Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_OFFSET, height + chart.ChartArea.PLANK_DEPTH)
				};
			}
		}

		public static void Update(IElement sender, ElementTypes elementType, VcProperties property, object newValue, bool isAxisChanged)
		{
			bool flag = elementType == ElementTypes.DataPoint;
			if (flag)
			{
				StockChart.UpdateDataPoint(sender as IDataPoint, property, newValue, isAxisChanged);
				return;
			}
			StockChart.UpdateDataSeries(sender as DataSeries, property, newValue, isAxisChanged);
		}

		private static void UpdateRasterDataPoint(IDataPoint dataPoint)
		{
			DataSeries parent = dataPoint.Parent;
			Faces faces = parent.Faces;
			Faces faces2 = dataPoint.DpInfo.Faces;
			if (faces == null)
			{
				return;
			}
			double dataPointWidth;
			if (faces2 != null)
			{
				dataPointWidth = faces2.Width;
			}
			else
			{
				dataPointWidth = CandleStick.CalculateDataPointWidth(faces.Visual.Width, faces.Visual.Height, dataPoint.Chart as Chart);
			}
			Canvas canvas = faces.Visual as Canvas;
			double num = dataPoint.Parent.PlotGroup.AxisX.GetAxisVisualLeftAsOffset();
			if (double.IsNaN(num))
			{
				num = 0.0;
			}
			using (WriteableBitmapAdapter writeableBitmapAdapter = new WriteableBitmapAdapter((canvas.Children[0] as Image).Source as BitmapSource))
			{
				StockChart.CreateOrUpdateARasterStockDataPoint(writeableBitmapAdapter, dataPoint, canvas.Width, canvas.Height, dataPointWidth, num);
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
			if (chart == null)
			{
				return;
			}
			DataSeries parent = dataPoint.Parent;
			Faces faces = parent.Faces;
			Faces faces2 = dataPoint.DpInfo.Faces;
			if (faces != null)
			{
				RenderHelper.UpdateParentVisualCanvasSize(chart, faces.Visual as Canvas);
			}
			if (dataPoint.DpInfo.LightWeight)
			{
				StockChart.UpdateRasterDataPoint(dataPoint);
				return;
			}
			PlotGroup arg_59_0 = parent.PlotGroup;
			double dataPointWidth;
			if (faces2 != null && faces2.Visual != null)
			{
				dataPointWidth = faces2.Visual.Width;
			}
			else
			{
				if (faces == null)
				{
					return;
				}
				dataPointWidth = CandleStick.CalculateDataPointWidth(faces.Visual.Width, faces.Visual.Height, chart);
			}
			if (property == VcProperties.Enabled || (faces2 == null && (property == VcProperties.XValue || property == VcProperties.YValues)))
			{
				StockChart.CreateOrUpdateAStockDataPoint(dataPoint, faces.Visual as Canvas, faces.LabelCanvas, faces.Visual.Width, faces.Visual.Height, dataPointWidth);
				return;
			}
			if (faces2 == null)
			{
				return;
			}
			Canvas dataPointVisual = faces2.Visual as Canvas;
			Line highLow = faces2.VisualComponents[0] as Line;
			Line closeLine = faces2.VisualComponents[1] as Line;
			Line openLine = faces2.VisualComponents[2] as Line;
			if (property <= VcProperties.LabelFontWeight)
			{
				if (property <= VcProperties.Cursor)
				{
					switch (property)
					{
					case VcProperties.BorderStyle:
					case VcProperties.BorderThickness:
						StockChart.ApplyBorderProperties(dataPoint, highLow, openLine, closeLine, dataPointWidth);
						return;
					case VcProperties.BubbleStyle:
						return;
					case VcProperties.Color:
						StockChart.ApplyOrUpdateColorForAStockDp(dataPoint, highLow, openLine, closeLine);
						return;
					default:
						if (property != VcProperties.Cursor)
						{
							return;
						}
						DataPointHelper.SetCursor2DataPointVisualFaces(dataPoint);
						return;
					}
				}
				else
				{
					switch (property)
					{
					case VcProperties.Effect:
						StockChart.ApplyEffects(dataPoint);
						return;
					case VcProperties.Enabled:
						StockChart.CreateOrUpdateAStockDataPoint(dataPoint, faces.Visual as Canvas, faces.LabelCanvas, faces.Visual.Width, faces.Visual.Height, dataPointWidth);
						return;
					default:
						switch (property)
						{
						case VcProperties.Href:
							if (!dataPoint.IsLightDataPoint)
							{
								(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
								DataPointHelper.SetHref2DataPointVisualFaces(dataPoint);
								return;
							}
							return;
						case VcProperties.HrefTarget:
							DataPointHelper.SetHref2DataPointVisualFaces(dataPoint);
							return;
						case VcProperties.Inlines:
						case VcProperties.IncludeZero:
						case VcProperties.InterlacedColor:
						case VcProperties.Interval:
						case VcProperties.IntervalType:
						case VcProperties.IncludeYValueInLegend:
						case VcProperties.IncludePercentageInLegend:
						case VcProperties.IncludeDataPointsInLegend:
							return;
						case VcProperties.LabelBackground:
						case VcProperties.LabelEnabled:
						case VcProperties.LabelFontColor:
						case VcProperties.LabelFontFamily:
						case VcProperties.LabelFontSize:
						case VcProperties.LabelFontStyle:
						case VcProperties.LabelFontWeight:
							break;
						default:
							return;
						}
						break;
					}
				}
			}
			else if (property <= VcProperties.LightingEnabled)
			{
				switch (property)
				{
				case VcProperties.LabelStyle:
				case VcProperties.LabelText:
					break;
				case VcProperties.LabelAngle:
				case VcProperties.Legend:
					return;
				case VcProperties.LegendText:
					chart.InvokeRender();
					return;
				default:
					if (property != VcProperties.LightingEnabled)
					{
						return;
					}
					StockChart.ApplyOrUpdateColorForAStockDp(dataPoint, highLow, openLine, closeLine);
					return;
				}
			}
			else
			{
				if (property == VcProperties.Opacity)
				{
					faces2.Visual.Opacity = parent.Opacity * (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity);
					return;
				}
				switch (property)
				{
				case VcProperties.ShadowEnabled:
					StockChart.ApplyOrUpdateShadow(dataPoint, dataPointVisual, highLow, openLine, closeLine, dataPointWidth);
					return;
				case VcProperties.ShowInLegend:
					chart.InvokeRender();
					return;
				default:
					switch (property)
					{
					case VcProperties.ToolTipText:
						dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
						return;
					case VcProperties.To:
					case VcProperties.TrendLines:
					case VcProperties.Value:
					case VcProperties.ValueFormatString:
					case VcProperties.VerticalAlignment:
					case VcProperties.XValues:
						return;
					case VcProperties.XValue:
					case VcProperties.YValue:
					case VcProperties.YValues:
						if (isAxisChanged || dataPoint.DpInfo.InternalYValues == null)
						{
							StockChart.UpdateDataSeries(parent, property, newValue, isAxisChanged);
						}
						else
						{
							dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
							if (!dataPoint.IsLightDataPoint)
							{
								(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
							}
							StockChart.UpdateYValueAndXValuePosition(dataPoint, faces.Visual.Width, faces.Visual.Height, faces2.Visual.Width);
							if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled))
							{
								CandleStick.CreateAndPositionLabel(faces.LabelCanvas, dataPoint);
							}
						}
						if (!dataPoint.IsLightDataPoint && dataPoint.Parent.SelectionEnabled && (dataPoint as DataPoint).Selected)
						{
							DataPointHelper.Select(dataPoint, true);
							return;
						}
						return;
					case VcProperties.XValueFormatString:
					case VcProperties.YValueFormatString:
						dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
						if (!dataPoint.IsLightDataPoint)
						{
							(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
						}
						CandleStick.CreateAndPositionLabel(faces.LabelCanvas, dataPoint);
						return;
					case VcProperties.XValueType:
						chart.InvokeRender();
						return;
					default:
						return;
					}
					break;
				}
			}
			CandleStick.CreateAndPositionLabel(faces.LabelCanvas, dataPoint);
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
				StockChart.UpdateRasterStockSeries(dataSeries, width, height);
				return;
			}
			if (property == VcProperties.DataPoints || property == VcProperties.XValue || property == VcProperties.YValues)
			{
				chart.ChartArea.RenderSeries();
				return;
			}
			foreach (IDataPoint current in dataSeries.InternalDataPoints)
			{
				StockChart.UpdateDataPoint(current, property, newValue, isAxisChanged);
			}
		}
	}
}
