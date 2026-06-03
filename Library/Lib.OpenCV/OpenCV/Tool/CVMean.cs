using OpenCvSharp;
using System;
using System.Collections.Generic;
using Lib.OpenCV.Property;
using Lib.OpenCV.Result;
using Lib.Common;
using System.Reflection;

namespace Lib.OpenCV.Tool
{
    public class CVMean : COpenCVAlgorithmBase
    {
        public IOpenCVPropertyMean property;
        public List<CResultMean> results = new List<CResultMean>();

        public CVMean() { }        
        
        public void SetProperty(IOpenCVPropertyMean property) => this.property = property;

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

        public void MultiRun()
        {
            try
            {
                results.Clear();

                if (COpenCVHelper.IsImageEmpty(imageSource))
                {
                    CLOG.ABNORMAL("Image is Empty");
                    return;
                }
                using (Mat ImageSrc = imageSource.Clone())
                {
                    if (COpenCVHelper.IsImageEmpty(imageSource)) return;

                    Lib.OpenCV.COpenCVHelper.SetImageChannel1(ImageSrc);

                    for (int i = 0; i < property.CvROIS.Count; i++)
                    {
                        if (property.CvROIS[i].Width == 0 || property.CvROIS[i].Height == 0)
                        {
                            property.CvROIS[i] = new OpenCvSharp.Rect(0, 0, imageSource.Width, imageSource.Height);
                        }

                        Mat ImageMean = property.USE_ROI ? ImageSrc.SubMat(property.CvROIS[i]) : ImageSrc.Clone();

                        if (property.USE_THRESHOLD) { Cv2.Threshold(ImageMean, ImageMean, property.THRESHOLD, 255, property.THRESHOLD_TYPES); }
                        else if (property.USE_ADAPTIVE_THRESHOLD) { Cv2.AdaptiveThreshold(ImageMean, ImageMean, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight); }

                        if (property.USE_BITWISENOT) Cv2.BitwiseNot(ImageMean, ImageMean);

                        double Mean = 0;
                        double MeanStdDev = 0;

                        switch (property.MEAN_TYPES)
                        {
                            case MeanType.Mean:
                                Mean = Cv2.Mean(ImageMean).Val0;
                                Mean = Math.Round(Mean, 1);
                                results.Add(new CResultMean(0, Mean, Lib.Common.CConverter.RectToRectangle(property.CvROIS[i])));
                                break;
                            case MeanType.MeanStdDev:
                                Cv2.MeanStdDev(ImageMean, out Scalar mean, out Scalar stddev);
                                MeanStdDev = double.Parse(stddev[0].ToString("F1"));
                                results.Add(new CResultMean(0, MeanStdDev, Lib.Common.CConverter.RectToRectangle(property.CvROIS[i])));
                                break;
                        }
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return;
            }

            return;
        }

        public void SingleRun()
        {
            try
            {                
                results.Clear();

                if (COpenCVHelper.IsImageEmpty(imageSource))
                {
                    CLOG.ABNORMAL("Image is Empty");
                    return;
                }

                if (property.CvROI.Width == 0 || property.CvROI.Height == 0)
                {
                    property.CvROI = new OpenCvSharp.Rect(0, 0, imageSource.Width, imageSource.Height);
                }

                using (Mat ImageSrc = imageSource.Clone())
                {
                    if (COpenCVHelper.IsImageEmpty(imageSource)) return;
                    
                    Lib.OpenCV.COpenCVHelper.SetImageChannel1(ImageSrc);

                    Mat ImageMean = property.USE_ROI ? ImageSrc.SubMat(property.CvROI) : ImageSrc.Clone();

                    if (property.USE_THRESHOLD) { Cv2.Threshold(ImageMean, ImageMean, property.THRESHOLD, 255, property.THRESHOLD_TYPES); }
                    else if (property.USE_ADAPTIVE_THRESHOLD) { Cv2.AdaptiveThreshold(ImageMean, ImageMean, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight); }

                    if (property.USE_BITWISENOT) Cv2.BitwiseNot(ImageMean, ImageMean);

                    double Mean = 0;
                    double MeanStdDev = 0;

                    switch (property.MEAN_TYPES)
                    {
                        case MeanType.Mean:
                            Mean = Cv2.Mean(ImageMean).Val0;
                            Mean = Math.Round(Mean, 1);
                            results.Add(new CResultMean(0, Mean, Lib.Common.CConverter.RectToRectangle(property.CvROI)));          
                            break;
                        case MeanType.MeanStdDev:
                            Cv2.MeanStdDev(ImageMean, out Scalar mean, out Scalar stddev);
                            MeanStdDev = double.Parse(stddev[0].ToString("F1"));
                            results.Add(new CResultMean(0, MeanStdDev, Lib.Common.CConverter.RectToRectangle(property.CvROI)));                  
                            break;
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return;
            }

            return;
        }
    }
}
