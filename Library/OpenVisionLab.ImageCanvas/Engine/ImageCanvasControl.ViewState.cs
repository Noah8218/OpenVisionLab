using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab.ImageCanvas.Rendering
{
	public partial class ImageCanvasControl
	{
		public void FitToRect(RectangleF rect)
		{
			SetZoomValue(rect);
			UpdateView(rect);
		}

		public void ZoomToFit()
		{
			if (_fitRect.IsEmpty) return;
			SetZoomValue(_fitRect);
			UpdateView(_fitRect);
		}

		public CanvasViewState CaptureViewState()
		{
			return new CanvasViewState(_zoom, _offsetSize);
		}

		public void ApplyViewState(CanvasViewState viewState)
		{
			if (viewState == null) { return; }

			_zoom = viewState.Zoom;
			_offsetSize = viewState.OffsetSize;
			Reshape();
			RefreshGL();
		}

		public void ZoomAt(Point mousePos, int delta)
		{
			float oldZoom = UpdateZoom(delta);
			if (oldZoom == 0) { return; }

			AdjustOffsetForZoom(mousePos, oldZoom);
			Reshape();
			RefreshGL();
		}

		public void UpdateView(RectangleF rect)
		{
			Reshape();
			Move(new PointF(rect.Left + rect.Width / 2, rect.Y + rect.Height / 2));
		}

		private new void Move(PointF pt)
		{
			_aspectRatio = ((float)openGLControl.Width) / openGLControl.Height;
			_xSpan = _zoom;
			_ySpan = _zoom;

			if (_aspectRatio > 1)
			{
				_xSpan *= _aspectRatio;
			}
			else
			{
				_ySpan /= _aspectRatio;
			}

			_offsetSize.Width = _xSpan / 2 - pt.X;
			_offsetSize.Height = _ySpan / 2 - pt.Y;
			openGLControl.Refresh();
		}

		private void SetZoomValue(RectangleF rect)
		{
			float scaleWidth = (float)openGLControl.Width / rect.Width;
			float scaleHeight = (float)openGLControl.Height / rect.Height;
			float zoomFactor = 1.1f;

			if (scaleHeight < scaleWidth)
			{
				_zoom = rect.Height * zoomFactor;
			}
			else
			{
				_zoom = rect.Height * (rect.Width / rect.Height) * zoomFactor;
			}
		}

		private void ResetMousePositions()
		{
			PreMousePos = new Point();
			PostMousePos = new Point();
		}

		private void DragViewMovement(MouseEventArgs e)
		{
			Point currentRobotyPos = GetCurrentRobotPos(e.X, e.Y);
			float dx = currentRobotyPos.X - _preMousePos.X;
			float dy = currentRobotyPos.Y - _preMousePos.Y;
			_offsetSize.Width += dx;
			_offsetSize.Height += dy;
		}

		public float UpdateZoom(int delta)
		{
			float oldZoom = _zoom;
			if (float.IsNaN(oldZoom))
			{
				return 0;
			}

			_zoom *= (delta < 0) ? 1.20F : 0.80F;
			if (_zoom <= 0.0f)
			{
				_zoom = MIN_ZOOM_SCALE;
			}

			_zoom = (float)Math.Round((decimal)_zoom, 3);
			OpenGlDrawing.ZoomFactor = ZoomScale;

			return oldZoom;
		}

		public void AdjustOffsetForZoom(Point mousePos, float oldZoom)
		{
			float oldImageX = mousePos.X * (oldZoom / GetControlMinSize());
			float oldImageY = (mousePos.Y - openGLControl.Size.Height) * (oldZoom / GetControlMinSize());
			float newImageX = mousePos.X * (_zoom / GetControlMinSize());
			float newImageY = (mousePos.Y - openGLControl.Size.Height) * (_zoom / GetControlMinSize());

			_offsetSize.Width += newImageX - oldImageX;
			_offsetSize.Height += oldImageY - newImageY;
		}

		public PointF GetRoundPointF(PointF pt, int digit = 1)
		{
			PointF result = pt;
			result.X = (float)Math.Round(result.X, digit);
			result.Y = (float)Math.Round(result.Y, digit);

			return result;
		}

		public PointF GetCurrentRobotPosf(int mouseLocationX, int mouseLocationY)
		{
			PointF robotPos = new PointF(
				mouseLocationX * _zoom / GetControlMinSize() - _offsetSize.Width,
				(openGLControl.Size.Height - mouseLocationY) * _zoom / GetControlMinSize() - _offsetSize.Height);
			return new PointF(robotPos.X, robotPos.Y);
		}

		public PointF GetScreenPosFromPixelCoordf(int pixelX, int pixelY)
		{
			float screenX = (pixelX + _offsetSize.Width) * GetControlMinSize() / _zoom;
			float screenY = openGLControl.Size.Height - ((pixelY + _offsetSize.Height) * GetControlMinSize() / _zoom);

			return new PointF(screenX, screenY);
		}

		public Point GetCurrentRobotPos(int mouseLocationX, int mouseLocationY)
		{
			Point robotPos = new Point(
				(int)(mouseLocationX * _zoom / GetControlMinSize() - _offsetSize.Width),
				(int)((openGLControl.Size.Height - mouseLocationY) * _zoom / GetControlMinSize() - _offsetSize.Height));
			return new Point(robotPos.X, robotPos.Y);
		}

		public PointF ConvertOpenGlToImagePoint(PointF openGlPoint)
		{
			RectangleF imageBounds = CalculateBoundingRectangle(_textureAreas);
			if (imageBounds.Width <= 0 || imageBounds.Height <= 0)
			{
				return openGlPoint;
			}

			return new PointF(openGlPoint.X - imageBounds.Left, imageBounds.Bottom - openGlPoint.Y);
		}

		public Point GetScreenPosFromPixelCoord(int pixelX, int pixelY)
		{
			int screenX = (int)((pixelX + _offsetSize.Width) * GetControlMinSize() / _zoom);
			int screenY = (int)(openGLControl.Size.Height - ((pixelY + _offsetSize.Height) * GetControlMinSize() / _zoom));

			return new Point(screenX, screenY);
		}
	}
}
