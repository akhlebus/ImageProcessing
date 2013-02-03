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
			byte[] pixelAsByteArray = BitConverter.GetBytes(_pixel);
			byte brightness = GetValidByte(0.2126 * pixelAsByteArray[2] + 0.7152 * pixelAsByteArray[1] + 0.0722 * pixelAsByteArray[0]);
			return brightness;
		}

		public void SetBrightness(byte newBrightness)
		{
			var oldBrightness = GetBrightness();
			var percents = ((double)newBrightness - oldBrightness) / 256;
			byte[] pixelAsByteArray = BitConverter.GetBytes(_pixel);
			var red = GetValidByte(pixelAsByteArray[2]);
			red = (byte)((red == 0) ? 1 : red);
			var green = GetValidByte(pixelAsByteArray[1]);
			green = (byte)((green == 0) ? 1 : green);
			var blue = GetValidByte(pixelAsByteArray[0]);
			blue = (byte)((blue == 0) ? 1 : blue);
			pixelAsByteArray[2] = (byte)(red * (1 + percents));
			pixelAsByteArray[1] = (byte)(green * (1 + percents));
			pixelAsByteArray[0] = (byte)(blue * (1 + percents));
			_pixel = BitConverter.ToInt32(pixelAsByteArray, 0);
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

		public void ToBinary(Int32 brightnessThreshold, Int32 level)
		{
			_pixel = BitConverter.ToInt32(new byte[] { (byte)(50 + (level % 100)), 0, (byte)(level % 100), 0 }, 0);
		}
	}
}
