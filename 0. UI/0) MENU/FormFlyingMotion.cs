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
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;

using OpenCvSharp;
using OpenCvSharp.UserInterface;
using OpenCvSharp.Blob;

using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;

using ImageGlass;

#if MATROX
using Matrox.MatroxImagingLibrary; 
#endif

#if DATAMATRIXNET
using DataMatrix.net;
#endif

namespace IntelligentFactory
{
    public partial class FormFlyingMotion : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;
        private CStatusMotion Status = null;
        private CStatusMotionHome Home = null;
        private CAXIS_AJIN Axis = null;
        

        //private MIL_ID ImageSource = new MIL_ID();
        private Mat    ImageSource = new Mat();

#if MATROX
        private MIL_ID ImageMSource = MIL.M_NULL;
#endif
        private Bitmap ImageDisplay = new Bitmap(1024, 768);

        private float m_fScaleFactorX = 1.0F;
        private float m_fScaleFactorY = 1.0F;

        private const int DGV_NO = 0;
        private const int DGV_NAME = 1;
        private const int DGV_STATUS = 2;

        private List<string> ListPreImageProcess = new List<string>();

        public float m_fImageScale { get; set; } = 1;

        private bool m_bKeepThreshold = false;
        private bool m_bUseMorpPattern = false;
        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
#endregion

        public FormFlyingMotion()
        {
            InitializeComponent();
        }

        private void FormFlyingMotion_Load(object sender, EventArgs e)
        {
            EventUpdateUi += OnUpdateUi;

            if (EventUpdateUi != null)
            {
                EventUpdateUi(null, null);
            }

            Global.iDevice.CAMERA.EventGrabEnd += OnGrabEnd;

            Global.System.Recipe.EventChagedRecipe += OnChangedRecipe;

            InitUI();
            InitControl();
            InitProperty();
        }

        private void InitControl()
        {
            try
            {
                foreach (DataGridViewColumn i in gridPosition.Columns)
                {
                    i.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                foreach (DataGridViewColumn c in gridPosition.Columns)
                {
                    c.DefaultCellStyle.Font = new Font("Arial", 15F, GraphicsUnit.Pixel);
                }
                ShowVisionPositionDgv();


            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.AbNormal, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public bool ShowVisionPositionDgv()
        {
            try
            {
                if (Global.System.Recipe.PositionManager.ListPosition.Count < 0)
                {
                    //Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, iData.PositionManager.ListPositionVisionStage.Count);
                    return false;
                }

                gridPosition.EnableHeadersVisualStyles = false;

                gridPosition.Rows.Clear();

                int nStageIndex = 0;

                for (int i = 0; i < Global.System.Recipe.PositionManager.ListPosition.Count; i++)
                {
                    if (nStageIndex == 8)
                    {
                        nStageIndex = 0;
                    }
                    nStageIndex++;

                    switch (Global.System.Recipe.PositionManager.ListPosition[i].Axis)
                    {
                        case "X":
                            string strDistancebymm = (Global.System.Recipe.PositionManager.ListPosition[i].Position / 1000.0D).ToString() + "mm";
                            gridPosition.Rows.Add(Global.System.Recipe.PositionManager.ListPosition[i].Axis,
                        Global.System.Recipe.PositionManager.ListPosition[i].Name, Global.System.Recipe.PositionManager.ListPosition[i].Position, strDistancebymm);
                            break;
                        case "Y":
                            strDistancebymm = (Global.System.Recipe.PositionManager.ListPosition[i].Position / 1000.0D).ToString() + "mm";
                            gridPosition.Rows.Add(Global.System.Recipe.PositionManager.ListPosition[i].Axis,
                        Global.System.Recipe.PositionManager.ListPosition[i].Name, Global.System.Recipe.PositionManager.ListPosition[i].Position, strDistancebymm);
                            break;
                        case "Z":
                            strDistancebymm = (Global.System.Recipe.PositionManager.ListPosition[i].Position / 10000.0D).ToString() + "mm";
                            gridPosition.Rows.Add(Global.System.Recipe.PositionManager.ListPosition[i].Axis,
                        Global.System.Recipe.PositionManager.ListPosition[i].Name, Global.System.Recipe.PositionManager.ListPosition[i].Position, strDistancebymm);
                            break;
                        case "R":
                            strDistancebymm = (Global.System.Recipe.PositionManager.ListPosition[i].Position / 5000.0D).ToString() + "mm";
                            gridPosition.Rows.Add(Global.System.Recipe.PositionManager.ListPosition[i].Axis,
                        Global.System.Recipe.PositionManager.ListPosition[i].Name, Global.System.Recipe.PositionManager.ListPosition[i].Position, strDistancebymm);
                            break;
                    }
                }

                gridPosition.Refresh();
                return true;
            }
            catch (Exception ex)
            {
                //Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        private void gridPosition_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (gridPosition.CurrentCell == null)
                {
                    return;
                }
                int nRowIndex = gridPosition.CurrentCell.RowIndex;
                var oldValue = gridPosition[e.ColumnIndex, e.RowIndex].Value;

                if (oldValue == null)
                {
                    return;
                }
                try
                {
                    switch (e.ColumnIndex)
                    {
                        case 0:
                            Global.System.Recipe.PositionManager.ListPosition[nRowIndex].Axis = oldValue.ToString();
                            break;
                        case 1:
                            Global.System.Recipe.PositionManager.ListPosition[nRowIndex].Name = oldValue.ToString();
                            break;
                        case 2:
                            Global.System.Recipe.PositionManager.ListPosition[nRowIndex].Position = long.Parse(oldValue.ToString());
                            break;
                    }

                    gridPosition.Rows[nRowIndex].Frozen = true;
                    gridPosition.EnableHeadersVisualStyles = false;

                    gridPosition.Rows.Clear();

                    for (int i = 0; i < Global.System.Recipe.PositionManager.ListPosition.Count; i++)
                    {
                        switch (Global.System.Recipe.PositionManager.ListPosition[i].Axis)
                        {
                            case "X":
                                string strDistancebymm = (Global.System.Recipe.PositionManager.ListPosition[i].Position / 1000.0D).ToString() + "mm";
                                gridPosition.Rows.Add(Global.System.Recipe.PositionManager.ListPosition[i].Axis,
                            Global.System.Recipe.PositionManager.ListPosition[i].Name, Global.System.Recipe.PositionManager.ListPosition[i].Position, strDistancebymm);
                                break;
                            case "Y":
                                strDistancebymm = (Global.System.Recipe.PositionManager.ListPosition[i].Position / 1000.0D).ToString() + "mm";
                                gridPosition.Rows.Add(Global.System.Recipe.PositionManager.ListPosition[i].Axis,
                            Global.System.Recipe.PositionManager.ListPosition[i].Name, Global.System.Recipe.PositionManager.ListPosition[i].Position, strDistancebymm);
                                break;
                            case "Z":
                                strDistancebymm = (Global.System.Recipe.PositionManager.ListPosition[i].Position / 10000.0D).ToString() + "mm";
                                gridPosition.Rows.Add(Global.System.Recipe.PositionManager.ListPosition[i].Axis,
                            Global.System.Recipe.PositionManager.ListPosition[i].Name, Global.System.Recipe.PositionManager.ListPosition[i].Position, strDistancebymm);
                                break;
                            case "R":
                                strDistancebymm = (Global.System.Recipe.PositionManager.ListPosition[i].Position / 5000.0D).ToString() + "mm";
                                gridPosition.Rows.Add(Global.System.Recipe.PositionManager.ListPosition[i].Axis,
                            Global.System.Recipe.PositionManager.ListPosition[i].Name, Global.System.Recipe.PositionManager.ListPosition[i].Position, strDistancebymm);
                                break;
                        }
                    }
                }
                catch
                {
                    switch (e.ColumnIndex)
                    {
                        case 0:
                            gridPosition[e.ColumnIndex, e.RowIndex].Value = Global.System.Recipe.PositionManager.ListPosition[nRowIndex].Axis;
                            break;
                        case 1:
                            gridPosition[e.ColumnIndex, e.RowIndex].Value = Global.System.Recipe.PositionManager.ListPosition[nRowIndex].Name;
                            break;
                        case 2:
                            gridPosition[e.ColumnIndex, e.RowIndex].Value = Global.System.Recipe.PositionManager.ListPosition[nRowIndex].Position;
                            break;

                    }
                    gridPosition.Rows[nRowIndex].Frozen = true;
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.AbNormal, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

        }

        private void FormMotor_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Global.iDevice.CAMERA.EventGrabEnd -= OnGrabEnd;
            }
            catch(Exception Desc)
            {

            }
        }

        private bool InitUI()
        {
            try
            {
                cbAxis.SelectedIndex = 0;
                cbCamera.SelectedIndex = 0;

                ibSource.MouseWheel += new MouseEventHandler(ImageBox_MouseWheelEvent);
                ibSource.MouseDoubleClick += new MouseEventHandler(ImageBox_MouseDoubleClickEvent);


                this.Invalidate();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool InitProperty()
        {
            try
            {
                Global.iData.PropertyVisionInsp.LoadConfig(Global.System.Recipe.Name);
                Global.SeqVision.Matching.Property.LoadConfig(Global.System.Recipe.Name);

                propertyGrid_Tray.SelectedObject = Global.iData.PropertyTray;
                propertyGrid_Camera.SelectedObject = Global.iDevice.CAMERA.Property;

                propertyGrid_Matching.SelectedObject = Global.iData.PropertyMatching;

                propertyGrid_VisionInsp.SelectedObject = Global.iData.PropertyVisionInsp; 

                ibTemplate.Image = CConverter.ToBitmap(Global.iData.PropertyMatching.ImageTemplate);
                ibTemplate.ZoomToFit();

                InitTrayMap(false);

                this.Invalidate();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void ImageBox_MouseWheelEvent(object sender, MouseEventArgs e)
        {
            try
            {
                if (!(sender is ImageBoxEx)) return;
                ImageBoxEx ib = (sender as ImageBoxEx);

                if (e.Delta > 0) ib.ZoomIn();
                else ib.ZoomOut();
            }
            catch (Exception ex)
            {

            }
        }

        private void ImageBox_MouseDoubleClickEvent(object sender, MouseEventArgs e)
        {
            try
            {
                if (!(sender is ImageBoxEx)) return;
                ImageBoxEx ib = (sender as ImageBoxEx);

                FormImageEditView FrmImageView = new FormImageEditView((Bitmap)ib.Image);
                if (FrmImageView.ShowDialog() == DialogResult.OK)
                {
                    //SetImageData("SOURCE", FrmImageView.ImageProcess);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void OnUpdateUi(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnUpdateUi(sender, e);
                    }));
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.AbNormal, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void OnChangedRecipe(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnChangedRecipe(sender, e);
                    }));
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {
                try
                {
                    InitUI();
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
        }

        #region IMAGEBOX WHEEL

        private void MouseWheelEventSource(object sender, MouseEventArgs e)
        {
            if ((e.Delta / 120) > 0)
            {
                // up
                if (m_fImageScale > 1)
                    m_fImageScale--;

                ZoomInImageSource();
            }
            else
            {
                // down
                m_fImageScale++;

                ZoomOutImageSource();
            }
        }

        private void ZoomInImageSource()
        {
        }

        private void ZoomOutImageSource()
        {
        }

        private void MouseWheelEventResult(object sender, MouseEventArgs e)
        {
            if ((e.Delta / 120) > 0)
            {
                // up
                if (m_fImageScale > 1)
                    m_fImageScale--;

                ZoomInImageResult();
            }
            else
            {
                // down
                m_fImageScale++;

                ZoomOutImageResult();
            }
        }

        private void ZoomInImageResult()
        {
        }

        private void ZoomOutImageResult()
        {
        }

        private void MouseWheelEventProcess(object sender, MouseEventArgs e)
        {
            if ((e.Delta / 120) > 0)
            {
                // up
                if (m_fImageScale > 1)
                    m_fImageScale--;

                ZoomInImageProcess();
            }
            else
            {
                // down
                m_fImageScale++;

                ZoomOutImageProcess();
            }
        }

        private void ZoomInImageProcess()
        {
        }

        private void ZoomOutImageProcess()
        {
        }

#endregion
        private bool InitFilter()
        {
            try
            {
              
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void OnGrabEnd(object sender, GrabEventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        OnGrabEnd(sender, e);
                    }));
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {
                try
                {
                    if (!CUtil.IsImageEmpty(e.ImageGrab))
                    {
                        ImageSource = e.ImageGrab.Clone();

                        if(Global.iDevice.CAMERA.VisibleCross)
                        {
                            CUtil.SetImageChannel3(ImageSource);

                            ImageSource.Line(new OpenCvSharp.Point(0, ImageSource.Height / 2), new OpenCvSharp.Point(ImageSource.Width, ImageSource.Height / 2), Scalar.Red, 2, LineTypes.AntiAlias);
                            ImageSource.Line(new OpenCvSharp.Point(ImageSource.Width/2, 0), new OpenCvSharp.Point(ImageSource.Width/2, ImageSource.Height), Scalar.Red, 2, LineTypes.AntiAlias);
                        }
                        ibSource.Image = CConverter.ToBitmap(ImageSource.Clone());
                    }

                    GC.Collect();
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
        }
#if MATROX

        private bool PreImageProcess(string strFilter, MIL_ID imageSource, MIL_ID imageDest, params object[] args)
        {
            try
            {
                if (imageSource == null) return false;

                switch (strFilter)
                {
                    case "Threshold":             // Param 1 : Threshold
                        if (args.Length != 1) return false;
                        ////EasyImage.Threshold(imageSource, imageDest, (uint)args[0]);                        
                        break;
                    case "Threshold_Inv":         // Param 1 : Threshold
                        if (args.Length != 1) return false;
                        ////EasyImage.Threshold(imageSource, imageDest, (uint)args[0]);
                        ////EasyImage.Oper(EArithmeticLogicOperation.Invert, imageDest, imageDest);
                        break;
                    case "Threshold_Adaptive":    // Param 1 : Kernel Size   Param 2 : Threshold Offset
                        if (args.Length != 2) return false;
                        ////EasyImage.AdaptiveThreshold(imageSource, imageDest, EAdaptiveThresholdMethod.Mean, (int)args[0], (int)args[1]);
                        break;
                    case "Morp_Open":             // Param 1 : Kernel Size
                        if (args.Length != 1) return false;
                        ////EasyImage.OpenBox(imageSource, imageDest, (uint)args[0]);
                        break;
                    case "Morp_Close":            // Param 1 : Kernel Size
                        if (args.Length != 1) return false;
                        //EasyImage.CloseBox(imageSource, imageDest, (uint)args[0]);
                        break;
                    case "Morp_Erode":            // Param 1 : Kernel Size
                        if (args.Length != 1) return false;
                        //EasyImage.ErodeBox(imageSource, imageDest, (uint)args[0]);
                        break;
                    case "Morp_Dilate":           // Param 1 : Kernel Size
                        if (args.Length != 1) return false;
                        //EasyImage.DilateBox(imageSource, imageDest, (uint)args[0]);
                        break;
                    case "Uniform":               // Param 1 : Kernel Size Width   Param 2 : Kernel Size Height
                        if (args.Length != 2) return false;
                        //EasyImage.ConvolUniform(imageSource, imageDest, (uint)args[0], (uint)args[1]);
                        break;
                    case "Gaussian":
                        if (args.Length != 2) return false;
                        //EasyImage.ConvolGaussian(imageSource, imageDest, (uint)args[0], (uint)args[1]);
                        break;
                    case "LowPass":
                        //EasyImage.ConvolLowpass3(imageSource, imageDest);
                        break;
                    case "HighPass":
                        //EasyImage.ConvolHighpass2(imageSource, imageDest);
                        break;
                    case "Gradient":
                        //EasyImage.ConvolGradient(imageSource, imageDest);
                        break;
                    case "GradientX":
                        //EasyImage.ConvolGradientX(imageSource, imageDest);
                        break;
                    case "GradientY":
                        //EasyImage.ConvolGradientY(imageSource, imageDest);
                        break;
                    case "Sobel":
                        //EasyImage.ConvolSobel(imageSource, imageDest);
                        break;
                    case "SobelX":
                        //EasyImage.ConvolSobelX(imageSource, imageDest);
                        break;
                    case "SobelY":
                        //EasyImage.ConvolSobelY(imageSource, imageDest);
                        break;
                    case "Laplacian":
                        //EasyImage.ConvolLaplacian8(imageSource, imageDest);
                        break;
                    case "LaplacianX":
                        //EasyImage.ConvolLaplacianX(imageSource, imageDest);
                        break;
                    case "LaplacianY":
                        //EasyImage.ConvolLaplacianY(imageSource, imageDest);
                        break;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;

        }

#endif
        private void pblImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int nPosX = e.X;
                int nPosY = e.Y;

                try
                {
                    
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateDisplay(List<System.Drawing.Point> points = null)
        {
            //if (ImageSource.IsVoid) return;

            //using (Graphics g = Graphics.FromImage(ImageDisplay))
            //{
            //    m_fScaleFactorX = (float)pblImage.Width / (float)ImageSource.Width;
            //    m_fScaleFactorY = (float)pblImage.Height / (float)ImageSource.Height;

            //    ImageSource.Draw(g, m_fScaleFactorX, m_fScaleFactorY, g.Transform.OffsetX, g.Transform.OffsetY);
            //    eLineGuage.Draw(g);
            //    eLineGuage.Draw(g, EDrawingMode.Nominal);
            //    //eLineGuage.Draw(g, EDrawingMode.Actual);
            //    //eLineGuage.Draw(g, EDrawingMode.InvalidSampledPoints);
            //    eLineGuage.Draw(g, EDrawingMode.SampledPoints);

            //    int nMeasCount = (int)eLineGuage.NumValidSamples;
            //    ELine eLine = eLineGuage.MeasuredLine;

            //    int nSpecOutMin = int.MaxValue;
            //    int nSpecOutMax = int.MinValue;

            //    for (int i = 0; i < nMeasCount; i++)
            //    {
            //        using (EPoint ptMeas = new EPoint())
            //        {
            //            eLineGuage.GetSample(ptMeas, (uint)i);

            //            if (ptMeas.X == 0 || ptMeas.Y == 0) continue;

            //            double dDistance = IMath.GetDistanceLineToPoint(ptMeas, eLine);

            //            nSpecOutMin = nSpecOutMin > dDistance ? (int)dDistance : nSpecOutMin;
            //            nSpecOutMax = nSpecOutMax < dDistance ? (int)dDistance : nSpecOutMax;
            //        }
            //    }

            //    string strSpecOutMin = $"SPEC OUT 최소 픽셀 거리 [기준선 <-> 불량]: {nSpecOutMin}";
            //    string strSpecOutMax = $"SPEC OUT 최대 픽셀 거리 [기준선 <-> 불량]: {nSpecOutMax}";

            //    g.DrawString(strSpecOutMin, new Font("Arial", 10, FontStyle.Bold), new SolidBrush(Color.Red), new PointF(10, ImageSource.Height * 0.85F * m_fScaleFactorY));
            //    g.DrawString(strSpecOutMax, new Font("Arial", 10, FontStyle.Bold), new SolidBrush(Color.Red), new PointF(10, ImageSource.Height * 0.90F * m_fScaleFactorY));

            //    pblImage.Image = ImageDisplay;
            //}
        }

        private void pblImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int nPosX = e.X;
                int nPosY = e.Y;

                try
                {
                    
                    UpdateDisplay();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                GC.Collect();
            }
        }

        private void pblImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int nPosX = e.X;
                int nPosY = e.Y;

                try
                {
                  
                    
                    UpdateDisplay();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void trbThreshold_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void btnRun_Click(object sender, EventArgs e)
        {
           
        }

   

        private void btnGrab_Click(object sender, EventArgs e)
        {
            //Global.CamManager.Cameras[m_nCameraIndex].Grab();

            //Thread.Sleep(250);

            //int nThreshold = trbThreshold.Value;
            //lbThreshold.Text = nThreshold.ToString();

            //using (EImageBW8 ImageGrab = new EImageBW8())
            //using (Graphics g = Graphics.FromImage(ImageDisplay))
            //{
            //    ImageGrab.SetSize(Global.CamManager.Cameras[m_nCameraIndex].ImageGrab);
            //    //EasyImage.Oper(EArithmeticLogicOperation.Copy, Global.CamManager.Cameras[m_nCameraIndex].ImageGrab, ImageSource);

            //    ImageGrab.Draw(g, m_fScaleFactorX, m_fScaleFactorY, g.Transform.OffsetX, g.Transform.OffsetY);

            //    eLineGuage.Measure(ImageSource);
            //    eLineGuage.Draw(g);
            //    eLineGuage.Draw(g, EDrawingMode.Nominal);
            //    eLineGuage.Draw(g, EDrawingMode.SampledPoints);

            //    pblImage.Image = ImageDisplay;
            //}

            //UpdateDisplay();
        }



        private void btnApply_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Global.System.Recipe.SaveConfig();                
            }
            catch (Exception ex)
            {

            }
        }

        private void pblImage_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                //if(e.Button == MouseButtons.Right)
                //{
                //    if (!ImageSource.IsVoid)
                //    {
                //        ImageDisplay = new Bitmap(ImageSource.Width, ImageSource.Height);
                //        ImageSource.SetSize(ImageSource);
                //        //EasyImage.Oper(EArithmeticLogicOperation.Copy, ImageSource, ImageSource);

                //        m_fScaleFactorX = (float)pblImage.Width / (float)ImageSource.Width;
                //        m_fScaleFactorY = (float)pblImage.Height / (float)ImageSource.Height;

                //        int nCenterX = ImageSource.Width / 2;
                //        int nCenterY = ImageSource.Height / 2;

                //        int nTolerance = ImageSource.Width / 10;
                //        int nLength = ImageSource.Width / 4;

                //        eLineGuage.SetCenterXY(nCenterX, nCenterY);
                //        eLineGuage.SetZoom(m_fScaleFactorX, m_fScaleFactorY);
                //        eLineGuage.Tolerance = nTolerance;
                //        eLineGuage.Length = nLength;
                //        eLineGuage.Thickness = 1;
                //        eLineGuage.Smoothing = 1;
                //        eLineGuage.Threshold = 20;
                //        eLineGuage.Angle = 180;

                //        eLineGuage.TransitionType = ETransitionType.Wb;
                //        eLineGuage.TransitionChoice = ETransitionChoice.NthFromBegin;
                //        eLineGuage.SamplingStep = 5;

                //        eLineGuage.Dragable = true;
                //        eLineGuage.Resizable = true;
                //        eLineGuage.Rotatable = true;
                //    }

                //    UpdateDisplay();
                //}
            }
            catch(Exception Desc)
            {
                //Logger.WriteLog(LOG.AbNormal, )
            }
        }

        private void btnImageLoad_Click(object sender, EventArgs e)
        {
            try
            {
                //string strImagePath = CUtil.LoadImage();

                //if (strImagePath != "") ImageSource.Load(strImagePath);
                //UpdateDisplay();
            }
            catch (Exception ex)
            {
                //Logger.WriteLog(LOG.AbNormal, )
            }
        }

        private void btnImageSave_Click(object sender, EventArgs e)
        {
            try
            {
                //if (ImageSource.IsVoid) return;

                //CUtil.InitDirectory(DEFINE.CAPTURE);
                //string strImagePath = Application.StartupPath + "\\" + DEFINE.CAPTURE + "\\Image_" + DateTime.Now.ToString("yyyyMMdd-HHmmssfff") + ".jpg";

                //int[] sizes = new int[2] { (int)ImageSource.Height, (int)ImageSource.Width };
                //IntPtr intPtr = ImageSource.GetImagePtr();
                //OpenCvSharp.Mat MatGrab = new OpenCvSharp.Mat(sizes, OpenCvSharp.MatType.CV_8UC1, intPtr);
                //OpenCvSharp.Cv2.ImShow("TEST", MatGrab);
                ////MatGrab = MatGrab.Resize(new OpenCvSharp.Size(pblMain.Width, pblMain.Height));
                ////pblMain.ImageIpl = MatGrab;

                //if (strImagePath != "") ImageSource.Save(strImagePath);
            }
            catch (Exception ex)
            {
                //Logger.WriteLog(LOG.AbNormal, )
            }
        }

        private void pblImage_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                //if (ImageSource.IsVoid) return;

                //int[] sizes = new int[2] { (int)ImageSource.Height, (int)ImageSource.Width };
                //IntPtr intPtr = ImageSource.GetImagePtr();
                //OpenCvSharp.Mat imageConvert = new OpenCvSharp.Mat(sizes, OpenCvSharp.MatType.CV_8UC1, intPtr);
                //OpenCvSharp.Cv2.ImShow("TEST", imageConvert);

                //FormImageView FrmImageViewer = new FormImageView(imageConvert);
                //FrmImageViewer.Show();
            }
            catch (Exception ex)
            {
                //Logger.WriteLog(LOG.AbNormal, )
            }
            
        }

        private void lbUseInsp_Click(object sender, EventArgs e)
        {
            try
            {
                //Global.iSystem.Recipe.Tools[m_SeletedToolIndex].UseInsp = !Global.iSystem.Recipe.Tools[m_SeletedToolIndex].UseInsp;

                //lbUseInsp.Style = Global.iSystem.Recipe.Tools[m_SeletedToolIndex].UseInsp ? MetroColorStyle.Lime : MetroColorStyle.Silver;
                //lbUseInsp.Text = Global.iSystem.Recipe.Tools[m_SeletedToolIndex].UseInsp ? "검사 사용" : "검사 패스";
            }
            catch (Exception ex)
            {
                //Logger.WriteLog(LOG.AbNormal, )
            }
        }

        private void btnFilterAdd_Click(object sender, EventArgs e)
        {
        }

        private void btnFilterDelete_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridPosition.Rows.Count; i++)
            {
                if(gridPosition.SelectedRows.Count > 0)
                {
                    int nIndex = gridPosition.SelectedRows[0].Index;
                    ListPreImageProcess.RemoveAt(nIndex);

                    break;
                }
            }

            gridPosition.Rows.Clear();
            for (int i = 0; i < ListPreImageProcess.Count; i++)
            {
                string[] strProcess = ListPreImageProcess[i].Split(',');

                gridPosition.Rows.Add(new string[] { (i + 1).ToString(), strProcess[1], strProcess[0] });
            }
        }

        private void OnClickCameraOperation(object sender, EventArgs e)
        {
            try
            {
                string strIndex = (sender as MetroButton).Text;

                switch(strIndex)
                {
                    case "SETTING":                       
                        break;
                    case "GRAB":
                        //Global.iDevice.CAMERA.GrabStart();
                        break;
                    case "LIVE":
                        //Global.iDevice.CAMERA.Live(true);
                        (sender as MetroTile).Text = "LIVE STOP";
                        break;
                    case "LIVE STOP":
                        //Global.iDevice.CAMERA.Live(false);
                        (sender as MetroTile).Text = "LIVE";
                        break;
                    case "IMAGE LOAD":
                        string strImagePath = CUtil.LoadImage();
                        //Bitmap imageload = new Bitmap(strImagePath);
                        ImageSource = Cv2.ImRead(strImagePath);

                        //Bitmap imageLoad = Global.iDevice.CAMERA.LoadImage(strImagePath, ref ImageMSource);
                        //ibSource.Image = imageLoad;
                        //MIL.MdispSelectWindow(Global.iDevice.CAMERA.MIL_Display, ImageMSource, pnDisplay.Handle);
                        InitInteractivity();

                        break;
                    case "IMAGE SAVE":
                        strImagePath = CUtil.SaveImage();
                        Cv2.ImWrite(strImagePath, ImageSource);
                        break;
                    case "TRIG (SW)":
                        Global.iDevice.CAMERA.SoftwareTrigger();
                        break;
                    case "TRIG (HW)":
                        Global.iDevice.CAMERA.HardwareTrigger("IIN1");
                        break;
                    case "보기 (전체)":
                        break;
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        private void InitInteractivity()
        {

        }


        private void btnLightSetting_Click(object sender, EventArgs e)
        {
            try
            {
                FormSettings_Illumination FrmSetting_Illumination = new FormSettings_Illumination();
                FrmSetting_Illumination.ShowDialog();

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void trbLightValue_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
            }
        }

        private void btnCameraSetting_Click(object sender, EventArgs e)
        {
            try
            {
                FormSettings_Grabber FrmSetting_Grabber = new FormSettings_Grabber();
                FrmSetting_Grabber.ShowDialog();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnFindContour_Click(object sender, EventArgs e)
        {
        }

        private void btnROI_BCR_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRun_BCR_Click(object sender, EventArgs e)
        {
        }

        private void btnDistRoiInsp1_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnDistRoiInsp2_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnDistanceRun_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch
            {

            }
        }

        private void btnDistRoiAlign_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void rdbMin_CheckedChanged(object sender, EventArgs e)
        {
            Global.System.Recipe.Insp_Min_Use = true;
            Global.System.Recipe.Insp_Max_Use = false;
            Global.System.Recipe.Insp_Avg_Use = false;
        }

        private void rdbMax_CheckedChanged(object sender, EventArgs e)
        {
            Global.System.Recipe.Insp_Min_Use = false;
            Global.System.Recipe.Insp_Max_Use = true;
            Global.System.Recipe.Insp_Avg_Use = false;
        }

        private void rdbAvg_CheckedChanged(object sender, EventArgs e)
        {
            Global.System.Recipe.Insp_Min_Use = false;
            Global.System.Recipe.Insp_Max_Use = false;
            Global.System.Recipe.Insp_Avg_Use = true;
        }

        private void btnTeaching_Click(object sender, EventArgs e)
        {
#if MATROX
            FormSettings_CVEdge FrmTest = new FormSettings_CVEdge();
            FrmTest.Show(); 
#endif
        }

        private void OnClickCameraOperation(object sender, MouseEventArgs e)
        {
            try
            {
                if (!(sender is MetroButton)) return;

                string strIndex = (sender as MetroButton).Text;

                switch (strIndex)
                {
                    case "SETTING":
                        break;
                    case "GRAB":
                        Global.iDevice.CAMERA.SetExpose(0, Global.iData.PropertyVisionInsp.EXPOSURE_TIME_US);
                        Global.iDevice.CAMERA.Grab();
                        break;
                    case "LIVE":
                        Global.iDevice.CAMERA.SetExpose(0, Global.iData.PropertyVisionInsp.EXPOSURE_TIME_US);
                        (sender as MetroButton).Text = "LIVE STOP";
                        Global.iDevice.CAMERA.Live(true);
                        break;
                    case "LIVE STOP":
                        (sender as MetroButton).Text = "LIVE";
                        Global.iDevice.CAMERA.Live(false);
                        break;
                    case "CROSS":
                        Global.iDevice.CAMERA.VisibleCross = !Global.iDevice.CAMERA.VisibleCross;
                        break;
                    case "IMAGE\r\nLOAD":
                        string strImagePath = CUtil.LoadImage();
                        ImageSource = Cv2.ImRead(strImagePath);
                        ibSource.Image = CConverter.ToBitmap(ImageSource.Clone());
                        ibSource.ZoomToFit();
                        break;
                    case "IMAGE\r\nSAVE":
                        if (!CUtil.IsImageEmpty(ImageSource))
                        {
                            strImagePath = CUtil.SaveImage();
                            Cv2.ImWrite(strImagePath, ImageSource);
                        }
                        break;
                    case "TRIG (SW)":
                        Global.iDevice.CAMERA.SoftwareTrigger();
                        break;
                    case "TRIG (HW)":
                        Global.iDevice.CAMERA.HardwareTrigger();
                        break;
                    case "보기 (전체)":
                        break;
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void cbAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strIndex = cbAxis.Text;

            try
            {
                switch(strIndex)
                {                 
                    default:
                        Axis = Global.iDevice.MOTION_AJIN.AxisWorkLoadingLiftZ;
                        break;
                }

                Status = Axis.Status;
                Home = Axis.Home;
            }
            catch(Exception ex)
            {

            }
        }
       
        private void timerStatus_Tick(object sender, EventArgs e)
        {
            if (Status == null) { return; }
            if (Home == null) { return; }

            if (Status.ServoOn) { CUtil.UpdateLabelOnOff(lbStatusServoOn, true); }
            else { CUtil.UpdateLabelOnOff(lbStatusServoOn, false); }

            if (Status.PlusLimit) { CUtil.UpdateLabelOnOff(lbStatusLimitPlus, true); }
            else { CUtil.UpdateLabelOnOff(lbStatusLimitPlus, false); }

            if (Status.MinusLimit) { CUtil.UpdateLabelOnOff(lbStatusLimitMinus, true); }
            else { CUtil.UpdateLabelOnOff(lbStatusLimitMinus, false); }

            if (Status.ServoAlarm) { CUtil.UpdateLabelOnOff(lbStatusServoAlarm, true); }
            else { CUtil.UpdateLabelOnOff(lbStatusServoAlarm, false); }

            if (Status.Inposition) { CUtil.UpdateLabelOnOff(lbStatusInPosition, true); }
            else { CUtil.UpdateLabelOnOff(lbStatusInPosition, false); }

            if (Status.InMotion) { CUtil.UpdateLabelOnOff(lbStatusMotioning, true); }
            else { CUtil.UpdateLabelOnOff(lbStatusMotioning, false); }

            if (Axis.HomeComplete) { CUtil.UpdateLabelOnOff(lbStatusHomeComplete, true); }
            else { CUtil.UpdateLabelOnOff(lbStatusHomeComplete, false); }

            


            lbActualPulse.Text = (Status.ActualPos).ToString();
            lbActualDistance.Text = (Status.ActualPos * Axis.PulsePermm).ToString("F3") + "mm";
            lbCommandPulse.Text = Status.CommandPos.ToString();
            lbCommandDistance.Text = (Status.CommandPos * Axis.PulsePermm).ToString("F3") + "mm";


            CUtil.UpdateLabelOnOff(lbStatusHome, false);

            // Home은 언제 보려고??            
        }

        private void btnJogPlus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (Axis == null) return;
                int nJogSpeed = int.Parse(tbJogSpeed.Text);

                string strIndex = "";

                if (sender is Button) strIndex = ((Button)sender).Name;

                switch (strIndex)
                {
                    case "btnJogPlus":
                        Axis.JogStart(nJogSpeed);
                        break;
                    case "btnJogMinus":
                        Axis.JogStart(nJogSpeed * -1);
                        break;
                }

                
            }
            catch(Exception ex)
            {

            }
        }

        private void btnJogPlus_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (Axis == null) return;
                Axis.JogStop();
            }
            catch(Exception ex)
            {

            }
            
        }

        private void btnStepMove_Click(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) return;
                if (Axis.Status.InMotion) return;

                if (!Axis.HomeComplete)
                {
                    CUtil.ShowMessageBox("ALARM", "SET THE HOME FIRST");
                    return;
                }

                double dPulsePermm = Axis.PulsePermm;

                double dStepSpeed = double.Parse(tbStepSpeed.Text);
                double dStepmm = double.Parse(tbStepmm.Text);
                double dActualPos = Axis.Status.ActualPos;
                double dTargetPos = dActualPos + (dStepmm / dPulsePermm);

                Axis.SetMotionEndVelocity(Axis.AxisNo, dStepSpeed);
                Axis.Move(dTargetPos, dStepSpeed);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnAbsoluteMove_Click(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) return;
                if (Axis.Status.InMotion) return;

                if (!Axis.HomeComplete)
                {
                    CUtil.ShowMessageBox("ALARM", "SET THE HOME FIRST");
                    return;
                }

                double dPulsePermm = Axis.PulsePermm;

                double dMoveSpeed = double.Parse(tbAbsoluteSpeed.Text);
                double dTargetmm = double.Parse(lbAbsoluteTarget.Text);
                double dTargetPos = (dTargetmm  / dPulsePermm);

                Axis.SetMotionEndVelocity(Axis.AxisNo, dMoveSpeed);
                Axis.Move(dTargetPos, dMoveSpeed);
            }
            catch (Exception ex)
            {

            }
        }

        private void OnMouseDownJog(object sender, MouseEventArgs e)
        {
            try
            {
                string strIndex = "";

                string strIndexSpeed = tbJogSpeed.Text;
                int nSpeed = 0;
                switch (strIndexSpeed)
                {
                    case "SLOW":
                        nSpeed = 5000;
                        break;
                    case "NORMAL":
                        nSpeed = 20000;
                        break;
                    case "FAST":
                        nSpeed = 40000;
                        break;
                }

                if (sender is Button) strIndex = ((Button)sender).Text;
                if (Axis == null) return;

                if (!Global.iDevice.MOTION_AJIN.HOME.ThreadStatusAllHome.IsExit())
                {
                    CUtil.ShowMessageBox("ERROR", "Homing");
                    return;
                }

                //if (Axis.AxisName != "Z"
                //    && !Global.iDevice.MOTION_AJIN.AxisZ.HomeComplete)
                //{
                //    CUtil.ShowMessageBox("ERROR", "Should be Home Z");
                //    return;
                //}

                switch (strIndex)
                {
                    case "JOG +":
                        Axis.JogStart(nSpeed);
                        break;
                    case "JOG -":
                        Axis.JogStart(nSpeed * -1);
                        break;
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}   Action ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, strIndex);

            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.AbNormal, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnMouseUpJog(object sender, MouseEventArgs e)
        {
            try
            {
                string strIndex = "";

                if (sender is Button) strIndex = ((Button)sender).Text;
                if (Axis == null) return;

                switch (strIndex)
                {
                    case "JOG +":
                        Axis.JogStop();
                        break;
                    case "JOG -":
                        Axis.JogStop();
                        break;
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}   Action ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, strIndex);

            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.AbNormal, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnJogPlus_Click(object sender, EventArgs e)
        {

        }

        private void lbStatusServoOn_Click(object sender, EventArgs e)
        {
            try
            {
                string strIndex = "";

                if (sender is Label) strIndex = ((Label)sender).Text;

                if(Status.ServoOn)
                {
                    Axis.ServoOnOff(false);
                    CUtil.UpdateLabelOnOff((Label)sender, false);
                }
                else
                {
                    Axis.ServoOnOff(true);
                    CUtil.UpdateLabelOnOff((Label)sender, true);
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}   Action ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, strIndex);

            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.AbNormal, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnPositionSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (CUtil.ShowMessageBox("Save Position", string.Format("Do you want to Save Position?"), FormMessageBox.MESSAGEBOX_TYPE.OKCANCEL))
                {
                    Global.System.Recipe.PositionManager.SaveConfig();
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.AbNormal, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private const int COL_AXIS = 0;
        private const int COL_NAME = 1;
        private const int COL_POS = 2;

        public long GetPos(DEFINE.MOVE_POS index)
        {
            return Global.System.Recipe.PositionManager.ListPosition[(int)index].Position;
        }

        private void btnMovePosition_Click(object sender, EventArgs e)
        {
            try
            {
                //if (Global.iDevice.MOTION_AJIN.AxisX.Status.InMotion
                //    || Global.iDevice.MOTION_AJIN.AxisY.Status.InMotion
                //    || Global.iDevice.MOTION_AJIN.AxisZ.Status.InMotion
                //    || Global.iDevice.MOTION_AJIN.AxisR.Status.InMotion)
                //{
                //    CUtil.ShowMessageBox("ALARM", "SOULD TO MOVE AFETER MOVING");
                //    return;
                //}

                int nRowIndex = gridPosition.CurrentRow.Index;

                string strAxis = gridPosition[COL_AXIS, nRowIndex].Value.ToString();
                string strName = gridPosition[COL_NAME, nRowIndex].Value.ToString();
                string strPos = gridPosition[COL_POS, nRowIndex].Value.ToString();

                if (CUtil.ShowMessageBox("MOVE POS", string.Format("Do you want to move to {0}?", strName), FormMessageBox.MESSAGEBOX_TYPE.OKCANCEL))
                {
                    //switch (strAxis)
                    //{
                    //    case "Z":
                    //        if (!Global.iDevice.MOTION_AJIN.AxisZ.HomeComplete)
                    //        {
                    //            CUtil.ShowMessageBox("ERROR", "Axis Z Not Completed the Home");
                    //            return;
                    //        }
                    //        break;
                    //    case "X":
                    //        if (!Global.iDevice.MOTION_AJIN.AxisX.HomeComplete)
                    //        {
                    //            CUtil.ShowMessageBox("ERROR", "Axis X Not Completed the Home");
                    //            return;
                    //        }
                    //        break;
                    //    case "Y":
                    //        if (!Global.iDevice.MOTION_AJIN.AxisY.HomeComplete)
                    //        {
                    //            CUtil.ShowMessageBox("ERROR", "Axis Y Not Completed the Home");
                    //            return;
                    //        }
                    //        break;
                    //    case "R":
                    //        if (!Global.iDevice.MOTION_AJIN.AxisR.HomeComplete)
                    //        {
                    //            CUtil.ShowMessageBox("ERROR", "Axis R Not Completed the Home");
                    //            return;
                    //        }
                    //        break;
                    //}


                    bool bPosZ_Handler = true;
                    long lPos = long.Parse(strPos);

                    switch (strAxis)
                    {
                        case "X":
                            if (GetPos(DEFINE.MOVE_POS.X_READY) == lPos
                                || GetPos(DEFINE.MOVE_POS.X_ULD_READY) == lPos)
                            {
                                bPosZ_Handler = true;
                            }
                            else
                            {
                                bPosZ_Handler = false;
                            }


                            break;
                        case "Y":
                            if (GetPos(DEFINE.MOVE_POS.Y_CENTER) != lPos) bPosZ_Handler = true;
                            else bPosZ_Handler = false;
                            break;
                    }

                    //if(bPosZ_Handler)
                    //{
                    //    if (strAxis != "Z")
                    //    {
                    //        long lPosZ = GetPos(DEFINE.MOVE_POS.Z_HANDLER);
                    //        Global.iDevice.MOTION_AJIN.AxisZ.Move(lPosZ);

                    //        int nStartTime = Environment.TickCount;

                    //        Global.iDevice.MOTION_AJIN.AxisZ.Wait();
                    //    }
                    //}

                    //switch (strAxis)
                    //{
                    //    case "Z":
                    //        Global.iDevice.MOTION_AJIN.AxisZ.Move(lPos);
                    //        break;
                    //    case "X":
                    //        Global.iDevice.MOTION_AJIN.AxisX.Move(lPos);
                    //        break;
                    //    case "Y":
                    //        Global.iDevice.MOTION_AJIN.AxisY.Move(lPos);
                    //        break;
                    //    case "R":
                    //        Global.iDevice.MOTION_AJIN.AxisR.Move(lPos);
                    //        break;
                    //}
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.AbNormal, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnKeyPressOnlyNum(object sender, KeyPressEventArgs e)
        {
            CUtil.InputOnlyNumber(sender, e, true, true);
        }

        private void btnGetCurrentPosition_Click(object sender, EventArgs e)
        {
            try
            {
                int nRowIndex = gridPosition.CurrentRow.Index;

                string strAxis = gridPosition[COL_AXIS, nRowIndex].Value.ToString();
                string strName = gridPosition[COL_NAME, nRowIndex].Value.ToString();
                string strPos = gridPosition[COL_POS, nRowIndex].Value.ToString();

                double dPos = 0;

                if (CUtil.ShowMessageBox("GET POS", string.Format("DO YOU WANT TO GET POSITION OF CURRENT {0}?", strName), FormMessageBox.MESSAGEBOX_TYPE.OKCANCEL))
                {
                    //switch (strAxis)
                    //{
                    //    case "X":
                    //        dPos = Global.iDevice.MOTION_AJIN.AxisX.Status.ActualPos;
                    //        break;
                    //    case "Y":
                    //        dPos = Global.iDevice.MOTION_AJIN.AxisY.Status.ActualPos;
                    //        break;
                    //    case "Z":
                    //        dPos = Global.iDevice.MOTION_AJIN.AxisZ.Status.ActualPos;
                    //        break;
                    //    case "R":
                    //        dPos = Global.iDevice.MOTION_AJIN.AxisR.Status.ActualPos;
                    //        break;
                    //}

                    Global.System.Recipe.PositionManager.ListPosition[nRowIndex].Position = (long)dPos;

                    gridPosition.Rows.Clear();

                    for (int i = 0; i < Global.System.Recipe.PositionManager.ListPosition.Count; i++)
                    {
                        switch (Global.System.Recipe.PositionManager.ListPosition[i].Axis)
                        {
                            case "X":
                                string strDistancebymm = (Global.System.Recipe.PositionManager.ListPosition[i].Position / 1000.0D).ToString() + "mm";
                                gridPosition.Rows.Add(Global.System.Recipe.PositionManager.ListPosition[i].Axis,
                                Global.System.Recipe.PositionManager.ListPosition[i].Name, Global.System.Recipe.PositionManager.ListPosition[i].Position, strDistancebymm);
                                break;
                            case "Y":
                                strDistancebymm = (Global.System.Recipe.PositionManager.ListPosition[i].Position / 1000.0D).ToString() + "mm";
                                gridPosition.Rows.Add(Global.System.Recipe.PositionManager.ListPosition[i].Axis,
                                Global.System.Recipe.PositionManager.ListPosition[i].Name, Global.System.Recipe.PositionManager.ListPosition[i].Position, strDistancebymm);
                                break;
                            case "Z":
                                strDistancebymm = (Global.System.Recipe.PositionManager.ListPosition[i].Position / 10000.0D).ToString() + "mm";
                                gridPosition.Rows.Add(Global.System.Recipe.PositionManager.ListPosition[i].Axis,
                                Global.System.Recipe.PositionManager.ListPosition[i].Name, Global.System.Recipe.PositionManager.ListPosition[i].Position, strDistancebymm);
                                break;
                            case "R":
                                strDistancebymm = (Global.System.Recipe.PositionManager.ListPosition[i].Position / 5000.0D).ToString() + "mm";
                                gridPosition.Rows.Add(Global.System.Recipe.PositionManager.ListPosition[i].Axis,
                                Global.System.Recipe.PositionManager.ListPosition[i].Name, Global.System.Recipe.PositionManager.ListPosition[i].Position, strDistancebymm);
                                break;
                        }
                    }
                }
                }
            catch
            {

            }
        }

        private void btnPropertyTrayCreate_Click(object sender, EventArgs e)
        {
            InitTrayMap(true);
            propertygrid_Parameter.Invalidate();
        }

        private void InitTrayMap(bool bIsCreate)
        {
            try
            {

                int nBetweenIndex = Global.iData.PropertyTray.BOTTOM_WINDOW_INDEX;

                int nStripColCount = Global.iData.PropertyTray.COLUMNS;
                int nStripRowCount = Global.iData.PropertyTray.ROWS;

                double dPitchX_mm = Global.iData.PropertyTray.PITCH_X_MM;
                double dPitchY_mm = Global.iData.PropertyTray.PITCH_Y_MM;

                double dWindowGap_mm = Global.iData.PropertyTray.WINDOW_GAP_MM;

                System.Drawing.Point ptFirstPos = new System.Drawing.Point();

                //if(bIsCreate)
                //{
                //    ptFirstPos = new System.Drawing.Point((int)Global.iDevice.MOTION_AJIN.AxisX.ActualPos, (int)Global.iDevice.MOTION_AJIN.AxisY.ActualPos);
                //}
                //else
                //{
                //    ptFirstPos = Global.iData.PropertyTray.FIRST_INSP_POS;
                //}

                dgvMap.Rows.Clear();
                dgvMap.Columns.Clear();
                dgvMap.ColumnHeadersVisible = false;

                dgvMap.ColumnCount = nStripColCount;

                int nColWidth = (dgvMap.Width - 2) / (nStripColCount);
                int nRowHeight = (dgvMap.Height - 2) / (nStripRowCount);

                for (int i = 0; i < nStripColCount; i++)
                {
                    dgvMap.Columns[i].Width = nColWidth;
                }

                for (int i = 0; i < nStripRowCount; i++)
                {
                    string[] strRowData = new string[nStripColCount];

                    for (int j = 0; j < strRowData.Length; j++)
                    {
                        strRowData[j] = ((i * nStripColCount) + j).ToString();
                    }

                    dgvMap.Rows.Add(strRowData);
                    dgvMap.Rows[dgvMap.Rows.Count - 1].Height = nRowHeight;
                }


                Global.iData.PropertyTray.Init(nStripColCount, nStripRowCount, dPitchX_mm, dPitchY_mm, nBetweenIndex, dWindowGap_mm, ptFirstPos);

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private int m_nSelectedX = 0;
        private int m_nSelectedY = 0;
        private void dgvMap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                m_nSelectedX = e.ColumnIndex;
                m_nSelectedY = e.RowIndex;

                System.Drawing.PointF ptSelectPos = Global.iData.PropertyTray.GetPos(m_nSelectedX, m_nSelectedY);
                lbSelectedPos.Text = $"X : {m_nSelectedX}[{ptSelectPos.X}]\nY : {m_nSelectedY}[{ptSelectPos.Y}]"; 
                

                CLogger.Add(LOG.NORMAL, "[SELECT] X: {0} Y : {1}", m_nSelectedX, m_nSelectedY);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message); 
            }
        }

        private void btnSelectedMove_Click(object sender, EventArgs e)
        {
            try
            {
                string strIndex = cbAxis.Text;

                //if (!Global.iDevice.MOTION_AJIN.AxisX.HomeComplete) { CUtil.ShowMessageBox("ERROR", "Axis X Not Completed the Home"); return; }
                //if (!Global.iDevice.MOTION_AJIN.AxisY.HomeComplete) { CUtil.ShowMessageBox("ERROR", "Axis Y Not Completed the Home"); return; }
                //if (!Global.iDevice.MOTION_AJIN.AxisZ.HomeComplete) { CUtil.ShowMessageBox("ERROR", "Axis Z Not Completed the Home"); return; }
                //if (!Global.iDevice.MOTION_AJIN.AxisR.HomeComplete) { CUtil.ShowMessageBox("ERROR", "Axis R Not Completed the Home"); return; }
                                
                //if (!Global.iDevice.MOTION_AJIN.AxisZ.InpositionEx(GetPos(DEFINE.MOVE_POS.Z_VISION))) 
                //{
                //    Global.iDevice.MOTION_AJIN.AxisZ.Move(GetPos(DEFINE.MOVE_POS.Z_VISION));
                //    CUtil.ShowMessageBox("ERROR", "Please Move the Position of Z To Z_HANDLER"); 
                //    return; 
                //}

                bool bOK = CUtil.ShowMessageBox("MOVE OK?", string.Format("Do you want to Move at X : {0} Y : {1} ?", m_nSelectedX, m_nSelectedY), FormMessageBox.MESSAGEBOX_TYPE.OKCANCEL);

                if (bOK)
                {
                    //MOVE
                    PointF ptPos = Global.iData.PropertyTray.GetPos(m_nSelectedX, m_nSelectedY);
                    //Global.iDevice.MOTION_AJIN.MoveToPos(ptPos.X, ptPos.Y);
                }

                CLogger.Add(LOG.NORMAL, "[MOVE] X: {0} Y : {1} OK? {2}", m_nSelectedX, m_nSelectedY, bOK);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnPropertTraySave_Click(object sender, EventArgs e)
        {
            Global.iData.PropertyTray.SaveConfig(Global.System.Recipe.Name);
        }

        private void btnPropertyCameraApply_Click(object sender, EventArgs e)
        {
            try
            {
                Global.iDevice.CAMERA.SetExpose(Global.iDevice.CAMERA.Property.INDEX, Global.iDevice.CAMERA.Property.EXPOSURETIME_US);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnPropertyCameraSave_Click(object sender, EventArgs e)
        {
            try
            {
                Global.iDevice.CAMERA.Property.SaveConfig();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void trbThreshold_Scroll_1(object sender, ScrollEventArgs e)
        {
            try
            {
                
                int nThreshold = trbThreshold.Value;
                lbThreshold.Text = nThreshold.ToString( );

                if ( CUtil.IsImageEmpty( ImageSource ) ) return;
                if ( CUtil.IsRectEmpty( m_rtDefaultRoi ) ) m_rtDefaultRoi = new Rect( 0, 0, ImageSource.Width, ImageSource.Height );

                using ( Mat ImageSrc = ImageSource.Clone( ) )
                using ( Mat ImageSub = ImageSrc.SubMat( m_rtDefaultRoi ) )
                {
                    //CUtil.SetImageChannel1( ImageSub );
                    Cv2.Threshold( ImageSub, ImageSub, nThreshold, 255, ThresholdTypes.Binary );

                    if(m_rtDefaultRoi.Width != ImageSrc.Width
                        && m_rtDefaultRoi.Height != ImageSrc.Height )
                    {
                        ImageSrc.Rectangle( m_rtDefaultRoi, Scalar.Blue, 2 );
                    }

                    ibSource.Image = ( Image ) OpenCvSharp.Extensions.BitmapConverter.ToBitmap( ImageSrc ).Clone( );
                }
                             
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnTrainTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                FormImageEditView FrmImageView = new FormImageEditView(ImageSource);
                if (FrmImageView.ShowDialog() == DialogResult.OK)
                {
                    if (CUtil.IsImageEmpty(ImageSource)) return;
                    //if (CUtil.IsRectEmpty(m_rtDefaultRoi)) m_rtDefaultRoi = new Rect(0, 0, ImageSource.Width, ImageSource.Height);

                    Global.iData.PropertyMatching.ImageTemplate = FrmImageView.ImageProcess;

                    ibTemplate.Image = CConverter.ToBitmap(Global.iData.PropertyMatching.ImageTemplate);
                    ibTemplate.ZoomToFit();
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnRunTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                if(CUtil.IsImageEmpty(ImageSource))
                {
                    CLogger.Add(LOG.NORMAL, "IMAGE SOURCE is Empty");
                    CUtil.ShowMessageBox("NOTICE", "IMAGE SOURCE is Empty");
                    return;
                }                

                Global.SeqVision.Matching.SetProperty(Global.iData.PropertyMatching);
                Global.SeqVision.Matching.SetSourceImage(ImageSource);
                Global.SeqVision.Matching.Run();

                if(Global.SeqVision.Matching.Results.Count > 0)
                {
                    ibSource.Image = CConverter.ToBitmap(Global.SeqVision.Matching.ImageResult);
                    
                }
                else
                {
                    CLogger.Add(LOG.NORMAL, "CAN'T FIND THE TEMPLATE");
                    CUtil.ShowMessageBox("NOTICE", "CAN'T FIND THE TEMPLATE");
                    return;
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                Global.iData.PropertyMatching.SaveConfig(Global.System.Recipe.Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnChangeTabPage( object sender, EventArgs e )
        {
            try
            {
                string strIndex = tcParameter.TabPages[tcParameter.SelectedIndex].Text;

                switch(strIndex)
                {
                    case "Parameter (S/W)":
                    case "Parameter (H/W)":
                        timerIO.Enabled = false;
                        break;

                    case "INPUT":
                    case "OUTPUT":
                        timerIO.Enabled = true;
                        break;
                }
            }
            catch ( Exception ex )
            {
                CLogger.Add( LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod( ).ReflectedType.Name, MethodBase.GetCurrentMethod( ).Name, ex.Message );
            }
        }

        private void timerIO_Tick( object sender, EventArgs e )
        {
            try
            {
            }
            catch ( Exception ex )
            {
                CLogger.Add( LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod( ).ReflectedType.Name, MethodBase.GetCurrentMethod( ).Name, ex.Message );
            }
        }

        private void dgvDO_CellContentClick( object sender, DataGridViewCellEventArgs e )
        {
            try
            {
            }
            catch ( Exception ex )
            {
                CLogger.Add( LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod( ).ReflectedType.Name, MethodBase.GetCurrentMethod( ).Name, ex.Message );
            }
        }

        private void btnKeepThreshold_Click( object sender, EventArgs e )
        {
            try
            {
                m_bKeepThreshold = !m_bKeepThreshold;

                CUtil.UpdateLabelOnOff( btnKeepThreshold, m_bKeepThreshold );
            }
            catch ( Exception ex )
            {
                CLogger.Add( LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod( ).ReflectedType.Name, MethodBase.GetCurrentMethod( ).Name, ex.Message );
            }
            
        }

        private Rect m_rtDefaultRoi = new Rect();
        private void btnDefaultRoi_Click( object sender, EventArgs e )
        {
            try
            {
                FormImageEditView FrmImageView = new FormImageEditView(ImageSource);
                if ( FrmImageView.ShowDialog( ) == DialogResult.OK )
                {
                    m_rtDefaultRoi = FrmImageView.SelectedRegion;
                }
            }
            catch ( Exception ex )
            {
                CLogger.Add( LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod( ).ReflectedType.Name, MethodBase.GetCurrentMethod( ).Name, ex.Message );
            }
        }

        private void btnMeasureMean_Click( object sender, EventArgs e )
        {
            try
            {

                int nThreshold = trbThreshold.Value;
                lbThreshold.Text = nThreshold.ToString( );

                if ( CUtil.IsImageEmpty( ImageSource ) ) return;
                if ( CUtil.IsRectEmpty( m_rtDefaultRoi ) ) m_rtDefaultRoi = new Rect( 0, 0, ImageSource.Width, ImageSource.Height );

                using ( Mat imageSrc = ImageSource.Clone( ) )
                using ( Mat imageSub = imageSrc.SubMat( m_rtDefaultRoi ) )
                {
                    CUtil.SetImageChannel1( imageSub );
                    lbMean.Text = Cv2.Mean( imageSub ).Val0.ToString( "F2" );
                }
            }
            catch ( Exception ex )
            {
                CLogger.Add( LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod( ).ReflectedType.Name, MethodBase.GetCurrentMethod( ).Name, ex.Message );
            }
        }

        private void btnManualInsp_Click(object sender, EventArgs e)
        {
            try
            {

                int nThreshold = trbThreshold.Value;

                if (CUtil.IsImageEmpty(ImageSource)) return;
                //if (CUtil.IsRectEmpty(m_rtDefaultRoi)) m_rtDefaultRoi = new Rect(0, 0, ImageSource.Width, ImageSource.Height);

                using (Mat imageSrc = ImageSource.Clone())
                using (Mat imageBin = ImageSource.Clone())
                using (Mat imageRst = ImageSource.Clone())
                {
                    CUtil.SetImageChannel1(imageSrc);
                    CUtil.SetImageChannel3(imageRst);

                    double dMean = Cv2.Mean(imageSrc).Val0;
                    double dMeanWeight = dMean + Global.iData.PropertyVisionInsp.PARAM_WEIGHT;

                    int nAreaMax = Global.iData.PropertyVisionInsp.AREA_MAX;
                    int nAreaMin = Global.iData.PropertyVisionInsp.AREA_MIN;

                    Cv2.Threshold(imageSrc, imageBin, dMeanWeight, 255, ThresholdTypes.Binary);

                    CvBlobs Blobs = new CvBlobs();
                    Blobs.Label(imageBin);
                    Blobs.FilterByArea(nAreaMin, nAreaMax);
                    Blobs.RenderBlobs(imageBin, imageRst);

                    foreach (var item in Blobs)
                    {
                        CvBlob b = item.Value;
                        Cv2.PutText(imageRst, b.Area.ToString(), new OpenCvSharp.Point(b.Centroid.X, b.Centroid.Y), HersheyFonts.HersheyComplex, 0.75, Scalar.Blue, 1, LineTypes.AntiAlias);
                    }

                    FormImageEditView FrmImageView = new FormImageEditView(imageRst);
                    FrmImageView.Show();
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        private void btnSetSourceOriginal_Click(object sender, EventArgs e)
        {
            try
            {
                ibSource.Image = CConverter.ToBitmap(ImageSource);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        private void btnVisionInspApply_Click(object sender, EventArgs e)
        {
            try
            {
                Global.iDevice.CAMERA.SetExpose(0, Global.iData.PropertyVisionInsp.EXPOSURE_TIME_US);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnVisionInspSave_Click(object sender, EventArgs e)
        {
            try
            {
                Global.iData.PropertyVisionInsp.SaveConfig(Global.System.Recipe.Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

    }
 }