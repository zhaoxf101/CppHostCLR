using System;
using System.Collections.Generic;

namespace Visifire.Charts
{
	public class ElementStyle
	{
		public Elements Element
		{
			get;
			set;
		}

		public List<PropertyValuePair> PropertyValueCollection
		{
			get;
			set;
		}

		public ElementStyle()
		{
			this.PropertyValueCollection = new List<PropertyValuePair>();
		}
	}
}
