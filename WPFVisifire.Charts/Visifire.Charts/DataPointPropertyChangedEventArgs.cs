using System;
using System.Windows;

namespace Visifire.Charts
{
	internal sealed class DataPointPropertyChangedEventArgs : EventArgs
	{
		public object NewValue;

		public object OldValue;

		public VcProperties Property;

		public DataPointPropertyChangedEventArgs(DependencyPropertyChangedEventArgs e, VcProperties property)
		{
			this.NewValue = e.NewValue;
			this.OldValue = e.OldValue;
			this.Property = property;
		}

		public DataPointPropertyChangedEventArgs(object oldValue, object newValue, VcProperties property)
		{
			this.OldValue = oldValue;
			this.NewValue = newValue;
			this.Property = property;
		}
	}
}
