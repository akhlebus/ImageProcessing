using System;

namespace DiAl.ImageProcessing
{
	public class Point
	{
		public Int32 X { get; set; }
		public Int32 Y { get; set; }

		public Point() {}
		public Point(Int32 x, Int32 y)
		{
			X = x;
			Y = y;
		}
	}
}
