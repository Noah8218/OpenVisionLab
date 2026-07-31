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
	{		public static void DrawMeasurement(OpenGL gl, Measurement measurement, OpenGlFontRenderOptions glFontRenderOptions, List<OpenGlFontBitmapEntry> fontBitmapEntries, float xSpan, float ySpan, System.Drawing.RectangleF fitRect, System.Drawing.SizeF offsetSize, float pixelPermm)
		{
			if (measurement.StartPoint.IsEmpty) { return; }

			float arrowheadLength = 10.0f;
			float arrowheadAngle = 45.0f;

			// 방향 벡터 계산
			float dx = measurement.EndPoint.X - measurement.StartPoint.X;
			float dy = measurement.EndPoint.Y - measurement.StartPoint.Y;
			float magnitude = (float)Math.Sqrt(dx * dx + dy * dy);

			float ux = dx / magnitude;
			float uy = dy / magnitude;

			float angleRadians = (float)(Math.PI * arrowheadAngle / 180.0);
			float arrowheadDx1 = arrowheadLength * (float)(ux * Math.Cos(angleRadians) - uy * Math.Sin(angleRadians));
			float arrowheadDy1 = arrowheadLength * (float)(ux * Math.Sin(angleRadians) + uy * Math.Cos(angleRadians));

			float arrowheadDx2 = arrowheadLength * (float)(ux * Math.Cos(-angleRadians) - uy * Math.Sin(-angleRadians));
			float arrowheadDy2 = arrowheadLength * (float)(ux * Math.Sin(-angleRadians) + uy * Math.Cos(-angleRadians));

			gl.PushAttrib(OpenGL.GL_LINE_BIT | OpenGL.GL_LINE_STIPPLE);

			gl.Color(0.0f, 1.0f, 0.0f);
			gl.LineWidth(5);
			gl.Begin(OpenGL.GL_LINES);
			{
				gl.Vertex(measurement.StartPoint.X, measurement.StartPoint.Y);
				gl.Vertex(measurement.EndPoint.X, measurement.EndPoint.Y);

				gl.Vertex(measurement.StartPoint.X, measurement.StartPoint.Y);
				gl.Vertex(measurement.StartPoint.X + arrowheadDx1, measurement.StartPoint.Y + arrowheadDy1);

				gl.Vertex(measurement.StartPoint.X, measurement.StartPoint.Y);
				gl.Vertex(measurement.StartPoint.X + arrowheadDx2, measurement.StartPoint.Y + arrowheadDy2);

				gl.Vertex(measurement.EndPoint.X, measurement.EndPoint.Y);
				gl.Vertex(measurement.EndPoint.X - arrowheadDx1, measurement.EndPoint.Y - arrowheadDy1);

				gl.Vertex(measurement.EndPoint.X, measurement.EndPoint.Y);
				gl.Vertex(measurement.EndPoint.X - arrowheadDx2, measurement.EndPoint.Y - arrowheadDy2);
			}
			gl.End();

			gl.PopAttrib();


			float distancePermm = measurement.Distance * pixelPermm;
			string distanceText = $"{distancePermm:0.0000} mm / Pixel:{measurement.Distance}";

			float midX = (measurement.StartPoint.X + measurement.EndPoint.X) / 2;
			float midY = (measurement.StartPoint.Y + measurement.EndPoint.Y) / 2;

			string faceName = glFontRenderOptions.FontName;
			float fontSize = glFontRenderOptions.FontSize;
			System.Drawing.Color color = glFontRenderOptions.Color;
			System.Drawing.Color color2 = System.Drawing.Color.Blue;
			System.Drawing.Color color3 = System.Drawing.Color.Yellow;

			//DrawViewerBaseText(gl, fontBitmapEntries, xSpanPixed, ySpanPixed, (int)midX, (int)midY, color2, faceName, 30, distanceText);
			DrawText(gl, fontBitmapEntries, xSpan, ySpan, offsetSize, (int)midX, (int)midY, color, faceName, fontSize, distanceText);
			//DrawFixedText(gl, fontBitmapEntries, (int)midX, (int)midY, offsetSizePixed, color3, faceName, fontSize, distanceText);
		}

		public static void DrawTextOnTexture(OpenGL gl, string text, float texCoordX, float texCoordY, Size glControlSize)
		{
			string faceName = "Arial";
			float fontSize = 30f;
			float screenX = texCoordX * glControlSize.Width;
			float screenY = texCoordY * glControlSize.Height;

			gl.DrawText((int)screenX, (int)screenY, 1, 0, 0, faceName, fontSize, text);
		}
	}
}