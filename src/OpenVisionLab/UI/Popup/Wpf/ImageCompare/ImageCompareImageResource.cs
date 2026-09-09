using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;

namespace OpenVisionLab
{
    /// <summary>
    /// Owns the decoded image resources used by one Image Compare slot.
    /// </summary>
    internal sealed class ImageCompareImageResource : IDisposable
    {
        private bool disposed;

        private ImageCompareImageResource(
            Bitmap bitmap,
            BitmapSource source,
            string formatText)
        {
            Bitmap = bitmap;
            Source = source;
            FormatText = formatText ?? string.Empty;
        }

        public Bitmap Bitmap { get; }

        public BitmapSource Source { get; }

        public string FormatText { get; }

        public static ImageCompareImageResource Load(string path)
        {
            Bitmap bitmap = new Bitmap(path);
            try
            {
                string formatText = ResolveFormatText(path, bitmap.PixelFormat);
                BitmapSource source = LoadBitmapSource(path);
                return new ImageCompareImageResource(bitmap, source, formatText);
            }
            catch
            {
                bitmap.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            Bitmap.Dispose();
        }

        private static BitmapSource LoadBitmapSource(string path)
        {
            BitmapImage image = new BitmapImage();
            using FileStream stream = File.OpenRead(path);
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            return image;
        }

        private static string ResolveFormatText(string path, PixelFormat pixelFormat)
        {
            if (TryReadPngFormatText(path, out string pngText)) { return pngText; }
            if (TryReadBmpFormatText(path, out string bmpText)) { return bmpText; }

            int bits = Image.GetPixelFormatSize(pixelFormat);
            return bits > 0 ? $"Decoded {bits}-bit ({pixelFormat})" : pixelFormat.ToString();
        }

        private static bool TryReadPngFormatText(string path, out string formatText)
        {
            formatText = string.Empty;
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                if (bytes.Length < 29 ||
                    bytes[0] != 0x89 ||
                    bytes[1] != 0x50 ||
                    bytes[2] != 0x4E ||
                    bytes[3] != 0x47)
                {
                    return false;
                }

                int bitDepth = bytes[24];
                int colorType = bytes[25];
                int channels = colorType switch
                {
                    0 => 1,
                    2 => 3,
                    3 => 1,
                    4 => 2,
                    6 => 4,
                    _ => 1
                };
                string colorName = colorType switch
                {
                    0 => "Gray",
                    2 => "RGB",
                    3 => "Indexed",
                    4 => "GrayA",
                    6 => "RGBA",
                    _ => "ColorType" + colorType.ToString(CultureInfo.InvariantCulture)
                };

                formatText = $"PNG {bitDepth * channels}-bit {colorName}";
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryReadBmpFormatText(string path, out string formatText)
        {
            formatText = string.Empty;
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                if (bytes.Length < 30 || bytes[0] != 0x42 || bytes[1] != 0x4D) { return false; }

                int bitsPerPixel = BitConverter.ToUInt16(bytes, 28);
                formatText = $"BMP {bitsPerPixel}-bit";
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
