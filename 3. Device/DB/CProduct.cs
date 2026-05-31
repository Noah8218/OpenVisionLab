using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVisionLab._3._Device.DB
{
    public class CProduct
    {
        public int Index { get; set; } = 0;
        public string LotID { get; set; } = "";        
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
