using System;
using System.Globalization;
using System.Windows.Data;

namespace OpenVisionLab.ImageCanvas
{
	public class DoubleToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// Double 값을 문자열로 변환
			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// 문자열을 Double로 변환
			double result;
			if (double.TryParse((string)value, out result))
			{
				return result;
			}
			return 0;
		}
	}
}
