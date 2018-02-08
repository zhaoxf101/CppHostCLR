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
	internal static class FunnelChart
	{
		private const double TITLE_FUNNEL_GAP = 10.0;

		private static Random rand = new Random();

		private static double _singleGap = 0.0;

		private static double _totalGap = 0.0;

		private static Size _streamLineParentTitleSize;

		public static Grid GetVisualObjectForFunnelChart(double width, double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, bool animationEnabled, bool isStreamLine)
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
			List<IDataPoint> list3;
			if (isStreamLine)
			{
				if (list2.Count <= 1)
				{
					throw new Exception("Invalid DataSet. StreamLineFunnel chart must have more than one DataPoint in a DataSeries with YValue > 0.");
				}
				list3 = (from dp in list2
				orderby dp.YValue descending
				select dp).ToList<IDataPoint>();
			}
			else
			{
				list3 = list2.ToList<IDataPoint>();
			}
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
			FunnelChart.CreateLabelsAndSetFunnelCanvasSize(isStreamLine, grid, canvas, canvas2, list3);
			double minPointHeight = dataSeries.MinPointHeight;
			double yScale = 40.0;
			bool isSameSlantAngle = true;
			double bottomRadius = 5.0;
			double gapRatio = chart.View3D ? 0.04 : 0.02;
			canvas2 = FunnelChart.CreateFunnelChart(grid, dataSeries, list3, isStreamLine, canvas2, minPointHeight, chart.View3D, yScale, gapRatio, isSameSlantAngle, bottomRadius, animationEnabled);
			if (!chart.InternalAnimationEnabled || chart.IsInDesignMode || !chart.ChartArea._isFirstTimeRender)
			{
				foreach (IDataPoint current2 in list3)
				{
					if (current2.DpInfo.Faces != null)
					{
						foreach (Shape current3 in current2.DpInfo.Faces.BorderElements)
						{
							if (current2.IsLightDataPoint)
							{
								InteractivityHelper.ApplyBorderEffect(current3, (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(current2, VcProperties.BorderStyle), (current2 as LightDataPoint).Parent.BorderThickness.Left, (Brush)DataPointHelper.GetDataPointValueFromProperty(current2, VcProperties.BorderColor));
							}
							else
							{
								InteractivityHelper.ApplyBorderEffect(current3, (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(current2, VcProperties.BorderStyle), (current2 as DataPoint).InternalBorderThickness.Left, (Brush)DataPointHelper.GetDataPointValueFromProperty(current2, VcProperties.BorderColor));
							}
						}
					}
				}
			}
			grid.Clip = new RectangleGeometry
			{
				Rect = new Rect(0.0, 0.0, width, chart.PlotArea.Visual.Height)
			};
			return grid;
		}

		private static void CreateLabelsAndSetFunnelCanvasSize(bool isStreamLine, Grid funnelChartCanvas, Canvas labelCanvas, Canvas funnelCanvas, List<IDataPoint> funnelDataPoints)
		{
			int i = 0;
			double num = 0.0;
			FunnelChart._streamLineParentTitleSize = new Size(0.0, 0.0);
			labelCanvas.Width = 0.0;
			while (i < funnelDataPoints.Count)
			{
				funnelDataPoints[i].DpInfo.LabelVisual = FunnelChart.CreateLabelForDataPoint(funnelDataPoints[i], isStreamLine, i);
				Size streamLineParentTitleSize = Graphics.CalculateVisualSize(funnelDataPoints[i].DpInfo.LabelVisual);
				streamLineParentTitleSize.Width += 2.5;
				if (isStreamLine && i == 0)
				{
					streamLineParentTitleSize.Height += 10.0;
				}
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(funnelDataPoints[i], VcProperties.LabelEnabled) && (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(funnelDataPoints[i], VcProperties.LabelStyle) == LabelStyles.OutSide)
				{
					num += streamLineParentTitleSize.Height;
				}
				if (isStreamLine && i == 0)
				{
					FunnelChart._streamLineParentTitleSize = streamLineParentTitleSize;
				}
				else if (streamLineParentTitleSize.Width > labelCanvas.Width && (bool)DataPointHelper.GetDataPointValueFromProperty(funnelDataPoints[i], VcProperties.LabelEnabled) && (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(funnelDataPoints[i], VcProperties.LabelStyle) == LabelStyles.OutSide)
				{
					labelCanvas.Width = streamLineParentTitleSize.Width;
				}
				funnelDataPoints[i].DpInfo.LabelVisual.Height = streamLineParentTitleSize.Height;
				funnelDataPoints[i].DpInfo.LabelVisual.Width = streamLineParentTitleSize.Width;
				i++;
			}
			labelCanvas.Width += 5.0;
			double arg_1AF_0 = labelCanvas.Width;
			double arg_1AE_0 = 0.6 * funnelChartCanvas.Width;
			funnelCanvas.Width = Math.Max(funnelChartCanvas.Width - labelCanvas.Width, 0.0);
			labelCanvas.SetValue(Canvas.LeftProperty, funnelCanvas.Width);
			if (isStreamLine)
			{
				funnelCanvas.Height -= FunnelChart._streamLineParentTitleSize.Height;
				labelCanvas.Height -= FunnelChart._streamLineParentTitleSize.Height;
				funnelCanvas.SetValue(Canvas.TopProperty, FunnelChart._streamLineParentTitleSize.Height);
				labelCanvas.SetValue(Canvas.TopProperty, FunnelChart._streamLineParentTitleSize.Height);
				funnelDataPoints[0].DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, (funnelCanvas.Width - FunnelChart._streamLineParentTitleSize.Width) / 2.0);
				funnelDataPoints[0].DpInfo.Faces = new Faces();
				funnelDataPoints[0].DpInfo.Faces.VisualComponents.Add(funnelDataPoints[0].DpInfo.LabelVisual);
				funnelDataPoints[0].DpInfo.Faces.Visual = funnelDataPoints[0].DpInfo.LabelVisual;
				bool arg_311_0 = (funnelDataPoints[0].Chart as Chart).InternalAnimationEnabled;
			}
		}

		private static Border CreateLabelForDataPoint(IDataPoint dataPoint, bool isStreamLine, int sliceIndex)
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
				InternalFontColor = ((isStreamLine && sliceIndex == 0) ? Chart.CalculateDataPointLabelFontColor(dataPoint.Chart as Chart, dataPoint, null, LabelStyles.OutSide) : Chart.CalculateDataPointLabelFontColor(dataPoint.Chart as Chart, dataPoint, labelFontColor, labelStyle)),
				InternalFontFamily = internalFontFamily,
				InternalFontStyle = internalFontStyle,
				InternalFontWeight = internalFontWeight,
				InternalBackground = internalBackground
			};
			if (isStreamLine && sliceIndex == 0 && !DataPointHelper.GetLabelFontSize_DataPoint(dataPoint).HasValue && dataPoint.Parent.GetValue(DataPoint.LabelFontSizeProperty) == null)
			{
				title.InternalFontSize += 1.0;
			}
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

		private static Canvas CreateFunnelChart(Grid _funnelChartGrid, DataSeries dataSeries, List<IDataPoint> dataPoints, bool isStreamLine, Canvas funnelCanvas, double minPointHeight, bool is3D, double yScale, double gapRatio, bool isSameSlantAngle, double bottomRadius, bool animationEnabled)
		{
			bool arg_10_0 = (dataSeries.Chart as Chart).InternalAnimationEnabled;
			double height = funnelCanvas.Height;
			double width = funnelCanvas.Width;
			TriangularChartSliceParms[] array = FunnelChart.CalculateFunnelSliceParmsInfo(isStreamLine, dataSeries, dataPoints, height, width - 5.0, minPointHeight, is3D, yScale, gapRatio, isSameSlantAngle, bottomRadius);
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
				Canvas funnelSliceVisual = FunnelChart.GetFunnelSliceVisual(i, num, is3D, array[i], num5, num6, color, animationEnabled);
				array[i].Top = array[i].TopGap + ((i == 0) ? 0.0 : (array[i - 1].Top + array[i - 1].Height + array[i - 1].BottomGap));
				funnelSliceVisual.SetValue(Canvas.TopProperty, array[i].Top);
				funnelSliceVisual.SetValue(Panel.ZIndexProperty, num2--);
				funnelSliceVisual.Height = array[i].Height;
				funnelSliceVisual.Width = num * 2.0;
				num4 += array[i].Height + array[i].TopGap;
				funnelCanvas.Children.Add(funnelSliceVisual);
				if (isStreamLine && i == 0)
				{
					funnelSliceVisual.Children.Add(dataPoints[0].DpInfo.LabelVisual);
					dataPoints[0].DpInfo.Faces.Visual = dataPoints[0].DpInfo.LabelVisual;
					dataPoints[0].DpInfo.LabelVisual.SetValue(Canvas.TopProperty, -(FunnelChart._streamLineParentTitleSize.Height + (is3D ? (yScale / 2.0) : 0.0)));
					array[i].DataPoint.DpInfo.VisualParams = null;
				}
				array[i].DataPoint.DpInfo.Faces.Visual = funnelSliceVisual;
				array[i].DataPoint.DpInfo.VisualParams = array[i];
			}
			FunnelChart.CalcutateExplodedPosition(ref array, isStreamLine, yScale, dataSeries);
			funnelCanvas.Height = Math.Max(num4 - FunnelChart._streamLineParentTitleSize.Height, 0.0);
			FunnelChart.ArrangeLabels(array, double.NaN, _funnelChartGrid.Height);
			return funnelCanvas;
		}

		internal static void ArrangeLabelsOnExpload(TriangularChartSliceParms[] funnelSlices, IDataPoint dataPoint, double width, double height)
		{
			if (funnelSlices == null || funnelSlices.Length < 0)
			{
				return;
			}
			FunnelChart.ArrangeLabels(funnelSlices, double.NaN, height);
			TriangularChartSliceParms[] array = (from fs in funnelSlices
			where (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(fs.DataPoint, VcProperties.LabelStyle) == LabelStyles.OutSide
			select fs).ToArray<TriangularChartSliceParms>();
			Rect area = new Rect(0.0, 0.0, width, height);
			Rect[] array2 = new Rect[array.Length];
			List<Point> list = (from ex in array
			where ex.DataPoint == dataPoint
			select ex.ExplodedPoints).First<List<Point>>();
			double num = 0.0;
			for (int i = 0; i < array.Length; i++)
			{
				double x = (double)array[i].DataPoint.DpInfo.LabelVisual.GetValue(Canvas.LeftProperty);
				double num2 = array[i].OrginalLabelLineEndPoint.Y - array[i].DataPoint.DpInfo.LabelVisual.Height / 2.0;
				double num3 = list[i].Y + num2;
				if (num3 + array[i].DataPoint.DpInfo.LabelVisual.Height + num > area.Height)
				{
					num3 -= num3 + array[i].DataPoint.DpInfo.LabelVisual.Height + num - area.Height;
				}
				array2[i] = new Rect(x, num3, array[i].DataPoint.DpInfo.LabelVisual.Width, array[i].DataPoint.DpInfo.LabelVisual.Height);
			}
			LabelPlacementHelper.VerticalLabelPlacement(area, ref array2);
			for (int j = 0; j < array.Length; j++)
			{
				double num4 = array2[j].Top - list[j].Y;
				array[j].DataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, array2[j].Left);
				array[j].DataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, num4);
				array[j].LabelLineEndPoint = new Point(array[j].LabelLineEndPoint.X, num4 + array[j].DataPoint.DpInfo.LabelVisual.Height / 2.0);
				FunnelChart.UpdateLabelLineEndPoint(array[j]);
			}
		}

		internal static void ArrangeLabels(TriangularChartSliceParms[] funnelSlices, double width, double height)
		{
			if (funnelSlices == null || funnelSlices.Length < 0)
			{
				return;
			}
			TriangularChartSliceParms[] array = (from fs in funnelSlices
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
				FunnelChart.UpdateLabelLineEndPoint(array[j]);
			}
		}

		private static void UpdateLabelLineEndPoint(TriangularChartSliceParms funnelSlice)
		{
			Path labelLine = funnelSlice.DataPoint.DpInfo.LabelLine;
			if (labelLine != null && labelLine.Data != null)
			{
				LineSegment lineSegment = (labelLine.Data as PathGeometry).Figures[0].Segments[0] as LineSegment;
				lineSegment.Point = funnelSlice.LabelLineEndPoint;
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

		private static void CalcutateExplodedPosition(ref TriangularChartSliceParms[] funnelSlices, bool isStreamLine, double yScale, DataSeries dataSeries)
		{
			int num = funnelSlices.Count<TriangularChartSliceParms>();
			int i = 0;
			if (funnelSlices[0].DataPoint.Parent.Exploded)
			{
				if (!funnelSlices[0].DataPoint.Chart.IsInDesignMode)
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
						double targetValue = funnelSlices[i].Top - (double)(num2 - i) * FunnelChart._singleGap;
						dataSeries.Storyboard = FunnelChart.CreateExplodingAnimation(dataSeries, funnelSlices[i].DataPoint, dataSeries.Storyboard, funnelSlices[i].DataPoint.DpInfo.Faces.Visual as Panel, targetValue, beginTime);
						i--;
					}
					for (i = num2 + 1; i < num; i++)
					{
						double targetValue2 = funnelSlices[i].Top + (double)(i - num2) * FunnelChart._singleGap;
						dataSeries.Storyboard = FunnelChart.CreateExplodingAnimation(dataSeries, funnelSlices[i].DataPoint, dataSeries.Storyboard, funnelSlices[i].DataPoint.DpInfo.Faces.Visual as Panel, targetValue2, beginTime);
					}
					if (dataSeries.Storyboard != null && dataSeries.Chart != null && !(dataSeries.Chart as Chart).ChartArea._isFirstTimeRender)
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
					funnelSlices[i].ExplodedPoints = new List<Point>();
					funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation = new Storyboard();
					if (i == 0)
					{
						funnelSlices[i].ExplodedPoints.Add(new Point(0.0, funnelSlices[i].Top - FunnelChart._singleGap / 2.0));
						funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation = FunnelChart.CreateExplodingAnimation(dataSeries, funnelSlices[i].DataPoint, funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation, funnelSlices[i].DataPoint.DpInfo.Faces.Visual as Panel, funnelSlices[i].Top - FunnelChart._singleGap / 2.0, 0.0);
						for (int j = 1; j < funnelSlices.Length; j++)
						{
							funnelSlices[i].ExplodedPoints.Add(new Point(0.0, funnelSlices[j].Top + FunnelChart._singleGap / 2.0));
							funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation = FunnelChart.CreateExplodingAnimation(dataSeries, funnelSlices[i].DataPoint, funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation, funnelSlices[j].DataPoint.DpInfo.Faces.Visual as Panel, funnelSlices[j].Top + FunnelChart._singleGap / 2.0, 0.0);
						}
					}
					else if (i == funnelSlices.Length - 1)
					{
						int k;
						for (k = 0; k < i; k++)
						{
							funnelSlices[i].ExplodedPoints.Add(new Point(0.0, funnelSlices[k].Top - FunnelChart._singleGap / 2.0 + FunnelChart._singleGap / 6.0));
							funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation = FunnelChart.CreateExplodingAnimation(dataSeries, funnelSlices[i].DataPoint, funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation, funnelSlices[k].DataPoint.DpInfo.Faces.Visual as Panel, funnelSlices[k].Top - FunnelChart._singleGap / 2.0 + FunnelChart._singleGap / 6.0, 0.0);
						}
						funnelSlices[i].ExplodedPoints.Add(new Point(0.0, funnelSlices[k].Top + FunnelChart._singleGap / 2.0 + FunnelChart._singleGap / 6.0));
						funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation = FunnelChart.CreateExplodingAnimation(dataSeries, funnelSlices[i].DataPoint, funnelSlices[k].DataPoint.DpInfo.ExplodeAnimation, funnelSlices[k].DataPoint.DpInfo.Faces.Visual as Panel, funnelSlices[k].Top + FunnelChart._singleGap / 2.0 + FunnelChart._singleGap / 6.0, 0.0);
					}
					else
					{
						int l;
						for (l = 0; l < i; l++)
						{
							funnelSlices[i].ExplodedPoints.Add(new Point(0.0, funnelSlices[l].Top - FunnelChart._singleGap / 2.0));
							funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation = FunnelChart.CreateExplodingAnimation(dataSeries, funnelSlices[i].DataPoint, funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation, funnelSlices[l].DataPoint.DpInfo.Faces.Visual as Panel, funnelSlices[l].Top - FunnelChart._singleGap / 2.0, 0.0);
						}
						funnelSlices[i].ExplodedPoints.Add(new Point(0.0, funnelSlices[l].Top));
						funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation = FunnelChart.CreateExplodingAnimation(dataSeries, funnelSlices[i].DataPoint, funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation, funnelSlices[i].DataPoint.DpInfo.Faces.Visual as Panel, funnelSlices[i].Top, 0.0);
						for (l++; l < funnelSlices.Length; l++)
						{
							funnelSlices[i].ExplodedPoints.Add(new Point(0.0, funnelSlices[l].Top + FunnelChart._singleGap / 2.0));
							funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation = FunnelChart.CreateExplodingAnimation(dataSeries, funnelSlices[i].DataPoint, funnelSlices[i].DataPoint.DpInfo.ExplodeAnimation, funnelSlices[l].DataPoint.DpInfo.Faces.Visual as Panel, funnelSlices[l].Top + FunnelChart._singleGap / 2.0, 0.0);
						}
					}
					i++;
				}
			}
		}

		private static TriangularChartSliceParms[] CalculateFunnelSliceParmsInfo(bool isStreamLine, DataSeries dataSeries, List<IDataPoint> dataPoints, double plotHeight, double plotWidth, double minPointHeight, bool is3D, double yScale, double gapRatio, bool isSameSlantAngle, double bottomRadius)
		{
			plotHeight = Math.Max(plotHeight, 0.0);
			plotWidth = Math.Max(plotWidth, 0.0);
			if (dataSeries.Exploded)
			{
				gapRatio = 0.02;
				FunnelChart._singleGap = gapRatio * plotHeight;
				FunnelChart._totalGap = FunnelChart._singleGap * (double)(isStreamLine ? dataPoints.Count : (dataPoints.Count + 1));
			}
			else
			{
				FunnelChart._singleGap = gapRatio * plotHeight;
				FunnelChart._totalGap = FunnelChart._singleGap * 2.0;
			}
			double num = plotHeight - FunnelChart._totalGap - (is3D ? (yScale / 2.0) : 0.0);
			TriangularChartSliceParms[] array;
			if (!isStreamLine)
			{
				array = new TriangularChartSliceParms[dataPoints.Count];
				double num2 = (from dp in dataPoints
				select dp.YValue).Sum();
				(from dp in dataPoints
				select dp.YValue).Min();
				double num3 = Math.Atan((plotWidth / 2.0 - bottomRadius) / num);
				num = Math.Max(num - Math.Tan(num3) / ((bottomRadius == 0.0) ? 1.0 : bottomRadius), 0.0);
				for (int i = 0; i < dataPoints.Count; i++)
				{
					array[i] = new TriangularChartSliceParms
					{
						DataPoint = dataPoints[i],
						TopAngle = 1.5707963267948966 - num3,
						BottomAngle = 1.5707963267948966 + num3
					};
					array[i].Height = num * (dataPoints[i].YValue / num2);
					array[i].TopRadius = ((i == 0) ? (plotWidth / 2.0) : array[i - 1].BottomRadius);
					array[i].BottomRadius = array[i].TopRadius - array[i].Height * Math.Tan(num3);
					array[i].TopGap = ((i == 0 && is3D) ? (yScale / 2.0) : 0.0);
				}
				if (!double.IsNaN(minPointHeight))
				{
					bool flag = false;
					double num4 = (from funnelSlice in array
					select funnelSlice.Height).Sum();
					double num5 = num4 / (double)array.Length;
					minPointHeight = minPointHeight / 100.0 * num;
					List<TriangularChartSliceParms> fixedHeightFunnelSlices = (from funnelSlice in array
					where funnelSlice.Height < minPointHeight
					select funnelSlice).ToList<TriangularChartSliceParms>();
					List<TriangularChartSliceParms> source = (from funnelSlice in array
					where !(from slice in fixedHeightFunnelSlices
					select slice).Contains(funnelSlice)
					select funnelSlice).ToList<TriangularChartSliceParms>();
					if (minPointHeight > num5 || fixedHeightFunnelSlices.Count == array.Count<TriangularChartSliceParms>())
					{
						flag = true;
					}
					double num6 = (from funnelSlice in fixedHeightFunnelSlices
					select funnelSlice.Height).Sum();
					double num7 = (from funnelSlice in source
					select funnelSlice.Height).Sum();
					double num8 = minPointHeight * (double)fixedHeightFunnelSlices.Count<TriangularChartSliceParms>() - num6;
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
						array[j].TopRadius = ((j == 0) ? (plotWidth / 2.0) : array[j - 1].BottomRadius);
						array[j].BottomRadius = array[j].TopRadius - array[j].Height * Math.Tan(num3);
					}
				}
			}
			else
			{
				num = Math.Max(num - FunnelChart._streamLineParentTitleSize.Height, 0.0);
				IOValuePair[] array2 = new IOValuePair[dataPoints.Count];
				for (int k = 0; k < dataPoints.Count; k++)
				{
					array2[k] = default(IOValuePair);
					array2[k].InputValue = ((k == 0) ? double.NaN : dataPoints[k - 1].YValue);
					array2[k].OutPutValue = dataPoints[k].YValue;
				}
				array = new TriangularChartSliceParms[dataPoints.Count - 1];
				for (int l = 1; l < array2.Count<IOValuePair>(); l++)
				{
					int num9 = l - 1;
					array[num9] = new TriangularChartSliceParms
					{
						DataPoint = dataPoints[l]
					};
					array[num9].Height = (array2[l].InputValue - array2[l].OutPutValue) / array2[0].OutPutValue * num;
					array[num9].TopRadius = ((num9 == 0) ? (plotWidth / 2.0) : array[num9 - 1].BottomRadius);
					if (!isSameSlantAngle)
					{
						array[num9].BottomRadius = array[num9].TopRadius * Math.Sqrt(array2[l].OutPutValue / array2[l].InputValue);
					}
					else
					{
						array[num9].BottomRadius = array[num9].TopRadius * (array2[l].OutPutValue / array2[l].InputValue);
					}
					double num10 = Math.Atan((array[num9].TopRadius - array[num9].BottomRadius) / array[num9].Height);
					array[num9].TopAngle = 1.5707963267948966 - num10;
					array[num9].BottomAngle = 1.5707963267948966 + num10;
					FunnelChart.FixTopAndBottomRadiusForStreamLineFunnel(ref array[num9]);
				}
				double num11 = (from funnelSlice in array
				select funnelSlice.Height).Sum();
				if (num11 < num)
				{
					for (int m = 1; m < array2.Count<IOValuePair>(); m++)
					{
						int num9 = m - 1;
						array[num9].Height += (num - num11) * (array[num9].Height / num11);
						array[num9].TopRadius = ((num9 == 0) ? (plotWidth / 2.0) : array[num9 - 1].BottomRadius);
						if (!isSameSlantAngle)
						{
							array[num9].BottomRadius = Math.Round(array[num9].TopRadius * Math.Sqrt(array2[m].OutPutValue / array2[m].InputValue));
						}
						else
						{
							array[num9].BottomRadius = Math.Round(array[num9].TopRadius * (array2[m].OutPutValue / array2[m].InputValue));
						}
						double num12 = Math.Atan((array[num9].TopRadius - array[num9].BottomRadius) / array[num9].Height);
						array[num9].TopAngle = 1.5707963267948966 - num12;
						array[num9].BottomAngle = 1.5707963267948966 + num12;
						FunnelChart.FixTopAndBottomRadiusForStreamLineFunnel(ref array[num9]);
					}
				}
				if (!double.IsNaN(minPointHeight))
				{
					bool flag2 = false;
					double num13 = num - FunnelChart._streamLineParentTitleSize.Height;
					double num14 = num13 / (double)array.Length;
					minPointHeight = minPointHeight / 100.0 * num;
					IEnumerable<TriangularChartSliceParms> fixedHeightFunnelSlices = from funnelSlice in array
					where funnelSlice.Height < minPointHeight
					select funnelSlice;
					IEnumerable<TriangularChartSliceParms> source2 = from funnelSlice in array
					where !(from slice in fixedHeightFunnelSlices
					select slice).Contains(funnelSlice)
					select funnelSlice;
					if (minPointHeight > num14 || fixedHeightFunnelSlices.Count<TriangularChartSliceParms>() == array.Count<TriangularChartSliceParms>())
					{
						flag2 = true;
					}
					double num15 = (from funnelSlice in fixedHeightFunnelSlices
					select funnelSlice.Height).Sum();
					double num16 = (from funnelSlice in source2
					select funnelSlice.Height).Sum();
					double num17 = minPointHeight * (double)fixedHeightFunnelSlices.Count<TriangularChartSliceParms>() - num15;
					for (int n = 1; n < array2.Count<IOValuePair>(); n++)
					{
						int num9 = n - 1;
						if (flag2)
						{
							array[num9].Height = num14;
						}
						else if (array[num9].Height < minPointHeight)
						{
							array[num9].Height = minPointHeight;
						}
						else
						{
							array[num9].Height -= num17 * (array[num9].Height / num16);
						}
						array[num9].TopRadius = ((num9 == 0) ? (plotWidth / 2.0) : array[num9 - 1].BottomRadius);
						if (!isSameSlantAngle)
						{
							array[num9].BottomRadius = Math.Round(array[num9].TopRadius * Math.Sqrt(array2[n].OutPutValue / array2[n].InputValue));
						}
						else
						{
							array[num9].BottomRadius = Math.Round(array[num9].TopRadius * (array2[n].OutPutValue / array2[n].InputValue));
						}
						FunnelChart.FixTopAndBottomRadiusForStreamLineFunnel(ref array[num9]);
						double num18 = Math.Atan((array[num9].TopRadius - array[num9].BottomRadius) / array[num9].Height);
						array[num9].TopAngle = 1.5707963267948966 - num18;
						array[num9].BottomAngle = 1.5707963267948966 + num18;
					}
				}
			}
			return array;
		}

		public static void FixTopAndBottomRadiusForStreamLineFunnel(ref TriangularChartSliceParms funnelSlice)
		{
			if (double.IsNaN(funnelSlice.TopRadius))
			{
				funnelSlice.TopRadius = 1E-08;
				funnelSlice.Height = 0.0;
			}
			if (double.IsNaN(funnelSlice.BottomRadius))
			{
				funnelSlice.BottomRadius = 1E-07;
			}
			if (double.IsNaN(funnelSlice.Height))
			{
				funnelSlice.Height = 1E-07;
			}
		}

		private static Canvas GetFunnelSliceVisual(int funnelSliceIndex, double topRadius, bool is3D, TriangularChartSliceParms funnelSlice, double yScaleTop, double yScaleBottom, Brush fillColor, bool animationEnabled)
		{
			funnelSlice.Index = funnelSliceIndex;
			Canvas canvas = FunnelChart.CreateFunnelSlice(false, topRadius, is3D, funnelSlice, yScaleTop, yScaleBottom, fillColor, fillColor, fillColor, animationEnabled);
			if ((bool)DataPointHelper.GetDataPointValueFromProperty(funnelSlice.DataPoint, VcProperties.LightingEnabled))
			{
				Brush lightingBrushForStroke = FunnelChart.GetLightingBrushForStroke(fillColor, funnelSlice.Index);
				Brush sideFillColor = ((bool)DataPointHelper.GetDataPointValueFromProperty(funnelSlice.DataPoint, VcProperties.LightingEnabled)) ? FunnelChart.GetSideLightingBrush(funnelSlice) : fillColor;
				Brush topFillColor = is3D ? FunnelChart.GetTopBrush(fillColor, funnelSlice) : fillColor;
				Canvas element = FunnelChart.CreateFunnelSlice(true, topRadius, is3D, funnelSlice, yScaleTop, yScaleBottom, sideFillColor, topFillColor, lightingBrushForStroke, animationEnabled);
				canvas.Children.Add(element);
			}
			return canvas;
		}

		private static Brush GetLightingBrushForStroke(Brush fillColor, int funnelSliceIndex)
		{
			SolidColorBrush solidColorBrush = fillColor as SolidColorBrush;
			Brush result = fillColor;
			if (funnelSliceIndex > 0 && solidColorBrush != null)
			{
				result = new SolidColorBrush(Graphics.GetLighterColor(solidColorBrush.Color, 100.0));
			}
			else if (solidColorBrush != null)
			{
				result = Graphics.GetBevelTopBrush(new SolidColorBrush(Graphics.GetLighterColor(solidColorBrush.Color, 100.0)), 0.12);
			}
			return result;
		}

		internal static void ReCalculateAndApplyTheNewBrush(Shape shape, Brush newBrush, bool isLightingEnabled, bool is3D, TriangularChartSliceParms funnelSliceParms)
		{
			string visualElementName;
			switch (visualElementName = (shape.Tag as ElementData).VisualElementName)
			{
			case "FunnelBase":
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
				shape.Fill = (isLightingEnabled ? FunnelChart.GetSideLightingBrush(funnelSliceParms) : newBrush);
				return;
			case "FunnelTopLighting":
				shape.Fill = (is3D ? FunnelChart.GetTopBrush(newBrush, funnelSliceParms) : newBrush);
				shape.Stroke = FunnelChart.GetLightingBrushForStroke(newBrush, funnelSliceParms.Index);
				break;

				return;
			}
		}

		private static void ApplyFunnelBevel(Canvas parentVisual, TriangularChartSliceParms funnelSlice, Brush sideFillColor, Point[] points)
		{
			if (funnelSlice.DataPoint.Parent.Bevel.Value && funnelSlice.Height > 5.0)
			{
				FunnelChart.CalculateBevelInnerPoints(funnelSlice, points);
				Path pathFromPoints = ExtendedGraphics.GetPathFromPoints(Graphics.GetBevelTopBrush(sideFillColor, 90.0), new Point[]
				{
					points[0],
					points[4],
					points[5],
					points[1]
				});
				Path pathFromPoints2 = ExtendedGraphics.GetPathFromPoints(Graphics.GetBevelSideBrush(45.0, sideFillColor), new Point[]
				{
					points[0],
					points[4],
					points[7],
					points[3]
				});
				Path pathFromPoints3 = ExtendedGraphics.GetPathFromPoints(Graphics.GetBevelSideBrush(45.0, sideFillColor), new Point[]
				{
					points[1],
					points[5],
					points[6],
					points[2]
				});
				Path pathFromPoints4 = ExtendedGraphics.GetPathFromPoints(Graphics.GetBevelSideBrush(180.0, sideFillColor), new Point[]
				{
					points[7],
					points[6],
					points[2],
					points[3]
				});
				pathFromPoints.IsHitTestVisible = false;
				pathFromPoints2.IsHitTestVisible = false;
				pathFromPoints3.IsHitTestVisible = false;
				pathFromPoints4.IsHitTestVisible = false;
				parentVisual.Children.Add(pathFromPoints);
				parentVisual.Children.Add(pathFromPoints2);
				parentVisual.Children.Add(pathFromPoints3);
				parentVisual.Children.Add(pathFromPoints4);
				pathFromPoints.Tag = new ElementData
				{
					Element = funnelSlice.DataPoint,
					VisualElementName = "TopBevel"
				};
				pathFromPoints2.Tag = new ElementData
				{
					Element = funnelSlice.DataPoint,
					VisualElementName = "LeftBevel"
				};
				pathFromPoints3.Tag = new ElementData
				{
					Element = funnelSlice.DataPoint,
					VisualElementName = "RightBevel"
				};
				pathFromPoints4.Tag = new ElementData
				{
					Element = funnelSlice.DataPoint,
					VisualElementName = "BottomBevel"
				};
				funnelSlice.DataPoint.DpInfo.Faces.Parts.Add(pathFromPoints);
				funnelSlice.DataPoint.DpInfo.Faces.Parts.Add(pathFromPoints2);
				funnelSlice.DataPoint.DpInfo.Faces.Parts.Add(pathFromPoints3);
				funnelSlice.DataPoint.DpInfo.Faces.Parts.Add(pathFromPoints4);
				funnelSlice.DataPoint.DpInfo.Faces.VisualComponents.Add(pathFromPoints);
				funnelSlice.DataPoint.DpInfo.Faces.VisualComponents.Add(pathFromPoints2);
				funnelSlice.DataPoint.DpInfo.Faces.VisualComponents.Add(pathFromPoints3);
				funnelSlice.DataPoint.DpInfo.Faces.VisualComponents.Add(pathFromPoints4);
				if ((funnelSlice.DataPoint.Chart as Chart).InternalAnimationEnabled)
				{
					double beginTime = 0.0;
					if (funnelSlice.DataPoint.IsLightDataPoint)
					{
						funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime, (funnelSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
						funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints2, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime, (funnelSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
						funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints3, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime, (funnelSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
						funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints4, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime, (funnelSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
						return;
					}
					funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime, (funnelSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
					funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints2, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime, (funnelSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
					funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints3, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime, (funnelSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
					funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(pathFromPoints4, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime, (funnelSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
				}
			}
		}

		private static Canvas CreateFunnelSlice(bool isLightingGradientLayer, double topRadius, bool is3D, TriangularChartSliceParms funnelSlice, double yScaleTop, double yScaleBottom, Brush sideFillColor, Brush topFillColor, Brush topSurfaceStroke, bool animationEnabled)
		{
			double num = 3.0;
			if (funnelSlice.Index == 0 && is3D && isLightingGradientLayer && funnelSlice.FillType == FillType.Solid)
			{
				funnelSlice.Height += num;
			}
			Canvas canvas = new Canvas
			{
				Tag = new ElementData
				{
					Element = funnelSlice.DataPoint
				}
			};
			Canvas canvas2 = new Canvas
			{
				Width = topRadius * 2.0,
				Height = funnelSlice.Height,
				Tag = new ElementData
				{
					Element = funnelSlice.DataPoint
				}
			};
			Faces faces = null;
			GeometryGroup geometryGroup = new GeometryGroup();
			PathGeometry pathGeometry = new PathGeometry();
			PathFigure pathFigure = new PathFigure
			{
				StartPoint = new Point(topRadius - funnelSlice.TopRadius, 0.0)
			};
			Path path = new Path
			{
				Fill = sideFillColor
			};
			path.Tag = new ElementData
			{
				Element = funnelSlice.DataPoint
			};
			geometryGroup.Children.Add(pathGeometry);
			path.Data = geometryGroup;
			path.StrokeThickness = 0.0;
			path.Stroke = new SolidColorBrush(Colors.Black);
			pathGeometry.Figures.Add(pathFigure);
			canvas2.Children.Add(path);
			if (is3D)
			{
				geometryGroup.FillRule = FillRule.Nonzero;
				ArcSegment arcSegment = new ArcSegment();
				arcSegment.Point = new Point(topRadius, yScaleTop / 2.0);
				arcSegment.Size = new Size(funnelSlice.TopRadius, yScaleTop / 2.0);
				pathFigure.Segments.Add(arcSegment);
				arcSegment = new ArcSegment();
				arcSegment.Point = new Point(topRadius + funnelSlice.TopRadius, 0.0);
				arcSegment.Size = new Size(funnelSlice.TopRadius, yScaleTop / 2.0);
				pathFigure.Segments.Add(arcSegment);
				LineSegment value = new LineSegment
				{
					Point = new Point(topRadius + funnelSlice.BottomRadius, funnelSlice.Height)
				};
				pathFigure.Segments.Add(value);
				value = new LineSegment
				{
					Point = new Point(topRadius - funnelSlice.BottomRadius, funnelSlice.Height)
				};
				pathFigure.Segments.Add(value);
				value = new LineSegment
				{
					Point = new Point(topRadius - funnelSlice.TopRadius, 0.0)
				};
				pathFigure.Segments.Add(value);
				EllipseGeometry ellipseGeometry = new EllipseGeometry();
				ellipseGeometry.Center = new Point(topRadius, funnelSlice.Height);
				ellipseGeometry.RadiusX = funnelSlice.BottomRadius;
				ellipseGeometry.RadiusY = yScaleBottom / 2.0;
				geometryGroup.Children.Add(ellipseGeometry);
				Ellipse ellipse = new Ellipse
				{
					Height = yScaleTop,
					Width = funnelSlice.TopRadius * 2.0,
					Fill = topFillColor,
					Tag = new ElementData
					{
						Element = funnelSlice.DataPoint
					}
				};
				ellipse.SetValue(Canvas.TopProperty, -yScaleTop / 2.0);
				ellipse.SetValue(Canvas.LeftProperty, topRadius - funnelSlice.TopRadius);
				ellipse.StrokeThickness = 1.24;
				ellipse.Stroke = Graphics.GetBevelTopBrush(topSurfaceStroke, 0.0);
				canvas2.Children.Add(ellipse);
				if (!isLightingGradientLayer)
				{
					faces = new Faces();
					faces.VisualComponents.Add(path);
					faces.VisualComponents.Add(ellipse);
					(path.Tag as ElementData).VisualElementName = "FunnelBase";
					(ellipse.Tag as ElementData).VisualElementName = "FunnelTop";
					GeometryGroup geometryGroup2 = new GeometryGroup();
					PathGeometry pathGeometry2 = new PathGeometry();
					PathFigure pathFigure2 = new PathFigure
					{
						StartPoint = pathFigure.StartPoint
					};
					LineSegment value2 = new LineSegment
					{
						Point = new Point(topRadius - funnelSlice.BottomRadius, funnelSlice.Height)
					};
					pathFigure2.Segments.Add(value2);
					pathGeometry2.Figures.Add(pathFigure2);
					PathGeometry pathGeometry3 = new PathGeometry();
					PathFigure pathFigure3 = new PathFigure
					{
						StartPoint = new Point(topRadius + funnelSlice.TopRadius, 0.0)
					};
					LineSegment value3 = new LineSegment
					{
						Point = new Point(topRadius + funnelSlice.BottomRadius, funnelSlice.Height)
					};
					pathFigure3.Segments.Add(value3);
					pathGeometry3.Figures.Add(pathFigure3);
					EllipseGeometry ellipseGeometry2 = new EllipseGeometry();
					ellipseGeometry2.Center = new Point(topRadius, funnelSlice.Height);
					ellipseGeometry2.RadiusX = funnelSlice.BottomRadius;
					ellipseGeometry2.RadiusY = yScaleBottom / 2.0;
					geometryGroup2.Children.Add(ellipseGeometry2);
					ellipseGeometry2 = new EllipseGeometry();
					ellipseGeometry2.Center = new Point(topRadius, 0.0);
					ellipseGeometry2.RadiusX = funnelSlice.TopRadius;
					ellipseGeometry2.RadiusY = yScaleTop / 2.0;
					geometryGroup2.Children.Add(ellipseGeometry2);
					geometryGroup2.Children.Add(pathGeometry2);
					geometryGroup2.Children.Add(pathGeometry3);
					Path path2 = new Path
					{
						Data = geometryGroup2,
						IsHitTestVisible = false
					};
					path2.SetValue(Panel.ZIndexProperty, -1);
					canvas2.Children.Add(path2);
					faces.BorderElements.Add(path2);
					faces.BorderElements.Add(ellipse);
					faces.Parts.Add(path);
					faces.Parts.Add(ellipse);
					funnelSlice.DataPoint.DpInfo.Faces = faces;
				}
				else
				{
					if (funnelSlice.FillType == FillType.Solid && funnelSlice.Index == 0)
					{
						path.SetValue(Panel.ZIndexProperty, 1);
						path.SetValue(Canvas.TopProperty, -num);
						funnelSlice.Height -= num;
					}
					path.IsHitTestVisible = false;
					ellipse.IsHitTestVisible = false;
					funnelSlice.DataPoint.DpInfo.Faces.Parts.Add(path);
					funnelSlice.DataPoint.DpInfo.Faces.Parts.Add(ellipse);
					funnelSlice.DataPoint.DpInfo.Faces.VisualComponents.Add(path);
					funnelSlice.DataPoint.DpInfo.Faces.VisualComponents.Add(ellipse);
					(path.Tag as ElementData).VisualElementName = "Lighting";
					(ellipse.Tag as ElementData).VisualElementName = "FunnelTopLighting";
				}
				if (animationEnabled)
				{
					double beginTime = 0.0;
					if (funnelSlice.DataPoint.IsLightDataPoint)
					{
						funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(ellipse, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime, (funnelSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
						funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(path, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime, (funnelSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
					}
					else
					{
						funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(ellipse, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime, (funnelSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
						funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(path, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime, (funnelSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
					}
				}
			}
			else
			{
				Point[] array = new Point[8];
				LineSegment lineSegment = new LineSegment
				{
					Point = new Point(topRadius - funnelSlice.BottomRadius, funnelSlice.Height)
				};
				pathFigure.Segments.Add(lineSegment);
				array[3] = lineSegment.Point;
				lineSegment = new LineSegment
				{
					Point = new Point(topRadius + funnelSlice.BottomRadius, funnelSlice.Height)
				};
				pathFigure.Segments.Add(lineSegment);
				array[2] = lineSegment.Point;
				lineSegment = new LineSegment
				{
					Point = new Point(topRadius + funnelSlice.TopRadius, 0.0)
				};
				pathFigure.Segments.Add(lineSegment);
				array[1] = lineSegment.Point;
				lineSegment = new LineSegment
				{
					Point = new Point(topRadius - funnelSlice.TopRadius, 0.0)
				};
				pathFigure.Segments.Add(lineSegment);
				array[0] = lineSegment.Point;
				if (animationEnabled)
				{
					double beginTime2 = 0.0;
					if (funnelSlice.DataPoint.IsLightDataPoint)
					{
						funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(path, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime2, (funnelSlice.DataPoint as LightDataPoint).Parent.InternalOpacity, 0.0, 1.0);
					}
					else
					{
						funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(path, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime2, (funnelSlice.DataPoint as DataPoint).InternalOpacity, 0.0, 1.0);
					}
				}
				if (!isLightingGradientLayer)
				{
					faces = new Faces();
					faces.VisualComponents.Add(path);
					(path.Tag as ElementData).VisualElementName = "FunnelBase";
					faces.Parts.Add(path);
					faces.BorderElements.Add(path);
					funnelSlice.DataPoint.DpInfo.Faces = faces;
					if (funnelSlice.DataPoint.Parent.Bevel.Value)
					{
						FunnelChart.ApplyFunnelBevel(canvas2, funnelSlice, sideFillColor, array);
					}
				}
				else
				{
					path.IsHitTestVisible = false;
					funnelSlice.DataPoint.DpInfo.Faces.VisualComponents.Add(path);
					(path.Tag as ElementData).VisualElementName = "Lighting";
				}
			}
			if (isLightingGradientLayer)
			{
				canvas2.IsHitTestVisible = false;
			}
			else
			{
				Canvas canvas3 = FunnelChart.CreateLabelLine(funnelSlice, topRadius, animationEnabled);
				if (canvas3 != null)
				{
					canvas.Children.Add(canvas3);
					faces.VisualComponents.Add(canvas3);
				}
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(funnelSlice.DataPoint, VcProperties.LabelEnabled))
				{
					Canvas canvas4 = new Canvas();
					canvas4.SetValue(Panel.ZIndexProperty, 10);
					faces.VisualComponents.Add(funnelSlice.DataPoint.DpInfo.LabelVisual);
					funnelSlice.DataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, funnelSlice.LabelLineEndPoint.Y - funnelSlice.DataPoint.DpInfo.LabelVisual.Height / 2.0);
					if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(funnelSlice.DataPoint, VcProperties.LabelStyle) == LabelStyles.OutSide)
					{
						funnelSlice.DataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, funnelSlice.LabelLineEndPoint.Y - funnelSlice.DataPoint.DpInfo.LabelVisual.Height / 2.0);
						funnelSlice.DataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, funnelSlice.LabelLineEndPoint.X);
					}
					else
					{
						funnelSlice.DataPoint.DpInfo.LabelVisual.SetValue(Canvas.TopProperty, funnelSlice.LabelLineEndPoint.Y - funnelSlice.DataPoint.DpInfo.LabelVisual.Height / 2.0 + (is3D ? (yScaleTop / 2.0) : 0.0));
						funnelSlice.DataPoint.DpInfo.LabelVisual.SetValue(Canvas.LeftProperty, topRadius - funnelSlice.DataPoint.DpInfo.LabelVisual.Width / 2.0);
					}
					if (animationEnabled)
					{
						double beginTime3 = 1.2;
						funnelSlice.DataPoint.Parent.Storyboard = AnimationHelper.ApplyOpacityAnimation(funnelSlice.DataPoint.DpInfo.LabelVisual, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard, beginTime3, 0.5, 0.0, 1.0);
					}
					canvas4.Children.Add(funnelSlice.DataPoint.DpInfo.LabelVisual);
					canvas.Children.Add(canvas4);
				}
			}
			canvas.Children.Add(canvas2);
			return canvas;
		}

		private static Canvas CreateLabelLine(TriangularChartSliceParms funnelSlice, double topRadius, bool animationEnabled)
		{
			Canvas canvas = null;
			Point point = new Point(topRadius + funnelSlice.TopRadius, 0.0);
			Point point2 = new Point(topRadius + funnelSlice.BottomRadius, funnelSlice.Height);
			funnelSlice.RightMidPoint = Graphics.MidPointOfALine(point, point2);
			if (funnelSlice.DataPoint.Parent.RenderAs == RenderAs.StreamLineFunnel)
			{
				funnelSlice.LabelLineEndPoint = new Point(2.0 * topRadius, (point2.Y - 7.5 < 0.0) ? (point2.Y * 0.9) : (point2.Y - 7.5));
			}
			else
			{
				funnelSlice.LabelLineEndPoint = new Point(2.0 * topRadius, funnelSlice.RightMidPoint.Y);
			}
			funnelSlice.OrginalLabelLineEndPoint = funnelSlice.LabelLineEndPoint;
			if ((bool)DataPointHelper.GetDataPointValueFromProperty(funnelSlice.DataPoint, VcProperties.LabelLineEnabled) && (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(funnelSlice.DataPoint, VcProperties.LabelStyle) == LabelStyles.OutSide)
			{
				canvas = new Canvas();
				canvas.Width = topRadius * 2.0;
				canvas.Height = funnelSlice.Height;
				funnelSlice.DataPoint.DpInfo.LabelLine = null;
				Path path = new Path
				{
					Stroke = (Brush)DataPointHelper.GetDataPointValueFromProperty(funnelSlice.DataPoint, VcProperties.LabelLineColor),
					Fill = (Brush)DataPointHelper.GetDataPointValueFromProperty(funnelSlice.DataPoint, VcProperties.LabelLineColor),
					StrokeDashArray = ExtendedGraphics.GetDashArray((LineStyles)DataPointHelper.GetDataPointValueFromProperty(funnelSlice.DataPoint, VcProperties.LabelLineStyle)),
					StrokeThickness = (double)DataPointHelper.GetDataPointValueFromProperty(funnelSlice.DataPoint, VcProperties.LabelLineThickness)
				};
				PathGeometry pathGeometry = new PathGeometry();
				PathFigure pathFigure = new PathFigure
				{
					StartPoint = ((funnelSlice.DataPoint.Parent.RenderAs == RenderAs.StreamLineFunnel) ? point2 : funnelSlice.RightMidPoint)
				};
				pathFigure.Segments.Add(new LineSegment
				{
					Point = funnelSlice.LabelLineEndPoint
				});
				pathGeometry.Figures.Add(pathFigure);
				path.Data = pathGeometry;
				funnelSlice.DataPoint.DpInfo.LabelLine = path;
				canvas.Children.Add(path);
				if (animationEnabled)
				{
					funnelSlice.DataPoint.Parent.Storyboard = FunnelChart.ApplyLabeLineAnimation(canvas, funnelSlice.DataPoint.Parent, funnelSlice.DataPoint.Parent.Storyboard);
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

		private static void CalculateBevelInnerPoints(TriangularChartSliceParms funnelSlice, Point[] points)
		{
			double num = 5.0;
			double num2 = num * Math.Sin(funnelSlice.TopAngle / 2.0);
			double num3 = num * Math.Cos(funnelSlice.TopAngle / 2.0);
			points[4] = new Point(points[0].X + num3, num2);
			points[5] = new Point(points[1].X - num3, num2);
			num3 = num * Math.Sin(funnelSlice.TopAngle / 2.0);
			points[6] = new Point(points[2].X - num2, points[2].Y - num3);
			points[7] = new Point(points[3].X + num2, points[3].Y - num3);
		}

		private static Brush GetSideLightingBrush(TriangularChartSliceParms funnelSlice)
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

		private static Brush GetTopBrush(Brush fillBrush, TriangularChartSliceParms funnelSlice)
		{
			if (fillBrush is SolidColorBrush)
			{
				SolidColorBrush solidColorBrush = fillBrush as SolidColorBrush;
				double num = (double)solidColorBrush.Color.R / 255.0 * 0.9999;
				double num2 = (double)solidColorBrush.Color.G / 255.0 * 0.9999;
				double num3 = (double)solidColorBrush.Color.B / 255.0 * 0.9999;
				LinearGradientBrush linearGradientBrush = null;
				if (funnelSlice.FillType == FillType.Solid)
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
				else if (funnelSlice.FillType == FillType.Hollow)
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
