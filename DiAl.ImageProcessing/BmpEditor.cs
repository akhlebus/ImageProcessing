using System;
using System.Linq;
using System.Windows.Media;

namespace DiAl.ImageProcessing
{
	public static class BmpEditor
	{
		public static PixelFormat Format { get; set; }
		static BmpEditor()
		{
			Format = PixelFormats.Bgr32;
		}
		public static byte GetBrightness(Int32 pixel)
		{
			if (Format != PixelFormats.Bgr32)
				throw new ApplicationException();
			byte[] pixelAsByteArray = BitConverter.GetBytes(pixel);
			byte brightness = GetValidByte(0.2126 * pixelAsByteArray[2] + 0.7152 * pixelAsByteArray[1] + 0.0722 * pixelAsByteArray[0]);
			return brightness;
		}

		public static Int32 SetBrightness(Int32 pixel, byte newBrightness)
		{
			if (Format != PixelFormats.Bgr32)
				throw new ApplicationException();
			var oldBrightness = GetBrightness(pixel);
			var percents = ((double)newBrightness - oldBrightness) / 256;
			byte[] pixelAsByteArray = BitConverter.GetBytes(pixel);
			pixelAsByteArray[2] = GetValidByte((pixelAsByteArray[2]) * (1 + percents));
			pixelAsByteArray[1] = GetValidByte((pixelAsByteArray[1]) * (1 + percents));
			pixelAsByteArray[0] = GetValidByte((pixelAsByteArray[0]) * (1 + percents));
			return BitConverter.ToInt32(pixelAsByteArray, 0);
		}

		/// <summary>
		/// g = c * log (1 + f)
		/// </summary>
		/// <param name="pixels"></param>
		/// <param name="c">constant</param>
		/// <returns></returns>
		public static Int32[] LogarithmicCorrection(Int32[] pixels, byte c)
		{
			if (Format != PixelFormats.Bgr32)
				throw new ApplicationException();
			return pixels.Select(pixel => SetBrightness(pixel, GetValidByte(c * Math.Log(1 + GetBrightness(pixel))))).ToArray();
		}

		public static Int32[] MinFilter(Int32[] pixels, ImageSize size)
		{
			if (Format != PixelFormats.Bgr32)
				throw new ApplicationException();
			return BaseFilter(pixels, size, Math.Min);
		}
		public static Int32[] MaxFilter(Int32[] pixels, ImageSize size)
		{
			if (Format != PixelFormats.Bgr32)
				throw new ApplicationException();
			return BaseFilter(pixels, size, Math.Max);
		}
		static bool isMin = true;
		public static Int32[] MaxMinFilter(Int32[] pixels, ImageSize size)
		{
			if (Format != PixelFormats.Bgr32)
				throw new ApplicationException();
			isMin = !isMin;
			return (isMin ? MinFilter(pixels, size) : MaxFilter(pixels, size));
		}
		public static Int32[] BaseFilter(Int32[] pixels, ImageSize size, Func<Int32, Int32, Int32> func)
		{
			Func<Int32, Int32, bool> isValid = (y, x) => ((x > 0 && y > 0) && (x < size.X && y < size.Y));
			if (Format != PixelFormats.Bgr32)
				throw new ApplicationException();
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
			if (Format != PixelFormats.Bgr32)
				throw new ApplicationException();
			return pixels.Select(pixel => SetBrightness(pixel, GetValidByte(c + GetBrightness(pixel)))).ToArray();
		}



		public static byte GetValidByte(Int32 data)
		{
			if (data > 255)
				data = 255;
			if (data < 0)
				data = 0;
			return (byte)data;
		}
		public static byte GetValidByte(Double data)
		{
			if (data > 255)
				data = 255;
			if (data < 0)
				data = 0;
			return (byte)data;
		}
	}
}
