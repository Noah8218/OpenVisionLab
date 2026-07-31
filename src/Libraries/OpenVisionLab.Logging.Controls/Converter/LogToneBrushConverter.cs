using OpenVisionLab.Logging;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OpenVisionLab.Logging.Controls.Converter
{
    public sealed class LogToneBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string levelName = value as string;
            LogLevel level = ParseLevel(levelName);
            return GetBrush(level);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        private static LogLevel ParseLevel(string levelName)
        {
            if (string.IsNullOrWhiteSpace(levelName))
            {
                return LogLevel.Info;
            }

            return Enum.TryParse(levelName, true, out LogLevel level)
                ? level
                : LogLevel.Info;
        }

        private static Brush GetBrush(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Error:
                    return Brushes.IndianRed;
                case LogLevel.Warning:
                    return Brushes.Khaki;
                case LogLevel.Debug:
                    return Brushes.LightSteelBlue;
                default:
                    return Brushes.WhiteSmoke;
            }
        }
    }
}


