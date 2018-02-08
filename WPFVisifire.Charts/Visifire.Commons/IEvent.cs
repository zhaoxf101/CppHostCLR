using System;
using System.Windows;

namespace Visifire.Commons
{
	public interface IEvent
	{
		void AttachEvents2Visual(VisifireElement obj, FrameworkElement visual);

		void AttachEvents2Visual(VisifireElement chartElement, IElement senderElement, FrameworkElement visual);
	}
}
