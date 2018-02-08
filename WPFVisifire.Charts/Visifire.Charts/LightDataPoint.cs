using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class LightDataPoint : IDataPoint, IEvent, IToolTip, IElement, INotifyPropertyChanged, IDataPointParentSetter
	{
		private string _tooltipText = string.Empty;

		private object _xValue;

		private double _yValue;

		private object _dataContext;

		private double[] _yValues;

		private double _zValue;

		private bool _isZValueSet;

		private string _axisXLabel;

		internal Brush _color;

		private bool? _enabled;

		private bool _isEnabledCalculated;

		private bool _calculatedEnabledValue;

		internal bool? _exploded;

		internal string _name;

		private DataSeries _parent;

		private VisifireControl _control;

		private IToolTip _element;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public event PropertyChangedEventHandler PropertyChanged;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsLightDataPoint
		{
			get;
			set;
		}

		public object Tag
		{
			get;
			set;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public VisifireControl Chart
		{
			get;
			set;
		}

		public string ToolTipText
		{
			get
			{
				if (this.Chart != null && !string.IsNullOrEmpty((this.Chart as Chart).ToolTipText))
				{
					return null;
				}
				if (this._tooltipText == string.Empty && this.Parent != null)
				{
					return this.Parent.ToolTipText;
				}
				return this._tooltipText;
			}
			set
			{
				if (this._tooltipText != value)
				{
					this._tooltipText = value;
					this.OnToolTipTextPropertyChanged(value);
				}
			}
		}

		[TypeConverter(typeof(Converters.ObjectConverter))]
		public object XValue
		{
			get
			{
				return this._xValue;
			}
			set
			{
				if (this._xValue != value)
				{
					object xValue = this._xValue;
					this._xValue = value;
					DataPointHelper.OnXValuePropertyChanged(this, new DataPointPropertyChangedEventArgs(xValue, value, VcProperties.XValue));
				}
			}
		}

		[TypeConverter(typeof(Converters.ValueConverter))]
		public double YValue
		{
			get
			{
				return this._yValue;
			}
			set
			{
				if (this._yValue != value)
				{
					object oldValue = this._yValue;
					this._yValue = value;
					DataPointHelper.OnYValuePropertyChanged(this, new DataPointPropertyChangedEventArgs(oldValue, value, VcProperties.YValue));
				}
			}
		}

		public object DataContext
		{
			get
			{
				return this._dataContext;
			}
			set
			{
				this._dataContext = value;
			}
		}

		[TypeConverter(typeof(Converters.DoubleArrayConverter))]
		public double[] YValues
		{
			get
			{
				return this._yValues;
			}
			set
			{
				if (this._yValues != value)
				{
					object yValues = this._yValues;
					this._yValues = value;
					DataPointHelper.OnYValuesPropertyChanged(this, new DataPointPropertyChangedEventArgs(yValues, value, VcProperties.YValues));
				}
			}
		}

		[TypeConverter(typeof(Converters.ValueConverter))]
		public double ZValue
		{
			get
			{
				if (!this._isZValueSet)
				{
					return double.NaN;
				}
				return this._zValue;
			}
			set
			{
				if (this._zValue != value)
				{
					this._isZValueSet = true;
					object oldValue = this._zValue;
					this._zValue = value;
					DataPointHelper.OnZValuePropertyChanged(this, new DataPointPropertyChangedEventArgs(oldValue, value, VcProperties.ZValue));
				}
			}
		}

		public string AxisXLabel
		{
			get
			{
				return this._axisXLabel;
			}
			set
			{
				if (this._axisXLabel != value)
				{
					object axisXLabel = this._axisXLabel;
					this._axisXLabel = value;
					DataPointHelper.OnAxisXLabelPropertyChanged(this, new DataPointPropertyChangedEventArgs(axisXLabel, value, VcProperties.AxisXLabel));
				}
			}
		}

		public Brush Color
		{
			get
			{
				if (this._color != null)
				{
					return this._color;
				}
				VisifireControl arg_0E_0 = this.Chart;
				if (this.Parent == null)
				{
					return this.DpInfo.InternalColor;
				}
				if (this.Parent.Color != null)
				{
					return this.Parent.Color;
				}
				return this.DpInfo.InternalColor;
			}
			set
			{
				if (this._color != value)
				{
					object color = this._color;
					this._color = value;
					DataPointHelper.OnColorPropertyChanged(this, new DataPointPropertyChangedEventArgs(color, value, VcProperties.Color));
				}
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Enabled
		{
			get
			{
				if (this._enabled.HasValue || this.Parent == null)
				{
					return this._enabled;
				}
				if (this._isEnabledCalculated)
				{
					return new bool?(this._calculatedEnabledValue);
				}
				this._isEnabledCalculated = true;
				bool? flag = (bool?)this.Parent.GetValue(DataSeries.EnabledProperty);
				if (flag.HasValue && !flag.Value)
				{
					this._calculatedEnabledValue = false;
				}
				else
				{
					this._calculatedEnabledValue = true;
				}
				return new bool?(this._calculatedEnabledValue);
			}
			set
			{
				if (this._enabled != value)
				{
					object oldValue = this._enabled;
					this._enabled = value;
					DataPointHelper.OnEnabledPropertyChanged(this, new DataPointPropertyChangedEventArgs(oldValue, value, VcProperties.Enabled));
				}
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Exploded
		{
			get
			{
				if (this._exploded.HasValue)
				{
					return this._exploded;
				}
				return new bool?(this.Parent != null && this.Parent.Exploded);
			}
			set
			{
				if (this._exploded != value)
				{
					object oldValue = this._exploded;
					this._exploded = value;
					DataPointHelper.OnExplodedPropertyChanged(this, new DataPointPropertyChangedEventArgs(oldValue, value, VcProperties.Exploded));
				}
			}
		}

		public string Name
		{
			get
			{
				if (string.IsNullOrEmpty(this._name))
				{
					return "DataPoint";
				}
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public DataSeries Parent
		{
			get
			{
				return this._parent;
			}
		}

		DataSeries IDataPointParentSetter.Parent
		{
			set
			{
				this._parent = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DpInfo DpInfo
		{
			get;
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DpFunc DpFunc
		{
			get;
			set;
		}

		public LightDataPoint()
		{
			this.DpInfo = new DpInfo(this);
			this.DpFunc = new DpFunc(this);
			this.IsLightDataPoint = true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void FirePropertyChanged(VcProperties propertyName)
		{
			if (this.PropertyChanged != null && this.DpInfo.IsNotificationEnable)
			{
				if (this.Chart != null)
				{
					(this.Chart as Chart)._internalPartialUpdateEnabled = false;
				}
				if (this.Chart != null)
				{
					(this.Chart as Chart)._forcedRedraw = true;
					this.PropertyChanged(this, new PropertyChangedEventArgs(Enum.GetName(typeof(VcProperties), propertyName)));
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AttachToolTip(VisifireControl control, IToolTip element, List<FrameworkElement> visualElements)
		{
			foreach (FrameworkElement current in visualElements)
			{
				this.AttachToolTip(control, element, current);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AttachToolTip(VisifireControl control, IToolTip element, FrameworkElement visualObject)
		{
			if (visualObject == null || control == null || element == null)
			{
				return;
			}
			this._control = control;
			this._element = element;
			visualObject.MouseMove += new MouseEventHandler(this.VisualObject_MouseMove4ToolTip);
			visualObject.MouseEnter += new MouseEventHandler(this.VisualObject_MouseEnter4ToolTip);
			visualObject.MouseLeave += new MouseEventHandler(this.VisualObject_MouseLeave4ToolTip);
		}

		private void VisualObject_MouseLeave4ToolTip(object sender, MouseEventArgs e)
		{
			string b = this._element.ParseToolTipText(this._element.ToolTipText);
			if (this._control._toolTip.Text == b)
			{
				this._control._toolTip.Hide();
			}
		}

		private void VisualObject_MouseEnter4ToolTip(object sender, MouseEventArgs e)
		{
			if (!string.IsNullOrEmpty(this._control.ToolTipText) && this._control != this._element)
			{
				return;
			}
			this._control._toolTip.CallOutVisiblity = Visibility.Collapsed;
			this.UpdateToolTip(sender, e);
		}

		private void VisualObject_MouseMove4ToolTip(object sender, MouseEventArgs e)
		{
			if (!string.IsNullOrEmpty(this._control.ToolTipText) && this._control != this._element)
			{
				return;
			}
			this._control._toolTip.CallOutVisiblity = Visibility.Collapsed;
			this.UpdateToolTip(sender, e);
		}

		private void UpdateToolTip(object sender, MouseEventArgs e)
		{
			if (!string.IsNullOrEmpty(this._control.ToolTipText) && this._control != this._element)
			{
				return;
			}
			if (this._element.ToolTipText == null)
			{
				return;
			}
			string text = this._element.ParseToolTipText(this._element.ToolTipText);
			if (!string.IsNullOrEmpty(text) && this._control._toolTip != null)
			{
				this._control._toolTip.Text = text;
				if (this._control.ToolTipEnabled)
				{
					this._control._toolTip.Show();
				}
				(this._control as Chart).UpdateToolTipPosition(sender, e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void DetachToolTip(FrameworkElement visualObject)
		{
			if (visualObject != null)
			{
				visualObject.MouseMove -= new MouseEventHandler(this.VisualObject_MouseMove4ToolTip);
				visualObject.MouseEnter -= new MouseEventHandler(this.VisualObject_MouseEnter4ToolTip);
				visualObject.MouseLeave -= new MouseEventHandler(this.VisualObject_MouseLeave4ToolTip);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void OnToolTipTextPropertyChanged(string newValue)
		{
			if (this.Chart != null && this.DpInfo.ParsedToolTipText != null)
			{
				this.DpInfo.ParsedToolTipText = this.TextParser(newValue);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ParseToolTipText(string unParsed)
		{
			return this.DpInfo.ParsedToolTipText;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public string TextParser(string unParsed)
		{
			return DataPointHelper.TextParser(this, unParsed);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AttachEvents2Visual4MouseDownEvent(IEvent obj, IEvent senderElement, FrameworkElement visual)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AttachEvents2Visual(VisifireElement obj, FrameworkElement visual)
		{
			if (visual != null)
			{
				this.AttachEvents2Visual(obj, obj, visual);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AttachEvents2Visual(VisifireElement chartElement, IElement senderElement, FrameworkElement visual)
		{
			if (visual == null)
			{
				return;
			}
			object obj = chartElement.GetMouseEnterEventHandler();
			if (obj != null)
			{
				visual.MouseEnter += delegate(object sender, MouseEventArgs e)
				{
					chartElement.FireMouseEnterEvent(senderElement, e);
				};
			}
			obj = chartElement.GetMouseLeaveEventHandler();
			if (obj != null)
			{
				visual.MouseLeave += delegate(object sender, MouseEventArgs e)
				{
					chartElement.FireMouseLeaveEvent(senderElement, e);
				};
			}
			obj = chartElement.GetMouseLeftButtonDownEventHandler();
			if (obj != null)
			{
				visual.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
				{
					chartElement.FireMouseLeftButtonDownEvent(senderElement, e);
				};
			}
			obj = chartElement.GetMouseLeftButtonUpEventHandler();
			if (obj != null)
			{
				visual.MouseLeftButtonUp += delegate(object sender, MouseButtonEventArgs e)
				{
					chartElement.FireMouseLeftButtonUpEvent(senderElement, e);
				};
			}
			obj = chartElement.GetMouseMoveEventHandler();
			if (obj != null)
			{
				visual.MouseMove += delegate(object sender, MouseEventArgs e)
				{
					chartElement.FireMouseMoveEvent(senderElement, e);
				};
			}
			obj = chartElement.GetMouseRightButtonDownEventHandler();
			if (obj != null)
			{
				visual.MouseRightButtonDown += delegate(object sender, MouseButtonEventArgs e)
				{
					chartElement.FireMouseRightButtonDownEvent(senderElement, e);
				};
			}
			obj = chartElement.GetMouseRightButtonUpEventHandler();
			if (obj != null)
			{
				visual.MouseRightButtonUp += delegate(object sender, MouseButtonEventArgs e)
				{
					chartElement.FireMouseRightButtonUpEvent(senderElement, e);
				};
			}
		}
	}
}
