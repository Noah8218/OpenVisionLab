using System.Collections.Generic;
using System.ComponentModel;

namespace OpenVisionLab._3._Device.DB
{
    public class CDefectSummary
    {
        [DisplayName("No")]
        public int Index { get; set; } = 0;
        public string LotID { get; set; } = "";

        [DisplayName("기포")]
        public int CountBubble { get; set; } = 0; // 기포

        [DisplayName("스크래치")]
        public int CountScratch { get; set; } = 0; // 스크래치

        [DisplayName("이물")]
        public int CountBlob { get; set; } = 0; // 이물

        [DisplayName("미코팅")]
        public int CountUncoated { get; set; } = 0; // 미코팅

        [DisplayName("코팅폭")]
        public int CountCoatingWidth { get; set; } = 0; // 코팅폭   

        //public void RunCountIncrease(DefectType defectType)
        //{
        //    switch(defectType)
        //    {
        //        case DefectType.폭불량:
        //            CountCoatingWidth++;
        //            break;
        //        case DefectType.이물:
        //            CountBlob++;
        //            break;
        //        case DefectType.기포:
        //            CountBubble++;
        //            break;
        //        case DefectType.미도금:
        //            CountUncoated++;
        //            break;
        //        case DefectType.스크래치:
        //            CountScratch++;
        //            break;                
        //    }
        //}

        public BindingList<CDefectSummary> GetAttachLabelList(List<CDefectSummary> cDefectSummaries)
        {
            BindingList<CDefectSummary> bindingList = new BindingList<CDefectSummary>(cDefectSummaries);
            return bindingList;
        }
    }
}
