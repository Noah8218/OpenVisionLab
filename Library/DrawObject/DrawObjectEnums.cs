using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVisionLab.DrawObject
{
    public class DrawObjectEnums
    {
        public enum RoiMode
        {
            Drag,
            ROI,
            Train,
            MultiROI
        }

        public enum PosSizableRect : int
        {
            UpMiddle = 0,
            LeftMiddle = 1,
            LeftBottom = 2,
            LeftUp = 3,
            RightUp = 4,
            RightMiddle = 5,
            RightBottom = 6,
            BottomMiddle = 7,
            Rotate = 8,
            None,
            SizeAll
        };

        /// <summary>
        /// 현재 선택된 상태
        /// </summary>
        public enum SelectMode
        {
            None,            //아무것도 선택되지 않음
            NetSelection,   //영역으로 선택
            Move,           //이동
            Size            //사이즈 변경
        };
    }
}
