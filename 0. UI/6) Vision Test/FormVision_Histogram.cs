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
using Cyotek.Windows.Forms;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormVision_Histogram : VisionTestForm
    {
        private enum HistogramType
        {
            clahe,
            equalizeHist,
            Normalize,
        }

        public FormVision_Histogram(List<FormLayerDisplay> Displays, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
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
            InitMenu();
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
        }

        public void InitMenu()
        {
            cbType.Items.Add("clahe");
            cbType.Items.Add("equalizeHist");
            cbType.Items.Add("Normalize");

            cbType.SelectedIndex = 0;
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
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
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
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
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

                    if (tbClipLimit.Text == "") { tbClipLimit.Text = "3"; }
                    if (tbTilesGridSize.Text == "") { tbTilesGridSize.Text = "3"; }
                    if (tbAlpha.Text == "") { tbAlpha.Text = "0"; }
                    if (tbBeta.Text == "") { tbBeta.Text = "100"; }

                    double ClipLimit = double.Parse(tbClipLimit.Text);
                    int TilesGridSize = int.Parse(tbTilesGridSize.Text);
                    int alpha = int.Parse(tbAlpha.Text);
                    int beta = int.Parse(tbBeta.Text);

                    Bitmap Result = new Bitmap(10, 10);

                    if (displays[source1_Index].viewer.Roi.IsEmpty)
                    {
                        switch (CUtil.ParseEnum<HistogramType>(cbType.SelectedItem.ToString()))
                        {
                            case HistogramType.clahe:
                                using (CLAHE clahe = Cv2.CreateCLAHE())
                                {
                                    clahe.ClipLimit = ClipLimit;
                                    clahe.TilesGridSize = new OpenCvSharp.Size(TilesGridSize, TilesGridSize);
                                    clahe.Apply(ImageCVSource, ImageCVSource);
                                }                                
                                break;
                            case HistogramType.equalizeHist:
                                Cv2.EqualizeHist(ImageCVSource, ImageCVSource);
                                break;
                            case HistogramType.Normalize:
                                Cv2.Normalize(ImageCVSource, ImageCVSource, alpha, beta, NormTypes.MinMax);
                                //Cv2.Normalize(ImageCVSource, ImageCVSource);
                                break;                                
                        }


                        Result = Lib.Common.CImageConverter.ToBitmap(ImageCVSource);
                    }
                    else
                    {
                        Rect r = CConverter.RectangleToRect(displays[source1_Index].viewer.Roi);
                        Mat ImageRoi = ImageCVSource.SubMat(r);

                        switch (CUtil.ParseEnum<HistogramType>(cbType.SelectedItem.ToString()))
                        {
                            case HistogramType.clahe:
                                using (CLAHE clahe = Cv2.CreateCLAHE())
                                {
                                    clahe.ClipLimit = ClipLimit;
                                    clahe.TilesGridSize = new OpenCvSharp.Size(TilesGridSize, TilesGridSize);
                                    clahe.Apply(ImageRoi, ImageRoi);
                                }
                                break;
                            case HistogramType.equalizeHist:
                                Cv2.EqualizeHist(ImageRoi, ImageRoi);
                                break;
                            case HistogramType.Normalize:
                                Cv2.Normalize(ImageRoi, ImageRoi, alpha, beta);
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
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowMessageBox("ALARM", $"{Desc}");
            }
        }

        private void cbFilterType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!(sender is RJCodeUI_M1.RJControls.RJComboBox)) return;
            string Index = (sender as System.Windows.Forms.ComboBox).Text;

            //tbFilterW.Enabled = t;
            //tbFilterH.Enabled = false;
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}

