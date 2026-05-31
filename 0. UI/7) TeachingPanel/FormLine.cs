using KtemVisionSystem._1._Core;
using KtemVisionSystem._2._Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace KtemVisionSystem
{
    public partial class FormLine : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        public FormLine()
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
            dgvLine_L.DataSource = new CEdgeList_Line().GetBlobList(new List<CIVT_LineGuage_Result>());
            dgvLine_L.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLine_L.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dgvLine_R.DataSource = new CEdgeList_Line().GetBlobList(new List<CIVT_LineGuage_Result>());
            dgvLine_R.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLine_R.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
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

        private void btnLineTest_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = CDisplayManager.ImageSrc.Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap TempSrc = CConverter.ToBitmap(CDisplayManager.ImageSrc);
                    Bitmap Result = TempSrc.Clone(new Rectangle(0, 0, TempSrc.Width, TempSrc.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    //Bitmap Result = new Bitmap(10, 10);                    

                    int W = ImageCVSource.Width;
                    int H = ImageCVSource.Height;

                    Stopwatch stopwatch1 = Stopwatch.StartNew();

                    Graphics g = Graphics.FromImage(Result);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    List<Line> ver_Line_L = new List<Line>();
                    List<Line> ver_Line_R = new List<Line>();
                    List<Line> fit_Lines_L = new List<Line>();
                    List<Line> fit_Lines_R = new List<Line>();
                    CIVT_LineGuage LineGuage_EDGE = new CIVT_LineGuage();

                    if (rdoEdgeL.Checked) { LineGuage_EDGE.SetProperty(CVisionTools.Lines_L[CDisplayManager.CameraIndex]); }
                    else { LineGuage_EDGE.SetProperty(CVisionTools.Lines_R[CDisplayManager.CameraIndex]); }
                    LineGuage_EDGE.SetSourceImage(ImageCVSource);
                    LineGuage_EDGE.Run();

                    // 수직선 라인들(직선의 방정식으로 만들어진)

                    List<OpenCvSharp.Point> Edges = new List<OpenCvSharp.Point>();

                    if (rdoEdgeL.Checked)
                    {
                        LineGuage_EDGE.DrawFitLine(g, out double T, out fit_Lines_L, true);
                        LineGuage_EDGE.DrawEdge(g, out Edges);
                        LineGuage_EDGE.DrawVerticalLine(Edges, W, H, g, out ver_Line_L);

                        for (int j = 0; j < LineGuage_EDGE.Results.Count; j++)
                        {
                            LineGuage_EDGE.Results[j].MeasPos = new System.Drawing.Point(LineGuage_EDGE.Results[j].MeasPos.X + LineGuage_EDGE.Property.CVROI.X, LineGuage_EDGE.Results[j].MeasPos.Y + LineGuage_EDGE.Property.CVROI.Y);
                        }
                        dgvLine_L.DataSource = new CEdgeList_Line().GetBlobList(LineGuage_EDGE.Results);
                    }
                    else
                    {
                        LineGuage_EDGE.DrawFitLine(g, out double T, out fit_Lines_R, true);
                        LineGuage_EDGE.DrawEdge(g, out Edges);
                        LineGuage_EDGE.DrawVerticalLine(Edges, W, H, g, out ver_Line_R);

                        for (int j = 0; j < LineGuage_EDGE.Results.Count; j++)
                        {
                            LineGuage_EDGE.Results[j].MeasPos = new System.Drawing.Point(LineGuage_EDGE.Results[j].MeasPos.X + LineGuage_EDGE.Property.CVROI.X, LineGuage_EDGE.Results[j].MeasPos.Y + LineGuage_EDGE.Property.CVROI.Y);
                        }
                        dgvLine_R.DataSource = new CEdgeList_Line().GetBlobList(LineGuage_EDGE.Results);
                    }

                    CDisplayManager.TackTime = stopwatch.Elapsed.TotalSeconds.ToString() + "s";

                    CDisplayManager.CreateLayerDisplay(Result, "Edge", true);
                    stopwatch1.Stop();
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnIntersectionTest_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = CDisplayManager.ImageSrc.Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap TempSrc = CConverter.ToBitmap(CDisplayManager.ImageSrc);
                    Bitmap Result = TempSrc.Clone(new Rectangle(0, 0, TempSrc.Width, TempSrc.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    //Bitmap Result = new Bitmap(10, 10);                    

                    int W = ImageCVSource.Width;
                    int H = ImageCVSource.Height;

                    Stopwatch stopwatch1 = Stopwatch.StartNew();

                    Graphics g = Graphics.FromImage(Result);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    List<Line> ver_Line_L = new List<Line>();
                    List<Line> ver_Line_R = new List<Line>();
                    List<Line> fit_Lines_L = new List<Line>();
                    List<Line> fit_Lines_R = new List<Line>();
                    for (int i = 0; i < 2; i++)
                    {
                        CIVT_LineGuage LineGuage_EDGE = new CIVT_LineGuage();

                        if (i == 0) { LineGuage_EDGE.SetProperty(CVisionTools.Lines_L[CDisplayManager.CameraIndex]); }
                        else { LineGuage_EDGE.SetProperty(CVisionTools.Lines_R[CDisplayManager.CameraIndex]); }
                        LineGuage_EDGE.SetSourceImage(ImageCVSource);
                        LineGuage_EDGE.Run();

                        // 수직선 라인들(직선의 방정식으로 만들어진)

                        List<OpenCvSharp.Point> Edges = new List<OpenCvSharp.Point>();

                        if (i == 0)
                        {
                            LineGuage_EDGE.DrawFitLine(g, out double T, out fit_Lines_L, true);
                            LineGuage_EDGE.DrawEdge(g, out Edges);
                            LineGuage_EDGE.DrawVerticalLine(Edges, W, H, g, out ver_Line_L);

                            for (int j = 0; j < LineGuage_EDGE.Results.Count; j++)
                            {
                                LineGuage_EDGE.Results[j].MeasPos = new System.Drawing.Point(LineGuage_EDGE.Results[j].MeasPos.X + LineGuage_EDGE.Property.CVROI.X, LineGuage_EDGE.Results[j].MeasPos.Y + LineGuage_EDGE.Property.CVROI.Y);
                            }
                            dgvLine_L.DataSource = new CEdgeList_Line().GetBlobList(LineGuage_EDGE.Results);
                        }
                        else
                        {
                            LineGuage_EDGE.DrawFitLine(g, out double T, out fit_Lines_R, true);
                            LineGuage_EDGE.DrawEdge(g, out Edges);
                            LineGuage_EDGE.DrawVerticalLine(Edges, W, H, g, out ver_Line_R);

                            for (int j = 0; j < LineGuage_EDGE.Results.Count; j++)
                            {
                                LineGuage_EDGE.Results[j].MeasPos = new System.Drawing.Point(LineGuage_EDGE.Results[j].MeasPos.X + LineGuage_EDGE.Property.CVROI.X, LineGuage_EDGE.Results[j].MeasPos.Y + LineGuage_EDGE.Property.CVROI.Y);
                            }
                            dgvLine_R.DataSource = new CEdgeList_Line().GetBlobList(LineGuage_EDGE.Results);
                        }
                    }

                    try
                    {
                        for (int i = 0; i < ver_Line_L.Count; i++)
                        {
                            for (int j = 0; j < fit_Lines_R.Count; j++)
                            {
                                Line verLine = ver_Line_L[i];
                                Line fitLine = fit_Lines_R[j];

                                bool bInterSection = CVision.CrossCheck(verLine.Start, verLine.End, fitLine.Start, fitLine.End);
                                if (bInterSection)
                                {
                                    CVision.FindIntersection(fitLine, verLine, out OpenCvSharp.Point intersection);

                                    if (intersection.Y != 0)
                                    {

                                        //g.DrawString(Distance.ToString("F1")+"mm", new Font("Arial", 8, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), intersection.X + 5, intersection.Y);
                                        if (verLine.Start.X > 0 && verLine.Start.Y > 0)
                                        {
                                            Line lineCalVertical = new Line(verLine.Start, intersection);
                                            double Distance = lineCalVertical.Distance() * DEFINE.PIXEL_RESOLUTION_MM;
                                            g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Lime, 5), CConverter.CVPointToPoint(intersection), CConverter.CVPointToPoint(verLine.Start));
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CLogger.Add(LOG.EXCEPTION, "[FAILED] 수직선");
                        CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                    }
                    CDisplayManager.CreateLayerDisplay(Result, "Line", true);
                    stopwatch1.Stop();
                    CDisplayManager.TackTime = stopwatch.Elapsed.TotalSeconds.ToString() + "s";
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void dgvLineL_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;

                if (dgv.CurrentCell == null) { return; }

                int columnIndex = dgv.CurrentCell.ColumnIndex;
                int rowIndex = dgv.CurrentCell.RowIndex;

                if (dgv.Rows[rowIndex].Cells[1].Value == null) { return; }
                var P = new System.Drawing.Point(int.Parse(dgv.Rows[rowIndex].Cells[1].Value.ToString()),
                                                    int.Parse(dgv.Rows[rowIndex].Cells[2].Value.ToString()));


                int Index = CDisplayManager.FindIndex("Line");

                var pt = CDisplayManager.Displays[Index].ImageView.ib.PointToScreen(P);
                CDisplayManager.Displays[Index].ImageView.ib.ZoomToRegion(pt.X, pt.Y, 0, 100);
                CDisplayManager.Displays[Index].ImageView.ib.ScrollTo(pt.X - 750, pt.Y, 0, 1000);
                CDisplayManager.Displays[Index].Activate();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowMessageBox("EXCEPTION", $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {ex.Message}");
            }
        }
    }
}
