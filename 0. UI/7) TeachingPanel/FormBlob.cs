using KtemVisionSystem._1._Core;
using KtemVisionSystem._2._Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;
using System.Windows.Forms;
using static KtemVisionSystem.CPropertySocket;

namespace KtemVisionSystem
{
    public partial class FormBlob : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        public FormBlob()
        {           
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }
     
        // If the content won't display nicely, hide it
        private void ResizeEvent(object sender, EventArgs e)
        {
            this.Visible = this.Width > this.MinimumSize.Width && this.Height > this.MinimumSize.Height;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            dgvDefect.DataSource = new CDefectList_Blob().GetBlobList(new List<CResultBlob>());
            dgvDefect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDefect.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;            
        }

        private void dgvSeletecList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;

                if (dgv.CurrentCell == null) { return; }

                int columnIndex = dgv.CurrentCell.ColumnIndex;
                int rowIndex = dgv.CurrentCell.RowIndex;

                if (dgv.Rows[rowIndex].Cells[1].Value == null) { return; }
                var rect = new Rectangle(int.Parse(dgv.Rows[rowIndex].Cells[5].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[6].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[7].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[8].Value.ToString()));


                int Index = CDisplayManager.FindIndex("Defect");

                double Factor_W = 0.95;
                double Factor_H = 2;

                int W = (int)(CDisplayManager.Displays[Index].ImageView.ib.Width / Factor_W);
                int H = (int)(CDisplayManager.Displays[Index].ImageView.ib.Height / Factor_H);

                Rectangle rt = CDisplayManager.Displays[Index].ImageView.ib.RectangleToScreen(rect);
                CDisplayManager.Displays[Index].ImageView.ib.ZoomToRegion(rt.X, rt.Y, (int)(W), (int)(H));
                CDisplayManager.Displays[Index].ImageView.ib.ScrollTo(rt.X, rt.Y, W, H);
                CDisplayManager.Displays[Index].Activate();

                //trbThreshold_Scroll(null, null);

                //Rectangle rt2 = CDisplayManager.Displays[DEFINE.nThreshold].ImageView.ib.RectangleToScreen(rect);
                //CDisplayManager.Displays[DEFINE.nThreshold].ImageView.ib.ZoomToRegion(rt.X, rt.Y, 500, 500);
                //CDisplayManager.Displays[DEFINE.nThreshold].ImageView.ib.ScrollTo(rt.X, rt.Y, 400, 400);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowMessageBox("EXCEPTION", $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {ex.Message}");
            }
        }

        private void btnBlobRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();                
                using (Mat ImageCVSource = CDisplayManager.ImageSrc.Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap TempSrc = CConverter.ToBitmap(ImageCVSource);
                    Bitmap Result = TempSrc.Clone(new Rectangle(0, 0, TempSrc.Width, TempSrc.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Stopwatch stopwatch1 = Stopwatch.StartNew();

                    CIVT_CVBlob cIVT_CVBlob = new CIVT_CVBlob("BLOB");
                    cIVT_CVBlob.SetProperty(CVisionTools.Blobs[CDisplayManager.CameraIndex]);
                    cIVT_CVBlob.SetSourceImage(ImageCVSource);
                    cIVT_CVBlob.Run();

                    Graphics g = Graphics.FromImage(Result);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    int Count = 0;
                    foreach (var item in cIVT_CVBlob.Results)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                        g.DrawString((Count + 1).ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10);
                        g.DrawString(item.Area.ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X, (int)item.Center.Y);
                        Count++;
                    }

                    if (cIVT_CVBlob.Results.Count == 0)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(cIVT_CVBlob.Property.CVROI));
                        g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)cIVT_CVBlob.Property.CVROI.X - 20, (int)cIVT_CVBlob.Property.CVROI.Y - 20);
                    }

                    dgvDefect.DataSource = new CDefectList_Blob().GetBlobList(cIVT_CVBlob.Results);
                    dgvDefect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    stopwatch1.Stop();

                    fpnImageList.Controls.Clear();
                    for (int i = 0; i < cIVT_CVBlob.Results.Count;i++)
                    {
                        Rectangle ResizeRect = cIVT_CVBlob.Results[i].Bounding;
                        int OffsetSize = 20;  
                        ResizeRect = new Rectangle(ResizeRect.X - OffsetSize, ResizeRect.Y - OffsetSize, ResizeRect.Width + (OffsetSize * 2), ResizeRect.Height + (OffsetSize * 2));
                        
                        PictureBox pb = new PictureBox();
                        pb.SizeMode = PictureBoxSizeMode.StretchImage;
                        pb.Tag = "Index : "+(i+1);
                        pb.Image = CConverter.cropAtRect(Result, ResizeRect);                        
                        pb.Width = (int)(fpnImageList.Width / 2.5F);
                        pb.Height = (int)(fpnImageList.Width / 2.5F);
                        pb.MouseHover += Pb_MouseHover;
                        pb.MouseMove += new MouseEventHandler(OnImageMouseMove);
                        pb.MouseLeave += new EventHandler(OnImageMouseLeave);

                        string strFileName = pb.Tag.ToString();

                        Graphics g2 = Graphics.FromImage(pb.Image);

                        Rectangle rtFullScreen = new Rectangle(0, 0, pb.Image.Width, pb.Image.Height);

                        g2.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.DarkGreen, 30), rtFullScreen);

                        g2.TextRenderingHint = TextRenderingHint.AntiAlias;
                        g2.DrawString(strFileName, new Font("Arial", 8, FontStyle.Bold), new SolidBrush(Color.White), 0, 0);
                        g2.DrawString("이물", new Font("Arial", 8, FontStyle.Bold), new SolidBrush(Color.White), 0, rtFullScreen.Height - 15);

                        fpnImageList.Controls.Add(pb);
                    }
                    CDisplayManager.TackTime = stopwatch.Elapsed.TotalSeconds.ToString() + "s";
                    CDisplayManager.CreateLayerDisplay(Result, "Defect", true);
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void Pb_MouseHover(object sender, EventArgs e)
        {
            try
            {
                PictureBox pb = (PictureBox)sender;

                pb.Width = (int)(fpnImageList.Width / 2.3F);
                pb.Height = (int)(fpnImageList.Width / 2.3F);

                pb.Refresh();

                //timerDisplay.Start();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnImageMouseMove(object sender, EventArgs e)
        {
            try
            {
                //timerDisplay.Stop();

                PictureBox pb = (PictureBox)sender;
                Graphics g2 = pb.CreateGraphics();                
            }
            catch (Exception Desc)
            {
               // ILogger.Add(LOG_TYPE.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void OnImageMouseLeave(object sender, EventArgs e)
        {
            try
            {
                PictureBox pb = (PictureBox)sender;

                pb.Width = (int)(fpnImageList.Width / 2.5F);
                pb.Height = (int)(fpnImageList.Width / 2.5F);

                pb.Refresh();

                //timerDisplay.Start();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        private bool ChangeSize = false;

        private void Form_VisibleChanged(object sender, EventArgs e)
        {
            if (!ChangeSize)
            {
                if (DockHandler.FloatPane == null) { return; }
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
                this.Refresh();
                ChangeSize = true;
            }
        }
    }
}
