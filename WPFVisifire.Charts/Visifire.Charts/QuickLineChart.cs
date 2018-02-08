using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class QuickLineChart
	{
		public static void Update(IElement sender, ElementTypes elementType, VcProperties property, object newValue, bool isAxisChanged)
		{
			bool flag = elementType == ElementTypes.DataPoint;
			if (flag)
			{
				DataSeries parent = (sender as IDataPoint).Parent;
				if (parent != null && (property == VcProperties.YValue || property == VcProperties.XValue))
				{
					QuickLineChart.UpdateDataSeries(parent, property, newValue);
					return;
				}
			}
			else
			{
				QuickLineChart.UpdateDataSeries(sender as DataSeries, property, newValue);
			}
		}

		private static void UpdateDataSeries(ObservableObject obj, VcProperties property, object newValue)
		{
			DataSeries dataSeries = obj as DataSeries;
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
			PlotGroup arg_3C_0 = dataSeries.PlotGroup;
			Canvas canvas = null;
			List<Shape> list = new List<Shape>();
			if (dataSeries.Faces != null)
			{
				canvas = (dataSeries.Faces.Visual as Canvas);
				List<Shape> list2 = canvas.Children.OfType<Shape>().ToList<Shape>();
				using (List<Shape>.Enumerator enumerator = list2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Shape current = enumerator.Current;
						if (current != null)
						{
							list.Add(current);
						}
					}
					goto IL_FD;
				}
				goto IL_AE;
				IL_FD:
				double height = chart.ChartArea.ChartVisualCanvas.Height;
				double width = chart.ChartArea.ChartVisualCanvas.Width;
				if (dataSeries.LightWeight.Value)
				{
					QuickLineChart.UpdateLineSeries(dataSeries, width, height);
					return;
				}
				if (property <= VcProperties.LineCap)
				{
					if (property <= VcProperties.Color)
					{
						if (property == VcProperties.ViewportRangeEnabled)
						{
							goto IL_46A;
						}
						if (property != VcProperties.Color)
						{
							return;
						}
						if (list.Count <= 0)
						{
							return;
						}
						using (List<Shape>.Enumerator enumerator2 = list.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Shape current2 = enumerator2.Current;
								Brush brush = (newValue != null) ? (newValue as Brush) : dataSeries.Color;
								current2.Stroke = (dataSeries.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(brush, "Linear", new double[]
								{
									0.65,
									0.55
								}) : brush);
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
							goto IL_46A;
						default:
							if (property == VcProperties.Enabled)
							{
								goto IL_3FA;
							}
							switch (property)
							{
							case VcProperties.LightingEnabled:
								break;
							case VcProperties.LightWeight:
								return;
							case VcProperties.LineCap:
								goto IL_358;
							default:
								return;
							}
							break;
						}
					}
					if (list.Count <= 0)
					{
						return;
					}
					using (List<Shape>.Enumerator enumerator3 = list.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							Shape current3 = enumerator3.Current;
							if (newValue != null)
							{
								current3.Stroke = (((bool)newValue) ? Graphics.GetLightingEnabledBrush(dataSeries.Color, "Linear", new double[]
								{
									0.65,
									0.55
								}) : dataSeries.Color);
							}
						}
						return;
					}
				}
				else if (property <= VcProperties.Opacity)
				{
					switch (property)
					{
					case VcProperties.LineStyle:
					case VcProperties.LineThickness:
						if (list.Count > 0)
						{
							using (List<Shape>.Enumerator enumerator4 = list.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									Shape current4 = enumerator4.Current;
									current4.StrokeThickness = dataSeries.LineThickness.Value;
									current4.StrokeDashArray = ExtendedGraphics.GetDashArray(dataSeries.LineStyle);
								}
								return;
							}
							goto IL_3FA;
						}
						return;
					case VcProperties.LineTension:
						return;
					default:
						if (property != VcProperties.Opacity)
						{
							return;
						}
						break;
					}
				}
				else
				{
					if (property == VcProperties.ShadowEnabled || property == VcProperties.XValue)
					{
						goto IL_46A;
					}
					switch (property)
					{
					case VcProperties.YValue:
					case VcProperties.YValues:
						goto IL_46A;
					case VcProperties.YValueFormatString:
						return;
					default:
						return;
					}
				}
				if (list.Count <= 0)
				{
					return;
				}
				using (List<Shape>.Enumerator enumerator5 = list.GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						Shape current5 = enumerator5.Current;
						current5.Opacity = dataSeries.Opacity;
					}
					return;
				}
				IL_358:
				if (list.Count > 0)
				{
					list[0].StrokeStartLineCap = dataSeries.LineCap;
					list[list.Count - 1].StrokeEndLineCap = dataSeries.LineCap;
					return;
				}
				return;
				IL_3FA:
				if (!flag && canvas != null)
				{
					if (!(bool)newValue)
					{
						canvas.Visibility = Visibility.Collapsed;
					}
					else
					{
						if (canvas.Parent == null)
						{
							ColumnChart.Update(chart, dataSeries.RenderAs, (from ds in chart.InternalSeries
							where ds.RenderAs == RenderAs.QuickLine
							select ds).ToList<DataSeries>());
							return;
						}
						canvas.Visibility = Visibility.Visible;
					}
					chart._toolTip.Hide();
					return;
				}
				IL_46A:
				QuickLineChart.UpdateLineSeries(dataSeries, width, height);
				return;
			}
			IL_AE:
			if (dataSeries.Faces == null && property == VcProperties.Enabled && (bool)newValue)
			{
				ColumnChart.Update(chart, dataSeries.RenderAs, (from ds in chart.InternalSeries
				where ds.RenderAs == RenderAs.QuickLine
				select ds).ToList<DataSeries>());
			}
		}

		internal static void UpdateSizeOfTheParentCanvas(DataSeries dataSeries, double width, double height)
		{
			(dataSeries.Faces.Visual as Canvas).Width = width;
			(dataSeries.Faces.Visual as Canvas).Height = height;
			Canvas canvas = dataSeries.Faces.Visual.Parent as Canvas;
			canvas.Width = width;
			canvas.Height = height;
		}

		internal static void UpdateLineSeries(DataSeries dataSeries, double width, double height)
		{
			if (dataSeries == null)
			{
				return;
			}
			Chart chart = dataSeries.Chart as Chart;
			bool isOffsetAdded = true;
			if (chart == null)
			{
				return;
			}
			if (dataSeries.Faces == null || dataSeries.Faces.Visual == null || dataSeries.Faces.Visual.Parent == null)
			{
				return;
			}
			Canvas chartsCanvas = dataSeries.Faces.Visual.Parent as Canvas;
			QuickLineChart.UpdateSizeOfTheParentCanvas(dataSeries, width, height);
			List<List<IDataPoint>> list = new List<List<IDataPoint>>();
			List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(dataSeries, false, RenderHelper.IsRasterRenderSupported(dataSeries));
			LineChart.CreateGroupOfBrokenLineSegment(dataSeries, dataPointsUnderViewPort, width, height, out list);
			Canvas canvas = dataSeries.Faces.Visual as Canvas;
			if (dataSeries.LightWeight.Value)
			{
				chart.ChartArea.DisableIndicators();
				LineChartShapeParams lineChartShapeParams = new LineChartShapeParams();
				lineChartShapeParams.LineThickness = dataSeries.LineThickness.Value;
				lineChartShapeParams.LineColor = dataSeries.Color;
				LineStyles lineStyle = dataSeries.LineStyle;
				if (lineStyle != LineStyles.Solid)
				{
					lineChartShapeParams.LineStyle = ExtendedGraphics.GetDashArray(lineStyle);
				}
				lineChartShapeParams.Opacity = dataSeries.Opacity;
				dataSeries.VisualParams = lineChartShapeParams;
				QuickLineChart.AddRasterQuickLine2D(dataSeries, list, dataSeries.VisualParams as LineChartShapeParams, canvas);
			}
			else
			{
				bool flag = false;
				if (canvas.Children.Count > 0)
				{
					List<Shape> list2 = canvas.Children.OfType<Shape>().ToList<Shape>();
					using (List<Shape>.Enumerator enumerator = list2.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Shape current = enumerator.Current;
							if (current != null && current.GetType() == typeof(Line))
							{
								flag = true;
								break;
							}
						}
						goto IL_195;
					}
				}
				flag = true;
				IL_195:
				if (flag)
				{
					canvas.Children.Clear();
					LineChartShapeParams lineChartShapeParams2 = new LineChartShapeParams();
					lineChartShapeParams2.Points = new List<IDataPoint>();
					lineChartShapeParams2.ShadowPoints = new List<IDataPoint>();
					lineChartShapeParams2.LineGeometryGroup = new GeometryGroup();
					lineChartShapeParams2.LineThickness = dataSeries.LineThickness.Value;
					lineChartShapeParams2.Lighting = dataSeries.LightingEnabled.Value;
					lineChartShapeParams2.LineColor = (lineChartShapeParams2.Lighting ? Graphics.GetLightingEnabledBrush(dataSeries.Color, "Linear", new double[]
					{
						0.65,
						0.55
					}) : dataSeries.Color);
					LineStyles lineStyle2 = dataSeries.LineStyle;
					if (lineStyle2 != LineStyles.Solid)
					{
						lineChartShapeParams2.LineStyle = ExtendedGraphics.GetDashArray(lineStyle2);
					}
					lineChartShapeParams2.Opacity = dataSeries.Opacity;
					dataSeries.VisualParams = lineChartShapeParams2;
					QuickLineChart.AddQuickPolyLine2D(dataSeries, list, lineChartShapeParams2, canvas);
				}
				else
				{
					QuickLineChart.UpdateListOfPolyLine(dataSeries, list);
				}
			}
			for (int i = 0; i < dataPointsUnderViewPort.Count; i++)
			{
				if (dataPointsUnderViewPort[i].Parent != null && dataPointsUnderViewPort[i].Parent.PlotGroup.AxisY.GetValue(Axis.AxisMinimumProperty) != null)
				{
					double limitingYValue = dataPointsUnderViewPort[i].Parent.PlotGroup.GetLimitingYValue();
					if (dataPointsUnderViewPort[i].DpInfo.InternalYValue < limitingYValue)
					{
						isOffsetAdded = false;
						break;
					}
				}
			}
			QuickLineChart.Clip(chart, chartsCanvas, isOffsetAdded);
			dataSeries.Visual.Visibility = Visibility.Visible;
		}

		internal static void Clip(Chart chart, Canvas chartsCanvas, bool isOffsetAdded)
		{
			if (chartsCanvas == null)
			{
				return;
			}
			double num = chart.ChartArea.PLANK_DEPTH / (double)((chart.PlotDetails.Layer3DCount == 0) ? 1 : chart.PlotDetails.Layer3DCount) * (double)(chart.View3D ? 1 : 0);
			Rect rect = default(Rect);
			double num2 = 4.0;
			if (isOffsetAdded)
			{
				rect = new Rect(0.0, 0.0, chartsCanvas.Width + num, chartsCanvas.Height + chart.ChartArea.PLANK_DEPTH + num2);
			}
			else
			{
				rect = new Rect(0.0, 0.0, chartsCanvas.Width + num, chartsCanvas.Height);
			}
			if (chartsCanvas.Clip != null && chartsCanvas.Clip.GetType().Equals(typeof(RectangleGeometry)))
			{
				(chartsCanvas.Clip as RectangleGeometry).Rect = rect;
				return;
			}
			RectangleGeometry clip = new RectangleGeometry
			{
				Rect = rect
			};
			chartsCanvas.Clip = clip;
		}

		internal static Canvas GetVisualObjectForQuickLineChart(Panel preExistingPanel, double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, double plankDepth, bool animationEnabled)
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
			foreach (DataSeries current in seriesList)
			{
				current.Faces = null;
				QuickLineChart.CreateAlineSeries(current, width, height, canvas3, animationEnabled);
				flag = (flag || current.MovingMarkerEnabled);
			}
			List<IDataPoint> listOfAllDataPoints = plotDetails.ListOfAllDataPoints;
			for (int i = 0; i < listOfAllDataPoints.Count - 1; i++)
			{
				if (listOfAllDataPoints[i].Parent != null && listOfAllDataPoints[i].Parent.PlotGroup.AxisY.GetValue(Axis.AxisMinimumProperty) != null)
				{
					double limitingYValue = listOfAllDataPoints[i].Parent.PlotGroup.GetLimitingYValue();
					if (listOfAllDataPoints[i].DpInfo.InternalYValue < limitingYValue)
					{
						isOffsetAdded = false;
						break;
					}
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
			canvas3.Height = height;
			canvas2.Height = height;
			canvas3.Width = width;
			canvas2.Width = width;
			QuickLineChart.Clip(chart, canvas3, isOffsetAdded);
			return canvas;
		}

		internal static void CreateAlineSeries(DataSeries dataSeries, double width, double height, Canvas chartsCanvas, bool animationEnabled)
		{
			Canvas canvas = null;
			if (dataSeries.Faces != null)
			{
				canvas = (dataSeries.Faces.Visual as Canvas);
			}
			if (!dataSeries.Enabled.Value)
			{
				return;
			}
			VisifireControl arg_33_0 = dataSeries.Chart;
			List<List<IDataPoint>> pointCollectionList = new List<List<IDataPoint>>();
			PlotGroup arg_40_0 = dataSeries.PlotGroup;
			LineChartShapeParams lineChartShapeParams = new LineChartShapeParams();
			lineChartShapeParams.Points = new List<IDataPoint>();
			lineChartShapeParams.ShadowPoints = new List<IDataPoint>();
			lineChartShapeParams.LineGeometryGroup = new GeometryGroup();
			lineChartShapeParams.LineThickness = dataSeries.LineThickness.Value;
			lineChartShapeParams.Lighting = dataSeries.LightingEnabled.Value;
			lineChartShapeParams.LineColor = dataSeries.Color;
			LineStyles lineStyle = dataSeries.LineStyle;
			if (lineStyle != LineStyles.Solid)
			{
				lineChartShapeParams.LineStyle = ExtendedGraphics.GetDashArray(lineStyle);
			}
			lineChartShapeParams.ShadowEnabled = false;
			lineChartShapeParams.Opacity = dataSeries.Opacity;
			if (lineChartShapeParams.ShadowEnabled)
			{
				lineChartShapeParams.LineShadowGeometryGroup = new GeometryGroup();
			}
			dataSeries.VisualParams = lineChartShapeParams;
			dataSeries.Faces = new Faces();
			List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(dataSeries, false, RenderHelper.IsRasterRenderSupported(dataSeries));
			LineChart.CreateGroupOfBrokenLineSegment(dataSeries, dataPointsUnderViewPort, width, height, out pointCollectionList);
			if (canvas == null)
			{
				canvas = new Canvas
				{
					Width = Math.Ceiling(width),
					Height = Math.Ceiling(height)
				};
			}
			if (dataSeries.LightWeight.Value)
			{
				QuickLineChart.AddRasterQuickLine2D(dataSeries, pointCollectionList, lineChartShapeParams, canvas);
			}
			else
			{
				lineChartShapeParams.LineColor = (lineChartShapeParams.Lighting ? Graphics.GetLightingEnabledBrush(dataSeries.Color, "Linear", new double[]
				{
					0.65,
					0.55
				}) : dataSeries.Color);
				QuickLineChart.AddQuickPolyLine2D(dataSeries, pointCollectionList, lineChartShapeParams, canvas);
			}
			canvas.Width = width;
			canvas.Height = height;
			chartsCanvas.Children.Add(canvas);
			dataSeries.Faces.Visual = canvas;
			if (animationEnabled)
			{
				if (dataSeries.Storyboard == null)
				{
					dataSeries.Storyboard = new Storyboard();
				}
				else
				{
					dataSeries.Storyboard.Stop();
				}
				dataSeries.Storyboard = LineChart.ApplyLineChartAnimation(dataSeries, canvas, dataSeries.Storyboard, true);
			}
		}

		private static void AddQuickLine2D(DataSeries tagReference, List<List<IDataPoint>> pointCollectionList, LineChartShapeParams lineParams, Canvas lineCanvas)
		{
			foreach (List<IDataPoint> current in pointCollectionList)
			{
				if (current.Count != 0)
				{
					QuickLineChart.DrawLineBrokenSegment(lineParams, tagReference, current, lineCanvas);
				}
			}
		}

		private static void DrawLineBrokenSegment(LineChartShapeParams lineParams, DataSeries tagReference, List<IDataPoint> dataPoints, Canvas lineCanvas)
		{
			if (dataPoints.Count > 0)
			{
				Point startPoint = dataPoints[0].DpInfo.VisualPosition;
				for (int i = 1; i < dataPoints.Count; i++)
				{
					IDataPoint dataPoint = dataPoints[i];
					if (dataPoint.Chart != null)
					{
						Point visualPosition = dataPoint.DpInfo.VisualPosition;
						Line line = new Line();
						QuickLineChart.UpdateLinePoints(line, startPoint, visualPosition, tagReference, lineParams);
						if (i == 1)
						{
							line.StrokeStartLineCap = dataPoint.Parent.LineCap;
						}
						else if (i == dataPoints.Count - 1)
						{
							line.StrokeEndLineCap = dataPoint.Parent.LineCap;
						}
						startPoint = visualPosition;
						lineCanvas.Children.Add(line);
						if (dataPoints[i - 1].DpInfo.Faces == null)
						{
							dataPoints[i - 1].DpInfo.Faces = new Faces();
							dataPoints[i - 1].DpInfo.Faces.IsStartPoint = true;
						}
						dataPoints[i].DpInfo.Faces = new Faces();
						dataPoints[i - 1].DpInfo.Faces.Parts.Add(line);
						dataPoints[i].DpInfo.Faces.Parts.Add(line);
						if (i == dataPoints.Count - 1)
						{
							dataPoints[i].DpInfo.Faces.IsEndPoint = true;
						}
					}
				}
			}
		}

		private static void UpdateLinePoints(Line line, Point startPoint, Point endPoint, DataSeries tagReference, LineChartShapeParams lineParams)
		{
			line.X1 = startPoint.X;
			line.Y1 = startPoint.Y;
			line.X2 = endPoint.X;
			line.Y2 = endPoint.Y;
			line.Tag = new ElementData
			{
				Element = tagReference
			};
			line.StrokeLineJoin = PenLineJoin.Round;
			line.StrokeStartLineCap = PenLineCap.Round;
			line.StrokeEndLineCap = PenLineCap.Round;
			line.Stroke = lineParams.LineColor;
			line.StrokeThickness = lineParams.LineThickness;
			line.StrokeDashArray = lineParams.LineStyle;
			line.Opacity = lineParams.Opacity;
		}

		private static void AddQuickPolyLine2D(DataSeries tagReference, List<List<IDataPoint>> pointCollectionList, LineChartShapeParams lineParams, Canvas lineCanvas)
		{
			List<Polyline> listOfPolyLine = QuickLineChart.GetListOfPolyLine(lineParams, pointCollectionList);
			foreach (Polyline current in listOfPolyLine)
			{
				current.Tag = new ElementData
				{
					Element = tagReference
				};
				lineCanvas.Children.Add(current);
			}
		}

		private static void AddRasterQuickLine2D(DataSeries tagReference, List<List<IDataPoint>> pointCollectionList, LineChartShapeParams lineParams, Canvas lineCanvas)
		{
			Canvas plotAreaCanvas = (tagReference.Chart as Chart).ChartArea.PlotAreaCanvas;
			int num = (int)plotAreaCanvas.Width;
			int num2 = (int)plotAreaCanvas.Height;
			if (num2 == 0 || num == 0)
			{
				return;
			}
			RasterLineStyle rasterLineStyle = new RasterLineStyle
			{
				Stroke = (lineParams.LineColor as SolidColorBrush).Color,
				Fill = ((lineParams.LineFill == null) ? Colors.Transparent : (lineParams.LineFill as SolidColorBrush).Color),
				StrokeThickness = lineParams.LineThickness,
				IsAntiAliased = tagReference.IsAntiAliased,
				Opacity = lineParams.Opacity,
				PenLineJoinA = new PenLineJoin?(PenLineJoin.Round)
			};
			using (WriteableBitmapAdapter writeableBitmapAdapter = new WriteableBitmapAdapter(num, num2))
			{
				double num3 = tagReference.PlotGroup.AxisX.GetAxisVisualLeftAsOffset();
				if (double.IsNaN(num3))
				{
					num3 = 0.0;
				}
				foreach (List<IDataPoint> current in pointCollectionList)
				{
					for (int i = 0; i < current.Count; i++)
					{
						if (num3 != 0.0)
						{
							Point visualPosition = current[i].DpInfo.VisualPosition;
							visualPosition.X -= Math.Abs(num3);
							current[i].DpInfo.RasterVisualPosition = visualPosition;
						}
						else
						{
							current[i].DpInfo.RasterVisualPosition = current[i].DpInfo.VisualPosition;
						}
					}
					int count = current.Count;
					for (int j = 0; j < count - 1; j++)
					{
						IDataPoint dataPoint = current[j];
						IDataPoint dataPoint2 = current[j + 1];
						if (j + 1 == count - 1)
						{
							rasterLineStyle.PenLineJoinB = new PenLineJoin?(PenLineJoin.Round);
						}
						Point rasterVisualPosition = dataPoint.DpInfo.RasterVisualPosition;
						Point rasterVisualPosition2 = dataPoint2.DpInfo.RasterVisualPosition;
						RasterRenderEngine.DrawLine(writeableBitmapAdapter, rasterVisualPosition, rasterVisualPosition2, rasterLineStyle);
					}
				}
				if (tagReference.Faces != null && tagReference.Faces.Parts.Count > 0)
				{
					Canvas canvas = tagReference.Faces.Parts[0] as Canvas;
					canvas.Height = lineCanvas.Height;
					canvas.Width = lineCanvas.Width;
					canvas.Background = null;
					canvas.Tag = new ElementData
					{
						Element = tagReference
					};
					Image image = canvas.Children[0] as Image;
					image.Width = (double)num;
					image.Height = (double)num2;
					image.Source = null;
					image.Source = writeableBitmapAdapter.GetImageSource();
					image.SetValue(Canvas.LeftProperty, Math.Abs(num3));
				}
				else
				{
					Canvas canvas = new Canvas
					{
						Width = lineCanvas.Width,
						Height = lineCanvas.Height,
						Tag = new ElementData
						{
							Element = tagReference
						}
					};
					tagReference.Faces.Parts.Add(canvas);
					Image image2 = new Image
					{
						Height = (double)num2,
						Width = (double)num,
						IsHitTestVisible = false
					};
					image2.Source = writeableBitmapAdapter.GetImageSource();
					image2.SetValue(Canvas.LeftProperty, Math.Abs(num3));
					canvas.Children.Add(image2);
					lineCanvas.Children.Add(canvas);
				}
			}
		}

		private static void UpdateListOfPolyLine(DataSeries dataSeries, List<List<IDataPoint>> dataPointCollectionList)
		{
			dataSeries.Dispatcher.BeginInvoke(new Action(delegate
			{
				if (dataSeries.Faces == null || dataSeries.Faces.Visual == null)
				{
					return;
				}
				Canvas canvas = dataSeries.Faces.Visual as Canvas;
				List<Polyline> list = new List<Polyline>();
				foreach (UIElement uIElement in canvas.Children)
				{
					list.Add(uIElement as Polyline);
				}
				if (list != null)
				{
					using (List<Polyline>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Polyline current = enumerator2.Current;
							if (current != null)
							{
								current.Points.Clear();
							}
						}
						goto IL_C6;
					}
				}
				list = new List<Polyline>();
				IL_C6:
				for (int i = 0; i < dataPointCollectionList.Count; i++)
				{
					Polyline polyline = null;
					if (dataPointCollectionList[i].Count > 0)
					{
						if (list.Count > i)
						{
							polyline = list[i];
						}
						if (polyline == null)
						{
							polyline = new Polyline();
							polyline.Tag = new ElementData
							{
								Element = dataSeries
							};
							QuickLineChart.UpdatePolyLineProperties(polyline, dataSeries.VisualParams as LineChartShapeParams);
							canvas.Children.Add(polyline);
						}
						PointCollection pointCollection = new PointCollection();
						foreach (IDataPoint current2 in dataPointCollectionList[i])
						{
							pointCollection.Add(current2.DpInfo.VisualPosition);
						}
						polyline.Points = pointCollection;
					}
				}
			}), new object[0]);
		}

		private static void UpdatePolyLineProperties(Polyline polyLine, LineChartShapeParams lineParams)
		{
			polyLine.Stroke = lineParams.LineColor;
			polyLine.StrokeThickness = lineParams.LineThickness;
			polyLine.StrokeLineJoin = PenLineJoin.Round;
			polyLine.StrokeStartLineCap = PenLineCap.Round;
			polyLine.StrokeEndLineCap = PenLineCap.Round;
			polyLine.Opacity = lineParams.Opacity;
			if (lineParams.LineStyle != null)
			{
				polyLine.StrokeDashArray = lineParams.LineStyle;
			}
		}

		private static List<Polyline> GetListOfPolyLine(LineChartShapeParams lineParams, List<List<IDataPoint>> dataPointCollectionList)
		{
			List<Polyline> list = new List<Polyline>();
			foreach (List<IDataPoint> current in dataPointCollectionList)
			{
				Polyline polyline = new Polyline();
				QuickLineChart.UpdatePolyLineProperties(polyline, lineParams);
				if (current.Count > 0)
				{
					PointCollection pointCollection = new PointCollection();
					foreach (IDataPoint current2 in current)
					{
						pointCollection.Add(current2.DpInfo.VisualPosition);
					}
					polyline.Points = pointCollection;
				}
				list.Add(polyline);
			}
			return list;
		}
	}
}
