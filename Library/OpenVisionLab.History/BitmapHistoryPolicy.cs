using System;

namespace OpenVisionLab.History
{
    public sealed class BitmapHistoryPolicy
    {
        public static BitmapHistoryPolicy Default { get; } = new BitmapHistoryPolicy();

        public BitmapHistoryPolicy()
        {
            MaxPixels = 25_000_000;
            MaxEncodedBytes = 64 * 1024 * 1024;
        }

        public int MaxPixels { get; set; }
        public int MaxEncodedBytes { get; set; }

        public string Validate(int width, int height, long encodedBytes = -1)
        {
            if (width <= 0 || height <= 0)
            {
                return "Invalid image size.";
            }

            long pixels = (long)width * height;
            if (MaxPixels > 0 && pixels > MaxPixels)
            {
                return string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "Image is too large for history. Pixels={0}, Limit={1}.",
                    pixels,
                    MaxPixels);
            }

            if (encodedBytes >= 0 && MaxEncodedBytes > 0 && encodedBytes > MaxEncodedBytes)
            {
                return string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "Encoded image is too large for history. Bytes={0}, Limit={1}.",
                    encodedBytes,
                    MaxEncodedBytes);
            }

            return string.Empty;
        }
    }
}

