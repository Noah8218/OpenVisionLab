using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using OpenCvSharp;
using OpenVisionLab._1._Core;
using RJCodeUI_M1.RJForms;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormVision_RotateAndScale : VisionTestForm
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
        public FormVision_RotateAndScale(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }

        public FormVision_RotateAndScale()
        {

        }
        private void btnNewPanel_Source_Click(object sender, EventArgs e)
        {     
            InitLayListItem();
        }

        private void btnMorpRun_Click(object sender, EventArgs e)
        {
                        
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (Mat ImageCVSource = Lib.Common.BitmapImageConverter.ToMat(ibSource.DisplayBitmap).Clone())
            {
                if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);
               
                Bitmap Result = new Bitmap(10, 10);
                //Cv2.MorphologyEx(ImageCVSource, ImageCVSource, AppUtil.ParseEnum<MorphTypes>(Operator), Kernel, new OpenCvSharp.Point(-1, -1), 1);
                //Result = CommonConverter.ToBitmap(ImageCVSource);
                PublishResult(cbLayerList2, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
            }
        
        }
        private void trbRotate_Scroll(object sender, EventArgs e)
        {

                        Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (Mat ImageCVSource = Lib.Common.BitmapImageConverter.ToMat(ibSource.DisplayBitmap).Clone())
            {
                if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                Bitmap Result = new Bitmap(10, 10);
                Mat ImageRotate = Rotate(ImageCVSource, trbRotate.Value);
                //Cv2.MorphologyEx(ImageCVSource, ImageCVSource, AppUtil.ParseEnum<MorphTypes>(Operator), Kernel, new OpenCvSharp.Point(-1, -1), 1);
                Result = Lib.Common.BitmapImageConverter.ToBitmap(ImageRotate);
                PublishResult(cbLayerList2, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
            }
        

        }

        public Mat Rotate(Mat src, double angle)
        {
            Mat rotate = new Mat(src.Size(), MatType.CV_8UC1);
            Mat matrix = Cv2.GetRotationMatrix2D(new Point2f(src.Width / 2, src.Height / 2), angle, 1);
            Cv2.WarpAffine(src, rotate, matrix, src.Size(), InterpolationFlags.Linear);
            return rotate;
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {

                        Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (Mat ImageCVSource = Lib.Common.BitmapImageConverter.ToMat(ibSource.DisplayBitmap).Clone())
            {
                if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                Bitmap Result = new Bitmap(10, 10);
                Mat ImageRotate = Rotate(ImageCVSource, double.Parse(tbRotate.Text));
                //Cv2.MorphologyEx(ImageCVSource, ImageCVSource, AppUtil.ParseEnum<MorphTypes>(Operator), Kernel, new OpenCvSharp.Point(-1, -1), 1);
                Result = Lib.Common.BitmapImageConverter.ToBitmap(ImageRotate);
                PublishResult(cbLayerList2, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
            }
        
        }
    }
 }



