using OpenCvSharp;
using SharpGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace OpenVisionLab.ImageCanvas
{
	public static class OpenGlRenderer
	{
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


		//public static void SetupFrameAndRenderBuffers(OpenGL gl, uint textureId, int width, int height, Action action)
		//{
		//	uint[] frameBuffer = new uint[1];
		//	gl.GenFramebuffersEXT(1, frameBuffer);
		//	gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBuffer[0]);
		//	gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

		//	uint[] renderBuffer = new uint[1];
		//	gl.GenRenderbuffersEXT(1, renderBuffer);
		//	gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);
		//	gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_STENCIL_INDEX8_EXT, width, height);
		//	gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_STENCIL_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);

		//	action();

		//	// 프레임버퍼 해제 및 청소
		//	gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
		//	gl.DeleteFramebuffersEXT(1, frameBuffer);
		//	gl.DeleteRenderbuffersEXT(1, renderBuffer); // 렌더버퍼 해제 추가
		//												//ReshapeNonRefrsh();
		//}

		public static void SetupFrameAndRenderBuffers(OpenGL gl, uint textureId, int width, int height, Action action)
		{
			uint[] frameBuffer = new uint[1];
			gl.GenFramebuffersEXT(1, frameBuffer);
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBuffer[0]);

			gl.FramebufferTexture2DEXT(
				OpenGL.GL_FRAMEBUFFER_EXT,
				OpenGL.GL_COLOR_ATTACHMENT0_EXT,
				OpenGL.GL_TEXTURE_2D,
				textureId,
				0);

			uint[] renderBuffer = new uint[1];
			gl.GenRenderbuffersEXT(1, renderBuffer);
			gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);
			gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_STENCIL_INDEX8_EXT, width, height);

			gl.FramebufferRenderbufferEXT(
				OpenGL.GL_FRAMEBUFFER_EXT,
				OpenGL.GL_STENCIL_ATTACHMENT_EXT,
				OpenGL.GL_RENDERBUFFER_EXT,
				renderBuffer[0]);

			uint status = gl.CheckFramebufferStatusEXT(OpenGL.GL_FRAMEBUFFER_EXT);
			if (status != OpenGL.GL_FRAMEBUFFER_COMPLETE_EXT)
			{
				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
				gl.DeleteFramebuffersEXT(1, frameBuffer);
				gl.DeleteRenderbuffersEXT(1, renderBuffer);

				throw new Exception($"FBO incomplete. Status = {status}");
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

			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
			gl.DeleteFramebuffersEXT(1, frameBuffer);
			gl.DeleteRenderbuffersEXT(1, renderBuffer);
		}


		public static void RestorePartTexture(OpenGL gl, uint textureId, uint backupTextureId, int imgWidth, int imgHeight, int imgX, int imgY, int width, int height)
		{
			// 이미지 좌표를 OpenGL 좌표로 변환
			int oglX = imgX;
			int oglY = imgHeight - (imgY + height);

			// 프레임버퍼 생성
			uint[] frameBuffer = new uint[1];
			gl.GenFramebuffersEXT(1, frameBuffer);
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBuffer[0]);

			// 복원할 텍스처를 프레임버퍼에 바인딩
			gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, backupTextureId, 0);

			// 복원할 텍스처를 바인딩
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

			// 백업 텍스처의 특정 영역을 복사하여 복원할 텍스처의 지정된 위치에 붙여넣기
			gl.CopyTexSubImage2D(OpenGL.GL_TEXTURE_2D, 0, oglX, oglY - 1, imgX, imgY + 1, width + 1, height + 1);

			// 텍스처 바인딩 해제
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

			// 프레임버퍼 해제 및 삭제
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
			gl.DeleteFramebuffersEXT(1, frameBuffer);
		}

		public static void BackupCurrentTexture(OpenGL gl, uint textureId, uint backupTextureId, int width, int height)
		{
			// 프레임버퍼 생성
			uint[] frameBuffer = new uint[1];
			gl.GenFramebuffersEXT(1, frameBuffer);
			//CheckGLError(openGLControl.OpenGL, "GenFramebuffersEXT");
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBuffer[0]);

			// 텍스처를 프레임버퍼에 바인딩
			gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

			// 백업 텍스처에 현재 텍스처의 내용을 복사
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, backupTextureId);
			gl.CopyTexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, 0, 0, width, height, 0);
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

			// 프레임버퍼 해제 및 삭제
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
			gl.DeleteFramebuffersEXT(1, frameBuffer);
		}

		public static void RestoreTexture(OpenGL gl, uint textureId, uint backupTextureId, int width, int height)
		{
			// 프레임버퍼 생성
			uint[] frameBuffer = new uint[1];
			gl.GenFramebuffersEXT(1, frameBuffer);
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBuffer[0]);

			// 복원할 텍스처를 프레임버퍼에 바인딩
			gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, backupTextureId, 0);

			// 백업된 텍스처를 복원할 텍스처로 복사
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);
			gl.CopyTexSubImage2D(OpenGL.GL_TEXTURE_2D, 0, 0, 0, 0, 0, width, height);
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

			// 프레임버퍼 해제 및 삭제
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
			gl.DeleteFramebuffersEXT(1, frameBuffer);
		}

		public static Bitmap TextureToBitmap(OpenGL gl, uint textureId, uint bpp)
		{
			// 텍스처 바인딩
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

			// 텍스처 파라미터 쿼리
			int[] widthArr = new int[1];
			int[] heightArr = new int[1];
			int[] formatArr = new int[1];
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_INTERNAL_FORMAT, formatArr);

			// 실제 사용할 너비와 높이
			int width = widthArr[0];
			int height = heightArr[0];

			int[] textureData = new int[width * height * 4]; // int 배열 생성 (나머지 무시)
			int padding = 4;
			int stride = (width * (int)bpp + padding - 1) & ~(padding - 1);
			stride = (stride * height) / 4;
			PixelFormat pixelFormat = PixelFormat.Format32bppArgb;
			if (bpp == 1)
			{
				pixelFormat = PixelFormat.Format8bppIndexed;
				gl.GetTexImage(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, textureData);
			}
			else if (bpp == 3)
			{
				pixelFormat = PixelFormat.Format24bppRgb;
				// 텍스처 이미지 데이터 가져오기
				gl.GetTexImage(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, textureData);
			}
			else if (bpp == 4)
			{
				pixelFormat = PixelFormat.Format32bppArgb;
				gl.GetTexImage(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, textureData);
			}

			// Bitmap 생성
			Bitmap bitmap = new Bitmap(width, height, pixelFormat);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, pixelFormat);

			// textureData의 내용을 bitmapData에 복사
			Marshal.Copy(textureData, 0, bitmapData.Scan0, stride);

			bitmap.UnlockBits(bitmapData);

			// 8비트 색상 팔레트 설정
			if (bpp == 1)
			{
				ColorPalette palette = bitmap.Palette;
				for (int i = 0; i < 256; i++)
				{
					palette.Entries[i] = Color.FromArgb(i, i, i);
				}
				bitmap.Palette = palette;
			}

			// 텍스처 바인딩 해제
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

			return bitmap;
		}

		public static OpenCvSharp.Mat TextureToMat(OpenGL gl, uint textureId, uint bpp)
		{		
			// FBO 설정
			uint[] fbo = new uint[1];
			gl.GenFramebuffersEXT(1, fbo);
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
			if (ptr != IntPtr.Zero)
			{
				Marshal.Copy(ptr, pixelData, 0, bufferSize);
				gl.UnmapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER);
			}

			MatType cvMT = bpp == 1 ? OpenCvSharp.MatType.CV_8UC1 : (bpp == 3 ? OpenCvSharp.MatType.CV_8UC3 : OpenCvSharp.MatType.CV_8UC4);
			Mat mat = new Mat(height, width, cvMT);

			for (int row = 0; row < height; row++)
			{
				int sourceIndex = row * paddedRowSize;
				int destIndex = row * width * bytesPerPixel;
				Marshal.Copy(pixelData, sourceIndex, mat.Data + destIndex, width * bytesPerPixel);
			}

			// PBO와 FBO 바인딩 해제 및 삭제
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, 0);
			gl.DeleteBuffers(1, pbo);
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
			gl.DeleteFramebuffersEXT(1, fbo);

			return mat;
		}

		//public static unsafe OpenCvSharp.Mat TextureToMat(OpenGL gl, uint textureId, uint bpp)
		//{
		//	// FBO 생성
		//	uint[] fbo = new uint[1];
		//	gl.GenFramebuffersEXT(1, fbo);
		//	gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, fbo[0]);

		//	// 텍스처를 FBO에 연결
		//	gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT,
		//							   OpenGL.GL_TEXTURE_2D, textureId, 0);

		//	gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

		//	// 텍스처 크기 얻기
		//	int[] widthArr = new int[1];
		//	int[] heightArr = new int[1];
		//	gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
		//	gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);

		//	int width = widthArr[0];
		//	int height = heightArr[0];

		//	// PBO 생성
		//	uint[] pbo = new uint[1];
		//	gl.GenBuffers(1, pbo);
		//	gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);

		//	int bytesPerPixel = bpp == 1 ? 1 : (bpp == 3 ? 3 : 4);
		//	int bufferSize = width * height * bytesPerPixel;

		//	// 버퍼 크기 예약
		//	gl.BufferData(OpenGL.GL_PIXEL_PACK_BUFFER, bufferSize, IntPtr.Zero, OpenGL.GL_STREAM_READ);

		//	// FBO → PBO로 데이터 읽기
		//	if (bpp == 1)
		//		gl.ReadPixels(0, 0, width, height, OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
		//	else if (bpp == 3)
		//		gl.ReadPixels(0, 0, width, height, OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
		//	else if (bpp == 4)
		//		gl.ReadPixels(0, 0, width, height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);

		//	// PBO 데이터 맵핑
		//	IntPtr ptr = gl.MapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, OpenGL.GL_READ_ONLY);
		//	if (ptr == IntPtr.Zero)
		//		throw new Exception("Failed to map buffer.");

		//	// OpenCV Mat 생성
		//	var matType = bpp == 1 ? OpenCvSharp.MatType.CV_8UC1
		//						   : (bpp == 3 ? OpenCvSharp.MatType.CV_8UC3
		//									   : OpenCvSharp.MatType.CV_8UC4);

		//	Mat mat = new Mat(height, width, matType);

		//	// GPU 메모리 → Mat 메모리 직접 복사
		//	Buffer.MemoryCopy((void*)ptr, (void*)mat.Data, bufferSize, bufferSize);

		//	// 해제
		//	gl.UnmapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER);
		//	gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, 0);
		//	gl.DeleteBuffers(1, pbo);

		//	gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
		//	gl.DeleteFramebuffersEXT(1, fbo);

		//	return mat;
		//}



		public static Bitmap RenderTextureToBitmap(OpenGL gl, uint textureId, uint texturebBpp, uint displayBpp, Action action)
		{
			Bitmap bmp = TextureToBitmap(gl, textureId, texturebBpp);

			InitializeOpenGLSettings(gl, bmp.Width, bmp.Height);

			// FBO 설정
			uint[] ids = new uint[1];
			gl.GenFramebuffersEXT(1, ids);
			uint frameBufferID = ids[0];
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBufferID);
			gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

			// 스텐실 첨부를 위한 렌더버퍼 생성
			uint[] renderBuffer = new uint[1];
			gl.GenRenderbuffersEXT(1, renderBuffer);
			gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);
			gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_STENCIL_INDEX8_EXT, bmp.Width, bmp.Height);

			// 프레임버퍼에 스텐실 첨부
			gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_STENCIL_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);

			uint[] gtexture = new uint[1];
			gtexture[0] = GenerateOpenGLTexture(gl, bmp.Width, bmp.Height, texturebBpp);

			//	Get the maximum texture size supported by OpenGL.
			int[] textureMaxSize = { 0 };
			gl.GetInteger(OpenGL.GL_MAX_TEXTURE_SIZE, textureMaxSize);

			//  Ensure that the image does not exceed the maximum texture size.
			if (bmp.Width > textureMaxSize[0] || bmp.Height > textureMaxSize[0])
			{
				throw new InvalidOperationException("Image exceeds the maximum texture size.");
			}

			//	Bind our texture object (make it the current texture).
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, gtexture[0]);
			gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, gtexture[0], 0);

			//  Lock the image bits (so that we can pass them to OGL).
			BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
				ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

			//  Set the image data.
			gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA,
				bmp.Width, bmp.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE,
				bitmapData.Scan0);

			//  Unlock the image.
			bmp.UnlockBits(bitmapData);

			//  Set linear filtering mode.
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MIN_FILTER, SharpGL.OpenGL.GL_LINEAR);
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MAG_FILTER, SharpGL.OpenGL.GL_NEAREST);

			action();

			Bitmap copy = TextureToBitmap(gl, gtexture[0], displayBpp);

			// 자원 해제
			gl.DeleteTextures(1, gtexture);
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
			gl.DeleteFramebuffersEXT(1, ids);

			return copy;
		}

		public static uint GenerateOpenGLTexture(OpenGL gl, int width, int height, uint bpp)
		{
			uint[] gtexture = new uint[1];

			gl.GenTextures(1, gtexture); // 텍스처 ID를 생성
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, gtexture[0]); // 생성된 ID를 현재 작업중인 텍스처로 설정

			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);

			// 텍스처 경계 처리 설정
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);

			gl.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);

			if (bpp == 3)
			{
				// for Color
				gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB, width, height, 0, OpenGL.GL_RGB, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
			}
			else if (bpp == 4)
			{
				// for Color
				gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, width, height, 0, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
			}
			else if (bpp == 1)
			{
				// for Mono
				gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_LUMINANCE, width, height, 0, OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
			}

			gl.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);

			int[] widthArr = new int[1];
			int[] heightArr = new int[1];
			int[] formatArr = new int[1];
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_INTERNAL_FORMAT, formatArr);

			gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

			return gtexture[0];
		}

	}
}
