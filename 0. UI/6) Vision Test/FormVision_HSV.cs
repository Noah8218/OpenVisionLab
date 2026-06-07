using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using OpenCvSharp;
using OpenVisionLab._1._Core;
using RJCodeUI_M1.RJForms;
using System.Linq;
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
            CUtil.InitDirectory("TEST");
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

        }
        private void btnNewPanel_Source_Click(object sender, EventArgs e)
        {     
            InitLayListItem();
        }     
        private void trbHsv_Scroll(object sender, EventArgs e)
        {

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat(ibSource.DisplayBitmap).Clone())
                {
                    //if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.BGR2HSV);
                    //if (ImageCVSource.Channels() != 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.BGR2HSV);

                    Bitmap Result = new Bitmap(10, 10);
                    
                    List<int> mins = new List<int>();
                    mins.Add(trbHueMin.Value);
                    mins.Add(trbSatMin.Value);
                    mins.Add(trbValMin.Value);

                    int min = mins.Min();

                    List<int> maxs = new List<int>();
                    maxs.Add(trbHueMax.Value);
                    maxs.Add(trbSatMax.Value);
                    maxs.Add(trbValMax.Value);

                    int max = maxs.Max();

                    
                    Cv2.InRange(ImageCVSource, new Scalar(100, 100 , 100), new Scalar(200 , 255, 200), ImageCVSource);

                    Mat and = new Mat();

                    Mat sourc1 = Lib.Common.CImageConverter.ToMat(ibSource.DisplayBitmap).Clone();
                    //if (sourc1.Channels() != 1) Cv2.CvtColor(sourc1, sourc1, ColorConversionCodes.BGR2GRAY);

                    Cv2.BitwiseAnd(sourc1, sourc1, and, ImageCVSource);

                    if (and.Channels() != 3) Cv2.CvtColor(and, and, ColorConversionCodes.GRAY2RGB);

                    Result = Lib.Common.CImageConverter.ToBitmap(and);
                    PublishResult(cbLayerList2, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }

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



