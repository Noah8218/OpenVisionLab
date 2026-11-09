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
		private string _validationMessage = string.Empty;
		private string validationKey;

		internal AddRoiArrayData AcceptedData { get; private set; }

		public string ValidationMessage
		{
			get => _validationMessage;
			private set => SetProperty(ref _validationMessage, value);
		}

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
			AcceptedData = null;
			if (!int.TryParse(Rows, out int rows) || rows <= 0)
			{
				SetValidation("RoiArray.InvalidRows");
				return;
			}
			if (!int.TryParse(Columns, out int columns) || columns <= 0)
			{
				SetValidation("RoiArray.InvalidColumns");
				return;
			}
			if (!float.TryParse(RowSpacing, out float rowSpacing) || !float.IsFinite(rowSpacing))
			{
				SetValidation("RoiArray.InvalidRowGap");
				return;
			}
			if (!float.TryParse(ColumnSpacing, out float columnSpacing) || !float.IsFinite(columnSpacing))
			{
				SetValidation("RoiArray.InvalidColumnGap");
				return;
			}

			AcceptedData = new AddRoiArrayData { Rows = rows, Columns = columns, RowSpacing = rowSpacing, ColumnSpacing = columnSpacing };
			SetValidation(null);
			RequestClose?.Invoke(true);
		}

		private void OnCancel()
		{
			AcceptedData = null;
			SetValidation(null);
			RequestClose?.Invoke(false);
		}

		private void SetValidation(string key)
		{
			validationKey = key;
			RefreshLocalization();
		}

		internal void RefreshLocalization() => ValidationMessage = validationKey == null ? string.Empty : OpenVisionLanguageService.T(validationKey);
	}
}
