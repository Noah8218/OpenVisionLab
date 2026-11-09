using System;
using System.Globalization;
using System.Windows.Data;

namespace OpenVisionLab
{
    public sealed class OpenVisionPipelineReviewDetailTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string ?? string.Empty;
            if (parameter as string == "GeometryQuality")
            {
                return text.Replace("support ", OpenVisionLanguageService.T("PipelineReview.GeometryReview.SupportPrefix") + " ")
                    .Replace(", residual ", OpenVisionLanguageService.T("PipelineReview.GeometryReview.ResidualPrefix") + " ");
            }

            return OpenVisionLanguageService.TryT("PipelineReview.CircleEvidence.State." + text.Replace(" ", string.Empty), out string localized) ? localized : text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
