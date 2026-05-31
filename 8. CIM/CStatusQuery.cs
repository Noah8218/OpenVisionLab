using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KtemVisionSystem
{
    public class CStatusQuery
    {
        public CStatusQuery()
        {

        }

        public enum QCStatus : int
        { 
            IDLE = 0,
            In_Progress_Initialize = 1,
            STOP = 2,
            AUTORUN = 3,
            ManualRun = 4,
            Error = 5
        }

        public enum QCStripExist : int
        {
            NA = 0,
            EXISTS = 1,
            UNKNOWN = 2
        }

        public enum QC_END_PUSHER : int
        {
            BKD = 0,
            FWD = 1
        }

        private QCStatus m_Status = QCStatus.IDLE;
        public QCStatus Status 
        {
            get { return m_Status; }
            set 
            {
                if (m_Status != value)
                {
                    m_Status = value;                
                }                
            }
        }

        private QCStripExist m_StripExist = QCStripExist.NA;
        public QCStripExist StripExist
        {
            get { return m_StripExist; }
            set
            {
                if (m_StripExist != value)
                {
                    m_StripExist = value;
                }               
            }
        }
        private QC_END_PUSHER m_MagazineExhaust = QC_END_PUSHER.BKD;
        public QC_END_PUSHER END_PUSHER
        {
            get { return m_MagazineExhaust; }
            set
            {
                if (m_MagazineExhaust != value)
                {
                    m_MagazineExhaust = value;
                }                
            }
        }

    }
}
