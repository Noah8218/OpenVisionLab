using OpenVisionLab.ImageCanvas.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenVisionLab.ImageCanvas.Views
{
	public partial class RoiImageCanvasView : UserControl
	{
		public RoiImageCanvasView()
		{
			InitializeComponent();

			Loaded += ImageCanvasView_Loaded;
			PreviewKeyDown += ImageCanvasView_PreviewKeyDown;
			KeyUp += ImageCanvasView_KeyUp;
		}

		private void ImageCanvasView_Loaded(object sender, RoutedEventArgs e)
		{
			if (DataContext is RoiImageCanvasViewModel viewModel && viewModel.ImageViewer != null)
			{
				imageBoxCameraTwoD.Child = viewModel.ImageViewer;
				viewModel.ContextMenu = MainGrid.ContextMenu;
				MainGrid.ContextMenu.DataContext = viewModel;

				if (viewModel.LoadedCommand?.CanExecute(null) == true)
				{
					viewModel.LoadedCommand.Execute(null);
				}
			}
		}

		private void ImageCanvasView_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (DataContext is RoiImageCanvasViewModel viewModel && viewModel.PreviewKeyDownCommand?.CanExecute(e) == true)
			{
				viewModel.PreviewKeyDownCommand.Execute(e);
			}
		}

		private void ImageCanvasView_KeyUp(object sender, KeyEventArgs e)
		{
			if (DataContext is RoiImageCanvasViewModel viewModel && viewModel.KeyUpCommand?.CanExecute(e) == true)
			{
				viewModel.KeyUpCommand.Execute(e);
			}
		}
	}
}
