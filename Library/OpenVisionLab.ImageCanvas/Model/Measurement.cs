using System;
using System.Drawing;

namespace OpenVisionLab.ImageCanvas
{
	public class Measurement
	{
		public PointF StartPoint { get; set; }
		public PointF EndPoint { get; set; }

		public float Distance => (float)Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2));
	}
}
