using OpenVisionLab.ImageCanvas.Infrastructure;
using OpenVisionLab.ImageCanvas.Commands;
using System;
using System.Windows.Input;

namespace OpenVisionLab.ImageCanvas.ViewModels
{
	public class AutoWarpageTeachingViewModel : ObservableObject
	{
		public ICommand ApplyCommand { get; set; }
		public ICommand CancelCommand { get; set; }


		private string _roiSizePermm = "30";
		public string RoiSizePermm
		{
			get { return _roiSizePermm; }
			set
			{
				_roiSizePermm = value;
				OnPropertyChanged();
			}
		}


		public AutoWarpageTeachingViewModel()
		{
			ApplyCommand = new RelayCommand(() => OnApply());
			CancelCommand = new RelayCommand(() => OnCancel());
		}
		public event Action<bool> RequestClose;

		private void OnApply()
		{
			RequestClose?.Invoke(true);
		}

		private void OnCancel()
		{
			RequestClose?.Invoke(false);
		}
	}
}
