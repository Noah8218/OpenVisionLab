using OpenVisionLab.Services;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal static class VisionToolControlValueReader
    {
        // Keep coercion rules explicit so threshold, kernel, and arithmetic inputs do not leak behavior into each other.
        public static double ReadDouble(TextBox textBox, double fallback, double minimum, double maximum)
        {
            double value = fallback;
            string text = textBox?.Text;
            if (!string.IsNullOrWhiteSpace(text)
                && !double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                && !double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
            {
                value = fallback;
            }

            return Math.Max(minimum, Math.Min(maximum, value));
        }

        public static int ReadClampedInt(TextBox textBox, int fallback, int minimum, int maximum)
        {
            return (int)Math.Round(ReadDouble(textBox, fallback, minimum, maximum));
        }

        public static int ReadPositiveInt(TextBox textBox, int fallback)
        {
            if (TryReadInt(textBox, out int value))
            {
                return Math.Max(1, value);
            }

            return fallback;
        }

        public static int ReadInvariantInt(TextBox textBox, int fallback)
        {
            return int.TryParse(textBox?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                ? value
                : fallback;
        }

        public static int NormalizeThresholdBlockSize(int value)
        {
            return VisionToolParameterPolicy.NormalizeThresholdBlockSize(value);
        }

        public static int NormalizeOddKernelSize(int value)
        {
            return VisionToolParameterPolicy.NormalizeOddKernelSize(value);
        }

        private static bool TryReadInt(TextBox textBox, out int value)
        {
            value = 0;
            string text = textBox?.Text;
            return !string.IsNullOrWhiteSpace(text)
                && (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
                    || int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out value));
        }
    }
}
