using OpenVisionLab.Core;
using System;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeArithmeticDocumentFactory
    {
        private const string ToolName = "Arithmetic";
        private const string DefaultOutputLayer = "Arithmetic_Preview";

        internal static OpenVisionNativeToolDocument Create(IDisplayManager displayManager, bool forWorkbench = false)
        {
            ArithmeticToolWpfView view = new ArithmeticToolWpfView();
            string settingsConfigName = OpenVisionNativeToolSettingsStore.CreateConfigName(ToolName);
            ArithmeticToolSettings settings = forWorkbench ? OpenVisionNativeToolSettingsStore.LoadExisting(settingsConfigName, new ArithmeticToolSettings())
                : OpenVisionNativeToolSettingsStore.Load(settingsConfigName, new ArithmeticToolSettings());
            ConfigureOperations(view);
            view.ApplyPersistedSettings(settings);
            OpenVisionNativeToolDocument document = new OpenVisionNativeToolDocument(
                displayManager,
                view,
                view,
                ToolName,
                DefaultOutputLayer);
            view.ParameterChanged += (sender, e) => document.RecordParameterChange(() => OpenVisionNativeToolSettingsStore.Save(settingsConfigName, view.CaptureSettings()));
            document.ConfigureParameterState(forWorkbench, () => view.CaptureSettings(), snapshot => view.ApplyPersistedSettings((ArithmeticToolSettings)snapshot),
                context => OpenVisionNativeToolSettingsStore.LoadExisting(settingsConfigName, new ArithmeticToolSettings(), context.Name),
                step => view.ApplyPersistedSettings(VisionPipelineStepPropertyMapper.CreateArithmeticSettings(step)), createDefaults: () => new ArithmeticToolSettings());
            return document;
        }

        private static void ConfigureOperations(ArithmeticToolWpfView view)
        {
            // Arithmetic owns its double-input operation list outside SimplePreprocess so tool families stay separated.
            view.SetOperationList(
                Enum.GetNames(typeof(ArithmeticOperation)),
                ArithmeticOperation.Bitwise_AND.ToString());
        }

        private enum ArithmeticOperation
        {
            Bitwise_AND,
            Bitwise_OR,
            Bitwise_XOR,
            Bitwise_NOT,
            ADD,
            SUBTRACT,
            MULTIPLY,
            DIVIDE,
            MAX,
            MIN,
            ABS,
            ABSDIFF
        }
    }
}
