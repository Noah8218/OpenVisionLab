using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Diagnostics;


//IF 전용 Library
using OpenCvSharp;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;

using MetroFramework;
using MetroFramework.Forms;
using ADOX;
using Keys = System.Windows.Forms.Keys;
using System.Windows.Media;
using Automation.BDaq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using KtemVisionSystem._1._Core;

namespace KtemVisionSystem
{
    public partial class FormVision_EdgeDection : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;

        private List<FormLayerDisplay> Displays { get; set; } = new List<FormLayerDisplay>();
        private int PanelCount = 0;

        private KtemViewer Source = new KtemViewer();
        private KtemViewer Destination = new KtemViewer();

        #region Event Register        
        public EventHandler<DockDisplayEventArgs> EventUpdateDisplay;
        #endregion

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
            this.Displays = Displays;
            this.EventUpdateDisplay = EventUpdateDisplay;
            PanelCount = Displays.Count;
            timer1.Enabled = true;
            timer1.Start();
        }

        private void InitLayListItem()
        {
            cbLayerList.Items.Clear();
            for (int i = 0; i < Displays.Count; i++) { cbLayerList.Items.Add(Displays[i].Text); }
            cbLayerList.SelectedIndex = Source_Index;
            cbLayerList2.Items.Clear();
            for (int i = 0; i < Displays.Count; i++) { cbLayerList2.Items.Add(Displays[i].Text); }
            cbLayerList2.SelectedIndex = Destination_Index;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            InitFilterMenu();
            InitEvent();
            InitLayListItem();
            Source.LoadImageBox(ibSource, false);
            Destination.LoadImageBox(ibDestination, false);

            ibSource.Image = (Bitmap)Displays[DEFINE.Main].ImageView.ib.Image;
            ibDestination.Image = (Bitmap)Displays[DEFINE.Main].ImageView.ib.Image;
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
            Displays[Destination_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbSource_MouseClick(object sender, MouseEventArgs e)
        {
            Displays[Source_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbDestination_ImageChanged(object sender, EventArgs e)
        {
            Destination_Index = cbLayerList2.SelectedIndex;
            Displays[Destination_Index].ibSource.Image = (Bitmap)ibDestination.Image;
        }

        private void IbSource_ImageChanged(object sender, EventArgs e)
        {
            Source_Index = cbLayerList.SelectedIndex;
            Displays[Source_Index].ibSource.Image = (Bitmap)ibSource.Image;
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

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        private void btnNewPanel_Desty_Click(object sender, EventArgs e)
        {
            CDisplayManager.CreatePanel();
            InitLayListItem();
            Destination_Index = cbLayerList2.Items.Count - 1;
            cbLayerList2.SelectedIndex = Destination_Index;
        }

        private int GetDisplayIndex(string strTitle)
        {
            for (int i = 0; i < Displays.Count; i++)
            {
                if (Displays[i].Text == strTitle) { return i; }
            }

            return 0;
        }

        private int Source_Index = 0;
        private int Destination_Index = 0;

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Source_Index = cbLayerList.SelectedIndex;
            ibSource.Image = (Bitmap)Displays[Source_Index].ibSource.Image;
        }

        private void cbLayerList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Destination_Index = cbLayerList2.SelectedIndex;
            ibDestination.Image = (Bitmap)Displays[Destination_Index].ibSource.Image;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PanelCount != Displays.Count)
                {
                    PanelCount = Displays.Count;
                    InitLayListItem();
                }

                if (Source.ROI != Displays[Source_Index].ImageView.ROI || Source.TrainROI != Displays[Source_Index].ImageView.TrainROI)
                {
                    ibSource.Invalidate();
                }
                Source.ROI = Displays[Source_Index].ImageView.ROI;
                Source.TrainROI = Displays[Source_Index].ImageView.TrainROI;

                if (Destination.ROI != Displays[Destination_Index].ImageView.ROI || Destination.TrainROI != Displays[Destination_Index].ImageView.TrainROI)
                {
                    ibDestination.Invalidate();
                }
                Destination.ROI = Displays[Destination_Index].ImageView.ROI;
                Destination.TrainROI = Displays[Destination_Index].ImageView.TrainROI;

                cbLayerList_SelectedIndexChanged(null, null);
                cbLayerList2_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnFilterRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = CConverter.ToMat((Bitmap)ibSource.Image).Clone())
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

                    if (Displays[Source_Index].ImageView.ROI.IsEmpty)
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
                        Result = CConverter.ToBitmap(ImageCVSource);
                    }
                    else
                    {
                        Rect r = CConverter.RectangleToRect(Displays[Source_Index].ImageView.ROI);
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

                        Result = CConverter.OverlayImage(CConverter.ToBitmap(ImageCVSource), CConverter.ToBitmap(ImageRoi), r.Left, r.Top);
                    }

                    Displays[GetDisplayIndex(cbLayerList2.SelectedItem.ToString())].ImageView.ib.Image = Result;
                    ibDestination.Image = Result;
                    EventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowMessageBox("ALARM", $"{ex}");
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

