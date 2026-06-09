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
                return LogLevel.Normal;
            }

            return Enum.TryParse(levelName, true, out LogLevel level)
                ? level
                : LogLevel.Normal;
        }

        private static Brush GetBrush(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Abnormal:
                    return Brushes.IndianRed;
                case LogLevel.Debug:
                    return Brushes.LightSteelBlue;
                case LogLevel.Grab:
                case LogLevel.Vision:
                    return Brushes.GreenYellow;
                case LogLevel.Network:
                    return Brushes.LightSkyBlue;
                case LogLevel.DB:
                case LogLevel.Config:
                    return Brushes.Khaki;
                case LogLevel.Inspect:
                    return Brushes.LightGreen;
                default:
                    return Brushes.WhiteSmoke;
            }
        }
    }
}
