using System;
using System.Linq;

namespace DiAl.ImageProcessing
{
	public class Image : ICloneable
	{
		private readonly Pixel[] _pixels;
		public ImageSize Size { get; private set; }
		public Image(Int32[] pixels, ImageSize size)
		{
			_pixels = new Pixel[pixels.LongLength];
			for (Int64 i = 0; i < pixels.LongLength; ++i)
			{
				_pixels[i] = new Pixel(pixels[i]);
			}
				Size = size;
		}

		public Pixel this[Int32 x, Int32 y]
		{
			get
			{
				if (!IsValid(x, y))
					throw new ApplicationException();
				return _pixels[x * Size.Y + y];
			}
			set
			{
				if (!IsValid(x, y))
					throw new ApplicationException();
				_pixels[x * Size.Y + y] = value;
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

		public object Clone()
		{
			return new Image(GetPixels(), Size);
		}
	}
}
