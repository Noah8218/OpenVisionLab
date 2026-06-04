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
    public partial class FormVision_RotateAndScale : VisionTestForm
    {                
        public FormVision_RotateAndScale(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
            panelCount = this.displayManager.LayerCount;
            timer1.Enabled = true;
            timer1.Start();
        }

        public FormVision_RotateAndScale()
        {

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

            //toolTip1.SetToolTip(btnNewPanel_Source, "Create New Layer");
            toolTip1.SetToolTip(btnNewPanel_Desty, "Create New Layer");
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

        private void btnNewPanel_Source_Click(object sender, EventArgs e)
        {     
            InitLayListItem();
        }

        private void btnMorpRun_Click(object sender, EventArgs e)
        {
            try
            {                
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource.Image).Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);
                   
                    Bitmap Result = new Bitmap(10, 10);
                    //Cv2.MorphologyEx(ImageCVSource, ImageCVSource, CUtil.ParseEnum<MorphTypes>(Operator), Kernel, new OpenCvSharp.Point(-1, -1), 1);
                    //Result = CConverter.ToBitmap(ImageCVSource);

                    SetLayerImage(GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), Result);
                    ibDestination.Image = Result;
                    eventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            source1_Index = cbLayerList.SelectedIndex;
            source_1.Roi = GetLayerRoi(source1_Index);
            ibSource.Image = GetLayerImage(source1_Index);
        }

        private void cbLayerList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            destination_Index = cbLayerList2.SelectedIndex;
            destination.Roi = GetLayerRoi(destination_Index);
            ibDestination.Image = GetLayerImage(destination_Index);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if(panelCount != displayManager.LayerCount)
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

        private void trbRotate_Scroll(object sender, EventArgs e)
        {

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource.Image).Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap Result = new Bitmap(10, 10);
                    Mat ImageRotate = Rotate(ImageCVSource, trbRotate.Value);
                    //Cv2.MorphologyEx(ImageCVSource, ImageCVSource, CUtil.ParseEnum<MorphTypes>(Operator), Kernel, new OpenCvSharp.Point(-1, -1), 1);
                    Result = Lib.Common.CImageConverter.ToBitmap(ImageRotate);

                    SetLayerImage(GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), Result);
                    ibDestination.Image = Result;
                    eventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
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

        private void rjButton1_Click(object sender, EventArgs e)
        {

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource.Image).Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap Result = new Bitmap(10, 10);
                    Mat ImageRotate = Rotate(ImageCVSource, double.Parse(tbRotate.Text));
                    //Cv2.MorphologyEx(ImageCVSource, ImageCVSource, CUtil.ParseEnum<MorphTypes>(Operator), Kernel, new OpenCvSharp.Point(-1, -1), 1);
                    Result = Lib.Common.CImageConverter.ToBitmap(ImageRotate);

                    SetLayerImage(GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), Result);
                    ibDestination.Image = Result;
                    eventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
    }
 }

