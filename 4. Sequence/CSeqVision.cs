using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
namespace OpenVisionLab
{
    public class CSeqVision : CSeqBase
    {        
        //public EventHandler<InspResultArgs> EventInspResult;
        
        private bool[] GRAB_COMPLETE { get; set; } = new bool[1] { false};
        private bool[] INSP_COMPLETE { get; set; } = new bool[1] { false };
        private Bitmap[] INSP_IMAGE { get; set; } = new Bitmap[1] { new Bitmap(10, 10)};

        public CSeqVision()
        {
            this.Name = nameof(CSeqVision);
            this.TimeMS = 1;
        }

        public override void Run()
        {
            if (CGlobal.Inst.Data.GrabQueue.Count > 0)
            {
                if (CGlobal.Inst.Data.GrabQueue.TryDequeue(out CGrabBuffer GrabBuffer))
                {
                    RunInspection(GrabBuffer);
                }
            }
        }

        private Stopwatch stopwatch = new Stopwatch();

        // 비동기 Task를 실행하는 가장 기본적인 방법입니다.
        // async키워드로 비동기가 가능하며, 해당 키워드를 사용시 await 키워드를 찾게 됩니다.
        // await키워드를 사용하지 않을시 동기식으로 실행이 됩니다.
        public async void RunInspection(CGrabBuffer GrabBuffer)
        {
            await Task.Run(() =>
            {                

            });
        }
    }
}
