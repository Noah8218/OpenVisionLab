using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace OpenVisionLab
{
    internal static class WpfBitmapSourceFactory
    {
        public static Bitmap CloneCompatibleBitmap(Bitmap source)
        {
            if (source == null || source.Width <= 0 || source.Height <= 0)
            {
                return new Bitmap(10, 10, PixelFormat.Format24bppRgb);
            }

            return CloneRegion(source, new Rectangle(0, 0, source.Width, source.Height));
        }

        public static Bitmap CloneRegion(Bitmap source, Rectangle region)
        {
            if (source == null || source.Width <= 0 || source.Height <= 0)
            {
                return new Bitmap(10, 10, PixelFormat.Format24bppRgb);
            }

            Rectangle bounds = new Rectangle(0, 0, source.Width, source.Height);
            Rectangle clipped = Rectangle.Intersect(bounds, region);
            if (clipped.Width <= 0 || clipped.Height <= 0)
            {
                return new Bitmap(10, 10, PixelFormat.Format24bppRgb);
            }

            // The OpenGL ROI canvas is most reliable with opaque 3-channel upload data.
            // Normalizing editor sources to 24bpp also makes grayscale/indexed images visible.
            Bitmap copy = new Bitmap(clipped.Width, clipped.Height, PixelFormat.Format24bppRgb);
            if (TryDrawRegion(source, clipped, copy))
            {
                return copy;
            }

            TryCopyPixels(source, clipped, copy);
            return copy;
        }

        public static BitmapSource Create(Bitmap bitmap)
        {
            if (bitmap == null || bitmap.Width <= 0 || bitmap.Height <= 0)
            {
                return null;
            }

            try
            {
                using Bitmap compatible = CloneCompatibleBitmap(bitmap);
                return CreateFromCompatibleBitmap(compatible);
            }
            catch
            {
                using Bitmap fallback = new Bitmap(10, 10, PixelFormat.Format24bppRgb);
                using Graphics graphics = Graphics.FromImage(fallback);
                graphics.Clear(Color.Black);
                return CreateFromCompatibleBitmap(fallback);
            }
        }

        private static BitmapSource CreateFromCompatibleBitmap(Bitmap bitmap)
        {
            using MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Bmp);
            stream.Position = 0;

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            return image;
        }

        private static bool TryDrawRegion(Bitmap source, Rectangle sourceRegion, Bitmap destination)
        {
            try
            {
                using Graphics graphics = Graphics.FromImage(destination);
                graphics.DrawImage(
                    source,
                    new Rectangle(0, 0, destination.Width, destination.Height),
                    sourceRegion,
                    GraphicsUnit.Pixel);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void TryCopyPixels(Bitmap source, Rectangle sourceRegion, Bitmap destination)
        {
            try
            {
                for (int y = 0; y < destination.Height; y++)
                {
                    for (int x = 0; x < destination.Width; x++)
                    {
                        destination.SetPixel(x, y, source.GetPixel(sourceRegion.X + x, sourceRegion.Y + y));
                    }
                }
            }
            catch
            {
                using Graphics graphics = Graphics.FromImage(destination);
                graphics.Clear(Color.Black);
            }
        }
    }
}
