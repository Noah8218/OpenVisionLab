using System;
using System.Drawing;
using System.Windows.Forms;
using OpenCvSharp;
using OpenVisionLab._1._Core;
using RJCodeUI_M1.RJForms;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormVision_HSV : VisionTestForm
    {        
        private void InitLayListItem()
        {
            InitializeSingleInputLayerList(cbLayerList, cbLayerList2);
        }

        private void IbDestination_MouseClick(object sender, MouseEventArgs e)
        {
            ActivateDestinationLayer();
        }

        private void IbSource_MouseClick(object sender, MouseEventArgs e)
        {
            ActivateSourceLayer();
        }

        private void IbDestination_ImageChanged(object sender, EventArgs e)
        {
            AcceptDestinationImageChange(cbLayerList2, ibDestination);
        }

        private void IbSource_ImageChanged(object sender, EventArgs e)
        {
            AcceptSourceImageChange(cbLayerList, ibSource);
        }

        private bool InitEvent()
        {
            return RegisterEscapeClose();
        }

        private void btnNewPanel_Desty_Click(object sender, EventArgs e)
        {
            CreateSingleInputDestinationLayer(cbLayerList2, InitLayListItem);
        }

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectSourceLayer(cbLayerList, ibSource, true);
        }

        private void cbLayerList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectDestinationLayer(cbLayerList2, ibDestination);
        }
        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            if (displayManager == null) { return; }

            AppUtil.InitDirectory("TEST");
            InitializeSingleInputViewers(
                InitLayListItem,
                ibSource,
                ibDestination,
                IbSource_ImageChanged,
                IbDestination_ImageChanged,
                IbSource_MouseClick,
                IbDestination_MouseClick,
                toolTip1,
                btnNewPanel_Desty);
        }
        public FormVision_HSV(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }

        public FormVision_HSV()
        {
            InitializeComponent();
        }
        private void btnNewPanel_Source_Click(object sender, EventArgs e)
        {     
            InitLayListItem();
        }     
        private void trbHsv_Scroll(object sender, EventArgs e)
        {
            ScheduleHsvPreview();
        }

        private void ScheduleHsvPreview()
        {
            if (ibSource.DisplayBitmap == null) { return; }

            hsvPreviewTimer.Stop();
            hsvPreviewTimer.Start();
        }

        private void hsvPreviewTimer_Tick(object sender, EventArgs e)
        {
            hsvPreviewTimer.Stop();
            PublishHsvPreview();
        }

        private void PublishHsvPreview()
        {
            if (ibSource.DisplayBitmap == null || cbLayerList2.SelectedItem == null) { return; }

            try
            {
                using (Bitmap preview = CreateHsvPreviewBitmap())
                {
                    PublishPreviewBitmap(cbLayerList2, ibDestination, preview);
                }
            }
            catch
            {
                hsvPreviewTimer.Stop();
            }
        }

        private Bitmap CreateHsvPreviewBitmap()
        {
            using (Mat source = BitmapImageConverter.ToMat(ibSource.DisplayBitmap).Clone())
            using (Mat hsv = ConvertToHsv(source))
            using (Mat mask = new Mat())
            using (Mat preview = new Mat())
            {
                int hueMin = Math.Min(trbHueMin.Value, trbHueMax.Value);
                int hueMax = Math.Max(trbHueMin.Value, trbHueMax.Value);
                int satMin = Math.Min(trbSatMin.Value, trbSatMax.Value);
                int satMax = Math.Max(trbSatMin.Value, trbSatMax.Value);
                int valMin = Math.Min(trbValMin.Value, trbValMax.Value);
                int valMax = Math.Max(trbValMin.Value, trbValMax.Value);

                Cv2.InRange(
                    hsv,
                    new Scalar(hueMin, satMin, valMin),
                    new Scalar(hueMax, satMax, valMax),
                    mask);
                Cv2.BitwiseAnd(source, source, preview, mask);

                return BitmapImageConverter.ToBitmap(preview);
            }
        }

        private static Mat ConvertToHsv(Mat source)
        {
            Mat hsv = new Mat();
            if (source.Channels() == 1)
            {
                using (Mat rgb = new Mat())
                {
                    Cv2.CvtColor(source, rgb, ColorConversionCodes.GRAY2RGB);
                    Cv2.CvtColor(rgb, hsv, ColorConversionCodes.RGB2HSV);
                }

                return hsv;
            }

            if (source.Channels() == 4)
            {
                using (Mat rgb = new Mat())
                {
                    Cv2.CvtColor(source, rgb, ColorConversionCodes.RGBA2RGB);
                    Cv2.CvtColor(rgb, hsv, ColorConversionCodes.RGB2HSV);
                }

                return hsv;
            }

            Cv2.CvtColor(source, hsv, ColorConversionCodes.RGB2HSV);
            return hsv;
        }

        public Mat Rotate(Mat src, double angle)
        {
            Mat rotate = new Mat(src.Size(), MatType.CV_8UC1);
            Mat matrix = Cv2.GetRotationMatrix2D(new Point2f(src.Width / 2, src.Height / 2), angle, 1);
            Cv2.WarpAffine(src, rotate, matrix, src.Size(), InterpolationFlags.Linear);
            return rotate;
        }

    }
 }



