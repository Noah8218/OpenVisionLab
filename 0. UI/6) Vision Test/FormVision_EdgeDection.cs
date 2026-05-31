using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using OpenCvSharp;
using Keys = System.Windows.Forms.Keys;
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

        public FormVision_EdgeDection(List<FormLayerDisplay> Displays, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            this.displays = Displays;
            this.eventUpdateDisplay = EventUpdateDisplay;
            panelCount = Displays.Count;
            timer1.Enabled = true;
            timer1.Start();
        }

        private void InitLayListItem()
        {
            cbLayerList.Items.Clear();
            for (int i = 0; i < displays.Count; i++) { cbLayerList.Items.Add(displays[i].Text); }
            cbLayerList.SelectedIndex = source1_Index;
            cbLayerList2.Items.Clear();
            for (int i = 0; i < displays.Count; i++) { cbLayerList2.Items.Add(displays[i].Text); }
            cbLayerList2.SelectedIndex = destination_Index;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            InitFilterMenu();
            InitEvent();
            InitLayListItem();
            source_1.LoadImageBox(ibSource, false);
            destination.LoadImageBox(ibDestination, false);

            ibSource.Image = (Bitmap)displays[DEFINE.Main].viewer._Ib.Image;
            ibDestination.Image = (Bitmap)displays[DEFINE.Main].viewer._Ib.Image;
            ibSource.ImageChanged += IbSource_ImageChanged;
            ibDestination.ImageChanged += IbDestination_ImageChanged;
            ibDestination.MouseClick += IbDestination_MouseClick;
            ibSource.MouseClick += IbSource_MouseClick;
            ibSource.ZoomToFit();
            ibDestination.ZoomToFit();

            //toolTip1.SetToolTip(btnNewPanel_Source, "Create New Layer");            

            toolTip1.SetToolTip(rjLabel4, "픽셀이 상위 임곗값보다 큰 기울기를 가지면 픽셀을 가장자리로 간주합니다.");
            toolTip1.SetToolTip(rjLabel2, "픽셀 값이 하위 임곗값보다 낮은 경우 가장자리로 고려하지 않습니다.");
        }

        public void InitFilterMenu()
        {
            cbEdgeType.Items.Add(EdgeDetector.Canny);
            cbEdgeType.Items.Add(EdgeDetector.Sobel);
            cbEdgeType.Items.Add(EdgeDetector.Scharr);
            cbEdgeType.Items.Add(EdgeDetector.Laplacian);
            cbEdgeType.SelectedIndex = 0;
        }

        private void IbDestination_MouseClick(object sender, MouseEventArgs e)
        {
            displays[destination_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbSource_MouseClick(object sender, MouseEventArgs e)
        {
            displays[source1_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbDestination_ImageChanged(object sender, EventArgs e)
        {
            destination_Index = cbLayerList2.SelectedIndex;
            displays[destination_Index].ibSource.Image = (Bitmap)ibDestination.Image;
        }

        private void IbSource_ImageChanged(object sender, EventArgs e)
        {
            source1_Index = cbLayerList.SelectedIndex;
            displays[source1_Index].ibSource.Image = (Bitmap)ibSource.Image;
        }
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private bool InitEvent()
        {
            try
            {
                this.KeyPreview = true;
                this.KeyDown += Form_KeyDown;

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");                
                return false;
            }

            return true;
        }

        private void btnNewPanel_Desty_Click(object sender, EventArgs e)
        {
            CDisplayManager.CreatePanel();
            InitLayListItem();
            destination_Index = cbLayerList2.Items.Count - 1;
            cbLayerList2.SelectedIndex = destination_Index;
        }

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            source1_Index = cbLayerList.SelectedIndex;
            ibSource.Image = (Bitmap)displays[source1_Index].ibSource.Image;
        }

        private void cbLayerList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            destination_Index = cbLayerList2.SelectedIndex;
            ibDestination.Image = (Bitmap)displays[destination_Index].ibSource.Image;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (panelCount != displays.Count)
                {
                    panelCount = displays.Count;
                    InitLayListItem();
                }

                if (source_1.Roi != displays[source1_Index].viewer.Roi || source_1.TrainROI != displays[source1_Index].viewer.TrainROI)
                {
                    ibSource.Invalidate();
                }
                source_1.Roi = displays[source1_Index].viewer.Roi;
                source_1.TrainROI = displays[source1_Index].viewer.TrainROI;

                if (destination.Roi != displays[destination_Index].viewer.Roi || destination.TrainROI != displays[destination_Index].viewer.TrainROI)
                {
                    ibDestination.Invalidate();
                }
                destination.Roi = displays[destination_Index].viewer.Roi;
                destination.TrainROI = displays[destination_Index].viewer.TrainROI;

                cbLayerList_SelectedIndexChanged(null, null);
                cbLayerList2_SelectedIndexChanged(null, null);
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnFilterRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource.Image).Clone())
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

                    if (displays[source1_Index].viewer.Roi.IsEmpty)
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
                        Rect r = CConverter.RectangleToRect(displays[source1_Index].viewer.Roi);
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

                    displays[GetDisplayIndex(cbLayerList2.SelectedItem.ToString())].viewer._Ib.Image = Result;
                    ibDestination.Image = Result;
                    eventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
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

