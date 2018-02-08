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
	internal class CandleStick
	{
		internal static Brush _shadowColor = new SolidColorBrush(Color.FromArgb(255, 176, 176, 176));

		internal static double _shadowDepth = 1.5;

		internal static void ReCalculateAndApplyTheNewBrush(IDataPoint dataPoint, Shape shape, Brush newBrush, bool isLightingEnabled, bool is3D)
		{
			Brush openCloseRectangleFillbrush = CandleStick.GetOpenCloseRectangleFillbrush(dataPoint, newBrush);
			string visualElementName;
			if ((visualElementName = (shape.Tag as ElementData).VisualElementName) != null)
			{
				if (visualElementName == "HlLine")
				{
					shape.Stroke = (isLightingEnabled ? Graphics.GetLightingEnabledBrush(newBrush, "Linear", null) : Graphics.GetBevelTopBrush(newBrush));
					return;
				}
				if (visualElementName == "OcRect")
				{
					shape.Fill = (isLightingEnabled ? Graphics.GetLightingEnabledBrush(openCloseRectangleFillbrush, "Linear", null) : openCloseRectangleFillbrush);
					shape.Stroke = CandleStick.GetOpenCloseRectangleBorderbrush(dataPoint, newBrush);
					return;
				}
				if (visualElementName == "TopBevel")
				{
					shape.Fill = Graphics.GetBevelTopBrush(openCloseRectangleFillbrush);
					return;
				}
				if (visualElementName == "LeftBevel")
				{
					shape.Fill = Graphics.GetBevelSideBrush((double)(isLightingEnabled ? -70 : 0), openCloseRectangleFillbrush);
					return;
				}
				if (visualElementName == "RightBevel")
				{
					shape.Fill = Graphics.GetBevelSideBrush((double)(isLightingEnabled ? -110 : 180), openCloseRectangleFillbrush);
					return;
				}
				if (!(visualElementName == "BottomBevel"))
				{
					return;
				}
				shape.Fill = Graphics.GetBevelSideBrush(90.0, openCloseRectangleFillbrush);
			}
		}

		internal static double CalculateDataPointWidth(double width, double height, Chart chart)
		{
			double num = chart.PlotArea.Width / 100.0 * chart.MaxDataPointWidth;
			double num2 = chart.PlotDetails.GetMinOfMinDifferencesForXValue(new RenderAs[]
			{
				RenderAs.Column,
				RenderAs.StackedColumn,
				RenderAs.StackedColumn100,
				RenderAs.Stock,
				RenderAs.CandleStick
			});
			if (double.IsPositiveInfinity(num2))
			{
				num2 = 0.0;
			}
			double num3;
			if (double.IsNaN(chart.DataPointWidth) || chart.DataPointWidth < 0.0)
			{
				if (num2 != 0.0)
				{
					num3 = Graphics.ValueToPixelPosition(0.0, width, chart.AxesX[0].InternalAxisMinimum, chart.AxesX[0].InternalAxisMaximum, num2 + chart.AxesX[0].InternalAxisMinimum);
					num3 *= 0.9;
					if (num3 < 5.0)
					{
						num3 = 5.0;
					}
				}
				else
				{
					num3 = width * 0.3;
				}
			}
			else
			{
				num3 = chart.PlotArea.Width / 100.0 * chart.DataPointWidth;
			}
			if (num3 > num)
			{
				num3 = num;
			}
			if (num == 0.0)
			{
				num3 = 0.0;
			}
			return num3;
		}

		internal static double GetStrokeThickness(IDataPoint dataPoint, double dataPointWidth)
		{
			if (((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left != 0.0)
			{
				return ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left;
			}
			if (dataPoint.Parent.LineThickness.Value < dataPointWidth / 2.0)
			{
				return dataPoint.Parent.LineThickness.Value;
			}
			return dataPointWidth / 4.0;
		}

		internal static void SetDataPointValues(IDataPoint dataPoint, ref double highY, ref double lowY, ref double openY, ref double closeY)
		{
			highY = (lowY = (openY = (closeY = 0.0)));
			switch (dataPoint.DpInfo.InternalYValues.Length)
			{
			case 0:
				break;
			case 1:
				openY = dataPoint.DpInfo.InternalYValues[0];
				return;
			case 2:
				openY = dataPoint.DpInfo.InternalYValues[0];
				closeY = dataPoint.DpInfo.InternalYValues[1];
				return;
			case 3:
				openY = dataPoint.DpInfo.InternalYValues[0];
				closeY = dataPoint.DpInfo.InternalYValues[1];
				highY = dataPoint.DpInfo.InternalYValues[2];
				return;
			default:
				openY = dataPoint.DpInfo.InternalYValues[0];
				closeY = dataPoint.DpInfo.InternalYValues[1];
				highY = dataPoint.DpInfo.InternalYValues[2];
				lowY = dataPoint.DpInfo.InternalYValues[3];
				break;
			}
		}

		private static Brush GetOpenCloseRectangleBorderbrush(IDataPoint dataPoint, Brush dataPointColor)
		{
			if (DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor) != null)
			{
				return (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor);
			}
			if (dataPointColor != null)
			{
				return dataPointColor;
			}
			return dataPoint.Parent.PriceUpColor;
		}

		private static Brush GetOpenCloseRectangleFillbrush(IDataPoint dataPoint, Brush dataPointColor)
		{
			if (dataPoint.DpInfo.IsPriceUp)
			{
				if (dataPointColor != null)
				{
					return dataPointColor;
				}
				return dataPoint.Parent.PriceUpColor;
			}
			else
			{
				if (dataPointColor != null)
				{
					return dataPointColor;
				}
				return dataPoint.Parent.PriceDownColor;
			}
		}

		internal static void ApplyOrRemoveBevel(IDataPoint dataPoint, double dataPointWidth)
		{
			Canvas canvas = null;
			Faces faces = dataPoint.DpInfo.Faces;
			Rectangle rectangle = faces.VisualComponents[1] as Rectangle;
			faces.ClearList(ref faces.BevelElements);
			Thickness thickness = (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness);
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled);
			if (dataPoint.Parent.Bevel.Value && dataPointWidth > 8.0 && rectangle.Height > 6.0)
			{
				double num = rectangle.StrokeThickness;
				if (dataPoint.Parent.SelectionEnabled && thickness.Left == 0.0)
				{
					num = 1.5 + num;
				}
				if (rectangle.Width - 2.0 * num >= 0.0 && rectangle.Height - 2.0 * num >= 0.0)
				{
					Brush openCloseRectangleFillbrush = CandleStick.GetOpenCloseRectangleFillbrush(dataPoint, DataPointHelper.GetColor_DataPoint(dataPoint));
					canvas = ExtendedGraphics.Get2DRectangleBevel(dataPoint, rectangle.Width - 2.0 * num, rectangle.Height - 2.0 * num, 5.0, 5.0, Graphics.GetBevelTopBrush(openCloseRectangleFillbrush), Graphics.GetBevelSideBrush((double)(flag ? -70 : 0), openCloseRectangleFillbrush), Graphics.GetBevelSideBrush((double)(flag ? -110 : 180), openCloseRectangleFillbrush), Graphics.GetBevelSideBrush(90.0, openCloseRectangleFillbrush));
					canvas.IsHitTestVisible = false;
					canvas.SetValue(Canvas.TopProperty, (double)rectangle.GetValue(Canvas.TopProperty) + num);
					canvas.SetValue(Canvas.LeftProperty, num);
					foreach (FrameworkElement item in canvas.Children)
					{
						dataPoint.DpInfo.Faces.BevelElements.Add(item);
					}
					dataPoint.DpInfo.Faces.BevelElements.Add(canvas);
					(dataPoint.DpInfo.Faces.Visual as Canvas).Children.Add(canvas);
				}
			}
		}

		internal static void ApplyOrRemoveShadow(IDataPoint dataPoint, double dataPointWidth)
		{
			Canvas canvas = dataPoint.DpInfo.Faces.Visual as Canvas;
			if (!VisifireControl.IsMediaEffectsEnabled)
			{
				Faces faces = dataPoint.DpInfo.Faces;
				Rectangle rectangle = faces.VisualComponents[1] as Rectangle;
				dataPoint.DpInfo.Faces.ClearList(ref faces.ShadowElements);
				bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled);
				BorderStyles borderStyles = (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle);
				Thickness thickness = (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness);
				if (flag)
				{
					Line line = new Line
					{
						IsHitTestVisible = false,
						X1 = canvas.Width / 2.0 + CandleStick._shadowDepth,
						X2 = canvas.Width / 2.0 + CandleStick._shadowDepth,
						Y1 = 0.0,
						Y2 = Math.Max(canvas.Height - CandleStick._shadowDepth, 1.0),
						Stroke = CandleStick._shadowColor,
						StrokeThickness = CandleStick.GetStrokeThickness(dataPoint, dataPointWidth),
						StrokeDashArray = Graphics.LineStyleToStrokeDashArray(borderStyles.ToString())
					};
					line.SetValue(Panel.ZIndexProperty, -4);
					line.SetValue(Canvas.TopProperty, CandleStick._shadowDepth);
					Rectangle rectangle2 = new Rectangle
					{
						IsHitTestVisible = false,
						Fill = CandleStick._shadowColor,
						Width = dataPointWidth,
						Height = rectangle.Height,
						Stroke = CandleStick._shadowColor,
						StrokeThickness = thickness.Left,
						StrokeDashArray = Graphics.LineStyleToStrokeDashArray(borderStyles.ToString())
					};
					rectangle2.SetValue(Canvas.TopProperty, (double)rectangle.GetValue(Canvas.TopProperty) + CandleStick._shadowDepth);
					rectangle2.SetValue(Canvas.LeftProperty, (double)rectangle.GetValue(Canvas.LeftProperty) + CandleStick._shadowDepth);
					rectangle2.SetValue(Panel.ZIndexProperty, -4);
					faces.ShadowElements.Add(line);
					faces.ShadowElements.Add(rectangle2);
					canvas.Children.Add(rectangle2);
					canvas.Children.Add(line);
					return;
				}
			}
			else
			{
				bool flag2 = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled);
				if (DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect) == null)
				{
					if (flag2)
					{
						canvas.Effect = ExtendedGraphics.GetShadowEffect(315.0, 5.0, 0.95);
						return;
					}
					canvas.Effect = null;
				}
			}
		}

		private static void UpdateYValueAndXValuePosition(IDataPoint dataPoint, double canvasWidth, double canvasHeight, double dataPointWidth, ref bool isLabelEnabled)
		{
			Faces faces = dataPoint.DpInfo.Faces;
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
			dataPoint.DpInfo.IsPriceUp = (num3 > num4);
			if (dataPoint.Parent.PlotGroup.AxisY.AxisMinimum != null)
			{
				double limitingYValue = dataPoint.Parent.PlotGroup.GetLimitingYValue();
				double num6 = Graphics.ValueToPixelPosition(canvasHeight, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, limitingYValue);
				if (num6 < num3 && num6 < num4 && num6 < num && num6 < num2)
				{
					isLabelEnabled = false;
				}
			}
			Canvas canvas = dataPoint.DpInfo.Faces.Visual as Canvas;
			canvas.Width = dataPointWidth;
			canvas.Height = Math.Abs(num2 - num);
			canvas.SetValue(Canvas.TopProperty, (num < num2) ? num : num2);
			canvas.SetValue(Canvas.LeftProperty, num5 - dataPointWidth / 2.0);
			Line line = dataPoint.DpInfo.Faces.VisualComponents[0] as Line;
			line.X1 = canvas.Width / 2.0;
			line.X2 = canvas.Width / 2.0;
			line.Y1 = 0.0;
			line.Y2 = canvas.Height;
			line.Tag = new ElementData
			{
				Element = dataPoint,
				VisualElementName = "HlLine"
			};
			Rectangle rectangle = faces.VisualComponents[1] as Rectangle;
			rectangle.Width = dataPointWidth;
			rectangle.Height = Math.Max(Math.Abs(num3 - num4), 1.0);
			rectangle.Tag = new ElementData
			{
				Element = dataPoint,
				VisualElementName = "OcRect"
			};
			rectangle.SetValue(Canvas.TopProperty, ((num4 > num3) ? num3 : num4) - (double)canvas.GetValue(Canvas.TopProperty));
			rectangle.SetValue(Canvas.LeftProperty, 0.0);
			CandleStick.ApplyOrUpdateColorForACandleStick(dataPoint);
			CandleStick.ApplyEffects(dataPoint);
			CandleStick.ApplyOrRemoveShadow(dataPoint, dataPointWidth);
			CandleStick.ApplyOrUpdateBorder(dataPoint, dataPointWidth);
			CandleStick.ApplyOrRemoveBevel(dataPoint, dataPointWidth);
			if (dataPoint.DpInfo.LabelVisual != null)
			{
				CandleStick.SetLabelPosition(dataPoint, canvasWidth, canvasHeight);
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
				if (DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect) != null)
				{
					canvas.Effect = (Effect)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect);
					return;
				}
				canvas.Effect = null;
			}
		}

		private static void ApplyOrUpdateBorder(IDataPoint dataPoint, double dataPointWidth)
		{
			Faces faces = dataPoint.DpInfo.Faces;
			Line line = dataPoint.DpInfo.Faces.VisualComponents[0] as Line;
			Rectangle rectangle = faces.VisualComponents[1] as Rectangle;
			BorderStyles borderStyles = (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle);
			Thickness thickness = (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness);
			line.StrokeThickness = CandleStick.GetStrokeThickness(dataPoint, dataPointWidth);
			line.StrokeDashArray = Graphics.LineStyleToStrokeDashArray(borderStyles.ToString());
			rectangle.StrokeThickness = thickness.Left;
			if (thickness.Left != 0.0)
			{
				rectangle.StrokeDashArray = Graphics.LineStyleToStrokeDashArray(borderStyles.ToString());
				rectangle.Stroke = CandleStick.GetOpenCloseRectangleBorderbrush(dataPoint, DataPointHelper.GetColor_DataPoint(dataPoint));
			}
		}

		internal static void CreateOrUpdateARasterCandleStick(WriteableBitmapAdapter wba, IDataPoint dataPoint, double width, double height, double dataPointWidth, double scrollOffsetAsPixel)
		{
			dataPoint.DpInfo.Faces = null;
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
			Axis axisY = parent.PlotGroup.AxisY;
			double valueMin = axisY.InternalAxisMinimum;
			double valueMax = axisY.InternalAxisMaximum;
			Axis axisX = parent.PlotGroup.AxisX;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			CandleStick.SetDataPointValues(dataPoint, ref num2, ref num3, ref num4, ref num5);
			num4 = Graphics.ValueToPixelPosition(height, 0.0, valueMin, valueMax, num4);
			num5 = Graphics.ValueToPixelPosition(height, 0.0, valueMin, valueMax, num5);
			num2 = Graphics.ValueToPixelPosition(height, 0.0, valueMin, valueMax, num2);
			num3 = Graphics.ValueToPixelPosition(height, 0.0, valueMin, valueMax, num3);
			double x = Graphics.ValueToPixelPosition(0.0, width, axisX.InternalAxisMinimum, axisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
			double num6 = (num2 < num3) ? num2 : num3;
			double num7 = Math.Abs(num3 - num2);
			dataPoint.DpInfo.VisualPosition = new Point(x, num6);
			Point visualPosition = dataPoint.DpInfo.VisualPosition;
			visualPosition.X -= Math.Abs(scrollOffsetAsPixel);
			dataPoint.DpInfo.RasterVisualPosition = visualPosition;
			dataPoint.DpInfo.IsPriceUp = (num4 > num5);
			LineStructure lineStructure = new LineStructure
			{
				PointA = new Point(dataPoint.DpInfo.RasterVisualPosition.X, num6),
				PointB = new Point(dataPoint.DpInfo.RasterVisualPosition.X, num6 + num7)
			};
			Rect rect = new Rect
			{
				Width = Math.Round(dataPointWidth),
				Height = Math.Max(Math.Abs(num4 - num5), 1.0),
				Y = ((num5 > num4) ? num4 : num5),
				X = Math.Round(dataPoint.DpInfo.RasterVisualPosition.X - num)
			};
			Brush color_DataPoint = DataPointHelper.GetColor_DataPoint(dataPoint);
			Brush openCloseRectangleFillbrush = CandleStick.GetOpenCloseRectangleFillbrush(dataPoint, color_DataPoint);
			Brush brush = ((Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.StickColor) != null) ? ((Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.StickColor)) : ((color_DataPoint == null) ? dataPoint.Parent.PriceUpColor : color_DataPoint);
			RasterShapeStyle rasterShapeStyle = new RasterLineStyle
			{
				Stroke = (brush as SolidColorBrush).Color,
				StrokeThickness = ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left,
				IsAntiAliased = parent.IsAntiAliased,
				StrokeStyle = parent.LineStyle,
				Opacity = parent.Opacity * (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity)
			};
			double num8 = rasterShapeStyle.StrokeThickness / 2.0;
			lineStructure.PointA.X = lineStructure.PointA.X - num8;
			lineStructure.PointB.X = lineStructure.PointB.X - num8;
			RasterRenderEngine.DrawLine(wba, lineStructure.PointA, lineStructure.PointB, rasterShapeStyle as RasterLineStyle);
			rasterShapeStyle.Fill = (openCloseRectangleFillbrush as SolidColorBrush).Color;
			rasterShapeStyle.Width = rect.Width;
			rasterShapeStyle.Height = rect.Height;
			RasterRenderEngine.DrawRect(wba, new Point(rect.X, rect.Y), rasterShapeStyle);
		}

		internal static void CreateOrUpdateACandleStick(IDataPoint dataPoint, Canvas candleStickCanvas, Canvas labelCanvas, double canvasWidth, double canvasHeight, double dataPointWidth)
		{
			Faces faces = dataPoint.DpInfo.Faces;
			if (faces != null && faces.Visual != null && candleStickCanvas == faces.Visual.Parent)
			{
				candleStickCanvas.Children.Remove(dataPoint.DpInfo.Faces.Visual);
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
			ElementData tag = new ElementData
			{
				Element = dataPoint
			};
			dataPoint.DpInfo.Faces = new Faces();
			Canvas canvas = new Canvas();
			dataPoint.DpInfo.Faces.Visual = canvas;
			Line line = new Line
			{
				Tag = tag
			};
			dataPoint.DpInfo.Faces.Parts.Add(line);
			dataPoint.DpInfo.Faces.VisualComponents.Add(line);
			canvas.Children.Add(line);
			Rectangle rectangle = new Rectangle
			{
				Tag = tag
			};
			dataPoint.DpInfo.Faces.VisualComponents.Add(rectangle);
			dataPoint.DpInfo.Faces.BorderElements.Add(rectangle);
			canvas.Children.Add(rectangle);
			bool flag = true;
			CandleStick.UpdateYValueAndXValuePosition(dataPoint, canvasWidth, canvasHeight, dataPointWidth, ref flag);
			candleStickCanvas.Children.Add(canvas);
			if (flag)
			{
				CandleStick.CreateAndPositionLabel(labelCanvas, dataPoint);
			}
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

		internal static void AttachEvents2Visual(VisifireElement obj, IDataPoint dataPoint)
		{
			foreach (FrameworkElement current in dataPoint.DpInfo.Faces.VisualComponents)
			{
				dataPoint.AttachEvents2Visual(obj, dataPoint, current);
			}
			if (dataPoint.DpInfo.LabelVisual != null)
			{
				dataPoint.AttachEvents2Visual(obj, dataPoint, dataPoint.DpInfo.LabelVisual);
			}
		}

		internal static void ApplyOrUpdateColorForACandleStick(IDataPoint dataPoint)
		{
			Faces arg_0B_0 = dataPoint.DpInfo.Faces;
			Brush color_DataPoint = DataPointHelper.GetColor_DataPoint(dataPoint);
			Line line = dataPoint.DpInfo.Faces.VisualComponents[0] as Line;
			Rectangle rectangle = dataPoint.DpInfo.Faces.VisualComponents[1] as Rectangle;
			Brush openCloseRectangleFillbrush = CandleStick.GetOpenCloseRectangleFillbrush(dataPoint, color_DataPoint);
			Brush brush;
			if (DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.StickColor) != null)
			{
				brush = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.StickColor);
			}
			else if (color_DataPoint == null)
			{
				brush = dataPoint.Parent.PriceUpColor;
			}
			else
			{
				brush = color_DataPoint;
			}
			if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled))
			{
				rectangle.Fill = Graphics.GetLightingEnabledBrush(openCloseRectangleFillbrush, "Linear", null);
				line.Stroke = Graphics.GetLightingEnabledBrush(brush, "Linear", null);
			}
			else
			{
				rectangle.Fill = openCloseRectangleFillbrush;
				line.Stroke = brush;
			}
			CandleStick.UpdateBevelLayerColor(dataPoint, openCloseRectangleFillbrush);
		}

		internal static void UpdateBevelLayerColor(IDataPoint dataPoint, Brush brush)
		{
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled);
			Faces faces = dataPoint.DpInfo.Faces;
			foreach (FrameworkElement current in faces.BevelElements)
			{
				Shape shape = current as Shape;
				string visualElementName;
				if (shape != null && (visualElementName = (shape.Tag as ElementData).VisualElementName) != null)
				{
					if (!(visualElementName == "TopBevel"))
					{
						if (!(visualElementName == "LeftBevel"))
						{
							if (!(visualElementName == "RightBevel"))
							{
								if (visualElementName == "BottomBevel")
								{
									shape.Fill = Graphics.GetBevelSideBrush(90.0, brush);
								}
							}
							else
							{
								shape.Fill = Graphics.GetBevelSideBrush((double)(flag ? -110 : 180), brush);
							}
						}
						else
						{
							shape.Fill = Graphics.GetBevelSideBrush((double)(flag ? -70 : 0), brush);
						}
					}
					else
					{
						shape.Fill = Graphics.GetBevelTopBrush(brush);
					}
				}
			}
		}

		internal static Canvas GetVisualObjectForCandleStick(Panel preExistingPanel, double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, double plankDepth, bool animationEnabled)
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
			if (num4 == 0 || num5 == 0)
			{
				return null;
			}
			double num6 = 0.0;
			foreach (DataSeries current in seriesList)
			{
				if (!(current.Enabled == false))
				{
					num6 = current.PlotGroup.AxisX.GetAxisVisualLeftAsOffset();
					if (double.IsNaN(num6))
					{
						num6 = 0.0;
					}
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
							CandleStick.CreateOrUpdateARasterCandleStick(writeableBitmapAdapter, current2, width, height, dataPointWidth, num6);
							if (indicatorEnabled)
							{
								current2.DpInfo.ParsedToolTipText = current2.TextParser(current2.ToolTipText);
							}
						}
						else
						{
							CandleStick.CreateOrUpdateACandleStick(current2, canvas3, canvas2, width, height, dataPointWidth);
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
				dataSeries.Storyboard = AnimationHelper.ApplyOpacityAnimation(canvas2, dataSeries, dataSeries.Storyboard, num3, 1.0, 0.0, 1.0);
			}
			canvas3.Tag = null;
			CandleStick.AddToVisual(canvas, preExistingPanel, canvas3, canvas2);
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			rectangleGeometry.Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_OFFSET, height + chart.ChartArea.PLANK_DEPTH);
			canvas.Clip = rectangleGeometry;
			return canvas;
		}

		private static void UpdateRasterCandleStickSeries(DataSeries series, double width, double height)
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
			Canvas plotAreaCanvas = chart.ChartArea.PlotAreaCanvas;
			int num = (int)plotAreaCanvas.Width;
			int num2 = (int)plotAreaCanvas.Height;
			if (num2 == 0 || num == 0)
			{
				return;
			}
			WriteableBitmapAdapter writeableBitmapAdapter = new WriteableBitmapAdapter(num, num2);
			bool indicatorEnabled = chart.IndicatorEnabled;
			double dataPointWidth = CandleStick.CalculateDataPointWidth(width, height, chart);
			double num3 = series.PlotGroup.AxisX.GetAxisVisualLeftAsOffset();
			if (double.IsNaN(num3))
			{
				num3 = 0.0;
			}
			foreach (IDataPoint current in dataPointsUnderViewPort)
			{
				if (series.LightWeight.Value)
				{
					CandleStick.CreateOrUpdateARasterCandleStick(writeableBitmapAdapter, current, width, height, dataPointWidth, num3);
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

		private static void AddToVisual(Panel visual, Panel preExistingPanel, Canvas candleStickCanvas, Canvas labelCanvas)
		{
			if (preExistingPanel != null)
			{
				visual.Children.RemoveAt(1);
				visual.Children.Add(candleStickCanvas);
				return;
			}
			labelCanvas.SetValue(Panel.ZIndexProperty, 1);
			visual.Children.Add(labelCanvas);
			visual.Children.Add(candleStickCanvas);
		}

		internal static void CreateAndPositionLabel(Canvas labelCanvas, IDataPoint dataPoint)
		{
			if (dataPoint.DpInfo.LabelVisual != null)
			{
				Panel panel = dataPoint.DpInfo.LabelVisual.Parent as Panel;
				if (panel != null)
				{
					panel.Children.Remove(dataPoint.DpInfo.LabelVisual);
				}
			}
			Brush labelFontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
			FontStyle fontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
			Brush background = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
			FontFamily fontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
			double fontSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontSize);
			FontWeight fontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled);
			string text = (string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText);
			if (flag && !string.IsNullOrEmpty(text))
			{
				FrameworkElement arg_D4_0 = dataPoint.DpInfo.Faces.Visual;
				Title title = new Title
				{
					Text = dataPoint.TextParser(text),
					FontFamily = fontFamily,
					FontSize = fontSize,
					FontWeight = fontWeight,
					FontStyle = fontStyle,
					Background = background,
					FontColor = Chart.CalculateDataPointLabelFontColor(dataPoint.Chart as Chart, dataPoint, labelFontColor, LabelStyles.OutSide)
				};
				title.CreateVisualObject(new ElementData
				{
					Element = dataPoint
				}, dataPoint.Chart.FlowDirection);
				title.Visual.Height = title.Height;
				title.Visual.Width = title.Width;
				dataPoint.DpInfo.LabelVisual = title.Visual;
				CandleStick.SetLabelPosition(dataPoint, labelCanvas.Width, labelCanvas.Height);
				labelCanvas.Children.Add(title.Visual);
			}
		}

		private static void SetLabelPosition(IDataPoint dataPoint, double canvasWidth, double canvasHeight)
		{
			Canvas canvas = dataPoint.DpInfo.Faces.Visual as Canvas;
			double num = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelAngle);
			double num2;
			double num3;
			if (double.IsNaN(num) || num == 0.0)
			{
				num2 = (double)canvas.GetValue(Canvas.TopProperty) - dataPoint.DpInfo.LabelVisual.Height;
				num3 = (double)canvas.GetValue(Canvas.LeftProperty) + (canvas.Width - dataPoint.DpInfo.LabelVisual.Width) / 2.0;
				if (num2 < 0.0)
				{
					num2 = (double)canvas.GetValue(Canvas.TopProperty);
				}
				if (num3 < 0.0)
				{
					num3 = 1.0;
				}
				if (num3 + dataPoint.DpInfo.LabelVisual.Width > canvasWidth)
				{
					num3 = canvasWidth - dataPoint.DpInfo.LabelVisual.Width - 2.0;
				}
				dataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, num3);
				dataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, num2);
				return;
			}
			Point point = new Point((double)canvas.GetValue(Canvas.LeftProperty) + canvas.Width / 2.0, (double)canvas.GetValue(Canvas.TopProperty));
			double num4 = 4.0;
			double num5 = 0.0;
			double num6 = 0.0;
			if (num > 0.0 && num <= 90.0)
			{
				num5 = num - 180.0;
				num6 = 0.017453292519943295 * num5;
				num4 += dataPoint.DpInfo.LabelVisual.Width;
				num5 = (num6 - 3.1415926535897931) * 57.295779513082323;
			}
			else if (num >= -90.0 && num < 0.0)
			{
				num5 = num;
				num6 = 0.017453292519943295 * num5;
			}
			num3 = point.X + num4 * Math.Cos(num6);
			num2 = point.Y + num4 * Math.Sin(num6);
			num2 -= dataPoint.DpInfo.LabelVisual.Height / 2.0;
			dataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, num3);
			dataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, num2);
			dataPoint.DpInfo.LabelVisual.RenderTransformOrigin = new Point(0.0, 0.5);
			dataPoint.DpInfo.LabelVisual.RenderTransform = new RotateTransform
			{
				CenterX = 0.0,
				CenterY = 0.0,
				Angle = num5
			};
		}

		public static void Update(IElement sender, ElementTypes elementType, VcProperties property, object newValue, bool isAxisChanged)
		{
			bool flag = elementType == ElementTypes.DataPoint;
			if (flag)
			{
				CandleStick.UpdateDataPoint(sender as IDataPoint, property, newValue, isAxisChanged);
				return;
			}
			CandleStick.UpdateDataSeries(sender as DataSeries, property, newValue, isAxisChanged);
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
				CandleStick.UpdateRasterCandleStickSeries(dataSeries, width, height);
				return;
			}
			if (property == VcProperties.DataPoints || property == VcProperties.XValue || property == VcProperties.YValues)
			{
				chart.ChartArea.RenderSeries();
				return;
			}
			foreach (IDataPoint current in dataSeries.InternalDataPoints)
			{
				CandleStick.UpdateDataPoint(current, property, newValue, isAxisChanged);
			}
		}

		private static void UpdateRasterDataPoint(IDataPoint dataPoint)
		{
			DataSeries parent = dataPoint.Parent;
			PlotGroup arg_0D_0 = parent.PlotGroup;
			Faces faces = dataPoint.DpInfo.Faces;
			Faces faces2 = parent.Faces;
			if (faces2 == null)
			{
				return;
			}
			double dataPointWidth;
			if (faces != null)
			{
				dataPointWidth = faces.Width;
			}
			else
			{
				dataPointWidth = CandleStick.CalculateDataPointWidth(faces2.Visual.Width, faces2.Visual.Height, parent.Chart as Chart);
			}
			Canvas canvas = faces2.Visual as Canvas;
			double num = dataPoint.Parent.PlotGroup.AxisX.GetAxisVisualLeftAsOffset();
			if (double.IsNaN(num))
			{
				num = 0.0;
			}
			using (WriteableBitmapAdapter writeableBitmapAdapter = new WriteableBitmapAdapter((canvas.Children[0] as Image).Source as BitmapSource))
			{
				CandleStick.CreateOrUpdateARasterCandleStick(writeableBitmapAdapter, dataPoint, canvas.Width, canvas.Height, dataPointWidth, num);
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
			PlotGroup arg_1D_0 = parent.PlotGroup;
			Faces faces = parent.Faces;
			Faces faces2 = dataPoint.DpInfo.Faces;
			if (faces != null)
			{
				RenderHelper.UpdateParentVisualCanvasSize(chart, faces.Visual as Canvas);
			}
			if (parent.LightWeight.Value)
			{
				CandleStick.UpdateRasterDataPoint(dataPoint);
				return;
			}
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
				CandleStick.CreateOrUpdateACandleStick(dataPoint, faces.Visual as Canvas, faces.LabelCanvas, faces.Visual.Width, faces.Visual.Height, dataPointWidth);
				return;
			}
			if (faces2 == null)
			{
				return;
			}
			if (property <= VcProperties.LegendText)
			{
				if (property > VcProperties.Enabled)
				{
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
							return;
						}
						break;
					}
					CandleStick.CreateAndPositionLabel(faces.LabelCanvas, dataPoint);
					return;
				}
				switch (property)
				{
				case VcProperties.Bevel:
					CandleStick.ApplyOrRemoveBevel(dataPoint, dataPointWidth);
					return;
				case VcProperties.BorderColor:
					CandleStick.ApplyOrUpdateBorder(dataPoint, dataPointWidth);
					return;
				case VcProperties.BorderStyle:
					CandleStick.ApplyOrUpdateBorder(dataPoint, dataPointWidth);
					return;
				case VcProperties.BorderThickness:
					CandleStick.ApplyOrUpdateBorder(dataPoint, dataPointWidth);
					CandleStick.ApplyOrRemoveBevel(dataPoint, dataPointWidth);
					return;
				case VcProperties.BubbleStyle:
				case VcProperties.ColorSet:
				case VcProperties.CornerRadius:
				case VcProperties.ClosestPlotDistance:
					return;
				case VcProperties.Color:
					break;
				case VcProperties.Cursor:
					DataPointHelper.SetCursor2DataPointVisualFaces(dataPoint);
					return;
				default:
					switch (property)
					{
					case VcProperties.Effect:
						CandleStick.ApplyEffects(dataPoint);
						return;
					case VcProperties.Enabled:
						CandleStick.CreateOrUpdateACandleStick(dataPoint, faces.Visual as Canvas, faces.LabelCanvas, faces.Visual.Width, faces.Visual.Height, dataPointWidth);
						return;
					default:
						return;
					}
					break;
				}
			}
			else if (property <= VcProperties.Opacity)
			{
				if (property == VcProperties.LightingEnabled)
				{
					CandleStick.ApplyOrUpdateColorForACandleStick(dataPoint);
					return;
				}
				if (property != VcProperties.Opacity)
				{
					return;
				}
				faces2.Visual.Opacity = parent.Opacity * (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity);
				return;
			}
			else
			{
				switch (property)
				{
				case VcProperties.PriceDownColor:
				case VcProperties.PriceUpColor:
					break;
				default:
					switch (property)
					{
					case VcProperties.ShadowEnabled:
						CandleStick.ApplyOrRemoveShadow(dataPoint, dataPointWidth);
						return;
					case VcProperties.ShowInLegend:
						chart.InvokeRender();
						return;
					case VcProperties.StartAngle:
					case VcProperties.StartFromZero:
						return;
					case VcProperties.StickColor:
						break;
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
								CandleStick.UpdateDataSeries(parent, property, newValue, isAxisChanged);
							}
							else
							{
								dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
								if (!dataPoint.IsLightDataPoint)
								{
									(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
								}
								bool flag = true;
								CandleStick.UpdateYValueAndXValuePosition(dataPoint, faces.Visual.Width, faces.Visual.Height, faces2.Visual.Width, ref flag);
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
					break;
				}
			}
			CandleStick.ApplyOrUpdateColorForACandleStick(dataPoint);
		}
	}
}
