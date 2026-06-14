using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using OpenCvSharp;
using Keys = System.Windows.Forms.Keys;
using OpenVisionLab._1._Core;
using RJCodeUI_M1.RJForms;
using Lib.OpenCV;
using Lib.Common;
using Lib.OpenCV.Tool;

namespace OpenVisionLab
{
    public partial class FormVision_Mean : VisionTestForm
    {        
        private MeanProperty cPropertyMean = new MeanProperty();
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
            cPropertyMean = cPropertyMean.LoadTestConfig(AppPathService.GetTestConfigPath(cPropertyMean.NAME));
            AttachPropertyGridWithThresholdPreview(pnParameter, cPropertyMean, cbLayerList2, ibSource, ibDestination);
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
            VisionPipelineFormBridge.AttachAddButton(btnRun, () => cPropertyMean, cbLayerList, cbLayerList2);
        }

        public FormVision_Mean(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            RunVisionStep("Mean", () =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                using (Mat ImageCVSource = CreateRunSourceMat(ibSource, out Bitmap Result))
                {

                    MeanTool CvMean = new MeanTool();
                    CvMean.SetProperty(cPropertyMean);
                    CvMean.SetSourceImage(ImageCVSource);
                    CvMean.Run();

                    using (Graphics g = Graphics.FromImage(Result))
                    {
                        foreach (var item in CvMean.results)
                        {
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                            g.DrawString($"Mean : {item.meanValue}", new Font("Arial", 3, FontStyle.Regular), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X - 20, (int)item.Center.Y - 20);
                        }
                    }

                    PublishResult(cbLayerList2, ibDestination, Result, FormatElapsed(stopwatch));
                }

                wpg.SelectedObject = cPropertyMean;
                cPropertyMean.SaveTestConfig(AppPathService.GetTestConfigPath(cPropertyMean.NAME));
            });
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}


