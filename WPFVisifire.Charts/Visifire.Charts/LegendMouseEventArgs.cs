using System;
using System.Windows;
using System.Windows.Input;

namespace Visifire.Charts
{
	public sealed class LegendMouseEventArgs : EventArgs
	{
		public MouseEventArgs MouseEventArgs
		{
			get;
			internal set;
		}

		public IDataPoint DataPoint
		{
			get;
			internal set;
		}

		public DataSeries DataSeries
		{
			get;
			internal set;
		}

		public LegendMouseEventArgs(MouseEventArgs e)
		{
			this.DataPoint = null;
			this.DataSeries = null;
			this.MouseEventArgs = e;
			if (e != null)
			{
				ElementData elementData = (e.OriginalSource as FrameworkElement).Tag as ElementData;
				if (elementData != null && elementData.Element != null)
				{
					Type type = elementData.Element.GetType();
					if (type.Equals(typeof(DataSeries)) || type.IsSubclassOf(typeof(DataSeries)))
					{
						this.DataSeries = (DataSeries)elementData.Element;
						return;
					}
					if (type.Equals(typeof(DataPoint)) || type.IsSubclassOf(typeof(DataPoint)))
					{
						this.DataPoint = (IDataPoint)elementData.Element;
						return;
					}
					if (type.Equals(typeof(LightDataPoint)) || type.IsSubclassOf(typeof(LightDataPoint)))
					{
						this.DataPoint = (IDataPoint)elementData.Element;
					}
				}
			}
		}
	}
}
