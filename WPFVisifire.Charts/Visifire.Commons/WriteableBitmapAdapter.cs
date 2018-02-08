using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Visifire.Commons
{
	public class WriteableBitmapAdapter : IDisposable
	{
		private int _width;

		private int[] _pixels;

		private int _height;

		private WriteableBitmap _wb;

		public int PixelHeight
		{
			get
			{
				if (this._wb == null)
				{
					return this._height;
				}
				return this._wb.PixelHeight;
			}
		}

		public int[] Pixels
		{
			get
			{
				if (this._wb == null)
				{
					return this._pixels;
				}
				throw new NotImplementedException();
			}
		}

		public int PixelWidth
		{
			get
			{
				if (this._wb == null)
				{
					return this._width;
				}
				return this._wb.PixelWidth;
			}
		}

		public void Dispose()
		{
			this._pixels = null;
			this._wb = null;
		}

		public WriteableBitmapAdapter(Image img)
		{
			this.LoadFromBitmapSource(img.Source as BitmapSource);
		}

		public WriteableBitmapAdapter(BitmapSource bmp)
		{
			this.LoadFromBitmapSource(bmp);
		}

		private void LoadFromBitmapSource(BitmapSource bmp)
		{
			this._pixels = new int[bmp.PixelWidth * bmp.PixelHeight];
			this._height = bmp.PixelHeight;
			this._width = bmp.PixelWidth;
			bmp.CopyPixels(this._pixels, 4 * bmp.PixelWidth, 0);
		}

		public WriteableBitmapAdapter(WriteableBitmap wb)
		{
			this._wb = wb;
		}

		public WriteableBitmapAdapter(int width, int height)
		{
			this._height = height;
			this._width = width;
			this._pixels = new int[height * width];
		}

		public Image GetNewImage()
		{
			if (this._wb == null)
			{
				Image image = new Image
				{
					Height = (double)this._height,
					Width = (double)this._width
				};
				image.Source = BitmapSource.Create(this._width, this._height, 96.0, 96.0, PixelFormats.Bgra32, null, this._pixels, 4 * this._width);
				return image;
			}
			Image image2 = new Image
			{
				Height = (double)this._wb.PixelHeight,
				Width = (double)this._wb.PixelWidth
			};
			image2.Source = this._wb;
			return image2;
		}

		public BitmapSource GetImageSource()
		{
			if (this._wb == null)
			{
				return BitmapSource.Create(this._width, this._height, 96.0, 96.0, PixelFormats.Bgra32, null, this._pixels, 4 * this._width);
			}
			return this._wb;
		}
	}
}
