using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
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
        private MatchingProperty Property_Matching = new MatchingProperty("Matching");
        private void InitLayListItem()
        {
            InitializeSingleInputLayerList(cbLayerList, cbLayerList2);
        }

        private void IbDestination_MouseClick(object sender, MouseEventArgs e)
        {
            ActivateDestinationLayer();
        }

        private void IbSource_MouseClick(object sender, MouseEventArgs e)
        {
            ActivateSourceLayer();
        }

        private void IbDestination_ImageChanged(object sender, EventArgs e)
        {
            AcceptDestinationImageChange(cbLayerList2, ibDestination);
        }

        private void IbSource_ImageChanged(object sender, EventArgs e)
        {
            AcceptSourceImageChange(cbLayerList, ibSource);
        }

        private bool InitEvent()
        {
            return RegisterEscapeClose();
        }

        private void btnNewPanel_Desty_Click(object sender, EventArgs e)
        {
            CreateSingleInputDestinationLayer(cbLayerList2, InitLayListItem);
        }

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectSourceLayer(cbLayerList, ibSource, true);
        }

        private void cbLayerList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectDestinationLayer(cbLayerList2, ibDestination);
        }
        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            AppUtil.InitDirectory("TEST");
            Property_Matching = Property_Matching.LoadTestConfig(AppPathService.GetTestConfigPath(Property_Matching.NAME));
            AttachPropertyGridWithThresholdPreview(pnParameter, Property_Matching, cbLayerList2, ibSource, ibDestination);
            InitializeSingleInputViewers(
                InitLayListItem,
                ibSource,
                ibDestination,
                IbSource_ImageChanged,
                IbDestination_ImageChanged,
                IbSource_MouseClick,
                IbDestination_MouseClick,
                toolTip1,
                btnNewPanel_Desty);
            VisionPipelineFormBridge.AttachAddButton(btnRun, () => Property_Matching, cbLayerList, cbLayerList2);
        }
        
        public FormVision_Matching(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }

        public FormVision_Matching() { }
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

        private List<MatchingResult> cResultMatchings = new List<MatchingResult>();

        private void btnRun_Click(object sender, EventArgs e)
        {
            RunVisionStep("Matching", () =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                using (Mat ImageCVSource = CreateRunSourceMat(ibSource, out Bitmap Result))
                {
                    MatchingTool Matching = new MatchingTool();
                    Matching.SetProperty(Property_Matching);
                    Property_Matching.ReloadTemplateImage();
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
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CommonConverter.CVRectToRect(Matching.property.CvROI));
                            g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)Matching.property.CvROI.X - 20, (int)Matching.property.CvROI.Y - 20);
                        }
                    }
                    PublishResult(cbLayerList2, ibDestination, Result, FormatElapsed(stopwatch));
                }
                Property_Matching.SaveTestConfig(AppPathService.GetTestConfigPath(Property_Matching.NAME));
            });
        }

        public void RotateDraw(Graphics g, MatchingResult item, float angle, System.Drawing.Pen pen)
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
            }
}



