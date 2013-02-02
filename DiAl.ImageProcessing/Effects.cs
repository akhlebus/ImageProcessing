using System;

namespace DiAl.ImageProcessing
{
	public static class Effects
	{
		public static Int32[] ToGrayView(Int32[] pixels)
		{
			var outPixels = new Int32[pixels.LongLength];
			for (Int32 i = 0; i < pixels.Length; ++i)
			{
				byte brightness = BmpEditor.GetBrightness(pixels[i]);
				outPixels[i] = BitConverter.ToInt32(new[] { brightness, brightness, brightness, brightness }, 0);
			}
			return outPixels;
		}

		public static Int32[] ToBinaryView(Int32[] pixels, byte brightnessThreshold)
		{
			var outPixels = new Int32[pixels.LongLength];
			for (Int32 i = 0; i < pixels.Length; ++i)
			{
				byte brightness = BmpEditor.GetBrightness(pixels[i]);
				brightness = (byte)((brightness < brightnessThreshold) ? 0 : 255);
				outPixels[i] = BitConverter.ToInt32(new[] { brightness, brightness, brightness, brightness }, 0);
			}
			return outPixels;
		}

		public static Image SegmentedImage(Image image)
		{
			Int32 level = 1;
			Int32[,] labels = new Int32[image.Size.X, image.Size.Y];
			for(Int32 i = 0; i < labels.GetLength(0); ++i)
			{
				for (Int32 j = 0; j < labels.GetLength(1); ++j)
				{
					labels[i, j] = 0;
				}
			}
			for (Int32 y = 0; y < image.Size.Y; ++y)
			{
				for (Int32 x = 0; x < image.Size.X; ++x)
				{
					Fill(image, ref labels, new Point(x, y), level++);
				}
			}
			return image;
		}

		public static void Fill(Image image, ref Int32[,] labels, Point point, Int32 level)
		{
			if((labels[point.X, point.Y] == 0) && (image[point.X, point.Y].GetBrightness() > 240))
			{
				image[point.X, point.Y].ToBinary(200, level);
				labels[point.X, point.Y] = level;
				if(point.X > 0)
					Fill(image, ref labels, new Point(point.X - 1, point.Y), level);
				if(point.X < image.Size.X - 1)
					Fill(image, ref labels, new Point(point.X + 1, point.Y), level);
				if(point.Y > 0)
					Fill(image, ref labels, new Point(point.X, point.Y - 1), level);
				if(point.Y < image.Size.Y - 1)
					Fill(image, ref labels, new Point(point.X, point.Y + 1), level);
			}
		}
	}
}
