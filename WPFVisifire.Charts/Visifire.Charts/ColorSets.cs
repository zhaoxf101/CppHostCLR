using System;
using System.Collections.Generic;

namespace Visifire.Charts
{
	public class ColorSets : List<ColorSet>
	{
		public ColorSet GetColorSetByName(string ColorSetId)
		{
			foreach (ColorSet current in this)
			{
				if (current.Id == ColorSetId)
				{
					current.ResetIndex();
					return current;
				}
			}
			return null;
		}
	}
}
