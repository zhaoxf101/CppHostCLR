using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class Title : ObservableObject
	{
		public static readonly DependencyProperty EnabledProperty;

		public static readonly DependencyProperty HrefTargetProperty;

		public static readonly DependencyProperty HrefProperty;

		public static readonly DependencyProperty FontColorProperty;

		public new static readonly DependencyProperty FontFamilyProperty;

		public new static readonly DependencyProperty FontSizeProperty;

		public new static readonly DependencyProperty FontStyleProperty;

		public new static readonly DependencyProperty FontWeightProperty;

		public static readonly DependencyProperty TextProperty;

		public static readonly DependencyProperty BorderColorProperty;

		public new static readonly DependencyProperty BorderThicknessProperty;

		public static readonly DependencyProperty CornerRadiusProperty;

		public static readonly DependencyProperty ShadowEnabledProperty;

		public new static readonly DependencyProperty BackgroundProperty;

		public new static readonly DependencyProperty HorizontalAlignmentProperty;

		public new static readonly DependencyProperty VerticalAlignmentProperty;

		public new static readonly DependencyProperty MarginProperty;

		public new static readonly DependencyProperty PaddingProperty;

		public new static readonly DependencyProperty OpacityProperty;

		public static readonly DependencyProperty TextAlignmentProperty;

		public static readonly DependencyProperty TextDecorationsProperty;

		public static readonly DependencyProperty DockInsidePlotAreaProperty;

		private double _internalOpacity = double.NaN;

		private double _internalFontSize = double.NaN;

		private FontFamily _internalFontFamily;

		internal Brush InternalFontColor;

		private FontStyle? _internalFontStyle = null;

		private FontWeight? _internalFontWeight = null;

		private Thickness? _borderThickness = null;

		private Brush _internalBackground;

		private HorizontalAlignment? _internalHorizontalAlignment = null;

		private VerticalAlignment? _internalVerticalAlignment = null;

		internal Thickness? _internalMargin = null;

		private Thickness? _internalPadding = null;

		private static bool _defaultStyleKeyApplied;

		public InlinesCollection Inlines
		{
			get;
			set;
		}

		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? Enabled
		{
			get
			{
				if (!((bool?)base.GetValue(Title.EnabledProperty)).HasValue)
				{
					return new bool?(true);
				}
				return (bool?)base.GetValue(Title.EnabledProperty);
			}
			set
			{
				base.SetValue(Title.EnabledProperty, value);
			}
		}

		public HrefTargets HrefTarget
		{
			get
			{
				return (HrefTargets)base.GetValue(Title.HrefTargetProperty);
			}
			set
			{
				base.SetValue(Title.HrefTargetProperty, value);
			}
		}

		public string Href
		{
			get
			{
				return (string)base.GetValue(Title.HrefProperty);
			}
			set
			{
				base.SetValue(Title.HrefProperty, value);
			}
		}

		public new double Opacity
		{
			get
			{
				return (double)base.GetValue(Title.OpacityProperty);
			}
			set
			{
				base.SetValue(Title.OpacityProperty, value);
			}
		}

		internal double InternalOpacity
		{
			get
			{
				return (double)(double.IsNaN(this._internalOpacity) ? base.GetValue(Title.OpacityProperty) : this._internalOpacity);
			}
			set
			{
				this._internalOpacity = value;
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

		public new FontFamily FontFamily
		{
			get
			{
				FontFamily fontFamily;
				if (this._internalFontFamily == null)
				{
					fontFamily = (FontFamily)base.GetValue(Title.FontFamilyProperty);
				}
				else
				{
					fontFamily = this._internalFontFamily;
				}
				if (fontFamily != null)
				{
					return (FontFamily)base.GetValue(Title.FontFamilyProperty);
				}
				return new FontFamily("Verdana");
			}
			set
			{
				base.SetValue(Title.FontFamilyProperty, value);
			}
		}

		internal FontFamily InternalFontFamily
		{
			get
			{
				FontFamily fontFamily;
				if (this._internalFontFamily == null)
				{
					fontFamily = (FontFamily)base.GetValue(Title.FontFamilyProperty);
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

		public new double FontSize
		{
			get
			{
				return (double)base.GetValue(Title.FontSizeProperty);
			}
			set
			{
				base.SetValue(Title.FontSizeProperty, value);
			}
		}

		internal double InternalFontSize
		{
			get
			{
				if (double.IsNaN(this._internalFontSize))
				{
					if ((double)base.GetValue(Title.FontSizeProperty) != 0.0)
					{
						return (double)base.GetValue(Title.FontSizeProperty);
					}
					return 10.0;
				}
				else
				{
					if (this._internalFontSize != 0.0)
					{
						return this._internalFontSize;
					}
					return 10.0;
				}
			}
			set
			{
				this._internalFontSize = value;
			}
		}

		public Brush FontColor
		{
			get
			{
				return (Brush)base.GetValue(Title.FontColorProperty);
			}
			set
			{
				base.SetValue(Title.FontColorProperty, value);
			}
		}

		[TypeConverter(typeof(FontStyleConverter))]
		public new FontStyle FontStyle
		{
			get
			{
				return (FontStyle)base.GetValue(Title.FontStyleProperty);
			}
			set
			{
				base.SetValue(Title.FontStyleProperty, value);
			}
		}

		[TypeConverter(typeof(FontStyleConverter))]
		internal FontStyle InternalFontStyle
		{
			get
			{
				return (FontStyle)((!this._internalFontStyle.HasValue) ? base.GetValue(Title.FontStyleProperty) : this._internalFontStyle);
			}
			set
			{
				this._internalFontStyle = new FontStyle?(value);
			}
		}

		[TypeConverter(typeof(FontWeightConverter))]
		public new FontWeight FontWeight
		{
			get
			{
				return (FontWeight)base.GetValue(Title.FontWeightProperty);
			}
			set
			{
				base.SetValue(Title.FontWeightProperty, value);
			}
		}

		[TypeConverter(typeof(FontWeightConverter))]
		internal FontWeight InternalFontWeight
		{
			get
			{
				return (FontWeight)((!this._internalFontWeight.HasValue) ? base.GetValue(Title.FontWeightProperty) : this._internalFontWeight);
			}
			set
			{
				this._internalFontWeight = new FontWeight?(value);
			}
		}

		public string Text
		{
			get
			{
				if (base.GetValue(Title.TextProperty) != null)
				{
					return (string)base.GetValue(Title.TextProperty);
				}
				return "";
			}
			set
			{
				base.SetValue(Title.TextProperty, value);
			}
		}

		public Brush BorderColor
		{
			get
			{
				return (Brush)base.GetValue(Title.BorderColorProperty);
			}
			set
			{
				base.SetValue(Title.BorderColorProperty, value);
			}
		}

		public new Thickness BorderThickness
		{
			get
			{
				return (Thickness)base.GetValue(Title.BorderThicknessProperty);
			}
			set
			{
				base.SetValue(Title.BorderThicknessProperty, value);
			}
		}

		internal Thickness InternalBorderThickness
		{
			get
			{
				return (Thickness)((!this._borderThickness.HasValue) ? base.GetValue(Title.BorderThicknessProperty) : this._borderThickness);
			}
			set
			{
				this._borderThickness = new Thickness?(value);
			}
		}

		[TypeConverter(typeof(CornerRadiusConverter))]
		public CornerRadius CornerRadius
		{
			get
			{
				return (CornerRadius)base.GetValue(Title.CornerRadiusProperty);
			}
			set
			{
				base.SetValue(Title.CornerRadiusProperty, value);
			}
		}

		public new Brush Background
		{
			get
			{
				return (Brush)base.GetValue(Title.BackgroundProperty);
			}
			set
			{
				base.SetValue(Title.BackgroundProperty, value);
			}
		}

		internal Brush InternalBackground
		{
			get
			{
				return (Brush)((this._internalBackground == null) ? base.GetValue(Title.BackgroundProperty) : this._internalBackground);
			}
			set
			{
				this._internalBackground = value;
			}
		}

		public new HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return (HorizontalAlignment)base.GetValue(Title.HorizontalAlignmentProperty);
			}
			set
			{
				base.SetValue(Title.HorizontalAlignmentProperty, value);
			}
		}

		internal HorizontalAlignment InternalHorizontalAlignment
		{
			get
			{
				return (HorizontalAlignment)((!this._internalHorizontalAlignment.HasValue) ? base.GetValue(Title.HorizontalAlignmentProperty) : this._internalHorizontalAlignment);
			}
			set
			{
				this._internalHorizontalAlignment = new HorizontalAlignment?(value);
			}
		}

		public new VerticalAlignment VerticalAlignment
		{
			get
			{
				return (VerticalAlignment)base.GetValue(Title.VerticalAlignmentProperty);
			}
			set
			{
				base.SetValue(Title.VerticalAlignmentProperty, value);
			}
		}

		internal VerticalAlignment InternalVerticalAlignment
		{
			get
			{
				return (VerticalAlignment)((!this._internalVerticalAlignment.HasValue) ? base.GetValue(Title.VerticalAlignmentProperty) : this._internalVerticalAlignment);
			}
			set
			{
				this._internalVerticalAlignment = new VerticalAlignment?(value);
			}
		}

		public new Thickness Margin
		{
			get
			{
				return (Thickness)base.GetValue(Title.MarginProperty);
			}
			set
			{
				base.SetValue(Title.MarginProperty, value);
			}
		}

		public bool ShadowEnabled
		{
			get
			{
				return (bool)base.GetValue(Title.ShadowEnabledProperty);
			}
			set
			{
				base.SetValue(Title.ShadowEnabledProperty, value);
			}
		}

		internal Thickness InternalMargin
		{
			get
			{
				return (Thickness)((!this._internalMargin.HasValue) ? base.GetValue(Title.MarginProperty) : this._internalMargin);
			}
			set
			{
				this._internalMargin = new Thickness?(value);
			}
		}

		public new Thickness Padding
		{
			get
			{
				return (Thickness)base.GetValue(Title.PaddingProperty);
			}
			set
			{
				base.SetValue(Title.PaddingProperty, value);
			}
		}

		public Thickness InternalPadding
		{
			get
			{
				return (Thickness)((!this._internalPadding.HasValue) ? base.GetValue(Title.PaddingProperty) : this._internalPadding);
			}
			set
			{
				this._internalPadding = new Thickness?(value);
			}
		}

		public TextAlignment TextAlignment
		{
			get
			{
				return (TextAlignment)base.GetValue(Title.TextAlignmentProperty);
			}
			set
			{
				base.SetValue(Title.TextAlignmentProperty, value);
			}
		}

		public TextDecorationCollection TextDecorations
		{
			get
			{
				return (TextDecorationCollection)base.GetValue(Title.TextDecorationsProperty);
			}
			set
			{
				base.SetValue(Title.TextDecorationsProperty, value);
			}
		}

		public bool DockInsidePlotArea
		{
			get
			{
				return (bool)base.GetValue(Title.DockInsidePlotAreaProperty);
			}
			set
			{
				base.SetValue(Title.DockInsidePlotAreaProperty, value);
			}
		}

		internal Border Visual
		{
			get;
			private set;
		}

		internal Size TextBlockDesiredSize
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

		private new Brush Foreground
		{
			get;
			set;
		}

		private Canvas InnerCanvas
		{
			get;
			set;
		}

		internal TextBlock TextElement
		{
			get;
			set;
		}

		private TextBlock ShadowTextElement
		{
			get;
			set;
		}

		private Grid ShadowGrid
		{
			get;
			set;
		}

		public Title()
		{
			this.SetDefaultStyle();
			this.Inlines = new InlinesCollection();
			this.Inlines.CollectionChanged += delegate(object A_1, NotifyCollectionChangedEventArgs A_2)
			{
				if (base.Parent != null && string.IsNullOrEmpty(this.Text))
				{
					base.FirePropertyChanged(VcProperties.Inlines);
				}
			};
		}

		static Title()
		{
			Title.EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool?), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnEnabledPropertyChanged)));
			Title.HrefTargetProperty = DependencyProperty.Register("HrefTarget", typeof(HrefTargets), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnHrefTargetChanged)));
			Title.HrefProperty = DependencyProperty.Register("Href", typeof(string), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnHrefChanged)));
			Title.FontColorProperty = DependencyProperty.Register("FontColor", typeof(Brush), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnFontColorPropertyChanged)));
			Title.FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnFontFamilyPropertyChanged)));
			Title.FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnFontSizePropertyChanged)));
			Title.FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnFontStylePropertyChanged)));
			Title.FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnFontWeightPropertyChanged)));
			Title.TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnTextPropertyChanged)));
			Title.BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(Brush), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnBorderColorPropertyChanged)));
			Title.BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnBorderThicknessPropertyChanged)));
			Title.CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnCornerRadiusPropertyChanged)));
			Title.ShadowEnabledProperty = DependencyProperty.Register("ShadowEnabled", typeof(bool), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnShadowEnabledPropertyChanged)));
			Title.BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnBackgroundPropertyChanged)));
			Title.HorizontalAlignmentProperty = DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnHorizontalAlignmentPropertyChanged)));
			Title.VerticalAlignmentProperty = DependencyProperty.Register("VerticalAlignment", typeof(VerticalAlignment), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnVerticalAlignmentPropertyChanged)));
			Title.MarginProperty = DependencyProperty.Register("Margin", typeof(Thickness), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnMarginPropertyChanged)));
			Title.PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnPaddingPropertyChanged)));
			Title.OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(Title), new PropertyMetadata(1.0, new PropertyChangedCallback(Title.OnOpacityPropertyChanged)));
			Title.TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnTextAlignmentPropertyChanged)));
			Title.TextDecorationsProperty = DependencyProperty.Register("TextDecorations", typeof(TextDecorationCollection), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnTextDecorationsPropertyChanged)));
			Title.DockInsidePlotAreaProperty = DependencyProperty.Register("DockInsidePlotArea", typeof(bool), typeof(Title), new PropertyMetadata(new PropertyChangedCallback(Title.OnDockInsidePlotAreaPropertyChanged)));
			if (!Title._defaultStyleKeyApplied)
			{
				FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Title), new FrameworkPropertyMetadata(typeof(Title)));
				Title._defaultStyleKeyApplied = true;
			}
		}

		internal Title(string text)
		{
			this.SetDefaultStyle();
			this.Text = text;
		}

		internal override void Bind()
		{
		}

		private static void OnFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.InternalFontSize = (double)e.NewValue;
			title.FirePropertyChanged(VcProperties.FontSize);
		}

		private static void OnEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.FirePropertyChanged(VcProperties.Enabled);
		}

		private static void OnHrefTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.FirePropertyChanged(VcProperties.HrefTarget);
		}

		private static void OnHrefChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.FirePropertyChanged(VcProperties.Href);
		}

		private static void OnFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			if (e.NewValue == null || e.OldValue == null)
			{
				title.InternalFontFamily = (FontFamily)e.NewValue;
				title.FirePropertyChanged(VcProperties.FontFamily);
				return;
			}
			if (e.NewValue.ToString() != e.OldValue.ToString())
			{
				title.InternalFontFamily = (FontFamily)e.NewValue;
				title.FirePropertyChanged(VcProperties.FontFamily);
			}
		}

		private static void OnOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.InternalOpacity = (double)e.NewValue;
			title.FirePropertyChanged(VcProperties.Opacity);
		}

		private static void OnFontColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.InternalFontColor = (Brush)e.NewValue;
			title.UpdateVisual(VcProperties.FontColor, e.NewValue);
		}

		private static void OnFontStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.InternalFontStyle = (FontStyle)e.NewValue;
			title.UpdateVisual(VcProperties.FontStyle, e.NewValue);
		}

		private static void OnFontWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.InternalFontWeight = (FontWeight)e.NewValue;
			title.UpdateVisual(VcProperties.FontWeight, e.NewValue);
		}

		private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.FirePropertyChanged(VcProperties.Text);
		}

		private static void OnBorderColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.UpdateVisual(VcProperties.BorderColor, e.NewValue);
		}

		private static void OnBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.InternalBorderThickness = (Thickness)e.NewValue;
			title.FirePropertyChanged(VcProperties.BorderThickness);
		}

		private static void OnCornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.FirePropertyChanged(VcProperties.CornerRadius);
		}

		private static void OnBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title._internalBackground = (Brush)e.NewValue;
			title.UpdateVisual(VcProperties.Background, e.NewValue);
		}

		private static void OnHorizontalAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.InternalHorizontalAlignment = (HorizontalAlignment)e.NewValue;
			title.FirePropertyChanged(VcProperties.HorizontalAlignment);
		}

		private static void OnVerticalAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.InternalVerticalAlignment = (VerticalAlignment)e.NewValue;
			title.FirePropertyChanged(VcProperties.VerticalAlignment);
		}

		private static void OnMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.InternalMargin = (Thickness)e.NewValue;
			title.FirePropertyChanged(VcProperties.Margin);
		}

		private static void OnShadowEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.FirePropertyChanged(VcProperties.ShadowEnabled);
		}

		private static void OnPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.InternalPadding = (Thickness)e.NewValue;
			title.FirePropertyChanged(VcProperties.Padding);
		}

		private static void OnTextAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.FirePropertyChanged(VcProperties.TextAlignment);
		}

		private static void OnTextDecorationsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.UpdateVisual(VcProperties.TextDecorations, e.NewValue);
		}

		private static void OnDockInsidePlotAreaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Title title = d as Title;
			title.FirePropertyChanged(VcProperties.DockInsidePlotArea);
		}

		private void SetDefaultStyle()
		{
		}

		private void UpdateInlines()
		{
			if (this.TextElement != null)
			{
				this.TextElement.Inlines.Clear();
				if (this.Inlines != null)
				{
					foreach (Inline current in this.Inlines)
					{
						this.TextElement.Inlines.Add(current);
					}
				}
			}
		}

		private bool ApplyProperties(Title title)
		{
			if (title.Visual != null)
			{
				title.Visual.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
				title.TextElement.FontFamily = title.InternalFontFamily;
				title.TextElement.FontSize = title.InternalFontSize;
				title.TextElement.FontStyle = title.InternalFontStyle;
				title.TextElement.FontWeight = title.InternalFontWeight;
				title.TextElement.Text = ObservableObject.GetFormattedMultilineText(title.Text);
				if (string.IsNullOrEmpty(title.Text))
				{
					this.UpdateInlines();
				}
				else
				{
					Inline item = title.TextElement.Inlines.First<Inline>();
					title.TextElement.Inlines.Clear();
					title.TextElement.Inlines.Add(item);
				}
				title.TextElement.Foreground = Visifire.Charts.Chart.CalculateFontColor(title.Chart as Chart, title.Background, title.InternalFontColor, title.DockInsidePlotArea);
				title.TextElement.TextWrapping = TextWrapping.Wrap;
				title.TextElement.TextDecorations = title.TextDecorations;
				if (!VisifireControl.IsMediaEffectsEnabled && title.ShadowTextElement != null)
				{
					title.ShadowTextElement.FontFamily = title.InternalFontFamily;
					title.ShadowTextElement.FontSize = title.InternalFontSize;
					title.ShadowTextElement.FontStyle = title.InternalFontStyle;
					title.ShadowTextElement.FontWeight = title.InternalFontWeight;
					title.ShadowTextElement.Text = ObservableObject.GetFormattedMultilineText(title.Text);
					title.ShadowTextElement.Foreground = new SolidColorBrush(Colors.Gray);
					title.ShadowTextElement.Foreground.Opacity = 0.6;
					title.ShadowTextElement.TextWrapping = TextWrapping.Wrap;
				}
				title.Visual.BorderBrush = title.BorderColor;
				title.Visual.Background = title.InternalBackground;
				title.Visual.VerticalAlignment = title.InternalVerticalAlignment;
				title.Visual.HorizontalAlignment = title.InternalHorizontalAlignment;
				title.Visual.BorderThickness = title.InternalBorderThickness;
				title.Visual.Margin = title.InternalMargin;
				title.Visual.Padding = title.InternalPadding;
				title.Visual.CornerRadius = title.CornerRadius;
				title.Visual.SetValue(Panel.ZIndexProperty, title.GetValue(Panel.ZIndexProperty));
				title.AttachToolTip(title.Chart, title, title.TextElement);
				title.AttachHref(title.Chart, title.TextElement, title.Href, title.HrefTarget);
				title.Visual.Cursor = ((title.Cursor == null) ? null : title.Cursor);
				return true;
			}
			return false;
		}

		private Brush GetParentColor(Chart chart)
		{
			Brush result = null;
			if (!this.DockInsidePlotArea)
			{
				if (chart.Background != null && !Graphics.AreBrushesEqual(chart.Background, new SolidColorBrush(Colors.Transparent)))
				{
					result = chart.Background;
				}
			}
			else if (chart.PlotArea.Background != null && !Graphics.AreBrushesEqual(chart.Background, new SolidColorBrush(Colors.Transparent)))
			{
				result = chart.PlotArea.Background;
			}
			else if (chart.Background != null && !Graphics.AreBrushesEqual(chart.Background, new SolidColorBrush(Colors.Transparent)))
			{
				result = chart.Background;
			}
			return result;
		}

		private void ApplyShadow()
		{
			if (this.ShadowEnabled)
			{
				if (VisifireControl.IsMediaEffectsEnabled)
				{
					DropShadowEffect effect = new DropShadowEffect
					{
						BlurRadius = 5.0,
						Direction = 315.0,
						ShadowDepth = 4.0,
						Opacity = 0.95,
						Color = Color.FromArgb(255, 185, 185, 185)
					};
					this.Visual.Effect = effect;
					return;
				}
				Brush parentColor = this.GetParentColor(base.Chart as Chart);
				if (((this.Background != null && !Graphics.AreBrushesEqual(this.Background, new SolidColorBrush(Colors.Transparent))) || (parentColor != null && !Graphics.AreBrushesEqual(parentColor, new SolidColorBrush(Colors.White)))) && (this.Background == null || parentColor == null || !Graphics.AreBrushesEqual(parentColor, this.Background)))
				{
					Size size = Graphics.CalculateVisualSize(this.Visual);
					this.ShadowGrid = ExtendedGraphics.Get2DRectangleShadow(this, size.Width + 4.0, size.Height + 4.0, this.CornerRadius, this.CornerRadius, 6.0);
					Size clipSize = new Size(this.ShadowGrid.Width, this.ShadowGrid.Height);
					this.ShadowGrid.Clip = ExtendedGraphics.GetShadowClip(clipSize, this.CornerRadius);
					this.ShadowGrid.SetValue(Panel.ZIndexProperty, -5);
					this.ShadowGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
					this.ShadowGrid.VerticalAlignment = VerticalAlignment.Stretch;
					this.ShadowGrid.Margin = new Thickness(-this.InternalPadding.Left, -this.InternalPadding.Top, 0.0, 0.0);
					this.InnerCanvas.Children.Insert(0, this.ShadowGrid);
					return;
				}
				if (this.ShadowTextElement != null)
				{
					this.ShadowTextElement.Visibility = Visibility.Visible;
					if ((this.HorizontalAlignment == HorizontalAlignment.Left || this.HorizontalAlignment == HorizontalAlignment.Right) && (this.VerticalAlignment == VerticalAlignment.Center || this.VerticalAlignment == VerticalAlignment.Stretch))
					{
						this.ShadowTextElement.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
						Size size2 = new Size(this.ShadowTextElement.DesiredSize.Width, this.ShadowTextElement.DesiredSize.Height);
						this.ShadowTextElement.SetValue(Canvas.LeftProperty, 2.0);
						this.ShadowTextElement.SetValue(Canvas.TopProperty, size2.Width + 2.0);
						return;
					}
					this.ShadowTextElement.SetValue(Canvas.LeftProperty, 2.0);
					this.ShadowTextElement.SetValue(Canvas.TopProperty, 2.0);
					return;
				}
			}
			else if (!VisifireControl.IsMediaEffectsEnabled)
			{
				if (this.InnerCanvas.Children.Contains(this.ShadowGrid))
				{
					this.InnerCanvas.Children.Remove(this.ShadowGrid);
					this.ShadowGrid = null;
				}
				if (this.ShadowTextElement != null)
				{
					this.ShadowTextElement.Visibility = Visibility.Collapsed;
					return;
				}
			}
			else
			{
				this.Visual.Effect = null;
			}
		}

		internal override void UpdateVisual(VcProperties propertyName, object value)
		{
			if (propertyName == VcProperties.Background)
			{
				if (this.Visual != null)
				{
					Binding binding = new Binding("Background");
					binding.Source = this;
					binding.Mode = BindingMode.TwoWay;
					this.Visual.SetBinding(Border.BackgroundProperty, binding);
					return;
				}
				base.FirePropertyChanged(propertyName);
				return;
			}
			else
			{
				if (propertyName != VcProperties.BorderColor)
				{
					if (!this.ApplyProperties(this))
					{
						base.FirePropertyChanged(propertyName);
					}
					return;
				}
				if (this.Visual != null)
				{
					this.Visual.BorderBrush = (Brush)value;
					return;
				}
				base.FirePropertyChanged(propertyName);
				return;
			}
		}

		internal void SetTextAlignment4LeftAndRight()
		{
			switch (this.TextAlignment)
			{
			case TextAlignment.Left:
				this.InnerCanvas.VerticalAlignment = VerticalAlignment.Bottom;
				return;
			case TextAlignment.Right:
				this.InnerCanvas.VerticalAlignment = VerticalAlignment.Top;
				return;
			case TextAlignment.Center:
				this.InnerCanvas.VerticalAlignment = VerticalAlignment.Center;
				return;
			default:
				return;
			}
		}

		internal void SetTextAlignment4TopAndBottom()
		{
			switch (this.TextAlignment)
			{
			case TextAlignment.Left:
				this.InnerCanvas.HorizontalAlignment = HorizontalAlignment.Left;
				return;
			case TextAlignment.Right:
				this.InnerCanvas.HorizontalAlignment = HorizontalAlignment.Right;
				return;
			case TextAlignment.Center:
				this.InnerCanvas.HorizontalAlignment = HorizontalAlignment.Center;
				return;
			default:
				return;
			}
		}

		private void SetAlignment4TextBlock(TextBlock textElement)
		{
			textElement.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
			this.TextBlockDesiredSize = new Size(textElement.DesiredSize.Width, textElement.DesiredSize.Height);
			if (this.InternalVerticalAlignment != VerticalAlignment.Center && this.InternalVerticalAlignment != VerticalAlignment.Stretch)
			{
				this.InnerCanvas.Height = this.TextBlockDesiredSize.Height;
				this.InnerCanvas.Width = this.TextBlockDesiredSize.Width;
				return;
			}
			if (this.InternalHorizontalAlignment == HorizontalAlignment.Left || this.InternalHorizontalAlignment == HorizontalAlignment.Right)
			{
				RotateTransform rotateTransform = new RotateTransform();
				rotateTransform.Angle = 270.0;
				textElement.RenderTransformOrigin = new Point(0.0, 0.0);
				textElement.RenderTransform = rotateTransform;
				this.InnerCanvas.Height = this.TextBlockDesiredSize.Width;
				this.InnerCanvas.Width = this.TextBlockDesiredSize.Height;
				textElement.SetValue(Canvas.LeftProperty, 0.0);
				textElement.SetValue(Canvas.TopProperty, this.TextBlockDesiredSize.Width);
				this.SetTextAlignment4LeftAndRight();
				return;
			}
			this.InnerCanvas.Height = this.TextBlockDesiredSize.Height;
			this.InnerCanvas.Width = this.TextBlockDesiredSize.Width;
			this.SetTextAlignment4TopAndBottom();
		}

		internal void CreateVisualObject(ElementData tag, FlowDirection flowDirection)
		{
			if (!this.Enabled.Value)
			{
				return;
			}
			this.Visual = new Border();
			if (this.TextElement != null)
			{
				this.TextElement.Inlines.Clear();
			}
			this.TextElement = new TextBlock
			{
				Tag = tag
			};
			this.TextElement.FlowDirection = flowDirection;
			this.InnerCanvas = new Canvas
			{
				Tag = tag
			};
			this.InnerCanvas.Children.Add(this.TextElement);
			if (!VisifireControl.IsMediaEffectsEnabled && this.ShadowEnabled)
			{
				this.ShadowTextElement = new TextBlock();
				this.ShadowTextElement.FlowDirection = flowDirection;
				this.ShadowTextElement.IsHitTestVisible = false;
				this.InnerCanvas.Children.Add(this.ShadowTextElement);
			}
			this.Visual.Child = this.InnerCanvas;
			this.Visual.Opacity = this.InternalOpacity;
			this.ApplyProperties(this);
			this.SetAlignment4TextBlock(this.TextElement);
			if (!VisifireControl.IsMediaEffectsEnabled && this.ShadowEnabled)
			{
				this.SetAlignment4TextBlock(this.ShadowTextElement);
			}
			base.Height = this.InnerCanvas.Height;
			base.Width = this.InnerCanvas.Width;
			this.SetTextAlignment4TopAndBottom();
			this.ApplyShadow();
			base.AttachEvents2Visual(this, this.TextElement);
			this.Visual.SnapsToDevicePixels = true;
			Size size = Graphics.CalculateVisualSize(this.Visual);
			this.Visual.Height = size.Height;
			this.Visual.Width = size.Width;
		}
	}
}
