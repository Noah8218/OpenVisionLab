using System;
using System.Linq;
using System.Windows;

namespace OpenVisionLab.ImageCompare
{
	internal static class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			global::OpenVisionLab.OpenVisionLanguageService.Load();

			Application application = new Application
			{
				ShutdownMode = ShutdownMode.OnMainWindowClose
			};

			global::OpenVisionLab.ImageCompareWindow window = new global::OpenVisionLab.ImageCompareWindow();
			application.MainWindow = window;

			string[] imagePaths = args?
				.Where(path => !string.IsNullOrWhiteSpace(path))
				.ToArray() ?? Array.Empty<string>();

			if (imagePaths.Length >= 2)
			{
				window.Loaded += (sender, e) => window.LoadImages(imagePaths);
			}

			application.Run(window);
		}
	}
}
