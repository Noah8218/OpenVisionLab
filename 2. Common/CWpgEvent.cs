using OpenVisionLab._1._Core;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.Common;
using Lib.OpenCV;
using OpenCvSharp;
using System;
using System.Drawing;
using System.Reflection;

namespace OpenVisionLab._2._Common
{
    public sealed class CWpgEvent
    {
        private readonly Func<IDisplayManager> displayManagerAccessor;

        public CWpgEvent(Func<IDisplayManager> displayManagerAccessor)
        {
            this.displayManagerAccessor = displayManagerAccessor ?? (() => DisplayManagerService.Default);
        }

        private IDisplayManager DisplayManager => displayManagerAccessor() ?? DisplayManagerService.Default;

        public void Wpg_SelectedObjectsChanged(object sender, EventArgs e)
        {
            Wpg_PropertyValueChanged(sender, null);
        }

        public void Wpg_PropertyValueChanged(object sender, System.Windows.Controls.WpfPropertyGrid.PropertyValueChangedEventArgs e)
        {
            try
            {
                var propertyGrid = (System.Windows.Controls.WpfPropertyGrid.PropertyGrid)sender;
                if (propertyGrid.SelectedObject == null) return;
                if(e != null && propertyGrid.SelectedObject is COpenCVPropertyBase)
                {
                    var Prop = (COpenCVPropertyBase)propertyGrid.SelectedObject;
                    switch (e.Property.Name.ToUpper())
                    {
                        case nameof(Prop.THRESHOLD):
                            RunThreshold(Prop, Prop.THRESHOLD);
                            break;
                        case nameof(Prop.ADAPTIVE_THRESHOLD):
                            RunAdaptiveThreshold(Prop, Prop.ADAPTIVE_THRESHOLD);
                            break;
                            //case "BLACK_THRESHOLD":                        
                            //    var Prop2 = (CPropertyBlob)propertyGrid.SelectedObject;
                            //    RunThreshold(Prop, Prop2.BLACK_THRESHOLD);
                            //    break;
                            //case "WHITE_THRESHOLD":
                            //    Prop2 = (CPropertyBlob)propertyGrid.SelectedObject;
                            //    RunThreshold(Prop, Prop2.WHITE_THRESHOLD);
                            //    break;
                    }
                }                
                SetCOpencvPropertyBase(propertyGrid);
                SetCPropertyLineGuage(propertyGrid);
                SetCPropertyMatching(propertyGrid);
                SetCPropertyContour(propertyGrid);      
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private static void SetCPropertyMatching(System.Windows.Controls.WpfPropertyGrid.PropertyGrid propertyGrid)
        {
            if (propertyGrid.SelectedObject is CPropertyMatching)
            {
                var match = (CPropertyMatching)propertyGrid.SelectedObject;
                if (match.USE_FIND_ANGLE)
                {
                    propertyGrid.Properties[nameof(match.FIND_ANGLE)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(match.FIND_ANGLE_MAX)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(match.FIND_ANGLE_MIN)].IsBrowsable = true;
                }
                else
                {
                    propertyGrid.Properties[nameof(match.FIND_ANGLE)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(match.FIND_ANGLE_MAX)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(match.FIND_ANGLE_MIN)].IsBrowsable = false;
                }

                if (match.USE_CANNY)
                {
                    propertyGrid.Properties[nameof(match.CANNY_HIGH)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(match.CANNY_LOW)].IsBrowsable = true;
                }
                else
                {
                    propertyGrid.Properties[nameof(match.CANNY_HIGH)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(match.CANNY_LOW)].IsBrowsable = false;
                }
            }
        }
        private static void SetCPropertyContour(System.Windows.Controls.WpfPropertyGrid.PropertyGrid propertyGrid)
        {
            if (propertyGrid.SelectedObject is CPropertyContour)
            {
                var contour = (CPropertyContour)propertyGrid.SelectedObject;
                if (contour.USE_DRAW_IMAGE)
                {
                    propertyGrid.Properties[nameof(contour.DrawColor)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(contour.DrawThickness)].IsBrowsable = true;
                }
                else
                {
                    propertyGrid.Properties[nameof(contour.DrawColor)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(contour.DrawThickness)].IsBrowsable = false;
                }

                if (contour.USE_APPROXPOLYDP)
                {
                    propertyGrid.Properties[nameof(contour.EPSILON)].IsBrowsable = true;
                }
                else
                {
                    propertyGrid.Properties[nameof(contour.EPSILON)].IsBrowsable = false;
                }
            }
        }
        private static void SetCPropertyLineGuage(System.Windows.Controls.WpfPropertyGrid.PropertyGrid propertyGrid)
        {
            if (propertyGrid.SelectedObject is CPropertyLineGuage)
            {
                var line = (CPropertyLineGuage)propertyGrid.SelectedObject;
                if (line.USE_MANUAL_ANGLE)
                {
                    propertyGrid.Properties[nameof(line.MANUAL_ANGLE_VALUE)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(line.POINT_RANGE)].IsBrowsable = false;
                }
                else
                {
                    propertyGrid.Properties[nameof(line.MANUAL_ANGLE_VALUE)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(line.POINT_RANGE)].IsBrowsable = true;
                }
            }
        }

        private static void SetCOpencvPropertyBase(System.Windows.Controls.WpfPropertyGrid.PropertyGrid propertyGrid)
        {
            if (propertyGrid.SelectedObject is COpenCVPropertyBase)
            {
                var Prop = (COpenCVPropertyBase)propertyGrid.SelectedObject;
                if (Prop.USE_ROI)
                {
                    propertyGrid.Properties[nameof(Prop.USE_MULTI_ROI)].IsBrowsable = true;

                    if (Prop.USE_MULTI_ROI)
                    {
                        propertyGrid.Properties[nameof(Prop.CvROI)].IsBrowsable = false;
                        propertyGrid.Properties[nameof(Prop.CvROIS)].IsBrowsable = true;
                    }
                    else
                    {
                        propertyGrid.Properties[nameof(Prop.CvROI)].IsBrowsable = true;
                        propertyGrid.Properties[nameof(Prop.CvROIS)].IsBrowsable = false;
                    }
                }
                else
                {
                    propertyGrid.Properties[nameof(Prop.USE_MULTI_ROI)].SetValue(false);
                    propertyGrid.Properties[nameof(Prop.USE_MULTI_ROI)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.CvROI)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.CvROIS)].IsBrowsable = false;
                }

                if (Prop.USE_THRESHOLD)
                {
                    propertyGrid.Properties[nameof(Prop.THRESHOLD)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(Prop.THRESHOLD_TYPES)].IsBrowsable = true;
                }
                else
                {
                    propertyGrid.Properties[nameof(Prop.THRESHOLD)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.THRESHOLD_TYPES)].IsBrowsable = false;
                }

                if (Prop.USE_ADAPTIVE_THRESHOLD)
                {
                    propertyGrid.Properties[nameof(Prop.ADAPTIVE_THRESHOLD)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(Prop.ADAPTIVE_THRESHOLD_TYPES)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(Prop.ADAPTIVE_THRESHOLD_ALGORITHM)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(Prop.Weight)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(Prop.BlockSize)].IsBrowsable = true;
                }
                else
                {
                    propertyGrid.Properties[nameof(Prop.ADAPTIVE_THRESHOLD)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.ADAPTIVE_THRESHOLD_TYPES)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.ADAPTIVE_THRESHOLD_ALGORITHM)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.Weight)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.BlockSize)].IsBrowsable = false;
                }
            }        
        }

        private void RunThreshold(COpenCVPropertyBase editor, double value)
        {
            if (COpenCVHelper.IsImageEmpty(DisplayManager.GetImageSrc())) return;

            using (Mat imageSrc = DisplayManager.SelectedItem != DEFINE.Threshold ? DisplayManager.GetImageSrc().Clone() : Lib.Common.CImageConverter.ToMat(DisplayManager.GetLayerImage("Main")).Clone())
            {
                Cv2.Threshold(imageSrc, imageSrc, (int)value, 255, CUtil.ParseEnum<ThresholdTypes>(editor.THRESHOLD_TYPES.ToString()));
                DisplayManager.CreateLayerDisplay(Lib.Common.CImageConverter.ToBitmap(imageSrc), DEFINE.Threshold, false);
            }
        }

        private void RunAdaptiveThreshold(COpenCVPropertyBase editor, double value)
        {
            if (COpenCVHelper.IsImageEmpty(DisplayManager.GetImageSrc())) return;

            using (Mat imageSrc = DisplayManager.SelectedItem != nameof(DEFINE.AdaptiveThreshold) ? DisplayManager.GetImageSrc().Clone() : Lib.Common.CImageConverter.ToMat(DisplayManager.GetLayerImage("Main")).Clone())
            {
                COpenCVHelper.SetImageChannel1(imageSrc);
                Cv2.AdaptiveThreshold(imageSrc, imageSrc, (int)value, editor.ADAPTIVE_THRESHOLD_ALGORITHM, editor.ADAPTIVE_THRESHOLD_TYPES, editor.BlockSize, editor.Weight);
                DisplayManager.CreateLayerDisplay(Lib.Common.CImageConverter.ToBitmap(imageSrc), nameof(DEFINE.AdaptiveThreshold), false);
            }
        }
    }
}
