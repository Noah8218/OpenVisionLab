using System;
using System.Windows.Input;

namespace OpenVisionLab.ImageCanvas
{
	// Owns WPF PreviewKeyDown/KeyUp policy only; the View keeps event forwarding and the WinForms controller remains separate.
	internal sealed class RoiImageCanvasWpfKeyboardInputController
	{
		private readonly Action _removeSelectedOverlay;

		internal RoiImageCanvasWpfKeyboardInputController(Action removeSelectedOverlay)
		{
			_removeSelectedOverlay = removeSelectedOverlay ?? throw new ArgumentNullException(nameof(removeSelectedOverlay));
		}

		internal void HandlePreviewKeyDown(KeyEventArgs args)
		{
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				return;
			}

			switch (args.Key)
			{
				case Key.Delete:
					_removeSelectedOverlay();
					args.Handled = true;
					break;
				case Key.F2:
				case Key.Enter:
					break;
			}
		}

		internal void HandleKeyUp(KeyEventArgs args)
		{
			if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
			{
				return;
			}

			switch (args.Key)
			{
				case Key.C:
				case Key.V:
				case Key.S:
					break;
			}
		}
	}
}
