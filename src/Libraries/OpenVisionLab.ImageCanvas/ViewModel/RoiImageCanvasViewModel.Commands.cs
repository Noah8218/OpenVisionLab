using Microsoft.Win32;
using OpenCvSharp;
using OpenVisionLab.ImageCanvas.Canvas;
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
			FitImageCommand = new RelayCommand(FitImageToView);
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
			if (_currentImageMat == null || _currentImageMat.Empty())
			{
				return;
			}

			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Title = "Save Image",
				Filter = "PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg;*.jpeg|TIFF (*.tif)|*.tif;*.tiff",
				FileName = CreateDefaultSaveFileName(),
				InitialDirectory = ResolveImageDialogDirectory(),
				AddExtension = true,
				DefaultExt = ".png"
			};

			if (saveFileDialog.ShowDialog() != true)
			{
				return;
			}

			if (SaveCurrentImage(saveFileDialog.FileName))
			{
				lastImageDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
			}
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
				Filter = "Image files (*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff)|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff|All files (*.*)|*.*",
				InitialDirectory = ResolveImageDialogDirectory()
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
				lastImageDirectory = Path.GetDirectoryName(fileName);
				Console.WriteLine($"LoadImage : {stopwatch2.ElapsedMilliseconds}");
			}
		}

		private static string lastImageDirectory;

		private string CreateDefaultSaveFileName()
		{
			string name = string.IsNullOrWhiteSpace(_currentImageName) ? "Image" : _currentImageName;
			foreach (char invalid in Path.GetInvalidFileNameChars())
			{
				name = name.Replace(invalid, '_');
			}

			return name + ".png";
		}

		private static string ResolveImageDialogDirectory()
		{
			if (IsDirectory(lastImageDirectory))
			{
				return lastImageDirectory;
			}

			string sampleDirectory = ResolveSampleImageDirectory();
			if (IsDirectory(sampleDirectory))
			{
				return sampleDirectory;
			}

			if (IsDirectory(AppDomain.CurrentDomain.BaseDirectory))
			{
				return AppDomain.CurrentDomain.BaseDirectory;
			}

			string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			return IsDirectory(pictures) ? pictures : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
		}

		private static string ResolveSampleImageDirectory()
		{
			foreach (string root in new[] { AppDomain.CurrentDomain.BaseDirectory, Directory.GetCurrentDirectory() })
			{
				if (!IsDirectory(root))
				{
					continue;
				}

				DirectoryInfo directory = new DirectoryInfo(root);
				while (directory != null)
				{
					foreach (string sampleName in new[] { "Sample", "Samples", "samples" })
					{
						string candidate = Path.Combine(directory.FullName, sampleName);
						if (IsDirectory(candidate))
						{
							return candidate;
						}
					}

					directory = directory.Parent;
				}
			}

			return null;
		}

		private static bool IsDirectory(string path)
		{
			return !string.IsNullOrWhiteSpace(path) && Directory.Exists(path);
		}
	}
}
