using System;
using System.Windows;

namespace Visifire.Charts
{
	internal class PieDoughnut2DPoints : PieDoughnutPoints
	{
		public Point Center
		{
			get;
			set;
		}

		public Point InnerArcStart
		{
			get;
			set;
		}

		public Point InnerArcMid
		{
			get;
			set;
		}

		public Point InnerArcEnd
		{
			get;
			set;
		}

		public Point OuterArcStart
		{
			get;
			set;
		}

		public Point OuterArcMid
		{
			get;
			set;
		}

		public Point OuterArcEnd
		{
			get;
			set;
		}
	}
}
