using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Visifire.Charts
{
	internal class ZoomingPanningEventHandler : EventArgs
	{
		private ChartOrientationType _orientataion;

		private double _startPosition;

		private double _currentPosition;

		private bool _isDragging;

		private Chart _chart;

		public event EventHandler<DragEventArgs> OnDrag;

		public event EventHandler<DragEventArgs> DragStart;

		public void Attach(Chart chart, FrameworkElement visual, ChartOrientationType orientataion)
		{
			this._orientataion = orientataion;
			this._chart = chart;
			if (chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis && chart.PlotDetails.ChartOrientation != ChartOrientationType.Circular && visual != null)
			{
				if (chart.ZoomingEnabled && (chart.ZoomingMode == ZoomingMode.MouseDrag || chart.ZoomingMode == ZoomingMode.MouseDragAndWheel) && chart.PanningMode == PanningMode.ScrollBarAndMouseDrag)
				{
					throw new Exception("Wrong property value combination. PanningMode cannot be set as ZoomBarAndMouseDrag when ZoomingMode is set as MouseDrag or MouseDragAndWheel. This restriction is present in order to avoid conflict between ‘zooming by selecting a rectangular area’ and ‘panning using mouse drag’.");
				}
				if (chart.PanningMode == PanningMode.ScrollBarAndMouseDrag)
				{
					visual.MouseLeftButtonUp += new MouseButtonEventHandler(this.Visual_MouseLeftButtonUp);
					visual.MouseMove += new MouseEventHandler(this.Visual_MouseMove);
					visual.MouseLeftButtonDown += new MouseButtonEventHandler(this.Visual_MouseLeftButtonDown);
					visual.LostMouseCapture += new MouseEventHandler(this.visual_LostMouseCapture);
				}
				if (chart.ZoomingEnabled && (chart.ZoomingMode == ZoomingMode.MouseWheel || chart.ZoomingMode == ZoomingMode.MouseDragAndWheel))
				{
					visual.MouseWheel += new MouseWheelEventHandler(this.visual_MouseWheel);
				}
			}
		}

		private void visual_LostMouseCapture(object sender, MouseEventArgs e)
		{
			this.CancelDragging(sender, e, false);
		}

		private void Visual_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this._isDragging = true;
			FrameworkElement frameworkElement = sender as FrameworkElement;
			this._chart.CustomCursor.Visibility = Visibility.Visible;
			this._chart.SetCustomCursorPosition(e);
			if (this._orientataion == ChartOrientationType.Vertical)
			{
				frameworkElement.Cursor = Cursors.None;
			}
			else if (this._orientataion == ChartOrientationType.Horizontal)
			{
				frameworkElement.Cursor = Cursors.None;
			}
			if (this._orientataion == ChartOrientationType.Vertical)
			{
				this._startPosition = e.GetPosition(frameworkElement).X;
			}
			else if (this._orientataion == ChartOrientationType.Horizontal)
			{
				this._startPosition = e.GetPosition(frameworkElement).Y;
			}
			this._currentPosition = this._startPosition;
			if (this.DragStart != null)
			{
				this.DragStart(frameworkElement, new DragEventArgs
				{
					DistanceTraveled = 0.0
				});
			}
			if (!e.Handled)
			{
				e.Handled = true;
				frameworkElement.CaptureMouse();
			}
		}

		private void Visual_MouseMove(object sender, MouseEventArgs e)
		{
			this.FireDragEvent(sender, e);
		}

		private void FireDragEvent(object sender, MouseEventArgs e)
		{
			if (this._isDragging)
			{
				UIElement uIElement = sender as UIElement;
				double num = 0.0;
				if (this._orientataion == ChartOrientationType.Vertical)
				{
					num = e.GetPosition(uIElement).X;
				}
				else if (this._orientataion == ChartOrientationType.Horizontal)
				{
					num = e.GetPosition(uIElement).Y;
				}
				if (this._currentPosition != num)
				{
					this._currentPosition = num;
					if (this.OnDrag != null)
					{
						this.OnDrag(uIElement, new DragEventArgs
						{
							DistanceTraveled = this._currentPosition - this._startPosition,
							Orientataion = ((this._orientataion == ChartOrientationType.Vertical) ? Orientation.Horizontal : Orientation.Vertical)
						});
					}
				}
				this._chart.SetCustomCursorPosition(e);
			}
		}

		private void CancelDragging(object sender, MouseEventArgs e, bool removeMouseCapture)
		{
			FrameworkElement frameworkElement = sender as FrameworkElement;
			frameworkElement.Cursor = this._chart.PlotArea.Cursor;
			this._chart.CustomCursor.Visibility = Visibility.Collapsed;
			this._isDragging = false;
			if (removeMouseCapture)
			{
				frameworkElement.ReleaseMouseCapture();
			}
		}

		private void Visual_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.FireDragEvent(sender, e);
			this.CancelDragging(sender, e, true);
		}

		public void DetachEvents(UIElement visual)
		{
			if (visual != null)
			{
				visual.MouseLeftButtonUp -= new MouseButtonEventHandler(this.Visual_MouseLeftButtonUp);
				visual.MouseMove -= new MouseEventHandler(this.Visual_MouseMove);
				visual.MouseLeftButtonDown -= new MouseButtonEventHandler(this.Visual_MouseLeftButtonDown);
				visual.LostMouseCapture -= new MouseEventHandler(this.visual_LostMouseCapture);
				visual.MouseWheel -= new MouseWheelEventHandler(this.visual_MouseWheel);
			}
		}

		private void visual_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (this._chart.ZoomingEnabled)
			{
				Axis axisX = this._chart.ChartArea.AxisX;
				Math.Sign(e.Delta);
				double num = (double)e.Delta;
				double num2 = RenderHelper.CalculateInternalXValueFromPixelPos(this._chart, axisX, e);
				double num3 = RenderHelper.XValueToInternalXValue(axisX, axisX.ViewMinimum);
				double num4 = RenderHelper.XValueToInternalXValue(axisX, axisX.ViewMaximum);
				if (this._chart.PlotDetails.ListOfAllDataPoints.Count == 0)
				{
					return;
				}
				double num5;
				double num6;
				if (!axisX.Logarithmic)
				{
					num5 = (from dataPoint in this._chart.PlotDetails.ListOfAllDataPoints
					select dataPoint.DpInfo.ActualNumericXValue).Min();
					num6 = (from dataPoint in this._chart.PlotDetails.ListOfAllDataPoints
					select dataPoint.DpInfo.ActualNumericXValue).Max();
				}
				else
				{
					num5 = (from dataPoint in this._chart.PlotDetails.ListOfAllDataPoints
					select dataPoint.DpInfo.InternalXValue).Min();
					num6 = (from dataPoint in this._chart.PlotDetails.ListOfAllDataPoints
					select dataPoint.DpInfo.InternalXValue).Max();
				}
				double num7 = num6 - num5;
				if (num7 == 0.0)
				{
					num7 = 1.0;
				}
				double num8 = (num4 - num3) / num7;
				double num9;
				if (this._chart.PlotDetails.ChartOrientation == ChartOrientationType.Vertical)
				{
					num9 = axisX.Width;
				}
				else
				{
					num9 = axisX.Height;
				}
				double num10 = num8 / num9;
				num *= num10;
				double num11 = (num2 - num3) / (num4 - num3) * num;
				double num12 = (num4 - num2) / (num4 - num3) * num;
				double num13 = num7 / 100.0 * 40.0;
				double num14 = num3 + num13 * num11;
				double num15 = num4 - num13 * num12;
				if (num14 > num15)
				{
					num14 = num3 + (num4 - num3) / 2.0;
					num15 = num4 - (num4 - num3) / 2.0;
				}
				object obj = axisX.InternalXValue2XValue(num14);
				object obj2 = axisX.InternalXValue2XValue(num15);
				ZoomingAction action = (num < 0.0) ? ZoomingAction.Out : ZoomingAction.In;
				axisX.Zoom(obj, obj2, action);
				if (!obj.Equals(obj2))
				{
					axisX._zoomState.MinXValue = obj;
					axisX._zoomState.MaxXValue = obj2;
					axisX.FireZoomEvent(axisX._zoomState, null);
				}
			}
		}
	}
}
