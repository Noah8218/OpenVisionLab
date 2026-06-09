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


		public static void DrawODBTexture(OpenGL gl, ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>> textureAreas, List<string> order)
		{
			gl.Enable(OpenGL.GL_TEXTURE_2D);

			foreach (var key in order.AsEnumerable().Reverse())
			{
				if (textureAreas.TryGetValue(key, out var drawingParams))
				{
					var groupedByImageName = drawingParams.GroupBy(param => param.ImageName);

					foreach (var group in groupedByImageName)
					{
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
									gl.PushMatrix();

									gl.Translate(centerX, centerY, 0);
									gl.Rotate(param.RotationAngle, 0, 0, 1);
									gl.Translate(-centerX, -centerY, 0);
								}

								gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
								gl.BindTexture(OpenGL.GL_TEXTURE_2D, param.OriTextureId);
								DrawQuad(gl, param.GLDrawingTextureArea);

								gl.Enable(OpenGL.GL_BLEND);
								gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

								gl.Color(1f, 1f, 1f, param.IsTransParency ? param.TransParency : 1.0f);
								gl.BindTexture(OpenGL.GL_TEXTURE_2D, param.OriBackgroundTextureId);
								DrawQuad(gl, param.GLDrawingTextureArea);

								gl.Disable(OpenGL.GL_BLEND);

								if (param.IsRotated)
								{
									gl.PopMatrix();
								}
							}
						}
					}
				}
			}
			gl.Disable(OpenGL.GL_TEXTURE_2D);
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
					var groupedByImageName = drawingParams.GroupBy(param => param.ImageName);

					foreach (var group in groupedByImageName)
					{
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
									gl.PushMatrix();

									gl.Translate(centerX, centerY, 0);
									gl.Rotate(param.RotationAngle, 0, 0, 1);
									gl.Translate(-centerX, -centerY, 0);
								}

								gl.BindTexture(OpenGL.GL_TEXTURE_2D, param.OriTextureId);
								DrawQuad(gl, param.GLDrawingTextureArea);

								if (param.IsRotated)
								{
									gl.PopMatrix();
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
			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);
			gl.Begin(OpenGL.GL_QUADS);

			gl.TexCoord(0.0f, 1.0f); gl.Vertex(drawArea.Left, drawArea.Top);
			gl.TexCoord(1.0f, 1.0f); gl.Vertex(drawArea.Right, drawArea.Top);
			gl.TexCoord(1.0f, 0.0f); gl.Vertex(drawArea.Right, drawArea.Bottom);
			gl.TexCoord(0.0f, 0.0f); gl.Vertex(drawArea.Left, drawArea.Bottom);

			gl.End();

			gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

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
	}
}
