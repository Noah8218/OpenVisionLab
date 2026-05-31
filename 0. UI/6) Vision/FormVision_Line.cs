using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using MetroFramework.Forms;
using Keys = System.Windows.Forms.Keys;
using System.Xml.Serialization;
using OpenCvSharp;
using static OpenCvSharp.FileStorage;
using System.Diagnostics;
using MetroFramework.Drawing.Html;
using System.Xml.Linq;
using KtemVisionSystem._1._Core;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Linq;

namespace KtemVisionSystem
{
    public partial class FormVision_Line : MetroForm
    {
        private CPropertyLineGuage Property_Line_L = new CPropertyLineGuage("Line_L");
        private CPropertyLineGuage Property_Line_R = new CPropertyLineGuage("Line_R");

        private List<FormLayerDisplay> Displays { get; set; } = new List<FormLayerDisplay>();
        private int PanelCount = 0;

        private KtemViewer Source = new KtemViewer();
        private KtemViewer Destination = new KtemViewer();

        #region Event Register        
        public EventHandler<DockDisplayEventArgs> EventUpdateDisplay;
        #endregion

        public FormVision_Line(List<FormLayerDisplay> Displays, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            this.Displays = Displays;
            this.EventUpdateDisplay = EventUpdateDisplay;
            PanelCount = Displays.Count;
            timer1.Enabled = true;
            timer1.Start();            
        }

        public FormVision_Line()
        {

        }

        private void InitLayListItem()
        {
            cbLayerList.Items.Clear();
            for (int i = 0; i < Displays.Count; i++) { cbLayerList.Items.Add(Displays[i].Text); }
            cbLayerList.SelectedIndex = Source_Index;            
            cbLayerList2.Items.Clear();
            for (int i = 0; i < Displays.Count; i++) { cbLayerList2.Items.Add(Displays[i].Text); }
            cbLayerList2.SelectedIndex = Destination_Index;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            CUtil.InitDirectory("TEST");
            string strPath = Application.StartupPath + "\\TEST\\" + Property_Line_L.NAME + ".xml";
            Property_Line_L = Property_Line_L.LoadTestConfig(strPath);
            propertyGrid_Parameter.SelectedObject = Property_Line_L;

            strPath = Application.StartupPath + "\\TEST\\" + Property_Line_R.NAME + ".xml";
            Property_Line_R = Property_Line_R.LoadTestConfig(strPath);            

            InitEvent();
            InitLayListItem();
            Source.LoadImageBox(ibSource, false);
            Destination.LoadImageBox(ibDestination, false);

            ibSource.Image = (Bitmap)Displays[DEFINE.Main].ImageView.ib.Image;            
            ibDestination.Image = (Bitmap)Displays[DEFINE.Main].ImageView.ib.Image;
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
            Displays[Destination_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbSource_MouseClick(object sender, MouseEventArgs e)
        {
            Displays[Source_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbDestination_ImageChanged(object sender, EventArgs e)
        {
            Destination_Index = cbLayerList2.SelectedIndex;
            Displays[Destination_Index].ibSource.Image = (Bitmap)ibDestination.Image;
        }

        private void IbSource_ImageChanged(object sender, EventArgs e)
        {
            Source_Index = cbLayerList.SelectedIndex;
            Displays[Source_Index].ibSource.Image = (Bitmap)ibSource.Image;
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

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        private void btnNewPanel_Desty_Click(object sender, EventArgs e)
        {
            CDisplayManager.CreatePanel();
            InitLayListItem();
            Destination_Index = cbLayerList2.Items.Count - 1;
            cbLayerList2.SelectedIndex = Destination_Index;
        }

        private void btnNewPanel_Source_Click(object sender, EventArgs e)
        {            
            InitLayListItem();
        }

        public Font GetAdjustedFont(Graphics g, string graphicString, Font originalFont, int containerWidth, int maxFontSize, int minFontSize, bool smallestOnFail)
        {
            Font testFont = null;
            // We utilize MeasureString which we get via a control instance           
            for (int adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize--)
            {
                testFont = new Font(originalFont.Name, adjustedSize, originalFont.Style);

                // Test the string with the new size
                SizeF adjustedSizeNew = g.MeasureString(graphicString, testFont);

                if (containerWidth > Convert.ToInt32(adjustedSizeNew.Width))
                {
                    // Good font, return it
                    return testFont;
                }
            }

            // If you get here there was no fontsize that worked
            // return minimumSize or original?
            if (smallestOnFail)
            {
                return testFont;
            }
            else
            {
                return originalFont;
            }
        }

        private List<CResultBlob> cResultBlobs = new List<CResultBlob>();

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = CConverter.ToMat((Bitmap)ibSource.Image).Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap TempSrc = (Bitmap)ibSource.Image;
                    Bitmap Result = TempSrc.Clone(new Rectangle(0, 0, ibSource.Image.Width, ibSource.Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    //Bitmap Result = new Bitmap(10, 10);                    

                    int W = ImageCVSource.Width;
                    int H = ImageCVSource.Height;

                    if (Displays[Source_Index].ImageView.ROI.IsEmpty)
                    {
                        Stopwatch stopwatch1 = Stopwatch.StartNew();

                        Graphics g = Graphics.FromImage(Result);
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        List<Line> ver_Line_L = new List<Line>();
                        List<Line> ver_Line_R = new List<Line>();
                        List<Line> fit_Lines_L = new List<Line>();
                        List<Line> fit_Lines_R = new List<Line>();
                        List<OpenCvSharp.Point> Edges_L = new List<OpenCvSharp.Point>();
                        List<OpenCvSharp.Point> Edges_R = new List<OpenCvSharp.Point>();
                        for (int i = 0; i < 2; i++)
                        {
                            CIVT_LineGuage LineGuage_EDGE = new CIVT_LineGuage();

                            if (i == 0) { LineGuage_EDGE.SetProperty(Property_Line_L); }
                            else { LineGuage_EDGE.SetProperty(Property_Line_R); }
                            LineGuage_EDGE.SetSourceImage(ImageCVSource);
                            LineGuage_EDGE.Run();

                            string str = stopwatch.ElapsedMilliseconds.ToString();

                            // 수직선 라인들(직선의 방정식으로 만들어진)
                            if (i == 0)
                            {
                                LineGuage_EDGE.DrawFitLine(g, out double T, out fit_Lines_L, true);
                                LineGuage_EDGE.DrawEdge(g, out Edges_L);
                                LineGuage_EDGE.DrawVerticalLine(Edges_L, W, H, g, out ver_Line_L);
                            }
                            else
                            {
                                LineGuage_EDGE.DrawFitLine(g, out double T, out fit_Lines_R, true);
                                LineGuage_EDGE.DrawEdge(g, out Edges_R);
                                LineGuage_EDGE.DrawVerticalLine(Edges_R, W, H, g, out ver_Line_R);
                            }
                        }
                        List<PointF> Points_R = new List<PointF>();
                        for(int i = 0; i < Edges_R.Count; i++)
                        {
                            PointF pointF = new PointF();
                            Points_R.Add(pointF = new PointF(Edges_R[i].X, Edges_R[i].Y));
                        }
                        
                        for (int i = 0; i < ver_Line_L.Count; i++)
                        {
                            Line verLine = ver_Line_L[i];

                            bool draw = true;

                            PointF start = new PointF(ver_Line_L[i].Start.X, ver_Line_L[i].Start.Y);
                            PointF end = new PointF(ver_Line_L[i].End.X, ver_Line_L[i].End.Y);

                            PointF[] intersectionPointArray = CVision.GetClippingPointArray
                            (
                                out draw,
                                start,
                                end,
                                Points_R
                            );

                            using (Pen pen = new Pen(Color.Green, 5))
                            {
                                for (int h = 0; h < intersectionPointArray.Length - 1; h++)
                                {
                                    OpenCvSharp.Point ptS = new OpenCvSharp.Point(intersectionPointArray[h].X, intersectionPointArray[h].Y);
                                    OpenCvSharp.Point ptE = new OpenCvSharp.Point(intersectionPointArray[h + 1].X, intersectionPointArray[h + 1].Y);

                                    double Distance = ptS.DistanceTo(ptE) * DEFINE.PIXEL_RESOLUTION_MM;
                                    g.DrawString(Distance.ToString("F1")+"mm", new Font("Arial", 8, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), intersectionPointArray[h + 1].X + 5, intersectionPointArray[h + 1].Y);                             

                                    g.DrawLine(pen, intersectionPointArray[h], intersectionPointArray[h + 1]);
                                }

                                foreach (PointF point in intersectionPointArray)
                                {
                                    DrawPoint(g, point);
                                }
                            }
                        }                  
                        stopwatch1.Stop();
                    }                

                    Displays[GetDisplayIndex(cbLayerList2.SelectedItem.ToString())].ImageView.ib.Image = Result;
                    ibDestination.Image = Result;
                    EventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }


                CUtil.InitDirectory("TEST");
                string strPath = Application.StartupPath + "\\TEST\\" + Property_Line_L.NAME + ".xml";
                Property_Line_L.SaveTestConfig(strPath);

                CUtil.InitDirectory("TEST");
                strPath = Application.StartupPath + "\\TEST\\" + Property_Line_R.NAME + ".xml";
                Property_Line_R.SaveTestConfig(strPath);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void DrawPoint(Graphics graphics, PointF point)
        {
            RectangleF rectangle = new RectangleF(point.X - 3, point.Y - 3, 6, 6);

            graphics.FillEllipse(Brushes.White, rectangle);

            graphics.DrawEllipse(Pens.Black, rectangle);
        }

        private void OnPara_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                RJCodeUI_M1.RJControls.RJRadioButton rdoButton = (RJCodeUI_M1.RJControls.RJRadioButton)sender;

                if (rdoButton.Checked)
                {
                    switch (rdoButton.Text)
                    {
                        case "Edge(L)":
                            propertyGrid_Parameter.SelectedObject = Property_Line_L;
                            break;
                        case "Edge(R)":
                            propertyGrid_Parameter.SelectedObject = Property_Line_R;
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        private int GetDisplayIndex(string strTitle)
        {
            for (int i = 0; i < Displays.Count; i++)
            {
                if (Displays[i].Text == strTitle) { return i; }
            }

            return 0;
        }

        private int Source_Index = 0;
        private int Destination_Index = 0;

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Source_Index = cbLayerList.SelectedIndex;
            Source.ROI = Displays[Source_Index].ImageView.ROI;
            ibSource.Image = (Bitmap)Displays[Source_Index].ibSource.Image;

            CDisplayManager.ImageSrc = CConverter.ToMat((Bitmap)Displays[Source_Index].ibSource.Image);
        }

        private void cbLayerList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Destination_Index = cbLayerList2.SelectedIndex;
            Destination.ROI = Displays[Destination_Index].ImageView.ROI;
            ibDestination.Image = (Bitmap)Displays[Destination_Index].ibSource.Image;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Source_Index < 0) { return; }

                if(PanelCount != Displays.Count)
                {
                    PanelCount = Displays.Count;
                    InitLayListItem();
                }
                
                if (Source.ROI != Displays[Source_Index].ImageView.ROI || Source.TrainROI != Displays[Source_Index].ImageView.TrainROI)
                {
                    ibSource.Invalidate();
                }
                Source.ROI = Displays[Source_Index].ImageView.ROI;
                Source.TrainROI = Displays[Source_Index].ImageView.TrainROI;

                if (Destination.ROI != Displays[Destination_Index].ImageView.ROI || Destination.TrainROI != Displays[Destination_Index].ImageView.TrainROI)
                {
                    ibDestination.Invalidate();
                }
                Destination.ROI = Displays[Destination_Index].ImageView.ROI;
                Destination.TrainROI = Displays[Destination_Index].ImageView.TrainROI;

                Source_Index = cbLayerList.SelectedIndex;
                Source.ROI = Displays[Source_Index].ImageView.ROI;
                ibSource.Image = (Bitmap)Displays[Source_Index].ibSource.Image;

                cbLayerList2_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
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
            FormVision_Blob_Result formVision_Blob_Result = new FormVision_Blob_Result(cResultBlobs);
            formVision_Blob_Result.StartPosition = FormStartPosition.CenterScreen;

            if (!CUtil.OpenCheckForm(formVision_Blob_Result)) return;
            formVision_Blob_Result.Show();
        }
    }
 }

