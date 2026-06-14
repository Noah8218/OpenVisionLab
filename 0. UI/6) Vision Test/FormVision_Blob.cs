using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using OpenCvSharp;
using System.Diagnostics;
using OpenVisionLab._1._Core;
using RJCodeUI_M1.RJForms;
using Lib.Common;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Lib.OpenCV;
using Lib.OpenCV.Blob;

namespace OpenVisionLab
{
    public partial class FormVision_Blob : VisionTestForm
    {
        private BlobProperty Property_Blob = new BlobProperty("Blob");
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
            Property_Blob = Property_Blob.LoadTestConfig(AppPathService.GetTestConfigPath(Property_Blob.NAME));
            AttachPropertyGridWithThresholdPreview(pnParameter, Property_Blob, cbLayerList2, ibSource, ibDestination);
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
            VisionPipelineFormBridge.AttachAddButton(btnRun, () => Property_Blob, cbLayerList, cbLayerList2);
        }

        public FormVision_Blob(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }

        public FormVision_Blob() { }
        public void ToLowQuality(Graphics graphics)
        {
            graphics.InterpolationMode = InterpolationMode.Low;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        }

        private List<BlobResult> cResultBlobs = new List<BlobResult>();
        private void btnRun_Click(object sender, EventArgs e)
        {
            RunVisionStep("Blob", () =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                using (Mat ImageCVSource = CreateRunSourceMat(ibSource, out Bitmap Result))
                {
                    BlobTool CvBlob = new BlobTool();
                    CvBlob.SetProperty(Property_Blob);
                    CvBlob.SetSourceImage(ImageCVSource);
                    CvBlob.Run();

                    using (Graphics g = Graphics.FromImage(Result))
                    {
                        foreach (var item in CvBlob.results)
                        {
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                            //  g.DrawString((Count + 1).ToString(), new Font("Arial", 1, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10);
                            // g.DrawString(item.Area.ToString(), new Font("Arial", 1, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X, (int)item.Center.Y);
                        }

                        if (CvBlob.results.Count == 0)
                        {
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CommonConverter.CVRectToRect(CvBlob.property.CvROI));
                            g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)CvBlob.property.CvROI.X - 20, (int)CvBlob.property.CvROI.Y - 20);
                        }
                    }

                    cResultBlobs = CvBlob.results;
                    PublishResult(cbLayerList2, ibDestination, Result, FormatElapsed(stopwatch));
                }

                wpg.SelectedObject = Property_Blob;

                AppUtil.InitDirectory("TEST");
                Property_Blob.SaveTestConfig(AppPathService.GetTestConfigPath(Property_Blob.NAME));
            });
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnResult_Click(object sender, EventArgs e)
        {
            FormVision_Result resultForm = new FormVision_Result(cResultBlobs);
            resultForm.StartPosition = FormStartPosition.CenterScreen;

            if (!AppUtil.OpenCheckForm(resultForm)) return;
            resultForm.Show();
        }
    }
}



