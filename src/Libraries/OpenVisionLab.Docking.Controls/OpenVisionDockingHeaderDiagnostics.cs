namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockingHeaderDiagnostics
    {
        public static OpenVisionDockingHeaderDiagnostics Empty { get; } =
            new OpenVisionDockingHeaderDiagnostics(0, true, true, true, string.Empty);

        public OpenVisionDockingHeaderDiagnostics(
            int headerCount,
            bool areGestureReady,
            bool areReadable,
            bool areGripsReady,
            string diagnosticsText)
        {
            HeaderCount = headerCount;
            AreGestureReady = areGestureReady;
            AreReadable = areReadable;
            AreGripsReady = areGripsReady;
            DiagnosticsText = diagnosticsText ?? string.Empty;
        }

        public int HeaderCount { get; }

        public bool AreGestureReady { get; }

        public bool AreReadable { get; }

        public bool AreGripsReady { get; }

        public string DiagnosticsText { get; }
    }
}
