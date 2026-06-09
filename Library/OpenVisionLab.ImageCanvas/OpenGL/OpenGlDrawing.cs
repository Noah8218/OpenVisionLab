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

		private static float minHandleSize = 5;
		private static float maxHandleSize = 30;

		public static float ZoomFactor = 1.0f;

		/// <summary>
		/// Draws the text.
		/// </summary>
		/// <param name="gl">The gl.</param>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <param name="r">The r.</param>
		/// <param name="g">The g.</param>
		/// <param name="b">The b.</param>
		/// <param name="faceName">Name of the face.</param>
		/// <param name="fontSize">Size of the font.</param>
		/// <param name="text">The text.</param>
		/// <summary>
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="fontBitmapEntries"></param>
		/// <param name="xSpan"></param>
		/// <param name="ySpan"></param>
		/// <param name="fitRect"></param>
		/// <param name="color"></param>
		/// <param name="faceName"></param>
		/// <param name="fontSize"></param>
		/// <param name="text"></param>
		//public static void DrawViewerBaseText(OpenGL gl, float x, float y, System.Drawing.Color color, string faceName, float baseFontSize, string text)
		//{
		//	float r, g, b, a;
		//	(r, g, b, a) = ConvertColorToOpenGLRGB(color);

		//	int[] viewport = new int[4];
		//	gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
		//	float screenWidth = viewport[2];
		//	float screenHeight = viewport[3];

		//	gl.MatrixMode(OpenGL.GL_PROJECTION);

		//	gl.MatrixMode(OpenGL.GL_MODELVIEW);


		//	gl.MatrixMode(OpenGL.GL_PROJECTION);
		//	gl.MatrixMode(OpenGL.GL_MODELVIEW);
		//}

		/// <summary>
		/// </summary>
		/// <param name="gl">SharpGL 객체</param>
		/// <summary>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="text"></param>
		/// <summary>
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="mainRectPoints"></param>
		/// <param name="handleSize"></param>
		/// <param name="lineWidth"></param>
		/// <param name="lineColorRGB"></param>
		//public static void DrawRectangle(OpenGL gl, RectangleF rect, float lineWidth, System.Windows.Media.SolidColorBrush color)
		//{
		//	if (rect.IsEmpty) { return; }


		//	PointF topLeft = new PointF(rect.Left, rect.Top);
		//	PointF topRight = new PointF(rect.Right, rect.Top);
		//	PointF bottomRight = new PointF(rect.Right, rect.Bottom);
		//	PointF bottomLeft = new PointF(rect.Left, rect.Bottom);

		//	gl.LineWidth(lineWidth);
		//	gl.Color(r, g, b);

		//	gl.Begin(OpenGL.GL_LINE_LOOP);
		//	{
		//	}
		//	gl.End();
		//}
		//public static void DrawShape(OpenGL gl, CanvasShape shape, System.Drawing.Color color, bool isDotted, bool isFill, float lineWidth = 1.0f)
		//{
		//	SetShapeColorAndStyle(gl, shape, color, isDotted, lineWidth);


		//	foreach (var segment in segments)
		//	{
		//		if (isFill)
		//		{
		//			foreach (var dot in segment)
		//			{
		//				gl.Vertex(dot.X, dot.Y);
		//			}
		//			gl.End();
		//		}
		//		else
		//		{
		//			PointF? previousPoint = null;
		//			foreach (var currentPoint in segment)
		//			{
		//				if (previousPoint != null)
		//				{
		//					DrawThickLine(gl, (PointF)previousPoint, currentPoint, lineWidth, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B)));
		//				}
		//				previousPoint = currentPoint;
		//			}
		//		}
		//	}
		//}

		//public static void DrawShape(OpenGL gl, CanvasShape shape, System.Drawing.Color color, bool isDotted, bool isFill, float lineWidth = 1.0f)
		//{
		//	SetShapeColorAndStyle(gl, shape, color, isDotted, lineWidth);

		//	if (isFill)
		//	{
		//		var dots = shape.ShapePoints.ToArray();
		//		float r, g, b;
		//		foreach (var dot in dots)
		//		{
		//			if (dot != null)
		//			{
		//				DrawPointAsSquare(gl, new PointF(dot.X, dot.Y), lineWidth, r, g, b);
		//			}
		//		}
		//	}
		//	else
		//	{

		//		foreach (var segment in segments)
		//		{
		//			PointF? previousPoint = null;
		//			foreach (var currentPoint in segment)
		//			{
		//				if (previousPoint != null)
		//				{
		//					DrawThickLine(gl, (PointF)previousPoint, currentPoint, lineWidth, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B)));
		//				}
		//				previousPoint = currentPoint;
		//			}
		//		}
		//	}
		//}
		//private static List<PointF> GeneratePointList(PointF startPoint, PointF endPoint, float interval)
		//{
		//	List<PointF> pointList = new List<PointF>();

		//	int distanceX = (int)endPoint.X - (int)startPoint.X;
		//	int distanceY = (int)endPoint.Y - (int)startPoint.Y;

		//	float totalDistance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

		//	if (totalDistance > 0)
		//		for (float t = 0; t <= 1; t += 1 / totalDistance)
		//		{
		//			int newX = (int)(startPoint.X + t * distanceX);
		//			int newY = (int)(startPoint.Y + t * distanceY);
		//			pointList.Add(new PointF(newX, newY));
		//		}
		//	}
		//	Console.WriteLine($"{totalDistance}, {pointList.Count}");

		//	return pointList;
		//}
		//public static void DrawThickCircle(OpenGL gl, float centerX, float centerY, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode, SizeF textureSize)
		//{
		//	gl.Enable(OpenGL.GL_BLEND);
		//	gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

		//	float r, g, b, a;

		//	gl.Enable(OpenGL.GL_STENCIL_TEST);
		//	gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

		//	if (enumFillMode == EnumFillMode.InFill)
		//	{
		//		gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
		//		gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

		//		FillCirclePoints(gl, (int)centerX, (int)centerY, (int)radius, r, g, b, a);
		//	}
		//	else if (enumFillMode == EnumFillMode.OutFill)
		//	{
		//		gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
		//		gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);


		//		gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_KEEP);

		//		gl.Color(r, g, b, a);

		//		gl.Begin(OpenGL.GL_QUADS);
		//		gl.Vertex(0, 0);
		//		gl.Vertex(textureSize.Width, 0);
		//		gl.Vertex(textureSize.Width, textureSize.Height);
		//		gl.Vertex(0, textureSize.Height);
		//		gl.End();
		//	}
		//	else if (enumFillMode == EnumFillMode.None)
		//	{
		//		int x1 = 0;
		//		int y2 = (int)radius;
		//		int d = 3 - 2 * (int)radius;

		//		while (y2 >= x1)
		//		{
		//			DrawCirclePoints(gl, (int)centerX, (int)centerY, x1, y2, lineWidth, r, g, b, a);

		//			x1++;
		//			if (d > 0)
		//			{
		//				y2--;
		//				d = d + 4 * (x1 - y2) + 10;
		//			}
		//			else
		//			{
		//				d = d + 4 * x1 + 6;
		//			}

		//			DrawCirclePoints(gl, (int)centerX, (int)centerY, x1, y2, lineWidth, r, g, b, a);
		//		}
		//	}

		//	gl.Disable(OpenGL.GL_STENCIL_TEST);
		//	gl.Disable(OpenGL.GL_BLEND);
		//}
		//public static void DrawThickCircle(OpenGL gl, float centerX, float centerY, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, bool isFill = false)
		//{
		//	gl.Enable(OpenGL.GL_BLEND);
		//	gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

		//	float r, g, b, a;
		//	int x1 = 0;
		//	int y2 = (int)radius;
		//	int d = 3 - 2 * (int)radius;

		//	void DrawCirclePoints(int cx, int cy, int x, int y)
		//	{
		//		DrawPointAsSquare(gl, new PointF(cx + x, cy + y), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx - x, cy + y), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx + x, cy - y), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx - x, cy - y), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx + y, cy + x), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx - y, cy + x), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx + y, cy - x), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx - y, cy - x), lineWidth, r, g, b, a);
		//	}

		//	void FillCirclePoints(int cx, int cy, int x, int y)
		//	{
		//		for (int i = cx - x; i <= cx + x; i++)
		//		{
		//			DrawPointAsSquare(gl, new PointF(i, cy + y), lineWidth, r, g, b, a);
		//			DrawPointAsSquare(gl, new PointF(i, cy - y), lineWidth, r, g, b, a);
		//		}
		//		for (int i = cx - y; i <= cx + y; i++)
		//		{
		//			DrawPointAsSquare(gl, new PointF(i, cy + x), lineWidth, r, g, b, a);
		//			DrawPointAsSquare(gl, new PointF(i, cy - x), lineWidth, r, g, b, a);
		//		}
		//	}

		//	while (y2 >= x1)
		//	{
		//		if (isFill)
		//		{
		//			FillCirclePoints((int)centerX, (int)centerY, x1, y2);
		//		}
		//		else
		//		{
		//			DrawCirclePoints((int)centerX, (int)centerY, x1, y2);
		//		}

		//		x1++;

		//		if (d > 0)
		//		{
		//			y2--;
		//			d = d + 4 * (x1 - y2) + 10;
		//		}
		//		else
		//		{
		//			d = d + 4 * x1 + 6;
		//		}

		//		if (isFill)
		//		{
		//			FillCirclePoints((int)centerX, (int)centerY, x1, y2);
		//		}
		//		else
		//		{
		//			DrawCirclePoints((int)centerX, (int)centerY, x1, y2);
		//		}

		//	}

		//	gl.Disable(OpenGL.GL_BLEND);
		//}


		//public static void DrawThickCircle(OpenGL gl, float centerX, float centerY, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color)
		//{
		//	gl.Enable(OpenGL.GL_BLEND);
		//	gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

		//	float r, g, b, a;
		//	int x1 = 0;
		//	int y2 = (int)radius;
		//	int d = 3 - 2 * (int)radius;

		//	void DrawCirclePoints(int cx, int cy, int x, int y)
		//	{
		//		DrawPointAsSquare(gl, new PointF(cx + x, cy + y), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx - x, cy + y), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx + x, cy - y), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx - x, cy - y), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx + y, cy + x), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx - y, cy + x), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx + y, cy - x), lineWidth, r, g, b, a);
		//		DrawPointAsSquare(gl, new PointF(cx - y, cy - x), lineWidth, r, g, b, a);
		//	}

		//	while (y2 >= x1)
		//	{
		//		DrawCirclePoints((int)centerX, (int)centerY, x1, y2);
		//		x1++;

		//		if (d > 0)
		//		{
		//			y2--;
		//			d = d + 4 * (x1 - y2) + 10;
		//		}
		//		else
		//		{
		//			d = d + 4 * x1 + 6;
		//		}

		//		DrawCirclePoints((int)centerX, (int)centerY, x1, y2);
		//	}

		//	gl.Disable(OpenGL.GL_BLEND);
		//}
		//public static void DrawWithPen(OpenGL gl, List<PointF> points, float lineWidth, System.Windows.Media.SolidColorBrush color)
		//{
		//	for (int i = 0; i < points.Count; i++)
		//	{
		//		DrawPointAsSquare(gl, new PointF(points[i].X, points[i].Y), lineWidth, color);
		//	}
		//}
		/// <summary>
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="measurement"></param>
		//private static uint shaderProgram;
		//public static bool tt = false;

		//public static void InitializeShader(OpenGL gl)
		//{
		//	string vertexShaderSource = "#version 120\n" +
		//								"attribute vec3 position;\n" +
		//								"attribute vec2 texcoord;\n" +
		//								"varying vec2 TexCoord;\n" +
		//								"void main()\n" +
		//								"{\n" +
		//								"    gl_Position = gl_ModelViewProjectionMatrix * vec4(position, 1.0);\n" +
		//								"    TexCoord = texcoord;\n" +
		//								"}\n";

		//	string fragmentShaderSource = "#version 120\n" +
		//								  "uniform sampler2D maskTexture;\n" +
		//								  "uniform float alphaFactor;\n" +
		//								  "varying vec2 TexCoord;\n" +
		//								  "void main()\n" +
		//								  "{\n" +
		//								  "    vec4 maskColor = texture2D(maskTexture, TexCoord);\n" +
		//								  "    if (abs(maskColor.r - 1.0) < 0.01 && abs(maskColor.g - 1.0) < 0.01 && abs(maskColor.b - 1.0) < 0.01)\n" +
		//								  "    {\n" +
		//								  "    }\n" +
		//								  "    else if (abs(maskColor.r - 1.0) < 0.01 && abs(maskColor.g) < 0.01 && abs(maskColor.b) < 0.01)\n" +
		//								  "    {\n" +
		//								  "    }\n" +
		//								  "    else\n" +
		//								  "    {\n" +
		//								  "    }\n" +
		//								  "}\n";

		//	uint vertexShader = gl.CreateShader(OpenGL.GL_VERTEX_SHADER);
		//	gl.ShaderSource(vertexShader, vertexShaderSource);
		//	gl.CompileShader(vertexShader);

		//	uint fragmentShader = gl.CreateShader(OpenGL.GL_FRAGMENT_SHADER);
		//	gl.ShaderSource(fragmentShader, fragmentShaderSource);
		//	gl.CompileShader(fragmentShader);

		//	shaderProgram = gl.CreateProgram();
		//	gl.AttachShader(shaderProgram, vertexShader);
		//	gl.AttachShader(shaderProgram, fragmentShader);
		//	gl.LinkProgram(shaderProgram);

		//	int[] status = new int[1];
		//	gl.GetShader(vertexShader, OpenGL.GL_COMPILE_STATUS, status);
		//	if (status[0] == 0)
		//	{
		//		StringBuilder infoLog = new StringBuilder(512);
		//		gl.GetShaderInfoLog(vertexShader, 512, IntPtr.Zero, infoLog);
		//		Console.WriteLine("Vertex Shader Compile Error: " + infoLog.ToString());
		//	}

		//	gl.GetShader(fragmentShader, OpenGL.GL_COMPILE_STATUS, status);
		//	if (status[0] == 0)
		//	{
		//		StringBuilder infoLog = new StringBuilder(512);
		//		gl.GetShaderInfoLog(fragmentShader, 512, IntPtr.Zero, infoLog);
		//		Console.WriteLine("Fragment Shader Compile Error: " + infoLog.ToString());
		//	}

		//	gl.GetProgram(shaderProgram, OpenGL.GL_LINK_STATUS, status);
		//	if (status[0] == 0)
		//	{
		//		StringBuilder infoLog = new StringBuilder(512);
		//		gl.GetProgramInfoLog(shaderProgram, 512, IntPtr.Zero, infoLog);
		//		Console.WriteLine("Program Link Error: " + infoLog.ToString());
		//	}
		//}








		//public static void DrawODBTexture(OpenGL gl, ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>> textureAreas, List<string> order)
		//{
		//	if (tt == false)
		//	{
		//		tt = true;
		//	}

		//	gl.Enable(OpenGL.GL_TEXTURE_2D);
		//	gl.Enable(OpenGL.GL_BLEND);

		//	foreach (var key in order.AsEnumerable().Reverse())
		//	{
		//		if (textureAreas.TryGetValue(key, out var drawingParams))
		//		{
		//			var groupedByImageName = drawingParams.GroupBy(param => param.ImageName);

		//			foreach (var group in groupedByImageName)
		//			{
		//				float minX = group.Min(param => param.GLDrawingTextureArea.Left);
		//				float maxX = group.Max(param => param.GLDrawingTextureArea.Right);
		//				float minY = group.Min(param => param.GLDrawingTextureArea.Top);
		//				float maxY = group.Max(param => param.GLDrawingTextureArea.Bottom);
		//				float centerX = (minX + maxX) / 2;
		//				float centerY = (minY + maxY) / 2;

		//				foreach (OpenGlTextureDrawingParam param in group)
		//				{
		//					if (param.IsVisible)
		//					{
		//						if (param.IsRotated)
		//						{
		//							gl.Translate(centerX, centerY, 0);
		//							gl.Rotate(param.RotationAngle, 0, 0, 1);
		//							gl.Translate(-centerX, -centerY, 0);
		//						}

		//						gl.BindTexture(OpenGL.GL_TEXTURE_2D, param.OriTextureId);

		//						gl.UseProgram(shaderProgram);

		//						int alphaFactorLocation = gl.GetUniformLocation(shaderProgram, "alphaFactor");

		//						gl.ActiveTexture(OpenGL.GL_TEXTURE0);
		//						gl.BindTexture(OpenGL.GL_TEXTURE_2D, param.OriBackgroundTextureId);

		//						DrawQuad(gl, param.GLDrawingTextureArea);

		//						gl.UseProgram(0);

		//						if (param.IsRotated)
		//						{
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}

		//	gl.Disable(OpenGL.GL_BLEND);
		//	gl.Disable(OpenGL.GL_TEXTURE_2D);
		//}
		/// <summary>
		///
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="cx">Center X of Rectangle</param>
		/// <param name="cy">Center Y of Rectangle</param>
		/// <param name="rx">Half of Rectangle Width</param>
		/// <param name="ry">Half of Rectangle Height</param>
		/// <param name="lineWidth"></param>
		/// <param name="lineColorRGB"></param>
		/// <summary>
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="newObject"></param>
		/// <summary>
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="textureId"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		//private static void DrawDirectlyToTexture(OpenGL gl, uint textureId, int width, int height)
		//{
		//	gl.Viewport(0, 0, width, height);

		//	gl.MatrixMode(OpenGL.GL_PROJECTION);
		//	gl.LoadIdentity();

		//	gl.MatrixMode(OpenGL.GL_MODELVIEW);
		//	gl.LoadIdentity();

		//	uint[] frameBuffer = new uint[1];
		//	gl.GenFramebuffersEXT(1, frameBuffer);
		//	gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBuffer[0]);
		//	gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

		//	uint[] renderBuffer = new uint[1];
		//	gl.GenRenderbuffersEXT(1, renderBuffer);
		//	gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);
		//	gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_STENCIL_INDEX8_EXT, width, height);

		//	gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_STENCIL_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT,
		//	Buffer[0]);


		//	gl.Enable(OpenGL.GL_STENCIL_TEST);

		//	gl.ClearStencil(0);
		//	gl.Clear(OpenGL.GL_STENCIL_BUFFER_BIT);
		//	gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
		//	gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

		//	gl.Color(System.Drawing.Color.Yellow.R / 255.0f, System.Drawing.Color.Yellow.G / 255.0f, System.Drawing.Color.Yellow.B / 255.0f);
		//	gl.Begin(OpenGL.GL_QUADS);
		//	gl.Vertex(50, 50);
		//	gl.Vertex(50, 60);
		//	gl.Vertex(60, 60);
		//	gl.Vertex(60, 50);
		//	gl.End();

		//	gl.ColorMask(1, 1, 1, 1);
		//	gl.StencilFunc(OpenGL.GL_NOTEQUAL, 1, 0xFF);
		//	gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_KEEP);

		//	gl.Color(System.Drawing.Color.Red.R / 255.0f, System.Drawing.Color.Red.G / 255.0f, System.Drawing.Color.Red.B / 255.0f);
		//	gl.Begin(OpenGL.GL_QUADS);
		//	gl.Vertex(0, 0);
		//	gl.Vertex(0, 100);
		//	gl.Vertex(100, 100);
		//	gl.Vertex(100, 0);
		//	gl.End();

		//	gl.Disable(OpenGL.GL_STENCIL_TEST);

		//	gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
		//	gl.DeleteFramebuffersEXT(1, frameBuffer);
		//}
		//private static List<PointF> ConvertDotInfoToPoints(List<DotInfo> listDot)
		//{
		//	List<PointF> listPoints = new List<PointF>();
		//	foreach (DotInfo dot in listDot)
		//	{
		//		listPoints.Add(new PointF(dot.X, dot.Y));
		//	}
		//	return listPoints;
		//}
	}
}
