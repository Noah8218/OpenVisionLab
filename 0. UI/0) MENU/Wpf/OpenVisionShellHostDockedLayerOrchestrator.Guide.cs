using System.Windows;

namespace OpenVisionLab
{
    internal sealed partial class OpenVisionShellHostDockedLayerOrchestrator
    {
        public void ShowGuideAt(Point point)
        {
            composition.ShowGuideAt(point);
        }

        public bool BeginTestDragGuide(DependencyObject source, Point point)
        {
            return composition.BeginTestDragGuide(source, point);
        }

        public bool IsGestureSource(DependencyObject source)
        {
            return composition.IsGestureSource(source);
        }

        public void ResetGuide()
        {
            composition.ResetGuide();
        }
    }
}
