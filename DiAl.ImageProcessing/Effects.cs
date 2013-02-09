using System;
using System.Collections.Generic;
using System.Linq;
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

		public static void SegmentedView(Image image, bool isRecursive = false)
		{
			if (isRecursive)
				RecursiveSegmentedView(image);
			else
				IterativeSegmentedView(image);
		}
		private static void IterativeSegmentedView(Image image)
		{
			Random random = new Random();
			HashSet<Point> labels = new HashSet<Point>();
			for (Int32 y = 0; y < image.Size.Y; ++y)
			{
				for (Int32 x = 0; x < image.Size.X; ++x)
				{
					var point = new Point(x, y);
					QuickFill(image, ref labels, point, random.Next());
				}
			}
		}
		private static void QuickFill(Image image, ref HashSet<Point> labels, Point point, Int32 level)
		{
			if (image[point.X, point.Y].GetBrightness() > 200)
			{
				labels.Add(point);
				if (!labels.Contains(new Point(point.X - 1, point.Y)) && !labels.Contains(new Point(point.X, point.Y - 1)))
				{
					image[point.X, point.Y].ToBinary(200, level);
				}
				else if (labels.Contains(new Point(point.X - 1, point.Y)))
				{
					image[point.X, point.Y].ToBinary(200, image[point.X - 1, point.Y].ToInt32());
				}
				else if (labels.Contains(new Point(point.X, point.Y - 1)))
				{
					image[point.X, point.Y].ToBinary(200, image[point.X, point.Y - 1].ToInt32());
				}
				else
				{
					throw new ApplicationException();
				}
			}
		}

		private static Int32 _countFunctionCopy;
		public const Int32 MaxCountFunctionCopy = 5000;
		private static void RecursiveSegmentedView(Image image)
		{
			HashSet<Point> labels = new HashSet<Point>();
			for (Int32 y = 0; y < image.Size.Y; ++y)
			{
				Random random = new Random();
				for (Int32 x = 0; x < image.Size.X; ++x)
				{
					var point = new Point(x, y);
					if (!labels.Contains(point))
						RecursionResolver(image, ref labels, point, random.Next());
				}
			}
		}
		private static void RecursionResolver(Image image, ref HashSet<Point> labels, Point point, Int32 level)
		{
			_countFunctionCopy = 0;
			var unprocessedPoints = Fill(image, ref labels, point, level);
			foreach (var currentPoint in unprocessedPoints)
			{
				var outList = new List<Point>(Fill(image, ref labels, currentPoint, level));
				if (outList.Count != 0)
				{
					foreach (var currentInnerPoint in outList)
					{
						RecursionResolver(image, ref labels, currentInnerPoint, level);
					}
				}
			}
		}
		private static IEnumerable<Point> Fill(Image image, ref HashSet<Point> labels, Point point, Int32 level)
		{
			++_countFunctionCopy;
			var outList = new List<Point>();
			if ((image[point.X, point.Y].GetBrightness() < 200))
			{
				return outList;
			}
			if (_countFunctionCopy >= MaxCountFunctionCopy)
			{
				outList.Add(point);
				return outList;
			}
			if((!labels.Contains(point)))
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

		public static void StricturedView(Image image)
		{
			for (Int32 y = 0; y < image.Size.Y; ++y)
			{
				for (Int32 x = 0; x < image.Size.X; ++x)
				{
					var neighbors = image.GetNeighbores(new Point(x, y), 8).ToList();
					if (neighbors.Count(point => image[point.X, point.Y].GetBrightness() < 200) < 4)
					{
						const byte brightness = (byte) (255);
						image[x, y] = BitConverter.ToInt32(new[] { brightness, brightness, brightness, brightness }, 0);
					}
				}
			}
		}
	}
}
