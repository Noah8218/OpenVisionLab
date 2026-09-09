using System.Windows;

namespace ImageCanvasExternalConsumerSmoke;

public partial class App : System.Windows.Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);
		MainWindow window = new MainWindow(e.Args);
		MainWindow = window;
		window.Show();
	}
}
