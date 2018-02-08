using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class DpInfo
	{
		private IDataPoint dp;

		private ChartValueTypes _xValueType;

		internal double _actualNumericXValue = double.NaN;

		internal Path LabelLine;

		private bool _lightWeight = true;

		internal ChartValueTypes XValueType
		{
			get
			{
				return this._xValueType;
			}
			set
			{
				this._xValueType = value;
			}
		}

		internal bool IsNotificationEnable
		{
			get;
			set;
		}

		internal double ActualNumericXValue
		{
			get
			{
				return this._actualNumericXValue;
			}
			set
			{
				this._actualNumericXValue = value;
			}
		}

		internal Point VisualPosition
		{
			get;
			set;
		}

		internal FrameworkElement LabelVisual
		{
			get;
			set;
		}

		internal Storyboard Storyboard
		{
			get;
			set;
		}

		internal double InternalXValue
		{
			get;
			set;
		}

		internal DateTime InternalXValueAsDateTime
		{
			get;
			set;
		}

		internal DateTime ActualXValueAsDateTime
		{
			get;
			set;
		}

		internal Marker LegendMarker
		{
			get;
			set;
		}

		internal Faces Faces
		{
			get;
			set;
		}

		internal Faces ShadowFaces
		{
			get;
			set;
		}

		internal Marker Marker
		{
			get;
			set;
		}

		internal bool IsTopOfStack
		{
			get;
			set;
		}

		internal Storyboard ExplodeAnimation
		{
			get;
			set;
		}

		internal Storyboard UnExplodeAnimation
		{
			get;
			set;
		}

		internal object VisualParams
		{
			get;
			set;
		}

		internal bool IsLabelStyleSet
		{
			get;
			set;
		}

		internal bool OldLightingState
		{
			get;
			set;
		}

		internal double OldYValue
		{
			get;
			set;
		}

		internal bool InteractiveExplodeState
		{
			get;
			set;
		}

		internal bool InterativityAnimationState
		{
			get;
			set;
		}

		internal Brush InternalColor
		{
			get;
			set;
		}

		internal bool IsAlreadyExploded
		{
			get;
			set;
		}

		internal bool IsExplodeRunning
		{
			get;
			set;
		}

		internal bool IsPriceUp
		{
			get;
			set;
		}

		internal Point RasterVisualPosition
		{
			get;
			set;
		}

		internal double[] InternalYValues
		{
			get;
			set;
		}

		internal double InternalYValue
		{
			get;
			set;
		}

		internal double[] OldYValues
		{
			get;
			set;
		}

		internal string ParsedToolTipText
		{
			get;
			set;
		}

		internal double ClippedWidth4Pie
		{
			get;
			set;
		}

		internal bool LightWeight
		{
			get
			{
				return this._lightWeight;
			}
			set
			{
				this._lightWeight = value;
			}
		}

		public DpInfo(IDataPoint dataPoint)
		{
			this.dp = dataPoint;
			this.IsNotificationEnable = true;
		}

		public DpInfo()
		{
		}
	}
}
