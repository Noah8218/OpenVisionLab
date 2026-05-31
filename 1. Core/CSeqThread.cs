using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVisionLab._1._Core
{
    public class CSeqThread
    {
        public CSeqThread() { }

        public CSeqVision CSeqVision { get; set; } = new CSeqVision();

        public void Start()
        {
            CSeqVision.StartThread();
        }
        
        public void Stop()
        {
            CSeqVision.StopThread();
        }
    }
}
