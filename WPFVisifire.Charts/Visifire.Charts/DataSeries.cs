using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Visifire.Commons;
using Visifire.Commons.Controls;

namespace Visifire.Charts
{
	[TemplatePart(Name = "RootElement", Type = typeof(Grid))]
	public class DataSeries : ObservableObject, IElement
	{
		internal const string RootElementName = "RootElement";

		public static readonly DependencyProperty DataSourceProperty;

		public static readonly DependencyProperty IncludeDataPointsInLegendProperty;

		public static readonly DependencyProperty LineFillProperty;

		public static readonly DependencyProperty LineCapProperty;

		public static readonly DependencyProperty AutoFitToPlotAreaProperty;

		public static readonly DependencyProperty IncludeYValueInLegendProperty;

		public static readonly DependencyProperty IncludePercentageInLegendProperty;

		private static readonly DependencyProperty FillTypeProperty;

		private static readonly DependencyProperty LightWeightProperty;

		public static readonly DependencyProperty MinPointHeightProperty;

		public static readonly DependencyProperty LineTensionProperty;

		public static readonly DependencyProperty MovingMarkerEnabledProperty;

		public static readonly DependencyProperty ExplodedProperty;

		public static readonly DependencyProperty EnabledProperty;

		public static readonly DependencyProperty RenderAsProperty;

		public static readonly DependencyProperty HrefTargetProperty;

		public static readonly DependencyProperty HrefProperty;

		public static readonly DependencyProperty HighLightColorProperty;

		public static readonly DependencyProperty ColorProperty;

		public new static readonly DependencyProperty EffectProperty;

		public static readonly DependencyProperty StickColorProperty;

		public static readonly DependencyProperty PriceUpColorProperty;

		public static readonly DependencyProperty PriceDownColorProperty;

		public static readonly DependencyProperty LightingEnabledProperty;

		public static readonly DependencyProperty LegendMarkerTypeProperty;

		public static readonly DependencyProperty ShadowEnabledProperty;

		public static readonly DependencyProperty LegendTextProperty;

		public static readonly DependencyProperty LegendProperty;

		public static readonly DependencyProperty BevelProperty;

		public static readonly DependencyProperty LegendMarkerColorProperty;

		public static readonly DependencyProperty ColorSetProperty;

		public static readonly DependencyProperty RadiusXProperty;

		public static readonly DependencyProperty RadiusYProperty;

		public static readonly DependencyProperty LineThicknessProperty;

		public static readonly DependencyProperty LineStyleProperty;

		public new static readonly DependencyProperty OpacityProperty;

		public static readonly DependencyProperty ShowInLegendProperty;

		public static readonly DependencyProperty LabelEnabledProperty;

		public static readonly DependencyProperty LabelTextProperty;

		public static readonly DependencyProperty LabelFontFamilyProperty;

		public static readonly DependencyProperty LabelFontSizeProperty;

		public static readonly DependencyProperty LabelFontColorProperty;

		public static readonly DependencyProperty LabelFontWeightProperty;

		public static readonly DependencyProperty LabelFontStyleProperty;

		public static readonly DependencyProperty LabelBackgroundProperty;

		public static readonly DependencyProperty LabelAngleProperty;

		public static readonly DependencyProperty LabelStyleProperty;

		public static readonly DependencyProperty LabelLineEnabledProperty;

		public static readonly DependencyProperty LabelLineColorProperty;

		public static readonly DependencyProperty LabelLineThicknessProperty;

		public static readonly DependencyProperty LabelLineStyleProperty;

		public static readonly DependencyProperty MarkerEnabledProperty;

		public static readonly DependencyProperty MarkerTypeProperty;

		public static readonly DependencyProperty MarkerBorderThicknessProperty;

		public static readonly DependencyProperty MarkerBorderColorProperty;

		public static readonly DependencyProperty MarkerSizeProperty;

		public static readonly DependencyProperty MarkerColorProperty;

		public static readonly DependencyProperty MarkerScaleProperty;

		public static readonly DependencyProperty BubbleStyleProperty;

		public static readonly DependencyProperty StartAngleProperty;

		public new static readonly DependencyProperty BorderThicknessProperty;

		public static readonly DependencyProperty BorderColorProperty;

		public static readonly DependencyProperty BorderStyleProperty;

		public static readonly DependencyProperty XValueFormatStringProperty;

		public static readonly DependencyProperty YValueFormatStringProperty;

		public static readonly DependencyProperty PercentageFormatStringProperty;

		public new static readonly DependencyProperty ToolTipProperty;

		public static readonly DependencyProperty ZValueFormatStringProperty;

		public static readonly DependencyProperty AxisXTypeProperty;

		public static readonly DependencyProperty AxisYTypeProperty;

		public static readonly DependencyProperty XValueTypeProperty;

		public static readonly DependencyProperty SelectionEnabledProperty;

		public static readonly DependencyProperty SelectionModeProperty;

		public static readonly DependencyProperty ZIndexProperty;

		public new static readonly DependencyProperty NameProperty;

		public static readonly DependencyProperty DataPointsProperty;

		internal Canvas _rootElement;

		internal bool isSeriesRenderPending;

		private bool PARTIAL_DS_RENDER_LOCK;

		internal WeakEventListener<DataSeries, object, NotifyCollectionChangedEventArgs> _weakEventListener;

		internal Stack<DataPointCollection> _oldDataPoints = new Stack<DataPointCollection>();

		internal bool _isSelectedEventAttached;

		internal Brush _internalColor;

		internal bool _isAutoName = true;

		internal Ellipse _movingMarker;

		internal bool _isZooming;

		internal IDataPoint _nearestDataPoint;

		private Thickness? _borderThickness = null;

		private double _internalOpacity = double.NaN;

		private static bool _defaultStyleKeyApplied;

		public IEnumerable DataSource
		{
			get
			{
				return (IEnumerable)base.GetValue(DataSeries.DataSourceProperty);
			}
			set
			{
				base.SetValue(DataSeries.DataSourceProperty, value);
			}
		}

		public bool IsAntiAliased
		{
			get;
			set;
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? LightWeight
		{
			get
			{
				if (((bool?)base.GetValue(DataSeries.LightWeightProperty)).HasValue)
				{
					return (bool?)base.GetValue(DataSeries.LightWeightProperty);
				}
				if (this.RenderAs == RenderAs.QuickLine)
				{
					return new bool?(true);
				}
				if (this.InternalDataPoints != null && this.InternalDataPoints.Count > 2000)
				{
					return new bool?(true);
				}
				return new bool?(false);
			}
			set
			{
				base.SetValue(DataSeries.LightWeightProperty, value);
			}
		}

		public new string Name
		{
			get
			{
				return (string)base.GetValue(DataSeries.NameProperty);
			}
			set
			{
				base.SetValue(DataSeries.NameProperty, value);
			}
		}

		internal FillType FillType
		{
			get
			{
				return (FillType)base.GetValue(DataSeries.FillTypeProperty);
			}
			set
			{
				base.SetValue(DataSeries.FillTypeProperty, value);
			}
		}

		public bool IncludePercentageInLegend
		{
			get
			{
				return (bool)base.GetValue(DataSeries.IncludePercentageInLegendProperty);
			}
			set
			{
				base.SetValue(DataSeries.IncludePercentageInLegendProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? IncludeDataPointsInLegend
		{
			get
			{
				return (bool?)base.GetValue(DataSeries.IncludeDataPointsInLegendProperty);
			}
			set
			{
				base.SetValue(DataSeries.IncludeDataPointsInLegendProperty, value);
			}
		}

		public Brush LineFill
		{
			get
			{
				return (Brush)base.GetValue(DataSeries.LineFillProperty);
			}
			set
			{
				base.SetValue(DataSeries.LineFillProperty, value);
			}
		}

		public PenLineCap LineCap
		{
			get
			{
				return (PenLineCap)base.GetValue(DataSeries.LineCapProperty);
			}
			set
			{
				base.SetValue(DataSeries.LineCapProperty, value);
			}
		}

		public bool AutoFitToPlotArea
		{
			get
			{
				return (bool)base.GetValue(DataSeries.AutoFitToPlotAreaProperty);
			}
			set
			{
				base.SetValue(DataSeries.AutoFitToPlotAreaProperty, value);
			}
		}

		public bool IncludeYValueInLegend
		{
			get
			{
				return (bool)base.GetValue(DataSeries.IncludeYValueInLegendProperty);
			}
			set
			{
				base.SetValue(DataSeries.IncludeYValueInLegendProperty, value);
			}
		}

		public double MinPointHeight
		{
			get
			{
				return (double)base.GetValue(DataSeries.MinPointHeightProperty);
			}
			set
			{
				base.SetValue(DataSeries.MinPointHeightProperty, value);
			}
		}

		public double LineTension
		{
			get
			{
				return (double)base.GetValue(DataSeries.LineTensionProperty);
			}
			set
			{
				base.SetValue(DataSeries.LineTensionProperty, value);
			}
		}

		public bool MovingMarkerEnabled
		{
			get
			{
				return (bool)base.GetValue(DataSeries.MovingMarkerEnabledProperty);
			}
			set
			{
				base.SetValue(DataSeries.MovingMarkerEnabledProperty, value);
			}
		}

		public bool Exploded
		{
			get
			{
				return (bool)base.GetValue(DataSeries.ExplodedProperty);
			}
			set
			{
				base.SetValue(DataSeries.ExplodedProperty, value);
			}
		}

		public int ZIndex
		{
			get
			{
				return (int)base.GetValue(DataSeries.ZIndexProperty);
			}
			set
			{
				this.InternalZIndex = value;
				base.SetValue(DataSeries.ZIndexProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Enabled
		{
			get
			{
				if (!((bool?)base.GetValue(DataSeries.EnabledProperty)).HasValue)
				{
					return new bool?(true);
				}
				return (bool?)base.GetValue(DataSeries.EnabledProperty);
			}
			set
			{
				base.SetValue(DataSeries.EnabledProperty, value);
			}
		}

		public new double Opacity
		{
			get
			{
				return (double)base.GetValue(DataSeries.OpacityProperty);
			}
			set
			{
				base.SetValue(DataSeries.OpacityProperty, value);
			}
		}

		public new Cursor Cursor
		{
			get
			{
				return base.Cursor;
			}
			set
			{
				base.Cursor = value;
				if (this.DataPoints != null)
				{
					using (IEnumerator<IDataPoint> enumerator = this.DataPoints.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IDataPoint current = enumerator.Current;
							DataPointHelper.SetCursor2DataPointVisualFaces(current);
						}
						return;
					}
				}
				base.FirePropertyChanged(VcProperties.Cursor);
			}
		}

		public RenderAs RenderAs
		{
			get
			{
				return (RenderAs)base.GetValue(DataSeries.RenderAsProperty);
			}
			set
			{
				base.SetValue(DataSeries.RenderAsProperty, value);
			}
		}

		public HrefTargets HrefTarget
		{
			get
			{
				return (HrefTargets)base.GetValue(DataSeries.HrefTargetProperty);
			}
			set
			{
				base.SetValue(DataSeries.HrefTargetProperty, value);
			}
		}

		public string Href
		{
			get
			{
				return (string)base.GetValue(DataSeries.HrefProperty);
			}
			set
			{
				base.SetValue(DataSeries.HrefProperty, value);
			}
		}

		public Brush HighLightColor
		{
			get
			{
				return (Brush)base.GetValue(DataSeries.HighLightColorProperty);
			}
			set
			{
				base.SetValue(DataSeries.HighLightColorProperty, value);
			}
		}

		public Brush Color
		{
			get
			{
				if ((Brush)base.GetValue(DataSeries.ColorProperty) == null)
				{
					return this._internalColor;
				}
				return (Brush)base.GetValue(DataSeries.ColorProperty);
			}
			set
			{
				base.SetValue(DataSeries.ColorProperty, value);
			}
		}

		public new Effect Effect
		{
			get
			{
				return (Effect)base.GetValue(DataSeries.EffectProperty);
			}
			set
			{
				base.SetValue(DataSeries.EffectProperty, value);
			}
		}

		public Brush StickColor
		{
			get
			{
				return (Brush)base.GetValue(DataSeries.StickColorProperty);
			}
			set
			{
				base.SetValue(DataSeries.StickColorProperty, value);
			}
		}

		public Brush PriceUpColor
		{
			get
			{
				return (Brush)base.GetValue(DataSeries.PriceUpColorProperty);
			}
			set
			{
				base.SetValue(DataSeries.PriceUpColorProperty, value);
			}
		}

		public Brush PriceDownColor
		{
			get
			{
				if ((Brush)base.GetValue(DataSeries.PriceDownColorProperty) == null)
				{
					return Graphics.RED_BRUSH;
				}
				return (Brush)base.GetValue(DataSeries.PriceDownColorProperty);
			}
			set
			{
				base.SetValue(DataSeries.PriceDownColorProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? LightingEnabled
		{
			get
			{
				if (this.LightWeight.Value && this.RenderAs != RenderAs.QuickLine)
				{
					return new bool?(false);
				}
				if (((bool?)base.GetValue(DataSeries.LightingEnabledProperty)).HasValue)
				{
					return (bool?)base.GetValue(DataSeries.LightingEnabledProperty);
				}
				RenderAs renderAs = this.RenderAs;
				if (renderAs == RenderAs.QuickLine)
				{
					return new bool?(false);
				}
				if (renderAs == RenderAs.Bubble)
				{
					return new bool?(true);
				}
				if (this.PlotGroup == null || this.PlotGroup._dataPointsInCurrentPlotGroup == null)
				{
					return new bool?(true);
				}
				if (this.PlotGroup._dataPointsInCurrentPlotGroup.Count > 1000)
				{
					return new bool?(false);
				}
				return new bool?(true);
			}
			set
			{
				base.SetValue(DataSeries.LightingEnabledProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? ShadowEnabled
		{
			get
			{
				if (this.LightWeight.Value)
				{
					return new bool?(false);
				}
				if (((bool?)base.GetValue(DataSeries.ShadowEnabledProperty)).HasValue)
				{
					return (bool?)base.GetValue(DataSeries.ShadowEnabledProperty);
				}
				if (!RenderHelper.IsLineCType(this))
				{
					return new bool?(false);
				}
				if (base.Chart != null && !(base.Chart as Chart).ZoomingEnabled && !(base.Chart as Chart).IsScrollingActivated)
				{
					return new bool?(true);
				}
				return new bool?(false);
			}
			set
			{
				base.SetValue(DataSeries.ShadowEnabledProperty, value);
			}
		}

		public MarkerTypes? LegendMarkerType
		{
			get
			{
				return (MarkerTypes?)base.GetValue(DataSeries.LegendMarkerTypeProperty);
			}
			set
			{
				base.SetValue(DataSeries.LegendMarkerTypeProperty, value);
			}
		}

		public string LegendText
		{
			get
			{
				if (!string.IsNullOrEmpty((string)base.GetValue(DataSeries.LegendTextProperty)))
				{
					return (string)base.GetValue(DataSeries.LegendTextProperty);
				}
				RenderAs renderAs = this.RenderAs;
				if (renderAs != RenderAs.Pie && renderAs != RenderAs.Doughnut && renderAs != RenderAs.SectionFunnel && renderAs != RenderAs.StreamLineFunnel && renderAs != RenderAs.Pyramid)
				{
					return string.Empty;
				}
				if (this.InternalXValueType != ChartValueTypes.Numeric)
				{
					return "#XValue";
				}
				return "#AxisXLabel";
			}
			set
			{
				base.SetValue(DataSeries.LegendTextProperty, value);
			}
		}

		public string Legend
		{
			get
			{
				if (!string.IsNullOrEmpty((string)base.GetValue(DataSeries.LegendProperty)))
				{
					return (string)base.GetValue(DataSeries.LegendProperty);
				}
				return "Legend0";
			}
			set
			{
				base.SetValue(DataSeries.LegendProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Bevel
		{
			get
			{
				if (this.LightWeight.Value)
				{
					return new bool?(false);
				}
				if (((bool?)base.GetValue(DataSeries.BevelProperty)).HasValue)
				{
					return (bool?)base.GetValue(DataSeries.BevelProperty);
				}
				if (this.PlotGroup == null || this.PlotGroup._dataPointsInCurrentPlotGroup == null)
				{
					return new bool?(true);
				}
				if (this.PlotGroup._dataPointsInCurrentPlotGroup.Count > 1000)
				{
					return new bool?(false);
				}
				if (base.Chart != null && (base.Chart as Chart).Theme == "Theme2")
				{
					return new bool?(false);
				}
				return new bool?(true);
			}
			set
			{
				base.SetValue(DataSeries.BevelProperty, value);
			}
		}

		public Brush LegendMarkerColor
		{
			get
			{
				return (Brush)base.GetValue(DataSeries.LegendMarkerColorProperty);
			}
			set
			{
				base.SetValue(DataSeries.LegendMarkerColorProperty, value);
			}
		}

		public string ColorSet
		{
			get
			{
				return (string)base.GetValue(DataSeries.ColorSetProperty);
			}
			set
			{
				base.SetValue(DataSeries.ColorSetProperty, value);
			}
		}

		[TypeConverter(typeof(CornerRadiusConverter))]
		public CornerRadius RadiusX
		{
			get
			{
				return (CornerRadius)base.GetValue(DataSeries.RadiusXProperty);
			}
			set
			{
				base.SetValue(DataSeries.RadiusXProperty, value);
			}
		}

		[TypeConverter(typeof(CornerRadiusConverter))]
		public CornerRadius RadiusY
		{
			get
			{
				return (CornerRadius)base.GetValue(DataSeries.RadiusYProperty);
			}
			set
			{
				base.SetValue(DataSeries.RadiusYProperty, value);
			}
		}

		public double? LineThickness
		{
			get
			{
				if (((double?)base.GetValue(DataSeries.LineThicknessProperty)).HasValue)
				{
					return (double?)base.GetValue(DataSeries.LineThicknessProperty);
				}
				if (RenderHelper.IsLineCType(this))
				{
					double num = ((base.Chart as Chart).ActualWidth * (base.Chart as Chart).ActualHeight + 25000.0) / 35000.0;
					if (this.RenderAs == RenderAs.QuickLine)
					{
						return new double?((num > 1.0) ? 1.0 : num);
					}
					if (this.LightWeight.Value)
					{
						return new double?((num > 1.0) ? 1.0 : num);
					}
					return new double?((num > 3.0) ? 3.0 : num);
				}
				else
				{
					if (this.RenderAs == RenderAs.Stock)
					{
						return new double?(2.0);
					}
					return new double?(((base.Chart as Chart).ActualWidth * (base.Chart as Chart).ActualHeight + 25000.0) / 70000.0);
				}
			}
			set
			{
				base.SetValue(DataSeries.LineThicknessProperty, value);
			}
		}

		public LineStyles LineStyle
		{
			get
			{
				return (LineStyles)base.GetValue(DataSeries.LineStyleProperty);
			}
			set
			{
				base.SetValue(DataSeries.LineStyleProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? ShowInLegend
		{
			get
			{
				if (((bool?)base.GetValue(DataSeries.ShowInLegendProperty)).HasValue)
				{
					return (bool?)base.GetValue(DataSeries.ShowInLegendProperty);
				}
				if (base.Chart != null && (base.Chart as Chart).Series.Count > 1)
				{
					return new bool?(true);
				}
				return new bool?(false);
			}
			set
			{
				base.SetValue(DataSeries.ShowInLegendProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? LabelEnabled
		{
			get
			{
				if (this.LightWeight.Value)
				{
					return new bool?(false);
				}
				if (base.GetValue(DataSeries.LabelEnabledProperty) == null)
				{
					RenderAs renderAs = this.RenderAs;
					if (renderAs <= RenderAs.Doughnut)
					{
						if (renderAs != RenderAs.Pie && renderAs != RenderAs.Doughnut)
						{
							goto IL_56;
						}
					}
					else
					{
						switch (renderAs)
						{
						case RenderAs.StreamLineFunnel:
						case RenderAs.SectionFunnel:
							break;
						default:
							if (renderAs != RenderAs.Pyramid)
							{
								goto IL_56;
							}
							break;
						}
					}
					return new bool?(true);
					IL_56:
					return new bool?(false);
				}
				return (bool?)base.GetValue(DataSeries.LabelEnabledProperty);
			}
			set
			{
				base.SetValue(DataSeries.LabelEnabledProperty, value);
			}
		}

		public string LabelText
		{
			get
			{
				Chart chart = base.Chart as Chart;
				string text;
				if (string.IsNullOrEmpty((string)base.GetValue(DataSeries.LabelTextProperty)))
				{
					if (this.RenderAs == RenderAs.Doughnut || this.RenderAs == RenderAs.Pie || this.RenderAs == RenderAs.StreamLineFunnel || this.RenderAs == RenderAs.SectionFunnel || this.RenderAs == RenderAs.Pyramid)
					{
						text = "#AxisXLabel, #YValue";
					}
					else if (this.RenderAs == RenderAs.Stock || this.RenderAs == RenderAs.CandleStick)
					{
						text = "#Close";
					}
					else
					{
						text = "#YValue";
					}
				}
				else
				{
					text = (string)base.GetValue(DataSeries.LabelTextProperty);
				}
				if (chart.SamplingThreshold == 0 || chart.SamplingThreshold >= chart.PlotDetails.ListOfAllDataPoints.Count)
				{
					return text;
				}
				if (chart == null || chart.SamplingThreshold >= chart.PlotDetails.ListOfAllDataPoints.Count)
				{
					return text;
				}
				return text.Replace("#AxisXLabel", "#XValue");
			}
			set
			{
				base.SetValue(DataSeries.LabelTextProperty, value);
			}
		}

		public FontFamily LabelFontFamily
		{
			get
			{
				if (base.GetValue(DataSeries.LabelFontFamilyProperty) == null)
				{
					return new FontFamily("Verdana");
				}
				return (FontFamily)base.GetValue(DataSeries.LabelFontFamilyProperty);
			}
			set
			{
				base.SetValue(DataSeries.LabelFontFamilyProperty, value);
			}
		}

		public double? LabelFontSize
		{
			get
			{
				if (!((double?)base.GetValue(DataSeries.LabelFontSizeProperty)).HasValue)
				{
					return new double?(10.2);
				}
				return (double?)base.GetValue(DataSeries.LabelFontSizeProperty);
			}
			set
			{
				base.SetValue(DataSeries.LabelFontSizeProperty, value);
			}
		}

		public Brush LabelFontColor
		{
			get
			{
				return (Brush)base.GetValue(DataSeries.LabelFontColorProperty);
			}
			set
			{
				base.SetValue(DataSeries.LabelFontColorProperty, value);
			}
		}

		public FontWeight LabelFontWeight
		{
			get
			{
				return (FontWeight)base.GetValue(DataSeries.LabelFontWeightProperty);
			}
			set
			{
				base.SetValue(DataSeries.LabelFontWeightProperty, value);
			}
		}

		public FontStyle LabelFontStyle
		{
			get
			{
				return (FontStyle)base.GetValue(DataSeries.LabelFontStyleProperty);
			}
			set
			{
				base.SetValue(DataSeries.LabelFontStyleProperty, value);
			}
		}

		public Brush LabelBackground
		{
			get
			{
				if (base.GetValue(DataSeries.LabelBackgroundProperty) != null)
				{
					return (Brush)base.GetValue(DataSeries.LabelBackgroundProperty);
				}
				return null;
			}
			set
			{
				base.SetValue(DataSeries.LabelBackgroundProperty, value);
			}
		}

		public double LabelAngle
		{
			get
			{
				return (double)base.GetValue(DataSeries.LabelAngleProperty);
			}
			set
			{
				if (value > 90.0 || value < -90.0)
				{
					throw new Exception("Invalid property value:: LabelAngle should be greater than -90 and less than 90.");
				}
				base.SetValue(DataSeries.LabelAngleProperty, value);
			}
		}

		public LabelStyles? LabelStyle
		{
			get
			{
				if (!((LabelStyles?)base.GetValue(DataSeries.LabelStyleProperty)).HasValue)
				{
					switch (this.RenderAs)
					{
					case RenderAs.StackedColumn100:
					case RenderAs.StackedBar100:
					case RenderAs.StackedArea100:
						return new LabelStyles?(LabelStyles.Inside);
					}
					return new LabelStyles?(LabelStyles.OutSide);
				}
				this.IsLabelStyleSet = true;
				return (LabelStyles?)base.GetValue(DataSeries.LabelStyleProperty);
			}
			set
			{
				base.SetValue(DataSeries.LabelStyleProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? LabelLineEnabled
		{
			get
			{
				return (bool?)base.GetValue(DataSeries.LabelLineEnabledProperty);
			}
			set
			{
				base.SetValue(DataSeries.LabelLineEnabledProperty, value);
			}
		}

		public Brush LabelLineColor
		{
			get
			{
				if (base.GetValue(DataSeries.LabelLineColorProperty) != null)
				{
					return (Brush)base.GetValue(DataSeries.LabelLineColorProperty);
				}
				return new SolidColorBrush(Colors.Gray);
			}
			set
			{
				base.SetValue(DataSeries.LabelLineColorProperty, value);
			}
		}

		public double LabelLineThickness
		{
			get
			{
				return (double)base.GetValue(DataSeries.LabelLineThicknessProperty);
			}
			set
			{
				base.SetValue(DataSeries.LabelLineThicknessProperty, value);
			}
		}

		public LineStyles LabelLineStyle
		{
			get
			{
				return (LineStyles)base.GetValue(DataSeries.LabelLineStyleProperty);
			}
			set
			{
				base.SetValue(DataSeries.LabelLineStyleProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? MarkerEnabled
		{
			get
			{
				RenderAs renderAs = this.RenderAs;
				if (this.LightWeight.Value && renderAs != RenderAs.Line && renderAs != RenderAs.StepLine && renderAs != RenderAs.Spline)
				{
					return new bool?(false);
				}
				if (RenderHelper.IsLineCType(this) || renderAs == RenderAs.Point)
				{
					if (((bool?)base.GetValue(DataSeries.MarkerEnabledProperty)).HasValue)
					{
						return (bool?)base.GetValue(DataSeries.MarkerEnabledProperty);
					}
					return new bool?(true);
				}
				else
				{
					if (((bool?)base.GetValue(DataSeries.MarkerEnabledProperty)).HasValue)
					{
						return (bool?)base.GetValue(DataSeries.MarkerEnabledProperty);
					}
					return new bool?(false);
				}
			}
			set
			{
				base.SetValue(DataSeries.MarkerEnabledProperty, value);
			}
		}

		public MarkerTypes MarkerType
		{
			get
			{
				return (MarkerTypes)base.GetValue(DataSeries.MarkerTypeProperty);
			}
			set
			{
				base.SetValue(DataSeries.MarkerTypeProperty, value);
			}
		}

		public Thickness? MarkerBorderThickness
		{
			get
			{
				return (Thickness?)base.GetValue(DataSeries.MarkerBorderThicknessProperty);
			}
			set
			{
				base.SetValue(DataSeries.MarkerBorderThicknessProperty, value);
			}
		}

		public Brush MarkerBorderColor
		{
			get
			{
				return (Brush)base.GetValue(DataSeries.MarkerBorderColorProperty);
			}
			set
			{
				base.SetValue(DataSeries.MarkerBorderColorProperty, value);
			}
		}

		public double? MarkerSize
		{
			get
			{
				if (((double?)base.GetValue(DataSeries.MarkerSizeProperty)).HasValue)
				{
					return (double?)base.GetValue(DataSeries.MarkerSizeProperty);
				}
				if (!RenderHelper.IsLineCType(this))
				{
					return new double?(8.0);
				}
				if (!((double?)base.GetValue(DataSeries.LineThicknessProperty)).HasValue)
				{
					double num = ((base.Chart as Chart).ActualWidth * (base.Chart as Chart).ActualHeight + 25000.0) / 35000.0;
					if (this.LightWeight.Value)
					{
						num = ((num > 2.0) ? 2.0 : num);
					}
					else
					{
						num = ((num > 3.0) ? 3.0 : num);
					}
					if (num < 2.0)
					{
						return new double?(4.0);
					}
					return new double?(num * 2.0);
				}
				else
				{
					double num2 = (double)base.GetValue(DataSeries.LineThicknessProperty);
					if (num2 < 2.0)
					{
						return new double?(4.0);
					}
					return new double?(num2 * 2.0);
				}
			}
			set
			{
				base.SetValue(DataSeries.MarkerSizeProperty, value);
			}
		}

		public Brush MarkerColor
		{
			get
			{
				return (Brush)base.GetValue(DataSeries.MarkerColorProperty);
			}
			set
			{
				base.SetValue(DataSeries.MarkerColorProperty, value);
			}
		}

		public double? MarkerScale
		{
			get
			{
				if (((double?)base.GetValue(DataSeries.MarkerScaleProperty)).HasValue)
				{
					return (double?)base.GetValue(DataSeries.MarkerScaleProperty);
				}
				if (this.RenderAs == RenderAs.Bubble)
				{
					double num = Math.Abs(base.Chart.ActualHeight - base.Chart.ActualWidth);
					num /= 100.0;
					return new double?(2.0 + Math.Sqrt(num));
				}
				return new double?(1.0);
			}
			set
			{
				base.SetValue(DataSeries.MarkerScaleProperty, value);
			}
		}

		public BubbleStyles? BubbleStyle
		{
			get
			{
				if (((BubbleStyles?)base.GetValue(DataSeries.BubbleStyleProperty)).HasValue)
				{
					return (BubbleStyles?)base.GetValue(DataSeries.BubbleStyleProperty);
				}
				if (base.Chart == null)
				{
					return new BubbleStyles?(BubbleStyles.Style1);
				}
				if ((base.Chart as Chart).View3D)
				{
					return new BubbleStyles?(BubbleStyles.Style2);
				}
				return new BubbleStyles?(BubbleStyles.Style1);
			}
			set
			{
				base.SetValue(DataSeries.BubbleStyleProperty, value);
			}
		}

		internal int InternalZIndex
		{
			get;
			set;
		}

		internal bool IsZIndexSet
		{
			get;
			set;
		}

		internal double InternalStartAngle
		{
			get
			{
				return this.StartAngle % 360.0 * 0.017453292519943295;
			}
		}

		public double StartAngle
		{
			get
			{
				return (double)base.GetValue(DataSeries.StartAngleProperty);
			}
			set
			{
				if (value < 0.0 || value > 360.0)
				{
					throw new Exception("Invalid property value:: StartAngle should be greater than 0 and less than 360.");
				}
				base.SetValue(DataSeries.StartAngleProperty, value);
			}
		}

		public new Thickness BorderThickness
		{
			get
			{
				Thickness t = (Thickness)base.GetValue(DataSeries.BorderThicknessProperty);
				if (t == new Thickness(0.0) && this.RenderAs == RenderAs.Stock)
				{
					return new Thickness(2.0);
				}
				return (Thickness)base.GetValue(DataSeries.BorderThicknessProperty);
			}
			set
			{
				base.SetValue(DataSeries.BorderThicknessProperty, value);
			}
		}

		public Brush BorderColor
		{
			get
			{
				if (base.GetValue(DataSeries.BorderColorProperty) != null)
				{
					return (Brush)base.GetValue(DataSeries.BorderColorProperty);
				}
				if (this.RenderAs == RenderAs.CandleStick)
				{
					return (Brush)base.GetValue(DataSeries.BorderColorProperty);
				}
				if (this.RenderAs != RenderAs.Radar)
				{
					return Graphics.BLACK_BRUSH;
				}
				return null;
			}
			set
			{
				base.SetValue(DataSeries.BorderColorProperty, value);
			}
		}

		public BorderStyles BorderStyle
		{
			get
			{
				return (BorderStyles)base.GetValue(DataSeries.BorderStyleProperty);
			}
			set
			{
				base.SetValue(DataSeries.BorderStyleProperty, value);
			}
		}

		public string XValueFormatString
		{
			get
			{
				return (string)base.GetValue(DataSeries.XValueFormatStringProperty);
			}
			set
			{
				base.SetValue(DataSeries.XValueFormatStringProperty, value);
			}
		}

		public string YValueFormatString
		{
			get
			{
				return (string)base.GetValue(DataSeries.YValueFormatStringProperty);
			}
			set
			{
				base.SetValue(DataSeries.YValueFormatStringProperty, value);
			}
		}

		public string PercentageFormatString
		{
			get
			{
				return (string)base.GetValue(DataSeries.PercentageFormatStringProperty);
			}
			set
			{
				base.SetValue(DataSeries.PercentageFormatStringProperty, value);
			}
		}

		public string ZValueFormatString
		{
			get
			{
				if (string.IsNullOrEmpty((string)base.GetValue(DataSeries.ZValueFormatStringProperty)))
				{
					return "###,##0.##";
				}
				return (string)base.GetValue(DataSeries.ZValueFormatStringProperty);
			}
			set
			{
				base.SetValue(DataSeries.ZValueFormatStringProperty, value);
			}
		}

		public new object ToolTip
		{
			get
			{
				return base.GetValue(DataSeries.ToolTipProperty);
			}
			set
			{
				base.SetValue(DataSeries.ToolTipProperty, value);
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
				Chart chart = base.Chart as Chart;
				string text;
				if (string.IsNullOrEmpty((string)base.GetValue(VisifireElement.ToolTipTextProperty)))
				{
					if (base.GetValue(VisifireElement.ToolTipTextProperty) == null)
					{
						return null;
					}
					RenderAs renderAs = this.RenderAs;
					if (renderAs != RenderAs.Pie)
					{
						switch (renderAs)
						{
						case RenderAs.Doughnut:
							goto IL_DB;
						case RenderAs.StackedColumn:
						case RenderAs.StackedBar:
						case RenderAs.StackedArea:
							break;
						case RenderAs.StackedColumn100:
						case RenderAs.StackedBar100:
						case RenderAs.StackedArea100:
							if (chart.ChartArea.AxisX != null && chart.ChartArea.AxisX.XValueType != ChartValueTypes.Numeric)
							{
								text = "#XValue, #YValue(#Sum)";
								goto IL_158;
							}
							text = "#AxisXLabel, #YValue(#Sum)";
							goto IL_158;
						default:
							switch (renderAs)
							{
							case RenderAs.Stock:
							case RenderAs.CandleStick:
								text = "Open: #Open\nClose: #Close\nHigh:  #High\nLow:   #Low";
								goto IL_158;
							case RenderAs.Radar:
								text = "#AxisXLabel, #YValue";
								goto IL_158;
							case RenderAs.Polar:
								text = "#XValue, #YValue";
								goto IL_158;
							}
							break;
						}
						if (chart != null && chart.ChartArea != null && chart.ChartArea.AxisX != null && chart.ChartArea.AxisX.XValueType != ChartValueTypes.Numeric)
						{
							text = "#XValue, #YValue";
							goto IL_158;
						}
						text = "#AxisXLabel, #YValue";
						goto IL_158;
					}
					IL_DB:
					if (this.InternalXValueType != ChartValueTypes.Numeric)
					{
						text = "#XValue, #YValue(#Percentage%)";
					}
					else
					{
						text = "#AxisXLabel, #YValue(#Percentage%)";
					}
				}
				else
				{
					text = (string)base.GetValue(VisifireElement.ToolTipTextProperty);
				}
				IL_158:
				if (chart != null && (chart.SamplingThreshold == 0 || chart.SamplingThreshold >= chart.PlotDetails.ListOfAllDataPoints.Count))
				{
					return text;
				}
				if (chart == null || chart.SamplingThreshold >= chart.PlotDetails.ListOfAllDataPoints.Count)
				{
					return text;
				}
				return text.Replace("#AxisXLabel", "#XValue");
			}
			set
			{
				base.SetValue(VisifireElement.ToolTipTextProperty, value);
			}
		}

		internal string ToolTipName
		{
			get;
			set;
		}

		internal ToolTip ToolTipElement
		{
			get;
			set;
		}

		public DataPointCollection DataPoints
		{
			get
			{
				return (DataPointCollection)base.GetValue(DataSeries.DataPointsProperty);
			}
			set
			{
				base.SetValue(DataSeries.DataPointsProperty, value);
			}
		}

		public DataMappingCollection DataMappings
		{
			get;
			set;
		}

		public AxisTypes AxisXType
		{
			get
			{
				return (AxisTypes)base.GetValue(DataSeries.AxisXTypeProperty);
			}
			set
			{
				base.SetValue(DataSeries.AxisXTypeProperty, value);
			}
		}

		public AxisTypes AxisYType
		{
			get
			{
				return (AxisTypes)base.GetValue(DataSeries.AxisYTypeProperty);
			}
			set
			{
				base.SetValue(DataSeries.AxisYTypeProperty, value);
			}
		}

		public ChartValueTypes XValueType
		{
			get
			{
				return (ChartValueTypes)base.GetValue(DataSeries.XValueTypeProperty);
			}
			set
			{
				base.SetValue(DataSeries.XValueTypeProperty, value);
			}
		}

		public bool SelectionEnabled
		{
			get
			{
				return (bool)base.GetValue(DataSeries.SelectionEnabledProperty);
			}
			set
			{
				base.SetValue(DataSeries.SelectionEnabledProperty, value);
			}
		}

		public SelectionModes SelectionMode
		{
			get
			{
				return (SelectionModes)base.GetValue(DataSeries.SelectionModeProperty);
			}
			set
			{
				base.SetValue(DataSeries.SelectionModeProperty, value);
			}
		}

		internal Panel Visual
		{
			get;
			set;
		}

		internal bool IsAllDataPointsEnabled
		{
			get;
			set;
		}

		internal Thickness InternalBorderThickness
		{
			get
			{
				Thickness thickness = (Thickness)((!this._borderThickness.HasValue) ? base.GetValue(DataSeries.BorderThicknessProperty) : this._borderThickness);
				if (thickness == new Thickness(0.0) && this.RenderAs == RenderAs.Stock)
				{
					return new Thickness(2.0);
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
				return (double)(double.IsNaN(this._internalOpacity) ? base.GetValue(DataSeries.OpacityProperty) : this._internalOpacity);
			}
			set
			{
				this._internalOpacity = value;
			}
		}

		internal bool IsLabelStyleSet
		{
			get;
			set;
		}

		internal List<IDataPoint> InternalDataPoints
		{
			get;
			set;
		}

		internal ChartValueTypes InternalXValueType
		{
			get;
			set;
		}

		internal List<IDataPoint> ListOfSelectedDataPoints
		{
			get;
			set;
		}

		internal object VisualParams
		{
			get;
			set;
		}

		internal Marker LegendMarker
		{
			get;
			set;
		}

		internal Storyboard Storyboard
		{
			get;
			set;
		}

		internal int SeriesCountOfSameRenderAs
		{
			get;
			set;
		}

		internal Faces Faces
		{
			get;
			set;
		}

		internal PlotGroup PlotGroup
		{
			get;
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		private new Brush Background
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		private new Brush Foreground
		{
			get;
			set;
		}

		public DataSeries()
		{
			this.DataMappings = new DataMappingCollection();
			if (this.DataPoints == null)
			{
				this.DataPoints = new DataPointCollection();
			}
			this.InternalDataPoints = new List<IDataPoint>();
			this.DataMappings.CollectionChanged += new NotifyCollectionChangedEventHandler(this.DataMappings_CollectionChanged);
		}

		static DataSeries()
		{
			DataSeries.DataSourceProperty = DependencyProperty.Register("DataSource", typeof(IEnumerable), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnDataSourceChanged)));
			DataSeries.IncludeDataPointsInLegendProperty = DependencyProperty.Register("IncludeDataPointsInLegend", typeof(bool?), typeof(DataSeries), new PropertyMetadata(null, new PropertyChangedCallback(DataSeries.OnIncludeDataPointsInLegendPropertyChanged)));
			DataSeries.LineFillProperty = DependencyProperty.Register("LineFill", typeof(Brush), typeof(DataSeries), new PropertyMetadata(null, new PropertyChangedCallback(DataSeries.OnLineFillPropertyChanged)));
			DataSeries.LineCapProperty = DependencyProperty.Register("LineCap", typeof(PenLineCap), typeof(DataSeries), new PropertyMetadata(PenLineCap.Round, new PropertyChangedCallback(DataSeries.OnLineCapPropertyChanged)));
			DataSeries.AutoFitToPlotAreaProperty = DependencyProperty.Register("AutoFitToPlotArea", typeof(bool), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnAutoFitToPlotAreaPropertyChanged)));
			DataSeries.IncludeYValueInLegendProperty = DependencyProperty.Register("IncludeYValueInLegend", typeof(bool), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnIncludeYValueInLegendPropertyChanged)));
			DataSeries.IncludePercentageInLegendProperty = DependencyProperty.Register("IncludePercentageInLegend", typeof(bool), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnIncludePercentageInLegendPropertyChanged)));
			DataSeries.FillTypeProperty = DependencyProperty.Register("FillType", typeof(FillType), typeof(DataSeries), new PropertyMetadata(FillType.Hollow, new PropertyChangedCallback(DataSeries.OnFillTypePropertyChanged)));
			DataSeries.LightWeightProperty = DependencyProperty.Register("LightWeight", typeof(bool?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLightWeightPropertyChanged)));
			DataSeries.MinPointHeightProperty = DependencyProperty.Register("MinPointHeight", typeof(double), typeof(DataSeries), new PropertyMetadata(double.NaN, new PropertyChangedCallback(DataSeries.OnMinPointHeightPropertyChanged)));
			DataSeries.LineTensionProperty = DependencyProperty.Register("LineTension", typeof(double), typeof(DataSeries), new PropertyMetadata(0.5, new PropertyChangedCallback(DataSeries.OnLineTensionPropertyChanged)));
			DataSeries.MovingMarkerEnabledProperty = DependencyProperty.Register("MovingMarkerEnabled", typeof(bool), typeof(DataSeries), new PropertyMetadata(false, new PropertyChangedCallback(DataSeries.OnMovingMarkerEnabledPropertyChanged)));
			DataSeries.ExplodedProperty = DependencyProperty.Register("Exploded", typeof(bool), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnExplodedPropertyChanged)));
			DataSeries.EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnEnabledPropertyChanged)));
			DataSeries.RenderAsProperty = DependencyProperty.Register("RenderAs", typeof(RenderAs), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnRenderAsPropertyChanged)));
			DataSeries.HrefTargetProperty = DependencyProperty.Register("HrefTarget", typeof(HrefTargets), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnHrefTargetChanged)));
			DataSeries.HrefProperty = DependencyProperty.Register("Href", typeof(string), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnHrefChanged)));
			DataSeries.HighLightColorProperty = DependencyProperty.Register("HighLightColor", typeof(Brush), typeof(DataSeries), new PropertyMetadata(new SolidColorBrush(Colors.Red), null));
			DataSeries.ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnColorPropertyChanged)));
			DataSeries.EffectProperty = DependencyProperty.Register("Effect", typeof(Effect), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnEffectPropertyChanged)));
			DataSeries.StickColorProperty = DependencyProperty.Register("StickColor", typeof(Brush), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnStickColorPropertyChanged)));
			DataSeries.PriceUpColorProperty = DependencyProperty.Register("PriceUpColor", typeof(Brush), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnPriceUpColorPropertyChanged)));
			DataSeries.PriceDownColorProperty = DependencyProperty.Register("PriceDownColor", typeof(Brush), typeof(DataSeries), new PropertyMetadata(new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 221, 0, 0)), new PropertyChangedCallback(DataSeries.OnPriceDownColorPropertyChanged)));
			DataSeries.LightingEnabledProperty = DependencyProperty.Register("LightingEnabled", typeof(bool?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLightingEnabledPropertyChanged)));
			DataSeries.LegendMarkerTypeProperty = DependencyProperty.Register("LegendMarkerType", typeof(MarkerTypes?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLegendMarkerTypePropertyChanged)));
			DataSeries.ShadowEnabledProperty = DependencyProperty.Register("ShadowEnabled", typeof(bool?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnShadowEnabledPropertyChanged)));
			DataSeries.LegendTextProperty = DependencyProperty.Register("LegendText", typeof(string), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLegendTextPropertyChanged)));
			DataSeries.LegendProperty = DependencyProperty.Register("Legend", typeof(string), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLegendPropertyChanged)));
			DataSeries.BevelProperty = DependencyProperty.Register("Bevel", typeof(bool?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnBevelPropertyChanged)));
			DataSeries.LegendMarkerColorProperty = DependencyProperty.Register("LegendMarkerColor", typeof(Brush), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLegendMarkerColorPropertyChanged)));
			DataSeries.ColorSetProperty = DependencyProperty.Register("ColorSet", typeof(string), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnColorSetPropertyChanged)));
			DataSeries.RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(CornerRadius), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnRadiusXPropertyChanged)));
			DataSeries.RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(CornerRadius), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnRadiusYPropertyChanged)));
			DataSeries.LineThicknessProperty = DependencyProperty.Register("LineThickness", typeof(double?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLineThicknessPropertyChanged)));
			DataSeries.LineStyleProperty = DependencyProperty.Register("LineStyle", typeof(LineStyles), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLineStylePropertyChanged)));
			DataSeries.OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(DataSeries), new PropertyMetadata(1.0, new PropertyChangedCallback(DataSeries.OnOpacityPropertyChanged)));
			DataSeries.ShowInLegendProperty = DependencyProperty.Register("ShowInLegend", typeof(bool?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnShowInLegendPropertyChanged)));
			DataSeries.LabelEnabledProperty = DependencyProperty.Register("LabelEnabled", typeof(bool?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelEnabledPropertyChanged)));
			DataSeries.LabelTextProperty = DependencyProperty.Register("LabelText", typeof(string), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelTextPropertyChanged)));
			DataSeries.LabelFontFamilyProperty = DependencyProperty.Register("LabelFontFamily", typeof(FontFamily), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelFontFamilyPropertyChanged)));
			DataSeries.LabelFontSizeProperty = DependencyProperty.Register("LabelFontSize", typeof(double?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelFontSizePropertyChanged)));
			DataSeries.LabelFontColorProperty = DependencyProperty.Register("LabelFontColor", typeof(Brush), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelFontColorPropertyChanged)));
			DataSeries.LabelFontWeightProperty = DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelFontWeightPropertyChanged)));
			DataSeries.LabelFontStyleProperty = DependencyProperty.Register("LabelFontStyle", typeof(FontStyle), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelFontStylePropertyChanged)));
			DataSeries.LabelBackgroundProperty = DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelBackgroundPropertyChanged)));
			DataSeries.LabelAngleProperty = DependencyProperty.Register("LabelAngle", typeof(double), typeof(DataSeries), new PropertyMetadata(double.NaN, new PropertyChangedCallback(DataSeries.OnLabelAnglePropertyChanged)));
			DataSeries.LabelStyleProperty = DependencyProperty.Register("LabelStyle", typeof(LabelStyles?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelStylePropertyChanged)));
			DataSeries.LabelLineEnabledProperty = DependencyProperty.Register("LabelLineEnabled", typeof(bool?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelLineEnabledPropertyChanged)));
			DataSeries.LabelLineColorProperty = DependencyProperty.Register("LabelLineColor", typeof(Brush), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelLineColorPropertyChanged)));
			DataSeries.LabelLineThicknessProperty = DependencyProperty.Register("LabelLineThickness", typeof(double), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelLineThicknessPropertyChanged)));
			DataSeries.LabelLineStyleProperty = DependencyProperty.Register("LabelLineStyle", typeof(LineStyles), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnLabelLineStylePropertyChanged)));
			DataSeries.MarkerEnabledProperty = DependencyProperty.Register("MarkerEnabled", typeof(bool?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnMarkerEnabledPropertyChanged)));
			DataSeries.MarkerTypeProperty = DependencyProperty.Register("MarkerType", typeof(MarkerTypes), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnMarkerTypePropertyChanged)));
			DataSeries.MarkerBorderThicknessProperty = DependencyProperty.Register("MarkerBorderThickness", typeof(Thickness?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnMarkerBorderThicknessPropertyChanged)));
			DataSeries.MarkerBorderColorProperty = DependencyProperty.Register("MarkerBorderColor", typeof(Brush), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnMarkerBorderColorPropertyChanged)));
			DataSeries.MarkerSizeProperty = DependencyProperty.Register("MarkerSize", typeof(double?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnMarkerSizePropertyChanged)));
			DataSeries.MarkerColorProperty = DependencyProperty.Register("MarkerColor", typeof(Brush), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnMarkerColorPropertyChanged)));
			DataSeries.MarkerScaleProperty = DependencyProperty.Register("MarkerScale", typeof(double?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnMarkerScalePropertyChanged)));
			DataSeries.BubbleStyleProperty = DependencyProperty.Register("BubbleStyle", typeof(BubbleStyles?), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnBubbleStylePropertyChanged)));
			DataSeries.StartAngleProperty = DependencyProperty.Register("StartAngle", typeof(double), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnStartAnglePropertyChanged)));
			DataSeries.BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnBorderThicknessPropertyChanged)));
			DataSeries.BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(Brush), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnBorderColorPropertyChanged)));
			DataSeries.BorderStyleProperty = DependencyProperty.Register("BorderStyle", typeof(BorderStyles), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnBorderStylePropertyChanged)));
			DataSeries.XValueFormatStringProperty = DependencyProperty.Register("XValueFormatString", typeof(string), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnXValueFormatStringPropertyChanged)));
			DataSeries.YValueFormatStringProperty = DependencyProperty.Register("YValueFormatString", typeof(string), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnYValueFormatStringPropertyChanged)));
			DataSeries.PercentageFormatStringProperty = DependencyProperty.Register("PercentageFormatString", typeof(string), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnPercentageFormatStringPropertyChanged)));
			DataSeries.ToolTipProperty = DependencyProperty.Register("ToolTip", typeof(string), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnToolTipPropertyChanged)));
			DataSeries.ZValueFormatStringProperty = DependencyProperty.Register("ZValueFormatString", typeof(string), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnZValueFormatStringPropertyChanged)));
			DataSeries.AxisXTypeProperty = DependencyProperty.Register("AxisXType", typeof(AxisTypes), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnAxisXTypePropertyChanged)));
			DataSeries.AxisYTypeProperty = DependencyProperty.Register("AxisYType", typeof(AxisTypes), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnAxisYTypePropertyChanged)));
			DataSeries.XValueTypeProperty = DependencyProperty.Register("XValueType", typeof(ChartValueTypes), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnXValueTypePropertyChanged)));
			DataSeries.SelectionEnabledProperty = DependencyProperty.Register("SelectionEnabled", typeof(bool), typeof(DataSeries), new PropertyMetadata(false, new PropertyChangedCallback(DataSeries.OnSelectionEnabledPropertyChanged)));
			DataSeries.SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(SelectionModes), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnSelectionModePropertyChanged)));
			DataSeries.ZIndexProperty = DependencyProperty.Register("ZIndex", typeof(int), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnZIndexPropertyChanged)));
			DataSeries.NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DataSeries), new PropertyMetadata(string.Empty));
			DataSeries.DataPointsProperty = DependencyProperty.Register("DataPoints", typeof(DataPointCollection), typeof(DataSeries), new PropertyMetadata(new PropertyChangedCallback(DataSeries.OnDataPointsPropertyChanged)));
			if (!DataSeries._defaultStyleKeyApplied)
			{
				FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DataSeries), new FrameworkPropertyMetadata(typeof(DataSeries)));
				DataSeries._defaultStyleKeyApplied = true;
			}
		}

		public void HideMovingMarker()
		{
			if (this._movingMarker != null)
			{
				this._movingMarker.Visibility = Visibility.Collapsed;
			}
		}

		internal void ShowMovingMarker(double internalXValue, double internalYValue)
		{
			if (this.MovingMarkerEnabled)
			{
				Chart chart = base.Chart as Chart;
				if (chart != null && chart.ChartArea != null && chart.ChartArea.AxisX != null)
				{
					Axis arg_54_0 = chart.ChartArea.AxisX;
					Axis arg_60_0 = this.PlotGroup.AxisY;
					if (!double.IsNaN(internalYValue) && !double.IsNaN(internalXValue))
					{
						base.Dispatcher.BeginInvoke(new Action(delegate
						{
							IDataPoint nearestDataPoint = RenderHelper.GetNearestDataPoint(this, internalXValue, internalYValue);
							if (this.RenderAs == RenderAs.Line || this.RenderAs == RenderAs.Spline)
							{
								LineChart.ShowMovingMarkerAtDataPoint(this, nearestDataPoint);
								return;
							}
							StepLineChart.ShowMovingMarkerAtDataPoint(this, nearestDataPoint);
						}), new object[0]);
					}
				}
			}
		}

		public void ShowMovingMarker(object xValue, double yValue)
		{
			if (this.MovingMarkerEnabled)
			{
				Chart chart = base.Chart as Chart;
				if (chart != null && chart.ChartArea != null && chart.ChartArea.AxisX != null)
				{
					Axis axisX = chart.ChartArea.AxisX;
					Axis axisY = this.PlotGroup.AxisY;
					double num;
					double internalXValue;
					RenderHelper.UserValueToInternalValues(axisX, axisY, xValue, yValue, out internalXValue, out num);
					if (!double.IsNaN(yValue) && xValue != null)
					{
						base.Dispatcher.BeginInvoke(new Action(delegate
						{
							if (this._movingMarker != null)
							{
								this._movingMarker.Visibility = Visibility.Visible;
							}
							IDataPoint nearestDataPoint = RenderHelper.GetNearestDataPoint(this, internalXValue, yValue);
							if (this.RenderAs == RenderAs.Line || this.RenderAs == RenderAs.Spline)
							{
								LineChart.ShowMovingMarkerAtDataPoint(this, nearestDataPoint);
								return;
							}
							StepLineChart.ShowMovingMarkerAtDataPoint(this, nearestDataPoint);
						}), new object[0]);
					}
				}
			}
		}

		private void DataMappings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				using (IEnumerator<IDataPoint> enumerator = this.DataPoints.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IDataPoint current = enumerator.Current;
						foreach (DataMapping dm in e.NewItems)
						{
							DataPointHelper.UpdateBindProperty(current, dm, current.DataContext);
						}
					}
					return;
				}
			}
			if (e.OldItems != null)
			{
				foreach (IDataPoint current2 in this.DataPoints)
				{
					foreach (DataMapping dm2 in e.OldItems)
					{
						DataPointHelper.ResetBindProperty(current2, dm2, current2.DataContext);
					}
				}
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this._rootElement = (base.GetTemplateChild("RootElement") as Canvas);
			if (!this.LightWeight.Value)
			{
				this.AddChartElementsToRootElement();
			}
		}

		private void AddChartElementsToRootElement()
		{
			if (this.DataPoints != null)
			{
				foreach (IDataPoint current in this.DataPoints)
				{
					this.AddDataPointToRootElement(current);
				}
			}
		}

		internal void AddDataPointToRootElement(IDataPoint dataPoint)
		{
			if (dataPoint.IsLightDataPoint)
			{
				return;
			}
			DataPoint dataPoint2 = dataPoint as DataPoint;
			if (this._rootElement == null)
			{
				return;
			}
			dataPoint2.DpInfo.IsNotificationEnable = false;
			if (base.IsInDesignMode)
			{
				ObservableObject.RemoveElementFromElementTree(dataPoint2);
			}
			if (dataPoint2.ParentTree == null)
			{
				this._rootElement.Children.Add(dataPoint2);
			}
			dataPoint2.DpInfo.IsNotificationEnable = true;
			dataPoint2.IsTabStop = false;
		}

		private static void OnDataSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			((DataSeries)o).OnDataSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
		}

		private void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
		{
			INotifyCollectionChanged notifyCollectionChanged = oldValue as INotifyCollectionChanged;
			if (notifyCollectionChanged != null && this._weakEventListener != null)
			{
				this._weakEventListener.Detach();
				this._weakEventListener = null;
			}
			INotifyCollectionChanged newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
			if (newValueINotifyCollectionChanged != null)
			{
				this._weakEventListener = new WeakEventListener<DataSeries, object, NotifyCollectionChangedEventArgs>(this);
				this._weakEventListener.OnEventAction = delegate(DataSeries instance, object source, NotifyCollectionChangedEventArgs eventArgs)
				{
					this.DataSource_CollectionChanged(source, eventArgs);
				};
				this._weakEventListener.OnDetachAction = delegate(WeakEventListener<DataSeries, object, NotifyCollectionChangedEventArgs> weakEventListener)
				{
					newValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(weakEventListener.OnEvent);
				};
				newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this._weakEventListener.OnEvent);
			}
			if (base.IsNotificationEnable)
			{
				this.BindData();
			}
		}

		private void DataSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				this.BindData();
				return;
			}
			List<IDataPoint> list = null;
			if (e.OldItems != null)
			{
				list = (from dp in this.DataPoints
				where dp.DataContext != null && e.OldItems.Contains(dp.DataContext)
				select dp).ToList<IDataPoint>();
				foreach (IDataPoint current in list)
				{
					current.DpFunc.DetachEventFromWeakEventListner();
				}
			}
			if (e.NewItems != null)
			{
				foreach (object current2 in e.NewItems)
				{
					IDataPoint arg_E6_0;
					if (!this.LightWeight.Value)
					{
						IDataPoint dataPoint = new DataPoint();
						arg_E6_0 = dataPoint;
					}
					else
					{
						arg_E6_0 = new LightDataPoint();
					}
					IDataPoint dataPoint2 = arg_E6_0;
					dataPoint2.DataContext = current2;
					if (this.DataMappings != null)
					{
						try
						{
							DataPointHelper.BindData(dataPoint2, current2, this.DataMappings);
						}
						catch
						{
							throw new Exception("Error While Mapping Data: Please Verify that you are mapping the Data Correctly");
						}
					}
					if (e.NewStartingIndex == this.DataPoints.Count + 1)
					{
						this.DataPoints.Add(dataPoint2);
					}
					else
					{
						this.DataPoints.Insert(e.NewStartingIndex, dataPoint2);
					}
				}
			}
			if (list != null)
			{
				foreach (IDataPoint current3 in list)
				{
					this.DataPoints.Remove(current3);
				}
				list = null;
			}
			if (base.Chart != null && base.Chart._toolTip != null)
			{
				base.Chart._toolTip.Hide();
			}
		}

		internal override void Bind()
		{
		}

		internal void BindData()
		{
			if (this.DataPoints.Count > 0)
			{
				if (!this.LightWeight.Value)
				{
					foreach (IDataPoint current in this.DataPoints)
					{
						if (this._rootElement != null)
						{
							DataPoint dataPoint = current as DataPoint;
							if (dataPoint != null)
							{
								ObservableObject.RemoveElementFromElementTree(dataPoint);
							}
						}
					}
				}
				this.DataPoints.Clear();
			}
			if (this.DataSource != null && this.DataSource != null)
			{
				IEnumerable dataSource = this.DataSource;
				foreach (object current2 in dataSource)
				{
					IDataPoint arg_C0_0;
					if (!this.LightWeight.Value)
					{
						IDataPoint dataPoint2 = new DataPoint();
						arg_C0_0 = dataPoint2;
					}
					else
					{
						arg_C0_0 = new LightDataPoint();
					}
					IDataPoint dataPoint3 = arg_C0_0;
					dataPoint3.DataContext = current2;
					if (this.DataMappings != null)
					{
						try
						{
							DataPointHelper.BindData(dataPoint3, current2, this.DataMappings);
						}
						catch
						{
							throw new Exception("Error While Mapping Data: Please Verify that you are mapping the Data Correctly");
						}
					}
					this.DataPoints.Add(dataPoint3);
				}
			}
		}

		internal void PartialUpdateOfColorProperty(Brush value)
		{
			Brush color = this.Color;
			if (this.RenderAs == RenderAs.StackedArea || this.RenderAs == RenderAs.StackedArea100)
			{
				if (this.Faces == null || this.Faces.Parts == null)
				{
					goto IL_3E0;
				}
				if ((base.Chart as Chart).View3D)
				{
					Brush fill = this.LightingEnabled.Value ? Graphics.GetRightFaceBrush(color) : color;
					Brush fill2 = this.LightingEnabled.Value ? Graphics.GetTopFaceBrush(color) : color;
					using (List<DependencyObject>.Enumerator enumerator = this.Faces.Parts.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							FrameworkElement frameworkElement = (FrameworkElement)enumerator.Current;
							ElementData elementData = frameworkElement.Tag as ElementData;
							if (elementData != null)
							{
								if (elementData.VisualElementName == "AreaBase")
								{
									(frameworkElement as Shape).Fill = (this.LightingEnabled.Value ? Graphics.GetFrontFaceBrush(color) : color);
								}
								else if (elementData.VisualElementName == "Side")
								{
									(frameworkElement as Shape).Fill = fill;
								}
								else if (elementData.VisualElementName == "Top")
								{
									(frameworkElement as Shape).Fill = fill2;
								}
							}
						}
						goto IL_20B;
					}
				}
				using (List<DependencyObject>.Enumerator enumerator2 = this.Faces.Parts.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						FrameworkElement frameworkElement2 = (FrameworkElement)enumerator2.Current;
						ElementData elementData2 = frameworkElement2.Tag as ElementData;
						if (elementData2 != null)
						{
							if (elementData2.VisualElementName == "AreaBase")
							{
								(frameworkElement2 as Shape).Fill = (this.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(color, "Linear", null) : color);
							}
							else if (elementData2.VisualElementName == "Bevel")
							{
								(frameworkElement2 as Shape).Fill = Graphics.GetBevelTopBrush(color);
							}
						}
					}
				}
				IL_20B:
				using (List<IDataPoint>.Enumerator enumerator3 = this.InternalDataPoints.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						IDataPoint current = enumerator3.Current;
						DataPointHelper.PartialUpdateOfColorProperty(current, current.Color);
					}
					goto IL_3E0;
				}
			}
			if (this.RenderAs == RenderAs.Radar)
			{
				if (this.Faces != null && this.Faces.Visual != null)
				{
					(this.Faces.Visual as Polygon).Fill = (this.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(color, "Linear", null) : color);
				}
				using (List<IDataPoint>.Enumerator enumerator4 = this.InternalDataPoints.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						IDataPoint current2 = enumerator4.Current;
						DataPointHelper.PartialUpdateOfColorProperty(current2, current2.Color);
					}
					goto IL_3E0;
				}
			}
			if (this.RenderAs == RenderAs.Polar)
			{
				if (this.Faces != null)
				{
					using (List<DependencyObject>.Enumerator enumerator5 = this.Faces.Parts.GetEnumerator())
					{
						while (enumerator5.MoveNext())
						{
							FrameworkElement frameworkElement3 = (FrameworkElement)enumerator5.Current;
							(frameworkElement3 as Path).Stroke = (this.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(color, "Linear", null) : color);
						}
					}
				}
				using (List<IDataPoint>.Enumerator enumerator6 = this.InternalDataPoints.GetEnumerator())
				{
					while (enumerator6.MoveNext())
					{
						IDataPoint current3 = enumerator6.Current;
						DataPointHelper.PartialUpdateOfColorProperty(current3, current3.Color);
					}
					goto IL_3E0;
				}
			}
			foreach (IDataPoint current4 in this.InternalDataPoints)
			{
				DataPointHelper.PartialUpdateOfColorProperty(current4, current4.Color);
			}
			IL_3E0:
			this.UpdateLegendMarker();
		}

		internal void UpdateLegendMarker()
		{
			if (this.LegendMarker != null && this.LegendMarker.Visual != null && this.RenderAs != RenderAs.CandleStick)
			{
				if (this.LegendMarkerColor == null)
				{
					if (RenderHelper.IsLineCType(this))
					{
						this.LegendMarker.BorderColor = (this.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(this.Color, "Linear", new double[]
						{
							0.65,
							0.55
						}) : this.Color);
						this.LegendMarker.MarkerFillColor = new SolidColorBrush(Colors.White);
						if (this.LegendMarker.Visual.Parent != null && (this.LegendMarker.Visual.Parent as Canvas).Children[0] is Line)
						{
							((this.LegendMarker.Visual.Parent as Canvas).Children[0] as Line).Stroke = (this.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(this.Color, "Linear", new double[]
							{
								0.65,
								0.55
							}) : this.Color);
						}
					}
					else
					{
						this.LegendMarker.BorderColor = this.Color;
						this.LegendMarker.MarkerFillColor = this.Color;
					}
					this.LegendMarker.UpdateMarker();
					return;
				}
				if (RenderHelper.IsLineCType(this))
				{
					this.LegendMarker.BorderColor = (this.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(this.LegendMarkerColor, "Linear", new double[]
					{
						0.65,
						0.55
					}) : this.LegendMarkerColor);
					this.LegendMarker.MarkerFillColor = new SolidColorBrush(Colors.White);
					if (this.LegendMarker.Visual.Parent != null && (this.LegendMarker.Visual.Parent as Canvas).Children[0] is Line)
					{
						((this.LegendMarker.Visual.Parent as Canvas).Children[0] as Line).Stroke = (this.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(this.LegendMarkerColor, "Linear", new double[]
						{
							0.65,
							0.55
						}) : this.LegendMarkerColor);
					}
				}
				else
				{
					this.LegendMarker.BorderColor = this.LegendMarkerColor;
					this.LegendMarker.MarkerFillColor = this.LegendMarkerColor;
				}
				this.LegendMarker.UpdateMarker();
			}
		}

		internal void SetToolTipProperties(IDataPoint nearestDataPoint)
		{
			Chart chart = base.Chart as Chart;
			string text = nearestDataPoint.ParseToolTipText(nearestDataPoint.ToolTipText);
			DataSeries parent = nearestDataPoint.Parent;
			parent.ToolTipElement.Text = text;
			if (chart.ToolTipEnabled)
			{
				if (parent.ToolTipElement._isAutoGeneratedFontColor)
				{
					parent.ToolTipElement.FontColor = DataSeries.CalculateFontColor(parent.ToolTipElement.Background, parent.Chart as Chart);
					parent.ToolTipElement._isAutoGeneratedFontColor = true;
				}
				parent.ToolTipElement.Show();
			}
		}

		internal bool HideToolTip()
		{
			if (this.ToolTipElement != null)
			{
				this.ToolTipElement.Hide();
				return true;
			}
			return false;
		}

		internal bool ShowToolTip()
		{
			if (this.ToolTipElement != null)
			{
				this.ToolTipElement.Show();
				return true;
			}
			return false;
		}

		internal IDataPoint FindNearestDataPointFromMousePointer(MouseEventArgs e)
		{
			Axis axisX = this.PlotGroup.AxisX;
			Axis axisY = this.PlotGroup.AxisY;
			double xValueAtMousePos = RenderHelper.CalculateInternalXValueFromPixelPos(this.Chart as Chart, axisX, e);
			double yValueAtMousePos = RenderHelper.CalculateInternalYValueFromPixelPos(this.Chart as Chart, axisY, e);
			this._nearestDataPoint = RenderHelper.GetNearestDataPoint(this, xValueAtMousePos, yValueAtMousePos);
			if (this._nearestDataPoint == null)
			{
				this.HideToolTip();
			}
			return this._nearestDataPoint;
		}

		internal IDataPoint FindNearestDataPointFromValues(double internalXValue, double internalYValue)
		{
			this._nearestDataPoint = RenderHelper.GetNearestDataPoint(this, internalXValue, internalYValue);
			return this._nearestDataPoint;
		}

		internal IDataPoint GetNearestDataPointAlongYPosition(MouseEventArgs e, List<IDataPoint> listOfDataPoints)
		{
			IDataPoint dataPoint = null;
			Chart chart = base.Chart as Chart;
			if (chart.ChartArea.AxisX != null)
			{
				AxisOrientation axisOrientation = chart.ChartArea.AxisX.AxisOrientation;
				double num = (axisOrientation == AxisOrientation.Horizontal) ? e.GetPosition(chart.ChartArea.PlottingCanvas).Y : e.GetPosition(chart.ChartArea.PlottingCanvas).X;
				if (axisOrientation != AxisOrientation.Horizontal)
				{
					double arg_7E_0 = chart.ChartArea.ChartVisualCanvas.Width;
				}
				else
				{
					double arg_91_0 = chart.ChartArea.ChartVisualCanvas.Height;
				}
				Dictionary<IDataPoint, double> dictionary = new Dictionary<IDataPoint, double>();
				foreach (IDataPoint current in listOfDataPoints)
				{
					DataSeries arg_B6_0 = current.Parent;
					if (current.Enabled.Value)
					{
						if (chart.ChartArea.AxisX.AxisOrientation == AxisOrientation.Horizontal)
						{
							double value = Math.Abs(num - current.DpInfo.VisualPosition.Y);
							dictionary.Add(current, value);
							if (dataPoint == null)
							{
								dataPoint = current;
							}
							else if (dictionary[current] < dictionary[dataPoint])
							{
								dataPoint = current;
							}
						}
						else
						{
							double value2 = Math.Abs(num - current.DpInfo.VisualPosition.X);
							dictionary.Add(current, value2);
							if (dataPoint == null)
							{
								dataPoint = current;
							}
							else if (dictionary[current] < dictionary[dataPoint])
							{
								dataPoint = current;
							}
						}
					}
				}
			}
			return dataPoint;
		}

		internal void StopDataPointsAnimation()
		{
			if (base.Chart != null && (base.Chart as Chart).AnimatedUpdate.Value)
			{
				foreach (IDataPoint current in this.InternalDataPoints)
				{
					if (current.DpInfo.Storyboard != null)
					{
						current.DpInfo.Storyboard.Stop();
					}
				}
			}
		}

		internal void StopDataSeriesAnimation()
		{
			if (base.Chart != null && (base.Chart as Chart).AnimatedUpdate.Value && this.Storyboard != null)
			{
				this.Storyboard.Stop();
			}
		}

		internal override void UpdateVisual(VcProperties property, object newValue)
		{
			if (!base.IsNotificationEnable || (base.Chart != null && (double.IsNaN((base.Chart as Chart).ActualWidth) || double.IsNaN((base.Chart as Chart).ActualHeight) || (base.Chart as Chart).ActualWidth == 0.0 || (base.Chart as Chart).ActualHeight == 0.0)))
			{
				return;
			}
			if (base.IsInDesignMode)
			{
				if (base.Chart != null)
				{
					(base.Chart as Chart).RENDER_LOCK = false;
				}
				base.FirePropertyChanged(property);
				return;
			}
			if (property != VcProperties.ScrollBarScale && base.Chart != null && (base.Chart as Chart).IS_FULL_RENDER_PENDING)
			{
				return;
			}
			if (ObservableObject.ValidatePartialUpdate(this, property))
			{
				if (ObservableObject.NonPartialUpdateChartTypes(this.RenderAs) && property != VcProperties.ScrollBarScale)
				{
					if (property == VcProperties.Color)
					{
						this.PartialUpdateOfColorProperty((Brush)newValue);
						return;
					}
					base.FirePropertyChanged(property);
					return;
				}
				else
				{
					Chart chart = base.Chart as Chart;
					if (chart == null)
					{
						return;
					}
					chart._internalPartialUpdateEnabled = true;
					if (chart.ChartArea != null && (chart.ChartArea._plotAreaSizeAfterDrawingAxes.Width == 0.0 || chart.ChartArea._plotAreaSizeAfterDrawingAxes.Height == 0.0))
					{
						return;
					}
					bool flag = false;
					this._isZooming = false;
					if (property == VcProperties.LegendMarkerType)
					{
						base.FirePropertyChanged(property);
						return;
					}
					if (chart.ChartArea != null)
					{
						chart.ChartArea._axisUpdated = false;
						chart.ChartArea._plotDetailsReCreated = false;
						chart.ChartArea._seriesListUpdated = false;
					}
					if (property == VcProperties.LegendMarkerColor)
					{
						if (this.RenderAs != RenderAs.CandleStick)
						{
							this.UpdateLegendMarker();
							return;
						}
						if (this.LegendMarker != null && this.LegendMarker.Visual != null)
						{
							if (newValue == null)
							{
								this.LegendMarker.BorderColor = this.PriceUpColor;
							}
							else
							{
								this.LegendMarker.BorderColor = (Brush)newValue;
								this.LegendMarker.MarkerFillColor = new SolidColorBrush(Colors.White);
								if (this.LegendMarker.Visual.Parent != null && (this.LegendMarker.Visual.Parent as Canvas).Children[0] is Line)
								{
									((this.LegendMarker.Visual.Parent as Canvas).Children[0] as Line).Stroke = (Brush)newValue;
								}
							}
							this.LegendMarker.UpdateMarker();
							return;
						}
					}
					else if (property == VcProperties.ColorSet)
					{
						Brush newValue2 = null;
						(base.Chart as Chart).ChartArea.LoadSeriesColorSet4SingleSeries(this);
						if (RenderHelper.IsRasterRenderSupported(this))
						{
							RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
							using (List<IDataPoint>.Enumerator enumerator = this.InternalDataPoints.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									IDataPoint current = enumerator.Current;
									if (this.RenderAs != RenderAs.CandleStick)
									{
										DataPointHelper.UpdateLegendMarker(current, current.Color);
									}
								}
								goto IL_3B0;
							}
						}
						if (RenderHelper.IsLineCType(this) || this.RenderAs == RenderAs.Area)
						{
							newValue2 = this.Color;
							RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, VcProperties.Color, newValue2, flag));
						}
						foreach (IDataPoint current2 in this.InternalDataPoints)
						{
							if (!string.IsNullOrEmpty(this.ColorSet) && DataPointHelper.GetColor_DataPoint(current2) == null)
							{
								newValue2 = current2.DpInfo.InternalColor;
							}
							RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, current2, ElementTypes.DataPoint, VcProperties.Color, newValue2, flag));
							if (this.RenderAs != RenderAs.CandleStick)
							{
								DataPointHelper.UpdateLegendMarker(current2, current2.Color);
							}
						}
						IL_3B0:
						if (this.RenderAs != RenderAs.CandleStick)
						{
							this.UpdateLegendMarker();
							return;
						}
					}
					else
					{
						if (property == VcProperties.Color)
						{
							if (this.RenderAs != RenderAs.CandleStick)
							{
								this.UpdateLegendMarker();
							}
							if (RenderHelper.IsRasterRenderSupported(this))
							{
								foreach (IDataPoint current3 in this.InternalDataPoints)
								{
									current3.DpInfo.InternalColor = (newValue as Brush);
									if (this.RenderAs != RenderAs.CandleStick)
									{
										DataPointHelper.UpdateLegendMarker(current3, (Brush)newValue);
									}
								}
								RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
								return;
							}
							if (RenderHelper.IsLineCType(this) || this.RenderAs == RenderAs.Area)
							{
								RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, VcProperties.Color, newValue, flag));
							}
							using (List<IDataPoint>.Enumerator enumerator4 = this.InternalDataPoints.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									IDataPoint current4 = enumerator4.Current;
									RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, current4, ElementTypes.DataPoint, VcProperties.Color, newValue, flag));
									if (this.RenderAs != RenderAs.CandleStick)
									{
										DataPointHelper.UpdateLegendMarker(current4, (Brush)newValue);
									}
								}
								return;
							}
						}
						if (property == VcProperties.Effect || property == VcProperties.ShadowEnabled || property == VcProperties.Opacity)
						{
							if (RenderHelper.IsRasterRenderSupported(this))
							{
								RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
								return;
							}
							if (RenderHelper.IsLineCType(this) || this.RenderAs == RenderAs.Area)
							{
								RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
							}
							using (List<IDataPoint>.Enumerator enumerator5 = this.InternalDataPoints.GetEnumerator())
							{
								while (enumerator5.MoveNext())
								{
									IDataPoint current5 = enumerator5.Current;
									RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, current5, ElementTypes.DataPoint, property, newValue, flag));
								}
								return;
							}
						}
						if (property == VcProperties.LineStyle || property == VcProperties.LineThickness || property == VcProperties.LightingEnabled)
						{
							if (RenderHelper.IsRasterRenderSupported(this))
							{
								RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
								return;
							}
							bool flag2 = RenderHelper.IsLineCType(this);
							if (property == VcProperties.LightingEnabled && this.RenderAs != RenderAs.CandleStick)
							{
								this.UpdateLegendMarker();
							}
							if (flag2 || this.RenderAs == RenderAs.Area)
							{
								RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
							}
							if (flag2 && property == VcProperties.LineThickness)
							{
								using (List<IDataPoint>.Enumerator enumerator6 = this.InternalDataPoints.GetEnumerator())
								{
									while (enumerator6.MoveNext())
									{
										IDataPoint current6 = enumerator6.Current;
										RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, current6, ElementTypes.DataPoint, VcProperties.MarkerSize, newValue, flag));
									}
									return;
								}
							}
							using (List<IDataPoint>.Enumerator enumerator7 = this.InternalDataPoints.GetEnumerator())
							{
								while (enumerator7.MoveNext())
								{
									IDataPoint current7 = enumerator7.Current;
									RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, current7, ElementTypes.DataPoint, property, newValue, flag));
									if (property == VcProperties.LightingEnabled && this.RenderAs != RenderAs.CandleStick)
									{
										DataPointHelper.UpdateLegendMarker(current7, null);
									}
								}
								return;
							}
						}
						if (property == VcProperties.BorderColor || property == VcProperties.BorderThickness || property == VcProperties.BorderStyle)
						{
							if (RenderHelper.IsRasterRenderSupported(this))
							{
								RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
								return;
							}
							if (this.RenderAs == RenderAs.Area)
							{
								RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
							}
							using (List<IDataPoint>.Enumerator enumerator8 = this.InternalDataPoints.GetEnumerator())
							{
								while (enumerator8.MoveNext())
								{
									IDataPoint current8 = enumerator8.Current;
									RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, current8, ElementTypes.DataPoint, property, newValue, flag));
								}
								return;
							}
						}
						if (property == VcProperties.ViewportRangeEnabled && (bool)newValue)
						{
							flag = true;
							RenderHelper.UpdateVisualObject(new RenderHelperInfo(chart, property, newValue, false, false, true));
							return;
						}
						if (property == VcProperties.DataPointUpdate || property == VcProperties.DataPoints || property == VcProperties.Enabled || property == VcProperties.ScrollBarScale || property == VcProperties.AxisMinimum || property == VcProperties.AxisMaximum)
						{
							if (property != VcProperties.DataPointUpdate && !RenderHelper.IsRasterRenderSupported(this) && this.InternalDataPoints != null)
							{
								foreach (IDataPoint current9 in this.InternalDataPoints)
								{
									if (current9.DpInfo.Marker != null && current9.DpInfo.Marker.Visual != null)
									{
										current9.DpInfo.Marker.Visual.Visibility = Visibility.Collapsed;
									}
									if (current9.DpInfo.LabelVisual != null)
									{
										current9.DpInfo.LabelVisual.Visibility = Visibility.Collapsed;
									}
								}
							}
							AxisRepresentations renderAxisType = AxisRepresentations.AxisX;
							if ((chart.ZoomingEnabled && property == VcProperties.ScrollBarScale) || property == VcProperties.AxisMinimum || property == VcProperties.AxisMaximum)
							{
								flag = true;
								this._isZooming = true;
								property = VcProperties.DataPoints;
							}
							else
							{
								if (chart.PlotDetails != null && property == VcProperties.DataPoints && newValue != null && this.PlotGroup != null && this.PlotGroup.RenderAs != this.RenderAs)
								{
									PartialUpdateConfiguration partialUpdateConfiguration = new PartialUpdateConfiguration
									{
										Sender = this,
										ElementType = ElementTypes.Chart,
										Property = VcProperties.Series,
										OldValue = null,
										NewValue = newValue,
										IsUpdateLists = false,
										IsCalculatePlotDetails = true,
										IsUpdateAxis = false,
										RenderAxisType = renderAxisType,
										IsPartialUpdate = true
									};
									chart.ChartArea.PrePartialUpdateConfiguration(partialUpdateConfiguration);
								}
								if (chart.PlotDetails == null || this.PlotGroup == null)
								{
									return;
								}
								this.CheckWhetherToRenderAxis(chart, property, newValue, ref flag, ref renderAxisType);
							}
							if (!this._isZooming)
							{
								bool isUpdateAxis = flag;
								if (flag && property == VcProperties.DataPoints)
								{
									isUpdateAxis = false;
								}
								PartialUpdateConfiguration partialUpdateConfiguration2 = new PartialUpdateConfiguration
								{
									Sender = this,
									ElementType = ElementTypes.DataSeries,
									Property = property,
									OldValue = null,
									NewValue = newValue,
									IsUpdateLists = false,
									IsCalculatePlotDetails = false,
									IsUpdateAxis = isUpdateAxis,
									RenderAxisType = renderAxisType,
									IsPartialUpdate = true
								};
								chart.ChartArea.PrePartialUpdateConfiguration(partialUpdateConfiguration2);
							}
							if (property == VcProperties.DataPointUpdate && !this._isZooming)
							{
								if (flag)
								{
									using (List<DataSeries>.Enumerator enumerator10 = chart.InternalSeries.GetEnumerator())
									{
										while (enumerator10.MoveNext())
										{
											DataSeries current10 = enumerator10.Current;
											if (RenderHelper.IsRasterRenderSupported(this))
											{
												RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
											}
											else if (current10.RenderAs == RenderAs.Spline || current10.RenderAs == RenderAs.QuickLine)
											{
												RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
											}
											else
											{
												foreach (IDataPoint current11 in current10.InternalDataPoints)
												{
													DataPointHelper.UpdateVisual(current11, VcProperties.YValue, current11.DpInfo.InternalYValue, false);
												}
											}
										}
										return;
									}
								}
								RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
								return;
							}
							if (flag)
							{
								RenderHelper.UpdateVisualObject(new RenderHelperInfo(chart, property, newValue, false, false, this._isZooming));
								return;
							}
							this.ClearOlderVisuals();
							RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
							return;
						}
						else
						{
							if (property == VcProperties.AxisXType || property == VcProperties.AxisXType || property == VcProperties.Enabled)
							{
								flag = true;
								PartialUpdateConfiguration partialUpdateConfiguration3 = new PartialUpdateConfiguration
								{
									Sender = this,
									ElementType = ElementTypes.DataSeries,
									Property = property,
									OldValue = null,
									NewValue = newValue,
									IsUpdateLists = true,
									IsCalculatePlotDetails = true,
									IsUpdateAxis = flag,
									RenderAxisType = AxisRepresentations.AxisX,
									IsPartialUpdate = true
								};
								chart.ChartArea.PrePartialUpdateConfiguration(partialUpdateConfiguration3);
								RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
								return;
							}
							if (RenderHelper.IsRasterRenderSupported(this))
							{
								RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, this, ElementTypes.DataSeries, property, newValue, flag));
								if (property != VcProperties.ViewportRangeEnabled)
								{
									return;
								}
								using (List<DataSeries>.Enumerator enumerator12 = chart.InternalSeries.GetEnumerator())
								{
									while (enumerator12.MoveNext())
									{
										DataSeries current12 = enumerator12.Current;
										if (!RenderHelper.IsRasterRenderSupported(current12))
										{
											RenderHelper.UpdateVisualObject(current12.RenderAs, new RenderHelperInfo(chart, current12, ElementTypes.DataSeries, VcProperties.DataPoints, newValue, flag));
										}
									}
									return;
								}
							}
							foreach (IDataPoint current13 in this.InternalDataPoints)
							{
								RenderHelper.UpdateVisualObject(this.RenderAs, new RenderHelperInfo(chart, current13, ElementTypes.DataPoint, property, newValue, flag));
							}
						}
					}
				}
			}
		}

		private void CheckWhetherToRenderAxis(Chart chart, VcProperties property, object newValue, ref bool renderAxis, ref AxisRepresentations axisRepresentation)
		{
			double internalAxisMinimum = chart.ChartArea.AxisY.InternalAxisMinimum;
			double internalAxisMaximum = chart.ChartArea.AxisY.InternalAxisMaximum;
			double internalAxisMinimum2 = chart.ChartArea.AxisX.InternalAxisMinimum;
			double internalAxisMaximum2 = chart.ChartArea.AxisX.InternalAxisMaximum;
			double axisYMaximumDataValue = chart.PlotDetails.GetAxisYMaximumDataValue(this.PlotGroup.AxisY);
			double axisYMinimumDataValue = chart.PlotDetails.GetAxisYMinimumDataValue(this.PlotGroup.AxisY);
			double axisXMaximumDataValue = chart.PlotDetails.GetAxisXMaximumDataValue(this.PlotGroup.AxisX);
			double axisXMinimumDataValue = chart.PlotDetails.GetAxisXMinimumDataValue(this.PlotGroup.AxisX);
			PartialUpdateConfiguration partialUpdateConfiguration = new PartialUpdateConfiguration
			{
				Sender = this,
				ElementType = ElementTypes.DataSeries,
				Property = ((property == VcProperties.DataPointUpdate) ? VcProperties.DataPoints : property),
				OldValue = null,
				NewValue = newValue,
				IsUpdateLists = true,
				IsCalculatePlotDetails = true,
				IsUpdateAxis = false,
				RenderAxisType = AxisRepresentations.AxisX,
				IsPartialUpdate = true
			};
			chart.ChartArea.PrePartialUpdateConfiguration(partialUpdateConfiguration);
			double axisYMaximumDataValue2 = chart.PlotDetails.GetAxisYMaximumDataValue(this.PlotGroup.AxisY);
			double axisYMinimumDataValue2 = chart.PlotDetails.GetAxisYMinimumDataValue(this.PlotGroup.AxisY);
			double axisXMaximumDataValue2 = chart.PlotDetails.GetAxisXMaximumDataValue(this.PlotGroup.AxisX);
			double axisXMinimumDataValue2 = chart.PlotDetails.GetAxisXMinimumDataValue(this.PlotGroup.AxisX);
			if ((chart.ScrollingEnabled.Value || chart.ZoomingEnabled) && this.PlotGroup.AxisY.ViewportRangeEnabled)
			{
				renderAxis = true;
				axisRepresentation = AxisRepresentations.AxisY;
			}
			if (axisYMaximumDataValue2 != axisYMaximumDataValue || axisYMinimumDataValue2 != axisYMinimumDataValue)
			{
				if (this.PlotGroup.AxisY.AxisMinimum != null && this.PlotGroup.AxisY.AxisMaximum != null && property != VcProperties.DataPoints)
				{
					if (axisXMaximumDataValue2 != axisXMaximumDataValue || axisXMinimumDataValue2 != axisXMinimumDataValue)
					{
						if (this.PlotGroup.AxisX.AxisMinimum != null && this.PlotGroup.AxisX.AxisMaximum != null)
						{
							return;
						}
						chart.ChartArea.AxisX.SetUpAxisManager();
						if (internalAxisMinimum2 == chart.ChartArea.AxisX.InternalAxisMinimum && internalAxisMaximum2 == chart.ChartArea.AxisX.InternalAxisMaximum && property != VcProperties.DataPoints)
						{
							return;
						}
						renderAxis = true;
					}
					return;
				}
				chart.ChartArea.AxisY.SetUpAxisManager();
				if (internalAxisMinimum == chart.ChartArea.AxisY.InternalAxisMinimum && internalAxisMaximum == chart.ChartArea.AxisY.InternalAxisMaximum && property != VcProperties.DataPoints)
				{
					return;
				}
				renderAxis = true;
				axisRepresentation = AxisRepresentations.AxisY;
				return;
			}
			else
			{
				if (axisXMaximumDataValue2 == axisXMaximumDataValue && axisXMinimumDataValue2 == axisXMinimumDataValue)
				{
					if (property == VcProperties.DataPoints && newValue == null)
					{
						renderAxis = true;
						axisRepresentation = AxisRepresentations.AxisX;
					}
					return;
				}
				if (this.PlotGroup.AxisX.AxisMinimum != null && this.PlotGroup.AxisX.AxisMaximum != null && property != VcProperties.DataPoints)
				{
					if (axisYMaximumDataValue2 != axisYMaximumDataValue || axisYMinimumDataValue2 != axisYMinimumDataValue)
					{
						if (this.PlotGroup.AxisY.AxisMinimum != null && this.PlotGroup.AxisY.AxisMaximum != null)
						{
							return;
						}
						chart.ChartArea.AxisY.SetUpAxisManager();
						if (internalAxisMinimum == chart.ChartArea.AxisY.InternalAxisMinimum && internalAxisMaximum == chart.ChartArea.AxisY.InternalAxisMaximum && property != VcProperties.DataPoints)
						{
							return;
						}
						renderAxis = true;
						axisRepresentation = AxisRepresentations.AxisY;
					}
					return;
				}
				chart.ChartArea.AxisX.SetUpAxisManager();
				if (internalAxisMinimum2 == chart.ChartArea.AxisX.InternalAxisMinimum && internalAxisMaximum2 == chart.ChartArea.AxisX.InternalAxisMaximum && property != VcProperties.DataPoints)
				{
					return;
				}
				renderAxis = true;
				return;
			}
		}

		internal static Brush CalculateFontColor(Brush color, Chart chart)
		{
			Brush result = color;
			if (color != null && !Graphics.AreBrushesEqual(color, Graphics.TRANSPARENT_BRUSH))
			{
				double brushIntensity = Graphics.GetBrushIntensity(color);
				result = Graphics.GetDefaultFontColor(brushIntensity);
			}
			else if (chart.PlotArea != null)
			{
				if (Graphics.AreBrushesEqual(chart.PlotArea.InternalBackground, Graphics.TRANSPARENT_BRUSH) || chart.PlotArea.InternalBackground == null)
				{
					if (Graphics.AreBrushesEqual(chart.Background, Graphics.TRANSPARENT_BRUSH) || chart.Background == null)
					{
						result = Graphics.BLACK_BRUSH;
					}
					else if (chart.Background.GetType().Name == "ImageBrush")
					{
						result = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 218, 218, 218));
					}
					else
					{
						double brushIntensity = Graphics.GetBrushIntensity(chart.Background);
						result = Graphics.GetDefaultFontColor(brushIntensity);
					}
				}
				else
				{
					double brushIntensity = Graphics.GetBrushIntensity(chart.PlotArea.InternalBackground);
					result = Graphics.GetDefaultFontColor(brushIntensity);
				}
			}
			return result;
		}

		internal void ClearOlderVisuals()
		{
			if (this._oldDataPoints != null)
			{
				while (this._oldDataPoints.Any<DataPointCollection>())
				{
					DataPointCollection dataPointCollection = this._oldDataPoints.Pop();
					foreach (IDataPoint current in dataPointCollection)
					{
						this.RemoveVisualElementsFromTree(current, false);
					}
				}
			}
		}

		private static void OnDataPointsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			DataSeries ds = o as DataSeries;
			Chart chart = ds.Chart as Chart;
			if (e.OldValue != null)
			{
				(e.OldValue as DataPointCollection).CollectionChanged -= new NotifyCollectionChangedEventHandler(ds.DataPoints_CollectionChanged);
				DataPointCollection dataPointCollection = e.OldValue as DataPointCollection;
				foreach (IDataPoint current in dataPointCollection)
				{
					current.PropertyChanged -= new PropertyChangedEventHandler(ds.DataPoint_PropertyChanged);
					DataPointHelper.DetachEventForSelection(current);
					if (!current.DpInfo.LightWeight && ds._rootElement != null)
					{
						DataPoint dataPoint = current as DataPoint;
						if (dataPoint != null)
						{
							ObservableObject.RemoveElementFromElementTree(dataPoint);
						}
					}
				}
				ds._oldDataPoints.Push(dataPointCollection);
				if (e.NewValue == null && ObservableObject.ValidatePartialUpdate(ds, VcProperties.DataPoints))
				{
					ds.DataPointRenderManager(VcProperties.DataPoints, dataPointCollection);
				}
			}
			if (e.NewValue != null)
			{
				DataPointCollection dataPointCollection2 = e.NewValue as DataPointCollection;
				if (dataPointCollection2 != null)
				{
					RenderAs renderAs = ds.RenderAs;
					bool value = ds.LightWeight.Value;
					if (!((bool?)ds.GetValue(DataSeries.LightWeightProperty)).HasValue && !value)
					{
						ds.IsNotificationEnable = false;
						if (dataPointCollection2.Count > 2000)
						{
							ds.LightWeight = new bool?(true);
						}
						ds.IsNotificationEnable = true;
						value = ds.LightWeight.Value;
					}
					bool selectionEnabled = ds.SelectionEnabled;
					foreach (IDataPoint current2 in dataPointCollection2)
					{
						((IDataPointParentSetter)current2).Parent = ds;
						current2.DpInfo.LightWeight = value;
						if (ds.Chart != null)
						{
							current2.Chart = ds.Chart;
						}
						if (!current2.IsLightDataPoint && renderAs != RenderAs.QuickLine)
						{
							DataPointHelper.AttachEventForSelection(current2, selectionEnabled);
						}
						if (!current2.DpInfo.LightWeight)
						{
							current2.PropertyChanged -= new PropertyChangedEventHandler(ds.DataPoint_PropertyChanged);
							current2.PropertyChanged += new PropertyChangedEventHandler(ds.DataPoint_PropertyChanged);
							DataPointHelper.BindStyleAttribute(current2);
							DataPointHelper.AttachEventChanged(current2);
							ds.AddDataPointToRootElement(current2);
						}
					}
				}
				(e.NewValue as DataPointCollection).CollectionChanged += new NotifyCollectionChangedEventHandler(ds.DataPoints_CollectionChanged);
				if (ds.Chart != null && ds.ShowInLegend.Value && (ds.Chart as Chart).Series.Count == 1)
				{
					ds.FirePropertyChanged(VcProperties.ShowInLegend);
					return;
				}
				if (ObservableObject.ValidatePartialUpdate(ds, VcProperties.DataPoints))
				{
					ds.Dispatcher.BeginInvoke(new Action(delegate
					{
						ds.UpdateVisual(VcProperties.DataPoints, e.NewValue);
					}), new object[0]);
					return;
				}
				if (chart != null && chart.ChartArea != null && chart.ChartArea._isFirstTimeRender)
				{
					ds.FirePropertyChanged(VcProperties.DataPoints);
				}
			}
		}

		private static void OnMinPointHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((double)e.NewValue < 0.0 || (double)e.NewValue > 100.0)
			{
				throw new Exception("MinPointHeightProperty value is out of Range. MinPointHeight Property Value must be within the range of 0 to 100.");
			}
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.MinPointHeight);
		}

		private static void OnLineTensionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.LineTension);
		}

		private static void OnIncludeDataPointsInLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.IncludeDataPointsInLegend);
		}

		private static void OnLineFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.LineFill);
		}

		private static void OnLineCapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.LineCap);
		}

		private static void OnAutoFitToPlotAreaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.AutoFitToPlotArea);
		}

		private static void OnIncludeYValueInLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.IncludeYValueInLegend);
		}

		private static void OnIncludePercentageInLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.IncludePercentageInLegend);
		}

		private static void OnFillTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.FillType);
		}

		private static void OnLightWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.LightWeight);
		}

		private static void OnMovingMarkerEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.MovingMarkerEnabled, e.NewValue);
		}

		private static void OnExplodedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			if (dataSeries.Chart != null && (dataSeries.RenderAs == RenderAs.Pie || dataSeries.RenderAs == RenderAs.Doughnut))
			{
				using (List<IDataPoint>.Enumerator enumerator = dataSeries.InternalDataPoints.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IDataPoint current = enumerator.Current;
						if (!DataPointHelper.GetExploded_DataPoint(current).HasValue)
						{
							DataPointHelper.ExplodeOrUnexplodeAnimation(current);
						}
					}
					return;
				}
			}
			dataSeries.FirePropertyChanged(VcProperties.Exploded);
		}

		private static void OnEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.Enabled);
		}

		private static void OnOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.InternalOpacity = (double)e.NewValue;
			dataSeries.UpdateVisual(VcProperties.Opacity, e.NewValue);
		}

		private static void OnRenderAsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries._internalColor = null;
			if (dataSeries.Chart != null)
			{
				if (e.OldValue != null && ((RenderAs)e.OldValue == RenderAs.Bar || (RenderAs)e.OldValue == RenderAs.StackedBar || (RenderAs)e.OldValue == RenderAs.StackedBar100))
				{
					if (e.NewValue != null && (RenderAs)e.NewValue != RenderAs.Bar && (RenderAs)e.NewValue != RenderAs.StackedBar && (RenderAs)e.NewValue != RenderAs.StackedBar100)
					{
						(dataSeries.Chart as Chart)._clearAndResetZoomState = true;
					}
				}
				else if (e.NewValue != null && ((RenderAs)e.NewValue == RenderAs.Bar || (RenderAs)e.NewValue == RenderAs.StackedBar || (RenderAs)e.NewValue == RenderAs.StackedBar100) && e.OldValue != null && (RenderAs)e.OldValue != RenderAs.Bar && (RenderAs)e.OldValue != RenderAs.StackedBar && (RenderAs)e.OldValue != RenderAs.StackedBar100)
				{
					(dataSeries.Chart as Chart)._clearAndResetZoomState = true;
				}
			}
			dataSeries.FirePropertyChanged(VcProperties.RenderAs);
		}

		private void ResetFlags(Chart chart)
		{
			if (chart != null && chart.ChartArea != null && chart.ChartArea.AxisX != null)
			{
				chart.ChartArea.AxisX._oldScrollBarOffsetInPixel = double.NaN;
			}
		}

		private static void OnHrefTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.HrefTarget, e.NewValue);
		}

		private static void OnHrefChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			if (dataSeries.Chart != null && dataSeries.DataPoints != null)
			{
				foreach (IDataPoint current in dataSeries.DataPoints)
				{
					current.DpFunc.OnHrefPropertyChanged(current.ToolTipText);
				}
			}
			dataSeries.UpdateVisual(VcProperties.Href, e.NewValue);
		}

		private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.Color, e.NewValue);
		}

		private static void OnEffectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.Effect, e.NewValue);
		}

		private static void OnStickColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.StickColor, e.NewValue);
		}

		private static void OnPriceUpColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.PriceUpColor);
		}

		private static void OnPriceDownColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.PriceDownColor);
		}

		private static void OnLightingEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LightingEnabled, e.NewValue);
		}

		private static void OnLegendMarkerTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LegendMarkerType, e.NewValue);
		}

		private static void OnShadowEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.ShadowEnabled, e.NewValue);
		}

		private static void OnLegendTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.LegendText);
		}

		private static void OnLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.Legend);
		}

		private static void OnBevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.Bevel, e.NewValue);
		}

		private static void OnLegendMarkerColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LegendMarkerColor, e.NewValue);
		}

		private static void OnColorSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.ColorSet, e.NewValue);
		}

		private static void OnRadiusXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.RadiusX, e.NewValue);
		}

		private static void OnRadiusYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.RadiusY, e.NewValue);
		}

		private static void OnLineThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LineThickness, e.NewValue);
		}

		private static void OnLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LineStyle, e.NewValue);
		}

		private static void OnShowInLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.ShowInLegend);
		}

		private static void OnLabelEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelEnabled, e.NewValue);
		}

		private static void OnLabelTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelText, e.NewValue);
		}

		private static void OnLabelFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelFontFamily, e.NewValue);
		}

		private static void OnLabelFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelFontSize, e.NewValue);
		}

		private static void OnLabelFontColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelFontColor, e.NewValue);
		}

		private static void OnLabelFontWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelFontWeight, e.NewValue);
		}

		private static void OnLabelFontStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelFontStyle, e.NewValue);
		}

		private static void OnLabelBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelBackground, e.NewValue);
		}

		private static void OnLabelAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.LabelAngle);
		}

		private static void OnLabelStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelStyle, e.NewValue);
		}

		private static void OnLabelLineEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelLineEnabled, e.NewValue);
		}

		private static void OnLabelLineColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelLineColor, e.NewValue);
		}

		private static void OnLabelLineThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelLineThickness, e.NewValue);
		}

		private static void OnLabelLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.LabelLineStyle, e.NewValue);
		}

		private static void OnMarkerEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.MarkerEnabled, e.NewValue);
		}

		private static void OnMarkerTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.MarkerType, e.NewValue);
		}

		private static void OnMarkerBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.MarkerBorderThickness, e.NewValue);
		}

		private static void OnMarkerBorderColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.MarkerBorderColor, e.NewValue);
		}

		private static void OnMarkerSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.MarkerSize, e.NewValue);
		}

		private static void OnMarkerColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.MarkerColor, e.NewValue);
		}

		private static void OnMarkerScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.MarkerScale, e.NewValue);
		}

		private static void OnBubbleStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.BubbleStyle, e.NewValue);
		}

		private static void OnStartAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.StartAngle);
		}

		private static void OnBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.InternalBorderThickness = (Thickness)e.NewValue;
			dataSeries.UpdateVisual(VcProperties.BorderThickness, e.NewValue);
		}

		private static void OnBorderColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.BorderColor, e.NewValue);
		}

		private static void OnBorderStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.BorderStyle, e.NewValue);
		}

		private static void OnXValueFormatStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.XValueFormatString, e.NewValue);
		}

		private static void OnYValueFormatStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.YValueFormatString, e.NewValue);
		}

		private static void OnPercentageFormatStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.PercentageFormatString, e.NewValue);
		}

		private static void OnZValueFormatStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.UpdateVisual(VcProperties.ZValueFormatString, e.NewValue);
		}

		private static void OnToolTipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.ToolTipName = Convert.ToString(e.NewValue);
			dataSeries.FirePropertyChanged(VcProperties.ToolTip);
		}

		private static void OnAxisXTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.AxisXType);
		}

		private static void OnAxisYTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.FirePropertyChanged(VcProperties.AxisYType);
		}

		private static void OnXValueTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.InternalXValueType = (ChartValueTypes)e.NewValue;
			dataSeries.FirePropertyChanged(VcProperties.XValueType);
		}

		private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			if ((SelectionModes)e.NewValue == SelectionModes.Single)
			{
				Visifire.Charts.Chart.SelectDataPoints(dataSeries.Chart as Chart);
			}
		}

		private static void OnSelectionEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			if (dataSeries.SelectionEnabled)
			{
				if (dataSeries.InternalDataPoints == null)
				{
					goto IL_93;
				}
				bool selectionEnabled = dataSeries.SelectionEnabled;
				using (List<IDataPoint>.Enumerator enumerator = dataSeries.InternalDataPoints.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IDataPoint current = enumerator.Current;
						DataPointHelper.AttachEventForSelection(current, selectionEnabled);
					}
					goto IL_93;
				}
			}
			if (dataSeries.InternalDataPoints != null)
			{
				foreach (IDataPoint current2 in dataSeries.InternalDataPoints)
				{
					DataPointHelper.DetachEventForSelection(current2);
				}
			}
			IL_93:
			if (!dataSeries.SelectionEnabled)
			{
				foreach (IDataPoint current3 in dataSeries.InternalDataPoints)
				{
					DataPointHelper.DeSelect(current3, false, true);
				}
			}
			dataSeries.AttachOrDetachIntaractivity();
			if (e.NewValue != null && (bool)e.NewValue)
			{
				dataSeries.UpdateVisual(VcProperties.DataPoints, null);
			}
		}

		private static void OnZIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataSeries dataSeries = d as DataSeries;
			dataSeries.InternalZIndex = (int)e.NewValue;
			dataSeries.FirePropertyChanged(VcProperties.ZIndex);
		}

		private void DataPoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems == null)
				{
					goto IL_375;
				}

                foreach (IDataPoint dataPoint in e.NewItems)
                {
                    ((IDataPointParentSetter)dataPoint).Parent = this;
                    dataPoint.DpInfo.LightWeight = this.LightWeight.Value;
                    if (base.Chart != null)
                    {
                        dataPoint.Chart = base.Chart;
                    }
                    if (!dataPoint.IsLightDataPoint)
                    {
                        bool selectionEnabled = this.SelectionEnabled;
                        DataPointHelper.AttachEventForSelection(dataPoint, selectionEnabled);
                    }
                    if (!dataPoint.DpInfo.LightWeight)
                    {
                        dataPoint.PropertyChanged -= new PropertyChangedEventHandler(this.DataPoint_PropertyChanged);
                        dataPoint.PropertyChanged += new PropertyChangedEventHandler(this.DataPoint_PropertyChanged);
                        DataPointHelper.BindStyleAttribute(dataPoint);
                        DataPointHelper.AttachEventChanged(dataPoint);
                        this.AddDataPointToRootElement(dataPoint);
                    }
                }

                goto IL_375;
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				if (e.OldItems != null)
				{
					foreach (IDataPoint dataPoint2 in e.OldItems)
					{
						DataPointHelper.DetachEventForSelection(dataPoint2);
						dataPoint2.DpInfo.Faces = null;
						dataPoint2.DpInfo.Marker = null;
						dataPoint2.DpInfo.LabelVisual = null;
						dataPoint2.PropertyChanged -= new PropertyChangedEventHandler(this.DataPoint_PropertyChanged);
					}
				}
				if (e.NewItems != null)
				{
					bool selectionEnabled2 = this.SelectionEnabled;
					foreach (IDataPoint dp in e.NewItems)
					{
						DataPointHelper.AttachEventForSelection(dp, selectionEnabled2);
					}
				}
				foreach (IDataPoint current in this.InternalDataPoints)
				{
					if (this._rootElement != null)
					{
						DataPoint dataPoint3 = current as DataPoint;
						if (dataPoint3 != null)
						{
							ObservableObject.RemoveElementFromElementTree(dataPoint3);
						}
					}
					this.RemoveVisualElementsFromTree(current, true);
				}
				this.InternalDataPoints.Clear();
				if (ObservableObject.ValidatePartialUpdate(this, VcProperties.DataPoints))
				{
					this.DataPointRenderManager(VcProperties.DataPoints, e.NewItems);
					return;
				}
				base.FirePropertyChanged(VcProperties.DataPoints);
				return;
			}
			else
			{
				if (e.Action == NotifyCollectionChangedAction.Remove)
				{
					if (e.OldItems == null)
					{
						goto IL_375;
					}

                    foreach (IDataPoint dataPoint4 in e.OldItems)
                    {
                        if (dataPoint4.Parent != null)
                        {
                            DataSeries parent = dataPoint4.Parent;
                            parent.InternalDataPoints.Remove(dataPoint4);
                        }
                        dataPoint4.PropertyChanged -= new PropertyChangedEventHandler(this.DataPoint_PropertyChanged);
                        DataPointHelper.DetachEventForSelection(dataPoint4);
                        this.RemoveVisualElementsFromTree(dataPoint4, true);
                        if (!this.LightWeight.Value && this._rootElement != null)
                        {
                            DataPoint dataPoint5 = dataPoint4 as DataPoint;
                            if (dataPoint5 != null)
                            {
                                ObservableObject.RemoveElementFromElementTree(dataPoint5);
                            }
                        }
                    }
                    goto IL_375;
				}
				if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems != null)
				{
					foreach (DataPoint dataPoint6 in e.NewItems)
					{
						dataPoint6.Chart = base.Chart;
					}
				}
			}
			IL_375:
			if (base.Chart != null && this.ShowInLegend.Value && (base.Chart as Chart).Series.Count == 1)
			{
				base.FirePropertyChanged(VcProperties.ShowInLegend);
				return;
			}
			if (ObservableObject.ValidatePartialUpdate(this, VcProperties.DataPoints))
			{
				this.DataPointRenderManager(VcProperties.DataPoints, e.NewItems);
				return;
			}
			if (base.Chart is Chart && (base.Chart as Chart).ChartArea != null && (base.Chart as Chart).ChartArea._isFirstTimeRender)
			{
				base.FirePropertyChanged(VcProperties.DataPoints);
			}
		}

		internal void RemoveVisualElementsFromTree(IDataPoint dataPoint, bool setToNull)
		{
			if (dataPoint.DpInfo.Faces != null && dataPoint.DpInfo.Faces.Visual != null)
			{
				dataPoint.DetachToolTip(dataPoint.DpInfo.Faces.Visual);
				ObservableObject.RemoveElementFromElementTree(dataPoint.DpInfo.Faces.Visual);
			}
			if (dataPoint.DpInfo.LabelVisual != null)
			{
				ObservableObject.RemoveElementFromElementTree(dataPoint.DpInfo.LabelVisual);
			}
			if (dataPoint.DpInfo.Marker != null && dataPoint.DpInfo.Marker.Visual != null)
			{
				dataPoint.DetachToolTip(dataPoint.DpInfo.Marker.Visual);
				ObservableObject.RemoveElementFromElementTree(dataPoint.DpInfo.Marker.Visual);
			}
			if (dataPoint.DpInfo.Faces != null && dataPoint.DpInfo.Faces.VisualComponents != null)
			{
				foreach (FrameworkElement current in dataPoint.DpInfo.Faces.VisualComponents)
				{
					if (current != null)
					{
						dataPoint.DetachToolTip(current);
						ObservableObject.RemoveElementFromElementTree(current);
					}
				}
			}
			if (setToNull)
			{
				((IDataPointParentSetter)dataPoint).Parent = null;
				dataPoint.DpInfo.Faces = null;
				dataPoint.Chart = null;
				dataPoint.DpInfo.Marker = null;
				dataPoint.DpInfo.LabelVisual = null;
			}
		}

		private void DataPointRenderManager(VcProperties property, object newValue)
		{
			Chart chart = base.Chart as Chart;
			if (chart != null && !this.PARTIAL_DS_RENDER_LOCK)
			{
				this.PARTIAL_DS_RENDER_LOCK = true;
				chart.Dispatcher.BeginInvoke(new Action<VcProperties, object>(this.UpdateVisual), new object[]
				{
					VcProperties.DataPoints,
					newValue
				});
				chart.Dispatcher.BeginInvoke(new Action<Chart>(this.ActivatePartialUpdateLock), new object[]
				{
					chart
				});
			}
		}

		private void ActivatePartialUpdateLock(Chart chart)
		{
			this.PARTIAL_DS_RENDER_LOCK = false;
		}

		private IDataPoint GetNearestDataPoint(double xValue)
		{
			IDataPoint dataPoint = this.InternalDataPoints[0];
			double num = Math.Abs(dataPoint.DpInfo.ActualNumericXValue - xValue);
			for (int i = 1; i < this.InternalDataPoints.Count; i++)
			{
				if (Math.Abs(this.InternalDataPoints[i].DpInfo.ActualNumericXValue - xValue) < num)
				{
					num = Math.Abs(this.InternalDataPoints[i].DpInfo.ActualNumericXValue - xValue);
					dataPoint = this.InternalDataPoints[i];
				}
			}
			return dataPoint;
		}

		public double GetYValue(object targetXValue)
		{
			double num = double.NaN;
			if (this.PlotGroup != null && this.PlotGroup.AxisX != null && this.PlotGroup.AxisY != null)
			{
				Axis axisX = this.PlotGroup.AxisX;
				Axis axisY = this.PlotGroup.AxisY;
				Point[] knotPoints;
				if (axisY.ViewportRangeEnabled)
				{
					List<IDataPoint> dataPointsUnderViewPort = RenderHelper.GetDataPointsUnderViewPort(this, false, false);
					knotPoints = (from dp in dataPointsUnderViewPort
					select dp.DpInfo.VisualPosition).ToArray<Point>();
				}
				else
				{
					knotPoints = (from dp in this.InternalDataPoints
					select dp.DpInfo.VisualPosition).ToArray<Point>();
				}
				Point point = new Point(axisX.XValueToPixelXPosition(targetXValue), double.NaN);
				point.Y = LineChart.FindPointOnLine(point.X, knotPoints, this);
				if (this.Faces != null && this.Faces.Visual != null)
				{
					num = Graphics.PixelPositionToValue(this.Faces.Visual.Height, 0.0, axisY.AxisManager.AxisMinimumValue, axisY.AxisManager.AxisMaximumValue, point.Y);
				}
				if (axisY.Logarithmic && !double.IsNaN(num))
				{
					num = Math.Pow(axisY.LogarithmBase, num);
				}
				return num;
			}
			return num;
		}

		private void DataPoint_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.FirePropertyChanged((VcProperties)Enum.Parse(typeof(VcProperties), e.PropertyName, true));
		}

		public override void OnToolTipTextPropertyChanged(string newValue)
		{
			if (base.Chart != null)
			{
				foreach (IDataPoint current in this.DataPoints)
				{
					current.OnToolTipTextPropertyChanged(current.ToolTipText);
				}
			}
		}

		internal void AttachEvent2DataSeriesVisualFaces()
		{
			if (this.RenderAs == RenderAs.StackedArea || this.RenderAs == RenderAs.StackedArea100)
			{
				this.AttachEvent2AreaVisualFaces(this);
				return;
			}
			foreach (IDataPoint current in this.InternalDataPoints)
			{
				DataPointHelper.AttachEvent2DataPointVisualFaces(current, current.Parent);
			}
		}

		internal void AttachEvent2AreaVisualFaces(ObservableObject Object)
		{
			if (this.Faces != null)
			{
				foreach (FrameworkElement current in this.Faces.VisualComponents)
				{
					AreaChart.AttachEvents2AreaVisual(Object, this, current);
				}
			}
			foreach (IDataPoint current2 in this.InternalDataPoints)
			{
				DataPointHelper.AttachEvent2DataPointVisualFaces(current2, this);
			}
		}

		internal IDataPoint GetNearestDataPointOnMouseButtonEvent(MouseButtonEventArgs e)
		{
			Point position = e.GetPosition(this.Faces.Visual);
			double xValue = Graphics.PixelPositionToValue(0.0, this.Faces.Visual.Width, (base.Chart as Chart).ChartArea.AxisX.AxisManager.AxisMinimumValue, (base.Chart as Chart).ChartArea.AxisX.AxisManager.AxisMaximumValue, position.X);
			return this.GetNearestDataPoint(xValue);
		}

		internal IDataPoint GetNearestDataPointOnMouseEvent(MouseEventArgs e)
		{
			Point position = e.GetPosition(this.Faces.Visual);
			double xValue = Graphics.PixelPositionToValue(0.0, this.Faces.Visual.Width, (base.Chart as Chart).ChartArea.AxisX.AxisManager.AxisMinimumValue, (base.Chart as Chart).ChartArea.AxisX.AxisManager.AxisMaximumValue, position.X);
			return this.GetNearestDataPoint(xValue);
		}

		internal void RemoveToolTip()
		{
			if (this.ToolTipElement != null)
			{
				Chart chart = base.Chart as Chart;
				chart.ToolTips.Remove(this.ToolTipElement);
				chart._toolTipCanvas.Children.Remove(this.ToolTipElement);
				this.ToolTipElement = null;
			}
		}

		internal void AttachAreaToolTip(VisifireControl control, List<FrameworkElement> elements)
		{
			foreach (FrameworkElement current in elements)
			{
				current.MouseMove += delegate(object sender, MouseEventArgs e)
				{
					Point position = e.GetPosition(this.Faces.Visual);
					double xValue = Graphics.PixelPositionToValue(0.0, this.Faces.Visual.Width, (control as Chart).ChartArea.AxisX.AxisManager.AxisMinimumValue, (control as Chart).ChartArea.AxisX.AxisManager.AxisMaximumValue, position.X);
					IDataPoint nearestDataPoint = this.GetNearestDataPoint(xValue);
					control._toolTip.CallOutVisiblity = Visibility.Collapsed;
					if (nearestDataPoint.ToolTipText == null)
					{
						control._toolTip.Text = "";
						control._toolTip.Hide();
						return;
					}
					control._toolTip.Text = nearestDataPoint.ParseToolTipText(nearestDataPoint.ToolTipText);
					if (control.ToolTipEnabled)
					{
						control._toolTip.Show();
					}
					(control as Chart).UpdateToolTipPosition(sender, e);
				};
				current.MouseLeave += delegate(object sender, MouseEventArgs e)
				{
					control._toolTip.Hide();
				};
			}
		}

		internal void DetachOpacityPropertyFromAnimation()
		{
			foreach (IDataPoint current in this.InternalDataPoints)
			{
				if (current.DpInfo.Faces != null && (base.Chart as Chart).View3D && (this.RenderAs == RenderAs.Pie || this.RenderAs == RenderAs.Doughnut || this.RenderAs == RenderAs.SectionFunnel || this.RenderAs == RenderAs.StreamLineFunnel || this.RenderAs == RenderAs.Pyramid))
				{
					foreach (FrameworkElement current2 in current.DpInfo.Faces.VisualComponents)
					{
						if (current.IsLightDataPoint)
						{
							InteractivityHelper.DetachOpacityPropertyFromAnimation(current2, this.Opacity);
						}
						else
						{
							InteractivityHelper.DetachOpacityPropertyFromAnimation(current2, this.Opacity * (current as DataPoint).Opacity);
						}
					}
				}
			}
		}

		internal void AttachOrDetachIntaractivity()
		{
			foreach (IDataPoint current in this.InternalDataPoints)
			{
				this.AttachOrDetachInteractivity4DataPoint(current);
			}
		}

		internal void AttachOrDetachInteractivity4DataPoint(IDataPoint dp)
		{
			if (base.Chart == null)
			{
				return;
			}
			if (dp.DpInfo.Faces != null)
			{
				if ((base.Chart as Chart).View3D && (this.RenderAs == RenderAs.Pie || this.RenderAs == RenderAs.Doughnut || this.RenderAs == RenderAs.SectionFunnel || this.RenderAs == RenderAs.StreamLineFunnel || this.RenderAs == RenderAs.Pyramid))
				{
					using (List<FrameworkElement>.Enumerator enumerator = dp.DpInfo.Faces.VisualComponents.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							FrameworkElement current = enumerator.Current;
							if (this.SelectionEnabled)
							{
								InteractivityHelper.ApplyOnMouseOverOpacityInteractivity2Visuals(current);
							}
							else
							{
								InteractivityHelper.RemoveOnMouseOverOpacityInteractivity(current, this.Opacity * (double)DataPointHelper.GetDataPointValueFromProperty(dp, VcProperties.Opacity));
							}
						}
						goto IL_10B;
					}
				}
				if (this.SelectionEnabled)
				{
					InteractivityHelper.ApplyOnMouseOverOpacityInteractivity(dp.DpInfo.Faces.Visual);
				}
				else
				{
					InteractivityHelper.RemoveOnMouseOverOpacityInteractivity(dp.DpInfo.Faces.Visual, this.Opacity * (double)DataPointHelper.GetDataPointValueFromProperty(dp, VcProperties.Opacity));
				}
			}
			IL_10B:
			if (dp.DpInfo.Marker != null && dp.DpInfo.Marker.Visual != null)
			{
				if (this.SelectionEnabled)
				{
					InteractivityHelper.ApplyOnMouseOverOpacityInteractivity(dp.DpInfo.Marker.Visual);
					return;
				}
				if (!(base.Chart as Chart).ChartArea._isFirstTimeRender)
				{
					InteractivityHelper.RemoveOnMouseOverOpacityInteractivity(dp.DpInfo.Marker.Visual, this.Opacity * (double)DataPointHelper.GetDataPointValueFromProperty(dp, VcProperties.Opacity));
				}
			}
		}

		internal override void ClearInstanceRefs()
		{
			if (this.Storyboard != null)
			{
				this.Storyboard.FillBehavior = FillBehavior.Stop;
				this.Storyboard.Stop();
				this.Storyboard = null;
			}
		}

		internal bool IsXValueNull4AllDataPoints()
		{
			if (this.InternalDataPoints != null)
			{
				using (List<IDataPoint>.Enumerator enumerator = this.InternalDataPoints.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IDataPoint current = enumerator.Current;
						if (current.XValue != null)
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
			return false;
		}
	}
}
