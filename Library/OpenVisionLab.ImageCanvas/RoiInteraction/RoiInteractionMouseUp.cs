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

			// ROI瑜??뺤쓽?섎뒗 RectangleF 媛앹껜 ?앹꽦
			RectangleF roi = new RectangleF(preMousePos.X, preMousePos.Y, postMousePos.X - preMousePos.X, postMousePos.Y - preMousePos.Y);
			if (roi.Width == 0 || roi.Height == 0) return false;

			// _activeRoiRect??吏곸젒 珥덇린?뷀븯怨? UniqueId ?ㅼ젙
			activeRoiRect = new CanvasRect<float>(roi.Left, roi.Top, roi.Right, roi.Bottom)
			{
				UniqueId = Guid.NewGuid().ToString()
			};

			// 留덉?留?洹몃９??媛?몄샂
			CanvasOverlayItem parentOverlay = imageViewer.GetLastGroup();
			if (parentOverlay == null) return false;

			// _activeRoiRect???ъ슜?섏뿬 ?ㅼ씠?닿렇??異붽?
			imageViewer.AddOverlay(parentOverlay.GroupType, parentOverlay.GroupType, activeRoiRect, activeRoiRect.UniqueId, parentOverlay.InspWindowType, EnumItemType.Window);

			// MouseUp ?대깽??泥섎━瑜??꾪븳 異붽? 濡쒖쭅
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
			// 湲곕낯 ROI 怨꾩궛
			RectangleF baseRoi = new RectangleF(preMousePos.X, preMousePos.Y, postMousePos.X - preMousePos.X, postMousePos.Y - preMousePos.Y);

			float pixelPerMm = 1 / pixelPermm; // 1?쎌???0.12mm?먯꽌 1mm???쎌? ?섎줈 蹂??

			// 媛꾧꺽??mm ?⑥쐞濡??ㅼ젙
			float rowSpacingInMm = roiArrayData.RowSpacing; // ?덈? ?ㅼ뼱 70mm
			float columnSpacingInMm = roiArrayData.ColumnSpacing; // ?덈? ?ㅼ뼱 46mm

			// mm ?⑥쐞 媛꾧꺽???쎌? ?⑥쐞濡?蹂??
			float rowSpacingInPixels = rowSpacingInMm * pixelPerMm;
			float columnSpacingInPixels = columnSpacingInMm * pixelPerMm;


			// 媛??됯낵 ?댁뿉 ???ROI 異붽?
			for (int row = 0; row < roiArrayData.Rows; row++)
			{
				for (int column = 0; column < roiArrayData.Columns; column++)
				{
					// ?꾩옱 ROI???꾩튂 怨꾩궛 (mm ?⑥쐞 媛꾧꺽???ъ슜)
					float currentX = baseRoi.X + column * (columnSpacingInPixels);
					float currentY = baseRoi.Y - row * (rowSpacingInPixels);

					// ROI瑜??뺤쓽?섎뒗 RectangleF 媛앹껜 ?앹꽦
					RectangleF currentRoi = new RectangleF(currentX, currentY, baseRoi.Width, baseRoi.Height);

					// _activeRoiRect??吏곸젒 珥덇린?뷀븯怨? UniqueId ?ㅼ젙
					CanvasRect<float> activeRoiRect = new CanvasRect<float>(currentRoi.Left, currentRoi.Top, currentRoi.Right, currentRoi.Bottom)
					{
						UniqueId = Guid.NewGuid().ToString()
					};

					// 留덉?留?洹몃９??媛?몄샂
					CanvasOverlayItem parentOverlay = imageViewer.GetLastGroup();

					// _activeRoiRect???ъ슜?섏뿬 ?ㅼ씠?닿렇??異붽?
					imageViewer.AddOverlay(parentOverlay.GroupType, parentOverlay.GroupType, activeRoiRect, activeRoiRect.UniqueId, parentOverlay.InspWindowType, EnumItemType.Window);

					callbackRoiAdded?.Invoke(activeRoiRect, parentOverlay);
				}
			}
		}
	}
}
