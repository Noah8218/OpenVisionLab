using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    // Owns Recipe Manager Pipeline option projection without owning UI state or storage recovery.
    internal sealed class OpenVisionRecipePipelineOptionProjectionOwner
    {
        internal OpenVisionRecipePipelineOptionProjection Project(
            OpenVisionRecipePipelineOptionProjectionRequest request)
        {
            request = request ?? new OpenVisionRecipePipelineOptionProjectionRequest();
            IReadOnlyList<OpenVisionRecipePipelineOption> options =
                (request.PipelineNames ?? Array.Empty<string>())
                    .Select(name => OpenVisionRecipePipelineOption.Create(
                        request.RecipeName,
                        name,
                        request.ActivePipelineName))
                    .Where(option => option != null)
                    .OrderBy(option => option.IsActive ? 0 : 1)
                    .ThenBy(option => option.PipelineName, StringComparer.OrdinalIgnoreCase)
                    .ToList();

            string selectedName = request.NormalizedPreferredPipelineName;
            if (string.IsNullOrWhiteSpace(request.PreferredPipelineName)
                && !string.IsNullOrWhiteSpace(request.PreviousSelectedPipelineName)
                && options.Any(option => string.Equals(
                    option.PipelineName,
                    request.PreviousSelectedPipelineName,
                    StringComparison.OrdinalIgnoreCase)))
            {
                selectedName = request.PreviousSelectedPipelineName;
            }
            else if (string.IsNullOrWhiteSpace(request.PreferredPipelineName))
            {
                selectedName = request.ActivePipelineName;
            }

            OpenVisionRecipePipelineOption selectedOption = options.FirstOrDefault(option =>
                    string.Equals(
                        option.PipelineName,
                        selectedName,
                        StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault(option => option.IsActive)
                ?? options.FirstOrDefault();

            return new OpenVisionRecipePipelineOptionProjection(options, selectedOption);
        }

        internal IReadOnlyList<OpenVisionRecipePipelineOption> Filter(
            IReadOnlyList<OpenVisionRecipePipelineOption> options,
            string filterText)
        {
            string filter = (filterText ?? string.Empty).Trim();
            IEnumerable<OpenVisionRecipePipelineOption> source =
                options ?? Array.Empty<OpenVisionRecipePipelineOption>();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                source = source.Where(option =>
                    option != null
                    && ((option.PipelineName?.IndexOf(filter, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0
                        || (option.DisplayText?.IndexOf(filter, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0
                        || (option.DetailText?.IndexOf(filter, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0));
            }

            return source.ToList();
        }

        internal string ProjectListSummary(string listText, int total, int visible)
        {
            if (total <= 0)
            {
                return listText ?? string.Empty;
            }

            return visible == total
                ? string.Format(CultureInfo.CurrentCulture, "{0} ({1})", listText, total)
                : string.Format(CultureInfo.CurrentCulture, "{0} ({1}/{2})", listText, visible, total);
        }
    }

    internal sealed class OpenVisionRecipePipelineOptionProjectionRequest
    {
        internal string RecipeName { get; set; }

        internal IReadOnlyList<string> PipelineNames { get; set; }

        internal string ActivePipelineName { get; set; }

        internal string PreferredPipelineName { get; set; }

        internal string NormalizedPreferredPipelineName { get; set; }

        internal string PreviousSelectedPipelineName { get; set; }
    }

    internal sealed class OpenVisionRecipePipelineOptionProjection
    {
        internal OpenVisionRecipePipelineOptionProjection(
            IReadOnlyList<OpenVisionRecipePipelineOption> options,
            OpenVisionRecipePipelineOption selectedOption)
        {
            Options = options ?? Array.Empty<OpenVisionRecipePipelineOption>();
            SelectedOption = selectedOption;
        }

        internal IReadOnlyList<OpenVisionRecipePipelineOption> Options { get; }

        internal OpenVisionRecipePipelineOption SelectedOption { get; }
    }
}
