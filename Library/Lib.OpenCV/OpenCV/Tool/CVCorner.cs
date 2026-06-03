using System;
using System.Collections.Generic;
using System.Reflection;
using Lib.Common;
using Lib.OpenCV.Property;
using Lib.OpenCV.Result;
using OpenCvSharp;

namespace Lib.OpenCV.Tool
{
    public partial class CVCorner : COpenCVAlgorithmBase
    {
        public IOpenCVPropertyContour property;

        public List<CResultCorner> results = new List<CResultCorner>();        
        public CVCorner() { }

        public void SetProperty(IOpenCVPropertyContour property) => this.property = property;

        public override void Run()
        {
            try
            {
                results.Clear();

                if (COpenCVHelper.IsImageEmpty(imageSource))
                {
                    CLOG.ABNORMAL( "Image is Empty");
                    return;
                }

                if (property.CvROI.Width == 0 || property.CvROI.Height == 0)
                {
                    property.CvROI = new OpenCvSharp.Rect(0, 0, imageSource.Width, imageSource.Height);
                }

                using (Mat ImageSrc = imageSource.Clone())
                {
                    if (COpenCVHelper.IsImageEmpty(imageSource)) return;
                    imageResult = ImageSrc.Clone();

                    Lib.OpenCV.COpenCVHelper.SetImageChannel1(ImageSrc);
                    Lib.OpenCV.COpenCVHelper.SetImageChannel3(imageResult);
                    
                    Mat ImageCorner = property.USE_ROI ? ImageSrc.SubMat(property.CvROI) : ImageSrc.Clone();

                    if (property.USE_THRESHOLD) { Cv2.Threshold(ImageCorner, ImageCorner, property.THRESHOLD, 255, property.THRESHOLD_TYPES); }
                    else if (property.USE_ADAPTIVE_THRESHOLD) { Cv2.AdaptiveThreshold(ImageCorner, ImageCorner, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight); }

                    // 컨투어 자체가 검은색 영역에서 흰색영역을 검출하는 알고리즘 
                    // 검출하려고 하는 물체가 검은색이면 반전으로 검출해야함
                    if (property.USE_BITWISENOT) Cv2.BitwiseNot(ImageCorner, ImageCorner);

                    Point2f[] corners = Cv2.GoodFeaturesToTrack(ImageCorner, 1000, 0.1, 5, null, 3, true, 0);
                    Point2f[] sub_corners = Cv2.CornerSubPix(ImageCorner, corners, new OpenCvSharp.Size(3, 3), new OpenCvSharp.Size(-1, -1), TermCriteria.Both(10, 0.03));

                    if (property.USE_ROI)
                    {
                        for (int i = 0; i < corners.Length; i++)
                        {
                            corners[i].X = corners[i].X + property.CvROI.X;
                            corners[i].Y = corners[i].Y + property.CvROI.Y;
                        }

                        for (int i = 0; i < sub_corners.Length; i++)
                        {
                            sub_corners[i].X = sub_corners[i].X + property.CvROI.X;
                            sub_corners[i].Y = sub_corners[i].Y + property.CvROI.Y;
                        }
                    }

                    for (int i = 0; i < corners.Length; i++)
                    {
                        OpenCvSharp.Point pt = new OpenCvSharp.Point((int)corners[i].X, (int)corners[i].Y);
                        Cv2.Circle(imageResult, pt, 5, Scalar.Yellow, Cv2.FILLED);
                    }

                    for (int i = 0; i < sub_corners.Length; i++)
                    {
                        OpenCvSharp.Point pt = new OpenCvSharp.Point((int)sub_corners[i].X, (int)sub_corners[i].Y);
                        Cv2.Circle(imageResult, pt, 5, Scalar.Red, Cv2.FILLED);
                    }
                }

            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
            }
        }
    }
}

