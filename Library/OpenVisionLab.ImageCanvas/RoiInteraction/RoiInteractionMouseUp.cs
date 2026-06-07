using OpenVisionLab.ImageCanvas.Views;
using OpenVisionLab.ImageCanvas.ViewModels;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using System;
using System.Drawing;
using System.Windows;

namespace OpenVisionLab.ImageCanvas
{
	public class RoiInteractionMouseUp
	{
		public static bool AddRectangleToOverlay(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, System.Drawing.PointF preMousePos, System.Drawing.PointF postMousePos, ref CanvasRect<float> activeRoiRect, OverlayAddedCallback callbackRoiAdded)
		{
			if (imageViewer.GetViewMode() != CanvasInteractionMode.Drawing) return false;

			// ROIÎ•??ïÏùò?òÎäî RectangleF Í∞ùÏ≤¥ ?ùÏÑ±
			RectangleF roi = new RectangleF(preMousePos.X, preMousePos.Y, postMousePos.X - preMousePos.X, postMousePos.Y - preMousePos.Y);
			if (roi.Width == 0 || roi.Height == 0) return false;

			// _activeRoiRect??ÏßÅÏ†ë Ï¥àÍ∏∞?îÌïòÍ≥? UniqueId ?§Ï†ï
			activeRoiRect = new CanvasRect<float>(roi.Left, roi.Top, roi.Right, roi.Bottom)
			{
				UniqueId = Guid.NewGuid().ToString()
			};

			// ÎßàÏ?Îß?Í∑∏Î£π??Í∞Ä?∏Ïò¥
			CanvasOverlayItem parentOverlay = imageViewer.GetLastGroup();
			if (parentOverlay == null) return false;

			// _activeRoiRect???¨Ïö©?òÏó¨ ?§Ïù¥?¥Í∑∏??Ï∂îÍ?
			imageViewer.AddOverlay(parentOverlay.GroupType, parentOverlay.GroupType, activeRoiRect, activeRoiRect.UniqueId, parentOverlay.InspWindowType, EnumItemType.Window);

			// MouseUp ?¥Î≤§??Ï≤òÎ¶¨Î•??ÑÌïú Ï∂îÍ? Î°úÏßÅ
			callbackRoiAdded?.Invoke(activeRoiRect, parentOverlay);
			return true;
		}

		public static void OpenAddRoiArrayView(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, AddRoiArrayViewModel addRoiArrayVm, OverlayAddedCallback callbackRoiAdded)
		{
			AddRoiArrayView addRoiArrayView = new AddRoiArrayView();
			addRoiArrayView.Title = "Roi Add";
			addRoiArrayView.DataContext = addRoiArrayVm;
			addRoiArrayView.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			bool? dialogResult = addRoiArrayView.ShowDialog();

			try
			{
				AddRoiArrayData data = new AddRoiArrayData();
				data.Rows = int.Parse(addRoiArrayVm.Rows);
				data.Columns = int.Parse(addRoiArrayVm.Columns);
				data.RowSpacing = float.Parse(addRoiArrayVm.RowSpacing);
				data.ColumnSpacing = float.Parse(addRoiArrayVm.ColumnSpacing);

				AddRectangleToOverlayArray(imageViewer, data, imageViewer.PreMousePos, imageViewer.PostMousePos, imageViewer.PixelPermm, callbackRoiAdded);
			}
			catch
			{
				//MessageBoxManager.ShowWindow("Warning", "Please enter a normal value.", MessageBoxViewModel.EnumMessageBoxType.Warning);
			}
		}

		private static void AddRectangleToOverlayArray(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, AddRoiArrayData roiArrayData, System.Drawing.PointF preMousePos, System.Drawing.PointF postMousePos, float pixelPermm, OverlayAddedCallback callbackRoiAdded)
		{
			// Í∏∞Î≥∏ ROI Í≥ÑÏÇ∞
			RectangleF baseRoi = new RectangleF(preMousePos.X, preMousePos.Y, postMousePos.X - preMousePos.X, postMousePos.Y - preMousePos.Y);

			float pixelPerMm = 1 / pixelPermm; // 1?ΩÏ???0.12mm?êÏÑú 1mm???ΩÏ? ?òÎ°ú Î≥Ä??

			// Í∞ÑÍ≤©??mm ?®ÏúÑÎ°??§Ï†ï
			float rowSpacingInMm = roiArrayData.RowSpacing; // ?àÎ? ?§Ïñ¥ 70mm
			float columnSpacingInMm = roiArrayData.ColumnSpacing; // ?àÎ? ?§Ïñ¥ 46mm

			// mm ?®ÏúÑ Í∞ÑÍ≤©???ΩÏ? ?®ÏúÑÎ°?Î≥Ä??
			float rowSpacingInPixels = rowSpacingInMm * pixelPerMm;
			float columnSpacingInPixels = columnSpacingInMm * pixelPerMm;


			// Í∞??âÍ≥º ?¥Ïóê ?Ä??ROI Ï∂îÍ?
			for (int row = 0; row < roiArrayData.Rows; row++)
			{
				for (int column = 0; column < roiArrayData.Columns; column++)
				{
					// ?ÑÏû¨ ROI???ÑÏπò Í≥ÑÏÇ∞ (mm ?®ÏúÑ Í∞ÑÍ≤©???¨Ïö©)
					float currentX = baseRoi.X + column * (columnSpacingInPixels);
					float currentY = baseRoi.Y - row * (rowSpacingInPixels);

					// ROIÎ•??ïÏùò?òÎäî RectangleF Í∞ùÏ≤¥ ?ùÏÑ±
					RectangleF currentRoi = new RectangleF(currentX, currentY, baseRoi.Width, baseRoi.Height);

					// _activeRoiRect??ÏßÅÏ†ë Ï¥àÍ∏∞?îÌïòÍ≥? UniqueId ?§Ï†ï
					CanvasRect<float> activeRoiRect = new CanvasRect<float>(currentRoi.Left, currentRoi.Top, currentRoi.Right, currentRoi.Bottom)
					{
						UniqueId = Guid.NewGuid().ToString()
					};

					// ÎßàÏ?Îß?Í∑∏Î£π??Í∞Ä?∏Ïò¥
					CanvasOverlayItem parentOverlay = imageViewer.GetLastGroup();

					// _activeRoiRect???¨Ïö©?òÏó¨ ?§Ïù¥?¥Í∑∏??Ï∂îÍ?
					imageViewer.AddOverlay(parentOverlay.GroupType, parentOverlay.GroupType, activeRoiRect, activeRoiRect.UniqueId, parentOverlay.InspWindowType, EnumItemType.Window);

					callbackRoiAdded?.Invoke(activeRoiRect, parentOverlay);
				}
			}
		}
	}
}
