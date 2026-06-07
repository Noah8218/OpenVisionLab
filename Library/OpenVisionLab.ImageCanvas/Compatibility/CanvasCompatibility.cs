using OpenVisionLab.ImageCanvas.OpenGLRendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenVisionLab.ImageCanvas.CanvasShapes
{
	using OpenVisionLab.ImageCanvas.CanvasShapes;

	public enum EditingType
	{
		None,
		Left,
		Right,
		Top,
		Bottom,
		LeftTop,
		RightTop,
		RightBottom,
		LeftBottom,
		Move
	}

	public enum LineOverType
	{
		None,
		HSplit,
		VSplit,
		SizeNWSE,
		SizeNESE,
		SizeNESW,
		Move2D
	}

	public struct CanvasPoint<T>
	{
		public CanvasPoint(T x, T y)
		{
			X = x;
			Y = y;
		}

		public T X { get; set; }
		public T Y { get; set; }

		public PointF ToPointF() => new PointF(Convert.ToSingle(X), Convert.ToSingle(Y));
	}

	public struct CanvasSize<T>
	{
		public CanvasSize(T width, T height)
		{
			Width = width;
			Height = height;
		}

		public T Width { get; set; }
		public T Height { get; set; }
	}

	public abstract class CanvasShape : ICloneable
	{
		public string UniqueId { get; set; }
		public string GroupType { get; set; }
		public bool IsChanged { get; set; } = true;
		public Action OnChanged { get; set; }
		public uint DisplayListId { get; set; }
		public List<DotInfo> ShapePoints { get; protected set; } = new List<DotInfo>();

		public virtual object Clone() => MemberwiseClone();
	}

	public sealed class CanvasRect<T> : CanvasShape
	{
		private float _left;
		private float _top;
		private float _right;
		private float _bottom;

		public CanvasRect()
		{
			UpdateRectangle(0, 0, 0, 0);
		}

		public CanvasRect(float left, float top, float right, float bottom)
		{
			UpdateRectangle(left, top, right, bottom);
		}

		public float Left => Math.Min(_left, _right);
		public float Right => Math.Max(_left, _right);
		public float Top => Math.Max(_top, _bottom);
		public float Bottom => Math.Min(_top, _bottom);
		public float Width => Math.Abs(_right - _left);
		public float Height => Math.Abs(_top - _bottom);
		public float LineWidth { get; set; } = 1.0f;
		public bool IsEditing { get; set; }
		public bool IsFill { get; set; }
		public EditingType EditingType { get; private set; }
		public CanvasRect<T> ExtendedRectangle { get; set; }

		public CanvasPoint<float> LeftTop => new CanvasPoint<float>(Left, Top);
		public CanvasPoint<float> RightTop => new CanvasPoint<float>(Right, Top);
		public CanvasPoint<float> RightBottom => new CanvasPoint<float>(Right, Bottom);
		public CanvasPoint<float> LeftBottom => new CanvasPoint<float>(Left, Bottom);
		public CanvasPoint<float> Center => new CanvasPoint<float>((Left + Right) / 2.0f, (Top + Bottom) / 2.0f);
		public IEnumerable<CanvasPoint<float>> Points => new[] { LeftTop, RightTop, RightBottom, LeftBottom };

		public void UpdateRectangle(float left, float top, float right, float bottom)
		{
			_left = left;
			_top = top;
			_right = right;
			_bottom = bottom;
			RefreshDots();
			IsChanged = true;
			OnChanged?.Invoke();
		}

		public bool IsEmpty() => Width <= 0 || Height <= 0;
		public bool Contain(float x, float y) => x >= Left && x <= Right && y >= Bottom && y <= Top;
		public RectangleF ToRobotRectangle() => new RectangleF(Left, Bottom, Width, Height);
		public Rectangle ToImageArea() => Rectangle.Round(new RectangleF(Left, Bottom, Width, Height));
		public CanvasRect<T> ToCanvasRect() => new CanvasRect<T>(Left, Top, Right, Bottom) { UniqueId = UniqueId, GroupType = GroupType, LineWidth = LineWidth, IsFill = IsFill };
		public object DeepClone() => ToCanvasRect();

		public void OffsetMove(CanvasSize<float> size, bool notify = true)
		{
			_left += size.Width;
			_right += size.Width;
			_top += size.Height;
			_bottom += size.Height;
			RefreshDots();
			IsChanged = true;
			if (notify) OnChanged?.Invoke();
		}

		public void Move(float x, float y, Size imageSize)
		{
			switch (EditingType)
			{
				case EditingType.Left:
					_left = Clamp(x, 0, imageSize.Width);
					break;
				case EditingType.Right:
					_right = Clamp(x, 0, imageSize.Width);
					break;
				case EditingType.Top:
					_top = Clamp(y, 0, imageSize.Height);
					break;
				case EditingType.Bottom:
					_bottom = Clamp(y, 0, imageSize.Height);
					break;
				case EditingType.LeftTop:
					_left = Clamp(x, 0, imageSize.Width);
					_top = Clamp(y, 0, imageSize.Height);
					break;
				case EditingType.RightTop:
					_right = Clamp(x, 0, imageSize.Width);
					_top = Clamp(y, 0, imageSize.Height);
					break;
				case EditingType.RightBottom:
					_right = Clamp(x, 0, imageSize.Width);
					_bottom = Clamp(y, 0, imageSize.Height);
					break;
				case EditingType.LeftBottom:
					_left = Clamp(x, 0, imageSize.Width);
					_bottom = Clamp(y, 0, imageSize.Height);
					break;
			}

			RefreshDots();
			IsChanged = true;
			OnChanged?.Invoke();
		}

		public void SetEditingType(float x, float y, float zoomScale, float handleSize)
		{
			float tolerance = GetHitTolerance(zoomScale, handleSize);

			if (Distance(x, y, Left, Top) <= tolerance)
			{
				EditingType = EditingType.LeftTop;
			}
			else if (Distance(x, y, Right, Top) <= tolerance)
			{
				EditingType = EditingType.RightTop;
			}
			else if (Distance(x, y, Right, Bottom) <= tolerance)
			{
				EditingType = EditingType.RightBottom;
			}
			else if (Distance(x, y, Left, Bottom) <= tolerance)
			{
				EditingType = EditingType.LeftBottom;
			}
			else if (Math.Abs(x - Left) <= tolerance && IsWithinVerticalRange(y, tolerance))
			{
				EditingType = EditingType.Left;
			}
			else if (Math.Abs(x - Right) <= tolerance && IsWithinVerticalRange(y, tolerance))
			{
				EditingType = EditingType.Right;
			}
			else if (Math.Abs(y - Top) <= tolerance && IsWithinHorizontalRange(x, tolerance))
			{
				EditingType = EditingType.Top;
			}
			else if (Math.Abs(y - Bottom) <= tolerance && IsWithinHorizontalRange(x, tolerance))
			{
				EditingType = EditingType.Bottom;
			}
			else if (Contain(x, y))
			{
				EditingType = EditingType.Move;
			}
			else
			{
				EditingType = EditingType.None;
			}
		}

		public LineOverType CheckHandleContainsPosition(float x, float y, float zoomScale, float handleSize) => GetHandleContainsPoint(x, y, zoomScale, handleSize);

		public void InitializeHandleRects(float handleSize)
		{
			LineWidth = handleSize;
		}

		public LineOverType GetHandleContainsPoint(float x, float y, float zoomScale, float handleSize)
		{
			if (IsEmpty()) return LineOverType.None;

			float tolerance = GetHitTolerance(zoomScale, handleSize);
			if (Distance(x, y, Left, Top) <= tolerance) return LineOverType.SizeNWSE;
			if (Distance(x, y, Right, Bottom) <= tolerance) return LineOverType.SizeNWSE;
			if (Distance(x, y, Right, Top) <= tolerance) return LineOverType.SizeNESE;
			if (Distance(x, y, Left, Bottom) <= tolerance) return LineOverType.SizeNESE;
			if ((Math.Abs(x - Left) <= tolerance || Math.Abs(x - Right) <= tolerance) && IsWithinVerticalRange(y, tolerance)) return LineOverType.VSplit;
			if ((Math.Abs(y - Top) <= tolerance || Math.Abs(y - Bottom) <= tolerance) && IsWithinHorizontalRange(x, tolerance)) return LineOverType.HSplit;
			return Contain(x, y) ? LineOverType.Move2D : LineOverType.None;
		}

		public void CreateExtendedRectangleFromSize(float offset = 20.0f)
		{
			ExtendedRectangle = new CanvasRect<T>(Left - offset, Top + offset, Right + offset, Bottom - offset)
			{
				UniqueId = UniqueId,
				GroupType = GroupType,
				LineWidth = LineWidth
			};
		}

		public override object Clone() => ToCanvasRect();

		private void RefreshDots()
		{
			ShapePoints = new List<DotInfo>
			{
				new DotInfo(Left, Top),
				new DotInfo(Right, Top),
				new DotInfo(Right, Bottom),
				new DotInfo(Left, Bottom)
			};

			if (ExtendedRectangle != null && !ReferenceEquals(ExtendedRectangle, this) && !IsEmpty())
			{
				ExtendedRectangle.UpdateRectangle(Left - 20, Top + 20, Right + 20, Bottom - 20);
			}
		}

		private static float Clamp(float value, float min, float max) => Math.Max(min, Math.Min(max, value));
		private static float Distance(float x1, float y1, float x2, float y2) => (float)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
		private bool IsWithinHorizontalRange(float x, float tolerance) => x >= Left - tolerance && x <= Right + tolerance;
		private bool IsWithinVerticalRange(float y, float tolerance) => y >= Bottom - tolerance && y <= Top + tolerance;

		private float GetHitTolerance(float zoomScale, float handleSize)
		{
			float effectiveHandleSize = Math.Max(handleSize, LineWidth);
			float scaleAdjustedSize = effectiveHandleSize / Math.Max(zoomScale, 0.0001f);
			return Math.Max(8.0f, scaleAdjustedSize);
		}

		private static EditingType GetEditingTypeFromLineOver(LineOverType type)
		{
			return type switch
			{
				LineOverType.HSplit => EditingType.Top,
				LineOverType.VSplit => EditingType.Left,
				LineOverType.SizeNWSE => EditingType.LeftTop,
				LineOverType.SizeNESE => EditingType.RightTop,
				LineOverType.Move2D => EditingType.Move,
				_ => EditingType.None
			};
		}
	}
}

namespace OpenVisionLab.ImageCanvas.CanvasShapes
{
	using OpenVisionLab.ImageCanvas.Canvas;
	using OpenVisionLab.ImageCanvas.CanvasShapes;

	public enum LineType
	{
		Solid,
		Dash
	}

	public class DotInfo
	{
		public DotInfo()
		{
		}

		public DotInfo(float x, float y)
		{
			X = x;
			Y = y;
		}

		public float X { get; set; }
		public float Y { get; set; }
		public PointF ToPointF() => new PointF(X, Y);
	}

	public class LineInfo : CanvasShape
	{
		public LineInfo(DotInfo startDot, DotInfo endDot, LineType lineType, float width)
		{
			StartDot = startDot;
			EndDot = endDot;
			LineType = lineType;
			Width = width;
			ShapePoints = new List<DotInfo> { startDot, endDot };
		}

		public DotInfo StartDot { get; set; }
		public DotInfo EndDot { get; set; }
		public LineType LineType { get; set; }
		public float Width { get; set; }
		public float[] LineColor { get; set; } = { 1, 1, 1, 1 };
	}

	public class RectInfo : CanvasShape
	{
		public RectInfo(DotInfo leftTop, DotInfo leftBottom, DotInfo rightTop, DotInfo rightBottom, LineType lineType, float width)
		{
			LeftTop = leftTop;
			LeftBottom = leftBottom;
			RightTop = rightTop;
			RightBottom = rightBottom;
			LineType = lineType;
			Width = width;
			ShapePoints = new List<DotInfo> { leftTop, rightTop, rightBottom, leftBottom };
		}

		public DotInfo LeftTop { get; set; }
		public DotInfo LeftBottom { get; set; }
		public DotInfo RightTop { get; set; }
		public DotInfo RightBottom { get; set; }
		public LineType LineType { get; set; }
		public float Width { get; set; }
		public bool IsFill { get; set; }
		public float[] LineColor { get; set; } = { 1, 1, 1, 1 };
	}

	public class CircleInfo : CanvasShape
	{
		public CircleInfo(DotInfo centerDot, LineType lineType, float radius, float width)
		{
			CenterDot = centerDot;
			LineType = lineType;
			Radius = radius;
			Width = width;
			ShapePoints = BuildCircleDots(centerDot, radius);
		}

		public DotInfo CenterDot { get; set; }
		public DotInfo StartDot => new DotInfo(CenterDot.X - Radius, CenterDot.Y - Radius);
		public DotInfo EndDot => new DotInfo(CenterDot.X + Radius, CenterDot.Y + Radius);
		public LineType LineType { get; set; }
		public float Radius { get; set; }
		public float Width { get; set; }
		public bool IsFill { get; set; }
		public float[] LineColor { get; set; } = { 1, 1, 1, 1 };

		private static List<DotInfo> BuildCircleDots(DotInfo center, float radius)
		{
			return Enumerable.Range(0, 32)
				.Select(i =>
				{
					double angle = Math.PI * 2 * i / 32.0;
					return new DotInfo(center.X + (float)Math.Cos(angle) * radius, center.Y + (float)Math.Sin(angle) * radius);
				})
				.ToList();
		}
	}

	public class PensInfo : CanvasShape
	{
		public PensInfo(List<DotInfo> dots, LineType lineType, float width, float[] lineColor = null)
		{
			Dots = dots ?? new List<DotInfo>();
			LineType = lineType;
			Width = width;
			LineColor = lineColor ?? new[] { 1f, 1f, 1f, 1f };
			ShapePoints = Dots;
		}

		public List<DotInfo> Dots { get; set; }
		public LineType LineType { get; set; }
		public float Width { get; set; }
		public float[] LineColor { get; set; }
		public DotInfo ImageTitleOffset { get; set; } = new DotInfo();
	}

	public class TextInfo : CanvasShape
	{
		public string Text { get; set; }
		public DotInfo TextPositionDot { get; set; } = new DotInfo();
		public string FaceName { get; set; } = "Arial";
		public float BaseFontSize { get; set; } = 12;
		public float XSpan { get; set; }
		public float YSpan { get; set; }
		public SizeF OffsetSize { get; set; }
		public List<OpenGlFontBitmapEntry> FontBitmapEntries { get; set; } = new List<OpenGlFontBitmapEntry>();
		public float[] LineColor { get; set; } = { 1, 1, 1, 1 };
	}
}
