using OpenCvSharp;
using OpenVisionLab.ImageSpace.Core;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace OpenVisionLab.Core
{
    public static class DisplayManagerImageExtensions
    {
        private const int PlaceholderImageMaxSize = 10;
        private static long snapshotCopyCount;
        private static long snapshotEstimatedBytes;

        internal static ImageSpaceSnapshotDiagnostics SnapshotDiagnostics => new ImageSpaceSnapshotDiagnostics(
            Interlocked.Read(ref snapshotCopyCount),
            Interlocked.Read(ref snapshotEstimatedBytes));

        internal static void ResetSnapshotDiagnosticsForTest()
        {
            Interlocked.Exchange(ref snapshotCopyCount, 0);
            Interlocked.Exchange(ref snapshotEstimatedBytes, 0);
        }

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
            displayManager?.CreatePanel(ImageSpaceFrameAdapter.BorrowBitmap(image));
        }

        public static void CreateLayerDisplay(this IDisplayManager displayManager, Mat imageSource, string title, bool useClose = true)
        {
            if (imageSource == null || imageSource.Empty()) return;
            displayManager?.CreateLayerDisplay(ImageSpaceFrameAdapter.FromMat(imageSource), title, useClose);
        }

        public static void CreateLayerDisplay(this IDisplayManager displayManager, Bitmap imageSource, string title, bool useClose = true)
        {
            displayManager?.CreateLayerDisplay(ImageSpaceFrameAdapter.BorrowBitmap(imageSource), title, useClose);
        }

        public static Bitmap GetLayerImage(this IDisplayManager displayManager, string title)
        {
            return displayManager?.ImageSpace?.GetImage(title);
        }

        public static Bitmap GetLayerImage(this IDisplayManager displayManager, int index)
        {
            return displayManager?.ImageSpace?.GetImage(index);
        }

        /// <summary>
        /// Returns an owned Bitmap cloned while an ImageSpace lease is active.
        /// The caller owns and must dispose the returned snapshot.
        /// </summary>
        public static Bitmap GetLayerImageSnapshot(this IDisplayManager displayManager, string title)
        {
            using ImageSpaceImageLease lease = displayManager?.ImageSpace?.AcquireImage(title);
            return CloneLeasedImage(lease?.Image);
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

        private static Bitmap CloneLeasedImage(Bitmap image)
        {
            if (image == null)
            {
                return null;
            }

            Bitmap snapshot;
            try
            {
                snapshot = image.Clone(
                    new Rectangle(0, 0, image.Width, image.Height),
                    image.PixelFormat);
            }
            catch
            {
                snapshot = new Bitmap(image);
            }

            Interlocked.Increment(ref snapshotCopyCount);
            Interlocked.Add(ref snapshotEstimatedBytes, EstimateBytes(image));
            return snapshot;
        }

        private static long EstimateBytes(Bitmap image)
        {
            int bitsPerPixel = Image.GetPixelFormatSize(image.PixelFormat);
            long bytesPerPixel = Math.Max(1, (bitsPerPixel + 7) / 8);
            return checked((long)image.Width * image.Height * bytesPerPixel);
        }
    }

    internal sealed class ImageSpaceSnapshotDiagnostics
    {
        public ImageSpaceSnapshotDiagnostics(long copyCount, long estimatedBytes)
        {
            CopyCount = copyCount;
            EstimatedBytes = estimatedBytes;
        }

        public long CopyCount { get; }

        public long EstimatedBytes { get; }
    }
}
