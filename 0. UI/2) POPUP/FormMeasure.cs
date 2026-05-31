using System;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Lib.Common;
using Lib.Line;

namespace OpenVisionLab
{
    public partial class FormMeasure : RJCodeUI_M1.RJForms.RJChildForm
    {
        private Bitmap m_ImageOriginal = null;
        private Bitmap m_ImageDisplay = null;

        private CViewer ImageView = new CViewer(false);

        private bool m_bFullScreen = false;
        private bool m_bUseMouseDown = false;
        
        private System.Drawing.Point GreyPoint = new System.Drawing.Point();
        private System.Drawing.Point LocationPoint = new System.Drawing.Point();
        private Point m_ptStart = new Point();
        private Point m_ptEnd = new Point();
        private OpenCvSharp.Point m_ptBase = new OpenCvSharp.Point();
        private OpenCvSharp.Point m_ptVertical = new OpenCvSharp.Point();

        private List<System.Drawing.Point> m_Points = new List<Point>();

        public EventHandler<RectEventArgs> EventRoiEnd;
        private EventHandler<EventArgs> EventUpdateUi;
        
        private bool m_bHighQuality = true;
        
        public bool HighQuality
        {
            set
            {
                if (value != m_bHighQuality)
                {
                    m_bHighQuality = value;
                    Refresh(); // Force redraw
                }
            }
            get { return m_bHighQuality; }
        }

        private const int MOUSEMODE_NONE = -1;
        private const int MOUSEMODE_IDLE = 0;
        private const int MOUSEMODE_START = 1;
        private const int MOUSEMODE_END = 2;
        private const int MOUSEMODE_MOVE = 3;
        private const int MOUSEMODE_CREATEVERTICAL = 4;

        private int m_nMouseMode = MOUSEMODE_IDLE;
        public enum SettingMode
        {
            Point = 0
        }

        private SettingMode m_Mode = SettingMode.Point;
        public SettingMode Mode
        {
            get
            {
                return m_Mode;
            }
            set
            {
                m_Mode = value;
                if (EventUpdateUi != null)
                {
                    EventUpdateUi(null, null);
                }
            }
        }
            
        public FormMeasure(Bitmap ImageSource)
        {
            InitializeComponent();

            ImageView.LoadImageBoxOnlyViewer(ibSource);
            ImageView._Ib.Text = "";
            //this.Style = MetroColorStyle.Teal;
            //this.Theme = MetroThemeStyle.Dark;

            m_ImageOriginal = ImageSource.Clone(new Rectangle(0, 0, ImageSource.Width, ImageSource.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            m_ImageDisplay = ImageSource.Clone(new Rectangle(0, 0, ImageSource.Width, ImageSource.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            tbPixelPermm.Text = 0.0175.ToString();
            ibSource.ShowPixelGrid = true;
        }

        private void FormPoint_Load(object sender, EventArgs e)
        {
            try
            {
                EventUpdateUi += UpdateUI;

                Mode = SettingMode.Point;

                ibSource.Image = m_ImageDisplay;                
            }
            catch (Exception Desc)
            {
               // CLog.Error( "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void UpdateUI(object sender, EventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {
                lbMode.Text = Mode.ToString();
            });            
        }

        private void ibSource_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                double dPixelPermm = double.Parse(tbPixelPermm.Text);
                LocationPoint = e.Location;

                GreyPoint = UpdateCursorPosition(e.Location);                
                xLocationToolStripStatusLabel.Text = string.Format("{0}", GreyPoint.X);
                yLocationToolStripStatusLabel.Text = string.Format("{0}", GreyPoint.Y);
                int nGreyValue = 0;

                if (GreyPoint.X + 100 < m_ImageOriginal.Width && GreyPoint.Y + 100 < m_ImageOriginal.Height)
                {
                    if (GreyPoint.X > 0 && GreyPoint.Y > 0)
                    {
                        Color color = m_ImageOriginal.GetPixel(GreyPoint.X, GreyPoint.Y);
                        nGreyValue = (color.R + color.G + color.B) / 3;
                        lbContrast.Text = string.Format("Grey Value : {0}", nGreyValue.ToString());
                    }
                }

                if (m_nMouseMode == MOUSEMODE_START && m_ImageOriginal != null)
                {
                    m_ptEnd = new Point(GreyPoint.X, GreyPoint.Y);
                }
                else if (m_nMouseMode == MOUSEMODE_END)
                {                    
                    OpenCvSharp.Point ptStart = new OpenCvSharp.Point(m_ptStart.X, m_ptStart.Y);
                    OpenCvSharp.Point ptEnd = new OpenCvSharp.Point(m_ptEnd.X, m_ptEnd.Y);
                    m_ptBase = new OpenCvSharp.Point(GreyPoint.X, GreyPoint.Y);
                    
                    OpenCvSharp.Point ptImageSize = new OpenCvSharp.Point(m_ImageOriginal.Width, m_ImageOriginal.Height);
                    CLineVertical.GetLineCoef(ptStart, ptEnd, m_ptBase, ptImageSize, out List<OpenCvSharp.Point> listPtVertical);

                    bool bInterSectionStart = CFormula.CrossCheck(ptStart, ptEnd, m_ptBase, listPtVertical[0]);
                    bool bInterSectionEnd = CFormula.CrossCheck(ptStart, ptEnd, m_ptBase, listPtVertical[listPtVertical.Count - 1]);
                    if (bInterSectionStart || bInterSectionEnd)
                    {
                        CLine line = new CLine(ptStart, ptEnd);
                        CLine lineVerical = new CLine();
                        if (bInterSectionStart)
                        {
                            lineVerical = new CLine(m_ptBase, listPtVertical[0]);
                        }
                        else
                        {
                            lineVerical = new CLine(m_ptBase, listPtVertical[listPtVertical.Count - 1]);
                        }

                        CFormula.FindIntersection(lineVerical, line, out m_ptVertical);

                        OpenCvSharp.Point ptBase = new OpenCvSharp.Point(m_ptBase.X, m_ptBase.Y);
                        OpenCvSharp.Point ptVertical = new OpenCvSharp.Point(m_ptVertical.X, m_ptVertical.Y);
                        CLine lineCalVertical = new CLine(ptBase, ptVertical);                        

                        lbVertical.Text = (lineCalVertical.Distance() * dPixelPermm).ToString("F4");
                    }
                    else
                    {
                        bool bInterSectionVertical = false;
                        for (int nIndex = 0; nIndex < listPtVertical.Count - 1; nIndex++)
                        {
                            bInterSectionVertical = CFormula.CrossCheck(ptStart, ptEnd, m_ptBase, listPtVertical[nIndex]);
                            if (bInterSectionVertical)
                            {
                                if (bInterSectionVertical)
                                {
                                    CLine line = new CLine(ptStart, ptEnd);
                                    CLine lineVerical = new CLine(m_ptBase, listPtVertical[nIndex]);

                                    CFormula.FindIntersection(lineVerical, line, out m_ptVertical);
                                }

                                OpenCvSharp.Point ptBase = new OpenCvSharp.Point(m_ptBase.X, m_ptBase.Y);
                                OpenCvSharp.Point ptVertical = new OpenCvSharp.Point(m_ptVertical.X, m_ptVertical.Y);

                                CLine lineCalVertical = new CLine(ptBase, ptVertical);

                                lbVertical.Text = (lineCalVertical.Distance() * dPixelPermm).ToString("F4");
                                break;
                            }
                        }
                    }

                    DrawRectangle();
                }


                //ILogger.Add(LOG_TYPE.SYSTEM, $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
              //  CLog.Error( "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void ibSource_MouseDown(object sender, MouseEventArgs e)
        {
            double dPixelPermm = double.Parse(tbPixelPermm.Text);

            if (Mode == SettingMode.Point)
            {
                if ((e.Button == MouseButtons.Left))
                {
                    if(m_bUseMouseDown)
                    {
                        System.Drawing.Point point = new System.Drawing.Point();
                        point = UpdateCursorPosition(e.Location);
                        
                        if (m_nMouseMode == MOUSEMODE_IDLE)
                        {
                            m_ptStart = new System.Drawing.Point(point.X, point.Y);
                            m_nMouseMode = MOUSEMODE_START;
                        }
                        else if (m_nMouseMode == MOUSEMODE_START)
                        {
                            m_ptEnd = new System.Drawing.Point(point.X, point.Y);
                            m_nMouseMode = MOUSEMODE_END;
                            
                            OpenCvSharp.Point ptStart = new OpenCvSharp.Point(m_ptStart.X, m_ptStart.Y);
                            OpenCvSharp.Point ptEnd = new OpenCvSharp.Point();

                            ptEnd = DrawLine(ptStart.X, ptStart.Y, m_ptEnd.X, m_ptEnd.Y);

                            CLine line = new CLine(ptStart, ptEnd);

                            lbVertical.Text = (line.Distance() * dPixelPermm).ToString("F4");
                        }
                        else if (m_nMouseMode == MOUSEMODE_END)
                        {
                            m_nMouseMode = MOUSEMODE_CREATEVERTICAL;
                            OpenCvSharp.Point ptStart = new OpenCvSharp.Point(m_ptStart.X, m_ptStart.Y);
                            OpenCvSharp.Point ptEnd = new OpenCvSharp.Point(m_ptEnd.X, m_ptEnd.Y);
                            m_ptBase = new OpenCvSharp.Point(point.X, point.Y);
                            OpenCvSharp.Point ptImageSize = new OpenCvSharp.Point(m_ImageOriginal.Width, m_ImageOriginal.Height);

                            CLineVertical.GetLineCoef(ptStart, ptEnd, m_ptBase, ptImageSize, out List<OpenCvSharp.Point> listPtVertical);

                            bool bInterSectionStart = CFormula.CrossCheck(ptStart, ptEnd, m_ptBase, listPtVertical[0]);
                            bool bInterSectionEnd = CFormula.CrossCheck(ptStart, ptEnd, m_ptBase, listPtVertical[listPtVertical.Count - 1]);
                            if (bInterSectionStart || bInterSectionEnd)
                            {
                                CLine line = new CLine(ptStart, ptEnd);
                                CLine lineVerical = new CLine();
                                if (bInterSectionStart)
                                {
                                    lineVerical = new CLine(m_ptBase, listPtVertical[0]);
                                }
                                else
                                {
                                    lineVerical = new CLine(m_ptBase, listPtVertical[listPtVertical.Count - 1]);
                                }

                                CFormula.FindIntersection(lineVerical, line, out m_ptVertical);
                                
                                OpenCvSharp.Point ptBase = new OpenCvSharp.Point(m_ptBase.X, m_ptBase.Y);
                                OpenCvSharp.Point ptVertical = new OpenCvSharp.Point(m_ptVertical.X, m_ptVertical.Y);
                                CLine lineCalVertical = new CLine(ptBase, ptVertical);

                                lbVertical.Text = (lineCalVertical.Distance() * dPixelPermm).ToString("F4");
                            }
                            else
                            {
                                bool bInterSectionVertical = false;
                                for (int nIndex = 0; nIndex < listPtVertical.Count - 1; nIndex++)
                                {
                                    bInterSectionVertical = CFormula.CrossCheck(ptStart, ptEnd, m_ptBase, listPtVertical[nIndex]);
                                    if (bInterSectionVertical)
                                    {
                                        if (bInterSectionVertical)
                                        {
                                            CLine line = new CLine(ptStart, ptEnd);
                                            CLine lineVerical = new CLine(m_ptBase, listPtVertical[nIndex]);

                                            CFormula.FindIntersection(lineVerical, line, out m_ptVertical);
                                        }

                                        OpenCvSharp.Point ptBase = new OpenCvSharp.Point(m_ptBase.X, m_ptBase.Y);
                                        OpenCvSharp.Point ptVertical = new OpenCvSharp.Point(m_ptVertical.X, m_ptVertical.Y);

                                        CLine lineCalVertical = new CLine(ptBase, ptVertical);

                                        lbVertical.Text = (lineCalVertical.Distance() * dPixelPermm).ToString("F4");
                                        break;
                                    }
                                }
                            }
                            m_nMouseMode = MOUSEMODE_CREATEVERTICAL;
                        }
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    m_ptStart = new Point();
                    m_ptEnd = new Point();
                    m_ptBase = new OpenCvSharp.Point();
                    m_ptVertical = new OpenCvSharp.Point();
                    m_nMouseMode = MOUSEMODE_IDLE;

                    m_ImageDisplay = (Bitmap)m_ImageOriginal.Clone();
                    ibSource.Image = m_ImageDisplay;
                }

                DrawRectangle();                
            }
        }

        private OpenCvSharp.Point DrawLine(int X0, int Y0, int X1, int Y1)
        {
            int dx = X1 - X0;
            int dy = Y1 - Y0;
            int steps = Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy);
            float Xinc = dx / (float)steps;
            float Yinc = dy / (float)steps;
            float X = X0;
            float Y = Y0;
            for (int i = 0; i <= steps; i++)
            {
                //cells.SetValue((int)Math.Round(Y), (int)Math.Round(X), Value);
                X += Xinc;
                Y += Yinc;
            }

            return new OpenCvSharp.Point(X, Y);
        }

        private void ibSource_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;// CreateGraphics();

            //SmoothingMode prevSmoothingMode = g.SmoothingMode;
            //g.SmoothingMode = (m_bHighQuality ? SmoothingMode.HighQuality
            //                                    : SmoothingMode.Default);

            //if (m_ImageOriginal != null)
            //{
            //    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //    //g.DrawImage(m_ImageOriginal, m_rtDisplay);
            //    this.Focus();
            //}
            
            //DrawLine(ref g);

            //g.ResetClip();
            //g.SmoothingMode = prevSmoothingMode;
        }

        private System.Drawing.Point UpdateCursorPosition(System.Drawing.Point location)
        {
            return ibSource.PointToImage(location);
        }

        protected string FormatPoint(System.Drawing.Point point)
        {
            return string.Format("X:{0}, Y:{1}", point.X, point.Y);
        }

        protected string FormatRectangle(RectangleF rect)
        {
            return string.Format("X:{0}, Y:{1}, W:{2}, H:{3}", (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath;
            ofd.Filter = "Images Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string strFilePath = ofd.FileName;

                using(Bitmap LoadImage =new Bitmap(strFilePath))
                {
                    ibSource.Image = (Bitmap)LoadImage.Clone();
                }
            }
        }

        private void ibSource_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void pblImage_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (Mode == SettingMode.Point)
                {
                    if (e.KeyCode == Keys.ControlKey)
                    {
                        m_bUseMouseDown = true;
                    }

                    if (e.KeyCode == Keys.Enter)
                    {
                        // ROI 영역
                        Rectangle rect = new Rectangle((int)ibSource.SelectionRegion.X, (int)ibSource.SelectionRegion.Y, (int)ibSource.SelectionRegion.Width, (int)ibSource.SelectionRegion.Height);
                        
                        //DrawRectangle(rect);

                       // Logger.WriteLog(LOG.Normal, "[Success] Success ROI");
                        CCommon.ShowMessageBox("[Success]!!", "Success ROI", FormMessageBox.MESSAGEBOX_TYPE.Info);
                    }
                }
            }
            catch (Exception Desc)
            {
              //  CLog.Error( "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }

        }

        private void ibSource_KeyUp(object sender, KeyEventArgs e)
        {
            if (Mode == SettingMode.Point)
            {
                m_bUseMouseDown = false;
            }
        }

        private void DrawRectangle()
        {
            try
            {
                if (m_ImageOriginal == null) return;

                m_ImageDisplay = (Bitmap)m_ImageOriginal.Clone();

                Font FontNotice = new Font("Arial", 100, FontStyle.Bold);
                SolidBrush BrushNotice = new SolidBrush(Color.Red);

                Bitmap bitmap = m_ImageDisplay;

                Graphics g = System.Drawing.Graphics.FromImage(bitmap);

                SmoothingMode prevSmoothingMode = g.SmoothingMode;
                g.SmoothingMode = (m_bHighQuality ? SmoothingMode.HighQuality
                                                    : SmoothingMode.Default);

                if (m_ImageOriginal != null)
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //g.DrawImage(m_ImageOriginal, m_rtDisplay);
                    this.Focus();
                }

                g.ResetClip();
                g.SmoothingMode = prevSmoothingMode;

                if (Mode == SettingMode.Point)
                {
                    if (m_nMouseMode == MOUSEMODE_START || m_nMouseMode == MOUSEMODE_END || m_nMouseMode == MOUSEMODE_CREATEVERTICAL)
                    {
                        Pen PenLine = new Pen(Color.Red, 1);
                        Pen PenVerticalLine = new Pen(Color.Green, 1);
                        Point ptBase = new Point(m_ptBase.X, m_ptBase.Y);
                        Point ptVertical = new Point(m_ptVertical.X, m_ptVertical.Y);

                        if (m_ptStart.X != 0 && m_ptStart.Y != 0
                             && m_ptEnd.X != 0 && m_ptEnd.Y != 0)
                        {
                            g.DrawLine(PenLine, m_ptStart, m_ptEnd);
                        }

                        if (ptBase.X != 0 && ptBase.Y != 0
                            && ptVertical.X != 0 && ptVertical.Y != 0)
                        {
                            g.DrawLine(PenVerticalLine, ptBase, ptVertical);
                        }

                        PenLine.Dispose();
                        PenVerticalLine.Dispose();
                    }

                    //g.
                    //(string.Format("Press Ctrl key to set the value."), FontNotice, BrushNotice, new PointF(0, 0));
                    //g.DrawString(string.Format("Press Enter to save the setting values."), FontNotice, BrushNotice, new PointF(0, 200));

                    ibSource.Image = bitmap;
                }

                FontNotice.Dispose();
                BrushNotice.Dispose();

                this.Refresh();
            }
            catch (Exception Desc)
            {
                //Logger.WriteLog(LOG.Normal, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                CCommon.ShowdialogMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void SaveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                Bitmap bmp1 = m_ImageDisplay;
                
                saveFileDialog.Title = "Save Gerber Project As";
                saveFileDialog.Filter = "BMP Image (.bmp)|*.bmp";
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    bmp1.Save(saveFileDialog.FileName);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_ImageDisplay.Dispose();
            m_ImageDisplay = null;
            m_ImageOriginal.Dispose();
            m_ImageOriginal = null;

            this.Close();
        }

        private void timerDisplay_Tick(object sender, EventArgs e)
        {
            ibSource.Refresh();

            Graphics g = ibSource.CreateGraphics();

            //g.Clear(Color.Transparent);

            System.Drawing.Point StartPoint = new System.Drawing.Point();
            System.Drawing.Point EndPoint = new System.Drawing.Point();

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawLine(Pens.Red,
                LocationPoint.X - 100, LocationPoint.Y, LocationPoint.X + 100, LocationPoint.Y);
            g.DrawLine(Pens.Red,
                LocationPoint.X, LocationPoint.Y - 100, LocationPoint.X, LocationPoint.Y + 100);
        }
        private void zooInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ibSource.ZoomIn();
        }

        private void zooOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ibSource.ZoomOut();
        }        
        private void fullToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            m_bFullScreen = !m_bFullScreen;
            if (m_bFullScreen)
            {
                if (this.WindowState == FormWindowState.Maximized)
                    this.WindowState = FormWindowState.Normal;

                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }

            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void ScaleToFitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ibSource.ZoomToFit(); 
        }

        private void ScaleToFullsizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int i =0; i < 10; i++)
            {
                ibSource.ZoomOut();
            }
        }

        private void ParameterClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_ptStart = new Point();
            m_ptEnd = new Point();
            m_ptBase = new OpenCvSharp.Point();
            m_ptVertical = new OpenCvSharp.Point();
            m_nMouseMode = MOUSEMODE_IDLE;

            m_ImageDisplay = (Bitmap)m_ImageOriginal.Clone();
            ibSource.Image = m_ImageDisplay;

            DrawRectangle();
        }

        private void ParameterSaveToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            // m_Points 레시피로 빼면됨
        }

        private void tbPixelPermm_Click(object sender, EventArgs e)
        {

        }
    }
}

