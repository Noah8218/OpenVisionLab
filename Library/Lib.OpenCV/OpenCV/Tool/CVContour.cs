using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Lib.Common;
using Lib.OpenCV.Property;
using Lib.OpenCV.Result;
using OpenCvSharp;

namespace Lib.OpenCV.Tool
{
    public partial class CVContour : COpenCVAlgorithmBase
    {
        public IOpenCVPropertyContour property;

        public List<CResultContour> results = new List<CResultContour>();
        
        public CVContour() { }

        public void SetProperty(IOpenCVPropertyContour propertyBase) => property = propertyBase;

        public override void Run()
        {
            if (property.USE_MULTI_ROI)
            {
                MultiRun();
            }
            else
            {
                SingleRun();
            }
        }

        public bool SingleRun()
        {
            try
            {
                OpenCvSharp.Point[] pp = null;
                OpenCvSharp.Point Points = new OpenCvSharp.Point();
                OpenCvSharp.Point[][] Contours;
                HierarchyIndex[] hierarchy;

                int Threshold = (int)property.THRESHOLD;
                int MinArea = property.MIN_AREA;
                int MaxArea = property.MAX_AREA;

                results.Clear();

                if (COpenCVHelper.IsImageEmpty(imageSource))
                {
                    CLOG.ABNORMAL("Image is Empty");
                    return false;
                }

                if (property.CvROI.Width == 0 || property.CvROI.Height == 0)
                {
                    property.CvROI = new OpenCvSharp.Rect(0, 0, imageSource.Width, imageSource.Height);
                }

                Mat ImageSrc = property.USE_ROI ? imageSource.SubMat(property.CvROI) : imageSource.Clone();

                if (property.USE_DRAW_IMAGE)
                {
                    imageResult = imageSource.Clone();
                    COpenCVHelper.SetImageChannel3(imageResult);
                }

                COpenCVHelper.SetImageChannel1(ImageSrc);

                #region THRESHOLD
                if (property.USE_THRESHOLD) { Cv2.Threshold(ImageSrc, ImageSrc, property.THRESHOLD, 255, property.THRESHOLD_TYPES); }
                else if (property.USE_ADAPTIVE_THRESHOLD) { Cv2.AdaptiveThreshold(ImageSrc, ImageSrc, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight); }

                #endregion

                // 컨투어 자체가 검은색 영역에서 흰색영역을 검출하는 알고리즘 
                // 검출하려고 하는 물체가 검은색이면 반전으로 검출해야함
                if (property.USE_BITWISENOT) Cv2.BitwiseNot(ImageSrc, ImageSrc);

                Cv2.FindContours(ImageSrc, out Contours, out hierarchy, property.DetectMode, property.ApproximationModes, null);

                AddRoiToContourPoints(Contours, property.CvROI);

                ConcurrentBag<CResultContour> filteredContours = new ConcurrentBag<CResultContour>();
                ConcurrentBag<OpenCvSharp.Point[]> drawContours = new ConcurrentBag<OpenCvSharp.Point[]>();

                Parallel.ForEach(Contours, (item, state, index) =>
                {
                    RotatedRect rrect;
                    Rect rt;

                    double dContourArea = Cv2.ContourArea(item, false);

                    if (dContourArea < MinArea || dContourArea > MaxArea)
                        return;

                    OpenCvSharp.Point[] contourForDraw;
                    OpenCvSharp.Point[] contourForCalc;

                    if (property.USE_APPROXPOLYDP)
                    {
                        double peri = Cv2.ArcLength(item, true);

                        // 핵심: pp를 지역 변수로 선언
                        OpenCvSharp.Point[] approxPoints =
                            Cv2.ApproxPolyDP(item, property.EPSILON * peri, true);

                        contourForCalc = approxPoints;
                        contourForDraw = approxPoints;
                    }
                    else
                    {
                        contourForCalc = item;
                        contourForDraw = item;
                    }

                    rt = Cv2.BoundingRect(contourForCalc);
                    rrect = Cv2.MinAreaRect(contourForCalc);

                    drawContours.Add(contourForDraw);

                    OpenCvSharp.Point ptCenter = new OpenCvSharp.Point(
                        rt.X + rt.Width / 2,
                        rt.Y + rt.Height / 2);

                    filteredContours.Add(
                        new CResultContour(
                            (int)index,
                            dContourArea,
                            ptCenter,
                            rt,
                            contourForDraw,
                            Math.Round(rrect.Angle, 1)));
                });

                if (property.USE_DRAW_IMAGE) { Cv2.DrawContours(imageResult, drawContours.ToArray(), -1, new Scalar(property.DrawColor.B, property.DrawColor.G, property.DrawColor.R, property.DrawColor.A), property.DrawThickness, LineTypes.Link4); }
                results = filteredContours.OrderBy(c => c.Index).ToList();

            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return false;
            }
            return true;
        }

        public bool MultiRun()
        {
            try
            {
                OpenCvSharp.Point[] pp = null;
                OpenCvSharp.Point Points = new OpenCvSharp.Point();
                OpenCvSharp.Point[][] Contours;
                HierarchyIndex[] hierarchy;

                int Threshold = (int)property.THRESHOLD;
                int MinArea = property.MIN_AREA;
                int MaxArea = property.MAX_AREA;

                results.Clear();
                List<OpenCvSharp.Point[]> drawContoursList = new List<OpenCvSharp.Point[]>();

                if (COpenCVHelper.IsImageEmpty(imageSource))
                {
                    CLOG.ABNORMAL("Image is Empty");
                    return false;
                }

                if (property.USE_DRAW_IMAGE)
                {
                    imageResult = imageSource.Clone();
                    COpenCVHelper.SetImageChannel3(imageResult);
                }

                for (int i = 0; i < property.CvROIS.Count; i++)
                {
                    if (property.CvROIS[i].Width == 0 || property.CvROIS[i].Height == 0)
                    {
                        property.CvROIS[i] = new OpenCvSharp.Rect(0, 0, imageSource.Width, imageSource.Height);
                    }

                    Mat ImageSrc = new Mat();

                    #region ROI
                    if (property.USE_ROI) { ImageSrc = imageSource.SubMat(property.CvROIS[i]); }
                    else { ImageSrc = imageSource.Clone(); }
                    #endregion

                    COpenCVHelper.SetImageChannel1(ImageSrc);

                    #region THRESHOLD
                    if (property.USE_THRESHOLD) { Cv2.Threshold(ImageSrc, ImageSrc, property.THRESHOLD, 255, property.THRESHOLD_TYPES); }
                    else if (property.USE_ADAPTIVE_THRESHOLD) { Cv2.AdaptiveThreshold(ImageSrc, ImageSrc, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight); }

                    #endregion

                    // 컨투어 자체가 검은색 영역에서 흰색영역을 검출하는 알고리즘 
                    // 검출하려고 하는 물체가 검은색이면 반전으로 검출해야함
                    if (property.USE_BITWISENOT) Cv2.BitwiseNot(ImageSrc, ImageSrc);

                    Cv2.FindContours(ImageSrc, out Contours, out hierarchy, property.DetectMode, property.ApproximationModes, null);

                    AddRoiToContourPoints(Contours, property.CvROIS[i]);

                    ConcurrentBag<CResultContour> filteredContours = new ConcurrentBag<CResultContour>();
                    ConcurrentBag<OpenCvSharp.Point[]> drawContours = new ConcurrentBag<OpenCvSharp.Point[]>();

                    Parallel.ForEach(Contours, (item, state, index) =>
                    {
                        RotatedRect rrect = new RotatedRect();
                        Rect rt = new Rect();
                        double dContourArea = Cv2.ContourArea(item, false);

                        if ((dContourArea >= MinArea) && (dContourArea <= MaxArea))
                        {
                            if (property.USE_APPROXPOLYDP)
                            {
                                double peri = Cv2.ArcLength(item, true);
                                pp = Cv2.ApproxPolyDP(item, property.EPSILON * peri, true);

                                rt = Cv2.BoundingRect(pp);
                                rrect = Cv2.MinAreaRect(pp);

                                drawContours.Add(pp);
                            }
                            else
                            {
                                rrect = Cv2.MinAreaRect(item);
                                rt = Cv2.BoundingRect(item);
                                drawContours.Add(item);
                            }
                            double areaRatio = Math.Abs(Cv2.ContourArea(item, false)) / (rrect.Size.Width * rrect.Size.Height);
                            OpenCvSharp.Point ptCenter = new OpenCvSharp.Point(rt.X + rt.Width / 2, rt.Y + rt.Height / 2);

                            filteredContours.Add(new CResultContour((int)index, dContourArea, ptCenter, rt, item, Math.Round(rrect.Angle, 1)));
                        }
                    });

                    results.AddRange(filteredContours.OrderBy(c => c.Index).ToList());
                    drawContoursList.AddRange(drawContours);
                }

                if (property.USE_DRAW_IMAGE) { Cv2.DrawContours(imageResult, drawContoursList.ToArray(), -1, new Scalar(property.DrawColor.B, property.DrawColor.G, property.DrawColor.R, property.DrawColor.A), property.DrawThickness, LineTypes.Link4); }
                
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return false;
            }

            return true;
        }

        private void AddRoiToContourPoints(OpenCvSharp.Point[][] Contours, OpenCvSharp.Rect CvROI)
        {
            if (property.USE_ROI)
            {
                for (int i = 0; i < Contours.Length; i++)
                {
                    for (int j = 0; j < Contours[i].Length; j++)
                    {
                        Contours[i][j].X = Contours[i][j].X + CvROI.X;
                        Contours[i][j].Y = Contours[i][j].Y + CvROI.Y;
                    }
                }
            }
        }

        public bool SquareRun()
        {
            try
            {
                results.Clear();

                if (COpenCVHelper.IsImageEmpty(imageSource))
                {
                    CLOG.ABNORMAL( "Image is Empty");
                    return false;
                }

                if (property.CvROI.Width == 0 || property.CvROI.Height == 0)
                {
                    property.CvROI = new OpenCvSharp.Rect(0, 0, imageSource.Width, imageSource.Height);
                }

                using (Mat ImageSrc = imageSource.Clone())
                {
                    if (COpenCVHelper.IsImageEmpty(imageSource)) return false;
                    imageResult = ImageSrc.Clone();

                    if (ImageSrc.Channels() == 4) Cv2.CvtColor(ImageSrc, ImageSrc, ColorConversionCodes.RGBA2GRAY);
                    if (ImageSrc.Channels() == 3) Cv2.CvtColor(ImageSrc, ImageSrc, ColorConversionCodes.RGB2GRAY);
                    if (imageResult.Channels() == 1) Cv2.CvtColor(imageResult, imageResult, ColorConversionCodes.GRAY2RGB);

                    Mat ImageContour = null;

                    #region ROI
                    if (property.USE_ROI) { ImageContour = ImageSrc.SubMat(property.CvROI).Clone(); }
                    else { ImageContour = ImageSrc.Clone(); }
                    #endregion

                    #region THRESHOLD
                    if (property.USE_THRESHOLD)
                    {
                        Cv2.Threshold(ImageContour, ImageContour, property.THRESHOLD, 255, property.THRESHOLD_TYPES);
                    }
                    #endregion

                    // 컨투어 자체가 검은색 영역에서 흰색영역을 검출하는 알고리즘 
                    // 검출하려고 하는 물체가 검은색이면 반전으로 검출해야함
                    if (property.USE_BITWISENOT) Cv2.BitwiseNot(ImageContour, ImageContour);

                    OpenCvSharp.Point[] pp = null;
                    OpenCvSharp.Point Points = new OpenCvSharp.Point();
                    OpenCvSharp.Point[][] Contours;
                    HierarchyIndex[] hierarchy;

                    int Threshold = (int)property.THRESHOLD;
                    int MinArea = property.MIN_AREA;
                    int MaxArea = property.MAX_AREA;


                    Cv2.FindContours(ImageContour, out Contours, out hierarchy, property.DetectMode, property.ApproximationModes, null);
                    CLOG.INSP("Find Contours Count : {0}", Contours.Length);

                    if (property.USE_ROI)
                    {
                        for (int i = 0; i < Contours.Length; i++)
                        {
                            for (int j = 0; j < Contours[i].Length; j++)
                            {
                                Contours[i][j].X = Contours[i][j].X + property.CvROI.X;
                                Contours[i][j].Y = Contours[i][j].Y + property.CvROI.Y;
                            }
                        }
                    }

                    for (int i = 0; i < Contours.Length; i++)
                    {
                        double dContourArea = Cv2.ContourArea(Contours[i], false);

                        if ((dContourArea >= MinArea) && (dContourArea <= MaxArea))
                        {
                            double peri = Cv2.ArcLength(Contours[i], true);
                            pp = Cv2.ApproxPolyDP(Contours[i], property.EPSILON * peri, true);

                            bool convex = Cv2.IsContourConvex(pp);

                            // 사각형은 4개의 꼭지점과 4개의 각도가 합쳤을 떄 360도가 나와야 함
                            if (pp.Length == 4 && convex)
                            {
                                double cos = 0;
                                OpenCvSharp.Point[][] pts = new OpenCvSharp.Point[1][];
                                pts[0] = new OpenCvSharp.Point[0];
                                pts[0] = pp;
                                for (int k = 1; k < 5; k++)
                                {

                                    //double Angle = CVision.RadianToDegree(Math.Abs(CVision.Angle(pp[k % 4], pp[(k - 1) % 4], pp[(k + 1) % 4])));
                                    double Angle2 = CFormula.threePointAngle(pp[k % 4], pp[(k - 1) % 4], pp[(k + 1) % 4]);
                                    cos = cos > Angle2 ? cos : Angle2;
                                    Cv2.Circle(imageResult, pp[k - 1], 5, Scalar.Yellow, Cv2.FILLED);

                                }
                                // 각도가 90도 이상이어야 함
                                if (cos > 90 && cos < 95)
                                {
                                    Cv2.Polylines(imageResult, pts, true, Scalar.Yellow, 1, LineTypes.AntiAlias, 0);
                                    //Cv2.FillPoly(ImageResult, pts, Scalar.Yellow); 
                                    // 중심점 기준 원근 변환 실행
                                    Mat dst = CFormula.PerspectiveTransform(imageSource.Clone(), pp);

                                    //Cv2.ImShow("dst", dst);
                                    //Cv2.WaitKey(0);
                                    //Cv2.DestroyAllWindows();
                                }
                            }


                            //Cv2.Polylines(ImageResult, pts, true, Scalar.Yellow, 1, LineTypes.AntiAlias, 0);
                            //Cv2.Polylines(ImageResult, pp, true, Scalar.Yellow, 3, LineTypes.AntiAlias, 0);



                            Rect rt = Cv2.BoundingRect(pp);

                            RotatedRect rrect = Cv2.MinAreaRect(pp);
                            double areaRatio = Math.Abs(Cv2.ContourArea(Contours[i], false)) / (rrect.Size.Width * rrect.Size.Height);



                            //OpenCvSharp.Scalar Color = new Scalar(Property.DrawColor.B, Property.DrawColor.G, Property.DrawColor.R, Property.DrawColor.A);

                            // Cv2.DrawContours(ImageResult, Contours, i, Color, Property.DrawThickness, LineTypes.Link4, hierarchy, 100, Points);
                            //OpenCvSharp.Point ptCenter = new OpenCvSharp.Point(rt.X + rt.Width / 2, rt.Y + rt.Height / 2);
                            //Cv2.PutText(ImageResult, dContourArea.ToString("F0"), ptCenter, HersheyFonts.HersheyTriplex, 2, Scalar.Aquamarine, 3, LineTypes.AntiAlias);

                            //Cv2.DrawContours(imageMasking, Contours, i, Scalar.White, -1, LineTypes.AntiAlias, hierarchy, 100, Points);

                            //CLOG.INSP("Draw Contours : {0}", i);

                            //Results.Add(new CResultContour(dContourArea, ptCenter, rt));

                            // ImageResult가 Draw 이미지
                        }
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return false;
            }

            return true;
        }
    }
}

