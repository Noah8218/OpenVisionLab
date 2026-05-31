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

                // 방향에 따라서 Y축 값이 바뀜
                if (CGlobal.Inst.Device.CAMERAS[cam_Index].Property.USE_ROTATE)
                {
                    switch (CGlobal.Inst.Device.CAMERAS[cam_Index].Property.ROTATE)
                    {
                        case OpenCvSharp.RotateFlags.Rotate90Counterclockwise:
                            cDefectList.CenterDistY = Math.Truncate(100 * (cResultBlobs[i].Center.Y * CGlobal.Inst.Data.SETTING.EncoderPermm)) / 100;
                            break;
                        case OpenCvSharp.RotateFlags.Rotate90Clockwise:
                            cDefectList.CenterDistY = Math.Truncate(100 * (cResultBlobs[i].Center.Y * CGlobal.Inst.Data.SETTING.EncoderPermm)) / 100;
                            break;
                        case OpenCvSharp.RotateFlags.Rotate180:
                            cDefectList.CenterDistY = Math.Truncate(100 * ((ImageH - cResultBlobs[i].Center.Y) * CGlobal.Inst.Data.SETTING.EncoderPermm)) / 100;
                            break;
                    }
                }
                else { cDefectList.CenterDistY = Math.Truncate(100 * (cResultBlobs[i].Center.Y * CGlobal.Inst.Data.SETTING.EncoderPermm)) / 100; }               
                //cDefectList.Reel_No = cResultBlobs[i].ReelIndex + 1;
                //cDefectList.Defect_Type = cResultBlobs[i].defectType.ToString();
                cDefectList.Pos_X = Math.Truncate(100 * (cResultBlobs[i].Bounding.X * CGlobal.Inst.Device.CAMERAS[cam_Index].Property.PIXELPERMM)) / 100;
                cDefectList.Size_X = Math.Truncate(100 * (cResultBlobs[i].Bounding.Width * CGlobal.Inst.Device.CAMERAS[cam_Index].Property.PIXELPERMM)) / 100;
                cDefectList.Size_Y = Math.Truncate(100 * (cResultBlobs[i].Bounding.Height * CGlobal.Inst.Device.CAMERAS[cam_Index].Property.PIXELPERMM)) / 100;
                switch(cam_Index)
                {
                    case DEFINE.CAM_1:
                        cDefectList.CAM = "A";
                        break;
                    case DEFINE.CAM_2:
                        cDefectList.CAM = "B";
                        break;
                }
                cDefectList.Bounding = cResultBlobs[i].Bounding;
                list.Add(cDefectList);
            }

            return list;
        }
    }
}
