#if EURESYS
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

using OpenCvSharp.UserInterface;

using MetroFramework;
using MetroFramework.Forms;

using Euresys.Open_eVision_2_14;

namespace IntelligentFactory
{
    public partial class FormSettings_ECircleGuage : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;
        private EImageBW8 ImageSource = new EImageBW8();

        private ECircleGauge eCircleGuage = new ECircleGauge();
        private float m_fScaleFactorX = 1.0F;
        private float m_fScaleFactorY = 1.0F;

        private Bitmap ImageDisplay = new Bitmap(1024, 768);

        private int m_nCameraIndex = 0;

        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
        #endregion

        public FormSettings_ECircleGuage(EImageBW8 imageSource)
        {
            InitializeComponent();

            cbIndex.SelectedIndex = 0;
            cbDirection.SelectedIndex = 0;
            try
            {
                if (cbIndex.Items.Count > 0) cbIndex.SelectedIndex = 0;

                if(imageSource.IsVoid)
                {
                    ImageSource.SetSize(2592, 1944);
                }
                else
                {
                    ImageSource.SetSize(imageSource);
                    EasyImage.Oper(EArithmeticLogicOperation.Copy, imageSource, ImageSource);
                }

                if (!ImageSource.IsVoid)
                {
                    ImageDisplay = new Bitmap(ImageSource.Width, ImageSource.Height);
                    ImageSource.SetSize(ImageSource);
                    EasyImage.Oper(EArithmeticLogicOperation.Copy, ImageSource, ImageSource);

                    m_fScaleFactorX = (float)pblImage.Width / (float)ImageSource.Width;
                    m_fScaleFactorY = (float)pblImage.Height / (float)ImageSource.Height;
                    
                    UpdateDisplay();
                }
            }
            catch(EException eDesc)
            {

            }
            catch(Exception Desc)
            {

            }
        }

        private void FormSettings_ECircleGuage_Load(object sender, EventArgs e)
        {
            
        }

        private void pblImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int nPosX = e.X;
                int nPosY = e.Y;

                try
                {
                    if (eCircleGuage != null)
                    {
                        eCircleGuage.Labeled = true;
                        eCircleGuage.SetCursor(nPosX, nPosY);
                        eCircleGuage.HitTest();
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateDisplay(List<Point> points = null)
        {
            if (ImageSource.IsVoid) return;

            using (Graphics g = Graphics.FromImage(ImageDisplay))
            {
                m_fScaleFactorX = (float)pblImage.Width / (float)ImageSource.Width;
                m_fScaleFactorY = (float)pblImage.Height / (float)ImageSource.Height;

                ImageSource.Draw(g, m_fScaleFactorX, m_fScaleFactorY, g.Transform.OffsetX, g.Transform.OffsetY);
                eCircleGuage.Draw(g);
                eCircleGuage.Draw(g, EDrawingMode.Nominal);
                //eLineGuage.Draw(g, EDrawingMode.Actual);
                //eLineGuage.Draw(g, EDrawingMode.InvalidSampledPoints);
                eCircleGuage.Draw(g, EDrawingMode.SampledPoints);

                int nMeasCount = (int)eCircleGuage.NumValidSamples;
                ECircle eCircle = eCircleGuage.MeasuredCircle;

                int nSpecOutMin = int.MaxValue;
                int nSpecOutMax = int.MinValue;

                for (int i = 0; i < nMeasCount; i++)
                {
                    using (EPoint ptMeas = new EPoint())
                    {
                        eCircleGuage.GetSample(ptMeas, (uint)i);
                        

                        if (ptMeas.X == 0 || ptMeas.Y == 0) continue;

                        double dDistance = ptMeas.Distance(eCircle.Center);

                        nSpecOutMin = nSpecOutMin > dDistance ? (int)dDistance : nSpecOutMin;
                        nSpecOutMax = nSpecOutMax < dDistance ? (int)dDistance : nSpecOutMax;
                    }
                }

                string strSpecOutMin = $"SPEC OUT 최소 픽셀 거리 [기준선 <-> 불량]: {nSpecOutMin}";
                string strSpecOutMax = $"SPEC OUT 최대 픽셀 거리 [기준선 <-> 불량]: {nSpecOutMax}";

                g.DrawString(strSpecOutMin, new Font("Arial", 10, FontStyle.Bold), new SolidBrush(Color.Red), new PointF(10, ImageSource.Height * 0.85F * m_fScaleFactorY));
                g.DrawString(strSpecOutMax, new Font("Arial", 10, FontStyle.Bold), new SolidBrush(Color.Red), new PointF(10, ImageSource.Height * 0.90F * m_fScaleFactorY));

                pblImage.Image = ImageDisplay;
            }
        }

        private void pblImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int nPosX = e.X;
                int nPosY = e.Y;

                try
                {
                    if (eCircleGuage != null)
                    {
                        eCircleGuage.Drag(nPosX, nPosY);
                        eCircleGuage.Measure(ImageSource);
                    }

                    //List<Point> points = new List<Point>();

                    //int nMeasCount = eLineGuage.NumValidSamples;

                    //for (int i = 0; i < nMeasCount; i++)
                    //{   
                    //    using (EPoint ptMeas = new EPoint())
                    //    {
                    //        eLineGuage.GetSample(ptMeas, i);
                    //        points.Add(new Point((int)ptMeas.X, (int)ptMeas.Y));
                    //    }
                    //}                    

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
                    if(eCircleGuage != null)
                    {
                        eCircleGuage.Drag(nPosX, nPosY);
                    }
                    
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
            if (ImageSource.IsVoid) return;

            int nThreshold = trbThreshold.Value;
            lbThreshold.Text = nThreshold.ToString();

            using (Graphics g = Graphics.FromImage(ImageDisplay))
            {
                EImageBW8 ImageBinary = new EImageBW8();

                ImageBinary.SetSize(ImageSource);
                EasyImage.Threshold(ImageSource, ImageBinary, (uint)nThreshold);

                ImageBinary.Draw(g, m_fScaleFactorX, m_fScaleFactorY, g.Transform.OffsetX, g.Transform.OffsetY);

                eCircleGuage.Measure(ImageBinary);
                eCircleGuage.Draw(g);
                eCircleGuage.Draw(g, EDrawingMode.Nominal);
                eCircleGuage.Draw(g, EDrawingMode.SampledPoints);

                pblImage.Image = ImageDisplay;
            }

            GC.Collect();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            Stopwatch sw_TaktTimems = new Stopwatch();
            sw_TaktTimems.Start();

            if (ImageSource.IsVoid) return;

            int nThreshold = trbThreshold.Value;
            lbThreshold.Text = nThreshold.ToString();

            Debug.WriteLine($"Takt Time : {sw_TaktTimems.ElapsedMilliseconds} ms");

            using (Graphics g = Graphics.FromImage(ImageDisplay))
            {
                eCircleGuage.SamplingStep = 5;
                EImageBW8 ImageBinary = new EImageBW8();

                ImageBinary.SetSize(ImageSource);
                EasyImage.Threshold(ImageSource, ImageBinary, (uint)nThreshold);

                Debug.WriteLine($"Takt Time : {sw_TaktTimems.ElapsedMilliseconds} ms");

                ImageBinary.Draw(g, m_fScaleFactorX, m_fScaleFactorY, g.Transform.OffsetX, g.Transform.OffsetY);

                Debug.WriteLine($"Takt Time : {sw_TaktTimems.ElapsedMilliseconds} ms");

                eCircleGuage.Measure(ImageBinary);

                Debug.WriteLine($"Takt Time : {sw_TaktTimems.ElapsedMilliseconds} ms");
                eCircleGuage.Draw(g);
                eCircleGuage.Draw(g, EDrawingMode.Nominal);                
                eCircleGuage.Draw(g, EDrawingMode.SampledPoints);

                ECircle eCircle = eCircleGuage.MeasuredCircle;

                float fCenterX = eCircle.CenterX;
                float fCenterY = eCircle.CenterY;

                int nMeasCount = (int)eCircleGuage.NumValidSamples;

                for (int i = 0; i < nMeasCount; i++)
                {
                    using (EPoint ptMeas = new EPoint())
                    {                      

                        eCircleGuage.GetSample(ptMeas, (uint)i);
                        if (ptMeas.X == 0 || ptMeas.Y == 0) continue;

                        double dDistance = ptMeas.Distance(eCircle.Center);

                        bool bInspX = !Global.iSystem.Recipe.Tools[m_SeletedToolIndex].InspY;
                        double dSpecPlusMinus = Global.iSystem.Recipe.Tools[m_SeletedToolIndex].InvalidSpec;

                        float fX = ptMeas.X * m_fScaleFactorX;
                        float fY = ptMeas.Y * m_fScaleFactorY;

                        if (bInspX && dDistance > dSpecPlusMinus)
                        {
                            g.DrawEllipse(new Pen(Color.Red, 1), new RectangleF(fX - 2.5F, fY - 2.5F, 5, 5));
                        }

                        if (!bInspX && dDistance > dSpecPlusMinus)
                        {
                            g.DrawEllipse(new Pen(Color.Red, 1), new RectangleF(fX - 2.5F, fY - 2.5F, 5, 5));
                        }
                    }
                }

                Debug.WriteLine($"Takt Time : {sw_TaktTimems.ElapsedMilliseconds} ms");

                pblImage.Image = ImageDisplay;

                Debug.WriteLine($"Takt Time : {sw_TaktTimems.ElapsedMilliseconds} ms");
            }

            GC.Collect();

            sw_TaktTimems.Stop();
            lbTaktTimems.Text = sw_TaktTimems.ElapsedMilliseconds.ToString() + "ms";
        }

   

        private void btnGrab_Click(object sender, EventArgs e)
        {
            Global.CamManager.Cameras[m_nCameraIndex].Grab();

            Thread.Sleep(250);

            int nThreshold = trbThreshold.Value;
            lbThreshold.Text = nThreshold.ToString();

            using (EImageBW8 ImageGrab = new EImageBW8())
            using (Graphics g = Graphics.FromImage(ImageDisplay))
            {
                ImageGrab.SetSize(Global.CamManager.Cameras[m_nCameraIndex].ImageGrab);
                EasyImage.Oper(EArithmeticLogicOperation.Copy, Global.CamManager.Cameras[m_nCameraIndex].ImageGrab, ImageSource);

                ImageGrab.Draw(g, m_fScaleFactorX, m_fScaleFactorY, g.Transform.OffsetX, g.Transform.OffsetY);

                eCircleGuage.Measure(ImageSource);
                eCircleGuage.Draw(g);
                eCircleGuage.Draw(g, EDrawingMode.Nominal);
                eCircleGuage.Draw(g, EDrawingMode.SampledPoints);

                pblImage.Image = ImageDisplay;
            }

            UpdateDisplay();
        }


        private int m_SeletedToolIndex = 0;
        private void cbIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_SeletedToolIndex = cbIndex.SelectedIndex;

                switch(m_SeletedToolIndex)
                {
                    case DEFINE.CAM1_X1:
                    case DEFINE.CAM1_X2:
                        m_nCameraIndex = 0;
                        break;
                    case DEFINE.CAM2_X1:
                    case DEFINE.CAM2_X2:
                        m_nCameraIndex = 1;
                        break;
                    case DEFINE.CAM3_LX1:
                    case DEFINE.CAM3_LY1:
                    case DEFINE.CAM3_RX1:
                    case DEFINE.CAM3_RY1:
                        m_nCameraIndex = 2;
                        break;
                    case DEFINE.CAM4_LX1:
                    case DEFINE.CAM4_LY1:
                    case DEFINE.CAM4_RX1:
                    case DEFINE.CAM4_RY1:
                        m_nCameraIndex = 3;
                        break;
                }
                
                int nThreshold = Global.iSystem.Recipe.Tools[m_SeletedToolIndex].Threshold;

                eCircleGuage = new ECircleGauge();
                eCircleGuage.SetZoom(m_fScaleFactorX, m_fScaleFactorY);
                eCircleGuage.TransitionType = Global.iSystem.Recipe.Tools[m_SeletedToolIndex].DirectionWB==0 ? ETransitionType.Wb : ETransitionType.Bw;

                tbInvalidSpec.Text = Global.iSystem.Recipe.Tools[m_SeletedToolIndex].InvalidSpec.ToString();
                tbThickness.Text = eCircleGuage.Thickness.ToString();
                tbSmoothing.Text = eCircleGuage.Smoothing.ToString();
                cbDirection.SelectedIndex = Global.iSystem.Recipe.Tools[m_SeletedToolIndex].DirectionWB;

                trbThreshold.Value = nThreshold;
                lbThreshold.Text = nThreshold.ToString();

                lbUseInsp.Style = Global.iSystem.Recipe.Tools[m_SeletedToolIndex].UseInsp ? MetroColorStyle.Lime : MetroColorStyle.Silver;
                lbUseInsp.Text = Global.iSystem.Recipe.Tools[m_SeletedToolIndex].UseInsp ? "검사 사용" : "검사 패스";

                if (m_nCameraIndex >= Global.CamManager.Cameras.Count)
                {
                    IUtil.ShowMessageBox("ALARM", "Camera Index Overflow");
                    return;
                }

                UpdateDisplay();
            }
            catch(Exception Desc)
            {

            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            int nIndex = cbIndex.SelectedIndex;

            eCircleGuage.Thickness = uint.Parse(tbThickness.Text);
            eCircleGuage.Smoothing = uint.Parse(tbSmoothing.Text);

            Global.iSystem.Recipe.Tools[nIndex].Threshold = trbThreshold.Value;
            Global.iSystem.Recipe.Tools[nIndex].InvalidSpec = double.Parse(tbInvalidSpec.Text);
            Global.iSystem.Recipe.Tools[nIndex].DirectionWB = eCircleGuage.TransitionType == ETransitionType.Wb ? 0 : 1;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Global.iSystem.Recipe.SaveTools();
            Global.iSystem.Recipe.SaveConfig();
        }

        private void btnInitPosition_Click(object sender, EventArgs e)
        {
            
        }

        private void cbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nIndex = cbDirection.SelectedIndex;

            if(nIndex == 0)
            {
                eCircleGuage.TransitionType = ETransitionType.Wb;
            }
            else
            {
                eCircleGuage.TransitionType = ETransitionType.Bw;
            }

            UpdateDisplay();

        }

        private void pblImage_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if(e.Button == MouseButtons.Right)
                {
                    if (!ImageSource.IsVoid)
                    {
                        ImageDisplay = new Bitmap(ImageSource.Width, ImageSource.Height);
                        ImageSource.SetSize(ImageSource);
                        EasyImage.Oper(EArithmeticLogicOperation.Copy, ImageSource, ImageSource);

                        m_fScaleFactorX = (float)pblImage.Width / (float)ImageSource.Width;
                        m_fScaleFactorY = (float)pblImage.Height / (float)ImageSource.Height;

                        int nCenterX = ImageSource.Width / 2;
                        int nCenterY = ImageSource.Height / 2;

                        int nTolerance = ImageSource.Width / 10;
                        int nLength = ImageSource.Width / 4;

                        eCircleGuage.SetCenterXY(nCenterX, nCenterY);
                        eCircleGuage.SetZoom(m_fScaleFactorX, m_fScaleFactorY);
                        eCircleGuage.Tolerance = nTolerance;
                        eCircleGuage.Diameter = nLength;
                        eCircleGuage.Thickness = 1;
                        eCircleGuage.Smoothing = 1;
                        eCircleGuage.Threshold = 20;
                        eCircleGuage.Angle = 180;

                        eCircleGuage.TransitionType = ETransitionType.Wb;
                        eCircleGuage.TransitionChoice = ETransitionChoice.NthFromBegin;
                        eCircleGuage.SamplingStep = 5;

                        eCircleGuage.Dragable = true;
                        eCircleGuage.Resizable = true;
                        eCircleGuage.Rotatable = true;
                    }

                    UpdateDisplay();
                }
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
                string strImagePath = IUtil.LoadImage();
                
                if (strImagePath != "") ImageSource.Load(strImagePath);

                m_fScaleFactorX = (float)pblImage.Width / (float)ImageSource.Width;
                m_fScaleFactorY = (float)pblImage.Height / (float)ImageSource.Height;

                eCircleGuage.SetZoom(m_fScaleFactorX, m_fScaleFactorY);

                UpdateDisplay();

                //OpenCvSharp.Mat image = OpenCvSharp.Cv2.ImRead(strImagePath);
            }
            catch (Exception Desc)
            {
                //Logger.WriteLog(LOG.AbNormal, )
            }
        }

        private void btnImageSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ImageSource.IsVoid) return;

                IUtil.InitDirectory(DEFINE.CAPTURE);
                string strImagePath = Application.StartupPath + "\\" + DEFINE.CAPTURE + "\\Image_" + DateTime.Now.ToString("yyyyMMdd-HHmmssfff") + ".jpg";

                int[] sizes = new int[2] { (int)ImageSource.Height, (int)ImageSource.Width };
                IntPtr intPtr = ImageSource.GetImagePtr();
                OpenCvSharp.Mat MatGrab = new OpenCvSharp.Mat(sizes, OpenCvSharp.MatType.CV_8UC1, intPtr);
                OpenCvSharp.Cv2.ImShow("TEST", MatGrab);
                //MatGrab = MatGrab.Resize(new OpenCvSharp.Size(pblMain.Width, pblMain.Height));
                //pblMain.ImageIpl = MatGrab;

                if (strImagePath != "") ImageSource.Save(strImagePath);
            }
            catch (Exception Desc)
            {
                //Logger.WriteLog(LOG.AbNormal, )
            }
        }

        private void lbUseInsp_Click(object sender, EventArgs e)
        {
            try
            {
                Global.iSystem.Recipe.Tools[m_SeletedToolIndex].UseInsp = !Global.iSystem.Recipe.Tools[m_SeletedToolIndex].UseInsp;

                lbUseInsp.Style = Global.iSystem.Recipe.Tools[m_SeletedToolIndex].UseInsp ? MetroColorStyle.Lime : MetroColorStyle.Silver;
                lbUseInsp.Text = Global.iSystem.Recipe.Tools[m_SeletedToolIndex].UseInsp ? "검사 사용" : "검사 패스";
            }
            catch (Exception Desc)
            {
                //Logger.WriteLog(LOG.AbNormal, )
            }
        }
    }
 }

#endif