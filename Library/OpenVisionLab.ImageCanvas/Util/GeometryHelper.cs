using OpenVisionLab.ImageCanvas.CanvasShapes;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenVisionLab.ImageCanvas
{
	public class GeometryHelper
	{
		public static Rectangle CreateRectangle(PointF center, int halfSize)
		{
			int x = (int)center.X - halfSize;
			int y = (int)center.Y - halfSize;
			int width = halfSize * 2;
			int height = halfSize * 2;

			return new Rectangle(x, y, width, height);
		}

		public static RectangleF CreateRectangleFromCenter(PointF centerPoint, float width, float height)
		{
			// 중심점을 기준으로 사각형의 왼쪽 상단 모서리 위치 계산
			float left = (int)(centerPoint.X - width / 2);
			float top = (int)(centerPoint.Y - height / 2);

			// RectangleF 객체 생성
			RectangleF rect = new RectangleF(left, top, width, height);

			return rect;
		}

		public static System.Drawing.RectangleF CreateRectangleFromPoints(System.Drawing.PointF startPoint, System.Drawing.PointF endPoint)
		{
			if (startPoint.IsEmpty) { return new RectangleF(); }
			if (endPoint.IsEmpty) { return new RectangleF(); }
			if (Equals(startPoint, endPoint)) { return new RectangleF(); }
			// 좌표 계산
			float x = Math.Min(startPoint.X, endPoint.X);
			float y = Math.Min(startPoint.Y, endPoint.Y);

			// 너비와 높이 계산
			float width = Math.Abs(endPoint.X - startPoint.X);
			float height = Math.Abs(endPoint.Y - startPoint.Y);

			// RectangleF 객체 생성
			return new System.Drawing.RectangleF(x, y, width, height);
		}

		public static List<PointF> CreatePointListFromStartEnd(PointF start, PointF end)
		{
			// 두 점 사이의 모든 포인트 계산 로직			
			List<PointF> points = new List<PointF>();
			float distance = (float)Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
			int pointCount = (int)distance; // 간격이 1픽셀이라고 가정

			for (int i = 0; i <= pointCount; i++)
			{
				float t = (float)i / pointCount;
				float x = start.X + t * (end.X - start.X);
				float y = start.Y + t * (end.Y - start.Y);
				points.Add(new PointF(x, y));
			}

			return points;
		}

		public static List<PointF> CreateCirclePointsFromStartEnd(PointF start, PointF end, int segments = 300)
		{
			List<PointF> points = new List<PointF>();

			// 원의 중심을 계산합니다.
			PointF center = new PointF((start.X + end.X) / 2, (start.Y + end.Y) / 2);

			// 반지름은 두 점 사이의 거리의 절반으로 계산합니다. (여기서는 직사각형이 아닐 경우에 대비해 가로와 세로 중 짧은 쪽을 반지름으로 합니다)
			float radius = Math.Min(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)) / 2;

			// 원을 그리기 위한 각도 단계 계산
			float twoPi = 2.0f * (float)Math.PI;
			for (int i = 0; i < segments; i++)
			{
				float theta = twoPi * i / segments;
				float x = center.X + radius * (float)Math.Cos(theta);
				float y = center.Y + radius * (float)Math.Sin(theta);
				points.Add(new PointF(x, y));
			}

			return points;
		}

		public static void AddIntermediatePoints(ref List<System.Drawing.PointF> points, System.Drawing.PointF newPoint, int interval)
		{
			if (points.Count > 0)
			{
				System.Drawing.PointF lastPoint = points[points.Count - 1];
				double distance = IntermediateCalculateDistance(lastPoint, newPoint);
				if (distance > interval) // 누락된 포인트가 있을 경우
				{
					int numIntermediatePoints = (int)Math.Ceiling(distance / interval); // 누락된 포인트의 수 계산
					double dx = (newPoint.X - lastPoint.X) / numIntermediatePoints;
					double dy = (newPoint.Y - lastPoint.Y) / numIntermediatePoints;
					for (int i = 1; i < numIntermediatePoints; i++)
					{
						System.Drawing.PointF intermediatePoint = new System.Drawing.PointF((int)(lastPoint.X + dx * i), (int)(lastPoint.Y + dy * i));
						if (!points.Contains(intermediatePoint))
							points.Add(intermediatePoint); // 누락된 포인트 추가
					}
				}
			}
			if (!points.Contains(newPoint))
				points.Add(newPoint);  // 새로운 포인트 추가
		}

		// 두 점 사이의 거리 계산 함수
		public static double IntermediateCalculateDistance(System.Drawing.PointF point1, System.Drawing.PointF point2)
		{
			double dx = point2.X - point1.X;
			double dy = point2.Y - point1.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		public static List<DotInfo> ConvertPointsToDotInfo(List<System.Drawing.PointF> listPoints)
		{
			List<DotInfo> listDot = new List<DotInfo>();
			foreach (System.Drawing.PointF point in listPoints)
			{
				listDot.Add(new DotInfo(point.X, point.Y));
			}
			return listDot;
		}
	}
}
