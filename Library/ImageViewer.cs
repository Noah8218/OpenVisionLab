using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing.Drawing2D;
using Cyotek.Windows.Forms;
using RJCodeUI_M1.RJControls;
using KtemVisionSystem._1._Core;
using System.Diagnostics;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.IO;

namespace KtemVisionSystem
{
    public partial class KtemViewer : Component
    {
        private float m_fImageScale { get; set; } = 5;    
        private bool m_bCenter { get; set; } = true;
        private bool bUseControlMoveCell { get; set; } = false;
        private bool ControlROI { get; set; } = true;

        public enum RoiMode
        {
            ROI,
            Train,
            MultiROI
        }

        public RoiMode Mode { get; set; } = RoiMode.ROI;

        public System.Drawing.Point POSITION { get; set; } = new System.Drawing.Point();
        public System.Drawing.Color RGB { get; set; } = new System.Drawing.Color();
        public int GrayValue { get; set; } = 0;

        public ImageBox ib = new ImageBox();
        private int ExcuteCount { get; set; } = 0;        
        private Bitmap Image = new Bitmap(10, 10);        
        private System.Drawing.Point _startPt = new System.Drawing.Point(0, 0);
        private System.Drawing.Point _endPt = new System.Drawing.Point(0, 0);

        public Rectangle ROI = new Rectangle();
        public Rectangle TrainROI = new Rectangle();
        public List<CRectangle> ROIs = new List<CRectangle>();
        public Rectangle TempROI = new Rectangle();

        private PosSizableRect nodeSelectedROI = PosSizableRect.None;        

        private int oldX;
        private int oldY;

        private int _MinY = 0;
        private int _MaxY = 540;
        private int _MinX = 0;
        private int _MaxX = 720;        
        private bool mMove = false;
        private int SelectROiIndex = 0;
        public bool ImageChanged = false;

        private enum PosSizableRect
        {
            UpMiddle,
            LeftMiddle,
            LeftBottom,
            LeftUp,
            RightUp,
            RightMiddle,
            RightBottom,
            BottomMiddle,
            None

        };

        public KtemViewer(bool bCenter = true)
        {
            InitializeComponent();
            m_bCenter = bCenter;
            //ddmImageMenu.IsMainMenu = true;
            ddmImageMenu.OwnerIsMenuButton = true;
        }

        public void LoadImageBox(ImageBox ImageBox, bool ControlROI = true)
        {
            ib = ImageBox; 
            ib.MouseWheel += new MouseEventHandler(MouseWheelEvent);
            ib.MouseDoubleClick += Ib_MouseDoubleClick;
            ib.MouseDown += Ib_MouseDown;
            ib.MouseMove += IbSource_MouseMove;
            ib.MouseHover += Ib_MouseHover;
            ib.MouseUp += IbSource_MouseUp;
            ib.KeyDown += IbSource_KeyDown;
            ib.KeyUp += IbSource_KeyUp;
            ib.ImageChanged += Ib_ImageChanged;
            ib.Paint += Ib_Paint;
            ib.AllowDrop = true;
            ib.DragEnter += Ib_DragEnter;
            ib.DragDrop += new DragEventHandler(Form1_DragDrop);
            ib.AllowClickZoom = false;
            ib.AllowDoubleClick = true;
            ib.SelectionMode = ImageBoxSelectionMode.None;
            ib.GridColor = System.Drawing.Color.FromArgb(20, 20, 20);
            ib.GridColorAlternate = System.Drawing.Color.FromArgb(20, 20, 20);      
            ib.ShowPixelGrid = true;
            this.ControlROI = ControlROI;

            ItemROI.Click += ItemROI_Click;
            ItemTrainROI.Click += ItemROI_Click;
            ItemMultiROI.Click += ItemROI_Click;

            toolStripMenuItem3.Click += ItemCollection_Click;
            toolStripMenuItem4.Click += ItemCollection_Click;

            ItemROI.IconChar = FontAwesome.Sharp.IconChar.Check;
            ItemROI.IconColor = System.Drawing.Color.Red;
            ItemTrainROI.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemMultiROI.IconChar = FontAwesome.Sharp.IconChar.None;
            Mode = RoiMode.ROI;
        }

        private void Ib_MouseHover(object sender, EventArgs e)
        {                        
            
        }

        private void Ib_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
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
                        ib.Image = Image;
                        this.Image = Image;
                        CDisplayManager.ImageSrc = CConverter.ToMat(Image);
                        ib.ZoomToFit();
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
                if (ExcuteCount != 1) { return; }
                ddmImageMenu.Hide();
                switch (strIndex)
                {
                    case "3 Point Measure":
                        if (ib.Image.Width != 10 && ib.Image.Height != 10)
                        {
                            FormMeasure formMeasure = new FormMeasure((Bitmap)ib.Image);
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
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            ExcuteCount++;
        }


        private void ItemROI_Click(object sender, EventArgs e)
        {
            try
            {
                string strIndex = (sender as FontAwesome.Sharp.IconMenuItem).Text;
                if (ExcuteCount != 1) { return; }
                ddmImageMenu.Hide();
                switch (strIndex)
                {                   
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
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            ExcuteCount++;
        }

        public void SetModeRoi()
        {
            ItemROI.IconChar = FontAwesome.Sharp.IconChar.Check;
            ItemROI.IconColor = System.Drawing.Color.Red;
            ItemTrainROI.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemMultiROI.IconChar = FontAwesome.Sharp.IconChar.None;
            Mode = RoiMode.ROI;
        }

        public void SetModeTrainRoi()
        {
            ItemROI.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemMultiROI.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemTrainROI.IconColor = System.Drawing.Color.Red;
            ItemTrainROI.IconChar = FontAwesome.Sharp.IconChar.Check;
            Mode = RoiMode.Train;
        }

        public void SetModeMultiRoi()
        {
            ItemMultiROI.IconColor = System.Drawing.Color.Red;
            ItemMultiROI.IconChar = FontAwesome.Sharp.IconChar.Check;
            ItemTrainROI.IconChar = FontAwesome.Sharp.IconChar.None;
            ItemROI.IconChar = FontAwesome.Sharp.IconChar.None;
            Mode = RoiMode.MultiROI;
        }

        public void LoadImageBox2(ImageBox ImageBox, bool ControlROI = true)
        {
            ib = ImageBox;

            for (int i = 0; i < 10; i++) { ib.ZoomOut(); }
            ib.MouseWheel += new MouseEventHandler(MouseWheelEvent);
            ib.MouseDoubleClick += Ib_MouseDoubleClick;            
            ib.AllowClickZoom = false;
            ib.AllowDoubleClick = true;
            ib.SelectionMode = ImageBoxSelectionMode.None;

            System.Drawing.Color color = System.Drawing.Color.FromArgb(20, 20, 20);

            ib.GridColor = color;
            ib.GridColorAlternate = color;
            ib.ShowPixelGrid = true;

            this.ControlROI = ControlROI;
        }


        private void Ib_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < 50; i++)
            {
                ib.ZoomOut();
            }

            ib.ZoomToFit();
            ib.Update();
        }
     
        private void Ib_Paint(object sender, PaintEventArgs e)
        {

            Graphics g;
            GraphicsState originalState;
            System.Drawing.Size scaledSizeROI;
            System.Drawing.Size originalSizeROI;
            System.Drawing.Size drawSizeROI;

            System.Drawing.Size scaledSizeTrain;
            System.Drawing.Size originalSizeTrain;
            System.Drawing.Size drawSizeTrain;

            bool scaleAdornmentSize = true;

            g = e.Graphics;

            originalState = g.Save();

            // Work out the size of the marker graphic according to the current zoom level
     
            originalSizeROI = new System.Drawing.Size(ROI.Width, ROI.Height);
            scaledSizeROI = ib.GetScaledSize(originalSizeROI);
            drawSizeROI = scaleAdornmentSize ? scaledSizeROI : originalSizeROI;

            originalSizeTrain = new System.Drawing.Size(TrainROI.Width, TrainROI.Height);
            scaledSizeTrain = ib.GetScaledSize(originalSizeTrain);
            drawSizeTrain = scaleAdornmentSize ? scaledSizeTrain : originalSizeTrain;            

            System.Drawing.Point locationROI;
            System.Drawing.Point locationTrain;

            // Work out the location of the marker graphic according to the current zoom level and scroll offset
            locationROI = ib.GetOffsetPoint(ROI.X, ROI.Y);
            locationTrain = ib.GetOffsetPoint(TrainROI.X, TrainROI.Y);

            int n = 5;
            //sizeNodeRect = (n * ib.Zoom);

            for(int i = 0; i < ROIs.Count; i++)
            {
                System.Drawing.Point Location2 = ib.GetOffsetPoint(ROIs[i].Roi.X, ROIs[i].Roi.Y);
                System.Drawing.Size scaledSize2 = ib.GetScaledSize(ROIs[i].Roi.Width, ROIs[i].Roi.Height);
                System.Drawing.Color color = new System.Drawing.Color();
                if (i != SelectROiIndex) { color = System.Drawing.Color.Green; }                
                else { color = System.Drawing.Color.Red; }                             
                DrawRectangle(g, color, scaledSize2, Location2, (i + 1).ToString());
            }            
            DrawRectangle(g, System.Drawing.Color.Red, drawSizeTrain, locationTrain);
            DrawRectangle(g, System.Drawing.Color.Blue, drawSizeROI, locationROI); 

            g.Restore(originalState);
        }

        private void DrawRectangle(Graphics g, System.Drawing.Color color, System.Drawing.Size Size, System.Drawing.Point Location, string Text = "")
        {
            g.DrawRectangle(new System.Drawing.Pen(new SolidBrush(color), 3), new Rectangle(Location, Size));
            g.DrawString(Text, new Font("Arial", 12, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), Location.X + 15, Location.Y + 15);

            if (!Size.IsEmpty)
            {
                foreach (PosSizableRect pos in Enum.GetValues(typeof(PosSizableRect)))
                {
                    g.DrawRectangle(new System.Drawing.Pen(color), GetRect(pos, new Rectangle(Location, Size)));
                }
            }
        }

        private PosSizableRect GetNodeSelectable(System.Drawing.Point p, Rectangle rt)
        {
            Rectangle r2 = new Rectangle(rt.X - sizeNodeRect / 2, rt.Y - sizeNodeRect / 2, rt.Width + sizeNodeRect, rt.Height + sizeNodeRect);
            
            foreach (PosSizableRect r in Enum.GetValues(typeof(PosSizableRect)))
            {
                if (GetRect(r, r2).Contains(p))
                {
                    return r;
                }
            }
            return PosSizableRect.None;
        }

        private Cursor ChangeCursor(Cursor Cursor, System.Drawing.Point p, Rectangle rt)
        {
            return Cursor.Current = GetCursor(GetNodeSelectable(p, rt));
        }

        private Cursor GetCursor(PosSizableRect p)
        {
            switch (p)
            {
                case PosSizableRect.LeftUp:
                    return Cursors.SizeNWSE;

                case PosSizableRect.LeftMiddle:
                    return Cursors.SizeWE;

                case PosSizableRect.LeftBottom:
                    return Cursors.SizeNESW;

                case PosSizableRect.BottomMiddle:
                    return Cursors.SizeNS;

                case PosSizableRect.RightUp:
                    return Cursors.SizeNESW;

                case PosSizableRect.RightBottom:
                    return Cursors.SizeNWSE;

                case PosSizableRect.RightMiddle:
                    return Cursors.SizeWE;

                case PosSizableRect.UpMiddle:
                    return Cursors.SizeNS;
                default:
                    return Cursors.Default;
            }
        }

        private Rectangle GetRect(PosSizableRect p, Rectangle rect)
        {
            switch (p)
            {
                case PosSizableRect.LeftUp:
                    return CreateRectSizableNode(rect.X, rect.Y);

                case PosSizableRect.LeftMiddle:
                    return CreateRectSizableNode(rect.X, rect.Y + +rect.Height / 2);
                case PosSizableRect.LeftBottom:
                    return CreateRectSizableNode(rect.X, rect.Y + rect.Height);

                case PosSizableRect.BottomMiddle:
                    return CreateRectSizableNode(rect.X + rect.Width / 2, rect.Y + rect.Height);
                case PosSizableRect.RightUp:
                    return CreateRectSizableNode(rect.X + rect.Width, rect.Y);
                case PosSizableRect.RightBottom:
                    return CreateRectSizableNode(rect.X + rect.Width, rect.Y + rect.Height);
                case PosSizableRect.RightMiddle:
                    return CreateRectSizableNode(rect.X + rect.Width, rect.Y + rect.Height / 2);
                case PosSizableRect.UpMiddle:
                    return CreateRectSizableNode(rect.X + rect.Width / 2, rect.Y);
                default:
                    return new Rectangle();
            }
        }

        private int sizeNodeRect = 30;

        private Rectangle CreateRectSizableNode(int x, int y)
        {
            return new Rectangle(x - sizeNodeRect / 2, y - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);
        }


        private void IbSource_KeyUp(object sender, KeyEventArgs e)
        {            
            switch (e.KeyCode)
            {
                case Keys.ShiftKey:
                case Keys.ControlKey:
                    ib.PanMode = ImageBoxPanMode.Both;
                    break;
            }
        }

        private void IbSource_KeyDown(object sender, KeyEventArgs e)
        {            
            switch (e.KeyCode)
            {
                case Keys.ShiftKey:
                case Keys.ControlKey:
                    ib.PanMode = ImageBoxPanMode.Middle;
                    break;
                case Keys.Enter:
                    switch(Mode)
                    {
                        case RoiMode.ROI:
                            if (!ROI.IsEmpty)
                            {
                                Bitmap Crop = CConverter.cropAtRect(Image, ROI);
                                CDisplayManager.CreatePanel(Crop);
                                ClearROI();
                            }
                            break;
                        case RoiMode.Train:
                            if (!TrainROI.IsEmpty)
                            {
                                Bitmap Crop = CConverter.cropAtRect(Image, TrainROI);
                                CDisplayManager.CreatePanel(Crop);
                                ClearROI();
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
            switch (Mode)
            {
                case RoiMode.ROI:
                    ROI = new Rectangle();
                    break;
                case RoiMode.Train:
                    TrainROI = new Rectangle(); 
                    break;
                case RoiMode.MultiROI:
                    try
                    {
                        ROIs.RemoveAt(SelectROiIndex);
                    }
                    catch { }
                    
                    break;
            }
            _startPt = new System.Drawing.Point();
            _endPt = new System.Drawing.Point();

            ib.Invalidate();
            oldX = 0;
            oldY = 0;
            ib.PanMode = ImageBoxPanMode.Both;
        }

        
        private void Ib_ImageChanged(object sender, EventArgs e)
        {            
            Image = (Bitmap)ib.Image.Clone();            
            ib.ZoomToFit();
            ImageChanged = true;          
        }

        private void Ib_MouseDown(object sender, MouseEventArgs e)
        {
            if (!ControlROI) { return; }

            if (ib.IsPointInImage(e.Location))
            {                
                _startPt = ib.PointToImage(e.Location);
                nodeSelectedROI = PosSizableRect.None;
                switch (Mode)
                {
                    case RoiMode.ROI:                        
                        nodeSelectedROI = GetNodeSelectable(_startPt, ROI);
                        if (ROI.Contains(new System.Drawing.Point(_startPt.X, _startPt.Y))) { mMove = true; }                       
                        break;
                    case RoiMode.Train:                        
                        nodeSelectedROI = GetNodeSelectable(_startPt, TrainROI);
                        if (TrainROI.Contains(new System.Drawing.Point(_startPt.X, _startPt.Y))) { mMove = true; }                        
                        break;
                    case RoiMode.MultiROI:
                        if (Control.ModifierKeys == Keys.Shift)
                        {
                            CRectangle Roi = new CRectangle();
                            Roi.Roi = new Rectangle();
                            Roi.Index = ROIs.Count;
                            ROIs.Add(Roi);
                            SelectROiIndex = ROIs.Count - 1;
                        }

                        for(int i= 0; i < ROIs.Count; i++)
                        {
                            nodeSelectedROI = GetNodeSelectable(_startPt, ROIs[i].Roi);
                            if (ROIs[i].Roi.Contains(new System.Drawing.Point(_startPt.X, _startPt.Y)))
                            {
                                mMove = true;
                                SelectROiIndex = i;
                                break;
                            }
                            if (nodeSelectedROI != PosSizableRect.None) { break; }
                        }
                        break;
                }
                oldX = _startPt.X;
                oldY = _startPt.Y;
                ib.Invalidate();
            }
            else
            {
                _startPt = new Point();
                oldX = 0;
                oldY = 0;
            }
        }

        private void IbSource_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    ExcuteCount = 1;                    
                    Open_DropdownMenu(ddmImageMenu, sender, e);                    
                }
       
                mMove = false;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }      

        private void IbSource_MouseMove(object sender, MouseEventArgs e)
        {
            if (ib.Image == null) { return; }
            if (ib.Image.Width == 10 || ib.Image.Width == 10) { return; }

            _MaxY = ib.Image.Height;
            _MaxX = ib.Image.Width;

            POSITION = UpdateCursorPosition(ib, e.Location);
            if (POSITION.X > 0 && POSITION.Y > 0 && POSITION.X < ib.Image.Width && POSITION.Y < ib.Image.Height)
            {
                if(Image.Width == 10 || Image.Height == 10)
                {
                    Image = (Bitmap)ib.Image.Clone();
                }
                RGB = Image.GetPixel(POSITION.X, POSITION.Y);
                GrayValue = (RGB.R + RGB.G + RGB.B) / 3;
            }

            if (e.Button == MouseButtons.Left)
            {
                switch (Mode)
                {
                    case RoiMode.ROI:
                        SetToRectangle(e, ref ROI);
                        break;
                    case RoiMode.Train:
                        SetToRectangle(e, ref TrainROI);
                        break;
                    case RoiMode.MultiROI:
                        if (Control.ModifierKeys == Keys.Shift ||
                            Control.ModifierKeys == Keys.Control)
                        {

                            if(ROIs.Count > SelectROiIndex)
                            {
                                Rectangle Temp = new Rectangle();
                                Temp = ROIs[SelectROiIndex].Roi;
                                SetToRectangle(e, ref Temp);
                                ROIs[SelectROiIndex].Roi = Temp;
                            }          
                        }
                        break;
                }
            }
            ib.Invalidate();
        }

        private void SetToRectangle(MouseEventArgs e, ref Rectangle ROI)
        {
            if (ib.PanMode == ImageBoxPanMode.Middle) { ChangeCursor(ib.Cursor, POSITION, ROI); }
            
            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Control)
            {
                if (!ControlROI) { return; }
                Rectangle r = new Rectangle();
                r = ROI;
                switch (nodeSelectedROI)
                {
                    case PosSizableRect.LeftUp:
                        r.X += POSITION.X - oldX;
                        r.Width -= POSITION.X - oldX;
                        r.Y += POSITION.Y - oldY;
                        r.Height -= POSITION.Y - oldY;
                        break;
                    case PosSizableRect.LeftMiddle:
                        r.X += POSITION.X - oldX;
                        r.Y = ROI.Y;
                        r.Width -= POSITION.X - oldX;
                        r.Height = ROI.Height;
                        break;
                    case PosSizableRect.LeftBottom:
                        r.Width -= POSITION.X - oldX;
                        r.X += POSITION.X - oldX;
                        r.Height += POSITION.Y - oldY;
                        r.Y = ROI.Y;
                        break;
                    case PosSizableRect.BottomMiddle:
                        r.Width = ROI.Width;
                        r.X = ROI.X;
                        r.Y = ROI.Y;
                        r.Height += POSITION.Y - oldY;
                        break;
                    case PosSizableRect.RightUp:
                        r.Width += POSITION.X - oldX;
                        r.Y += POSITION.Y - oldY;
                        r.Height -= POSITION.Y - oldY;
                        r.X = ROI.X;
                        break;
                    case PosSizableRect.RightBottom:
                        r.Width += POSITION.X - oldX;
                        r.Height += POSITION.Y - oldY;
                        r.X = ROI.X;
                        r.Y = ROI.Y;
                        break;
                    case PosSizableRect.RightMiddle:
                        r.Width += POSITION.X - oldX;
                        r.Height = ROI.Height;
                        r.X = ROI.X;
                        r.Y = ROI.Y;
                        break;

                    case PosSizableRect.UpMiddle:
                        r.Y += POSITION.Y - oldY;
                        r.Height -= POSITION.Y - oldY;
                        r.X = ROI.X;
                        r.Width = ROI.Width;
                        break;
                    default:
                        if (mMove)
                        {
                            r.X = ROI.X + POSITION.X - oldX;
                            r.Y = ROI.Y + POSITION.Y - oldY;
                            r.Width = ROI.Width;
                            r.Height = ROI.Height;
                        }

                        break;
                }
                ROI = r;

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

                oldX = POSITION.X;
                oldY = POSITION.Y;
            }

            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Shift)
            {
                if (!ControlROI) { return; }
                _endPt = ib.PointToImage(e.Location);

                OpenCvSharp.Point ptStart = new OpenCvSharp.Point(_startPt.X, _startPt.Y);
                OpenCvSharp.Point ptEnd = new OpenCvSharp.Point(_endPt.X, _endPt.Y);

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
        private System.Drawing.Point UpdateCursorPosition(ImageBox imageBoxEx, System.Drawing.Point location) => imageBoxEx.PointToImage(location);   
        private void ImageMenuClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                if (ExcuteCount != 1) { return; }
                ddmImageMenu.Hide();
                switch (e.ClickedItem.Text)
                {
                    case "Image Load":
                        string ImagePath = CUtil.LoadImage();

                        if (ImagePath != "")
                        {
                            Bitmap Image = new Bitmap(ImagePath);
                            ib.Image = Image;
                            this.Image = Image;
                            CDisplayManager.ImageSrc = CConverter.ToMat(Image);
                            ib.ZoomToFit();
                        }                        
                        break;
                    case "Image Save":
                        if (ib.Image.Width != 10 && ib.Image.Height != 10)
                        {
                            ImagePath = CUtil.SaveImage();

                            if (ImagePath != "") { ib.Image.Save(ImagePath); }                           
                        }
                        break;
                    case "Show Folder":
                        Process.Start(Application.StartupPath + "\\");
                        break;
                    default:
                        break;
                }                                
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            ExcuteCount++;
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
            if (bUseControlMoveCell) { return; }

            if ((e.Delta / 120) > 0)
            {
                // up
                if (m_fImageScale > 1)
                    m_fImageScale--;

                ZoomInImage();
            }
            else
            {
                // down
                m_fImageScale++;

                ZoomOutImage();
            }
        }

        #region Display
        private void ZoomInImage() => ib.ZoomIn();    
        private void ZoomOutImage() => ib.ZoomOut();    
        private void ZoomFitImage() => ib.ZoomToFit();
        private void btnZoomOut_Click(object sender, EventArgs e) => ZoomOutImage();        
        private void btnZoomIn_Click(object sender, EventArgs e) => ZoomInImage();       
        private void btnFit_Click(object sender, EventArgs e) => ZoomFitImage();
        #endregion

        #endregion

        private void timerDrawRect_Tick(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }

    public class CRectangle
    {
        public Rectangle Roi { get; set; } = new Rectangle();
        public Rectangle PreRoi { get; set; } = new Rectangle();
        public DEFINE.ALGORITHM Algorithm { get; set; } = DEFINE.ALGORITHM.MATCING;
        public string Name { get; set; } = "";
        public int Index { get; set; } = 0;
        public float PidutialX { get; set; } = 0;
        public float PidutialY { get; set; } = 0;
        #region Blob
        public bool USE_BITWISENOT { get; set; } = true;
        public int THRESHOLD { get; set; } = 30;
        public int MIN_AREA { get; set; } = 50;
        public int MAX_AREA { get; set; } = 500000;
        public int InspCount { get; set; } = 1;
        #endregion        
        public string PATTERN_PATH { get; set; } = "TEMP";
        public double SCORE { get; set; } = 0.7;
        public bool USE_ROI { get; set; } = true;
        public bool Relative_Coordinates { get; set; } = false;

        public CRectangle() { }

        public CRectangle(CRectangle C)
        {
            this.Roi = C.Roi;
            this.Algorithm = C.Algorithm;
            this.Name = C.Name;
            this.Index = C.Index;
            this.PidutialX = C.PidutialX;
            this.PidutialY = C.PidutialY;
            this.USE_BITWISENOT = C.USE_BITWISENOT;
            this.THRESHOLD = C.THRESHOLD;
            this.MIN_AREA = C.MIN_AREA;
            this.MAX_AREA = C.MAX_AREA;
            this.PATTERN_PATH = C.PATTERN_PATH;
            this.SCORE = C.SCORE;
            this.USE_ROI = C.USE_ROI;
            this.InspCount = C.InspCount;
        }
    }
}
