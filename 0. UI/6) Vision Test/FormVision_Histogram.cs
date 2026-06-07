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
            CUtil.InitDirectory("TEST");
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
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat(ibSource.DisplayBitmap).Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    if (tbClipLimit.Text == "") { tbClipLimit.Text = "3"; }
                    if (tbTilesGridSize.Text == "") { tbTilesGridSize.Text = "3"; }
                    if (tbAlpha.Text == "") { tbAlpha.Text = "0"; }
                    if (tbBeta.Text == "") { tbBeta.Text = "100"; }

                    double ClipLimit = double.Parse(tbClipLimit.Text);
                    int TilesGridSize = int.Parse(tbTilesGridSize.Text);
                    int alpha = int.Parse(tbAlpha.Text);
                    int beta = int.Parse(tbBeta.Text);

                    Bitmap Result = new Bitmap(10, 10);

                    if (displayManager.IsLayerRoiEmpty(source1_Index))
                    {
                        switch (CUtil.ParseEnum<HistogramType>(cbType.SelectedItem.ToString()))
                        {
                            case HistogramType.clahe:
                                using (CLAHE clahe = Cv2.CreateCLAHE())
                                {
                                    clahe.ClipLimit = ClipLimit;
                                    clahe.TilesGridSize = new OpenCvSharp.Size(TilesGridSize, TilesGridSize);
                                    clahe.Apply(ImageCVSource, ImageCVSource);
                                }                                
                                break;
                            case HistogramType.equalizeHist:
                                Cv2.EqualizeHist(ImageCVSource, ImageCVSource);
                                break;
                            case HistogramType.Normalize:
                                Cv2.Normalize(ImageCVSource, ImageCVSource, alpha, beta, NormTypes.MinMax);
                                //Cv2.Normalize(ImageCVSource, ImageCVSource);
                                break;                                
                        }


                        Result = Lib.Common.CImageConverter.ToBitmap(ImageCVSource);
                    }
                    else
                    {
                        Rect r = CConverter.RectangleToRect(GetLayerRoi(source1_Index));
                        Mat ImageRoi = ImageCVSource.SubMat(r);

                        switch (CUtil.ParseEnum<HistogramType>(cbType.SelectedItem.ToString()))
                        {
                            case HistogramType.clahe:
                                using (CLAHE clahe = Cv2.CreateCLAHE())
                                {
                                    clahe.ClipLimit = ClipLimit;
                                    clahe.TilesGridSize = new OpenCvSharp.Size(TilesGridSize, TilesGridSize);
                                    clahe.Apply(ImageRoi, ImageRoi);
                                }
                                break;
                            case HistogramType.equalizeHist:
                                Cv2.EqualizeHist(ImageRoi, ImageRoi);
                                break;
                            case HistogramType.Normalize:
                                Cv2.Normalize(ImageRoi, ImageRoi, alpha, beta);
                                break;
                        }

                        Result = Lib.Common.CBitmapProcessing.OverlayImage(Lib.Common.CImageConverter.ToBitmap(ImageCVSource), Lib.Common.CImageConverter.ToBitmap(ImageRoi), r.Left, r.Top);
                    }
                    PublishResult(cbLayerList2, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowMessageBox("ALARM", $"{Desc}");
            }
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



