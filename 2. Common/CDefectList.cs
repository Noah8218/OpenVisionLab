using Lib.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Result;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace OpenVisionLab._2._Common
{
    public class CDefectList
    {
        public CDefectList() { }
        
        public int No { get; set; } = 0;

        [DisplayName("검출 길이\n[mm]")]
        public double CenterDistY { get; set; } = 0.0;

        [DisplayName("릴 번호")]
        public int Reel_No { get; set; } = 0;

        [DisplayName("결함\n종류")]
        public string Defect_Type { get; set; } = "";

        // 결함의 X 방향 위치를 mm단위로 표시합니다
        [DisplayName("X 위치\n[mm]")]
        public double Pos_X { get; set; } = 0;

        // 결함의 X 방향 크기를 mm단위로 표시합니다
        [DisplayName("X 크기\n[mm]")]
        public double Size_X { get; set; } = 0;

        // 결함의 Y 방향 크기를 mm단위로 표시합니다
        [DisplayName("Y 크기\n[mm]")]
        public double Size_Y { get; set; } = 0;

        //[DisplayName("Real\n번호")]
        //public double Real_No { get; set; } = 0;
        [DisplayName("CAM")]
        public string CAM { get; set; } = "";
        public string 확인 { get; set; } = "";
        //public string 코멘트 { get; set; } = "";

        public Rectangle Bounding { get; set; } = new Rectangle();

        public List<CDefectList> GetProductsList()
        {
            var list = new List<CDefectList>();           

            for(int i= 0; i < 100; i++)
            {
                CDefectList cDefectList = new CDefectList();
                cDefectList.No = i + 1;
                list.Add(cDefectList);
            }

            return list;
        }

        public BindingList<CDefectList> GetDefectList(List<CResultBlob> cResultBlobs, int cam_Index, int ImageH)
        {
            var list = new BindingList<CDefectList>();

            for (int i = 0; i < cResultBlobs.Count; i++)
            {
                CDefectList cDefectList = new CDefectList();
                cDefectList.No = i + 1;

                cDefectList.CenterDistY = TruncateTo2(cResultBlobs[i].Center.Y);
                //cDefectList.Reel_No = cResultBlobs[i].ReelIndex + 1;
                //cDefectList.Defect_Type = cResultBlobs[i].defectType.ToString();
                cDefectList.Pos_X = TruncateTo2(cResultBlobs[i].Bounding.X);
                cDefectList.Size_X = TruncateTo2(cResultBlobs[i].Bounding.Width);
                cDefectList.Size_Y = TruncateTo2(cResultBlobs[i].Bounding.Height);
                cDefectList.CAM = $"Camera {cam_Index + 1}";
                cDefectList.Bounding = cResultBlobs[i].Bounding;
                list.Add(cDefectList);
            }

            return list;
        }

        private static double TruncateTo2(double value)
        {
            return Math.Truncate(100.0 * value) / 100.0;
        }
    }
}
