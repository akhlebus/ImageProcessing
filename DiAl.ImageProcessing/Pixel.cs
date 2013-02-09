using System;

namespace DiAl.ImageProcessing
{
	public class Pixel
	{
		private Int32 _pixel;
		public Pixel(Int32 pixel)
		{
			_pixel = pixel;
		}

		public Int32 ToInt32()
		{
			return _pixel;
		}

		public static explicit operator Int32(Pixel pixel)
		{
			return pixel.ToInt32();
		}

		public static implicit operator Pixel(Int32 pixel)
		{
			return new Pixel(pixel);
		}

		public byte GetBrightness()
		{
			return GetBrightness(_pixel);
		}

		public void SetBrightness(byte newBrightness)
		{
			_pixel = SetBrightness(_pixel, newBrightness);
		}

		public static byte GetBrightness(Int32 pixel)
		{
			byte[] pixelAsByteArray = BitConverter.GetBytes(pixel);
			byte brightness = GetValidByte(0.2126 * pixelAsByteArray[2] + 0.7152 * pixelAsByteArray[1] + 0.0722 * pixelAsByteArray[0]);
			return brightness;
		}

		public static Int32 SetBrightness(Int32 pixel, byte newBrightness)
		{
			var oldBrightness = GetBrightness(pixel);
			var percents = ((double)newBrightness - oldBrightness) / 256;
			byte[] pixelAsByteArray = BitConverter.GetBytes(pixel);
			pixelAsByteArray[2] = GetValidByte((pixelAsByteArray[2]) * (1 + percents));
			pixelAsByteArray[1] = GetValidByte((pixelAsByteArray[1]) * (1 + percents));
			pixelAsByteArray[0] = GetValidByte((pixelAsByteArray[0]) * (1 + percents));
			return BitConverter.ToInt32(pixelAsByteArray, 0);
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
			return GetValidByte((Int32) data);
		}

		public void ToBinary(Int32 brightnessThreshold, Int32 level)
		{
			_pixel = level;
		}
	}
}
