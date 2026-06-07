using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using OpenCvSharp;
using System.Diagnostics;
using OpenVisionLab._1._Core;
using RJCodeUI_M1.RJForms;
using System.Windows.Media;
using OpenVisionLab._2._Common;
using System.Windows.Forms.VisualStyles;
using Lib.Common;
using Lib.Line;
using Lib.OpenCV;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using Lib.OpenCV.Blob;

namespace OpenVisionLab
{
    public partial class FormVision_Line : VisionTestForm
    {
        private CPropertyLineGuage Property_Line_L = new CPropertyLineGuage("Line_L");
        private CPropertyLineGuage Property_Line_R = new CPropertyLineGuage("Line_R");
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
            Property_Line_L = Property_Line_L.LoadTestConfig(Application.StartupPath + "\\TEST\\" + Property_Line_L.NAME + ".xml");
            Property_Line_R = Property_Line_R.LoadTestConfig(Application.StartupPath + "\\TEST\\" + Property_Line_R.NAME + ".xml");
            AttachPropertyGrid(pnParameter, Property_Line_L);
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

        public FormVision_Line(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
        }

        public FormVision_Line()
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

        private List<CResultBlob> cResultBlobs = new List<CResultBlob>();

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                
                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat(ibSource.DisplayBitmap).Clone())
                {
                    Bitmap Result = CDrawBitmap.GetBitmapFormat24bppRgb(ibSource.DisplayBitmap);
                    COpenCVHelper.SetImageChannel1(ImageCVSource);
                    var intersectionLine = CInspectionAlgorithm.RunIntersectionFittingLines(ImageCVSource, Property_Line_L, Property_Line_R);
                    using (Graphics g = Graphics.FromImage(Result))
                    {                        
                        CDrawBitmap.DrawFitLinesInstersectionLines(g, intersectionLine.Item1, intersectionLine.Item2, intersectionLine.Item3);

                        stopwatch.Stop();
                    PublishResult(cbLayerList2, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
                    }
                }

                Property_Line_L.SaveTestConfig(Application.StartupPath + "\\TEST\\" + Property_Line_L.NAME + ".xml");
                Property_Line_R.SaveTestConfig(Application.StartupPath + "\\TEST\\" + Property_Line_R.NAME + ".xml");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void OnPara_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                RJCodeUI_M1.RJControls.RJRadioButton rdoButton = (RJCodeUI_M1.RJControls.RJRadioButton)sender;

                if (rdoButton.Checked)
                {
                    switch (rdoButton.Text)
                    {
                        case "Edge(L)":
                            wpg.SelectedObject = Property_Line_L;
                            break;
                        case "Edge(R)":
                            wpg.SelectedObject = Property_Line_R;
                            break;
                    }
                }

            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnResult_Click(object sender, EventArgs e)
        {
            FormVision_Result formVision_Blob_Result = new FormVision_Result(cResultBlobs);
            formVision_Blob_Result.StartPosition = FormStartPosition.CenterScreen;

            if (!CUtil.OpenCheckForm(formVision_Blob_Result)) return;
            formVision_Blob_Result.Show();
        }

        private void btnEdgeRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                using (Mat imageCVSource = Lib.Common.CImageConverter.ToMat(ibSource.DisplayBitmap).Clone())
                {
                    Bitmap Result = CDrawBitmap.GetBitmapFormat24bppRgb(ibSource.DisplayBitmap);
                    using (Graphics g = Graphics.FromImage(Result))
                    {
                        CVLineGuage edgeTool = new CVLineGuage();
                        edgeTool.SetSourceImage(imageCVSource);                        
                        edgeTool.SetProperty(rdoLeftEdgePara.Checked ? (CPropertyLineGuage)Property_Line_L.DeepCopy() : (CPropertyLineGuage)Property_Line_R.DeepCopy());
                        edgeTool.Run();

                        CDrawBitmap.DrawCVLineGuage(g, edgeTool);
 
                        stopwatch.Stop();
                    PublishResult(cbLayerList2, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
                    }                    
                }

                CUtil.InitDirectory("TEST");                
                Property_Line_L.SaveTestConfig(Application.StartupPath + "\\TEST\\" + Property_Line_L.NAME + ".xml");
                Property_Line_R.SaveTestConfig(Application.StartupPath + "\\TEST\\" + Property_Line_R.NAME + ".xml");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnVerLineRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat(ibSource.DisplayBitmap).Clone())
                {
                    Bitmap Result = CDrawBitmap.GetBitmapFormat24bppRgb(ibSource.DisplayBitmap);
                    COpenCVHelper.SetImageChannel1(ImageCVSource);
                    var intersectionLine = CInspectionAlgorithm.RunIntersectionLines(ImageCVSource, Property_Line_L, Property_Line_R);

                    using (Graphics g = Graphics.FromImage(Result))
                    {
                        CDrawBitmap.DrawInstersectionLines(g, intersectionLine.Item1, intersectionLine.Item2, intersectionLine.Item3);

                        stopwatch.Stop();
                    PublishResult(cbLayerList2, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
                    }
                }
                
                Property_Line_L.SaveTestConfig(Application.StartupPath + "\\TEST\\" + Property_Line_L.NAME + ".xml");
                Property_Line_R.SaveTestConfig(Application.StartupPath + "\\TEST\\" + Property_Line_R.NAME + ".xml");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
    }
 }



