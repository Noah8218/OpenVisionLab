using OpenCvSharp;
using System.Diagnostics;
using System.Windows.Media;

namespace OpenVisionLab.ImageCanvas.OpenCVSharp
{
	public class CvUtill
	{
		public static bool SetImageChannel1(Mat image)
		{
			if (image.Channels() == 3) Cv2.CvtColor(image, image, ColorConversionCodes.RGB2GRAY);
			if (image.Channels() == 4) Cv2.CvtColor(image, image, ColorConversionCodes.RGBA2GRAY);

			return true;
		}

		public static bool SetImageChannel3(Mat image)
		{
			if (image.Channels() == 1) Cv2.CvtColor(image, image, ColorConversionCodes.GRAY2RGB);
			if (image.Channels() == 4) Cv2.CvtColor(image, image, ColorConversionCodes.RGBA2RGB);

			return true;
		}

		public static bool SetImageChannel4(Mat image)
		{
			if (image.Channels() == 1) Cv2.CvtColor(image, image, ColorConversionCodes.GRAY2RGBA);
			if (image.Channels() == 3) Cv2.CvtColor(image, image, ColorConversionCodes.RGB2RGBA);

			return true;
		}

		public static OpenCvSharp.Mat SetMaskColor(Mat mask, System.Drawing.Color color)
		{
			// 흰색 배경 고정
			Mat whiteBackground = Mat.Zeros(mask.Size(), MatType.CV_8UC4);
			whiteBackground.SetTo(new Scalar(255, 255, 255));

			// 흰색 배경에 원본 이미지 복사
			Mat maskRed = new Mat();
			Cv2.InRange(mask, new Scalar(0, 0, 0, 0), new Scalar(127, 127, 127, 255), maskRed);

			Mat red = new Mat(mask.Size(), MatType.CV_8UC4, ConvertColorToScalar(color));

			Mat result = new Mat();
			red.CopyTo(result, maskRed);
			whiteBackground.CopyTo(result, ~maskRed);

			return result;
		}

		public static Mat SetMaskColorInverse(Mat mask, System.Drawing.Color color)
		{
			// 흰색 배경 고정
			Mat BlackBackground = Mat.Zeros(mask.Size(), MatType.CV_8UC4);
			BlackBackground.SetTo(new Scalar(0, 0, 0));

			// 흰색 배경에 원본 이미지 복사
			Mat maskRed = new Mat();
			Cv2.InRange(mask, new Scalar(255, 255, 255, 255), new Scalar(127, 127, 127, 255), maskRed);

			Mat red = new Mat(mask.Size(), MatType.CV_8UC4, ConvertColorToScalar(color));

			Mat result = new Mat();
			red.CopyTo(result, maskRed);
			BlackBackground.CopyTo(result, ~maskRed);

			return result;
		}


		//public static OpenCvSharp.Mat SetDefectImageColor(Mat defectImage, System.Drawing.Color color)
		//{
		//	// 검은색 배경 고정
		//	Mat darkBackground = Mat.Zeros(defectImage.Size(), MatType.CV_8UC4);
		//	darkBackground.SetTo(new Scalar(0,0,0));

		//	// 검은색 배경에 원본 이미지 복사
		//	Mat maskRed = new Mat();
		//	Cv2.InRange(defectImage, new Scalar(127, 127, 127, 255), new Scalar(255, 255, 255, 255), maskRed);

		//	Mat setColor = new Mat(defectImage.Size(), MatType.CV_8UC4, ConvertColorToScalar(color));

		//	Mat result = new Mat();
		//	setColor.CopyTo(result, maskRed);
		//	darkBackground.CopyTo(result, ~maskRed);

		//	return result;
		//}

		public static OpenCvSharp.Mat SetDefectImageColor(OpenCvSharp.Mat defectImage, System.Drawing.Color color)
		{
			// 배경을 설정하여 초기화합니다.
			Mat result = Mat.Zeros(defectImage.Size(), MatType.CV_8UC4);
			Mat maskRed = new Mat();

			// 색상 범위 마스크를 생성합니다.
			Cv2.InRange(defectImage, new Scalar(127, 127, 127, 255), new Scalar(255, 255, 255, 255), maskRed);

			// 선택한 색상으로 설정된 이미지 복사
			result.SetTo(ConvertColorToScalar(color), maskRed);
			return result;
		}


		public static Mat SetRedChannelTo255(Mat img)
		{
			if (img.Channels() == 3 || img.Channels() == 4)
			{
				// 이미지를 3채널로 분리 (B, G, R)
				Mat[] channels = Cv2.Split(img);

				// R 채널 값이 높고 G와 B 채널 값이 낮은 영역만 마스크로 선택
				Mat mask = new Mat();
				Cv2.InRange(img, new Scalar(0, 0, 100), new Scalar(100, 100, 255), mask);

				// R 채널의 값이 마스크된 영역에서 255로 설정
				channels[2].SetTo(255, mask);

				// 채널을 다시 합쳐서 이미지로 만듦
				Cv2.Merge(channels, img);
			}

			return img;
		}

		public static bool IsImageEmpty(Mat image)
		{
			if (image == null)
			{
				//CLog.Write(CLog.EnumLogLevel.Abnormal, "Image is null");
				Debug.WriteLine("Image is null");
				return true;
			}

			if (image.IsDisposed)
			{
				//CLog.Write(CLog.EnumLogLevel.Abnormal, "Image Disposed");
				Debug.WriteLine("Image Disposed");
				return true;
			}

			if (image.Width == 0 || image.Height == 0)
			{
				//CLog.Write(CLog.EnumLogLevel.Abnormal, "Image Size Empty");
				Debug.WriteLine("Image Size Empty");
				return true;
			}

			return false;
		}

		public static System.Drawing.Color ConvertBrushToColor(SolidColorBrush brush)
		{
			// SolidColorBrush에서 Color 추출
			System.Windows.Media.Color mediaColor = brush.Color;

			// System.Windows.Media.Color를 System.Drawing.Color로 변환
			return System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
		}

		public static System.Windows.Media.SolidColorBrush ConvertToSolidColorBrush(System.Drawing.Color color)
		{
			// System.Drawing.Color to System.Windows.Media.Color
			System.Windows.Media.Color mediaColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);

			// Convert to SolidColorBrush
			return new System.Windows.Media.SolidColorBrush(mediaColor);
		}

		public static System.Windows.Media.SolidColorBrush ConvertToSolidColorBrush(System.Windows.Media.Color mediaColor)
		{		
			// Convert to SolidColorBrush
			return new System.Windows.Media.SolidColorBrush(mediaColor);
		}

		public static Scalar ConvertColorToScalar(System.Drawing.Color color)
		{
			// OpenCvSharp.Scalar uses the order (Blue, Green, Red, Alpha)
			return new Scalar(color.B, color.G, color.R, color.A);
		}
	}
}
