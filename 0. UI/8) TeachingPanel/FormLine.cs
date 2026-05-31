using ADOX;
using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using Lib.Common;
using Lib.OpenCV.Result;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;

namespace OpenVisionLab
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
            dgvLine_L.DataSource = new CEdgeList_Line().GetEdgeList(new List<CVLineGuage_Edge>());
            dgvLine_L.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLine_L.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dgvLine_R.DataSource = new CEdgeList_Line().GetEdgeList(new List<CVLineGuage_Edge>());
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

                    Bitmap TempSrc = Lib.Common.CImageConverter.ToBitmap(CDisplayManager.ImageSrc);
                    Bitmap Result = TempSrc.Clone(new Rectangle(0, 0, TempSrc.Width, TempSrc.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    Stopwatch stopwatch1 = Stopwatch.StartNew();

                    Graphics g = Graphics.FromImage(Result);
                    CDrawBitmap.ToLowQuality(g);
                    //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;                                        

                    List<CVLineGuage_Result> Edges = new List<CVLineGuage_Result>();

                    if (rdoEdgeL.Checked) { Edges = CInspectionAlgorithm.RunEdge(g, ImageCVSource, CVisionTools.Lines_L[CDisplayManager.CameraIndex], new System.Drawing.Size(TempSrc.Width, TempSrc.Height), true); }
                    else if (rdoEdgeR.Checked) { Edges =  CInspectionAlgorithm.RunEdge(g, ImageCVSource, CVisionTools.Lines_R[CDisplayManager.CameraIndex], new System.Drawing.Size(TempSrc.Width, TempSrc.Height), true); }
                    else { Edges = CInspectionAlgorithm.RunEdge(g, ImageCVSource, CVisionTools.Lines_TOP[CDisplayManager.CameraIndex], new System.Drawing.Size(TempSrc.Width, TempSrc.Height), true); }
                    if (rdoEdgeL.Checked) { dgvLine_L.DataSource = new CEdgeList_Line().GetEdgeList(Edges[0].edgeList); }                    
                    else if (rdoEdgeR.Checked) { dgvLine_R.DataSource = new CEdgeList_Line().GetEdgeList(Edges[0].edgeList); }
                    else
                    {                       

                        dgvLine_Top.DataSource = new CEdgeList_Line().GetEdgeList(Edges[0].edgeList); 
                    }
                    CDisplayManager.TackTime = stopwatch.Elapsed.TotalSeconds.ToString() + "s";

                    CDisplayManager.CreateLayerDisplay(Result, "Edge", true);
                    stopwatch1.Stop();
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
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

                    Bitmap TempSrc = Lib.Common.CImageConverter.ToBitmap(CDisplayManager.ImageSrc);
                    Bitmap Result = TempSrc.Clone(new Rectangle(0, 0, TempSrc.Width, TempSrc.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    int W = ImageCVSource.Width;
                    int H = ImageCVSource.Height;

                    Stopwatch stopwatch1 = Stopwatch.StartNew();

                    Graphics g = Graphics.FromImage(Result);
                    CDrawBitmap.ToLowQuality(g);
                    //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    List<CVLineGuage_Result> intersectionLin1 = new List<CVLineGuage_Result>();
                    List<CVLineGuage_Result> intersectionLin2 = new List<CVLineGuage_Result>();
                    //(intersectionLin1, intersectionLin2) = CInspectionAlgorithm.RunIntersectionLines(ImageCVSource, CVisionTools.Lines_L[CDisplayManager.CameraIndex], CVisionTools.Lines_R[CDisplayManager.CameraIndex], true);
                    //CInspectionAlgorithm.DrawInstersectionLines(g, CVisionTools.Lines_L[CDisplayManager.CameraIndex], CVisionTools.Lines_R[CDisplayManager.CameraIndex],
                    //intersectionLin1, intersectionLin2, new System.Drawing.Size(ImageCVSource.Width, ImageCVSource.Height), true, false);

                    stopwatch1.Stop();
                    CDisplayManager.TackTime = stopwatch.Elapsed.TotalSeconds.ToString() + "s";

                    dgvLine_L.DataSource = new CEdgeList_Line().GetEdgeList(intersectionLin1);
                    dgvLine_R.DataSource = new CEdgeList_Line().GetEdgeList(intersectionLin2);

                    CDisplayManager.CreateLayerDisplay(Result, "Line", true);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
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

                var pt = CDisplayManager.Displays[Index].viewer._Ib.PointToScreen(P);
                CDisplayManager.Displays[Index].viewer._Ib.ZoomToRegion(pt.X, pt.Y, 0, 100);
                CDisplayManager.Displays[Index].viewer._Ib.ScrollTo(pt.X - 750, pt.Y, 0, 1000);
                CDisplayManager.Displays[Index].Activate();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowMessageBox("EXCEPTION", $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void rjLabel6_Click(object sender, EventArgs e)
        {

        }

        private void dgvLine_R_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvLine_L_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void rdoEdgeR_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoEdgeL_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjLabel16_Click(object sender, EventArgs e)
        {

        }
    }
}
