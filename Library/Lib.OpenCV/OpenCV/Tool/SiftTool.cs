using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lib.Common;
using Lib.OpenCV.Property;
using Lib.OpenCV.Result;
using OpenCvSharp;
using OpenCvSharp.Features2D;

namespace Lib.OpenCV.Tool
{
    public partial class SiftTool : OpenCvAlgorithmBase
    {
        public IOpenCVPropertyFeatureSIFT property;
        public List<MatchingResult> results = new List<MatchingResult>();

        public SiftTool() { }

        public void SetProperty(IOpenCVPropertyFeatureSIFT propertyBase) => property = propertyBase;

        public void SetTemplateImage(Mat Image) => imageTemplate = Image;

        public override void Run()
        {
            if(property.USE_MULTI_ROI)
            {
                MultiRun();
            }
            else
            {
                SingleRun();
            }            
        }

        protected bool MultiRun()
        {
                        swTaktTimems.Restart();
            results.Clear();

            if (OpenCvHelper.IsImageEmpty(imageSource))
            {

                return false;
            }
            //imageResult = imageSource.Clone();
            //OpenCvHelper.SetImageChannel3(imageResult);


            if (OpenCvHelper.IsImageEmpty(imageSource)) return false;
            OpenCvHelper.SetImageChannel1(imageSource);
            OpenCvHelper.SetImageChannel1(imageTemplate);

            using (Mat ImageTemplate = imageTemplate.Clone())
            {
                for(int i =0; i < property.CvROIS.Count; i++)
                {
                    if (property.CvROIS[i].Width == 0 || property.CvROIS[i].Height == 0)
                    {
                        property.CvROIS[i] = new Rect(0, 0, imageSource.Width, imageSource.Height);
                    }

                    using (Mat ImageSIFT = property.USE_ROI ? imageSource.SubMat(property.CvROIS[i]) : imageSource.Clone())
                    {
                        /*
                    * 속도와 효율성: ORB는 SIFT에 비해 훨씬 빠르며 효율적입니다. ORB는 특징점 검출을 위해 FAST(Features from Accelerated Segment Test) 알고리즘을 사용하고, 특징점 설명을 위해 BRIEF(Binary Robust Independent Elementary Features) 알고리즘을 사용합니다. 이 두 알고리즘 모두 계산적으로 매우 효율적입니다. 반면에, SIFT는 키포인트 검출과 설명을 위해 복잡한 계산을 수행하므로 처리 시간이 더 길어질 수 있습니다.
                       특징점의 수: 일반적으로, ORB는 SIFT에 비해 더 많은 특징점을 검출합니다. 이는 ORB의 FAST 알고리즘이 코너 특징점을 검출하는 데 특화되어 있기 때문입니다.
                       회전과 크기 불변성: SIFT는 이미지의 회전과 크기 변화에 대해 불변입니다. 이는 SIFT가 각 특징점의 방향과 스케일 정보를 계산하기 때문입니다. 반면에, ORB는 회전에 대해 불변이지만 크기 변화에 대해서는 불변이 아닙니다.
                       특징점 설명자: SIFT는 128차원의 실수 벡터를 사용하여 특징점을 설명합니다. 이 설명자는 각 특징점 주변의 그래디언트 정보를 기반으로 합니다. 반면에, ORB는 256차원의 이진 벡터를 사용하여 특징점을 설명합니다. 이 설명자는 특징점 주변의 간단한 이진 테스트를 기반으로 합니다.
                    */

                        if (property.USE_THRESHOLD)
                        {
                            Cv2.Threshold(ImageSIFT, ImageSIFT, property.THRESHOLD, 255, property.THRESHOLD_TYPES);
                            Cv2.Threshold(ImageTemplate, ImageTemplate, property.THRESHOLD, 255, property.THRESHOLD_TYPES);
                        }
                        else if (property.USE_ADAPTIVE_THRESHOLD)
                        {
                            Cv2.AdaptiveThreshold(ImageSIFT, ImageSIFT, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight);
                            Cv2.AdaptiveThreshold(ImageTemplate, ImageTemplate, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight);
                        }

                        if (property.USE_BITWISENOT)
                        {
                            Cv2.BitwiseNot(ImageSIFT, ImageSIFT);
                            Cv2.BitwiseNot(ImageTemplate, ImageTemplate);
                        }

                        var sift = SIFT.Create();

                        // 각 이미지에서 키포인트 및 디스크립터 계산
                        KeyPoint[] keypoints1, keypoints2;
                        Mat descriptors1 = new Mat(), descriptors2 = new Mat();

                        sift.DetectAndCompute(ImageTemplate, null, out keypoints1, descriptors1);
                        sift.DetectAndCompute(ImageSIFT, null, out keypoints2, descriptors2);

                        // BFMatcher를 사용하여 특징점 매칭
                        BFMatcher matcher = new BFMatcher(NormTypes.L2);
                        // k값은 2개, 예를 들어서 5개면  첫 번째 이미지의 각 특징점에 대해 두 번째 이미지에서 가장 가까운 다섯 개의 특징점이 반환됩니다.                     
                        DMatch[][] knnMatches = matcher.KnnMatch(descriptors1, descriptors2, 2);

                        // Lowe's ratio test를 사용하여 좋은 매치 선택
                        float ratioThreshold = (float)property.SCORE_MIN; // Can be tuned
                        List<DMatch> goodMatches = new List<DMatch>();
                        foreach (DMatch[] match in knnMatches)
                        {
                            if (match[0].Distance < ratioThreshold * match[1].Distance)
                            {
                                goodMatches.Add(match[0]);
                            }
                        }

                        // Extract location of good matches
                        Point2f[] srcPts = goodMatches.Select(m => keypoints1[m.QueryIdx].Pt).ToArray();
                        Point2f[] dstPts = goodMatches.Select(m => keypoints2[m.TrainIdx].Pt).ToArray();
                        Point2d[] srcPtsD = ConvertPoint2fToPoint2d(srcPts);
                        Point2d[] dstPtsD = ConvertPoint2fToPoint2d(dstPts);
                        // Calculate homography
                        double ransacReprojThreshold = property.RANSAC_REPROJ_THRESHOLD;
                        Mat homography = Cv2.FindHomography(srcPtsD, dstPtsD, HomographyMethods.Ransac, ransacReprojThreshold, new Mat());

                        // Define source image region of interest
                        Size size = ImageTemplate.Size();
                        Point2f[] pts = new Point2f[]
                        {
                        new Point2f(0, 0),
                        new Point2f(0, size.Height - 1),
                        new Point2f(size.Width - 1, size.Height - 1),
                        new Point2f(size.Width - 1, 0)
                        };

                        // Apply homography to source ROI
                        Point2f[] dst = Cv2.PerspectiveTransform(pts, homography);

                        if (property.USE_ROI)
                        {
                            for (int j = 0; j < dst.Length; j++)
                            {
                                dst[j].X = dst[j].X + property.CvROIS[i].X;
                                dst[j].Y = dst[j].Y + property.CvROIS[i].Y;
                            }
                        }

                        // Ensure dst has exactly 4 points
                        if (dst.Length != 4)
                        {
                            throw new ArgumentException("dst must have exactly 4 points");
                        }

                        // Create a RotatedRect
                        Point2f center = new Point2f(
                            (dst[0].X + dst[2].X) / 2,
                            (dst[0].Y + dst[2].Y) / 2);
                        Size2f size2f = new Size2f(
                            (float)Math.Sqrt(Math.Pow(dst[1].X - dst[0].X, 2) + Math.Pow(dst[1].Y - dst[0].Y, 2)),
                            (float)Math.Sqrt(Math.Pow(dst[3].X - dst[0].X, 2) + Math.Pow(dst[3].Y - dst[0].Y, 2)));
                        float angle = (float)(Math.Atan2(dst[1].Y - dst[0].Y, dst[1].X - dst[0].X) * 180 / Math.PI);

                        RotatedRect rect = new RotatedRect(center, size2f, angle);

                        // Extract the angle
                        float rotationAngle = rect.Angle;

                        // Assuming you have a RotatedRect named 'rotatedRect'
                        //Point2f[] vertices = Cv2.BoxPoints(rect);
                        Point[] points = dst.Select(p => new Point((int)Math.Round(p.X), (int)Math.Round(p.Y))).ToArray();

                        OpenCvSharp.Rect rectangle = new OpenCvSharp.Rect(Math.Abs((int)((size2f.Width / 2) - center.X)), Math.Abs((int)((size2f.Height / 2) - center.Y)), (int)Math.Round(size2f.Width), (int)Math.Round(size2f.Height));

                        results.Add(new MatchingResult(results.Count, 0, center, ConvertRectToRect2f(rectangle), rotationAngle));

                        // Draw the rectangle 
                        //Cv2.Polylines(imageResult, new Point[][] { points }, true, new Scalar(0, 255, 0), thickness: 2);
                        //Cv2.DrawMatches(ImageTemplate, keypoints1, ImageORB, keypoints2, goodMatches, imageResult, Scalar.All(-1), Scalar.All(-1), null, DrawMatchesFlags.NotDrawSinglePoints);                                        
                    }
                } 
            }

            swTaktTimems.Stop();
        
            return true;
        }

        protected bool SingleRun()
        {
                        swTaktTimems.Restart();
            results.Clear();

            if (OpenCvHelper.IsImageEmpty(imageSource))
            {

                return false;
            }
            //imageResult = imageSource.Clone();
            //OpenCvHelper.SetImageChannel3(imageResult);


            if (OpenCvHelper.IsImageEmpty(imageSource)) return false;
            OpenCvHelper.SetImageChannel1(imageSource);
            OpenCvHelper.SetImageChannel1(imageTemplate);
            
            using (Mat ImageTemplate = imageTemplate.Clone())
            {
                if (property.CvROI.Width == 0 || property.CvROI.Height == 0)
                {
                    property.CvROI = new Rect(0, 0, imageSource.Width, imageSource.Height);
                }
                using (Mat ImageSIFT = property.USE_ROI ? imageSource.SubMat(property.CvROI) : imageSource.Clone())
                {
                    /*
                * 속도와 효율성: ORB는 SIFT에 비해 훨씬 빠르며 효율적입니다. ORB는 특징점 검출을 위해 FAST(Features from Accelerated Segment Test) 알고리즘을 사용하고, 특징점 설명을 위해 BRIEF(Binary Robust Independent Elementary Features) 알고리즘을 사용합니다. 이 두 알고리즘 모두 계산적으로 매우 효율적입니다. 반면에, SIFT는 키포인트 검출과 설명을 위해 복잡한 계산을 수행하므로 처리 시간이 더 길어질 수 있습니다.
                   특징점의 수: 일반적으로, ORB는 SIFT에 비해 더 많은 특징점을 검출합니다. 이는 ORB의 FAST 알고리즘이 코너 특징점을 검출하는 데 특화되어 있기 때문입니다.
                   회전과 크기 불변성: SIFT는 이미지의 회전과 크기 변화에 대해 불변입니다. 이는 SIFT가 각 특징점의 방향과 스케일 정보를 계산하기 때문입니다. 반면에, ORB는 회전에 대해 불변이지만 크기 변화에 대해서는 불변이 아닙니다.
                   특징점 설명자: SIFT는 128차원의 실수 벡터를 사용하여 특징점을 설명합니다. 이 설명자는 각 특징점 주변의 그래디언트 정보를 기반으로 합니다. 반면에, ORB는 256차원의 이진 벡터를 사용하여 특징점을 설명합니다. 이 설명자는 특징점 주변의 간단한 이진 테스트를 기반으로 합니다.
                */

                    if (property.USE_THRESHOLD)
                    {
                        Cv2.Threshold(ImageSIFT, ImageSIFT, property.THRESHOLD, 255, property.THRESHOLD_TYPES);
                        Cv2.Threshold(ImageTemplate, ImageTemplate, property.THRESHOLD, 255, property.THRESHOLD_TYPES);
                    }
                    else if (property.USE_ADAPTIVE_THRESHOLD) 
                    {
                        Cv2.AdaptiveThreshold(ImageSIFT, ImageSIFT, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight);
                        Cv2.AdaptiveThreshold(ImageTemplate, ImageTemplate, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight);
                    }

                    if (property.USE_BITWISENOT)
                    {
                        Cv2.BitwiseNot(ImageSIFT, ImageSIFT);
                        Cv2.BitwiseNot(ImageTemplate, ImageTemplate);
                    }

                    var sift = SIFT.Create();

                    // 각 이미지에서 키포인트 및 디스크립터 계산
                    KeyPoint[] keypoints1, keypoints2;
                    Mat descriptors1 = new Mat(), descriptors2 = new Mat();

                    sift.DetectAndCompute(ImageTemplate, null, out keypoints1, descriptors1);
                    sift.DetectAndCompute(ImageSIFT, null, out keypoints2, descriptors2);

                    // BFMatcher를 사용하여 특징점 매칭
                    BFMatcher matcher = new BFMatcher(NormTypes.L2);
                    // k값은 2개, 예를 들어서 5개면  첫 번째 이미지의 각 특징점에 대해 두 번째 이미지에서 가장 가까운 다섯 개의 특징점이 반환됩니다.                     
                    DMatch[][] knnMatches = matcher.KnnMatch(descriptors1, descriptors2, 2);

                    // Lowe's ratio test를 사용하여 좋은 매치 선택
                    float ratioThreshold = (float)property.SCORE_MIN; // Can be tuned
                    List<DMatch> goodMatches = new List<DMatch>();
                    foreach (DMatch[] match in knnMatches)
                    {
                        if (match[0].Distance < ratioThreshold * match[1].Distance)
                        {
                            goodMatches.Add(match[0]);
                        }
                    }

                    // Extract location of good matches
                    Point2f[] srcPts = goodMatches.Select(m => keypoints1[m.QueryIdx].Pt).ToArray();
                    Point2f[] dstPts = goodMatches.Select(m => keypoints2[m.TrainIdx].Pt).ToArray();
                    Point2d[] srcPtsD = ConvertPoint2fToPoint2d(srcPts);
                    Point2d[] dstPtsD = ConvertPoint2fToPoint2d(dstPts);
                    // Calculate homography
                    double ransacReprojThreshold = property.RANSAC_REPROJ_THRESHOLD;
                    Mat homography = Cv2.FindHomography(srcPtsD, dstPtsD, HomographyMethods.Ransac, ransacReprojThreshold, new Mat());

                    // Define source image region of interest
                    Size size = ImageTemplate.Size();
                    Point2f[] pts = new Point2f[]
                    {
                        new Point2f(0, 0),
                        new Point2f(0, size.Height - 1),
                        new Point2f(size.Width - 1, size.Height - 1),
                        new Point2f(size.Width - 1, 0)
                    };

                    // Apply homography to source ROI
                    Point2f[] dst = Cv2.PerspectiveTransform(pts, homography);

                    if (property.USE_ROI)
                    {
                        for (int i = 0; i < dst.Length; i++)
                        {
                            dst[i].X = dst[i].X + property.CvROI.X;
                            dst[i].Y = dst[i].Y + property.CvROI.Y;
                        }
                    }

                    // Ensure dst has exactly 4 points
                    if (dst.Length != 4)
                    {
                        throw new ArgumentException("dst must have exactly 4 points");
                    }

                    // Create a RotatedRect
                    Point2f center = new Point2f(
                        (dst[0].X + dst[2].X) / 2,
                        (dst[0].Y + dst[2].Y) / 2);
                    Size2f size2f = new Size2f(
                        (float)Math.Sqrt(Math.Pow(dst[1].X - dst[0].X, 2) + Math.Pow(dst[1].Y - dst[0].Y, 2)),
                        (float)Math.Sqrt(Math.Pow(dst[3].X - dst[0].X, 2) + Math.Pow(dst[3].Y - dst[0].Y, 2)));
                    float angle = (float)(Math.Atan2(dst[1].Y - dst[0].Y, dst[1].X - dst[0].X) * 180 / Math.PI);

                    RotatedRect rect = new RotatedRect(center, size2f, angle);

                    // Extract the angle
                    float rotationAngle = rect.Angle;

                    // Assuming you have a RotatedRect named 'rotatedRect'
                    //Point2f[] vertices = Cv2.BoxPoints(rect);
                    Point[] points = dst.Select(p => new Point((int)Math.Round(p.X), (int)Math.Round(p.Y))).ToArray();

                    OpenCvSharp.Rect rectangle = new OpenCvSharp.Rect(Math.Abs((int)((size2f.Width / 2) - center.X)), Math.Abs((int)((size2f.Height / 2) - center.Y)), (int)Math.Round(size2f.Width), (int)Math.Round(size2f.Height));

                    results.Add(new MatchingResult(results.Count, 0, center, ConvertRectToRect2f(rectangle), rotationAngle));

                    // Draw the rectangle 
                    //Cv2.Polylines(imageResult, new Point[][] { points }, true, new Scalar(0, 255, 0), thickness: 2);
                    //Cv2.DrawMatches(ImageTemplate, keypoints1, ImageORB, keypoints2, goodMatches, imageResult, Scalar.All(-1), Scalar.All(-1), null, DrawMatchesFlags.NotDrawSinglePoints);                                        
                }
            }

            swTaktTimems.Stop();
        
            return true;
        }

        public Rect2f ConvertRectToRect2f(Rect rect)
        {
            return new Rect2f(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public OpenCvSharp.Point2d ConvertPoint2fToPoint2d(OpenCvSharp.Point2f point)
        {
            return new OpenCvSharp.Point2d((double)point.X, (double)point.Y);
        }

        public OpenCvSharp.Point2d[] ConvertPoint2fToPoint2d(OpenCvSharp.Point2f[] points)
        {

            OpenCvSharp.Point2d[] point2fs = new OpenCvSharp.Point2d[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                point2fs[i] = new OpenCvSharp.Point2d(points[i].X, points[i].Y);
            }

            return point2fs;
        }
    }
}

