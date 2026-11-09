using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenVisionLab
{
    // Shared by the remaining Window topics and MatchingLearnView; keep cell geometry in one place.
    internal static class LearnCellVisuals
    {
        internal static Border CreateSmallValueCell(string text, int grayValue)
        {
            TextBlock textBlock = new()
            {
                Text = text,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Border cell = new()
            {
                Height = 30,
                Margin = new Thickness(0, 0, 5, 5),
                CornerRadius = new CornerRadius(3),
                BorderThickness = new Thickness(1),
                Background = CreateGrayBrush(grayValue),
                Child = textBlock
            };

            cell.SetResourceReference(Border.BorderBrushProperty, "OpenVision.Workbench.FieldBorderBrush");
            textBlock.Foreground = GetContrastBrush(grayValue);
            return cell;
        }

        internal static SolidColorBrush CreateGrayBrush(int value)
        {
            byte channel = (byte)Math.Max(0, Math.Min(255, value));
            return new SolidColorBrush(Color.FromRgb(channel, channel, channel));
        }

        internal static Brush GetContrastBrush(int grayValue)
        {
            byte channel = (byte)Math.Max(0, Math.Min(255, grayValue));
            return GetContrastBrush(Color.FromRgb(channel, channel, channel));
        }

        internal static Brush GetContrastBrush(Brush background) => GetContrastBrush(((SolidColorBrush)background).Color);

        internal static Brush GetContrastBrush(Color color)
        {
            static double Linearize(byte value)
            {
                double channel = value / 255D;
                return channel <= 0.04045D ? channel / 12.92D : Math.Pow((channel + 0.055D) / 1.055D, 2.4D);
            }

            double luminance = 0.2126D * Linearize(color.R) + 0.7152D * Linearize(color.G) + 0.0722D * Linearize(color.B);
            return (luminance + 0.05D) / 0.05D >= 1.05D / (luminance + 0.05D) ? Brushes.Black : Brushes.White;
        }
    }
}
