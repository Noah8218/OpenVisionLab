using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using Keys = System.Windows.Forms.Keys;
using OpenCvSharp;
using System.Diagnostics;
using System.Drawing.Imaging;
using OpenVisionLab._1._Core;
using RJCodeUI_M1.RJForms;
using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;

namespace OpenVisionLab
{
    public partial class FormVision_Matching : VisionTestForm
    {
        private CPropertyMatching Property_Matching = new CPropertyMatching("Matching");
        
        public FormVision_Matching(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
            panelCount = this.displayManager.LayerCount;
            timer1.Enabled = true;
            timer1.Start();
        }

        public FormVision_Matching() { }

        private void InitLayListItem()
        {
            cbLayerList.Items.Clear();
            for (int i = 0; i < displayManager.LayerCount; i++) { cbLayerList.Items.Add(displayManager.GetLayerTitle(i)); }
            cbLayerList.SelectedIndex = source1_Index;
            cbLayerList2.Items.Clear();
            for (int i = 0; i < displayManager.LayerCount; i++) { cbLayerList2.Items.Add(displayManager.GetLayerTitle(i)); }
            cbLayerList2.SelectedIndex = destination_Index;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            Property_Matching = (CPropertyMatching)Property_Matching.LoadTestConfig(Application.StartupPath + "\\TEST\\" + Property_Matching.NAME + ".xml");

            wpg.SelectedObject = Property_Matching;            
            pnParameter.Controls.Add(host);

            InitEvent();
            InitLayListItem();
            source_1.LoadImageBox(ibSource, false);
            destination.LoadImageBox(ibDestination, false);
            
            ibSource.Image = GetLayerImage(DEFINE.Main);
            ibDestination.Image = GetLayerImage(DEFINE.Main);
            ibSource.ImageChanged += IbSource_ImageChanged;
            ibDestination.ImageChanged += IbDestination_ImageChanged;
            ibDestination.MouseClick += IbDestination_MouseClick;
            ibSource.MouseClick += IbSource_MouseClick;
            ibSource.ZoomToFit();
            ibDestination.ZoomToFit();

            //toolTip1.SetToolTip(btnNewPanel_Source, "Create New Layer");
            toolTip1.SetToolTip(btnNewPanel_Desty, "Create New Layer");
        }

        private void Wpg_SelectedObjectsChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void IbDestination_MouseClick(object sender, MouseEventArgs e)
        {
            displayManager.ActivateLayer(destination_Index);
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbSource_MouseClick(object sender, MouseEventArgs e)
        {
            displayManager.ActivateLayer(source1_Index);
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbDestination_ImageChanged(object sender, EventArgs e)
        {
            destination_Index = cbLayerList2.SelectedIndex;
            SetLayerImage(destination_Index, (Bitmap)ibDestination.Image);
        }

        private void IbSource_ImageChanged(object sender, EventArgs e)
        {
            source1_Index = cbLayerList.SelectedIndex;
            SetLayerImage(source1_Index, (Bitmap)ibSource.Image);
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

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }

        private void btnNewPanel_Desty_Click(object sender, EventArgs e)
        {
            displayManager.CreatePanel();
            InitLayListItem();
            destination_Index = cbLayerList2.Items.Count - 1;
            cbLayerList2.SelectedIndex = destination_Index;
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
                Stopwatch stopwatch = Stopwatch.StartNew();
                
                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource.Image).Clone())
                {
                    Bitmap Result = CDrawBitmap.GetBitmapFormat24bppRgb((Bitmap)ibSource.Image);
                    COpenCVHelper.SetImageChannel1(ImageCVSource);
                    
                    CVMatching Matching = new CVMatching();
                    Matching.SetProperty(Property_Matching);
                    Matching.SetTemplateImage(Property_Matching.ImageTemplate);
                    Matching.SetSourceImage(ImageCVSource);
                    Matching.Run();

                    cResultMatchings = Matching.results;

                    using (Graphics g = Graphics.FromImage(Result))
                    {
                        foreach (var item in cResultMatchings)
                        {
                            RotateDraw(g, item, (float)item.Angle, new System.Drawing.Pen(System.Drawing.Color.Red, 1));
                        }

                        if (Matching.results.Count == 0)
                        {
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(Matching.property.CvROI));
                            g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)Matching.property.CvROI.X - 20, (int)Matching.property.CvROI.Y - 20);
                        }
                    }

                    SetLayerImage(GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), Result);
                    ibDestination.Image = Result;
                    eventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }
                Property_Matching.SaveTestConfig(Application.StartupPath + "\\TEST\\" + Property_Matching.NAME + ".xml");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public void RotateDraw(Graphics g, CResultMatching item, float angle, System.Drawing.Pen pen)
        {
            using (System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix())
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

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            source1_Index = cbLayerList.SelectedIndex;
            source_1.Roi = GetLayerRoi(source1_Index);
            ibSource.Image = GetLayerImage(source1_Index);

            displayManager.SetImageSrc(Lib.Common.CImageConverter.ToMat(GetLayerImage(source1_Index)));
        }

        private void cbLayerList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            destination_Index = cbLayerList2.SelectedIndex;
            destination.Roi = GetLayerRoi(destination_Index);
            ibDestination.Image = GetLayerImage(destination_Index);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (panelCount != displayManager.LayerCount)
                {
                    panelCount = displayManager.LayerCount;
                    InitLayListItem();
                }

                RefreshViewerRoi(source_1, ibSource, source1_Index);

                RefreshViewerRoi(destination, ibDestination, destination_Index);

                source1_Index = cbLayerList.SelectedIndex;
                source_1.Roi = GetLayerRoi(source1_Index);
                ibSource.Image = GetLayerImage(source1_Index);

                cbLayerList2_SelectedIndexChanged(null, null);
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
    }
}

