using System;

namespace DiAl.ImageProcessing
{
	public class Point : IComparable<Point>, IEquatable<Point>
	{
		public Int32 X { get; set; }
		public Int32 Y { get; set; }

		public Point() {}
		public Point(Int32 x, Int32 y)
		{
			X = x;
			Y = y;
		}

		public int CompareTo(Point other)
		{
			if (X != other.X)
				return (X - other.X);
			else
				return (Y - other.Y);
		}

		public bool Equals(Point other)
		{
			return ((X == other.X) && (Y == other.Y));
		}

		public override int GetHashCode()
		{
			return ((X + 1) * (Y + 1)).GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is Point && Equals(obj as Point);
		}
	}
}
