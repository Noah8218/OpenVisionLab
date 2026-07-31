using System;

namespace OpenVisionLab.ImageCanvas.OpenGLRendering
{
	public sealed class OpenGlFontBitmapEntry
	{
		public string FaceName { get; set; }
		public int Height { get; set; }
		public uint DisplayListId { get; set; }
		public IntPtr HDC { get; set; }
		public IntPtr HRC { get; set; }
		public uint ListBase { get; set; }
		public int ListCount { get; set; }
	}
}
