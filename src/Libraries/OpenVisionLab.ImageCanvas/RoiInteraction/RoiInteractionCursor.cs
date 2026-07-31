using OpenVisionLab.ImageCanvas.CanvasShapes;

namespace OpenVisionLab.ImageCanvas
{
	public class RoiInteractionCursor
	{
		public static System.Windows.Forms.Cursor GetCursorFromType(CanvasRect<float> canvasRect, System.Drawing.PointF currentImagePos, float zoomScale, float handleSize)
		{
			LineOverType lineOverType = GetLineOverTypeFromRoiRect(canvasRect, currentImagePos, zoomScale, handleSize);
			System.Windows.Forms.Cursor cursor = System.Windows.Forms.Cursors.Default;
			switch (lineOverType)
			{
				case LineOverType.None:
					cursor = System.Windows.Forms.Cursors.Default;
					break;
				case LineOverType.HSplit:
					cursor = System.Windows.Forms.Cursors.HSplit;
					break;
				case LineOverType.VSplit:
					cursor = System.Windows.Forms.Cursors.VSplit;
					break;
				case LineOverType.SizeNWSE:
					cursor = System.Windows.Forms.Cursors.SizeNWSE;
					break;
				case LineOverType.SizeNESE:
					cursor = System.Windows.Forms.Cursors.SizeNESW;
					break;
				case LineOverType.Move2D:
					cursor = System.Windows.Forms.Cursors.SizeAll;
					break;
			}

			return cursor;
		}

		private static LineOverType GetLineOverTypeFromRoiRect(CanvasRect<float> canvasRect, System.Drawing.PointF currentImagePos, float zoomScale, float handleSize)
		{
			if (canvasRect == null) { return LineOverType.None; }
			LineOverType type = canvasRect.GetHandleContainsPoint(currentImagePos.X, currentImagePos.Y, zoomScale, handleSize);
			return type;
		}
	}
}
