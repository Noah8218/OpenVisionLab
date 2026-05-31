using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KtemVisionSystem
{
    public class CResultManager
    {
        private int m_nColCount = 0;
        private int m_nRowCount = 0;

        private List<Point> m_listPosOrderByInsp = new List<Point>();
        public List<Point> ListPosOrderByInsp
        {
            get => m_listPosOrderByInsp;
            set => m_listPosOrderByInsp = value;
        }

        private List<CFormatResult> m_listResult = new List<CFormatResult>();
        public List<CFormatResult> ListResult
        {
            get => m_listResult;
            set => m_listResult = value;
        }

        public CResultManager(int nCol, int nRow)
        {
            m_nColCount = nCol;
            m_nRowCount = nRow;
        }

        public CResultManager()
        {
        }


        public Point GetActualPosition()
        {
            Point ptActualPos = new Point();
            return ptActualPos;
        }
    }
}
