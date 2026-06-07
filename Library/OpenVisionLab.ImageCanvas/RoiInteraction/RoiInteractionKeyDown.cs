using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using System;

namespace OpenVisionLab.ImageCanvas
{
	public class RoiInteractionKeyDown
	{
		/// <summary>
		/// ?�재 ?�택???�각??(_activeRoiRect)??복사?�는 로직 구현			
		/// </summary>
		public static void CopyRectangle(CanvasRect<float> activeRoiRect, ref CanvasRect<float> copyRoiRect)
		{
			if (activeRoiRect != null) { copyRoiRect = activeRoiRect.DeepClone() as CanvasRect<float>; }
		}

		/// <summary>
		/// ?�립보드 ?�는 ?�시 변?�에??복사???�각?�을 가?��???붙여?�는 로직 구현			
		/// </summary>
		public static void PasteRectangle(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, ref CanvasRect<float> copyRoiRect, OverlayAddedCallback callbackRoiAdded, OverlayGroupAddedCallback callbackGroupAddition)
		{
			if (copyRoiRect == null) return;

			CanvasOverlayItem overlayItem = imageViewer.GetOverlayByUniqueId(copyRoiRect.UniqueId);

			if (overlayItem.IsGroupRectangle)
			{
				CopyGroupAddition(imageViewer, overlayItem, -60, 60, callbackGroupAddition);
			}
			else
			{
				CanvasRect<float> canvasRect = CreateOffsetRect(copyRoiRect, 20, 20);

				CanvasOverlayItem parentOverlay = imageViewer.GetGroupToType(copyRoiRect.GroupType);
				AddNewOverlay(imageViewer, parentOverlay.GroupType, canvasRect.GroupType, canvasRect, canvasRect.UniqueId, parentOverlay.InspWindowType, EnumItemType.Window);

				// 콜백 ?�출
				callbackRoiAdded?.Invoke(canvasRect, parentOverlay);
			}

			copyRoiRect = null;
		}

		private static void CopyGroupAddition(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, CanvasOverlayItem copyGroupOb, float row, float col, OverlayGroupAddedCallback callbackGroupAddition)
		{

			CanvasOverlayItem parentOb = imageViewer.GetParentToType(copyGroupOb.GroupType);
			string groupNewname = imageViewer.GetNewOverlayName(copyGroupOb);

			CanvasRect<float> rect = CreateOffsetRect((CanvasRect<float>)copyGroupOb.Shape, col, row * -1);
			AddNewOverlay(imageViewer, parentOb.GroupType, groupNewname, rect, rect.UniqueId, copyGroupOb.InspWindowType, EnumItemType.Group);

			foreach (CanvasOverlayItem overlayItem in copyGroupOb.ChildObjects)
			{
				CanvasRect<float> xRect = CreateOffsetRect((CanvasRect<float>)overlayItem.Shape, col, row * -1);
				AddNewOverlay(imageViewer, groupNewname, groupNewname, xRect, xRect.UniqueId, copyGroupOb.InspWindowType, EnumItemType.Window);
			}


			OpenVisionLab.ImageCanvas.Model.RoiChangedEventArgs arg = new OpenVisionLab.ImageCanvas.Model.RoiChangedEventArgs();
			arg.Group = imageViewer.GetGroupToType(groupNewname);

			callbackGroupAddition?.Invoke(arg);
		}

		private static CanvasRect<float> CreateOffsetRect(CanvasRect<float> sourceRect, float offsetX, float offsetY)
		{
			CanvasSize<float> offsetSize = new CanvasSize<float>()
			{
				Width = offsetX,
				Height = offsetY
			};

			CanvasRect<float> newRect = sourceRect.ToCanvasRect();
			newRect.GroupType = sourceRect.GroupType;
			newRect.UniqueId = Guid.NewGuid().ToString();
			newRect.OffsetMove(offsetSize, false);
			return newRect;
		}

		private static void AddNewOverlay(OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl imageViewer, string groupType, string groupNewName, CanvasRect<float> shape, string uniqueId, EnumInspWindowType inspWindowType, EnumItemType itemType, bool isExtentionRectange = false)
		{
			bool isGroupRectangle = itemType == EnumItemType.Group ? true : false;
			imageViewer.AddOverlay(groupType, groupNewName, shape, uniqueId, inspWindowType, itemType, isExtentionRectange, isGroupRectangle);
		}
	}
}
