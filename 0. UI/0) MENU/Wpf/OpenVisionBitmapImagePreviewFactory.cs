using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace OpenVisionLab
{
    internal static class OpenVisionBitmapImagePreviewFactory
    {
        private const int MaxPreviewDimension = 2048;

        public static BitmapImage Create(Bitmap image)
        {
            if (image == null)
            {
                return null;
            }

            int longest = Math.Max(image.Width, image.Height);
            if (longest <= MaxPreviewDimension)
            {
                return CreateBitmapImage(image);
            }

            double scale = MaxPreviewDimension / (double)longest;
            int width = Math.Max(1, (int)Math.Round(image.Width * scale));
            int height = Math.Max(1, (int)Math.Round(image.Height * scale));
            using Bitmap preview = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(preview))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(image, new Rectangle(0, 0, width, height));
            }

            return CreateBitmapImage(preview);
        }

        private static BitmapImage CreateBitmapImage(Bitmap image)
        {
            try
            {
                using MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
            catch
            {
                return null;
            }
        }
    }
}
