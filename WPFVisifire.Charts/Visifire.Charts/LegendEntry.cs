using System;
using System.Collections.Generic;
using System.Windows;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class LegendEntry
	{
		public bool IsCompleteLine;

		public Marker Marker;

		public List<string> Labels;

		public List<HorizontalAlignment> XAlignments;

		public LegendEntry(Marker marker, List<string> labels, List<HorizontalAlignment> xAlignments)
		{
			this.Marker = marker;
			this.Labels = labels;
			this.XAlignments = xAlignments;
		}

		public LegendEntry()
		{
		}
	}
}
