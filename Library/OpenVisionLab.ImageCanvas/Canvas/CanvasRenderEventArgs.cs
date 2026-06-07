using SharpGL;
using System;

namespace OpenVisionLab.ImageCanvas.Canvas
{
	public sealed class CanvasRenderEventArgs : EventArgs
	{
		public CanvasRenderEventArgs(OpenGL gl)
		{
			OpenGL = gl;
		}

		public OpenGL OpenGL { get; }
		public OpenGL GL => OpenGL;
	}
}
