using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using SharpGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;

namespace OpenVisionLab.ImageCanvas.OpenGLRendering
{
	/// <summary>
	/// Public compatibility façade for the ImageCanvas OpenGL drawing API.
	/// Responsibility-specific implementation lives in the concrete renderers.
	/// </summary>
	public static class OpenGlDrawing
	{
		public static float ZoomFactor = 1.0f;
		internal const int FontGlyphCount = 256;

		#region OpenGlColorConverter
		public static (float, float, float, float) ConvertColorToOpenGLRGB(System.Drawing.Color color) => OpenGlColorConverter.ConvertColorToOpenGLRGB(color);

		public static System.Drawing.Color ConvertOpenGLRGBToColor(float red, float green, float blue, float alpha) => OpenGlColorConverter.ConvertOpenGLRGBToColor(red, green, blue, alpha);

		public static float[] ConvertColorToOpenGLRGBArr(System.Drawing.Color color) => OpenGlColorConverter.ConvertColorToOpenGLRGBArr(color);

		public static System.Drawing.Color ConvertOpenGLRGBArrToColor(float[] rgba) => OpenGlColorConverter.ConvertOpenGLRGBArrToColor(rgba);

		public static (float, float, float, float) ConvertColorToOpenGLRGB(System.Windows.Media.SolidColorBrush brush) => OpenGlColorConverter.ConvertColorToOpenGLRGB(brush);

		public static System.Windows.Media.SolidColorBrush ConvertOpenGLRGBToBrush(float red, float green, float blue, float alpha) => OpenGlColorConverter.ConvertOpenGLRGBToBrush(red, green, blue, alpha);

		public static System.Windows.Media.SolidColorBrush ConvertOpenGLRGBArrToBrush(float[] rgba) => OpenGlColorConverter.ConvertOpenGLRGBArrToBrush(rgba);

		public static System.Windows.Media.Color ToMediaColor(System.Drawing.Color color) => OpenGlColorConverter.ToMediaColor(color);


		#endregion

		#region OpenGlTextureRenderer
		public static void DrawODBTexture(OpenGL gl, ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>> textureAreas, List<string> order) => OpenGlTextureRenderer.DrawODBTexture(gl, textureAreas, order);

		public static void DrawTexture(OpenGL gl, ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>> textureAreas, List<string> order) => OpenGlTextureRenderer.DrawTexture(gl, textureAreas, order);

		public static void DrawTexturedQuadWithTransparency(OpenGL gl, uint textureId, RectangleF drawArea) => OpenGlTextureRenderer.DrawTexturedQuadWithTransparency(gl, textureId, drawArea);

		public static void DrawQuad(OpenGL gl, RectangleF rect) => OpenGlTextureRenderer.DrawQuad(gl, rect);

		public static void DrawIrregularQuad(OpenGL gl, RectangleF rect, System.Windows.Media.SolidColorBrush solidColorBrush) => OpenGlTextureRenderer.DrawIrregularQuad(gl, rect, solidColorBrush);


		#endregion

		#region OpenGlShapeRenderer
		public static void DrawCrossOfImage(OpenGL gl, ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>> textureAreas) => OpenGlShapeRenderer.DrawCrossOfImage(gl, textureAreas);

		public static void DrawRoiEditHandles(OpenGL gl, CanvasRect<float> canvasRect, float zoomScale, System.Windows.Media.SolidColorBrush color) => OpenGlShapeRenderer.DrawRoiEditHandles(gl, canvasRect, zoomScale, color);

		public static void DrawRectangleWithHandles(OpenGL gl, IEnumerable<PointF> mainRectPoints, float handleSize, float lineWidth, float[] lineColorRGB) => OpenGlShapeRenderer.DrawRectangleWithHandles(gl, mainRectPoints, handleSize, lineWidth, lineColorRGB);

		public static void DrawStippleLineLoop(OpenGL gl, IEnumerable<PointF> points, float lineWidth, float[] lineColorRGB) => OpenGlShapeRenderer.DrawStippleLineLoop(gl, points, lineWidth, lineColorRGB);

		public static List<Point> GetRectangleOutLinePoint(PointF start, PointF end, float lineWidth) => OpenGlShapeRenderer.GetRectangleOutLinePoint(start, end, lineWidth);

		public static void DrawRectangle(OpenGL gl, PointF start, PointF end, float lineWidth, EnumFillMode enumFillMode, System.Windows.Media.SolidColorBrush color, SizeF textureSize = new SizeF()) => OpenGlShapeRenderer.DrawRectangle(gl, start, end, lineWidth, enumFillMode, color, textureSize);

		public static void DrawRectangle(OpenGL gl, PointF start, PointF end, float lineWidth, bool isFillMode, System.Windows.Media.SolidColorBrush color) => OpenGlShapeRenderer.DrawRectangle(gl, start, end, lineWidth, isFillMode, color);

		public static void DrawRectangle(OpenGL gl, Rectangle rect, float lineWidth, EnumFillMode enumFillMode, System.Drawing.Color color, bool isInFill = true) => OpenGlShapeRenderer.DrawRectangle(gl, rect, lineWidth, enumFillMode, color, isInFill);

		public static void DrawRectangle(OpenGL gl, Rectangle rect, float lineWidth, bool isFillMode, System.Drawing.Color color, bool isInFill = true) => OpenGlShapeRenderer.DrawRectangle(gl, rect, lineWidth, isFillMode, color, isInFill);

		public static void DrawRectangle(OpenGL gl, Rectangle rect, float lineWidth, EnumFillMode enumFillMode, System.Windows.Media.SolidColorBrush color) => OpenGlShapeRenderer.DrawRectangle(gl, rect, lineWidth, enumFillMode, color);

		public static void DrawRectangle(OpenGL gl, RectangleF rect, float lineWidth, EnumFillMode enumFillMode, System.Windows.Media.SolidColorBrush color, SizeF textureSize = new SizeF()) => OpenGlShapeRenderer.DrawRectangle(gl, rect, lineWidth, enumFillMode, color, textureSize);

		public static void DrawCircle(OpenGL gl, Rectangle rect, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode, SizeF textureSize) => OpenGlShapeRenderer.DrawCircle(gl, rect, lineWidth, color, enumFillMode, textureSize);

		public static void DrawCircle(OpenGL gl, Rectangle rect, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode) => OpenGlShapeRenderer.DrawCircle(gl, rect, lineWidth, color, enumFillMode);

		public static void DrawCircle(OpenGL gl, Rectangle rect, float lineWidth, System.Windows.Media.SolidColorBrush color, bool useFill) => OpenGlShapeRenderer.DrawCircle(gl, rect, lineWidth, color, useFill);

		public static List<System.Drawing.Point> GetThickLinePoint(PointF startPoint, PointF endPoint, float lineWidth) => OpenGlShapeRenderer.GetThickLinePoint(startPoint, endPoint, lineWidth);

		public static void DrawThickLine(OpenGL gl, PointF startPoint, PointF endPoint, float lineWidth, System.Windows.Media.SolidColorBrush color) => OpenGlShapeRenderer.DrawThickLine(gl, startPoint, endPoint, lineWidth, color);

		public static void DrawShape(OpenGL gl, CanvasShape shape, System.Drawing.Color color, bool isDotted, bool isFill, float lineWidth = 1.0f) => OpenGlShapeRenderer.DrawShape(gl, shape, color, isDotted, isFill, lineWidth);

		public static void DrawPointAsSquare(OpenGL gl, PointF point, float size, System.Windows.Media.SolidColorBrush color) => OpenGlShapeRenderer.DrawPointAsSquare(gl, point, size, color);

		public static void DrawPointAsSquare(OpenGL gl, PointF point, float size, System.Drawing.Color color) => OpenGlShapeRenderer.DrawPointAsSquare(gl, point, size, color);

		public static void DrawPointsAsColoredSquares(OpenGL gl, List<PointF> points, float size, List<System.Drawing.Color> colors) => OpenGlShapeRenderer.DrawPointsAsColoredSquares(gl, points, size, colors);

		public static void DrawFilledPolygon(OpenGL gl, List<System.Drawing.Point> points, System.Drawing.Color fillColor) => OpenGlShapeRenderer.DrawFilledPolygon(gl, points, fillColor);

		public static void DrawPointAsSquareBlend(OpenGL gl, PointF point, float size, System.Windows.Media.SolidColorBrush color) => OpenGlShapeRenderer.DrawPointAsSquareBlend(gl, point, size, color);

		public static void DrawPointAsSquare(OpenGL gl, PointF point, float size, float r, float g, float b, float a) => OpenGlShapeRenderer.DrawPointAsSquare(gl, point, size, r, g, b, a);

		public static void DrawLineAtAngle(OpenGL gl, PointF startPoint, PointF endPoint, float lineWidth, System.Windows.Media.SolidColorBrush color) => OpenGlShapeRenderer.DrawLineAtAngle(gl, startPoint, endPoint, lineWidth, color);

		public static List<PointF> GenerateLineAtAngle(PointF startPoint, PointF endPoint, float interval, float angle) => OpenGlShapeRenderer.GenerateLineAtAngle(startPoint, endPoint, interval, angle);

		public static void DrawVerticalOrHorizontalLine(OpenGL gl, PointF startPoint, PointF endPoint, float lineWidth, System.Windows.Media.SolidColorBrush color) => OpenGlShapeRenderer.DrawVerticalOrHorizontalLine(gl, startPoint, endPoint, lineWidth, color);

		public static List<PointF> GenerateVerticalPointList(PointF startPoint, PointF endPoint, float interval) => OpenGlShapeRenderer.GenerateVerticalPointList(startPoint, endPoint, interval);

		public static List<PointF> GenerateHorizontalPointList(PointF startPoint, PointF endPoint, float interval) => OpenGlShapeRenderer.GenerateHorizontalPointList(startPoint, endPoint, interval);

		public static void DrawLine(OpenGL gl, PointF startPoint, PointF endPoint, float lineWidth, System.Windows.Media.SolidColorBrush color) => OpenGlShapeRenderer.DrawLine(gl, startPoint, endPoint, lineWidth, color);

		public static List<System.Drawing.PointF> GetLinePoints(PointF startPoint, PointF endPoint, float lineWidth) => OpenGlShapeRenderer.GetLinePoints(startPoint, endPoint, lineWidth);

		public static void DrawLine(OpenGL gl, PointF startPoint, PointF endPoint, float lineWidth, System.Drawing.Color color) => OpenGlShapeRenderer.DrawLine(gl, startPoint, endPoint, lineWidth, color);

		public static List<PointF> GeneratePointList(PointF startPoint, PointF endPoint, float interval) => OpenGlShapeRenderer.GeneratePointList(startPoint, endPoint, interval);

		public static void DrawLine(OpenGL gl, List<System.Drawing.PointF> points, float lineWidth, System.Windows.Media.SolidColorBrush color) => OpenGlShapeRenderer.DrawLine(gl, points, lineWidth, color);

		public static void DrawCircle(OpenGL gl, PointF start, PointF end, float lineWidth, System.Windows.Media.SolidColorBrush color, bool useFill) => OpenGlShapeRenderer.DrawCircle(gl, start, end, lineWidth, color, useFill);

		public static void DrawCircle(OpenGL gl, PointF start, PointF end, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode) => OpenGlShapeRenderer.DrawCircle(gl, start, end, lineWidth, color, enumFillMode);

		public static void DrawCircle(OpenGL gl, PointF start, PointF end, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode, SizeF textureSize) => OpenGlShapeRenderer.DrawCircle(gl, start, end, lineWidth, color, enumFillMode, textureSize);

		public static void DrawCircle(OpenGL gl, PointF center, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode) => OpenGlShapeRenderer.DrawCircle(gl, center, radius, lineWidth, color, enumFillMode);

		public static void DrawCircle(OpenGL gl, PointF center, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode, SizeF textureSize) => OpenGlShapeRenderer.DrawCircle(gl, center, radius, lineWidth, color, enumFillMode, textureSize);

		public static void DrawCircle(OpenGL gl, PointF center, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, bool useFill) => OpenGlShapeRenderer.DrawCircle(gl, center, radius, lineWidth, color, useFill);

		public static void FastDrawCircle(OpenGL gl, PointF center, float radius, float lineWidth, SolidColorBrush color, bool useFill) => OpenGlShapeRenderer.FastDrawCircle(gl, center, radius, lineWidth, color, useFill);

		public static List<System.Drawing.PointF> GetThickCirclePoints(PointF start, PointF end, float lineWidth) => OpenGlShapeRenderer.GetThickCirclePoints(start, end, lineWidth);

		public static List<PointF> DrawPointAsSquareAndReturnVertices(PointF point, float size) => OpenGlShapeRenderer.DrawPointAsSquareAndReturnVertices(point, size);

		public static List<PointF> GetThickCircleWithPoints(float centerX, float centerY, float radius, float lineWidth) => OpenGlShapeRenderer.GetThickCircleWithPoints(centerX, centerY, radius, lineWidth);

		public static void DrawThickCircle(OpenGL gl, float centerX, float centerY, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode) => OpenGlShapeRenderer.DrawThickCircle(gl, centerX, centerY, radius, lineWidth, color, enumFillMode);

		public static void DrawThickCircle(OpenGL gl, float centerX, float centerY, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode, SizeF textureSize) => OpenGlShapeRenderer.DrawThickCircle(gl, centerX, centerY, radius, lineWidth, color, enumFillMode, textureSize);

		public static void DrawThickCircle(OpenGL gl, PointF center, List<PointF> pointFs, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, bool isFiil = false) => OpenGlShapeRenderer.DrawThickCircle(gl, center, pointFs, radius, lineWidth, color, isFiil);

		public static void DrawFilledCircle(OpenGL gl, PointF center, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color) => OpenGlShapeRenderer.DrawFilledCircle(gl, center, radius, lineWidth, color);

		public static void DrawLineLoop(OpenGL gl, IEnumerable<PointF> points, float lineWidth, float[] lineColorRGB) => OpenGlShapeRenderer.DrawLineLoop(gl, points, lineWidth, lineColorRGB);

		public static void DrawLine(OpenGL gl, IEnumerable<PointF> points, float lineWidth, float[] lineColorRGB) => OpenGlShapeRenderer.DrawLine(gl, points, lineWidth, lineColorRGB);

		public static void DrawLineLoopPx(OpenGL gl, IEnumerable<PointF> points, float lineWidth, float[] lineColorRGB, RectangleF textureArea, float pixelSizeX, float pixelSizeY) => OpenGlShapeRenderer.DrawLineLoopPx(gl, points, lineWidth, lineColorRGB, textureArea, pixelSizeX, pixelSizeY);

		public static void DrawPoint(OpenGL gl, System.Drawing.PointF point, float r, float g, float b, float pointSize = 1.0f) => OpenGlShapeRenderer.DrawPoint(gl, point, r, g, b, pointSize);

		public static void DrawEllipse(OpenGL gl, float cx, float cy, float rx, float ry, int num_segments, float lineWidth, float[] lineColorRGB) => OpenGlShapeRenderer.DrawEllipse(gl, cx, cy, rx, ry, num_segments, lineWidth, lineColorRGB);


		#endregion

		#region OpenGlPenRenderer
		public static List<System.Drawing.RectangleF> GetDrawAreas(List<System.Drawing.PointF> points, float lineWidth) => OpenGlPenRenderer.GetDrawAreas(points, lineWidth);

		public static List<System.Drawing.RectangleF> DrawWithPenAndGetDrawAreas(OpenGL gl, List<System.Drawing.PointF> points, float lineWidth, System.Windows.Media.SolidColorBrush color) => OpenGlPenRenderer.DrawWithPenAndGetDrawAreas(gl, points, lineWidth, color);

		public static void DrawWithPen(OpenGL gl, List<Point> points, float lineWidth, System.Windows.Media.SolidColorBrush color) => OpenGlPenRenderer.DrawWithPen(gl, points, lineWidth, color);

		public static void DrawWithPen(OpenGL gl, List<PointF> points, float lineWidth, System.Windows.Media.SolidColorBrush color) => OpenGlPenRenderer.DrawWithPen(gl, points, lineWidth, color);

		public static void DrawWithPen(OpenGL gl, List<PointF> points, float lineWidth, System.Drawing.Color color) => OpenGlPenRenderer.DrawWithPen(gl, points, lineWidth, color);

		public static void DrawWithPen(OpenGL gl, List<DotInfo> points, float lineWidth, System.Windows.Media.SolidColorBrush color) => OpenGlPenRenderer.DrawWithPen(gl, points, lineWidth, color);


		#endregion

		#region OpenGlTextRenderer
		public static void DrawText(OpenGL gl, List<OpenGlFontBitmapEntry> fontBitmapEntries, float xSpan, float ySpan, SizeF offsetSize, float x, float y, System.Drawing.Color color, string faceName, float baseFontSize, string text) => OpenGlTextRenderer.DrawText(gl, fontBitmapEntries, xSpan, ySpan, offsetSize, x, y, color, faceName, baseFontSize, text);

		public static void DrawTextAt(OpenGL gl, List<OpenGlFontBitmapEntry> fontBitmapEntries, string text, float x, float y, int fontSize, System.Drawing.Color color, bool originTop = true) => OpenGlTextRenderer.DrawTextAt(gl, fontBitmapEntries, text, x, y, fontSize, color, originTop);

		public static void DrawFixedText(OpenGL gl, List<OpenGlFontBitmapEntry> fontBitmapEntries, float x, float y, SizeF offsetSize, System.Drawing.Color color, string faceName, float fontSize, string text) => OpenGlTextRenderer.DrawFixedText(gl, fontBitmapEntries, x, y, offsetSize, color, faceName, fontSize, text);

		public static OpenGlFontBitmapEntry CreateOpenGlFontBitmapEntry(OpenGL gl, List<OpenGlFontBitmapEntry> fontBitmapEntries, string faceName, int height) => OpenGlTextRenderer.CreateOpenGlFontBitmapEntry(gl, fontBitmapEntries, faceName, height);

		public static void DrawTextOnStaticPosition(OpenGL gl, float zoomScale, int x, int y, float r, float g, float b, string faceName, float fontSize, string text) => OpenGlTextRenderer.DrawTextOnStaticPosition(gl, zoomScale, x, y, r, g, b, faceName, fontSize, text);


		#endregion

		#region OpenGlMeasurementRenderer
		public static void DrawMeasurement(OpenGL gl, Measurement measurement, OpenGlFontRenderOptions glFontRenderOptions, List<OpenGlFontBitmapEntry> fontBitmapEntries, float xSpan, float ySpan, System.Drawing.RectangleF fitRect, System.Drawing.SizeF offsetSize, float pixelPermm) => OpenGlMeasurementRenderer.DrawMeasurement(gl, measurement, glFontRenderOptions, fontBitmapEntries, xSpan, ySpan, fitRect, offsetSize, pixelPermm);

		public static void DrawTextOnTexture(OpenGL gl, string text, float texCoordX, float texCoordY, Size glControlSize) => OpenGlMeasurementRenderer.DrawTextOnTexture(gl, text, texCoordX, texCoordY, glControlSize);


		#endregion

		#region OpenGlOverlayRenderer
		public static void CompileOverlayShape(OpenGL gl, CanvasOverlayItem newObject) => OpenGlOverlayRenderer.CompileOverlayShape(gl, newObject);

		public static void SetStencile(OpenGL gl) => OpenGlOverlayRenderer.SetStencile(gl);


		#endregion

		#region OpenGlOverlayTextRenderer
		public static void DrawGroupName(OpenGL gl, CanvasOverlayManager overlayManager, OpenGlTextDrawOptions glDrawTextOptions) => OpenGlOverlayTextRenderer.DrawGroupName(gl, overlayManager, glDrawTextOptions);

		public static void DrawRoiItemName(OpenGL gl, CanvasOverlayManager overlayManager, OpenGlTextDrawOptions glDrawTextOptions) => OpenGlOverlayTextRenderer.DrawRoiItemName(gl, overlayManager, glDrawTextOptions);

		public static uint CreateTextTexture(OpenGL gl, string text, Font font, System.Drawing.Color textColor) => OpenGlOverlayTextRenderer.CreateTextTexture(gl, text, font, textColor);

		#endregion
	}
}
