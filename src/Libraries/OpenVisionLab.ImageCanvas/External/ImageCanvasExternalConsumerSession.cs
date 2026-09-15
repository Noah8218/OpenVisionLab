using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using OpenVisionLab.ImageCanvas.Overlays;
using OpenVisionLab.ImageCanvas.Presentation;
using OpenVisionLab.ImageCanvas.Rendering;
using OpenVisionLab.ImageCanvas.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DrawingSize = System.Drawing.Size;

namespace OpenVisionLab.ImageCanvas.External
{
	/// <summary>
	/// Ownership contract for an image passed to an external ImageCanvas consumer.
	/// </summary>
	public enum ImageCanvasImageOwnership
	{
		/// <summary>The caller keeps the image and remains responsible for disposing it.</summary>
		Borrow,
		/// <summary>The session uploads a private copy and leaves the caller's image untouched.</summary>
		Clone,
		/// <summary>The session consumes and disposes the image before this call returns.</summary>
		TakeOwnership
	}

	/// <summary>
	/// A rectangle expressed in the source-image coordinate frame. Right and bottom are exclusive.
	/// </summary>
	public sealed class ImageCanvasSourceRectangle
	{
		public ImageCanvasSourceRectangle(float left, float top, float right, float bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public float Left { get; }
		public float Top { get; }
		public float Right { get; }
		public float Bottom { get; }
		public float Width => Right - Left;
		public float Height => Bottom - Top;
	}

	/// <summary>
	/// Minimal consumer-owned metadata for one visible rectangle overlay.
	/// </summary>
	public sealed class ImageCanvasExternalOverlay
	{
		public ImageCanvasExternalOverlay(
			string id,
			ImageCanvasSourceRectangle bounds,
			bool accepted,
			string rejectReason,
			Color color,
			float lineWidth = 1.5f)
		{
			Id = id;
			Bounds = bounds;
			Accepted = accepted;
			RejectReason = rejectReason ?? string.Empty;
			Color = color;
			LineWidth = lineWidth;
		}

		public string Id { get; }
		public ImageCanvasSourceRectangle Bounds { get; }
		public bool Accepted { get; }
		public string RejectReason { get; }
		public Color Color { get; }
		public float LineWidth { get; }
		public string StatusText => Accepted ? "Accepted" : "Rejected";
	}

	/// <summary>
	/// View state that does not expose the internal CanvasViewState type.
	/// </summary>
	public sealed class ImageCanvasExternalViewState
	{
		public ImageCanvasExternalViewState(float zoom, float offsetX, float offsetY)
		{
			Zoom = zoom;
			OffsetX = offsetX;
			OffsetY = offsetY;
		}

		public float Zoom { get; }
		public float OffsetX { get; }
		public float OffsetY { get; }
	}

	public sealed class ImageCanvasSelectionChangedEventArgs : EventArgs
	{
		public ImageCanvasSelectionChangedEventArgs(ImageCanvasExternalOverlay selectedOverlay)
		{
			SelectedOverlay = selectedOverlay;
			SelectedId = selectedOverlay?.Id;
		}

		public string SelectedId { get; }
		public ImageCanvasExternalOverlay SelectedOverlay { get; }
	}

	/// <summary>
	/// Small public boundary for a separate .NET 8 WPF consumer.
	/// This session owns the ImageCanvas presentation/native lifetime, a minimal WPF
	/// host, and consumer-owned overlay metadata; the ViewModel owns mutable ROI state.
	/// Calls that touch the session must be made on the WPF UI dispatcher.
	/// </summary>
	public sealed class ImageCanvasExternalConsumerSession : IDisposable
	{
		private const string ProjectionOverlayPrefix = "cross-modal-3d-to-2d:";

		private sealed class OverlayEntry
		{
			public OverlayEntry(ImageCanvasExternalOverlay descriptor, CanvasOverlayItem item, float baseLineWidth)
			{
				Descriptor = descriptor;
				Item = item;
				BaseLineWidth = baseLineWidth;
			}

			public ImageCanvasExternalOverlay Descriptor { get; }
			public CanvasOverlayItem Item { get; }
			public float BaseLineWidth { get; }
		}

		private readonly RoiImageCanvasViewModel viewModel;
		private readonly RoiImageCanvasPresentation presentation;
		private readonly ImageCanvasControl imageViewer;
		private readonly Grid hostView;
		private readonly System.Windows.Forms.Integration.WindowsFormsHost nativeHost;
		private readonly Dictionary<string, OverlayEntry> overlays = new(StringComparer.Ordinal);
		private DrawingSize imageSize;
		private string selectedOverlayId;
		private bool disposed;

		public ImageCanvasExternalConsumerSession(string name = "ExternalImageCanvas")
		{
			if (Application.Current != null && !Application.Current.Dispatcher.CheckAccess())
			{
				throw new InvalidOperationException("ImageCanvas consumer sessions must be created on the WPF UI dispatcher.");
			}

			presentation = new RoiImageCanvasPresentation();
			viewModel = new RoiImageCanvasViewModel(string.IsNullOrWhiteSpace(name) ? "ExternalImageCanvas" : name)
			{
				ShowGroupNames = false,
				ShowRoiItemNames = false,
				IsTeachingMode = false,
				IsShowMeasure = false
			};
			presentation.Attach(viewModel);
			imageViewer = presentation.Control;
			imageViewer.AutoSize = false;
			imageViewer.MinimumSize = System.Drawing.Size.Empty;
			imageViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			if (viewModel.LoadedCommand?.CanExecute(null) == true)
			{
				viewModel.LoadedCommand.Execute(null);
			}

			nativeHost = new System.Windows.Forms.Integration.WindowsFormsHost
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				Child = imageViewer
			};
			hostView = new Grid
			{
				Background = System.Windows.Media.Brushes.Black,
				ClipToBounds = true
			};
			hostView.Children.Add(nativeHost);
			hostView.Loaded += HostView_Loaded;
			hostView.SizeChanged += HostView_SizeChanged;
		}

		public event EventHandler<ImageCanvasSelectionChangedEventArgs> SelectionChanged = delegate { };

		/// <summary>WPF element that can be hosted by the external consumer.</summary>
		public FrameworkElement View => hostView;

		public int ImageWidth => imageSize.Width;
		public int ImageHeight => imageSize.Height;
		public bool HasImage => imageSize.Width > 0 && imageSize.Height > 0;
		public string SelectedOverlayId => selectedOverlayId;
		public ImageCanvasExternalOverlay SelectedOverlay =>
			selectedOverlayId != null && overlays.TryGetValue(selectedOverlayId, out OverlayEntry entry)
				? entry.Descriptor
				: null;
		public IReadOnlyList<ImageCanvasExternalOverlay> Overlays => overlays.Values.Select(x => x.Descriptor).ToList();
		public int ProjectionOverlayCount => overlays.Keys.Count(IsProjectionOverlayId);

		public void LoadImage(Bitmap image, string imageName, ImageCanvasImageOwnership ownership = ImageCanvasImageOwnership.Borrow)
		{
			EnsureUsable();
			EnsureUiAccess();
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}
			if (image.Width <= 0 || image.Height <= 0)
			{
				throw new ArgumentException("The image must have positive dimensions.", nameof(image));
			}
			if (ownership != ImageCanvasImageOwnership.Borrow
				&& ownership != ImageCanvasImageOwnership.Clone
				&& ownership != ImageCanvasImageOwnership.TakeOwnership)
			{
				throw new ArgumentOutOfRangeException(nameof(ownership));
			}
			EnsureNativeViewerReady();

			ClearOverlays();
			Bitmap uploadImage = image;
			try
			{
				if (ownership == ImageCanvasImageOwnership.Clone)
				{
					uploadImage = new Bitmap(image);
				}

				viewModel.LoadImage(uploadImage, string.IsNullOrWhiteSpace(imageName) ? "Image" : imageName);
				imageSize = image.Size;
			}
			finally
			{
				if (ownership != ImageCanvasImageOwnership.Borrow)
				{
					uploadImage.Dispose();
				}
			}
		}

		public void ClearImage()
		{
			EnsureUsable();
			EnsureUiAccess();
			EnsureNativeViewerReady();
			ClearOverlays();
			viewModel.ClearImage();
			imageSize = DrawingSize.Empty;
		}

		public void AddOverlay(ImageCanvasExternalOverlay overlay)
		{
			EnsureUsable();
			EnsureUiAccess();
			EnsureNativeViewerReady();
			ValidateOverlay(overlay);
			if (overlays.ContainsKey(overlay.Id))
			{
				throw new ArgumentException("An overlay with the same id already exists.", nameof(overlay));
			}

			ImageCanvasSourceRectangle bounds = overlay.Bounds;
			CanvasRect<float> rect = new CanvasRect<float>(
				bounds.Left,
				imageSize.Height - bounds.Top,
				bounds.Right,
				imageSize.Height - bounds.Bottom)
			{
				UniqueId = overlay.Id,
				LineWidth = overlay.LineWidth
			};

			imageViewer.AddOverlay(
				string.Empty,
				overlay.Id,
				rect,
				overlay.Id,
				EnumInspWindowType.Unit,
				EnumItemType.Window);

			CanvasOverlayItem item = imageViewer.GetOverlayByUniqueId(overlay.Id);
			if (item == null)
			{
				throw new InvalidOperationException("The ImageCanvas overlay was not created.");
			}

			item.Color = overlay.Color;
			item.Shape.IsChanged = true;
			item.Shape.OnChanged?.Invoke();
			overlays.Add(overlay.Id, new OverlayEntry(overlay, item, overlay.LineWidth));
			imageViewer.RefreshGL();
		}

		/// <summary>
		/// Replaces the read-only reverse-projection markers owned by this session.
		/// The marker is a small source-image rectangle so the existing renderer,
		/// zoom, pan, and clipping behavior remain authoritative.
		/// </summary>
		public int ReplaceProjectionMarkers(IEnumerable<ImageCanvasExternalProjectedPoint> points)
		{
			EnsureUsable();
			EnsureUiAccess();
			EnsureNativeViewerReady();

			foreach (string id in overlays.Keys.Where(IsProjectionOverlayId).ToList())
			{
				RemoveOverlay(id);
			}

			if (points == null)
			{
				return 0;
			}

			float halfSize = Math.Max(6.0f, Math.Min(imageSize.Width, imageSize.Height) / 36.0f);
			HashSet<string> ids = new HashSet<string>(StringComparer.Ordinal);
			int added = 0;
			foreach (ImageCanvasExternalProjectedPoint point in points)
			{
				if (point == null
					|| !IsFinite(point.ImageX)
					|| !IsFinite(point.ImageY)
					|| point.ImageX < 0
					|| point.ImageX >= imageSize.Width
					|| point.ImageY < 0
					|| point.ImageY >= imageSize.Height)
				{
					continue;
				}

				string id = ProjectionOverlayPrefix + point.Id;
				if (string.IsNullOrWhiteSpace(point.Id) || !ids.Add(id))
				{
					continue;
				}

				float x = (float)point.ImageX;
				float y = (float)point.ImageY;
				float left = Math.Max(0, x - halfSize);
				float top = Math.Max(0, y - halfSize);
				float right = Math.Min(imageSize.Width, x + halfSize);
				float bottom = Math.Min(imageSize.Height, y + halfSize);
				if (right <= left || bottom <= top)
				{
					continue;
				}

				bool accepted = string.Equals(point.InspectionStatus, "Pass", StringComparison.OrdinalIgnoreCase)
					|| string.Equals(point.InspectionStatus, "OK", StringComparison.OrdinalIgnoreCase);
				string label = string.IsNullOrWhiteSpace(point.Label) ? "3D-to-2D" : point.Label.Trim();
				string status = string.IsNullOrWhiteSpace(point.InspectionStatus) ? "Unknown" : point.InspectionStatus.Trim();
				AddOverlay(new ImageCanvasExternalOverlay(
					id,
					new ImageCanvasSourceRectangle(left, top, right, bottom),
					accepted,
					$"{label} · {status}",
					accepted ? Color.DeepSkyBlue : Color.OrangeRed,
					2.0f));
				added++;
			}

			return added;
		}

		public bool RemoveOverlay(string id)
		{
			EnsureUsable();
			EnsureUiAccess();
			if (string.IsNullOrWhiteSpace(id) || !overlays.ContainsKey(id))
			{
				return false;
			}

			if (string.Equals(selectedOverlayId, id, StringComparison.Ordinal))
			{
				ClearSelection();
			}

			imageViewer.DeleteOverlay(id, string.Empty);
			overlays.Remove(id);
			return true;
		}

		public bool SetOverlayVisible(string id, bool visible)
		{
			EnsureUsable();
			EnsureUiAccess();
			if (string.IsNullOrWhiteSpace(id) || !overlays.TryGetValue(id, out OverlayEntry entry))
			{
				return false;
			}

			entry.Item.IsVisible = visible;
			entry.Item.Shape.IsChanged = true;
			entry.Item.Shape.OnChanged?.Invoke();
			imageViewer.RefreshGL();
			return true;
		}

		public bool SelectOverlay(string id)
		{
			EnsureUsable();
			EnsureUiAccess();
			if (string.IsNullOrWhiteSpace(id) || !overlays.TryGetValue(id, out OverlayEntry entry))
			{
				return false;
			}
			if (string.Equals(selectedOverlayId, id, StringComparison.Ordinal))
			{
				return true;
			}

			if (selectedOverlayId != null && overlays.TryGetValue(selectedOverlayId, out OverlayEntry previous))
			{
				SetHighlight(previous, false);
			}

			selectedOverlayId = id;
			SetHighlight(entry, true);
			SelectionChanged(this, new ImageCanvasSelectionChangedEventArgs(entry.Descriptor));
			return true;
		}

		public void ClearSelection()
		{
			EnsureUsable();
			EnsureUiAccess();
			if (selectedOverlayId == null)
			{
				return;
			}

			if (overlays.TryGetValue(selectedOverlayId, out OverlayEntry previous))
			{
				SetHighlight(previous, false);
			}
			selectedOverlayId = null;
			SelectionChanged(this, new ImageCanvasSelectionChangedEventArgs(null));
		}

		public void FitImageToView()
		{
			EnsureUsable();
			EnsureUiAccess();
			EnsureNativeViewerReady();
			viewModel.FitImageToView();
		}

		public ImageCanvasExternalViewState CaptureViewState()
		{
			EnsureUsable();
			EnsureUiAccess();
			EnsureNativeViewerReady();
			CanvasViewState state = imageViewer.CaptureViewState();
			if (float.IsNaN(state.Zoom) || float.IsInfinity(state.Zoom)
				|| float.IsNaN(state.OffsetSize.Width) || float.IsInfinity(state.OffsetSize.Width)
				|| float.IsNaN(state.OffsetSize.Height) || float.IsInfinity(state.OffsetSize.Height)
				|| state.Zoom <= 0)
			{
				throw new InvalidOperationException("ImageCanvas returned an invalid view state.");
			}
			return new ImageCanvasExternalViewState(state.Zoom, state.OffsetSize.Width, state.OffsetSize.Height);
		}

		public void ApplyViewState(ImageCanvasExternalViewState state)
		{
			EnsureUsable();
			EnsureUiAccess();
			EnsureNativeViewerReady();
			if (state == null)
			{
				throw new ArgumentNullException(nameof(state));
			}
			if (float.IsNaN(state.Zoom) || float.IsInfinity(state.Zoom) || state.Zoom <= 0
				|| float.IsNaN(state.OffsetX) || float.IsInfinity(state.OffsetX)
				|| float.IsNaN(state.OffsetY) || float.IsInfinity(state.OffsetY))
			{
				throw new ArgumentException("View state values must be finite and have a positive zoom.", nameof(state));
			}

			imageViewer.ApplyViewState(new CanvasViewState(state.Zoom, new SizeF(state.OffsetX, state.OffsetY)));
		}

		public void Dispose()
		{
			if (disposed)
			{
				return;
			}

			if (Application.Current != null && !Application.Current.Dispatcher.CheckAccess())
			{
				throw new InvalidOperationException("ImageCanvas consumer sessions must be disposed on the WPF UI dispatcher.");
			}

			disposed = true;
			SelectionChanged = delegate { };
			hostView.Loaded -= HostView_Loaded;
			hostView.SizeChanged -= HostView_SizeChanged;
			nativeHost.Child = null;
			hostView.Children.Clear();
			presentation.Detach(viewModel);
			viewModel.Dispose();
			presentation.Dispose();
			overlays.Clear();
			selectedOverlayId = null;
			imageSize = DrawingSize.Empty;
		}

		private void ClearOverlays()
		{
			if (selectedOverlayId != null)
			{
				ClearSelection();
			}

			foreach (string id in overlays.Keys.ToList())
			{
				imageViewer.DeleteOverlay(id, string.Empty);
			}
			overlays.Clear();
		}

		private void SetHighlight(OverlayEntry entry, bool selected)
		{
			if (entry.Item.Shape is CanvasRect<float> rect)
			{
				rect.IsEditing = selected;
				rect.LineWidth = selected ? Math.Max(entry.BaseLineWidth + 1.0f, 2.5f) : entry.BaseLineWidth;
			}
			entry.Item.Shape.IsChanged = true;
			entry.Item.Shape.OnChanged?.Invoke();
			imageViewer.RefreshGL();
		}

		private void ValidateOverlay(ImageCanvasExternalOverlay overlay)
		{
			if (overlay == null)
			{
				throw new ArgumentNullException(nameof(overlay));
			}
			if (string.IsNullOrWhiteSpace(overlay.Id))
			{
				throw new ArgumentException("Overlay id is required.", nameof(overlay));
			}
			if (overlay.Bounds == null)
			{
				throw new ArgumentException("Overlay bounds are required.", nameof(overlay));
			}
			ImageCanvasSourceRectangle bounds = overlay.Bounds;
			if (float.IsNaN(bounds.Left) || float.IsInfinity(bounds.Left)
				|| float.IsNaN(bounds.Top) || float.IsInfinity(bounds.Top)
				|| float.IsNaN(bounds.Right) || float.IsInfinity(bounds.Right)
				|| float.IsNaN(bounds.Bottom) || float.IsInfinity(bounds.Bottom)
				|| bounds.Left < 0 || bounds.Top < 0
				|| bounds.Right <= bounds.Left || bounds.Bottom <= bounds.Top
				|| bounds.Right > imageSize.Width || bounds.Bottom > imageSize.Height)
			{
				throw new ArgumentException("Overlay bounds must be a finite, positive source-image rectangle.", nameof(overlay));
			}
			if (float.IsNaN(overlay.LineWidth) || float.IsInfinity(overlay.LineWidth) || overlay.LineWidth <= 0)
			{
				throw new ArgumentException("Overlay line width must be finite and positive.", nameof(overlay));
			}
		}

		private void EnsureNativeViewerReady()
		{
			hostView.UpdateLayout();
			if (hostView.ActualWidth > 0 && hostView.ActualHeight > 0
				&& (nativeHost.ActualWidth <= 0 || nativeHost.ActualHeight <= 0))
			{
				// HwndHost can report its desired slot before the first explicit arrange when
				// the viewer is introduced through an external ContentControl. Re-apply only
				// the already measured WPF slot; this does not change pipeline or image state.
				System.Windows.Size slot = new(hostView.ActualWidth, hostView.ActualHeight);
				nativeHost.Width = slot.Width;
				nativeHost.Height = slot.Height;
				nativeHost.Measure(slot);
				nativeHost.Arrange(new System.Windows.Rect(0, 0, slot.Width, slot.Height));
				nativeHost.UpdateLayout();
				hostView.UpdateLayout();
			}
			DrawingSize size = imageViewer.GetSize();
			if (hostView.IsLoaded && imageViewer.IsHandleCreated && (size.Width <= 0 || size.Height <= 0)
				&& hostView.ActualWidth > 0 && hostView.ActualHeight > 0)
			{
				int width = Math.Max(1, (int)Math.Round(hostView.ActualWidth));
				int height = Math.Max(1, (int)Math.Round(hostView.ActualHeight));
				imageViewer.AutoSize = false;
				imageViewer.SetBounds(0, 0, width, height);
				var nativeViewer = imageViewer.GetOpenGLControl();
				nativeViewer.AutoSize = false;
				nativeViewer.SetBounds(0, 0, width, height);
				nativeViewer.Dock = System.Windows.Forms.DockStyle.Fill;
				imageViewer.PerformLayout();
				size = imageViewer.GetSize();
			}
			if (!hostView.IsLoaded || !imageViewer.IsHandleCreated || size.Width <= 0 || size.Height <= 0)
			{
				throw new InvalidOperationException(
					$"Host the ImageCanvas View and wait until it is loaded and laid out before loading images or overlays. "
					+ $"View={hostView.ActualWidth:0.##}x{hostView.ActualHeight:0.##}, Canvas={imageViewer.Width}x{imageViewer.Height}, "
					+ $"Native={size.Width}x{size.Height}, Host={nativeHost.ActualWidth:0.##}x{nativeHost.ActualHeight:0.##} ({nativeHost.Width:0.##}x{nativeHost.Height:0.##}), "
					+ $"HostVisibility={nativeHost.Visibility}, HostDesired={nativeHost.DesiredSize.Width:0.##}x{nativeHost.DesiredSize.Height:0.##}, "
					+ $"Loaded={hostView.IsLoaded}, Handle={imageViewer.IsHandleCreated}.");
			}
		}

		private void HostView_Loaded(object sender, RoutedEventArgs e)
		{
			SyncHostedViewerBounds();
		}

		private void HostView_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			SyncHostedViewerBounds();
		}

		private void SyncHostedViewerBounds()
		{
			if (!hostView.IsLoaded || hostView.ActualWidth <= 0 || hostView.ActualHeight <= 0)
			{
				return;
			}

			int width = Math.Max(1, (int)Math.Round(hostView.ActualWidth));
			int height = Math.Max(1, (int)Math.Round(hostView.ActualHeight));
			nativeHost.Width = width;
			nativeHost.Height = height;
			imageViewer.AutoSize = false;
			imageViewer.Dock = System.Windows.Forms.DockStyle.None;
			imageViewer.SetBounds(0, 0, width, height);
			var openGlControl = imageViewer.GetOpenGLControl();
			openGlControl.AutoSize = false;
			openGlControl.Dock = System.Windows.Forms.DockStyle.Fill;
			openGlControl.SetBounds(0, 0, width, height);
			imageViewer.PerformLayout();
		}

		private void EnsureUiAccess()
		{
			if (Application.Current != null && !Application.Current.Dispatcher.CheckAccess())
			{
				throw new InvalidOperationException("ImageCanvas consumer calls must run on the WPF UI dispatcher.");
			}
		}

		private void EnsureUsable()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(nameof(ImageCanvasExternalConsumerSession));
			}
		}

		private static bool IsProjectionOverlayId(string id) =>
			id?.StartsWith(ProjectionOverlayPrefix, StringComparison.Ordinal) == true;

		private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
	}
}
