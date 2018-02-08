using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Visifire.Charts
{
	public class ColorSet
	{
		private int colorSetIndex;

		public string Id
		{
			get;
			set;
		}

		public Collection<Brush> Brushes
		{
			get;
			set;
		}

		public ColorSet()
		{
			this.Brushes = new Collection<Brush>();
		}

		public Brush GetNewColorFromColorSet()
		{
			if (this.colorSetIndex == this.Brushes.Count)
			{
				this.colorSetIndex = 0;
			}
			return this.Brushes[this.colorSetIndex++];
		}

		internal void ResetIndex()
		{
			this.colorSetIndex = 0;
		}
	}
}
