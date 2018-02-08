using System;
using System.Collections.Generic;
using System.Linq;

namespace Visifire.Charts
{
	public class Theme
	{
		public string Id
		{
			get;
			set;
		}

		public string DefaultColorSet
		{
			get;
			set;
		}

		public ColorSet ColorSet
		{
			get;
			set;
		}

		public List<ElementStyle> Styles
		{
			get;
			set;
		}

		public Theme()
		{
			this.ColorSet = new ColorSet();
			this.Styles = new List<ElementStyle>();
		}

		public object GetValueFromTheme(Elements Element, string PropertyName)
		{
			IEnumerable<ElementStyle> source = from style in this.Styles
			where style.Element == Element
			select style;
			if (source.Count<ElementStyle>() <= 0)
			{
				return null;
			}
			ElementStyle elementStyle = source.First<ElementStyle>();
			IEnumerable<PropertyValuePair> source2 = from propertyValuePair in elementStyle.PropertyValueCollection
			where propertyValuePair.PropertyName == PropertyName
			select propertyValuePair;
			if (source2.Count<PropertyValuePair>() > 0)
			{
				return source2.First<PropertyValuePair>().Value;
			}
			return null;
		}
	}
}
