using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using OpenCvSharp;
using MetroFramework.Forms;
using Keys = System.Windows.Forms.Keys;
using KtemVisionSystem._1._Core;

namespace KtemVisionSystem
{
    public partial class FormVision_Filter : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;

        private List<FormLayerDisplay> Displays { get; set; } = new List<FormLayerDisplay>();
        private int PanelCount = 0;

        private KtemViewer Source = new KtemViewer();
        private KtemViewer Destination = new KtemViewer();

        #region Event Register        
        public EventHandler<DockDisplayEventArgs> EventUpdateDisplay;
        #endregion

        private enum FilterType
        {
            Blur,
            BoxFilter,
            MedianBlur,
            GaussianBlur,
            BilateralFilter
        }

        public FormVision_Filter(List<FormLayerDisplay> Displays, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            this.Displays = Displays;
            this.EventUpdateDisplay = EventUpdateDisplay;
            PanelCount = Displays.Count;
            timer1.Enabled = true;
            timer1.Start();
        }

        private void InitLayListItem()
        {
            cbLayerList.Items.Clear();
            for (int i = 0; i < Displays.Count; i++) { cbLayerList.Items.Add(Displays[i].Text); }
            cbLayerList.SelectedIndex = Source_Index;
            cbLayerList2.Items.Clear();
            for (int i = 0; i < Displays.Count; i++) { cbLayerList2.Items.Add(Displays[i].Text); }
            cbLayerList2.SelectedIndex = Destination_Index;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            InitFilterMenu();
            InitFilterBorderMenu();
            InitEvent();
            InitLayListItem();
            Source.LoadImageBox(ibSource, false);
            Destination.LoadImageBox(ibDestination, false);

            ibSource.Image = (Bitmap)Displays[DEFINE.Main].ImageView.ib.Image;
            ibDestination.Image = (Bitmap)Displays[DEFINE.Main].ImageView.ib.Image;
            ibSource.ImageChanged += IbSource_ImageChanged;
            ibDestination.ImageChanged += IbDestination_ImageChanged;
            ibDestination.MouseClick += IbDestination_MouseClick;
            ibSource.MouseClick += IbSource_MouseClick;
            ibSource.ZoomToFit();
            ibDestination.ZoomToFit();

            //toolTip1.SetToolTip(btnNewPanel_Source, "Create New Layer");
            toolTip1.SetToolTip(lbKernel, "홀수값만 가능합니다.");
            toolTip1.SetToolTip(lbDiameter, "흐림 효과를 적용할 각 픽셀 영역의 지름을 의미합니다.");
            toolTip1.SetToolTip(lbSigmaColor, "색상 공간(color domain)에서 사용할 가우시안 커널의 너비를 설정하며, 매개변수의 값이 클수록 흐림 효과에 포함될 강도의 범위가 넓어집니다.");
            toolTip1.SetToolTip(lbSigmaSpace, "좌표 공간(space domain)에서 사용할 가우시안 커널의 너비를 설정하며, 값이 클수록 인접한 픽셀에 영향을 미칩니다.");
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
            Displays[Destination_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbSource_MouseClick(object sender, MouseEventArgs e)
        {
            Displays[Source_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void IbDestination_ImageChanged(object sender, EventArgs e)
        {            
            Destination_Index = cbLayerList2.SelectedIndex;
            Displays[Destination_Index].ibSource.Image = (Bitmap)ibDestination.Image;
        }

        private void IbSource_ImageChanged(object sender, EventArgs e)
        {            
            Source_Index = cbLayerList.SelectedIndex;
            Displays[Source_Index].ibSource.Image = (Bitmap)ibSource.Image;
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

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        private void btnNewPanel_Desty_Click(object sender, EventArgs e)
        {
            CDisplayManager.CreatePanel();
            InitLayListItem();
            Destination_Index = cbLayerList2.Items.Count - 1;
            cbLayerList2.SelectedIndex = Destination_Index;
        }

        private int GetDisplayIndex(string strTitle)
        {
            for (int i = 0; i < Displays.Count; i++)
            {
                if (Displays[i].Text == strTitle) { return i; }
            }

            return 0;
        }

        private int Source_Index = 0;
        private int Destination_Index = 0;

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Source_Index = cbLayerList.SelectedIndex;
            ibSource.Image = (Bitmap)Displays[Source_Index].ibSource.Image;
        }

        private void cbLayerList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Destination_Index = cbLayerList2.SelectedIndex;
            ibDestination.Image = (Bitmap)Displays[Destination_Index].ibSource.Image;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PanelCount != Displays.Count)
                {
                    PanelCount = Displays.Count;
                    InitLayListItem();
                }

                if (Source.ROI != Displays[Source_Index].ImageView.ROI || Source.TrainROI != Displays[Source_Index].ImageView.TrainROI)
                {
                    ibSource.Invalidate();
                }
                Source.ROI = Displays[Source_Index].ImageView.ROI;
                Source.TrainROI = Displays[Source_Index].ImageView.TrainROI;

                if (Destination.ROI != Displays[Destination_Index].ImageView.ROI || Destination.TrainROI != Displays[Destination_Index].ImageView.TrainROI)
                {
                    ibDestination.Invalidate();
                }
                Destination.ROI = Displays[Destination_Index].ImageView.ROI;
                Destination.TrainROI = Displays[Destination_Index].ImageView.TrainROI;

                cbLayerList_SelectedIndexChanged(null, null);
                cbLayerList2_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnFilterRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = CConverter.ToMat((Bitmap)ibSource.Image).Clone())
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

                    if (Displays[Source_Index].ImageView.ROI.IsEmpty)
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
                        Result = CConverter.ToBitmap(ImageCVSource);
                    }
                    else
                    {
                        Rect r = CConverter.RectangleToRect(Displays[Source_Index].ImageView.ROI);
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

                        Result = CConverter.OverlayImage(CConverter.ToBitmap(ImageCVSource), CConverter.ToBitmap(ImageRoi), r.Left, r.Top);
                    }

                    Displays[GetDisplayIndex(cbLayerList2.SelectedItem.ToString())].ImageView.ib.Image = Result;
                    ibDestination.Image = Result;
                    EventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList2.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowMessageBox("ALARM", $"{ex}");
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

