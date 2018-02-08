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
	internal class BarChart
	{
		internal static double BAR_GAP_RATIO = 0.2;

		private static DataSeries currentDataSeries
		{
			get;
			set;
		}

		private static void CalculateAutoPlacement(bool isView3D, IDataPoint dataPoint, Size barVisualSize, bool isPositive, LabelStyles labelStyle, ref double labelLeft, ref double labelTop, ref double angle, double canvasLeft, double canvasTop, double canvasRight, bool isVertical, double insideGap, double outsideGap, Title tb)
		{
			double num = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
			double num2 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
			if (isPositive)
			{
				if (labelStyle != LabelStyles.Inside)
				{
					labelLeft = canvasRight + num / 2.0 * num2 + outsideGap;
					labelTop = canvasTop + (barVisualSize.Height - tb.TextBlockDesiredSize.Height) / 2.0 + 6.0;
					return;
				}
				if (barVisualSize.Width - insideGap - num / 2.0 * num2 >= tb.TextBlockDesiredSize.Width)
				{
					labelLeft = canvasRight - tb.TextBlockDesiredSize.Width - num / 2.0 * (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale) - insideGap;
					labelTop = canvasTop + (barVisualSize.Height - tb.TextBlockDesiredSize.Height) / 2.0 + 6.0;
					return;
				}
				labelLeft = canvasLeft + insideGap;
				labelTop = canvasTop + (barVisualSize.Height - tb.TextBlockDesiredSize.Height) / 2.0 + 6.0;
				return;
			}
			else
			{
				if (labelStyle != LabelStyles.Inside)
				{
					labelLeft = canvasLeft - tb.TextBlockDesiredSize.Width - (num / 2.0 * num2 + outsideGap);
					labelTop = canvasTop + (barVisualSize.Height - tb.TextBlockDesiredSize.Height) / 2.0 + 6.0;
					return;
				}
				if (barVisualSize.Width - insideGap - num / 2.0 * num2 >= tb.TextBlockDesiredSize.Width)
				{
					labelLeft = canvasLeft + (num / 2.0 * num2 + insideGap);
					labelTop = canvasTop + (barVisualSize.Height - tb.TextBlockDesiredSize.Height) / 2.0 + 6.0;
					return;
				}
				labelLeft = canvasRight - tb.TextBlockDesiredSize.Width - insideGap;
				labelTop = canvasTop + (barVisualSize.Height - tb.TextBlockDesiredSize.Height) / 2.0 + 6.0;
				return;
			}
		}

		private static void CreateLabel(Chart chart, Size barVisualSize, bool isPositive, bool isTopOfStack, IDataPoint dataPoint, double canvasLeft, double canvasTop, double canvasRight, Canvas labelCanvas)
		{
			if (dataPoint.DpInfo.Faces == null)
			{
				return;
			}
			LabelStyles labelStyles = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			double num = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelAngle);
			Brush labelFontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
			FontStyle internalFontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
			Brush internalBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
			FontFamily internalFontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
			double internalFontSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontSize);
			FontWeight internalFontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
			LabelStyles labelStyles2 = labelStyles;
			if (isPositive || dataPoint.YValue == 0.0)
			{
				isPositive = true;
			}
			canvasTop -= 7.0;
			double num2 = 0.0;
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled);
			string text = (string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText);
			if (flag && !string.IsNullOrEmpty(text))
			{
				Title title = new Title
				{
					Text = dataPoint.TextParser(text),
					InternalFontFamily = internalFontFamily,
					InternalFontSize = internalFontSize,
					InternalFontWeight = internalFontWeight,
					InternalFontStyle = internalFontStyle,
					InternalBackground = internalBackground,
					InternalFontColor = Chart.CalculateDataPointLabelFontColor(dataPoint.Chart as Chart, dataPoint, labelFontColor, labelStyles2),
					Tag = new ElementData
					{
						Element = dataPoint
					}
				};
				title.CreateVisualObject(new ElementData
				{
					Element = dataPoint
				}, dataPoint.Chart.FlowDirection);
				double num3 = 0.0;
				double num4 = 0.0;
				double outsideGap = (double)(chart.View3D ? 5 : 3);
				double insideGap = (double)(chart.View3D ? 4 : 3);
				if (double.IsNaN(num) || num == 0.0)
				{
					bool flag2 = false;
					if (!dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet && !isTopOfStack && dataPoint.Parent.RenderAs != RenderAs.Bar)
					{
						labelStyles2 = LabelStyles.Inside;
					}
					BarChart.CalculateAutoPlacement(chart.View3D, dataPoint, barVisualSize, isPositive, labelStyles2, ref num4, ref num3, ref num2, canvasLeft, canvasTop, canvasRight, flag2, insideGap, outsideGap, title);
					title.Visual.SetValue(Canvas.LeftProperty, num4);
					title.Visual.SetValue(Canvas.TopProperty, num3);
					title.Visual.RenderTransformOrigin = new Point(0.0, 0.5);
					title.Visual.RenderTransform = new RotateTransform
					{
						CenterX = 0.0,
						CenterY = 0.0,
						Angle = num2
					};
					if (!dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
					{
						if (isPositive)
						{
							if (num4 + title.TextBlockDesiredSize.Width > chart.PlotArea.BorderElement.Width)
							{
								labelStyles2 = LabelStyles.Inside;
							}
						}
						else if (num4 < 0.0)
						{
							labelStyles2 = LabelStyles.Inside;
						}
					}
					if (labelStyles2 != labelStyles)
					{
						BarChart.CalculateAutoPlacement(chart.View3D, dataPoint, barVisualSize, isPositive, labelStyles2, ref num4, ref num3, ref num2, canvasLeft, canvasTop, canvasRight, flag2, insideGap, outsideGap, title);
						title.Visual.SetValue(Canvas.LeftProperty, num4);
						title.Visual.SetValue(Canvas.TopProperty, num3);
					}
					if (chart.SmartLabelEnabled && (dataPoint.Parent.RenderAs == RenderAs.StackedBar || dataPoint.Parent.RenderAs == RenderAs.StackedBar100) && !flag2 && labelStyles2 == LabelStyles.Inside && barVisualSize.Width < title.Visual.Width)
					{
						return;
					}
				}
				else
				{
					double num5 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
					double num6 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
					if (isPositive)
					{
						Point centerOfRotation = new Point(canvasRight + num5 / 2.0 * num6, canvasTop + barVisualSize.Height / 2.0 + 6.0);
						double num7 = 0.0;
						num2 = 0.0;
						if (labelStyles2 == LabelStyles.OutSide)
						{
							if (num <= 90.0 && num >= -90.0)
							{
								num2 = num;
								num7 += 4.0;
								double num8 = 0.017453292519943295 * num2;
								BarChart.SetRotation(num7, num2, num8, centerOfRotation, num4, num3, title);
							}
						}
						else
						{
							centerOfRotation = new Point(canvasRight - num5 / 2.0 * num6, canvasTop + barVisualSize.Height / 2.0 + 6.0);
							if (num > 0.0 && num <= 90.0)
							{
								num2 = num - 180.0;
								double num8 = 0.017453292519943295 * num2;
								num7 += title.TextBlockDesiredSize.Width + 3.0;
								num2 = (num8 - 3.1415926535897931) * 57.295779513082323;
								BarChart.SetRotation(num7, num2, num8, centerOfRotation, num4, num3, title);
							}
							else if (num >= -90.0 && num < 0.0)
							{
								num2 = num - 180.0;
								double num8 = 0.017453292519943295 * num2;
								num7 += title.TextBlockDesiredSize.Width + 4.0;
								num2 = (num8 - 3.1415926535897931) * 57.295779513082323;
								BarChart.SetRotation(num7, num2, num8, centerOfRotation, num4, num3, title);
							}
						}
					}
					else
					{
						Point centerOfRotation2 = new Point(canvasLeft - num5 / 2.0 * num6, canvasTop + barVisualSize.Height / 2.0 + 6.0);
						double num9 = 0.0;
						num2 = 0.0;
						if (labelStyles2 == LabelStyles.OutSide)
						{
							if (num > 0.0 && num <= 90.0)
							{
								num2 = num - 180.0;
								double num10 = 0.017453292519943295 * num2;
								num9 += title.TextBlockDesiredSize.Width + 3.0;
								num2 = (num10 - 3.1415926535897931) * 57.295779513082323;
								BarChart.SetRotation(num9, num2, num10, centerOfRotation2, num4, num3, title);
							}
							else if (num >= -90.0 && num < 0.0)
							{
								num2 = num - 180.0;
								double num10 = 0.017453292519943295 * num2;
								num9 += title.TextBlockDesiredSize.Width + 3.0;
								num2 = (num10 - 3.1415926535897931) * 57.295779513082323;
								BarChart.SetRotation(num9, num2, num10, centerOfRotation2, num4, num3, title);
							}
						}
						else
						{
							centerOfRotation2 = new Point(canvasLeft + num5 / 2.0 * num6, canvasTop + barVisualSize.Height / 2.0 + 6.0);
							if (num <= 90.0 && num >= -90.0)
							{
								num2 = num;
								num9 += 3.0;
								double num10 = 0.017453292519943295 * num2;
								BarChart.SetRotation(num9, num2, num10, centerOfRotation2, num4, num3, title);
							}
						}
					}
				}
				if (labelStyles2 != labelStyles)
				{
					title.TextElement.Foreground = Chart.CalculateDataPointLabelFontColor(dataPoint.Chart as Chart, dataPoint, labelFontColor, (dataPoint.YValue == 0.0) ? LabelStyles.OutSide : labelStyles2);
				}
				dataPoint.DpInfo.LabelVisual = title.Visual;
				labelCanvas.Children.Add(title.Visual);
			}
		}

		private static void SetRotation(double radius, double angle, double angleInRadian, Point centerOfRotation, double labelLeft, double labelTop, Title textBlock)
		{
			labelLeft = centerOfRotation.X + radius * Math.Cos(angleInRadian);
			labelTop = centerOfRotation.Y + radius * Math.Sin(angleInRadian);
			labelTop -= textBlock.TextBlockDesiredSize.Height / 2.0;
			textBlock.Visual.SetValue(Canvas.LeftProperty, labelLeft);
			textBlock.Visual.SetValue(Canvas.TopProperty, labelTop);
			textBlock.Visual.RenderTransformOrigin = new Point(0.0, 0.5);
			textBlock.Visual.RenderTransform = new RotateTransform
			{
				CenterX = 0.0,
				CenterY = 0.0,
				Angle = angle
			};
		}

		private static void SetMarkerPosition(Chart chart, IDataPoint dataPoint, bool isPositive, string labelText, Size markerSize, double canvasLeft, double canvasTop, Point markerPosition)
		{
			if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && !string.IsNullOrEmpty(labelText))
			{
				LabelStyles labelStyles = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
				dataPoint.DpInfo.Marker.CreateVisual((chart != null) ? chart.FlowDirection : FlowDirection.LeftToRight);
				if (isPositive)
				{
					if (canvasLeft + markerPosition.X + dataPoint.DpInfo.Marker.MarkerActualSize.Width > chart.PlotArea.BorderElement.Width)
					{
						labelStyles = LabelStyles.Inside;
					}
				}
				else if (canvasLeft < dataPoint.DpInfo.Marker.MarkerActualSize.Width)
				{
					labelStyles = LabelStyles.Inside;
				}
				dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Center;
				if (!(bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
				{
					if (chart.View3D)
					{
						if (labelStyles == LabelStyles.OutSide)
						{
							dataPoint.DpInfo.Marker.MarkerSize = new Size(markerSize.Width + chart.ChartArea.PLANK_DEPTH, markerSize.Height + chart.ChartArea.PLANK_DEPTH);
						}
						else
						{
							dataPoint.DpInfo.Marker.MarkerSize = new Size(markerSize.Width, markerSize.Height);
						}
					}
				}
				else if (chart.View3D)
				{
					labelStyles = LabelStyles.Inside;
				}
				if (isPositive)
				{
					dataPoint.DpInfo.Marker.TextAlignmentX = ((labelStyles == LabelStyles.Inside) ? AlignmentX.Left : AlignmentX.Right);
					return;
				}
				dataPoint.DpInfo.Marker.TextAlignmentX = ((labelStyles == LabelStyles.Inside) ? AlignmentX.Right : AlignmentX.Left);
			}
		}

		private static double CalculateHeightOfEachColumn(ref double top, double heightPerBar, double height)
		{
			return heightPerBar;
		}

		private static int GetBarZIndex(double left, double top, double height, bool isPositive)
		{
			int num = (int)(height - top);
			int num2 = (int)Math.Sqrt(Math.Pow(left / 2.0, 2.0) + Math.Pow((double)num, 2.0));
			if (isPositive)
			{
				return num2;
			}
			return -2147483648 + num2;
		}

		private static int GetStackedBarZIndex(double plotAreaCanvasHeight, double left, double top, double width, double height, bool isPositive, int index)
		{
			double num = Math.Pow(10.0, (double)((int)(Math.Log10(width) - 1.0)));
			int num2 = (int)(left / ((num < 1.0) ? 1.0 : num));
			int num3;
			if (top < plotAreaCanvasHeight)
			{
				num3 = (int)((plotAreaCanvasHeight - top) * num) + num2;
			}
			else
			{
				num3 = (int)((height - top) * num) + num2;
			}
			num3 /= 2;
			if (isPositive)
			{
				return num3 + index;
			}
			return -2147483648 + num3 + index;
		}

		private static Storyboard ApplyBarChartAnimation(Panel bar, Storyboard storyboard, bool isPositive, double beginTime)
		{
			ScaleTransform scaleTransform = new ScaleTransform
			{
				ScaleX = 0.0
			};
			bar.RenderTransform = scaleTransform;
			if (isPositive)
			{
				bar.RenderTransformOrigin = new Point(0.0, 0.5);
			}
			else
			{
				bar.RenderTransformOrigin = new Point(1.0, 0.5);
			}
			DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				1.0
			});
			DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				0.75
			});
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(0.5, 1.0)
			});
			DoubleAnimationUsingKeyFrames value = AnimationHelper.CreateDoubleAnimation(BarChart.currentDataSeries, scaleTransform, "(ScaleTransform.ScaleX)", beginTime, frameTime, values, splines);
			storyboard.Children.Add(value);
			return storyboard;
		}

		private static Storyboard ApplyStackedBarChartAnimation(Panel bar, Storyboard storyboard, double begin, double duration)
		{
			ScaleTransform scaleTransform = new ScaleTransform
			{
				ScaleX = 0.0
			};
			bar.RenderTransform = scaleTransform;
			bar.RenderTransformOrigin = new Point(0.5, 0.5);
			DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				1.5,
				0.75,
				1.125,
				0.9325,
				1.0
			});
			DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				0.25 * duration,
				0.5 * duration,
				0.75 * duration,
				1.0 * duration,
				1.25 * duration
			});
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 0.5),
				new Point(0.0, 0.0),
				new Point(0.5, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 0.5),
				new Point(0.0, 0.0),
				new Point(0.5, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 0.5),
				new Point(0.0, 0.0),
				new Point(0.5, 1.0)
			});
			DoubleAnimationUsingKeyFrames value = AnimationHelper.CreateDoubleAnimation(BarChart.currentDataSeries, scaleTransform, "(ScaleTransform.ScaleX)", begin + 0.5, frameTime, values, splines);
			storyboard.Children.Add(value);
			return storyboard;
		}

		internal static void CreateOrUpdateMarker4HorizontalChart(double width, Chart chart, Canvas labelCanvas, IDataPoint dataPoint, double left, double top, bool isPositive, double depth3d, double prevSum)
		{
			if (dataPoint.DpInfo.Faces == null)
			{
				return;
			}
			Canvas canvas = dataPoint.DpInfo.Faces.Visual as Canvas;
			if (chart.ChartArea != null && !chart.ChartArea._isFirstTimeRender)
			{
				ColumnChart.CleanUpMarkerAndLabel(dataPoint, labelCanvas);
			}
			if (!DataPointHelper.GetMarkerEnabled_DataPoint(dataPoint).HasValue && dataPoint.Parent.GetValue(DataSeries.MarkerEnabledProperty) == null && !DataPointHelper.GetLabelEnabled_DataPoint(dataPoint).HasValue && dataPoint.Parent.GetValue(DataSeries.LabelEnabledProperty) == null)
			{
				return;
			}
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled);
			double num = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
			if (flag)
			{
				Size markerSize = new Size(num, num);
				string labelText = "";
				double arg_C4_0 = dataPoint.DpInfo.InternalYValue;
				Marker marker = ColumnChart.CreateNewMarker(dataPoint, markerSize, labelText);
				dataPoint.DpInfo.Marker = marker;
				if (!flag)
				{
					marker.MarkerFillColor = Graphics.TRANSPARENT_BRUSH;
					marker.BorderColor = Graphics.TRANSPARENT_BRUSH;
				}
				Point point = default(Point);
				if (isPositive)
				{
					if (chart.View3D)
					{
						point = new Point(canvas.Width, canvas.Height / 2.0);
					}
					else
					{
						point = new Point(canvas.Width, canvas.Height / 2.0);
					}
				}
				else if (chart.View3D)
				{
					point = new Point(0.0, canvas.Height / 2.0);
				}
				else
				{
					point = new Point(0.0, canvas.Height / 2.0);
				}
				marker.Tag = new ElementData
				{
					Element = dataPoint
				};
				marker.CreateVisual((chart != null) ? chart.FlowDirection : FlowDirection.LeftToRight);
				marker.AddToParent(labelCanvas, left + point.X, top + point.Y, new Point(0.5, 0.5));
				if (marker != null && marker.Visual != null && !chart.IndicatorEnabled)
				{
					dataPoint.AttachToolTip(chart, dataPoint, marker.Visual);
				}
				if (dataPoint.Parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					double limitingYValue = dataPoint.Parent.PlotGroup.GetLimitingYValue();
					if ((prevSum < limitingYValue && limitingYValue > 0.0) || (prevSum < limitingYValue && dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || prevSum < dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum)
					{
						dataPoint.DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
					}
				}
				if (dataPoint.Parent.PlotGroup.AxisY.AxisMaximum != null)
				{
					dataPoint.Parent.PlotGroup.GetLimitingYValue();
					if (prevSum > dataPoint.Parent.PlotGroup.AxisY.InternalAxisMaximum)
					{
						dataPoint.DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
					}
				}
			}
			if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled))
			{
				double internalYValue = dataPoint.DpInfo.InternalYValue;
				double canvasRight = left + canvas.Width;
				if (dataPoint.Parent.PlotGroup.AxisY.AxisMaximum != null && prevSum > dataPoint.Parent.PlotGroup.AxisY.InternalAxisMaximum)
				{
					if (dataPoint.Parent.RenderAs == RenderAs.StackedBar)
					{
						if (internalYValue > dataPoint.Parent.PlotGroup.AxisY.InternalAxisMaximum)
						{
							canvasRight = Graphics.ValueToPixelPosition(0.0, width, dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum, dataPoint.Parent.PlotGroup.AxisY.InternalAxisMaximum, double.IsNaN(internalYValue) ? 0.0 : dataPoint.Parent.PlotGroup.AxisY.InternalAxisMaximum);
						}
					}
					else
					{
						canvasRight = Graphics.ValueToPixelPosition(0.0, width, dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum, dataPoint.Parent.PlotGroup.AxisY.InternalAxisMaximum, double.IsNaN(internalYValue) ? 0.0 : dataPoint.Parent.PlotGroup.AxisY.InternalAxisMaximum);
					}
				}
				else if (left < 0.0 && dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum < 0.0 && dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum > prevSum)
				{
					left = 0.0;
				}
				BarChart.CreateLabel(chart, new Size(canvas.Width, canvas.Height), isPositive, dataPoint.DpInfo.IsTopOfStack, dataPoint, left, top, canvasRight, labelCanvas);
				if (dataPoint.Parent.PlotGroup.AxisY.AxisMinimum != null && dataPoint.DpInfo.InternalYValue > 0.0)
				{
					if (!dataPoint.DpInfo.IsTopOfStack)
					{
						double limitingYValue2 = dataPoint.Parent.PlotGroup.GetLimitingYValue();
						if (((prevSum < limitingYValue2 && limitingYValue2 > 0.0) || (prevSum < limitingYValue2 && dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || prevSum < dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum) && dataPoint.DpInfo.LabelVisual != null)
						{
							(((dataPoint.DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Collapsed;
						}
					}
				}
				else if (dataPoint.Parent.PlotGroup.AxisY.AxisMaximum != null && !dataPoint.DpInfo.IsTopOfStack)
				{
					dataPoint.Parent.PlotGroup.GetLimitingYValue();
					double arg_623_0 = dataPoint.DpInfo.InternalYValue;
					double arg_622_0 = dataPoint.Parent.PlotGroup.AxisY.InternalAxisMaximum;
				}
				if (dataPoint.DpInfo.LabelVisual != null && !chart.IndicatorEnabled)
				{
					dataPoint.AttachToolTip(chart, dataPoint, dataPoint.DpInfo.LabelVisual);
				}
			}
		}

		internal static void CreateBarDataPointVisual(IDataPoint dataPoint, Canvas labelCanvas, Canvas columnCanvas, bool isPositive, double heightPerBar, double depth3d, bool animationEnabled)
		{
			double width = columnCanvas.Width;
			double height = columnCanvas.Height;
			Chart chart = dataPoint.Chart as Chart;
			dataPoint.DpInfo.OldLightingState = false;
			PlotDetails plotDetails = chart.PlotDetails;
			PlotGroup plotGroup = dataPoint.Parent.PlotGroup;
			double internalXValue = dataPoint.DpInfo.InternalXValue;
			double num = 0.0;
			if (plotGroup.AxisY.InternalAxisMinimum > 0.0)
			{
				num = plotGroup.AxisY.InternalAxisMinimum;
			}
			if (plotGroup.AxisY.InternalAxisMaximum < 0.0)
			{
				num = 0.0;
			}
			List<DataSeries> seriesFromDataPoint = plotDetails.GetSeriesFromDataPoint(dataPoint);
			int num2 = seriesFromDataPoint.IndexOf(dataPoint.Parent);
			double internalYValue = dataPoint.DpInfo.InternalYValue;
			double num3 = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, 0.0);
			double num4 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, internalXValue);
			num4 += ((double)num2 - (double)seriesFromDataPoint.Count<DataSeries>() / 2.0) * heightPerBar;
			double num5;
			double num6;
			if (isPositive)
			{
				num5 = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num);
				num6 = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, double.IsNaN(internalYValue) ? 0.0 : internalYValue);
				if (plotGroup.AxisY.AxisMinimum != null && internalYValue < num)
				{
					num6 = (num5 = num3 - 100.0);
				}
				if (chart.View3D && internalYValue > plotGroup.AxisY.InternalAxisMaximum)
				{
					num6 = width;
				}
			}
			else
			{
				num5 = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, double.IsNaN(internalYValue) ? 0.0 : internalYValue);
				num6 = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num);
				if (plotGroup.AxisY.AxisMinimum != null && ((internalYValue < num && num > 0.0) || (plotGroup.AxisY.InternalAxisMinimum == 0.0 && internalYValue < 0.0 && chart.View3D)))
				{
					num5 = (num6 = num3 - 100.0);
				}
			}
			double num7 = Math.Abs(num5 - num6);
			if (num7 < dataPoint.Parent.MinPointHeight)
			{
				if (dataPoint.DpInfo.InternalYValue == 0.0)
				{
					if (plotGroup.AxisY.InternalAxisMaximum <= 0.0)
					{
						num5 -= dataPoint.Parent.MinPointHeight - num7;
					}
					else
					{
						num6 += dataPoint.Parent.MinPointHeight - num7;
					}
				}
				else if (isPositive)
				{
					num6 += dataPoint.Parent.MinPointHeight - num7;
				}
				else
				{
					num5 -= dataPoint.Parent.MinPointHeight - num7;
				}
				num7 = dataPoint.Parent.MinPointHeight;
			}
			double num8 = BarChart.CalculateHeightOfEachColumn(ref num4, heightPerBar, height);
			if (num8 < 0.0)
			{
				return;
			}
			Panel panel;
			if (chart.View3D)
			{
				Faces faces = ColumnChart.Get3DColumn(dataPoint, num7, num8, depth3d, dataPoint.Color, null, null, null, (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled), (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle), (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor), ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left);
				panel = (faces.Visual as Panel);
				panel.SetValue(Panel.ZIndexProperty, BarChart.GetBarZIndex(num5, num4, height, internalYValue > 0.0));
				dataPoint.DpInfo.Faces = faces;
				if (!dataPoint.Parent.LightWeight.Value && !VisifireControl.IsMediaEffectsEnabled)
				{
					ColumnChart.ApplyOrRemoveShadow4XBAP(dataPoint, false, false);
				}
			}
			else
			{
				Faces faces = ColumnChart.Get2DColumn(dataPoint, num7, num8, false, false, isPositive);
				panel = (faces.Visual as Panel);
				dataPoint.DpInfo.Faces = faces;
			}
			if (!dataPoint.Parent.LightWeight.Value && VisifireControl.IsMediaEffectsEnabled)
			{
				BarChart.ApplyEffects(chart, dataPoint);
				BarChart.ApplyOrRemoveShadow(chart, dataPoint);
			}
			dataPoint.DpInfo.Faces.LabelCanvas = labelCanvas;
			dataPoint.Parent.Faces = new Faces
			{
				Visual = columnCanvas,
				LabelCanvas = labelCanvas
			};
			panel.SetValue(Canvas.LeftProperty, num5);
			panel.SetValue(Canvas.TopProperty, num4);
			columnCanvas.Children.Add(panel);
			dataPoint.DpInfo.IsTopOfStack = true;
			if (!dataPoint.Parent.LightWeight.Value)
			{
				BarChart.CreateOrUpdateMarker4HorizontalChart(width, chart, labelCanvas, dataPoint, num5, num4, isPositive, depth3d, internalYValue);
			}
			if (isPositive)
			{
				dataPoint.DpInfo.VisualPosition = new Point(num6, num4 + num8 / 2.0);
			}
			else
			{
				dataPoint.DpInfo.VisualPosition = new Point(num5, num4 + num8 / 2.0);
			}
			dataPoint.DpInfo.Faces.LabelCanvas = labelCanvas;
			if (animationEnabled)
			{
				if (dataPoint.Parent.Storyboard == null)
				{
					dataPoint.Parent.Storyboard = new Storyboard();
				}
				BarChart.currentDataSeries = dataPoint.Parent;
				double beginTime = 0.5;
				dataPoint.Parent.Storyboard = BarChart.ApplyBarChartAnimation(panel, dataPoint.Parent.Storyboard, isPositive, beginTime);
			}
			dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
			if (!dataPoint.IsLightDataPoint)
			{
				(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
			}
			if (!dataPoint.IsLightDataPoint)
			{
				ColumnChart.AttachEvents2Visual(dataPoint as DataPoint, dataPoint);
			}
			ColumnChart.AttachEvents2Visual(dataPoint.Parent, dataPoint);
			dataPoint.DpInfo.Faces.Visual.Opacity = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity) * dataPoint.Parent.Opacity;
			if (!dataPoint.Parent.LightWeight.Value && !chart.IndicatorEnabled)
			{
				dataPoint.AttachToolTip(chart, dataPoint, dataPoint.DpInfo.Faces.Visual);
			}
			dataPoint.DpFunc.AttachHref(chart, dataPoint.DpInfo.Faces.Visual);
			DataPointHelper.SetCursor2DataPointVisualFaces(dataPoint);
			if (chart.ChartArea != null && !chart.ChartArea._isFirstTimeRender)
			{
				chart._toolTip.Hide();
			}
		}

		internal static void ApplyEffects(Chart chart, IDataPoint dataPoint)
		{
			Faces faces = dataPoint.DpInfo.Faces;
			if (faces == null)
			{
				throw new Exception("Faces of DataPoint is null. ApplyEffects()");
			}
			Canvas canvas = faces.Visual as Canvas;
			if (DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect) != null)
			{
				canvas.Effect = (Effect)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect);
				return;
			}
			canvas.Effect = null;
		}

		internal static void ApplyOrRemoveShadow(Chart chart, IDataPoint dataPoint)
		{
			Faces faces = dataPoint.DpInfo.Faces;
			if (faces == null)
			{
				throw new Exception("Faces of DataPoint is null. ApplyOrRemoveShadow()");
			}
			Canvas canvas = faces.Visual as Canvas;
			if (DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect) == null)
			{
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled))
				{
					canvas.Effect = ExtendedGraphics.GetShadowEffect(325.0, 3.3, 0.95);
					return;
				}
				canvas.Effect = null;
			}
		}

		internal static void DrawStackedBarsAtXValue(RenderAs chartType, double xValue, PlotGroup plotGroup, Canvas columnCanvas, Canvas labelCanvas, double drawingIndex, double heightPerBar, double maxBarHeight, double limitingYValue, double depth3d, bool animationEnabled)
		{
			RectangularChartShapeParams rectangularChartShapeParams = new RectangularChartShapeParams();
			rectangularChartShapeParams.ShadowOffset = 5.0;
			rectangularChartShapeParams.Depth = depth3d;
			rectangularChartShapeParams.IsStacked = true;
			IDataPoint dataPoint = null;
			int num = 1;
			int num2 = 1;
			double top = Graphics.ValueToPixelPosition(columnCanvas.Height, 0.0, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, xValue) + drawingIndex * heightPerBar - maxBarHeight / 2.0;
			double num3 = Graphics.ValueToPixelPosition(0.0, columnCanvas.Width, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, limitingYValue);
			double finalHeight = BarChart.CalculateHeightOfEachColumn(ref top, heightPerBar, columnCanvas.Height);
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.4;
			double num7 = 1.0 / (double)plotGroup.XWiseStackedDataList[xValue].Positive.Count;
			if (plotGroup.XWiseStackedDataList[xValue].Positive.Count > 0)
			{
				dataPoint = plotGroup.XWiseStackedDataList[xValue].Positive.Last<IDataPoint>();
			}
			double absoluteSum = double.NaN;
			if (chartType == RenderAs.StackedBar100)
			{
				absoluteSum = plotGroup.XWiseStackedDataList[xValue].AbsoluteYValueSum;
			}
			foreach (IDataPoint current in plotGroup.XWiseStackedDataList[xValue].Positive)
			{
				current.Parent.Faces = new Faces
				{
					Visual = columnCanvas,
					LabelCanvas = labelCanvas
				};
				double internalYValue = current.DpInfo.InternalYValue;
				if (!current.Enabled.Value || double.IsNaN(internalYValue))
				{
					ColumnChart.CleanUpMarkerAndLabel(current, labelCanvas);
				}
				else
				{
					bool flag = current == dataPoint;
					int num8 = plotGroup.XWiseStackedDataList[xValue].Positive.IndexOf(current);
					bool flag2 = true;
					for (int i = num8 + 1; i < plotGroup.XWiseStackedDataList[xValue].Positive.Count; i++)
					{
						if (plotGroup.XWiseStackedDataList[xValue].Positive[i].DpInfo.InternalYValue != 0.0)
						{
							flag2 = false;
							break;
						}
					}
					if (flag2 && !flag)
					{
						flag = true;
					}
					BarChart.CreateStackedBarVisual(current.Parent.RenderAs, internalYValue >= 0.0, columnCanvas, labelCanvas, current, top, ref num3, ref num4, finalHeight, ref num5, absoluteSum, depth3d, animationEnabled, num6, flag, num, plotGroup.XWiseStackedDataList[xValue].Positive.ToList<IDataPoint>());
					num6 += num7;
					num++;
				}
			}
			num5 = 0.0;
			num4 = Graphics.ValueToPixelPosition(0.0, columnCanvas.Width, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, limitingYValue);
			dataPoint = null;
			if (plotGroup.XWiseStackedDataList[xValue].Negative.Count > 0)
			{
				dataPoint = plotGroup.XWiseStackedDataList[xValue].Negative.Last<IDataPoint>();
				num7 = 1.0 / (double)plotGroup.XWiseStackedDataList[xValue].Negative.Count;
				num6 = 0.4;
			}
			foreach (IDataPoint current2 in plotGroup.XWiseStackedDataList[xValue].Negative)
			{
				current2.Parent.Faces = new Faces
				{
					Visual = columnCanvas,
					LabelCanvas = labelCanvas
				};
				double internalYValue2 = current2.DpInfo.InternalYValue;
				if (current2.Enabled.Value && !double.IsNaN(internalYValue2))
				{
					bool flag = current2 == dataPoint;
					CornerRadius cr = (CornerRadius)DataPointHelper.GetDataPointValueFromProperty(current2, VcProperties.RadiusX);
					CornerRadius cr2 = (CornerRadius)DataPointHelper.GetDataPointValueFromProperty(current2, VcProperties.RadiusY);
					if (cr != new CornerRadius(0.0) && cr != new CornerRadius(0.0, 0.0, 0.0, 0.0) && (cr2 != new CornerRadius(0.0) || cr2 != new CornerRadius(0.0, 0.0, 0.0, 0.0)))
					{
						int num9 = plotGroup.XWiseStackedDataList[xValue].Negative.IndexOf(current2);
						bool flag3 = true;
						for (int j = num9 + 1; j < plotGroup.XWiseStackedDataList[xValue].Negative.Count; j++)
						{
							if (plotGroup.XWiseStackedDataList[xValue].Negative[j].DpInfo.InternalYValue != 0.0)
							{
								flag3 = false;
								break;
							}
						}
						if (flag3 && !flag)
						{
							flag = true;
						}
					}
					BarChart.CreateStackedBarVisual(current2.Parent.RenderAs, internalYValue2 >= 0.0, columnCanvas, labelCanvas, current2, top, ref num3, ref num4, finalHeight, ref num5, absoluteSum, depth3d, animationEnabled, num6, flag, num2, plotGroup.XWiseStackedDataList[xValue].Negative.ToList<IDataPoint>());
					num6 += num7;
					num2--;
				}
			}
		}

		internal static Canvas GetVisualObjectForStackedBarChart(RenderAs chartType, Panel preExistingPanel, double width, double height, PlotDetails plotDetails, Chart chart, double plankDepth, bool animationEnabled)
		{
			if (double.IsNaN(width) || double.IsNaN(height) || width <= 0.0 || height <= 0.0)
			{
				return null;
			}
			bool flag = true;
			Canvas canvas;
			Canvas canvas2;
			Canvas canvas3;
			RenderHelper.PrepareCanvas4Drawing(preExistingPanel as Canvas, out canvas, out canvas2, out canvas3, width, height);
			List<PlotGroup> list = (from plots in plotDetails.PlotGroups
			where plots.RenderAs == chartType
			select plots).ToList<PlotGroup>();
			if (list.Count == 0)
			{
				return null;
			}
			if (list[0].DataSeriesList.Count == 0)
			{
				return null;
			}
			int arg_96_0 = plotDetails.DrawingDivisionFactor;
			double num = plankDepth / (double)plotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
			double num2 = num * (double)(plotDetails.SeriesDrawingIndex[list[0].DataSeriesList[0]] + 1);
			canvas.SetValue(Canvas.TopProperty, num2);
			canvas.SetValue(Canvas.LeftProperty, -num2);
			List<DataSeries> seriesListByRenderAs = plotDetails.GetSeriesListByRenderAs(chartType);
			Dictionary<Axis, Dictionary<Axis, int>> seriesIndex = ColumnChart.GetSeriesIndex(seriesListByRenderAs);
			int num3 = 1;
			DataSeries dataSeries = null;
			foreach (PlotGroup current in list)
			{
				if (seriesIndex.ContainsKey(current.AxisY))
				{
					dataSeries = current.DataSeriesList[0];
					int num4 = seriesIndex[current.AxisY][current.AxisX];
					double num5;
					double maxBarHeight;
					double heightPerBar = ColumnChart.CalculateWidthOfEachStackedColumn(chart, current, height, out num5, out maxBarHeight);
					double[] xValuesUnderViewPort = RenderHelper.GetXValuesUnderViewPort(current.XWiseStackedDataList.Keys.ToList<double>(), current.AxisX, current.AxisY, false);
					double limitingYValue = 0.0;
					if (current.AxisY.InternalAxisMinimum > 0.0)
					{
						limitingYValue = current.AxisY.InternalAxisMinimum;
					}
					if (current.AxisY.InternalAxisMaximum < 0.0)
					{
						limitingYValue = 0.0;
					}
					num3++;
					double[] array = xValuesUnderViewPort;
					for (int i = 0; i < array.Length; i++)
					{
						double xValue = array[i];
						BarChart.DrawStackedBarsAtXValue(chartType, xValue, current, canvas3, canvas2, (double)num4, heightPerBar, maxBarHeight, limitingYValue, num, animationEnabled);
					}
				}
			}
			if (list.Count > 0 && list[0].XWiseStackedDataList.Keys.Count > 0)
			{
				ColumnChart.CreateOrUpdatePlank(chart, list[0].AxisY, canvas3, num, Orientation.Vertical);
			}
			if (animationEnabled && dataSeries != null)
			{
				if (dataSeries.Storyboard == null)
				{
					dataSeries.Storyboard = new Storyboard();
				}
				double beginTime = 1.0;
				dataSeries.Storyboard = AnimationHelper.ApplyOpacityAnimation(canvas2, dataSeries, dataSeries.Storyboard, beginTime, 1.0, 0.0, 1.0);
			}
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
			List<IDataPoint> list2 = (from datapoint in plotDetails.ListOfAllDataPoints
			where datapoint.Enabled == true
			select datapoint).ToList<IDataPoint>();
			for (int j = 0; j < list2.Count; j++)
			{
				if (list2[j].Parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					double limitingYValue2 = list2[j].Parent.PlotGroup.GetLimitingYValue();
					if (list2[j].DpInfo.InternalYValue < limitingYValue2)
					{
						flag = false;
						break;
					}
				}
			}
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			if (flag)
			{
				rectangleGeometry.Rect = new Rect((double)(-(double)(chart.View3D ? 0 : 5)) - chart.ChartArea.PLANK_THICKNESS, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + (double)(chart.View3D ? 0 : 10), height + chart.ChartArea.PLANK_DEPTH);
			}
			else
			{
				rectangleGeometry.Rect = new Rect((double)(-(double)(chart.View3D ? 0 : 5)), -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH + (double)(chart.View3D ? 0 : 10), height + chart.ChartArea.PLANK_DEPTH);
			}
			canvas.Clip = rectangleGeometry;
			PlotArea plotArea = chart.PlotArea;
			RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
			if (flag)
			{
				rectangleGeometry2.Rect = new Rect(plotArea.BorderThickness.Left - chart.ChartArea.PLANK_THICKNESS, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS - plotArea.BorderThickness.Left - plotArea.BorderThickness.Right, height + chart.ChartArea.PLANK_DEPTH);
			}
			else
			{
				rectangleGeometry2.Rect = new Rect(plotArea.BorderThickness.Left, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH - plotArea.BorderThickness.Left - plotArea.BorderThickness.Right, height + chart.ChartArea.PLANK_DEPTH);
			}
			canvas3.Clip = rectangleGeometry2;
			return canvas;
		}

		private static void CreateStackedBarVisual(RenderAs chartType, bool isPositive, Canvas columnCanvas, Canvas labelCanvas, IDataPoint dataPoint, double top, ref double left, ref double right, double finalHeight, ref double prevSum, double absoluteSum, double depth3d, bool animationEnabled, double animationBeginTime, bool isTopOFStack, int PositiveOrNegativeZIndex, List<IDataPoint> listOfDataPointsInXValue)
		{
			PlotGroup plotGroup = dataPoint.Parent.PlotGroup;
			Chart chart = dataPoint.Chart as Chart;
			dataPoint.DpInfo.OldLightingState = false;
			double num = 0.0;
			double internalYValue = dataPoint.DpInfo.InternalYValue;
			if (chartType == RenderAs.StackedBar100)
			{
				if (absoluteSum != 0.0)
				{
					if (plotGroup.AxisY.Logarithmic)
					{
						num = Math.Log(internalYValue / absoluteSum * 100.0, plotGroup.AxisY.LogarithmBase);
					}
					else
					{
						num = internalYValue / absoluteSum * 100.0;
					}
				}
			}
			else
			{
				num = internalYValue;
			}
			double num2 = 0.0;
			if (plotGroup.AxisY.InternalAxisMinimum > 0.0)
			{
				num2 = plotGroup.AxisY.InternalAxisMinimum;
			}
			if (plotGroup.AxisY.InternalAxisMaximum < 0.0)
			{
				num2 = 0.0;
			}
			double num3 = Graphics.ValueToPixelPosition(0.0, columnCanvas.Width, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, 0.0);
			List<IDataPoint> list = listOfDataPointsInXValue.ToList<IDataPoint>();
			double num4 = 0.0;
			foreach (IDataPoint current in list)
			{
				num4 += current.DpInfo.InternalYValue;
			}
			if (isPositive)
			{
				if (plotGroup.AxisY.Logarithmic)
				{
					right = ColumnChart.CalculatePositionOfDataPointForLogAxis(dataPoint, columnCanvas.Width, plotGroup, listOfDataPointsInXValue, absoluteSum);
				}
				else
				{
					right = Graphics.ValueToPixelPosition(0.0, columnCanvas.Width, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num + prevSum);
				}
				if (chartType == RenderAs.Bar)
				{
					if (plotGroup.AxisY.AxisMinimum != null && dataPoint.DpInfo.InternalYValue < num2)
					{
						left = (right = num3 - 100.0);
					}
					else if (chart.View3D && dataPoint.DpInfo.InternalYValue > plotGroup.AxisY.InternalAxisMaximum)
					{
						right = columnCanvas.Width;
					}
				}
				else if (plotGroup.AxisY.AxisMinimum != null && chartType == RenderAs.StackedBar100 && dataPoint.DpInfo.InternalYValue == 0.0)
				{
					left = (right = num3);
				}
				if (chartType == RenderAs.StackedBar)
				{
					if (plotGroup.AxisY.AxisMinimum != null && dataPoint.DpInfo.InternalYValue + prevSum < num2 && num2 >= num4)
					{
						left = (right = num3 - 100.0);
					}
					else if (chart.View3D && dataPoint.DpInfo.InternalYValue + prevSum > plotGroup.AxisY.InternalAxisMaximum)
					{
						right = columnCanvas.Width;
					}
				}
				else if (plotGroup.AxisY.AxisMinimum != null && chartType == RenderAs.StackedBar100 && dataPoint.DpInfo.InternalYValue == 0.0 && num2 >= num4)
				{
					left = (right = num3 - 100.0);
				}
				else if (plotGroup.AxisY.AxisMinimum != null && plotGroup.AxisY.AxisMaximum == null && chartType == RenderAs.StackedBar100 && num2 >= 100.0)
				{
					left = (right = num3 - 100.0);
				}
				if (chartType == RenderAs.StackedBar100 && num + prevSum > plotGroup.AxisY.InternalAxisMaximum)
				{
					right = columnCanvas.Width;
				}
			}
			else
			{
				left = Graphics.ValueToPixelPosition(0.0, columnCanvas.Width, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num + prevSum);
				if (chartType == RenderAs.StackedBar || chartType == RenderAs.Bar)
				{
					if (plotGroup.AxisY.AxisMinimum != null && num2 > 0.0 && internalYValue < num2 && chart.View3D)
					{
						right = (left = num3 - 100.0);
					}
					else if (plotGroup.AxisY.AxisMinimum != null && plotGroup.AxisY.InternalAxisMinimum == 0.0 && internalYValue < 0.0 && chart.View3D)
					{
						right = (left = num3 - 100.0);
					}
				}
				else if (chart.View3D && chartType == RenderAs.StackedBar100 && plotGroup.AxisY.InternalAxisMinimum < 0.0 && plotGroup.AxisY.InternalAxisMinimum > num + prevSum)
				{
					left = Graphics.ValueToPixelPosition(0.0, columnCanvas.Width, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, plotGroup.AxisY.InternalAxisMinimum);
				}
			}
			if (right < 0.0)
			{
				right = left;
			}
			double num5 = Math.Abs(right - left);
			prevSum += num;
			if (num5 == 0.0 && prevSum > plotGroup.AxisY.InternalAxisMaximum)
			{
				return;
			}
			Panel panel;
			if ((dataPoint.Chart as Chart).View3D)
			{
				Brush rightBrush = null;
				if ((dataPoint.Parent.RenderAs == RenderAs.StackedBar && ((!isTopOFStack && prevSum == plotGroup.AxisY.InternalAxisMaximum) || prevSum > plotGroup.AxisY.InternalAxisMaximum)) || (dataPoint.Parent.RenderAs == RenderAs.StackedBar100 && plotGroup.AxisY.AxisMaximum != null && ((!isTopOFStack && prevSum == plotGroup.AxisY.InternalAxisMaximum) || prevSum > plotGroup.AxisY.InternalAxisMaximum)))
				{
					rightBrush = (((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) ? Graphics.GetRightFaceBrush(dataPoint.Color) : Graphics.GetDarkerBrush(dataPoint.Color, 0.55));
				}
				Faces faces = ColumnChart.Get3DColumn(dataPoint, num5, finalHeight, depth3d, dataPoint.Color, null, null, rightBrush, (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled), (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle), (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor), ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left);
				panel = (faces.Visual as Panel);
				panel.SetValue(Panel.ZIndexProperty, BarChart.GetStackedBarZIndex(chart.ChartArea.PlotAreaCanvas.Height, left, top, columnCanvas.Width, columnCanvas.Height, internalYValue > 0.0, PositiveOrNegativeZIndex));
				dataPoint.DpInfo.Faces = faces;
				if (!dataPoint.Parent.LightWeight.Value && !VisifireControl.IsMediaEffectsEnabled)
				{
					ColumnChart.ApplyOrRemoveShadow4XBAP(dataPoint, true, false);
				}
			}
			else
			{
				Faces faces = ColumnChart.Get2DColumn(dataPoint, num5, finalHeight, true, isTopOFStack, isPositive);
				panel = (faces.Visual as Panel);
				dataPoint.DpInfo.Faces = faces;
			}
			if (!dataPoint.Parent.LightWeight.Value && VisifireControl.IsMediaEffectsEnabled)
			{
				BarChart.ApplyEffects(chart, dataPoint);
				BarChart.ApplyOrRemoveShadow(chart, dataPoint);
			}
			dataPoint.DpInfo.Faces.LabelCanvas = labelCanvas;
			panel.SetValue(Canvas.LeftProperty, left);
			panel.SetValue(Canvas.TopProperty, top);
			columnCanvas.Children.Add(panel);
			dataPoint.DpInfo.IsTopOfStack = isTopOFStack;
			if (!dataPoint.Parent.LightWeight.Value)
			{
				BarChart.CreateOrUpdateMarker4HorizontalChart(columnCanvas.Width, dataPoint.Chart as Chart, labelCanvas, dataPoint, left, top, isPositive, depth3d, prevSum);
			}
			if (animationEnabled)
			{
				if (dataPoint.Parent.Storyboard == null)
				{
					dataPoint.Parent.Storyboard = new Storyboard();
				}
				BarChart.currentDataSeries = dataPoint.Parent;
				dataPoint.Parent.Storyboard = BarChart.ApplyStackedBarChartAnimation(panel, dataPoint.Parent.Storyboard, animationBeginTime, 0.5);
			}
			if (isPositive)
			{
				left = right;
			}
			else
			{
				right = left;
			}
			if (isPositive)
			{
				dataPoint.DpInfo.VisualPosition = new Point(right, top + finalHeight / 2.0);
			}
			else
			{
				dataPoint.DpInfo.VisualPosition = new Point(left, top + finalHeight / 2.0);
			}
			dataPoint.DpInfo.Faces.Visual.Opacity = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity) * dataPoint.Parent.Opacity;
			if (!dataPoint.IsLightDataPoint)
			{
				ColumnChart.AttachEvents2Visual(dataPoint as DataPoint, dataPoint);
			}
			ColumnChart.AttachEvents2Visual(dataPoint.Parent, dataPoint);
			dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
			if (!dataPoint.IsLightDataPoint)
			{
				(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
			}
			if (!chart.IndicatorEnabled)
			{
				dataPoint.AttachToolTip(chart, dataPoint, dataPoint.DpInfo.Faces.Visual);
			}
			dataPoint.DpFunc.AttachHref(chart, dataPoint.DpInfo.Faces.Visual);
			DataPointHelper.SetCursor2DataPointVisualFaces(dataPoint);
		}
	}
}
