using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal static class OpenVisionPreviewImageFileService
    {
        public static Bitmap CloneBitmapPreservingPixelFormat(Bitmap image)
        {
            if (image == null)
            {
                return null;
            }

            try
            {
                return image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            }
            catch
            {
                return new Bitmap(image);
            }
        }

        public static Bitmap CreateSavableBitmap(Bitmap image)
        {
            if (image == null)
            {
                return null;
            }

            if ((image.PixelFormat & PixelFormat.Indexed) == 0)
            {
                return CloneBitmapPreservingPixelFormat(image);
            }

            Bitmap copy = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            using Graphics graphics = Graphics.FromImage(copy);
            graphics.DrawImageUnscaled(image, 0, 0);
            return copy;
        }

        public static string CreateDefaultImageFileName(string layerName)
        {
            char[] invalidCharacters = Path.GetInvalidFileNameChars();
            string safeName = new string((string.IsNullOrWhiteSpace(layerName) ? "Layer" : layerName)
                .Select(ch => invalidCharacters.Contains(ch) ? '_' : ch)
                .ToArray());
            return safeName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".png";
        }

        public static string ResolveOpenImageDirectory(string lastDirectory)
        {
            return OpenVisionImageDirectoryResolver.ResolveOpenImageDirectory(lastDirectory);
        }

        public static ImageFormat ResolveImageFormat(string path)
        {
            string extension = Path.GetExtension(path)?.ToLowerInvariant();
            return extension switch
            {
                ".bmp" => ImageFormat.Bmp,
                ".jpg" => ImageFormat.Jpeg,
                ".jpeg" => ImageFormat.Jpeg,
                ".tif" => ImageFormat.Tiff,
                ".tiff" => ImageFormat.Tiff,
                _ => ImageFormat.Png
            };
        }
    }
}
