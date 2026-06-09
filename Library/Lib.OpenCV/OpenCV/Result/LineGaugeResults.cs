using Lib.Line;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.OpenCV.Result
{
    public class LineGaugeResult
    {
        public List<LineGaugeEdge> Results_List { get; set; } = new List<LineGaugeEdge>();
        public LineSegment2D FitLine { get; set; } = new LineSegment2D(); // 라인 핏팅 
        public List<OpenCvSharp.Point> edgeList { get; set; } = new List<OpenCvSharp.Point>(); // 엣지 리스트
        public LineGaugeResult(List<LineGaugeEdge> Results_List, LineSegment2D fitLine)
        {
            this.Results_List = Results_List.ConvertAll(s => new LineGaugeEdge(s.NO, s.MeasPos, s.UseEdge));
            this.FitLine = fitLine;
            edgeList = this.Results_List.Select(item => item.MeasPos).ToList();
        }
    }

    public class LineGaugeVerticalLines
    {
        public int index { get; set; } = 0;

        public List<double> intersectionLengths { get; set; } = new List<double>();
        public List<LineSegment2D> cLines { get; set; } = new List<LineSegment2D>();
    }
}
