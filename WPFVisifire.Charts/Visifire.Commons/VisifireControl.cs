using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Visifire.Charts;
using Visifire.Commons.Controls;

namespace Visifire.Commons
{
	public abstract class VisifireControl : VisifireElement
	{
		private const double TOOLTIP_HEIGHT_OFFSET = 5.0;

		private const double TOOLTIP_XPOSITION_OFFSET = 10.0;

		private const double TOOLTIP_YPOSITION_OFFSET = 20.0;

		internal const string ZoomRectangleName = "ZoomRectangle";

		internal const string ZoomAreaCanvasName = "ZoomAreaCanvas";

		internal const string ElementCanvasName = "ElementCanvas";

		internal const string LicenseInfoElementName = "LicenseInfoElement";

		internal const string RootElementName = "RootElement";

		internal const string ShadowGridName = "ShadowGrid";

		internal const string ChartBorderName = "ChartBorder";

		internal const string ChartLightingBorderName = "ChartLightingBorder";

		internal const string BevelCanvasName = "BevelCanvas";

		internal const string ChartAreaGridName = "ChartAreaGrid";

		internal const string TopOuterPanelName = "TopOuterPanel";

		internal const string TopOuterTitlePanelName = "TopOuterTitlePanel";

		internal const string TopOuterLegendPanelName = "TopOuterLegendPanel";

		internal const string BottomOuterPanelName = "BottomOuterPanel";

		internal const string BottomOuterLegendPanelName = "BottomOuterLegendPanel";

		internal const string BottomOuterTitlePanelName = "BottomOuterTitlePanel";

		internal const string LeftOuterPanelName = "LeftOuterPanel";

		internal const string LeftOuterTitlePanelName = "LeftOuterTitlePanel";

		internal const string LeftOuterLegendPanelName = "LeftOuterLegendPanel";

		internal const string RightOuterPanelName = "RightOuterPanel";

		internal const string RightOuterLegendPanelName = "RightOuterLegendPanel";

		internal const string RightOuterTitlePanelName = "RightOuterTitlePanel";

		internal const string CenterOuterGridName = "CenterOuterGrid";

		internal const string CenterGridName = "CenterGrid";

		internal const string TopOffsetGridName = "TopOffsetGrid";

		internal const string BottomOffsetGridName = "BottomOffsetGrid";

		internal const string LeftOffsetGridName = "LeftOffsetGrid";

		internal const string RightOffsetGridName = "RightOffsetGrid";

		internal const string TopAxisGridName = "TopAxisGrid";

		internal const string TopAxisContainerName = "TopAxisContainer";

		internal const string TopAxisPanelName = "TopAxisPanel";

		internal const string TopAxisScrollBarName = "TopAxisScrollBar";

		internal const string LeftAxisGridName = "LeftAxisGrid";

		internal const string LeftAxisContainerName = "LeftAxisContainer";

		internal const string LeftAxisPanelName = "LeftAxisPanel";

		internal const string LeftAxisScrollBarName = "LeftAxisScrollBar";

		internal const string RightAxisGridName = "RightAxisGrid";

		internal const string RightAxisContainerName = "RightAxisContainer";

		internal const string RightAxisScrollBarName = "RightAxisScrollBar";

		internal const string RightAxisPanelName = "RightAxisPanel";

		internal const string BottomAxisGridName = "BottomAxisGrid";

		internal const string BottomAxisContainerName = "BottomAxisContainer";

		internal const string BottomAxisScrollBarName = "BottomAxisScrollBar";

		internal const string BottomAxisPanelName = "BottomAxisPanel";

		internal const string CenterInnerGridName = "CenterInnerGrid";

		internal const string InnerGridName = "InnerGrid";

		internal const string TopInnerPanelName = "TopInnerPanel";

		internal const string TopInnerTitlePanelName = "TopInnerTitlePanel";

		internal const string TopInnerLegendPanelName = "TopInnerLegendPanel";

		internal const string BottomInnerPanelName = "BottomInnerPanel";

		internal const string BottomInnerLegendPanelName = "BottomInnerLegendPanel";

		internal const string BottomInnerTitlePanelName = "BottomInnerTitlePanel";

		internal const string LeftInnerPanelName = "LeftInnerPanel";

		internal const string LeftInnerTitlePanelName = "LeftInnerTitlePanel";

		internal const string LeftInnerLegendPanelName = "LeftInnerLegendPanel";

		internal const string RightInnerPanelName = "RightInnerPanel";

		internal const string RightInnerLegendPanelName = "RightInnerLegendPanel";

		internal const string RightInnerTitlePanelName = "RightInnerTitlePanel";

		internal const string PlotAreaGridName = "PlotAreaGrid";

		internal const string PlotAreaScrollViewerName = "PlotAreaScrollViewer";

		internal const string PlotCanvasName = "PlotCanvas";

		internal const string PlotAreaShadowCanvasName = "PlotAreaShadowCanvas";

		internal const string DrawingCanvasName = "DrawingCanvas";

		internal const string CenterDockInsidePlotAreaPanelName = "CenterDockInsidePlotAreaPanel";

		internal const string CenterDockOutsidePlotAreaPanelName = "CenterDockOutsidePlotAreaPanel";

		internal const string ToolTipCanvasName = "ToolTipCanvas";

		public static readonly DependencyProperty ToolTipEnabledProperty = DependencyProperty.Register("ToolTipEnabled", typeof(bool), typeof(VisifireControl), new PropertyMetadata(true, null));

		private static readonly DependencyProperty SeparaterTextProperty = DependencyProperty.Register("SeparaterText", typeof(string), typeof(VisifireControl), new PropertyMetadata("|", null));

		public static readonly DependencyProperty ZoomOutTextProperty = DependencyProperty.Register("ZoomOutText", typeof(string), typeof(VisifireControl), new PropertyMetadata("Zoom Out", new PropertyChangedCallback(VisifireControl.OnZoomOutTextPropertyChanged)));

		public static readonly DependencyProperty ShowAllTextProperty = DependencyProperty.Register("ShowAllText", typeof(string), typeof(VisifireControl), new PropertyMetadata("Show All", new PropertyChangedCallback(VisifireControl.OnShowAllTextPropertyChanged)));

		public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(bool), typeof(VisifireControl), new PropertyMetadata(true, new PropertyChangedCallback(VisifireControl.OnWatermarkPropertyChanged)));

		public static readonly DependencyProperty ToolBarEnabledProperty = DependencyProperty.Register("ToolBarEnabled", typeof(bool), typeof(VisifireControl), new PropertyMetadata(false, new PropertyChangedCallback(VisifireControl.OnSaveIconEnabledPropertyChanged)));

		internal bool _isTemplateApplied;

		internal TextBlock _wMElement;

		internal Border _zoomRectangle;

		internal Canvas _zoomAreaCanvas;

		internal Canvas _elementCanvas;

		internal Border _licenseInfoElement;

		internal Grid _rootElement;

		internal Grid _shadowGrid;

		internal Border _chartBorder;

		internal Rectangle _chartLightingBorder;

		internal Canvas _bevelCanvas;

		internal Grid _chartAreaGrid;

		internal StackPanel _topOuterPanel;

		internal StackPanel _topOuterTitlePanel;

		internal StackPanel _topOuterLegendPanel;

		internal StackPanel _bottomOuterPanel;

		internal StackPanel _bottomOuterLegendPanel;

		internal StackPanel _bottomOuterTitlePanel;

		internal StackPanel _leftOuterPanel;

		internal StackPanel _leftOuterTitlePanel;

		internal StackPanel _leftOuterLegendPanel;

		internal StackPanel _rightOuterPanel;

		internal StackPanel _rightOuterLegendPanel;

		internal StackPanel _rightOuterTitlePanel;

		internal Grid _centerOuterGrid;

		internal Grid _centerGrid;

		internal Grid _topOffsetGrid;

		internal Grid _bottomOffsetGrid;

		internal Grid _leftOffsetGrid;

		internal Grid _rightOffsetGrid;

		internal Grid _topAxisGrid;

		internal StackPanel _topAxisContainer;

		internal StackPanel _topAxisPanel;

		internal ZoomBar _topAxisScrollBar;

		internal Grid _leftAxisGrid;

		internal StackPanel _leftAxisContainer;

		internal StackPanel _leftAxisPanel;

		internal ZoomBar _leftAxisScrollBar;

		internal Grid _rightAxisGrid;

		internal StackPanel _rightAxisContainer;

		internal ZoomBar _rightAxisScrollBar;

		internal StackPanel _rightAxisPanel;

		internal Grid _bottomAxisGrid;

		internal StackPanel _bottomAxisContainer;

		internal ZoomBar _bottomAxisScrollBar;

		internal StackPanel _bottomAxisPanel;

		internal Grid _centerInnerGrid;

		internal Grid _innerGrid;

		internal StackPanel _topInnerPanel;

		internal StackPanel _topInnerTitlePanel;

		internal StackPanel _topInnerLegendPanel;

		internal StackPanel _bottomInnerPanel;

		internal StackPanel _bottomInnerLegendPanel;

		internal StackPanel _bottomInnerTitlePanel;

		internal StackPanel _leftInnerPanel;

		internal StackPanel _leftInnerTitlePanel;

		internal StackPanel _leftInnerLegendPanel;

		internal StackPanel _rightInnerPanel;

		internal StackPanel _rightInnerLegendPanel;

		internal StackPanel _rightInnerTitlePanel;

		internal Grid _plotAreaGrid;

		internal Panel _plotAreaScrollViewer;

		internal Panel _plotCanvas;

		internal Canvas _plotAreaShadowCanvas;

		internal Canvas _drawingCanvas;

		internal StackPanel _centerDockInsidePlotAreaPanel;

		internal StackPanel _centerDockOutsidePlotAreaPanel;

		internal Canvas _toolTipCanvas;

		internal Visifire.Charts.ToolTip _toolTip;

		private StackPanel _toolbarContainer;

		private Image _saveIconImage;

		private Image _printIconImage;

		internal TextBlock _zoomOutTextBlock;

		internal TextBlock _showAllTextBlock;

		internal TextBlock _zoomIconSeparater;

		internal StackPanel _zoomIconContainer;

		private static byte[] wmRegVal = new byte[]
		{
			86,
			105,
			115,
			105,
			102,
			105,
			114,
			101,
			32,
			84,
			114,
			105,
			97,
			108,
			32,
			69,
			100,
			105,
			116,
			105,
			111,
			110
		};

		private static byte[] wmLinkVal = new byte[]
		{
			104,
			116,
			116,
			112,
			58,
			47,
			47,
			119,
			119,
			119,
			46,
			86,
			105,
			115,
			105,
			102,
			105,
			114,
			101,
			46,
			99,
			111,
			109,
			47,
			108,
			105,
			99,
			101,
			110,
			115,
			101,
			46,
			112,
			104,
			112
		};

		public bool ToolTipEnabled
		{
			get
			{
				return (bool)base.GetValue(VisifireControl.ToolTipEnabledProperty);
			}
			set
			{
				base.SetValue(VisifireControl.ToolTipEnabledProperty, value);
			}
		}

		private string SeparaterText
		{
			get
			{
				return (string)base.GetValue(VisifireControl.SeparaterTextProperty);
			}
			set
			{
				base.SetValue(VisifireControl.SeparaterTextProperty, value);
			}
		}

		public string ZoomOutText
		{
			get
			{
				return (string)base.GetValue(VisifireControl.ZoomOutTextProperty);
			}
			set
			{
				if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(this.ShowAllText))
				{
					this.SeparaterText = "";
				}
				else
				{
					this.SeparaterText = "|";
				}
				base.SetValue(VisifireControl.ZoomOutTextProperty, value);
			}
		}

		public string ShowAllText
		{
			get
			{
				return (string)base.GetValue(VisifireControl.ShowAllTextProperty);
			}
			set
			{
				if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(this.ZoomOutText))
				{
					this.SeparaterText = "";
				}
				else
				{
					this.SeparaterText = "|";
				}
				base.SetValue(VisifireControl.ShowAllTextProperty, value);
			}
		}

		internal bool IsInDesignMode
		{
			get
			{
				return DesignerProperties.GetIsInDesignMode(this);
			}
		}

		[Obsolete]
		public bool Watermark
		{
			get
			{
				return (bool)base.GetValue(VisifireControl.WatermarkProperty);
			}
			set
			{
				base.SetValue(VisifireControl.WatermarkProperty, value);
			}
		}

		public bool ToolBarEnabled
		{
			get
			{
				return (bool)base.GetValue(VisifireControl.ToolBarEnabledProperty);
			}
			set
			{
				base.SetValue(VisifireControl.ToolBarEnabledProperty, value);
			}
		}

		internal bool IsRunningInDevice
		{
			get;
			set;
		}

		internal static bool IsXBAPApp
		{
			get
			{
				return BrowserInteropHelper.IsBrowserHosted;
			}
		}

		internal static bool IsMediaEffectsEnabled
		{
			get
			{
				return !VisifireControl.IsXBAPApp;
			}
		}

		public VisifireControl()
		{
		}

		public static string GetAbsolutePath(string path)
		{
			return path;
		}

		protected virtual void OnWatermarkPropertyValueChanged(bool value)
		{
		}

		protected static void OnWatermarkPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VisifireControl visifireControl = d as VisifireControl;
			visifireControl.OnWatermarkPropertyValueChanged((bool)e.NewValue);
		}

		protected void LoadToolBar()
		{
			this._toolbarContainer = new StackPanel();
			this._toolbarContainer.Orientation = Orientation.Horizontal;
			this._toolbarContainer.HorizontalAlignment = HorizontalAlignment.Right;
			this._toolbarContainer.VerticalAlignment = VerticalAlignment.Top;
			this._toolbarContainer.Margin = new Thickness(0.0, 3.0, 5.0, 0.0);
			this._toolbarContainer.SetValue(Panel.ZIndexProperty, 90000);
			this.LoadWm();
			this.LoadSaveIcon();
			this.LoadPrintIcon();
			if (this._rootElement != null)
			{
				this._rootElement.Children.Add(this._toolbarContainer);
			}
		}

		protected void LoadZoomIcons()
		{
			this._zoomIconContainer = new StackPanel();
			this._zoomIconContainer.Orientation = Orientation.Horizontal;
			this._zoomIconContainer.HorizontalAlignment = HorizontalAlignment.Right;
			this._zoomIconContainer.VerticalAlignment = VerticalAlignment.Top;
			this._zoomIconContainer.Margin = new Thickness(0.0, 3.0, 5.0, 0.0);
			this._zoomIconContainer.SetValue(Panel.ZIndexProperty, 95000);
			this.LoadZoomOutIcon();
			this.LoadSeparater();
			this.LoadShowAllIcon();
			this._plotAreaGrid.Children.Add(this._zoomIconContainer);
		}

		private void LoadSeparater()
		{
			this._zoomIconSeparater = new TextBlock();
			Binding binding = new Binding("SeparaterText");
			binding.Source = this;
			this._zoomIconSeparater.SetBinding(TextBlock.TextProperty, binding);
			this._zoomIconSeparater.FontSize = 9.0;
			this._zoomIconSeparater.Foreground = new SolidColorBrush(Colors.Gray);
			this._zoomIconSeparater.HorizontalAlignment = HorizontalAlignment.Right;
			this._zoomIconSeparater.VerticalAlignment = VerticalAlignment.Center;
			this._zoomIconSeparater.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
			this._zoomIconSeparater.Visibility = Visibility.Collapsed;
			this._zoomIconContainer.Children.Add(this._zoomIconSeparater);
		}

		private void LoadShowAllIcon()
		{
			this._showAllTextBlock = new TextBlock();
			this._showAllTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
			this._showAllTextBlock.VerticalAlignment = VerticalAlignment.Center;
			this._showAllTextBlock.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
			this._showAllTextBlock.Text = this.ShowAllText;
			if (string.IsNullOrEmpty(this.ShowAllText))
			{
				this.SeparaterText = "";
			}
			else if (!string.IsNullOrEmpty(this.ZoomOutText))
			{
				this.SeparaterText = "|";
			}
			this._showAllTextBlock.FontSize = 9.0;
			this._showAllTextBlock.MouseMove += delegate(object sender, MouseEventArgs e)
			{
				this._showAllTextBlock.TextDecorations = TextDecorations.Underline;
				if (string.IsNullOrEmpty(this.ShowAllText))
				{
					this._showAllTextBlock.Cursor = base.Cursor;
					return;
				}
				this._showAllTextBlock.Cursor = Cursors.Hand;
			};
			this._showAllTextBlock.MouseLeave += delegate(object sender, MouseEventArgs e)
			{
				this._showAllTextBlock.TextDecorations = null;
			};
			this._showAllTextBlock.Visibility = Visibility.Collapsed;
			this._zoomIconContainer.Children.Add(this._showAllTextBlock);
		}

		private void LoadZoomOutIcon()
		{
			this._zoomOutTextBlock = new TextBlock();
			this._zoomOutTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
			this._zoomOutTextBlock.VerticalAlignment = VerticalAlignment.Center;
			this._zoomOutTextBlock.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
			this._zoomOutTextBlock.Text = this.ZoomOutText;
			if (string.IsNullOrEmpty(this.ZoomOutText))
			{
				this.SeparaterText = "";
			}
			else if (!string.IsNullOrEmpty(this.ShowAllText))
			{
				this.SeparaterText = "|";
			}
			this._zoomOutTextBlock.FontSize = 9.0;
			this._zoomOutTextBlock.MouseMove += delegate(object sender, MouseEventArgs e)
			{
				this._zoomOutTextBlock.TextDecorations = TextDecorations.Underline;
				if (string.IsNullOrEmpty(this.ZoomOutText))
				{
					this._zoomOutTextBlock.Cursor = base.Cursor;
					return;
				}
				this._zoomOutTextBlock.Cursor = Cursors.Hand;
			};
			this._zoomOutTextBlock.MouseLeave += delegate(object sender, MouseEventArgs e)
			{
				this._zoomOutTextBlock.TextDecorations = null;
			};
			this._zoomOutTextBlock.Visibility = Visibility.Collapsed;
			this._zoomIconContainer.Children.Add(this._zoomOutTextBlock);
		}

		internal void UpdateToolTipPosition(object sender, MouseEventArgs e)
		{
			if (this.ToolTipEnabled && this._toolTip.Enabled.Value)
			{
				double x = e.GetPosition(this).X;
				double num = x;
				double num2 = e.GetPosition(this).Y;
				this._toolTip.Measure(new Size(1.7976931348623157E+308, 1.7976931348623157E+308));
				this._toolTip.UpdateLayout();
				Size size = Graphics.CalculateVisualSize(this._toolTip._borderElement);
				num2 -= size.Height + 5.0;
				num -= size.Width / 2.0;
				if (num <= 0.0)
				{
					num = e.GetPosition(this).X + 10.0;
					num2 = e.GetPosition(this).Y + 20.0;
					if (num2 + size.Height >= base.ActualHeight)
					{
						num2 = base.ActualHeight - size.Height;
					}
				}
				if (num + size.Width >= base.ActualWidth)
				{
					num = e.GetPosition(this).X - size.Width;
				}
				if (num2 < 0.0)
				{
					num2 = e.GetPosition(this).Y + 20.0;
				}
				if (num + size.Width > base.ActualWidth)
				{
					num = num + size.Width - base.ActualWidth;
				}
				if (size.Width == this._toolTip.MaxWidth)
				{
					num = 0.0;
				}
				if (num < 0.0)
				{
					num = 0.0;
				}
				if (!double.IsNaN(base.ActualHeight) && num2 + size.Height > base.ActualHeight)
				{
					num2 = 0.0;
					num = x + 10.0;
					if (num <= 0.0)
					{
						num = e.GetPosition(this).X + 10.0;
					}
					if (num + size.Width >= base.ActualWidth)
					{
						num = e.GetPosition(this).X - size.Width;
					}
					if (num + size.Width > base.ActualWidth)
					{
						num = num + size.Width - base.ActualWidth;
					}
					if (size.Width == this._toolTip.MaxWidth)
					{
						num = 0.0;
					}
					if (num < 0.0)
					{
						num = 0.0;
					}
				}
				this._toolTip.SetValue(Canvas.LeftProperty, num);
				this._toolTip.SetValue(Canvas.TopProperty, num2);
				return;
			}
			this._toolTip.Hide();
		}

		private void LoadSaveIcon()
		{
			this._saveIconImage = new Image();
			this._saveIconImage.HorizontalAlignment = HorizontalAlignment.Right;
			this._saveIconImage.VerticalAlignment = VerticalAlignment.Center;
			this._saveIconImage.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
			this._saveIconImage.Width = (this._saveIconImage.Height = 14.0);
			this._saveIconImage.Cursor = Cursors.Hand;
			base.MouseMove += delegate(object sender, MouseEventArgs e)
			{
				this._saveIconImage.Visibility = (this.ToolBarEnabled ? Visibility.Visible : Visibility.Collapsed);
			};
			base.MouseLeave += delegate(object sender, MouseEventArgs e)
			{
				this._saveIconImage.Visibility = Visibility.Collapsed;
			};
			this._saveIconImage.MouseMove += delegate(object sender, MouseEventArgs e)
			{
				this._toolTip.Text = "Export as image";
				this._toolTip.Show();
				if (this._toolTip._callOutPath != null)
				{
					this._toolTip.CallOutVisiblity = Visibility.Collapsed;
				}
				this.UpdateToolTipPosition(sender, e);
			};
			this._saveIconImage.MouseLeave += delegate(object sender, MouseEventArgs e)
			{
				this._toolTip.Text = "";
				this._toolTip.Hide();
			};
			this._saveIconImage.MouseLeftButtonUp += delegate(object A_1, MouseButtonEventArgs A_2)
			{
				this.Save(null, ExportType.Jpg, true);
			};
			Graphics.SetImageSource(this._saveIconImage, "Visifire.Charts.save_icon.png");
			this._saveIconImage.Visibility = Visibility.Collapsed;
			this._toolbarContainer.Children.Add(this._saveIconImage);
		}

		private void LoadPrintIcon()
		{
			this._printIconImage = new Image();
			this._printIconImage.HorizontalAlignment = HorizontalAlignment.Right;
			this._printIconImage.VerticalAlignment = VerticalAlignment.Center;
			this._printIconImage.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
			this._printIconImage.Stretch = Stretch.Fill;
			this._printIconImage.Width = 14.0;
			this._printIconImage.Height = 12.0;
			this._printIconImage.Cursor = Cursors.Hand;
			base.MouseMove += delegate(object sender, MouseEventArgs e)
			{
				this._printIconImage.Visibility = (this.ToolBarEnabled ? Visibility.Visible : Visibility.Collapsed);
			};
			base.MouseLeave += delegate(object sender, MouseEventArgs e)
			{
				this._printIconImage.Visibility = Visibility.Collapsed;
			};
			this._printIconImage.MouseMove += delegate(object sender, MouseEventArgs e)
			{
				this._toolTip.Text = "Print Chart image";
				this._toolTip.Show();
				if (this._toolTip._callOutPath != null)
				{
					this._toolTip.CallOutVisiblity = Visibility.Collapsed;
				}
				this.UpdateToolTipPosition(sender, e);
			};
			this._printIconImage.MouseLeave += delegate(object sender, MouseEventArgs e)
			{
				this._toolTip.Text = "";
				this._toolTip.Hide();
			};
			this._printIconImage.MouseLeftButtonUp += delegate(object A_1, MouseButtonEventArgs A_2)
			{
				this.Print();
			};
			Graphics.SetImageSource(this._printIconImage, "Visifire.Charts.print_icon.png");
			this._printIconImage.Visibility = Visibility.Collapsed;
			this._toolbarContainer.Children.Add(this._printIconImage);
		}

		public virtual void Print()
		{
			PrintDialog printDialog = new PrintDialog();
			if (printDialog.ShowDialog() != true)
			{
				return;
			}
			printDialog.PrintVisual(this, "Visifire Chart");
		}

		protected virtual void Save(string path, ExportType exportType, bool showDilog)
		{
			if (this._saveIconImage != null)
			{
				this._saveIconImage.Visibility = Visibility.Collapsed;
				this._toolTip.Hide();
				this._toolbarContainer.UpdateLayout();
			}
			if (this._printIconImage != null)
			{
				this._printIconImage.Visibility = Visibility.Collapsed;
				this._toolbarContainer.UpdateLayout();
			}
			Transform layoutTransform = base.LayoutTransform;
			this._rootElement.LayoutTransform = null;
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)base.ActualWidth, (int)base.ActualHeight, 96.0, 96.0, PixelFormats.Pbgra32);
			renderTargetBitmap.Render(this._rootElement);
			if (showDilog)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "Jpg Files (*.jpg)|*.jpg|BMP Files (*.bmp)|*.bmp";
				saveFileDialog.DefaultExt = ".jpg";
				if (!saveFileDialog.ShowDialog().Value)
				{
					goto IL_1BE;
				}
				BitmapEncoder bitmapEncoder;
				if (saveFileDialog.FilterIndex == 2)
				{
					bitmapEncoder = new BmpBitmapEncoder();
				}
				else
				{
					bitmapEncoder = new JpegBitmapEncoder();
				}
				using (Stream stream = saveFileDialog.OpenFile())
				{
					bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
					bitmapEncoder.Save(stream);
					goto IL_1BE;
				}
			}
			path = System.IO.Path.GetFullPath(path.Trim());
			string text = System.IO.Path.GetFileName(path);
			if (string.IsNullOrEmpty(text))
			{
				text = "VisifireChart";
				path += text;
			}
			if (exportType == ExportType.Bmp)
			{
				path += ".bmp";
			}
			else
			{
				path += ".jpg";
			}
			FileStream fileStream2;
			FileStream fileStream = fileStream2 = new FileStream(path, FileMode.Create);
			try
			{
				BitmapEncoder bitmapEncoder2;
				if (exportType == ExportType.Bmp)
				{
					bitmapEncoder2 = new BmpBitmapEncoder();
				}
				else
				{
					bitmapEncoder2 = new JpegBitmapEncoder();
				}
				bitmapEncoder2.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
				bitmapEncoder2.Save(fileStream);
			}
			finally
			{
				if (fileStream2 != null)
				{
					((IDisposable)fileStream2).Dispose();
				}
			}
			if (fileStream == null)
			{
				throw new IOException("Unable to export the chart to an image as the specified path is invalid.");
			}
			IL_1BE:
			this._rootElement.LayoutTransform = layoutTransform;
		}

		protected virtual void LoadWm()
		{
		}

		protected void CreateWmElement(string text, string href)
		{
			if (this._wMElement == null)
			{
				this._wMElement = new TextBlock();
				this._wMElement.Tag = "Watermark";
				this._wMElement.HorizontalAlignment = HorizontalAlignment.Right;
				this._wMElement.VerticalAlignment = VerticalAlignment.Center;
				this._wMElement.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
				this._wMElement.SetValue(Panel.ZIndexProperty, 90000);
				this._wMElement.Text = text;
				if (!string.IsNullOrEmpty(href))
				{
					this._wMElement.TextDecorations = TextDecorations.Underline;
				}
				this._wMElement.Foreground = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
				this._wMElement.FontSize = 11.0;
				base.AttachHref(this, this._wMElement, href, HrefTargets._blank);
				this._toolbarContainer.Children.Add(this._wMElement);
			}
		}

		private static void OnShowAllTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VisifireControl visifireControl = d as VisifireControl;
			if (visifireControl._showAllTextBlock != null)
			{
				visifireControl._showAllTextBlock.Text = e.NewValue.ToString();
			}
		}

		private static void OnSaveIconEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VisifireControl visifireControl = d as VisifireControl;
			if (visifireControl._saveIconImage != null)
			{
				visifireControl._saveIconImage.Visibility = (((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed);
			}
		}

		private static void OnZoomOutTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VisifireControl visifireControl = d as VisifireControl;
			if (visifireControl._zoomOutTextBlock != null)
			{
				visifireControl._zoomOutTextBlock.Text = e.NewValue.ToString();
			}
		}

		internal virtual void OnToolTipFontSizePropertyValueChanged()
		{
		}
	}
}
