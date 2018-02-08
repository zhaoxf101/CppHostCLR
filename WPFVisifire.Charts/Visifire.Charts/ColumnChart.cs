using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class ColumnChart
	{
		internal static double COLUMN_GAP_RATIO = 0.1;

		private static void CalculateAutoPlacement(bool isView3D, IDataPoint dataPoint, Size columnVisualSize, bool isPositive, LabelStyles labelStyle, ref double labelLeft, ref double labelTop, ref double angle, double canvasLeft, double canvasTop, bool isVertical, double insideGap, double outsideGap, Title tb, bool isTopOfStack)
		{
			double num = 0.0;
			double num2 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
			double num3 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
			if (isPositive)
			{
				if (labelStyle == LabelStyles.Inside)
				{
					if (isVertical)
					{
						if (columnVisualSize.Height - insideGap - num2 / 2.0 * num3 < tb.TextBlockDesiredSize.Width)
						{
							labelLeft = canvasLeft + columnVisualSize.Width / 2.0;
							labelTop = canvasTop - tb.TextBlockDesiredSize.Height + columnVisualSize.Height + insideGap;
							angle = -90.0;
							return;
						}
						Point point = new Point(canvasLeft + columnVisualSize.Width / 2.0, canvasTop + (num2 / 2.0 * num3 + insideGap));
						angle = 90.0;
						double num4 = 0.017453292519943295 * angle;
						num += tb.Width;
						angle = (num4 - 3.1415926535897931) * 57.295779513082323;
						labelLeft = point.X + num * Math.Cos(num4);
						labelTop = point.Y + num * Math.Sin(num4);
						return;
					}
					else
					{
						if (columnVisualSize.Height - insideGap - num2 / 2.0 * num3 >= tb.TextBlockDesiredSize.Height)
						{
							labelLeft = canvasLeft + columnVisualSize.Width / 2.0 - tb.TextBlockDesiredSize.Width / 2.0;
							labelTop = canvasTop + num2 / 2.0 * num3 + insideGap;
							return;
						}
						labelLeft = canvasLeft + columnVisualSize.Width / 2.0 - tb.TextBlockDesiredSize.Width / 2.0;
						labelTop = canvasTop - tb.TextBlockDesiredSize.Height + columnVisualSize.Height + insideGap;
						if (labelTop < 0.0)
						{
							labelTop = 0.0;
							return;
						}
					}
				}
				else
				{
					if (isVertical)
					{
						labelTop = canvasTop - tb.TextBlockDesiredSize.Height - (num2 / 2.0 * num3 - (isView3D ? outsideGap : (outsideGap + 3.0)));
						labelLeft = canvasLeft + columnVisualSize.Width / 2.0;
						angle = -90.0;
						return;
					}
					if (dataPoint.Parent.RenderAs != RenderAs.StackedColumn100 || !isView3D || !isTopOfStack)
					{
						labelLeft = canvasLeft + columnVisualSize.Width / 2.0 - tb.TextBlockDesiredSize.Width / 2.0;
						labelTop = canvasTop - tb.TextBlockDesiredSize.Height - (num2 / 2.0 * num3 - (isView3D ? (-outsideGap) : outsideGap));
						return;
					}
					if (!dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet && !isVertical && tb.TextBlockDesiredSize.Height >= columnVisualSize.Height)
					{
						labelLeft = canvasLeft + columnVisualSize.Width / 2.0 - tb.TextBlockDesiredSize.Width / 2.0;
						labelTop = canvasTop - outsideGap;
						return;
					}
					labelLeft = canvasLeft + columnVisualSize.Width / 2.0 - tb.TextBlockDesiredSize.Width / 2.0;
					labelTop = canvasTop - tb.TextBlockDesiredSize.Height - (num2 / 2.0 * num3 - (isView3D ? (-outsideGap) : outsideGap));
					return;
				}
			}
			else if (labelStyle == LabelStyles.Inside)
			{
				if (isVertical)
				{
					if (columnVisualSize.Height - insideGap - num2 / 2.0 * num3 < tb.TextBlockDesiredSize.Width)
					{
						Point point = new Point(canvasLeft + columnVisualSize.Width / 2.0, canvasTop - columnVisualSize.Height + insideGap);
						angle = -270.0;
						double num4 = 0.017453292519943295 * angle;
						num += tb.Width;
						angle = (num4 - 3.1415926535897931) * 57.295779513082323;
						labelLeft = point.X + num * Math.Cos(num4);
						labelTop = point.Y + num * Math.Sin(num4);
						return;
					}
					labelLeft = canvasLeft + columnVisualSize.Width / 2.0;
					labelTop = canvasTop - (num2 / 2.0 * num3 + insideGap);
					angle = -90.0;
					return;
				}
				else
				{
					if (columnVisualSize.Height + insideGap - num2 / 2.0 * num3 < tb.TextBlockDesiredSize.Height)
					{
						labelLeft = canvasLeft + columnVisualSize.Width / 2.0 - tb.TextBlockDesiredSize.Width / 2.0;
						labelTop = canvasTop - columnVisualSize.Height + insideGap;
						if (labelTop + tb.TextBlockDesiredSize.Height > (dataPoint.Chart as Chart).ChartArea.ChartVisualCanvas.Height - (dataPoint.Chart as Chart).ChartArea.PLANK_THICKNESS)
						{
							labelTop = (dataPoint.Chart as Chart).ChartArea.ChartVisualCanvas.Height - (dataPoint.Chart as Chart).ChartArea.PLANK_THICKNESS - tb.TextBlockDesiredSize.Height;
						}
						angle = 0.0;
						return;
					}
					labelLeft = canvasLeft + columnVisualSize.Width / 2.0 - tb.TextBlockDesiredSize.Width / 2.0;
					labelTop = canvasTop - tb.TextBlockDesiredSize.Height - (num2 / 2.0 * num3 - insideGap);
					angle = 0.0;
					return;
				}
			}
			else
			{
				if (isVertical)
				{
					Point point = new Point(canvasLeft + columnVisualSize.Width / 2.0, canvasTop + (num2 / 2.0 * num3 + outsideGap + 2.0));
					angle = 90.0;
					double num4 = 0.017453292519943295 * angle;
					num += tb.Width;
					angle = (num4 - 3.1415926535897931) * 57.295779513082323;
					labelLeft = point.X + num * Math.Cos(num4);
					labelTop = point.Y + num * Math.Sin(num4);
					return;
				}
				if (dataPoint.Parent.RenderAs == RenderAs.StackedColumn100 && isView3D && isTopOfStack)
				{
					if (!dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet && !isVertical && tb.TextBlockDesiredSize.Height >= columnVisualSize.Height)
					{
						labelLeft = canvasLeft + columnVisualSize.Width / 2.0 - tb.TextBlockDesiredSize.Width / 2.0;
						labelTop = canvasTop - outsideGap;
						return;
					}
					labelLeft = canvasLeft + columnVisualSize.Width / 2.0 - tb.TextBlockDesiredSize.Width / 2.0;
					labelTop = canvasTop + (num2 / 2.0 * num3 + outsideGap + 3.0);
					return;
				}
				else
				{
					labelLeft = canvasLeft + columnVisualSize.Width / 2.0 - tb.TextBlockDesiredSize.Width / 2.0;
					labelTop = canvasTop + (num2 / 2.0 * num3 + outsideGap + 3.0);
				}
			}
		}

		private static void CreateLabel(Chart chart, Size columnVisualSize, bool isPositive, bool isTopOfStack, IDataPoint dataPoint, double canvasLeft, double canvasTop, ref Canvas labelCanvas, double prevSum)
		{
			if (dataPoint.DpInfo.Faces == null)
			{
				return;
			}
			LabelStyles labelStyles = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && !string.IsNullOrEmpty((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText)))
			{
				LabelStyles labelStyles2 = labelStyles;
				if (isPositive || dataPoint.YValue == 0.0)
				{
					isPositive = true;
				}
				if (isPositive)
				{
					canvasTop -= 6.0;
				}
				else
				{
					canvasTop -= 8.0;
				}
				double num = 0.0;
				double num2 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelAngle);
				Brush labelFontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
				FontStyle internalFontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
				Brush internalBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
				FontFamily internalFontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
				double internalFontSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontSize);
				FontWeight internalFontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
				Title title = new Title
				{
					Text = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText)),
					InternalFontFamily = internalFontFamily,
					InternalFontSize = internalFontSize,
					InternalFontWeight = internalFontWeight,
					InternalFontStyle = internalFontStyle,
					InternalBackground = internalBackground,
					InternalFontColor = Chart.CalculateDataPointLabelFontColor(dataPoint.Chart as Chart, dataPoint, labelFontColor, labelStyles2),
					Padding = new Thickness(0.1, 0.1, 0.1, 0.1),
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
				double num5 = 4.0;
				double insideGap = 6.0;
				if (double.IsNaN(num2) || num2 == 0.0)
				{
					bool flag;
					if (columnVisualSize.Width < title.TextBlockDesiredSize.Width)
					{
						title.Visual.RenderTransformOrigin = new Point(0.0, 0.5);
						title.Visual.RenderTransform = new RotateTransform
						{
							CenterX = 0.0,
							CenterY = 0.0,
							Angle = -90.0
						};
						flag = true;
					}
					else
					{
						flag = false;
					}
					if (!dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet && !isTopOfStack && dataPoint.Parent.RenderAs != RenderAs.Column)
					{
						labelStyles2 = LabelStyles.Inside;
					}
					if (dataPoint.Parent.RenderAs == RenderAs.StackedColumn100 && chart.View3D && isTopOfStack && !dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet && !flag && title.TextBlockDesiredSize.Height >= columnVisualSize.Height)
					{
						labelStyles2 = LabelStyles.OutSide;
					}
					ColumnChart.CalculateAutoPlacement(chart.View3D, dataPoint, columnVisualSize, isPositive, labelStyles2, ref num3, ref num4, ref num, canvasLeft, canvasTop, flag, insideGap, num5, title, isTopOfStack);
					if (dataPoint.Parent.PlotGroup.AxisY.AxisMaximum != null && dataPoint.Parent.PlotGroup.AxisY.InternalAxisMaximum > 0.0 && !chart.View3D && prevSum > dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum && num4 < 0.0)
					{
						num4 = 0.0;
					}
					title.Visual.SetValue(Canvas.LeftProperty, num3);
					title.Visual.SetValue(Canvas.TopProperty, num4);
					title.Visual.RenderTransformOrigin = new Point(0.0, 0.5);
					title.Visual.RenderTransform = new RotateTransform
					{
						CenterX = 0.0,
						CenterY = 0.0,
						Angle = num
					};
					double num6 = chart.ChartArea.PLANK_DEPTH / (double)chart.PlotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
					if (!dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
					{
						if (isPositive)
						{
							if (flag)
							{
								if (num4 + num5 - title.TextBlockDesiredSize.Width < -num6)
								{
									labelStyles2 = LabelStyles.Inside;
								}
							}
							else if (num4 < -num6)
							{
								labelStyles2 = LabelStyles.Inside;
							}
							else if (num4 == 0.0)
							{
								labelStyles2 = LabelStyles.Inside;
							}
						}
						else if (flag)
						{
							if (num4 + num5 + 2.0 > chart.PlotArea.BorderElement.Height - num6 + chart.ChartArea.PLANK_THICKNESS)
							{
								labelStyles2 = LabelStyles.Inside;
							}
						}
						else if (num4 + title.TextBlockDesiredSize.Height > chart.PlotArea.BorderElement.Height - num6 + chart.ChartArea.PLANK_THICKNESS)
						{
							labelStyles2 = LabelStyles.Inside;
						}
					}
					if (labelStyles2 != labelStyles)
					{
						ColumnChart.CalculateAutoPlacement(chart.View3D, dataPoint, columnVisualSize, isPositive, labelStyles2, ref num3, ref num4, ref num, canvasLeft, canvasTop, flag, insideGap, num5, title, isTopOfStack);
						if (dataPoint.Parent.PlotGroup.AxisY.AxisMaximum != null && dataPoint.Parent.PlotGroup.AxisY.InternalAxisMaximum > 0.0 && !chart.View3D && prevSum > dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum && num4 < 0.0)
						{
							num4 = 0.0;
						}
						title.Visual.SetValue(Canvas.LeftProperty, num3);
						title.Visual.SetValue(Canvas.TopProperty, num4);
						title.Visual.RenderTransformOrigin = new Point(0.0, 0.5);
						title.Visual.RenderTransform = new RotateTransform
						{
							CenterX = 0.0,
							CenterY = 0.0,
							Angle = num
						};
					}
					if (chart.SmartLabelEnabled && (dataPoint.Parent.RenderAs == RenderAs.StackedColumn || dataPoint.Parent.RenderAs == RenderAs.StackedColumn100) && flag && labelStyles2 == LabelStyles.Inside && columnVisualSize.Height < title.Visual.Width)
					{
						return;
					}
				}
				else
				{
					double num7 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
					double num8 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
					if (isPositive)
					{
						Point centerOfRotation = new Point(canvasLeft + columnVisualSize.Width / 2.0, canvasTop - num7 / 2.0 * num8);
						double num9 = 0.0;
						num = 0.0;
						if (labelStyles2 == LabelStyles.OutSide)
						{
							if (num2 > 0.0 && num2 <= 90.0)
							{
								num = num2 - 180.0;
								double num10 = 0.017453292519943295 * num;
								num9 += title.TextBlockDesiredSize.Width;
								num = (num10 - 3.1415926535897931) * 57.295779513082323;
								ColumnChart.SetRotation(num9, num, num10, centerOfRotation, num3, num4, title);
							}
							else if (num2 >= -90.0 && num2 < 0.0)
							{
								num = num2;
								double num10 = 0.017453292519943295 * num;
								ColumnChart.SetRotation(num9, num, num10, centerOfRotation, num3, num4, title);
							}
						}
						else
						{
							centerOfRotation = new Point(canvasLeft + columnVisualSize.Width / 2.0, canvasTop + num7 / 2.0 * num8);
							num9 = 4.0;
							if (num2 >= -90.0 && num2 < 0.0)
							{
								num = 180.0 + num2;
								double num10 = 0.017453292519943295 * num;
								num9 += title.TextBlockDesiredSize.Width + 5.0;
								num = (num10 - 3.1415926535897931) * 57.295779513082323;
								ColumnChart.SetRotation(num9, num, num10, centerOfRotation, num3, num4, title);
							}
							else if (num2 > 0.0 && num2 <= 90.0)
							{
								num9 += 5.0;
								num = num2;
								double num10 = 0.017453292519943295 * num;
								ColumnChart.SetRotation(num9, num, num10, centerOfRotation, num3, num4, title);
							}
						}
					}
					else
					{
						Point centerOfRotation2 = default(Point);
						double num11 = 0.0;
						num = 0.0;
						if (labelStyles2 == LabelStyles.OutSide)
						{
							centerOfRotation2 = new Point(canvasLeft + columnVisualSize.Width / 2.0, canvasTop + num7 / 2.0 * num8 + 10.0);
							num11 = 4.0;
							if (num2 >= -90.0 && num2 < 0.0)
							{
								num = 180.0 + num2;
								double num12 = 0.017453292519943295 * num;
								num11 += title.TextBlockDesiredSize.Width;
								num = (num12 - 3.1415926535897931) * 57.295779513082323;
								ColumnChart.SetRotation(num11, num, num12, centerOfRotation2, num3, num4, title);
							}
							else if (num2 > 0.0 && num2 <= 90.0)
							{
								num = num2;
								double num12 = 0.017453292519943295 * num;
								ColumnChart.SetRotation(num11, num, num12, centerOfRotation2, num3, num4, title);
							}
						}
						else
						{
							centerOfRotation2 = new Point(canvasLeft + columnVisualSize.Width / 2.0, canvasTop - num7 / 2.0 * num8);
							if (num2 > 0.0 && num2 <= 90.0)
							{
								num = num2 - 180.0;
								double num12 = 0.017453292519943295 * num;
								num11 += title.TextBlockDesiredSize.Width;
								num = (num12 - 3.1415926535897931) * 57.295779513082323;
								ColumnChart.SetRotation(num11, num, num12, centerOfRotation2, num3, num4, title);
							}
							else if (num2 >= -90.0 && num2 < 0.0)
							{
								num = num2;
								double num12 = 0.017453292519943295 * num;
								ColumnChart.SetRotation(num11, num, num12, centerOfRotation2, num3, num4, title);
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

		internal static void SetRotation(double radius, double angle, double angleInRadian, Point centerOfRotation, double labelLeft, double labelTop, Title textBlock)
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

		private static void SetMarkerPosition(Size columnVisualSize, Chart chart, IDataPoint dataPoint, string labelText, Size markerSize, double canvasLeft, double canvasTop, Point markerPosition)
		{
			Marker marker = dataPoint.DpInfo.Marker;
			bool flag = dataPoint.DpInfo.InternalYValue >= 0.0;
			if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && !string.IsNullOrEmpty(labelText))
			{
				marker.CreateVisual((chart != null) ? chart.FlowDirection : FlowDirection.LeftToRight);
				if (columnVisualSize.Width < marker.TextBlockSize.Width)
				{
					marker.TextOrientation = Orientation.Vertical;
				}
				else
				{
					marker.TextOrientation = Orientation.Horizontal;
				}
				LabelStyles labelStyles = LabelStyles.OutSide;
				if (flag)
				{
					if (marker.TextOrientation == Orientation.Vertical)
					{
						if (canvasTop - marker.MarkerActualSize.Width - marker.MarkerSize.Height < 0.0)
						{
							labelStyles = LabelStyles.Inside;
						}
					}
					else if (canvasTop - marker.MarkerActualSize.Height - marker.MarkerSize.Height < 0.0)
					{
						labelStyles = LabelStyles.Inside;
					}
				}
				else if (marker.TextOrientation == Orientation.Vertical)
				{
					if (canvasTop + markerPosition.Y + marker.MarkerActualSize.Width + marker.MarkerSize.Height > chart.PlotArea.BorderElement.Height + chart.ChartArea.PLANK_DEPTH - chart.ChartArea.PLANK_THICKNESS)
					{
						labelStyles = LabelStyles.Inside;
					}
				}
				else if (canvasTop + markerPosition.Y + marker.MarkerActualSize.Height + marker.MarkerSize.Height > chart.PlotArea.BorderElement.Height + chart.ChartArea.PLANK_DEPTH - chart.ChartArea.PLANK_THICKNESS)
				{
					labelStyles = LabelStyles.Inside;
				}
				marker.TextAlignmentX = AlignmentX.Center;
				if (!(bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
				{
					if (chart.View3D)
					{
						if (labelStyles == LabelStyles.OutSide)
						{
							if (flag)
							{
								marker.MarkerSize = new Size(markerSize.Width / 2.0 + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS, markerSize.Height / 2.0 + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS);
							}
							else
							{
								marker.MarkerSize = new Size(markerSize.Width / 2.0, markerSize.Height / 2.0);
							}
						}
						else
						{
							marker.MarkerSize = new Size(markerSize.Width / 2.0, markerSize.Height / 2.0);
						}
					}
				}
				else if (chart.View3D)
				{
					labelStyles = LabelStyles.Inside;
				}
				if (flag)
				{
					marker.TextAlignmentY = ((labelStyles == LabelStyles.Inside) ? AlignmentY.Bottom : AlignmentY.Top);
					return;
				}
				marker.TextAlignmentY = ((labelStyles == LabelStyles.Inside) ? AlignmentY.Top : AlignmentY.Bottom);
			}
		}

		private static Marker GetMarker(Size columnVisualSize, Chart chart, IDataPoint dataPoint, double left, double top)
		{
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled);
			double num = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
			if (flag)
			{
				Size markerSize = new Size(num, num);
				string labelText = "";
				dataPoint.DpInfo.Marker = ColumnChart.CreateNewMarker(dataPoint, markerSize, labelText);
				if (!flag)
				{
					dataPoint.DpInfo.Marker.MarkerFillColor = Graphics.TRANSPARENT_BRUSH;
					dataPoint.DpInfo.Marker.BorderColor = Graphics.TRANSPARENT_BRUSH;
				}
				if (dataPoint.DpInfo.InternalYValue >= 0.0)
				{
					if (chart.View3D)
					{
						new Point(columnVisualSize.Width / 2.0, 0.0);
					}
					else
					{
						new Point(columnVisualSize.Width / 2.0, 0.0);
					}
				}
				else if (chart.View3D)
				{
					new Point(columnVisualSize.Width / 2.0, columnVisualSize.Height);
				}
				else
				{
					new Point(columnVisualSize.Width / 2.0, columnVisualSize.Height);
				}
				dataPoint.DpInfo.Marker.Tag = new ElementData
				{
					Element = dataPoint
				};
				dataPoint.DpInfo.Marker.CreateVisual((chart != null) ? chart.FlowDirection : FlowDirection.LeftToRight);
				return dataPoint.DpInfo.Marker;
			}
			return null;
		}

		internal static double CalculateWidthOfEachColumn(Chart chart, double width, Axis axisX, RenderAs chartType, Orientation direction)
		{
			double num = chart.MaxDataPointWidth;
			PlotDetails plotDetails = chart.PlotDetails;
			double num2 = plotDetails.GetMinOfMinDifferencesForXValue(new RenderAs[]
			{
				chartType,
				RenderAs.StackedColumn,
				RenderAs.StackedColumn100
			});
			if (double.IsPositiveInfinity(num2))
			{
				num2 = 0.0;
			}
			double arg_4A_0 = ColumnChart.COLUMN_GAP_RATIO;
			double num3 = (double)plotDetails.DrawingDivisionFactor;
			double num4;
			if (num2 == 0.0)
			{
				num4 = width * 0.5 / num3;
			}
			else
			{
				num4 = Graphics.ValueToPixelPosition(0.0, width, axisX.InternalAxisMinimum, axisX.InternalAxisMaximum, num2 + axisX.InternalAxisMinimum);
				num4 *= 1.0 - ((direction == Orientation.Horizontal) ? ColumnChart.COLUMN_GAP_RATIO : BarChart.BAR_GAP_RATIO);
				num4 /= num3;
			}
			num = num / 100.0 * ((direction == Orientation.Horizontal) ? chart.PlotArea.Width : chart.PlotArea.Height);
			if (!double.IsNaN(chart.DataPointWidth) && chart.DataPointWidth >= 0.0)
			{
				num4 = chart.DataPointWidth / 100.0 * ((direction == Orientation.Horizontal) ? chart.PlotArea.Width : chart.PlotArea.Height);
			}
			if (num4 > num)
			{
				num4 = num;
			}
			if (num4 < 1.0 && num >= 1.0 && double.IsNaN(chart.DataPointWidth))
			{
				num4 = 1.0;
			}
			else if (num4 < 1.0 && (double.IsPositiveInfinity(num) || double.IsNaN(num)) && double.IsNaN(chart.DataPointWidth))
			{
				num4 = 1.0;
			}
			return num4;
		}

		private static int GetColumnZIndex(double left, double top, bool isPositive)
		{
			int num = 0;
			int num2 = (int)left;
			if (num2 == 0)
			{
				num2++;
			}
			return isPositive ? (num + num2) : (num + -2147483648 + Math.Abs(num2));
		}

		internal static int GetStackedColumnZIndex(double left, double top, bool isPositive, int index, int maxOrMinIndex)
		{
			int num = (int)left;
			int result;
			if (isPositive)
			{
				result = num + index;
			}
			else
			{
				if (num == 0)
				{
					num = 1;
				}
				num = Math.Abs(num) + maxOrMinIndex;
				result = -2147483648 + (num + index);
			}
			return result;
		}

		private static Storyboard ApplyColumnChartAnimation(DataSeries currentDataSeries, Panel column, Storyboard storyboard, bool isPositive, double beginTime, double[] timeCollection, double[] valueCollection, RenderAs renderAs)
		{
			string property;
			ScaleTransform scaleTransform;
			if (renderAs == RenderAs.Column)
			{
				property = "(ScaleTransform.ScaleY)";
				scaleTransform = new ScaleTransform
				{
					ScaleY = valueCollection[0]
				};
				column.RenderTransformOrigin = (isPositive ? new Point(0.5, 1.0) : new Point(0.5, 0.0));
			}
			else
			{
				scaleTransform = new ScaleTransform
				{
					ScaleX = valueCollection[0]
				};
				property = "(ScaleTransform.ScaleX)";
				column.RenderTransformOrigin = (isPositive ? new Point(0.0, 0.5) : new Point(1.0, 0.5));
			}
			column.RenderTransform = scaleTransform;
			DoubleCollection values = Graphics.GenerateDoubleCollection(valueCollection);
			DoubleCollection frameTime = Graphics.GenerateDoubleCollection(timeCollection);
			List<KeySpline> splines = null;
			if (valueCollection.Length == 2)
			{
				splines = AnimationHelper.GenerateKeySplineList(new Point[]
				{
					new Point(0.0, 0.0),
					new Point(1.0, 1.0),
					new Point(0.0, 1.0),
					new Point(0.5, 1.0)
				});
			}
			DoubleAnimationUsingKeyFrames value = AnimationHelper.CreateDoubleAnimation(currentDataSeries, scaleTransform, property, beginTime, frameTime, values, splines);
			storyboard.Children.Add(value);
			return storyboard;
		}

		internal static void SetColumnParms(ref RectangularChartShapeParams columnParams, ref Chart chart, IDataPoint dataPoint, bool isPositive)
		{
			LabelStyles value = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			Brush labelFontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
			FontStyle labelFontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
			Brush labelBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
			FontFamily labelFontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
			double labelFontSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontSize);
			FontWeight labelFontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
			double markerSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
			double markerScale = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
			MarkerTypes markerType = (MarkerTypes)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerType);
			Brush markerColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerColor);
			Brush markerBorderColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor);
			bool isMarkerEnabled = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled);
			bool shadow = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled);
			bool lighting = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled);
			Brush borderBrush = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor);
			Thickness thickness = (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness);
			BorderStyles borderStyle = (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle);
			Thickness markerBorderThickness = (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderThickness);
			columnParams.Bevel = dataPoint.Parent.Bevel.Value;
			columnParams.Lighting = lighting;
			columnParams.Shadow = shadow;
			columnParams.BorderBrush = borderBrush;
			columnParams.BorderThickness = thickness.Left;
			columnParams.BorderStyle = ExtendedGraphics.GetDashArray(borderStyle);
			columnParams.IsPositive = isPositive;
			columnParams.BackgroundBrush = dataPoint.Color;
			columnParams.IsMarkerEnabled = isMarkerEnabled;
			columnParams.MarkerType = markerType;
			columnParams.MarkerColor = markerColor;
			columnParams.MarkerBorderColor = markerBorderColor;
			columnParams.MarkerBorderThickness = markerBorderThickness;
			columnParams.MarkerScale = markerScale;
			columnParams.MarkerSize = markerSize;
			columnParams.IsLabelEnabled = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled);
			columnParams.LabelStyle = new LabelStyles?(value);
			columnParams.LabelText = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText));
			columnParams.LabelBackground = labelBackground;
			columnParams.LabelFontColor = labelFontColor;
			columnParams.LabelFontSize = labelFontSize;
			columnParams.LabelFontFamily = labelFontFamily;
			columnParams.LabelFontStyle = labelFontStyle;
			columnParams.LabelFontWeight = labelFontWeight;
			columnParams.TagReference = dataPoint;
		}

		internal static Marker CreateNewMarker(IDataPoint dataPoint, Size markerSize, string labelText)
		{
			bool markerBevel = false;
			Brush fontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
			FontStyle fontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
			Brush textBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
			FontFamily fontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
			double fontSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontSize);
			FontWeight fontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
			double scaleFactor = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
			MarkerTypes markerType = (MarkerTypes)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerType);
			Brush markerColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerColor);
			bool arg_93_0 = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled);
			Thickness thickness = (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderThickness);
			Brush borderColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor);
			Marker marker = new Marker();
			marker.AssignPropertiesValue(markerType, scaleFactor, markerSize, markerBevel, markerColor, labelText);
			marker.MarkerSize = markerSize;
			marker.BorderColor = borderColor;
			marker.BorderThickness = thickness.Left;
			marker.MarkerType = markerType;
			marker.FontColor = fontColor;
			marker.FontFamily = fontFamily;
			marker.FontSize = fontSize;
			marker.FontStyle = fontStyle;
			marker.FontWeight = fontWeight;
			marker.TextBackground = textBackground;
			return marker;
		}

		internal static Dictionary<Axis, Dictionary<Axis, int>> GetSeriesIndex(List<DataSeries> seriesList)
		{
			Dictionary<Axis, Dictionary<Axis, int>> dictionary = new Dictionary<Axis, Dictionary<Axis, int>>();
			var enumerable = from series in seriesList
			where series.Enabled == true
			group series by new
			{
				series.PlotGroup.AxisX,
				series.PlotGroup.AxisY
			};
			int num = 0;
			foreach (var current in enumerable)
			{
				if (dictionary.ContainsKey(current.Key.AxisY))
				{
					if (!dictionary[current.Key.AxisY].ContainsKey(current.Key.AxisX))
					{
						dictionary[current.Key.AxisY].Add(current.Key.AxisX, num++);
					}
				}
				else
				{
					dictionary.Add(current.Key.AxisY, new Dictionary<Axis, int>());
					dictionary[current.Key.AxisY].Add(current.Key.AxisX, num++);
				}
			}
			return dictionary;
		}

		private static void CreateColumnDataPointVisual(Canvas parentCanvas, Canvas labelCanvas, PlotDetails plotDetails, IDataPoint dataPoint, bool isPositive, double widthOfAcolumn, double depth3D, bool animationEnabled)
		{
			if (widthOfAcolumn < 0.0)
			{
				return;
			}
			DataSeries parent = dataPoint.Parent;
			Chart chart = dataPoint.Chart as Chart;
			dataPoint.DpInfo.OldLightingState = false;
			dataPoint.Parent.Faces = new Faces
			{
				Visual = parentCanvas,
				LabelCanvas = labelCanvas
			};
			PlotGroup plotGroup = dataPoint.Parent.PlotGroup;
			double num = 0.0;
			if (plotGroup.AxisY.InternalAxisMinimum > 0.0)
			{
				num = plotGroup.AxisY.InternalAxisMinimum;
			}
			if (plotGroup.AxisY.InternalAxisMaximum < 0.0)
			{
				num = 0.0;
			}
			double num2;
			double num3;
			if (chart.InternalSeries.Count == 1)
			{
				num2 = 1.0;
				num3 = 0.0;
			}
			else
			{
				List<DataSeries> seriesFromDataPoint = plotDetails.GetSeriesFromDataPoint(dataPoint);
				num2 = (double)seriesFromDataPoint.Count;
				num3 = (double)seriesFromDataPoint.IndexOf(dataPoint.Parent);
			}
			double internalYValue = dataPoint.DpInfo.InternalYValue;
			double num4 = Graphics.ValueToPixelPosition(0.0, parentCanvas.Width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
			num4 += (num3 - num2 / 2.0) * widthOfAcolumn;
			double num5 = Graphics.ValueToPixelPosition(parentCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, 0.0);
			double num6;
			double num7;
			if (isPositive)
			{
				num6 = Graphics.ValueToPixelPosition(parentCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num);
				num7 = Graphics.ValueToPixelPosition(parentCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, double.IsNaN(internalYValue) ? 0.0 : internalYValue);
				if (plotGroup.AxisY.AxisMinimum != null && internalYValue < num)
				{
					num7 = (num6 = num5 + 100.0);
				}
				if (chart.View3D && internalYValue > plotGroup.AxisY.InternalAxisMaximum && plotGroup.AxisY.InternalAxisMaximum > 0.0)
				{
					num7 = 0.0;
				}
			}
			else
			{
				num6 = Graphics.ValueToPixelPosition(parentCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, double.IsNaN(internalYValue) ? 0.0 : internalYValue);
				num7 = Graphics.ValueToPixelPosition(parentCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num);
				if ((internalYValue > num && num < 0.0) || (internalYValue < num && num > 0.0) || (internalYValue < 0.0 && plotGroup.AxisY.InternalAxisMinimum == 0.0))
				{
					num6 = (num7 = num6 + 100.0);
				}
				else if (plotGroup.AxisY.InternalAxisMinimum < 0.0 && plotGroup.AxisY.InternalAxisMinimum > internalYValue)
				{
					num6 = parentCanvas.Height;
				}
			}
			double num8 = Math.Abs(num7 - num6);
			double minPointHeight = dataPoint.Parent.MinPointHeight;
			if (num8 < minPointHeight)
			{
				if (internalYValue == 0.0)
				{
					if (plotGroup.AxisY.InternalAxisMaximum <= 0.0)
					{
						num6 += minPointHeight - num8;
					}
					else
					{
						num7 -= minPointHeight - num8;
					}
				}
				else if (isPositive)
				{
					num7 -= minPointHeight - num8;
				}
				else
				{
					num6 += minPointHeight - num8;
				}
				num8 = minPointHeight;
			}
			Size columnVisualSize = new Size(widthOfAcolumn, num8);
			Faces faces;
			Panel panel;
			if (chart.View3D)
			{
				Brush borderBrush = null;
				double left = ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left;
				if (left != 0.0)
				{
					borderBrush = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor);
				}
				Brush topBrush = null;
				if (isPositive && internalYValue > plotGroup.AxisY.InternalAxisMaximum)
				{
					topBrush = (((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) ? Graphics.GetRightFaceBrush(dataPoint.Color) : Graphics.GetDarkerBrush(dataPoint.Color, 0.55));
				}
				faces = ColumnChart.Get3DColumn(dataPoint, widthOfAcolumn, num8, depth3D, dataPoint.Color, null, topBrush, null, (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled), (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle), borderBrush, left);
				panel = (faces.Visual as Panel);
				panel.SetValue(Panel.ZIndexProperty, ColumnChart.GetColumnZIndex(num4, num7, internalYValue > 0.0));
				dataPoint.DpInfo.Faces = faces;
				if (!dataPoint.Parent.LightWeight.Value && !VisifireControl.IsMediaEffectsEnabled)
				{
					ColumnChart.ApplyOrRemoveShadow4XBAP(dataPoint, false, false);
				}
			}
			else
			{
				faces = ColumnChart.Get2DColumn(dataPoint, widthOfAcolumn, num8, false, false, isPositive);
				panel = (faces.Visual as Panel);
			}
			dataPoint.DpInfo.Faces = faces;
			if (!dataPoint.Parent.LightWeight.Value && VisifireControl.IsMediaEffectsEnabled)
			{
				ColumnChart.ApplyEffects(chart, dataPoint);
				ColumnChart.ApplyOrRemoveShadow(chart, dataPoint);
			}
			panel.SetValue(Canvas.LeftProperty, num4);
			panel.SetValue(Canvas.TopProperty, num7);
			parentCanvas.Children.Add(panel);
			dataPoint.DpInfo.IsTopOfStack = true;
			if (!dataPoint.Parent.LightWeight.Value)
			{
				ColumnChart.CreateOrUpdateMarker4VerticalChart(dataPoint, labelCanvas, columnVisualSize, num4, num7, internalYValue);
			}
			if (isPositive)
			{
				dataPoint.DpInfo.VisualPosition = new Point(num4 + columnVisualSize.Width / 2.0, num7);
			}
			else
			{
				dataPoint.DpInfo.VisualPosition = new Point(num4 + columnVisualSize.Width / 2.0, num6);
			}
			if (animationEnabled)
			{
				if (dataPoint.Parent.Storyboard == null)
				{
					dataPoint.Parent.Storyboard = new Storyboard();
				}
				parent = dataPoint.Parent;
				double beginTime = 1.0;
				dataPoint.Parent.Storyboard = ColumnChart.ApplyColumnChartAnimation(parent, panel, dataPoint.Parent.Storyboard, isPositive, beginTime, new double[]
				{
					0.0,
					1.0
				}, new double[]
				{
					0.0,
					1.0
				}, dataPoint.Parent.RenderAs);
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

		internal static void AttachEvents2Visual(VisifireElement obj, IDataPoint dpAsSender)
		{
			dpAsSender.AttachEvents2Visual(obj, dpAsSender, dpAsSender.DpInfo.Faces.Visual);
			if (dpAsSender.DpInfo.Marker != null)
			{
				dpAsSender.AttachEvents2Visual(obj, dpAsSender, dpAsSender.DpInfo.Marker.Visual);
			}
			if (dpAsSender.DpInfo.LabelVisual != null)
			{
				dpAsSender.AttachEvents2Visual(obj, dpAsSender, dpAsSender.DpInfo.LabelVisual);
			}
		}

		internal static void CleanUpMarkerAndLabel(IDataPoint dataPoint, Canvas labelCanvas)
		{
			if (dataPoint.DpInfo.Marker != null && dataPoint.DpInfo.Marker.Visual != null)
			{
				labelCanvas.Children.Remove(dataPoint.DpInfo.Marker.Visual);
				dataPoint.DpInfo.Marker.Visual = null;
			}
			if (dataPoint.DpInfo.LabelVisual != null)
			{
				labelCanvas.Children.Remove(dataPoint.DpInfo.LabelVisual);
				dataPoint.DpInfo.LabelVisual = null;
			}
		}

		private static void CreateOrUpdateMarker4VerticalChart(IDataPoint dataPoint, Canvas labelCanvas, Size columnVisualSize, double left, double top, double prevSum)
		{
			if (dataPoint.DpInfo.Faces == null)
			{
				return;
			}
			Chart chart = dataPoint.Chart as Chart;
			if (chart.ChartArea != null && !chart.ChartArea._isFirstTimeRender)
			{
				ColumnChart.CleanUpMarkerAndLabel(dataPoint, labelCanvas);
			}
			if (!DataPointHelper.GetMarkerEnabled_DataPoint(dataPoint).HasValue && dataPoint.Parent.GetValue(DataSeries.MarkerEnabledProperty) == null && !DataPointHelper.GetLabelEnabled_DataPoint(dataPoint).HasValue && dataPoint.Parent.GetValue(DataSeries.LabelEnabledProperty) == null)
			{
				return;
			}
			if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
			{
				Point point = default(Point);
				double internalYValue = dataPoint.DpInfo.InternalYValue;
				if (internalYValue >= 0.0)
				{
					point = (chart.View3D ? new Point(columnVisualSize.Width / 2.0, 0.0) : new Point(columnVisualSize.Width / 2.0, 0.0));
				}
				else
				{
					point = (chart.View3D ? new Point(columnVisualSize.Width / 2.0, columnVisualSize.Height) : new Point(columnVisualSize.Width / 2.0, columnVisualSize.Height));
				}
				Marker marker = ColumnChart.GetMarker(columnVisualSize, chart, dataPoint, left, top);
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
				if (marker != null)
				{
					if (dataPoint.IsLightDataPoint)
					{
						dataPoint.DpInfo.Marker.Visual.Opacity = dataPoint.Parent.InternalOpacity;
					}
					else
					{
						dataPoint.DpInfo.Marker.Visual.Opacity = (dataPoint as DataPoint).InternalOpacity * dataPoint.Parent.InternalOpacity;
					}
					marker.AddToParent(labelCanvas, left + point.X, top + point.Y, new Point(0.5, 0.5));
				}
				if (marker != null && marker.Visual != null && !chart.IndicatorEnabled)
				{
					dataPoint.AttachToolTip(chart, dataPoint, marker.Visual);
				}
				dataPoint.DpInfo.Marker = marker;
			}
			if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled))
			{
				double arg_30D_0 = dataPoint.DpInfo.InternalYValue;
				double yValue = dataPoint.YValue;
				bool flag = false;
				if (!double.IsNaN(yValue) && yValue > 0.0)
				{
					flag = true;
				}
				double canvasTop = top + columnVisualSize.Height;
				if (flag)
				{
					ColumnChart.CreateLabel(chart, columnVisualSize, flag, dataPoint.DpInfo.IsTopOfStack, dataPoint, left, top, ref labelCanvas, prevSum);
				}
				else
				{
					ColumnChart.CreateLabel(chart, columnVisualSize, flag, dataPoint.DpInfo.IsTopOfStack, dataPoint, left, canvasTop, ref labelCanvas, prevSum);
				}
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
					double arg_4B4_0 = dataPoint.Parent.PlotGroup.AxisY.InternalAxisMaximum;
				}
				if (dataPoint.DpInfo.LabelVisual != null && !chart.IndicatorEnabled)
				{
					dataPoint.AttachToolTip(chart, dataPoint, dataPoint.DpInfo.LabelVisual);
				}
			}
		}

		internal static Canvas GetVisualObjectForColumnChart(Panel preExistingPanel, double width, double height, PlotDetails plotDetails, List<DataSeries> dataSeriesList4Rendering, Chart chart, double plankDepth, bool animationEnabled)
		{
			if (double.IsNaN(width) || double.IsNaN(height) || width <= 0.0 || height <= 0.0)
			{
				return null;
			}
			DataSeries dataSeries = null;
			bool flag = true;
			Canvas canvas;
			Canvas canvas2;
			Canvas canvas3;
			RenderHelper.PrepareCanvas4Drawing(preExistingPanel as Canvas, out canvas, out canvas2, out canvas3, width, height);
			double num = plankDepth / (double)plotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
			double num2 = num * (double)(plotDetails.SeriesDrawingIndex[dataSeriesList4Rendering[0]] + 1);
			canvas.SetValue(Canvas.TopProperty, num2);
			canvas.SetValue(Canvas.LeftProperty, -num2);
			if (dataSeriesList4Rendering[0].PlotGroup == null)
			{
				return null;
			}
			Axis axisX = dataSeriesList4Rendering[0].PlotGroup.AxisX;
			Axis axisY = dataSeriesList4Rendering[0].PlotGroup.AxisY;
			foreach (DataSeries current in dataSeriesList4Rendering)
			{
				if (current.PlotGroup.AxisY != null && current.PlotGroup.AxisY.ViewportRangeEnabled)
				{
					axisY = current.PlotGroup.AxisY;
					break;
				}
			}
			if (axisY == null)
			{
				return null;
			}
			double num3;
			if (plotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				num3 = ColumnChart.CalculateWidthOfEachColumn(chart, width, axisX, RenderAs.Column, Orientation.Horizontal);
			}
			else
			{
				num3 = ColumnChart.CalculateWidthOfEachColumn(chart, height, axisX, RenderAs.Bar, Orientation.Vertical);
			}
			Dictionary<double, SortDataPoints> dataPointsGroupedByXValue = plotDetails.GetDataPointsGroupedByXValue((plotDetails.ChartOrientation == ChartOrientationType.Vertical) ? RenderAs.Column : RenderAs.Bar);
			double[] xValuesUnderViewPort = RenderHelper.GetXValuesUnderViewPort(dataPointsGroupedByXValue.Keys.ToList<double>(), axisX, axisY, false);
			double[] array = xValuesUnderViewPort;
			for (int i = 0; i < array.Length; i++)
			{
				double key = array[i];
				List<IDataPoint> positive = dataPointsGroupedByXValue[key].Positive;
				List<IDataPoint> negative = dataPointsGroupedByXValue[key].Negative;
				foreach (IDataPoint current2 in positive)
				{
					if (!(current2.Enabled == false) && current2.Parent != null)
					{
						current2.Parent.Faces = new Faces
						{
							Visual = canvas3,
							LabelCanvas = canvas2
						};
						current2.DpInfo.Faces = null;
						dataSeries = current2.Parent;
						if (plotDetails.ChartOrientation == ChartOrientationType.Vertical)
						{
							ColumnChart.CreateColumnDataPointVisual(canvas3, canvas2, plotDetails, current2, true, num3, num, animationEnabled);
						}
						else
						{
							BarChart.CreateBarDataPointVisual(current2, canvas2, canvas3, true, num3, num, animationEnabled);
						}
					}
				}
				foreach (IDataPoint current3 in negative)
				{
					if (!(current3.Enabled == false) && current3.Parent != null && !double.IsNaN(current3.DpInfo.InternalYValue))
					{
						current3.Parent.Faces = new Faces
						{
							Visual = canvas3,
							LabelCanvas = canvas2
						};
						current3.DpInfo.Faces = null;
						dataSeries = current3.Parent;
						if (plotDetails.ChartOrientation == ChartOrientationType.Vertical)
						{
							ColumnChart.CreateColumnDataPointVisual(canvas3, canvas2, plotDetails, current3, false, num3, num, animationEnabled);
						}
						else
						{
							BarChart.CreateBarDataPointVisual(current3, canvas2, canvas3, false, num3, num, animationEnabled);
						}
					}
				}
			}
			if (animationEnabled && dataSeries != null && canvas2.Children.Count > 0)
			{
				if (dataSeries.Storyboard == null)
				{
					dataSeries.Storyboard = new Storyboard();
				}
				double beginTime = 1.0;
				dataSeries.Storyboard = AnimationHelper.ApplyOpacityAnimation(canvas2, dataSeries, dataSeries.Storyboard, beginTime, 1.0, 0.0, 1.0);
			}
			canvas3.Tag = null;
			if (xValuesUnderViewPort.Any<double>())
			{
				if (plotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					ColumnChart.CreateOrUpdatePlank(chart, dataSeriesList4Rendering[0].PlotGroup.AxisY, canvas3, num, Orientation.Horizontal);
				}
				else
				{
					ColumnChart.CreateOrUpdatePlank(chart, dataSeriesList4Rendering[0].PlotGroup.AxisY, canvas3, num, Orientation.Vertical);
				}
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
			int num4 = 0;
			List<IDataPoint> list = new List<IDataPoint>();
			foreach (DataSeries current4 in dataSeriesList4Rendering)
			{
				if (current4.LightWeight.Value)
				{
					num4++;
				}
				list.InsertRange(0, (from dp in current4.InternalDataPoints
				select dp).ToList<IDataPoint>());
			}
			List<IDataPoint> list2 = (from datapoint in list
			where datapoint.Enabled == true
			select datapoint).ToList<IDataPoint>();
			for (int j = 0; j < list2.Count; j++)
			{
				if (list2[j].Parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					double limitingYValue = list2[j].Parent.PlotGroup.GetLimitingYValue();
					if (list2[j].DpInfo.InternalYValue < limitingYValue)
					{
						flag = false;
						break;
					}
				}
			}
			if (num4 == dataSeriesList4Rendering.Count && !chart.IndicatorEnabled)
			{
				chart.ChartArea.AttachEvents2ColumnVisualFaces(chart, canvas3, list, plotDetails.ChartOrientation);
			}
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			if (plotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (flag)
				{
					rectangleGeometry.Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH - (double)(chart.View3D ? 0 : 5), width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + (double)(chart.View3D ? 0 : 10));
				}
				else
				{
					rectangleGeometry.Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH - (double)(chart.View3D ? 0 : 5), width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH + (double)(chart.View3D ? 0 : 10));
				}
				canvas.Clip = rectangleGeometry;
			}
			else
			{
				if (flag)
				{
					rectangleGeometry.Rect = new Rect((double)(-(double)(chart.View3D ? 0 : 5)) - chart.ChartArea.PLANK_THICKNESS, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + (double)(chart.View3D ? 0 : 10), height + chart.ChartArea.PLANK_DEPTH);
				}
				else
				{
					rectangleGeometry.Rect = new Rect((double)(-(double)(chart.View3D ? 0 : 5)), -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH + (double)(chart.View3D ? 0 : 10), height + chart.ChartArea.PLANK_DEPTH);
				}
				canvas.Clip = rectangleGeometry;
			}
			PlotArea plotArea = chart.PlotArea;
			RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
			if (plotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				rectangleGeometry2.Rect = new Rect(0.0, plotArea.BorderThickness.Top - chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS - plotArea.BorderThickness.Bottom - plotArea.BorderThickness.Top);
			}
			else
			{
				rectangleGeometry2.Rect = new Rect(plotArea.BorderThickness.Left, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS - plotArea.BorderThickness.Left - plotArea.BorderThickness.Right, height + chart.ChartArea.PLANK_DEPTH);
			}
			canvas3.Clip = rectangleGeometry2;
			return canvas;
		}

		internal static void Update(IElement sender, ElementTypes elementType, VcProperties property, object newValue, bool isAxisChanged)
		{
			bool flag = elementType == ElementTypes.DataPoint;
			if (flag)
			{
				ColumnChart.UpdateDataPoint(sender as IDataPoint, property, newValue, isAxisChanged);
				return;
			}
			ColumnChart.UpdateDataSeries(sender as DataSeries, property, newValue);
		}

		internal static void Update(Chart chart, RenderAs currentRenderAs, List<DataSeries> selectedDataSeries4Rendering)
		{
			bool arg_06_0 = chart.View3D;
			ChartArea chartArea = chart.ChartArea;
			Canvas chartVisualCanvas = chart.ChartArea.ChartVisualCanvas;
			Panel panel = selectedDataSeries4Rendering[0].Visual;
			if (selectedDataSeries4Rendering[0].RenderAs == RenderAs.StackedArea || selectedDataSeries4Rendering[0].RenderAs == RenderAs.StackedArea100)
			{
				if (panel != null && panel.Parent != null)
				{
					Panel panel2 = panel.Parent as Panel;
					panel2.Children.Remove(panel);
				}
				panel = null;
			}
			else if (panel != null)
			{
				panel.Width = chart.ChartArea.ChartVisualCanvas.Width;
				panel.Height = chart.ChartArea.ChartVisualCanvas.Height;
			}
			panel = chartArea.RenderSeriesFromList(panel, selectedDataSeries4Rendering);
			foreach (DataSeries current in selectedDataSeries4Rendering)
			{
				current.Visual = panel;
			}
			if ((selectedDataSeries4Rendering[0].RenderAs == RenderAs.StackedArea || selectedDataSeries4Rendering[0].RenderAs == RenderAs.StackedArea100) && panel.Parent == null)
			{
				chartVisualCanvas.Children.Add(panel);
			}
		}

		private static void UpdateDataSeries1(DataSeries dataSeries, VcProperties property, object newValue)
		{
			Chart chart = dataSeries.Chart as Chart;
			if (chart == null)
			{
				return;
			}
			bool arg_16_0 = chart.View3D;
			if (property <= VcProperties.Enabled)
			{
				if (property != VcProperties.DataPoints && property != VcProperties.Enabled)
				{
					goto IL_64;
				}
			}
			else
			{
				if (property == VcProperties.XValue)
				{
					chart.ChartArea.RenderSeries();
					goto IL_64;
				}
				switch (property)
				{
				case VcProperties.YValue:
				case VcProperties.YValues:
					break;
				case VcProperties.YValueFormatString:
					goto IL_64;
				default:
					goto IL_64;
				}
			}
			chart.ChartArea.RenderSeries();
			IL_64:
			chart.ChartArea.AttachOrDetachIntaractivity(chart.InternalSeries);
			Chart.SelectDataPoints(chart);
		}

		private static void UpdateDataSeries(DataSeries dataSeries, VcProperties property, object newValue)
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
			if (dataSeries != null)
			{
				dataSeries.isSeriesRenderPending = false;
			}
			PlotGroup arg_24_0 = dataSeries.PlotGroup;
			if (dataSeries.Faces != null)
			{
				if (dataSeries.Faces.Visual == null || dataSeries.Faces.LabelCanvas == null)
				{
					return;
				}
				Canvas canvas = dataSeries.Faces.Visual as Canvas;
				Canvas labelCanvas = dataSeries.Faces.LabelCanvas;
				canvas.Children.Clear();
				labelCanvas.Children.Clear();
			}
			else if (dataSeries.Faces == null)
			{
				if (dataSeries.RenderAs == RenderAs.Column)
				{
					ColumnChart.Update(chart, dataSeries.RenderAs, (from ds in chart.InternalSeries
					where ds.RenderAs == RenderAs.Column
					select ds).ToList<DataSeries>());
					return;
				}
				if (dataSeries.RenderAs == RenderAs.Bar)
				{
					ColumnChart.Update(chart, dataSeries.RenderAs, (from ds in chart.InternalSeries
					where ds.RenderAs == RenderAs.Bar
					select ds).ToList<DataSeries>());
					return;
				}
				chart.ChartArea.RenderSeries();
				chart.ChartArea.AttachOrDetachIntaractivity(chart.InternalSeries);
				Chart.SelectDataPoints(chart);
				return;
			}
			double height = chart.ChartArea.ChartVisualCanvas.Height;
			double width = chart.ChartArea.ChartVisualCanvas.Width;
			if (property > VcProperties.DataPointUpdate)
			{
				if (property != VcProperties.Enabled)
				{
					if (property == VcProperties.XValue)
					{
						goto IL_1A5;
					}
					switch (property)
					{
					case VcProperties.YValue:
					case VcProperties.YValues:
						goto IL_1A5;
					case VcProperties.YValueFormatString:
						break;
					default:
						return;
					}
				}
				else
				{
					chart.ChartArea.RenderSeries();
					chart.ChartArea.AttachOrDetachIntaractivity(chart.InternalSeries);
					Chart.SelectDataPoints(chart);
				}
				return;
			}
			if (property != VcProperties.ViewportRangeEnabled)
			{
				switch (property)
				{
				case VcProperties.DataPoints:
				case VcProperties.DataPointUpdate:
					break;
				default:
					return;
				}
			}
			IL_1A5:
			if (dataSeries.RenderAs == RenderAs.Column || dataSeries.RenderAs == RenderAs.Bar)
			{
				ColumnChart.UpdateColumnSeries(dataSeries, width, height);
				return;
			}
			chart.ChartArea.RenderSeries();
			chart.ChartArea.AttachOrDetachIntaractivity(chart.InternalSeries);
			Chart.SelectDataPoints(chart);
		}

		private static void UpdateColumnSeries(DataSeries dataSeries, double width, double height)
		{
			if (dataSeries == null)
			{
				return;
			}
			Chart chart = dataSeries.Chart as Chart;
			if (dataSeries.Enabled == false || chart == null)
			{
				return;
			}
			Canvas canvas = dataSeries.Faces.Visual.Parent as Canvas;
			if (canvas == null)
			{
				return;
			}
			if (dataSeries.Faces.Visual == null || dataSeries.Faces.LabelCanvas == null)
			{
				return;
			}
			Canvas canvas2 = dataSeries.Faces.Visual as Canvas;
			Canvas labelCanvas = dataSeries.Faces.LabelCanvas;
			RenderHelper.UpdateParentVisualCanvasSize(chart, canvas);
			RenderHelper.UpdateParentVisualCanvasSize(chart, canvas2);
			RenderHelper.UpdateParentVisualCanvasSize(chart, labelCanvas);
			double num = chart.ChartArea.PLANK_DEPTH / (double)chart.PlotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
			double num2 = num * (double)(chart.PlotDetails.SeriesDrawingIndex[dataSeries] + 1);
			canvas.SetValue(Canvas.TopProperty, num2);
			canvas.SetValue(Canvas.LeftProperty, -num2);
			Axis axisX = dataSeries.PlotGroup.AxisX;
			Axis axisY = dataSeries.PlotGroup.AxisY;
			double num3 = 0.0;
			if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				num3 = ColumnChart.CalculateWidthOfEachColumn(chart, width, axisX, RenderAs.Column, Orientation.Horizontal);
			}
			else
			{
				num3 = ColumnChart.CalculateWidthOfEachColumn(chart, height, axisX, RenderAs.Bar, Orientation.Vertical);
			}
			Dictionary<double, SortDataPoints> dataPointsGroupedByXValue = chart.PlotDetails.GetDataPointsGroupedByXValue((chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical) ? RenderAs.Column : RenderAs.Bar);
			double[] xValuesUnderViewPort = RenderHelper.GetXValuesUnderViewPort(dataPointsGroupedByXValue.Keys.ToList<double>(), axisX, axisY, false);
			double[] array = xValuesUnderViewPort;
			for (int i = 0; i < array.Length; i++)
			{
				double key = array[i];
				List<IDataPoint> positive = dataPointsGroupedByXValue[key].Positive;
				List<IDataPoint> negative = dataPointsGroupedByXValue[key].Negative;
				foreach (IDataPoint current in positive)
				{
					if (!(current.Enabled == false) && current.Parent != null)
					{
						current.DpInfo.Faces = null;
						if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
						{
							ColumnChart.CreateColumnDataPointVisual(canvas2, labelCanvas, chart.PlotDetails, current, true, num3, num, false);
						}
						else
						{
							BarChart.CreateBarDataPointVisual(current, labelCanvas, canvas2, true, num3, num, false);
						}
					}
				}
				foreach (IDataPoint current2 in negative)
				{
					if (!(current2.Enabled == false) && current2.Parent != null && !double.IsNaN(current2.DpInfo.InternalYValue))
					{
						current2.DpInfo.Faces = null;
						if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
						{
							ColumnChart.CreateColumnDataPointVisual(canvas2, labelCanvas, chart.PlotDetails, current2, false, num3, num, false);
						}
						else
						{
							BarChart.CreateBarDataPointVisual(current2, labelCanvas, canvas2, false, num3, num, false);
						}
					}
				}
			}
			if (xValuesUnderViewPort.Any<double>())
			{
				if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					ColumnChart.CreateOrUpdatePlank(chart, dataSeries.PlotGroup.AxisY, canvas2, num, Orientation.Horizontal);
				}
				else
				{
					ColumnChart.CreateOrUpdatePlank(chart, dataSeries.PlotGroup.AxisY, canvas2, num, Orientation.Vertical);
				}
			}
			List<IDataPoint> list = (from datapoint in dataSeries.InternalDataPoints
			where datapoint.Enabled == true
			select datapoint).ToList<IDataPoint>();
			bool flag = true;
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].Parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					double limitingYValue = list[j].Parent.PlotGroup.GetLimitingYValue();
					if (list[j].DpInfo.InternalYValue < limitingYValue)
					{
						flag = false;
						break;
					}
				}
			}
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (flag)
				{
					rectangleGeometry.Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH - (double)(chart.View3D ? 0 : 5), width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + (double)(chart.View3D ? 0 : 10));
				}
				else
				{
					rectangleGeometry.Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH - (double)(chart.View3D ? 0 : 5), width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH + (double)(chart.View3D ? 0 : 10));
				}
				canvas.Clip = rectangleGeometry;
			}
			else
			{
				if (flag)
				{
					rectangleGeometry.Rect = new Rect((double)(-(double)(chart.View3D ? 0 : 5)) - chart.ChartArea.PLANK_THICKNESS, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + (double)(chart.View3D ? 0 : 10), height + chart.ChartArea.PLANK_DEPTH);
				}
				else
				{
					rectangleGeometry.Rect = new Rect((double)(-(double)(chart.View3D ? 0 : 5)), -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH + (double)(chart.View3D ? 0 : 10), height + chart.ChartArea.PLANK_DEPTH);
				}
				canvas.Clip = rectangleGeometry;
			}
			PlotArea plotArea = chart.PlotArea;
			RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
			if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				rectangleGeometry2.Rect = new Rect(0.0, plotArea.BorderThickness.Top - chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS - plotArea.BorderThickness.Bottom - plotArea.BorderThickness.Top);
			}
			else
			{
				rectangleGeometry2.Rect = new Rect(plotArea.BorderThickness.Left, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS - plotArea.BorderThickness.Left - plotArea.BorderThickness.Right, height + chart.ChartArea.PLANK_DEPTH);
			}
			canvas2.Clip = rectangleGeometry2;
		}

		private static void Update2DAnd3DColumnBorderColor(IDataPoint dataPoint, bool view3d)
		{
			Faces faces = dataPoint.DpInfo.Faces;
			foreach (Shape current in faces.BorderElements)
			{
				Shape shape = current;
				if (shape != null)
				{
					CornerRadius cornerRadius = (CornerRadius)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.RadiusX);
					CornerRadius cornerRadius2 = (CornerRadius)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.RadiusY);
					bool value = dataPoint.DpInfo.IsTopOfStack;
					if ((cornerRadius == new CornerRadius(0.0) || cornerRadius == new CornerRadius(0.0, 0.0, 0.0, 0.0)) && (cornerRadius2 == new CornerRadius(0.0) || cornerRadius2 == new CornerRadius(0.0, 0.0, 0.0, 0.0)))
					{
						value = false;
					}
					if (view3d)
					{
						ExtendedGraphics.UpdateBorderOf3DRectangle(shape, ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left, ExtendedGraphics.GetDashArray((BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle)), (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor));
					}
					else
					{
						ExtendedGraphics.UpdateBorderOf2DRectangle(shape, ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left, ExtendedGraphics.GetDashArray((BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle)), (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor), cornerRadius, cornerRadius2, new bool?(value));
					}
				}
			}
		}

		private static void Update2DAnd3DColumnColor(IDataPoint dataPoint, Brush newValue)
		{
			Brush brush = (DataPointHelper.GetColor_DataPoint(dataPoint) != null) ? DataPointHelper.GetColor_DataPoint(dataPoint) : ((newValue != null) ? newValue : dataPoint.Color);
			Faces faces = dataPoint.DpInfo.Faces;
			if (faces == null)
			{
				return;
			}
			if (!dataPoint.Parent.LightWeight.Value)
			{
				using (List<DependencyObject>.Enumerator enumerator = faces.Parts.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FrameworkElement frameworkElement = (FrameworkElement)enumerator.Current;
						string visualElementName;
						if (frameworkElement != null && frameworkElement.Tag != null && (visualElementName = (frameworkElement.Tag as ElementData).VisualElementName) != null)
						{
							if (!(visualElementName == "ColumnBase"))
							{
								if (!(visualElementName == "FrontFace"))
								{
									if (!(visualElementName == "TopFace"))
									{
										if (visualElementName == "RightFace")
										{
											(frameworkElement as Shape).Fill = (((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) ? Graphics.GetRightFaceBrush(brush) : Graphics.GetDarkerBrush(brush, 0.55));
										}
									}
									else
									{
										(frameworkElement as Shape).Fill = (((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) ? Graphics.GetTopFaceBrush(brush) : Graphics.GetDarkerBrush(brush, 0.7665));
									}
								}
								else
								{
									(frameworkElement as Shape).Fill = (((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) ? Graphics.GetFrontFaceBrush(brush) : brush);
								}
							}
							else
							{
								(frameworkElement as Shape).Fill = (((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) ? Graphics.GetLightingEnabledBrush(brush, "Linear", null) : brush);
							}
						}
					}
					goto IL_284;
				}
			}
			if (faces.Visual != null)
			{
				Canvas canvas = faces.Visual as Canvas;
				if (!(dataPoint.Parent.Chart as Chart).View3D)
				{
					canvas.Background = brush;
				}
				else if ((faces.Visual as Canvas).Children.Count > 1)
				{
					Rectangle rectangle = (faces.Visual as Canvas).Children[0] as Rectangle;
					rectangle.Fill = brush;
					Rectangle rectangle2 = (faces.Visual as Canvas).Children[1] as Rectangle;
					rectangle2.Fill = Graphics.GetDarkerBrush(brush, 0.7665);
					Rectangle rectangle3 = (faces.Visual as Canvas).Children[2] as Rectangle;
					rectangle3.Fill = Graphics.GetDarkerBrush(brush, 0.55);
				}
			}
			IL_284:
			foreach (FrameworkElement current in faces.BevelElements)
			{
				string visualElementName2;
				if (current != null && (visualElementName2 = (current.Tag as ElementData).VisualElementName) != null)
				{
					if (!(visualElementName2 == "TopBevel"))
					{
						if (!(visualElementName2 == "LeftBevel"))
						{
							if (!(visualElementName2 == "RightBevel"))
							{
								if (visualElementName2 == "BottomBevel")
								{
									(current as Shape).Fill = null;
								}
							}
							else
							{
								(current as Shape).Fill = Graphics.GetBevelSideBrush((double)(((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) ? -110 : 180), brush);
							}
						}
						else
						{
							(current as Shape).Fill = Graphics.GetBevelSideBrush((double)(((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) ? -70 : 0), brush);
						}
					}
					else
					{
						(current as Shape).Fill = Graphics.GetBevelTopBrush(brush);
					}
				}
			}
			if (dataPoint.DpInfo.Marker != null && (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
			{
				dataPoint.DpInfo.Marker.BorderColor = ((DataPointHelper.GetMarkerBorderColor_DataPoint(dataPoint) == null) ? ((newValue != null) ? newValue : ((Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor))) : ((Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor)));
			}
		}

		private static void CreateOrUpdateMarker(Chart chart, IDataPoint dataPoint, Canvas labelCanvas, Canvas columnVisual)
		{
			if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				ColumnChart.CreateOrUpdateMarker4VerticalChart(dataPoint, labelCanvas, new Size(columnVisual.Width, columnVisual.Height), (double)columnVisual.GetValue(Canvas.LeftProperty), (double)columnVisual.GetValue(Canvas.TopProperty), dataPoint.DpInfo.InternalYValue);
				return;
			}
			double depth3d = chart.ChartArea.PLANK_DEPTH / (double)chart.PlotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
			BarChart.CreateOrUpdateMarker4HorizontalChart(columnVisual.Width, chart, labelCanvas, dataPoint, (double)columnVisual.GetValue(Canvas.LeftProperty), (double)columnVisual.GetValue(Canvas.TopProperty), dataPoint.DpInfo.InternalYValue >= 0.0, depth3d, dataPoint.DpInfo.InternalYValue);
		}

		private static void UpdateDataPoint(IDataPoint dataPoint, VcProperties property, object newValue, bool isAxisChanged)
		{
			if (property != VcProperties.Enabled && (dataPoint.Parent.Enabled == false || !dataPoint.Enabled.Value))
			{
				return;
			}
			Chart chart = dataPoint.Chart as Chart;
			if (chart == null)
			{
				return;
			}
			PlotDetails plotDetails = chart.PlotDetails;
			Marker marker = dataPoint.DpInfo.Marker;
			DataSeries parent = dataPoint.Parent;
			PlotGroup arg_69_0 = parent.PlotGroup;
			Canvas labelCanvas = null;
			Canvas canvas = null;
			if (parent.Faces != null)
			{
				labelCanvas = parent.Faces.LabelCanvas;
				canvas = (parent.Faces.Visual as Canvas);
			}
			if (dataPoint.DpInfo.Faces != null)
			{
				canvas = (dataPoint.DpInfo.Faces.Visual as Canvas);
			}
			if ((dataPoint.DpInfo.Faces == null || canvas == null) && property != VcProperties.Enabled && property != VcProperties.YValue && property != VcProperties.YValues && property != VcProperties.XValue)
			{
				return;
			}
			if (property <= VcProperties.LightingEnabled)
			{
				if (property <= VcProperties.Enabled)
				{
					switch (property)
					{
					case VcProperties.Bevel:
						ColumnChart.ApplyOrRemoveBevel(dataPoint);
						return;
					case VcProperties.BorderColor:
					case VcProperties.BorderStyle:
					case VcProperties.BorderThickness:
						break;
					case VcProperties.BubbleStyle:
					case VcProperties.ColorSet:
					case VcProperties.CornerRadius:
					case VcProperties.ClosestPlotDistance:
						return;
					case VcProperties.Color:
						ColumnChart.Update2DAnd3DColumnColor(dataPoint, (Brush)newValue);
						return;
					case VcProperties.Cursor:
						DataPointHelper.SetCursor2DataPointVisualFaces(dataPoint);
						return;
					default:
						switch (property)
						{
						case VcProperties.Effect:
							if (!VisifireControl.IsMediaEffectsEnabled)
							{
								return;
							}
							if (parent.RenderAs == RenderAs.Column || parent.RenderAs == RenderAs.StackedColumn || parent.RenderAs == RenderAs.StackedColumn100)
							{
								ColumnChart.ApplyEffects(chart, dataPoint);
								return;
							}
							BarChart.ApplyEffects(chart, dataPoint);
							return;
						case VcProperties.Enabled:
							if (dataPoint.DpInfo.Faces == null)
							{
								ColumnChart.UpdateDataSeries(parent, property, newValue);
								return;
							}
							if ((dataPoint.Parent.RenderAs == RenderAs.Column || parent.RenderAs == RenderAs.Bar) && dataPoint.DpInfo.Faces.Visual != null)
							{
								dataPoint.DpInfo.Faces.Visual.Visibility = (((bool)newValue) ? Visibility.Visible : Visibility.Collapsed);
								if (marker != null && marker.Visual != null)
								{
									marker.Visual.Visibility = (((bool)newValue) ? Visibility.Visible : Visibility.Collapsed);
								}
								if (dataPoint.DpInfo.LabelVisual != null)
								{
									dataPoint.DpInfo.LabelVisual.Visibility = (((bool)newValue) ? Visibility.Visible : Visibility.Collapsed);
									return;
								}
								return;
							}
							else
							{
								if (marker != null && marker.Visual != null)
								{
									marker.Visual.Visibility = (((bool)newValue) ? Visibility.Visible : Visibility.Collapsed);
								}
								if (dataPoint.DpInfo.LabelVisual != null)
								{
									dataPoint.DpInfo.LabelVisual.Visibility = (((bool)newValue) ? Visibility.Visible : Visibility.Collapsed);
									goto IL_C4B;
								}
								goto IL_C4B;
							}
							break;
						default:
							return;
						}
						break;
					}
				}
				else
				{
					switch (property)
					{
					case VcProperties.Href:
						if (!dataPoint.IsLightDataPoint)
						{
							(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
						}
						DataPointHelper.SetHref2DataPointVisualFaces(dataPoint);
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
						if (plotDetails.ChartOrientation == ChartOrientationType.Vertical)
						{
							ColumnChart.CreateOrUpdateMarker4VerticalChart(dataPoint, labelCanvas, new Size(canvas.Width, canvas.Height), (double)canvas.GetValue(Canvas.LeftProperty), (double)canvas.GetValue(Canvas.TopProperty), dataPoint.DpInfo.InternalYValue);
							return;
						}
						BarChart.CreateOrUpdateMarker4HorizontalChart(canvas.Width, chart, labelCanvas, dataPoint, (double)canvas.GetValue(Canvas.LeftProperty), (double)canvas.GetValue(Canvas.TopProperty), dataPoint.DpInfo.InternalYValue >= 0.0, chart.ChartArea.PLANK_DEPTH / (double)plotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0), dataPoint.DpInfo.InternalYValue);
						return;
					case VcProperties.LabelEnabled:
						if (plotDetails.ChartOrientation == ChartOrientationType.Vertical)
						{
							ColumnChart.CreateOrUpdateMarker4VerticalChart(dataPoint, labelCanvas, new Size(canvas.Width, canvas.Height), (double)canvas.GetValue(Canvas.LeftProperty), (double)canvas.GetValue(Canvas.TopProperty), dataPoint.DpInfo.InternalYValue);
							return;
						}
						BarChart.CreateOrUpdateMarker4HorizontalChart(canvas.Width, chart, labelCanvas, dataPoint, (double)canvas.GetValue(Canvas.LeftProperty), (double)canvas.GetValue(Canvas.TopProperty), dataPoint.DpInfo.InternalYValue >= 0.0, chart.ChartArea.PLANK_DEPTH / (double)plotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0), dataPoint.DpInfo.InternalYValue);
						return;
					case VcProperties.LabelFontColor:
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && dataPoint.DpInfo.LabelVisual != null && ((dataPoint.DpInfo.LabelVisual as Border).Child as Canvas).Children[0] != null)
						{
							(((dataPoint.DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Foreground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
							return;
						}
						return;
					case VcProperties.LabelFontFamily:
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && dataPoint.DpInfo.LabelVisual != null && ((dataPoint.DpInfo.LabelVisual as Border).Child as Canvas).Children[0] != null)
						{
							(((dataPoint.DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).FontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
							return;
						}
						return;
					case VcProperties.LabelFontSize:
						if (plotDetails.ChartOrientation == ChartOrientationType.Vertical)
						{
							ColumnChart.CreateOrUpdateMarker4VerticalChart(dataPoint, labelCanvas, new Size(canvas.Width, canvas.Height), (double)canvas.GetValue(Canvas.LeftProperty), (double)canvas.GetValue(Canvas.TopProperty), dataPoint.DpInfo.InternalYValue);
							return;
						}
						BarChart.CreateOrUpdateMarker4HorizontalChart(canvas.Width, chart, labelCanvas, dataPoint, (double)canvas.GetValue(Canvas.LeftProperty), (double)canvas.GetValue(Canvas.TopProperty), dataPoint.DpInfo.InternalYValue >= 0.0, chart.ChartArea.PLANK_DEPTH / (double)plotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0), dataPoint.DpInfo.InternalYValue);
						return;
					case VcProperties.LabelFontStyle:
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && dataPoint.DpInfo.LabelVisual != null && ((dataPoint.DpInfo.LabelVisual as Border).Child as Canvas).Children[0] != null)
						{
							(((dataPoint.DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).FontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
							return;
						}
						return;
					case VcProperties.LabelFontWeight:
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && dataPoint.DpInfo.LabelVisual != null && ((dataPoint.DpInfo.LabelVisual as Border).Child as Canvas).Children[0] != null)
						{
							(((dataPoint.DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).FontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
							return;
						}
						return;
					default:
						switch (property)
						{
						case VcProperties.LabelStyle:
							if (plotDetails.ChartOrientation == ChartOrientationType.Vertical)
							{
								ColumnChart.CreateOrUpdateMarker4VerticalChart(dataPoint, labelCanvas, new Size(canvas.Width, canvas.Height), (double)canvas.GetValue(Canvas.LeftProperty), (double)canvas.GetValue(Canvas.TopProperty), dataPoint.DpInfo.InternalYValue);
								return;
							}
							BarChart.CreateOrUpdateMarker4HorizontalChart(canvas.Width, chart, labelCanvas, dataPoint, (double)canvas.GetValue(Canvas.LeftProperty), (double)canvas.GetValue(Canvas.TopProperty), dataPoint.DpInfo.InternalYValue >= 0.0, chart.ChartArea.PLANK_DEPTH / (double)plotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0), dataPoint.DpInfo.InternalYValue);
							return;
						case VcProperties.LabelText:
							ColumnChart.CreateOrUpdateMarker(chart, dataPoint, labelCanvas, canvas);
							return;
						case VcProperties.LabelAngle:
							if (plotDetails.ChartOrientation == ChartOrientationType.Vertical)
							{
								ColumnChart.CreateOrUpdateMarker4VerticalChart(dataPoint, labelCanvas, new Size(canvas.Width, canvas.Height), (double)canvas.GetValue(Canvas.LeftProperty), (double)canvas.GetValue(Canvas.TopProperty), dataPoint.DpInfo.InternalYValue);
								return;
							}
							BarChart.CreateOrUpdateMarker4HorizontalChart(canvas.Width, chart, labelCanvas, dataPoint, (double)canvas.GetValue(Canvas.LeftProperty), (double)canvas.GetValue(Canvas.TopProperty), dataPoint.DpInfo.InternalYValue >= 0.0, chart.ChartArea.PLANK_DEPTH / (double)plotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0), dataPoint.DpInfo.InternalYValue);
							return;
						case VcProperties.Legend:
						case VcProperties.LegendMarkerColor:
						case VcProperties.LegendMarkerType:
							return;
						case VcProperties.LegendText:
							chart.InvokeRender();
							return;
						case VcProperties.LightingEnabled:
							ColumnChart.ApplyRemoveLighting(dataPoint);
							return;
						default:
							return;
						}
						break;
					}
				}
			}
			else if (property <= VcProperties.RadiusY)
			{
				switch (property)
				{
				case VcProperties.MarkerBorderColor:
					if (marker == null)
					{
						ColumnChart.CreateOrUpdateMarker(chart, dataPoint, labelCanvas, canvas);
						return;
					}
					if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
					{
						marker.BorderColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor);
						return;
					}
					return;
				case VcProperties.MarkerBorderThickness:
					if (marker == null)
					{
						ColumnChart.CreateOrUpdateMarker(chart, dataPoint, labelCanvas, canvas);
						return;
					}
					if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
					{
						marker.BorderThickness = ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderThickness)).Left;
						return;
					}
					return;
				case VcProperties.MarkerColor:
					if (marker == null)
					{
						ColumnChart.CreateOrUpdateMarker(chart, dataPoint, labelCanvas, canvas);
						return;
					}
					if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
					{
						marker.MarkerFillColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerColor);
						return;
					}
					return;
				case VcProperties.MarkerEnabled:
					if (marker == null)
					{
						ColumnChart.CreateOrUpdateMarker(chart, dataPoint, labelCanvas, canvas);
						return;
					}
					if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
					{
						LineChart.ShowDataPointMarker(dataPoint);
						return;
					}
					LineChart.HideDataPointMarker(dataPoint);
					return;
				case VcProperties.MarkerScale:
				case VcProperties.MarkerSize:
				case VcProperties.MarkerType:
					ColumnChart.CreateOrUpdateMarker(chart, dataPoint, labelCanvas, canvas);
					return;
				case VcProperties.MaxWidth:
				case VcProperties.MaxHeight:
				case VcProperties.MinWidth:
				case VcProperties.MinHeight:
				case VcProperties.MinPointHeight:
				case VcProperties.MouseEvent:
				case VcProperties.MovingMarkerEnabled:
					return;
				case VcProperties.Opacity:
					if (marker != null)
					{
						marker.Visual.Opacity = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity) * parent.Opacity;
					}
					if (dataPoint.DpInfo.Faces.Visual != null)
					{
						dataPoint.DpInfo.Faces.Visual.Opacity = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity) * parent.Opacity;
						return;
					}
					return;
				default:
					switch (property)
					{
					case VcProperties.RadiusX:
					case VcProperties.RadiusY:
						break;
					default:
						return;
					}
					break;
				}
			}
			else
			{
				switch (property)
				{
				case VcProperties.ShadowEnabled:
					if (!VisifireControl.IsMediaEffectsEnabled)
					{
						ColumnChart.ApplyOrRemoveShadow4XBAP(dataPoint, parent.RenderAs == RenderAs.StackedColumn || parent.RenderAs == RenderAs.StackedColumn100 || parent.RenderAs == RenderAs.StackedBar || parent.RenderAs == RenderAs.StackedBar100, false);
						return;
					}
					if (parent.RenderAs == RenderAs.Column || parent.RenderAs == RenderAs.StackedColumn || parent.RenderAs == RenderAs.StackedColumn100)
					{
						ColumnChart.ApplyOrRemoveShadow(chart, dataPoint);
						return;
					}
					BarChart.ApplyOrRemoveShadow(chart, dataPoint);
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
						ColumnChart.UpdateDataSeries(parent, property, newValue);
						return;
					case VcProperties.XValueFormatString:
					case VcProperties.YValueFormatString:
						dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
						if (!dataPoint.IsLightDataPoint)
						{
							(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
						}
						ColumnChart.CreateOrUpdateMarker(chart, dataPoint, labelCanvas, canvas);
						return;
					case VcProperties.XValueType:
						chart.InvokeRender();
						return;
					case VcProperties.YValue:
					case VcProperties.YValues:
						goto IL_C4B;
					default:
						return;
					}
					break;
				}
			}
			ColumnChart.Update2DAnd3DColumnBorderColor(dataPoint, chart.View3D);
			ColumnChart.ApplyOrRemoveBevel(dataPoint);
			return;
			IL_C4B:
			if (isAxisChanged)
			{
				ColumnChart.UpdateDataSeries(parent, property, newValue);
			}
			else if (parent.RenderAs == RenderAs.Column || parent.RenderAs == RenderAs.Bar)
			{
				chart.Dispatcher.BeginInvoke(new Action<Chart, IDataPoint, bool>(ColumnChart.UpdateVisualForYValue4ColumnChart), new object[]
				{
					chart,
					dataPoint,
					isAxisChanged
				});
			}
			else if (plotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (chart.AnimatedUpdate.Value)
				{
					chart.Dispatcher.BeginInvoke(new Action<RenderAs, Chart, IDataPoint, bool>(ColumnChart.UpdateVisualForYValue4StackedColumnChart), new object[]
					{
						parent.RenderAs,
						chart,
						dataPoint,
						isAxisChanged
					});
				}
				else
				{
					ColumnChart.UpdateVisualForYValue4StackedColumnChart(parent.RenderAs, chart, dataPoint, isAxisChanged);
				}
			}
			else if (chart.AnimatedUpdate.Value)
			{
				chart.Dispatcher.BeginInvoke(new Action<RenderAs, Chart, IDataPoint, bool>(ColumnChart.UpdateVisualForYValue4StackedBarChart), new object[]
				{
					parent.RenderAs,
					chart,
					dataPoint,
					isAxisChanged
				});
			}
			else
			{
				ColumnChart.UpdateVisualForYValue4StackedBarChart(parent.RenderAs, chart, dataPoint, isAxisChanged);
			}
			chart._toolTip.Hide();
		}

		internal static Canvas CreateOrUpdatePlank(Chart chart, Axis axis, Canvas columnCanvas, double depth3d, Orientation orientation)
		{
			Canvas canvas = columnCanvas.Tag as Canvas;
			double num = 0.0;
			double num2 = 0.0;
			if (chart.View3D && axis.InternalAxisMinimum < 0.0 && axis.InternalAxisMaximum > 0.0)
			{
				if (orientation == Orientation.Horizontal)
				{
					num = columnCanvas.Height - Graphics.ValueToPixelPosition(0.0, columnCanvas.Height, axis.InternalAxisMinimum, axis.InternalAxisMaximum, 0.0);
					if (canvas != null && (double)canvas.GetValue(Canvas.TopProperty) == num && columnCanvas.Children.Contains(canvas))
					{
						return canvas;
					}
				}
				else
				{
					num2 = Graphics.ValueToPixelPosition(0.0, columnCanvas.Width, axis.InternalAxisMinimum, axis.InternalAxisMaximum, 0.0);
					if (canvas != null && (double)canvas.GetValue(Canvas.LeftProperty) == num2 && columnCanvas.Children.Contains(canvas))
					{
						return canvas;
					}
				}
				if (canvas != null)
				{
					columnCanvas.Children.Remove(canvas);
					columnCanvas.Tag = null;
				}
				Brush frontBrush;
				Brush topBrush;
				Brush rightBrush;
				ExtendedGraphics.GetBrushesForPlank(chart, out frontBrush, out topBrush, out rightBrush, true);
				if (orientation == Orientation.Horizontal)
				{
					Faces faces = ColumnChart.Get3DPlank(columnCanvas.Width, 1.4, depth3d, frontBrush, topBrush, rightBrush);
					canvas = (faces.Visual as Canvas);
					canvas.SetValue(Panel.ZIndexProperty, 0);
					canvas.Opacity = 0.7;
					canvas.IsHitTestVisible = false;
					columnCanvas.Children.Add(canvas);
					columnCanvas.Tag = canvas;
				}
				else if (orientation == Orientation.Vertical)
				{
					Faces faces2 = ColumnChart.Get3DPlank(1.0, columnCanvas.Height, depth3d, frontBrush, topBrush, rightBrush);
					canvas = (faces2.Visual as Canvas);
					canvas.SetValue(Panel.ZIndexProperty, 0);
					canvas.Opacity = 0.7;
					canvas.IsHitTestVisible = false;
					columnCanvas.Children.Add(canvas);
					columnCanvas.Tag = canvas;
				}
				canvas.SetValue(Canvas.LeftProperty, num2);
				canvas.SetValue(Canvas.TopProperty, num);
			}
			else if (canvas != null)
			{
				columnCanvas.Children.Remove(canvas);
				canvas = null;
			}
			return canvas;
		}

		public static void UpdateVisualForYValue4ColumnChart(Chart chart, IDataPoint dataPoint, bool isAxisChanged)
		{
			DataSeries parent = dataPoint.Parent;
			double num = chart.ChartArea.PLANK_DEPTH / (double)chart.PlotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
			if (chart != null && !chart._internalPartialUpdateEnabled)
			{
				return;
			}
			Canvas canvas;
			Canvas canvas2;
			if (dataPoint.DpInfo.Faces == null)
			{
				if (parent == null || parent.Faces == null)
				{
					ColumnChart.UpdateDataSeries(parent, VcProperties.YValue, null);
					return;
				}
				canvas = parent.Faces.LabelCanvas;
				canvas2 = (parent.Faces.Visual as Canvas);
				if (dataPoint.Parent.RenderAs == RenderAs.Column)
				{
					double num2 = ColumnChart.CalculateWidthOfEachColumn(chart, chart.ChartArea.ChartVisualCanvas.Width, parent.PlotGroup.AxisX, RenderAs.Column, Orientation.Horizontal);
					ColumnChart.CreateColumnDataPointVisual(canvas2, canvas, chart.PlotDetails, dataPoint, true, num2, num, false);
				}
				else
				{
					double num2 = ColumnChart.CalculateWidthOfEachColumn(chart, chart.ChartArea.ChartVisualCanvas.Height, parent.PlotGroup.AxisX, RenderAs.Bar, Orientation.Vertical);
					BarChart.CreateBarDataPointVisual(dataPoint, canvas, canvas2, true, num2, num, false);
				}
			}
			Canvas canvas3 = dataPoint.DpInfo.Faces.Visual as Canvas;
			canvas2 = (canvas3.Parent as Canvas);
			Canvas labelCanvas = parent.Faces.LabelCanvas;
			double internalYValue = dataPoint.DpInfo.InternalYValue;
			if (double.IsNaN(internalYValue))
			{
				if (canvas2 != null)
				{
					canvas2.Children.Remove(canvas3);
				}
				dataPoint.DpInfo.Faces = null;
				ColumnChart.CleanUpMarkerAndLabel(dataPoint, labelCanvas);
				return;
			}
			bool isPositive = internalYValue >= 0.0;
			double num3 = double.NaN;
			double num4 = double.NaN;
			double num5 = double.NaN;
			double num6 = double.NaN;
			RenderAs renderAs = dataPoint.Parent.RenderAs;
			if (dataPoint.DpInfo.Storyboard != null)
			{
				dataPoint.DpInfo.Storyboard.Pause();
			}
			if (dataPoint.DpInfo.Marker != null && dataPoint.DpInfo.Marker.Visual != null)
			{
				if (renderAs == RenderAs.Column)
				{
					num3 = (double)dataPoint.DpInfo.Marker.Visual.GetValue(Canvas.TopProperty);
				}
				else
				{
					num3 = (double)dataPoint.DpInfo.Marker.Visual.GetValue(Canvas.LeftProperty);
				}
			}
			if (dataPoint.DpInfo.LabelVisual != null)
			{
				if (renderAs == RenderAs.Column)
				{
					num5 = (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.TopProperty);
				}
				else
				{
					num5 = (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
				}
			}
			if (dataPoint.Parent.RenderAs == RenderAs.Column)
			{
				double num7 = (double)canvas3.GetValue(Canvas.TopProperty);
				double num8 = canvas3.Height;
			}
			else
			{
				double num7 = (double)canvas3.GetValue(Canvas.LeftProperty);
				double num8 = canvas3.Width;
			}
			if (canvas2 == null || canvas2.Parent == null || double.IsNaN(internalYValue))
			{
				return;
			}
			canvas = ((canvas2.Parent as Canvas).Children[0] as Canvas);
			RenderHelper.UpdateParentVisualCanvasSize(chart, canvas2);
			RenderHelper.UpdateParentVisualCanvasSize(chart, canvas);
			double num9;
			if (renderAs == RenderAs.Column)
			{
				num9 = ColumnChart.CalculateWidthOfEachColumn(chart, canvas2.Width, parent.PlotGroup.AxisX, RenderAs.Column, Orientation.Horizontal);
			}
			else
			{
				num9 = ColumnChart.CalculateWidthOfEachColumn(chart, canvas2.Height, parent.PlotGroup.AxisX, RenderAs.Bar, Orientation.Vertical);
			}
			if (renderAs == RenderAs.Column)
			{
				ColumnChart.CreateColumnDataPointVisual(canvas2, canvas, chart.PlotDetails, dataPoint, isPositive, num9, num, false);
			}
			else
			{
				BarChart.CreateBarDataPointVisual(dataPoint, canvas, canvas2, isPositive, num9, num, false);
			}
			canvas2.Children.Remove(canvas3);
			ColumnChart.CreateOrUpdatePlank(chart, parent.PlotGroup.AxisY, canvas2, num, (dataPoint.Parent.RenderAs == RenderAs.Column) ? Orientation.Horizontal : Orientation.Vertical);
			if (parent.ToolTipElement != null)
			{
				parent.ToolTipElement.Hide();
			}
			chart.ChartArea.DisableIndicators();
			bool value = chart.AnimatedUpdate.Value;
			if (value && !parent._isZooming)
			{
				if (dataPoint.DpInfo.Storyboard != null)
				{
					dataPoint.DpInfo.Storyboard.Stop();
					dataPoint.DpInfo.Storyboard.Children.Clear();
				}
				double num10 = (renderAs == RenderAs.Column) ? canvas2.Height : canvas2.Width;
				double value2 = 0.0;
				PlotGroup plotGroup = parent.PlotGroup;
				if (plotGroup.AxisY.InternalAxisMinimum > 0.0)
				{
					value2 = plotGroup.AxisY.InternalAxisMinimum;
				}
				if (plotGroup.AxisY.InternalAxisMaximum < 0.0)
				{
					value2 = 0.0;
				}
				double arg_499_0 = plotGroup.AxisY.InternalAxisMaximum;
				double valueMin = (isAxisChanged || double.IsNaN(plotGroup.AxisY._oldInternalAxisMinimum)) ? plotGroup.AxisY.InternalAxisMinimum : plotGroup.AxisY._oldInternalAxisMinimum;
				double valueMax = (isAxisChanged || double.IsNaN(plotGroup.AxisY._oldInternalAxisMaximum)) ? plotGroup.AxisY.InternalAxisMaximum : plotGroup.AxisY._oldInternalAxisMaximum;
				double num11 = Graphics.ValueToPixelPosition(num10, 0.0, valueMin, valueMax, value2);
				double num7;
				double num8;
				if (dataPoint.DpInfo.OldYValue >= 0.0)
				{
					if (renderAs == RenderAs.Column)
					{
						num7 = Graphics.ValueToPixelPosition(num10, 0.0, valueMin, valueMax, dataPoint.DpInfo.OldYValue);
						num11 = Graphics.ValueToPixelPosition(num10, 0.0, valueMin, valueMax, value2);
					}
					else
					{
						num7 = Graphics.ValueToPixelPosition(0.0, num10, valueMin, valueMax, dataPoint.DpInfo.OldYValue);
						num11 = Graphics.ValueToPixelPosition(0.0, num10, valueMin, valueMax, value2);
					}
					num8 = Math.Abs(num7 - num11);
				}
				else
				{
					if (renderAs == RenderAs.Column)
					{
						num11 = Graphics.ValueToPixelPosition(num10, 0.0, valueMin, valueMax, dataPoint.DpInfo.OldYValue);
						num7 = Graphics.ValueToPixelPosition(num10, 0.0, valueMin, valueMax, value2);
					}
					else
					{
						num7 = Graphics.ValueToPixelPosition(0.0, num10, valueMin, valueMax, dataPoint.DpInfo.OldYValue);
						num11 = Graphics.ValueToPixelPosition(0.0, num10, valueMin, valueMax, value2);
					}
					num8 = Math.Abs(num7 - num11);
				}
				double num12 = num8 / ((dataPoint.Parent.RenderAs == RenderAs.Column) ? dataPoint.DpInfo.Faces.Visual.Height : dataPoint.DpInfo.Faces.Visual.Width);
				if (double.IsInfinity(num12))
				{
					num12 = 0.0;
					if (dataPoint.DpInfo.Marker != null && dataPoint.DpInfo.Marker.Visual != null)
					{
						num3 = num11;
					}
				}
				if (double.IsNaN(num12))
				{
					num12 = 1.0;
				}
				Storyboard storyboard;
				if (dataPoint.DpInfo.Storyboard != null)
				{
					storyboard = dataPoint.DpInfo.Storyboard;
				}
				else
				{
					storyboard = new Storyboard();
				}
				if (!double.IsNaN(num3))
				{
					if (dataPoint.Parent.RenderAs == RenderAs.Column)
					{
						num4 = (double)dataPoint.DpInfo.Marker.Visual.GetValue(Canvas.TopProperty);
					}
					else
					{
						num4 = (double)dataPoint.DpInfo.Marker.Visual.GetValue(Canvas.LeftProperty);
					}
				}
				if (!double.IsNaN(num5))
				{
					if (dataPoint.Parent.RenderAs == RenderAs.Column)
					{
						num6 = (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.TopProperty);
					}
					else
					{
						num6 = (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
					}
				}
				double num13;
				if (dataPoint.Parent.RenderAs == RenderAs.Column)
				{
					num13 = (double)dataPoint.DpInfo.Faces.Visual.GetValue(Canvas.TopProperty);
				}
				else
				{
					num13 = (double)dataPoint.DpInfo.Faces.Visual.GetValue(Canvas.LeftProperty);
				}
				string property = (dataPoint.Parent.RenderAs == RenderAs.Column) ? "(Canvas.Top)" : "(Canvas.Left)";
				string property2 = (dataPoint.Parent.RenderAs == RenderAs.Column) ? "Height" : "Width";
				if (chart.View3D)
				{
					Rectangle rectangle;
					Rectangle objectToAnimate;
					Rectangle rectangle2;
					if (renderAs == RenderAs.Column)
					{
						rectangle = (dataPoint.DpInfo.Faces.VisualComponents[0] as Rectangle);
						objectToAnimate = (dataPoint.DpInfo.Faces.VisualComponents[1] as Rectangle);
						rectangle2 = (dataPoint.DpInfo.Faces.VisualComponents[2] as Rectangle);
					}
					else
					{
						rectangle = (dataPoint.DpInfo.Faces.VisualComponents[0] as Rectangle);
						objectToAnimate = (dataPoint.DpInfo.Faces.VisualComponents[2] as Rectangle);
						rectangle2 = (dataPoint.DpInfo.Faces.VisualComponents[1] as Rectangle);
					}
					if ((dataPoint.DpInfo.OldYValue > 0.0 && internalYValue > 0.0) || (dataPoint.DpInfo.OldYValue < 0.0 && internalYValue < 0.0))
					{
						if (renderAs == RenderAs.Column)
						{
							storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Faces.Visual, property, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								1.5
							}, new double[]
							{
								num7,
								num13
							}, AnimationHelper.GenerateKeySplineList(new Point[]
							{
								new Point(0.0, 0.0),
								new Point(1.0, 1.0),
								new Point(0.0, 1.0),
								new Point(0.5, 1.0)
							}));
						}
						else
						{
							if (dataPoint.DpInfo.OldYValue < 0.0 && internalYValue < 0.0)
							{
								storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Faces.Visual, property, dataPoint, storyboard, 0.0, new double[]
								{
									0.0,
									1.5
								}, new double[]
								{
									num7,
									num13
								}, AnimationHelper.GenerateKeySplineList(new Point[]
								{
									new Point(0.0, 0.0),
									new Point(1.0, 1.0),
									new Point(0.0, 1.0),
									new Point(0.5, 1.0)
								}));
							}
							storyboard = AnimationHelper.ApplyPropertyAnimation(objectToAnimate, property, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								1.5
							}, new double[]
							{
								num8,
								rectangle.Width
							}, AnimationHelper.GenerateKeySplineList(new Point[]
							{
								new Point(0.0, 0.0),
								new Point(1.0, 1.0),
								new Point(0.0, 1.0),
								new Point(0.5, 1.0)
							}));
						}
						storyboard = AnimationHelper.ApplyPropertyAnimation(rectangle, property2, dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.5
						}, new double[]
						{
							num8,
							(renderAs == RenderAs.Column) ? dataPoint.DpInfo.Faces.Visual.Height : dataPoint.DpInfo.Faces.Visual.Width
						}, AnimationHelper.GenerateKeySplineList(new Point[]
						{
							new Point(0.0, 0.0),
							new Point(1.0, 1.0),
							new Point(0.0, 1.0),
							new Point(0.5, 1.0)
						}));
						storyboard = AnimationHelper.ApplyPropertyAnimation(rectangle2, property2, dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.5
						}, new double[]
						{
							num8,
							(renderAs == RenderAs.Column) ? rectangle2.Height : rectangle2.Width
						}, AnimationHelper.GenerateKeySplineList(new Point[]
						{
							new Point(0.0, 0.0),
							new Point(1.0, 1.0),
							new Point(0.0, 1.0),
							new Point(0.5, 1.0)
						}));
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled) && !double.IsNaN(num3))
						{
							storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Marker.Visual, property, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								1.5
							}, new double[]
							{
								num3,
								num4
							}, AnimationHelper.GenerateKeySplineList(new Point[]
							{
								new Point(0.0, 0.0),
								new Point(1.0, 1.0),
								new Point(0.0, 1.0),
								new Point(0.5, 1.0)
							}));
							if (renderAs == RenderAs.Column)
							{
								dataPoint.DpInfo.Marker.Visual.SetValue(Canvas.TopProperty, num3);
							}
							else
							{
								dataPoint.DpInfo.Marker.Visual.SetValue(Canvas.LeftProperty, num3);
							}
						}
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && !double.IsNaN(num5))
						{
							storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.LabelVisual, property, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								1.5
							}, new double[]
							{
								num5,
								num6
							}, AnimationHelper.GenerateKeySplineList(new Point[]
							{
								new Point(0.0, 0.0),
								new Point(1.0, 1.0),
								new Point(0.0, 1.0),
								new Point(0.5, 1.0)
							}));
							if (renderAs == RenderAs.Column)
							{
								dataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, num5);
							}
							else
							{
								dataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, num5);
							}
						}
					}
					else
					{
						if (dataPoint.DpInfo.OldYValue >= 0.0 && internalYValue < 0.0)
						{
							if (renderAs == RenderAs.Column)
							{
								storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Faces.Visual, property, dataPoint, storyboard, 0.0, new double[]
								{
									0.0,
									0.75,
									1.5
								}, new double[]
								{
									num7,
									num11,
									num13
								}, AnimationHelper.GenerateKeySplineList(new Point[]
								{
									new Point(0.0, 0.0),
									new Point(1.0, 1.0),
									new Point(0.0, 0.5),
									new Point(0.5, 0.5),
									new Point(0.0, 1.0),
									new Point(0.5, 1.0)
								}));
							}
							else
							{
								storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Faces.Visual, property, dataPoint, storyboard, 0.0, new double[]
								{
									0.0,
									0.75,
									1.5
								}, new double[]
								{
									num11,
									num11,
									num13
								}, AnimationHelper.GenerateKeySplineList(new Point[]
								{
									new Point(0.0, 0.0),
									new Point(1.0, 1.0),
									new Point(0.0, 0.5),
									new Point(0.5, 0.5),
									new Point(0.0, 1.0),
									new Point(0.5, 1.0)
								}));
								storyboard = AnimationHelper.ApplyPropertyAnimation(objectToAnimate, property, dataPoint, storyboard, 0.0, new double[]
								{
									0.0,
									0.75,
									1.5
								}, new double[]
								{
									num8,
									0.0,
									dataPoint.DpInfo.Faces.Visual.Width
								}, AnimationHelper.GenerateKeySplineList(new Point[]
								{
									new Point(0.0, 0.0),
									new Point(1.0, 1.0),
									new Point(0.0, 0.5),
									new Point(0.5, 0.5),
									new Point(0.0, 1.0),
									new Point(0.5, 1.0)
								}));
							}
							storyboard = AnimationHelper.ApplyPropertyAnimation(rectangle, property2, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								0.75,
								1.5
							}, new double[]
							{
								num8,
								0.0,
								(renderAs == RenderAs.Column) ? dataPoint.DpInfo.Faces.Visual.Height : dataPoint.DpInfo.Faces.Visual.Width
							}, AnimationHelper.GenerateKeySplineList(new Point[]
							{
								new Point(0.0, 0.0),
								new Point(1.0, 1.0),
								new Point(0.0, 0.5),
								new Point(0.5, 0.5),
								new Point(0.0, 1.0),
								new Point(0.5, 1.0)
							}));
							storyboard = AnimationHelper.ApplyPropertyAnimation(rectangle2, property2, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								0.75,
								1.5
							}, new double[]
							{
								num8,
								0.0,
								(renderAs == RenderAs.Column) ? rectangle2.Height : rectangle2.Width
							}, AnimationHelper.GenerateKeySplineList(new Point[]
							{
								new Point(0.0, 0.0),
								new Point(1.0, 1.0),
								new Point(0.0, 0.5),
								new Point(0.5, 0.5),
								new Point(0.0, 1.0),
								new Point(0.5, 1.0)
							}));
						}
						else if (dataPoint.DpInfo.OldYValue <= 0.0 && internalYValue >= 0.0)
						{
							if (renderAs == RenderAs.Column)
							{
								storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Faces.Visual, property, dataPoint, storyboard, 0.0, new double[]
								{
									0.0,
									0.75,
									1.5
								}, new double[]
								{
									num7,
									num7,
									num13
								}, AnimationHelper.GenerateKeySplineList(new Point[]
								{
									new Point(0.0, 0.0),
									new Point(1.0, 1.0),
									new Point(0.0, 0.5),
									new Point(0.5, 0.5),
									new Point(0.0, 1.0),
									new Point(0.5, 1.0)
								}));
							}
							else
							{
								storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Faces.Visual, property, dataPoint, storyboard, 0.0, new double[]
								{
									0.0,
									0.75,
									1.5
								}, new double[]
								{
									num7,
									num11,
									num13
								}, AnimationHelper.GenerateKeySplineList(new Point[]
								{
									new Point(0.0, 0.0),
									new Point(1.0, 1.0),
									new Point(0.0, 0.5),
									new Point(0.5, 0.5),
									new Point(0.0, 1.0),
									new Point(0.5, 1.0)
								}));
								storyboard = AnimationHelper.ApplyPropertyAnimation(objectToAnimate, property, dataPoint, storyboard, 0.0, new double[]
								{
									0.0,
									0.75,
									1.5
								}, new double[]
								{
									num8,
									0.0,
									dataPoint.DpInfo.Faces.Visual.Width
								}, AnimationHelper.GenerateKeySplineList(new Point[]
								{
									new Point(0.0, 0.0),
									new Point(1.0, 1.0),
									new Point(0.0, 0.5),
									new Point(0.5, 0.5),
									new Point(0.0, 1.0),
									new Point(0.5, 1.0)
								}));
							}
							storyboard = AnimationHelper.ApplyPropertyAnimation(rectangle, property2, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								0.75,
								1.5
							}, new double[]
							{
								num8,
								0.0,
								(renderAs == RenderAs.Column) ? dataPoint.DpInfo.Faces.Visual.Height : dataPoint.DpInfo.Faces.Visual.Width
							}, AnimationHelper.GenerateKeySplineList(new Point[]
							{
								new Point(0.0, 0.0),
								new Point(1.0, 1.0),
								new Point(0.0, 0.5),
								new Point(0.5, 0.5),
								new Point(0.0, 1.0),
								new Point(0.5, 1.0)
							}));
							storyboard = AnimationHelper.ApplyPropertyAnimation(rectangle2, property2, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								0.75,
								1.5
							}, new double[]
							{
								num8,
								0.0,
								(renderAs == RenderAs.Column) ? rectangle2.Height : rectangle2.Width
							}, AnimationHelper.GenerateKeySplineList(new Point[]
							{
								new Point(0.0, 0.0),
								new Point(1.0, 1.0),
								new Point(0.0, 0.5),
								new Point(0.5, 0.5),
								new Point(0.0, 1.0),
								new Point(0.5, 1.0)
							}));
						}
						if (renderAs == RenderAs.Column)
						{
							num11 = Math.Abs(num10 - Graphics.ValueToPixelPosition(0.0, num10, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, value2));
						}
						else
						{
							num11 = Graphics.ValueToPixelPosition(0.0, num10, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, value2);
						}
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled) && !double.IsNaN(num3))
						{
							storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Marker.Visual, property, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								0.75,
								1.5
							}, new double[]
							{
								num3,
								num11,
								num4
							}, AnimationHelper.GenerateKeySplineList(new Point[]
							{
								new Point(0.0, 0.0),
								new Point(1.0, 1.0),
								new Point(0.0, 0.5),
								new Point(0.5, 0.5),
								new Point(0.0, 1.0),
								new Point(0.5, 1.0)
							}));
							if (renderAs == RenderAs.Column)
							{
								dataPoint.DpInfo.Marker.Visual.SetValue(Canvas.TopProperty, num3);
							}
							else
							{
								dataPoint.DpInfo.Marker.Visual.SetValue(Canvas.LeftProperty, num3);
							}
						}
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && !double.IsNaN(num5))
						{
							storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.LabelVisual, property, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								0.75,
								1.5
							}, new double[]
							{
								num5,
								num11,
								num6
							}, AnimationHelper.GenerateKeySplineList(new Point[]
							{
								new Point(0.0, 0.0),
								new Point(1.0, 1.0),
								new Point(0.0, 0.5),
								new Point(0.5, 0.5),
								new Point(0.0, 1.0),
								new Point(0.5, 1.0)
							}));
							if (renderAs == RenderAs.Column)
							{
								dataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, num5);
							}
							else
							{
								dataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, num5);
							}
						}
					}
				}
				else if ((dataPoint.DpInfo.OldYValue < 0.0 && internalYValue < 0.0) || (dataPoint.DpInfo.OldYValue > 0.0 && internalYValue > 0.0))
				{
					DataSeries parent2 = dataPoint.Parent;
					storyboard = ColumnChart.ApplyColumnChartAnimation(parent2, dataPoint.DpInfo.Faces.Visual as Panel, storyboard, isPositive, 0.0, new double[]
					{
						0.0,
						1.0
					}, new double[]
					{
						num12,
						1.0
					}, dataPoint.Parent.RenderAs);
					if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled) && !double.IsNaN(num3))
					{
						storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Marker.Visual, property, dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.0
						}, new double[]
						{
							num3,
							num4
						}, AnimationHelper.GenerateKeySplineList(new Point[]
						{
							new Point(0.0, 0.0),
							new Point(1.0, 1.0),
							new Point(0.0, 1.0),
							new Point(0.5, 1.0)
						}));
						storyboard = AnimationHelper.ApplyOpacityAnimation(dataPoint.DpInfo.Marker.Visual, dataPoint, storyboard, 0.0, 0.2, 0.98, 1.0);
						if (renderAs == RenderAs.Column)
						{
							dataPoint.DpInfo.Marker.Visual.SetValue(Canvas.TopProperty, num3);
						}
						else
						{
							dataPoint.DpInfo.Marker.Visual.SetValue(Canvas.LeftProperty, num3);
						}
					}
					if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && !double.IsNaN(num5))
					{
						storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.LabelVisual, property, dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.0
						}, new double[]
						{
							num5,
							num6
						}, AnimationHelper.GenerateKeySplineList(new Point[]
						{
							new Point(0.0, 0.0),
							new Point(1.0, 1.0),
							new Point(0.0, 1.0),
							new Point(0.5, 1.0)
						}));
						if (renderAs == RenderAs.Column)
						{
							dataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, num5);
						}
						else
						{
							dataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, num5);
						}
					}
				}
				else
				{
					double num14;
					if (dataPoint.Parent.RenderAs == RenderAs.Column)
					{
						num13 = (double)dataPoint.DpInfo.Faces.Visual.GetValue(Canvas.TopProperty);
						num14 = num10 - Graphics.ValueToPixelPosition(0.0, num10, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, 0.0);
						if (dataPoint.DpInfo.OldYValue <= 0.0)
						{
							storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Faces.Visual, property, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								0.5,
								0.5,
								1.0
							}, new double[]
							{
								num14,
								num14,
								num14,
								num13
							}, null);
							storyboard = ColumnChart.ApplyColumnChartAnimation(dataPoint.Parent, dataPoint.DpInfo.Faces.Visual as Panel, storyboard, false, 0.0, new double[]
							{
								0.0,
								0.5,
								0.5,
								1.0
							}, new double[]
							{
								num12,
								0.0,
								0.0,
								1.0
							}, dataPoint.Parent.RenderAs);
						}
						else
						{
							storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Faces.Visual, property, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								0.5,
								0.5
							}, new double[]
							{
								num7,
								num14,
								num14
							}, null);
							storyboard = ColumnChart.ApplyColumnChartAnimation(dataPoint.Parent, dataPoint.DpInfo.Faces.Visual as Panel, storyboard, false, 0.0, new double[]
							{
								0.0,
								0.5,
								0.5,
								1.0
							}, new double[]
							{
								num12,
								0.0,
								0.0,
								1.0
							}, dataPoint.Parent.RenderAs);
						}
					}
					else
					{
						num13 = (double)dataPoint.DpInfo.Faces.Visual.GetValue(Canvas.LeftProperty);
						num14 = Graphics.ValueToPixelPosition(0.0, num10, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, 0.0);
						if (num14 < 0.0)
						{
							num14 = 0.0;
						}
						if (dataPoint.DpInfo.OldYValue > 0.0)
						{
							storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Faces.Visual, property, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								0.5,
								0.5,
								1.0
							}, new double[]
							{
								num14,
								num14,
								num14,
								num13
							}, null);
							storyboard = ColumnChart.ApplyColumnChartAnimation(dataPoint.Parent, dataPoint.DpInfo.Faces.Visual as Panel, storyboard, true, 0.0, new double[]
							{
								0.0,
								0.5,
								0.5,
								1.0
							}, new double[]
							{
								num12,
								0.0,
								0.0,
								1.0
							}, dataPoint.Parent.RenderAs);
						}
						else
						{
							storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Faces.Visual, property, dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								0.5,
								0.5,
								1.0
							}, new double[]
							{
								num7,
								num14,
								num14
							}, null);
							storyboard = ColumnChart.ApplyColumnChartAnimation(dataPoint.Parent, dataPoint.DpInfo.Faces.Visual as Panel, storyboard, true, 0.0, new double[]
							{
								0.0,
								0.5,
								0.5,
								1.0
							}, new double[]
							{
								num12,
								0.0,
								0.0,
								1.0
							}, dataPoint.Parent.RenderAs);
						}
					}
					if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled) && !double.IsNaN(num3))
					{
						storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Marker.Visual, property, dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							0.5,
							0.5,
							1.0
						}, new double[]
						{
							num3,
							num14,
							num14,
							num4
						}, null);
					}
					if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && !double.IsNaN(num5))
					{
						storyboard = AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.LabelVisual, property, dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							0.5,
							0.5,
							1.0
						}, new double[]
						{
							num5,
							num14,
							num14,
							num6
						}, null);
					}
				}
				dataPoint.DpInfo.Storyboard = storyboard;
				storyboard.Begin(dataPoint.Chart._rootElement, true);
			}
			if (canvas2.Parent != null)
			{
				double width = chart.ChartArea.ChartVisualCanvas.Width;
				double height = chart.ChartArea.ChartVisualCanvas.Height;
				bool flag = true;
				List<IDataPoint> list = (from datapoint in chart.PlotDetails.ListOfAllDataPoints
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
				if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					if (flag)
					{
						rectangleGeometry.Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH - (double)(chart.View3D ? 0 : 5), width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + (double)(chart.View3D ? 0 : 10));
					}
					else
					{
						rectangleGeometry.Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH - (double)(chart.View3D ? 0 : 5), width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH + (double)(chart.View3D ? 0 : 10));
					}
					(canvas2.Parent as Canvas).Clip = rectangleGeometry;
				}
				else
				{
					if (flag)
					{
						rectangleGeometry.Rect = new Rect((double)(-(double)(chart.View3D ? 0 : 5)) - chart.ChartArea.PLANK_THICKNESS, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + (double)(chart.View3D ? 0 : 10), height + chart.ChartArea.PLANK_DEPTH);
					}
					else
					{
						rectangleGeometry.Rect = new Rect((double)(-(double)(chart.View3D ? 0 : 5)), -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH + (double)(chart.View3D ? 0 : 10), height + chart.ChartArea.PLANK_DEPTH);
					}
					(canvas2.Parent as Canvas).Clip = rectangleGeometry;
				}
			}
			dataPoint.Parent.AttachOrDetachInteractivity4DataPoint(dataPoint);
			if (!dataPoint.IsLightDataPoint && dataPoint.Parent.SelectionEnabled && (dataPoint as DataPoint).Selected)
			{
				DataPointHelper.Select(dataPoint, true);
			}
		}

		private static void ApplyRemoveLighting(IDataPoint dataPoint)
		{
			Faces faces = dataPoint.DpInfo.Faces;
			if (faces == null)
			{
				return;
			}
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled);
			bool value = dataPoint.Parent.Bevel.Value;
			if (!dataPoint.DpInfo.OldLightingState == !flag && !value)
			{
				using (List<DependencyObject>.Enumerator enumerator = faces.Parts.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FrameworkElement frameworkElement = (FrameworkElement)enumerator.Current;
						if (frameworkElement.Tag != null && (frameworkElement.Tag as ElementData).VisualElementName == "ColumnBase")
						{
							Brush fill = flag ? Graphics.GetLightingEnabledBrush(dataPoint.Color, "Linear", null) : dataPoint.Color;
							(frameworkElement as Path).Fill = fill;
						}
					}
				}
				return;
			}
			dataPoint.DpInfo.OldLightingState = flag;
			if ((dataPoint.Chart as Chart).View3D)
			{
				ColumnChart.Update2DAnd3DColumnColor(dataPoint, dataPoint.Color);
				return;
			}
			Canvas canvas = faces.Visual as Canvas;
			faces.ClearList(ref faces.LightingElements);
			if (!flag && value)
			{
				Canvas canvas2 = ExtendedGraphics.Get2DRectangleGradiance(canvas.Width, canvas.Height, Graphics.GetLeftGradianceBrush(63), Graphics.GetRightGradianceBrush(63), Orientation.Vertical);
				canvas.Children.Add(canvas2);
				foreach (FrameworkElement item in canvas2.Children)
				{
					dataPoint.DpInfo.Faces.LightingElements.Add(item);
				}
			}
			using (List<DependencyObject>.Enumerator enumerator3 = faces.Parts.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					FrameworkElement frameworkElement2 = (FrameworkElement)enumerator3.Current;
					if (frameworkElement2.Tag != null && (frameworkElement2.Tag as ElementData).VisualElementName == "ColumnBase")
					{
						Brush fill2 = flag ? Graphics.GetLightingEnabledBrush(dataPoint.Color, "Linear", null) : dataPoint.Color;
						(frameworkElement2 as Path).Fill = fill2;
					}
				}
			}
		}

		private static void ApplyOrRemoveBevel(IDataPoint dataPoint)
		{
			bool value = dataPoint.Parent.Bevel.Value;
			Faces faces = dataPoint.DpInfo.Faces;
			if (faces == null)
			{
				throw new Exception("Faces of DataPoint is null. ColumnChart.ApplyBevel()");
			}
			Canvas canvas = faces.Visual as Canvas;
			faces.ClearList(ref faces.BevelElements);
			if (!value)
			{
				return;
			}
			if ((dataPoint.Chart as Chart).View3D)
			{
				return;
			}
			Thickness thickness = (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness);
			if (value && canvas.Height > 7.0 && canvas.Width > 14.0)
			{
				if (dataPoint.Parent.SelectionEnabled && (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness) == new Thickness(0.0))
				{
					thickness = new Thickness(1.0);
				}
				bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled);
				Brush color = dataPoint.Color;
				Canvas canvas2 = ExtendedGraphics.Get2DRectangleBevel(null, canvas.Width - 2.0 * thickness.Left, canvas.Height - 2.0 * thickness.Left, 6.0, 6.0, Graphics.GetBevelTopBrush(color), Graphics.GetBevelSideBrush((double)(flag ? -70 : 0), color), Graphics.GetBevelSideBrush((double)(flag ? -110 : 180), color), null);
				foreach (FrameworkElement item in canvas2.Children)
				{
					dataPoint.DpInfo.Faces.BevelElements.Add(item);
				}
				dataPoint.DpInfo.Faces.BevelElements.Add(canvas2);
				canvas2.SetValue(Canvas.LeftProperty, thickness.Left);
				canvas2.SetValue(Canvas.TopProperty, thickness.Left);
				canvas.Children.Add(canvas2);
			}
		}

		internal static Faces Get2DColumn(IDataPoint dataPoint, double width, double height, bool isStacked, bool isTopOfStack, bool isPositive)
		{
			Faces faces = new Faces();
			dataPoint.DpInfo.Faces = faces;
			Canvas canvas = new Canvas();
			faces.Visual = canvas;
			canvas.Width = width;
			canvas.Height = height;
			if (!dataPoint.Parent.LightWeight.Value)
			{
				DoubleCollection strokeDashArray = null;
				BorderStyles borderStyles = (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle);
				Thickness thickness = (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness);
				Brush stroke = null;
				CornerRadius xRadius = (CornerRadius)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.RadiusX);
				CornerRadius yRadius = (CornerRadius)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.RadiusY);
				if (borderStyles != BorderStyles.Solid)
				{
					strokeDashArray = ExtendedGraphics.GetDashArray(borderStyles);
				}
				Brush fill = null;
				if (thickness.Left != 0.0)
				{
					stroke = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor);
				}
				Path path = ExtendedGraphics.Get2DRectangle(dataPoint, width, height, thickness.Left, strokeDashArray, stroke, fill, xRadius, yRadius, new bool?(!isStacked || isTopOfStack), isPositive);
				(path.Tag as ElementData).VisualElementName = "ColumnBase";
				faces.VisualComponents.Add(path);
				faces.Parts.Add(path);
				faces.BorderElements.Add(path);
				canvas.Children.Add(path);
				ColumnChart.ApplyOrRemoveBevel(dataPoint);
				ColumnChart.ApplyRemoveLighting(dataPoint);
				if (!VisifireControl.IsMediaEffectsEnabled)
				{
					ColumnChart.ApplyOrRemoveShadow4XBAP(dataPoint, isStacked, isTopOfStack);
				}
			}
			else
			{
				canvas.Background = dataPoint.Color;
			}
			return faces;
		}

		internal static void ApplyOrRemoveShadow4XBAP(IDataPoint dataPoint, bool isStacked, bool isTopOfStack)
		{
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled);
			Faces faces = dataPoint.DpInfo.Faces;
			if (faces == null)
			{
				throw new Exception("Faces of DataPoint is null. ColumnChart.ApplyOrRemoveShadow()");
			}
			Canvas canvas = faces.Visual as Canvas;
			faces.ClearList(ref faces.ShadowElements);
			if (flag)
			{
				CornerRadius cornerRadius = (CornerRadius)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.RadiusX);
				CornerRadius cornerRadius2 = (CornerRadius)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.RadiusY);
				double num = 1.0;
				double num2 = 5.0 - num;
				double height = canvas.Height;
				CornerRadius xRadius = cornerRadius;
				CornerRadius yRadius = cornerRadius2;
				Grid grid = ExtendedGraphics.Get2DRectangleShadow(null, canvas.Width, height, xRadius, yRadius, (double)(isStacked ? 3 : 5));
				grid.SetValue(Canvas.TopProperty, num2);
				grid.SetValue(Canvas.LeftProperty, 5.0);
				grid.Opacity = 0.7;
				grid.SetValue(Panel.ZIndexProperty, -1);
				faces.ShadowElements.Add(grid);
				canvas.Children.Add(grid);
			}
		}

		internal static void ApplyEffects(Chart chart, IDataPoint dataPoint)
		{
			Faces faces = dataPoint.DpInfo.Faces;
			if (faces == null)
			{
				throw new Exception("Faces of DataPoint is null. ColumnChart.ApplyEffects()");
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
				throw new Exception("Faces of DataPoint is null. ColumnChart.ApplyOrRemoveShadow()");
			}
			Canvas canvas = faces.Visual as Canvas;
			if (DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect) == null)
			{
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled))
				{
					if (chart.View3D)
					{
						canvas.Effect = ExtendedGraphics.GetShadowEffect(50.0, 5.0, 0.95);
						return;
					}
					canvas.Effect = ExtendedGraphics.GetShadowEffect(330.0, 3.5, 0.95);
					return;
				}
				else
				{
					canvas.Effect = null;
				}
			}
		}

		internal static Faces Get3DColumn(IDataPoint tagRef, double width, double height, double depth, Brush backgroundBrush, Brush frontBrush, Brush topBrush, Brush rightBrush, bool lightingEnabled, BorderStyles borderStyle, Brush borderBrush, double borderThickness)
		{
			Faces faces = new Faces();
			Canvas canvas = new Canvas();
			canvas.Width = width;
			canvas.Height = height;
			if (!tagRef.Parent.LightWeight.Value)
			{
				DoubleCollection strokeDashArray = null;
				if (borderStyle != BorderStyles.Solid)
				{
					strokeDashArray = ExtendedGraphics.GetDashArray(borderStyle);
				}
				if (frontBrush == null)
				{
					frontBrush = (lightingEnabled ? Graphics.GetFrontFaceBrush4Column3D(backgroundBrush) : backgroundBrush);
				}
				if (topBrush == null)
				{
					topBrush = (lightingEnabled ? Graphics.GetTopFaceBrush(backgroundBrush) : Graphics.GetDarkerBrush(backgroundBrush, 0.7665));
				}
				if (rightBrush == null)
				{
					rightBrush = (lightingEnabled ? Graphics.GetRightFaceBrush4Column3D(backgroundBrush) : Graphics.GetDarkerBrush(backgroundBrush, 0.55));
				}
				Shape shape = ExtendedGraphics.Get3DRectangle(tagRef, width, height, borderThickness, strokeDashArray, borderBrush, frontBrush);
				shape.Tag = new ElementData
				{
					VisualElementName = "FrontFace",
					Element = tagRef
				};
				faces.VisualComponents.Add(shape);
				faces.Parts.Add(shape);
				faces.BorderElements.Add(shape);
				Shape shape2 = ExtendedGraphics.Get3DRectangle(tagRef, width, depth, borderThickness, strokeDashArray, borderBrush, topBrush);
				shape2.Tag = new ElementData
				{
					VisualElementName = "TopFace",
					Element = tagRef
				};
				faces.VisualComponents.Add(shape2);
				faces.Parts.Add(shape2);
				faces.BorderElements.Add(shape2);
				shape2.RenderTransformOrigin = new Point(0.0, 1.0);
				shape2.RenderTransform = new SkewTransform
				{
					AngleX = -45.0
				};
				Shape shape3 = ExtendedGraphics.Get3DRectangle(tagRef, depth, height, borderThickness, strokeDashArray, borderBrush, rightBrush);
				shape3.Tag = new ElementData
				{
					VisualElementName = "RightFace",
					Element = tagRef
				};
				faces.VisualComponents.Add(shape3);
				faces.Parts.Add(shape3);
				faces.BorderElements.Add(shape3);
				shape3.RenderTransformOrigin = new Point(0.0, 0.0);
				shape3.RenderTransform = new SkewTransform
				{
					AngleY = -45.0
				};
				canvas.Children.Add(shape);
				canvas.Children.Add(shape2);
				canvas.Children.Add(shape3);
				shape2.SetValue(Canvas.TopProperty, -depth);
				shape3.SetValue(Canvas.LeftProperty, width);
			}
			else
			{
				if (frontBrush == null)
				{
					frontBrush = backgroundBrush;
				}
				if (topBrush == null)
				{
					topBrush = Graphics.GetDarkerBrush(backgroundBrush, 0.7665);
				}
				if (rightBrush == null)
				{
					rightBrush = Graphics.GetDarkerBrush(backgroundBrush, 0.55);
				}
				Rectangle rectangle = new Rectangle
				{
					Width = width,
					Height = height
				};
				rectangle.Fill = frontBrush;
				faces.Parts.Add(rectangle);
				faces.VisualComponents.Add(rectangle);
				Rectangle rectangle2 = new Rectangle
				{
					Width = width,
					Height = depth
				};
				rectangle2.Fill = topBrush;
				faces.Parts.Add(rectangle2);
				faces.VisualComponents.Add(rectangle2);
				rectangle2.RenderTransformOrigin = new Point(0.0, 1.0);
				rectangle2.RenderTransform = new SkewTransform
				{
					AngleX = -45.0
				};
				Rectangle rectangle3 = new Rectangle
				{
					Width = depth,
					Height = height
				};
				rectangle3.Fill = rightBrush;
				faces.Parts.Add(rectangle3);
				faces.VisualComponents.Add(rectangle3);
				rectangle3.RenderTransformOrigin = new Point(0.0, 0.0);
				rectangle3.RenderTransform = new SkewTransform
				{
					AngleY = -45.0
				};
				canvas.Children.Add(rectangle);
				canvas.Children.Add(rectangle2);
				canvas.Children.Add(rectangle3);
				rectangle2.SetValue(Canvas.TopProperty, -depth);
				rectangle3.SetValue(Canvas.LeftProperty, width);
			}
			faces.Visual = canvas;
			return faces;
		}

		internal static void Update3DPlank(double width, double height, double depth3D, Faces plankFaces)
		{
			Rectangle rectangle = plankFaces.VisualComponents[0] as Rectangle;
			Rectangle rectangle2 = plankFaces.VisualComponents[1] as Rectangle;
			Rectangle rectangle3 = plankFaces.VisualComponents[2] as Rectangle;
			rectangle.Width = width;
			rectangle.Height = height;
			rectangle2.Width = width;
			rectangle2.Height = depth3D;
			rectangle3.Width = depth3D;
			rectangle3.Height = height;
			rectangle2.SetValue(Canvas.TopProperty, -depth3D);
			rectangle3.SetValue(Canvas.LeftProperty, width);
		}

		internal static Faces Get3DPlank(double width, double height, double depth3D, Brush frontBrush, Brush topBrush, Brush rightBrush)
		{
			Faces faces = new Faces();
			Canvas canvas = new Canvas();
			canvas.Width = width;
			canvas.Height = height;
			Shape shape = ExtendedGraphics.Get3DRectangle(null, width, height, 0.25, null, null, frontBrush);
			Shape shape2 = ExtendedGraphics.Get3DRectangle(null, width, depth3D, 0.25, null, null, topBrush);
			Shape shape3 = ExtendedGraphics.Get3DRectangle(null, depth3D, height, 0.25, null, null, rightBrush);
			shape2.RenderTransformOrigin = new Point(0.0, 1.0);
			shape2.RenderTransform = new SkewTransform
			{
				AngleX = -45.0
			};
			shape3.RenderTransformOrigin = new Point(0.0, 0.0);
			shape3.RenderTransform = new SkewTransform
			{
				AngleY = -45.0
			};
			canvas.Children.Add(shape);
			canvas.Children.Add(shape2);
			canvas.Children.Add(shape3);
			shape2.SetValue(Canvas.TopProperty, -depth3D);
			shape3.SetValue(Canvas.LeftProperty, width);
			faces.VisualComponents.Add(shape);
			faces.VisualComponents.Add(shape2);
			faces.VisualComponents.Add(shape3);
			faces.Visual = canvas;
			return faces;
		}

		private static void CreateStackedColumnVisual(bool isPositive, Canvas columnCanvas, Canvas labelCanvas, IDataPoint dataPoint, bool isTopOFStack, double PrevSum, double left, ref double top, ref double bottom, double columnWidth, double columnHeight, double depth3d, ref int positiveOrNegativeIndex, int maxOrMinIndex, bool animationEnabled, double animationBeginTime)
		{
			PlotGroup plotGroup = dataPoint.Parent.PlotGroup;
			Chart chart = dataPoint.Chart as Chart;
			dataPoint.Parent.Faces = new Faces
			{
				Visual = columnCanvas,
				LabelCanvas = labelCanvas
			};
			dataPoint.DpInfo.OldLightingState = false;
			double internalYValue = dataPoint.DpInfo.InternalYValue;
			Panel panel;
			if (chart.View3D)
			{
				Brush topBrush = null;
				if ((dataPoint.Parent.RenderAs == RenderAs.StackedColumn && ((!isTopOFStack && PrevSum == plotGroup.AxisY.InternalAxisMaximum) || PrevSum > plotGroup.AxisY.InternalAxisMaximum)) || (dataPoint.Parent.RenderAs == RenderAs.StackedColumn100 && plotGroup.AxisY.AxisMaximum != null && ((!isTopOFStack && PrevSum == plotGroup.AxisY.InternalAxisMaximum) || PrevSum > plotGroup.AxisY.InternalAxisMaximum)))
				{
					topBrush = (((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled)) ? Graphics.GetRightFaceBrush(dataPoint.Color) : Graphics.GetDarkerBrush(dataPoint.Color, 0.55));
				}
				Faces faces = ColumnChart.Get3DColumn(dataPoint, columnWidth, columnHeight, depth3d, dataPoint.Color, null, topBrush, null, (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled), (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle), (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor), ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderThickness)).Left);
				panel = (faces.Visual as Panel);
				panel.SetValue(Panel.ZIndexProperty, ColumnChart.GetStackedColumnZIndex(left, top, internalYValue > 0.0, positiveOrNegativeIndex, maxOrMinIndex));
				dataPoint.DpInfo.Faces = faces;
				if (!dataPoint.Parent.LightWeight.Value && !VisifireControl.IsMediaEffectsEnabled)
				{
					ColumnChart.ApplyOrRemoveShadow4XBAP(dataPoint, true, false);
				}
			}
			else
			{
				Faces faces = ColumnChart.Get2DColumn(dataPoint, columnWidth, columnHeight, true, isTopOFStack, isPositive);
				panel = (faces.Visual as Panel);
				dataPoint.DpInfo.Faces = faces;
			}
			if (!dataPoint.Parent.LightWeight.Value && VisifireControl.IsMediaEffectsEnabled)
			{
				ColumnChart.ApplyEffects(chart, dataPoint);
				ColumnChart.ApplyOrRemoveShadow(chart, dataPoint);
			}
			dataPoint.DpInfo.Faces.LabelCanvas = labelCanvas;
			panel.SetValue(Canvas.LeftProperty, left);
			panel.SetValue(Canvas.TopProperty, top);
			columnCanvas.Children.Add(panel);
			dataPoint.DpInfo.IsTopOfStack = isTopOFStack;
			if (!dataPoint.Parent.LightWeight.Value)
			{
				ColumnChart.CreateOrUpdateMarker4VerticalChart(dataPoint, labelCanvas, new Size(panel.Width, panel.Height), left, top, PrevSum);
			}
			if (animationEnabled)
			{
				DataSeries parent = dataPoint.Parent;
				if (parent.Storyboard == null)
				{
					parent.Storyboard = new Storyboard();
				}
				parent.Storyboard = ColumnChart.ApplyStackedColumnChartAnimation(parent, panel, dataPoint.Parent.Storyboard, animationBeginTime, 0.5);
			}
			if (isPositive)
			{
				bottom = top;
			}
			else
			{
				top = bottom;
			}
			if (isPositive)
			{
				dataPoint.DpInfo.VisualPosition = new Point(left + columnWidth / 2.0, top);
			}
			else
			{
				dataPoint.DpInfo.VisualPosition = new Point(left + columnWidth / 2.0, bottom);
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

		internal static Canvas GetVisualObjectForStackedColumnChart(RenderAs chartType, Panel preExistingPanel, double width, double height, PlotDetails plotDetails, Chart chart, double plankDepth, bool animationEnabled)
		{
			if (double.IsNaN(width) || double.IsNaN(height) || width <= 0.0 || height <= 0.0)
			{
				return null;
			}
			Canvas canvas;
			Canvas canvas2;
			Canvas canvas3;
			RenderHelper.PrepareCanvas4Drawing(preExistingPanel as Canvas, out canvas, out canvas2, out canvas3, width, height);
			bool flag = true;
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
			double num = plankDepth / (double)plotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
			double num2 = num * (double)(plotDetails.SeriesDrawingIndex[list[0].DataSeriesList[0]] + 1);
			canvas.SetValue(Canvas.TopProperty, num2);
			canvas.SetValue(Canvas.LeftProperty, -num2);
			List<DataSeries> seriesListByRenderAs = plotDetails.GetSeriesListByRenderAs(chartType);
			Dictionary<Axis, Dictionary<Axis, int>> seriesIndex = ColumnChart.GetSeriesIndex(seriesListByRenderAs);
			DataSeries dataSeries = null;
			foreach (PlotGroup current in list)
			{
				if (seriesIndex.ContainsKey(current.AxisY))
				{
					dataSeries = current.DataSeriesList[0];
					double[] xValuesUnderViewPort = RenderHelper.GetXValuesUnderViewPort(current.XWiseStackedDataList.Keys.ToList<double>(), current.AxisX, current.AxisY, false);
					current.DrawingIndex = (double)seriesIndex[current.AxisY][current.AxisX];
					double num3;
					double maxColumnWidth;
					double widthPerColumn = ColumnChart.CalculateWidthOfEachStackedColumn(chart, current, width, out num3, out maxColumnWidth);
					double limitingYValue = 0.0;
					if (current.AxisY.InternalAxisMinimum > 0.0)
					{
						limitingYValue = current.AxisY.InternalAxisMinimum;
					}
					if (current.AxisY.InternalAxisMaximum < 0.0)
					{
						limitingYValue = 0.0;
					}
					double[] array = xValuesUnderViewPort;
					for (int i = 0; i < array.Length; i++)
					{
						double xValue = array[i];
						ColumnChart.DrawStackedColumnsAtXValue(chartType, xValue, current, canvas3, canvas2, current.DrawingIndex, widthPerColumn, maxColumnWidth, limitingYValue, num, animationEnabled);
					}
				}
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
			if (list.Count > 0 && list[0].XWiseStackedDataList.Keys.Count > 0)
			{
				ColumnChart.CreateOrUpdatePlank(chart, list[0].AxisY, canvas3, num, Orientation.Horizontal);
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
				rectangleGeometry.Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH - (double)(chart.View3D ? 0 : 5), width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + (double)(chart.View3D ? 0 : 10));
			}
			else
			{
				rectangleGeometry.Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH - (double)(chart.View3D ? 0 : 5), width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + (double)(chart.View3D ? 0 : 10));
			}
			canvas.Clip = rectangleGeometry;
			PlotArea plotArea = chart.PlotArea;
			RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
			if (flag)
			{
				rectangleGeometry2.Rect = new Rect(0.0, plotArea.BorderThickness.Top - chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS - plotArea.BorderThickness.Bottom - plotArea.BorderThickness.Top);
			}
			else
			{
				rectangleGeometry2.Rect = new Rect(0.0, plotArea.BorderThickness.Top - chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH - plotArea.BorderThickness.Bottom - plotArea.BorderThickness.Top);
			}
			canvas3.Clip = rectangleGeometry2;
			return canvas;
		}

		internal static double CalculatePositionOfDataPointForLogAxis(IDataPoint dataPoint, double canvasSize, PlotGroup plotGroup, List<IDataPoint> stackedDataPoints, double absoluteSum)
		{
			double internalYValue = dataPoint.DpInfo.InternalYValue;
			if (stackedDataPoints.First<IDataPoint>() == dataPoint)
			{
				double num;
				if (dataPoint.Parent.RenderAs == RenderAs.StackedColumn100 || dataPoint.Parent.RenderAs == RenderAs.StackedBar100)
				{
					num = dataPoint.YValue / absoluteSum * 100.0;
					num = Math.Log(num, plotGroup.AxisY.LogarithmBase);
				}
				else
				{
					num = internalYValue;
				}
				double result;
				if ((dataPoint.Chart as Chart).PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					result = Graphics.ValueToPixelPosition(canvasSize, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num);
				}
				else
				{
					result = Graphics.ValueToPixelPosition(0.0, canvasSize, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num);
				}
				return result;
			}
			int i = stackedDataPoints.IndexOf(dataPoint);
			double num2 = 0.0;
			while (i >= 0)
			{
				if (dataPoint.Parent.RenderAs == RenderAs.StackedColumn100 || dataPoint.Parent.RenderAs == RenderAs.StackedBar100)
				{
					num2 += dataPoint.YValue / absoluteSum * 100.0;
				}
				else
				{
					num2 += stackedDataPoints[i].YValue;
				}
				i--;
			}
			num2 = Math.Log(num2, plotGroup.AxisY.LogarithmBase);
			double result2;
			if ((dataPoint.Chart as Chart).PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				result2 = Graphics.ValueToPixelPosition(canvasSize, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num2);
			}
			else
			{
				result2 = Graphics.ValueToPixelPosition(0.0, canvasSize, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num2);
			}
			return result2;
		}

		private static void DrawStackedColumnsAtXValue(RenderAs chartType, double xValue, PlotGroup plotGroup, Canvas columnCanvas, Canvas labelCanvas, double drawingIndex, double widthPerColumn, double maxColumnWidth, double limitingYValue, double depth3d, bool animationEnabled)
		{
			double num = 0.0;
			int num2 = 1;
			int num3 = 1;
			IDataPoint dataPoint = null;
			double num4 = 0.0;
			double num5 = Graphics.ValueToPixelPosition(columnCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, 0.0);
			double left = Graphics.ValueToPixelPosition(0.0, columnCanvas.Width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, xValue) + drawingIndex * widthPerColumn - maxColumnWidth / 2.0;
			double num6 = Graphics.ValueToPixelPosition(columnCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, limitingYValue);
			if (plotGroup.XWiseStackedDataList[xValue].Positive.Count > 0)
			{
				dataPoint = plotGroup.XWiseStackedDataList[xValue].Positive.Last<IDataPoint>();
			}
			if (chartType == RenderAs.StackedColumn100)
			{
				num4 = plotGroup.XWiseStackedDataList[xValue].AbsoluteYValueSum;
			}
			double num7 = 0.4;
			double num8 = 1.0 / (double)plotGroup.XWiseStackedDataList[xValue].Positive.Count;
			int count = plotGroup.XWiseStackedDataList[xValue].Positive.Count;
			List<IDataPoint> list = plotGroup.XWiseStackedDataList[xValue].Positive.ToList<IDataPoint>();
			bool flag = true;
			double num10;
			foreach (IDataPoint current in plotGroup.XWiseStackedDataList[xValue].Positive)
			{
				foreach (IDataPoint current2 in list)
				{
					if (current2.DpInfo.InternalYValue <= limitingYValue)
					{
						flag = false;
						break;
					}
				}
				current.Parent.Faces = new Faces
				{
					Visual = columnCanvas,
					LabelCanvas = labelCanvas
				};
				if (current.Enabled.Value && !double.IsNaN(current.DpInfo.InternalYValue))
				{
					bool flag2 = current == dataPoint;
					int num9 = plotGroup.XWiseStackedDataList[xValue].Positive.IndexOf(current);
					bool flag3 = true;
					for (int i = num9 + 1; i < plotGroup.XWiseStackedDataList[xValue].Positive.Count; i++)
					{
						if (plotGroup.XWiseStackedDataList[xValue].Positive[i].DpInfo.InternalYValue != 0.0)
						{
							flag3 = false;
							break;
						}
					}
					if (flag3 && !flag2)
					{
						flag2 = true;
					}
					double num11;
					if (chartType == RenderAs.StackedColumn)
					{
						if (plotGroup.AxisY.Logarithmic)
						{
							num10 = ColumnChart.CalculatePositionOfDataPointForLogAxis(current, columnCanvas.Height, plotGroup, plotGroup.XWiseStackedDataList[xValue].Positive.ToList<IDataPoint>(), num4);
						}
						else
						{
							num10 = Graphics.ValueToPixelPosition(columnCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, current.DpInfo.InternalYValue + num);
							if (depth3d != 0.0 && current.DpInfo.InternalYValue + num > plotGroup.AxisY.InternalAxisMaximum && plotGroup.AxisY.InternalAxisMaximum > 0.0)
							{
								num10 = 0.0;
							}
							else if (plotGroup.AxisY.AxisMinimum != null && current.DpInfo.InternalYValue < limitingYValue)
							{
								if (list.Count == 1)
								{
									num6 = (num10 = num5 + 100.0);
								}
								else if (flag)
								{
									num6 = (num10 = num5 + 100.0);
								}
							}
							else if (current.DpInfo.InternalYValue < limitingYValue && current.DpInfo.InternalYValue == 0.0)
							{
								if (list.Count == 1)
								{
									num6 = (num10 = num5 + 100.0);
								}
								else if (flag)
								{
									num6 = (num10 = num5 + 100.0);
								}
							}
						}
						if (num10 > columnCanvas.Height)
						{
							num6 = (num10 = num5 + 100.0);
						}
						num11 = Math.Abs(num10 - num6);
						num += current.DpInfo.InternalYValue;
					}
					else
					{
						double num12 = 0.0;
						if (plotGroup.AxisY.Logarithmic)
						{
							if (num4 != 0.0)
							{
								num12 = Math.Log(current.YValue / num4 * 100.0, plotGroup.AxisY.LogarithmBase);
							}
							num10 = ColumnChart.CalculatePositionOfDataPointForLogAxis(current, columnCanvas.Height, plotGroup, plotGroup.XWiseStackedDataList[xValue].Positive.ToList<IDataPoint>(), num4);
						}
						else
						{
							if (num4 != 0.0)
							{
								num12 = current.DpInfo.InternalYValue / num4 * 100.0;
							}
							if (depth3d != 0.0 && num12 + num > plotGroup.AxisY.InternalAxisMaximum && plotGroup.AxisY.InternalAxisMaximum > 0.0)
							{
								num10 = 0.0;
							}
							else
							{
								num10 = Graphics.ValueToPixelPosition(columnCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num12 + num);
							}
							if (num10 > columnCanvas.Height)
							{
								num10 = num6;
							}
						}
						num11 = Math.Abs(num10 - num6);
						num += num12;
					}
					if (num11 != 0.0 || num <= plotGroup.AxisY.InternalAxisMaximum)
					{
						if (chartType == RenderAs.StackedColumn || chartType == RenderAs.StackedColumn100)
						{
							ColumnChart.CreateStackedColumnVisual(true, columnCanvas, labelCanvas, current, flag2, num, left, ref num10, ref num6, widthPerColumn, num11, depth3d, ref num2, count, animationEnabled, num7);
						}
						num7 += num8;
						num2++;
					}
				}
			}
			num = 0.0;
			num10 = Graphics.ValueToPixelPosition(columnCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, limitingYValue);
			dataPoint = null;
			if (plotGroup.XWiseStackedDataList[xValue].Negative.Count > 0)
			{
				dataPoint = plotGroup.XWiseStackedDataList[xValue].Negative.Last<IDataPoint>();
				num8 = 1.0 / (double)plotGroup.XWiseStackedDataList[xValue].Negative.Count;
				num7 = 0.4;
			}
			int count2 = plotGroup.XWiseStackedDataList[xValue].Negative.Count;
			foreach (IDataPoint current3 in plotGroup.XWiseStackedDataList[xValue].Negative)
			{
				current3.Parent.Faces = new Faces
				{
					Visual = columnCanvas,
					LabelCanvas = labelCanvas
				};
				if (current3.Enabled.Value && !double.IsNaN(current3.DpInfo.InternalYValue))
				{
					bool flag2 = current3 == dataPoint;
					int num13 = plotGroup.XWiseStackedDataList[xValue].Negative.IndexOf(current3);
					bool flag4 = true;
					for (int j = num13 + 1; j < plotGroup.XWiseStackedDataList[xValue].Negative.Count; j++)
					{
						if (plotGroup.XWiseStackedDataList[xValue].Negative[j].DpInfo.InternalYValue != 0.0)
						{
							flag4 = false;
							break;
						}
					}
					if (flag4 && !flag2)
					{
						flag2 = true;
					}
					double num11;
					if (chartType == RenderAs.StackedColumn)
					{
						num6 = Graphics.ValueToPixelPosition(columnCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, current3.DpInfo.InternalYValue + num);
						if (plotGroup.AxisY.AxisMinimum != null && limitingYValue > 0.0 && current3.DpInfo.InternalYValue < limitingYValue)
						{
							num6 = (num10 = num6 + 100.0);
						}
						else if (plotGroup.AxisY.AxisMinimum != null && plotGroup.AxisY.InternalAxisMinimum == 0.0 && current3.DpInfo.InternalYValue < 0.0)
						{
							num6 = (num10 = num6 + 100.0);
						}
						else if (plotGroup.AxisY.InternalAxisMinimum < 0.0 && plotGroup.AxisY.InternalAxisMinimum > current3.DpInfo.InternalYValue + num)
						{
							num6 = columnCanvas.Height;
						}
						num += current3.DpInfo.InternalYValue;
						num11 = Math.Abs(num10 - num6);
					}
					else
					{
						double num14 = current3.DpInfo.InternalYValue / num4 * 100.0;
						num6 = Graphics.ValueToPixelPosition(columnCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num14 + num);
						if (plotGroup.AxisY.InternalAxisMinimum < 0.0 && plotGroup.AxisY.InternalAxisMinimum > num14 + num)
						{
							num6 = Graphics.ValueToPixelPosition(columnCanvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, plotGroup.AxisY.InternalAxisMinimum);
						}
						num11 = Math.Abs(num10 - num6);
						num += num14;
					}
					if (num11 != 0.0)
					{
						ColumnChart.CreateStackedColumnVisual(false, columnCanvas, labelCanvas, current3, flag2, num, left, ref num10, ref num6, widthPerColumn, num11, depth3d, ref num3, count2, animationEnabled, num7);
						num7 += num8;
						num3--;
					}
				}
			}
		}

		private static Storyboard ApplyStackedColumnChartAnimation(DataSeries dataSeries, Panel column, Storyboard storyboard, double begin, double duration)
		{
			ScaleTransform scaleTransform = new ScaleTransform
			{
				ScaleY = 0.0
			};
			column.RenderTransform = scaleTransform;
			column.RenderTransformOrigin = new Point(0.5, 0.5);
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
			DoubleAnimationUsingKeyFrames value = AnimationHelper.CreateDoubleAnimation(dataSeries, scaleTransform, "(ScaleTransform.ScaleY)", begin + 0.5, frameTime, values, splines);
			storyboard.Children.Add(value);
			return storyboard;
		}

		internal static double CalculateWidthOfEachStackedColumn(Chart chart, PlotGroup plotGroup, double heightOrWidth, out double minDiff, out double maxColumnWidth)
		{
			PlotDetails plotDetails = chart.PlotDetails;
			double num = (double)plotDetails.DrawingDivisionFactor;
			double num2 = chart.MaxDataPointWidth;
			if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				minDiff = plotDetails.GetMinOfMinDifferencesForXValue(new RenderAs[]
				{
					RenderAs.Column,
					RenderAs.StackedColumn,
					RenderAs.StackedColumn100
				});
			}
			else
			{
				minDiff = plotDetails.GetMinOfMinDifferencesForXValue(new RenderAs[]
				{
					RenderAs.Bar,
					RenderAs.StackedBar,
					RenderAs.StackedBar100
				});
			}
			if (double.IsPositiveInfinity(minDiff))
			{
				minDiff = 0.0;
			}
			maxColumnWidth = Graphics.ValueToPixelPosition(0.0, heightOrWidth, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, minDiff + plotGroup.AxisX.InternalAxisMinimum) * (1.0 - ColumnChart.COLUMN_GAP_RATIO);
			double num3 = maxColumnWidth / num;
			if (minDiff == 0.0)
			{
				num3 = heightOrWidth * 0.5 / num;
				maxColumnWidth = num3 * num;
			}
			else
			{
				num3 = Graphics.ValueToPixelPosition(0.0, heightOrWidth, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, minDiff + plotGroup.AxisX.InternalAxisMinimum);
				num3 *= 1.0 - ((chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical) ? ColumnChart.COLUMN_GAP_RATIO : BarChart.BAR_GAP_RATIO);
				maxColumnWidth = num3;
				num3 /= num;
			}
			if (!double.IsNaN(chart.DataPointWidth) && chart.DataPointWidth >= 0.0)
			{
				num3 = (maxColumnWidth = chart.DataPointWidth / 100.0 * ((chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical) ? chart.PlotArea.Width : chart.PlotArea.Height));
				maxColumnWidth *= num;
			}
			num2 = num2 / 100.0 * ((chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical) ? chart.PlotArea.Width : chart.PlotArea.Height);
			if (num3 > num2)
			{
				num3 = num2;
				maxColumnWidth = num2 * num;
			}
			if (num3 < 1.0 && num2 >= 1.0 && double.IsNaN(chart.DataPointWidth))
			{
				num3 = 1.0;
			}
			else if (num3 < 1.0 && (double.IsPositiveInfinity(num2) || double.IsNaN(num2)) && double.IsNaN(chart.DataPointWidth))
			{
				num3 = 1.0;
			}
			return num3;
		}

		public static void UpdateVisualForYValue4StackedBarChart(RenderAs chartType, Chart chart, IDataPoint dataPoint, bool isAxisChanged)
		{
			if (chart != null && !chart._internalPartialUpdateEnabled)
			{
				return;
			}
			bool value = chart.AnimatedUpdate.Value;
			DataSeries parent = dataPoint.Parent;
			PlotGroup plotGroup = parent.PlotGroup;
			double depth3d = chart.ChartArea.PLANK_DEPTH / (double)chart.PlotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
			double limitingYValue = 0.0;
			if (plotGroup.AxisY.InternalAxisMinimum > 0.0)
			{
				limitingYValue = plotGroup.AxisY.InternalAxisMinimum;
			}
			if (plotGroup.AxisY.InternalAxisMaximum < 0.0)
			{
				limitingYValue = 0.0;
			}
			Canvas labelCanvas;
			Canvas canvas;
			double num;
			double maxBarHeight;
			double heightPerBar;
			if (dataPoint.DpInfo.Faces == null)
			{
				if (parent == null || parent.Faces == null)
				{
					ColumnChart.UpdateDataSeries(parent, VcProperties.YValue, null);
					return;
				}
				labelCanvas = parent.Faces.LabelCanvas;
				canvas = (parent.Faces.Visual as Canvas);
				heightPerBar = ColumnChart.CalculateWidthOfEachStackedColumn(chart, plotGroup, canvas.Height, out num, out maxBarHeight);
				if (dataPoint.Parent.RenderAs == RenderAs.StackedBar || dataPoint.Parent.RenderAs == RenderAs.StackedBar100)
				{
					BarChart.DrawStackedBarsAtXValue(chartType, dataPoint.DpInfo.InternalXValue, plotGroup, canvas, labelCanvas, plotGroup.DrawingIndex, heightPerBar, maxBarHeight, limitingYValue, depth3d, false);
				}
				if (dataPoint.DpInfo.Faces == null)
				{
					return;
				}
			}
			Canvas canvas2 = dataPoint.DpInfo.Faces.Visual as Canvas;
			labelCanvas = dataPoint.DpInfo.Faces.LabelCanvas;
			if (labelCanvas == null)
			{
				return;
			}
			canvas = ((labelCanvas.Parent as Canvas).Children[1] as Canvas);
			heightPerBar = ColumnChart.CalculateWidthOfEachStackedColumn(chart, plotGroup, canvas.Height, out num, out maxBarHeight);
			RenderHelper.UpdateParentVisualCanvasSize(chart, canvas);
			RenderHelper.UpdateParentVisualCanvasSize(chart, labelCanvas);
			XWiseStackedData xWiseStackedData = plotGroup.XWiseStackedDataList[dataPoint.DpInfo.InternalXValue];
			List<IDataPoint> collection = xWiseStackedData.Positive.ToList<IDataPoint>();
			List<IDataPoint> collection2 = xWiseStackedData.Negative.ToList<IDataPoint>();
			List<IDataPoint> list = new List<IDataPoint>();
			list.AddRange(collection);
			list.AddRange(collection2);
			List<KeyValuePair<IDataPoint, FrameworkElement>> list2 = new List<KeyValuePair<IDataPoint, FrameworkElement>>();
			List<KeyValuePair<IDataPoint, Point>> list3 = new List<KeyValuePair<IDataPoint, Point>>();
			List<KeyValuePair<IDataPoint, Point>> list4 = new List<KeyValuePair<IDataPoint, Point>>();
			foreach (IDataPoint current in list)
			{
				if (current.DpInfo.Marker != null && current.DpInfo.Marker.Visual != null)
				{
					list3.Add(new KeyValuePair<IDataPoint, Point>(current, new Point((double)current.DpInfo.Marker.Visual.GetValue(Canvas.LeftProperty), (double)current.DpInfo.Marker.Visual.GetValue(Canvas.TopProperty))));
				}
				else
				{
					list3.Add(new KeyValuePair<IDataPoint, Point>(current, default(Point)));
				}
				if (current.DpInfo.LabelVisual != null)
				{
					list4.Add(new KeyValuePair<IDataPoint, Point>(current, new Point((double)current.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty), (double)current.DpInfo.LabelVisual.GetValue(Canvas.TopProperty))));
				}
				else
				{
					list4.Add(new KeyValuePair<IDataPoint, Point>(current, default(Point)));
				}
				if (current.DpInfo.Faces != null)
				{
					FrameworkElement visual = current.DpInfo.Faces.Visual;
					list2.Add(new KeyValuePair<IDataPoint, FrameworkElement>(current, visual));
					canvas.Children.Remove(visual);
					ColumnChart.CleanUpMarkerAndLabel(current, labelCanvas);
				}
			}
			BarChart.DrawStackedBarsAtXValue(chartType, dataPoint.DpInfo.InternalXValue, plotGroup, canvas, labelCanvas, plotGroup.DrawingIndex, canvas2.Height, maxBarHeight, limitingYValue, depth3d, false);
			if (parent.ToolTipElement != null)
			{
				parent.ToolTipElement.Hide();
			}
			ColumnChart.CreateOrUpdatePlank(chart, parent.PlotGroup.AxisY, canvas, depth3d, (dataPoint.Parent.RenderAs == RenderAs.StackedColumn || dataPoint.Parent.RenderAs == RenderAs.StackedColumn100) ? Orientation.Horizontal : Orientation.Vertical);
			if (value)
			{
				if (dataPoint.DpInfo.Storyboard != null)
				{
					dataPoint.DpInfo.Storyboard.Stop();
					dataPoint.DpInfo.Storyboard.Children.Clear();
				}
				if (dataPoint.DpInfo.Storyboard == null)
				{
					dataPoint.DpInfo.Storyboard = new Storyboard();
				}
				bool flag = false;
				foreach (IDataPoint current2 in list)
				{
					int index = list.IndexOf(current2);
					KeyValuePair<IDataPoint, FrameworkElement> keyValuePair = list2.ElementAt(index);
					if (current2.DpInfo.Faces != null && keyValuePair.Value != null)
					{
						FrameworkElement visual2 = current2.DpInfo.Faces.Visual;
						double num2 = (double)keyValuePair.Value.GetValue(Canvas.LeftProperty);
						double num3 = (double)visual2.GetValue(Canvas.LeftProperty);
						double width = keyValuePair.Value.Width;
						double width2 = visual2.Width;
						double num4 = width / width2;
						bool flag2;
						if (current2 == dataPoint)
						{
							flag2 = ((dataPoint.DpInfo.OldYValue < 0.0 && dataPoint.DpInfo.InternalYValue > 0.0) || ((dataPoint.DpInfo.OldYValue <= 0.0 || dataPoint.DpInfo.InternalYValue >= 0.0) && current2.DpInfo.OldYValue >= 0.0));
						}
						else
						{
							flag2 = (current2.DpInfo.InternalYValue >= 0.0);
						}
						if (flag2)
						{
							visual2.RenderTransformOrigin = new Point(0.0, 0.5);
						}
						else
						{
							visual2.RenderTransformOrigin = new Point(1.0, 0.5);
						}
						visual2.RenderTransform = new ScaleTransform();
						if (double.IsInfinity(num4) || double.IsNaN(num4))
						{
							num4 = 0.0;
						}
						if (num4 != 1.0)
						{
							dataPoint.DpInfo.Storyboard = AnimationHelper.ApplyPropertyAnimation(visual2, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)", dataPoint, dataPoint.DpInfo.Storyboard, 0.0, new double[]
							{
								0.0,
								1.0
							}, new double[]
							{
								num4,
								1.0
							}, null);
						}
						if ((flag && num2 != num3) || (current2.DpInfo.OldYValue == current2.DpInfo.InternalYValue && num2 != num3))
						{
							visual2.SetValue(Canvas.LeftProperty, num2);
							dataPoint.DpInfo.Storyboard = AnimationHelper.ApplyPropertyAnimation(visual2, "(Canvas.Left)", dataPoint, dataPoint.DpInfo.Storyboard, 0.0, new double[]
							{
								0.0,
								1.0
							}, new double[]
							{
								num2,
								num3
							}, null);
						}
						if (current2 == dataPoint)
						{
							flag = true;
						}
						if (current2.DpInfo.Marker != null && current2.DpInfo.Marker.Visual != null)
						{
							double num5 = (double)current2.DpInfo.Marker.Visual.GetValue(Canvas.LeftProperty);
							int index2 = list.IndexOf(current2);
							KeyValuePair<IDataPoint, Point> keyValuePair2 = list3.ElementAt(index2);
							current2.DpInfo.Marker.Visual.SetValue(Canvas.LeftProperty, keyValuePair2.Value.X);
							dataPoint.DpInfo.Storyboard = AnimationHelper.ApplyPropertyAnimation(current2.DpInfo.Marker.Visual, "(Canvas.Left)", dataPoint, dataPoint.DpInfo.Storyboard, 0.0, new double[]
							{
								0.0,
								1.0
							}, new double[]
							{
								keyValuePair2.Value.X,
								num5
							}, null);
						}
						if (current2.DpInfo.LabelVisual != null)
						{
							double num6 = (double)current2.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
							int index3 = list.IndexOf(current2);
							KeyValuePair<IDataPoint, Point> keyValuePair3 = list4.ElementAt(index3);
							current2.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, keyValuePair3.Value.X);
							dataPoint.DpInfo.Storyboard = AnimationHelper.ApplyPropertyAnimation(current2.DpInfo.LabelVisual, "(Canvas.Left)", dataPoint, dataPoint.DpInfo.Storyboard, 0.0, new double[]
							{
								0.0,
								1.0
							}, new double[]
							{
								keyValuePair3.Value.X,
								num6
							}, null);
						}
						dataPoint.DpInfo.Storyboard.SpeedRatio = 2.0;
					}
				}
				dataPoint.DpInfo.Storyboard.SpeedRatio = 2.0;
				dataPoint.DpInfo.Storyboard.Begin(dataPoint.Chart._rootElement, true);
			}
			if (canvas.Parent != null)
			{
				double width3 = chart.ChartArea.ChartVisualCanvas.Width;
				double height = chart.ChartArea.ChartVisualCanvas.Height;
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = new Rect((double)(-(double)(chart.View3D ? 0 : 5)) - chart.ChartArea.PLANK_THICKNESS, -chart.ChartArea.PLANK_DEPTH, width3 + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + (double)(chart.View3D ? 0 : 10), height + chart.ChartArea.PLANK_DEPTH);
				(canvas.Parent as Canvas).Clip = rectangleGeometry;
			}
			dataPoint.Parent.AttachOrDetachInteractivity4DataPoint(dataPoint);
			if (!dataPoint.IsLightDataPoint && dataPoint.Parent.SelectionEnabled && (dataPoint as DataPoint).Selected)
			{
				DataPointHelper.Select(dataPoint, true);
			}
		}

		public static void UpdateVisualForYValue4StackedColumnChart(RenderAs chartType, Chart chart, IDataPoint dataPoint, bool isAxisChanged)
		{
			if (chart != null && !chart._internalPartialUpdateEnabled)
			{
				return;
			}
			bool value = chart.AnimatedUpdate.Value;
			DataSeries parent = dataPoint.Parent;
			double depth3d = chart.ChartArea.PLANK_DEPTH / (double)chart.PlotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
			PlotGroup plotGroup = parent.PlotGroup;
			double limitingYValue = 0.0;
			if (plotGroup.AxisY.InternalAxisMinimum > 0.0)
			{
				limitingYValue = plotGroup.AxisY.InternalAxisMinimum;
			}
			if (plotGroup.AxisY.InternalAxisMaximum < 0.0)
			{
				limitingYValue = 0.0;
			}
			Canvas labelCanvas;
			Canvas canvas;
			double height;
			double width;
			double num;
			double maxColumnWidth;
			double widthPerColumn;
			if (dataPoint.DpInfo.Faces == null)
			{
				if (parent == null || parent.Faces == null)
				{
					ColumnChart.UpdateDataSeries(parent, VcProperties.YValue, null);
					return;
				}
				labelCanvas = parent.Faces.LabelCanvas;
				canvas = (parent.Faces.Visual as Canvas);
				height = labelCanvas.Height;
				width = labelCanvas.Width;
				widthPerColumn = ColumnChart.CalculateWidthOfEachStackedColumn(chart, plotGroup, width, out num, out maxColumnWidth);
				if (dataPoint.Parent.RenderAs == RenderAs.StackedColumn || dataPoint.Parent.RenderAs == RenderAs.StackedColumn100)
				{
					ColumnChart.DrawStackedColumnsAtXValue(chartType, dataPoint.DpInfo.InternalXValue, plotGroup, canvas, labelCanvas, plotGroup.DrawingIndex, widthPerColumn, maxColumnWidth, limitingYValue, depth3d, false);
				}
				if (dataPoint.DpInfo.Faces == null)
				{
					return;
				}
			}
			labelCanvas = dataPoint.DpInfo.Faces.LabelCanvas;
			if (labelCanvas == null)
			{
				return;
			}
			canvas = ((labelCanvas.Parent as Canvas).Children[1] as Canvas);
			height = labelCanvas.Height;
			width = labelCanvas.Width;
			widthPerColumn = ColumnChart.CalculateWidthOfEachStackedColumn(chart, plotGroup, width, out num, out maxColumnWidth);
			Canvas canvas2 = dataPoint.DpInfo.Faces.Visual as Canvas;
			RenderHelper.UpdateParentVisualCanvasSize(chart, canvas);
			RenderHelper.UpdateParentVisualCanvasSize(chart, labelCanvas);
			XWiseStackedData xWiseStackedData = plotGroup.XWiseStackedDataList[dataPoint.DpInfo.InternalXValue];
			List<IDataPoint> collection = xWiseStackedData.Positive.ToList<IDataPoint>();
			List<IDataPoint> collection2 = xWiseStackedData.Negative.ToList<IDataPoint>();
			List<IDataPoint> list = new List<IDataPoint>();
			list.AddRange(collection);
			list.AddRange(collection2);
			List<KeyValuePair<IDataPoint, FrameworkElement>> list2 = new List<KeyValuePair<IDataPoint, FrameworkElement>>();
			List<KeyValuePair<IDataPoint, Point>> list3 = new List<KeyValuePair<IDataPoint, Point>>();
			List<KeyValuePair<IDataPoint, Point>> list4 = new List<KeyValuePair<IDataPoint, Point>>();
			foreach (IDataPoint current in list)
			{
				if (current.DpInfo.Marker != null && current.DpInfo.Marker.Visual != null)
				{
					list3.Add(new KeyValuePair<IDataPoint, Point>(current, new Point((double)current.DpInfo.Marker.Visual.GetValue(Canvas.LeftProperty), (double)current.DpInfo.Marker.Visual.GetValue(Canvas.TopProperty))));
				}
				else
				{
					list3.Add(new KeyValuePair<IDataPoint, Point>(current, default(Point)));
				}
				if (current.DpInfo.LabelVisual != null)
				{
					list4.Add(new KeyValuePair<IDataPoint, Point>(current, new Point((double)current.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty), (double)current.DpInfo.LabelVisual.GetValue(Canvas.TopProperty))));
				}
				else
				{
					list4.Add(new KeyValuePair<IDataPoint, Point>(current, default(Point)));
				}
				if (current.DpInfo.Faces != null)
				{
					FrameworkElement visual = current.DpInfo.Faces.Visual;
					list2.Add(new KeyValuePair<IDataPoint, FrameworkElement>(current, visual));
					canvas.Children.Remove(visual);
					ColumnChart.CleanUpMarkerAndLabel(current, labelCanvas);
				}
				else
				{
					list2.Add(new KeyValuePair<IDataPoint, FrameworkElement>(current, null));
				}
			}
			ColumnChart.DrawStackedColumnsAtXValue(chartType, dataPoint.DpInfo.InternalXValue, plotGroup, canvas, labelCanvas, plotGroup.DrawingIndex, canvas2.Width, maxColumnWidth, limitingYValue, depth3d, false);
			if (parent.ToolTipElement != null)
			{
				parent.ToolTipElement.Hide();
			}
			chart.ChartArea.DisableIndicators();
			ColumnChart.CreateOrUpdatePlank(chart, parent.PlotGroup.AxisY, canvas, depth3d, (dataPoint.Parent.RenderAs == RenderAs.StackedColumn || dataPoint.Parent.RenderAs == RenderAs.StackedColumn100) ? Orientation.Horizontal : Orientation.Vertical);
			if (value)
			{
				if (dataPoint.DpInfo.Storyboard != null)
				{
					dataPoint.DpInfo.Storyboard.Stop();
					dataPoint.DpInfo.Storyboard.Children.Clear();
				}
				if (dataPoint.DpInfo.Storyboard == null)
				{
					dataPoint.DpInfo.Storyboard = new Storyboard();
				}
				bool flag = false;
				foreach (IDataPoint current2 in list)
				{
					int index = list.IndexOf(current2);
					KeyValuePair<IDataPoint, FrameworkElement> keyValuePair = list2.ElementAt(index);
					if (current2.DpInfo.Faces != null && keyValuePair.Value != null)
					{
						FrameworkElement visual2 = current2.DpInfo.Faces.Visual;
						double num2 = (double)keyValuePair.Value.GetValue(Canvas.TopProperty);
						double num3 = (double)visual2.GetValue(Canvas.TopProperty);
						double height2 = keyValuePair.Value.Height;
						double height3 = visual2.Height;
						double num4 = height2 / height3;
						bool flag2;
						if (current2 == dataPoint)
						{
							flag2 = ((dataPoint.DpInfo.OldYValue < 0.0 && dataPoint.DpInfo.InternalYValue > 0.0) || ((dataPoint.DpInfo.OldYValue <= 0.0 || dataPoint.DpInfo.InternalYValue >= 0.0) && current2.DpInfo.OldYValue >= 0.0));
						}
						else
						{
							flag2 = (current2.DpInfo.InternalYValue >= 0.0);
						}
						if (flag2)
						{
							visual2.RenderTransformOrigin = new Point(0.5, 1.0);
						}
						else
						{
							visual2.RenderTransformOrigin = new Point(0.5, 0.0);
						}
						visual2.RenderTransform = new ScaleTransform();
						if (double.IsInfinity(num4) || double.IsNaN(num4))
						{
							num4 = 0.0;
						}
						if (num4 != 1.0)
						{
							dataPoint.DpInfo.Storyboard = AnimationHelper.ApplyPropertyAnimation(visual2, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)", dataPoint, dataPoint.DpInfo.Storyboard, 0.0, new double[]
							{
								0.0,
								1.0
							}, new double[]
							{
								num4,
								1.0
							}, null);
						}
						if ((flag && num2 != num3) || (current2.DpInfo.OldYValue == current2.DpInfo.InternalYValue && num2 != num3))
						{
							visual2.SetValue(Canvas.TopProperty, num2);
							dataPoint.DpInfo.Storyboard = AnimationHelper.ApplyPropertyAnimation(visual2, "(Canvas.Top)", dataPoint, dataPoint.DpInfo.Storyboard, 0.0, new double[]
							{
								0.0,
								1.0
							}, new double[]
							{
								num2,
								num3
							}, null);
						}
						if (current2 == dataPoint)
						{
							flag = true;
						}
						if (current2.DpInfo.Marker != null && current2.DpInfo.Marker.Visual != null)
						{
							double num5 = (double)current2.DpInfo.Marker.Visual.GetValue(Canvas.TopProperty);
							int index2 = list.IndexOf(current2);
							KeyValuePair<IDataPoint, Point> keyValuePair2 = list3.ElementAt(index2);
							dataPoint.DpInfo.Storyboard = AnimationHelper.ApplyPropertyAnimation(current2.DpInfo.Marker.Visual, "(Canvas.Top)", dataPoint, dataPoint.DpInfo.Storyboard, 0.0, new double[]
							{
								0.0,
								1.0
							}, new double[]
							{
								keyValuePair2.Value.Y,
								num5
							}, null);
							current2.DpInfo.Marker.Visual.SetValue(Canvas.TopProperty, keyValuePair2.Value.Y);
						}
						if (current2.DpInfo.LabelVisual != null)
						{
							double num6 = (double)current2.DpInfo.LabelVisual.GetValue(Canvas.TopProperty);
							int index3 = list.IndexOf(current2);
							KeyValuePair<IDataPoint, Point> keyValuePair3 = list4.ElementAt(index3);
							dataPoint.DpInfo.Storyboard = AnimationHelper.ApplyPropertyAnimation(current2.DpInfo.LabelVisual, "(Canvas.Top)", dataPoint, dataPoint.DpInfo.Storyboard, 0.0, new double[]
							{
								0.0,
								1.0
							}, new double[]
							{
								keyValuePair3.Value.Y,
								num6
							}, null);
							current2.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, keyValuePair3.Value.Y);
						}
						dataPoint.DpInfo.Storyboard.SpeedRatio = 2.0;
					}
				}
				dataPoint.DpInfo.Storyboard.SpeedRatio = 2.0;
				dataPoint.DpInfo.Storyboard.Begin(dataPoint.Chart._rootElement, true);
			}
			if (canvas.Parent != null)
			{
				width = chart.ChartArea.ChartVisualCanvas.Width;
				height = chart.ChartArea.ChartVisualCanvas.Height;
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = new Rect(0.0, -chart.ChartArea.PLANK_DEPTH - (double)(chart.View3D ? 0 : 5), width + chart.ChartArea.PLANK_DEPTH, height + chart.ChartArea.PLANK_DEPTH + chart.ChartArea.PLANK_THICKNESS + (double)(chart.View3D ? 0 : 10));
				(canvas.Parent as Canvas).Clip = rectangleGeometry;
			}
			dataPoint.Parent.AttachOrDetachInteractivity4DataPoint(dataPoint);
			if (!dataPoint.IsLightDataPoint && dataPoint.Parent.SelectionEnabled && (dataPoint as DataPoint).Selected)
			{
				DataPointHelper.Select(dataPoint, true);
			}
		}
	}
}
