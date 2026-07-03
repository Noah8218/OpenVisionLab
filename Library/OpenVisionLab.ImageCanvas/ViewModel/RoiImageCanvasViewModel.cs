using OpenVisionLab.ImageCanvas.SharedViewModels;
using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.Infrastructure;
using OpenVisionLab.ImageCanvas.Commands;
using OpenVisionLab.ImageCanvas.Events;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using Microsoft.Win32;
using OpenCvSharp;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Model = OpenVisionLab.ImageCanvas.Model;

namespace OpenVisionLab.ImageCanvas.ViewModels
{
	public partial class RoiImageCanvasViewModel : ObservableObject, IDisposable
	{
		#region Event
		public event EventHandler<object> LoadImageRequested = delegate { };
		public event EventHandler<CanvasRect<float>> RemoveRoiRequested = delegate { };

		public event EventHandler<Model.RoiChangedEventArgs> RoiAdded = delegate { };
		public event EventHandler<Model.RoiChangedEventArgs> RoiMouseUp = delegate { };
		public event EventHandler<Model.RoiChangedEventArgs> RoiGrouped = delegate { };
		public event EventHandler<Model.RoiChangedEventArgs> RoiEditingCompleted = delegate { };
		public event EventHandler<Model.RoiSnapshotChangedEventArgs> RoiSnapshotChanged = delegate { };
		public event EventHandler UndoRequested = delegate { };
		public event EventHandler RedoRequested = delegate { };
		public event EventHandler<object> QuickTestRequest = delegate { };

		// 모델?�리???�성???�시�??�니??
		public Action OnWindowsChanged { get; set; } // 콜백 추�?
		#endregion

		#region Fields
		private bool _isTeachingMode = false;
		private bool _isAddRoiArrayMode = false;
		private bool _isShowMeasure = false;
		private bool _useGroupMoveMode = false;
		private bool _isPreviewMode = false;
		private float _heightValue;
		private Measurement _measurement = new Measurement();
		private OpenGlFontRenderOptions _measureFontOption = new OpenGlFontRenderOptions(System.Drawing.Color.Red, "Arial", 20, "");
		protected CanvasRect<float> _selectedRect = new CanvasRect<float>();
		protected CanvasRect<float> _drawingRect = new CanvasRect<float>();
		protected CanvasRect<float> _copyRoiRect;
		private System.Drawing.Point _mouseDownCanvasPos = System.Drawing.Point.Empty;
		private System.Drawing.Size _imageSize = new System.Drawing.Size();
		private OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl _imageViewer = new OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl();
		private Mat _currentImageMat;
		private string _currentImageName = "Image";
		private Func<string, bool> _saveImageOverride;
		private bool _isPanning;
		private System.Drawing.PointF _panAnchorPoint;
		private IReadOnlyList<Model.RoiSnapshotItem> _roiSnapshotBeforeInteraction = new List<Model.RoiSnapshotItem>();
		private bool _roiInteractionSnapshotActive;
		public float[] AfData3D = new float[10];
		private AddRoiArrayViewModel _addRoiArrayVm = new AddRoiArrayViewModel();
		private System.Timers.Timer _refreshTimer;  // ?�?�머 객체
		#endregion

		#region Properties
		public OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl ImageViewer
		{
			get { return _imageViewer; }
		}

		public System.Windows.Controls.ContextMenu ContextMenu { get; set; }

		public int GrayValue
		{
			get
			{
				if (_imageViewer != null) { return _imageViewer.GrayValue; }
				return 0;
			}
		}

		public bool IsShowCrossLine
		{
			get
			{
				if (_imageViewer != null) { return _imageViewer.IsShowCrossLine; }
				return false;
			}
			set
			{
				if (_imageViewer != null) { _imageViewer.IsShowCrossLine = value; }
				_imageViewer.RefreshGL();
				OnPropertyChanged(nameof(IsShowCrossLine));
			}
		}

		public bool IsShowMeasure
		{
			get
			{
				if (_imageViewer != null) { return _isShowMeasure; }
				return false;
			}
			set
			{
				if (_imageViewer != null)
				{
					_isShowMeasure = value;
					_imageViewer.SetViewMode((_isShowMeasure == true) ? CanvasInteractionMode.Measure : CanvasInteractionMode.None);
				}
				if (!value)
				{
					_measurement = new Measurement();
				}
				OnPropertyChanged(nameof(IsShowMeasure));
			}
		}

		public bool IsTeachingMode
		{
			get
			{
				if (_imageViewer != null) { return _isTeachingMode; }
				OnPropertyChanged(nameof(IsTeachingMode));
				return false;
			}
			set
			{
				if (_imageViewer != null)
				{
					_isTeachingMode = value;
					_imageViewer.SetViewMode((_isTeachingMode == true) ? CanvasInteractionMode.Drawing : CanvasInteractionMode.None);
				}

				OnPropertyChanged(nameof(IsTeachingMode));
			}
		}

		public bool IsAddRoiArrayMode
		{
			get
			{
				if (_imageViewer != null) { return _isAddRoiArrayMode; }
				OnPropertyChanged(nameof(IsAddRoiArrayMode));
				return false;
			}
			set
			{
				if (_imageViewer != null)
				{
					_isAddRoiArrayMode = value;
					_imageViewer.SetViewMode((_isAddRoiArrayMode == true) ? CanvasInteractionMode.Drawing : CanvasInteractionMode.None);
				}
				OnPropertyChanged(nameof(IsAddRoiArrayMode));
			}
		}

		public bool IsPreviewMode
		{
			get
			{
				if (_imageViewer != null) { return _isPreviewMode; }
				OnPropertyChanged();
				return false;
			}
			set
			{
				if (_imageViewer != null) { _isPreviewMode = value; }
				_imageViewer.RefreshGL();
				OnPropertyChanged();
			}
		}

		public bool UseGroupMoveMode
		{
			get
			{
				if (_imageViewer != null) { return _useGroupMoveMode; }
				OnPropertyChanged();
				return false;
			}
			set
			{
				if (_imageViewer != null) { _useGroupMoveMode = value; }
				OnPropertyChanged();
			}
		}

		public bool ShowGroupNames { get; set; } = true;
		public bool ShowRoiItemNames { get; set; } = true;
		public bool ReplaceExistingRoiOnDraw { get; set; }

		public bool ShowGroupBounds
		{
			get => _imageViewer.GetLastGroup()?.IsVisible ?? true;
			set
			{
				CanvasOverlayItem group = _imageViewer.GetLastGroup();
				if (group == null) { return; }

				group.IsVisible = value;
				group.Shape.IsChanged = true;
				_imageViewer.RefreshGL();
			}
		}

		public System.Drawing.PointF CanvasPos
		{
			get
			{
				if (_imageViewer != null) { return _imageViewer.PixelPos; }
				return new System.Drawing.PointF();
			}
		}

		public System.Drawing.PointF ImagePos
		{
			get
			{
				if (_imageViewer != null) { return _imageViewer.ImagePixelPos; }
				return new System.Drawing.PointF();
			}
		}

		public float HeightValue
		{
			get
			{
				if (_imageViewer != null) { return _heightValue; }
				return 0;
			}
			set
			{
				if (_imageViewer != null) { _heightValue = value; }
				OnPropertyChanged();
			}
		}

		public System.Drawing.Color PixelColor
		{
			get
			{
				if (_imageViewer != null) { return _imageViewer.PixelColor; }
				return new System.Drawing.Color();
			}
		}

		public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

		#endregion

		#region Command
		public ICommand LoadedCommand { get; set; }
		public ICommand RightClickCommand { get; private set; }
		public ICommand LoadImageCommand { get; set; }
		public ICommand SaveImageCommand { get; set; }
		public ICommand FitImageCommand { get; set; }
		public ICommand ShowCrossLineCommand { get; set; }
		public ICommand MeasureCommand { get; set; }
		public ICommand TeachingCommand { get; set; }
		public ICommand AddingArrayCommand { get; set; }
		public ICommand ShowPreviewCommand { get; set; }
		public ICommand PreviewKeyDownCommand { get; private set; }
		public ICommand KeyUpCommand { get; set; }
		#endregion
		public RoiImageCanvasViewModel(string name)
		{
			InitCommand();
			InitEvent();
			InitMenuItems();
			_imageViewer.SetNameGL(name);
			InitializeDefaultGroup();
		}

		private void InitializeDefaultGroup()
		{
			string groupType = EnumInspWindowType.Module.ToString();
			_imageViewer.AddOverlay("", groupType, new CanvasRect<float>(), Guid.NewGuid().ToString(), EnumInspWindowType.Module, EnumItemType.Group, false, true);
			_imageViewer.SetLastGroupType(groupType);
		}

		private void Loaded()
		{
			_imageViewer.InvertYAxis = true;
		}

		private void InitEvent()
		{
			_imageViewer.Load += OnLoad;
			_imageViewer.Resized += OnResized;
			_imageViewer.MouseDoubleClicked += OnMouseDoubleClicked;
			_imageViewer.Draw += OnDraw;
			_imageViewer.KeyDown += OnKeyDown;
			_imageViewer.KeyUp += OnKeyUp;
			_imageViewer.MouseClicked += OnMouseClicked;
			_imageViewer.MouseDown += OnMouseDown;
			_imageViewer.MouseMove += OnMouseMove;
			_imageViewer.MouseUp += OnMouseUp;
			_imageViewer.MouseLeave += OnMouseLeave;
			_imageViewer.MouseWheel += OnMouseWheel;

			_refreshTimer = new System.Timers.Timer(1);  // 1초마???�벤??발생
			_refreshTimer.Elapsed += _dataTimer_Elapsed;
			_refreshTimer.Start();  // ?�?�머 ?�작
		}
		private void InitMenuItems()
		{
			MenuItems = new ObservableCollection<MenuItemViewModel>();
			MenuItems.Add(new MenuItemViewModel { Header = MenuItemUtil.GetDescription(EnumImageCanvasItems.LoadImage), Command = LoadImageCommand, IconData = MaterialIconData.Image, IsVisible = true });
			MenuItems.Add(new MenuItemViewModel { Header = MenuItemUtil.GetDescription(EnumImageCanvasItems.FitImage), Command = FitImageCommand, IconData = MaterialIconData.CheckCircle, IsVisible = true });
			MenuItems.Add(new MenuItemViewModel { Header = MenuItemUtil.GetDescription(EnumImageCanvasItems.SaveImage), Command = SaveImageCommand, IconData = MaterialIconData.ContentSave, IsVisible = true });
		}
		private void OnDraw(object sender, OpenVisionLab.ImageCanvas.Canvas.CanvasRenderEventArgs e)
		{
			OpenGL gl = e.GL;

			_imageViewer.DrawContent();
			OpenGlDrawing.DrawRoiEditHandles(gl, GetOverlayRect(), _imageViewer.ZoomScale, System.Windows.Media.Brushes.Yellow);
			if (ShowGroupNames)
			{
				OpenGlDrawing.DrawGroupName(gl, _imageViewer.GetCanvasOverlayManager(), _imageViewer.GetOpenGlTextDrawOptions());
			}
			if (ShowRoiItemNames)
			{
				OpenGlDrawing.DrawRoiItemName(gl, _imageViewer.GetCanvasOverlayManager(), _imageViewer.GetOpenGlTextDrawOptions());
			}
			if (IsShowMeasure) { _imageViewer.DrawMeasurement(gl, _measurement, _measureFontOption); }
		}

		private void OnMouseDown(object sender, CanvasMouseEventArgs e)
		{
			OpenGLControl openGLControl = (OpenGLControl)sender;

			switch (e.Button)
			{
				case System.Windows.Forms.MouseButtons.Left:
					BeginRoiInteractionSnapshot();
					_mouseDownCanvasPos = new System.Drawing.Point(e.X, e.Y);
					RoiInteractionMouseDown.InitializeMouseDownState(ImageViewer, ref _selectedRect, openGLControl, e);
					switch (_imageViewer.GetViewMode())
					{
						case CanvasInteractionMode.Drawing:
							_drawingRect = new CanvasRect<float>();
							_drawingRect.IsEditing = true;
							break;
						case CanvasInteractionMode.Edit:
						case CanvasInteractionMode.Move:
							_drawingRect = new CanvasRect<float>();
							if (_selectedRect != null) { _selectedRect.IsEditing = true; }

							break;
						case CanvasInteractionMode.Drag:
						case CanvasInteractionMode.Measure:
							_drawingRect = new CanvasRect<float>();
							if (_selectedRect != null) { _selectedRect.IsEditing = false; }
							break;
					}
					break;
				case System.Windows.Forms.MouseButtons.Right:
					ClearSelection();
					OnMouseRightClick(CanvasContextMenuMode.Default);
					break;
				case System.Windows.Forms.MouseButtons.Middle:
					_isPanning = true;
					_panAnchorPoint = _imageViewer.GetCurrentCanvasPositionF(e.X, e.Y);
					openGLControl.Cursor = System.Windows.Forms.Cursors.Hand;
					break;
			}

			StartDrawingTimer();
		}

		private void OnMouseMove(object sender, CanvasMouseEventArgs e)
		{
			OpenGLControl openGLControl = (OpenGLControl)sender;
			if (_isPanning)
			{
				_imageViewer.PanToKeepPointAtMouse(_panAnchorPoint, new System.Drawing.Point(e.X, e.Y));
				UpdatePixelProperty();
				openGLControl.Cursor = System.Windows.Forms.Cursors.Hand;
				return;
			}

			System.Drawing.PointF currentImagePos = _imageViewer.GetCurrentCanvasPosition(e.X, e.Y);
			openGLControl.Cursor = RoiInteractionCursor.GetCursorFromType(GetCursorInteractionRect(currentImagePos), currentImagePos, _imageViewer.ZoomScale, _imageViewer.HandleSize);
			_imageViewer.PostMousePos = currentImagePos;

			switch (_imageViewer.GetViewMode())
			{
				case CanvasInteractionMode.Edit:
					RoiInteractionMouseMove.ResizeRoiRect(_imageViewer, _selectedRect, currentImagePos, _imageSize, OnRoiEditingCompleted);
					break;
				case CanvasInteractionMode.Move:
					RoiInteractionMouseMove.MoveOverlay(_imageViewer, _selectedRect, currentImagePos, _imageSize, true, OnRoiEditingCompleted, UseGroupMoveMode);
					break;
				case CanvasInteractionMode.Drawing:
					RoiInteractionMouseMove.UpdateRectangleToOverlay(_imageViewer, _drawingRect);
					break;
				case CanvasInteractionMode.Measure:
					RoiInteractionMouseMove.UpdateMeasurement(_imageViewer, ref _measurement);
					break;
			}

			UpdatePixelProperty();
		}

		private void OnMouseUp(object sender, CanvasMouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Middle)
			{
				_isPanning = false;
				return;
			}

			_imageViewer.PostMousePos = _imageViewer.GetCurrentCanvasPosition(e.X, e.Y);
			CanvasRect<float> mouseUpRect = GetActiveInteractionRect();
			if (_selectedRect != null) { _selectedRect.IsEditing = false; }
			if (_drawingRect != null) { _drawingRect.IsEditing = false; }

			bool hasValidLeftDrag = e.Button == System.Windows.Forms.MouseButtons.Left
				&& HasValidMouseDrag(_mouseDownCanvasPos, new System.Drawing.Point(e.X, e.Y))
				&& HasValidDrawingBounds(_imageViewer.PreMousePos, _imageViewer.PostMousePos);

			if (e.Button == System.Windows.Forms.MouseButtons.Left && !hasValidLeftDrag && _imageViewer.GetViewMode() == CanvasInteractionMode.Drawing)
			{
				_drawingRect = new CanvasRect<float>();
				mouseUpRect = _selectedRect;
			}

			if (hasValidLeftDrag)
			{
				if (IsAddRoiArrayMode)
				{
					RoiInteractionMouseUp.OpenAddRoiArrayView(_imageViewer, _addRoiArrayVm, OnRoiAdded);
					// ?�성???�료?�면 ?�당 모드�?종료?�다.
					IsAddRoiArrayMode = false;
				}
				else
				{
					if (_imageViewer.GetViewMode() == CanvasInteractionMode.Drawing && ReplaceExistingRoiOnDraw)
					{
						ReplaceWindowRoisForSingleDraw();
					}

					if (_imageViewer.GetViewMode() == CanvasInteractionMode.Drawing)
					{
						bool added = RoiInteractionMouseUp.AddRectangleToOverlay(_imageViewer, _imageViewer.PreMousePos, _imageViewer.PostMousePos, ref _drawingRect, OnRoiAdded);
						if (added)
						{
							_selectedRect = _drawingRect;
							mouseUpRect = _selectedRect;
						}
						_drawingRect = new CanvasRect<float>();
					}
				}
			}
			OnRoiMouseUp(mouseUpRect);
			ResetViewMode();
			CompleteRoiInteractionSnapshot("ROI Edit");
		}

		private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.Control)
			{
				if (e.KeyCode == System.Windows.Forms.Keys.Z && !e.Shift)
				{
					UndoRequested(this, EventArgs.Empty);
					e.Handled = true;
					e.SuppressKeyPress = true;
					return;
				}

				if (e.KeyCode == System.Windows.Forms.Keys.Y || (e.KeyCode == System.Windows.Forms.Keys.Z && e.Shift))
				{
					RedoRequested(this, EventArgs.Empty);
					e.Handled = true;
					e.SuppressKeyPress = true;
					return;
				}
			}

			switch (e.KeyCode)
			{
				case System.Windows.Forms.Keys.ShiftKey:
					break;
				case System.Windows.Forms.Keys.ControlKey:

					break;
				case System.Windows.Forms.Keys.Enter:

					break;
				case System.Windows.Forms.Keys.Delete:
					IReadOnlyList<Model.RoiSnapshotItem> beforeDelete = CaptureWindowRoiSnapshot();
					RemoveSelectedOverlay();
					PublishRoiSnapshotChanged("Delete ROI", beforeDelete, CaptureWindowRoiSnapshot());
					break;
			}

			if (e.Modifiers == System.Windows.Forms.Keys.Control)
			{
				switch (e.KeyCode)
				{
					case System.Windows.Forms.Keys.C:
						RoiInteractionKeyDown.CopyRectangle(_selectedRect, ref _copyRoiRect);
						break;
					case System.Windows.Forms.Keys.V:
						IReadOnlyList<Model.RoiSnapshotItem> beforePaste = CaptureWindowRoiSnapshot();
						RoiInteractionKeyDown.PasteRectangle(ImageViewer, ref _copyRoiRect, OnRoiAdded, OnRoiGrouped);
						PublishRoiSnapshotChanged("Paste ROI", beforePaste, CaptureWindowRoiSnapshot());
						break;
				}
			}
		}
		private void OnMouseWheel(object sender, CanvasMouseEventArgs e)
		{
			_imageViewer.AdjustOffsetForZoom(e.Location, _imageViewer.UpdateZoom(e.Delta));
			_imageViewer.Reshape();
		}

		private void OnMouseLeave(object sender, EventArgs e)
		{
			_isPanning = false;
		}

		private void OnMouseClicked(object sender, EventArgs e)
		{

		}

		private void OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{

		}

		private void OnMouseDoubleClicked(object sender, EventArgs e)
		{

		}

		private void OnResized(object sender, EventArgs e)
		{
			StartDrawingTimer();
		}

		private void OnLoad(object sender, EventArgs e)
		{

		}

		private CanvasRect<float> GetActiveInteractionRect()
		{
			return _imageViewer.GetViewMode() == CanvasInteractionMode.Drawing ? _drawingRect : _selectedRect;
		}

		private CanvasRect<float> GetOverlayRect()
		{
			if (_imageViewer.GetViewMode() == CanvasInteractionMode.Drawing && _drawingRect != null && !_drawingRect.IsEmpty())
			{
				return _drawingRect;
			}

			if (_selectedRect != null && !_selectedRect.IsEmpty())
			{
				return _selectedRect;
			}

			return null;
		}

		private CanvasRect<float> GetCursorInteractionRect(System.Drawing.PointF currentImagePos)
		{
			switch (_imageViewer.GetViewMode())
			{
				case CanvasInteractionMode.Edit:
				case CanvasInteractionMode.Move:
					return _selectedRect;
				case CanvasInteractionMode.Drawing:
					if (_drawingRect != null && _drawingRect.IsEditing)
					{
						return _drawingRect;
					}
					break;
			}

			var (hoverRect, _) = RoiInteractionMouseDown.FindOverlayAtPosition(_imageViewer, currentImagePos);
			if (hoverRect != null)
			{
				return hoverRect;
			}

			return GetOverlayRect();
		}

		private void ClearSelection()
		{
			if (_selectedRect != null)
			{
				_selectedRect.IsEditing = false;
				_selectedRect.IsChanged = true;
			}

			if (_drawingRect != null)
			{
				_drawingRect.IsEditing = false;
				_drawingRect.IsChanged = true;
			}

			_selectedRect = new CanvasRect<float>();
			_drawingRect = new CanvasRect<float>();
		}

		private void ResetViewMode()
		{
			if (_imageViewer.GetViewMode() == CanvasInteractionMode.Drag) { _imageViewer.SetViewMode(CanvasInteractionMode.None); }
			if (_imageViewer.GetViewMode() == CanvasInteractionMode.Move) { _imageViewer.SetViewMode(CanvasInteractionMode.None); }
			if (_imageViewer.GetViewMode() == CanvasInteractionMode.Edit) { _imageViewer.SetViewMode(CanvasInteractionMode.None); }
			if (IsTeachingMode && _imageViewer.GetViewMode() == CanvasInteractionMode.None) { _imageViewer.SetViewMode(CanvasInteractionMode.Drawing); }
		}

		private static bool HasValidDrawingBounds(System.Drawing.PointF preMousePos, System.Drawing.PointF postMousePos)
		{
			return Math.Abs(postMousePos.X - preMousePos.X) > 0 && Math.Abs(postMousePos.Y - preMousePos.Y) > 0;
		}

		private static bool HasValidMouseDrag(System.Drawing.Point startPoint, System.Drawing.Point endPoint)
		{
			const int minimumDrawingPixels = 2;
			return Math.Abs(endPoint.X - startPoint.X) >= minimumDrawingPixels && Math.Abs(endPoint.Y - startPoint.Y) >= minimumDrawingPixels;
		}

		private void RemoveSelectedOverlay()
		{
			OnRemoveOverlay(ref _selectedRect);
		}

		private void BeginRoiInteractionSnapshot()
		{
			_roiSnapshotBeforeInteraction = CaptureWindowRoiSnapshot();
			_roiInteractionSnapshotActive = true;
		}

		private void CompleteRoiInteractionSnapshot(string actionName)
		{
			if (!_roiInteractionSnapshotActive)
			{
				return;
			}

			IReadOnlyList<Model.RoiSnapshotItem> before = _roiSnapshotBeforeInteraction;
			_roiSnapshotBeforeInteraction = new List<Model.RoiSnapshotItem>();
			_roiInteractionSnapshotActive = false;
			PublishRoiSnapshotChanged(actionName, before, CaptureWindowRoiSnapshot());
		}

		private void PublishRoiSnapshotChanged(
			string actionName,
			IReadOnlyList<Model.RoiSnapshotItem> before,
			IReadOnlyList<Model.RoiSnapshotItem> after)
		{
			if (AreSameRoiSnapshots(before, after))
			{
				return;
			}

			RoiSnapshotChanged(this, new Model.RoiSnapshotChangedEventArgs(actionName, before, after));
		}

		public IReadOnlyList<Model.RoiSnapshotItem> CaptureWindowRoiSnapshot()
		{
			return _imageViewer.GetVisibleUnlockedOverlays()
				.Where(item => item?.Shape != null
					&& !item.IsGroupRectangle
					&& item.ItemType == EnumItemType.Window)
				.Select(Model.RoiSnapshotItem.FromOverlay)
				.Where(item => item != null)
				.ToList();
		}

		public void RestoreWindowRoiSnapshot(IEnumerable<Model.RoiSnapshotItem> snapshot)
		{
			IReadOnlyList<Model.RoiSnapshotItem> items = Model.RoiSnapshotChangedEventArgs.CloneSnapshot(snapshot);
			CanvasOverlayItem lastGroup = _imageViewer.GetLastGroup();
			string fallbackGroupType = lastGroup?.GroupType ?? string.Empty;

			List<string> currentIds = _imageViewer.GetVisibleUnlockedOverlays()
				.Where(item => item?.Shape != null
					&& !item.IsGroupRectangle
					&& item.ItemType == EnumItemType.Window)
				.Select(item => item.Shape.UniqueId)
				.Where(id => !string.IsNullOrWhiteSpace(id))
				.ToList();

			foreach (string uniqueId in currentIds)
			{
				_imageViewer.DeleteOverlay(uniqueId, fallbackGroupType);
			}

			CanvasRect<float> lastRect = null;
			foreach (Model.RoiSnapshotItem item in items)
			{
				string parentGroupType = string.IsNullOrWhiteSpace(item.ParentGroupType) ? fallbackGroupType : item.ParentGroupType;
				string groupType = string.IsNullOrWhiteSpace(item.GroupType) ? parentGroupType : item.GroupType;
				string uniqueId = string.IsNullOrWhiteSpace(item.UniqueId) ? Guid.NewGuid().ToString() : item.UniqueId;
				CanvasRect<float> rect = item.ToCanvasRect();
				rect.UniqueId = uniqueId;
				rect.GroupType = groupType;

				_imageViewer.AddOverlay(parentGroupType, groupType, rect, uniqueId, item.InspWindowType, item.ItemType, item.IsExtensionRectangle, item.IsGroupRectangle);
				lastRect = rect;
			}

			_selectedRect = lastRect ?? new CanvasRect<float>();
			_drawingRect = new CanvasRect<float>();
			_imageViewer.RefreshGL();
		}

		private static bool AreSameRoiSnapshots(IReadOnlyList<Model.RoiSnapshotItem> before, IReadOnlyList<Model.RoiSnapshotItem> after)
		{
			List<Model.RoiSnapshotItem> left = Model.RoiSnapshotChangedEventArgs.CloneSnapshot(before)
				.OrderBy(item => item.UniqueId ?? string.Empty)
				.ToList();
			List<Model.RoiSnapshotItem> right = Model.RoiSnapshotChangedEventArgs.CloneSnapshot(after)
				.OrderBy(item => item.UniqueId ?? string.Empty)
				.ToList();

			if (left.Count != right.Count)
			{
				return false;
			}

			for (int i = 0; i < left.Count; i++)
			{
				if (!left[i].HasSameGeometry(right[i]))
				{
					return false;
				}
			}

			return true;
		}

		private void ClearWindowRois()
		{
			var removableIds = _imageViewer.GetVisibleUnlockedOverlays()
				.Where(x => !x.IsGroupRectangle && x.ItemType == EnumItemType.Window && x.Shape != null)
				.Select(x => x.Shape.UniqueId)
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ToList();

			foreach (string uniqueId in removableIds)
			{
				_imageViewer.DeleteOverlay(uniqueId, _imageViewer.GetLastGroup()?.GroupType ?? string.Empty);
			}
		}

		private void ReplaceWindowRoisForSingleDraw()
		{
			ClearWindowRois();
			_selectedRect = new CanvasRect<float>();
		}

		public void UpdatePixelProperty()
		{
			OnPropertyChanged(nameof(GrayValue));
			OnPropertyChanged(nameof(CanvasPos));
			OnPropertyChanged(nameof(ImagePos));
			OnPropertyChanged(nameof(PixelColor));
			OnPropertyChanged(nameof(HeightValue));
		}

		[Obsolete("Use UpdatePixelProperty instead.")]
		public void UpdatePiexelProperty()
		{
			UpdatePixelProperty();
		}

		public void LoadImage(Mat mat, string fileName)
		{
			LoadImage(mat, fileName, true);
		}

		public void LoadImage(Mat mat, string fileName, bool keepCurrentImage, Func<string, bool> saveImageOverride = null)
		{
			//Ex. Load Image
			//LoadImageRequested(this, mat);
			_currentImageMat?.Dispose();
			_currentImageMat = keepCurrentImage && mat != null && !mat.Empty() ? mat.Clone() : null;
			_saveImageOverride = saveImageOverride;
			_currentImageName = string.IsNullOrWhiteSpace(fileName)
				? "Image"
				: System.IO.Path.GetFileNameWithoutExtension(fileName);
			CanvasImageLoader.UploadMatAsTexture(_imageViewer, mat, fileName, ref _imageSize);
		}

		public void LoadImage(System.Drawing.Bitmap bitmap, string fileName, Func<string, bool> saveImageOverride = null)
		{
			_currentImageMat?.Dispose();
			_currentImageMat = null;
			_saveImageOverride = saveImageOverride;
			_currentImageName = string.IsNullOrWhiteSpace(fileName)
				? "Image"
				: System.IO.Path.GetFileNameWithoutExtension(fileName);
			CanvasImageLoader.UploadBitmapAsTexture(_imageViewer, bitmap, fileName, ref _imageSize);
		}

		public void ClearImage()
		{
			_currentImageMat?.Dispose();
			_currentImageMat = null;
			_saveImageOverride = null;
			_currentImageName = "Image";
			_imageSize = new System.Drawing.Size();
			_imageViewer.ClearTexture();
			_imageViewer.RefreshGL();
		}

		public bool SaveCurrentImage(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return false;
			}

			string savePath = EnsureImageFileExtension(path);
			string directory = System.IO.Path.GetDirectoryName(savePath);
			if (!string.IsNullOrWhiteSpace(directory))
			{
				System.IO.Directory.CreateDirectory(directory);
			}

			if (_saveImageOverride != null)
			{
				return _saveImageOverride(savePath);
			}

			if (_currentImageMat == null || _currentImageMat.Empty())
			{
				return false;
			}

			return Cv2.ImWrite(savePath, _currentImageMat);
		}

		public void FitImageToView()
		{
			_imageViewer.ZoomToFit();
			_imageViewer.RefreshGL();
		}

		private static string EnsureImageFileExtension(string path)
		{
			return string.IsNullOrWhiteSpace(System.IO.Path.GetExtension(path))
				? path + ".png"
				: path;
		}

		public void AddInitialRoi(System.Drawing.Rectangle roi)
		{
			if (roi.IsEmpty || roi.Width <= 0 || roi.Height <= 0) { return; }

			CanvasOverlayItem parentOverlay = _imageViewer.GetLastGroup();
			if (parentOverlay == null) { return; }

			int canvasTop = _imageSize.Height > 0 ? _imageSize.Height - roi.Top : roi.Top + roi.Height;
			int canvasBottom = _imageSize.Height > 0 ? _imageSize.Height - roi.Bottom : roi.Top;

			CanvasRect<float> rect = new CanvasRect<float>(roi.Left, canvasTop, roi.Right, canvasBottom)
			{
				UniqueId = Guid.NewGuid().ToString()
			};

			_imageViewer.AddOverlay(parentOverlay.GroupType, parentOverlay.GroupType, rect, rect.UniqueId, parentOverlay.InspWindowType, EnumItemType.Window);
			_selectedRect = rect;
			_drawingRect = new CanvasRect<float>();
			OnRoiAdded(rect, parentOverlay);
			_imageViewer.RefreshGL();
		}

		#region EventHandler
		public void OnRoiGrouped(Model.RoiChangedEventArgs e)
		{
			RoiGrouped?.Invoke(this, e);
		}

		public void OnRoiAdded(CanvasRect<float> canvasRect, CanvasOverlayItem parentOverlay)
		{
			Model.RoiChangedEventArgs argOverlay = CreateRoiChangedEventArgs(canvasRect);
			argOverlay.Group = parentOverlay;
			RoiAdded(this, argOverlay);
		}

		private void OnRoiMouseUp(CanvasRect<float> canvasRect)
		{
			if (IsTeachingMode) { return; }
			if (canvasRect == null) { return; }
			if (canvasRect.IsEmpty()) { return; }
			Model.RoiChangedEventArgs argOverlay = CreateRoiChangedEventArgs(canvasRect);
			RoiMouseUp(this, argOverlay);
		}

		private void OnRemoveOverlay(ref CanvasRect<float> canvasRect)
		{
			if (canvasRect == null || string.IsNullOrWhiteSpace(canvasRect.UniqueId)) { return; }

			RemoveRoiRequested(this, canvasRect);
			_imageViewer.DeleteOverlay(canvasRect.UniqueId, canvasRect.GroupType);
			canvasRect = new CanvasRect<float>();
			_drawingRect = new CanvasRect<float>();
		}

		public void OnRoiEditingCompleted(CanvasRect<float> canvasRect)
		{
			if (canvasRect == null) { return; }

			Model.RoiChangedEventArgs argEdit = new Model.RoiChangedEventArgs
			{
				CanvasPoints = canvasRect.Points.Select(x => x.ToPointF()),
				RoiRect = canvasRect,
			};
			RoiEditingCompleted(this, argEdit);
		}

		private Model.RoiChangedEventArgs CreateRoiChangedEventArgs(CanvasRect<float> canvasRect)
		{
			Model.RoiChangedEventArgs arg = new Model.RoiChangedEventArgs();
			arg.CanvasPoints = canvasRect.Points.Select(x => x.ToPointF());
			arg.RoiRect = canvasRect;
			return arg;
		}

		#endregion
	}
}
