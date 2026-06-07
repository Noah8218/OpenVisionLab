using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using OpenVisionLab.ImageCanvas.Rendering;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenVisionLab.ImageCanvas.OpenGLRendering
{
	public static class OpenGlOverlayExtensions
	{
		#region Overlay
		public static void AddOverlay(this ImageCanvasControl canvasViewer, string parentType, string childType, CanvasShape shape, string uniqueId, EnumInspWindowType InspType = EnumInspWindowType.Unit, EnumItemType itemType = EnumItemType.Window, bool isExtentionRectange = false, bool isGroupRectangle = false)
		{
			shape.UniqueId = uniqueId;
			shape.GroupType = childType;

			// 새로운 Shape를 추가합니다.
			canvasViewer.GetCanvasOverlayManager().AddOverlayItem(parentType, CreateNewCanvasOverlayItem(canvasViewer, childType, shape, InspType, itemType, isExtentionRectange, isGroupRectangle));

			// 추가한 Group(Type)에 여러 ROI들이 들어가 있는 상황입니다.
			// 해당 Group Roi 사이즈를 다시 계산합니다.
			ResizeGroupRectangle(canvasViewer, parentType);
			// Panel은 전체를 감싸야 합니다.
			ResizeGroupRectangle(canvasViewer, EnumInspWindowType.Panel.ToString());
		}

		private static CanvasOverlayItem CreateNewCanvasOverlayItem(ImageCanvasControl canvasViewer, string childType, CanvasShape shape, EnumInspWindowType InspType, EnumItemType itemType, bool isExtentionRectange, bool isGroupRectangle)
		{
			System.Drawing.Color drawColor = System.Drawing.Color.White;
			// Group별 색을 설정합니다.
			if (canvasViewer.GetCanvasOverlayManager().GroupBrushes.TryGetValue(InspType, out System.Windows.Media.SolidColorBrush brush))
			{
				drawColor = System.Drawing.Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B);
			}

			CanvasOverlayItem newObject = new CanvasOverlayItem { GroupType = childType, Shape = shape, ItemType = itemType, InspWindowType = InspType, IsExtentionRectange = isExtentionRectange, IsGroupRectangle = isGroupRectangle, Color = drawColor };
			ConnectOverlayCallback(newObject, canvasViewer.GetOpenGL());

			return newObject;
		}

		public static void UpdateOverlays(this ImageCanvasControl canvasViewer)
		{
			// 모든 CanvasShape 객체를 순회합니다.
			foreach (CanvasOverlayItem overlayItem in canvasViewer.GetCanvasOverlayManager().GetAllOverlays())
			{
				overlayItem.Shape.IsChanged = true;
			}
		}

		/// <summary>
		/// 변경점이 감지가 되면 자동으로 업데이트하는 콜벡을 연결합니다.
		/// </summary>
		/// <param name="newObject"></param>
		private static void ConnectOverlayCallback(CanvasOverlayItem newObject, OpenGL gl)
		{
			newObject.Shape.OnChanged = () =>
			{
				if (newObject.Shape.IsChanged)
				{
					OpenGlDrawing.CompileOverlayShape(gl, newObject);
					newObject.Shape.IsChanged = false;
				}
			};
			newObject.Shape.IsChanged = true;
		}

		public static void DeleteOverlay(this ImageCanvasControl canvasViewer, string uniqueId = "", string groupName = "")
		{
			CanvasOverlayItem overlayItem = canvasViewer.GetCanvasOverlayManager().GetOverlayByUniqueId(uniqueId);
			ReleaseDisplayList(canvasViewer.GetOpenGL(), overlayItem?.Shape);
			bool removed = canvasViewer.GetCanvasOverlayManager().RemoveOverlayByUniqueId(uniqueId);
			if (removed)
			{
				ResizeGroupRectangle(canvasViewer, groupName);
			}
			canvasViewer.RefreshGL();
		}

		private static void ReleaseDisplayList(OpenGL gl, CanvasShape shape)
		{
			if (gl == null || shape == null) { return; }

			if (shape.DisplayListId != 0)
			{
				gl.DeleteLists(shape.DisplayListId, 1);
				shape.DisplayListId = 0;
			}

			if (shape is CanvasRect<float> rect && rect.ExtendedRectangle != null && rect.ExtendedRectangle.DisplayListId != 0)
			{
				gl.DeleteLists(rect.ExtendedRectangle.DisplayListId, 1);
				rect.ExtendedRectangle.DisplayListId = 0;
			}
		}

		public static List<CanvasOverlayItem> GetVisibleUnlockedOverlays(this ImageCanvasControl canvasViewer)
		{
			return canvasViewer.GetCanvasOverlayManager().GetAllVisibleUnlockedOverlays().ToList();
		}

		private static bool CheckMoveWithinBounds(CanvasRect<float> roi, CanvasSize<float> moveSize, Size imageSize)
		{
			CanvasRect<float> check = (CanvasRect<float>)roi.Clone();
			check.OffsetMove(moveSize, false);
			return IsRoiWithinImageBounds(check, imageSize);
		}

		/// <summary>
		/// 상하좌우 방향을 검사하여 roi가 이동 가능한지 검사합니다.
		/// </summary>
		/// <param name="roi"></param>
		/// <param name="size"></param>
		/// <param name="imageSize"></param>
		/// <returns></returns>
		private static CanvasSize<float> GetPossibleMoveDirection(CanvasRect<float> roi, CanvasSize<float> size, Size imageSize)
		{
			bool canMoveLeft = CheckMoveWithinBounds(roi, new CanvasSize<float>(-Math.Abs(size.Width), 0), imageSize);
			bool canMoveRight = CheckMoveWithinBounds(roi, new CanvasSize<float>(Math.Abs(size.Width), 0), imageSize);
			bool canMoveUp = CheckMoveWithinBounds(roi, new CanvasSize<float>(0, Math.Abs(size.Height)), imageSize);
			bool canMoveDown = CheckMoveWithinBounds(roi, new CanvasSize<float>(0, -Math.Abs(size.Height)), imageSize);

			CanvasSize<float> move = new CanvasSize<float>(0, 0);
			if (canMoveLeft && size.Width < 0)
			{
				move.Width = size.Width;
			}
			else if (canMoveRight && size.Width > 0)
			{
				move.Width = size.Width;
			}

			if (canMoveUp && size.Height > 0)
			{
				move.Height = size.Height;
			}
			else if (canMoveDown && size.Height < 0)
			{
				move.Height = size.Height;
			}

			return move;
		}

		/// <summary>
		/// ROI가 이미지 사이즈를 넘어서 Move가 안되도록 막습니다.
		/// </summary>
		/// <param name="roi"></param>
		/// <param name="size"></param>
		/// <param name="imageSize"></param>
		public static void PerformRoiMove(this ImageCanvasControl canvasViewer, CanvasRect<float> roi, CanvasSize<float> size, Size imageSize, bool canMoveRoi)
		{
			if (IsRoiWithinImageBounds(roi, imageSize))
			{
				if (CheckMoveWithinBounds(roi, size, imageSize))
				{
					roi.OffsetMove(size); // roi move
				}
			}
			else
			{
				CanvasSize<float> move = GetPossibleMoveDirection(roi, size, imageSize);
				if (move.Width != 0 || move.Height != 0 || !canMoveRoi)
				{
					roi.OffsetMove(move);
				}
				else
				{
					// 이동할 수 없는 경우 처리
					roi.OffsetMove(size);
				}
			}
		}

		private static bool IsRoiWithinImageBounds(CanvasRect<float> roi, Size imageSize)
		{
			// 이미 정렬된 ROI 점들을 사용
			CanvasPoint<float> leftTop = roi.LeftTop;
			CanvasPoint<float> rightTop = roi.RightTop;
			CanvasPoint<float> rightBottom = roi.RightBottom;
			CanvasPoint<float> leftBottom = roi.LeftBottom;

			bool isRoiWithinImageBoundsX = false;
			bool isRoiWithinImageBoundsY = false;

			if (leftBottom.X >= 0 && rightTop.X <= imageSize.Width)// X축 체크
			{
				isRoiWithinImageBoundsX = true;
			}
			if ((leftTop.Y) <= imageSize.Height && (leftBottom.Y) >= 0)// Y축 체크
			{
				isRoiWithinImageBoundsY = true;
			}

			return isRoiWithinImageBoundsX & isRoiWithinImageBoundsY;
		}

		public static void ResizeGroupRectangle(this ImageCanvasControl canvasViewer, string targetGroup)
		{
			if (targetGroup == "") { return; }
			CanvasOverlayItem currentGroup = canvasViewer.GetCanvasOverlayManager().GetGroupToType(targetGroup);
			if (currentGroup == null) { return; }
			RectangleF rt = currentGroup.GetGroupRectangle(canvasViewer.GetCanvasOverlayManager().IsTopLevelObject(currentGroup));
			CanvasShape target = currentGroup.Shape;

			if (target != null)
			{
				// 계산된 Group Rectangle을 사용합니다.
				(target as CanvasRect<float>).UpdateRectangle(0, 0, 0, 0);
				(target as CanvasRect<float>).UpdateRectangle(rt.Left, rt.Top, rt.Right, rt.Bottom);
				target.IsChanged = true;
			}
		}

		public static void SetLockControl(this ImageCanvasControl canvasViewer, string type, bool isControlLock) => canvasViewer.GetCanvasOverlayManager().SetLockControl(type, isControlLock);
		public static void SetLastGroupType(this ImageCanvasControl canvasViewer, string type) => canvasViewer.GetCanvasOverlayManager().LastGroupType = type;
		public static void SetVisible(this ImageCanvasControl canvasViewer, string type, bool visible) => canvasViewer.GetCanvasOverlayManager().SetVisible(type, visible);
		public static void SetAllVisible(this ImageCanvasControl canvasViewer, bool visible) => canvasViewer.GetCanvasOverlayManager().SetAllVisible(visible);
		public static CanvasOverlayItem GetLastGroup(this ImageCanvasControl canvasViewer) => canvasViewer.GetCanvasOverlayManager().GetGroupToType(canvasViewer.GetCanvasOverlayManager().LastGroupType);
		public static CanvasOverlayItem GetGroupToType(this ImageCanvasControl canvasViewer, string childType) => canvasViewer.GetCanvasOverlayManager().GetGroupToType(childType);
		public static CanvasOverlayItem GetOverlayByUniqueId(this ImageCanvasControl canvasViewer, string uniqueid) => canvasViewer.GetCanvasOverlayManager().GetOverlayByUniqueId(uniqueid);
		public static CanvasOverlayItem GetParentToType(this ImageCanvasControl canvasViewer, string childType) => canvasViewer.GetCanvasOverlayManager().GetParentToType(childType);
		public static string GetNewOverlayName(this ImageCanvasControl canvasViewer, CanvasOverlayItem overlayItem) => canvasViewer.GetCanvasOverlayManager().GetNewname(overlayItem);
		public static void ClearOverlays(this ImageCanvasControl canvasViewer)
		{
			ReleaseDisplayLists(canvasViewer);
			canvasViewer.GetCanvasOverlayManager().Clear();
			canvasViewer.RefreshGL();
		}

		private static void ReleaseDisplayLists(ImageCanvasControl canvasViewer)
		{
			OpenGL gl = canvasViewer.GetOpenGL();
			foreach (CanvasOverlayItem overlayItem in canvasViewer.GetCanvasOverlayManager().GetAllOverlays())
			{
				ReleaseDisplayList(gl, overlayItem?.Shape);
			}
		}
		#endregion
	}
}
