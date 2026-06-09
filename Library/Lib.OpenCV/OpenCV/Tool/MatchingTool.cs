using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Lib.Common;
using Lib.OpenCV.Property;
using Lib.OpenCV.Result;
using OpenCvSharp;

namespace Lib.OpenCV.Tool
{
    public partial class MatchingTool : OpenCvAlgorithmBase
    {
        public IOpenCVPropertyMatching property;
        public List<MatchingResult> results = new List<MatchingResult>();        
        public MatchingTool() { }
        
        public void SetTemplateImage(Mat Image) => imageTemplate = Image.Clone();
       
        public void SetProperty(IOpenCVPropertyMatching property) => this.property = property;
     
        private void FindTemplate(Mat ImageSorce, Mat Template, ConcurrentBag<MatchingResult> Results_T, double angle, double Mag, OpenCvSharp.Rect CvROI)
        {
            Mat ImageMatching = new Mat();

            Cv2.MatchTemplate(ImageSorce, Template, ImageMatching, property.MATCH_MODE, null);
            Cv2.MinMaxLoc(ImageMatching, out double dMinScore, out double dMaxScore, out OpenCvSharp.Point ptMinLocation, out OpenCvSharp.Point ptMaxLocation);

            if (dMaxScore > property.SCORE_MIN)
            {
                int ImageTplW = Template.Width;
                int ImageTplH = Template.Height;

                OpenCvSharp.Point ptStart = new OpenCvSharp.Point(ptMaxLocation.X, ptMaxLocation.Y);
                OpenCvSharp.Point ptEnd = new OpenCvSharp.Point(ptMaxLocation.X + (ImageTplW), ptMaxLocation.Y + (ImageTplH));
                double dScore = dMaxScore * 100.0D;

                Rect2f rtBounding = new Rect2f();
                if (property.USE_ROI) { rtBounding = new Rect2f(ptStart.X + CvROI.X, ptStart.Y + CvROI.Y, ptEnd.X - ptStart.X, ptEnd.Y - ptStart.Y); }
                else { rtBounding = new Rect2f(ptStart.X, ptStart.Y, ptEnd.X - ptStart.X, ptEnd.Y - ptStart.Y); }
                
                OpenCvSharp.Point2f ptCenter = new OpenCvSharp.Point2f((rtBounding.X + (rtBounding.X + rtBounding.Width)) / 2, (rtBounding.Y + (rtBounding.Y + rtBounding.Height)) / 2);
                Results_T.Add(new MatchingResult(0, dScore, ptCenter, rtBounding, angle));
            }
        }

        public Mat Rotate(Mat src, double angle, bool PaddingWhite = false)
        {
            Mat rotate = new Mat(src.Size(), MatType.CV_8UC1);
            Mat matrix = Cv2.GetRotationMatrix2D(new Point2f(src.Width / 2, src.Height / 2), angle, 1);
            if (PaddingWhite)
            {
                Cv2.WarpAffine(src, rotate, matrix, src.Size(), InterpolationFlags.Linear, BorderTypes.Constant, new Scalar(255, 255, 255));
            }
            else
            {
                Cv2.WarpAffine(src, rotate, matrix, src.Size(), InterpolationFlags.Linear, BorderTypes.Reflect);
            }

            return rotate;
        }

        public override void Run()
        {
            if(property.USE_MULTI_ROI)
            {
                ImagePyramidsMultiRun();
            }
            else 
            {
                ImagePyramidsSingleRun();
            }
        }

        public bool ImagePyramidsMultiRun()
        {
                        swTaktTimems.Restart();
            results.Clear();

            for(int i = 0; i < property.CvROIS.Count; i++)
            {
                if (property.CvROIS[i].Width == 0 || property.CvROIS[i].Height == 0 || !property.USE_ROI)
                {
                    property.CvROIS[i] = new Rect(0, 0, imageSource.Width, imageSource.Height);
                }

                if (OpenCvHelper.IsImageEmpty(imageSource))
                {

                    return false;
                }

                OpenCvHelper.SetImageChannel1(imageTemplate);
                OpenCvHelper.SetImageChannel1(imageSource);

                if (property.USE_THRESHOLD)
                {
                    Cv2.Threshold(imageTemplate, imageTemplate, property.THRESHOLD, 255, property.THRESHOLD_TYPES);
                    Cv2.Threshold(imageSource, imageSource, property.THRESHOLD, 255, property.THRESHOLD_TYPES);
                }

                if (property.USE_ADAPTIVE_THRESHOLD)
                {
                    Cv2.AdaptiveThreshold(imageTemplate, imageTemplate, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight);
                    Cv2.AdaptiveThreshold(imageSource, imageSource, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight);
                }

                if (property.USE_CANNY)
                {
                    Cv2.GaussianBlur(imageTemplate, imageTemplate, new OpenCvSharp.Size(3, 3), 1, 1, BorderTypes.Default);
                    Cv2.GaussianBlur(imageSource, imageSource, new OpenCvSharp.Size(3, 3), 1, 1, BorderTypes.Default);

                    Cv2.Canny(imageTemplate, imageTemplate, property.CANNY_LOW, property.CANNY_HIGH, 3, true);
                    Cv2.Canny(imageSource, imageSource, property.CANNY_LOW, property.CANNY_HIGH, 3, true);
                }
                
                using (Mat ImageSrc = property.USE_ROI ? imageSource.SubMat(property.CvROIS[i]) : imageSource.Clone())
                using (Mat ImageSubMat = ImageSrc.Resize(new OpenCvSharp.Size((int)(ImageSrc.Width / property.MAGNIFIATION), (int)(ImageSrc.Height / property.MAGNIFIATION))))
                using (Mat ImageTpl = imageTemplate.Resize(new OpenCvSharp.Size((int)(imageTemplate.Width / property.MAGNIFIATION), (int)(imageTemplate.Height / property.MAGNIFIATION))))
                {
                    if (OpenCvHelper.IsImageEmpty(imageTemplate)
                           || OpenCvHelper.IsImageEmpty(imageSource)) return false;

                    int maxCount = property.NUM_MATCH;
                    while (true && maxCount > 0)
                    {
                        Thread.Sleep(0);
                        double dScore = 0;
                        ConcurrentBag<MatchingResult> Results_T = new ConcurrentBag<MatchingResult>();
                        double Angle = property.FIND_ANGLE;

                        var firstTask = Task.Run(() =>
                        {
                            FindTemplate(ImageSubMat, ImageTpl, Results_T, 0, property.MAGNIFIATION, property.CvROIS[i]);
                        });

                        if (property.USE_FIND_ANGLE)
                        {
                            int offset = (int)(property.FIND_ANGLE_MAX / Angle);

                            var plusTask = Task.Run(() =>
                            {
                                Parallel.For(0, offset,
                              k =>
                              {
                                  double T_Result = T_Result = Angle * k;
                                  Mat mat = Rotate(ImageSubMat, T_Result, property.USE_PADDING_COLOR_WHITE);
                                  FindTemplate(mat, ImageTpl, Results_T, T_Result, property.MAGNIFIATION, property.CvROIS[i]);
                              });
                            });

                            int offsetMin = (int)(property.FIND_ANGLE_MIN / Angle);
                            if (offsetMin < 0) { offsetMin = offsetMin * -1; }

                            var minusTask = Task.Run(() =>
                            {
                                Parallel.For(0, offsetMin,
                                         k =>
                                         {
                                             double T_Result = (Angle * k) * -1;
                                             Mat mat = Rotate(ImageSubMat, T_Result, property.USE_PADDING_COLOR_WHITE);
                                             FindTemplate(mat, ImageTpl, Results_T, T_Result, property.MAGNIFIATION, property.CvROIS[i]);
                                         });
                            });
                            Task.WaitAll(plusTask);
                            Task.WaitAll(minusTask);
                        }

                        Task.WaitAll(firstTask);

                        MatchingResult highestScoreResult = Results_T.OrderByDescending(r => r.Score).FirstOrDefault();

                        if (highestScoreResult == null) { break;}

                        List<MatchingResult> Results_T2 = new List<MatchingResult>();

                        int OrgMag = 2;
                        maxCount--;
                        using (Mat ImageSubMatPy = ImageSrc.Resize(new OpenCvSharp.Size((int)(ImageSrc.Width / OrgMag), (int)(ImageSrc.Height / OrgMag))))
                        using (Mat ImageTplPy = imageTemplate.Resize(new OpenCvSharp.Size((int)(imageTemplate.Width / OrgMag), (int)(imageTemplate.Height / OrgMag))))
                        {
                            double ImageTplW = ImageTplPy.Width;
                            double ImageTplH = ImageTplPy.Height;

                            Mat ImageSrc_T = Rotate(ImageTplPy, (highestScoreResult.Angle * -1), property.USE_PADDING_COLOR_WHITE);

                            //if (bDraw) { Cv2.ImShow("Tem", ImageSrc_T); }

                            OpenCvSharp.Point ptMax = new OpenCvSharp.Point();
                            double dMax = 0.0D;
                            Mat ImageMatching = new Mat();

                            Cv2.MatchTemplate(ImageSubMatPy, ImageSrc_T, ImageMatching, property.MATCH_MODE, null);
                            Cv2.MinMaxLoc(ImageMatching, out double dMinScore, out double dMaxScore, out OpenCvSharp.Point ptMinLocation, out OpenCvSharp.Point ptMaxLocation);

                            if (dMaxScore > property.SCORE_MIN)
                            {
                                ptMax = (dMaxScore > dMax) ? ptMaxLocation : ptMax;
                                dMax = (dMaxScore > dMax) ? dMaxScore : dMax;
                                OpenCvSharp.Point ptStart = new OpenCvSharp.Point(ptMaxLocation.X, ptMaxLocation.Y);
                                OpenCvSharp.Point ptEnd = new OpenCvSharp.Point(ptMaxLocation.X + (ImageTplW), ptMaxLocation.Y + (ImageTplH));

                                ptMaxLocation.X = (int)(ptMaxLocation.X * OrgMag);
                                ptMaxLocation.Y = (int)(ptMaxLocation.Y * OrgMag);
                                //ImageTplW = (int)(ImageTpl.Width);
                                //ImageTplH = (int)(ImageTpl.Height);

                                Rect rt = new Rect(new OpenCvSharp.Point(ptMaxLocation.X - 5, ptMaxLocation.Y - 5), new OpenCvSharp.Size(imageTemplate.Width + 10, imageTemplate.Height + 10));
                                ImageSrc.Rectangle(rt, Scalar.Black, -1);

                                ptStart.X = (ptStart.X * OrgMag);
                                ptStart.Y = (ptStart.Y * OrgMag);
                                ptEnd.X = (ptEnd.X * OrgMag);
                                ptEnd.Y = (ptEnd.Y * OrgMag);

                                dScore = dMaxScore * 100.0D;

                                Rect2f rtBounding = new Rect2f(ptStart.X + property.CvROIS[i].X, ptStart.Y + property.CvROIS[i].Y, ptEnd.X - ptStart.X, ptEnd.Y - ptStart.Y);
                                OpenCvSharp.Point2f ptCenter = new OpenCvSharp.Point2f((rtBounding.X + (rtBounding.X + rtBounding.Width)) / 2, (rtBounding.Y + (rtBounding.Y + rtBounding.Height)) / 2);

                                Results_T2.Add(new MatchingResult(0, dScore, ptCenter, rtBounding, highestScoreResult.Angle));
                            }
                        }
                        if (Results_T2.Count > 0) { results.Add(new MatchingResult(Results_T2[0].Index, Results_T2[0].Score, Results_T2[0].Center, CommonConverter.RectangleToRect(Results_T2[0].Bounding), Results_T2[0].Angle)); }
                    }
                }
                swTaktTimems.Stop();
            }
        

            return true;
        }

        public bool ImagePyramidsSingleRun()
        {
                        swTaktTimems.Restart();
            results.Clear();

            Mat ImageMatching = new Mat();

            if (property.CvROI.Width == 0 || property.CvROI.Height == 0 || !property.USE_ROI)
            {
                property.CvROI = new Rect(0, 0, imageSource.Width, imageSource.Height);
            }

            if (OpenCvHelper.IsImageEmpty(imageSource))
            {
                return false;
            }

            OpenCvHelper.SetImageChannel1(imageTemplate);
            OpenCvHelper.SetImageChannel1(imageSource);
            
            if (property.USE_THRESHOLD)
            {
                Cv2.Threshold(imageTemplate, imageTemplate, property.THRESHOLD, 255, property.THRESHOLD_TYPES);
                Cv2.Threshold(imageSource, imageSource, property.THRESHOLD, 255, property.THRESHOLD_TYPES);
            }

            if (property.USE_ADAPTIVE_THRESHOLD)
            {
                Cv2.AdaptiveThreshold(imageTemplate, imageTemplate, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight);
                Cv2.AdaptiveThreshold(imageSource, imageSource, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight);
            }

            if (property.USE_CANNY)
            {
                Cv2.GaussianBlur(imageTemplate, imageTemplate, new OpenCvSharp.Size(3, 3), 1, 1, BorderTypes.Default);
                Cv2.GaussianBlur(imageSource, imageSource, new OpenCvSharp.Size(3, 3), 1, 1, BorderTypes.Default);

                Cv2.Canny(imageTemplate, imageTemplate, property.CANNY_LOW, property.CANNY_HIGH, 3, true);
                Cv2.Canny(imageSource, imageSource, property.CANNY_LOW, property.CANNY_HIGH, 3, true);
            }

            using (Mat ImageSrc = property.USE_ROI ? imageSource.SubMat(property.CvROI) : imageSource.Clone())
            using (Mat ImageSubMat = ImageSrc.Resize(new OpenCvSharp.Size((int)(ImageSrc.Width / property.MAGNIFIATION), (int)(ImageSrc.Height / property.MAGNIFIATION))))
            using (Mat ImageTpl = imageTemplate.Resize(new OpenCvSharp.Size((int)(imageTemplate.Width / property.MAGNIFIATION), (int)(imageTemplate.Height / property.MAGNIFIATION))))
            {
                if (OpenCvHelper.IsImageEmpty(imageTemplate)
                       || OpenCvHelper.IsImageEmpty(imageSource)) return false;

                int maxCount = property.NUM_MATCH;
                while (true && maxCount > 0)
                {
                    Thread.Sleep(0);
                    double dScore = 0;
                    ConcurrentBag<MatchingResult> Results_T = new ConcurrentBag<MatchingResult>();
                    double Angle = property.FIND_ANGLE;

                    var firstTask = Task.Run(() =>
                    {
                        FindTemplate(ImageSubMat, ImageTpl, Results_T, 0, property.MAGNIFIATION, property.CvROI);
                    });

                    if(property.USE_FIND_ANGLE)
                    {
                        int offset = (int)(property.FIND_ANGLE_MAX / Angle);

                        var plusTask = Task.Run(() =>
                        {
                            Parallel.For(0, offset,
                          i =>
                          {
                              double T_Result = T_Result = Angle * i;
                              Mat mat = Rotate(ImageSubMat, T_Result, property.USE_PADDING_COLOR_WHITE);
                              FindTemplate(mat, ImageTpl, Results_T, T_Result, property.MAGNIFIATION, property.CvROI);
                          });
                        });
                        
                        int offsetMin = (int)(property.FIND_ANGLE_MIN / Angle);
                        if (offsetMin < 0) { offsetMin = offsetMin * -1; }

                        var minusTask = Task.Run(() =>
                        {
                            Parallel.For(0, offsetMin,
                                     i =>
                                     {
                                         double T_Result = (Angle * i) * -1;
                                         Mat mat = Rotate(ImageSubMat, T_Result, property.USE_PADDING_COLOR_WHITE);
                                         FindTemplate(mat, ImageTpl, Results_T, T_Result, property.MAGNIFIATION, property.CvROI);
                                     });
                        });
                        Task.WaitAll(plusTask);
                        Task.WaitAll(minusTask);
                    }
                   
                    Task.WaitAll(firstTask);

                    MatchingResult highestScoreResult = Results_T.OrderByDescending(r => r.Score).FirstOrDefault();

                    if (highestScoreResult == null) { break; }

                    List<MatchingResult> Results_T2 = new List<MatchingResult>();

                    int OrgMag = 2;
                    maxCount--;
                    using (Mat ImageSubMatPy = ImageSrc.Resize(new OpenCvSharp.Size((int)(ImageSrc.Width / OrgMag), (int)(ImageSrc.Height / OrgMag))))
                    using (Mat ImageTplPy = imageTemplate.Resize(new OpenCvSharp.Size((int)(imageTemplate.Width / OrgMag), (int)(imageTemplate.Height / OrgMag))))
                    {
                        double ImageTplW = ImageTplPy.Width;
                        double ImageTplH = ImageTplPy.Height;

                        Mat ImageSrc_T = Rotate(ImageTplPy, (highestScoreResult.Angle * -1), property.USE_PADDING_COLOR_WHITE);

                        //if (bDraw) { Cv2.ImShow("Tem", ImageSrc_T); }

                        OpenCvSharp.Point ptMax = new OpenCvSharp.Point();
                        double dMax = 0.0D;

                        Cv2.MatchTemplate(ImageSubMatPy, ImageSrc_T, ImageMatching, property.MATCH_MODE, null);
                        Cv2.MinMaxLoc(ImageMatching, out double dMinScore, out double dMaxScore, out OpenCvSharp.Point ptMinLocation, out OpenCvSharp.Point ptMaxLocation);

                        if (dMaxScore > property.SCORE_MIN)
                        {
                            ptMax = (dMaxScore > dMax) ? ptMaxLocation : ptMax;
                            dMax = (dMaxScore > dMax) ? dMaxScore : dMax;
                            OpenCvSharp.Point ptStart = new OpenCvSharp.Point(ptMaxLocation.X, ptMaxLocation.Y);
                            OpenCvSharp.Point ptEnd = new OpenCvSharp.Point(ptMaxLocation.X + (ImageTplW), ptMaxLocation.Y + (ImageTplH));

                            ptMaxLocation.X = (int)(ptMaxLocation.X * OrgMag);
                            ptMaxLocation.Y = (int)(ptMaxLocation.Y * OrgMag);
                            //ImageTplW = (int)(ImageTpl.Width);
                            //ImageTplH = (int)(ImageTpl.Height);

                            Rect rt = new Rect(new OpenCvSharp.Point(ptMaxLocation.X - 5, ptMaxLocation.Y - 5), new OpenCvSharp.Size(imageTemplate.Width + 10, imageTemplate .Height+ 10));                                
                            ImageSrc.Rectangle(rt, Scalar.Black, -1);                                
                          
                            ptStart.X = (ptStart.X * OrgMag);
                            ptStart.Y = (ptStart.Y * OrgMag);
                            ptEnd.X = (ptEnd.X * OrgMag);
                            ptEnd.Y = (ptEnd.Y * OrgMag);

                            dScore = dMaxScore * 100.0D;

                            Rect2f rtBounding = new Rect2f(ptStart.X + property.CvROI.X, ptStart.Y + property.CvROI.Y, ptEnd.X - ptStart.X, ptEnd.Y - ptStart.Y);
                            OpenCvSharp.Point2f ptCenter = new OpenCvSharp.Point2f((rtBounding.X + (rtBounding.X + rtBounding.Width)) / 2, (rtBounding.Y + (rtBounding.Y + rtBounding.Height)) / 2);

                            Results_T2.Add(new MatchingResult(0, dScore, ptCenter, rtBounding, highestScoreResult.Angle));
                        }
                    }
                    if (Results_T2.Count > 0) { results.Add(new MatchingResult(Results_T2[0].Index, Results_T2[0].Score, Results_T2[0].Center, CommonConverter.RectangleToRect(Results_T2[0].Bounding), Results_T2[0].Angle)); }                        
                }                   
            }
            swTaktTimems.Stop();

            if (!ImageMatching.IsDisposed && ImageMatching.IsEnabledDispose) ImageMatching.Dispose();
        

            return true;
        }
    }

}


