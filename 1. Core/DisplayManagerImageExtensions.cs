using OpenCvSharp;
using System.Drawing;

namespace OpenVisionLab._1._Core
{
    public static class DisplayManagerImageExtensions
    {
        private const int PlaceholderImageMaxSize = 10;

        public static bool IsPlaceholderBitmap(Bitmap image)
        {
            if (image == null) return true;
            if (image.Width > PlaceholderImageMaxSize || image.Height > PlaceholderImageMaxSize) return false;

            try
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        Color pixel = image.GetPixel(x, y);
                        if (pixel.R > 2 || pixel.G > 2 || pixel.B > 2)
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static Mat GetImageSrc(this IDisplayManager displayManager)
        {
            Bitmap image = displayManager?.ImageSpace?.GetActiveImage();
            return ImageSpaceFrameAdapter.ToMat(image);
        }

        public static void SetImageSrc(this IDisplayManager displayManager, Mat image)
        {
            if (displayManager == null) return;
            if (image == null || image.Empty())
            {
                displayManager.ImageSpace.SetActiveImage(null);
                return;
            }

            displayManager.ImageSpace.SetActiveImage(ImageSpaceFrameAdapter.ToBitmap(image));
        }

        public static void SetImageSrc(this IDisplayManager displayManager, Bitmap image)
        {
            displayManager?.ImageSpace?.SetActiveImage(image);
        }

        public static void CreatePanel(this IDisplayManager displayManager, Bitmap image)
        {
            displayManager?.CreatePanel(ImageSpaceFrameAdapter.FromBitmap(image));
        }

        public static void CreateLayerDisplay(this IDisplayManager displayManager, Mat imageSource, string title, bool useClose = true)
        {
            if (imageSource == null || imageSource.Empty()) return;
            displayManager?.CreateLayerDisplay(ImageSpaceFrameAdapter.FromMat(imageSource), title, useClose);
        }

        public static void CreateLayerDisplay(this IDisplayManager displayManager, Bitmap imageSource, string title, bool useClose = true)
        {
            displayManager?.CreateLayerDisplay(ImageSpaceFrameAdapter.FromBitmap(imageSource), title, useClose);
        }

        public static Bitmap GetLayerImage(this IDisplayManager displayManager, string title)
        {
            return displayManager?.ImageSpace?.GetImage(title);
        }

        public static Bitmap GetLayerImage(this IDisplayManager displayManager, int index)
        {
            return displayManager?.ImageSpace?.GetImage(index);
        }

        public static Rectangle GetLayerRoi(this IDisplayManager displayManager, string title)
        {
            return displayManager?.ImageSpace?.GetRoi(title) ?? Rectangle.Empty;
        }

        public static Rectangle GetLayerRoi(this IDisplayManager displayManager, int index)
        {
            return displayManager?.ImageSpace?.GetRoi(index) ?? Rectangle.Empty;
        }

        public static Rectangle GetLayerTrainRoi(this IDisplayManager displayManager, string title)
        {
            return displayManager?.ImageSpace?.GetTrainRoi(title) ?? Rectangle.Empty;
        }

        public static Rectangle GetLayerTrainRoi(this IDisplayManager displayManager, int index)
        {
            return displayManager?.ImageSpace?.GetTrainRoi(index) ?? Rectangle.Empty;
        }

        public static bool IsLayerRoiEmpty(this IDisplayManager displayManager, string title)
        {
            return displayManager.GetLayerRoi(title).IsEmpty;
        }

        public static bool IsLayerRoiEmpty(this IDisplayManager displayManager, int index)
        {
            return displayManager.GetLayerRoi(index).IsEmpty;
        }

        public static void SetLayerImage(this IDisplayManager displayManager, int index, Bitmap image)
        {
            if (displayManager is DisplayManagerService service)
            {
                service.SetLayerImage(index, image);
                return;
            }

            string title = displayManager?.GetLayerTitle(index) ?? string.Empty;
            displayManager?.ImageSpace?.SetImage(index, title, image);
        }

        public static bool IsLayerImageChanged(this IDisplayManager displayManager, string title)
        {
            return displayManager?.ImageSpace?.IsImageChanged(title) ?? false;
        }

        public static void AcceptLayerImageChanged(this IDisplayManager displayManager, string title)
        {
            if (displayManager is DisplayManagerService service)
            {
                service.AcceptLayerImageChanged(title);
                return;
            }

            displayManager?.ImageSpace?.AcceptImageChanged(title);
        }
    }
}
