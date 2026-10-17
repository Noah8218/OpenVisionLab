using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    // A session image owns these copies until replacement, removal or Main shutdown.
    internal sealed class OpenVisionWorkbenchImageResult : IDisposable
    {
        private readonly Dictionary<int, Bitmap> outputs;
        private bool disposed;

        internal OpenVisionWorkbenchImageResult(string context, long revision, VisionPipeline definition,
            Dictionary<int, VisionPipelineStepResultSummary> summaries, Dictionary<int, Bitmap> outputs,
            string verdict, double elapsedMilliseconds, string source, string loadErrorImageName = "")
        {
            Context = context;
            RecipeRevision = revision;
            Definition = definition;
            Summaries = summaries;
            this.outputs = outputs;
            Verdict = verdict;
            ElapsedMilliseconds = elapsedMilliseconds;
            Source = source;
            LoadErrorImageName = loadErrorImageName;
        }

        internal string Context { get; }
        internal long RecipeRevision { get; }
        internal VisionPipeline Definition { get; }
        internal IReadOnlyDictionary<int, VisionPipelineStepResultSummary> Summaries { get; }
        internal string Verdict { get; }
        internal double ElapsedMilliseconds { get; }
        internal string Source { get; }
        internal string LoadErrorImageName { get; }
        internal string Error => string.IsNullOrEmpty(LoadErrorImageName) ? "" : string.Format(CultureInfo.CurrentCulture, OpenVisionLanguageService.T("Workbench.Batch.LoadError"), LoadErrorImageName);
        internal bool IsStale { get; set; }
        internal string FailedStep => !string.IsNullOrEmpty(Error) ? Error : Summaries.OrderBy(item => item.Key)
            .Where(item => item.Value.Executed && !item.Value.Skipped && (!item.Value.Success || item.Value.IsAcceptanceNg))
            .Select(item => OpenVisionWorkbenchStepViewModel.DisplayName(item.Value.Name)).FirstOrDefault() ?? "";
        internal string KeyValues => string.Join(" · ", Definition.Steps.Select((step, index) => new { step, index })
            .Where(item => item.step.UseAcceptance).Select(item => OpenVisionWorkbenchStepViewModel.DisplayName(item.step.Name) + " "
                + (Summaries.TryGetValue(item.index, out VisionPipelineStepResultSummary result)
                    && result.Metrics.TryGetValue(item.step.AcceptanceMetricName ?? "", out double value)
                        ? OpenVisionPipelineReviewResultPresenter.WorkbenchMetricValue(item.step.AcceptanceMetricName, value) : "—")));
        internal Bitmap CloneOutput(int index) => !disposed && outputs.TryGetValue(index, out Bitmap image) ? new Bitmap(image) : null;

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;
            foreach (Bitmap image in outputs.Values) { image.Dispose(); }
            outputs.Clear();
        }
    }
}
