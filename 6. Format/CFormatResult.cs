using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KtemVisionSystem
{
    public class CFormatResult
    {
        public DEFINE.RESULT m_Result = DEFINE.RESULT.NA;

        private int m_nInspIndex = 0;
        public int InspIndex
        {
            get => m_nInspIndex;
            set => m_nInspIndex = value;
        }

        private int m_InspCount = 0;
        public int InspCount
        {
            get => m_InspCount;
            set => m_InspCount = value;
        }

        //private Mat m_ImageOriginal = new Mat();
        //public Mat ImageOriginal
        //{
        //    get => m_ImageOriginal;
        //    set => m_ImageOriginal = value;
        //}

        //private Mat m_ImageResult = new Mat();
        //public Mat ImageResult
        //{
        //    get => m_ImageResult;
        //    set => m_ImageResult = value;
        //}

        //private Mat m_ImageBinary = new Mat();
        //public Mat ImageBinary
        //{
        //    get => m_ImageBinary;
        //    set => m_ImageBinary = value;
        //}

        //public IResultFormat(int nInspIndex, RESULT result, int nCount, Mat imageOriginal, Mat imageResult, Mat imageBinary)
        //{
        //    m_nInspIndex = nInspIndex;
        //    m_Result = result;
        //    m_InspCount = nCount;
        //    //ImageOriginal = imageOriginal.Clone();
        //    //ImageResult = imageResult.Clone();
        //    //ImageBinary = imageBinary.Clone();
        //}

        public CFormatResult(int nInspIndex, DEFINE.RESULT result, int nCount)
        {
            m_nInspIndex = nInspIndex;
            m_Result = result;
            m_InspCount = nCount;
            //ImageOriginal = imageOriginal.Clone();
            //ImageResult = imageResult.Clone();
            //ImageBinary = imageBinary.Clone();
        }

        public CFormatResult()
        {

        }
    }
}