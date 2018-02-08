using System;
using System.Collections.Generic;
using System.Windows;
using Visifire.Commons;

namespace Visifire.Charts
{
	public interface IToolTip
	{
		string ToolTipText
		{
			get;
			set;
		}

		void AttachToolTip(VisifireControl control, IToolTip element, List<FrameworkElement> visualElements);

		void AttachToolTip(VisifireControl control, IToolTip element, FrameworkElement visualObject);

		string ParseToolTipText(string unParsed);

		void OnToolTipTextPropertyChanged(string newValue);

		void DetachToolTip(FrameworkElement visualObject);
	}
}
