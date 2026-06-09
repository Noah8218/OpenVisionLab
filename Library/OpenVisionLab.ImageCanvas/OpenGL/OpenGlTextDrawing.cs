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
	{		public static void DrawText(OpenGL gl, List<OpenGlFontBitmapEntry> fontBitmapEntries, float xSpan, float ySpan, SizeF offsetSize, float x, float y, System.Drawing.Color color, string faceName, float baseFontSize, string text)
		{
			float r, g, b, a = 1.0f;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);


			var result = (from fbe in fontBitmapEntries
						  where fbe.HDC == gl.RenderContextProvider.DeviceContextHandle
						  && fbe.HRC == gl.RenderContextProvider.RenderContextHandle
						  && String.Compare(fbe.FaceName, faceName, StringComparison.OrdinalIgnoreCase) == 0
						  && fbe.Height == baseFontSize
						  select fbe).ToList();

			var fontBitmapEntry = result.FirstOrDefault();

			if (fontBitmapEntry == null)
				fontBitmapEntry = CreateOpenGlFontBitmapEntry(gl, fontBitmapEntries, faceName, (int)baseFontSize);

			double width = gl.RenderContextProvider.Width;
			double height = gl.RenderContextProvider.Height;

			//  Create the appropriate projection matrix.
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.PushMatrix();
			gl.LoadIdentity();

			gl.Ortho2D(0, xSpan, 0, ySpan);

			gl.Translate(offsetSize.Width, offsetSize.Height, -0f);            // Move Left And Into The Screen

			//  Create the appropriate modelview matrix.
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.PushMatrix();
			gl.LoadIdentity();
			gl.Color(r, g, b);

			gl.PushAttrib(OpenGL.GL_LIST_BIT | OpenGL.GL_CURRENT_BIT | OpenGL.GL_ENABLE_BIT | OpenGL.GL_TRANSFORM_BIT);
			gl.Color(r, g, b);
			gl.Disable(OpenGL.GL_LIGHTING);
			gl.Disable(OpenGL.GL_TEXTURE_2D);
			gl.Disable(OpenGL.GL_DEPTH_TEST);
			gl.RasterPos(x, y);

			//  Set the list base.
			gl.ListBase(fontBitmapEntry.ListBase);

			//  Create an array of lists for the glyphs.
			var lists = text.Select(c => (byte)c).ToArray();

			//  Call the lists for the string.
			gl.CallLists(lists.Length, lists);
			//gl.Flush();

			//  Reset the list bit.
			gl.PopAttrib();

			//  Pop the modelview.
			gl.PopMatrix();

			//  back to the projection and pop it, then back to the model view.
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.PopMatrix();
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
		}

		public static void DrawTextAt(OpenGL gl, List<OpenGlFontBitmapEntry> fontBitmapEntries,
							  string text, float x, float y, int fontSize, System.Drawing.Color color, bool originTop = true)
		{
			if (string.IsNullOrEmpty(text)) return;

			int[] viewport = new int[4];
			gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
			int screenWidth = viewport[2];
			int screenHeight = viewport[3];

			var fontBitmapEntry = fontBitmapEntries.FirstOrDefault(fbe =>
						  fbe.HDC == gl.RenderContextProvider.DeviceContextHandle
						  && fbe.HRC == gl.RenderContextProvider.RenderContextHandle
						  && String.Compare(fbe.FaceName, "Arial", StringComparison.OrdinalIgnoreCase) == 0
						  && fbe.Height == fontSize);

			if (fontBitmapEntry == null)
				fontBitmapEntry = CreateOpenGlFontBitmapEntry(gl, fontBitmapEntries, "Arial", fontSize);

			gl.PushAttrib(OpenGL.GL_ALL_ATTRIB_BITS);

			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.PushMatrix();
			gl.LoadIdentity();

			if (originTop)
				gl.Ortho2D(0, screenWidth, screenHeight, 0);
			else
				gl.Ortho2D(0, screenWidth, 0, screenHeight);

			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.PushMatrix();
			gl.LoadIdentity();

			gl.Disable(OpenGL.GL_LIGHTING);
			gl.Disable(OpenGL.GL_TEXTURE_2D);
			gl.Disable(OpenGL.GL_DEPTH_TEST);
			gl.Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

			gl.RasterPos(x, y);

			gl.ListBase(fontBitmapEntry.ListBase);
			var lists = text.Select(c => (byte)c).ToArray();
			gl.CallLists(lists.Length, lists);

			// 9. 복구
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.PopMatrix();
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.PopMatrix();
			gl.PopAttrib();
		}

		public static void DrawFixedText(OpenGL gl, List<OpenGlFontBitmapEntry> fontBitmapEntries, float x, float y, SizeF offsetSize, System.Drawing.Color color, string faceName, float fontSize, string text)
		{
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			gl.DrawText((int)0, (int)0, r, g, b, faceName, fontSize, "");

			var fontHeight = (int)(fontSize * (16.0f / 12.0f));
			var fontBitmapEntry = fontBitmapEntries.FirstOrDefault(fbe =>
				fbe.HDC == gl.RenderContextProvider.DeviceContextHandle &&
				fbe.HRC == gl.RenderContextProvider.RenderContextHandle &&
				string.Equals(fbe.FaceName, faceName, StringComparison.OrdinalIgnoreCase) &&
				fbe.Height == fontHeight);

			if (fontBitmapEntry == null)
				fontBitmapEntry = CreateOpenGlFontBitmapEntry(gl, fontBitmapEntries, faceName, fontHeight);

			int[] viewport = new int[4];
			gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
			float screenWidth = viewport[2];
			float screenHeight = viewport[3];

			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.PushMatrix();
			gl.LoadIdentity();
			gl.Ortho2D(0, offsetSize.Width, 0, offsetSize.Height);

			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.PushMatrix();
			gl.LoadIdentity();
			gl.Color(r, g, b);
			gl.RasterPos(x, screenHeight - y);

			gl.ListBase(fontBitmapEntry.ListBase);
			var lists = text.Select(c => (byte)c).ToArray();
			gl.CallLists(lists.Length, lists);

			gl.PopMatrix();
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.PopMatrix();
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
		}

		public static OpenGlFontBitmapEntry CreateOpenGlFontBitmapEntry(OpenGL gl, List<OpenGlFontBitmapEntry> fontBitmapEntries, string faceName, int height)
		{

			//  Make the OpenGL instance current.
			gl.MakeCurrent();

			//  Create the font based on the face name.
			var hFont = Win32.CreateFont(height, 0, 0, 0, Win32.FW_DONTCARE, 0, 0, 0, Win32.DEFAULT_CHARSET,
				 Win32.OUT_OUTLINE_PRECIS, Win32.CLIP_DEFAULT_PRECIS, Win32.CLEARTYPE_QUALITY, Win32.VARIABLE_PITCH, faceName);

			//  Select the font handle.
			var hOldObject = Win32.SelectObject(gl.RenderContextProvider.DeviceContextHandle, hFont);

			//  Create the list base.
			var listBase = gl.GenLists(1);

			//  Create the font bitmaps.

			bool ok = TryUseFontBitmapsWithRetry(gl, gl.RenderContextProvider.DeviceContextHandle, listBase);

			//  Reselect the old font.
			Win32.SelectObject(gl.RenderContextProvider.DeviceContextHandle, hOldObject);

			//  Free the font.
			Win32.DeleteObject(hFont);

			//  Create the font bitmap entry.
			var fbe = new OpenGlFontBitmapEntry()
			{
				HDC = gl.RenderContextProvider.DeviceContextHandle,
				HRC = gl.RenderContextProvider.RenderContextHandle,
				FaceName = faceName,
				Height = height,
				ListBase = listBase,
				ListCount = 255
			};

			//  Add the font bitmap entry to the internal list.
			fontBitmapEntries.Add(fbe);

			return fbe;
		}

		private static bool TryUseFontBitmapsWithRetry(OpenGL gl, IntPtr hdc, uint listBase)
		{
			const int MaxRetry = 3;

			for (int i = 0; i < MaxRetry; i++)
			{
				gl.MakeCurrent();

				gl.Finish();

				if (Win32.wglUseFontBitmaps(hdc, 0, 256, listBase))
					return true;

				System.Threading.Thread.Sleep(0);
			}

			return false;
		}

		public static void DrawTextOnStaticPosition(OpenGL gl, float zoomScale, int x, int y, float r, float g, float b, string faceName, float fontSize, string text)
		{
			gl.DrawText(x, y, r, g, b, faceName, fontSize / zoomScale, text);
		}

		public static void DrawText(OpenGL gl, int x, int y, string text)
		{
			//var LocationX = (x + _offsetSize.Width) / _zoom * GetControlMinSize();
			//var LocationY = -1 * (_offsetSize.Height - y) / _zoom * GetControlMinSize();
			//gl.DrawText((int)LocationX, (int)LocationY, 0.0f, 256.0f, 0.0f, "Arial", 10, text);
		}
	}
}
