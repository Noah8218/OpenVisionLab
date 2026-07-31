using System;
using System.Globalization;
using System.Windows.Data;

namespace OpenVisionLab.ImageCanvas
{
	public class EqualityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value?.Equals(parameter) ?? false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value.Equals(true) ? parameter : Binding.DoNothing;
		}
	}

}
