using Microsoft.Win32;
using OpenVisionLab.ImageCanvas.Dialogs;

namespace OpenVisionLab.ImageCanvas.Views
{
	internal sealed class RoiImageCanvasDialogHost : IImageCanvasDialogHost
	{
		private bool isModalDialogOpen;

		public string ShowOpenImageDialog(string initialDirectory)
		{
			if (isModalDialogOpen)
			{
				return null;
			}

			isModalDialogOpen = true;
			try
			{
				OpenFileDialog dialog = new OpenFileDialog
				{
					Filter = "Image files (*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff)|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff|All files (*.*)|*.*",
					InitialDirectory = initialDirectory
				};

				return dialog.ShowDialog() == true ? dialog.FileName : null;
			}
			finally
			{
				isModalDialogOpen = false;
			}
		}

		public string ShowSaveImageDialog(string defaultFileName, string initialDirectory)
		{
			if (isModalDialogOpen)
			{
				return null;
			}

			isModalDialogOpen = true;
			try
			{
				SaveFileDialog dialog = new SaveFileDialog
				{
					Title = "Save Image",
					Filter = "PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg;*.jpeg|TIFF (*.tif)|*.tif;*.tiff",
					FileName = defaultFileName,
					InitialDirectory = initialDirectory,
					AddExtension = true,
					DefaultExt = ".png"
				};

				return dialog.ShowDialog() == true ? dialog.FileName : null;
			}
			finally
			{
				isModalDialogOpen = false;
			}
		}
	}
}
