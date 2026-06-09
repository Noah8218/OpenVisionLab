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

        private Bitmap CreateLayerOperationResult(Action<Mat> processImage, Action<Mat> prepareSource = null, bool convertFullImageToGray = false)
        {
            using (Mat imageSrc = displayManager.SelectedItem != DEFINE.Threshold
                ? displayManager.GetImageSrc().Clone()
                : Lib.Common.BitmapImageConverter.ToMat(displayManager.GetLayerImage("Main")).Clone())
            {
                prepareSource?.Invoke(imageSrc);

                if (displayManager.IsLayerRoiEmpty(displayManager.SelectedItem))
                {
                    if (convertFullImageToGray && imageSrc.Channels() != 1)
                    {
                        Cv2.CvtColor(imageSrc, imageSrc, ColorConversionCodes.RGB2GRAY);
                    }

                    processImage(imageSrc);
                    return Lib.Common.BitmapImageConverter.ToBitmap(imageSrc);
                }

                Rect roi = CommonConverter.RectangleToRect(displayManager.GetLayerRoi(displayManager.SelectedItem));
                using (Mat imageRoi = imageSrc.SubMat(roi))
                {
                    processImage(imageRoi);
                    using (Bitmap sourceBitmap = Lib.Common.BitmapImageConverter.ToBitmap(imageSrc))
                    using (Bitmap roiBitmap = Lib.Common.BitmapImageConverter.ToBitmap(imageRoi))
                    {
                        return Lib.Common.BitmapProcessing.OverlayImage(sourceBitmap, roiBitmap, roi.Left, roi.Top);
                    }
                }
            }
        }

        private void trbThreshold_Scroll(object sender, EventArgs e)
        {
                        if (OpenCvHelper.IsImageEmpty(displayManager.GetImageSrc())) return;

            Bitmap result = CreateLayerOperationResult(image =>
            {
                Cv2.Threshold(image, image, trbThreshold.Value, 255, AppUtil.ParseEnum<ThresholdTypes>(cbThresholdMenu.SelectedItem.ToString()));
            }, convertFullImageToGray: true);
            displayManager.CreateLayerDisplay(result, DEFINE.Threshold, false);
        
        }

        private void trbDoubleThresholdMax_Scroll(object sender, EventArgs e)
        {
                        if (OpenCvHelper.IsImageEmpty(displayManager.GetImageSrc())) return;

            int Min = trbDoubleThresholdMin.Value;
            int Max = trbDoubleThresholdMax.Value;

            Bitmap result = CreateLayerOperationResult(image =>
            {
                Cv2.InRange(image, new Scalar(Min, Min, Min), new Scalar(Max, Max, Max), image);
            });
            displayManager.CreateLayerDisplay(result, DEFINE.Threshold, false);
        
        }

        private void trbAdaptiveThreshold_Scroll(object sender, EventArgs e)
        {
                        if (OpenCvHelper.IsImageEmpty(displayManager.GetImageSrc())) return;

            if (tbBlockSize.Text == "") { tbBlockSize.Text = "25"; }
            if (tbWeight.Text == "") { tbWeight.Text = "5"; }

            int Block = int.Parse(tbBlockSize.Text);
            int Weight = int.Parse(tbWeight.Text);

            Bitmap result = CreateLayerOperationResult(image =>
            {
                Cv2.AdaptiveThreshold(image, image, trbAdaptiveThreshold.Value, AppUtil.ParseEnum<AdaptiveThresholdTypes>(cbAdaptiveType.SelectedItem.ToString()), AppUtil.ParseEnum<ThresholdTypes>(cbAdaptiveThresholdMenu.SelectedItem.ToString()), Block, Weight);
            }, image => OpenCvHelper.SetImageChannel1(image));
            displayManager.CreateLayerDisplay(result, DEFINE.Threshold, false);
        
        }
    }
}
