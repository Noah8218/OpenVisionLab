using KtemVisionSystem;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KtemVisionSystem
{
    public enum ColorDetectTypeInLineGauge
    {
        BlackToWhite,
        WhiteToBlack,
        BlackToWhiteToBlack,
        WhiteToBlackToWhite,
    }

    public class OpencvHelper
    {
        Mat sourceMat = null;


        public OpencvHelper()
        {

        }

        public OpencvHelper(Mat src)
        {
            sourceMat = src;
        }

        //public OpencvHelper(BitmapSource source)
        //{
        //    sourceMat = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(source);
        //}

        public OpencvHelper(string path)
        {
            sourceMat = Cv2.ImRead(path);// OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(source);
        }

        public Mat SourceMat { get => sourceMat; set => sourceMat = value; }

        public Mat ChangeToGraySource(Mat src = null)
        {
            if(src == null)
            {
                src = sourceMat;
            }

            Mat dst0 = new Mat();
            if (src.Type() != MatType.CV_8UC1)
            {
                Cv2.CvtColor(src, dst0, ColorConversionCodes.BGR2GRAY, sourceMat.Channels());
                Cv2.Threshold(dst0, dst0, 0, 255, ThresholdTypes.Otsu);
            }
            else
            {
                Cv2.Threshold(sourceMat, dst0, 0, 255, ThresholdTypes.Otsu);
            }

            return dst0;
        }

        public static Mat GetSafetySubImage(int x, int y, int width, int height,
            int orgImgWidth, int orgImgHeight, Mat srcMat )
        {
            // crop the out of bound.

            if( x< 0 || y < 0 || 
                x >= orgImgWidth || y >= orgImgHeight || 
                width <= 0 || height <= 0
                )
            {
                //VTS.Logger.Error("Cross point is out of bound");

                return new Mat(1, 1, OpenCvSharp.MatType.CV_8UC1);

            }

            return srcMat.SubMat(
                new OpenCvSharp.Rect(x, y,
                width,
                height)
                );

        }

        public List<Tuple<Point, int>> GetLineHistogram(Point startPoint, Point endPoint)
        {
            return GetLineHistogram2(StaticConvertors.Windows2OpenCvPoint(startPoint),
                StaticConvertors.Windows2OpenCvPoint(endPoint));
        }

        public List<Tuple<Point,int>> GetLineHistogram2(Point startPoint, Point endPoint)
        {
            List<Tuple<Point, int>> result = new List<Tuple<Point, int>>();
            foreach (var lip in new LineIterator(sourceMat, startPoint, endPoint))
            {                
                Point p = lip.Pos;
                // Use appropriate type for generic GetValue<of T>().
                byte v = lip.GetValue<byte>();

                result.Add(new Tuple<Point, int>(p, v));
            }


            return result;
        }

        public void PrintLineHistogram(List<Tuple<Point, int>> lineHistogram)
        {
            Console.WriteLine("==Start Line Histogram");
            foreach (Tuple<Point, int> ptV in lineHistogram)
            {
                Console.WriteLine($"X:{ptV.Item1.X}, Y:{ptV.Item1.Y}, Val:{ptV.Item2}");
            }
            Console.WriteLine("<==End of Line Histogram");
        }

        public Tuple<Point, int> FindPoint(double threshold, int direction, bool w2b, List<Tuple<Point, int>> line)
        {
            double oldV = double.NaN;

            if(direction < 0)
            {
                line.Reverse();
            }

            foreach(var ptV in line)
            {
                if(double.IsNaN(oldV))
                {
                    // first time
                    oldV = ptV.Item2;
                }
                else
                {                    
                    if (Math.Abs((double)ptV.Item2 - oldV) > threshold)
                    {
                        return ptV;
                    }
                    
                }

            }

            return null;
        }

        public Line2D FitLine(List<Point> points, 
            DistanceTypes distanceTypes = DistanceTypes.L2,
            double param = 0, double reps = 0.01, double aeps = 0.01)
        {
            if (points.Count <= 0)
                return null;

            return  Cv2.FitLine(points, distanceTypes, param, reps, aeps);            
        }

        public Line2D FitLine(List<Point2f> points,
                DistanceTypes distanceTypes = DistanceTypes.L2,
                double param = 0, double reps = 0.01, double aeps = 0.01)
        {
            return Cv2.FitLine(points, distanceTypes, param, reps, aeps);
        }


        public static Point FindCrossPoint(double a1, double b1, double c1, double a2, double b2, double c2)
        {
            // c1 = a1 * x + b1 * y
            // c2 = a2 * x + b2 * y

            double[,] dEqParams = new double[2, 2];

            dEqParams[0, 0] = a1; //-2 * x0*sin(dT/2) *sin(dT/2)
            dEqParams[0, 1] = b1; //-2 * y0 * cos(dT/2) * sin(dT/2)

            dEqParams[1, 0] = a2; // 2 * x0 *cos(dT/2) * sin(dT/2)
            dEqParams[1, 1] = b2; // - 2 * y0 * sin(dT/2) * sin(dT/2)

            double[] dVals = new double[2];
            dVals[0] = c1;
            dVals[1] = c2;

            OpenCvSharp.Mat matEq = new OpenCvSharp.Mat(2, 2, OpenCvSharp.MatType.CV_64FC1, dEqParams);
            OpenCvSharp.Mat matVal = new OpenCvSharp.Mat(2, 1, OpenCvSharp.MatType.CV_64FC1, dVals);

            OpenCvSharp.Mat matResult = new OpenCvSharp.Mat();

            if(OpenCvSharp.Cv2.Solve(matEq, matVal, matResult))
            {
                return new Point(matResult.At<double>(0), matResult.At<double>(1));
            }

            return new Point(0,0);
        }


        public static (bool, Point)
            CalculateCameraOffset2(
                OpenCvSharp.Mat imageTop,
                OpenCvSharp.Mat imageBtm,
                int templateWidth = 200,
                int nTimeOut = 10000,
                double dScoreMin = 0.6,
                bool bFirstImageFlipX = false,
                bool bSecondImageFlipX = false,
                bool bFirstImageInvert = false,
                bool bSecondImageInvert = true,
                double dMagnification = 1.0D)
        {








            return (false, new Point());
        }





        public static (bool, Point)
            CalculateCameraOffset(
                OpenCvSharp.Mat imageTop,
                OpenCvSharp.Mat imageBtm,
                OpenCvSharp.Mat imageTemplate,
                int nTimeOut = 10000,
                double dScoreMin = 0.6,
                bool bFirstImageFlipX = false,
                bool bSecondImageFlipX = false,
                bool bFirstImageInvert = false,
                bool bSecondImageInvert = false,
                double dMagnification = 1.0D)
        {
            //20210912-송현수
            //Offset 은 현재 Top-Bottom 으로 했으나, 연산 기준은 확인 필요

            bool bResult = false;
            Point ptOffset = new Point();

            try
            {
                string dirPath = @"c:\test\acal";
                Directory.CreateDirectory(dirPath);

                OpenCvSharp.Point ptCenter_Top = new OpenCvSharp.Point();
                OpenCvSharp.Point ptCenter_Btm = new OpenCvSharp.Point();

                //using (Mat imageSrc_Top = imageTop.Resize(new OpenCvSharp.Size(imageTop.Width / dMagnification, imageTop.Height / dMagnification)))
                //using (Mat imageSrc_Btm = imageBtm.Resize(new OpenCvSharp.Size(imageBtm.Width / dMagnification, imageBtm.Height / dMagnification)))
                //using (Mat imageTpl = imageTemplate.Resize(new OpenCvSharp.Size(imageTemplate.Width / dMagnification, imageTemplate.Height / dMagnification)))


                imageTemplate = imageTop.SubMat(new Rect(imageTop.Width / 2 - 100, imageTop.Height / 2 - 100, 200, 200));

                using (Mat imageSrc_Top = imageTop)
                using (Mat imageSrc_Btm = imageBtm)
                using (Mat imageTpl = imageTemplate)
                using (Mat imageMatching_Top = new OpenCvSharp.Mat())
                using (Mat imageMatching_Btm = new OpenCvSharp.Mat())
                {
                    if (imageSrc_Top == null || imageSrc_Top.IsDisposed || imageSrc_Top.Width == 0 || imageSrc_Top.Height == 0) return (bResult, ptOffset);
                    if (imageSrc_Btm == null || imageSrc_Btm.IsDisposed || imageSrc_Btm.Width == 0 || imageSrc_Btm.Height == 0) return (bResult, ptOffset);
                    if (imageTpl == null || imageTpl.IsDisposed || imageTpl.Width == 0 || imageTpl.Height == 0) return (bResult, ptOffset);

                    if (imageSrc_Top.Channels() == 4) Cv2.CvtColor(imageSrc_Top, imageSrc_Top, OpenCvSharp.ColorConversionCodes.RGBA2GRAY);
                    if (imageSrc_Top.Channels() == 3) Cv2.CvtColor(imageSrc_Top, imageSrc_Top, OpenCvSharp.ColorConversionCodes.RGB2GRAY);

                    if (imageSrc_Btm.Channels() == 4) Cv2.CvtColor(imageSrc_Btm, imageSrc_Btm, OpenCvSharp.ColorConversionCodes.RGBA2GRAY);
                    if (imageSrc_Btm.Channels() == 3) Cv2.CvtColor(imageSrc_Btm, imageSrc_Btm, OpenCvSharp.ColorConversionCodes.RGB2GRAY);

                    if (imageTpl.Channels() == 4) Cv2.CvtColor(imageTpl, imageTpl, OpenCvSharp.ColorConversionCodes.RGBA2GRAY);
                    if (imageTpl.Channels() == 3) Cv2.CvtColor(imageTpl, imageTpl, OpenCvSharp.ColorConversionCodes.RGB2GRAY);

                    if (bFirstImageFlipX) Cv2.Flip(imageSrc_Top, imageSrc_Top, FlipMode.X);
                    if (bSecondImageFlipX) Cv2.Flip(imageSrc_Btm, imageSrc_Btm, FlipMode.X);

                    if (bFirstImageInvert) Cv2.BitwiseNot(imageSrc_Top, imageSrc_Top);
                    if (bSecondImageInvert) Cv2.BitwiseNot(imageSrc_Btm, imageSrc_Btm);

                    double dScoreMax_Top = 0.0D;
                    double dScoreMax_Btm = 0.0D;

                    OpenCvSharp.Point ptDetectedMax_Top = new OpenCvSharp.Point();
                    OpenCvSharp.Point ptDetectedMax_Btm = new OpenCvSharp.Point();

                    bool bDetected_Top = false;
                    bool bDetected_Btm = false;

                    int nTimeOutCheck = Environment.TickCount;

                    #region TOP 검사
                    while (true)
                    {
                        if ((Environment.TickCount - nTimeOutCheck) > nTimeOut)
                            return (bResult, ptOffset);

                        Cv2.MatchTemplate(imageSrc_Top, imageTpl, imageMatching_Top,
                            TemplateMatchModes.CCoeffNormed, null);
                        Cv2.MinMaxLoc(imageMatching_Top, out double dMinScore, out double dScore, out OpenCvSharp.Point ptMinLocation, out OpenCvSharp.Point ptMaxLocation);

                        if (dScore > dScoreMin)
                        {
                            bDetected_Top = true;

                            
                            ptDetectedMax_Top = (dScore > dScoreMax_Top) ? new OpenCvSharp.Point(ptMaxLocation.X * dMagnification, ptMaxLocation.Y * dMagnification) : ptDetectedMax_Top;
                            dScoreMax_Top = (dScore > dScoreMax_Top) ? dScore : dScoreMax_Top;

                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (bDetected_Top) ptCenter_Top = ptDetectedMax_Top;
                    else return (bResult, ptOffset);
                    #endregion

                    nTimeOutCheck = Environment.TickCount;

                    #region Bottom 검사
                    while (true)
                    {
                        if ((Environment.TickCount - nTimeOutCheck) > nTimeOut)
                            return (bResult, ptOffset);

                        Cv2.MatchTemplate(imageSrc_Btm, imageTpl, imageMatching_Btm,
                            TemplateMatchModes.CCoeffNormed, null);
                        Cv2.MinMaxLoc(imageMatching_Btm, out double dMinScore,
                            out double dScore, out OpenCvSharp.Point ptMinLocation, out OpenCvSharp.Point ptMaxLocation);

                        if (dScore > dScoreMin)
                        {
                            bDetected_Btm = true;

                            
                            ptDetectedMax_Btm = (dScore > dScoreMax_Btm) ? new OpenCvSharp.Point(ptMaxLocation.X * dMagnification, ptMaxLocation.Y * dMagnification) : ptDetectedMax_Btm;
                            dScoreMax_Btm = (dScore > dScoreMax_Btm) ? dScore : dScoreMax_Btm;

                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (bDetected_Btm) ptCenter_Btm = ptDetectedMax_Btm;
                    else return (bResult, ptOffset);
                    #endregion
                }

                ptOffset = new Point(ptCenter_Top.X - ptCenter_Btm.X, ptCenter_Top.Y - ptCenter_Btm.Y);
                bResult = true;
            }
            catch (Exception ex)
            {
                //VTS.Logger.Error(ex.Message);
                return (bResult, ptOffset);
            }

            return (bResult, ptOffset);
        }

    }

    
}
