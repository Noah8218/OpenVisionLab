using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KtemVisionSystem
{
    public class CResultQuery
    {
        private string m_strRowCount = "";
        public string RowCount
        {
            get => m_strRowCount;
            set => m_strRowCount = value;
        }
        private string m_strColCount = "";
        public string ColCount
        {
            get => m_strColCount;
            set => m_strColCount = value;
        }
        private string m_strResultData = "";
        public string ResultData
        {
            get => m_strResultData;
            set => m_strResultData = value;
        }

        private bool m_bJudgement = false;
        public bool Judgement
        {
            get => m_bJudgement;
            set => m_bJudgement = value;
        }

        //private string m_strBlobSize = "";
        //public string BlobSize
        //{
        //    get => m_strBlobSize;
        //    set => m_strBlobSize = value;
        //}

        private Dictionary<int, Dictionary<int,double>> m_dicResultRadius = new Dictionary<int, Dictionary<int, double>>();
        public Dictionary<int, Dictionary<int,double>> ResultRadius
        {
            get => m_dicResultRadius;
            set => m_dicResultRadius = value;
        }

        private List<double> m_listRadius = new List<double>();
        public List<double> listRadius
        {
            get => m_listRadius;
            set => m_listRadius = value;
        }                

        public CResultQuery()
        {

        }
    }

}
