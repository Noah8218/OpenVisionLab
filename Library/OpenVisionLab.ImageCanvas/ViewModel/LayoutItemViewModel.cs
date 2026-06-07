using OpenVisionLab.ImageCanvas.Infrastructure;
using OpenVisionLab.ImageCanvas.Commands;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenVisionLab.ImageCanvas
{
	public class LayoutItemViewModel : ObservableObject
	{
		public ICommand ToggleIsSelectedCommand { get; private set; }

		private ImageSource _imageSource;
		private bool _isSelected = false;
		private bool _showLayout = false;
		private string _imageName = "";
		private string _imagePath = "";

		public Action<string, bool> OnChanged { get; set; } // 콜백 추가		

		public LayoutItemViewModel()
		{
			ToggleIsSelectedCommand = new RelayCommand(ToggleIsSelected);
		}

		private void ToggleIsSelected()
		{
			IsSelected = !IsSelected;
			OnPropertyChanged(nameof(IsSelected));
			OnChanged?.Invoke(_imageName, IsSelected);
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged();
			}
		}

		public string ImageName
		{
			get => _imageName;
			set
			{
				_imageName = value;
				OnPropertyChanged();
			}
		}
		public string ImagePath
		{
			get => _imagePath;
			set
			{
				_imagePath = value;
				OnPropertyChanged();
			}
		}

		public bool ShowLayout
		{
			get => _showLayout;
			set
			{
				_showLayout = value;
				OnPropertyChanged();
			}
		}

		public ImageSource ImageSource
		{
			get => _imageSource;
			set
			{
				_imageSource = value;
				OnPropertyChanged(nameof(ImageSource));
			}
		}
	}
}
