using System;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class PartialUpdateConfiguration
	{
		internal IElement Sender;

		internal ElementTypes ElementType;

		internal VcProperties Property;

		internal object OldValue;

		internal object NewValue;

		internal bool IsUpdateLists;

		internal bool IsCalculatePlotDetails;

		internal bool IsUpdateAxis;

		internal AxisRepresentations RenderAxisType;

		internal bool IsPartialUpdate;
	}
}
