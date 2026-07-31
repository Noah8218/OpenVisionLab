using OpenVisionLab.ImageCanvas.ViewModels;
using System.Windows;

namespace OpenVisionLab.ImageCanvas.Views
{
	/// <summary>
	/// AutoAlignTeachingView.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class AutoAlignTeachingView : Window
	{
		public AutoAlignTeachingView()
		{
			InitializeComponent();

			this.Loaded += AutoAlignTeachingView_Loaded;
		}

		private void AutoAlignTeachingView_Loaded(object sender, RoutedEventArgs e)
		{
			var viewModel = this.DataContext as AutoAlignTeachingViewModel;
			if (viewModel != null)
			{
				viewModel.RequestClose += (result) =>
				{
					try
					{
						this.DialogResult = result;
					}
					catch { }
					this.Close();
				};
			}
		}
	}
}
