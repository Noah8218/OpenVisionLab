using System.Drawing;

namespace OpenVisionLab.ImageSpace.Core
{
    public interface IImageSpace
    {
        void SetActiveImage(Bitmap image);
        Bitmap GetActiveImage();
        void SetImage(int index, string title, Bitmap image);
        Bitmap GetImage(int index);
        Bitmap GetImage(string title);
        void SetRoi(int index, Rectangle roi);
        Rectangle GetRoi(int index);
        Rectangle GetRoi(string title);
        void SetTrainRoi(int index, Rectangle roi);
        Rectangle GetTrainRoi(int index);
        Rectangle GetTrainRoi(string title);
        void MarkImageChanged(string title, bool changed);
        bool IsImageChanged(string title);
        void AcceptImageChanged(string title);
    }
}
