using OpenVisionLab._1._Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeLayerRouteController
    {
        private readonly IDisplayManager displayManager;
        private readonly string defaultOutputLayer;
        private readonly Func<string> getSelectedInputLayer;
        private readonly Func<string> getSelectedOutputLayer;
        private readonly Func<string> getSelectedInputLayerA;
        private readonly Func<string> getSelectedInputLayerB;
        private readonly Func<string> getSelectedArithmeticOutputLayer;
        private readonly HashSet<string> preparedOutputLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> preparedInputLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private string selectedInputLayer;
        private string selectedOutputLayer;
        private string selectedArithmeticInputLayerA;
        private string selectedArithmeticInputLayerB;
        private string selectedArithmeticOutputLayer;
        private bool selectedArithmeticInputLayerBExplicit;

        public OpenVisionNativeLayerRouteController(
            IDisplayManager displayManager,
            string defaultOutputLayer,
            Func<string> getSelectedInputLayer = null,
            Func<string> getSelectedOutputLayer = null,
            Func<string> getSelectedInputLayerA = null,
            Func<string> getSelectedInputLayerB = null,
            Func<string> getSelectedArithmeticOutputLayer = null)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.defaultOutputLayer = string.IsNullOrWhiteSpace(defaultOutputLayer) ? "Preview" : defaultOutputLayer;
            this.getSelectedInputLayer = getSelectedInputLayer ?? (() => string.Empty);
            this.getSelectedOutputLayer = getSelectedOutputLayer ?? (() => string.Empty);
            this.getSelectedInputLayerA = getSelectedInputLayerA ?? (() => string.Empty);
            this.getSelectedInputLayerB = getSelectedInputLayerB ?? (() => string.Empty);
            this.getSelectedArithmeticOutputLayer = getSelectedArithmeticOutputLayer ?? (() => string.Empty);

            selectedInputLayer = "Main";
            selectedOutputLayer = this.defaultOutputLayer;
            selectedArithmeticInputLayerA = "Main";
            selectedArithmeticInputLayerB = "Main";
            selectedArithmeticOutputLayer = this.defaultOutputLayer;
        }

        public string ResolveInputLayer()
        {
            return ResolveInputLayerForOutput(ResolveOutputLayer());
        }

        public string ResolveInputLayerForOutput(string outputLayer)
        {
            outputLayer = string.IsNullOrWhiteSpace(outputLayer) ? ResolveOutputLayer() : outputLayer;
            string selected = getSelectedInputLayer();
            if (IsInputLayerCandidate(selected, outputLayer))
            {
                selectedInputLayer = selected;
                return selected;
            }

            selected = selectedInputLayer;
            if (IsInputLayerCandidate(selected, outputLayer))
            {
                return selected;
            }

            if (IsInputLayerCandidate("Main", outputLayer))
            {
                selectedInputLayer = "Main";
                return selectedInputLayer;
            }

            if (IsInputLayerCandidate(displayManager.SelectedItem, outputLayer))
            {
                selectedInputLayer = displayManager.SelectedItem;
                return selectedInputLayer;
            }

            selectedInputLayer = GetFirstInputLayerName(outputLayer);
            return selectedInputLayer;
        }

        public string ResolveOutputLayer()
        {
            string selected = getSelectedOutputLayer();
            if (!string.IsNullOrWhiteSpace(selected))
            {
                selectedOutputLayer = selected;
                return selected;
            }

            selected = selectedOutputLayer;
            if (!string.IsNullOrWhiteSpace(selected))
            {
                return selected;
            }

            selectedOutputLayer = defaultOutputLayer;
            return selectedOutputLayer;
        }

        public string ResolveArithmeticInputLayerA()
        {
            return ResolveArithmeticInputLayerAForOutput(ResolveArithmeticOutputLayer());
        }

        public string ResolveArithmeticInputLayerAForOutput(string outputLayer)
        {
            outputLayer = string.IsNullOrWhiteSpace(outputLayer) ? ResolveArithmeticOutputLayer() : outputLayer;
            string selected = getSelectedInputLayerA();
            if (IsInputLayerCandidate(selected, outputLayer))
            {
                selectedArithmeticInputLayerA = selected;
                return selected;
            }

            selected = selectedArithmeticInputLayerA;
            if (IsInputLayerCandidate(selected, outputLayer))
            {
                return selected;
            }

            if (IsInputLayerCandidate("Main", outputLayer))
            {
                selectedArithmeticInputLayerA = "Main";
                return selectedArithmeticInputLayerA;
            }

            selectedArithmeticInputLayerA = GetFirstInputLayerName(outputLayer);
            return selectedArithmeticInputLayerA;
        }

        public string ResolveArithmeticInputLayerB()
        {
            return ResolveArithmeticInputLayerBForOutput(ResolveArithmeticOutputLayer());
        }

        public string ResolveArithmeticInputLayerBForOutput(string outputLayer)
        {
            outputLayer = string.IsNullOrWhiteSpace(outputLayer) ? ResolveArithmeticOutputLayer() : outputLayer;
            string inputA = ResolveArithmeticInputLayerAForOutput(outputLayer);

            if (selectedArithmeticInputLayerBExplicit
                && IsInputLayerCandidate(selectedArithmeticInputLayerB, outputLayer))
            {
                // Programmatic B-image load prepares a new layer before the ComboBox can refresh.
                // Keep that explicit route instead of reading the stale visible selection back as B.
                return selectedArithmeticInputLayerB;
            }

            string selected = getSelectedInputLayerB();
            if (IsInputLayerCandidate(selected, outputLayer))
            {
                if (!selectedArithmeticInputLayerBExplicit
                    && IsSameLayer(selected, inputA)
                    && TryGetFirstInputLayerNameExcept(outputLayer, inputA, out string alternative))
                {
                    selectedArithmeticInputLayerB = alternative;
                    return selectedArithmeticInputLayerB;
                }

                selectedArithmeticInputLayerB = selected;
                return selected;
            }

            selected = selectedArithmeticInputLayerB;
            if (IsInputLayerCandidate(selected, outputLayer))
            {
                if (!selectedArithmeticInputLayerBExplicit
                    && IsSameLayer(selected, inputA)
                    && TryGetFirstInputLayerNameExcept(outputLayer, inputA, out string alternative))
                {
                    selectedArithmeticInputLayerB = alternative;
                    return selectedArithmeticInputLayerB;
                }

                return selected;
            }

            selectedArithmeticInputLayerBExplicit = false;
            // Arithmetic needs two independently testable inputs. If the operator has not explicitly chosen B,
            // prefer another available layer over silently reusing Input A.
            if (TryGetFirstInputLayerNameExcept(outputLayer, inputA, out string fallback))
            {
                selectedArithmeticInputLayerB = fallback;
                return selectedArithmeticInputLayerB;
            }

            if (IsInputLayerCandidate(inputA, outputLayer))
            {
                selectedArithmeticInputLayerB = inputA;
                return selectedArithmeticInputLayerB;
            }

            selectedArithmeticInputLayerB = GetFirstInputLayerName(outputLayer);
            return selectedArithmeticInputLayerB;
        }

        public string ResolveArithmeticOutputLayer()
        {
            string selected = getSelectedArithmeticOutputLayer();
            if (!string.IsNullOrWhiteSpace(selected))
            {
                selectedArithmeticOutputLayer = selected;
                return selected;
            }

            selected = selectedArithmeticOutputLayer;
            if (!string.IsNullOrWhiteSpace(selected))
            {
                return selected;
            }

            selectedArithmeticOutputLayer = defaultOutputLayer;
            return selectedArithmeticOutputLayer;
        }

        public IReadOnlyList<string> GetInputLayerNames(string outputLayer, params string[] requiredLayers)
        {
            // A tool's own output preview is a destination, not an input candidate for that same tool.
            // Other tools can still consume that layer through their own input combos.
            List<string> names = GetLayerNames(requiredLayers)
                .Where(title => !IsOwnOutputLayer(title, outputLayer))
                .ToList();

            if (names.Count == 0)
            {
                string fallback = GetFirstInputLayerName(outputLayer);
                if (!string.IsNullOrWhiteSpace(fallback)
                    && !names.Any(title => string.Equals(title, fallback, StringComparison.OrdinalIgnoreCase)))
                {
                    names.Add(fallback);
                }
            }

            return names;
        }

        public IReadOnlyList<string> GetWorkspaceLayerNames(params string[] requiredLayers)
        {
            return GetLayerNames(requiredLayers);
        }

        public bool TryAcceptInputLayer(string requestedInputLayer, string outputLayer)
        {
            if (!IsInputLayerCandidate(requestedInputLayer, outputLayer))
            {
                return false;
            }

            selectedInputLayer = requestedInputLayer;
            return true;
        }

        public void AcceptOutputLayer(string requestedOutputLayer)
        {
            if (!string.IsNullOrWhiteSpace(requestedOutputLayer))
            {
                selectedOutputLayer = requestedOutputLayer.Trim();
            }
        }

        public string SelectNextOutputLayerName()
        {
            selectedOutputLayer = CreateAvailableOutputLayerName(ResolveOutputLayer());
            preparedOutputLayers.Add(selectedOutputLayer);
            return selectedOutputLayer;
        }

        public bool TryAcceptArithmeticInputLayerA(string requestedInputLayer, string outputLayer)
        {
            if (!IsInputLayerCandidate(requestedInputLayer, outputLayer))
            {
                return false;
            }

            selectedArithmeticInputLayerA = requestedInputLayer;
            return true;
        }

        public bool TryAcceptArithmeticInputLayerB(string requestedInputLayer, string outputLayer)
        {
            if (!IsInputLayerCandidate(requestedInputLayer, outputLayer))
            {
                return false;
            }

            selectedArithmeticInputLayerB = requestedInputLayer;
            selectedArithmeticInputLayerBExplicit = true;
            return true;
        }

        public string SelectArithmeticInputLayerBLoadTarget()
        {
            string outputLayer = ResolveArithmeticOutputLayer();
            string inputA = ResolveArithmeticInputLayerAForOutput(outputLayer);
            string inputB = ResolveArithmeticInputLayerBForOutput(outputLayer);
            if (IsInputLayerCandidate(inputB, outputLayer) && !IsSameLayer(inputB, inputA))
            {
                selectedArithmeticInputLayerB = inputB;
                selectedArithmeticInputLayerBExplicit = true;
                return selectedArithmeticInputLayerB;
            }

            // Explicit B-image load must not overwrite Input A/Main when only one input layer exists.
            selectedArithmeticInputLayerB = CreateAvailableInputLayerName(CreateDefaultArithmeticInputLayerName("InputB"));
            selectedArithmeticInputLayerBExplicit = true;
            preparedInputLayers.Add(selectedArithmeticInputLayerB);
            return selectedArithmeticInputLayerB;
        }

        public void AcceptArithmeticOutputLayer(string requestedOutputLayer)
        {
            if (!string.IsNullOrWhiteSpace(requestedOutputLayer))
            {
                selectedArithmeticOutputLayer = requestedOutputLayer.Trim();
            }
        }

        public string SelectNextArithmeticOutputLayerName()
        {
            selectedArithmeticOutputLayer = CreateAvailableOutputLayerName(ResolveArithmeticOutputLayer());
            preparedOutputLayers.Add(selectedArithmeticOutputLayer);
            return selectedArithmeticOutputLayer;
        }

        public bool IsInputLayerCandidate(string layerName, string outputLayer)
        {
            return !string.IsNullOrWhiteSpace(layerName)
                && displayManager.FindIndex(layerName) >= 0
                && !IsOwnOutputLayer(layerName, outputLayer);
        }

        private List<string> GetLayerNames(params string[] requiredLayers)
        {
            List<string> names = displayManager.GetLayerInfos()
                .Select(layer => layer.Title)
                .Where(title => !string.IsNullOrWhiteSpace(title))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (string requiredLayer in requiredLayers ?? Array.Empty<string>())
            {
                if (!string.IsNullOrWhiteSpace(requiredLayer)
                    && !names.Any(title => string.Equals(title, requiredLayer, StringComparison.OrdinalIgnoreCase)))
                {
                    names.Add(requiredLayer);
                }
            }

            if (names.Count == 0)
            {
                names.Add("Main");
            }

            return names;
        }

        private string GetFirstInputLayerName(string outputLayer)
        {
            string title = displayManager.GetLayerInfos()
                .Select(layer => layer.Title)
                .FirstOrDefault(candidate => !string.IsNullOrWhiteSpace(candidate) && !IsOwnOutputLayer(candidate, outputLayer));
            return string.IsNullOrWhiteSpace(title) ? "Main" : title;
        }

        private bool TryGetFirstInputLayerNameExcept(string outputLayer, string excludedLayer, out string layerName)
        {
            layerName = displayManager.GetLayerInfos()
                .Select(layer => layer.Title)
                .FirstOrDefault(candidate =>
                    !string.IsNullOrWhiteSpace(candidate)
                    && !IsOwnOutputLayer(candidate, outputLayer)
                    && !IsSameLayer(candidate, excludedLayer));
            return !string.IsNullOrWhiteSpace(layerName);
        }

        private bool IsOwnOutputLayer(string layerName, string outputLayer)
        {
            if (string.IsNullOrWhiteSpace(layerName))
            {
                return false;
            }

            return IsSameLayer(layerName, outputLayer)
                || IsSameLayer(layerName, defaultOutputLayer)
                || IsSameLayer(layerName, selectedOutputLayer)
                || IsSameLayer(layerName, selectedArithmeticOutputLayer)
                || IsSameLayer(layerName, getSelectedOutputLayer())
                || IsSameLayer(layerName, getSelectedArithmeticOutputLayer());
        }

        private string CreateAvailableOutputLayerName(string requestedLayer)
        {
            string baseName = string.IsNullOrWhiteSpace(requestedLayer)
                ? defaultOutputLayer
                : requestedLayer.Trim();
            if (!IsOutputLayerNameInUse(baseName))
            {
                return baseName;
            }

            string stem = StripTrailingOrdinalSuffix(baseName);
            for (int index = 1; index < 10000; index++)
            {
                string candidate = stem + "_" + index.ToString("000", System.Globalization.CultureInfo.InvariantCulture);
                if (!IsOutputLayerNameInUse(candidate))
                {
                    return candidate;
                }
            }

            return stem + "_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        private string CreateAvailableInputLayerName(string requestedLayer)
        {
            string baseName = string.IsNullOrWhiteSpace(requestedLayer)
                ? CreateDefaultArithmeticInputLayerName("Input")
                : requestedLayer.Trim();
            if (!IsInputLayerNameInUse(baseName))
            {
                return baseName;
            }

            string stem = StripTrailingOrdinalSuffix(baseName);
            for (int index = 1; index < 10000; index++)
            {
                string candidate = stem + "_" + index.ToString("000", System.Globalization.CultureInfo.InvariantCulture);
                if (!IsInputLayerNameInUse(candidate))
                {
                    return candidate;
                }
            }

            return stem + "_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        private string CreateDefaultArithmeticInputLayerName(string suffix)
        {
            string stem = defaultOutputLayer;
            if (!string.IsNullOrWhiteSpace(stem) && stem.EndsWith("_Preview", StringComparison.OrdinalIgnoreCase))
            {
                stem = stem.Substring(0, stem.Length - "_Preview".Length);
            }

            return (string.IsNullOrWhiteSpace(stem) ? "Arithmetic" : stem) + "_" + suffix;
        }

        private bool IsOutputLayerNameInUse(string layerName)
        {
            return !string.IsNullOrWhiteSpace(layerName)
                && (preparedOutputLayers.Contains(layerName) || displayManager.FindIndex(layerName) >= 0);
        }

        private bool IsInputLayerNameInUse(string layerName)
        {
            return !string.IsNullOrWhiteSpace(layerName)
                && (preparedInputLayers.Contains(layerName) || displayManager.FindIndex(layerName) >= 0);
        }

        private static string StripTrailingOrdinalSuffix(string layerName)
        {
            int separator = string.IsNullOrWhiteSpace(layerName) ? -1 : layerName.LastIndexOf('_');
            if (separator <= 0 || separator + 4 != layerName.Length)
            {
                return layerName;
            }

            string suffix = layerName.Substring(separator + 1);
            return suffix.All(char.IsDigit) ? layerName.Substring(0, separator) : layerName;
        }

        private static bool IsSameLayer(string left, string right)
        {
            return !string.IsNullOrWhiteSpace(left)
                && !string.IsNullOrWhiteSpace(right)
                && string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }
    }
}
