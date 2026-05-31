using OpenVisionLab._2._Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVisionLab._3._Device.DB
{
    public class CDefect
    {
        [DisplayName("No")]
        public int Index { get; set; } = 0;        
        public string LotID { get; set; } = "";
        public string DefectType { get; set; } = "";
        [DisplayName("CAM 위치")]
        public string Position { get; set; } = "상부";
        [DisplayName("릴 위치")]
        public int Reel_No { get; set; } = 0;

        [DisplayName("센터 X")]
        public double CenterX { get; set; } = 0;
        [DisplayName("센터 Y")]
        public double CenterY { get; set; } = 0;
        [DisplayName("가로 길이")]
        public double Width { get; set; } = 0;
        [DisplayName("세로 길이")]
        public double Height { get; set; } = 0;
        [DisplayName("검출 사이즈")]
        public double Area { get; set; } = 0;
        [DisplayName("검사 이미지 경로")]
        public string ImagePath { get; set; } = "";
        [DisplayName("검사 시간")]
        public DateTime DateTime { get; set; } = DateTime.Now;

        public BindingList<CDefect> GetDefectList(List<CDefect> defects)
        {
            BindingList<CDefect> bindingList = new BindingList<CDefect>(defects);
            return bindingList;
        }
    }
}
