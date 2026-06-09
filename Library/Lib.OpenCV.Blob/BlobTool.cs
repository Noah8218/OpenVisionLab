using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Lib.Common;
using Lib.OpenCV.Property;
using Lib.OpenCV.Result;
using OpenCvSharp;
using OpenCvSharp.Blob;

namespace Lib.OpenCV.Blob
{
    public partial class BlobTool : OpenCvAlgorithmBase
    {
        public IOpenCVPropertyBlob property;
        public List<BlobResult> results = new List<BlobResult>();

        public BlobTool() { }

        public void SetProperty(IOpenCVPropertyBlob propertyBase) => property = propertyBase;

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

        protected bool SingleRun()
        {
                        swTaktTimems.Restart();
            results.Clear();

            if (OpenCvHelper.IsImageEmpty(imageSource))
            {

                return false;
            }

            if (property.CvROI.Width == 0 || property.CvROI.Height == 0)
            {
                property.CvROI = new Rect(0, 0, imageSource.Width, imageSource.Height);
            }

            if (OpenCvHelper.IsImageEmpty(imageSource)) return false;
            OpenCvHelper.SetImageChannel1(imageSource);

            Mat ImageBlob = property.USE_ROI ? imageSource.SubMat(property.CvROI) : imageSource.Clone();

            using (Mat ImageSrc = ImageBlob)
            {
                if (property.USE_THRESHOLD) { Cv2.Threshold(ImageBlob, ImageBlob, property.THRESHOLD, 255, property.THRESHOLD_TYPES); }
                else if (property.USE_ADAPTIVE_THRESHOLD) { Cv2.AdaptiveThreshold(ImageBlob, ImageBlob, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight); }

                // 검은색 영역에서 흰 물체를 라벨링하여 검출하기 때문
                // 검출하려고 하는 물체가 검은색이면 반전으로 검출해야함
                if (property.USE_BITWISENOT) Cv2.BitwiseNot(ImageBlob, ImageBlob);

                CvBlobs Blobs = new CvBlobs();
                Blobs.Label(ImageBlob);
                Blobs.FilterByArea(property.MIN_AREA, property.MAX_AREA);

                ConcurrentBag<BlobResult> defectsS = new ConcurrentBag<BlobResult>();
                Parallel.ForEach(Blobs, (item, state, index) =>
                {
                    CvBlob b = item.Value;

                    Rect rect = new Rect();
                    Point2d Center = new Point2d();

                    if (property.USE_ROI)
                    {
                        rect.X = b.Rect.X + property.CvROI.X;
                        rect.Y = b.Rect.Y + property.CvROI.Y;
                        rect.Width = b.Rect.Width;
                        rect.Height = b.Rect.Height;

                        Center.X = b.Centroid.X + property.CvROI.X;
                        Center.Y = b.Centroid.Y + property.CvROI.Y;
                    }
                    else
                    {
                        rect = b.Rect;
                        Center = b.Centroid;
                    }

                    bool Masking = false;
                    for (int i = 0; i < property.CvMASKS.Count; i++)
                    {
                        // ==> IntersectsWith 사용 이물이 걸치기만해도 필터 나옴                        
                        if (property.CvMASKS[i].Contains(rect))
                        {
                            Masking = true;
                            break;
                        }
                    }

                    if (!Masking)
                    {
                        defectsS.Add(new BlobResult((int)index, b.Area, Center, rect, b.Angle()));
                    }
                });
                results = defectsS.OrderBy(c => c.Index).ToList();
            }

            swTaktTimems.Stop();
        
            return true;
        }

        protected bool MultiRun()
        {
                        swTaktTimems.Restart();
            results.Clear();

            if (OpenCvHelper.IsImageEmpty(imageSource))
            {

                return false;
            }

            if (OpenCvHelper.IsImageEmpty(imageSource)) return false;
            OpenCvHelper.SetImageChannel1(imageSource);

            for (int i = 0; i < property.CvROIS.Count; i++)
            {
                if (property.CvROIS[i].Width == 0 || property.CvROIS[i].Height == 0)
                {
                    property.CvROIS[i] = new Rect(0, 0, imageSource.Width, imageSource.Height);
                }

                Mat ImageBlob = null;

                if (property.USE_ROI) { ImageBlob = imageSource.SubMat(property.CvROIS[i]); }
                else { ImageBlob = imageSource.Clone(); }

                using (Mat ImageSrc = ImageBlob)
                {
                    if (property.USE_THRESHOLD) { Cv2.Threshold(ImageBlob, ImageBlob, property.THRESHOLD, 255, property.THRESHOLD_TYPES); }
                    else if (property.USE_ADAPTIVE_THRESHOLD) { Cv2.AdaptiveThreshold(ImageBlob, ImageBlob, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight); }

                    // 검은색 영역에서 흰 물체를 라벨링하여 검출하기 때문
                    // 검출하려고 하는 물체가 검은색이면 반전으로 검출해야함
                    if (property.USE_BITWISENOT) Cv2.BitwiseNot(ImageBlob, ImageBlob);

                    Stopwatch sw_TaktTimems2 = Stopwatch.StartNew();

                    CvBlobs Blobs = new CvBlobs();
                    Blobs.Label(ImageBlob);
                    Blobs.FilterByArea(property.MIN_AREA, property.MAX_AREA);

                    ConcurrentBag<BlobResult> defectsS = new ConcurrentBag<BlobResult>();
                    Parallel.ForEach(Blobs, (item, state, index) =>
                    {
                        CvBlob b = item.Value;

                        Rect rect = new Rect();
                        Point2d Center = new Point2d();

                        if (property.USE_ROI)
                        {
                            rect.X = b.Rect.X + property.CvROIS[i].X;
                            rect.Y = b.Rect.Y + property.CvROIS[i].Y;
                            rect.Width = b.Rect.Width;
                            rect.Height = b.Rect.Height;

                            Center.X = b.Centroid.X + property.CvROIS[i].X;
                            Center.Y = b.Centroid.Y + property.CvROIS[i].Y;
                        }
                        else
                        {
                            rect = b.Rect;
                            Center = b.Centroid;
                        }

                        bool Masking = false;
                        for (int j = 0; j < property.CvMASKS.Count; j++)
                        {
                            // ==> IntersectsWith 사용 이물이 걸치기만해도 필터 나옴                        
                            if (property.CvMASKS[j].Contains(rect))
                            {
                                Masking = true;
                                break;
                            }
                        }

                        if (!Masking)
                        {
                            defectsS.Add(new BlobResult((int)index, b.Area, Center, rect, b.Angle()));
                        }
                    });
                    results.AddRange(defectsS.OrderBy(c => c.Index).ToList());
                    sw_TaktTimems2.Stop();
                }
            }

            swTaktTimems.Stop();
        

            return true;
        }
    }
}

