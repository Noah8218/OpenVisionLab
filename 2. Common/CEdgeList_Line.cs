using Lib.OpenCV.Result;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenVisionLab._2._Common
{
    public class CEdgeList_Line
    {
        public CEdgeList_Line()
        {

        }

        public int No { get; set; } = 0;

        [DisplayName("X")]
        public double X { get; set; } = 0;

        [DisplayName("Y")]
        public double Y { get; set; } = 0;

        public List<CEdgeList_Line> GetEdgeList(List<OpenCvSharp.Point> Edges)
        {
            var list = new List<CEdgeList_Line>();           

            for(int i= 0; i < Edges.Count; i++)
            {
                CEdgeList_Line cEdgetList = new CEdgeList_Line();
                cEdgetList.No = i + 1;
                cEdgetList.X = Edges[i].X;
                cEdgetList.Y = Edges[i].Y;
                list.Add(cEdgetList);
            }

            return list;
        }

        public List<CEdgeList_Line> GetEdgeList(List<CVLineGuage_Result> Edges)
        {
            var list = new List<CEdgeList_Line>();

            int count = 0;
            foreach(var edge in Edges)
            {
                count++;
                var points = edge;
                for (int j = 0; j < points.edgeList.Count; j++)
                {
                    CEdgeList_Line cEdgetList = new CEdgeList_Line();
                    cEdgetList.No = count;
                    cEdgetList.X = points.edgeList[j].X;
                    cEdgetList.Y = points.edgeList[j].Y;
                    list.Add(cEdgetList);
                }
            }

            return list;
        }

        public List<CEdgeList_Line> GetEdgeList(List<List<OpenCvSharp.Point>> Edges)
        {
            var list = new List<CEdgeList_Line>();

            for (int i = 0; i < Edges.Count; i++)
            {
                List<OpenCvSharp.Point> points = Edges[i];
                for(int j = 0; j < points.Count; j++)
                {
                    CEdgeList_Line cEdgetList = new CEdgeList_Line();
                    cEdgetList.No = i + 1;
                    cEdgetList.X = points[j].X;
                    cEdgetList.Y = points[j].Y;
                    list.Add(cEdgetList);
                }                
            }

            return list;
        }

        public List<CEdgeList_Line> GetEdgeList(List<CVLineGuage_Edge> cResultBlobs)
        {
            var list = new List<CEdgeList_Line>();

            for (int i = 0; i < cResultBlobs.Count; i++)
            {
                CEdgeList_Line cEdgetList = new CEdgeList_Line();
                cEdgetList.No = i + 1;
                cEdgetList.X = cResultBlobs[i].MeasPos.X;
                cEdgetList.Y = cResultBlobs[i].MeasPos.Y;
                list.Add(cEdgetList);
            }

            return list;
        }
    }
}
