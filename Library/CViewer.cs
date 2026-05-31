using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing.Drawing2D;
using Cyotek.Windows.Forms;
using OpenVisionLab._1._Core;
using System.Diagnostics;
using System.IO;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventHandler = System.Windows.Forms.MouseEventHandler;
using Point = System.Drawing.Point;
using static OpenVisionLab.DrawObject.CEnum;
using static OpenVisionLab._2._Common.CParameterManager;
using OpenVisionLab.DrawObject;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class CViewer : Component
    {
        private PosSizableRect _MouseOperation = PosSizableRect.None;

        private CRectangleObject _RoiOb = new CRectangleObject();
        private CRectangleObject _TrainOb = new CRectangleObject();
        private CRectangleObject _TempOb = new CRectangleObject();
        private int _ExcuteCount { get; set; } = 0;
        private System.Drawing.Point _StartPt = new System.Drawing.Point(0, 0);
        private System.Drawing.Point _EndPt = new System.Drawing.Point(0, 0);
        private System.Drawing.Point _MouseDown = new System.Drawing.Point(0, 0);

        public bool _ViewCross { get; set; } = false;

        private FormVision_Result _FormVision_Result = new FormVision_Result();

        public RoiMode _Mode { get; set; } = RoiMode.ROI;

        public System.Drawing.Point _Position { get; set; } = new System.Drawing.Point();
        public System.Drawing.Color _Rgb { get; set; } = new System.Drawing.Color();
        public int _GrayValue { get; set; } = 0;

        public ImageBox _Ib = new ImageBox();
        /// <summary>
        /// 마우스의 마지막 위치
        /// </summary>
        private System.Drawing.Point _LastPoint = new System.Drawing.Point(0, 0);

        public Rectangle Roi
        {
            get { return _RoiOb.Roi; }
            set { _RoiOb.Roi = value; }
        }

        public Rectangle TrainROI
        {
            get { return _TrainOb.Roi; }
            set { _TrainOb.Roi = value; }
        }

        private Rectangle TempROI
        {
            get { return _TempOb.Roi; }
            set { _TempOb.Roi = value; }
        }

        public List<CRectangleObject> _RoisOb = new List<CRectangleObject>();

        private int _MinY = 0;
        private int _MaxY = 10000;
        private int _MinX = 0;
        private int _MaxX = 10000;
        private int _SelectROiIndex = 0;
        public bool _ImageChanged = false;
        private bool _OnlyDragMode = false;

        public CViewer(bool bCenter = true)
        {
            InitializeComponent();
            //ddmImageMenu.OwnerIsMenuButton = true;
            //ddmDelete.OwnerIsMenuButton = true;
        }

        public void LoadImageBox(ImageBox ImageBox, bool ControlROI = true, bool onlyDragmode = false)
        {
            _Ib = ImageBox;
            _Ib.MouseWheel += new MouseEventHandler(MouseWheelEvent);
            _Ib.MouseDoubleClick += Ib_MouseDoubleClick;
            _Ib.MouseDown += Ib_MouseDown;
            _Ib.MouseMove += IbSource_MouseMove;
            _Ib.MouseUp += IbSource_MouseUp;
            _Ib.KeyDown += IbSource_KeyDown;
            _Ib.ImageChanged += Ib_ImageChanged;
            _Ib.Paint += Ib_Paint;
            _Ib.AllowDrop = true;
            _Ib.DragEnter += Ib_DragEnter;
            _Ib.DragDrop += new DragEventHandler(Form1_DragDrop);
            _Ib.AllowClickZoom = false;
            _Ib.AllowDoubleClick = true;
            _Ib.SelectionMode = ImageBoxSelectionMode.None;
            _Ib.GridColor = System.Drawing.Color.FromArgb(20, 20, 20);
            _Ib.GridColorAlternate = System.Drawing.Color.FromArgb(20, 20, 20);
            _Ib.HorizontalScroll.Visible = true;
            _Ib.VerticalScroll.Visible = true;

            ItemROI.Click += ItemROI_Click;
            ItemTrainROI.Click += ItemROI_Click;
            ItemMultiROI.Click += ItemROI_Click;
            ItemDrag.Click += ItemROI_Click;

            iconMenuItem7.Click += ItemCollection_Click;
            iconMenuItem8.Click += ItemCollection_Click;

            _Ib.Font = new System.Drawing.Font("Verdana", 20F);
            _Ib.TextAlign = ContentAlignment.TopLeft;
            _Ib.ForeColor = System.Drawing.Color.White;

            this._OnlyDragMode = onlyDragmode;

            SetModeDrag();
        }

        private void Ib_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        public void LoadImageBoxOnlyViewer(ImageBox ImageBox, bool ControlROI = true)
        {
            _Ib = ImageBox;

            for (int i = 0; i < 10; i++) { _Ib.ZoomOut(); }
            _Ib.MouseWheel += new MouseEventHandler(MouseWheelEvent);
            _Ib.MouseDoubleClick += Ib_MouseDoubleClick;
            _Ib.AllowClickZoom = false;
            _Ib.AllowDoubleClick = true;
            _Ib.SelectionMode = ImageBoxSelectionMode.None;

            System.Drawing.Color color = System.Drawing.Color.FromArgb(20, 20, 20);

            _Ib.GridColor = color;
            _Ib.GridColorAlternate = color;
            _Ib.ShowPixelGrid = true;

            _Ib.Font = new System.Drawing.Font("Verdana", 20F);
            _Ib.TextAlign = ContentAlignment.TopLeft;
            _Ib.ForeColor = System.Drawing.Color.White;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);
                switch (extension.ToLower())
                {
                    case ".bmp":
                    case ".exif":
                    case ".gif":
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".tif":
                    case ".tiff":
                        Bitmap Image = new Bitmap(file);
                        _Ib.Image = Image;
                        //this.Image = Image;
                        CDisplayManager.ImageSrc = Lib.Common.CImageConverter.ToMat(Image);
                        _Ib.ZoomToFit();
                        break;
                    default:
                        throw new NotSupportedException(
                            "Unknown file extension " + extension);
                }
            }
        }

        private void ItemCollection_Click(object sender, EventArgs e)
        {
            try
            {
                string strIndex = (sender as ToolStripMenuItem).Text;
                if (_ExcuteCount != 1) { return; }
                ddmImageMenu.Hide();
                switch (strIndex)
                {
                    case "3 Point Measure":
                        if (_Ib.Image.Width != 10 && _Ib.Image.Height != 10)
                        {
                            FormMeasure formMeasure = new FormMeasure((Bitmap)_Ib.Image);
                            formMeasure.TopLevel = true;
                            formMeasure.TopMost = true;
                            formMeasure.StartPosition = FormStartPosition.CenterParent;
                            if (!CUtil.OpenCheckForm(formMeasure)) return;
                            formMeasure.Show();
                        }
                        break;
                    case "Image Compare":
                        FormImageCompare formCompare = new FormImageCompare();
                        formCompare.TopLevel = true;
                        formCompare.TopMost = true;
                        formCompare.StartPosition = FormStartPosition.CenterParent;
                        if (!CUtil.OpenCheckForm(formCompare)) return;
                        formCompare.Show();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
            _ExcuteCount++;
        }

        private void ItemROI_Click(object sender, EventArgs e)
        {
            try
            {
                string strIndex = (sender as FontAwesome.Sharp.IconMenuItem).Text;
                if (_ExcuteCount != 1) { return; }
                ddmImageMenu.Hide();
                ddmDelete.Hide();
                switch (strIndex)
                {
                    case "Drag":
                        SetModeDrag();
                        break;
                    case "ROI":
                        SetModeRoi();
                        break;
                    case "Train ROI":
                        SetModeTrainRoi();
                        break;
                    case "Multi ROI":
                        SetModeMultiRoi();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
            _ExcuteCount++;
        }

        public void SetModeRoi()
        {
            ItemROI.IconChar = FontAwesome.Sharp.IconChar.Check;
            ItemROI.IconColor = System.Drawing.Color.Red;
            ItemTrainROI.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemDrag.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemMultiROI.IconChar = FontAwesome.Sharp.IconChar.None;
            _Mode = RoiMode.ROI;

            _Ib.PanMode = ImageBoxPanMode.Middle;
        }

        public void SetModeTrainRoi()
        {
            ItemROI.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemMultiROI.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemTrainROI.IconColor = System.Drawing.Color.Red;
            ItemDrag.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemTrainROI.IconChar = FontAwesome.Sharp.IconChar.Check;
            _Mode = RoiMode.Train;

            _Ib.PanMode = ImageBoxPanMode.Middle;
        }

        public void SetModeMultiRoi()
        {
            ItemMultiROI.IconColor = System.Drawing.Color.Red;
            ItemMultiROI.IconChar = FontAwesome.Sharp.IconChar.Check;
            ItemTrainROI.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemROI.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemDrag.IconChar = FontAwesome.Sharp.IconChar.None;
            _Mode = RoiMode.MultiROI;

            _Ib.PanMode = ImageBoxPanMode.Middle;
        }

        public void SetModeDrag()
        {
            ItemDrag.IconColor = System.Drawing.Color.Red;
            ItemDrag.IconChar = FontAwesome.Sharp.IconChar.Check;
            ItemTrainROI.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemROI.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemMultiROI.IconChar = FontAwesome.Sharp.IconChar.None;
            _Mode = RoiMode.Drag;

            _Ib.PanMode = ImageBoxPanMode.Both;
        }


        private void Ib_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < 50; i++) { _Ib.ZoomOut(); }
            _Ib.ZoomToFit();
            _Ib.Update();
        }

        private void Ib_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;
            GraphicsState originalState;
            System.Drawing.Size scaledSizeROI;
            System.Drawing.Size originalSizeROI;
            System.Drawing.Size drawSizeROI;
            System.Drawing.Size scaledSizeTempROI;

            System.Drawing.Size scaledSizeTrain;
            System.Drawing.Size originalSizeTrain;
            System.Drawing.Size drawSizeTrain;
            System.Drawing.Size drawSizeTempTrain;

            bool scaleAdornmentSize = true;

            g = e.Graphics;

            originalState = g.Save();

            originalSizeROI = new System.Drawing.Size(Roi.Width, Roi.Height);
            scaledSizeROI = _Ib.GetScaledSize(originalSizeROI);
            drawSizeROI = scaleAdornmentSize ? scaledSizeROI : originalSizeROI;

            originalSizeTrain = new System.Drawing.Size(TrainROI.Width, TrainROI.Height);
            scaledSizeTrain = _Ib.GetScaledSize(originalSizeTrain);
            drawSizeTrain = scaleAdornmentSize ? scaledSizeTrain : originalSizeTrain;

            scaledSizeTempROI = new System.Drawing.Size(TempROI.Width, TempROI.Height);
            drawSizeTempTrain = _Ib.GetScaledSize(scaledSizeTempROI);

            System.Drawing.Point locationROI;
            System.Drawing.Point locationTrain;
            System.Drawing.Point locationTemp;

            // Work out the location of the marker graphic according to the current zoom level and scroll offset
            locationROI = _Ib.GetOffsetPoint(Roi.X, Roi.Y);
            locationTrain = _Ib.GetOffsetPoint(TrainROI.X, TrainROI.Y);
            locationTemp = _Ib.GetOffsetPoint(TempROI.X, TempROI.Y);

            for (int i = 0; i < _RoisOb.Count; i++)
            {
                System.Drawing.Point Location2 = _Ib.GetOffsetPoint(_RoisOb[i].Roi.X, _RoisOb[i].Roi.Y);
                System.Drawing.Size scaledSize2 = _Ib.GetScaledSize(_RoisOb[i].Roi.Width, _RoisOb[i].Roi.Height);
                System.Drawing.Color color = i != _SelectROiIndex ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                _RoisOb[i].SetParameter(color, scaledSize2, Location2, false, (i + 1).ToString());
                _RoisOb[i].Draw(g);
            }

            _TempOb.SetParameter(System.Drawing.Color.Green, drawSizeTempTrain, locationTemp, false, "");
            _TempOb.Draw(g);

            _RoiOb.SetParameter(System.Drawing.Color.Blue, drawSizeROI, locationROI, true, "");
            _RoiOb.Draw(g);

            _TrainOb.SetParameter(System.Drawing.Color.Red, drawSizeTrain, locationTrain, true, "");
            _TrainOb.Draw(g);

            if (_ViewCross)
            {
                System.Drawing.Point CrossLocationVerStart = _Ib.GetOffsetPoint(0, (_Ib.Image.Height / 2));
                System.Drawing.Point CrossLocationVerEnd = _Ib.GetOffsetPoint(_Ib.Image.Width, (_Ib.Image.Height / 2));
                System.Drawing.Point CrossLocationHorStart = _Ib.GetOffsetPoint((_Ib.Image.Width / 2), 0);
                System.Drawing.Point CrossLocationHorEnd = _Ib.GetOffsetPoint((_Ib.Image.Width / 2), _Ib.Image.Height);

                g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Yellow, 3), CrossLocationVerStart, CrossLocationVerEnd);
                g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Yellow, 3), CrossLocationHorStart, CrossLocationHorEnd);
            }

            g.Restore(originalState);
        }

        private void IbSource_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D1:
                    SetModeDrag();
                    break;
                case Keys.D2:
                    SetModeRoi();
                    break;
                case Keys.D3:
                    SetModeTrainRoi();
                    break;
                case Keys.D4:
                    SetModeMultiRoi();
                    break;

                case Keys.ShiftKey:
                case Keys.ControlKey:
                    // ib.PanMode = ImageBoxPanMode.Middle;
                    break;
                case Keys.Enter:
                    switch (_Mode)
                    {
                        case RoiMode.ROI:
                            if (!Roi.IsEmpty)
                            {
                                using (Bitmap Image = new Bitmap(_Ib.Image))
                                {
                                    Bitmap Crop = Lib.Common.CBitmapProcessing.CropAtRect(Image, Roi).Result;
                                    CDisplayManager.CreatePanel(Crop);
                                    ClearROI();
                                }

                            }
                            break;
                        case RoiMode.Train:
                            if (!TrainROI.IsEmpty)
                            {
                                using (Bitmap Image = new Bitmap(_Ib.Image))
                                {
                                    Bitmap Crop = Lib.Common.CBitmapProcessing.CropAtRect(Image, TrainROI).Result;
                                    CDisplayManager.CreatePanel(Crop);
                                    ClearROI();
                                }
                            }
                            break;
                    }

                    break;
                case Keys.Delete:
                    ClearROI();
                    break;
            }
        }

        private void ClearROI()
        {
            switch (_Mode)
            {
                case RoiMode.ROI:
                    Roi = new Rectangle();
                    break;
                case RoiMode.Train:
                    TrainROI = new Rectangle();
                    break;
                case RoiMode.MultiROI:
                    _RoisOb.RemoveAt(_SelectROiIndex);
                    break;
            }
            _MouseDown = Point.Empty;
            _StartPt = new System.Drawing.Point();
            _EndPt = new System.Drawing.Point();

            _Ib.Invalidate();
        }

        private void Ib_ImageChanged(object sender, EventArgs e) => _ImageChanged = true;

        private void Ib_MouseDown(object sender, MouseEventArgs e)
        {
            SettingParameter();
            UnSelectAll();
            _StartPt = _Ib.PointToImage(e.Location);
            switch (_Mode)
            {
                case RoiMode.ROI:
                    _MouseOperation = _RoiOb.GetNodeSelectable(_StartPt, Roi, _Ib);
                    _RoiOb.Selected = true;
                    break;
                case RoiMode.Train:
                    _MouseOperation = _TrainOb.GetNodeSelectable(_StartPt, TrainROI, _Ib);
                    _TrainOb.Selected = true;
                    break;
                case RoiMode.MultiROI:
                    _MouseOperation = _TempOb.GetNodeSelectable(_StartPt, TempROI, _Ib);
                    for (int i = 0; i < _RoisOb.Count; i++)
                    {
                        _MouseOperation = _RoisOb[i].GetNodeSelectable(_StartPt, _RoisOb[i].Roi, _Ib);
                        if (_MouseOperation != PosSizableRect.None)
                        {
                            _SelectROiIndex = i;
                            _RoisOb[i].Selected = true;
                            break;
                        }
                    }

                    break;
            }

            _LastPoint = _Ib.PointToImage(e.Location);
            _MouseDown = _Ib.PointToImage(e.Location);
        }

        private void IbSource_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Ib.Image == null) { return; }
            if (_Ib.Image.Width == 10 || _Ib.Image.Width == 10) { return; }

            _Position = _Ib.PointToImage(e.Location);
            GetPixelData(_Position);
            ChangeCursor();

            if (e.Button == MouseButtons.Left)
            {
                if (_MouseOperation == PosSizableRect.Rotate)
                {
                    switch (_Mode)
                    {
                        case RoiMode.ROI:
                            _RoiOb.CalculatorAngle(_MouseDown, _Position);
                            break;
                        case RoiMode.Train:
                            _TrainOb.CalculatorAngle(_MouseDown, _Position);
                            break;
                    }
                }

                //마우스의 현재 위치에서 마지막 위치를 뺀 값을 저장한다.
                int distanceX = _Position.X - _LastPoint.X;
                int distanceY = _Position.Y - _LastPoint.Y;

                _LastPoint = _Position;

                switch (_Mode)
                {
                    case RoiMode.ROI:
                        SetToRectangle(e, ref _RoiOb.Roi);
                        _RoiOb.MoveHandleTo(_Position, _MouseOperation);
                        if (_MouseOperation == PosSizableRect.SizeAll)
                        {
                            _RoiOb.Move(distanceX, distanceY);
                        }
                        break;
                    case RoiMode.Train:
                        SetToRectangle(e, ref _TrainOb.Roi);
                        _TrainOb.MoveHandleTo(_Position, _MouseOperation);
                        if (_MouseOperation == PosSizableRect.SizeAll)
                        {
                            _TrainOb.Move(distanceX, distanceY);
                        }
                        break;

                    case RoiMode.MultiROI:

                        if (_MouseOperation == PosSizableRect.None)
                        {
                            SetToRectangle(e, ref _TempOb.Roi);
                            _TempOb.MoveHandleTo(_Position, _MouseOperation);
                        }


                        if (_RoisOb.Count > _SelectROiIndex)
                        {
                            if (_MouseOperation != PosSizableRect.None)
                            {                                
                                SetToRectangle(e, ref _RoisOb[_SelectROiIndex].Roi);
                                _RoisOb[_SelectROiIndex].MoveHandleTo(_Position, _MouseOperation);
                                if (_MouseOperation == PosSizableRect.SizeAll)
                                {
                                    _RoisOb[_SelectROiIndex].Move(distanceX, distanceY);
                                }
                            }
                        }
                        break;
                }
            }
            _Ib.Invalidate();
        }

        private void ChangeCursor()
        {
            switch (_Mode)
            {
                case RoiMode.ROI:
                    _Ib.Cursor = _RoiOb.ChangeCursor(_Position, Roi, _Ib);
                    break;
                case RoiMode.Train:
                    _Ib.Cursor = _TrainOb.ChangeCursor(_Position, TrainROI, _Ib);
                    break;
                case RoiMode.MultiROI:
                    if (_RoisOb.Count > _SelectROiIndex) { _Ib.Cursor = _RoisOb[_SelectROiIndex].ChangeCursor(_Position, _RoisOb[_SelectROiIndex].Roi, _Ib); }
                    break;
            }
        }

        /// <summary>
        /// 그려진 모든 DrawObject의 선택을 해제한다.
        /// </summary>
        private void UnSelectAll()
        {
            _RoiOb.Selected = false;
            _TrainOb.Selected = false;

            for (int i = 0; i < _RoisOb.Count; i++)
            {
                _RoisOb[i].Selected = false;
            }
        }

        private void SettingParameter()
        {
            if (_Ib.Image == null) { return; }
            _MaxX = _Ib.Image.Width;
            _MaxY = _Ib.Image.Height;
            _RoiOb._MaxX = _Ib.Image.Width;
            _RoiOb._MaxY = _Ib.Image.Height;
            _TrainOb._MaxX = _Ib.Image.Width;
            _TrainOb._MaxY = _Ib.Image.Height;
            _TempOb._MaxX = _Ib.Image.Width;
            _TempOb._MaxY = _Ib.Image.Height;
            _TempOb.OriginalSize = new System.Drawing.Size(this._Ib.Image.Width, this._Ib.Image.Height);
            _RoiOb.OriginalSize = new System.Drawing.Size(this._Ib.Image.Width, this._Ib.Image.Height);
            _TrainOb.OriginalSize = new System.Drawing.Size(this._Ib.Image.Width, this._Ib.Image.Height);

            for (int i = 0; i < _RoisOb.Count; i++)
            {
                _RoisOb[i]._MaxX = _Ib.Image.Width;
                _RoisOb[i]._MaxY = _Ib.Image.Height;
                _RoisOb[i].OriginalSize = new System.Drawing.Size(this._Ib.Image.Width, this._Ib.Image.Height);
            }
        }

        private void GetPixelData(System.Drawing.Point _POSITION)
        {
            if (_POSITION.X > 0 && _POSITION.Y > 0 && _POSITION.X < _Ib.Image.Width && _POSITION.Y < _Ib.Image.Height)
            {
                Bitmap Image = (Bitmap)_Ib.Image;
                _Rgb = Image.GetPixel(_POSITION.X, _POSITION.Y);
                _GrayValue = (_Rgb.R + _Rgb.G + _Rgb.B) / 3;
            }
        }

        private void IbSource_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        switch (_Mode)
                        {
                            case RoiMode.ROI:
                                if (this.Roi.IsEmpty || this.Roi.Width < 15 || this.Roi.Height < 15) { this.Roi = new Rectangle(); }
                                CRectangle Roi = new CRectangle();
                                Roi.Roi = new Rectangle(this.Roi.X, this.Roi.Y, this.Roi.Width, this.Roi.Height);
                                Roi.Index = 1;
                                //_formVision_Result.SetBindingRoi(Roi);
                                break;
                            case RoiMode.Train:
                                if (TrainROI.IsEmpty || TrainROI.Width < 15 || TrainROI.Height < 15) { TrainROI = new Rectangle(); }
                                CRectangle TrainRoi = new CRectangle();
                                TrainRoi.Roi = new Rectangle(TrainROI.X, TrainROI.Y, TrainROI.Width, TrainROI.Height);
                                TrainRoi.Index = 1;
                                // _formVision_Result.SetBindingRoi(TrainRoi);

                                break;
                            case RoiMode.MultiROI:
                                if (!TempROI.IsEmpty && TempROI.Width > 15 && TempROI.Height > 15)
                                {
                                    CRectangleObject rectangleObject = new CRectangleObject();
                                    rectangleObject.Roi = TempROI;
                                    _RoisOb.Add(rectangleObject);

                                    _SelectROiIndex = _RoisOb.Count - 1;
                                }
                                TempROI = new Rectangle();
                                break;
                        }
                        break;
                    case MouseButtons.Right:
                        _ExcuteCount = 1;
                        if (_MouseOperation == PosSizableRect.SizeAll) { Open_DropdownMenu(ddmDelete, sender, e); }
                        else { Open_DropdownMenu(ddmImageMenu, sender, e); }
                        break;
                }
                _MouseOperation = PosSizableRect.None;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void SetToRectangle(MouseEventArgs e, ref Rectangle ROI)
        {
            if (e.Button == MouseButtons.Left)
            {
                _EndPt = _Ib.PointToImage(e.Location);
                if (_MouseOperation != PosSizableRect.None) { return; }
                if (_MouseOperation == PosSizableRect.SizeAll) { return; }

                OpenCvSharp.Point ptStart = new OpenCvSharp.Point(_StartPt.X, _StartPt.Y);
                OpenCvSharp.Point ptEnd = new OpenCvSharp.Point(_EndPt.X, _EndPt.Y);

                if (ptStart.X > ptEnd.X)
                {
                    if (ptStart.Y < ptEnd.Y) { ROI = new Rectangle(ptEnd.X, ptStart.Y, ptStart.X - ptEnd.X, ptEnd.Y - ptStart.Y); }
                    else { ROI = new Rectangle(ptEnd.X, ptEnd.Y, ptStart.X - ptEnd.X, ptStart.Y - ptEnd.Y); }
                }
                else
                {
                    if (ptStart.Y < ptEnd.Y)
                    {
                        if (ptStart.X < ptEnd.X) { ROI = new Rectangle(ptStart.X, ptStart.Y, ptEnd.X - ptStart.X, ptEnd.Y - ptStart.Y); }
                        else { ROI = new Rectangle(ptStart.X, ptStart.Y, ptEnd.X - ptStart.X, ptEnd.Y - ptStart.Y); }
                    }
                    else
                    {
                        if (ptStart.X < ptEnd.X) { ROI = new Rectangle(ptStart.X, ptEnd.Y, ptEnd.X - ptStart.X, ptStart.Y - ptEnd.Y); }
                        else { ROI = new Rectangle(ptEnd.X, ptEnd.Y, ptStart.X - ptEnd.X, ptStart.Y - ptEnd.Y); }
                    }
                }

                if (ROI.Y < _MinY) ROI = new Rectangle(ROI.X, _MinY, ROI.Width, ROI.Height);
                if (ROI.X < _MinX) ROI = new Rectangle(_MinX, ROI.Y, ROI.Width, ROI.Height);
                if (ROI.Bottom > _MaxY)
                {
                    int Max_H = _MaxY - ROI.Y;
                    ROI = new Rectangle(ROI.X, ROI.Y, ROI.Width, Max_H);
                }
                if (ROI.Right > _MaxX)
                {
                    int Max_W = _MaxX - ROI.X;
                    ROI = new Rectangle(ROI.X, ROI.Y, Max_W, ROI.Height);
                }
            }
        }

        private void ImageMenuClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                if (_ExcuteCount != 1) { return; }
                ddmImageMenu.Hide();
                ddmDelete.Hide();
                switch (e.ClickedItem.Text)
                {
                    case "Image Load":
                        string ImagePath = CUtil.LoadImageFilePath();

                        if (ImagePath != "")
                        {
                            Bitmap Image = new Bitmap(ImagePath);
                            _Ib.Image = Image;
                            //this.Image = Image;
                            CDisplayManager.ImageSrc = Lib.Common.CImageConverter.ToMat(Image);
                            _Ib.ZoomToFit();
                        }
                        break;
                    case "Image Save":
                        if (_Ib.Image.Width != 10 && _Ib.Image.Height != 10)
                        {
                            ImagePath = CUtil.SaveImageFilePath();

                            if (ImagePath != "") { _Ib.Image.Save(ImagePath); }
                        }
                        break;
                    case "Show Folder":
                        Process.Start(Application.StartupPath + "\\");
                        break;
                    case "CROSS":
                        _ViewCross = !_ViewCross;
                        break;
                    case "Delete":
                        ClearROI();
                        break;
                    case "Roi List":
                        _FormVision_Result = new FormVision_Result();
                        _FormVision_Result.StartPosition = FormStartPosition.CenterScreen;
                        if (!CUtil.OpenCheckForm(_FormVision_Result)) return;
                        switch (_Mode)
                        {
                            case RoiMode.ROI:
                                CRectangle Roi = new CRectangle();
                                Roi.Roi = new Rectangle(this.Roi.X, this.Roi.Y, this.Roi.Width, this.Roi.Height);
                                Roi.Index = 1;
                                _FormVision_Result.SetBindingRoi(Roi);
                                break;
                            case RoiMode.Train:
                                CRectangle TrainRoi = new CRectangle();
                                TrainRoi.Roi = new Rectangle(TrainROI.X, TrainROI.Y, TrainROI.Width, TrainROI.Height);
                                TrainRoi.Index = 1;
                                _FormVision_Result.SetBindingRoi(TrainRoi);
                                break;
                            case RoiMode.MultiROI:
                                List<CRectangle> Rois = new List<CRectangle>();
                                for (int i= 0; i < _RoisOb.Count; i++)
                                {
                                    CRectangle cRectangle = new CRectangle();
                                    cRectangle.Index = i + 1;
                                    cRectangle.Roi = _RoisOb[i].Roi;
                                    Rois.Add(cRectangle);
                                }
                                _FormVision_Result.SetBindingRois(Rois);
                                break;
                        }
                        _FormVision_Result.Show();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
            _ExcuteCount++;
        }


        private void Open_DropdownMenu(RJCodeUI_M1.RJControls.RJDropdownMenu dropdownMenu, object sender, MouseEventArgs e)
        {
            Control control = (Control)sender;

            dropdownMenu.VisibleChanged += new EventHandler((sender2, ev)
             => DropdownMenu_VisibleChanged(sender2, ev, control));
            dropdownMenu.ItemClicked += new ToolStripItemClickedEventHandler(ImageMenuClicked);
            dropdownMenu.Show(control, e.X, e.Y);
        }

        private void DropdownMenu_VisibleChanged(object sender, EventArgs e, Control ctrl)
        {
            RJCodeUI_M1.RJControls.RJDropdownMenu dropdownMenu = (RJCodeUI_M1.RJControls.RJDropdownMenu)sender;
            if (!DesignMode)
            {
                if (dropdownMenu.Visible)
                    ctrl.BackColor = DEFINE.MOUSEHOVER_COLOR;
                else ctrl.BackColor = System.Drawing.Color.FromArgb(49, 42, 81);
            }
        }

        #region ImageBox
        private void MouseWheelEvent(object sender, MouseEventArgs e)
        {
            if ((e.Delta / 120) > 0) { ZoomInImage(); }
            else { ZoomOutImage(); }
        }

        #region Display
        private void ZoomInImage() => _Ib.ZoomIn();
        private void ZoomOutImage() => _Ib.ZoomOut();
        #endregion

        #endregion
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this._OnlyDragMode)
            {
                _Mode = RoiMode.Drag;
                _Ib.Text = "";
                return;
            }

            switch (_Mode)
            {
                case RoiMode.Drag:
                    _Ib.Text = "DRAG MODE";
                    break;
                case RoiMode.ROI:
                    _Ib.Text = "ROI MODE";
                    break;
                case RoiMode.MultiROI:
                    _Ib.Text = "MULTI ROI MODE";
                    break;
                case RoiMode.Train:
                    _Ib.Text = "TRAIN ROI MODE";
                    break;
            }
        }
    }
}
