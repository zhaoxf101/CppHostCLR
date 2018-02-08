using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class DataPoint : ObservableObject, IDataPoint, IEvent, IToolTip, IElement, INotifyPropertyChanged, IDataPointParentSetter
	{
		public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnSelectedChanged)));

		public static readonly DependencyProperty HrefTargetProperty = DependencyProperty.Register("HrefTarget", typeof(HrefTargets?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnHrefTargetChanged)));

		public static readonly DependencyProperty HrefProperty = DependencyProperty.Register("Href", typeof(string), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnHrefChanged)));

		public new static readonly DependencyProperty OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(DataPoint), new PropertyMetadata(1.0, new PropertyChangedCallback(DataPoint.OnOpacityPropertyChanged)));

		public static readonly DependencyProperty YValueProperty = DependencyProperty.Register("YValue", typeof(double), typeof(DataPoint), new PropertyMetadata(double.NaN, new PropertyChangedCallback(DataPoint.OnYValuePropertyChanged)));

		public static readonly DependencyProperty YValuesProperty = DependencyProperty.Register("YValues", typeof(double[]), typeof(DataPoint), new PropertyMetadata(null, new PropertyChangedCallback(DataPoint.OnYValuesPropertyChanged)));

		public static readonly DependencyProperty XValueProperty = DependencyProperty.Register("XValue", typeof(object), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnXValuePropertyChanged)));

		public static readonly DependencyProperty ZValueProperty = DependencyProperty.Register("ZValue", typeof(double), typeof(DataPoint), new PropertyMetadata(double.NaN, new PropertyChangedCallback(DataPoint.OnZValuePropertyChanged)));

		public static readonly DependencyProperty AxisXLabelProperty = DependencyProperty.Register("AxisXLabel", typeof(string), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnAxisXLabelPropertyChanged)));

		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnColorPropertyChanged)));

		public static readonly DependencyProperty StickColorProperty = DependencyProperty.Register("StickColor", typeof(Brush), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnStickColorPropertyChanged)));

		public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnEnabledPropertyChanged)));

		public static readonly DependencyProperty ExplodedProperty = DependencyProperty.Register("Exploded", typeof(bool?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnExplodedPropertyChanged)));

		public static readonly DependencyProperty LightingEnabledProperty = DependencyProperty.Register("LightingEnabled", typeof(bool?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLightingEnabledPropertyChanged)));

		public new static readonly DependencyProperty EffectProperty = DependencyProperty.Register("Effect", typeof(Effect), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnEffectPropertyChanged)));

		public static readonly DependencyProperty ShadowEnabledProperty = DependencyProperty.Register("ShadowEnabled", typeof(bool?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnShadowEnabledPropertyChanged)));

		public static readonly DependencyProperty ShowInLegendProperty = DependencyProperty.Register("ShowInLegend", typeof(bool?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnShowInLegendPropertyChanged)));

		public static readonly DependencyProperty LegendTextProperty = DependencyProperty.Register("LegendText", typeof(string), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLegendTextPropertyChanged)));

		public static readonly DependencyProperty LegendMarkerColorProperty = DependencyProperty.Register("LegendMarkerColor", typeof(Brush), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLegendMarkerColorPropertyChanged)));

		public static readonly DependencyProperty LegendMarkerTypeProperty = DependencyProperty.Register("LegendMarkerType", typeof(MarkerTypes?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLegendMarkerTypePropertyChanged)));

		public new static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnBorderThicknessPropertyChanged)));

		public static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(Brush), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnBorderColorPropertyChanged)));

		public static readonly DependencyProperty BorderStyleProperty = DependencyProperty.Register("BorderStyle", typeof(BorderStyles?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnBorderStylePropertyChanged)));

		public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(CornerRadius?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnRadiusXPropertyChanged)));

		public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(CornerRadius?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnRadiusYPropertyChanged)));

		public static readonly DependencyProperty LabelEnabledProperty = DependencyProperty.Register("LabelEnabled", typeof(bool?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelEnabledPropertyChanged)));

		public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register("LabelText", typeof(string), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelTextPropertyChanged)));

		public static readonly DependencyProperty LabelFontFamilyProperty = DependencyProperty.Register("LabelFontFamily", typeof(FontFamily), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelFontFamilyPropertyChanged)));

		public static readonly DependencyProperty LabelFontSizeProperty = DependencyProperty.Register("LabelFontSize", typeof(double?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelFontSizePropertyChanged)));

		public static readonly DependencyProperty LabelFontColorProperty = DependencyProperty.Register("LabelFontColor", typeof(Brush), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelFontColorPropertyChanged)));

		public static readonly DependencyProperty LabelFontWeightProperty = DependencyProperty.Register("LabelFontWeight", typeof(FontWeight?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelFontWeightPropertyChanged)));

		public static readonly DependencyProperty LabelFontStyleProperty = DependencyProperty.Register("LabelFontStyle", typeof(FontStyle?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelFontStylePropertyChanged)));

		public static readonly DependencyProperty LabelBackgroundProperty = DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelBackgroundPropertyChanged)));

		public static readonly DependencyProperty LabelStyleProperty = DependencyProperty.Register("LabelStyle", typeof(LabelStyles?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelStylePropertyChanged)));

		public static readonly DependencyProperty LabelAngleProperty = DependencyProperty.Register("LabelAngle", typeof(double), typeof(DataPoint), new PropertyMetadata(double.NaN, new PropertyChangedCallback(DataPoint.OnLabelAnglePropertyChanged)));

		public static readonly DependencyProperty LabelLineEnabledProperty = DependencyProperty.Register("LabelLineEnabled", typeof(bool?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelLineEnabledPropertyChanged)));

		public static readonly DependencyProperty LabelLineColorProperty = DependencyProperty.Register("LabelLineColor", typeof(Brush), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelLineColorPropertyChanged)));

		public static readonly DependencyProperty LabelLineThicknessProperty = DependencyProperty.Register("LabelLineThickness", typeof(double?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelLineThicknessPropertyChanged)));

		public static readonly DependencyProperty LabelLineStyleProperty = DependencyProperty.Register("LabelLineStyle", typeof(LineStyles?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnLabelLineStylePropertyChanged)));

		public static readonly DependencyProperty MarkerEnabledProperty = DependencyProperty.Register("MarkerEnabled", typeof(bool?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnMarkerEnabledPropertyChanged)));

		public static readonly DependencyProperty MarkerTypeProperty = DependencyProperty.Register("MarkerType", typeof(MarkerTypes?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnMarkerTypePropertyChanged)));

		public static readonly DependencyProperty MarkerBorderThicknessProperty = DependencyProperty.Register("MarkerBorderThickness", typeof(Thickness?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnMarkerBorderThicknessPropertyChanged)));

		public static readonly DependencyProperty MarkerBorderColorProperty = DependencyProperty.Register("MarkerBorderColor", typeof(Brush), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnMarkerBorderColorPropertyChanged)));

		public static readonly DependencyProperty MarkerSizeProperty = DependencyProperty.Register("MarkerSize", typeof(double?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnMarkerSizePropertyChanged)));

		public static readonly DependencyProperty MarkerColorProperty = DependencyProperty.Register("MarkerColor", typeof(Brush), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnMarkerColorPropertyChanged)));

		public static readonly DependencyProperty MarkerScaleProperty = DependencyProperty.Register("MarkerScale", typeof(double?), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnMarkerScalePropertyChanged)));

		public new static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DataPoint), new PropertyMetadata("DataPoint"));

		private Thickness? _borderThickness = null;

		private double _internalOpacity = double.NaN;

		private DataSeries _parent;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsLightDataPoint
		{
			get;
			set;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public DpFunc DpFunc
		{
			get;
			set;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public DpInfo DpInfo
		{
			get;
			set;
		}

		public new string Name
		{
			get
			{
				return (string)base.GetValue(DataPoint.NameProperty);
			}
			set
			{
				base.SetValue(DataPoint.NameProperty, value);
			}
		}

		public HrefTargets? HrefTarget
		{
			get
			{
				if (!((HrefTargets?)base.GetValue(DataPoint.HrefTargetProperty)).HasValue && this._parent != null)
				{
					return new HrefTargets?(this._parent.HrefTarget);
				}
				return (HrefTargets?)base.GetValue(DataPoint.HrefTargetProperty);
			}
			set
			{
				base.SetValue(DataPoint.HrefTargetProperty, value);
			}
		}

		public bool Selected
		{
			get
			{
				if (this.Parent != null)
				{
					return (bool)base.GetValue(DataPoint.SelectedProperty) & this.Parent.SelectionEnabled;
				}
				return (bool)base.GetValue(DataPoint.SelectedProperty);
			}
			set
			{
				base.SetValue(DataPoint.SelectedProperty, value);
			}
		}

		public string Href
		{
			get
			{
				if (string.IsNullOrEmpty((string)base.GetValue(DataPoint.HrefProperty)) && this._parent != null)
				{
					return this._parent.Href;
				}
				return (string)base.GetValue(DataPoint.HrefProperty);
			}
			set
			{
				base.SetValue(DataPoint.HrefProperty, value);
			}
		}

		public new double Opacity
		{
			get
			{
				return (double)base.GetValue(DataPoint.OpacityProperty);
			}
			set
			{
				base.SetValue(DataPoint.OpacityProperty, value);
			}
		}

		public new Cursor Cursor
		{
			get
			{
				if (base.Cursor == null && this._parent != null)
				{
					return this._parent.Cursor;
				}
				return base.Cursor;
			}
			set
			{
				if (base.Cursor != value)
				{
					base.Cursor = value;
					if (this.DpInfo.Faces != null || (this._parent != null && this._parent.Faces != null) || this.DpInfo.Marker != null)
					{
						DataPointHelper.SetCursor2DataPointVisualFaces(this);
						return;
					}
					DataPointHelper.InvokeUpdateVisual(this, VcProperties.Cursor, value);
				}
			}
		}

		[TypeConverter(typeof(Converters.ValueConverter))]
		public double YValue
		{
			get
			{
				return (double)base.GetValue(DataPoint.YValueProperty);
			}
			set
			{
				base.SetValue(DataPoint.YValueProperty, value);
			}
		}

		[TypeConverter(typeof(Converters.DoubleArrayConverter))]
		public double[] YValues
		{
			get
			{
				return (double[])base.GetValue(DataPoint.YValuesProperty);
			}
			set
			{
				base.SetValue(DataPoint.YValuesProperty, value);
			}
		}

		[TypeConverter(typeof(Converters.ObjectConverter))]
		public object XValue
		{
			get
			{
				return base.GetValue(DataPoint.XValueProperty);
			}
			set
			{
				base.SetValue(DataPoint.XValueProperty, value);
			}
		}

		[TypeConverter(typeof(Converters.ValueConverter))]
		public double ZValue
		{
			get
			{
				return (double)base.GetValue(DataPoint.ZValueProperty);
			}
			set
			{
				base.SetValue(DataPoint.ZValueProperty, value);
			}
		}

		public string AxisXLabel
		{
			get
			{
				return (string)base.GetValue(DataPoint.AxisXLabelProperty);
			}
			set
			{
				base.SetValue(DataPoint.AxisXLabelProperty, value);
			}
		}

		public Brush Color
		{
			get
			{
				if ((Brush)base.GetValue(DataPoint.ColorProperty) != null)
				{
					return (Brush)base.GetValue(DataPoint.ColorProperty);
				}
				VisifireControl arg_18_0 = base.Chart;
				if (this._parent == null)
				{
					return this.DpInfo.InternalColor;
				}
				if (this._parent.Color != null)
				{
					return this._parent.Color;
				}
				return this.DpInfo.InternalColor;
			}
			set
			{
				base.SetValue(DataPoint.ColorProperty, value);
			}
		}

		public Brush StickColor
		{
			get
			{
				if ((Brush)base.GetValue(DataPoint.StickColorProperty) == null && this._parent != null)
				{
					return this._parent.StickColor;
				}
				return (Brush)base.GetValue(DataPoint.StickColorProperty);
			}
			set
			{
				base.SetValue(DataPoint.StickColorProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Enabled
		{
			get
			{
				if (!((bool?)base.GetValue(DataPoint.EnabledProperty)).HasValue && this._parent != null)
				{
					return this.Parent.Enabled;
				}
				return (bool?)base.GetValue(DataPoint.EnabledProperty);
			}
			set
			{
				base.SetValue(DataPoint.EnabledProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Exploded
		{
			get
			{
				return new bool?((base.GetValue(DataPoint.ExplodedProperty) == null) ? (this._parent != null && this._parent.Exploded) : ((bool)base.GetValue(DataPoint.ExplodedProperty)));
			}
			set
			{
				base.SetValue(DataPoint.ExplodedProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? LightingEnabled
		{
			get
			{
				if (!((bool?)base.GetValue(DataPoint.LightingEnabledProperty)).HasValue && this._parent != null)
				{
					return this._parent.LightingEnabled;
				}
				if (this._parent != null && this._parent.LightWeight.Value)
				{
					return new bool?(false);
				}
				return (bool?)base.GetValue(DataPoint.LightingEnabledProperty);
			}
			set
			{
				base.SetValue(DataPoint.LightingEnabledProperty, value);
			}
		}

		public new Effect Effect
		{
			get
			{
				if ((Effect)base.GetValue(DataPoint.EffectProperty) == null && this._parent != null)
				{
					return this._parent.Effect;
				}
				return (Effect)base.GetValue(DataPoint.EffectProperty);
			}
			set
			{
				base.SetValue(DataPoint.EffectProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? ShadowEnabled
		{
			get
			{
				if (!((bool?)base.GetValue(DataPoint.ShadowEnabledProperty)).HasValue && this._parent != null)
				{
					return this._parent.ShadowEnabled;
				}
				if (this._parent != null && this._parent.LightWeight.Value)
				{
					return new bool?(false);
				}
				return (bool?)base.GetValue(DataPoint.ShadowEnabledProperty);
			}
			set
			{
				base.SetValue(DataPoint.ShadowEnabledProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? LabelEnabled
		{
			get
			{
				if (base.GetValue(DataPoint.LabelEnabledProperty) == null && this._parent != null)
				{
					return this._parent.LabelEnabled;
				}
				if (this._parent != null && this._parent.LightWeight.Value)
				{
					return new bool?(false);
				}
				return (bool?)base.GetValue(DataPoint.LabelEnabledProperty);
			}
			set
			{
				base.SetValue(DataPoint.LabelEnabledProperty, value);
			}
		}

		public string LabelText
		{
			get
			{
				if (string.IsNullOrEmpty((string)base.GetValue(DataPoint.LabelTextProperty)) && this._parent != null)
				{
					return this._parent.LabelText;
				}
				return (string)base.GetValue(DataPoint.LabelTextProperty);
			}
			set
			{
				base.SetValue(DataPoint.LabelTextProperty, value);
			}
		}

		public FontFamily LabelFontFamily
		{
			get
			{
				if (base.GetValue(DataPoint.LabelFontFamilyProperty) == null && this._parent != null)
				{
					return this._parent.LabelFontFamily;
				}
				return (FontFamily)base.GetValue(DataPoint.LabelFontFamilyProperty);
			}
			set
			{
				base.SetValue(DataPoint.LabelFontFamilyProperty, value);
			}
		}

		public double? LabelFontSize
		{
			get
			{
				if (((double?)base.GetValue(DataPoint.LabelFontSizeProperty)).HasValue)
				{
					return (double?)base.GetValue(DataPoint.LabelFontSizeProperty);
				}
				if (this._parent != null)
				{
					double? labelFontSize = this._parent.LabelFontSize;
					if (labelFontSize.GetValueOrDefault() != 0.0 || !labelFontSize.HasValue)
					{
						return this._parent.LabelFontSize;
					}
				}
				return new double?(10.0);
			}
			set
			{
				base.SetValue(DataPoint.LabelFontSizeProperty, value);
			}
		}

		public Brush LabelFontColor
		{
			get
			{
				if (base.GetValue(DataPoint.LabelFontColorProperty) == null && this._parent != null)
				{
					return this._parent.LabelFontColor;
				}
				return (Brush)base.GetValue(DataPoint.LabelFontColorProperty);
			}
			set
			{
				base.SetValue(DataPoint.LabelFontColorProperty, value);
			}
		}

		public FontWeight? LabelFontWeight
		{
			get
			{
				if (!((FontWeight?)base.GetValue(DataPoint.LabelFontWeightProperty)).HasValue && this._parent != null)
				{
					return new FontWeight?(this._parent.LabelFontWeight);
				}
				return (FontWeight?)base.GetValue(DataPoint.LabelFontWeightProperty);
			}
			set
			{
				base.SetValue(DataPoint.LabelFontWeightProperty, value);
			}
		}

		public FontStyle? LabelFontStyle
		{
			get
			{
				if (!((FontStyle?)base.GetValue(DataPoint.LabelFontStyleProperty)).HasValue && this._parent != null)
				{
					return new FontStyle?(this._parent.LabelFontStyle);
				}
				return (FontStyle?)base.GetValue(DataPoint.LabelFontStyleProperty);
			}
			set
			{
				base.SetValue(DataPoint.LabelFontStyleProperty, value);
			}
		}

		public Brush LabelBackground
		{
			get
			{
				if (base.GetValue(DataPoint.LabelBackgroundProperty) == null && this._parent != null)
				{
					return this._parent.LabelBackground;
				}
				return (Brush)base.GetValue(DataPoint.LabelBackgroundProperty);
			}
			set
			{
				base.SetValue(DataPoint.LabelBackgroundProperty, value);
			}
		}

		public double LabelAngle
		{
			get
			{
				if (double.IsNaN((double)base.GetValue(DataPoint.LabelAngleProperty)) && this._parent != null)
				{
					return this._parent.LabelAngle;
				}
				return (double)base.GetValue(DataPoint.LabelAngleProperty);
			}
			set
			{
				if (value > 90.0 || value < -90.0)
				{
					throw new Exception("Invalid property value:: LabelAngle should be greater than -90 and less than 90.");
				}
				base.SetValue(DataPoint.LabelAngleProperty, value);
			}
		}

		public LabelStyles? LabelStyle
		{
			get
			{
				if (!((LabelStyles?)base.GetValue(DataPoint.LabelStyleProperty)).HasValue && this._parent != null)
				{
					this.DpInfo.IsLabelStyleSet = false;
					return this._parent.LabelStyle;
				}
				this.DpInfo.IsLabelStyleSet = true;
				return (LabelStyles?)base.GetValue(DataPoint.LabelStyleProperty);
			}
			set
			{
				base.SetValue(DataPoint.LabelStyleProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? LabelLineEnabled
		{
			get
			{
				if (this.LabelEnabled.HasValue && this.LabelEnabled.Value)
				{
					bool? result = null;
					if (!((bool?)base.GetValue(DataPoint.LabelLineEnabledProperty)).HasValue)
					{
						if (this._parent != null)
						{
							RenderAs renderAs = this._parent.RenderAs;
							if ((renderAs == RenderAs.Pie || renderAs == RenderAs.Doughnut || renderAs == RenderAs.StreamLineFunnel || renderAs == RenderAs.SectionFunnel || renderAs == RenderAs.Pyramid) && this.LabelStyle == LabelStyles.OutSide)
							{
								result = (this._parent.LabelLineEnabled.HasValue ? this._parent.LabelLineEnabled : new bool?(true));
							}
							else
							{
								result = new bool?(false);
							}
						}
						else
						{
							result = new bool?(false);
						}
					}
					else
					{
						result = (bool?)base.GetValue(DataPoint.LabelLineEnabledProperty);
					}
					return result;
				}
				return new bool?(false);
			}
			set
			{
				base.SetValue(DataPoint.LabelLineEnabledProperty, value);
			}
		}

		public Brush LabelLineColor
		{
			get
			{
				if (base.GetValue(DataPoint.LabelLineColorProperty) == null && this._parent != null)
				{
					return this._parent.LabelLineColor;
				}
				return (Brush)base.GetValue(DataPoint.LabelLineColorProperty);
			}
			set
			{
				base.SetValue(DataPoint.LabelLineColorProperty, value);
			}
		}

		public double? LabelLineThickness
		{
			get
			{
				if (((double?)base.GetValue(DataPoint.LabelLineThicknessProperty)).HasValue)
				{
					return new double?((double)base.GetValue(DataPoint.LabelLineThicknessProperty));
				}
				if (this._parent != null && this._parent.LabelLineThickness != 0.0)
				{
					return new double?(this._parent.LabelLineThickness);
				}
				return new double?(0.5);
			}
			set
			{
				base.SetValue(DataPoint.LabelLineThicknessProperty, value);
			}
		}

		public LineStyles? LabelLineStyle
		{
			get
			{
				if (!((LineStyles?)base.GetValue(DataPoint.LabelLineStyleProperty)).HasValue && this._parent != null)
				{
					return new LineStyles?(this._parent.LabelLineStyle);
				}
				return (LineStyles?)base.GetValue(DataPoint.LabelLineStyleProperty);
			}
			set
			{
				base.SetValue(DataPoint.LabelLineStyleProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? MarkerEnabled
		{
			get
			{
				if (!((bool?)base.GetValue(DataPoint.MarkerEnabledProperty)).HasValue && this._parent != null)
				{
					return this._parent.MarkerEnabled;
				}
				if (this._parent == null || !this._parent.LightWeight.Value)
				{
					return (bool?)base.GetValue(DataPoint.MarkerEnabledProperty);
				}
				RenderAs renderAs = this._parent.RenderAs;
				if (renderAs != RenderAs.Line && renderAs != RenderAs.StepLine && renderAs != RenderAs.Spline)
				{
					return new bool?(false);
				}
				return (bool?)base.GetValue(DataPoint.MarkerEnabledProperty);
			}
			set
			{
				base.SetValue(DataPoint.MarkerEnabledProperty, value);
			}
		}

		public MarkerTypes? MarkerType
		{
			get
			{
				if (!((MarkerTypes?)base.GetValue(DataPoint.MarkerTypeProperty)).HasValue && this._parent != null)
				{
					return new MarkerTypes?(this._parent.MarkerType);
				}
				return (MarkerTypes?)base.GetValue(DataPoint.MarkerTypeProperty);
			}
			set
			{
				base.SetValue(DataPoint.MarkerTypeProperty, value);
			}
		}

		public Thickness? MarkerBorderThickness
		{
			get
			{
				if (((Thickness?)base.GetValue(DataPoint.MarkerBorderThicknessProperty)).HasValue)
				{
					return (Thickness?)base.GetValue(DataPoint.MarkerBorderThicknessProperty);
				}
				if (this._parent == null)
				{
					return new Thickness?(new Thickness(0.0));
				}
				Thickness? markerBorderThickness = this._parent.MarkerBorderThickness;
				if (markerBorderThickness.HasValue)
				{
					return markerBorderThickness;
				}
				RenderAs renderAs = this.Parent.RenderAs;
				MarkerTypes value = this.MarkerType.Value;
				if (renderAs == RenderAs.Point && value == MarkerTypes.Cross)
				{
					return new Thickness?(new Thickness(1.0));
				}
				if (renderAs == RenderAs.Bubble || (renderAs == RenderAs.Point && value != MarkerTypes.Cross))
				{
					return new Thickness?(new Thickness(0.0));
				}
				return new Thickness?(new Thickness(this.MarkerSize.Value / 6.0));
			}
			set
			{
				base.SetValue(DataPoint.MarkerBorderThicknessProperty, value);
			}
		}

		public Brush MarkerBorderColor
		{
			get
			{
				if (base.GetValue(DataPoint.MarkerBorderColorProperty) != null)
				{
					return (Brush)base.GetValue(DataPoint.MarkerBorderColorProperty);
				}
				if (this._parent == null)
				{
					return null;
				}
				Brush markerBorderColor = this._parent.MarkerBorderColor;
				if (markerBorderColor != null)
				{
					return markerBorderColor;
				}
				if (this.DpInfo.InternalColor != null)
				{
					return this.DpInfo.InternalColor;
				}
				return this._parent._internalColor;
			}
			set
			{
				base.SetValue(DataPoint.MarkerBorderColorProperty, value);
			}
		}

		public double? MarkerSize
		{
			get
			{
				if (!((double?)base.GetValue(DataPoint.MarkerSizeProperty)).HasValue && this._parent != null)
				{
					return this._parent.MarkerSize;
				}
				return (double?)base.GetValue(DataPoint.MarkerSizeProperty);
			}
			set
			{
				base.SetValue(DataPoint.MarkerSizeProperty, value);
			}
		}

		public Brush MarkerColor
		{
			get
			{
				if (base.GetValue(DataPoint.MarkerColorProperty) != null || this._parent == null)
				{
					return (Brush)base.GetValue(DataPoint.MarkerColorProperty);
				}
				if (this._parent.MarkerColor == null)
				{
					return Graphics.WHITE_BRUSH;
				}
				return this._parent.MarkerColor;
			}
			set
			{
				base.SetValue(DataPoint.MarkerColorProperty, value);
			}
		}

		public double? MarkerScale
		{
			get
			{
				if (!((double?)base.GetValue(DataPoint.MarkerScaleProperty)).HasValue && this._parent != null)
				{
					return this._parent.MarkerScale;
				}
				return (double?)base.GetValue(DataPoint.MarkerScaleProperty);
			}
			set
			{
				base.SetValue(DataPoint.MarkerScaleProperty, value);
			}
		}

		public override string ToolTipText
		{
			get
			{
				if (base.Chart != null && !string.IsNullOrEmpty((base.Chart as Chart).ToolTipText))
				{
					return null;
				}
				if ((string)base.GetValue(VisifireElement.ToolTipTextProperty) == string.Empty && this._parent != null)
				{
					return this._parent.ToolTipText;
				}
				return (string)base.GetValue(VisifireElement.ToolTipTextProperty);
			}
			set
			{
				base.SetValue(VisifireElement.ToolTipTextProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? ShowInLegend
		{
			get
			{
				if (!((bool?)base.GetValue(DataPoint.ShowInLegendProperty)).HasValue && this._parent != null)
				{
					return this._parent.ShowInLegend;
				}
				return (bool?)base.GetValue(DataPoint.ShowInLegendProperty);
			}
			set
			{
				base.SetValue(DataPoint.ShowInLegendProperty, value);
			}
		}

		public string LegendText
		{
			get
			{
				if (string.IsNullOrEmpty((string)base.GetValue(DataPoint.LegendTextProperty)) && this._parent != null)
				{
					return this._parent.LegendText;
				}
				return (string)base.GetValue(DataPoint.LegendTextProperty);
			}
			set
			{
				base.SetValue(DataPoint.LegendTextProperty, value);
			}
		}

		public Brush LegendMarkerColor
		{
			get
			{
				return (Brush)base.GetValue(DataPoint.LegendMarkerColorProperty);
			}
			set
			{
				base.SetValue(DataPoint.LegendMarkerColorProperty, value);
			}
		}

		public MarkerTypes? LegendMarkerType
		{
			get
			{
				return (MarkerTypes?)base.GetValue(DataPoint.LegendMarkerTypeProperty);
			}
			set
			{
				base.SetValue(DataPoint.LegendMarkerTypeProperty, value);
			}
		}

		public new Thickness BorderThickness
		{
			get
			{
				Thickness thickness = (Thickness)base.GetValue(DataPoint.BorderThicknessProperty);
				if (thickness == new Thickness(0.0, 0.0, 0.0, 0.0))
				{
					thickness = ((this._parent == null) ? thickness : this._parent.InternalBorderThickness);
				}
				return thickness;
			}
			set
			{
				base.SetValue(DataPoint.BorderThicknessProperty, value);
			}
		}

		public Brush BorderColor
		{
			get
			{
				if (base.GetValue(DataPoint.BorderColorProperty) == null && this._parent != null)
				{
					return this._parent.BorderColor;
				}
				return (Brush)base.GetValue(DataPoint.BorderColorProperty);
			}
			set
			{
				base.SetValue(DataPoint.BorderColorProperty, value);
			}
		}

		public BorderStyles? BorderStyle
		{
			get
			{
				if (!((BorderStyles?)base.GetValue(DataPoint.BorderStyleProperty)).HasValue && this._parent != null)
				{
					return new BorderStyles?(this._parent.BorderStyle);
				}
				return (BorderStyles?)base.GetValue(DataPoint.BorderStyleProperty);
			}
			set
			{
				base.SetValue(DataPoint.BorderStyleProperty, value);
			}
		}

		[TypeConverter(typeof(CornerRadiusConverter))]
		public CornerRadius? RadiusX
		{
			get
			{
				if (!((CornerRadius?)base.GetValue(DataPoint.RadiusXProperty)).HasValue && this._parent != null)
				{
					return new CornerRadius?(this._parent.RadiusX);
				}
				return (CornerRadius?)base.GetValue(DataPoint.RadiusXProperty);
			}
			set
			{
				base.SetValue(DataPoint.RadiusXProperty, value);
			}
		}

		[TypeConverter(typeof(CornerRadiusConverter))]
		public CornerRadius? RadiusY
		{
			get
			{
				if (!((CornerRadius?)base.GetValue(DataPoint.RadiusYProperty)).HasValue && this._parent != null)
				{
					return new CornerRadius?(this._parent.RadiusY);
				}
				return (CornerRadius?)base.GetValue(DataPoint.RadiusYProperty);
			}
			set
			{
				base.SetValue(DataPoint.RadiusYProperty, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DependencyObject ParentTree
		{
			get
			{
				return base.Parent;
			}
		}

		public new DataSeries Parent
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

		internal Thickness InternalBorderThickness
		{
			get
			{
				Thickness thickness = ((!this._borderThickness.HasValue) ? new Thickness?(DataPointHelper.GetBorderThickness_DataPoint(this)) : this._borderThickness).Value;
				if (thickness == new Thickness(0.0, 0.0, 0.0, 0.0))
				{
					thickness = ((this.Parent == null) ? thickness : this.Parent.InternalBorderThickness);
				}
				return thickness;
			}
			set
			{
				this._borderThickness = new Thickness?(value);
			}
		}

		internal double InternalOpacity
		{
			get
			{
				return double.IsNaN(this._internalOpacity) ? DataPointHelper.GetOpacity_DataPoint(this) : this._internalOpacity;
			}
			set
			{
				this._internalOpacity = value;
			}
		}

		internal string ParsedHref
		{
			get;
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		private new Brush BorderBrush
		{
			get;
			set;
		}

		public DataPoint()
		{
			this.DpInfo = new DpInfo(this);
			this.DpFunc = new DpFunc(this);
			this.IsLightDataPoint = false;
		}

		internal override void BindStyleAttribute()
		{
		}

		internal override void Bind()
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string ParseToolTipText(string unParsed)
		{
			return this.DpInfo.ParsedToolTipText;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void OnToolTipTextPropertyChanged(string newValue)
		{
			if (base.Chart != null && this.DpInfo.ParsedToolTipText != null)
			{
				this.DpInfo.ParsedToolTipText = this.TextParser(newValue);
			}
		}

		public override string TextParser(string unParsed)
		{
			return DataPointHelper.TextParser(this, unParsed);
		}

		private static void OnSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnSelectedChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.Selected));
		}

		private static void OnHrefTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnHrefTargetChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.HrefTarget));
		}

		private static void OnHrefChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnHrefChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.Href));
		}

		private static void OnOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnOpacityPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.Opacity));
		}

		private static void OnYValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnYValuePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.YValue));
		}

		private static void OnYValuesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnYValuesPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.YValues));
		}

		private static void OnXValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnXValuePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.XValue));
		}

		private static void OnZValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnZValuePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.ZValue));
		}

		private static void OnAxisXLabelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnAxisXLabelPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.AxisXLabel));
		}

		private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnColorPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.Color));
		}

		private static void OnStickColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnStickColorPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.StickColor));
		}

		private static void OnEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnEnabledPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.Enabled));
		}

		private static void OnExplodedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnExplodedPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.Exploded));
		}

		private static void OnLightingEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLightingEnabledPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LightingEnabled));
		}

		private static void OnEffectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnEffectPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.Effect));
		}

		private static void OnShadowEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnShadowEnabledPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.ShadowEnabled));
		}

		private static void OnLabelEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelEnabledPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelEnabled));
		}

		private static void OnLabelTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelTextPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelText));
		}

		private static void OnLabelFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelFontFamilyPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelFontFamily));
		}

		private static void OnLabelFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelFontSizePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelFontSize));
		}

		private static void OnLabelFontColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelFontColorPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelFontColor));
		}

		private static void OnLabelFontWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelFontWeightPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelFontWeight));
		}

		private static void OnLabelFontStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelFontStylePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelFontStyle));
		}

		private static void OnLabelBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelBackgroundPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelBackground));
		}

		private static void OnLabelStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelStylePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelStyle));
		}

		private static void OnLabelAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelAnglePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelAngle));
		}

		private static void OnLabelLineEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelLineEnabledPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelLineEnabled));
		}

		private static void OnLabelLineColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelLineColorPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelLineColor));
		}

		private static void OnLabelLineThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelLineThicknessPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelLineThickness));
		}

		private static void OnLabelLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLabelLineStylePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LabelLineStyle));
		}

		private static void OnMarkerEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnMarkerEnabledPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.MarkerEnabled));
		}

		private static void OnMarkerTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnMarkerTypePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.MarkerType));
		}

		private static void OnMarkerBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnMarkerBorderThicknessPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.MarkerBorderThickness));
		}

		private static void OnMarkerBorderColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnMarkerBorderColorPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.MarkerBorderColor));
		}

		private static void OnMarkerSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnMarkerSizePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.MarkerSize));
		}

		private static void OnMarkerColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnMarkerColorPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.MarkerColor));
		}

		private static void OnMarkerScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnMarkerScalePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.MarkerScale));
		}

		private static void OnShowInLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnShowInLegendPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.ShowInLegend));
		}

		private static void OnLegendTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLegendTextPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LegendText));
		}

		private static void OnBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnBorderThicknessPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.BorderThickness));
		}

		private static void OnBorderColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnBorderColorPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.BorderColor));
		}

		private static void OnLegendMarkerColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLegendMarkerColorPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LegendMarkerColor));
		}

		private static void OnLegendMarkerTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnLegendMarkerTypePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.LegendMarkerType));
		}

		private static void OnBorderStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnBorderStylePropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.BorderStyle));
		}

		private static void OnRadiusXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnRadiusXPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.RadiusX));
		}

		private static void OnRadiusYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataPointHelper.OnRadiusYPropertyChanged(d as IDataPoint, new DataPointPropertyChangedEventArgs(e, VcProperties.RadiusY));
		}

		object IDataPoint.get_DataContext()
		{
			return base.DataContext;
		}

		void IDataPoint.set_DataContext(object A_1)
		{
			base.DataContext = A_1;
		}
	}
}
