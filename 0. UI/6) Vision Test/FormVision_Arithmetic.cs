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
        private void InitLayListItem()
        {
            InitializeLayerList(cbLayerList1, source1_Index);
            InitializeLayerList(cbLayerList2, source2_Index);
            InitializeLayerList(cbLayerList_Dest, destination_Index);
        }

        private void IbSource2_MouseClick(object sender, MouseEventArgs e)
        {
            ActivateViewerLayer(source2_Index, true);
        }

        private void IbSource2_ImageChanged(object sender, EventArgs e)
        {
            AcceptUserImageChange(cbLayerList2, ibSource2, index => source2_Index = index);
        }

        private void IbDestination_MouseClick(object sender, MouseEventArgs e)
        {
            ActivateViewerLayer(destination_Index, true);
        }

        private void IbSource1_MouseClick(object sender, MouseEventArgs e)
        {
            ActivateViewerLayer(source1_Index, true);
        }

        private void IbDestination_ImageChanged(object sender, EventArgs e)
        {
            AcceptUserImageChange(cbLayerList_Dest, ibDestination, index => destination_Index = index);
        }

        private void IbSource1_ImageChanged(object sender, EventArgs e)
        {
            AcceptUserImageChange(cbLayerList1, ibSource1, index => source1_Index = index);
        }

        private bool InitEvent()
        {
            return RegisterEscapeClose();
        }

        private void btnNewPanel_Desty_Click(object sender, EventArgs e)
        {
            CreateDestinationLayer(cbLayerList_Dest, InitLayListItem, index => destination_Index = index);
        }

        private void cbLayerList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectLayer(cbLayerList1, source_1, ibSource1, index => source1_Index = index);
        }

        private void cbLayerListDestination_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectLayer(cbLayerList_Dest, destination, ibDestination, index => destination_Index = index);
        }

        private void cbLayerList2_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectLayer(cbLayerList2, source_2, ibSource2, index => source2_Index = index);
        }
        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            InitMenu();
            RegisterEscapeClose();
            ApplyVisionTestCompactStyle();
            InitLayListItem();

            ibSource1.UserImageChanged += IbSource1_ImageChanged;
            ibSource1.MouseClick += IbSource1_MouseClick;

            ibSource2.UserImageChanged += IbSource2_ImageChanged;
            ibSource2.MouseClick += IbSource2_MouseClick;

            ibDestination.UserImageChanged += IbDestination_ImageChanged;
            ibDestination.MouseClick += IbDestination_MouseClick;

            rdoSourceImage.Checked = true;

            DeferInitialViewerLoad(() =>
            {
                ibSource1.DisplayImage = GetLayerImage(DEFINE.Main);
                ibSource2.DisplayImage = GetLayerImage(DEFINE.Main);
                ibDestination.DisplayImage = GetLayerImage(DEFINE.Main);
                ibSource1.ZoomToFit();
                ibSource2.ZoomToFit();
                ibDestination.ZoomToFit();
            }, "arithmetic viewer image load");
        }

        public FormVision_Arithmetic(IDisplayManager displayManager, EventHandler<DockDisplayEventArgs> EventUpdateDisplay)
        {
            InitializeComponent();
            SetDisplayManager(displayManager);
            this.eventUpdateDisplay = EventUpdateDisplay;
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
        private void btnFilterRun_Click(object sender, EventArgs e)
        {

        }
        private void btnArithmeticRun_Click(object sender, EventArgs e)
        {
                        Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (Mat ImageCVSource1 = Lib.Common.BitmapImageConverter.ToMat(ibSource1.DisplayBitmap).Clone())
            using (Mat ImageCVSource2 = Lib.Common.BitmapImageConverter.ToMat(ibSource2.DisplayBitmap).Clone())
            {
                // bgr��
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
                if (displayManager.IsLayerRoiEmpty(source1_Index))
                {
                    if (ImageCVSource1.Channels() != 1) Cv2.CvtColor(ImageCVSource1, ImageCVSource1, ColorConversionCodes.RGB2GRAY);
                    if (ImageCVSource2.Channels() != 1) Cv2.CvtColor(ImageCVSource2, ImageCVSource2, ColorConversionCodes.RGB2GRAY);

                    switch (AppUtil.ParseEnum<Arithmetic>(cbArithmeticType.SelectedItem.ToString()))
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

                    Result = Lib.Common.BitmapImageConverter.ToBitmap(ResultImage);
                }
                else
                {
                    Rect r = CommonConverter.RectangleToRect(GetLayerRoi(source1_Index));
                    Mat ImageRoi = ImageCVSource1.SubMat(r);
                    Mat ImageRoi2 = ImageCVSource2.SubMat(r);

                    switch (AppUtil.ParseEnum<Arithmetic>(cbArithmeticType.SelectedItem.ToString()))
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

                    Result = Lib.Common.BitmapProcessing.OverlayImage(Lib.Common.BitmapImageConverter.ToBitmap(ImageCVSource1), Lib.Common.BitmapImageConverter.ToBitmap(ResultImage), r.Left, r.Top);
                }
                PublishResult(cbLayerList_Dest, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
            }
        
        }

        private void cbArithmeticType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            string Index = (sender as System.Windows.Forms.ComboBox).Text;

            ibSource2.Enabled = false;
            cbLayerList2.Enabled = false;

            switch (AppUtil.ParseEnum<Arithmetic>(Index))
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

            switch (AppUtil.ParseEnum<Arithmetic>(Index))
            {
                case Arithmetic.Bitwise_AND:
                    toolTip1.SetToolTip(tip, "�迭�� �迭 �Ǵ� �迭�� ��Į���� ��Һ� ������� ����Ѵ�.\r\n\r\nCv2.BitwiseAnd(���� �迭 1, ���� �迭 2, ��� �迭)�� ������� �����Ѵ�.\r\n\r\n�������� ǥ���� ��� dst = src1 & src2;�� ���¸� ���´�.");
                    break;
                case Arithmetic.Bitwise_OR:
                    toolTip1.SetToolTip(tip, "�迭�� �迭 �Ǵ� �迭�� ��Į���� ��Һ� ������� ����Ѵ�.\r\n\r\nCv2.BitwiseOr(���� �迭 1, ���� �迭 2, ��� �迭)�� ������� �����Ѵ�.\r\n\r\n�������� ǥ���� ��� dst = src1 | src2;�� ���¸� ���´�.");
                    break;
                case Arithmetic.Bitwise_XOR:
                    toolTip1.SetToolTip(tip, "�迭�� �迭 �Ǵ� �迭�� ��Į���� ��Һ� ��Ÿ�� ������� ����Ѵ�.\r\n\r\nCv2.BitwiseXor(���� �迭 1, ���� �迭 2, ��� �迭)�� ��Ÿ�� ������� �����Ѵ�.\r\n\r\n�������� ǥ���� ��� dst = src1 ^ src2;�� ���¸� ���´�.");
                    break;
                case Arithmetic.Bitwise_NOT:
                    toolTip1.SetToolTip(tip, "�迭�� �迭 �Ǵ� �迭�� ��Į���� ��Һ� ��� ������ ����Ѵ�.\r\n\r\nCv2.BitwiseNot(���� �迭, ��� �迭)�� ��Ÿ�� ��� ������ �����Ѵ�.\r\n\r\n�������� ǥ���� ��� dst = ~src1;�� ���¸� ���´�.");
                    break;
                case Arithmetic.ADD:
                    toolTip1.SetToolTip(tip, "���� �Լ�(Cv2.Add)�� �迭�� �迭 �Ǵ� �迭�� ��Į���� ��Һ� ���� ����մϴ�.\r\n\r\nCv2.Add(���� �迭 1, ���� �迭 2, ��� �迭, ����ũ, ��ȯ ����)�� ������ �����մϴ�.\r\n\r\n�������� ǥ���� ��� dst = src1 + src2;�� ���¸� �����ϴ�.\r\n\r\n����ũ�� null�� �ƴ� ���, ����ũ�� ��ڰ��� 0�� �ƴ� ���� ������ �����մϴ�.");
                    break;
                case Arithmetic.SUBTRACT:
                    toolTip1.SetToolTip(tip, "���� �Լ�(Cv2.Subtract)�� �迭�� �迭 �Ǵ� �迭�� ��Į���� ��Һ� ���� ����մϴ�.\r\n\r\nCv2.Subtract(���� �迭 1, ���� �迭 2, ��� �迭, ����ũ, ��ȯ ����)�� ������ �����մϴ�.\r\n\r\n�������� ǥ���� ��� dst = src1 - src2;�� ���¸� �����ϴ�.\r\n\r\n����ũ�� null�� �ƴ� ���, ����ũ�� ��ڰ��� 0�� �ƴ� ���� ������ �����մϴ�.\r\n\r\nTip : src1�� src2�� ��ġ�� ���� ����� �޶����Ƿ� �迭�� ������ �����ؾ� �մϴ�.");
                    break;
                case Arithmetic.MULTIPLY:
                    toolTip1.SetToolTip(tip, "���� �Լ�(Cv2.Multiply)�� �迭�� �迭 �Ǵ� �迭�� ��Į���� ��Һ� ���� ����մϴ�.\r\n\r\nCv2.Multiply(���� �迭 1, ���� �迭 2, ��� �迭, ����, ��ȯ ����)�� ������ �����մϴ�.\r\n\r\n�������� ǥ���� ��� dst = src1 * src2;�� ���¸� �����ϴ�.\r\n\r\n������ null�� �ƴ� ���, ���꿡 �������� �߰��� ���մϴ�.");
                    break;
                case Arithmetic.DIVIDE:
                    toolTip1.SetToolTip(tip, "������ �Լ�(Cv2.DIVIDE)�� �迭�� �迭 �Ǵ� �迭�� ��Į���� ��Һ� �������� ����մϴ�.\r\n\r\nCv2.Divide(���� �迭 1, ���� �迭 2, ��� �迭, ����, ��ȯ ����)�� �������� �����մϴ�.\r\n\r\n�������� ǥ���� ��� dst = src1 / src2;�� ���¸� �����ϴ�.\r\n\r\n������ null�� �ƴ� ���, ���꿡 �������� �߰��� ���մϴ�.");
                    break;
                case Arithmetic.MAX:
                    toolTip1.SetToolTip(tip, "�ִ� �Լ�(Cv2.Max)�� �迭�� �迭 �Ǵ� �迭�� ��Į���� ��Һ� �ִ��� ����մϴ�.\r\n\r\nCv2.Max(���� �迭 1, ���� �迭 2, ��� �迭)�� �ִ��� �����մϴ�.\r\n\r\n�� �迭�� ��� �� �ִ��� ������ ��� �迭�� ��ڰ��� �Ҵ�˴ϴ�.");
                    break;
                case Arithmetic.MIN:
                    toolTip1.SetToolTip(tip, "�ּڰ� �Լ�(Cv2.Min)�� �迭�� �迭 �Ǵ� �迭�� ��Į���� ��Һ� �ּڰ��� ����մϴ�.\r\n\r\nCv2.Min(���� �迭 1, ���� �迭 2, ��� �迭)�� �ּڰ��� �����մϴ�.\r\n\r\n�� �迭�� ��� �� �ּڰ��� ������ ��� �迭�� ��ڰ��� �Ҵ�˴ϴ�.");
                    break;
                case Arithmetic.ABS:
                    toolTip1.SetToolTip(tip, "���� �Լ�(Cv2.Abs)�� �迭�� ��Һ� ������ ����մϴ�.\r\n\r\nCv2.Min(���� �迭)�� ������ �����մϴ�.\r\n\r\n���� �Լ��� ��ȯ ������ ��� ǥ����(MatExpr Ŭ����)�̸�, �Ű������ε� Ȱ���� �� �־� Ư���� ��� ������ ������ ������ �� �ֽ��ϴ�.");
                    break;
                case Arithmetic.ABSDIFF:
                    toolTip1.SetToolTip(tip, "���� ���� �Լ�(Cv2.Absdiff)�� �迭�� �迭 �Ǵ� �迭�� ��Į���� ��Һ� ���� ������ ����մϴ�.\r\n\r\nCv2.Absdiff(���� �迭 1, ���� �迭 2, ��� �迭)�� ���� ���̸� �����մϴ�.\r\n\r\n���� �Լ��� ���� �Լ������� �� �迭�� ��Ҹ� ���� �������� �� ������ �߻��ϸ� 0�� ��ȯ�߽��ϴ�.\r\n\r\n������, ���� ���� �Լ��� �� ���� �������� �����ؼ� ��� ���·� ��ȯ�մϴ�.");
                    break;
            }
        }

        private void OnPara_CheckedChanged(object sender, EventArgs e)
        {
                        RJCodeUI_M1.RJControls.RJRadioButton rdoButton = (RJCodeUI_M1.RJControls.RJRadioButton)sender;

            if (rdoButton.Checked)
            {
                switch (rdoButton.Text)
                {
                    case "Input B":
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

        private void FormVision_Arithmetic_Click(object sender, EventArgs e)
        {

        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
                        Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (Mat ImageCVSource1 = Lib.Common.BitmapImageConverter.ToMat(ibSource1.DisplayBitmap).Clone())
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

                // �̹��� ī�� ���� ���
                OpenCvSharp.Point ptSrcLt;
                OpenCvSharp.Point ptDstLt;
                OpenCvSharp.Size cpSize;
                ptSrcLt.X = ptOffset.X <= 0 ? 0 : ptOffset.X;
                ptSrcLt.Y = ptOffset.Y <= 0 ? 0 : ptOffset.Y;

                cpSize.Width = ImgSize.Width - Math.Abs(ptOffset.X);
                cpSize.Height = ImgSize.Height - Math.Abs(ptOffset.Y);

                ptDstLt.X = ptOffset.X <= 0 ? -ptOffset.X : 0;
                ptDstLt.Y = ptOffset.Y <= 0 ? -ptOffset.Y : 0;

                // �̹��� ī��                    
                OpenCvSharp.Rect srcRect = new OpenCvSharp.Rect(ptSrcLt.X, ptSrcLt.Y, cpSize.Width, cpSize.Height);
                OpenCvSharp.Rect DstRect = new OpenCvSharp.Rect(ptDstLt.X, ptDstLt.Y, cpSize.Width, cpSize.Height);

                SrcImg[srcRect].CopyTo(DstImg[DstRect]);

                Result = Lib.Common.BitmapImageConverter.ToBitmap(DstImg);
                PublishResult(cbLayerList_Dest, ibDestination, Result, stopwatch.Elapsed.TotalSeconds.ToString() + "s");
            }
        
        }
    }
}



