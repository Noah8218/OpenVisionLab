#if EURESYS
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Euresys.Open_eVision_2_14;

using MetroFramework;
using MetroFramework.Forms;

namespace IntelligentFactory
{
    public partial class FormSettings_EROI : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;

        private bool m_bUseThresholdImage = false;

        private float m_fScaleFactorResizeX = 0;
        private float m_fScaleFactorResizeY = 0;

        private int m_nMinY = 1;
        private int m_nMaxY = 540;
        private int m_nMinX = 1;
        private int m_nMaxX = 720;

        private Point m_ptStart = new Point();
        private Point m_ptEnd = new Point();

        private EImageBW8 ImageSource = new EImageBW8();
        private EImageBW8 ImageRotation = new EImageBW8();

        private EROIBW8 ROI = new EROIBW8();

        private Bitmap ImageDisplay = null;

        private EDragHandle m_DragHandle = EDragHandle.NoHandle;

        private string TEMPLATE_PATH = Application.StartupPath + "\\Template.bmp";

        public FormSettings_EROI(EImageBW8 image)
        {
            InitializeComponent();

            try
            {
                this.SetStyle(ControlStyles.DoubleBuffer, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                this.SetStyle(ControlStyles.ResizeRedraw, true);

                this.DoubleBuffered = true;
                UpdateStyles();

                m_nMinY = 1;
                m_nMaxY = pblImage.Height;
                m_nMinX = 1;
                m_nMaxX = pblImage.Width;

                ImageSource.SetSize(image);
                EasyImage.Oper(EArithmeticLogicOperation.Copy, image, ImageSource);

                UpdateDisplay();
            }
            catch (Exception Desc)
            {
            }
        }

        private void OnClickOperation(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is Button)) return;

                string strIndex = (sender as Button).Text;

                switch (strIndex)
                {
                    case "OK":
                        
                        break;
                    case "CANCEL":
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                        break;
                    case "LOAD":
                        
                        break;
                }

            }
            catch (Exception Desc)
            {
                Debug.WriteLine($"Ex ==> {Desc.Message}");
            }
        }

        private void UpdateDisplay(bool bDrawROI = true)
        {
            try
            {
                if (ImageSource == null) return;
                if (ImageSource.IsVoid) return;

                ImageDisplay = new Bitmap(pblImage.Width, pblImage.Height);

                using (Graphics g = Graphics.FromImage(ImageDisplay))
                {
                    m_fScaleFactorResizeX = (float)pblImage.Width / (float)ImageSource.Width;
                    m_fScaleFactorResizeY = (float)pblImage.Height / (float)ImageSource.Height;

                    ImageSource.Draw(g, m_fScaleFactorResizeX, m_fScaleFactorResizeY);

                    ROI.Attach(ImageSource);                    
                    if (bDrawROI) ROI.DrawFrame(g, true, m_fScaleFactorResizeX, m_fScaleFactorResizeY);


                    pblImage.Image = ImageDisplay;
                }

            }
            catch (Exception Desc)
            {
                Debug.WriteLine($"Ex ==> {Desc.Message}");
            }
        }

        private void pblImage_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    ROI.SetPlacement(ImageSource.Width / 2, ImageSource.Height / 2, 100, 100);
                }

                if (e.Button == MouseButtons.Left && ImageSource != null)
                {
                    int nX = e.X;
                    int nY = e.Y;

                    if (e.Y < m_nMinY) { nY = m_nMinY; }
                    if (e.Y > m_nMaxY) { nY = m_nMaxY; }
                    if (e.X < m_nMinX) { nX = m_nMinX; }
                    if (e.X > m_nMaxX) { nX = m_nMaxX; }

                    m_fScaleFactorResizeX = (float)(pblImage.Width) / (float)(ImageSource.Width);
                    m_fScaleFactorResizeY = (float)(pblImage.Height) / (float)(ImageSource.Height);

                    m_ptStart = new Point(nX, nY);
                    m_ptEnd = new Point(nX, nY);


                    m_DragHandle = ROI.HitTest(nX, nY, m_fScaleFactorResizeX, m_fScaleFactorResizeY);
                }
            }
            catch (Exception Desc)
            {
                Debug.WriteLine($"Ex ==> {Desc.Message}");
            }
        }

        private void pblImage_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left && ImageSource != null)
                {
                    int nX = e.X;
                    int nY = e.Y;

                    if (e.Y < m_nMinY) { nY = m_nMinY; }
                    if (e.Y > m_nMaxY) { nY = m_nMaxY; }
                    if (e.X < m_nMinX) { nX = m_nMinX; }
                    if (e.X > m_nMaxX) { nX = m_nMaxX; }

                    if (m_DragHandle != EDragHandle.NoHandle)
                    {
                        ROI.Drag(m_DragHandle, nX, nY, m_fScaleFactorResizeX, m_fScaleFactorResizeY);
                        UpdateDisplay();

                        lbROI.Text = $"X : {ROI.OrgX} Y : {ROI.OrgY} W : {ROI.Width} H : {ROI.Height}";
                    }

                    m_ptEnd = new Point(nX, nY);

                }
            }
            catch (Exception Desc)
            {
                Debug.WriteLine($"Ex ==> {Desc.Message}");
            }
        }

        private void pblImage_MouseUp(object sender, MouseEventArgs e)
        {
            m_DragHandle = EDragHandle.NoHandle;
        }

        private void btnMatching_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.iSystem.Recipe.ImageTemplate.IsVoid) return;

                IVT_ETemplateMatching ivt_TemplateMatching = new IVT_ETemplateMatching();
                List<IVT_ETemplateMatching_Result> Results = new List<IVT_ETemplateMatching_Result>();

                long lTaktTime = 0;
                Bitmap ImageResult = null;

                Results = ivt_TemplateMatching.Run(ImageSource, Global.iSystem.Recipe.ImageTemplate, out lTaktTime, out ImageResult);

                if (Results.Count > 0)
                {
                    if (ImageResult != null) pblImage.Image = ImageResult;
                }

            }
            catch (Exception Desc)
            {
                Debug.WriteLine($"Ex ==> {Desc.Message}");
            }
        }

        public Rectangle ScreenRectToLogicalRect(Rectangle rt, float fScaleFactorX, float fScaleFactorY)
        {
            rt.X = (int)(rt.X / fScaleFactorX);
            rt.Y = (int)(rt.Y / fScaleFactorY);
            rt.Width = (int)(rt.Width / fScaleFactorX);
            rt.Height = (int)(rt.Height / fScaleFactorY);

            return rt;
        }

        public Rectangle LogicalRectToScreenRect(Rectangle rt, float fScaleFactorX, float fScaleFactorY)
        {
            rt.X = (int)(rt.X * fScaleFactorX);
            rt.Y = (int)(rt.Y * fScaleFactorY);
            rt.Width = (int)(rt.Width * fScaleFactorX);
            rt.Height = (int)(rt.Height * fScaleFactorY);

            return rt;
        }


        private void btnApply_Click(object sender, EventArgs e)
        {
            ROI.Save(Application.StartupPath + "\\Template.bmp", EImageFileType.Bmp);
            Global.iSystem.Recipe.ImageTemplate.SetSize(ROI);
            EasyImage.Oper(EArithmeticLogicOperation.Copy, ROI, Global.iSystem.Recipe.ImageTemplate);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (IUtil.ShowMessageBox("SAVE", "Do you want to save the image?") == true)
            {
                ROI.Save(Application.StartupPath + "\\Template.bmp", EImageFileType.Bmp);
                Global.iSystem.Recipe.ImageTemplate.SetSize(ROI);
                EasyImage.Oper(EArithmeticLogicOperation.Copy, ROI, Global.iSystem.Recipe.ImageTemplate);
            }
        }

        private void btnImageLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "이미지 로드";
            ofd.Filter = "All Files(*.*)|*.*|Bitmap File(*.bmp)|*.bmp|JPEG File(*.jpg)|*.jpg";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string strPath = ofd.FileName;
                ImageSource.Load(strPath);

                ImageDisplay = new Bitmap(pblImage.Width, pblImage.Height);

                using (Graphics g = Graphics.FromImage(ImageDisplay))
                {
                    m_fScaleFactorResizeX = (float)pblImage.Width / (float)ImageSource.Width;
                    m_fScaleFactorResizeY = (float)pblImage.Height / (float)ImageSource.Height;

                    ImageSource.Draw(g, m_fScaleFactorResizeX, m_fScaleFactorResizeY);

                    ROI.Attach(ImageSource);
                    ROI.SetPlacement(ImageSource.Width / 2, ImageSource.Height / 2, 100, 100);
                    ROI.DrawFrame(g, true, m_fScaleFactorResizeX, m_fScaleFactorResizeY);


                    pblImage.Image = ImageDisplay;
                }
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
    }
}

#endif