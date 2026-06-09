using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OpenVisionLab.ImageCanvas
{
	public class ColorToRGBConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is System.Drawing.Color)
			{
				var color = (System.Drawing.Color)(value);
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

	public class DrawingColorToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is System.Drawing.Color color && color.A != 0)
			{
				return new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
			}

			return new SolidColorBrush(Colors.Transparent);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException("Cannot convert back");
		}
	}
}
