using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class LineChart
	{
		private const double MOVING_MARKER_SCALE = 1.1;

		private static bool _isMouseEnteredInPlotArea;

		public static Point[] cp1
		{
			get;
			set;
		}

		public static Point[] cp2
		{
			get;
			set;
		}

		internal static Point CreateLabel4LineDataPoint(IDataPoint dataPoint, double width, double height, bool isPositive, double markerLeft, double markerTop, ref Canvas labelCanvas, bool IsSetPosition)
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
				}, dataPoint.Chart.FlowDirection);
				dataPoint.DpInfo.LabelVisual = title.Visual;
				double num2 = 0.0;
				double num3 = 0.0;
				if (double.IsNaN(num) || num == 0.0)
				{
					LineChart.SetLabelPosition4LineDataPoint(dataPoint, ref num2, ref num3);
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
						LineChart.SetLabelPosition4LineDataPoint(dataPoint, ref num2, ref num3);
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

		internal static Marker GetMarkerForDataPoint(bool reCreate, Chart chart, double plotWidth, double plotHeight, double yPosition, IDataPoint dataPoint, bool isPositive)
		{
			bool flag = false;
			RenderAs renderAs = dataPoint.Parent.RenderAs;
			string text;
			if (renderAs == RenderAs.Spline || renderAs == RenderAs.Line || renderAs == RenderAs.StepLine)
			{
				text = "";
			}
			else
			{
				flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled);
				text = (flag ? dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText)) : "");
			}
			bool markerBevel = false;
			LabelStyles labelStyles = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			double num = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelAngle);
			double num2 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
			double scaleFactor = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
			MarkerTypes markerType = (MarkerTypes)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerType);
			Brush brush = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerColor);
			bool isEnabled = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled);
			double num3 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity);
			string str = (string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href);
			if (reCreate)
			{
				Marker marker = dataPoint.DpInfo.Marker;
				if (marker != null && marker.Visual != null)
				{
					Panel panel = marker.Visual.Parent as Panel;
					if (panel != null)
					{
						panel.Children.Remove(marker.Visual);
					}
				}
				dataPoint.DpInfo.Marker = new Marker();
				dataPoint.DpInfo.Marker.AssignPropertiesValue(markerType, scaleFactor, new Size(num2, num2), markerBevel, brush, text);
				dataPoint.DpInfo.Marker.IsEnabled = isEnabled;
			}
			else
			{
				Marker marker2 = dataPoint.DpInfo.Marker;
				marker2.MarkerType = markerType;
				marker2.ScaleFactor = scaleFactor;
				marker2.MarkerSize = new Size(num2, num2);
				marker2.Bevel = false;
				marker2.MarkerFillColor = brush;
				marker2.Text = text;
				marker2.TextAlignmentX = AlignmentX.Center;
				marker2.TextAlignmentY = AlignmentY.Center;
				marker2.IsEnabled = isEnabled;
			}
			LineChart.ApplyMarkerProperties(dataPoint);
			if (!string.IsNullOrEmpty(text) && flag)
			{
				LineChart.ApplyLabelProperties(dataPoint);
				if (!double.IsNaN(num) && num != 0.0)
				{
					dataPoint.DpInfo.Marker.LabelAngle = num;
					dataPoint.DpInfo.Marker.TextOrientation = Orientation.Vertical;
					if (isPositive)
					{
						dataPoint.DpInfo.Marker.TextAlignmentX = AlignmentX.Center;
						dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Top;
					}
					else
					{
						dataPoint.DpInfo.Marker.TextAlignmentX = AlignmentX.Center;
						dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Bottom;
					}
					dataPoint.DpInfo.Marker.LabelStyle = labelStyles;
				}
				dataPoint.DpInfo.Marker.CreateVisual((chart != null) ? chart.FlowDirection : FlowDirection.LeftToRight);
				if (double.IsNaN(num) || num == 0.0)
				{
					dataPoint.DpInfo.Marker.TextAlignmentX = AlignmentX.Center;
					if (isPositive)
					{
						if (labelStyles == LabelStyles.OutSide && !dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
						{
							if (yPosition - dataPoint.DpInfo.Marker.MarkerActualSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 < 0.0 || (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle) == LabelStyles.Inside)
							{
								dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Bottom;
							}
							else
							{
								dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Top;
							}
						}
						else if (labelStyles == LabelStyles.OutSide)
						{
							dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Top;
						}
						else
						{
							dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Bottom;
						}
					}
					else if (labelStyles == LabelStyles.OutSide && !dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
					{
						if (yPosition + dataPoint.DpInfo.Marker.MarkerActualSize.Height + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 > chart.PlotArea.BorderElement.Height || (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle) == LabelStyles.Inside)
						{
							dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Top;
						}
						else
						{
							dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Bottom;
						}
					}
					else if (labelStyles == LabelStyles.OutSide)
					{
						dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Bottom;
					}
					else
					{
						dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Top;
					}
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
			if (!chart.IndicatorEnabled)
			{
				dataPoint.AttachToolTip(chart, dataPoint, dataPoint.DpInfo.Marker.Visual);
			}
			dataPoint.DpFunc.AttachHref(chart, dataPoint.DpInfo.Marker.Visual);
			DataPointHelper.SetCursor2DataPointVisualFaces(dataPoint);
			return dataPoint.DpInfo.Marker;
		}

		internal static Point SetLabelPosition4LineDataPoint(IDataPoint dataPoint, ref double labelLeft, ref double labelTop)
		{
			double num = 6.0;
			Chart chart = dataPoint.Chart as Chart;
			double width = chart.ChartArea.PlotAreaCanvas.Width;
			double height = chart.ChartArea.PlotAreaCanvas.Height;
			bool flag = dataPoint.DpInfo.InternalYValue >= 0.0;
			double x = dataPoint.DpInfo.VisualPosition.X;
			double y = dataPoint.DpInfo.VisualPosition.Y;
			Size size = new Size(dataPoint.DpInfo.LabelVisual.Width, dataPoint.DpInfo.LabelVisual.Height);
			Point point = new Point(x, y);
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
			bool flag2 = false;
			LabelStyles labelStyles = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			if (dataPoint.DpInfo.Marker != null)
			{
				if (flag)
				{
					if (labelStyles == LabelStyles.OutSide && !dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
					{
						if (point.Y - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 < 0.0 || labelStyles == LabelStyles.Inside)
						{
							if (point.X + size.Width > width && visualPosition.Y - point.Y <= 50.0 && size.Width < 50.0)
							{
								labelLeft = point.X - size.Width / 2.0;
								labelTop = point.Y + num + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if ((point.X + size.Width > width && visualPosition.Y - point.Y > 50.0) || point.X + size.Width > width)
							{
								labelLeft = point.X - size.Width - num - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y - num + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (visualPosition.Y > point.Y && visualPosition2.Y > point.Y && visualPosition.Y - point.Y > 20.0 && visualPosition.Y - point.Y > visualPosition2.Y - point.Y)
							{
								if (point.X - size.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor <= 2.0)
								{
									labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
									labelTop = point.Y - size.Height / 2.0;
								}
								else
								{
									labelLeft = point.X - size.Width - num - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
									labelTop = point.Y - size.Height / 2.0;
								}
							}
							else if (visualPosition2.X - point.X > 120.0 && size.Width < 50.0)
							{
								labelLeft = point.X - size.Width / 2.0;
								labelTop = point.Y + num + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y > visualPosition.Y && point.Y - visualPosition.Y > point.Y - visualPosition2.Y && visualPosition.X != 0.0)
							{
								labelLeft = point.X - size.Width - num - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y - num + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y >= visualPosition2.Y && visualPosition == new Point(0.0, 0.0))
							{
								labelLeft = point.X - size.Width / 2.0;
								labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y >= visualPosition2.Y && visualPosition.Y >= point.Y)
							{
								labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y <= visualPosition2.Y && visualPosition.Y >= point.Y)
							{
								if (size.Width < 15.0)
								{
									labelLeft = point.X - size.Width / 2.0;
									labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								}
								else
								{
									labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
									labelTop = point.Y + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								}
							}
							else if (visualPosition == new Point(0.0, 0.0) && point.X - size.Width / 2.0 > 0.0)
							{
								labelLeft = point.X - size.Width / 2.0;
								labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else
							{
								labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y - size.Height / 2.0;
							}
							flag2 = true;
						}
						else if (point.Y + size.Height + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 > height || labelStyles == LabelStyles.Inside)
						{
							if (point.X + size.Width > width && visualPosition.Y - point.Y <= 50.0 && size.Width < 50.0)
							{
								labelLeft = point.X - size.Width / 2.0;
								labelTop = point.Y - num / 2.0 - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if ((point.X + size.Width > width && visualPosition.Y - point.Y > 50.0) || point.X + size.Width > width)
							{
								labelLeft = point.X - num - size.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (visualPosition.Y > point.Y && visualPosition2.Y > point.Y && visualPosition.Y - point.Y > 20.0 && visualPosition.Y - point.Y > visualPosition2.Y - point.Y)
							{
								labelLeft = point.X - size.Width - num - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (visualPosition2.X - point.X > 120.0 && size.Width < 50.0 && visualPosition.X == 0.0)
							{
								labelLeft = point.X - size.Width / 2.0;
								labelTop = point.Y - num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y > visualPosition.Y && point.Y - visualPosition.Y > point.Y - visualPosition2.Y && visualPosition.X != 0.0)
							{
								labelLeft = point.X - size.Width - num - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y >= visualPosition2.Y && visualPosition == new Point(0.0, 0.0))
							{
								labelLeft = point.X - size.Width / 2.0;
								labelTop = point.Y - num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else if (point.Y >= visualPosition2.Y && visualPosition.Y >= point.Y)
							{
								if (point.Y - visualPosition2.Y >= 10.0)
								{
									labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
									labelTop = point.Y + num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								}
								else
								{
									labelLeft = point.X - size.Width / 2.0;
									labelTop = point.Y - num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								}
							}
							else if (point.Y <= visualPosition2.Y && visualPosition.Y >= point.Y)
							{
								if (size.Width < 20.0)
								{
									labelLeft = point.X - size.Width / 2.0;
									labelTop = point.Y - num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								}
								else
								{
									labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
									labelTop = point.Y + num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								}
							}
							else if (visualPosition == new Point(0.0, 0.0) && point.X - size.Width / 2.0 > 0.0)
							{
								labelLeft = point.X - size.Width / 2.0;
								labelTop = point.Y - num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else
							{
								labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							flag2 = true;
						}
					}
					else if (labelStyles == LabelStyles.OutSide)
					{
						labelLeft = point.X - size.Width / 2.0;
						labelTop = point.Y - num / 2.0 - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						flag2 = true;
					}
					else
					{
						labelLeft = point.X - size.Width / 2.0;
						labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						flag2 = true;
					}
				}
				else if (labelStyles == LabelStyles.OutSide && !dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
				{
					if (point.Y + size.Height + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 > height || labelStyles == LabelStyles.Inside)
					{
						if (point.X + size.Width > width && visualPosition.Y - point.Y <= 50.0 && size.Width < 50.0)
						{
							labelLeft = point.X - size.Width / 2.0;
							labelTop = point.Y - num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if ((point.X + size.Width > width && visualPosition.Y - point.Y > 50.0) || point.X + size.Width > width)
						{
							labelLeft = point.X - num - size.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (visualPosition.Y > point.Y && visualPosition2.Y > point.Y && visualPosition.Y - point.Y > 20.0 && visualPosition.Y - point.Y > visualPosition2.Y - point.Y)
						{
							labelLeft = point.X - size.Width - num - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (visualPosition2.X - point.X > 100.0 && size.Width < 50.0 && visualPosition.X == 0.0)
						{
							labelLeft = point.X - size.Width / 2.0;
							labelTop = point.Y - num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y > visualPosition.Y && point.Y > visualPosition2.Y && point.Y - visualPosition.Y > point.Y - visualPosition2.Y && visualPosition.X != 0.0)
						{
							labelLeft = point.X - size.Width - num - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y >= visualPosition2.Y && visualPosition == new Point(0.0, 0.0))
						{
							labelLeft = point.X - size.Width / 2.0;
							labelTop = point.Y - num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y >= visualPosition2.Y && visualPosition.Y >= point.Y)
						{
							if (point.Y - visualPosition2.Y >= 10.0)
							{
								labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else
							{
								labelLeft = point.X - size.Width / 2.0;
								labelTop = point.Y - num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
						}
						else if (point.Y <= visualPosition2.Y && visualPosition.Y >= point.Y)
						{
							if (size.Width < 20.0 || visualPosition.Y - point.Y < 20.0)
							{
								labelLeft = point.X - size.Width / 2.0;
								labelTop = point.Y - num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else
							{
								labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
						}
						else if (visualPosition == new Point(0.0, 0.0) && point.X - size.Width / 2.0 > 0.0)
						{
							labelLeft = point.X - size.Width / 2.0;
							labelTop = point.Y - num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else
						{
							labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + num - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						flag2 = true;
					}
					else if (point.Y - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 < 0.0 || labelStyles == LabelStyles.Inside)
					{
						if (point.X + size.Width > width && visualPosition.Y - point.Y <= 50.0 && size.Width < 50.0)
						{
							labelLeft = point.X - size.Width / 2.0;
							labelTop = point.Y + num + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if ((point.X + size.Width > width && visualPosition.Y - point.Y > 50.0) || point.X + size.Width > width)
						{
							labelLeft = point.X - size.Width - num - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - num + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (visualPosition.Y > point.Y && visualPosition2.Y > point.Y && visualPosition.Y - point.Y > 20.0 && visualPosition.Y - point.Y > visualPosition2.Y - point.Y)
						{
							labelLeft = point.X - size.Width - num - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - num + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (visualPosition2.X - point.X > 100.0 && size.Width < 50.0 && visualPosition2.Y - point.Y < 20.0)
						{
							labelLeft = point.X - size.Width / 2.0;
							labelTop = point.Y + num + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y > visualPosition.Y && point.Y - visualPosition.Y > point.Y - visualPosition2.Y && visualPosition.X != 0.0)
						{
							labelLeft = point.X - size.Width - num - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - num + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y >= visualPosition2.Y && visualPosition == new Point(0.0, 0.0))
						{
							labelLeft = point.X - size.Width / 2.0;
							labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y >= visualPosition2.Y && visualPosition.Y >= point.Y)
						{
							labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y <= visualPosition2.Y && visualPosition.Y >= point.Y)
						{
							if (size.Width < 15.0)
							{
								labelLeft = point.X - size.Width / 2.0;
								labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else
							{
								labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
						}
						else if (visualPosition == new Point(0.0, 0.0) && point.X - size.Width / 2.0 > 0.0)
						{
							labelLeft = point.X - size.Width / 2.0;
							labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else
						{
							labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - num + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						flag2 = true;
					}
				}
				else if (labelStyles == LabelStyles.OutSide)
				{
					labelLeft = point.X - size.Width / 2.0;
					labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
					flag2 = true;
				}
				else
				{
					labelLeft = point.X - size.Width / 2.0;
					labelTop = point.Y - num / 2.0 - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
					flag2 = true;
				}
				if (!flag2 && labelStyles == LabelStyles.OutSide && !dataPoint.DpInfo.IsLabelStyleSet && !dataPoint.Parent.IsLabelStyleSet)
				{
					if (visualPosition.Y <= point.Y && point.X + size.Width >= width && visualPosition2.X == 0.0 && visualPosition2.Y == 0.0)
					{
						labelLeft = point.X - size.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						labelTop = point.Y + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
					}
					else if (visualPosition.Y > point.Y && point.X + size.Width > width && visualPosition2.X == 0.0 && visualPosition2.Y == 0.0)
					{
						labelLeft = point.X - size.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						labelTop = point.Y - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
					}
					else if (point.X + size.Width > width)
					{
						labelLeft = point.X - num - size.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						labelTop = point.Y - size.Height / 2.0;
					}
					else if (visualPosition.Y <= point.Y && visualPosition2.Y <= point.Y && visualPosition2 != new Point(0.0, 0.0) && visualPosition != new Point(0.0, 0.0))
					{
						labelLeft = point.X - size.Width / 2.0;
						labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
					}
					else if (visualPosition.Y > point.Y && visualPosition2.Y > point.Y)
					{
						labelLeft = point.X - size.Width / 2.0;
						labelTop = point.Y - num / 2.0 - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
					}
					else if (visualPosition.X == 0.0 && visualPosition.Y == 0.0)
					{
						if (point.Y > visualPosition2.Y && point.Y - visualPosition2.Y > 20.0 && point.X - size.Width / 2.0 < 0.0)
						{
							labelLeft = point.X + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y > visualPosition2.Y && point.Y - visualPosition2.Y > 20.0)
						{
							if (point.X - size.Width >= 0.0)
							{
								labelLeft = point.X - size.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
								labelTop = point.Y - num / 2.0 - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
							else
							{
								labelLeft = point.X - size.Width / 2.0;
								labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							}
						}
						else if (point.X - size.Width / 2.0 > 0.0)
						{
							labelLeft = point.X - size.Width / 2.0;
							labelTop = point.Y - num / 2.0 - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y > visualPosition2.Y)
						{
							labelLeft = point.X + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else
						{
							labelLeft = point.X + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
					}
					else if (visualPosition2.X == 0.0 && visualPosition2.Y == 0.0 && point.X + size.Width > width)
					{
						if (point.Y > visualPosition.Y)
						{
							labelLeft = point.X - size.Width / 2.0;
							labelTop = point.Y + num / 2.0 + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else
						{
							labelLeft = point.X - size.Width - dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - num / 2.0 - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
					}
					else if (visualPosition2.X == 0.0 && visualPosition2.Y == 0.0)
					{
						labelLeft = point.X + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						labelTop = point.Y - num / 2.0 - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
					}
					else if ((visualPosition.Y <= point.Y && visualPosition2.Y >= point.Y) || (visualPosition.Y > point.Y && visualPosition2.Y < point.Y))
					{
						if (visualPosition.Y <= point.Y && visualPosition2.Y >= point.Y && (visualPosition2.Y - point.Y < 10.0 || visualPosition.Y < point.Y))
						{
							labelLeft = point.X + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - num / 2.0 - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
						else if (point.Y - visualPosition2.Y > 20.0)
						{
							labelLeft = point.X + num + dataPoint.DpInfo.Marker.MarkerSize.Width / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
							labelTop = point.Y - size.Height / 2.0;
						}
						else
						{
							labelLeft = point.X - size.Width / 2.0;
							labelTop = point.Y - num / 2.0 - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
						}
					}
					else
					{
						labelLeft = point.X - size.Width / 2.0;
						labelTop = point.Y - num / 2.0 - size.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 * dataPoint.DpInfo.Marker.ScaleFactor;
					}
				}
			}
			return new Point(labelLeft, labelTop);
		}

		internal static void ApplyDefaultInteractivityForMarker(IDataPoint dataPoint)
		{
			if (dataPoint.DpInfo.Marker.IsEnabled)
			{
				if (!dataPoint.Parent.MovingMarkerEnabled)
				{
					bool isSelected = false;
					if (!dataPoint.IsLightDataPoint)
					{
						isSelected = (dataPoint as DataPoint).Selected;
					}
					dataPoint.DpInfo.Marker.MarkerShape.MouseEnter += delegate(object sender, MouseEventArgs e)
					{
						if (!isSelected)
						{
							Shape shape = sender as Shape;
							if (dataPoint.Parent.HighLightColor != null)
							{
								shape.Stroke = dataPoint.Parent.HighLightColor;
							}
							if (dataPoint.DpInfo.Marker != null)
							{
								shape.StrokeThickness = dataPoint.DpInfo.Marker.BorderThickness;
							}
						}
					};
					dataPoint.DpInfo.Marker.MarkerShape.MouseLeave += delegate(object sender, MouseEventArgs e)
					{
						if (!isSelected)
						{
							Shape shape = sender as Shape;
							if (dataPoint.DpInfo.Marker != null)
							{
								shape.Stroke = dataPoint.DpInfo.Marker.BorderColor;
								shape.StrokeThickness = dataPoint.DpInfo.Marker.BorderThickness;
							}
						}
					};
					return;
				}
			}
			else
			{
				LineChart.HideDataPointMarker(dataPoint);
			}
		}

		internal static void HideDataPointMarker(IDataPoint dataPoint)
		{
			Brush brush = new SolidColorBrush(Colors.Transparent);
			dataPoint.DpInfo.Marker.MarkerShape.Fill = brush;
			SolidColorBrush solidColorBrush = dataPoint.DpInfo.Marker.MarkerShape.Stroke as SolidColorBrush;
			if (solidColorBrush == null || !solidColorBrush.Color.ToString().Equals(brush.ToString()))
			{
				dataPoint.DpInfo.Marker.MarkerShape.Stroke = brush;
			}
			if (dataPoint.DpInfo.Marker.MarkerShadow != null)
			{
				dataPoint.DpInfo.Marker.MarkerShadow.Visibility = Visibility.Collapsed;
			}
			if (dataPoint.DpInfo.Marker.BevelLayer != null)
			{
				dataPoint.DpInfo.Marker.BevelLayer.Visibility = Visibility.Collapsed;
			}
		}

		internal static void ShowDataPointMarker(IDataPoint dataPoint)
		{
			Brush brush = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerColor);
			Brush brush2 = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor);
			if (brush != null)
			{
				dataPoint.DpInfo.Marker.MarkerShape.Fill = brush;
			}
			if (brush2 != null)
			{
				dataPoint.DpInfo.Marker.MarkerShape.Stroke = brush2;
			}
			else
			{
				dataPoint.DpInfo.Marker.MarkerShape.Stroke = dataPoint.Color;
			}
			if (dataPoint.DpInfo.Marker.MarkerShadow != null)
			{
				dataPoint.DpInfo.Marker.MarkerShadow.Visibility = Visibility.Visible;
			}
			if (dataPoint.DpInfo.Marker.BevelLayer != null)
			{
				dataPoint.DpInfo.Marker.BevelLayer.Visibility = Visibility.Visible;
			}
		}

		private static Canvas GetOrUpdateRasterLine2DWithMarker(DataSeries seriesAsTagReference, double width, double height, LineChartShapeParams lineParams, List<List<IDataPoint>> pointCollectionList)
		{
			Canvas plotAreaCanvas = (seriesAsTagReference.Chart as Chart).ChartArea.PlotAreaCanvas;
			int num = (int)plotAreaCanvas.Width;
			int num2 = (int)plotAreaCanvas.Height;
			if (num2 == 0 || num == 0)
			{
				return null;
			}
			RasterLineStyle rasterLineStyle = new RasterLineStyle
			{
				Stroke = (lineParams.LineColor as SolidColorBrush).Color,
				Fill = ((lineParams.LineFill == null) ? Colors.Transparent : (lineParams.LineFill as SolidColorBrush).Color),
				StrokeThickness = lineParams.LineThickness,
				IsAntiAliased = seriesAsTagReference.IsAntiAliased,
				StrokeStyle = seriesAsTagReference.LineStyle,
				Opacity = lineParams.Opacity
			};
			RasterMarkerStyle rasterMarkerStyle = new RasterMarkerStyle();
			bool value = seriesAsTagReference.MarkerEnabled.Value;
			Brush brush = seriesAsTagReference.MarkerColor;
			if (brush == null)
			{
				brush = new SolidColorBrush(Colors.White);
			}
			rasterMarkerStyle.Fill = (brush as SolidColorBrush).Color;
			rasterMarkerStyle.MarkerType = seriesAsTagReference.MarkerType;
			double num3 = seriesAsTagReference.MarkerSize.Value;
			rasterMarkerStyle.Width = num3;
			rasterMarkerStyle.Height = num3;
			if (!((Thickness?)seriesAsTagReference.GetValue(DataSeries.MarkerBorderThicknessProperty)).HasValue)
			{
				rasterMarkerStyle.StrokeThickness = num3 / 6.0;
			}
			else
			{
				rasterMarkerStyle.StrokeThickness = seriesAsTagReference.MarkerBorderThickness.Value.Left;
			}
			double num4 = seriesAsTagReference.PlotGroup.AxisX.GetAxisVisualLeftAsOffset();
			if (double.IsNaN(num4))
			{
				num4 = 0.0;
			}
			Canvas canvas;
			using (WriteableBitmapAdapter writeableBitmapAdapter = new WriteableBitmapAdapter(num, num2))
			{
				foreach (List<IDataPoint> current in pointCollectionList)
				{
					for (int i = 0; i < current.Count; i++)
					{
						if (num4 != 0.0)
						{
							Point visualPosition = current[i].DpInfo.VisualPosition;
							visualPosition.X -= Math.Abs(num4);
							current[i].DpInfo.RasterVisualPosition = visualPosition;
						}
						else
						{
							current[i].DpInfo.RasterVisualPosition = current[i].DpInfo.VisualPosition;
						}
					}
					for (int j = 0; j < current.Count - 1; j++)
					{
						IDataPoint dataPoint = current[j];
						IDataPoint dataPoint2 = current[j + 1];
						rasterLineStyle.PenLineJoinA = (value ? null : new PenLineJoin?(PenLineJoin.Round));
						if (j + 1 == current.Count - 1)
						{
							rasterLineStyle.PenLineJoinB = (value ? null : new PenLineJoin?(PenLineJoin.Round));
						}
						RasterRenderEngine.DrawLine(writeableBitmapAdapter, dataPoint.DpInfo.RasterVisualPosition, dataPoint2.DpInfo.RasterVisualPosition, rasterLineStyle);
					}
					if (value)
					{
						for (int k = 0; k < current.Count; k++)
						{
							LineChart.DrawMarkerCentered(writeableBitmapAdapter, current[k], rasterMarkerStyle);
						}
					}
				}
				if (seriesAsTagReference.Faces != null && seriesAsTagReference.Faces.Parts.Count > 0)
				{
					canvas = (seriesAsTagReference.Faces.Parts[0] as Canvas);
					if (canvas == null)
					{
						seriesAsTagReference.Faces.Parts.Clear();
						canvas = new Canvas
						{
							Width = width,
							Height = height,
							Tag = new ElementData
							{
								Element = seriesAsTagReference
							}
						};
						seriesAsTagReference.Faces.Parts.Add(canvas);
						Image image = new Image
						{
							Height = (double)num2,
							Width = (double)num,
							IsHitTestVisible = false
						};
						image.Source = writeableBitmapAdapter.GetImageSource();
						image.SetValue(Canvas.LeftProperty, Math.Abs(num4));
						canvas.Children.Add(image);
						Canvas canvas2 = seriesAsTagReference.Faces.Visual as Canvas;
						Canvas canvas3 = canvas2.Parent as Canvas;
						if (canvas3 != null)
						{
							canvas3.Children.Clear();
							canvas3.Children.Add(canvas);
						}
						seriesAsTagReference.Faces.Visual = canvas;
						return canvas;
					}
					canvas.Height = height;
					canvas.Width = width;
					canvas.Background = null;
					canvas.Tag = new ElementData
					{
						Element = seriesAsTagReference
					};
					Image image2 = canvas.Children[0] as Image;
					image2.Width = (double)num;
					image2.Height = (double)num2;
					image2.Source = null;
					image2.Source = writeableBitmapAdapter.GetImageSource();
					image2.SetValue(Canvas.LeftProperty, Math.Abs(num4));
				}
				else
				{
					canvas = new Canvas
					{
						Width = width,
						Height = height,
						Tag = new ElementData
						{
							Element = seriesAsTagReference
						}
					};
					seriesAsTagReference.Faces.Parts.Add(canvas);
					Image image3 = new Image
					{
						Height = (double)num2,
						Width = (double)num,
						IsHitTestVisible = false
					};
					image3.Source = writeableBitmapAdapter.GetImageSource();
					image3.SetValue(Canvas.LeftProperty, Math.Abs(num4));
					canvas.Children.Add(image3);
				}
			}
			return canvas;
		}

		private static Canvas GetLine2D(DataSeries tagReference, double width, double height, Canvas line2dLabelCanvas, LineChartShapeParams lineParams, out Path line, out Path lineShadow, List<List<IDataPoint>> pointCollectionList, List<List<IDataPoint>> shadowPointCollectionList)
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
			line.Fill = (lineParams.Lighting ? Graphics.GetLightingEnabledBrush(lineParams.LineFill, "Linear", null) : lineParams.LineFill);
			line.Stroke = (lineParams.Lighting ? Graphics.GetLightingEnabledBrush(lineParams.LineColor, "Linear", new double[]
			{
				0.65,
				0.55
			}) : lineParams.LineColor);
			line.StrokeThickness = lineParams.LineThickness;
			line.StrokeDashArray = lineParams.LineStyle;
			line.Opacity = lineParams.Opacity;
			line.Data = LineChart.GetPathGeometry(tagReference.RenderAs, null, pointCollectionList, false, width, height, line2dLabelCanvas);
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
						lineShadow.Data = LineChart.GetPathGeometry(tagReference.RenderAs, null, shadowPointCollectionList, true, width, height, null);
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

		internal static Geometry GetPathGeometry(RenderAs renderAs, GeometryGroup oldData, List<List<IDataPoint>> dataPointCollectionList, bool isShadow, double width, double height, Canvas line2dLabelCanvas)
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
						LineChart.CreateMarkerAndLabel(current, 0, width, height, line2dLabelCanvas);
					}
					if (renderAs == RenderAs.Spline)
					{
						LineChart.GeneratePointCollections4SpLine(current, width, height, isShadow, pathFigure, ref line2dLabelCanvas);
					}
					else
					{
						LineChart.GeneratePointCollections4Line(current, width, height, isShadow, pathFigure, ref line2dLabelCanvas);
					}
				}
				pathGeometry.Figures.Add(pathFigure);
				geometryGroup.Children.Add(pathGeometry);
			}
			return geometryGroup;
		}

		private static void CreateMarkerAndLabel(List<IDataPoint> pointCollection, int index, double width, double height, Canvas line2dLabelCanvas)
		{
			if (pointCollection[index].Parent != null && pointCollection[index].Parent.RenderAs != RenderAs.QuickLine)
			{
				double markerLeft = 0.0;
				double markerTop = 0.0;
				if (pointCollection[index].DpInfo.Marker != null && pointCollection[index].DpInfo.Marker.Visual != null)
				{
					Point point = pointCollection[index].DpInfo.Marker.CalculateActualPosition(pointCollection[index].DpInfo.VisualPosition.X, pointCollection[index].DpInfo.VisualPosition.Y, new Point(0.5, 0.5));
					pointCollection[index].DpInfo.ParsedToolTipText = pointCollection[index].TextParser(pointCollection[index].ToolTipText);
					if (!pointCollection[index].IsLightDataPoint)
					{
						(pointCollection[index] as DataPoint).ParsedHref = pointCollection[index].TextParser((string)DataPointHelper.GetDataPointValueFromProperty(pointCollection[index], VcProperties.Href));
					}
					pointCollection[index].DpInfo.Marker.Visual.Visibility = Visibility.Visible;
					pointCollection[index].DpInfo.Marker.Visual.SetValue(Canvas.TopProperty, point.Y);
					pointCollection[index].DpInfo.Marker.Visual.SetValue(Canvas.LeftProperty, point.X);
				}
				else
				{
					LineChart.CreateMarkerAForLineDataPoint(pointCollection[index], width, height, ref line2dLabelCanvas, out markerLeft, out markerTop);
					double internalYValue = pointCollection[index].DpInfo.InternalYValue;
					if (pointCollection[index].Parent.PlotGroup.AxisY.AxisMinimum != null)
					{
						double limitingYValue = pointCollection[index].Parent.PlotGroup.GetLimitingYValue();
						if (internalYValue > pointCollection[index].Parent.PlotGroup.AxisY.InternalAxisMaximum || (internalYValue < limitingYValue && limitingYValue > 0.0) || (internalYValue < limitingYValue && pointCollection[index].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue < pointCollection[index].Parent.PlotGroup.AxisY.InternalAxisMinimum)
						{
							pointCollection[index].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
						}
					}
					if (pointCollection[index].Parent.PlotGroup.AxisY.AxisMaximum != null && internalYValue > pointCollection[index].Parent.PlotGroup.AxisY.InternalAxisMaximum)
					{
						pointCollection[index].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
					}
				}
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(pointCollection[index], VcProperties.LabelEnabled))
				{
					if (pointCollection[index].DpInfo.LabelVisual != null)
					{
						double num = 0.0;
						double num2 = 0.0;
						LineChart.CreateLabel4LineDataPoint(pointCollection[index], width, height, pointCollection[index].DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref line2dLabelCanvas, true);
						LineChart.SetLabelPosition4LineDataPoint(pointCollection[index], ref num, ref num2);
						(((pointCollection[index].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Text = pointCollection[index].TextParser((string)DataPointHelper.GetDataPointValueFromProperty(pointCollection[index], VcProperties.LabelText));
						pointCollection[index].DpInfo.LabelVisual.Visibility = Visibility.Visible;
						pointCollection[index].DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, num);
						pointCollection[index].DpInfo.LabelVisual.SetValue(Canvas.TopProperty, num2);
						return;
					}
					LineChart.CreateLabel4LineDataPoint(pointCollection[index], width, height, pointCollection[index].DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref line2dLabelCanvas, true);
					double internalYValue2 = pointCollection[index].DpInfo.InternalYValue;
					if (pointCollection[index].Parent.PlotGroup.AxisY.AxisMinimum != null)
					{
						double limitingYValue2 = pointCollection[index].Parent.PlotGroup.GetLimitingYValue();
						if (((internalYValue2 < limitingYValue2 && limitingYValue2 > 0.0) || (internalYValue2 < limitingYValue2 && pointCollection[index].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue2 < pointCollection[index].Parent.PlotGroup.AxisY.InternalAxisMinimum) && pointCollection[index].DpInfo.LabelVisual != null)
						{
							(((pointCollection[index].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Collapsed;
						}
					}
					if (pointCollection[index].Parent.PlotGroup.AxisY.AxisMaximum != null && internalYValue2 > pointCollection[index].Parent.PlotGroup.AxisY.InternalAxisMaximum && pointCollection[index].DpInfo.LabelVisual != null)
					{
						(((pointCollection[index].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Collapsed;
					}
				}
			}
		}

		public static double FindPointOnLine(double targetXValue, Point[] knotPoints, DataSeries series)
		{
			if (knotPoints.Count<Point>() == 0 || targetXValue < knotPoints[0].X || targetXValue > knotPoints[knotPoints.Length - 1].X)
			{
				return double.NaN;
			}
			int num = 0;
			int num2 = knotPoints.Length - 1;
			int num3 = 0;
			while (num3 < 100 && num2 > num + 1)
			{
				int num4 = (num2 + num) / 2;
				if (targetXValue > knotPoints[num4].X)
				{
					num = num4;
				}
				else
				{
					num2 = num4;
				}
				num3++;
			}
			if (series.RenderAs == RenderAs.Spline)
			{
				PointCollection pointCollection = new PointCollection();
				for (int i = 0; i < knotPoints.Length; i++)
				{
					Point value = knotPoints[i];
					pointCollection.Add(value);
				}
				new PointCollection();
				PointCollection pointCollection2;
				PointCollection pointCollection3;
				Bezier.GetBezierPoints(pointCollection, 1.0 / series.LineTension, out pointCollection2, out pointCollection3);
				Point point = knotPoints[num];
				Point point2 = pointCollection2[num];
				Point point3 = pointCollection3[num];
				Point point4 = knotPoints[num2];
				double arg_106_0 = point.X;
				double y = point.Y;
				for (double num5 = 0.0; num5 < 1.0; num5 += 0.01)
				{
					double num6 = num5 * num5 * num5;
					double num7 = 3.0 * num5 * num5 * (1.0 - num5);
					double num8 = 3.0 * num5 * (1.0 - num5) * (1.0 - num5);
					double num9 = (1.0 - num5) * (1.0 - num5) * (1.0 - num5);
					Point point5 = default(Point);
					point5.X = point.X * num6 + point2.X * num7 + point3.X * num8 + point4.X * num9;
					point5.Y = point.Y * num6 + point2.Y * num7 + point3.Y * num8 + point4.Y * num9;
					if (point5.X < targetXValue)
					{
						return y;
					}
					if (point5.X == targetXValue)
					{
						return point5.Y;
					}
					double arg_22A_0 = point5.X;
					y = point5.Y;
				}
				return y;
			}
			if (series.RenderAs == RenderAs.StepLine)
			{
				if (targetXValue == knotPoints[num2].X)
				{
					return knotPoints[num2].Y;
				}
				return knotPoints[num].Y;
			}
			else
			{
				if (series.RenderAs == RenderAs.Line || series.RenderAs == RenderAs.QuickLine)
				{
					Point point6 = knotPoints[num];
					Point point7 = knotPoints[num2];
					double num10 = (point7.Y - point6.Y) / (point7.X - point6.X);
					return num10 * (targetXValue - point6.X) + point6.Y;
				}
				return double.NaN;
			}
		}

		private static void GeneratePointCollections4SpLine(List<IDataPoint> pointCollection, double width, double height, bool isShadow, PathFigure pathFigure, ref Canvas line2dLabelCanvas)
		{
			Point[] array = (from dp in pointCollection
			select dp.DpInfo.VisualPosition).ToArray<Point>();
			double num = 0.0;
			foreach (IDataPoint current in pointCollection)
			{
				if (current.Parent != null)
				{
					num = current.Parent.LineTension;
					break;
				}
			}
			num = 1.0 / num;
			PointCollection pointCollection2 = new PointCollection();
			for (int i = 0; i < array.Length; i++)
			{
				pointCollection2.Add(array[i]);
			}
			new PointCollection();
			PointCollection pointCollection3;
			PointCollection pointCollection4;
			Bezier.GetBezierPoints(pointCollection2, num, out pointCollection3, out pointCollection4);
			LineChart.cp1 = new Point[pointCollection3.Count];
			LineChart.cp2 = new Point[pointCollection4.Count];
			for (int j = 0; j < pointCollection3.Count; j++)
			{
				LineChart.cp1[j] = pointCollection3[j];
			}
			for (int k = 0; k < pointCollection4.Count; k++)
			{
				LineChart.cp2[k] = pointCollection4[k];
			}
			if (pointCollection.Count > 0 && pointCollection[0].DpInfo.Faces != null)
			{
				pointCollection[0].DpInfo.Faces.DataContext = pointCollection;
			}
			for (int l = 1; l < pointCollection.Count; l++)
			{
				Faces faces = new Faces();
				faces.DataContext = pointCollection;
				BezierSegment bezierSegment = new BezierSegment
				{
					Point1 = LineChart.cp1[l - 1],
					Point2 = LineChart.cp2[l - 1],
					Point3 = array[l]
				};
				faces.PreviousDataPoint = pointCollection[l - 1];
				if (!isShadow)
				{
					if (l != pointCollection.Count - 1)
					{
						faces.NextDataPoint = pointCollection[l + 1];
					}
					else
					{
						faces.NextDataPoint = null;
					}
				}
				faces.Parts.Add(bezierSegment);
				faces.Parts.Add(pathFigure);
				if (isShadow)
				{
					pointCollection[l].DpInfo.ShadowFaces = faces;
				}
				else
				{
					pointCollection[l].DpInfo.Faces = faces;
				}
				pathFigure.Segments.Add(bezierSegment);
				if (!isShadow)
				{
					LineChart.CreateMarkerAndLabel(pointCollection, l, width, height, line2dLabelCanvas);
				}
			}
		}

		private static void GeneratePointCollections4Line(List<IDataPoint> pointCollection, double width, double height, bool isShadow, PathFigure pathFigure, ref Canvas line2dLabelCanvas)
		{
			for (int i = 1; i < pointCollection.Count; i++)
			{
				LineSegment lineSegment = new LineSegment();
				lineSegment.Point = pointCollection[i].DpInfo.VisualPosition;
				Faces faces = new Faces();
				faces.PreviousDataPoint = pointCollection[i - 1];
				if (!isShadow)
				{
					if (i != pointCollection.Count - 1)
					{
						faces.NextDataPoint = pointCollection[i + 1];
					}
					else
					{
						faces.NextDataPoint = null;
					}
				}
				faces.Parts.Add(lineSegment);
				faces.Parts.Add(pathFigure);
				if (isShadow)
				{
					pointCollection[i].DpInfo.ShadowFaces = faces;
				}
				else
				{
					pointCollection[i].DpInfo.Faces = faces;
				}
				pathFigure.Segments.Add(lineSegment);
				if (!isShadow)
				{
					LineChart.CreateMarkerAndLabel(pointCollection, i, width, height, line2dLabelCanvas);
				}
			}
		}

		public static void Update(IElement sender, ElementTypes elementType, VcProperties property, object newValue, bool isAxisChanged)
		{
			bool flag = elementType == ElementTypes.DataPoint;
			if (flag)
			{
				LineChart.UpdateDataPoint(sender as IDataPoint, property, newValue);
				return;
			}
			LineChart.UpdateDataSeries(sender as DataSeries, property, newValue);
		}

		private static void UpdateDataSeries(IElement obj, VcProperties property, object newValue)
		{
			DataSeries dataSeries = obj as DataSeries;
			if (dataSeries != null)
			{
				dataSeries.isSeriesRenderPending = false;
			}
			bool flag = false;
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
			PlotGroup arg_4D_0 = dataSeries.PlotGroup;
			Path path = null;
			Path path2 = null;
			if (dataSeries.Faces == null)
			{
				if (dataSeries.Faces == null && property == VcProperties.Enabled && (bool)newValue)
				{
					ColumnChart.Update(chart, dataSeries.RenderAs, (from ds in chart.InternalSeries
					where ds.RenderAs == RenderAs.Spline || ds.RenderAs == RenderAs.Line
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
			Canvas canvas = dataSeries.Faces.Visual as Canvas;
			Canvas labelCanvas = dataSeries.Faces.LabelCanvas;
			double height = chart.ChartArea.ChartVisualCanvas.Height;
			double width = chart.ChartArea.ChartVisualCanvas.Width;
			if (dataSeries.LightWeight.Value && dataSeries.RenderAs == RenderAs.Line)
			{
				LineChart.UpdateLineSeries(dataSeries, width, height, labelCanvas);
				return;
			}
			if (property <= VcProperties.LineThickness)
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
				else
				{
					switch (property)
					{
					case VcProperties.DataPoints:
					case VcProperties.DataPointUpdate:
					case VcProperties.Effect:
						break;
					case VcProperties.DisplayAutoAxisLabels:
					case VcProperties.DockInsidePlotArea:
						return;
					case VcProperties.Enabled:
						if (!flag && canvas != null)
						{
							if (!(bool)newValue)
							{
								canvas.Visibility = Visibility.Collapsed;
								labelCanvas.Visibility = Visibility.Collapsed;
							}
							else
							{
								if (canvas.Parent == null)
								{
									ColumnChart.Update(chart, dataSeries.RenderAs, (from ds in chart.InternalSeries
									where ds.RenderAs == RenderAs.Line || ds.RenderAs == RenderAs.Spline
									select ds).ToList<DataSeries>());
									return;
								}
								canvas.Visibility = Visibility.Visible;
								labelCanvas.Visibility = Visibility.Visible;
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
						case VcProperties.LineColor:
						case VcProperties.LineTension:
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
						case VcProperties.LineFill:
							if (path != null)
							{
								Brush brush2 = (newValue != null) ? (newValue as Brush) : dataSeries.LineFill;
								path.Fill = (dataSeries.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(brush2, "Linear", null) : brush2);
								return;
							}
							return;
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
						default:
							return;
						}
						break;
					}
				}
			}
			else if (property <= VcProperties.ShadowEnabled)
			{
				if (property != VcProperties.Opacity)
				{
					if (property != VcProperties.ShadowEnabled)
					{
						return;
					}
				}
				else
				{
					if (path != null)
					{
						path.Opacity = dataSeries.Opacity;
						return;
					}
					return;
				}
			}
			else if (property != VcProperties.XValue)
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
			if (dataSeries.RenderAs == RenderAs.Spline && property != VcProperties.DataPointUpdate)
			{
				dataSeries.StopDataSeriesAnimation();
			}
			else if (dataSeries.RenderAs == RenderAs.Line)
			{
				dataSeries.StopDataPointsAnimation();
			}
			if (dataSeries.RenderAs == RenderAs.Line)
			{
				LineChart.UpdateLineSeries(dataSeries, width, height, labelCanvas);
				return;
			}
			LineChart.UpdateSplineSeries(property, dataSeries, width, height, labelCanvas);
		}

		internal static void UpdateSplineSeries(VcProperties property, DataSeries dataSeries, double width, double height, Canvas label2dCanvas)
		{
			if (dataSeries == null)
			{
				return;
			}
			Chart chart = dataSeries.Chart as Chart;
			bool isOffsetAdded = true;
			if (dataSeries.Enabled == false || chart == null)
			{
				return;
			}
			bool value = chart.AnimatedUpdate.Value;
			if (dataSeries.Faces == null || dataSeries.Faces.Visual == null || dataSeries.Faces.Visual.Parent == null || dataSeries.Faces.LabelCanvas == null || dataSeries.Faces.LabelCanvas.Parent == null)
			{
				return;
			}
			Canvas chartsCanvas = dataSeries.Faces.Visual.Parent as Canvas;
			Canvas labelCanvas = dataSeries.Faces.LabelCanvas.Parent as Canvas;
			LineChart.UpdateSizeOfTheParentCanvas(dataSeries, width, height);
			List<List<IDataPoint>> list = new List<List<IDataPoint>>();
			LineChart.HideAllMarkersAndLabels(dataSeries);
			List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(dataSeries, false, false);
			LineChart.CreateGroupOfBrokenLineSegment(dataSeries, dataPointsUnderViewPort, width, height, out list);
			if (property != VcProperties.DataPointUpdate || !value)
			{
				LineChart.ReDrawLineSplineSereis(dataSeries, list, width, height, label2dCanvas);
			}
			if (property == VcProperties.DataPointUpdate && value)
			{
				bool flag = false;
				foreach (IDataPoint current in dataPointsUnderViewPort)
				{
					if ((!double.IsNaN(current.DpInfo.OldYValue) && double.IsNaN(current.DpInfo.InternalYValue)) || (!double.IsNaN(current.DpInfo.InternalYValue) && double.IsNaN(current.DpInfo.OldYValue)))
					{
						current.DpInfo.OldYValue = current.DpInfo.InternalYValue;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					foreach (List<IDataPoint> current2 in list)
					{
						foreach (IDataPoint current3 in current2)
						{
							if (current3.DpInfo.Faces == null)
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
				if (flag)
				{
					LineChart.ReDrawLineSplineSereis(dataSeries, list, width, height, label2dCanvas);
				}
				dataSeries.Storyboard = LineChart.AnimateSpline(list);
				if (dataSeries.Storyboard != null)
				{
					dataSeries.Storyboard.Begin();
				}
				if (dataSeries._movingMarker != null)
				{
					dataSeries._movingMarker.Visibility = Visibility.Collapsed;
				}
				chart._toolTip.Hide();
				if (dataSeries.ToolTipElement != null)
				{
					dataSeries.ToolTipElement.Hide();
				}
				chart.ChartArea.DisableIndicators();
			}
			dataSeries._movingMarker.Visibility = Visibility.Collapsed;
			List<IDataPoint> list2 = (from datapoint in chart.PlotDetails.ListOfAllDataPoints
			where datapoint.Enabled == true
			select datapoint).ToList<IDataPoint>();
			for (int i = 0; i < list2.Count; i++)
			{
				if (list2[i].Parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					double limitingYValue = list2[i].Parent.PlotGroup.GetLimitingYValue();
					if ((list2[i].DpInfo.InternalYValue < limitingYValue && limitingYValue > 0.0) || (list2[i].DpInfo.InternalYValue < limitingYValue && list2[i].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || list2[i].DpInfo.InternalYValue < list2[i].Parent.PlotGroup.AxisY.InternalAxisMinimum)
					{
						if (list2[i].DpInfo.Marker != null && list2[i].DpInfo.Marker.Visual != null)
						{
							list2[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
						}
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(list2[i], VcProperties.LabelEnabled) && list2[i].DpInfo.LabelVisual != null)
						{
							list2[i].DpInfo.LabelVisual.Visibility = Visibility.Collapsed;
						}
					}
					else if (list2[i].DpInfo.LabelVisual != null)
					{
						(((list2[i].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Visible;
					}
				}
				if (list2[i].Parent.PlotGroup.AxisY.AxisMaximum != null && list2[i].DpInfo.InternalYValue > list2[i].Parent.PlotGroup.AxisY.InternalAxisMaximum)
				{
					if (list2[i].DpInfo.Marker != null && list2[i].DpInfo.Marker.Visual != null)
					{
						list2[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
					}
					if ((bool)DataPointHelper.GetDataPointValueFromProperty(list2[i], VcProperties.LabelEnabled) && list2[i].DpInfo.LabelVisual != null)
					{
						list2[i].DpInfo.LabelVisual.Visibility = Visibility.Collapsed;
					}
				}
			}
			for (int j = 0; j < list2.Count; j++)
			{
				if (list2[j].Parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					double limitingYValue2 = list2[j].Parent.PlotGroup.GetLimitingYValue();
					if (list2[j].DpInfo.InternalYValue < limitingYValue2)
					{
						isOffsetAdded = false;
						break;
					}
				}
			}
			LineChart.Clip(chart, chartsCanvas, labelCanvas, dataSeries.PlotGroup, isOffsetAdded);
		}

		private static void ReDrawLineSplineSereis(DataSeries dataSeries, List<List<IDataPoint>> pointCollectionList, double width, double height, Canvas label2dCanvas)
		{
			GeometryGroup oldData = (dataSeries.Faces.Parts[0] as Path).Data as GeometryGroup;
			LineChart.GetPathGeometry(dataSeries.RenderAs, oldData, pointCollectionList, false, width, height, label2dCanvas);
			LineChart.ApplyEffects(dataSeries);
			LineChart.ApplyShadow(dataSeries, ref pointCollectionList, width, height, label2dCanvas);
		}

		internal static void UpdateSizeOfTheParentCanvas(DataSeries dataSeries, double width, double height)
		{
			(dataSeries.Faces.Visual as Canvas).Width = width;
			(dataSeries.Faces.Visual as Canvas).Height = height;
			dataSeries.Faces.LabelCanvas.Width = width;
			dataSeries.Faces.LabelCanvas.Height = height;
			Canvas canvas = dataSeries.Faces.Visual.Parent as Canvas;
			Canvas canvas2 = dataSeries.Faces.LabelCanvas.Parent as Canvas;
			canvas.Width = width;
			canvas.Height = height;
			canvas2.Width = width;
			canvas2.Height = height;
		}

		internal static void CreateGroupOfBrokenLineSegment(DataSeries dataSeries, List<IDataPoint> viewPortDataPoints, double width, double height, out List<List<IDataPoint>> pointCollectionList)
		{
			Chart chart = dataSeries.Chart as Chart;
			Axis axisX = dataSeries.PlotGroup.AxisX;
			Axis axisY = dataSeries.PlotGroup.AxisY;
			double internalAxisMinimum = axisX.InternalAxisMinimum;
			double internalAxisMaximum = axisX.InternalAxisMaximum;
			double internalAxisMinimum2 = axisY.InternalAxisMinimum;
			double internalAxisMaximum2 = axisY.InternalAxisMaximum;
			List<IDataPoint> list = new List<IDataPoint>();
			pointCollectionList = new List<List<IDataPoint>>();
			pointCollectionList.Add(list);
			RenderAs renderAs = dataSeries.RenderAs;
			bool indicatorEnabled = chart.IndicatorEnabled;
			foreach (IDataPoint current in viewPortDataPoints)
			{
				if (!dataSeries.IsAllDataPointsEnabled && current.Enabled == false)
				{
					chart._toolTip.Hide();
				}
				else
				{
					double internalYValue = current.DpInfo.InternalYValue;
					if (double.IsNaN(internalYValue))
					{
						current.DpInfo.Faces = null;
						list = new List<IDataPoint>();
						pointCollectionList.Add(list);
					}
					else
					{
						double x = Graphics.ValueToPixelPosition(0.0, width, internalAxisMinimum, internalAxisMaximum, current.DpInfo.InternalXValue);
						double y = Graphics.ValueToPixelPosition(height, 0.0, internalAxisMinimum2, internalAxisMaximum2, internalYValue);
						current.DpInfo.VisualPosition = new Point(x, y);
						if (indicatorEnabled)
						{
							if (dataSeries.LightWeight.Value)
							{
								current.DpInfo.ParsedToolTipText = current.TextParser(current.ToolTipText);
							}
							else if (renderAs == RenderAs.QuickLine)
							{
								current.DpInfo.ParsedToolTipText = current.TextParser(current.ToolTipText);
								if (!current.IsLightDataPoint)
								{
									(current as DataPoint).ParsedHref = current.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.Href));
								}
							}
						}
						list.Add(current);
					}
				}
			}
		}

		private static void ApplyShadow(DataSeries dataSeries, ref List<List<IDataPoint>> pointCollectionList, double width, double height, Canvas label2dCanvas)
		{
			if (!VisifireControl.IsMediaEffectsEnabled)
			{
				if (dataSeries.Faces.Parts[1] != null)
				{
					if (dataSeries.ShadowEnabled.Value)
					{
						(dataSeries.Faces.Parts[1] as Path).Visibility = Visibility.Visible;
						GeometryGroup oldData = (dataSeries.Faces.Parts[1] as Path).Data as GeometryGroup;
						LineChart.GetPathGeometry(dataSeries.RenderAs, oldData, pointCollectionList, true, width, height, label2dCanvas);
						return;
					}
					(dataSeries.Faces.Parts[1] as Path).Visibility = Visibility.Collapsed;
					return;
				}
			}
			else if (dataSeries.Faces != null && dataSeries.Faces.Visual != null && dataSeries.Effect == null)
			{
				if (dataSeries.ShadowEnabled.Value)
				{
					dataSeries.Faces.Visual.Effect = ExtendedGraphics.GetShadowEffect(315.0, 2.5, 1.0);
					return;
				}
				dataSeries.Faces.Visual.Effect = null;
			}
		}

		private static void ApplyEffects(DataSeries dataSeries)
		{
			if (dataSeries.Faces != null && dataSeries.Faces.Visual != null)
			{
				dataSeries.Faces.Visual.Effect = dataSeries.Effect;
			}
		}

		private static void HideAllMarkersAndLabels(DataSeries dataSeries)
		{
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
			}
		}

		private static Storyboard AnimateSpline(List<List<IDataPoint>> pointCollectionList)
		{
			Storyboard storyboard = new Storyboard();
			foreach (List<IDataPoint> current in pointCollectionList)
			{
				LineChart.AnimateBezierCurves(current, storyboard);
				LineChart.AnimateMarkers4Spline(current, storyboard);
				LineChart.AnimateLabel4Spline(current, storyboard);
			}
			return storyboard;
		}

		private static Point[] CollectOldControlPoints(int index, List<IDataPoint> pointCollection, out Point oldStartPoint)
		{
			IDataPoint dataPoint = pointCollection[index];
			BezierSegment bezierSegment = dataPoint.DpInfo.Faces.Parts[0] as BezierSegment;
			PathFigure pathFigure = dataPoint.DpInfo.Faces.Parts[1] as PathFigure;
			oldStartPoint = pathFigure.StartPoint;
			if (bezierSegment != null)
			{
				return new Point[]
				{
					bezierSegment.Point1,
					bezierSegment.Point2,
					bezierSegment.Point3
				};
			}
			return null;
		}

		private static void AnimateBezierCurves(List<IDataPoint> pointCollection, Storyboard storyBoard)
		{
			bool flag = pointCollection.Count > 0 && pointCollection[0].Parent.ShadowEnabled.Value;
			double num = 0.0;
			using (List<IDataPoint>.Enumerator enumerator = pointCollection.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					IDataPoint current = enumerator.Current;
					num = current.Parent.LineTension;
				}
			}
			num = 1.0 / num;
			Point[] array = (from dp in pointCollection
			select dp.DpInfo.VisualPosition).ToArray<Point>();
			PointCollection pointCollection2 = new PointCollection();
			for (int i = 0; i < array.Length; i++)
			{
				pointCollection2.Add(array[i]);
			}
			new PointCollection();
			PointCollection pointCollection3;
			PointCollection pointCollection4;
			Bezier.GetBezierPoints(pointCollection2, num, out pointCollection3, out pointCollection4);
			Point[] array2 = new Point[pointCollection3.Count];
			Point[] array3 = new Point[pointCollection4.Count];
			for (int j = 0; j < pointCollection3.Count; j++)
			{
				array2[j] = pointCollection3[j];
			}
			for (int k = 0; k < pointCollection4.Count; k++)
			{
				array3[k] = pointCollection4[k];
			}
			for (int l = 1; l < pointCollection.Count; l++)
			{
				IDataPoint dataPoint = pointCollection[l];
				Point oldFirstPointOfBezierSeg;
				Point[] oldCtrlPoints = LineChart.CollectOldControlPoints(l, pointCollection, out oldFirstPointOfBezierSeg);
				Point[] newCtrlPoints = new Point[]
				{
					array2[l - 1],
					array3[l - 1],
					array[l]
				};
				BezierSegment bezierSegment = dataPoint.DpInfo.Faces.Parts[0] as BezierSegment;
				PathFigure pathFigure = dataPoint.DpInfo.Faces.Parts[1] as PathFigure;
				LineChart.ApplyAnimationToBezierSegments(l, storyBoard, bezierSegment, oldCtrlPoints, newCtrlPoints, pathFigure, oldFirstPointOfBezierSeg, pointCollection[0].DpInfo.VisualPosition);
				if (flag && !VisifireControl.IsMediaEffectsEnabled)
				{
					bezierSegment = (dataPoint.DpInfo.ShadowFaces.Parts[0] as BezierSegment);
					pathFigure = (dataPoint.DpInfo.ShadowFaces.Parts[1] as PathFigure);
					LineChart.ApplyAnimationToBezierSegments(l, storyBoard, bezierSegment, oldCtrlPoints, newCtrlPoints, pathFigure, oldFirstPointOfBezierSeg, pointCollection[0].DpInfo.VisualPosition);
				}
			}
		}

		private static void AnimateLabel4Spline(List<IDataPoint> pointCollection, Storyboard storyBoard)
		{
			foreach (IDataPoint current in pointCollection)
			{
				bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LabelEnabled);
				if (flag)
				{
					FrameworkElement labelVisual = current.DpInfo.LabelVisual;
					if (labelVisual != null)
					{
						(((labelVisual as Border).Child as Canvas).Children[0] as TextBlock).Text = current.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LabelText));
						Point point = new Point((double)labelVisual.GetValue(Canvas.LeftProperty), (double)labelVisual.GetValue(Canvas.TopProperty));
						double num = 0.0;
						double num2 = 0.0;
						Point value = LineChart.SetLabelPosition4LineDataPoint(current, ref num, ref num2);
						labelVisual.Visibility = Visibility.Visible;
						if (!point.Equals(value))
						{
							DoubleAnimation doubleAnimation = new DoubleAnimation
							{
								From = new double?(point.X),
								To = new double?(value.X),
								Duration = new Duration(new TimeSpan(0, 0, 1)),
								SpeedRatio = 2.0
							};
							Storyboard.SetTarget(doubleAnimation, labelVisual);
							Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)", new object[0]));
							Storyboard.SetTargetName(doubleAnimation, (string)labelVisual.GetValue(FrameworkElement.NameProperty));
							storyBoard.Children.Add(doubleAnimation);
							doubleAnimation = new DoubleAnimation
							{
								From = new double?(point.Y),
								To = new double?(value.Y),
								Duration = new Duration(new TimeSpan(0, 0, 1)),
								SpeedRatio = 2.0
							};
							Storyboard.SetTarget(doubleAnimation, labelVisual);
							Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)", new object[0]));
							Storyboard.SetTargetName(doubleAnimation, (string)labelVisual.GetValue(FrameworkElement.NameProperty));
							storyBoard.Children.Add(doubleAnimation);
						}
					}
				}
			}
		}

		private static void AnimateMarkers4Spline(List<IDataPoint> pointCollection, Storyboard storyBoard)
		{
			foreach (IDataPoint current in pointCollection)
			{
				current.DpInfo.ParsedToolTipText = current.TextParser(current.ToolTipText);
				if (!current.IsLightDataPoint)
				{
					(current as DataPoint).ParsedHref = current.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.Href));
				}
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.MarkerEnabled))
				{
					FrameworkElement visual = current.DpInfo.Marker.Visual;
					if (visual != null)
					{
						Point value = current.DpInfo.Marker.CalculateActualPosition(current.DpInfo.VisualPosition.X, current.DpInfo.VisualPosition.Y, new Point(0.5, 0.5));
						Point point = new Point((double)visual.GetValue(Canvas.LeftProperty), (double)visual.GetValue(Canvas.TopProperty));
						current.DpInfo.Marker.Visual.Visibility = Visibility.Visible;
						if (!point.Equals(value))
						{
							DoubleAnimation doubleAnimation = new DoubleAnimation
							{
								From = new double?(point.X),
								To = new double?(value.X),
								Duration = new Duration(new TimeSpan(0, 0, 1)),
								SpeedRatio = 2.0
							};
							Storyboard.SetTarget(doubleAnimation, visual);
							Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)", new object[0]));
							Storyboard.SetTargetName(doubleAnimation, (string)visual.GetValue(FrameworkElement.NameProperty));
							storyBoard.Children.Add(doubleAnimation);
							doubleAnimation = new DoubleAnimation
							{
								From = new double?(point.Y),
								To = new double?(value.Y),
								Duration = new Duration(new TimeSpan(0, 0, 1)),
								SpeedRatio = 2.0
							};
							Storyboard.SetTarget(doubleAnimation, visual);
							Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)", new object[0]));
							Storyboard.SetTargetName(doubleAnimation, (string)visual.GetValue(FrameworkElement.NameProperty));
							storyBoard.Children.Add(doubleAnimation);
						}
					}
				}
			}
		}

		internal static void UpdateLineSeries(DataSeries dataSeries, double width, double height, Canvas label2dCanvas)
		{
			if (dataSeries == null)
			{
				return;
			}
			Chart chart = dataSeries.Chart as Chart;
			bool isOffsetAdded = true;
			if (dataSeries.Enabled == false || chart == null)
			{
				return;
			}
			if (dataSeries.Faces == null || dataSeries.Faces.Visual == null || dataSeries.Faces.Visual.Parent == null || dataSeries.Faces.LabelCanvas == null || dataSeries.Faces.LabelCanvas.Parent == null)
			{
				return;
			}
			Canvas chartsCanvas = dataSeries.Faces.Visual.Parent as Canvas;
			Canvas labelCanvas = dataSeries.Faces.LabelCanvas.Parent as Canvas;
			LineChart.UpdateSizeOfTheParentCanvas(dataSeries, width, height);
			List<List<IDataPoint>> list = new List<List<IDataPoint>>();
			LineChart.HideAllMarkersAndLabels(dataSeries);
			List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(dataSeries, false, RenderHelper.IsRasterRenderSupported(dataSeries));
			List<IDataPoint> list2 = (from datapoint in dataPointsUnderViewPort
			where datapoint.Enabled == true
			select datapoint).ToList<IDataPoint>();
			LineChart.CreateGroupOfBrokenLineSegment(dataSeries, list2, width, height, out list);
			if (dataSeries.LightWeight.Value)
			{
				if (dataSeries.ToolTipElement != null)
				{
					dataSeries.ToolTipElement.Hide();
				}
				chart.ChartArea.DisableIndicators();
				dataSeries.VisualParams = new LineChartShapeParams
				{
					LineThickness = dataSeries.LineThickness.Value,
					LineColor = dataSeries.Color,
					LineFill = dataSeries.LineFill,
					LineStyle = ExtendedGraphics.GetDashArray(dataSeries.LineStyle),
					Lighting = dataSeries.LightingEnabled.Value,
					Opacity = dataSeries.Opacity
				};
				LineChart.GetOrUpdateRasterLine2DWithMarker(dataSeries, width, height, dataSeries.VisualParams as LineChartShapeParams, list);
			}
			else
			{
				List<DataSeries> list3 = new List<DataSeries>();
				list3.Add(dataSeries);
				GeometryGroup oldData = null;
				if (!(dataSeries.Faces.Parts[0] is Path))
				{
					if (dataSeries.Faces.Parts != null)
					{
						(dataSeries.Faces.Visual as Canvas).Children.Clear();
						ColumnChart.Update(chart, dataSeries.RenderAs, list3);
						return;
					}
				}
				else
				{
					oldData = ((dataSeries.Faces.Parts[0] as Path).Data as GeometryGroup);
				}
				LineChart.GetPathGeometry(dataSeries.RenderAs, oldData, list, false, width, height, label2dCanvas);
				if (!VisifireControl.IsMediaEffectsEnabled)
				{
					if (dataSeries.Faces.Parts[1] != null)
					{
						if (dataSeries.ShadowEnabled.Value)
						{
							(dataSeries.Faces.Parts[1] as Path).Visibility = Visibility.Visible;
							GeometryGroup oldData2 = (dataSeries.Faces.Parts[1] as Path).Data as GeometryGroup;
							LineChart.GetPathGeometry(dataSeries.RenderAs, oldData2, list, true, width, height, label2dCanvas);
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
				for (int i = 0; i < list2.Count; i++)
				{
					if (list2[i].Parent.PlotGroup.AxisY.AxisMinimum != null)
					{
						double limitingYValue = list2[i].Parent.PlotGroup.GetLimitingYValue();
						if ((list2[i].DpInfo.InternalYValue < limitingYValue && limitingYValue > 0.0) || (list2[i].DpInfo.InternalYValue < limitingYValue && list2[i].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || list2[i].DpInfo.InternalYValue < list2[i].Parent.PlotGroup.AxisY.InternalAxisMinimum)
						{
							if (list2[i].DpInfo.Marker != null && list2[i].DpInfo.Marker.Visual != null)
							{
								list2[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
							}
							if ((bool)DataPointHelper.GetDataPointValueFromProperty(list2[i], VcProperties.LabelEnabled) && list2[i].DpInfo.LabelVisual != null)
							{
								list2[i].DpInfo.LabelVisual.Visibility = Visibility.Collapsed;
							}
						}
						else if (list2[i].DpInfo.LabelVisual != null)
						{
							(((list2[i].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Visible;
						}
					}
					if (list2[i].Parent.PlotGroup.AxisY.AxisMaximum != null && dataSeries.RenderAs != RenderAs.QuickLine)
					{
						double internalYValue = list2[i].DpInfo.InternalYValue;
						if (internalYValue > list2[i].Parent.PlotGroup.AxisY.InternalAxisMaximum)
						{
							if (list2[i].DpInfo.Marker != null && list2[i].DpInfo.Marker.Visual != null)
							{
								list2[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
							}
							if ((bool)DataPointHelper.GetDataPointValueFromProperty(list2[i], VcProperties.LabelEnabled) && list2[i].DpInfo.LabelVisual != null)
							{
								list2[i].DpInfo.LabelVisual.Visibility = Visibility.Collapsed;
							}
						}
					}
				}
			}
			if (dataSeries._movingMarker != null)
			{
				dataSeries._movingMarker.Visibility = Visibility.Collapsed;
			}
			for (int j = 0; j < list2.Count; j++)
			{
				if (list2[j].Parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					double limitingYValue2 = list2[j].Parent.PlotGroup.GetLimitingYValue();
					if (list2[j].DpInfo.InternalYValue < limitingYValue2)
					{
						isOffsetAdded = false;
						break;
					}
				}
			}
			LineChart.Clip(chart, chartsCanvas, labelCanvas, dataSeries.PlotGroup, isOffsetAdded);
		}

		private static void DrawMarkerCentered(WriteableBitmapAdapter wb, IDataPoint dataPoint, RasterMarkerStyle parentMarkerStyle)
		{
			RasterRenderEngine.DrawMarkerCentered(wb, dataPoint, parentMarkerStyle, true);
		}

		private static void UpdateRasterMarker(IDataPoint dataPoint)
		{
			Canvas canvas = dataPoint.Parent.Faces.Parts[0] as Canvas;
			using (WriteableBitmapAdapter writeableBitmapAdapter = new WriteableBitmapAdapter((canvas.Children[0] as Image).Source as BitmapSource))
			{
				RasterMarkerStyle rasterMarkerStyle = new RasterMarkerStyle();
				rasterMarkerStyle.Enabled = dataPoint.Parent.MarkerEnabled.Value;
				Brush brush = dataPoint.Parent.MarkerColor;
				if (brush == null)
				{
					brush = new SolidColorBrush(Colors.White);
				}
				rasterMarkerStyle.Fill = (brush as SolidColorBrush).Color;
				double num = dataPoint.Parent.MarkerSize.Value;
				rasterMarkerStyle.Width = num;
				rasterMarkerStyle.Height = num;
				if (!((Thickness?)dataPoint.Parent.GetValue(DataSeries.MarkerBorderThicknessProperty)).HasValue)
				{
					rasterMarkerStyle.StrokeThickness = num / 6.0;
				}
				else
				{
					rasterMarkerStyle.StrokeThickness = dataPoint.Parent.MarkerBorderThickness.Value.Left;
				}
				if (rasterMarkerStyle.Enabled)
				{
					LineChart.DrawMarkerCentered(writeableBitmapAdapter, dataPoint, rasterMarkerStyle);
					if (canvas.Children.Count > 0)
					{
						Image image = canvas.Children[0] as Image;
						image.Source = null;
						image.Source = writeableBitmapAdapter.GetImageSource();
					}
				}
			}
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
			if (!parent.LightWeight.Value || parent.RenderAs != RenderAs.Line)
			{
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
							LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
							return;
						case VcProperties.LabelText:
							LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
							return;
						case VcProperties.LabelAngle:
							LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
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
								LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
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
									LineChart.UpdateDataSeries(dataPoint.Parent, property, newValue);
									return;
								}
								if (parent.RenderAs == RenderAs.Spline)
								{
									LineChart.UpdateDataSeries(parent, VcProperties.DataPointUpdate, newValue);
									return;
								}
								LineChart.UpdateXAndYValueOfLineDataPoint(dataPoint, canvas);
								return;
							case VcProperties.XValueType:
								chart.InvokeRender();
								return;
							case VcProperties.YValue:
							case VcProperties.YValues:
								if (double.IsNaN(dataPoint.DpInfo.OldYValue) || double.IsNaN(dataPoint.DpInfo.InternalYValue) || dataPoint.DpInfo.Faces == null)
								{
									LineChart.UpdateDataSeries(dataPoint.Parent, property, newValue);
									return;
								}
								if (parent.RenderAs == RenderAs.Spline)
								{
									LineChart.UpdateDataSeries(dataPoint.Parent, VcProperties.DataPointUpdate, newValue);
									return;
								}
								chart.Dispatcher.BeginInvoke(new Action<IDataPoint, Canvas>(LineChart.UpdateXAndYValueOfLineDataPoint), new object[]
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
							LineChart.UpdateDataSeries(chart, dataPoint, VcProperties.Enabled, newValue);
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
							LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
							return;
						case VcProperties.LabelEnabled:
							LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
							return;
						case VcProperties.LabelFontColor:
							LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
							return;
						case VcProperties.LabelFontFamily:
							LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
							return;
						case VcProperties.LabelFontSize:
							LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
							return;
						case VcProperties.LabelFontStyle:
							LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
							return;
						case VcProperties.LabelFontWeight:
							LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, dataPoint.DpInfo.InternalYValue >= 0.0, markerLeft, markerTop, ref canvas, true);
							return;
						default:
							return;
						}
						break;
					}
				}
				return;
			}
			if (property == VcProperties.Color)
			{
				dataPoint.DpInfo.InternalColor = (newValue as Brush);
			}
			if (property == VcProperties.Color || property == VcProperties.MarkerColor || property == VcProperties.MarkerBorderColor)
			{
				LineChart.UpdateRasterMarker(dataPoint);
				return;
			}
			Canvas labelCanvas = parent.Faces.LabelCanvas;
			LineChart.UpdateLineSeries(parent, width, height, labelCanvas);
		}

		private static void UpdateDataSeries(Chart chart, IDataPoint dataPoint, VcProperties property, object newValue)
		{
			dataPoint.Parent.isSeriesRenderPending = true;
			chart.Dispatcher.BeginInvoke(new Action<IDataPoint, VcProperties, object>(LineChart.UpdateDataSeries), new object[]
			{
				dataPoint,
				property,
				newValue
			});
		}

		internal static void ApplyAnimationToBezierSegments(int index, Storyboard storyBoard, BezierSegment bezierSegment, Point[] oldCtrlPoints, Point[] newCtrlPoints, PathFigure pathFigure, Point oldFirstPointOfBezierSeg, Point newFirstPointOfBezierSeg)
		{
			if (index == 1)
			{
				PointAnimation pointAnimation = new PointAnimation
				{
					From = new Point?(oldFirstPointOfBezierSeg),
					To = new Point?(newFirstPointOfBezierSeg),
					SpeedRatio = 2.0,
					Duration = new Duration(new TimeSpan(0, 0, 1))
				};
				Storyboard.SetTarget(pointAnimation, pathFigure);
				Storyboard.SetTargetProperty(pointAnimation, new PropertyPath("StartPoint", new object[0]));
				Storyboard.SetTargetName(pointAnimation, (string)pathFigure.GetValue(FrameworkElement.NameProperty));
				storyBoard.Children.Add(pointAnimation);
				pathFigure.BeginAnimation(PathFigure.StartPointProperty, pointAnimation);
			}
			if (oldCtrlPoints != null && newCtrlPoints != null && oldCtrlPoints.Count<Point>() == 3 && newCtrlPoints.Count<Point>() == 3)
			{
				for (int i = 0; i < 3; i++)
				{
					if (!oldCtrlPoints[i].Equals(newCtrlPoints[i]))
					{
						PointAnimation pointAnimation2 = new PointAnimation
						{
							From = new Point?(oldCtrlPoints[i]),
							To = new Point?(newCtrlPoints[i]),
							SpeedRatio = 2.0,
							Duration = new Duration(new TimeSpan(0, 0, 1))
						};
						Storyboard.SetTarget(pointAnimation2, bezierSegment);
						Storyboard.SetTargetProperty(pointAnimation2, new PropertyPath("Point" + (i + 1).ToString(), new object[0]));
						Storyboard.SetTargetName(pointAnimation2, (string)bezierSegment.GetValue(FrameworkElement.NameProperty));
						storyBoard.Children.Add(pointAnimation2);
						switch (i)
						{
						case 0:
							bezierSegment.BeginAnimation(BezierSegment.Point1Property, pointAnimation2);
							break;
						case 1:
							bezierSegment.BeginAnimation(BezierSegment.Point2Property, pointAnimation2);
							break;
						case 2:
							bezierSegment.BeginAnimation(BezierSegment.Point3Property, pointAnimation2);
							break;
						}
					}
				}
			}
		}

		private static void UpdateXAndYValueOfLineDataPoint(IDataPoint dataPoint, Canvas line2dLabelCanvas)
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
					point2 = LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, internalYValue >= 0.0, num, num2, ref line2dLabelCanvas, false);
					dataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, point3.Y);
					dataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, point3.X);
				}
				else
				{
					LineChart.CreateLabel4LineDataPoint(dataPoint, width, height, internalYValue >= 0.0, num, num2, ref line2dLabelCanvas, true);
				}
			}
			if (!value && marker != null && marker.Visual != null)
			{
				marker.Visual.SetValue(Canvas.TopProperty, point.Y);
				marker.Visual.SetValue(Canvas.LeftProperty, point.X);
			}
			DependencyObject dependencyObject = null;
			Point value2 = default(Point);
			LineSegment lineSegment = dataPoint.DpInfo.Faces.Parts[0] as LineSegment;
			PathFigure pathFigure = dataPoint.DpInfo.Faces.Parts[1] as PathFigure;
			if (!VisifireControl.IsMediaEffectsEnabled && dataPoint.Parent.ShadowEnabled.Value)
			{
				LineSegment lineSegment2 = dataPoint.DpInfo.ShadowFaces.Parts[0] as LineSegment;
				PathFigure pathFigure2 = dataPoint.DpInfo.ShadowFaces.Parts[1] as PathFigure;
				if (lineSegment2 == null)
				{
					dependencyObject = pathFigure2;
					if (!value)
					{
						pathFigure2.StartPoint = new Point(num, num2);
					}
				}
				else
				{
					dependencyObject = lineSegment2;
					if (!value)
					{
						lineSegment2.Point = new Point(num, num2);
					}
				}
			}
			DependencyObject dependencyObject2;
			if (lineSegment == null)
			{
				dependencyObject2 = pathFigure;
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
				}
			}
			else
			{
				dependencyObject2 = lineSegment;
				if (value)
				{
					if (dataPoint.DpInfo.Storyboard != null)
					{
						dataPoint.DpInfo.Storyboard.Pause();
					}
					value2 = lineSegment.Point;
				}
				else
				{
					lineSegment.Point = new Point(num, num2);
				}
			}
			if (value)
			{
				Storyboard storyboard = new Storyboard();
				PointAnimation pointAnimation = new PointAnimation();
				pointAnimation.From = new Point?(value2);
				pointAnimation.To = new Point?(new Point(num, num2));
				pointAnimation.SpeedRatio = 2.0;
				pointAnimation.Duration = new Duration(new TimeSpan(0, 0, 1));
				Storyboard.SetTarget(pointAnimation, dependencyObject2);
				Storyboard.SetTargetProperty(pointAnimation, (lineSegment != null) ? new PropertyPath("Point", new object[0]) : new PropertyPath("StartPoint", new object[0]));
				Storyboard.SetTargetName(pointAnimation, (string)dependencyObject2.GetValue(FrameworkElement.NameProperty));
				storyboard.Children.Add(pointAnimation);
				if (!VisifireControl.IsMediaEffectsEnabled && dependencyObject != null)
				{
					pointAnimation = new PointAnimation();
					pointAnimation.From = new Point?(value2);
					pointAnimation.To = new Point?(new Point(num, num2));
					pointAnimation.SpeedRatio = 2.0;
					pointAnimation.Duration = new Duration(new TimeSpan(0, 0, 1));
					dependencyObject.SetValue(FrameworkElement.NameProperty, "ShadowSegment_" + dataPoint.Name);
					Storyboard.SetTarget(pointAnimation, dependencyObject);
					Storyboard.SetTargetProperty(pointAnimation, (lineSegment != null) ? new PropertyPath("Point", new object[0]) : new PropertyPath("StartPoint", new object[0]));
					Storyboard.SetTargetName(pointAnimation, (string)dependencyObject.GetValue(FrameworkElement.NameProperty));
					storyboard.Children.Add(pointAnimation);
					if (lineSegment != null)
					{
						(dependencyObject as LineSegment).BeginAnimation(LineSegment.PointProperty, pointAnimation);
					}
					else
					{
						(dependencyObject as PathFigure).BeginAnimation(PathFigure.StartPointProperty, pointAnimation);
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
				if (lineSegment != null)
				{
					(dependencyObject2 as LineSegment).BeginAnimation(LineSegment.PointProperty, pointAnimation);
				}
				else
				{
					(dependencyObject2 as PathFigure).BeginAnimation(PathFigure.StartPointProperty, pointAnimation);
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
				bool flag = true;
				if (dataPoint.Parent.PlotGroup.AxisY.AxisMinimum != null && parent.RenderAs != RenderAs.QuickLine)
				{
					double limitingYValue = dataPoint.Parent.PlotGroup.GetLimitingYValue();
					double internalYValue2 = dataPoint.DpInfo.InternalYValue;
					if ((internalYValue2 < limitingYValue && limitingYValue > 0.0) || (internalYValue2 < limitingYValue && dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue2 < dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum)
					{
						if (dataPoint.DpInfo.Marker != null && dataPoint.DpInfo.Marker.Visual != null)
						{
							dataPoint.DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
						}
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && dataPoint.DpInfo.LabelVisual != null)
						{
							dataPoint.DpInfo.LabelVisual.Visibility = Visibility.Collapsed;
						}
					}
					else if (dataPoint.DpInfo.LabelVisual != null)
					{
						(((dataPoint.DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Visible;
					}
				}
				if (dataPoint.Parent.PlotGroup.AxisY.AxisMaximum != null && parent.RenderAs != RenderAs.QuickLine)
				{
					double internalYValue3 = dataPoint.DpInfo.InternalYValue;
					if (internalYValue3 > dataPoint.Parent.PlotGroup.AxisY.InternalAxisMaximum)
					{
						if (dataPoint.DpInfo.Marker != null && dataPoint.DpInfo.Marker.Visual != null)
						{
							dataPoint.DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
						}
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled) && dataPoint.DpInfo.LabelVisual != null)
						{
							dataPoint.DpInfo.LabelVisual.Visibility = Visibility.Collapsed;
						}
					}
				}
				if (dataPoint.Parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					double limitingYValue2 = dataPoint.Parent.PlotGroup.GetLimitingYValue();
					if (dataPoint.DpInfo.InternalYValue < limitingYValue2)
					{
						flag = false;
					}
				}
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				double num3 = chart.ChartArea.PLANK_DEPTH / (double)((chart.PlotDetails.Layer3DCount == 0) ? 1 : chart.PlotDetails.Layer3DCount) * (double)(chart.View3D ? 1 : 0);
				double x = 0.0;
				double y = -num3 - 4.0;
				double width2 = width + num3;
				double height2 = height + num3 + chart.ChartArea.PLANK_THICKNESS + 10.0;
				AreaChart.GetClipCoordinates(chart, ref x, ref y, ref width2, ref height2, parent.PlotGroup.MinimumX, parent.PlotGroup.MaximumX);
				rectangleGeometry.Rect = new Rect(x, y, width2, height2);
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

		internal static void ApplyMarkerProperties(IDataPoint dataPoint)
		{
			Marker marker = dataPoint.DpInfo.Marker;
			if (dataPoint.DpInfo.Marker.IsEnabled)
			{
				marker.BorderColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor);
			}
			else
			{
				marker.BorderColor = new SolidColorBrush(Colors.Transparent);
			}
			if (!DataPointHelper.GetMarkerBorderThickness_DataPoint(dataPoint).HasValue && !((Thickness?)dataPoint.Parent.GetValue(DataSeries.MarkerBorderThicknessProperty)).HasValue)
			{
				marker.BorderThickness = marker.MarkerSize.Width / 6.0;
			}
			else
			{
				marker.BorderThickness = ((Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderThickness)).Left;
			}
			if (!VisifireControl.IsMediaEffectsEnabled)
			{
				marker.ShadowEnabled = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled);
				return;
			}
			if (DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect) == null)
			{
				marker.ShadowEnabled = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.ShadowEnabled);
				marker.PixelLevelShadow = true;
				return;
			}
			marker.Effect = (Effect)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect);
		}

		internal static void ApplyLabelProperties(IDataPoint dataPoint)
		{
			Brush labelFontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
			FontStyle fontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
			Brush textBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
			FontFamily fontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
			double fontSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontSize);
			FontWeight fontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
			dataPoint.DpInfo.Marker.FontColor = Chart.CalculateDataPointLabelFontColor(dataPoint.Chart as Chart, dataPoint, labelFontColor, LabelStyles.OutSide);
			dataPoint.DpInfo.Marker.FontFamily = fontFamily;
			dataPoint.DpInfo.Marker.FontSize = fontSize;
			dataPoint.DpInfo.Marker.FontStyle = fontStyle;
			dataPoint.DpInfo.Marker.FontWeight = fontWeight;
			dataPoint.DpInfo.Marker.TextBackground = textBackground;
		}

		internal static Storyboard ApplyLineChartAnimation(DataSeries currentDataSeries, Panel canvas, Storyboard storyboard, bool isLineCanvas)
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush
			{
				StartPoint = new Point(0.0, 0.5),
				EndPoint = new Point(1.0, 0.5)
			};
			GradientStop GradStop1 = new GradientStop
			{
				Color = Colors.White,
				Offset = 0.0
			};
			GradientStop GradStop2 = new GradientStop
			{
				Color = Colors.White,
				Offset = 0.0
			};
			GradientStop GradStop3 = new GradientStop
			{
				Color = Colors.Transparent,
				Offset = 0.01
			};
			GradientStop GradStop4 = new GradientStop
			{
				Color = Colors.Transparent,
				Offset = 1.0
			};
			linearGradientBrush.GradientStops.Add(GradStop1);
			linearGradientBrush.GradientStops.Add(GradStop2);
			linearGradientBrush.GradientStops.Add(GradStop3);
			linearGradientBrush.GradientStops.Add(GradStop4);
			canvas.OpacityMask = linearGradientBrush;
			double beginTime = isLineCanvas ? 0.75 : 0.5;
			DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				1.0
			});
			DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				1.0
			});
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0)
			});
			storyboard.Children.Add(AnimationHelper.CreateDoubleAnimation(currentDataSeries, GradStop2, "(GradientStop.Offset)", beginTime, frameTime, values, splines));
			values = Graphics.GenerateDoubleCollection(new double[]
			{
				0.01,
				1.0
			});
			frameTime = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				1.0
			});
			splines = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0)
			});
			storyboard.Children.Add(AnimationHelper.CreateDoubleAnimation(currentDataSeries, GradStop3, "(GradientStop.Offset)", beginTime, frameTime, values, splines));
			storyboard.Completed += delegate(object A_1, EventArgs A_2)
			{
				GradStop2.Offset = 1.0;
				GradStop3.Offset = 1.0;
				GradStop1.Color = Colors.White;
				GradStop2.Color = Colors.White;
				GradStop3.Color = Colors.White;
				GradStop4.Color = Colors.White;
			};
			return storyboard;
		}

		internal static void CalculateMarkerPosition(IDataPoint dataPoint, double width, double height, ref double xPosition, ref double yPosition)
		{
			double internalYValue = dataPoint.DpInfo.InternalYValue;
			PlotGroup plotGroup = dataPoint.Parent.PlotGroup;
			VisifireControl arg_1E_0 = dataPoint.Chart;
			xPosition = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
			yPosition = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, internalYValue);
		}

		internal static Marker CreateMarkerAForLineDataPoint(IDataPoint dataPoint, double width, double height, ref Canvas line2dLabelCanvas, out double xPosition, out double yPosition)
		{
			xPosition = double.NaN;
			yPosition = double.NaN;
			double internalYValue = dataPoint.DpInfo.InternalYValue;
			if (double.IsNaN(internalYValue) || dataPoint.Parent == null)
			{
				return null;
			}
			PlotGroup plotGroup = dataPoint.Parent.PlotGroup;
			Chart chart = dataPoint.Chart as Chart;
			xPosition = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
			yPosition = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, internalYValue);
			dataPoint.DpInfo.VisualPosition = new Point(xPosition, yPosition);
			Marker markerForDataPoint = LineChart.GetMarkerForDataPoint(true, chart, width, height, yPosition, dataPoint, internalYValue > 0.0);
			if (line2dLabelCanvas != null)
			{
				markerForDataPoint.AddToParent(line2dLabelCanvas, xPosition, yPosition, new Point(0.5, 0.5));
			}
			return markerForDataPoint;
		}

		internal static void CreateAlineSeries(DataSeries series, double width, double height, Canvas labelsCanvas, Canvas chartsCanvas, bool animationEnabled)
		{
			Canvas canvas;
			Canvas canvas2;
			if (series.Faces != null)
			{
				canvas = (series.Faces.Visual as Canvas);
				canvas2 = series.Faces.LabelCanvas;
				if (canvas != null && canvas.Parent != chartsCanvas)
				{
					Panel panel = canvas.Parent as Panel;
					if (panel != null)
					{
						panel.Children.Remove(canvas);
					}
				}
				if (canvas2 != null && canvas2.Parent != labelsCanvas)
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
			Chart chart = series.Chart as Chart;
			canvas2 = new Canvas
			{
				Width = width,
				Height = height
			};
			List<List<IDataPoint>> list = new List<List<IDataPoint>>();
			List<List<IDataPoint>> list2 = new List<List<IDataPoint>>();
			PlotGroup arg_D7_0 = series.PlotGroup;
			LineChartShapeParams lineChartShapeParams = new LineChartShapeParams();
			lineChartShapeParams.Points = new List<IDataPoint>();
			lineChartShapeParams.ShadowPoints = new List<IDataPoint>();
			lineChartShapeParams.LineGeometryGroup = new GeometryGroup();
			lineChartShapeParams.LineThickness = series.LineThickness.Value;
			lineChartShapeParams.LineColor = series.Color;
			lineChartShapeParams.LineFill = series.LineFill;
			lineChartShapeParams.LineStyle = ExtendedGraphics.GetDashArray(series.LineStyle);
			lineChartShapeParams.Lighting = series.LightingEnabled.Value;
			lineChartShapeParams.ShadowEnabled = (series.ShadowEnabled.Value && series.RenderAs != RenderAs.QuickLine);
			lineChartShapeParams.Opacity = series.Opacity;
			if (lineChartShapeParams.ShadowEnabled)
			{
				lineChartShapeParams.LineShadowGeometryGroup = new GeometryGroup();
			}
			series.VisualParams = lineChartShapeParams;
			Point point = default(Point);
			bool flag = true;
			List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(series, false, RenderHelper.IsRasterRenderSupported(series));
			bool indicatorEnabled = chart.IndicatorEnabled;
			bool value = series.LightWeight.Value;
			foreach (IDataPoint current in dataPointsUnderViewPort)
			{
				if (current.Parent != null && (series.IsAllDataPointsEnabled || !(current.Enabled == false)))
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
							if (lineChartShapeParams.Points.Count > 0)
							{
								list.Add(lineChartShapeParams.Points);
								list2.Add(lineChartShapeParams.ShadowPoints);
							}
							lineChartShapeParams.Points = new List<IDataPoint>();
							lineChartShapeParams.ShadowPoints = new List<IDataPoint>();
						}
						else
						{
							point = new Point(x, y);
							flag = false;
						}
						current.DpInfo.VisualPosition = new Point(x, y);
						lineChartShapeParams.Points.Add(current);
						if (lineChartShapeParams.ShadowEnabled)
						{
							lineChartShapeParams.ShadowPoints.Add(current);
						}
					}
					if (current.Parent != null && indicatorEnabled && value)
					{
						current.DpInfo.ParsedToolTipText = current.TextParser(current.ToolTipText);
					}
				}
			}
			list.Add(lineChartShapeParams.Points);
			list2.Add(lineChartShapeParams.ShadowPoints);
			series.Faces = new Faces();
			if (RenderHelper.IsRasterRenderSupported(series))
			{
				canvas = LineChart.GetOrUpdateRasterLine2DWithMarker(series, width, height, lineChartShapeParams, list);
				if (canvas == null)
				{
					return;
				}
			}
			else
			{
				Path item;
				Path item2;
				canvas = LineChart.GetLine2D(series, width, height, canvas2, lineChartShapeParams, out item, out item2, list, list2);
				series.Faces.Parts.Add(item);
				if (!VisifireControl.IsMediaEffectsEnabled)
				{
					series.Faces.Parts.Add(item2);
				}
			}
			canvas.Width = width;
			canvas.Height = height;
			if (canvas2.Parent == null)
			{
				labelsCanvas.Children.Add(canvas2);
			}
			if (canvas.Parent == null)
			{
				chartsCanvas.Children.Add(canvas);
			}
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
				Fill = lineChartShapeParams.LineColor
			};
			labelsCanvas.Children.Add(ellipse);
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
				LineChart.CreateAlineSeries(current, width, height, canvas2, canvas3, animationEnabled);
				flag = (flag || current.MovingMarkerEnabled);
				val = Math.Min(val, current.PlotGroup.MinimumX);
				val2 = Math.Max(val2, current.PlotGroup.MaximumX);
			}
			List<IDataPoint> dataPointsInCurrentPlotGroup = seriesList[0].PlotGroup._dataPointsInCurrentPlotGroup;
			for (int i = 0; i < dataPointsInCurrentPlotGroup.Count; i++)
			{
				if (dataPointsInCurrentPlotGroup[i].Parent != null && dataPointsInCurrentPlotGroup[i].Parent.PlotGroup.AxisY.GetValue(Axis.AxisMinimumProperty) != null)
				{
					double limitingYValue = dataPointsInCurrentPlotGroup[i].Parent.PlotGroup.GetLimitingYValue();
					if (dataPointsInCurrentPlotGroup[i].DpInfo.InternalYValue < limitingYValue)
					{
						isOffsetAdded = false;
						break;
					}
				}
			}
			chart.ChartArea.PlotAreaCanvas.MouseMove -= new MouseEventHandler(LineChart.PlotAreaCanvas_MouseMove);
			chart.ChartArea.PlotAreaCanvas.MouseLeave -= new MouseEventHandler(LineChart.PlotAreaCanvas_MouseLeave);
			chart.ChartArea.PlotAreaCanvas.MouseEnter -= new MouseEventHandler(LineChart.PlotAreaCanvas_MouseEnter);
			if (flag)
			{
				chart.ChartArea.PlotAreaCanvas.Tag = chart.PlotArea;
				chart.ChartArea.PlotAreaCanvas.MouseMove += new MouseEventHandler(LineChart.PlotAreaCanvas_MouseMove);
				chart.ChartArea.PlotAreaCanvas.MouseLeave += new MouseEventHandler(LineChart.PlotAreaCanvas_MouseLeave);
				chart.ChartArea.PlotAreaCanvas.MouseEnter += new MouseEventHandler(LineChart.PlotAreaCanvas_MouseEnter);
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

		internal static void Clip(Chart chart, Canvas chartsCanvas, Canvas labelCanvas, PlotGroup plotGroup, bool isOffsetAdded)
		{
			double num = chart.ChartArea.PLANK_DEPTH / (double)((chart.PlotDetails.Layer3DCount == 0) ? 1 : chart.PlotDetails.Layer3DCount) * (double)(chart.View3D ? 1 : 0);
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			double x = 0.0;
			double y = -num - 4.0;
			double width = labelCanvas.Width + num;
			double height = labelCanvas.Height + num + chart.ChartArea.PLANK_THICKNESS + 10.0;
			AreaChart.GetClipCoordinates(chart, ref x, ref y, ref width, ref height, plotGroup.MinimumX, plotGroup.MaximumX);
			rectangleGeometry.Rect = new Rect(x, y, width, height);
			labelCanvas.Clip = rectangleGeometry;
			rectangleGeometry = new RectangleGeometry();
			double num2 = 4.0;
			if (isOffsetAdded)
			{
				double num3 = num2 + 2.0;
				if (plotGroup.AxisY.AxisMaximum != null)
				{
					rectangleGeometry.Rect = new Rect(0.0, 0.0, labelCanvas.Width + num, labelCanvas.Height + chart.ChartArea.PLANK_DEPTH + num3);
				}
				else
				{
					rectangleGeometry.Rect = new Rect(0.0, -num - num2, labelCanvas.Width + num, labelCanvas.Height + chart.ChartArea.PLANK_DEPTH + num3);
				}
			}
			else if (plotGroup.AxisY.AxisMinimum != null)
			{
				rectangleGeometry.Rect = new Rect(0.0, 0.0, labelCanvas.Width + num, labelCanvas.Height);
			}
			else
			{
				rectangleGeometry.Rect = new Rect(0.0, 0.0, labelCanvas.Width, labelCanvas.Height - num2 + chart.ChartArea.PLANK_DEPTH + num2);
			}
			chartsCanvas.Clip = rectangleGeometry;
		}

		private static void PlotAreaCanvas_MouseEnter(object sender, MouseEventArgs e)
		{
			LineChart._isMouseEnteredInPlotArea = true;
		}

		private static void PlotAreaCanvas_MouseLeave(object sender, MouseEventArgs e)
		{
			LineChart._isMouseEnteredInPlotArea = false;
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
			((FrameworkElement)sender).Dispatcher.BeginInvoke(new Action<Chart, object, MouseEventArgs, RenderAs[]>(LineChart.MoveMovingMarker), new object[]
			{
				chart,
				sender,
				e,
				new RenderAs[]
				{
					RenderAs.Line,
					RenderAs.Spline
				}
			});
		}

		internal static void MoveMovingMarker(Chart chart, object sender, MouseEventArgs e, params RenderAs[] chartTypes)
		{
			if (!LineChart._isMouseEnteredInPlotArea)
			{
				return;
			}
			foreach (DataSeries current in chart.InternalSeries)
			{
				if (Array.IndexOf<RenderAs>(chartTypes, current.RenderAs) != -1)
				{
					double internalXValue = RenderHelper.CalculateInternalXValueFromPixelPos(chart, current.PlotGroup.AxisX, e);
					double internalYValue = RenderHelper.CalculateInternalYValueFromPixelPos(chart, current.PlotGroup.AxisY, e);
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
				if (LineChart._isMouseEnteredInPlotArea)
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

		internal static void SelectMovingMarker(IDataPoint dataPoint)
		{
			Ellipse movingMarker = dataPoint.Parent._movingMarker;
			if (movingMarker != null)
			{
				movingMarker.Stroke = dataPoint.DpInfo.Marker.MarkerShape.Stroke;
				movingMarker.StrokeThickness = 2.0;
				movingMarker.Width = dataPoint.DpInfo.Marker.MarkerShape.Width + 2.0;
				movingMarker.Height = movingMarker.Width;
				movingMarker.SetValue(Canvas.LeftProperty, dataPoint.DpInfo.VisualPosition.X - movingMarker.Width / 2.0);
				movingMarker.SetValue(Canvas.TopProperty, dataPoint.DpInfo.VisualPosition.Y - movingMarker.Height / 2.0);
			}
		}
	}
}
