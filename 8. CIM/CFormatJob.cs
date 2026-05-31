using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KtemVisionSystem
{
    public class CFormatJob
    {        
        private string m_strGroupName = "";
        public string GroupName
        {
            get => m_strGroupName;
            set => m_strGroupName = value;
        }

        private string m_strDeviceName = "";
        public string DeviceName
        {
            get => m_strDeviceName;
            set => m_strDeviceName = value;
        }

        private bool m_bUseVision = false;
        public bool UseVision
        {
            get => m_bUseVision;
            set => m_bUseVision = value;
        }        

    }
}
