using OpenVisionLab._1._Core;
using Lib.Common;
using Lib.OpenCV;
using OpenCvSharp;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Media;

namespace OpenVisionLab
{
    public partial class FormThreshold : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private readonly IDisplayManager displayManager;

        public FormThreshold()
            : this(ApplicationRuntimeContext.CreateDefault().DisplayManager)
        {
        }

        public FormThreshold(IDisplayManager displayManager)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }
     
        // If the content won't display nicely, hide it
        private void ResizeEvent(object sender, EventArgs e)
        {
            this.Visible = this.Width > this.MinimumSize.Width && this.Height > this.MinimumSize.Height;
        }

        private bool ChangeSize = false;

        private void Form_VisibleChanged(object sender, EventArgs e)
        {
            if (!ChangeSize)
            {
                if (DockHandler.FloatPane == null) { return; }
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
                this.Refresh();
                ChangeSize = true;
            }
        }

        public void InitThresholdMenu()
        {
            cbThresholdMenu.Items.Add(ThresholdTypes.Binary);
            cbThresholdMenu.Items.Add(ThresholdTypes.BinaryInv);
            cbThresholdMenu.Items.Add(ThresholdTypes.Mask);
            cbThresholdMenu.Items.Add(ThresholdTypes.Otsu);
            cbThresholdMenu.Items.Add(ThresholdTypes.Tozero);
            cbThresholdMenu.Items.Add(ThresholdTypes.TozeroInv);
            cbThresholdMenu.Items.Add(ThresholdTypes.Triangle);
            cbThresholdMenu.Items.Add(ThresholdTypes.Trunc);
            cbThresholdMenu.SelectedIndex = 0;

            cbAdaptiveThresholdMenu.Items.Add(ThresholdTypes.Binary);
            cbAdaptiveThresholdMenu.Items.Add(ThresholdTypes.BinaryInv);
            cbAdaptiveThresholdMenu.SelectedIndex = 0;

            cbAdaptiveType.Items.Add(AdaptiveThresholdTypes.MeanC);
            cbAdaptiveType.Items.Add(AdaptiveThresholdTypes.GaussianC);
            cbAdaptiveType.SelectedIndex = 0;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            InitThresholdMenu();
        }

        private void trbThreshold_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (COpenCVHelper.IsImageEmpty(displayManager.GetImageSrc())) return;

                using (Mat imageSrc = displayManager.SelectedItem != DEFINE.Threshold ? displayManager.GetImageSrc().Clone() : Lib.Common.CImageConverter.ToMat(displayManager.GetLayerImage("Main")).Clone())
                {
                    if (displayManager.IsLayerRoiEmpty(displayManager.SelectedItem))
                    {
                        if (imageSrc.Channels() != 1) Cv2.CvtColor(imageSrc, imageSrc, ColorConversionCodes.RGB2GRAY);

                        Cv2.Threshold(imageSrc, imageSrc, trbThreshold.Value, 255, CUtil.ParseEnum<ThresholdTypes>(cbThresholdMenu.SelectedItem.ToString()));
                        displayManager.CreateLayerDisplay(Lib.Common.CImageConverter.ToBitmap(imageSrc), DEFINE.Threshold, false);
                    }
                    else
                    {
                        Rect r = CConverter.RectangleToRect(displayManager.GetLayerRoi(displayManager.SelectedItem));
                        Mat ImageRoi = imageSrc.SubMat(r);
                        Cv2.Threshold(ImageRoi, ImageRoi, trbThreshold.Value, 255, CUtil.ParseEnum<ThresholdTypes>(cbThresholdMenu.SelectedItem.ToString()));
                        Bitmap Reuslt = Lib.Common.CBitmapProcessing.OverlayImage(Lib.Common.CImageConverter.ToBitmap(imageSrc), Lib.Common.CImageConverter.ToBitmap(ImageRoi), r.Left, r.Top);
                        displayManager.CreateLayerDisplay(Reuslt, DEFINE.Threshold, false);
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void trbDoubleThresholdMax_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (COpenCVHelper.IsImageEmpty(displayManager.GetImageSrc())) return;

                int Min = trbDoubleThresholdMin.Value;
                int Max = trbDoubleThresholdMax.Value;

                using (Mat imageSrc = displayManager.SelectedItem != DEFINE.Threshold ? displayManager.GetImageSrc().Clone() : Lib.Common.CImageConverter.ToMat(displayManager.GetLayerImage("Main")).Clone())
                {
                    if (displayManager.IsLayerRoiEmpty(displayManager.SelectedItem))
                    {
                        Cv2.InRange(imageSrc, new Scalar(Min, Min, Min), new Scalar(Max, Max, Max), imageSrc);
                        displayManager.CreateLayerDisplay(Lib.Common.CImageConverter.ToBitmap(imageSrc), DEFINE.Threshold, false);
                    }
                    else
                    {
                        Rect r = CConverter.RectangleToRect(displayManager.GetLayerRoi(displayManager.SelectedItem));
                        Mat ImageRoi = imageSrc.SubMat(r);
                        Cv2.InRange(ImageRoi, new Scalar(Min, Min, Min), new Scalar(Max, Max, Max), ImageRoi);
                        Bitmap Reuslt = Lib.Common.CBitmapProcessing.OverlayImage(Lib.Common.CImageConverter.ToBitmap(imageSrc), Lib.Common.CImageConverter.ToBitmap(ImageRoi), r.Left, r.Top);
                        displayManager.CreateLayerDisplay(Reuslt, DEFINE.Threshold, false);
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void trbAdaptiveThreshold_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (COpenCVHelper.IsImageEmpty(displayManager.GetImageSrc())) return;

                if (tbBlockSize.Text == "") { tbBlockSize.Text = "25"; }
                if (tbWeight.Text == "") { tbWeight.Text = "5"; }

                int Block = int.Parse(tbBlockSize.Text);
                int Weight = int.Parse(tbWeight.Text);

                using (Mat imageSrc = displayManager.SelectedItem != DEFINE.Threshold ? displayManager.GetImageSrc().Clone() : Lib.Common.CImageConverter.ToMat(displayManager.GetLayerImage("Main")).Clone())
                {
                    COpenCVHelper.SetImageChannel1(imageSrc);
                    
                    if (displayManager.IsLayerRoiEmpty(displayManager.SelectedItem))
                    {
                        Cv2.AdaptiveThreshold(imageSrc, imageSrc, trbAdaptiveThreshold.Value, CUtil.ParseEnum<AdaptiveThresholdTypes>(cbAdaptiveType.SelectedItem.ToString()), CUtil.ParseEnum<ThresholdTypes>(cbAdaptiveThresholdMenu.SelectedItem.ToString()), Block, Weight);
                        displayManager.CreateLayerDisplay(Lib.Common.CImageConverter.ToBitmap(imageSrc), DEFINE.Threshold, false);
                    }
                    else
                    {
                        Rect r = CConverter.RectangleToRect(displayManager.GetLayerRoi(displayManager.SelectedItem));
                        Mat ImageRoi = imageSrc.SubMat(r);
                        Cv2.AdaptiveThreshold(ImageRoi, ImageRoi, trbAdaptiveThreshold.Value, CUtil.ParseEnum<AdaptiveThresholdTypes>(cbAdaptiveType.SelectedItem.ToString()), CUtil.ParseEnum<ThresholdTypes>(cbAdaptiveThresholdMenu.SelectedItem.ToString()), Block, Weight);                        
                        Bitmap Reuslt = Lib.Common.CBitmapProcessing.OverlayImage(Lib.Common.CImageConverter.ToBitmap(imageSrc), Lib.Common.CImageConverter.ToBitmap(ImageRoi), r.Left, r.Top);
                        displayManager.CreateLayerDisplay(Reuslt, DEFINE.Threshold, false);
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
    }
}
