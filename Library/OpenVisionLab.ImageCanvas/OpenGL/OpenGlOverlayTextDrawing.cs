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

				uint[] gtexture = new uint[1];
				gl.GenTextures(1, gtexture);
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, gtexture[0]);

				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);

				BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA, bitmap.Width, bitmap.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);
				bitmap.UnlockBits(data);

				return gtexture[0];
			}

			//SizeF textSize;
			//using (var tempBitmap = new Bitmap(1, 1))
			//{
			//	using (var g = Graphics.FromImage(tempBitmap))
			//	{
			//		textSize = g.MeasureString(text, font);
			//	}
			//}

			//using (Bitmap bitmap = new Bitmap((int)textSize.Width, (int)textSize.Height))
			//{
			//	using (Graphics graphics = Graphics.FromImage(bitmap))
			//	{
			//		graphics.Clear(Color.Transparent);

			//		using (Brush brush = new SolidBrush(textColor))
			//		{
			//			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			//			StringFormat stringFormat = new StringFormat();
			//			stringFormat.FormatFlags = StringFormatFlags.NoClip;
			//			//graphics.DrawString(text, font, brush, new PointF(0, 0), stringFormat);
			//			TextRenderer.DrawText(graphics, text, font, new Point(0, 0), textColor);
			//		}
			//	}
			//	bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

			//	uint[] gtexture = new uint[1];
			//	gl.GenTextures(1, gtexture);
			//	gl.BindTexture(OpenGL.GL_TEXTURE_2D, gtexture[0]);

			//	gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
			//	gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
			//	gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
			//	gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);

			//	BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			//	gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA, bitmap.Width, bitmap.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);
			//	bitmap.UnlockBits(data);

			//}
		}
	}
}
