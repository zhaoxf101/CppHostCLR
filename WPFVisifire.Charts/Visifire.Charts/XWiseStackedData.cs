using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Visifire.Charts
{
	internal class XWiseStackedData
	{
		private double _positiveYValueSum;

		private double _negativeYValueSum;

		private bool _isUpdated;

		public ObservableCollection<IDataPoint> Positive
		{
			get;
			set;
		}

		public ObservableCollection<IDataPoint> Negative
		{
			get;
			set;
		}

		public double PositiveYValueSum
		{
			get
			{
				if (!this._isUpdated)
				{
					this.Update();
				}
				return this._positiveYValueSum;
			}
		}

		public double NegativeYValueSum
		{
			get
			{
				if (!this._isUpdated)
				{
					this.Update();
				}
				return this._negativeYValueSum;
			}
		}

		public double AbsoluteYValueSum
		{
			get
			{
				return Math.Abs(this.NegativeYValueSum) + Math.Abs(this.PositiveYValueSum);
			}
		}

		public XWiseStackedData()
		{
			this.Positive = new ObservableCollection<IDataPoint>();
			this.Negative = new ObservableCollection<IDataPoint>();
			this.Positive.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Positive_CollectionChanged);
			this.Negative.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Negative_CollectionChanged);
			this._isUpdated = false;
		}

		private void Positive_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this._isUpdated = false;
		}

		private void Negative_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this._isUpdated = false;
		}

		private void Update()
		{
			this._isUpdated = true;
			this._positiveYValueSum = (from dataPoint in this.Positive
			where !double.IsNaN(dataPoint.YValue)
			select dataPoint.YValue).Sum();
			this._negativeYValueSum = (from dataPoint in this.Negative
			where !double.IsNaN(dataPoint.YValue)
			select dataPoint.YValue).Sum();
		}
	}
}
