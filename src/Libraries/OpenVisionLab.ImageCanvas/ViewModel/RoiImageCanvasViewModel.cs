using Model = OpenVisionLab.ImageCanvas.Model;
using OpenCvSharp;
using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Commands;
using OpenVisionLab.ImageCanvas.Dialogs;
using OpenVisionLab.ImageCanvas.Events;
using OpenVisionLab.ImageCanvas.Infrastructure;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using OpenVisionLab.ImageCanvas.Overlays;
using OpenVisionLab.ImageCanvas.Presentation;
using OpenVisionLab.ImageCanvas.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace OpenVisionLab.ImageCanvas.ViewModels
{
	public class RoiImageCanvasViewModel : ObservableObject, IDisposable
	{
		#region Core

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
		private bool _isShowCrossLine;
		private bool _showGroupBounds = true;
		private Measurement _measurement = new Measurement();
		private OpenGlFontRenderOptions _measureFontOption = new OpenGlFontRenderOptions(System.Drawing.Color.Red, "Arial", 20, "");
		protected CanvasRect<float> _selectedRect = new CanvasRect<float>();
		protected CanvasRect<float> _drawingRect = new CanvasRect<float>();
		protected CanvasRect<float> _copyRoiRect;
		private System.Drawing.Point _mouseDownCanvasPos = System.Drawing.Point.Empty;
		private System.Drawing.Size _imageSize = new System.Drawing.Size();
		private readonly RoiImageCanvasWpfKeyboardInputController _wpfKeyboardInputController;
		private readonly string _viewerName;
		private RoiImageCanvasPresentation _presentation;
		private Mat _currentImageMat;
		private string _currentImageName = "Image";
		private Func<string, bool> _saveImageOverride;
		private bool _isPanning;
		private System.Drawing.PointF _panAnchorPoint;
		private IReadOnlyList<Model.RoiSnapshotItem> _roiSnapshotBeforeInteraction = new List<Model.RoiSnapshotItem>();
		private bool _roiInteractionSnapshotActive;
		private bool _disposed;
		public float[] AfData3D = new float[10];
		private AddRoiArrayViewModel _addRoiArrayVm = new AddRoiArrayViewModel();
		#endregion

		#region Properties
		internal IImageCanvasContextMenuHost ContextMenuHost { get; set; }

		internal IImageCanvasDialogHost ImageDialogHost { get; set; }

		public int GrayValue
		{
			get => _presentation?.GrayValue ?? 0;
		}

		public bool IsShowCrossLine
		{
			get => _isShowCrossLine;
			set
			{
				_isShowCrossLine = value;
				_presentation?.SetShowCrossLine(value);
				OnPropertyChanged(nameof(IsShowCrossLine));
			}
		}

		public bool IsShowMeasure
		{
			get => _isShowMeasure;
			set
			{
				_isShowMeasure = value;
				_presentation?.SetViewMode(value ? CanvasInteractionMode.Measure : CanvasInteractionMode.None);
				if (!value)
				{
					_measurement = new Measurement();
				}
				OnPropertyChanged(nameof(IsShowMeasure));
			}
		}

		public bool IsTeachingMode
		{
			get => _isTeachingMode;
			set
			{
				_isTeachingMode = value;
				_presentation?.SetViewMode(value ? CanvasInteractionMode.Drawing : CanvasInteractionMode.None);
				OnPropertyChanged(nameof(IsTeachingMode));
			}
		}

		public bool IsAddRoiArrayMode
		{
			get => _isAddRoiArrayMode;
			set
			{
				_isAddRoiArrayMode = value;
				_presentation?.SetViewMode(value ? CanvasInteractionMode.Drawing : CanvasInteractionMode.None);
				OnPropertyChanged(nameof(IsAddRoiArrayMode));
			}
		}

		public bool IsPreviewMode
		{
			get => _isPreviewMode;
			set
			{
				_isPreviewMode = value;
				_presentation?.Refresh();
				OnPropertyChanged();
			}
		}

		public bool UseGroupMoveMode
		{
			get => _useGroupMoveMode;
			set
			{
				_useGroupMoveMode = value;
				OnPropertyChanged();
			}
		}

		public bool ShowGroupNames { get; set; } = true;
		public bool ShowRoiItemNames { get; set; } = true;
		public bool ReplaceExistingRoiOnDraw { get; set; }

		public bool ShowGroupBounds
		{
			get => _presentation?.GetGroupBoundsVisible() ?? _showGroupBounds;
			set
			{
				_showGroupBounds = value;
				_presentation?.SetGroupBoundsVisible(value);
			}
		}

		public System.Drawing.PointF CanvasPos
		{
			get => _presentation?.CanvasPosition ?? new System.Drawing.PointF();
		}

		public System.Drawing.PointF ImagePos
		{
			get => _presentation?.ImagePosition ?? new System.Drawing.PointF();
		}

		public float HeightValue
		{
			get => _heightValue;
			set
			{
				_heightValue = value;
				OnPropertyChanged();
			}
		}

		public System.Drawing.Color PixelColor
		{
			get => _presentation?.PixelColor ?? new System.Drawing.Color();
		}

		public int TextureTileCount => _presentation?.TextureTileCount ?? 0;

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
			_viewerName = string.IsNullOrWhiteSpace(name) ? "ImageCanvas" : name;
			_wpfKeyboardInputController = new RoiImageCanvasWpfKeyboardInputController(RemoveSelectedOverlay);
			InitCommand();
			InitMenuItems();
		}

		internal string ViewerName => _viewerName;

		internal CanvasRect<float> SelectedRect
		{
			get => _selectedRect;
			set => _selectedRect = value;
		}

		internal CanvasRect<float> DrawingRect
		{
			get => _drawingRect;
			set => _drawingRect = value;
		}

		internal CanvasRect<float> CopyRoiRect
		{
			get => _copyRoiRect;
			set => _copyRoiRect = value;
		}

		internal Measurement CurrentMeasurement
		{
			get => _measurement;
			set => _measurement = value;
		}

		internal OpenGlFontRenderOptions MeasureFontOption => _measureFontOption;

		internal System.Drawing.Point MouseDownCanvasPosition
		{
			get => _mouseDownCanvasPos;
			set => _mouseDownCanvasPos = value;
		}

		internal bool IsPanning
		{
			get => _isPanning;
			set => _isPanning = value;
		}

		internal System.Drawing.PointF PanAnchorPoint
		{
			get => _panAnchorPoint;
			set => _panAnchorPoint = value;
		}

		internal System.Drawing.Size ImageSize => _imageSize;

		internal AddRoiArrayViewModel AddRoiArrayViewModel => _addRoiArrayVm;

		internal bool RequestedShowGroupBounds => _showGroupBounds;

		internal CanvasInteractionMode ResolveInteractionMode()
		{
			if (_isShowMeasure) { return CanvasInteractionMode.Measure; }
			if (_isTeachingMode || _isAddRoiArrayMode) { return CanvasInteractionMode.Drawing; }
			return CanvasInteractionMode.None;
		}

		internal void AttachPresentation(RoiImageCanvasPresentation presentation)
		{
			if (_disposed) { throw new ObjectDisposedException(nameof(RoiImageCanvasViewModel)); }
			if (_presentation != null && !ReferenceEquals(_presentation, presentation))
			{
				throw new InvalidOperationException("RoiImageCanvasViewModel is already attached to another presentation owner.");
			}

			_presentation = presentation ?? throw new ArgumentNullException(nameof(presentation));
		}

		internal void DetachPresentation(RoiImageCanvasPresentation presentation)
		{
			if (ReferenceEquals(_presentation, presentation))
			{
				_presentation = null;
			}
		}

		private void Loaded()
		{
			_presentation?.SetInvertYAxis(true);
		}

		private void InitMenuItems()
		{
			MenuItems = new ObservableCollection<MenuItemViewModel>();
			MenuItems.Add(new MenuItemViewModel { Header = MenuItemUtil.GetDescription(EnumImageCanvasItems.LoadImage), Command = LoadImageCommand, IconData = MaterialIconData.Image, IsVisible = true });
			MenuItems.Add(new MenuItemViewModel { Header = MenuItemUtil.GetDescription(EnumImageCanvasItems.FitImage), Command = FitImageCommand, IconData = MaterialIconData.CheckCircle, IsVisible = true });
			MenuItems.Add(new MenuItemViewModel { Header = MenuItemUtil.GetDescription(EnumImageCanvasItems.SaveImage), Command = SaveImageCommand, IconData = MaterialIconData.ContentSave, IsVisible = true });
		}
		internal void RemoveSelectedOverlay()
		{
			OnRemoveOverlay(ref _selectedRect);
		}

		internal void BeginRoiInteractionSnapshot()
		{
			_roiSnapshotBeforeInteraction = CaptureWindowRoiSnapshot();
			_roiInteractionSnapshotActive = true;
		}

		internal void CompleteRoiInteractionSnapshot(string actionName)
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

		internal void PublishRoiSnapshotChanged(
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
			return RequirePresentation().GetVisibleUnlockedOverlays()
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
			RoiImageCanvasPresentation presentation = RequirePresentation();
			CanvasOverlayItem lastGroup = presentation.GetLastGroup();
			string fallbackGroupType = lastGroup?.GroupType ?? string.Empty;

			List<string> currentIds = presentation.GetVisibleUnlockedOverlays()
				.Where(item => item?.Shape != null
					&& !item.IsGroupRectangle
					&& item.ItemType == EnumItemType.Window)
				.Select(item => item.Shape.UniqueId)
				.Where(id => !string.IsNullOrWhiteSpace(id))
				.ToList();

			foreach (string uniqueId in currentIds)
			{
				presentation.DeleteOverlay(uniqueId, fallbackGroupType);
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

				presentation.AddOverlay(parentGroupType, groupType, rect, uniqueId, item.InspWindowType, item.ItemType, item.IsExtensionRectangle, item.IsGroupRectangle);
				lastRect = rect;
			}

			_selectedRect = lastRect ?? new CanvasRect<float>();
			_drawingRect = new CanvasRect<float>();
			presentation.Refresh();
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
			RoiImageCanvasPresentation presentation = RequirePresentation();
			var removableIds = presentation.GetVisibleUnlockedOverlays()
				.Where(x => !x.IsGroupRectangle && x.ItemType == EnumItemType.Window && x.Shape != null)
				.Select(x => x.Shape.UniqueId)
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ToList();

			foreach (string uniqueId in removableIds)
			{
				presentation.DeleteOverlay(uniqueId, presentation.GetLastGroup()?.GroupType ?? string.Empty);
			}
		}

		internal void ReplaceWindowRoisForSingleDraw()
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
			_currentImageName = ImageCanvasDirectoryPolicy.ResolveImageName(fileName);
			_imageSize = RequirePresentation().UploadMat(mat, fileName);
		}

		public void LoadImage(System.Drawing.Bitmap bitmap, string fileName, Func<string, bool> saveImageOverride = null)
		{
			_currentImageMat?.Dispose();
			_currentImageMat = null;
			_saveImageOverride = saveImageOverride;
			_currentImageName = ImageCanvasDirectoryPolicy.ResolveImageName(fileName);
			_imageSize = RequirePresentation().UploadBitmap(bitmap, fileName);
		}

		public void ClearImage()
		{
			_currentImageMat?.Dispose();
			_currentImageMat = null;
			_saveImageOverride = null;
			_currentImageName = "Image";
			_imageSize = new System.Drawing.Size();
			_presentation?.ClearImage();
		}

		public bool SaveCurrentImage(string path)
		{
			return CanvasImageSaver.SaveMat(_currentImageMat, path, _saveImageOverride);
		}

		public void FitImageToView()
		{
			RequirePresentation().FitImageToView();
		}

		public void AddOverlay(
			string parentType,
			string childType,
			CanvasShape shape,
			string uniqueId,
			EnumInspWindowType inspectionWindowType = EnumInspWindowType.Unit,
			EnumItemType itemType = EnumItemType.Window,
			bool isExtensionRectangle = false,
			bool isGroupRectangle = false)
		{
			RequirePresentation().AddOverlay(parentType, childType, shape, uniqueId, inspectionWindowType, itemType, isExtensionRectangle, isGroupRectangle);
		}

		public void DeleteOverlay(string uniqueId, string groupName = "") => RequirePresentation().DeleteOverlay(uniqueId, groupName);

		public CanvasOverlayItem GetOverlayByUniqueId(string uniqueId) => RequirePresentation().GetOverlayByUniqueId(uniqueId);

		public void RefreshCanvas() => _presentation?.Refresh();

		public void ReshapeAndRefresh() => _presentation?.ReshapeAndRefresh();

		public CanvasViewState CaptureViewState() => RequirePresentation().CaptureViewState();

		public void ApplyViewState(CanvasViewState state) => RequirePresentation().ApplyViewState(state);

		public void AddInitialRoi(System.Drawing.Rectangle roi)
		{
			if (roi.IsEmpty || roi.Width <= 0 || roi.Height <= 0) { return; }

			RoiImageCanvasPresentation presentation = RequirePresentation();
			CanvasOverlayItem parentOverlay = presentation.GetLastGroup();
			if (parentOverlay == null) { return; }

			int canvasTop = _imageSize.Height > 0 ? _imageSize.Height - roi.Top : roi.Top + roi.Height;
			int canvasBottom = _imageSize.Height > 0 ? _imageSize.Height - roi.Bottom : roi.Top;

			CanvasRect<float> rect = new CanvasRect<float>(roi.Left, canvasTop, roi.Right, canvasBottom)
			{
				UniqueId = Guid.NewGuid().ToString()
			};

			presentation.AddOverlay(parentOverlay.GroupType, parentOverlay.GroupType, rect, rect.UniqueId, parentOverlay.InspWindowType, EnumItemType.Window);
			_selectedRect = rect;
			_drawingRect = new CanvasRect<float>();
			OnRoiAdded(rect, parentOverlay);
			presentation.Refresh();
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

		internal void OnRoiMouseUp(CanvasRect<float> canvasRect)
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
			RequirePresentation().DeleteOverlay(canvasRect.UniqueId, canvasRect.GroupType);
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

		#endregion

		#region Commands

		private void AllOffVisiblility()
		{
			foreach (var item in MenuItems)
			{
				item.IsVisible = false;
			}
		}

		private void InitCommand()
		{
			LoadedCommand = new RelayCommand(() => Loaded());
			SaveImageCommand = new RelayCommand(() => OnSaveIamge());
			FitImageCommand = new RelayCommand(FitImageToView);
			RightClickCommand = new RelayCommand(ExecuteRightClickCommand);
			LoadImageCommand = new RelayCommand(OpenLoadImage);
			TeachingCommand = new RelayCommand(ChangeTeachingMode);
			AddingArrayCommand = new RelayCommand(ChangeAddingRoiArrayMode);
			ShowPreviewCommand = new RelayCommand(ChangePreviewMode);
			ShowCrossLineCommand = new RelayCommand(ShowCrossLine);
			MeasureCommand = new RelayCommand(ExecuteMeasure);
			PreviewKeyDownCommand = new RelayCommand<KeyEventArgs>(x => _wpfKeyboardInputController.HandlePreviewKeyDown(x));
			KeyUpCommand = new RelayCommand<KeyEventArgs>(x => _wpfKeyboardInputController.HandleKeyUp(x));
		}

		private void OnSaveIamge()
		{
			if (_currentImageMat == null || _currentImageMat.Empty())
			{
				return;
			}

			string fileName = ImageDialogHost?.ShowSaveImageDialog(ImageCanvasDirectoryPolicy.CreateDefaultSaveFileName(_currentImageName), ImageCanvasDirectoryPolicy.ResolveInitialDirectory());
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return;
			}

			if (SaveCurrentImage(fileName))
			{
				ImageCanvasDirectoryPolicy.RememberImagePath(fileName);
			}
		}

		private void ShowCrossLine() => IsShowCrossLine = !IsShowCrossLine;

		private void ExecuteMeasure()
		{
			bool enableMeasure = !IsShowMeasure;
			if (enableMeasure)
			{
				IsTeachingMode = false;
				IsAddRoiArrayMode = false;
			}

			IsShowMeasure = enableMeasure;
			OnWindowsChanged?.Invoke();
		}

		private void ChangePreviewMode() => IsPreviewMode = !IsPreviewMode;

		private void ChangeAddingRoiArrayMode()
		{
			if (IsAddRoiArrayMode)
			{
				IsAddRoiArrayMode = false;
				OnWindowsChanged?.Invoke();
			}
		}

		private void ChangeTeachingMode()
		{
			bool enableTeaching = !IsTeachingMode;
			if (enableTeaching)
			{
				IsShowMeasure = false;
				IsAddRoiArrayMode = false;
			}

			IsTeachingMode = enableTeaching;
			OnWindowsChanged?.Invoke();
		}

		internal void ExecuteRightClickCommand()
		{
			if (ContextMenuHost == null)
			{
				return;
			}

			if (IsShowMeasure || IsTeachingMode || IsAddRoiArrayMode)
			{
				IsShowMeasure = false;
				IsTeachingMode = false;
				IsAddRoiArrayMode = false;
				OnWindowsChanged?.Invoke();
				return;
			}

			ContextMenuHost.OpenContextMenu();
		}

		private void OpenLoadImage()
		{
			string fileName = ImageDialogHost?.ShowOpenImageDialog(ImageCanvasDirectoryPolicy.ResolveInitialDirectory());
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return;
			}

			using (Mat mat = CanvasImageLoader.LoadMatFromFile(fileName))
			{
				LoadImage(mat, fileName);
				ImageCanvasDirectoryPolicy.RememberImagePath(fileName);
			}
		}

		#endregion

		#region Refresh

		public void StartDrawingTimer() => _presentation?.StartDrawingTimer();

		internal void RaiseUndoRequested() => UndoRequested(this, EventArgs.Empty);

		internal void RaiseRedoRequested() => RedoRequested(this, EventArgs.Empty);

		private RoiImageCanvasPresentation RequirePresentation()
		{
			if (_disposed) { throw new ObjectDisposedException(nameof(RoiImageCanvasViewModel)); }
			return _presentation ?? throw new InvalidOperationException("Host RoiImageCanvasViewModel in a RoiImageCanvasView before using canvas operations.");
		}

		public void Dispose()
		{
			if (_disposed) { return; }
			_disposed = true;

			_currentImageMat?.Dispose();
			_currentImageMat = null;
			_presentation = null;
		}

		#endregion
	}
}
