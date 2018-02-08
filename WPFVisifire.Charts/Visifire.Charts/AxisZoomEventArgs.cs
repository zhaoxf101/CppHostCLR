using System;

namespace Visifire.Charts
{
	public class AxisZoomEventArgs : EventArgs
	{
		public EventArgs ZoomEventArgs
		{
			get;
			internal set;
		}

		public object MinValue
		{
			get;
			internal set;
		}

		public object MaxValue
		{
			get;
			internal set;
		}

		public AxisZoomEventArgs(EventArgs e)
		{
			this.MinValue = null;
			this.MaxValue = null;
			this.ZoomEventArgs = e;
		}
	}
}
