using OpenVisionLab.ImageCanvas.OpenCVSharp;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using Emgu.CV.CvEnum;
using OpenCvSharp;
using System;
using System.Drawing;
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

		public static void UploadMatAsTexture(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, OpenCvSharp.Mat mat, string imageName, ref System.Drawing.Size imageSize)
		{
			imageViewer.DeleteTexture(imageName);
			//_imageViewer.ClearTexture();
			// 31800 * 96800 사이즈
			//  X,Y,W,H가 (0,0,31800,32768),  (0,32768,31800,32768), (0,64032,31800,31264)
			imageSize = new System.Drawing.Size(mat.Size().Width, mat.Size().Height);
			System.Drawing.Size maxSize = imageViewer.GetMaxTextureSize();
			//System.Drawing.Size maxSize = new System.Drawing.Size(10240, 10240);
			int tileWidth = 5000;
			int tileHeight = 5000;

			int offsetHeight = imageSize.Height;

			for (int actualY = 0; actualY < mat.Rows; actualY += tileHeight)
			{
				int actualTileHeight = Math.Min(tileHeight, mat.Rows - actualY);
				for (int actualX = 0; actualX < mat.Cols; actualX += tileWidth)
				{
					int actualTileWidth = Math.Min(tileWidth, mat.Cols - actualX);
					// 분할된 영역의 Mat 객체 생성						
					OpenCvSharp.Rect tileRect = new OpenCvSharp.Rect(actualX, actualY, actualTileWidth, actualTileHeight);

					using (Mat tileMat = mat.SubMat(tileRect))
					{
						uint oriBpp = tileMat.Type() == MatType.CV_8UC1 ? (uint)1 : (uint)3;
						System.Drawing.Size titleSize = new System.Drawing.Size(tileWidth, tileHeight);

						imageViewer.AddTexture(tileMat.Clone().Data, actualX, actualY, actualTileWidth, actualTileHeight, tileMat.Width, tileMat.Height, offsetHeight, oriBpp,
							 imageName, imageSize, titleSize);
					}
				}
			}
			imageViewer.ZoomToFit();
			GC.Collect();
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
			LoadImageType imReadMode = LoadImageType.Unchanged;
			Emgu.CV.Mat mat = new Emgu.CV.Mat(path, imReadMode);
			MatType cvMT = mat.NumberOfChannels == 1 ? OpenCvSharp.MatType.CV_8UC1 : OpenCvSharp.MatType.CV_8UC3;
			return new OpenCvSharp.Mat(mat.Rows, mat.Cols, cvMT, mat.DataPointer);
		}

	}
}
