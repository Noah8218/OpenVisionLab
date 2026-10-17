using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace OpenVisionLab
{
    public sealed class RecipeManagerLibraryItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is not string recipe || values[1] is not IReadOnlyDictionary<string, string> details) { return string.Empty; }
            string text = details.TryGetValue(recipe, out string detail) ? detail : string.Empty;
            return values[2] is string main && string.Equals(main, recipe, StringComparison.OrdinalIgnoreCase)
                ? text + " / " + OpenVisionLanguageService.T("Workbench.Recipe.InUse") : text;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
