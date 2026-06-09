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
			_imageViewer.Reshape();
		}
	}
}
