using System.Drawing;

namespace OpenVisionLab.ImageCanvas.Canvas
{
	public sealed class CanvasViewState
	{
		public CanvasViewState(float zoom, SizeF offsetSize)
		{
			Zoom = zoom;
			OffsetSize = offsetSize;
		}

		public float Zoom { get; }
		public SizeF OffsetSize { get; }
	}
}
