using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Lib.Common
{
    /// <summary>
    /// 비트맵 헬퍼
    /// </summary>
    public class BitmapHelper
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Field
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region Field

        /// <summary>
        /// 이미지 바이트 배열
        /// </summary>
        public byte[] ImageByteArray;

        /// <summary>
        /// 행 크기 바이트 수
        /// </summary>
        public int RowSizeByteCount;

        /// <summary>
        /// 픽셀 데이터 크기
        /// </summary>
        public const int PixelDataSize = 32;

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////// Private

        #region Field

        /// <summary>
        /// 비트맵
        /// </summary>
        private Bitmap bitmap;

        /// <summary>
        /// 비트맵 데이터
        /// </summary>
        private BitmapData bitmapData;

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Property
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 너비 - Width

        /// <summary>
        /// 너비
        /// </summary>
        public int Width
        {
            get
            {
                return this.bitmap.Width;
            }
        }

        #endregion
        #region 높이 - Height

        /// <summary>
        /// 높이
        /// </summary>
        public int Height
        {
            get
            {
                return this.bitmap.Height;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Constructor
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 생성자 - BitmapHelper(bitmap)

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="bitmap">비트맵</param>
        public BitmapHelper(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 비트맵 잠그기 - LockBitmap()

        /// <summary>
        /// 비트맵 잠그기
        /// </summary>
        public void LockBitmap()
        {
            Rectangle boundRectangle = new Rectangle(0, 0, this.bitmap.Width, this.bitmap.Height);

            this.bitmapData = this.bitmap.LockBits
            (
                boundRectangle,
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb
            );

            RowSizeByteCount = this.bitmapData.Stride;

            int totalSize = this.bitmapData.Stride * this.bitmapData.Height;

            ImageByteArray = new byte[totalSize];

            Marshal.Copy(this.bitmapData.Scan0, ImageByteArray, 0, totalSize);
        }

        #endregion
        #region 비트맵 잠금 해제하기 - UnlockBitmap()

        /// <summary>
        /// 비트맵 잠금 해제하기
        /// </summary>
        public void UnlockBitmap()
        {
            int totalSize = this.bitmapData.Stride * this.bitmapData.Height;

            Marshal.Copy(ImageByteArray, 0, this.bitmapData.Scan0, totalSize);

            this.bitmap.UnlockBits(this.bitmapData);

            ImageByteArray = null;

            this.bitmapData = null;
        }

        #endregion
        #region 픽셀 구하기 - GetPixel(x, y, red, green, blue, alpha)

        /// <summary>
        /// 픽셀 구하기
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="red">빨강색</param>
        /// <param name="green">녹색</param>
        /// <param name="blue">파랑색</param>
        /// <param name="alpha">불투명도</param>
        public void GetPixel(int x, int y, out byte red, out byte green, out byte blue, out byte alpha)
        {
            int i = y * this.bitmapData.Stride + x * 4;

            blue  = ImageByteArray[i++];
            green = ImageByteArray[i++];
            red   = ImageByteArray[i++];
            alpha = ImageByteArray[i  ];
        }

        #endregion
        #region 픽셀 설정하기 - SetPixel(x, y, red, green, blue, alpha)

        /// <summary>
        /// 픽셀 설정하기
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="red">빨강색</param>
        /// <param name="green">녹색</param>
        /// <param name="blue">파랑색</param>
        /// <param name="alpha">불투명도</param>
        public void SetPixel(int x, int y, byte red, byte green, byte blue, byte alpha)
        {
            int i = y * this.bitmapData.Stride + x * 4;

            ImageByteArray[i++] = blue;
            ImageByteArray[i++] = green;
            ImageByteArray[i++] = red;
            ImageByteArray[i  ] = alpha;
        }

        #endregion
        #region 빨강색 구하기 - GetRed(x, y)

        /// <summary>
        /// 빨강색 구하기
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>빨강색</returns>
        public byte GetRed(int x, int y)
        {
            int i = y * this.bitmapData.Stride + x * 4;

            return ImageByteArray[i + 2];
        }

        #endregion
        #region 빨강색 설정하기 - SetRed(x, y, red)

        /// <summary>
        /// 빨강색 설정하기
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="red">빨강색</param>
        public void SetRed(int x, int y, byte red)
        {
            int i = y * this.bitmapData.Stride + x * 4;

            ImageByteArray[i + 2] = red;
        }

        #endregion
        #region 녹색 구하기 - GetGreen(x, y)

        /// <summary>
        /// 녹색 구하기
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>녹색</returns>
        public byte GetGreen(int x, int y)
        {
            int i = y * this.bitmapData.Stride + x * 4;

            return ImageByteArray[i + 1];
        }

        #endregion
        #region 녹색 설정하기 - SetGreen(x, y, green)

        /// <summary>
        /// 녹색 설정하기
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="green">녹색</param>
        public void SetGreen(int x, int y, byte green)
        {
            int i = y * this.bitmapData.Stride + x * 4;

            ImageByteArray[i + 1] = green;
        }

        #endregion
        #region 파랑색 구하기 - GetBlue(x, y)

        /// <summary>
        /// 파랑색 구하기
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>파랑색</returns>
        public byte GetBlue(int x, int y)
        {
            int i = y * this.bitmapData.Stride + x * 4;

            return ImageByteArray[i];
        }

        #endregion
        #region 파랑색 설정하기 - SetBlue(x, y, blue)

        /// <summary>
        /// 파랑색 설정하기
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="blue">파랑색</param>
        public void SetBlue(int x, int y, byte blue)
        {
            int i = y * this.bitmapData.Stride + x * 4;

            ImageByteArray[i] = blue;
        }

        #endregion
        #region 붙투명도 구하기 - GetAlpha(x, y)

        /// <summary>
        /// 붙투명도 구하기
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>불투명도</returns>
        public byte GetAlpha(int x, int y)
        {
            int i = y * this.bitmapData.Stride + x * 4;

            return ImageByteArray[i + 3];
        }

        #endregion
        #region 불투명도 설정하기 - SetAlpha(x, y, alpha)

        /// <summary>
        /// 불투명도 설정하기
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="alpha">불투명도</param>
        public void SetAlpha(int x, int y, byte alpha)
        {
            int i = y * this.bitmapData.Stride + x * 4;

            ImageByteArray[i + 3] = alpha;
        }

        #endregion
    }
}