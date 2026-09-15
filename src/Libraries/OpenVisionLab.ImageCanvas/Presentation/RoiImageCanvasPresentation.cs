using Model = OpenVisionLab.ImageCanvas.Model;
using OpenCvSharp;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using OpenVisionLab.ImageCanvas.Overlays;
using OpenVisionLab.ImageCanvas.Rendering;
using OpenVisionLab.ImageCanvas.ViewModels;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenVisionLab.ImageCanvas.Presentation
{
	/// <summary>
	/// Owns the concrete WinForms/OpenGL control, its input adapters, render callbacks,
	/// refresh timer, and native disposal. RoiImageCanvasViewModel remains the mutable
	/// ROI and image-state owner and talks to this presentation boundary only.
	/// </summary>
	internal sealed class RoiImageCanvasPresentation : IDisposable
	{
		private readonly ImageCanvasControl imageViewer = new ImageCanvasControl();
		private RoiImageCanvasViewModel viewModel;
		private WeakReference<RoiImageCanvasViewModel> retainedStateOwner;
		private RoiImageCanvasKeyboardInputController keyboardInputController;
		private RoiImageCanvasMouseInputController mouseInputController;
		private System.Timers.Timer refreshTimer;
		private bool disposed;

		internal ImageCanvasControl Control => imageViewer;

		internal int GrayValue => imageViewer.GrayValue;

		internal PointF CanvasPosition => imageViewer.PixelPos;

		internal PointF ImagePosition => imageViewer.ImagePixelPos;

		internal Color PixelColor => imageViewer.PixelColor;

		internal int TextureTileCount => imageViewer.TextureAreas.Values.Sum(items => items?.Count ?? 0);

		internal void Attach(RoiImageCanvasViewModel nextViewModel)
		{
			if (disposed) { throw new ObjectDisposedException(nameof(RoiImageCanvasPresentation)); }
			if (nextViewModel == null) { throw new ArgumentNullException(nameof(nextViewModel)); }
			if (ReferenceEquals(viewModel, nextViewModel)) { return; }
			if (viewModel != null)
			{
				throw new InvalidOperationException("The ImageCanvas presentation is already attached to another ViewModel.");
			}

			bool preserveCanvasState = retainedStateOwner?.TryGetTarget(out RoiImageCanvasViewModel previousViewModel) == true
				&& ReferenceEquals(previousViewModel, nextViewModel);
			nextViewModel.AttachPresentation(this);
			viewModel = nextViewModel;
			if (!preserveCanvasState)
			{
				imageViewer.ClearTexture();
				imageViewer.ClearOverlays();
				InitializeDefaultGroup();
				retainedStateOwner = new WeakReference<RoiImageCanvasViewModel>(nextViewModel);
			}
			imageViewer.SetNameGL(viewModel.ViewerName);
			imageViewer.InvertYAxis = true;
			ApplyViewModelState();

			imageViewer.Load += OnLoad;
			imageViewer.Resized += OnResized;
			imageViewer.Draw += OnDraw;
			keyboardInputController = new RoiImageCanvasKeyboardInputController(
				imageViewer,
				() => viewModel.SelectedRect,
				() => viewModel.CopyRoiRect,
				value => viewModel.CopyRoiRect = value,
				viewModel.CaptureWindowRoiSnapshot,
				viewModel.RemoveSelectedOverlay,
				viewModel.PublishRoiSnapshotChanged,
				viewModel.RaiseUndoRequested,
				viewModel.RaiseRedoRequested,
				viewModel.OnRoiAdded,
				viewModel.OnRoiGrouped);
			mouseInputController = new RoiImageCanvasMouseInputController(
				imageViewer,
				() => viewModel.SelectedRect,
				value => viewModel.SelectedRect = value,
				() => viewModel.DrawingRect,
				value => viewModel.DrawingRect = value,
				() => viewModel.CurrentMeasurement,
				value => viewModel.CurrentMeasurement = value,
				() => viewModel.MouseDownCanvasPosition,
				value => viewModel.MouseDownCanvasPosition = value,
				() => viewModel.IsPanning,
				value => viewModel.IsPanning = value,
				() => viewModel.PanAnchorPoint,
				value => viewModel.PanAnchorPoint = value,
				() => viewModel.ImageSize,
				() => viewModel.IsAddRoiArrayMode,
				value => viewModel.IsAddRoiArrayMode = value,
				() => viewModel.IsTeachingMode,
				() => viewModel.ReplaceExistingRoiOnDraw,
				() => viewModel.UseGroupMoveMode,
				viewModel.AddRoiArrayViewModel,
				viewModel.BeginRoiInteractionSnapshot,
				viewModel.CompleteRoiInteractionSnapshot,
				viewModel.OnRoiMouseUp,
				viewModel.OnRoiEditingCompleted,
				viewModel.OnRoiAdded,
				viewModel.ReplaceWindowRoisForSingleDraw,
				viewModel.UpdatePixelProperty,
				viewModel.ExecuteRightClickCommand,
				StartDrawingTimer);

			refreshTimer = new System.Timers.Timer(1);
			refreshTimer.Elapsed += OnRefreshTimerElapsed;
			refreshTimer.Start();
		}

		internal void Detach(RoiImageCanvasViewModel currentViewModel)
		{
			if (!ReferenceEquals(viewModel, currentViewModel)) { return; }

			StopRefreshTimer();
			mouseInputController?.Dispose();
			mouseInputController = null;
			keyboardInputController?.Dispose();
			keyboardInputController = null;
			imageViewer.Load -= OnLoad;
			imageViewer.Resized -= OnResized;
			imageViewer.Draw -= OnDraw;
			viewModel.DetachPresentation(this);
			viewModel = null;
		}

		internal void SetShowCrossLine(bool value)
		{
			imageViewer.IsShowCrossLine = value;
			imageViewer.RefreshGL();
		}

		internal bool GetShowCrossLine() => imageViewer.IsShowCrossLine;

		internal void SetViewMode(CanvasInteractionMode mode) => imageViewer.SetViewMode(mode);

		internal void SetInvertYAxis(bool value) => imageViewer.InvertYAxis = value;

		internal CanvasInteractionMode GetViewMode() => imageViewer.GetViewMode();

		internal void SetGroupBoundsVisible(bool visible)
		{
			CanvasOverlayItem group = imageViewer.GetLastGroup();
			if (group == null) { return; }

			group.IsVisible = visible;
			group.Shape.IsChanged = true;
			imageViewer.RefreshGL();
		}

		internal bool GetGroupBoundsVisible() => imageViewer.GetLastGroup()?.IsVisible ?? true;

		internal IReadOnlyList<CanvasOverlayItem> GetVisibleUnlockedOverlays() => imageViewer.GetVisibleUnlockedOverlays();

		internal CanvasOverlayItem GetLastGroup() => imageViewer.GetLastGroup();

		internal CanvasOverlayItem GetOverlayByUniqueId(string uniqueId) => imageViewer.GetOverlayByUniqueId(uniqueId);

		internal void AddOverlay(
			string parentType,
			string childType,
			CanvasShape shape,
			string uniqueId,
			EnumInspWindowType inspectionWindowType = EnumInspWindowType.Unit,
			EnumItemType itemType = EnumItemType.Window,
			bool isExtensionRectangle = false,
			bool isGroupRectangle = false)
		{
			imageViewer.AddOverlay(parentType, childType, shape, uniqueId, inspectionWindowType, itemType, isExtensionRectangle, isGroupRectangle);
		}

		internal void DeleteOverlay(string uniqueId, string groupName) => imageViewer.DeleteOverlay(uniqueId, groupName);

		internal System.Drawing.Size UploadMat(Mat mat, string imageName)
		{
			System.Drawing.Size imageSize = System.Drawing.Size.Empty;
			CanvasImageLoader.UploadMatAsTexture(imageViewer, mat, imageName, ref imageSize);
			return imageSize;
		}

		internal System.Drawing.Size UploadBitmap(Bitmap bitmap, string imageName)
		{
			System.Drawing.Size imageSize = System.Drawing.Size.Empty;
			CanvasImageLoader.UploadBitmapAsTexture(imageViewer, bitmap, imageName, ref imageSize);
			return imageSize;
		}

		internal void ClearImage()
		{
			imageViewer.ClearTexture();
			imageViewer.RefreshGL();
		}

		internal void FitImageToView()
		{
			imageViewer.ZoomToFit();
			imageViewer.RefreshGL();
		}

		internal void Refresh() => imageViewer.RefreshGL();

		internal void ReshapeAndRefresh()
		{
			imageViewer.Reshape();
			imageViewer.RefreshGL();
		}

		internal CanvasViewState CaptureViewState() => imageViewer.CaptureViewState();

		internal void ApplyViewState(CanvasViewState state) => imageViewer.ApplyViewState(state);

		internal void StartDrawingTimer() => refreshTimer?.Start();

		private void InitializeDefaultGroup()
		{
			string groupType = EnumInspWindowType.Module.ToString();
			imageViewer.AddOverlay("", groupType, new CanvasRect<float>(), Guid.NewGuid().ToString(), EnumInspWindowType.Module, EnumItemType.Group, false, true);
			imageViewer.SetLastGroupType(groupType);
		}

		private void ApplyViewModelState()
		{
			imageViewer.IsShowCrossLine = viewModel.IsShowCrossLine;
			imageViewer.SetViewMode(viewModel.ResolveInteractionMode());
			SetGroupBoundsVisible(viewModel.RequestedShowGroupBounds);
		}

		private void OnDraw(object sender, CanvasRenderEventArgs e)
		{
			RoiImageCanvasViewModel current = viewModel;
			if (current == null) { return; }

			OpenGL gl = e.GL;
			imageViewer.DrawContent();
			CanvasRect<float> overlayRect = imageViewer.GetViewMode() == CanvasInteractionMode.Drawing
				? current.DrawingRect
				: current.SelectedRect;
			if (overlayRect != null && overlayRect.IsEmpty()) { overlayRect = null; }
			OpenGlDrawing.DrawRoiEditHandles(gl, overlayRect, imageViewer.ZoomScale, System.Windows.Media.Brushes.Yellow);
			if (current.ShowGroupNames)
			{
				OpenGlDrawing.DrawGroupName(gl, imageViewer.GetCanvasOverlayManager(), imageViewer.GetOpenGlTextDrawOptions());
			}
			if (current.ShowRoiItemNames)
			{
				OpenGlDrawing.DrawRoiItemName(gl, imageViewer.GetCanvasOverlayManager(), imageViewer.GetOpenGlTextDrawOptions());
			}
			if (current.IsShowMeasure)
			{
				imageViewer.DrawMeasurement(gl, current.CurrentMeasurement, current.MeasureFontOption);
			}
		}

		private void OnResized(object sender, EventArgs e) => StartDrawingTimer();

		private void OnLoad(object sender, EventArgs e)
		{
		}

		private void OnRefreshTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			System.Timers.Timer timer = refreshTimer;
			if (timer == null) { return; }

			try
			{
				timer.Stop();
				if (imageViewer.IsDisposed || !imageViewer.IsHandleCreated) { return; }
				if (imageViewer.InvokeRequired)
				{
					imageViewer.BeginInvoke(new Action(() =>
					{
						if (!imageViewer.IsDisposed && imageViewer.IsHandleCreated)
						{
							imageViewer.Reshape();
						}
					}));
					return;
				}

				imageViewer.Reshape();
			}
			catch (ObjectDisposedException)
			{
			}
			catch (InvalidOperationException)
			{
			}
		}

		private void StopRefreshTimer()
		{
			if (refreshTimer == null) { return; }

			refreshTimer.Stop();
			refreshTimer.Elapsed -= OnRefreshTimerElapsed;
			refreshTimer.Dispose();
			refreshTimer = null;
		}

		public void Dispose()
		{
			if (disposed) { return; }

			disposed = true;
			if (viewModel != null)
			{
				Detach(viewModel);
			}
			StopRefreshTimer();
			retainedStateOwner = null;
			imageViewer.ClearTexture();
			imageViewer.ClearOverlays();
			imageViewer.Dispose();
		}
	}
}
