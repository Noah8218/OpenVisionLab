using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.Infrastructure;
using OpenVisionLab.ImageCanvas.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace OpenVisionLab.ImageCanvas.ViewModels
{
	public class AutoAlignTeachingViewModel : ObservableObject
	{
		public ICommand ApplyCommand { get; set; }
		public ICommand CancelCommand { get; set; }

		private EnumAutoAlignDirection _autoAlignDirection = EnumAutoAlignDirection.TopLeft;
		public EnumAutoAlignDirection AutoAlignDirection
		{
			get => _autoAlignDirection;
			set
			{
				_autoAlignDirection = value;
				OnPropertyChanged();
			}
		}

		private string _gapDistance = "1";
		public string GapDistance
		{
			get { return _gapDistance; }
			set
			{
				_gapDistance = value;
				OnPropertyChanged();
			}
		}


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

		public IEnumerable<EnumAutoAlignDirection> AutoAlignType
		{
			get
			{
				return Enum.GetValues(typeof(EnumAutoAlignDirection)).Cast<EnumAutoAlignDirection>();
			}
		}

		public AutoAlignTeachingViewModel()
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
