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

namespace OpenVisionLab
{
    public partial class FormVision_Filter : VisionTestForm
    {
        private enum FilterType
        {
            Blur,
            BoxFilter,
            MedianBlur,
            GaussianBlur,
            BilateralFilter
        }
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
            foreach (FilterType filterType in Enum.GetValues(typeof(FilterType)))
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
                        Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (tbFilterW.Text == "") { tbFilterW.Text = "3"; }
            if (tbFilterH.Text == "") { tbFilterH.Text = "3"; }
            if (tbKernalFilter.Text == "") { tbKernalFilter.Text = "3"; }
            if (tbDiameter.Text == "") { tbDiameter.Text = "3"; }
            if (tbSigmaColor.Text == "") { tbSigmaColor.Text = "3"; }
            if (tbSigmaSpace.Text == "") { tbSigmaSpace.Text = "3"; }

            int W = int.Parse(tbFilterW.Text);
            int H = int.Parse(tbFilterH.Text);
            int Diameter = int.Parse(tbDiameter.Text);
            int sigmaColor = int.Parse(tbSigmaColor.Text);
            int sigmaSpace = int.Parse(tbSigmaSpace.Text);
            BorderTypes borderType = AppUtil.ParseEnum<BorderTypes>(cbFilterBorderType.SelectedItem.ToString());
            FilterType filterType = AppUtil.ParseEnum<FilterType>(cbFilterType.SelectedItem.ToString());

            Bitmap Result = CreateSingleInputResult(ibSource, image =>
            {
                switch (filterType)
                {
                    case FilterType.Blur:
                        Cv2.Blur(image, image, new OpenCvSharp.Size(W, H), new OpenCvSharp.Point(-1, -1), borderType);
                        break;
                    case FilterType.GaussianBlur:
                        Cv2.GaussianBlur(image, image, new OpenCvSharp.Size(W, H), 1, 1, borderType);
                        break;
                    case FilterType.MedianBlur:
                        Cv2.MedianBlur(image, image, int.Parse(tbKernalFilter.Text));
                        break;
                    case FilterType.BoxFilter:
                        Cv2.BoxFilter(image, image, MatType.CV_8UC3, new OpenCvSharp.Size(W, H), new OpenCvSharp.Point(-1, -1));
                        break;
                    case FilterType.BilateralFilter:
                        Cv2.BilateralFilter(image, image, Diameter, sigmaColor, sigmaSpace, borderType);
                        break;
                }
            });
            PublishResult(cbLayerList2, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
        
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

            switch (AppUtil.ParseEnum<FilterType>(Index))
            {
                case FilterType.Blur:                
                case FilterType.GaussianBlur:
                case FilterType.BoxFilter:                    
                    tbFilterW.Enabled = true;                    
                    tbFilterH.Enabled = true;                                        
                    break;
                case FilterType.MedianBlur:
                    tbKernalFilter.Enabled = true;
                    break;
                case FilterType.BilateralFilter:
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



