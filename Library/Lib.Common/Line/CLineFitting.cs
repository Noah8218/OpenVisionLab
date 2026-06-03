using Lib.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static Lib.Common.CFormula;

namespace Lib.Line
{
    public static class CLineFitting
    {
        public static CLine GetFitLine(List<OpenCvSharp.Point> Edges, PROJECTION_DIR PRJ_DIR)
        {
            if (Edges.Count == 0)
            {
                CLOG.ABNORMAL($"Not Exists Edge, Check Parameter");
                Debug.WriteLine($"Not Exists Edge, Check Parameter");
                return new CLine();
            }

            CLineCalculatorFitting CLineCalculatorFitting = new CLineCalculatorFitting();
            System.Drawing.Point start = new System.Drawing.Point();
            System.Drawing.Point end = new System.Drawing.Point();

            switch (PRJ_DIR)
            {
                case PROJECTION_DIR.X_RTOL:
                case PROJECTION_DIR.X_LTOR:
                    (start, end) = CLineCalculatorFitting.LineFitY(Edges);
                    break;
                case PROJECTION_DIR.Y_TTOB:
                case PROJECTION_DIR.Y_BTOT:
                    (start, end) = CLineCalculatorFitting.LineFitX(Edges);
                    break;
            }
            return new CLine(start, end);
        }

        public static CLine GetFitLineExtend(List<OpenCvSharp.Point> Edges, int Length, PROJECTION_DIR PRJ_DIR)
        {
            if (Edges.Count == 0)
            {
                CLOG.ABNORMAL($"Not Exists Edge, Check Parameter");
                Debug.WriteLine($"Not Exists Edge, Check Parameter");
                return new CLine();
            }

            CLineCalculatorFitting CLineCalculatorFitting = new CLineCalculatorFitting();
            System.Drawing.Point start = new System.Drawing.Point();
            System.Drawing.Point end = new System.Drawing.Point();

            switch (PRJ_DIR)
            {
                case PROJECTION_DIR.X_RTOL:
                case PROJECTION_DIR.X_LTOR:
                    (start, end) = CLineCalculatorFitting.LineFitY(Edges);
                    break;
                case PROJECTION_DIR.Y_TTOB:
                case PROJECTION_DIR.Y_BTOT:
                    (start, end) = CLineCalculatorFitting.LineFitX(Edges);
                    break;
            }

            double T = Angle(start, end);
            (start, end) = ExtendLength(new CLine(start, end), T, Length);
            return new CLine(start, end);
        }
    }
}
