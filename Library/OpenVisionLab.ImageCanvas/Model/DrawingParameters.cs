using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;

namespace OpenVisionLab.ImageCanvas.Model
{
	public class DrawingParameters
	{
		public string ImageName { get; set; }
		public SolidColorBrush ColorBrush { get; set; }
		public Point PreMousePos { get; set; }
		public Point PostMousePos { get; set; }
		public float LineWidth { get; set; }
		public EnumFillMode SelectedFillMode { get; set; }
		public List<PointF> PenPoints { get; set; }
		public System.Drawing.PointF CenterPos { get; set; }
		public float CircleRadius { get; set; }
		public bool IsFill { get; set; }
	}
}
