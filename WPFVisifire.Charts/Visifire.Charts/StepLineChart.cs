using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class StepLineChart
	{
		private const double MOVING_MARKER_SCALE = 1.1;

		private static bool _isMouseEnteredInPlotArea;

		private static Point CreateLabel4LineDataPoint(IDataPoint dataPoint, double width, double height, bool isPositive, double markerLeft, double markerTop, ref Canvas labelCanvas, bool IsSetPosition)
		{
			Point result = default(Point);
			if (dataPoint.DpInfo.LabelVisual != null)
			{
				Panel panel = dataPoint.DpInfo.LabelVisual.Parent as Panel;
				if (panel != null)
				{
					panel.Children.Remove(dataPoint.DpInfo.LabelVisual);
				}
			}
			Chart chart = dataPoint.Chart as Chart;
			if (dataPoint.DpInfo.Faces == null || double.IsNaN(dataPoint.DpInfo.InternalYValue))
			{
				return result;
			}
			LabelStyles labelStyles = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			double num = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelAngle);
			Brush labelFontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
			FontStyle internalFontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
			Brush internalBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
			FontFamily internalFontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
			double internalFontSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontSize);
			FontWeight internalFontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled);
			string text = (string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText);
			if (flag && !string.IsNullOrEmpty(text))
			{
				LabelStyles labelStyles2 = labelStyles;
				Title title = new Title
				{
					Text = dataPoint.TextParser(text),
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
				}, chart.FlowDirection);
				double num2 = 0.0;
				double num3 = 0.0;
				double gap = 6.0;
				if (double.IsNaN(num) || num == 0.0)
				{
					StepLineChart.SetLabelPosition4LineDataPoint(dataPoint, width, height, isPositive, markerLeft, markerTop, ref num2, ref num3, gap, new Size(title.TextBlockDesiredSize.Width, title.TextBlockDesiredSize.Height));
					result.X = num2;
					result.Y = num3;
					if (IsSetPosition)
					{
						title.Visual.SetValue(Canvas.LeftProperty, num2);
						title.Visual.SetValue(Canvas.TopProperty, num3);
					}
					double num4 = chart.ChartArea.PLANK_DEPTH / (double)chart.PlotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
					if (!dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
					{
						if (isPositive)
						{
							if (num3 < -num4)
							{
								labelStyles2 = LabelStyles.Inside;
							}
						}
						else if (num3 + title.TextBlockDesiredSize.Height > chart.PlotArea.BorderElement.Height - num4 + chart.ChartArea.PLANK_THICKNESS)
						{
							labelStyles2 = LabelStyles.Inside;
						}
					}
					if (labelStyles2 != labelStyles)
					{
						StepLineChart.SetLabelPosition4LineDataPoint(dataPoint, width, height, isPositive, markerLeft, markerTop, ref num2, ref num3, gap, new Size(title.TextBlockDesiredSize.Width, title.TextBlockDesiredSize.Height));
						result.X = num2;
						result.Y = num3;
						if (IsSetPosition)
						{
							title.Visual.SetValue(Canvas.LeftProperty, num2);
							title.Visual.SetValue(Canvas.TopProperty, num3);
						}
					}
				}
				else
				{
					if (dataPoint.DpInfo.Marker == null)
					{
						return result;
					}
					if (isPositive)
					{
						Point centerOfRotation = new Point(markerLeft, markerTop - title.TextBlockDesiredSize.Height / 2.0);
						double num5 = dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						if (labelStyles2 == LabelStyles.OutSide)
						{
							if (num > 0.0 && num <= 90.0)
							{
								double num6 = num - 180.0;
								double num7 = 0.017453292519943295 * num6;
								num5 += title.TextBlockDesiredSize.Width;
								num6 = (num7 - 3.1415926535897931) * 57.295779513082323;
								ColumnChart.SetRotation(num5, num6, num7, centerOfRotation, num2, num3, title);
							}
							else if (num >= -90.0 && num < 0.0)
							{
								double num6 = num;
								double num7 = 0.017453292519943295 * num6;
								ColumnChart.SetRotation(num5, num6, num7, centerOfRotation, num2, num3, title);
							}
						}
						else
						{
							centerOfRotation = new Point(markerLeft, markerTop + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0);
							if (num >= -90.0 && num < 0.0)
							{
								double num6 = 180.0 + num;
								double num7 = 0.017453292519943295 * num6;
								num5 += title.TextBlockDesiredSize.Width + 3.0;
								num6 = (num7 - 3.1415926535897931) * 57.295779513082323;
								ColumnChart.SetRotation(num5, num6, num7, centerOfRotation, num2, num3, title);
							}
							else if (num > 0.0 && num <= 90.0)
							{
								double num6 = num;
								double num7 = 0.017453292519943295 * num6;
								ColumnChart.SetRotation(num5, num6, num7, centerOfRotation, num2, num3, title);
							}
						}
					}
					else
					{
						Point centerOfRotation2 = default(Point);
						double num8 = dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						if (labelStyles2 == LabelStyles.OutSide)
						{
							centerOfRotation2 = new Point(markerLeft, markerTop + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0);
							if (num >= -90.0 && num < 0.0)
							{
								double num9 = 180.0 + num;
								double num10 = 0.017453292519943295 * num9;
								num8 += title.TextBlockDesiredSize.Width;
								num9 = (num10 - 3.1415926535897931) * 57.295779513082323;
								ColumnChart.SetRotation(num8, num9, num10, centerOfRotation2, num2, num3, title);
							}
							else if (num > 0.0 && num <= 90.0)
							{
								double num9 = num;
								double num10 = 0.017453292519943295 * num9;
								ColumnChart.SetRotation(num8, num9, num10, centerOfRotation2, num2, num3, title);
							}
						}
						else
						{
							centerOfRotation2 = new Point(markerLeft, markerTop - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0);
							if (num > 0.0 && num <= 90.0)
							{
								double num9 = num - 180.0;
								double num10 = 0.017453292519943295 * num9;
								num8 += title.TextBlockDesiredSize.Width + 3.0;
								num9 = (num10 - 3.1415926535897931) * 57.295779513082323;
								ColumnChart.SetRotation(num8, num9, num10, centerOfRotation2, num2, num3, title);
							}
							else if (num >= -90.0 && num < 0.0)
							{
								double num9 = num;
								double num10 = 0.017453292519943295 * num9;
								ColumnChart.SetRotation(num8, num9, num10, centerOfRotation2, num2, num3, title);
							}
						}
					}
				}
				if (labelStyles2 != labelStyles)
				{
					title.TextElement.Foreground = Chart.CalculateDataPointLabelFontColor(dataPoint.Chart as Chart, dataPoint, labelFontColor, (dataPoint.DpInfo.InternalYValue <= 0.0) ? LabelStyles.OutSide : labelStyles2);
				}
				dataPoint.DpInfo.LabelVisual = title.Visual;
				dataPoint.DpInfo.LabelVisual.Width = title.TextBlockDesiredSize.Width;
				dataPoint.DpInfo.LabelVisual.Height = title.TextBlockDesiredSize.Height;
				labelCanvas.Children.Add(title.Visual);
			}
			return result;
		}

		private static void SetLabelPosition4LineDataPoint(IDataPoint dataPoint, double plotWidth, double plotHeight, bool isPositive, double markerLeft, double markerTop, ref double labelLeft, ref double labelTop, double gap, Size textBlockSize)
		{
			Point point = new Point(markerLeft, markerTop);
			Point visualPosition = new Point(0.0, 0.0);
			Point visualPosition2 = new Point(0.0, 0.0);
			if (dataPoint.DpInfo.Faces.PreviousDataPoint != null)
			{
				visualPosition = dataPoint.DpInfo.Faces.PreviousDataPoint.DpInfo.VisualPosition;
			}
			if (dataPoint.DpInfo.Faces.NextDataPoint != null)
			{
				visualPosition2 = dataPoint.DpInfo.Faces.NextDataPoint.DpInfo.VisualPosition;
			}
			bool flag = false;
			LabelStyles labelStyles = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			if (dataPoint.DpInfo.Marker != null)
			{
				if (isPositive)
				{
					if (labelStyles == LabelStyles.OutSide && !dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
					{
						if (point.Y - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 < 0.0 || labelStyles == LabelStyles.Inside)
						{
							if (point.X + textBlockSize.Width > plotWidth && visualPosition.Y - point.Y <= 50.0 && textBlockSize.Width < 50.0)
							{
								labelLeft = point.X - textBlockSize.Width / 2.0;
								labelTop = point.Y + gap + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if ((point.X + textBlockSize.Width > plotWidth && visualPosition.Y - point.Y > 50.0) || point.X + textBlockSize.Width > plotWidth)
							{
								labelLeft = point.X - textBlockSize.Width - gap - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y - gap + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (visualPosition.Y > point.Y && visualPosition2.Y > point.Y && visualPosition.Y - point.Y > 20.0 && visualPosition.Y - point.Y > visualPosition2.Y - point.Y)
							{
								if (point.X - textBlockSize.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor <= 2.0)
								{
									labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
									labelTop = point.Y - textBlockSize.Height / 2.0;
								}
								else
								{
									labelLeft = point.X - textBlockSize.Width - gap - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
									labelTop = point.Y - textBlockSize.Height / 2.0;
								}
							}
							else if (visualPosition2.X - point.X > 120.0 && textBlockSize.Width < 50.0)
							{
								labelLeft = point.X - textBlockSize.Width / 2.0;
								labelTop = point.Y + gap + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y > visualPosition.Y && point.Y - visualPosition.Y > point.Y - visualPosition2.Y && visualPosition.X != 0.0)
							{
								labelLeft = point.X - textBlockSize.Width - gap - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y - gap + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y >= visualPosition2.Y && visualPosition == new Point(0.0, 0.0))
							{
								labelLeft = point.X - textBlockSize.Width / 2.0;
								labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y >= visualPosition2.Y && visualPosition.Y >= point.Y)
							{
								labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y <= visualPosition2.Y && visualPosition.Y >= point.Y)
							{
								if (textBlockSize.Width < 15.0)
								{
									labelLeft = point.X - textBlockSize.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 - 2.0;
									labelTop = point.Y - gap / 2.0 - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								}
								else
								{
									labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
									labelTop = point.Y + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								}
							}
							else if (visualPosition == new Point(0.0, 0.0) && point.X - textBlockSize.Width / 2.0 > 0.0)
							{
								labelLeft = point.X - textBlockSize.Width / 2.0;
								labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else
							{
								labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y - textBlockSize.Height / 2.0;
							}
							flag = true;
						}
						else if (point.Y + textBlockSize.Height + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 > plotHeight || labelStyles == LabelStyles.Inside)
						{
							if (point.X + textBlockSize.Width > plotWidth && visualPosition.Y - point.Y <= 50.0 && textBlockSize.Width < 50.0)
							{
								labelLeft = point.X - textBlockSize.Width / 2.0;
								labelTop = point.Y - gap / 2.0 - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if ((point.X + textBlockSize.Width > plotWidth && visualPosition.Y - point.Y > 50.0) || point.X + textBlockSize.Width > plotWidth)
							{
								labelLeft = point.X - gap - textBlockSize.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (visualPosition.Y > point.Y && visualPosition2.Y > point.Y && visualPosition.Y - point.Y > 20.0 && visualPosition.Y - point.Y > visualPosition2.Y - point.Y)
							{
								labelLeft = point.X - textBlockSize.Width - gap - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (visualPosition2.X - point.X > 120.0 && textBlockSize.Width < 50.0 && visualPosition.X == 0.0)
							{
								labelLeft = point.X - textBlockSize.Width / 2.0;
								labelTop = point.Y - gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y > visualPosition.Y && point.Y - visualPosition.Y > point.Y - visualPosition2.Y && visualPosition.X != 0.0)
							{
								labelLeft = point.X - textBlockSize.Width - gap - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y >= visualPosition2.Y && visualPosition == new Point(0.0, 0.0))
							{
								labelLeft = point.X - textBlockSize.Width / 2.0;
								labelTop = point.Y - gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y >= visualPosition2.Y && visualPosition.Y >= point.Y)
							{
								if (point.Y - visualPosition2.Y >= 10.0)
								{
									labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
									labelTop = point.Y + gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								}
								else
								{
									labelLeft = point.X - textBlockSize.Width / 2.0;
									labelTop = point.Y - gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								}
							}
							else if (point.Y <= visualPosition2.Y && visualPosition.Y >= point.Y)
							{
								if (textBlockSize.Width < 20.0)
								{
									labelLeft = point.X - textBlockSize.Width / 2.0;
									labelTop = point.Y - gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								}
								else
								{
									labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
									labelTop = point.Y + gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								}
							}
							else if (visualPosition == new Point(0.0, 0.0) && point.X - textBlockSize.Width / 2.0 > 0.0)
							{
								labelLeft = point.X - textBlockSize.Width / 2.0;
								labelTop = point.Y - gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else
							{
								labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							flag = true;
						}
					}
					else if (labelStyles == LabelStyles.OutSide)
					{
						labelLeft = point.X - textBlockSize.Width / 2.0;
						labelTop = point.Y - gap / 2.0 - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						flag = true;
					}
					else
					{
						labelLeft = point.X - textBlockSize.Width / 2.0;
						labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						flag = true;
					}
				}
				else if (labelStyles == LabelStyles.OutSide && !dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
				{
					if (point.Y + textBlockSize.Height + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 > plotHeight || labelStyles == LabelStyles.Inside)
					{
						if (point.X + textBlockSize.Width > plotWidth && visualPosition.Y - point.Y <= 50.0 && textBlockSize.Width < 50.0)
						{
							labelLeft = point.X - textBlockSize.Width / 2.0;
							labelTop = point.Y - gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if ((point.X + textBlockSize.Width > plotWidth && visualPosition.Y - point.Y > 50.0) || point.X + textBlockSize.Width > plotWidth)
						{
							labelLeft = point.X - gap - textBlockSize.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (visualPosition.Y > point.Y && visualPosition2.Y > point.Y && visualPosition.Y - point.Y > 20.0 && visualPosition.Y - point.Y > visualPosition2.Y - point.Y)
						{
							labelLeft = point.X - textBlockSize.Width - gap - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (visualPosition2.X - point.X > 100.0 && textBlockSize.Width < 50.0 && visualPosition.X == 0.0)
						{
							labelLeft = point.X - textBlockSize.Width / 2.0;
							labelTop = point.Y - gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y > visualPosition.Y && point.Y > visualPosition2.Y && point.Y - visualPosition.Y > point.Y - visualPosition2.Y && visualPosition.X != 0.0)
						{
							labelLeft = point.X - textBlockSize.Width - gap - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y >= visualPosition2.Y && visualPosition == new Point(0.0, 0.0))
						{
							labelLeft = point.X - textBlockSize.Width / 2.0;
							labelTop = point.Y - gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y >= visualPosition2.Y && visualPosition.Y >= point.Y)
						{
							if (point.Y - visualPosition2.Y >= 10.0)
							{
								labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else
							{
								labelLeft = point.X - textBlockSize.Width / 2.0;
								labelTop = point.Y - gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
						}
						else if (point.Y <= visualPosition2.Y && visualPosition.Y >= point.Y)
						{
							if (textBlockSize.Width < 20.0 || visualPosition.Y - point.Y < 20.0)
							{
								labelLeft = point.X - textBlockSize.Width / 2.0;
								labelTop = point.Y - gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else
							{
								labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
						}
						else if (visualPosition == new Point(0.0, 0.0) && point.X - textBlockSize.Width / 2.0 > 0.0)
						{
							labelLeft = point.X - textBlockSize.Width / 2.0;
							labelTop = point.Y - gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else
						{
							labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + gap - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						flag = true;
					}
					else if (point.Y - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 < 0.0 || labelStyles == LabelStyles.Inside)
					{
						if (point.X + textBlockSize.Width > plotWidth && visualPosition.Y - point.Y <= 50.0 && textBlockSize.Width < 50.0)
						{
							labelLeft = point.X - textBlockSize.Width / 2.0;
							labelTop = point.Y + gap + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if ((point.X + textBlockSize.Width > plotWidth && visualPosition.Y - point.Y > 50.0) || point.X + textBlockSize.Width > plotWidth)
						{
							labelLeft = point.X - textBlockSize.Width - gap - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - gap + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (visualPosition.Y > point.Y && visualPosition2.Y > point.Y && visualPosition.Y - point.Y > 20.0 && visualPosition.Y - point.Y > visualPosition2.Y - point.Y)
						{
							labelLeft = point.X - textBlockSize.Width - gap - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - gap + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (visualPosition2.X - point.X > 100.0 && textBlockSize.Width < 50.0 && visualPosition2.Y - point.Y < 20.0)
						{
							labelLeft = point.X - textBlockSize.Width / 2.0;
							labelTop = point.Y + gap + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y > visualPosition.Y && point.Y - visualPosition.Y > point.Y - visualPosition2.Y && visualPosition.X != 0.0)
						{
							labelLeft = point.X - textBlockSize.Width - gap - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - gap + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y >= visualPosition2.Y && visualPosition == new Point(0.0, 0.0))
						{
							labelLeft = point.X - textBlockSize.Width / 2.0;
							labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y >= visualPosition2.Y && visualPosition.Y >= point.Y)
						{
							labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y <= visualPosition2.Y && visualPosition.Y >= point.Y)
						{
							if (textBlockSize.Width < 15.0)
							{
								labelLeft = point.X - textBlockSize.Width / 2.0;
								labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else
							{
								labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
						}
						else if (visualPosition == new Point(0.0, 0.0) && point.X - textBlockSize.Width / 2.0 > 0.0)
						{
							labelLeft = point.X - textBlockSize.Width / 2.0;
							labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else
						{
							labelLeft = point.X + gap + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - gap + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						flag = true;
					}
				}
				else if (labelStyles == LabelStyles.OutSide)
				{
					labelLeft = point.X - textBlockSize.Width / 2.0;
					labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
					flag = true;
				}
				else
				{
					labelLeft = point.X - textBlockSize.Width / 2.0;
					labelTop = point.Y - gap / 2.0 - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
					flag = true;
				}
				if (!flag && labelStyles == LabelStyles.OutSide && !dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
				{
					if (visualPosition.Y <= point.Y && point.X + textBlockSize.Width >= plotWidth && visualPosition2.X == 0.0 && visualPosition2.Y == 0.0)
					{
						labelLeft = point.X - textBlockSize.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						labelTop = point.Y + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						return;
					}
					if (visualPosition.Y > point.Y && point.X + textBlockSize.Width > plotWidth && visualPosition2.X == 0.0 && visualPosition2.Y == 0.0)
					{
						labelLeft = point.X - textBlockSize.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						labelTop = point.Y - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						return;
					}
					if (point.X + textBlockSize.Width > plotWidth)
					{
						labelLeft = point.X - gap - textBlockSize.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						labelTop = point.Y - textBlockSize.Height / 2.0;
						return;
					}
					if (visualPosition.Y <= point.Y && visualPosition2.Y <= point.Y && visualPosition2 != new Point(0.0, 0.0) && visualPosition != new Point(0.0, 0.0))
					{
						labelLeft = point.X - textBlockSize.Width / 2.0;
						labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						return;
					}
					if (visualPosition.Y > point.Y && visualPosition2.Y > point.Y)
					{
						labelLeft = point.X - textBlockSize.Width / 2.0;
						labelTop = point.Y - gap / 2.0 - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						return;
					}
					if (visualPosition.X == 0.0 && visualPosition.Y == 0.0)
					{
						if (point.Y > visualPosition2.Y && point.Y - visualPosition2.Y > 20.0 && point.X - textBlockSize.Width / 2.0 < 0.0)
						{
							labelLeft = point.X + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							return;
						}
						if (point.Y > visualPosition2.Y && point.Y - visualPosition2.Y > 20.0)
						{
							if (point.X - textBlockSize.Width >= 0.0)
							{
								labelLeft = point.X - textBlockSize.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y - gap / 2.0 - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								return;
							}
							labelLeft = point.X - textBlockSize.Width / 2.0;
							labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							return;
						}
						else
						{
							if (point.X - textBlockSize.Width / 2.0 > 0.0)
							{
								labelLeft = point.X - textBlockSize.Width / 2.0;
								labelTop = point.Y - gap / 2.0 - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								return;
							}
							if (point.Y > visualPosition2.Y)
							{
								labelLeft = point.X + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								return;
							}
							labelLeft = point.X + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							return;
						}
					}
					else if (visualPosition2.X == 0.0 && visualPosition2.Y == 0.0 && point.X + textBlockSize.Width > plotWidth)
					{
						if (point.Y > visualPosition.Y)
						{
							labelLeft = point.X - textBlockSize.Width / 2.0;
							labelTop = point.Y + gap / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							return;
						}
						labelLeft = point.X - textBlockSize.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						labelTop = point.Y - gap / 2.0 - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						return;
					}
					else
					{
						if (visualPosition2.X == 0.0 && visualPosition2.Y == 0.0)
						{
							labelLeft = point.X + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - gap / 2.0 - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							return;
						}
						if ((visualPosition.Y <= point.Y && visualPosition2.Y >= point.Y) || (visualPosition.Y > point.Y && visualPosition2.Y < point.Y))
						{
							if (visualPosition.Y <= point.Y && visualPosition2.Y >= point.Y && (visualPosition2.Y - point.Y < 10.0 || visualPosition.Y < point.Y))
							{
								labelLeft = point.X + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y - gap / 2.0 - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								return;
							}
							if (point.Y - visualPosition2.Y > 20.0)
							{
								labelLeft = point.X - gap - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor - textBlockSize.Width / 2.0;
								labelTop = point.Y - textBlockSize.Height / 2.0;
								return;
							}
							labelLeft = point.X - textBlockSize.Width / 2.0;
							labelTop = point.Y - gap / 2.0 - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							return;
						}
						else
						{
							labelLeft = point.X - textBlockSize.Width / 2.0;
							labelTop = point.Y - gap / 2.0 - textBlockSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
					}
				}
			}
		}

		private static Canvas GetStepLine2D(DataSeries tagReference, double width, double height, Canvas line2dLabelCanvas, StepLineChartShapeParams lineParams, out Path line, out Path lineShadow, List<List<IDataPoint>> pointCollectionList, List<List<IDataPoint>> shadowPointCollectionList)
		{
			Canvas canvas = new Canvas();
			line = new Path
			{
				Tag = new ElementData
				{
					Element = tagReference
				}
			};
			line.StrokeLineJoin = PenLineJoin.Round;
			line.StrokeStartLineCap = tagReference.LineCap;
			line.StrokeEndLineCap = tagReference.LineCap;
			line.Stroke = (lineParams.Lighting ? Graphics.GetLightingEnabledBrush(lineParams.LineColor, "Linear", new double[]
			{
				0.65,
				0.55
			}) : lineParams.LineColor);
			line.StrokeThickness = lineParams.LineThickness;
			line.StrokeDashArray = lineParams.LineStyle;
			line.Opacity = lineParams.Opacity;
			line.Data = StepLineChart.GetPathGeometry(null, pointCollectionList, false, width, height, line2dLabelCanvas);
			if (VisifireControl.IsMediaEffectsEnabled)
			{
				if (tagReference.Effect != null)
				{
					canvas.Effect = tagReference.Effect;
				}
				lineShadow = null;
			}
			if (lineParams.ShadowEnabled)
			{
				if (!VisifireControl.IsMediaEffectsEnabled)
				{
					lineShadow = new Path
					{
						IsHitTestVisible = false
					};
					lineShadow.Stroke = Graphics.GetLightingEnabledBrush(new SolidColorBrush(Colors.LightGray), "Linear", new double[]
					{
						0.65,
						0.55
					});
					lineShadow.StrokeStartLineCap = tagReference.LineCap;
					lineShadow.StrokeEndLineCap = tagReference.LineCap;
					lineShadow.StrokeLineJoin = PenLineJoin.Round;
					lineShadow.StrokeThickness = lineParams.LineThickness;
					lineShadow.Opacity = 0.5;
					if (lineParams.ShadowEnabled)
					{
						lineShadow.Data = StepLineChart.GetPathGeometry(null, shadowPointCollectionList, true, width, height, null);
					}
					TranslateTransform renderTransform = new TranslateTransform
					{
						X = 2.0,
						Y = 2.0
					};
					lineShadow.RenderTransform = renderTransform;
					canvas.Children.Add(lineShadow);
				}
				else
				{
					if (tagReference.Effect == null)
					{
						canvas.Effect = ExtendedGraphics.GetShadowEffect(315.0, 2.5, 1.0);
					}
					lineShadow = null;
				}
			}
			else
			{
				lineShadow = null;
			}
			canvas.Children.Add(line);
			return canvas;
		}

		private static Geometry GetPathGeometry(GeometryGroup oldData, List<List<IDataPoint>> dataPointCollectionList, bool isShadow, double width, double height, Canvas line2dLabelCanvas)
		{
			GeometryGroup geometryGroup;
			if (oldData != null)
			{
				geometryGroup = oldData;
				geometryGroup.Children.Clear();
			}
			else
			{
				geometryGroup = new GeometryGroup();
			}
			foreach (List<IDataPoint> current in dataPointCollectionList)
			{
				PathGeometry pathGeometry = new PathGeometry();
				PathFigure pathFigure = new PathFigure();
				double markerLeft = 0.0;
				double markerTop = 0.0;
				if (current.Count > 0)
				{
					pathFigure.StartPoint = current[0].DpInfo.VisualPosition;
					Faces faces = new Faces();
					faces.Parts.Add(null);
					faces.Parts.Add(pathFigure);
					if (isShadow)
					{
						current[0].DpInfo.ShadowFaces = faces;
					}
					else
					{
						current[0].DpInfo.Faces = faces;
						faces.PreviousDataPoint = null;
						if (current.Count > 1)
						{
							faces.NextDataPoint = current[1];
						}
						if (current[0].DpInfo.Marker != null && current[0].DpInfo.Marker.Visual != null)
						{
							Point point = current[0].DpInfo.Marker.CalculateActualPosition(current[0].DpInfo.VisualPosition.X, current[0].DpInfo.VisualPosition.Y, new Point(0.5, 0.5));
							current[0].DpInfo.ParsedToolTipText = current[0].TextParser(current[0].ToolTipText);
							if (!current[0].IsLightDataPoint)
							{
								(current[0] as DataPoint).ParsedHref = current[0].TextParser((string)DataPointHelper.GetDataPointValueFromProperty(current[0], VcProperties.Href));
							}
							current[0].DpInfo.Marker.Visual.Visibility = Visibility.Visible;
							current[0].DpInfo.Marker.Visual.SetValue(Canvas.TopProperty, point.Y);
							current[0].DpInfo.Marker.Visual.SetValue(Canvas.LeftProperty, point.X);
							double internalYValue = current[0].DpInfo.InternalYValue;
							if (current[0].Parent != null && current[0].Parent.PlotGroup.AxisY.AxisMinimum != null)
							{
								double limitingYValue = current[0].Parent.PlotGroup.GetLimitingYValue();
								if (internalYValue > current[0].Parent.PlotGroup.AxisY.InternalAxisMaximum || (internalYValue < limitingYValue && limitingYValue > 0.0) || (internalYValue < limitingYValue && current[0].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue < current[0].Parent.PlotGroup.AxisY.InternalAxisMinimum)
								{
									current[0].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
								}
							}
							if (current[0].Parent != null && current[0].Parent.PlotGroup.AxisY.AxisMaximum != null && internalYValue > current[0].Parent.PlotGroup.AxisY.InternalAxisMaximum)
							{
								current[0].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
							}
						}
						else
						{
							LineChart.CreateMarkerAForLineDataPoint(current[0], width, height, ref line2dLabelCanvas, out markerLeft, out markerTop);
							double internalYValue2 = current[0].DpInfo.InternalYValue;
							if (current[0].Parent != null && current[0].Parent.PlotGroup.AxisY.AxisMinimum != null)
							{
								double limitingYValue2 = current[0].Parent.PlotGroup.GetLimitingYValue();
								if (internalYValue2 > current[0].Parent.PlotGroup.AxisY.InternalAxisMaximum || (internalYValue2 < limitingYValue2 && limitingYValue2 > 0.0) || (internalYValue2 < limitingYValue2 && current[0].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue2 < current[0].Parent.PlotGroup.AxisY.InternalAxisMinimum)
								{
									current[0].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
								}
							}
							if (current[0].Parent != null && current[0].Parent.PlotGroup.AxisY.AxisMaximum != null && internalYValue2 > current[0].Parent.PlotGroup.AxisY.InternalAxisMaximum)
							{
								current[0].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
							}
						}
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(current[0], VcProperties.LabelEnabled))
						{
							if (current[0].DpInfo.LabelVisual != null)
							{
								double num = 0.0;
								double num2 = 0.0;
								StepLineChart.CreateLabel4LineDataPoint(current[0], width, height, current[0].DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref line2dLabelCanvas, true);
								StepLineChart.SetLabelPosition4LineDataPoint(current[0], width, height, current[0].DpInfo.InternalYValue >= 0.0, current[0].DpInfo.VisualPosition.X, current[0].DpInfo.VisualPosition.Y, ref num, ref num2, 6.0, new Size(current[0].DpInfo.LabelVisual.Width, current[0].DpInfo.LabelVisual.Height));
								(((current[0].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Text = current[0].TextParser((string)DataPointHelper.GetDataPointValueFromProperty(current[0], VcProperties.LabelText));
								current[0].DpInfo.LabelVisual.Visibility = Visibility.Visible;
								current[0].DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, num);
								current[0].DpInfo.LabelVisual.SetValue(Canvas.TopProperty, num2);
							}
							else
							{
								StepLineChart.CreateLabel4LineDataPoint(current[0], width, height, current[0].DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref line2dLabelCanvas, true);
								double internalYValue3 = current[0].DpInfo.InternalYValue;
								if (current[0].Parent != null && current[0].Parent.PlotGroup.AxisY.AxisMinimum != null)
								{
									double limitingYValue3 = current[0].Parent.PlotGroup.GetLimitingYValue();
									if (((internalYValue3 < limitingYValue3 && limitingYValue3 > 0.0) || (internalYValue3 < limitingYValue3 && current[0].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue3 < current[0].Parent.PlotGroup.AxisY.InternalAxisMinimum) && current[0].DpInfo.LabelVisual != null)
									{
										(((current[0].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Collapsed;
									}
								}
								if (current[0].Parent != null && current[0].Parent.PlotGroup.AxisY.AxisMaximum != null && internalYValue3 > current[0].Parent.PlotGroup.AxisY.InternalAxisMaximum && current[0].DpInfo.LabelVisual != null)
								{
									(((current[0].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Collapsed;
								}
							}
						}
					}
					for (int i = 1; i < current.Count; i++)
					{
						LineSegment lineSegment = new LineSegment();
						lineSegment.Point = new Point
						{
							X = current[i].DpInfo.VisualPosition.X,
							Y = current[i - 1].DpInfo.VisualPosition.Y
						};
						LineSegment lineSegment2 = new LineSegment();
						lineSegment2.Point = current[i].DpInfo.VisualPosition;
						faces = new Faces();
						faces.PreviousDataPoint = current[i - 1];
						if (!isShadow)
						{
							if (i != current.Count - 1)
							{
								faces.NextDataPoint = current[i + 1];
							}
							else
							{
								faces.NextDataPoint = null;
							}
						}
						faces.Parts.Add(lineSegment);
						faces.Parts.Add(lineSegment2);
						faces.Parts.Add(pathFigure);
						if (isShadow)
						{
							current[i].DpInfo.ShadowFaces = faces;
						}
						else
						{
							current[i].DpInfo.Faces = faces;
						}
						pathFigure.Segments.Add(lineSegment);
						pathFigure.Segments.Add(lineSegment2);
						if (!isShadow)
						{
							if (current[i].DpInfo.Marker != null && current[i].DpInfo.Marker.Visual != null)
							{
								Point point2 = current[i].DpInfo.Marker.CalculateActualPosition(current[i].DpInfo.VisualPosition.X, current[i].DpInfo.VisualPosition.Y, new Point(0.5, 0.5));
								current[i].DpInfo.ParsedToolTipText = current[i].TextParser(current[i].ToolTipText);
								if (!current[i].IsLightDataPoint)
								{
									(current[i] as DataPoint).ParsedHref = current[i].TextParser((string)DataPointHelper.GetDataPointValueFromProperty(current[i], VcProperties.Href));
								}
								current[i].DpInfo.Marker.Visual.Visibility = Visibility.Visible;
								current[i].DpInfo.Marker.Visual.SetValue(Canvas.TopProperty, point2.Y);
								current[i].DpInfo.Marker.Visual.SetValue(Canvas.LeftProperty, point2.X);
								double internalYValue4 = current[i].DpInfo.InternalYValue;
								if (current[i].Parent != null && current[i].Parent.PlotGroup.AxisY.AxisMinimum != null)
								{
									double limitingYValue4 = current[i].Parent.PlotGroup.GetLimitingYValue();
									if (internalYValue4 > current[i].Parent.PlotGroup.AxisY.InternalAxisMaximum || (internalYValue4 < limitingYValue4 && limitingYValue4 > 0.0) || (internalYValue4 < limitingYValue4 && current[i].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue4 < current[i].Parent.PlotGroup.AxisY.InternalAxisMinimum)
									{
										current[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
									}
								}
								if (current[i].Parent != null && current[i].Parent.PlotGroup.AxisY.AxisMaximum != null && internalYValue4 > current[i].Parent.PlotGroup.AxisY.InternalAxisMaximum)
								{
									current[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
								}
							}
							else
							{
								LineChart.CreateMarkerAForLineDataPoint(current[i], width, height, ref line2dLabelCanvas, out markerLeft, out markerTop);
								if (current[i].Parent != null && current[i].Parent.PlotGroup.AxisY.AxisMinimum != null)
								{
									double limitingYValue5 = current[i].Parent.PlotGroup.GetLimitingYValue();
									double internalYValue5 = current[i].DpInfo.InternalYValue;
									if (internalYValue5 > current[i].Parent.PlotGroup.AxisY.InternalAxisMaximum || (internalYValue5 < limitingYValue5 && limitingYValue5 > 0.0) || (internalYValue5 < limitingYValue5 && current[i].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue5 < current[i].Parent.PlotGroup.AxisY.InternalAxisMinimum)
									{
										current[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
									}
									if (current[i].Parent.PlotGroup.AxisY.AxisMaximum != null && internalYValue5 > current[i].Parent.PlotGroup.AxisY.InternalAxisMaximum)
									{
										current[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
									}
								}
							}
							if ((bool)DataPointHelper.GetDataPointValueFromProperty(current[i], VcProperties.LabelEnabled))
							{
								if (current[i].DpInfo.LabelVisual != null)
								{
									double num3 = 0.0;
									double num4 = 0.0;
									StepLineChart.CreateLabel4LineDataPoint(current[i], width, height, current[i].DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref line2dLabelCanvas, true);
									StepLineChart.SetLabelPosition4LineDataPoint(current[i], width, height, current[i].DpInfo.InternalYValue >= 0.0, current[i].DpInfo.VisualPosition.X, current[i].DpInfo.VisualPosition.Y, ref num3, ref num4, 6.0, new Size(current[i].DpInfo.LabelVisual.Width, current[i].DpInfo.LabelVisual.Height));
									(((current[i].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Text = current[i].TextParser((string)DataPointHelper.GetDataPointValueFromProperty(current[i], VcProperties.LabelText));
									current[i].DpInfo.LabelVisual.Visibility = Visibility.Visible;
									current[i].DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, num3);
									current[i].DpInfo.LabelVisual.SetValue(Canvas.TopProperty, num4);
								}
								else
								{
									StepLineChart.CreateLabel4LineDataPoint(current[i], width, height, current[i].DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref line2dLabelCanvas, true);
									double internalYValue6 = current[i].DpInfo.InternalYValue;
									if (current[i].Parent != null && current[i].Parent.PlotGroup.AxisY.AxisMinimum != null)
									{
										double limitingYValue6 = current[i].Parent.PlotGroup.GetLimitingYValue();
										if (((internalYValue6 < limitingYValue6 && limitingYValue6 > 0.0) || (internalYValue6 < limitingYValue6 && current[i].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue6 < current[i].Parent.PlotGroup.AxisY.InternalAxisMinimum) && current[i].DpInfo.LabelVisual != null)
										{
											(((current[i].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Collapsed;
										}
									}
									if (current[i].Parent != null && current[i].Parent.PlotGroup.AxisY.AxisMaximum != null && internalYValue6 > current[i].Parent.PlotGroup.AxisY.InternalAxisMaximum && current[i].DpInfo.LabelVisual != null)
									{
										(((current[i].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Collapsed;
									}
								}
							}
						}
					}
				}
				pathGeometry.Figures.Add(pathFigure);
				geometryGroup.Children.Add(pathGeometry);
			}
			return geometryGroup;
		}

		public static void Update(IElement sender, ElementTypes elementType, VcProperties property, object newValue, bool isAxisChanged)
		{
			bool flag = elementType == ElementTypes.DataPoint;
			if (flag)
			{
				StepLineChart.UpdateDataPoint(sender as IDataPoint, property, newValue);
				return;
			}
			StepLineChart.UpdateDataSeries(sender as DataSeries, property, newValue);
		}

		private static void UpdateDataSeries(IElement obj, VcProperties property, object newValue)
		{
			DataSeries dataSeries = obj as DataSeries;
			if (dataSeries != null)
			{
				dataSeries.isSeriesRenderPending = false;
			}
			bool flag = false;
			bool isOffsetAdded = true;
			if (dataSeries == null)
			{
				flag = true;
				IDataPoint dataPoint = obj as IDataPoint;
				if (dataPoint == null)
				{
					return;
				}
				dataSeries = dataPoint.Parent;
				dataSeries.isSeriesRenderPending = false;
			}
			if (dataSeries == null)
			{
				return;
			}
			Chart chart = dataSeries.Chart as Chart;
			if (chart == null)
			{
				return;
			}
			PlotGroup plotGroup = dataSeries.PlotGroup;
			Canvas canvas = null;
			Path path = null;
			Path path2 = null;
			if (dataSeries.Faces == null)
			{
				if (dataSeries.Faces == null && property == VcProperties.Enabled && (bool)newValue)
				{
					ColumnChart.Update(chart, RenderAs.StepLine, (from ds in chart.InternalSeries
					where ds.RenderAs == RenderAs.StepLine
					select ds).ToList<DataSeries>());
				}
				return;
			}
			if (dataSeries.Faces.Parts.Count > 0)
			{
				path = (dataSeries.Faces.Parts[0] as Path);
				if (dataSeries.Faces.Parts.Count > 1)
				{
					path2 = (dataSeries.Faces.Parts[1] as Path);
				}
			}
			if (dataSeries.Faces.Visual == null || dataSeries.Faces.LabelCanvas == null)
			{
				return;
			}
			Canvas canvas2 = dataSeries.Faces.Visual as Canvas;
			canvas = dataSeries.Faces.LabelCanvas;
			double height = chart.ChartArea.ChartVisualCanvas.Height;
			double width = chart.ChartArea.ChartVisualCanvas.Width;
			if (property <= VcProperties.LineCap)
			{
				if (property <= VcProperties.Color)
				{
					if (property != VcProperties.ViewportRangeEnabled)
					{
						if (property != VcProperties.Color)
						{
							return;
						}
						if (path != null)
						{
							Brush brush = (newValue != null) ? (newValue as Brush) : dataSeries.Color;
							path.Stroke = (dataSeries.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(brush, "Linear", new double[]
							{
								0.65,
								0.55
							}) : brush);
							return;
						}
						return;
					}
				}
				else if (property != VcProperties.DataPoints)
				{
					switch (property)
					{
					case VcProperties.Effect:
						break;
					case VcProperties.Enabled:
						if (!flag && canvas2 != null)
						{
							if (!(bool)newValue)
							{
								canvas2.Visibility = Visibility.Collapsed;
								canvas.Visibility = Visibility.Collapsed;
							}
							else
							{
								if (canvas2.Parent == null)
								{
									ColumnChart.Update(chart, RenderAs.StepLine, (from ds in chart.InternalSeries
									where ds.RenderAs == RenderAs.StepLine
									select ds).ToList<DataSeries>());
									return;
								}
								canvas2.Visibility = Visibility.Visible;
								canvas.Visibility = Visibility.Visible;
							}
							chart._toolTip.Hide();
							return;
						}
						break;
					default:
						switch (property)
						{
						case VcProperties.LightingEnabled:
							if (path != null && newValue != null)
							{
								path.Stroke = (((bool)newValue) ? Graphics.GetLightingEnabledBrush(dataSeries.Color, "Linear", new double[]
								{
									0.65,
									0.55
								}) : dataSeries.Color);
								return;
							}
							return;
						case VcProperties.LightWeight:
							return;
						case VcProperties.LineCap:
							if (path2 != null)
							{
								path2.StrokeStartLineCap = dataSeries.LineCap;
								path2.StrokeEndLineCap = dataSeries.LineCap;
							}
							if (path != null)
							{
								path.StrokeStartLineCap = dataSeries.LineCap;
								path.StrokeEndLineCap = dataSeries.LineCap;
								return;
							}
							return;
						default:
							return;
						}
						break;
					}
				}
			}
			else if (property <= VcProperties.Opacity)
			{
				switch (property)
				{
				case VcProperties.LineStyle:
				case VcProperties.LineThickness:
					if (path2 != null)
					{
						path2.StrokeThickness = dataSeries.LineThickness.Value;
					}
					if (path != null)
					{
						path.StrokeThickness = dataSeries.LineThickness.Value;
					}
					if (path2 != null)
					{
						path2.StrokeDashArray = ExtendedGraphics.GetDashArray(dataSeries.LineStyle);
					}
					if (path != null)
					{
						path.StrokeDashArray = ExtendedGraphics.GetDashArray(dataSeries.LineStyle);
						return;
					}
					return;
				case VcProperties.LineTension:
					return;
				default:
					if (property != VcProperties.Opacity)
					{
						return;
					}
					if (path != null)
					{
						path.Opacity = dataSeries.Opacity;
						return;
					}
					return;
				}
			}
			else if (property != VcProperties.ShadowEnabled && property != VcProperties.XValue)
			{
				switch (property)
				{
				case VcProperties.YValue:
				case VcProperties.YValues:
					break;
				case VcProperties.YValueFormatString:
					return;
				default:
					return;
				}
			}
			if (dataSeries.Enabled == false)
			{
				return;
			}
			dataSeries.StopDataPointsAnimation();
			Axis axisX = plotGroup.AxisX;
			Axis axisY = plotGroup.AxisY;
			(dataSeries.Faces.Visual as Canvas).Width = width;
			(dataSeries.Faces.Visual as Canvas).Height = height;
			dataSeries.Faces.LabelCanvas.Width = width;
			dataSeries.Faces.LabelCanvas.Height = height;
			Canvas canvas3 = dataSeries.Faces.Visual.Parent as Canvas;
			Canvas canvas4 = dataSeries.Faces.LabelCanvas.Parent as Canvas;
			canvas3.Width = width;
			canvas3.Height = height;
			canvas4.Width = width;
			canvas4.Height = height;
			List<IDataPoint> list = new List<IDataPoint>();
			List<List<IDataPoint>> list2 = new List<List<IDataPoint>>();
			list2.Add(list);
			foreach (IDataPoint current in dataSeries.InternalDataPoints)
			{
				if (current.DpInfo.Marker != null && current.DpInfo.Marker.Visual != null)
				{
					current.DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
				}
				if (current.DpInfo.LabelVisual != null)
				{
					current.DpInfo.LabelVisual.Visibility = Visibility.Collapsed;
				}
				if (double.IsNaN(current.DpInfo.InternalYValue))
				{
					current.DpInfo.Faces = null;
				}
			}
			List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(dataSeries, false, false);
			foreach (IDataPoint current2 in dataPointsUnderViewPort)
			{
				if (current2.Enabled == false)
				{
					chart._toolTip.Hide();
				}
				else
				{
					double internalYValue = current2.DpInfo.InternalYValue;
					if (double.IsNaN(internalYValue))
					{
						list = new List<IDataPoint>();
						list2.Add(list);
					}
					else
					{
						double x = Graphics.ValueToPixelPosition(0.0, width, axisX.InternalAxisMinimum, axisX.InternalAxisMaximum, current2.DpInfo.InternalXValue);
						double y = Graphics.ValueToPixelPosition(height, 0.0, axisY.InternalAxisMinimum, axisY.InternalAxisMaximum, internalYValue);
						current2.DpInfo.VisualPosition = new Point(x, y);
						list.Add(current2);
					}
				}
			}
			GeometryGroup oldData = (dataSeries.Faces.Parts[0] as Path).Data as GeometryGroup;
			StepLineChart.GetPathGeometry(oldData, list2, false, width, height, canvas);
			if (!VisifireControl.IsMediaEffectsEnabled)
			{
				if (dataSeries.Faces.Parts[1] != null)
				{
					if (dataSeries.ShadowEnabled.Value)
					{
						(dataSeries.Faces.Parts[1] as Path).Visibility = Visibility.Visible;
						GeometryGroup oldData2 = (dataSeries.Faces.Parts[1] as Path).Data as GeometryGroup;
						StepLineChart.GetPathGeometry(oldData2, list2, true, width, height, canvas);
					}
					else
					{
						(dataSeries.Faces.Parts[1] as Path).Visibility = Visibility.Collapsed;
					}
				}
			}
			else if (dataSeries.Faces != null && dataSeries.Faces.Visual != null)
			{
				if (dataSeries.Effect == null)
				{
					if (dataSeries.ShadowEnabled.Value)
					{
						dataSeries.Faces.Visual.Effect = ExtendedGraphics.GetShadowEffect(315.0, 2.5, 1.0);
					}
					else
					{
						dataSeries.Faces.Visual.Effect = null;
					}
				}
				else
				{
					dataSeries.Faces.Visual.Effect = dataSeries.Effect;
				}
			}
			dataSeries._movingMarker.Visibility = Visibility.Collapsed;
			List<IDataPoint> list3 = (from datapoint in chart.PlotDetails.ListOfAllDataPoints
			where datapoint.Enabled == true
			select datapoint).ToList<IDataPoint>();
			for (int i = 0; i < list3.Count; i++)
			{
				if (list3[i].Parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					double limitingYValue = list3[i].Parent.PlotGroup.GetLimitingYValue();
					if (list3[i].DpInfo.InternalYValue < limitingYValue)
					{
						isOffsetAdded = false;
						break;
					}
				}
			}
			LineChart.Clip(chart, canvas3, canvas4, dataSeries.PlotGroup, isOffsetAdded);
		}

		private static void UpdateDataPoint(IDataPoint dataPoint, VcProperties property, object newValue)
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
			Marker marker = dataPoint.DpInfo.Marker;
			DataSeries parent = dataPoint.Parent;
			PlotGroup plotGroup = parent.PlotGroup;
			double height = chart.ChartArea.ChartVisualCanvas.Height;
			double width = chart.ChartArea.ChartVisualCanvas.Width;
			Canvas canvas = null;
			double markerLeft = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
			double markerTop = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, dataPoint.DpInfo.InternalYValue);
			if (parent.Faces != null)
			{
				canvas = parent.Faces.LabelCanvas;
				RenderHelper.UpdateParentVisualCanvasSize(chart, canvas);
			}
			if (property > VcProperties.LabelFontWeight)
			{
				if (property <= VcProperties.Opacity)
				{
					switch (property)
					{
					case VcProperties.LabelStyle:
						StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
						return;
					case VcProperties.LabelText:
						StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
						return;
					case VcProperties.LabelAngle:
						StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
						return;
					case VcProperties.Legend:
					case VcProperties.LegendMarkerColor:
					case VcProperties.LegendMarkerType:
					case VcProperties.LightingEnabled:
						return;
					case VcProperties.LegendText:
						chart.InvokeRender();
						return;
					default:
						switch (property)
						{
						case VcProperties.MarkerBorderColor:
							if (marker == null)
							{
								LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas, out markerLeft, out markerTop);
								return;
							}
							if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
							{
								marker.BorderColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor);
								return;
							}
							return;
						case VcProperties.MarkerBorderThickness:
							LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas, out markerLeft, out markerTop);
							return;
						case VcProperties.MarkerColor:
							if (marker != null && (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
							{
								marker.MarkerFillColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerColor);
								return;
							}
							return;
						case VcProperties.MarkerEnabled:
							LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas, out markerLeft, out markerTop);
							return;
						case VcProperties.MarkerScale:
						case VcProperties.MarkerSize:
						case VcProperties.MarkerType:
							break;
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
								return;
							}
							return;
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
						break;
					case VcProperties.ShowInLegend:
						chart.InvokeRender();
						return;
					default:
						switch (property)
						{
						case VcProperties.ToolTipText:
						case VcProperties.XValueFormatString:
						case VcProperties.YValueFormatString:
							dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
							if (!dataPoint.IsLightDataPoint)
							{
								(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
							}
							StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
							return;
						case VcProperties.To:
						case VcProperties.TrendLines:
						case VcProperties.Value:
						case VcProperties.ValueFormatString:
						case VcProperties.VerticalAlignment:
						case VcProperties.XValues:
							return;
						case VcProperties.XValue:
							if (double.IsNaN(dataPoint.DpInfo.OldYValue) || dataPoint.DpInfo.Faces == null)
							{
								StepLineChart.UpdateDataSeries(dataPoint.Parent, property, newValue);
								return;
							}
							StepLineChart.UpdateXAndYValue(dataPoint, canvas);
							return;
						case VcProperties.XValueType:
							chart.InvokeRender();
							return;
						case VcProperties.YValue:
						case VcProperties.YValues:
							if (double.IsNaN(dataPoint.DpInfo.OldYValue) || double.IsNaN(dataPoint.DpInfo.InternalYValue) || dataPoint.DpInfo.Faces == null)
							{
								StepLineChart.UpdateDataSeries(dataPoint.Parent, property, newValue);
								return;
							}
							chart.Dispatcher.BeginInvoke(new Action<IDataPoint, Canvas>(StepLineChart.UpdateXAndYValue), new object[]
							{
								dataPoint,
								canvas
							});
							return;
						default:
							return;
						}
						break;
					}
				}
				LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas, out markerLeft, out markerTop);
				return;
			}
			if (property <= VcProperties.Cursor)
			{
				if (property != VcProperties.Color)
				{
					if (property != VcProperties.Cursor)
					{
						return;
					}
					DataPointHelper.SetCursor2DataPointVisualFaces(dataPoint);
					return;
				}
				else if (marker != null && (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
				{
					marker.BorderColor = ((DataPointHelper.GetMarkerBorderColor_DataPoint(dataPoint) == null) ? ((newValue != null) ? (newValue as Brush) : ((Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor))) : ((Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor)));
					return;
				}
			}
			else
			{
				switch (property)
				{
				case VcProperties.Effect:
					if (marker != null && (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled) && VisifireControl.IsMediaEffectsEnabled)
					{
						marker.Effect = (Effect)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect);
						marker.ApplyEffects();
						return;
					}
					break;
				case VcProperties.Enabled:
					if (dataPoint.Parent.Enabled.Value && !dataPoint.Parent.isSeriesRenderPending)
					{
						StepLineChart.UpdateDataSeries(chart, dataPoint, VcProperties.Enabled, newValue);
						return;
					}
					break;
				default:
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
						break;
					case VcProperties.LabelBackground:
						StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
						return;
					case VcProperties.LabelEnabled:
						StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
						return;
					case VcProperties.LabelFontColor:
						StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
						return;
					case VcProperties.LabelFontFamily:
						StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
						return;
					case VcProperties.LabelFontSize:
						StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
						return;
					case VcProperties.LabelFontStyle:
						StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
						return;
					case VcProperties.LabelFontWeight:
						StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
						return;
					default:
						return;
					}
					break;
				}
			}
		}

		private static void UpdateDataSeries(Chart chart, IDataPoint dataPoint, VcProperties property, object newValue)
		{
			dataPoint.Parent.isSeriesRenderPending = true;
			chart.Dispatcher.BeginInvoke(new Action<IDataPoint, VcProperties, object>(StepLineChart.UpdateDataSeries), new object[]
			{
				dataPoint,
				property,
				newValue
			});
		}

		private static void UpdateXAndYValue(IDataPoint dataPoint, Canvas line2dLabelCanvas)
		{
			Chart chart = dataPoint.Chart as Chart;
			if (chart == null)
			{
				return;
			}
			bool value = chart.AnimatedUpdate.Value;
			if (!dataPoint.Enabled.Value || dataPoint.DpInfo.Faces == null)
			{
				return;
			}
			DataSeries parent = dataPoint.Parent;
			parent._movingMarker.Visibility = Visibility.Collapsed;
			Axis axisX = parent.PlotGroup.AxisX;
			Axis axisY = parent.PlotGroup.AxisY;
			Marker marker = dataPoint.DpInfo.Marker;
			Marker arg_83_0 = dataPoint.DpInfo.LegendMarker;
			double height = chart.ChartArea.ChartVisualCanvas.Height;
			double width = chart.ChartArea.ChartVisualCanvas.Width;
			double internalYValue = dataPoint.DpInfo.InternalYValue;
			double num = Graphics.ValueToPixelPosition(0.0, width, axisX.InternalAxisMinimum, axisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
			double num2 = Graphics.ValueToPixelPosition(height, 0.0, axisY.InternalAxisMinimum, axisY.InternalAxisMaximum, internalYValue);
			double y = num2;
			if (dataPoint.DpInfo.Faces.PreviousDataPoint != null)
			{
				Graphics.ValueToPixelPosition(0.0, width, axisX.InternalAxisMinimum, axisX.InternalAxisMaximum, dataPoint.DpInfo.Faces.PreviousDataPoint.DpInfo.InternalXValue);
				y = Graphics.ValueToPixelPosition(height, 0.0, axisY.InternalAxisMinimum, axisY.InternalAxisMaximum, dataPoint.DpInfo.Faces.PreviousDataPoint.DpInfo.InternalYValue);
			}
			double x = num;
			if (dataPoint.DpInfo.Faces.NextDataPoint != null)
			{
				x = Graphics.ValueToPixelPosition(0.0, width, axisX.InternalAxisMinimum, axisX.InternalAxisMaximum, dataPoint.DpInfo.Faces.NextDataPoint.DpInfo.InternalXValue);
				Graphics.ValueToPixelPosition(height, 0.0, axisY.InternalAxisMinimum, axisY.InternalAxisMaximum, dataPoint.DpInfo.Faces.NextDataPoint.DpInfo.InternalYValue);
			}
			dataPoint.DpInfo.VisualPosition = new Point(num, num2);
			Point point = default(Point);
			Point point2 = default(Point);
			if (marker != null)
			{
				point = marker.CalculateActualPosition(num, num2, new Point(0.5, 0.5));
				if (marker.Visual != null)
				{
					marker.Visual.Visibility = Visibility.Visible;
				}
			}
			Point point3 = default(Point);
			if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled))
			{
				if (value && dataPoint.DpInfo.LabelVisual != null)
				{
					point3 = new Point((double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty), (double)dataPoint.DpInfo.LabelVisual.GetValue(Canvas.TopProperty));
					point2 = StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, num, num2, ref line2dLabelCanvas, false);
					dataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, point3.Y);
					dataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, point3.X);
				}
				else
				{
					StepLineChart.CreateLabel4LineDataPoint(dataPoint, width, height, internalYValue >= 0.0, num, num2, ref line2dLabelCanvas, true);
				}
			}
			if (!value && marker != null && marker.Visual != null)
			{
				marker.Visual.SetValue(Canvas.TopProperty, point.Y);
				marker.Visual.SetValue(Canvas.LeftProperty, point.X);
			}
			DependencyObject dependencyObject = null;
			Point value2 = default(Point);
			Point value3 = default(Point);
			Point value4 = default(Point);
			LineSegment lineSegment = dataPoint.DpInfo.Faces.Parts[0] as LineSegment;
			LineSegment lineSegment2 = dataPoint.DpInfo.Faces.Parts[1] as LineSegment;
			LineSegment lineSegment3 = lineSegment2;
			if (dataPoint.DpInfo.Faces.NextDataPoint != null)
			{
				lineSegment3 = (dataPoint.DpInfo.Faces.NextDataPoint.DpInfo.Faces.Parts[0] as LineSegment);
			}
			PathFigure pathFigure = dataPoint.DpInfo.Faces.Parts[1] as PathFigure;
			if (dataPoint.DpInfo.Faces.PreviousDataPoint != null)
			{
				pathFigure = (dataPoint.DpInfo.Faces.Parts[2] as PathFigure);
			}
			if (!VisifireControl.IsMediaEffectsEnabled && dataPoint.Parent.ShadowEnabled.Value)
			{
				LineSegment lineSegment4 = dataPoint.DpInfo.ShadowFaces.Parts[0] as LineSegment;
				LineSegment lineSegment5 = dataPoint.DpInfo.ShadowFaces.Parts[1] as LineSegment;
				LineSegment lineSegment6 = lineSegment5;
				if (dataPoint.DpInfo.Faces.NextDataPoint != null)
				{
					lineSegment6 = (dataPoint.DpInfo.Faces.NextDataPoint.DpInfo.ShadowFaces.Parts[0] as LineSegment);
				}
				PathFigure pathFigure2 = dataPoint.DpInfo.ShadowFaces.Parts[1] as PathFigure;
				if (dataPoint.DpInfo.Faces.PreviousDataPoint != null)
				{
					pathFigure = (dataPoint.DpInfo.ShadowFaces.Parts[2] as PathFigure);
				}
				if (lineSegment4 == null)
				{
					value4 = pathFigure2.StartPoint;
					dependencyObject = pathFigure2;
					if (!value)
					{
						pathFigure2.StartPoint = new Point(num, num2);
						lineSegment6.Point = new Point(x, num2);
					}
				}
				else
				{
					dependencyObject = lineSegment5;
					value4 = lineSegment5.Point;
					if (!value)
					{
						lineSegment4.Point = new Point(num, y);
						lineSegment5.Point = new Point(num, num2);
						lineSegment6.Point = new Point(x, num2);
					}
				}
			}
			if (lineSegment == null)
			{
				if (value)
				{
					if (dataPoint.DpInfo.Storyboard != null)
					{
						dataPoint.DpInfo.Storyboard.Pause();
					}
					value2 = pathFigure.StartPoint;
				}
				else
				{
					pathFigure.StartPoint = new Point(num, num2);
					if (lineSegment3 != null)
					{
						lineSegment3.Point = new Point(x, num2);
					}
				}
			}
			else
			{
				value2 = lineSegment2.Point;
				if (value)
				{
					if (dataPoint.DpInfo.Storyboard != null)
					{
						dataPoint.DpInfo.Storyboard.Pause();
					}
				}
				else
				{
					lineSegment.Point = new Point(num, y);
					lineSegment2.Point = new Point(num, num2);
					lineSegment3.Point = new Point(x, num2);
				}
			}
			if (value)
			{
				Storyboard storyboard = new Storyboard();
				value3 = new Point(num, num2);
				DependencyObject dependencyObject2 = pathFigure;
				if (lineSegment2 != null)
				{
					dependencyObject2 = lineSegment2;
				}
				PointAnimation pointAnimation = new PointAnimation();
				pointAnimation.From = new Point?(value2);
				pointAnimation.To = new Point?(value3);
				pointAnimation.SpeedRatio = 2.0;
				pointAnimation.Duration = new Duration(new TimeSpan(0, 0, 1));
				Storyboard.SetTarget(pointAnimation, dependencyObject2);
				Storyboard.SetTargetProperty(pointAnimation, (lineSegment2 != null) ? new PropertyPath("Point", new object[0]) : new PropertyPath("StartPoint", new object[0]));
				Storyboard.SetTargetName(pointAnimation, (string)dependencyObject2.GetValue(FrameworkElement.NameProperty));
				storyboard.Children.Add(pointAnimation);
				PointAnimation pointAnimation2 = new PointAnimation();
				pointAnimation2 = pointAnimation;
				if (dataPoint.DpInfo.Faces.NextDataPoint != null)
				{
					value3 = new Point(x, num2);
					dependencyObject2 = lineSegment3;
					pointAnimation2 = new PointAnimation();
					pointAnimation2.From = new Point?(lineSegment3.Point);
					pointAnimation2.To = new Point?(value3);
					pointAnimation2.SpeedRatio = 2.0;
					pointAnimation2.Duration = new Duration(new TimeSpan(0, 0, 1));
					Storyboard.SetTarget(pointAnimation2, dependencyObject2);
					Storyboard.SetTargetProperty(pointAnimation2, (lineSegment3 != null) ? new PropertyPath("Point", new object[0]) : new PropertyPath("StartPoint", new object[0]));
					Storyboard.SetTargetName(pointAnimation2, (string)dependencyObject2.GetValue(FrameworkElement.NameProperty));
					storyboard.Children.Add(pointAnimation2);
				}
				if (!VisifireControl.IsMediaEffectsEnabled && dependencyObject != null)
				{
					LineSegment lineSegment4 = dataPoint.DpInfo.ShadowFaces.Parts[0] as LineSegment;
					LineSegment lineSegment5 = dataPoint.DpInfo.ShadowFaces.Parts[1] as LineSegment;
					LineSegment lineSegment6 = lineSegment5;
					if (dataPoint.DpInfo.Faces.NextDataPoint != null)
					{
						lineSegment6 = (dataPoint.DpInfo.Faces.NextDataPoint.DpInfo.ShadowFaces.Parts[0] as LineSegment);
					}
					PathFigure pathFigure2 = dataPoint.DpInfo.ShadowFaces.Parts[1] as PathFigure;
					if (dataPoint.DpInfo.Faces.PreviousDataPoint != null)
					{
						pathFigure = (dataPoint.DpInfo.ShadowFaces.Parts[2] as PathFigure);
					}
					dependencyObject = pathFigure2;
					if (lineSegment4 != null)
					{
						dependencyObject = lineSegment5;
					}
					value3 = new Point(num, num2);
					PointAnimation pointAnimation3 = new PointAnimation();
					pointAnimation3.From = new Point?(value4);
					pointAnimation3.To = new Point?(value3);
					pointAnimation3.SpeedRatio = 2.0;
					pointAnimation3.Duration = new Duration(new TimeSpan(0, 0, 1));
					Storyboard.SetTarget(pointAnimation3, dependencyObject);
					Storyboard.SetTargetProperty(pointAnimation3, (lineSegment5 != null) ? new PropertyPath("Point", new object[0]) : new PropertyPath("StartPoint", new object[0]));
					Storyboard.SetTargetName(pointAnimation3, (string)dependencyObject.GetValue(FrameworkElement.NameProperty));
					storyboard.Children.Add(pointAnimation3);
					PointAnimation pointAnimation4 = new PointAnimation();
					pointAnimation4 = pointAnimation3;
					if (dataPoint.DpInfo.Faces.NextDataPoint != null)
					{
						value3 = new Point(x, num2);
						dependencyObject = lineSegment6;
						pointAnimation4 = new PointAnimation();
						pointAnimation4.From = new Point?(lineSegment6.Point);
						pointAnimation4.To = new Point?(value3);
						pointAnimation4.SpeedRatio = 2.0;
						pointAnimation4.Duration = new Duration(new TimeSpan(0, 0, 1));
						Storyboard.SetTarget(pointAnimation4, dependencyObject);
						Storyboard.SetTargetProperty(pointAnimation4, (lineSegment6 != null) ? new PropertyPath("Point", new object[0]) : new PropertyPath("StartPoint", new object[0]));
						Storyboard.SetTargetName(pointAnimation4, (string)dependencyObject.GetValue(FrameworkElement.NameProperty));
						storyboard.Children.Add(pointAnimation4);
					}
					if (lineSegment4 != null && lineSegment6 != null)
					{
						lineSegment5.BeginAnimation(LineSegment.PointProperty, pointAnimation3);
						lineSegment6.BeginAnimation(LineSegment.PointProperty, pointAnimation4);
					}
					else if (lineSegment6 == null && lineSegment4 != null)
					{
						lineSegment5.BeginAnimation(LineSegment.PointProperty, pointAnimation3);
					}
					else
					{
						pathFigure2.BeginAnimation(PathFigure.StartPointProperty, pointAnimation3);
						lineSegment6.BeginAnimation(LineSegment.PointProperty, pointAnimation4);
					}
				}
				FrameworkElement visual = dataPoint.DpInfo.Marker.Visual;
				if (visual != null)
				{
					DoubleAnimation doubleAnimation = new DoubleAnimation
					{
						From = new double?((double)visual.GetValue(Canvas.LeftProperty)),
						To = new double?(point.X),
						Duration = new Duration(new TimeSpan(0, 0, 1)),
						SpeedRatio = 2.0
					};
					Storyboard.SetTarget(doubleAnimation, visual);
					Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)", new object[0]));
					Storyboard.SetTargetName(doubleAnimation, (string)visual.GetValue(FrameworkElement.NameProperty));
					storyboard.Children.Add(doubleAnimation);
					doubleAnimation = new DoubleAnimation
					{
						From = new double?((double)visual.GetValue(Canvas.TopProperty)),
						To = new double?(point.Y),
						Duration = new Duration(new TimeSpan(0, 0, 1)),
						SpeedRatio = 2.0
					};
					Storyboard.SetTarget(doubleAnimation, visual);
					Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)", new object[0]));
					Storyboard.SetTargetName(doubleAnimation, (string)visual.GetValue(FrameworkElement.NameProperty));
					storyboard.Children.Add(doubleAnimation);
				}
				FrameworkElement labelVisual = dataPoint.DpInfo.LabelVisual;
				if (labelVisual != null)
				{
					DoubleAnimation doubleAnimation2 = new DoubleAnimation
					{
						From = new double?(point3.X),
						To = new double?(point2.X),
						Duration = new Duration(new TimeSpan(0, 0, 1)),
						SpeedRatio = 2.0
					};
					Storyboard.SetTarget(doubleAnimation2, labelVisual);
					Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath("(Canvas.Left)", new object[0]));
					Storyboard.SetTargetName(doubleAnimation2, (string)labelVisual.GetValue(FrameworkElement.NameProperty));
					storyboard.Children.Add(doubleAnimation2);
					doubleAnimation2 = new DoubleAnimation
					{
						From = new double?(point3.Y),
						To = new double?(point2.Y),
						Duration = new Duration(new TimeSpan(0, 0, 1)),
						SpeedRatio = 2.0
					};
					Storyboard.SetTarget(doubleAnimation2, labelVisual);
					Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath("(Canvas.Top)", new object[0]));
					Storyboard.SetTargetName(doubleAnimation2, (string)labelVisual.GetValue(FrameworkElement.NameProperty));
					storyboard.Children.Add(doubleAnimation2);
				}
				dataPoint.DpInfo.Storyboard = storyboard;
				if (lineSegment != null && lineSegment3 != null)
				{
					lineSegment2.BeginAnimation(LineSegment.PointProperty, pointAnimation);
					lineSegment3.BeginAnimation(LineSegment.PointProperty, pointAnimation2);
				}
				else if (lineSegment3 == null && lineSegment != null)
				{
					lineSegment2.BeginAnimation(LineSegment.PointProperty, pointAnimation);
				}
				else
				{
					pathFigure.BeginAnimation(PathFigure.StartPointProperty, pointAnimation);
					lineSegment3.BeginAnimation(LineSegment.PointProperty, pointAnimation2);
				}
				storyboard.Begin();
			}
			parent.Faces.Visual.Width = chart.ChartArea.ChartVisualCanvas.Width;
			parent.Faces.Visual.Height = chart.ChartArea.ChartVisualCanvas.Height;
			parent.Faces.LabelCanvas.Width = chart.ChartArea.ChartVisualCanvas.Width;
			parent.Faces.LabelCanvas.Height = chart.ChartArea.ChartVisualCanvas.Height;
			dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
			if (!dataPoint.IsLightDataPoint)
			{
				(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
			}
			if (parent._movingMarker != null)
			{
				parent._movingMarker.Visibility = Visibility.Collapsed;
			}
			chart._toolTip.Hide();
			if (parent.ToolTipElement != null)
			{
				parent.ToolTipElement.Hide();
			}
			chart.ChartArea.DisableIndicators();
			if (parent.Faces != null)
			{
				double limitingYValue = dataPoint.Parent.PlotGroup.GetLimitingYValue();
				bool flag = true;
				List<IDataPoint> list = (from datapoint in chart.PlotDetails.ListOfAllDataPoints
				where datapoint.Enabled == true
				select datapoint).ToList<IDataPoint>();
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Parent.PlotGroup.AxisY.AxisMinimum != null)
					{
						double internalYValue2 = list[i].DpInfo.InternalYValue;
						if ((internalYValue2 < limitingYValue && limitingYValue > 0.0) || (internalYValue2 < limitingYValue && list[i].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue2 < list[i].Parent.PlotGroup.AxisY.InternalAxisMinimum)
						{
							list[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
							if ((bool)DataPointHelper.GetDataPointValueFromProperty(list[i], VcProperties.LabelEnabled) && list[i].DpInfo.LabelVisual != null)
							{
								list[i].DpInfo.LabelVisual.Visibility = Visibility.Collapsed;
							}
						}
						else if (list[i].DpInfo.LabelVisual != null)
						{
							(((list[i].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Visible;
						}
					}
					if (list[i].Parent.PlotGroup.AxisY.AxisMaximum != null)
					{
						double internalYValue3 = list[i].DpInfo.InternalYValue;
						if (internalYValue3 > list[i].Parent.PlotGroup.AxisY.InternalAxisMaximum)
						{
							list[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
							if ((bool)DataPointHelper.GetDataPointValueFromProperty(list[i], VcProperties.LabelEnabled) && list[i].DpInfo.LabelVisual != null)
							{
								list[i].DpInfo.LabelVisual.Visibility = Visibility.Collapsed;
							}
						}
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].Parent.PlotGroup.AxisY.AxisMinimum != null && list[j].DpInfo.InternalYValue < limitingYValue)
					{
						flag = false;
						break;
					}
				}
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				double num3 = chart.ChartArea.PLANK_DEPTH / (double)((chart.PlotDetails.Layer3DCount == 0) ? 1 : chart.PlotDetails.Layer3DCount) * (double)(chart.View3D ? 1 : 0);
				double x2 = 0.0;
				double y2 = -num3 - 4.0;
				double width2 = width + num3;
				double height2 = height + num3 + chart.ChartArea.PLANK_THICKNESS + 10.0;
				AreaChart.GetClipCoordinates(chart, ref x2, ref y2, ref width2, ref height2, parent.PlotGroup.MinimumX, parent.PlotGroup.MaximumX);
				rectangleGeometry.Rect = new Rect(x2, y2, width2, height2);
				if (parent.Faces.LabelCanvas != null && parent.Faces.LabelCanvas.Parent != null)
				{
					(parent.Faces.LabelCanvas.Parent as Canvas).Clip = rectangleGeometry;
				}
				rectangleGeometry = new RectangleGeometry();
				if (flag)
				{
					if (parent.PlotGroup.AxisY.AxisMaximum != null)
					{
						rectangleGeometry.Rect = new Rect(0.0, 0.0, width + num3, height + chart.ChartArea.PLANK_DEPTH + 6.0);
					}
					else
					{
						rectangleGeometry.Rect = new Rect(0.0, -num3 - 4.0, width + num3, height + chart.ChartArea.PLANK_DEPTH + 6.0);
					}
				}
				else if (parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					rectangleGeometry.Rect = new Rect(0.0, 0.0, width + num3, height + 0.0 + 0.0 + 0.0);
				}
				else
				{
					rectangleGeometry.Rect = new Rect(0.0, 0.0, width, height - 4.0 + chart.ChartArea.PLANK_DEPTH + 4.0);
				}
				if (parent.Faces.Visual != null)
				{
					(parent.Faces.Visual.Parent as Canvas).Clip = rectangleGeometry;
				}
			}
		}

		internal static void CreateAstepLineSeries(DataSeries series, double width, double height, Canvas labelCanvas, Canvas chartsCanvas, bool animationEnabled)
		{
			Canvas canvas;
			Canvas canvas2;
			if (series.Faces != null)
			{
				canvas = (series.Faces.Visual as Canvas);
				canvas2 = series.Faces.LabelCanvas;
				if (canvas != null)
				{
					Panel panel = canvas.Parent as Panel;
					if (panel != null)
					{
						panel.Children.Remove(canvas);
					}
				}
				if (canvas2 != null)
				{
					Panel panel2 = canvas2.Parent as Panel;
					if (panel2 != null)
					{
						panel2.Children.Remove(canvas2);
					}
				}
			}
			if (!series.Enabled.Value)
			{
				return;
			}
			double x = double.NaN;
			double y = double.NaN;
			VisifireControl arg_8F_0 = series.Chart;
			canvas2 = new Canvas
			{
				Width = width,
				Height = height
			};
			List<List<IDataPoint>> list = new List<List<IDataPoint>>();
			List<List<IDataPoint>> list2 = new List<List<IDataPoint>>();
			PlotGroup arg_BE_0 = series.PlotGroup;
			StepLineChartShapeParams stepLineChartShapeParams = new StepLineChartShapeParams();
			stepLineChartShapeParams.Points = new List<IDataPoint>();
			stepLineChartShapeParams.ShadowPoints = new List<IDataPoint>();
			stepLineChartShapeParams.LineGeometryGroup = new GeometryGroup();
			stepLineChartShapeParams.LineThickness = series.LineThickness.Value;
			stepLineChartShapeParams.LineColor = series.Color;
			stepLineChartShapeParams.LineStyle = ExtendedGraphics.GetDashArray(series.LineStyle);
			stepLineChartShapeParams.Lighting = series.LightingEnabled.Value;
			stepLineChartShapeParams.ShadowEnabled = series.ShadowEnabled.Value;
			stepLineChartShapeParams.Opacity = series.Opacity;
			if (series.ShadowEnabled.Value)
			{
				stepLineChartShapeParams.LineShadowGeometryGroup = new GeometryGroup();
			}
			series.VisualParams = stepLineChartShapeParams;
			Point point = default(Point);
			bool flag = true;
			List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(series, false, false);
			foreach (IDataPoint current in dataPointsUnderViewPort)
			{
				if (!(current.Enabled == false))
				{
					current.DpInfo.Marker = null;
					current.DpInfo.LabelVisual = null;
					current.DpInfo.Faces = null;
					if (double.IsNaN(current.DpInfo.InternalYValue))
					{
						x = double.NaN;
						y = double.NaN;
						flag = true;
					}
					else
					{
						LineChart.CalculateMarkerPosition(current, width, height, ref x, ref y);
						if (flag)
						{
							new Point(x, y);
							flag = !flag;
							if (stepLineChartShapeParams.Points.Count > 0)
							{
								list.Add(stepLineChartShapeParams.Points);
								list2.Add(stepLineChartShapeParams.ShadowPoints);
							}
							stepLineChartShapeParams.Points = new List<IDataPoint>();
							stepLineChartShapeParams.ShadowPoints = new List<IDataPoint>();
						}
						else
						{
							point = new Point(x, y);
							flag = false;
						}
						current.DpInfo.VisualPosition = new Point(x, y);
						stepLineChartShapeParams.Points.Add(current);
						if (stepLineChartShapeParams.ShadowEnabled)
						{
							stepLineChartShapeParams.ShadowPoints.Add(current);
						}
					}
				}
			}
			list.Add(stepLineChartShapeParams.Points);
			list2.Add(stepLineChartShapeParams.ShadowPoints);
			series.Faces = new Faces();
			Path item;
			Path item2;
			canvas = StepLineChart.GetStepLine2D(series, width, height, canvas2, stepLineChartShapeParams, out item, out item2, list, list2);
			canvas.Width = width;
			canvas.Height = height;
			series.Faces.Parts.Add(item);
			if (!VisifireControl.IsMediaEffectsEnabled)
			{
				series.Faces.Parts.Add(item2);
			}
			labelCanvas.Children.Add(canvas2);
			chartsCanvas.Children.Add(canvas);
			series.Faces.Visual = canvas;
			series.Faces.LabelCanvas = canvas2;
			if (animationEnabled)
			{
				if (series.Storyboard == null)
				{
					series.Storyboard = new Storyboard();
				}
				else
				{
					series.Storyboard.Stop();
				}
				series.Storyboard = LineChart.ApplyLineChartAnimation(series, canvas, series.Storyboard, true);
			}
			double num = series.MarkerSize.Value * series.MarkerScale.Value * 1.1;
			if (num < 6.0)
			{
				num = 6.0;
			}
			Ellipse ellipse = new Ellipse
			{
				Visibility = Visibility.Collapsed,
				IsHitTestVisible = false,
				Height = num,
				Width = num,
				Fill = stepLineChartShapeParams.LineColor
			};
			labelCanvas.Children.Add(ellipse);
			series._movingMarker = ellipse;
		}

		internal static Canvas GetVisualObjectForLineChart(Panel preExistingPanel, double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, double plankDepth, bool animationEnabled)
		{
			if (double.IsNaN(width) || double.IsNaN(height) || width <= 0.0 || height <= 0.0)
			{
				return null;
			}
			bool isOffsetAdded = true;
			Canvas canvas;
			Canvas canvas2;
			Canvas canvas3;
			RenderHelper.PrepareCanvas4Drawing(preExistingPanel as Canvas, out canvas, out canvas2, out canvas3, width, height);
			double num = plankDepth / (double)((plotDetails.Layer3DCount == 0) ? 1 : plotDetails.Layer3DCount) * (double)(chart.View3D ? 1 : 0);
			double num2 = num * (double)(plotDetails.SeriesDrawingIndex[seriesList[0]] + 1 - ((plotDetails.Layer3DCount == 0) ? 0 : 1));
			canvas.SetValue(Canvas.TopProperty, num2);
			canvas.SetValue(Canvas.LeftProperty, -num2);
			bool flag = false;
			double val = 1.7976931348623157E+308;
			double val2 = -1.7976931348623157E+308;
			foreach (DataSeries current in seriesList)
			{
				StepLineChart.CreateAstepLineSeries(current, width, height, canvas2, canvas3, animationEnabled);
				flag = (flag || current.MovingMarkerEnabled);
				val = Math.Min(val, current.PlotGroup.MinimumX);
				val2 = Math.Max(val2, current.PlotGroup.MaximumX);
			}
			List<IDataPoint> list = (from datapoint in plotDetails.ListOfAllDataPoints
			where datapoint.Enabled == true
			select datapoint).ToList<IDataPoint>();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					double limitingYValue = list[i].Parent.PlotGroup.GetLimitingYValue();
					if (list[i].DpInfo.InternalYValue < limitingYValue)
					{
						isOffsetAdded = false;
						break;
					}
				}
			}
			chart.ChartArea.PlotAreaCanvas.MouseMove -= new MouseEventHandler(StepLineChart.PlotAreaCanvas_MouseMove);
			chart.ChartArea.PlotAreaCanvas.MouseLeave -= new MouseEventHandler(StepLineChart.PlotAreaCanvas_MouseLeave);
			chart.ChartArea.PlotAreaCanvas.MouseEnter -= new MouseEventHandler(StepLineChart.PlotAreaCanvas_MouseEnter);
			if (flag)
			{
				chart.ChartArea.PlotAreaCanvas.Tag = chart.PlotArea;
				chart.ChartArea.PlotAreaCanvas.MouseMove += new MouseEventHandler(StepLineChart.PlotAreaCanvas_MouseMove);
				chart.ChartArea.PlotAreaCanvas.MouseLeave += new MouseEventHandler(StepLineChart.PlotAreaCanvas_MouseLeave);
				chart.ChartArea.PlotAreaCanvas.MouseEnter += new MouseEventHandler(StepLineChart.PlotAreaCanvas_MouseEnter);
			}
			if (animationEnabled && seriesList.Count > 0)
			{
				DataSeries dataSeries = seriesList[0];
				if (dataSeries.Storyboard == null)
				{
					dataSeries.Storyboard = new Storyboard();
				}
				dataSeries.Storyboard = LineChart.ApplyLineChartAnimation(dataSeries, canvas2, dataSeries.Storyboard, false);
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
			canvas3.Height = height;
			canvas2.Height = height;
			canvas3.Width = width;
			canvas2.Width = width;
			LineChart.Clip(chart, canvas3, canvas2, seriesList[0].PlotGroup, isOffsetAdded);
			return canvas;
		}

		private static void PlotAreaCanvas_MouseEnter(object sender, MouseEventArgs e)
		{
			StepLineChart._isMouseEnteredInPlotArea = true;
		}

		private static void PlotAreaCanvas_MouseLeave(object sender, MouseEventArgs e)
		{
			StepLineChart._isMouseEnteredInPlotArea = false;
			PlotArea plotArea = ((FrameworkElement)sender).Tag as PlotArea;
			if (plotArea == null)
			{
				return;
			}
			Chart chart = plotArea.Chart as Chart;
			if (chart == null)
			{
				return;
			}
			foreach (DataSeries current in chart.InternalSeries)
			{
				if (current.RenderAs != RenderAs.StepLine)
				{
					break;
				}
				if (current._movingMarker != null)
				{
					current._movingMarker.Visibility = Visibility.Collapsed;
				}
			}
		}

		private static void PlotAreaCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			PlotArea plotArea = ((FrameworkElement)sender).Tag as PlotArea;
			if (plotArea == null)
			{
				return;
			}
			Chart chart = plotArea.Chart as Chart;
			if (chart == null)
			{
				return;
			}
			((FrameworkElement)sender).Dispatcher.BeginInvoke(new Action<Chart, object, MouseEventArgs, RenderAs[]>(StepLineChart.MoveMovingMarker), new object[]
			{
				chart,
				sender,
				e,
				new RenderAs[]
				{
					RenderAs.StepLine
				}
			});
		}

		internal static void MoveMovingMarker(Chart chart, object sender, MouseEventArgs e, params RenderAs[] chartTypes)
		{
			if (!StepLineChart._isMouseEnteredInPlotArea)
			{
				return;
			}
			foreach (DataSeries current in chart.InternalSeries)
			{
				if (Array.IndexOf<RenderAs>(chartTypes, current.RenderAs) != -1)
				{
					double internalXValue = RenderHelper.CalculateInternalXValueFromPixelPos(chart, current.PlotGroup.AxisX, e);
					double internalYValue = RenderHelper.CalculateInternalXValueFromPixelPos(chart, current.PlotGroup.AxisY, e);
					current.ShowMovingMarker(internalXValue, internalYValue);
				}
			}
		}

		internal static void ShowMovingMarkerAtDataPoint(DataSeries dataSeries, IDataPoint nearestDataPoint)
		{
			if (nearestDataPoint == null)
			{
				dataSeries._movingMarker.Visibility = Visibility.Collapsed;
				return;
			}
			if (dataSeries.MovingMarkerEnabled)
			{
				Ellipse movingMarker = nearestDataPoint.Parent._movingMarker;
				if (StepLineChart._isMouseEnteredInPlotArea)
				{
					movingMarker.Visibility = Visibility.Visible;
				}
				bool flag = false;
				if (!nearestDataPoint.IsLightDataPoint)
				{
					flag = (nearestDataPoint as DataPoint).Selected;
				}
				if (flag)
				{
					LineChart.SelectMovingMarker(nearestDataPoint);
					return;
				}
				movingMarker.Fill = nearestDataPoint.Parent.Color;
				double num = nearestDataPoint.Parent.MarkerSize.Value * nearestDataPoint.Parent.MarkerScale.Value * 1.1;
				if (num < 6.0)
				{
					num = 6.0;
				}
				movingMarker.Height = num;
				movingMarker.Width = movingMarker.Height;
				movingMarker.StrokeThickness = 0.0;
				movingMarker.SetValue(Canvas.LeftProperty, nearestDataPoint.DpInfo.VisualPosition.X - movingMarker.Width / 2.0);
				movingMarker.SetValue(Canvas.TopProperty, nearestDataPoint.DpInfo.VisualPosition.Y - movingMarker.Height / 2.0);
			}
		}
	}
}
