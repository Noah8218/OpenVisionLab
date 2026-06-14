using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineAddStep : Form
    {
        private readonly HashSet<string> existingLayerNames;
        private readonly HashSet<string> existingStepNames;
        private readonly ToolTip toolTip = new ToolTip();
        private readonly Timer layerRefreshTimer = new Timer();
        private readonly Func<IEnumerable<string>> layerNameProvider;
        private readonly string insertPositionText;
        private readonly string expectedInputLayer;

        private ComboBox cbToolType;
        private ComboBox cbInputLayer;
        private Label insertPositionValue;
        private TextBox tbStepName;
        private TextBox tbOutputLayer;
        private Panel flowPreviewPanel;
        private Label inputCardTitle;
        private Label inputCardValue;
        private Label toolCardTitle;
        private Label toolCardValue;
        private Label outputCardTitle;
        private Label outputCardValue;
        private Label flowRelationLabel;
        private CheckBox chkAllowBranch;
        private Label validationLabel;
        private Button btnAdd;
        private Button btnCancel;

        private string lastSuggestedStepName;
        private string lastSuggestedOutputLayer;
        private bool userEditedStepName;
        private bool userEditedOutputLayer;
        private bool isUpdatingSuggestion;
        private bool isRefreshingLayers;
        private string layerSnapshot = string.Empty;
        private string latestLayerName = "Main";

        public string SelectedToolType { get; private set; }
        public string SelectedInputLayer { get; private set; }
        public string SelectedOutputLayer { get; private set; }
        public string SelectedStepName { get; private set; }

        public FormVisionPipelineAddStep()
            : this(
                  new[] { "Threshold", "Morphology", "Contour" },
                  new[] { "Main", "TextSymbol_Binary", "TextSymbol_Clean" },
                  "Threshold",
                  "Main",
                  "Pipeline_Output",
                  "01 Sample Step",
                  new[] { "01 Text Symbol Binary" },
                  () => new[] { "Main", "TextSymbol_Binary", "TextSymbol_Clean" },
                  "Designer preview",
                  "Main")
        {
        }

        public FormVisionPipelineAddStep(
            IEnumerable<string> toolTypes,
            IEnumerable<string> layerNames,
            string defaultToolType,
            string defaultInputLayer,
            string defaultOutputLayer,
            string defaultStepName,
            IEnumerable<string> stepNames,
            Func<IEnumerable<string>> layerNameProvider = null)
            : this(
                  toolTypes,
                  layerNames,
                  defaultToolType,
                  defaultInputLayer,
                  defaultOutputLayer,
                  defaultStepName,
                  stepNames,
                  layerNameProvider,
                  null,
                  null)
        {
        }

        public FormVisionPipelineAddStep(
            IEnumerable<string> toolTypes,
            IEnumerable<string> layerNames,
            string defaultToolType,
            string defaultInputLayer,
            string defaultOutputLayer,
            string defaultStepName,
            IEnumerable<string> stepNames,
            Func<IEnumerable<string>> layerNameProvider,
            string insertPositionText,
            string expectedInputLayer)
        {
            List<string> initialLayerNames = NormalizeLayerNames(layerNames).ToList();
            existingLayerNames = new HashSet<string>(initialLayerNames, StringComparer.OrdinalIgnoreCase);
            existingStepNames = new HashSet<string>(stepNames ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            this.layerNameProvider = layerNameProvider;
            this.insertPositionText = string.IsNullOrWhiteSpace(insertPositionText) ? "Append to pipeline" : insertPositionText.Trim();
            this.expectedInputLayer = string.IsNullOrWhiteSpace(expectedInputLayer) ? string.Empty : expectedInputLayer.Trim();

            InitializeComponent();
            insertPositionValue.Text = this.insertPositionText;
            VisionPipelineDialogStyle.Apply(this);
            VisionPipelineDialogStyle.StyleButton(btnAdd, primary: true);
            LoadToolTypes(toolTypes, defaultToolType);
            LoadLayers(initialLayerNames, defaultInputLayer);
            SetInitialSuggestions(defaultToolType, defaultOutputLayer, defaultStepName);
            ValidateInputs(showSuccess: false);

            layerRefreshTimer.Interval = 300;
            layerRefreshTimer.Tick += OnLayerRefreshTimerTick;
            Shown += OnFormShown;
            FormClosed += OnFormClosed;
        }

        private void LoadToolTypes(IEnumerable<string> toolTypes, string defaultToolType)
        {
            cbToolType.Items.Clear();
            foreach (string toolType in toolTypes ?? Enumerable.Empty<string>())
            {
                if (!string.IsNullOrWhiteSpace(toolType))
                {
                    cbToolType.Items.Add(toolType.Trim());
                }
            }

            SelectComboItem(cbToolType, defaultToolType);
            if (cbToolType.SelectedIndex < 0 && cbToolType.Items.Count > 0)
            {
                cbToolType.SelectedIndex = 0;
            }
        }

        private void LoadLayers(IEnumerable<string> layerNames, string defaultInputLayer)
        {
            List<string> normalizedLayerNames = NormalizeLayerNames(layerNames).ToList();
            if (normalizedLayerNames.Count == 0)
            {
                normalizedLayerNames.Add("Main");
            }

            RefreshExistingLayerNames(normalizedLayerNames);
            latestLayerName = normalizedLayerNames
                .LastOrDefault(layer => !string.Equals(layer, "Main", StringComparison.OrdinalIgnoreCase))
                ?? "Main";

            string previousSelection = cbInputLayer.SelectedItem?.ToString();
            string targetSelection = string.IsNullOrWhiteSpace(defaultInputLayer)
                ? previousSelection
                : defaultInputLayer.Trim();

            isRefreshingLayers = true;
            try
            {
                cbInputLayer.Items.Clear();
                foreach (string layerName in normalizedLayerNames)
                {
                    cbInputLayer.Items.Add(layerName);
                }

                SelectComboItem(cbInputLayer, targetSelection);
                if (cbInputLayer.SelectedIndex < 0)
                {
                    SelectComboItem(cbInputLayer, "Main");
                }

                if (cbInputLayer.SelectedIndex < 0 && cbInputLayer.Items.Count > 0)
                {
                    cbInputLayer.SelectedIndex = 0;
                }
            }
            finally
            {
                isRefreshingLayers = false;
            }

            layerSnapshot = BuildLayerSnapshot(normalizedLayerNames);
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            RefreshLayersFromProvider();
            layerRefreshTimer.Start();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            layerRefreshTimer.Stop();
            layerRefreshTimer.Tick -= OnLayerRefreshTimerTick;
            layerRefreshTimer.Dispose();
            Shown -= OnFormShown;
            FormClosed -= OnFormClosed;
            toolTip.Dispose();
        }

        private void OnLayerRefreshTimerTick(object sender, EventArgs e)
        {
            RefreshLayersFromProvider();
        }

        private void RefreshLayersFromProvider()
        {
            if (layerNameProvider == null || cbInputLayer == null || isRefreshingLayers)
            {
                return;
            }

            List<string> currentLayers = NormalizeLayerNames(layerNameProvider()).ToList();
            string currentSnapshot = BuildLayerSnapshot(currentLayers);
            if (string.Equals(layerSnapshot, currentSnapshot, StringComparison.Ordinal))
            {
                return;
            }

            LoadLayers(currentLayers, cbInputLayer.SelectedItem?.ToString());
            ValidateInputs(showSuccess: false);
        }

        private void RefreshExistingLayerNames(IEnumerable<string> layerNames)
        {
            existingLayerNames.Clear();
            foreach (string layerName in NormalizeLayerNames(layerNames))
            {
                existingLayerNames.Add(layerName);
            }
        }

        private static IEnumerable<string> NormalizeLayerNames(IEnumerable<string> layerNames)
        {
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string layerName in layerNames ?? Enumerable.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(layerName))
                {
                    continue;
                }

                string normalized = layerName.Trim();
                if (seen.Add(normalized))
                {
                    yield return normalized;
                }
            }

            if (seen.Add("Main"))
            {
                yield return "Main";
            }
        }

        private static string BuildLayerSnapshot(IEnumerable<string> layerNames)
        {
            return string.Join("|", NormalizeLayerNames(layerNames));
        }

        private void SetInitialSuggestions(string defaultToolType, string defaultOutputLayer, string defaultStepName)
        {
            isUpdatingSuggestion = true;
            try
            {
                lastSuggestedStepName = string.IsNullOrWhiteSpace(defaultStepName)
                    ? CreateUniqueStepName(SelectedToolText())
                    : defaultStepName.Trim();
                lastSuggestedOutputLayer = string.IsNullOrWhiteSpace(defaultOutputLayer)
                    ? CreateUniqueLayerName(CreateSuggestedLayerName(SelectedToolText(), SelectedInputText()))
                    : defaultOutputLayer.Trim();

                tbStepName.Text = lastSuggestedStepName;
                tbOutputLayer.Text = lastSuggestedOutputLayer;
            }
            finally
            {
                isUpdatingSuggestion = false;
            }
        }

        private static void SelectComboItem(ComboBox comboBox, string value)
        {
            if (comboBox == null || string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (string.Equals(comboBox.Items[i]?.ToString(), value, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }
        }

        private void OnToolTypeChanged(object sender, EventArgs e)
        {
            RefreshSuggestionsForToolChange();
            ValidateInputs(showSuccess: false);
        }

        private void OnInputLayerChanged(object sender, EventArgs e)
        {
            if (isRefreshingLayers)
            {
                return;
            }

            RefreshSuggestionsForInputChange();
            ValidateInputs(showSuccess: false);
        }

        private void OnStepNameTextChanged(object sender, EventArgs e)
        {
            if (!isUpdatingSuggestion)
            {
                userEditedStepName = !string.Equals(tbStepName.Text, lastSuggestedStepName, StringComparison.Ordinal);
            }

            ValidateInputs(showSuccess: false);
        }

        private void OnOutputLayerTextChanged(object sender, EventArgs e)
        {
            if (!isUpdatingSuggestion)
            {
                userEditedOutputLayer = !string.Equals(tbOutputLayer.Text, lastSuggestedOutputLayer, StringComparison.Ordinal);
            }

            ValidateInputs(showSuccess: false);
        }

        private void OnAllowBranchChanged(object sender, EventArgs e)
        {
            ValidateInputs(showSuccess: false);
        }

        private void RefreshSuggestionsForToolChange()
        {
            string toolType = SelectedToolText();
            if (string.IsNullOrWhiteSpace(toolType))
            {
                return;
            }

            isUpdatingSuggestion = true;
            try
            {
                if (!userEditedStepName)
                {
                    lastSuggestedStepName = CreateUniqueStepName(toolType);
                    tbStepName.Text = lastSuggestedStepName;
                }

                if (!userEditedOutputLayer)
                {
                    lastSuggestedOutputLayer = CreateUniqueLayerName(CreateSuggestedLayerName(toolType, SelectedInputText()));
                    tbOutputLayer.Text = lastSuggestedOutputLayer;
                }
            }
            finally
            {
                isUpdatingSuggestion = false;
            }
        }

        private void RefreshSuggestionsForInputChange()
        {
            if (userEditedOutputLayer)
            {
                return;
            }

            string toolType = SelectedToolText();
            if (string.IsNullOrWhiteSpace(toolType))
            {
                return;
            }

            isUpdatingSuggestion = true;
            try
            {
                lastSuggestedOutputLayer = CreateUniqueLayerName(CreateSuggestedLayerName(toolType, SelectedInputText()));
                tbOutputLayer.Text = lastSuggestedOutputLayer;
            }
            finally
            {
                isUpdatingSuggestion = false;
            }
        }

        private void OnAddClicked(object sender, EventArgs e)
        {
            if (!ValidateInputs(showSuccess: true))
            {
                return;
            }

            SelectedToolType = SelectedToolText();
            SelectedInputLayer = SelectedInputText();
            SelectedOutputLayer = tbOutputLayer.Text.Trim();
            SelectedStepName = tbStepName.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool ValidateInputs(bool showSuccess)
        {
            string message = ResolveValidationMessage();
            bool isValid = string.IsNullOrWhiteSpace(message);
            UpdateAddButtonState(isValid);
            UpdateBranchConfirmationState();
            validationLabel.ForeColor = isValid
                ? Color.FromArgb(35, 85, 132)
                : Color.FromArgb(168, 55, 55);
            validationLabel.Text = isValid
                ? BuildReadyStateText(showSuccess)
                : message;
            UpdateFlowPreview(isValid, message);
            return isValid;
        }

        private void UpdateAddButtonState(bool isValid)
        {
            if (btnAdd == null)
            {
                return;
            }

            btnAdd.Enabled = isValid;
            btnAdd.Cursor = isValid ? Cursors.Hand : Cursors.Default;
            btnAdd.BackColor = isValid
                ? Color.FromArgb(35, 85, 132)
                : Color.FromArgb(224, 230, 236);
            btnAdd.ForeColor = isValid
                ? Color.White
                : Color.FromArgb(112, 122, 134);
            btnAdd.FlatAppearance.BorderColor = isValid
                ? Color.FromArgb(35, 85, 132)
                : Color.FromArgb(174, 184, 195);
            btnAdd.FlatAppearance.MouseOverBackColor = isValid
                ? Color.FromArgb(47, 111, 171)
                : Color.FromArgb(224, 230, 236);
            btnAdd.FlatAppearance.MouseDownBackColor = isValid
                ? Color.FromArgb(31, 73, 116)
                : Color.FromArgb(224, 230, 236);
        }

        private string ResolveValidationMessage()
        {
            string toolType = SelectedToolText();
            string inputLayer = SelectedInputText();
            string outputLayer = tbOutputLayer.Text.Trim();
            string stepName = tbStepName.Text.Trim();

            if (string.IsNullOrWhiteSpace(toolType))
            {
                return "Select a tool.";
            }

            if (string.IsNullOrWhiteSpace(stepName))
            {
                return "Enter a step name.";
            }

            if (existingStepNames.Contains(stepName))
            {
                return $"Step name '{stepName}' already exists.";
            }

            if (string.IsNullOrWhiteSpace(inputLayer))
            {
                return "Select an input layer.";
            }

            if (string.IsNullOrWhiteSpace(outputLayer))
            {
                return "Enter an output layer.";
            }

            if (string.Equals(outputLayer, inputLayer, StringComparison.OrdinalIgnoreCase))
            {
                return "Output layer must be different from input layer.";
            }

            if (string.Equals(outputLayer, "Main", StringComparison.OrdinalIgnoreCase))
            {
                return "Output layer cannot be Main.";
            }

            if (existingLayerNames.Contains(outputLayer))
            {
                return $"Output layer '{outputLayer}' already exists.";
            }

            if (RequiresBranchConfirmation(inputLayer) && !chkAllowBranch.Checked)
            {
                return $"Branch input selected. Turn on 'Allow branch input' to read '{inputLayer}' instead of previous output '{expectedInputLayer}'.";
            }

            return string.Empty;
        }

        private string BuildReadyStateText(bool showSuccess)
        {
            string inputLayer = SelectedInputText();
            string outputLayer = tbOutputLayer.Text.Trim();
            string prefix = showSuccess ? "Ready. " : string.Empty;

            if (IsSourceInput(inputLayer))
            {
                return $"{prefix}Source flow: starts from '{inputLayer}' and creates '{outputLayer}'.";
            }

            if (RequiresBranchConfirmation(inputLayer))
            {
                return $"{prefix}Branch flow: reads '{inputLayer}' instead of previous output '{expectedInputLayer}'.";
            }

            string linkedLayer = string.IsNullOrWhiteSpace(expectedInputLayer)
                ? inputLayer
                : expectedInputLayer;
            return $"{prefix}Default link: previous output '{linkedLayer}' becomes this step input.";
        }

        private void UpdateBranchConfirmationState()
        {
            if (chkAllowBranch == null)
            {
                return;
            }

            bool requiresBranch = RequiresBranchConfirmation(SelectedInputText());
            chkAllowBranch.Visible = requiresBranch;
            chkAllowBranch.Enabled = requiresBranch;
            if (statusPanel?.ColumnStyles?.Count > 0)
            {
                statusPanel.ColumnStyles[0].Width = requiresBranch ? 185F : 0F;
            }

            if (!requiresBranch && chkAllowBranch.Checked)
            {
                chkAllowBranch.Checked = false;
            }
        }

        private void UpdateFlowPreview(bool isValid, string validationMessage)
        {
            string toolType = string.IsNullOrWhiteSpace(SelectedToolText()) ? "Tool" : SelectedToolText();
            string inputLayer = string.IsNullOrWhiteSpace(SelectedInputText()) ? "Input?" : SelectedInputText();
            string outputLayer = string.IsNullOrWhiteSpace(tbOutputLayer.Text) ? "Output?" : tbOutputLayer.Text.Trim();
            if (inputCardValue == null || toolCardValue == null || outputCardValue == null || flowRelationLabel == null)
            {
                return;
            }

            inputCardValue.Text = inputLayer;
            toolCardValue.Text = toolType;
            outputCardValue.Text = outputLayer;

            bool isLinked = IsLatestLayer(inputLayer);
            bool isExpected = IsExpectedInput(inputLayer);
            bool isSource = IsSourceInput(inputLayer);
            bool branchPendingOnly = !isValid
                && !string.IsNullOrWhiteSpace(validationMessage)
                && validationMessage.StartsWith("Branch input selected.", StringComparison.Ordinal);
            bool useRelationCardStyle = isValid || branchPendingOnly;
            flowRelationLabel.Text = BuildFlowRelationText(inputLayer, outputLayer, isLinked || isExpected, isSource);
            flowRelationLabel.ForeColor = !isValid
                ? branchPendingOnly ? Color.FromArgb(168, 92, 0) : Color.FromArgb(168, 55, 55)
                : isLinked || isExpected
                    ? Color.FromArgb(18, 116, 76)
                    : isSource
                        ? Color.FromArgb(35, 85, 132)
                        : Color.FromArgb(168, 92, 0);

            ApplyCardStyle(inputCardValue.Parent as Panel, useRelationCardStyle, isLinked || isExpected, isSource, isOutput: false);
            ApplyCardStyle(outputCardValue.Parent as Panel, useRelationCardStyle, linked: true, source: false, isOutput: true);
        }

        private bool IsLatestLayer(string inputLayer)
        {
            return !string.IsNullOrWhiteSpace(inputLayer)
                && !string.Equals(inputLayer, "Main", StringComparison.OrdinalIgnoreCase)
                && string.Equals(inputLayer.Trim(), latestLayerName, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsExpectedInput(string inputLayer)
        {
            return string.IsNullOrWhiteSpace(expectedInputLayer)
                || (!string.IsNullOrWhiteSpace(inputLayer)
                    && string.Equals(inputLayer.Trim(), expectedInputLayer, StringComparison.OrdinalIgnoreCase));
        }

        private bool RequiresBranchConfirmation(string inputLayer)
        {
            return !string.IsNullOrWhiteSpace(expectedInputLayer)
                && !string.IsNullOrWhiteSpace(inputLayer)
                && !string.Equals(inputLayer.Trim(), expectedInputLayer, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsSourceInput(string inputLayer)
        {
            return string.Equals(inputLayer, "Main", StringComparison.OrdinalIgnoreCase)
                && (string.IsNullOrWhiteSpace(expectedInputLayer)
                    || string.Equals(expectedInputLayer, "Main", StringComparison.OrdinalIgnoreCase));
        }

        private static string BuildFlowRelationText(string inputLayer, string outputLayer, bool isLinked, bool isSource)
        {
            if (isSource)
            {
                return $"Source flow: Main -> {outputLayer}.";
            }

            if (isLinked)
            {
                return $"Default link: previous output '{inputLayer}' -> {outputLayer}.";
            }

            return $"Branch flow: selected input '{inputLayer}' bypasses previous output.";
        }

        private static void ApplyCardStyle(Panel card, bool isValid, bool linked, bool source, bool isOutput)
        {
            if (card == null)
            {
                return;
            }

            if (!isValid)
            {
                card.BackColor = Color.FromArgb(252, 245, 245);
                return;
            }

            if (isOutput)
            {
                card.BackColor = Color.FromArgb(231, 240, 255);
            }
            else if (linked)
            {
                card.BackColor = Color.FromArgb(226, 247, 236);
            }
            else if (source)
            {
                card.BackColor = Color.FromArgb(231, 240, 255);
            }
            else
            {
                card.BackColor = Color.FromArgb(255, 244, 224);
            }
        }

        private string SelectedToolText()
        {
            return cbToolType.SelectedItem?.ToString()?.Trim() ?? string.Empty;
        }

        private string SelectedInputText()
        {
            return cbInputLayer.SelectedItem?.ToString()?.Trim() ?? string.Empty;
        }

        private string CreateUniqueStepName(string toolType)
        {
            string baseName = string.IsNullOrWhiteSpace(toolType) ? "Step" : toolType.Trim();
            if (!existingStepNames.Contains(baseName))
            {
                return baseName;
            }

            for (int index = 2; index < 10000; index++)
            {
                string candidate = $"{baseName}_{index}";
                if (!existingStepNames.Contains(candidate))
                {
                    return candidate;
                }
            }

            return $"{baseName}_{DateTime.Now:HHmmss}";
        }

        private string CreateUniqueLayerName(string baseName)
        {
            string name = string.IsNullOrWhiteSpace(baseName) ? "Pipeline_Output" : baseName.Trim();
            if (!existingLayerNames.Contains(name))
            {
                return name;
            }

            for (int index = 2; index < 10000; index++)
            {
                string candidate = $"{name}_{index}";
                if (!existingLayerNames.Contains(candidate))
                {
                    return candidate;
                }
            }

            return $"{name}_{DateTime.Now:HHmmss}";
        }

        private static string CreateSuggestedLayerName(string toolType, string inputLayer)
        {
            string toolToken = SanitizeLayerToken(toolType, "Step");
            string inputToken = SanitizeLayerToken(inputLayer, "Input");
            if (string.Equals(inputToken, "Main", StringComparison.OrdinalIgnoreCase)
                || string.Equals(inputToken, "Input", StringComparison.OrdinalIgnoreCase))
            {
                return $"{toolToken}_Output";
            }

            return $"{inputToken}_{toolToken}";
        }

        private static string SanitizeLayerToken(string value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            char[] chars = value.Trim()
                .Select(ch => char.IsLetterOrDigit(ch) ? ch : '_')
                .ToArray();
            string token = new string(chars).Trim('_');
            while (token.Contains("__", StringComparison.Ordinal))
            {
                token = token.Replace("__", "_");
            }

            return string.IsNullOrWhiteSpace(token) ? fallback : token;
        }
    }
}
