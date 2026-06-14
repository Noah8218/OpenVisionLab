using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using OpenCvSharp;
using OpenVisionLab._1._Core;
using RJCodeUI_M1.RJForms;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormVision_RotateAndScale : VisionTestForm
    {
        private bool suppressTransformPreview;

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
            InitializeTransformControls();
        }
        public FormVision_RotateAndScale(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }

        public FormVision_RotateAndScale()
        {
            InitializeComponent();
        }
        private void btnNewPanel_Source_Click(object sender, EventArgs e)
        {     
            InitLayListItem();
        }

        private void btnMorpRun_Click(object sender, EventArgs e)
        {
            RunRotateAndScale(true);
        }

        private void trbRotate_Scroll(object sender, EventArgs e)
        {
            if (suppressTransformPreview) { return; }

            SyncTransformTextBoxesFromTrackBars();
            ScheduleTransformPreview();
        }

        private void trbScale_Scroll(object sender, EventArgs e)
        {
            if (suppressTransformPreview) { return; }

            SyncTransformTextBoxesFromTrackBars();
            ScheduleTransformPreview();
        }

        private void InitializeTransformControls()
        {
            suppressTransformPreview = true;
            try
            {
                trbRotate.Value = ClampTrackValue(trbRotate, trbRotate.Value);
                trbScaleX.Value = ClampTrackValue(trbScaleX, trbScaleX.Value == 0 ? 100 : trbScaleX.Value);
                trbScaleY.Value = ClampTrackValue(trbScaleY, trbScaleY.Value == 0 ? 100 : trbScaleY.Value);
                SyncTransformTextBoxesFromTrackBars();
            }
            finally
            {
                suppressTransformPreview = false;
            }
        }

        private void SyncTransformTextBoxesFromTrackBars()
        {
            tbRotate.Text = trbRotate.Value.ToString(CultureInfo.InvariantCulture);
            tbScaleX.Text = trbScaleX.Value.ToString(CultureInfo.InvariantCulture);
            tbScaleY.Text = trbScaleY.Value.ToString(CultureInfo.InvariantCulture);
        }

        private void SyncTrackBarsFromTextBoxes()
        {
            suppressTransformPreview = true;
            try
            {
                trbRotate.Value = ClampTrackValue(trbRotate, (int)Math.Round(ReadNumber(tbRotate.Text, trbRotate.Value)));
                trbScaleX.Value = ClampTrackValue(trbScaleX, (int)Math.Round(ReadNumber(tbScaleX.Text, trbScaleX.Value)));
                trbScaleY.Value = ClampTrackValue(trbScaleY, (int)Math.Round(ReadNumber(tbScaleY.Text, trbScaleY.Value)));
                SyncTransformTextBoxesFromTrackBars();
            }
            finally
            {
                suppressTransformPreview = false;
            }
        }

        private void RunRotateAndScale(bool writeLifecycleLog)
        {
            transformPreviewTimer.Stop();
            SyncTrackBarsFromTextBoxes();

            RunVisionStep(writeLifecycleLog ? "Rotate / Scale" : "Rotate / Scale Preview", () =>
            {
                if (ibSource.DisplayBitmap == null)
                {
                    throw new InvalidOperationException("Input image is empty.");
                }

                Stopwatch stopwatch = Stopwatch.StartNew();
                double angle = ReadNumber(tbRotate.Text, trbRotate.Value);
                double scaleX = ReadScalePercent(tbScaleX.Text, trbScaleX.Value) / 100d;
                double scaleY = ReadScalePercent(tbScaleY.Text, trbScaleY.Value) / 100d;

                using (Mat source = BitmapImageConverter.ToMat(ibSource.DisplayBitmap).Clone())
                using (Mat transformed = RotateAndScale(source, angle, scaleX, scaleY))
                {
                    Bitmap result = BitmapImageConverter.ToBitmap(transformed);
                    PublishResult(cbLayerList2, ibDestination, result, FormatElapsed(stopwatch));
                }
            }, writeLifecycleLog);
        }

        private void ScheduleTransformPreview()
        {
            if (ibSource.DisplayBitmap == null) { return; }

            transformPreviewTimer.Stop();
            transformPreviewTimer.Start();
        }

        private void transformPreviewTimer_Tick(object sender, EventArgs e)
        {
            transformPreviewTimer.Stop();
            PublishRotateAndScalePreview();
        }

        private void PublishRotateAndScalePreview()
        {
            if (ibSource.DisplayBitmap == null || cbLayerList2.SelectedItem == null) { return; }

            try
            {
                SyncTrackBarsFromTextBoxes();
                double angle = ReadNumber(tbRotate.Text, trbRotate.Value);
                double scaleX = ReadScalePercent(tbScaleX.Text, trbScaleX.Value) / 100d;
                double scaleY = ReadScalePercent(tbScaleY.Text, trbScaleY.Value) / 100d;

                using (Mat source = BitmapImageConverter.ToMat(ibSource.DisplayBitmap).Clone())
                using (Mat transformed = RotateAndScale(source, angle, scaleX, scaleY))
                using (Bitmap result = BitmapImageConverter.ToBitmap(transformed))
                {
                    PublishPreviewBitmap(cbLayerList2, ibDestination, result);
                }
            }
            catch
            {
                transformPreviewTimer.Stop();
            }
        }

        public Mat Rotate(Mat src, double angle)
        {
            return RotateAndScale(src, angle, 1d, 1d);
        }

        public Mat RotateAndScale(Mat src, double angle, double scaleX, double scaleY)
        {
            if (src == null || src.Empty())
            {
                throw new ArgumentException("Source image is empty.", nameof(src));
            }

            scaleX = Math.Max(0.01d, scaleX);
            scaleY = Math.Max(0.01d, scaleY);
            int width = Math.Max(1, (int)Math.Round(src.Width * scaleX));
            int height = Math.Max(1, (int)Math.Round(src.Height * scaleY));

            Mat scaled = new Mat();
            if (width != src.Width || height != src.Height)
            {
                Cv2.Resize(src, scaled, new OpenCvSharp.Size(width, height), 0d, 0d, InterpolationFlags.Linear);
            }
            else
            {
                scaled = src.Clone();
            }

            if (Math.Abs(angle) < 0.0001d)
            {
                return scaled;
            }

            Mat rotated = new Mat(scaled.Size(), scaled.Type());
            using (Mat matrix = Cv2.GetRotationMatrix2D(new Point2f(scaled.Width / 2f, scaled.Height / 2f), angle, 1d))
            {
                Cv2.WarpAffine(scaled, rotated, matrix, scaled.Size(), InterpolationFlags.Linear, BorderTypes.Constant);
            }

            scaled.Dispose();
            return rotated;
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            RunRotateAndScale(true);
        }

        private static int ClampTrackValue(TrackBar trackBar, int value)
        {
            return Math.Min(trackBar.Maximum, Math.Max(trackBar.Minimum, value));
        }

        private static double ReadNumber(string text, double fallback)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                ? value
                : fallback;
        }

        private static double ReadScalePercent(string text, double fallback)
        {
            return Math.Min(300d, Math.Max(10d, ReadNumber(text, fallback)));
        }
    }
 }



