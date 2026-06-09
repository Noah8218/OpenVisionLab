
using OpenCvSharp;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static OpenVisionLab.DrawObject.DrawObjectEnums;
using static System.Net.Mime.MediaTypeNames;
using Point = System.Drawing.Point;

namespace OpenVisionLab.DrawObject
{
    public class RectangleDrawObject : DrawObjectBase
    {
        public Rectangle Roi = new Rectangle();
        
        #region 생성자

        public RectangleDrawObject() : this(0, 0, 0, 0)
        {

        }

        public RectangleDrawObject(int x, int y, int width, int height)
            : base()
        {
            Roi = new Rectangle(x, y, width, height);
            Initialize();
        }

        #endregion


        /// <summary>
        /// 이 객체를 복사한다.
        /// </summary>   
        public override DrawObjectBase Clone() => (RectangleDrawObject)this.MemberwiseClone();

        public override void SetParameter(System.Drawing.Color color, System.Drawing.Size Size, System.Drawing.Point Location, bool isRotate = true, string Text = "")
        {
            this.Color = color;
            this.Size = Size;
            this.Location = Location;
            this.IsRotate = isRotate;
            this.Title = Text;
        }

        /// <summary>
        /// 이 객체를 그려준다.
        /// </summary>
        public override void Draw(Graphics g)
        {
            if (Size.IsEmpty) { return; }
            RectangleF r = new RectangleF(Location, Size);
            PointF ptCenter = new PointF(r.Left + (r.Width / 2), r.Top + (r.Height / 2));
            PointF ptCenterFull = new PointF((OriginalSize.Width / 2), (OriginalSize.Height / 2));
            Matrix m = new Matrix();

            m.RotateAt(Angle, ptCenterFull, MatrixOrder.Append);
            g.Transform = m;
            _rgRect = new Region(Roi);

            g.Transform.Reset();
            m.Reset();
            m.RotateAt(Angle, ptCenter, MatrixOrder.Append);
            g.Transform = m;

            using (System.Drawing.Pen pen = new System.Drawing.Pen(Color, PenWidth))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                g.DrawString(Title, new System.Drawing.Font("Arial", 12, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), Location.X + 15, Location.Y + 15);
                if (IsRotate)
                {
                    g.DrawString(Angle.ToString(), new System.Drawing.Font("Arial", 12, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), Location.X + 15, Location.Y + 15);
                }
                
                foreach (PosSizableRect pos in Enum.GetValues(typeof(PosSizableRect)))
                {
                    if (!IsRotate) { if (pos == PosSizableRect.Rotate) { continue; } }
                    Rectangle miniRect = GetRect(pos, new Rectangle(Location, Size));
                    PointF[] rectCorners = new PointF[4];
                    rectCorners[0] = new PointF(miniRect.Location.X, miniRect.Location.Y);
                    rectCorners[1] = new PointF(miniRect.Size.Width + miniRect.Location.X, miniRect.Location.Y);
                    rectCorners[2] = new PointF(miniRect.Size.Width + miniRect.Location.X, miniRect.Size.Height + miniRect.Location.Y);
                    rectCorners[3] = new PointF(miniRect.Location.X, miniRect.Size.Height + miniRect.Location.Y);

                    switch (pos)
                    {
                        case PosSizableRect.LeftUp:
                        case PosSizableRect.LeftMiddle:
                        case PosSizableRect.LeftBottom:
                        case PosSizableRect.BottomMiddle: 
                        case PosSizableRect.RightUp:
                        case PosSizableRect.RightBottom:
                        case PosSizableRect.RightMiddle:
                        case PosSizableRect.UpMiddle:                                                        
                        case PosSizableRect.Rotate:
                            if (Selected)
                            {
                                if (pos == PosSizableRect.Rotate)
                                {
                                    g.FillEllipse(new SolidBrush(System.Drawing.Color.LightGreen), miniRect);
                                    g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Black), miniRect);
                                }
                                else
                                {
                                    { g.DrawPolygon(new System.Drawing.Pen(Color), rectCorners); }
                                }
                            }
                            break;
                        case PosSizableRect.SizeAll:
                            g.DrawPolygon(new System.Drawing.Pen(Color), rectCorners);
                            break;
                    }
            
                    if ((int)pos < 9)
                    {
                        _rgAnchors[(int)pos] = new Region(miniRect);
                        _rgAnchors[(int)pos].Transform(m);
                    }
                }
                _rgRect = new Region(r);
                _rgRect.Transform(m);
            }
        }

        private Cursor AnchorToCursor(PosSizableRect r, double angle)
        {
            double snAngle = angle;

            switch (r)
            {
                case PosSizableRect.Rotate:
                    return Cursors.NoMove2D;
                case PosSizableRect.SizeAll:
                    return Cursors.SizeAll;
                case PosSizableRect.None:
                    return Cursors.Default;
                case PosSizableRect.LeftUp:
                case PosSizableRect.RightBottom:
                    snAngle += 45;
                    break;
                case PosSizableRect.UpMiddle:
                case PosSizableRect.BottomMiddle:
                    snAngle += 90;
                    break;
                case PosSizableRect.LeftBottom:
                case PosSizableRect.RightUp:
                    snAngle += 135;
                    break;
                case PosSizableRect.LeftMiddle:
                case PosSizableRect.RightMiddle:
                    break;
            }

            if (snAngle > 360)
            { snAngle -= 360; }

            switch (snAngle)
            {
                case double T when (T > 26 && T < 68 || T > 204 && T < 248):
                    return Cursors.SizeNWSE;
                case double T when (T > 69 && T < 113 || T > 249 && T < 293):
                    return Cursors.SizeNS;
                case double T when (T > 114 && T < 158 || T > 294 && T < 338):
                    return Cursors.SizeNESW;
                default:
                    return Cursors.SizeWE;
            }
        }

        public PosSizableRect GetNodeSelectable(System.Drawing.Point p, Rectangle rt)
        {
            if (rt.IsEmpty) { return PosSizableRect.None; }

            foreach (PosSizableRect r in Enum.GetValues(typeof(PosSizableRect)))
            {
                // 그외 기타 포지션
                if ((int)r < 9)
                {
                    if (_rgAnchors[(int)r] != null)
                    {
                        if (_rgAnchors[(int)r].IsVisible(p))
                        {
                            return r;
                        }
                    }
                }

                // 중심
                if (_rgRect.IsVisible(p))
                {
                    return PosSizableRect.SizeAll;
                }
            }

            return PosSizableRect.None;
        }

        public Cursor ChangeCursor(System.Drawing.Point p, Rectangle rt) => AnchorToCursor(GetNodeSelectable(p, rt), this.Angle);

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
                case PosSizableRect.SizeAll:
                    return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                case PosSizableRect.Rotate:
                    Rectangle r = CreateRectSizableNode(rect.X + rect.Width / 2, rect.Y);
                    System.Drawing.Size szAnhorSize = new System.Drawing.Size(7, 7);
                    return new Rectangle(r.Left, r.Top - (szAnhorSize.Height * 6) + 1, r.Width + 1, r.Height + 1);
                default:
                    return new Rectangle();
            }
        }

        private int sizeNodeRect = 15;

        private Rectangle CreateRectSizableNode(int x, int y)
        {
            return new Rectangle(x - sizeNodeRect / 2, y - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);
        }

        //Retangle 의 크기와 위치를 설정한다.
        protected Rectangle SetRectangle(int x, int y, int width, int height)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;

            return rectangle;
        }

        private double GetAngleBetweenTwoPointsWithFixedPoint(PointF tPt1, PointF tPt2, PointF tPtFixed)
        {
            double snAngle1 = Math.Atan2(tPt1.Y - tPtFixed.Y, tPt1.X - tPtFixed.X);
            double snAngle2 = Math.Atan2(tPt2.Y - tPtFixed.Y, tPt2.X - tPtFixed.X);
            return snAngle1 - snAngle2;
        }

        private double EditRotateAngle(double snAngle)
        {
            snAngle = -snAngle * 180.0 / Math.PI;

            //Keep within 0 ~ 359.9
            Math.Floor(snAngle);

            if (snAngle >= 360) snAngle = 0;
            if (snAngle < 0) snAngle = 360 - (-snAngle);

            bool bQuantized = false;
            for (int i = 1; i < 9; i++)
            {
                snAngle = QuantizeRotation(snAngle, i * 45, out bQuantized);
                if (bQuantized) { return snAngle; }
            }
            return snAngle;
        }

        private double QuantizeRotation(double snRotation, double snTarget, out bool bQuantized)
        {
            bQuantized = false;
            double snQuantize = 6;

            // Set init
            double snLowRef = (snTarget - snQuantize);
            double snHiRef = (snTarget + snQuantize);

            // Keep targets within boundires
            if (snLowRef >= 360) snLowRef = 360;
            if (snLowRef < 0) snLowRef = 360 - (-snLowRef);
            if (snHiRef >= 360) snHiRef = 360;
            if (snHiRef < 0) snHiRef = 360 - (-snHiRef);

            if (snLowRef < snHiRef)
            {
                if (snRotation > snLowRef && snRotation < snHiRef)
                {
                    bQuantized = true;

                    if (snTarget == 360) { snTarget = 0; }

                    return snTarget;
                }
                else { return snRotation; }
            }
            else
            {
                if (snLowRef < 360 || 0 < snHiRef)
                {
                    bQuantized = true;
                    return snTarget;
                }
                else { return snRotation; }
            }
        }

        public void CalculatorAngle(PointF mouseDown, PointF POSITION)
        {
            PointF Center_ROI = new PointF(Roi.Left + (Roi.Width / 2), Roi.Top + (Roi.Height / 2));
            double snAngle = GetAngleBetweenTwoPointsWithFixedPoint(mouseDown, POSITION, Center_ROI);
            Angle = (float)EditRotateAngle(snAngle);
        }

        /// <summary>
        /// DrawObject 의 사이즈를 변경한다.
        /// </summary>
        public override void MoveHandleTo(Point point, PosSizableRect MouseOperation)
        {
            if (MouseOperation == PosSizableRect.None) { return; }
            if (MouseOperation == PosSizableRect.SizeAll) { return; }
            int left = Roi.Left;
            int top = Roi.Top;
            int right = Roi.Right;
            int bottom = Roi.Bottom;

            switch (MouseOperation)
            {
                case PosSizableRect.LeftUp:
                    if ((int)point.X < _MinX) { left = _MinX; }
                    else { left = (int)point.X; }

                    if ((int)point.Y < _MinY) { top = _MinY; }
                    else { top = (int)point.Y; }
                    break;
                case PosSizableRect.LeftMiddle:
                    if ((int)point.X < _MinX) { left = _MinX; }
                    else { left = (int)point.X; }                   
                    break;
                case PosSizableRect.LeftBottom:
                    if ((int)point.X < _MinX) { left = _MinX; }
                    else { left = (int)point.X; }

                    if ((int)point.Y > _MaxY) { bottom = _MaxY; }
                    else { bottom = (int)point.Y; }                    
                    break;
                case PosSizableRect.BottomMiddle:
                    if ((int)point.Y > _MaxY) { bottom = _MaxY; }                    
                    else { bottom = (int)point.Y; }                    
                    break;
                case PosSizableRect.RightUp:
                    if ((int)point.X > _MaxX) { right = _MaxX; }
                    else { right = (int)point.X; }

                    if ((int)point.Y < _MinY) { top = _MinY; }
                    else { top = (int)point.Y; }
                    break;
                case PosSizableRect.RightBottom:
                    if ((int)point.X > _MaxX) { right = _MaxX; }
                    else { right = (int)point.X; }

                    if ((int)point.Y > _MaxY) { bottom = _MaxY; }
                    else { bottom = (int)point.Y; }
                    break;
                case PosSizableRect.RightMiddle:
                    if ((int)point.X > _MaxX) { right = _MaxX; }
                    else { right = (int)point.X; }                                    
                    break;
                case PosSizableRect.UpMiddle:
                    if ((int)point.Y < _MinY) { top = _MinY; }
                    else { top = (int)point.Y; }                    
                    break;
            }

            Roi = SetRectangle(left, top, right - left, bottom - top);  
        }

        /// <summary>
        /// DrawObject 의 위치를 이동한다.
        /// </summary>
        public override void Move(int deltaX, int deltaY)
        {
            if (Roi.X + Roi.Width + deltaX > _MaxX) { return; }
            if (Roi.X + deltaX < _MinX) { return; }
            if (Roi.Y + deltaY < _MinY) { return; }
            if (Roi.Y + Roi.Height + deltaY > _MaxY) { return; }
            
            Roi.X += deltaX;
            Roi.Y += deltaY;
        }
    }
}
