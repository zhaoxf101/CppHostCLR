using System;
using System.Collections.Generic;

namespace Visifire.Commons
{
	public class PointComparer : IEqualityComparer<DataSamplingHelper.Point>
	{
		public bool Equals(DataSamplingHelper.Point x, DataSamplingHelper.Point y)
		{
			return object.ReferenceEquals(x, y) || (!object.ReferenceEquals(x, null) && !object.ReferenceEquals(y, null) && x.XValue == y.XValue && x.YValues[0] == y.YValues[0]);
		}

		public int GetHashCode(DataSamplingHelper.Point point)
		{
			if (object.ReferenceEquals(point, null))
			{
				return 0;
			}
			int hashCode = point.XValue.GetHashCode();
			int hashCode2 = point.YValues[0].GetHashCode();
			return hashCode ^ hashCode2;
		}
	}
}
