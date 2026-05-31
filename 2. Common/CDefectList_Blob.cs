using Automation.BDaq;
using OpenVisionLab._3._Device.DB;
using Lib.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Result;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OpenVisionLab._2._Common
{
    public class CDefectList_Result
    {
        public CDefectList_Result()
        {

        }

        public int No { get; set; } = 0;

        [DisplayName("크기")]
        public double Area { get; set; } = 0.0;
        [DisplayName("각도")]
        public double Angle { get; set; } = 0.0;

        [DisplayName("센터 X")]
        public double CentetX { get; set; } = 0;

        [DisplayName("센터 Y")]
        public double CentetY { get; set; } = 0;

        [DisplayName("영역 X")]
        public int Bound_X { get; set; } = 0;
        [DisplayName("영역 Y")]
        public int Bound_Y { get; set; } = 0;
        [DisplayName("영역 W")]
        public int Bound_W { get; set; } = 0;

        [DisplayName("영역 H")]
        public int Bound_H { get; set; } = 0;

        public BindingList<CDefectList_Result> GetBlobList(List<CResultBlob> cResultBlobs)
        {
            var list = new BindingList<CDefectList_Result>();

            ConcurrentBag<CDefectList_Result> defectsS = new ConcurrentBag<CDefectList_Result>();

            Parallel.ForEach(cResultBlobs, cResultBlob =>
            {
                CDefectList_Result cDefectList = new CDefectList_Result();
                //cDefectList.No = i + 1;
                cDefectList.Area = cResultBlob.Area;
                cDefectList.Angle = cResultBlob.Angle;
                cDefectList.CentetX = cResultBlob.Center.X;
                cDefectList.CentetY = cResultBlob.Center.Y;
                cDefectList.Bound_X = cResultBlob.Bounding.X;
                cDefectList.Bound_Y = cResultBlob.Bounding.Y;
                cDefectList.Bound_W = cResultBlob.Bounding.Width;
                cDefectList.Bound_H = cResultBlob.Bounding.Height;
                defectsS.Add(cDefectList);
            });

            var listTemp2 = defectsS.OrderBy(x => x.Bound_X).ToList();

            var listTemp = listTemp2.Select((defect, index) =>
            {
                defect.No = index + 1;
                return defect;
            }).ToList();

            list = new BindingList<CDefectList_Result>(listTemp);
            return list;
        }

        public BindingList<CDefectList_Result> GetContourList(List<CResultContour> cResultBlobs)
        {
            var list = new BindingList<CDefectList_Result>();

            for (int i = 0; i < cResultBlobs.Count; i++)
            {
                CDefectList_Result cDefectList = new CDefectList_Result();
                cDefectList.No = i + 1;
                cDefectList.Area = cResultBlobs[i].Area;
                cDefectList.Angle = cResultBlobs[i].Angle;
                cDefectList.CentetX = cResultBlobs[i].Center.X;
                cDefectList.CentetY = cResultBlobs[i].Center.Y;
                cDefectList.Bound_X = cResultBlobs[i].Bounding.X;
                cDefectList.Bound_Y = cResultBlobs[i].Bounding.Y;
                cDefectList.Bound_W = cResultBlobs[i].Bounding.Width;
                cDefectList.Bound_H = cResultBlobs[i].Bounding.Height;
                list.Add(cDefectList);
            }

            return list;
        }
    }
}
