using System;
using System.Linq;
using System.Windows.Media;

namespace DiAl.ImageProcessing
{
	/// <summary>
	/// Legacy class. Please, use class Effects instead it.
	/// </summary>
	public static class BmpEditor
	{
		public static PixelFormat Format { get { return PixelFormats.Bgr32; } }

		/// <summary>
		/// g = c * log (1 + f)
		/// </summary>
		/// <param name="pixels"></param>
		/// <param name="c">constant</param>
		/// <returns></returns>
		public static Int32[] LogarithmicCorrection(Int32[] pixels, byte c)
		{
			return pixels.Select(pixel => Pixel.SetBrightness(pixel, Pixel.GetValidByte(c * Math.Log(1 + Pixel.GetBrightness(pixel))))).ToArray();
		}

		public static Int32[] MinFilter(Int32[] pixels, ImageSize size)
		{
			return BaseFilter(pixels, size, Math.Min);
		}
		public static Int32[] MaxFilter(Int32[] pixels, ImageSize size)
		{
			return BaseFilter(pixels, size, Math.Max);
		}
		static bool isMin = true;
		public static Int32[] MaxMinFilter(Int32[] pixels, ImageSize size)
		{
			isMin = !isMin;
			return (isMin ? MinFilter(pixels, size) : MaxFilter(pixels, size));
		}
		public static Int32[] BaseFilter(Int32[] pixels, ImageSize size, Func<Int32, Int32, Int32> func)
		{
			Func<Int32, Int32, bool> isValid = (y, x) => ((x > 0 && y > 0) && (x < size.X && y < size.Y));
			if ((size.X * size.Y) != pixels.LongLength)
				throw new ApplicationException();
			Int32[] outPixels = new Int32[pixels.Length];
			for (Int32 i = 0; i < size.Y; ++i)
			{
				for (Int32 j = 0; j < size.X; ++j)
				{
					Int32 min = pixels[i * size.X + j];
					for (Int32 i1 = -1; i1 <= 1; ++i1)
					{
						for (Int32 j1 = -1; j1 <= 1; ++j1)
						{
							if (isValid(i + i1, j + j1))
							{
								min = func(pixels[(i + i1) * size.X + (j + j1)], min);
							}
						}
					}
					outPixels[i * size.X + j] = min;
				}
			}
			return outPixels;
		}

		/// <summary>
		/// g = c + f
		/// </summary>
		/// <param name="pixels"></param>
		/// <param name="c">constant</param>
		/// <returns></returns>
		public static Int32[] ChangeBrightness(Int32[] pixels, byte c)
		{
			return pixels.Select(pixel => Pixel.SetBrightness(pixel, Pixel.GetValidByte(c + Pixel.GetBrightness(pixel)))).ToArray();
		}
	}
}
