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
    public partial class FormVision_Morphology : VisionTestForm
    {        
        private string Shapes = "Rect";
        private string Operator = "Erode";
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

        public FormVision_Morphology(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }
        private void btnNewPanel_Source_Click(object sender, EventArgs e)
        {  
            InitLayListItem();
        }

        private void btnMorpRun_Click(object sender, EventArgs e)
        {
                        
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (tbMorpW.Text == "") { tbMorpW.Text = "3"; }
            if (tbMorpH.Text == "") { tbMorpH.Text = "3"; }

            int W = int.Parse(tbMorpW.Text);
            int H = int.Parse(tbMorpH.Text);
            using (Mat Kernel = Cv2.GetStructuringElement(AppUtil.ParseEnum<MorphShapes>(Shapes), new OpenCvSharp.Size(W, H)))
            {
                Bitmap Result = CreateSingleInputResult(ibSource, image =>
                {
                    Cv2.MorphologyEx(image, image, AppUtil.ParseEnum<MorphTypes>(Operator), Kernel, new OpenCvSharp.Point(-1, -1), 1);
                });
                PublishResult(cbLayerList2, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
            }
        
        }

        private void OnShapes_CheckedChanged(object sender, EventArgs e)
        {
                        RJCodeUI_M1.RJControls.RJRadioButton rdoButton = (RJCodeUI_M1.RJControls.RJRadioButton)sender;

            if (rdoButton.Checked)
            {
                switch (rdoButton.Text)
                {
                    case "Ellipse":
                    case "Cross":
                    case "Rect":
                        Shapes = rdoButton.Text;
                        break;
                    default:
                        Operator = rdoButton.Text;
                        break;
                }
                
            }

        
        }
            }
 }



