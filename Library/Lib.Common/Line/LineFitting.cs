using Lib.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static Lib.Common.FormulaUtil;

namespace Lib.Line
{
    public static class LineFitting
    {
        public static LineSegment2D GetFitLine(List<OpenCvSharp.Point> Edges, PROJECTION_DIR PRJ_DIR)
        {
            if (Edges.Count == 0)
            {

                Debug.WriteLine($"Not Exists Edge, Check Parameter");
                return new LineSegment2D();
            }

            LineFittingCalculator LineFittingCalculator = new LineFittingCalculator();
            System.Drawing.Point start = new System.Drawing.Point();
            System.Drawing.Point end = new System.Drawing.Point();

            switch (PRJ_DIR)
            {
                case PROJECTION_DIR.X_RTOL:
                case PROJECTION_DIR.X_LTOR:
                    (start, end) = LineFittingCalculator.LineFitY(Edges);
                    break;
                case PROJECTION_DIR.Y_TTOB:
                case PROJECTION_DIR.Y_BTOT:
                    (start, end) = LineFittingCalculator.LineFitX(Edges);
                    break;
            }
            return new LineSegment2D(start, end);
        }

        public static LineSegment2D GetFitLineExtend(List<OpenCvSharp.Point> Edges, int Length, PROJECTION_DIR PRJ_DIR)
        {
            if (Edges.Count == 0)
            {

                Debug.WriteLine($"Not Exists Edge, Check Parameter");
                return new LineSegment2D();
            }

            LineFittingCalculator LineFittingCalculator = new LineFittingCalculator();
            System.Drawing.Point start = new System.Drawing.Point();
            System.Drawing.Point end = new System.Drawing.Point();

            switch (PRJ_DIR)
            {
                case PROJECTION_DIR.X_RTOL:
                case PROJECTION_DIR.X_LTOR:
                    (start, end) = LineFittingCalculator.LineFitY(Edges);
                    break;
                case PROJECTION_DIR.Y_TTOB:
                case PROJECTION_DIR.Y_BTOT:
                    (start, end) = LineFittingCalculator.LineFitX(Edges);
                    break;
            }

            double T = Angle(start, end);
            (start, end) = ExtendLength(new LineSegment2D(start, end), T, Length);
            return new LineSegment2D(start, end);
        }
    }
}
