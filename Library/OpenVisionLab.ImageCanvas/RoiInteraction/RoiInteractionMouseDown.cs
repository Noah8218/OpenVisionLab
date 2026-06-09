using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab.ImageCanvas
{
	public class RoiInteractionMouseDown
	{
		/// <summary>
		/// Viewer瑜??대┃ ?덉쓣 ??珥덇린 Mode? RoiRect瑜??좊떦?⑸땲??
		/// </summary>
		/// <param name="viewModel"></param>
		/// <param name="activeRoiRect"></param>
		/// <param name="openGLControl"></param>
		/// <param name="e"></param>
		public static void InitializeMouseDownState(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, ref CanvasRect<float> activeRoiRect, OpenGLControl openGLControl, CanvasMouseEventArgs e)
		{
			if (activeRoiRect == null) { activeRoiRect = new CanvasRect<float>(); }
			imageViewer.PreMousePos = imageViewer.GetCurrentRobotPos(e.X, e.Y);
			imageViewer.PostMousePos = imageViewer.PreMousePos;

			CanvasRect<float> previousRoiRect = activeRoiRect;
			previousRoiRect.IsEditing = false;
			previousRoiRect.IsChanged = true;

			activeRoiRect.SetEditingType(imageViewer.PreMousePos.X, imageViewer.PreMousePos.Y, imageViewer.ZoomScale, imageViewer.HandleSize);
			openGLControl.Cursor = RoiInteractionCursor.GetCursorFromType(activeRoiRect, imageViewer.PreMousePos, imageViewer.ZoomScale, imageViewer.HandleSize);

			// ?대떦 ?ъ씤?몄뿉 ?ㅼ씠?닿렇?⑥씠 ?덈뒗吏 ?뺤씤
			//imageViewer._targetGroupOverlay = null;
			var (targetOverlay, isGroupOverlay) = GetLeftClickOverlay(imageViewer, e);
			activeRoiRect.IsChanged = true; // ?댁쟾???⑥븘?덈뒗 Drawing ???곗씠?곕? 珥덇린???댁빞??
			activeRoiRect = targetOverlay == null ? new CanvasRect<float>() : targetOverlay;
			activeRoiRect.SetEditingType(imageViewer.PreMousePos.X, imageViewer.PreMousePos.Y, imageViewer.ZoomScale, imageViewer.HandleSize);
			openGLControl.Cursor = RoiInteractionCursor.GetCursorFromType(activeRoiRect, imageViewer.PreMousePos, imageViewer.ZoomScale, imageViewer.HandleSize);

			InitializeViewMode(imageViewer, activeRoiRect, isGroupOverlay);
		}

		private static void InitializeViewMode(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, CanvasRect<float> activeRoiRect, bool isGroupOverlay)
		{
			if (activeRoiRect.IsEmpty())
			{
				if (imageViewer.GetViewMode() != CanvasInteractionMode.Drawing && imageViewer.GetViewMode() != CanvasInteractionMode.Measure)
				{
					imageViewer.SetViewMode(CanvasInteractionMode.Drag);
				}
				return;
			}

			if (imageViewer.GetViewMode() == CanvasInteractionMode.Measure)
			{
				return;
			}

			activeRoiRect.IsChanged = true;
			imageViewer.SetViewMode(isGroupOverlay ? CanvasInteractionMode.Drag : CanvasInteractionMode.Edit);
			if (activeRoiRect.EditingType == EditingType.Move && imageViewer.GetViewMode() != CanvasInteractionMode.Drag)
			{
				imageViewer.SetViewMode(CanvasInteractionMode.Move);
			}
		}

		public static (CanvasRect<float> Overlay, bool IsGroupOverlay) FindOverlayAtPosition(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, System.Drawing.PointF currentRobotyPos, bool includeGroupRectangles = false)
		{
			List<CanvasOverlayItem> overlayItems = imageViewer.GetVisibleUnlockedOverlays();

			// ?ъ슜?먭? 吏곸젒 ?몄쭛?????덈뒗 ?ㅼ젣 Window ROI留??먯깋?⑸땲??
			CanvasRect<float> targetOverlay = overlayItems
				.Where(x => !x.IsGroupRectangle && x.ItemType == EnumItemType.Window)
				.Select(x => x.Shape)
				.OfType<CanvasRect<float>>()
				.Where(rect => rect.CheckHandleContainsPosition(currentRobotyPos.X, currentRobotyPos.Y, imageViewer.ZoomScale, imageViewer.HandleSize) != LineOverType.None)
				.OrderBy(rect => Math.Sqrt(Math.Pow(rect.Center.X - currentRobotyPos.X, 2) + Math.Pow(rect.Center.Y - currentRobotyPos.Y, 2)))
				.FirstOrDefault();

			bool isGroupOverlay = false;

			if (targetOverlay == null && includeGroupRectangles)
			{
				targetOverlay = overlayItems
					.Where(x => x.IsGroupRectangle)
					.Select(x => x.Shape)
					.OfType<CanvasRect<float>>()
					.Where(rect => rect.CheckHandleContainsPosition(currentRobotyPos.X, currentRobotyPos.Y, imageViewer.ZoomScale, imageViewer.HandleSize) != LineOverType.None)
					.OrderBy(rect => CalculateDistanceToRectangle(currentRobotyPos, new System.Drawing.RectangleF(rect.Left, rect.Top, rect.Width, rect.Height)))
					.FirstOrDefault();

				if (targetOverlay != null)
				{
					isGroupOverlay = true; // ??寃쎌슦?먮쭔 isGroupRectangle??true濡??ㅼ젙
				}
			}
			else
			{
				isGroupOverlay = overlayItems.Find(x => x.Shape == targetOverlay)?.IsGroupRectangle ?? false;
			}

			return (targetOverlay, isGroupOverlay);
		}

		private static (CanvasRect<float>, bool) GetLeftClickOverlay(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, CanvasMouseEventArgs e)
		{
			//if (!IsEdittingDrawing) { return (null, false); }
			var currentRobotyPos = imageViewer.GetCurrentRobotPos(e.X, e.Y);
			var (targetOverlay, isGroupOverlay) = FindOverlayAtPosition(imageViewer, currentRobotyPos);
			return (targetOverlay, isGroupOverlay);
		}

		public static double CalculateDistanceToRectangle(System.Drawing.PointF point, System.Drawing.RectangleF rect)
		{
			float dx = Math.Max(Math.Max(rect.Left - point.X, 0), point.X - rect.Right);
			float dy = Math.Max(Math.Max(rect.Top - point.Y, 0), point.Y - rect.Bottom);
			return Math.Sqrt(dx * dx + dy * dy);
		}

	}
}
