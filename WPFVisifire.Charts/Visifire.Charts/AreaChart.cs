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
	internal class AreaChart
	{
		private static Canvas GetStacked3DSideFaces(ref Faces faces, PolygonalChartShapeParams areaParams)
		{
			Brush fill = areaParams.Lighting ? Graphics.GetRightFaceBrush(areaParams.Background) : areaParams.Background;
			Brush fill2 = areaParams.Lighting ? Graphics.GetTopFaceBrush(areaParams.Background) : areaParams.Background;
			int num = areaParams.IsPositive ? (areaParams.Points.Count - 1) : areaParams.Points.Count;
			Canvas canvas = new Canvas();
			Rect bounds = AreaChart.GetBounds(areaParams.Points);
			canvas.Width = bounds.Width + areaParams.Depth3D;
			canvas.Height = bounds.Height + areaParams.Depth3D;
			canvas.SetValue(Canvas.TopProperty, bounds.Top - areaParams.Depth3D);
			canvas.SetValue(Canvas.LeftProperty, bounds.Left);
			for (int i = 0; i < num; i++)
			{
				Polygon polygon = new Polygon
				{
					Tag = new ElementData
					{
						Element = areaParams.TagReference
					}
				};
				PointCollection pointCollection = new PointCollection();
				int index = i % areaParams.Points.Count;
				int index2 = (i + 1) % areaParams.Points.Count;
				pointCollection.Add(areaParams.Points[index]);
				pointCollection.Add(areaParams.Points[index2]);
				pointCollection.Add(new Point(areaParams.Points[index2].X + areaParams.Depth3D, areaParams.Points[index2].Y - areaParams.Depth3D));
				pointCollection.Add(new Point(areaParams.Points[index].X + areaParams.Depth3D, areaParams.Points[index].Y - areaParams.Depth3D));
				polygon.Points = pointCollection;
				Point centroid = AreaChart.GetCentroid(pointCollection);
				int areaZIndex = AreaChart.GetAreaZIndex(centroid.X, centroid.Y, areaParams.IsPositive);
				polygon.SetValue(Panel.ZIndexProperty, areaZIndex);
				if (i == areaParams.Points.Count - 2)
				{
					polygon.Fill = fill;
					(polygon.Tag as ElementData).VisualElementName = "Side";
				}
				else
				{
					polygon.Fill = fill2;
					(polygon.Tag as ElementData).VisualElementName = "Top";
				}
				polygon.Stroke = areaParams.BorderColor;
				polygon.StrokeDashArray = ((areaParams.BorderStyle != null) ? ExtendedGraphics.CloneCollection(areaParams.BorderStyle) : areaParams.BorderStyle);
				polygon.StrokeThickness = areaParams.BorderThickness;
				polygon.StrokeMiterLimit = 1.0;
				Rect bounds2 = AreaChart.GetBounds(pointCollection);
				polygon.Stretch = Stretch.Fill;
				polygon.Width = bounds2.Width;
				polygon.Height = bounds2.Height;
				polygon.SetValue(Canvas.TopProperty, bounds2.Y - (bounds.Top - areaParams.Depth3D));
				polygon.SetValue(Canvas.LeftProperty, bounds2.X - bounds.X);
				faces.Parts.Add(polygon);
				canvas.Children.Add(polygon);
			}
			return canvas;
		}

		private static int GetAreaZIndex(double left, double top, bool isPositive)
		{
			int num = 0;
			int num2 = (int)left;
			if (num2 == 0)
			{
				num2++;
			}
			return isPositive ? (num + num2) : (num + -2147483648 + num2);
		}

		private static int GetStackedAreaZIndex(double left, double top, bool isPositive, int index)
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
				result = -2147483648 + (num + index);
			}
			return result;
		}

		private static Storyboard ApplyAreaAnimation(DataSeries currentDataSeries, UIElement areaElement, Storyboard storyboard, bool isPositive, double beginTime)
		{
			ScaleTransform scaleTransform = new ScaleTransform
			{
				ScaleY = 0.0
			};
			areaElement.RenderTransform = scaleTransform;
			if (isPositive)
			{
				areaElement.RenderTransformOrigin = new Point(0.5, 1.0);
			}
			else
			{
				areaElement.RenderTransformOrigin = new Point(0.5, 0.0);
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
			DoubleAnimationUsingKeyFrames value = AnimationHelper.CreateDoubleAnimation(currentDataSeries, scaleTransform, "(ScaleTransform.ScaleY)", beginTime + 0.5, frameTime, values, splines);
			storyboard.Children.Add(value);
			return storyboard;
		}

		private static Storyboard ApplyStackedAreaAnimation(DataSeries currentDataSeries, FrameworkElement areaElement, Storyboard storyboard, double beginTime, double duration)
		{
			return AnimationHelper.ApplyOpacityAnimation(areaElement, currentDataSeries, storyboard, beginTime, duration, 0.0, 1.0);
		}

		private static Storyboard CreateOpacityAnimation(DataSeries currentDataSeries, Storyboard storyboard, DependencyObject target, double beginTime, double opacity, double duration)
		{
			DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				opacity
			});
			DoubleCollection doubleCollection = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				duration
			});
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(doubleCollection.Count);
			DoubleAnimationUsingKeyFrames value = AnimationHelper.CreateDoubleAnimation(currentDataSeries, target, "(UIElement.Opacity)", beginTime + 0.5, doubleCollection, values, splines);
			storyboard.Children.Add(value);
			return storyboard;
		}

		private static PolygonalChartShapeParams GetAreaParms(DataSeries series, Brush colorBrush, double depth3d)
		{
			return new PolygonalChartShapeParams
			{
				Background = colorBrush,
				Lighting = series.LightingEnabled.Value,
				Shadow = series.ShadowEnabled.Value,
				Bevel = series.Bevel.Value,
				BorderColor = series.BorderColor,
				BorderStyle = ExtendedGraphics.GetDashArray(series.BorderStyle),
				BorderThickness = series.BorderThickness.Left,
				Depth3D = depth3d,
				TagReference = series
			};
		}

		internal static Marker CreateNewMarker(Chart chart, IDataPoint dataPoint, Size markerSize, string labelText)
		{
			bool markerBevel = false;
			Brush labelFontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
			FontStyle fontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
			Brush textBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
			FontFamily fontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
			double fontSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontSize);
			FontWeight fontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
			Brush borderColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor);
			Thickness thickness = (Thickness)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderThickness);
			double scaleFactor = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerScale);
			MarkerTypes markerType = (MarkerTypes)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerType);
			Brush brush = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerColor);
			Marker marker = new Marker();
			marker.AssignPropertiesValue(markerType, scaleFactor, markerSize, markerBevel, brush, labelText);
			marker.MarkerSize = markerSize;
			marker.BorderColor = borderColor;
			marker.BorderThickness = thickness.Left;
			marker.FontColor = Chart.CalculateDataPointLabelFontColor(chart, dataPoint, labelFontColor, LabelStyles.OutSide);
			marker.FontFamily = fontFamily;
			marker.FontSize = fontSize;
			marker.FontStyle = fontStyle;
			marker.FontWeight = fontWeight;
			marker.TextBackground = textBackground;
			marker.MarkerFillColor = brush;
			marker.Tag = new ElementData
			{
				Element = dataPoint
			};
			return marker;
		}

		internal static Marker GetMarkerForDataPoint(Chart chart, double height, bool isTopOfStack, IDataPoint dataPoint, double position, bool isPositive)
		{
			LabelStyles labelStyles = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			double num = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelAngle);
			double num2 = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerSize);
			Brush arg_37_0 = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerColor);
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled);
			bool flag2 = (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled);
			if (flag || flag2)
			{
				Size markerSize = flag ? new Size(num2, num2) : new Size(0.0, 0.0);
				string text = flag2 ? dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText)) : "";
				dataPoint.DpInfo.Marker = AreaChart.CreateNewMarker(chart, dataPoint, markerSize, text);
				if (!string.IsNullOrEmpty(text))
				{
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
								if (!isTopOfStack)
								{
									if (position + dataPoint.DpInfo.Marker.TextBlockSize.Height > height)
									{
										dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Top;
									}
									else
									{
										dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Bottom;
									}
								}
								else if (position - dataPoint.DpInfo.Marker.MarkerActualSize.Height - dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 < 0.0 || labelStyles == LabelStyles.Inside)
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
							if (!isTopOfStack)
							{
								dataPoint.DpInfo.Marker.TextAlignmentY = AlignmentY.Top;
							}
							else if (position + dataPoint.DpInfo.Marker.MarkerActualSize.Height + dataPoint.DpInfo.Marker.MarkerSize.Height / 2.0 > chart.PlotArea.BorderElement.Height || labelStyles == LabelStyles.Inside)
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
				dataPoint.DpInfo.Marker.CreateVisual((chart != null) ? chart.FlowDirection : FlowDirection.LeftToRight);
				return dataPoint.DpInfo.Marker;
			}
			return null;
		}

		internal static double GetInternalYValue4StackedArea(IDataPoint dataPoint)
		{
			DataSeries parent = dataPoint.Parent;
			if ((double.IsNaN(dataPoint.YValue) || dataPoint.Enabled == false) && parent != null && (parent.RenderAs == RenderAs.StackedArea100 || parent.RenderAs == RenderAs.StackedArea))
			{
				return 0.0;
			}
			return dataPoint.YValue;
		}

		public static List<List<IDataPoint>> BrokenAreaDataPointsGroup(double width, double height, DataSeries dataSeries)
		{
			VisifireControl arg_06_0 = dataSeries.Chart;
			PlotGroup plotGroup = dataSeries.PlotGroup;
			List<List<IDataPoint>> list = new List<List<IDataPoint>>();
			List<IDataPoint> list2 = new List<IDataPoint>();
			bool flag = true;
			plotGroup.GetLimitingYValue();
			Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, 0.0);
			List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(dataSeries, false, false);
			List<IDataPoint> list3 = (from dp in dataPointsUnderViewPort
			orderby dp.DpInfo.InternalXValue
			select dp).ToList<IDataPoint>();
			foreach (IDataPoint current in list3)
			{
				current.DpInfo.Faces = null;
				if (!(current.Enabled == false))
				{
					double internalYValue = current.DpInfo.InternalYValue;
					if (double.IsNaN(internalYValue))
					{
						flag = true;
					}
					else
					{
						double x = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, current.DpInfo.InternalXValue);
						double y = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, internalYValue);
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

		private static void CreateBevelFor2DArea(Canvas areaCanvas, IDataPoint currentDataPoint, IDataPoint previusDataPoint, bool clipAtStart, bool clipAtEnd)
		{
			Line line = new Line
			{
				StrokeThickness = 2.4,
				Stroke = Graphics.GetBevelTopBrush(currentDataPoint.Parent.Color),
				StrokeEndLineCap = PenLineCap.Triangle,
				StrokeStartLineCap = PenLineCap.Triangle,
				IsHitTestVisible = false,
				Opacity = currentDataPoint.Parent.Opacity
			};
			line.SetValue(Panel.ZIndexProperty, 2);
			line.X1 = previusDataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point.X;
			line.Y1 = previusDataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point.Y;
			line.X2 = currentDataPoint.DpInfo.VisualPosition.X;
			line.Y2 = currentDataPoint.DpInfo.VisualPosition.Y;
			currentDataPoint.DpInfo.Faces.BevelLine = line;
			previusDataPoint.DpInfo.Faces.BevelLine = line;
			areaCanvas.Children.Add(line);
		}

		internal static Canvas GetVisualObjectForAreaChart(Panel preExistingPanel, double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, double plankDepth, bool animationEnabled)
		{
			if (double.IsNaN(width) || double.IsNaN(height) || width <= 0.0 || height <= 0.0)
			{
				return null;
			}
			DataSeries dataSeries = seriesList[0];
			if (animationEnabled && dataSeries.Storyboard == null)
			{
				dataSeries.Storyboard = new Storyboard();
			}
			Canvas canvas;
			Canvas canvas2;
			Canvas canvas3;
			RenderHelper.PrepareCanvas4Drawing(preExistingPanel as Canvas, out canvas, out canvas2, out canvas3, width, height);
			Canvas canvas4 = new Canvas
			{
				Height = height,
				Width = width
			};
			double num = plankDepth / (double)plotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
			double num2 = num * (double)(plotDetails.SeriesDrawingIndex[dataSeries] + 1);
			canvas.SetValue(Canvas.TopProperty, num2);
			canvas.SetValue(Canvas.LeftProperty, -num2);
			canvas2.SetValue(Canvas.TopProperty, 0.0);
			canvas2.SetValue(Canvas.LeftProperty, 0.0);
			canvas4.SetValue(Canvas.TopProperty, 0.0);
			canvas4.SetValue(Canvas.LeftProperty, 0.0);
			double num3 = 1.7976931348623157E+308;
			double num4 = -1.7976931348623157E+308;
			bool view3D = chart.View3D;
			if (dataSeries.Enabled.Value)
			{
				if (dataSeries.Storyboard == null)
				{
					dataSeries.Storyboard = new Storyboard();
				}
				PlotGroup plotGroup = dataSeries.PlotGroup;
				double limitingYValue = plotGroup.GetLimitingYValue();
				num3 = Math.Min(num3, plotGroup.MinimumX);
				num4 = Math.Max(num4, plotGroup.MaximumX);
				Faces faces = new Faces();
				faces.FrontFacePaths = new List<Path>();
				faces.Visual = canvas4;
				faces.LabelCanvas = canvas2;
				dataSeries.Faces = faces;
				List<List<IDataPoint>> list = AreaChart.BrokenAreaDataPointsGroup(width, height, dataSeries);
				double num5 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, limitingYValue);
				foreach (List<IDataPoint> current in list)
				{
					if (current.Count > 0)
					{
						IDataPoint dataPoint = current[0];
						if (dataPoint.Parent != null)
						{
							IDataPoint dataPoint2 = dataPoint;
							new PointCollection();
							new List<IDataPoint>();
							Path path = null;
							PathFigure pathFigure = null;
							int num6 = 0;
							for (int i = 0; i < current.Count - 1; i++)
							{
								Path path2 = new Path();
								Faces faces2 = new Faces();
								dataPoint = current[i];
								dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
								if (!dataPoint.IsLightDataPoint)
								{
									(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
								}
								IDataPoint dataPoint3 = current[i + 1];
								Faces faces3;
								if (dataPoint.DpInfo.Faces == null)
								{
									faces3 = new Faces();
									dataPoint.DpInfo.Faces = faces3;
								}
								else
								{
									faces3 = dataPoint.DpInfo.Faces;
								}
								dataPoint3.DpInfo.Faces = faces2;
								faces3.PreviousDataPoint = dataPoint2;
								faces3.NextDataPoint = dataPoint3;
								double internalYValue = dataPoint.DpInfo.InternalYValue;
								if (i == 0)
								{
									double x = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
									if (view3D)
									{
										Area3DDataPointFace area3DDataPointFace = new Area3DDataPointFace(((dataPoint.Parent.PlotGroup.AxisY.AxisMinimum != null && internalYValue <= limitingYValue && limitingYValue > 0.0) || (internalYValue < limitingYValue && dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0)) ? 0.0 : num);
										area3DDataPointFace.FrontFacePoints.Add(new Point(x, num5));
										area3DDataPointFace.FrontFacePoints.Add(dataPoint.DpInfo.VisualPosition);
										dataPoint.DpInfo.Faces.Area3DLeftFace = area3DDataPointFace;
										Area3DDataPointFace area3DDataPointFace2 = new Area3DDataPointFace(((dataPoint.Parent.PlotGroup.AxisY.AxisMinimum != null && internalYValue <= limitingYValue && limitingYValue > 0.0) || (internalYValue < limitingYValue && dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0)) ? 0.0 : num);
										area3DDataPointFace2.FrontFacePoints.Add(dataPoint.DpInfo.VisualPosition);
										area3DDataPointFace2.FrontFacePoints.Add(dataPoint3.DpInfo.VisualPosition);
										dataPoint.DpInfo.Faces.Area3DRightTopFace = area3DDataPointFace2;
										faces2.Area3DLeftTopFace = area3DDataPointFace2;
									}
									path = new Path
									{
										Opacity = dataPoint.Parent.Opacity
									};
									AreaChart.ApplyBorderProperties(path, dataPoint.Parent);
									faces.FrontFacePaths.Add(path);
									PathGeometry pathGeometry = new PathGeometry();
									pathFigure = new PathFigure
									{
										StartPoint = new Point(x, num5),
										IsClosed = true
									};
									pathGeometry.Figures.Add(pathFigure);
									path.Data = pathGeometry;
									LineSegment lineSegment = new LineSegment
									{
										Point = dataPoint.DpInfo.VisualPosition
									};
									pathFigure.Segments.Add(lineSegment);
									faces3.AreaFrontFaceLineSegment = lineSegment;
								}
								else
								{
									if (view3D)
									{
										Area3DDataPointFace area3DDataPointFace3 = new Area3DDataPointFace(num);
										area3DDataPointFace3.FrontFacePoints.Add(dataPoint.DpInfo.VisualPosition);
										area3DDataPointFace3.FrontFacePoints.Add(dataPoint3.DpInfo.VisualPosition);
										dataPoint.DpInfo.Faces.Area3DRightTopFace = area3DDataPointFace3;
										faces2.Area3DLeftTopFace = area3DDataPointFace3;
									}
									else if (dataPoint.Parent.Bevel.Value)
									{
										AreaChart.CreateBevelFor2DArea(canvas4, dataPoint, dataPoint2, false, false);
									}
									LineSegment lineSegment2 = new LineSegment
									{
										Point = dataPoint.DpInfo.VisualPosition
									};
									pathFigure.Segments.Add(lineSegment2);
									faces3.AreaFrontFaceLineSegment = lineSegment2;
								}
								if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled) || (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled))
								{
									double num7;
									double num8;
									LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num7, out num8);
									if (dataPoint.Parent.PlotGroup.AxisY.AxisMinimum != null && ((internalYValue < limitingYValue && limitingYValue > 0.0) || (internalYValue < limitingYValue && dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue < dataPoint.Parent.PlotGroup.AxisY.InternalAxisMinimum))
									{
										dataPoint.DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
										if (dataPoint.DpInfo.LabelVisual != null)
										{
											(((dataPoint.DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Collapsed;
										}
									}
								}
								if (view3D)
								{
									int val = AreaChart.Draw3DArea(canvas4, dataPoint2, dataPoint, dataPoint3, ref faces, ref faces3, dataPoint.Parent, num5);
									num6 = Math.Max(num6, val);
								}
								if (i == current.Count - 2)
								{
									if (view3D)
									{
										IDataPoint dataPoint4 = dataPoint3;
										double internalYValue2 = dataPoint4.DpInfo.InternalYValue;
										Area3DDataPointFace area3DDataPointFace4 = new Area3DDataPointFace(((dataPoint4.Parent.PlotGroup.AxisY.AxisMinimum != null && internalYValue2 <= limitingYValue && limitingYValue > 0.0) || (internalYValue2 < limitingYValue && dataPoint4.Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0)) ? 0.0 : num);
										area3DDataPointFace4.FrontFacePoints.Add(dataPoint3.DpInfo.VisualPosition);
										area3DDataPointFace4.FrontFacePoints.Add(new Point(dataPoint3.DpInfo.VisualPosition.X, num5));
										dataPoint3.DpInfo.Faces.Area3DRightFace = area3DDataPointFace4;
										path2 = new Path();
										path2.Fill = (current[0].Parent.LightingEnabled.Value ? Graphics.GetTopFaceBrush(current[0].Parent.Color) : current[0].Parent.Color);
										PathGeometry pathGeometry2 = new PathGeometry();
										PathFigure pathFigure2 = new PathFigure
										{
											StartPoint = new Point(current[0].DpInfo.VisualPosition.X, num5)
										};
										pathGeometry2.Figures.Add(pathFigure2);
										pathFigure2.Segments.Add(new LineSegment
										{
											Point = new Point(current[0].DpInfo.VisualPosition.X + num, num5 - num)
										});
										pathFigure2.Segments.Add(new LineSegment
										{
											Point = new Point(dataPoint4.DpInfo.VisualPosition.X + num, num5 - num)
										});
										pathFigure2.Segments.Add(new LineSegment
										{
											Point = new Point(dataPoint4.DpInfo.VisualPosition.X, num5)
										});
										path2.Data = pathGeometry2;
										path2.SetValue(Panel.ZIndexProperty, 1);
										path2.Opacity = dataPoint4.Parent.Opacity;
										canvas3.Children.Add(path2);
										faces.FrontFacePaths.Add(path2);
										dataSeries.Faces.VisualComponents.Add(path2);
										if (animationEnabled)
										{
											dataSeries.Storyboard = AnimationHelper.ApplyOpacityAnimation(path2, dataSeries, dataSeries.Storyboard, 0.25, 1.0, 0.0, 1.0);
										}
									}
									else if (dataPoint3.Parent.Bevel.Value)
									{
										AreaChart.CreateBevelFor2DArea(canvas4, dataPoint3, dataPoint, false, false);
									}
									LineSegment lineSegment3 = new LineSegment
									{
										Point = dataPoint3.DpInfo.VisualPosition
									};
									pathFigure.Segments.Add(lineSegment3);
									faces2.AreaFrontFaceLineSegment = lineSegment3;
									lineSegment3 = new LineSegment
									{
										Point = new Point(dataPoint3.DpInfo.VisualPosition.X, num5)
									};
									pathFigure.Segments.Add(lineSegment3);
									faces2.AreaFrontFaceBaseLineSegment = lineSegment3;
									faces2.NextDataPoint = dataPoint3;
									if (view3D)
									{
										int val2 = AreaChart.Draw3DArea(canvas4, dataPoint2, dataPoint3, dataPoint3, ref faces, ref faces2, dataPoint3.Parent, num5);
										num6 = Math.Max(num6, val2);
									}
									if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint3, VcProperties.MarkerEnabled) || (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint3, VcProperties.LabelEnabled))
									{
										double num9;
										double num10;
										LineChart.CreateMarkerAForLineDataPoint(dataPoint3, width, height, ref canvas2, out num9, out num10);
										double internalYValue3 = dataPoint3.DpInfo.InternalYValue;
										if (dataPoint3.Parent.PlotGroup.AxisY.AxisMinimum != null && ((internalYValue3 < limitingYValue && limitingYValue > 0.0) || (internalYValue3 < limitingYValue && dataPoint3.Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue3 < dataPoint3.Parent.PlotGroup.AxisY.InternalAxisMinimum))
										{
											dataPoint3.DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
											if (dataPoint3.DpInfo.LabelVisual != null)
											{
												(((dataPoint3.DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Collapsed;
											}
										}
									}
									dataPoint3.DpInfo.ParsedToolTipText = dataPoint3.TextParser(dataPoint3.ToolTipText);
									if (!dataPoint3.IsLightDataPoint)
									{
										(dataPoint3 as DataPoint).ParsedHref = dataPoint3.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint3, VcProperties.Href));
									}
								}
								dataPoint2 = dataPoint;
							}
							if (path != null)
							{
								if (view3D)
								{
									path.Fill = (current[0].Parent.LightingEnabled.Value ? Graphics.GetFrontFaceBrush(current[0].Parent.Color) : current[0].Parent.Color);
								}
								else
								{
									path.Fill = (current[0].Parent.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(current[0].Parent.Color, "Linear", null) : current[0].Parent.Color);
								}
								dataSeries.Faces.VisualComponents.Add(path);
								path.SetValue(Panel.ZIndexProperty, num6);
								canvas4.Children.Add(path);
							}
							foreach (FrameworkElement current2 in dataSeries.Faces.VisualComponents)
							{
								if (!dataPoint.IsLightDataPoint)
								{
									AreaChart.AttachEvents2AreaVisual(dataPoint as DataPoint, dataPoint as DataPoint, current2);
								}
							}
						}
					}
				}
				foreach (FrameworkElement current3 in dataSeries.Faces.VisualComponents)
				{
					AreaChart.AttachEvents2AreaVisual(dataSeries, dataSeries, current3);
				}
				if (VisifireControl.IsMediaEffectsEnabled)
				{
					if (dataSeries.Faces != null && dataSeries.Faces.Visual != null && dataSeries.Effect != null)
					{
						dataSeries.Faces.Visual.Effect = dataSeries.Effect;
					}
					if (dataSeries.ShadowEnabled.Value && dataSeries.Faces != null && dataSeries.Faces.Visual != null && dataSeries.Effect == null)
					{
						dataSeries.Faces.Visual.Effect = ExtendedGraphics.GetShadowEffect(135.0, 2.0, 1.0);
					}
				}
				if (!chart.IndicatorEnabled)
				{
					dataSeries.AttachAreaToolTip(chart, faces.VisualComponents);
				}
				canvas4.Tag = null;
				if (list.Count > 0)
				{
					ColumnChart.CreateOrUpdatePlank(chart, dataSeries.PlotGroup.AxisY, canvas3, num, Orientation.Horizontal);
				}
				if (animationEnabled)
				{
					ScaleTransform scaleTransform = new ScaleTransform
					{
						ScaleY = 0.0
					};
					canvas4.RenderTransformOrigin = new Point(0.5, num5 / height);
					canvas4.RenderTransform = scaleTransform;
					List<KeySpline> splain = AnimationHelper.GenerateKeySplineList(new Point[]
					{
						new Point(0.0, 0.0),
						new Point(1.0, 1.0),
						new Point(0.0, 1.0),
						new Point(0.5, 1.0)
					});
					double num11 = 1.0;
					dataSeries.Storyboard = AnimationHelper.ApplyPropertyAnimation(scaleTransform, "(ScaleTransform.ScaleY)", dataSeries, dataSeries.Storyboard, num11, new double[]
					{
						0.0,
						1.0
					}, new double[]
					{
						0.0,
						1.0
					}, splain);
					if (canvas2.Children.Count > 0)
					{
						dataSeries.Storyboard = AnimationHelper.ApplyOpacityAnimation(canvas2, dataSeries, dataSeries.Storyboard, num11 + 0.25, 1.0, 0.0, 1.0);
					}
				}
			}
			canvas4.SetValue(Panel.ZIndexProperty, 2);
			canvas3.Children.Add(canvas4);
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
			PlotArea plotArea = chart.PlotArea;
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			rectangleGeometry.Rect = new Rect(0.0, plotArea.BorderThickness.Top - num, width + num, height + num - plotArea.BorderThickness.Bottom - plotArea.BorderThickness.Top);
			canvas3.Clip = rectangleGeometry;
			rectangleGeometry = new RectangleGeometry();
			double x2 = 0.0;
			double y = -num - 4.0;
			double width2 = width + num;
			double height2 = height + num + chart.ChartArea.PLANK_THICKNESS + 10.0;
			AreaChart.GetClipCoordinates(chart, ref x2, ref y, ref width2, ref height2, num3, num4);
			rectangleGeometry.Rect = new Rect(x2, y, width2, height2);
			canvas2.Clip = rectangleGeometry;
			return canvas;
		}

		internal static void GetClipCoordinates(Chart chart, ref double clipLeft, ref double clipTop, ref double clipWidth, ref double clipHeight, double minimumXValue, double maximumXValue)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			Axis.CalculateTotalTickLength(chart, ref num, ref num2, ref num3);
			if (minimumXValue >= chart.ChartArea.AxisX.InternalAxisMinimum)
			{
				clipLeft -= num2;
				clipWidth += num2;
			}
			if (maximumXValue <= chart.ChartArea.AxisX.InternalAxisMaximum)
			{
				clipWidth += num3;
			}
		}

		internal static Canvas Get3DArea(DataSeries currentDataSeries, ref Faces faces, PolygonalChartShapeParams areaParams)
		{
			Canvas canvas = new Canvas();
			canvas.Width = areaParams.Size.Width;
			canvas.Height = areaParams.Size.Height;
			Brush fill = areaParams.Lighting ? Graphics.GetRightFaceBrush(areaParams.Background) : areaParams.Background;
			Brush fill2 = areaParams.Lighting ? Graphics.GetTopFaceBrush(areaParams.Background) : areaParams.Background;
			int num = areaParams.IsPositive ? (areaParams.Points.Count - 1) : areaParams.Points.Count;
			Canvas canvas2 = new Canvas();
			Rect bounds = AreaChart.GetBounds(areaParams.Points);
			canvas2.Width = bounds.Width + areaParams.Depth3D;
			canvas2.Height = bounds.Height + areaParams.Depth3D;
			canvas2.SetValue(Canvas.TopProperty, bounds.Top - areaParams.Depth3D);
			canvas2.SetValue(Canvas.LeftProperty, bounds.Left);
			canvas.Children.Add(canvas2);
			Point centroid;
			for (int i = 0; i < num; i++)
			{
				Polygon polygon = new Polygon
				{
					Tag = new ElementData
					{
						Element = areaParams.TagReference
					}
				};
				PointCollection pointCollection = new PointCollection();
				int index = i % areaParams.Points.Count;
				int index2 = (i + 1) % areaParams.Points.Count;
				pointCollection.Add(areaParams.Points[index]);
				pointCollection.Add(areaParams.Points[index2]);
				pointCollection.Add(new Point(areaParams.Points[index2].X + areaParams.Depth3D, areaParams.Points[index2].Y - areaParams.Depth3D));
				pointCollection.Add(new Point(areaParams.Points[index].X + areaParams.Depth3D, areaParams.Points[index].Y - areaParams.Depth3D));
				polygon.Points = pointCollection;
				centroid = AreaChart.GetCentroid(pointCollection);
				int areaZIndex = AreaChart.GetAreaZIndex(centroid.X, centroid.Y, areaParams.IsPositive);
				polygon.SetValue(Panel.ZIndexProperty, areaZIndex);
				if (i == areaParams.Points.Count - 2)
				{
					polygon.Fill = fill;
					(polygon.Tag as ElementData).VisualElementName = "Side";
				}
				else
				{
					polygon.Fill = fill2;
					(polygon.Tag as ElementData).VisualElementName = "Top";
				}
				polygon.Stroke = areaParams.BorderColor;
				polygon.StrokeDashArray = ((areaParams.BorderStyle != null) ? ExtendedGraphics.CloneCollection(areaParams.BorderStyle) : areaParams.BorderStyle);
				polygon.StrokeThickness = areaParams.BorderThickness;
				polygon.StrokeMiterLimit = 1.0;
				Rect bounds2 = AreaChart.GetBounds(pointCollection);
				polygon.Stretch = Stretch.Fill;
				polygon.Width = bounds2.Width;
				polygon.Height = bounds2.Height;
				polygon.SetValue(Canvas.TopProperty, bounds2.Y - (bounds.Top - areaParams.Depth3D));
				polygon.SetValue(Canvas.LeftProperty, bounds2.X - bounds.X);
				faces.Parts.Add(polygon);
				canvas2.Children.Add(polygon);
			}
			Polygon polygon2 = new Polygon
			{
				Tag = new ElementData
				{
					Element = areaParams.TagReference,
					VisualElementName = "AreaBase"
				}
			};
			faces.Parts.Add(polygon2);
			centroid = AreaChart.GetCentroid(areaParams.Points);
			polygon2.SetValue(Panel.ZIndexProperty, (int)centroid.Y + 1000);
			polygon2.Fill = (areaParams.Lighting ? Graphics.GetFrontFaceBrush(areaParams.Background) : areaParams.Background);
			polygon2.Stroke = areaParams.BorderColor;
			polygon2.StrokeDashArray = ((areaParams.BorderStyle != null) ? ExtendedGraphics.CloneCollection(areaParams.BorderStyle) : areaParams.BorderStyle);
			polygon2.StrokeThickness = areaParams.BorderThickness;
			polygon2.StrokeMiterLimit = 1.0;
			polygon2.Points = areaParams.Points;
			polygon2.Stretch = Stretch.Fill;
			polygon2.Width = bounds.Width;
			polygon2.Height = bounds.Height;
			polygon2.SetValue(Canvas.TopProperty, areaParams.Depth3D);
			polygon2.SetValue(Canvas.LeftProperty, 0.0);
			if (areaParams.AnimationEnabled)
			{
				areaParams.Storyboard = AreaChart.ApplyAreaAnimation(currentDataSeries, canvas2, areaParams.Storyboard, areaParams.IsPositive, 0.0);
			}
			canvas2.Children.Add(polygon2);
			return canvas;
		}

		internal static Canvas GetVisualObjectForStackedAreaChart(Panel preExistingPanel, double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, double plankDepth, bool animationEnabled)
		{
			if (double.IsNaN(width) || double.IsNaN(height) || width <= 0.0 || height <= 0.0)
			{
				return null;
			}
			bool flag = false;
			double num = plankDepth / (double)plotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
			double num2 = num * (double)(plotDetails.SeriesDrawingIndex[seriesList[0]] + 1);
			if (double.IsNaN(num2) || double.IsInfinity(num2))
			{
				return null;
			}
			Canvas canvas;
			Canvas canvas2;
			Canvas canvas3;
			RenderHelper.PrepareCanvas4Drawing(preExistingPanel as Canvas, out canvas, out canvas2, out canvas3, width, height);
			canvas.SetValue(Canvas.TopProperty, num2);
			canvas.SetValue(Canvas.LeftProperty, -num2);
			IEnumerable<PlotGroup> source = from series in seriesList
			where series.PlotGroup != null
			select series.PlotGroup;
			if (source.Count<PlotGroup>() == 0)
			{
				return canvas;
			}
			PlotGroup plotGroup = source.First<PlotGroup>();
			Dictionary<double, List<double>> dataPointValuesInStackedOrder4StackedArea = plotDetails.GetDataPointValuesInStackedOrder4StackedArea(plotGroup);
			Dictionary<double, List<IDataPoint>> dataPointInStackOrder4StackedArea = plotDetails.GetDataPointInStackOrder4StackedArea(plotGroup);
			double[] xValuesUnderViewPort = RenderHelper.GetXValuesUnderViewPort(dataPointValuesInStackedOrder4StackedArea.Keys.ToList<double>(), plotGroup.AxisX, plotGroup.AxisY, false);
			double minimumX = plotGroup.MinimumX;
			double maximumX = plotGroup.MaximumX;
			double num3 = 0.0;
			foreach (DataSeries current in seriesList)
			{
				current.Faces = null;
				if (current.ToolTipElement != null)
				{
					current.ToolTipElement.Hide();
				}
			}
			double num4 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num3);
			for (int i = 0; i < xValuesUnderViewPort.Length - 1; i++)
			{
				List<double> list = dataPointValuesInStackedOrder4StackedArea[xValuesUnderViewPort[i]];
				List<double> list2 = dataPointValuesInStackedOrder4StackedArea[xValuesUnderViewPort[i + 1]];
				double num5 = num3;
				double num6 = num3;
				List<IDataPoint> list3 = dataPointInStackOrder4StackedArea[xValuesUnderViewPort[i]];
				List<IDataPoint> list4 = dataPointInStackOrder4StackedArea[xValuesUnderViewPort[i + 1]];
				double num7 = 0.0;
				double num8 = 0.0;
				double num9 = 0.0;
				double num10 = 0.0;
				for (int j = 0; j < list.Count; j++)
				{
					if (j < list2.Count && j < list.Count && list3[j] != null && list4[j] != null)
					{
						double num11 = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, xValuesUnderViewPort[i]);
						double num12 = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, xValuesUnderViewPort[i + 1]);
						double internalYValue = list3[j].DpInfo.InternalYValue;
						double internalYValue2 = list4[j].DpInfo.InternalYValue;
						double num13;
						double num14;
						if (plotGroup.AxisY.Logarithmic)
						{
							num13 = Math.Log(num5 + list[j], plotGroup.AxisY.LogarithmBase);
							if (double.IsNegativeInfinity(num13))
							{
								num13 = 0.0;
							}
							num14 = Math.Log(num6 + list2[j], plotGroup.AxisY.LogarithmBase);
							if (double.IsNegativeInfinity(num14))
							{
								num14 = 0.0;
							}
						}
						else
						{
							num13 = num5 + list[j];
							num14 = num6 + list2[j];
							num7 = num5;
							num8 = num6;
						}
						double num15 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num13);
						double num16 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num14);
						double num17 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num7);
						double num18 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num8);
						if (plotGroup.AxisY.Logarithmic)
						{
							num9 += list[j];
							num10 += list2[j];
							num7 = Math.Log(num9, plotGroup.AxisY.LogarithmBase);
							if (double.IsNegativeInfinity(num7))
							{
								num7 = 0.0;
							}
							num8 = Math.Log(num10, plotGroup.AxisY.LogarithmBase);
							if (double.IsNegativeInfinity(num8))
							{
								num8 = 0.0;
							}
						}
						Point intersection = AreaChart.GetIntersection(new Point(num11, num17), new Point(num12, num18), new Point(num11, num15), new Point(num12, num16));
						bool isTopOfStack = false;
						if (j == list.Count - 1)
						{
							isTopOfStack = true;
						}
						Marker markerForDataPoint = AreaChart.GetMarkerForDataPoint(chart, height, isTopOfStack, list3[j], num15, internalYValue > 0.0);
						if ((bool)DataPointHelper.GetDataPointValueFromProperty(list3[j], VcProperties.MarkerEnabled) || (bool)DataPointHelper.GetDataPointValueFromProperty(list3[j], VcProperties.LabelEnabled))
						{
							if (double.IsNaN(list3[j].DpInfo.InternalYValue))
							{
								list3[j].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
							}
							if (list3[j].Parent.PlotGroup.AxisY.AxisMinimum != null && ((internalYValue < num3 && num3 > 0.0) || (internalYValue < num3 && list3[j].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || internalYValue < plotGroup.AxisY.InternalAxisMinimum))
							{
								list3[j].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
								if (list3[j].DpInfo.LabelVisual != null)
								{
									(((list3[j].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Collapsed;
								}
							}
						}
						DataSeries dataSeries;
						if (markerForDataPoint != null)
						{
							if (list3[j].Parent.Storyboard == null)
							{
								list3[j].Parent.Storyboard = new Storyboard();
							}
							dataSeries = list3[j].Parent;
							markerForDataPoint.AddToParent(canvas2, num11, num15, new Point(0.5, 0.5));
							if (animationEnabled)
							{
								double beginTime = 1.0;
								list3[j].Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(markerForDataPoint, dataSeries, list3[j].Parent.Storyboard, beginTime, (double)DataPointHelper.GetDataPointValueFromProperty(list3[j], VcProperties.Opacity) * list3[j].Parent.Opacity);
							}
						}
						list3[j].DpInfo.VisualPosition = new Point(num11, num15);
						if (i + 1 == xValuesUnderViewPort.Length - 1)
						{
							markerForDataPoint = AreaChart.GetMarkerForDataPoint(chart, height, isTopOfStack, list4[j], num16, internalYValue2 > 0.0);
							if ((bool)DataPointHelper.GetDataPointValueFromProperty(list4[j], VcProperties.MarkerEnabled) || (bool)DataPointHelper.GetDataPointValueFromProperty(list4[j], VcProperties.LabelEnabled))
							{
								if (double.IsNaN(internalYValue2))
								{
									list4[j].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
								}
								if (list4[j].Parent.PlotGroup.AxisY.AxisMinimum != null && internalYValue2 < plotGroup.AxisY.InternalAxisMinimum)
								{
									list4[j].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
									if (list4[j].DpInfo.LabelVisual != null)
									{
										(((list4[j].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Collapsed;
									}
								}
							}
							if (markerForDataPoint != null)
							{
								if (list4[j].Parent.Storyboard == null)
								{
									list4[j].Parent.Storyboard = new Storyboard();
								}
								dataSeries = list4[j].Parent;
								markerForDataPoint.AddToParent(canvas2, num12, num16, new Point(0.5, 0.5));
								if (animationEnabled)
								{
									double beginTime2 = 1.0;
									list4[j].Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(markerForDataPoint, dataSeries, list4[j].Parent.Storyboard, beginTime2, (double)DataPointHelper.GetDataPointValueFromProperty(list4[j], VcProperties.Opacity) * list4[j].Parent.Opacity);
								}
							}
							list4[j].DpInfo.VisualPosition = new Point(num12, num16);
						}
						if (list3[j].Parent.Faces == null)
						{
							list3[j].Parent.Faces = new Faces();
						}
						if (list3[j].DpInfo.Faces == null)
						{
							list3[j].DpInfo.Faces = new Faces();
						}
						List<PointCollection> list6;
						if (!double.IsNaN(intersection.X) && !double.IsInfinity(intersection.X) && intersection.X >= num11 && intersection.X <= num12)
						{
							List<PointCollection> list5 = AreaChart.GeneratePointsCollection(num11, num15, num17, intersection.X, intersection.Y, intersection.Y, num4);
							List<PointCollection> collection = AreaChart.GeneratePointsCollection(intersection.X, intersection.Y, intersection.Y, num12, num16, num18, num4);
							list6 = list5;
							list6.InsertRange(list6.Count, collection);
						}
						else
						{
							list6 = AreaChart.GeneratePointsCollection(num11, num15, num17, num12, num16, num18, num4);
						}
						DataSeries parent = list3[j].Parent;
						Brush color = parent.Color;
						dataSeries = parent;
						PolygonalChartShapeParams areaParms = AreaChart.GetAreaParms(parent, color, num);
						double num19 = 0.8;
						foreach (PointCollection current2 in list6)
						{
							current2[current2.Count - 2] = new Point(current2[current2.Count - 2].X + num19, current2[current2.Count - 2].Y);
							current2[current2.Count - 1] = new Point(current2[current2.Count - 1].X + num19, current2[current2.Count - 1].Y);
							areaParms.Points = current2;
							Faces faces = list3[j].Parent.Faces;
							if (faces.Parts == null)
							{
								faces.Parts = new List<DependencyObject>();
							}
							if (chart.View3D)
							{
								Point centroid = AreaChart.GetCentroid(current2);
								areaParms.IsPositive = (centroid.Y < num4);
								Canvas stacked3DAreaFrontFace = AreaChart.GetStacked3DAreaFrontFace(ref faces, areaParms);
								Canvas stacked3DSideFaces = AreaChart.GetStacked3DSideFaces(ref faces, areaParms);
								stacked3DSideFaces.SetValue(Panel.ZIndexProperty, AreaChart.GetStackedAreaZIndex(centroid.X, centroid.Y, areaParms.IsPositive, j));
								stacked3DAreaFrontFace.SetValue(Panel.ZIndexProperty, 50000);
								canvas3.Children.Add(stacked3DSideFaces);
								canvas3.Children.Add(stacked3DAreaFrontFace);
								list3[j].Parent.Faces.VisualComponents.Add(stacked3DSideFaces);
								list3[j].Parent.Faces.VisualComponents.Add(stacked3DAreaFrontFace);
								list3[j].DpInfo.Faces.VisualComponents.Add(stacked3DSideFaces);
								list3[j].DpInfo.Faces.VisualComponents.Add(stacked3DAreaFrontFace);
								if (animationEnabled)
								{
									if (list3[j].Parent.Storyboard == null)
									{
										list3[j].Parent.Storyboard = new Storyboard();
									}
									Storyboard storyboard = list3[j].Parent.Storyboard;
									double num20 = 0.0;
									storyboard = AreaChart.ApplyStackedAreaAnimation(dataSeries, stacked3DSideFaces, storyboard, 1.0 / (double)seriesList.Count * (double)seriesList.IndexOf(list3[j].Parent) + 0.05 + num20, 1.0 / (double)seriesList.Count);
									storyboard = AreaChart.ApplyStackedAreaAnimation(dataSeries, stacked3DAreaFrontFace, storyboard, 1.0 / (double)seriesList.Count * (double)seriesList.IndexOf(list3[j].Parent) + num20, 1.0 / (double)seriesList.Count);
								}
							}
							else
							{
								Canvas stacked2DArea = AreaChart.GetStacked2DArea(ref faces, areaParms);
								canvas3.Children.Add(stacked2DArea);
								list3[j].Parent.Faces.VisualComponents.Add(stacked2DArea);
								list3[j].DpInfo.Faces.VisualComponents.Add(stacked2DArea);
								if (animationEnabled)
								{
									if (list3[j].Parent.Storyboard == null)
									{
										list3[j].Parent.Storyboard = new Storyboard();
									}
									Storyboard storyboard2 = list3[j].Parent.Storyboard;
									double num21 = 0.0;
									storyboard2 = AreaChart.ApplyStackedAreaAnimation(dataSeries, stacked2DArea, storyboard2, 1.0 / (double)seriesList.Count * (double)seriesList.IndexOf(list3[j].Parent) + num21, 1.0 / (double)seriesList.Count);
								}
							}
							list3[j].Parent.Faces.Visual = canvas;
							if (VisifireControl.IsMediaEffectsEnabled)
							{
								if (parent.Faces != null && parent.Faces.Visual != null && parent.Effect != null)
								{
									parent.Faces.Visual.Effect = parent.Effect;
								}
								if (parent.ShadowEnabled.Value && parent.Faces != null && parent.Faces.Visual != null && parent.Effect == null)
								{
									parent.Faces.Visual.Effect = ExtendedGraphics.GetShadowEffect(135.0, 2.0, 1.0);
								}
							}
						}
						num5 += list[j];
						num6 += list2[j];
					}
				}
			}
			if (xValuesUnderViewPort.Any<double>() && !flag && chart.View3D && plotGroup.AxisY.InternalAxisMinimum < 0.0 && plotGroup.AxisY.InternalAxisMaximum > 0.0)
			{
				Brush frontBrush;
				Brush topBrush;
				Brush rightBrush;
				ExtendedGraphics.GetBrushesForPlank(chart, out frontBrush, out topBrush, out rightBrush, true);
				Faces faces2 = ColumnChart.Get3DPlank(width, 1.0, num, frontBrush, topBrush, rightBrush);
				Panel panel = faces2.Visual as Panel;
				double num22 = height - Graphics.ValueToPixelPosition(0.0, height, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, 0.0);
				panel.SetValue(Canvas.LeftProperty, 0.0);
				panel.SetValue(Canvas.TopProperty, num22);
				panel.SetValue(Panel.ZIndexProperty, 0);
				panel.Opacity = 0.7;
				canvas.Children.Add(panel);
			}
			chart.ChartArea.DisableIndicators();
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
			PlotArea plotArea = chart.PlotArea;
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			rectangleGeometry.Rect = new Rect(0.0, plotArea.BorderThickness.Top - num, width + num, height + num - plotArea.BorderThickness.Bottom - plotArea.BorderThickness.Top);
			canvas3.Clip = rectangleGeometry;
			rectangleGeometry = new RectangleGeometry();
			double x = 0.0;
			double y = -num - 4.0;
			double width2 = width + num;
			double height2 = height + num + chart.ChartArea.PLANK_THICKNESS + 10.0;
			AreaChart.GetClipCoordinates(chart, ref x, ref y, ref width2, ref height2, minimumX, maximumX);
			rectangleGeometry.Rect = new Rect(x, y, width2, height2);
			canvas2.Clip = rectangleGeometry;
			return canvas;
		}

		internal static Canvas GetVisualObjectForStackedArea100Chart(Panel preExistingPanel, double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, double plankDepth, bool animationEnabled)
		{
			if (double.IsNaN(width) || double.IsNaN(height) || width <= 0.0 || height <= 0.0)
			{
				return null;
			}
			bool flag = false;
			double num = plankDepth / (double)plotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
			double num2 = num * (double)(plotDetails.SeriesDrawingIndex[seriesList[0]] + 1);
			if (double.IsNaN(num2) || double.IsInfinity(num2))
			{
				return null;
			}
			Canvas canvas;
			Canvas canvas2;
			Canvas canvas3;
			RenderHelper.PrepareCanvas4Drawing(preExistingPanel as Canvas, out canvas, out canvas2, out canvas3, width, height);
			canvas.SetValue(Canvas.TopProperty, num2);
			canvas.SetValue(Canvas.LeftProperty, -num2);
			canvas2.SetValue(Canvas.TopProperty, 0.0);
			canvas2.SetValue(Canvas.LeftProperty, 0.0);
			canvas2.SetValue(Panel.ZIndexProperty, 1);
			IEnumerable<PlotGroup> source = from series in seriesList
			where series.PlotGroup != null
			select series.PlotGroup;
			if (source.Count<PlotGroup>() == 0)
			{
				return canvas;
			}
			PlotGroup plotGroup = source.First<PlotGroup>();
			Dictionary<double, List<double>> dataPointValuesInStackedOrder4StackedArea = plotDetails.GetDataPointValuesInStackedOrder4StackedArea(plotGroup);
			Dictionary<double, List<IDataPoint>> dataPointInStackOrder4StackedArea = plotDetails.GetDataPointInStackOrder4StackedArea(plotGroup);
			double[] array = dataPointValuesInStackedOrder4StackedArea.Keys.ToArray<double>();
			double minimumX = plotGroup.MinimumX;
			double maximumX = plotGroup.MaximumX;
			double num3 = 0.0;
			foreach (DataSeries current in seriesList)
			{
				current.Faces = null;
				if (current.ToolTipElement != null)
				{
					current.ToolTipElement.Hide();
				}
			}
			double num4 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num3);
			for (int i = 0; i < array.Length - 1; i++)
			{
				List<double> list = dataPointValuesInStackedOrder4StackedArea[array[i]];
				List<double> list2 = dataPointValuesInStackedOrder4StackedArea[array[i + 1]];
				double num5 = num3;
				double num6 = num3;
				double num7 = plotGroup.XWiseStackedDataList[array[i]].AbsoluteYValueSum;
				double num8 = plotGroup.XWiseStackedDataList[array[i + 1]].AbsoluteYValueSum;
				List<IDataPoint> list3 = dataPointInStackOrder4StackedArea[array[i]];
				List<IDataPoint> list4 = dataPointInStackOrder4StackedArea[array[i + 1]];
				if (double.IsNaN(num7))
				{
					num7 = 1.0;
				}
				if (double.IsNaN(num8))
				{
					num8 = 1.0;
				}
				double num9 = 0.0;
				double num10 = 0.0;
				double num11 = 0.0;
				double num12 = 0.0;
				for (int j = 0; j < list.Count; j++)
				{
					if (j < list2.Count && j < list.Count && list3[j] != null && list4[j] != null)
					{
						double num13 = 0.0;
						double num14 = 0.0;
						if (!double.IsNaN(num14) && !double.IsNaN(num13) && (j != 0 || !double.IsNaN(list[j])))
						{
							double internalYValue = list3[j].DpInfo.InternalYValue;
							double internalYValue2 = list4[j].DpInfo.InternalYValue;
							double num15;
							double num16;
							if (plotGroup.AxisY.Logarithmic)
							{
								num13 = list[j] / num7 * 100.0;
								num14 = list2[j] / num8 * 100.0;
								num13 = (double.IsNaN(num13) ? 0.0 : num13);
								num14 = (double.IsNaN(num14) ? 0.0 : num14);
								num15 = Math.Log(num13 + num5, plotGroup.AxisY.LogarithmBase);
								if (double.IsNegativeInfinity(num15))
								{
									num15 = 0.0;
								}
								num16 = Math.Log(num14 + num6, plotGroup.AxisY.LogarithmBase);
								if (double.IsNegativeInfinity(num16))
								{
									num16 = 0.0;
								}
							}
							else
							{
								num13 = list[j] / num7 * 100.0;
								num14 = list2[j] / num8 * 100.0;
								num13 = (double.IsNaN(num13) ? 0.0 : num13);
								num14 = (double.IsNaN(num14) ? 0.0 : num14);
								num15 = num5 + num13;
								num16 = num6 + num14;
								num9 = num5;
								num10 = num6;
							}
							double num17 = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, array[i]);
							double num18 = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, array[i + 1]);
							double num19 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num15);
							double num20 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num16);
							double num21 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num9);
							double num22 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, num10);
							Point intersection = AreaChart.GetIntersection(new Point(num17, num21), new Point(num18, num22), new Point(num17, num19), new Point(num18, num20));
							if (plotGroup.AxisY.Logarithmic)
							{
								num11 += num13;
								num12 += num14;
								num9 = Math.Log(num11, plotGroup.AxisY.LogarithmBase);
								if (double.IsNegativeInfinity(num9))
								{
									num9 = 0.0;
								}
								num10 = Math.Log(num12, plotGroup.AxisY.LogarithmBase);
								if (double.IsNegativeInfinity(num10))
								{
									num10 = 0.0;
								}
							}
							Marker markerForDataPoint = AreaChart.GetMarkerForDataPoint(chart, height, false, list3[j], num19, internalYValue > 0.0);
							if (((bool)DataPointHelper.GetDataPointValueFromProperty(list3[j], VcProperties.MarkerEnabled) || (bool)DataPointHelper.GetDataPointValueFromProperty(list3[j], VcProperties.LabelEnabled)) && double.IsNaN(internalYValue))
							{
								list3[j].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
							}
							DataSeries dataSeries;
							if (markerForDataPoint != null)
							{
								dataSeries = list3[j].Parent;
								if (list3[j].Parent.Storyboard == null)
								{
									list3[j].Parent.Storyboard = new Storyboard();
								}
								markerForDataPoint.AddToParent(canvas2, num17, num19, new Point(0.5, 0.5));
								if (animationEnabled)
								{
									double beginTime = 1.0;
									list3[j].Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(markerForDataPoint, dataSeries, list3[j].Parent.Storyboard, beginTime, (double)DataPointHelper.GetDataPointValueFromProperty(list3[j], VcProperties.Opacity) * list3[j].Parent.Opacity);
								}
							}
							list3[j].DpInfo.VisualPosition = new Point(num17, num19);
							if (i + 1 == array.Length - 1)
							{
								markerForDataPoint = AreaChart.GetMarkerForDataPoint(chart, height, false, list4[j], num20, internalYValue2 > 0.0);
								if (((bool)DataPointHelper.GetDataPointValueFromProperty(list4[j], VcProperties.MarkerEnabled) || (bool)DataPointHelper.GetDataPointValueFromProperty(list4[j], VcProperties.LabelEnabled)) && double.IsNaN(internalYValue2))
								{
									list4[j].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
								}
								if (markerForDataPoint != null)
								{
									if (list3[j].Parent.Storyboard == null)
									{
										list3[j].Parent.Storyboard = new Storyboard();
									}
									dataSeries = list3[j].Parent;
									markerForDataPoint.AddToParent(canvas2, num18, num20, new Point(0.5, 0.5));
									if (animationEnabled)
									{
										double beginTime2 = 1.0;
										list4[j].Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(markerForDataPoint, dataSeries, list4[j].Parent.Storyboard, beginTime2, (double)DataPointHelper.GetDataPointValueFromProperty(list4[j], VcProperties.Opacity) * list4[j].Parent.Opacity);
									}
								}
								list4[j].DpInfo.VisualPosition = new Point(num18, num20);
							}
							if (list3[j].Parent.Faces == null)
							{
								list3[j].Parent.Faces = new Faces();
							}
							if (list3[j].DpInfo.Faces == null)
							{
								list3[j].DpInfo.Faces = new Faces();
							}
							List<PointCollection> list6;
							if (!double.IsNaN(intersection.X) && !double.IsInfinity(intersection.X) && intersection.X >= num17 && intersection.X <= num18)
							{
								List<PointCollection> list5 = AreaChart.GeneratePointsCollection(num17, num19, num21, intersection.X, intersection.Y, intersection.Y, num4);
								List<PointCollection> collection = AreaChart.GeneratePointsCollection(intersection.X, intersection.Y, intersection.Y, num18, num20, num22, num4);
								list6 = list5;
								list6.InsertRange(list6.Count, collection);
							}
							else
							{
								list6 = AreaChart.GeneratePointsCollection(num17, num19, num21, num18, num20, num22, num4);
							}
							DataSeries parent = list3[j].Parent;
							Brush color = parent.Color;
							dataSeries = parent;
							PolygonalChartShapeParams areaParms = AreaChart.GetAreaParms(parent, color, num);
							Faces faces = list3[j].Parent.Faces;
							if (faces.Parts == null)
							{
								faces.Parts = new List<DependencyObject>();
							}
							double num23 = 0.8;
							foreach (PointCollection current2 in list6)
							{
								current2[current2.Count - 2] = new Point(current2[current2.Count - 2].X + num23, current2[current2.Count - 2].Y);
								current2[current2.Count - 1] = new Point(current2[current2.Count - 1].X + num23, current2[current2.Count - 1].Y);
								areaParms.Points = current2;
								if (chart.View3D)
								{
									Point centroid = AreaChart.GetCentroid(current2);
									areaParms.IsPositive = (centroid.Y < num4);
									Canvas stacked3DAreaFrontFace = AreaChart.GetStacked3DAreaFrontFace(ref faces, areaParms);
									Canvas stacked3DSideFaces = AreaChart.GetStacked3DSideFaces(ref faces, areaParms);
									stacked3DSideFaces.SetValue(Panel.ZIndexProperty, AreaChart.GetStackedAreaZIndex(centroid.X, centroid.Y, areaParms.IsPositive, j));
									stacked3DAreaFrontFace.SetValue(Panel.ZIndexProperty, 50000);
									canvas3.Children.Add(stacked3DSideFaces);
									canvas3.Children.Add(stacked3DAreaFrontFace);
									list3[j].Parent.Faces.VisualComponents.Add(stacked3DSideFaces);
									list3[j].Parent.Faces.VisualComponents.Add(stacked3DAreaFrontFace);
									list3[j].DpInfo.Faces.VisualComponents.Add(stacked3DSideFaces);
									list3[j].DpInfo.Faces.VisualComponents.Add(stacked3DAreaFrontFace);
									if (animationEnabled)
									{
										if (list3[j].Parent.Storyboard == null)
										{
											list3[j].Parent.Storyboard = new Storyboard();
										}
										Storyboard storyboard = list3[j].Parent.Storyboard;
										double num24 = 0.0;
										storyboard = AreaChart.ApplyStackedAreaAnimation(dataSeries, stacked3DSideFaces, storyboard, 1.0 / (double)seriesList.Count * (double)seriesList.IndexOf(list3[j].Parent) + 0.05 + num24, 1.0 / (double)seriesList.Count);
										storyboard = AreaChart.ApplyStackedAreaAnimation(dataSeries, stacked3DAreaFrontFace, storyboard, 1.0 / (double)seriesList.Count * (double)seriesList.IndexOf(list3[j].Parent) + num24, 1.0 / (double)seriesList.Count);
									}
								}
								else
								{
									Canvas canvas4 = AreaChart.Get2DArea(dataSeries, ref faces, areaParms);
									canvas3.Children.Add(canvas4);
									list3[j].Parent.Faces.VisualComponents.Add(canvas4);
									list3[j].DpInfo.Faces.VisualComponents.Add(canvas4);
									if (animationEnabled)
									{
										if (list3[j].Parent.Storyboard == null)
										{
											list3[j].Parent.Storyboard = new Storyboard();
										}
										Storyboard storyboard2 = list3[j].Parent.Storyboard;
										double num25 = 0.0;
										storyboard2 = AreaChart.ApplyStackedAreaAnimation(dataSeries, canvas4, storyboard2, 1.0 / (double)seriesList.Count * (double)seriesList.IndexOf(list3[j].Parent) + num25, 1.0 / (double)seriesList.Count);
									}
								}
								list3[j].Parent.Faces.Visual = canvas;
								if (VisifireControl.IsMediaEffectsEnabled)
								{
									if (parent.Faces != null && parent.Faces.Visual != null && parent.Effect != null)
									{
										parent.Faces.Visual.Effect = parent.Effect;
									}
									if (parent.ShadowEnabled.Value && parent.Faces != null && parent.Faces.Visual != null && parent.Effect == null)
									{
										parent.Faces.Visual.Effect = ExtendedGraphics.GetShadowEffect(135.0, 2.0, 1.0);
									}
								}
							}
							num5 += num13;
							num6 += num14;
						}
					}
				}
			}
			if (array.Any<double>() && !flag && chart.View3D && plotGroup.AxisY.InternalAxisMinimum < 0.0 && plotGroup.AxisY.InternalAxisMaximum > 0.0)
			{
				Brush frontBrush;
				Brush topBrush;
				Brush rightBrush;
				ExtendedGraphics.GetBrushesForPlank(chart, out frontBrush, out topBrush, out rightBrush, true);
				Faces faces2 = ColumnChart.Get3DPlank(width, 1.0, num, frontBrush, topBrush, rightBrush);
				Panel panel = faces2.Visual as Panel;
				double num26 = height - Graphics.ValueToPixelPosition(0.0, height, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, 0.0);
				panel.SetValue(Canvas.LeftProperty, 0.0);
				panel.SetValue(Canvas.TopProperty, num26);
				panel.SetValue(Panel.ZIndexProperty, 0);
				panel.Opacity = 0.7;
				canvas.Children.Add(panel);
			}
			chart.ChartArea.DisableIndicators();
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
			PlotArea plotArea = chart.PlotArea;
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			rectangleGeometry.Rect = new Rect(0.0, plotArea.BorderThickness.Top - num, width + num, height + num - plotArea.BorderThickness.Bottom - plotArea.BorderThickness.Top);
			canvas3.Clip = rectangleGeometry;
			rectangleGeometry = new RectangleGeometry();
			double x = 0.0;
			double y = -num - 4.0;
			double width2 = width + num;
			double height2 = height + num + chart.ChartArea.PLANK_THICKNESS + 10.0;
			AreaChart.GetClipCoordinates(chart, ref x, ref y, ref width2, ref height2, minimumX, maximumX);
			rectangleGeometry.Rect = new Rect(x, y, width2, height2);
			canvas2.Clip = rectangleGeometry;
			return canvas;
		}

		internal static Point GetCentroid(PointCollection points)
		{
			double num = 0.0;
			double num2 = 0.0;
			foreach (Point current in points)
			{
				num += current.X;
				num2 += current.Y;
			}
			return new Point(num / (double)points.Count, num2 / (double)points.Count);
		}

		internal static List<PointCollection> GeneratePointsCollection(double curX, double curY, double curBase, double nextX, double nextY, double nextBase, double limitingY)
		{
			List<PointCollection> list = new List<PointCollection>();
			if (curY < limitingY && nextY < limitingY && curBase > limitingY && nextBase < limitingY)
			{
				PointCollection pointCollection = new PointCollection();
				pointCollection.Add(new Point(curX, limitingY));
				pointCollection.Add(new Point(curX, curY));
				pointCollection.Add(new Point(nextX, nextY));
				pointCollection.Add(new Point(nextX, nextBase));
				double x = Graphics.ConvertScale(curBase, nextBase, limitingY, curX, nextX);
				pointCollection.Add(new Point(x, limitingY));
				list.Add(pointCollection);
				list.Add(new PointCollection
				{
					new Point(curX, limitingY),
					new Point(x, limitingY),
					new Point(curX, curBase)
				});
			}
			else if (curY < limitingY && nextY > limitingY && curBase > limitingY && nextBase > limitingY)
			{
				PointCollection pointCollection = new PointCollection();
				pointCollection.Add(new Point(curX, limitingY));
				pointCollection.Add(new Point(curX, curY));
				double x2 = Graphics.ConvertScale(curY, nextY, limitingY, curX, nextX);
				pointCollection.Add(new Point(x2, limitingY));
				list.Add(pointCollection);
				list.Add(new PointCollection
				{
					new Point(curX, curBase),
					new Point(curX, limitingY),
					new Point(x2, limitingY),
					new Point(nextX, nextY),
					new Point(nextX, nextBase)
				});
			}
			else if (curY < limitingY && nextY < limitingY && curBase < limitingY && nextBase > limitingY)
			{
				PointCollection pointCollection = new PointCollection();
				double x3 = Graphics.ConvertScale(curBase, nextBase, limitingY, curX, nextX);
				pointCollection.Add(new Point(x3, limitingY));
				pointCollection.Add(new Point(curX, curBase));
				pointCollection.Add(new Point(curX, curY));
				pointCollection.Add(new Point(nextX, nextY));
				pointCollection.Add(new Point(nextX, limitingY));
				list.Add(pointCollection);
				list.Add(new PointCollection
				{
					new Point(x3, limitingY),
					new Point(nextX, limitingY),
					new Point(nextX, nextBase)
				});
			}
			else if (curY > limitingY && nextY < limitingY && curBase > limitingY && nextBase > limitingY)
			{
				PointCollection pointCollection = new PointCollection();
				double x4 = Graphics.ConvertScale(curY, nextY, limitingY, curX, nextX);
				pointCollection.Add(new Point(x4, limitingY));
				pointCollection.Add(new Point(nextX, nextY));
				pointCollection.Add(new Point(nextX, limitingY));
				list.Add(pointCollection);
				list.Add(new PointCollection
				{
					new Point(curX, curBase),
					new Point(curX, curY),
					new Point(x4, limitingY),
					new Point(nextX, limitingY),
					new Point(nextX, nextY)
				});
			}
			else if ((curY < limitingY && nextY < limitingY && curBase > limitingY && nextBase > limitingY) || (curY > limitingY && nextY > limitingY && curBase < limitingY && nextBase < limitingY))
			{
				list.Add(new PointCollection
				{
					new Point(curX, limitingY),
					new Point(curX, curY),
					new Point(nextX, nextY),
					new Point(nextX, limitingY)
				});
				list.Add(new PointCollection
				{
					new Point(curX, curBase),
					new Point(curX, limitingY),
					new Point(nextX, limitingY),
					new Point(nextX, nextBase)
				});
			}
			else if (curY < limitingY && nextY > limitingY && curBase < limitingY && nextBase > limitingY)
			{
				PointCollection pointCollection = new PointCollection();
				pointCollection.Add(new Point(curX, curBase));
				pointCollection.Add(new Point(curX, curY));
				double x5 = Graphics.ConvertScale(curY, nextY, limitingY, curX, nextX);
				pointCollection.Add(new Point(x5, limitingY));
				double x6 = Graphics.ConvertScale(curBase, nextBase, limitingY, curX, nextX);
				pointCollection.Add(new Point(x6, limitingY));
				list.Add(pointCollection);
				list.Add(new PointCollection
				{
					new Point(x6, limitingY),
					new Point(x5, limitingY),
					new Point(nextX, nextY),
					new Point(nextX, nextBase)
				});
			}
			else if (curY > limitingY && nextY < limitingY && curBase > limitingY && nextBase < limitingY)
			{
				PointCollection pointCollection = new PointCollection();
				pointCollection.Add(new Point(curX, curBase));
				pointCollection.Add(new Point(curX, curY));
				double x7 = Graphics.ConvertScale(curY, nextY, limitingY, curX, nextX);
				pointCollection.Add(new Point(x7, limitingY));
				double x8 = Graphics.ConvertScale(curBase, nextBase, limitingY, curX, nextX);
				pointCollection.Add(new Point(x8, limitingY));
				list.Add(pointCollection);
				list.Add(new PointCollection
				{
					new Point(x8, limitingY),
					new Point(x7, limitingY),
					new Point(nextX, nextY),
					new Point(nextX, nextBase)
				});
			}
			else if (curY > limitingY && nextY < limitingY && curBase < limitingY && nextBase < limitingY)
			{
				PointCollection pointCollection = new PointCollection();
				double x9 = Graphics.ConvertScale(curY, nextY, limitingY, curX, nextX);
				pointCollection.Add(new Point(x9, limitingY));
				pointCollection.Add(new Point(curX, limitingY));
				pointCollection.Add(new Point(curX, curBase));
				pointCollection.Add(new Point(nextX, nextBase));
				pointCollection.Add(new Point(nextX, nextY));
				list.Add(pointCollection);
				list.Add(new PointCollection
				{
					new Point(curX, curY),
					new Point(curX, limitingY),
					new Point(x9, limitingY)
				});
			}
			else if (curY > limitingY && nextY > limitingY && curBase < limitingY && nextBase > limitingY)
			{
				PointCollection pointCollection = new PointCollection();
				pointCollection.Add(new Point(curX, limitingY));
				pointCollection.Add(new Point(curX, curBase));
				double x10 = Graphics.ConvertScale(curBase, nextBase, limitingY, curX, nextX);
				pointCollection.Add(new Point(x10, limitingY));
				list.Add(pointCollection);
				list.Add(new PointCollection
				{
					new Point(curX, curY),
					new Point(curX, limitingY),
					new Point(x10, limitingY),
					new Point(nextX, nextBase),
					new Point(nextX, nextY)
				});
			}
			else if (curY < limitingY && nextY > limitingY && curBase < limitingY && nextBase < limitingY)
			{
				PointCollection pointCollection = new PointCollection();
				double x11 = Graphics.ConvertScale(curY, nextY, limitingY, curX, nextX);
				pointCollection.Add(new Point(x11, limitingY));
				pointCollection.Add(new Point(curX, curY));
				pointCollection.Add(new Point(curX, curBase));
				pointCollection.Add(new Point(nextX, nextBase));
				pointCollection.Add(new Point(nextX, limitingY));
				list.Add(pointCollection);
				list.Add(new PointCollection
				{
					new Point(x11, limitingY),
					new Point(nextX, limitingY),
					new Point(nextX, nextY)
				});
			}
			else if (curY > limitingY && nextY > limitingY && curBase > limitingY && nextBase < limitingY)
			{
				PointCollection pointCollection = new PointCollection();
				double x12 = Graphics.ConvertScale(curBase, nextBase, limitingY, curX, nextX);
				pointCollection.Add(new Point(curX, curY));
				pointCollection.Add(new Point(curX, curBase));
				pointCollection.Add(new Point(x12, limitingY));
				pointCollection.Add(new Point(nextX, limitingY));
				pointCollection.Add(new Point(nextX, nextBase));
				list.Add(pointCollection);
				list.Add(new PointCollection
				{
					new Point(x12, limitingY),
					new Point(nextX, nextY),
					new Point(nextX, limitingY)
				});
			}
			else if (curY > curBase && nextY > nextBase)
			{
				list.Add(new PointCollection
				{
					new Point(curX, curY),
					new Point(curX, curBase),
					new Point(nextX, nextBase),
					new Point(nextX, nextY)
				});
			}
			else
			{
				list.Add(new PointCollection
				{
					new Point(curX, curBase),
					new Point(curX, curY),
					new Point(nextX, nextY),
					new Point(nextX, nextBase)
				});
			}
			return list;
		}

		internal static Point GetCrossingPointWithSmallestX(double curX, List<double> curYValues, double nextX, List<double> nextYValues, double curBase, double nextBase, int startIndex)
		{
			Point result = new Point(1.7976931348623157E+308, 1.7976931348623157E+308);
			for (int i = startIndex; i < curYValues.Count; i++)
			{
				Point intersection = AreaChart.GetIntersection(new Point(curX, curBase), new Point(nextX, nextBase), new Point(curX, curYValues[i]), new Point(nextX, nextYValues[i]));
				if (intersection.X < result.X)
				{
					result = intersection;
				}
			}
			if (result.X == 1.7976931348623157E+308)
			{
				return new Point(double.NaN, double.NaN);
			}
			return result;
		}

		internal static double GetSlope(double x1, double y1, double x2, double y2)
		{
			return (y2 - y1) / (x2 - x1);
		}

		internal static double GetIntercept(double x1, double y1, double x2, double y2)
		{
			return y1 - x1 * AreaChart.GetSlope(x1, y1, x2, y2);
		}

		internal static Point GetIntersection(Point Line1Start, Point Line1End, Point Line2Start, Point Line2End)
		{
			double slope = AreaChart.GetSlope(Line1Start.X, Line1Start.Y, Line1End.X, Line1End.Y);
			double slope2 = AreaChart.GetSlope(Line2Start.X, Line2Start.Y, Line2End.X, Line2End.Y);
			double intercept = AreaChart.GetIntercept(Line1Start.X, Line1Start.Y, Line1End.X, Line1End.Y);
			double intercept2 = AreaChart.GetIntercept(Line2Start.X, Line2Start.Y, Line2End.X, Line2End.Y);
			double y = (slope * intercept2 - slope2 * intercept) / (slope - slope2);
			double x = (intercept2 - intercept) / (slope - slope2);
			return new Point(x, y);
		}

		internal static Rect GetBounds(params Point[] points)
		{
			double num = 1.7976931348623157E+308;
			double num2 = 1.7976931348623157E+308;
			double num3 = -1.7976931348623157E+308;
			double num4 = -1.7976931348623157E+308;
			for (int i = 0; i < points.Length; i++)
			{
				Point point = points[i];
				num = Math.Min(num, point.X);
				num2 = Math.Min(num2, point.Y);
				num3 = Math.Max(num3, point.X);
				num4 = Math.Max(num4, point.Y);
			}
			return new Rect(num, num2, Math.Abs(num3 - num), Math.Abs(num4 - num2));
		}

		internal static Rect GetBounds(PointCollection points)
		{
			double num = 1.7976931348623157E+308;
			double num2 = 1.7976931348623157E+308;
			double num3 = -1.7976931348623157E+308;
			double num4 = -1.7976931348623157E+308;
			foreach (Point current in points)
			{
				num = Math.Min(num, current.X);
				num2 = Math.Min(num2, current.Y);
				num3 = Math.Max(num3, current.X);
				num4 = Math.Max(num4, current.Y);
			}
			return new Rect(num, num2, Math.Abs(num3 - num), Math.Abs(num4 - num2));
		}

		internal static Canvas Get2DArea(DataSeries currentDataSeries, ref Faces faces, PolygonalChartShapeParams areaParams)
		{
			if (faces.Parts == null)
			{
				faces.Parts = new List<DependencyObject>();
			}
			Canvas canvas = new Canvas();
			canvas.Width = areaParams.Size.Width;
			canvas.Height = areaParams.Size.Height;
			Polygon polygon = new Polygon
			{
				Tag = new ElementData
				{
					Element = areaParams.TagReference,
					VisualElementName = "AreaBase"
				}
			};
			faces.Parts.Add(polygon);
			polygon.Fill = (areaParams.Lighting ? Graphics.GetLightingEnabledBrush(areaParams.Background, "Linear", null) : areaParams.Background);
			polygon.Stroke = areaParams.BorderColor;
			polygon.StrokeDashArray = ((areaParams.BorderStyle != null) ? ExtendedGraphics.CloneCollection(areaParams.BorderStyle) : areaParams.BorderStyle);
			polygon.StrokeThickness = areaParams.BorderThickness;
			polygon.StrokeMiterLimit = 1.0;
			polygon.Points = areaParams.Points;
			Rect bounds = AreaChart.GetBounds(areaParams.Points);
			polygon.Stretch = Stretch.Fill;
			polygon.Width = bounds.Width;
			polygon.Height = bounds.Height;
			polygon.SetValue(Canvas.TopProperty, bounds.Y);
			polygon.SetValue(Canvas.LeftProperty, bounds.X);
			if (areaParams.AnimationEnabled)
			{
				areaParams.Storyboard = AreaChart.ApplyAreaAnimation(currentDataSeries, polygon, areaParams.Storyboard, areaParams.IsPositive, 0.0);
			}
			canvas.Children.Add(polygon);
			if (areaParams.Bevel)
			{
				for (int i = 0; i < areaParams.Points.Count - 1; i++)
				{
					if (areaParams.Points[i].X != areaParams.Points[i + 1].X)
					{
						double slope = AreaChart.GetSlope(areaParams.Points[i].X, areaParams.Points[i].Y, areaParams.Points[i + 1].X, areaParams.Points[i + 1].Y);
						double num = AreaChart.GetIntercept(areaParams.Points[i].X, areaParams.Points[i].Y, areaParams.Points[i + 1].X, areaParams.Points[i + 1].Y);
						num += (double)((areaParams.IsPositive ? 1 : -1) * 4);
						Point value = new Point(areaParams.Points[i].X, slope * areaParams.Points[i].X + num);
						Point value2 = new Point(areaParams.Points[i + 1].X, slope * areaParams.Points[i + 1].X + num);
						PointCollection pointCollection = new PointCollection();
						pointCollection.Add(areaParams.Points[i]);
						pointCollection.Add(areaParams.Points[i + 1]);
						pointCollection.Add(value2);
						pointCollection.Add(value);
						Polygon polygon2 = new Polygon
						{
							Tag = new ElementData
							{
								Element = areaParams.TagReference,
								VisualElementName = "Bevel"
							}
						};
						polygon2.Points = pointCollection;
						polygon2.Fill = Graphics.GetBevelTopBrush(areaParams.Background);
						if (areaParams.AnimationEnabled)
						{
							areaParams.Storyboard = AreaChart.CreateOpacityAnimation(currentDataSeries, areaParams.Storyboard, polygon2, 1.0, 1.0, 1.0);
							polygon2.Opacity = 0.0;
						}
						faces.Parts.Add(polygon2);
						canvas.Children.Add(polygon2);
					}
				}
			}
			return canvas;
		}

		internal static int Draw3DArea(Canvas parentVisual, IDataPoint previusDataPoint, IDataPoint dataPoint, IDataPoint nextDataPoint, ref Faces dataSeriesFaces, ref Faces dataPointFaces, DataSeries dataSeries, double plankYPos)
		{
			Brush color = dataSeries.Color;
			bool value = dataSeries.LightingEnabled.Value;
			Brush fill = value ? Graphics.GetRightFaceBrush(dataSeries.Color) : color;
			Brush fill2 = value ? Graphics.GetTopFaceBrush(dataSeries.Color) : color;
			new Random(DateTime.Now.Millisecond);
			bool flag = false;
			int areaZIndex = AreaChart.GetAreaZIndex(dataPoint.DpInfo.VisualPosition.X, dataPoint.DpInfo.VisualPosition.Y, dataPoint.DpInfo.InternalYValue > 0.0 || flag);
			if (dataPointFaces.Area3DLeftFace != null)
			{
				Area3DDataPointFace area3DLeftFace = dataPointFaces.Area3DLeftFace;
				area3DLeftFace.CalculateBackFacePoints();
				Path path = new Path
				{
					Tag = new ElementData
					{
						Element = dataSeries
					}
				};
				path.SetValue(Panel.ZIndexProperty, areaZIndex);
				PathGeometry pathGeometry = new PathGeometry();
				PathFigure pathFigure = new PathFigure
				{
					IsClosed = true
				};
				pathGeometry.Figures.Add(pathFigure);
				PointCollection facePoints = area3DLeftFace.GetFacePoints();
				pathFigure.StartPoint = area3DLeftFace.FrontFacePoints[0];
				foreach (Point current in facePoints)
				{
					LineSegment value2 = new LineSegment
					{
						Point = current
					};
					pathFigure.Segments.Add(value2);
				}
				path.Data = pathGeometry;
				path.Fill = fill;
				path.Opacity = dataPoint.Parent.Opacity;
				AreaChart.ApplyBorderProperties(path, dataPoint.Parent);
				parentVisual.Children.Add(path);
				area3DLeftFace.LeftFace = path;
				dataSeriesFaces.VisualComponents.Add(path);
			}
			if (dataPointFaces.Area3DLeftTopFace != null)
			{
				Area3DDataPointFace area3DLeftTopFace = dataPointFaces.Area3DLeftTopFace;
				if (flag)
				{
					Point value3 = default(Point);
					if (dataPointFaces.Area3DRightFace == null && Graphics.IntersectionOfTwoStraightLines(new Point(previusDataPoint.DpInfo.VisualPosition.X, plankYPos), new Point(dataPoint.DpInfo.VisualPosition.X, plankYPos), previusDataPoint.DpInfo.VisualPosition, dataPoint.DpInfo.VisualPosition, ref value3))
					{
						area3DLeftTopFace.FrontFacePoints[1] = value3;
					}
				}
				area3DLeftTopFace.CalculateBackFacePoints();
				Path path2 = new Path
				{
					Tag = new ElementData
					{
						Element = dataSeries
					}
				};
				path2.SetValue(Panel.ZIndexProperty, areaZIndex);
				PathGeometry pathGeometry2 = new PathGeometry();
				PathFigure pathFigure2 = new PathFigure
				{
					IsClosed = true
				};
				pathGeometry2.Figures.Add(pathFigure2);
				pathFigure2.StartPoint = area3DLeftTopFace.FrontFacePoints[0];
				PointCollection facePoints2 = area3DLeftTopFace.GetFacePoints();
				foreach (Point current2 in facePoints2)
				{
					LineSegment value4 = new LineSegment
					{
						Point = current2
					};
					pathFigure2.Segments.Add(value4);
				}
				path2.Data = pathGeometry2;
				path2.Fill = fill2;
				path2.Opacity = dataPoint.Parent.Opacity;
				AreaChart.ApplyBorderProperties(path2, dataPoint.Parent);
				parentVisual.Children.Add(path2);
				area3DLeftTopFace.TopFace = path2;
				dataSeriesFaces.VisualComponents.Add(path2);
			}
			if (dataPointFaces.Area3DRightFace != null)
			{
				Area3DDataPointFace area3DRightFace = dataPointFaces.Area3DRightFace;
				area3DRightFace.CalculateBackFacePoints();
				Path path3 = new Path
				{
					Tag = new ElementData
					{
						Element = dataSeries
					}
				};
				path3.SetValue(Panel.ZIndexProperty, areaZIndex);
				PathGeometry pathGeometry3 = new PathGeometry();
				PathFigure pathFigure3 = new PathFigure
				{
					IsClosed = true
				};
				pathGeometry3.Figures.Add(pathFigure3);
				PointCollection facePoints3 = area3DRightFace.GetFacePoints();
				pathFigure3.StartPoint = area3DRightFace.FrontFacePoints[0];
				foreach (Point current3 in facePoints3)
				{
					LineSegment value5 = new LineSegment
					{
						Point = current3
					};
					pathFigure3.Segments.Add(value5);
				}
				path3.Data = pathGeometry3;
				path3.Fill = fill;
				path3.Opacity = dataPoint.Parent.Opacity;
				AreaChart.ApplyBorderProperties(path3, dataPoint.Parent);
				parentVisual.Children.Add(path3);
				area3DRightFace.RightFace = path3;
				AreaChart.ApplyBorderProperties(path3, dataSeries);
				dataSeriesFaces.VisualComponents.Add(path3);
			}
			return areaZIndex;
		}

		internal static void ApplyBorderProperties(Path path, DataSeries dataSeries)
		{
			DoubleCollection strokeDashArray = null;
			if (dataSeries.BorderStyle != BorderStyles.Solid)
			{
				strokeDashArray = ExtendedGraphics.GetDashArray(dataSeries.BorderStyle);
			}
			path.StrokeDashArray = strokeDashArray;
			double left = dataSeries.BorderThickness.Left;
			path.StrokeThickness = left;
			if (left != 0.0)
			{
				path.Stroke = dataSeries.BorderColor;
			}
			path.StrokeMiterLimit = 1.0;
		}

		internal static void Update(IElement sender, ElementTypes elementType, VcProperties property, object newValue, bool isAxisChanged)
		{
			bool flag;
			if (property == VcProperties.Bevel)
			{
				sender = (sender as IDataPoint).Parent;
				flag = false;
			}
			else
			{
				flag = (elementType == ElementTypes.DataPoint);
			}
			if (flag)
			{
				AreaChart.UpdateDataPoint(sender as IDataPoint, property, newValue, isAxisChanged);
				return;
			}
			AreaChart.UpdateDataSeries(sender as DataSeries, property, newValue);
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
			PlotGroup arg_30_0 = parent.PlotGroup;
			if (parent.Faces == null)
			{
				return;
			}
			Canvas canvas = parent.Faces.Visual as Canvas;
			if (canvas == null)
			{
				return;
			}
			Canvas canvas2 = ((canvas.Parent as Canvas).Parent as Canvas).Children[0] as Canvas;
			double height = chart.ChartArea.ChartVisualCanvas.Height;
			double width = chart.ChartArea.ChartVisualCanvas.Width;
			Faces faces = dataPoint.DpInfo.Faces;
			if (faces == null && property != VcProperties.Enabled && property != VcProperties.YValue)
			{
				return;
			}
			if (property > VcProperties.LabelFontWeight)
			{
				double num;
				double num2;
				if (property <= VcProperties.Opacity)
				{
					switch (property)
					{
					case VcProperties.LabelStyle:
						LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
						return;
					case VcProperties.LabelText:
						LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
						return;
					case VcProperties.LabelAngle:
						LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
						return;
					case VcProperties.Legend:
					case VcProperties.LegendMarkerColor:
					case VcProperties.LegendMarkerType:
						return;
					case VcProperties.LegendText:
						chart.InvokeRender();
						return;
					case VcProperties.LightingEnabled:
						goto IL_22C;
					default:
						switch (property)
						{
						case VcProperties.MarkerBorderColor:
							LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
							return;
						case VcProperties.MarkerBorderThickness:
							LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
							return;
						case VcProperties.MarkerColor:
							if (marker != null && (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
							{
								marker.MarkerFillColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerColor);
								return;
							}
							return;
						case VcProperties.MarkerEnabled:
							LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
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
						{
							double opacity = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Opacity) * parent.Opacity;
							if (marker != null)
							{
								marker.Visual.Opacity = opacity;
							}
							if (faces == null)
							{
								return;
							}
							if (dataPoint.DpInfo.Faces.BevelLine != null)
							{
								dataPoint.DpInfo.Faces.BevelLine.Opacity = opacity;
							}
							if (dataPoint.DpInfo.Faces.Area3DLeftFace != null)
							{
								(dataPoint.DpInfo.Faces.Area3DLeftFace.LeftFace as Path).Opacity = opacity;
							}
							if (dataPoint.DpInfo.Faces.Area3DRightTopFace != null)
							{
								(dataPoint.DpInfo.Faces.Area3DRightTopFace.TopFace as Path).Opacity = opacity;
							}
							if (dataPoint.DpInfo.Faces.Area3DLeftTopFace != null)
							{
								(dataPoint.DpInfo.Faces.Area3DLeftTopFace.TopFace as Path).Opacity = opacity;
							}
							if (dataPoint.DpInfo.Faces.Area3DRightFace != null)
							{
								(dataPoint.DpInfo.Faces.Area3DRightFace.RightFace as Path).Opacity = opacity;
								return;
							}
							return;
						}
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
							LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
							return;
						case VcProperties.To:
						case VcProperties.TrendLines:
						case VcProperties.Value:
						case VcProperties.ValueFormatString:
						case VcProperties.VerticalAlignment:
						case VcProperties.XValues:
							return;
						case VcProperties.XValue:
							AreaChart.UpdateDataSeries(parent, property, newValue);
							return;
						case VcProperties.XValueType:
							chart.InvokeRender();
							return;
						case VcProperties.YValue:
						case VcProperties.YValues:
							if (isAxisChanged || double.IsNaN(dataPoint.DpInfo.InternalYValue) || (dataPoint.DpInfo.OldYValue >= 0.0 && dataPoint.DpInfo.InternalYValue < 0.0) || (dataPoint.DpInfo.OldYValue <= 0.0 && dataPoint.DpInfo.InternalYValue > 0.0))
							{
								AreaChart.UpdateDataSeries(parent, property, newValue);
							}
							else
							{
								dataPoint.DpInfo.ParsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
								if (!dataPoint.IsLightDataPoint)
								{
									(dataPoint as DataPoint).ParsedHref = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Href));
								}
								AreaChart.UpdateVisualForYValue4AreaChart(chart, dataPoint, isAxisChanged);
							}
							chart._toolTip.Hide();
							return;
						default:
							return;
						}
						break;
					}
				}
				LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
				return;
			}
			switch (property)
			{
			case VcProperties.Bevel:
				AreaChart.UpdateDataSeries(parent, property, newValue);
				return;
			case VcProperties.BorderColor:
			case VcProperties.BorderStyle:
			case VcProperties.BorderThickness:
				if (faces == null)
				{
					return;
				}
				if (dataPoint.DpInfo.Faces.Area3DLeftFace != null)
				{
					AreaChart.ApplyBorderProperties(dataPoint.DpInfo.Faces.Area3DLeftFace.LeftFace as Path, parent);
				}
				if (dataPoint.DpInfo.Faces.Area3DRightTopFace != null)
				{
					AreaChart.ApplyBorderProperties(dataPoint.DpInfo.Faces.Area3DRightTopFace.TopFace as Path, parent);
				}
				if (dataPoint.DpInfo.Faces.Area3DLeftTopFace != null)
				{
					AreaChart.ApplyBorderProperties(dataPoint.DpInfo.Faces.Area3DLeftTopFace.TopFace as Path, parent);
				}
				if (dataPoint.DpInfo.Faces.Area3DRightFace != null)
				{
					AreaChart.ApplyBorderProperties(dataPoint.DpInfo.Faces.Area3DRightFace.RightFace as Path, parent);
					return;
				}
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
					if (marker != null && VisifireControl.IsMediaEffectsEnabled)
					{
						marker.Effect = (Effect)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.Effect);
						marker.ApplyEffects();
						return;
					}
					return;
				case VcProperties.Enabled:
					AreaChart.UpdateDataSeries(parent, property, newValue);
					return;
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
						return;
					case VcProperties.LabelBackground:
					{
						double num;
						double num2;
						LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
						marker.TextBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
						return;
					}
					case VcProperties.LabelEnabled:
					{
						double num;
						double num2;
						LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
						return;
					}
					case VcProperties.LabelFontColor:
						marker.FontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
						return;
					case VcProperties.LabelFontFamily:
					{
						double num;
						double num2;
						LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
						return;
					}
					case VcProperties.LabelFontSize:
					{
						double num;
						double num2;
						LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
						return;
					}
					case VcProperties.LabelFontStyle:
					{
						double num;
						double num2;
						LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref canvas2, out num, out num2);
						return;
					}
					case VcProperties.LabelFontWeight:
						marker.FontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
						return;
					default:
						return;
					}
					break;
				}
				break;
			}
			IL_22C:
			if (property != VcProperties.LightingEnabled && marker != null && (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
			{
				marker.BorderColor = ((DataPointHelper.GetMarkerBorderColor_DataPoint(dataPoint) == null) ? ((newValue != null) ? (newValue as Brush) : ((Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor))) : ((Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor)));
			}
			if (faces != null)
			{
				if (dataPoint.DpInfo.Faces.BevelLine != null)
				{
					dataPoint.DpInfo.Faces.BevelLine.Stroke = Graphics.GetBevelTopBrush(dataPoint.Parent.Color);
				}
				Brush fill = parent.LightingEnabled.Value ? Graphics.GetRightFaceBrush(parent.Color) : parent.Color;
				Brush fill2 = parent.LightingEnabled.Value ? Graphics.GetTopFaceBrush(parent.Color) : parent.Color;
				if (dataPoint.DpInfo.Faces.Area3DLeftFace != null)
				{
					(dataPoint.DpInfo.Faces.Area3DLeftFace.LeftFace as Path).Fill = fill;
				}
				if (dataPoint.DpInfo.Faces.Area3DRightTopFace != null)
				{
					(dataPoint.DpInfo.Faces.Area3DRightTopFace.TopFace as Path).Fill = fill2;
				}
				if (dataPoint.DpInfo.Faces.Area3DLeftTopFace != null)
				{
					(dataPoint.DpInfo.Faces.Area3DLeftTopFace.TopFace as Path).Fill = fill2;
				}
				if (dataPoint.DpInfo.Faces.Area3DRightFace != null)
				{
					(dataPoint.DpInfo.Faces.Area3DRightFace.RightFace as Path).Fill = fill;
					return;
				}
			}
		}

		public static void UpdateVisualForYValue4AreaChart(Chart chart, IDataPoint dataPoint, bool isAxisChanged)
		{
			if (dataPoint.DpInfo.Faces == null)
			{
				return;
			}
			DataSeries parent = dataPoint.Parent;
			Canvas canvas = parent.Faces.Visual as Canvas;
			double arg_31_0 = dataPoint.DpInfo.InternalYValue;
			double num = chart.ChartArea.PLANK_DEPTH / (double)chart.PlotDetails.Layer3DCount * (double)(chart.View3D ? 1 : 0);
			PlotGroup plotGroup = dataPoint.Parent.PlotGroup;
			Storyboard storyboard = null;
			bool value = chart.AnimatedUpdate.Value;
			Canvas labelCanvas = parent.Faces.LabelCanvas;
			double height = chart.ChartArea.ChartVisualCanvas.Height;
			double width = chart.ChartArea.ChartVisualCanvas.Width;
			RenderHelper.UpdateParentVisualCanvasSize(chart, labelCanvas);
			RenderHelper.UpdateParentVisualCanvasSize(chart, canvas);
			RenderHelper.UpdateParentVisualCanvasSize(chart, canvas.Children[0] as Canvas);
			if (dataPoint.DpInfo.Storyboard != null)
			{
				dataPoint.DpInfo.Storyboard.Stop();
				dataPoint.DpInfo.Storyboard.Children.Clear();
			}
			if (value)
			{
				if (dataPoint.DpInfo.Storyboard != null)
				{
					storyboard = dataPoint.DpInfo.Storyboard;
				}
				else
				{
					storyboard = new Storyboard();
				}
			}
			double num2 = Graphics.ValueToPixelPosition(0.0, width, plotGroup.AxisX.InternalAxisMinimum, plotGroup.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
			double num3 = Graphics.ValueToPixelPosition(height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, dataPoint.DpInfo.InternalYValue);
			Point oldVisualPositionOfDataPoint = dataPoint.DpInfo.VisualPosition;
			if (dataPoint.DpInfo.Marker != null && dataPoint.DpInfo.Marker.Visual != null)
			{
				if (storyboard != null)
				{
					storyboard.Pause();
				}
				double num4 = (double)dataPoint.DpInfo.Marker.Visual.GetValue(Canvas.TopProperty);
				double num5 = (double)dataPoint.DpInfo.Marker.Visual.GetValue(Canvas.LeftProperty);
				double num6;
				double num7;
				LineChart.CreateMarkerAForLineDataPoint(dataPoint, width, height, ref labelCanvas, out num6, out num7);
				Point point = dataPoint.DpInfo.Marker.CalculateActualPosition(num2, num3, new Point(0.5, 0.5));
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled) || (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled))
				{
					if (value)
					{
						AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Marker.Visual, "(Canvas.Top)", dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.0
						}, new double[]
						{
							num4,
							point.Y
						}, null);
						AnimationHelper.ApplyPropertyAnimation(dataPoint.DpInfo.Marker.Visual, "(Canvas.Left)", dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.0
						}, new double[]
						{
							num5,
							point.X
						}, null);
					}
					else
					{
						dataPoint.DpInfo.Marker.Visual.SetValue(Canvas.TopProperty, point.Y);
						dataPoint.DpInfo.Marker.Visual.SetValue(Canvas.LeftProperty, point.X);
					}
				}
			}
			dataPoint.DpInfo.VisualPosition = new Point(num2, num3);
			IDataPoint nextDataPoint = dataPoint.DpInfo.Faces.NextDataPoint;
			double num8 = Graphics.ValueToPixelPosition(canvas.Height, 0.0, plotGroup.AxisY.InternalAxisMinimum, plotGroup.AxisY.InternalAxisMaximum, plotGroup.GetLimitingYValue());
			if (dataPoint.DpInfo.Faces.Area3DLeftFace != null)
			{
				Area3DDataPointFace area3DLeftFace = dataPoint.DpInfo.Faces.Area3DLeftFace;
				Path path = area3DLeftFace.LeftFace as Path;
				(path.Data as PathGeometry).Figures[0].StartPoint = new Point(num2, num8);
				LineSegment lineSegment = Area3DDataPointFace.GetLineSegment(path, 0);
				Point point2 = new Point(num2, num3);
				Point point3 = lineSegment.Point;
				lineSegment.Point = point2;
				if (value)
				{
					AnimationHelper.ApplyPointAnimation(lineSegment, "Point", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new Point[]
					{
						point3,
						point2
					}, null, double.NaN);
				}
				lineSegment = Area3DDataPointFace.GetLineSegment(path, 1);
				point2 = new Point(num2 + area3DLeftFace.Depth3d, num3 - area3DLeftFace.Depth3d);
				point3 = lineSegment.Point;
				lineSegment.Point = point2;
				if (value)
				{
					AnimationHelper.ApplyPointAnimation(lineSegment, "Point", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new Point[]
					{
						point3,
						point2
					}, null, double.NaN);
				}
				lineSegment = Area3DDataPointFace.GetLineSegment(path, 2);
				point2 = new Point(num2 + area3DLeftFace.Depth3d, num8 - area3DLeftFace.Depth3d);
				point3 = lineSegment.Point;
				lineSegment.Point = point2;
			}
			if (dataPoint.DpInfo.Faces.Area3DRightTopFace != null)
			{
				Area3DDataPointFace area3DRightTopFace = dataPoint.DpInfo.Faces.Area3DRightTopFace;
				Path path2 = area3DRightTopFace.TopFace as Path;
				PathFigure pathFigure = Area3DDataPointFace.GetPathFigure(path2);
				Point point2 = new Point(num2, num3);
				Point point3 = pathFigure.StartPoint;
				pathFigure.StartPoint = point2;
				if (value)
				{
					AnimationHelper.ApplyPointAnimation(pathFigure, "StartPoint", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new Point[]
					{
						point3,
						point2
					}, null, double.NaN);
				}
				else
				{
					pathFigure.StartPoint = point2;
				}
				LineSegment lineSegment2 = Area3DDataPointFace.GetLineSegment(path2, 2);
				point2 = new Point(num2 + area3DRightTopFace.Depth3d, num3 - area3DRightTopFace.Depth3d);
				point3 = lineSegment2.Point;
				lineSegment2.Point = point2;
				if (value)
				{
					AnimationHelper.ApplyPointAnimation(lineSegment2, "Point", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new Point[]
					{
						point3,
						point2
					}, null, double.NaN);
				}
				else
				{
					lineSegment2.Point = point2;
				}
			}
			if (dataPoint.DpInfo.Faces.Area3DLeftTopFace != null)
			{
				IDataPoint previousDataPoint = dataPoint.DpInfo.Faces.PreviousDataPoint;
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				if (dataPoint.DpInfo.InternalYValue < 0.0 && previousDataPoint != null && previousDataPoint.DpInfo.InternalYValue > 0.0)
				{
					flag = true;
				}
				if (dataPoint.DpInfo.InternalYValue > 0.0 && nextDataPoint != null && nextDataPoint.DpInfo.InternalYValue < 0.0)
				{
					flag2 = true;
				}
				if (dataPoint.DpInfo.InternalYValue < 0.0 && previousDataPoint != null && nextDataPoint.DpInfo.InternalYValue < 0.0)
				{
					flag3 = true;
				}
				Area3DDataPointFace area3DLeftTopFace = dataPoint.DpInfo.Faces.Area3DLeftTopFace;
				Path path3 = area3DLeftTopFace.TopFace as Path;
				if (flag)
				{
					Point point4 = default(Point);
					Graphics.IntersectionOfTwoStraightLines(new Point(previousDataPoint.DpInfo.VisualPosition.X, num8), new Point(dataPoint.DpInfo.VisualPosition.X, num8), previousDataPoint.DpInfo.VisualPosition, dataPoint.DpInfo.VisualPosition, ref point4);
					LineSegment lineSegment3 = Area3DDataPointFace.GetLineSegment(path3, 0);
					Point point2 = new Point(point4.X, point4.Y);
					Point point3 = lineSegment3.Point;
					lineSegment3.Point = point2;
					LineSegment lineSegment4 = Area3DDataPointFace.GetLineSegment(path3, 1);
					Point point5 = new Point(point4.X + area3DLeftTopFace.Depth3d, point4.Y - area3DLeftTopFace.Depth3d);
					Point point6 = lineSegment4.Point;
					lineSegment4.Point = point5;
					double num9 = Math.Abs(dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point.Y - num8);
					double num10 = num9 / Math.Abs(dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point.Y - dataPoint.DpInfo.VisualPosition.Y);
					double num11 = Math.Abs(point2.X - dataPoint.DpInfo.VisualPosition.X);
					double num12 = num11 / Math.Abs(dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point.Y - dataPoint.DpInfo.VisualPosition.Y);
					if (value)
					{
						if (dataPoint.DpInfo.OldYValue > 0.0 && dataPoint.DpInfo.InternalYValue < 0.0)
						{
							AnimationHelper.ApplyPointAnimation(lineSegment3, "Point", dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								num10,
								num10 + num12
							}, new Point[]
							{
								point3,
								new Point(point3.X, num8),
								point2
							}, null, double.NaN);
							AnimationHelper.ApplyPointAnimation(lineSegment4, "Point", dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								num10,
								num10 + num12
							}, new Point[]
							{
								point6,
								new Point(point6.X, num8 - area3DLeftTopFace.Depth3d),
								point5
							}, null, double.NaN);
						}
						else
						{
							AnimationHelper.ApplyPointAnimation(lineSegment3, "Point", dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								1.0
							}, new Point[]
							{
								point3,
								point2
							}, null, double.NaN);
							AnimationHelper.ApplyPointAnimation(lineSegment4, "Point", dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								1.0
							}, new Point[]
							{
								point6,
								point5
							}, null, double.NaN);
						}
					}
				}
				else if (flag3)
				{
					Area3DDataPointFace area3DLeftTopFace2 = nextDataPoint.DpInfo.Faces.Area3DLeftTopFace;
					Path path4 = area3DLeftTopFace2.TopFace as Path;
					AreaChart.GetAreaZIndex(dataPoint.DpInfo.VisualPosition.X, dataPoint.DpInfo.VisualPosition.Y, dataPoint.DpInfo.InternalYValue > 0.0 || flag3);
					LineSegment lineSegment5 = Area3DDataPointFace.GetLineSegment(path4, 0);
					Point point2 = new Point(nextDataPoint.DpInfo.VisualPosition.X, nextDataPoint.DpInfo.VisualPosition.Y);
					Point point3 = lineSegment5.Point;
					lineSegment5.Point = point2;
					if (value)
					{
						AnimationHelper.ApplyPointAnimation(lineSegment5, "Point", dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.0
						}, new Point[]
						{
							point3,
							point2
						}, null, double.NaN);
					}
					else
					{
						lineSegment5.Point = point2;
					}
					lineSegment5 = Area3DDataPointFace.GetLineSegment(path4, 1);
					point2 = new Point(nextDataPoint.DpInfo.VisualPosition.X + area3DLeftTopFace2.Depth3d, nextDataPoint.DpInfo.VisualPosition.Y - area3DLeftTopFace2.Depth3d);
					point3 = lineSegment5.Point;
					lineSegment5.Point = point2;
					if (value)
					{
						AnimationHelper.ApplyPointAnimation(lineSegment5, "Point", dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.0
						}, new Point[]
						{
							point3,
							point2
						}, null, double.NaN);
					}
					else
					{
						lineSegment5.Point = point2;
					}
					lineSegment5 = Area3DDataPointFace.GetLineSegment(path3, 0);
					point2 = new Point(dataPoint.DpInfo.VisualPosition.X, num3);
					point3 = lineSegment5.Point;
					lineSegment5.Point = point2;
					if (value)
					{
						AnimationHelper.ApplyPointAnimation(lineSegment5, "Point", dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.0
						}, new Point[]
						{
							point3,
							point2
						}, null, double.NaN);
					}
					else
					{
						lineSegment5.Point = point2;
					}
					lineSegment5 = Area3DDataPointFace.GetLineSegment(path3, 1);
					point2 = new Point(dataPoint.DpInfo.VisualPosition.X + area3DLeftTopFace.Depth3d, num3 - area3DLeftTopFace.Depth3d);
					point3 = lineSegment5.Point;
					lineSegment5.Point = point2;
					if (value)
					{
						AnimationHelper.ApplyPointAnimation(lineSegment5, "Point", dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.0
						}, new Point[]
						{
							point3,
							point2
						}, null, double.NaN);
					}
					else
					{
						lineSegment5.Point = point2;
					}
				}
				else if (flag2)
				{
					Point point7 = default(Point);
					Graphics.IntersectionOfTwoStraightLines(new Point(nextDataPoint.DpInfo.VisualPosition.X, num8), new Point(dataPoint.DpInfo.VisualPosition.X, num8), nextDataPoint.DpInfo.VisualPosition, dataPoint.DpInfo.VisualPosition, ref point7);
					Area3DDataPointFace area3DLeftTopFace3 = nextDataPoint.DpInfo.Faces.Area3DLeftTopFace;
					Path path5 = area3DLeftTopFace3.TopFace as Path;
					int areaZIndex = AreaChart.GetAreaZIndex(dataPoint.DpInfo.VisualPosition.X, dataPoint.DpInfo.VisualPosition.Y, dataPoint.DpInfo.InternalYValue > 0.0 || flag2);
					path5.SetValue(Panel.ZIndexProperty, areaZIndex);
					LineSegment lineSegment6 = Area3DDataPointFace.GetLineSegment(path5, 0);
					Point point2 = new Point(point7.X, point7.Y);
					Point point3 = lineSegment6.Point;
					lineSegment6.Point = point2;
					if (value)
					{
						AnimationHelper.ApplyPointAnimation(lineSegment6, "Point", dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.0
						}, new Point[]
						{
							point3,
							point2
						}, null, double.NaN);
					}
					else
					{
						lineSegment6.Point = point2;
					}
					lineSegment6 = Area3DDataPointFace.GetLineSegment(path5, 1);
					point2 = new Point(point7.X + area3DLeftTopFace.Depth3d, point7.Y - area3DLeftTopFace.Depth3d);
					point3 = lineSegment6.Point;
					lineSegment6.Point = point2;
					if (value)
					{
						AnimationHelper.ApplyPointAnimation(lineSegment6, "Point", dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.0
						}, new Point[]
						{
							point3,
							point2
						}, null, double.NaN);
					}
					else
					{
						lineSegment6.Point = point2;
					}
					lineSegment6 = Area3DDataPointFace.GetLineSegment(path3, 0);
					point2 = new Point(dataPoint.DpInfo.VisualPosition.X, num3);
					point3 = lineSegment6.Point;
					lineSegment6.Point = point2;
					LineSegment lineSegment7 = Area3DDataPointFace.GetLineSegment(path3, 1);
					Point point8 = new Point(dataPoint.DpInfo.VisualPosition.X + area3DLeftTopFace.Depth3d, num3 - area3DLeftTopFace.Depth3d);
					Point point9 = lineSegment7.Point;
					lineSegment7.Point = point8;
					if (value)
					{
						if (dataPoint.DpInfo.OldYValue < 0.0 && dataPoint.DpInfo.InternalYValue >= 0.0)
						{
							double num13 = Math.Abs(dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point.Y - num8);
							double num14 = num13 / Math.Abs(dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point.Y - dataPoint.DpInfo.VisualPosition.Y);
							double num15 = Math.Abs(point3.X - dataPoint.DpInfo.VisualPosition.X);
							double arg_12CC_0 = num15 / Math.Abs(dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point.Y - dataPoint.DpInfo.VisualPosition.Y);
							AnimationHelper.ApplyPointAnimation(lineSegment6, "Point", dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								num14,
								1.0
							}, new Point[]
							{
								point3,
								new Point(dataPoint.DpInfo.VisualPosition.X, num8),
								point2
							}, null, double.NaN);
							AnimationHelper.ApplyPointAnimation(lineSegment7, "Point", dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								num14,
								1.0
							}, new Point[]
							{
								point9,
								new Point(dataPoint.DpInfo.VisualPosition.X + area3DLeftTopFace.Depth3d, num8 - area3DLeftTopFace.Depth3d),
								point8
							}, null, double.NaN);
						}
						else
						{
							AnimationHelper.ApplyPointAnimation(lineSegment6, "Point", dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								1.0
							}, new Point[]
							{
								point3,
								point2
							}, null, double.NaN);
							AnimationHelper.ApplyPointAnimation(lineSegment7, "Point", dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								1.0
							}, new Point[]
							{
								point3,
								point2
							}, null, double.NaN);
						}
					}
				}
				else
				{
					LineSegment lineSegment8 = Area3DDataPointFace.GetLineSegment(path3, 0);
					Point point2 = new Point(dataPoint.DpInfo.VisualPosition.X, num3);
					Point point3 = lineSegment8.Point;
					lineSegment8.Point = point2;
					LineSegment lineSegment9 = Area3DDataPointFace.GetLineSegment(path3, 1);
					Point point10 = new Point(dataPoint.DpInfo.VisualPosition.X + area3DLeftTopFace.Depth3d, num3 - area3DLeftTopFace.Depth3d);
					Point point11 = lineSegment9.Point;
					lineSegment9.Point = point10;
					if (dataPoint.DpInfo.OldYValue < 0.0 && dataPoint.DpInfo.InternalYValue >= 0.0)
					{
						if (value)
						{
							double num16 = Math.Abs(dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point.Y - num8);
							double num17 = num16 / Math.Abs(dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point.Y - dataPoint.DpInfo.VisualPosition.Y);
							double num18 = Math.Abs(point3.X - dataPoint.DpInfo.VisualPosition.X);
							double arg_1649_0 = num18 / Math.Abs(dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point.Y - dataPoint.DpInfo.VisualPosition.Y);
							AnimationHelper.ApplyPointAnimation(lineSegment8, "Point", dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								num17,
								1.0
							}, new Point[]
							{
								point3,
								new Point(dataPoint.DpInfo.VisualPosition.X, num8),
								point2
							}, null, double.NaN);
							AnimationHelper.ApplyPointAnimation(lineSegment9, "Point", dataPoint, storyboard, 0.0, new double[]
							{
								0.0,
								num17,
								1.0
							}, new Point[]
							{
								point11,
								new Point(dataPoint.DpInfo.VisualPosition.X + area3DLeftTopFace.Depth3d, num8 - area3DLeftTopFace.Depth3d),
								point10
							}, null, double.NaN);
						}
					}
					else if (value)
					{
						AnimationHelper.ApplyPointAnimation(lineSegment8, "Point", dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.0
						}, new Point[]
						{
							point3,
							point2
						}, null, double.NaN);
						AnimationHelper.ApplyPointAnimation(lineSegment9, "Point", dataPoint, storyboard, 0.0, new double[]
						{
							0.0,
							1.0
						}, new Point[]
						{
							point11,
							point10
						}, null, double.NaN);
					}
				}
			}
			if (dataPoint.DpInfo.Faces.Area3DRightFace != null)
			{
				Area3DDataPointFace area3DRightFace = dataPoint.DpInfo.Faces.Area3DRightFace;
				Path path6 = area3DRightFace.RightFace as Path;
				PathFigure pathFigure2 = Area3DDataPointFace.GetPathFigure(path6);
				Point point2 = new Point(num2, num3);
				Point point3 = pathFigure2.StartPoint;
				pathFigure2.StartPoint = point2;
				if (value)
				{
					AnimationHelper.ApplyPointAnimation(pathFigure2, "StartPoint", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new Point[]
					{
						point3,
						point2
					}, null, double.NaN);
				}
				else
				{
					pathFigure2.StartPoint = point2;
				}
				LineSegment lineSegment10 = Area3DDataPointFace.GetLineSegment(path6, 2);
				point2 = new Point(num2 + area3DRightFace.Depth3d, num3 - area3DRightFace.Depth3d);
				point3 = lineSegment10.Point;
				lineSegment10.Point = point2;
				if (value)
				{
					AnimationHelper.ApplyPointAnimation(lineSegment10, "Point", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new Point[]
					{
						point3,
						point2
					}, null, double.NaN);
				}
				else
				{
					lineSegment10.Point = point2;
				}
				lineSegment10 = Area3DDataPointFace.GetLineSegment(path6, 0);
				point2 = new Point(num2, num8);
				point3 = lineSegment10.Point;
				lineSegment10.Point = point2;
				lineSegment10 = Area3DDataPointFace.GetLineSegment(path6, 1);
				point2 = new Point(num2 + area3DRightFace.Depth3d, num8 - area3DRightFace.Depth3d);
				point3 = lineSegment10.Point;
				lineSegment10.Point = point2;
			}
			if (value)
			{
				if (dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment == null)
				{
					return;
				}
				Point point3 = dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point;
				dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point = dataPoint.DpInfo.VisualPosition;
				if (dataPoint == nextDataPoint)
				{
					if (chart.View3D && parent.Faces != null && parent.Faces.FrontFacePaths.Count > 0)
					{
						(parent.Faces.FrontFacePaths[parent.Faces.FrontFacePaths.Count - 1].Data as PathGeometry).Figures[0].StartPoint = new Point(parent.DataPoints[0].DpInfo.VisualPosition.X + num, num8);
						LineSegment lineSegment11 = Area3DDataPointFace.GetLineSegment(parent.Faces.FrontFacePaths[parent.Faces.FrontFacePaths.Count - 1], 0);
						lineSegment11.Point = new Point(parent.DataPoints[0].DpInfo.VisualPosition.X + num, num8 - num);
						lineSegment11 = Area3DDataPointFace.GetLineSegment(parent.Faces.FrontFacePaths[parent.Faces.FrontFacePaths.Count - 1], 1);
						lineSegment11.Point = new Point(dataPoint.DpInfo.VisualPosition.X + num, num8 - num);
						lineSegment11 = Area3DDataPointFace.GetLineSegment(parent.Faces.FrontFacePaths[parent.Faces.FrontFacePaths.Count - 1], 2);
						lineSegment11.Point = new Point(dataPoint.DpInfo.VisualPosition.X, num8);
					}
					if (parent.Faces != null && parent.Faces.FrontFacePaths.Count > 0)
					{
						(parent.Faces.FrontFacePaths[0].Data as PathGeometry).Figures[0].StartPoint = new Point((parent.Faces.FrontFacePaths[0].Data as PathGeometry).Figures[0].StartPoint.X, num8);
					}
					dataPoint.DpInfo.Faces.AreaFrontFaceBaseLineSegment.Point = new Point(dataPoint.DpInfo.VisualPosition.X, num8);
				}
				AnimationHelper.ApplyPointAnimation(dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment, "Point", dataPoint, storyboard, 0.0, new double[]
				{
					0.0,
					1.0
				}, new Point[]
				{
					point3,
					dataPoint.DpInfo.VisualPosition
				}, null, double.NaN);
				dataPoint.DpInfo.Storyboard = storyboard;
				oldVisualPositionOfDataPoint = point3;
				if (parent.Bevel.Value && !chart.View3D)
				{
					AreaChart.AnimateBevelLayer(dataPoint, oldVisualPositionOfDataPoint, value);
				}
				storyboard.Begin(chart._rootElement, true);
			}
			else
			{
				dataPoint.DpInfo.Faces.AreaFrontFaceLineSegment.Point = dataPoint.DpInfo.VisualPosition;
				if (dataPoint == nextDataPoint)
				{
					if (chart.View3D && parent.Faces != null && parent.Faces.FrontFacePaths.Count > 0)
					{
						(parent.Faces.FrontFacePaths[parent.Faces.FrontFacePaths.Count - 1].Data as PathGeometry).Figures[0].StartPoint = new Point(parent.DataPoints[0].DpInfo.VisualPosition.X, num8);
						LineSegment lineSegment12 = Area3DDataPointFace.GetLineSegment(parent.Faces.FrontFacePaths[parent.Faces.FrontFacePaths.Count - 1], 0);
						lineSegment12.Point = new Point(parent.DataPoints[0].DpInfo.VisualPosition.X, num8 - num);
						lineSegment12 = Area3DDataPointFace.GetLineSegment(parent.Faces.FrontFacePaths[parent.Faces.FrontFacePaths.Count - 1], 1);
						lineSegment12.Point = new Point(dataPoint.DpInfo.VisualPosition.X + num, num8 - num);
						lineSegment12 = Area3DDataPointFace.GetLineSegment(parent.Faces.FrontFacePaths[parent.Faces.FrontFacePaths.Count - 1], 2);
						lineSegment12.Point = new Point(dataPoint.DpInfo.VisualPosition.X, num8);
					}
					if (parent.Faces != null && parent.Faces.FrontFacePaths.Count > 0)
					{
						(parent.Faces.FrontFacePaths[0].Data as PathGeometry).Figures[0].StartPoint = new Point((parent.Faces.FrontFacePaths[0].Data as PathGeometry).Figures[0].StartPoint.X, num8);
					}
					dataPoint.DpInfo.Faces.AreaFrontFaceBaseLineSegment.Point = new Point(dataPoint.DpInfo.VisualPosition.X, num8);
				}
				if (parent.Bevel.Value && !chart.View3D)
				{
					AreaChart.AnimateBevelLayer(dataPoint, oldVisualPositionOfDataPoint, value);
				}
			}
			if (parent.ToolTipElement != null)
			{
				parent.ToolTipElement.Hide();
			}
			chart.ChartArea.DisableIndicators();
			ColumnChart.CreateOrUpdatePlank(chart, parent.PlotGroup.AxisY, canvas.Parent as Canvas, num, Orientation.Horizontal);
			PlotArea plotArea = chart.PlotArea;
			List<IDataPoint> list = (from datapoint in chart.PlotDetails.ListOfAllDataPoints
			where datapoint.Enabled == true
			select datapoint).ToList<IDataPoint>();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Parent.PlotGroup.AxisY.AxisMinimum != null)
				{
					double limitingYValue = list[i].Parent.PlotGroup.GetLimitingYValue();
					if ((list[i].DpInfo.InternalYValue < limitingYValue && limitingYValue > 0.0) || (list[i].DpInfo.InternalYValue < limitingYValue && list[i].Parent.PlotGroup.AxisY.InternalAxisMinimum == 0.0) || list[i].DpInfo.InternalYValue < list[i].Parent.PlotGroup.AxisY.InternalAxisMinimum)
					{
						if (list[i].DpInfo.Marker != null && list[i].DpInfo.Marker.Visual != null)
						{
							list[i].DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
						}
					}
					else if (list[i].DpInfo.LabelVisual != null)
					{
						(((list[i].DpInfo.LabelVisual as Border).Child as Canvas).Children[0] as TextBlock).Visibility = Visibility.Visible;
					}
				}
			}
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			rectangleGeometry.Rect = new Rect(0.0, plotArea.BorderThickness.Top - num, width + num, height + num - plotArea.BorderThickness.Bottom - plotArea.BorderThickness.Top);
			if (canvas.Parent != null)
			{
				(canvas.Parent as Canvas).Clip = rectangleGeometry;
			}
			rectangleGeometry = new RectangleGeometry();
			double x = 0.0;
			double y = -num - 4.0;
			double width2 = width + num;
			double height2 = height + num + chart.ChartArea.PLANK_THICKNESS + 10.0;
			AreaChart.GetClipCoordinates(chart, ref x, ref y, ref width2, ref height2, parent.PlotGroup.MinimumX, parent.PlotGroup.MaximumX);
			rectangleGeometry.Rect = new Rect(x, y, width2, height2);
			if (labelCanvas != null)
			{
				labelCanvas.Clip = rectangleGeometry;
			}
		}

		private static void AnimateBevelLayer(IDataPoint dataPoint, Point oldVisualPositionOfDataPoint, bool animationEnabled)
		{
			Line bevelLine = dataPoint.DpInfo.Faces.BevelLine;
			IDataPoint arg_21_0 = dataPoint.DpInfo.Faces.PreviousDataPoint;
			IDataPoint nextDataPoint = dataPoint.DpInfo.Faces.NextDataPoint;
			Storyboard storyboard = dataPoint.DpInfo.Storyboard;
			if (dataPoint.DpInfo.Faces.PreviousDataPoint == dataPoint)
			{
				if (animationEnabled)
				{
					AnimationHelper.ApplyPropertyAnimation(bevelLine, "Y1", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new double[]
					{
						oldVisualPositionOfDataPoint.Y,
						dataPoint.DpInfo.VisualPosition.Y
					}, null);
					AnimationHelper.ApplyPropertyAnimation(bevelLine, "X1", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new double[]
					{
						oldVisualPositionOfDataPoint.X,
						dataPoint.DpInfo.VisualPosition.X
					}, null);
					return;
				}
				bevelLine.Y1 = dataPoint.DpInfo.VisualPosition.Y;
				bevelLine.X1 = dataPoint.DpInfo.VisualPosition.X;
				return;
			}
			else if (dataPoint == nextDataPoint)
			{
				if (animationEnabled)
				{
					AnimationHelper.ApplyPropertyAnimation(bevelLine, "Y2", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new double[]
					{
						oldVisualPositionOfDataPoint.Y,
						dataPoint.DpInfo.VisualPosition.Y
					}, null);
					AnimationHelper.ApplyPropertyAnimation(bevelLine, "X2", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new double[]
					{
						oldVisualPositionOfDataPoint.X,
						dataPoint.DpInfo.VisualPosition.X
					}, null);
					return;
				}
				bevelLine.Y2 = dataPoint.DpInfo.VisualPosition.Y;
				bevelLine.X2 = dataPoint.DpInfo.VisualPosition.X;
				return;
			}
			else
			{
				if (animationEnabled)
				{
					AnimationHelper.ApplyPropertyAnimation(bevelLine, "Y1", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new double[]
					{
						oldVisualPositionOfDataPoint.Y,
						dataPoint.DpInfo.VisualPosition.Y
					}, null);
					AnimationHelper.ApplyPropertyAnimation(bevelLine, "X1", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new double[]
					{
						oldVisualPositionOfDataPoint.X,
						dataPoint.DpInfo.VisualPosition.X
					}, null);
				}
				else
				{
					bevelLine.Y1 = dataPoint.DpInfo.VisualPosition.Y;
					bevelLine.X1 = dataPoint.DpInfo.VisualPosition.X;
				}
				bevelLine = dataPoint.DpInfo.Faces.PreviousDataPoint.DpInfo.Faces.BevelLine;
				if (animationEnabled)
				{
					AnimationHelper.ApplyPropertyAnimation(bevelLine, "Y2", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new double[]
					{
						oldVisualPositionOfDataPoint.Y,
						dataPoint.DpInfo.VisualPosition.Y
					}, null);
					AnimationHelper.ApplyPropertyAnimation(bevelLine, "X2", dataPoint, storyboard, 0.0, new double[]
					{
						0.0,
						1.0
					}, new double[]
					{
						oldVisualPositionOfDataPoint.X,
						dataPoint.DpInfo.VisualPosition.X
					}, null);
					return;
				}
				bevelLine.Y2 = dataPoint.DpInfo.VisualPosition.Y;
				bevelLine.X2 = dataPoint.DpInfo.VisualPosition.X;
				return;
			}
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
			bool arg_1A_0 = chart.View3D;
			if (property <= VcProperties.LightingEnabled)
			{
				if (property <= VcProperties.DataPoints)
				{
					switch (property)
					{
					case VcProperties.Bevel:
						goto IL_285;
					case VcProperties.BorderColor:
					case VcProperties.BorderStyle:
					case VcProperties.BorderThickness:
						goto IL_1BB;
					case VcProperties.BubbleStyle:
						return;
					case VcProperties.Color:
						break;
					default:
						if (property != VcProperties.DataPoints)
						{
							return;
						}
						goto IL_285;
					}
				}
				else
				{
					if (property == VcProperties.Enabled)
					{
						goto IL_285;
					}
					if (property != VcProperties.LightingEnabled)
					{
						return;
					}
				}
				if (dataSeries.Faces == null || dataSeries.Faces.FrontFacePaths == null)
				{
					return;
				}
				using (List<Path>.Enumerator enumerator = dataSeries.Faces.FrontFacePaths.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Path current = enumerator.Current;
						if (chart.View3D)
						{
							current.Fill = (dataSeries.LightingEnabled.Value ? Graphics.GetFrontFaceBrush(dataSeries.Color) : dataSeries.Color);
						}
						else
						{
							current.Fill = (dataSeries.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(dataSeries.Color, "Linear", null) : dataSeries.Color);
						}
					}
					return;
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
					goto IL_215;
				}
			}
			else
			{
				if (property == VcProperties.XValue)
				{
					goto IL_285;
				}
				switch (property)
				{
				case VcProperties.YValue:
				case VcProperties.YValues:
					goto IL_285;
				case VcProperties.YValueFormatString:
					return;
				default:
					return;
				}
			}
			if (dataSeries.Faces == null || dataSeries.Faces.FrontFacePaths == null)
			{
				return;
			}
			using (List<Path>.Enumerator enumerator2 = dataSeries.Faces.FrontFacePaths.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Path current2 = enumerator2.Current;
					current2.Opacity = (double)newValue;
				}
				return;
			}
			IL_1BB:
			if (dataSeries.Faces == null || dataSeries.Faces.FrontFacePaths == null)
			{
				return;
			}
			using (List<Path>.Enumerator enumerator3 = dataSeries.Faces.FrontFacePaths.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Path current3 = enumerator3.Current;
					AreaChart.ApplyBorderProperties(current3, dataSeries);
				}
				return;
			}
			IL_215:
			if (!VisifireControl.IsMediaEffectsEnabled || dataSeries.Faces == null || dataSeries.Faces.Visual == null)
			{
				return;
			}
			if (dataSeries.ShadowEnabled.Value)
			{
				dataSeries.Faces.Visual.Effect = ExtendedGraphics.GetShadowEffect(135.0, 2.0, 1.0);
				return;
			}
			dataSeries.Faces.Visual.Effect = null;
			return;
			IL_285:
			chart.ChartArea.RenderSeries();
		}

		internal static Canvas GetStacked2DArea(ref Faces faces, PolygonalChartShapeParams areaParams)
		{
			if (faces.Parts == null)
			{
				faces.Parts = new List<DependencyObject>();
			}
			Canvas canvas = new Canvas();
			canvas.Width = areaParams.Size.Width;
			canvas.Height = areaParams.Size.Height;
			Polygon polygon = new Polygon
			{
				Tag = new ElementData
				{
					Element = areaParams.TagReference,
					VisualElementName = "AreaBase"
				}
			};
			faces.Parts.Add(polygon);
			polygon.Fill = (areaParams.Lighting ? Graphics.GetLightingEnabledBrush(areaParams.Background, "Linear", null) : areaParams.Background);
			polygon.Stroke = areaParams.BorderColor;
			polygon.StrokeDashArray = ((areaParams.BorderStyle != null) ? ExtendedGraphics.CloneCollection(areaParams.BorderStyle) : areaParams.BorderStyle);
			polygon.StrokeThickness = areaParams.BorderThickness;
			polygon.StrokeMiterLimit = 1.0;
			polygon.Points = areaParams.Points;
			Rect bounds = AreaChart.GetBounds(areaParams.Points);
			polygon.Stretch = Stretch.Fill;
			polygon.Width = bounds.Width;
			polygon.Height = bounds.Height;
			polygon.SetValue(Canvas.TopProperty, bounds.Y);
			polygon.SetValue(Canvas.LeftProperty, bounds.X);
			canvas.Children.Add(polygon);
			if (areaParams.Bevel)
			{
				for (int i = 0; i < areaParams.Points.Count - 1; i++)
				{
					if (areaParams.Points[i].X != areaParams.Points[i + 1].X)
					{
						double slope = AreaChart.GetSlope(areaParams.Points[i].X, areaParams.Points[i].Y, areaParams.Points[i + 1].X, areaParams.Points[i + 1].Y);
						double num = AreaChart.GetIntercept(areaParams.Points[i].X, areaParams.Points[i].Y, areaParams.Points[i + 1].X, areaParams.Points[i + 1].Y);
						num += (double)((areaParams.IsPositive ? 1 : -1) * 4);
						Point value = new Point(areaParams.Points[i].X, slope * areaParams.Points[i].X + num);
						Point value2 = new Point(areaParams.Points[i + 1].X, slope * areaParams.Points[i + 1].X + num);
						PointCollection pointCollection = new PointCollection();
						pointCollection.Add(areaParams.Points[i]);
						pointCollection.Add(areaParams.Points[i + 1]);
						pointCollection.Add(value2);
						pointCollection.Add(value);
						Polygon polygon2 = new Polygon
						{
							Tag = new ElementData
							{
								Element = areaParams.TagReference,
								VisualElementName = "Bevel"
							}
						};
						polygon2.Points = pointCollection;
						polygon2.Fill = Graphics.GetBevelTopBrush(areaParams.Background);
						faces.Parts.Add(polygon2);
						canvas.Children.Add(polygon2);
					}
				}
			}
			return canvas;
		}

		internal static Canvas GetStacked3DAreaFrontFace(ref Faces faces, PolygonalChartShapeParams areaParams)
		{
			Polygon polygon = new Polygon
			{
				Tag = new ElementData
				{
					Element = areaParams.TagReference,
					VisualElementName = "AreaBase"
				}
			};
			faces.Parts.Add(polygon);
			Point centroid = AreaChart.GetCentroid(areaParams.Points);
			Rect bounds = AreaChart.GetBounds(areaParams.Points);
			polygon.SetValue(Panel.ZIndexProperty, (int)centroid.Y + 1000);
			polygon.Fill = (areaParams.Lighting ? Graphics.GetFrontFaceBrush4Area3D(areaParams.Background) : areaParams.Background);
			polygon.Stroke = areaParams.BorderColor;
			polygon.StrokeDashArray = ((areaParams.BorderStyle != null) ? ExtendedGraphics.CloneCollection(areaParams.BorderStyle) : areaParams.BorderStyle);
			polygon.StrokeThickness = areaParams.BorderThickness;
			polygon.StrokeMiterLimit = 1.0;
			polygon.Points = areaParams.Points;
			polygon.Stretch = Stretch.Fill;
			polygon.Width = bounds.Width;
			polygon.Height = bounds.Height;
			polygon.SetValue(Canvas.TopProperty, areaParams.Depth3D);
			polygon.SetValue(Canvas.LeftProperty, 0.0);
			Canvas canvas = new Canvas
			{
				Tag = new ElementData
				{
					Element = areaParams.TagReference
				}
			};
			canvas.Width = bounds.Width + areaParams.Depth3D;
			canvas.Height = bounds.Height + areaParams.Depth3D;
			canvas.SetValue(Canvas.TopProperty, bounds.Top - areaParams.Depth3D);
			canvas.SetValue(Canvas.LeftProperty, bounds.Left);
			canvas.Children.Add(polygon);
			return canvas;
		}

		internal static void AttachEvents2AreaVisual(VisifireElement obj, VisifireElement senderElement, FrameworkElement visual)
		{
			if (visual == null)
			{
				return;
			}
			if (obj.GetMouseEnterEventHandler() != null)
			{
				visual.MouseEnter += delegate(object sender, MouseEventArgs e)
				{
					if (obj.GetMouseEnterEventHandler() != null)
					{
						IDataPoint nearestDataPointOnMouseEvent;
						if (obj.GetType().Equals(typeof(DataSeries)) || obj.GetType().IsSubclassOf(typeof(DataSeries)))
						{
							nearestDataPointOnMouseEvent = (obj as DataSeries).GetNearestDataPointOnMouseEvent(e);
						}
						else
						{
							nearestDataPointOnMouseEvent = (obj as IDataPoint).Parent.GetNearestDataPointOnMouseEvent(e);
						}
						obj.FireMouseEnterEvent(nearestDataPointOnMouseEvent, e);
					}
				};
			}
			if (obj.GetMouseLeaveEventHandler() != null)
			{
				visual.MouseLeave += delegate(object sender, MouseEventArgs e)
				{
					if (obj.GetMouseLeaveEventHandler() != null)
					{
						IDataPoint nearestDataPointOnMouseEvent;
						if (obj.GetType().Equals(typeof(DataSeries)) || obj.GetType().IsSubclassOf(typeof(DataSeries)))
						{
							nearestDataPointOnMouseEvent = (obj as DataSeries).GetNearestDataPointOnMouseEvent(e);
						}
						else
						{
							nearestDataPointOnMouseEvent = (obj as IDataPoint).Parent.GetNearestDataPointOnMouseEvent(e);
						}
						obj.FireMouseLeaveEvent(nearestDataPointOnMouseEvent, e);
					}
				};
			}
			object obj2 = obj.GetMouseLeftButtonDownEventHandler();
			if (obj2 != null)
			{
				visual.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
				{
					if (obj.GetMouseLeftButtonDownEventHandler() != null)
					{
						IDataPoint nearestDataPointOnMouseButtonEvent;
						if (obj.GetType().Equals(typeof(DataSeries)) || obj.GetType().IsSubclassOf(typeof(DataSeries)))
						{
							nearestDataPointOnMouseButtonEvent = (obj as DataSeries).GetNearestDataPointOnMouseButtonEvent(e);
						}
						else
						{
							nearestDataPointOnMouseButtonEvent = (obj as IDataPoint).Parent.GetNearestDataPointOnMouseButtonEvent(e);
						}
						obj.FireMouseLeftButtonDownEvent(nearestDataPointOnMouseButtonEvent, e);
					}
				};
			}
			obj2 = obj.GetMouseLeftButtonUpEventHandler();
			if (obj2 != null)
			{
				visual.MouseLeftButtonUp += delegate(object sender, MouseButtonEventArgs e)
				{
					if (obj.GetMouseLeftButtonUpEventHandler() != null)
					{
						IDataPoint nearestDataPointOnMouseButtonEvent;
						if (obj.GetType().Equals(typeof(DataSeries)) || obj.GetType().IsSubclassOf(typeof(DataSeries)))
						{
							nearestDataPointOnMouseButtonEvent = (obj as DataSeries).GetNearestDataPointOnMouseButtonEvent(e);
						}
						else
						{
							nearestDataPointOnMouseButtonEvent = (obj as IDataPoint).Parent.GetNearestDataPointOnMouseButtonEvent(e);
						}
						obj.FireMouseLeftButtonUpEvent(nearestDataPointOnMouseButtonEvent, e);
					}
				};
			}
			obj2 = obj.GetMouseMoveEventHandler();
			if (obj2 != null)
			{
				visual.MouseMove += delegate(object sender, MouseEventArgs e)
				{
					if (obj.GetMouseMoveEventHandler() != null)
					{
						IDataPoint nearestDataPointOnMouseEvent;
						if (obj.GetType().Equals(typeof(DataSeries)) || obj.GetType().IsSubclassOf(typeof(DataSeries)))
						{
							nearestDataPointOnMouseEvent = (obj as DataSeries).GetNearestDataPointOnMouseEvent(e);
						}
						else
						{
							nearestDataPointOnMouseEvent = (obj as IDataPoint).Parent.GetNearestDataPointOnMouseEvent(e);
						}
						obj.FireMouseMoveEvent(nearestDataPointOnMouseEvent, e);
					}
				};
			}
			object obj3 = obj.GetMouseLeftButtonDownEventHandler();
			if (obj3 != null)
			{
				visual.MouseRightButtonDown += delegate(object sender, MouseButtonEventArgs e)
				{
					if (obj.GetMouseLeftButtonDownEventHandler() != null)
					{
						IDataPoint nearestDataPointOnMouseButtonEvent;
						if (obj.GetType().Equals(typeof(DataSeries)) || obj.GetType().IsSubclassOf(typeof(DataSeries)))
						{
							nearestDataPointOnMouseButtonEvent = (obj as DataSeries).GetNearestDataPointOnMouseButtonEvent(e);
						}
						else
						{
							nearestDataPointOnMouseButtonEvent = (obj as IDataPoint).Parent.GetNearestDataPointOnMouseButtonEvent(e);
						}
						obj.FireMouseLeftButtonDownEvent(nearestDataPointOnMouseButtonEvent, e);
					}
				};
			}
			obj3 = obj.GetMouseRightButtonUpEventHandler();
			if (obj3 != null)
			{
				visual.MouseRightButtonUp += delegate(object sender, MouseButtonEventArgs e)
				{
					if (obj.GetMouseRightButtonUpEventHandler() != null)
					{
						IDataPoint nearestDataPointOnMouseButtonEvent;
						if (obj.GetType().Equals(typeof(DataSeries)) || obj.GetType().IsSubclassOf(typeof(DataSeries)))
						{
							nearestDataPointOnMouseButtonEvent = (obj as DataSeries).GetNearestDataPointOnMouseButtonEvent(e);
						}
						else
						{
							nearestDataPointOnMouseButtonEvent = (obj as IDataPoint).Parent.GetNearestDataPointOnMouseButtonEvent(e);
						}
						obj.FireMouseRightButtonUpEvent(nearestDataPointOnMouseButtonEvent, e);
					}
				};
			}
		}
	}
}
