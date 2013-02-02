using System;

namespace DiAl.ImageProcessing
{
	public class ImageSize
	{
		public Int32 X { get; set; }
		public Int32 Y { get; set; }
		
		public ImageSize() {}
		public ImageSize(Int32 x, Int32 y)
		{
			X = x;
			Y = y;
		}
	}
}
