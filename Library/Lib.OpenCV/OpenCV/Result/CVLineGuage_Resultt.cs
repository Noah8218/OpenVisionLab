using Lib.Line;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.OpenCV.Result
{
    public class CVLineGuage_Result
    {
        public List<CVLineGuage_Edge> Results_List { get; set; } = new List<CVLineGuage_Edge>();
        public CLine FitLine { get; set; } = new CLine(); // 라인 핏팅 
        public List<OpenCvSharp.Point> edgeList { get; set; } = new List<OpenCvSharp.Point>(); // 엣지 리스트
        public CVLineGuage_Result(List<CVLineGuage_Edge> Results_List, CLine fitLine)
        {
            this.Results_List = Results_List.ConvertAll(s => new CVLineGuage_Edge(s.NO, s.MeasPos, s.UseEdge));
            this.FitLine = fitLine;
            edgeList = this.Results_List.Select(item => item.MeasPos).ToList();
        }
    }

    public class CVLineGuage_VerticalLines
    {
        public int index { get; set; } = 0;

        public List<double> intersectionLengths { get; set; } = new List<double>();
        public List<CLine> cLines { get; set; } = new List<CLine>();
    }
}
