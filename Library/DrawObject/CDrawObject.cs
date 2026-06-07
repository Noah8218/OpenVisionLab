using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using static OpenVisionLab.DrawObject.CEnum;

namespace OpenVisionLab.DrawObject
{
    public abstract class CDrawObject
    {
        #region 전역변수

        public int _MinY = 0;
        public int _MaxY = 10000;
        public int _MinX = 0;
        public int _MaxX = 10000;

        //DrawObject 의 선택 여부를 저장
        public bool Selected { get; set; } = false;
        public System.Drawing.Size OriginalSize { get; set; } = new System.Drawing.Size();

        public System.Drawing.Region[] _rgAnchors = new Region[9];

        public System.Drawing.Region _rgRect = new Region();
        public System.Drawing.Size Size { get; set; } = new System.Drawing.Size();
        public System.Drawing.Point Location { get; set; } = new System.Drawing.Point();
        public bool IsRotate { get; set; } = true;

        //DrawObject 의 테두리 색깔을 지정한다.
        public System.Drawing.Color Color { get; set; } = new System.Drawing.Color();

        //DrawObject 의 배경 색깔을 지정한다.
        public System.Drawing.Color BackColor { get; set; } = new System.Drawing.Color();

        //DrawObject 의 Pen 두께를 지정한다.
        public int PenWidth { get; set; } = 1;

        public float Angle { get; set; } = 0.0f;

        public string Title { get; set; } = "";

        #endregion

        #region 생성자

        public CDrawObject()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// DrawObject 의 선택여부
        /// </summary>
        
        /// <summary>
        /// DrawObject 의 핸들 갯수
        /// </summary>
        public virtual int HandleCount
        {
            get
            {
                return 0;
            }
        }

        #endregion

        #region 가상 함수

        /// <summary>
        /// DrawObject 복사 함수
        /// </summary>
        public abstract CDrawObject Clone();

        public abstract void SetParameter(System.Drawing.Color color, System.Drawing.Size Size, System.Drawing.Point Location, bool isRotate = true, string Text = "");

        /// <summary>
        /// DrawObject 그리기 함수
        /// </summary>
        /// <param name="g"></param>
        public virtual void Draw(Graphics g)
        {

        }

        /// <summary>
        /// 핸들 넘버의 위치를 반환한다.
        /// </summary>
        public virtual Point GetHandle(int handleNumber)
        {
            return new Point(0, 0);
        }

        /// <summary>
        /// 핸들의 Rectangle 을 반환한다.
        /// </summary>
        public virtual Rectangle GetHandleRectangle(int handleNumber)
        {
            Point point = GetHandle(handleNumber);

            return new Rectangle(point.X - 3, point.Y - 3, 7, 7);
        }

        /// <summary>
        /// DrawObject 가 선택되었을때 표시를 해주는 Pointer 를 그린다
        /// </summary>
        public virtual void DrawPointer(Graphics g)
        {
            if (!Selected)
                return;

            //using (SolidBrush brush = new SolidBrush(Color.Black))
            //{
            //    for (int i = 1; i <= HandleCount; i++)
            //    {
            //        g.FillRectangle(brush, GetHandleRectangle(i));
            //    }
            //}
        }

        /// <summary>
        ///  마우스가 클릭된 위치가 DrawObject를 포함하는지 알려준다
        ///  -1 - no hit
        ///   0 - hit anywhere
        /// > 1 - handle number
        /// </summary>
        public virtual int HitTest(Point point)
        {
            return -1;
        }


        /// <summary>
        /// 마우스의 위치가 DrawObject 내에 있는지 알려준다.
        /// </summary>
        protected virtual bool PointInObject(Point point)
        {
            return false;
        }


        /// <summary>
        /// Pointer 의 HandleNumber 에 따라서 마우스 커서를 반환한다.
        /// </summary>
        public virtual Cursor GetHandleCursor(int handleNumber)
        {
            return Cursors.Default;
        }

        /// <summary>
        /// DrawObject가 rectangle 에 포함되는지 알려준다.
        /// </summary>
        public virtual bool IntersectsWith(Rectangle rectangle)
        {
            return false;
        }

        /// <summary>
        /// DrawObject 의 위치를 이동한다.
        /// </summary>
        public virtual void Move(int deltaX, int deltaY)
        {
        }

        /// <summary>
        /// DrawObject 의 사이즈를 변경한다.
        /// </summary>
        public virtual void MoveHandleTo(Point point, PosSizableRect MouseOperation)
        {

        }

        /// <summary>
        /// DrawObject 를 새로 그리거나, 사이즈를 변경이 끝났을 때 호출된다.
        /// </summary>
        public virtual void Normalize()
        {
        }

        #endregion

        #region 내부 함수

        /// <summary>
        /// DrawObject 초기화
        /// </summary>
        protected void Initialize()
        {
            //DrawObject 를 선택으로 설정
            this.Selected = true;
        }

        /// <summary>
        /// DrawObject 를 복사 할 때, 속성들을 복사해준다.
        /// </summary>
        protected CDrawObject DeepCopy() => (CDrawObject)this.MemberwiseClone();
        
        #endregion
    }
}
