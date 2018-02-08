using System;
using System.Windows.Controls.Primitives;

namespace Visifire.Charts
{
	public class AxisScrollEventArgs : EventArgs
	{
		public ScrollEventArgs ScrollEventArgs
		{
			get;
			internal set;
		}

		public AxisScrollEventArgs(ScrollEventArgs e)
		{
			this.ScrollEventArgs = e;
		}
	}
}
