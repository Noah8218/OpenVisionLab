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

        public FormVision_Filter(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
            panelCount = this.displayManager.LayerCount;
            timer1.Enabled = true;
            timer1.Start();
        }

        private void InitLayListItem()
        {
            cbLayerList.Items.Clear();
            for (int i = 0; i < displayManager.LayerCount; i++) { cbLayerList.Items.Add(displayManager.GetLayerTitle(i)); }
            cbLayerList.SelectedIndex = source1_Index;
            cbLayerList2.Items.Clear();
            for (int i = 0; i < displayManager.LayerCount; i++) { cbLayerList2.Items.Add(displayManager.GetLayerTitle(i)); }
            cbLayerList2.SelectedIndex = destination_Index;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            InitFilterMenu();
            InitFilterBorderMenu();
            InitEvent();
            InitLayListItem();
            source_1.LoadImageBox(ibSource, false);
            destination.LoadImageBox(ibDestination, false);

            ibSource.Image = GetLayerImage(DEFINE.Main);
            ibDestination.Image = GetLayerImage(DEFINE.Main);
            ibSource.ImageChanged += IbSource_ImageChanged;
            ibDestination.ImageChanged += IbDestination_ImageChanged;
            ibDestination.MouseClick += IbDestination_MouseClick;
            ibSource.MouseClick += IbSource_MouseClick;
            ibSource.ZoomToFit();
            ibDestination.ZoomToFit();

            //toolTip1.SetToolTip(btnNewPanel_Source, "Create New Layer");
            toolTip1.SetToolTip(lbKernel, "Ȧ������ �����մϴ�.");
            toolTip1.SetToolTip(lbDiameter, "�帲 ȿ���� ������ �� �ȼ� ������ ������ �ǹ��մϴ�.");
            toolTip1.SetToolTip(lbSigmaColor, "���� ����(color domain)���� ����� ����þ� Ŀ���� �ʺ� �����ϸ�, �Ű������� ���� Ŭ���� �帲 ȿ���� ���Ե� ������ ������ �о����ϴ�.");
            toolTip1.SetToolTip(lbSigmaSpace, "��ǥ ����(space domain)���� ����� ����þ� Ŀ���� �ʺ� �����ϸ�, ���� Ŭ���� ������ �ȼ��� ������ ��Ĩ�ϴ�.");
        }

        public void InitFilterMenu()
        {
            cbFilterType.Items.Add(FilterType.Blur);
            cbFilterType.Items.Add(FilterType.BoxFilter);
            cbFilterType.Items.Add(FilterType.BilateralFilter);
            cbFilterType.Items.Add(FilterType.GaussianBlur);
            cbFilterType.Items.Add(FilterType.MedianBlur);
            cbFilterType.SelectedIndex = 0;
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

        private void IbDestination_MouseClick(object sender, MouseEventArgs e)
        {
            displayManager.ActivateLayer(destination_Index);
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbSource_MouseClick(object sender, MouseEventArgs e)
        {
            displayManager.ActivateLayer(source1_Index);
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbDestination_ImageChanged(object sender, EventArgs e)
        {            
            destination_Index = cbLayerList2.SelectedIndex;
            SetLayerImage(destination_Index, (Bitmap)ibDestination.Image);
        }

        private void IbSource_ImageChanged(object sender, EventArgs e)
        {            
            source1_Index = cbLayerList.SelectedIndex;
            SetLayerImage(source1_Index, (Bitmap)ibSource.Image);
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private bool InitEvent()
        {
            try
            {
                this.KeyPreview = true;
                this.KeyDown += Form_KeyDown;

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }

        private void btnNewPanel_Desty_Click(object sender, EventArgs e)
        {
            displayManager.CreatePanel();
            InitLayListItem();
            destination_Index = cbLayerList2.Items.Count - 1;
            cbLayerList2.SelectedIndex = destination_Index;
        }
        
        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            source1_Index = cbLayerList.SelectedIndex;
            ibSource.Image = GetLayerImage(source1_Index);
        }

        private void cbLayerList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            destination_Index = cbLayerList2.SelectedIndex;
            ibDestination.Image = GetLayerImage(destination_Index);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (panelCount != displayManager.LayerCount)
                {
                    panelCount = displayManager.LayerCount;
                    InitLayListItem();
                }

                RefreshViewerRoi(source_1, ibSource, source1_Index);

                RefreshViewerRoi(destination, ibDestination, destination_Index);

                cbLayerList_SelectedIndexChanged(null, null);
                cbLayerList2_SelectedIndexChanged(null, null);
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnFilterRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource.Image).Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

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
                    Bitmap Result = new Bitmap(10, 10);

                    if (displayManager.IsLayerRoiEmpty(source1_Index))
                    {
                        switch (CUtil.ParseEnum<FilterType>(cbFilterType.SelectedItem.ToString()))
                        {
                            case FilterType.Blur:
                                Cv2.Blur(ImageCVSource, ImageCVSource, new OpenCvSharp.Size(W, H), new OpenCvSharp.Point(-1, -1), CUtil.ParseEnum<BorderTypes>(cbFilterBorderType.SelectedItem.ToString()));
                                break;
                            case FilterType.GaussianBlur:
                                Cv2.GaussianBlur(ImageCVSource, ImageCVSource, new OpenCvSharp.Size(W, H), 1, 1, CUtil.ParseEnum<BorderTypes>(cbFilterBorderType.SelectedItem.ToString()));
                                break;
                            case FilterType.MedianBlur:
                                Cv2.MedianBlur(ImageCVSource, ImageCVSource, int.Parse(tbKernalFilter.Text));
                                break;
                            case FilterType.BoxFilter:
                                Cv2.BoxFilter(ImageCVSource, ImageCVSource, MatType.CV_8UC3, new OpenCvSharp.Size(W, H), new OpenCvSharp.Point(-1, -1));
                                break;
                            case FilterType.BilateralFilter:
                                Cv2.BilateralFilter(ImageCVSource, ImageCVSource, Diameter, sigmaColor, sigmaSpace, CUtil.ParseEnum<BorderTypes>(cbFilterBorderType.SelectedItem.ToString()));
                                break;
                        }
                        Result = Lib.Common.CImageConverter.ToBitmap(ImageCVSource);
                    }
                    else
                    {
                        Rect r = CConverter.RectangleToRect(GetLayerRoi(source1_Index));
                        Mat ImageRoi = ImageCVSource.SubMat(r);

                        switch (CUtil.ParseEnum<FilterType>(cbFilterType.SelectedItem.ToString()))
                        {
                            case FilterType.Blur:
                                Cv2.Blur(ImageRoi, ImageRoi, new OpenCvSharp.Size(W, H), new OpenCvSharp.Point(-1, -1), CUtil.ParseEnum<BorderTypes>(cbFilterBorderType.SelectedItem.ToString()));
                                break;
                            case FilterType.GaussianBlur:
                                Cv2.GaussianBlur(ImageRoi, ImageRoi, new OpenCvSharp.Size(W, H), 1, 1, CUtil.ParseEnum<BorderTypes>(cbFilterBorderType.SelectedItem.ToString()));
                                break;
                            case FilterType.MedianBlur:
                                Cv2.MedianBlur(ImageRoi, ImageRoi, int.Parse(tbKernalFilter.Text));
                                break;
                            case FilterType.BoxFilter:
                                Cv2.BoxFilter(ImageRoi, ImageRoi, MatType.CV_8UC3, new OpenCvSharp.Size(W, H), new OpenCvSharp.Point(-1, -1));
                                break;
                            case FilterType.BilateralFilter:
                                Cv2.BilateralFilter(ImageRoi, ImageRoi, Diameter, sigmaColor, sigmaSpace, CUtil.ParseEnum<BorderTypes>(cbFilterBorderType.SelectedItem.ToString()));
                                break;
                        }

                        Result = Lib.Common.CBitmapProcessing.OverlayImage(Lib.Common.CImageConverter.ToBitmap(ImageCVSource), Lib.Common.CImageConverter.ToBitmap(ImageRoi), r.Left, r.Top);
                    }

                    SetLayerImage(GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), Result);
                    ibDestination.Image = Result;
                    eventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
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

            tbKernalFilter.Enabled = false;
            tbFilterW.Enabled = false;
            tbFilterH.Enabled = false;
            tbDiameter.Enabled = false;
            tbSigmaColor.Enabled = false;
            tbSigmaSpace.Enabled = false;

            switch (CUtil.ParseEnum<FilterType>(Index))
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

