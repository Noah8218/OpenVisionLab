using ADOX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace OpenVisionLab
{
    public class CAttatchLabelling
    {
        // 이 이물은 부착할것인지 

        public int No { get; set; } = 0;

        [Browsable(false)]
        public bool Use_Labelling { get; set; } = false;

        [Browsable(false)]
        public int Cam_No { get; set; } = 0;

        /*
        * * 토탈 취득프레임 예) 5회 이미지 취득(2만 5천줄) 4회는 이미 흘러갔고 5회차때 디펙이 발견된거면 아래와 같음
               계산 -> 4프레임 길이(1960mm) + Defect Center Y * 1줄(0.098mm) : 5회 이미지 취득 때 발견한 디팩의 위치
        * */

        [DisplayName("라벨 부착 위치\n[mm]")]
        public double Total_Acq_DistPerMM { get; set; } = 0;

        // 릴 위치(1 ~ 3)
        [DisplayName("라벨러 포지션")]
        public int Reel_Position { get; set; } = 1;

        [DisplayName("라벨 부착 유무")]
        public bool IsAttachLabel { get; set; } = false;

        // 이미지상 검출위치
        [Browsable(false)]
        public double detection_DistPerMM { get; set; } = 0;

        // 카메라 이미지 취득 개수
        [Browsable(false)]
        public int AcqusitionFrameCount { get; set; } = 0;

        // 카메라 이미지 취득 개수 기준으로 거리값 계산
        [Browsable(false)]
        public double AcqusitionFramePerMM { get; set; } = 0;

        // 카메라 인덱스 총 거리
        [Browsable(false)]
        public double TotalPerMM { get; set; } = 0;

        // 카메라에서 라벨러까지의 위치
        /*
         * * 카메라 -> 오프셋 예) 카메라에 스티커같은걸 붙혀놓고, Live로 스티커가 검출되는 위치부터 
            라벨러 위치까지 흘러봄, 다만 스티커가 작기 때문에 정확한 위치가 아닐거라 몇번 해보면서 다시 맞춰보는 테스트 필요    
         */

        [Browsable(false)]
        public double CameraToLabeler_Enc_Offset { get; set; } = 0;
        // 엔코더 값을 mm로 환산
        [Browsable(false)]
        public double CameraToLabeler_MM_Offset { get; set; } = 0;

        /*
         * * 속도 비례 오프셋 예) 라벨러 On/Off 하는 시간이 있기 때문에 속도에 따라 부착 위치가 틀려질것임
            분당 10/30/50/70/90/100 등 테스트하여 부착 위치에 길이 오차율을 비례식 오프셋으로 가지고 있음
         */

        [Browsable(false)] public double Speed_Proportion_Offset { get; set; } = 0;
        [Browsable(false)] public double Speed_Proportion_MM_Offset { get; set; } = 0;

        // 디팩 위치
        [Browsable(false)] public Rectangle Bounding { get; set; } = new Rectangle();

        [Browsable(false)] public Bitmap CropImage { get; set; } = new Bitmap(10, 10);
        [Browsable(false)] public bool InsertCropImage { get; set; } = false;
        [Browsable(false)] public bool IsSaveCropImage { get; set; } = false;

      //  [Browsable(false)] public DefectType defectType { get; set; } = DefectType.기포;

        public void CalculatorAttach(int ImageH, double TotalEncoder)
        {
            this.AcqusitionFramePerMM = CGlobal.Inst.Data.SETTING.EncoderPermm * ImageH;
            //this.TotalFramePerMM = this.AcqusitionFramePerMM * this.AcqusitionFrameCount;            
            this.TotalPerMM = (CGlobal.Inst.Data.SETTING.EncoderPermm * TotalEncoder) - AcqusitionFramePerMM;

            // Grab 당시 엔코더 값은 3000 6000 9000 이렇게 늘어날 것임
            // 하지만 detection_DistPerMM값은 1회 Grab한 곳에 디팩의 Y축 디팩 위치임
            // TotalPerMM = 현재 엔코더거리값 - 1회 카메라 Height 거리값이 정확한 위치일것임;
            this.Total_Acq_DistPerMM = this.TotalPerMM + detection_DistPerMM + CameraToLabeler_MM_Offset + Speed_Proportion_MM_Offset;
        }

        public CAttatchLabelling DeepCopy() => (CAttatchLabelling)this.MemberwiseClone();       

        public BindingList<CAttatchLabelling> GetAttachLabelList(List<List<CAttatchLabelling>> attaches)
        {
            var list = new BindingList<CAttatchLabelling>();

            foreach(var attach in attaches)
            {                
                for (int i = 0; i < attach.Count; i++)
                {
                    CAttatchLabelling cDefectList = new CAttatchLabelling();
                    cDefectList = attach[i].DeepCopy();
                    cDefectList.Total_Acq_DistPerMM = Math.Truncate(100 * cDefectList.Total_Acq_DistPerMM) / 100;
                    cDefectList.No = list.Count + 1;                    
                    list.Add(cDefectList);
                }
            }
            list = new BindingList<CAttatchLabelling>(list.OrderBy(x => x.Total_Acq_DistPerMM).ToList());
            for (int i = 0; i < list.Count; i++) { list[i].No = i + 1; }
            
            return list;
        }

        public BindingList<CAttatchLabelling> GetAttachLabelList(List<CAttatchLabelling> attach)
        {
            var list = new BindingList<CAttatchLabelling>();

            for (int i = 0; i < attach.Count; i++)
            {
                CAttatchLabelling cDefectList = new CAttatchLabelling();
                cDefectList = attach[i].DeepCopy();
                cDefectList.Total_Acq_DistPerMM = Math.Truncate(100 * cDefectList.Total_Acq_DistPerMM) / 100;
                cDefectList.No = list.Count + 1;
                list.Add(cDefectList);
            }

            list = new BindingList<CAttatchLabelling>(list.OrderBy(x => x.Total_Acq_DistPerMM).ToList());
            for (int i = 0; i < list.Count; i++) { list[i].No = i + 1; }

            return list;
        }
    }
}
