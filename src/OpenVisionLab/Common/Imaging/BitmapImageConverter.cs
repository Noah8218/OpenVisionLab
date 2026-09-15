using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenCvSharp;

namespace OpenVisionLab.Common
{
    /// <summary>
    /// Provides conversion between System.Drawing.Bitmap and OpenCvSharp.Mat.
    /// </summary>
    public static class BitmapImageConverter
    {
        #region ToMat

        /// <summary>
        /// Converts a System.Drawing.Bitmap to a Mat.
        /// </summary>
        public static Mat ToMat(this Bitmap src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            int channels = GetDefaultMatChannels(src.PixelFormat);
            Mat dst = new Mat(src.Height, src.Width, MatType.CV_8UC(channels));
            try
            {
                ToMat(src, dst);
                return dst;
            }
            catch
            {
                dst.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Converts a System.Drawing.Bitmap into an existing Mat.
        /// </summary>
        public static unsafe void ToMat(this Bitmap src, Mat dst)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (dst == null)
                throw new ArgumentNullException(nameof(dst));
            if (dst.IsDisposed)
                throw new ArgumentException("The specified dst is disposed.", nameof(dst));
            if (dst.Depth() != MatType.CV_8U)
                throw new NotSupportedException("Bitmap conversion requires a CV_8U destination Mat.");
            if (dst.Dims != 2)
                throw new NotSupportedException("Bitmap conversion requires a two-dimensional destination Mat.");
            if (src.Width != dst.Width || src.Height != dst.Height)
                throw new ArgumentException("src.Size != dst.Size");

            PixelFormat pixelFormat = src.PixelFormat;
            int sourceRowBytes = GetBitmapRowBytes(src.Width, pixelFormat);
            int channels = dst.Channels();
            ValidateToMatChannels(pixelFormat, channels);
            int destinationRowBytes = checked(src.Width * channels);
            ValidateMatStorage(dst, destinationRowBytes);

            Rectangle rect = new Rectangle(0, 0, src.Width, src.Height);
            BitmapData bitmapData = null;
            try
            {
                bitmapData = src.LockBits(rect, ImageLockMode.ReadOnly, pixelFormat);
                ValidateBitmapStorage(bitmapData, sourceRowBytes);

                byte* sourceBase = (byte*)bitmapData.Scan0.ToPointer();
                byte* destinationBase = (byte*)dst.Data.ToPointer();
                long sourceStep = bitmapData.Stride;
                long destinationStep = dst.Step();

                for (int y = 0; y < src.Height; y++)
                {
                    byte* sourceRow = sourceBase + (sourceStep * y);
                    byte* destinationRow = destinationBase + (destinationStep * y);

                    switch (pixelFormat)
                    {
                        case PixelFormat.Format1bppIndexed:
                            for (int x = 0; x < src.Width; x++)
                            {
                                byte sourceByte = sourceRow[x >> 3];
                                destinationRow[x] = (sourceByte & (0x80 >> (x & 7))) == 0
                                    ? (byte)0
                                    : (byte)255;
                            }

                            break;

                        case PixelFormat.Format8bppIndexed:
                            CopyIndexedRow(src.Palette, sourceRow, destinationRow, src.Width, channels);
                            break;

                        case PixelFormat.Format24bppRgb:
                            Buffer.MemoryCopy(
                                sourceRow,
                                destinationRow,
                                destinationRowBytes,
                                destinationRowBytes);
                            break;

                        case PixelFormat.Format32bppRgb:
                        case PixelFormat.Format32bppArgb:
                        case PixelFormat.Format32bppPArgb:
                            if (channels == 4)
                            {
                                Buffer.MemoryCopy(
                                    sourceRow,
                                    destinationRow,
                                    destinationRowBytes,
                                    destinationRowBytes);
                            }
                            else
                            {
                                for (int x = 0; x < src.Width; x++)
                                {
                                    int sourceOffset = x * 4;
                                    int destinationOffset = x * 3;
                                    destinationRow[destinationOffset] = sourceRow[sourceOffset];
                                    destinationRow[destinationOffset + 1] = sourceRow[sourceOffset + 1];
                                    destinationRow[destinationOffset + 2] = sourceRow[sourceOffset + 2];
                                }
                            }

                            break;

                        default:
                            throw new NotSupportedException(
                                $"Bitmap pixel format '{pixelFormat}' is not supported.");
                    }
                }
            }
            finally
            {
                if (bitmapData != null)
                    src.UnlockBits(bitmapData);
            }
        }

        #endregion

        #region ToBitmap

        /// <summary>
        /// Converts a Mat to a System.Drawing.Bitmap.
        /// </summary>
        public static Bitmap ToBitmap(this Mat src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            src.ThrowIfDisposed();
            PixelFormat pixelFormat = src.Channels() switch
            {
                1 => PixelFormat.Format8bppIndexed,
                3 => PixelFormat.Format24bppRgb,
                4 => PixelFormat.Format32bppArgb,
                _ => throw new NotSupportedException(
                    "Mat conversion supports only 1, 3, or 4 channels.")
            };
            return ToBitmap(src, pixelFormat);
        }

        /// <summary>
        /// Converts a Mat to a System.Drawing.Bitmap with the requested pixel format.
        /// </summary>
        public static Bitmap ToBitmap(this Mat src, PixelFormat pixelFormat)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            src.ThrowIfDisposed();
            GetBitmapRowBytes(src.Width, pixelFormat);
            ValidateToBitmapChannels(pixelFormat, src.Channels());
            ValidateMatStorage(src, checked(src.Width * src.Channels()));

            Bitmap bitmap = new Bitmap(src.Width, src.Height, pixelFormat);
            try
            {
                ToBitmap(src, bitmap);
                return bitmap;
            }
            catch
            {
                bitmap.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Converts a Mat into an existing Bitmap.
        /// </summary>
        public static unsafe void ToBitmap(this Mat src, Bitmap dst)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (dst == null)
                throw new ArgumentNullException(nameof(dst));
            if (src.IsDisposed)
                throw new ArgumentException("The image is disposed.", nameof(src));
            if (src.Dims != 2)
                throw new NotSupportedException("Bitmap conversion requires a two-dimensional source Mat.");
            if (src.Depth() != MatType.CV_8U)
                throw new NotSupportedException("Bitmap conversion requires a CV_8U source Mat.");
            if (src.Width != dst.Width || src.Height != dst.Height)
                throw new ArgumentException("src.Size != dst.Size");

            PixelFormat pixelFormat = dst.PixelFormat;
            int bitmapRowBytes = GetBitmapRowBytes(src.Width, pixelFormat);
            ValidateToBitmapChannels(pixelFormat, src.Channels());
            int sourceRowBytes = checked(src.Width * src.Channels());
            ValidateMatStorage(src, sourceRowBytes);

            if (pixelFormat == PixelFormat.Format8bppIndexed)
                SetGrayPalette(dst);

            Rectangle rect = new Rectangle(0, 0, src.Width, src.Height);
            BitmapData bitmapData = null;
            try
            {
                bitmapData = dst.LockBits(rect, ImageLockMode.WriteOnly, pixelFormat);
                ValidateBitmapStorage(bitmapData, bitmapRowBytes);

                byte* sourceBase = (byte*)src.Data.ToPointer();
                byte* destinationBase = (byte*)bitmapData.Scan0.ToPointer();
                long sourceStep = src.Step();
                long destinationStep = bitmapData.Stride;

                for (int y = 0; y < src.Height; y++)
                {
                    byte* sourceRow = sourceBase + (sourceStep * y);
                    byte* destinationRow = destinationBase + (destinationStep * y);

                    if (pixelFormat == PixelFormat.Format1bppIndexed)
                    {
                        for (int byteIndex = 0; byteIndex < bitmapRowBytes; byteIndex++)
                            destinationRow[byteIndex] = 0;

                        for (int x = 0; x < src.Width; x++)
                        {
                            if (sourceRow[x] != 0)
                                destinationRow[x >> 3] |= (byte)(0x80 >> (x & 7));
                        }
                    }
                    else
                    {
                        Buffer.MemoryCopy(
                            sourceRow,
                            destinationRow,
                            bitmapRowBytes,
                            bitmapRowBytes);
                    }
                }
            }
            finally
            {
                if (bitmapData != null)
                    dst.UnlockBits(bitmapData);
            }
        }

        #endregion

        private static int GetDefaultMatChannels(PixelFormat pixelFormat)
        {
            return pixelFormat switch
            {
                PixelFormat.Format1bppIndexed => 1,
                PixelFormat.Format8bppIndexed => 1,
                PixelFormat.Format24bppRgb => 3,
                PixelFormat.Format32bppRgb => 3,
                PixelFormat.Format32bppArgb => 4,
                PixelFormat.Format32bppPArgb => 4,
                _ => throw new NotSupportedException(
                    $"Bitmap pixel format '{pixelFormat}' is not supported.")
            };
        }

        private static int GetBitmapRowBytes(int width, PixelFormat pixelFormat)
        {
            if (width < 1)
                throw new NotSupportedException("Bitmap width must be positive.");

            long rowBytes = pixelFormat switch
            {
                PixelFormat.Format1bppIndexed => (width + 7L) / 8L,
                PixelFormat.Format8bppIndexed => width,
                PixelFormat.Format24bppRgb => width * 3L,
                PixelFormat.Format32bppRgb => width * 4L,
                PixelFormat.Format32bppArgb => width * 4L,
                PixelFormat.Format32bppPArgb => width * 4L,
                _ => throw new NotSupportedException(
                    $"Bitmap pixel format '{pixelFormat}' is not supported.")
            };

            if (rowBytes > int.MaxValue)
                throw new NotSupportedException("Bitmap row is too large.");
            return (int)rowBytes;
        }

        private static void ValidateToMatChannels(PixelFormat pixelFormat, int channels)
        {
            bool valid = pixelFormat switch
            {
                PixelFormat.Format1bppIndexed => channels == 1,
                PixelFormat.Format8bppIndexed => channels == 1 || channels == 3,
                PixelFormat.Format24bppRgb => channels == 3,
                PixelFormat.Format32bppRgb => channels == 3 || channels == 4,
                PixelFormat.Format32bppArgb => channels == 3 || channels == 4,
                PixelFormat.Format32bppPArgb => channels == 3 || channels == 4,
                _ => false
            };

            if (!valid)
            {
                throw new NotSupportedException(
                    $"Bitmap pixel format '{pixelFormat}' is not compatible with a {channels}-channel Mat.");
            }
        }

        private static void ValidateToBitmapChannels(PixelFormat pixelFormat, int channels)
        {
            bool valid = pixelFormat switch
            {
                PixelFormat.Format1bppIndexed => channels == 1,
                PixelFormat.Format8bppIndexed => channels == 1,
                PixelFormat.Format24bppRgb => channels == 3,
                PixelFormat.Format32bppRgb => channels == 4,
                PixelFormat.Format32bppArgb => channels == 4,
                PixelFormat.Format32bppPArgb => channels == 4,
                _ => false
            };

            if (!valid)
            {
                throw new NotSupportedException(
                    $"Bitmap pixel format '{pixelFormat}' is not compatible with a {channels}-channel Mat.");
            }
        }

        private static void ValidateBitmapStorage(BitmapData bitmapData, int rowBytes)
        {
            if (bitmapData == null || bitmapData.Scan0 == IntPtr.Zero)
                throw new NotSupportedException("Bitmap pixel storage is unavailable.");

            long absoluteStride = Math.Abs((long)bitmapData.Stride);
            if (absoluteStride < rowBytes)
            {
                throw new NotSupportedException(
                    $"Bitmap stride {bitmapData.Stride} is smaller than the visible row size {rowBytes}.");
            }
        }

        private static void ValidateMatStorage(Mat mat, int rowBytes)
        {
            long step = mat.Step();
            if (step < rowBytes)
            {
                throw new NotSupportedException(
                    $"Mat row step {step} is smaller than the visible row size {rowBytes}.");
            }

            IntPtr data = mat.Data;
            IntPtr dataEnd = mat.DataEnd;
            if (data == IntPtr.Zero || dataEnd == IntPtr.Zero)
                throw new NotSupportedException("Mat pixel storage is unavailable.");

            long requiredEnd = checked(
                data.ToInt64()
                + checked(step * (mat.Height - 1L))
                + rowBytes);
            if (requiredEnd > dataEnd.ToInt64())
            {
                throw new NotSupportedException(
                    "Mat row capacity is smaller than the requested visible pixel range.");
            }
        }

        private static unsafe void CopyIndexedRow(
            ColorPalette palette,
            byte* sourceRow,
            byte* destinationRow,
            int width,
            int channels)
        {
            byte[] red = new byte[256];
            byte[] green = new byte[256];
            byte[] blue = new byte[256];
            int paletteLength = Math.Min(256, palette?.Entries.Length ?? 0);
            for (int index = 0; index < paletteLength; index++)
            {
                Color color = palette.Entries[index];
                red[index] = color.R;
                green[index] = color.G;
                blue[index] = color.B;
            }

            if (channels == 1)
            {
                for (int x = 0; x < width; x++)
                    destinationRow[x] = red[sourceRow[x]];
                return;
            }

            for (int x = 0; x < width; x++)
            {
                int offset = x * 3;
                byte index = sourceRow[x];
                destinationRow[offset] = blue[index];
                destinationRow[offset + 1] = green[index];
                destinationRow[offset + 2] = red[index];
            }
        }

        private static void SetGrayPalette(Bitmap bitmap)
        {
            ColorPalette palette = bitmap.Palette;
            for (int index = 0; index < 256; index++)
                palette.Entries[index] = Color.FromArgb(index, index, index);
            bitmap.Palette = palette;
        }
    }
}
