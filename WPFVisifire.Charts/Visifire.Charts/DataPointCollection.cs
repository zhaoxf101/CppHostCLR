using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Visifire.Charts
{
	public class DataPointCollection : ObservableCollection<IDataPoint>
	{
		public DataPointCollection()
		{
		}

		public DataPointCollection(List<IDataPoint> list) : base(list)
		{
		}

		public DataPointCollection(IEnumerable<IDataPoint> list) : base(list)
		{
		}
	}
}
