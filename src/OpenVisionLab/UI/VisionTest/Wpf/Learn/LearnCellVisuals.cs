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
                BorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219)),
                BorderThickness = new Thickness(1),
                Background = CreateGrayBrush(grayValue),
                Child = textBlock
            };

            textBlock.Foreground = grayValue > 128 ? Brushes.Black : Brushes.White;
            return cell;
        }

        internal static SolidColorBrush CreateGrayBrush(int value)
        {
            byte channel = (byte)Math.Max(0, Math.Min(255, value));
            return new SolidColorBrush(Color.FromRgb(channel, channel, channel));
        }
    }
}
