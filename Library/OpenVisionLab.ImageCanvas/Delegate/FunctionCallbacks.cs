using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;

namespace OpenVisionLab.ImageCanvas
{
	public delegate void OverlayAddedCallback(CanvasRect<float> canvasRect, CanvasOverlayItem parentOverlay);
	public delegate void OverlayGroupAddedCallback(OpenVisionLab.ImageCanvas.Model.RoiChangedEventArgs arg);
	public delegate void OverlayEditingCompletedCallback(CanvasRect<float> canvasRect);
}
