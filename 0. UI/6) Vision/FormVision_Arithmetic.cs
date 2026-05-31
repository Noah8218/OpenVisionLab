using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using OpenCvSharp;
using MetroFramework.Forms;
using Keys = System.Windows.Forms.Keys;
using Automation.BDaq;
using System.Windows.Media;
using KtemVisionSystem._1._Core;

namespace KtemVisionSystem
{
    public partial class FormVision_Arithmetic : MetroForm
    {
        private List<FormLayerDisplay> Displays { get; set; } = new List<FormLayerDisplay>();
        private int PanelCount = 0;

        private KtemViewer Source_1 = new KtemViewer();
        private KtemViewer Source_2 = new KtemViewer();
        private KtemViewer Destination = new KtemViewer();

        #region Event Register        
        public EventHandler<DockDisplayEventArgs> EventUpdateDisplay;
        #endregion

        private enum Arithmetic
        {
            Bitwise_AND,
            Bitwise_OR,
            Bitwise_XOR,
            Bitwise_NOT,
            ADD,
            SUBTRACT,
            MULTIPLY,
            DIVIDE,
            MAX,
            MIN,
            ABS,
            ABSDIFF
        }

        public FormVision_Arithmetic(List<FormLayerDisplay> Displays, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
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
            cbLayerList1.Items.Clear();
            for (int i = 0; i < Displays.Count; i++) { cbLayerList1.Items.Add(Displays[i].Text); }
            cbLayerList1.SelectedIndex = Source1_Index;

            cbLayerList2.Items.Clear();
            for (int i = 0; i < Displays.Count; i++) { cbLayerList2.Items.Add(Displays[i].Text); }
            cbLayerList2.SelectedIndex = Source2_Index;

            cbLayerList_Dest.Items.Clear();
            for (int i = 0; i < Displays.Count; i++) { cbLayerList_Dest.Items.Add(Displays[i].Text); }
            cbLayerList_Dest.SelectedIndex = Destination_Index;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            InitMenu();
            InitEvent();
            InitLayListItem();
            Source_1.LoadImageBox(ibSource1, false);
            Source_2.LoadImageBox(ibSource2, false);
            Destination.LoadImageBox(ibDestination, false);

            ibSource1.Image = (Bitmap)Displays[DEFINE.Main].ImageView.ib.Image;
            ibSource1.ImageChanged += IbSource1_ImageChanged;
            ibSource1.MouseClick += IbSource1_MouseClick;
            ibSource1.ZoomToFit();

            ibSource2.Image = (Bitmap)Displays[DEFINE.Main].ImageView.ib.Image;
            ibSource2.ImageChanged += IbSource2_ImageChanged;
            ibSource2.MouseClick += IbSource2_MouseClick;
            ibSource2.ZoomToFit();

            ibDestination.Image = (Bitmap)Displays[DEFINE.Main].ImageView.ib.Image;
            ibDestination.ImageChanged += IbDestination_ImageChanged;
            ibDestination.MouseClick += IbDestination_MouseClick;
            ibDestination.ZoomToFit();

            this.GotFocus += FormVision_Arithmetic_GotFocus;

            rdoSourceImage.Checked = true;

            //toolTip1.SetToolTip(btnNewPanel_Source, "Create New Layer");
            //toolTip1.SetToolTip(lbKernel, "홀수값만 가능합니다.");
            //toolTip1.SetToolTip(lbDiameter, "흐림 효과를 적용할 각 픽셀 영역의 지름을 의미합니다.");
            //toolTip1.SetToolTip(lbSigmaColor, "색상 공간(color domain)에서 사용할 가우시안 커널의 너비를 설정하며, 매개변수의 값이 클수록 흐림 효과에 포함될 강도의 범위가 넓어집니다.");
            //toolTip1.SetToolTip(lbSigmaSpace, "좌표 공간(space domain)에서 사용할 가우시안 커널의 너비를 설정하며, 값이 클수록 인접한 픽셀에 영향을 미칩니다.");
        }

        private void FormVision_Arithmetic_GotFocus(object sender, EventArgs e)
        {

        }

        public void InitMenu()
        {
            cbArithmeticType.Items.Add(Arithmetic.Bitwise_AND);
            cbArithmeticType.Items.Add(Arithmetic.Bitwise_OR);
            cbArithmeticType.Items.Add(Arithmetic.Bitwise_XOR);
            cbArithmeticType.Items.Add(Arithmetic.Bitwise_NOT);
            cbArithmeticType.Items.Add(Arithmetic.ADD);
            cbArithmeticType.Items.Add(Arithmetic.SUBTRACT);
            cbArithmeticType.Items.Add(Arithmetic.MULTIPLY);
            cbArithmeticType.Items.Add(Arithmetic.DIVIDE);
            cbArithmeticType.Items.Add(Arithmetic.MAX);
            cbArithmeticType.Items.Add(Arithmetic.MIN);
            cbArithmeticType.Items.Add(Arithmetic.ABS);
            cbArithmeticType.Items.Add(Arithmetic.ABSDIFF);
            cbArithmeticType.SelectedIndex = 0;
        }

        private void IbSource2_MouseClick(object sender, MouseEventArgs e)
        {
            Displays[Source2_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
            Displays[Source2_Index].ibSource.ZoomToFit();
        }

        private void IbSource2_ImageChanged(object sender, EventArgs e)
        {
            Source2_Index = cbLayerList2.SelectedIndex;
            Displays[Source2_Index].ibSource.Image = (Bitmap)ibSource2.Image;
        }

        private void IbDestination_MouseClick(object sender, MouseEventArgs e)
        {
            Displays[Destination_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
            Displays[Destination_Index].ibSource.ZoomToFit();
        }

        private void IbSource1_MouseClick(object sender, MouseEventArgs e)
        {
            Displays[Source1_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
            Displays[Source1_Index].ibSource.ZoomToFit();
        }

        private void IbDestination_ImageChanged(object sender, EventArgs e)
        {
            Destination_Index = cbLayerList_Dest.SelectedIndex;
            Displays[Destination_Index].ibSource.Image = (Bitmap)ibDestination.Image;
        }

        private void IbSource1_ImageChanged(object sender, EventArgs e)
        {
            Source1_Index = cbLayerList1.SelectedIndex;
            Displays[Source1_Index].ibSource.Image = (Bitmap)ibSource1.Image;
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
            Destination_Index = cbLayerList_Dest.Items.Count - 1;
            cbLayerList_Dest.SelectedIndex = Destination_Index;
        }

        private int GetDisplayIndex(string strTitle)
        {
            for (int i = 0; i < Displays.Count; i++)
            {
                if (Displays[i].Text == strTitle) { return i; }
            }

            return 0;
        }

        private int Source1_Index = 0;
        private int Source2_Index = 0;
        private int Destination_Index = 0;

        private void cbLayerList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Source1_Index = cbLayerList1.SelectedIndex;
            Source_1.ib.Image = 
            ibSource1.Image = (Bitmap)Displays[Source1_Index].ibSource.Image;
        }

        private void cbLayerListDestination_SelectedIndexChanged(object sender, EventArgs e)
        {
            Destination_Index = cbLayerList_Dest.SelectedIndex;
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

                if (Source_1.ROI != Displays[Source1_Index].ImageView.ROI || Source_1.TrainROI != Displays[Source1_Index].ImageView.TrainROI)
                {
                    ibSource1.Invalidate();
                }
                Source_1.ROI = Displays[Source1_Index].ImageView.ROI;
                Source_1.TrainROI = Displays[Source1_Index].ImageView.TrainROI;

                if (Source_2.ROI != Displays[Source2_Index].ImageView.ROI || Source_2.TrainROI != Displays[Source2_Index].ImageView.TrainROI)
                {
                    ibSource2.Invalidate();
                }
                Source_2.ROI = Displays[Source2_Index].ImageView.ROI;
                Source_2.TrainROI = Displays[Source2_Index].ImageView.TrainROI;


                if (Destination.ROI != Displays[Destination_Index].ImageView.ROI || Destination.TrainROI != Displays[Destination_Index].ImageView.TrainROI)
                {
                    ibDestination.Invalidate();
                }
                Destination.ROI = Displays[Destination_Index].ImageView.ROI;
                Destination.TrainROI = Displays[Destination_Index].ImageView.TrainROI;

                cbLayerList1_SelectedIndexChanged(null, null);
                cbLayerList2_OnSelectedIndexChanged(null, null);
                cbLayerListDestination_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnFilterRun_Click(object sender, EventArgs e)
        {

        }

        private void cbLayerList2_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Source2_Index = cbLayerList2.SelectedIndex;
            ibSource2.Image = (Bitmap)Displays[Source2_Index].ibSource.Image;
        }

        private void btnArithmeticRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource1 = CConverter.ToMat((Bitmap)ibSource1.Image).Clone())
                using (Mat ImageCVSource2 = CConverter.ToMat((Bitmap)ibSource2.Image).Clone())
                {
                    // bgr임
                    if (tbGray.Text == "") { tbGray.Text = "1"; }
                    if (tbR.Text == "") { tbR.Text = "1"; }
                    if (tbB.Text == "") { tbB.Text = "1"; }
                    if (tbG.Text == "") { tbG.Text = "1"; }

                    Mat val = new Mat();

                    if (rdoContrast.Checked)
                    {
                        int Gray = int.Parse(tbGray.Text);
                        int B = int.Parse(tbB.Text);
                        int G = int.Parse(tbG.Text);
                        int R = int.Parse(tbR.Text);

                        if (rdoGray.Checked)
                        {
                            if (ImageCVSource1.Channels() == 3) Cv2.CvtColor(ImageCVSource1, ImageCVSource1, ColorConversionCodes.RGB2GRAY);
                            val = new Mat(ImageCVSource1.Size(), MatType.CV_8UC1, new Scalar(Gray, Gray, Gray));
                        }
                        else if (rdoColor.Checked)
                        {
                            if (ImageCVSource1.Channels() == 1) Cv2.CvtColor(ImageCVSource1, ImageCVSource1, ColorConversionCodes.GRAY2BGR);
                            val = new Mat(ImageCVSource1.Size(), MatType.CV_8UC3, new Scalar(B, G, R));
                        }
                        val.CopyTo(ImageCVSource2);
                        //ImageCVSource2.CopyTo(val);
                    }

                    Mat ResultImage = new Mat();
                    Bitmap Result = new Bitmap(10, 10);
                    if (Displays[Source1_Index].ImageView.ROI.IsEmpty)
                    {
                        if (ImageCVSource1.Channels() != 1) Cv2.CvtColor(ImageCVSource1, ImageCVSource1, ColorConversionCodes.RGB2GRAY);
                        if (ImageCVSource2.Channels() != 1) Cv2.CvtColor(ImageCVSource2, ImageCVSource2, ColorConversionCodes.RGB2GRAY);

                        switch (CUtil.ParseEnum<Arithmetic>(cbArithmeticType.SelectedItem.ToString()))
                        {
                            case Arithmetic.Bitwise_AND:
                                Cv2.BitwiseAnd(ImageCVSource1, ImageCVSource2, ResultImage);
                                break;
                            case Arithmetic.Bitwise_OR:
                                Cv2.BitwiseOr(ImageCVSource1, ImageCVSource2, ResultImage);
                                break;
                            case Arithmetic.Bitwise_XOR:
                                Cv2.BitwiseXor(ImageCVSource1, ImageCVSource2, ResultImage);
                                break;
                            case Arithmetic.Bitwise_NOT:
                                Cv2.BitwiseNot(ImageCVSource1, ResultImage);
                                break;
                            case Arithmetic.ADD:
                                Cv2.Add(ImageCVSource1, ImageCVSource2, ResultImage);
                                break;
                            case Arithmetic.SUBTRACT:
                                Cv2.Subtract(ImageCVSource1, ImageCVSource2, ResultImage);
                                break;
                            case Arithmetic.MULTIPLY:
                                Cv2.Multiply(ImageCVSource1, ImageCVSource2, ResultImage);
                                break;
                            case Arithmetic.DIVIDE:
                                Cv2.Divide(ImageCVSource1, ImageCVSource2, ResultImage);
                                break;
                            case Arithmetic.MAX:
                                Cv2.Max(ImageCVSource1, ImageCVSource2, ResultImage);
                                break;
                            case Arithmetic.MIN:
                                Cv2.Min(ImageCVSource1, ImageCVSource2, ResultImage);
                                break;
                            case Arithmetic.ABSDIFF:
                                Cv2.Absdiff(ImageCVSource1, ImageCVSource2, ResultImage);
                                break;
                            case Arithmetic.ABS:
                                ResultImage = Cv2.Abs(ImageCVSource1);
                                break;
                        }

                        Result = CConverter.ToBitmap(ResultImage);
                    }
                    else
                    {
                        Rect r = CConverter.RectangleToRect(Displays[Source1_Index].ImageView.ROI);
                        Mat ImageRoi = ImageCVSource1.SubMat(r);
                        Mat ImageRoi2 = ImageCVSource2.SubMat(r);

                        switch (CUtil.ParseEnum<Arithmetic>(cbArithmeticType.SelectedItem.ToString()))
                        {
                            case Arithmetic.Bitwise_AND:
                                Cv2.BitwiseAnd(ImageRoi, ImageRoi2, ResultImage);
                                break;
                            case Arithmetic.Bitwise_OR:
                                Cv2.BitwiseOr(ImageRoi, ImageRoi2, ResultImage);
                                break;
                            case Arithmetic.Bitwise_XOR:
                                Cv2.BitwiseXor(ImageRoi, ImageRoi2, ResultImage);
                                break;
                            case Arithmetic.Bitwise_NOT:
                                Cv2.BitwiseNot(ImageRoi, ResultImage);
                                break;
                            case Arithmetic.ADD:
                                Cv2.Add(ImageRoi, ImageRoi2, ResultImage);
                                break;
                            case Arithmetic.SUBTRACT:
                                Cv2.Subtract(ImageRoi, ImageRoi2, ResultImage);
                                break;
                            case Arithmetic.MULTIPLY:
                                Cv2.Multiply(ImageRoi, ImageRoi2, ResultImage);
                                break;
                            case Arithmetic.DIVIDE:
                                Cv2.Divide(ImageRoi, ImageRoi2, ResultImage);
                                break;
                            case Arithmetic.MAX:
                                Cv2.Max(ImageRoi, ImageRoi2, ResultImage);
                                break;
                            case Arithmetic.MIN:
                                Cv2.Min(ImageRoi, ImageRoi2, ResultImage);
                                break;
                            case Arithmetic.ABSDIFF:
                                Cv2.Absdiff(ImageRoi, ImageRoi2, ResultImage);
                                break;
                            case Arithmetic.ABS:
                                ResultImage = Cv2.Abs(ImageRoi);
                                break;
                        }

                        Result = CConverter.OverlayImage(CConverter.ToBitmap(ImageCVSource1), CConverter.ToBitmap(ResultImage), r.Left, r.Top);
                    }

                    Displays[GetDisplayIndex(cbLayerList_Dest.SelectedItem.ToString())].ImageView.ib.Image = Result;
                    ibDestination.Image = Result;
                    EventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList_Dest.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void cbArithmeticType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            string Index = (sender as System.Windows.Forms.ComboBox).Text;

            ibSource2.Enabled = false;
            cbLayerList2.Enabled = false;

            switch (CUtil.ParseEnum<Arithmetic>(Index))
            {
                case Arithmetic.Bitwise_AND:
                case Arithmetic.Bitwise_OR:
                case Arithmetic.Bitwise_XOR:
                case Arithmetic.ADD:
                case Arithmetic.SUBTRACT:
                case Arithmetic.MULTIPLY:
                case Arithmetic.DIVIDE:
                case Arithmetic.MAX:
                case Arithmetic.MIN:
                case Arithmetic.ABSDIFF:
                    ibSource2.Enabled = true;
                    cbLayerList2.Enabled = true;
                    break;
                case Arithmetic.Bitwise_NOT:
                case Arithmetic.ABS:
                    ibSource2.Enabled = false;
                    cbLayerList2.Enabled = false;
                    break;
            }

            switch (CUtil.ParseEnum<Arithmetic>(Index))
            {
                case Arithmetic.Bitwise_AND:
                    toolTip1.SetToolTip(tip, "배열과 배열 또는 배열과 스칼라의 요소별 논리곱을 계산한다.\r\n\r\nCv2.BitwiseAnd(원본 배열 1, 원본 배열 2, 결과 배열)로 논리곱을 적용한다.\r\n\r\n수식으로 표현할 경우 dst = src1 & src2;의 형태를 갖는다.");
                    break;
                case Arithmetic.Bitwise_OR:
                    toolTip1.SetToolTip(tip, "배열과 배열 또는 배열과 스칼라의 요소별 논리합을 계산한다.\r\n\r\nCv2.BitwiseOr(원본 배열 1, 원본 배열 2, 결과 배열)로 논리곱을 적용한다.\r\n\r\n수식으로 표현할 경우 dst = src1 | src2;의 형태를 갖는다.");
                    break;
                case Arithmetic.Bitwise_XOR:
                    toolTip1.SetToolTip(tip, "배열과 배열 또는 배열과 스칼라의 요소별 배타적 논리합을 계산한다.\r\n\r\nCv2.BitwiseXor(원본 배열 1, 원본 배열 2, 결과 배열)로 배타적 논리합을 적용한다.\r\n\r\n수식으로 표현할 경우 dst = src1 ^ src2;의 형태를 갖는다.");
                    break;
                case Arithmetic.Bitwise_NOT:
                    toolTip1.SetToolTip(tip, "배열과 배열 또는 배열과 스칼라의 요소별 논리 부정을 계산한다.\r\n\r\nCv2.BitwiseNot(원본 배열, 결과 배열)로 배타적 논리 부정을 적용한다.\r\n\r\n수식으로 표현할 경우 dst = ~src1;의 형태를 갖는다.");
                    break;
                case Arithmetic.ADD:
                    toolTip1.SetToolTip(tip, "덧셈 함수(Cv2.Add)는 배열과 배열 또는 배열과 스칼라의 요소별 합을 계산합니다.\r\n\r\nCv2.Add(원본 배열 1, 원본 배열 2, 결과 배열, 마스크, 반환 형식)로 덧셈을 적용합니다.\r\n\r\n수식으로 표현할 경우 dst = src1 + src2;의 형태를 갖습니다.\r\n\r\n마스크가 null이 아닌 경우, 마스크의 요솟값이 0이 아닌 곳만 연산을 진행합니다.");
                    break;
                case Arithmetic.SUBTRACT:
                    toolTip1.SetToolTip(tip, "뺄셈 함수(Cv2.Subtract)는 배열과 배열 또는 배열과 스칼라의 요소별 차를 계산합니다.\r\n\r\nCv2.Subtract(원본 배열 1, 원본 배열 2, 결과 배열, 마스크, 반환 형식)로 뺄셈을 적용합니다.\r\n\r\n수식으로 표현할 경우 dst = src1 - src2;의 형태를 갖습니다.\r\n\r\n마스크가 null이 아닌 경우, 마스크의 요솟값이 0이 아닌 곳만 연산을 진행합니다.\r\n\r\nTip : src1과 src2의 위치에 따라 결과가 달라지므로 배열의 순서에 유의해야 합니다.");
                    break;
                case Arithmetic.MULTIPLY:
                    toolTip1.SetToolTip(tip, "곱셈 함수(Cv2.Multiply)는 배열과 배열 또는 배열과 스칼라의 요소별 곱을 계산합니다.\r\n\r\nCv2.Multiply(원본 배열 1, 원본 배열 2, 결과 배열, 비율, 반환 형식)로 곱셈을 적용합니다.\r\n\r\n수식으로 표현할 경우 dst = src1 * src2;의 형태를 갖습니다.\r\n\r\n비율이 null이 아닌 경우, 연산에 비율값을 추가로 곱합니다.");
                    break;
                case Arithmetic.DIVIDE:
                    toolTip1.SetToolTip(tip, "나눗셈 함수(Cv2.DIVIDE)는 배열과 배열 또는 배열과 스칼라의 요소별 나눗셈을 계산합니다.\r\n\r\nCv2.Divide(원본 배열 1, 원본 배열 2, 결과 배열, 비율, 반환 형식)로 나눗셈을 적용합니다.\r\n\r\n수식으로 표현할 경우 dst = src1 / src2;의 형태를 갖습니다.\r\n\r\n비율이 null이 아닌 경우, 연산에 비율값을 추가로 곱합니다.");
                    break;
                case Arithmetic.MAX:
                    toolTip1.SetToolTip(tip, "최댓값 함수(Cv2.Max)는 배열과 배열 또는 배열과 스칼라의 요소별 최댓값을 계산합니다.\r\n\r\nCv2.Max(원본 배열 1, 원본 배열 2, 결과 배열)로 최댓값을 적용합니다.\r\n\r\n두 배열의 요소 중 최댓값인 값으로 결과 배열의 요솟값이 할당됩니다.");
                    break;
                case Arithmetic.MIN:
                    toolTip1.SetToolTip(tip, "최솟값 함수(Cv2.Min)는 배열과 배열 또는 배열과 스칼라의 요소별 최솟값을 계산합니다.\r\n\r\nCv2.Min(원본 배열 1, 원본 배열 2, 결과 배열)로 최솟값을 적용합니다.\r\n\r\n두 배열의 요소 중 최솟값인 값으로 결과 배열의 요솟값이 할당됩니다.");
                    break;
                case Arithmetic.ABS:
                    toolTip1.SetToolTip(tip, "절댓값 함수(Cv2.Abs)는 배열의 요소별 절댓값을 계산합니다.\r\n\r\nCv2.Min(원본 배열)로 절댓값을 적용합니다.\r\n\r\n절댓값 함수는 반환 형식이 행렬 표현식(MatExpr 클래스)이며, 매개변수로도 활용할 수 있어 특수한 경우 적절한 연산을 수행할 수 있습니다.");
                    break;
                case Arithmetic.ABSDIFF:
                    toolTip1.SetToolTip(tip, "절댓값 차이 함수(Cv2.Absdiff)는 배열과 배열 또는 배열과 스칼라의 요소별 절댓값 차이을 계산합니다.\r\n\r\nCv2.Absdiff(원본 배열 1, 원본 배열 2, 결과 배열)로 절댓값 차이를 적용합니다.\r\n\r\n덧셈 함수나 뺄셈 함수에서는 두 배열의 요소를 서로 뺄셈했을 때 음수가 발생하면 0을 반환했습니다.\r\n\r\n하지만, 절댓값 차이 함수는 이 값을 절댓값으로 변경해서 양수 형태로 반환합니다.");
                    break;
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
                        case "Source Image 2":
                            gpSourceImage.Enabled = true;
                            gpContrast.Enabled = false;
                            break;
                        case "Contrast":
                            gpSourceImage.Enabled = false;
                            gpContrast.Enabled = true;
                            break;
                        case "Gray Scale":
                            tbGray.Enabled = true;
                            tbR.Enabled = false;
                            tbG.Enabled = false;
                            tbB.Enabled = false;
                            break;
                        case "Color":
                            tbGray.Enabled = false;
                            tbR.Enabled = true;
                            tbG.Enabled = true;
                            tbB.Enabled = true;
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void FormVision_Arithmetic_Click(object sender, EventArgs e)
        {

        }
    }
}

