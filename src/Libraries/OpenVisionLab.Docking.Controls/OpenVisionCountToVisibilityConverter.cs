using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionCountToVisibilityConverter : IValueConverter
    {
        public int MinimumVisibleCount { get; set; } = 1;

        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int minimum = MinimumVisibleCount;
            if (parameter != null && int.TryParse(parameter.ToString(), out int parsedMinimum))
            {
                minimum = parsedMinimum;
            }

            int count = value is int intValue ? intValue : 0;
            bool isVisible = count >= minimum;
            if (Invert)
            {
                isVisible = !isVisible;
            }

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
