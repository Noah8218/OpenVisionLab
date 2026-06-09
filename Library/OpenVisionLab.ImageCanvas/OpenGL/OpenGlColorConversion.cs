using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.OpenCVSharp;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using SharpGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace OpenVisionLab.ImageCanvas.OpenGLRendering
{
	public static partial class OpenGlDrawing
	{		public static (float, float, float, float) ConvertColorToOpenGLRGB(System.Drawing.Color color)
		{
			float red = color.R / 255f;
			float green = color.G / 255f;
			float blue = color.B / 255f;
			float alpha = color.A / 255f;

			return (red, green, blue, alpha);
		}

		public static System.Drawing.Color ConvertOpenGLRGBToColor(float red, float green, float blue, float alpha)
		{
			return System.Drawing.Color.FromArgb(
				(int)(alpha * 255),
				(int)(red * 255),
				(int)(green * 255),
				(int)(blue * 255)
			);
		}

		public static float[] ConvertColorToOpenGLRGBArr(System.Drawing.Color color)
		{
			float red = color.R / 255f;
			float green = color.G / 255f;
			float blue = color.B / 255f;
			float alpha = color.A / 255f;

			return new float[4] { red, green, blue, alpha };
		}

		public static System.Drawing.Color ConvertOpenGLRGBArrToColor(float[] rgba)
		{
			if (rgba == null || rgba.Length != 4)
				throw new ArgumentException("Input array must have exactly 4 elements (R,G,B,A).");

			return System.Drawing.Color.FromArgb(
				(int)(rgba[3] * 255),
				(int)(rgba[0] * 255),
				(int)(rgba[1] * 255),
				(int)(rgba[2] * 255)
			);
		}

		public static (float, float, float, float) ConvertColorToOpenGLRGB(System.Windows.Media.SolidColorBrush brush)
		{
			float red = brush.Color.R / 255f;
			float green = brush.Color.G / 255f;
			float blue = brush.Color.B / 255f;
			float alpha = brush.Color.A / 255f;

			return (red, green, blue, alpha);
		}

		public static System.Windows.Media.SolidColorBrush ConvertOpenGLRGBToBrush(float red, float green, float blue, float alpha)
		{
			var color = System.Windows.Media.Color.FromArgb(
				(byte)(alpha * 255),
				(byte)(red * 255),
				(byte)(green * 255),
				(byte)(blue * 255)
			);
			return new System.Windows.Media.SolidColorBrush(color);
		}

		public static System.Windows.Media.SolidColorBrush ConvertOpenGLRGBArrToBrush(float[] rgba)
		{
			if (rgba == null || rgba.Length != 4)
				throw new ArgumentException("Input array must have exactly 4 elements (R,G,B,A).");

			var color = System.Windows.Media.Color.FromArgb(
				(byte)(rgba[3] * 255),
				(byte)(rgba[0] * 255),
				(byte)(rgba[1] * 255),
				(byte)(rgba[2] * 255)
			);
			return new System.Windows.Media.SolidColorBrush(color);
		}


		public static System.Windows.Media.Color ToMediaColor(System.Drawing.Color color)
		{
			return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
		}
	}
}
