using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Xml;
using Visifire.Charts;

namespace Visifire.Commons
{
	public class Marker
	{
		private bool _labelEnabled;

		private Brush _textBackground;

		internal bool _isLineWithoutMarker;

		private double _fontSize;

		private FontFamily _fontFamily;

		private FontStyle _fontStyle;

		private FontWeight _fontWeight;

		private Brush _fontColor;

		private double _borderThickness;

		private Brush _markerFillColor;

		private Brush _borderColor;

		private AlignmentX _textAlignmentX;

		private AlignmentY _textAlignmentY;

		private string _text;

		private double _scaleFactor = 1.0;

		private Point _markerShapePosition;

		public object Tag
		{
			get;
			set;
		}

		public Grid Visual
		{
			get;
			set;
		}

		public bool Bevel
		{
			get;
			set;
		}

		public double ScaleFactor
		{
			get
			{
				return this._scaleFactor;
			}
			set
			{
				if (value >= 1.0)
				{
					this._scaleFactor = value;
				}
			}
		}

		public double LabelPadding
		{
			get;
			set;
		}

		public Point Position
		{
			get;
			set;
		}

		public Size MarkerSize
		{
			get;
			set;
		}

		public Orientation TextOrientation
		{
			get;
			set;
		}

		public AlignmentX TextAlignmentX
		{
			get
			{
				return this._textAlignmentX;
			}
			set
			{
				this._textAlignmentX = value;
			}
		}

		public AlignmentY TextAlignmentY
		{
			get
			{
				return this._textAlignmentY;
			}
			set
			{
				this._textAlignmentY = value;
			}
		}

		public MarkerTypes MarkerType
		{
			get;
			set;
		}

		public double Opacity
		{
			get;
			set;
		}

		public double Margin
		{
			get;
			set;
		}

		public double LabelMargin
		{
			get;
			set;
		}

		public bool LabelEnabled
		{
			get
			{
				return this._labelEnabled;
			}
			set
			{
				this._labelEnabled = value;
				if (this.Visual != null)
				{
					if (value)
					{
						this.ShowLabel();
						return;
					}
					this.HideLabel();
				}
			}
		}

		public FontFamily FontFamily
		{
			get
			{
				if (this._fontFamily == null)
				{
					return new FontFamily("Arial");
				}
				return this._fontFamily;
			}
			set
			{
				this._fontFamily = value;
				if (this.TextBlock != null)
				{
					this.TextBlock.FontFamily = value;
				}
			}
		}

		public double FontSize
		{
			get
			{
				if (this._fontSize == 0.0)
				{
					this._fontSize = 10.0;
				}
				return this._fontSize;
			}
			set
			{
				this._fontSize = value;
			}
		}

		public Brush FontColor
		{
			get
			{
				if (this._fontColor != null)
				{
					return this._fontColor;
				}
				return new SolidColorBrush(Colors.Black);
			}
			set
			{
				this._fontColor = value;
				if (this.TextBlock != null)
				{
					this.TextBlock.Foreground = this._fontColor;
				}
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				return this._fontStyle;
			}
			set
			{
				this._fontStyle = value;
				if (this.TextBlock != null)
				{
					this.TextBlock.FontStyle = this._fontStyle;
				}
			}
		}

		public FontWeight FontWeight
		{
			get
			{
				return this._fontWeight;
			}
			set
			{
				this._fontWeight = value;
				if (this.TextBlock != null)
				{
					this.TextBlock.FontWeight = this._fontWeight;
				}
			}
		}

		public string Text
		{
			get
			{
				if (this._text != null)
				{
					return this._text;
				}
				return "";
			}
			set
			{
				this._text = value;
				if (this.TextBlock != null)
				{
					this.TextBlock.Text = this._text;
					Size size = Graphics.CalculateVisualSize(this.TextBlock);
					if (this.TextBackgroundCanvas != null)
					{
						this.TextBackgroundCanvas.Height = size.Height;
						this.TextBackgroundCanvas.Width = size.Width;
					}
				}
			}
		}

		public Brush TextBackground
		{
			get
			{
				return this._textBackground;
			}
			set
			{
				this._textBackground = value;
				if (this.TextBlock != null)
				{
					this.TextBlock.Text = this._text;
				}
				if (this.TextBackgroundCanvas != null)
				{
					this.TextBackgroundCanvas.Background = value;
				}
			}
		}

		public Brush BorderColor
		{
			get
			{
				if (this._borderColor == null)
				{
					this._borderColor = this.MarkerFillColor;
				}
				return this._borderColor;
			}
			set
			{
				this._borderColor = value;
				if (this.MarkerShape != null)
				{
					this.MarkerShape.Stroke = value;
				}
			}
		}

		public double BorderThickness
		{
			get
			{
				return this._borderThickness;
			}
			set
			{
				this._borderThickness = value;
				if (this.MarkerShape != null)
				{
					this.MarkerShape.StrokeThickness = value;
				}
			}
		}

		public Brush MarkerFillColor
		{
			get
			{
				return this._markerFillColor;
			}
			set
			{
				this._markerFillColor = value;
				if (this.MarkerShape != null)
				{
					this.MarkerShape.Fill = value;
				}
			}
		}

		public Shape MarkerShape
		{
			get;
			set;
		}

		internal bool IsEnabled
		{
			get;
			set;
		}

		internal FrameworkElement BevelLayer
		{
			get;
			set;
		}

		internal Point MarkerActualPosition
		{
			get;
			set;
		}

		internal Shape MarkerShadow
		{
			get;
			set;
		}

		internal VisifireControl Control
		{
			get;
			set;
		}

		internal TextBlock TextBlock
		{
			get;
			set;
		}

		internal Canvas LabelCanvas
		{
			get;
			set;
		}

		internal double LabelAngle
		{
			get;
			set;
		}

		internal LabelStyles LabelStyle
		{
			get;
			set;
		}

		internal Size MarkerActualSize
		{
			get;
			private set;
		}

		internal Size TextBlockSize
		{
			get;
			set;
		}

		internal Canvas TextBackgroundCanvas
		{
			get;
			set;
		}

		internal Effect Effect
		{
			get;
			set;
		}

		internal bool ShadowEnabled
		{
			get;
			set;
		}

		internal bool PixelLevelShadow
		{
			get;
			set;
		}

		internal DataSeries DataSeriesOfLegendMarker
		{
			get;
			set;
		}

		internal void AssignPropertiesValue(MarkerTypes markerType, double scaleFactor, Size markerSize, bool markerBevel, Brush markerColor, string markerLabelText)
		{
			this.MarkerType = markerType;
			this.MarkerFillColor = markerColor;
			this.Text = markerLabelText;
			this.TextOrientation = Orientation.Horizontal;
			this.TextAlignmentX = AlignmentX.Right;
			this.TextAlignmentY = AlignmentY.Center;
			this.ScaleFactor = scaleFactor;
			this.MarkerSize = markerSize;
			this.BorderThickness = 1.0;
			this.TextBackground = new SolidColorBrush(Colors.Transparent);
			this.Bevel = markerBevel;
			this.Opacity = 1.0;
			this.LabelAngle = double.NaN;
		}

		public void HideLabel()
		{
			if (this.TextBackgroundCanvas != null)
			{
				this.TextBackgroundCanvas.Background = Graphics.TRANSPARENT_BRUSH;
			}
			if (this.TextBlock != null)
			{
				this.TextBlock.Foreground = Graphics.TRANSPARENT_BRUSH;
			}
		}

		public void ShowLabel()
		{
			if (this.TextBackgroundCanvas != null)
			{
				this.TextBackgroundCanvas.Background = this.TextBackground;
			}
			if (this.TextBlock != null)
			{
				this.TextBlock.Foreground = this.FontColor;
			}
		}

		public void AddToParent(Canvas parentCanvas, double xPosition, double yPosition, Point anchorPoint)
		{
			parentCanvas.Children.Add(this.Visual);
			this.SetPosition(xPosition, yPosition, anchorPoint);
		}

		public void SetPosition(double xPosition, double yPosition, Point anchorPoint)
		{
			this.Position = new Point(xPosition, yPosition);
			Point markerActualPosition = this.CalculateActualPosition(xPosition, yPosition, anchorPoint);
			this.Visual.SetValue(Canvas.TopProperty, markerActualPosition.Y);
			this.Visual.SetValue(Canvas.LeftProperty, markerActualPosition.X);
			this.MarkerActualPosition = markerActualPosition;
		}

		public Point CalculateActualPosition(double xPosition, double yPosition, Point anchorPoint)
		{
			double arg_0E_0 = this.MarkerActualSize.Height;
			double arg_1D_0 = this.MarkerActualSize.Width;
			if (anchorPoint.X == 0.5)
			{
				xPosition -= this.MarkerShape.Width / 2.0 + ((this.TextAlignmentX == AlignmentX.Left) ? this._markerShapePosition.X : 0.0);
			}
			else if (anchorPoint.X == 1.0)
			{
				xPosition -= this.MarkerShape.Width + ((this.TextAlignmentX == AlignmentX.Left) ? this._markerShapePosition.X : 0.0);
			}
			if (anchorPoint.Y == 0.5)
			{
				yPosition -= this.MarkerShape.Height / 2.0 + ((this.TextAlignmentY == AlignmentY.Top) ? this._markerShapePosition.Y : 0.0);
			}
			else if (anchorPoint.Y == 1.0)
			{
				yPosition -= this.MarkerShape.Height + ((this.TextAlignmentY == AlignmentY.Top) ? this._markerShapePosition.Y : 0.0);
			}
			if (double.IsNaN(this.LabelAngle))
			{
				if (this.TextAlignmentX == AlignmentX.Center && this.TextBlockSize.Width > this.MarkerShape.Width)
				{
					xPosition -= (this.TextBlockSize.Width - this.MarkerShape.Width) / 2.0;
				}
				if (this.TextAlignmentY == AlignmentY.Center && this.TextBlockSize.Height > this.MarkerShape.Height)
				{
					yPosition -= (this.TextBlockSize.Height - this.MarkerShape.Height) / 2.0;
				}
			}
			this.MarkerActualPosition = new Point(xPosition, yPosition);
			return this.MarkerActualPosition;
		}

		public void AddToParent(Panel parent)
		{
			parent.Children.Add(this.Visual);
		}

		internal void ApplyEffects()
		{
			if (this.PixelLevelShadow)
			{
				if (this.Effect != null)
				{
					this.MarkerShape.Effect = this.Effect;
					return;
				}
				this.MarkerShape.Effect = null;
			}
		}

		internal void ApplyRemoveShadow()
		{
			if (this.ShadowEnabled)
			{
				if (!this.PixelLevelShadow)
				{
					this.MarkerShadow = this.GetShape();
					this.MarkerShadow.SetValue(Panel.ZIndexProperty, -2);
					this.MarkerShadow.Tag = this.Tag;
					this.MarkerShadow.Height = this.MarkerShape.Height;
					this.MarkerShadow.Width = this.MarkerShape.Width;
					this.MarkerShadow.Fill = this.GetMarkerShadowColor();
					TranslateTransform renderTransform = new TranslateTransform
					{
						X = 1.0,
						Y = 1.0
					};
					this.MarkerShadow.RenderTransform = renderTransform;
					this.MarkerShadow.Visibility = (this.ShadowEnabled ? Visibility.Visible : Visibility.Collapsed);
					this.MarkerShadow.SetValue(Grid.RowProperty, 1);
					this.MarkerShadow.SetValue(Grid.ColumnProperty, 1);
					this.Visual.Children.Add(this.MarkerShadow);
					this.MarkerShape.Effect = null;
					return;
				}
				if (this.Effect == null)
				{
					DropShadowEffect dropShadowEffect = new DropShadowEffect();
					dropShadowEffect.Color = Colors.Gray;
					dropShadowEffect.BlurRadius = 5.0;
					dropShadowEffect.ShadowDepth = 2.5;
					dropShadowEffect.Opacity = 0.9;
					dropShadowEffect.Direction = 315.0;
					this.MarkerShape.Effect = dropShadowEffect;
					return;
				}
			}
			else
			{
				if (this.Effect == null)
				{
					this.MarkerShape.Effect = null;
				}
				if (this.MarkerShadow != null)
				{
					this.Visual.Children.Remove(this.MarkerShadow);
				}
				this.MarkerShadow = null;
			}
		}

		public void CreateVisual(FlowDirection flowDirection)
		{
			this.Visual = new Grid();
			this.MarkerShape = this.GetShape();
			this.MarkerShape.Tag = this.Tag;
			if (!string.IsNullOrEmpty(this.Text))
			{
				this.MarkerShape.SetValue(Grid.RowProperty, 1);
				this.MarkerShape.SetValue(Grid.ColumnProperty, 1);
			}
			this.ApplyEffects();
			if (this.ShadowEnabled)
			{
				this.ApplyRemoveShadow();
			}
			this.Visual.Children.Add(this.MarkerShape);
			this.UpdateMarker();
			if (!string.IsNullOrEmpty(this.Text))
			{
				this.Visual.RowDefinitions.Add(new RowDefinition());
				this.Visual.RowDefinitions.Add(new RowDefinition());
				this.Visual.RowDefinitions.Add(new RowDefinition());
				this.Visual.ColumnDefinitions.Add(new ColumnDefinition());
				this.Visual.ColumnDefinitions.Add(new ColumnDefinition());
				this.Visual.ColumnDefinitions.Add(new ColumnDefinition());
				this.TextBlock = new TextBlock
				{
					Tag = this.Tag
				};
				this.ApplyTextBlockProperties(flowDirection);
				if (!double.IsNaN(this.LabelAngle))
				{
					this.LabelCanvas = new Canvas();
					this.TextBackgroundCanvas = new Canvas();
					this.TextBackgroundCanvas.Background = this.TextBackground;
					this.TextBackgroundCanvas.Children.Add(this.TextBlock);
					this.LabelCanvas.Children.Add(this.TextBackgroundCanvas);
					this.SetAlignment4Label();
					if (this.TextAlignmentX == AlignmentX.Left)
					{
						this.LabelCanvas.SetValue(Grid.ColumnProperty, 0);
					}
					else if (this.TextAlignmentX == AlignmentX.Right)
					{
						this.LabelCanvas.SetValue(Grid.ColumnProperty, 2);
					}
					else
					{
						this.LabelCanvas.SetValue(Grid.ColumnProperty, 1);
						this.LabelCanvas.HorizontalAlignment = HorizontalAlignment.Center;
					}
					if (this.TextAlignmentY == AlignmentY.Top)
					{
						this.LabelCanvas.SetValue(Grid.RowProperty, 0);
						this.LabelCanvas.VerticalAlignment = VerticalAlignment.Top;
					}
					else if (this.TextAlignmentY == AlignmentY.Bottom)
					{
						this.LabelCanvas.SetValue(Grid.RowProperty, 2);
						this.LabelCanvas.VerticalAlignment = VerticalAlignment.Top;
					}
					else
					{
						this.LabelCanvas.SetValue(Grid.RowProperty, 1);
						this.LabelCanvas.VerticalAlignment = VerticalAlignment.Top;
					}
					this.Visual.Children.Add(this.LabelCanvas);
				}
				else
				{
					if (this.TextBackground != null)
					{
						this.TextBackgroundCanvas = new Canvas();
						this.TextBackgroundCanvas.Background = this.TextBackground;
						this.Visual.Children.Add(this.TextBackgroundCanvas);
					}
					this.SetAlignment4Label();
					this.Visual.Children.Add(this.TextBlock);
				}
				this.TextBlock.Margin = new Thickness(this.LabelMargin, 0.0, 0.0, 0.0);
				this.Visual.Margin = new Thickness(this.Margin, this.Margin, this.Margin, this.Margin);
			}
			else
			{
				this.TextBlock = null;
				this.TextBackgroundCanvas = null;
				this.TextBlockSize = new Size(0.0, 0.0);
				this._markerShapePosition.X = 0.0;
				this._markerShapePosition.Y = 0.0;
			}
			this.MarkerShape.Opacity = this.Opacity;
			this.MarkerActualSize = Graphics.CalculateVisualSize(this.Visual);
		}

		public void UpdateMarker()
		{
			if (this.BevelLayer != null)
			{
				this.Visual.Children.Remove(this.BevelLayer);
			}
			if (this.Bevel)
			{
				this.BevelLayer = this.GetBevelLayer();
				this.BevelLayer.IsHitTestVisible = false;
				this.BevelLayer.SetValue(Grid.RowProperty, 1);
				this.BevelLayer.SetValue(Grid.ColumnProperty, 1);
				this.BevelLayer.SetValue(Grid.RowProperty, 1);
				this.BevelLayer.SetValue(Grid.ColumnProperty, 1);
				this.Visual.Children.Add(this.BevelLayer);
			}
			this.ApplyMarkerShapeProperties();
		}

		private void ApplyMarkerShapeProperties()
		{
			if (this.MarkerShape != null)
			{
				this.MarkerShape.StrokeThickness = this.BorderThickness;
				this.MarkerShape.Height = this.MarkerSize.Height * this.ScaleFactor;
				this.MarkerShape.Width = this.MarkerSize.Width * this.ScaleFactor;
				if (this.BorderThickness >= 0.0 && this.BorderColor != null)
				{
					this.MarkerShape.Stroke = (this.BorderColor.GetCurrentValueAsFrozen() as Brush);
				}
				if (this.MarkerFillColor != null)
				{
					this.MarkerShape.Fill = (this.MarkerFillColor.GetCurrentValueAsFrozen() as Brush);
				}
				if (this.MarkerShadow != null)
				{
					this.MarkerShadow.Height = this.MarkerSize.Height * this.ScaleFactor;
					this.MarkerShadow.Width = this.MarkerSize.Width * this.ScaleFactor;
					this.MarkerShadow.Stroke = null;
				}
			}
			if (this.BevelLayer != null)
			{
				this.BevelLayer.Height = this.MarkerSize.Height * this.ScaleFactor;
				this.BevelLayer.Width = this.MarkerSize.Width * this.ScaleFactor;
				if (this.BevelLayer is Shape)
				{
					(this.BevelLayer as Shape).StrokeThickness = this.BorderThickness;
				}
			}
		}

		private void ApplyTextBlockProperties(FlowDirection flowDirection)
		{
			if (this.TextBlock != null)
			{
				this.TextBlock.FlowDirection = flowDirection;
				this.TextBlock.FontFamily = this.FontFamily;
				this.TextBlock.FontSize = this.FontSize;
				this.TextBlock.FontStyle = this.FontStyle;
				this.TextBlock.FontWeight = this.FontWeight;
				this.TextBlock.Text = this.Text;
				this.TextBlock.Foreground = this.FontColor;
				this.TextBlock.VerticalAlignment = VerticalAlignment.Top;
			}
		}

		private Shape GetShape()
		{
			string s = null;
			PointCollection pointCollection = new PointCollection();
			switch (this.MarkerType)
			{
			case MarkerTypes.Circle:
				return new Ellipse
				{
					Height = 6.0,
					Width = 6.0
				};
			case MarkerTypes.Square:
				pointCollection.Add(new Point(0.0, 0.0));
				pointCollection.Add(new Point(0.0, 6.0));
				pointCollection.Add(new Point(6.0, 6.0));
				pointCollection.Add(new Point(6.0, 0.0));
				pointCollection.Add(new Point(0.0, 0.0));
				return new Polyline
				{
					StrokeMiterLimit = 1.0,
					StrokeLineJoin = PenLineJoin.Miter,
					Stretch = Stretch.Fill,
					Points = pointCollection
				};
			case MarkerTypes.Triangle:
				pointCollection.Add(new Point(3.0, 3.0));
				pointCollection.Add(new Point(0.0, 6.0));
				pointCollection.Add(new Point(6.0, 6.0));
				pointCollection.Add(new Point(3.0, 3.0));
				return new Polyline
				{
					StrokeMiterLimit = 1.0,
					StrokeLineJoin = PenLineJoin.Miter,
					Stretch = Stretch.Fill,
					Points = pointCollection
				};
			case MarkerTypes.Cross:
				pointCollection.Add(new Point(0.0, 0.0));
				pointCollection.Add(new Point(6.0, 6.0));
				pointCollection.Add(new Point(3.0, 3.0));
				pointCollection.Add(new Point(0.0, 6.0));
				pointCollection.Add(new Point(6.0, 0.0));
				pointCollection.Add(new Point(3.0, 3.0));
				return new Polyline
				{
					StrokeMiterLimit = 1.0,
					StrokeLineJoin = PenLineJoin.Miter,
					Stretch = Stretch.Fill,
					Points = pointCollection
				};
			case MarkerTypes.Diamond:
				pointCollection.Add(new Point(3.0, 0.0));
				pointCollection.Add(new Point(0.0, 3.0));
				pointCollection.Add(new Point(3.0, 6.0));
				pointCollection.Add(new Point(6.0, 3.0));
				pointCollection.Add(new Point(3.0, 0.0));
				return new Polyline
				{
					StrokeMiterLimit = 1.0,
					StrokeLineJoin = PenLineJoin.Miter,
					Stretch = Stretch.Fill,
					Points = pointCollection
				};
			case MarkerTypes.Line:
				if (this._isLineWithoutMarker)
				{
					s = string.Format("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Data=\"M215,305 L252,305\" Stretch=\"Fill\" Width=\"18\" Height=\"10\" StrokeStartLineCap=\"Round\" StrokeEndLineCap=\"Round\" VerticalAlignment=\"Center\" HorizontalAlignment=\"Center\" Stroke=\"#FF000000\" StrokeThickness=\"0.8\" Fill=\"#FFFFFFFF\" />", new object[0]);
				}
				else
				{
					s = string.Format("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Data=\"M188.63132,258.42844 L188.63132,258.42847 C188.9104,278.91281 201.84741,278.91315 202.18343,258.49094 203.19153,235.62109 187.90231,234.79944 188.63132,258.42844 z M202.56129,256.09153 C202.26549,256.27575 202.32971,260.25791 202.54637,260.48211 202.68528,260.62585 208.86081,260.26716 212.77637,260.48211 214.29462,260.56545 214.56375,256.40657 212.8964,256.19363 211.16966,255.97311 202.56129,256.09153 202.56129,256.09153 z M188.24223,260.6353 L177.59218,260.65346 C176.06613,260.87177 176.17243,255.97066 177.43324,255.91033 L188.28842,255.91961\" Stretch=\"Fill\" Width=\"18\" Height=\"8\" VerticalAlignment=\"Center\" HorizontalAlignment=\"Center\" Stroke=\"#FF000000\" Fill=\"#FFFFFFFF\" />", new object[0]);
				}
				break;
			}
			return (Shape)XamlReader.Load(new XmlTextReader(new StringReader(s)));
		}

		private FrameworkElement GetBevelLayer()
		{
			string s = null;
			Brush bevelTopBrush = Graphics.GetBevelTopBrush(this.MarkerFillColor);
			switch (this.MarkerType)
			{
			case MarkerTypes.Circle:
			{
				Ellipse ellipse = new Ellipse
				{
					Height = 6.0,
					Width = 6.0
				};
				GradientStopCollection gradientStopCollection = new GradientStopCollection();
				gradientStopCollection.Add(new GradientStop
				{
					Offset = 0.0,
					Color = Graphics.GetDarkerColor((bevelTopBrush as LinearGradientBrush).GradientStops[1].Color, 0.8)
				});
				gradientStopCollection.Add(new GradientStop
				{
					Offset = 0.6,
					Color = Graphics.GetDarkerColor((bevelTopBrush as LinearGradientBrush).GradientStops[1].Color, 0.8)
				});
				gradientStopCollection.Add(new GradientStop
				{
					Offset = 0.8,
					Color = (bevelTopBrush as LinearGradientBrush).GradientStops[1].Color
				});
				gradientStopCollection.Add(new GradientStop
				{
					Offset = 1.0,
					Color = (bevelTopBrush as LinearGradientBrush).GradientStops[1].Color
				});
				ellipse.Fill = new LinearGradientBrush
				{
					GradientStops = gradientStopCollection,
					StartPoint = new Point(0.5, 1.0),
					EndPoint = new Point(0.5, 0.0)
				};
				return ellipse;
			}
			case MarkerTypes.Square:
			{
				double height = this.MarkerSize.Height * this.ScaleFactor;
				double width = this.MarkerSize.Width * this.ScaleFactor;
				return ExtendedGraphics.Get2DRectangleBevel(this.Tag as FrameworkElement, width, height, 3.0, 3.0, bevelTopBrush, Graphics.GetBevelSideBrush(0.0, this.MarkerFillColor), Graphics.GetBevelSideBrush(120.0, this.MarkerFillColor), Graphics.GetBevelSideBrush(100.0, this.MarkerFillColor));
			}
			case MarkerTypes.Triangle:
			{
				string arg = (bevelTopBrush as LinearGradientBrush).GradientStops[1].Color.ToString();
				s = string.Format("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Height=\"8\" Margin=\"0,0,0,0\" Width=\"8\" Stretch=\"Fill\" StrokeThickness=\"0.8\" Data=\"M163.89264,40.166725 L160.00034,46.921658 L168.00789,46.922005 z\">\r\n    \t\t        <Path.Fill>\r\n    \t\t\t       <RadialGradientBrush>\r\n        \t\t\t       <GradientStop Color=\"{0}\" Offset=\"0\"/>\r\n                            <GradientStop Color=\"{0}\" Offset=\"0.6\"/>\r\n        \t\t\t        <GradientStop Color=\"{1}\" Offset=\"0.8\"/>\r\n        \t\t\t        <GradientStop Color=\"{1}\" Offset=\"1\"/>\r\n        \t\t        </RadialGradientBrush>\r\n    \t\t        </Path.Fill>\r\n    \t            </Path>", Graphics.GetDarkerColor((bevelTopBrush as LinearGradientBrush).GradientStops[1].Color, 0.8).ToString(), arg);
				break;
			}
			case MarkerTypes.Cross:
			{
				bevelTopBrush = Graphics.GetBevelTopBrush(this.BorderColor);
				string arg = (bevelTopBrush as LinearGradientBrush).GradientStops[2].Color.ToString();
				s = string.Format("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Height=\"6\" Width=\"6\" Stretch=\"Fill\" Data=\"M126.66666,111 L156.33333,84.333336 M156.00032,111 L126.33299,84.667\" StrokeThickness=\"0.5\" >\r\n                    <Path.Stroke>\r\n    \t\t\t        <RadialGradientBrush>\r\n        \t\t\t       <GradientStop Color=\"{0}\" Offset=\"0\"/>\r\n                         \r\n        \t\t\t        <GradientStop Color=\"{1}\" Offset=\"0.9889\"/>\r\n        \t\t\t        <GradientStop Color=\"{1}\" Offset=\"1\"/>\r\n        \t\t        </RadialGradientBrush>\r\n    \t\t        </Path.Stroke>\r\n    \t            </Path>", Graphics.GetDarkerColor((bevelTopBrush as LinearGradientBrush).GradientStops[2].Color, 0.8).ToString(), arg);
				break;
			}
			case MarkerTypes.Diamond:
			{
				string arg = (bevelTopBrush as LinearGradientBrush).GradientStops[2].Color.ToString();
				s = string.Format("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Height=\"11\" Width=\"8\" Stretch=\"Fill\" Data=\"M97.374908,52.791668 L94.541656,57.041668 L97.386475,61.010586 L100.59509,57.010422 z\" StrokeThickness=\"0.8\">\r\n    \t\t        <Path.Fill>\r\n    \t\t\t       <RadialGradientBrush>\r\n        \t\t\t       <GradientStop Color=\"{0}\" Offset=\"0\"/>\r\n                            <GradientStop Color=\"{0}\" Offset=\"0.6\"/>\r\n        \t\t\t        <GradientStop Color=\"{1}\" Offset=\"0.75\"/>\r\n        \t\t\t        <GradientStop Color=\"{1}\" Offset=\"1\"/>\r\n        \t\t        </RadialGradientBrush>\r\n    \t\t        </Path.Fill>\r\n    \t            </Path>", Graphics.GetDarkerColor((bevelTopBrush as LinearGradientBrush).GradientStops[2].Color, 0.8).ToString(), arg);
				break;
			}
			case MarkerTypes.Line:
			{
				string arg = (bevelTopBrush as LinearGradientBrush).GradientStops[1].Color.ToString();
				s = string.Format("<Path Margin=\"0,0,0,0\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Data=\"M188.63132,258.42844 L188.63132,258.42847 C188.9104,278.91281 201.84741,278.91315 202.18343,258.49094 203.19153,235.62109 187.90231,234.79944 188.63132,258.42844 z M202.56129,256.09153 C202.26549,256.27575 202.32971,260.25791 202.54637,260.48211 202.68528,260.62585 208.86081,260.26716 212.77637,260.48211 214.29462,260.56545 214.56375,256.40657 212.8964,256.19363 211.16966,255.97311 202.56129,256.09153 202.56129,256.09153 z M188.24223,260.6353 L177.59218,260.65346 C176.06613,260.87177 176.17243,255.97066 177.43324,255.91033 L188.28842,255.91961\" Stretch=\"Fill\" StrokeThickness=\"1\" Width=\"19\" Height=\"8\" VerticalAlignment=\"Center\" HorizontalAlignment=\"Center\" Stroke=\"#FF000000\" >\r\n                    <Path.Fill>\r\n    \t\t\t       <RadialGradientBrush>\r\n        \t\t\t       <GradientStop Color=\"{0}\" Offset=\"0\"/>\r\n                            <GradientStop Color=\"{0}\" Offset=\"0.6\"/>\r\n        \t\t\t        <GradientStop Color=\"{1}\" Offset=\"0.8\"/>\r\n        \t\t\t        <GradientStop Color=\"{1}\" Offset=\"1\"/>\r\n        \t\t        </RadialGradientBrush>\r\n    \t\t        </Path.Fill>\r\n    \t            </Path>", Graphics.GetDarkerColor((bevelTopBrush as LinearGradientBrush).GradientStops[1].Color, 0.8).ToString(), arg);
				break;
			}
			}
			return (Shape)XamlReader.Load(new XmlTextReader(new StringReader(s)));
		}

		private void SetRotateTransformValues(ref RotateTransform rotateTransform, double centerX, double centerY, double angle)
		{
			rotateTransform.CenterX = centerX;
			rotateTransform.CenterY = centerY;
			rotateTransform.Angle = angle;
		}

		private void SetRotation(double radius, double angle, double angleInRadian, Point centerOfRotation)
		{
			double num = centerOfRotation.X + radius * Math.Cos(angleInRadian);
			double num2 = centerOfRotation.Y + radius * Math.Sin(angleInRadian);
			num2 -= this.TextBlockSize.Height / 2.0;
			if (this.TextBackgroundCanvas != null)
			{
				this.TextBackgroundCanvas.SetValue(Canvas.LeftProperty, num);
				this.TextBackgroundCanvas.SetValue(Canvas.TopProperty, num2);
				this.TextBackgroundCanvas.Width = this.TextBlockSize.Width;
				this.TextBackgroundCanvas.Height = this.TextBlockSize.Height;
				this.TextBlock.SetValue(Canvas.TopProperty, -2.0);
				this.TextBackgroundCanvas.RenderTransformOrigin = new Point(0.0, 0.5);
				this.TextBackgroundCanvas.RenderTransform = new RotateTransform
				{
					CenterX = 0.0,
					CenterY = 0.0,
					Angle = angle
				};
			}
		}

		internal void SetAlignment4Label()
		{
			if (this.TextBlock != null)
			{
				this.TextBlockSize = Graphics.CalculateVisualSize(this.TextBlock);
				if (this.TextOrientation == Orientation.Vertical)
				{
					TransformGroup transformGroup = new TransformGroup();
					RotateTransform rotateTransform = new RotateTransform();
					TranslateTransform translateTransform = new TranslateTransform();
					transformGroup.Children.Add(rotateTransform);
					transformGroup.Children.Add(translateTransform);
					switch (this.TextAlignmentY)
					{
					case AlignmentY.Top:
						switch (this.TextAlignmentX)
						{
						case AlignmentX.Left:
							this.TextBlock.RenderTransformOrigin = new Point(0.0, 1.0);
							this.SetRotateTransformValues(ref rotateTransform, 0.0, 0.0, -90.0);
							translateTransform.X = this.TextBlockSize.Width;
							break;
						case AlignmentX.Center:
							if (double.IsNaN(this.LabelAngle))
							{
								this.SetRotateTransformValues(ref rotateTransform, this.TextBlockSize.Width / 2.0, this.TextBlockSize.Height / 2.0, -90.0);
								translateTransform.Y = -this.TextBlockSize.Width / 2.0 + this.TextBlockSize.Height / 2.0 - 5.0;
								translateTransform.X = -1.0;
							}
							else
							{
								Point centerOfRotation = new Point(this.Position.X, this.Position.Y);
								double num = this.MarkerSize.Height / 2.0 * this.ScaleFactor;
								if (this.LabelStyle == LabelStyles.OutSide)
								{
									if (this.LabelAngle > 0.0 && this.LabelAngle <= 90.0)
									{
										double num2 = this.LabelAngle - 180.0;
										double num3 = 0.017453292519943295 * num2;
										num += this.TextBlockSize.Width;
										num2 = (num3 - 3.1415926535897931) * 57.295779513082323;
										this.SetRotation(num, num2, num3, centerOfRotation);
									}
									else if (this.LabelAngle >= -90.0 && this.LabelAngle < 0.0)
									{
										double num2 = this.LabelAngle;
										double num3 = 0.017453292519943295 * num2;
										this.SetRotation(num, num2, num3, centerOfRotation);
									}
								}
								else
								{
									centerOfRotation = new Point(this.Position.X, this.Position.Y + this.MarkerSize.Height / 2.0);
									if (this.LabelAngle >= -90.0 && this.LabelAngle < 0.0)
									{
										double num2 = 180.0 + this.LabelAngle;
										double num3 = 0.017453292519943295 * num2;
										num += this.TextBlockSize.Width + 3.0;
										num2 = (num3 - 3.1415926535897931) * 57.295779513082323;
										this.SetRotation(num, num2, num3, centerOfRotation);
									}
									else if (this.LabelAngle > 0.0 && this.LabelAngle <= 90.0)
									{
										num += 3.0;
										double num2 = this.LabelAngle;
										double num3 = 0.017453292519943295 * num2;
										this.SetRotation(num, num2, num3, centerOfRotation);
									}
								}
							}
							break;
						case AlignmentX.Right:
							this.TextBlock.RenderTransformOrigin = new Point(0.0, 1.0);
							this.SetRotateTransformValues(ref rotateTransform, 0.0, 0.0, -90.0);
							translateTransform.X = this.TextBlockSize.Height;
							break;
						}
						break;
					case AlignmentY.Center:
						switch (this.TextAlignmentX)
						{
						case AlignmentX.Left:
							if (double.IsNaN(this.LabelAngle))
							{
								this.SetRotateTransformValues(ref rotateTransform, this.TextBlockSize.Width, this.TextBlockSize.Height / 2.0, 90.0);
								translateTransform.Y = this.TextBlockSize.Width / 2.0;
								translateTransform.X = -this.TextBlockSize.Height / 2.0;
							}
							else
							{
								Point centerOfRotation2 = new Point(this.Position.X + this.MarkerSize.Width / 2.0, this.Position.Y + this.MarkerSize.Height / 2.0);
								double num4 = this.MarkerSize.Width / 2.0 * this.ScaleFactor;
								if (this.LabelStyle == LabelStyles.OutSide)
								{
									if (this.LabelAngle > 0.0 && this.LabelAngle <= 90.0)
									{
										double num5 = this.LabelAngle - 180.0;
										double num6 = 0.017453292519943295 * num5;
										num4 += this.TextBlockSize.Width + 3.0;
										num5 = (num6 - 3.1415926535897931) * 57.295779513082323;
										this.SetRotation(num4, num5, num6, centerOfRotation2);
									}
									else if (this.LabelAngle >= -90.0 && this.LabelAngle < 0.0)
									{
										double num5 = this.LabelAngle - 180.0;
										double num6 = 0.017453292519943295 * num5;
										num4 += this.TextBlockSize.Width + 3.0;
										num5 = (num6 - 3.1415926535897931) * 57.295779513082323;
										this.SetRotation(num4, num5, num6, centerOfRotation2);
									}
								}
								else if (this.LabelAngle <= 90.0 && this.LabelAngle >= -90.0)
								{
									double num5 = this.LabelAngle;
									num4 += 3.0;
									double num6 = 0.017453292519943295 * num5;
									this.SetRotation(num4, num5, num6, centerOfRotation2);
								}
							}
							break;
						case AlignmentX.Center:
							if (double.IsNaN(this.LabelAngle))
							{
								this.TextBlock.RenderTransformOrigin = new Point(0.5, 0.5);
								rotateTransform.Angle = 90.0;
							}
							else
							{
								Point centerOfRotation3 = new Point(this.Position.X, this.Position.Y + this.MarkerShape.Height / 2.0);
								double num7 = 4.0;
								if (this.LabelAngle > 0.0 && this.LabelAngle <= 90.0)
								{
									double num8 = this.LabelAngle - 180.0;
									double num9 = 0.017453292519943295 * num8;
									num7 += this.TextBlockSize.Width;
									num8 = (num9 - 3.1415926535897931) * 57.295779513082323;
									this.SetRotation(num7, num8, num9, centerOfRotation3);
								}
								else if (this.LabelAngle >= -90.0 && this.LabelAngle < 0.0)
								{
									double num8 = this.LabelAngle;
									double num9 = 0.017453292519943295 * num8;
									this.SetRotation(num7, num8, num9, centerOfRotation3);
								}
							}
							break;
						case AlignmentX.Right:
							if (double.IsNaN(this.LabelAngle))
							{
								this.SetRotateTransformValues(ref rotateTransform, 0.0, this.TextBlockSize.Height / 2.0, -90.0);
								translateTransform.Y = this.TextBlockSize.Width / 2.0;
								translateTransform.X = this.TextBlockSize.Height / 2.0;
							}
							else
							{
								Point centerOfRotation4 = new Point(this.Position.X - this.MarkerSize.Width / 2.0, this.Position.Y + this.MarkerSize.Height / 2.0);
								double num10 = this.MarkerSize.Width / 2.0 * this.ScaleFactor;
								if (this.LabelStyle == LabelStyles.OutSide)
								{
									if (this.LabelAngle <= 90.0 && this.LabelAngle >= -90.0)
									{
										double num11 = this.LabelAngle;
										num10 += 3.0;
										double num12 = 0.017453292519943295 * num11;
										this.SetRotation(num10, num11, num12, centerOfRotation4);
									}
								}
								else if (this.LabelAngle > 0.0 && this.LabelAngle <= 90.0)
								{
									double num11 = this.LabelAngle - 180.0;
									double num12 = 0.017453292519943295 * num11;
									num10 += this.TextBlockSize.Width + 3.0;
									num11 = (num12 - 3.1415926535897931) * 57.295779513082323;
									this.SetRotation(num10, num11, num12, centerOfRotation4);
								}
								else if (this.LabelAngle >= -90.0 && this.LabelAngle < 0.0)
								{
									double num11 = this.LabelAngle - 180.0;
									double num12 = 0.017453292519943295 * num11;
									num10 += this.TextBlockSize.Width + 3.0;
									num11 = (num12 - 3.1415926535897931) * 57.295779513082323;
									this.SetRotation(num10, num11, num12, centerOfRotation4);
								}
							}
							break;
						}
						break;
					case AlignmentY.Bottom:
						switch (this.TextAlignmentX)
						{
						case AlignmentX.Left:
							this.SetRotateTransformValues(ref rotateTransform, this.TextBlockSize.Width / 2.0, this.TextBlockSize.Height / 2.0, -90.0);
							translateTransform.Y = this.TextBlockSize.Width / 2.0 - this.TextBlockSize.Height / 2.0;
							translateTransform.X = this.TextBlockSize.Width / 2.0 - this.TextBlockSize.Height / 2.0;
							break;
						case AlignmentX.Center:
							if (double.IsNaN(this.LabelAngle))
							{
								this.SetRotateTransformValues(ref rotateTransform, this.TextBlockSize.Width / 2.0, this.TextBlockSize.Height / 2.0, -90.0);
								translateTransform.Y = this.TextBlockSize.Width / 2.0;
								translateTransform.X = -1.0;
							}
							else
							{
								Point centerOfRotation5 = default(Point);
								double num13 = this.MarkerSize.Height / 2.0 * this.ScaleFactor;
								if (this.LabelStyle == LabelStyles.OutSide)
								{
									centerOfRotation5 = new Point(this.Position.X, this.Position.Y);
									if (this.LabelAngle >= -90.0 && this.LabelAngle < 0.0)
									{
										double num14 = 180.0 + this.LabelAngle;
										double num15 = 0.017453292519943295 * num14;
										num13 += this.TextBlockSize.Width;
										num14 = (num15 - 3.1415926535897931) * 57.295779513082323;
										this.SetRotation(num13, num14, num15, centerOfRotation5);
									}
									else if (this.LabelAngle > 0.0 && this.LabelAngle <= 90.0)
									{
										double num14 = this.LabelAngle;
										double num15 = 0.017453292519943295 * num14;
										this.SetRotation(num13, num14, num15, centerOfRotation5);
									}
								}
								else
								{
									centerOfRotation5 = new Point(this.Position.X, this.Position.Y - this.MarkerSize.Height / 2.0);
									if (this.LabelAngle > 0.0 && this.LabelAngle <= 90.0)
									{
										double num14 = this.LabelAngle - 180.0;
										double num15 = 0.017453292519943295 * num14;
										num13 += this.TextBlockSize.Width + 3.0;
										num14 = (num15 - 3.1415926535897931) * 57.295779513082323;
										this.SetRotation(num13, num14, num15, centerOfRotation5);
									}
									else if (this.LabelAngle >= -90.0 && this.LabelAngle < 0.0)
									{
										num13 += 3.0;
										double num14 = this.LabelAngle;
										double num15 = 0.017453292519943295 * num14;
										this.SetRotation(num13, num14, num15, centerOfRotation5);
									}
								}
							}
							break;
						case AlignmentX.Right:
							this.SetRotateTransformValues(ref rotateTransform, 0.0, 0.0, -90.0);
							translateTransform.Y = this.TextBlockSize.Width;
							break;
						}
						break;
					}
					if (double.IsNaN(this.LabelAngle))
					{
						this.TextBlock.RenderTransform = transformGroup;
						if (this.TextBackgroundCanvas != null)
						{
							this.TextBackgroundCanvas.RenderTransformOrigin = this.TextBlock.RenderTransformOrigin;
							this.TextBackgroundCanvas.RenderTransform = transformGroup;
							this.TextBackgroundCanvas.Height = this.TextBlockSize.Height;
							this.TextBackgroundCanvas.Width = this.TextBlockSize.Width;
						}
					}
				}
				else if (this.DataSeriesOfLegendMarker == null)
				{
					TransformGroup transformGroup2 = new TransformGroup();
					RotateTransform value = new RotateTransform();
					TranslateTransform translateTransform2 = new TranslateTransform();
					transformGroup2.Children.Add(value);
					transformGroup2.Children.Add(translateTransform2);
					switch (this.TextAlignmentY)
					{
					case AlignmentY.Top:
					{
						AlignmentX textAlignmentX = this.TextAlignmentX;
						if (textAlignmentX == AlignmentX.Center && double.IsNaN(this.LabelAngle))
						{
							this.SetRotateTransformValues(ref value, 0.0, 0.0, 0.0);
							translateTransform2.Y = -3.0;
						}
						break;
					}
					case AlignmentY.Center:
						switch (this.TextAlignmentX)
						{
						case AlignmentX.Left:
							if (double.IsNaN(this.LabelAngle))
							{
								this.SetRotateTransformValues(ref value, 0.0, 0.0, 0.0);
								translateTransform2.X = -5.0;
								translateTransform2.Y = -1.0;
							}
							break;
						case AlignmentX.Center:
							if (double.IsNaN(this.LabelAngle))
							{
								this.SetRotateTransformValues(ref value, 0.0, 0.0, 0.0);
								translateTransform2.Y = -1.0;
							}
							break;
						case AlignmentX.Right:
							if (double.IsNaN(this.LabelAngle))
							{
								this.SetRotateTransformValues(ref value, 0.0, 0.0, 0.0);
								translateTransform2.X = 4.0;
								translateTransform2.Y = -1.0;
							}
							break;
						}
						break;
					case AlignmentY.Bottom:
					{
						AlignmentX textAlignmentX = this.TextAlignmentX;
						if (textAlignmentX == AlignmentX.Center && double.IsNaN(this.LabelAngle))
						{
							this.SetRotateTransformValues(ref value, 0.0, 0.0, 0.0);
							translateTransform2.Y = 1.0;
						}
						break;
					}
					}
					if (double.IsNaN(this.LabelAngle))
					{
						this.TextBlock.RenderTransform = transformGroup2;
						if (this.TextBackgroundCanvas != null)
						{
							this.TextBackgroundCanvas.RenderTransformOrigin = this.TextBlock.RenderTransformOrigin;
							this.TextBackgroundCanvas.RenderTransform = transformGroup2;
							this.TextBackgroundCanvas.Height = this.TextBlockSize.Height;
							this.TextBackgroundCanvas.Width = this.TextBlockSize.Width;
						}
					}
				}
				this.SetLabelBackgroundAndSymbolPosition();
			}
		}

		private void SetLabelBackgroundAndSymbolPosition()
		{
			if (double.IsNaN(this.LabelAngle))
			{
				if (this.TextBackgroundCanvas != null)
				{
					this.TextBackgroundCanvas.Height = this.TextBlockSize.Height;
					this.TextBackgroundCanvas.Width = this.TextBlockSize.Width;
				}
				if (this.TextAlignmentX == AlignmentX.Left)
				{
					this.TextBlock.SetValue(Grid.ColumnProperty, 0);
					this._markerShapePosition.X = this.TextBlockSize.Width;
					if (this.TextBackgroundCanvas != null)
					{
						this.TextBackgroundCanvas.SetValue(Grid.ColumnProperty, 0);
					}
				}
				else if (this.TextAlignmentX == AlignmentX.Right)
				{
					this.TextBlock.SetValue(Grid.ColumnProperty, 2);
					if (this.TextBackgroundCanvas != null)
					{
						this.TextBackgroundCanvas.SetValue(Grid.ColumnProperty, 2);
					}
				}
				else
				{
					this.TextBlock.SetValue(Grid.ColumnProperty, 1);
					this.TextBlock.HorizontalAlignment = HorizontalAlignment.Center;
					if (this.TextBackgroundCanvas != null)
					{
						this.TextBackgroundCanvas.SetValue(Grid.ColumnProperty, 1);
						this.TextBackgroundCanvas.HorizontalAlignment = HorizontalAlignment.Center;
					}
				}
				if (this.TextAlignmentY == AlignmentY.Top)
				{
					this.TextBlock.SetValue(Grid.RowProperty, 0);
					this._markerShapePosition.Y = this.TextBlockSize.Height;
					if (this.TextBackgroundCanvas != null)
					{
						this.TextBackgroundCanvas.SetValue(Grid.RowProperty, 0);
					}
				}
				else if (this.TextAlignmentY == AlignmentY.Bottom)
				{
					this.TextBlock.SetValue(Grid.RowProperty, 2);
					if (this.TextBackgroundCanvas != null)
					{
						this.TextBackgroundCanvas.SetValue(Grid.RowProperty, 2);
					}
				}
				else
				{
					this.TextBlock.SetValue(Grid.RowProperty, 1);
					this.TextBlock.VerticalAlignment = VerticalAlignment.Center;
					if (this.TextBackgroundCanvas != null)
					{
						this.TextBackgroundCanvas.SetValue(Grid.RowProperty, 1);
						this.TextBackgroundCanvas.VerticalAlignment = VerticalAlignment.Center;
					}
				}
			}
			if (this.TextAlignmentX == AlignmentX.Center)
			{
				this._markerShapePosition.X = this._markerShapePosition.X + Math.Abs((this.TextBlockSize.Width - this.MarkerShape.Width) / 2.0);
			}
			if (this.TextAlignmentY == AlignmentY.Center)
			{
				this._markerShapePosition.Y = this._markerShapePosition.Y + Math.Abs((this.TextBlockSize.Height - this.MarkerShape.Height) / 2.0);
			}
		}

		private Brush GetMarkerShadowColor()
		{
			return new LinearGradientBrush
			{
				StartPoint = new Point(0.497000008821487, 1.00100004673004),
				EndPoint = new Point(0.503000020980835, -0.00100000004749745),
				GradientStops = 
				{
					new GradientStop
					{
						Color = Color.FromArgb(180, 108, 108, 108),
						Offset = 1.0
					},
					new GradientStop
					{
						Color = Color.FromArgb(0, 255, 255, 255),
						Offset = 1.0
					}
				}
			};
		}
	}
}
