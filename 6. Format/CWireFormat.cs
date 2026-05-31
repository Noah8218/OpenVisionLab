using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;

namespace KtemVisionSystem
{
    public class IWireResultManager
    {
        public CResultWire[] ListWireResult = null;
        public Mat ImageSub = new Mat();
        public Mat ImageSrc = new Mat();
        public Mat ImageResult = new Mat();

        public enum RESULT : int
        { NONE = 0, OK, NG }

        public DEFINE.RESULT Result = DEFINE.RESULT.NA;

        public int Index = 0;

        public IWireResultManager(int nIndex, CResultWire[] list, DEFINE.RESULT result, Mat imageSub, Mat imageSrc, Mat imageResult)
        {
            Index = nIndex;
            Result = result;

            ListWireResult = new CResultWire[list.Length];

            for (int i = 0; i < list.Length; i++)
            {
                ListWireResult[i] = list[i];
            }

            ImageSub = imageSub.Clone();
            ImageSrc = imageSrc.Clone();
            ImageResult = imageResult.Clone();
        }
    }
    public class CResultWire
    {
        public int Index { get; set; } = 0;
        public Point Pos { get; set; } = new OpenCvSharp.Point();
        public bool IsFind { get; set; } = false;

        public double Radius { get; set; } = 0;

        public CResultWire(int nIndex, Point pt, bool bIsFind, double dRadius)
        {
            Index = nIndex;
            Pos = pt;
            IsFind = bIsFind;
            Radius = dRadius;
        }
    }
}
