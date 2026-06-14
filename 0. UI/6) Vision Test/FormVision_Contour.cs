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
using Lib.OpenCV;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;

namespace OpenVisionLab
{
    public partial class FormVision_Contour : VisionTestForm
    {
        private ContourProperty Property_Contour = new ContourProperty("Contour");
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
            Property_Contour = Property_Contour.LoadTestConfig(AppPathService.GetTestConfigPath(Property_Contour.NAME));
            AttachPropertyGridWithThresholdPreview(pnParameter, Property_Contour, cbLayerList2, ibSource, ibDestination);
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
            VisionPipelineFormBridge.AttachAddButton(btnRun, () => Property_Contour, cbLayerList, cbLayerList2);
        }

        public FormVision_Contour(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }

        public FormVision_Contour()
        {

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

        private List<ContourResult> cResultContours = new List<ContourResult>();

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (!ValidateDrawOption())
            {
                return;
            }

            RunVisionStep("Contour", () =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                using (Mat ImageCVSource = CreateRunSourceMat(ibSource, out Bitmap Result))
                {
                    ContourTool contourTool = new ContourTool();
                    contourTool.SetProperty(Property_Contour);
                    contourTool.SetSourceImage(ImageCVSource);
                    contourTool.Run();

                    if (!Property_Contour.USE_DRAW_IMAGE)
                    {
                        using (Graphics g = Graphics.FromImage(Result))
                        {
                            foreach (var item in contourTool.results)
                            {
                                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                            }

                            if (contourTool.results.Count == 0)
                            {
                                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CommonConverter.CVRectToRect(contourTool.property.CvROI));
                                g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)contourTool.property.CvROI.X - 20, (int)contourTool.property.CvROI.Y - 20);
                            }
                        }
                    }
                    else
                    {
                        Result.Dispose();
                        Bitmap drawResult = Lib.Common.BitmapImageConverter.ToBitmap(contourTool.imageResult);
                        Result = drawResult;
                    }

                    cResultContours = contourTool.results;
                    PublishResult(cbLayerList2, ibDestination, Result, FormatElapsed(stopwatch));
                }

                AppUtil.InitDirectory("TEST");
                string strPath = AppPathService.GetTestConfigPath(Property_Contour.NAME);
                Property_Contour.SaveTestConfig(strPath);
            });
        }

        private bool ValidateDrawOption()
        {
            if (!Property_Contour.USE_DRAW_IMAGE)
            {
                return true;
            }

            if (!Property_Contour.DrawColor.IsEmpty && Property_Contour.DrawColor.A != 0)
            {
                return true;
            }

            AppCommon.ShowMessageBox("ALARM", "DrawColor is empty. Please select a draw color before running contour inspection.", FormMessageBox.MESSAGEBOX_TYPE.Waring);
            return false;
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
            FormVision_Result resultForm = new FormVision_Result(cResultContours);
            resultForm.StartPosition = FormStartPosition.CenterScreen;

            if (!AppUtil.OpenCheckForm(resultForm)) return;
            resultForm.Show();
        }
    }
 }



