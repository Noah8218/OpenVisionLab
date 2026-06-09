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
	{		public static void DrawCrossOfImage(OpenGL gl, ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>> textureAreas)
		{
			Action action = delegate
			{
				if (textureAreas.Count == 0)
					return;

				gl.PushAttrib(OpenGL.GL_LINE_BIT);

				float top = textureAreas.SelectMany(x => x.Value).Max(param => param.GLDrawingTextureArea.Top);
				float bottom = textureAreas.SelectMany(x => x.Value).Min(param => param.GLDrawingTextureArea.Bottom);
				float left = textureAreas.SelectMany(x => x.Value).Min(param => param.GLDrawingTextureArea.Left);
				float right = textureAreas.SelectMany(x => x.Value).Max(param => param.GLDrawingTextureArea.Right);

				float height = top - bottom;
				float width = right - left;

				PointF leftCenter = new PointF(left, bottom + height / 2.0f);
				PointF rightCenter = new PointF(right, bottom + height / 2.0f);
				PointF topCenter = new PointF(left + width / 2.0f, top);
				PointF bottomCenter = new PointF(left + width / 2.0f, bottom);

				gl.Disable(OpenGL.GL_LINE_STIPPLE);
				gl.LineWidth(3.0f);
				gl.Begin(OpenGL.GL_LINES);
				{
					gl.Color(1f, 1f, 0.0f); // Yellow
					gl.Vertex(leftCenter.X, leftCenter.Y);
					gl.Vertex(rightCenter.X, rightCenter.Y);

					gl.Vertex(topCenter.X, topCenter.Y);
					gl.Vertex(bottomCenter.X, bottomCenter.Y);
				}
				gl.End();

				gl.PopAttrib();
			};
			action();
		}

		public static void DrawRoiEditHandles(OpenGL gl, CanvasRect<float> canvasRect, float zoomScale, System.Windows.Media.SolidColorBrush color)
		{
			if (canvasRect == null || canvasRect.IsEmpty()) { return; }

			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);
			gl.Color(r, g, b);

			if (zoomScale > 0.5) { maxHandleSize = 30; }
			else { maxHandleSize = 3; }

			minHandleSize = 3;

			float handlesizeResize = 0;
			handlesizeResize = Math.Max(minHandleSize, Math.Min(handlesizeResize, maxHandleSize));
			canvasRect.LineWidth = (int)handlesizeResize;
			canvasRect.InitializeHandleRects(handlesizeResize);
			DrawRectangleWithHandles(gl, canvasRect.Points.Select(x => new PointF(x.X, x.Y)), handlesizeResize, 2, new float[] { r, g, b });
		}

		public static void DrawRectangleWithHandles(OpenGL gl, IEnumerable<PointF> mainRectPoints, float handleSize, float lineWidth, float[] lineColorRGB)
		{
			//float handlesizeResize = (handleSize / _zoomScale);

			DrawStippleLineLoop(gl, mainRectPoints, lineWidth, lineColorRGB);

			var pointsList = mainRectPoints.ToList();
			PointF topCenter = new PointF((pointsList[0].X + pointsList[1].X) / 2, pointsList[0].Y);
			PointF bottomCenter = new PointF((pointsList[2].X + pointsList[3].X) / 2, pointsList[2].Y);
			PointF leftCenter = new PointF(pointsList[0].X, (pointsList[0].Y + pointsList[3].Y) / 2);
			PointF rightCenter = new PointF(pointsList[1].X, (pointsList[1].Y + pointsList[2].Y) / 2);

			var allPoints = pointsList.Concat(new[] { topCenter, bottomCenter, leftCenter, rightCenter });
			foreach (var center in allPoints)
			{
				float halfSize = handleSize / 2.0f;
				List<PointF> handleRectPoints = new List<PointF>
					{
						new PointF(center.X - halfSize, center.Y - halfSize),
						new PointF(center.X + halfSize, center.Y - halfSize),
						new PointF(center.X + halfSize, center.Y + halfSize),
						new PointF(center.X - halfSize, center.Y + halfSize)
					};

				DrawStippleLineLoop(gl, handleRectPoints, lineWidth, lineColorRGB);
			}
		}

		public static void DrawStippleLineLoop(OpenGL gl, IEnumerable<PointF> points, float lineWidth, float[] lineColorRGB)
		{
			gl.LineWidth(lineWidth);
			gl.Color(lineColorRGB);
			gl.LineStipple(1, 0xffff);
			gl.Enable(OpenGL.GL_LINE_STIPPLE);

			gl.Begin(OpenGL.GL_LINE_LOOP);
			foreach (var pt in points)
			{
				gl.Vertex(pt.X, pt.Y);
			}
			gl.End();

			gl.Disable(OpenGL.GL_LINE_STIPPLE);
		}

		public static List<Point> GetRectangleOutLinePoint(PointF start, PointF end, float lineWidth)
		{
			List<Point> outlinePoints = new List<Point>();
			PointF topLeft = new PointF(start.X, start.Y);
			PointF topRight = new PointF(end.X, start.Y);
			PointF bottomRight = new PointF(end.X, end.Y);
			PointF bottomLeft = new PointF(start.X, end.Y);


			outlinePoints.AddRange(GetThickLinePoint(topLeft, topRight, lineWidth));
			outlinePoints.AddRange(GetThickLinePoint(bottomLeft, bottomRight, lineWidth));
			outlinePoints.AddRange(GetThickLinePoint(topLeft, bottomLeft, lineWidth));
			outlinePoints.AddRange(GetThickLinePoint(topRight, bottomRight, lineWidth));

			return outlinePoints;
		}

		public static void DrawRectangle(OpenGL gl, PointF start, PointF end, float lineWidth, EnumFillMode enumFillMode, System.Windows.Media.SolidColorBrush color, SizeF textureSize = new SizeF())
		{
			//if (start.IsEmpty || end.IsEmpty) { return; }
			if (start.Equals(end)) { return; }

			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			PointF topLeft = new PointF(start.X, start.Y);
			PointF topRight = new PointF(end.X, start.Y);
			PointF bottomRight = new PointF(end.X, end.Y);
			PointF bottomLeft = new PointF(start.X, end.Y);

			gl.Enable(OpenGL.GL_STENCIL_TEST);
			gl.Clear(OpenGL.GL_STENCIL_BUFFER_BIT);
			if (enumFillMode == EnumFillMode.InFill)
			{
				gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

				gl.Color(r, g, b, a);
				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(topLeft.X, topLeft.Y);
				gl.Vertex(topRight.X, topRight.Y);
				gl.Vertex(bottomRight.X, bottomRight.Y);
				gl.Vertex(bottomLeft.X, bottomLeft.Y);
				gl.End();
			}
			else if (enumFillMode == EnumFillMode.OutFill)
			{
				gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

				gl.Color(0, 0, 0, 0);
				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(topLeft.X, topLeft.Y);
				gl.Vertex(topRight.X, topRight.Y);
				gl.Vertex(bottomRight.X, bottomRight.Y);
				gl.Vertex(bottomLeft.X, bottomLeft.Y);
				gl.End();

				gl.StencilFunc(OpenGL.GL_EQUAL, 0, 0xFF);
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_KEEP);

				gl.Color(r, g, b, a);

				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(0, 0);
				gl.Vertex(textureSize.Width, 0);
				gl.Vertex(textureSize.Width, topLeft.Y);
				gl.Vertex(0, topLeft.Y);
				gl.End();

				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(0, bottomRight.Y);
				gl.Vertex(textureSize.Width, bottomRight.Y);
				gl.Vertex(textureSize.Width, textureSize.Height);
				gl.Vertex(0, textureSize.Height);
				gl.End();

				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(0, topLeft.Y);
				gl.Vertex(topLeft.X, topLeft.Y);
				gl.Vertex(topLeft.X, bottomLeft.Y);
				gl.Vertex(0, bottomLeft.Y);
				gl.End();

				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(topRight.X, topRight.Y);
				gl.Vertex(textureSize.Width, topRight.Y);
				gl.Vertex(textureSize.Width, bottomRight.Y);
				gl.Vertex(topRight.X, bottomRight.Y);
				gl.End();
			}

			gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
			gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_KEEP);

			DrawThickLine(gl, topLeft, topRight, lineWidth, color);
			DrawThickLine(gl, bottomLeft, bottomRight, lineWidth, color);
			DrawThickLine(gl, topLeft, bottomLeft, lineWidth, color);
			DrawThickLine(gl, topRight, bottomRight, lineWidth, color);

			DrawPointAsSquare(gl, topLeft, lineWidth, r, g, b, a);
			DrawPointAsSquare(gl, topRight, lineWidth, r, g, b, a);
			DrawPointAsSquare(gl, bottomRight, lineWidth, r, g, b, a);
			DrawPointAsSquare(gl, bottomLeft, lineWidth, r, g, b, a);

			gl.Disable(OpenGL.GL_STENCIL_TEST);
			gl.Disable(OpenGL.GL_BLEND);
		}

		public static void DrawRectangle(OpenGL gl, PointF start, PointF end, float lineWidth, bool isFillMode, System.Windows.Media.SolidColorBrush color)
		{
			DrawRectangle(gl, start, end, lineWidth, EnumFillMode.InFill, color);
		}

		public static void DrawRectangle(OpenGL gl, Rectangle rect, float lineWidth, EnumFillMode enumFillMode, System.Drawing.Color color, bool isInFill = true)
		{
			if (rect.IsEmpty) { return; }

			System.Drawing.PointF start = new PointF(rect.Left, rect.Bottom);
			System.Drawing.PointF end = new PointF(rect.Right, rect.Top);

			DrawRectangle(gl, start, end, lineWidth, enumFillMode, CvUtill.ConvertToSolidColorBrush(color));
		}

		public static void DrawRectangle(OpenGL gl, Rectangle rect, float lineWidth, bool isFillMode, System.Drawing.Color color, bool isInFill = true)
		{
			if (rect.IsEmpty) { return; }

			System.Drawing.PointF start = new PointF(rect.Left, rect.Bottom);
			System.Drawing.PointF end = new PointF(rect.Right, rect.Top);

			DrawRectangle(gl, start, end, lineWidth, EnumFillMode.InFill, CvUtill.ConvertToSolidColorBrush(color));
		}

		public static void DrawRectangle(OpenGL gl, Rectangle rect, float lineWidth, EnumFillMode enumFillMode, System.Windows.Media.SolidColorBrush color)
		{
			if (rect.IsEmpty) { return; }

			System.Drawing.PointF start = new PointF(rect.Left, rect.Bottom);
			System.Drawing.PointF end = new PointF(rect.Right, rect.Top);

			DrawRectangle(gl, start, end, lineWidth, enumFillMode, color);
		}

		public static void DrawRectangle(OpenGL gl, RectangleF rect, float lineWidth, EnumFillMode enumFillMode, System.Windows.Media.SolidColorBrush color, SizeF textureSize = new SizeF())
		{
			if (rect.IsEmpty) { return; }

			System.Drawing.PointF start = new PointF(rect.Left, rect.Bottom);
			System.Drawing.PointF end = new PointF(rect.Right, rect.Top);

			DrawRectangle(gl, start, end, lineWidth, enumFillMode, color, textureSize);
		}

		public static void DrawCircle(OpenGL gl, Rectangle rect, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode, SizeF textureSize)
		{
			if (rect.IsEmpty) { return; }

			System.Drawing.PointF start = new PointF(rect.Left, rect.Bottom);
			System.Drawing.PointF end = new PointF(rect.Right, rect.Top);

			DrawCircle(gl, start, end, lineWidth, color, enumFillMode);
		}

		public static void DrawCircle(OpenGL gl, Rectangle rect, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode)
		{
			if (rect.IsEmpty) { return; }

			System.Drawing.PointF start = new PointF(rect.Left, rect.Bottom);
			System.Drawing.PointF end = new PointF(rect.Right, rect.Top);

			DrawCircle(gl, start, end, lineWidth, color, enumFillMode);
		}

		public static void DrawCircle(OpenGL gl, Rectangle rect, float lineWidth, System.Windows.Media.SolidColorBrush color, bool useFill)
		{
			if (rect.IsEmpty) { return; }

			System.Drawing.PointF start = new PointF(rect.Left, rect.Bottom);
			System.Drawing.PointF end = new PointF(rect.Right, rect.Top);

			EnumFillMode enumFillMode = useFill == true ? EnumFillMode.InFill : EnumFillMode.None;

			DrawCircle(gl, start, end, lineWidth, color, enumFillMode);
		}

		public static List<System.Drawing.Point> GetThickLinePoint(PointF startPoint, PointF endPoint, float lineWidth)
		{
			List<Point> outlinePoints = new List<Point>();

			float dx = endPoint.X - startPoint.X;
			float dy = endPoint.Y - startPoint.Y;
			float length = (float)Math.Sqrt(dx * dx + dy * dy);

			float nx = -dy * lineWidth / length / 2;
			float ny = dx * lineWidth / length / 2;

			if ((int)lineWidth % 2 != 0)
			{
				startPoint.X += nx;
				startPoint.Y += ny;
				endPoint.X += nx;
				endPoint.Y += ny;
			}

			PointF p1 = new PointF((int)(startPoint.X + nx), (int)(startPoint.Y + ny));
			PointF p2 = new PointF((int)(startPoint.X - nx), (int)(startPoint.Y - ny));
			PointF p3 = new PointF((int)(endPoint.X - nx), (int)(endPoint.Y - ny));
			PointF p4 = new PointF((int)(endPoint.X + nx), (int)(endPoint.Y + ny));

			int minX = (int)Math.Min(p1.X, Math.Min(p2.X, Math.Min(p3.X, p4.X)));
			int maxX = (int)Math.Max(p1.X, Math.Max(p2.X, Math.Max(p3.X, p4.X)));
			int minY = (int)Math.Min(p1.Y, Math.Min(p2.Y, Math.Min(p3.Y, p4.Y)));
			int maxY = (int)Math.Max(p1.Y, Math.Max(p2.Y, Math.Max(p3.Y, p4.Y)));

			for (int i = minX; i <= maxX; i++)
			{
				for (int j = minY; j <= maxY; j++)
				{
					outlinePoints.Add(new Point(i, j));
				}
			}

			return outlinePoints;
		}

		public static void DrawThickLine(OpenGL gl, PointF startPoint, PointF endPoint, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			float dx = endPoint.X - startPoint.X;
			float dy = endPoint.Y - startPoint.Y;
			float length = (float)Math.Sqrt(dx * dx + dy * dy);

			if (length == 0) return;

			float nx = -dy * lineWidth / length / 2;
			float ny = dx * lineWidth / length / 2;

			if ((int)lineWidth % 2 != 0)
			{
				startPoint.X += nx;
				startPoint.Y += ny;
				endPoint.X += nx;
				endPoint.Y += ny;
			}

			PointF p1 = new PointF((int)(startPoint.X + nx), (int)(startPoint.Y + ny));
			PointF p2 = new PointF((int)(startPoint.X - nx), (int)(startPoint.Y - ny));
			PointF p3 = new PointF((int)(endPoint.X - nx), (int)(endPoint.Y - ny));
			PointF p4 = new PointF((int)(endPoint.X + nx), (int)(endPoint.Y + ny));

			gl.Color(r, g, b, a);
			gl.Begin(OpenGL.GL_TRIANGLE_FAN);
			gl.Vertex(p1.X, p1.Y);
			gl.Vertex(p2.X, p2.Y);
			gl.Vertex(p3.X, p3.Y);
			gl.Vertex(p4.X, p4.Y);
			gl.End();
		}



		public static void DrawShape(OpenGL gl, CanvasShape shape, System.Drawing.Color color, bool isDotted, bool isFill, float lineWidth = 1.0f)
		{
			CanvasRect<float> canvasRect = shape as CanvasRect<float>;

			SetShapeColorAndStyle(gl, shape, color, isDotted, lineWidth);
			var array = shape.ShapePoints.ToArray();

			gl.Begin(OpenGL.GL_LINE_LOOP);
			for (int i = 0; i < array.Length; ++i)
			{
				gl.Vertex(array[i].X, array[i].Y);
				//if (i < 3)
				//if (i < array.Length - 1)
				//{
				//	gl.Vertex(array[i + 1].X, array[i + 1].Y);
				//}
				//else
				//{
				//	gl.Vertex(array[0].X, array[0].Y);
				//}
			}
			gl.End();
		}


		public static void DrawPointAsSquare(OpenGL gl, PointF point, float size, System.Windows.Media.SolidColorBrush color)
		{
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			DrawPointAsSquare(gl, point, size, r, g, b, a);
		}

		public static void DrawPointAsSquare(OpenGL gl, PointF point, float size, System.Drawing.Color color)
		{
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			DrawPointAsSquare(gl, point, size, r, g, b, a);
		}

		public static void DrawPointsAsColoredSquares(OpenGL gl, List<PointF> points, float size, List<System.Drawing.Color> colors)
		{
			float halfSize = size / 2;
			gl.Begin(OpenGL.GL_QUADS);

			for (int i = 0; i < points.Count; i++)
			{
				PointF point = points[i];
				System.Drawing.Color color = colors[i];
				gl.Color(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);

				if ((int)size % 2 != 0)
				{
					point.X += halfSize;
					point.Y += halfSize;
				}

				gl.Vertex(point.X - halfSize, point.Y - halfSize);
				gl.Vertex(point.X + halfSize, point.Y - halfSize);
				gl.Vertex(point.X + halfSize, point.Y + halfSize);
				gl.Vertex(point.X - halfSize, point.Y + halfSize);
			}

			gl.End();
		}

		public static void DrawFilledPolygon(OpenGL gl, List<System.Drawing.Point> points, System.Drawing.Color fillColor)
		{
			gl.Color(fillColor.R / 255.0f, fillColor.G / 255.0f, fillColor.B / 255.0f);
			gl.Begin(OpenGL.GL_TRIANGLE_FAN);
			foreach (var point in points)
			{
				gl.Vertex(point.X, point.Y);
			}
			gl.End();
		}

		public static void DrawPointAsSquareBlend(OpenGL gl, PointF point, float size, System.Windows.Media.SolidColorBrush color)
		{
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);
			gl.Enable(OpenGL.GL_BLEND);
			DrawPointAsSquare(gl, point, size, r, g, b, 0.5f);
			gl.Disable(OpenGL.GL_BLEND);
		}

		public static void DrawPointAsSquare(OpenGL gl, PointF point, float size, float r, float g, float b, float a)
		{
			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			gl.Color(r, g, b, a);
			gl.Begin(OpenGL.GL_QUADS);
			float halfSize = size / 2;

			if ((int)size % 2 != 0)
			{
				point.X += halfSize;
				point.Y += halfSize;
			}

			gl.Vertex(point.X - halfSize, point.Y - halfSize);
			gl.Vertex(point.X + halfSize, point.Y - halfSize);
			gl.Vertex(point.X + halfSize, point.Y + halfSize);
			gl.Vertex(point.X - halfSize, point.Y + halfSize);
			gl.End();

			gl.Disable(OpenGL.GL_BLEND);
		}

		public static void DrawLineAtAngle(OpenGL gl, PointF startPoint, PointF endPoint, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			if (startPoint.IsEmpty || endPoint.IsEmpty)
				return;
			if (startPoint.Equals(endPoint))
				return;

			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			float angle = (float)Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);

			float adjustedAngle = (float)(Math.Round(angle / (Math.PI / 4)) * (Math.PI / 4));

			List<PointF> pointList = GenerateLineAtAngle(startPoint, endPoint, 1, adjustedAngle);

			for (int i = 0; i < pointList.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(pointList[i].X, pointList[i].Y), lineWidth, r, g, b, a);
			}

			gl.Disable(OpenGL.GL_BLEND);
		}

		public static List<PointF> GenerateLineAtAngle(PointF startPoint, PointF endPoint, float interval, float angle)
		{
			List<PointF> pointList = new List<PointF>();

			float distanceX = endPoint.X - startPoint.X;
			float distanceY = endPoint.Y - startPoint.Y;
			float totalDistance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

			if (totalDistance > 0 && interval > 0)
			{
				int numberOfPoints = (int)(totalDistance / interval);
				for (int i = 0; i <= numberOfPoints; i++)
				{
					float t = i * interval / totalDistance;
					float newX = startPoint.X + t * totalDistance * (float)Math.Cos(angle);
					float newY = startPoint.Y + t * totalDistance * (float)Math.Sin(angle);
					pointList.Add(new PointF(newX, newY));
				}
			}
			return pointList;
		}

		public static void DrawVerticalOrHorizontalLine(OpenGL gl, PointF startPoint, PointF endPoint, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			if (startPoint.IsEmpty || endPoint.IsEmpty)
				return;
			if (startPoint.Equals(endPoint))
				return;

			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			//gl.Enable(OpenGL.GL_BLEND);

			List<PointF> pointList;
			if (Math.Abs(endPoint.X - startPoint.X) > Math.Abs(endPoint.Y - startPoint.Y))
			{
				pointList = GenerateHorizontalPointList(startPoint, endPoint, 1);
			}
			else
			{
				pointList = GenerateVerticalPointList(startPoint, endPoint, 1);
			}

			for (int i = 0; i < pointList.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(pointList[i].X, pointList[i].Y), lineWidth, r, g, b, a);
			}

			gl.Disable(OpenGL.GL_BLEND);
		}

		public static List<PointF> GenerateVerticalPointList(PointF startPoint, PointF endPoint, float interval)
		{
			List<PointF> pointList = new List<PointF>();

			float distanceY = endPoint.Y - startPoint.Y;

			float totalDistance = Math.Abs(distanceY);

			if (totalDistance > 0 && interval > 0)
			{
				int numberOfPoints = (int)(totalDistance / interval);
				for (int i = 0; i <= numberOfPoints; i++)
				{
					float t = i * interval / totalDistance;
					float newY = startPoint.Y + t * distanceY;
					pointList.Add(new PointF(startPoint.X, newY));
				}
			}
			return pointList;
		}

		public static List<PointF> GenerateHorizontalPointList(PointF startPoint, PointF endPoint, float interval)
		{
			List<PointF> pointList = new List<PointF>();

			float distanceX = endPoint.X - startPoint.X;

			float totalDistance = Math.Abs(distanceX);

			if (totalDistance > 0 && interval > 0)
			{
				int numberOfPoints = (int)(totalDistance / interval);
				for (int i = 0; i <= numberOfPoints; i++)
				{
					float t = i * interval / totalDistance;
					float newX = startPoint.X + t * distanceX;
					pointList.Add(new PointF(newX, startPoint.Y));
				}
			}
			return pointList;
		}

		public static void DrawLine(OpenGL gl, PointF startPoint, PointF endPoint, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			if (startPoint.IsEmpty || endPoint.IsEmpty)
				return;
			if (startPoint.Equals(endPoint))
				return;

			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			gl.Enable(OpenGL.GL_BLEND);

			List<PointF> pointList = GeneratePointList(startPoint, endPoint, 1);

			for (int i = 0; i < pointList.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(pointList[i].X, pointList[i].Y), lineWidth, r, g, b, a);
			}

			gl.Disable(OpenGL.GL_BLEND);
		}

		public static List<System.Drawing.PointF> GetLinePoints(PointF startPoint, PointF endPoint, float lineWidth)
		{
			List<PointF> allPoints = new List<PointF>();

			List<PointF> pointList = GeneratePointList(startPoint, endPoint, 1);

			for (int i = 0; i < pointList.Count; i++)
			{
				var points = DrawPointAsSquareAndReturnVertices(new PointF(pointList[i].X, pointList[i].Y), lineWidth);
				allPoints.AddRange(points);
			}

			return allPoints;
		}

		public static void DrawLine(OpenGL gl, PointF startPoint, PointF endPoint, float lineWidth, System.Drawing.Color color)
		{
			if (startPoint.IsEmpty || endPoint.IsEmpty)
				return;
			if (startPoint.Equals(endPoint))
				return;

			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			gl.Color(r, g, b);

			List<PointF> pointList = GeneratePointList(startPoint, endPoint, 1);

			for (int i = 0; i < pointList.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(pointList[i].X, pointList[i].Y), lineWidth, r, g, b, 1);
			}
		}

		public static List<PointF> GeneratePointList(PointF startPoint, PointF endPoint, float interval)
		{
			List<PointF> pointList = new List<PointF>();

			float distanceX = endPoint.X - startPoint.X;
			float distanceY = endPoint.Y - startPoint.Y;

			float totalDistance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

			if (totalDistance > 0 && interval > 0)
			{
				int preX = 0;
				int PreY = 0;
				int numberOfPoints = (int)(totalDistance / interval);
				for (int i = 0; i <= numberOfPoints; i++)
				{
					float t = i * interval / totalDistance;
					int newX = (int)(startPoint.X + t * distanceX);
					int newY = (int)(startPoint.Y + t * distanceY);
					pointList.Add(new PointF(newX, newY));
					if (Math.Abs(newX - preX) == 2)
					{
						if (preX > newX)
						{
							pointList.Add(new PointF(preX - 1, newY));
						}
						else
						{
							pointList.Add(new PointF(newX - 1, newY));
						}
					}
					preX = newX;

					if (Math.Abs(newY - PreY) == 2)
					{
						if (PreY > newY)
						{
							pointList.Add(new PointF(newX, PreY - 1));
						}
						else
						{
							pointList.Add(new PointF(newX, newY - 1));
						}
					}
					PreY = newY;
				}
			}
			return pointList;
		}


		private static List<PointF> GeneratePointList(PointF startPoint, PointF endPoint)
		{
			List<PointF> pointList = new List<PointF>();

			int dx = (int)endPoint.X - (int)startPoint.X;
			int dy = (int)endPoint.Y - (int)startPoint.Y;

			int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

			float xIncrement = dx / (float)steps;
			float yIncrement = dy / (float)steps;

			float x = startPoint.X;
			float y = startPoint.Y;

			for (int i = 0; i <= steps; i++)
			{
				pointList.Add(new PointF(x, y));

				x += xIncrement;
				y += yIncrement;
			}

			return pointList;
		}

		public static void DrawLine(OpenGL gl, List<System.Drawing.PointF> points, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			if (points == null || points.Count < 2)
				return;

			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			gl.Color(r, g, b);

			for (int i = 0; i < points.Count - 1; i++)
			{
				System.Drawing.PointF p1 = points[i];
				System.Drawing.PointF p2 = points[i + 1];

				float dx = p2.X - p1.X;
				float dy = p2.Y - p1.Y;
				float length = (float)Math.Sqrt(dx * dx + dy * dy);

				float ux = lineWidth * (dy / length) / 2;
				float uy = lineWidth * (-dx / length) / 2;

				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(p1.X + ux, p1.Y + uy);
				gl.Vertex(p1.X - ux, p1.Y - uy);
				gl.Vertex(p2.X - ux, p2.Y - uy);
				gl.Vertex(p2.X + ux, p2.Y + uy);
				gl.End();
			}
		}

		public static void DrawCircle(OpenGL gl, PointF start, PointF end, float lineWidth, System.Windows.Media.SolidColorBrush color, bool useFill)
		{
			if (start.IsEmpty || end.IsEmpty) { return; }

			PointF center = new PointF((start.X + end.X) / 2, (start.Y + end.Y) / 2);

			float radius = Math.Min(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)) / 2;

			EnumFillMode enumFillMode = useFill == true ? EnumFillMode.InFill : EnumFillMode.None;

			DrawThickCircle(gl, center.X, center.Y, radius, lineWidth, color, enumFillMode);
		}

		public static void DrawCircle(OpenGL gl, PointF start, PointF end, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode)
		{
			if (start.IsEmpty || end.IsEmpty) { return; }

			PointF center = new PointF((start.X + end.X) / 2, (start.Y + end.Y) / 2);

			float radius = Math.Min(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)) / 2;

			DrawThickCircle(gl, center.X, center.Y, radius, lineWidth, color, enumFillMode);
		}

		public static void DrawCircle(OpenGL gl, PointF start, PointF end, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode, SizeF textureSize)
		{
			if (start.IsEmpty || end.IsEmpty) { return; }

			PointF center = new PointF((start.X + end.X) / 2, (start.Y + end.Y) / 2);

			float radius = Math.Min(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)) / 2;

			DrawThickCircle(gl, center.X, center.Y, radius, lineWidth, color, enumFillMode, textureSize);
		}

		public static void DrawCircle(OpenGL gl, PointF center, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode)
		{
			DrawThickCircle(gl, center.X, center.Y, radius, lineWidth, color, enumFillMode);
		}

		public static void DrawCircle(OpenGL gl, PointF center, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode, SizeF textureSize)
		{
			DrawThickCircle(gl, center.X, center.Y, radius, lineWidth, color, enumFillMode, textureSize);
		}

		public static void DrawCircle(OpenGL gl, PointF center, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, bool useFill)
		{
			EnumFillMode enumFillMode = useFill == true ? EnumFillMode.InFill : EnumFillMode.None;
			DrawThickCircle(gl, center.X, center.Y, radius, lineWidth, color, enumFillMode);
		}

		public static void FastDrawCircle(OpenGL gl, PointF center, float radius, float lineWidth, SolidColorBrush color, bool useFill)
		{
			gl.Color(color.Color.R / 255.0, color.Color.G / 255.0, color.Color.B / 255.0, color.Color.A / 255.0);

			gl.LineWidth(lineWidth);

			if (useFill)
			{
				gl.Begin(OpenGL.GL_TRIANGLE_FAN);
				gl.Vertex(center.X, center.Y);
			}
			else
			{
				gl.Begin(OpenGL.GL_LINE_LOOP);
			}

			int numSegments = 100;
			for (int i = 0; i <= numSegments; ++i)
			{
				double angle = 2.0 * Math.PI * i / numSegments;
				float x = center.X + (float)(Math.Cos(angle) * radius);
				float y = center.Y + (float)(Math.Sin(angle) * radius);
				gl.Vertex(x, y);
			}

			gl.End();
		}

		public static List<System.Drawing.PointF> GetThickCirclePoints(PointF start, PointF end, float lineWidth)
		{
			List<System.Drawing.PointF> circlePoints = new List<System.Drawing.PointF>();

			PointF center = new PointF((start.X + end.X) / 2, (start.Y + end.Y) / 2);
			center = new PointF((float)Math.Round(center.X), (float)Math.Round(center.Y));

			float radius = Math.Min(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)) / 2;

			float innerRadius = radius - lineWidth / 2;
			float outerRadius = radius + lineWidth / 2;

			for (float r = innerRadius; r <= outerRadius; r += 0.1f)
			{
				int x = (int)r;
				int y = 0;
				int d = 1 - x;

				while (y <= x)
				{
					AddCirclePoints(ref circlePoints, (int)center.X, (int)center.Y, x, y);
					AddCirclePoints(ref circlePoints, (int)center.X, (int)center.Y, y, x);

					y++;

					if (d < 0)
					{
						d += 2 * y + 1;
					}
					else
					{
						x--;
						d += 2 * (y - x) + 1;
					}
				}
			}

			return circlePoints.Distinct().ToList();
		}

		private static void AddCirclePoints(ref List<System.Drawing.PointF> points, int cx, int cy, int x, int y)
		{
			points.Add(new PointF(cx + x, cy + y));
			points.Add(new PointF(cx - x, cy + y));
			points.Add(new PointF(cx + x, cy - y));
			points.Add(new PointF(cx - x, cy - y));
			points.Add(new PointF(cx + y, cy + x));
			points.Add(new PointF(cx - y, cy + x));
			points.Add(new PointF(cx + y, cy - x));
			points.Add(new PointF(cx - y, cy - x));
		}

		public static List<PointF> DrawPointAsSquareAndReturnVertices(PointF point, float size)
		{
			List<PointF> vertices = new List<PointF>();
			float halfSize = size / 2;

			if ((int)size % 2 != 0)
			{
				point.X += halfSize;
				point.Y += halfSize;
			}

			for (float x = point.X - halfSize; x < point.X + halfSize; x++)
			{
				for (float y = point.Y - halfSize; y < point.Y + halfSize; y++)
				{
					vertices.Add(new PointF(x, y));
				}
			}

			return vertices;
		}

		public static List<PointF> GetThickCircleWithPoints(float centerX, float centerY, float radius, float lineWidth)
		{
			List<PointF> allPoints = new List<PointF>();
			int x1 = 0;
			int y2 = (int)radius;
			int d = 3 - 2 * (int)radius;

			void DrawCirclePoints(int cx, int cy, int x, int y)
			{
				var points = DrawPointAsSquareAndReturnVertices(new PointF(cx + x, cy + y), lineWidth);
				allPoints.AddRange(points);
				//DrawPointAsSquare(gl, new PointF(cx + x, cy + y), lineWidth, r, g, b, 1);

				points = DrawPointAsSquareAndReturnVertices(new PointF(cx - x, cy + y), lineWidth);
				allPoints.AddRange(points);
				//DrawPointAsSquare(gl, new PointF(cx - x, cy + y), lineWidth, r, g, b, 1);

				points = DrawPointAsSquareAndReturnVertices(new PointF(cx + x, cy - y), lineWidth);
				allPoints.AddRange(points);
				//DrawPointAsSquare(gl, new PointF(cx + x, cy - y), lineWidth, r, g, b, 1);

				points = DrawPointAsSquareAndReturnVertices(new PointF(cx - x, cy - y), lineWidth);
				allPoints.AddRange(points);
				//DrawPointAsSquare(gl, new PointF(cx - x, cy - y), lineWidth, r, g, b, 1);

				points = DrawPointAsSquareAndReturnVertices(new PointF(cx + y, cy + x), lineWidth);
				allPoints.AddRange(points);
				//DrawPointAsSquare(gl, new PointF(cx + y, cy + x), lineWidth, r, g, b, 1);

				points = DrawPointAsSquareAndReturnVertices(new PointF(cx - y, cy + x), lineWidth);
				allPoints.AddRange(points);
				//DrawPointAsSquare(gl, new PointF(cx - y, cy + x), lineWidth, r, g, b, 1);

				points = DrawPointAsSquareAndReturnVertices(new PointF(cx + y, cy - x), lineWidth);
				allPoints.AddRange(points);
				//DrawPointAsSquare(gl, new PointF(cx + y, cy - x), lineWidth, r, g, b, 1);

				points = DrawPointAsSquareAndReturnVertices(new PointF(cx - y, cy - x), lineWidth);
				allPoints.AddRange(points);
				//DrawPointAsSquare(gl, new PointF(cx - y, cy - x), lineWidth, r, g, b, 1);
			}

			while (y2 >= x1)
			{
				DrawCirclePoints((int)centerX, (int)centerY, x1, y2);
				x1++;

				if (d > 0)
				{
					y2--;
					d = d + 4 * (x1 - y2) + 10;
				}
				else
				{
					d = d + 4 * x1 + 6;
				}

				DrawCirclePoints((int)centerX, (int)centerY, x1, y2);
			}

			//return allPoints;
			return allPoints.Distinct().ToList();
		}

		public static void DrawThickCircle(OpenGL gl, float centerX, float centerY, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode)
		{
			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);
			int x1 = 0;
			int y2 = (int)radius;
			int d = 3 - 2 * (int)radius;

			void DrawCirclePoints(int cx, int cy, int x, int y)
			{
				DrawPointAsSquare(gl, new PointF(cx + x, cy + y), lineWidth, r, g, b, a);
				DrawPointAsSquare(gl, new PointF(cx - x, cy + y), lineWidth, r, g, b, a);
				DrawPointAsSquare(gl, new PointF(cx + x, cy - y), lineWidth, r, g, b, a);
				DrawPointAsSquare(gl, new PointF(cx - x, cy - y), lineWidth, r, g, b, a);
				DrawPointAsSquare(gl, new PointF(cx + y, cy + x), lineWidth, r, g, b, a);
				DrawPointAsSquare(gl, new PointF(cx - y, cy + x), lineWidth, r, g, b, a);
				DrawPointAsSquare(gl, new PointF(cx + y, cy - x), lineWidth, r, g, b, a);
				DrawPointAsSquare(gl, new PointF(cx - y, cy - x), lineWidth, r, g, b, a);
			}

			void FillCirclePoints(int cx, int cy, int radius2)
			{
				gl.Color(r, g, b, a);
				gl.Begin(OpenGL.GL_TRIANGLE_FAN);
				gl.Vertex(cx, cy);
				for (int angle = 0; angle <= 360; angle += 5)
				{
					float angleRad = (float)(Math.PI * angle / 180.0);
					float x = cx + radius2 * (float)Math.Cos(angleRad);
					float y = cy + radius2 * (float)Math.Sin(angleRad);
					gl.Vertex(x, y);
				}
				gl.End();
			}

			while (y2 >= x1)
			{
				if (enumFillMode == EnumFillMode.InFill)
				{
					FillCirclePoints((int)centerX, (int)centerY, (int)radius);
					break; // If we fill the circle, we don't need to continue the loop
				}
				else if (enumFillMode == EnumFillMode.None)
				{
					DrawCirclePoints((int)centerX, (int)centerY, x1, y2);
				}

				x1++;

				if (d > 0)
				{
					y2--;
					d = d + 4 * (x1 - y2) + 10;
				}
				else
				{
					d = d + 4 * x1 + 6;
				}

				if (enumFillMode == EnumFillMode.None)
				{
					DrawCirclePoints((int)centerX, (int)centerY, x1, y2);
				}
			}

			gl.Disable(OpenGL.GL_BLEND);
		}

		public static void DrawThickCircle(OpenGL gl, float centerX, float centerY, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode, SizeF textureSize)
		{
			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			gl.Enable(OpenGL.GL_STENCIL_TEST);
			gl.Clear(OpenGL.GL_STENCIL_BUFFER_BIT);

			if (enumFillMode == EnumFillMode.InFill)
			{
				gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

				FillCirclePoints(gl, (int)centerX, (int)centerY, (int)radius, r, g, b, a);
			}
			else if (enumFillMode == EnumFillMode.OutFill)
			{
				gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

				FillCirclePoints(gl, (int)centerX, (int)centerY, (int)radius, 0, 0, 0, 0);

				gl.StencilFunc(OpenGL.GL_EQUAL, 0, 0xFF);
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_KEEP);

				gl.Color(r, g, b, a);

				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(0, 0);
				gl.Vertex(textureSize.Width, 0);
				gl.Vertex(textureSize.Width, textureSize.Height);
				gl.Vertex(0, textureSize.Height);
				gl.End();
			}
			else if (enumFillMode == EnumFillMode.None)
			{
				int x1 = 0;
				int y2 = (int)radius;
				int d = 3 - 2 * (int)radius;

				while (y2 >= x1)
				{
					DrawCirclePoints(gl, (int)centerX, (int)centerY, x1, y2, lineWidth, r, g, b, a);

					x1++;
					if (d > 0)
					{
						y2--;
						d = d + 4 * (x1 - y2) + 10;
					}
					else
					{
						d = d + 4 * x1 + 6;
					}

					DrawCirclePoints(gl, (int)centerX, (int)centerY, x1, y2, lineWidth, r, g, b, a);
				}
			}

			gl.Disable(OpenGL.GL_STENCIL_TEST);
			gl.Disable(OpenGL.GL_BLEND);
		}



		private static void FillCirclePoints(OpenGL gl, int cx, int cy, int radius, float r, float g, float b, float a)
		{
			gl.Color(r, g, b, a);
			gl.Begin(OpenGL.GL_TRIANGLE_FAN);
			gl.Vertex(cx, cy);
			for (int angle = 0; angle <= 360; angle += 1)
			{
				float angleRad = (float)(Math.PI * angle / 180.0);
				float x = cx + radius * (float)Math.Cos(angleRad);
				float y = cy + radius * (float)Math.Sin(angleRad);
				gl.Vertex(x, y);
			}
			gl.End();
		}

		private static void DrawCirclePoints(OpenGL gl, int cx, int cy, int x, int y, float lineWidth, float r, float g, float b, float a)
		{
			DrawPointAsSquare(gl, new PointF(cx + x, cy + y), lineWidth, r, g, b, a);
			DrawPointAsSquare(gl, new PointF(cx - x, cy + y), lineWidth, r, g, b, a);
			DrawPointAsSquare(gl, new PointF(cx + x, cy - y), lineWidth, r, g, b, a);
			DrawPointAsSquare(gl, new PointF(cx - x, cy - y), lineWidth, r, g, b, a);
			DrawPointAsSquare(gl, new PointF(cx + y, cy + x), lineWidth, r, g, b, a);
			DrawPointAsSquare(gl, new PointF(cx - y, cy + x), lineWidth, r, g, b, a);
			DrawPointAsSquare(gl, new PointF(cx + y, cy - x), lineWidth, r, g, b, a);
			DrawPointAsSquare(gl, new PointF(cx - y, cy - x), lineWidth, r, g, b, a);
		}


		public static void DrawThickCircle(OpenGL gl, PointF center, List<PointF> pointFs, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, bool isFiil = false)
		{
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			gl.Color(r, g, b);
			if (isFiil) { DrawFilledCircle(gl, center, radius, lineWidth, color); }
			else
			{
				foreach (var point in pointFs)
				{
					DrawPointAsSquare(gl, point, 1, color);
				}
			}
		}

		private static bool CheckIfInteger(float coordinate)
		{
			bool isInteger = true;
			if (Math.Abs(coordinate - Math.Round(coordinate)) < 0.00001)
			{
				isInteger = true;
			}
			else
			{
				isInteger = false;
			}
			return isInteger;
		}

		public static void DrawFilledCircle(OpenGL gl, PointF center, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			int segments = 300;
			float twoPi = 2.0f * (float)Math.PI;

			gl.Color(r, g, b);
			gl.Begin(OpenGL.GL_TRIANGLE_FAN);
			gl.Vertex(center.X, center.Y);
			for (int i = 0; i <= segments; i++)
			{
				float theta = twoPi * i / segments;
				float x = center.X + (radius + (lineWidth / 2)) * (float)Math.Cos(theta);
				float y = center.Y + (radius + (lineWidth / 2)) * (float)Math.Sin(theta);
				gl.Vertex(x, y);
			}
			gl.End();
		}

		private static void SetShapeColorAndStyle(OpenGL gl, CanvasShape shape, System.Drawing.Color color, bool isDotted, float lineWidth = 1.0f)
		{
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			gl.Color(r, g, b, a);

			gl.LineWidth(lineWidth);

			if (shape is CanvasRect<float>)
			{
				if ((shape as CanvasRect<float>).IsEditing)
				{
					gl.Color(System.Drawing.Color.Yellow.R / 255.0f, System.Drawing.Color.Yellow.G / 255.0f, System.Drawing.Color.Yellow.B / 255.0f);
				}
			}

			if (isDotted)
			{
				gl.Enable(OpenGL.GL_LINE_STIPPLE);
				gl.LineStipple(1, 0x00FF);
			}
			else
			{
				gl.Disable(OpenGL.GL_LINE_STIPPLE);
			}
		}

		public static void DrawLineLoop(OpenGL gl, IEnumerable<PointF> points, float lineWidth, float[] lineColorRGB)
		{
			gl.LineWidth(lineWidth);
			gl.Color(lineColorRGB);
			//gl.Enable(OpenGL.GL_BLEND);
			//gl.BlendFunc(SharpGL.Enumerations.BlendingSourceFactor.OneMinusDestinationColor, SharpGL.Enumerations.BlendingDestinationFactor.OneMinusSourceColor);
			gl.Begin(OpenGL.GL_LINE_LOOP);
			{
				foreach (var pt in points)
				{
					gl.Vertex(pt.X, pt.Y);
				}
			}
			gl.End();
		}

		public static void DrawLine(OpenGL gl, IEnumerable<PointF> points, float lineWidth, float[] lineColorRGB)
		{
			gl.LineWidth(lineWidth);
			gl.Color(lineColorRGB);
			gl.Begin(OpenGL.GL_LINE_STRIP);
			{
				foreach (var pt in points)
				{
					gl.Vertex(pt.X, pt.Y);
				}
			}
			gl.End();
		}

		public static void DrawLineLoopPx(OpenGL gl, IEnumerable<PointF> points, float lineWidth, float[] lineColorRGB, RectangleF textureArea, float pixelSizeX, float pixelSizeY)
		{
			gl.LineWidth(lineWidth);
			gl.Color(lineColorRGB);
			gl.Begin(OpenGL.GL_LINE_LOOP);               // Start Drawing The Pyramid
			{
				foreach (var pt in points.Select(x => PixelToRobot(x.X, x.Y, textureArea, pixelSizeX, pixelSizeY)))
				{
					gl.Vertex(pt.X, pt.Y);
				}
			}
			gl.End();
		}

		private static PointF PixelToRobot(float x, float y, RectangleF textureArea, float pixelSizeX, float pixelSizeY)
		{
			float robotX = (x * pixelSizeX + textureArea.Left);
			float robotY = (y * pixelSizeY + textureArea.Bottom);

			return new PointF(robotX, robotY);
		}

		public static void DrawPoint(OpenGL gl, System.Drawing.PointF point, float r, float g, float b, float pointSize = 1.0f)
		{
			gl.PointSize(pointSize);
			gl.Color(r, g, b);
			gl.Begin(OpenGL.GL_POINTS);
			{
				gl.Vertex(point.X, point.Y);
			}
			gl.End();
		}

		public static void DrawEllipse(OpenGL gl, float cx, float cy, float rx, float ry, int num_segments, float lineWidth, float[] lineColorRGB)
		{
			double theta = 2 * Math.PI / (double)num_segments;
			double c = Math.Cos(theta);//precalculate the sine and cosine
			double s = Math.Sin(theta);
			double t;

			double x = 1;//we start at angle = 0
			double y = 0;

			gl.LineWidth(lineWidth);
			gl.Begin(OpenGL.GL_LINE_LOOP);
			for (int ii = 0; ii < num_segments; ii++)
			{
				//apply radius and offset
				gl.Vertex(x * rx + cx, y * ry + cy);//output vertex

				//apply the rotation matrix
				t = x;
				x = c * x - s * y;
				y = s * t + c * y;
			}
			gl.End();
		}
	}
}