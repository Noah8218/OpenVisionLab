using OpenVisionLab.ImageCanvas.ViewModels;
using System.Windows;

namespace OpenVisionLab.ImageCanvas.Views
{
	/// <summary>
	/// TooltipView.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class AddRoiArrayView : Window
	{
		public AddRoiArrayView()
		{
			InitializeComponent();

			this.Loaded += AddRoiArrayView_Loaded;

			//this.KeyDown += (sender, e) =>
			//{
			//	if (e.Key == Key.Escape)
			//	{
			//		this.Close();
			//	}
			//};
		}

		private void AddRoiArrayView_Loaded(object sender, RoutedEventArgs e)
		{
			var viewModel = this.DataContext as AddRoiArrayViewModel;
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
