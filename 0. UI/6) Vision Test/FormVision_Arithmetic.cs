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
    public partial class FormVision_Arithmetic : VisionTestForm
    {                        
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
            this.displays = Displays;
            this.eventUpdateDisplay = EventUpdateDisplay;
            panelCount = Displays.Count;
            timer1.Enabled = true;
            timer1.Start();
        }

        private void InitLayListItem()
        {
            cbLayerList1.Items.Clear();
            for (int i = 0; i < displays.Count; i++) { cbLayerList1.Items.Add(displays[i].Text); }
            cbLayerList1.SelectedIndex = source1_Index;

            cbLayerList2.Items.Clear();
            for (int i = 0; i < displays.Count; i++) { cbLayerList2.Items.Add(displays[i].Text); }
            cbLayerList2.SelectedIndex = source2_Index;

            cbLayerList_Dest.Items.Clear();
            for (int i = 0; i < displays.Count; i++) { cbLayerList_Dest.Items.Add(displays[i].Text); }
            cbLayerList_Dest.SelectedIndex = destination_Index;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            InitMenu();
            InitEvent();
            InitLayListItem();
            source_1.LoadImageBox(ibSource1, false);
            source_2.LoadImageBox(ibSource2, false);
            destination.LoadImageBox(ibDestination, false);

            ibSource1.Image = (Bitmap)displays[DEFINE.Main].viewer._Ib.Image;
            ibSource1.ImageChanged += IbSource1_ImageChanged;
            ibSource1.MouseClick += IbSource1_MouseClick;
            ibSource1.ZoomToFit();

            ibSource2.Image = (Bitmap)displays[DEFINE.Main].viewer._Ib.Image;
            ibSource2.ImageChanged += IbSource2_ImageChanged;
            ibSource2.MouseClick += IbSource2_MouseClick;
            ibSource2.ZoomToFit();

            ibDestination.Image = (Bitmap)displays[DEFINE.Main].viewer._Ib.Image;
            ibDestination.ImageChanged += IbDestination_ImageChanged;
            ibDestination.MouseClick += IbDestination_MouseClick;
            ibDestination.ZoomToFit();

            this.GotFocus += FormVision_Arithmetic_GotFocus;

            rdoSourceImage.Checked = true;            
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
            displays[source2_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
            displays[source2_Index].ibSource.ZoomToFit();
        }

        private void IbSource2_ImageChanged(object sender, EventArgs e)
        {
            source2_Index = cbLayerList2.SelectedIndex;
            displays[source2_Index].ibSource.Image = (Bitmap)ibSource2.Image;
        }

        private void IbDestination_MouseClick(object sender, MouseEventArgs e)
        {
            displays[destination_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
            displays[destination_Index].ibSource.ZoomToFit();
        }

        private void IbSource1_MouseClick(object sender, MouseEventArgs e)
        {
            displays[source1_Index].Activate();
            this.Focus();
            this.TopLevel = true;
            this.TopMost = true;
            displays[source1_Index].ibSource.ZoomToFit();
        }

        private void IbDestination_ImageChanged(object sender, EventArgs e)
        {
            destination_Index = cbLayerList_Dest.SelectedIndex;
            displays[destination_Index].ibSource.Image = (Bitmap)ibDestination.Image;
        }

        private void IbSource1_ImageChanged(object sender, EventArgs e)
        {
            source1_Index = cbLayerList1.SelectedIndex;
            displays[source1_Index].ibSource.Image = (Bitmap)ibSource1.Image;
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

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }

        private void btnNewPanel_Desty_Click(object sender, EventArgs e)
        {
            CDisplayManager.CreatePanel();
            InitLayListItem();
            destination_Index = cbLayerList_Dest.Items.Count - 1;
            cbLayerList_Dest.SelectedIndex = destination_Index;
        }

        private void cbLayerList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            source1_Index = cbLayerList1.SelectedIndex;
            source_1._Ib.Image =
            ibSource1.Image = (Bitmap)displays[source1_Index].ibSource.Image;
        }

        private void cbLayerListDestination_SelectedIndexChanged(object sender, EventArgs e)
        {
            destination_Index = cbLayerList_Dest.SelectedIndex;
            ibDestination.Image = (Bitmap)displays[destination_Index].ibSource.Image;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (panelCount != displays.Count)
                {
                    panelCount = displays.Count;
                    InitLayListItem();
                }

                if (source_1.Roi != displays[source1_Index].viewer.Roi || source_1.TrainROI != displays[source1_Index].viewer.TrainROI)
                {
                    ibSource1.Invalidate();
                }
                source_1.Roi = displays[source1_Index].viewer.Roi;
                source_1.TrainROI = displays[source1_Index].viewer.TrainROI;

                if (source_2.Roi != displays[source2_Index].viewer.Roi || source_2.TrainROI != displays[source2_Index].viewer.TrainROI)
                {
                    ibSource2.Invalidate();
                }
                source_2.Roi = displays[source2_Index].viewer.Roi;
                source_2.TrainROI = displays[source2_Index].viewer.TrainROI;

                if (destination.Roi != displays[destination_Index].viewer.Roi || destination.TrainROI != displays[destination_Index].viewer.TrainROI)
                {
                    ibDestination.Invalidate();
                }
                destination.Roi = displays[destination_Index].viewer.Roi;
                destination.TrainROI = displays[destination_Index].viewer.TrainROI;

                cbLayerList1_SelectedIndexChanged(null, null);
                cbLayerList2_OnSelectedIndexChanged(null, null);
                cbLayerListDestination_SelectedIndexChanged(null, null);
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnFilterRun_Click(object sender, EventArgs e)
        {

        }

        private void cbLayerList2_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            source2_Index = cbLayerList2.SelectedIndex;
            ibSource2.Image = (Bitmap)displays[source2_Index].ibSource.Image;
        }

        private void btnArithmeticRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource1 = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource1.Image).Clone())
                using (Mat ImageCVSource2 = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource2.Image).Clone())
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
                    if (displays[source1_Index].viewer.Roi.IsEmpty)
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

                        Result = Lib.Common.CImageConverter.ToBitmap(ResultImage);
                    }
                    else
                    {
                        Rect r = CConverter.RectangleToRect(displays[source1_Index].viewer.Roi);
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

                        Result = Lib.Common.CBitmapProcessing.OverlayImage(Lib.Common.CImageConverter.ToBitmap(ImageCVSource1), Lib.Common.CImageConverter.ToBitmap(ResultImage), r.Left, r.Top);
                    }

                    displays[GetDisplayIndex(cbLayerList_Dest.SelectedItem.ToString())].viewer._Ib.Image = Result;
                    ibDestination.Image = Result;
                    eventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList_Dest.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
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
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void FormVision_Arithmetic_Click(object sender, EventArgs e)
        {

        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource1 = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource1.Image).Clone())
                {
                    Mat val = new Mat();

                    if (tbX.Text == "") { tbX.Text = "1"; }
                    if (tbY.Text == "") { tbY.Text = "1"; }

                    int x = int.Parse(tbX.Text);
                    int y = int.Parse(tbY.Text);

                    Mat ResultImage = new Mat();
                    Bitmap Result = new Bitmap(10, 10);
                    if (ImageCVSource1.Channels() != 1) Cv2.CvtColor(ImageCVSource1, ImageCVSource1, ColorConversionCodes.RGB2GRAY);

                    OpenCvSharp.Point ptOffset = new OpenCvSharp.Point(x, y);
                    Mat SrcImg = ImageCVSource1;
                    Mat DstImg = new Mat(SrcImg.Size(), SrcImg.Type());
                    OpenCvSharp.Size ImgSize = new OpenCvSharp.Size(SrcImg.Width, SrcImg.Height);

                    // 이미지 카피 영역 계산
                    OpenCvSharp.Point ptSrcLt;
                    OpenCvSharp.Point ptDstLt;
                    OpenCvSharp.Size cpSize;
                    ptSrcLt.X = ptOffset.X <= 0 ? 0 : ptOffset.X;
                    ptSrcLt.Y = ptOffset.Y <= 0 ? 0 : ptOffset.Y;

                    cpSize.Width = ImgSize.Width - Math.Abs(ptOffset.X);
                    cpSize.Height = ImgSize.Height - Math.Abs(ptOffset.Y);

                    ptDstLt.X = ptOffset.X <= 0 ? -ptOffset.X : 0;
                    ptDstLt.Y = ptOffset.Y <= 0 ? -ptOffset.Y : 0;

                    // 이미지 카피                    
                    OpenCvSharp.Rect srcRect = new OpenCvSharp.Rect(ptSrcLt.X, ptSrcLt.Y, cpSize.Width, cpSize.Height);
                    OpenCvSharp.Rect DstRect = new OpenCvSharp.Rect(ptDstLt.X, ptDstLt.Y, cpSize.Width, cpSize.Height);

                    SrcImg[srcRect].CopyTo(DstImg[DstRect]);

                    Result = Lib.Common.CImageConverter.ToBitmap(DstImg);

                    displays[GetDisplayIndex(cbLayerList_Dest.SelectedItem.ToString())].viewer._Ib.Image = Result;
                    ibDestination.Image = Result;
                    eventUpdateDisplay(null, new DockDisplayEventArgs(Result, GetDisplayIndex(cbLayerList_Dest.SelectedItem.ToString()), stopwatch.Elapsed.TotalSeconds.ToString() + "s"));
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
    }
}

