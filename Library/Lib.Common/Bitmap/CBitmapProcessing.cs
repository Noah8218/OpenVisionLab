using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Common
{
    public static class CBitmapProcessing
    {
        #region 비트맵 구하기 - GetBitmap(filePath)

        /// <summary>
        /// 비트맵 구하기
        /// </summary>
        /// <param name="filePath">파일 경로</param>
        /// <returns>비트맵</returns>
        public static Bitmap GetBitmap(string filePath)
        {
            using (Bitmap bitmap = new Bitmap(filePath))
            {
                return new Bitmap(bitmap);
            }
        }
        #endregion
        
        #region 이진 대비 적용하기 - ApplyBinaryContrast(targetBitmap, cutoff)

        /// <summary>
        /// 이진 대비 적용하기
        /// </summary>
        /// <param name="targetBitmap">타겟 비트맵</param>
        /// <param name="cutoff">컷오프</param>
        public static void ApplyBinaryContrast(Bitmap targetBitmap, int cutoff)
        {
            CBitmapHelper helper = new CBitmapHelper(targetBitmap);

            helper.LockBitmap();

            for (int y = 0; y < targetBitmap.Height; y++)
            {
                for (int x = 0; x < targetBitmap.Width; x++)
                {
                    byte red;
                    byte green;
                    byte blue;
                    byte alpha;

                    helper.GetPixel(x, y, out red, out green, out blue, out alpha);

                    if (red + green + blue > cutoff)
                    {
                        helper.SetPixel(x, y, 255, 255, 255, 255);
                    }
                    else
                    {
                        helper.SetPixel(x, y, 0, 0, 0, 255);
                    }
                }
            }

            helper.UnlockBitmap();
        }

        #endregion
        #region 비트맵 저장하기 - SaveBitmap(bitmap, filePath)

        /// <summary>
        /// 비트맵 저장하기
        /// </summary>
        /// <param name="bitmap">비트맵</param>
        /// <param name="filePath">파일 경로</param>
        public static void SaveBitmap(Bitmap bitmap, string filePath)
        {
            string extension = Path.GetExtension(filePath);

            switch (extension.ToLower())
            {
                case ".bmp": bitmap.Save(filePath, ImageFormat.Bmp); break;
                case ".exif": bitmap.Save(filePath, ImageFormat.Exif); break;
                case ".gif": bitmap.Save(filePath, ImageFormat.Gif); break;
                case ".jpg":
                case ".jpeg": bitmap.Save(filePath, ImageFormat.Jpeg); break;
                case ".png": bitmap.Save(filePath, ImageFormat.Png); break;
                case ".tif":
                case ".tiff": bitmap.Save(filePath, ImageFormat.Tiff); break;
                default: throw new NotSupportedException("Unknown file extension " + extension);
            }
        }

        #endregion

        public static async Task<Bitmap> CropAtRect(Bitmap orgImg, Rectangle sRect)
        {
            Rectangle destRect = new Rectangle(System.Drawing.Point.Empty, sRect.Size);

            var cropImage = new Bitmap(destRect.Width, destRect.Height);
            using (var graphics = Graphics.FromImage(cropImage))
            {
                graphics.DrawImage(orgImg, destRect, sRect, GraphicsUnit.Pixel);
            }
            return await Task.FromResult(cropImage);
        }


        /// <summary>
        /// 이미지를 Overlay 합니다.
        /// </summary>
        /// <param name="bottom">원본이미지</param>
        /// <param name="overlay">원본 이미지 위에 Overlay 할 이미지</param>
        /// <param name="pLeft">Overlay 시작 포인트: Left Point</param>
        /// <param name="pTop">Overlay 시작 포인트 : Top Point</param>
        /// <returns>Overlay 된 이미지 반환</returns>
        public static Bitmap OverlayImage(System.Drawing.Image bottom, Bitmap overlay, int pLeft, int pTop)
        {
            Bitmap result = new Bitmap(bottom.Width, bottom.Height);
            Graphics g = Graphics.FromImage((System.Drawing.Image)result);

            g.DrawImage(bottom, 0, 0, bottom.Width, bottom.Height);
            g.DrawImage(overlay, pLeft, pTop, overlay.Width, overlay.Height);
            g.Dispose();

            return result;
        }

        /// <summary>
        /// 이미지를 Overlay 합니다.
        /// </summary>
        /// <param name="bottom">원본이미지</param>
        /// <param name="overlay">원본 이미지 위에 Overlay 할 이미지</param>
        /// <param name="pLeft">Overlay 시작 포인트: Left Point</param>
        /// <param name="pTop">Overlay 시작 포인트 : Top Point</param>
        /// <returns>Overlay 된 이미지 반환</returns>
        public static Bitmap OverlayImage(Bitmap bottom, Bitmap overlay, int pLeft, int pTop)
        {
            Bitmap result = new Bitmap(bottom.Width, bottom.Height);
            Graphics g = Graphics.FromImage(result);

            g.DrawImage(bottom, 0, 0, bottom.Width, bottom.Height);
            g.DrawImage(overlay, pLeft, pTop, overlay.Width, overlay.Height);
            g.Dispose();

            return result;
        }
    }
}
