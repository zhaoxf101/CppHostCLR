using System;
using System.Collections.Generic;
using System.Windows;

namespace Visifire.Commons
{
	public interface IHref
	{
		void AttachHref(VisifireControl control, List<FrameworkElement> visualElements, string href, HrefTargets hrefTarget);

		void AttachHref(VisifireControl control, FrameworkElement visualElement, string href, HrefTargets hrefTarget);
	}
}
