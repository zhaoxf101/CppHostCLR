using System;
using System.Windows;

namespace Visifire.Charts
{
	internal abstract class PieDoughnutPoints
	{
		public Point LabelLineStartPoint
		{
			get;
			set;
		}

		public Point LabelLineMidPoint
		{
			get;
			set;
		}

		public Point LabelLineEndPoint
		{
			get;
			set;
		}

		public Point LabelPosition
		{
			get;
			set;
		}
	}
}
