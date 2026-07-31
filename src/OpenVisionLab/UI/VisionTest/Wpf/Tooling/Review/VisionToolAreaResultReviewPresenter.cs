using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class VisionToolAreaResultReviewPresenter
    {
        public static void Show<TResult>(
            Action<string, bool, IEnumerable<VisionToolResultReviewItem>> showResultReview,
            string title,
            string emptyState,
            IEnumerable<TResult> results,
            Func<TResult, double> getArea,
            Func<TResult, double> getCenterX,
            Func<TResult, double> getCenterY,
            Func<TResult, double> getBoxWidth,
            Func<TResult, double> getBoxHeight)
            where TResult : class
        {
            if (showResultReview == null)
            {
                throw new ArgumentNullException(nameof(showResultReview));
            }

            string resolvedTitle = string.IsNullOrWhiteSpace(title) ? "Area result" : title.Trim();
            string resolvedEmptyState = string.IsNullOrWhiteSpace(emptyState) ? VisionToolVerificationText.NoResult : emptyState.Trim();
            List<TResult> items = results?.Where(item => item != null).ToList() ?? new List<TResult>();
            if (items.Count == 0)
            {
                showResultReview(
                    VisionToolVerificationText.FormatAreaEmptySummary(resolvedTitle, resolvedEmptyState),
                    false,
                    new[]
                    {
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CountLabel, 0),
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.StateLabel, resolvedEmptyState)
                    });
                return;
            }

            TResult best = items.OrderByDescending(getArea).First();
            double area = getArea(best);
            double centerX = getCenterX(best);
            double centerY = getCenterY(best);
            double boxWidth = getBoxWidth(best);
            double boxHeight = getBoxHeight(best);
            string summary = VisionToolVerificationText.FormatAreaSummary(
                resolvedTitle,
                items.Count,
                area,
                centerX,
                centerY,
                boxWidth,
                boxHeight);
            showResultReview(
                summary,
                true,
                new[]
                {
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CountLabel, items.Count),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.MaxAreaLabel, string.Format(CultureInfo.CurrentCulture, "{0:0.#}", area)),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CenterLabel, VisionToolResultReviewPresenter.FormatPoint(centerX, centerY)),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.BoxLabel, VisionToolResultReviewPresenter.FormatSize(boxWidth, boxHeight))
                });
        }
    }
}
