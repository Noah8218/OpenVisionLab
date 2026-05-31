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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Vila.Win32;
using KtemVisionSystem._1._Core;

namespace KtemVisionSystem
{
    public partial class FormVision_Matching : MetroForm
    {
        private CPropertyMatching Property_Matching = new CPropertyMatching("Matching");

        private List<FormLayerDisplay> Displays { get; set; } = new List<FormLayerDisplay>();
        private int PanelCount = 0;

        private KtemViewer Source = new KtemViewer();
        private KtemViewer Destination = new KtemViewer();
        private KtemViewer Train = new KtemViewer();

        #region Event Register        
        public EventHandler<DockDisplayEventArgs> EventUpdateDisplay;
        #endregion

        public FormVision_Matching(List<FormLayerDisplay> Displays, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            this.Displays = Displays;
            this.EventUpdateDisplay = EventUpdateDisplay;
            PanelCount = Displays.Count;
            timer1.Enabled = true;
            timer1.Start();            
        }

        public FormVision_Matching() { }

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
            string strPath = Application.StartupPath + "\\TEST\\" + Property_Matching.NAME + ".xml";
            Property_Matching = Property_Matching.LoadTestConfig(strPath);
            propertyGrid_Parameter.SelectedObject = Property_Matching;

            InitEvent();
            InitLayListItem();
            Source.LoadImageBox(ibSource, false);
            Destination.LoadImageBox(ibDestination, false);
            Train.LoadImageBox(ibTrainImage, false);

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

        private List<CResultMatching> cResultMatchings = new List<CResultMatching>();

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
                    if (Displays[Source_Index].ImageView.ROI.IsEmpty)
                    {
                        Stopwatch stopwatch1 = Stopwatch.StartNew();

                        Property_Matching.TrainROI = new Rect();
                        CIVT_CVMatching Matching = new CIVT_CVMatching("T");
                        Matching.SetProperty(Property_Matching);
                        Matching.SetTemplateImage(CConverter.ToMat((Bitmap)ibTrainImage.Image));
                        Matching.SetSourceImage(ImageCVSource);
                        Matching.ImagePyramidsRun(true);

                        cResultMatchings = Matching.Results;
                        //cResultMatchings = Matching.Results.ConvertAll(s => s);

                        Graphics g = Graphics.FromImage(Result);
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        int Count = 0;

                        foreach (var item in cResultMatchings)
                        {
                            RotateDraw(g, item, (float)item.Angle, new System.Drawing.Pen(System.Drawing.Color.Red, 1));
                        }
                        
                        //foreach (var item in cResultMatchings)
                        //{
                        //    g.DrawRectangles(new System.Drawing.Pen(System.Drawing.Color.Blue, 5), new[] { item.Bounding });
                        //    g.DrawString((Count + 1).ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10);
                        //    g.DrawString(item.Score.ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X, (int)item.Center.Y);
                        //    Count++;
                        //}

                        if (Matching.Results.Count == 0)
                        {
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(Matching.Property.TrainROI));
                            g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)Matching.Property.TrainROI.X - 20, (int)Matching.Property.TrainROI.Y - 20);
                        }                        
                        stopwatch1.Stop();                        
                    }
                    else
                    {
                        Property_Matching.TrainROI = CConverter.RectangleToRect(Displays[Source_Index].ImageView.ROI);
                        CIVT_CVMatching Matching = new CIVT_CVMatching("T");
                        Matching.SetProperty(Property_Matching);                        
                        Matching.SetTemplateImage(CConverter.ToMat((Bitmap)ibTrainImage.Image));
                        Matching.SetSourceImage(ImageCVSource);
                        Matching.ImagePyramidsRun(true);

                        Graphics g = Graphics.FromImage(Result);
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        cResultMatchings = Matching.Results;

                        foreach (var item in cResultMatchings)
                        {
                            RotateDraw(g, item, (float)item.Angle, new System.Drawing.Pen(System.Drawing.Color.Red, 1));
                        }

                        //int Count = 0;
                        //foreach (var item in cResultMatchings)
                        //{
                        //    g.DrawRectangles(new System.Drawing.Pen(System.Drawing.Color.Blue, 5), new[] { item.Bounding });                            
                        //    g.DrawString((Count + 1).ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10);
                        //    g.DrawString(item.Score.ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X, (int)item.Center.Y);
                        //    Count++;
                        //}

                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 5), CConverter.CVRectToRect(Matching.Property.TrainROI));

                        if (Matching.Results.Count == 0)
                        {
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(Matching.Property.TrainROI));
                            g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)Matching.Property.TrainROI.X - 20, (int)Matching.Property.TrainROI.Y - 20);
                        }
                        
                        //cResultMatchings = Matching.Results.ConvertAll(s => s);
                    }

                    Displays[GetDisplayIndex(cbLayerList2.SelectedItem.ToString())].ImageView.ib.Image = Result;
                    ibDestination.Image = Result;
                    EventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }


                propertyGrid_Parameter.SelectedObject = Property_Matching;
                CUtil.InitDirectory("TEST");
                string strPath = Application.StartupPath + "\\TEST\\" + Property_Matching.NAME + ".xml";
                Property_Matching.SaveTestConfig(strPath);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void RotateDraw(Graphics g, CResultMatching item, float angle, System.Drawing.Pen pen)
        {
            using (Matrix m = new Matrix())
            {
                RectangleF r = item.Bounding;
                m.RotateAt(angle, new PointF(r.Left + (r.Width / 2), r.Top + (r.Height / 2)));
                g.Transform = m;
                g.DrawRectangles(pen, new[] { r });                
                g.DrawLine(pen, r.X + r.Width / 2, r.Y, r.X + r.Width / 2, r.Y + r.Height);
                g.DrawLine(pen, r.X, r.Y + r.Height / 2, r.X + r.Width, r.Y + r.Height / 2);
                g.DrawString((item.Index + 1).ToString(), new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 20, (int)item.Bounding.Y - 20);
                g.DrawString(item.Score.ToString("F3"), new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Center.X, (int)item.Center.Y);
                g.ResetTransform();
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
                if(PanelCount != Displays.Count)
                {
                    PanelCount = Displays.Count;
                    InitLayListItem();
                }
                
                if (Source.ROI != Displays[Source_Index].ImageView.ROI ||
                    Source.TrainROI != Displays[Source_Index].ImageView.TrainROI)
                {
                    ibSource.Invalidate();
                    if(chkUseAutoTrain.Check)
                    {
                        string Path = Application.StartupPath + $"\\TEST\\TEST.bmp";

                        Rectangle r = Displays[Source_Index].ImageView.TrainROI;

                        //int PaddingW = (int)(r.Width * 0.01);
                        //int PaddingH = (int)(r.Height * 0.3);

                        //r.X = r.X - PaddingW;
                        //r.Y = r.Y - PaddingH;
                        //r.Width = r.Width + (PaddingW * 2);
                        //r.Height = r.Height+ (PaddingH * 2);

                        Bitmap ImageTemplate =  CConverter.cropAtRect((Bitmap)ibSource.Image, r);
                        
                        Property_Matching.PATTERN_PATH = Path;
                        string strPath = Application.StartupPath + "\\TEST\\" + Property_Matching.NAME + ".xml";
                        Property_Matching.SaveTestConfig(strPath);

                        ImageTemplate.Save(Path, ImageFormat.Bmp);
                        ibTrainImage.Image = ImageTemplate;
                    }

                    if(chkAutoRun.Check)
                    {
                        btnRun_Click(null, null);
                    }
                }                
                Source.ROI = Displays[Source_Index].ImageView.ROI;
                Source.TrainROI = Displays[Source_Index].ImageView.TrainROI;

                if (Destination.ROI != Displays[Destination_Index].ImageView.ROI ||
                    Destination.TrainROI != Displays[Destination_Index].ImageView.TrainROI) { ibDestination.Invalidate(); }                
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

        private void btnExit_Click(object sender, EventArgs e) => this.Close();        

        private void btnMinimizar_Click(object sender, EventArgs e)  => this.WindowState = FormWindowState.Minimized;

        private void rjButton1_Click(object sender, EventArgs e)
        {
            Rectangle r = Displays[Source_Index].ImageView.TrainROI;            
            Bitmap ImageTemplate = CConverter.cropAtRect((Bitmap)ibSource.Image, r);
            ibTrainImage.Image = ImageTemplate;
        }
    }
 }

