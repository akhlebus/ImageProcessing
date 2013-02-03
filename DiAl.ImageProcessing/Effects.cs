using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DiAl.ImageProcessing
{
	public static class Effects
	{
		public static PixelFormat Format { get { return PixelFormats.Bgr32; } }

		public static void GrayView(Image image)
		{
			var pixels = image.GetPixels();
			var outPixels = new Int32[pixels.LongLength];
			for (Int32 i = 0; i < pixels.Length; ++i)
			{
				byte brightness = Pixel.GetBrightness(pixels[i]);
				outPixels[i] = BitConverter.ToInt32(new[] { brightness, brightness, brightness, brightness }, 0);
			}
			image.SetPixels(outPixels);
		}

		public static void BinaryView(Image image, byte brightnessThreshold)
		{
			var pixels = image.GetPixels();
			var outPixels = new Int32[pixels.LongLength];
			for (Int32 i = 0; i < pixels.Length; ++i)
			{
				byte brightness = Pixel.GetBrightness(pixels[i]);
				brightness = (byte)((brightness < brightnessThreshold) ? 0 : 255);
				outPixels[i] = BitConverter.ToInt32(new[] { brightness, brightness, brightness, brightness }, 0);
			}
			image.SetPixels(outPixels);
		}

		private static Int32 countFunctionCopy;
		public const Int32 MaxCountFunctionCopy = 5000;
		public static Image SegmentedImage(Image image)
		{
			Int32 level = 1;
			HashSet<Point> labels = new HashSet<Point>();
			for (Int32 y = 0; y < image.Size.Y; ++y)
			{
				for (Int32 x = 0; x < image.Size.X; ++x)
				{
					RecursionResolver(image, ref labels, new Point(x, y), level);
					level += 10;
				}
			}
			return image;
		}

		public static void RecursionResolver(Image image, ref HashSet<Point> labels, Point point, Int32 level)
		{
			countFunctionCopy = 0;
			var outList = new List<Point>();
			var unprocessedPoints = Fill(image, ref labels, point, level);
			foreach (var currentPoint in unprocessedPoints)
			{
				outList.AddRange(Fill(image, ref labels, currentPoint, level));
				if (outList.Count != 0)
				{
					foreach (var currentInnerPoint in outList)
					{
						RecursionResolver(image, ref labels, currentInnerPoint, level);
					}
				}
			}
		}

		public static IEnumerable<Point> Fill(Image image, ref HashSet<Point> labels, Point point, Int32 level)
		{
			++countFunctionCopy;
			var outList = new List<Point>();
			if (countFunctionCopy >= MaxCountFunctionCopy)
			{
				outList.Add(point);
				return outList;
			}
			if((!labels.Contains(point)) && (image[point.X, point.Y].GetBrightness() > 200))
			{
				image[point.X, point.Y].ToBinary(200, level);
				labels.Add(point);
				if(point.X > 0)
					outList.AddRange(Fill(image, ref labels, new Point(point.X - 1, point.Y), level));
				if(point.X < image.Size.X - 1)
					outList.AddRange(Fill(image, ref labels, new Point(point.X + 1, point.Y), level));
				if(point.Y > 0)
					outList.AddRange(Fill(image, ref labels, new Point(point.X, point.Y - 1), level));
				if(point.Y < image.Size.Y - 1)
					outList.AddRange(Fill(image, ref labels, new Point(point.X, point.Y + 1), level));
			}
			return outList;
		}
	}
}
