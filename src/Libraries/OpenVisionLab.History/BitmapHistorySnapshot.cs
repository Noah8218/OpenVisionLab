using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OpenVisionLab.History
{
    public sealed class BitmapHistorySnapshot
    {
        private BitmapHistorySnapshot()
        {
        }

        public bool HasImage { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public byte[] EncodedPng { get; private set; }

        public static BitmapHistorySnapshot Empty()
        {
            return new BitmapHistorySnapshot();
        }

        public static bool TryCapture(Bitmap image, BitmapHistoryPolicy policy, out BitmapHistorySnapshot snapshot, out string reason)
        {
            policy ??= BitmapHistoryPolicy.Default;
            snapshot = null;
            reason = string.Empty;

            if (image == null)
            {
                snapshot = Empty();
                return true;
            }

            reason = policy.Validate(image.Width, image.Height);
            if (!string.IsNullOrWhiteSpace(reason))
            {
                return false;
            }

            try
            {
                using MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Png);
                byte[] bytes = stream.ToArray();

                reason = policy.Validate(image.Width, image.Height, bytes.LongLength);
                if (!string.IsNullOrWhiteSpace(reason))
                {
                    return false;
                }

                snapshot = new BitmapHistorySnapshot
                {
                    HasImage = true,
                    Width = image.Width,
                    Height = image.Height,
                    EncodedPng = bytes
                };
                return true;
            }
            catch (Exception ex)
            {
                reason = ex.GetBaseException().Message;
                return false;
            }
        }

        public Bitmap ToBitmap()
        {
            if (!HasImage || EncodedPng == null || EncodedPng.Length == 0)
            {
                return null;
            }

            using MemoryStream stream = new MemoryStream(EncodedPng, false);
            using Bitmap decoded = new Bitmap(stream);
            return new Bitmap(decoded);
        }

        public bool HasSameContent(BitmapHistorySnapshot other)
        {
            if (other == null)
            {
                return false;
            }

            if (HasImage != other.HasImage || Width != other.Width || Height != other.Height)
            {
                return false;
            }

            if (!HasImage)
            {
                return true;
            }

            byte[] left = EncodedPng ?? Array.Empty<byte>();
            byte[] right = other.EncodedPng ?? Array.Empty<byte>();
            if (left.Length != right.Length)
            {
                return false;
            }

            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}

