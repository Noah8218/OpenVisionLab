namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeToolStatusPresenter
    {
        private readonly ISingleInputVisionToolWpfView view;
        private readonly IArithmeticVisionToolWpfView arithmeticView;

        public OpenVisionNativeToolStatusPresenter(
            ISingleInputVisionToolWpfView view,
            IArithmeticVisionToolWpfView arithmeticView)
        {
            this.view = view;
            this.arithmeticView = arithmeticView;
        }

        public void Present(string status)
        {
            switch (view)
            {
                case BlobToolWpfView blobView:
                    blobView.SetStatus(status);
                    break;
                case ContourToolWpfView contourView:
                    contourView.SetStatus(status);
                    break;
                case LineToolWpfView lineView:
                    lineView.SetStatus(status);
                    break;
                case MatchingToolWpfView matchingView:
                    matchingView.SetStatus(status);
                    break;
                case EdgeBasedMatchingToolWpfView edgeBasedMatchingView:
                    edgeBasedMatchingView.SetStatus(status);
                    break;
                case FeatureMatchingToolWpfView featureMatchingView:
                    featureMatchingView.SetStatus(status);
                    break;
                case FilterToolWpfView filterView:
                    filterView.SetStatus(status);
                    break;
                case ThresholdToolWpfView thresholdView:
                    thresholdView.SetStatus(status);
                    break;
                case MorphologyToolWpfView morphologyView:
                    morphologyView.SetStatus(status);
                    break;
                case SimplePreprocessToolWpfView simpleView:
                    simpleView.SetStatus(status);
                    break;
                case null when arithmeticView != null:
                    arithmeticView.SetStatus(status);
                    break;
            }
        }
    }
}
