using OpenCvSharp;
using OpenVisionLab.ImageCanvas.Commands;
using OpenVisionLab.ImageCanvas.SharedViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace OpenVisionLab.ImageCanvas.ViewModels
{
	public partial class RoiImageCanvasViewModel
	{
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

			string fileName = ImageDialogHost?.ShowSaveImageDialog(CreateDefaultSaveFileName(), ImageCanvasDirectoryPolicy.ResolveInitialDirectory());
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

		private void ExecuteRightClickCommand()
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

			Stopwatch stopwatch = Stopwatch.StartNew();
			using (Mat mat = CanvasImageLoader.LoadMatFromFile(fileName))
			{
				Console.WriteLine($"LoadMatFromFile : {stopwatch.ElapsedMilliseconds}");
				Stopwatch stopwatch2 = Stopwatch.StartNew();
				LoadImage(mat, fileName);
				ImageCanvasDirectoryPolicy.RememberImagePath(fileName);
				Console.WriteLine($"LoadImage : {stopwatch2.ElapsedMilliseconds}");
			}
		}

		private string CreateDefaultSaveFileName()
		{
			string name = string.IsNullOrWhiteSpace(_currentImageName) ? "Image" : _currentImageName;
			foreach (char invalid in Path.GetInvalidFileNameChars())
			{
				name = name.Replace(invalid, '_');
			}

			return name + ".png";
		}

	}
}
