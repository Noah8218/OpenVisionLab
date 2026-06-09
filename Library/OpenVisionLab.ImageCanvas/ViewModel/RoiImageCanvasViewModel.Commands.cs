using Microsoft.Win32;
using OpenCvSharp;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.Commands;
using OpenVisionLab.ImageCanvas.SharedViewModels;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace OpenVisionLab.ImageCanvas.ViewModels
{
	public partial class RoiImageCanvasViewModel
	{
		private void OnMouseRightClick(CanvasContextMenuMode t)
		{
			ExecuteRightClickCommand();
		}

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
			RightClickCommand = new RelayCommand(ExecuteRightClickCommand);
			LoadImageCommand = new RelayCommand(OpenLoadImage);
			TeachingCommand = new RelayCommand(ChangeTeachingMode);
			AddingArrayCommand = new RelayCommand(ChangeAddingRoiArrayMode);
			ShowPreviewCommand = new RelayCommand(ChangePreviewMode);
			ShowCrossLineCommand = new RelayCommand(ShowCrossLine);
			MeasureCommand = new RelayCommand(ExecuteMeasure);
			PreviewKeyDownCommand = new RelayCommand<KeyEventArgs>(x => OnPreviewKeyDown(x));
			KeyUpCommand = new RelayCommand<KeyEventArgs>(x => OnPreviewKeyUp(x));
		}

		private void OnSaveIamge()
		{
			// Reserved for a future texture/image export command.
		}

		private void OnPreviewKeyUp(KeyEventArgs args)
		{
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				switch (args.Key)
				{
					case Key.C:
						break;
					case Key.V:
						break;
					case Key.S:
						break;
				}
			}
		}

		private void OnPreviewKeyDown(KeyEventArgs args)
		{
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				return;
			}

			switch (args.Key)
			{
				case Key.Delete:
					RemoveSelectedOverlay();
					args.Handled = true;
					break;
				case Key.F2:
					break;
				case Key.Enter:
					break;
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

		private void ExecuteRightClickCommand()
		{
			if (ContextMenu == null)
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

			ContextMenu.IsOpen = true;
		}

		private void OpenLoadImage()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Image files (*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*"
			};

			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}

			string fileName = openFileDialog.FileName;
			Stopwatch stopwatch = Stopwatch.StartNew();
			using (Mat mat = CanvasImageLoader.LoadMatFromFile(fileName))
			{
				Console.WriteLine($"LoadMatFromFile : {stopwatch.ElapsedMilliseconds}");
				Stopwatch stopwatch2 = Stopwatch.StartNew();
				LoadImage(mat, fileName);
				Console.WriteLine($"LoadImage : {stopwatch2.ElapsedMilliseconds}");
			}
		}
	}
}
