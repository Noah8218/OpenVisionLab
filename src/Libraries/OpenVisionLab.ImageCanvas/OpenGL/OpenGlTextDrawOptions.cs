using OpenVisionLab.ImageCanvas.Canvas;
using System.Collections.Generic;

namespace OpenVisionLab.ImageCanvas.OpenGLRendering
{
	public class OpenGlTextDrawOptions
	{
		public List<OpenGlFontBitmapEntry> FontBitmapEntries { get; set; }
		public float XSpan { get; set; }
		public float YSpan { get; set; }
		public System.Drawing.SizeF OffsetSize { get; set; }
		public float ZoomLevel { get; set; }
		public string Text { get; set; }
		public System.Drawing.Color Color { get; set; }
		public System.Drawing.PointF Pos { get; set; }

		public OpenGlTextDrawOptions(List<OpenGlFontBitmapEntry> fontBitmapEntries, float xSpan, float ySpan, System.Drawing.SizeF offsetSize, float zoomLevel)
		{
			this.XSpan = xSpan;
			this.YSpan = ySpan;
			this.FontBitmapEntries = fontBitmapEntries;
			this.OffsetSize = offsetSize;
			this.ZoomLevel = zoomLevel;
		}

		public OpenGlTextDrawOptions(List<OpenGlFontBitmapEntry> fontBitmapEntries, float xSpan, float ySpan, System.Drawing.SizeF offsetSize, float zoomLevel,
			string text, System.Drawing.Color color, System.Drawing.PointF pointF)
		{
			this.XSpan = xSpan;
			this.YSpan = ySpan;
			this.FontBitmapEntries = fontBitmapEntries;
			this.OffsetSize = offsetSize;
			this.ZoomLevel = zoomLevel;
			this.Text = text;
			this.Color = color;
			this.Pos = pointF;
		}
	}
}
