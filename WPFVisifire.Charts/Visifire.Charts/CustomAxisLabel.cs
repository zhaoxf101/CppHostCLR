using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Visifire.Commons;

namespace Visifire.Charts
{
	public class CustomAxisLabel : ObservableObject
	{
		public static readonly DependencyProperty TextProperty;

		public static readonly DependencyProperty ToProperty;

		public static readonly DependencyProperty FromProperty;

		public static readonly DependencyProperty LineColorProperty;

		public static readonly DependencyProperty LineThicknessProperty;

		public static readonly DependencyProperty FontColorProperty;

		private static bool _defaultStyleKeyApplied;

		private ElementData _tag;

		public Brush LineColor
		{
			get
			{
				if (base.GetValue(CustomAxisLabel.LineColorProperty) == null)
				{
					return this.Parent.LineColor;
				}
				return (Brush)base.GetValue(CustomAxisLabel.LineColorProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabel.LineColorProperty, value);
			}
		}

		public double? LineThickness
		{
			get
			{
				if (base.GetValue(CustomAxisLabel.LineThicknessProperty) == null)
				{
					return new double?(this.Parent.LineThickness);
				}
				return (double?)base.GetValue(CustomAxisLabel.LineThicknessProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabel.LineThicknessProperty, value);
			}
		}

		public Brush FontColor
		{
			get
			{
				if (base.GetValue(CustomAxisLabel.FontColorProperty) == null)
				{
					return this.Parent.FontColor;
				}
				return (Brush)base.GetValue(CustomAxisLabel.FontColorProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabel.FontColorProperty, value);
			}
		}

		public string Text
		{
			get
			{
				return (string)base.GetValue(CustomAxisLabel.TextProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabel.TextProperty, value);
			}
		}

		public object From
		{
			get
			{
				return base.GetValue(CustomAxisLabel.FromProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabel.FromProperty, value);
			}
		}

		public object To
		{
			get
			{
				return base.GetValue(CustomAxisLabel.ToProperty);
			}
			set
			{
				base.SetValue(CustomAxisLabel.ToProperty, value);
			}
		}

		internal DependencyObject ParentTree
		{
			get
			{
				return base.Parent;
			}
		}

		internal new CustomAxisLabels Parent
		{
			get;
			set;
		}

		internal TextBlock TextElement
		{
			get;
			set;
		}

		internal Path CustomLabelPath
		{
			get;
			set;
		}

		public CustomAxisLabel()
		{
			this._tag = new ElementData
			{
				Element = this
			};
		}

		static CustomAxisLabel()
		{
			CustomAxisLabel.TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CustomAxisLabel), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabel.OnTextPropertyChanged)));
			CustomAxisLabel.ToProperty = DependencyProperty.Register("To", typeof(object), typeof(CustomAxisLabel), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabel.OnToPropertyChanged)));
			CustomAxisLabel.FromProperty = DependencyProperty.Register("From", typeof(object), typeof(CustomAxisLabel), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabel.OnFromPropertyChanged)));
			CustomAxisLabel.LineColorProperty = DependencyProperty.Register("LineColor", typeof(Brush), typeof(CustomAxisLabel), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabel.OnLineColorPropertyChanged)));
			CustomAxisLabel.LineThicknessProperty = DependencyProperty.Register("LineThickness", typeof(double?), typeof(CustomAxisLabel), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabel.OnLineThicknessPropertyChanged)));
			CustomAxisLabel.FontColorProperty = DependencyProperty.Register("FontColor", typeof(Brush), typeof(CustomAxisLabel), new PropertyMetadata(new PropertyChangedCallback(CustomAxisLabel.OnFontColorPropertyChanged)));
			if (!CustomAxisLabel._defaultStyleKeyApplied)
			{
				FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomAxisLabel), new FrameworkPropertyMetadata(typeof(CustomAxisLabel)));
				CustomAxisLabel._defaultStyleKeyApplied = true;
			}
		}

		private static void OnFontColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabel customAxisLabel = d as CustomAxisLabel;
			customAxisLabel.UpdateVisual(VcProperties.FontColor, e.NewValue);
		}

		private static void OnLineColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabel customAxisLabel = d as CustomAxisLabel;
			customAxisLabel.UpdateVisual(VcProperties.LineColor, e.NewValue);
		}

		private static void OnLineThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabel customAxisLabel = d as CustomAxisLabel;
			customAxisLabel.UpdateVisual(VcProperties.LineThickness, e.NewValue);
		}

		private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabel customAxisLabel = d as CustomAxisLabel;
			customAxisLabel.FirePropertyChanged(VcProperties.Text);
		}

		private static void OnToPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabel customAxisLabel = d as CustomAxisLabel;
			customAxisLabel.FirePropertyChanged(VcProperties.To);
		}

		private static void OnFromPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomAxisLabel customAxisLabel = d as CustomAxisLabel;
			customAxisLabel.FirePropertyChanged(VcProperties.From);
		}

		private void ApplyCustomLabelProperties()
		{
			if (this.TextElement != null)
			{
				this.TextElement.Foreground = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.Parent.ParentAxis.Background, this.FontColor, false);
			}
			if (this.CustomLabelPath != null)
			{
				this.CustomLabelPath.Stroke = Visifire.Charts.Chart.CalculateFontColor(base.Chart as Chart, this.Parent.ParentAxis.Background, this.LineColor, false);
				this.CustomLabelPath.StrokeThickness = this.LineThickness.Value;
			}
		}

		internal override void UpdateVisual(VcProperties propertyName, object value)
		{
			if (this.TextElement != null || this.CustomLabelPath != null)
			{
				this.ApplyCustomLabelProperties();
				return;
			}
			base.FirePropertyChanged(propertyName);
		}
	}
}
