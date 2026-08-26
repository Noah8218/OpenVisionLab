using OpenCvSharp;
using SharpGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OpenVisionLab.ImageCanvas
{
	public static class OpenGlRenderer
	{
		private static void TryCleanup(string resourceName, Action cleanup)
		{
			try
			{
				cleanup();
			}
			catch (Exception exception)
			{
				Trace.TraceWarning("OpenGL cleanup failed for {0}: {1}", resourceName, exception);
			}
		}

		public static void InitializeOpenGLSettings(OpenGL gl, int width, int height)
		{
			// Viewport 설정
			gl.Viewport(0, 0, width, height);

			// Projection 설정
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.LoadIdentity();
			gl.Ortho2D(0, width, height, 0);  // Y축의 시작과 끝을 반전

			// Modelview 설정
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.LoadIdentity();
		}

		public static void SetupFrameAndRenderBuffers(OpenGL gl, uint textureId, int width, int height, Action action)
		{
			if (action == null) { throw new ArgumentNullException(nameof(action)); }

			uint frameBufferId = 0;
			uint renderBufferId = 0;
			try
			{
				uint[] frameBuffer = new uint[1];
				gl.GenFramebuffersEXT(1, frameBuffer);
				frameBufferId = frameBuffer[0];
				if (frameBufferId == 0) { throw new InvalidOperationException("OpenGL could not allocate the frame buffer."); }
				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBufferId);

				gl.FramebufferTexture2DEXT(
					OpenGL.GL_FRAMEBUFFER_EXT,
					OpenGL.GL_COLOR_ATTACHMENT0_EXT,
					OpenGL.GL_TEXTURE_2D,
					textureId,
					0);

				uint[] renderBuffer = new uint[1];
				gl.GenRenderbuffersEXT(1, renderBuffer);
				renderBufferId = renderBuffer[0];
				if (renderBufferId == 0) { throw new InvalidOperationException("OpenGL could not allocate the render buffer."); }
				gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, renderBufferId);
				gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_STENCIL_INDEX8_EXT, width, height);

				gl.FramebufferRenderbufferEXT(
					OpenGL.GL_FRAMEBUFFER_EXT,
					OpenGL.GL_STENCIL_ATTACHMENT_EXT,
					OpenGL.GL_RENDERBUFFER_EXT,
					renderBufferId);

				uint status = gl.CheckFramebufferStatusEXT(OpenGL.GL_FRAMEBUFFER_EXT);
				if (status != OpenGL.GL_FRAMEBUFFER_COMPLETE_EXT)
				{
					throw new InvalidOperationException($"FBO incomplete. Status = {status}");
				}

				gl.Viewport(0, 0, width, height);
				gl.MatrixMode(OpenGL.GL_PROJECTION);
				gl.LoadIdentity();
				gl.Ortho2D(0, width, height, 0);
				gl.MatrixMode(OpenGL.GL_MODELVIEW);
				gl.LoadIdentity();

				action();
				gl.Flush();
				gl.Finish();
			}
			finally
			{
				TryCleanup("render-buffer binding", () => gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, 0));
				TryCleanup("frame-buffer binding", () => gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0));
				if (frameBufferId != 0)
				{
					uint[] frameBuffer = { frameBufferId };
					TryCleanup("frame-buffer deletion", () => gl.DeleteFramebuffersEXT(1, frameBuffer));
				}
				if (renderBufferId != 0)
				{
					uint[] renderBuffer = { renderBufferId };
					TryCleanup("render-buffer deletion", () => gl.DeleteRenderbuffersEXT(1, renderBuffer));
				}
			}
		}


		public static void RestorePartTexture(OpenGL gl, uint textureId, uint backupTextureId, int imgWidth, int imgHeight, int imgX, int imgY, int width, int height)
		{
			if (imgWidth <= 0) { throw new ArgumentOutOfRangeException(nameof(imgWidth)); }
			if (imgHeight <= 0) { throw new ArgumentOutOfRangeException(nameof(imgHeight)); }
			if (imgX < 0 || imgY < 0 || width <= 0 || height <= 0 || imgX > imgWidth - width || imgY > imgHeight - height)
			{
				throw new ArgumentOutOfRangeException(nameof(width), "The restore region must be a non-empty half-open image region.");
			}

			uint frameBufferId = 0;
			try
			{
				uint[] frameBuffer = new uint[1];
				gl.GenFramebuffersEXT(1, frameBuffer);
				frameBufferId = frameBuffer[0];
				if (frameBufferId == 0) { throw new InvalidOperationException("OpenGL could not allocate the restore frame buffer."); }
				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBufferId);
				gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, backupTextureId, 0);
				uint status = gl.CheckFramebufferStatusEXT(OpenGL.GL_FRAMEBUFFER_EXT);
				if (status != OpenGL.GL_FRAMEBUFFER_COMPLETE_EXT)
				{
					throw new InvalidOperationException($"Restore FBO incomplete. Status = {status}");
				}

				gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);
				int lowerLeftY = imgHeight - imgY - height;
				gl.CopyTexSubImage2D(OpenGL.GL_TEXTURE_2D, 0, imgX, lowerLeftY, imgX, lowerLeftY, width, height);
			}
			finally
			{
				TryCleanup("restore texture binding", () => gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0));
				TryCleanup("restore frame-buffer binding", () => gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0));
				if (frameBufferId != 0)
				{
					uint[] frameBuffer = { frameBufferId };
					TryCleanup("restore frame-buffer deletion", () => gl.DeleteFramebuffersEXT(1, frameBuffer));
				}
			}
		}

		public static void BackupCurrentTexture(OpenGL gl, uint textureId, uint backupTextureId, int width, int height)
		{
			uint frameBufferId = 0;
			try
			{
				uint[] frameBuffer = new uint[1];
				gl.GenFramebuffersEXT(1, frameBuffer);
				frameBufferId = frameBuffer[0];
				if (frameBufferId == 0) { throw new InvalidOperationException("OpenGL could not allocate the backup frame buffer."); }
				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBufferId);
				gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);
				uint status = gl.CheckFramebufferStatusEXT(OpenGL.GL_FRAMEBUFFER_EXT);
				if (status != OpenGL.GL_FRAMEBUFFER_COMPLETE_EXT)
				{
					throw new InvalidOperationException($"Backup FBO incomplete. Status = {status}");
				}
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, backupTextureId);
				gl.CopyTexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, 0, 0, width, height, 0);
			}
			finally
			{
				TryCleanup("backup texture binding", () => gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0));
				TryCleanup("backup frame-buffer binding", () => gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0));
				if (frameBufferId != 0)
				{
					uint[] frameBuffer = { frameBufferId };
					TryCleanup("backup frame-buffer deletion", () => gl.DeleteFramebuffersEXT(1, frameBuffer));
				}
			}
		}

		public static void RestoreTexture(OpenGL gl, uint textureId, uint backupTextureId, int width, int height)
		{
			uint frameBufferId = 0;
			try
			{
				uint[] frameBuffer = new uint[1];
				gl.GenFramebuffersEXT(1, frameBuffer);
				frameBufferId = frameBuffer[0];
				if (frameBufferId == 0) { throw new InvalidOperationException("OpenGL could not allocate the restore frame buffer."); }
				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBufferId);
				gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, backupTextureId, 0);
				uint status = gl.CheckFramebufferStatusEXT(OpenGL.GL_FRAMEBUFFER_EXT);
				if (status != OpenGL.GL_FRAMEBUFFER_COMPLETE_EXT)
				{
					throw new InvalidOperationException($"Restore FBO incomplete. Status = {status}");
				}
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);
				gl.CopyTexSubImage2D(OpenGL.GL_TEXTURE_2D, 0, 0, 0, 0, 0, width, height);
			}
			finally
			{
				TryCleanup("restore texture binding", () => gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0));
				TryCleanup("restore frame-buffer binding", () => gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0));
				if (frameBufferId != 0)
				{
					uint[] frameBuffer = { frameBufferId };
					TryCleanup("restore frame-buffer deletion", () => gl.DeleteFramebuffersEXT(1, frameBuffer));
				}
			}
		}

		public static Bitmap TextureToBitmap(OpenGL gl, uint textureId, uint bpp)
		{
			if (bpp != 1 && bpp != 3 && bpp != 4) { throw new ArgumentOutOfRangeException(nameof(bpp)); }

			Bitmap bitmap = null;
			BitmapData bitmapData = null;
			bool bitmapLocked = false;
			bool returned = false;
			try
			{
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

				int[] widthArr = new int[1];
				int[] heightArr = new int[1];
				gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
				gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);
				int width = widthArr[0];
				int height = heightArr[0];
				if (width <= 0 || height <= 0) { throw new InvalidOperationException("OpenGL texture dimensions are invalid."); }

				int rowBytes = (width * (int)bpp + 3) & ~3;
				int byteCount = rowBytes * height;
				int[] textureData = new int[(byteCount + sizeof(int) - 1) / sizeof(int)];
				PixelFormat pixelFormat = PixelFormat.Format32bppArgb;
				if (bpp == 1)
				{
					pixelFormat = PixelFormat.Format8bppIndexed;
					gl.GetTexImage(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, textureData);
				}
				else if (bpp == 3)
				{
					pixelFormat = PixelFormat.Format24bppRgb;
					gl.GetTexImage(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, textureData);
				}
				else
				{
					gl.GetTexImage(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, textureData);
				}

				bitmap = new Bitmap(width, height, pixelFormat);
				bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, pixelFormat);
				bitmapLocked = true;
				int rowInts = rowBytes / sizeof(int);
				for (int row = 0; row < height; row++)
				{
					int sourceRow = height - 1 - row;
					Marshal.Copy(
						textureData,
						sourceRow * rowInts,
						IntPtr.Add(bitmapData.Scan0, row * bitmapData.Stride),
						rowInts);
				}
				bitmap.UnlockBits(bitmapData);
				bitmapLocked = false;
				bitmapData = null;

				if (bpp == 1)
				{
					ColorPalette palette = bitmap.Palette;
					for (int i = 0; i < 256; i++)
					{
						palette.Entries[i] = Color.FromArgb(i, i, i);
					}
					bitmap.Palette = palette;
				}

				returned = true;
				return bitmap;
			}
			finally
			{
				if (bitmapLocked && bitmapData != null && bitmap != null)
				{
					TryCleanup("bitmap unlock", () => bitmap.UnlockBits(bitmapData));
				}
				TryCleanup("texture binding", () => gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0));
				if (!returned && bitmap != null)
				{
					TryCleanup("bitmap disposal", () => bitmap.Dispose());
				}
			}
		}

		public static OpenCvSharp.Mat TextureToMat(OpenGL gl, uint textureId, uint bpp)
		{		
			if (bpp != 1 && bpp != 3 && bpp != 4) { throw new ArgumentOutOfRangeException(nameof(bpp)); }
			uint frameBufferId = 0;
			uint pboId = 0;
			bool mapped = false;
			OpenCvSharp.Mat mat = null;
			bool returned = false;
			try
			{
			// FBO 설정
			uint[] fbo = new uint[1];
			gl.GenFramebuffersEXT(1, fbo);
			frameBufferId = fbo[0];
			if (frameBufferId == 0) { throw new InvalidOperationException("OpenGL could not allocate the readback frame buffer."); }
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, fbo[0]);

			// 텍스처를 FBO에 연결
			gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);
			int[] widthArr = new int[1];
			int[] heightArr = new int[1];
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);

			// 실제 사용할 너비와 높이
			int width = widthArr[0];
			int height = heightArr[0];

			// PBO 설정
			uint[] pbo = new uint[1];
			gl.GenBuffers(1, pbo);
			pboId = pbo[0];
			if (pboId == 0) { throw new InvalidOperationException("OpenGL could not allocate the readback PBO."); }
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
			int bytesPerPixel = bpp == 1 ? 1 : (bpp == 3 ? 3 : 4);
			int paddedRowSize = (width * bytesPerPixel + 3) & ~3; // 4의 배수로 맞추기
			int bufferSize = paddedRowSize * height; // 패딩을 포함한 전체 버퍼 크기
			gl.BufferData(OpenGL.GL_PIXEL_PACK_BUFFER, bufferSize, IntPtr.Zero, OpenGL.GL_STREAM_READ);


			// FBO에서 PBO로 픽셀 데이터 읽기
			if (bpp == 1)
			{
				gl.ReadPixels(0, 0, width, height, OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
			}
			else if (bpp == 3)
			{
				gl.ReadPixels(0, 0, width, height, OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
			}
			else if (bpp == 4)
			{
				gl.ReadPixels(0, 0, width, height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
			}

			// GPU로부터 데이터를 받아오기
			byte[] pixelData = new byte[bufferSize];
			IntPtr ptr = gl.MapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, OpenGL.GL_READ_ONLY);
			if (ptr == IntPtr.Zero) { throw new InvalidOperationException("OpenGL returned a null readback mapping."); }
			mapped = true;
			Marshal.Copy(ptr, pixelData, 0, bufferSize);
			gl.UnmapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER);
			mapped = false;

			MatType cvMT = bpp == 1 ? OpenCvSharp.MatType.CV_8UC1 : (bpp == 3 ? OpenCvSharp.MatType.CV_8UC3 : OpenCvSharp.MatType.CV_8UC4);
			mat = new Mat(height, width, cvMT);

				for (int row = 0; row < height; row++)
				{
					int sourceIndex = row * paddedRowSize;
					int destIndex = (height - 1 - row) * width * bytesPerPixel;
					Marshal.Copy(pixelData, sourceIndex, mat.Data + destIndex, width * bytesPerPixel);
				}

			returned = true;
			return mat;
			}
			finally
			{
				if (mapped)
				{
					TryCleanup("readback PBO unmap", () => gl.UnmapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER));
				}
				TryCleanup("readback PBO binding", () => gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, 0));
				if (pboId != 0)
				{
					uint[] pbo = { pboId };
					TryCleanup("readback PBO deletion", () => gl.DeleteBuffers(1, pbo));
				}
				TryCleanup("readback texture binding", () => gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0));
				TryCleanup("readback frame-buffer binding", () => gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0));
				if (frameBufferId != 0)
				{
					uint[] fbo = { frameBufferId };
					TryCleanup("readback frame-buffer deletion", () => gl.DeleteFramebuffersEXT(1, fbo));
				}
				if (!returned && mat != null)
				{
					TryCleanup("readback Mat disposal", () => mat.Dispose());
				}
			}
		}
		public static Bitmap RenderTextureToBitmap(OpenGL gl, uint textureId, uint texturebBpp, uint displayBpp, Action action)
		{
			if (action == null) { throw new ArgumentNullException(nameof(action)); }
			Bitmap bmp = null;
			BitmapData bitmapData = null;
			bool bitmapLocked = false;
			uint frameBufferId = 0;
			uint renderBufferId = 0;
			uint textureIdForRender = 0;
			try
			{
				bmp = TextureToBitmap(gl, textureId, texturebBpp);
				InitializeOpenGLSettings(gl, bmp.Width, bmp.Height);

				int[] textureMaxSize = { 0 };
				gl.GetInteger(OpenGL.GL_MAX_TEXTURE_SIZE, textureMaxSize);
				if (bmp.Width > textureMaxSize[0] || bmp.Height > textureMaxSize[0])
				{
					throw new InvalidOperationException("Image exceeds the maximum texture size.");
				}

				uint[] ids = new uint[1];
				gl.GenFramebuffersEXT(1, ids);
				frameBufferId = ids[0];
				if (frameBufferId == 0) { throw new InvalidOperationException("OpenGL could not allocate the render frame buffer."); }
				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBufferId);
				gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

				uint[] renderBuffer = new uint[1];
				gl.GenRenderbuffersEXT(1, renderBuffer);
				renderBufferId = renderBuffer[0];
				if (renderBufferId == 0) { throw new InvalidOperationException("OpenGL could not allocate the render stencil buffer."); }
				gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, renderBufferId);
				gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_STENCIL_INDEX8_EXT, bmp.Width, bmp.Height);
				gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_STENCIL_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, renderBufferId);

				textureIdForRender = GenerateOpenGLTexture(gl, bmp.Width, bmp.Height, texturebBpp);
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureIdForRender);
				gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureIdForRender, 0);
				uint status = gl.CheckFramebufferStatusEXT(OpenGL.GL_FRAMEBUFFER_EXT);
				if (status != OpenGL.GL_FRAMEBUFFER_COMPLETE_EXT)
				{
					throw new InvalidOperationException($"Render FBO incomplete. Status = {status}");
				}

				PixelFormat uploadPixelFormat = bmp.PixelFormat;
				uint uploadFormat = texturebBpp == 1 ? OpenGL.GL_LUMINANCE : (texturebBpp == 3 ? OpenGL.GL_BGR : OpenGL.GL_BGRA);
				uint uploadInternalFormat = texturebBpp == 1 ? OpenGL.GL_LUMINANCE : (texturebBpp == 3 ? OpenGL.GL_RGB : OpenGL.GL_RGBA);
				bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, uploadPixelFormat);
				bitmapLocked = true;
				gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, uploadInternalFormat, bmp.Width, bmp.Height, 0, uploadFormat, OpenGL.GL_UNSIGNED_BYTE, bitmapData.Scan0);
				bmp.UnlockBits(bitmapData);
				bitmapLocked = false;
				bitmapData = null;

				gl.TexParameter(OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MIN_FILTER, SharpGL.OpenGL.GL_LINEAR);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MAG_FILTER, SharpGL.OpenGL.GL_NEAREST);
				action();
				return TextureToBitmap(gl, textureIdForRender, displayBpp);
			}
			finally
			{
				if (bitmapLocked && bitmapData != null && bmp != null)
				{
					TryCleanup("render bitmap unlock", () => bmp.UnlockBits(bitmapData));
				}
				TryCleanup("render texture binding", () => gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0));
				if (textureIdForRender != 0)
				{
					uint[] texture = { textureIdForRender };
					TryCleanup("render texture deletion", () => gl.DeleteTextures(1, texture));
				}
				TryCleanup("render stencil binding", () => gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, 0));
				if (renderBufferId != 0)
				{
					uint[] renderBuffer = { renderBufferId };
					TryCleanup("render stencil deletion", () => gl.DeleteRenderbuffersEXT(1, renderBuffer));
				}
				TryCleanup("render frame-buffer binding", () => gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0));
				if (frameBufferId != 0)
				{
					uint[] frameBuffer = { frameBufferId };
					TryCleanup("render frame-buffer deletion", () => gl.DeleteFramebuffersEXT(1, frameBuffer));
				}
				if (bmp != null)
				{
					TryCleanup("render bitmap disposal", () => bmp.Dispose());
				}
			}
		}

		public static uint GenerateOpenGLTexture(OpenGL gl, int width, int height, uint bpp)
		{
			if (width <= 0 || height <= 0) { throw new ArgumentOutOfRangeException(nameof(width)); }
			if (bpp != 1 && bpp != 3 && bpp != 4) { throw new ArgumentOutOfRangeException(nameof(bpp)); }

			uint textureId = 0;
			bool succeeded = false;
			try
			{
				uint[] gtexture = new uint[1];
				gl.GenTextures(1, gtexture);
				textureId = gtexture[0];
				if (textureId == 0) { throw new InvalidOperationException("OpenGL could not allocate the texture."); }
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
				gl.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);

				if (bpp == 3)
				{
					gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB, width, height, 0, OpenGL.GL_RGB, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
				}
				else if (bpp == 4)
				{
					gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, width, height, 0, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
				}
				else
				{
					gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_LUMINANCE, width, height, 0, OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
				}

				int[] widthArr = new int[1];
				int[] heightArr = new int[1];
				gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
				gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);
				if (widthArr[0] != width || heightArr[0] != height)
				{
					throw new InvalidOperationException("OpenGL texture allocation returned unexpected dimensions.");
				}

				succeeded = true;
				return textureId;
			}
			finally
			{
				TryCleanup("generated texture binding", () => gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0));
				if (!succeeded && textureId != 0)
				{
					uint[] texture = { textureId };
					TryCleanup("generated texture deletion", () => gl.DeleteTextures(1, texture));
				}
			}
		}

	}
}
