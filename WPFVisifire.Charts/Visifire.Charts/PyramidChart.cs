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
	internal static class PyramidChart
	{
		private enum Pyramid3dLayer
		{
			BackLayerLeft,
			BackLayerRight,
			FrontLayerLeft,
			FrontLayerRight,
			TopLayer,
			BottomLayer,
			LightingLayerFront,
			LightingLayerLeft,
			LightingLayerRight
		}

		private class Pyramid3DLayer
		{
			public Point L;

			public Point R;

			public Point CT;

			public Point CB;

			public PointCollection GetAllPoints()
			{
				return new PointCollection
				{
					this.L,
					this.CT,
					this.R,
					this.CB
				};
			}
		}

		private class Pyramid2DSlice
		{
			public Point LT;

			public Point RT;

			public Point LB;

			public Point RB;

			public Point CT
			{
				get
				{
					return new Point(this.LT.X + Math.Abs(this.RT.X - this.LT.X) / 2.0, this.LT.Y);
				}
			}

			public Point CB
			{
				get
				{
					return new Point(this.LB.X + Math.Abs(this.RB.X - this.LB.X) / 2.0, this.LB.Y);
				}
			}

			public PointCollection GetAllPoints()
			{
				return new PointCollection
				{
					this.LT,
					this.RT,
					this.RB,
					this.LB
				};
			}
		}

		private class Pyramid3DSlice
		{
			private double _topOffset;

			private double _bottomOffset;

			private PyramidChart.Pyramid2DSlice _points2d;

			private PyramidChart.Pyramid3DLayer _top3DLayer;

			private PyramidChart.Pyramid3DLayer _bottom3DLayer;

			private Brush _color;

			private bool _isTopOfPyramid;

			private bool _lightingEnabled;

			private double _opacity;

			public PyramidChart.Pyramid3DLayer Top3DLayer
			{
				get
				{
					return this._top3DLayer;
				}
			}

			public PyramidChart.Pyramid3DLayer Bottom3DLayer
			{
				get
				{
					return this._bottom3DLayer;
				}
			}

			public Pyramid3DSlice(PyramidChart.Pyramid2DSlice pyramid2DSlice, double topOffset, double bottomOffset, Brush color, bool isTopOfPyramid, bool lightingEnabled, double opacity)
			{
				this._points2d = pyramid2DSlice;
				this._topOffset = topOffset;
				this._bottomOffset = bottomOffset;
				this._color = color;
				this._isTopOfPyramid = isTopOfPyramid;
				this._lightingEnabled = lightingEnabled;
				this._opacity = opacity;
				this.Calculate3DLayerPoints();
			}

			private void Calculate3DLayerPoints()
			{
				this._top3DLayer = new PyramidChart.Pyramid3DLayer
				{
					L = this.CalculatePoint(this._points2d.LT, this._isTopOfPyramid ? 0.0 : (-this._topOffset)),
					R = this.CalculatePoint(this._points2d.RT, this._isTopOfPyramid ? 0.0 : (-this._topOffset)),
					CT = this.CalculatePoint(this._points2d.CT, this._isTopOfPyramid ? 0.0 : (-this._topOffset - this._bottomOffset)),
					CB = this.CalculatePoint(this._points2d.CT, this._isTopOfPyramid ? 0.0 : this._topOffset)
				};
				this._bottom3DLayer = new PyramidChart.Pyramid3DLayer
				{
					L = this.CalculatePoint(this._points2d.LB, -this._bottomOffset),
					R = this.CalculatePoint(this._points2d.RB, -this._bottomOffset),
					CT = this.CalculatePoint(this._points2d.CB, -2.0 * this._bottomOffset),
					CB = this.CalculatePoint(this._points2d.CB, this._bottomOffset)
				};
			}

			public Path GetFace(PyramidChart.Pyramid3dLayer pyramid3dFaceType, IDataPoint dp, bool attachTagInfo)
			{
				PointCollection points = this.Get3DLayerPoints(pyramid3dFaceType);
				Path path = this.GetPath(points);
				path.Opacity = this._opacity;
				PyramidChart.Pyramid3DSlice.ApplyColor(ref path, pyramid3dFaceType, this._color, this._lightingEnabled, this._isTopOfPyramid);
				this.SetZIndexOf3DLayers(ref path, pyramid3dFaceType);
				if (attachTagInfo)
				{
					path.Tag = new ElementData
					{
						Element = dp,
						VisualElementName = pyramid3dFaceType.ToString()
					};
				}
				return path;
			}

			private void SetZIndexOf3DLayers(ref Path path, PyramidChart.Pyramid3dLayer pyramid3dFaceType)
			{
				switch (pyramid3dFaceType)
				{
				case PyramidChart.Pyramid3dLayer.BackLayerLeft:
					path.SetValue(Panel.ZIndexProperty, -1);
					return;
				case PyramidChart.Pyramid3dLayer.BackLayerRight:
					path.SetValue(Panel.ZIndexProperty, -1);
					return;
				case PyramidChart.Pyramid3dLayer.FrontLayerLeft:
					path.SetValue(Panel.ZIndexProperty, 1);
					return;
				case PyramidChart.Pyramid3dLayer.FrontLayerRight:
					path.SetValue(Panel.ZIndexProperty, 1);
					return;
				case PyramidChart.Pyramid3dLayer.TopLayer:
					path.SetValue(Panel.ZIndexProperty, 2);
					return;
				case PyramidChart.Pyramid3dLayer.BottomLayer:
					path.SetValue(Panel.ZIndexProperty, -2);
					return;
				case PyramidChart.Pyramid3dLayer.LightingLayerFront:
					path.SetValue(Panel.ZIndexProperty, 2);
					return;
				case PyramidChart.Pyramid3dLayer.LightingLayerLeft:
					path.SetValue(Panel.ZIndexProperty, 2);
					return;
				case PyramidChart.Pyramid3dLayer.LightingLayerRight:
					path.SetValue(Panel.ZIndexProperty, 2);
					return;
				default:
					return;
				}
			}

			public static void ApplyColor(ref Path path, PyramidChart.Pyramid3dLayer pyramid3dFaceType, Brush color, bool lightingEnabled, bool isTopOfPyramid)
			{
				path.StrokeLineJoin = PenLineJoin.Round;
				Color arg_0D_0 = Colors.White;
				SolidColorBrush solidColorBrush = color as SolidColorBrush;
				switch (pyramid3dFaceType)
				{
				case PyramidChart.Pyramid3dLayer.BackLayerLeft:
					if (lightingEnabled)
					{
						path.Fill = Graphics.GetDarkerBrush(color, 0.4);
					}
					else
					{
						path.Fill = color;
					}
					break;
				case PyramidChart.Pyramid3dLayer.BackLayerRight:
					if (lightingEnabled)
					{
						path.Fill = Graphics.GetDarkerBrush(color, 0.4);
					}
					else
					{
						path.Fill = color;
					}
					break;
				case PyramidChart.Pyramid3dLayer.FrontLayerLeft:
					if (lightingEnabled)
					{
						if (solidColorBrush != null)
						{
							path.Fill = Graphics.CreateLinearGradientBrush(-35.0, new Point(0.0, 1.0), new Point(1.0, 0.0), new List<Color>
							{
								Graphics.GetLighterColor(solidColorBrush.Color, 0.8678),
								Graphics.GetLighterColor(solidColorBrush.Color, 0.99),
								solidColorBrush.Color
							}, new List<double>
							{
								0.0,
								0.7,
								1.0
							});
						}
					}
					else
					{
						path.Fill = color;
					}
					break;
				case PyramidChart.Pyramid3dLayer.FrontLayerRight:
					if (lightingEnabled)
					{
						path.Fill = Graphics.GetDarkerBrush(color, 0.6);
					}
					else
					{
						path.Fill = color;
					}
					break;
				case PyramidChart.Pyramid3dLayer.TopLayer:
					if (solidColorBrush == null || !lightingEnabled)
					{
						path.Fill = color;
					}
					else if (solidColorBrush != null)
					{
						path.Fill = Graphics.CreateLinearGradientBrush(-35.0, new Point(0.0, 0.0), new Point(1.0, 1.0), new List<Color>
						{
							Graphics.GetLighterColor(solidColorBrush.Color, 0.99),
							Graphics.GetDarkerColor(solidColorBrush.Color, 0.6)
						}, new List<double>
						{
							0.0,
							0.7
						});
					}
					break;
				case PyramidChart.Pyramid3dLayer.BottomLayer:
					if (lightingEnabled)
					{
						path.Fill = Graphics.GetDarkerBrush(color, 0.4);
					}
					else
					{
						path.Fill = color;
					}
					break;
				case PyramidChart.Pyramid3dLayer.LightingLayerFront:
					if (solidColorBrush != null && lightingEnabled)
					{
						Color color2 = Graphics.GetLighterColor(solidColorBrush.Color, 1.0);
						if (isTopOfPyramid)
						{
							path.StrokeThickness = 2.0;
						}
						else
						{
							path.StrokeThickness = 3.5;
						}
						path.Stroke = Graphics.GetBevelSideBrush(180.0, solidColorBrush);
						path.StrokeStartLineCap = PenLineCap.Flat;
						path.StrokeEndLineCap = PenLineCap.Flat;
					}
					break;
				case PyramidChart.Pyramid3dLayer.LightingLayerLeft:
					if (solidColorBrush != null && lightingEnabled)
					{
						path.StrokeThickness = 2.0;
						Color color2 = Graphics.GetLighterColor(solidColorBrush.Color, 0.88);
						path.Stroke = new SolidColorBrush(color2);
						path.StrokeStartLineCap = PenLineCap.Round;
						path.StrokeEndLineCap = PenLineCap.Round;
					}
					break;
				case PyramidChart.Pyramid3dLayer.LightingLayerRight:
					if (solidColorBrush != null && lightingEnabled)
					{
						path.StrokeThickness = 2.0;
						Color color2 = solidColorBrush.Color;
						color2.A = 127;
						path.Stroke = Graphics.CreateLinearGradientBrush(0.0, new Point(0.0, 1.0), new Point(1.0, 0.2), new List<Color>
						{
							color2,
							Graphics.GetDarkerColor(solidColorBrush.Color, 0.6)
						}, new List<double>
						{
							0.0,
							0.6
						});
						path.StrokeStartLineCap = PenLineCap.Round;
						path.StrokeEndLineCap = PenLineCap.Round;
					}
					break;
				}
				path.StrokeLineJoin = PenLineJoin.Round;
			}

			private Path GetPath(PointCollection points)
			{
				if (points.Count < 1)
				{
					return null;
				}
				Path path = new Path();
				PathGeometry pathGeometry = new PathGeometry();
				PathFigure pathFigure = new PathFigure
				{
					StartPoint = points[0],
					IsClosed = true
				};
				PolyLineSegment polyLineSegment = new PolyLineSegment();
				for (int i = 1; i < points.Count; i++)
				{
					polyLineSegment.Points.Add(points[i]);
				}
				pathFigure.Segments.Add(polyLineSegment);
				pathGeometry.Figures.Add(pathFigure);
				path.Data = pathGeometry;
				return path;
			}

			private PointCollection Get3DLayerPoints(PyramidChart.Pyramid3dLayer pyramid3dFaceType)
			{
				PointCollection pointCollection = new PointCollection();
				switch (pyramid3dFaceType)
				{
				case PyramidChart.Pyramid3dLayer.BackLayerLeft:
					pointCollection.Add(this._top3DLayer.L);
					pointCollection.Add(this._top3DLayer.CT);
					pointCollection.Add(this._bottom3DLayer.CT);
					pointCollection.Add(this._bottom3DLayer.L);
					break;
				case PyramidChart.Pyramid3dLayer.BackLayerRight:
					pointCollection.Add(this._top3DLayer.CT);
					pointCollection.Add(this._top3DLayer.R);
					pointCollection.Add(this._bottom3DLayer.R);
					pointCollection.Add(this._bottom3DLayer.CT);
					break;
				case PyramidChart.Pyramid3dLayer.FrontLayerLeft:
					pointCollection.Add(this._top3DLayer.L);
					pointCollection.Add(this._top3DLayer.CB);
					pointCollection.Add(this._bottom3DLayer.CB);
					pointCollection.Add(this._bottom3DLayer.L);
					break;
				case PyramidChart.Pyramid3dLayer.FrontLayerRight:
					pointCollection.Add(this._top3DLayer.CB);
					pointCollection.Add(this._top3DLayer.R);
					pointCollection.Add(this._bottom3DLayer.R);
					pointCollection.Add(this._bottom3DLayer.CB);
					break;
				case PyramidChart.Pyramid3dLayer.TopLayer:
					pointCollection = this._top3DLayer.GetAllPoints();
					break;
				case PyramidChart.Pyramid3dLayer.BottomLayer:
					pointCollection = this._bottom3DLayer.GetAllPoints();
					break;
				case PyramidChart.Pyramid3dLayer.LightingLayerFront:
					if (this._isTopOfPyramid)
					{
						pointCollection.Add(this.CalculatePoint(this._top3DLayer.CB, 2.0));
					}
					else
					{
						pointCollection.Add(this.CalculatePoint(this._top3DLayer.CB, 1.0));
					}
					pointCollection.Add(this.CalculatePoint(this._bottom3DLayer.CB, -3.0));
					break;
				case PyramidChart.Pyramid3dLayer.LightingLayerLeft:
					pointCollection.Add(this.CalculatePoint(this._top3DLayer.CB, 0.0));
					pointCollection.Add(this.CalculatePoint(this._top3DLayer.L, 2.0, 0.0));
					break;
				case PyramidChart.Pyramid3dLayer.LightingLayerRight:
					pointCollection.Add(this.CalculatePoint(this._top3DLayer.CB, 0.0));
					pointCollection.Add(this.CalculatePoint(this._top3DLayer.R, -2.0, 1.0));
					break;
				}
				return pointCollection;
			}

			private Point CalculatePoint(Point point, double yOffSet)
			{
				return new Point(point.X, point.Y + yOffSet);
			}

			private Point CalculatePoint(Point point, double xOffSet, double yOffSet)
			{
				return new Point(point.X + xOffSet, point.Y + yOffSet);
			}
		}

		private static Random rand = new Random();

		private static double _singleGap = 0.0;

		private static double _totalGap = 0.0;

		private static Size _streamLineParentTitleSize;

		public static Grid GetVisualObjectForPyramidChart(double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, bool animationEnabled)
		{
			if (seriesList.Count <= 0)
			{
				return null;
			}
			List<DataSeries> list = (from ds in seriesList
			where ds.Enabled.Value
			select ds).ToList<DataSeries>();
			if (list.Count <= 0)
			{
				return null;
			}
			DataSeries dataSeries = list.First<DataSeries>();
			foreach (IDataPoint current in dataSeries.DataPoints)
			{
				current.DpInfo.VisualParams = null;
			}
			dataSeries.Faces = null;
			List<IDataPoint> list2 = (from dp in dataSeries.DataPoints
			where dp.Enabled == true && dp.YValue >= 0.0
			select dp).ToList<IDataPoint>();
			if ((from dp in list2
			where dp.YValue == 0.0
			select dp).Count<IDataPoint>() == list2.Count)
			{
				return null;
			}
			if (list2.Count == 0 || (list2.Count == 1 && list2[0].YValue == 0.0))
			{
				return null;
			}
			List<IDataPoint> list3 = list2.ToList<IDataPoint>();
			Grid grid = new Grid
			{
				Height = height,
				Width = width
			};
			Canvas canvas = new Canvas
			{
				Height = height
			};
			Canvas canvas2 = new Canvas
			{
				Height = height,
				HorizontalAlignment = HorizontalAlignment.Left
			};
			grid.Children.Add(canvas2);
			grid.Children.Add(canvas);
			dataSeries.Storyboard = null;
			if ((dataSeries.Chart as Chart).InternalAnimationEnabled)
			{
				dataSeries.Storyboard = new Storyboard();
			}
			PyramidChart.CreateLabelsAndSetPyramidCanvasSize(grid, canvas, canvas2, list3);
			double minPointHeight = dataSeries.MinPointHeight;
			double num = 40.0;
			bool isSameSlantAngle = true;
			double bottomRadius = 5.0;
			double gapRatio = chart.View3D ? 0.06 : 0.02;
			canvas2 = PyramidChart.CreatePyramidChart(grid, dataSeries, list3, canvas2, minPointHeight, chart.View3D, num, gapRatio, isSameSlantAngle, bottomRadius, animationEnabled);
			if (chart.View3D)
			{
				canvas2.Margin = new Thickness(0.0, -num / 2.0, 0.0, 0.0);
			}
			grid.Clip = new RectangleGeometry
			{
				Rect = new Rect(0.0, 0.0, width, chart.PlotArea.Visual.Height)
			};
			return grid;
		}

		private static void CreateLabelsAndSetPyramidCanvasSize(Grid pyramidChartCanvas, Canvas labelCanvas, Canvas pyramidCanvas, List<IDataPoint> pyramidDataPoints)
		{
			int i = 0;
			double num = 0.0;
			PyramidChart._streamLineParentTitleSize = new Size(0.0, 0.0);
			labelCanvas.Width = 0.0;
			while (i < pyramidDataPoints.Count)
			{
				pyramidDataPoints[i].DpInfo.LabelVisual = PyramidChart.CreateLabelForDataPoint(pyramidDataPoints[i], i);
				Size size = Graphics.CalculateVisualSize(pyramidDataPoints[i].DpInfo.LabelVisual);
				size.Width += 2.5;
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(pyramidDataPoints[i], VcProperties.LabelEnabled) && (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(pyramidDataPoints[i], VcProperties.LabelStyle) == LabelStyles.OutSide)
				{
					num += size.Height;
				}
				if (size.Width > labelCanvas.Width && (bool)DataPointHelper.GetDataPointValueFromProperty(pyramidDataPoints[i], VcProperties.LabelEnabled) && (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(pyramidDataPoints[i], VcProperties.LabelStyle) == LabelStyles.OutSide)
				{
					labelCanvas.Width = size.Width;
				}
				pyramidDataPoints[i].DpInfo.LabelVisual.Height = size.Height;
				pyramidDataPoints[i].DpInfo.LabelVisual.Width = size.Width;
				i++;
			}
			labelCanvas.Width += 5.0;
			pyramidCanvas.Width = Math.Max(pyramidChartCanvas.Width - labelCanvas.Width, 0.0);
			labelCanvas.SetValue(Canvas.LeftProperty, pyramidCanvas.Width);
		}

		private static Border CreateLabelForDataPoint(IDataPoint dataPoint, int sliceIndex)
		{
			LabelStyles labelStyle = (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle);
			Brush labelFontColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontColor);
			FontStyle internalFontStyle = (FontStyle)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontStyle);
			Brush internalBackground = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelBackground);
			FontFamily internalFontFamily = (FontFamily)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontFamily);
			double internalFontSize = (double)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontSize);
			FontWeight internalFontWeight = (FontWeight)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelFontWeight);
			Title title = new Title
			{
				IsNotificationEnable = false,
				Chart = dataPoint.Chart,
				Text = dataPoint.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelText)),
				InternalFontSize = internalFontSize,
				InternalFontColor = Chart.CalculateDataPointLabelFontColor(dataPoint.Chart as Chart, dataPoint, labelFontColor, labelStyle),
				InternalFontFamily = internalFontFamily,
				InternalFontStyle = internalFontStyle,
				InternalFontWeight = internalFontWeight,
				InternalBackground = internalBackground
			};
			title.CreateVisualObject(new ElementData
			{
				Element = dataPoint
			}, dataPoint.Chart.FlowDirection);
			if (!(bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelEnabled))
			{
				title.Visual.Visibility = Visibility.Collapsed;
			}
			return title.Visual;
		}

		private static Canvas CreatePyramidChart(Grid _pyramidChartGrid, DataSeries dataSeries, List<IDataPoint> dataPoints, Canvas pyramidCanvas, double minPointHeight, bool is3D, double yScale, double gapRatio, bool isSameSlantAngle, double bottomRadius, bool animationEnabled)
		{
			bool arg_10_0 = (dataSeries.Chart as Chart).InternalAnimationEnabled;
			double height = pyramidCanvas.Height;
			double width = pyramidCanvas.Width;
			TriangularChartSliceParms[] array = PyramidChart.CalculatePyramidSliceParmsInfo(dataSeries, dataPoints, height, width - 5.0, minPointHeight, is3D, yScale, gapRatio, isSameSlantAngle, bottomRadius);
			dataSeries.VisualParams = array;
			double num = width / 2.0;
			int num2 = array.Count<TriangularChartSliceParms>() + 1;
			new Random(DateTime.Now.Millisecond);
			int num3 = array.Count<TriangularChartSliceParms>();
			double num4 = 0.0;
			for (int i = 0; i < num3; i++)
			{
				array[i].FillType = dataSeries.FillType;
				Brush color = array[i].DataPoint.Color;
				double num5 = yScale * (array[i].TopRadius / num);
				double num6 = yScale * (array[i].BottomRadius / num);
				if (double.IsNaN(num5) || double.IsInfinity(num5))
				{
					num5 = 1E-07;
				}
				if (double.IsNaN(num6) || double.IsInfinity(num6))
				{
					num6 = 1E-07;
				}
				Canvas pyramidSliceVisual = PyramidChart.GetPyramidSliceVisual(i, num, is3D, array[i], num5, num6, color, animationEnabled);
				array[i].Top = array[i].TopGap + ((i == 0) ? 0.0 : (array[i - 1].Top + array[i - 1].Height + array[i - 1].BottomGap));
				pyramidSliceVisual.SetValue(Canvas.TopProperty, array[i].Top);
				pyramidSliceVisual.SetValue(Panel.ZIndexProperty, num2--);
				pyramidSliceVisual.Height = array[i].Height;
				pyramidSliceVisual.Width = num * 2.0;
				num4 += array[i].Height + array[i].TopGap;
				pyramidCanvas.Children.Add(pyramidSliceVisual);
				array[i].DataPoint.DpInfo.Faces.Visual = pyramidSliceVisual;
				array[i].DataPoint.DpInfo.VisualParams = array[i];
			}
			PyramidChart.CalcutateExplodedPosition(ref array, yScale, dataSeries);
			pyramidCanvas.Height = num4 - PyramidChart._streamLineParentTitleSize.Height;
			PyramidChart.ArrangeLabels(array, double.NaN, _pyramidChartGrid.Height);
			return pyramidCanvas;
		}

		private static void ArrangeLabels(TriangularChartSliceParms[] pyramidSlices, double width, double height)
		{
			if (pyramidSlices == null || pyramidSlices.Length < 0)
			{
				return;
			}
			TriangularChartSliceParms[] array = (from fs in pyramidSlices
			where (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(fs.DataPoint, VcProperties.LabelStyle) == LabelStyles.OutSide
			select fs).ToArray<TriangularChartSliceParms>();
			Rect area = new Rect(0.0, 0.0, width, height);
			Rect[] array2 = new Rect[array.Length];
			double num = 0.0;
			for (int i = 0; i < array.Length; i++)
			{
				double x = (double)array[i].DataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
				double num2 = (double)array[i].DataPoint.DpInfo.LabelVisual.GetValue(Canvas.TopProperty);
				double num3 = array[i].Top + num2;
				if (num3 + array[i].DataPoint.DpInfo.LabelVisual.Height + num > area.Height)
				{
					num3 -= num3 + array[i].DataPoint.DpInfo.LabelVisual.Height + num - area.Height;
				}
				array2[i] = new Rect(x, num3, array[i].DataPoint.DpInfo.LabelVisual.Width, array[i].DataPoint.DpInfo.LabelVisual.Height);
			}
			LabelPlacementHelper.VerticalLabelPlacement(area, ref array2);
			for (int j = 0; j < array.Length; j++)
			{
				double num4 = array2[j].Top - array[j].Top;
				array[j].DataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, array2[j].Left);
				array[j].DataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, num4);
				array[j].LabelLineEndPoint = new Point(array[j].LabelLineEndPoint.X, num4 + array[j].DataPoint.DpInfo.LabelVisual.Height / 2.0);
				PyramidChart.UpdateLabelLineEndPoint(array[j]);
			}
		}

		private static void UpdateLabelLineEndPoint(TriangularChartSliceParms pyramidSlice)
		{
			Path labelLine = pyramidSlice.DataPoint.DpInfo.LabelLine;
			if (labelLine != null && labelLine.Data != null)
			{
				LineSegment lineSegment = (labelLine.Data as PathGeometry).Figures[0].Segments[0] as LineSegment;
				lineSegment.Point = pyramidSlice.LabelLineEndPoint;
			}
		}

		internal static Storyboard CreateExplodingAnimation(DataSeries dataSeries, IDataPoint dataPoint, Storyboard storyboard, Panel visual, double targetValue, double beginTime)
		{
			if (storyboard == null)
			{
				storyboard = new Storyboard();
			}
			if (storyboard != null && storyboard.GetValue(Storyboard.TargetProperty) != null)
			{
				storyboard.Stop();
			}
			DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
			{
				targetValue
			});
			DoubleCollection doubleCollection = Graphics.GenerateDoubleCollection(new double[]
			{
				0.2
			});
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(doubleCollection.Count);
			DoubleAnimationUsingKeyFrames value = PieChart.CreateDoubleAnimation(dataSeries, dataPoint, visual, "(Canvas.Top)", beginTime, doubleCollection, values, splines);
			storyboard.Children.Add(value);
			return storyboard;
		}

		internal static Storyboard CreateUnExplodingAnimation(DataSeries dataSeries, IDataPoint dataPoint, Storyboard storyboard, Panel visual, double targetValue)
		{
			if (storyboard != null && storyboard.GetValue(Storyboard.TargetProperty) != null)
			{
				storyboard.Stop();
			}
			double num = (double)visual.GetValue(Canvas.TopProperty);
			DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
			{
				num,
				targetValue
			});
			DoubleCollection doubleCollection = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				0.2
			});
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(doubleCollection.Count);
			DoubleAnimationUsingKeyFrames value = PieChart.CreateDoubleAnimation(dataSeries, dataPoint, visual, "(Canvas.Top)", 0.0, doubleCollection, values, splines);
			storyboard.Children.Add(value);
			return storyboard;
		}

		private static void CalcutateExplodedPosition(ref TriangularChartSliceParms[] pyramidSlices, double yScale, DataSeries dataSeries)
		{
			int num = pyramidSlices.Count<TriangularChartSliceParms>();
			int i = 0;
			if (pyramidSlices[0].DataPoint.Parent.Exploded)
			{
				if (!pyramidSlices[0].DataPoint.Chart.IsInDesignMode)
				{
					int num2 = num / 2;
					double beginTime = 0.4;
					if ((dataSeries.Chart as Chart).ChartArea._isFirstTimeRender)
					{
						beginTime = 1.0;
					}
					i = num2;
					while (i >= 0 && i >= 0)
					{
						double targetValue = pyramidSlices[i].Top - (double)(num2 - i) * PyramidChart._singleGap;
						dataSeries.Storyboard = PyramidChart.CreateExplodingAnimation(dataSeries, pyramidSlices[i].DataPoint, dataSeries.Storyboard, pyramidSlices[i].DataPoint.DpInfo.Faces.Visual as Panel, targetValue, beginTime);
						i--;
					}
					for (i = num2 + 1; i < num; i++)
					{
						double targetValue2 = pyramidSlices[i].Top + (double)(i - num2) * PyramidChart._singleGap;
						dataSeries.Storyboard = PyramidChart.CreateExplodingAnimation(dataSeries, pyramidSlices[i].DataPoint, dataSeries.Storyboard, pyramidSlices[i].DataPoint.DpInfo.Faces.Visual as Panel, targetValue2, beginTime);
					}
					if (dataSeries.Chart != null && !(dataSeries.Chart as Chart).ChartArea._isFirstTimeRender)
					{
						dataSeries.Storyboard.Begin(dataSeries.Chart._rootElement, true);
						return;
					}
				}
			}
			else
			{
				new Storyboard();
				while (i < num)
				{
					pyramidSlices[i].ExplodedPoints = new List<Point>();
					pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation = new Storyboard();
					if (i == 0)
					{
						pyramidSlices[i].ExplodedPoints.Add(new Point(0.0, pyramidSlices[i].Top - PyramidChart._singleGap / 2.0));
						pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation = PyramidChart.CreateExplodingAnimation(dataSeries, pyramidSlices[i].DataPoint, pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation, pyramidSlices[i].DataPoint.DpInfo.Faces.Visual as Panel, pyramidSlices[i].Top - PyramidChart._singleGap / 2.0, 0.0);
						for (int j = 1; j < pyramidSlices.Length; j++)
						{
							pyramidSlices[i].ExplodedPoints.Add(new Point(0.0, pyramidSlices[j].Top + PyramidChart._singleGap / 2.0));
							pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation = PyramidChart.CreateExplodingAnimation(dataSeries, pyramidSlices[i].DataPoint, pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation, pyramidSlices[j].DataPoint.DpInfo.Faces.Visual as Panel, pyramidSlices[j].Top + PyramidChart._singleGap / 2.0, 0.0);
						}
					}
					else if (i == pyramidSlices.Length - 1)
					{
						int k;
						for (k = 0; k < i; k++)
						{
							pyramidSlices[i].ExplodedPoints.Add(new Point(0.0, pyramidSlices[k].Top - PyramidChart._singleGap / 2.0 + PyramidChart._singleGap / 6.0));
							pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation = PyramidChart.CreateExplodingAnimation(dataSeries, pyramidSlices[i].DataPoint, pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation, pyramidSlices[k].DataPoint.DpInfo.Faces.Visual as Panel, pyramidSlices[k].Top - PyramidChart._singleGap / 2.0 + PyramidChart._singleGap / 6.0, 0.0);
						}
						pyramidSlices[i].ExplodedPoints.Add(new Point(0.0, pyramidSlices[k].Top + PyramidChart._singleGap / 2.0 + PyramidChart._singleGap / 6.0));
						pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation = PyramidChart.CreateExplodingAnimation(dataSeries, pyramidSlices[i].DataPoint, pyramidSlices[k].DataPoint.DpInfo.ExplodeAnimation, pyramidSlices[k].DataPoint.DpInfo.Faces.Visual as Panel, pyramidSlices[k].Top + PyramidChart._singleGap / 2.0 + PyramidChart._singleGap / 6.0, 0.0);
					}
					else
					{
						int l;
						for (l = 0; l < i; l++)
						{
							pyramidSlices[i].ExplodedPoints.Add(new Point(0.0, pyramidSlices[l].Top - PyramidChart._singleGap / 2.0));
							pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation = PyramidChart.CreateExplodingAnimation(dataSeries, pyramidSlices[i].DataPoint, pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation, pyramidSlices[l].DataPoint.DpInfo.Faces.Visual as Panel, pyramidSlices[l].Top - PyramidChart._singleGap / 2.0, 0.0);
						}
						pyramidSlices[i].ExplodedPoints.Add(new Point(0.0, pyramidSlices[l].Top));
						pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation = PyramidChart.CreateExplodingAnimation(dataSeries, pyramidSlices[i].DataPoint, pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation, pyramidSlices[i].DataPoint.DpInfo.Faces.Visual as Panel, pyramidSlices[i].Top, 0.0);
						for (l++; l < pyramidSlices.Length; l++)
						{
							pyramidSlices[i].ExplodedPoints.Add(new Point(0.0, pyramidSlices[l].Top + PyramidChart._singleGap / 2.0));
							pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation = PyramidChart.CreateExplodingAnimation(dataSeries, pyramidSlices[i].DataPoint, pyramidSlices[i].DataPoint.DpInfo.ExplodeAnimation, pyramidSlices[l].DataPoint.DpInfo.Faces.Visual as Panel, pyramidSlices[l].Top + PyramidChart._singleGap / 2.0, 0.0);
						}
					}
					i++;
				}
			}
		}

		private static TriangularChartSliceParms[] CalculatePyramidSliceParmsInfo(DataSeries dataSeries, List<IDataPoint> dataPoints, double plotHeight, double plotWidth, double minPointHeight, bool is3D, double yScale, double gapRatio, bool isSameSlantAngle, double bottomRadius)
		{
			plotHeight = Math.Max(plotHeight, 0.0);
			plotWidth = Math.Max(plotWidth, 0.0);
			if (dataSeries.Exploded)
			{
				gapRatio = 0.02;
				PyramidChart._singleGap = gapRatio * plotHeight;
				PyramidChart._totalGap = PyramidChart._singleGap * (double)(dataPoints.Count + 1);
			}
			else
			{
				PyramidChart._singleGap = gapRatio * plotHeight;
				PyramidChart._totalGap = PyramidChart._singleGap * 2.0;
			}
			double num = plotHeight - PyramidChart._totalGap - (is3D ? (yScale / 3.0) : 0.0) - (is3D ? (plotHeight * 0.05) : 0.0);
			TriangularChartSliceParms[] array = new TriangularChartSliceParms[dataPoints.Count];
			double num2 = (from dp in dataPoints
			select dp.YValue).Sum();
			(from dp in dataPoints
			select dp.YValue).Min();
			double num3 = Math.Atan((plotWidth / 2.0 - bottomRadius) / num);
			num -= Math.Tan(num3) / ((bottomRadius == 0.0) ? 1.0 : bottomRadius);
			for (int i = 0; i < dataPoints.Count; i++)
			{
				array[i] = new TriangularChartSliceParms
				{
					DataPoint = dataPoints[i],
					TopAngle = 1.5707963267948966 - num3,
					BottomAngle = 1.5707963267948966 + num3
				};
				array[i].Height = num * (dataPoints[i].YValue / num2);
				array[i].TopRadius = ((i == 0) ? 0.0 : array[i - 1].BottomRadius);
				array[i].BottomRadius = array[i].TopRadius - array[i].Height * Math.Tan(num3);
				array[i].TopGap = 0.0;
			}
			if (!double.IsNaN(minPointHeight))
			{
				bool flag = false;
				double num4 = (from pyramidSlice in array
				select pyramidSlice.Height).Sum();
				double num5 = num4 / (double)array.Length;
				minPointHeight = minPointHeight / 100.0 * num;
				List<TriangularChartSliceParms> fixedHeightPyramidSlices = (from pyramidSlice in array
				where pyramidSlice.Height < minPointHeight
				select pyramidSlice).ToList<TriangularChartSliceParms>();
				List<TriangularChartSliceParms> source = (from pyramidSlice in array
				where !(from slice in fixedHeightPyramidSlices
				select slice).Contains(pyramidSlice)
				select pyramidSlice).ToList<TriangularChartSliceParms>();
				if (minPointHeight > num5 || fixedHeightPyramidSlices.Count == array.Count<TriangularChartSliceParms>())
				{
					flag = true;
				}
				double num6 = (from pyramidSlice in fixedHeightPyramidSlices
				select pyramidSlice.Height).Sum();
				double num7 = (from pyramidSlice in source
				select pyramidSlice.Height).Sum();
				double num8 = minPointHeight * (double)fixedHeightPyramidSlices.Count<TriangularChartSliceParms>() - num6;
				for (int j = 0; j < dataPoints.Count; j++)
				{
					if (flag)
					{
						array[j].Height = num5;
					}
					else if (array[j].Height < minPointHeight)
					{
						array[j].Height = minPointHeight;
					}
					else
					{
						array[j].Height -= num8 * (array[j].Height / num7);
					}
					array[j].TopRadius = ((j == 0) ? 0.0 : array[j - 1].BottomRadius);
					array[j].BottomRadius = array[j].TopRadius - array[j].Height * Math.Tan(num3);
				}
			}
			return array;
		}

		private static Canvas GetPyramidSliceVisual(int pyramidSliceIndex, double topRadius, bool is3D, TriangularChartSliceParms pyramidSlice, double yScaleTop, double yScaleBottom, Brush fillColor, bool animationEnabled)
		{
			pyramidSlice.Index = pyramidSliceIndex;
			Canvas canvas = PyramidChart.CreatePyramidSlice(false, topRadius, is3D, pyramidSlice, yScaleTop, yScaleBottom, fillColor, fillColor, fillColor, animationEnabled);
			bool flag = (bool)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.LightingEnabled);
			if (flag)
			{
				Brush lightingBrushForStroke = PyramidChart.GetLightingBrushForStroke(fillColor, pyramidSlice.Index);
				Brush sideFillColor = flag ? PyramidChart.GetSideLightingBrush(pyramidSlice) : fillColor;
				Brush topFillColor = is3D ? PyramidChart.GetTopBrush(fillColor, pyramidSlice) : fillColor;
				Canvas element = PyramidChart.CreatePyramidSlice(true, topRadius, is3D, pyramidSlice, yScaleTop, yScaleBottom, sideFillColor, topFillColor, lightingBrushForStroke, animationEnabled);
				canvas.Children.Add(element);
			}
			return canvas;
		}

		private static Brush GetLightingBrushForStroke(Brush fillColor, int pyramidSliceIndex)
		{
			SolidColorBrush solidColorBrush = fillColor as SolidColorBrush;
			Brush result = fillColor;
			if (pyramidSliceIndex > 0 && solidColorBrush != null)
			{
				result = new SolidColorBrush(Graphics.GetLighterColor(solidColorBrush.Color, 100.0));
			}
			else if (solidColorBrush != null)
			{
				result = Graphics.GetBevelTopBrush(new SolidColorBrush(Graphics.GetLighterColor(solidColorBrush.Color, 100.0)), 0.12);
			}
			return result;
		}

		internal static void ReCalculateAndApplyTheNewBrush(Shape shape, Brush newBrush, bool isLightingEnabled, bool is3D, TriangularChartSliceParms pyramidSliceParms)
		{
			if (is3D)
			{
				Path path = shape as Path;
				PyramidChart.Pyramid3DSlice.ApplyColor(ref path, (PyramidChart.Pyramid3dLayer)Enum.Parse(typeof(PyramidChart.Pyramid3dLayer), (shape.Tag as ElementData).VisualElementName, true), newBrush, isLightingEnabled, pyramidSliceParms.Index == 0);
				return;
			}
			string visualElementName;
			switch (visualElementName = (shape.Tag as ElementData).VisualElementName)
			{
			case "PyramidBase":
			case "FunnelTop":
				shape.Fill = newBrush;
				shape.Stroke = newBrush;
				return;
			case "TopBevel":
				shape.Fill = Graphics.GetBevelTopBrush(newBrush, 90.0);
				return;
			case "LeftBevel":
				shape.Fill = Graphics.GetBevelSideBrush(45.0, newBrush);
				return;
			case "RightBevel":
				shape.Fill = Graphics.GetBevelSideBrush(45.0, newBrush);
				return;
			case "BottomBevel":
				shape.Fill = Graphics.GetBevelSideBrush(180.0, newBrush);
				return;
			case "Lighting":
				shape.Fill = (isLightingEnabled ? PyramidChart.GetSideLightingBrush(pyramidSliceParms) : newBrush);
				return;
			case "FunnelTopLighting":
				shape.Fill = (is3D ? PyramidChart.GetTopBrush(newBrush, pyramidSliceParms) : newBrush);
				shape.Stroke = PyramidChart.GetLightingBrushForStroke(newBrush, pyramidSliceParms.Index);
				break;

				return;
			}
		}

		private static void ApplyPyramidBevel(Canvas parentVisual, TriangularChartSliceParms pyramidSlice, Brush sideFillColor, Point[] points)
		{
			if (pyramidSlice.DataPoint.Parent.Bevel.Value && pyramidSlice.Height > 5.0)
			{
				PyramidChart.CalculateBevelInnerPoints(pyramidSlice, points, pyramidSlice.Index == 0);
				Path pathFromPoints = ExtendedGraphics.GetPathFromPoints(Graphics.GetBevelTopBrush(sideFillColor, 90.0), new Point[]
				{
					points[0],
					points[4],
					points[5],
					points[1]
				});
				Path pathFromPoints2 = ExtendedGraphics.GetPathFromPoints(Graphics.GetDarkerBrush(sideFillColor, 0.9), new Point[]
				{
					points[1],
					points[5],
					points[6],
					points[2]
				});
				Path pathFromPoints3 = ExtendedGraphics.GetPathFromPoints(Graphics.GetBevelSideBrush(45.0, sideFillColor), new Point[]
				{
					points[0],
					points[4],
					points[7],
					points[3]
				});
				Path pathFromPoints4 = ExtendedGraphics.GetPathFromPoints(Graphics.GetBevelSideBrush(180.0, sideFillColor), new Point[]
				{
					points[7],
					points[6],
					points[2],
					points[3]
				});
				pathFromPoints.IsHitTestVisible = false;
				pathFromPoints3.IsHitTestVisible = false;
				pathFromPoints2.IsHitTestVisible = false;
				pathFromPoints4.IsHitTestVisible = false;
				parentVisual.Children.Add(pathFromPoints);
				parentVisual.Children.Add(pathFromPoints3);
				parentVisual.Children.Add(pathFromPoints2);
				parentVisual.Children.Add(pathFromPoints4);
				pathFromPoints.Tag = new ElementData
				{
					Element = pyramidSlice.DataPoint,
					VisualElementName = "TopBevel"
				};
				pathFromPoints3.Tag = new ElementData
				{
					Element = pyramidSlice.DataPoint,
					VisualElementName = "LeftBevel"
				};
				pathFromPoints2.Tag = new ElementData
				{
					Element = pyramidSlice.DataPoint,
					VisualElementName = "RightBevel"
				};
				pathFromPoints4.Tag = new ElementData
				{
					Element = pyramidSlice.DataPoint,
					VisualElementName = "BottomBevel"
				};
				pyramidSlice.DataPoint.DpInfo.Faces.Parts.Add(pathFromPoints);
				pyramidSlice.DataPoint.DpInfo.Faces.Parts.Add(pathFromPoints3);
				pyramidSlice.DataPoint.DpInfo.Faces.Parts.Add(pathFromPoints2);
				pyramidSlice.DataPoint.DpInfo.Faces.Parts.Add(pathFromPoints4);
				pyramidSlice.DataPoint.DpInfo.Faces.VisualComponents.Add(pathFromPoints);
				pyramidSlice.DataPoint.DpInfo.Faces.VisualComponents.Add(pathFromPoints3);
				pyramidSlice.DataPoint.DpInfo.Faces.VisualComponents.Add(pathFromPoints2);
				pyramidSlice.DataPoint.DpInfo.Faces.VisualComponents.Add(pathFromPoints4);
				if ((pyramidSlice.DataPoint.Chart as Chart).InternalAnimationEnabled)
				{
					double beginTime = 0.0;
					if (pyramidSlice.DataPoint.IsLightDataPoint)
					{
						pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime, (pyramidSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
						pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints3, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime, (pyramidSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
						pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints2, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime, (pyramidSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
						pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints4, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime, (pyramidSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
						return;
					}
					pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime, (pyramidSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
					pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints3, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime, (pyramidSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
					pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints2, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime, (pyramidSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
					pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints4, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime, (pyramidSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
				}
			}
		}

		private static Canvas CreatePyramidSlice(bool isLightingGradientLayer, double topRadius, bool is3D, TriangularChartSliceParms pyramidSlice, double yScaleTop, double yScaleBottom, Brush sideFillColor, Brush topFillColor, Brush topSurfaceStroke, bool animationEnabled)
		{
			Canvas canvas = new Canvas
			{
				Tag = new ElementData
				{
					Element = pyramidSlice.DataPoint
				}
			};
			Canvas canvas2 = new Canvas
			{
				Width = topRadius * 2.0,
				Height = pyramidSlice.Height,
				Tag = new ElementData
				{
					Element = pyramidSlice.DataPoint
				}
			};
			Faces faces = null;
			PyramidChart.Pyramid2DSlice pyramid2DSlice = new PyramidChart.Pyramid2DSlice
			{
				LT = new Point(topRadius - Math.Abs(pyramidSlice.TopRadius), 0.0),
				RT = new Point(topRadius + Math.Abs(pyramidSlice.TopRadius), 0.0),
				LB = new Point(topRadius - Math.Abs(pyramidSlice.BottomRadius), pyramidSlice.Height),
				RB = new Point(topRadius + Math.Abs(pyramidSlice.BottomRadius), pyramidSlice.Height)
			};
			PyramidChart.Pyramid3DSlice pyramid3DSlice = null;
			if (is3D)
			{
				double num = (double)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.Opacity) * pyramidSlice.DataPoint.Parent.Opacity;
				pyramid3DSlice = new PyramidChart.Pyramid3DSlice(pyramid2DSlice, Math.Abs(yScaleTop / 2.0), Math.Abs(yScaleBottom / 2.0), isLightingGradientLayer ? new SolidColorBrush(Colors.Transparent) : pyramidSlice.DataPoint.Color, pyramidSlice.Index == 0, (bool)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.LightingEnabled), num);
				List<Path> list = new List<Path>();
				Path face = pyramid3DSlice.GetFace(PyramidChart.Pyramid3dLayer.BackLayerLeft, pyramidSlice.DataPoint, !isLightingGradientLayer);
				Path face2 = pyramid3DSlice.GetFace(PyramidChart.Pyramid3dLayer.BackLayerRight, pyramidSlice.DataPoint, !isLightingGradientLayer);
				Path face3 = pyramid3DSlice.GetFace(PyramidChart.Pyramid3dLayer.BottomLayer, pyramidSlice.DataPoint, !isLightingGradientLayer);
				Path face4 = pyramid3DSlice.GetFace(PyramidChart.Pyramid3dLayer.FrontLayerLeft, pyramidSlice.DataPoint, !isLightingGradientLayer);
				Path face5 = pyramid3DSlice.GetFace(PyramidChart.Pyramid3dLayer.FrontLayerRight, pyramidSlice.DataPoint, !isLightingGradientLayer);
				Path face6 = pyramid3DSlice.GetFace(PyramidChart.Pyramid3dLayer.TopLayer, pyramidSlice.DataPoint, !isLightingGradientLayer);
				list.Add(face);
				list.Add(face2);
				list.Add(face3);
				list.Add(face4);
				list.Add(face5);
				list.Add(face6);
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.LightingEnabled) && !isLightingGradientLayer)
				{
					if (pyramidSlice.Index != 0)
					{
						list.Add(pyramid3DSlice.GetFace(PyramidChart.Pyramid3dLayer.LightingLayerLeft, pyramidSlice.DataPoint, !isLightingGradientLayer));
					}
					list.Add(pyramid3DSlice.GetFace(PyramidChart.Pyramid3dLayer.LightingLayerFront, pyramidSlice.DataPoint, !isLightingGradientLayer));
					if (pyramidSlice.Index != 0)
					{
						list.Add(pyramid3DSlice.GetFace(PyramidChart.Pyramid3dLayer.LightingLayerRight, pyramidSlice.DataPoint, !isLightingGradientLayer));
					}
				}
				foreach (Path current in list)
				{
					canvas2.Children.Add(current);
				}
				if (!isLightingGradientLayer)
				{
					faces = new Faces();
					foreach (Path current2 in list)
					{
						faces.VisualComponents.Add(current2);
					}
					foreach (Path current3 in list)
					{
						faces.Parts.Add(current3);
					}
					if (!(bool)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.LightingEnabled))
					{
						foreach (Path current4 in list)
						{
							if (current4 != face3 && current4 != face && current4 != face2)
							{
								faces.BorderElements.Add(current4);
							}
						}
						if (num < 1.0)
						{
							faces.BorderElements.Add(face3);
						}
					}
					pyramidSlice.DataPoint.DpInfo.Faces = faces;
				}
				else
				{
					faces = pyramidSlice.DataPoint.DpInfo.Faces;
					if (faces != null)
					{
						foreach (Path current5 in list)
						{
							if (current5 != face3 && current5 != face && current5 != face2)
							{
								faces.BorderElements.Add(current5);
							}
						}
						if (num < 1.0)
						{
							faces.BorderElements.Add(face3);
						}
					}
				}
				if (faces.BorderElements != null)
				{
					using (List<Shape>.Enumerator enumerator6 = faces.BorderElements.GetEnumerator())
					{
						while (enumerator6.MoveNext())
						{
							Path path = (Path)enumerator6.Current;
							path.StrokeThickness = ((Thickness)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.BorderThickness)).Left;
							path.Stroke = (Brush)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.BorderColor);
							path.StrokeDashArray = ExtendedGraphics.GetDashArray((BorderStyles)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.BorderStyle));
						}
					}
				}
				if (!animationEnabled)
				{
					goto IL_94B;
				}
				double beginTime = 0.0;
				using (List<Path>.Enumerator enumerator7 = list.GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						Path current6 = enumerator7.Current;
						if (pyramidSlice.DataPoint.IsLightDataPoint)
						{
							pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(current6, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime, (pyramidSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
						}
						else
						{
							pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(current6, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime, (pyramidSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
						}
					}
					goto IL_94B;
				}
			}
			PathGeometry pathGeometry = new PathGeometry();
			PathFigure pathFigure = new PathFigure
			{
				StartPoint = pyramid2DSlice.LT,
				IsClosed = true
			};
			Path path2 = new Path
			{
				Fill = sideFillColor
			};
			path2.Tag = new ElementData
			{
				Element = pyramidSlice.DataPoint
			};
			path2.Data = pathGeometry;
			path2.StrokeThickness = 0.0;
			path2.Stroke = new SolidColorBrush(Colors.Black);
			pathGeometry.Figures.Add(pathFigure);
			canvas2.Children.Add(path2);
			PolyLineSegment polyLineSegment = new PolyLineSegment();
			polyLineSegment.Points = pyramid2DSlice.GetAllPoints();
			pathFigure.Segments.Add(polyLineSegment);
			Point[] array = new Point[8];
			array[0] = new Point(topRadius - pyramidSlice.TopRadius, 0.0);
			array[1] = new Point(topRadius + pyramidSlice.TopRadius, 0.0);
			array[2] = new Point(topRadius + pyramidSlice.BottomRadius, pyramidSlice.Height);
			array[3] = new Point(topRadius - pyramidSlice.BottomRadius, pyramidSlice.Height);
			if (animationEnabled)
			{
				double beginTime2 = 0.0;
				if (pyramidSlice.DataPoint.IsLightDataPoint)
				{
					pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(path2, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime2, (pyramidSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
				}
				else
				{
					pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(path2, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime2, (pyramidSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
				}
			}
			if (!isLightingGradientLayer)
			{
				faces = new Faces();
				faces.VisualComponents.Add(path2);
				(path2.Tag as ElementData).VisualElementName = "PyramidBase";
				faces.Parts.Add(path2);
				faces.BorderElements.Add(path2);
				path2.StrokeThickness = ((Thickness)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.BorderThickness)).Left;
				path2.Stroke = (Brush)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.BorderColor);
				path2.StrokeDashArray = ExtendedGraphics.GetDashArray((BorderStyles)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.BorderStyle));
				pyramidSlice.DataPoint.DpInfo.Faces = faces;
				if (pyramidSlice.DataPoint.Parent.Bevel.Value)
				{
					PyramidChart.ApplyPyramidBevel(canvas2, pyramidSlice, sideFillColor, array);
				}
			}
			else
			{
				path2.IsHitTestVisible = false;
				pyramidSlice.DataPoint.DpInfo.Faces.VisualComponents.Add(path2);
				(path2.Tag as ElementData).VisualElementName = "Lighting";
			}
			IL_94B:
			if (isLightingGradientLayer)
			{
				canvas2.IsHitTestVisible = false;
			}
			else
			{
				Canvas canvas3 = PyramidChart.CreateLabelLine(pyramidSlice, pyramid3DSlice, topRadius, animationEnabled);
				if (canvas3 != null)
				{
					canvas.Children.Add(canvas3);
					faces.VisualComponents.Add(canvas3);
				}
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.LabelEnabled))
				{
					Canvas canvas4 = new Canvas();
					canvas4.SetValue(Panel.ZIndexProperty, 10);
					faces.VisualComponents.Add(pyramidSlice.DataPoint.DpInfo.LabelVisual);
					if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.LabelStyle) == LabelStyles.OutSide)
					{
						pyramidSlice.DataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, pyramidSlice.LabelLineEndPoint.Y - pyramidSlice.DataPoint.DpInfo.LabelVisual.Height / 2.0);
						pyramidSlice.DataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, pyramidSlice.LabelLineEndPoint.X + 2.0);
					}
					else
					{
						if (is3D)
						{
							Point point = Graphics.MidPointOfALine(pyramid3DSlice.Top3DLayer.CB, pyramid3DSlice.Bottom3DLayer.CB);
							pyramidSlice.DataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, point.Y - pyramidSlice.DataPoint.DpInfo.LabelVisual.Height / 2.0);
						}
						else
						{
							pyramidSlice.DataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, pyramidSlice.LabelLineEndPoint.Y - pyramidSlice.DataPoint.DpInfo.LabelVisual.Height / 2.0);
						}
						pyramidSlice.DataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, topRadius - pyramidSlice.DataPoint.DpInfo.LabelVisual.Width / 2.0);
					}
					if (animationEnabled)
					{
						double beginTime3 = 1.2;
						pyramidSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pyramidSlice.DataPoint.DpInfo.LabelVisual, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard, beginTime3, 0.5, 0.0, 1.0);
					}
					canvas4.Children.Add(pyramidSlice.DataPoint.DpInfo.LabelVisual);
					canvas.Children.Add(canvas4);
				}
			}
			canvas.Children.Add(canvas2);
			return canvas;
		}

		private static Canvas CreateLabelLine(TriangularChartSliceParms pyramidSlice, PyramidChart.Pyramid3DSlice pyramid3DSlice, double topRadius, bool animationEnabled)
		{
			Canvas canvas = null;
			Point r;
			Point r2;
			if (pyramid3DSlice == null)
			{
				r = new Point(topRadius + Math.Abs(pyramidSlice.TopRadius), 0.0);
				r2 = new Point(topRadius + Math.Abs(pyramidSlice.BottomRadius), pyramidSlice.Height);
			}
			else
			{
				r = pyramid3DSlice.Top3DLayer.R;
				r2 = pyramid3DSlice.Bottom3DLayer.R;
			}
			pyramidSlice.RightMidPoint = Graphics.MidPointOfALine(r, r2);
			pyramidSlice.LabelLineEndPoint = new Point(2.0 * topRadius, pyramidSlice.RightMidPoint.Y);
			pyramidSlice.OrginalLabelLineEndPoint = pyramidSlice.LabelLineEndPoint;
			if ((bool)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.LabelLineEnabled) && (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.LabelStyle) == LabelStyles.OutSide)
			{
				canvas = new Canvas();
				canvas.Width = topRadius * 2.0;
				canvas.Height = pyramidSlice.Height;
				pyramidSlice.DataPoint.DpInfo.LabelLine = null;
				Path path = new Path
				{
					Stroke = (Brush)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.LabelLineColor),
					Fill = (Brush)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.LabelLineColor),
					StrokeDashArray = ExtendedGraphics.GetDashArray((LineStyles)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.LabelLineStyle)),
					StrokeThickness = (double)DataPointHelper.GetDataPointValueFromProperty(pyramidSlice.DataPoint, VcProperties.LabelLineThickness)
				};
				PathGeometry pathGeometry = new PathGeometry();
				PathFigure pathFigure = new PathFigure
				{
					StartPoint = pyramidSlice.RightMidPoint
				};
				pathFigure.Segments.Add(new LineSegment
				{
					Point = pyramidSlice.LabelLineEndPoint
				});
				pathGeometry.Figures.Add(pathFigure);
				path.Data = pathGeometry;
				pyramidSlice.DataPoint.DpInfo.LabelLine = path;
				canvas.Children.Add(path);
				if (animationEnabled)
				{
					pyramidSlice.DataPoint.Parent.Storyboard = PyramidChart.ApplyLabeLineAnimation(canvas, pyramidSlice.DataPoint.Parent, pyramidSlice.DataPoint.Parent.Storyboard);
				}
			}
			return canvas;
		}

		private static Storyboard ApplyLabeLineAnimation(Panel canvas, DataSeries dataSeries, Storyboard storyboard)
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush
			{
				StartPoint = new Point(0.0, 0.5),
				EndPoint = new Point(1.0, 0.5)
			};
			GradientStop value = new GradientStop
			{
				Color = Colors.White,
				Offset = 0.0
			};
			GradientStop gradientStop = new GradientStop
			{
				Color = Colors.White,
				Offset = 0.0
			};
			GradientStop gradientStop2 = new GradientStop
			{
				Color = Colors.Transparent,
				Offset = 0.01
			};
			GradientStop value2 = new GradientStop
			{
				Color = Colors.Transparent,
				Offset = 1.0
			};
			linearGradientBrush.GradientStops.Add(value);
			linearGradientBrush.GradientStops.Add(gradientStop);
			linearGradientBrush.GradientStops.Add(gradientStop2);
			linearGradientBrush.GradientStops.Add(value2);
			canvas.OpacityMask = linearGradientBrush;
			double beginTime = 1.0;
			DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				1.0
			});
			DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				0.5
			});
			List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0)
			});
			storyboard.Children.Add(AnimationHelper.CreateDoubleAnimation(dataSeries, gradientStop, "(GradientStop.Offset)", beginTime, frameTime, values, splines));
			values = Graphics.GenerateDoubleCollection(new double[]
			{
				0.01,
				1.0
			});
			frameTime = Graphics.GenerateDoubleCollection(new double[]
			{
				0.0,
				0.5
			});
			splines = AnimationHelper.GenerateKeySplineList(new Point[]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 0.0),
				new Point(1.0, 1.0)
			});
			storyboard.Children.Add(AnimationHelper.CreateDoubleAnimation(dataSeries, gradientStop2, "(GradientStop.Offset)", beginTime, frameTime, values, splines));
			return storyboard;
		}

		private static void CalculateBevelInnerPoints(TriangularChartSliceParms pyramidSlice, Point[] points, bool isTopOfPyramid)
		{
			double num = 3.0;
			double num2 = num * Math.Sin(pyramidSlice.TopAngle / 2.0);
			double num3 = num * Math.Cos(pyramidSlice.TopAngle / 2.0);
			if (isTopOfPyramid)
			{
				points[4] = new Point(points[0].X, points[0].Y + num);
				points[5] = new Point(points[1].X, points[1].Y + num);
			}
			else
			{
				points[4] = new Point(points[0].X - num2, num);
				points[5] = new Point(points[1].X + num2, num);
			}
			num3 = num * Math.Tan(pyramidSlice.BottomAngle / 2.0);
			num = 2.0;
			points[6] = new Point(points[2].X + num3, points[2].Y - num);
			points[7] = new Point(points[3].X - num3, points[3].Y - num);
		}

		private static Brush GetSideLightingBrush(TriangularChartSliceParms pyramidSlice)
		{
			List<double> list = new List<double>();
			List<Color> list2 = new List<Color>();
			list2.Add(Color.FromArgb(0, 0, 0, 0));
			list.Add(0.491);
			list2.Add(Color.FromArgb(135, 8, 8, 8));
			list.Add(0.938);
			list2.Add(Color.FromArgb(71, 71, 71, 71));
			list.Add(0.0);
			return Graphics.CreateLinearGradientBrush(0.0, new Point(-0.1, 0.5), new Point(1.0, 0.5), list2, list);
		}

		private static Brush GetTopBrush(Brush fillBrush, TriangularChartSliceParms pyramidSlice)
		{
			if (fillBrush is SolidColorBrush)
			{
				SolidColorBrush solidColorBrush = fillBrush as SolidColorBrush;
				double num = (double)solidColorBrush.Color.R / 255.0 * 0.9999;
				double num2 = (double)solidColorBrush.Color.G / 255.0 * 0.9999;
				double num3 = (double)solidColorBrush.Color.B / 255.0 * 0.9999;
				LinearGradientBrush linearGradientBrush = null;
				if (pyramidSlice.FillType == FillType.Solid)
				{
					linearGradientBrush = new LinearGradientBrush
					{
						StartPoint = new Point(0.0, 0.0),
						EndPoint = new Point(1.0, 1.0)
					};
					linearGradientBrush.GradientStops.Add(new GradientStop
					{
						Color = Graphics.GetLighterColor(solidColorBrush.Color, 1.0 - num, 1.0 - num2, 1.0 - num3),
						Offset = 0.0
					});
					linearGradientBrush.GradientStops.Add(new GradientStop
					{
						Color = solidColorBrush.Color,
						Offset = 0.9
					});
					linearGradientBrush.GradientStops.Add(new GradientStop
					{
						Color = Graphics.GetDarkerColor(solidColorBrush.Color, 1.0),
						Offset = 0.99
					});
					linearGradientBrush.Opacity = 1.0;
				}
				else if (pyramidSlice.FillType == FillType.Hollow)
				{
					linearGradientBrush = new LinearGradientBrush
					{
						StartPoint = new Point(0.233, 0.297),
						EndPoint = new Point(0.757, 0.495)
					};
					linearGradientBrush.GradientStops.Add(new GradientStop
					{
						Offset = 0.0,
						Color = Graphics.GetDarkerColor(solidColorBrush.Color, 0.5)
					});
					linearGradientBrush.GradientStops.Add(new GradientStop
					{
						Offset = 0.7,
						Color = solidColorBrush.Color
					});
					linearGradientBrush.GradientStops.Add(new GradientStop
					{
						Offset = 1.0,
						Color = Graphics.GetDarkerColor(solidColorBrush.Color, 0.8)
					});
				}
				return linearGradientBrush;
			}
			return fillBrush;
		}
	}
}
