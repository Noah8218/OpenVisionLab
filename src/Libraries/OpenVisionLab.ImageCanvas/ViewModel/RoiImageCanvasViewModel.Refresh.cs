using System;

namespace OpenVisionLab.ImageCanvas.ViewModels
{
	public partial class RoiImageCanvasViewModel
	{
		public void StartDrawingTimer()
		{
			if (_refreshTimer == null) { return; }
			_refreshTimer.Start();
		}

		private void _dataTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			_refreshTimer.Stop();
			if (_imageViewer == null || _imageViewer.IsDisposed || !_imageViewer.IsHandleCreated)
			{
				return;
			}

			try
			{
				if (_imageViewer.InvokeRequired)
				{
					_imageViewer.BeginInvoke(new Action(() =>
					{
						if (_imageViewer == null || _imageViewer.IsDisposed || !_imageViewer.IsHandleCreated)
						{
							return;
						}

						_imageViewer.Reshape();
					}));
					return;
				}

				_imageViewer.Reshape();
			}
			catch (ObjectDisposedException)
			{
			}
			catch (InvalidOperationException)
			{
			}
		}

		public void Dispose()
		{
			if (_refreshTimer != null)
			{
				_refreshTimer.Stop();
				_refreshTimer.Elapsed -= _dataTimer_Elapsed;
				_refreshTimer.Dispose();
				_refreshTimer = null;
			}

			_currentImageMat?.Dispose();
			_currentImageMat = null;

			if (_imageViewer != null)
			{
				ReleaseEvents();
				_imageViewer.ClearTexture();
				_imageViewer.Dispose();
				_imageViewer = null;
			}
		}
	}
}
