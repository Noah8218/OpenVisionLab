using System;
using System.IO;
using OpenCvSharp;

namespace OpenVisionLab.ImageCanvas
{
	/// <summary>
	/// Owns the synchronous Mat-to-file policy used by the ImageCanvas facade.
	/// The caller retains ownership of the Mat and may provide an application save callback.
	/// </summary>
	internal static class CanvasImageSaver
	{
		public static bool SaveMat(Mat image, string path, Func<string, bool> saveOverride = null)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return false;
			}

			string savePath = EnsureImageFileExtension(path);
			string directory = Path.GetDirectoryName(savePath);
			if (!string.IsNullOrWhiteSpace(directory))
			{
				Directory.CreateDirectory(directory);
			}

			if (saveOverride != null)
			{
				return saveOverride(savePath);
			}

			if (image == null || image.Empty())
			{
				return false;
			}

			return Cv2.ImWrite(savePath, image);
		}

		private static string EnsureImageFileExtension(string path)
		{
			return string.IsNullOrWhiteSpace(Path.GetExtension(path))
				? path + ".png"
				: path;
		}
	}
}
