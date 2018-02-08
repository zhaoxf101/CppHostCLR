using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Visifire.Commons;
using Visifire.Commons.Controls;

namespace Visifire.Charts
{
	public class DpFunc
	{
		private IDataPoint dp;

		internal WeakEventListener<IDataPoint, object, PropertyChangedEventArgs> _weakEventListener;

		public DpFunc(IDataPoint dataPoint)
		{
			this.dp = dataPoint;
		}

		public DpFunc()
		{
		}

		internal void OnHrefPropertyChanged(string newValue)
		{
			if (this.dp.Chart != null && this.dp.Parent != null && !this.dp.IsLightDataPoint)
			{
				(this.dp as DataPoint).ParsedHref = DataPointHelper.TextParser(this.dp, newValue);
			}
		}

		internal void AttachHref(VisifireControl control, FrameworkElement visualElement)
		{
			if (this.dp != null && this.dp.GetType().Equals(typeof(DataPoint)))
			{
				(this.dp as DataPoint).AttachHref(control, visualElement, (this.dp as DataPoint).ParsedHref, (this.dp as DataPoint).HrefTarget.Value);
			}
		}

		internal void iNotifyPropertyChanged_PropertyChanged(object dataSource, PropertyChangedEventArgs e)
		{
			DataMapping dataMapping;
			try
			{
				dataMapping = (from item in this.dp.Parent.DataMappings
				where item.Path == e.PropertyName
				select item).FirstOrDefault<DataMapping>();
			}
			catch
			{
				return;
			}
			if (dataMapping != null)
			{
				DataPointHelper.UpdateBindProperty(this.dp, dataMapping, dataSource);
			}
		}

		internal void DetachEventFromWeakEventListner()
		{
			if (this._weakEventListener != null)
			{
				this._weakEventListener.Detach();
				this._weakEventListener = null;
			}
		}

		internal void Visual_ExplodeUnExplode(object sender, MouseButtonEventArgs e)
		{
			DataPointHelper.InteractiveAnimation(this.dp, false, true);
		}

		internal void ExplodeAnimation_Completed(object sender, EventArgs e)
		{
			this.dp.DpInfo.InteractiveExplodeState = true;
			this.dp.DpInfo.InterativityAnimationState = false;
			this.dp.Chart._rootElement.IsHitTestVisible = true;
			if (this.dp.Parent.RenderAs == RenderAs.SectionFunnel || this.dp.Parent.RenderAs == RenderAs.StreamLineFunnel || this.dp.Parent.RenderAs == RenderAs.Pyramid)
			{
				TriangularChartSliceParms[] array = this.dp.Parent.VisualParams as TriangularChartSliceParms[];
				if (array != null && this.dp.DpInfo.Faces != null && this.dp.DpInfo.Faces.Visual != null)
				{
					double height = (this.dp.DpInfo.Faces.Visual.Parent as FrameworkElement).Height;
					FunnelChart.ArrangeLabels(array, double.NaN, height);
				}
			}
		}

		internal void UnExplodeAnimation_Completed(object sender, EventArgs e)
		{
			this.dp.DpInfo.InteractiveExplodeState = false;
			this.dp.DpInfo.InterativityAnimationState = false;
		}
	}
}
