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
	public static class OpenGlDrawing
	{

		private static float minHandleSize = 5; // 최소 ?�들 ?�기
		private static float maxHandleSize = 30; // 최�? ?�들 ?�기

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
		public static void DrawText(OpenGL gl, List<OpenGlFontBitmapEntry> fontBitmapEntries, float xSpan, float ySpan, SizeF offsetSize, float x, float y, System.Drawing.Color color, string faceName, float baseFontSize, string text)
		{
			float r, g, b, a = 1.0f; // 빨간??
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


		/// <summary>
		/// View 기�??�서 Text Drawing
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="fontBitmapEntries"></param>
		/// <param name="xSpan"></param>
		/// <param name="ySpan"></param>
		/// <param name="fitRect"></param>
		/// <param name="x">View 기�? ?��? ?�치, 좌하 기�? </param>
		/// <param name="y">View 기�? ?��? ?�치, 좌하 기�?</param>
		/// <param name="color"></param>
		/// <param name="faceName"></param>
		/// <param name="fontSize"></param>
		/// <param name="text"></param>
		//public static void DrawViewerBaseText(OpenGL gl, float x, float y, System.Drawing.Color color, string faceName, float baseFontSize, string text)
		//{
		//	// ?�상??OpenGL?�서 ?�용?????�는 RGB 값으�?변??
		//	float r, g, b, a;
		//	(r, g, b, a) = ConvertColorToOpenGLRGB(color);

		//	// ?�재 뷰포???�기�?가?�옴
		//	int[] viewport = new int[4];
		//	gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
		//	float screenWidth = viewport[2];
		//	float screenHeight = viewport[3];

		//	// Orthographic ?�영 ?�정
		//	gl.MatrixMode(OpenGL.GL_PROJECTION);
		//	gl.PushMatrix(); // ?�재 ?�영 매트�?�� ?�??
		//	gl.LoadIdentity(); // ?�영 매트�?���??�위 ?�렬�?초기??
		//	gl.Ortho2D(0, screenWidth, 0, screenHeight); // ?�면 ?�기??맞는 Orthographic ?�영 ?�정

		//	// 모델�?매트�?�� 초기??
		//	gl.MatrixMode(OpenGL.GL_MODELVIEW);
		//	gl.PushMatrix(); // ?�재 모델�?매트�?�� ?�??
		//	gl.LoadIdentity(); // 모델�?매트�?���??�위 ?�렬�?초기??

		//	// ?�스??그리�?
		//	gl.DrawText((int)x, (int)(screenHeight - y - baseFontSize), r, g, b, faceName, baseFontSize, text); // Y ?�치�??�집?�서 ?�단 기�??�로 조정

		//	// 매트�?�� 복원
		//	gl.PopMatrix(); // 모델�?매트�?�� 복원
		//	gl.MatrixMode(OpenGL.GL_PROJECTION);
		//	gl.PopMatrix(); // ?�영 매트�?�� 복원
		//	gl.MatrixMode(OpenGL.GL_MODELVIEW);
		//}

		/// <summary>
		/// 지?�된 뷰어 좌표???�스?��? ?�로?�합?�다.
		/// </summary>
		/// <param name="gl">SharpGL 객체</param>
		/// <param name="text">출력??문자??/param>
		/// <param name="x">X 좌표 (?�쪽 0)</param>
		/// <param name="y">Y 좌표 (기본�? ?�쪽 ?�단 0 기�?)</param>
		/// <param name="fontSize">?�트 ?�기</param>
		/// <param name="color">?�스???�상</param>
		/// <param name="isBottomLeftOrigin">true?�면 ?�쪽 ?�단??(0,0)?�로 처리, false?�면 ?�쪽 ?�단??(0,0)?�로 처리</param>
		public static void DrawTextAt(OpenGL gl, List<OpenGlFontBitmapEntry> fontBitmapEntries,
							  string text, float x, float y, int fontSize, System.Drawing.Color color, bool originTop = true)
		{
			if (string.IsNullOrEmpty(text)) return;

			// 1. ?�재 뷰포???�제 컨트롤의 ?��? ?�기) 가?�오�?
			int[] viewport = new int[4];
			gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
			int screenWidth = viewport[2];  // 컨트롤의 ?�제 ?�비(px)
			int screenHeight = viewport[3]; // 컨트롤의 ?�제 ?�이(px)

			// 2. ?�트 ?�트�??�인
			var fontBitmapEntry = fontBitmapEntries.FirstOrDefault(fbe =>
						  fbe.HDC == gl.RenderContextProvider.DeviceContextHandle
						  && fbe.HRC == gl.RenderContextProvider.RenderContextHandle
						  && String.Compare(fbe.FaceName, "Arial", StringComparison.OrdinalIgnoreCase) == 0
						  && fbe.Height == fontSize);

			if (fontBitmapEntry == null)
				fontBitmapEntry = CreateOpenGlFontBitmapEntry(gl, fontBitmapEntries, "Arial", fontSize);

			// 3. ?�태 백업
			gl.PushAttrib(OpenGL.GL_ALL_ATTRIB_BITS);

			// 4. ?�영 ?�렬 ?�정 (?��?지 ?�기가 ?�닌, ?�제 ?�면 ?��? ?�기�??�정)
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.PushMatrix();
			gl.LoadIdentity();

			// 0부???�제 ?�면 ?��? ?�비/?�이까�?�?좌표�??�습?�다.
			if (originTop)
				gl.Ortho2D(0, screenWidth, screenHeight, 0); // 0???�면 �????��?
			else
				gl.Ortho2D(0, screenWidth, 0, screenHeight); // 0???�면 �??�래 ?��?

			// 5. 모델�??�렬 ?�정 (Translate�??��? ?��? ?�음)
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.PushMatrix();
			gl.LoadIdentity();

			// 6. ?�더�??�태 ?�정
			gl.Disable(OpenGL.GL_LIGHTING);
			gl.Disable(OpenGL.GL_TEXTURE_2D);
			gl.Disable(OpenGL.GL_DEPTH_TEST);
			gl.Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

			// 7. 출력 ?�치 지??
			// x, y???�제 ?�제 ?�면???��? 좌표?�니??
			// ?? x=10, y=30 ?�면 ?�면 ?�쪽 ??구석?�서 10, 30?��? ?�어�?곳입?�다.
			gl.RasterPos(x, y);

			// 8. ?�스??출력
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
			float r, g, b, a; // ?�상 초기??
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

			gl.DrawText((int)0, (int)0, r, g, b, faceName, fontSize, "");

			// ?�트 ?�기 �?비트�??�트�?검??
			var fontHeight = (int)(fontSize * (16.0f / 12.0f));
			var fontBitmapEntry = fontBitmapEntries.FirstOrDefault(fbe =>
				fbe.HDC == gl.RenderContextProvider.DeviceContextHandle &&
				fbe.HRC == gl.RenderContextProvider.RenderContextHandle &&
				string.Equals(fbe.FaceName, faceName, StringComparison.OrdinalIgnoreCase) &&
				fbe.Height == fontHeight);

			if (fontBitmapEntry == null)
				fontBitmapEntry = CreateOpenGlFontBitmapEntry(gl, fontBitmapEntries, faceName, fontHeight);

			// 뷰포???�기 가?�오�?
			int[] viewport = new int[4];
			gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
			float screenWidth = viewport[2];
			float screenHeight = viewport[3];

			// ?�영 매트�?�� ?�정 (?��? 기�?)
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.PushMatrix();
			gl.LoadIdentity();
			gl.Ortho2D(0, offsetSize.Width, 0, offsetSize.Height);

			// 모델�?매트�?�� ?�정 �??�스???�치 지??
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.PushMatrix();
			gl.LoadIdentity();
			gl.Color(r, g, b); // ?�상 ?�정
							   // ?�스???�치 지??(?�면 좌하?�이 (0,0) 기�?)
			gl.RasterPos(x, screenHeight - y); // Y�?반전 ?�요

			// ?�스??그리�?
			gl.ListBase(fontBitmapEntry.ListBase);
			var lists = text.Select(c => (byte)c).ToArray();
			gl.CallLists(lists.Length, lists);

			// 매트�?�� 복구
			gl.PopMatrix(); // 모델�?매트�?�� 복구
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

		/// <summary>
		/// (x,y)?�리??text�?출력?�다.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="text"></param>
		public static void DrawText(OpenGL gl, int x, int y, string text)
		{
			//var LocationX = (x + _offsetSize.Width) / _zoom * GetControlMinSize();
			//var LocationY = -1 * (_offsetSize.Height - y) / _zoom * GetControlMinSize();
			//gl.DrawText((int)LocationX, (int)LocationY, 0.0f, 256.0f, 0.0f, "Arial", 10, text);
		}

		public static void DrawCrossOfImage(OpenGL gl, ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>> textureAreas)
		{
			Action action = delegate
			{
				if (textureAreas.Count == 0)
					return;

				gl.PushAttrib(OpenGL.GL_LINE_BIT);

				// 모든 OpenGlTextureDrawingParam??TextureArea??중에??top, bottom, left, right 값을 계산
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
			// ?�들 ?�기가 최소/최�? ?�기 범위�?벗어?��? ?�도�?조절
			handlesizeResize = Math.Max(minHandleSize, Math.Min(handlesizeResize, maxHandleSize));
			canvasRect.LineWidth = (int)handlesizeResize;
			canvasRect.InitializeHandleRects(handlesizeResize);
			DrawRectangleWithHandles(gl, canvasRect.Points.Select(x => new PointF(x.X, x.Y)), handlesizeResize, 2, new float[] { r, g, b });
		}

		/// <summary>
		/// RoiRect�?Draw?�며, 4개의 �???�과 4개의 변, 그리�?1개의 ?�본 ?�각?�을 Draw
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="mainRectPoints"></param>
		/// <param name="handleSize"></param>
		/// <param name="lineWidth"></param>
		/// <param name="lineColorRGB"></param>
		public static void DrawRectangleWithHandles(OpenGL gl, IEnumerable<PointF> mainRectPoints, float handleSize, float lineWidth, float[] lineColorRGB)
		{
			//float handlesizeResize = (handleSize / _zoomScale);

			// ???�각??그리�?
			DrawStippleLineLoop(gl, mainRectPoints, lineWidth, lineColorRGB);

			// ??변??중심??계산
			var pointsList = mainRectPoints.ToList();
			PointF topCenter = new PointF((pointsList[0].X + pointsList[1].X) / 2, pointsList[0].Y);
			PointF bottomCenter = new PointF((pointsList[2].X + pointsList[3].X) / 2, pointsList[2].Y);
			PointF leftCenter = new PointF(pointsList[0].X, (pointsList[0].Y + pointsList[3].Y) / 2);
			PointF rightCenter = new PointF(pointsList[1].X, (pointsList[1].Y + pointsList[2].Y) / 2);

			// 모든 �???�과 중심?�에 ?��? ?�각??그리�?
			var allPoints = pointsList.Concat(new[] { topCenter, bottomCenter, leftCenter, rightCenter });
			foreach (var center in allPoints)
			{
				// ?��? ?�각?�의 ??�????계산
				float halfSize = handleSize / 2.0f;
				List<PointF> handleRectPoints = new List<PointF>
					{
						new PointF(center.X - halfSize, center.Y - halfSize), // ?�쪽 ?�래
						new PointF(center.X + halfSize, center.Y - halfSize), // ?�른�??�래
						new PointF(center.X + halfSize, center.Y + halfSize), // ?�른�???
						new PointF(center.X - halfSize, center.Y + halfSize)  // ?�쪽 ??
					};

				// ?��? ?�각??그리�?
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

			// ?�단 변

			outlinePoints.AddRange(GetThickLinePoint(topLeft, topRight, lineWidth));
			// ?�단 변
			outlinePoints.AddRange(GetThickLinePoint(bottomLeft, bottomRight, lineWidth));
			// ?�쪽
			outlinePoints.AddRange(GetThickLinePoint(topLeft, bottomLeft, lineWidth));
			// ?�른�?변
			outlinePoints.AddRange(GetThickLinePoint(topRight, bottomRight, lineWidth));

			return outlinePoints;
		}

		public static void DrawRectangle(OpenGL gl, PointF start, PointF end, float lineWidth, EnumFillMode enumFillMode, System.Windows.Media.SolidColorBrush color, SizeF textureSize = new SizeF())
		{
			//if (start.IsEmpty || end.IsEmpty) { return; }
			if (start.Equals(end)) { return; }

			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			// ?�상 변??
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			// ?�각?�의 ??�????계산
			PointF topLeft = new PointF(start.X, start.Y);
			PointF topRight = new PointF(end.X, start.Y);
			PointF bottomRight = new PointF(end.X, end.Y);
			PointF bottomLeft = new PointF(start.X, end.Y);

			// ?�텐??버퍼 ?�정
			gl.Enable(OpenGL.GL_STENCIL_TEST);
			gl.Clear(OpenGL.GL_STENCIL_BUFFER_BIT);
			if (enumFillMode == EnumFillMode.InFill)
			{
				// ?�각???��? 채우�?
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
				// 1. ?�각???��?�??�텐??버퍼??기록
				gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

				gl.Color(0, 0, 0, 0); // ?�상?� ?��??�음 (??경우 ?�제�?그리지 ?�음)
				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(topLeft.X, topLeft.Y);
				gl.Vertex(topRight.X, topRight.Y);
				gl.Vertex(bottomRight.X, bottomRight.Y);
				gl.Vertex(bottomLeft.X, bottomLeft.Y);
				gl.End();

				// 2. ?�각???��?�?채우�?(?�텐??값이 0???�역�?그리�?
				gl.StencilFunc(OpenGL.GL_EQUAL, 0, 0xFF); // ?�텐??값이 0??부분만 그리�?
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_KEEP);

				gl.Color(r, g, b, a);

				// ?�단 ?�역
				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(0, 0);
				gl.Vertex(textureSize.Width, 0);
				gl.Vertex(textureSize.Width, topLeft.Y);
				gl.Vertex(0, topLeft.Y);
				gl.End();

				// ?�단 ?�역
				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(0, bottomRight.Y);
				gl.Vertex(textureSize.Width, bottomRight.Y);
				gl.Vertex(textureSize.Width, textureSize.Height);
				gl.Vertex(0, textureSize.Height);
				gl.End();

				// 좌측 ?�역
				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(0, topLeft.Y);
				gl.Vertex(topLeft.X, topLeft.Y);
				gl.Vertex(topLeft.X, bottomLeft.Y);
				gl.Vertex(0, bottomLeft.Y);
				gl.End();

				// ?�측 ?�역
				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(topRight.X, topRight.Y);
				gl.Vertex(textureSize.Width, topRight.Y);
				gl.Vertex(textureSize.Width, bottomRight.Y);
				gl.Vertex(topRight.X, bottomRight.Y);
				gl.End();
			}

			// ?�곽??그리�?
			gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF); // ?�곽?��? ??�� 그리�?
			gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_KEEP);

			// ?�단 변
			DrawThickLine(gl, topLeft, topRight, lineWidth, color);
			// ?�단 변
			DrawThickLine(gl, bottomLeft, bottomRight, lineWidth, color);
			// ?�쪽 변
			DrawThickLine(gl, topLeft, bottomLeft, lineWidth, color);
			// ?�른�?변
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

			// ?�의 ?�께�?조정?�니?? ?�???�께??경우 0.5�??�합?�다.
			if ((int)lineWidth % 2 != 0)
			{
				startPoint.X += nx;
				startPoint.Y += ny;
				endPoint.X += nx;
				endPoint.Y += ny;
			}

			// �??�에 ?�???��? ?�렬??보정
			PointF p1 = new PointF((int)(startPoint.X + nx), (int)(startPoint.Y + ny));
			PointF p2 = new PointF((int)(startPoint.X - nx), (int)(startPoint.Y - ny));
			PointF p3 = new PointF((int)(endPoint.X - nx), (int)(endPoint.Y - ny));
			PointF p4 = new PointF((int)(endPoint.X + nx), (int)(endPoint.Y + ny));

			// ?�곽 ?�역???�함??모든 ?�인??추�?
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
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

			float dx = endPoint.X - startPoint.X;
			float dy = endPoint.Y - startPoint.Y;
			float length = (float)Math.Sqrt(dx * dx + dy * dy);

			if (length == 0) return; // 길이가 0?�면 그리지 ?�음

			float nx = -dy * lineWidth / length / 2;
			float ny = dx * lineWidth / length / 2;

			// ?�의 ?�께�?조정?�니?? ?�???�께??경우 0.5�??�합?�다.
			if ((int)lineWidth % 2 != 0)
			{
				startPoint.X += nx;
				startPoint.Y += ny;
				endPoint.X += nx;
				endPoint.Y += ny;
			}

			// �??�에 ?�???��? ?�렬??보정
			PointF p1 = new PointF((int)(startPoint.X + nx), (int)(startPoint.Y + ny));
			PointF p2 = new PointF((int)(startPoint.X - nx), (int)(startPoint.Y - ny));
			PointF p3 = new PointF((int)(endPoint.X - nx), (int)(endPoint.Y - ny));
			PointF p4 = new PointF((int)(endPoint.X + nx), (int)(endPoint.Y + ny));

			gl.Color(r, g, b, a); // ?�상 ?�정
			gl.Begin(OpenGL.GL_TRIANGLE_FAN);
			gl.Vertex(p1.X, p1.Y);
			gl.Vertex(p2.X, p2.Y);
			gl.Vertex(p3.X, p3.Y);
			gl.Vertex(p4.X, p4.Y);
			gl.End();
		}

		//public static void DrawRectangle(OpenGL gl, RectangleF rect, float lineWidth, System.Windows.Media.SolidColorBrush color)
		//{
		//	if (rect.IsEmpty) { return; }

		//	float r, g, b; // ?�상 초기??
		//	(r, g, b) = ConvertColorToOpenGLRGB(color); // ?�상 변??

		//	// ?�각?�의 ??�????계산
		//	PointF topLeft = new PointF(rect.Left, rect.Top);
		//	PointF topRight = new PointF(rect.Right, rect.Top);
		//	PointF bottomRight = new PointF(rect.Right, rect.Bottom);
		//	PointF bottomLeft = new PointF(rect.Left, rect.Bottom);

		//	gl.LineWidth(lineWidth);
		//	gl.Color(r, g, b);

		//	gl.Begin(OpenGL.GL_LINE_LOOP);
		//	{
		//		gl.Vertex(topLeft.X, topLeft.Y); // ?�단 ?�쪽
		//		gl.Vertex(topRight.X, topRight.Y); // ?�단 ?�른�?
		//		gl.Vertex(bottomRight.X, bottomRight.Y); // ?�단 ?�른�?
		//		gl.Vertex(bottomLeft.X, bottomLeft.Y); // ?�단 ?�쪽
		//	}
		//	gl.End();
		//}


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


		//public static void DrawShape(OpenGL gl, CanvasShape shape, System.Drawing.Color color, bool isDotted, bool isFill, float lineWidth = 1.0f)
		//{
		//	SetShapeColorAndStyle(gl, shape, color, isDotted, lineWidth);

		//	var segments = SplitIntoSegments(shape.ShapePoints.ToList());  // ?�인??리스?��? ?�그먼트�?분리

		//	foreach (var segment in segments)
		//	{
		//		// Fill ?�션??true??경우 ?��?�?채웁?�다 (?�래??Triangle Fan ?�용)
		//		if (isFill)
		//		{
		//			gl.Begin(OpenGL.GL_TRIANGLE_FAN);  // ?��?�?채우�??�해 POLYGON ?�용
		//			foreach (var dot in segment)
		//			{
		//				gl.Vertex(dot.X, dot.Y);
		//			}
		//			gl.End();
		//		}
		//		else
		//		{
		//			// ?�곽?�에 ?�께 ?�용
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
		//		(r, g, b) = ConvertColorToOpenGLRGB(color); // ?�상 변??
		//		gl.Color(r, g, b); // ?�상 ?�정
		//		foreach (var dot in dots)
		//		{
		//			if (dot != null)
		//			{
		//				// ?�곽?�에 ?�께 ?�용
		//				DrawPointAsSquare(gl, new PointF(dot.X, dot.Y), lineWidth, r, g, b);
		//			}
		//		}
		//	}
		//	else
		//	{
		//		var segments = SplitIntoSegments(shape.ShapePoints.ToList());  // ?�인??리스?��? ?�그먼트�?분리

		//		foreach (var segment in segments)
		//		{
		//			// ?�곽?�에 ?�께 ?�용
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

		public static void DrawPointAsSquare(OpenGL gl, PointF point, float size, System.Windows.Media.SolidColorBrush color)
		{
			float r, g, b, a; // ?�상 초기??
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

			DrawPointAsSquare(gl, point, size, r, g, b, a);
		}

		public static void DrawPointAsSquare(OpenGL gl, PointF point, float size, System.Drawing.Color color)
		{
			float r, g, b, a; // ?�상 초기??
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

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

				// ?�각?�의 �?�????추�?
				gl.Vertex(point.X - halfSize, point.Y - halfSize);
				gl.Vertex(point.X + halfSize, point.Y - halfSize);
				gl.Vertex(point.X + halfSize, point.Y + halfSize);
				gl.Vertex(point.X - halfSize, point.Y + halfSize);
			}

			gl.End();
		}

		public static void DrawFilledPolygon(OpenGL gl, List<System.Drawing.Point> points, System.Drawing.Color fillColor)
		{
			gl.Color(fillColor.R / 255.0f, fillColor.G / 255.0f, fillColor.B / 255.0f); // ?�상 ?�정
			gl.Begin(OpenGL.GL_TRIANGLE_FAN);
			foreach (var point in points)
			{
				gl.Vertex(point.X, point.Y);
			}
			gl.End();
		}


		public static void DrawPointAsSquareBlend(OpenGL gl, PointF point, float size, System.Windows.Media.SolidColorBrush color)
		{
			float r, g, b, a; // ?�상 초기??
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??
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

			// ?�의 ?�께�?조정?�니?? ?�???�께??경우 0.5�??�합?�다.
			if ((int)size % 2 != 0)
			{
				point.X += halfSize;
				point.Y += halfSize;
			}

			// ?�각?�을 그리�??�한 �????계산
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

			float r, g, b, a; // ?�상 초기??
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

			// ?????�이??각도 계산
			float angle = (float)Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);

			// 45??각도마다 조정
			float adjustedAngle = (float)(Math.Round(angle / (Math.PI / 4)) * (Math.PI / 4));

			// ??리스???�성
			List<PointF> pointList = GenerateLineAtAngle(startPoint, endPoint, 1, adjustedAngle); // 간격?� 1�??�정

			// ?�들??그리�?
			for (int i = 0; i < pointList.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(pointList[i].X, pointList[i].Y), lineWidth, r, g, b, a);
			}

			gl.Disable(OpenGL.GL_BLEND);
		}

		public static List<PointF> GenerateLineAtAngle(PointF startPoint, PointF endPoint, float interval, float angle)
		{
			List<PointF> pointList = new List<PointF>();

			// ?�작?�과 ?�점 ?�이??거리 계산
			float distanceX = endPoint.X - startPoint.X;
			float distanceY = endPoint.Y - startPoint.Y;
			float totalDistance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

			// 간격???�라 ?�인???�성
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

			float r, g, b, a; // ?�상 초기??
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

			//gl.Enable(OpenGL.GL_BLEND);

			// ?�직???�는 ?�평?�을 ?�한 ??리스???�성
			List<PointF> pointList;
			if (Math.Abs(endPoint.X - startPoint.X) > Math.Abs(endPoint.Y - startPoint.Y))
			{
				// ?�평??
				pointList = GenerateHorizontalPointList(startPoint, endPoint, 1); // 간격?� 1�??�정
			}
			else
			{
				// ?�직??
				pointList = GenerateVerticalPointList(startPoint, endPoint, 1); // 간격?� 1�??�정
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

			// ?�작?�과 ?�점 ?�이??거리 계산 (y 축만 고려)
			float distanceY = endPoint.Y - startPoint.Y;

			// ?????�이??�?거리 계산
			float totalDistance = Math.Abs(distanceY);

			// 간격???�라 ?�인???�성
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

			// ?�작?�과 ?�점 ?�이??거리 계산 (x 축만 고려)
			float distanceX = endPoint.X - startPoint.X;

			// ?????�이??�?거리 계산
			float totalDistance = Math.Abs(distanceX);

			// 간격???�라 ?�인???�성
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

			float r, g, b, a; // ?�상 초기??
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

			gl.Enable(OpenGL.GL_BLEND);

			List<PointF> pointList = GeneratePointList(startPoint, endPoint, 1); // 간격?� 1�??�정

			for (int i = 0; i < pointList.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(pointList[i].X, pointList[i].Y), lineWidth, r, g, b, a);
			}

			gl.Disable(OpenGL.GL_BLEND);
		}

		public static List<System.Drawing.PointF> GetLinePoints(PointF startPoint, PointF endPoint, float lineWidth)
		{
			List<PointF> allPoints = new List<PointF>();

			List<PointF> pointList = GeneratePointList(startPoint, endPoint, 1); // 간격?� 1�??�정

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

			float r, g, b, a; // ?�상 초기??
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

			gl.Color(r, g, b); // ?�의 ?�상 ?�정

			List<PointF> pointList = GeneratePointList(startPoint, endPoint, 1); // 간격?� 1�??�정

			for (int i = 0; i < pointList.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(pointList[i].X, pointList[i].Y), lineWidth, r, g, b, 1);
			}
		}

		public static List<PointF> GeneratePointList(PointF startPoint, PointF endPoint, float interval)
		{
			List<PointF> pointList = new List<PointF>();

			// ?�작?�과 ?�점 ?�이??거리 계산
			float distanceX = endPoint.X - startPoint.X;
			float distanceY = endPoint.Y - startPoint.Y;

			// ?????�이??�?거리 계산
			float totalDistance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

			// 간격???�라 ?�인???�성
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


		//private static List<PointF> GeneratePointList(PointF startPoint, PointF endPoint, float interval)
		//{
		//	List<PointF> pointList = new List<PointF>();

		//	// ?�작?�과 ?�점 ?�이??거리 계산
		//	int distanceX = (int)endPoint.X - (int)startPoint.X;
		//	int distanceY = (int)endPoint.Y - (int)startPoint.Y;

		//	// ?????�이??�?거리 계산
		//	float totalDistance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

		//	// 간격?�로 ?�누?�서 ?�인???�성
		//	if (totalDistance > 0)
		//	{ // 분모가 0???�는 것을 방�?
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

		private static List<PointF> GeneratePointList(PointF startPoint, PointF endPoint)
		{
			List<PointF> pointList = new List<PointF>();

			// ?�작?�과 ?�점 ?�이??변?�량 계산
			int dx = (int)endPoint.X - (int)startPoint.X;
			int dy = (int)endPoint.Y - (int)startPoint.Y;

			// 최�? 변?�량??기�??�로 루프�??�행
			int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

			float xIncrement = dx / (float)steps;
			float yIncrement = dy / (float)steps;

			float x = startPoint.X;
			float y = startPoint.Y;

			for (int i = 0; i <= steps; i++)
			{
				// ?�재 ?�치???�인??추�?
				pointList.Add(new PointF(x, y));

				// ?�음 ?�치�??�동
				x += xIncrement;
				y += yIncrement;
			}

			return pointList;
		}

		public static void DrawLine(OpenGL gl, List<System.Drawing.PointF> points, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			if (points == null || points.Count < 2)
				return;

			float r, g, b, a; // ?�상 초기??
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

			gl.Color(r, g, b); // ?�의 ?�상 ?�정

			for (int i = 0; i < points.Count - 1; i++)
			{
				System.Drawing.PointF p1 = points[i];
				System.Drawing.PointF p2 = points[i + 1];

				// ?????�이??벡터 계산
				float dx = p2.X - p1.X;
				float dy = p2.Y - p1.Y;
				float length = (float)Math.Sqrt(dx * dx + dy * dy);

				// ?�께�??�용?�여 법선 벡터 계산
				float ux = lineWidth * (dy / length) / 2;
				float uy = lineWidth * (-dx / length) / 2;

				// ?�리곤으�??�꺼????그리�?
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

			// ?�의 중심??계산?�니??
			PointF center = new PointF((start.X + end.X) / 2, (start.Y + end.Y) / 2);

			// 반�?름�? ?????�이??거리???�반?�로 계산?�니?? (?�기?�는 직사각형???�닐 경우???�비해 가로�? ?�로 �?짧�? 쪽을 반�?름으�??�니??
			float radius = Math.Min(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)) / 2;

			EnumFillMode enumFillMode = useFill == true ? EnumFillMode.InFill : EnumFillMode.None;

			DrawThickCircle(gl, center.X, center.Y, radius, lineWidth, color, enumFillMode);
		}

		public static void DrawCircle(OpenGL gl, PointF start, PointF end, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode)
		{
			if (start.IsEmpty || end.IsEmpty) { return; }

			// ?�의 중심??계산?�니??
			PointF center = new PointF((start.X + end.X) / 2, (start.Y + end.Y) / 2);

			// 반�?름�? ?????�이??거리???�반?�로 계산?�니?? (?�기?�는 직사각형???�닐 경우???�비해 가로�? ?�로 �?짧�? 쪽을 반�?름으�??�니??
			float radius = Math.Min(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)) / 2;

			DrawThickCircle(gl, center.X, center.Y, radius, lineWidth, color, enumFillMode);
		}

		public static void DrawCircle(OpenGL gl, PointF start, PointF end, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode, SizeF textureSize)
		{
			if (start.IsEmpty || end.IsEmpty) { return; }

			// ?�의 중심??계산?�니??
			PointF center = new PointF((start.X + end.X) / 2, (start.Y + end.Y) / 2);

			// 반�?름�? ?????�이??거리???�반?�로 계산?�니?? (?�기?�는 직사각형???�닐 경우???�비해 가로�? ?�로 �?짧�? 쪽을 반�?름으�??�니??
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
			// ?�상 ?�정
			gl.Color(color.Color.R / 255.0, color.Color.G / 255.0, color.Color.B / 255.0, color.Color.A / 255.0);

			// ???�께 ?�정
			gl.LineWidth(lineWidth);

			// 채우�??��????�른 ??그리�?
			if (useFill)
			{
				gl.Begin(OpenGL.GL_TRIANGLE_FAN);
				gl.Vertex(center.X, center.Y); // 중심??
			}
			else
			{
				gl.Begin(OpenGL.GL_LINE_LOOP);
			}

			int numSegments = 100; // ?�을 그릴 ???�용???�그먼트????
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

			// ?�의 중심??계산?�니??
			PointF center = new PointF((start.X + end.X) / 2, (start.Y + end.Y) / 2);
			center = new PointF((float)Math.Round(center.X), (float)Math.Round(center.Y));

			// 반�?름�? ?????�이??거리???�반?�로 계산?�니??
			float radius = Math.Min(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)) / 2;

			// ?�작�???반�?름을 ?�의?�니??
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

			return circlePoints.Distinct().ToList(); // 중복 ???�거
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

			// ?�의 ?�께�?조정?�니?? ?�???�께??경우 0.5�??�합?�다.
			if ((int)size % 2 != 0)
			{
				point.X += halfSize;
				point.Y += halfSize;
			}

			// ?�각?�을 그리�??�한 모든 ?�인??계산
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
			return allPoints.Distinct().ToList(); // 중복 ???�거
		}

		public static void DrawThickCircle(OpenGL gl, float centerX, float centerY, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode)
		{
			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??
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
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

			// ?�텐??버퍼 ?�정
			gl.Enable(OpenGL.GL_STENCIL_TEST);
			gl.Clear(OpenGL.GL_STENCIL_BUFFER_BIT);

			if (enumFillMode == EnumFillMode.InFill)
			{
				// ???��?�?채우�?
				gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

				FillCirclePoints(gl, (int)centerX, (int)centerY, (int)radius, r, g, b, a);
			}
			else if (enumFillMode == EnumFillMode.OutFill)
			{
				// 1. ???��?�??�텐??버퍼??기록
				gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

				FillCirclePoints(gl, (int)centerX, (int)centerY, (int)radius, 0, 0, 0, 0); // ?�명???�으�?그리�?(?�제 그림 ?�음, ?�텐?�만 ?�정)

				// 2. ???��?�??�텐??값이 0??부분에�?그리�?
				gl.StencilFunc(OpenGL.GL_EQUAL, 0, 0xFF); // ?�텐??값이 0??부분만 그리�?
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_KEEP);

				gl.Color(r, g, b, a);

				// ?�체 ?�스�??�역????�� ???�각?�을 그립?�다.
				gl.Begin(OpenGL.GL_QUADS);
				gl.Vertex(0, 0);
				gl.Vertex(textureSize.Width, 0);
				gl.Vertex(textureSize.Width, textureSize.Height);
				gl.Vertex(0, textureSize.Height);
				gl.End();
			}
			else if (enumFillMode == EnumFillMode.None)
			{
				// ?�곽?�만 그리�?
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

		//public static void DrawThickCircle(OpenGL gl, float centerX, float centerY, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, EnumFillMode enumFillMode, SizeF textureSize)
		//{
		//	gl.Enable(OpenGL.GL_BLEND);
		//	gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

		//	float r, g, b, a;
		//	(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

		//	// ?�텐??버퍼 ?�정 �?초기??
		//	gl.Enable(OpenGL.GL_STENCIL_TEST);
		//	gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

		//	if (enumFillMode == EnumFillMode.InFill)
		//	{
		//		// ???��?�?채우�?
		//		gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
		//		gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

		//		FillCirclePoints(gl, (int)centerX, (int)centerY, (int)radius, r, g, b, a);
		//	}
		//	else if (enumFillMode == EnumFillMode.OutFill)
		//	{
		//		// 1. ???��?�??�텐??버퍼??기록
		//		gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
		//		gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

		//		FillCirclePoints(gl, (int)centerX, (int)centerY, (int)radius, 0, 0, 0, 0); // ?�명???�으�?그리�?(?�제 그림 ?�음, ?�텐?�만 ?�정)

		//		// 2. ???��?�??�텐??값이 0??부분에�?그리�?
		//		gl.StencilFunc(OpenGL.GL_EQUAL, 0, 0xFF); // ?�텐??값이 0??부분만 그리�?
		//		gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_KEEP);

		//		gl.Color(r, g, b, a);

		//		// ?�체 ?�스�??�역????�� ???�각?�을 그립?�다.
		//		gl.Begin(OpenGL.GL_QUADS);
		//		gl.Vertex(0, 0);
		//		gl.Vertex(textureSize.Width, 0);
		//		gl.Vertex(textureSize.Width, textureSize.Height);
		//		gl.Vertex(0, textureSize.Height);
		//		gl.End();
		//	}
		//	else if (enumFillMode == EnumFillMode.None)
		//	{
		//		// ?�곽?�만 그리�?
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


		private static void FillCirclePoints(OpenGL gl, int cx, int cy, int radius, float r, float g, float b, float a)
		{
			gl.Color(r, g, b, a);
			gl.Begin(OpenGL.GL_TRIANGLE_FAN);
			gl.Vertex(cx, cy);
			for (int angle = 0; angle <= 360; angle += 1) // 각도 간격??좁게 ?�정?�여 부?�러???�을 그리�?
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





		//public static void DrawThickCircle(OpenGL gl, float centerX, float centerY, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, bool isFill = false)
		//{
		//	gl.Enable(OpenGL.GL_BLEND);
		//	gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

		//	float r, g, b, a;
		//	(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??
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


		// 브레?�햄 ???�고리즘???�용?�여 ?�을 그리???�수
		//public static void DrawThickCircle(OpenGL gl, float centerX, float centerY, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color)
		//{
		//	gl.Enable(OpenGL.GL_BLEND);
		//	gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

		//	float r, g, b, a;
		//	(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??
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

		public static void DrawThickCircle(OpenGL gl, PointF center, List<PointF> pointFs, float radius, float lineWidth, System.Windows.Media.SolidColorBrush color, bool isFiil = false)
		{
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

			gl.Color(r, g, b); // ?�상 ?�정
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
			(r, g, b, a) = ConvertColorToOpenGLRGB(color); // ?�상 변??

			int segments = 300; // ?�의 부?�러?�??결정?�는 ?�그먼트????
			float twoPi = 2.0f * (float)Math.PI;

			gl.Color(r, g, b); // ?�상 ?�정
			gl.Begin(OpenGL.GL_TRIANGLE_FAN);
			gl.Vertex(center.X, center.Y); // ?�의 중심 ?�점
			for (int i = 0; i <= segments; i++)
			{
				float theta = twoPi * i / segments;
				float x = center.X + (radius + (lineWidth / 2)) * (float)Math.Cos(theta);
				float y = center.Y + (radius + (lineWidth / 2)) * (float)Math.Sin(theta);
				gl.Vertex(x, y);
			}
			gl.End();
		}

		private static List<List<System.Drawing.PointF>> SplitIntoSegments(List<DotInfo> points)
		{
			var segments = new List<List<System.Drawing.PointF>>();
			var currentSegment = new List<System.Drawing.PointF>();
			bool lastPointWasNull = true;  // 마�?�??�인?��? null?�었?��? 추적

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
						// ???�그먼트 ?�작 ?�에 ?�전 ?�그먼트�?추�?
						segments.Add(new List<System.Drawing.PointF>(currentSegment));
						currentSegment.Clear();
					}
					currentSegment.Add(new PointF(point.X, point.Y));
					lastPointWasNull = false;
				}
			}

			// 마�?�??�그먼트 추�?
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
			// 각각???�분???�???�꺼???�을 그립?�다.
			for (int i = 0; i < points.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(points[i].X, points[i].Y), lineWidth, color);
			}
		}

		public static void DrawWithPen(OpenGL gl, List<PointF> points, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			// ?�상 변??
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			// ?�텐??버퍼 ?�정
			gl.Enable(OpenGL.GL_STENCIL_TEST);
			gl.Clear(OpenGL.GL_STENCIL_BUFFER_BIT);

			// 각각???�에 ?�???�각?�을 그립?�다.
			for (int i = 0; i < points.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(points[i].X, points[i].Y), lineWidth, r, g, b, a);
			}

			gl.Disable(OpenGL.GL_STENCIL_TEST);
		}


		//public static void DrawWithPen(OpenGL gl, List<PointF> points, float lineWidth, System.Windows.Media.SolidColorBrush color)
		//{
		//	// 각각???�분???�???�꺼???�을 그립?�다.
		//	for (int i = 0; i < points.Count; i++)
		//	{
		//		DrawPointAsSquare(gl, new PointF(points[i].X, points[i].Y), lineWidth, color);
		//	}
		//}

		public static void DrawWithPen(OpenGL gl, List<PointF> points, float lineWidth, System.Drawing.Color color)
		{
			// 각각???�분???�???�꺼???�을 그립?�다.
			for (int i = 0; i < points.Count; i++)
			{
				DrawPointAsSquare(gl, new PointF(points[i].X, points[i].Y), lineWidth, color);
			}
		}

		public static void DrawWithPen(OpenGL gl, List<DotInfo> points, float lineWidth, System.Windows.Media.SolidColorBrush color)
		{
			DrawWithPen(gl, ConvertDotInfoToPoints(points), lineWidth, color);
		}

		/// <summary>
		/// 직선??Draw?�니??
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="measurement"></param>
		public static void DrawMeasurement(OpenGL gl, Measurement measurement, OpenGlFontRenderOptions glFontRenderOptions, List<OpenGlFontBitmapEntry> fontBitmapEntries, float xSpan, float ySpan, System.Drawing.RectangleF fitRect, System.Drawing.SizeF offsetSize, float pixelPermm)
		{
			if (measurement.StartPoint.IsEmpty) { return; }

			// ?�살??머리???�기?� 각도
			float arrowheadLength = 10.0f;
			float arrowheadAngle = 45.0f;

			// 방향 벡터 계산
			float dx = measurement.EndPoint.X - measurement.StartPoint.X;
			float dy = measurement.EndPoint.Y - measurement.StartPoint.Y;
			float magnitude = (float)Math.Sqrt(dx * dx + dy * dy);

			// ?�위 방향 벡터
			float ux = dx / magnitude;
			float uy = dy / magnitude;

			// ?�살??머리??방향 계산
			float angleRadians = (float)(Math.PI * arrowheadAngle / 180.0);
			float arrowheadDx1 = arrowheadLength * (float)(ux * Math.Cos(angleRadians) - uy * Math.Sin(angleRadians));
			float arrowheadDy1 = arrowheadLength * (float)(ux * Math.Sin(angleRadians) + uy * Math.Cos(angleRadians));

			float arrowheadDx2 = arrowheadLength * (float)(ux * Math.Cos(-angleRadians) - uy * Math.Sin(-angleRadians));
			float arrowheadDy2 = arrowheadLength * (float)(ux * Math.Sin(-angleRadians) + uy * Math.Cos(-angleRadians));

			gl.PushAttrib(OpenGL.GL_LINE_BIT | OpenGL.GL_LINE_STIPPLE);

			gl.Color(0.0f, 1.0f, 0.0f); // ?��??�으�??�정
			gl.LineWidth(5);
			gl.Begin(OpenGL.GL_LINES);
			{
				// ??그리�?
				gl.Vertex(measurement.StartPoint.X, measurement.StartPoint.Y);
				gl.Vertex(measurement.EndPoint.X, measurement.EndPoint.Y);

				// ?�작?�의 ?�살??머리 그리�?
				gl.Vertex(measurement.StartPoint.X, measurement.StartPoint.Y);
				gl.Vertex(measurement.StartPoint.X + arrowheadDx1, measurement.StartPoint.Y + arrowheadDy1);

				gl.Vertex(measurement.StartPoint.X, measurement.StartPoint.Y);
				gl.Vertex(measurement.StartPoint.X + arrowheadDx2, measurement.StartPoint.Y + arrowheadDy2);

				// ?�점???�살??머리 그리�?
				gl.Vertex(measurement.EndPoint.X, measurement.EndPoint.Y);
				gl.Vertex(measurement.EndPoint.X - arrowheadDx1, measurement.EndPoint.Y - arrowheadDy1);

				gl.Vertex(measurement.EndPoint.X, measurement.EndPoint.Y);
				gl.Vertex(measurement.EndPoint.X - arrowheadDx2, measurement.EndPoint.Y - arrowheadDy2);
			}
			gl.End();

			gl.PopAttrib();

			// 측정??거리 ?�스???�성

			float distancePermm = measurement.Distance * pixelPermm;
			string distanceText = $"{distancePermm:0.0000} mm / Pixel:{measurement.Distance}";

			// ???�의 중간 지??계산
			float midX = (measurement.StartPoint.X + measurement.EndPoint.X) / 2;
			float midY = (measurement.StartPoint.Y + measurement.EndPoint.Y) / 2;

			string faceName = glFontRenderOptions.FontName; // ?�트 ?�름
			float fontSize = glFontRenderOptions.FontSize; // ?�트 ?�기
			System.Drawing.Color color = glFontRenderOptions.Color;
			System.Drawing.Color color2 = System.Drawing.Color.Blue;
			System.Drawing.Color color3 = System.Drawing.Color.Yellow;

			//DrawViewerBaseText(gl, fontBitmapEntries, xSpanPixed, ySpanPixed, (int)midX, (int)midY, color2, faceName, 30, distanceText);
			DrawText(gl, fontBitmapEntries, xSpan, ySpan, offsetSize, (int)midX, (int)midY, color, faceName, fontSize, distanceText);
			//DrawFixedText(gl, fontBitmapEntries, (int)midX, (int)midY, offsetSizePixed, color3, faceName, fontSize, distanceText);
		}

		public static void DrawTextOnTexture(OpenGL gl, string text, float texCoordX, float texCoordY, Size glControlSize)
		{
			string faceName = "Arial"; // ?�트 ?�름
			float fontSize = 30f; // ?�트 ?�기
								  // ?�스�?좌표�??�면 좌표�?변??
			float screenX = texCoordX * glControlSize.Width;
			float screenY = texCoordY * glControlSize.Height;

			// ?�면 좌표�??�용?�여 ?�스???�더�?
			gl.DrawText((int)screenX, (int)screenY, 1, 0, 0, faceName, fontSize, text);
		}

		// ?�역?�서 ?�용???�이???�로그램 ID
		//private static uint shaderProgram;
		//public static bool tt = false;

		// ?�이???�로그램??초기?�하???�수 (?�플리�??�션 ?�작 ????번만 ?�출)
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
		//								  "        discard;\n" +  // ?��????�역 ?�전???�명?�게 처리
		//								  "    }\n" +
		//								  "    else if (abs(maskColor.r - 1.0) < 0.01 && abs(maskColor.g) < 0.01 && abs(maskColor.b) < 0.01)\n" +
		//								  "    {\n" +
		//								  "        gl_FragColor = vec4(1.0, 0.0, 0.0, alphaFactor);\n" +  // 빨간???�역 처리
		//								  "    }\n" +
		//								  "    else\n" +
		//								  "    {\n" +
		//								  "        gl_FragColor = vec4(maskColor.rgb, maskColor.a * alphaFactor);\n" +  // ?�른 ??처리
		//								  "    }\n" +
		//								  "}\n";

		//	// ?�이??컴파??�??�로그램 링크
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

		//	// ?�이??컴파??�?링크 ?�태 ?�인
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
		//		InitializeShader(gl);  // ?�이??초기??
		//	}

		//	gl.Enable(OpenGL.GL_TEXTURE_2D);
		//	gl.Enable(OpenGL.GL_BLEND);
		//	gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);  // ?�파 블렌???�정

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
		//						// ?�본 ?�스�?그리�?(기본 ?�스�?
		//						gl.Color(1.0f, 1.0f, 1.0f, 1.0f);  // 불투명하�??�정
		//						if (param.IsRotated)
		//						{
		//							gl.PushMatrix();  // 변???�태 ?�??
		//							gl.Translate(centerX, centerY, 0);
		//							gl.Rotate(param.RotationAngle, 0, 0, 1);
		//							gl.Translate(-centerX, -centerY, 0);
		//						}

		//						gl.BindTexture(OpenGL.GL_TEXTURE_2D, param.OriTextureId);
		//						DrawQuad(gl, param.GLDrawingTextureArea);  // ?�본 ?�스�?그리�?

		//						// ?�이???�로그램 ?�성??
		//						gl.UseProgram(shaderProgram);

		//						// ?�니???�정: alphaFactor�??�해 ?�명???�어
		//						int alphaFactorLocation = gl.GetUniformLocation(shaderProgram, "alphaFactor");
		//						gl.Uniform1(alphaFactorLocation, param.IsTransParency ? param.TransParency : 1.0f);  // ?�명???�용

		//						// 마스???�스�?바인??�?그리�?(백그?�운???�스�?
		//						gl.ActiveTexture(OpenGL.GL_TEXTURE0);
		//						gl.BindTexture(OpenGL.GL_TEXTURE_2D, param.OriBackgroundTextureId);

		//						// 마스???�스�?그리�?
		//						DrawQuad(gl, param.GLDrawingTextureArea);

		//						// ?�이??비활?�화
		//						gl.UseProgram(0);

		//						if (param.IsRotated)
		//						{
		//							gl.PopMatrix();  // ?�전 변???�태 복원
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}

		//	gl.Disable(OpenGL.GL_BLEND);
		//	gl.Disable(OpenGL.GL_TEXTURE_2D);
		//}



		public static void DrawODBTexture(OpenGL gl, ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>> textureAreas, List<string> order)
		{
			gl.Enable(OpenGL.GL_TEXTURE_2D);

			foreach (var key in order.AsEnumerable().Reverse())
			{
				if (textureAreas.TryGetValue(key, out var drawingParams))
				{
					// ?��?지 ?�름별로 그룹??�?처리
					var groupedByImageName = drawingParams.GroupBy(param => param.ImageName);

					foreach (var group in groupedByImageName)
					{
						// �?그룹(?��?지)???�??중심 계산
						float minX = group.Min(param => param.GLDrawingTextureArea.Left);
						float maxX = group.Max(param => param.GLDrawingTextureArea.Right);
						float minY = group.Min(param => param.GLDrawingTextureArea.Top);
						float maxY = group.Max(param => param.GLDrawingTextureArea.Bottom);
						float centerX = (minX + maxX) / 2;
						float centerY = (minY + maxY) / 2;

						foreach (OpenGlTextureDrawingParam param in group)
						{
							if (param.IsVisible)
							{
								if (param.IsRotated)
								{
									gl.PushMatrix(); // ?�재 변???�태 ?�??

									// ?�체 ?��?지??중심??기�??�로 ?�전
									gl.Translate(centerX, centerY, 0);
									gl.Rotate(param.RotationAngle, 0, 0, 1);
									gl.Translate(-centerX, -centerY, 0);
								}

								// ?�본
								gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
								gl.BindTexture(OpenGL.GL_TEXTURE_2D, param.OriTextureId);
								DrawQuad(gl, param.GLDrawingTextureArea);

								gl.Enable(OpenGL.GL_BLEND);
								gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);  // ?�파 블렌???�용

								gl.Color(1f, 1f, 1f, param.IsTransParency ? param.TransParency : 1.0f);
								gl.BindTexture(OpenGL.GL_TEXTURE_2D, param.OriBackgroundTextureId);
								DrawQuad(gl, param.GLDrawingTextureArea);

								gl.Disable(OpenGL.GL_BLEND);  // 블렌??비활?�화

								if (param.IsRotated)
								{
									gl.PopMatrix(); // ?�전 매트�?�� 복원
								}
							}
						}
					}
				}
			}
			gl.Disable(OpenGL.GL_TEXTURE_2D);  // ?�스�?비활?�화
		}

		public static void DrawTexture(OpenGL gl, ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>> textureAreas, List<string> order)
		{
			gl.Enable(OpenGL.GL_TEXTURE_2D);
			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			foreach (var key in order.AsEnumerable().Reverse())
			{
				if (textureAreas.TryGetValue(key, out var drawingParams))
				{
					// ?��?지 ?�름별로 그룹??�?처리
					var groupedByImageName = drawingParams.GroupBy(param => param.ImageName);

					foreach (var group in groupedByImageName)
					{
						// �?그룹(?��?지)???�??중심 계산
						float minX = group.Min(param => param.GLDrawingTextureArea.Left);
						float maxX = group.Max(param => param.GLDrawingTextureArea.Right);
						float minY = group.Min(param => param.GLDrawingTextureArea.Top);
						float maxY = group.Max(param => param.GLDrawingTextureArea.Bottom);
						float centerX = (minX + maxX) / 2;
						float centerY = (minY + maxY) / 2;

						foreach (OpenGlTextureDrawingParam param in group)
						{
							if (param.IsVisible)
							{
								//gl.Color(1.0f, 1.0f, 1.0f, param.IsTransParency ? param.TransParency : 1.0f);
								gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
								if (param.IsRotated)
								{
									gl.PushMatrix(); // ?�재 변???�태 ?�??

									// ?�체 ?��?지??중심??기�??�로 ?�전
									gl.Translate(centerX, centerY, 0);
									gl.Rotate(param.RotationAngle, 0, 0, 1);
									gl.Translate(-centerX, -centerY, 0);
								}

								// ?�스�?바인??�??�각??그리�?
								gl.BindTexture(OpenGL.GL_TEXTURE_2D, param.OriTextureId);
								DrawQuad(gl, param.GLDrawingTextureArea);

								if (param.IsRotated)
								{
									gl.PopMatrix(); // ?�전 매트�?�� 복원
								}
							}
						}
					}
				}
			}

			gl.Disable(OpenGL.GL_BLEND);
			gl.Disable(OpenGL.GL_TEXTURE_2D);
		}


		public static void DrawTexturedQuadWithTransparency(OpenGL gl, uint textureId, RectangleF drawArea)
		{
			// 블렌???�성??
			gl.Enable(OpenGL.GL_BLEND);
			// 블렌???�수 ?�정: ?�스 ?�파?� 1-?�스 ?�파 ?�용
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			// ?�스�?바인??�??�각??그리�?
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);
			gl.Begin(OpenGL.GL_QUADS);

			gl.TexCoord(0.0f, 1.0f); gl.Vertex(drawArea.Left, drawArea.Top);
			gl.TexCoord(1.0f, 1.0f); gl.Vertex(drawArea.Right, drawArea.Top);
			gl.TexCoord(1.0f, 0.0f); gl.Vertex(drawArea.Right, drawArea.Bottom);
			gl.TexCoord(0.0f, 0.0f); gl.Vertex(drawArea.Left, drawArea.Bottom);

			gl.End();

			// ?�스�?바인???�제
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

			// 블렌??비활?�화
			gl.Disable(OpenGL.GL_BLEND);
		}

		public static void DrawQuad(OpenGL gl, RectangleF rect)
		{
			gl.Begin(OpenGL.GL_QUADS);
			{
				gl.TexCoord(0.0f, 1.0f); gl.Vertex(rect.Left, rect.Bottom);
				gl.TexCoord(0.0f, 0.0f); gl.Vertex(rect.Left, rect.Top);
				gl.TexCoord(1.0f, 0.0f); gl.Vertex(rect.Right, rect.Top);
				gl.TexCoord(1.0f, 1.0f); gl.Vertex(rect.Right, rect.Bottom);
			}
			gl.End();
		}

		public static void DrawIrregularQuad(OpenGL gl, RectangleF rect, System.Windows.Media.SolidColorBrush solidColorBrush)
		{
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(solidColorBrush);
			gl.Color(r, g, b);
			gl.Begin(OpenGL.GL_QUADS);
			{
				gl.TexCoord(0.0f, 1.0f); gl.Vertex(rect.Left, rect.Bottom);
				gl.TexCoord(0.0f, 0.0f); gl.Vertex(rect.Left, rect.Top);
				gl.TexCoord(1.0f, 0.0f); gl.Vertex(rect.Right, rect.Top);
				gl.TexCoord(1.0f, 1.0f); gl.Vertex(rect.Right, rect.Bottom);
			}
			gl.End();
		}

		public static (float, float, float, float) ConvertColorToOpenGLRGB(System.Drawing.Color color)
		{
			float red = color.R / 255f;
			float green = color.G / 255f;
			float blue = color.B / 255f;
			float alpha = color.A / 255f;

			return (red, green, blue, alpha);
		}

		public static System.Drawing.Color ConvertOpenGLRGBToColor(float red, float green, float blue, float alpha)
		{
			return System.Drawing.Color.FromArgb(
				(int)(alpha * 255),
				(int)(red * 255),
				(int)(green * 255),
				(int)(blue * 255)
			);
		}

		public static float[] ConvertColorToOpenGLRGBArr(System.Drawing.Color color)
		{
			float red = color.R / 255f;
			float green = color.G / 255f;
			float blue = color.B / 255f;
			float alpha = color.A / 255f;

			return new float[4] { red, green, blue, alpha };
		}

		public static System.Drawing.Color ConvertOpenGLRGBArrToColor(float[] rgba)
		{
			if (rgba == null || rgba.Length != 4)
				throw new ArgumentException("Input array must have exactly 4 elements (R,G,B,A).");

			return System.Drawing.Color.FromArgb(
				(int)(rgba[3] * 255),
				(int)(rgba[0] * 255),
				(int)(rgba[1] * 255),
				(int)(rgba[2] * 255)
			);
		}


		public static (float, float, float, float) ConvertColorToOpenGLRGB(System.Windows.Media.SolidColorBrush brush)
		{
			float red = brush.Color.R / 255f;
			float green = brush.Color.G / 255f;
			float blue = brush.Color.B / 255f;
			float alpha = brush.Color.A / 255f;

			return (red, green, blue, alpha);
		}

		public static System.Windows.Media.SolidColorBrush ConvertOpenGLRGBToBrush(float red, float green, float blue, float alpha)
		{
			var color = System.Windows.Media.Color.FromArgb(
				(byte)(alpha * 255),
				(byte)(red * 255),
				(byte)(green * 255),
				(byte)(blue * 255)
			);
			return new System.Windows.Media.SolidColorBrush(color);
		}

		public static System.Windows.Media.SolidColorBrush ConvertOpenGLRGBArrToBrush(float[] rgba)
		{
			if (rgba == null || rgba.Length != 4)
				throw new ArgumentException("Input array must have exactly 4 elements (R,G,B,A).");

			var color = System.Windows.Media.Color.FromArgb(
				(byte)(rgba[3] * 255),
				(byte)(rgba[0] * 255),
				(byte)(rgba[1] * 255),
				(byte)(rgba[2] * 255)
			);
			return new System.Windows.Media.SolidColorBrush(color);
		}


		public static void DrawGroupName(OpenGL gl, CanvasOverlayManager overlayManager, OpenGlTextDrawOptions glDrawTextOptions)
		{
			foreach (var overlayItem in overlayManager.GetAllVisibleOverlays())
			{
				if (!overlayItem.IsGroupRectangle) { continue; }

				if ((overlayItem.Shape as CanvasRect<float>).Width == 0 || (overlayItem.Shape as CanvasRect<float>).Height == 0) { continue; }
				float midX = (overlayItem.Shape as CanvasRect<float>).LeftTop.X;
				float midY = (overlayItem.Shape as CanvasRect<float>).LeftTop.Y + 20;

				string faceName = "Arial"; // ?�트 ?�름
				float fontSize = 15; // ?�트 ?�기

				DrawText(gl, glDrawTextOptions.FontBitmapEntries, glDrawTextOptions.XSpan, glDrawTextOptions.YSpan, glDrawTextOptions.OffsetSize, (int)midX, (int)midY, overlayItem.Color, faceName, fontSize, overlayItem.GroupType);
			}
		}

		public static void DrawRoiItemName(OpenGL gl, CanvasOverlayManager overlayManager, OpenGlTextDrawOptions glDrawTextOptions)
		{
			int index = 1;
			foreach (var overlayItem in overlayManager.GetAllVisibleOverlays())
			{
				if (overlayItem.IsGroupRectangle) { continue; }
				EnumInspWindowType groupType = overlayItem.InspWindowType;

				if ((overlayItem.Shape as CanvasRect<float>).Width == 0 || (overlayItem.Shape as CanvasRect<float>).Height == 0) { continue; }
				float midX = (overlayItem.Shape as CanvasRect<float>).LeftTop.X;
				float midY = (overlayItem.Shape as CanvasRect<float>).LeftTop.Y + 10;

				string faceName = "Arial"; // ?�트 ?�름
				float fontSize = 12; // ?�트 ?�기

				DrawText(gl, glDrawTextOptions.FontBitmapEntries, glDrawTextOptions.XSpan, glDrawTextOptions.YSpan, glDrawTextOptions.OffsetSize, (int)midX, (int)midY, overlayItem.Color, faceName, fontSize, $"{index}");
				index++;
			}
		}

		public static uint CreateTextTexture(OpenGL gl, string text, Font font, System.Drawing.Color textColor)
		{
			// Graphics 객체�??�용?�여 ?�스?�의 ?�기�?측정
			SizeF textSize;
			using (var tempBitmap = new Bitmap(1, 1))
			{
				using (var g = Graphics.FromImage(tempBitmap))
				{
					textSize = g.MeasureString(text, font);
				}
			}

			// 측정???�스???�기�?기반?�로 충분???�백??가�?비트�??�성
			int width = (int)Math.Ceiling(textSize.Width);
			int height = (int)Math.Ceiling(font.GetHeight());

			using (Bitmap bitmap = new Bitmap(width, height))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.Clear(System.Drawing.Color.Transparent); // 비트�?배경???�명?�게 채우�?

					// ?�스???�더�?
					using (System.Drawing.Brush brush = new SolidBrush(textColor))
					{
						// ?�트 메트�?��??기반?�여 ?�스???�더�??�치 조정
						PointF position = new PointF(0, 0); // ?�트 ?�이즈�? 베이?�라?�에 ?�라 조정 가??
						graphics.DrawString(text, font, brush, position);
					}
				}
				// 비트맵을 ?�하 반전?�킴
				bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

				uint[] gtexture = new uint[1];
				gl.GenTextures(1, gtexture);
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, gtexture[0]);

				// ?�스�??�터�?�??�핑 ?�션 ?�정
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);

				// 비트�??�이?��? ?�스�??�이?�로 ?�로??
				BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA, bitmap.Width, bitmap.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);
				bitmap.UnlockBits(data);

				return gtexture[0]; // ?�성???�스�?ID 반환
			}

			// Graphics 객체�??�용?�여 ?�스?�의 ?�기�?측정
			//SizeF textSize;
			//using (var tempBitmap = new Bitmap(1, 1))
			//{
			//	using (var g = Graphics.FromImage(tempBitmap))
			//	{
			//		textSize = g.MeasureString(text, font);
			//	}
			//}

			//// 측정???�스???�기??기반?�여 비트�??�성
			//using (Bitmap bitmap = new Bitmap((int)textSize.Width, (int)textSize.Height))
			//{
			//	using (Graphics graphics = Graphics.FromImage(bitmap))
			//	{
			//		// 비트�?배경???�명?�게 채우�?
			//		graphics.Clear(Color.Transparent);

			//		// ?�스???�더�?
			//		using (Brush brush = new SolidBrush(textColor))
			//		{
			//			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			//			StringFormat stringFormat = new StringFormat();
			//			stringFormat.FormatFlags = StringFormatFlags.NoClip;
			//			//graphics.DrawString(text, font, brush, new PointF(0, 0), stringFormat);
			//			TextRenderer.DrawText(graphics, text, font, new Point(0, 0), textColor);
			//		}
			//	}
			//	// 비트맵을 ?�하 반전?�킴
			//	bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

			//	// OpenGL ?�스�??�성 �??�정
			//	uint[] gtexture = new uint[1];
			//	gl.GenTextures(1, gtexture);
			//	gl.BindTexture(OpenGL.GL_TEXTURE_2D, gtexture[0]);

			//	// ?�스�??�터�?�??�핑 ?�션 ?�정
			//	gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
			//	gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
			//	gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
			//	gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);

			//	// 비트�??�이?��? ?�스�??�이?�로 ?�로??
			//	BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			//	gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA, bitmap.Width, bitmap.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);
			//	bitmap.UnlockBits(data);

			//	return gtexture[0]; // ?�성???�스�?ID 반환
			//}
		}


		private static void SetShapeColorAndStyle(OpenGL gl, CanvasShape shape, System.Drawing.Color color, bool isDotted, float lineWidth = 1.0f)
		{
			float r, g, b, a;
			(r, g, b, a) = ConvertColorToOpenGLRGB(color);

			// ???�상 ?�정
			gl.Color(r, g, b, a);

			// ???�께 ?�정
			gl.LineWidth(lineWidth);

			// ?�정 조건???�라 ?�상 변�?
			if (shape is CanvasRect<float>)
			{
				if ((shape as CanvasRect<float>).IsEditing)
				{
					gl.Color(System.Drawing.Color.Yellow.R / 255.0f, System.Drawing.Color.Yellow.G / 255.0f, System.Drawing.Color.Yellow.B / 255.0f);
				}
			}

			// ???��????�정 (?�선 ??
			if (isDotted)
			{
				gl.Enable(OpenGL.GL_LINE_STIPPLE);
				gl.LineStipple(1, 0x00FF); // ?�선 ?�턴 ?�정
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


		/// <summary>
		///
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="cx">Center X of Rectangle</param>
		/// <param name="cy">Center Y of Rectangle</param>
		/// <param name="rx">Half of Rectangle Width</param>
		/// <param name="ry">Half of Rectangle Height</param>
		/// <param name="num_segments">Ellipse�??�인?�로 분할?�서 그릴 개수</param>
		/// <param name="lineWidth"></param>
		/// <param name="lineColorRGB"></param>
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

		public static System.Windows.Media.Color ToMediaColor(System.Drawing.Color color)
		{
			return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		/// <summary>
		/// 변?��? 감�???Shape�??�시 Draw?�니??
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="newObject"></param>
		public static void CompileOverlayShape(OpenGL gl, CanvasOverlayItem newObject)
		{
			AllocatDisplayId(gl, newObject.Shape);
			SetStencile(gl);
			gl.NewList(newObject.Shape.DisplayListId, OpenGL.GL_COMPILE);

			bool isDotted = (newObject.IsGroupRectangle && (newObject.Shape is CanvasRect<float>) && !(newObject.Shape as CanvasRect<float>).IsEmpty()) == true ? true : false;

			if (newObject.Shape is CanvasRect<float>)
			{
				CanvasRect<float> canvasRect = newObject.Shape as CanvasRect<float>;

				DrawShape(gl, canvasRect, newObject.Color, isDotted, false, canvasRect.LineWidth);

				// ?�장???�각?�을 ?�선?�로 그립?�다.
				if (canvasRect.ExtendedRectangle != null)
				{
					// ?�장???�이?�그?�을 ?�선?�로 그리�??�한 ?�정
					//DrawShape(gl, (newObject.Shape as CanvasRect<float>).ExtendedRectangle, newObject.Color, true, false);
					System.Drawing.Color newColor = newObject.Color;
					if (canvasRect.IsEditing)
					{
						newColor = System.Drawing.Color.Yellow;
						DrawShape(gl, canvasRect.ExtendedRectangle, newColor, true, false);
					}
					else
					{
						if (newObject.IsExtentionRectange)
						{
							DrawShape(gl, canvasRect.ExtendedRectangle, newColor, true, false);
						}
					}
				}
				if (newObject.IsFill)
				{
					System.Drawing.PointF start = new System.Drawing.PointF(canvasRect.LeftBottom.X, canvasRect.LeftBottom.Y);
					System.Drawing.PointF end = new System.Drawing.PointF(canvasRect.RightTop.X, canvasRect.RightTop.Y);
					OpenGlDrawing.DrawRectangle(gl, start, end, 1, EnumFillMode.InFill, new SolidColorBrush(ToMediaColor(newObject.Color)), new SizeF());
				}
			}
			if (newObject.Shape is LineInfo)
			{
				LineInfo lineInfo = newObject.Shape as LineInfo;
				PointF startPoint = new PointF(lineInfo.StartDot.X, lineInfo.StartDot.Y);
				PointF endPoint = new PointF(lineInfo.EndDot.X, lineInfo.EndDot.Y);

				// RGB 값을 0?�서 255 ?�이??값으�?변??
				byte red = (byte)(lineInfo.LineColor[0] * 255);
				byte green = (byte)(lineInfo.LineColor[1] * 255);
				byte blue = (byte)(lineInfo.LineColor[2] * 255);

				// SolidColorBrush ?�성
				System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(newObject.Color.R, newObject.Color.G, newObject.Color.B));

				DrawLine(gl, startPoint, endPoint, lineInfo.Width, brush);
			}

			if (newObject.Shape is RectInfo)
			{
				RectInfo rectInfo = newObject.Shape as RectInfo;
				System.Drawing.PointF start = new PointF(rectInfo.LeftBottom.X, rectInfo.LeftBottom.Y);
				System.Drawing.PointF end = new PointF(rectInfo.RightTop.X, rectInfo.RightTop.Y);

				// RGB 값을 0?�서 255 ?�이??값으�?변??
				byte red = (byte)(rectInfo.LineColor[0] * 255);
				byte green = (byte)(rectInfo.LineColor[1] * 255);
				byte blue = (byte)(rectInfo.LineColor[2] * 255);

				System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(newObject.Color.A, newObject.Color.R, newObject.Color.G, newObject.Color.B));
				//System.Drawing.Color brush = System.Drawing.Color.FromArgb(red, green, blue);

				DrawRectangle(gl, start, end, rectInfo.Width, rectInfo.IsFill, brush);
			}
			if (newObject.Shape is CircleInfo)
			{
				CircleInfo circleInfo = newObject.Shape as CircleInfo;
				//System.Drawing.PointF start = new PointF(circleInfo.StartDot.X, circleInfo.StartDot.Y);
				//System.Drawing.PointF end = new PointF(circleInfo.EndDot.X, circleInfo.EndDot.Y);
				System.Drawing.PointF center = new PointF(circleInfo.CenterDot.X, circleInfo.CenterDot.Y);
				float radius = circleInfo.Radius;
				// RGB 값을 0?�서 255 ?�이??값으�?변??
				byte red = (byte)(circleInfo.LineColor[0] * 255);
				byte green = (byte)(circleInfo.LineColor[1] * 255);
				byte blue = (byte)(circleInfo.LineColor[2] * 255);

				System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(newObject.Color.R, newObject.Color.G, newObject.Color.B));
				FastDrawCircle(gl, center, radius, circleInfo.Width, brush, circleInfo.IsFill);
			}
			if (newObject.Shape is PensInfo)
			{
				PensInfo pensInfo = newObject.Shape as PensInfo;

				// RGB 값을 0?�서 255 ?�이??값으�?변??
				byte red = (byte)(pensInfo.LineColor[0] * 255);
				byte green = (byte)(pensInfo.LineColor[1] * 255);
				byte blue = (byte)(pensInfo.LineColor[2] * 255);

				System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(newObject.Color.R, newObject.Color.G, newObject.Color.B));

				DrawShape(gl, pensInfo, newObject.Color, false, false);
			}
			if (newObject.Shape is OpenVisionLab.ImageCanvas.CanvasShapes.TextInfo)
			{
				OpenVisionLab.ImageCanvas.CanvasShapes.TextInfo textInfo = newObject.Shape as OpenVisionLab.ImageCanvas.CanvasShapes.TextInfo;

				float xSpan = textInfo.XSpan;
				float ySpan = textInfo.YSpan;
				SizeF offsetSize = textInfo.OffsetSize;
				float x = textInfo.TextPositionDot.X;
				float y = textInfo.TextPositionDot.Y;

				int r = (int)(textInfo.LineColor[0] * 255);
				int g = (int)(textInfo.LineColor[1] * 255);
				int b = (int)(textInfo.LineColor[2] * 255);

				System.Drawing.Color color = newObject.Color;

				string faceName = textInfo.FaceName;
				float baseFontSize = textInfo.BaseFontSize;
				if (textInfo.Text != null)
				{
					string text = textInfo.Text;

					//DrawTextOnImage(gl, textInfo.FontBitmapEntries, 12000,12000,  x, y, color, faceName, baseFontSize, text);
					DrawText(gl, textInfo.FontBitmapEntries, xSpan, ySpan, offsetSize, x, y, color, faceName, baseFontSize, text);
				}
			}

			gl.Disable(OpenGL.GL_LINES);
			gl.Disable(OpenGL.GL_LINE_STIPPLE); // ?�선 비활?�화
			gl.EndList();
		}

		public static void SetStencile(OpenGL gl)
		{
			gl.Enable(OpenGL.GL_STENCIL_TEST);
			gl.ClearStencil(0);
			gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
			gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);
		}

		/// <summary>
		/// ?�선 코드 보�? ?�중???�용
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="textureId"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		//private static void DrawDirectlyToTexture(OpenGL gl, uint textureId, int width, int height)
		//{
		//	// Viewport ?�정
		//	gl.Viewport(0, 0, width, height);

		//	// Projection ?�정
		//	gl.MatrixMode(OpenGL.GL_PROJECTION);
		//	gl.LoadIdentity();
		//	gl.Ortho2D(0, width, height, 0);  // Y축의 ?�작�??�을 반전

		//	// Modelview ?�정
		//	gl.MatrixMode(OpenGL.GL_MODELVIEW);
		//	gl.LoadIdentity();

		//	// ?�레?�버???�정
		//	uint[] frameBuffer = new uint[1];
		//	gl.GenFramebuffersEXT(1, frameBuffer);
		//	gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBuffer[0]);
		//	gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

		//	// ?�텐??첨�?�??�한 ?�더버퍼 ?�성
		//	uint[] renderBuffer = new uint[1];
		//	gl.GenRenderbuffersEXT(1, renderBuffer);
		//	gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);
		//	gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_STENCIL_INDEX8_EXT, width, height);

		//	// ?�레?�버?�에 ?�텐??첨�?
		//	gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_STENCIL_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT,
		//	Buffer[0]);


		//	// ?�텐???�스?��? ?�성?�합?�다.
		//	gl.Enable(OpenGL.GL_STENCIL_TEST);

		//	// ?�텐??버퍼�?초기?�합?�다.
		//	gl.ClearStencil(0);
		//	gl.Clear(OpenGL.GL_STENCIL_BUFFER_BIT);
		//	gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
		//	gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

		//	// 중앙???��? ?�각???�외???�역)??그립?�다.
		//	gl.ColorMask(0, 0, 0, 0); // ?�면???�을 그리지 ?�습?�다.
		//	gl.Color(System.Drawing.Color.Yellow.R / 255.0f, System.Drawing.Color.Yellow.G / 255.0f, System.Drawing.Color.Yellow.B / 255.0f);
		//	gl.Begin(OpenGL.GL_QUADS);
		//	gl.Vertex(50, 50);
		//	gl.Vertex(50, 60);
		//	gl.Vertex(60, 60);
		//	gl.Vertex(60, 50);
		//	gl.End();

		//	// ?�상 마스?��? ?�시 ?�성?�하???�면??그릴 ???�도�??�정?�니??
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

		//	// ?�텐???�스?��? 비활?�화?�니??
		//	gl.Disable(OpenGL.GL_STENCIL_TEST);

		//	// ?�레?�버???�제 �?�?��
		//	gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
		//	gl.DeleteFramebuffersEXT(1, frameBuffer);
		//}

		private static List<System.Drawing.PointF> ConvertDotInfoToPoints(List<DotInfo> listPoints)
		{
			List<System.Drawing.PointF> listDot = new List<System.Drawing.PointF>();
			foreach (var point in listPoints)
			{
				listDot.Add(new System.Drawing.PointF(point.X, point.Y));
			}
			return listDot;
		}

		//private static List<PointF> ConvertDotInfoToPoints(List<DotInfo> listDot)
		//{
		//	List<PointF> listPoints = new List<PointF>();
		//	foreach (DotInfo dot in listDot)
		//	{
		//		listPoints.Add(new PointF(dot.X, dot.Y));
		//	}
		//	return listPoints;
		//}

		private static void AllocatDisplayId(OpenGL gl, CanvasShape shape)
		{
			if (shape.DisplayListId != 0)
			{
				gl.DeleteLists(shape.DisplayListId, 1);
			}

			shape.DisplayListId = gl.GenLists(1); // Draw 객체마다 ID�?부?�합?�다.
		}

	}
}
