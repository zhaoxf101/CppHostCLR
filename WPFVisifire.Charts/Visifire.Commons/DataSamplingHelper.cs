using System;
using System.Collections.Generic;
using System.Linq;
using Visifire.Charts;

namespace Visifire.Commons
{
	public static class DataSamplingHelper
	{
		public class Point
		{
			public double[] YValues;

			public double XValue;

			public Point(double xValue, double[] yValues)
			{
				this.XValue = xValue;
				this.YValues = yValues;
			}
		}

		private class PointGroup
		{
			public List<DataSamplingHelper.Point> dataPoints = new List<DataSamplingHelper.Point>();

			public DataSamplingHelper.Point GetAvgPoint()
			{
				if (this.dataPoints.Count > 0)
				{
					List<double> list = new List<double>();
					int num = this.dataPoints[0].YValues.Length;
					for (int i = 0; i < num; i++)
					{
						list.Add(this.GetAvgYValue(i));
					}
					return new DataSamplingHelper.Point(this.GetAvgXValue(), list.ToArray());
				}
				return null;
			}

			private double GetAvgYValue(int index)
			{
				return (from dataPoint in this.dataPoints
				select dataPoint.YValues[index]).Sum() / (double)this.dataPoints.Count;
			}

			private double GetAvgXValue()
			{
				double num = (from dataPoint in this.dataPoints
				select dataPoint.XValue).Max();
				double num2 = (from dataPoint in this.dataPoints
				select dataPoint.XValue).Min();
				return (num2 + num) / 2.0;
			}
		}

		public static List<List<DataSamplingHelper.Point>> CreateGroupOfBrokenLineSegment(List<DataSamplingHelper.Point> actualList, ref List<DataSamplingHelper.Point> brokenPointsInfo)
		{
			List<DataSamplingHelper.Point> list = new List<DataSamplingHelper.Point>();
			List<List<DataSamplingHelper.Point>> list2 = new List<List<DataSamplingHelper.Point>>();
			list2.Add(list);
			foreach (DataSamplingHelper.Point current in actualList)
			{
				if (current.YValues.Contains(double.NaN))
				{
					list = new List<DataSamplingHelper.Point>();
					list2.Add(list);
					brokenPointsInfo.Add(current);
				}
				else
				{
					list.Add(current);
				}
			}
			return list2;
		}

		public static List<DataSamplingHelper.Point> Filter(List<DataSamplingHelper.Point> actualList, double minXPosition, double plotXValueDistance, int DataPointsLimit, SamplingFunction samplingFunction)
		{
			if (actualList.Count <= DataPointsLimit || DataPointsLimit == 0)
			{
				return actualList;
			}
			List<DataSamplingHelper.Point> list = actualList.ToList<DataSamplingHelper.Point>();
			if (samplingFunction == SamplingFunction.Average)
			{
				list = DataSamplingHelper.GroupAverageFilter(list, minXPosition, plotXValueDistance, DataPointsLimit);
			}
			return list;
		}

		private static List<DataSamplingHelper.PointGroup> CreatePointGroups(List<DataSamplingHelper.Point> actualList, double minXPosition, double plotXValueDistance, double threshold)
		{
			List<DataSamplingHelper.PointGroup> list = new List<DataSamplingHelper.PointGroup>();
			double num = 0.0;
			for (double num2 = minXPosition; num2 <= minXPosition + plotXValueDistance; num2 = num)
			{
				num = num2 + threshold;
				DataSamplingHelper.PointGroup pointGroup = new DataSamplingHelper.PointGroup();
				foreach (DataSamplingHelper.Point current in actualList)
				{
					double xValue = current.XValue;
					if (xValue >= num2 && xValue <= num)
					{
						pointGroup.dataPoints.Add(current);
					}
					else if (xValue > num)
					{
						break;
					}
				}
				foreach (DataSamplingHelper.Point current2 in pointGroup.dataPoints)
				{
					if (current2.XValue < num)
					{
						actualList.Remove(current2);
					}
				}
				list.Add(pointGroup);
			}
			return list;
		}

		private static List<DataSamplingHelper.Point> GroupAverageFilter(List<DataSamplingHelper.Point> actualList, double minXPosition, double plotXValueDistance, int DataPointsLimit)
		{
			double threshold = Math.Abs(plotXValueDistance / (double)DataPointsLimit);
			List<DataSamplingHelper.Point> list = new List<DataSamplingHelper.Point>();
			List<DataSamplingHelper.Point> list2 = new List<DataSamplingHelper.Point>();
			List<DataSamplingHelper.Point> list3 = new List<DataSamplingHelper.Point>();
			List<DataSamplingHelper.Point> list4 = new List<DataSamplingHelper.Point>();
			List<List<DataSamplingHelper.Point>> list5 = DataSamplingHelper.CreateGroupOfBrokenLineSegment(actualList, ref list2);
			if (list5.Count > 0)
			{
				foreach (List<DataSamplingHelper.Point> current in list5)
				{
					list4 = list4.Union(current).ToList<DataSamplingHelper.Point>();
				}
				List<DataSamplingHelper.PointGroup> list6 = DataSamplingHelper.CreatePointGroups(list4, minXPosition, plotXValueDistance, threshold);
				List<DataSamplingHelper.Point> list7 = new List<DataSamplingHelper.Point>();
				foreach (DataSamplingHelper.PointGroup current2 in list6)
				{
					if (current2.dataPoints.Count > 0)
					{
						DataSamplingHelper.Point avgPoint = current2.GetAvgPoint();
						if (avgPoint != null)
						{
							list7.Add(avgPoint);
						}
					}
				}
				list = list7;
			}
			foreach (DataSamplingHelper.Point current3 in list2)
			{
				list.Add(current3);
			}
			return (from dp in list
			orderby dp.XValue
			select dp).ToList<DataSamplingHelper.Point>();
		}
	}
}
