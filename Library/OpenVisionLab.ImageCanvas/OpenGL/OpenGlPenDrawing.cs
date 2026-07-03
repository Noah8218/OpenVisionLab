using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.OpenCVSharp;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using SharpGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace OpenVisionLab.ImageCanvas.OpenGLRendering
{
	public static partial class OpenGlDrawing
	{
		private static List<List<System.Drawing.PointF>> SplitIntoSegments(List<DotInfo> points)
		{
			var segments = new List<List<System.Drawing.PointF>>();
			var currentSegment = new List<System.Drawing.PointF>();
			bool lastPointWasNull = true;

			foreach (DotInfo point in points)
			{
				if (point == null)
				{
					if (!lastPointWasNull && currentSegment.Count > 0)
					{
						segments.Add(new List<System.Drawing.PointF>(currentSegment));
						currentSegment.Clear();
					}
					lastPointWasNull = true;
				}
				else
				{
					if (lastPointWasNull && currentSegment.Count > 0)
					{
						segments.Add(new List<System.Drawing.PointF>(currentSegment));
						currentSegment.Clear();
					}
					currentSegment.Add(new PointF(point.X, point.Y));
					lastPointWasNull = false;
				}
			}

			if (!lastPointWasNull && currentSegment.Count > 0)
			{
				segments.Add(currentSegment);
			}

			return segments;
		}

		public static List<System.Drawing.RectangleF> GetDrawAreas(List<System.Drawing.PointF> points, float lineWidth)
		{
			List<System.Drawing.RectangleF> drawnAreas = new List<System.Drawing.RectangleF>();
			float halfLineWidth = lineWidth / 2;

			foreach (System.Drawing.PointF point in points)
			{
				System.Drawing.PointF newPoint = new System.Drawing.PointF(point.X, point.Y);
				if ((int)lineWidth % 2 != 0)
				{
					newPoint.X += halfLineWidth;
					newPoint.Y += halfLineWidth;
				}

				System.Drawing.RectangleF drawnArea = new System.Drawing.RectangleF(
					newPoint.X - halfLineWidth,
					newPoint.Y - halfLineWidth,
					lineWidth,
					lineWidth
				);
				drawnAreas.Add(drawnArea);
			}

			return drawnAreas;
		}

		public static List<System.Drawing.RectangleF> DrawWithPenAndGetDrawAreas(OpenGL gl, List<System.Drawing.PointF> points, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			List<System.Drawing.RectangleF> drawnAreas = new List<System.Drawing.RectangleF>();
			float halfLineWidth = lineWidth / 2;

			foreach (System.Drawing.PointF point in points)
			{
				System.Drawing.PointF newPoint = new System.Drawing.PointF(point.X, point.Y);
				if ((int)lineWidth % 2 != 0)
				{
					newPoint.X += halfLineWidth;
					newPoint.Y += halfLineWidth;
				}

				System.Drawing.RectangleF drawnArea = new System.Drawing.RectangleF(
					newPoint.X - halfLineWidth,
					newPoint.Y - halfLineWidth,
					lineWidth,
					lineWidth
				);
				OpenGlDrawing.DrawPointAsSquare(gl, point, lineWidth, color);
				drawnAreas.Add(drawnArea);
			}

			return drawnAreas;
		}

		public static void DrawWithPen(OpenGL gl, List<Point> points, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			for (int i = 0; i < points.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(points[i].X, points[i].Y), lineWidth, color);
			}
		}

		public static void DrawWithPen(OpenGL gl, List<PointF> points, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			gl.Enable(OpenGL.GL_STENCIL_TEST);
			gl.Clear(OpenGL.GL_STENCIL_BUFFER_BIT);

			for (int i = 0; i < points.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(points[i].X, points[i].Y), lineWidth, r, g, b, a);
			}

			gl.Disable(OpenGL.GL_STENCIL_TEST);
		}


		public static void DrawWithPen(OpenGL gl, List<PointF> points, float lineWidth, System.Drawing.Color color)
		{
			for (int i = 0; i < points.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(points[i].X, points[i].Y), lineWidth, color);
			}
		}

		public static void DrawWithPen(OpenGL gl, List<DotInfo> points, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			DrawWithPen(gl, ConvertDotInfoToPoints(points), lineWidth, color);
		}
	}
}