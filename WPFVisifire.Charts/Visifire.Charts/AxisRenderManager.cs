using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal static class AxisRenderManager
	{
		public static Size CreateAxisForVerticalChart(ChartArea chartArea, Size plotAreaSize)
		{
			Chart chart = chartArea.Chart;
			Axis axisX = chartArea.AxisX;
			Axis axisY = chartArea.AxisY;
			Axis axisY2 = chartArea.AxisY2;
			chartArea._axisOverflowOccured = false;
			chart.ScrollingActivatedForcefully = false;
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			if (chartArea.AxisY != null)
			{
				chartArea.AxisY.ClearReferences();
			}
			if (chartArea.AxisY2 != null)
			{
				chartArea.AxisY2.ClearReferences();
			}
			double num5 = AxisRenderManager.PrepareAxesY4VerticalChart(chartArea, chart, plotAreaSize);
			plotAreaSize.Width = Math.Max(plotAreaSize.Width - num5, 0.0);
			AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
			chartArea.AxisX.ClearReferences();
			double num6 = AxisRenderManager.PrepareAxesX4VerticalChart(chartArea, chart, plotAreaSize);
			plotAreaSize.Height = Math.Max(plotAreaSize.Height - num6, 0.0);
			double scrollableLength = chartArea.ScrollableLength;
			double width = plotAreaSize.Width;
			plotAreaSize = chartArea.SetChartAreaCenterGridMargin(plotAreaSize, ref num2, ref num, ref num3, ref num4);
			double width2 = plotAreaSize.Width;
			if (width != width2)
			{
				chartArea._axisOverflowOccured = true;
			}
			AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
			bool isScrollingActivated = chart.IsScrollingActivated;
			if (!double.IsNaN(axisX.ClosestPlotDistance) || ((chart.ScrollingEnabled.Value || chart.ZoomingEnabled) && ((axisY != null && axisY.ViewportRangeEnabled) || (axisY2 != null && axisY2.ViewportRangeEnabled))))
			{
				chartArea.AxisX.ClearReferences();
				double num7 = AxisRenderManager.PrepareAxesX4VerticalChart(chartArea, chart, plotAreaSize);
				plotAreaSize = chartArea.SetChartAreaCenterGridMargin(plotAreaSize, ref num2, ref num, ref num3, ref num4);
				plotAreaSize.Height += num6;
				plotAreaSize.Height = Math.Max(plotAreaSize.Height - num7, 0.0);
				width2 = plotAreaSize.Width;
				if (width != width2)
				{
					chartArea._axisOverflowOccured = true;
				}
				AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
				if (chartArea.AxisY != null)
				{
					chartArea.AxisY.ClearReferences();
				}
				if (chartArea.AxisY2 != null)
				{
					chartArea.AxisY2.ClearReferences();
				}
				double num8 = AxisRenderManager.PrepareAxesY4VerticalChart(chartArea, chart, plotAreaSize);
				if (num5 != num8)
				{
					plotAreaSize.Width += num5;
					plotAreaSize.Width = Math.Max(plotAreaSize.Width - num8, 0.0);
					AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
					if (isScrollingActivated != chart.IsScrollingActivated)
					{
						chart.ScrollingActivatedForcefully = true;
						AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
					}
				}
				if (width != width2 || scrollableLength != chartArea.ScrollableLength || num5 != num8)
				{
					chartArea.AxisX.ClearReferences();
					double num9 = AxisRenderManager.PrepareAxesX4VerticalChart(chartArea, chart, plotAreaSize);
					if (num7 != num9)
					{
						plotAreaSize.Height += num7;
						plotAreaSize.Height = Math.Max(plotAreaSize.Height - num9, 0.0);
						AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
						if (chartArea.AxisY != null)
						{
							chartArea.AxisY.ClearReferences();
						}
						if (chartArea.AxisY2 != null)
						{
							chartArea.AxisY2.ClearReferences();
						}
						AxisRenderManager.PrepareAxesY4VerticalChart(chartArea, chart, plotAreaSize);
					}
				}
			}
			else
			{
				double num10 = AxisRenderManager.UpdateAxesY4VerticalChart(chartArea, chart, plotAreaSize);
				if (num5 != num10)
				{
					plotAreaSize.Width += num5;
					plotAreaSize.Width = Math.Max(plotAreaSize.Width - num10, 0.0);
					AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
					if (isScrollingActivated != chart.IsScrollingActivated)
					{
						chart.ScrollingActivatedForcefully = true;
						AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
					}
				}
				if (width != width2 || scrollableLength != chartArea.ScrollableLength || num5 != num10)
				{
					double num11 = AxisRenderManager.UpdateAxesX4VerticalChart(chartArea, chart, plotAreaSize);
					if (num6 != num11 && !chart.IsScrollingActivated && !chart.ZoomingEnabled)
					{
						plotAreaSize.Height += num6;
						plotAreaSize.Height = Math.Max(plotAreaSize.Height - num11, 0.0);
						AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
						AxisRenderManager.UpdateAxesY4VerticalChart(chartArea, chart, plotAreaSize);
					}
				}
			}
			return plotAreaSize;
		}

		public static Size CreateAxisForHorizontalChart(ChartArea chartArea, Size plotAreaSize)
		{
			Chart chart = chartArea.Chart;
			Axis axisX = chartArea.AxisX;
			Axis axisY = chartArea.AxisY;
			Axis axisY2 = chartArea.AxisY2;
			chartArea._axisOverflowOccured = false;
			chart.ScrollingActivatedForcefully = false;
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			if (chartArea.AxisY != null)
			{
				chartArea.AxisY.ClearReferences();
			}
			if (chartArea.AxisY2 != null)
			{
				chartArea.AxisY2.ClearReferences();
			}
			double num5 = AxisRenderManager.PrepareAxesY4HorizontalChart(chartArea, chart, plotAreaSize);
			plotAreaSize.Height = Math.Max(plotAreaSize.Height - num5, 0.0);
			AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
			chartArea.AxisX.ClearReferences();
			double num6 = AxisRenderManager.PrepareAxesX4HorizontalChart(chartArea, chart, plotAreaSize);
			plotAreaSize.Width = Math.Max(plotAreaSize.Width - num6, 0.0);
			double scrollableLength = chartArea.ScrollableLength;
			double height = plotAreaSize.Height;
			plotAreaSize = chartArea.SetChartAreaCenterGridMargin(plotAreaSize, ref num2, ref num, ref num3, ref num4);
			double height2 = plotAreaSize.Height;
			if (height != height2)
			{
				chartArea._axisOverflowOccured = true;
			}
			AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
			bool isScrollingActivated = chart.IsScrollingActivated;
			if (!double.IsNaN(axisX.ClosestPlotDistance) || ((chart.ScrollingEnabled.Value || chart.ZoomingEnabled) && ((axisY != null && axisY.ViewportRangeEnabled) || (axisY2 != null && axisY2.ViewportRangeEnabled))))
			{
				chartArea.AxisX.ClearReferences();
				double num7 = AxisRenderManager.PrepareAxesX4HorizontalChart(chartArea, chart, plotAreaSize);
				plotAreaSize.Width += num6;
				plotAreaSize.Width = Math.Max(plotAreaSize.Width - num7, 0.0);
				height2 = plotAreaSize.Height;
				if (height != height2)
				{
					chartArea._axisOverflowOccured = true;
				}
				AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
				if (chartArea.AxisY != null)
				{
					chartArea.AxisY.ClearReferences();
				}
				if (chartArea.AxisY2 != null)
				{
					chartArea.AxisY2.ClearReferences();
				}
				double num8 = AxisRenderManager.PrepareAxesY4HorizontalChart(chartArea, chart, plotAreaSize);
				if (num5 != num8)
				{
					plotAreaSize.Height += num5;
					plotAreaSize.Height = Math.Max(plotAreaSize.Height - num8, 0.0);
					AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
					if (isScrollingActivated != chart.IsScrollingActivated)
					{
						chart.ScrollingActivatedForcefully = true;
						AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
					}
				}
				if (height != height2 || scrollableLength != chartArea.ScrollableLength || num5 != num8)
				{
					chartArea.AxisX.ClearReferences();
					AxisRenderManager.PrepareAxesX4HorizontalChart(chartArea, chart, plotAreaSize);
				}
			}
			else
			{
				double num9 = AxisRenderManager.UpdateAxesY4HorizontalChart(chartArea, chart, plotAreaSize);
				if (num5 != num9)
				{
					plotAreaSize.Height += num5;
					plotAreaSize.Height = Math.Max(plotAreaSize.Height - num9, 0.0);
					AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
					if (isScrollingActivated != chart.IsScrollingActivated)
					{
						chart.ScrollingActivatedForcefully = true;
						AxisRenderManager.UpdateLayoutSettings(chartArea, plotAreaSize);
					}
				}
				if (height != height2 || scrollableLength != chartArea.ScrollableLength || num5 != num9)
				{
					AxisRenderManager.UpdateAxesX4HorizontalChart(chartArea, chart, plotAreaSize);
				}
			}
			return plotAreaSize;
		}

		public static void AddAxesYVisualsToAxisPanels(ChartArea chartArea, Chart chart)
		{
			if (chartArea.AxisY != null && chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chart._leftAxisPanel.Children.Clear();
				chart._leftAxisPanel.Children.Add(chartArea.AxisY.Visual);
			}
			if (chartArea.AxisY2 != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chart._rightAxisPanel.Children.Clear();
				chart._rightAxisPanel.Children.Add(chartArea.AxisY2.Visual);
			}
			if (chartArea.AxisY != null && chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				chart._bottomAxisPanel.Children.Clear();
				chart._bottomAxisPanel.Children.Add(chartArea.AxisY.Visual);
			}
			if (chartArea.AxisY2 != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				chart._bottomAxisPanel.Children.Clear();
				chart._bottomAxisPanel.Children.Add(chartArea.AxisY2.Visual);
			}
		}

		public static void AddAxesXVisualsToAxisPanels(ChartArea chartArea, Chart chart)
		{
			if (chartArea.AxisX != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chart._bottomAxisPanel.Children.Clear();
				chart._bottomAxisPanel.Children.Add(chartArea.AxisX.Visual);
			}
			if (chartArea.AxisX2 != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chart._topAxisPanel.Children.Clear();
				chart._topAxisPanel.Children.Add(chartArea.AxisX2.Visual);
			}
			if (chartArea.AxisX != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				chart._bottomAxisPanel.Children.Clear();
				chart._bottomAxisPanel.Children.Add(chartArea.AxisX.Visual);
			}
			if (chartArea.AxisX2 != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				chart._topAxisPanel.Children.Clear();
				chart._topAxisPanel.Children.Add(chartArea.AxisX2.Visual);
			}
		}

		public static double UpdateAxesX4VerticalChart(ChartArea chartArea, Chart chart, Size availableSize)
		{
			double num = 0.0;
			if (chartArea.AxisX != null && chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chartArea.AxisX.Width = availableSize.Width;
				chartArea.AxisX.ScrollBarElement.Width = chartArea.AxisX.Width;
				if (chartArea.PlotAreaScrollViewer != null)
				{
					chartArea.AxisX.ScrollableSize = chartArea.ScrollableLength;
					if (chart.ZoomingEnabled)
					{
						chartArea.AxisX.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
						chartArea.AxisX.ScrollBarElement.Maximum = 100.0;
					}
					else
					{
						chartArea.AxisX.ScrollBarElement.Maximum = chartArea.ScrollableLength - chartArea.PlotAreaScrollViewer.ActualWidth;
						chartArea.AxisX.ScrollBarElement.ViewportSize = chartArea.PlotAreaScrollViewer.ActualWidth;
					}
				}
				if (!chartArea.AxisX.Enabled.Value && !chart.ScrollingEnabled.Value && !chart.ZoomingEnabled)
				{
					return num;
				}
				chartArea.AxisX.UpdateAxisVisual();
				chartArea.SetScrollBarChanges(chartArea.AxisX);
				chartArea.AxisX.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
				chartArea.AxisX.ScrollBarElement.Scroll -= new ScrollEventHandler(chartArea.AxesXScrollBarElement_Scroll);
				chartArea.AxisX.ScrollBarElement.Scroll += new ScrollEventHandler(chartArea.AxesXScrollBarElement_Scroll);
				chartArea.AxisX.ZoomingScaleChanged -= new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
				if (chart.ZoomingEnabled)
				{
					chartArea.AxisX.ZoomingScaleChanged += new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
					chartArea.AxisX.ScrollBarElement.Visibility = Visibility.Visible;
					chartArea.zoomCount++;
				}
				else
				{
					if (chartArea.AxisX.Width >= chartArea.AxisX.ScrollableSize || chartArea.AxisX.ScrollBarElement.Maximum == 0.0)
					{
						chartArea.AxisX.ScrollBarElement.Visibility = Visibility.Collapsed;
					}
					else
					{
						chartArea.AxisX.ScrollBarElement.Visibility = Visibility.Visible;
					}
					chartArea.zoomCount = 0;
				}
				if (!chartArea._isFirstTimeRender)
				{
					chartArea.AxisX.ScrollBarElement.UpdateTrackLayout();
				}
				chartArea.AxisX.Height = chartArea.AxisX.Visual.Height;
				num += chartArea.AxisX.Height;
				if (chartArea.AxisX.ScrollViewerElement.Children.Count > 0)
				{
					FrameworkElement frameworkElement = chartArea.AxisX.ScrollViewerElement.Children[0] as FrameworkElement;
					if (frameworkElement != null)
					{
						double num2 = chartArea.AxisX.GetScrollBarValueFromOffset(chartArea.AxisX.CurrentScrollScrollBarOffset);
						if (!double.IsNaN(num2))
						{
							num2 = chartArea.GetScrollingOffsetOfAxis(chartArea.AxisX, num2);
							chart.PlotArea.ScrollToHorizontalOffset(num2);
							frameworkElement.SetValue(Canvas.LeftProperty, -1.0 * num2);
						}
					}
				}
			}
			else
			{
				chart._bottomAxisScrollBar.Visibility = Visibility.Collapsed;
			}
			if (chartArea.AxisX2 != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chartArea.AxisX2.Width = availableSize.Width;
				chartArea.AxisX2.ScrollBarElement.Width = chartArea.AxisX2.Width;
				if (chartArea.PlotAreaScrollViewer != null)
				{
					chartArea.AxisX2.ScrollableSize = chartArea.ScrollableLength;
					if (chart.ZoomingEnabled)
					{
						chartArea.AxisX2.ScrollBarElement.Maximum = 100.0;
					}
					else
					{
						chartArea.AxisX2.ScrollBarElement.Maximum = chartArea.ScrollableLength - chartArea.PlotAreaScrollViewer.ActualWidth;
						chartArea.AxisX2.ScrollBarElement.ViewportSize = chartArea.PlotAreaScrollViewer.ActualWidth;
					}
				}
				if (!chartArea.AxisX2.Enabled.Value && !chart.ScrollingEnabled.Value && !chart.ZoomingEnabled)
				{
					return num;
				}
				chartArea.AxisX2.UpdateAxisVisual();
				chartArea.SetScrollBarChanges(chartArea.AxisX2);
				chartArea.AxisX2.ScrollBarElement.Scroll -= new ScrollEventHandler(chartArea.AxesX2ScrollBarElement_Scroll);
				chartArea.AxisX2.ScrollBarElement.Scroll += new ScrollEventHandler(chartArea.AxesX2ScrollBarElement_Scroll);
				chartArea.AxisX2.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
				chartArea.AxisX2.ZoomingScaleChanged -= new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
				if (chart.ZoomingEnabled)
				{
					chartArea.AxisX2.ZoomingScaleChanged += new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
				}
				if (chartArea.AxisX2.Width >= chartArea.AxisX2.ScrollableSize || chartArea.AxisX2.ScrollBarElement.Maximum == 0.0)
				{
					chartArea.AxisX2.ScrollBarElement.Visibility = Visibility.Collapsed;
				}
				else
				{
					chartArea.AxisX2.ScrollBarElement.Visibility = Visibility.Visible;
				}
				chartArea.AxisX2.Height = chartArea.AxisX2.Visual.Height;
				num += chartArea.AxisX2.Height;
				if (chartArea.AxisX2.ScrollViewerElement.Children.Count > 0)
				{
					(chartArea.AxisX2.ScrollViewerElement.Children[0] as FrameworkElement).SetValue(Canvas.LeftProperty, -chartArea.AxisX2.GetScrollBarValueFromOffset(chartArea.AxisX2.CurrentScrollScrollBarOffset));
				}
			}
			else
			{
				chart._topAxisScrollBar.Visibility = Visibility.Collapsed;
			}
			return num;
		}

		public static double PrepareAxesX4VerticalChart(ChartArea chartArea, Chart chart, Size availableSize)
		{
			double num = 0.0;
			if (chartArea.AxisX != null && chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chart._bottomAxisPanel.Children.Clear();
				chartArea.AxisX.Width = availableSize.Width;
				chartArea.AxisX.ScrollBarElement.Width = chartArea.AxisX.Width;
				if (chartArea.PlotAreaScrollViewer != null)
				{
					chartArea.AxisX.ScrollableSize = chartArea.ScrollableLength;
					if (chart.ZoomingEnabled)
					{
						chartArea.AxisX.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
						chartArea.AxisX.ScrollBarElement.Maximum = 100.0;
					}
					else
					{
						chartArea.AxisX.ScrollBarElement.Maximum = chartArea.ScrollableLength - chartArea.PlotAreaScrollViewer.ActualWidth;
						chartArea.AxisX.ScrollBarElement.ViewportSize = chartArea.PlotAreaScrollViewer.ActualWidth;
					}
				}
				chartArea.AxisX.CreateDefaultElements();
				chartArea.AxisX.SetUpAxisManager();
				chartArea.AxisX.SetUpPropertiesOfElements();
				if (!chartArea.AxisX.Enabled.Value && !chart.ScrollingEnabled.Value && !chart.ZoomingEnabled)
				{
					return num;
				}
				if (chartArea._isFirstTimeRender && chartArea.AxisX.ScrollBarOffset != 0.0)
				{
					double scrollBarValueFromOffsetAtInitialRender = chartArea.AxisX.GetScrollBarValueFromOffsetAtInitialRender(chartArea.AxisX.ScrollBarOffset);
					chartArea.SaveAxisContentOffsetAndResetMargin(chartArea.AxisX, scrollBarValueFromOffsetAtInitialRender);
				}
				chartArea.AxisX.CreateVisual();
				chartArea.SetScrollBarChanges(chartArea.AxisX);
				chartArea.AxisX.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
				chartArea.AxisX.ScrollBarElement.Scroll -= new ScrollEventHandler(chartArea.AxesXScrollBarElement_Scroll);
				chartArea.AxisX.ScrollBarElement.Scroll += new ScrollEventHandler(chartArea.AxesXScrollBarElement_Scroll);
				chartArea.AxisX.ZoomingScaleChanged -= new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
				if (chart.ZoomingEnabled)
				{
					chartArea.AxisX.ZoomingScaleChanged += new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
					chartArea.AxisX.ScrollBarElement.Visibility = Visibility.Visible;
					chartArea.zoomCount++;
				}
				else
				{
					if (chartArea.AxisX.Width >= chartArea.AxisX.ScrollableSize || chartArea.AxisX.ScrollBarElement.Maximum == 0.0)
					{
						chartArea.AxisX.ScrollBarElement.Visibility = Visibility.Collapsed;
					}
					else
					{
						chartArea.AxisX.ScrollBarElement.Visibility = Visibility.Visible;
					}
					chartArea.zoomCount = 0;
				}
				if (!chartArea._isFirstTimeRender && (chart.ScrollingEnabled.Value || chart.ZoomingEnabled))
				{
					chartArea.AxisX.ScrollBarElement.UpdateTrackLayout();
				}
				chart._bottomAxisPanel.Children.Add(chartArea.AxisX.Visual);
				Size size = Graphics.CalculateVisualSize(chart._bottomAxisContainer);
				num += size.Height;
				chartArea.AxisX.Height = size.Height;
				if (chartArea.AxisX.ScrollViewerElement.Children.Count > 0)
				{
					FrameworkElement frameworkElement = chartArea.AxisX.ScrollViewerElement.Children[0] as FrameworkElement;
					if (frameworkElement != null)
					{
						double num2 = chartArea.AxisX.GetScrollBarValueFromOffset(chartArea.AxisX.CurrentScrollScrollBarOffset);
						if (!double.IsNaN(num2))
						{
							num2 = chartArea.GetScrollingOffsetOfAxis(chartArea.AxisX, num2);
							chart.PlotArea.ScrollToHorizontalOffset(num2);
							frameworkElement.SetValue(Canvas.LeftProperty, -1.0 * num2);
						}
					}
				}
			}
			else
			{
				chart._bottomAxisScrollBar.Visibility = Visibility.Collapsed;
			}
			if (chartArea.AxisX2 != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (chartArea.AxisX2.Width == availableSize.Width && chartArea.AxisX2.Width < chartArea.AxisX2.ScrollableSize && chartArea.AxisX2.ScrollBarElement.Maximum != 0.0)
				{
					return double.NaN;
				}
				chart._topAxisPanel.Children.Clear();
				chartArea.AxisX2.Width = availableSize.Width;
				chartArea.AxisX2.ScrollBarElement.Width = chartArea.AxisX2.Width;
				if (chartArea.PlotAreaScrollViewer != null)
				{
					chartArea.AxisX2.ScrollableSize = chartArea.ScrollableLength;
					if (chart.ZoomingEnabled)
					{
						chartArea.AxisX2.ScrollBarElement.Maximum = 100.0;
					}
					else
					{
						chartArea.AxisX2.ScrollBarElement.Maximum = chartArea.ScrollableLength - chartArea.PlotAreaScrollViewer.ActualWidth;
						chartArea.AxisX2.ScrollBarElement.ViewportSize = chartArea.PlotAreaScrollViewer.ActualWidth;
					}
				}
				chartArea.AxisX2.CreateDefaultElements();
				chartArea.AxisX2.SetUpAxisManager();
				chartArea.AxisX2.SetUpPropertiesOfElements();
				if (!chartArea.AxisX2.Enabled.Value && !chart.ScrollingEnabled.Value && !chart.ZoomingEnabled)
				{
					return num;
				}
				chartArea.AxisX2.CreateVisual();
				chartArea.SetScrollBarChanges(chartArea.AxisX2);
				chartArea.AxisX2.ScrollBarElement.Scroll -= new ScrollEventHandler(chartArea.AxesX2ScrollBarElement_Scroll);
				chartArea.AxisX2.ScrollBarElement.Scroll += new ScrollEventHandler(chartArea.AxesX2ScrollBarElement_Scroll);
				chartArea.AxisX2.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
				chartArea.AxisX2.ZoomingScaleChanged -= new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
				if (chart.ZoomingEnabled)
				{
					chartArea.AxisX2.ZoomingScaleChanged += new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
				}
				if (chartArea.AxisX2.Width >= chartArea.AxisX2.ScrollableSize || chartArea.AxisX2.ScrollBarElement.Maximum == 0.0)
				{
					chartArea.AxisX2.ScrollBarElement.Visibility = Visibility.Collapsed;
				}
				else
				{
					chartArea.AxisX2.ScrollBarElement.Visibility = Visibility.Visible;
				}
				chart._topAxisPanel.Children.Add(chartArea.AxisX2.Visual);
				Size size2 = Graphics.CalculateVisualSize(chart._topAxisContainer);
				num += size2.Height;
				chartArea.AxisX2.Height = size2.Height;
				if (chartArea.AxisX2.ScrollViewerElement.Children.Count > 0)
				{
					(chartArea.AxisX2.ScrollViewerElement.Children[0] as FrameworkElement).SetValue(Canvas.LeftProperty, -chartArea.AxisX2.GetScrollBarValueFromOffset(chartArea.AxisX2.CurrentScrollScrollBarOffset));
				}
			}
			else
			{
				chart._topAxisScrollBar.Visibility = Visibility.Collapsed;
			}
			return num;
		}

		public static double UpdateAxesY4VerticalChart(ChartArea chartArea, Chart chart, Size availableSize)
		{
			double num = 0.0;
			if (chartArea.AxisY != null && chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chartArea.AxisY.Height = availableSize.Height;
				chartArea.AxisY.ScrollableSize = availableSize.Height;
				if (chartArea.AxisY.Enabled.Value)
				{
					chartArea.AxisY.UpdateAxisVisual();
					chartArea.AxisY.ScrollBarElement.Visibility = Visibility.Collapsed;
					chartArea.AxisY.Width = chartArea.AxisY.Visual.Width;
					num += chartArea.AxisY.Width;
					double num2 = chart.Padding.Left + chart._leftOuterTitlePanel.ActualWidth + chart._leftOuterLegendPanel.ActualWidth;
					chartArea.AxisY.SetValue(Canvas.LeftProperty, num2);
				}
			}
			if (chartArea.AxisY2 != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chartArea.AxisY2.Height = availableSize.Height;
				chartArea.AxisY2.ScrollableSize = availableSize.Height;
				if (chartArea.AxisY2.Enabled.Value)
				{
					chartArea.AxisY2.UpdateAxisVisual();
					chartArea.AxisY2.ScrollBarElement.Visibility = Visibility.Collapsed;
					chartArea.AxisY2.Width = chartArea.AxisY2.Visual.Width;
					num += chartArea.AxisY2.Width;
				}
			}
			return num;
		}

		public static double PrepareAxesY4VerticalChart(ChartArea chartArea, Chart chart, Size availableSize)
		{
			double num = 0.0;
			if (chartArea.AxisY != null && chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chart._leftAxisPanel.Children.Clear();
				chartArea.AxisY.Height = availableSize.Height;
				chartArea.AxisY.ScrollableSize = availableSize.Height;
				chartArea.AxisY.CreateDefaultElements();
				chartArea.AxisY.SetUpAxisManager();
				chartArea.AxisY.SetUpPropertiesOfElements();
				if (chartArea.AxisY.Enabled.Value)
				{
					chartArea.AxisY.CreateVisual();
					chartArea.AxisY.ScrollBarElement.Visibility = Visibility.Collapsed;
					chart._leftAxisPanel.Children.Add(chartArea.AxisY.Visual);
					chartArea.AxisY.Width = chartArea.AxisY.Visual.Width;
					num += chartArea.AxisY.Width;
					double num2 = chart.Padding.Left + chart._leftOuterTitlePanel.ActualWidth + chart._leftOuterLegendPanel.ActualWidth;
					chartArea.AxisY.SetValue(Canvas.LeftProperty, num2);
				}
				else
				{
					chartArea.AxisY.Width = 0.0;
				}
			}
			if (chartArea.AxisY2 != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chart._rightAxisPanel.Children.Clear();
				chartArea.AxisY2.Height = availableSize.Height;
				chartArea.AxisY2.ScrollableSize = availableSize.Height;
				chartArea.AxisY2.CreateDefaultElements();
				chartArea.AxisY2.SetUpAxisManager();
				chartArea.AxisY2.SetUpPropertiesOfElements();
				if (chartArea.AxisY2.Enabled.Value)
				{
					chartArea.AxisY2.CreateVisual();
					chartArea.AxisY2.ScrollBarElement.Visibility = Visibility.Collapsed;
					chart._rightAxisPanel.Children.Add(chartArea.AxisY2.Visual);
					chartArea.AxisY2.Width = chartArea.AxisY2.Visual.Width;
					num += chartArea.AxisY2.Width;
				}
				else
				{
					chartArea.AxisY2.Width = 0.0;
				}
			}
			return num;
		}

		public static double UpdateAxesY4HorizontalChart(ChartArea chartArea, Chart chart, Size availableSize)
		{
			double num = 0.0;
			if (chartArea.AxisY != null && chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				chartArea.AxisY.Width = availableSize.Width;
				chartArea.AxisY.ScrollableSize = availableSize.Width;
				if (chartArea.AxisY.Enabled.Value)
				{
					chartArea.AxisY.UpdateAxisVisual();
					chartArea.AxisY.ScrollBarElement.Visibility = Visibility.Collapsed;
					chartArea.AxisY.Height = chartArea.AxisY.Visual.Height;
					num += chartArea.AxisY.Height;
				}
			}
			else if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chart._leftAxisScrollBar.Visibility = Visibility.Collapsed;
			}
			if (chartArea.AxisY2 != null && chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				chartArea.AxisY2.Width = availableSize.Width;
				chartArea.AxisY2.ScrollableSize = availableSize.Width;
				if (chartArea.AxisY2.Enabled.Value)
				{
					chartArea.AxisY2.UpdateAxisVisual();
					chartArea.AxisY2.ScrollBarElement.Visibility = Visibility.Collapsed;
					chartArea.AxisY2.Height = chartArea.AxisY2.Visual.Height;
					num += chartArea.AxisY2.Height;
				}
			}
			else
			{
				chart._rightAxisScrollBar.Visibility = Visibility.Collapsed;
			}
			return num;
		}

		public static double PrepareAxesY4HorizontalChart(ChartArea chartArea, Chart chart, Size availableSize)
		{
			double num = 0.0;
			if (chartArea.AxisY != null && chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				chart._bottomAxisPanel.Children.Clear();
				chartArea.AxisY.Width = availableSize.Width;
				chartArea.AxisY.ScrollableSize = availableSize.Width;
				chartArea.AxisY.CreateDefaultElements();
				chartArea.AxisY.SetUpAxisManager();
				chartArea.AxisY.SetUpPropertiesOfElements();
				if (chartArea.AxisY.Enabled.Value)
				{
					chartArea.AxisY.CreateVisual();
					chartArea.AxisY.ScrollBarElement.Visibility = Visibility.Collapsed;
					chart._bottomAxisPanel.Children.Add(chartArea.AxisY.Visual);
					chartArea.AxisY.Height = chartArea.AxisY.Visual.Height;
					num += chartArea.AxisY.Height;
				}
			}
			else if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				chart._leftAxisScrollBar.Visibility = Visibility.Collapsed;
			}
			if (chartArea.AxisY2 != null && chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				chart._topAxisPanel.Children.Clear();
				chartArea.AxisY2.Width = availableSize.Width;
				chartArea.AxisY2.ScrollableSize = availableSize.Width;
				chartArea.AxisY2.CreateDefaultElements();
				chartArea.AxisY2.SetUpAxisManager();
				chartArea.AxisY2.SetUpPropertiesOfElements();
				if (chartArea.AxisY2.Enabled.Value)
				{
					chartArea.AxisY2.CreateVisual();
					chartArea.AxisY2.ScrollBarElement.Visibility = Visibility.Collapsed;
					chart._topAxisPanel.Children.Add(chartArea.AxisY2.Visual);
					chartArea.AxisY2.Height = chartArea.AxisY2.Visual.Height;
					num += chartArea.AxisY2.Height;
				}
			}
			else
			{
				chart._rightAxisScrollBar.Visibility = Visibility.Collapsed;
			}
			return num;
		}

		public static double UpdateAxesX4HorizontalChart(ChartArea chartArea, Chart chart, Size availableSize)
		{
			double result = 0.0;
			if (chartArea.AxisX != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				chartArea.AxisX.Height = availableSize.Height;
				chartArea.AxisX.ScrollBarElement.Height = chartArea.AxisX.Height;
				if (chartArea.PlotAreaScrollViewer != null)
				{
					chartArea.AxisX.ScrollableSize = chartArea.ScrollableLength;
					if (chart.ZoomingEnabled)
					{
						chartArea.AxisX.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
						chartArea.AxisX.ScrollBarElement.Height = availableSize.Height;
						chartArea.AxisX.ScrollBarElement.Maximum = 100.0;
					}
					else
					{
						chartArea.AxisX.ScrollBarElement.Maximum = chartArea.ScrollableLength - chartArea.PlotAreaScrollViewer.ActualHeight;
						chartArea.AxisX.ScrollBarElement.ViewportSize = chartArea.PlotAreaScrollViewer.ActualHeight;
					}
				}
				if (!chartArea.AxisX.Enabled.Value && !chart.ScrollingEnabled.Value && !chart.ZoomingEnabled)
				{
					return result;
				}
				chartArea.AxisX.UpdateAxisVisual();
				chartArea.SetScrollBarChanges(chartArea.AxisX);
				chartArea.AxisX.ScrollBarElement.Height = availableSize.Height;
				chartArea.AxisX.ScrollViewerElement.Height = availableSize.Height;
				chartArea.AxisX.ScrollBarElement.Scroll -= new ScrollEventHandler(chartArea.AxesXScrollBarElement_Scroll);
				chartArea.AxisX.ScrollBarElement.Scroll += new ScrollEventHandler(chartArea.AxesXScrollBarElement_Scroll);
				chartArea.AxisX.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
				chartArea.AxisX.ZoomingScaleChanged -= new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
				if (chart.ZoomingEnabled)
				{
					chartArea.AxisX.ZoomingScaleChanged += new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
					chartArea.AxisX.ScrollBarElement.Visibility = Visibility.Visible;
					chartArea.zoomCount++;
				}
				else
				{
					if (chartArea.AxisX.Height >= chartArea.AxisX.ScrollableSize || chartArea.AxisX.ScrollBarElement.Maximum == 0.0)
					{
						chartArea.AxisX.ScrollBarElement.Visibility = Visibility.Collapsed;
					}
					else
					{
						chartArea.AxisX.ScrollBarElement.Visibility = Visibility.Visible;
					}
					chartArea.zoomCount = 0;
				}
				if (!chartArea._isFirstTimeRender)
				{
					chartArea.AxisX.ScrollBarElement.UpdateTrackLayout();
				}
				if (chartArea.AxisX.ScrollViewerElement.Children.Count > 0)
				{
					FrameworkElement frameworkElement = chartArea.AxisX.ScrollViewerElement.Children[0] as FrameworkElement;
					if (frameworkElement != null)
					{
						double num = chartArea.AxisX.GetScrollBarValueFromOffset(chartArea.AxisX.CurrentScrollScrollBarOffset);
						num = chartArea.GetScrollingOffsetOfAxis(chartArea.AxisX, num);
						chart.PlotArea.ScrollToVerticalOffset(num);
						frameworkElement.SetValue(Canvas.TopProperty, -1.0 * num);
					}
				}
			}
			if (chartArea.AxisX2 != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				chartArea.AxisX2.Height = availableSize.Height;
				chartArea.AxisX2.ScrollBarElement.Height = chartArea.AxisX2.Height;
				if (chartArea.PlotAreaScrollViewer != null)
				{
					chartArea.AxisX2.ScrollableSize = chartArea.ScrollableLength;
					if (chart.ZoomingEnabled)
					{
						chartArea.AxisX.ScrollBarElement.Maximum = 100.0;
					}
					else
					{
						chartArea.AxisX2.ScrollBarElement.Maximum = chartArea.ScrollableLength - chartArea.PlotAreaScrollViewer.ActualHeight;
						chartArea.AxisX2.ScrollBarElement.ViewportSize = chartArea.PlotAreaScrollViewer.ActualHeight;
					}
				}
				if (!chartArea.AxisX2.Enabled.Value && !chart.ScrollingEnabled.Value && !chart.ZoomingEnabled)
				{
					return result;
				}
				chartArea.AxisX2.UpdateAxisVisual();
				chartArea.SetScrollBarChanges(chartArea.AxisX2);
				chartArea.AxisX2.ScrollBarElement.Scroll -= new ScrollEventHandler(chartArea.AxesX2ScrollBarElement_Scroll);
				chartArea.AxisX2.ScrollBarElement.Scroll += new ScrollEventHandler(chartArea.AxesX2ScrollBarElement_Scroll);
				chartArea.AxisX2.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
				chartArea.AxisX2.ZoomingScaleChanged -= new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
				if (chart.ZoomingEnabled)
				{
					chartArea.AxisX2.ZoomingScaleChanged += new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
				}
				if (chartArea.AxisX2.Height >= chartArea.AxisX2.ScrollableSize || chartArea.AxisX2.ScrollBarElement.Maximum == 0.0)
				{
					chartArea.AxisX2.ScrollBarElement.Visibility = Visibility.Collapsed;
				}
				else
				{
					chartArea.AxisX2.ScrollBarElement.Visibility = Visibility.Visible;
				}
				if (chartArea.AxisX2.ScrollViewerElement.Children.Count > 0)
				{
					(chartArea.AxisX2.ScrollViewerElement.Children[0] as FrameworkElement).SetValue(Canvas.TopProperty, -chartArea.AxisX2.GetScrollBarValueFromOffset(chartArea.AxisX2.CurrentScrollScrollBarOffset));
				}
			}
			return result;
		}

		public static double PrepareAxesX4HorizontalChart(ChartArea chartArea, Chart chart, Size availableSize)
		{
			double num = 0.0;
			if (chartArea.AxisX != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				chart._leftAxisPanel.Children.Clear();
				chartArea.AxisX.Height = availableSize.Height;
				chartArea.AxisX.ScrollBarElement.Height = chartArea.AxisX.Height;
				if (chartArea.PlotAreaScrollViewer != null)
				{
					chartArea.AxisX.ScrollableSize = chartArea.ScrollableLength;
					if (chart.ZoomingEnabled)
					{
						chartArea.AxisX.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
						chartArea.AxisX.ScrollBarElement.Height = availableSize.Height;
						chartArea.AxisX.ScrollBarElement.Maximum = 100.0;
					}
					else
					{
						chartArea.AxisX.ScrollBarElement.Maximum = chartArea.ScrollableLength - chartArea.PlotAreaScrollViewer.ActualHeight;
						chartArea.AxisX.ScrollBarElement.ViewportSize = chartArea.PlotAreaScrollViewer.ActualHeight;
					}
				}
				chartArea.AxisX.CreateDefaultElements();
				chartArea.AxisX.SetUpAxisManager();
				chartArea.AxisX.SetUpPropertiesOfElements();
				if (!chartArea.AxisX.Enabled.Value && !chart.ScrollingEnabled.Value && !chart.ZoomingEnabled)
				{
					return num;
				}
				if (chartArea._isFirstTimeRender && chartArea.AxisX.ScrollBarOffset != 0.0)
				{
					double scrollBarValueFromOffsetAtInitialRender = chartArea.AxisX.GetScrollBarValueFromOffsetAtInitialRender(chartArea.AxisX.ScrollBarOffset);
					chartArea.SaveAxisContentOffsetAndResetMargin(chartArea.AxisX, scrollBarValueFromOffsetAtInitialRender);
				}
				chartArea.AxisX.CreateVisual();
				chartArea.SetScrollBarChanges(chartArea.AxisX);
				chartArea.AxisX.ScrollBarElement.Height = availableSize.Height;
				chartArea.AxisX.ScrollViewerElement.Height = availableSize.Height;
				chartArea.AxisX.ScrollBarElement.Scroll -= new ScrollEventHandler(chartArea.AxesXScrollBarElement_Scroll);
				chartArea.AxisX.ScrollBarElement.Scroll += new ScrollEventHandler(chartArea.AxesXScrollBarElement_Scroll);
				chartArea.AxisX.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
				chartArea.AxisX.ZoomingScaleChanged -= new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
				if (chart.ZoomingEnabled)
				{
					chartArea.AxisX.ZoomingScaleChanged += new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
					chartArea.AxisX.ScrollBarElement.Visibility = Visibility.Visible;
					chartArea.zoomCount++;
				}
				else
				{
					if (chartArea.AxisX.Height >= chartArea.AxisX.ScrollableSize || chartArea.AxisX.ScrollBarElement.Maximum == 0.0)
					{
						chartArea.AxisX.ScrollBarElement.Visibility = Visibility.Collapsed;
					}
					else
					{
						chartArea.AxisX.ScrollBarElement.Visibility = Visibility.Visible;
					}
					chartArea.zoomCount = 0;
				}
				if (!chartArea._isFirstTimeRender && (chart.ScrollingEnabled.Value || chart.ZoomingEnabled))
				{
					chartArea.AxisX.ScrollBarElement.UpdateTrackLayout();
				}
				chart._leftAxisPanel.Children.Add(chartArea.AxisX.Visual);
				Size size = Graphics.CalculateVisualSize(chart._leftAxisContainer);
				num += size.Width;
				chartArea.AxisX.Width = size.Width;
				if (chartArea.AxisX.ScrollViewerElement.Children.Count > 0)
				{
					FrameworkElement frameworkElement = chartArea.AxisX.ScrollViewerElement.Children[0] as FrameworkElement;
					if (frameworkElement != null)
					{
						double num2 = chartArea.AxisX.GetScrollBarValueFromOffset(chartArea.AxisX.CurrentScrollScrollBarOffset);
						num2 = chartArea.GetScrollingOffsetOfAxis(chartArea.AxisX, num2);
						chart.PlotArea.ScrollToVerticalOffset(num2);
						frameworkElement.SetValue(Canvas.TopProperty, -1.0 * num2);
					}
				}
			}
			if (chartArea.AxisX2 != null && chartArea.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				if (chartArea.AxisX2.Height == availableSize.Height && chartArea.AxisX2.Height < chartArea.AxisX2.ScrollableSize && chartArea.AxisX2.ScrollBarElement.Maximum != 0.0)
				{
					return double.NaN;
				}
				chart._rightAxisPanel.Children.Clear();
				chartArea.AxisX2.Height = availableSize.Height;
				chartArea.AxisX2.ScrollBarElement.Height = chartArea.AxisX2.Height;
				if (chartArea.PlotAreaScrollViewer != null)
				{
					chartArea.AxisX2.ScrollableSize = chartArea.ScrollableLength;
					if (chart.ZoomingEnabled)
					{
						chartArea.AxisX.ScrollBarElement.Maximum = 100.0;
					}
					else
					{
						chartArea.AxisX2.ScrollBarElement.Maximum = chartArea.ScrollableLength - chartArea.PlotAreaScrollViewer.ActualHeight;
						chartArea.AxisX2.ScrollBarElement.ViewportSize = chartArea.PlotAreaScrollViewer.ActualHeight;
					}
				}
				chartArea.AxisX2.CreateDefaultElements();
				chartArea.AxisX2.SetUpAxisManager();
				chartArea.AxisX2.SetUpPropertiesOfElements();
				if (!chartArea.AxisX2.Enabled.Value && !chart.ScrollingEnabled.Value && !chart.ZoomingEnabled)
				{
					return num;
				}
				chartArea.AxisX2.CreateVisual();
				chartArea.SetScrollBarChanges(chartArea.AxisX2);
				chartArea.AxisX2.ScrollBarElement.Scroll -= new ScrollEventHandler(chartArea.AxesX2ScrollBarElement_Scroll);
				chartArea.AxisX2.ScrollBarElement.Scroll += new ScrollEventHandler(chartArea.AxesX2ScrollBarElement_Scroll);
				chartArea.AxisX2.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
				chartArea.AxisX2.ZoomingScaleChanged -= new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
				if (chart.ZoomingEnabled)
				{
					chartArea.AxisX2.ZoomingScaleChanged += new EventHandler(chartArea.ScrollBarElement_ScaleChanged);
				}
				if (chartArea.AxisX2.Height >= chartArea.AxisX2.ScrollableSize || chartArea.AxisX2.ScrollBarElement.Maximum == 0.0)
				{
					chartArea.AxisX2.ScrollBarElement.Visibility = Visibility.Collapsed;
				}
				else
				{
					chartArea.AxisX2.ScrollBarElement.Visibility = Visibility.Visible;
				}
				chart._rightAxisPanel.Children.Add(chartArea.AxisX2.Visual);
				Size size2 = Graphics.CalculateVisualSize(chart._rightAxisContainer);
				num += size2.Width;
				chartArea.AxisX2.Width = size2.Width;
				if (chartArea.AxisX2.ScrollViewerElement.Children.Count > 0)
				{
					(chartArea.AxisX2.ScrollViewerElement.Children[0] as FrameworkElement).SetValue(Canvas.TopProperty, -chartArea.AxisX2.GetScrollBarValueFromOffset(chartArea.AxisX2.CurrentScrollScrollBarOffset));
				}
			}
			return num;
		}

		internal static void UpdateLayoutSettings(ChartArea chartArea, Size newSize)
		{
			Chart chart = chartArea.Chart;
			chart.PlotArea.Height = newSize.Height;
			chart.PlotArea.Width = newSize.Width;
			chart._drawingCanvas.Height = newSize.Height;
			chart._drawingCanvas.Width = newSize.Width;
			chart.PlotArea.BorderElement.Height = newSize.Height;
			chart.PlotArea.BorderElement.Width = newSize.Width;
			double num;
			if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				num = newSize.Height;
			}
			else
			{
				num = newSize.Width;
			}
			if (chart.ScrollingEnabled.Value)
			{
				if (chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis && chart.InternalSeries.Count > 0)
				{
					num = chartArea.CalculatePlotAreaAutoSize(num);
				}
				if (double.IsNaN(newSize.Height) || newSize.Height <= 0.0 || double.IsNaN(newSize.Width) || newSize.Width <= 0.0)
				{
					chart._plotAreaScrollViewer.Width = newSize.Width;
					chart._plotAreaScrollViewer.Height = newSize.Height;
					return;
				}
				chart.IsScrollingActivated = false;
				if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					if (num <= newSize.Width)
					{
						num = newSize.Width;
					}
					else
					{
						chart.IsScrollingActivated = true;
					}
					chart._drawingCanvas.Width = num;
					chart.PlotArea.BorderElement.Width = num;
				}
				if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
				{
					if (num <= newSize.Height)
					{
						num = newSize.Height;
					}
					else
					{
						chart.IsScrollingActivated = true;
					}
					chart._drawingCanvas.Height = num;
					chart.PlotArea.BorderElement.Height = num;
				}
			}
			else
			{
				chart.IsScrollingActivated = false;
			}
			chartArea.ScrollableLength = num;
			chart._plotAreaScrollViewer.Width = newSize.Width;
			chart._plotAreaScrollViewer.Height = newSize.Height;
			chart._plotAreaScrollViewer.UpdateLayout();
			chartArea.PlotAreaScrollViewer = chart._plotAreaScrollViewer;
			if (chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis && !double.IsNaN(chartArea.AxisX.InternalAxisMinimum) && !double.IsNaN(chartArea.AxisX.InternalAxisMaximum))
			{
				double num3;
				if (!chart.ZoomingEnabled && chart.ScrollingEnabled.Value && ((chartArea.AxisY != null && chartArea.AxisY.ViewportRangeEnabled) || (chartArea.AxisY2 != null && chartArea.AxisY2.ViewportRangeEnabled)))
				{
					double num2 = chartArea.AxisX.GetScrollBarValueFromOffset(chartArea.AxisX.CurrentScrollScrollBarOffset);
					num2 = chartArea.GetScrollingOffsetOfAxis(chartArea.AxisX, num2);
					num3 = num2;
				}
				else if (!chart.ZoomingEnabled && chart.ScrollingEnabled.Value)
				{
					double num4 = chartArea.AxisX.GetScrollBarValueFromOffset(chartArea.AxisX.CurrentScrollScrollBarOffset);
					num4 = chartArea.GetScrollingOffsetOfAxis(chartArea.AxisX, num4);
					num3 = num4;
				}
				else
				{
					num3 = chartArea.AxisX._oldScrollBarOffsetInPixel;
				}
				if (double.IsNaN(num3))
				{
					if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
					{
						num3 = chartArea.AxisX.ScrollBarElement.Maximum;
					}
					else
					{
						num3 = 0.0;
					}
				}
				if (chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					chartArea.AxisX.CalculateViewMinimumAndMaximum(chart, num3, num, newSize.Height);
					return;
				}
				chartArea.AxisX.CalculateViewMinimumAndMaximum(chart, num3, newSize.Width, num);
			}
		}
	}
}
