using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using OpenVisionLab.ImageCanvas.Rendering;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Model = OpenVisionLab.ImageCanvas.Model;

namespace OpenVisionLab.ImageCanvas
{
	// Owns WinForms KeyDown policy only; WPF preview and mouse input stay in separate owners.
	internal sealed class RoiImageCanvasKeyboardInputController : IDisposable
	{
		private readonly ImageCanvasControl _imageViewer;
		private readonly Func<CanvasRect<float>> _selectedRect;
		private readonly Func<CanvasRect<float>> _copyRoiRect;
		private readonly Action<CanvasRect<float>> _setCopyRoiRect;
		private readonly Func<IReadOnlyList<Model.RoiSnapshotItem>> _captureSnapshot;
		private readonly Action _removeSelectedOverlay;
		private readonly Action<string, IReadOnlyList<Model.RoiSnapshotItem>, IReadOnlyList<Model.RoiSnapshotItem>> _publishSnapshot;
		private readonly Action _undo;
		private readonly Action _redo;
		private readonly OverlayAddedCallback _roiAdded;
		private readonly OverlayGroupAddedCallback _roiGrouped;
		private bool _disposed;

		internal RoiImageCanvasKeyboardInputController(
			ImageCanvasControl imageViewer,
			Func<CanvasRect<float>> selectedRect,
			Func<CanvasRect<float>> copyRoiRect,
			Action<CanvasRect<float>> setCopyRoiRect,
			Func<IReadOnlyList<Model.RoiSnapshotItem>> captureSnapshot,
			Action removeSelectedOverlay,
			Action<string, IReadOnlyList<Model.RoiSnapshotItem>, IReadOnlyList<Model.RoiSnapshotItem>> publishSnapshot,
			Action undo,
			Action redo,
			OverlayAddedCallback roiAdded,
			OverlayGroupAddedCallback roiGrouped)
		{
			_imageViewer = imageViewer ?? throw new ArgumentNullException(nameof(imageViewer));
			_selectedRect = selectedRect ?? throw new ArgumentNullException(nameof(selectedRect));
			_copyRoiRect = copyRoiRect ?? throw new ArgumentNullException(nameof(copyRoiRect));
			_setCopyRoiRect = setCopyRoiRect ?? throw new ArgumentNullException(nameof(setCopyRoiRect));
			_captureSnapshot = captureSnapshot ?? throw new ArgumentNullException(nameof(captureSnapshot));
			_removeSelectedOverlay = removeSelectedOverlay ?? throw new ArgumentNullException(nameof(removeSelectedOverlay));
			_publishSnapshot = publishSnapshot ?? throw new ArgumentNullException(nameof(publishSnapshot));
			_undo = undo ?? throw new ArgumentNullException(nameof(undo));
			_redo = redo ?? throw new ArgumentNullException(nameof(redo));
			_roiAdded = roiAdded;
			_roiGrouped = roiGrouped;

			_imageViewer.KeyDown += OnKeyDown;
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				if (e.KeyCode == Keys.Z && !e.Shift)
				{
					_undo();
					e.Handled = true;
					e.SuppressKeyPress = true;
					return;
				}

				if (e.KeyCode == Keys.Y || (e.KeyCode == Keys.Z && e.Shift))
				{
					_redo();
					e.Handled = true;
					e.SuppressKeyPress = true;
					return;
				}
			}

			switch (e.KeyCode)
			{
				case Keys.ShiftKey:
				case Keys.ControlKey:
				case Keys.Enter:
					break;
				case Keys.Delete:
					IReadOnlyList<Model.RoiSnapshotItem> beforeDelete = _captureSnapshot();
					_removeSelectedOverlay();
					_publishSnapshot("Delete ROI", beforeDelete, _captureSnapshot());
					break;
			}

			if (e.Modifiers == Keys.Control)
			{
				switch (e.KeyCode)
				{
					case Keys.C:
						CanvasRect<float> copySource = _selectedRect();
						CanvasRect<float> copyTarget = _copyRoiRect();
						RoiInteractionKeyDown.CopyRectangle(copySource, ref copyTarget);
						_setCopyRoiRect(copyTarget);
						break;
					case Keys.V:
						IReadOnlyList<Model.RoiSnapshotItem> beforePaste = _captureSnapshot();
						CanvasRect<float> pasteSource = _copyRoiRect();
						RoiInteractionKeyDown.PasteRectangle(_imageViewer, ref pasteSource, _roiAdded, _roiGrouped);
						_setCopyRoiRect(pasteSource);
						_publishSnapshot("Paste ROI", beforePaste, _captureSnapshot());
						break;
				}
			}
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;
			_imageViewer.KeyDown -= OnKeyDown;
		}
	}
}
