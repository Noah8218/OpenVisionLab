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
using Lib.OpenCV;
using Lib.Common;
using Lib.OpenCV.Tool;

namespace OpenVisionLab
{
    public partial class FormVision_Mean : VisionTestForm
    {        
        private CPropertyMean cPropertyMean = new CPropertyMean();

        public FormVision_Mean(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
            panelCount = this.displayManager.LayerCount;
            timer1.Enabled = true;
            timer1.Start();
        }

        private void InitLayListItem()
        {
            cbLayerList.Items.Clear();
            for (int i = 0; i < displayManager.LayerCount; i++) { cbLayerList.Items.Add(displayManager.GetLayerTitle(i)); }
            cbLayerList.SelectedIndex = source1_Index;
            cbLayerList2.Items.Clear();
            for (int i = 0; i < displayManager.LayerCount; i++) { cbLayerList2.Items.Add(displayManager.GetLayerTitle(i)); }
            cbLayerList2.SelectedIndex = destination_Index;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {            
            InitEvent();
            InitLayListItem();
            source_1.LoadImageBox(ibSource, false);
            destination.LoadImageBox(ibDestination, false);

            ibSource.Image = GetLayerImage(DEFINE.Main);
            ibDestination.Image = GetLayerImage(DEFINE.Main);
            ibSource.ImageChanged += IbSource_ImageChanged;
            ibDestination.ImageChanged += IbDestination_ImageChanged;
            ibDestination.MouseClick += IbDestination_MouseClick;
            ibSource.MouseClick += IbSource_MouseClick;
            ibSource.ZoomToFit();
            ibDestination.ZoomToFit();            
            cPropertyMean = cPropertyMean.LoadTestConfig(Application.StartupPath + "\\TEST\\" + cPropertyMean.NAME + ".xml");            
            wpg.SelectedObject = cPropertyMean;
            pnParameter.Controls.Add(host);            
        }

        private void IbDestination_MouseClick(object sender, MouseEventArgs e)
        {
            displayManager.ActivateLayer(destination_Index);
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbSource_MouseClick(object sender, MouseEventArgs e)
        {
            displayManager.ActivateLayer(source1_Index);
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbDestination_ImageChanged(object sender, EventArgs e)
        {            
            destination_Index = cbLayerList2.SelectedIndex;
            SetLayerImage(destination_Index, (Bitmap)ibDestination.Image);
        }

        private void IbSource_ImageChanged(object sender, EventArgs e)
        {            
            source1_Index = cbLayerList.SelectedIndex;
            SetLayerImage(source1_Index, (Bitmap)ibSource.Image);
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
            displayManager.CreatePanel();
            InitLayListItem();
            destination_Index = cbLayerList2.Items.Count - 1;
            cbLayerList2.SelectedIndex = destination_Index;
        }

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            source1_Index = cbLayerList.SelectedIndex;
            ibSource.Image = GetLayerImage(source1_Index);
        }

        private void cbLayerList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            destination_Index = cbLayerList2.SelectedIndex;
            ibDestination.Image = GetLayerImage(destination_Index);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (panelCount != displayManager.LayerCount)
                {
                    panelCount = displayManager.LayerCount;
                    InitLayListItem();
                }

                RefreshViewerRoi(source_1, ibSource, source1_Index);

                RefreshViewerRoi(destination, ibDestination, destination_Index);

                cbLayerList_SelectedIndexChanged(null, null);
                cbLayerList2_SelectedIndexChanged(null, null);
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource.Image).Clone())
                {
                    Bitmap Result = CDrawBitmap.GetBitmapFormat24bppRgb((Bitmap)ibSource.Image);
                    COpenCVHelper.SetImageChannel1(ImageCVSource);

                    CVMean CvMean = new CVMean();
                    CvMean.SetProperty(cPropertyMean);                    
                    CvMean.SetSourceImage(ImageCVSource);
                    CvMean.Run();

                    Graphics g = Graphics.FromImage(Result);

                    foreach (var item in CvMean.results)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                        g.DrawString($"Mean : {item.meanValue}", new Font("Arial", 3, FontStyle.Regular), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X - 20, (int)item.Center.Y - 20);
                    }

                    SetLayerImage(GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), (Bitmap)Result.Clone());
                    ibDestination.Image = Result;
                    eventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }
                wpg.SelectedObject = cPropertyMean;                
                cPropertyMean.SaveTestConfig(Application.StartupPath + "\\TEST\\" + cPropertyMean.NAME + ".xml");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowMessageBox("ALARM", $"{Desc}");
            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}

