using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
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

        public static BitmapSource CreateFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                throw new FileNotFoundException("The packet overlay file is missing.", path);
            }

            return CreateBitmapSourceFromPath(path, decodePixelWidth: 0);
        }

        public static BitmapImage TryCreateFromPath(string path, int decodePixelWidth)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return null;
            }

            try
            {
                return CreateBitmapSourceFromPath(path, decodePixelWidth);
            }
            catch
            {
                return null;
            }
        }

        public static Bitmap LoadBitmap(string path, string role)
        {
            try
            {
                using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using System.Drawing.Image decoded = System.Drawing.Image.FromStream(
                    stream,
                    useEmbeddedColorManagement: false,
                    validateImageData: true);
                return new Bitmap(decoded);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    (role ?? "Image") + " image could not be loaded: " + path,
                    exception);
            }
        }

        public static (int Width, int Height) ReadPixelSize(string path)
        {
            BitmapFrame frame = BitmapFrame.Create(
                new Uri(path, UriKind.Absolute),
                BitmapCreateOptions.DelayCreation,
                BitmapCacheOption.OnLoad);
            return (frame.PixelWidth, frame.PixelHeight);
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

        private static BitmapImage CreateBitmapSourceFromPath(string path, int decodePixelWidth)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            if (decodePixelWidth > 0)
            {
                image.DecodePixelWidth = decodePixelWidth;
            }

            image.UriSource = new Uri(Path.GetFullPath(path), UriKind.Absolute);
            image.EndInit();
            image.Freeze();
            return image;
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr handle);
    }
}
