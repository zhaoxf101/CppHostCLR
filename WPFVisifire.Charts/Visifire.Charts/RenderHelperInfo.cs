using System;
using Visifire.Commons;

namespace Visifire.Charts
{
	internal class RenderHelperInfo
	{
		internal Chart Chart;

		internal IElement Sender;

		internal ElementTypes ElementType;

		internal VcProperties Property;

		internal object NewValue;

		internal bool IsAxisChanged;

		internal bool IsPartialUpdate;

		internal bool IsThroughRasterRender;

		internal bool IsZoomingOnProgress;

		public RenderHelperInfo(Chart chart, VcProperties property, object newValue, bool partialUpdate, bool isThroughRasterRender, bool isZoomingOnProgress)
		{
			this.Chart = chart;
			this.Property = property;
			this.NewValue = newValue;
			this.IsPartialUpdate = partialUpdate;
			this.IsThroughRasterRender = isThroughRasterRender;
			this.IsZoomingOnProgress = isZoomingOnProgress;
		}

		public RenderHelperInfo(Chart chart, IElement sender, ElementTypes elementType, VcProperties property, object newValue, bool isAXisChanged)
		{
			this.Chart = chart;
			this.Sender = sender;
			this.ElementType = elementType;
			this.Property = property;
			this.NewValue = newValue;
			this.IsAxisChanged = isAXisChanged;
		}
	}
}
