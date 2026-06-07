using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenVisionLab.ImageCanvas.Overlays
{
	public class CanvasOverlayItem
	{
		public string GroupType { get; set; } // 그룹 타입
		public bool IsVisible { get; set; } = true; // 해당 Group을 보일지 안보일지				
		public EnumInspWindowType InspWindowType { get; set; } = EnumInspWindowType.Panel;
		public bool IsExtentionRectange { get; set; } // 해당 Object에 Extention ROI를 추가할지 안할지 결정, 해당 ROI는 User가 화면에서 수정이 안됩니다.		
		public bool IsGroupRectangle { get; set; } // 해당 Object가 Group안에 ROI를 포함하는 Rectangle인지 결정합니다. 해당 ROI는 User가 화면에서 수정이 안됩니다.
		public bool IsFill { get; set; } // 해당 Object 안쪽을 가득 채울지 안채울지 플래그
		public bool IsControlLock { get; set; } = false;
		public EnumItemType ItemType { get; set; } = EnumItemType.Group;
		public CanvasShape Shape { get; set; }
		public List<CanvasOverlayItem> ChildObjects { get; set; } = new List<CanvasOverlayItem>(); // 추가한 하위 그룹		
		public CanvasOverlayItem Parent { get; set; }
		public System.Drawing.Color Color { get; set; } = Color.White;

		public void AddChildGroup(CanvasOverlayItem childGroup) => ChildObjects.Add(childGroup);
		public void RemoveChildGroup(CanvasOverlayItem childGroup) => ChildObjects.Remove(childGroup);
		public CanvasOverlayItem FindChildGroup(string type) => ChildObjects.FirstOrDefault(d => d.GroupType == type);

		public RectangleF GetGroupRectangle(bool includeGroupRect = false)
		{
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;

			bool findExtendedRectangle = false;

			foreach (var childOverlay in ChildObjects)
			{
				var rect = childOverlay.Shape as CanvasRect<float>;

				if (includeGroupRect)
				{
					GetLargestRectangleRecursive(childOverlay, ref minX, ref minY, ref maxX, ref maxY);
				}

				if (!includeGroupRect && childOverlay.IsGroupRectangle) { continue; }
				if (rect != null && !rect.IsEmpty())
				{
					RectangleF rt = rect.ToRobotRectangle();

					if (childOverlay.IsExtentionRectange)
					{
						findExtendedRectangle = true;
						var dots = rect.ExtendedRectangle.ShapePoints.ToArray();
						float shapeLeft = dots.Min(dot => dot.X);
						float shapeRight = dots.Max(dot => dot.X);
						float shapeTop = dots.Max(dot => dot.Y);
						float shapeBottom = dots.Min(dot => dot.Y);
						CanvasRect<float> xRect = new CanvasRect<float>(shapeLeft, shapeTop, shapeRight, shapeBottom);

						rt = xRect.ToRobotRectangle();
					}

					// 최소, 최대 X 및 Y 좌표 업데이트
					minX = Math.Min(minX, rt.Left);
					minY = Math.Min(minY, rt.Top);
					maxX = Math.Max(maxX, rt.Right);
					maxY = Math.Max(maxY, rt.Bottom);
				}
			}

			RectangleF groupRectangle = new RectangleF();
			// 전체 그룹을 포함하는 Rectangle 생성
			if (minX < float.MaxValue && minY < float.MaxValue)
			{
				groupRectangle = new RectangleF(minX, minY, maxX - minX, maxY - minY);

				int offset = 40;

				if (findExtendedRectangle) { offset = 60; }

				// 센터를 중심으로 상하좌우 10픽셀씩 늘리기
				float centerX = groupRectangle.X + groupRectangle.Width / 2;
				float centerY = groupRectangle.Y + groupRectangle.Height / 2;
				groupRectangle = new RectangleF(centerX - (groupRectangle.Width / 2) - offset, centerY - (groupRectangle.Height / 2) - offset, groupRectangle.Width + (offset * 2), groupRectangle.Height + (offset * 2));
			}

			return groupRectangle;
		}

		private void GetLargestRectangleRecursive(CanvasOverlayItem objects, ref float minX, ref float minY, ref float maxX, ref float maxY)
		{
			foreach (var child in objects.ChildObjects)
			{
				var rect = child.Shape as CanvasRect<float>;

				if (rect != null && !rect.IsEmpty())
				{
					RectangleF rt = rect.ToRobotRectangle();

					// 최소, 최대 X 및 Y 좌표 업데이트
					minX = Math.Min(minX, rt.Left);
					minY = Math.Min(minY, rt.Top);
					maxX = Math.Max(maxX, rt.Right);
					maxY = Math.Max(maxY, rt.Bottom);
				}

				GetLargestRectangleRecursive(child, ref minX, ref minY, ref maxX, ref maxY);
			}
		}

	}
}
