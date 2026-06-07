using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OpenVisionLab.ImageCanvas
{
	public class BooleanToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool boolValue)
			{
				return new SolidColorBrush((boolValue) ?
					(Color)ColorConverter.ConvertFromString("#F05B23") :
					Colors.White);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
