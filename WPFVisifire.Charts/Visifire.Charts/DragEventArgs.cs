using System;
using System.Windows.Controls;

namespace Visifire.Charts
{
	public class DragEventArgs : EventArgs
	{
		public double DistanceTraveled;

		public Orientation Orientataion;

		public double GetOffset(double totalWidth)
		{
			return this.DistanceTraveled / totalWidth;
		}
	}
}
