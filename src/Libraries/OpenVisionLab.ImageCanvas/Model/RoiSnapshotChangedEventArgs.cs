using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab.ImageCanvas.Model
{
	public sealed class RoiSnapshotChangedEventArgs : EventArgs
	{
		public RoiSnapshotChangedEventArgs(string actionName, IReadOnlyList<RoiSnapshotItem> before, IReadOnlyList<RoiSnapshotItem> after)
		{
			ActionName = string.IsNullOrWhiteSpace(actionName) ? "ROI" : actionName;
			Before = CloneSnapshot(before);
			After = CloneSnapshot(after);
		}

		public string ActionName { get; }
		public IReadOnlyList<RoiSnapshotItem> Before { get; }
		public IReadOnlyList<RoiSnapshotItem> After { get; }

		public static IReadOnlyList<RoiSnapshotItem> CloneSnapshot(IEnumerable<RoiSnapshotItem> snapshot)
		{
			return snapshot?.Select(item => item?.Clone()).Where(item => item != null).ToList()
				?? new List<RoiSnapshotItem>();
		}
	}

	public sealed class RoiSnapshotItem
	{
		public string UniqueId { get; set; }
		public string GroupType { get; set; }
		public string ParentGroupType { get; set; }
		public EnumInspWindowType InspWindowType { get; set; }
		public EnumItemType ItemType { get; set; }
		public bool IsExtensionRectangle { get; set; }
		public bool IsGroupRectangle { get; set; }
		public bool IsFill { get; set; }
		public float Left { get; set; }
		public float Top { get; set; }
		public float Right { get; set; }
		public float Bottom { get; set; }

		public static RoiSnapshotItem FromOverlay(CanvasOverlayItem overlay)
		{
			CanvasRect<float> rect = overlay?.Shape as CanvasRect<float>;
			if (rect == null || rect.IsEmpty())
			{
				return null;
			}

			return new RoiSnapshotItem
			{
				UniqueId = rect.UniqueId,
				GroupType = overlay.GroupType ?? rect.GroupType ?? string.Empty,
				ParentGroupType = overlay.Parent?.GroupType ?? string.Empty,
				InspWindowType = overlay.InspWindowType,
				ItemType = overlay.ItemType,
				IsExtensionRectangle = overlay.IsExtensionRectangle,
				IsGroupRectangle = overlay.IsGroupRectangle,
				IsFill = overlay.IsFill,
				Left = rect.Left,
				Top = rect.Top,
				Right = rect.Right,
				Bottom = rect.Bottom
			};
		}

		public RoiSnapshotItem Clone()
		{
			return new RoiSnapshotItem
			{
				UniqueId = UniqueId,
				GroupType = GroupType,
				ParentGroupType = ParentGroupType,
				InspWindowType = InspWindowType,
				ItemType = ItemType,
				IsExtensionRectangle = IsExtensionRectangle,
				IsGroupRectangle = IsGroupRectangle,
				IsFill = IsFill,
				Left = Left,
				Top = Top,
				Right = Right,
				Bottom = Bottom
			};
		}

		public CanvasRect<float> ToCanvasRect()
		{
			return new CanvasRect<float>(Left, Top, Right, Bottom)
			{
				UniqueId = UniqueId,
				GroupType = GroupType,
				IsFill = IsFill
			};
		}

		public bool HasSameGeometry(RoiSnapshotItem other)
		{
			if (other == null)
			{
				return false;
			}

			return string.Equals(UniqueId, other.UniqueId, StringComparison.Ordinal)
				&& string.Equals(GroupType, other.GroupType, StringComparison.Ordinal)
				&& string.Equals(ParentGroupType, other.ParentGroupType, StringComparison.Ordinal)
				&& InspWindowType == other.InspWindowType
				&& ItemType == other.ItemType
				&& IsExtensionRectangle == other.IsExtensionRectangle
				&& IsGroupRectangle == other.IsGroupRectangle
				&& IsFill == other.IsFill
				&& NearlyEquals(Left, other.Left)
				&& NearlyEquals(Top, other.Top)
				&& NearlyEquals(Right, other.Right)
				&& NearlyEquals(Bottom, other.Bottom);
		}

		private static bool NearlyEquals(float left, float right)
		{
			return Math.Abs(left - right) < 0.001f;
		}
	}
}
