using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using OpenCvSharp;
using OpenVisionLab._1._Core;
using RJCodeUI_M1.RJForms;
using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Property;
using Lib.OpenCV.Tool;

namespace OpenVisionLab
{
    public partial class FormVision_EdgeDetection : VisionTestForm
    {
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
            foreach (EdgeDetectionToolType edgeDetector in Enum.GetValues(typeof(EdgeDetectionToolType)))
            {
                cbEdgeType.Items.Add(edgeDetector);
            }
            cbEdgeType.SelectedIndex = 0;
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
            VisionPipelineFormBridge.AttachAddButton(
                btnFilterRun,
                () => VisionPipelineStepBuilder.FromEdgeDetectionProperty(
                    CreateEdgeDetectionProperty(),
                    "EdgeDetection",
                    Convert.ToString(cbLayerList.SelectedItem),
                    Convert.ToString(cbLayerList2.SelectedItem)));
        }

        public FormVision_EdgeDetection(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }
        private void btnFilterRun_Click(object sender, EventArgs e)
        {
            RunVisionStep("Edge Detection", () =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                EdgeDetectionToolProperty property = CreateEdgeDetectionProperty();

                Bitmap Result = CreateSingleInputResult(ibSource, image =>
                {
                    EdgeDetectionTool tool = new EdgeDetectionTool();
                    tool.SetProperty(property);
                    tool.SetSourceImage(image);
                    tool.Run();
                    tool.imageResult.CopyTo(image);
                });
                PublishResult(cbLayerList2, ibDestination, Result, FormatElapsed(stopwatch));
            });
        }

        private EdgeDetectionToolProperty CreateEdgeDetectionProperty()
        {
            if (tbThresholdHight.Text == "") { tbThresholdHight.Text = "200"; }
            if (tbThresholdLow.Text == "") { tbThresholdLow.Text = "100"; }
            if (tbSobelMask.Text == "") { tbSobelMask.Text = "3"; }

            return new EdgeDetectionToolProperty
            {
                EdgeType = AppUtil.ParseEnum<EdgeDetectionToolType>(cbEdgeType.SelectedItem.ToString()),
                CannyThresholdLow = int.Parse(tbThresholdLow.Text),
                CannyThresholdHigh = int.Parse(tbThresholdHight.Text),
                CannyApertureSize = int.Parse(tbSobelMask.Text),
                UseL2Gradient = chkUseL2.Check,
                SobelDegreeX = int.Parse(nudDegreeX.Value.ToString()),
                SobelDegreeY = int.Parse(nudDegreeY.Value.ToString()),
                SobelKernelSize = int.Parse(nudKernel.Value.ToString()),
                ScharrDegreeX = int.Parse(nudScharrDegreeX.Value.ToString()),
                ScharrDegreeY = int.Parse(nudScharrDegreeY.Value.ToString()),
                LaplacianKernelSize = int.Parse(nudLaplacianKernel.Value.ToString())
            };
        }

        private void cbFilterType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!(sender is RJCodeUI_M1.RJControls.RJComboBox)) return;
            string Index = (sender as System.Windows.Forms.ComboBox).Text;

            //tbKernalFilter.Enabled = false;
            //tbFilterW.Enabled = false;
            //tbFilterH.Enabled = false;
            //tbDiameter.Enabled = false;
            //tbSigmaColor.Enabled = false;
            //tbSigmaSpace.Enabled = false;

            switch (AppUtil.ParseEnum<EdgeDetectionToolType>(Index))
            {
                case EdgeDetectionToolType.Canny:
                    Tab.SelectedIndex = 0;
                    break;
                case EdgeDetectionToolType.Sobel:
                    Tab.SelectedIndex = 1;
                    break;
                case EdgeDetectionToolType.Scharr:
                    Tab.SelectedIndex = 2;
                    break;
                case EdgeDetectionToolType.Laplacian:
                    Tab.SelectedIndex = 3;
                    break;
            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}



