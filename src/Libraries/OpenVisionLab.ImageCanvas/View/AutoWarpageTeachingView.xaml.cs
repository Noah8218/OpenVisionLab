using OpenVisionLab.ImageCanvas.ViewModels;
using System.Windows;

namespace OpenVisionLab.ImageCanvas.Views
{
	/// <summary>
	/// AutoWarpageTeachingView.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class AutoWarpageTeachingView : Window
	{
		public AutoWarpageTeachingView()
		{
			InitializeComponent();
			this.Loaded += AutoWarpageTeachingView_Loaded;
		}

		private void AutoWarpageTeachingView_Loaded(object sender, RoutedEventArgs e)
		{
			var viewModel = this.DataContext as AutoWarpageTeachingViewModel;
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
