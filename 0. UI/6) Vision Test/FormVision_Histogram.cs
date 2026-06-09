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
    public partial class FormVision_Histogram : VisionTestForm
    {
        private enum HistogramType
        {
            clahe,
            equalizeHist,
            Normalize,
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
            foreach (HistogramType histogramType in Enum.GetValues(typeof(HistogramType)))
            {
                cbType.Items.Add(histogramType);
            }
            cbType.SelectedIndex = 0;
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

        public FormVision_Histogram(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }
        private void btnFilterRun_Click(object sender, EventArgs e)
        {
                        Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (tbClipLimit.Text == "") { tbClipLimit.Text = "3"; }
            if (tbTilesGridSize.Text == "") { tbTilesGridSize.Text = "3"; }
            if (tbAlpha.Text == "") { tbAlpha.Text = "0"; }
            if (tbBeta.Text == "") { tbBeta.Text = "100"; }

            double ClipLimit = double.Parse(tbClipLimit.Text);
            int TilesGridSize = int.Parse(tbTilesGridSize.Text);
            int alpha = int.Parse(tbAlpha.Text);
            int beta = int.Parse(tbBeta.Text);
            HistogramType histogramType = AppUtil.ParseEnum<HistogramType>(cbType.SelectedItem.ToString());

            Bitmap Result = CreateSingleInputResult(ibSource, (image, isRoi) =>
            {
                switch (histogramType)
                {
                    case HistogramType.clahe:
                        using (CLAHE clahe = Cv2.CreateCLAHE())
                        {
                            clahe.ClipLimit = ClipLimit;
                            clahe.TilesGridSize = new OpenCvSharp.Size(TilesGridSize, TilesGridSize);
                            clahe.Apply(image, image);
                        }
                        break;
                    case HistogramType.equalizeHist:
                        Cv2.EqualizeHist(image, image);
                        break;
                    case HistogramType.Normalize:
                        if (isRoi) { Cv2.Normalize(image, image, alpha, beta); }
                        else { Cv2.Normalize(image, image, alpha, beta, NormTypes.MinMax); }
                        break;
                }
            });
            PublishResult(cbLayerList2, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
        
        }

        private void cbFilterType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!(sender is RJCodeUI_M1.RJControls.RJComboBox)) return;
            string Index = (sender as System.Windows.Forms.ComboBox).Text;

            //tbFilterW.Enabled = t;
            //tbFilterH.Enabled = false;
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}



