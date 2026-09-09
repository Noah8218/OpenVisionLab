using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using OpenVisionLab.ImageCanvas.Rendering;
using OpenVisionLab.ImageCanvas.ViewModels;
using SharpGL;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab.ImageCanvas
{
	// Owns ImageCanvas mouse policy and event lifetime; keyboard input remains in separate owners.
	internal sealed class RoiImageCanvasMouseInputController : IDisposable
	{
		private readonly ImageCanvasControl _imageViewer;
		private readonly Func<CanvasRect<float>> _getSelectedRect;
		private readonly Action<CanvasRect<float>> _setSelectedRect;
		private readonly Func<CanvasRect<float>> _getDrawingRect;
		private readonly Action<CanvasRect<float>> _setDrawingRect;
		private readonly Func<Measurement> _getMeasurement;
		private readonly Action<Measurement> _setMeasurement;
		private readonly Func<Point> _getMouseDownCanvasPos;
		private readonly Action<Point> _setMouseDownCanvasPos;
		private readonly Func<bool> _getIsPanning;
		private readonly Action<bool> _setIsPanning;
		private readonly Func<PointF> _getPanAnchorPoint;
		private readonly Action<PointF> _setPanAnchorPoint;
		private readonly Func<Size> _getImageSize;
		private readonly Func<bool> _getIsAddRoiArrayMode;
		private readonly Action<bool> _setIsAddRoiArrayMode;
		private readonly Func<bool> _getIsTeachingMode;
		private readonly Func<bool> _getReplaceExistingRoiOnDraw;
		private readonly Func<bool> _getUseGroupMoveMode;
		private readonly AddRoiArrayViewModel _addRoiArrayViewModel;
		private readonly Action _beginRoiInteractionSnapshot;
		private readonly Action<string> _completeRoiInteractionSnapshot;
		private readonly Action<CanvasRect<float>> _roiMouseUp;
		private readonly OverlayEditingCompletedCallback _roiEditingCompleted;
		private readonly OverlayAddedCallback _roiAdded;
		private readonly Action _replaceWindowRoisForSingleDraw;
		private readonly Action _updatePixelProperty;
		private readonly Action _handleRightClick;
		private readonly Action _startDrawingTimer;
		private bool _disposed;

		internal RoiImageCanvasMouseInputController(
			ImageCanvasControl imageViewer,
			Func<CanvasRect<float>> getSelectedRect,
			Action<CanvasRect<float>> setSelectedRect,
			Func<CanvasRect<float>> getDrawingRect,
			Action<CanvasRect<float>> setDrawingRect,
			Func<Measurement> getMeasurement,
			Action<Measurement> setMeasurement,
			Func<Point> getMouseDownCanvasPos,
			Action<Point> setMouseDownCanvasPos,
			Func<bool> getIsPanning,
			Action<bool> setIsPanning,
			Func<PointF> getPanAnchorPoint,
			Action<PointF> setPanAnchorPoint,
			Func<Size> getImageSize,
			Func<bool> getIsAddRoiArrayMode,
			Action<bool> setIsAddRoiArrayMode,
			Func<bool> getIsTeachingMode,
			Func<bool> getReplaceExistingRoiOnDraw,
			Func<bool> getUseGroupMoveMode,
			AddRoiArrayViewModel addRoiArrayViewModel,
			Action beginRoiInteractionSnapshot,
			Action<string> completeRoiInteractionSnapshot,
			Action<CanvasRect<float>> roiMouseUp,
			OverlayEditingCompletedCallback roiEditingCompleted,
			OverlayAddedCallback roiAdded,
			Action replaceWindowRoisForSingleDraw,
			Action updatePixelProperty,
			Action handleRightClick,
			Action startDrawingTimer)
		{
			_imageViewer = imageViewer ?? throw new ArgumentNullException(nameof(imageViewer));
			_getSelectedRect = getSelectedRect ?? throw new ArgumentNullException(nameof(getSelectedRect));
			_setSelectedRect = setSelectedRect ?? throw new ArgumentNullException(nameof(setSelectedRect));
			_getDrawingRect = getDrawingRect ?? throw new ArgumentNullException(nameof(getDrawingRect));
			_setDrawingRect = setDrawingRect ?? throw new ArgumentNullException(nameof(setDrawingRect));
			_getMeasurement = getMeasurement ?? throw new ArgumentNullException(nameof(getMeasurement));
			_setMeasurement = setMeasurement ?? throw new ArgumentNullException(nameof(setMeasurement));
			_getMouseDownCanvasPos = getMouseDownCanvasPos ?? throw new ArgumentNullException(nameof(getMouseDownCanvasPos));
			_setMouseDownCanvasPos = setMouseDownCanvasPos ?? throw new ArgumentNullException(nameof(setMouseDownCanvasPos));
			_getIsPanning = getIsPanning ?? throw new ArgumentNullException(nameof(getIsPanning));
			_setIsPanning = setIsPanning ?? throw new ArgumentNullException(nameof(setIsPanning));
			_getPanAnchorPoint = getPanAnchorPoint ?? throw new ArgumentNullException(nameof(getPanAnchorPoint));
			_setPanAnchorPoint = setPanAnchorPoint ?? throw new ArgumentNullException(nameof(setPanAnchorPoint));
			_getImageSize = getImageSize ?? throw new ArgumentNullException(nameof(getImageSize));
			_getIsAddRoiArrayMode = getIsAddRoiArrayMode ?? throw new ArgumentNullException(nameof(getIsAddRoiArrayMode));
			_setIsAddRoiArrayMode = setIsAddRoiArrayMode ?? throw new ArgumentNullException(nameof(setIsAddRoiArrayMode));
			_getIsTeachingMode = getIsTeachingMode ?? throw new ArgumentNullException(nameof(getIsTeachingMode));
			_getReplaceExistingRoiOnDraw = getReplaceExistingRoiOnDraw ?? throw new ArgumentNullException(nameof(getReplaceExistingRoiOnDraw));
			_getUseGroupMoveMode = getUseGroupMoveMode ?? throw new ArgumentNullException(nameof(getUseGroupMoveMode));
			_addRoiArrayViewModel = addRoiArrayViewModel ?? throw new ArgumentNullException(nameof(addRoiArrayViewModel));
			_beginRoiInteractionSnapshot = beginRoiInteractionSnapshot ?? throw new ArgumentNullException(nameof(beginRoiInteractionSnapshot));
			_completeRoiInteractionSnapshot = completeRoiInteractionSnapshot ?? throw new ArgumentNullException(nameof(completeRoiInteractionSnapshot));
			_roiMouseUp = roiMouseUp ?? throw new ArgumentNullException(nameof(roiMouseUp));
			_roiEditingCompleted = roiEditingCompleted ?? throw new ArgumentNullException(nameof(roiEditingCompleted));
			_roiAdded = roiAdded ?? throw new ArgumentNullException(nameof(roiAdded));
			_replaceWindowRoisForSingleDraw = replaceWindowRoisForSingleDraw ?? throw new ArgumentNullException(nameof(replaceWindowRoisForSingleDraw));
			_updatePixelProperty = updatePixelProperty ?? throw new ArgumentNullException(nameof(updatePixelProperty));
			_handleRightClick = handleRightClick ?? throw new ArgumentNullException(nameof(handleRightClick));
			_startDrawingTimer = startDrawingTimer ?? throw new ArgumentNullException(nameof(startDrawingTimer));

			_imageViewer.MouseDoubleClicked += OnMouseDoubleClicked;
			_imageViewer.MouseClicked += OnMouseClicked;
			_imageViewer.MouseDown += OnMouseDown;
			_imageViewer.MouseMove += OnMouseMove;
			_imageViewer.MouseUp += OnMouseUp;
			_imageViewer.MouseLeave += OnMouseLeave;
			_imageViewer.MouseWheel += OnMouseWheel;
		}

		private void OnMouseDown(object sender, CanvasMouseEventArgs e)
		{
			OpenGLControl openGLControl = (OpenGLControl)sender;

			switch (e.Button)
			{
				case MouseButtons.Left:
					_beginRoiInteractionSnapshot();
					_setMouseDownCanvasPos(new Point(e.X, e.Y));
					CanvasRect<float> selectedRect = _getSelectedRect();
					RoiInteractionMouseDown.InitializeMouseDownState(_imageViewer, ref selectedRect, openGLControl, e);
					_setSelectedRect(selectedRect);
					switch (_imageViewer.GetViewMode())
					{
						case CanvasInteractionMode.Drawing:
							_setDrawingRect(new CanvasRect<float> { IsEditing = true });
							break;
						case CanvasInteractionMode.Edit:
						case CanvasInteractionMode.Move:
							_setDrawingRect(new CanvasRect<float>());
							if (selectedRect != null) { selectedRect.IsEditing = true; }

							_setSelectedRect(selectedRect);
							break;
						case CanvasInteractionMode.Drag:
						case CanvasInteractionMode.Measure:
							_setDrawingRect(new CanvasRect<float>());
							if (selectedRect != null) { selectedRect.IsEditing = false; }

							_setSelectedRect(selectedRect);
							break;
					}
					break;
				case MouseButtons.Right:
					ClearSelection();
					_handleRightClick();
					break;
				case MouseButtons.Middle:
					_setIsPanning(true);
					_setPanAnchorPoint(_imageViewer.GetCurrentCanvasPositionF(e.X, e.Y));
					openGLControl.Cursor = Cursors.Hand;
					break;
			}

			_startDrawingTimer();
		}

		private void OnMouseMove(object sender, CanvasMouseEventArgs e)
		{
			OpenGLControl openGLControl = (OpenGLControl)sender;
			if (_getIsPanning())
			{
				_imageViewer.PanToKeepPointAtMouse(_getPanAnchorPoint(), new Point(e.X, e.Y));
				_updatePixelProperty();
				openGLControl.Cursor = Cursors.Hand;
				return;
			}

			PointF currentImagePos = _imageViewer.GetCurrentCanvasPosition(e.X, e.Y);
			openGLControl.Cursor = RoiInteractionCursor.GetCursorFromType(GetCursorInteractionRect(currentImagePos), currentImagePos, _imageViewer.ZoomScale, _imageViewer.HandleSize);
			_imageViewer.PostMousePos = currentImagePos;

			switch (_imageViewer.GetViewMode())
			{
				case CanvasInteractionMode.Edit:
					RoiInteractionMouseMove.ResizeRoiRect(_imageViewer, _getSelectedRect(), currentImagePos, _getImageSize(), _roiEditingCompleted);
					break;
				case CanvasInteractionMode.Move:
					RoiInteractionMouseMove.MoveOverlay(_imageViewer, _getSelectedRect(), currentImagePos, _getImageSize(), true, _roiEditingCompleted, _getUseGroupMoveMode());
					break;
				case CanvasInteractionMode.Drawing:
					RoiInteractionMouseMove.UpdateRectangleToOverlay(_imageViewer, _getDrawingRect());
					break;
				case CanvasInteractionMode.Measure:
					Measurement measurement = _getMeasurement();
					RoiInteractionMouseMove.UpdateMeasurement(_imageViewer, ref measurement);
					_setMeasurement(measurement);
					break;
			}

			_updatePixelProperty();
		}

		private void OnMouseUp(object sender, CanvasMouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
			{
				_setIsPanning(false);
				return;
			}

			_imageViewer.PostMousePos = _imageViewer.GetCurrentCanvasPosition(e.X, e.Y);
			CanvasRect<float> mouseUpRect = GetActiveInteractionRect();
			CanvasRect<float> selectedRect = _getSelectedRect();
			if (selectedRect != null) { selectedRect.IsEditing = false; }
			_setSelectedRect(selectedRect);
			CanvasRect<float> drawingRect = _getDrawingRect();
			if (drawingRect != null) { drawingRect.IsEditing = false; }
			_setDrawingRect(drawingRect);

			bool hasValidLeftDrag = e.Button == MouseButtons.Left
				&& HasValidMouseDrag(_getMouseDownCanvasPos(), new Point(e.X, e.Y))
				&& HasValidDrawingBounds(_imageViewer.PreMousePos, _imageViewer.PostMousePos);

			if (e.Button == MouseButtons.Left && !hasValidLeftDrag && _imageViewer.GetViewMode() == CanvasInteractionMode.Drawing)
			{
				_setDrawingRect(new CanvasRect<float>());
				mouseUpRect = selectedRect;
			}

			if (hasValidLeftDrag)
			{
				if (_getIsAddRoiArrayMode())
				{
					RoiInteractionMouseUp.OpenAddRoiArrayView(_imageViewer, _addRoiArrayViewModel, _roiAdded);
					// 입력이 완료되면 배열 추가 모드를 종료한다.
					_setIsAddRoiArrayMode(false);
				}
				else
				{
					if (_imageViewer.GetViewMode() == CanvasInteractionMode.Drawing && _getReplaceExistingRoiOnDraw())
					{
						_replaceWindowRoisForSingleDraw();
					}

					if (_imageViewer.GetViewMode() == CanvasInteractionMode.Drawing)
					{
						drawingRect = _getDrawingRect();
						bool added = RoiInteractionMouseUp.AddRectangleToOverlay(_imageViewer, _imageViewer.PreMousePos, _imageViewer.PostMousePos, ref drawingRect, _roiAdded);
						_setDrawingRect(drawingRect);
						if (added)
						{
							_setSelectedRect(drawingRect);
							mouseUpRect = drawingRect;
						}
						_setDrawingRect(new CanvasRect<float>());
					}
				}
			}

			_roiMouseUp(mouseUpRect);
			ResetViewMode();
			_completeRoiInteractionSnapshot("ROI Edit");
		}

		private void OnMouseWheel(object sender, CanvasMouseEventArgs e)
		{
			_imageViewer.AdjustOffsetForZoom(e.Location, _imageViewer.UpdateZoom(e.Delta));
			_imageViewer.Reshape();
		}

		private void OnMouseLeave(object sender, EventArgs e)
		{
			_setIsPanning(false);
		}

		private void OnMouseClicked(object sender, EventArgs e)
		{
		}

		private void OnMouseDoubleClicked(object sender, EventArgs e)
		{
		}

		private CanvasRect<float> GetActiveInteractionRect()
		{
			return _imageViewer.GetViewMode() == CanvasInteractionMode.Drawing ? _getDrawingRect() : _getSelectedRect();
		}

		private CanvasRect<float> GetCursorInteractionRect(PointF currentImagePos)
		{
			switch (_imageViewer.GetViewMode())
			{
				case CanvasInteractionMode.Edit:
				case CanvasInteractionMode.Move:
					return _getSelectedRect();
				case CanvasInteractionMode.Drawing:
					CanvasRect<float> drawingRect = _getDrawingRect();
					if (drawingRect != null && drawingRect.IsEditing)
					{
						return drawingRect;
					}
					break;
			}

			var (hoverRect, _) = RoiInteractionMouseDown.FindOverlayAtPosition(_imageViewer, currentImagePos);
			if (hoverRect != null)
			{
				return hoverRect;
			}

			CanvasRect<float> drawingRectForOverlay = _getDrawingRect();
			if (_imageViewer.GetViewMode() == CanvasInteractionMode.Drawing && drawingRectForOverlay != null && !drawingRectForOverlay.IsEmpty())
			{
				return drawingRectForOverlay;
			}

			CanvasRect<float> selectedRect = _getSelectedRect();
			if (selectedRect != null && !selectedRect.IsEmpty())
			{
				return selectedRect;
			}

			return null;
		}

		private void ClearSelection()
		{
			CanvasRect<float> selectedRect = _getSelectedRect();
			if (selectedRect != null)
			{
				selectedRect.IsEditing = false;
				selectedRect.IsChanged = true;
			}

			CanvasRect<float> drawingRect = _getDrawingRect();
			if (drawingRect != null)
			{
				drawingRect.IsEditing = false;
				drawingRect.IsChanged = true;
			}

			_setSelectedRect(new CanvasRect<float>());
			_setDrawingRect(new CanvasRect<float>());
		}

		private void ResetViewMode()
		{
			if (_imageViewer.GetViewMode() == CanvasInteractionMode.Drag) { _imageViewer.SetViewMode(CanvasInteractionMode.None); }
			if (_imageViewer.GetViewMode() == CanvasInteractionMode.Move) { _imageViewer.SetViewMode(CanvasInteractionMode.None); }
			if (_imageViewer.GetViewMode() == CanvasInteractionMode.Edit) { _imageViewer.SetViewMode(CanvasInteractionMode.None); }
			if (_getIsTeachingMode() && _imageViewer.GetViewMode() == CanvasInteractionMode.None) { _imageViewer.SetViewMode(CanvasInteractionMode.Drawing); }
		}

		private static bool HasValidDrawingBounds(PointF preMousePos, PointF postMousePos)
		{
			return Math.Abs(postMousePos.X - preMousePos.X) > 0 && Math.Abs(postMousePos.Y - preMousePos.Y) > 0;
		}

		private static bool HasValidMouseDrag(Point startPoint, Point endPoint)
		{
			const int minimumDrawingPixels = 2;
			return Math.Abs(endPoint.X - startPoint.X) >= minimumDrawingPixels && Math.Abs(endPoint.Y - startPoint.Y) >= minimumDrawingPixels;
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;
			_imageViewer.MouseDoubleClicked -= OnMouseDoubleClicked;
			_imageViewer.MouseClicked -= OnMouseClicked;
			_imageViewer.MouseDown -= OnMouseDown;
			_imageViewer.MouseMove -= OnMouseMove;
			_imageViewer.MouseUp -= OnMouseUp;
			_imageViewer.MouseLeave -= OnMouseLeave;
			_imageViewer.MouseWheel -= OnMouseWheel;
		}
	}
}
