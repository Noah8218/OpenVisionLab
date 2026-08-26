using OpenVisionLab.ImageCanvas.OpenCVSharp;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using OpenCvSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace OpenVisionLab.ImageCanvas
{
	public class CanvasImageLoader
	{
		public static BitmapSource GetImageSource(Bitmap bitmap)
		{
			BitmapSource img;
			IntPtr hBitmap;
			hBitmap = bitmap.GetHbitmap();
			img = Imaging.CreateBitmapSourceFromHBitmap(
				hBitmap,
				IntPtr.Zero,
				System.Windows.Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());
			return img;
		}

		public static void UploadMatAsTexture(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, OpenCvSharp.Mat mat, string imageName, ref System.Drawing.Size imageSize, bool zoomToFit = true)
		{
			if (imageViewer == null || mat == null || mat.Empty())
			{
				imageSize = System.Drawing.Size.Empty;
				return;
			}

			Mat textureMat = CreateTextureUploadMat(mat, out bool ownsTextureMat);
			try
			{
				using (imageViewer.SuppressRefresh())
				{
					imageViewer.DeleteTexture(imageName);
					//_imageViewer.ClearTexture();
					// 31800 * 96800 사이즈
					//  X,Y,W,H가 (0,0,31800,32768),  (0,32768,31800,32768), (0,64032,31800,31264)
					imageSize = new System.Drawing.Size(textureMat.Size().Width, textureMat.Size().Height);
					System.Drawing.Size maxSize = imageViewer.GetMaxTextureSize();
					//System.Drawing.Size maxSize = new System.Drawing.Size(10240, 10240);
					int tileWidth = 5000;
					int tileHeight = 5000;

					int offsetHeight = imageSize.Height;

					for (int actualY = 0; actualY < textureMat.Rows; actualY += tileHeight)
					{
						int actualTileHeight = Math.Min(tileHeight, textureMat.Rows - actualY);
						for (int actualX = 0; actualX < textureMat.Cols; actualX += tileWidth)
						{
							int actualTileWidth = Math.Min(tileWidth, textureMat.Cols - actualX);
							// 분할된 영역의 Mat 객체 생성
							OpenCvSharp.Rect tileRect = new OpenCvSharp.Rect(actualX, actualY, actualTileWidth, actualTileHeight);

							using (Mat tileMat = textureMat.SubMat(tileRect))
							using (Mat uploadTile = tileMat.Clone())
							{
								uint oriBpp = uploadTile.Channels() == 1 ? (uint)1 : (uint)3;
								System.Drawing.Size titleSize = new System.Drawing.Size(tileWidth, tileHeight);

								imageViewer.AddTexture(uploadTile.Data, actualX, actualY, actualTileWidth, actualTileHeight, uploadTile.Width, uploadTile.Height, offsetHeight, oriBpp,
									 imageName, imageSize, titleSize);
							}
						}
					}
				}
			}
			finally
			{
				if (ownsTextureMat)
				{
					textureMat.Dispose();
				}
			}
			if (zoomToFit)
			{
				imageViewer.ZoomToFit();
			}
		}

		public static void UploadBitmapAsTexture(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, Bitmap bitmap, string imageName, ref System.Drawing.Size imageSize, bool zoomToFit = true)
		{
			if (imageViewer == null || bitmap == null)
			{
				imageSize = System.Drawing.Size.Empty;
				return;
			}

			string textureName = string.IsNullOrWhiteSpace(imageName) ? "Image" : imageName;
			imageSize = new System.Drawing.Size(bitmap.Width, bitmap.Height);
			int tileWidth = 5000;
			int tileHeight = 5000;
			int offsetHeight = imageSize.Height;

			using (imageViewer.SuppressRefresh())
			{
				imageViewer.DeleteTexture(textureName);

				for (int actualY = 0; actualY < bitmap.Height; actualY += tileHeight)
				{
					int actualTileHeight = Math.Min(tileHeight, bitmap.Height - actualY);
					for (int actualX = 0; actualX < bitmap.Width; actualX += tileWidth)
					{
						int actualTileWidth = Math.Min(tileWidth, bitmap.Width - actualX);
						Rectangle tileRect = new Rectangle(actualX, actualY, actualTileWidth, actualTileHeight);
						byte[] tileBytes = CopyBitmapTile(bitmap, tileRect, out uint bpp);
						GCHandle pinned = GCHandle.Alloc(tileBytes, GCHandleType.Pinned);
						try
						{
							System.Drawing.Size titleSize = new System.Drawing.Size(tileWidth, tileHeight);
							imageViewer.AddTexture(
								pinned.AddrOfPinnedObject(),
								actualX,
								actualY,
								actualTileWidth,
								actualTileHeight,
								actualTileWidth,
								actualTileHeight,
								offsetHeight,
								bpp,
								textureName,
								imageSize,
								titleSize);
						}
						finally
						{
							pinned.Free();
						}
					}
				}
			}

			if (zoomToFit)
			{
				imageViewer.ZoomToFit();
			}
		}

		private static byte[] CopyBitmapTile(Bitmap bitmap, Rectangle tileRect, out uint bpp)
		{
			PixelFormat pixelFormat = bitmap.PixelFormat;
			if (pixelFormat == PixelFormat.Format8bppIndexed
				|| pixelFormat == PixelFormat.Format24bppRgb
				|| pixelFormat == PixelFormat.Format32bppArgb
				|| pixelFormat == PixelFormat.Format32bppPArgb
				|| pixelFormat == PixelFormat.Format32bppRgb)
			{
				bpp = GetBytesPerPixel(pixelFormat);
				return CopyLockedBitmapTile(bitmap, tileRect, pixelFormat, (int)bpp);
			}

			using Bitmap converted = new Bitmap(tileRect.Width, tileRect.Height, PixelFormat.Format24bppRgb);
			using (Graphics graphics = Graphics.FromImage(converted))
			{
				graphics.DrawImage(
					bitmap,
					new Rectangle(0, 0, tileRect.Width, tileRect.Height),
					tileRect,
					GraphicsUnit.Pixel);
			}

			bpp = 3;
			return CopyLockedBitmapTile(converted, new Rectangle(0, 0, converted.Width, converted.Height), PixelFormat.Format24bppRgb, 3);
		}

		private static byte[] CopyLockedBitmapTile(Bitmap bitmap, Rectangle tileRect, PixelFormat pixelFormat, int bytesPerPixel)
		{
			BitmapData data = bitmap.LockBits(tileRect, ImageLockMode.ReadOnly, pixelFormat);
			try
			{
				int rowBytes = tileRect.Width * bytesPerPixel;
				byte[] buffer = new byte[rowBytes * tileRect.Height];
				for (int y = 0; y < tileRect.Height; y++)
				{
					IntPtr sourceRow = data.Stride >= 0
						? IntPtr.Add(data.Scan0, y * data.Stride)
						: IntPtr.Add(data.Scan0, (tileRect.Height - 1 - y) * -data.Stride);
					Marshal.Copy(sourceRow, buffer, y * rowBytes, rowBytes);
				}

				return buffer;
			}
			finally
			{
				bitmap.UnlockBits(data);
			}
		}

		private static uint GetBytesPerPixel(PixelFormat pixelFormat)
		{
			return pixelFormat switch
			{
				PixelFormat.Format8bppIndexed => 1,
				PixelFormat.Format24bppRgb => 3,
				_ => 4
			};
		}

		private static Mat CreateTextureUploadMat(Mat mat, out bool ownsTextureMat)
		{
			int channels = mat.Channels();
			if (channels == 1 || channels == 3)
			{
				ownsTextureMat = false;
				return mat;
			}

			if (channels == 4)
			{
				Mat converted = new Mat();
				Cv2.CvtColor(mat, converted, ColorConversionCodes.BGRA2BGR);
				ownsTextureMat = true;
				return converted;
			}

			throw new NotSupportedException($"Unsupported image channel count for OpenGL texture upload: {channels}");
		}



		public void GetMatPointColor(Mat image, System.Drawing.Point point)
		{
			// 이미지의 높이와 너비를 확인합니다.
			int rows = image.Rows;
			int cols = image.Cols;

			// 이미지가 다중 채널 (예: BGR)을 갖는 경우 확인합니다.
			int channels = image.Channels();

			Vec3b color = image.At<Vec3b>((rows - 1) - point.Y, point.X); // BGR 색상을 읽습니다.
			byte blue = color[0];   // Blue 채널
			byte green = color[1];  // Green 채널
			byte red = color[2];    // Red 채널

			//Console.WriteLine($"Pixel at ({point.X},{(rows - 1) - point.Y}):  R={red}, G={green}, B={blue}");
		}

		public System.Drawing.Color[] GetMatColorArray(OpenCvSharp.Mat image)
		{
			// 이미지에서 픽셀 데이터를 바이트 배열로 직접 받습니다.
			byte[] buffer = image.ToBytes();

			int channels = image.Channels();
			int width = image.Cols;
			int height = image.Rows;
			System.Drawing.Color[] colors = new System.Drawing.Color[width * height];


			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					int index = (i * width + j) * channels;
					byte blue = buffer[index];
					byte green = buffer[index + 1];
					byte red = buffer[index + 2];

					// Color 배열에 색상 정보를 저장
					colors[i * width + j] = System.Drawing.Color.FromArgb(red, green, blue);

					// 콘솔에 색상 정보 출력 (선택적)
					//Console.WriteLine($"Pixel at ({i},{j}): B={blue}, G={green}, R={red}");
				}
			}

			return colors;
		}

		public static OpenCvSharp.Mat LoadMatFromFile(string path)
		{
			return Cv2.ImRead(path, ImreadModes.AnyColor);
		}

	}
}
