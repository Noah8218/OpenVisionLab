using System;
using System.IO;

namespace OpenVisionLab.ImageCanvas
{
	/// <summary>
	/// Owns the initial image-dialog directory and the last selected image path.
	/// </summary>
	internal static class ImageCanvasDirectoryPolicy
	{
		private static string lastImageDirectory;

		public static string ResolveInitialDirectory()
		{
			if (IsDirectory(lastImageDirectory))
			{
				return lastImageDirectory;
			}

			string sampleDirectory = ResolveSampleImageDirectory();
			if (IsDirectory(sampleDirectory))
			{
				return sampleDirectory;
			}

			if (IsDirectory(AppDomain.CurrentDomain.BaseDirectory))
			{
				return AppDomain.CurrentDomain.BaseDirectory;
			}

			string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			return IsDirectory(pictures) ? pictures : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
		}

		public static void RememberImagePath(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				return;
			}

			lastImageDirectory = Path.GetDirectoryName(filePath);
		}

		public static string ResolveImageName(string filePath)
		{
			return string.IsNullOrWhiteSpace(filePath) ? "Image" : Path.GetFileNameWithoutExtension(filePath);
		}

		public static string CreateDefaultSaveFileName(string imageName)
		{
			string name = string.IsNullOrWhiteSpace(imageName) ? "Image" : imageName;
			foreach (char invalid in Path.GetInvalidFileNameChars())
			{
				name = name.Replace(invalid, '_');
			}

			return name + ".png";
		}

		private static string ResolveSampleImageDirectory()
		{
			foreach (string root in new[] { AppDomain.CurrentDomain.BaseDirectory, Directory.GetCurrentDirectory() })
			{
				if (!IsDirectory(root))
				{
					continue;
				}

				DirectoryInfo directory = new DirectoryInfo(root);
				while (directory != null)
				{
					foreach (string sampleName in new[] { "Sample", "Samples", "samples" })
					{
						string candidate = Path.Combine(directory.FullName, sampleName);
						if (IsDirectory(candidate))
						{
							return candidate;
						}
					}

					directory = directory.Parent;
				}
			}

			return null;
		}

		private static bool IsDirectory(string path)
		{
			return !string.IsNullOrWhiteSpace(path) && Directory.Exists(path);
		}
	}
}
