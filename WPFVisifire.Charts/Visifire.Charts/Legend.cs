using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class Legend : ObservableObject
	{
		private struct EntrySize
		{
			public Size SymbolSize;

			public Size TextSize;
		}

		private const double DEFAULT_MARGIN = 8.0;

		private const double SCROLLBAR_SIZE_OF_SCROLLVIEWER = 18.0;

		private const double ENTRY_SYMBOL_LINE_WIDTH = 12.0;

		public static readonly DependencyProperty HrefTargetProperty;

		public static readonly DependencyProperty HrefProperty;

		public static readonly DependencyProperty ReversedProperty;

		public static readonly DependencyProperty LabelMarginProperty;

		public static readonly DependencyProperty MarkerSizeProperty;

		public static readonly DependencyProperty BorderColorProperty;

		public static readonly DependencyProperty TitleFontColorProperty;

		public static readonly DependencyProperty DockInsidePlotAreaProperty;

		public static readonly DependencyProperty EnabledProperty;

		public static readonly DependencyProperty FontColorProperty;

		public static readonly DependencyProperty LightingEnabledProperty;

		public static readonly DependencyProperty ShadowEnabledProperty;

		public static readonly DependencyProperty CornerRadiusProperty;

		public static readonly DependencyProperty TitleProperty;

		public static readonly DependencyProperty TitleAlignmentXProperty;

		public static readonly DependencyProperty TitleTextAlignmentProperty;

		public static readonly DependencyProperty TitleBackgroundProperty;

		public static readonly DependencyProperty TitleFontFamilyProperty;

		public static readonly DependencyProperty TitleFontSizeProperty;

		public static readonly DependencyProperty TitleFontStyleProperty;

		public static readonly DependencyProperty TitleFontWeightProperty;

		public static readonly DependencyProperty EntryMarginProperty;

		public new static readonly DependencyProperty FontFamilyProperty;

		public new static readonly DependencyProperty FontSizeProperty;

		public new static readonly DependencyProperty FontStyleProperty;

		public new static readonly DependencyProperty FontWeightProperty;

		public new static readonly DependencyProperty OpacityProperty;

		public new static readonly DependencyProperty BorderThicknessProperty;

		public new static readonly DependencyProperty BackgroundProperty;

		public new static readonly DependencyProperty PaddingProperty;

		public new static readonly DependencyProperty HorizontalAlignmentProperty;

		public new static readonly DependencyProperty VerticalAlignmentProperty;

		public new static readonly DependencyProperty MaxHeightProperty;

		public new static readonly DependencyProperty MaxWidthProperty;

		internal static readonly DependencyProperty LayoutProperty;

		internal bool _isAutoName = true;

		private double _internalFontSize = double.NaN;

		private FontFamily _internalFontFamily;

		private FontStyle? _internalFontStyle = null;

		private FontWeight? _internalFontWeight = null;

		private Thickness? _borderThickness = null;

		private Brush _internalBackground;

		private HorizontalAlignment? _internalHorizontalAlignment = null;

		private VerticalAlignment? _internalVerticalAlignment = null;

		private Thickness? _internalPadding = null;

		private double _internalOpacity = double.NaN;

		private double _internalMaxheight = double.NaN;

		private double _internalMaxWidth = double.NaN;

		private static bool _defaultStyleKeyApplied;

		public new event EventHandler<LegendMouseButtonEventArgs> MouseLeftButtonDown
		{
			add
			{
				this._onMouseLeftButtonDown += value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
			remove
			{
				this._onMouseLeftButtonDown -= value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
		}

		public new event EventHandler<LegendMouseButtonEventArgs> MouseLeftButtonUp
		{
			add
			{
				this._onMouseLeftButtonUp += value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
			remove
			{
				this._onMouseLeftButtonUp -= value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
		}

		public new event EventHandler<LegendMouseButtonEventArgs> MouseRightButtonDown
		{
			add
			{
				this._onMouseRightButtonDown += value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
			remove
			{
				this._onMouseRightButtonDown -= value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
		}

		public new event EventHandler<LegendMouseButtonEventArgs> MouseRightButtonUp
		{
			add
			{
				this._onMouseRightButtonUp += value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
			remove
			{
				this._onMouseRightButtonUp -= value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
		}

		public new event EventHandler<LegendMouseEventArgs> MouseMove
		{
			add
			{
				this._onMouseMove += value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
			remove
			{
				this._onMouseMove -= value;
				if (this.EventChanged != null)
				{
					this.EventChanged(this, null);
				}
			}
		}

		internal new event EventHandler EventChanged;

		private event EventHandler<LegendMouseButtonEventArgs> _onMouseLeftButtonDown;

		private event EventHandler<LegendMouseButtonEventArgs> _onMouseLeftButtonUp;

		private event EventHandler<LegendMouseEventArgs> _onMouseMove;

		private event EventHandler<LegendMouseButtonEventArgs> _onMouseRightButtonDown;

		private event EventHandler<LegendMouseButtonEventArgs> _onMouseRightButtonUp;

		public HrefTargets HrefTarget
		{
			get
			{
				return (HrefTargets)base.GetValue(Legend.HrefTargetProperty);
			}
			set
			{
				base.SetValue(Legend.HrefTargetProperty, value);
			}
		}

		public string Href
		{
			get
			{
				return (string)base.GetValue(Legend.HrefProperty);
			}
			set
			{
				base.SetValue(Legend.HrefProperty, value);
			}
		}

		public bool Reversed
		{
			get
			{
				return (bool)base.GetValue(Legend.ReversedProperty);
			}
			set
			{
				base.SetValue(Legend.ReversedProperty, value);
			}
		}

		public new double Opacity
		{
			get
			{
				return (double)base.GetValue(Legend.OpacityProperty);
			}
			set
			{
				base.SetValue(Legend.OpacityProperty, value);
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
				if (base.Cursor != value)
				{
					base.Cursor = value;
					base.FirePropertyChanged(VcProperties.Cursor);
				}
			}
		}

		public double MarkerSize
		{
			get
			{
				return (double)base.GetValue(Legend.MarkerSizeProperty);
			}
			set
			{
				base.SetValue(Legend.MarkerSizeProperty, value);
			}
		}

		public double LabelMargin
		{
			get
			{
				return (double)base.GetValue(Legend.LabelMarginProperty);
			}
			set
			{
				base.SetValue(Legend.LabelMarginProperty, value);
			}
		}

		public new Thickness Padding
		{
			get
			{
				return (Thickness)base.GetValue(Legend.PaddingProperty);
			}
			set
			{
				base.SetValue(Legend.PaddingProperty, value);
			}
		}

		public new HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return (HorizontalAlignment)base.GetValue(Legend.HorizontalAlignmentProperty);
			}
			set
			{
				base.SetValue(Legend.HorizontalAlignmentProperty, value);
			}
		}

		public new VerticalAlignment VerticalAlignment
		{
			get
			{
				return (VerticalAlignment)base.GetValue(Legend.VerticalAlignmentProperty);
			}
			set
			{
				base.SetValue(Legend.VerticalAlignmentProperty, value);
			}
		}

		public Brush BorderColor
		{
			get
			{
				return (Brush)base.GetValue(Legend.BorderColorProperty);
			}
			set
			{
				base.SetValue(Legend.BorderColorProperty, value);
			}
		}

		public new Thickness BorderThickness
		{
			get
			{
				return (Thickness)base.GetValue(Legend.BorderThicknessProperty);
			}
			set
			{
				base.SetValue(Legend.BorderThicknessProperty, value);
			}
		}

		public new Brush Background
		{
			get
			{
				return (Brush)base.GetValue(Legend.BackgroundProperty);
			}
			set
			{
				base.SetValue(Legend.BackgroundProperty, value);
			}
		}

		public bool DockInsidePlotArea
		{
			get
			{
				return (bool)base.GetValue(Legend.DockInsidePlotAreaProperty);
			}
			set
			{
				base.SetValue(Legend.DockInsidePlotAreaProperty, value);
			}
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Enabled
		{
			get
			{
				if (!((bool?)base.GetValue(Legend.EnabledProperty)).HasValue)
				{
					return new bool?(true);
				}
				return (bool?)base.GetValue(Legend.EnabledProperty);
			}
			set
			{
				base.SetValue(Legend.EnabledProperty, value);
			}
		}

		public Brush FontColor
		{
			get
			{
				return (Brush)base.GetValue(Legend.FontColorProperty);
			}
			set
			{
				base.SetValue(Legend.FontColorProperty, value);
			}
		}

		public new FontFamily FontFamily
		{
			get
			{
				if ((FontFamily)base.GetValue(Legend.FontFamilyProperty) == null)
				{
					return new FontFamily("Verdana");
				}
				return (FontFamily)base.GetValue(Legend.FontFamilyProperty);
			}
			set
			{
				base.SetValue(Legend.FontFamilyProperty, value);
			}
		}

		public new double FontSize
		{
			get
			{
				return (double)base.GetValue(Legend.FontSizeProperty);
			}
			set
			{
				base.SetValue(Legend.FontSizeProperty, value);
			}
		}

		[TypeConverter(typeof(FontStyleConverter))]
		public new FontStyle FontStyle
		{
			get
			{
				return (FontStyle)base.GetValue(Legend.FontStyleProperty);
			}
			set
			{
				base.SetValue(Legend.FontStyleProperty, value);
			}
		}

		[TypeConverter(typeof(FontWeightConverter))]
		public new FontWeight FontWeight
		{
			get
			{
				return (FontWeight)base.GetValue(Legend.FontWeightProperty);
			}
			set
			{
				base.SetValue(Legend.FontWeightProperty, value);
			}
		}

		public bool LightingEnabled
		{
			get
			{
				return (bool)base.GetValue(Legend.LightingEnabledProperty);
			}
			set
			{
				base.SetValue(Legend.LightingEnabledProperty, value);
			}
		}

		public bool ShadowEnabled
		{
			get
			{
				return (bool)base.GetValue(Legend.ShadowEnabledProperty);
			}
			set
			{
				base.SetValue(Legend.ShadowEnabledProperty, value);
			}
		}

		[TypeConverter(typeof(CornerRadiusConverter))]
		public CornerRadius CornerRadius
		{
			get
			{
				return (CornerRadius)base.GetValue(Legend.CornerRadiusProperty);
			}
			set
			{
				base.SetValue(Legend.CornerRadiusProperty, value);
			}
		}

		public string Title
		{
			get
			{
				return (string)base.GetValue(Legend.TitleProperty);
			}
			set
			{
				base.SetValue(Legend.TitleProperty, value);
			}
		}

		public HorizontalAlignment TitleAlignmentX
		{
			get
			{
				return (HorizontalAlignment)base.GetValue(Legend.TitleAlignmentXProperty);
			}
			set
			{
				base.SetValue(Legend.TitleAlignmentXProperty, value);
			}
		}

		public TextAlignment TitleTextAlignment
		{
			get
			{
				return (TextAlignment)base.GetValue(Legend.TitleTextAlignmentProperty);
			}
			set
			{
				base.SetValue(Legend.TitleTextAlignmentProperty, value);
			}
		}

		public Brush TitleBackground
		{
			get
			{
				return (Brush)base.GetValue(Legend.TitleBackgroundProperty);
			}
			set
			{
				base.SetValue(Legend.TitleBackgroundProperty, value);
			}
		}

		public Brush TitleFontColor
		{
			get
			{
				return (Brush)base.GetValue(Legend.TitleFontColorProperty);
			}
			set
			{
				base.SetValue(Legend.TitleFontColorProperty, value);
			}
		}

		public FontFamily TitleFontFamily
		{
			get
			{
				return (FontFamily)base.GetValue(Legend.TitleFontFamilyProperty);
			}
			set
			{
				base.SetValue(Legend.TitleFontFamilyProperty, value);
			}
		}

		public double TitleFontSize
		{
			get
			{
				return (double)base.GetValue(Legend.TitleFontSizeProperty);
			}
			set
			{
				base.SetValue(Legend.TitleFontSizeProperty, value);
			}
		}

		public FontStyle TitleFontStyle
		{
			get
			{
				return (FontStyle)base.GetValue(Legend.TitleFontStyleProperty);
			}
			set
			{
				base.SetValue(Legend.TitleFontStyleProperty, value);
			}
		}

		public FontWeight TitleFontWeight
		{
			get
			{
				return (FontWeight)base.GetValue(Legend.TitleFontWeightProperty);
			}
			set
			{
				base.SetValue(Legend.TitleFontWeightProperty, value);
			}
		}

		public double EntryMargin
		{
			get
			{
				return (double)base.GetValue(Legend.EntryMarginProperty);
			}
			set
			{
				base.SetValue(Legend.EntryMarginProperty, value);
			}
		}

		public new double MaxHeight
		{
			get
			{
				return (double)base.GetValue(Legend.MaxHeightProperty);
			}
			set
			{
				base.SetValue(Legend.MaxHeightProperty, value);
			}
		}

		public new double MaxWidth
		{
			get
			{
				return (double)base.GetValue(Legend.MaxWidthProperty);
			}
			set
			{
				base.SetValue(Legend.MaxWidthProperty, value);
			}
		}

		internal FontFamily InternalFontFamily
		{
			get
			{
				FontFamily fontFamily;
				if (this._internalFontFamily == null)
				{
					fontFamily = (FontFamily)base.GetValue(Legend.FontFamilyProperty);
				}
				else
				{
					fontFamily = this._internalFontFamily;
				}
				if (fontFamily != null)
				{
					return fontFamily;
				}
				return new FontFamily("Verdana");
			}
			set
			{
				this._internalFontFamily = value;
			}
		}

		internal double InternalFontSize
		{
			get
			{
				return (double)(double.IsNaN(this._internalFontSize) ? base.GetValue(Legend.FontSizeProperty) : this._internalFontSize);
			}
			set
			{
				this._internalFontSize = value;
			}
		}

		[TypeConverter(typeof(FontStyleConverter))]
		internal FontStyle InternalFontStyle
		{
			get
			{
				return (FontStyle)((!this._internalFontStyle.HasValue) ? base.GetValue(Legend.FontStyleProperty) : this._internalFontStyle);
			}
			set
			{
				this._internalFontStyle = new FontStyle?(value);
			}
		}

		[TypeConverter(typeof(FontWeightConverter))]
		internal FontWeight InternalFontWeight
		{
			get
			{
				return (FontWeight)((!this._internalFontWeight.HasValue) ? base.GetValue(Legend.FontWeightProperty) : this._internalFontWeight);
			}
			set
			{
				this._internalFontWeight = new FontWeight?(value);
			}
		}

		internal Thickness InternalBorderThickness
		{
			get
			{
				return (Thickness)((!this._borderThickness.HasValue) ? base.GetValue(Legend.BorderThicknessProperty) : this._borderThickness);
			}
			set
			{
				this._borderThickness = new Thickness?(value);
			}
		}

		internal Brush InternalBackground
		{
			get
			{
				return (Brush)((this._internalBackground == null) ? base.GetValue(Legend.BackgroundProperty) : this._internalBackground);
			}
			set
			{
				this._internalBackground = value;
			}
		}

		internal HorizontalAlignment InternalHorizontalAlignment
		{
			get
			{
				return (HorizontalAlignment)((!this._internalHorizontalAlignment.HasValue) ? base.GetValue(Legend.HorizontalAlignmentProperty) : this._internalHorizontalAlignment);
			}
			set
			{
				this._internalHorizontalAlignment = new HorizontalAlignment?(value);
			}
		}

		internal VerticalAlignment InternalVerticalAlignment
		{
			get
			{
				return (VerticalAlignment)((!this._internalVerticalAlignment.HasValue) ? base.GetValue(Legend.VerticalAlignmentProperty) : this._internalVerticalAlignment);
			}
			set
			{
				this._internalVerticalAlignment = new VerticalAlignment?(value);
			}
		}

		internal Thickness InternalPadding
		{
			get
			{
				return (Thickness)((!this._internalPadding.HasValue) ? base.GetValue(Legend.PaddingProperty) : this._internalPadding);
			}
			set
			{
				this._internalPadding = new Thickness?(value);
			}
		}

		internal double InternalOpacity
		{
			get
			{
				return (double)(double.IsNaN(this._internalOpacity) ? base.GetValue(Legend.OpacityProperty) : this._internalOpacity);
			}
			set
			{
				this._internalOpacity = value;
			}
		}

		internal double InternalMaxHeight
		{
			get
			{
				return (double)(double.IsNaN(this._internalMaxheight) ? base.GetValue(Legend.MaxHeightProperty) : this._internalMaxheight);
			}
			set
			{
				this._internalMaxheight = value;
			}
		}

		internal double InternalMaxWidth
		{
			get
			{
				return (double)(double.IsNaN(this._internalMaxWidth) ? base.GetValue(Legend.MaxWidthProperty) : this._internalMaxWidth);
			}
			set
			{
				this._internalMaxWidth = value;
			}
		}

		internal string InternalName
		{
			get;
			set;
		}

		internal double InternalMaximumHeight
		{
			get;
			set;
		}

		internal double InternalMaximumWidth
		{
			get;
			set;
		}

		internal Orientation Orientation
		{
			get;
			set;
		}

		internal int MaxRows
		{
			get;
			set;
		}

		internal int MaxColumns
		{
			get;
			set;
		}

		internal Border ShadowBorder
		{
			get;
			set;
		}

		internal Border Visual
		{
			get;
			set;
		}

		internal Layouts Layout
		{
			get
			{
				if ((Layouts)base.GetValue(Legend.LayoutProperty) == Layouts.Auto)
				{
					return Layouts.FlowLayout;
				}
				return (Layouts)base.GetValue(Legend.LayoutProperty);
			}
			set
			{
				base.SetValue(Legend.LayoutProperty, value);
			}
		}

		internal List<LegendEntry> Entries
		{
			get;
			set;
		}

		private StackPanel LegendContainer
		{
			get;
			set;
		}

		public Legend()
		{
			this.Entries = new List<LegendEntry>();
			this.EventChanged += delegate(object A_1, EventArgs A_2)
			{
				base.FirePropertyChanged(VcProperties.MouseEvent);
			};
		}

		static Legend()
		{
			Legend.HrefTargetProperty = DependencyProperty.Register("HrefTarget", typeof(HrefTargets), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnHrefTargetChanged)));
			Legend.HrefProperty = DependencyProperty.Register("Href", typeof(string), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnHrefChanged)));
			Legend.ReversedProperty = DependencyProperty.Register("Reversed", typeof(bool), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnReversedChanged)));
			Legend.LabelMarginProperty = DependencyProperty.Register("LabelMargin", typeof(double), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnLabelMarginPropertyChanged)));
			Legend.MarkerSizeProperty = DependencyProperty.Register("MarkerSize", typeof(double), typeof(Legend), new PropertyMetadata(8.0, new PropertyChangedCallback(Legend.OnMarkerSizePropertyChanged)));
			Legend.BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(Brush), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnBorderColorPropertyChanged)));
			Legend.TitleFontColorProperty = DependencyProperty.Register("TitleFontColor", typeof(Brush), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnTitleFontColorPropertyChanged)));
			Legend.DockInsidePlotAreaProperty = DependencyProperty.Register("DockInsidePlotArea", typeof(bool), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnDockInsidePlotAreaPropertyChanged)));
			Legend.EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool?), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnEnabledPropertyChanged)));
			Legend.FontColorProperty = DependencyProperty.Register("FontColor", typeof(Brush), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnFontColorPropertyChanged)));
			Legend.LightingEnabledProperty = DependencyProperty.Register("LightingEnabled", typeof(bool), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnLightingEnabledPropertyChanged)));
			Legend.ShadowEnabledProperty = DependencyProperty.Register("ShadowEnabled", typeof(bool), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnShadowEnabledPropertyChanged)));
			Legend.CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Legend), new PropertyMetadata(new CornerRadius(1.0), new PropertyChangedCallback(Legend.OnCornerRadiusPropertyChanged)));
			Legend.TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnTitlePropertyChanged)));
			Legend.TitleAlignmentXProperty = DependencyProperty.Register("TitleAlignmentX", typeof(HorizontalAlignment), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnTitleAlignmentXPropertyChanged)));
			Legend.TitleTextAlignmentProperty = DependencyProperty.Register("TitleTextAlignment", typeof(TextAlignment), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnTitleTextAlignmentPropertyChanged)));
			Legend.TitleBackgroundProperty = DependencyProperty.Register("TitleBackground", typeof(Brush), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnTitleBackgroundPropertyChanged)));
			Legend.TitleFontFamilyProperty = DependencyProperty.Register("TitleFontFamily", typeof(FontFamily), typeof(Legend), new PropertyMetadata(new FontFamily("Arial"), new PropertyChangedCallback(Legend.OnTitleFontFamilyPropertyChanged)));
			Legend.TitleFontSizeProperty = DependencyProperty.Register("TitleFontSize", typeof(double), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnTitleFontSizePropertyChanged)));
			Legend.TitleFontStyleProperty = DependencyProperty.Register("TitleFontStyle", typeof(FontStyle), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnTitleFontStylePropertyChanged)));
			Legend.TitleFontWeightProperty = DependencyProperty.Register("TitleFontWeight", typeof(FontWeight), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnTitleFontWeightPropertyChanged)));
			Legend.EntryMarginProperty = DependencyProperty.Register("EntryMargin", typeof(double), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnEntryMarginPropertyChanged)));
			Legend.FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnFontFamilyPropertyChanged)));
			Legend.FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnFontSizePropertyChanged)));
			Legend.FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnFontStylePropertyChanged)));
			Legend.FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnFontWeightPropertyChanged)));
			Legend.OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(Legend), new PropertyMetadata(1.0, new PropertyChangedCallback(Legend.OnOpacityPropertyChanged)));
			Legend.BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnBorderThicknessPropertyChanged)));
			Legend.BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnBackgroundPropertyChanged)));
			Legend.PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnPaddingPropertyChanged)));
			Legend.HorizontalAlignmentProperty = DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnHorizontalAlignmentPropertyChanged)));
			Legend.VerticalAlignmentProperty = DependencyProperty.Register("VerticalAlignment", typeof(VerticalAlignment), typeof(Legend), new PropertyMetadata(new PropertyChangedCallback(Legend.OnVerticalAlignmentPropertyChanged)));
			Legend.MaxHeightProperty = DependencyProperty.Register("MaxHeight", typeof(double), typeof(Legend), new PropertyMetadata(double.PositiveInfinity, new PropertyChangedCallback(Legend.OnMaxHeightPropertyChanged)));
			Legend.MaxWidthProperty = DependencyProperty.Register("MaxWidth", typeof(double), typeof(Legend), new PropertyMetadata(double.PositiveInfinity, new PropertyChangedCallback(Legend.OnMaxWidthPropertyChanged)));
			Legend.LayoutProperty = DependencyProperty.Register("Layout", typeof(Layouts), typeof(Legend), null);
			if (!Legend._defaultStyleKeyApplied)
			{
				FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Legend), new FrameworkPropertyMetadata(typeof(Legend)));
				Legend._defaultStyleKeyApplied = true;
			}
		}

		internal override void Bind()
		{
		}

		private static void OnHrefTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.HrefTarget);
		}

		private static void OnHrefChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.Href);
		}

		private static void OnReversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.Reversed);
		}

		private static void OnLabelMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.LabelMargin);
		}

		private static void OnIntractivityEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			if ((bool)e.NewValue)
			{
				legend.MouseLeftButtonUp += new EventHandler<LegendMouseButtonEventArgs>(Legend.legend_MouseLeftButtonUp);
				legend.MouseMove += new EventHandler<LegendMouseEventArgs>(Legend.legend_MouseMove);
				return;
			}
			if (legend.Visual != null)
			{
				legend.Visual.Cursor = legend.Cursor;
			}
			legend.MouseLeftButtonUp -= new EventHandler<LegendMouseButtonEventArgs>(Legend.legend_MouseLeftButtonUp);
			legend.MouseMove -= new EventHandler<LegendMouseEventArgs>(Legend.legend_MouseMove);
		}

		private static void legend_MouseMove(object sender, LegendMouseEventArgs e)
		{
			Legend legend = sender as Legend;
			if (legend.Visual != null)
			{
				legend.Visual.Cursor = Cursors.Hand;
			}
		}

		private static void legend_MouseLeftButtonUp(object sender, LegendMouseButtonEventArgs e)
		{
			if (e.DataPoint != null)
			{
				Legend.HideShowDataPoint(e.DataPoint, ElementTypes.DataPoint);
				return;
			}
			if (e.DataSeries != null)
			{
				RenderHelper.IsLineCType(e.DataSeries);
				foreach (IDataPoint current in e.DataSeries.DataPoints)
				{
					Legend.HideShowDataPoint(current, ElementTypes.DataSeries);
				}
			}
		}

		private static void HideShowDataPoint(IDataPoint dp, ElementTypes type)
		{
			if (dp.DpInfo.Faces.Visual.Visibility == Visibility.Visible)
			{
				dp.DpInfo.Faces.Visual.Visibility = Visibility.Collapsed;
				if (type == ElementTypes.DataSeries)
				{
					dp.Parent.LegendMarker.Visual.Opacity = 0.4;
					return;
				}
				dp.DpInfo.LegendMarker.Visual.Opacity = 0.4;
				return;
			}
			else
			{
				dp.DpInfo.Faces.Visual.Visibility = Visibility.Visible;
				if (type == ElementTypes.DataSeries)
				{
					dp.Parent.LegendMarker.Visual.Opacity = dp.Parent.Opacity;
					return;
				}
				dp.DpInfo.LegendMarker.Visual.Opacity = (double)DataPointHelper.GetDataPointValueFromProperty(dp, VcProperties.Opacity);
				return;
			}
		}

		private static void HideShowDataSeries(DataSeries ds)
		{
			if (ds.Faces.Visual.Visibility == Visibility.Visible)
			{
				ds.Faces.Visual.Visibility = Visibility.Collapsed;
				ds.LegendMarker.Visual.Opacity = 0.4;
				return;
			}
			ds.Faces.Visual.Visibility = Visibility.Visible;
			ds.LegendMarker.Visual.Opacity = ds.Opacity;
		}

		private static void OnBorderColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.BorderColor);
		}

		private static void OnMarkerSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.MarkerSize);
		}

		private static void OnDockInsidePlotAreaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.DockInsidePlotArea);
		}

		private static void OnEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.Enabled);
		}

		private static void OnFontColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.FontColor);
		}

		private static void OnFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			if (e.NewValue == null || e.OldValue == null)
			{
				legend.InternalFontFamily = (FontFamily)e.NewValue;
				legend.FirePropertyChanged(VcProperties.FontFamily);
				return;
			}
			if (e.NewValue.ToString() != e.OldValue.ToString())
			{
				legend.InternalFontFamily = (FontFamily)e.NewValue;
				legend.FirePropertyChanged(VcProperties.FontFamily);
			}
		}

		private static void OnFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.InternalFontSize = (double)e.NewValue;
			legend.FirePropertyChanged(VcProperties.FontSize);
		}

		private static void OnFontStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.InternalFontStyle = (FontStyle)e.NewValue;
			legend.FirePropertyChanged(VcProperties.FontStyle);
		}

		private static void OnFontWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.InternalFontWeight = (FontWeight)e.NewValue;
			legend.FirePropertyChanged(VcProperties.FontWeight);
		}

		private static void OnBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.InternalBorderThickness = (Thickness)e.NewValue;
			legend.FirePropertyChanged(VcProperties.BorderThickness);
		}

		private static void OnBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.InternalBackground = (Brush)e.NewValue;
			legend.FirePropertyChanged(VcProperties.Background);
		}

		private static void OnPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.InternalPadding = (Thickness)e.NewValue;
			legend.FirePropertyChanged(VcProperties.Padding);
		}

		private static void OnHorizontalAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.InternalHorizontalAlignment = (HorizontalAlignment)e.NewValue;
			legend.FirePropertyChanged(VcProperties.HorizontalAlignment);
		}

		private static void OnVerticalAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.InternalVerticalAlignment = (VerticalAlignment)e.NewValue;
			legend.FirePropertyChanged(VcProperties.VerticalAlignment);
		}

		private static void OnOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.InternalOpacity = (double)e.NewValue;
			legend.FirePropertyChanged(VcProperties.Opacity);
		}

		private static void OnMaxHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.InternalMaxHeight = (double)e.NewValue;
			legend.FirePropertyChanged(VcProperties.MaxHeight);
		}

		private static void OnMaxWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.InternalMaxWidth = (double)e.NewValue;
			legend.FirePropertyChanged(VcProperties.MaxWidth);
		}

		private static void OnLightingEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.LightingEnabled);
		}

		private static void OnShadowEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.ShadowEnabled);
		}

		private static void OnCornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.CornerRadius);
		}

		private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.Title);
		}

		private static void OnTitleAlignmentXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.TitleAlignmentX);
		}

		private static void OnTitleTextAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.TitleTextAlignment);
		}

		private static void OnTitleBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.TitleBackground);
		}

		private static void OnTitleFontColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.TitleFontColor);
		}

		private static void OnTitleFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.TitleFontFamily);
		}

		private static void OnTitleFontStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.TitleFontStyle);
		}

		private static void OnTitleFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.TitleFontSize);
		}

		private static void OnTitleFontWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.TitleFontWeight);
		}

		private static void OnEntryMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Legend legend = d as Legend;
			legend.FirePropertyChanged(VcProperties.EntryMargin);
		}

		private void ApplyFontProperty(TextBlock textBlock)
		{
			textBlock.FontFamily = this.InternalFontFamily;
			textBlock.FontStyle = this.InternalFontStyle;
			textBlock.FontWeight = this.InternalFontWeight;
			textBlock.FontSize = this.InternalFontSize;
			textBlock.Foreground = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.Background, this.FontColor, this.DockInsidePlotArea);
		}

		private void ApplyFontPropertiesOfMarkerAsSymbol(Marker marker)
		{
			marker.FontFamily = this.InternalFontFamily;
			marker.FontStyle = this.InternalFontStyle;
			marker.FontWeight = this.InternalFontWeight;
			marker.FontSize = this.InternalFontSize;
			marker.FontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.Background, this.FontColor, this.DockInsidePlotArea);
		}

		private void ApplyFontProperty(Title title)
		{
			if (this.TitleFontFamily != null)
			{
				title.InternalFontFamily = this.TitleFontFamily;
			}
			if (this.TitleFontSize != 0.0)
			{
				title.InternalFontSize = this.TitleFontSize;
			}
			FontStyle arg_37_0 = this.TitleFontStyle;
			title.InternalFontStyle = this.TitleFontStyle;
			FontWeight arg_4A_0 = this.TitleFontWeight;
			title.InternalFontWeight = this.TitleFontWeight;
			if (!string.IsNullOrEmpty(this.Title))
			{
				title.Text = ObservableObject.GetFormattedMultilineText(this.Title);
			}
			title.InternalFontColor = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.Background, this.TitleFontColor, this.DockInsidePlotArea);
		}

		private void ApplyVisualProperty()
		{
			if (this.Cursor != null)
			{
				this.Visual.Cursor = this.Cursor;
			}
			this.Visual.BorderBrush = this.BorderColor;
			this.Visual.BorderThickness = this.InternalBorderThickness;
			this.Visual.CornerRadius = this.CornerRadius;
			this.Visual.Background = this.InternalBackground;
			this.Visual.HorizontalAlignment = this.InternalHorizontalAlignment;
			this.Visual.VerticalAlignment = this.InternalVerticalAlignment;
			this.Visual.Opacity = this.InternalOpacity;
			this.ApplyLighting();
			base.AttachHref(base.Chart, this.Visual, this.Href, this.HrefTarget);
			base.AttachToolTip(base.Chart, this, this.Visual);
			base.AttachEvents2Visual(this, this.Visual);
		}

		private Size TextBlockActualSize(TextBlock textBlock)
		{
			textBlock.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			return textBlock.DesiredSize;
		}

		private void ApplyLighting()
		{
			if (this.LightingEnabled)
			{
				this.LegendContainer.Background = Graphics.LightingBrush(this.LightingEnabled);
				return;
			}
			this.LegendContainer.Background = new SolidColorBrush(Colors.Transparent);
		}

		private void ApplyShadow(Grid innerGrid)
		{
			if (this.ShadowEnabled)
			{
				if (!VisifireControl.IsMediaEffectsEnabled)
				{
					double num = 6.0;
					Grid grid = ExtendedGraphics.Get2DRectangleShadow(this, this.Visual.Width, this.Visual.Height, this.CornerRadius, this.CornerRadius, num);
					grid.Clip = ExtendedGraphics.GetShadowClip(new Size(grid.Width, grid.Height), this.CornerRadius);
					grid.SetValue(Panel.ZIndexProperty, -12);
					grid.HorizontalAlignment = HorizontalAlignment.Stretch;
					grid.VerticalAlignment = VerticalAlignment.Stretch;
					grid.Margin = new Thickness(0.0, 0.0, -(num + this.BorderThickness.Right), -(num + this.BorderThickness.Bottom));
					innerGrid.Children.Insert(0, grid);
					return;
				}
				DropShadowEffect effect = new DropShadowEffect
				{
					BlurRadius = 5.0,
					Direction = 315.0,
					ShadowDepth = 4.0,
					Opacity = 0.95,
					Color = Color.FromArgb(255, 185, 185, 185)
				};
				this.Visual.Effect = effect;
			}
		}

		private StackPanel StackPanelColumn()
		{
			return new StackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				Orientation = Orientation.Vertical
			};
		}

		private StackPanel StackPanelRow()
		{
			return new StackPanel
			{
				Orientation = Orientation.Horizontal
			};
		}

		private Legend.EntrySize GetMaxSymbolAndColumnWidth()
		{
			Legend.EntrySize result = default(Legend.EntrySize);
			foreach (LegendEntry current in this.Entries)
			{
				TextBlock textBlock = new TextBlock();
				textBlock.FlowDirection = ((base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				textBlock.Text = current.Labels[0];
				this.ApplyFontProperty(textBlock);
				Size size = this.TextBlockActualSize(textBlock);
				result.TextSize.Width = ((size.Width > result.TextSize.Width) ? size.Width : result.TextSize.Width);
				result.TextSize.Height = ((size.Height > result.TextSize.Height) ? size.Height : result.TextSize.Height);
				current.Marker.Margin = this.EntryMargin;
				current.Marker.CreateVisual((base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				result.SymbolSize.Width = ((current.Marker.MarkerActualSize.Width > result.SymbolSize.Width) ? current.Marker.MarkerActualSize.Width : result.SymbolSize.Width);
				result.SymbolSize.Height = ((current.Marker.MarkerActualSize.Height > result.SymbolSize.Height) ? current.Marker.MarkerActualSize.Height : result.SymbolSize.Height);
			}
			return result;
		}

		private DoubleCollection ApplyLineStyleForMarkerOfLegendEntry(Line line, string lineStyle)
		{
			DoubleCollection result = null;
			if (lineStyle != null)
			{
				if (!(lineStyle == "Solid"))
				{
					if (!(lineStyle == "Dashed"))
					{
						if (lineStyle == "Dotted")
						{
							line.StrokeThickness = 3.0;
							result = new DoubleCollection
							{
								0.5,
								0.5,
								0.5,
								0.5
							};
						}
					}
					else
					{
						line.StrokeThickness = 3.0;
						result = new DoubleCollection
						{
							0.2,
							0.4,
							0.2,
							0.12
						};
					}
				}
				else
				{
					line.StrokeThickness = 3.0;
					result = null;
				}
			}
			return result;
		}

		private Canvas GetNewMarkerForLineChart(Marker marker)
		{
			Canvas canvas = new Canvas();
			Line line = new Line
			{
				Tag = marker.Tag
			};
			line.Stroke = marker.BorderColor;
			double num = (marker.TextBlockSize.Height > marker.MarkerSize.Height) ? marker.TextBlockSize.Height : marker.MarkerSize.Height;
			canvas.Height = marker.MarkerActualSize.Height;
			double width = marker.MarkerSize.Width;
			line.X1 = 0.0;
			line.X2 = width * 2.0;
			line.Y1 = 0.0;
			line.Y2 = 0.0;
			line.Width = width * 2.0;
			canvas.Width = marker.MarkerActualSize.Width + width / 2.0;
			line.StrokeDashArray = this.ApplyLineStyleForMarkerOfLegendEntry(line, marker.DataSeriesOfLegendMarker.LineStyle.ToString());
			canvas.Children.Add(line);
			canvas.Children.Add(marker.Visual);
			if (this.InternalVerticalAlignment != VerticalAlignment.Center || (this.InternalHorizontalAlignment != HorizontalAlignment.Left && this.InternalHorizontalAlignment != HorizontalAlignment.Right))
			{
				line.Margin = new Thickness(marker.Visual.Margin.Left + width / 2.0, marker.Visual.Margin.Top, marker.Visual.Margin.Right, marker.Visual.Margin.Bottom);
				marker.Visual.Margin = new Thickness(marker.Visual.Margin.Left + width / 2.0, marker.Visual.Margin.Top, marker.Visual.Margin.Right, marker.Visual.Margin.Bottom);
			}
			else
			{
				line.Margin = new Thickness(marker.Visual.Margin.Left + width / 2.0, marker.Visual.Margin.Top, marker.Visual.Margin.Right, marker.Visual.Margin.Bottom);
				marker.Visual.Margin = new Thickness(marker.Visual.Margin.Left + width / 2.0, marker.Visual.Margin.Top, marker.Visual.Margin.Right, marker.Visual.Margin.Bottom);
			}
			line.StrokeThickness = 1.5;
			line.SetValue(Canvas.TopProperty, num / 2.0);
			line.SetValue(Canvas.LeftProperty, -width / 2.0);
			return canvas;
		}

		private void DrawVerticalFlowLayout4Legend(ref Grid legendContent)
		{
			int num = 0;
			double num2 = 0.0;
			StackPanel stackPanel = new StackPanel();
			stackPanel.Orientation = Orientation.Horizontal;
			stackPanel.Children.Add(this.StackPanelColumn());
			stackPanel.Height = 0.0;
			foreach (LegendEntry current in this.Entries)
			{
				Marker marker = current.Marker;
				marker.Margin = this.EntryMargin;
				marker.LabelMargin = this.LabelMargin;
				marker.Text = current.Labels[0];
				marker.TextAlignmentY = AlignmentY.Center;
				marker.TextAlignmentX = AlignmentX.Right;
				this.ApplyFontPropertiesOfMarkerAsSymbol(marker);
				if (marker.DataSeriesOfLegendMarker.RenderAs == RenderAs.Line || marker.DataSeriesOfLegendMarker.RenderAs == RenderAs.StepLine || marker.DataSeriesOfLegendMarker.RenderAs == RenderAs.Spline || marker.DataSeriesOfLegendMarker.RenderAs == RenderAs.QuickLine || marker.DataSeriesOfLegendMarker.RenderAs == RenderAs.Stock || marker.DataSeriesOfLegendMarker.RenderAs == RenderAs.CandleStick)
				{
					marker.BorderColor = marker.MarkerFillColor;
					marker.MarkerFillColor = new SolidColorBrush(Colors.White);
					marker.BorderThickness = 0.7;
					marker.LabelMargin += marker.MarkerSize.Width / 2.0;
					marker.CreateVisual((base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
					Canvas newMarkerForLineChart = this.GetNewMarkerForLineChart(marker);
					newMarkerForLineChart.HorizontalAlignment = HorizontalAlignment.Left;
					newMarkerForLineChart.VerticalAlignment = VerticalAlignment.Center;
					if (num2 + newMarkerForLineChart.Height <= this.InternalMaximumHeight)
					{
						(stackPanel.Children[num] as StackPanel).Children.Add(newMarkerForLineChart);
						num2 += newMarkerForLineChart.Height;
					}
					else
					{
						stackPanel.Children.Add(this.StackPanelColumn());
						num++;
						(stackPanel.Children[num] as StackPanel).Children.Add(newMarkerForLineChart);
						num2 = marker.MarkerActualSize.Height;
					}
				}
				else
				{
					marker.CreateVisual((base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
					if (num2 + marker.MarkerActualSize.Height <= this.InternalMaximumHeight)
					{
						(stackPanel.Children[num] as StackPanel).Children.Add(marker.Visual);
						num2 += marker.MarkerActualSize.Height;
					}
					else
					{
						stackPanel.Children.Add(this.StackPanelColumn());
						num++;
						(stackPanel.Children[num] as StackPanel).Children.Add(marker.Visual);
						num2 = marker.MarkerActualSize.Height;
					}
				}
				marker.Visual.HorizontalAlignment = HorizontalAlignment.Left;
				stackPanel.Height = ((stackPanel.Height < num2) ? num2 : stackPanel.Height);
			}
			stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
			stackPanel.VerticalAlignment = VerticalAlignment.Center;
			legendContent.Children.Add(stackPanel);
		}

		private void DrawVerticalGridlayout4Legend(ref Grid legendContent, out ScrollViewer scrollViewer)
		{
			this.DrawHorizontalGridlayout4Legend(ref legendContent, out scrollViewer);
		}

		private void DrawHorizontalFlowLayout4Legend(ref Grid legendContent)
		{
			int num = 0;
			double num2 = 0.0;
			StackPanel stackPanel = new StackPanel();
			stackPanel.Orientation = Orientation.Vertical;
			stackPanel.Children.Add(this.StackPanelRow());
			foreach (LegendEntry current in this.Entries)
			{
				Marker marker = current.Marker;
				marker.Margin = this.EntryMargin;
				marker.LabelMargin = this.LabelMargin;
				marker.Text = current.Labels[0];
				this.ApplyFontPropertiesOfMarkerAsSymbol(marker);
				marker.TextAlignmentY = AlignmentY.Center;
				marker.TextAlignmentX = AlignmentX.Right;
				if (marker.DataSeriesOfLegendMarker.RenderAs == RenderAs.Line || marker.DataSeriesOfLegendMarker.RenderAs == RenderAs.StepLine || marker.DataSeriesOfLegendMarker.RenderAs == RenderAs.Spline || marker.DataSeriesOfLegendMarker.RenderAs == RenderAs.QuickLine || marker.DataSeriesOfLegendMarker.RenderAs == RenderAs.Stock || marker.DataSeriesOfLegendMarker.RenderAs == RenderAs.CandleStick)
				{
					marker.BorderColor = marker.MarkerFillColor;
					marker.MarkerFillColor = new SolidColorBrush(Colors.White);
					marker.BorderThickness = 0.7;
					marker.LabelMargin += marker.MarkerSize.Width / 2.0;
					marker.CreateVisual((base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
					Canvas newMarkerForLineChart = this.GetNewMarkerForLineChart(marker);
					if (num2 + newMarkerForLineChart.Width <= this.InternalMaximumWidth)
					{
						(stackPanel.Children[num] as StackPanel).Children.Add(newMarkerForLineChart);
						num2 += newMarkerForLineChart.Width;
					}
					else
					{
						stackPanel.Children.Add(this.StackPanelRow());
						num++;
						(stackPanel.Children[num] as StackPanel).Children.Add(newMarkerForLineChart);
						num2 = marker.MarkerActualSize.Width;
					}
				}
				else
				{
					marker.CreateVisual((base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
					if (num2 + marker.MarkerActualSize.Width <= this.InternalMaximumWidth)
					{
						(stackPanel.Children[num] as StackPanel).Children.Add(marker.Visual);
						num2 += marker.MarkerActualSize.Width;
					}
					else
					{
						stackPanel.Children.Add(this.StackPanelRow());
						num++;
						(stackPanel.Children[num] as StackPanel).Children.Add(marker.Visual);
						num2 = marker.MarkerActualSize.Width;
					}
				}
				marker.Visual.HorizontalAlignment = HorizontalAlignment.Center;
			}
			stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
			stackPanel.VerticalAlignment = VerticalAlignment.Center;
			legendContent.Children.Add(stackPanel);
		}

		private void DrawHorizontalGridlayout4Legend(ref Grid legendContent, out ScrollViewer scrollViewer)
		{
			Grid grid = new Grid();
			scrollViewer = new ScrollViewer();
			scrollViewer.BorderThickness = new Thickness(0.0);
			if (this.Entries.Count > 0)
			{
				int num = (from le in this.Entries
				where le.Labels != null
				select le.Labels.Count<string>()).Max() + 1;
				for (int i = 0; i < num; i++)
				{
					grid.ColumnDefinitions.Add(new ColumnDefinition
					{
						Width = GridLength.Auto
					});
				}
				for (int j = 1; j <= this.Entries.Count; j++)
				{
					grid.RowDefinitions.Add(new RowDefinition
					{
						Height = GridLength.Auto
					});
				}
				int num2 = 0;
				List<Rectangle> list = new List<Rectangle>();
				foreach (LegendEntry current in this.Entries)
				{
					int num3 = 0;
					if (current.IsCompleteLine)
					{
						Rectangle rectangle = new Rectangle
						{
							Height = 1.0,
							HorizontalAlignment = HorizontalAlignment.Stretch,
							Fill = new SolidColorBrush(Colors.Black)
						};
						list.Add(rectangle);
						rectangle.SetValue(Grid.RowProperty, num2);
						grid.Children.Add(rectangle);
					}
					else
					{
						Marker marker = current.Marker;
						if (marker != null)
						{
							marker.CreateVisual((base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
							marker.Visual.Margin = new Thickness(this.EntryMargin, this.EntryMargin, this.LabelMargin / 2.0, this.EntryMargin);
							marker.Visual.SetValue(Grid.RowProperty, num2);
							marker.Visual.SetValue(Grid.ColumnProperty, num3);
							marker.Visual.VerticalAlignment = VerticalAlignment.Center;
							grid.Children.Add(marker.Visual);
						}
						VerticalAlignment verticalAlignment = VerticalAlignment.Center;
						List<TextBlock> list2 = new List<TextBlock>();
						int num4 = 0;
						foreach (string current2 in current.Labels)
						{
							TextBlock textBlock = new TextBlock();
							textBlock.FlowDirection = ((base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
							list2.Add(textBlock);
							num3++;
							if (current.XAlignments[num4] != HorizontalAlignment.Left)
							{
								textBlock.Margin = new Thickness(this.LabelMargin, 0.0, 0.0, 0.0);
							}
							textBlock.Text = current2;
							textBlock.FlowDirection = ((base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
							this.ApplyFontProperty(textBlock);
							textBlock.SetValue(Grid.RowProperty, num2);
							textBlock.SetValue(Grid.ColumnProperty, num3);
							textBlock.HorizontalAlignment = current.XAlignments[num4];
							textBlock.VerticalAlignment = VerticalAlignment.Center;
							grid.Children.Add(textBlock);
							if (current2.Contains('\n'))
							{
								verticalAlignment = VerticalAlignment.Top;
							}
							num4++;
						}
						if (marker != null)
						{
							marker.Visual.VerticalAlignment = verticalAlignment;
						}
						foreach (TextBlock current3 in list2)
						{
							current3.VerticalAlignment = verticalAlignment;
						}
					}
					num2++;
				}
				foreach (Rectangle current4 in list)
				{
					current4.SetValue(Grid.ColumnSpanProperty, num);
				}
			}
			grid.HorizontalAlignment = HorizontalAlignment.Center;
			grid.VerticalAlignment = VerticalAlignment.Center;
			scrollViewer.Content = grid;
			scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
			scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
			legendContent.Children.Add(scrollViewer);
		}

		private Grid CreateLegendContent()
		{
			Grid grid = new Grid();
			ScrollViewer scrollViewer = null;
			this.InternalMaximumWidth -= 2.0 * this.InternalPadding.Left;
			this.InternalMaximumHeight -= 2.0 * this.InternalPadding.Left;
			if (this.Orientation == Orientation.Vertical)
			{
				if (this.Layout == Layouts.FlowLayout)
				{
					this.DrawVerticalFlowLayout4Legend(ref grid);
				}
				else if (this.Layout == Layouts.GridLayout)
				{
					this.DrawVerticalGridlayout4Legend(ref grid, out scrollViewer);
				}
			}
			else if (this.Orientation == Orientation.Horizontal)
			{
				if (this.Layout == Layouts.FlowLayout)
				{
					this.DrawHorizontalFlowLayout4Legend(ref grid);
				}
				else if (this.Layout == Layouts.GridLayout)
				{
					this.DrawHorizontalGridlayout4Legend(ref grid, out scrollViewer);
				}
			}
			grid.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			grid.Height = grid.DesiredSize.Height + this.InternalPadding.Left * 2.0;
			grid.Width = grid.DesiredSize.Width + this.InternalPadding.Left * 2.0;
			if (this.Layout == Layouts.GridLayout)
			{
				double num;
				if (double.IsInfinity(this.InternalMaximumWidth))
				{
					num = (base.Chart as Chart).ActualWidth * 0.5;
				}
				else
				{
					num = this.InternalMaximumWidth;
				}
				double num2;
				if (double.IsInfinity(this.InternalMaximumHeight))
				{
					num2 = (base.Chart as Chart).ActualHeight * 0.5;
				}
				else
				{
					num2 = this.InternalMaximumHeight;
				}
				if (grid.Width > num)
				{
					scrollViewer.Width = num - this.InternalPadding.Left * 2.0;
					grid.Width = num;
					scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
				}
				if (grid.Height > num2)
				{
					scrollViewer.Height = num2 - this.InternalPadding.Left * 2.0;
					scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
					grid.Height = num2;
				}
				if (scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Visible && grid.Width + 18.0 < num)
				{
					grid.Width += 18.0;
					if (!double.IsNaN(scrollViewer.Width))
					{
						scrollViewer.Width += 18.0;
					}
				}
				if (scrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible && grid.Height + 18.0 < num2)
				{
					grid.Height += 18.0;
					if (!double.IsNaN(scrollViewer.Height))
					{
						scrollViewer.Height += 18.0;
					}
				}
			}
			return grid;
		}

		internal override void FireMouseRightButtonDownEvent(object sender, object e)
		{
			if (this._onMouseRightButtonDown != null)
			{
				this._onMouseRightButtonDown(sender, new LegendMouseButtonEventArgs(e as MouseButtonEventArgs));
			}
		}

		internal override void FireMouseRightButtonUpEvent(object sender, object e)
		{
			if (this._onMouseRightButtonUp != null)
			{
				this._onMouseRightButtonUp(sender, new LegendMouseButtonEventArgs(e as MouseButtonEventArgs));
			}
		}

		internal override void FireMouseLeftButtonDownEvent(object sender, object e)
		{
			if (this._onMouseLeftButtonDown != null)
			{
				this._onMouseLeftButtonDown(sender, new LegendMouseButtonEventArgs(e as MouseButtonEventArgs));
			}
		}

		internal override void FireMouseLeftButtonUpEvent(object sender, object e)
		{
			if (this._onMouseLeftButtonUp != null)
			{
				this._onMouseLeftButtonUp(sender, new LegendMouseButtonEventArgs(e as MouseButtonEventArgs));
			}
		}

		internal override void FireMouseMoveEvent(object sender, object e)
		{
			if (this._onMouseMove != null)
			{
				this._onMouseMove(sender, new LegendMouseEventArgs(e as MouseEventArgs));
			}
		}

		internal override object GetMouseLeftButtonDownEventHandler()
		{
			return this._onMouseLeftButtonDown;
		}

		internal override object GetMouseLeftButtonUpEventHandler()
		{
			return this._onMouseLeftButtonUp;
		}

		internal override object GetMouseMoveEventHandler()
		{
			return this._onMouseMove;
		}

		internal override object GetMouseRightButtonDownEventHandler()
		{
			return this._onMouseRightButtonDown;
		}

		internal override object GetMouseRightButtonUpEventHandler()
		{
			return this._onMouseRightButtonUp;
		}

		internal string GetLegendName()
		{
			if (this._isAutoName)
			{
				string[] array = base.Name.Split(new char[]
				{
					'_'
				});
				return array[0];
			}
			return base.Name;
		}

		internal string GetLegendName4Series(string seriesLegendName)
		{
			if (seriesLegendName.Equals(base.Name))
			{
				return this.GetLegendName();
			}
			return seriesLegendName;
		}

		internal void CreateVisualObject()
		{
			if (!this.Enabled.Value)
			{
				this.Visual = null;
				return;
			}
			ElementData tag = new ElementData
			{
				Element = this,
				VisualElementName = "Legend"
			};
			this.Visual = new Border
			{
				Tag = tag
			};
			Grid grid = new Grid
			{
				Tag = tag
			};
			this.Visual.Child = grid;
			this.LegendContainer = new StackPanel
			{
				Tag = tag
			};
			if (!string.IsNullOrEmpty(this.Title))
			{
				Title title = new Title();
				this.ApplyFontProperty(title);
				if (this.TitleBackground != null)
				{
					title.InternalBackground = this.TitleBackground;
				}
				title.InternalHorizontalAlignment = this.TitleAlignmentX;
				title.InternalVerticalAlignment = VerticalAlignment.Top;
				title.TextAlignment = this.TitleTextAlignment;
				title.CreateVisualObject(tag, (base.Chart != null) ? base.Chart.FlowDirection : FlowDirection.LeftToRight);
				title.Visual.IsHitTestVisible = false;
				title.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				if (title.DesiredSize.Width > this.InternalMaxWidth)
				{
					title.Visual.Width = this.InternalMaxWidth;
				}
				this.LegendContainer.Children.Add(title.Visual);
			}
			Grid grid2 = this.CreateLegendContent();
			grid2.Tag = tag;
			this.LegendContainer.Children.Add(grid2);
			this.LegendContainer.VerticalAlignment = VerticalAlignment.Center;
			this.LegendContainer.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.ApplyVisualProperty();
			grid.Children.Add(this.LegendContainer);
			this.Visual.Cursor = this.Cursor;
			this.Visual.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			if (!double.IsPositiveInfinity(this.InternalMaxHeight) && this.InternalMaxHeight < this.Visual.DesiredSize.Height)
			{
				this.Visual.Height = this.InternalMaxHeight;
			}
			else
			{
				this.Visual.Height = this.Visual.DesiredSize.Height;
			}
			if (!double.IsPositiveInfinity(this.InternalMaxWidth) && this.InternalMaxWidth < this.Visual.DesiredSize.Width + this.InternalPadding.Left)
			{
				this.Visual.Width = this.InternalMaxWidth;
			}
			else if (this.Layout == Layouts.GridLayout)
			{
				this.Visual.Width = this.Visual.DesiredSize.Width + this.InternalPadding.Left;
			}
			else
			{
				this.Visual.Width = this.Visual.DesiredSize.Width;
			}
			PlotArea arg_2EB_0 = (base.Chart as Chart).PlotArea;
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			rectangleGeometry.Rect = new Rect(this.InternalBorderThickness.Left, this.InternalBorderThickness.Top, this.Visual.Width - this.InternalBorderThickness.Left - this.InternalBorderThickness.Right, this.Visual.Height - this.InternalBorderThickness.Top - this.InternalBorderThickness.Bottom);
			rectangleGeometry.RadiusX = this.CornerRadius.TopLeft;
			rectangleGeometry.RadiusY = this.CornerRadius.TopRight;
			this.LegendContainer.Clip = rectangleGeometry;
			this.ApplyShadow(grid);
			if (this.VerticalAlignment == VerticalAlignment.Bottom && (this.HorizontalAlignment == HorizontalAlignment.Center || this.HorizontalAlignment == HorizontalAlignment.Left || this.HorizontalAlignment == HorizontalAlignment.Right || this.HorizontalAlignment == HorizontalAlignment.Stretch))
			{
				this.Visual.Margin = new Thickness(0.0, 8.0, 0.0, 0.0);
				return;
			}
			if (this.VerticalAlignment == VerticalAlignment.Top && (this.HorizontalAlignment == HorizontalAlignment.Center || this.HorizontalAlignment == HorizontalAlignment.Left || this.HorizontalAlignment == HorizontalAlignment.Right || this.HorizontalAlignment == HorizontalAlignment.Stretch))
			{
				this.Visual.Margin = new Thickness(0.0, 0.0, 0.0, 8.0);
				return;
			}
			if (this.HorizontalAlignment == HorizontalAlignment.Left && (this.VerticalAlignment == VerticalAlignment.Center || this.VerticalAlignment == VerticalAlignment.Stretch))
			{
				this.Visual.Margin = new Thickness(0.0, 0.0, 8.0, 0.0);
				return;
			}
			if (this.HorizontalAlignment == HorizontalAlignment.Right && (this.VerticalAlignment == VerticalAlignment.Center || this.VerticalAlignment == VerticalAlignment.Stretch))
			{
				this.Visual.Margin = new Thickness(8.0, 0.0, 0.0, 0.0);
			}
		}
	}
}
