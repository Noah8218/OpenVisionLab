using Lib.Common;
using System.Drawing;

namespace Lib.OpenCV.Result
{
    public class MeanResult
    {
        public int index { get; set; } = 0;
        public double meanValue { get; set; } = 0.0D;
        public Rectangle Bounding { get; set; } = new Rectangle();
        public PointF Center { get; set; } = new PointF();

        public MeanResult(int index, double Mean, Rectangle Bounding)
        {
            this.index = index;
            this.meanValue = Mean;
            this.Bounding = Bounding;
            this.Center = CommonConverter.Center(Bounding);
        }
    }
}
