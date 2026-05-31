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

namespace KtemVisionSystem
{
    public partial class FormVision_Contour : MetroForm
    {
        private CPropertyContour Property_Contour = new CPropertyContour("Contour");

        private List<FormLayerDisplay> Displays { get; set; } = new List<FormLayerDisplay>();
        private int PanelCount = 0;

        private KtemViewer Source = new KtemViewer();
        private KtemViewer Destination = new KtemViewer();

        #region Event Register        
        public EventHandler<DockDisplayEventArgs> EventUpdateDisplay;
        #endregion

        public FormVision_Contour(List<FormLayerDisplay> Displays, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            this.Displays = Displays;
            this.EventUpdateDisplay = EventUpdateDisplay;
            PanelCount = Displays.Count;
            timer1.Enabled = true;
            timer1.Start();            
        }

        public FormVision_Contour()
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
            string strPath = Application.StartupPath + "\\TEST\\" + Property_Contour.NAME + ".xml";
            Property_Contour = Property_Contour.LoadTestConfig(strPath);
            propertyGrid_Parameter.SelectedObject = Property_Contour;

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

        private List<CResultContour> cResultContours = new List<CResultContour>();

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

                        CIVT_CVContour cIVT_CVContour = new CIVT_CVContour("Contour");
                        cIVT_CVContour.SetProperty(Property_Contour);
                        cIVT_CVContour.Property.USE_ROI = false;
                        cIVT_CVContour.SetSourceImage(ImageCVSource);
                        cIVT_CVContour.Run(Property_Contour.USE_DRAW_IMAGE);

                        if(!Property_Contour.USE_DRAW_IMAGE)
                        {
                            Graphics g = Graphics.FromImage(Result);
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                            int Count = 0;
                            foreach (var item in cIVT_CVContour.Results)
                            {
                                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                                g.DrawString((Count + 1).ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10);
                                g.DrawString(item.Area.ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X, (int)item.Center.Y);
                                Count++;
                            }

                            if (cIVT_CVContour.Results.Count == 0)
                            {
                                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(cIVT_CVContour.Property.CVROI));
                                g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)cIVT_CVContour.Property.CVROI.X - 20, (int)cIVT_CVContour.Property.CVROI.Y - 20);
                            }
                        }
                        else
                        {
                            Result = CConverter.ToBitmap(cIVT_CVContour.ImageResult);
                        }

                        cResultContours = cIVT_CVContour.Results;
                        stopwatch1.Stop();
                        //CLog.Error( "[Time] Blob {0} ", stopwatch1.ElapsedMilliseconds);
                    }
                    else
                    {
                        CIVT_CVContour cIVT_CVContour = new CIVT_CVContour("Contour");
                        cIVT_CVContour.SetProperty(Property_Contour);
                        cIVT_CVContour.Property.CVROI = CConverter.RectangleToRect(Displays[Source_Index].ImageView.ROI);
                        cIVT_CVContour.Property.USE_ROI = true;
                        cIVT_CVContour.SetSourceImage(ImageCVSource);
                        cIVT_CVContour.Run(Property_Contour.USE_DRAW_IMAGE);
                        if (!Property_Contour.USE_DRAW_IMAGE)
                        {
                            Graphics g = Graphics.FromImage(Result);
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                            int Count = 0;
                            foreach (var item in cIVT_CVContour.Results)
                            {
                                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                                g.DrawString((Count + 1).ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10);
                                g.DrawString(item.Area.ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X, (int)item.Center.Y);
                                Count++;
                            }

                            if (cIVT_CVContour.Results.Count == 0)
                            {
                                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(cIVT_CVContour.Property.CVROI));
                                g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)cIVT_CVContour.Property.CVROI.X - 20, (int)cIVT_CVContour.Property.CVROI.Y - 20);
                            }
                        }
                        else
                        {
                            Result = CConverter.ToBitmap(cIVT_CVContour.ImageResult);
                        }
                        
                        cResultContours = cIVT_CVContour.Results;
                    }

                    Displays[GetDisplayIndex(cbLayerList2.SelectedItem.ToString())].ImageView.ib.Image = Result;
                    ibDestination.Image = Result;
                    EventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }

                propertyGrid_Parameter.SelectedObject = Property_Contour;
                CUtil.InitDirectory("TEST");
                string strPath = Application.StartupPath + "\\TEST\\" + Property_Contour.NAME + ".xml";
                Property_Contour.SaveTestConfig(strPath);
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
            FormVision_Blob_Result formVision_Blob_Result = new FormVision_Blob_Result(cResultContours);
            formVision_Blob_Result.StartPosition = FormStartPosition.CenterScreen;

            if (!CUtil.OpenCheckForm(formVision_Blob_Result)) return;
            formVision_Blob_Result.Show();
        }
    }
 }

