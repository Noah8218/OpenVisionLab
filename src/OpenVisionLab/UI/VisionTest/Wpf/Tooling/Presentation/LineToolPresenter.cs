using OpenVisionLab.Contracts;
using System;

namespace OpenVisionLab
{
    internal sealed class LineToolPresenter
    {
        private readonly ILineToolViewModel viewModel;
        private readonly Action persistProperties;

        public LineToolPresenter(ILineToolViewModel viewModel, Action persistProperties)
        {
            this.viewModel = viewModel ?? throw new System.ArgumentNullException(nameof(viewModel));
            this.persistProperties = persistProperties ?? throw new ArgumentNullException(nameof(persistProperties));
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

        public void PersistProperties()
        {
            persistProperties();
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
