using System;
using System.Linq;

namespace DiAl.ImageProcessing
{
	public class Image : ICloneable
	{
		private Pixel[] _pixels;
		public ImageSize Size { get; private set; }
		public Image(Int32[] pixels, ImageSize size)
		{
			Size = size;
			SetPixels(pixels);
		}

		public Pixel this[Int32 x, Int32 y]
		{
			get
			{
				if (!IsValid(x, y))
					throw new ApplicationException();
				return _pixels[x + y * Size.X];
			}
			set
			{
				if (!IsValid(x, y))
					throw new ApplicationException();
				_pixels[x + y * Size.X] = value;
			}
		}

		public bool IsValid(Int32 x, Int32 y)
		{
			return (x >= 0 && x < Size.X && y >= 0 && y < Size.Y);
		}

		public Int32[] GetPixels()
		{
			return _pixels.Select(pixel => pixel.ToInt32()).ToArray();
		}

		public void SetPixels(Int32[] pixels)
		{
			_pixels = new Pixel[pixels.LongLength];
			for (Int64 i = 0; i < pixels.LongLength; ++i)
			{
				_pixels[i] = new Pixel(pixels[i]);
			}
		}

		public object Clone()
		{
			return new Image(GetPixels(), Size);
		}
	}
}
