using System.Collections.Generic;

namespace OpenVisionLab.ImageCanvas.Model
{
	public class EraserParameter
	{
		private List<System.Drawing.Point> _eraserPointfs = new List<System.Drawing.Point>();
		private int _eraserWidth = 1;
		public List<System.Drawing.Point> EraserPointfs
		{
			get { return _eraserPointfs; }
			set
			{
				_eraserPointfs = value;
			}
		}

		public int EraserWidth
		{
			get => _eraserWidth;
			set
			{
				_eraserWidth = value;
			}
		}

		// 중복된 포인트를 제거하는 메소드 추가
		public void RemoveDuplicatePoints()
		{
			HashSet<System.Drawing.Point> uniquePoints = new HashSet<System.Drawing.Point>();
			foreach (var point in _eraserPointfs)
			{
				uniquePoints.Add(point);
			}
			_eraserPointfs = new List<System.Drawing.Point>(uniquePoints);
		}
	}
}
