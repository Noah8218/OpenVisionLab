using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace OpenVisionLab.ImageCanvas
{
	public class ColorToRGBConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Color)
			{
				var color = (Color)(value);
				if (color.A == 0)
					return String.Format($"RGB({-1},{-1},{-1})");
				else
					return String.Format($"RGB({color.R},{color.G},{color.B})");
			}

			return "RGB(-1,-1, -1)";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException("Cannot convert back");
		}
	}
}
