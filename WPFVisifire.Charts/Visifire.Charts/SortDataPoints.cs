using System;
using System.Collections.Generic;

namespace Visifire.Charts
{
	public class SortDataPoints
	{
		public List<IDataPoint> Positive
		{
			get;
			private set;
		}

		public List<IDataPoint> Negative
		{
			get;
			private set;
		}

		public SortDataPoints()
		{
		}

		public SortDataPoints(List<IDataPoint> positive, List<IDataPoint> negative)
		{
			this.Positive = positive;
			this.Negative = negative;
		}
	}
}
