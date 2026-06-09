using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace Lib.Common
{
    public static class BitmapImageConverter
    {
        public static Bitmap ToBitmap(OpenCvSharp.Mat image) => OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
        public static Mat ToMat(Bitmap image) => OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

        public static Bitmap ByteToBitmap(byte[] imgArr, int nW, int nH)
        {
            Bitmap bmp = new Bitmap(nW, nH, PixelFormat.Format8bppIndexed);

            BitmapData data = bmp.LockBits(
                                    new Rectangle(0, 0, nW, nH),
                                    ImageLockMode.ReadWrite,
                                        PixelFormat.Format8bppIndexed);
            IntPtr ptr = data.Scan0;
            Marshal.Copy(imgArr, 0, ptr, nW * nH);
            bmp.UnlockBits(data);

            //모노이미지로 변환해준다 사용하지 않을경우 칼라이미지가 깨진채로 사용된다
            ColorPalette Gpal = bmp.Palette;
            for (int i = 0; i < 256; i++)
            {
                Gpal.Entries[i] = Color.FromArgb(i, i, i);
            }
            bmp.Palette = Gpal;

            return bmp;
        }
    }
}
