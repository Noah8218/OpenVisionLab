using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using Keys = System.Windows.Forms.Keys;
using OpenCvSharp;
using System.Diagnostics;
using OpenVisionLab._1._Core;
using RJCodeUI_M1.RJForms;
using Lib.Common;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Lib.OpenCV;
using Lib.OpenCV.Blob;

namespace OpenVisionLab
{
    public partial class FormVision_Blob : VisionTestForm
    {
        private CPropertyBlob Property_Blob = new CPropertyBlob("Blob");

        public FormVision_Blob(List<FormLayerDisplay> Displays, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            this.displays = Displays;
            this.eventUpdateDisplay = EventUpdateDisplay;
            panelCount = Displays.Count;
            timer1.Enabled = true;
            timer1.Start();
        }

        public FormVision_Blob() { }

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
            CUtil.InitDirectory("TEST");
            string strPath = Application.StartupPath + "\\TEST\\" + Property_Blob.NAME + ".xml";
            Property_Blob = Property_Blob.LoadTestConfig(strPath);
            wpg.SelectedObject = Property_Blob;
            pnParameter.Controls.Add(host);
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
            toolTip1.SetToolTip(btnNewPanel_Desty, "Create New Layer");
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

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
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

        public void ToLowQuality(Graphics graphics)
        {
            graphics.InterpolationMode = InterpolationMode.Low;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        }

        private List<CResultBlob> cResultBlobs = new List<CResultBlob>();
        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource.Image).Clone())
                {
                    Bitmap Result = CDrawBitmap.GetBitmapFormat24bppRgb((Bitmap)ibSource.Image);
                    COpenCVHelper.SetImageChannel1(ImageCVSource);
                    Stopwatch stopwatch1 = Stopwatch.StartNew();

                    CVBlob CvBlob = new CVBlob();
                    CvBlob.SetProperty(Property_Blob);
                    CvBlob.SetSourceImage(ImageCVSource);
                    CvBlob.Run();

                    Graphics g = Graphics.FromImage(Result);

                    foreach (var item in CvBlob.results)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                        //  g.DrawString((Count + 1).ToString(), new Font("Arial", 1, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10);
                        // g.DrawString(item.Area.ToString(), new Font("Arial", 1, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X, (int)item.Center.Y);
                    }

                    if (CvBlob.results.Count == 0)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(CvBlob.property.CvROI));
                        g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)CvBlob.property.CvROI.X - 20, (int)CvBlob.property.CvROI.Y - 20);
                    }

                    cResultBlobs = CvBlob.results;
                    stopwatch1.Stop();
                    CLOG.NORMAL( $"[Time] Blob {stopwatch1.ElapsedMilliseconds} ");

                    displays[GetDisplayIndex(cbLayerList2.SelectedItem.ToString())].viewer._Ib.Image = Result;
                    ibDestination.Image = Result;
                    eventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }

                wpg.SelectedObject = Property_Blob;

                CUtil.InitDirectory("TEST");
                Property_Blob.SaveTestConfig(Application.StartupPath + "\\TEST\\" + Property_Blob.NAME + ".xml");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            source1_Index = cbLayerList.SelectedIndex;
            source_1.Roi = displays[source1_Index].viewer.Roi;
            ibSource.Image = (Bitmap)displays[source1_Index].ibSource.Image;

            CDisplayManager.ImageSrc = Lib.Common.CImageConverter.ToMat((Bitmap)displays[source1_Index].ibSource.Image);
        }

        private void cbLayerList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            destination_Index = cbLayerList2.SelectedIndex;
            destination.Roi = displays[destination_Index].viewer.Roi;
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

                source1_Index = cbLayerList.SelectedIndex;
                source_1.Roi = displays[source1_Index].viewer.Roi;
                ibSource.Image = (Bitmap)displays[source1_Index].ibSource.Image;

                cbLayerList2_SelectedIndexChanged(null, null);
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnResult_Click(object sender, EventArgs e)
        {
            FormVision_Result formVision_Blob_Result = new FormVision_Result(cResultBlobs);
            formVision_Blob_Result.StartPosition = FormStartPosition.CenterScreen;

            if (!CUtil.OpenCheckForm(formVision_Blob_Result)) return;
            formVision_Blob_Result.Show();
        }
    }
}

