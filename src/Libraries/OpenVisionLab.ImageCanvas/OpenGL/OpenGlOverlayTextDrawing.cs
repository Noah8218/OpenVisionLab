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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace OpenVisionLab.ImageCanvas.OpenGLRendering
{
	public static partial class OpenGlDrawing
	{		public static void DrawGroupName(OpenGL gl, CanvasOverlayManager overlayManager, OpenGlTextDrawOptions glDrawTextOptions)
		{
			foreach (var overlayItem in overlayManager.GetAllVisibleOverlays())
			{
				if (!overlayItem.IsGroupRectangle) { continue; }

				if ((overlayItem.Shape as CanvasRect<float>).Width == 0 || (overlayItem.Shape as CanvasRect<float>).Height == 0) { continue; }
				float midX = (overlayItem.Shape as CanvasRect<float>).LeftTop.X;
				float midY = (overlayItem.Shape as CanvasRect<float>).LeftTop.Y + 20;

				string faceName = "Arial";
				float fontSize = 15;

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

				string faceName = "Arial";
				float fontSize = 12;

				DrawText(gl, glDrawTextOptions.FontBitmapEntries, glDrawTextOptions.XSpan, glDrawTextOptions.YSpan, glDrawTextOptions.OffsetSize, (int)midX, (int)midY, overlayItem.Color, faceName, fontSize, $"{index}");
				index++;
			}
		}

		public static uint CreateTextTexture(OpenGL gl, string text, Font font, System.Drawing.Color textColor)
		{
			SizeF textSize;
			using (var tempBitmap = new Bitmap(1, 1))
			{
				using (var g = Graphics.FromImage(tempBitmap))
				{
					textSize = g.MeasureString(text, font);
				}
			}

			int width = (int)Math.Ceiling(textSize.Width);
			int height = (int)Math.Ceiling(font.GetHeight());

			using (Bitmap bitmap = new Bitmap(width, height))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.Clear(System.Drawing.Color.Transparent);

					using (System.Drawing.Brush brush = new SolidBrush(textColor))
					{
						PointF position = new PointF(0, 0);
						graphics.DrawString(text, font, brush, position);
					}
				}
				bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

				uint textureId = 0;
				bool succeeded = false;
				BitmapData data = null;
				bool bitmapLocked = false;
				try
				{
					uint[] gtexture = new uint[1];
					gl.GenTextures(1, gtexture);
					textureId = gtexture[0];
					if (textureId == 0) { throw new InvalidOperationException("OpenGL could not allocate the text texture."); }
					gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

					gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
					gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
					gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
					gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);

					data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					bitmapLocked = true;
					gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, bitmap.Width, bitmap.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);
					bitmap.UnlockBits(data);
					bitmapLocked = false;
					data = null;

					succeeded = true;
					return textureId;
				}
				finally
				{
					if (bitmapLocked && data != null)
					{
						try { bitmap.UnlockBits(data); } catch (Exception exception) { Trace.TraceWarning("OpenGL text bitmap unlock failed: {0}", exception); }
					}
					try { gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0); } catch (Exception exception) { Trace.TraceWarning("OpenGL text texture unbind failed: {0}", exception); }
					if (!succeeded && textureId != 0)
					{
						uint[] texture = { textureId };
						try { gl.DeleteTextures(1, texture); } catch (Exception exception) { Trace.TraceWarning("OpenGL text texture deletion failed: {0}", exception); }
					}
				}
			}
		}
	}
}
