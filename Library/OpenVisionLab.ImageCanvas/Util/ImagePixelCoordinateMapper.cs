using System;
using System.Drawing;

namespace OpenVisionLab.ImageCanvas
{
	public static class ImagePixelCoordinateMapper
	{
		public static Point ToTopLeftPixel(PointF openGlPoint, Size imageSize)
		{
			int imageX = (int)Math.Floor(openGlPoint.X);
			int imageY = (int)Math.Floor(imageSize.Height - openGlPoint.Y);
			return new Point(imageX, imageY);
		}

		public static PointF ToOpenGlPixelCenter(Point imagePoint, int imageHeight)
		{
			RectangleF bounds = ToOpenGlPixelBounds(imagePoint, imageHeight);
			return new PointF(bounds.Left + bounds.Width / 2F, bounds.Top + bounds.Height / 2F);
		}

		public static RectangleF ToOpenGlPixelBounds(Point imagePoint, int imageHeight)
		{
			if (imageHeight <= 0) { return RectangleF.Empty; }

			float left = imagePoint.X;
			float top = imageHeight - imagePoint.Y;
			return new RectangleF(left, top - 1F, 1F, 1F);
		}

		public static bool IsPointInImage(Size imageSize, Point imagePoint)
		{
			return imageSize.Width > 0 &&
				imageSize.Height > 0 &&
				imagePoint.X >= 0 &&
				imagePoint.Y >= 0 &&
				imagePoint.X < imageSize.Width &&
				imagePoint.Y < imageSize.Height;
		}
	}
}
