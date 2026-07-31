using OpenVisionLab.ImageCanvas.Infrastructure;
using OpenVisionLab.ImageCanvas.Commands;
using System;
using System.Windows.Input;

namespace OpenVisionLab.ImageCanvas.ViewModels
{
	public class AddRoiArrayViewModel : ObservableObject
	{
		public ICommand ApplyCommand { get; set; }
		public ICommand CancelCommand { get; set; }

		private string _rows;
		private string _columns;
		private string _rowSpacing;
		private string _columnSpacing;

		public string Rows
		{
			get { return _rows; }
			set
			{
				_rows = value;
				OnPropertyChanged();
			}
		}

		public string Columns
		{
			get { return _columns; }
			set
			{
				_columns = value;
				OnPropertyChanged();
			}
		}

		public string RowSpacing
		{
			get { return _rowSpacing; }
			set
			{
				_rowSpacing = value;
				OnPropertyChanged();
			}
		}

		public string ColumnSpacing
		{
			get { return _columnSpacing; }
			set
			{
				_columnSpacing = value;
				OnPropertyChanged();
			}
		}

		public AddRoiArrayViewModel()
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
