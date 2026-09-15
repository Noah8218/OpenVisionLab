using System;
using System.Collections.Generic;
using System.IO;

internal static class RoiImageCanvasBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "p1-view-boundaries-20260913",
                    "m3-imagecanvas")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string viewModelPath = Path.Combine(
            repositoryRoot,
            "src",
            "Libraries",
            "OpenVisionLab.ImageCanvas",
            "ViewModel",
            "RoiImageCanvasViewModel.cs");
        string viewPath = Path.Combine(
            repositoryRoot,
            "src",
            "Libraries",
            "OpenVisionLab.ImageCanvas",
            "View",
            "RoiImageCanvasView.xaml.cs");
        string presentationPath = Path.Combine(
            repositoryRoot,
            "src",
            "Libraries",
            "OpenVisionLab.ImageCanvas",
            "Presentation",
            "RoiImageCanvasPresentation.cs");
        string externalSessionPath = Path.Combine(
            repositoryRoot,
            "src",
            "Libraries",
            "OpenVisionLab.ImageCanvas",
            "External",
            "ImageCanvasExternalConsumerSession.cs");
        string previewAdapterPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Composition",
            "VisionToolOpenGlPreviewCanvasAdapter.cs");
        string bitmapPresenterPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Viewer",
            "OpenVisionBitmapCanvasPresenter.cs");
        string templateEditorPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Popup",
            "Wpf",
            "OpenGlTemplateEditorWindow.xaml.cs");
        string shellViewPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "OpenVisionShellHostView.xaml.cs");
        string layerViewerPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Viewer",
            "OpenVisionLayerViewerView.xaml.cs");

        string viewModelSource = File.ReadAllText(viewModelPath);
        string viewSource = File.ReadAllText(viewPath);
        string presentationSource = File.ReadAllText(presentationPath);
        string externalSessionSource = File.ReadAllText(externalSessionPath);
        string previewAdapterSource = File.ReadAllText(previewAdapterPath);
        string bitmapPresenterSource = File.ReadAllText(bitmapPresenterPath);
        string templateEditorSource = File.ReadAllText(templateEditorPath);
        string shellViewSource = File.ReadAllText(shellViewPath);
        string layerViewerSource = File.ReadAllText(layerViewerPath);
        List<string> results = new();

        Check(
            "ViewModel retains state and binding contracts without the concrete native control",
            viewModelSource.Contains("RoiImageCanvasWpfKeyboardInputController", StringComparison.Ordinal)
                && viewModelSource.Contains("IImageCanvasDialogHost", StringComparison.Ordinal)
                && viewModelSource.Contains("IImageCanvasContextMenuHost", StringComparison.Ordinal)
                && viewModelSource.Contains("ImageCanvasDirectoryPolicy", StringComparison.Ordinal)
                && viewModelSource.Contains("CanvasImageLoader", StringComparison.Ordinal)
                && viewModelSource.Contains("CanvasImageSaver", StringComparison.Ordinal)
                && !viewModelSource.Contains("ImageCanvasControl", StringComparison.Ordinal)
                && !viewModelSource.Contains("_imageViewer", StringComparison.Ordinal)
                && !viewModelSource.Contains("new RoiImageCanvasPresentation", StringComparison.Ordinal),
            results);
        Check(
            "ViewModel does not create a modal UI or own file encoding policy",
            !viewModelSource.Contains("new OpenFileDialog", StringComparison.Ordinal)
                && !viewModelSource.Contains("new SaveFileDialog", StringComparison.Ordinal)
                && !viewModelSource.Contains("FileStream", StringComparison.Ordinal)
                && !viewModelSource.Contains("Cv2.ImWrite", StringComparison.Ordinal),
            results);
        Check(
            "ViewModel remains the Mat and mutable ROI state owner",
            Count(viewModelSource, "_currentImageMat?.Dispose();") >= 3
                && viewModelSource.Contains("CaptureWindowRoiSnapshot", StringComparison.Ordinal)
                && viewModelSource.Contains("RestoreWindowRoiSnapshot", StringComparison.Ordinal)
                && viewModelSource.Contains("public void Dispose()", StringComparison.Ordinal)
                && !viewModelSource.Contains("_presentation.Dispose();", StringComparison.Ordinal),
            results);
        Check(
            "Presentation owner creates, wires, and disposes the native control",
            presentationSource.Contains("new ImageCanvasControl()", StringComparison.Ordinal)
                && presentationSource.Contains("new RoiImageCanvasKeyboardInputController", StringComparison.Ordinal)
                && presentationSource.Contains("new RoiImageCanvasMouseInputController", StringComparison.Ordinal)
                && presentationSource.Contains("new System.Timers.Timer", StringComparison.Ordinal)
                && presentationSource.Contains("imageViewer.Draw += OnDraw", StringComparison.Ordinal)
                && presentationSource.Contains("imageViewer.Draw -= OnDraw", StringComparison.Ordinal)
                && presentationSource.Contains("imageViewer.ClearTexture();", StringComparison.Ordinal)
                && presentationSource.Contains("imageViewer.Dispose();", StringComparison.Ordinal),
            results);
        Check(
            "Detached presentation preserves same-owner state without strongly retaining the ViewModel",
            presentationSource.Contains("WeakReference<RoiImageCanvasViewModel>", StringComparison.Ordinal)
                && presentationSource.Contains("preserveCanvasState", StringComparison.Ordinal)
                && presentationSource.Contains("viewModel = null;", StringComparison.Ordinal),
            results);
        Check(
            "WPF View owns presentation attach, native hosting, detach, and disposal",
            viewSource.Contains("viewModel.ImageDialogHost = imageCanvasDialogHost", StringComparison.Ordinal)
                && viewSource.Contains("attachedViewModel.ImageDialogHost = null", StringComparison.Ordinal)
                && viewSource.Contains("new RoiImageCanvasPresentation()", StringComparison.Ordinal)
                && viewSource.Contains("presentation.Attach(viewModel)", StringComparison.Ordinal)
                && viewSource.Contains("presentation.Detach(attachedViewModel)", StringComparison.Ordinal)
                && viewSource.Contains("imageBoxCameraTwoD.Child = presentation.Control", StringComparison.Ordinal)
                && viewSource.Contains("presentation.Dispose();", StringComparison.Ordinal)
                && viewSource.Contains("viewModel.PreviewKeyDownCommand", StringComparison.Ordinal)
                && viewSource.Contains("viewModel.KeyUpCommand", StringComparison.Ordinal),
            results);
        Check(
            "Every known native presentation lifetime has a concrete release owner",
            externalSessionSource.Contains("presentation.Dispose();", StringComparison.Ordinal)
                && previewAdapterSource.Contains("canvasView.Dispose();", StringComparison.Ordinal)
                && bitmapPresenterSource.Contains("canvasViewModel.Dispose();", StringComparison.Ordinal)
                && templateEditorSource.Contains("glCanvas.Dispose();", StringComparison.Ordinal)
                && shellViewSource.Contains("hostWorkspaceCanvas?.Dispose();", StringComparison.Ordinal)
                && layerViewerSource.Contains("layerCanvas.Dispose();", StringComparison.Ordinal),
            results);
        Check(
            "Public canvas operations no longer require exposing ImageCanvasControl",
            viewModelSource.Contains("public class RoiImageCanvasViewModel : ObservableObject, IDisposable", StringComparison.Ordinal)
                && viewModelSource.Contains("public void LoadImage(Mat mat", StringComparison.Ordinal)
                && viewModelSource.Contains("public void ClearImage()", StringComparison.Ordinal)
                && viewModelSource.Contains("public void AddOverlay(", StringComparison.Ordinal)
                && viewModelSource.Contains("public CanvasViewState CaptureViewState()", StringComparison.Ordinal)
                && !externalSessionSource.Contains("viewModel.ImageViewer", StringComparison.Ordinal)
                && !previewAdapterSource.Contains("canvasViewModel.ImageViewer", StringComparison.Ordinal)
                && !bitmapPresenterSource.Contains("canvasViewModel.ImageViewer", StringComparison.Ordinal)
                && !templateEditorSource.Contains("canvasViewModel.ImageViewer", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "roi-image-canvas-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "ROI_IMAGE_CANVAS_BOUNDARY_CONTRACT="
            + (passed ? "PASS" : "FAIL")
            + "|checks="
            + results.Count);
        return passed ? 0 : 1;
    }

    private static int Count(string value, string token)
    {
        int count = 0;
        int index = 0;
        while ((index = value.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += token.Length;
        }

        return count;
    }

    private static void Check(string name, bool condition, ICollection<string> results)
    {
        results.Add((condition ? "PASS: " : "FAIL: ") + name);
    }

    private static string ResolveRepositoryRoot()
    {
        DirectoryInfo? current = new DirectoryInfo(Environment.CurrentDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "src", "OpenVisionLab", "OpenVisionLab.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "src", "OpenVisionLab", "OpenVisionLab.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("OpenVisionLab repository root was not found.");
    }
}
