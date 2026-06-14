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
    public partial class FormVision_Filter : VisionTestForm
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
            foreach (FilterToolType filterType in Enum.GetValues(typeof(FilterToolType)))
            {
                cbFilterType.Items.Add(filterType);
            }
            cbFilterType.SelectedIndex = 0;
            InitFilterBorderMenu();
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
                () => VisionPipelineStepBuilder.FromFilterProperty(
                    CreateFilterProperty(),
                    "Filter",
                    Convert.ToString(cbLayerList.SelectedItem),
                    Convert.ToString(cbLayerList2.SelectedItem)));
        }

        public FormVision_Filter(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }
        public void InitFilterBorderMenu()
        {
            cbFilterBorderType.Items.Add(BorderTypes.Reflect101);
            cbFilterBorderType.Items.Add(BorderTypes.Replicate);
            cbFilterBorderType.Items.Add(BorderTypes.Reflect);
            cbFilterBorderType.Items.Add(BorderTypes.Wrap);
            cbFilterBorderType.Items.Add(BorderTypes.Constant);
            cbFilterBorderType.Items.Add(BorderTypes.Transparent);
            cbFilterBorderType.Items.Add(BorderTypes.Isolated);
            cbFilterBorderType.SelectedIndex = 0;
        }
        private void btnFilterRun_Click(object sender, EventArgs e)
        {
            RunVisionStep("Filter", () =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                FilterToolProperty property = CreateFilterProperty();

                Bitmap Result = CreateSingleInputResult(ibSource, image =>
                {
                    FilterTool tool = new FilterTool();
                    tool.SetProperty(property);
                    tool.SetSourceImage(image);
                    tool.Run();
                    tool.imageResult.CopyTo(image);
                });
                PublishResult(cbLayerList2, ibDestination, Result, FormatElapsed(stopwatch));
            });
        }

        private FilterToolProperty CreateFilterProperty()
        {
            if (tbFilterW.Text == "") { tbFilterW.Text = "3"; }
            if (tbFilterH.Text == "") { tbFilterH.Text = "3"; }
            if (tbKernalFilter.Text == "") { tbKernalFilter.Text = "3"; }
            if (tbDiameter.Text == "") { tbDiameter.Text = "3"; }
            if (tbSigmaColor.Text == "") { tbSigmaColor.Text = "3"; }
            if (tbSigmaSpace.Text == "") { tbSigmaSpace.Text = "3"; }

            return new FilterToolProperty
            {
                FilterType = AppUtil.ParseEnum<FilterToolType>(cbFilterType.SelectedItem.ToString()),
                KernelWidth = int.Parse(tbFilterW.Text),
                KernelHeight = int.Parse(tbFilterH.Text),
                MedianKernelSize = int.Parse(tbKernalFilter.Text),
                Diameter = int.Parse(tbDiameter.Text),
                SigmaColor = int.Parse(tbSigmaColor.Text),
                SigmaSpace = int.Parse(tbSigmaSpace.Text),
                BorderType = AppUtil.ParseEnum<BorderTypes>(cbFilterBorderType.SelectedItem.ToString())
            };
        }

        private void cbFilterType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!(sender is RJCodeUI_M1.RJControls.RJComboBox)) return;
            string Index = (sender as System.Windows.Forms.ComboBox).Text;

            tbKernalFilter.Enabled = false;
            tbFilterW.Enabled = false;
            tbFilterH.Enabled = false;
            tbDiameter.Enabled = false;
            tbSigmaColor.Enabled = false;
            tbSigmaSpace.Enabled = false;

            switch (AppUtil.ParseEnum<FilterToolType>(Index))
            {
                case FilterToolType.Blur:                
                case FilterToolType.GaussianBlur:
                case FilterToolType.BoxFilter:                    
                    tbFilterW.Enabled = true;                    
                    tbFilterH.Enabled = true;                                        
                    break;
                case FilterToolType.MedianBlur:
                    tbKernalFilter.Enabled = true;
                    break;
                case FilterToolType.BilateralFilter:
                    tbDiameter.Enabled = true;
                    tbSigmaColor.Enabled = true;
                    tbSigmaSpace.Enabled = true;
                    break;
            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}



