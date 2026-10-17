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
				ValidationMessage = LocalText("행 수는 1 이상의 정수로 입력하십시오.", "Rows must be a positive whole number.");
				return;
			}
			if (!int.TryParse(Columns, out int columns) || columns <= 0)
			{
				ValidationMessage = LocalText("열 수는 1 이상의 정수로 입력하십시오.", "Columns must be a positive whole number.");
				return;
			}
			if (!float.TryParse(RowSpacing, out float rowSpacing) || !float.IsFinite(rowSpacing))
			{
				ValidationMessage = LocalText("행 간격(mm)은 유한한 숫자로 입력하십시오.", "Row gap (mm) must be a finite number.");
				return;
			}
			if (!float.TryParse(ColumnSpacing, out float columnSpacing) || !float.IsFinite(columnSpacing))
			{
				ValidationMessage = LocalText("열 간격(mm)은 유한한 숫자로 입력하십시오.", "Column gap (mm) must be a finite number.");
				return;
			}

			AcceptedData = new AddRoiArrayData { Rows = rows, Columns = columns, RowSpacing = rowSpacing, ColumnSpacing = columnSpacing };
			ValidationMessage = string.Empty;
			RequestClose?.Invoke(true);
		}

		private void OnCancel()
		{
			AcceptedData = null;
			ValidationMessage = string.Empty;
			RequestClose?.Invoke(false);
		}

		private static string LocalText(string korean, string english) =>
			OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English ? english : korean;
	}
}
