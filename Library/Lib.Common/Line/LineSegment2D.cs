using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Lib.Line
{
    public class LineSegment2D
    {
        private OpenCvSharp.Point m_ptStart = new OpenCvSharp.Point();
        public OpenCvSharp.Point Start
        {
            get { return m_ptStart; }
            set { m_ptStart = value; }
        }

        private OpenCvSharp.Point m_ptEnd = new OpenCvSharp.Point();
        public OpenCvSharp.Point End
        {
            get { return m_ptEnd; }
            set { m_ptEnd = value; }
        }

        public double Distance()
        {
            return m_ptStart.DistanceTo(m_ptEnd);
        }

        public LineSegment2D(OpenCvSharp.Point ptStart, OpenCvSharp.Point ptEnd)
        {
            m_ptStart = ptStart;
            m_ptEnd = ptEnd;
        }

        public LineSegment2D(PointF ptStart, PointF ptEnd)
        {
            m_ptStart = new OpenCvSharp.Point((int)ptStart.X, (int)ptStart.Y);
            m_ptEnd = new OpenCvSharp.Point((int)ptEnd.X, (int)ptEnd.Y);
        }

        public LineSegment2D()
        {

        }
    }
}
