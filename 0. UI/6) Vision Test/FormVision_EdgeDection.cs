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
    public partial class FormVision_EdgeDection : VisionTestForm
    {        
        private enum EdgeDetector
        {
            Canny,
            Sobel,
            Scharr,
            Laplacian
        }
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
            foreach (EdgeDetector edgeDetector in Enum.GetValues(typeof(EdgeDetector)))
            {
                cbEdgeType.Items.Add(edgeDetector);
            }
            cbEdgeType.SelectedIndex = 0;
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

        public FormVision_EdgeDection(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }
        private void btnFilterRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat(ibSource.DisplayBitmap).Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    if (tbThresholdHight.Text == "") { tbThresholdHight.Text = "200"; }
                    if (tbThresholdLow.Text == "") { tbThresholdLow.Text = "100"; }
                    if (tbSobelMask.Text == "") { tbSobelMask.Text = "3"; }

                    int High = int.Parse(tbThresholdHight.Text);
                    int Low = int.Parse(tbThresholdLow.Text);
                    int Sobel = int.Parse(tbSobelMask.Text);

                    int SobelDegreeX = int.Parse(nudDegreeX.Value.ToString());
                    int SobelDegreeY = int.Parse(nudDegreeY.Value.ToString());
                    int Kernel = int.Parse(nudKernel.Value.ToString());

                    int ScharrDegreeX = int.Parse(nudScharrDegreeX.Value.ToString());
                    int ScharrDegreeY = int.Parse(nudScharrDegreeY.Value.ToString());

                    int LaplacianKernel = int.Parse(nudLaplacianKernel.Value.ToString());

                    Bitmap Result = new Bitmap(10, 10);

                    if (displayManager.IsLayerRoiEmpty(source1_Index))
                    {
                        switch (CUtil.ParseEnum<EdgeDetector>(cbEdgeType.SelectedItem.ToString()))
                        {
                            case EdgeDetector.Canny:
                                Cv2.Canny(ImageCVSource, ImageCVSource, Low, High, Sobel, chkUseL2.Check);                               
                                break;
                            case EdgeDetector.Sobel:
                                Cv2.Sobel(ImageCVSource, ImageCVSource, MatType.CV_8U, SobelDegreeX , SobelDegreeY, Kernel, 1, 0, BorderTypes.Default);
                                break;
                            case EdgeDetector.Scharr:
                                Cv2.Scharr(ImageCVSource, ImageCVSource, MatType.CV_8U, ScharrDegreeX, ScharrDegreeY, 1, 0, BorderTypes.Default);
                                break;
                            case EdgeDetector.Laplacian:
                                Cv2.Laplacian(ImageCVSource, ImageCVSource, MatType.CV_8U, LaplacianKernel, 1, 0, BorderTypes.Default);
                                break;
                        }
                        //ImageCVSource.ConvertTo(ImageCVSource, MatType.CV_8UC1);
                        Result = Lib.Common.CImageConverter.ToBitmap(ImageCVSource);
                    }
                    else
                    {
                        Rect r = CConverter.RectangleToRect(GetLayerRoi(source1_Index));
                        Mat ImageRoi = ImageCVSource.SubMat(r);

                        switch (CUtil.ParseEnum<EdgeDetector>(cbEdgeType.SelectedItem.ToString()))
                        {
                            case EdgeDetector.Canny:
                                Cv2.Canny(ImageRoi, ImageRoi, Low, High, Sobel, chkUseL2.Check);
                                break;
                            case EdgeDetector.Sobel:
                                Cv2.Sobel(ImageRoi, ImageRoi, MatType.CV_8U, SobelDegreeX, SobelDegreeY, Kernel, 1, 0, BorderTypes.Default);
                                break;
                            case EdgeDetector.Scharr:
                                Cv2.Scharr(ImageRoi, ImageRoi, MatType.CV_8U, ScharrDegreeX, ScharrDegreeY, 1, 0, BorderTypes.Default);
                                break;
                            case EdgeDetector.Laplacian:
                                Cv2.Laplacian(ImageRoi, ImageRoi, MatType.CV_8U, LaplacianKernel, 1, 0, BorderTypes.Default);
                                break;
                        }

                        Result = Lib.Common.CBitmapProcessing.OverlayImage(Lib.Common.CImageConverter.ToBitmap(ImageCVSource), Lib.Common.CImageConverter.ToBitmap(ImageRoi), r.Left, r.Top);
                    }
                    PublishResult(cbLayerList2, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowMessageBox("ALARM", $"{Desc}");
            }
        }

        private void cbFilterType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!(sender is RJCodeUI_M1.RJControls.RJComboBox)) return;
            string Index = (sender as System.Windows.Forms.ComboBox).Text;

            //tbKernalFilter.Enabled = false;
            //tbFilterW.Enabled = false;
            //tbFilterH.Enabled = false;
            //tbDiameter.Enabled = false;
            //tbSigmaColor.Enabled = false;
            //tbSigmaSpace.Enabled = false;

            switch (CUtil.ParseEnum<EdgeDetector>(Index))
            {
                case EdgeDetector.Canny:
                    Tab.SelectedIndex = 0;
                    break;
                case EdgeDetector.Sobel:
                    Tab.SelectedIndex = 1;
                    break;
                case EdgeDetector.Scharr:
                    Tab.SelectedIndex = 2;
                    break;
                case EdgeDetector.Laplacian:
                    Tab.SelectedIndex = 3;
                    break;
            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}



