using OpenVisionLab._1._Core;
using System.ComponentModel;
using System.Drawing;

namespace OpenVisionLab
{
	public class CViewer : Component
	{
		private Rectangle roi = Rectangle.Empty;
		private Rectangle trainRoi = Rectangle.Empty;
		private IDisplayManager displayManager;
		private int displayIndex = -1;

		public CViewer()
		{
		}

		public Rectangle Roi
		{
			get => HasDisplayContext ? displayManager.ImageSpace.GetRoi(displayIndex) : roi;
			set
			{
				roi = value;
				if (HasDisplayContext)
				{
					displayManager.ImageSpace.SetRoi(displayIndex, value);
				}
			}
		}

		public Rectangle TrainROI
		{
			get => HasDisplayContext ? displayManager.ImageSpace.GetTrainRoi(displayIndex) : trainRoi;
			set
			{
				trainRoi = value;
				if (HasDisplayContext)
				{
					displayManager.ImageSpace.SetTrainRoi(displayIndex, value);
				}
			}
		}

		private bool HasDisplayContext => displayManager != null && displayIndex >= 0;

		public void SetDisplayManager(IDisplayManager manager)
		{
			SetDisplayContext(manager, displayIndex, string.Empty);
		}

		public void SetDisplayContext(IDisplayManager manager, int index, string title)
		{
			displayManager = manager;
			displayIndex = index;

			if (!HasDisplayContext) { return; }

			roi = displayManager.ImageSpace.GetRoi(displayIndex);
			trainRoi = displayManager.ImageSpace.GetTrainRoi(displayIndex);
		}
	}
}
