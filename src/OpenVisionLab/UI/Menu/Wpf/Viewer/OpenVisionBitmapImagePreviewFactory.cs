using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace OpenVisionLab
{
    internal static class OpenVisionBitmapImagePreviewFactory
    {
        private const int MaxPreviewDimension = 1024;

        public static BitmapSource Create(Bitmap image)
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

        private static BitmapSource CreateBitmapImage(Bitmap image)
        {
            IntPtr hBitmap = IntPtr.Zero;
            try
            {
                hBitmap = image.GetHbitmap();
                BitmapSource source = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                source.Freeze();
                return source;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (hBitmap != IntPtr.Zero)
                {
                    DeleteObject(hBitmap);
                }
            }
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr handle);
    }
}
