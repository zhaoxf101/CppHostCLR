using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Visifire.Commons;
using Visifire.Commons.Controls;

namespace Visifire.Charts
{
	internal class ChartArea
	{
		internal const double ZOOMING_MAX_VAL = 100.0;

		private const double DEFAULT_INDICATOR_GAP = 6.0;

		private const double DEFAULT_TOOLTIP_OPACITY = 0.9;

		private double _dragScrollBarOffset;

		internal MouseEventArgs _storedIndicatorEventArgs;

		internal Faces _horizontalPlank;

		internal Faces _verticalPlank;

		public Dictionary<RenderAs, Panel> RenderedCanvasList = new Dictionary<RenderAs, Panel>();

		private ColorSet _chartColorSet;

		internal Size _plotAreaSizeAfterDrawingAxes;

		internal bool _plotDetailsReCreated;

		internal bool _seriesListUpdated;

		internal bool _axisUpdated;

		internal bool _axisOverflowOccured;

		internal bool _isDefaultSeriesSet;

		internal Size _plotAreaSize;

		internal static double GRID_ANIMATION_DURATION = 1.0;

		internal static double SCROLLVIEWER_OFFSET4HORIZONTAL_CHART = 0.0;

		internal bool _isAnimationFired;

		internal Canvas _axisCallOutContainer;

		internal TextBlock _axisIndicatorTextBlock;

		internal Path _callOutPath4AxisIndicator;

		internal Border _axisIndicatorBorderElement;

		internal Line _verticalLineIndicator;

		internal bool _isFirstTimeRender = true;

		internal int zoomCount;

		internal ColorSet _financialColorSet;

		internal ColorSet _circularChartColorSet;

		internal int _renderCount;

		internal bool isScrollingActive;

		internal bool _isDragging;

		internal bool _isDefaultInteractivityAllowed;

		private Point _firstZoomRectPosOverPlotArea;

		private bool _zoomStart;

		private bool _zoomRegionSelected;

		private Point _actualZoomMinPos;

		private Point _actualZoomMaxPos;

		private bool _isZoomOutEventAttached;

		private bool _isShowAllEventAttached;

		private Canvas GridLineCanvas4VerticalPlank;

		public Canvas PlotAreaCanvas
		{
			get;
			set;
		}

		public Canvas PlottingCanvas
		{
			get;
			set;
		}

		public Canvas ChartVisualCanvas
		{
			get;
			set;
		}

		public Panel PlotAreaScrollViewer
		{
			get;
			set;
		}

		public Chart Chart
		{
			get;
			set;
		}

		internal PlotDetails PlotDetails
		{
			get
			{
				return this.Chart.PlotDetails;
			}
			set
			{
				this.Chart.PlotDetails = value;
			}
		}

		internal double PLANK_OFFSET
		{
			get;
			private set;
		}

		internal double PLANK_DEPTH
		{
			get;
			private set;
		}

		internal double PLANK_THICKNESS
		{
			get;
			private set;
		}

		internal Axis AxisX
		{
			get;
			set;
		}

		internal Axis AxisX2
		{
			get;
			set;
		}

		internal Axis AxisY
		{
			get;
			set;
		}

		internal Axis AxisY2
		{
			get;
			set;
		}

		internal double ScrollableLength
		{
			get;
			set;
		}

		internal bool IsAutoCalculatedScrollBarScale
		{
			get;
			set;
		}

		internal static double MAX_PLOTAREA_SIZE
		{
			get
			{
				return 31000.0;
			}
		}

		internal Storyboard Storyboard4PlankGridLines
		{
			get;
			set;
		}

		internal List<Path> InterlacedPathsOverVerticalPlank
		{
			get;
			set;
		}

		internal List<Line> InterlacedLinesOverVerticalPlank
		{
			get;
			set;
		}

		public ChartArea(Chart chart)
		{
			this.Chart = chart;
		}

		public void UpdateAxes()
		{
			this.PlotDetails = new PlotDetails(this.Chart);
			this.ClearAxesPanel();
			this.PopulateInternalAxesXList();
			this.PopulateInternalAxesYList();
			if (this.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis)
			{
				this.SetAxesProperties();
			}
			this.RenderAxes(this._plotAreaSize);
			this.SetAxesProperties();
		}

		public void Draw(Chart chart)
		{
			this.isScrollingActive = this.Chart.IsScrollingActivated;
			this._renderCount = 0;
			this.Chart = chart;
			this.Chart.FULL_CHART_DRAW_ENDED = false;
			this.ClearAxesPanel();
			this.ResetTitleAndLegendPannelsSize();
			this.SetSeriesStyleFromTheme();
			this.SetTitleStyleFromTheme();
			this.PopulateInternalAxesXList();
			this.PopulateInternalAxesYList();
			this.PopulateInternalSeriesList(false);
			this.SetTrendLineStyleFromTheme();
			this.SetDataPointColorFromColorSet(chart.Series);
			this.CreateOrUpdateDefaultToolTipsForSeries(chart);
			this.PlotDetails = new PlotDetails(chart);
			this.PlotDetails.Calculate(true);
			if (this._isFirstTimeRender)
			{
				chart._internalAnimationEnabled = chart.InternalAnimationEnabled;
				if (chart._internalAnimationEnabled)
				{
					chart._rootElement.IsHitTestVisible = false;
				}
				else
				{
					chart._rootElement.IsHitTestVisible = true;
				}
			}
			this.SetLegendsName(chart);
			this.ApplySampling();
			this.SetLegendStyleFromTheme();
			this.CalculatePlankParameters();
			this.ClearTitlePanels();
			this.ClearLegendPanels();
			Size actualChartSize = this.GetActualChartSize();
			bool flag;
			this.AddTitles(chart, false, actualChartSize.Height, actualChartSize.Width, out flag);
			Size boundingRec = this.CalculateLegendMaxSize(actualChartSize);
			this.AddLegends(chart, false, boundingRec.Height, boundingRec.Width);
			this.CreatePlotArea(chart);
			this._plotAreaSize = this.CalculatePlotAreaSize(boundingRec);
			if (flag)
			{
				this.ClearTitlePanels();
				this.AddTitles(chart, false, this._plotAreaSize.Height, actualChartSize.Width, out flag);
				this.ResetTitleAndLegendPannelsSize();
				boundingRec = this.CalculateLegendMaxSize(actualChartSize);
				this._plotAreaSize = this.CalculatePlotAreaSize(boundingRec);
			}
			this.HideAllAxesScrollBars();
			if (this.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis)
			{
				this.SetAxesProperties();
				this.Chart._elementCanvas.Children.Clear();
				this.CreateTrendLinesLabel();
				if (this.PlotDetails.ChartOrientation != ChartOrientationType.Circular)
				{
					if (chart.ZoomingEnabled && this.AxisX.ScrollBarElement != null && !double.IsNaN(this.AxisX.ScrollBarElement.Scale) && Math.Round(this.AxisX.ScrollBarElement.Scale, 4) != 1.0)
					{
						this.EnableZoomIcons(chart);
					}
				}
				else
				{
					this.DisableZoomIcons(chart);
				}
			}
			else if (this.PlotDetails.ChartOrientation == ChartOrientationType.NoAxis)
			{
				this.DisableZoomIcons(chart);
			}
			Size size = this.DrawChart(this._plotAreaSize);
			this.AddTitles(this.Chart, true, size.Height, size.Width, out flag);
			this.AddLegends(this.Chart, true, size.Height, size.Width);
			this.AttachEventToRetainOldScrollOffsetOfScrollViewer();
			this.AttachOrDetachIntaractivity(chart.InternalSeries);
			if (!this._isFirstTimeRender || !chart.InternalAnimationEnabled)
			{
				this.AttachScrollBarOffsetChangedEventWithAxes();
				Chart.SelectDataPoints(this.Chart);
				chart.Dispatcher.BeginInvoke(new Action(chart.UnlockRender), new object[0]);
			}
			chart._forcedRedraw = false;
			chart._clearAndResetZoomState = false;
			this.AddOrRemovePanels(chart);
			this.AttachEvents2ZoomOutIcons(chart);
			this.SettingsForVirtualRendering();
			chart.FULL_CHART_DRAW_ENDED = true;
		}

		private void SetLegendsName(Chart chart)
		{
			foreach (Legend current in chart.Legends)
			{
				if (chart.Legends.Count > 0)
				{
					if (string.IsNullOrEmpty((string)current.GetValue(FrameworkElement.NameProperty)))
					{
						current.Name = current.InternalName;
						current._isAutoName = true;
					}
					else if (string.IsNullOrEmpty(current.InternalName))
					{
						current._isAutoName = false;
					}
				}
			}
		}

		private void ApplySampling()
		{
			if (!double.IsNaN((double)this.Chart.SamplingThreshold) && this.Chart.SamplingThreshold != 0 && this.PlotDetails.ChartOrientation != ChartOrientationType.Circular && this.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis)
			{
				this.SetAxesProperties();
				this.PopulateInternalSeriesList(true);
				this.PlotDetails.Calculate(false);
				this.SetDataPointColorFromColorSet(this.Chart.Series);
			}
		}

		internal void DisableZoomIcons(Chart chart)
		{
			chart._showAllTextBlock.Visibility = Visibility.Collapsed;
			chart._zoomIconSeparater.Visibility = Visibility.Collapsed;
			chart._zoomOutTextBlock.Visibility = Visibility.Collapsed;
		}

		internal void EnableZoomIcons(Chart chart)
		{
			bool flag = true;
			if (flag)
			{
				chart._showAllTextBlock.Visibility = Visibility.Visible;
				chart._zoomIconSeparater.Visibility = Visibility.Visible;
				chart._zoomOutTextBlock.Visibility = Visibility.Visible;
			}
		}

		private void SettingsForVirtualRendering()
		{
			if (this.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis && this.AxisX != null)
			{
				this.AxisX.Scroll -= new EventHandler<AxisScrollEventArgs>(this.AxisX_Scroll);
				this.AxisX.Scroll += new EventHandler<AxisScrollEventArgs>(this.AxisX_Scroll);
			}
		}

		private void AxisX_Scroll(object sender, AxisScrollEventArgs e)
		{
			if (this.PlotDetails.IsOverrideViewPortRangeEnabled)
			{
				if (e.ScrollEventArgs.ScrollEventType != ScrollEventType.ThumbTrack && e.ScrollEventArgs.ScrollEventType != ScrollEventType.EndScroll && e.ScrollEventArgs.ScrollEventType != ScrollEventType.LargeDecrement && e.ScrollEventArgs.ScrollEventType != ScrollEventType.LargeIncrement && e.ScrollEventArgs.ScrollEventType != ScrollEventType.SmallDecrement && e.ScrollEventArgs.ScrollEventType != ScrollEventType.SmallIncrement)
				{
					return;
				}
				using (IEnumerator<DataSeries> enumerator = this.Chart.Series.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DataSeries current = enumerator.Current;
						if (current.Enabled.Value && current.PlotGroup != null && current.PlotGroup.AxisY != null && (current.PlotGroup.AxisY.ViewportRangeEnabled || (this.PlotDetails.IsOverrideViewPortRangeEnabled && RenderHelper.IsRasterRenderSupported(current))))
						{
							this.Chart.Dispatcher.BeginInvoke(new Action<VcProperties, object>(current.UpdateVisual), new object[]
							{
								VcProperties.ViewportRangeEnabled,
								current.PlotGroup.AxisY.ViewportRangeEnabled
							});
						}
					}
					return;
				}
			}
			if (e.ScrollEventArgs.ScrollEventType == ScrollEventType.ThumbTrack || e.ScrollEventArgs.ScrollEventType == ScrollEventType.ThumbPosition || e.ScrollEventArgs.ScrollEventType == ScrollEventType.LargeDecrement || e.ScrollEventArgs.ScrollEventType == ScrollEventType.LargeIncrement || e.ScrollEventArgs.ScrollEventType == ScrollEventType.SmallDecrement || e.ScrollEventArgs.ScrollEventType == ScrollEventType.SmallIncrement)
			{
				foreach (DataSeries current2 in this.Chart.Series)
				{
					if (current2.Enabled.Value && current2.PlotGroup != null && current2.PlotGroup.AxisY != null && current2.PlotGroup.AxisY.ViewportRangeEnabled)
					{
						this.Chart.Dispatcher.BeginInvoke(new Action<VcProperties, object>(current2.UpdateVisual), new object[]
						{
							VcProperties.ViewportRangeEnabled,
							current2.PlotGroup.AxisY.ViewportRangeEnabled
						});
					}
				}
			}
		}

		private void ClearTrendLineLabels()
		{
			foreach (TrendLine current in this.Chart.TrendLines)
			{
				if (current.LabelVisual != null)
				{
					this.Chart._elementCanvas.Children.Remove(current.LabelVisual);
					current.LabelVisual.Children.Clear();
					current.LabelTextBlock = null;
					current.LabelVisual = null;
				}
			}
		}

		private void CreateTrendLinesLabel()
		{
			this.ClearTrendLineLabels();
			foreach (TrendLine current in this.Chart.TrendLines)
			{
				if (!string.IsNullOrEmpty(current.LabelText))
				{
					TextBlock textBlock = new TextBlock();
					textBlock.FlowDirection = ((this.Chart != null) ? this.Chart.FlowDirection : FlowDirection.LeftToRight);
					current.ApplyLabelFontProperties(textBlock);
					current.LabelTextBlock = textBlock;
					current.LabelVisual = new Canvas();
					current.LabelVisual.Children.Add(current.LabelTextBlock);
					if (this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
					{
						if (current.AxisType == AxisTypes.Primary)
						{
							if (current.Orientation == Orientation.Horizontal)
							{
								textBlock.HorizontalAlignment = HorizontalAlignment.Right;
								textBlock.VerticalAlignment = VerticalAlignment.Center;
							}
							else
							{
								textBlock.HorizontalAlignment = HorizontalAlignment.Center;
								textBlock.VerticalAlignment = VerticalAlignment.Top;
							}
						}
						else if (current.Orientation == Orientation.Horizontal)
						{
							textBlock.HorizontalAlignment = HorizontalAlignment.Left;
							textBlock.VerticalAlignment = VerticalAlignment.Center;
						}
					}
					else if (current.AxisType == AxisTypes.Primary)
					{
						if (current.Orientation == Orientation.Horizontal)
						{
							textBlock.HorizontalAlignment = HorizontalAlignment.Right;
							textBlock.VerticalAlignment = VerticalAlignment.Center;
						}
						else
						{
							textBlock.HorizontalAlignment = HorizontalAlignment.Center;
							textBlock.VerticalAlignment = VerticalAlignment.Top;
						}
					}
					else if (current.Orientation == Orientation.Vertical)
					{
						textBlock.HorizontalAlignment = HorizontalAlignment.Center;
						textBlock.VerticalAlignment = VerticalAlignment.Top;
					}
				}
			}
		}

		private void AttachEvents2ZoomOutIcons(Chart chart)
		{
			if (chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis && this.Chart.PlotDetails.ChartOrientation != ChartOrientationType.Circular)
			{
				if (chart._zoomOutTextBlock != null && !this._isZoomOutEventAttached)
				{
					chart._zoomOutTextBlock.MouseLeftButtonUp -= new MouseButtonEventHandler(this.ZoomOutIconImage_MouseLeftButtonUp);
					chart._zoomOutTextBlock.MouseLeftButtonUp += new MouseButtonEventHandler(this.ZoomOutIconImage_MouseLeftButtonUp);
					this._isZoomOutEventAttached = true;
				}
				if (chart._showAllTextBlock != null && !this._isShowAllEventAttached)
				{
					chart._showAllTextBlock.MouseLeftButtonUp -= new MouseButtonEventHandler(this.ShowAllIconImage_MouseLeftButtonUp);
					chart._showAllTextBlock.MouseLeftButtonUp += new MouseButtonEventHandler(this.ShowAllIconImage_MouseLeftButtonUp);
					this._isShowAllEventAttached = true;
				}
			}
		}

		private void ZoomOutIconImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.AxisX == null)
			{
				return;
			}
			if (this.AxisX._zoomStateStack.Count > 0)
			{
				this.AxisX._zoomState = this.AxisX._zoomStateStack.Pop();
				this.AxisX.ZoomIn(this.AxisX._zoomState.MinXValue, this.AxisX._zoomState.MaxXValue, ZoomingAction.Unknown);
				Chart chart = this.Chart;
				if (this.AxisX._zoomStateStack.Count == 0)
				{
					if (this.AxisX.ScrollBarElement != null && this.AxisX.ScrollBarElement.Scale == 1.0)
					{
						chart.ChartArea.DisableZoomIcons(chart);
					}
					else
					{
						chart._zoomOutTextBlock.Visibility = Visibility.Collapsed;
						chart._zoomIconSeparater.Visibility = Visibility.Collapsed;
					}
				}
				this.AxisX.FireZoomEvent(this.AxisX._zoomState, e);
			}
		}

		private void ShowAllIconImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.AxisX == null)
			{
				return;
			}
			this.AxisX.ZoomIn(this.AxisX._initialState.MinXValue, this.AxisX._initialState.MaxXValue, ZoomingAction.Unknown);
			this.AxisX._showAllState = true;
			Chart chart = this.Chart;
			if (chart != null)
			{
				chart._zoomOutTextBlock.Visibility = Visibility.Collapsed;
				chart._showAllTextBlock.Visibility = Visibility.Collapsed;
				chart._zoomIconSeparater.Visibility = Visibility.Collapsed;
			}
			this.AxisX.FireZoomEvent(this.AxisX._initialState, e);
		}

		private void CreateOrUpdateDefaultToolTipsForSeries(Chart chart)
		{
			this.DisableIndicators();
			List<DataSeries> list = (from series in this.Chart.InternalSeries
			where !string.IsNullOrEmpty(Convert.ToString(series.ToolTip))
			select series).ToList<DataSeries>();
			List<DataSeries> list2 = (from series in this.Chart.InternalSeries
			where string.IsNullOrEmpty(Convert.ToString(series.ToolTip))
			select series).ToList<DataSeries>();
			if (list.Count > 0)
			{
				ToolTip toolTip = null;
				foreach (DataSeries ds in list)
				{
					IEnumerable<ToolTip> source = from entry in this.Chart.ToolTips
					where entry.Name == ds.ToolTipName && entry != toolTip
					select entry;
					if (this.Chart.IndicatorEnabled && (ds.RenderAs != RenderAs.Pie || ds.RenderAs != RenderAs.Doughnut || ds.RenderAs != RenderAs.SectionFunnel || ds.RenderAs != RenderAs.StreamLineFunnel || ds.RenderAs != RenderAs.Pyramid))
					{
						if (source.Count<ToolTip>() == 0)
						{
							ds.RemoveToolTip();
							toolTip = new ToolTip
							{
								_isAutoGenerated = true,
								Visibility = Visibility.Collapsed
							};
							toolTip.SetValue(FrameworkElement.NameProperty, ds.ToolTipName);
							this.Chart.ToolTips.Add(toolTip);
							ds.ToolTipElement = toolTip;
							if (toolTip.Parent == null)
							{
								chart._toolTipCanvas.Children.Add(toolTip);
							}
						}
						else
						{
							ToolTip toolTip3 = null;
							if (source.Any<ToolTip>())
							{
								toolTip3 = source.First<ToolTip>();
							}
							if (ds.ToolTipElement != null)
							{
								if (ds.ToolTipElement._isAutoGenerated)
								{
									ds.RemoveToolTip();
								}
								else
								{
									ds.ToolTipElement.Hide();
									ds.ToolTipElement = null;
								}
							}
							if (toolTip3 != null)
							{
								toolTip = toolTip3;
								toolTip.Visibility = Visibility.Collapsed;
								ds.ToolTipElement = toolTip;
								if (toolTip.Parent == null)
								{
									chart._toolTipCanvas.Children.Add(toolTip);
								}
							}
						}
					}
					else
					{
						if (source.Count<ToolTip>() == 0)
						{
							ds.RemoveToolTip();
						}
						else if (ds.ToolTipElement != null)
						{
							if (ds.ToolTipElement._isAutoGenerated)
							{
								ds.RemoveToolTip();
							}
							else
							{
								ds.ToolTipElement.Hide();
								ds.ToolTipElement = null;
							}
						}
						this.DisableIndicators();
					}
				}
			}
			if (list2.Count > 0)
			{
				foreach (DataSeries current in list2)
				{
					current.RemoveToolTip();
					if (this.Chart.IndicatorEnabled && (current.RenderAs != RenderAs.Pie || current.RenderAs != RenderAs.Doughnut || current.RenderAs != RenderAs.SectionFunnel || current.RenderAs != RenderAs.StreamLineFunnel || current.RenderAs != RenderAs.Pyramid))
					{
						ToolTip toolTip2 = new ToolTip
						{
							_isAutoGenerated = true,
							Visibility = Visibility.Collapsed
						};
						this.Chart.ToolTips.Add(toolTip2);
						current.ToolTipElement = toolTip2;
						current.ToolTipName = toolTip2.Name;
						if (toolTip2.Parent == null)
						{
							chart._toolTipCanvas.Children.Add(toolTip2);
						}
					}
					else
					{
						current.RemoveToolTip();
						this.DisableIndicators();
					}
				}
			}
			this.ReAssignDefaultToolTip();
		}

		private void ReAssignDefaultToolTip()
		{
			foreach (ToolTip toolTip in this.Chart.ToolTips)
			{
				List<DataSeries> list = (from series in this.Chart.InternalSeries
				where Convert.ToString(series.ToolTip) != toolTip.Name
				select series).ToList<DataSeries>();
				if (list.Count == this.Chart.InternalSeries.Count)
				{
					if (this.Chart._toolTip != null)
					{
						this.Chart._toolTip.Hide();
						this.Chart._toolTip = null;
					}
					this.Chart._toolTip = toolTip;
					if (!this.Chart._toolTipCanvas.Children.Contains(toolTip))
					{
						if (toolTip.Parent != null)
						{
							Canvas canvas = toolTip.Parent as Canvas;
							canvas.Children.Remove(toolTip);
						}
						this.Chart._toolTipCanvas.Children.Add(toolTip);
					}
					return;
				}
			}
			if (this.Chart._toolTip != null)
			{
				IEnumerable<DataSeries> source = from series in this.Chart.InternalSeries
				where series.ToolTipName == this.Chart._toolTip.Name
				select series;
				if (source.Count<DataSeries>() == 0)
				{
					this.Chart.ToolTips.Remove(this.Chart._toolTip);
					if (this.Chart._toolTip.Parent != null)
					{
						this.Chart._toolTipCanvas.Children.Remove(this.Chart._toolTip);
					}
				}
			}
			ToolTip toolTip2 = new ToolTip
			{
				_isAutoGenerated = true,
				Visibility = Visibility.Collapsed
			};
			this.Chart.ToolTips.Add(toolTip2);
			this.Chart._toolTip = toolTip2;
			if (toolTip2.Parent == null)
			{
				this.Chart._toolTipCanvas.Children.Add(toolTip2);
			}
		}

		internal void DisableIndicators()
		{
			if (this._verticalLineIndicator != null)
			{
				this._verticalLineIndicator.Visibility = Visibility.Collapsed;
			}
			if (this._axisCallOutContainer != null)
			{
				this._axisCallOutContainer.Visibility = Visibility.Collapsed;
			}
			if (this._axisIndicatorBorderElement != null)
			{
				this._axisIndicatorBorderElement.Visibility = Visibility.Collapsed;
			}
		}

		internal void ArrangeToolTips4DataSeries(List<IDataPoint> nearestDataPointList, double xPlotCanvasOffset, double yPlotCanvasOffset)
		{
			double num = xPlotCanvasOffset;
			double num2 = yPlotCanvasOffset + (this.Chart.View3D ? this.PLANK_DEPTH : 0.0);
			if (this.AxisX.AxisOrientation == AxisOrientation.Vertical)
			{
				num += (this.Chart.View3D ? this.PLANK_THICKNESS : 0.0);
			}
			double width = this.PlotAreaCanvas.Width;
			double height = this.PlotAreaCanvas.Height;
			Rect[] array = new Rect[nearestDataPointList.Count];
			int i = 0;
			List<Point> list = new List<Point>();
			string axisIndicatorText = "";
			double num3 = this.AxisX.GetScrollBarValueFromOffset(this.AxisX.CurrentScrollScrollBarOffset);
			num3 = this.GetScrollingOffsetOfAxis(this.AxisX, num3);
			foreach (IDataPoint current in nearestDataPointList)
			{
				current.Parent.ShowToolTip();
				double num4 = current.DpInfo.VisualPosition.X;
				double num5 = current.DpInfo.VisualPosition.Y;
				if (this.AxisX.AxisOrientation == AxisOrientation.Vertical)
				{
					if (num4 < 0.0)
					{
						num4 = 0.0;
					}
					if (num4 > width)
					{
						num4 = width;
					}
				}
				else
				{
					if (num5 < 0.0)
					{
						num5 = 0.0;
					}
					if (num5 > height)
					{
						num5 = height;
					}
				}
				Point point = new Point(num4 + num, num5 + num2);
				ToolTip toolTipElement = current.Parent.ToolTipElement;
				Point location = point;
				list.Add(new Point(location.X, location.Y));
				if (this.AxisX.AxisOrientation == AxisOrientation.Horizontal)
				{
					location.X -= num3;
				}
				else
				{
					location.Y -= num3;
				}
				toolTipElement.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				toolTipElement.UpdateLayout();
				Size size = Graphics.CalculateVisualSize(toolTipElement);
				if (location.X + size.Width < this.Chart.ActualWidth)
				{
					if (RenderHelper.IsLineCType(current.Parent) || RenderHelper.IsAreaCType(current.Parent) || current.Parent.RenderAs == RenderAs.Bubble || current.Parent.RenderAs == RenderAs.Point)
					{
						location.X += 10.0;
						location.Y -= size.Height / 2.0;
						if (location.Y < 0.0)
						{
							location.Y += 10.0;
						}
					}
					else
					{
						if (current.Parent.RenderAs == RenderAs.Bar || current.Parent.RenderAs == RenderAs.StackedBar || current.Parent.RenderAs == RenderAs.StackedBar100)
						{
							if (current.YValue >= 0.0)
							{
								location.X += 10.0;
								if (location.X > this.Chart.ActualWidth)
								{
									location.X = point.X - size.Width - 10.0;
								}
								location.Y += 5.0;
							}
							else
							{
								location.X = location.X - size.Width - 10.0;
								if (location.X < 0.0)
								{
									location.X = point.X + 10.0;
								}
								location.Y = location.Y - size.Height - 5.0;
							}
						}
						else
						{
							location.X += 10.0;
							if (current.YValue >= 0.0 || current.Parent.RenderAs == RenderAs.CandleStick || current.Parent.RenderAs == RenderAs.Stock)
							{
								location.Y = location.Y - size.Height / 2.0 - 10.0;
							}
							else
							{
								location.Y = location.Y - size.Height / 2.0 + 10.0;
							}
						}
						if (location.Y < 0.0)
						{
							location.Y += 20.0;
						}
					}
					toolTipElement.SetValue(Canvas.LeftProperty, location.X);
					toolTipElement.SetValue(Canvas.TopProperty, location.Y);
				}
				else
				{
					if (RenderHelper.IsLineCType(current.Parent) || RenderHelper.IsAreaCType(current.Parent))
					{
						location.X = location.X - size.Width - 10.0;
						location.Y -= size.Height / 2.0;
						if (location.Y < 0.0)
						{
							location.Y += 10.0;
						}
					}
					else
					{
						if (current.Parent.RenderAs == RenderAs.Bar || current.Parent.RenderAs == RenderAs.StackedBar || current.Parent.RenderAs == RenderAs.StackedBar100)
						{
							location.X = location.X - size.Width - 10.0;
							location.Y += 5.0;
						}
						else
						{
							location.X = location.X - size.Width - 10.0;
							if (current.YValue >= 0.0 || current.Parent.RenderAs == RenderAs.CandleStick || current.Parent.RenderAs == RenderAs.Stock)
							{
								location.Y = location.Y - size.Height / 2.0 - 10.0;
							}
							else
							{
								location.Y = location.Y - size.Height / 2.0 + 10.0;
							}
						}
						if (location.Y < 0.0)
						{
							location.Y += 20.0;
						}
					}
					toolTipElement.SetValue(Canvas.LeftProperty, location.X);
					toolTipElement.SetValue(Canvas.TopProperty, location.Y);
				}
				Rect rect = new Rect(location, size);
				array[i++] = rect;
			}
			Rect area = new Rect(0.0, 0.0, this.Chart.ActualWidth, this.Chart.ActualHeight);
			LabelPlacementHelper.VerticalLabelPlacement(area, ref array);
			IDataPoint dataPoint = null;
			for (i = 0; i < array.Length; i++)
			{
				IDataPoint dataPoint2 = nearestDataPointList[i];
				dataPoint = dataPoint2;
				if (!this.AxisX.IsDateTimeAxis)
				{
					if (this.AxisX.AxisLabels != null)
					{
						if (this.AxisX.AxisLabels.AxisLabelContentDictionary.ContainsKey(dataPoint2.DpInfo.ActualNumericXValue))
						{
							axisIndicatorText = ObservableObject.GetFormattedMultilineText(this.AxisX.AxisLabels.AxisLabelContentDictionary[dataPoint2.DpInfo.ActualNumericXValue]);
						}
						else
						{
							axisIndicatorText = this.AxisX.AxisLabels.GetFormattedString(dataPoint2.DpInfo.ActualNumericXValue);
						}
					}
				}
				else
				{
					string text = (this.AxisX.XValueType == ChartValueTypes.Date) ? "d" : ((this.AxisX.XValueType == ChartValueTypes.Time) ? "t" : "G");
					text = (string.IsNullOrEmpty((string)this.AxisX.GetValue(Axis.ValueFormatStringProperty)) ? text : this.AxisX.ValueFormatString);
					axisIndicatorText = Convert.ToDateTime(dataPoint2.XValue, CultureInfo.InvariantCulture).ToString(text, CultureInfo.CurrentCulture);
				}
				if (dataPoint2.Parent.ToolTipElement._borderElement != null)
				{
					if (dataPoint2.Parent.RenderAs == RenderAs.CandleStick)
					{
						if (dataPoint2.Parent.ToolTipElement._callOutPath != null)
						{
							dataPoint2.Parent.ToolTipElement._callOutPath.Fill = ((dataPoint2.Parent.ToolTipElement.Background == null) ? (dataPoint2.Parent.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(dataPoint2.Parent.PriceUpColor, "Linear", new double[]
							{
								0.8,
								0.55
							}) : dataPoint2.Parent.PriceUpColor) : dataPoint2.Parent.ToolTipElement.Background);
						}
						dataPoint2.Parent.ToolTipElement._borderElement.Background = ((dataPoint2.Parent.ToolTipElement.Background == null) ? (dataPoint2.Parent.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(dataPoint2.Parent.PriceUpColor, "Linear", new double[]
						{
							0.8,
							0.55
						}) : dataPoint2.Parent.PriceUpColor) : dataPoint2.Parent.ToolTipElement.Background);
						if (dataPoint2.Parent.ToolTipElement._isAutoGeneratedFontColor)
						{
							dataPoint2.Parent.ToolTipElement.FontColor = DataSeries.CalculateFontColor(dataPoint2.Parent.ToolTipElement._borderElement.Background, dataPoint2.Parent.Chart as Chart);
							dataPoint2.Parent.ToolTipElement._isAutoGeneratedFontColor = true;
						}
					}
					else
					{
						if (dataPoint2.Parent.ToolTipElement._callOutPath != null)
						{
							dataPoint2.Parent.ToolTipElement._callOutPath.Fill = ((dataPoint2.Parent.ToolTipElement.Background == null) ? (dataPoint2.Parent.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(dataPoint2.Parent.Color, "Linear", new double[]
							{
								0.8,
								0.55
							}) : dataPoint2.Parent.Color) : dataPoint2.Parent.ToolTipElement.Background);
						}
						dataPoint2.Parent.ToolTipElement._borderElement.Background = ((dataPoint2.Parent.ToolTipElement.Background == null) ? (dataPoint2.Parent.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(dataPoint2.Parent.Color, "Linear", new double[]
						{
							0.8,
							0.55
						}) : dataPoint2.Parent.Color) : dataPoint2.Parent.ToolTipElement.Background);
						if (dataPoint2.Parent.ToolTipElement._isAutoGeneratedFontColor)
						{
							dataPoint2.Parent.ToolTipElement.FontColor = DataSeries.CalculateFontColor(dataPoint2.Parent.ToolTipElement._borderElement.Background, dataPoint2.Parent.Chart as Chart);
							dataPoint2.Parent.ToolTipElement._isAutoGeneratedFontColor = true;
						}
					}
					if (dataPoint2.Parent.ToolTipElement._callOutPath != null && dataPoint2.Parent.ToolTipElement._callOutPath.Fill == null && this.Chart._toolTip != null)
					{
						dataPoint2.Parent.ToolTipElement._callOutPath.Fill = ToolTip.GetBackgroundBrush();
					}
					if (dataPoint2.Parent.ToolTipElement._borderElement.Background == null && this.Chart._toolTip != null)
					{
						dataPoint2.Parent.ToolTipElement._borderElement.Background = ToolTip.GetBackgroundBrush();
					}
					dataPoint2.Parent.ToolTipElement.SetValue(Canvas.LeftProperty, array[i].Left);
					dataPoint2.Parent.ToolTipElement.SetValue(Canvas.TopProperty, array[i].Top);
					double num6 = dataPoint2.DpInfo.VisualPosition.X;
					double num7 = dataPoint2.DpInfo.VisualPosition.Y;
					if (this.AxisX.AxisOrientation == AxisOrientation.Vertical)
					{
						if (num6 < 0.0)
						{
							num6 = 0.0;
						}
						if (num6 > width)
						{
							num6 = width;
						}
					}
					else
					{
						if (num7 < 0.0)
						{
							num7 = 0.0;
						}
						if (num7 > height)
						{
							num7 = height;
						}
					}
					Point point2 = new Point(num6 + num - num3, num7 + num2);
					if (this.AxisX.AxisOrientation == AxisOrientation.Vertical)
					{
						point2.X += num3;
						point2.Y -= num3;
					}
					if (dataPoint2.Parent.RenderAs == RenderAs.Bar || dataPoint2.Parent.RenderAs == RenderAs.StackedBar || dataPoint2.Parent.RenderAs == RenderAs.StackedBar100)
					{
						if (array[i].Left + dataPoint2.Parent.ToolTipElement._borderElement.ActualWidth < dataPoint2.DpInfo.VisualPosition.X + num)
						{
							dataPoint2.Parent.ToolTipElement.CallOutStartPoint = new Point(dataPoint2.Parent.ToolTipElement._borderElement.ActualWidth - 1.0, dataPoint2.Parent.ToolTipElement._borderElement.ActualHeight / 4.0);
							dataPoint2.Parent.ToolTipElement.CallOutMidPoint = new Point(dataPoint2.Parent.ToolTipElement._borderElement.ActualWidth + (point2.X - (array[i].Left + dataPoint2.Parent.ToolTipElement._borderElement.ActualWidth)), point2.Y - array[i].Top);
							dataPoint2.Parent.ToolTipElement.CallOutEndPoint = new Point(dataPoint2.Parent.ToolTipElement._borderElement.ActualWidth - 1.0, dataPoint2.Parent.ToolTipElement._borderElement.ActualHeight - dataPoint2.Parent.ToolTipElement._borderElement.ActualHeight / 4.0);
							dataPoint2.Parent.ToolTipElement._borderElement.BorderThickness = new Thickness(0.25, 0.25, 0.0, 1.0);
						}
						else
						{
							dataPoint2.Parent.ToolTipElement.CallOutStartPoint = new Point(1.0, dataPoint2.Parent.ToolTipElement._borderElement.ActualHeight / 4.0);
							dataPoint2.Parent.ToolTipElement.CallOutMidPoint = new Point(point2.X - array[i].Left, point2.Y - array[i].Top);
							dataPoint2.Parent.ToolTipElement.CallOutEndPoint = new Point(1.0, dataPoint2.Parent.ToolTipElement._borderElement.ActualHeight - dataPoint2.Parent.ToolTipElement._borderElement.ActualHeight / 4.0);
							dataPoint2.Parent.ToolTipElement._borderElement.BorderThickness = new Thickness(0.0, 0.25, 0.25, 1.0);
						}
					}
					else if (list[i].X - num3 + dataPoint2.Parent.ToolTipElement._borderElement.ActualWidth < this.Chart.ActualWidth)
					{
						dataPoint2.Parent.ToolTipElement.CallOutStartPoint = new Point(1.0, dataPoint2.Parent.ToolTipElement._borderElement.ActualHeight / 4.0);
						dataPoint2.Parent.ToolTipElement.CallOutMidPoint = new Point(point2.X - array[i].Left, point2.Y - array[i].Top);
						dataPoint2.Parent.ToolTipElement.CallOutEndPoint = new Point(1.0, dataPoint2.Parent.ToolTipElement._borderElement.ActualHeight - dataPoint2.Parent.ToolTipElement._borderElement.ActualHeight / 4.0);
						dataPoint2.Parent.ToolTipElement._borderElement.BorderThickness = new Thickness(0.0, 0.25, 0.25, 1.0);
					}
					else
					{
						dataPoint2.Parent.ToolTipElement.CallOutStartPoint = new Point(dataPoint2.Parent.ToolTipElement._borderElement.ActualWidth - 1.0, dataPoint2.Parent.ToolTipElement._borderElement.ActualHeight / 4.0);
						dataPoint2.Parent.ToolTipElement.CallOutMidPoint = new Point(dataPoint2.Parent.ToolTipElement._borderElement.ActualWidth + (point2.X - (array[i].Left + dataPoint2.Parent.ToolTipElement._borderElement.ActualWidth)), point2.Y - array[i].Top);
						dataPoint2.Parent.ToolTipElement.CallOutEndPoint = new Point(dataPoint2.Parent.ToolTipElement._borderElement.ActualWidth - 1.0, dataPoint2.Parent.ToolTipElement._borderElement.ActualHeight - dataPoint2.Parent.ToolTipElement._borderElement.ActualHeight / 4.0);
						dataPoint2.Parent.ToolTipElement._borderElement.BorderThickness = new Thickness(0.25, 0.25, 0.0, 1.0);
					}
				}
			}
			if (list.Count > 0)
			{
				double actualPos;
				if (this.AxisX.AxisOrientation == AxisOrientation.Horizontal)
				{
					actualPos = Graphics.ValueToPixelPosition(num, this.ScrollableLength + num - this.PLANK_DEPTH, this.AxisX.InternalAxisMinimum, this.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
					this.SetPositon4Indicators(this.AxisX, actualPos, axisIndicatorText, xPlotCanvasOffset, yPlotCanvasOffset, width, height);
					return;
				}
				actualPos = Graphics.ValueToPixelPosition(this.ScrollableLength + num2 - this.PLANK_DEPTH, num2, this.AxisX.InternalAxisMinimum, this.AxisX.InternalAxisMaximum, dataPoint.DpInfo.InternalXValue);
				this.SetPositon4Indicators(this.AxisX, actualPos, axisIndicatorText, xPlotCanvasOffset, yPlotCanvasOffset, width, height);
			}
		}

		private void AddOrRemovePanels(Chart chart)
		{
			if (this.isScrollingActive && this.Chart.IsScrollingActivated)
			{
				if (chart.IsInDesignMode)
				{
					ObservableObject.RemoveElementFromElementTree(this.PlotAreaCanvas);
				}
				else
				{
					chart._centerInnerGrid.Children.Remove(this.PlotAreaCanvas);
				}
				if (this.PlotAreaCanvas.Parent == null)
				{
					chart._drawingCanvas.Children.Add(this.PlotAreaCanvas);
					return;
				}
			}
			else if (!this.isScrollingActive && !this.Chart.IsScrollingActivated)
			{
				if (chart.IsInDesignMode)
				{
					ObservableObject.RemoveElementFromElementTree(this.PlotAreaCanvas);
				}
				else
				{
					chart._drawingCanvas.Children.Remove(this.PlotAreaCanvas);
				}
				if (this.PlotAreaCanvas.Parent == null)
				{
					chart._centerInnerGrid.Children.Add(this.PlotAreaCanvas);
					return;
				}
			}
			else if (!this.isScrollingActive && this.Chart.IsScrollingActivated)
			{
				if (chart.IsInDesignMode)
				{
					ObservableObject.RemoveElementFromElementTree(this.PlotAreaCanvas);
				}
				else
				{
					chart._centerInnerGrid.Children.Remove(this.PlotAreaCanvas);
				}
				if (this.PlotAreaCanvas.Parent == null)
				{
					chart._drawingCanvas.Children.Add(this.PlotAreaCanvas);
					return;
				}
			}
			else if (this.isScrollingActive && !this.Chart.IsScrollingActivated)
			{
				if (chart.IsInDesignMode)
				{
					ObservableObject.RemoveElementFromElementTree(this.PlotAreaCanvas);
				}
				else
				{
					chart._drawingCanvas.Children.Remove(this.PlotAreaCanvas);
				}
				if (this.PlotAreaCanvas.Parent == null)
				{
					chart._centerInnerGrid.Children.Add(this.PlotAreaCanvas);
				}
			}
		}

		internal void PrePartialUpdateConfiguration(PartialUpdateConfiguration partialUpdateConfiguration)
		{
			if (partialUpdateConfiguration.IsUpdateLists && !this._seriesListUpdated)
			{
				this.PopulateInternalSeriesList(false);
				this.SetDataPointColorFromColorSet(this.Chart.Series);
				this._seriesListUpdated = partialUpdateConfiguration.IsUpdateLists;
			}
			if (partialUpdateConfiguration.IsCalculatePlotDetails && !this._plotDetailsReCreated)
			{
				this.PlotDetails.ReCreate(partialUpdateConfiguration.Sender, partialUpdateConfiguration.ElementType, partialUpdateConfiguration.Property, partialUpdateConfiguration.OldValue, partialUpdateConfiguration.NewValue);
				this._plotDetailsReCreated = partialUpdateConfiguration.IsCalculatePlotDetails;
			}
			bool flag = false;
			if (partialUpdateConfiguration.IsUpdateAxis && !this._axisUpdated)
			{
				if (this.AxisX != null)
				{
					this.AxisX.ClearReferences();
				}
				this.PopulateInternalAxesXList();
				if (this.AxisY != null)
				{
					this.AxisY.ClearReferences();
				}
				if (this.AxisY2 != null)
				{
					this.AxisY2.ClearReferences();
				}
				this.PopulateInternalAxesYList();
				this.ClearAxesPanel();
				this.CreateTrendLinesLabel();
				Size remainingSizeAfterDrawingAxes = this.RenderAxes(this._plotAreaSize);
				remainingSizeAfterDrawingAxes = this.RecalculateAxisAndRerenderBasedOnChartTypes(this._plotAreaSize, remainingSizeAfterDrawingAxes);
				this.ResizePanels(remainingSizeAfterDrawingAxes, partialUpdateConfiguration.RenderAxisType, partialUpdateConfiguration.IsPartialUpdate);
				if (double.IsNaN(remainingSizeAfterDrawingAxes.Height) || double.IsNaN(remainingSizeAfterDrawingAxes.Width) || remainingSizeAfterDrawingAxes.Height == 0.0 || remainingSizeAfterDrawingAxes.Width == 0.0)
				{
					return;
				}
				if (this.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis)
				{
					this.RenderGrids();
					this.RenderTrendLines();
				}
				this._axisUpdated = partialUpdateConfiguration.IsUpdateAxis;
				flag = true;
			}
			if (flag)
			{
				this.AddOrRemovePanels(this.Chart);
			}
		}

		internal void AttachOrDetachIntaractivity(List<DataSeries> series)
		{
			foreach (DataSeries current in series)
			{
				if (this._isFirstTimeRender)
				{
					if (current.SelectionEnabled)
					{
						current.AttachOrDetachIntaractivity();
					}
				}
				else
				{
					current.AttachOrDetachIntaractivity();
				}
			}
		}

		private Size GetActualChartSize()
		{
			Size result = new Size(this.Chart._chartBorder.ActualWidth, this.Chart._chartBorder.ActualHeight);
			result.Width -= this.Chart._chartBorder.Padding.Left + this.Chart._chartBorder.Padding.Right;
			result.Height -= this.Chart._chartBorder.Padding.Top + this.Chart._chartBorder.Padding.Bottom;
			result.Width -= this.Chart._chartBorder.BorderThickness.Left + this.Chart._chartBorder.BorderThickness.Right;
			result.Height -= this.Chart._chartBorder.BorderThickness.Top + this.Chart._chartBorder.BorderThickness.Bottom;
			result.Width -= this.Chart._chartAreaGrid.Margin.Left + this.Chart._chartAreaGrid.Margin.Right;
			result.Height -= this.Chart._chartAreaGrid.Margin.Top + this.Chart._chartAreaGrid.Margin.Bottom;
			if (this.Chart.Bevel)
			{
				result.Height -= 5.0;
			}
			return result;
		}

		private void ClearAxesPanel()
		{
			this.Chart._leftAxisPanel.Children.Clear();
			this.Chart._bottomAxisPanel.Children.Clear();
			this.Chart._topAxisPanel.Children.Clear();
			this.Chart._rightAxisPanel.Children.Clear();
		}

		private void ClearTitlePanels()
		{
			this.Chart._topOuterTitlePanel.Children.Clear();
			this.Chart._rightOuterTitlePanel.Children.Clear();
			this.Chart._leftOuterTitlePanel.Children.Clear();
			this.Chart._bottomOuterTitlePanel.Children.Clear();
			this.Chart._topInnerTitlePanel.Children.Clear();
			this.Chart._rightInnerTitlePanel.Children.Clear();
			this.Chart._leftInnerTitlePanel.Children.Clear();
			this.Chart._bottomInnerTitlePanel.Children.Clear();
		}

		private void ClearLegendPanels()
		{
			this.Chart._topInnerLegendPanel.Children.Clear();
			this.Chart._bottomInnerLegendPanel.Children.Clear();
			this.Chart._leftInnerLegendPanel.Children.Clear();
			this.Chart._rightInnerLegendPanel.Children.Clear();
			this.Chart._topOuterLegendPanel.Children.Clear();
			this.Chart._bottomOuterLegendPanel.Children.Clear();
			this.Chart._leftOuterLegendPanel.Children.Clear();
			this.Chart._rightOuterLegendPanel.Children.Clear();
			this.Chart._centerDockInsidePlotAreaPanel.Children.Clear();
			this.Chart._centerDockOutsidePlotAreaPanel.Children.Clear();
		}

		private void AttachEventToRetainOldScrollOffsetOfScrollViewer()
		{
			bool arg_06_0 = this._isFirstTimeRender;
		}

		private void RetainOldScrollOffsetOfScrollViewer()
		{
			if (this.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis && this.AxisX != null)
			{
				double offset = this.AxisX.GetScrollBarValueFromOffset(this.AxisX.CurrentScrollScrollBarOffset);
				offset = this.GetScrollingOffsetOfAxis(this.AxisX, offset);
				if (this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
				{
					this.Chart.PlotArea.ScrollToVerticalOffset(offset);
					return;
				}
				this.Chart.PlotArea.ScrollToHorizontalOffset(offset);
			}
		}

		internal double GetScrollingOffsetOfAxis(Axis axis, double offset)
		{
			Chart chart = axis.Chart as Chart;
			if (chart.ZoomingEnabled)
			{
				double result = 0.0;
				if (axis.ScrollViewerElement != null && axis.ScrollViewerElement.Children.Count > 0)
				{
					UIElement arg_54_0 = this.AxisX.ScrollViewerElement.Children[0];
					if (axis.AxisOrientation == AxisOrientation.Horizontal)
					{
						double num = this.ScrollableLength - this.PlotAreaScrollViewer.Width;
						result = offset * (num / 100.0);
					}
					else
					{
						double num2 = this.ScrollableLength - this.PlotAreaScrollViewer.Height;
						result = offset * (num2 / 100.0);
					}
				}
				return result;
			}
			return offset;
		}

		private void HideAllAxesScrollBars()
		{
			this.Chart._leftAxisScrollBar.Visibility = Visibility.Collapsed;
			this.Chart._rightAxisScrollBar.Visibility = Visibility.Collapsed;
			this.Chart._bottomAxisScrollBar.Visibility = Visibility.Collapsed;
			this.Chart._topAxisScrollBar.Visibility = Visibility.Collapsed;
		}

		private void ResetTitleAndLegendPannelsSize()
		{
			this.Chart._leftOuterTitlePanel.Width = 0.0;
			this.Chart._rightOuterTitlePanel.Width = 0.0;
			this.Chart._bottomOuterTitlePanel.Height = 0.0;
			this.Chart._topOuterTitlePanel.Height = 0.0;
			this.Chart._leftOuterLegendPanel.Width = 0.0;
			this.Chart._rightOuterLegendPanel.Width = 0.0;
			this.Chart._bottomOuterLegendPanel.Height = 0.0;
			this.Chart._topOuterLegendPanel.Height = 0.0;
		}

		private Size CalculatePlotAreaSize(Size boundingRec)
		{
			foreach (Legend current in this.Chart.Legends)
			{
				if (!current.DockInsidePlotArea && current.Enabled.Value && current.Visual != null)
				{
					Size size = Graphics.CalculateVisualSize(current.Visual);
					if (current.InternalVerticalAlignment == VerticalAlignment.Bottom || current.InternalVerticalAlignment == VerticalAlignment.Top)
					{
						if (boundingRec.Height >= size.Height)
						{
							boundingRec.Height -= size.Height;
						}
						if (current.InternalVerticalAlignment == VerticalAlignment.Bottom)
						{
							this.Chart._bottomOuterLegendPanel.Height += size.Height;
						}
						else
						{
							this.Chart._topOuterLegendPanel.Height += size.Height;
						}
					}
					else if ((current.InternalVerticalAlignment == VerticalAlignment.Center || current.InternalVerticalAlignment == VerticalAlignment.Stretch) && (current.InternalHorizontalAlignment == HorizontalAlignment.Left || current.InternalHorizontalAlignment == HorizontalAlignment.Right))
					{
						if (boundingRec.Width >= size.Width)
						{
							boundingRec.Width -= size.Width;
						}
						if (current.InternalHorizontalAlignment == HorizontalAlignment.Left)
						{
							this.Chart._leftOuterLegendPanel.Width += size.Width;
						}
						else
						{
							this.Chart._rightOuterLegendPanel.Width += size.Width;
						}
					}
				}
			}
			this.Chart._centerOuterGrid.Height = boundingRec.Height;
			this.Chart._centerOuterGrid.Width = boundingRec.Width;
			this.PlotAreaCanvas.Height = boundingRec.Height;
			this.PlotAreaCanvas.Width = boundingRec.Width;
			return boundingRec;
		}

		private void CreatePlotArea(Chart chart)
		{
			if (this.Chart.PlotArea == null)
			{
				this.Chart.PlotArea = new PlotArea
				{
					IsDefault = true
				};
				this.Chart.AddPlotAreaToChartRootElement();
			}
			this.Chart.PlotArea.Chart = this.Chart;
			if (chart._zoomOutTextBlock != null)
			{
				chart._zoomOutTextBlock.Foreground = DataSeries.CalculateFontColor(null, chart);
			}
			if (chart._showAllTextBlock != null)
			{
				chart._showAllTextBlock.Foreground = DataSeries.CalculateFontColor(null, chart);
			}
			if (this.Chart.PlotArea.Visual == null)
			{
				if (!string.IsNullOrEmpty(this.Chart.Theme))
				{
					this.Chart.PlotArea.ApplyStyleFromTheme(this.Chart, "PlotArea");
				}
				if (this.PlotAreaCanvas != null && this.PlotAreaCanvas.Parent != null)
				{
					(this.PlotAreaCanvas.Parent as Panel).Children.Remove(this.PlotAreaCanvas);
				}
				this.Chart.PlotArea.CreateVisualObject();
				this.PlotAreaCanvas = this.Chart.PlotArea.Visual;
				this.Chart.AttachEvents2Visual(this.Chart.PlotArea, this.PlotAreaCanvas);
			}
			else
			{
				this.Chart.PlotArea.UpdateProperties();
			}
			this.PlotAreaCanvas.MouseMove -= new MouseEventHandler(this.PlotAreaCanvas_MouseMove);
			this.PlotAreaCanvas.MouseLeave -= new MouseEventHandler(this.PlotAreaCanvas_MouseLeave);
			this.PlotAreaCanvas.MouseLeftButtonDown -= new MouseButtonEventHandler(this.PlotAreaCanvas_MouseLeftButtonDown);
			this.PlotAreaCanvas.MouseLeftButtonUp -= new MouseButtonEventHandler(this.PlotAreaCanvas_MouseLeftButtonUp);
			if (chart.ZoomingMode == ZoomingMode.MouseDrag || chart.ZoomingMode == ZoomingMode.MouseDragAndWheel)
			{
				this.PlotAreaCanvas.MouseMove += new MouseEventHandler(this.PlotAreaCanvas_MouseMove);
				this.PlotAreaCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(this.PlotAreaCanvas_MouseLeftButtonDown);
				this.PlotAreaCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(this.PlotAreaCanvas_MouseLeftButtonUp);
				this.PlotAreaCanvas.MouseLeave += new MouseEventHandler(this.PlotAreaCanvas_MouseLeave);
			}
			else if (chart.ZoomingMode == ZoomingMode.MouseWheel)
			{
				this.PlotAreaCanvas.MouseMove += new MouseEventHandler(this.PlotAreaCanvas_MouseMove);
				this.PlotAreaCanvas.MouseLeave += new MouseEventHandler(this.PlotAreaCanvas_MouseLeave);
			}
			this.Chart.PlotArea.DragEventHandler.DetachEvents(this.Chart.PlotArea.Visual);
			this.Chart.PlotArea.DragEventHandler.Attach(chart, this.PlotAreaCanvas, chart.PlotDetails.ChartOrientation);
			this.Chart.PlotArea.DragEventHandler.OnDrag -= new EventHandler<DragEventArgs>(this.DragEventHandler_OnDrag);
			this.Chart.PlotArea.DragEventHandler.DragStart -= new EventHandler<DragEventArgs>(this.DragEventHandler_DragStart);
			this.Chart.PlotArea.DragEventHandler.OnDrag += new EventHandler<DragEventArgs>(this.DragEventHandler_OnDrag);
			this.Chart.PlotArea.DragEventHandler.DragStart += new EventHandler<DragEventArgs>(this.DragEventHandler_DragStart);
			this.PlotAreaCanvas.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.PlotAreaCanvas.VerticalAlignment = VerticalAlignment.Stretch;
			this.PlotAreaCanvas.Margin = new Thickness(0.0);
			this.Chart.PlotArea.AttachHref(this.Chart, this.Chart.PlotArea.BorderElement, this.Chart.PlotArea.Href, this.Chart.PlotArea.HrefTarget);
			this.Chart.PlotArea.DetachToolTip(this.Chart.PlotArea.BorderElement);
			this.Chart.PlotArea.AttachToolTip(this.Chart, this.Chart.PlotArea, this.Chart.PlotArea.BorderElement);
		}

		private void DragEventHandler_DragStart(object sender, DragEventArgs e)
		{
			this._dragScrollBarOffset = this.AxisX.ScrollBarOffset;
		}

		private void DragEventHandler_OnDrag(object sender, DragEventArgs e)
		{
			double offset = e.GetOffset(this.ScrollableLength);
			double num;
			if (e.Orientataion == Orientation.Vertical)
			{
				num = this._dragScrollBarOffset + offset;
			}
			else
			{
				num = this._dragScrollBarOffset - offset;
			}
			num = ((num < 0.0) ? 0.0 : ((num > 1.0) ? 1.0 : num));
			this.AxisX.ScrollBarOffset = num;
		}

		internal Point GetPositionWithRespect2ChartArea(Point positionWithRespect2PlotArea)
		{
			Point plotAreaStartPosition = this.Chart.PlotArea.GetPlotAreaStartPosition();
			return new Point(positionWithRespect2PlotArea.X + plotAreaStartPosition.X, positionWithRespect2PlotArea.Y + plotAreaStartPosition.Y);
		}

		private void PlotAreaCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			bool flag = true;
			if (flag && this.Chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis && this.Chart.PlotDetails.ChartOrientation != ChartOrientationType.Circular && this.Chart.ZoomingEnabled && this._zoomStart)
			{
				if (this.AxisX.AxisOrientation == AxisOrientation.Horizontal)
				{
					this._actualZoomMaxPos = new Point(e.GetPosition(this.PlottingCanvas).X, this.PlottingCanvas.Height);
					if (this._actualZoomMaxPos.X < this._actualZoomMinPos.X)
					{
						Point actualZoomMaxPos = this._actualZoomMaxPos;
						this._actualZoomMaxPos = this._actualZoomMinPos;
						this._actualZoomMinPos = actualZoomMaxPos;
					}
				}
				else
				{
					this._actualZoomMaxPos = new Point(this.PlottingCanvas.Width, e.GetPosition(this.PlottingCanvas).Y);
					if (this._actualZoomMaxPos.Y < this._actualZoomMinPos.Y)
					{
						Point actualZoomMaxPos2 = this._actualZoomMaxPos;
						this._actualZoomMaxPos = this._actualZoomMinPos;
						this._actualZoomMinPos = actualZoomMaxPos2;
					}
				}
				this.Chart._zoomRectangle.SetValue(Canvas.LeftProperty, this._firstZoomRectPosOverPlotArea.X);
				this.Chart._zoomRectangle.SetValue(Canvas.TopProperty, this._firstZoomRectPosOverPlotArea.Y);
				double num;
				double num2;
				if (this.AxisX.AxisOrientation == AxisOrientation.Horizontal)
				{
					if (this._actualZoomMinPos.X < 0.0)
					{
						this._actualZoomMinPos.X = 0.0;
					}
					if (this._actualZoomMaxPos.X > this.ChartVisualCanvas.Width)
					{
						this._actualZoomMaxPos.X = this.ChartVisualCanvas.Width;
					}
					num = Graphics.PixelPositionToValue(0.0, this.ScrollableLength, this.AxisX.InternalAxisMinimum, this.AxisX.InternalAxisMaximum, this._actualZoomMinPos.X);
					num2 = Graphics.PixelPositionToValue(0.0, this.ScrollableLength, this.AxisX.InternalAxisMinimum, this.AxisX.InternalAxisMaximum, this._actualZoomMaxPos.X);
				}
				else
				{
					if (this._actualZoomMinPos.Y < 0.0)
					{
						this._actualZoomMinPos.Y = 0.0;
					}
					if (this._actualZoomMaxPos.Y > this.ChartVisualCanvas.Height)
					{
						this._actualZoomMaxPos.Y = this.ChartVisualCanvas.Height;
					}
					num = Graphics.PixelPositionToValue(this.ScrollableLength, 0.0, this.AxisX.InternalAxisMinimum, this.AxisX.InternalAxisMaximum, this._actualZoomMinPos.Y);
					num2 = Graphics.PixelPositionToValue(this.ScrollableLength, 0.0, this.AxisX.InternalAxisMinimum, this.AxisX.InternalAxisMaximum, this._actualZoomMaxPos.Y);
				}
				object obj;
				object obj2;
				if (this.AxisX.IsDateTimeAxis)
				{
					if (num < 0.0)
					{
						obj = this.AxisX.MinDate;
					}
					else
					{
						obj = DateTimeHelper.XValueToDateTime(this.AxisX.MinDate, num, this.AxisX.InternalIntervalType);
					}
					if (num2 < 0.0)
					{
						obj2 = this.AxisX.MinDate;
					}
					else
					{
						obj2 = DateTimeHelper.XValueToDateTime(this.AxisX.MinDate, num2, this.AxisX.InternalIntervalType);
					}
				}
				else
				{
					obj = num;
					obj2 = num2;
				}
				if (!obj.Equals(obj2) && this._zoomRegionSelected)
				{
					this.AxisX.Zoom(obj, obj2);
				}
				this.AxisX._zoomState.MinXValue = obj;
				this.AxisX._zoomState.MaxXValue = obj2;
				if (!this.AxisX._zoomState.MinXValue.Equals(this.AxisX._zoomState.MaxXValue) && this._zoomRegionSelected)
				{
					this.AxisX.FireZoomEvent(this.AxisX._zoomState, e);
				}
				this.Chart._zoomRectangle.Visibility = Visibility.Collapsed;
				this._zoomStart = false;
			}
		}

		private void PlotAreaCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			bool flag = true;
			if (flag && this.Chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis && this.Chart.PlotDetails.ChartOrientation != ChartOrientationType.Circular && this.Chart.ZoomingEnabled)
			{
				double x = e.GetPosition(this.Chart._zoomRectangle.Parent as Canvas).X;
				double y = e.GetPosition(this.Chart._zoomRectangle.Parent as Canvas).Y;
				this._actualZoomMinPos = new Point(e.GetPosition(this.PlottingCanvas).X, e.GetPosition(this.PlottingCanvas).Y);
				this.Chart._zoomRectangle.Visibility = Visibility.Visible;
				this.Chart._zoomRectangle.SetValue(Panel.ZIndexProperty, 90000);
				if (this.AxisX.AxisOrientation == AxisOrientation.Horizontal)
				{
					this.Chart._zoomRectangle.BorderThickness = new Thickness(0.7, 0.0, 0.7, 0.0);
					this._firstZoomRectPosOverPlotArea = new Point(x, 0.0);
				}
				else
				{
					this.Chart._zoomRectangle.BorderThickness = new Thickness(0.0, 0.7, 0.0, 0.7);
					this._firstZoomRectPosOverPlotArea = new Point((this.Chart._zoomRectangle.Parent as Canvas).ActualWidth + this.PLANK_THICKNESS, y);
				}
				this.Chart._zoomRectangle.SetValue(Canvas.LeftProperty, this._firstZoomRectPosOverPlotArea.X);
				this.Chart._zoomRectangle.SetValue(Canvas.TopProperty, this._firstZoomRectPosOverPlotArea.Y);
				this.Chart._zoomRectangle.Width = 0.0;
				this.Chart._zoomRectangle.Height = 0.0;
				this.Chart._toolTip.Hide();
				this._zoomStart = true;
				this._zoomRegionSelected = false;
				this.Chart._zoomRectangle.IsHitTestVisible = false;
			}
		}

		private void SetPosition4ZoomRectangle(double x, double y)
		{
			if (this._firstZoomRectPosOverPlotArea.X <= x)
			{
				if (this.AxisX.AxisOrientation == AxisOrientation.Horizontal)
				{
					this.Chart._zoomRectangle.Width = x - this._firstZoomRectPosOverPlotArea.X;
				}
				else
				{
					this.Chart._zoomRectangle.Width = this.PlottingCanvas.Width - this.PLANK_THICKNESS;
				}
			}
			else if (this.AxisX.AxisOrientation == AxisOrientation.Horizontal)
			{
				this.Chart._zoomRectangle.SetValue(Canvas.LeftProperty, x);
				this.Chart._zoomRectangle.Width = this._firstZoomRectPosOverPlotArea.X - x;
			}
			else
			{
				this.Chart._zoomRectangle.Width = this.PlottingCanvas.Width - this.PLANK_THICKNESS;
			}
			if (this._firstZoomRectPosOverPlotArea.Y <= y)
			{
				if (this.AxisX.AxisOrientation == AxisOrientation.Horizontal)
				{
					this.Chart._zoomRectangle.Height = this.PlottingCanvas.Height - this.PLANK_THICKNESS;
					return;
				}
				this.Chart._zoomRectangle.Height = y - this._firstZoomRectPosOverPlotArea.Y;
				return;
			}
			else
			{
				if (this.AxisX.AxisOrientation == AxisOrientation.Horizontal)
				{
					this.Chart._zoomRectangle.SetValue(Canvas.TopProperty, this.PlottingCanvas.Height);
					this.Chart._zoomRectangle.Height = this.PlottingCanvas.Height - this.PLANK_THICKNESS;
					return;
				}
				this.Chart._zoomRectangle.SetValue(Canvas.TopProperty, y);
				this.Chart._zoomRectangle.Height = this._firstZoomRectPosOverPlotArea.Y - y;
				return;
			}
		}

		private void PlotAreaCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.Chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis && this.Chart.PlotDetails.ChartOrientation != ChartOrientationType.Circular)
			{
				bool flag = true;
				if (flag && this.Chart.ZoomingEnabled && this._zoomStart)
				{
					if (this._firstZoomRectPosOverPlotArea.X < 0.0 || this._firstZoomRectPosOverPlotArea.Y < 0.0)
					{
						this.PositionIndicator(this.Chart, e, double.NaN, double.NaN);
						return;
					}
					double x = e.GetPosition(this.Chart._zoomRectangle.Parent as Canvas).X;
					double y = e.GetPosition(this.Chart._zoomRectangle.Parent as Canvas).Y;
					this.SetPosition4ZoomRectangle(x, y);
					this._zoomRegionSelected = true;
				}
				this.PositionIndicator(this.Chart, e, double.NaN, double.NaN);
			}
		}

		public void HideIndicator()
		{
			this.DisableIndicators();
			foreach (DataSeries current in this.Chart.InternalSeries)
			{
				current.HideToolTip();
			}
		}

		public void PositionIndicator(Chart chart, double internalXValue, double internalYValue)
		{
			this.PositionIndicator(chart, null, internalXValue, internalYValue);
		}

		public void PositionIndicator(Chart chart, MouseEventArgs e, double xValueAtMousePos, double internalYValue)
		{
			if (this.Chart.IndicatorEnabled)
			{
				this._storedIndicatorEventArgs = e;
				double num = (double)this.Chart.PlotArea.GetValue(Canvas.LeftProperty);
				double num2 = (double)this.Chart.PlotArea.GetValue(Canvas.TopProperty);
				if (this.Chart.Bevel)
				{
					num += 5.0;
					num2 += 5.0;
				}
				List<IDataPoint> list;
				if (e == null)
				{
					list = (from ds in chart.InternalSeries
					where !RenderHelper.IsAxisIndependentCType(ds) && !RenderHelper.IsCircularCType(ds)
					select ds.FindNearestDataPointFromValues(xValueAtMousePos, internalYValue) into dp
					where dp != null && dp.Parent._nearestDataPoint != null
					select dp).ToList<IDataPoint>();
				}
				else
				{
					list = (from ds in chart.InternalSeries
					where !RenderHelper.IsAxisIndependentCType(ds) && !RenderHelper.IsCircularCType(ds)
					select ds.FindNearestDataPointFromMousePointer(e) into dp
					where dp != null && dp.Parent._nearestDataPoint != null
					select dp).ToList<IDataPoint>();
				}
				List<IDataPoint> list2 = new List<IDataPoint>();
				foreach (IDataPoint current in list)
				{
					DataSeries parent = current.Parent;
					if (parent.ToolTipElement != null)
					{
						parent.SetToolTipProperties(current);
						list2.Add(current);
					}
					parent.HideToolTip();
				}
				if (e != null)
				{
					xValueAtMousePos = RenderHelper.CalculateInternalXValueFromPixelPos(chart, chart.ChartArea.AxisX, e);
				}
				IDataPoint nearestDataPoint1 = RenderHelper.GetNearestDataPointAlongXAxis(list2, chart.ChartArea.AxisX, xValueAtMousePos);
				List<IDataPoint> list3 = new List<IDataPoint>();
				if (nearestDataPoint1 != null)
				{
					list3 = (from dataPoint in list2
					where dataPoint.DpInfo.ActualNumericXValue == nearestDataPoint1.DpInfo.ActualNumericXValue
					orderby dataPoint.DpInfo.VisualPosition.Y
					select dataPoint).TakeWhile((IDataPoint dp) => dp.Parent.HideToolTip()).ToList<IDataPoint>();
				}
				list2 = null;
				if (list3.Count > 0)
				{
					this.CreateIndicators();
					this._verticalLineIndicator.Visibility = Visibility.Visible;
					this._axisCallOutContainer.Visibility = Visibility.Visible;
					this._axisIndicatorBorderElement.Visibility = Visibility.Visible;
					this.ArrangeToolTips4DataSeries(list3.ToList<IDataPoint>(), num, num2);
					foreach (IDataPoint current2 in list3)
					{
						DataSeries parent2 = current2.Parent;
						double num3 = this.AxisX.GetScrollBarValueFromOffset(this.AxisX.CurrentScrollScrollBarOffset);
						num3 = this.GetScrollingOffsetOfAxis(this.AxisX, num3);
						if (this.AxisX.AxisOrientation == AxisOrientation.Vertical)
						{
							if (current2.DpInfo.VisualPosition.Y - num3 > this.PlotAreaCanvas.Height || current2.DpInfo.VisualPosition.Y - num3 < 0.0)
							{
								parent2.HideToolTip();
								this.DisableIndicators();
							}
						}
						else if (current2.DpInfo.VisualPosition.X - num3 > this.PlotAreaCanvas.Width || current2.DpInfo.VisualPosition.X - num3 < 0.0)
						{
							parent2.HideToolTip();
							this.DisableIndicators();
						}
					}
					if (this.Chart._toolTipCanvas.Clip != null)
					{
						Rect rect = new Rect(0.0, -this.PLANK_DEPTH, this.Chart.ActualWidth, this.Chart.ActualHeight + this.PLANK_DEPTH);
						RectangleGeometry rectangleGeometry = this.Chart._toolTipCanvas.Clip as RectangleGeometry;
						if (!rectangleGeometry.Rect.Equals(rect))
						{
							rectangleGeometry.Rect = rect;
							return;
						}
					}
					else
					{
						RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
						rectangleGeometry2.Rect = new Rect(0.0, -this.PLANK_DEPTH, this.Chart.ActualWidth, this.Chart.ActualHeight + this.PLANK_DEPTH);
						this.Chart._toolTipCanvas.Clip = rectangleGeometry2;
					}
				}
			}
		}

		private void CreateVerticalLineIndicator()
		{
			this._verticalLineIndicator = new Line();
			this._verticalLineIndicator.SetValue(Panel.ZIndexProperty, -1);
			this._verticalLineIndicator.Stroke = this.Chart.IndicatorLineColor;
			this._verticalLineIndicator.StrokeThickness = 1.0;
			this._verticalLineIndicator.IsHitTestVisible = false;
			this.Chart._toolTipCanvas.Children.Add(this._verticalLineIndicator);
		}

		private void CreateIndicators()
		{
			if (this._axisCallOutContainer == null && this._axisIndicatorBorderElement == null)
			{
				this._axisCallOutContainer = new Canvas();
				this._axisCallOutContainer.SetValue(Panel.ZIndexProperty, -1);
				this._axisCallOutContainer.Height = 8.0;
				this._axisCallOutContainer.HorizontalAlignment = HorizontalAlignment.Center;
				PathGeometry pathGeometry = new PathGeometry();
				this._callOutPath4AxisIndicator = new Path();
				this._callOutPath4AxisIndicator.Fill = this.Chart.AxisIndicatorBackground;
				this._callOutPath4AxisIndicator.Data = pathGeometry;
				PathFigure pathFigure = new PathFigure();
				pathFigure.StartPoint = new Point(-6.0, 8.0);
				pathGeometry.Figures.Add(pathFigure);
				LineSegment lineSegment = new LineSegment();
				lineSegment.Point = new Point(0.0, 0.0);
				pathFigure.Segments.Add(lineSegment);
				lineSegment = new LineSegment();
				lineSegment.Point = new Point(6.0, 8.0);
				pathFigure.Segments.Add(lineSegment);
				this._axisCallOutContainer.Children.Add(this._callOutPath4AxisIndicator);
				this._axisIndicatorBorderElement = new Border();
				this._axisIndicatorBorderElement.SetValue(Panel.ZIndexProperty, -1);
				this._axisIndicatorBorderElement.CornerRadius = new CornerRadius(3.0);
				this._axisIndicatorBorderElement.MinWidth = 25.0;
				this._axisIndicatorBorderElement.HorizontalAlignment = HorizontalAlignment.Center;
				this._axisIndicatorBorderElement.Padding = new Thickness(5.0, 0.0, 5.0, 0.0);
				this._axisIndicatorBorderElement.Background = this.Chart.AxisIndicatorBackground;
				this._axisIndicatorTextBlock = new TextBlock();
				this._axisIndicatorTextBlock.FlowDirection = ((this.Chart != null) ? this.Chart.FlowDirection : FlowDirection.LeftToRight);
				this._axisIndicatorTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
				this._axisIndicatorTextBlock.Foreground = this.Chart.AxisIndicatorFontColor;
				this._axisIndicatorTextBlock.FontSize = this.Chart.AxisIndicatorFontSize;
				this._axisIndicatorBorderElement.Child = this._axisIndicatorTextBlock;
				this.Chart._toolTipCanvas.Children.Add(this._axisCallOutContainer);
				this.Chart._toolTipCanvas.Children.Add(this._axisIndicatorBorderElement);
				this.CreateVerticalLineIndicator();
			}
		}

		private void SetPositon4Indicators(Axis axis, double actualPos, string axisIndicatorText, double xPlotCanvasOffset, double yPlotCanvasOffset, double plotWidth, double plotHeight)
		{
			this._axisIndicatorTextBlock.Text = axisIndicatorText;
			double num = axis.GetScrollBarValueFromOffset(axis.CurrentScrollScrollBarOffset);
			num = this.GetScrollingOffsetOfAxis(axis, num);
			double num2;
			double num3;
			if (axis.AxisOrientation == AxisOrientation.Horizontal)
			{
				this._axisIndicatorBorderElement.MinWidth = 25.0;
				this._axisCallOutContainer.Height = 8.0;
				this._axisCallOutContainer.Width = double.NaN;
				if (this._verticalLineIndicator.Parent != null)
				{
					this.Chart._toolTipCanvas.Children.Remove(this._verticalLineIndicator);
					this.CreateVerticalLineIndicator();
				}
				if (this._axisIndicatorBorderElement.RenderTransform.GetType() == typeof(RotateTransform))
				{
					(this._axisIndicatorBorderElement.RenderTransform as RotateTransform).Angle = 0.0;
					this._axisIndicatorBorderElement.RenderTransformOrigin = new Point(0.0, 0.0);
				}
				(this._callOutPath4AxisIndicator.Data as PathGeometry).Figures[0].StartPoint = new Point(-6.0, 8.0);
				((this._callOutPath4AxisIndicator.Data as PathGeometry).Figures[0].Segments[0] as LineSegment).Point = new Point(0.0, 0.0);
				((this._callOutPath4AxisIndicator.Data as PathGeometry).Figures[0].Segments[1] as LineSegment).Point = new Point(6.0, 8.0);
				this._verticalLineIndicator.SetValue(Canvas.LeftProperty, actualPos - xPlotCanvasOffset - num);
				this._verticalLineIndicator.X1 = xPlotCanvasOffset;
				this._verticalLineIndicator.Y1 = yPlotCanvasOffset;
				this._verticalLineIndicator.X2 = xPlotCanvasOffset;
				this._verticalLineIndicator.Y2 = plotHeight + yPlotCanvasOffset;
				num2 = actualPos - num;
				num3 = plotHeight + yPlotCanvasOffset;
				num3 += ((axis.ScrollBarElement.Visibility == Visibility.Visible) ? axis.ScrollBarElement.Height : 0.0);
				this._axisCallOutContainer.SetValue(Canvas.LeftProperty, num2);
				this._axisCallOutContainer.SetValue(Canvas.TopProperty, num3);
				num2 += -this._axisIndicatorBorderElement.ActualWidth / 2.0;
				num3 += 6.0;
				if (num2 < 0.0)
				{
					num2 = 0.0;
				}
				else if (num2 + this._axisIndicatorTextBlock.ActualWidth > this.Chart.ActualWidth)
				{
					num2 = this.Chart.ActualWidth - this._axisIndicatorTextBlock.ActualWidth - this.Chart.Padding.Right;
				}
				this._axisIndicatorBorderElement.SetValue(Canvas.LeftProperty, num2);
				this._axisIndicatorBorderElement.SetValue(Canvas.TopProperty, num3);
				return;
			}
			if (this._axisIndicatorBorderElement.ActualWidth + this._axisCallOutContainer.Width > axis.Width - this.Chart.Padding.Left)
			{
				this._axisIndicatorBorderElement.MinHeight = 0.0;
				this._axisCallOutContainer.Width = 8.0;
				if (this._verticalLineIndicator.Parent != null)
				{
					this.Chart._toolTipCanvas.Children.Remove(this._verticalLineIndicator);
					this.CreateVerticalLineIndicator();
				}
				RotateTransform rotateTransform = new RotateTransform();
				rotateTransform.Angle = 90.0;
				rotateTransform.CenterX = this._axisIndicatorBorderElement.ActualWidth / 2.0;
				rotateTransform.CenterY = this._axisIndicatorBorderElement.ActualHeight / 2.0;
				this._axisIndicatorBorderElement.RenderTransformOrigin = new Point(0.0, 0.0);
				this._axisIndicatorBorderElement.RenderTransform = rotateTransform;
				num2 = xPlotCanvasOffset - this._axisCallOutContainer.Width;
				num3 = actualPos - num - this._axisCallOutContainer.Height / 2.0;
				num2 -= ((axis.ScrollBarElement.Visibility == Visibility.Visible) ? axis.ScrollBarElement.Width : 0.0);
				this._axisCallOutContainer.SetValue(Canvas.LeftProperty, num2);
				this._axisCallOutContainer.SetValue(Canvas.TopProperty, num3);
				num2 -= this._axisIndicatorBorderElement.ActualWidth / 2.0 + this._axisCallOutContainer.Width;
				num3 -= this._axisCallOutContainer.Height / 2.0;
				if (num3 < this._axisIndicatorTextBlock.ActualWidth / 2.0)
				{
					num3 = this._axisIndicatorTextBlock.ActualWidth / 2.0;
				}
				else if (num3 + this._axisIndicatorTextBlock.ActualWidth / 2.0 > this.Chart.ActualHeight)
				{
					num3 = this.Chart.ActualHeight - this._axisIndicatorTextBlock.ActualWidth / 2.0 - this._axisIndicatorBorderElement.ActualHeight / 2.0 - this.Chart.Padding.Bottom;
				}
			}
			else
			{
				this._axisIndicatorBorderElement.MinWidth = 0.0;
				this._axisIndicatorBorderElement.MinHeight = 0.0;
				this._axisCallOutContainer.Width = 8.0;
				if (this._axisIndicatorBorderElement.RenderTransform.GetType() == typeof(RotateTransform))
				{
					(this._axisIndicatorBorderElement.RenderTransform as RotateTransform).Angle = 0.0;
					this._axisIndicatorBorderElement.RenderTransformOrigin = new Point(0.0, 0.0);
				}
				if (this._verticalLineIndicator.Parent != null)
				{
					this.Chart._toolTipCanvas.Children.Remove(this._verticalLineIndicator);
					this.CreateVerticalLineIndicator();
				}
				num2 = xPlotCanvasOffset - this._axisCallOutContainer.Width;
				num3 = actualPos - num - this._axisCallOutContainer.Height / 2.0;
				num2 -= ((axis.ScrollBarElement.Visibility == Visibility.Visible) ? axis.ScrollBarElement.Width : 0.0);
				this._axisCallOutContainer.SetValue(Canvas.LeftProperty, num2);
				this._axisCallOutContainer.SetValue(Canvas.TopProperty, num3);
				num2 -= this._axisIndicatorBorderElement.ActualWidth;
				num3 -= this._axisCallOutContainer.Height / 2.0;
			}
			(this._callOutPath4AxisIndicator.Data as PathGeometry).Figures[0].StartPoint = new Point(0.0, 0.0);
			((this._callOutPath4AxisIndicator.Data as PathGeometry).Figures[0].Segments[0] as LineSegment).Point = new Point(8.0, 4.0);
			((this._callOutPath4AxisIndicator.Data as PathGeometry).Figures[0].Segments[1] as LineSegment).Point = new Point(0.0, 8.0);
			this._verticalLineIndicator.SetValue(Canvas.TopProperty, actualPos - yPlotCanvasOffset - num);
			this._verticalLineIndicator.X1 = xPlotCanvasOffset;
			this._verticalLineIndicator.Y1 = yPlotCanvasOffset;
			this._verticalLineIndicator.X2 = xPlotCanvasOffset + plotWidth;
			this._verticalLineIndicator.Y2 = yPlotCanvasOffset;
			this._axisIndicatorBorderElement.SetValue(Canvas.LeftProperty, num2);
			this._axisIndicatorBorderElement.SetValue(Canvas.TopProperty, num3);
		}

		private void PlotAreaCanvas_MouseLeave(object sender, MouseEventArgs e)
		{
			foreach (DataSeries current in this.Chart.Series)
			{
				if (current.ToolTipElement != null)
				{
					current.ToolTipElement.Hide();
				}
			}
			this.DisableIndicators();
			this._zoomStart = false;
			this.Chart._zoomRectangle.Visibility = Visibility.Collapsed;
		}

		private Size CalculateLegendMaxSize(Size boundingRec)
		{
			foreach (Title current in this.Chart.Titles)
			{
				if (!current.DockInsidePlotArea && current.Enabled.Value && current.Visual != null)
				{
					Size size = Graphics.CalculateVisualSize(current.Visual);
					if (current.InternalVerticalAlignment == VerticalAlignment.Bottom || current.InternalVerticalAlignment == VerticalAlignment.Top)
					{
						if (boundingRec.Height - size.Height >= 0.0)
						{
							boundingRec.Height -= size.Height;
						}
						if (current.InternalVerticalAlignment == VerticalAlignment.Bottom)
						{
							this.Chart._bottomOuterTitlePanel.Height += size.Height;
						}
						else
						{
							this.Chart._topOuterTitlePanel.Height += size.Height;
						}
					}
					else if ((current.InternalVerticalAlignment == VerticalAlignment.Center || current.InternalVerticalAlignment == VerticalAlignment.Stretch) && (current.InternalHorizontalAlignment == HorizontalAlignment.Left || current.InternalHorizontalAlignment == HorizontalAlignment.Right))
					{
						if (boundingRec.Width - size.Width >= 0.0)
						{
							boundingRec.Width -= size.Width;
						}
						if (current.InternalHorizontalAlignment == HorizontalAlignment.Left)
						{
							this.Chart._leftOuterTitlePanel.Width += size.Width;
						}
						else
						{
							this.Chart._rightOuterTitlePanel.Width += size.Width;
						}
					}
				}
			}
			return boundingRec;
		}

		private void PopulateInternalAxesXList()
		{
			if (this.Chart.InternalAxesX != null)
			{
				foreach (Axis current in this.Chart.InternalAxesX)
				{
					if (current.Visual != null)
					{
						current.DetachToolTip(current.Visual);
					}
				}
				this.Chart.InternalAxesX.Clear();
			}
			this.Chart.InternalAxesX = this.Chart.AxesX.ToList<Axis>();
		}

		private void PopulateInternalAxesYList()
		{
			if (this.Chart.InternalAxesY != null)
			{
				foreach (Axis current in this.Chart.InternalAxesY)
				{
					if (current.Visual != null)
					{
						current.DetachToolTip(current.Visual);
					}
				}
				this.Chart.InternalAxesY.Clear();
			}
			this.Chart.InternalAxesY = this.Chart.AxesY.ToList<Axis>();
		}

		private void ResetStoryboards4InternalDataPointsList()
		{
			foreach (DataSeries current in this.Chart.InternalSeries)
			{
				foreach (IDataPoint current2 in current.InternalDataPoints)
				{
					if (current2.DpInfo.Storyboard != null)
					{
						current2.DpInfo.Storyboard.Stop();
						current2.DpInfo.Storyboard.Children.Clear();
						current2.DpInfo.Storyboard.Remove(this.Chart._rootElement);
						current2.DpInfo.Storyboard = null;
					}
				}
				current.InternalDataPoints.Clear();
			}
		}

		private void PopulateInternalSeriesList(bool isSamplingEnabled)
		{
			if (this.Chart.InternalSeries != null)
			{
				if (this.Chart.AnimatedUpdate.Value)
				{
					this.ResetStoryboards4InternalDataPointsList();
				}
				if (!this.Chart.IsInDesignMode)
				{
					this.Chart.InternalSeries.Clear();
				}
			}
			List<DataSeries> list = (from ds in this.Chart.Series
			where ds.Enabled.Value
			orderby ds.ZIndex
			select ds).ToList<DataSeries>();
			if (list.Count == 0)
			{
				if (this.Chart.IsInDesignMode)
				{
					if (this.Chart.InternalSeries == null)
					{
						this.Chart.InternalSeries = new List<DataSeries>();
					}
					this.AddDefaultDataSeriesInDesignMode();
					this.SetDataPointColorFromColorSet(this.Chart.InternalSeries);
				}
				else
				{
					this.Chart.InternalSeries = new List<DataSeries>();
					this.SetBlankSeries();
				}
				this._isDefaultSeriesSet = true;
			}
			else
			{
				this.Chart.InternalSeries = list;
				this._isDefaultSeriesSet = false;
			}
			double minXPosition = double.NaN;
			double plotXValueDistance = isSamplingEnabled ? this.CalculateXValueDistanceAndMinX(out minXPosition) : double.NaN;
			foreach (DataSeries current in this.Chart.InternalSeries)
			{
				if (current.SelectionEnabled)
				{
					if (current.ListOfSelectedDataPoints != null)
					{
						current.ListOfSelectedDataPoints.Clear();
					}
					else
					{
						current.ListOfSelectedDataPoints = new List<IDataPoint>();
					}
				}
				List<IDataPoint> list2 = null;
				if (current.DataPoints != null)
				{
					list2 = (isSamplingEnabled ? this.ApplySampling(current, current.DataPoints.ToList<IDataPoint>(), minXPosition, plotXValueDistance) : current.DataPoints.ToList<IDataPoint>());
				}
				if (this._isFirstTimeRender)
				{
					current.InternalDataPoints = new List<IDataPoint>();
					if (list2 == null)
					{
						continue;
					}
					using (List<IDataPoint>.Enumerator enumerator2 = list2.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							IDataPoint current2 = enumerator2.Current;
							current.InternalDataPoints.Add(current2);
							if (current.ListOfSelectedDataPoints != null && !current2.IsLightDataPoint && (current2 as DataPoint).Selected)
							{
								current.ListOfSelectedDataPoints.Add(current2);
							}
						}
						continue;
					}
				}
				current.InternalDataPoints.Clear();
				if (list2 != null)
				{
					foreach (IDataPoint current3 in list2)
					{
						current.InternalDataPoints.Add(current3);
						if (current.ListOfSelectedDataPoints != null && !current3.IsLightDataPoint && (current3 as DataPoint).Selected)
						{
							current.ListOfSelectedDataPoints.Add(current3);
						}
					}
				}
			}
		}

		private double CalculateXValueDistanceAndMinX(out double axisMinimum)
		{
			axisMinimum = double.NaN;
			Axis axisX = this.AxisX;
			axisX.ClearReferences();
			axisX.CreateVisualObject(this.Chart);
			axisMinimum = axisX.InternalAxisMinimum;
			return Math.Abs(axisX.InternalAxisMaximum - axisX.InternalAxisMinimum);
		}

		public List<IDataPoint> ApplySampling(DataSeries parentDs, List<IDataPoint> dataPoints, double minXPosition, double plotXValueDistance)
		{
			List<DataSamplingHelper.Point> actualList;
			if (parentDs.RenderAs == RenderAs.CandleStick || parentDs.RenderAs == RenderAs.Stock)
			{
				actualList = (from dataPoint in dataPoints
				where dataPoint.YValues != null
				select new DataSamplingHelper.Point(dataPoint.DpInfo.ActualNumericXValue, new double[]
				{
					(dataPoint.YValues.Count<double>() > 0) ? dataPoint.YValues[0] : 0.0,
					(dataPoint.YValues.Count<double>() > 1) ? dataPoint.YValues[1] : 0.0,
					(dataPoint.YValues.Count<double>() > 2) ? dataPoint.YValues[2] : 0.0,
					(dataPoint.YValues.Count<double>() > 3) ? dataPoint.YValues[3] : 0.0
				})).ToList<DataSamplingHelper.Point>();
			}
			else
			{
				actualList = (from dataPoint in dataPoints
				select new DataSamplingHelper.Point(dataPoint.DpInfo.ActualNumericXValue, new double[]
				{
					dataPoint.YValue
				})).ToList<DataSamplingHelper.Point>();
			}
			List<DataSamplingHelper.Point> list = DataSamplingHelper.Filter(actualList, minXPosition, plotXValueDistance, (parentDs.Chart as Chart).SamplingThreshold, SamplingFunction.Average);
			List<IDataPoint> list2 = new List<IDataPoint>();
			bool value = parentDs.LightWeight.Value;
			int num = 0;
			if (parentDs.RenderAs == RenderAs.CandleStick || parentDs.RenderAs == RenderAs.Stock)
			{
				using (List<DataSamplingHelper.Point>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DataSamplingHelper.Point current = enumerator.Current;
						IDataPoint dataPoint3;
						if (value)
						{
							dataPoint3 = new LightDataPoint
							{
								XValue = this.AxisX.CalculateDateTimeFromNumericXValue(current.XValue),
								YValues = new double[]
								{
									current.YValues[0],
									current.YValues[1],
									current.YValues[2],
									current.YValues[3]
								},
								Chart = this.Chart
							};
						}
						else
						{
							dataPoint3 = new DataPoint
							{
								XValue = this.AxisX.CalculateDateTimeFromNumericXValue(current.XValue),
								YValues = new double[]
								{
									current.YValues[0],
									current.YValues[1],
									current.YValues[2],
									current.YValues[3]
								},
								LegendText = (string.IsNullOrEmpty(parentDs.LegendText) ? ("DataPoint" + num++.ToString()) : null),
								Chart = this.Chart
							};
						}
						if (dataPoint3 != null)
						{
							(dataPoint3 as IDataPointParentSetter).Parent = parentDs;
						}
						dataPoint3.DpInfo.ActualNumericXValue = current.XValue;
						list2.Add(dataPoint3);
					}
					return list2;
				}
			}
			foreach (DataSamplingHelper.Point current2 in list)
			{
				IDataPoint dataPoint2;
				if (value)
				{
					dataPoint2 = new LightDataPoint
					{
						XValue = this.AxisX.CalculateDateTimeFromNumericXValue(current2.XValue),
						YValue = current2.YValues[0],
						Chart = this.Chart
					};
				}
				else
				{
					dataPoint2 = new DataPoint
					{
						XValue = this.AxisX.CalculateDateTimeFromNumericXValue(current2.XValue),
						YValue = current2.YValues[0],
						LegendText = (string.IsNullOrEmpty(parentDs.LegendText) ? ("DataPoint" + num++.ToString()) : null),
						Chart = this.Chart
					};
				}
				if (dataPoint2 != null)
				{
					(dataPoint2 as IDataPointParentSetter).Parent = parentDs;
				}
				dataPoint2.DpInfo.ActualNumericXValue = current2.XValue;
				list2.Add(dataPoint2);
			}
			return list2;
		}

		private void AddDefaultDataSeriesInDesignMode()
		{
			if (this.Chart.InternalSeries.Count == 1 && this.Chart.InternalSeries[0].Tag != null && this.Chart.InternalSeries[0].Tag.ToString().Equals("default_Series"))
			{
				return;
			}
			DataSeries dataSeries = new DataSeries
			{
				Tag = "default_Series"
			};
			dataSeries.RenderAs = RenderAs.Column;
			dataSeries.LightingEnabled = new bool?(true);
			dataSeries.ShadowEnabled = new bool?(true);
			dataSeries.Chart = this.Chart;
			IDataPoint dataPoint = new LightDataPoint();
			dataPoint.DpInfo.ActualNumericXValue = 1.0;
			dataPoint.YValue = 70.0;
			dataPoint.AxisXLabel = "Wall-Mart";
			dataPoint.Chart = this.Chart;
			dataSeries.DataPoints.Add(dataPoint);
			dataPoint = new LightDataPoint();
			dataPoint.DpInfo.ActualNumericXValue = 2.0;
			dataPoint.YValue = 40.0;
			dataPoint.AxisXLabel = "Exxon Mobil";
			dataPoint.Chart = this.Chart;
			dataSeries.DataPoints.Add(dataPoint);
			dataPoint = new LightDataPoint();
			dataPoint.DpInfo.ActualNumericXValue = 3.0;
			dataPoint.YValue = 60.0;
			dataPoint.AxisXLabel = "Shell";
			dataPoint.Chart = this.Chart;
			dataSeries.DataPoints.Add(dataPoint);
			dataPoint = new LightDataPoint();
			dataPoint.DpInfo.ActualNumericXValue = 4.0;
			dataPoint.YValue = 27.0;
			dataPoint.AxisXLabel = "BP";
			dataPoint.Chart = this.Chart;
			dataSeries.DataPoints.Add(dataPoint);
			dataPoint = new LightDataPoint();
			dataPoint.DpInfo.ActualNumericXValue = 5.0;
			dataPoint.YValue = 54.0;
			dataPoint.AxisXLabel = "General Motors";
			dataPoint.Chart = this.Chart;
			dataSeries.DataPoints.Add(dataPoint);
			this.Chart.InternalSeries.Add(dataSeries);
		}

		private void CalculatePlankParameters()
		{
			if (this.Chart.View3D && this.PlotDetails.ChartOrientation != ChartOrientationType.Circular)
			{
				if (this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					double num = 0.04;
					double num2 = 0.03;
					this.PLANK_DEPTH = ((this.Chart.ActualHeight > this.Chart.ActualWidth) ? this.Chart.ActualWidth : this.Chart.ActualHeight) * (num * (double)((this.PlotDetails.Layer3DCount == 0) ? 1 : this.PlotDetails.Layer3DCount));
					this.PLANK_THICKNESS = ((this.Chart.ActualHeight > this.Chart.ActualWidth) ? this.Chart.ActualWidth : this.Chart.ActualHeight) * num2;
					double num3 = this.Chart.ActualHeight * 40.0 / 100.0;
					if (this.PLANK_DEPTH > num3)
					{
						double num4 = this.Chart.ActualHeight / ((this.Chart.ActualWidth + this.Chart.ActualHeight) * (double)((this.PlotDetails.Layer3DCount == 0) ? 1 : this.PlotDetails.Layer3DCount));
						num = num4;
						this.PLANK_DEPTH = ((this.Chart.ActualHeight > this.Chart.ActualWidth) ? this.Chart.ActualWidth : this.Chart.ActualHeight) * (num * (double)((this.PlotDetails.Layer3DCount == 0) ? 1 : this.PlotDetails.Layer3DCount));
					}
				}
				else
				{
					double num5 = 0.015;
					double num6 = 0.025;
					this.PLANK_DEPTH = ((this.Chart.ActualHeight > this.Chart.ActualWidth) ? this.Chart.ActualWidth : this.Chart.ActualHeight) * (num5 * (double)this.PlotDetails.Layer3DCount);
					this.PLANK_THICKNESS = ((this.Chart.ActualHeight > this.Chart.ActualWidth) ? this.Chart.ActualWidth : this.Chart.ActualHeight) * num6;
				}
				this.PLANK_DEPTH = (double.IsNaN(this.PLANK_DEPTH) ? 12.0 : this.PLANK_DEPTH);
				this.PLANK_THICKNESS = (double.IsNaN(this.PLANK_THICKNESS) ? 5.0 : this.PLANK_THICKNESS);
				this.PLANK_OFFSET = this.PLANK_DEPTH + this.PLANK_THICKNESS;
				return;
			}
			this.PLANK_DEPTH = 0.0;
			this.PLANK_THICKNESS = 0.0;
			this.PLANK_OFFSET = 0.0;
		}

		internal void SetScrollBarChanges(Axis axis)
		{
			if (this.Chart != null && !this.Chart.ZoomingEnabled)
			{
				if (this.ScrollableLength < 1000.0)
				{
					axis.ScrollBarElement.SmallChange = 10.0;
					axis.ScrollBarElement.LargeChange = 50.0;
					return;
				}
				axis.ScrollBarElement.SmallChange = 20.0;
				axis.ScrollBarElement.LargeChange = 80.0;
			}
		}

		internal void ScrollBarElement_ScaleChanged(object sender, EventArgs e)
		{
			Axis axis = sender as Axis;
			Chart chart = this.Chart;
			axis._internalZoomingScale = axis.ScrollBarElement.Scale;
			this.OnScrollBarScaleChanged(chart);
			chart.Dispatcher.BeginInvoke(new Action<EventArgs>(this.FireZoomEvent), new object[]
			{
				e
			});
		}

		internal void OnScrollBarScaleChanged(Chart chart)
		{
			this._isDragging = true;
			if (chart.Series.Count > 0)
			{
				Dispatcher arg_45_0 = chart.Dispatcher;
				Delegate arg_45_1 = new Action<VcProperties, object>(chart.Series[0].UpdateVisual);
				object[] array = new object[2];
				array[0] = VcProperties.ScrollBarScale;
				arg_45_0.BeginInvoke(arg_45_1, array);
				chart.Dispatcher.BeginInvoke(new Action(this.ActivateDraggingLock), new object[0]);
			}
		}

		private void FireZoomEvent(EventArgs e)
		{
			this.AxisX._zoomState.MinXValue = this.AxisX.ViewMinimum;
			this.AxisX._zoomState.MaxXValue = this.AxisX.ViewMaximum;
			this.AxisX.FireZoomEvent(this.AxisX._zoomState, e);
		}

		private void ActivateDraggingLock()
		{
			this._isDragging = false;
		}

		private void ResetStoryboards()
		{
			if (this.Chart._internalAnimationEnabled)
			{
				foreach (DataSeries current in this.Chart.InternalSeries)
				{
					if (current.Storyboard != null && current.Storyboard.GetValue(Storyboard.TargetProperty) != null)
					{
						current.Storyboard.Stop();
					}
					if (current.Storyboard != null)
					{
						current.Storyboard.Children.Clear();
					}
					current.Storyboard = null;
					current.Faces = null;
				}
			}
		}

		private Size RenderAxes(Size plotAreaSize)
		{
			Axis.SaveOldValueOfAxisRange(this.Chart.ChartArea.AxisX);
			Axis.SaveOldValueOfAxisRange(this.Chart.ChartArea.AxisY);
			Axis.SaveOldValueOfAxisRange(this.Chart.ChartArea.AxisX2);
			Axis.SaveOldValueOfAxisRange(this.Chart.ChartArea.AxisY2);
			try
			{
				if (this.Chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					plotAreaSize = AxisRenderManager.CreateAxisForVerticalChart(this, plotAreaSize);
				}
				else if (this.Chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
				{
					plotAreaSize = AxisRenderManager.CreateAxisForHorizontalChart(this, plotAreaSize);
				}
				else
				{
					AxisRenderManager.UpdateLayoutSettings(this, plotAreaSize);
				}
				this.Chart.PlotArea.GetPlotAreaStartPosition();
				this._plotAreaSizeAfterDrawingAxes = plotAreaSize;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message, ex);
			}
			return plotAreaSize;
		}

		private Size DrawChart(Size plotAreaSize)
		{
			this.ResetStoryboards();
			Size size = this.RenderAxes(plotAreaSize);
			size = this.RecalculateAxisAndRerenderBasedOnChartTypes(plotAreaSize, size);
			this.RenderChart(size, AxisRepresentations.AxisX, false);
			return size;
		}

		private Size RecalculateAxisAndRerenderBasedOnChartTypes(Size plotAreaSize, Size remainingSizeAfterDrawingAxes)
		{
			if (this.PlotDetails.AutoFitToPlotArea)
			{
				bool flag = false;
				foreach (PlotGroup current in this.PlotDetails.PlotGroups)
				{
					if (current.RenderAs == RenderAs.Bubble)
					{
						current._initialAxisXMin = current.AxisX.InternalAxisMinimum;
						current._initialAxisXMax = current.AxisX.InternalAxisMaximum;
						current._intialAxisXWidth = current.AxisX.Width;
						current._initialAxisYMin = current.AxisY.InternalAxisMinimum;
						current._initialAxisYMax = current.AxisY.InternalAxisMaximum;
						current._intialAxisYHeight = current.AxisY.Height;
						current.Update(VcProperties.None, null, null);
						flag = true;
					}
				}
				if (flag)
				{
					remainingSizeAfterDrawingAxes = this.RenderAxes(plotAreaSize);
				}
				return remainingSizeAfterDrawingAxes;
			}
			return remainingSizeAfterDrawingAxes;
		}

		private void UpdateLayoutSettings(Size newSize)
		{
			this.Chart.PlotArea.Height = newSize.Height;
			this.Chart.PlotArea.Width = newSize.Width;
			this.Chart._drawingCanvas.Height = newSize.Height;
			this.Chart._drawingCanvas.Width = newSize.Width;
			this.Chart.PlotArea.BorderElement.Height = newSize.Height;
			this.Chart.PlotArea.BorderElement.Width = newSize.Width;
			double num;
			if (this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				num = newSize.Height;
			}
			else
			{
				num = newSize.Width;
			}
			if (this.Chart.ScrollingEnabled.Value)
			{
				if (this.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis && this.Chart.InternalSeries.Count > 0)
				{
					num = this.CalculatePlotAreaAutoSize(num);
				}
				if (double.IsNaN(newSize.Height) || newSize.Height <= 0.0 || double.IsNaN(newSize.Width) || newSize.Width <= 0.0)
				{
					return;
				}
				this.Chart.IsScrollingActivated = false;
				if (this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					if (num <= newSize.Width)
					{
						num = newSize.Width;
					}
					else
					{
						this.Chart.IsScrollingActivated = true;
					}
					this.Chart._drawingCanvas.Width = num;
					this.Chart.PlotArea.BorderElement.Width = num;
				}
				if (this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
				{
					if (num <= newSize.Height)
					{
						num = newSize.Height;
					}
					else
					{
						this.Chart.IsScrollingActivated = true;
					}
					this.Chart._drawingCanvas.Height = num;
					this.Chart.PlotArea.BorderElement.Height = num;
				}
			}
			else
			{
				this.Chart.IsScrollingActivated = false;
			}
			this.ScrollableLength = num;
			this.Chart._plotAreaScrollViewer.Height = newSize.Height;
			this.Chart._plotAreaScrollViewer.UpdateLayout();
			this.PlotAreaScrollViewer = this.Chart._plotAreaScrollViewer;
			if (this.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis && !double.IsNaN(this.AxisX.InternalAxisMinimum) && !double.IsNaN(this.AxisX.InternalAxisMaximum))
			{
				double num3;
				if (!this.Chart.ZoomingEnabled && ((this.AxisY != null && this.AxisY.ViewportRangeEnabled) || (this.AxisY2 != null && this.AxisY2.ViewportRangeEnabled)))
				{
					double num2 = this.AxisX.GetScrollBarValueFromOffset(this.AxisX.CurrentScrollScrollBarOffset);
					num2 = this.GetScrollingOffsetOfAxis(this.AxisX, num2);
					num3 = num2;
				}
				else
				{
					num3 = this.AxisX._oldScrollBarOffsetInPixel;
				}
				if (double.IsNaN(num3))
				{
					if (this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
					{
						num3 = this.AxisX.ScrollBarElement.Maximum;
					}
					else
					{
						num3 = 0.0;
					}
				}
				if (this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					this.AxisX.CalculateViewMinimumAndMaximum(this.Chart, num3, num, newSize.Height);
					return;
				}
				this.AxisX.CalculateViewMinimumAndMaximum(this.Chart, num3, newSize.Width, num);
			}
		}

		internal double CalculateChartSizeForZooming(Chart chart, double currentSize)
		{
			double result;
			if (this._isFirstTimeRender || chart._clearAndResetZoomState)
			{
				this.Chart.AxesX[0]._internalZoomingScale = Axis.INTERNAL_MINIMUM_ZOOMING_SCALE;
				result = currentSize + currentSize * this.Chart.AxesX[0]._internalZoomingScale;
				if (this.AxisX.ScrollBarElement != null)
				{
					this.AxisX.ScrollBarElement.ResetThumSize();
				}
				this.AxisX.ResetZoomState(chart, false);
				this.AxisX._internalOldZoomBarValue = this.AxisX.ScrollBarElement.Value;
				this.AxisX._internalOldZoomingScale = this.AxisX.ScrollBarElement.Scale;
			}
			else
			{
				if (Axis.INTERNAL_MINIMUM_ZOOMING_SCALE != this.AxisX._internalOldZoomingScale && !this._isDragging)
				{
					if (!double.IsNaN(this.Chart.AxesX[0].ScrollBarElement.Scale) && !double.IsNaN(this.Chart.AxesX[0].ScrollBarElement.ThumbSize) && this.zoomCount != 0)
					{
						this.Chart.AxesX[0]._internalZoomingScale = this.Chart.AxesX[0].ScrollBarElement.Scale;
					}
					else
					{
						if (!double.IsNaN(this.AxisX._internalOldZoomBarValue))
						{
							this.AxisX.ScrollBarElement.Value = this.AxisX._internalOldZoomBarValue;
						}
						if (double.IsNaN(this.AxisX._internalOldZoomingScale))
						{
							return currentSize + currentSize * this.Chart.AxesX[0]._internalZoomingScale;
						}
						this.AxisX.ScrollBarElement.Scale = this.AxisX._internalOldZoomingScale;
						this.AxisX._internalZoomingScale = this.AxisX._internalOldZoomingScale;
					}
				}
				if (this.AxisX._internalZoomingScale == 0.0)
				{
					chart.AxesX[0]._internalZoomingScale = Axis.INTERNAL_MINIMUM_ZOOMING_SCALE;
					result = currentSize + currentSize * this.Chart.AxesX[0]._internalZoomingScale;
				}
				else
				{
					result = currentSize / chart.AxesX[0]._internalZoomingScale;
				}
			}
			return result;
		}

		internal double CalculatePlotAreaAutoSize(double currentSize)
		{
			Chart chart = this.Chart;
			double num;
			if (chart.ZoomingEnabled)
			{
				num = this.CalculateChartSizeForZooming(chart, currentSize);
				if (num > ChartArea.MAX_PLOTAREA_SIZE)
				{
					num = ChartArea.MAX_PLOTAREA_SIZE;
				}
			}
			else
			{
				if (this.Chart.MinimumGap.HasValue)
				{
					num = this.Chart.MinimumGap.Value * (double)(from series in this.Chart.InternalSeries
					select series.InternalDataPoints.Count).Max();
				}
				else if (!double.IsNaN(this.Chart.AxesX[0].ClosestPlotDistance) && !double.IsNaN(this.AxisX.InternalAxisMinimum) && !double.IsNaN(this.AxisX.InternalAxisMaximum))
				{
					double internalAxisMinimum = this.AxisX.InternalAxisMinimum;
					double internalAxisMaximum = this.AxisX.InternalAxisMaximum;
					double maxOfMinDifferencesForXValue = this.PlotDetails.GetMaxOfMinDifferencesForXValue();
					double value = internalAxisMinimum;
					double value2 = internalAxisMinimum + maxOfMinDifferencesForXValue;
					double num2 = Graphics.ValueToPixelPosition(0.0, currentSize, internalAxisMinimum, internalAxisMaximum, value);
					double num3 = Graphics.ValueToPixelPosition(0.0, currentSize, internalAxisMinimum, internalAxisMaximum, value2);
					double num4 = num3 - num2;
					if (num4 > this.Chart.AxesX[0].ClosestPlotDistance)
					{
						num = currentSize;
					}
					else
					{
						num = currentSize * (this.Chart.AxesX[0].ClosestPlotDistance / num4);
					}
				}
				else if (!double.IsNaN(this.Chart.AxesX[0].ScrollBarScale) && !this.IsAutoCalculatedScrollBarScale)
				{
					num = currentSize / this.Chart.AxesX[0].ScrollBarScale;
				}
				else if (this._axisOverflowOccured)
				{
					num = currentSize;
				}
				else if (this.Chart.ScrollingActivatedForcefully)
				{
					double num5 = 5.0;
					num = currentSize + num5;
				}
				else if (this.PlotDetails.ListOfAllDataPoints.Count > 0)
				{
					double minOfMinDifferencesForXValue = this.PlotDetails.GetMinOfMinDifferencesForXValue();
					double num6 = -1.7976931348623157E+308;
					double num7 = 1.7976931348623157E+308;
					if (!this.AxisX.Logarithmic)
					{
						using (List<IDataPoint>.Enumerator enumerator = this.PlotDetails.ListOfAllDataPoints.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								IDataPoint current = enumerator.Current;
								num6 = Math.Max(num6, current.DpInfo.ActualNumericXValue);
								num7 = Math.Min(num7, current.DpInfo.ActualNumericXValue);
							}
							goto IL_2F8;
						}
					}
					foreach (IDataPoint current2 in this.PlotDetails.ListOfAllDataPoints)
					{
						num6 = Math.Max(num6, current2.DpInfo.InternalXValue);
						num7 = Math.Min(num7, current2.DpInfo.InternalXValue);
					}
					IL_2F8:
					double num8 = num6 - num7;
					double num9 = currentSize * 34.0 / (550.0 * (double)((this.PlotDetails.DrawingDivisionFactor == 0) ? 1 : this.PlotDetails.DrawingDivisionFactor));
					if (num8 / minOfMinDifferencesForXValue > num9 && num9 != 0.0)
					{
						num = num8 / minOfMinDifferencesForXValue * (550.0 * (double)((this.PlotDetails.DrawingDivisionFactor == 0) ? 1 : this.PlotDetails.DrawingDivisionFactor)) / 34.0;
					}
					else
					{
						num = currentSize;
					}
				}
				else
				{
					num = currentSize;
				}
				if (num > ChartArea.MAX_PLOTAREA_SIZE)
				{
					num = ChartArea.MAX_PLOTAREA_SIZE;
					this.Chart.AxesX[0].IsNotificationEnable = false;
					this.Chart.AxesX[0].ScrollBarScale = currentSize / num;
					this.Chart.AxesX[0].IsNotificationEnable = true;
				}
				if (double.IsNaN(this.Chart.AxesX[0].ScrollBarScale))
				{
					if (currentSize != 0.0 && num != 0.0)
					{
						this.Chart.AxesX[0].IsNotificationEnable = false;
						this.Chart.AxesX[0].ScrollBarScale = currentSize / num;
						this.Chart.AxesX[0].IsNotificationEnable = true;
					}
				}
				else if (!double.IsNaN(this.Chart.AxesX[0].ScrollBarScale) && this.IsAutoCalculatedScrollBarScale && currentSize != 0.0 && num != 0.0)
				{
					this.Chart.AxesX[0].IsNotificationEnable = false;
					this.Chart.AxesX[0].ScrollBarScale = currentSize / num;
					this.Chart.AxesX[0].IsNotificationEnable = true;
				}
			}
			return num;
		}

		private void SetBlankSeries()
		{
			DataSeries dataSeries = new DataSeries
			{
				_isAutoGenerated = true
			};
			dataSeries.IsNotificationEnable = false;
			dataSeries.RenderAs = RenderAs.Column;
			dataSeries.LightingEnabled = new bool?(true);
			dataSeries.ShadowEnabled = new bool?(false);
			dataSeries.Chart = this.Chart;
			for (int i = 1; i <= 5; i++)
			{
				IDataPoint dataPoint = new DataPoint();
				dataPoint.XValue = i;
				dataPoint.YValue = double.NaN;
				dataPoint.Color = Graphics.TRANSPARENT_BRUSH;
				dataPoint.AxisXLabel = i.ToString();
				dataPoint.Chart = this.Chart;
				dataSeries.DataPoints.Add(dataPoint);
			}
			dataSeries.IsNotificationEnable = false;
			this.Chart.InternalSeries.Add(dataSeries);
		}

		private void ClearPlotAreaChildren()
		{
			if (this.PlottingCanvas != null)
			{
				this.PlottingCanvas.Loaded -= new RoutedEventHandler(this.PlottingCanvas_Loaded);
				this.PlottingCanvas.Children.Clear();
				this.PlotAreaCanvas.Children.Remove(this.PlottingCanvas);
				this.PlottingCanvas = null;
			}
		}

		private void DrawHorizontalPlank(double plankDepth, double plankThickness, double position, AxisRepresentations axisChanged, bool isPartialUpdate)
		{
			if (isPartialUpdate && this._horizontalPlank != null && axisChanged == AxisRepresentations.AxisY)
			{
				ColumnChart.Update3DPlank(this.ScrollableLength - plankDepth, plankThickness, plankDepth, this._horizontalPlank);
				this.PlottingCanvas.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				this._horizontalPlank.Visual.SetValue(Canvas.TopProperty, this.PlottingCanvas.DesiredSize.Height - plankThickness);
				return;
			}
			if (this._horizontalPlank != null && this._horizontalPlank.Visual != null && this._horizontalPlank.Visual.Parent != null)
			{
				Panel panel = this._horizontalPlank.Visual.Parent as Canvas;
				panel.Children.Remove(this._horizontalPlank.Visual);
			}
			Brush frontBrush;
			Brush topBrush;
			Brush rightBrush;
			ExtendedGraphics.GetBrushesForPlank(this.Chart, out frontBrush, out topBrush, out rightBrush, false);
			this._horizontalPlank = ColumnChart.Get3DPlank(this.ScrollableLength - plankDepth, plankThickness, plankDepth, frontBrush, topBrush, rightBrush);
			Panel panel2 = this._horizontalPlank.Visual as Panel;
			this.PlottingCanvas.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			panel2.SetValue(Canvas.TopProperty, this.PlottingCanvas.DesiredSize.Height - plankThickness);
			panel2.SetValue(Panel.ZIndexProperty, -1);
			panel2.Opacity = 0.9;
			this.PlottingCanvas.Children.Add(panel2);
		}

		private void DrawVerticalPlank(double plankDepth, double plankThickness, AxisRepresentations axisChanged, bool isPartialUpdate)
		{
			if (isPartialUpdate && axisChanged == AxisRepresentations.AxisX)
			{
				ColumnChart.Update3DPlank(plankThickness, this.ScrollableLength - plankDepth, plankDepth, this._verticalPlank);
				return;
			}
			if (this._verticalPlank != null && this._verticalPlank.Visual != null && this._verticalPlank.Visual.Parent != null)
			{
				Panel panel = this._verticalPlank.Visual.Parent as Canvas;
				panel.Children.Remove(this._verticalPlank.Visual);
			}
			new RectangularChartShapeParams();
			List<Color> list = new List<Color>();
			list.Add(Color.FromArgb(255, 134, 134, 134));
			list.Add(Color.FromArgb(255, 210, 210, 210));
			list.Add(Color.FromArgb(255, 255, 255, 255));
			list.Add(Color.FromArgb(255, 223, 223, 223));
			Brush frontBrush = Graphics.CreateLinearGradientBrush(0.0, new Point(1.1, 0.49), new Point(-0.15, 0.49), list, new List<double>
			{
				0.0,
				0.844,
				1.0,
				0.442
			});
			list = new List<Color>();
			list.Add(Color.FromArgb(255, 232, 232, 232));
			list.Add(Color.FromArgb(255, 142, 135, 135));
			Brush rightBrush = Graphics.CreateLinearGradientBrush(0.0, new Point(0.0, 0.5), new Point(1.0, 0.5), list, new List<double>
			{
				1.0,
				0.0
			});
			list = new List<Color>();
			list.Add(Color.FromArgb(255, 232, 232, 232));
			list.Add(Color.FromArgb(255, 142, 135, 135));
			Brush topBrush = Graphics.CreateLinearGradientBrush(0.0, new Point(0.084, 0.441), new Point(1.916, 0.443), list, new List<double>
			{
				0.0,
				1.0
			});
			this._verticalPlank = ColumnChart.Get3DPlank(plankThickness, this.ScrollableLength - plankDepth, plankDepth, frontBrush, topBrush, rightBrush);
			Panel panel2 = this._verticalPlank.Visual as Panel;
			panel2.SetValue(Canvas.TopProperty, plankDepth);
			panel2.SetValue(Panel.ZIndexProperty, -1);
			this.PlottingCanvas.Children.Add(panel2);
		}

		private void DrawVerticalPlank(double height, double plankDepth, double plankThickness, double plankOpacity, bool isPartialUpdate)
		{
			if (this.Chart == null || this.Chart.PlotArea == null)
			{
				return;
			}
			if (isPartialUpdate && this.Chart.PlotArea.SideFace != null)
			{
				ColumnChart.Update3DPlank(plankThickness, height, plankDepth, this.Chart.PlotArea.SideFace);
				this.CreateGridLinesOverPlank(height, this.Chart.PlotArea.SideFace.Visual as Panel, plankDepth, plankThickness);
				return;
			}
			if (this.Chart.PlotArea.SideFace != null && this.Chart.PlotArea.SideFace.Visual != null && this.Chart.PlotArea.SideFace.Visual.Parent != null)
			{
				Panel panel = this.Chart.PlotArea.SideFace.Visual.Parent as Canvas;
				panel.Children.Remove(this.Chart.PlotArea.SideFace.Visual);
			}
			List<Color> list = new List<Color>();
			list.Add(Color.FromArgb(255, 240, 240, 240));
			list.Add(Color.FromArgb(255, 200, 200, 200));
			Brush rightBrush = (this.Chart.PlotArea.SideFaceBackground == null) ? Graphics.CreateLinearGradientBrush(0.0, new Point(0.0, 0.5), new Point(1.0, 0.5), list, new List<double>
			{
				0.0,
				1.0
			}) : this.Chart.PlotArea.SideFaceBackground;
			this.Chart.PlotArea.SideFace = ColumnChart.Get3DPlank(plankThickness, height, plankDepth, null, null, rightBrush);
			Panel panel2 = this.Chart.PlotArea.SideFace.Visual as Panel;
			this.CreateGridLinesOverPlank(height, panel2, plankDepth, plankThickness);
			panel2.SetValue(Canvas.TopProperty, plankDepth);
			panel2.SetValue(Panel.ZIndexProperty, -1);
			panel2.SetValue(UIElement.OpacityProperty, 0.8);
			this.PlottingCanvas.Children.Add(panel2);
		}

		private void CreateGridLinesOverPlank(double height, Panel plank, double plankDepth, double plankThickness)
		{
			if (this.GridLineCanvas4VerticalPlank != null && this.GridLineCanvas4VerticalPlank.Parent != null)
			{
				plank.Children.Remove(this.GridLineCanvas4VerticalPlank);
				this.GridLineCanvas4VerticalPlank.Children.Clear();
			}
			if (this.AxisY != null && this.AxisY.Grids.Count > 0)
			{
				this.GridLineCanvas4VerticalPlank = new Canvas();
				this.GridLineCanvas4VerticalPlank.Width = plank.Width;
				this.GridLineCanvas4VerticalPlank.Height = plank.Height;
				plank.Children.Add(this.GridLineCanvas4VerticalPlank);
				if (this.AxisY.Grids[0].Enabled.Value)
				{
					decimal num = (decimal)this.AxisY.Grids[0].Minimum;
					decimal d = (decimal)this.AxisY.Grids[0].Maximum;
					double num2 = this.AxisY.Grids[0].Interval.Value;
					decimal d2 = 0m;
					double[] array = new double[2];
					double[] array2 = new double[2];
					decimal d3 = (decimal)num2;
					int num3 = 0;
					this.InterlacedLinesOverVerticalPlank = new List<Line>();
					this.InterlacedPathsOverVerticalPlank = new List<Path>();
					this.Storyboard4PlankGridLines = new Storyboard();
					if (num != d)
					{
						decimal num4 = num;
						while (num4 <= d)
						{
							Line line = new Line();
							Brush lineColor = this.AxisY.Grids[0].LineColor;
							line.Stroke = lineColor;
							line.StrokeThickness = this.AxisY.Grids[0].LineThickness.Value;
							if (this.AxisY.Grids[0].LineStyle != LineStyles.Solid)
							{
								line.StrokeDashArray = ExtendedGraphics.GetDashArray(this.AxisY.Grids[0].LineStyle);
							}
							double num5 = Graphics.ValueToPixelPosition(height, 0.0, this.AxisY.Grids[0].Minimum, this.AxisY.Grids[0].Maximum, (double)num4);
							if (num5 == 0.0)
							{
								num5 += this.AxisY.Grids[0].LineThickness.Value;
							}
							line.X1 = plankDepth;
							line.X2 = 0.0;
							line.Y1 = num5 - plankDepth;
							line.Y2 = num5 - plankThickness;
							array = new double[]
							{
								num5 - plankDepth,
								num5 - plankThickness
							};
							if (this.Chart._internalAnimationEnabled && this.AxisY.Grids[0].AnimationEnabled)
							{
								line.X2 = line.X1;
								line.Y2 = line.Y1;
								this.Storyboard4PlankGridLines.Children.Add(this.AxisY.Grids[0].CreateDoubleAnimation(line, "X2", plankDepth, 0.0, 0.75, 0.75));
								this.Storyboard4PlankGridLines.Children.Add(this.AxisY.Grids[0].CreateDoubleAnimation(line, "Y2", num5 - plankDepth, num5 - plankThickness, 0.75, 0.75));
							}
							this.GridLineCanvas4VerticalPlank.Children.Add(line);
							this.InterlacedLinesOverVerticalPlank.Add(line);
							if (d2 % 2m == 1m)
							{
								Path path = new Path();
								path.StrokeThickness = 0.0;
								if (this.Chart._internalAnimationEnabled && this.AxisY.Grids[0].AnimationEnabled)
								{
									path.Opacity = 0.0;
									this.Storyboard4PlankGridLines.Children.Add(this.AxisY.Grids[0].CreateDoubleAnimation(path, "Opacity", 0.0, 1.0, 1.0, 0.75));
								}
								path.Data = this.GetPathGeometry(new PointCollection
								{
									new Point(plankDepth, array2[0]),
									new Point(0.0, array2[1]),
									new Point(0.0, array[1]),
									new Point(plankDepth, array[0]),
									new Point(plankDepth, array2[0])
								});
								Brush interlacedColor = this.AxisY.Grids[0].InterlacedColor;
								num3++;
								path.Fill = interlacedColor;
								path.SetValue(Panel.ZIndexProperty, 10);
								this.GridLineCanvas4VerticalPlank.Children.Add(path);
								this.InterlacedPathsOverVerticalPlank.Add(path);
							}
							d2 += this.AxisY.SkipOffset + 1;
							num4 = num + d2 * d3;
							array2 = new double[]
							{
								num5 - plankDepth,
								num5 - plankThickness
							};
						}
					}
				}
			}
		}

		private Geometry GetPathGeometry(PointCollection collection)
		{
			PathGeometry pathGeometry = new PathGeometry();
			PathFigure pathFigure = new PathFigure();
			foreach (Point current in collection)
			{
				LineSegment lineSegment = new LineSegment();
				lineSegment.Point = current;
				pathFigure.Segments.Add(lineSegment);
			}
			pathGeometry.Figures.Add(pathFigure);
			return pathGeometry;
		}

		private void AddTrendLines(Axis axis, List<TrendLine> trendLinesReferingToAAxes, Canvas trendLineCanvas)
		{
			if (axis != null)
			{
				foreach (TrendLine current in trendLinesReferingToAAxes)
				{
					current.ReferingAxis = axis;
					if (current.Visual == null)
					{
						current.CreateVisualObject(trendLineCanvas.Width, trendLineCanvas.Height);
						if (current.Visual != null)
						{
							trendLineCanvas.Children.Add(current.Visual);
							if (current.LabelVisual != null)
							{
								this.Chart._elementCanvas.Children.Add(current.LabelVisual);
							}
						}
					}
					else
					{
						current.CreateVisualObject(trendLineCanvas.Width, trendLineCanvas.Height);
						if (current.LabelVisual != null)
						{
							this.Chart._elementCanvas.Children.Add(current.LabelVisual);
						}
					}
					if (current.Visual != null)
					{
						PlotArea plotArea = this.Chart.PlotArea;
						RectangleGeometry rectangleGeometry = new RectangleGeometry();
						rectangleGeometry.Rect = new Rect(plotArea.BorderThickness.Left, plotArea.BorderThickness.Top, trendLineCanvas.Width - plotArea.BorderThickness.Left - plotArea.BorderThickness.Right, trendLineCanvas.Height - plotArea.BorderThickness.Top - plotArea.BorderThickness.Bottom);
						current.Visual.Clip = rectangleGeometry;
						if (current.LabelVisual != null && current.LabelTextBlock != null)
						{
							Graphics.CalculateVisualSize(current.LabelTextBlock);
							rectangleGeometry = new RectangleGeometry();
							Point plotAreaStartPosition = this.Chart.PlotArea.GetPlotAreaStartPosition();
							if (this.Chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
							{
								if (current.Orientation == Orientation.Horizontal)
								{
									rectangleGeometry.Rect = new Rect(0.0, plotAreaStartPosition.Y, this.Chart.ActualWidth, this.PlotAreaCanvas.Height);
								}
								else
								{
									rectangleGeometry.Rect = new Rect(plotAreaStartPosition.X, 0.0, this.PlotAreaCanvas.Width, this.Chart.ActualHeight);
								}
							}
							else if (current.Orientation == Orientation.Horizontal)
							{
								rectangleGeometry.Rect = new Rect(0.0, plotAreaStartPosition.Y, this.Chart.ActualWidth, this.PlotAreaCanvas.Height);
							}
							else
							{
								rectangleGeometry.Rect = new Rect(plotAreaStartPosition.X, 0.0, this.PlotAreaCanvas.Width, this.Chart.ActualHeight);
							}
							current.LabelVisual.Clip = rectangleGeometry;
						}
						int num = (int)current.GetValue(Panel.ZIndexProperty);
						if (num == 0)
						{
							current.Visual.SetValue(Panel.ZIndexProperty, -999);
						}
						else
						{
							current.Visual.SetValue(Panel.ZIndexProperty, num);
						}
					}
				}
			}
		}

		private void CleanUpTrendLines(Canvas trendLineCanvas)
		{
			foreach (TrendLine current in this.Chart.TrendLines)
			{
				if (current.Visual != null)
				{
					trendLineCanvas.Children.Remove(current.Visual);
					current.Visual.Children.Clear();
					current.Visual = null;
				}
			}
		}

		private void RenderTrendLines()
		{
			if (this.Chart._forcedRedraw)
			{
				this.CleanUpTrendLines(this.ChartVisualCanvas);
			}
			List<TrendLine> trendLinesReferingToAAxes;
			List<TrendLine> trendLinesReferingToAAxes2;
			List<TrendLine> trendLinesReferingToAAxes3;
			List<TrendLine> trendLinesReferingToAAxes4;
			if (this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				trendLinesReferingToAAxes = (from trendline in this.Chart.TrendLines
				where trendline.Orientation == Orientation.Vertical && trendline.AxisType == AxisTypes.Primary
				select trendline).ToList<TrendLine>();
				trendLinesReferingToAAxes2 = (from trendline in this.Chart.TrendLines
				where trendline.Orientation == Orientation.Horizontal && trendline.AxisType == AxisTypes.Primary
				select trendline).ToList<TrendLine>();
				trendLinesReferingToAAxes3 = (from trendline in this.Chart.TrendLines
				where trendline.Orientation == Orientation.Vertical && trendline.AxisType == AxisTypes.Secondary
				select trendline).ToList<TrendLine>();
				trendLinesReferingToAAxes4 = (from trendline in this.Chart.TrendLines
				where trendline.Orientation == Orientation.Horizontal && trendline.AxisType == AxisTypes.Secondary
				select trendline).ToList<TrendLine>();
			}
			else
			{
				trendLinesReferingToAAxes = (from trendline in this.Chart.TrendLines
				where trendline.Orientation == Orientation.Horizontal && trendline.AxisType == AxisTypes.Primary
				select trendline).ToList<TrendLine>();
				trendLinesReferingToAAxes2 = (from trendline in this.Chart.TrendLines
				where trendline.Orientation == Orientation.Vertical && trendline.AxisType == AxisTypes.Primary
				select trendline).ToList<TrendLine>();
				trendLinesReferingToAAxes3 = (from trendline in this.Chart.TrendLines
				where trendline.Orientation == Orientation.Horizontal && trendline.AxisType == AxisTypes.Secondary
				select trendline).ToList<TrendLine>();
				trendLinesReferingToAAxes4 = (from trendline in this.Chart.TrendLines
				where trendline.Orientation == Orientation.Vertical && trendline.AxisType == AxisTypes.Secondary
				select trendline).ToList<TrendLine>();
			}
			this.AddTrendLines(this.AxisX, trendLinesReferingToAAxes, this.ChartVisualCanvas);
			this.AddTrendLines(this.AxisY, trendLinesReferingToAAxes2, this.ChartVisualCanvas);
			this.AddTrendLines(this.AxisX2, trendLinesReferingToAAxes3, this.ChartVisualCanvas);
			this.AddTrendLines(this.AxisY2, trendLinesReferingToAAxes4, this.ChartVisualCanvas);
		}

		private void AddGrids(Axis axis, double width, double height, bool isAnimationEnabled, string styleName)
		{
			if (this.Chart._forcedRedraw)
			{
				this.CleanUpGrids(axis);
			}
			foreach (ChartGrid current in axis.Grids)
			{
				current.IsNotificationEnable = false;
				current.Chart = this.Chart;
				if (current.Visual == null)
				{
					current.CreateVisualObject(width, height, isAnimationEnabled, ChartArea.GRID_ANIMATION_DURATION);
					if (current.Visual != null)
					{
						this.ChartVisualCanvas.Children.Add(current.Visual);
					}
				}
				else
				{
					current.CreateVisualObject(width, height, isAnimationEnabled, ChartArea.GRID_ANIMATION_DURATION);
				}
				if (current.Visual != null)
				{
					current.Visual.SetValue(Panel.ZIndexProperty, -1000);
				}
				current.IsNotificationEnable = true;
			}
		}

		private void CleanUpGrids(Axis axis)
		{
			foreach (ChartGrid current in axis.Grids)
			{
				if (current.Visual != null)
				{
					this.ChartVisualCanvas.Children.Remove(current.Visual);
					current.Visual.Children.Clear();
					current.CachedGridLines.Clear();
					current.CachedGridRectangles.Clear();
					current.Visual = null;
				}
			}
		}

		private void RenderGrids()
		{
			bool isAnimationEnabled = this.Chart._internalAnimationEnabled && !this._isAnimationFired;
			if (this.AxisX != null)
			{
				this.AddGrids(this.AxisX, this.ChartVisualCanvas.Width, this.ChartVisualCanvas.Height, isAnimationEnabled, this.AxisX.AxisRepresentation.ToString() + "Grid");
			}
			if (this.AxisX2 != null)
			{
				this.AddGrids(this.AxisX2, this.ChartVisualCanvas.Width, this.ChartVisualCanvas.Height, isAnimationEnabled, this.AxisX2.AxisRepresentation.ToString() + "Grid");
			}
			if (this.AxisY != null)
			{
				this.AddGrids(this.AxisY, this.ChartVisualCanvas.Width, this.ChartVisualCanvas.Height, isAnimationEnabled, this.AxisY.AxisRepresentation.ToString() + "Grid");
			}
			if (this.AxisY2 != null)
			{
				this.AddGrids(this.AxisY2, this.ChartVisualCanvas.Width, this.ChartVisualCanvas.Height, isAnimationEnabled, this.AxisY2.AxisRepresentation.ToString() + "Grid");
			}
		}

		internal void ResizePanels(Size remainingSizeAfterDrawingAxes, AxisRepresentations renderAxisType, bool isPartialUpdate)
		{
			this.PlotAreaScrollViewer = this.Chart._plotAreaScrollViewer;
			if (this._isFirstTimeRender)
			{
				this.PlotAreaScrollViewer.Background = Graphics.TRANSPARENT_BRUSH;
			}
			this.PlotAreaCanvas.Width = remainingSizeAfterDrawingAxes.Width;
			this.PlotAreaCanvas.Height = remainingSizeAfterDrawingAxes.Height;
			if (this.Chart._forcedRedraw || this.PlottingCanvas == null)
			{
				if (this.PlottingCanvas != null)
				{
					this.PlottingCanvas.Children.Clear();
					this.PlottingCanvas.Loaded -= new RoutedEventHandler(this.PlottingCanvas_Loaded);
				}
				this.PlottingCanvas = new Canvas();
				this.PlottingCanvas.Loaded += new RoutedEventHandler(this.PlottingCanvas_Loaded);
				this.PlottingCanvas.SetValue(Panel.ZIndexProperty, 1);
				this.PlotAreaCanvas.Children.Add(this.PlottingCanvas);
			}
			if (double.IsNaN(remainingSizeAfterDrawingAxes.Height) || remainingSizeAfterDrawingAxes.Height <= 0.0 || double.IsNaN(remainingSizeAfterDrawingAxes.Width) || remainingSizeAfterDrawingAxes.Width <= 0.0)
			{
				return;
			}
			if (this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				this.PlottingCanvas.Width = this.ScrollableLength + this.PLANK_DEPTH;
				this.PlottingCanvas.Height = remainingSizeAfterDrawingAxes.Height;
			}
			else if (this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				this.PlottingCanvas.Width = remainingSizeAfterDrawingAxes.Width;
				this.PlottingCanvas.Height = this.ScrollableLength + this.PLANK_DEPTH;
			}
			if (this.Chart._forcedRedraw || this.ChartVisualCanvas == null)
			{
				if (this.ChartVisualCanvas != null)
				{
					this.ChartVisualCanvas.Children.Clear();
				}
				this.ChartVisualCanvas = new Canvas();
				this.PlottingCanvas.Children.Add(this.ChartVisualCanvas);
			}
			Size size = new Size(0.0, 0.0);
			switch (this.PlotDetails.ChartOrientation)
			{
			case ChartOrientationType.Vertical:
				size = this.CreateRegionsForVerticalCharts(this.ScrollableLength, remainingSizeAfterDrawingAxes, renderAxisType, isPartialUpdate);
				this.ChartVisualCanvas.SetValue(Canvas.LeftProperty, this.PLANK_DEPTH);
				this.Chart.PlotArea.BorderElement.SetValue(Canvas.LeftProperty, this.PLANK_DEPTH);
				break;
			case ChartOrientationType.Horizontal:
				size = this.CreateRegionsForHorizontalCharts(this.ScrollableLength, remainingSizeAfterDrawingAxes, renderAxisType, isPartialUpdate);
				this.ChartVisualCanvas.SetValue(Canvas.LeftProperty, this.PLANK_OFFSET);
				this.Chart.PlotArea.BorderElement.SetValue(Canvas.LeftProperty, this.PLANK_OFFSET);
				break;
			case ChartOrientationType.NoAxis:
			case ChartOrientationType.Circular:
				size = this.CreateRegionsForChartsWithoutAxis(remainingSizeAfterDrawingAxes);
				break;
			}
			if (size.Width <= 0.0 || size.Height <= 0.0)
			{
				return;
			}
			this.ChartVisualCanvas.Width = Math.Max(size.Width - ((this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal) ? ChartArea.SCROLLVIEWER_OFFSET4HORIZONTAL_CHART : 0.0), 0.0);
			this.ChartVisualCanvas.Height = Math.Max(size.Height - ((this.PlotDetails.ChartOrientation == ChartOrientationType.NoAxis) ? 5.0 : 0.0), 0.0);
			this.Chart.PlotArea.BorderElement.Height = this.ChartVisualCanvas.Height;
			this.Chart.PlotArea.BorderElement.Width = size.Width;
			this.Chart.PlotArea.ApplyBevel(this.PLANK_DEPTH, this.PLANK_THICKNESS);
			this.Chart.PlotArea.ApplyShadow(remainingSizeAfterDrawingAxes, this.PLANK_OFFSET, this.PLANK_DEPTH, this.PLANK_THICKNESS);
			this.Chart._plotCanvas.Width = this.PlottingCanvas.Width;
			this.Chart._plotCanvas.Height = this.PlottingCanvas.Height;
		}

		private void RenderChart(Size remainingSizeAfterDrawingAxes, AxisRepresentations renderAxisType, bool isPartialUpdate)
		{
			if (this.Chart._forcedRedraw || this.PlotDetails.ChartOrientation == ChartOrientationType.NoAxis || this.PlotDetails.ChartOrientation == ChartOrientationType.Circular)
			{
				this.Chart._forcedRedraw = true;
				this.ClearPlotAreaChildren();
			}
			this.ResizePanels(remainingSizeAfterDrawingAxes, renderAxisType, isPartialUpdate);
			if (double.IsNaN(remainingSizeAfterDrawingAxes.Height) || double.IsNaN(remainingSizeAfterDrawingAxes.Width) || remainingSizeAfterDrawingAxes.Height == 0.0 || remainingSizeAfterDrawingAxes.Width == 0.0)
			{
				return;
			}
			if (this.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis && this.PlotDetails.ChartOrientation != ChartOrientationType.Circular)
			{
				this.RenderGrids();
				this.RenderTrendLines();
			}
			this.RenderAxes4CircularChart();
			this.RenderSeries();
			this._renderCount++;
		}

		private void RenderAxes4CircularChart()
		{
			if (this.Chart.PlotDetails.ChartOrientation == ChartOrientationType.Circular && this.Chart.InternalSeries.Count > 0)
			{
				DataSeries dataSeries = this.Chart.InternalSeries[0];
				CircularPlotDetails circularPlotDetails = new CircularPlotDetails(this.Chart.PlotDetails.ChartOrientation, dataSeries.RenderAs);
				Canvas canvas = new Canvas();
				this.CreateAndPositionAxes4CircularChart(canvas, this.ChartVisualCanvas.Width, this.ChartVisualCanvas.Height, this.Chart.PlotDetails, circularPlotDetails);
				this.ChartVisualCanvas.Children.Add(canvas);
			}
		}

		private void CreateAndPositionAxes4CircularChart(Canvas visual, double width, double height, PlotDetails plotDetails, CircularPlotDetails circularPlotDetails)
		{
			if (this.AxisX != null)
			{
				this.CreateAxis4CircularChart(this.AxisX, width, height, plotDetails, circularPlotDetails);
				if (this.AxisX.CircularAxisVisual != null)
				{
					visual.Children.Add(this.AxisX.CircularAxisVisual);
				}
			}
			if (this.AxisY != null)
			{
				this.CreateAxis4CircularChart(this.AxisY, width, height, plotDetails, circularPlotDetails);
				if (this.AxisY.CircularAxisVisual != null)
				{
					visual.Children.Add(this.AxisY.CircularAxisVisual);
				}
			}
		}

		private Canvas CreateAxis4CircularChart(Axis axis, double width, double height, PlotDetails plotDetails, CircularPlotDetails circularPlotDetails)
		{
			Chart chart = plotDetails.Chart;
			if (axis != null && plotDetails.ChartOrientation == ChartOrientationType.Circular)
			{
				axis.Width = width;
				axis.Height = height;
				axis.CircularPlotDetails = circularPlotDetails;
				axis.CreateVisualObject(chart);
				return axis.CircularAxisVisual;
			}
			return null;
		}

		internal void SaveAxisContentOffsetAndResetMargin(Axis axis, double scrollBarOffset)
		{
			if (axis.ScrollBarElement.Maximum != 0.0)
			{
				axis.CurrentScrollScrollBarOffset = scrollBarOffset / axis.ScrollBarElement.Maximum;
			}
		}

		internal void AxesXScrollBarElement_Scroll(object sender, ScrollEventArgs e)
		{
			Chart chart = this.Chart;
			ZoomBar zoomBar = sender as ZoomBar;
			if (!this._isFirstTimeRender && this.PlotDetails.IsOverrideViewPortRangeEnabled && zoomBar.IsDragging)
			{
				if (chart.ZoomingEnabled)
				{
					if (e.ScrollEventType != ScrollEventType.EndScroll)
					{
						return;
					}
				}
				else if (e.ScrollEventType != ScrollEventType.EndScroll && e.ScrollEventType != ScrollEventType.SmallIncrement && e.ScrollEventType != ScrollEventType.SmallDecrement && e.ScrollEventType != ScrollEventType.LargeIncrement && e.ScrollEventType != ScrollEventType.LargeDecrement)
				{
					return;
				}
			}
			if (this.ChartVisualCanvas == null || (!chart.ScrollingEnabled.Value && !chart.ZoomingEnabled))
			{
				return;
			}
			if (this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				double num = e.NewValue;
				this.AxisX.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
				if (this.Chart.ZoomingEnabled)
				{
					this.AxisX.ScrollBarElement.Maximum = 100.0;
				}
				else
				{
					this.AxisX.ScrollBarElement.Maximum = this.ScrollableLength - this.PlotAreaScrollViewer.ActualWidth;
					this.AxisX.ScrollBarElement.ViewportSize = this.PlotAreaScrollViewer.ActualWidth;
					if (e.NewValue <= 1.0)
					{
						num = e.NewValue * this.AxisX.ScrollBarElement.Maximum;
					}
				}
				double offsetInPixel = num;
				if (this.AxisX.ScrollViewerElement.Children.Count > 0)
				{
					FrameworkElement frameworkElement = this.AxisX.ScrollViewerElement.Children[0] as FrameworkElement;
					double scrollingOffsetOfAxis = this.GetScrollingOffsetOfAxis(this.AxisX, num);
					if (!double.IsNaN(scrollingOffsetOfAxis))
					{
						this.Chart.PlotArea.ScrollToHorizontalOffset(scrollingOffsetOfAxis);
						frameworkElement.SetValue(Canvas.LeftProperty, -1.0 * scrollingOffsetOfAxis);
					}
				}
				this.SaveAxisContentOffsetAndResetMargin(this.AxisX, num);
				this.AxisX._isScrollToOffsetEnabled = false;
				num /= this.AxisX.ScrollBarElement.Maximum - this.AxisX.ScrollBarElement.Minimum;
				if (!double.IsNaN(num))
				{
					this.AxisX.ScrollBarOffset = ((num > 1.0) ? 1.0 : ((num < 0.0) ? 0.0 : num));
				}
				this.AxisX._isScrollToOffsetEnabled = true;
				this.AxisX.FireScrollEvent(e, offsetInPixel);
				foreach (TrendLine current in this.Chart.TrendLines)
				{
					current.UpdateTrendLineLabelPosition(this.ChartVisualCanvas.Width, this.ChartVisualCanvas.Height);
				}
			}
			if (this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				double num2 = e.NewValue;
				this.AxisX.ScrollBarElement.IsStretchable = chart.ZoomingEnabled;
				if (this.Chart.ZoomingEnabled)
				{
					this.AxisX.ScrollBarElement.Maximum = 100.0;
				}
				else
				{
					this.AxisX.ScrollBarElement.Maximum = this.ScrollableLength - this.PlotAreaScrollViewer.ActualHeight;
					this.AxisX.ScrollBarElement.ViewportSize = this.PlotAreaScrollViewer.ActualHeight;
				}
				if (e.NewValue <= 1.0)
				{
					num2 = e.NewValue * this.AxisX.ScrollBarElement.Maximum;
				}
				double offsetInPixel2 = num2;
				if (this.AxisX.ScrollViewerElement.Children.Count > 0)
				{
					FrameworkElement frameworkElement2 = this.AxisX.ScrollViewerElement.Children[0] as FrameworkElement;
					if (frameworkElement2 != null)
					{
						double scrollingOffsetOfAxis2 = this.GetScrollingOffsetOfAxis(this.AxisX, num2);
						if (!chart.ZoomingEnabled)
						{
							offsetInPixel2 = scrollingOffsetOfAxis2;
						}
						if (!double.IsNaN(scrollingOffsetOfAxis2))
						{
							this.Chart.PlotArea.ScrollToVerticalOffset(scrollingOffsetOfAxis2);
							frameworkElement2.SetValue(Canvas.TopProperty, -1.0 * scrollingOffsetOfAxis2);
						}
					}
				}
				this.SaveAxisContentOffsetAndResetMargin(this.AxisX, this.AxisX.ScrollBarElement.Maximum - num2);
				this.AxisX._isScrollToOffsetEnabled = false;
				num2 = (this.AxisX.ScrollBarElement.Maximum - num2) / (this.AxisX.ScrollBarElement.Maximum - this.AxisX.ScrollBarElement.Minimum);
				if (!double.IsNaN(num2))
				{
					this.AxisX.ScrollBarOffset = ((num2 > 1.0) ? 1.0 : ((num2 < 0.0) ? 0.0 : num2));
				}
				this.AxisX._isScrollToOffsetEnabled = true;
				this.AxisX.FireScrollEvent(e, offsetInPixel2);
				foreach (TrendLine current2 in this.Chart.TrendLines)
				{
					current2.UpdateTrendLineLabelPosition(this.ChartVisualCanvas.Width, this.ChartVisualCanvas.Height);
				}
			}
			if (this.AxisX2 != null)
			{
				this.AxisX2.ScrollBarElement.Value = e.NewValue;
			}
		}

		internal void AxesX2ScrollBarElement_Scroll(object sender, ScrollEventArgs e)
		{
			Chart chart = this.Chart;
			if (this.ChartVisualCanvas == null || (!chart.ScrollingEnabled.Value && !chart.ZoomingEnabled))
			{
				return;
			}
			if (this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				double num = e.NewValue;
				this.AxisX2.ScrollBarElement.Maximum = this.ScrollableLength - this.PlotAreaScrollViewer.ActualWidth;
				this.AxisX2.ScrollBarElement.ViewportSize = this.PlotAreaScrollViewer.ActualWidth;
				if (e.NewValue <= 1.0)
				{
					num = e.NewValue * this.AxisX2.ScrollBarElement.Maximum;
				}
				double offsetInPixel = num;
				if (this.AxisX2.ScrollViewerElement.Children.Count > 0)
				{
					FrameworkElement frameworkElement = this.AxisX2.ScrollViewerElement.Children[0] as FrameworkElement;
					double offset = this.Chart.ZoomingEnabled ? (num * (frameworkElement.Width / 100.0)) : num;
					this.Chart.PlotArea.ScrollToHorizontalOffset(offset);
					frameworkElement.Margin = new Thickness(num, 0.0, 0.0, 0.0);
				}
				this.SaveAxisContentOffsetAndResetMargin(this.AxisX2, num);
				this.AxisX2.FireScrollEvent(e, offsetInPixel);
			}
			if (this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				double num2 = e.NewValue;
				this.AxisX2.ScrollBarElement.Maximum = this.ScrollableLength - this.PlotAreaScrollViewer.ActualHeight;
				this.AxisX2.ScrollBarElement.ViewportSize = this.PlotAreaScrollViewer.ActualHeight;
				if (e.NewValue <= 1.0)
				{
					num2 = e.NewValue * this.AxisX2.ScrollBarElement.Maximum;
				}
				double offsetInPixel2 = num2;
				if (this.AxisX2.ScrollViewerElement.Children.Count > 0)
				{
					FrameworkElement frameworkElement2 = this.AxisX2.ScrollViewerElement.Children[0] as FrameworkElement;
					double num3 = this.Chart.ZoomingEnabled ? (num2 * (frameworkElement2.Height / 100.0)) : num2;
					this.Chart.PlotArea.ScrollToVerticalOffset(num3);
					frameworkElement2.Margin = new Thickness(0.0, num3, 0.0, 0.0);
				}
				this.SaveAxisContentOffsetAndResetMargin(this.AxisX2, num2);
				this.AxisX2.FireScrollEvent(e, offsetInPixel2);
			}
			if (this.AxisX != null)
			{
				this.AxisX.ScrollBarElement.Value = e.NewValue;
			}
		}

		internal void RenderSeries()
		{
			int i = 0;
			List<DataSeries> list = this.PlotDetails.SeriesDrawingIndex.Keys.ToList<DataSeries>();
			Panel panel = null;
			int num = 1;
			while (i < this.Chart.InternalSeries.Count)
			{
				List<DataSeries> list2 = new List<DataSeries>();
				RenderAs renderAs = list[i].RenderAs;
				int num2 = this.PlotDetails.SeriesDrawingIndex[list[i]];
				for (int j = i; j < this.Chart.InternalSeries.Count; j++)
				{
					if (renderAs == list[j].RenderAs && num2 == this.PlotDetails.SeriesDrawingIndex[list[j]])
					{
						list2.Add(list[j]);
					}
				}
				if (list2.Count != 0)
				{
					bool flag = false;
					panel = list2[0].Visual;
					if (panel == null && list2[0].RenderAs != RenderAs.Area)
					{
						using (List<DataSeries>.Enumerator enumerator = list2.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								DataSeries current = enumerator.Current;
								if (current.Visual != null)
								{
									panel = current.Visual;
									flag = true;
									break;
								}
							}
							goto IL_126;
						}
						goto IL_11A;
					}
					goto IL_11A;
					IL_126:
					if (this.Chart._forcedRedraw || list2[0].RenderAs == RenderAs.StackedArea || list2[0].RenderAs == RenderAs.StackedArea100)
					{
						if (panel != null && panel.Parent != null)
						{
							Panel panel2 = panel.Parent as Panel;
							panel2.Children.Remove(panel);
						}
						panel = null;
						flag = false;
					}
					panel = this.RenderSeriesFromList(panel, list2);
					foreach (DataSeries current2 in list2)
					{
						current2.Visual = panel;
					}
					if (panel != null && !flag)
					{
						this.ChartVisualCanvas.Children.Add(panel);
					}
					i += list2.Count;
					if (panel != null)
					{
						panel.SetValue(Panel.ZIndexProperty, num++);
						continue;
					}
					continue;
					IL_11A:
					flag = (panel != null);
					goto IL_126;
				}
				break;
			}
			this.ApplyOpacity();
			this.AttachEventsToolTipHref2DataSeries();
		}

		internal Panel RenderSeriesFromList(Panel preExistingPanel, List<DataSeries> dataSeriesList4Rendering)
		{
			return RenderHelper.GetVisualObject(preExistingPanel, dataSeriesList4Rendering[0].RenderAs, this.ChartVisualCanvas.Width, this.ChartVisualCanvas.Height, this.PlotDetails, dataSeriesList4Rendering, this.Chart, this.PLANK_DEPTH, this.Chart._internalAnimationEnabled && !this._isAnimationFired);
		}

		private void AnimateChartGrid(Axis axis)
		{
			if (axis != null)
			{
				foreach (ChartGrid current in axis.Grids)
				{
					if (current.Storyboard != null)
					{
						current.Storyboard.Begin(this.Chart._rootElement, true);
						current.Storyboard.Completed += delegate(object A_1, EventArgs A_2)
						{
							this._isAnimationFired = true;
						};
					}
				}
				if (axis.AxisRepresentation == AxisRepresentations.AxisY && axis.AxisType == AxisTypes.Primary && this.Storyboard4PlankGridLines != null)
				{
					this.Storyboard4PlankGridLines.Begin(this.Chart._rootElement, true);
					this.Storyboard4PlankGridLines.Completed += delegate(object A_1, EventArgs A_2)
					{
						this._isAnimationFired = true;
					};
				}
			}
		}

		private void AnimateCircularAxis(Axis axis)
		{
			if (axis != null && axis.Storyboard != null)
			{
				axis.Storyboard.Begin(this.Chart._rootElement, true);
				axis.Storyboard.Completed += delegate(object A_1, EventArgs A_2)
				{
					this._isAnimationFired = true;
				};
			}
		}

		private void Animate()
		{
			if (this.Chart._internalAnimationEnabled && !this.Chart.IsInDesignMode)
			{
				try
				{
					if (this.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis)
					{
						if (this.PlotDetails.ChartOrientation == ChartOrientationType.Circular)
						{
							this.AnimateCircularAxis(this.AxisX);
							this.AnimateCircularAxis(this.AxisY);
						}
						else
						{
							this.AnimateChartGrid(this.AxisX);
							this.AnimateChartGrid(this.AxisY);
							this.AnimateChartGrid(this.AxisY2);
						}
					}
					bool flag = false;
					foreach (DataSeries series in this.Chart.InternalSeries)
					{
						if (series.Storyboard != null)
						{
							if (series.InternalDataPoints.Count >= 1)
							{
								flag = true;
							}
							series.Storyboard.Completed += delegate(object sender, EventArgs e)
							{
								this._isAnimationFired = true;
								this.Chart._rootElement.IsHitTestVisible = true;
								if (this.PlotDetails.ChartOrientation == ChartOrientationType.NoAxis)
								{
									foreach (IDataPoint current2 in series.InternalDataPoints)
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
								Chart.SelectDataPoints(this.Chart);
								this.Chart.DisplayLicenseInfoBanner();
							};
							if (this.PlotDetails.ChartOrientation == ChartOrientationType.NoAxis)
							{
								series.Storyboard.Completed += delegate(object sender, EventArgs e)
								{
									series.DetachOpacityPropertyFromAnimation();
									foreach (IDataPoint current2 in series.InternalDataPoints)
									{
										if (current2.Exploded.Value && current2.DpInfo.InternalYValue != 0.0)
										{
											DataPointHelper.InteractiveAnimation(current2, true, false);
										}
									}
								};
							}
							series.Storyboard.Begin(this.Chart._rootElement, true);
						}
					}
					if (!flag)
					{
						this.Chart._rootElement.IsHitTestVisible = true;
						this._isAnimationFired = true;
						this.Chart.DisplayLicenseInfoBanner();
					}
					return;
				}
				catch (Exception)
				{
					return;
				}
			}
			if (this.PlotDetails.ChartOrientation == ChartOrientationType.NoAxis && this.Chart.InternalSeries.Count > 0)
			{
				foreach (IDataPoint current in this.Chart.InternalSeries[0].InternalDataPoints)
				{
					if (current.Parent.RenderAs != RenderAs.SectionFunnel && current.Parent.RenderAs != RenderAs.StreamLineFunnel && current.Parent.RenderAs != RenderAs.Pyramid && current.Exploded.Value && current.DpInfo.InternalYValue != 0.0)
					{
						DataPointHelper.PieExplodeOrUnExplodeWithoutAnimation(current);
					}
				}
			}
			this.ExplodFunnelChartWithOutAnimation();
			if (!this.Chart._internalAnimationEnabled && !this.Chart.IsInDesignMode)
			{
				this.Chart.DisplayLicenseInfoBanner();
			}
		}

		private void ExplodFunnelChartWithOutAnimation()
		{
			if (this.Chart != null && this.Chart.InternalSeries.Count > 0 && !this.Chart._internalAnimationEnabled && (this.Chart.InternalSeries[0].RenderAs == RenderAs.SectionFunnel || this.Chart.InternalSeries[0].RenderAs == RenderAs.StreamLineFunnel || this.Chart.InternalSeries[0].RenderAs == RenderAs.Pyramid))
			{
				if (this.Chart.InternalSeries[0].Exploded)
				{
					if (this.Chart.InternalSeries[0].Storyboard != null)
					{
						this.Chart.InternalSeries[0].Storyboard.Begin(this.Chart._rootElement, true);
						this.Chart.InternalSeries[0].Storyboard.SkipToFill();
						return;
					}
				}
				else
				{
					foreach (IDataPoint current in this.Chart.InternalSeries[0].InternalDataPoints)
					{
						DataPointHelper.ExplodeOrUnexplodeAnimation(current);
					}
				}
			}
		}

		private void SetTrendLineStyleFromTheme()
		{
			if (!string.IsNullOrEmpty(this.Chart.Theme))
			{
				foreach (TrendLine current in this.Chart.TrendLines)
				{
					if (!string.IsNullOrEmpty(this.Chart.Theme))
					{
						current.ApplyStyleFromTheme(this.Chart, "TrendLine");
					}
				}
			}
		}

		private void SetSeriesStyleFromTheme()
		{
			if (this.Chart.Series != null)
			{
				foreach (DataSeries current in this.Chart.Series)
				{
					current.IsNotificationEnable = false;
					current.ApplyStyleFromTheme(this.Chart, "DataSeries");
					current.IsNotificationEnable = true;
				}
			}
		}

		private void SetTitleStyleFromTheme()
		{
			int num = 0;
			if (!string.IsNullOrEmpty(this.Chart.Theme))
			{
				foreach (Title current in this.Chart.Titles)
				{
					if (num == 0)
					{
						current.ApplyStyleFromTheme(this.Chart, "MainTitle");
					}
					else
					{
						current.ApplyStyleFromTheme(this.Chart, "SubTitle");
					}
					num++;
				}
			}
		}

		private void SetLegendStyleFromTheme()
		{
			foreach (Legend current in this.Chart.Legends)
			{
				if (!string.IsNullOrEmpty(this.Chart.Theme))
				{
					current.ApplyStyleFromTheme(this.Chart, "Legend");
				}
			}
		}

		private void SetDataPointColorFromColorSet(IList<DataSeries> series)
		{
			ColorSet colorSet = null;
			if (this._financialColorSet == null)
			{
				this._financialColorSet = this.Chart.GetColorSetByName("CandleLight");
			}
			if (this._circularChartColorSet == null)
			{
				this._circularChartColorSet = this.Chart.GetColorSetByName("SpiderWeb");
			}
			this._financialColorSet.ResetIndex();
			this._circularChartColorSet.ResetIndex();
			if (!string.IsNullOrEmpty(this.Chart.ColorSet))
			{
				colorSet = this.Chart.GetColorSetByName(this.Chart.ColorSet);
				this._chartColorSet = colorSet;
			}
			if (series.Count == 1)
			{
				this.LoadSeriesColorSet4SingleSeries(series[0]);
			}
			else if (series.Count > 1)
			{
				if (this._chartColorSet != null)
				{
					this._chartColorSet.ResetIndex();
				}
				foreach (DataSeries current in series)
				{
					this.LoadSeriesColorSet(current);
				}
			}
			if (colorSet != null)
			{
				colorSet.ResetIndex();
			}
		}

		internal void LoadSeriesColorSet4SingleSeries(DataSeries dataSeries)
		{
			ColorSet colorSet = this._chartColorSet;
			if (dataSeries.RenderAs == RenderAs.CandleStick)
			{
				if (!string.IsNullOrEmpty(dataSeries.ColorSet))
				{
					this._financialColorSet = this.Chart.GetColorSetByName(dataSeries.ColorSet);
					if (this._financialColorSet == null)
					{
						throw new Exception("ColorSet named " + dataSeries.ColorSet + " is not found.");
					}
				}
				if (dataSeries.PriceUpColor == null)
				{
					dataSeries.IsNotificationEnable = false;
					dataSeries.PriceUpColor = this._financialColorSet.GetNewColorFromColorSet();
					dataSeries.IsNotificationEnable = true;
					return;
				}
			}
			else
			{
				if (RenderHelper.IsCircularCType(dataSeries))
				{
					if (!string.IsNullOrEmpty(dataSeries.ColorSet))
					{
						this._circularChartColorSet = this.Chart.GetColorSetByName(dataSeries.ColorSet);
						if (this._circularChartColorSet == null)
						{
							throw new Exception("ColorSet named " + dataSeries.ColorSet + " is not found.");
						}
					}
					Brush brush = dataSeries.GetValue(DataSeries.ColorProperty) as Brush;
					if (brush == null)
					{
						dataSeries._internalColor = this._circularChartColorSet.GetNewColorFromColorSet();
					}
					else
					{
						dataSeries._internalColor = brush;
					}
					using (List<IDataPoint>.Enumerator enumerator = dataSeries.InternalDataPoints.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IDataPoint current = enumerator.Current;
							current.DpInfo.IsNotificationEnable = false;
							Brush color_DataPoint = DataPointHelper.GetColor_DataPoint(current);
							if (color_DataPoint == null)
							{
								if (brush == null)
								{
									current.DpInfo.InternalColor = this._circularChartColorSet.GetNewColorFromColorSet();
								}
								else
								{
									current.DpInfo.InternalColor = brush;
								}
							}
							else
							{
								current.DpInfo.InternalColor = color_DataPoint;
							}
							current.DpInfo.IsNotificationEnable = true;
						}
						return;
					}
				}
				if (!string.IsNullOrEmpty(dataSeries.ColorSet))
				{
					colorSet = this.Chart.GetColorSetByName(dataSeries.ColorSet);
					if (colorSet == null)
					{
						throw new Exception("ColorSet named " + dataSeries.ColorSet + " is not found.");
					}
				}
				else if (colorSet == null)
				{
					throw new Exception("ColorSet named " + this.Chart.ColorSet + " is not found.");
				}
				Brush brush2 = dataSeries.GetValue(DataSeries.ColorProperty) as Brush;
				bool uniqueColors = this.Chart.UniqueColors;
				if (!uniqueColors || RenderHelper.IsAreaCType(dataSeries) || RenderHelper.IsLineCType(dataSeries))
				{
					if (brush2 == null)
					{
						dataSeries._internalColor = colorSet.GetNewColorFromColorSet();
					}
					else
					{
						dataSeries._internalColor = brush2;
					}
					int num = 0;
					bool flag = false;
					List<Brush> list = new List<Brush>();
					colorSet.ResetIndex();
					if (dataSeries.RenderAs == RenderAs.QuickLine)
					{
						return;
					}
					using (List<IDataPoint>.Enumerator enumerator2 = dataSeries.InternalDataPoints.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							IDataPoint current2 = enumerator2.Current;
							current2.DpInfo.IsNotificationEnable = false;
							Brush color_DataPoint2 = DataPointHelper.GetColor_DataPoint(current2);
							if (color_DataPoint2 == null)
							{
								if (!uniqueColors)
								{
									current2.DpInfo.InternalColor = dataSeries._internalColor;
								}
								else if (brush2 == null)
								{
									if (colorSet.Brushes.Count == num)
									{
										flag = true;
										num = 0;
									}
									if (flag)
									{
										current2.DpInfo.InternalColor = list[num];
										if (num == list.Count - 1)
										{
											num = 0;
										}
										else
										{
											num++;
										}
									}
									else
									{
										current2.DpInfo.InternalColor = colorSet.GetNewColorFromColorSet();
										list.Add(current2.DpInfo.InternalColor);
										num++;
									}
								}
								else
								{
									current2.DpInfo.InternalColor = brush2;
								}
							}
							else
							{
								current2.DpInfo.InternalColor = color_DataPoint2;
							}
							current2.DpInfo.IsNotificationEnable = true;
						}
						return;
					}
				}
				if (this.Chart.Series.Count > 1)
				{
					if (brush2 == null)
					{
						dataSeries._internalColor = colorSet.GetNewColorFromColorSet();
					}
				}
				else
				{
					dataSeries._internalColor = null;
				}
				int num2 = 0;
				bool flag2 = false;
				List<Brush> list2 = new List<Brush>();
				foreach (IDataPoint current3 in dataSeries.InternalDataPoints)
				{
					current3.DpInfo.IsNotificationEnable = false;
					Brush color_DataPoint3 = DataPointHelper.GetColor_DataPoint(current3);
					if (color_DataPoint3 == null)
					{
						if (brush2 == null)
						{
							if (colorSet.Brushes.Count == num2)
							{
								flag2 = true;
								num2 = 0;
							}
							if (flag2)
							{
								current3.DpInfo.InternalColor = list2[num2];
								if (num2 == list2.Count - 1)
								{
									num2 = 0;
								}
								else
								{
									num2++;
								}
							}
							else
							{
								current3.DpInfo.InternalColor = colorSet.GetNewColorFromColorSet();
								list2.Add(current3.DpInfo.InternalColor);
								num2++;
							}
						}
						else
						{
							current3.DpInfo.InternalColor = brush2;
						}
					}
					else
					{
						current3.DpInfo.InternalColor = color_DataPoint3;
					}
					current3.DpInfo.IsNotificationEnable = true;
				}
			}
		}

		private void SetColor4CircularSeries(DataSeries dataSeries, ColorSet colorSet4MultiSeries, bool isUniqueColor4EachDP)
		{
			if (dataSeries.Color == null)
			{
				Brush newColorFromColorSet = colorSet4MultiSeries.GetNewColorFromColorSet();
				Brush brush = dataSeries.GetValue(DataSeries.ColorProperty) as Brush;
				if (brush == null)
				{
					dataSeries._internalColor = newColorFromColorSet;
				}
				else
				{
					dataSeries._internalColor = brush;
				}
				foreach (IDataPoint current in dataSeries.DataPoints)
				{
					current.DpInfo.IsNotificationEnable = false;
					Brush color_DataPoint = DataPointHelper.GetColor_DataPoint(current);
					if (color_DataPoint == null)
					{
						if (!isUniqueColor4EachDP)
						{
							if (brush == null)
							{
								dataSeries._internalColor = newColorFromColorSet;
							}
							else
							{
								dataSeries._internalColor = brush;
							}
							current.DpInfo.IsNotificationEnable = true;
							break;
						}
						if (brush == null)
						{
							current.DpInfo.InternalColor = colorSet4MultiSeries.GetNewColorFromColorSet();
						}
						else
						{
							current.DpInfo.InternalColor = brush;
						}
					}
					else
					{
						current.DpInfo.InternalColor = color_DataPoint;
					}
					current.DpInfo.IsNotificationEnable = true;
				}
			}
		}

		internal void LoadSeriesColorSet(DataSeries dataSeries)
		{
			bool flag = false;
			Brush internalColor = null;
			if (dataSeries.RenderAs == RenderAs.CandleStick)
			{
				ColorSet colorSet = this._financialColorSet;
				if (!string.IsNullOrEmpty(dataSeries.ColorSet))
				{
					colorSet = this.Chart.GetColorSetByName(dataSeries.ColorSet);
					if (colorSet == null)
					{
						throw new Exception("ColorSet named " + dataSeries.ColorSet + " is not found.");
					}
				}
				if (dataSeries.PriceUpColor == null)
				{
					dataSeries.IsNotificationEnable = false;
					dataSeries.PriceUpColor = colorSet.GetNewColorFromColorSet();
					dataSeries.IsNotificationEnable = true;
					return;
				}
			}
			else
			{
				ColorSet colorSet;
				if (RenderHelper.IsCircularCType(dataSeries))
				{
					colorSet = this._circularChartColorSet;
					if (!string.IsNullOrEmpty(dataSeries.ColorSet))
					{
						colorSet = this.Chart.GetColorSetByName(dataSeries.ColorSet);
						if (colorSet == null)
						{
							throw new Exception("ColorSet named " + dataSeries.ColorSet + " is not found.");
						}
						flag = true;
					}
					this.SetColor4CircularSeries(dataSeries, colorSet, flag);
					return;
				}
				colorSet = this._chartColorSet;
				flag = false;
				if (!string.IsNullOrEmpty(dataSeries.ColorSet))
				{
					colorSet = this.Chart.GetColorSetByName(dataSeries.ColorSet);
					if (colorSet == null)
					{
						throw new Exception("ColorSet named " + dataSeries.ColorSet + " is not found.");
					}
					flag = true;
				}
				else if (colorSet == null)
				{
					throw new Exception("ColorSet named " + this.Chart.ColorSet + " is not found.");
				}
				if (dataSeries.RenderAs == RenderAs.Area || dataSeries.RenderAs == RenderAs.StackedArea || dataSeries.RenderAs == RenderAs.StackedArea100)
				{
					internalColor = colorSet.GetNewColorFromColorSet();
					Brush brush = dataSeries.GetValue(DataSeries.ColorProperty) as Brush;
					if (brush == null)
					{
						dataSeries._internalColor = internalColor;
					}
					else
					{
						dataSeries._internalColor = brush;
					}
					using (List<IDataPoint>.Enumerator enumerator = dataSeries.InternalDataPoints.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IDataPoint current = enumerator.Current;
							current.DpInfo.IsNotificationEnable = false;
							Brush color_DataPoint = DataPointHelper.GetColor_DataPoint(current);
							if (color_DataPoint != null)
							{
								current.DpInfo.InternalColor = color_DataPoint;
							}
							current.DpInfo.IsNotificationEnable = true;
						}
						goto IL_2EB;
					}
				}
				if (!flag || RenderHelper.IsLineCType(dataSeries))
				{
					internalColor = colorSet.GetNewColorFromColorSet();
				}
				dataSeries._internalColor = internalColor;
				foreach (IDataPoint current2 in dataSeries.InternalDataPoints)
				{
					current2.DpInfo.IsNotificationEnable = false;
					Brush color_DataPoint2 = DataPointHelper.GetColor_DataPoint(current2);
					Brush brush2 = dataSeries.GetValue(DataSeries.ColorProperty) as Brush;
					if (color_DataPoint2 == null)
					{
						if (flag)
						{
							if (brush2 == null)
							{
								current2.DpInfo.InternalColor = colorSet.GetNewColorFromColorSet();
								if (RenderHelper.IsLineCType(dataSeries))
								{
									dataSeries._internalColor = internalColor;
								}
								else
								{
									dataSeries._internalColor = null;
								}
							}
							else
							{
								dataSeries._internalColor = brush2;
							}
						}
						else
						{
							if (brush2 == null)
							{
								dataSeries._internalColor = internalColor;
							}
							else
							{
								dataSeries._internalColor = brush2;
							}
							current2.DpInfo.IsNotificationEnable = true;
						}
					}
					else
					{
						current2.DpInfo.InternalColor = color_DataPoint2;
					}
					current2.DpInfo.IsNotificationEnable = true;
				}
				IL_2EB:
				dataSeries.IsNotificationEnable = true;
			}
		}

		private bool CheckShowYValueAndPercentageInLegendApplicable(RenderAs renderAs)
		{
			return renderAs != RenderAs.Stock && renderAs != RenderAs.CandleStick;
		}

		private void AddEntriesAsSeriesToLegend(Legend legend, DataSeries dataSeries)
		{
			string item;
			if (string.IsNullOrEmpty((string)dataSeries.GetValue(DataSeries.LegendTextProperty)))
			{
				if (dataSeries._isAutoName)
				{
					string[] array = dataSeries.Name.Split(new char[]
					{
						'_'
					});
					item = array[0];
				}
				else
				{
					item = dataSeries.Name;
				}
			}
			else
			{
				item = ObservableObject.GetFormattedMultilineText(dataSeries.LegendText);
			}
			Brush brush;
			if (dataSeries.RenderAs == RenderAs.CandleStick)
			{
				brush = dataSeries.PriceUpColor;
			}
			else
			{
				brush = dataSeries.Color;
			}
			if (dataSeries.InternalDataPoints.Count > 0)
			{
				IDataPoint dataPoint = dataSeries.InternalDataPoints[0];
				brush = (brush ?? dataPoint.Color);
			}
			if (dataSeries.LegendMarkerColor != null)
			{
				brush = dataSeries.LegendMarkerColor;
			}
			if (dataSeries.LightingEnabled.Value && RenderHelper.IsLineCType(dataSeries))
			{
				brush = Graphics.GetLightingEnabledBrush(brush, "Linear", new double[]
				{
					0.65,
					0.55
				});
			}
			bool markerBevel = (dataSeries.LegendMarkerColor == null || !typeof(ImageBrush).Equals(dataSeries.LegendMarkerColor.GetType())) && dataSeries.RenderAs != RenderAs.Point && !RenderHelper.IsFinancialCType(dataSeries) && dataSeries.RenderAs != RenderAs.Bubble && !RenderHelper.IsLineCType(dataSeries) && !this.Chart.View3D && dataSeries.Bevel.Value && dataSeries.Bevel.Value;
			Size markerSize = new Size(legend.MarkerSize, legend.MarkerSize);
			MarkerTypes markerType = this.RenderAsToLegendMarkerType(dataSeries.RenderAs, dataSeries);
			if (dataSeries.LegendMarkerType.HasValue)
			{
				markerType = dataSeries.LegendMarkerType.Value;
			}
			dataSeries.LegendMarker = new Marker();
			dataSeries.LegendMarker.AssignPropertiesValue(markerType, 1.0, markerSize, markerBevel, brush, "");
			dataSeries.LegendMarker.DataSeriesOfLegendMarker = dataSeries;
			if (dataSeries.RenderAs == RenderAs.QuickLine || ((RenderHelper.IsLineCType(dataSeries) || RenderHelper.IsFinancialCType(dataSeries)) && dataSeries.MarkerEnabled == false))
			{
				dataSeries.LegendMarker.Opacity = 0.0;
			}
			dataSeries.LegendMarker.Tag = new ElementData
			{
				Element = dataSeries
			};
			legend.Entries.Add(new LegendEntry(dataSeries.LegendMarker, new List<string>
			{
				item
			}, new List<HorizontalAlignment>
			{
				HorizontalAlignment.Left
			}));
		}

		private void AddEntriesToLegendForSingleSeriesChart(Legend legend, List<IDataPoint> dataPoints, bool includeDataPointsInLegend)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			double num = 0.0;
			if (dataPoints.Count > 0)
			{
				DataSeries parent = dataPoints[0].Parent;
				if (dataPoints.Count > 500 && parent.LightWeight == true)
				{
					return;
				}
				if (parent.IncludeYValueInLegend || parent.IncludePercentageInLegend)
				{
					num = (from dp in dataPoints
					where !double.IsNaN(dp.YValue) && dp.Enabled.Value
					select dp.YValue).Sum();
				}
				flag3 = this.CheckShowYValueAndPercentageInLegendApplicable(parent.RenderAs);
			}
			string format = "#0.##";
			string format2 = "#0.##";
			foreach (IDataPoint current in dataPoints)
			{
				DataSeries parent2 = current.Parent;
				if ((bool)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.ShowInLegend) && (current.Parent.IsAllDataPointsEnabled || current.Enabled.Value) && ((parent2.RenderAs != RenderAs.SectionFunnel && parent2.RenderAs != RenderAs.StreamLineFunnel && parent2.RenderAs != RenderAs.Pyramid) || current.DpInfo.InternalYValue >= 0.0))
				{
					string item = string.IsNullOrEmpty((string)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LegendText)) ? current.Name : ObservableObject.GetFormattedMultilineText(current.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LegendText)));
					Brush brush = includeDataPointsInLegend ? current.Color : current.DpInfo.InternalColor;
					if (parent2.RenderAs == RenderAs.QuickLine)
					{
						brush = parent2.Color;
					}
					if (current.Parent.RenderAs == RenderAs.CandleStick)
					{
						brush = DataPointHelper.GetColor_DataPoint(current);
						if (brush == null)
						{
							if (current.DpInfo.InternalYValues != null && current.DpInfo.InternalYValues.Length >= 2)
							{
								double num2 = current.DpInfo.InternalYValues[0];
								double num3 = current.DpInfo.InternalYValues[1];
								brush = ((num3 > num2) ? current.Parent.PriceUpColor : current.Parent.PriceDownColor);
							}
						}
						else
						{
							brush = current.Color;
						}
					}
					if (DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LegendMarkerColor) != null)
					{
						brush = (Brush)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LegendMarkerColor);
					}
					if (includeDataPointsInLegend && brush == null)
					{
						brush = parent2.Color;
					}
					if ((bool)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LightingEnabled) && RenderHelper.IsLineCType(current.Parent))
					{
						brush = Graphics.GetLightingEnabledBrush(brush, "Linear", new double[]
						{
							0.65,
							0.55
						});
					}
					bool markerBevel = (brush == null || !typeof(ImageBrush).Equals(brush.GetType())) && current.Parent.RenderAs != RenderAs.Bubble && current.Parent.RenderAs != RenderAs.CandleStick && current.Parent.RenderAs != RenderAs.Doughnut && !RenderHelper.IsLineCType(current.Parent) && !RenderHelper.IsFinancialCType(current.Parent) && current.Parent.RenderAs != RenderAs.Point && current.Parent.RenderAs != RenderAs.Pie && current.Parent.RenderAs != RenderAs.Stock && !this.Chart.View3D && current.Parent.Bevel.Value && current.Parent.Bevel.Value;
					MarkerTypes markerType = this.RenderAsToLegendMarkerType(current.Parent.RenderAs, current);
					if (DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LegendMarkerType) != null)
					{
						markerType = (MarkerTypes)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.LegendMarkerType);
					}
					Size markerSize = new Size(legend.MarkerSize, legend.MarkerSize);
					current.DpInfo.LegendMarker = new Marker();
					current.DpInfo.LegendMarker.AssignPropertiesValue(markerType, 1.0, markerSize, markerBevel, brush, "");
					if ((!(bool)DataPointHelper.GetDataPointValueFromProperty(current, VcProperties.MarkerEnabled) && RenderHelper.IsLineCType(parent2)) || RenderHelper.IsFinancialCType(parent2))
					{
						if (RenderHelper.IsFinancialCType(parent2) || (!current.Parent.IncludeYValueInLegend && !current.Parent.IncludePercentageInLegend))
						{
							current.DpInfo.LegendMarker.Opacity = 0.0;
						}
						else
						{
							current.DpInfo.LegendMarker.BorderThickness = 2.5;
							current.DpInfo.LegendMarker.MarkerSize = new Size(18.0, 10.0);
							current.DpInfo.LegendMarker._isLineWithoutMarker = true;
						}
					}
					if (current.Parent.RenderAs == RenderAs.QuickLine)
					{
						if (current.Parent.IncludeYValueInLegend || current.Parent.IncludePercentageInLegend)
						{
							current.DpInfo.LegendMarker.BorderThickness = 2.5;
							current.DpInfo.LegendMarker.MarkerSize = new Size(18.0, 10.0);
							current.DpInfo.LegendMarker._isLineWithoutMarker = true;
							current.DpInfo.LegendMarker.Opacity = 1.0;
						}
						else
						{
							current.DpInfo.LegendMarker.Opacity = 0.0;
						}
					}
					current.DpInfo.LegendMarker.DataSeriesOfLegendMarker = current.Parent;
					current.DpInfo.LegendMarker.Tag = new ElementData
					{
						Element = current
					};
					List<string> list = new List<string>
					{
						item
					};
					List<HorizontalAlignment> list2 = new List<HorizontalAlignment>
					{
						HorizontalAlignment.Left
					};
					format = (string.IsNullOrEmpty(parent2.YValueFormatString) ? "#0.##" : parent2.YValueFormatString);
					format2 = (string.IsNullOrEmpty(parent2.PercentageFormatString) ? "#0.##" : parent2.PercentageFormatString);
					if (parent2.IncludeYValueInLegend && flag3)
					{
						list.Add(double.IsNaN(current.YValue) ? "" : current.YValue.ToString(format));
						list2.Add(HorizontalAlignment.Right);
						legend.Layout = Layouts.GridLayout;
						flag = true;
					}
					if (parent2.IncludePercentageInLegend && !double.IsNaN(current.YValue) && flag3)
					{
						list.Add((current.YValue / num * 100.0).ToString(format2));
						list2.Add(HorizontalAlignment.Right);
						list.Add("%");
						list2.Add(HorizontalAlignment.Left);
						legend.Layout = Layouts.GridLayout;
						flag2 = true;
					}
					legend.Entries.Add(new LegendEntry(current.DpInfo.LegendMarker, list, list2));
				}
			}
			if (legend != null && legend.Reversed)
			{
				legend.Entries.Reverse();
			}
			if (flag3 && (flag2 || flag))
			{
				List<string> list3 = new List<string>();
				List<HorizontalAlignment> list4 = new List<HorizontalAlignment>();
				legend.Entries.Add(new LegendEntry
				{
					IsCompleteLine = true
				});
				list3.Add("Total");
				list4.Add(HorizontalAlignment.Right);
				if (flag)
				{
					list3.Add(num.ToString(format));
					list4.Add(HorizontalAlignment.Right);
				}
				if (flag2)
				{
					string item2 = 100.0.ToString(format2);
					list3.Add(item2);
					list4.Add(HorizontalAlignment.Right);
					list3.Add("%");
					list4.Add(HorizontalAlignment.Left);
				}
				legend.Entries.Add(new LegendEntry(null, list3, list4));
			}
		}

		private void AddLegends(Chart chart, bool dockInsidePlotArea, double heightAvailable, double widthAvailable)
		{
			if (chart.Series.Count == 0)
			{
				return;
			}
			List<Legend> list = (from legend in chart.Legends
			where legend.DockInsidePlotArea == dockInsidePlotArea
			select legend).ToList<Legend>();
			if (list.Count <= 0)
			{
				return;
			}
			if ((chart.Series.Count == 1 || chart.Series[0].RenderAs == RenderAs.Pie || chart.Series[0].RenderAs == RenderAs.Doughnut || chart.Series[0].RenderAs == RenderAs.SectionFunnel || chart.Series[0].RenderAs == RenderAs.StreamLineFunnel || chart.Series[0].RenderAs == RenderAs.Pyramid) && chart.Series[0].Enabled.Value)
			{
				Legend legend3 = null;
				foreach (Legend current in chart.Legends)
				{
					current.Entries.Clear();
				}
				if (chart.Legends.Count > 0)
				{
					IEnumerable<Legend> source = from entry in chart.Legends
					where entry.GetLegendName() == entry.GetLegendName4Series(chart.Series[0].Legend) && entry.DockInsidePlotArea == dockInsidePlotArea
					select entry;
					if (source.Any<Legend>())
					{
						legend3 = source.First<Legend>();
					}
				}
				if (legend3 == null)
				{
					return;
				}
				if (chart.Series[0].ShowInLegend == true && chart.Series[0].IncludeDataPointsInLegend == false)
				{
					this.AddEntriesAsSeriesToLegend(legend3, chart.Series[0]);
				}
				else
				{
					this.AddEntriesToLegendForSingleSeriesChart(legend3, chart.Series[0].InternalDataPoints.ToList<IDataPoint>(), false);
				}
			}
			else
			{
				List<DataSeries> list2 = (from entry in chart.Series
				where (entry.ShowInLegend == true || entry.IncludeDataPointsInLegend == true) && entry.Enabled == true
				select entry).ToList<DataSeries>();
				if (list2.Count > 0)
				{
					Legend legend2 = null;
					foreach (Legend current2 in chart.Legends)
					{
						current2.Entries.Clear();
					}
					foreach (DataSeries dataSeries in list2)
					{
						if (chart.Legends.Count > 0)
						{
							legend2 = null;
							IEnumerable<Legend> source2 = from entry in chart.Legends
							where entry.GetLegendName() == entry.GetLegendName4Series(dataSeries.Legend) && entry.DockInsidePlotArea == dockInsidePlotArea
							select entry;
							if (source2.Any<Legend>())
							{
								legend2 = source2.First<Legend>();
							}
						}
						if (legend2 != null)
						{
							if (dataSeries.IncludeDataPointsInLegend == true)
							{
								this.AddEntriesToLegendForSingleSeriesChart(legend2, dataSeries.InternalDataPoints, true);
							}
							else
							{
								this.AddEntriesAsSeriesToLegend(legend2, dataSeries);
							}
						}
					}
					if (legend2 != null && legend2.Reversed)
					{
						legend2.Entries.Reverse();
					}
				}
			}
			List<Legend> legendsOnTop = (from entry in chart.Legends
			where entry.Entries.Count > 0 && entry.InternalVerticalAlignment == VerticalAlignment.Top && entry.DockInsidePlotArea == dockInsidePlotArea && entry.Enabled.Value
			select entry).ToList<Legend>();
			List<Legend> legendsOnBottom = (from entry in chart.Legends
			where entry.Entries.Count > 0 && entry.InternalVerticalAlignment == VerticalAlignment.Bottom && entry.DockInsidePlotArea == dockInsidePlotArea && entry.Enabled.Value
			select entry).ToList<Legend>();
			List<Legend> legendsOnLeft = (from entry in chart.Legends
			where entry.Entries.Count > 0 && (entry.InternalVerticalAlignment == VerticalAlignment.Center || entry.InternalVerticalAlignment == VerticalAlignment.Stretch) && entry.InternalHorizontalAlignment == HorizontalAlignment.Left && entry.DockInsidePlotArea == dockInsidePlotArea && entry.Enabled.Value
			select entry).ToList<Legend>();
			List<Legend> legendsOnRight = (from entry in chart.Legends
			where entry.Entries.Count > 0 && (entry.InternalVerticalAlignment == VerticalAlignment.Center || entry.InternalVerticalAlignment == VerticalAlignment.Stretch) && entry.InternalHorizontalAlignment == HorizontalAlignment.Right && entry.DockInsidePlotArea == dockInsidePlotArea && entry.Enabled.Value
			select entry).ToList<Legend>();
			List<Legend> legendsAtCenter = (from entry in chart.Legends
			where entry.Entries.Count > 0 && (entry.InternalVerticalAlignment == VerticalAlignment.Center || entry.InternalVerticalAlignment == VerticalAlignment.Stretch) && (entry.InternalHorizontalAlignment == HorizontalAlignment.Center || entry.InternalHorizontalAlignment == HorizontalAlignment.Stretch) && entry.DockInsidePlotArea == dockInsidePlotArea && entry.Enabled.Value
			select entry).ToList<Legend>();
			StackPanel topLegendPanel;
			StackPanel bottomLegendPanel;
			StackPanel leftLegendPanel;
			StackPanel rightLegendPanel;
			StackPanel centerPanel;
			this.GetReferenceOfLegendPanels(dockInsidePlotArea, out topLegendPanel, out bottomLegendPanel, out leftLegendPanel, out rightLegendPanel, out centerPanel);
			this.PlaceLegendsAtTop(legendsOnTop, heightAvailable, widthAvailable, topLegendPanel);
			this.PlaceLegendsAtBottom(legendsOnBottom, heightAvailable, widthAvailable, bottomLegendPanel);
			this.PlaceLegendsAtLeft(legendsOnLeft, heightAvailable, widthAvailable, leftLegendPanel);
			this.PlaceLegendsAtRight(legendsOnRight, heightAvailable, widthAvailable, rightLegendPanel);
			this.PlaceLegendsAtCenter(legendsAtCenter, heightAvailable, widthAvailable, centerPanel);
		}

		private void GetReferenceOfLegendPanels(bool dockInsidePlotArea, out StackPanel topLegendPanel, out StackPanel bottomLegendPanel, out StackPanel leftLegendPanel, out StackPanel rightLegendPanel, out StackPanel centerPanel)
		{
			if (dockInsidePlotArea)
			{
				topLegendPanel = this.Chart._topInnerLegendPanel;
				bottomLegendPanel = this.Chart._bottomInnerLegendPanel;
				leftLegendPanel = this.Chart._leftInnerLegendPanel;
				rightLegendPanel = this.Chart._rightInnerLegendPanel;
				centerPanel = this.Chart._centerDockInsidePlotAreaPanel;
				return;
			}
			topLegendPanel = this.Chart._topOuterLegendPanel;
			bottomLegendPanel = this.Chart._bottomOuterLegendPanel;
			leftLegendPanel = this.Chart._leftOuterLegendPanel;
			rightLegendPanel = this.Chart._rightOuterLegendPanel;
			centerPanel = this.Chart._centerDockOutsidePlotAreaPanel;
		}

		private void PlaceLegendsAtCenter(List<Legend> legendsAtCenter, double heightAvailable, double widthAvailable, StackPanel centerPanel)
		{
			if (legendsAtCenter.Count > 0)
			{
				foreach (Legend current in legendsAtCenter)
				{
					current.Orientation = Orientation.Horizontal;
					if (double.IsPositiveInfinity(current.InternalMaxWidth))
					{
						current.InternalMaximumWidth = widthAvailable * 60.0 / 100.0;
					}
					else if (current.InternalMaxWidth > widthAvailable * 60.0 / 100.0)
					{
						current.InternalMaximumWidth = widthAvailable * 60.0 / 100.0;
					}
					else
					{
						current.InternalMaximumWidth = current.InternalMaxWidth;
					}
					current.CreateVisualObject();
					if (current.Visual != null)
					{
						centerPanel.Children.Add(current.Visual);
					}
				}
			}
		}

		private void PlaceLegendsAtRight(List<Legend> legendsOnRight, double heightAvailable, double widthAvailable, StackPanel rightLegendPanel)
		{
			if (legendsOnRight.Count > 0)
			{
				legendsOnRight.Reverse();
				foreach (Legend current in legendsOnRight)
				{
					current.Orientation = Orientation.Vertical;
					if (!double.IsNaN(heightAvailable) && heightAvailable > 0.0)
					{
						if (double.IsPositiveInfinity(current.InternalMaxHeight))
						{
							current.InternalMaximumHeight = heightAvailable - this.Chart.BorderThickness.Top - this.Chart.BorderThickness.Bottom - this.Chart.Padding.Top - this.Chart.Padding.Bottom;
						}
						else if (current.InternalMaxHeight > heightAvailable - this.Chart.BorderThickness.Top - this.Chart.BorderThickness.Bottom - this.Chart.Padding.Top - this.Chart.Padding.Bottom)
						{
							current.InternalMaximumHeight = heightAvailable - this.Chart.BorderThickness.Top - this.Chart.BorderThickness.Bottom - this.Chart.Padding.Top - this.Chart.Padding.Bottom;
						}
						else
						{
							current.InternalMaximumHeight = current.InternalMaxHeight;
						}
						if (double.IsPositiveInfinity(current.InternalMaxWidth))
						{
							current.InternalMaximumWidth = double.PositiveInfinity;
						}
						else if (current.InternalMaxWidth > widthAvailable - this.Chart.BorderThickness.Left - this.Chart.BorderThickness.Right - this.Chart.Padding.Left - this.Chart.Padding.Right)
						{
							current.InternalMaximumWidth = widthAvailable - this.Chart.BorderThickness.Left - this.Chart.BorderThickness.Right - this.Chart.Padding.Left - this.Chart.Padding.Right;
						}
						else
						{
							current.InternalMaximumWidth = current.InternalMaxWidth;
						}
					}
					current.CreateVisualObject();
					if (current.Visual != null)
					{
						rightLegendPanel.Children.Add(current.Visual);
					}
				}
			}
		}

		private void PlaceLegendsAtLeft(List<Legend> legendsOnLeft, double heightAvailable, double widthAvailable, StackPanel leftLegendPanel)
		{
			if (legendsOnLeft.Count > 0)
			{
				foreach (Legend current in legendsOnLeft)
				{
					current.Orientation = Orientation.Vertical;
					if (!double.IsNaN(widthAvailable) && widthAvailable > 0.0 && !double.IsNaN(heightAvailable) && heightAvailable > 0.0)
					{
						if (double.IsPositiveInfinity(current.InternalMaxHeight))
						{
							current.InternalMaximumHeight = heightAvailable - this.Chart.BorderThickness.Top - this.Chart.BorderThickness.Bottom - this.Chart.Padding.Top - this.Chart.Padding.Bottom;
						}
						else if (current.InternalMaxHeight > heightAvailable - this.Chart.BorderThickness.Top - this.Chart.BorderThickness.Bottom - this.Chart.Padding.Top - this.Chart.Padding.Bottom)
						{
							current.InternalMaximumHeight = heightAvailable - this.Chart.BorderThickness.Top - this.Chart.BorderThickness.Bottom - this.Chart.Padding.Top - this.Chart.Padding.Bottom;
						}
						else
						{
							current.InternalMaximumHeight = current.InternalMaxHeight;
						}
						if (double.IsPositiveInfinity(current.InternalMaxWidth))
						{
							current.InternalMaximumWidth = double.PositiveInfinity;
						}
						else if (current.InternalMaxWidth > widthAvailable - this.Chart.BorderThickness.Left - this.Chart.BorderThickness.Right - this.Chart.Padding.Left - this.Chart.Padding.Right)
						{
							current.InternalMaximumWidth = widthAvailable - this.Chart.BorderThickness.Left - this.Chart.BorderThickness.Right - this.Chart.Padding.Left - this.Chart.Padding.Right;
						}
						else
						{
							current.InternalMaximumWidth = current.InternalMaxWidth;
						}
					}
					current.CreateVisualObject();
					if (current.Visual != null)
					{
						leftLegendPanel.Children.Add(current.Visual);
					}
				}
			}
		}

		private void PlaceLegendsAtBottom(List<Legend> legendsOnBottom, double heightAvailable, double widthAvailable, StackPanel bottomLegendPanel)
		{
			if (legendsOnBottom.Count > 0)
			{
				legendsOnBottom.Reverse();
				foreach (Legend current in legendsOnBottom)
				{
					current.Orientation = Orientation.Horizontal;
					if (!double.IsNaN(widthAvailable) && widthAvailable > 0.0 && !double.IsNaN(heightAvailable) && heightAvailable > 0.0)
					{
						if (double.IsPositiveInfinity(current.InternalMaxWidth))
						{
							current.InternalMaximumWidth = widthAvailable - this.Chart.BorderThickness.Left - this.Chart.BorderThickness.Right - this.Chart.Padding.Left - this.Chart.Padding.Right;
						}
						else if (current.InternalMaxWidth > widthAvailable - this.Chart.BorderThickness.Left - this.Chart.BorderThickness.Right - this.Chart.Padding.Left - this.Chart.Padding.Right)
						{
							current.InternalMaximumWidth = widthAvailable - this.Chart.BorderThickness.Left - this.Chart.BorderThickness.Right - this.Chart.Padding.Left - this.Chart.Padding.Right;
						}
						else
						{
							current.InternalMaximumWidth = current.InternalMaxWidth;
						}
						if (double.IsPositiveInfinity(current.InternalMaxHeight))
						{
							current.InternalMaximumHeight = double.PositiveInfinity;
						}
						else if (current.InternalMaxHeight > heightAvailable - this.Chart.BorderThickness.Top - this.Chart.BorderThickness.Bottom - this.Chart.Padding.Top - this.Chart.Padding.Bottom)
						{
							current.InternalMaximumHeight = heightAvailable - this.Chart.BorderThickness.Top - this.Chart.BorderThickness.Bottom - this.Chart.Padding.Top - this.Chart.Padding.Bottom;
						}
						else
						{
							current.InternalMaximumHeight = current.InternalMaxHeight;
						}
					}
					current.CreateVisualObject();
					if (current.Visual != null)
					{
						bottomLegendPanel.Children.Add(current.Visual);
					}
				}
			}
		}

		private void PlaceLegendsAtTop(List<Legend> legendsOnTop, double heightAvailable, double widthAvailable, StackPanel topLegendPanel)
		{
			if (legendsOnTop.Count > 0)
			{
				foreach (Legend current in legendsOnTop)
				{
					current.Orientation = Orientation.Horizontal;
					if (!double.IsNaN(widthAvailable) && widthAvailable > 0.0)
					{
						if (double.IsPositiveInfinity(current.InternalMaxWidth))
						{
							current.InternalMaximumWidth = widthAvailable - this.Chart.BorderThickness.Left - this.Chart.BorderThickness.Right - this.Chart.Padding.Left - this.Chart.Padding.Right;
						}
						else if (current.InternalMaxWidth > widthAvailable - this.Chart.BorderThickness.Left - this.Chart.BorderThickness.Right - this.Chart.Padding.Left - this.Chart.Padding.Right)
						{
							current.InternalMaximumWidth = widthAvailable - this.Chart.BorderThickness.Left - this.Chart.BorderThickness.Right - this.Chart.Padding.Left - this.Chart.Padding.Right;
						}
						else
						{
							current.InternalMaximumWidth = current.InternalMaxWidth;
						}
						if (double.IsPositiveInfinity(current.InternalMaxHeight))
						{
							current.InternalMaximumHeight = double.PositiveInfinity;
						}
						else if (current.InternalMaxHeight > heightAvailable - this.Chart.BorderThickness.Top - this.Chart.BorderThickness.Bottom - this.Chart.Padding.Top - this.Chart.Padding.Bottom)
						{
							current.InternalMaximumHeight = heightAvailable - this.Chart.BorderThickness.Top - this.Chart.BorderThickness.Bottom - this.Chart.Padding.Top - this.Chart.Padding.Bottom;
						}
						else
						{
							current.InternalMaximumHeight = current.InternalMaxHeight;
						}
					}
					current.CreateVisualObject();
					if (current.Visual != null)
					{
						topLegendPanel.Children.Add(current.Visual);
					}
				}
			}
		}

		private void AddTitles(Chart chart, bool DockInsidePlotArea, double height, double width, out bool isHLeftOrRightVCenterTitlesExists)
		{
			StackPanel panel = null;
			StackPanel panel2 = null;
			StackPanel panel3 = null;
			StackPanel stackPanel = null;
			isHLeftOrRightVCenterTitlesExists = false;
			IList<Title> list;
			StackPanel panel4;
			if (DockInsidePlotArea)
			{
				list = chart.GetTitlesDockedInsidePlotArea();
				panel4 = chart._topInnerTitlePanel;
				panel = chart._bottomInnerTitlePanel;
				panel2 = chart._leftInnerTitlePanel;
				panel3 = chart._rightInnerTitlePanel;
				stackPanel = chart._centerDockInsidePlotAreaPanel;
			}
			else
			{
				list = chart.GetTitlesDockedOutSidePlotArea();
				panel4 = chart._topOuterTitlePanel;
				panel = chart._bottomOuterTitlePanel;
				panel2 = chart._leftOuterTitlePanel;
				panel3 = chart._rightOuterTitlePanel;
				stackPanel = chart._centerDockOutsidePlotAreaPanel;
			}
			if (list.Count == 0)
			{
				return;
			}
			IEnumerable<Title> enumerable = from title in list
			where title.InternalVerticalAlignment == VerticalAlignment.Top && title.Enabled == true
			select title;
			if (enumerable.Count<Title>() > 0)
			{
				Title title2 = enumerable.Last<Title>();
				if (!title2._internalMargin.HasValue)
				{
					if (chart.View3D || chart.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal || chart.PlotDetails.ChartOrientation == ChartOrientationType.Circular)
					{
						title2.InternalMargin = new Thickness(0.0, 0.0, 0.0, 6.0);
					}
					else
					{
						title2.InternalMargin = new Thickness(0.0, 0.0, 0.0, 4.0);
					}
				}
			}
			foreach (Title current in enumerable)
			{
				this.AddTitle(chart, current, panel4, width, height);
			}
			IEnumerable<Title> enumerable2 = from title in list
			where title.InternalVerticalAlignment == VerticalAlignment.Bottom && title.Enabled == true
			select title;
			enumerable2.Reverse<Title>();
			foreach (Title current2 in enumerable2)
			{
				this.AddTitle(chart, current2, panel, width, height);
			}
			IEnumerable<Title> enumerable3 = from title in list
			where (title.InternalVerticalAlignment == VerticalAlignment.Center || title.InternalVerticalAlignment == VerticalAlignment.Stretch) && title.InternalHorizontalAlignment == HorizontalAlignment.Left && title.Enabled == true
			select title;
			if (enumerable3.Any<Title>())
			{
				isHLeftOrRightVCenterTitlesExists = true;
			}
			foreach (Title current3 in enumerable3)
			{
				this.AddTitle(chart, current3, panel2, width, height);
			}
			IEnumerable<Title> enumerable4 = from title in list
			where (title.InternalVerticalAlignment == VerticalAlignment.Center || title.InternalVerticalAlignment == VerticalAlignment.Stretch) && title.InternalHorizontalAlignment == HorizontalAlignment.Right && title.Enabled == true
			select title;
			if (enumerable4.Any<Title>())
			{
				isHLeftOrRightVCenterTitlesExists = true;
			}
			enumerable4.Reverse<Title>();
			foreach (Title current4 in enumerable4)
			{
				this.AddTitle(chart, current4, panel3, width, height);
			}
			IEnumerable<Title> enumerable5 = from title in list
			where (title.InternalHorizontalAlignment == HorizontalAlignment.Center || title.InternalHorizontalAlignment == HorizontalAlignment.Stretch) && (title.InternalVerticalAlignment == VerticalAlignment.Center || title.InternalVerticalAlignment == VerticalAlignment.Stretch) && title.Enabled == true
			select title;
			stackPanel.Children.Clear();
			foreach (Title current5 in enumerable5)
			{
				this.AddTitle(chart, current5, stackPanel, width, height);
			}
		}

		private double GetChartAreaCenterGridTopMargin()
		{
			double num = 0.0;
			if (this.AxisY != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (this.AxisY.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisY.AxisLabels.TopOverflow);
				}
				foreach (CustomAxisLabels current in this.AxisY.CustomAxisLabels)
				{
					if (current.TopOverflow > num)
					{
						num = current.TopOverflow;
					}
				}
			}
			if (this.AxisY2 != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (this.AxisY2.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisY2.AxisLabels.TopOverflow);
				}
				foreach (CustomAxisLabels current2 in this.AxisY2.CustomAxisLabels)
				{
					if (current2.TopOverflow > num)
					{
						num = current2.TopOverflow;
					}
				}
			}
			if (this.AxisX != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				if (this.AxisX.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisX.AxisLabels.TopOverflow);
				}
				foreach (CustomAxisLabels current3 in this.AxisX.CustomAxisLabels)
				{
					if (current3.TopOverflow > num)
					{
						num = current3.TopOverflow;
					}
				}
			}
			if (this.AxisX2 != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				if (this.AxisX2.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisX2.AxisLabels.TopOverflow);
				}
				foreach (CustomAxisLabels current4 in this.AxisX2.CustomAxisLabels)
				{
					if (current4.TopOverflow > num)
					{
						num = current4.TopOverflow;
					}
				}
			}
			if (this.AxisX2 != null && this.AxisX2.Visual != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				this.AxisX2.Visual.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				if (this.AxisX2.Visual.DesiredSize.Height < num)
				{
					return Math.Abs(num - this.AxisX2.Visual.DesiredSize.Height);
				}
				return 0.0;
			}
			else
			{
				if (this.AxisY2 == null || this.AxisY2.Visual == null || this.PlotDetails.ChartOrientation != ChartOrientationType.Horizontal)
				{
					return num;
				}
				this.AxisY2.Visual.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				if (this.AxisY2.Visual.DesiredSize.Height < num)
				{
					return Math.Abs(num - this.AxisY2.Visual.DesiredSize.Height);
				}
				return 0.0;
			}
		}

		private double GetChartAreaCenterGridBottomMargin()
		{
			double num = 0.0;
			if (this.AxisY != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (this.AxisY.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisY.AxisLabels.BottomOverflow);
				}
				foreach (CustomAxisLabels current in this.AxisY.CustomAxisLabels)
				{
					if (current.BottomOverflow > num)
					{
						num = current.BottomOverflow;
					}
				}
			}
			if (this.AxisY2 != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (this.AxisY2.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisY2.AxisLabels.BottomOverflow);
				}
				foreach (CustomAxisLabels current2 in this.AxisY2.CustomAxisLabels)
				{
					if (current2.BottomOverflow > num)
					{
						num = current2.BottomOverflow;
					}
				}
			}
			if (this.AxisX != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				if (this.AxisX.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisX.AxisLabels.BottomOverflow);
				}
				foreach (CustomAxisLabels current3 in this.AxisX.CustomAxisLabels)
				{
					if (current3.BottomOverflow > num)
					{
						num = current3.BottomOverflow;
					}
				}
			}
			if (this.AxisX2 != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				if (this.AxisX2.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisX2.AxisLabels.BottomOverflow);
				}
				foreach (CustomAxisLabels current4 in this.AxisX2.CustomAxisLabels)
				{
					if (current4.BottomOverflow > num)
					{
						num = current4.BottomOverflow;
					}
				}
			}
			if (this.AxisX != null && this.AxisX.Visual != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				this.AxisX.Visual.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				if (this.AxisX.Visual.DesiredSize.Height < num)
				{
					return Math.Abs(num - this.AxisX.Visual.DesiredSize.Height);
				}
				return 0.0;
			}
			else
			{
				if (this.AxisY == null || this.AxisY.Visual == null || this.PlotDetails.ChartOrientation != ChartOrientationType.Horizontal)
				{
					return num;
				}
				this.AxisY.Visual.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				if (this.AxisY.Visual.DesiredSize.Height < num)
				{
					return Math.Abs(num - this.AxisY.Visual.DesiredSize.Height);
				}
				return 0.0;
			}
		}

		private double GetChartAreaCenterGridRightMargin()
		{
			double num = 0.0;
			if (this.AxisX != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (this.AxisX.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisX.AxisLabels.RightOverflow);
				}
				foreach (CustomAxisLabels current in this.AxisX.CustomAxisLabels)
				{
					if (current.RightOverflow > num)
					{
						num = current.RightOverflow;
					}
				}
			}
			if (this.AxisX2 != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (this.AxisX2.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisX2.AxisLabels.RightOverflow);
				}
				foreach (CustomAxisLabels current2 in this.AxisX2.CustomAxisLabels)
				{
					if (current2.RightOverflow > num)
					{
						num = current2.RightOverflow;
					}
				}
			}
			if (this.AxisY != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				if (this.AxisY.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisY.AxisLabels.RightOverflow);
				}
				foreach (CustomAxisLabels current3 in this.AxisY.CustomAxisLabels)
				{
					if (current3.RightOverflow > num)
					{
						num = current3.RightOverflow;
					}
				}
			}
			if (this.AxisY2 != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				if (this.AxisY2.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisY2.AxisLabels.RightOverflow);
				}
				foreach (CustomAxisLabels current4 in this.AxisY2.CustomAxisLabels)
				{
					if (current4.RightOverflow > num)
					{
						num = current4.RightOverflow;
					}
				}
			}
			if (this.AxisY2 != null && this.AxisY2.Visual != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				this.AxisY2.Visual.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				if (this.AxisY2.Visual.DesiredSize.Width < num)
				{
					return Math.Abs(num - this.AxisY2.Visual.DesiredSize.Width);
				}
				return 0.0;
			}
			else
			{
				if (this.AxisX2 == null || this.AxisX2.Visual == null || this.PlotDetails.ChartOrientation != ChartOrientationType.Horizontal)
				{
					return num;
				}
				this.AxisX2.Visual.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				if (this.AxisX2.Visual.DesiredSize.Width < num)
				{
					return Math.Abs(num - this.AxisX2.Visual.DesiredSize.Width);
				}
				return 0.0;
			}
		}

		private double GetChartAreaCenterGridLeftMargin()
		{
			double num = 0.0;
			if (this.AxisX != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (this.AxisX.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisX.AxisLabels.LeftOverflow);
				}
				foreach (CustomAxisLabels current in this.AxisX.CustomAxisLabels)
				{
					if (current.LeftOverflow > num)
					{
						num = current.LeftOverflow;
					}
				}
			}
			if (this.AxisX2 != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				if (this.AxisX2.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisX2.AxisLabels.LeftOverflow);
				}
				foreach (CustomAxisLabels current2 in this.AxisX2.CustomAxisLabels)
				{
					if (current2.LeftOverflow > num)
					{
						num = current2.LeftOverflow;
					}
				}
			}
			if (this.AxisY != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				if (this.AxisY.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisY.AxisLabels.LeftOverflow);
				}
				foreach (CustomAxisLabels current3 in this.AxisY.CustomAxisLabels)
				{
					if (current3.LeftOverflow > num)
					{
						num = current3.LeftOverflow;
					}
				}
			}
			if (this.AxisY2 != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				if (this.AxisY2.AxisLabels != null)
				{
					num = Math.Max(num, this.AxisY2.AxisLabels.LeftOverflow);
				}
				foreach (CustomAxisLabels current4 in this.AxisY2.CustomAxisLabels)
				{
					if (current4.LeftOverflow > num)
					{
						num = current4.LeftOverflow;
					}
				}
			}
			if (this.AxisY != null && this.AxisY.Visual != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				this.AxisY.Visual.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				if (this.AxisY.Visual.DesiredSize.Width < num)
				{
					return Math.Abs(num - this.AxisY.Visual.DesiredSize.Width);
				}
				return 0.0;
			}
			else
			{
				if (this.AxisX == null || this.AxisX.Visual == null || this.PlotDetails.ChartOrientation != ChartOrientationType.Horizontal)
				{
					return num;
				}
				this.AxisX.Visual.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				if (this.AxisX.Visual.DesiredSize.Width < num)
				{
					return Math.Abs(num - this.AxisX.Visual.DesiredSize.Width);
				}
				return 0.0;
			}
		}

		internal Size SetChartAreaCenterGridMargin(Size newSize, ref double left, ref double bottom, ref double right, ref double top)
		{
			newSize.Height += top;
			newSize.Height += bottom;
			newSize.Width += left;
			newSize.Width += right;
			left = this.GetChartAreaCenterGridLeftMargin();
			right = this.GetChartAreaCenterGridRightMargin();
			top = this.GetChartAreaCenterGridTopMargin();
			bottom = this.GetChartAreaCenterGridBottomMargin();
			this.Chart._topOffsetGrid.Height = top;
			this.Chart._bottomOffsetGrid.Height = bottom;
			this.Chart._rightOffsetGrid.Width = right;
			this.Chart._leftOffsetGrid.Width = left;
			newSize.Height = Math.Max(newSize.Height - top, 0.0);
			newSize.Height = Math.Max(newSize.Height - bottom, 0.0);
			newSize.Width = Math.Max(newSize.Width - left, 0.0);
			newSize.Width = Math.Max(newSize.Width - right, 0.0);
			return newSize;
		}

		private void SetAxisProperties(Axis axis, double startOffset, double endOffset)
		{
			if (axis != null)
			{
				axis.ApplyStyleFromTheme(this.Chart, axis.AxisRepresentation.ToString());
				axis.StartOffset = startOffset;
				axis.EndOffset = endOffset;
				axis.SetScrollBar();
			}
		}

		private void SetAxesProperties()
		{
			this.AxisX = this.PlotDetails.GetAxisXFromChart(this.Chart, AxisTypes.Primary);
			this.AxisX2 = this.PlotDetails.GetAxisXFromChart(this.Chart, AxisTypes.Secondary);
			Axis axis = this.AxisY;
			this.AxisY = this.PlotDetails.GetAxisYFromChart(this.Chart, AxisTypes.Primary);
			if (axis != null && this.AxisY == null)
			{
				this.CleanUpGrids(axis);
			}
			axis = this.AxisY2;
			this.AxisY2 = this.PlotDetails.GetAxisYFromChart(this.Chart, AxisTypes.Secondary);
			if (axis != null && this.AxisY2 == null)
			{
				this.CleanUpGrids(axis);
			}
			if (this.AxisX != null)
			{
				if (this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					this.SetAxisProperties(this.AxisX, 0.0, this.PLANK_DEPTH);
					this.SetAxisProperties(this.AxisX2, this.PLANK_DEPTH, 0.0);
					this.SetAxisProperties(this.AxisY, this.PLANK_DEPTH, this.PLANK_THICKNESS);
					this.SetAxisProperties(this.AxisY2, 0.0, this.PLANK_OFFSET);
					return;
				}
				this.SetAxisProperties(this.AxisX, this.PLANK_DEPTH, 0.0);
				this.SetAxisProperties(this.AxisX2, this.PLANK_DEPTH, 0.0);
				this.SetAxisProperties(this.AxisY, this.PLANK_OFFSET, 0.0);
				this.SetAxisProperties(this.AxisY2, this.PLANK_OFFSET, 0.0);
			}
		}

		private void ApplyOpacity()
		{
			foreach (DataSeries current in this.Chart.InternalSeries)
			{
				RenderAs renderAs = current.RenderAs;
				if (renderAs <= RenderAs.Doughnut)
				{
					if (renderAs != RenderAs.Pie && renderAs != RenderAs.Doughnut)
					{
						continue;
					}
				}
				else
				{
					switch (renderAs)
					{
					case RenderAs.StackedArea:
					case RenderAs.StackedArea100:
					case RenderAs.StreamLineFunnel:
					case RenderAs.SectionFunnel:
						break;
					case RenderAs.Bubble:
					case RenderAs.Point:
						continue;
					default:
						if (renderAs != RenderAs.Pyramid)
						{
							continue;
						}
						break;
					}
				}
				if (current.Faces != null && current.Faces.Visual != null)
				{
					current.Faces.Visual.Opacity = current.Opacity;
				}
				foreach (IDataPoint current2 in current.InternalDataPoints)
				{
					double num = (double)DataPointHelper.GetDataPointValueFromProperty(current2, VcProperties.Opacity);
					if (current2.DpInfo.Faces != null)
					{
						if (!this.Chart.InternalAnimationEnabled || (this.Chart.InternalAnimationEnabled && !this._isFirstTimeRender))
						{
							if (current2.DpInfo.Faces.VisualComponents.Count != 0)
							{
								using (List<FrameworkElement>.Enumerator enumerator3 = current2.DpInfo.Faces.VisualComponents.GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										FrameworkElement current3 = enumerator3.Current;
										current3.Opacity = num * current.Opacity;
									}
									goto IL_1B4;
								}
							}
							if (current2.DpInfo.Faces.Visual != null)
							{
								current2.DpInfo.Faces.Visual.Opacity = current.Opacity * num;
							}
						}
						else if (current2.DpInfo.Faces.Visual != null)
						{
							current2.DpInfo.Faces.Visual.Opacity = current.Opacity * num;
						}
					}
					IL_1B4:
					if ((!this.Chart.InternalAnimationEnabled || (this.Chart.InternalAnimationEnabled && !this._isFirstTimeRender)) && current2.DpInfo.Marker != null && current2.DpInfo.Marker.Visual != null)
					{
						current2.DpInfo.Marker.Visual.Opacity = current.Opacity * num;
					}
				}
			}
		}

		private void Check4DefaultInteractivity(DataSeries ds)
		{
			if (ds.RenderAs == RenderAs.Pie || ds.RenderAs == RenderAs.Doughnut || ds.RenderAs == RenderAs.SectionFunnel || ds.RenderAs == RenderAs.StreamLineFunnel || ds.RenderAs == RenderAs.Pyramid)
			{
				this._isDefaultInteractivityAllowed = false;
				object obj = null;
				object obj2 = null;
				object obj3 = null;
				object obj4 = null;
				obj = ds.GetMouseLeftButtonDownEventHandler();
				obj2 = ds.GetMouseLeftButtonUpEventHandler();
				foreach (IDataPoint current in ds.DataPoints)
				{
					if (!current.IsLightDataPoint)
					{
						obj3 = (current as DataPoint).GetMouseLeftButtonDownEventHandler();
						obj4 = (current as DataPoint).GetMouseLeftButtonUpEventHandler();
						if (obj3 != null || obj4 != null)
						{
							break;
						}
					}
				}
				if (obj == null && obj2 == null && obj3 == null && obj4 == null)
				{
					this._isDefaultInteractivityAllowed = true;
				}
			}
		}

		private IDataPoint GetNearestDataPoint4VerticalChart(double xValue, List<IDataPoint> dataPoints)
		{
			IDataPoint dataPoint = dataPoints[0];
			double num = Math.Abs(dataPoint.DpInfo.VisualPosition.X - xValue);
			for (int i = 1; i < dataPoints.Count; i++)
			{
				if (Math.Abs(dataPoints[i].DpInfo.VisualPosition.X - xValue) < num)
				{
					num = Math.Abs(dataPoints[i].DpInfo.VisualPosition.X - xValue);
					dataPoint = dataPoints[i];
				}
			}
			return dataPoint;
		}

		private IDataPoint GetNearestDataPoint4HorizontalChart(double yValue, List<IDataPoint> dataPoints)
		{
			IDataPoint dataPoint = dataPoints[0];
			double num = Math.Abs(dataPoint.DpInfo.VisualPosition.Y - yValue);
			for (int i = 1; i < dataPoints.Count; i++)
			{
				if (Math.Abs(dataPoints[i].DpInfo.VisualPosition.Y - yValue) < num)
				{
					num = Math.Abs(dataPoints[i].DpInfo.VisualPosition.Y - yValue);
					dataPoint = dataPoints[i];
				}
			}
			return dataPoint;
		}

		internal void AttachEvents2ColumnVisualFaces(VisifireControl control, FrameworkElement element, List<IDataPoint> dataPoints, ChartOrientationType chartOrientation)
		{
			element.MouseMove -= delegate(object sender, MouseEventArgs e)
			{
			};
			element.MouseLeave -= delegate(object sender, MouseEventArgs e)
			{
			};
			element.MouseMove += delegate(object sender, MouseEventArgs e)
			{
				Point position = e.GetPosition(element);
				IDataPoint dataPoint;
				if (chartOrientation == ChartOrientationType.Vertical)
				{
					dataPoint = this.GetNearestDataPoint4VerticalChart(position.X, dataPoints);
				}
				else
				{
					dataPoint = this.GetNearestDataPoint4HorizontalChart(position.Y, dataPoints);
				}
				control._toolTip.CallOutVisiblity = Visibility.Collapsed;
				if (dataPoint.ToolTipText == null)
				{
					control._toolTip.Text = "";
					control._toolTip.Hide();
					return;
				}
				control._toolTip.Text = dataPoint.ParseToolTipText(dataPoint.ToolTipText);
				if (control.ToolTipEnabled)
				{
					control._toolTip.Show();
				}
				(control as Chart).UpdateToolTipPosition(sender, e);
			};
			element.MouseLeave += delegate(object sender, MouseEventArgs e)
			{
				control._toolTip.Hide();
			};
		}

		internal void AttachEventsToolTipHref2DataSeries()
		{
			foreach (DataSeries current in this.Chart.InternalSeries)
			{
				RenderAs renderAs = current.RenderAs;
				if (renderAs <= RenderAs.Doughnut)
				{
					if (renderAs != RenderAs.Pie && renderAs != RenderAs.Doughnut)
					{
						continue;
					}
				}
				else
				{
					switch (renderAs)
					{
					case RenderAs.StackedArea:
					case RenderAs.StackedArea100:
					case RenderAs.StreamLineFunnel:
					case RenderAs.SectionFunnel:
						break;
					case RenderAs.Bubble:
					case RenderAs.Point:
						continue;
					default:
						if (renderAs != RenderAs.Pyramid)
						{
							continue;
						}
						break;
					}
				}
				this.Check4DefaultInteractivity(current);
				foreach (IDataPoint current2 in current.InternalDataPoints)
				{
					if (current2.Parent != null)
					{
						if (!current2.IsLightDataPoint)
						{
							DataPointHelper.AttachEvent2DataPointVisualFaces(current2, current2 as DataPoint);
						}
						current2.DpInfo.ParsedToolTipText = current2.TextParser(current2.ToolTipText);
						if (!current2.IsLightDataPoint)
						{
							(current2 as DataPoint).ParsedHref = current2.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(current2, VcProperties.Href));
						}
						if (current2.DpInfo.Faces != null)
						{
							if (this.Chart.View3D && (current.RenderAs == RenderAs.Pie || current.RenderAs == RenderAs.Doughnut || current.RenderAs == RenderAs.SectionFunnel || current.RenderAs == RenderAs.StreamLineFunnel || current.RenderAs == RenderAs.Pyramid))
							{
								current2.AttachToolTip(this.Chart, current2, current2.DpInfo.Faces.VisualComponents);
							}
							else if (current.RenderAs != RenderAs.StackedArea && current.RenderAs != RenderAs.StackedArea100)
							{
								current2.AttachToolTip(this.Chart, current2, current2.DpInfo.Faces.Visual);
							}
							if (current2.DpInfo.LabelVisual != null && (current.RenderAs == RenderAs.Pie || current.RenderAs == RenderAs.Doughnut))
							{
								current2.AttachToolTip(this.Chart, current2, current2.DpInfo.LabelVisual);
							}
						}
						if ((current.RenderAs == RenderAs.StackedArea || current.RenderAs == RenderAs.StackedArea100) && current2.DpInfo.Marker != null && !this.Chart.IndicatorEnabled)
						{
							current2.AttachToolTip(this.Chart, current2, current2.DpInfo.Marker.Visual);
						}
						DataPointHelper.SetHref2DataPointVisualFaces(current2);
						DataPointHelper.SetCursor2DataPointVisualFaces(current2);
					}
				}
				current.AttachEvent2DataSeriesVisualFaces();
				if ((current.RenderAs == RenderAs.StackedArea || current.RenderAs == RenderAs.StackedArea100) && current.Faces != null && !this.Chart.IndicatorEnabled)
				{
					current.AttachAreaToolTip(this.Chart, current.Faces.VisualComponents);
				}
			}
		}

		private Size CreateRegionsForVerticalCharts(double chartSize, Size NewSize, AxisRepresentations renderAxisType, bool isPartialUpdate)
		{
			double num = 0.0;
			double width = 0.0;
			if (double.IsNaN(NewSize.Height) || NewSize.Height <= 0.0 || double.IsNaN(NewSize.Width) || NewSize.Width <= 0.0 || double.IsNaN(chartSize) || chartSize <= 0.0)
			{
				return new Size(width, num);
			}
			if (this.Chart.View3D)
			{
				double plankOpacity = 0.3;
				if (this.Chart.Background != null && this.Chart.Background is SolidColorBrush && (this.Chart.Background as SolidColorBrush).Color == Colors.Black)
				{
					plankOpacity = 1.0;
				}
				this.DrawHorizontalPlank(this.PLANK_DEPTH, this.PLANK_THICKNESS, NewSize.Height, renderAxisType, isPartialUpdate);
				if (NewSize.Height - this.PLANK_DEPTH - this.PLANK_THICKNESS > 0.0)
				{
					this.DrawVerticalPlank(NewSize.Height - this.PLANK_DEPTH - this.PLANK_THICKNESS, this.PLANK_DEPTH, 0.25, plankOpacity, isPartialUpdate);
				}
				num = NewSize.Height - this.PLANK_OFFSET;
				width = chartSize - this.PLANK_DEPTH;
			}
			else
			{
				num = NewSize.Height;
				width = chartSize;
			}
			if (num <= 0.0 || chartSize <= 0.0)
			{
				return new Size(0.0, 0.0);
			}
			return new Size(width, num);
		}

		private Size CreateRegionsForHorizontalCharts(double chartSize, Size NewSize, AxisRepresentations renderAxisType, bool isPartialUpdate)
		{
			double num = 0.0;
			double num2 = 0.0;
			if (double.IsNaN(NewSize.Height) || NewSize.Height <= 0.0 || double.IsNaN(NewSize.Width) || NewSize.Width <= 0.0 || double.IsNaN(chartSize) || chartSize <= 0.0)
			{
				return new Size(num2, num);
			}
			if (this.Chart.View3D)
			{
				if (this._horizontalPlank != null && this._horizontalPlank.Visual.Parent != null)
				{
					this.PlottingCanvas.Children.Remove(this._horizontalPlank.Visual);
				}
				this.DrawVerticalPlank(this.PLANK_DEPTH, this.PLANK_THICKNESS, renderAxisType, isPartialUpdate);
				num = chartSize - this.PLANK_DEPTH;
				num2 = NewSize.Width - this.PLANK_OFFSET;
			}
			else
			{
				num = chartSize;
				num2 = NewSize.Width;
			}
			if (num <= 0.0 || num2 <= 0.0)
			{
				return new Size(0.0, 0.0);
			}
			return new Size(num2, num);
		}

		private Size CreateRegionsForChartsWithoutAxis(Size NewSize)
		{
			double height = 0.0;
			double width = 0.0;
			if (double.IsNaN(NewSize.Height) || NewSize.Height <= 0.0 || double.IsNaN(NewSize.Width) || NewSize.Width <= 0.0)
			{
				return new Size(width, height);
			}
			height = NewSize.Height;
			width = NewSize.Width;
			return new Size(width, height);
		}

		internal void AttachScrollBarOffsetChangedEventWithAxes()
		{
			if (this.AxisX != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				this.AxisX.ScrollBarOffsetChanged -= new ScrollEventHandler(this.AxesXScrollBarElement_Scroll);
				this.AxisX.ScrollBarOffsetChanged += new ScrollEventHandler(this.AxesXScrollBarElement_Scroll);
				this.AxisX.SetScrollBarValueFromOffset(this.AxisX.ScrollBarOffset, false);
			}
			if (this.AxisX2 != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
			{
				this.AxisX2.ScrollBarOffsetChanged -= new ScrollEventHandler(this.AxesXScrollBarElement_Scroll);
				this.AxisX2.ScrollBarOffsetChanged += new ScrollEventHandler(this.AxesXScrollBarElement_Scroll);
				this.AxisX2.SetScrollBarValueFromOffset(this.AxisX2.ScrollBarOffset, false);
			}
			if (this.AxisX != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				this.AxisX.ScrollBarOffsetChanged -= new ScrollEventHandler(this.AxesXScrollBarElement_Scroll);
				this.AxisX.ScrollBarOffsetChanged += new ScrollEventHandler(this.AxesXScrollBarElement_Scroll);
				this.AxisX.SetScrollBarValueFromOffset(this.AxisX.ScrollBarOffset, false);
			}
			if (this.AxisX2 != null && this.PlotDetails.ChartOrientation == ChartOrientationType.Horizontal)
			{
				this.AxisX2.ScrollBarOffsetChanged -= new ScrollEventHandler(this.AxesXScrollBarElement_Scroll);
				this.AxisX2.ScrollBarOffsetChanged += new ScrollEventHandler(this.AxesXScrollBarElement_Scroll);
				this.AxisX2.SetScrollBarValueFromOffset(this.AxisX2.ScrollBarOffset, false);
			}
		}

		private void PlottingCanvas_Loaded(object sender, RoutedEventArgs e)
		{
			this.PlottingCanvas.Loaded -= new RoutedEventHandler(this.PlottingCanvas_Loaded);
			this.Chart.RENDER_LOCK = false;
			this.AttachScrollBarOffsetChangedEventWithAxes();
			this.Animate();
			this.Chart._internalAnimationEnabled = false;
			this._isFirstTimeRender = false;
			this.Chart.FireRenderedEvent();
			if (this.Chart._renderLapsedCounter > 0)
			{
				this.Chart.InvokeRender();
			}
		}

		private void AddTitle(Chart chart, Title title, Panel panel, double width, double height)
		{
			double internalFontSize = title.InternalFontSize;
			title.Chart = chart;
			while (true)
			{
				title.CreateVisualObject(new ElementData
				{
					Element = title
				}, chart.FlowDirection);
				Size size = Graphics.CalculateVisualSize(title.Visual);
				if (title.InternalVerticalAlignment == VerticalAlignment.Top || title.InternalVerticalAlignment == VerticalAlignment.Bottom || (title.InternalVerticalAlignment == VerticalAlignment.Center && title.InternalHorizontalAlignment == HorizontalAlignment.Center))
				{
					if (size.Width <= width || chart.ActualWidth - width >= width || title.InternalFontSize == 1.0)
					{
						break;
					}
					title.IsNotificationEnable = false;
					title.InternalFontSize -= 1.0;
					title.IsNotificationEnable = true;
				}
				else
				{
					if ((size.Height < height && title.Height < height) || title.InternalFontSize == 1.0)
					{
						break;
					}
					title.IsNotificationEnable = false;
					title.InternalFontSize -= 1.0;
					title.IsNotificationEnable = true;
				}
			}
			title.IsNotificationEnable = false;
			title.InternalFontSize = internalFontSize;
			title.IsNotificationEnable = true;
			panel.Children.Add(title.Visual);
		}

		internal MarkerTypes RenderAsToLegendMarkerType(RenderAs renderAs, IDataPoint dataPoint)
		{
			DataSeries parent = dataPoint.Parent;
			if (parent.RenderAs == RenderAs.QuickLine)
			{
				return MarkerTypes.Line;
			}
			if (!RenderHelper.IsLineCType(parent))
			{
				return this.RenderAsToLegendMarkerType(renderAs, parent);
			}
			if (!(bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
			{
				return MarkerTypes.Line;
			}
			return (MarkerTypes)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerType);
		}

		internal MarkerTypes RenderAsToLegendMarkerType(RenderAs renderAs, DataSeries dataSeries)
		{
			switch (renderAs)
			{
			case RenderAs.Line:
			case RenderAs.Stock:
			case RenderAs.CandleStick:
			case RenderAs.StepLine:
			case RenderAs.Spline:
			case RenderAs.QuickLine:
				return dataSeries.MarkerType;
			case RenderAs.Pie:
			case RenderAs.Doughnut:
			case RenderAs.Bubble:
				return MarkerTypes.Circle;
			case RenderAs.Area:
			case RenderAs.StackedArea:
			case RenderAs.StackedArea100:
				return MarkerTypes.Triangle;
			case RenderAs.Point:
				return dataSeries.MarkerType;
			}
			return MarkerTypes.Square;
		}

		internal void ClearInstanceRefs()
		{
			if (this.Storyboard4PlankGridLines != null)
			{
				this.Storyboard4PlankGridLines.FillBehavior = FillBehavior.Stop;
				this.Storyboard4PlankGridLines.Stop();
				this.Storyboard4PlankGridLines = null;
			}
		}
	}
}
