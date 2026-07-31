using OpenVisionLab.Contracts;

namespace OpenVisionLab
{
    internal sealed class LineToolPresenter
    {
        private readonly ILineToolViewModel viewModel;

        public LineToolPresenter(ILineToolViewModel viewModel)
        {
            this.viewModel = viewModel ?? throw new System.ArgumentNullException(nameof(viewModel));
        }

        public LineGaugeProperty LineAProperty => viewModel.LineAProperty;

        public LineGaugeProperty LineBProperty => viewModel.LineBProperty;

        public LineGaugeProperty GetSelectedLineProperty(bool isLineBSelected)
        {
            return viewModel.GetSelectedLineProperty(isLineBSelected);
        }

        public LineGaugeProperty CreateSelectedLineProperty(bool isLineBSelected)
        {
            return viewModel.CreateSelectedLineProperty(isLineBSelected);
        }

        public LineGaugeProperty CreateLineAProperty()
        {
            return viewModel.CreateLineAProperty();
        }

        public LineGaugeProperty CreateLineBProperty()
        {
            return viewModel.CreateLineBProperty();
        }

        public string CreateSummary(LineToolPurpose purpose, bool isLineBSelected, string purposeText, string lineText)
        {
            return viewModel.CreateSummary(purpose, isLineBSelected, purposeText, lineText);
        }

        public string CreatePurposeHint(LineToolPurpose purpose)
        {
            return viewModel.CreatePurposeHint(purpose);
        }
    }
}
