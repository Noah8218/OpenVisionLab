using System;
using OpenVisionLab.ImageCanvas.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace OpenVisionLab.ImageCanvas.Views
{
	public partial class RoiImageCanvasView : UserControl, IDisposable
	{
		private RoiImageCanvasViewModel attachedViewModel;
		private DispatcherOperation pendingImageViewerRefresh;
		private bool disposed;

		public static readonly DependencyProperty ShowStatusBarProperty =
			DependencyProperty.Register(
				nameof(ShowStatusBar),
				typeof(bool),
				typeof(RoiImageCanvasView),
				new PropertyMetadata(true, OnChromeVisibilityChanged));

		public static readonly DependencyProperty ShowToolBarProperty =
			DependencyProperty.Register(
				nameof(ShowToolBar),
				typeof(bool),
				typeof(RoiImageCanvasView),
				new PropertyMetadata(true, OnChromeVisibilityChanged));

		public RoiImageCanvasView()
		{
			InitializeComponent();
			ApplyChromeVisibility();

			Loaded += ImageCanvasView_Loaded;
			DataContextChanged += ImageCanvasView_DataContextChanged;
			SizeChanged += ImageCanvasView_SizeChanged;
			Unloaded += ImageCanvasView_Unloaded;
			PreviewKeyDown += ImageCanvasView_PreviewKeyDown;
			KeyUp += ImageCanvasView_KeyUp;
		}

		public bool ShowStatusBar
		{
			get => (bool)GetValue(ShowStatusBarProperty);
			set => SetValue(ShowStatusBarProperty, value);
		}

		public bool ShowToolBar
		{
			get => (bool)GetValue(ShowToolBarProperty);
			set => SetValue(ShowToolBarProperty, value);
		}

		private static void OnChromeVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is RoiImageCanvasView view)
			{
				view.ApplyChromeVisibility();
			}
		}

		private void ApplyChromeVisibility()
		{
			if (statusbar != null)
			{
				statusbar.Visibility = ShowStatusBar ? Visibility.Visible : Visibility.Collapsed;
				statusBarRow.Height = ShowStatusBar ? new GridLength(20) : new GridLength(0);
			}

			if (viewerToolBarTray != null)
			{
				viewerToolBarTray.Visibility = ShowToolBar ? Visibility.Visible : Visibility.Collapsed;
				toolBarColumn.Width = ShowToolBar ? new GridLength(35) : new GridLength(0);
			}
		}

		private void ImageCanvasView_Loaded(object sender, RoutedEventArgs e)
		{
			AttachImageViewer();
		}

		private void ImageCanvasView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (!ReferenceEquals(e.OldValue, e.NewValue))
			{
				DetachImageViewer();
			}

			if (IsLoaded)
			{
				AttachImageViewer();
			}
		}

		private void DetachImageViewer()
		{
			if (imageBoxCameraTwoD != null)
			{
				imageBoxCameraTwoD.Child = null;
			}

			if (MainGrid?.ContextMenu != null)
			{
				MainGrid.ContextMenu.DataContext = null;
			}

			attachedViewModel = null;
		}

		private void AttachImageViewer()
		{
			if (DataContext is RoiImageCanvasViewModel viewModel && viewModel.ImageViewer != null)
			{
				if (!ReferenceEquals(attachedViewModel, viewModel))
				{
					attachedViewModel = viewModel;
					imageBoxCameraTwoD.Child = viewModel.ImageViewer;
					viewModel.ContextMenu = MainGrid.ContextMenu;
					MainGrid.ContextMenu.DataContext = viewModel;

					if (viewModel.LoadedCommand?.CanExecute(null) == true)
					{
						viewModel.LoadedCommand.Execute(null);
					}
				}

				SyncHostedImageViewerBounds();

				pendingImageViewerRefresh = Dispatcher.BeginInvoke(new System.Action(() =>
				{
					pendingImageViewerRefresh = null;
					if (disposed || viewModel.ImageViewer == null)
					{
						return;
					}

					SyncHostedImageViewerBounds();
					viewModel.ImageViewer.Reshape();
					viewModel.ImageViewer.RefreshGL();
				}), DispatcherPriority.Loaded);
			}
		}

		private void ImageCanvasView_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			SyncHostedImageViewerBounds();
		}

		private void SyncHostedImageViewerBounds()
		{
			if (imageBoxCameraTwoD?.Child == null)
			{
				return;
			}

			int width = System.Math.Max(1, (int)System.Math.Round(imageBoxCameraTwoD.ActualWidth));
			int height = System.Math.Max(1, (int)System.Math.Round(imageBoxCameraTwoD.ActualHeight));
			System.Windows.Forms.Control child = imageBoxCameraTwoD.Child;

			// WindowsFormsHost can leave the native OpenGL HWND at its designer size; keep it inside the WPF slot.
			child.AutoSize = false;
			child.MinimumSize = System.Drawing.Size.Empty;
			child.Dock = System.Windows.Forms.DockStyle.None;
			child.SetBounds(0, 0, width, height);
			child.Dock = System.Windows.Forms.DockStyle.Fill;
		}
		private void ImageCanvasView_Unloaded(object sender, RoutedEventArgs e)
		{
			// Unloaded is raised when WindowsFormsHost/OpenGL views are reparented by WPF
			// containers such as AvalonDock. The DataContext owner controls the real lifetime.
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

		public void Dispose()
		{
			if (disposed)
			{
				return;
			}

			disposed = true;
			if (pendingImageViewerRefresh?.Status == DispatcherOperationStatus.Pending)
			{
				pendingImageViewerRefresh.Abort();
			}
			pendingImageViewerRefresh = null;
			Loaded -= ImageCanvasView_Loaded;
			DataContextChanged -= ImageCanvasView_DataContextChanged;
			SizeChanged -= ImageCanvasView_SizeChanged;
			Unloaded -= ImageCanvasView_Unloaded;
			PreviewKeyDown -= ImageCanvasView_PreviewKeyDown;
			KeyUp -= ImageCanvasView_KeyUp;
			attachedViewModel = null;
			if (MainGrid?.ContextMenu != null)
			{
				MainGrid.ContextMenu.DataContext = null;
			}
			DataContext = null;
			imageBoxCameraTwoD?.Dispose();
			Content = null;
			GC.SuppressFinalize(this);
		}
	}
}
