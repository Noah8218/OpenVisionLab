using OpenVisionLab.Contracts;
using OpenVisionLab.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab.ViewModels
{
    internal sealed class VisionToolLayerSelectionViewModel : ObservableObject, IVisionToolLayerSelectionViewModel
    {
        private IReadOnlyList<string> inputLayers = Array.Empty<string>();
        private IReadOnlyList<string> outputLayers = Array.Empty<string>();
        private string selectedInputLayer = string.Empty;
        private string selectedInputLayerB = string.Empty;
        private string selectedOutputLayer = string.Empty;

        public IReadOnlyList<string> InputLayers
        {
            get => inputLayers;
            private set => SetProperty(ref inputLayers, value ?? Array.Empty<string>());
        }

        public IReadOnlyList<string> OutputLayers
        {
            get => outputLayers;
            private set => SetProperty(ref outputLayers, value ?? Array.Empty<string>());
        }

        public string SelectedInputLayer
        {
            get => selectedInputLayer;
            private set => SetProperty(ref selectedInputLayer, value ?? string.Empty);
        }

        public string SelectedInputLayerB
        {
            get => selectedInputLayerB;
            private set => SetProperty(ref selectedInputLayerB, value ?? string.Empty);
        }

        public string SelectedOutputLayer
        {
            get => selectedOutputLayer;
            private set => SetProperty(ref selectedOutputLayer, value ?? string.Empty);
        }

        public static VisionToolLayerSelectionViewModel CreateSingle(
            IEnumerable<string> layerNames,
            string selectedInputLayer,
            string selectedOutputLayer)
        {
            VisionToolLayerSelectionViewModel viewModel = new VisionToolLayerSelectionViewModel();
            viewModel.ApplySingle(layerNames, selectedInputLayer, selectedOutputLayer);
            return viewModel;
        }

        public static VisionToolLayerSelectionViewModel CreateDual(
            IEnumerable<string> layerNames,
            string selectedInputLayerA,
            string selectedInputLayerB,
            string selectedOutputLayer)
        {
            VisionToolLayerSelectionViewModel viewModel = new VisionToolLayerSelectionViewModel();
            viewModel.ApplyDual(layerNames, selectedInputLayerA, selectedInputLayerB, selectedOutputLayer);
            return viewModel;
        }

        private void ApplySingle(
            IEnumerable<string> layerNames,
            string requestedInputLayer,
            string requestedOutputLayer)
        {
            IReadOnlyList<string> layers = NormalizeLayerNames(layerNames);

            // Inputs are existing workspace layers only; outputs may keep a pending layer name before it exists.
            InputLayers = ExcludeCurrentOutputLayer(layers, requestedOutputLayer);
            OutputLayers = CreateOutputLayerNames(layers, requestedOutputLayer);
            SelectedInputLayer = ResolveExistingLayer(InputLayers, requestedInputLayer);
            SelectedInputLayerB = string.Empty;
            SelectedOutputLayer = ResolveOutputLayer(OutputLayers, requestedOutputLayer);
        }

        private void ApplyDual(
            IEnumerable<string> layerNames,
            string requestedInputLayerA,
            string requestedInputLayerB,
            string requestedOutputLayer)
        {
            IReadOnlyList<string> layers = NormalizeLayerNames(layerNames);

            // Dual-input tools share the same workspace layer list but keep each input selection independent.
            InputLayers = ExcludeCurrentOutputLayer(layers, requestedOutputLayer);
            OutputLayers = CreateOutputLayerNames(layers, requestedOutputLayer);
            SelectedInputLayer = ResolveExistingLayer(InputLayers, requestedInputLayerA);
            SelectedInputLayerB = ResolveExistingLayer(InputLayers, requestedInputLayerB);
            SelectedOutputLayer = ResolveOutputLayer(OutputLayers, requestedOutputLayer);
        }

        private static IReadOnlyList<string> NormalizeLayerNames(IEnumerable<string> layerNames)
        {
            return layerNames?
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
                ?? new List<string>();
        }

        private static IReadOnlyList<string> CreateOutputLayerNames(
            IReadOnlyList<string> layers,
            string selectedOutputLayer)
        {
            List<string> outputLayerNames = layers?.ToList() ?? new List<string>();
            if (!string.IsNullOrWhiteSpace(selectedOutputLayer)
                && !outputLayerNames.Any(item => string.Equals(item, selectedOutputLayer, StringComparison.OrdinalIgnoreCase)))
            {
                outputLayerNames.Add(selectedOutputLayer);
            }

            return outputLayerNames;
        }

        private static IReadOnlyList<string> ExcludeCurrentOutputLayer(
            IReadOnlyList<string> layers,
            string selectedOutputLayer)
        {
            if (string.IsNullOrWhiteSpace(selectedOutputLayer))
            {
                return layers?.ToList() ?? new List<string>();
            }

            return (layers ?? Array.Empty<string>())
                .Where(item => !string.Equals(item, selectedOutputLayer, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private static string ResolveExistingLayer(IReadOnlyList<string> layers, string requestedLayer)
        {
            string match = layers?
                .FirstOrDefault(item => string.Equals(item, requestedLayer, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(match))
            {
                return match;
            }

            return layers?.FirstOrDefault() ?? requestedLayer ?? string.Empty;
        }

        private static string ResolveOutputLayer(IReadOnlyList<string> outputLayers, string requestedLayer)
        {
            if (!string.IsNullOrWhiteSpace(requestedLayer))
            {
                return outputLayers?
                    .FirstOrDefault(item => string.Equals(item, requestedLayer, StringComparison.OrdinalIgnoreCase))
                    ?? requestedLayer;
            }

            return outputLayers?.FirstOrDefault() ?? string.Empty;
        }
    }
}
