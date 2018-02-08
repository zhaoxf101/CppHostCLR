using System;
using System.ComponentModel;
using System.Windows.Media;
using Visifire.Commons;

namespace Visifire.Charts
{
	public interface IDataPoint : IEvent, IToolTip, IElement, INotifyPropertyChanged
	{
		VisifireControl Chart
		{
			get;
			set;
		}

		bool IsLightDataPoint
		{
			get;
			set;
		}

		object XValue
		{
			get;
			set;
		}

		double YValue
		{
			get;
			set;
		}

		object DataContext
		{
			get;
			set;
		}

		double[] YValues
		{
			get;
			set;
		}

		double ZValue
		{
			get;
			set;
		}

		string AxisXLabel
		{
			get;
			set;
		}

		Brush Color
		{
			get;
			set;
		}

		bool? Enabled
		{
			get;
			set;
		}

		bool? Exploded
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}

		DataSeries Parent
		{
			get;
		}

		DpInfo DpInfo
		{
			get;
			set;
		}

		DpFunc DpFunc
		{
			get;
			set;
		}

		void FirePropertyChanged(VcProperties property);

		string TextParser(string str);
	}
}
