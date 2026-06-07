using OpenVisionLab.ImageCanvas;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OpenVisionLab.ImageCanvas.Overlays
{
	/// <summary>
	/// 다이어그램 그룹들을 관리하는 클래스입니다.
	/// </summary>
	public class CanvasOverlayManager
	{
		#region Field		
		private List<CanvasOverlayItem> _overlayItems = new List<CanvasOverlayItem>();
		private string _lastGroupType = EnumInspWindowType.Module.ToString();
		#endregion
		#region Properties
		public string LastGroupType
		{
			get => _lastGroupType;
			set => _lastGroupType = value;
		}

		public Dictionary<EnumInspWindowType, System.Windows.Media.SolidColorBrush> GroupBrushes = new Dictionary<EnumInspWindowType, System.Windows.Media.SolidColorBrush>
		{
			{ EnumInspWindowType.Align, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange) },
			{ EnumInspWindowType.Module, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green) },
			//{ EnumInspWindowType.Target, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Aquamarine) },
			//{ EnumInspWindowType.Reference, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightSeaGreen) },
			{ EnumInspWindowType.Unit, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DodgerBlue) },
			//{ EnumInspWindowType.Warpage, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.MediumOrchid) },
			{ EnumInspWindowType.Thickness, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.SlateGray) },
			//{ EnumInspWindowType.Fitting, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.SandyBrown) },
		};
		#endregion

		private object _lock = new object();

		/// <summary>
		/// 모든 보이는 그룹(부모 및 자식 포함)의 다이어그램을 반환합니다.
		/// </summary>
		public List<CanvasOverlayItem> GetAllVisibleOverlays()
		{
			var allVisibleOverlays = new List<CanvasOverlayItem>();
			foreach (var group in GetOrderedGroups())
			{
				if (group.IsVisible)
				{
					allVisibleOverlays.Add(group);
				}
				AddChildGroupVisibleOverlays(group, allVisibleOverlays);
			}
			return allVisibleOverlays;
		}

		/// <summary>
		/// 최상위 Object인지 확인합니다.
		/// </summary>
		/// <param name="overlayItem"></param>
		/// <returns></returns>
		public bool IsTopLevelObject(CanvasOverlayItem overlayItem)
		{
			if (overlayItem.Parent == null)
			{
				return true;
			}
			return false;
		}

		public CanvasOverlayItem GetGroupToType(string childType)
		{
			foreach (var group in GetOrderedGroups())
			{
				if (group.GroupType == childType)
				{
					return group;
				}

				var parentOverlay = GetOverlayToType(group, childType);
				if (parentOverlay != null)
				{
					return parentOverlay;
				}
			}
			return null; // 찾지 못한 경우
		}

		public CanvasOverlayItem GetParentToType(string childType)
		{
			foreach (var group in GetOrderedGroups())
			{
				if (group.GroupType == childType)
				{
					return group.Parent;
				}

				var parentOverlay = GetOverlayToType(group, childType);
				if (parentOverlay != null)
				{
					return parentOverlay.Parent;
				}
			}

			return null;
		}


		private CanvasOverlayItem GetOverlayToType(CanvasOverlayItem group, string childType)
		{
			// 하위 그룹에서 재귀적으로 찾기
			foreach (var childGroup in group.ChildObjects)
			{
				if (childGroup.GroupType == childType)
				{
					return childGroup;
				}

				var parentOverlay = GetOverlayToType(childGroup, childType);
				if (parentOverlay != null)
				{
					return parentOverlay;
				}
			}

			return null; // 해당 그룹을 찾지 못한 경우
		}

		public string GetNewname(CanvasOverlayItem overlayItem)
		{
			string newName = "";
			int count = 0;
			while (true)
			{
				Thread.Sleep(1);
				bool isExist = false;
				newName = IncrementLastNumber(overlayItem.GroupType, count);
				//newName = $"{overlayItem.GroupType}-[{count}]";
				isExist = IsExistGroupTypeAll(newName);

				if (!isExist) { break; }
				count++;
			}
			return newName;
		}

		public bool IsExistGroupTypeAll(string name)
		{
			foreach (CanvasOverlayItem group in GetOrderedGroups())
			{
				if (group.GroupType == name)
				{
					return true;
				}

				if (IsExistGroupTypeRecursive(group, name))
				{
					return true;
				}
			}

			return false;
		}

		private bool IsExistGroupTypeRecursive(CanvasOverlayItem current, string name)
		{
			foreach (var child in current.ChildObjects.Select((value, index) => new { Value = value, Index = index }))
			{
				int currentIndex = child.Index;

				if (child.Value.GroupType == name)
				{
					return true;
				}

				// 재귀 호출의 결과를 확인하고, 참이면 반환
				if (IsExistGroupTypeRecursive(child.Value, name))
				{
					return true;
				}
			}
			return false;
		}

		public void RenameAllOverlay()
		{
			foreach (var group in GetOrderedGroups())
			{
				RenameAllOverlayParentGroup(group);
			}
			return;
		}

		private void RenameAllOverlayParentGroup(CanvasOverlayItem current, int currentIndex = 0)
		{
			for (int i = 0; i < current.ChildObjects.Count; i++)
			{
				CanvasOverlayItem childGroup = current.ChildObjects.ElementAt(i);
				string oldchildGroup = childGroup.GroupType;

				string newchildGroup = null;
				newchildGroup = IncrementLastNumber(oldchildGroup, i);
				childGroup.GroupType = newchildGroup;

				RenameAllOverlayParentGroup(childGroup, i);
			}

			return; // 해당 그룹을 찾지 못한 경우
		}

		private string IncrementLastNumber(string input, int currentIndex)
		{
			// '-'로 문자열을 분할
			var parts = input.Split('-');

			// 마지막 부분을 처리
			if (parts.Length > 0)
			{
				var lastPart = parts.Last();
				if (lastPart.StartsWith("[") && lastPart.EndsWith("]"))
				{
					int number;
					// '['와 ']' 사이의 숫자 부분 추출
					string numberStr = lastPart.Substring(1, lastPart.Length - 2);
					if (int.TryParse(numberStr, out number))
					{
						// 숫자 증가
						parts[parts.Length - 1] = $"[{currentIndex}]";
					}
				}
				else
				{
					parts[0] = $"{parts[0]}-{currentIndex}";
				}
			}

			// 다시 문자열을 조합
			return string.Join("-", parts);
		}


		/// <summary>
		/// 주어진 유니크 ID를 가진 다이어그램을 찾습니다.
		/// </summary>
		public CanvasOverlayItem GetOverlayByUniqueId(string uniqueId)
		{
			foreach (var group in GetOrderedGroups())
			{

				if (group.Shape != null && group.Shape.UniqueId == uniqueId)
				{
					return group;
				}

				var parentOverlay = GetOverlay(group, uniqueId);
				if (parentOverlay != null)
				{
					return parentOverlay;
				}
			}
			return null; // 찾지 못한 경우
		}

		private CanvasOverlayItem GetOverlay(CanvasOverlayItem group, string uniqueId)
		{
			foreach (var childGroup in group.ChildObjects)
			{
				if (childGroup.Shape.UniqueId == uniqueId)
				{
					return childGroup;
				}

				var parentOverlay = GetOverlay(childGroup, uniqueId);
				if (parentOverlay != null)
				{
					return parentOverlay;
				}
			}

			return null;
		}

		public bool RemoveOverlayByUniqueId(string uniqueId)
		{
			lock (_lock)
			{
				CanvasOverlayItem overlayItem = GetOverlayByUniqueId(uniqueId);
				if (overlayItem == null) { return false; }
				if (overlayItem.Parent != null)
				{
					CanvasOverlayItem parent = GetOverlayByUniqueId(overlayItem.Parent.Shape.UniqueId);
					parent?.ChildObjects.Remove(overlayItem);
				}
				else
				{
					_overlayItems.Remove(overlayItem);
				}
				return true;
			}
		}

		public void AddOverlayItem(string parentType, CanvasOverlayItem newObject)
		{
			lock (_lock)
			{
				if (parentType == "")
				{
					_overlayItems.Add(newObject);
				}
				else
				{
					CanvasOverlayItem parent = GetGroupToType(parentType);

					if (parent != null)
					{
						newObject.Parent = parent;

						parent.AddChildGroup(newObject);
					}
					else
					{
						_overlayItems.Add(newObject);						
					}
				}
			}
		}

		private CanvasOverlayItem FindGroupRecursive(CanvasOverlayItem currentGroup, Queue<string> groupPath)
		{
			if (groupPath.Count == 1) return currentGroup;

			string nextGroupName = groupPath.Peek(); // 현재 처리할 그룹 이름을 가져옴
			var nextGroup = currentGroup.FindChildGroup(nextGroupName);

			if (nextGroup == null)
			{
				if (currentGroup.ChildObjects.Count == 0) { return null; }
				else
				{
					foreach (var childGroup in currentGroup.ChildObjects)
					{
						return FindGroupRecursive(childGroup, groupPath);
					}
				}
			}

			groupPath.Dequeue(); // 다음 그룹으로 이동
			return FindGroupRecursive(nextGroup, groupPath);
		}

		public IEnumerable<CanvasOverlayItem> GetOverlaysByGroupType(string groupType)
		{
			var allVisibleOverlays = new List<CanvasOverlayItem>();
			foreach (var group in GetOrderedGroups())
			{
				if (group.GroupType == groupType)
				{
					allVisibleOverlays.Add(group);
				}
				AddChildGroupCommonTypeOverlays(group, groupType, allVisibleOverlays);
			}
			return allVisibleOverlays;
		}

		private void AddChildGroupCommonTypeOverlays(CanvasOverlayItem group, string groupType, List<CanvasOverlayItem> overlayItems)
		{
			foreach (var childGroup in group.ChildObjects)
			{
				if (childGroup.GroupType == groupType)
				{
					overlayItems.Add(childGroup);
				}
				AddChildGroupCommonTypeOverlays(childGroup, groupType, overlayItems); // 재귀적으로 보이는 자식 그룹들의 다이어그램을 추가
			}
		}

		public IEnumerable<CanvasOverlayItem> GetAllVisibleUnlockedOverlays()
		{
			var allVisibleOverlays = new List<CanvasOverlayItem>();
			foreach (var group in GetOrderedGroups())
			{
				if (group.IsVisible && !group.IsControlLock)
				{
					allVisibleOverlays.Add(group);
				}
				AddChildGroupWhereOverlays(group, allVisibleOverlays);
			}
			return allVisibleOverlays;
		}

		private void AddChildGroupWhereOverlays(CanvasOverlayItem group, List<CanvasOverlayItem> overlayItems)
		{
			foreach (var childGroup in group.ChildObjects)
			{
				if (childGroup.IsVisible && !childGroup.IsControlLock)
				{
					overlayItems.Add(childGroup);
				}
				AddChildGroupWhereOverlays(childGroup, overlayItems); // 재귀적으로 보이는 자식 그룹들의 다이어그램을 추가
			}
		}

		private void AddChildGroupVisibleOverlays(CanvasOverlayItem group, List<CanvasOverlayItem> overlayItems)
		{
			foreach (var childGroup in group.ChildObjects)
			{
				if (childGroup.IsVisible)
				{
					overlayItems.Add(childGroup);
				}
				AddChildGroupVisibleOverlays(childGroup, overlayItems); // 재귀적으로 보이는 자식 그룹들의 다이어그램을 추가
			}
		}

		/// <summary>
		/// 모든 그룹(부모 및 자식 포함)의 다이어그램을 반환합니다.
		/// </summary>
		public IEnumerable<CanvasOverlayItem> GetAllOverlays()
		{
			var allOverlays = new List<CanvasOverlayItem>();
			foreach (var group in GetOrderedGroups())
			{
				allOverlays.Add(group);
				AddChildGroupOverlays(group, allOverlays);
			}

			return allOverlays;
		}

		private void AddChildGroupOverlays(CanvasOverlayItem group, List<CanvasOverlayItem> overlayItems)
		{
			foreach (var childGroup in group.ChildObjects)
			{
				overlayItems.Add(childGroup);
				AddChildGroupOverlays(childGroup, overlayItems); // 재귀적으로 자식 그룹들의 다이어그램을 추가
			}
		}

		/// <summary>
		/// 추가된 순서대로 그룹 리스트를 가져옵니다.
		/// </summary>
		public IEnumerable<CanvasOverlayItem> GetOrderedGroups()
		{
			lock (_lock)
			{
				return _overlayItems;
			}
		}

		public void Clear()
		{
			lock (_lock)
			{
				foreach (var topGroup in GetOrderedGroups())
				{
					ClearChildGroups(topGroup);
				}

				_overlayItems.Clear();
				_lastGroupType = EnumInspWindowType.Module.ToString();
			}
		}

		public void ClearChildGroups(CanvasOverlayItem group)
		{
			if (group == null) { return; }
			foreach (var childGroup in group.ChildObjects)
			{
				ClearChildGroups(childGroup); // 재귀적으로 하위 그룹 클리어
			}

			group.ChildObjects.Clear(); // 현재 그룹의 자식 그룹 클리어
										//group.Overlays.Clear(); // 현재 그룹의 다이어그램 클리어
		}

		/// <summary>
		/// 특정 타입의 Group을 숨기거나 보여줍니다.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="visible"></param>
		public void SetVisible(string type)
		{
			foreach (var topGroup in GetOrderedGroups())
			{
				var targetGroup = FindGroupRecursive(topGroup, type);
				if (targetGroup != null)
				{
					targetGroup.IsVisible = !targetGroup.IsVisible;
					break;
				}
			}
		}

		public void SetAllVisible(bool visible)
		{
			foreach (var topGroup in GetOrderedGroups())
			{
				topGroup.IsVisible = visible;
				VisibleGroupRecursive(topGroup, visible);
			}
		}

		private void VisibleGroupRecursive(CanvasOverlayItem currentGroup, bool visible)
		{
			lock (_lock)
			{
				foreach (var childGroup in currentGroup.ChildObjects)
				{
					childGroup.IsVisible = visible;
					VisibleGroupRecursive(childGroup, visible);
				}
			}
		}

		public void SetVisible(string type, bool visible)
		{
			foreach (var topGroup in GetOrderedGroups())
			{
				var targetGroup = FindGroupRecursive(topGroup, type);
				if (targetGroup != null)
				{
					targetGroup.IsVisible = visible;
					targetGroup.ChildObjects.Where(x => x.IsGroupRectangle == false).ToList().ForEach(v => v.IsVisible = visible);
				}
			}
		}

		public void SetVisibleExceptionTypeName(string typeName, bool visible)
		{
			foreach (var topGroup in GetOrderedGroups())
			{
				var targetGroup = FindGroupRecursive(topGroup, typeName);
				if (targetGroup != null)
					continue;
				topGroup.IsVisible = visible;
				topGroup.ChildObjects.Where(x => x.IsGroupRectangle == false).ToList().ForEach(v => v.IsVisible = visible);
			}
		}

		public void SetLockControl(string type, bool isControlLock)
		{
			foreach (var topGroup in GetOrderedGroups())
			{
				var targetGroup = FindGroupRecursive(topGroup, type);
				if (targetGroup != null)
				{
					targetGroup.IsControlLock = isControlLock;
					targetGroup.ChildObjects.Where(x => x.IsGroupRectangle == false).ToList().ForEach(v => v.IsControlLock = isControlLock);
					break;
				}
			}
		}

		private CanvasOverlayItem FindGroupRecursive(CanvasOverlayItem currentGroup, string groupName)
		{
			if (currentGroup.GroupType == groupName)
			{
				return currentGroup;
			}

			foreach (var childGroup in currentGroup.ChildObjects)
			{
				var foundGroup = FindGroupRecursive(childGroup, groupName);
				if (foundGroup != null)
				{
					return foundGroup;
				}
			}

			return null; // 해당 그룹을 찾지 못한 경우
		}
	}
}
