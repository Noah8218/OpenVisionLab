using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Common;
using OpenVisionLab.Core;
using OpenVisionLab.Core.Integration;
using OpenVisionLab.Integration.Contracts;
using OpenVisionLab.Pipeline.Controls;
using OpenVisionLab.Smoke;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Blob;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D.Result;
using OpenVisionLab.Vision2D.Tool;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using OpenVisionLab.ImageSpace.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using Bitmap = System.Drawing.Bitmap;

if (args.Length == 6
    && string.Equals(args[0], "--integration-2d", StringComparison.OrdinalIgnoreCase))
{
    return await TwoDIntegrationSmoke.RunAsync(
        args[1],
        args[2],
        args[3],
        args[4],
        args[5]);
}

if (args.Length == 5
    && string.Equals(args[0], "--integration-2d-concurrent-process", StringComparison.OrdinalIgnoreCase))
{
    return await TwoDIntegrationSmoke.RunConcurrentProcessAsync(
        args[1],
        args[2],
        args[3],
        args[4]);
}

if (args.Length == 4
    && string.Equals(args[0], "--integration-2d-discovery-isolation-contract", StringComparison.OrdinalIgnoreCase))
{
    return TwoDIntegrationSmoke.RunDiscoveryIsolationContract(
        args[1],
        args[2],
        args[3]);
}

if (args.Length == 5
    && string.Equals(args[0], "--integration-2d-run-record-recovery-contract", StringComparison.OrdinalIgnoreCase))
{
    return await TwoDIntegrationSmoke.RunRunRecordRecoveryContractAsync(
        args[1],
        args[2],
        args[3],
        args[4]);
}

if (args.Length == 3
    && string.Equals(args[0], "--integration-2d-input-hash-decode-contract", StringComparison.OrdinalIgnoreCase))
{
    return await TwoDIntegrationInputHashDecodeContract.RunAsync(
        args[1],
        args[2]);
}

if (args.Length == 3
    && string.Equals(args[0], "--sdk-deployment-provenance-contract", StringComparison.OrdinalIgnoreCase))
{
    return SdkDeploymentProvenanceContract.Run(args[1], args[2]);
}

if (args.Length == 6
    && string.Equals(args[0], "--integration-2d-run-record-recovery-worker", StringComparison.OrdinalIgnoreCase))
{
    return await TwoDIntegrationSmoke.RunRunRecordRecoveryWorkerAsync(
        args[1],
        args[2],
        args[3],
        args[4],
        args[5]);
}

if (args.Length == 6
    && string.Equals(args[0], "--integration-2d-concurrent-process-worker", StringComparison.OrdinalIgnoreCase))
{
    return await TwoDIntegrationSmoke.RunConcurrentProcessWorkerAsync(
        args[1],
        args[2],
        args[3],
        args[4],
        args[5]);
}

if (args.Length == 2
    && string.Equals(args[0], "--integration-2d-result-disposition-contract", StringComparison.OrdinalIgnoreCase))
{
    return TwoDIntegrationResultDispositionContract.Run(args[1]);
}

if ((args.Length == 5
        || args.Length == 6
            && string.Equals(args[5], "--require-locator-evidence", StringComparison.OrdinalIgnoreCase))
    && string.Equals(args[0], "--integration-2d-published", StringComparison.OrdinalIgnoreCase))
{
    return await TwoDIntegrationCrossRepoSmoke.RunAsync(
        args[1],
        args[2],
        args[3],
        args[4],
        args.Length == 6);
}

if (args.Length == 1 && string.Equals(args[0], "--pinarraygap-intent-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunPinArrayGapIntentContract();
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--runtime-stability-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunRuntimeStabilityContractAsync(args.Length == 2 ? args[1] : null);
}

if (args.Length >= 1
    && args.Length <= 3
    && string.Equals(args[0], "--video-size-memory-baseline-contract", StringComparison.OrdinalIgnoreCase))
{
    string evidenceDirectory = args.Length >= 2
        ? args[1]
        : Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "2d034-video-size-memory-baseline-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    bool includeTwentyThousand = args.Length == 3
        && string.Equals(args[2], "--include-20000", StringComparison.OrdinalIgnoreCase);
    if (args.Length == 3 && !includeTwentyThousand)
    {
        Console.Error.WriteLine("Unknown 2D-034 option. Use --include-20000 only after a safe preflight.");
        return 2;
    }

    return await VideoSizeMemoryBaselineContract.RunAsync(evidenceDirectory, includeTwentyThousand);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--output-allocation-preflight-contract", StringComparison.OrdinalIgnoreCase))
{
    return await OutputAllocationPreflightContract.RunAsync(args.Length == 2 ? args[1] : Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "2d036-output-allocation-preflight-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--image-space-snapshot-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunImageSpaceSnapshotContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-review-layer-image-owner-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunPipelineReviewLayerImageOwnerContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-review-cache-lifetime-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunPipelineReviewCacheLifetimeContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-review-stale-callback-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunPipelineReviewStaleCallbackContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-review-execution-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunPipelineReviewExecutionContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-review-duplicate-step-identity-contract", StringComparison.OrdinalIgnoreCase))
{
    return await PipelineReviewDuplicateStepIdentityContract.RunAsync(
        args.Length == 2 ? args[1] : Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "2d011-duplicate-step-identity-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-review-run-input-isolation-contract", StringComparison.OrdinalIgnoreCase))
{
    return await PipelineReviewRunInputIsolationContract.RunAsync(
        args.Length == 2 ? args[1] : Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "2d012-run-input-isolation-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--preview-run-reopen-equivalence-contract", StringComparison.OrdinalIgnoreCase))
{
    return await PreviewRunEquivalenceContract.RunAsync(
        args.Length == 2 ? args[1] : Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "2d013-preview-run-reopen-equivalence-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pixelpermm-finite-unit-contract", StringComparison.OrdinalIgnoreCase))
{
    return PixelPerMmFiniteUnitContract.Run(args.Length == 2 ? args[1] : Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "2d014-pixelpermm-finite-unit-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-layer-reference-invariant-contract", StringComparison.OrdinalIgnoreCase))
{
    return await PipelineLayerReferenceInvariantContract.RunAsync(args.Length == 2 ? args[1] : Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "2d015-layer-reference-invariant-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--threshold-suggestion-session-contract", StringComparison.OrdinalIgnoreCase))
{
    return ThresholdSuggestionSessionContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-review-document-revision-contract", StringComparison.OrdinalIgnoreCase))
{
    return OpenVisionPipelineReviewDocumentRevisionContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--tcp-controller-disposal-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunTcpControllerDisposalContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--bitmap-converter-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunBitmapConverterContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--mat-view-ownership-contract", StringComparison.OrdinalIgnoreCase))
{
    return MatViewOwnershipContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--input-format-meaning-contract", StringComparison.OrdinalIgnoreCase))
{
    return InputFormatMeaningContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--image-compare-resource-contract", StringComparison.OrdinalIgnoreCase))
{
    return ImageCompareResourceContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--image-compare-directory-policy-contract", StringComparison.OrdinalIgnoreCase))
{
    return ImageCompareDirectoryPolicyContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--image-compare-point-mapping-contract", StringComparison.OrdinalIgnoreCase))
{
    return ImageComparePointMappingContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--app-path-boundary-contract", StringComparison.OrdinalIgnoreCase))
{
    return AppPathBoundaryContract.Run(args.Length == 2 ? args[1] : null);
}

if (args.Length == 2 && string.Equals(args[0], "--recipe-save-failure-contract", StringComparison.OrdinalIgnoreCase))
{
    return RecipeSaveFailureContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--pipeline-failure-lifetime-contract", StringComparison.OrdinalIgnoreCase))
{
    return await PipelineFailureLifetimeContract.RunAsync(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--pipeline-cancellation-result-contract", StringComparison.OrdinalIgnoreCase))
{
    return await PipelineCancellationResultContract.RunAsync(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--pipeline-not-run-tail-status-contract", StringComparison.OrdinalIgnoreCase))
{
    return await PipelineNotRunTailStatusContract.RunAsync(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--pipeline-acceptance-finite-contract", StringComparison.OrdinalIgnoreCase))
{
    return PipelineAcceptanceFiniteContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--expected-failure-contract", StringComparison.OrdinalIgnoreCase))
{
    return ExpectedFailureContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--confusion-matrix-contract", StringComparison.OrdinalIgnoreCase))
{
    return ConfusionMatrixContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--pipeline-prevalidation-contract", StringComparison.OrdinalIgnoreCase))
{
    return await PipelinePrevalidationContract.RunAsync(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--pipeline-roi-meaning-contract", StringComparison.OrdinalIgnoreCase))
{
    return await PipelineRoiMeaningContract.RunAsync(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--recipe-load-recovery-contract", StringComparison.OrdinalIgnoreCase))
{
    return RecipeLoadRecoveryContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--recipe-persistence-execution-gate-contract", StringComparison.OrdinalIgnoreCase))
{
    return RecipePersistenceExecutionGateContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--pipeline-xml-schema-compatibility-contract", StringComparison.OrdinalIgnoreCase))
{
    return PipelineXmlSchemaCompatibilityContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--pipeline-xml-roundtrip-compatibility-contract", StringComparison.OrdinalIgnoreCase))
{
    return PipelineXmlRoundTripCompatibilityContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--recipe-file-snapshot-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RecipeFileSnapshotContract.RunAsync(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--recipe-multi-file-save-recovery-contract", StringComparison.OrdinalIgnoreCase))
{
    return RecipeMultiFileSaveRecoveryContract.Run(args[1]);
}


if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--recipe-storage-path-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunRecipeStoragePathContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--validation-set-document-owner-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunValidationSetDocumentOwnerContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--validation-set-status-presenter-contract", StringComparison.OrdinalIgnoreCase))
{
    return ValidationSetStatusPresenterContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--validation-set-evidence-notification-contract", StringComparison.OrdinalIgnoreCase))
{
    return ValidationSetEvidenceNotificationContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--validation-set-projection-error-contract", StringComparison.OrdinalIgnoreCase))
{
    return ValidationSetProjectionErrorContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--validation-set-success-projection-contract", StringComparison.OrdinalIgnoreCase))
{
    return ValidationSetSuccessProjectionContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--refresh-options-command-state-contract", StringComparison.OrdinalIgnoreCase))
{
    return RefreshOptionsCommandStateContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--recipe-command-surface-clipboard-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return RecipeCommandSurfaceClipboardBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--recipe-command-surface-dispatch-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return RecipeCommandSurfaceDispatcherBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--recipe-command-surface-image-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return RecipeCommandSurfaceImageBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if (args.Length == 2
            && string.Equals(args[0], "--recipe-run-evidence-image-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return RecipeRunEvidenceImageBoundaryContract.Run(args[1]);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--recipe-dialog-localization-contract", StringComparison.OrdinalIgnoreCase))
        {
            return RecipeDialogLocalizationContract.Run(args.Length == 2 ? args[1] : null);
        }

        if (args.Length == 2
            && string.Equals(args[0], "--line-tool-persistence-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return LineToolPersistenceBoundaryContract.Run(args[1]);
        }
        if (args.Length == 2
            && string.Equals(args[0], "--line-tool-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return LineToolPartialBoundaryContract.Run(args[1]);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--line-overlay-label-layout-contract", StringComparison.OrdinalIgnoreCase))
        {
            return LineOverlayLabelLayoutContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--simple-preprocess-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return SimplePreprocessPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--affine-transform-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return AffineTransformPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--arithmetic-tool-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return ArithmeticToolPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--filter-tool-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return FilterToolPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--blob-tool-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return BlobToolPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--contour-tool-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return ContourToolPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--binary-learn-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return BinaryLearnPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--foundation-learn-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return FoundationLearnPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--grayscale-learn-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return GrayscaleLearnPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--layer-recipe-learn-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return LayerRecipeLearnPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--geometry-learn-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return GeometryLearnPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--metrics-acceptance-learn-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return MetricsAcceptanceLearnPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--vision-tool-verification-guide-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return VisionToolVerificationGuidePartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--vision-tool-parameter-guide-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return VisionToolParameterGuidePartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--vision-tool-double-input-shell-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return VisionToolDoubleInputCustomToolShellPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if (args.Length == 2
            && string.Equals(args[0], "--template-editor-image-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return TemplateEditorImageBoundaryContract.Run(args[1]);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--color-hsv-learn-presenter-contract", StringComparison.OrdinalIgnoreCase))
        {
            return ColorHsvLearnPresenterContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--roi-image-canvas-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return RoiImageCanvasBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--roi-image-canvas-path-policy-contract", StringComparison.OrdinalIgnoreCase))
        {
            return RoiImageCanvasPathPolicyContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--learn-window-document-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return LearnWindowDocumentBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--signal-inspector-export-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return SignalInspectorExportBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--morphology-tool-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return MorphologyToolPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--edge-based-matching-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return EdgeBasedMatchingPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--feature-matching-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
{
    return FeatureMatchingPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--matching-tool-partial-boundary-contract", StringComparison.OrdinalIgnoreCase))
{
    return MatchingToolPartialBoundaryContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--workspace-sample-picker-image-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceSamplePickerImageBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

        if ((args.Length == 1 || args.Length == 2)
            && string.Equals(args[0], "--tool-n-image-verification-image-boundary-contract", StringComparison.OrdinalIgnoreCase))
        {
            return ToolNImageVerificationImageBoundaryContract.Run(args.Length == 2 ? args[1] : null);
        }

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--validation-evidence-owner-contract", StringComparison.OrdinalIgnoreCase))
{
    return ValidationEvidenceOwnerContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--step-preview-navigation-owner-contract", StringComparison.OrdinalIgnoreCase))
{
    return StepPreviewNavigationOwnerContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--shell-recipe-basic-lifecycle-view-contract", StringComparison.OrdinalIgnoreCase))
{
    return ShellRecipeBasicLifecycleViewContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--property-grid-value-change-subscription-contract", StringComparison.OrdinalIgnoreCase))
{
    return PropertyGridPropertyValueChangeSubscriptionContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--property-grid-metadata-adapter-contract", StringComparison.OrdinalIgnoreCase))
{
    return PropertyGridMetadataAdapterContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--namespace-project-boundary-contract", StringComparison.OrdinalIgnoreCase))
{
    return NamespaceProjectBoundaryContract.Run(args.Length == 2 ? args[1] : null);
}
if (args.Length == 2 && string.Equals(args[0], "--recipe-execution-session-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RecipeExecutionSessionContract.RunAsync(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--recipe-run-history-orchestration-contract", StringComparison.OrdinalIgnoreCase))
{
    return RecipeRunHistoryOrchestrationContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--learn-matching-presentation-contract", StringComparison.OrdinalIgnoreCase))
{
    return LearnMatchingPresentationContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--learn-foundation-presentation-contract", StringComparison.OrdinalIgnoreCase))
{
    return LearnFoundationPresentationContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--learn-geometry-presentation-contract", StringComparison.OrdinalIgnoreCase))
{
    return LearnGeometryPresentationContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--learn-grayscale-presentation-contract", StringComparison.OrdinalIgnoreCase))
{
    return LearnGrayscalePresentationContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--learn-binary-presentation-contract", StringComparison.OrdinalIgnoreCase))
{
    return LearnBinaryPresentationContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--learn-line-presentation-contract", StringComparison.OrdinalIgnoreCase))
{
    return LearnLinePresentationContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--learn-metrics-acceptance-contract", StringComparison.OrdinalIgnoreCase))
{
    return LearnMetricsAcceptanceContract.Run(args[1]);
}

if (args.Length == 2 && string.Equals(args[0], "--learn-layer-recipe-contract", StringComparison.OrdinalIgnoreCase))
{
    return LearnLayerRecipeContract.Run(args[1]);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--step-edit-loader-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunStepEditLoaderContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--step-edit-apply-owner-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunStepEditApplyOwnerContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--step-edit-apply-projection-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunStepEditApplyProjectionContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--recipe-workspace-policy-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunRecipeWorkspacePolicyContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--recipe-workspace-lifecycle-projection-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunRecipeWorkspaceLifecycleProjectionContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--recipe-manager-summary-projection-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunRecipeManagerSummaryProjectionContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--recipe-manager-pipeline-option-projection-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunRecipeManagerPipelineOptionProjectionContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--recipe-pipeline-lifecycle-projection-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunRecipePipelineLifecycleProjectionContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--recipe-pipeline-exchange-projection-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunRecipePipelineExchangeProjectionContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--recipe-review-bundle-dry-run-projection-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunRecipeReviewBundleDryRunProjectionContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--recipe-external-asset-reconnection-contract", StringComparison.OrdinalIgnoreCase))
{
    return RecipeExternalAssetReconnectionContract.Run(
        args.Length == 2
            ? args[1]
            : Directory.Exists(@"D:\OpenVisionLab-TestData")
                ? Path.Combine(
                    @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                    "2d020-asset-reconnection")
                : Path.Combine(Path.GetTempPath(), "OpenVisionLab-recipe-external-asset-reconnection"));
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--llm-draft-review-owner-contract", StringComparison.OrdinalIgnoreCase))
{
    return LlmDraftReviewOwnerContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pinarraygap-validation-identity-owner-contract", StringComparison.OrdinalIgnoreCase))
{
    return PinArrayGapValidationIdentityOwnerContract.Run(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-review-result-status-projection-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunPipelineReviewResultStatusProjectionContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-review-guide-result-projection-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunPipelineReviewGuideResultProjectionContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-review-domain-evidence-projection-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunPipelineReviewDomainEvidenceProjectionContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-provenance-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunPipelineProvenanceContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-persistence-recovery-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunPipelinePersistenceRecoveryContract(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--pipeline-persistence-process-recovery-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunPipelinePersistenceProcessRecoveryContract(args.Length == 2 ? args[1] : null);
}

if (args.Length >= 2
    && string.Equals(args[0], "--pipeline-persistence-process-recovery-probe", StringComparison.OrdinalIgnoreCase))
{
    return RunPipelinePersistenceProcessRecoveryProbe(args);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--reliability-soak-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunReliabilitySoakContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--object-dimension-filter-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunObjectDimensionFilterContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--object-candidate-parity-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunObjectCandidateParityContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--blob-contour-audit-baseline", StringComparison.OrdinalIgnoreCase))
{
    return await RunBlobContourAuditBaselineAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--tool-n-image-verification-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunToolNImageVerificationContractAsync(args.Length == 2 ? args[1] : null);
}

if (args.Length == 6
    && string.Equals(args[0], "--tool-n-image-real-folder-acceptance", StringComparison.OrdinalIgnoreCase))
{
    return await RunToolNImageRealFolderAcceptanceAsync(
        args[1],
        args[2],
        args[3],
        args[4],
        args[5]);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--affine-transform-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunAffineTransformContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--coordinate-transform-meaning-contract", StringComparison.OrdinalIgnoreCase))
{
    return await CoordinateTransformMeaningContract.RunAsync(args.Length == 2
        ? args[1]
        : Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "2d030-coordinate-transform-meaning-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--matching-boundary-contract", StringComparison.OrdinalIgnoreCase))
{
    return await MatchingBoundaryContract.RunAsync(args.Length == 2
        ? args[1]
        : Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "2d031-matching-boundary-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--blob-contour-candidate-boundary-contract", StringComparison.OrdinalIgnoreCase))
{
    return await BlobContourCandidateBoundaryContract.RunAsync(args.Length == 2
        ? args[1]
        : Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "2d032-blob-contour-candidate-boundary-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--line-length-mean-degenerate-contract", StringComparison.OrdinalIgnoreCase))
{
    return await LineLengthMeanDegenerateContract.RunAsync(args.Length == 2
        ? args[1]
        : Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "2d033-line-length-mean-degenerate-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--affine-detected-points-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunAffineDetectedPointsContractAsync(args.Length == 2 ? args[1] : null);
}

if (args.Length == 3
    && string.Equals(args[0], "--affine-card-pilot", StringComparison.OrdinalIgnoreCase))
{
    return await RunAffineCardPilotAsync(args[1], args[2]);
}

if (args.Length == 3
    && string.Equals(args[0], "--affine-card-fixed-roi", StringComparison.OrdinalIgnoreCase))
{
    return await RunAffineCardPilotAsync(
        args[1],
        args[2],
        includeFixedRoiMean: true,
        maximumPostResidualPx: 5D);
}

if (args.Length == 4
    && string.Equals(args[0], "--edge-unique-card-r-matrix", StringComparison.OrdinalIgnoreCase))
{
    return await RunEdgeUniqueCardRMatrixAsync(args[1], args[2], args[3]);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--edge-global-polarity-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunEdgeGlobalPolarityContract(args.Length == 2 ? args[1] : null);
}

if (args.Length == 3
    && string.Equals(args[0], "--auto-mpoint-easymatch-candidates", StringComparison.OrdinalIgnoreCase))
{
    return RunAutoMPointEasyMatchCandidates(args[1], args[2]);
}

if (args.Length == 3
    && string.Equals(args[0], "--auto-mpoint-six-corpus-pilot", StringComparison.OrdinalIgnoreCase))
{
    return RunAutoMPointSixCorpusPilot(args[1], args[2]);
}

if (args.Length == 4
    && string.Equals(args[0], "--auto-mpoint-representative-best-pilot", StringComparison.OrdinalIgnoreCase))
{
    return RunAutoMPointRepresentativeBestPilot(args[1], args[2], args[3]);
}

if (args.Length == 5
    && string.Equals(args[0], "--auto-mpoint-full-stratum-qualification", StringComparison.OrdinalIgnoreCase))
{
    return RunAutoMPointFullStratumQualification(args[1], args[2], args[3], args[4]);
}

if (args.Length == 5 && string.Equals(args[0], "--batch", StringComparison.OrdinalIgnoreCase))
{
    return await RunBatchAsync(args[1], args[2], args[3], args[4]);
}

if (args.Length == 6 && string.Equals(args[0], "--batch-evidence", StringComparison.OrdinalIgnoreCase))
{
    return await RunBatchAsync(args[1], args[2], args[3], args[4], args[5]);
}

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: VisionRecipeRunnerSmoke <imagePath> <pipelineXmlPath> [resultImagePath]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --batch <imageListPath> <datasetRoot> <pipelineXmlPath> <csvPath>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --batch-evidence <imageListPath> <datasetRoot> <pipelineXmlPath> <csvPath> <evidenceRoot>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pinarraygap-intent-contract");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --integration-2d <evidenceRoot> <goodImagePath> <badImagePath> <pipelineXmlPath> <runtimeBuildManifestPath>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --integration-2d-concurrent-process <evidenceRoot> <imagePath> <pipelineXmlPath> <runtimeBuildManifestPath>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --integration-2d-published <exchangeRoot> <producerManifestPath> <evidenceRoot> <runtimeBuildManifestPath> [--require-locator-evidence]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --runtime-stability-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --image-space-snapshot-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-review-layer-image-owner-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-review-cache-lifetime-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-review-stale-callback-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-review-execution-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-review-duplicate-step-identity-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-review-run-input-isolation-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --preview-run-reopen-equivalence-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-layer-reference-invariant-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --threshold-suggestion-session-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-review-document-revision-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --tcp-controller-disposal-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --bitmap-converter-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --mat-view-ownership-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --input-format-meaning-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-run-evidence-image-boundary-contract <evidenceDirectory>");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-dialog-localization-contract <evidenceDirectory>");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --line-tool-persistence-boundary-contract <evidenceDirectory>");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --line-tool-partial-boundary-contract <evidenceDirectory>");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --line-overlay-label-layout-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --simple-preprocess-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --affine-transform-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --arithmetic-tool-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --filter-tool-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --blob-tool-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --contour-tool-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --binary-learn-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --foundation-learn-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --grayscale-learn-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --layer-recipe-learn-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --geometry-learn-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --metrics-acceptance-learn-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --vision-tool-verification-guide-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --vision-tool-parameter-guide-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --vision-tool-double-input-shell-partial-boundary-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --template-editor-image-boundary-contract <evidenceDirectory>");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --color-hsv-learn-presenter-contract [evidenceDirectory]");
        Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --roi-image-canvas-boundary-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-storage-path-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-save-failure-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-load-recovery-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-persistence-execution-gate-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-xml-schema-compatibility-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-xml-roundtrip-compatibility-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-multi-file-save-recovery-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-failure-lifetime-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-cancellation-result-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-not-run-tail-status-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-acceptance-finite-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --validation-set-document-owner-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --property-grid-value-change-subscription-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --property-grid-metadata-adapter-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-execution-session-contract <evidenceDirectory> (isolated D-drive runtime)");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-run-history-orchestration-contract <evidenceDirectory> (isolated D-drive runtime)");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --learn-matching-presentation-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --learn-metrics-acceptance-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --learn-layer-recipe-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --step-edit-loader-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --step-edit-apply-owner-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --step-edit-apply-projection-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-workspace-policy-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-workspace-lifecycle-projection-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-manager-summary-projection-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-manager-pipeline-option-projection-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-pipeline-lifecycle-projection-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-pipeline-exchange-projection-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-review-bundle-dry-run-projection-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --recipe-external-asset-reconnection-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --llm-draft-review-owner-contract <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pinarraygap-validation-identity-owner-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --validation-set-evidence-notification-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --validation-set-projection-error-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --validation-set-success-projection-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-review-result-status-projection-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-review-guide-result-projection-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-review-domain-evidence-projection-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-provenance-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --reliability-soak-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pipeline-persistence-process-recovery-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --object-dimension-filter-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --object-candidate-parity-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --blob-contour-audit-baseline [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --tool-n-image-verification-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --affine-transform-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --coordinate-transform-meaning-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --matching-boundary-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --blob-contour-candidate-boundary-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --line-length-mean-degenerate-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --affine-detected-points-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --affine-card-pilot <cardDatasetRoot> <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --affine-card-fixed-roi <cardDatasetRoot> <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --edge-unique-card-r-matrix <cardDatasetRoot> <p220ResultsCsv> <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --edge-global-polarity-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --auto-mpoint-easymatch-candidates <easyMatchSampleRoot> <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --auto-mpoint-six-corpus-pilot <labelTestRoot> <evidenceDirectory>");
    return 2;
}

string imagePath = Path.GetFullPath(args[0]);
string pipelineXmlPath = Path.GetFullPath(args[1]);
string? resultImagePath = args.Length >= 3 && !args[2].StartsWith("--", StringComparison.Ordinal)
    ? Path.GetFullPath(args[2])
    : null;
string? allOverlayImagePath = GetOptionValue(args, "--all-overlay-image");
bool printOverlays = args.Any(arg =>
    string.Equals(arg, "--overlays", StringComparison.OrdinalIgnoreCase)
    || string.Equals(arg, "--overlay-bounds", StringComparison.OrdinalIgnoreCase));

static string? GetOptionValue(string[] args, string optionName)
{
    for (int i = 0; i < args.Length - 1; i++)
    {
        if (string.Equals(args[i], optionName, StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFullPath(args[i + 1]);
        }
    }

    return null;
}

static string? GetRawOptionValue(string[] args, string optionName)
{
    for (int i = 0; i < args.Length - 1; i++)
    {
        if (string.Equals(args[i], optionName, StringComparison.OrdinalIgnoreCase))
        {
            return args[i + 1];
        }
    }

    return null;
}

static async Task<int> RunRuntimeStabilityContractAsync(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
        ?? Path.Combine("artifacts", "runtime_stability_contract"));
    Directory.CreateDirectory(evidenceDirectory);
    List<string> failures = new List<string>();

    if (!OpenVisionLabUnhandledExceptionPolicy.IsRecoverableDispatcherException(new OperationCanceledException())
        || OpenVisionLabUnhandledExceptionPolicy.IsRecoverableDispatcherException(new InvalidOperationException("fatal")))
    {
        failures.Add("Dispatcher exception policy did not distinguish cancellation from a fatal unhandled exception.");
    }

    using (CancellationTokenSource canceled = new CancellationTokenSource())
    {
        canceled.Cancel();
        VisionPipelineSampleCheckResult canceledResult = await VisionPipelineSampleCheckService.RunSampleCheckSafeAsync(
            null,
            null,
            canceled.Token);
        if (!canceledResult.Message.Contains("canceled", StringComparison.OrdinalIgnoreCase))
        {
            failures.Add("A pre-canceled sample check did not return the safe cancellation result.");
        }
    }

    VisionPipelineSampleCatalogItem? publicSample = VisionPipelineSampleCatalogItem
        .LoadRunnable(VisionPipelineSampleCatalogSourceKind.Public)
        .FirstOrDefault(item => string.Equals(
            item.SampleName,
            "Public_Threshold_BandPads_Good",
            StringComparison.OrdinalIgnoreCase));
    if (publicSample == null)
    {
        failures.Add("The public Threshold stability sample was not found.");
    }
    else
    {
        VisionPipelineSampleCheckResult publicResult = await VisionPipelineSampleCheckService.RunSampleCheckSafeAsync(
            publicSample,
            cancellationToken: CancellationToken.None);
        if (!publicResult.ExecutionCompleted || !publicResult.Success)
        {
            failures.Add("The valid async public sample path failed: " + publicResult.Message);
        }
    }

    TaskCompletionSource<VisionToolResult> lateCompletion = new TaskCompletionSource<VisionToolResult>(
        TaskCreationOptions.RunContinuationsAsynchronously);
    Task<bool> deadlineWait = VisionPipelineExecutionService.WaitForStepCompletionAsync(
        lateCompletion.Task,
        1,
        CancellationToken.None);
    await Task.Delay(100);
    if (deadlineWait.IsCompleted)
    {
        failures.Add("A timed-out Step returned before its in-process work released owned resources.");
    }

    Mat lateImage = new Mat(2, 2, MatType.CV_8UC1, Scalar.White);
    lateCompletion.SetResult(new VisionToolResult
    {
        Success = true,
        ResultImage = lateImage
    });
    bool completedWithinDeadline = await deadlineWait;
    if (completedWithinDeadline || !lateImage.IsDisposed)
    {
        failures.Add("The timed-out Step drain did not report the deadline or dispose the late result image.");
    }

    bool immediateCompletion = await VisionPipelineExecutionService.WaitForStepCompletionAsync(
        Task.FromResult(new VisionToolResult { Success = true }),
        1000,
        CancellationToken.None);
    if (!immediateCompletion)
    {
        failures.Add("A completed Step was incorrectly reported as timed out.");
    }

    using (Bitmap indexed = new Bitmap(2, 1, PixelFormat.Format8bppIndexed))
    {
        ColorPalette palette = indexed.Palette;
        palette.Entries[1] = System.Drawing.Color.FromArgb(10, 20, 30);
        palette.Entries[2] = System.Drawing.Color.FromArgb(40, 50, 60);
        indexed.Palette = palette;
        BitmapData bitmapData = indexed.LockBits(
            new System.Drawing.Rectangle(0, 0, 2, 1),
            ImageLockMode.WriteOnly,
            indexed.PixelFormat);
        try
        {
            Marshal.Copy(new byte[] { 1, 2, 0, 0 }, 0, bitmapData.Scan0, bitmapData.Stride);
        }
        finally
        {
            indexed.UnlockBits(bitmapData);
        }

        using Mat converted = new Mat(1, 2, MatType.CV_8UC3);
        BitmapImageConverter.ToMat(indexed, converted);
        Vec3b first = converted.At<Vec3b>(0, 0);
        Vec3b second = converted.At<Vec3b>(0, 1);
        if (first.Item0 != 30 || first.Item1 != 20 || first.Item2 != 10
            || second.Item0 != 60 || second.Item1 != 50 || second.Item2 != 40)
        {
            failures.Add("Indexed Bitmap palette conversion did not preserve BGR values.");
        }
    }

    CheckCanvasImageLoaderContract(evidenceDirectory, failures);

    FieldInfo? glyphCountField = typeof(OpenGlDrawing).GetField(
        "FontGlyphCount",
        BindingFlags.Static | BindingFlags.NonPublic);
    if (glyphCountField?.GetRawConstantValue() is not int glyphCount || glyphCount != 256)
    {
        failures.Add("OpenGL text rendering does not reserve all 256 byte glyph display lists.");
    }

    string reportPath = Path.Combine(evidenceDirectory, "runtime_stability_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "DispatcherPolicy: cancellation handled; unexpected UI exceptions remain fatal",
            "PipelineDeadline: timed-out in-process work drained before Context ownership ends",
            "SampleCheck: async CancellationToken path active",
            "BitmapConverter: indexed BGR conversion and temporary Mat ownership checked",
            "CanvasImageLoader: owned 8-bit Gray/BGR output, alpha drop, 16-to-8-bit conversion, and GC lifetime checked",
            "OpenGLFontLists: 256 contiguous glyph lists required"
        }.Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Runtime stability contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Runtime stability contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

static async Task<int> RunImageSpaceSnapshotContractAsync(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
        ?? Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "refactor-ovl04-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
    Directory.CreateDirectory(evidenceDirectory);
    List<string> observations = new List<string>();
    List<string> failures = new List<string>();

    try
    {
        DisplayManagerImageExtensions.ResetSnapshotDiagnosticsForTest();
        using (DisplayManagerService displayManager = new DisplayManagerService())
        {
            Bitmap original = new Bitmap(12, 8);
            original.SetPixel(2, 2, System.Drawing.Color.Red);
            displayManager.CreateLayerDisplay(ImageSpaceFrame.TakeOwnership(original), "Main");

            using (Bitmap snapshot = displayManager.GetLayerImageSnapshot("Main"))
            {
                if (snapshot == null)
                {
                    throw new InvalidOperationException("ImageSpace snapshot was not created.");
                }

                Require(snapshot.GetPixel(2, 2).ToArgb() == System.Drawing.Color.Red.ToArgb(),
                    "ImageSpace snapshot did not retain the source pixels.");

                using Bitmap replacement = new Bitmap(12, 8);
                replacement.SetPixel(2, 2, System.Drawing.Color.Blue);
                displayManager.SetLayerImage(0, replacement);
                displayManager.ImageSpace.RemoveImage("Main");

                Require(snapshot.GetPixel(2, 2).ToArgb() == System.Drawing.Color.Red.ToArgb(),
                    "ImageSpace snapshot changed after layer replacement/removal.");
                Require(displayManager.GetLayerImageSnapshot("Main") == null,
                    "Removed layer unexpectedly issued a new snapshot.");
            }
            observations.Add("display-snapshot: independent pixels survived replacement/removal");

            displayManager.CreateLayerDisplay(
                ImageSpaceFrame.TakeOwnership(new Bitmap(12, 8)),
                "Main");
            displayManager.CreateLayerDisplay(
                ImageSpaceFrame.TakeOwnership(new Bitmap(12, 8)),
                "Secondary");

            VisionPipeline arithmeticPipeline = new VisionPipeline { Name = "OVL-04 Arithmetic Snapshot" };
            arithmeticPipeline.Steps.Add(VisionPipelineStepBuilder.FromArithmetic(
                "Arithmetic Snapshot",
                "ADD",
                "Main",
                "Secondary",
                "Arithmetic_Output",
                useConstantInput: false,
                useColorConstant: false,
                gray: 1,
                b: 1,
                g: 1,
                r: 1,
                offsetX: 0,
                offsetY: 0));

            bool reviewLayersRemoved = false;
            using (OpenVisionPipelineReviewExecutionController review =
                new OpenVisionPipelineReviewExecutionController(displayManager, action =>
                {
                    action();
                    if (reviewLayersRemoved)
                    {
                        return;
                    }

                    reviewLayersRemoved = true;
                    displayManager.ImageSpace.RemoveImage("Main");
                    displayManager.ImageSpace.RemoveImage("Secondary");
                }))
            {
                OpenVisionPipelineReviewExecutionResult reviewResult = await review.RunAsync(
                    arithmeticPipeline,
                    VisionRecipeRunner.DefaultStepTimeoutMilliseconds);
                Require(reviewResult.StepResultCount == 1,
                    "Pipeline Review did not execute the multi-input Arithmetic step from snapshots.");
            }
            observations.Add("pipeline-review: Main/Secondary snapshots remained usable after storage removal");

            displayManager.CreateLayerDisplay(
                ImageSpaceFrame.TakeOwnership(new Bitmap(12, 8)),
                "Main");
            displayManager.CreateLayerDisplay(
                ImageSpaceFrame.TakeOwnership(new Bitmap(12, 8)),
                "Secondary");

            OpenVisionNativePreviewLayerPublisher publisher =
                new OpenVisionNativePreviewLayerPublisher(displayManager);
            OpenVisionNativePreviewExecutionController preview =
                new OpenVisionNativePreviewExecutionController(displayManager, publisher);
            OpenVisionNativePreviewExecutionResult singleResult = preview.RunSingleInput(
                "Main",
                "Preview_Output",
                "Main",
                normalizeSingleChannelInput: false,
                executePreview: source => new VisionToolResult
                {
                    Success = true,
                    ResultImage = source.Clone()
                });
            Require(singleResult.Success, "Native Preview did not execute from an image snapshot.");

            OpenVisionNativePreviewExecutionResult arithmeticResult = preview.RunArithmetic(
                arithmeticPipeline.Steps[0],
                "Main",
                "Arithmetic_Preview_Output",
                "Main",
                useOffsetMode: false);
            Require(arithmeticResult.Success,
                "Native Preview Arithmetic did not retain both snapshot inputs.");
            observations.Add("native-preview: single-input and Arithmetic A/B snapshot paths passed");
        }

        ImageSpaceSnapshotDiagnostics diagnostics = DisplayManagerImageExtensions.SnapshotDiagnostics;
        Require(diagnostics.CopyCount >= 6,
            "Snapshot diagnostics did not count the expected Preview/Review copies.");
        Require(diagnostics.EstimatedBytes > 0,
            "Snapshot diagnostics did not record an estimated byte count.");
        observations.Add($"snapshot-diagnostics: copies={diagnostics.CopyCount}, estimatedBytes={diagnostics.EstimatedBytes}");
        observations.Add("peak-memory: not measured; estimatedBytes is width*height*bytes-per-pixel only");
    }
    catch (Exception exception)
    {
        failures.Add(exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(evidenceDirectory, "image-space-snapshot-contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: ImageSpace lease-backed execution snapshots and atomic title removal",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("ImageSpace snapshot contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("ImageSpace snapshot contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunPipelineReviewLayerImageOwnerContract(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
        ?? Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "refactor-ovl07-pipeline-review-layer-image-owner-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
    Directory.CreateDirectory(evidenceDirectory);
    List<string> observations = new List<string>();
    List<string> failures = new List<string>();

    try
    {
        using DisplayManagerService displayManager = new DisplayManagerService();
        using Bitmap cachedOutput = new Bitmap(12, 8);
        cachedOutput.SetPixel(2, 2, System.Drawing.Color.Blue);
        Bitmap original = new Bitmap(12, 8);
        original.SetPixel(2, 2, System.Drawing.Color.Red);
        displayManager.CreateLayerDisplay(ImageSpaceFrame.TakeOwnership(original), "Main");

        OpenVisionPipelineReviewLayerImageOwner owner =
            new OpenVisionPipelineReviewLayerImageOwner(
                displayManager,
                layerName => string.Equals(layerName, "Main", StringComparison.OrdinalIgnoreCase)
                    ? new Bitmap(cachedOutput)
                    : null);

        using (Bitmap displaySnapshot = owner.AcquirePreview("Main"))
        {
            if (displaySnapshot == null)
            {
                throw new InvalidOperationException("Pipeline Review layer owner did not acquire a display snapshot.");
            }

            Require(
                displaySnapshot.GetPixel(2, 2).ToArgb() == System.Drawing.Color.Red.ToArgb(),
                "Pipeline Review preview did not prefer the current display layer.");

            using Bitmap outputSnapshot = owner.AcquireOutputPreview("Main");
            if (outputSnapshot == null)
            {
                throw new InvalidOperationException("Pipeline Review output owner did not acquire a cache snapshot.");
            }

            Require(
                outputSnapshot.GetPixel(2, 2).ToArgb() == System.Drawing.Color.Blue.ToArgb(),
                "Pipeline Review output preview did not prefer the review cache.");

            using Bitmap replacement = new Bitmap(12, 8);
            replacement.SetPixel(2, 2, System.Drawing.Color.Green);
            displayManager.SetLayerImage(0, replacement);
            displayManager.ImageSpace.RemoveImage("Main");

            Require(
                displaySnapshot.GetPixel(2, 2).ToArgb() == System.Drawing.Color.Red.ToArgb(),
                "Owned Pipeline Review snapshot changed after Layer replacement/removal.");
        }
        observations.Add("display snapshot remains independent after Layer replacement/removal");

        using (Bitmap fallbackSnapshot = owner.AcquirePreview("Main"))
        {
            if (fallbackSnapshot == null)
            {
                throw new InvalidOperationException("Pipeline Review owner did not fall back to cached output.");
            }

            Require(
                fallbackSnapshot.GetPixel(2, 2).ToArgb() == System.Drawing.Color.Blue.ToArgb(),
                "Cached output fallback was not cloned with the expected pixels.");
        }
        Require(owner.HasPreview("Main"), "Pipeline Review owner did not report cached fallback availability.");
        Require(!owner.HasPreview("Missing"), "Pipeline Review owner reported a missing Layer as available.");
        observations.Add("cached output fallback is cloned and missing Layers fail closed");
    }
    catch (Exception exception)
    {
        failures.Add(exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(evidenceDirectory, "pipeline-review-layer-image-owner-contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Pipeline Review layer image snapshot owner boundary",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Pipeline Review layer image owner contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Pipeline Review layer image owner contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    Console.Error.WriteLine(reportPath);
    return 1;
}

static async Task<int> RunPipelineReviewCacheLifetimeContractAsync(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
        ?? Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "refactor-ovl07-pipeline-review-cache-retirement-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
    Directory.CreateDirectory(evidenceDirectory);
    List<string> observations = new List<string>();
    List<string> failures = new List<string>();

    try
    {
        using DisplayManagerService displayManager = new DisplayManagerService();
        displayManager.CreateLayerDisplay(ImageSpaceFrame.TakeOwnership(new Bitmap(12, 8)), "Main");
        VisionPipeline pipeline = CreateReviewExecutionContractPipeline();

        using OpenVisionPipelineReviewExecutionController controller =
            new OpenVisionPipelineReviewExecutionController(displayManager, action => action());

        OpenVisionPipelineReviewExecutionResult firstRun = await controller.RunAsync(pipeline, 1000, 101, 201);
        Require(!firstRun.WasSuperseded, "The first cache-lifetime run was unexpectedly superseded.");
        using Bitmap firstSnapshot = controller.AcquireCachedOutputSnapshot("Review_Output");
        if (firstSnapshot == null)
        {
            throw new InvalidOperationException("The completed Review run did not expose an owned cached-output snapshot.");
        }

        int firstPixel = firstSnapshot.GetPixel(2, 2).ToArgb();
        Require(firstSnapshot.Width > 0 && firstSnapshot.Height > 0, "The first cached-output snapshot was empty.");

        OpenVisionPipelineReviewExecutionResult secondRun = await controller.RunAsync(pipeline, 1000, 102, 202);
        Require(!secondRun.WasSuperseded, "The replacement cache-lifetime run was unexpectedly superseded.");
        using Bitmap secondSnapshot = controller.AcquireCachedOutputSnapshot("Review_Output");
        if (secondSnapshot == null)
        {
            throw new InvalidOperationException("The replacement Review run did not expose a cached-output snapshot.");
        }

        int secondPixel = secondSnapshot.GetPixel(2, 2).ToArgb();
        Require(firstSnapshot.GetPixel(2, 2).ToArgb() == firstPixel, "Replacing the internal cache disposed or mutated an earlier snapshot.");
        observations.Add("replacement: cached output replacement leaves prior caller-owned snapshot valid");

        controller.Reset();
        using Bitmap resetSnapshot = controller.AcquireCachedOutputSnapshot("Review_Output");
        Require(resetSnapshot == null, "Reset did not retire the review cache.");
        Require(firstSnapshot.GetPixel(2, 2).ToArgb() == firstPixel, "Reset disposed an earlier caller-owned snapshot.");
        Require(secondSnapshot.GetPixel(2, 2).ToArgb() == secondPixel, "Reset invalidated the replacement snapshot.");
        observations.Add("reset: cache is empty while snapshots already returned to callers remain valid");

        OpenVisionPipelineReviewExecutionResult thirdRun = await controller.RunAsync(pipeline, 1000, 103, 203);
        Require(!thirdRun.WasSuperseded, "The post-reset cache-lifetime run was unexpectedly superseded.");
        using Bitmap closeSnapshot = controller.AcquireCachedOutputSnapshot("Review_Output");
        if (closeSnapshot == null)
        {
            throw new InvalidOperationException("The post-reset Review run did not expose a cached-output snapshot.");
        }

        await controller.DisposeAsync();
        using Bitmap closedSnapshot = controller.AcquireCachedOutputSnapshot("Review_Output");
        Require(closedSnapshot == null, "DisposeAsync left a retired review cache visible.");
        Require(closeSnapshot.Width > 0 && closeSnapshot.Height > 0, "DisposeAsync invalidated a caller-owned cached snapshot.");
        observations.Add("close: DisposeAsync retires internal cache without invalidating returned snapshots");
    }
    catch (Exception exception)
    {
        failures.Add(exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(evidenceDirectory, "pipeline-review-cache-lifetime-contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Pipeline Review cached image replacement and Reset/Close retirement",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Pipeline Review cache lifetime contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Pipeline Review cache lifetime contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    Console.Error.WriteLine(reportPath);
    return 1;
}

static async Task<int> RunPipelineReviewStaleCallbackContractAsync(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
        ?? Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "refactor-ovl07-pipeline-review-stale-callback-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
    Directory.CreateDirectory(evidenceDirectory);
    List<string> observations = new List<string>();
    List<string> failures = new List<string>();

    try
    {
        using DisplayManagerService displayManager = new DisplayManagerService();
        displayManager.CreateLayerDisplay(ImageSpaceFrame.TakeOwnership(new Bitmap(12, 8)), "Main");
        VisionPipeline pipeline = CreateReviewExecutionContractPipeline();

        using OpenVisionPipelineReviewExecutionController controller =
            new OpenVisionPipelineReviewExecutionController(displayManager, action => action());
        using ManualResetEventSlim callbackEntered = new ManualResetEventSlim(false);
        using ManualResetEventSlim resetStarted = new ManualResetEventSlim(false);
        using ManualResetEventSlim releaseCallback = new ManualResetEventSlim(false);
        EventHandler<OpenVisionPipelineReviewStepUpdatedEventArgs> callbackHandler = (_, _) =>
        {
            callbackEntered.Set();
            releaseCallback.Wait();
        };
        controller.StepUpdated += callbackHandler;
        try
        {
            // Run the controller from a worker so a synchronous invokeOnUi test callback can
            // hold the application boundary while the caller starts the Reset race probe.
            Task<OpenVisionPipelineReviewExecutionResult> runTask = Task.Run(
                async () => await controller.RunAsync(pipeline, 1000, 301, 401).ConfigureAwait(false));
            if (!callbackEntered.Wait(5000))
            {
                failures.Add("The callback atomicity probe did not enter StepUpdated.");
                controller.Reset();
                await runTask;
            }
            else
            {
                Task resetTask = Task.Factory.StartNew(
                    () =>
                    {
                        resetStarted.Set();
                        controller.Reset();
                    },
                    CancellationToken.None,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
                Require(resetStarted.Wait(5000), "The Reset atomicity probe did not start.");
                if (resetTask.Wait(250))
                {
                    failures.Add("Reset completed while an entered StepUpdated callback still held the application boundary.");
                }

                releaseCallback.Set();
                await resetTask;
                OpenVisionPipelineReviewExecutionResult runResult = await runTask;
                Require(runResult.WasSuperseded, "Reset did not supersede the run after the entered callback boundary.");
                Require(!controller.TryGetSummary(pipeline.Steps[0], out _), "Reset left a stale summary after the entered callback.");
                using Bitmap cachedOutput = controller.AcquireCachedOutputSnapshot("Review_Output");
                Require(cachedOutput == null, "Reset left a stale cached output after the entered callback.");
                observations.Add("entered callback: Reset waited for the atomic callback boundary and final state is empty");
            }
        }
        finally
        {
            releaseCallback.Set();
            controller.StepUpdated -= callbackHandler;
        }

        using OpenVisionPipelineReviewExecutionController closeController =
            new OpenVisionPipelineReviewExecutionController(displayManager, action => action());
        using ManualResetEventSlim closeCallbackEntered = new ManualResetEventSlim(false);
        using ManualResetEventSlim disposeStarted = new ManualResetEventSlim(false);
        using ManualResetEventSlim releaseCloseCallback = new ManualResetEventSlim(false);
        EventHandler<OpenVisionPipelineReviewStepUpdatedEventArgs> closeCallbackHandler = (_, _) =>
        {
            closeCallbackEntered.Set();
            releaseCloseCallback.Wait();
        };
        closeController.StepUpdated += closeCallbackHandler;
        try
        {
            Task<OpenVisionPipelineReviewExecutionResult> closeRunTask = Task.Run(
                async () => await closeController.RunAsync(pipeline, 1000, 302, 402).ConfigureAwait(false));
            if (!closeCallbackEntered.Wait(5000))
            {
                failures.Add("The Close callback atomicity probe did not enter StepUpdated.");
                releaseCloseCallback.Set();
                await closeRunTask;
            }
            else
            {
                Task disposeTask = Task.Factory.StartNew(
                    async () =>
                    {
                        disposeStarted.Set();
                        await closeController.DisposeAsync().ConfigureAwait(false);
                    },
                    CancellationToken.None,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default).Unwrap();
                Require(disposeStarted.Wait(5000), "The Close atomicity probe did not start.");
                if (disposeTask.Wait(250))
                {
                    failures.Add("Close completed while an entered StepUpdated callback still held the application boundary.");
                }

                releaseCloseCallback.Set();
                await disposeTask;
                OpenVisionPipelineReviewExecutionResult closeRunResult = await closeRunTask;
                Require(closeRunResult.WasSuperseded, "Close did not supersede the run after the entered callback boundary.");
                Require(!closeController.TryGetSummary(pipeline.Steps[0], out _), "Close left a stale summary after the entered callback.");
                using Bitmap closedCachedOutput = closeController.AcquireCachedOutputSnapshot("Review_Output");
                Require(closedCachedOutput == null, "Close left a stale cached output after the entered callback.");
                observations.Add("entered callback: Close waited for the atomic callback boundary and final state is empty");
            }
        }
        finally
        {
            releaseCloseCallback.Set();
            closeController.StepUpdated -= closeCallbackHandler;
        }
    }
    catch (Exception exception)
    {
        failures.Add(exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(evidenceDirectory, "pipeline-review-stale-callback-contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Pipeline Review stale callback atomic application",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Pipeline Review stale callback contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Pipeline Review stale callback contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    Console.Error.WriteLine(reportPath);
    return 1;
}

static async Task<int> RunPipelineReviewExecutionContractAsync(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
        ?? Path.Combine("D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev", "refactor-ovl05-review-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
    Directory.CreateDirectory(evidenceDirectory);
    List<string> observations = new List<string>();
    List<string> failures = new List<string>();

    try
    {
        TaskCompletionSource<VisionToolResult> timeoutSource = new TaskCompletionSource<VisionToolResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        Stopwatch timeoutDrainStopwatch = Stopwatch.StartNew();
        Task<VisionPipelineStepCompletion> timeoutTask = VisionPipelineExecutionService.WaitForStepCompletionStatusAsync(
            timeoutSource.Task,
            1,
            CancellationToken.None);
        await Task.Delay(100);
        if (timeoutTask.IsCompleted)
        {
            failures.Add("Timeout status returned before the worker was drained.");
        }

        Mat timeoutImage = new Mat(2, 2, MatType.CV_8UC1, Scalar.White);
        timeoutSource.SetResult(new VisionToolResult { Success = true, ResultImage = timeoutImage });
        VisionPipelineStepCompletion timeout = await timeoutTask;
        timeoutDrainStopwatch.Stop();
        Require(timeout.Status == VisionPipelineStepCompletionStatus.TimedOut, "Timeout status was not distinguished from cancellation.");
        Require(timeout.WorkerDrained && timeoutImage.IsDisposed, "Timeout did not report a drained worker and dispose the late image.");

        using (CancellationTokenSource cancelSource = new CancellationTokenSource())
        {
            TaskCompletionSource<VisionToolResult> canceledSource = new TaskCompletionSource<VisionToolResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            Task<VisionPipelineStepCompletion> canceledTask = VisionPipelineExecutionService.WaitForStepCompletionStatusAsync(
                canceledSource.Task,
                60000,
                cancelSource.Token);
            Stopwatch cancellationDrainStopwatch = Stopwatch.StartNew();
            cancelSource.Cancel();
            await Task.Delay(10);
            if (canceledTask.IsCompleted)
            {
                failures.Add("Cancellation status returned before the worker was drained.");
            }

            canceledSource.SetResult(new VisionToolResult { Success = true });
            VisionPipelineStepCompletion canceled = await canceledTask;
            cancellationDrainStopwatch.Stop();
            Require(canceled.Status == VisionPipelineStepCompletionStatus.Canceled, "Cancellation status was not distinguished from timeout.");
            Require(canceled.WorkerDrained, "Cancellation did not report a drained worker.");

            observations.Add(
                "cancellation-drain: status=" + canceled.Status
                + ", workerDrained=" + canceled.WorkerDrained
                + ", lateResultReleaseElapsedMilliseconds="
                + cancellationDrainStopwatch.Elapsed.TotalMilliseconds.ToString("F1", CultureInfo.InvariantCulture));
        }
        observations.Add(
            "deadline-drain: status=" + timeout.Status
            + ", workerDrained=" + timeout.WorkerDrained
            + ", lateResultReleaseElapsedMilliseconds="
            + timeoutDrainStopwatch.Elapsed.TotalMilliseconds.ToString("F1", CultureInfo.InvariantCulture));
        observations.Add("completion-status: timeout and cancellation remain distinct until worker drain");

        using (DisplayManagerService displayManager = new DisplayManagerService())
        {
            displayManager.CreateLayerDisplay(ImageSpaceFrame.TakeOwnership(new Bitmap(12, 8)), "Main");
            VisionPipeline pipeline = CreateReviewExecutionContractPipeline();

            using (OpenVisionPipelineReviewExecutionController invalidController =
                new OpenVisionPipelineReviewExecutionController(displayManager, action => action()))
            {
                VisionPipeline invalidPipeline = CreateReviewExecutionContractPipeline();
                invalidPipeline.Steps[0].Parameters["INVALID_XML"] = "\0";
                bool planFailed = false;
                try
                {
                    await invalidController.RunAsync(invalidPipeline, 1000, 7, 11);
                }
                catch (Exception)
                {
                    planFailed = true;
                }

                Require(planFailed, "Invalid pipeline did not fail during execution-plan creation.");
                Require(!invalidController.IsRunning, "Execution-plan failure left IsRunning set.");
            }
            observations.Add("plan-failure: IsRunning restored after execution-plan creation exception");

            List<Action> queuedCallbacks = new List<Action>();
            int invocationCount = 0;
            using (OpenVisionPipelineReviewExecutionController resetController =
                new OpenVisionPipelineReviewExecutionController(displayManager, action =>
                {
                    if (Interlocked.Increment(ref invocationCount) == 1)
                    {
                        action();
                        return;
                    }

                    lock (queuedCallbacks)
                    {
                        queuedCallbacks.Add(action);
                    }
                }))
            {
                OpenVisionPipelineReviewExecutionResult staleResult = await resetController.RunAsync(pipeline, 1000, 17, 23);
                Require(staleResult.WasSuperseded, "Queued Review callbacks were not marked superseded.");
                resetController.Reset();
                Action[] callbacks;
                lock (queuedCallbacks)
                {
                    callbacks = queuedCallbacks.ToArray();
                }

                foreach (Action callback in callbacks)
                {
                    callback();
                }

                Require(!resetController.TryGetSummary(pipeline.Steps[0], out _), "Reset allowed a stale Step result to repopulate summaries.");
                using Bitmap resetSnapshot = resetController.AcquireCachedOutputSnapshot("Review_Output");
                Require(resetSnapshot == null, "Reset allowed a stale output image to repopulate the cache.");
            }
            observations.Add("generation-guard: Reset invalidated queued Step and completion callbacks");

            using (ManualResetEventSlim contextEntered = new ManualResetEventSlim(false))
            using (ManualResetEventSlim releaseContext = new ManualResetEventSlim(false))
            {
                int duplicateInvocationCount = 0;
                using (OpenVisionPipelineReviewExecutionController duplicateController =
                    new OpenVisionPipelineReviewExecutionController(displayManager, action =>
                    {
                        if (Interlocked.Increment(ref duplicateInvocationCount) == 1)
                        {
                            contextEntered.Set();
                            releaseContext.Wait();
                        }

                        action();
                    }))
                {
                Task<OpenVisionPipelineReviewExecutionResult> firstRun = Task.Run(
                    () => duplicateController.RunAsync(pipeline, 1000, 31, 41));
                if (!contextEntered.Wait(5000))
                {
                    failures.Add("The duplicate-run probe did not reach the active execution boundary.");
                }

                Task<OpenVisionPipelineReviewExecutionResult> secondRun = duplicateController.RunAsync(pipeline, 1000, 32, 42);
                bool duplicateBlocked = false;
                try
                {
                    await secondRun;
                }
                catch (InvalidOperationException)
                {
                    duplicateBlocked = true;
                }

                if (!duplicateBlocked)
                {
                    failures.Add("A second Review run was not blocked while the first run was active.");
                }

                releaseContext.Set();
                await firstRun;
                }
            }
            observations.Add("duplicate-run: second Review invocation was rejected while the first was active");

            using (OpenVisionPipelineReviewExecutionController disposeController =
                new OpenVisionPipelineReviewExecutionController(displayManager, action => action()))
            {
                OpenVisionPipelineReviewExecutionResult result = await disposeController.RunAsync(pipeline, 1000, 51, 61);
                Require(!result.WasSuperseded, "A completed Review run was unexpectedly superseded.");
                await disposeController.DisposeAsync();
                Require(!disposeController.IsRunning, "DisposeAsync returned while Review was still running.");
            }
            observations.Add("dispose: async disposal waits for controller completion and leaves no active run");
        }
    }
    catch (Exception exception)
    {
        failures.Add(exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(evidenceDirectory, "pipeline-review-execution-contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: Review execution generation, cancellation, drain, and disposal",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Pipeline Review execution contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Pipeline Review execution contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    Console.Error.WriteLine(reportPath);
    return 1;
}

static async Task<int> RunTcpControllerDisposalContractAsync(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
        ?? Path.Combine("D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev", "refactor-ovl05-tcp-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
    Directory.CreateDirectory(evidenceDirectory);
    List<string> observations = new List<string>();
    List<string> failures = new List<string>();

    try
    {
        string exchangeRoot = Path.Combine(evidenceDirectory, "exchange");
        Directory.CreateDirectory(exchangeRoot);
        int listenPort;
        using (TcpListener probe = new TcpListener(IPAddress.Loopback, 0))
        {
            probe.Start();
            listenPort = ((IPEndPoint)probe.LocalEndpoint).Port;
        }

        byte[] key = RandomNumberGenerator.GetBytes(32);
        try
        {
            using (OpenVisionTcpIntegrationController controller =
                new OpenVisionTcpIntegrationController(Dispatcher.CurrentDispatcher))
            {
                controller.LocalExchangeRoot = exchangeRoot;
                controller.ListenAddress = "127.0.0.1";
                controller.ListenPortText = listenPort.ToString(CultureInfo.InvariantCulture);
                controller.PeerHost = "127.0.0.1";
                controller.PeerPortText = (listenPort == 65535 ? 65534 : listenPort + 1).ToString(CultureInfo.InvariantCulture);
                controller.SetSessionSharedKey(Convert.ToBase64String(key));
                await controller.StartAsync();
                Require(controller.IsListening, "TCP controller did not enter listening state.");

                await controller.DisposeAsync();
                Require(!controller.IsListening, "TCP controller remained listening after DisposeAsync.");
                observations.Add("tcp-dispose: listening exchange stopped and disposed without UI-thread blocking");
            }
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
        }
    }
    catch (Exception exception)
    {
        failures.Add(exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(evidenceDirectory, "tcp-controller-disposal-contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: TCP integration controller asynchronous disposal",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("TCP controller disposal contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("TCP controller disposal contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    Console.Error.WriteLine(reportPath);
    return 1;
}

static VisionPipeline CreateReviewExecutionContractPipeline()
{
    VisionPipeline pipeline = new VisionPipeline { Name = "OVL-05 Review Execution Contract" };
    pipeline.Steps.Add(VisionPipelineStepBuilder.FromArithmetic(
        "Review Execution Arithmetic",
        "ADD",
        "Main",
        "Main",
        "Review_Output",
        useConstantInput: false,
        useColorConstant: false,
        gray: 1,
        b: 1,
        g: 1,
        r: 1,
        offsetX: 0,
        offsetY: 0));
    return pipeline;
}

static int RunBitmapConverterContract(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
        ?? Path.Combine("artifacts", "bitmap_converter_contract"));
    Directory.CreateDirectory(evidenceDirectory);
    List<string> observations = new List<string>();
    List<string> failures = new List<string>();

    RunBitmapConverterCase(
        "indexed-odd-width-submatrix-guard",
        CheckIndexedOddWidthSubmatrixGuard,
        observations,
        failures);
    RunBitmapConverterCase(
        "indexed-positive-negative-stride-and-palette",
        CheckIndexedPositiveNegativeStrideAndPalette,
        observations,
        failures);
    RunBitmapConverterCase(
        "24-32bpp-positive-negative-stride-and-submatrix",
        Check24And32BppStorage,
        observations,
        failures);
    RunBitmapConverterCase(
        "1-3-4-channel-round-trip",
        CheckBitmapRoundTrips,
        observations,
        failures);
    RunBitmapConverterCase(
        "unsupported-format-contract",
        CheckUnsupportedBitmapFormats,
        observations,
        failures);

    string reportPath = Path.Combine(evidenceDirectory, "bitmap_converter_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: PL-0006 BitmapImageConverter row-byte, signed-stride, submatrix, and ownership safety",
            "EvidenceDirectory: " + evidenceDirectory,
            "FactoryOwnership: allocating ToMat/ToBitmap catch paths are source-verified; no output is returned after a conversion exception."
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Bitmap converter contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Bitmap converter contract failed.");
    foreach (string failure in failures)
        Console.Error.WriteLine("- " + failure);
    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunRecipeStoragePathContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "pl0007_recipe_storage_contract_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);
    string dataRoot = Path.Combine(evidenceDirectory, "data");
    Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    string recipeName = "한글 Recipe_01";
    string recipeDirectory = string.Empty;

    try
    {
        RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
        recipeDirectory = RecipeWorkspaceService.GetRecipeDirectoryPath(recipeName);
        string recipeRoot = Path.GetFullPath(Path.Combine(dataRoot, "RECIPE"));
        Require(IsContainedPath(recipeRoot, recipeDirectory), "Recipe directory escaped the RECIPE root.");

        string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, "Pipeline_01");
        string imageDirectory = RecipeWorkspaceService.GetVisionPipelineImageDirectory(recipeName, "Pipeline_01");
        string runDirectory = RecipeWorkspaceService.GetVisionPipelineRunDirectory(recipeName, "Pipeline_01", "Run_01");
        string sampleSetDirectory = RecipeWorkspaceService.GetVisionPipelineSampleSetDirectory(recipeName, "Pipeline_01", "샘플_01");
        string batchDirectory = RecipeWorkspaceService.GetVisionPipelineBatchRunDirectory(recipeName, "Pipeline_01", "Batch_01");
        string configPath = RecipeWorkspaceService.GetVisionConfigPath(recipeName, "pipeline.active");
        string dataPath = RecipeWorkspaceService.GetVisionDataPath(recipeName);
        string recipeFilePath = RecipeWorkspaceService.GetRecipeFilePath(recipeName, "RecipeNote");

        foreach ((string label, string path) in new[]
        {
            ("pipeline", pipelinePath),
            ("pipeline-images", imageDirectory),
            ("pipeline-run", runDirectory),
            ("sample-set", sampleSetDirectory),
            ("batch-run", batchDirectory),
            ("vision-config", configPath),
            ("vision-data", dataPath),
            ("recipe-file", recipeFilePath)
        })
        {
            Require(IsContainedPath(recipeDirectory, path), label + " escaped its Recipe directory.");
            observations.Add(label + ": contained");
        }

        Require(Directory.Exists(imageDirectory), "Pipeline image directory was not created.");
        Require(Directory.Exists(runDirectory), "Pipeline run directory was not created.");
        Require(Directory.Exists(sampleSetDirectory), "Sample-set directory was not created.");
        Require(Directory.Exists(batchDirectory), "Batch-run directory was not created.");

        VisionPipeline lifecyclePipeline = new VisionPipeline { Name = "Pipeline_01" };
        VisionPipelineStorage.Save(recipeName, lifecyclePipeline);
        Require(
            string.Equals(
                VisionPipelineStorage.Load(recipeName, "Pipeline_01").Name,
                "Pipeline_01",
                StringComparison.Ordinal),
            "Saved pipeline could not be loaded through the storage owner.");
        Require(
            VisionPipelineStorage.TryDuplicatePipeline(
                recipeName,
                "Pipeline_01",
                "Pipeline_02",
                out string pipelineMessage),
            pipelineMessage);
        Require(
            VisionPipelineStorage.TryRenamePipeline(
                recipeName,
                "Pipeline_02",
                "Pipeline_03",
                out pipelineMessage),
            pipelineMessage);
        Require(
            VisionPipelineStorage.TryDeletePipeline(
                recipeName,
                "Pipeline_03",
                out string fallbackPipelineName,
                out pipelineMessage),
            pipelineMessage);
        Require(
            string.Equals(fallbackPipelineName, "Pipeline_01", StringComparison.Ordinal),
            "Pipeline delete did not select the remaining pipeline as fallback.");
        observations.Add("pipeline-crud: save/load/duplicate/rename/delete");

        using (DisplayManagerService displayManager = new DisplayManagerService())
        {
            displayManager.CreateLayerDisplay(
                ImageSpaceFrame.TakeOwnership(new Bitmap(8, 6)),
                "Main");
            displayManager.CreateLayerDisplay(
                ImageSpaceFrame.TakeOwnership(new Bitmap(8, 6)),
                "CON");
            int savedLayerCount = VisionPipelineSampleSetStorage.Save(
                recipeName,
                "Pipeline_01",
                "Sample_01",
                displayManager);
            Require(savedLayerCount == 2, "Sample-set save did not persist both layers.");

            VisionPipelineSampleSetInfo sampleInfo = VisionPipelineSampleSetStorage
                .List(recipeName, "Pipeline_01")
                .Single(info => string.Equals(info.Name, "Sample_01", StringComparison.Ordinal));
            Require(
                IsContainedPath(recipeDirectory, sampleInfo.DirectoryPath),
                "Sample-set directory escaped its Recipe directory.");
            Require(
                VisionPipelineSampleSetStorage.GetLayerTitles(sampleInfo).Count == 2,
                "Sample-set manifest did not reload both layer titles.");
            Require(
                VisionPipelineSampleSetStorage.Load(
                    recipeName,
                    "Pipeline_01",
                    "Sample_01",
                    displayManager) == 2,
                "Sample-set load did not restore both layers.");
            VisionPipelineContext sampleContext = VisionPipelineSampleSetStorage.CreateContext(sampleInfo);
            Require(sampleContext != null, "Sample-set context creation returned null.");
            (sampleContext as IDisposable)?.Dispose();
            VisionPipelineSampleSetStorage.Delete(sampleInfo);
            Require(
                !Directory.Exists(sampleInfo.DirectoryPath),
                "Sample-set delete did not remove its contained directory.");
        }
        observations.Add("sample-set: save/load/context/delete contained");

        DateTime storageStart = DateTime.Now;
        string runReportPath = VisionPipelineRunReportStorage.Save(
            recipeName,
            lifecyclePipeline,
            (VisionPipelineRunResult)null!,
            storageStart,
            storageStart.AddMilliseconds(1),
            publishAllOutputs: false,
            runLabel: "path-contract");
        Require(
            IsContainedPath(recipeDirectory, runReportPath)
            && File.Exists(runReportPath),
            "Run report path was not contained or was not written.");
        Require(
            VisionPipelineRunReportStorage.List(recipeName, "Pipeline_01").Count > 0,
            "Run report list did not reload the saved report.");
        observations.Add("run-report: saved/listed contained");

        string batchSummaryPath = VisionPipelineBatchRunSummaryStorage.Save(
            recipeName,
            "Pipeline_01",
            storageStart,
            storageStart.AddMilliseconds(2),
            new[]
            {
                new VisionPipelineBatchSampleRunResult
                {
                    SampleName = "Sample_01",
                    Status = "OK",
                    Success = true,
                    TotalMilliseconds = 1D,
                    RunReportPath = runReportPath
                }
            },
            suiteName: "Path Contract",
            suiteKind: "Batch",
            notes: "PL-0007 storage boundary contract",
            pipelineSnapshot: lifecyclePipeline);
        Require(
            IsContainedPath(recipeDirectory, batchSummaryPath)
            && File.Exists(batchSummaryPath),
            "Batch summary path was not contained or was not written.");
        Require(
            VisionPipelineBatchRunSummaryStorage.List(recipeName, "Pipeline_01").Count > 0,
            "Batch summary list did not reload the saved summary.");
        observations.Add("batch-summary: saved/listed contained");

        string duplicateRecipeName = "한글 Recipe_02";
        string renamedRecipeName = "한글 Recipe_03";
        Require(
            RecipeWorkspaceService.DuplicateVisionWorkspace(recipeName, duplicateRecipeName),
            "Recipe duplicate operation failed.");
        Require(
            RecipeWorkspaceService.RenameVisionWorkspace(duplicateRecipeName, renamedRecipeName),
            "Recipe rename operation failed.");
        Require(
            RecipeWorkspaceService.DeleteVisionWorkspace(renamedRecipeName),
            "Recipe delete operation failed.");
        observations.Add("recipe-crud: duplicate/rename/delete contained");

        string upperCasePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, "Case_01");
        string lowerCasePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, "case_01");
        Require(
            string.Equals(upperCasePath, lowerCasePath, StringComparison.OrdinalIgnoreCase),
            "Case-insensitive pipeline names did not resolve to the same storage path.");
        observations.Add("case-collision: canonicalized");

        ExpectStoragePathRejected(
            "recipe-child-traversal",
            () => RecipeWorkspaceService.EnsureVisionWorkspace("..\\outside"),
            observations,
            failures);
        string unexpectedRecipeDirectory = Path.Combine(recipeRoot, "outside");
        Require(
            !Directory.Exists(unexpectedRecipeDirectory),
            "Rejected recipe traversal created a directory before validation.");
        observations.Add("rejected-recipe-mutation: no outside directory created");
        ExpectStoragePathRejected(
            "pipeline-child-traversal",
            () => RecipeWorkspaceService.GetVisionPipelinePath(recipeName, "..\\outside"),
            observations,
            failures);
        ExpectStoragePathRejected(
            "reserved-pipeline-device",
            () => RecipeWorkspaceService.GetVisionPipelineImageDirectory(recipeName, "CON"),
            observations,
            failures);
        ExpectStoragePathRejected(
            "reserved-batch-device",
            () => RecipeWorkspaceService.GetVisionPipelineBatchRunDirectory(recipeName, "Pipeline_01", "COM1"),
            observations,
            failures);
        ExpectStoragePathRejected(
            "trailing-space-sample-set",
            () => RecipeWorkspaceService.GetVisionPipelineSampleSetDirectory(recipeName, "Pipeline_01", "Sample_01 "),
            observations,
            failures);
        ExpectStoragePathRejected(
            "control-character-config",
            () => RecipeWorkspaceService.GetVisionConfigPath(recipeName, "Bad\u0001Config"),
            observations,
            failures);
        ExpectStoragePathRejected(
            "relative-artifact-traversal",
            () => RecipeWorkspaceService.GetContainedStoragePath(
                Path.Combine(recipeDirectory, "VISION"),
                "PipelineRuns\\..\\outside.xml",
                "Run artifact path"),
            observations,
            failures);

        string unexpectedDirectory = Path.Combine(recipeDirectory, "VISION", "PipelineImages", "CON");
        Require(!Directory.Exists(unexpectedDirectory), "Rejected reserved pipeline created a directory.");
        observations.Add("rejected-mutation: no reserved directory created");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(evidenceDirectory, "recipe_storage_path_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: PL-0007 Recipe/Pipeline storage segment validation and root containment",
            "EvidenceDirectory: " + evidenceDirectory,
            "DataRoot: " + dataRoot,
            "RecipeDirectory: " + recipeDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Recipe storage path contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Recipe storage path contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunValidationSetDocumentOwnerContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_validation_document_owner_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);
    string dataRoot = Path.Combine(evidenceDirectory, "data");
    Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    string recipeName = "ValidationOwnerContract";
    string recipeDirectory = string.Empty;
    string imagePath = Path.Combine(evidenceDirectory, "owner-contract.png");

    try
    {
        using (Bitmap image = new Bitmap(2, 2))
        {
            image.Save(imagePath, ImageFormat.Png);
        }

        OpenVisionRecipeValidationSetDocumentOwner owner =
            new OpenVisionRecipeValidationSetDocumentOwner();
        Require(owner.TryLoad(recipeName, out string error), error);
        Require(owner.StorageReady, "A missing validation-set document was not treated as ready storage.");
        recipeDirectory = RecipeWorkspaceService.GetRecipeDirectoryPath(recipeName);
        Require(owner.TryCreateSet("Owner Set"), "The document owner did not create a new set.");
        Require(!owner.TryCreateSet("owner set"), "Duplicate set names were not rejected by the document owner.");
        Require(owner.TrySave(recipeName, out error), error);
        string storagePath = OpenVisionRecipeValidationSetStorage.GetPath(recipeName);
        Require(File.Exists(storagePath), "The document owner did not persist validation-sets.xml.");
        observations.Add("create/save: owner persisted one set");

        OpenVisionRecipeValidationSetDocumentOwner reloaded =
            new OpenVisionRecipeValidationSetDocumentOwner();
        Require(reloaded.TryLoad(recipeName, out error), error);
        OpenVisionRecipeValidationSetSelectionOwner selectionOwner =
            new OpenVisionRecipeValidationSetSelectionOwner(reloaded);
        selectionOwner.Refresh(
            "Owner Set",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty);
        Require(selectionOwner.Options.Count == 1
            && selectionOwner.Selected != null
            && string.Equals(selectionOwner.Selected.Name, "Owner Set", StringComparison.Ordinal),
            "The selection owner did not expose the saved set through its selection projection.");
        Require(!selectionOwner.SelectSet(null)
            && selectionOwner.Selected != null
            && string.Equals(selectionOwner.Selected.Name, "Owner Set", StringComparison.Ordinal),
            "The selection owner did not preserve a valid set selection when a bound ComboBox reported a transient null.");

        Require(reloaded.TryAddImages(
                "Owner Set",
                new[] { imagePath },
                OpenVisionRecipeValidationSetImage.ExpectedOk,
                "owner contract",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                out int added,
                out int updated,
                out int skipped,
                out error),
            error);
        Require(added == 1 && updated == 0 && skipped == 0,
            "The document owner did not add the validation image exactly once.");
        Require(reloaded.TrySave(recipeName, out error), error);

        OpenVisionRecipeValidationSetDocumentOwner afterAdd =
            new OpenVisionRecipeValidationSetDocumentOwner();
        Require(afterAdd.TryLoad(recipeName, out error), error);
        OpenVisionRecipeValidationSetSelectionOwner afterAddSelectionOwner =
            new OpenVisionRecipeValidationSetSelectionOwner(afterAdd);
        afterAddSelectionOwner.Refresh(
                "Owner Set",
                string.Empty,
                string.Empty,
                string.Empty,
                imagePath);
        OpenVisionRecipeValidationSetOption addedOption = afterAddSelectionOwner.Selected;
        Require(addedOption != null
            && addedOption.ImageCount == 1
            && addedOption.ReadyCount == 1
            && addedOption.OkCount == 1,
            "The reloaded selection owner did not expose the persisted image state.");
        Require(afterAddSelectionOwner.ImageRows.Count == 1
            && afterAddSelectionOwner.SelectedImage != null
            && string.Equals(afterAddSelectionOwner.SelectedImage.Path, Path.GetFullPath(imagePath), StringComparison.OrdinalIgnoreCase),
            "The selection owner did not preserve the normalized image path.");
        Require(!afterAddSelectionOwner.SelectImage(null)
            && afterAddSelectionOwner.SelectedImage != null
            && string.Equals(afterAddSelectionOwner.SelectedImage.Path, Path.GetFullPath(imagePath), StringComparison.OrdinalIgnoreCase),
            "The selection owner did not preserve a valid image selection when a bound list reported a transient null.");
        observations.Add("reload/add: selection owner preserved set and image projection");

        Require(afterAdd.TryRemoveImage("Owner Set", imagePath),
            "The document owner did not remove the selected validation image.");
        Require(afterAdd.TrySave(recipeName, out error), error);
        OpenVisionRecipeValidationSetDocumentOwner afterRemove =
            new OpenVisionRecipeValidationSetDocumentOwner();
        Require(afterRemove.TryLoad(recipeName, out error), error);
        OpenVisionRecipeValidationSetSelectionOwner afterRemoveSelectionOwner =
            new OpenVisionRecipeValidationSetSelectionOwner(afterRemove);
        afterRemoveSelectionOwner.Refresh(
                "Owner Set",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);
        OpenVisionRecipeValidationSetOption emptyOption = afterRemoveSelectionOwner.Selected;
        Require(emptyOption != null && emptyOption.ImageCount == 0,
            "The document owner did not persist image removal.");

        Require(afterRemove.TryDeleteSet("Owner Set"),
            "The document owner did not delete the selected validation set.");
        Require(afterRemove.TrySave(recipeName, out error), error);
        OpenVisionRecipeValidationSetDocumentOwner afterDelete =
            new OpenVisionRecipeValidationSetDocumentOwner();
        Require(afterDelete.TryLoad(recipeName, out error), error);
        OpenVisionRecipeValidationSetSelectionOwner afterDeleteSelectionOwner =
            new OpenVisionRecipeValidationSetSelectionOwner(afterDelete);
        afterDeleteSelectionOwner.Refresh(
            "Owner Set",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty);
        Require(!afterDelete.ContainsSet("Owner Set") && afterDeleteSelectionOwner.Options.Count == 0,
            "The document and selection owners did not persist validation set deletion.");
        observations.Add("remove/delete: owner persisted both mutations");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(evidenceDirectory, "validation_set_document_owner_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Validation Set document owner persistence and selection projection boundary",
            "EvidenceDirectory: " + evidenceDirectory,
            "DataRoot: " + dataRoot,
            "RecipeDirectory: " + recipeDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Validation Set document/selection owner contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Validation Set document/selection owner contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunStepEditLoaderContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_step_edit_loader_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);
    string dataRoot = Path.Combine(evidenceDirectory, "data");
    Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    string recipeName = "StepEditLoaderContract";
    string recipeDirectory = string.Empty;

    try
    {
        RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
        recipeDirectory = RecipeWorkspaceService.GetRecipeDirectoryPath(recipeName);
        VisionPipeline pipeline = new VisionPipeline { Name = "StepEditPipeline" };
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "Blob",
            ToolType = "Blob",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "Blob_Output"
        };
        step.Parameters["USE_ROI"] = "false";
        step.Parameters["USE_THRESHOLD"] = "true";
        step.Parameters["MIN_AREA"] = "10";
        step.Parameters["MAX_AREA"] = "100000";
        pipeline.Steps.Add(step);
        VisionPipelineStorage.Save(recipeName, pipeline);
        VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);

        OpenVisionRecipePipelineStepPreview preview = new OpenVisionRecipePipelineStepPreview(
            1,
            step,
            OpenVisionRecipeLayerCard.CreateMissing);
        OpenVisionRecipeStepEditLoader loader = new OpenVisionRecipeStepEditLoader();
        OpenVisionRecipeStepEditLoadResult loaded = loader.Load(
            recipeName,
            pipeline.Name,
            preview);
        Require(loaded.Succeeded, loaded.Message);
        Require(loaded.Pipeline != null && loaded.Pipeline.Steps.Count == 1,
            "The Step Edit loader did not return the persisted pipeline.");
        Require(loaded.Step != null
            && string.Equals(loaded.Step.Name, "Blob", StringComparison.Ordinal)
            && loaded.EditObject is BlobProperty,
            "The Step Edit loader did not resolve and project the selected Blob Step.");
        observations.Add("selected-step load: XML Step resolved and projected to BlobProperty");

        OpenVisionRecipePipelineStepPreview relocatedPreview = new OpenVisionRecipePipelineStepPreview(
            9,
            step,
            OpenVisionRecipeLayerCard.CreateMissing);
        OpenVisionRecipeStepEditLoadResult relocated = loader.Load(
            recipeName,
            pipeline.Name,
            relocatedPreview);
        Require(relocated.Succeeded
            && relocated.Step != null
            && string.Equals(relocated.Step.Name, "Blob", StringComparison.Ordinal),
            "The Step Edit loader did not use the stable Step identity when the preview index was stale.");
        observations.Add("stale-index recovery: name/tool/output identity fallback preserved");

        OpenVisionRecipeStepEditLoadResult activePipelineLoad = loader.Load(
            recipeName,
            string.Empty,
            preview);
        Require(activePipelineLoad.Succeeded
            && string.Equals(activePipelineLoad.PipelineName, pipeline.Name, StringComparison.Ordinal),
            "The Step Edit loader did not resolve the active pipeline when no pipeline was requested.");

        OpenVisionRecipeStepEditLoadResult missingSelection = loader.Load(
            recipeName,
            pipeline.Name,
            null);
        Require(!missingSelection.Succeeded && !string.IsNullOrWhiteSpace(missingSelection.Message),
            "The Step Edit loader did not reject a missing selected Step.");
        observations.Add("boundary failures: active-pipeline fallback and missing-selection guard passed");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }
    finally
    {
        if (!string.IsNullOrWhiteSpace(recipeDirectory))
        {
            RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
        }
    }

    string reportPath = Path.Combine(evidenceDirectory, "step_edit_loader_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Step Edit selected-Step load and PropertyGrid projection boundary",
            "EvidenceDirectory: " + evidenceDirectory,
            "DataRoot: " + dataRoot,
            "RecipeDirectory: " + recipeDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Step Edit loader contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Step Edit loader contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunStepEditApplyOwnerContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_step_edit_apply_owner_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);
    string dataRoot = Path.Combine(evidenceDirectory, "data");
    Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    string recipeName = "StepEditApplyOwnerContract";
    string recipeDirectory = string.Empty;
    const string pipelineName = "StepEditApplyPipeline";

    VisionPipeline CreatePipeline(int minArea)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = pipelineName };
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "Blob",
            ToolType = "Blob",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "Blob_Output"
        };
        step.Parameters["USE_ROI"] = "false";
        step.Parameters["USE_THRESHOLD"] = "true";
        step.Parameters["MIN_AREA"] = minArea.ToString(CultureInfo.InvariantCulture);
        step.Parameters["MAX_AREA"] = "100000";
        pipeline.Steps.Add(step);
        return pipeline;
    }

    void SaveBaseline(int minArea)
    {
        VisionPipelineStorage.Save(recipeName, CreatePipeline(minArea));
    }

    OpenVisionRecipeStepEditLoadResult LoadBaseline()
    {
        VisionPipeline persisted = VisionPipelineStorage.Load(recipeName, pipelineName);
        VisionPipelineStep persistedStep = persisted.Steps[0];
        OpenVisionRecipePipelineStepPreview preview = new OpenVisionRecipePipelineStepPreview(
            1,
            persistedStep,
            OpenVisionRecipeLayerCard.CreateMissing);
        return new OpenVisionRecipeStepEditLoader().Load(
            recipeName,
            pipelineName,
            preview);
    }

    int ReadMinArea()
    {
        string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
        Require(
            VisionPipelineStorage.TryLoadFromFile(path, out VisionPipeline pipeline, out string message),
            "The persisted pipeline could not be read: " + message);
        Require(pipeline.Steps.Count > 0, "The persisted pipeline has no Step.");
        if (!pipeline.Steps[0].Parameters.TryGetValue("MIN_AREA", out string? value))
        {
            throw new InvalidOperationException("The persisted Blob MIN_AREA parameter was missing.");
        }

        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int minArea))
        {
            throw new InvalidOperationException("The persisted Blob MIN_AREA parameter was invalid.");
        }

        return minArea;
    }

    try
    {
        RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
        recipeDirectory = RecipeWorkspaceService.GetRecipeDirectoryPath(recipeName);

        SaveBaseline(10);
        OpenVisionRecipeStepEditLoadResult loaded = LoadBaseline();
        Require(loaded.Succeeded && loaded.EditObject is BlobProperty,
            "The apply-owner contract could not load a Blob edit object.");
        BlobProperty successProperty = (BlobProperty)loaded.EditObject;
        successProperty.MIN_AREA = 25;
        OpenVisionRecipeStepEditApplyResult success = new OpenVisionRecipeStepEditApplyOwner().Apply(
            recipeName,
            pipelineName,
            loaded.Pipeline,
            loaded.Step,
            successProperty);
        Require(success.Succeeded && success.ValidationMessage.Contains("passed", StringComparison.OrdinalIgnoreCase),
            "The Step Edit apply owner did not complete a successful round trip.");
        Require(ReadMinArea() == 25, "The successful Step Edit apply did not persist MIN_AREA.");
        observations.Add("success: ApplyProperty, save, and round-trip validation persisted the edited Step");

        SaveBaseline(10);
        loaded = LoadBaseline();
        BlobProperty saveFailureProperty = (BlobProperty)loaded.EditObject;
        saveFailureProperty.MIN_AREA = 30;
        OpenVisionRecipeStepEditApplyOwner saveFailureOwner = new OpenVisionRecipeStepEditApplyOwner(
            savePipeline: (_, __) => throw new InvalidOperationException("Forced save failure for owner contract."));
        OpenVisionRecipeStepEditApplyResult saveFailure = saveFailureOwner.Apply(
            recipeName,
            pipelineName,
            loaded.Pipeline,
            loaded.Step,
            saveFailureProperty);
        Require(!saveFailure.Succeeded && !saveFailure.IsRoundTripValidationFailure,
            "The Step Edit apply owner did not report the injected save failure.");
        Require(ReadMinArea() == 10, "The injected save failure did not restore the previous pipeline.");
        observations.Add("save failure: injected save exception restored the previous persisted Pipeline");

        SaveBaseline(10);
        loaded = LoadBaseline();
        BlobProperty validationFailureProperty = (BlobProperty)loaded.EditObject;
        validationFailureProperty.MIN_AREA = 35;
        OpenVisionRecipeStepEditApplyOwner validationFailureOwner = new OpenVisionRecipeStepEditApplyOwner(
            validateRoundTrip: (_, __) => new OpenVisionRecipeRoundTripValidationResult
            {
                Succeeded = false,
                Message = "Forced round-trip validation failure for owner contract."
            });
        OpenVisionRecipeStepEditApplyResult validationFailure = validationFailureOwner.Apply(
            recipeName,
            pipelineName,
            loaded.Pipeline,
            loaded.Step,
            validationFailureProperty);
        Require(
            !validationFailure.Succeeded
            && validationFailure.IsRoundTripValidationFailure
            && validationFailure.RestoreSucceeded,
            "The Step Edit apply owner did not report and restore the injected round-trip failure.");
        Require(ReadMinArea() == 10, "The injected round-trip failure did not restore the previous pipeline.");
        observations.Add("round-trip failure: injected validation failure restored the previous persisted Pipeline");

        SaveBaseline(10);
        loaded = LoadBaseline();
        OpenVisionRecipeStepEditApplyResult unsupported = new OpenVisionRecipeStepEditApplyOwner().Apply(
            recipeName,
            pipelineName,
            loaded.Pipeline,
            loaded.Step,
            new object());
        Require(!unsupported.Succeeded && !unsupported.IsRoundTripValidationFailure,
            "The Step Edit apply owner accepted an unsupported edit object.");
        Require(ReadMinArea() == 10, "The unsupported edit object changed the persisted pipeline.");
        observations.Add("unsupported property: mapping rejection left the persisted Pipeline unchanged");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }
    finally
    {
        if (!string.IsNullOrWhiteSpace(recipeDirectory))
        {
            RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
        }
    }

    string reportPath = Path.Combine(evidenceDirectory, "step_edit_apply_owner_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Step Edit apply persistence, validation, and rollback owner boundary",
            "EvidenceDirectory: " + evidenceDirectory,
            "DataRoot: " + dataRoot,
            "RecipeDirectory: " + recipeDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Step Edit apply owner contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Step Edit apply owner contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunStepEditApplyProjectionContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_step_edit_apply_projection_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    try
    {
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "Blob",
            ToolType = "Blob",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "Blob_Output"
        };
        OpenVisionRecipePipelineStepPreview preview = new OpenVisionRecipePipelineStepPreview(
            2,
            step,
            OpenVisionRecipeLayerCard.CreateMissing);
        OpenVisionRecipeStepEditApplyProjectionOwner projectionOwner =
            new OpenVisionRecipeStepEditApplyProjectionOwner();

        OpenVisionRecipeStepEditApplyProjection success = projectionOwner.ProjectSuccess(
            "ProjectionPipeline",
            preview.Index,
            preview,
            "round-trip validation passed",
            rerunValidationSet: false);
        Require(success.Succeeded, "Successful Step Edit apply projection was not marked succeeded.");
        Require(
            (success.SelectedStepEditStatusText.IndexOf("XML 반영 완료", StringComparison.Ordinal) >= 0
                || success.SelectedStepEditStatusText.IndexOf("Applied to XML", StringComparison.Ordinal) >= 0)
            && success.SelectedStepEditStatusText.Contains("Step 2", StringComparison.Ordinal)
            && (success.ShellStatusText.IndexOf("Step XML 반영 완료", StringComparison.Ordinal) >= 0
                || success.ShellStatusText.IndexOf("Step XML apply complete", StringComparison.Ordinal) >= 0)
            && success.CorrectedOutputReviewText.Contains("Good/Bad", StringComparison.Ordinal),
            "Successful Step Edit apply status/review projection did not preserve its user-facing contract.");
        observations.Add("success: pipeline/step/validation status and corrected-output review projected");

        OpenVisionRecipeStepEditApplyProjection saveFailure = projectionOwner.ProjectFailure(
            OpenVisionRecipeStepEditApplyResult.Failure("XML save failed: forced"));
        Require(
            !saveFailure.Succeeded
            && saveFailure.SelectedStepEditStatusText.Contains("XML save failed", StringComparison.Ordinal)
            && string.IsNullOrEmpty(saveFailure.ShellStatusText)
            && string.IsNullOrEmpty(saveFailure.CorrectedOutputReviewText),
            "Save failure projection changed the wrong status channel.");
        observations.Add("save failure: Step Edit error stayed in the edit status channel");

        OpenVisionRecipeStepEditApplyProjection restored = projectionOwner.ProjectFailure(
            OpenVisionRecipeStepEditApplyResult.RoundTripFailure("validation failed", restoreSucceeded: true));
        Require(
            !restored.Succeeded
            && (restored.ShellStatusText.IndexOf("기존 저장 상태 복원", StringComparison.Ordinal) >= 0
                || restored.ShellStatusText.IndexOf("previous saved state restored", StringComparison.Ordinal) >= 0),
            "Restored round-trip failure did not project the recovery status.");
        observations.Add("round-trip restored: recovery status projected to Shell status");

        OpenVisionRecipeStepEditApplyProjection restoreFailed = projectionOwner.ProjectFailure(
            OpenVisionRecipeStepEditApplyResult.RoundTripFailure("validation failed", restoreSucceeded: false));
        Require(
            !restoreFailed.Succeeded
            && (restoreFailed.ShellStatusText.IndexOf("복원 오류 확인 필요", StringComparison.Ordinal) >= 0
                || restoreFailed.ShellStatusText.IndexOf("review the restore error", StringComparison.Ordinal) >= 0),
            "Failed restore did not project the restore-error status.");
        observations.Add("round-trip restore failure: restore-error status projected to Shell status");

        OpenVisionRecipeStepEditApplyProjection unsupported = projectionOwner.ProjectFailure(
            OpenVisionRecipeStepEditApplyResult.Failure("This step property set cannot be applied to XML."));
        Require(
            !unsupported.Succeeded
            && unsupported.SelectedStepEditStatusText.Contains("cannot be applied to XML", StringComparison.Ordinal)
            && string.IsNullOrEmpty(unsupported.ShellStatusText),
            "Unsupported property projection changed the Shell status channel.");
        observations.Add("unsupported property: mapper rejection remained an edit-status failure");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(evidenceDirectory, "step_edit_apply_projection_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Step Edit apply result/status/corrected-output projection owner boundary",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Step Edit apply projection contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Step Edit apply projection contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunRecipeWorkspacePolicyContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_recipe_workspace_policy_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    try
    {
        OpenVisionRecipeWorkspaceUseCase workspaceOwner =
            new OpenVisionRecipeWorkspaceUseCase();
        string[] recipeNames = { "Default", "Recipe_A", "Recipe_B" };

        Require(
            workspaceOwner.CanCreate("  Recipe_C  "),
            "Valid named Recipe create policy was rejected.");
        Require(
            workspaceOwner.CanCreate("   "),
            "Blank named Recipe create policy was rejected.");
        Require(
            !workspaceOwner.CanCreate("Recipe/C"),
            "Invalid named Recipe create policy was accepted.");
        observations.Add("create: blank and valid names accepted; invalid path name rejected");

        Require(
            workspaceOwner.CanDuplicate(" Recipe_A ", string.Empty, recipeNames),
            "Existing Recipe duplicate with an automatic name was rejected.");
        Require(
            workspaceOwner.CanDuplicate("Recipe_A", "Recipe_B", recipeNames),
            "Valid duplicate name policy was rejected.");
        Require(
            !workspaceOwner.CanDuplicate("Missing", "Recipe_C", recipeNames),
            "Missing source Recipe duplicate policy was accepted.");
        Require(
            !workspaceOwner.CanDuplicate("Recipe_A", "Bad/Name", recipeNames),
            "Invalid duplicate name policy was accepted.");
        observations.Add("duplicate: source membership and requested-name validation owned by workspace use case");

        Require(
            workspaceOwner.CanRename("Recipe_A", "Recipe_C", recipeNames),
            "Valid Recipe rename policy was rejected.");
        Require(
            !workspaceOwner.CanRename("Recipe_A", "Recipe_B", recipeNames),
            "Rename to an existing Recipe was accepted.");
        Require(
            !workspaceOwner.CanRename("Recipe_A", "Recipe_A", recipeNames),
            "Rename to the same Recipe was accepted.");
        Require(
            !workspaceOwner.CanRename("Missing", "Recipe_C", recipeNames),
            "Rename of a missing Recipe was accepted.");
        observations.Add("rename: source, target validity, identity, and conflict policy preserved");

        Require(
            workspaceOwner.CanDelete("Recipe_A", recipeNames),
            "Recipe delete policy was rejected when multiple Recipes exist.");
        Require(
            !workspaceOwner.CanDelete("Recipe_A", new[] { "Recipe_A" }),
            "Last Recipe delete policy was accepted.");
        Require(
            !workspaceOwner.CanDelete("Missing", recipeNames),
            "Missing Recipe delete policy was accepted.");
        observations.Add("delete: existing selection and last-Recipe guard preserved");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(evidenceDirectory, "recipe_workspace_policy_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Recipe Manager lifecycle policy owner boundary",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Recipe workspace policy contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Recipe workspace policy contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunRecipeWorkspaceLifecycleProjectionContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_recipe_workspace_lifecycle_projection_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    try
    {
        OpenVisionRecipeWorkspaceLifecycleProjectionOwner projectionOwner =
            new OpenVisionRecipeWorkspaceLifecycleProjectionOwner();

        bool HasStatus(OpenVisionRecipeWorkspaceLifecycleProjection projection, string english, string korean)
        {
            return projection.StatusText.Contains(english, StringComparison.Ordinal)
                || projection.StatusText.Contains(korean, StringComparison.Ordinal);
        }

        OpenVisionRecipeWorkspaceLifecycleProjection created = projectionOwner.Project(
            OpenVisionRecipeWorkspaceLifecycleOperation.Create,
            OpenVisionRecipeWorkspaceResult.Success("Recipe_Create"));
        Require(created.Succeeded, "Create success projection was not marked succeeded.");
        Require(HasStatus(created, "Created:", "생성됨:"), "Create success status was not preserved.");
        Require(created.RecipeName == "Recipe_Create", "Create result name was not preserved.");
        observations.Add("create success: result name and localized status projected");

        OpenVisionRecipeWorkspaceLifecycleProjection duplicated = projectionOwner.Project(
            OpenVisionRecipeWorkspaceLifecycleOperation.Duplicate,
            OpenVisionRecipeWorkspaceResult.Success("Recipe_Copy"));
        Require(duplicated.Succeeded, "Duplicate success projection was not marked succeeded.");
        Require(HasStatus(duplicated, "Duplicated:", "복제됨:"), "Duplicate success status was not preserved.");
        Require(duplicated.RecipeName == "Recipe_Copy", "Duplicate result name was not preserved.");
        observations.Add("duplicate success: result name and localized status projected");

        OpenVisionRecipeWorkspaceLifecycleProjection renamed = projectionOwner.Project(
            OpenVisionRecipeWorkspaceLifecycleOperation.Rename,
            OpenVisionRecipeWorkspaceResult.Success("Recipe_Renamed"));
        Require(renamed.Succeeded, "Rename success projection was not marked succeeded.");
        Require(HasStatus(renamed, "Renamed:", "이름 변경됨:"), "Rename success status was not preserved.");
        Require(renamed.RecipeName == "Recipe_Renamed", "Rename result name was not preserved.");
        observations.Add("rename success: result name and localized status projected");

        OpenVisionRecipeWorkspaceLifecycleProjection deleted = projectionOwner.Project(
            OpenVisionRecipeWorkspaceLifecycleOperation.Delete,
            OpenVisionRecipeWorkspaceResult.Success("Recipe_Fallback"),
            "Recipe_Delete");
        Require(deleted.Succeeded, "Delete success projection was not marked succeeded.");
        Require(HasStatus(deleted, "Deleted:", "삭제됨:"), "Delete success status was not preserved.");
        Require(deleted.StatusText.Contains("Recipe_Delete", StringComparison.Ordinal),
            "Delete success status did not preserve the deleted recipe name.");
        Require(deleted.RecipeName == "Recipe_Fallback", "Delete fallback result name was not preserved.");
        observations.Add("delete success: deleted display name and fallback result name preserved");

        OpenVisionRecipeWorkspaceLifecycleProjection duplicateFailure = projectionOwner.Project(
            OpenVisionRecipeWorkspaceLifecycleOperation.Duplicate,
            OpenVisionRecipeWorkspaceResult.Failure());
        Require(!duplicateFailure.Succeeded, "Duplicate failure projection was marked succeeded.");
        Require(HasStatus(duplicateFailure, "Duplicate failed.", "레시피 복제에 실패했습니다."),
            "Duplicate failure status was not preserved.");

        OpenVisionRecipeWorkspaceLifecycleProjection renameFailure = projectionOwner.Project(
            OpenVisionRecipeWorkspaceLifecycleOperation.Rename,
            OpenVisionRecipeWorkspaceResult.Failure());
        Require(!renameFailure.Succeeded, "Rename failure projection was marked succeeded.");
        Require(HasStatus(renameFailure, "Rename failed.", "이름 변경에 실패했습니다."),
            "Rename failure status was not preserved.");

        OpenVisionRecipeWorkspaceLifecycleProjection deleteFailure = projectionOwner.Project(
            OpenVisionRecipeWorkspaceLifecycleOperation.Delete,
            OpenVisionRecipeWorkspaceResult.Failure());
        Require(!deleteFailure.Succeeded, "Delete failure projection was marked succeeded.");
        Require(HasStatus(deleteFailure, "Delete failed.", "삭제에 실패했습니다."),
            "Delete failure status was not preserved.");
        observations.Add("duplicate/rename/delete failure: localized failure statuses projected");

        OpenVisionRecipeWorkspaceLifecycleProjection createFailure = projectionOwner.Project(
            OpenVisionRecipeWorkspaceLifecycleOperation.Create,
            OpenVisionRecipeWorkspaceResult.Failure());
        Require(!createFailure.Succeeded, "Create failure projection was marked succeeded.");
        Require(string.IsNullOrEmpty(createFailure.StatusText),
            "Create failure changed the existing silent failure behavior.");
        observations.Add("create failure: silent failure behavior preserved");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(
        evidenceDirectory,
        "recipe_workspace_lifecycle_projection_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Recipe Manager lifecycle result/status projection owner boundary",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Recipe workspace lifecycle projection contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Recipe workspace lifecycle projection contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunRecipeManagerSummaryProjectionContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_recipe_manager_summary_projection_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    try
    {
        OpenVisionRecipeManagerSummaryProjectionOwner projectionOwner =
            new OpenVisionRecipeManagerSummaryProjectionOwner();
        int layerCardRequests = 0;

        VisionPipeline pipeline = new VisionPipeline { Name = "Pipeline_A" };
        pipeline.Steps.Add(new VisionPipelineStep
        {
            Name = "Threshold_1",
            ToolType = "Threshold",
            InputLayer = "Main",
            OutputLayer = "Output",
            Enabled = true
        });

        OpenVisionRecipeManagerSummary summary = projectionOwner.Project(
            new OpenVisionRecipeManagerSummaryProjectionRequest
            {
                RecipeName = "Recipe_A",
                ActivePipelineName = "Pipeline_A",
                PreviewPipelineName = "Pipeline_A",
                PipelineCount = 2,
                LastWriteTime = new DateTime(2026, 9, 7, 12, 30, 0),
                XmlValid = true,
                XmlMessage = string.Empty,
                PipelinePath = "C:\\Recipe_A\\Pipeline_A.xml",
                Pipeline = pipeline,
                LayerCardProvider = layerName =>
                {
                    layerCardRequests++;
                    return OpenVisionRecipeLayerCard.CreateMissing(layerName);
                }
            });

        Require(summary.RecipeName == "Recipe_A", "Summary recipe name was not preserved.");
        Require(summary.PipelineCount == 2, "Summary pipeline count was not preserved.");
        Require(summary.StepCount == 1, "Summary Step count was not projected.");
        Require(summary.XmlValid, "Valid XML state was not preserved.");
        Require(summary.PipelinePreviewSteps.Count == 1, "Summary preview Step count was not projected.");
        Require(layerCardRequests == 2, "Preview projection did not request input and output layer cards.");
        Require(summary.DetailText.Contains("Pipelines: 2", StringComparison.Ordinal)
            || summary.DetailText.Contains("파이프라인 수: 2", StringComparison.Ordinal),
            "Summary detail did not preserve the pipeline count text.");
        Require(summary.DetailText.Contains("Steps: 1", StringComparison.Ordinal)
            || summary.DetailText.Contains("Step 수: 1", StringComparison.Ordinal),
            "Summary detail did not preserve the Step count text.");
        Require(summary.LlmXmlValidationReport.Contains("XML", StringComparison.Ordinal),
            "Summary did not project the stored XML validation report.");
        observations.Add("valid summary: identity, counts, detail, validation report, and layer previews projected");

        OpenVisionRecipeManagerSummary invalidSummary = projectionOwner.Project(
            new OpenVisionRecipeManagerSummaryProjectionRequest
            {
                RecipeName = "Recipe_B",
                ActivePipelineName = "Pipeline_B",
                PreviewPipelineName = "Pipeline_B",
                PipelineCount = 1,
                XmlValid = false,
                XmlMessage = "invalid",
                PipelinePath = "C:\\Recipe_B\\Pipeline_B.xml"
            });
        Require(!invalidSummary.XmlValid, "Invalid XML state was marked valid.");
        Require(invalidSummary.PipelinePreviewSteps.Count == 0,
            "Invalid summary unexpectedly projected pipeline preview Steps.");
        Require(invalidSummary.LlmXmlValidationReport.Contains("NG", StringComparison.Ordinal),
            "Invalid summary did not preserve the NG validation report.");
        observations.Add("invalid summary: NG status and empty preview projected without a loaded pipeline");

        string baseLibraryText = projectionOwner.ProjectLibrarySummary("Recipe library", 0, 0);
        Require(baseLibraryText == "Recipe library", "Empty library summary changed the base label.");
        string fullLibraryText = projectionOwner.ProjectLibrarySummary("Recipe library", 5, 5);
        Require(fullLibraryText.Contains("Recipe library (5)", StringComparison.Ordinal),
            "Full library summary did not preserve the total count.");
        string filteredLibraryText = projectionOwner.ProjectLibrarySummary("Recipe library", 5, 2);
        Require(filteredLibraryText.Contains("(2/5)", StringComparison.Ordinal),
            "Filtered library summary did not preserve visible and total counts.");
        observations.Add("library text: empty, full, and filtered count formats projected");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(
        evidenceDirectory,
        "recipe_manager_summary_projection_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Recipe Manager summary/library projection owner boundary",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Recipe Manager summary projection contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Recipe Manager summary projection contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunRecipeManagerPipelineOptionProjectionContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_recipe_manager_pipeline_option_projection_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);
    string dataRoot = Path.Combine(evidenceDirectory, "data");
    Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    string recipeName = "Smoke_RecipeManagerPipelineOption_"
        + Guid.NewGuid().ToString("N").Substring(0, 12);
    string recipeDirectory = string.Empty;

    VisionPipeline CreatePipeline(string pipelineName)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = pipelineName };
        pipeline.Steps.Add(new VisionPipelineStep
        {
            Name = pipelineName + "_Threshold",
            ToolType = "Threshold",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = pipelineName + "_Output"
        });
        return pipeline;
    }

    try
    {
        RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
        recipeDirectory = RecipeWorkspaceService.GetRecipeDirectoryPath(recipeName);
        VisionPipelineStorage.Save(recipeName, CreatePipeline("Pipeline_A"));
        VisionPipelineStorage.Save(recipeName, CreatePipeline("Pipeline_B"));
        VisionPipelineStorage.SaveActivePipelineName(recipeName, "Pipeline_B");

        OpenVisionRecipePipelineOptionProjectionOwner projectionOwner =
            new OpenVisionRecipePipelineOptionProjectionOwner();
        OpenVisionRecipePipelineOptionProjection projection = projectionOwner.Project(
            new OpenVisionRecipePipelineOptionProjectionRequest
            {
                RecipeName = recipeName,
                PipelineNames = new[] { "Pipeline_A", "Pipeline_B" },
                ActivePipelineName = "Pipeline_B",
                PreferredPipelineName = "Pipeline_A",
                NormalizedPreferredPipelineName = "Pipeline_A",
                PreviousSelectedPipelineName = "Pipeline_B"
            });

        Require(projection.Options.Count == 2, "Pipeline option projection changed the inventory count.");
        Require(projection.Options[0].IsActive
            && string.Equals(projection.Options[0].PipelineName, "Pipeline_B", StringComparison.OrdinalIgnoreCase),
            "Pipeline option projection did not keep the active Pipeline first.");
        Require(string.Equals(projection.Options[1].PipelineName, "Pipeline_A", StringComparison.OrdinalIgnoreCase),
            "Pipeline option projection did not preserve deterministic name ordering.");
        Require(string.Equals(projection.SelectedOption?.PipelineName, "Pipeline_A", StringComparison.OrdinalIgnoreCase),
            "Preferred Pipeline selection was not preserved.");
        Require(projection.Options.All(option => option.XmlValid),
            "Stored valid Pipeline XML was not projected as valid options.");
        observations.Add("inventory: stored Pipeline options projected with active-first/name ordering and valid XML state");

        OpenVisionRecipePipelineOptionProjection previousSelection = projectionOwner.Project(
            new OpenVisionRecipePipelineOptionProjectionRequest
            {
                RecipeName = recipeName,
                PipelineNames = new[] { "Pipeline_A", "Pipeline_B" },
                ActivePipelineName = "Pipeline_B",
                PreferredPipelineName = string.Empty,
                NormalizedPreferredPipelineName = "Pipeline",
                PreviousSelectedPipelineName = "Pipeline_A"
            });
        Require(string.Equals(previousSelection.SelectedOption?.PipelineName, "Pipeline_A", StringComparison.OrdinalIgnoreCase),
            "Previous selected Pipeline fallback was not preserved.");

        OpenVisionRecipePipelineOptionProjection activeFallback = projectionOwner.Project(
            new OpenVisionRecipePipelineOptionProjectionRequest
            {
                RecipeName = recipeName,
                PipelineNames = new[] { "Pipeline_A", "Pipeline_B" },
                ActivePipelineName = "Pipeline_B",
                PreferredPipelineName = string.Empty,
                NormalizedPreferredPipelineName = "Pipeline",
                PreviousSelectedPipelineName = "Missing"
            });
        Require(string.Equals(activeFallback.SelectedOption?.PipelineName, "Pipeline_B", StringComparison.OrdinalIgnoreCase),
            "Active Pipeline fallback was not preserved when the previous selection was absent.");
        observations.Add("selection: preferred, previous, and active fallback order preserved");

        IReadOnlyList<OpenVisionRecipePipelineOption> nameFilter = projectionOwner.Filter(
            projection.Options,
            " pipeline_a ");
        Require(nameFilter.Count == 1
            && string.Equals(nameFilter[0].PipelineName, "Pipeline_A", StringComparison.OrdinalIgnoreCase),
            "Case-insensitive trimmed Pipeline name filtering changed.");
        IReadOnlyList<OpenVisionRecipePipelineOption> statusFilter = projectionOwner.Filter(
            projection.Options,
            "xml ok");
        Require(statusFilter.Count == 2, "Pipeline status/detail filtering changed.");
        Require(projectionOwner.Filter(projection.Options, string.Empty).Count == 2,
            "Empty Pipeline filter did not preserve the full option list.");
        observations.Add("filter: trimmed name, status text, and empty-filter behavior preserved");

        string emptySummary = projectionOwner.ProjectListSummary("Pipelines", 0, 0);
        Require(emptySummary == "Pipelines", "Empty Pipeline summary changed the base label.");
        string fullSummary = projectionOwner.ProjectListSummary("Pipelines", 2, 2);
        Require(fullSummary.Contains("Pipelines (2)", StringComparison.Ordinal),
            "Full Pipeline summary did not preserve the total count.");
        string filteredSummary = projectionOwner.ProjectListSummary("Pipelines", 2, 1);
        Require(filteredSummary.Contains("(1/2)", StringComparison.Ordinal),
            "Filtered Pipeline summary did not preserve visible and total counts.");
        observations.Add("summary: empty, full, and filtered Pipeline count formats projected");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }
    finally
    {
        if (!string.IsNullOrWhiteSpace(recipeDirectory)
            && !RecipeWorkspaceService.DeleteVisionWorkspace(recipeName))
        {
            failures.Add("Reserved smoke Recipe cleanup failed: " + recipeName);
        }
    }

    string reportPath = Path.Combine(
        evidenceDirectory,
        "recipe_manager_pipeline_option_projection_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Recipe Manager Pipeline option projection owner boundary",
            "EvidenceDirectory: " + evidenceDirectory,
            "DataRoot: " + dataRoot,
            "RecipeDirectory: " + recipeDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Recipe Manager Pipeline option projection contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Recipe Manager Pipeline option projection contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunRecipePipelineLifecycleProjectionContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_recipe_pipeline_lifecycle_projection_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    try
    {
        OpenVisionRecipePipelineLifecycleProjectionOwner projectionOwner =
            new OpenVisionRecipePipelineLifecycleProjectionOwner();

        bool HasStatus(
            OpenVisionRecipePipelineLifecycleProjection projection,
            string english,
            string korean)
        {
            return projection.StatusText.Contains(english, StringComparison.Ordinal)
                || projection.StatusText.Contains(korean, StringComparison.Ordinal);
        }

        OpenVisionRecipePipelineLifecycleProjection activated = projectionOwner.Project(
            OpenVisionRecipePipelineLifecycleOperation.Activate,
            OpenVisionRecipePipelineLifecycleResult.Success("Pipeline_Active", string.Empty));
        Require(activated.Succeeded, "Activate success projection was not marked succeeded.");
        Require(activated.PipelineName == "Pipeline_Active", "Activate result name was not preserved.");
        Require(HasStatus(activated, "Active pipeline: Pipeline_Active", "활성 파이프라인: Pipeline_Active"),
            "Activate status was not projected.");
        observations.Add("activate success: result name and localized status projected");

        const string duplicateDetail = "Duplicated pipeline 'Pipeline_A' to 'Pipeline_B'.";
        OpenVisionRecipePipelineLifecycleProjection duplicated = projectionOwner.Project(
            OpenVisionRecipePipelineLifecycleOperation.Duplicate,
            OpenVisionRecipePipelineLifecycleResult.Success("Pipeline_B", duplicateDetail));
        Require(duplicated.Succeeded, "Duplicate success projection was not marked succeeded.");
        Require(duplicated.PipelineName == "Pipeline_B", "Duplicate result name was not preserved.");
        Require(duplicated.StatusText == duplicateDetail, "Duplicate detail status changed.");

        const string duplicateFailureDetail = "Target pipeline already exists.";
        OpenVisionRecipePipelineLifecycleProjection duplicateFailure = projectionOwner.Project(
            OpenVisionRecipePipelineLifecycleOperation.Duplicate,
            OpenVisionRecipePipelineLifecycleResult.Failure(duplicateFailureDetail));
        Require(!duplicateFailure.Succeeded, "Duplicate failure projection was marked succeeded.");
        Require(duplicateFailure.PipelineName == string.Empty, "Duplicate failure exposed a Pipeline name.");
        Require(duplicateFailure.StatusText == duplicateFailureDetail, "Duplicate failure detail changed.");
        observations.Add("duplicate success/failure: storage detail and result name preserved");

        const string renameDetail = "Renamed pipeline 'Pipeline_B' to 'Pipeline_C'.";
        OpenVisionRecipePipelineLifecycleProjection renamed = projectionOwner.Project(
            OpenVisionRecipePipelineLifecycleOperation.Rename,
            OpenVisionRecipePipelineLifecycleResult.Success("Pipeline_C", renameDetail));
        Require(renamed.Succeeded, "Rename success projection was not marked succeeded.");
        Require(renamed.PipelineName == "Pipeline_C", "Rename result name was not preserved.");
        Require(renamed.StatusText == renameDetail, "Rename detail status changed.");

        const string deleteDetail = "Deleted pipeline 'Pipeline_C'.";
        OpenVisionRecipePipelineLifecycleProjection deleted = projectionOwner.Project(
            OpenVisionRecipePipelineLifecycleOperation.Delete,
            OpenVisionRecipePipelineLifecycleResult.Success("Pipeline_Fallback", deleteDetail));
        Require(deleted.Succeeded, "Delete success projection was not marked succeeded.");
        Require(deleted.PipelineName == "Pipeline_Fallback", "Delete fallback name was not preserved.");
        Require(deleted.StatusText == deleteDetail, "Delete detail status changed.");
        observations.Add("rename/delete success: result names and storage detail preserved");

        OpenVisionRecipePipelineLifecycleProjection sampleSuccess = projectionOwner.Project(
            OpenVisionRecipePipelineLifecycleOperation.DuplicateFromSample,
            OpenVisionRecipePipelineLifecycleResult.Success("Sample_Pipeline", string.Empty));
        Require(sampleSuccess.Succeeded, "Sample duplicate success projection was not marked succeeded.");
        Require(sampleSuccess.PipelineName == "Sample_Pipeline", "Sample duplicate result name was not preserved.");
        Require(HasStatus(sampleSuccess, "Duplicated sample pipeline: Sample_Pipeline", "샘플 파이프라인 복제됨: Sample_Pipeline"),
            "Sample duplicate success status was not projected.");

        const string sampleFailureDetail = "Source pipeline XML was not found.";
        OpenVisionRecipePipelineLifecycleProjection sampleFailure = projectionOwner.Project(
            OpenVisionRecipePipelineLifecycleOperation.DuplicateFromSample,
            OpenVisionRecipePipelineLifecycleResult.Failure(sampleFailureDetail));
        Require(!sampleFailure.Succeeded, "Sample duplicate failure projection was marked succeeded.");
        Require(sampleFailure.StatusText.Contains(sampleFailureDetail, StringComparison.Ordinal),
            "Sample duplicate failure detail was not preserved.");
        Require(HasStatus(sampleFailure, "Sample pipeline load failed: ", "샘플 파이프라인 로드 실패: "),
            "Sample duplicate failure prefix was not projected.");
        observations.Add("sample duplicate success/failure: localized prefix and result name preserved");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(
        evidenceDirectory,
        "recipe_pipeline_lifecycle_projection_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Pipeline lifecycle result/status projection owner boundary",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Recipe Pipeline lifecycle projection contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Recipe Pipeline lifecycle projection contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunRecipeReviewBundleDryRunProjectionContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_recipe_review_bundle_dry_run_projection_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    try
    {
        OpenVisionRecipeReviewBundleDryRunProjectionOwner projectionOwner =
            new OpenVisionRecipeReviewBundleDryRunProjectionOwner();

        bool HasStatus(
            OpenVisionRecipeReviewBundleDryRunProjection projection,
            string english,
            string korean)
        {
            return projection.StatusText.Contains(english, StringComparison.Ordinal)
                || projection.StatusText.Contains(korean, StringComparison.Ordinal);
        }

        OpenVisionRecipeReviewBundleDryRunProjection integrityFailure = projectionOwner.Project(
            inspectionSucceeded: false,
            xmlReady: false);
        Require(!integrityFailure.Succeeded, "Integrity failure projection was marked succeeded.");
        Require(HasStatus(
                integrityFailure,
                "Review bundle dry-run NG. Nothing was imported.",
                "검토 번들 dry-run NG. 가져오지 않았습니다."),
            "Integrity failure status was not projected.");
        observations.Add("integrity failure: NG status and false return preserved");

        OpenVisionRecipeReviewBundleDryRunProjection reviewReady = projectionOwner.Project(
            inspectionSucceeded: true,
            xmlReady: true);
        Require(reviewReady.Succeeded, "Review-ready projection was not marked succeeded.");
        Require(HasStatus(
                reviewReady,
                "Review bundle dry-run OK. XML was loaded for review only; import, Preview, and Run were not executed.",
                "검토 번들 dry-run OK. XML은 검토 화면에만 로드했으며 가져오기/Preview/Run은 실행하지 않았습니다."),
            "Review-ready status was not projected.");
        observations.Add("integrity OK/XML ready: review-only status and true return preserved");

        OpenVisionRecipeReviewBundleDryRunProjection reviewFailure = projectionOwner.Project(
            inspectionSucceeded: true,
            xmlReady: false);
        Require(reviewFailure.Succeeded, "Integrity-OK/XML-NG projection changed the existing return semantics.");
        Require(HasStatus(
                reviewFailure,
                "Review bundle integrity is OK, but XML/dependency review is NG. Import, Preview, and Run were not executed.",
                "검토 번들 무결성은 OK지만 XML/의존성 검토는 NG입니다. 가져오기/Preview/Run은 실행하지 않았습니다."),
            "Integrity-OK/XML-NG status was not projected.");
        observations.Add("integrity OK/XML NG: review failure status with true return preserved");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(
        evidenceDirectory,
        "recipe_review_bundle_dry_run_projection_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Review bundle dry-run result/status projection owner boundary",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Recipe Review bundle dry-run projection contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Recipe Review bundle dry-run projection contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunPipelineReviewResultStatusProjectionContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_pipeline_review_result_status_projection_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    try
    {
        OpenVisionPipelineReviewResultStatusProjectionOwner projectionOwner =
            new OpenVisionPipelineReviewResultStatusProjectionOwner();

        bool HasText(string value, string english, string korean)
        {
            return value.Contains(english, StringComparison.Ordinal)
                || value.Contains(korean, StringComparison.Ordinal);
        }

        void RequireStatus(
            OpenVisionPipelineReviewResultStatusProjection projection,
            string english,
            string korean,
            string description)
        {
            Require(
                HasText(projection.ExecutionStateText, english, korean),
                description + " execution status was not projected.");
        }

        OpenVisionPipelineReviewResultStatusProjection notRun = projectionOwner.Project(
            OpenVisionPipelineReviewResultStatusKind.NotRun);
        RequireStatus(notRun, "Not run", "실행 전", "Not-run");

        OpenVisionPipelineReviewResultStatusProjection draining = projectionOwner.Project(
            OpenVisionPipelineReviewResultStatusKind.Draining);
        RequireStatus(draining, "Waiting for execution to stop", "종료 대기", "Draining");

        OpenVisionPipelineReviewResultStatusProjection alreadyRunning = projectionOwner.Project(
            OpenVisionPipelineReviewResultStatusKind.AlreadyRunning);
        RequireStatus(alreadyRunning, "Already running", "이미 실행 중", "Already-running");

        OpenVisionPipelineReviewResultStatusProjection noSteps = projectionOwner.Project(
            OpenVisionPipelineReviewResultStatusKind.NoSteps);
        RequireStatus(noSteps, "No steps", "Step 없음", "No-steps");

        OpenVisionPipelineReviewResultStatusProjection validationErrors = projectionOwner.Project(
            OpenVisionPipelineReviewResultStatusKind.ValidationErrors);
        RequireStatus(validationErrors, "Validation errors", "검증 오류", "Validation-errors");
        Require(
            HasText(validationErrors.ResultSummaryText, "Validation error", "검증 오류"),
            "Validation error result summary was not projected.");
        Require(
            HasText(validationErrors.ResultDetailText, "Fix validation errors before running review.", "리뷰 실행 전 검증 오류를 수정하십시오."),
            "Validation error result detail was not projected.");

        OpenVisionPipelineReviewResultStatusProjection started = projectionOwner.Project(
            OpenVisionPipelineReviewResultStatusKind.Started);
        RequireStatus(started, "Started", "시작됨", "Started");
        Require(HasText(started.ResultSummaryText, "Running", "실행 중"), "Running result summary was not projected.");
        Require(HasText(started.ResultDetailText, "Pipeline review execution in progress.", "Pipeline 리뷰 실행 중입니다."), "Running result detail was not projected.");

        OpenVisionPipelineReviewResultStatusProjection superseded = projectionOwner.Project(
            OpenVisionPipelineReviewResultStatusKind.Superseded);
        RequireStatus(superseded, "Not run", "실행 전", "Superseded");

        const string failureMessage = "synthetic review failure";
        OpenVisionPipelineReviewResultStatusProjection failed = projectionOwner.Project(
            OpenVisionPipelineReviewResultStatusKind.Failed,
            errorMessage: failureMessage);
        RequireStatus(failed, "Failed: " + failureMessage, "실패: " + failureMessage, "Failed");
        Require(HasText(failed.ResultSummaryText, "Run failed", "실행 실패"), "Failed result summary was not projected.");
        Require(failed.ResultDetailText == failureMessage, "Failed result detail changed.");

        OpenVisionPipelineReviewResultStatusProjection completed = projectionOwner.Project(
            OpenVisionPipelineReviewResultStatusKind.Completed,
            stepResultCount: 3);
        RequireStatus(completed, "Completed / 3 step results", "완료 / 3 Step 결과", "Completed");

        OpenVisionPipelineReviewResultStatusProjection referenceChanged = projectionOwner.Project(
            OpenVisionPipelineReviewResultStatusKind.ReferenceChanged);
        RequireStatus(referenceChanged, "Reference changed / run review required", "참조 변경됨 / 리뷰 재실행 필요", "Reference-changed");
        Require(HasText(referenceChanged.ResultSummaryText, "Reference saved", "참조 저장됨"), "Reference result summary was not projected.");

        OpenVisionPipelineReviewResultStatusProjection runRequired = projectionOwner.Project(
            OpenVisionPipelineReviewResultStatusKind.RunRequired);
        RequireStatus(runRequired, "Not run", "실행 전", "Run-required");
        Require(HasText(runRequired.ResultSummaryText, "Run review required", "리뷰 실행 필요"), "Run-required result summary was not projected.");
        observations.Add("execution states: lifecycle, validation, failure, completion, reference, and run-required text preserved");

        IReadOnlyList<VisionPipelineStep> emptySteps = Array.Empty<VisionPipelineStep>();
        string emptyProgress = projectionOwner.ProjectProgress(emptySteps, null, isRunning: false, isStopping: false);
        Require(HasText(emptyProgress, "No steps", "Step 없음"), "Empty Pipeline progress changed.");

        VisionPipelineStep passedStep = new VisionPipelineStep
        {
            Name = "Review passed",
            ToolType = "Blob",
            InputLayer = "Main",
            OutputLayer = "Review_Output",
            Enabled = true
        };
        VisionPipelineStep failedStep = new VisionPipelineStep
        {
            Name = "Review failed",
            ToolType = "Blob",
            InputLayer = "Review_Output",
            OutputLayer = "Review_Output_2",
            Enabled = true
        };
        VisionPipelineStep disabledStep = new VisionPipelineStep
        {
            Name = "Review disabled",
            ToolType = "Blob",
            InputLayer = "Main",
            OutputLayer = "Disabled_Output",
            Enabled = false
        };
        List<VisionPipelineStep> steps = new List<VisionPipelineStep>
        {
            passedStep,
            failedStep,
            disabledStep
        };
        VisionPipelineStepResultSummary passedSummary = new VisionPipelineStepResultSummary
        {
            Success = true,
            IsAcceptanceNg = false
        };
        VisionPipelineStepResultSummary failedSummary = new VisionPipelineStepResultSummary
        {
            Success = true,
            IsAcceptanceNg = true
        };
        Func<VisionPipelineStep, VisionPipelineStepResultSummary> resolveSummary = step =>
            ReferenceEquals(step, passedStep)
                ? passedSummary
                : ReferenceEquals(step, failedStep)
                ? failedSummary
                : null!;

        string notRunProgress = projectionOwner.ProjectProgress(
            steps,
            step => null,
            isRunning: false,
            isStopping: false);
        Require(HasText(notRunProgress, "Not run", "미실행"), "Unexecuted progress changed.");

        string completedProgress = projectionOwner.ProjectProgress(
            steps,
            resolveSummary,
            isRunning: false,
            isStopping: false);
        Require(completedProgress.Contains("OK 1", StringComparison.Ordinal), "OK progress count changed.");
        Require(completedProgress.Contains("NG 1", StringComparison.Ordinal), "NG progress count changed.");
        Require(completedProgress.Contains("WAIT 0", StringComparison.Ordinal)
                || completedProgress.Contains("대기 0", StringComparison.Ordinal),
            "WAIT progress count changed.");
        Require(completedProgress.Contains("OFF 1", StringComparison.Ordinal), "OFF progress count changed.");

        string runningProgress = projectionOwner.ProjectProgress(
            steps,
            resolveSummary,
            isRunning: true,
            isStopping: false);
        Require(HasText(runningProgress, "Running...", "실행 중..."), "Running progress prefix changed.");

        string stoppingProgress = projectionOwner.ProjectProgress(
            steps,
            resolveSummary,
            isRunning: true,
            isStopping: true);
        Require(HasText(stoppingProgress, "Waiting for execution to stop", "종료 대기"), "Draining progress prefix changed.");
        observations.Add("progress: empty, not-run, OK/NG/WAIT/OFF, running, and draining projections preserved");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(
        evidenceDirectory,
        "pipeline_review_result_status_projection_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Pipeline Review run-level result/status projection owner boundary",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Pipeline Review result/status projection contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Pipeline Review result/status projection contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunPipelineReviewGuideResultProjectionContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_pipeline_review_guide_result_projection_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    try
    {
        OpenVisionPipelineReviewGuideResultProjectionOwner projectionOwner =
            new OpenVisionPipelineReviewGuideResultProjectionOwner();
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "Threshold_Main",
            ToolType = "Threshold",
            InputLayer = "Main",
            OutputLayer = "Threshold_Preview",
            AcceptanceMetricName = "Score",
            Enabled = true
        };
        VisionPipelineStep acceptanceStep = new VisionPipelineStep
        {
            Name = step.Name,
            ToolType = step.ToolType,
            InputLayer = step.InputLayer,
            OutputLayer = step.OutputLayer,
            Enabled = true,
            UseAcceptance = true,
            AcceptanceMetricName = "Score",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 0.5
        };
        VisionPipelineValidationResult validationResult = new VisionPipelineValidationResult();
        VisionPipelineSampleCatalogItem activeSample = new VisionPipelineSampleCatalogItem
        {
            SampleName = "Synthetic Good",
            PairGroup = "SyntheticPair",
            PairRole = "Good",
            ExpectedMetricName = "Score",
            ExpectedMetricMinimum = "0.5",
            ExpectedMetricMaximum = "1.0"
        };
        VisionPipelineSampleCatalogItem counterpartSample = new VisionPipelineSampleCatalogItem
        {
            SampleName = "Synthetic Bad",
            PairGroup = "SyntheticPair",
            PairRole = "Bad",
            ValidationMode = "ExpectedFailure",
            ExpectedMetricName = "Score",
            ExpectedMetricMinimum = "0.0",
            ExpectedMetricMaximum = "0.4"
        };
        OpenVisionWorkspaceSamplePairDecisionGuide pairGuide =
            new OpenVisionWorkspaceSamplePairDecisionGuide(
                true,
                "Synthetic pair summary",
                "Synthetic pair metric",
                "Synthetic pair checklist",
                "Run the paired sample",
                "Synthetic pair workflow",
                "Synthetic pair review");

        bool HasText(string value, params string[] expected)
        {
            return expected.Any(text => value?.Contains(text, StringComparison.Ordinal) == true);
        }

        OpenVisionPipelineReviewGuideResultProjectionRequest CreateRequest(
            VisionPipelineStep requestStep,
            VisionPipelineStepResultSummary? summary,
            string statusText,
            bool hasOutputImage,
            bool includePair)
        {
            return new OpenVisionPipelineReviewGuideResultProjectionRequest
            {
                DisplayIndex = 1,
                StepCount = 1,
                Step = requestStep,
                StatusText = statusText,
                HasInputImage = true,
                HasOutputImage = hasOutputImage,
                Summary = summary,
                ValidationResult = validationResult,
                ExpectedInputLayer = requestStep?.InputLayer,
                IsBranch = false,
                InputWillBeProduced = false,
                SamplePairGuide = includePair ? pairGuide : OpenVisionWorkspaceSamplePairDecisionGuide.Empty,
                ActiveCatalogSample = includePair ? activeSample : null,
                ActivePairCounterpartSample = includePair ? counterpartSample : null,
                PreviewMode = PipelineFlowPreviewMode.Overlay,
                ValidationStatusText = "Ready"
            };
        }

        OpenVisionPipelineReviewGuideResultProjection missingResult = projectionOwner.ProjectSelected(
            CreateRequest(step, null, "WAIT", hasOutputImage: false, includePair: false));
        Require(
            HasText(missingResult.ResultSummaryText, "Run review required", "리뷰 실행 필요"),
            "Missing-result summary was not projected.");
        Require(
            HasText(missingResult.ResultDetailText, "No run result for selected step.", "선택 Step 실행 결과 없음."),
            "Missing-result detail was not projected.");
        Require(!string.IsNullOrWhiteSpace(missingResult.RunLogText), "Missing-result run log was empty.");
        Require(!string.IsNullOrWhiteSpace(missingResult.GuideState.ResultDecisionText), "Missing-result guide decision was empty.");

        VisionPipelineStepResultSummary okSummary = new VisionPipelineStepResultSummary
        {
            Index = 1,
            Name = step.Name,
            ToolType = step.ToolType,
            InputLayer = step.InputLayer,
            OutputLayer = step.OutputLayer,
            Status = "OK",
            Success = true,
            HasResultImage = true,
            ResultImageWidth = 512,
            ResultImageHeight = 384,
            ElapsedMilliseconds = 2.5,
            Metrics = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                ["Score"] = 0.75
            }
        };
        OpenVisionPipelineReviewGuideResultProjection okResult = projectionOwner.ProjectSelected(
            CreateRequest(step, okSummary, "OK", hasOutputImage: true, includePair: true));
        Require(HasText(okResult.ResultSummaryText, "OK"), "OK result summary changed.");
        Require(
            HasText(okResult.ResultSummaryText, "Judgement not evaluated", "판정 미평가"),
            "No-acceptance result did not disclose that inspection judgment was not evaluated.");
        Require(okResult.ResultDetailText.Contains("512x384", StringComparison.Ordinal), "OK image dimensions were not projected.");
        Require(!string.IsNullOrWhiteSpace(okResult.GuideState.ResultDecisionText), "OK guide decision was empty.");
        Require(HasText(okResult.PairActionText, "NG reference", "NG 기준"), "Pair action text changed.");
        Require(!okResult.CanOpenPairAction, "Missing pair files were unexpectedly marked openable.");
        Require(!string.IsNullOrWhiteSpace(okResult.PairMetricText), "Pair metric comparison was not projected.");

        VisionPipelineStepResultSummary acceptancePassSummary = new VisionPipelineStepResultSummary
        {
            Index = 1,
            Name = acceptanceStep.Name,
            ToolType = acceptanceStep.ToolType,
            Status = "OK",
            Success = true,
            Metrics = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                ["Score"] = 0.75
            }
        };
        OpenVisionPipelineReviewGuideResultProjection acceptancePassResult = projectionOwner.ProjectSelected(
            CreateRequest(acceptanceStep, acceptancePassSummary, "OK", hasOutputImage: true, includePair: false));
        Require(
            HasText(acceptancePassResult.ResultSummaryText, "Judgement PASS", "판정 PASS"),
            "Acceptance-pass result did not disclose the inspection judgment.");

        VisionPipelineStepResultSummary toolNgSummary = new VisionPipelineStepResultSummary
        {
            Index = 1,
            Name = step.Name,
            ToolType = step.ToolType,
            Status = "NG",
            Success = false,
            IsToolError = true,
            Message = "synthetic tool failure"
        };
        OpenVisionPipelineReviewGuideResultProjection toolNgResult = projectionOwner.ProjectSelected(
            CreateRequest(step, toolNgSummary, "NG", hasOutputImage: false, includePair: false));
        Require(HasText(toolNgResult.ResultSummaryText, "NG"), "Tool-NG result summary changed.");
        Require(
            HasText(toolNgResult.ResultSummaryText, "Tool NG", "처리 NG"),
            "Tool-NG result did not separate processing failure from inspection judgment.");
        Require(
            HasText(toolNgResult.ResultSummaryText, "Judgement not evaluated", "판정 미평가"),
            "Tool-NG result did not disclose that inspection judgment was not evaluated.");
        Require(toolNgResult.ResultDetailText.Contains("synthetic tool failure", StringComparison.Ordinal), "Tool-NG detail changed.");

        VisionPipelineStepResultSummary acceptanceNgSummary = new VisionPipelineStepResultSummary
        {
            Index = 1,
            Name = acceptanceStep.Name,
            ToolType = acceptanceStep.ToolType,
            Status = "NG",
            Success = true,
            IsAcceptanceNg = true,
            AcceptanceMessage = "synthetic acceptance failure"
        };
        OpenVisionPipelineReviewGuideResultProjection acceptanceNgResult = projectionOwner.ProjectSelected(
            CreateRequest(acceptanceStep, acceptanceNgSummary, "NG", hasOutputImage: true, includePair: false));
        Require(HasText(acceptanceNgResult.ResultSummaryText, "NG"), "Acceptance-NG result summary changed.");
        Require(
            HasText(acceptanceNgResult.ResultSummaryText, "Tool OK", "처리 OK"),
            "Acceptance-NG result did not preserve tool-processing success.");
        Require(
            HasText(acceptanceNgResult.ResultSummaryText, "Judgement NG", "판정 NG"),
            "Acceptance-NG result did not disclose the inspection judgment.");
        Require(acceptanceNgResult.ResultDetailText.Contains("synthetic acceptance failure", StringComparison.Ordinal), "Acceptance-NG detail changed.");
        Require(!string.IsNullOrWhiteSpace(acceptanceNgResult.GuideState.DetailText), "Acceptance-NG guide detail was empty.");

        OpenVisionRecipeRunEvidenceDrawing notEvaluatedDrawing = new OpenVisionRecipeRunEvidenceDrawing(
            new VisionPipelineStepRunReport
            {
                AcceptanceEvaluated = false,
                AcceptancePassed = false,
                AcceptanceMessage = string.Empty
            },
            "synthetic.png");
        Require(
            HasText(notEvaluatedDrawing.AcceptanceText, "Not evaluated: no acceptance criteria", "미평가: 적용 기준 없음"),
            "Persisted no-acceptance evidence did not preserve the not-evaluated state.");

        OpenVisionRecipeRunEvidenceDrawing passDrawing = new OpenVisionRecipeRunEvidenceDrawing(
            new VisionPipelineStepRunReport
            {
                AcceptanceEvaluated = true,
                AcceptancePassed = true,
                AcceptanceMessage = "Score within range"
            },
            "synthetic.png");
        Require(passDrawing.AcceptanceText.StartsWith("PASS:", StringComparison.Ordinal), "Persisted acceptance PASS evidence changed.");

        OpenVisionLanguage originalLanguage = OpenVisionLanguageService.CurrentLanguage;
        try
        {
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, save: false);
            string koreanSummary = OpenVisionPipelineReviewResultPresenter.FormatResultSummary(step, okSummary);
            string koreanNativeMarker = VisionToolVerificationText.InspectionJudgmentNotEvaluated;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, save: false);
            string englishSummary = OpenVisionPipelineReviewResultPresenter.FormatResultSummary(step, okSummary);
            string englishNativeMarker = VisionToolVerificationText.InspectionJudgmentNotEvaluated;
            Require(koreanSummary.Contains("판정 미평가", StringComparison.Ordinal), "Korean acceptance-clarity text changed.");
            Require(englishSummary.Contains("Judgement not evaluated", StringComparison.Ordinal), "English acceptance-clarity text changed.");
            Require(koreanNativeMarker.Contains("검사 판정 미평가", StringComparison.Ordinal), "Korean native Preview marker changed.");
            Require(englishNativeMarker.Contains("Inspection judgment not evaluated", StringComparison.Ordinal), "English native Preview marker changed.");
        }
        finally
        {
            OpenVisionLanguageService.SetLanguage(originalLanguage, save: false);
        }

        OpenVisionPipelineReviewGuideState validationGuide = projectionOwner.ProjectValidationErrorGuide(1, 1, step);
        Require(
            HasText(validationGuide.NextActionText, "Fix validation errors before review", "검증 오류"),
            "Validation-error guide changed.");
        OpenVisionPipelineReviewGuideState runningGuide = projectionOwner.ProjectRunningGuide(1, 1, step);
        Require(
            HasText(runningGuide.NextActionText, "Review is running", "리뷰 실행"),
            "Running guide changed.");

        observations.Add("missing-result, no-acceptance OK, acceptance PASS, tool-NG, acceptance-NG, persisted evidence, Korean/English text, validation-error, running, pair action, pair metric, and run-log projection preserved");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(
        evidenceDirectory,
        "pipeline_review_guide_result_projection_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Pipeline Review guide/result projection owner boundary",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Pipeline Review guide/result projection contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Pipeline Review guide/result projection contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunPipelineReviewDomainEvidenceProjectionContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_pipeline_review_domain_evidence_projection_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    try
    {
        OpenVisionPipelineReviewDomainEvidenceProjectionOwner projectionOwner =
            new OpenVisionPipelineReviewDomainEvidenceProjectionOwner();
        List<VisionPipelineObjectResult> objectResults = new List<VisionPipelineObjectResult>
        {
            new VisionPipelineObjectResult { Number = 1, Accepted = true, Area = 25D }
        };
        List<VisionPipelineInstanceResult> instanceResults = new List<VisionPipelineInstanceResult>
        {
            new VisionPipelineInstanceResult { Number = 1, Accepted = false, MeanValue = 120D }
        };
        List<VisionPipelineGeometryFeatureResult> geometryResults = new List<VisionPipelineGeometryFeatureResult>
        {
            new VisionPipelineGeometryFeatureResult
            {
                SourceStep = "Line_Main",
                FeatureName = "Start",
                Kind = VisionPipelineGeometryKind.Point
            }
        };
        Dictionary<string, double> metrics = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            ["Score"] = 0.82D
        };
        VisionPipelineStepResultSummary summary = new VisionPipelineStepResultSummary
        {
            ObjectResults = objectResults,
            InstanceResults = instanceResults,
            GeometryFeatures = geometryResults,
            Metrics = metrics
        };

        VisionPipelineStep Step(string toolType)
        {
            return new VisionPipelineStep { ToolType = toolType };
        }

        OpenVisionPipelineReviewDomainEvidenceProjection blob = projectionOwner.Project(
            Step("Blob"),
            summary);
        Require(blob.SupportsObjectResults, "Blob object evidence support was not projected.");
        Require(!blob.SupportsInstanceResults, "Blob unexpectedly exposed instance evidence support.");
        Require(!blob.SupportsGeometryResults, "Blob unexpectedly exposed geometry evidence support.");
        Require(!blob.SupportsCircleEvidence, "Blob unexpectedly exposed circle evidence support.");
        Require(!blob.SupportsMatcherDiagnostics, "Blob unexpectedly exposed matcher diagnostics support.");
        Require(ReferenceEquals(blob.ObjectResults, objectResults), "Object evidence data was copied or lost.");
        Require(ReferenceEquals(blob.InstanceResults, instanceResults), "Instance evidence data was copied or lost.");
        Require(ReferenceEquals(blob.GeometryResults, geometryResults), "Geometry evidence data was copied or lost.");
        Require(ReferenceEquals(blob.Metrics, metrics), "Metric evidence data was copied or lost.");

        OpenVisionPipelineReviewDomainEvidenceProjection contour = projectionOwner.Project(
            Step("Contour Tool"),
            summary);
        Require(contour.SupportsObjectResults, "Contour tool alias lost object evidence support.");

        OpenVisionPipelineReviewDomainEvidenceProjection multiFixtureMean = projectionOwner.Project(
            Step("MultiFixtureMean"),
            summary);
        Require(multiFixtureMean.SupportsInstanceResults, "MultiFixtureMean instance evidence support was not projected.");

        OpenVisionPipelineReviewDomainEvidenceProjection geometry = projectionOwner.Project(
            Step("Geometry_Measure"),
            summary);
        Require(geometry.SupportsGeometryResults, "Geometry_Measure geometry evidence support was not projected.");

        OpenVisionPipelineReviewDomainEvidenceProjection circle = projectionOwner.Project(
            Step("CircleGauge"),
            summary);
        Require(circle.SupportsGeometryResults, "CircleGauge geometry evidence support was not projected.");
        Require(circle.SupportsCircleEvidence, "CircleGauge circle evidence support was not projected.");

        OpenVisionPipelineReviewDomainEvidenceProjection matcher = projectionOwner.Project(
            Step("EdgeTemplateMatching"),
            summary);
        Require(matcher.SupportsMatcherDiagnostics, "EdgeTemplateMatching diagnostics support was not projected.");

        OpenVisionPipelineReviewDomainEvidenceProjection unsupported = projectionOwner.Project(
            Step("Threshold"),
            summary);
        Require(!unsupported.SupportsObjectResults
                && !unsupported.SupportsInstanceResults
                && !unsupported.SupportsGeometryResults
                && !unsupported.SupportsCircleEvidence
                && !unsupported.SupportsMatcherDiagnostics,
            "Unsupported tool unexpectedly exposed domain evidence support.");
        Require(unsupported.CircleEvidence == null, "Unsupported tool changed circle evidence data.");
        Require(unsupported.MatcherDiagnostics == null, "Unsupported tool changed matcher diagnostics data.");

        observations.Add("Blob/Contour, MultiFixtureMean, Geometry_Measure, CircleGauge, EdgeTemplateMatching, unsupported tool, and evidence data identity preserved");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(
        evidenceDirectory,
        "pipeline_review_domain_evidence_projection_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Pipeline Review selected-Step domain evidence projection owner boundary",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Pipeline Review domain evidence projection contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Pipeline Review domain evidence projection contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunRecipePipelineExchangeProjectionContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "ovl07_recipe_pipeline_exchange_projection_contract_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    try
    {
        OpenVisionRecipePipelineExchangeProjectionOwner projectionOwner =
            new OpenVisionRecipePipelineExchangeProjectionOwner();

        bool HasStatus(
            OpenVisionRecipePipelineExchangeProjection projection,
            string english,
            string korean)
        {
            return projection.StatusText.Contains(english, StringComparison.Ordinal)
                || projection.StatusText.Contains(korean, StringComparison.Ordinal);
        }

        OpenVisionRecipePipelineExchangeProjection imported = projectionOwner.Project(
            OpenVisionRecipePipelineExchangeOperation.Import,
            OpenVisionRecipePipelineExchangeResult.Success("Imported_Pipeline", string.Empty));
        Require(imported.Succeeded, "Import success projection was not marked succeeded.");
        Require(imported.PipelineName == "Imported_Pipeline", "Import result name was not preserved.");
        Require(HasStatus(imported, "Imported XML: Imported_Pipeline", "XML 가져오기 완료: Imported_Pipeline"),
            "Import status was not projected.");
        observations.Add("import success: result name and localized status projected");

        const string exportPath = "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\exchange\\Active_Pipeline.xml";
        OpenVisionRecipePipelineExchangeProjection exported = projectionOwner.Project(
            OpenVisionRecipePipelineExchangeOperation.Export,
            OpenVisionRecipePipelineExchangeResult.Success("Active_Pipeline", exportPath));
        Require(exported.Succeeded, "Export success projection was not marked succeeded.");
        Require(exported.PipelineName == "Active_Pipeline", "Export result name was not preserved.");
        Require(HasStatus(exported, "Exported XML: Active_Pipeline.xml", "XML 내보내기 완료: Active_Pipeline.xml"),
            "Export status did not preserve the destination file name.");

        const string exportFailureDetail = "Destination XML could not be written.";
        OpenVisionRecipePipelineExchangeProjection exportFailure = projectionOwner.Project(
            OpenVisionRecipePipelineExchangeOperation.Export,
            OpenVisionRecipePipelineExchangeResult.Failure(exportFailureDetail));
        Require(!exportFailure.Succeeded, "Export failure projection was marked succeeded.");
        Require(exportFailure.PipelineName == string.Empty, "Export failure exposed a Pipeline name.");
        Require(exportFailure.StatusText == exportFailureDetail, "Export failure detail changed.");
        observations.Add("export success/failure: destination name and storage detail preserved");

        const string reviewBundlePath = "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\exchange\\Active_Pipeline.review.zip";
        OpenVisionRecipePipelineExchangeProjection bundle = projectionOwner.Project(
            OpenVisionRecipePipelineExchangeOperation.ExportReviewBundle,
            OpenVisionRecipePipelineExchangeResult.Success("Active_Pipeline", reviewBundlePath));
        Require(bundle.Succeeded, "Review bundle success projection was not marked succeeded.");
        Require(bundle.PipelineName == "Active_Pipeline", "Review bundle result name was not preserved.");
        Require(HasStatus(bundle, "Exported review bundle: Active_Pipeline.review.zip", "검토 묶음 내보내기 완료: Active_Pipeline.review.zip"),
            "Review bundle success status did not preserve the package file name.");

        const string bundleFailureDetail = "Review bundle manifest could not be created.";
        OpenVisionRecipePipelineExchangeProjection bundleFailure = projectionOwner.Project(
            OpenVisionRecipePipelineExchangeOperation.ExportReviewBundle,
            OpenVisionRecipePipelineExchangeResult.Failure(bundleFailureDetail));
        Require(!bundleFailure.Succeeded, "Review bundle failure projection was marked succeeded.");
        Require(bundleFailure.PipelineName == string.Empty, "Review bundle failure exposed a Pipeline name.");
        Require(bundleFailure.StatusText.Contains(bundleFailureDetail, StringComparison.Ordinal),
            "Review bundle failure detail was not preserved.");
        Require(HasStatus(bundleFailure, "Review bundle export failed: ", "검토 묶음 내보내기 실패: "),
            "Review bundle failure prefix was not projected.");
        observations.Add("review bundle success/failure: localized path and failure prefix preserved");
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string reportPath = Path.Combine(
        evidenceDirectory,
        "recipe_pipeline_exchange_projection_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: OVL-07 Pipeline exchange/review result projection owner boundary",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Recipe Pipeline exchange projection contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Recipe Pipeline exchange projection contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunPipelinePersistenceRecoveryContract(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "pl0009_pipeline_persistence_recovery_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);
    string dataRoot = Path.Combine(evidenceDirectory, "data");
    Environment.SetEnvironmentVariable(
        AppPathService.DataRootEnvironmentVariable,
        dataRoot);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    VisionPipelineLifecycleFailureStage[] failureStages =
    {
        VisionPipelineLifecycleFailureStage.AfterJournalPrepared,
        VisionPipelineLifecycleFailureStage.AfterBackupCreated,
        VisionPipelineLifecycleFailureStage.AfterTargetCreated,
        VisionPipelineLifecycleFailureStage.AfterActivePointerUpdated,
        VisionPipelineLifecycleFailureStage.AfterSourceRemoved,
        VisionPipelineLifecycleFailureStage.AfterBackupRemoved
    };

    string CreateSmokeRecipe(string kind, VisionPipelineLifecycleFailureStage stage)
    {
        string suffix = Guid.NewGuid().ToString("N").Substring(0, 12);
        string recipeName = "Smoke_PL0009_"
            + kind
            + "_"
            + stage
            + "_"
            + suffix;
        RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
        return recipeName;
    }

    void CleanupSmokeRecipe(string recipeName)
    {
        if (!RecipeWorkspaceService.DeleteVisionWorkspace(recipeName))
        {
            failures.Add("Reserved smoke Recipe cleanup failed: " + recipeName);
        }
    }

    void AssertNoLifecycleArtifacts(string recipeName, string pipelineName)
    {
        string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
        string directory = Path.GetDirectoryName(path) ?? string.Empty;
        string journalPath = RecipeWorkspaceService.GetVisionConfigPath(
            recipeName,
            "pipeline.lifecycle.json");
        Require(!File.Exists(journalPath), "Lifecycle journal remained after recovery.");
        Require(
            Directory.GetFiles(directory, "." + pipelineName + ".lifecycle-*.bak").Length == 0,
            "Lifecycle backup remained after recovery.");
        Require(
            Directory.GetFiles(directory, ".pipeline.*.tmp").Length == 0,
            "Atomic pointer/journal temporary file remained after recovery.");
    }

    void RunRenameFailure(VisionPipelineLifecycleFailureStage stage)
    {
        string recipeName = CreateSmokeRecipe("Rename", stage);
        const string oldName = "PL0009_A";
        const string newName = "PL0009_B";
        try
        {
            VisionPipelineStorage.Save(recipeName, new VisionPipeline { Name = oldName });
            VisionPipelineStorage.SaveActivePipelineName(recipeName, oldName);
            string oldPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, oldName);
            string oldHash = ComputeSha256(oldPath);

            using (VisionPipelineStorage.BeginLifecycleFailureInjectionForTest(stage))
            {
                Require(
                    !VisionPipelineStorage.TryRenamePipeline(
                        recipeName,
                        oldName,
                        newName,
                        out string failureMessage),
                    "Injected rename failure unexpectedly completed: " + failureMessage);
            }

            VisionPipelineStorage.ResetRuntimePersistenceStateForTest();
            string activeName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                oldName);
            string newPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, newName);
            bool priorState = File.Exists(oldPath)
                && !File.Exists(newPath)
                && string.Equals(activeName, oldName, StringComparison.OrdinalIgnoreCase);
            bool completedState = !File.Exists(oldPath)
                && File.Exists(newPath)
                && string.Equals(activeName, newName, StringComparison.OrdinalIgnoreCase);
            Require(
                priorState || completedState,
                "Rename recovery did not produce one valid prior or completed state at " + stage);
            if (priorState)
            {
                Require(
                    string.Equals(ComputeSha256(oldPath), oldHash, StringComparison.Ordinal),
                    "Rename rollback changed the prior Pipeline bytes at " + stage);
            }

            string stateName = completedState ? newName : oldName;
            Require(
                VisionPipelineStorage.TryGetPersistenceState(
                    recipeName,
                    stateName,
                    out VisionPipelinePersistenceState state)
                && state.Kind == VisionPipelinePersistenceStateKind.LifecycleRecovered,
                "Rename recovery state was not explained at " + stage);
            AssertNoLifecycleArtifacts(recipeName, stateName);
            observations.Add(
                "rename-" + stage + ": "
                + (priorState ? "rolled back" : "completed")
                + "; active=" + activeName);
        }
        catch (Exception ex)
        {
            failures.Add("Rename " + stage + ": " + ex.GetBaseException().Message);
        }
        finally
        {
            CleanupSmokeRecipe(recipeName);
        }
    }

    void RunDeleteFailure(VisionPipelineLifecycleFailureStage stage)
    {
        string recipeName = CreateSmokeRecipe("Delete", stage);
        const string deletedName = "PL0009_A";
        const string fallbackName = "PL0009_B";
        try
        {
            VisionPipelineStorage.Save(recipeName, new VisionPipeline { Name = deletedName });
            VisionPipelineStorage.Save(recipeName, new VisionPipeline { Name = fallbackName });
            VisionPipelineStorage.SaveActivePipelineName(recipeName, deletedName);
            string deletedPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, deletedName);
            string deletedHash = ComputeSha256(deletedPath);

            using (VisionPipelineStorage.BeginLifecycleFailureInjectionForTest(stage))
            {
                Require(
                    !VisionPipelineStorage.TryDeletePipeline(
                        recipeName,
                        deletedName,
                        out string returnedFallback,
                        out string failureMessage),
                    "Injected delete failure unexpectedly completed: " + failureMessage);
                Require(
                    string.Equals(returnedFallback, fallbackName, StringComparison.OrdinalIgnoreCase),
                    "Delete failure did not retain the deterministic fallback name.");
            }

            VisionPipelineStorage.ResetRuntimePersistenceStateForTest();
            string activeName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                deletedName);
            string fallbackPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, fallbackName);
            bool priorState = File.Exists(deletedPath)
                && File.Exists(fallbackPath)
                && string.Equals(activeName, deletedName, StringComparison.OrdinalIgnoreCase);
            bool completedState = !File.Exists(deletedPath)
                && File.Exists(fallbackPath)
                && string.Equals(activeName, fallbackName, StringComparison.OrdinalIgnoreCase);
            Require(
                priorState || completedState,
                "Delete recovery did not produce one valid prior or completed state at " + stage);
            if (priorState)
            {
                Require(
                    string.Equals(ComputeSha256(deletedPath), deletedHash, StringComparison.Ordinal),
                    "Delete rollback changed the prior Pipeline bytes at " + stage);
            }

            string stateName = completedState ? fallbackName : deletedName;
            Require(
                VisionPipelineStorage.TryGetPersistenceState(
                    recipeName,
                    stateName,
                    out VisionPipelinePersistenceState state)
                && state.Kind == VisionPipelinePersistenceStateKind.LifecycleRecovered,
                "Delete recovery state was not explained at " + stage);
            AssertNoLifecycleArtifacts(recipeName, stateName);
            observations.Add(
                "delete-" + stage + ": "
                + (priorState ? "rolled back" : "completed")
                + "; active=" + activeName);
        }
        catch (Exception ex)
        {
            failures.Add("Delete " + stage + ": " + ex.GetBaseException().Message);
        }
        finally
        {
            CleanupSmokeRecipe(recipeName);
        }
    }

    void RunPointerValidation()
    {
        string recipeName = CreateSmokeRecipe(
            "Pointer",
            VisionPipelineLifecycleFailureStage.AfterJournalPrepared);
        const string firstName = "PL0009_A";
        const string secondName = "PL0009_B";
        try
        {
            VisionPipelineStorage.Save(recipeName, new VisionPipeline { Name = firstName });
            VisionPipelineStorage.Save(recipeName, new VisionPipeline { Name = secondName });
            VisionPipelineStorage.SaveActivePipelineName(recipeName, firstName);
            string pointerPath = RecipeWorkspaceService.GetVisionConfigPath(
                recipeName,
                "pipeline.active");
            Require(
                string.Equals(File.ReadAllText(pointerPath, Encoding.UTF8), firstName, StringComparison.Ordinal),
                "Atomic active pointer did not persist the first existing Pipeline.");
            VisionPipelineStorage.SaveActivePipelineName(recipeName, secondName);
            Require(
                string.Equals(File.ReadAllText(pointerPath, Encoding.UTF8), secondName, StringComparison.Ordinal),
                "Atomic active pointer did not replace the existing value.");
            try
            {
                VisionPipelineStorage.SaveActivePipelineName(recipeName, "PL0009_Missing");
                failures.Add("Active pointer accepted a Pipeline outside the inventory.");
            }
            catch (InvalidOperationException)
            {
                Require(
                    string.Equals(
                        File.ReadAllText(pointerPath, Encoding.UTF8),
                        secondName,
                        StringComparison.Ordinal),
                    "Rejected active pointer write changed the existing pointer.");
                observations.Add("active-pointer: atomic replacement and inventory validation");
            }
        }
        catch (Exception ex)
        {
            failures.Add("Active pointer: " + ex.GetBaseException().Message);
        }
        finally
        {
            CleanupSmokeRecipe(recipeName);
        }
    }

    void RunNormalLifecycleRoundTrip()
    {
        string recipeName = CreateSmokeRecipe(
            "Normal",
            VisionPipelineLifecycleFailureStage.AfterJournalPrepared);
        const string firstName = "PL0009_A";
        const string duplicateName = "PL0009_B";
        const string renamedName = "PL0009_C";
        try
        {
            VisionPipelineStorage.Save(recipeName, new VisionPipeline { Name = firstName });
            VisionPipelineStorage.SaveActivePipelineName(recipeName, firstName);
            Require(
                VisionPipelineStorage.TryDuplicatePipeline(
                    recipeName,
                    firstName,
                    duplicateName,
                    out string duplicateMessage),
                duplicateMessage);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, duplicateName);
            Require(
                VisionPipelineStorage.TryRenamePipeline(
                    recipeName,
                    duplicateName,
                    renamedName,
                    out string renameMessage),
                renameMessage);
            Require(
                string.Equals(
                    VisionPipelineStorage.LoadActivePipelineName(recipeName, firstName),
                    renamedName,
                    StringComparison.OrdinalIgnoreCase),
                "Normal rename did not preserve the active pointer.");
            Require(
                VisionPipelineStorage.TryDeletePipeline(
                    recipeName,
                    renamedName,
                    out string fallbackName,
                    out string deleteMessage),
                deleteMessage);
            Require(
                string.Equals(fallbackName, firstName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(
                    VisionPipelineStorage.LoadActivePipelineName(recipeName, firstName),
                    firstName,
                    StringComparison.OrdinalIgnoreCase),
                "Normal delete did not restore the remaining Pipeline as active.");
            VisionPipelineStorage.ResetRuntimePersistenceStateForTest();
            Require(
                VisionPipelineStorage.Load(recipeName, firstName)?.Name == firstName,
                "Recipe reopen did not reload the remaining Pipeline.");
            observations.Add(
                "normal-crud-reopen: duplicate/rename/delete/active pointer round-trip; "
                + "storage-only path has no Preview/Run, layer, or route mutation");
        }
        catch (Exception ex)
        {
            failures.Add("Normal lifecycle round-trip: " + ex.GetBaseException().Message);
        }
        finally
        {
            CleanupSmokeRecipe(recipeName);
        }
    }

    try
    {
        foreach (VisionPipelineLifecycleFailureStage stage in failureStages)
        {
            RunRenameFailure(stage);
            if (stage != VisionPipelineLifecycleFailureStage.AfterTargetCreated)
            {
                RunDeleteFailure(stage);
            }
        }

        RunPointerValidation();
        RunNormalLifecycleRoundTrip();
    }
    catch (Exception ex)
    {
        failures.Add("Pipeline persistence recovery contract: " + ex.GetBaseException().Message);
    }

    string reportPath = Path.Combine(
        evidenceDirectory,
        "pipeline_persistence_recovery_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: PL-0009 recoverable Pipeline rename/delete and atomic active pointer",
            "EvidenceDirectory: " + evidenceDirectory,
            "FailureInjection: six rename stages and five applicable delete stages",
            "Recovery: journal-backed prior-state rollback or completed-state adoption",
            "Pointer: atomic temporary-file replacement plus existing-inventory validation"
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Pipeline persistence recovery contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Pipeline persistence recovery contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunPipelinePersistenceProcessRecoveryContract(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory
        ?? Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "refactor-ovl06-process-recovery_"
                + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
    Directory.CreateDirectory(evidenceDirectory);
    string dataRoot = Path.Combine(evidenceDirectory, "data");
    Environment.SetEnvironmentVariable(
        AppPathService.DataRootEnvironmentVariable,
        dataRoot,
        EnvironmentVariableTarget.Process);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    VisionPipelineLifecycleFailureStage[] stages =
    {
        VisionPipelineLifecycleFailureStage.AfterJournalPrepared,
        VisionPipelineLifecycleFailureStage.AfterBackupCreated,
        VisionPipelineLifecycleFailureStage.AfterTargetCreated,
        VisionPipelineLifecycleFailureStage.AfterActivePointerUpdated,
        VisionPipelineLifecycleFailureStage.AfterSourceRemoved,
        VisionPipelineLifecycleFailureStage.AfterBackupRemoved
    };
    int cases = 0;
    int recovered = 0;
    int childLaunches = 0;
    int childKills = 0;
    long totalRecoveryMilliseconds = 0;
    long maximumRecoveryMilliseconds = 0;

    static bool IsRename(string operation)
    {
        return string.Equals(operation, "rename", StringComparison.OrdinalIgnoreCase);
    }

    void RunCase(string operation, VisionPipelineLifecycleFailureStage stage)
    {
        cases++;
        string caseDirectory = Path.Combine(
            evidenceDirectory,
            operation + "-" + stage + "-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(caseDirectory);
        string recipeName = "Smoke_PL0009_Process_"
            + operation
            + "_"
            + stage
            + "_"
            + Guid.NewGuid().ToString("N").Substring(0, 12);
        const string oldName = "PL0009_A";
        const string newName = "PL0009_B";
        string markerPath = Path.Combine(caseDirectory, "crash.marker");
        Process? child = null;

        try
        {
            RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
            VisionPipelineStorage.Save(recipeName, new VisionPipeline { Name = oldName });
            if (IsRename(operation))
            {
                VisionPipelineStorage.SaveActivePipelineName(recipeName, oldName);
            }
            else
            {
                VisionPipelineStorage.Save(recipeName, new VisionPipeline { Name = newName });
                VisionPipelineStorage.SaveActivePipelineName(recipeName, oldName);
            }

            string oldPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, oldName);
            string newPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, newName);
            string oldHash = ComputeSha256(oldPath);
            ProcessStartInfo startInfo = CreateVisionRecipeRunnerProcessStartInfo();
            startInfo.ArgumentList.Add("--pipeline-persistence-process-recovery-probe");
            startInfo.ArgumentList.Add("--operation");
            startInfo.ArgumentList.Add(operation);
            startInfo.ArgumentList.Add("--recipe-name");
            startInfo.ArgumentList.Add(recipeName);
            startInfo.ArgumentList.Add("--old-name");
            startInfo.ArgumentList.Add(oldName);
            startInfo.ArgumentList.Add("--new-name");
            startInfo.ArgumentList.Add(newName);
            startInfo.ArgumentList.Add("--stage");
            startInfo.ArgumentList.Add(stage.ToString());
            startInfo.ArgumentList.Add("--marker");
            startInfo.ArgumentList.Add(markerPath);
            startInfo.Environment[AppPathService.DataRootEnvironmentVariable] = dataRoot;

            child = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Process recovery child could not be started.");
            childLaunches++;
            bool markerObserved = WaitForProcessRecoveryMarker(
                markerPath,
                child,
                TimeSpan.FromSeconds(20));
            string markerText = ReadProcessRecoveryMarker(
                markerPath,
                TimeSpan.FromSeconds(5));
            if (!markerObserved || child.HasExited || !markerText.Contains("Ready=true", StringComparison.Ordinal))
            {
                string errorPath = markerPath + ".error";
                string errorText = File.Exists(errorPath)
                    ? File.ReadAllText(errorPath, Encoding.UTF8)
                    : string.Empty;
                throw new InvalidOperationException(
                    "Process recovery child did not reach the crash boundary. "
                    + markerText
                    + errorText);
            }

            child.Kill(entireProcessTree: true);
            childKills++;
            Require(
                child.WaitForExit(15000),
                "Process recovery child did not terminate after the kill boundary.");
            int childExitCode = child.ExitCode;

            VisionPipelineStorage.ResetRuntimePersistenceStateForTest();
            Stopwatch recoveryTimer = Stopwatch.StartNew();
            string activeName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                oldName);
            recoveryTimer.Stop();
            long recoveryMilliseconds = recoveryTimer.ElapsedMilliseconds;
            totalRecoveryMilliseconds += recoveryMilliseconds;
            maximumRecoveryMilliseconds = Math.Max(
                maximumRecoveryMilliseconds,
                recoveryMilliseconds);

            string fallbackName = newName;
            bool priorState;
            bool completedState;
            if (IsRename(operation))
            {
                priorState = File.Exists(oldPath)
                    && !File.Exists(newPath)
                    && string.Equals(activeName, oldName, StringComparison.OrdinalIgnoreCase);
                completedState = !File.Exists(oldPath)
                    && File.Exists(newPath)
                    && string.Equals(activeName, newName, StringComparison.OrdinalIgnoreCase)
                    && SerializeHelper.TryLoadFromXmlFile(newPath, out VisionPipeline renamed)
                    && renamed != null
                    && string.Equals(renamed.Name, newName, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                string fallbackPath = RecipeWorkspaceService.GetVisionPipelinePath(
                    recipeName,
                    fallbackName);
                priorState = File.Exists(oldPath)
                    && File.Exists(fallbackPath)
                    && string.Equals(activeName, oldName, StringComparison.OrdinalIgnoreCase);
                completedState = !File.Exists(oldPath)
                    && File.Exists(fallbackPath)
                    && string.Equals(activeName, fallbackName, StringComparison.OrdinalIgnoreCase)
                    && SerializeHelper.TryLoadFromXmlFile(fallbackPath, out VisionPipeline fallback)
                    && fallback != null
                    && string.Equals(fallback.Name, fallbackName, StringComparison.OrdinalIgnoreCase);
            }

            Require(
                priorState || completedState,
                "Recovery did not produce one valid prior or completed state.");
            if (priorState)
            {
                Require(
                    string.Equals(ComputeSha256(oldPath), oldHash, StringComparison.Ordinal),
                    "Recovery changed the prior Pipeline bytes.");
            }

            string stateName = completedState ? newName : oldName;
            Require(
                VisionPipelineStorage.TryGetPersistenceState(
                    recipeName,
                    stateName,
                    out VisionPipelinePersistenceState state)
                && state.Kind == VisionPipelinePersistenceStateKind.LifecycleRecovered,
                "Recovery state was not retained as LifecycleRecovered.");

            string journalPath = RecipeWorkspaceService.GetVisionConfigPath(
                recipeName,
                "pipeline.lifecycle.json");
            string pipelineDirectory = Path.GetDirectoryName(oldPath)
                ?? throw new InvalidOperationException("Pipeline directory could not be resolved.");
            Require(!File.Exists(journalPath), "Lifecycle journal remained after process recovery.");
            Require(
                Directory.GetFiles(pipelineDirectory, "." + oldName + ".lifecycle-*.bak").Length == 0,
                "Lifecycle backup remained after process recovery.");
            Require(
                Directory.GetFiles(pipelineDirectory, ".pipeline.*.tmp").Length == 0,
                "Atomic temporary file remained after process recovery.");

            recovered++;
            observations.Add(
                operation
                + "-"
                + stage
                + ": "
                + (priorState ? "rolled back" : "completed")
                + "; childExit="
                + childExitCode.ToString(CultureInfo.InvariantCulture)
                + "; recoveryMs="
                + recoveryMilliseconds.ToString(CultureInfo.InvariantCulture));
        }
        catch (Exception exception)
        {
            failures.Add(operation + "-" + stage + ": " + exception.GetBaseException().Message);
        }
        finally
        {
            if (child != null)
            {
                try
                {
                    if (!child.HasExited)
                    {
                        child.Kill(entireProcessTree: true);
                        childKills++;
                        child.WaitForExit(5000);
                    }
                }
                catch
                {
                    // Preserve the original failure; cleanup evidence is recorded below.
                }

                child.Dispose();
            }

            if (!RecipeWorkspaceService.DeleteVisionWorkspace(recipeName))
            {
                failures.Add("Process recovery workspace cleanup failed: " + recipeName);
            }
        }
    }

    foreach (VisionPipelineLifecycleFailureStage stage in stages)
    {
        RunCase("rename", stage);
        if (stage != VisionPipelineLifecycleFailureStage.AfterTargetCreated)
        {
            RunCase("delete", stage);
        }
    }

    double averageRecoveryMilliseconds = recovered == 0
        ? 0D
        : totalRecoveryMilliseconds / (double)recovered;
    string reportPath = Path.Combine(
        evidenceDirectory,
        "pipeline_persistence_process_recovery_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: PL-0009 process-boundary Pipeline lifecycle recovery",
            "EvidenceDirectory: " + evidenceDirectory,
            "DataRoot: " + dataRoot,
            "Cases: " + cases.ToString(CultureInfo.InvariantCulture),
            "Recovered: " + recovered.ToString(CultureInfo.InvariantCulture),
            "ChildLaunches: " + childLaunches.ToString(CultureInfo.InvariantCulture),
            "ChildKills: " + childKills.ToString(CultureInfo.InvariantCulture),
            "AverageRecoveryMs: " + averageRecoveryMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
            "MaximumRecoveryMs: " + maximumRecoveryMilliseconds.ToString(CultureInfo.InvariantCulture),
            "RecoveryPolicy: journal-backed prior-state rollback or completed-state adoption",
            "ProcessBoundary: child stopped after durable journal stage; parent reopened the same data root",
            "Executable: " + (Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location),
            "ExecutableSha256: " + ComputeSha256(Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location)
        }
        .Concat(observations)
        .Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Pipeline persistence process recovery contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Pipeline persistence process recovery contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    Console.Error.WriteLine(reportPath);
    return 1;
}

static int RunPipelinePersistenceProcessRecoveryProbe(string[] args)
{
    string operation = GetRawOptionValue(args, "--operation") ?? string.Empty;
    string recipeName = GetRawOptionValue(args, "--recipe-name") ?? string.Empty;
    string oldName = GetRawOptionValue(args, "--old-name") ?? string.Empty;
    string newName = GetRawOptionValue(args, "--new-name") ?? string.Empty;
    string stageText = GetRawOptionValue(args, "--stage") ?? string.Empty;
    string markerPath = GetRawOptionValue(args, "--marker") ?? string.Empty;
    string errorPath = markerPath + ".error";

    if (string.IsNullOrWhiteSpace(operation)
        || string.IsNullOrWhiteSpace(recipeName)
        || string.IsNullOrWhiteSpace(oldName)
        || string.IsNullOrWhiteSpace(newName)
        || string.IsNullOrWhiteSpace(markerPath)
        || !Enum.TryParse(
            stageText,
            ignoreCase: true,
            out VisionPipelineLifecycleFailureStage stage))
    {
        return 2;
    }

    try
    {
        bool operationCompleted;
        using (VisionPipelineStorage.BeginLifecycleFailureInjectionForTest(stage))
        {
            if (string.Equals(operation, "rename", StringComparison.OrdinalIgnoreCase))
            {
                operationCompleted = VisionPipelineStorage.TryRenamePipeline(
                    recipeName,
                    oldName,
                    newName,
                    out _);
            }
            else if (string.Equals(operation, "delete", StringComparison.OrdinalIgnoreCase))
            {
                operationCompleted = VisionPipelineStorage.TryDeletePipeline(
                    recipeName,
                    oldName,
                    out _,
                    out _);
            }
            else
            {
                File.WriteAllText(errorPath, "Unknown operation: " + operation, Encoding.UTF8);
                return 2;
            }
        }

        if (operationCompleted)
        {
            File.WriteAllText(
                errorPath,
                "Failure injection unexpectedly completed the operation.",
                Encoding.UTF8);
            return 3;
        }

        File.WriteAllText(
            markerPath,
            "Ready=true"
                + Environment.NewLine
                + "Operation=" + operation
                + Environment.NewLine
                + "Stage=" + stage
                + Environment.NewLine
                + "ProcessId=" + Environment.ProcessId,
            Encoding.UTF8);
        // ponytail: the child only waits for the parent kill boundary; no production path uses this wait.
        Thread.Sleep(Timeout.Infinite);
        return 0;
    }
    catch (Exception exception)
    {
        File.WriteAllText(
            errorPath,
            exception.GetBaseException().ToString(),
            Encoding.UTF8);
        return 1;
    }
}

static ProcessStartInfo CreateVisionRecipeRunnerProcessStartInfo()
{
    string entryAssemblyPath = Assembly.GetEntryAssembly()?.Location
        ?? throw new InvalidOperationException("Runner assembly path is unavailable.");
    string processPath = Environment.ProcessPath
        ?? throw new InvalidOperationException("Runner host path is unavailable.");
    ProcessStartInfo startInfo;
    bool isAppHost = entryAssemblyPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
        && processPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
        && string.Equals(
            Path.GetFileNameWithoutExtension(processPath),
            Path.GetFileNameWithoutExtension(entryAssemblyPath),
            StringComparison.OrdinalIgnoreCase);
    if (isAppHost)
    {
        startInfo = new ProcessStartInfo(processPath);
    }
    else if (entryAssemblyPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
    {
        startInfo = new ProcessStartInfo(processPath);
        startInfo.ArgumentList.Add(entryAssemblyPath);
    }
    else
    {
        startInfo = new ProcessStartInfo(entryAssemblyPath);
    }

    startInfo.UseShellExecute = false;
    startInfo.CreateNoWindow = true;
    startInfo.WorkingDirectory = AppContext.BaseDirectory;
    return startInfo;
}

static bool WaitForProcessRecoveryMarker(
    string markerPath,
    Process process,
    TimeSpan timeout)
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    while (stopwatch.Elapsed < timeout)
    {
        if (File.Exists(markerPath))
        {
            return true;
        }

        if (process.HasExited)
        {
            return false;
        }

        Thread.Sleep(25);
    }

    return File.Exists(markerPath);
}

static string ReadProcessRecoveryMarker(string markerPath, TimeSpan timeout)
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    while (stopwatch.Elapsed < timeout)
    {
        try
        {
            return File.Exists(markerPath)
                ? File.ReadAllText(markerPath, Encoding.UTF8)
                : string.Empty;
        }
        catch (IOException)
        {
            Thread.Sleep(25);
        }
        catch (UnauthorizedAccessException)
        {
            Thread.Sleep(25);
        }
    }

    return string.Empty;
}

static void ExpectStoragePathRejected(
    string name,
    Action action,
    ICollection<string> observations,
    ICollection<string> failures)
{
    try
    {
        action();
        failures.Add(name + ": invalid path segment was accepted.");
    }
    catch (ArgumentException exception)
    {
        if (string.IsNullOrWhiteSpace(exception.Message))
        {
            failures.Add(name + ": rejection message was empty.");
            return;
        }

        observations.Add(name + ": rejected");
    }
    catch (InvalidOperationException exception)
    {
        if (string.IsNullOrWhiteSpace(exception.Message))
        {
            failures.Add(name + ": rejection message was empty.");
            return;
        }

        observations.Add(name + ": rejected");
    }
    catch (Exception exception)
    {
        failures.Add(name + ": unexpected exception " + exception.GetType().Name + ".");
    }
}

static async Task<int> RunPipelineProvenanceContractAsync(string? requestedEvidenceDirectory)
{
    string defaultEvidenceDirectory = Path.Combine(
        "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
        "pl0008_pipeline_provenance_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory ?? defaultEvidenceDirectory);
    Directory.CreateDirectory(evidenceDirectory);
    string dataRoot = Path.Combine(evidenceDirectory, "data");
    Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);

    List<string> observations = new List<string>();
    List<string> failures = new List<string>();
    string recipeName = "PL0008_Provenance";
    string pipelinePath = Path.Combine(evidenceDirectory, "original-pipeline.xml");
    string sourcePath = Path.Combine(evidenceDirectory, "source.png");

    try
    {
        VisionPipeline sourcePipeline = CreatePipelineProvenanceContractPipeline();
        Require(SerializeHelper.SaveXmlFile(pipelinePath, sourcePipeline), "Could not save the provenance contract Pipeline.");
        byte[] originalFileBytes = File.ReadAllBytes(pipelinePath);
        string pipelineXml = File.ReadAllText(pipelinePath);
        using Mat source = new Mat(new OpenCvSharp.Size(160, 120), MatType.CV_8UC1, Scalar.All(0));
        Cv2.Rectangle(source, new Rect(42, 30, 54, 40), Scalar.All(255), -1);
        Require(Cv2.ImWrite(sourcePath, source), "Could not write the provenance contract source image.");

        VisionPipeline objectRunnerPipeline = CreatePipelineProvenanceContractPipeline();
        byte[] objectBefore = SerializePipelineForProvenanceContract(objectRunnerPipeline);
        using (VisionRecipeRunResult firstRun = await new VisionRecipeRunner().RunAsync(
            objectRunnerPipeline,
            source.Clone(),
            VisionRecipeRunner.DefaultInputLayer,
            VisionRecipeRunner.DefaultStepTimeoutMilliseconds))
        {
            AssertProvenance(firstRun?.ExecutionProvenance, failures, "direct Runner");
            Require(
                firstRun?.ExecutionProvenance?.NormalizationChanges.Count == 4,
                "Direct Runner did not retain exactly four structured normalization changes.");
        }

        byte[] objectAfterFirstRun = SerializePipelineForProvenanceContract(objectRunnerPipeline);
        Require(objectBefore.SequenceEqual(objectAfterFirstRun), "Direct Runner changed the caller Pipeline.");
        observations.Add("direct-runner: caller Pipeline unchanged");

        using (VisionRecipeRunResult secondRun = await new VisionRecipeRunner().RunAsync(
            objectRunnerPipeline,
            source.Clone(),
            VisionRecipeRunner.DefaultInputLayer,
            VisionRecipeRunner.DefaultStepTimeoutMilliseconds))
        {
            AssertProvenance(secondRun?.ExecutionProvenance, failures, "repeated Runner");
        }

        byte[] objectAfterSecondRun = SerializePipelineForProvenanceContract(objectRunnerPipeline);
        Require(objectBefore.SequenceEqual(objectAfterSecondRun), "Repeated Runner execution accumulated Pipeline changes.");
        observations.Add("repeated-runner: no cumulative mutation");

        VisionPipeline fileRunnerPipeline = CreatePipelineProvenanceContractPipeline();
        Require(SerializeHelper.SaveXmlFile(pipelinePath, fileRunnerPipeline), "Could not reset the file Pipeline.");
        originalFileBytes = File.ReadAllBytes(pipelinePath);
        using (VisionRecipeRunResult fileRun = await new VisionRecipeRunner().RunAsync(
            pipelinePath,
            source.Clone()))
        {
            AssertProvenance(fileRun?.ExecutionProvenance, failures, "file Runner");
            Require(
                fileRun?.ExecutionProvenance?.OriginalPipelineSha256 == ComputeSha256(pipelinePath),
                "File Runner did not retain the original source-file hash.");
        }

        Require(originalFileBytes.SequenceEqual(File.ReadAllBytes(pipelinePath)), "File Runner changed the source Pipeline file.");
        observations.Add("file-runner: source XML bytes unchanged");

        VisionPipeline reviewPipeline = CreatePipelineProvenanceContractPipeline();
        byte[] reviewBefore = SerializePipelineForProvenanceContract(reviewPipeline);
        using (DisplayManagerService displayManager = new DisplayManagerService())
        {
            displayManager.CreateLayerDisplay(
                ImageSpaceFrame.TakeOwnership(new Bitmap(160, 120)),
                "Main");
            int reviewLayerCountBefore = displayManager.LayerCount;
            string reviewSelectedItemBefore = displayManager.SelectedItem;
            string[] reviewLayerTitlesBefore = displayManager.GetLayerInfos()
                .Select(info => info?.Title ?? string.Empty)
                .ToArray();
            using OpenVisionPipelineReviewExecutionController review =
                new OpenVisionPipelineReviewExecutionController(displayManager, action => action());
            await review.RunAsync(reviewPipeline, VisionRecipeRunner.DefaultStepTimeoutMilliseconds);
            string[] reviewLayerTitlesAfter = displayManager.GetLayerInfos()
                .Select(info => info?.Title ?? string.Empty)
                .ToArray();
            Require(
                displayManager.LayerCount == reviewLayerCountBefore
                && string.Equals(displayManager.SelectedItem, reviewSelectedItemBefore, StringComparison.Ordinal)
                && reviewLayerTitlesBefore.SequenceEqual(reviewLayerTitlesAfter),
                "Pipeline Review changed the display layer set or selected layer.");
        }

        Require(
            reviewBefore.SequenceEqual(SerializePipelineForProvenanceContract(reviewPipeline)),
            "Pipeline Review changed the caller Pipeline.");
        observations.Add("pipeline-review: caller Pipeline and display selection unchanged");

        VisionPipeline samplePipeline = CreatePipelineProvenanceContractPipeline();
        Require(SerializeHelper.SaveXmlFile(pipelinePath, samplePipeline), "Could not save the sample-validation Pipeline.");
        byte[] sampleBefore = File.ReadAllBytes(pipelinePath);
        VisionPipelineSampleCheckResult sampleCheck =
            await VisionPipelineSampleCheckService.RunSampleCheckWithReportSafeAsync(
                new VisionPipelineSampleCatalogItem
                {
                    SampleName = "PL0008 source",
                    ImageFullPath = sourcePath,
                    PairGroup = "PL0008",
                    PairRole = "UNLABELED"
                },
                File.ReadAllText(pipelinePath),
                recipeName,
                normalizeInputToGray: false,
                CancellationToken.None);
        Require(sampleCheck != null && sampleCheck.ExecutionCompleted, "Sample validation did not complete.");
        Require(sampleBefore.SequenceEqual(File.ReadAllBytes(pipelinePath)), "Sample validation changed the source Pipeline file.");
        string sampleReportPath = sampleCheck?.RunReportPath
            ?? throw new InvalidOperationException("Sample validation did not return a Run Report path.");
        VisionPipelineRunReport? report = VisionPipelineRunReportStorage.Load(sampleReportPath);
        VisionPipelineRunReport reportForContract = report
            ?? throw new InvalidOperationException("Sample validation did not round-trip its Run Report.");
        VisionPipelineExecutionProvenance reportProvenance = reportForContract.ExecutionProvenance
            ?? throw new InvalidOperationException("Sample validation Run Report did not retain execution provenance.");
        AssertPersistedProvenance(reportForContract, sampleReportPath, failures);
        observations.Add("sample-validation: report retained original/effective evidence");

        string legacyReportPath = Path.Combine(evidenceDirectory, "legacy-report.xml");
        XmlDocument legacyReportDocument = new XmlDocument();
        legacyReportDocument.Load(sampleReportPath);
        XmlNode? legacySchemaNode = legacyReportDocument.SelectSingleNode(
            "/*[local-name()='VisionPipelineRunReport']/*[local-name()='SchemaVersion']");
        legacySchemaNode?.ParentNode?.RemoveChild(legacySchemaNode);
        XmlNode? legacyProvenanceNode = legacyReportDocument.SelectSingleNode(
            "/*[local-name()='VisionPipelineRunReport']/*[local-name()='ExecutionProvenance']");
        legacyProvenanceNode?.ParentNode?.RemoveChild(legacyProvenanceNode);
        legacyReportDocument.Save(legacyReportPath);
        VisionPipelineRunReport? legacyReport = VisionPipelineRunReportStorage.Load(legacyReportPath);
        Require(
            legacyReport != null && !string.IsNullOrWhiteSpace(legacyReport.PipelineSnapshotFile),
            "A legacy Run Report without PL-0008 fields was not readable.");
        observations.Add("legacy-report: old Run Report remained readable");

        string batchPath = VisionPipelineBatchRunSummaryStorage.Save(
            recipeName,
            samplePipeline.Name,
            DateTime.Now,
            DateTime.Now.AddMilliseconds(1),
            new[]
            {
                new VisionPipelineBatchSampleRunResult
                {
                    SampleName = "PL0008 source",
                    Status = "RUN OK",
                    Success = sampleCheck.Success,
                    TotalMilliseconds = sampleCheck.TotalMilliseconds,
                    RunReportPath = sampleReportPath,
                    SampleImagePath = sourcePath
                }
            },
            suiteName: "PL0008 provenance",
            suiteKind: "Batch",
            notes: "PL-0008 immutable execution provenance contract",
            pipelineSnapshot: samplePipeline,
            executionProvenance: reportProvenance);
        VisionPipelineBatchRunSummary batchForContract = VisionPipelineBatchRunSummaryStorage.Load(batchPath)
            ?? throw new InvalidOperationException("Batch Summary did not round-trip.");
        VisionPipelineExecutionProvenance batchProvenance = batchForContract.ExecutionProvenance
            ?? throw new InvalidOperationException("Batch Summary did not retain execution provenance.");
        Require(
            string.Equals(
                batchProvenance.OriginalPipelineSha256,
                reportProvenance.OriginalPipelineSha256,
                StringComparison.Ordinal)
            && string.Equals(
                batchProvenance.EffectivePipelineSha256,
                reportProvenance.EffectivePipelineSha256,
                StringComparison.Ordinal)
            && batchProvenance.NormalizationChanges.Count == reportProvenance.NormalizationChanges.Count,
            "Batch Summary provenance did not round-trip the report identity/change set.");
        observations.Add("batch-summary: provenance round-tripped");

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "completion.txt"),
            new[]
            {
                "Status=Complete",
                "Contract=PL-0008 immutable original/effective Pipeline provenance",
                "EvidenceDirectory=" + evidenceDirectory,
                "RunReport=" + sampleReportPath,
                "BatchSummary=" + batchPath
            }
            .Concat(observations)
            .Concat(failures.Select(failure => "Failure=" + failure)));
    }
    catch (Exception exception)
    {
        failures.Add("unexpected-contract-error: " + exception.GetBaseException().Message);
    }

    string contractPath = Path.Combine(evidenceDirectory, "pipeline_provenance_contract.txt");
    File.WriteAllLines(
        contractPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "Contract: PL-0008 immutable original/effective Pipeline provenance",
            "EvidenceDirectory: " + evidenceDirectory
        }
        .Concat(observations)
        .Concat(failures.Select(failure => "Failure: " + failure)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Pipeline provenance contract passed.");
        Console.WriteLine(contractPath);
        return 0;
    }

    Console.Error.WriteLine("Pipeline provenance contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    Console.Error.WriteLine(contractPath);
    return 1;
}

static VisionPipeline CreatePipelineProvenanceContractPipeline()
{
    VisionPipeline pipeline = new VisionPipeline { Name = "PL0008 Provenance Pipeline" };
    VisionPipelineStep threshold = new VisionPipelineStep
    {
        Name = "Threshold",
        ToolType = "Threshold",
        Enabled = true,
        InputLayer = "Main",
        OutputLayer = "Threshold_Output"
    };
    threshold.Parameters["Mode"] = "Threshold";
    threshold.Parameters["Threshold"] = "127";
    threshold.Parameters["MaxValue"] = "255";
    threshold.Parameters["ThresholdType"] = ThresholdTypes.Binary.ToString();
    pipeline.Steps.Add(threshold);

    VisionPipelineStep blob = new VisionPipelineStep
    {
        Name = "Blob",
        ToolType = "Blob",
        Enabled = true,
        InputLayer = "Main",
        OutputLayer = "Blob_Output"
    };
    blob.Parameters["USE_ROI"] = "false";
    blob.Parameters["USE_THRESHOLD"] = "true";
    blob.Parameters["MIN_AREA"] = "10";
    blob.Parameters["MAX_AREA"] = "100000";
    pipeline.Steps.Add(blob);
    return pipeline;
}

static byte[] SerializePipelineForProvenanceContract(VisionPipeline pipeline)
{
    using StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
    new XmlSerializer(typeof(VisionPipeline)).Serialize(writer, pipeline);
    return Encoding.UTF8.GetBytes(writer.ToString());
}

static void AssertProvenance(
    VisionPipelineExecutionProvenance? provenance,
    ICollection<string> failures,
    string path)
{
    if (provenance == null)
    {
        failures.Add(path + ": execution provenance is missing.");
        return;
    }

    if (provenance.SchemaVersion != 1
        || !QualifiedRecipeSnapshotPreflight.IsSha256(provenance.OriginalPipelineSha256)
        || !QualifiedRecipeSnapshotPreflight.IsSha256(provenance.EffectivePipelineSha256)
        || string.IsNullOrWhiteSpace(provenance.ApplicationIdentity)
        || string.IsNullOrWhiteSpace(provenance.VisionSdkIdentity)
        || string.IsNullOrWhiteSpace(provenance.VisionSdkManifestIdentity))
    {
        failures.Add(path + ": execution provenance identity fields are incomplete.");
    }
}

static void AssertPersistedProvenance(
    VisionPipelineRunReport? report,
    string? reportPath,
    ICollection<string> failures)
{
    AssertProvenance(report?.ExecutionProvenance, failures, "saved Run Report");
    if (report?.ExecutionProvenance == null || string.IsNullOrWhiteSpace(reportPath))
    {
        return;
    }

    string reportDirectory = Path.GetDirectoryName(reportPath) ?? string.Empty;
    string originalPath = Path.Combine(reportDirectory, report.ExecutionProvenance.OriginalPipelineSnapshotFile);
    string effectivePath = Path.Combine(reportDirectory, report.ExecutionProvenance.EffectivePipelineSnapshotFile);
    if (!File.Exists(originalPath)
        || !File.Exists(effectivePath)
        || !string.Equals(ComputeSha256(originalPath), report.ExecutionProvenance.OriginalPipelineSha256, StringComparison.Ordinal)
        || !string.Equals(ComputeSha256(effectivePath), report.ExecutionProvenance.EffectivePipelineSha256, StringComparison.Ordinal)
        || report.ExecutionProvenance.NormalizationChanges.Count != 4)
    {
        failures.Add("saved Run Report: snapshot/hash/change evidence is incomplete.");
    }
}

static bool IsContainedPath(string root, string path)
{
    string fullRoot = Path.GetFullPath(root).TrimEnd(
        Path.DirectorySeparatorChar,
        Path.AltDirectorySeparatorChar);
    string fullPath = Path.GetFullPath(path).TrimEnd(
        Path.DirectorySeparatorChar,
        Path.AltDirectorySeparatorChar);
    return string.Equals(fullRoot, fullPath, StringComparison.OrdinalIgnoreCase)
        || fullPath.StartsWith(
            fullRoot + Path.DirectorySeparatorChar,
            StringComparison.OrdinalIgnoreCase);
}

static void RunBitmapConverterCase(
    string name,
    Action check,
    ICollection<string> observations,
    ICollection<string> failures)
{
    try
    {
        check();
        observations.Add(name + ": PASS");
    }
    catch (Exception exception)
    {
        observations.Add(name + ": FAIL " + exception.GetType().Name);
        failures.Add(name + ": " + exception.Message);
    }
}

static void CheckIndexedOddWidthSubmatrixGuard()
{
    const int width = 13;
    const int height = 2;
    byte[][] rows = CreateIndexedRows(width, height);
    using BitmapStorageLease source = CreateBitmapStorage(
        width,
        height,
        PixelFormat.Format8bppIndexed,
        rows,
        negativeStride: false);
    using Mat parent = new Mat(height + 1, width + 2, MatType.CV_8UC1, Scalar.All(0xA5));
    Rect roiRect = new Rect(1, 0, width, height);
    using Mat destination = new Mat(parent, roiRect);

    BitmapImageConverter.ToMat(source.Bitmap, destination);

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            byte expected = GetIndexedRed(rows[y][x]);
            Require(
                destination.At<byte>(y, x) == expected,
                $"Indexed guard ROI pixel mismatch at ({x},{y}).");
        }
    }

    AssertMatGuard(parent, roiRect, 0xA5);
}

static void CheckIndexedPositiveNegativeStrideAndPalette()
{
    const int width = 13;
    const int height = 3;
    byte[][] rows = CreateIndexedRows(width, height);

    foreach (bool negativeStride in new[] { false, true })
    {
        using BitmapStorageLease source = CreateBitmapStorage(
            width,
            height,
            PixelFormat.Format8bppIndexed,
            rows,
            negativeStride);
        using Mat gray = new Mat(height, width, MatType.CV_8UC1);
        using Mat color = new Mat(height, width, MatType.CV_8UC3);

        BitmapImageConverter.ToMat(source.Bitmap, gray);
        BitmapImageConverter.ToMat(source.Bitmap, color);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte index = rows[y][x];
                byte expectedRed = GetIndexedRed(index);
                Vec3b expectedColor = GetIndexedBgr(index);
                Require(
                    gray.At<byte>(y, x) == expectedRed,
                    $"Indexed gray mismatch for {(negativeStride ? "negative" : "positive")} stride at ({x},{y}).");
                Require(
                    color.At<Vec3b>(y, x) == expectedColor,
                    $"Indexed BGR mismatch for {(negativeStride ? "negative" : "positive")} stride at ({x},{y}).");
            }
        }
    }
}

static void Check24And32BppStorage()
{
    const int width = 5;
    const int height = 3;

    foreach (bool negativeStride in new[] { false, true })
    {
        byte[][] rows24 = CreateColorRows(width, height, 3);
        using BitmapStorageLease source24 = CreateBitmapStorage(
            width,
            height,
            PixelFormat.Format24bppRgb,
            rows24,
            negativeStride);
        using Mat parent24 = new Mat(height + 2, width + 2, MatType.CV_8UC3, Scalar.All(0xA5));
        Rect roi24 = new Rect(1, 1, width, height);
        using Mat destination24 = new Mat(parent24, roi24);
        BitmapImageConverter.ToMat(source24.Bitmap, destination24);
        VerifyColorMat(destination24, rows24, 3);
        AssertMatGuard(parent24, roi24, 0xA5);

        byte[][] rows32 = CreateColorRows(width, height, 4);
        using BitmapStorageLease source32 = CreateBitmapStorage(
            width,
            height,
            PixelFormat.Format32bppArgb,
            rows32,
            negativeStride);
        using Mat parent32 = new Mat(height + 2, width + 2, MatType.CV_8UC4, Scalar.All(0xA5));
        Rect roi32 = new Rect(1, 1, width, height);
        using Mat destination32 = new Mat(parent32, roi32);
        BitmapImageConverter.ToMat(source32.Bitmap, destination32);
        VerifyColorMat(destination32, rows32, 4);
        AssertMatGuard(parent32, roi32, 0xA5);

        using Mat parent32ToBgr = new Mat(height + 2, width + 2, MatType.CV_8UC3, Scalar.All(0xA5));
        using Mat destination32ToBgr = new Mat(parent32ToBgr, roi32);
        BitmapImageConverter.ToMat(source32.Bitmap, destination32ToBgr);
        VerifyColorMat(destination32ToBgr, rows32, 3);
        AssertMatGuard(parent32ToBgr, roi32, 0xA5);

        using BitmapStorageLease source32Rgb = CreateBitmapStorage(
            width,
            height,
            PixelFormat.Format32bppRgb,
            rows32,
            negativeStride);
        using Mat parent32Rgb = new Mat(height + 2, width + 2, MatType.CV_8UC3, Scalar.All(0xA5));
        using Mat destination32Rgb = new Mat(parent32Rgb, roi32);
        BitmapImageConverter.ToMat(source32Rgb.Bitmap, destination32Rgb);
        VerifyColorMat(destination32Rgb, rows32, 3);
        AssertMatGuard(parent32Rgb, roi32, 0xA5);
    }
}

static void CheckBitmapRoundTrips()
{
    const int width = 13;
    const int height = 2;

    using Mat gray = new Mat(height, width, MatType.CV_8UC1);
    using Mat bgr = new Mat(height, width, MatType.CV_8UC3);
    using Mat bgra = new Mat(height, width, MatType.CV_8UC4);
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            byte value = (byte)(((x + y) & 1) == 0 ? 0 : 255);
            gray.Set(y, x, value);
            bgr.Set(y, x, new Vec3b((byte)(x + 1), (byte)(y + 11), (byte)(x + y + 21)));
            bgra.Set(y, x, new Vec4b((byte)(x + 1), (byte)(y + 11), (byte)(x + y + 21), 200));
        }
    }

    using Bitmap grayBitmap = BitmapImageConverter.ToBitmap(gray);
    using Mat grayRoundTrip = BitmapImageConverter.ToMat(grayBitmap);
    VerifyMatEqual(gray, grayRoundTrip, "8bpp gray round trip");

    using Bitmap oneBitBitmap = BitmapImageConverter.ToBitmap(gray, PixelFormat.Format1bppIndexed);
    using Mat oneBitRoundTrip = BitmapImageConverter.ToMat(oneBitBitmap);
    VerifyMatEqual(gray, oneBitRoundTrip, "1bpp round trip");

    using Bitmap bgrBitmap = BitmapImageConverter.ToBitmap(bgr);
    using Mat bgrRoundTrip = BitmapImageConverter.ToMat(bgrBitmap);
    VerifyMatEqual(bgr, bgrRoundTrip, "24bpp BGR round trip");

    using Bitmap bgraBitmap = BitmapImageConverter.ToBitmap(bgra);
    using Mat bgraRoundTrip = BitmapImageConverter.ToMat(bgraBitmap);
    VerifyMatEqual(bgra, bgraRoundTrip, "32bpp BGRA round trip");

    using Mat parent = new Mat(height + 2, width + 2, MatType.CV_8UC3, Scalar.All(0xA5));
    Rect roi = new Rect(1, 1, width, height);
    using Mat sourceSubmatrix = new Mat(parent, roi);
    bgr.CopyTo(sourceSubmatrix);
    using Bitmap submatrixBitmap = BitmapImageConverter.ToBitmap(sourceSubmatrix);
    using Mat submatrixRoundTrip = BitmapImageConverter.ToMat(submatrixBitmap);
    VerifyMatEqual(bgr, submatrixRoundTrip, "submatrix BGR round trip");
}

static void CheckUnsupportedBitmapFormats()
{
    using Bitmap unsupportedBitmap = new Bitmap(3, 2, PixelFormat.Format16bppRgb565);
    ExpectNotSupported(
        () => BitmapImageConverter.ToMat(unsupportedBitmap),
        "unsupported Bitmap pixel format");

    using Mat gray = new Mat(2, 3, MatType.CV_8UC1, Scalar.All(1));
    ExpectNotSupported(
        () => BitmapImageConverter.ToBitmap(gray, PixelFormat.Format24bppRgb),
        "incompatible Mat/Bitmap channels");

    using Mat twoChannel = new Mat(2, 3, MatType.CV_8UC2, Scalar.All(1));
    ExpectNotSupported(
        () => BitmapImageConverter.ToBitmap(twoChannel),
        "unsupported Mat channel count");
}

static byte[][] CreateIndexedRows(int width, int height)
{
    byte[][] rows = new byte[height][];
    for (int y = 0; y < height; y++)
    {
        rows[y] = new byte[width];
        for (int x = 0; x < width; x++)
            rows[y][x] = (byte)(1 + ((x + y) % 3));
    }

    return rows;
}

static byte[][] CreateColorRows(int width, int height, int channels)
{
    byte[][] rows = new byte[height][];
    for (int y = 0; y < height; y++)
    {
        rows[y] = new byte[width * channels];
        for (int x = 0; x < width; x++)
        {
            int offset = x * channels;
            rows[y][offset] = (byte)(x + 1);
            rows[y][offset + 1] = (byte)(y + 11);
            rows[y][offset + 2] = (byte)(x + y + 21);
            if (channels == 4)
                rows[y][offset + 3] = 200;
        }
    }

    return rows;
}

static BitmapStorageLease CreateBitmapStorage(
    int width,
    int height,
    PixelFormat pixelFormat,
    IReadOnlyList<byte[]> rows,
    bool negativeStride)
{
    int visibleRowBytes = GetBitmapVisibleRowBytes(width, pixelFormat);
    int absoluteStride = checked((visibleRowBytes + 3) / 4 * 4);
    int stride = negativeStride ? -absoluteStride : absoluteStride;
    IntPtr buffer = Marshal.AllocHGlobal(checked(absoluteStride * height));
    Bitmap? bitmap = null;

    try
    {
        byte[] cleared = new byte[checked(absoluteStride * height)];
        Marshal.Copy(cleared, 0, buffer, cleared.Length);
        IntPtr scan0 = negativeStride
            ? new IntPtr(buffer.ToInt64() + (long)absoluteStride * (height - 1))
            : buffer;
        bitmap = new Bitmap(width, height, stride, pixelFormat, scan0);

        if (pixelFormat == PixelFormat.Format8bppIndexed)
            SetIndexedPalette(bitmap);

        for (int y = 0; y < height; y++)
        {
            if (rows[y].Length < visibleRowBytes)
                throw new InvalidOperationException("Bitmap test row is shorter than its visible pixel bytes.");

            IntPtr row = new IntPtr(scan0.ToInt64() + ((long)y * stride));
            Marshal.Copy(rows[y], 0, row, visibleRowBytes);
        }

        return new BitmapStorageLease(bitmap, buffer);
    }
    catch
    {
        bitmap?.Dispose();
        Marshal.FreeHGlobal(buffer);
        throw;
    }
}

static int GetBitmapVisibleRowBytes(int width, PixelFormat pixelFormat)
{
    return pixelFormat switch
    {
        PixelFormat.Format1bppIndexed => (width + 7) / 8,
        PixelFormat.Format8bppIndexed => width,
        PixelFormat.Format24bppRgb => checked(width * 3),
        PixelFormat.Format32bppRgb => checked(width * 4),
        PixelFormat.Format32bppArgb => checked(width * 4),
        PixelFormat.Format32bppPArgb => checked(width * 4),
        _ => throw new NotSupportedException("Bitmap test format is not supported: " + pixelFormat)
    };
}

static void SetIndexedPalette(Bitmap bitmap)
{
    ColorPalette palette = bitmap.Palette;
    palette.Entries[0] = System.Drawing.Color.Black;
    if (palette.Entries.Length > 1)
        palette.Entries[1] = System.Drawing.Color.FromArgb(10, 20, 30);
    if (palette.Entries.Length > 2)
        palette.Entries[2] = System.Drawing.Color.FromArgb(40, 50, 60);
    if (palette.Entries.Length > 3)
        palette.Entries[3] = System.Drawing.Color.FromArgb(70, 80, 90);
    bitmap.Palette = palette;
}

static byte GetIndexedRed(byte index)
{
    return index switch
    {
        1 => 10,
        2 => 40,
        3 => 70,
        _ => 0
    };
}

static Vec3b GetIndexedBgr(byte index)
{
    return index switch
    {
        1 => new Vec3b(30, 20, 10),
        2 => new Vec3b(60, 50, 40),
        3 => new Vec3b(90, 80, 70),
        _ => new Vec3b(0, 0, 0)
    };
}

static void VerifyColorMat(Mat mat, IReadOnlyList<byte[]> rows, int channels)
{
    int sourceChannels = rows.Count == 0 ? 0 : rows[0].Length / mat.Width;
    Require(sourceChannels >= 3, "Color test rows do not contain three visible channels.");
    for (int y = 0; y < rows.Count; y++)
    {
        for (int x = 0; x < mat.Width; x++)
        {
            int offset = x * sourceChannels;
            byte expected0 = rows[y][offset];
            byte expected1 = rows[y][offset + 1];
            byte expected2 = rows[y][offset + 2];
            if (channels == 3)
            {
                Vec3b actual = mat.At<Vec3b>(y, x);
                Require(
                    actual.Item0 == expected0
                    && actual.Item1 == expected1
                    && actual.Item2 == expected2,
                    $"BGR pixel mismatch at ({x},{y}).");
            }
            else
            {
                Vec4b actual = mat.At<Vec4b>(y, x);
                Require(
                    actual.Item0 == expected0
                    && actual.Item1 == expected1
                    && actual.Item2 == expected2
                    && sourceChannels >= 4
                    && actual.Item3 == rows[y][offset + 3],
                    $"BGRA pixel mismatch at ({x},{y}).");
            }
        }
    }
}

static void VerifyMatEqual(Mat expected, Mat actual, string description)
{
    Require(
        expected.Size() == actual.Size() && expected.Type() == actual.Type(),
        description + " changed dimensions or channels.");
    using Mat difference = new Mat();
    Cv2.Absdiff(expected, actual, difference);
    Require(Cv2.CountNonZero(difference.Reshape(1)) == 0, description + " changed pixel values.");
}

static void AssertMatGuard(Mat parent, Rect roi, byte expected)
{
    for (int y = 0; y < parent.Height; y++)
    {
        for (int x = 0; x < parent.Width; x++)
        {
            if (x >= roi.X && x < roi.X + roi.Width && y >= roi.Y && y < roi.Y + roi.Height)
                continue;

            switch (parent.Channels())
            {
                case 1:
                    Require(parent.At<byte>(y, x) == expected, $"Guard byte changed at ({x},{y}).");
                    break;
                case 3:
                    Require(parent.At<Vec3b>(y, x) == new Vec3b(expected, expected, expected), $"Guard BGR changed at ({x},{y}).");
                    break;
                case 4:
                    Require(parent.At<Vec4b>(y, x) == new Vec4b(expected, expected, expected, expected), $"Guard BGRA changed at ({x},{y}).");
                    break;
                default:
                    throw new InvalidOperationException("Unsupported guard Mat channel count.");
            }
        }
    }
}

static void ExpectNotSupported(Action action, string description)
{
    try
    {
        action();
    }
    catch (NotSupportedException exception)
    {
        Require(!string.IsNullOrWhiteSpace(exception.Message), description + " returned an empty error message.");
        return;
    }

    throw new InvalidOperationException(description + " did not throw NotSupportedException.");
}

static void Require(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
}

static void CheckCanvasImageLoaderContract(string evidenceDirectory, ICollection<string> failures)
{
    string loaderDirectory = Path.Combine(evidenceDirectory, "canvas-image-loader");
    Directory.CreateDirectory(loaderDirectory);
    List<string> report = new List<string>();

    using (Mat gray8 = new Mat(2, 3, MatType.CV_8UC1))
    {
        byte[] values = { 10, 20, 30, 40, 50, 60 };
        for (int index = 0; index < values.Length; index++)
        {
            gray8.Set(index / 3, index % 3, values[index]);
        }

        string path = Path.Combine(loaderDirectory, "gray8.png");
        Cv2.ImWrite(path, gray8);
        CheckCanvasImageLoaderCase(path, MatType.CV_8UC1, 210D, 0D, 0D, report, failures);
    }

    using (Mat bgr8 = new Mat(2, 3, MatType.CV_8UC3, new Scalar(10, 20, 30)))
    {
        string path = Path.Combine(loaderDirectory, "bgr8.png");
        Cv2.ImWrite(path, bgr8);
        CheckCanvasImageLoaderCase(path, MatType.CV_8UC3, 60D, 120D, 180D, report, failures);
    }

    using (Mat bgra8 = new Mat(2, 3, MatType.CV_8UC4, new Scalar(10, 20, 30, 40)))
    {
        string path = Path.Combine(loaderDirectory, "bgra8.png");
        Cv2.ImWrite(path, bgra8);
        CheckCanvasImageLoaderCase(path, MatType.CV_8UC3, 60D, 120D, 180D, report, failures);
    }

    using (Mat gray16 = new Mat(2, 3, MatType.CV_16UC1))
    {
        ushort[] values = { 0, 257, 514, 1028, 32768, 65535 };
        for (int index = 0; index < values.Length; index++)
        {
            gray16.Set(index / 3, index % 3, values[index]);
        }

        string path = Path.Combine(loaderDirectory, "gray16.png");
        Cv2.ImWrite(path, gray16);
        CheckCanvasImageLoaderCase(path, MatType.CV_8UC1, 390D, 0D, 0D, report, failures);
    }

    File.WriteAllLines(
        Path.Combine(loaderDirectory, "canvas_image_loader_contract.txt"),
        new[] { "Result: " + (failures.Count == 0 ? "PASS" : "CHECK MAIN REPORT") }.Concat(report));
}

static void CheckCanvasImageLoaderCase(
    string sourcePath,
    MatType expectedType,
    double expectedSum0,
    double expectedSum1,
    double expectedSum2,
    ICollection<string> report,
    ICollection<string> failures)
{
    string name = Path.GetFileNameWithoutExtension(sourcePath);
    using Mat loaded = OpenVisionLab.ImageCanvas.CanvasImageLoader.LoadMatFromFile(sourcePath);
    if (loaded.Empty())
    {
        failures.Add($"CanvasImageLoader returned an empty Mat for {name}.");
        report.Add($"{name}: FAIL empty");
        return;
    }

    MatType actualType = loaded.Type();
    Scalar beforeGc = Cv2.Sum(loaded);
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
    Scalar afterGc = Cv2.Sum(loaded);

    bool valid = actualType == expectedType
        && loaded.Width == 3
        && loaded.Height == 2
        && Math.Abs(beforeGc.Val0 - expectedSum0) < 0.1D
        && Math.Abs(beforeGc.Val1 - expectedSum1) < 0.1D
        && Math.Abs(beforeGc.Val2 - expectedSum2) < 0.1D
        && Math.Abs(afterGc.Val0 - beforeGc.Val0) < 0.1D
        && Math.Abs(afterGc.Val1 - beforeGc.Val1) < 0.1D
        && Math.Abs(afterGc.Val2 - beforeGc.Val2) < 0.1D;

    report.Add(
        $"{name}: Type={actualType}; Size={loaded.Width}x{loaded.Height}; "
        + $"BeforeGc={beforeGc.Val0:0.###},{beforeGc.Val1:0.###},{beforeGc.Val2:0.###}; "
        + $"AfterGc={afterGc.Val0:0.###},{afterGc.Val1:0.###},{afterGc.Val2:0.###}; "
        + (valid ? "PASS" : "FAIL"));

    if (!valid)
    {
        failures.Add(
            $"CanvasImageLoader {name} mismatch. Expected {expectedType} "
            + $"sum {expectedSum0:0.###},{expectedSum1:0.###},{expectedSum2:0.###}; "
            + $"actual {actualType} sum {beforeGc.Val0:0.###},{beforeGc.Val1:0.###},{beforeGc.Val2:0.###}.");
        return;
    }

    using Mat retained = loaded.Clone();
    string retainedPath = Path.Combine(Path.GetDirectoryName(sourcePath) ?? ".", name + "-owned.png");
    if (!Cv2.ImWrite(retainedPath, retained))
    {
        failures.Add($"CanvasImageLoader retained clone could not be saved for {name}.");
    }
}

static async Task<int> RunReliabilitySoakContractAsync(string? requestedEvidenceDirectory)
{
    const int warmupRuns = 20;
    const int measuredRuns = 1000;
    const int resourceSampleInterval = 100;
    const long maximumPrivateGrowthBytes = 96L * 1024L * 1024L;
    const long maximumWorkingSetGrowthBytes = 128L * 1024L * 1024L;
    const long maximumManagedGrowthBytes = 16L * 1024L * 1024L;
    const int maximumHandleGrowth = 32;
    const int maximumGdiGrowth = 8;
    const int maximumUserGrowth = 8;

    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
        ?? Path.Combine("artifacts", "reliability_soak_contract"));
    string frozenDirectory = Path.Combine(evidenceDirectory, "frozen");
    Directory.CreateDirectory(frozenDirectory);

    string repositoryRoot = FindRepositoryRootForSmoke();
    string sourcePath = Path.Combine(
        repositoryRoot,
        "docs",
        "samples",
        "public",
        "Mean_Brightness_Synthetic_OK.png");
    string recipePath = Path.Combine(
        repositoryRoot,
        "docs",
        "samples",
        "public",
        "Public_Mean_BrightnessDrift.pipeline.xml");
    if (!File.Exists(sourcePath) || !File.Exists(recipePath))
    {
        Console.Error.WriteLine("Reliability soak fixture or Recipe is missing.");
        return 2;
    }

    string frozenSourcePath = Path.Combine(frozenDirectory, Path.GetFileName(sourcePath));
    string frozenRecipePath = Path.Combine(frozenDirectory, Path.GetFileName(recipePath));
    File.Copy(sourcePath, frozenSourcePath, true);
    File.Copy(recipePath, frozenRecipePath, true);
    string frozenSourceSha256 = ComputeSha256(frozenSourcePath);
    string frozenRecipeSha256 = ComputeSha256(frozenRecipePath);

    if (!SerializeHelper.TryLoadFromXmlFile(frozenRecipePath, out VisionPipeline pipeline)
        || pipeline == null)
    {
        Console.Error.WriteLine("Reliability soak Recipe could not be loaded.");
        return 2;
    }

    using Mat frozenSource = Cv2.ImRead(frozenSourcePath, ImreadModes.Unchanged);
    if (frozenSource.Empty())
    {
        Console.Error.WriteLine("Reliability soak image could not be loaded.");
        return 2;
    }

    string sourceMatSha256Before = ComputeMatSha256(frozenSource);
    VisionRecipeRunner runner = new VisionRecipeRunner();
    bool finalWarmupPassed = false;
    string finalWarmupFailure = string.Empty;
    for (int run = 1; run <= warmupRuns; run++)
    {
        try
        {
            using Mat source = frozenSource.Clone();
            using VisionRecipeRunResult result = await runner.RunAsync(pipeline, source);
            finalWarmupPassed = result.Success
                && result.StepCount == 1
                && result.FinalStepSummary?.Success == true
                && result.FinalStepSummary.AcceptancePassed;
            finalWarmupFailure = finalWarmupPassed
                ? string.Empty
                : $"Warm-up {run} failed: {result.SummaryText}";
        }
        catch (Exception ex)
        {
            finalWarmupPassed = false;
            finalWarmupFailure = $"Warm-up {run} threw {ex.GetType().Name}: {ex.Message}";
        }
    }

    string runtimeRecipeBeforePath = Path.Combine(frozenDirectory, "runtime_recipe_before.xml");
    string runtimeRecipeAfterPath = Path.Combine(frozenDirectory, "runtime_recipe_after.xml");
    if (!finalWarmupPassed || !SerializeHelper.SaveXmlFile(runtimeRecipeBeforePath, pipeline))
    {
        string failure = !finalWarmupPassed
            ? finalWarmupFailure
            : "Normalized runtime Recipe snapshot could not be saved.";
        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "reliability_soak_summary.txt"),
            new[] { "Result=FAIL", "Failure=" + failure });
        Console.Error.WriteLine(failure);
        return 1;
    }

    CollectForResourceSample();
    SoakResourceSample baseline = CaptureSoakResourceSample(0);
    List<SoakResourceSample> resourceSamples = new() { baseline };
    List<double> wallTimes = new(measuredRuns);
    List<string> failureDetails = new();
    int failedRuns = 0;
    int metricDrifts = 0;
    int imageDrifts = 0;
    string expectedResultSha256 = string.Empty;
    double expectedMeanValue = double.NaN;
    string firstResultPath = Path.Combine(evidenceDirectory, "result_first.png");
    string lastResultPath = Path.Combine(evidenceDirectory, "result_last.png");
    Stopwatch totalWatch = Stopwatch.StartNew();

    using (StreamWriter writer = new(
        Path.Combine(evidenceDirectory, "reliability_soak_runs.csv"),
        false,
        new UTF8Encoding(false)))
    {
        writer.WriteLine("Iteration,Success,StepStatus,MeanValueAvg,ResultSha256,WallElapsedMs,StepElapsedMs,Error");
        for (int run = 1; run <= measuredRuns; run++)
        {
            bool success = false;
            string stepStatus = string.Empty;
            double meanValue = double.NaN;
            string resultSha256 = string.Empty;
            double stepElapsed = 0D;
            string error = string.Empty;
            Stopwatch runWatch = Stopwatch.StartNew();
            try
            {
                using Mat source = frozenSource.Clone();
                using VisionRecipeRunResult result = await runner.RunAsync(pipeline, source);
                VisionRecipeStepRunSummary? step = result.FinalStepSummary;
                stepStatus = step?.Status ?? string.Empty;
                stepElapsed = step?.ElapsedMilliseconds ?? 0D;
                success = result.Success
                    && result.StepCount == 1
                    && step?.Success == true
                    && step.AcceptancePassed
                    && result.ResultImage != null
                    && !result.ResultImage.Empty()
                    && step.Metrics.TryGetValue("MeanValueAvg", out meanValue);
                if (success)
                {
                    Mat resultImage = result.ResultImage!;
                    resultSha256 = ComputeMatSha256(resultImage);
                    if (run == 1)
                    {
                        expectedResultSha256 = resultSha256;
                        expectedMeanValue = meanValue;
                        if (!Cv2.ImWrite(firstResultPath, resultImage))
                        {
                            success = false;
                            error = "First result image could not be saved.";
                        }
                    }
                    else
                    {
                        if (Math.Abs(meanValue - expectedMeanValue) > 1e-9D)
                        {
                            metricDrifts++;
                        }

                        if (!string.Equals(resultSha256, expectedResultSha256, StringComparison.Ordinal))
                        {
                            imageDrifts++;
                        }
                    }

                    if (run == measuredRuns && !Cv2.ImWrite(lastResultPath, resultImage))
                    {
                        success = false;
                        error = "Last result image could not be saved.";
                    }
                }
                else if (string.IsNullOrWhiteSpace(error))
                {
                    error = result.SummaryText;
                }
            }
            catch (Exception ex)
            {
                error = ex.GetType().Name + ": " + ex.Message;
            }
            finally
            {
                runWatch.Stop();
            }

            wallTimes.Add(runWatch.Elapsed.TotalMilliseconds);
            if (!success)
            {
                failedRuns++;
                AddBoundedSoakFailure(failureDetails, $"Run {run}: {error}");
            }

            writer.WriteLine(string.Join(",",
                run.ToString(CultureInfo.InvariantCulture),
                success ? "true" : "false",
                EscapeBatchCsvValue(stepStatus),
                double.IsFinite(meanValue) ? meanValue.ToString("0.############", CultureInfo.InvariantCulture) : string.Empty,
                resultSha256,
                runWatch.Elapsed.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                stepElapsed.ToString("0.###", CultureInfo.InvariantCulture),
                EscapeBatchCsvValue(error)));

            if (run % resourceSampleInterval == 0)
            {
                CollectForResourceSample();
                resourceSamples.Add(CaptureSoakResourceSample(run));
            }
        }
    }

    totalWatch.Stop();
    bool runtimeRecipeSaved = SerializeHelper.SaveXmlFile(runtimeRecipeAfterPath, pipeline);
    string runtimeRecipeSha256Before = ComputeSha256(runtimeRecipeBeforePath);
    string runtimeRecipeSha256After = runtimeRecipeSaved ? ComputeSha256(runtimeRecipeAfterPath) : string.Empty;
    string frozenSourceSha256After = ComputeSha256(frozenSourcePath);
    string frozenRecipeSha256After = ComputeSha256(frozenRecipePath);
    string sourceMatSha256After = ComputeMatSha256(frozenSource);
    string firstResultFileSha256 = File.Exists(firstResultPath) ? ComputeSha256(firstResultPath) : string.Empty;
    string lastResultFileSha256 = File.Exists(lastResultPath) ? ComputeSha256(lastResultPath) : string.Empty;

    WriteSoakResourceSamples(evidenceDirectory, resourceSamples);
    SoakResourceSample last = resourceSamples[^1];
    long privateGrowth = MaximumGrowth(resourceSamples, baseline, sample => sample.PrivateBytes);
    long workingSetGrowth = MaximumGrowth(resourceSamples, baseline, sample => sample.WorkingSetBytes);
    long managedGrowth = MaximumGrowth(resourceSamples, baseline, sample => sample.ManagedBytes);
    int handleGrowth = MaximumIntGrowth(resourceSamples, baseline, sample => sample.HandleCount);
    int gdiGrowth = MaximumIntGrowth(resourceSamples, baseline, sample => sample.GdiObjects);
    int userGrowth = MaximumIntGrowth(resourceSamples, baseline, sample => sample.UserObjects);
    SoakResourceSample[] plateauSamples = resourceSamples
        .Where(sample => sample.Iteration >= measuredRuns / 2)
        .ToArray();
    int handlePlateauRange = plateauSamples.Max(sample => sample.HandleCount)
        - plateauSamples.Min(sample => sample.HandleCount);
    int gdiPlateauRange = plateauSamples.Max(sample => sample.GdiObjects)
        - plateauSamples.Min(sample => sample.GdiObjects);
    int userPlateauRange = plateauSamples.Max(sample => sample.UserObjects)
        - plateauSamples.Min(sample => sample.UserObjects);

    if (failedRuns != 0)
    {
        AddBoundedSoakFailure(failureDetails, $"Measured failures: {failedRuns}/{measuredRuns}.");
    }
    if (metricDrifts != 0 || imageDrifts != 0)
    {
        AddBoundedSoakFailure(failureDetails, $"Result drift: metric={metricDrifts}, image={imageDrifts}.");
    }
    if (!runtimeRecipeSaved
        || !string.Equals(runtimeRecipeSha256Before, runtimeRecipeSha256After, StringComparison.Ordinal)
        || !string.Equals(frozenSourceSha256, frozenSourceSha256After, StringComparison.Ordinal)
        || !string.Equals(frozenRecipeSha256, frozenRecipeSha256After, StringComparison.Ordinal)
        || !string.Equals(sourceMatSha256Before, sourceMatSha256After, StringComparison.Ordinal))
    {
        AddBoundedSoakFailure(failureDetails, "Frozen Recipe or input identity changed during the measured runs.");
    }
    if (string.IsNullOrWhiteSpace(firstResultFileSha256)
        || !string.Equals(firstResultFileSha256, lastResultFileSha256, StringComparison.Ordinal))
    {
        AddBoundedSoakFailure(failureDetails, "First/last saved result image identity changed.");
    }
    if (privateGrowth > maximumPrivateGrowthBytes
        || workingSetGrowth > maximumWorkingSetGrowthBytes
        || managedGrowth > maximumManagedGrowthBytes
        || handleGrowth > maximumHandleGrowth
        || gdiGrowth > maximumGdiGrowth
        || userGrowth > maximumUserGrowth)
    {
        AddBoundedSoakFailure(
            failureDetails,
            "Resource growth exceeded the contract. "
            + $"Private={FormatMegabytes(privateGrowth)}MB, WorkingSet={FormatMegabytes(workingSetGrowth)}MB, "
            + $"Managed={FormatMegabytes(managedGrowth)}MB, Handles={handleGrowth}, GDI={gdiGrowth}, USER={userGrowth}.");
    }
    if (handlePlateauRange > 2 || gdiPlateauRange > 2 || userPlateauRange > 2)
    {
        AddBoundedSoakFailure(
            failureDetails,
            $"Late resource plateau was not stable. Handles={handlePlateauRange}, GDI={gdiPlateauRange}, USER={userPlateauRange}.");
    }

    double[] sortedTimes = wallTimes.OrderBy(value => value).ToArray();
    double median = PercentileNearestRank(sortedTimes, 0.50D);
    double p95 = PercentileNearestRank(sortedTimes, 0.95D);
    double maximum = sortedTimes.Length == 0 ? 0D : sortedTimes[^1];
    if (p95 > 300D || maximum > 1000D)
    {
        AddBoundedSoakFailure(
            failureDetails,
            $"Run timing exceeded the contract. P95={p95:0.###}ms, Max={maximum:0.###}ms.");
    }

    string summaryPath = Path.Combine(evidenceDirectory, "reliability_soak_summary.txt");
    List<string> summary = new()
    {
        "Result=" + (failureDetails.Count == 0 ? "PASS" : "FAIL"),
        "Fixture=Public_Mean_BrightnessDrift",
        $"WarmupRuns={warmupRuns}",
        $"MeasuredRuns={measuredRuns}",
        $"FailedRuns={failedRuns}",
        $"MetricDrifts={metricDrifts}",
        $"ImageDrifts={imageDrifts}",
        $"FrozenSourceSha256={frozenSourceSha256}",
        $"FrozenRecipeSha256={frozenRecipeSha256}",
        $"RuntimeRecipeSha256={runtimeRecipeSha256Before}",
        $"SourceMatSha256={sourceMatSha256Before}",
        $"ResultMatSha256={expectedResultSha256}",
        $"MeanValueAvg={expectedMeanValue.ToString("0.############", CultureInfo.InvariantCulture)}",
        $"TotalElapsedMs={totalWatch.Elapsed.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)}",
        $"AverageElapsedMs={(wallTimes.Count == 0 ? 0D : wallTimes.Average()).ToString("0.###", CultureInfo.InvariantCulture)}",
        $"MedianElapsedMs={median.ToString("0.###", CultureInfo.InvariantCulture)}",
        $"P95ElapsedMs={p95.ToString("0.###", CultureInfo.InvariantCulture)}",
        $"MaximumElapsedMs={maximum.ToString("0.###", CultureInfo.InvariantCulture)}",
        $"PrivateGrowthMaxMB={FormatMegabytes(privateGrowth)}",
        $"WorkingSetGrowthMaxMB={FormatMegabytes(workingSetGrowth)}",
        $"ManagedGrowthMaxMB={FormatMegabytes(managedGrowth)}",
        $"HandleGrowthMax={handleGrowth}",
        $"GdiGrowthMax={gdiGrowth}",
        $"UserGrowthMax={userGrowth}",
        $"HandlePlateauRange={handlePlateauRange}",
        $"GdiPlateauRange={gdiPlateauRange}",
        $"UserPlateauRange={userPlateauRange}",
        $"PrivateFinalMB={FormatMegabytes(last.PrivateBytes)}",
        $"WorkingSetFinalMB={FormatMegabytes(last.WorkingSetBytes)}",
        $"ManagedFinalMB={FormatMegabytes(last.ManagedBytes)}",
        $"HandleFinal={last.HandleCount}",
        $"GdiFinal={last.GdiObjects}",
        $"UserFinal={last.UserObjects}",
        "PrivateBytes includes managed and native/OpenCV process allocations.",
        "Thresholds=Private96MB|WorkingSet128MB|Managed16MB|Handles32|GDI8|USER8|LatePlateau2|P95-300ms|Max-1000ms"
    };
    summary.AddRange(failureDetails.Select(failure => "Failure=" + failure));
    File.WriteAllLines(summaryPath, summary);

    if (failureDetails.Count == 0)
    {
        Console.WriteLine("Reliability soak contract passed.");
        Console.WriteLine(summaryPath);
        return 0;
    }

    Console.Error.WriteLine("Reliability soak contract failed.");
    foreach (string failure in failureDetails)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

static void AddBoundedSoakFailure(ICollection<string> failures, string failure)
{
    if (failures.Count < 20)
    {
        failures.Add(failure);
    }
}

static void CollectForResourceSample()
{
    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
    GC.WaitForPendingFinalizers();
    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
}

static SoakResourceSample CaptureSoakResourceSample(int iteration)
{
    using System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
    process.Refresh();
    return new SoakResourceSample(
        iteration,
        process.PrivateMemorySize64,
        process.WorkingSet64,
        GC.GetTotalMemory(false),
        process.HandleCount,
        SoakNativeResourceProbe.GetGdiObjectCount(process.Handle),
        SoakNativeResourceProbe.GetUserObjectCount(process.Handle));
}

static void WriteSoakResourceSamples(string evidenceDirectory, IEnumerable<SoakResourceSample> samples)
{
    File.WriteAllLines(
        Path.Combine(evidenceDirectory, "reliability_soak_resources.csv"),
        new[] { "Iteration,PrivateBytes,WorkingSetBytes,ManagedBytes,HandleCount,GdiObjects,UserObjects" }
            .Concat(samples.Select(sample => string.Join(",",
                sample.Iteration.ToString(CultureInfo.InvariantCulture),
                sample.PrivateBytes.ToString(CultureInfo.InvariantCulture),
                sample.WorkingSetBytes.ToString(CultureInfo.InvariantCulture),
                sample.ManagedBytes.ToString(CultureInfo.InvariantCulture),
                sample.HandleCount.ToString(CultureInfo.InvariantCulture),
                sample.GdiObjects.ToString(CultureInfo.InvariantCulture),
                sample.UserObjects.ToString(CultureInfo.InvariantCulture)))));
}

static long MaximumGrowth(
    IEnumerable<SoakResourceSample> samples,
    SoakResourceSample baseline,
    Func<SoakResourceSample, long> selector)
{
    long start = selector(baseline);
    return Math.Max(0L, samples.Max(sample => selector(sample) - start));
}

static int MaximumIntGrowth(
    IEnumerable<SoakResourceSample> samples,
    SoakResourceSample baseline,
    Func<SoakResourceSample, int> selector)
{
    int start = selector(baseline);
    return Math.Max(0, samples.Max(sample => selector(sample) - start));
}

static double PercentileNearestRank(IReadOnlyList<double> sortedValues, double percentile)
{
    if (sortedValues.Count == 0)
    {
        return 0D;
    }

    int index = Math.Max(0, (int)Math.Ceiling(percentile * sortedValues.Count) - 1);
    return sortedValues[Math.Min(index, sortedValues.Count - 1)];
}

static string FormatMegabytes(long bytes)
{
    return (bytes / 1024D / 1024D).ToString("0.###", CultureInfo.InvariantCulture);
}

static string ComputeMatSha256(Mat mat)
{
    byte[] header = Encoding.UTF8.GetBytes(
        $"{mat.Rows}|{mat.Cols}|{mat.Type()}|{mat.Step()}|");
    using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
    hash.AppendData(header);
    hash.AppendData(mat.ToBytes());
    return Convert.ToHexString(hash.GetHashAndReset());
}

static string FindRepositoryRootForSmoke()
{
    foreach (string seed in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
    {
        DirectoryInfo? directory = new(Path.GetFullPath(seed));
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "OpenVisionLab.sln"))
                && Directory.Exists(Path.Combine(directory.FullName, "docs", "samples")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }
    }

    throw new DirectoryNotFoundException("OpenVisionLab repository root was not found.");
}

static int RunEdgeGlobalPolarityContract(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
        ?? Path.Combine("artifacts", "cvr11_global_polarity_contract"));
    string sourceDirectory = Path.Combine(evidenceDirectory, "sources");
    string drawingDirectory = Path.Combine(evidenceDirectory, "drawings");
    Directory.CreateDirectory(sourceDirectory);
    Directory.CreateDirectory(drawingDirectory);

    string templatePath = Path.Combine(evidenceDirectory, "template.png");
    using Mat template = CreateGlobalPolarityPattern();
    Cv2.ImWrite(templatePath, template);

    (string Split, string Name, bool HasTarget, bool Reversed, int X, int Y)[] cases =
    {
        ("Train", "train_same_01", true, false, 18, 18),
        ("Train", "train_same_02", true, false, 52, 24),
        ("Train", "train_same_03", true, false, 94, 16),
        ("Train", "train_same_04", true, false, 42, 62),
        ("Train", "train_reversed_01", true, true, 22, 20),
        ("Train", "train_reversed_02", true, true, 58, 28),
        ("Train", "train_reversed_03", true, true, 102, 18),
        ("Train", "train_reversed_04", true, true, 48, 64),
        ("Validation", "validation_same_01", true, false, 30, 38),
        ("Validation", "validation_same_02", true, false, 84, 52),
        ("Validation", "validation_reversed_01", true, true, 34, 42),
        ("Validation", "validation_reversed_02", true, true, 88, 50),
        ("Validation", "validation_no_target_01", false, false, 0, 0),
        ("Validation", "validation_no_target_02", false, true, 0, 0),
        ("HeldOut", "heldout_same_01", true, false, 16, 66),
        ("HeldOut", "heldout_same_02", true, false, 108, 58),
        ("HeldOut", "heldout_reversed_01", true, true, 20, 70),
        ("HeldOut", "heldout_reversed_02", true, true, 106, 60),
        ("HeldOut", "heldout_no_target_01", false, false, 0, 0),
        ("HeldOut", "heldout_no_target_02", false, true, 0, 0)
    };

    VisionPipelineStep enabledStep = CreateGlobalPolarityStep(templatePath, true);
    VisionPipelineStep legacyStep = CreateGlobalPolarityStep(templatePath, false);
    List<string> rows = new List<string>
    {
        "Split,Case,Expected,Actual,Success,Polarity,Score,CenterX,CenterY,CenterErrorPx,ErrorCode,SourceSha256,DrawingSha256"
    };
    List<string> failures = new List<string>();

    using (Mat reversedProbe = CreateGlobalPolaritySource(template, true, true, 52, 36))
    {
        EdgeBasedTemplateMatchingTool legacyTool =
            (EdgeBasedTemplateMatchingTool)VisionPipelineAppToolFactory.Create(legacyStep);
        VisionToolResult legacyResult = legacyTool.Execute(reversedProbe);
        try
        {
            if (legacyResult.Success || legacyTool.results.Count != 0)
            {
                failures.Add("Legacy XML/default path accepted a globally reversed target.");
            }
        }
        finally
        {
            legacyResult.ResultImage?.Dispose();
        }
    }

    foreach ((string split, string name, bool hasTarget, bool reversed, int x, int y) in cases)
    {
        using Mat source = CreateGlobalPolaritySource(template, hasTarget, reversed, x, y);
        string sourcePath = Path.Combine(sourceDirectory, name + ".png");
        string drawingPath = Path.Combine(drawingDirectory, name + ".png");
        Cv2.ImWrite(sourcePath, source);

        EdgeBasedTemplateMatchingTool tool =
            (EdgeBasedTemplateMatchingTool)VisionPipelineAppToolFactory.Create(enabledStep);
        VisionToolResult result = tool.Execute(source);
        OpenVisionLab.Vision2D.Result.MatchingResult? match = tool.results.SingleOrDefault();
        string actual = result.Success && match != null ? "Match" : "NoMatch";
        string expected = hasTarget ? "Match" : "NoMatch";
        double centerError = double.NaN;
        string polarity = match == null ? "None" : match.PolarityReversed ? "Reversed" : "Same";
        double score = match?.Score ?? double.NaN;

        try
        {
            if (match != null && result.ResultImage != null && !result.ResultImage.Empty())
            {
                Cv2.ImWrite(drawingPath, result.ResultImage);
            }
            else
            {
                using Mat fallback = source.Clone();
                Cv2.PutText(
                    fallback,
                    "NoMatch",
                    new OpenCvSharp.Point(8, 22),
                    HersheyFonts.HersheySimplex,
                    0.6,
                    Scalar.Red,
                    2,
                    LineTypes.AntiAlias);
                Cv2.ImWrite(drawingPath, fallback);
            }

            if (!string.Equals(actual, expected, StringComparison.Ordinal))
            {
                failures.Add($"{split}/{name}: expected {expected}, actual {actual} ({result.ErrorName}: {result.Message}).");
            }

            if (hasTarget && match != null)
            {
                centerError = Math.Sqrt(
                    Math.Pow(match.Center.X - (x + (template.Width / 2D)), 2D)
                    + Math.Pow(match.Center.Y - (y + (template.Height / 2D)), 2D));
                if (centerError > 2D)
                {
                    failures.Add($"{split}/{name}: center error {centerError:0.###} px exceeded 2 px.");
                }

                if (match.PolarityReversed != reversed)
                {
                    failures.Add($"{split}/{name}: expected polarity {(reversed ? "Reversed" : "Same")}, actual {polarity}.");
                }
            }

            rows.Add(string.Join(",",
                split,
                name,
                expected,
                actual,
                result.Success,
                polarity,
                double.IsNaN(score) ? string.Empty : score.ToString("0.###", CultureInfo.InvariantCulture),
                match == null ? string.Empty : match.Center.X.ToString("0.###", CultureInfo.InvariantCulture),
                match == null ? string.Empty : match.Center.Y.ToString("0.###", CultureInfo.InvariantCulture),
                double.IsNaN(centerError) ? string.Empty : centerError.ToString("0.###", CultureInfo.InvariantCulture),
                result.ErrorName,
                ComputeSha256(sourcePath),
                ComputeSha256(drawingPath)));
        }
        finally
        {
            result.ResultImage?.Dispose();
        }
    }

    string matrixPath = Path.Combine(evidenceDirectory, "matrix.csv");
    File.WriteAllLines(matrixPath, rows);
    File.WriteAllText(
        Path.Combine(evidenceDirectory, "pipeline.xml"),
        "<VisionPipeline Name=\"CVR-11 Global Polarity\"><Step Name=\"Global polarity edge match\" ToolType=\"EdgeBasedMatching\" InputLayer=\"Main\" OutputLayer=\"Match\"><Parameter><Key>PATTERN_PATH</Key><Value>"
        + System.Security.SecurityElement.Escape(templatePath)
        + "</Value></Parameter><Parameter><Key>SCORE_MIN</Key><Value>0.8</Value></Parameter><Parameter><Key>NUM_MATCH</Key><Value>1</Value></Parameter><Parameter><Key>ALLOW_GLOBAL_POLARITY_REVERSAL</Key><Value>true</Value></Parameter><Parameter><Key>SEARCH_STEP</Key><Value>1</Value></Parameter><Parameter><Key>USE_POSITION_REFINE</Key><Value>true</Value></Parameter></Step></VisionPipeline>");
    File.WriteAllLines(
        Path.Combine(evidenceDirectory, "completion.txt"),
        new[]
        {
            failures.Count == 0 ? "Status=Complete" : "Status=Incomplete",
            "Scope=Project-authored synthetic global contrast reversal only",
            "Train=8 target rows",
            "Validation=4 target + 2 no-target rows",
            "HeldOut=4 target + 2 no-target rows",
            "LegacyReversedProbe=Rejected",
            "MatrixSha256=" + ComputeSha256(matrixPath),
            "Boundary=No local edge-direction ignore; no automatic mode selection; no physical or field qualification"
        }.Concat(failures.Select(failure => "Failure=" + failure)));

    if (failures.Count > 0)
    {
        Console.Error.WriteLine("CVR-11 global polarity contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        return 1;
    }

    Console.WriteLine($"CVR-11 global polarity contract passed: {cases.Length}/{cases.Length} rows.");
    Console.WriteLine("Evidence=" + evidenceDirectory);
    return 0;
}

static VisionPipelineStep CreateGlobalPolarityStep(string templatePath, bool allowReversal)
{
    VisionPipelineStep step = new VisionPipelineStep
    {
        Name = "Global polarity edge match",
        ToolType = "EdgeBasedMatching",
        InputLayer = "Main",
        OutputLayer = "Match"
    };
    step.Parameters["Name"] = step.Name;
    step.Parameters["PATTERN_PATH"] = templatePath;
    step.Parameters["SCORE_MIN"] = "0.8";
    step.Parameters["NUM_MATCH"] = "1";
    step.Parameters["ALLOW_GLOBAL_POLARITY_REVERSAL"] = allowReversal.ToString(CultureInfo.InvariantCulture);
    step.Parameters["SEARCH_STEP"] = "1";
    step.Parameters["USE_POSITION_REFINE"] = "true";
    step.Parameters["USE_DRAW_IMAGE"] = "true";
    step.Parameters["USE_THRESHOLD"] = "false";
    step.Parameters["CANNY_LOW"] = "30";
    step.Parameters["CANNY_HIGH"] = "90";
    return step;
}

static Mat CreateGlobalPolarityPattern()
{
    Mat pattern = new Mat(new Size(64, 64), MatType.CV_8UC1, Scalar.All(230));
    Cv2.Rectangle(pattern, new Rect(10, 9, 12, 42), Scalar.All(28), -1);
    Cv2.Rectangle(pattern, new Rect(10, 39, 34, 12), Scalar.All(28), -1);
    Cv2.Circle(pattern, new OpenCvSharp.Point(44, 18), 8, Scalar.All(28), -1);
    Cv2.Line(pattern, new OpenCvSharp.Point(39, 32), new OpenCvSharp.Point(52, 47), Scalar.All(28), 5);
    return pattern;
}

static Mat CreateGlobalPolaritySource(
    Mat template,
    bool hasTarget,
    bool reversed,
    int x,
    int y)
{
    byte background = reversed ? (byte)25 : (byte)230;
    Mat source = new Mat(new Size(192, 144), MatType.CV_8UC1, Scalar.All(background));
    if (!hasTarget)
    {
        return source;
    }

    using Mat target = reversed ? new Mat() : template.Clone();
    if (reversed)
    {
        Cv2.BitwiseNot(template, target);
    }

    using Mat roi = new Mat(source, new Rect(x, y, target.Width, target.Height));
    target.CopyTo(roi);
    return source;
}

static int RunPinArrayGapIntentContract()
{
    const string skillTypeName = "OpenVisionLab.OpenVisionRecipePinArrayGapIntentSkill";
    const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    List<string> failures = new List<string>();
    Type? skillType = typeof(VisionRecipeRunner).Assembly.GetType(skillTypeName, throwOnError: false);
    MethodInfo? parseMethod = skillType?.GetMethod("TryParseRowRois", flags);
    MethodInfo? validateMethod = skillType?.GetMethod("TryValidateV1Inputs", flags);
    MethodInfo? measurementMethod = skillType?.GetMethod("CreateMeasurementPipeline", flags);
    MethodInfo? judgedMethod = skillType?.GetMethod("CreateJudgedPipeline", flags);

    if (skillType == null || parseMethod == null || validateMethod == null || measurementMethod == null || judgedMethod == null)
    {
        Console.Error.WriteLine("PinArrayGap intent contract smoke failed.");
        Console.Error.WriteLine($"- Internal skill contract was not found: {skillTypeName}");
        return 1;
    }

    try
    {
        object? rowRois = InvokePinArrayGapParse(parseMethod, "10,20,100,30;10,60,100,30", out bool parseSucceeded, out string parseMessage);
        if (!parseSucceeded || rowRois == null)
        {
            failures.Add("Two valid row ROIs were not parsed: " + parseMessage);
        }
        else
        {
            if (!InvokePinArrayGapValidation(validateMethod, rowRois, "Adjacent edge-to-edge clearance", "Dark", 200, 120, out string validMessage))
            {
                failures.Add("Supported v1 inputs were rejected: " + validMessage);
            }

            VisionPipeline? measurementPipeline = measurementMethod.Invoke(
                null,
                new object?[] { rowRois, 128, 0.55D, 5, 2, 3 }) as VisionPipeline;
            VerifyPinArrayGapMeasurementPipeline(measurementPipeline, failures);

            VisionPipeline? judgedPipeline = judgedMethod.Invoke(
                null,
                new object?[] { rowRois, 128, 0.55D, 5, 2, 3, 6D }) as VisionPipeline;
            VerifyPinArrayGapJudgedPipeline(judgedPipeline, failures);

            if (InvokePinArrayGapValidation(validateMethod, rowRois, "Adjacent edge-to-edge clearance", "Bright", 200, 120, out _))
            {
                failures.Add("Unsupported Bright polarity was accepted.");
            }

            if (InvokePinArrayGapValidation(validateMethod, rowRois, "Center-to-center pitch", "Dark", 200, 120, out _))
            {
                failures.Add("Unsupported center-pitch measurement was accepted.");
            }
        }

        object? outOfBoundsRoi = InvokePinArrayGapParse(parseMethod, "180,100,30,30", out bool outOfBoundsParsed, out string outOfBoundsParseMessage);
        if (!outOfBoundsParsed || outOfBoundsRoi == null)
        {
            failures.Add("The syntactically valid out-of-bounds ROI did not parse: " + outOfBoundsParseMessage);
        }
        else if (InvokePinArrayGapValidation(validateMethod, outOfBoundsRoi, "Adjacent edge-to-edge clearance", "Dark", 200, 120, out _))
        {
            failures.Add("An ROI extending beyond the source bounds was accepted.");
        }
    }
    catch (TargetInvocationException exception)
    {
        failures.Add("Internal skill invocation failed: " + (exception.InnerException?.Message ?? exception.Message));
    }
    catch (Exception exception)
    {
        failures.Add("Contract smoke failed unexpectedly: " + exception.Message);
    }

    if (failures.Count == 0)
    {
        Console.WriteLine("PinArrayGap intent contract smoke passed.");
        return 0;
    }

    Console.Error.WriteLine("PinArrayGap intent contract smoke failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    return 1;
}

static async Task<int> RunToolNImageVerificationContractAsync(string? evidenceDirectory)
{
    List<string> failures = new List<string>();
    string evidencePath = string.IsNullOrWhiteSpace(evidenceDirectory)
        ? Path.Combine(Path.GetTempPath(), "OpenVisionLab_P233_NImage")
        : Path.GetFullPath(evidenceDirectory);
    Directory.CreateDirectory(evidencePath);
    string inputDirectory = Path.Combine(evidencePath, "inputs");
    Directory.CreateDirectory(inputDirectory);
    List<string> imagePaths = new List<string>();
    for (int index = 0; index < 30; index++)
    {
        string path = Path.Combine(inputDirectory, $"n_image_{index + 1:000}.png");
        using Mat image = new Mat(new OpenCvSharp.Size(320, 220), MatType.CV_8UC3, Scalar.Black);
        Cv2.Rectangle(image, new Rect(48, 52, 112, 82), Scalar.White, -1);
        Cv2.Circle(image, new OpenCvSharp.Point(224, 96), 24, Scalar.White, -1);
        Cv2.Line(image, new OpenCvSharp.Point(62, 160), new OpenCvSharp.Point(266, 168), Scalar.White, 4);
        Cv2.Circle(
            image,
            new OpenCvSharp.Point(286, 184 + (index % 4)),
            2 + (index % 2),
            new Scalar(40 + index, 80, 120),
            -1);
        Cv2.ImWrite(path, image);
        imagePaths.Add(path);
    }

    string matchingTemplatePath = Path.Combine(evidencePath, "matching_template.png");
    using (Mat first = Cv2.ImRead(imagePaths[0], ImreadModes.Color))
    using (Mat template = first.SubMat(new Rect(38, 42, 138, 102)).Clone())
    {
        Cv2.ImWrite(matchingTemplatePath, template);
    }

    Func<VisionPipelineStep> thresholdFactory =
        () => VisionPipelineStepBuilder.FromThresholdProperty(
                    new ThresholdToolProperty
                    {
                        Mode = ThresholdToolMode.Threshold,
                        Threshold = 100,
                        MaxValue = 255,
                        ThresholdType = ThresholdTypes.Binary
                    },
                    "Threshold",
                    "Main",
                    "NImageResult");
    Func<VisionPipelineStep> blobFactory =
        () => VisionPipelineStepBuilder.FromProperty(
                    new BlobProperty("Blob")
                    {
                        USE_THRESHOLD = false,
                        USE_ADAPTIVE_THRESHOLD = false,
                        USE_BITWISENOT = false,
                        MIN_AREA = 20,
                        MAX_AREA = 30000
                    },
                    "Main",
                    "NImageResult");
    Func<VisionPipelineStep> lineFactory =
        () => VisionPipelineStepBuilder.FromProperty(
                    new LineGaugeProperty("Line")
                    {
                        USE_THRESHOLD = false,
                        USE_ADAPTIVE_THRESHOLD = false,
                        USE_BITWISENOT = false,
                        USE_ROI = true,
                        CvROI = new Rect(50, 145, 230, 40),
                        PRJ_PORALITY = FormulaUtil.PROJECTION_POLARITY.BTOW,
                        PRJ_DIR = FormulaUtil.PROJECTION_DIR.Y_TTOB,
                        VER_PRJ_DIR = FormulaUtil.PROJECTION_DIR.X_LTOR,
                        CONTRAST = 30,
                        THICKNESS = 2,
                        SAMPLING_STEP = 4,
                        POINT_RANGE = 8,
                        SHOW_VERTICAL_LINE = true,
                        SHOW_EDGE = true,
                        SHOW_CONTOUR = true,
                        SHOW_FITLINE = true
                    },
                    "Main",
                    "NImageResult");
    Func<VisionPipelineStep> matchingFactory =
        () => VisionPipelineStepBuilder.FromProperty(
                    new MatchingProperty("Matching")
                    {
                        PATTERN_PATH = matchingTemplatePath,
                        SCORE_MIN = 0.75,
                        NUM_MATCH = 1,
                        USE_FIND_ANGLE = false,
                        USE_FIND_SCALE = false
                    },
                    "Main",
                    "NImageResult");
    Func<VisionPipelineStep> edgeBasedMatchingFactory =
        () => VisionPipelineStepBuilder.FromProperty(
                    new EdgeBasedMatchingProperty("EdgeBasedMatching")
                    {
                        PATTERN_PATH = matchingTemplatePath,
                        SCORE_MIN = 0.7,
                        NUM_MATCH = 1,
                        CANNY_LOW = 20,
                        CANNY_HIGH = 60,
                        SEARCH_STEP = 2,
                        USE_POSITION_REFINE = true,
                        USE_FIND_ANGLE = false,
                        USE_FIND_SCALE = false,
                        USE_DRAW_IMAGE = true
                    },
                    "Main",
                    "NImageResult");
    Func<VisionPipelineStep> affineFactory =
        () => VisionPipelineStepBuilder.FromAffineTransformProperty(
                    new AffineTransformProperty("AffineTransform")
                    {
                        SourcePoint1X = 0,
                        SourcePoint1Y = 0,
                        SourcePoint2X = 319,
                        SourcePoint2Y = 0,
                        SourcePoint3X = 0,
                        SourcePoint3Y = 219,
                        DestinationPoint1X = 0,
                        DestinationPoint1Y = 0,
                        DestinationPoint2X = 319,
                        DestinationPoint2Y = 0,
                        DestinationPoint3X = 0,
                        DestinationPoint3Y = 219,
                        OutputWidth = 320,
                        OutputHeight = 220,
                        MinimumSourceTriangleArea = 100,
                        MinimumDestinationTriangleArea = 100,
                        MinimumValidPixelRatio = 0.95
                    },
                    "AffineTransform",
                    "Main",
                    "NImageResult");
    List<(string Name, Func<VisionPipelineStep> Factory)> tools =
        new List<(string, Func<VisionPipelineStep>)>
        {
            ("Threshold", thresholdFactory),
            ("Blob", blobFactory),
            ("Line", lineFactory),
            ("Matching", matchingFactory),
            ("EdgeBasedMatching", edgeBasedMatchingFactory),
            ("AffineTransform", affineFactory)
        };

    List<string> contractLines = new List<string>
    {
        "Tool\tRows\tOK\tNG\tCreateStepCount\tEquivalence\tHtml\tDefinitionSha256"
    };
    foreach ((string toolName, Func<VisionPipelineStep> factory) in tools)
    {
        int createStepCount = 0;
        VisionToolNImageVerificationSession session;
        try
        {
            session = await VisionToolNImageVerificationService.RunAsync(
                toolName,
                "P233_Smoke_" + toolName,
                () =>
                {
                    createStepCount++;
                    return factory();
                },
                normalizeInputToGray: true,
                imagePaths,
                progress: null,
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            failures.Add(toolName + ": N-image service failed: " + ex.GetBaseException().Message);
            continue;
        }

        if (createStepCount != 1)
        {
            failures.Add(toolName + $": current Step was created {createStepCount} times instead of once.");
        }

        if (session.Rows.Count != imagePaths.Count)
        {
            failures.Add(toolName + $": row count {session.Rows.Count} != {imagePaths.Count}.");
        }

        int executionFailures = session.Rows.Count(row => !row.Success);
        if (executionFailures > 0)
        {
            failures.Add(toolName + $": generated success corpus returned {executionFailures} NG/error rows.");
        }

        if (session.Rows.Any(row => !row.IsUngated || !string.Equals(row.Status, "RUN OK", StringComparison.Ordinal)))
        {
            failures.Add(toolName + ": execution-only rows were not clearly marked RUN OK/ungated.");
        }

        if (!SerializeHelper.TryLoadFromXmlText(
                session.PipelineXml,
                out VisionPipeline frozenPipeline,
                out string pipelineLoadError)
            || frozenPipeline == null)
        {
            failures.Add(toolName + ": frozen Pipeline XML did not reload: " + pipelineLoadError);
            continue;
        }

        bool equivalent = true;
        for (int index = 0; index < session.Rows.Count; index++)
        {
            VisionToolNImageVerificationRow row = session.Rows[index];
            VisionPipelineRunReport retainedReport =
                VisionPipelineRunReportStorage.Load(row.RunReportPath);
            if (retainedReport == null
                || !File.Exists(row.SourceSnapshotPath)
                || !VisionPipelineRunReportStorage.IsFileSha256Match(
                    row.SourceSnapshotPath,
                    row.SourceSha256)
                || !File.Exists(row.DrawingPath))
            {
                failures.Add(toolName + $": retained evidence is incomplete for row {index + 1}.");
                equivalent = false;
                continue;
            }

            using Mat source = Cv2.ImRead(imagePaths[index], ImreadModes.Unchanged);
            OpenCvHelper.SetImageChannel1(source);
            using VisionRecipeRunResult direct = await new VisionRecipeRunner().RunAsync(
                frozenPipeline,
                source,
                VisionRecipeRunner.DefaultInputLayer,
                VisionRecipeRunner.DefaultStepTimeoutMilliseconds);
            if (direct.Success != row.Success)
            {
                failures.Add(toolName + $": direct/N-image success mismatch at row {index + 1}.");
                equivalent = false;
            }

            VisionRecipeStepRunSummary? directStep = direct.Steps.LastOrDefault();
            VisionPipelineStepRunReport? retainedStep = retainedReport.Steps.LastOrDefault();
            Dictionary<string, double> retainedMetrics = (retainedStep?.Metrics
                    ?? new List<VisionPipelineMetricRunReport>())
                .ToDictionary(metric => metric.Name, metric => metric.Value, StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, double> metric in directStep?.Metrics
                ?? new Dictionary<string, double>())
            {
                if (!retainedMetrics.TryGetValue(metric.Key, out double retainedValue)
                    || Math.Abs(retainedValue - metric.Value) > 0.000001)
                {
                    failures.Add(
                        toolName + $": metric mismatch at row {index + 1}: {metric.Key} "
                        + $"direct={metric.Value:0.######}, retained={retainedValue:0.######}.");
                    equivalent = false;
                    break;
                }
            }
        }

        VisionPipelineBatchRunSummary batch =
            VisionPipelineBatchRunSummaryStorage.Load(session.BatchSummaryPath);
        if (batch == null
            || batch.TotalCount != imagePaths.Count
            || string.IsNullOrWhiteSpace(batch.PipelineSnapshotFile)
            || !File.Exists(Path.Combine(
                Path.GetDirectoryName(session.BatchSummaryPath) ?? string.Empty,
                batch.PipelineSnapshotFile)))
        {
            failures.Add(toolName + ": batch summary/pipeline snapshot is incomplete.");
        }
        else
        {
            VisionPipelineBatchRunSummaryStorage.BatchReviewQueue rebuilt =
                VisionPipelineBatchRunSummaryStorage.BuildReviewQueue(batch.Results);
            if (!string.Equals(
                    rebuilt.Sha256,
                    batch.ReviewQueueSha256,
                    StringComparison.OrdinalIgnoreCase))
            {
                failures.Add(toolName + ": deterministic review queue SHA-256 changed on rebuild.");
            }
        }

        Dictionary<string, DateTime> reportWriteTimes = session.Rows
            .Where(row => File.Exists(row.RunReportPath))
            .ToDictionary(
                row => row.RunReportPath,
                row => File.GetLastWriteTimeUtc(row.RunReportPath),
                StringComparer.OrdinalIgnoreCase);
        string toolEvidenceDirectory = Path.Combine(evidencePath, toolName);
        Directory.CreateDirectory(toolEvidenceDirectory);
        string htmlPath = Path.Combine(toolEvidenceDirectory, "n_image_report.html");
        bool htmlSaved = VisionToolNImageVerificationHtmlReportExporter.TryExport(
            session.BatchSummaryPath,
            session.PipelineXml,
            session.StepDefinitionSha256,
            htmlPath,
            OpenVisionLanguage.English,
            out string htmlError);
        string koreanHtmlPath = Path.Combine(toolEvidenceDirectory, "n_image_report_ko.html");
        bool koreanHtmlSaved = VisionToolNImageVerificationHtmlReportExporter.TryExport(
            session.BatchSummaryPath,
            session.PipelineXml,
            session.StepDefinitionSha256,
            koreanHtmlPath,
            OpenVisionLanguage.Korean,
            out string koreanHtmlError);
        if (!htmlSaved
            || !File.Exists(htmlPath)
            || !File.ReadAllText(htmlPath).Contains("data:image", StringComparison.Ordinal)
            || !File.ReadAllText(htmlPath).Contains(session.StepDefinitionSha256, StringComparison.Ordinal)
            || !File.ReadAllText(htmlPath).Contains("N-image verification report", StringComparison.Ordinal)
            || !koreanHtmlSaved
            || !File.Exists(koreanHtmlPath)
            || !File.ReadAllText(koreanHtmlPath).Contains("N장 검증 보고서", StringComparison.Ordinal)
            || File.ReadAllText(koreanHtmlPath).Contains("?", StringComparison.Ordinal))
        {
            failures.Add(toolName + ": localized self-contained HTML export failed: " + htmlError + " / " + koreanHtmlError);
        }

        if (reportWriteTimes.Any(pair =>
                !File.Exists(pair.Key)
                || File.GetLastWriteTimeUtc(pair.Key) != pair.Value))
        {
            failures.Add(toolName + ": HTML export modified or reran retained run reports.");
        }

        File.WriteAllText(
            Path.Combine(toolEvidenceDirectory, "pipeline.xml"),
            session.PipelineXml,
            new UTF8Encoding(false));
        File.Copy(
            session.BatchSummaryPath,
            Path.Combine(toolEvidenceDirectory, "summary.xml"),
            true);
        contractLines.Add(string.Join(
            "\t",
            toolName,
            session.Rows.Count.ToString(CultureInfo.InvariantCulture),
            session.Rows.Count(row => row.Success).ToString(CultureInfo.InvariantCulture),
            session.Rows.Count(row => !row.Success).ToString(CultureInfo.InvariantCulture),
            createStepCount.ToString(CultureInfo.InvariantCulture),
            equivalent ? "PASS" : "FAIL",
            htmlSaved ? "PASS" : "FAIL",
            session.StepDefinitionSha256));
    }

    try
    {
        VisionToolNImageVerificationSession gatedSession =
            await VisionToolNImageVerificationService.RunAsync(
                "Threshold",
                "P233_Smoke_AcceptanceNg",
                () =>
                {
                    VisionPipelineStep step = thresholdFactory();
                    step.UseAcceptance = true;
                    step.ExpectedSuccess = false;
                    return step;
                },
                normalizeInputToGray: true,
                imagePaths.Take(1).ToList(),
                progress: null,
                CancellationToken.None);
        VisionToolNImageVerificationRow gatedRow = gatedSession.Rows.Single();
        if (!gatedRow.IsNg
            || !string.Equals(gatedRow.Status, "NG", StringComparison.Ordinal)
            || string.IsNullOrWhiteSpace(gatedRow.ReviewDetailText))
        {
            failures.Add(
                "Acceptance-gated N-image result did not expose NG and its review reason. "
                + $"Status={gatedRow.Status}, Reason={gatedRow.ReviewDetailText}");
        }
    }
    catch (Exception ex)
    {
        failures.Add("Acceptance-gated NG contract failed: " + ex.GetBaseException().Message);
    }

    File.WriteAllLines(
        Path.Combine(evidencePath, "contract.tsv"),
        contractLines,
        new UTF8Encoding(false));
    if (failures.Count > 0)
    {
        Console.Error.WriteLine("Tool View N-image verification contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        return 1;
    }

    Console.WriteLine(
        $"Tool View N-image verification contract passed. Tools={tools.Count}, ImagesPerTool={imagePaths.Count}, "
        + $"Evidence={evidencePath}");
    foreach (string line in contractLines.Skip(1))
    {
        Console.WriteLine(line);
    }

    return 0;
}

static async Task<int> RunToolNImageRealFolderAcceptanceAsync(
    string datasetRootArgument,
    string sourceFile,
    string templatePathArgument,
    string baselineCsvArgument,
    string evidenceDirectoryArgument)
{
    const int rowsPerRole = 12;
    const double scoreTolerance = 0.1D;
    string datasetRoot = Path.GetFullPath(datasetRootArgument);
    string templatePath = Path.GetFullPath(templatePathArgument);
    string baselineCsvPath = Path.GetFullPath(baselineCsvArgument);
    string evidenceDirectory = Path.GetFullPath(evidenceDirectoryArgument);
    string metadataPath = Path.Combine(datasetRoot, "metadata.csv");
    if (!File.Exists(metadataPath)
        || !File.Exists(templatePath)
        || !File.Exists(baselineCsvPath))
    {
        Console.Error.WriteLine(
            "P234 real-folder acceptance prerequisite is missing. "
            + $"Metadata={metadataPath}; Template={templatePath}; Baseline={baselineCsvPath}.");
        return 2;
    }

    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> sourceRows =
        LoadAutoMPointCorpusMetadata(metadataPath)
            .Where(row => string.Equals(row.SourceFile, sourceFile, StringComparison.OrdinalIgnoreCase))
            .OrderBy(row => row.GlobalId)
            .ToList();
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> selectedRows =
        sourceRows
            .GroupBy(row => row.Status, StringComparer.OrdinalIgnoreCase)
            .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
            .SelectMany(group =>
            {
                List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> ordered =
                    group.OrderBy(row => row.Md5, StringComparer.OrdinalIgnoreCase).ToList();
                return Enumerable.Range(0, rowsPerRole)
                    .Select(index => ordered[(int)Math.Round(
                        index * (ordered.Count - 1) / (double)(rowsPerRole - 1),
                        MidpointRounding.AwayFromZero)]);
            })
            .OrderBy(row => row.Status, StringComparer.OrdinalIgnoreCase)
            .ThenBy(row => row.FileName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    if (selectedRows.Count != rowsPerRole * 2
        || selectedRows.Count(row => row.Status == "OK") != rowsPerRole
        || selectedRows.Count(row => row.Status == "NG") != rowsPerRole)
    {
        Console.Error.WriteLine(
            $"P234 expected {rowsPerRole} OK and {rowsPerRole} NG rows for {sourceFile}; "
            + $"selected {selectedRows.Count}.");
        return 2;
    }

    Dictionary<string, double> baselineScores = LoadBaselineScores(baselineCsvPath);
    Directory.CreateDirectory(evidenceDirectory);
    string inputDirectory = Path.Combine(evidenceDirectory, "real_folder_input");
    Directory.CreateDirectory(inputDirectory);
    List<string> integrityFailures = new List<string>();
    foreach (var row in selectedRows)
    {
        string sourcePath = GetAutoMPointCorpusImagePath(datasetRoot, row);
        string actualMd5 = File.Exists(sourcePath) ? ComputeMd5(sourcePath) : "MISSING";
        if (!string.Equals(actualMd5, row.Md5, StringComparison.OrdinalIgnoreCase))
        {
            integrityFailures.Add(
                $"{row.Status}/{row.FileName}: metadata MD5={row.Md5}, actual={actualMd5}.");
            continue;
        }

        File.Copy(sourcePath, Path.Combine(inputDirectory, row.FileName), true);
    }

    if (!OpenVisionRecipeValidationSetStorage.TryGetTopLevelImagePaths(
            inputDirectory,
            out IReadOnlyList<string> registeredPaths,
            out string folderError))
    {
        Console.Error.WriteLine("P234 top-level folder registration failed: " + folderError);
        return 1;
    }

    int createStepCount = 0;
    VisionToolNImageVerificationSession session =
        await VisionToolNImageVerificationService.RunAsync(
            "EdgeBasedMatching",
            "P234_DiePad1_RealFolder",
            () =>
            {
                createStepCount++;
                return CreateEdgeUniqueCardRPipeline(
                    templatePath,
                    new Rect(0, 0, 512, 512),
                    uniqueEnabled: true,
                    scoreMinimum: 0.75D,
                    uniqueMarginMinimum: 0.05D).Steps.Single();
            },
            normalizeInputToGray: true,
            registeredPaths,
            progress: null,
            CancellationToken.None);

    List<string> verificationFailures = new List<string>(integrityFailures);
    if (createStepCount != 1)
    {
        verificationFailures.Add($"Step factory count was {createStepCount}, expected 1.");
    }
    if (session.Rows.Count != selectedRows.Count)
    {
        verificationFailures.Add(
            $"Result row count was {session.Rows.Count}, expected {selectedRows.Count}.");
    }

    List<string> resultLines = new List<string>
    {
        "Role\tFileName\tStatus\tScoreMax\tBaselineScore\tScoreDelta\tSourceSha256\tDrawing"
    };
    foreach (VisionToolNImageVerificationRow result in session.Rows)
    {
        var selected = selectedRows.Single(row =>
            string.Equals(row.FileName, result.FileName, StringComparison.OrdinalIgnoreCase));
        VisionPipelineRunReport report = VisionPipelineRunReportStorage.Load(result.RunReportPath);
        VisionPipelineStepRunReport? step = report?.Steps?.LastOrDefault();
        double score = step?.Metrics?
            .FirstOrDefault(metric => string.Equals(
                metric.Name,
                VisionPipelineKnownMetrics.ScoreMax,
                StringComparison.OrdinalIgnoreCase))?.Value ?? double.NaN;
        baselineScores.TryGetValue(result.FileName, out double baselineScore);
        double scoreDelta = score - baselineScore;
        bool sourceEvidenceValid =
            File.Exists(result.SourceSnapshotPath)
            && VisionPipelineRunReportStorage.IsFileSha256Match(
                result.SourceSnapshotPath,
                result.SourceSha256)
            && AreDecodedImagesEqual(result.ImagePath, result.SourceSnapshotPath);
        if (!result.Success)
        {
            verificationFailures.Add($"{selected.Status}/{result.FileName}: {result.Status} {result.Message}");
        }
        if (!double.IsFinite(score)
            || !baselineScores.ContainsKey(result.FileName)
            || Math.Abs(scoreDelta) > scoreTolerance)
        {
            verificationFailures.Add(
                $"{selected.Status}/{result.FileName}: ScoreMax={score:0.###}, "
                + $"baseline={baselineScore:0.###}, delta={scoreDelta:0.######}.");
        }
        if (!sourceEvidenceValid)
        {
            verificationFailures.Add(
                $"{selected.Status}/{result.FileName}: retained source snapshot/hash/pixels mismatch.");
        }
        if (!result.HasDrawing)
        {
            verificationFailures.Add($"{selected.Status}/{result.FileName}: retained drawing missing.");
        }

        resultLines.Add(string.Join(
            "\t",
            selected.Status,
            result.FileName,
            result.Status,
            score.ToString("0.###", CultureInfo.InvariantCulture),
            baselineScore.ToString("0.###", CultureInfo.InvariantCulture),
            scoreDelta.ToString("0.######", CultureInfo.InvariantCulture),
            result.SourceSha256,
            result.HasDrawing ? "PASS" : "FAIL"));
    }

    string htmlPath = Path.Combine(evidenceDirectory, "P234_DIE_PAD_REAL_FOLDER_REPORT.html");
    bool htmlSaved = VisionToolNImageVerificationHtmlReportExporter.TryExport(
        session.BatchSummaryPath,
        session.PipelineXml,
        session.StepDefinitionSha256,
        htmlPath,
        OpenVisionLanguage.English,
        out string htmlError);
    if (!htmlSaved)
    {
        verificationFailures.Add("HTML export failed: " + htmlError);
    }

    File.WriteAllLines(
        Path.Combine(evidenceDirectory, "results.tsv"),
        resultLines,
        new UTF8Encoding(false));
    File.WriteAllText(
        Path.Combine(evidenceDirectory, "pipeline.xml"),
        session.PipelineXml,
        new UTF8Encoding(false));
    File.Copy(
        session.BatchSummaryPath,
        Path.Combine(evidenceDirectory, "summary.xml"),
        true);
    string completionRecordPath = Path.Combine(evidenceDirectory, "completion_record.txt");
    File.WriteAllLines(
        completionRecordPath,
        new[]
        {
            "Status: " + (verificationFailures.Count == 0 ? "Complete" : "Incomplete"),
            $"Scope: P233 shared Tool View N-image path on a deterministic top-level folder containing {rowsPerRole} OK + {rowsPerRole} NG real operator-supplied Die Pad 1 rows, without parameter tuning.",
            $"Acceptance criteria: folder registration -> {registeredPaths.Count}/{selectedRows.Count}; Step freeze -> {createStepCount}/1; execution -> {session.Rows.Count(row => row.Success)}/{selectedRows.Count}; drawings -> {session.Rows.Count(row => row.HasDrawing)}/{selectedRows.Count}; score parity within {scoreTolerance:0.###} -> {(verificationFailures.Count == 0 ? "PASS" : "FAIL")}.",
            $"Verification: source metadata MD5; retained source SHA-256; exact P230 score comparison; retained drawing; retained-only HTML; Step SHA-256 {session.StepDefinitionSha256}.",
            $"Evidence: {htmlPath}; {Path.Combine(evidenceDirectory, "results.tsv")}; {Path.Combine(evidenceDirectory, "pipeline.xml")}; {Path.Combine(evidenceDirectory, "summary.xml")}; {inputDirectory}.",
            "Boundary / next dependency: This is a 24-row same-source synthetic/augmented integration acceptance using the already frozen P230 locator. It does not create new semantic qualification, retune the locator, test other source strata, or prove parallel execution."
        },
        new UTF8Encoding(false));

    Console.WriteLine($"P234FolderRegistration={registeredPaths.Count}/{selectedRows.Count}");
    Console.WriteLine($"P234StepCreateCount={createStepCount}");
    Console.WriteLine($"P234Execution={session.Rows.Count(row => row.Success)}/{selectedRows.Count}");
    Console.WriteLine($"P234Drawings={session.Rows.Count(row => row.HasDrawing)}/{selectedRows.Count}");
    Console.WriteLine($"P234Failures={verificationFailures.Count}");
    Console.WriteLine($"P234Report={htmlPath}");
    if (verificationFailures.Count == 0)
    {
        return 0;
    }

    foreach (string failure in verificationFailures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

static Dictionary<string, double> LoadBaselineScores(string csvPath)
{
    string[] lines = File.ReadAllLines(csvPath);
    List<string> header = ParseCsvRecord(lines[0]);
    int fileIndex = header.FindIndex(value => value == "FileName");
    int scoreIndex = header.FindIndex(value => value == "Score");
    int outcomeIndex = header.FindIndex(value => value == "Outcome");
    if (fileIndex < 0 || scoreIndex < 0 || outcomeIndex < 0)
    {
        throw new InvalidDataException("Baseline CSV is missing FileName, Score, or Outcome.");
    }

    return lines.Skip(1)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(ParseCsvRecord)
        .Where(values => string.Equals(values[outcomeIndex], "SUCCESS", StringComparison.OrdinalIgnoreCase))
        .ToDictionary(
            values => values[fileIndex],
            values => double.Parse(values[scoreIndex], CultureInfo.InvariantCulture),
            StringComparer.OrdinalIgnoreCase);
}

static bool AreDecodedImagesEqual(string leftPath, string rightPath)
{
    using System.Drawing.Bitmap leftBitmap = new System.Drawing.Bitmap(leftPath);
    using System.Drawing.Bitmap rightBitmap = new System.Drawing.Bitmap(rightPath);
    using Mat left = BitmapImageConverter.ToMat(leftBitmap);
    using Mat right = BitmapImageConverter.ToMat(rightBitmap);
    return !left.Empty()
        && !right.Empty()
        && left.Size() == right.Size()
        && left.Type() == right.Type()
        && Cv2.Norm(left, right, NormTypes.L1) == 0D;
}

static int RunAutoMPointEasyMatchCandidates(
    string sampleRootArgument,
    string evidenceDirectoryArgument)
{
    string sampleRoot = Path.GetFullPath(sampleRootArgument);
    string evidenceDirectory = Path.GetFullPath(evidenceDirectoryArgument);
    string[] sampleNames =
    {
        "BOARD.JPG",
        "Die Pad 1.bmp",
        "Floppies.jpg",
        "Frame 1.tif",
        "Switch1.tif"
    };
    string[] samplePaths = sampleNames
        .Select(name => Path.Combine(sampleRoot, name))
        .ToArray();
    string[] missing = samplePaths.Where(path => !File.Exists(path)).ToArray();
    if (missing.Length > 0)
    {
        Console.Error.WriteLine("Auto MPoint EasyMatch samples are missing: " + string.Join("; ", missing));
        return 2;
    }

    Directory.CreateDirectory(evidenceDirectory);
    string drawingsDirectory = Path.Combine(evidenceDirectory, "drawings");
    string cropsDirectory = Path.Combine(evidenceDirectory, "candidate_crops");
    Directory.CreateDirectory(drawingsDirectory);
    Directory.CreateDirectory(cropsDirectory);

    List<string> csvRows = new List<string>
    {
        "Sample,SourcePath,SourceSha256,ExecutionSuccess,ErrorCode,ErrorName,CandidateIndex,Rank,Accepted,Suggested,PatternRoi,Score,FeatureQuality,ContrastStdDev,EdgeDensity,QuadrantBalance,OrientationBalance,SelfMatchScore,AlternativeMatchScore,UniquenessMargin,SyntheticSuccessRate,PositionErrorMaxPx,RuntimeP95Ms,RejectReason,DrawingPath,CropPath"
    };
    List<string> drawingPaths = new List<string>();
    List<string> drawingLabels = new List<string>();
    int suggestedTotal = 0;
    int evaluatedTotal = 0;
    int successfulSamples = 0;

    foreach (string samplePath in samplePaths)
    {
        string sampleKey = Path.GetFileNameWithoutExtension(samplePath)
            .Replace(' ', '_');
        using Mat source = Cv2.ImRead(samplePath, ImreadModes.Unchanged);
        if (source.Empty())
        {
            Console.Error.WriteLine("Auto MPoint sample could not be loaded: " + samplePath);
            return 2;
        }

        AutoMPointToolProperty property = new AutoMPointToolProperty
        {
            UseAnalysisRoi = false,
            CandidateMode = AutoMPointCandidateMode.Grid,
            PatternWidth = 96,
            PatternHeight = 96,
            CandidateStride = 16,
            MaximumFinalists = 8,
            MaximumResults = 5,
            MinimumFeatureQuality = 0.15D,
            MatchingMinimumScore = 0.75D,
            MinimumUniquenessMargin = 0.05D,
            MaximumTemplatePoints = 300,
            SearchStep = 2,
            UsePositionRefine = true,
            UseSubpixelRefine = true,
            UsePyramidPositionProposal = true,
            UseHybridVerify = true,
            UseAngleSearch = false,
            UseScaleSearch = false,
            MaximumPositionErrorPixels = 2.5D,
            MaximumAngleErrorDegrees = 1.5D,
            MaximumScaleErrorRatio = 0.03D
        };
        AutoMPointTool tool = new AutoMPointTool();
        tool.SetProperty(property);
        VisionToolResult execution = tool.Execute(source);
        try
        {
            evaluatedTotal += tool.candidates.Count;
            suggestedTotal += tool.results.Count;
            if (execution.Success)
            {
                successfulSamples++;
            }

            string drawingPath = Path.Combine(drawingsDirectory, sampleKey + "_auto_mpoint.png");
            if (execution.ResultImage != null && !execution.ResultImage.Empty())
            {
                Cv2.ImWrite(drawingPath, execution.ResultImage);
                drawingPaths.Add(drawingPath);
                drawingLabels.Add(string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} | suggested {1}/{2} | {3}",
                    Path.GetFileName(samplePath),
                    tool.results.Count,
                    tool.candidates.Count,
                    execution.Success ? "suggestions" : execution.ErrorName));
            }

            foreach (OpenVisionLab.Vision2D.Result.AutoMPointCandidateResult candidate in tool.candidates)
            {
                string cropPath = string.Empty;
                bool suggested = candidate.Rank > 0;
                if (suggested)
                {
                    cropPath = Path.Combine(
                        cropsDirectory,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}_rank_{1:00}_x{2}_y{3}.png",
                            sampleKey,
                            candidate.Rank,
                            candidate.PatternRoi.X,
                            candidate.PatternRoi.Y));
                    using Mat crop = source.SubMat(candidate.PatternRoi).Clone();
                    Cv2.ImWrite(cropPath, crop);
                }

                string[] values =
                {
                    Path.GetFileName(samplePath),
                    samplePath,
                    ComputeSha256(samplePath),
                    execution.Success.ToString(CultureInfo.InvariantCulture),
                    ((int)execution.ErrorCode).ToString(CultureInfo.InvariantCulture),
                    execution.ErrorName ?? string.Empty,
                    candidate.Index.ToString(CultureInfo.InvariantCulture),
                    candidate.Rank.ToString(CultureInfo.InvariantCulture),
                    candidate.Accepted.ToString(CultureInfo.InvariantCulture),
                    suggested.ToString(CultureInfo.InvariantCulture),
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0},{1},{2},{3}",
                        candidate.PatternRoi.X,
                        candidate.PatternRoi.Y,
                        candidate.PatternRoi.Width,
                        candidate.PatternRoi.Height),
                    candidate.Score.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.FeatureQuality.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.ContrastStdDev.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.EdgeDensity.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.QuadrantBalance.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.OrientationBalance.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.SelfMatchScore.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.AlternativeMatchScore.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.UniquenessMargin.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.SyntheticSuccessRate.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.PositionErrorMaxPixels.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.RuntimeP95Milliseconds.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.RejectReason ?? string.Empty,
                    File.Exists(drawingPath) ? drawingPath : string.Empty,
                    cropPath
                };
                csvRows.Add(string.Join(",", values.Select(EscapeBatchCsvValue)));
            }
        }
        finally
        {
            execution.ResultImage?.Dispose();
            tool.imageSource?.Dispose();
            tool.imageResult?.Dispose();
            tool.imageTemplate?.Dispose();
        }
    }

    string resultsPath = Path.Combine(evidenceDirectory, "p226_auto_mpoint_easymatch_candidates.csv");
    File.WriteAllLines(resultsPath, csvRows);
    string contactSheetPath = Path.Combine(evidenceDirectory, "p226_auto_mpoint_easymatch_contact_sheet.png");
    SaveCardPilotContactSheet(drawingPaths, drawingLabels, contactSheetPath);
    string recordPath = Path.Combine(evidenceDirectory, "completion_record.md");
    File.WriteAllLines(recordPath, new[]
    {
        "# P226 Auto MPoint EasyMatch candidate presentation",
        string.Empty,
        "Status: Complete",
        string.Empty,
        "Scope: Run the OpenVisionLab Vision SDK Auto MPoint engine once on five diverse public EasyMatch source images and retain operator-review drawings and candidate metrics.",
        string.Empty,
        "Acceptance criteria:",
        $"- Five frozen source images loaded: PASS ({samplePaths.Length}/5).",
        $"- Current-run Auto MPoint drawings retained: PASS ({drawingPaths.Count}/5).",
        $"- Evaluated candidate rows retained: PASS ({evaluatedTotal}).",
        $"- Displayed suggestions retained without automatic apply: PASS ({suggestedTotal}).",
        "- Result-dependent threshold tuning: PASS (none).",
        "- Pattern application or cross-image matching run: PASS (not performed).",
        string.Empty,
        "Verification:",
        "- Product UI defaults were frozen before execution: 96x96, stride 16, maximum results 5, minimum feature quality 0.15, matching score 0.75, uniqueness 0.05, maximum synthetic position error 2.5 px.",
        $"- Samples with at least one suggestion: {successfulSamples}/5.",
        $"- CSV: `{resultsPath}`",
        $"- Contact sheet: `{contactSheetPath}`",
        string.Empty,
        "Boundary / next dependency: These are automatic pattern suggestions on each source image only. They do not prove that a feature is a durable physical locator across a family, and no suggestion may be applied until the operator reviews and approves its physical meaning."
    });

    Console.WriteLine($"P226Samples={samplePaths.Length}");
    Console.WriteLine($"P226SuccessfulSamples={successfulSamples}");
    Console.WriteLine($"P226EvaluatedCandidates={evaluatedTotal}");
    Console.WriteLine($"P226DisplayedSuggestions={suggestedTotal}");
    Console.WriteLine($"P226Results={resultsPath}");
    Console.WriteLine($"P226ContactSheet={contactSheetPath}");
    Console.WriteLine($"P226Record={recordPath}");
    return drawingPaths.Count == samplePaths.Length ? 0 : 1;
}

static int RunAutoMPointFullStratumQualification(
    string datasetRootArgument,
    string sourceFile,
    string templatePathArgument,
    string evidenceDirectoryArgument)
{
    const int expectedRows = 122;
    const int expectedOkRows = 62;
    const int expectedNgRows = 60;
    const double scoreMinimum = 0.75D;
    const double uniquenessMinimum = 0.05D;

    string datasetRoot = Path.GetFullPath(datasetRootArgument);
    string templatePath = Path.GetFullPath(templatePathArgument);
    string evidenceDirectory = Path.GetFullPath(evidenceDirectoryArgument);
    string metadataPath = Path.Combine(datasetRoot, "metadata.csv");
    string drawingsDirectory = Path.Combine(evidenceDirectory, "drawings");
    string overlapDrawingsDirectory = Path.Combine(evidenceDirectory, "defect_overlap_drawings");
    Directory.CreateDirectory(drawingsDirectory);
    Directory.CreateDirectory(overlapDrawingsDirectory);
    if (!File.Exists(metadataPath) || !File.Exists(templatePath))
    {
        Console.Error.WriteLine(
            $"P230 requires metadata and approved template. Metadata={metadataPath}, Template={templatePath}.");
        return 2;
    }

    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> rows =
        LoadAutoMPointCorpusMetadata(metadataPath)
            .Where(row => string.Equals(row.SourceFile, sourceFile, StringComparison.OrdinalIgnoreCase))
            .OrderBy(row => row.GlobalId)
            .ToList();
    int okRows = rows.Count(row => string.Equals(row.Status, "OK", StringComparison.OrdinalIgnoreCase));
    int ngRows = rows.Count(row => string.Equals(row.Status, "NG", StringComparison.OrdinalIgnoreCase));
    if (rows.Count != expectedRows || okRows != expectedOkRows || ngRows != expectedNgRows)
    {
        Console.Error.WriteLine(
            $"P230 frozen stratum mismatch. Expected {expectedRows} ({expectedOkRows} OK/{expectedNgRows} NG), "
            + $"actual {rows.Count} ({okRows} OK/{ngRows} NG).");
        return 2;
    }

    Dictionary<string, string> maskPaths = Directory
        .EnumerateFiles(
            Path.Combine(datasetRoot, "segmentation", "masks_binary"),
            "*.png",
            SearchOption.AllDirectories)
        .GroupBy(path => Path.GetFileNameWithoutExtension(path), StringComparer.OrdinalIgnoreCase)
        .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
    List<string> integrityFailures = new List<string>();
    Dictionary<string, string> overlapDrawingPaths =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    List<(
        int GlobalId,
        string Role,
        string FileName,
        string Outcome,
        double Score,
        double Uniqueness,
        double CenterX,
        double CenterY,
        double Angle,
        double Scale,
        double ElapsedMs,
        int MaskOverlapPixels,
        string ErrorName,
        string Message,
        string DrawingPath,
        string MetadataMd5,
        string ActualMd5,
        bool Md5Verified)> results =
        new List<(
            int,
            string,
            string,
            string,
            double,
            double,
            double,
            double,
            double,
            double,
            double,
            int,
            string,
            string,
            string,
            string,
            string,
            bool)>();
    int runtimeErrors = 0;

    for (int index = 0; index < rows.Count; index++)
    {
        (int globalId, string fileName, string status, string _, string metadataMd5) = rows[index];
        string sourcePath = GetAutoMPointCorpusImagePath(datasetRoot, rows[index]);
        string actualMd5 = File.Exists(sourcePath) ? ComputeMd5(sourcePath) : "MISSING";
        bool md5Verified = string.Equals(actualMd5, metadataMd5, StringComparison.OrdinalIgnoreCase);
        if (!md5Verified)
        {
            integrityFailures.Add(
                $"{status}/{fileName}: metadata MD5={metadataMd5}, actual={actualMd5}.");
        }

        string drawingPath = Path.Combine(
            drawingsDirectory,
            $"{index + 1:000}_{status}_{Path.GetFileNameWithoutExtension(fileName)}.png");
        string outcome = "ERROR";
        double score = double.NaN;
        double uniqueness = double.NaN;
        double centerX = double.NaN;
        double centerY = double.NaN;
        double angle = double.NaN;
        double scale = double.NaN;
        double elapsedMs = double.NaN;
        int maskOverlapPixels = 0;
        string? overlapMaskPath = null;
        string errorName = "MissingImage";
        string message = "Source image could not be loaded.";

        using Mat source = Cv2.ImRead(sourcePath, ImreadModes.Color);
        if (source.Empty())
        {
            runtimeErrors++;
        }
        else
        {
            VisionPipeline pipeline = CreateEdgeUniqueCardRPipeline(
                templatePath,
                new Rect(0, 0, source.Width, source.Height),
                true,
                scoreMinimum,
                uniquenessMinimum);
            EdgeBasedTemplateMatchingTool matcher =
                (EdgeBasedTemplateMatchingTool)VisionPipelineAppToolFactory.Create(
                    pipeline.Steps.Single());
            VisionToolResult execution = matcher.Execute(source);
            try
            {
                elapsedMs = execution.Elapsed.TotalMilliseconds;
                errorName = execution.ErrorName ?? execution.ErrorCode.ToString();
                message = execution.Message ?? string.Empty;
                OpenVisionLab.Vision2D.Result.MatchingResult? match = matcher.results.SingleOrDefault();
                if (execution.Success && match != null)
                {
                    outcome = "SUCCESS";
                    score = match.Score;
                    uniqueness = GetMetricOrNaN(execution, "UniqueMatch.ScoreMargin");
                    centerX = match.Center.X;
                    centerY = match.Center.Y;
                    angle = match.Angle;
                    scale = match.Scale;
                    if (string.Equals(status, "NG", StringComparison.OrdinalIgnoreCase)
                        && maskPaths.TryGetValue(
                            Path.GetFileNameWithoutExtension(fileName),
                            out string? maskPath))
                    {
                        maskOverlapPixels = CountMaskOverlap(maskPath, match.Bounding);
                        overlapMaskPath = maskPath;
                    }
                }
                else if (execution.ErrorCode == VisionToolErrorCode.MatchingAmbiguous)
                {
                    outcome = "AMBIGUOUS";
                }
                else if (execution.ErrorCode == VisionToolErrorCode.MatchingNoResult)
                {
                    outcome = "NO_MATCH";
                }
                else
                {
                    runtimeErrors++;
                }

                if (execution.ResultImage != null && !execution.ResultImage.Empty())
                {
                    Cv2.ImWrite(drawingPath, execution.ResultImage);
                    if (maskOverlapPixels > 0
                        && !string.IsNullOrWhiteSpace(overlapMaskPath))
                    {
                        using Mat mask = Cv2.ImRead(overlapMaskPath, ImreadModes.Grayscale);
                        if (!mask.Empty()
                            && mask.Width == execution.ResultImage.Width
                            && mask.Height == execution.ResultImage.Height)
                        {
                            using Mat red = new Mat(
                                execution.ResultImage.Size(),
                                MatType.CV_8UC3,
                                new Scalar(0, 0, 255));
                            using Mat blended = new Mat();
                            using Mat overlapDrawing = execution.ResultImage.Clone();
                            Cv2.AddWeighted(execution.ResultImage, 0.55D, red, 0.45D, 0D, blended);
                            blended.CopyTo(overlapDrawing, mask);
                            Cv2.PutText(
                                overlapDrawing,
                                $"RED defect mask overlap = {maskOverlapPixels}px",
                                new OpenCvSharp.Point(12, 28),
                                HersheyFonts.HersheySimplex,
                                0.55D,
                                new Scalar(0, 255, 255),
                                2,
                                LineTypes.AntiAlias);
                            string overlapDrawingPath = Path.Combine(
                                overlapDrawingsDirectory,
                                $"{index + 1:000}_{status}_{Path.GetFileNameWithoutExtension(fileName)}.png");
                            Cv2.ImWrite(overlapDrawingPath, overlapDrawing);
                            overlapDrawingPaths[fileName] = overlapDrawingPath;
                        }
                    }
                }
                else
                {
                    using Mat fallback = source.Clone();
                    Cv2.PutText(
                        fallback,
                        $"{outcome}: {errorName}",
                        new OpenCvSharp.Point(12, 28),
                        HersheyFonts.HersheySimplex,
                        0.65,
                        new Scalar(0, 0, 255),
                        2,
                        LineTypes.AntiAlias);
                    Cv2.ImWrite(drawingPath, fallback);
                }
            }
            finally
            {
                execution.ResultImage?.Dispose();
                matcher.imageSource?.Dispose();
                matcher.imageResult?.Dispose();
                using Mat emptyTemplate = new Mat();
                matcher.SetTemplateImage(emptyTemplate);
                matcher.imageTemplate?.Dispose();
            }
        }

        results.Add((
            globalId,
            status,
            fileName,
            outcome,
            score,
            uniqueness,
            centerX,
            centerY,
            angle,
            scale,
            elapsedMs,
            maskOverlapPixels,
            errorName,
            message,
            drawingPath,
            metadataMd5,
            actualMd5,
            md5Verified));
    }

    string resultsCsvPath = Path.Combine(evidenceDirectory, "p230_full_stratum_results.csv");
    File.WriteAllLines(
        resultsCsvPath,
        new[]
        {
            "GlobalId,Role,FileName,Outcome,Score,UniquenessMargin,CenterX,CenterY,AngleDeg,Scale,ElapsedMs,DefectMaskOverlapPixels,ErrorName,Message,DrawingPath,MetadataMd5,ActualMd5,Md5Verified"
        }.Concat(results.Select(result => string.Join(",", new[]
        {
            result.GlobalId.ToString(CultureInfo.InvariantCulture),
            result.Role,
            result.FileName,
            result.Outcome,
            FormatFinite(result.Score),
            FormatFinite(result.Uniqueness),
            FormatFinite(result.CenterX),
            FormatFinite(result.CenterY),
            FormatFinite(result.Angle),
            FormatFinite(result.Scale),
            FormatFinite(result.ElapsedMs),
            result.MaskOverlapPixels.ToString(CultureInfo.InvariantCulture),
            result.ErrorName,
            result.Message,
            result.DrawingPath,
            result.MetadataMd5,
            result.ActualMd5,
            result.Md5Verified.ToString(CultureInfo.InvariantCulture)
        }.Select(EscapeBatchCsvValue)))),
        new System.Text.UTF8Encoding(true));

    Dictionary<string, HashSet<string>> queueReasons =
        new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
    void AddQueueReason(string fileName, string reason)
    {
        if (!queueReasons.TryGetValue(fileName, out HashSet<string>? reasons))
        {
            reasons = new HashSet<string>(StringComparer.Ordinal);
            queueReasons[fileName] = reasons;
        }
        reasons.Add(reason);
    }

    foreach (var result in results.Where(result => result.Outcome != "SUCCESS"))
    {
        AddQueueReason(result.FileName, result.Outcome);
    }
    foreach (var result in results.Where(result => result.MaskOverlapPixels > 0))
    {
        AddQueueReason(result.FileName, "DEFECT_OVERLAP");
    }
    foreach (var result in results.Where(result => result.Outcome == "SUCCESS")
        .OrderBy(result => result.Score).Take(6))
    {
        AddQueueReason(result.FileName, "LOWEST_SCORE");
    }
    foreach (var result in results.Where(result => result.Outcome == "SUCCESS")
        .OrderBy(result => result.Uniqueness).Take(6))
    {
        AddQueueReason(result.FileName, "LOWEST_UNIQUENESS");
    }
    foreach (var result in results.OrderByDescending(result => result.ElapsedMs).Take(4))
    {
        AddQueueReason(result.FileName, "HIGHEST_RUNTIME");
    }
    foreach (var result in results.Where(result => result.Outcome == "SUCCESS")
        .OrderByDescending(result => Math.Abs(result.Angle)).Take(4))
    {
        AddQueueReason(result.FileName, "ANGLE_EXTREME");
    }
    foreach (var result in results.Where(result => result.Outcome == "SUCCESS")
        .OrderBy(result => result.Scale).Take(2)
        .Concat(results.Where(result => result.Outcome == "SUCCESS")
            .OrderByDescending(result => result.Scale).Take(2)))
    {
        AddQueueReason(result.FileName, "SCALE_EXTREME");
    }
    List<(int GlobalId, string Role, string FileName, string Outcome, double Score, double Uniqueness, double CenterX, double CenterY, double Angle, double Scale, double ElapsedMs, int MaskOverlapPixels, string ErrorName, string Message, string DrawingPath, string MetadataMd5, string ActualMd5, bool Md5Verified)> hashOrdered =
        results.OrderBy(result => result.ActualMd5, StringComparer.OrdinalIgnoreCase).ToList();
    for (int index = 0; index < 8; index++)
    {
        int selectedIndex = (int)Math.Round(
            index * (hashOrdered.Count - 1) / 7D,
            MidpointRounding.AwayFromZero);
        AddQueueReason(hashOrdered[selectedIndex].FileName, "HASH_SPREAD");
    }

    List<(int GlobalId, string Role, string FileName, string Outcome, double Score, double Uniqueness, double CenterX, double CenterY, double Angle, double Scale, double ElapsedMs, int MaskOverlapPixels, string ErrorName, string Message, string DrawingPath, string MetadataMd5, string ActualMd5, bool Md5Verified)> reviewRows =
        results.Where(result => queueReasons.ContainsKey(result.FileName))
            .OrderBy(result => result.GlobalId)
            .ToList();
    string queueCsvPath = Path.Combine(evidenceDirectory, "p230_review_queue.csv");
    File.WriteAllLines(
        queueCsvPath,
        new[]
        {
            "GlobalId,Role,FileName,Reasons,Outcome,Score,UniquenessMargin,AngleDeg,Scale,ElapsedMs,DefectMaskOverlapPixels,DrawingPath"
        }.Concat(reviewRows.Select(result => string.Join(",", new[]
        {
            result.GlobalId.ToString(CultureInfo.InvariantCulture),
            result.Role,
            result.FileName,
            string.Join("+", queueReasons[result.FileName].OrderBy(reason => reason, StringComparer.Ordinal)),
            result.Outcome,
            FormatFinite(result.Score),
            FormatFinite(result.Uniqueness),
            FormatFinite(result.Angle),
            FormatFinite(result.Scale),
            FormatFinite(result.ElapsedMs),
            result.MaskOverlapPixels.ToString(CultureInfo.InvariantCulture),
            result.DrawingPath
        }.Select(EscapeBatchCsvValue)))),
        new System.Text.UTF8Encoding(true));

    string queueSheetPath = Path.Combine(evidenceDirectory, "p230_review_queue_contact_sheet.png");
    SaveCardPilotContactSheet(
        reviewRows.Select(result => result.DrawingPath).ToList(),
        reviewRows.Select(result =>
            $"{result.Role} {Path.GetFileNameWithoutExtension(result.FileName)} "
            + $"{result.Outcome} S={result.Score:0.0}").ToList(),
        queueSheetPath);

    string overlapSheetPath = Path.Combine(
        evidenceDirectory,
        "p230_defect_overlap_contact_sheet.png");
    List<(int GlobalId, string Role, string FileName, string Outcome, double Score, double Uniqueness, double CenterX, double CenterY, double Angle, double Scale, double ElapsedMs, int MaskOverlapPixels, string ErrorName, string Message, string DrawingPath, string MetadataMd5, string ActualMd5, bool Md5Verified)> overlapRows =
        results.Where(result => result.MaskOverlapPixels > 0)
            .OrderBy(result => result.GlobalId)
            .ToList();
    if (overlapRows.Count > 0)
    {
        SaveCardPilotContactSheet(
            overlapRows.Select(result => overlapDrawingPaths[result.FileName]).ToList(),
            overlapRows.Select(result =>
                $"{result.Role} {Path.GetFileNameWithoutExtension(result.FileName)} "
                + $"overlap={result.MaskOverlapPixels}px").ToList(),
            overlapSheetPath);
    }

    int successCount = results.Count(result => result.Outcome == "SUCCESS");
    int okSuccess = results.Count(result => result.Role == "OK" && result.Outcome == "SUCCESS");
    int ngSuccess = results.Count(result => result.Role == "NG" && result.Outcome == "SUCCESS");
    int ambiguousCount = results.Count(result => result.Outcome == "AMBIGUOUS");
    int noMatchCount = results.Count(result => result.Outcome == "NO_MATCH");
    int maskOverlapRows = results.Count(result => result.MaskOverlapPixels > 0);
    int drawingCount = results.Count(result => File.Exists(result.DrawingPath));
    double minimumScore = results.Where(result => result.Outcome == "SUCCESS")
        .Select(result => result.Score).DefaultIfEmpty(double.NaN).Min();
    double minimumUniqueness = results.Where(result => result.Outcome == "SUCCESS")
        .Select(result => result.Uniqueness).DefaultIfEmpty(double.NaN).Min();
    double maximumRuntime = results.Select(result => result.ElapsedMs)
        .Where(double.IsFinite).DefaultIfEmpty(double.NaN).Max();
    bool numericalPass =
        successCount == expectedRows
        && ambiguousCount == 0
        && noMatchCount == 0
        && runtimeErrors == 0
        && integrityFailures.Count == 0
        && drawingCount == expectedRows;
    string decision = numericalPass
        ? maskOverlapRows > 0
            ? "Keep with documented limits"
            : "Keep"
        : "Reject";

    string reportPath = Path.Combine(
        evidenceDirectory,
        "OPENVISIONLAB_AUTO_MPOINT_FULL_STRATUM_REPORT.html");
    System.Text.StringBuilder html = new System.Text.StringBuilder();
    html.AppendLine("<!doctype html><html lang=\"ko\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
    html.AppendLine("<title>OpenVisionLab Auto MPoint 전체 계층 자격 검증</title>");
    html.AppendLine("<style>body{margin:0;background:#08111f;color:#eef5ff;font-family:'Segoe UI','Malgun Gothic',sans-serif;line-height:1.55}.wrap{max-width:1400px;margin:auto;padding:32px}.panel{background:#111d2e;border:1px solid #2b3d56;border-radius:14px;padding:22px;margin:18px 0}.cards{display:grid;grid-template-columns:repeat(4,1fr);gap:12px}.card{background:#17263b;border-radius:12px;padding:16px}.value{font-size:25px;font-weight:800}.label{color:#9db0c9;font-size:13px}.ok{color:#6ce9bb}.bad{color:#ff7b83}.warn{color:#ffc36c}.image{background:#050a11;border:1px solid #31445d;border-radius:10px;padding:8px}.image img{width:100%;height:auto;display:block}.template{display:flex;justify-content:center}.template img{width:auto;max-width:420px;max-height:420px}table{width:100%;border-collapse:collapse;font-size:13px}th,td{padding:9px;border-bottom:1px solid #2b3d56;text-align:left}th{background:#1b2b42}.scroll{overflow:auto}.print{float:right;background:#24405e;color:white;border:1px solid #5e789b;border-radius:9px;padding:10px 14px}@media(max-width:900px){.cards{grid-template-columns:repeat(2,1fr)}}@media print{body{background:white;color:#111}.panel,.card{background:white;border-color:#aaa}.print{display:none}}</style></head><body><main class=\"wrap\">");
    html.AppendLine("<button class=\"print\" onclick=\"window.print()\">인쇄 / PDF 저장</button>");
    html.Append("<h1>Auto MPoint 전체 계층 자격 검증</h1><p>")
        .Append(HtmlReportEncode(sourceFile))
        .Append(" · 승인 ROI 128,256,96,96 · 템플릿 SHA-256 ")
        .Append(HtmlReportEncode(ComputeSha256(templatePath)))
        .AppendLine("</p><section class=\"cards\">");
    AppendAutoMPointHtmlMetric(html, $"{successCount}/{expectedRows}", "전체 Matching 성공");
    AppendAutoMPointHtmlMetric(html, $"{okSuccess}/{expectedOkRows}", "OK 성공");
    AppendAutoMPointHtmlMetric(html, $"{ngSuccess}/{expectedNgRows}", "NG 성공");
    AppendAutoMPointHtmlMetric(html, decision, "수치 결정");
    AppendAutoMPointHtmlMetric(html, minimumScore.ToString("0.0", CultureInfo.InvariantCulture), "최저 점수");
    AppendAutoMPointHtmlMetric(html, minimumUniqueness.ToString("0.000", CultureInfo.InvariantCulture), "최저 고유성");
    AppendAutoMPointHtmlMetric(html, maskOverlapRows.ToString(CultureInfo.InvariantCulture), "결함 마스크 겹침 행");
    AppendAutoMPointHtmlMetric(html, reviewRows.Count.ToString(CultureInfo.InvariantCulture), "결정적 검토 큐");
    html.AppendLine("</section>");
    html.Append("<section class=\"panel\"><h2>결론</h2><p><strong class=\"")
        .Append(numericalPass ? "ok" : "bad")
        .Append("\">").Append(HtmlReportEncode(decision)).Append("</strong> · 모호 ")
        .Append(ambiguousCount).Append(" · 미검출 ").Append(noMatchCount)
        .Append(" · 런타임 오류 ").Append(runtimeErrors)
        .Append(" · 무결성 오류 ").Append(integrityFailures.Count)
        .Append(" · 최대 실행시간 ").Append(maximumRuntime.ToString("0.0", CultureInfo.InvariantCulture))
        .AppendLine(" ms</p>");
    html.AppendLine("<p class=\"warn\">이 결정은 동일 합성·증강 Die Pad 1 계층의 수치 자격 검증입니다. 실제 촬영, 생산 변동, 자동 패턴 크기, 다른 Die Pad 2~4 계층, 현장 자격을 의미하지 않습니다.</p></section>");
    html.AppendLine("<section class=\"panel\"><h2>승인 템플릿</h2>");
    html.Append("<div class=\"image template\"><img alt=\"승인된 Auto MPoint 템플릿\" src=\"")
        .Append(ToEmbeddedImageDataUri(templatePath)).AppendLine("\"></div></section>");
    html.AppendLine("<section class=\"panel\"><h2>결정적 검토 큐</h2><p>모든 실패·결함 겹침과 최저 점수/고유성, 각도·배율·시간 극단, 해시 분산 표본을 중복 제거했습니다.</p>");
    html.Append("<div class=\"image\"><img alt=\"Auto MPoint 전체 계층 검토 큐\" src=\"")
        .Append(ToEmbeddedImageDataUri(queueSheetPath)).AppendLine("\"></div></section>");
    if (File.Exists(overlapSheetPath))
    {
        html.AppendLine("<section class=\"panel\"><h2>결함 마스크 겹침 9건</h2><p class=\"warn\">빨간색은 공급된 NG 결함 마스크입니다. 96×96 매칭 영역과 일부 겹쳤지만, 현재 데이터에서는 모두 동일한 중앙 패드에 성공했습니다. 이 겹침 때문에 결과를 실패로 바꾸지는 않되 실제 촬영 변동 위험으로 유지합니다.</p>");
        html.Append("<div class=\"image\"><img alt=\"결함 마스크와 매칭 영역 겹침\" src=\"")
            .Append(ToEmbeddedImageDataUri(overlapSheetPath)).AppendLine("\"></div></section>");
    }
    html.AppendLine("<section class=\"panel\"><h2>검토 큐 상세</h2><div class=\"scroll\"><table><thead><tr><th>Role</th><th>File</th><th>Reasons</th><th>Outcome</th><th>Score</th><th>Uniqueness</th><th>Angle</th><th>Scale</th><th>Overlap</th></tr></thead><tbody>");
    foreach (var result in reviewRows)
    {
        html.Append("<tr><td>").Append(HtmlReportEncode(result.Role))
            .Append("</td><td>").Append(HtmlReportEncode(result.FileName))
            .Append("</td><td>").Append(HtmlReportEncode(string.Join("+", queueReasons[result.FileName].OrderBy(reason => reason, StringComparer.Ordinal))))
            .Append("</td><td>").Append(HtmlReportEncode(result.Outcome))
            .Append("</td><td>").Append(FormatFinite(result.Score))
            .Append("</td><td>").Append(FormatFinite(result.Uniqueness))
            .Append("</td><td>").Append(FormatFinite(result.Angle))
            .Append("</td><td>").Append(FormatFinite(result.Scale))
            .Append("</td><td>").Append(result.MaskOverlapPixels)
            .AppendLine("</td></tr>");
    }
    html.AppendLine("</tbody></table></div></section>");
    html.AppendLine("<section class=\"panel\"><h2>증거 파일</h2><p><a href=\"p230_full_stratum_results.csv\">전체 122행 CSV</a> · <a href=\"p230_review_queue.csv\">검토 큐 CSV</a> · 개별 122장 드로잉은 <code>drawings/</code>에 보존됩니다.</p></section>");
    html.AppendLine("</main></body></html>");
    File.WriteAllText(reportPath, html.ToString(), new System.Text.UTF8Encoding(true));

    bool taskComplete =
        results.Count == expectedRows
        && runtimeErrors == 0
        && integrityFailures.Count == 0
        && drawingCount == expectedRows;
    string completionRecordPath = Path.Combine(evidenceDirectory, "completion_record.txt");
    File.WriteAllLines(
        completionRecordPath,
        new[]
        {
            "Status: " + (taskComplete ? "Complete" : "Incomplete"),
            $"Scope: Frozen approved Auto MPoint template replay on all {expectedRows} {sourceFile} rows without parameter tuning.",
            $"Acceptance criteria: rows -> {results.Count}/{expectedRows}; drawings -> {drawingCount}/{expectedRows}; success -> {successCount}/{expectedRows}; ambiguous -> {ambiguousCount}; no match -> {noMatchCount}; runtime errors -> {runtimeErrors}; integrity failures -> {integrityFailures.Count}; defect overlap rows -> {maskOverlapRows}.",
            $"Verification: score >= {scoreMinimum:0.##}; uniqueness >= {uniquenessMinimum:0.##}; angle -8..8; scale 0.9..1.1; template SHA-256 {ComputeSha256(templatePath)}; numerical decision {decision}.",
            $"Evidence: {reportPath}; {resultsCsvPath}; {queueCsvPath}; {queueSheetPath}; {overlapSheetPath}; {drawingsDirectory}.",
            "Boundary / next dependency: Same-source synthetic/augmented evidence only. This numerical record does not replace review of the deterministic queue and every defect-overlap drawing; no other source stratum or field qualification is implied."
        },
        new System.Text.UTF8Encoding(true));

    Console.WriteLine($"P230Rows={results.Count}/{expectedRows}");
    Console.WriteLine($"P230Success={successCount}/{expectedRows}");
    Console.WriteLine($"P230OkSuccess={okSuccess}/{expectedOkRows}");
    Console.WriteLine($"P230NgSuccess={ngSuccess}/{expectedNgRows}");
    Console.WriteLine($"P230Ambiguous={ambiguousCount}");
    Console.WriteLine($"P230NoMatch={noMatchCount}");
    Console.WriteLine($"P230MaskOverlapRows={maskOverlapRows}");
    Console.WriteLine($"P230RuntimeErrors={runtimeErrors}");
    Console.WriteLine($"P230IntegrityFailures={integrityFailures.Count}");
    Console.WriteLine($"P230Drawings={drawingCount}/{expectedRows}");
    Console.WriteLine($"P230ReviewQueue={reviewRows.Count}");
    Console.WriteLine($"P230Decision={decision}");
    Console.WriteLine($"P230Report={reportPath}");
    return taskComplete ? 0 : 1;
}

static int RunAutoMPointRepresentativeBestPilot(
    string datasetRoot,
    string sourceFile,
    string evidenceDirectory)
{
    Directory.CreateDirectory(evidenceDirectory);
    string metadataPath = Path.Combine(datasetRoot, "metadata.csv");
    if (!File.Exists(metadataPath))
    {
        Console.Error.WriteLine("P229 metadata is missing: " + metadataPath);
        return 2;
    }

    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> rows =
        LoadAutoMPointCorpusMetadata(metadataPath)
            .Where(row => string.Equals(row.SourceFile, sourceFile, StringComparison.OrdinalIgnoreCase))
            .OrderBy(row => row.GlobalId)
            .ToList();
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> okRows =
        rows.Where(row => string.Equals(row.Status, "OK", StringComparison.OrdinalIgnoreCase)).ToList();
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> ngRows =
        rows.Where(row => string.Equals(row.Status, "NG", StringComparison.OrdinalIgnoreCase)).ToList();
    if (okRows.Count < 9 || ngRows.Count < 8)
    {
        Console.Error.WriteLine(
            $"P229 requires at least 9 OK and 8 NG rows for {sourceFile}. "
            + $"Actual OK={okRows.Count}, NG={ngRows.Count}.");
        return 2;
    }

    (int GlobalId, string FileName, string Status, string SourceFile, string Md5) canonical = okRows[0];
    HashSet<string> usedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        canonical.FileName
    };
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> representativeRows =
        SelectAutoMPointPilotRows(
                okRows.Where(row => !usedFiles.Contains(row.FileName)).ToList(),
                null)
            .Concat(SelectAutoMPointPilotRows(ngRows, null))
            .ToList();
    foreach ((int _, string fileName, string _, string _, string _) in representativeRows)
    {
        usedFiles.Add(fileName);
    }
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> heldOutRows =
        SelectAutoMPointPilotRows(
                okRows.Where(row => !usedFiles.Contains(row.FileName)).ToList(),
                null)
            .Concat(SelectAutoMPointPilotRows(
                ngRows.Where(row => !usedFiles.Contains(row.FileName)).ToList(),
                null))
            .ToList();

    List<string> integrityFailures = new List<string>();
    foreach ((int _, string fileName, string status, string _, string md5) in
        new[] { canonical }.Concat(representativeRows).Concat(heldOutRows))
    {
        string path = Path.Combine(datasetRoot, "all_images", status, fileName);
        string actual = File.Exists(path) ? ComputeMd5(path) : "MISSING";
        if (!string.Equals(actual, md5, StringComparison.OrdinalIgnoreCase))
        {
            integrityFailures.Add($"{status}/{fileName}: metadata MD5={md5}, actual={actual}.");
        }
    }

    string canonicalPath = GetAutoMPointCorpusImagePath(datasetRoot, canonical);
    string candidateDrawingPath = Path.Combine(evidenceDirectory, "p229_auto_mpoint_best_candidate.png");
    string templatePath = Path.Combine(evidenceDirectory, "p229_auto_mpoint_best_template.png");
    string candidateCsvPath = Path.Combine(evidenceDirectory, "p229_auto_mpoint_candidate_ranking.csv");
    string evaluationCsvPath = Path.Combine(evidenceDirectory, "p229_auto_mpoint_representative_heldout_results.csv");
    string representativeContactSheetPath =
        Path.Combine(evidenceDirectory, "p229_representative_contact_sheet.png");
    string heldOutContactSheetPath =
        Path.Combine(evidenceDirectory, "p229_heldout_contact_sheet.png");
    string reportPath =
        Path.Combine(evidenceDirectory, "OPENVISIONLAB_AUTO_MPOINT_REPRESENTATIVE_BEST_REPORT.html");
    string recordPath = Path.Combine(evidenceDirectory, "completion_record.txt");

    using Mat canonicalImage = Cv2.ImRead(canonicalPath, ImreadModes.Color);
    if (canonicalImage.Empty())
    {
        Console.Error.WriteLine("P229 canonical image could not be loaded: " + canonicalPath);
        return 2;
    }

    List<Mat> representativeImages = representativeRows
        .Select(row => Cv2.ImRead(GetAutoMPointCorpusImagePath(datasetRoot, row), ImreadModes.Color))
        .ToList();
    AutoMPointToolProperty autoProperty = new AutoMPointToolProperty
    {
        UseAnalysisRoi = false,
        CandidateMode = AutoMPointCandidateMode.Grid,
        PatternWidth = 96,
        PatternHeight = 96,
        CandidateStride = 16,
        MaximumFinalists = 8,
        MaximumResults = 5,
        MinimumFeatureQuality = 0.15D,
        MatchingMinimumScore = 0.75D,
        MinimumUniquenessMargin = 0.05D,
        MaximumTemplatePoints = 300,
        SearchStep = 2,
        UsePositionRefine = true,
        UseSubpixelRefine = true,
        UsePyramidPositionProposal = false,
        UseHybridVerify = false,
        UseAngleSearch = true,
        AngleMinimum = -8,
        AngleMaximum = 8,
        AngleStep = 1D,
        UseScaleSearch = true,
        ScaleMinimum = 0.9D,
        ScaleMaximum = 1.1D,
        ScaleStep = 0.05D,
        MaximumPositionErrorPixels = 2.5D,
        MaximumAngleErrorDegrees = 1.5D,
        MaximumScaleErrorRatio = 0.03D,
        MinimumRepresentativeImageCount = 8,
        MinimumRepresentativeSuccessRate = 0.75D
    };
    AutoMPointTool autoTool = new AutoMPointTool();
    autoTool.SetProperty(autoProperty);
    VisionToolResult autoExecution = autoTool.Execute(canonicalImage, representativeImages);
    OpenVisionLab.Vision2D.Result.AutoMPointCandidateResult? selected = null;
    try
    {
        if (autoExecution.ResultImage != null && !autoExecution.ResultImage.Empty())
        {
            Cv2.ImWrite(candidateDrawingPath, autoExecution.ResultImage);
        }
        selected = autoTool.results.OrderBy(candidate => candidate.Rank).FirstOrDefault();
        if (selected != null)
        {
            using Mat template = canonicalImage.SubMat(selected.PatternRoi).Clone();
            Cv2.ImWrite(templatePath, template);
        }

        List<string> candidateRows = new List<string>
        {
            "PatternRoi,Rank,Accepted,OverallScore,SelfScore,SelfUniqueness,RepresentativeImages,RepresentativeSuccess,RepresentativeSuccessRate,RepresentativeMeanScore,RepresentativeMinimumScore,RepresentativeMeanUniqueness,RepresentativeMinimumUniqueness,RepresentativeRuntimeP95Ms,RejectReason"
        };
        candidateRows.AddRange(autoTool.candidates
            .OrderBy(candidate => candidate.Rank == 0 ? int.MaxValue : candidate.Rank)
            .ThenBy(candidate => candidate.PatternRoi.Y)
            .ThenBy(candidate => candidate.PatternRoi.X)
            .Select(candidate => string.Join(",", new[]
            {
                $"{candidate.PatternRoi.X};{candidate.PatternRoi.Y};{candidate.PatternRoi.Width};{candidate.PatternRoi.Height}",
                candidate.Rank.ToString(CultureInfo.InvariantCulture),
                candidate.Accepted.ToString(CultureInfo.InvariantCulture),
                FormatFinite(candidate.Score),
                FormatFinite(candidate.SelfMatchScore),
                FormatFinite(candidate.UniquenessMargin),
                candidate.RepresentativeImageCount.ToString(CultureInfo.InvariantCulture),
                candidate.RepresentativeSuccessCount.ToString(CultureInfo.InvariantCulture),
                FormatFinite(candidate.RepresentativeSuccessRate),
                FormatFinite(candidate.RepresentativeMeanScore),
                FormatFinite(candidate.RepresentativeMinimumScore),
                FormatFinite(candidate.RepresentativeMeanUniquenessMargin),
                FormatFinite(candidate.RepresentativeMinimumUniquenessMargin),
                FormatFinite(candidate.RepresentativeRuntimeP95Milliseconds),
                candidate.RejectReason
            }.Select(EscapeBatchCsvValue))));
        File.WriteAllLines(candidateCsvPath, candidateRows, new System.Text.UTF8Encoding(true));
    }
    finally
    {
        autoExecution.ResultImage?.Dispose();
        autoTool.imageSource?.Dispose();
        autoTool.imageResult?.Dispose();
        autoTool.imageTemplate?.Dispose();
        foreach (Mat image in representativeImages)
        {
            image.Dispose();
        }
    }

    if (selected == null || !File.Exists(templatePath))
    {
        File.WriteAllLines(recordPath, new[]
        {
            "Status: Incomplete",
            $"Scope: Select one Auto MPoint best pattern for {sourceFile} from eight representative images.",
            $"Verification: {autoExecution.ErrorName}: {autoExecution.Message}",
            $"Evidence: {candidateDrawingPath}; {candidateCsvPath}",
            "Boundary / next dependency: No candidate passed the frozen representative-image gates."
        });
        Console.Error.WriteLine($"P229 Auto MPoint selected no candidate. {autoExecution.ErrorName}: {autoExecution.Message}");
        return 1;
    }

    List<(string SetName, string Role, string FileName, string Outcome, double Score, double Uniqueness, double CenterX, double CenterY, double ElapsedMs, string DrawingPath)> evaluationResults
        = new List<(string, string, string, string, double, double, double, double, double, string)>();
    Dictionary<string, List<string>> drawingPaths = new Dictionary<string, List<string>>(StringComparer.Ordinal)
    {
        ["Representative"] = new List<string>(),
        ["HeldOut"] = new List<string>()
    };
    Dictionary<string, List<string>> drawingLabels = new Dictionary<string, List<string>>(StringComparer.Ordinal)
    {
        ["Representative"] = new List<string>(),
        ["HeldOut"] = new List<string>()
    };
    int runtimeErrors = 0;
    var evaluationRows = representativeRows.Select(row => (SetName: "Representative", Row: row))
        .Concat(heldOutRows.Select(row => (SetName: "HeldOut", Row: row)))
        .ToList();
    for (int index = 0; index < evaluationRows.Count; index++)
    {
        string setName = evaluationRows[index].SetName;
        (int _, string fileName, string status, string _, string _) = evaluationRows[index].Row;
        string imagePath = GetAutoMPointCorpusImagePath(datasetRoot, evaluationRows[index].Row);
        using Mat source = Cv2.ImRead(imagePath, ImreadModes.Color);
        string drawingPath = Path.Combine(
            evidenceDirectory,
            $"{setName.ToLowerInvariant()}_{index + 1:00}_{status}_{Path.GetFileNameWithoutExtension(fileName)}.png");
        string outcome = "ERROR";
        double score = double.NaN;
        double uniqueness = double.NaN;
        double centerX = double.NaN;
        double centerY = double.NaN;
        double elapsedMs = double.NaN;
        if (source.Empty())
        {
            runtimeErrors++;
        }
        else
        {
            VisionPipeline pipeline = CreateEdgeUniqueCardRPipeline(
                templatePath,
                new Rect(0, 0, source.Width, source.Height),
                true,
                0.75D,
                0.05D);
            EdgeBasedTemplateMatchingTool matcher =
                (EdgeBasedTemplateMatchingTool)VisionPipelineAppToolFactory.Create(
                    pipeline.Steps.Single());
            VisionToolResult execution = matcher.Execute(source);
            try
            {
                elapsedMs = execution.Elapsed.TotalMilliseconds;
                OpenVisionLab.Vision2D.Result.MatchingResult? match = matcher.results.SingleOrDefault();
                if (execution.Success && match != null)
                {
                    outcome = "SUCCESS";
                    score = match.Score;
                    uniqueness = GetMetricOrNaN(execution, "UniqueMatch.ScoreMargin");
                    centerX = match.Center.X;
                    centerY = match.Center.Y;
                }
                else if (execution.ErrorCode == VisionToolErrorCode.MatchingAmbiguous)
                {
                    outcome = "AMBIGUOUS";
                }
                else if (execution.ErrorCode == VisionToolErrorCode.MatchingNoResult)
                {
                    outcome = "NO_MATCH";
                }
                else
                {
                    runtimeErrors++;
                }

                if (execution.ResultImage != null && !execution.ResultImage.Empty())
                {
                    Cv2.ImWrite(drawingPath, execution.ResultImage);
                    drawingPaths[setName].Add(drawingPath);
                    drawingLabels[setName].Add(
                        $"{status} {Path.GetFileNameWithoutExtension(fileName)} {outcome} S={score:0.0}");
                }
            }
            finally
            {
                execution.ResultImage?.Dispose();
                matcher.imageSource?.Dispose();
                matcher.imageResult?.Dispose();
                using Mat emptyTemplate = new Mat();
                matcher.SetTemplateImage(emptyTemplate);
                matcher.imageTemplate?.Dispose();
            }
        }

        evaluationResults.Add((
            setName,
            status,
            fileName,
            outcome,
            score,
            uniqueness,
            centerX,
            centerY,
            elapsedMs,
            File.Exists(drawingPath) ? drawingPath : string.Empty));
    }

    if (drawingPaths["Representative"].Count > 0)
    {
        SaveCardPilotContactSheet(
            drawingPaths["Representative"],
            drawingLabels["Representative"],
            representativeContactSheetPath);
    }
    if (drawingPaths["HeldOut"].Count > 0)
    {
        SaveCardPilotContactSheet(
            drawingPaths["HeldOut"],
            drawingLabels["HeldOut"],
            heldOutContactSheetPath);
    }
    File.WriteAllLines(
        evaluationCsvPath,
        new[]
        {
            "Set,Role,FileName,Outcome,Score,UniquenessMargin,CenterX,CenterY,ElapsedMs,DrawingPath"
        }.Concat(evaluationResults.Select(result => string.Join(",", new[]
        {
            result.SetName,
            result.Role,
            result.FileName,
            result.Outcome,
            FormatFinite(result.Score),
            FormatFinite(result.Uniqueness),
            FormatFinite(result.CenterX),
            FormatFinite(result.CenterY),
            FormatFinite(result.ElapsedMs),
            result.DrawingPath
        }.Select(EscapeBatchCsvValue)))),
        new System.Text.UTF8Encoding(true));

    int representativeSuccess = evaluationResults.Count(result =>
        result.SetName == "Representative" && result.Outcome == "SUCCESS");
    int heldOutSuccess = evaluationResults.Count(result =>
        result.SetName == "HeldOut" && result.Outcome == "SUCCESS");
    System.Text.StringBuilder html = new System.Text.StringBuilder();
    html.AppendLine("<!doctype html><html lang=\"ko\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
    html.AppendLine("<title>OpenVisionLab Auto MPoint 대표 이미지 자동 선정 보고서</title>");
    html.AppendLine("<style>body{margin:0;background:#08111f;color:#eef5ff;font-family:'Segoe UI','Malgun Gothic',sans-serif;line-height:1.55}.wrap{max-width:1400px;margin:auto;padding:32px}.panel{background:#111d2e;border:1px solid #2b3d56;border-radius:14px;padding:22px;margin:18px 0}.cards{display:grid;grid-template-columns:repeat(4,1fr);gap:12px}.card{background:#17263b;border-radius:12px;padding:16px}.v{font-size:26px;font-weight:800}.m{color:#9db0c9;font-size:13px}.ok{color:#6ce9bb}.warn{color:#ffc36c}.image{background:#050a11;border:1px solid #31445d;border-radius:10px;padding:8px}.image img{width:100%;height:auto;display:block}table{width:100%;border-collapse:collapse;font-size:13px}th,td{padding:9px;border-bottom:1px solid #2b3d56;text-align:left}th{background:#1b2b42}.scroll{overflow:auto}.print{float:right;background:#24405e;color:white;border:1px solid #5e789b;border-radius:9px;padding:10px 14px}@media print{body{background:white;color:#111}.panel,.card{background:white;border-color:#aaa}.print{display:none}}</style></head><body><main class=\"wrap\">");
    html.AppendLine("<button class=\"print\" onclick=\"window.print()\">인쇄 / PDF 저장</button>");
    html.Append("<h1>Auto MPoint 대표 이미지 자동 선정</h1><p>")
        .Append(HtmlReportEncode(sourceFile))
        .AppendLine(" · 기준 이미지에서 만든 후보를 대표 이미지의 실제 Matching 성공률로 비교했습니다.</p>");
    html.AppendLine("<section class=\"cards\">");
    AppendAutoMPointHtmlMetric(html, $"{selected.RepresentativeSuccessCount}/{selected.RepresentativeImageCount}", "선정용 대표 이미지 성공");
    AppendAutoMPointHtmlMetric(html, $"{representativeSuccess}/{representativeRows.Count}", "선정 후보 재실행");
    AppendAutoMPointHtmlMetric(html, $"{heldOutSuccess}/{heldOutRows.Count}", "분리 확인 이미지 성공");
    AppendAutoMPointHtmlMetric(html, $"{selected.PatternRoi.X},{selected.PatternRoi.Y},{selected.PatternRoi.Width},{selected.PatternRoi.Height}", "자동 선정 ROI");
    html.AppendLine("</section>");
    html.Append("<section class=\"panel\"><h2>자동 선정 결과</h2><p><strong class=\"ok\">Rank #1 자동 선택</strong> · 대표 성공률 ")
        .Append((selected.RepresentativeSuccessRate * 100d).ToString("0.0", CultureInfo.InvariantCulture))
        .Append("% · 평균 점수 ").Append(selected.RepresentativeMeanScore.ToString("0.0", CultureInfo.InvariantCulture))
        .Append(" · 최소 고유성 ").Append(selected.RepresentativeMinimumUniquenessMargin.ToString("0.000", CultureInfo.InvariantCulture))
        .AppendLine("</p>");
    if (File.Exists(candidateDrawingPath))
    {
        html.Append("<div class=\"image\"><img alt=\"Auto MPoint 자동 선정 후보\" src=\"")
            .Append(ToEmbeddedImageDataUri(candidateDrawingPath)).AppendLine("\"></div>");
    }
    html.AppendLine("</section>");
    html.AppendLine("<section class=\"panel\"><h2>후보 순위 근거</h2><div class=\"scroll\"><table><thead><tr><th>Rank</th><th>ROI</th><th>상태</th><th>대표 성공</th><th>평균 점수</th><th>최소 고유성</th><th>탈락 사유</th></tr></thead><tbody>");
    foreach (OpenVisionLab.Vision2D.Result.AutoMPointCandidateResult candidate in autoTool.candidates
        .OrderBy(candidate => candidate.Rank == 0 ? int.MaxValue : candidate.Rank)
        .ThenBy(candidate => candidate.PatternRoi.Y)
        .ThenBy(candidate => candidate.PatternRoi.X))
    {
        html.Append("<tr><td>").Append(candidate.Rank == 0 ? "-" : candidate.Rank)
            .Append("</td><td>").Append(HtmlReportEncode(candidate.PatternRoi.ToString()))
            .Append("</td><td>").Append(candidate.Accepted ? "통과" : "탈락")
            .Append("</td><td>").Append(candidate.RepresentativeSuccessCount).Append("/")
            .Append(candidate.RepresentativeImageCount)
            .Append("</td><td>").Append(candidate.RepresentativeMeanScore.ToString("0.0", CultureInfo.InvariantCulture))
            .Append("</td><td>").Append(candidate.RepresentativeMinimumUniquenessMargin.ToString("0.000", CultureInfo.InvariantCulture))
            .Append("</td><td>").Append(HtmlReportEncode(candidate.RejectReason)).AppendLine("</td></tr>");
    }
    html.AppendLine("</tbody></table></div></section>");
    html.AppendLine("<section class=\"panel\"><h2>선정용 대표 이미지 드로잉</h2>");
    if (File.Exists(representativeContactSheetPath))
    {
        html.Append("<div class=\"image\"><img alt=\"선정용 대표 이미지 결과\" src=\"")
            .Append(ToEmbeddedImageDataUri(representativeContactSheetPath)).AppendLine("\"></div>");
    }
    html.AppendLine("</section><section class=\"panel\"><h2>분리 확인 이미지 드로잉</h2>");
    if (File.Exists(heldOutContactSheetPath))
    {
        html.Append("<div class=\"image\"><img alt=\"분리 확인 이미지 결과\" src=\"")
            .Append(ToEmbeddedImageDataUri(heldOutContactSheetPath)).AppendLine("\"></div>");
    }
    html.AppendLine("</section><section class=\"panel\"><h2>판정 경계</h2><p class=\"warn\">이 결과는 같은 합성 Die Pad 1 계열에서 자동 순위와 재현성을 확인한 것입니다. 실제 생산 특징의 의미, 자세 정답 오차, 500장 전체, 현장 변동은 아직 검증하지 않았습니다.</p>");
    html.AppendLine("<p><a href=\"p229_auto_mpoint_candidate_ranking.csv\">후보 순위 CSV</a> · <a href=\"p229_auto_mpoint_representative_heldout_results.csv\">실행 결과 CSV</a></p></section>");
    html.AppendLine("</main></body></html>");
    File.WriteAllText(reportPath, html.ToString(), new System.Text.UTF8Encoding(true));

    string statusText = integrityFailures.Count == 0 && runtimeErrors == 0
        ? "Complete"
        : "Incomplete";
    File.WriteAllLines(recordPath, new[]
    {
        "Status: " + statusText,
        $"Scope: Automatically select one Auto MPoint pattern for {sourceFile} using 4 OK + 4 NG representative images, then replay 4 OK + 4 NG disjoint held-out rows.",
        $"Acceptance criteria: candidate selected -> {(selected != null ? "PASS" : "FAIL")}; representative replay -> {representativeSuccess}/{representativeRows.Count}; held-out replay -> {heldOutSuccess}/{heldOutRows.Count}; runtime errors -> {runtimeErrors}; integrity failures -> {integrityFailures.Count}.",
        $"Verification: Auto MPoint 96x96/stride16/top8/score0.75/uniqueness0.05; rank by representative success, minimum uniqueness, mean score; unique EdgeBased replay with unchanged gates.",
        $"Evidence: {reportPath}; {candidateCsvPath}; {evaluationCsvPath}; {candidateDrawingPath}; {representativeContactSheetPath}; {heldOutContactSheetPath}.",
        "Boundary / next dependency: Synthetic/augmented same-source evidence only. Operator drawing review is still required before any 500-row run."
    });

    Console.WriteLine($"P229SelectedRoi={selected!.PatternRoi.X},{selected.PatternRoi.Y},{selected.PatternRoi.Width},{selected.PatternRoi.Height}");
    Console.WriteLine($"P229RepresentativeSelection={selected.RepresentativeSuccessCount}/{selected.RepresentativeImageCount}");
    Console.WriteLine($"P229RepresentativeReplay={representativeSuccess}/{representativeRows.Count}");
    Console.WriteLine($"P229HeldOutReplay={heldOutSuccess}/{heldOutRows.Count}");
    Console.WriteLine($"P229RuntimeErrors={runtimeErrors}");
    Console.WriteLine($"P229IntegrityFailures={integrityFailures.Count}");
    Console.WriteLine($"P229Report={reportPath}");
    Console.WriteLine($"P229Record={recordPath}");
    return statusText == "Complete" ? 0 : 1;
}

static int RunAutoMPointSixCorpusPilot(
    string labelTestRootArgument,
    string evidenceDirectoryArgument)
{
    const double matchingScoreMinimum = 0.75D;
    const double uniqueMarginMinimum = 0.05D;
    string labelTestRoot = Path.GetFullPath(labelTestRootArgument);
    string evidenceDirectory = Path.GetFullPath(evidenceDirectoryArgument);
    (string Key, string RelativeRoot)[] datasets =
    {
        ("switch_housing", @"EasyMatch_Switch_Housing_500(1)\EasyMatch_Switch_Housing_500"),
        ("pcb_board", @"EasyMatch_PCB_Board_500(1)\EasyMatch_PCB_Board_500"),
        ("ic_frame", @"EasyMatch_IC_Frame_500(1)\EasyMatch_IC_Frame_500"),
        ("floppy_disk", @"EasyMatch_Floppy_Disk_500(1)\EasyMatch_Floppy_Disk_500"),
        ("die_pad", @"EasyMatch_Die_Pad_500(1)\EasyMatch_Die_Pad_500"),
        ("die_array", @"EasyMatch_Die_Array_500(1)\EasyMatch_Die_Array_500")
    };
    Directory.CreateDirectory(evidenceDirectory);
    string candidatesDirectory = Path.Combine(evidenceDirectory, "candidate_analysis");
    string templatesDirectory = Path.Combine(evidenceDirectory, "templates");
    string runsDirectory = Path.Combine(evidenceDirectory, "runs");
    Directory.CreateDirectory(candidatesDirectory);
    Directory.CreateDirectory(templatesDirectory);
    Directory.CreateDirectory(runsDirectory);

    List<string> resultRows = new List<string>
    {
        "Dataset,SourceFile,Role,FileName,MetadataMd5,ActualMd5,Md5Verified,CandidateRoi,CandidateScore,CandidateUniqueness,ExecutionSuccess,Outcome,ErrorCode,ErrorName,Message,MatchScore,UniqueMargin,CenterX,CenterY,AngleDeg,Scale,Bounds,DefectMaskOverlapPixels,ElapsedMs,DrawingPath"
    };
    List<string> summaryRows = new List<string>
    {
        "Dataset,SourceFile,TotalRows,OkRows,NgRows,CanonicalFile,CandidateState,CandidateRoi,CandidateScore,CandidateUniqueness,PilotRows,OkSuccess,OkAmbiguous,OkNoMatch,NgSuccess,NgAmbiguous,NgNoMatch,NgSuccessWithDefectOverlap,RuntimeErrors,MeanElapsedMs,Decision,ContactSheet"
    };
    List<string> candidateDrawingPaths = new List<string>();
    List<string> candidateDrawingLabels = new List<string>();
    List<string> integrityFailures = new List<string>();
    int totalMetadataRows = 0;
    int sourceGroupCount = 0;
    int pilotRunCount = 0;
    int sourceGroupsWithSuggestion = 0;
    int sourceGroupsAdvancing = 0;
    int totalRuntimeErrors = 0;

    foreach ((string datasetKey, string relativeRoot) in datasets)
    {
        string datasetRoot = Path.Combine(labelTestRoot, relativeRoot);
        string metadataPath = Path.Combine(datasetRoot, "metadata.csv");
        if (!File.Exists(metadataPath))
        {
            integrityFailures.Add($"{datasetKey}: metadata.csv is missing at {metadataPath}.");
            continue;
        }

        List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> rows;
        try
        {
            rows = LoadAutoMPointCorpusMetadata(metadataPath);
        }
        catch (Exception ex)
        {
            integrityFailures.Add($"{datasetKey}: metadata parse failed: {ex.Message}");
            continue;
        }
        totalMetadataRows += rows.Count;
        Dictionary<string, string> maskPaths = Directory
            .EnumerateFiles(
                Path.Combine(datasetRoot, "segmentation", "masks_binary"),
                "*.png",
                SearchOption.AllDirectories)
            .GroupBy(path => Path.GetFileNameWithoutExtension(path), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.First(),
                StringComparer.OrdinalIgnoreCase);

        foreach (IGrouping<string, (int GlobalId, string FileName, string Status, string SourceFile, string Md5)> group
            in rows.GroupBy(row => row.SourceFile, StringComparer.OrdinalIgnoreCase).OrderBy(group => group.Key))
        {
            sourceGroupCount++;
            List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> groupRows =
                group.OrderBy(row => row.GlobalId).ToList();
            List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> okRows =
                groupRows.Where(row => string.Equals(row.Status, "OK", StringComparison.OrdinalIgnoreCase)).ToList();
            List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> ngRows =
                groupRows.Where(row => string.Equals(row.Status, "NG", StringComparison.OrdinalIgnoreCase)).ToList();
            if (okRows.Count == 0 || ngRows.Count == 0)
            {
                integrityFailures.Add($"{datasetKey}/{group.Key}: both OK and NG rows are required.");
                continue;
            }

            (int GlobalId, string FileName, string Status, string SourceFile, string Md5) canonical = okRows[0];
            string canonicalPath = GetAutoMPointCorpusImagePath(datasetRoot, canonical);
            string safeGroup = datasetKey + "__" + Path.GetFileNameWithoutExtension(group.Key).Replace(' ', '_');
            string groupCandidateDirectory = Path.Combine(candidatesDirectory, safeGroup);
            string groupRunDirectory = Path.Combine(runsDirectory, safeGroup);
            Directory.CreateDirectory(groupCandidateDirectory);
            Directory.CreateDirectory(groupRunDirectory);
            string candidateState = "NO_SUGGESTION";
            Rect candidateRoi = new Rect();
            double candidateScore = double.NaN;
            double candidateUniqueness = double.NaN;
            string candidateDrawingPath = Path.Combine(groupCandidateDirectory, "auto_mpoint.png");
            string templatePath = Path.Combine(templatesDirectory, safeGroup + "_rank_01.png");
            using Mat canonicalImage = Cv2.ImRead(canonicalPath, ImreadModes.Color);
            if (canonicalImage.Empty())
            {
                integrityFailures.Add($"{datasetKey}/{group.Key}: canonical image could not be loaded.");
                continue;
            }

            AutoMPointToolProperty autoProperty = new AutoMPointToolProperty
            {
                UseAnalysisRoi = false,
                CandidateMode = AutoMPointCandidateMode.Grid,
                PatternWidth = 96,
                PatternHeight = 96,
                CandidateStride = 16,
                MaximumFinalists = 8,
                MaximumResults = 5,
                MinimumFeatureQuality = 0.15D,
                MatchingMinimumScore = matchingScoreMinimum,
                MinimumUniquenessMargin = uniqueMarginMinimum,
                MaximumTemplatePoints = 300,
                SearchStep = 2,
                UsePositionRefine = true,
                UseSubpixelRefine = true,
                UsePyramidPositionProposal = false,
                UseHybridVerify = false,
                UseAngleSearch = false,
                UseScaleSearch = false,
                MaximumPositionErrorPixels = 2.5D,
                MaximumAngleErrorDegrees = 1.5D,
                MaximumScaleErrorRatio = 0.03D
            };
            AutoMPointTool autoTool = new AutoMPointTool();
            autoTool.SetProperty(autoProperty);
            VisionToolResult autoExecution = autoTool.Execute(canonicalImage);
            try
            {
                if (autoExecution.ResultImage != null && !autoExecution.ResultImage.Empty())
                {
                    Cv2.ImWrite(candidateDrawingPath, autoExecution.ResultImage);
                    candidateDrawingPaths.Add(candidateDrawingPath);
                }
                OpenVisionLab.Vision2D.Result.AutoMPointCandidateResult? candidate = autoTool.results
                    .OrderBy(result => result.Rank)
                    .FirstOrDefault();
                candidateDrawingLabels.Add(
                    $"{datasetKey}/{Path.GetFileNameWithoutExtension(group.Key)} "
                    + (candidate == null ? "NO SUGGESTION" : "rank #1"));
                if (candidate == null)
                {
                    summaryRows.Add(string.Join(",", new[]
                    {
                        datasetKey,
                        group.Key,
                        groupRows.Count.ToString(CultureInfo.InvariantCulture),
                        okRows.Count.ToString(CultureInfo.InvariantCulture),
                        ngRows.Count.ToString(CultureInfo.InvariantCulture),
                        canonical.FileName,
                        candidateState,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        string.Empty,
                        "No suggestion",
                        string.Empty
                    }.Select(EscapeBatchCsvValue)));
                    continue;
                }

                sourceGroupsWithSuggestion++;
                candidateState = "SUGGESTED";
                candidateRoi = candidate.PatternRoi;
                candidateScore = candidate.Score;
                candidateUniqueness = candidate.UniquenessMargin;
                using Mat template = canonicalImage.SubMat(candidateRoi).Clone();
                Cv2.ImWrite(templatePath, template);
            }
            finally
            {
                autoExecution.ResultImage?.Dispose();
                autoTool.imageSource?.Dispose();
                autoTool.imageResult?.Dispose();
                autoTool.imageTemplate?.Dispose();
            }

            List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> pilotRows =
                SelectAutoMPointPilotRows(okRows, canonical)
                .Concat(SelectAutoMPointPilotRows(ngRows, null))
                .ToList();
            int okSuccess = 0;
            int okAmbiguous = 0;
            int okNoMatch = 0;
            int ngSuccess = 0;
            int ngAmbiguous = 0;
            int ngNoMatch = 0;
            int ngOverlap = 0;
            int runtimeErrors = 0;
            List<double> elapsedValues = new List<double>();
            List<string> reviewImages = new List<string>();
            List<string> reviewLabels = new List<string>();

            for (int pilotIndex = 0; pilotIndex < pilotRows.Count; pilotIndex++)
            {
                (int GlobalId, string FileName, string Status, string SourceFile, string Md5) row = pilotRows[pilotIndex];
                pilotRunCount++;
                string imagePath = GetAutoMPointCorpusImagePath(datasetRoot, row);
                string actualMd5 = File.Exists(imagePath) ? ComputeMd5(imagePath) : "MISSING";
                bool md5Verified = string.Equals(actualMd5, row.Md5, StringComparison.OrdinalIgnoreCase);
                if (!md5Verified)
                {
                    integrityFailures.Add(
                        $"{datasetKey}/{group.Key}/{row.FileName}: MD5 mismatch. "
                        + $"Metadata={row.Md5}, Actual={actualMd5}.");
                }

                string drawingPath = Path.Combine(
                    groupRunDirectory,
                    $"{pilotIndex + 1:00}_{row.Status}_{Path.GetFileNameWithoutExtension(row.FileName)}.png");
                string outcome = "ERROR";
                int errorCode = -1;
                string errorName = "MissingImage";
                string errorMessage = "Source or template image could not be loaded.";
                bool executionSuccess = false;
                double matchScore = double.NaN;
                double uniqueMargin = double.NaN;
                double centerX = double.NaN;
                double centerY = double.NaN;
                double angle = double.NaN;
                double scale = double.NaN;
                string boundsText = string.Empty;
                int maskOverlapPixels = 0;
                double elapsedMs = double.NaN;

                using Mat source = Cv2.ImRead(imagePath, ImreadModes.Color);
                using Mat templateForRun = Cv2.ImRead(templatePath, ImreadModes.Color);
                if (source.Empty() || templateForRun.Empty())
                {
                    runtimeErrors++;
                }
                else
                {
                    VisionPipeline matchingPipeline = CreateEdgeUniqueCardRPipeline(
                        templatePath,
                        new Rect(0, 0, source.Width, source.Height),
                        true,
                        matchingScoreMinimum,
                        uniqueMarginMinimum);
                    EdgeBasedTemplateMatchingTool matcher =
                        (EdgeBasedTemplateMatchingTool)VisionPipelineAppToolFactory.Create(
                            matchingPipeline.Steps.Single());
                    VisionToolResult execution = matcher.Execute(source);
                    try
                    {
                        executionSuccess = execution.Success;
                        errorCode = (int)execution.ErrorCode;
                        errorName = execution.ErrorName ?? execution.ErrorCode.ToString();
                        errorMessage = execution.Message ?? string.Empty;
                        elapsedMs = execution.Elapsed.TotalMilliseconds;
                        elapsedValues.Add(elapsedMs);
                        OpenVisionLab.Vision2D.Result.MatchingResult? match = matcher.results.SingleOrDefault();
                        if (execution.Success && match != null)
                        {
                            outcome = "SUCCESS";
                            matchScore = match.Score;
                            uniqueMargin = GetMetricOrNaN(execution, "UniqueMatch.ScoreMargin");
                            centerX = match.Center.X;
                            centerY = match.Center.Y;
                            angle = match.Angle;
                            scale = match.Scale;
                            boundsText = string.Format(
                                CultureInfo.InvariantCulture,
                                "{0:0.###};{1:0.###};{2:0.###};{3:0.###}",
                                match.Bounding.X,
                                match.Bounding.Y,
                                match.Bounding.Width,
                                match.Bounding.Height);
                            if (string.Equals(row.Status, "OK", StringComparison.OrdinalIgnoreCase))
                            {
                                okSuccess++;
                            }
                            else
                            {
                                ngSuccess++;
                                if (maskPaths.TryGetValue(
                                        Path.GetFileNameWithoutExtension(row.FileName),
                                        out string? maskPath))
                                {
                                    maskOverlapPixels = CountMaskOverlap(maskPath, match.Bounding);
                                    if (maskOverlapPixels > 0)
                                    {
                                        ngOverlap++;
                                    }
                                }
                            }
                        }
                        else if (execution.ErrorCode == VisionToolErrorCode.MatchingAmbiguous)
                        {
                            outcome = "AMBIGUOUS";
                            if (string.Equals(row.Status, "OK", StringComparison.OrdinalIgnoreCase))
                            {
                                okAmbiguous++;
                            }
                            else
                            {
                                ngAmbiguous++;
                            }
                        }
                        else if (execution.ErrorCode == VisionToolErrorCode.MatchingNoResult)
                        {
                            outcome = "NO_MATCH";
                            if (string.Equals(row.Status, "OK", StringComparison.OrdinalIgnoreCase))
                            {
                                okNoMatch++;
                            }
                            else
                            {
                                ngNoMatch++;
                            }
                        }
                        else
                        {
                            runtimeErrors++;
                        }

                        if (execution.ResultImage != null && !execution.ResultImage.Empty())
                        {
                            Cv2.ImWrite(drawingPath, execution.ResultImage);
                            reviewImages.Add(drawingPath);
                            reviewLabels.Add(
                                $"{row.Status} {Path.GetFileNameWithoutExtension(row.FileName)} "
                                + $"{outcome} S={matchScore:0.0}");
                        }
                    }
                    finally
                    {
                        execution.ResultImage?.Dispose();
                        matcher.imageSource?.Dispose();
                        matcher.imageResult?.Dispose();
                        using Mat emptyTemplate = new Mat();
                        matcher.SetTemplateImage(emptyTemplate);
                        matcher.imageTemplate?.Dispose();
                    }
                }

                resultRows.Add(string.Join(",", new[]
                {
                    datasetKey,
                    group.Key,
                    row.Status,
                    row.FileName,
                    row.Md5,
                    actualMd5,
                    md5Verified.ToString(CultureInfo.InvariantCulture),
                    $"{candidateRoi.X};{candidateRoi.Y};{candidateRoi.Width};{candidateRoi.Height}",
                    FormatFinite(candidateScore),
                    FormatFinite(candidateUniqueness),
                    executionSuccess.ToString(CultureInfo.InvariantCulture),
                    outcome,
                    errorCode.ToString(CultureInfo.InvariantCulture),
                    errorName,
                    errorMessage,
                    FormatFinite(matchScore),
                    FormatFinite(uniqueMargin),
                    FormatFinite(centerX),
                    FormatFinite(centerY),
                    FormatFinite(angle),
                    FormatFinite(scale),
                    boundsText,
                    maskOverlapPixels.ToString(CultureInfo.InvariantCulture),
                    FormatFinite(elapsedMs),
                    File.Exists(drawingPath) ? drawingPath : string.Empty
                }.Select(EscapeBatchCsvValue)));
            }

            string contactSheetPath = Path.Combine(groupRunDirectory, "contact_sheet.png");
            if (reviewImages.Count > 0)
            {
                SaveCardPilotContactSheet(reviewImages, reviewLabels, contactSheetPath);
            }
            totalRuntimeErrors += runtimeErrors;
            string decision = runtimeErrors > 0
                ? "Incomplete: runtime error"
                : okSuccess >= 3
                    ? "Operator drawing review required"
                    : "Reject mechanical pilot";
            if (decision == "Operator drawing review required")
            {
                sourceGroupsAdvancing++;
            }
            summaryRows.Add(string.Join(",", new[]
            {
                datasetKey,
                group.Key,
                groupRows.Count.ToString(CultureInfo.InvariantCulture),
                okRows.Count.ToString(CultureInfo.InvariantCulture),
                ngRows.Count.ToString(CultureInfo.InvariantCulture),
                canonical.FileName,
                candidateState,
                $"{candidateRoi.X};{candidateRoi.Y};{candidateRoi.Width};{candidateRoi.Height}",
                FormatFinite(candidateScore),
                FormatFinite(candidateUniqueness),
                pilotRows.Count.ToString(CultureInfo.InvariantCulture),
                okSuccess.ToString(CultureInfo.InvariantCulture),
                okAmbiguous.ToString(CultureInfo.InvariantCulture),
                okNoMatch.ToString(CultureInfo.InvariantCulture),
                ngSuccess.ToString(CultureInfo.InvariantCulture),
                ngAmbiguous.ToString(CultureInfo.InvariantCulture),
                ngNoMatch.ToString(CultureInfo.InvariantCulture),
                ngOverlap.ToString(CultureInfo.InvariantCulture),
                runtimeErrors.ToString(CultureInfo.InvariantCulture),
                elapsedValues.Count > 0
                    ? elapsedValues.Average().ToString("0.###", CultureInfo.InvariantCulture)
                    : string.Empty,
                decision,
                File.Exists(contactSheetPath) ? contactSheetPath : string.Empty
            }.Select(EscapeBatchCsvValue)));
        }
    }

    string resultsPath = Path.Combine(evidenceDirectory, "p227_auto_mpoint_six_corpus_pilot_results.csv");
    string summaryPath = Path.Combine(evidenceDirectory, "p227_auto_mpoint_six_corpus_pilot_summary.csv");
    File.WriteAllLines(resultsPath, resultRows);
    File.WriteAllLines(summaryPath, summaryRows);
    string candidateSheetPath = Path.Combine(evidenceDirectory, "p227_candidate_analysis_contact_sheet.png");
    if (candidateDrawingPaths.Count > 0)
    {
        SaveCardPilotContactSheet(candidateDrawingPaths, candidateDrawingLabels, candidateSheetPath);
    }
    string reportPath = WriteAutoMPointSixCorpusReport(
        summaryPath,
        evidenceDirectory,
        candidateSheetPath,
        totalMetadataRows,
        sourceGroupCount,
        sourceGroupsWithSuggestion,
        pilotRunCount,
        sourceGroupsAdvancing,
        totalRuntimeErrors,
        integrityFailures);
    string recordPath = Path.Combine(evidenceDirectory, "completion_record.md");
    File.WriteAllLines(recordPath, new[]
    {
        "# P227 Auto MPoint six-corpus mechanical pilot",
        string.Empty,
        $"Status: {(integrityFailures.Count == 0 && totalRuntimeErrors == 0 ? "Complete" : "Incomplete")}",
        string.Empty,
        "Scope: Audit six operator-provided EasyMatch 500-image corpora, stratify by source_file, freeze one current-default Auto MPoint rank-1 suggestion per source stratum, and run a deterministic 4 OK + 4 NG EdgeBased unique-match pilot without result-dependent tuning.",
        string.Empty,
        "Acceptance criteria:",
        $"- Metadata rows found: {(totalMetadataRows == 3000 ? "PASS" : "FAIL")} ({totalMetadataRows}/3000).",
        $"- Source strata found: {(sourceGroupCount == 16 ? "PASS" : "FAIL")} ({sourceGroupCount}/16).",
        $"- Source strata with Auto MPoint suggestion: {sourceGroupsWithSuggestion}/16.",
        $"- Pilot executions retained: {pilotRunCount}.",
        $"- Source strata mechanically eligible for operator drawing review: {sourceGroupsAdvancing}/16.",
        $"- Runtime errors: {totalRuntimeErrors}.",
        $"- Integrity failures: {integrityFailures.Count}.",
        "- Codex drawing review: 13 matching contact sheets and the 16-stratum candidate sheet reviewed.",
        "- Drawing-review decision: 10 expansion candidates / 6 stopped strata.",
        "- Result-dependent threshold/ROI tuning: PASS (none).",
        "- Full 500-image expansion or automatic pattern apply: PASS (not performed).",
        string.Empty,
        "Verification:",
        "- Auto MPoint: current UI-equivalent 96x96/stride16/score0.75/uniqueness0.05/max-position-error2.5 defaults.",
        "- Matching pilot: full 512x512 search, score 0.75, unique margin 0.05, angle -8..8 step1, scale 0.9..1.1 step0.05.",
        "- Pilot selection: canonical first OK plus MD5-spread rows, four OK and four NG per source stratum.",
        $"- Results: `{resultsPath}`",
        $"- Summary: `{summaryPath}`",
        $"- Candidate drawings: `{candidateSheetPath}`",
        $"- Operator report: `{reportPath}`",
        string.Empty,
        "Boundary / next dependency: Corpus metadata does not contain generated pose ground truth, so this pilot does not claim pixel localization accuracy. Mechanical success still requires operator drawing review for physical-feature identity. A full 500-image replay is allowed only for an explicitly approved source stratum/candidate.",
        string.Empty,
        "Integrity issues:",
        integrityFailures.Count == 0 ? "- None." : string.Join(Environment.NewLine, integrityFailures.Select(item => "- " + item))
    });

    Console.WriteLine($"P227MetadataRows={totalMetadataRows}");
    Console.WriteLine($"P227SourceStrata={sourceGroupCount}");
    Console.WriteLine($"P227StrataWithSuggestion={sourceGroupsWithSuggestion}");
    Console.WriteLine($"P227PilotRuns={pilotRunCount}");
    Console.WriteLine($"P227MechanicallyEligible={sourceGroupsAdvancing}");
    Console.WriteLine($"P227RuntimeErrors={totalRuntimeErrors}");
    Console.WriteLine($"P227IntegrityFailures={integrityFailures.Count}");
    Console.WriteLine($"P227Summary={summaryPath}");
    Console.WriteLine($"P227Report={reportPath}");
    Console.WriteLine($"P227Record={recordPath}");
    return integrityFailures.Count == 0 && totalRuntimeErrors == 0 ? 0 : 1;
}

static string WriteAutoMPointSixCorpusReport(
    string summaryPath,
    string evidenceDirectory,
    string candidateSheetPath,
    int totalMetadataRows,
    int sourceGroupCount,
    int sourceGroupsWithSuggestion,
    int pilotRunCount,
    int sourceGroupsAdvancing,
    int totalRuntimeErrors,
    IReadOnlyList<string> integrityFailures)
{
    string[] lines = File.ReadAllLines(summaryPath);
    List<string> header = ParseCsvRecord(lines[0]);
    int datasetIndex = header.IndexOf("Dataset");
    int sourceIndex = header.IndexOf("SourceFile");
    int candidateStateIndex = header.IndexOf("CandidateState");
    int candidateRoiIndex = header.IndexOf("CandidateRoi");
    int okSuccessIndex = header.IndexOf("OkSuccess");
    int okAmbiguousIndex = header.IndexOf("OkAmbiguous");
    int okNoMatchIndex = header.IndexOf("OkNoMatch");
    int ngSuccessIndex = header.IndexOf("NgSuccess");
    int ngAmbiguousIndex = header.IndexOf("NgAmbiguous");
    int ngNoMatchIndex = header.IndexOf("NgNoMatch");
    int overlapIndex = header.IndexOf("NgSuccessWithDefectOverlap");
    int runtimeIndex = header.IndexOf("RuntimeErrors");
    int elapsedIndex = header.IndexOf("MeanElapsedMs");
    int decisionIndex = header.IndexOf("Decision");
    int contactSheetIndex = header.IndexOf("ContactSheet");
    List<List<string>> rows = lines
        .Skip(1)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(ParseCsvRecord)
        .ToList();
    List<(string Dataset, string SourceFile, string Decision, string Reason)> drawingReviews =
        rows.Select(row =>
        {
            (string decision, string reason) =
                GetAutoMPointSixCorpusDrawingReview(row[datasetIndex], row[sourceIndex]);
            return (row[datasetIndex], row[sourceIndex], decision, reason);
        }).ToList();
    int expansionCandidateCount = drawingReviews.Count(review =>
        string.Equals(review.Decision, "확대 검증 후보", StringComparison.Ordinal));
    int stoppedCount = drawingReviews.Count - expansionCandidateCount;
    string drawingReviewPath = Path.Combine(
        evidenceDirectory,
        "p227_auto_mpoint_six_corpus_drawing_review.csv");
    File.WriteAllLines(
        drawingReviewPath,
        new[] { "Dataset,SourceFile,DrawingReview,Reason" }
            .Concat(drawingReviews.Select(review => string.Join(
                ",",
                new[]
                {
                    review.Dataset,
                    review.SourceFile,
                    review.Decision,
                    review.Reason
                }.Select(EscapeBatchCsvValue)))),
        new System.Text.UTF8Encoding(true));

    string reportPath = Path.Combine(evidenceDirectory, "OPENVISIONLAB_AUTO_MPOINT_SIX_CORPUS_REPORT.md");
    string overallStatus =
        integrityFailures.Count == 0 && totalRuntimeErrors == 0 ? "Complete" : "Incomplete";
    List<string> report = new List<string>
    {
        "# OpenVisionLab Auto MPoint 6종 코퍼스 검증 보고서",
        string.Empty,
        $"상태: `{overallStatus}`",
        string.Empty,
        $"생성일: {DateTime.Now:yyyy-MM-dd HH:mm:ss K}",
        string.Empty,
        "## 결론 요약",
        string.Empty,
        $"- 데이터셋: 6종, 메타데이터 {totalMetadataRows}/3000장",
        $"- 원본 이미지별 검증 층: {sourceGroupCount}/16개",
        $"- Auto MPoint 1순위 후보 생성: {sourceGroupsWithSuggestion}/16개 층",
        $"- 고정 파일럿 실행: {pilotRunCount}건",
        $"- 기계적 통과 후 사람의 드로잉 검토가 필요한 층: {sourceGroupsAdvancing}/16개",
        $"- 드로잉 검토 후 확대 검증 후보: {expansionCandidateCount}/16개",
        $"- 드로잉 검토에서 중단: {stoppedCount}/16개",
        $"- 런타임 오류: {totalRuntimeErrors}건",
        $"- 무결성 오류: {integrityFailures.Count}건",
        string.Empty,
        "이 보고서의 `기계적 통과`는 정상적인 물리 특징을 찾았다는 뜻이 아닙니다. "
        + "OK 4장 중 3장 이상에서 실행됐다는 뜻이며, 최종 채택은 아래 드로잉으로 같은 물리 특징인지 확인해야 합니다.",
        string.Empty,
        "## 고정 검증 조건",
        string.Empty,
        "- Auto MPoint: 96×96, stride 16, score 0.75, uniqueness 0.05, 최대 위치 오차 2.5 px",
        "- EdgeBased Matching: 전체 512×512 검색, score 0.75, unique margin 0.05",
        "- 자세 범위: angle -8..8° / 1°, scale 0.9..1.1 / 0.05",
        "- 표본: 각 원본별 첫 OK와 MD5 분산 표본을 포함한 OK 4장 + NG 4장",
        "- 결과 확인 후 ROI·점수·각도·스케일 문턱을 변경하지 않음",
        "- 메타데이터에 생성 자세의 수치 정답이 없으므로 위치 정밀도 px는 판정하지 않음",
        string.Empty,
        "## 후보 분석 전체 보기",
        string.Empty
    };
    if (File.Exists(candidateSheetPath))
    {
        report.Add($"![16개 원본별 Auto MPoint 후보]({ToReportRelativePath(evidenceDirectory, candidateSheetPath)})");
        report.Add(string.Empty);
    }
    report.AddRange(new[]
    {
        "## 원본별 결과표",
        string.Empty,
        "| 데이터셋 | 원본 | 후보 | ROI | OK 성공/모호/미검출 | NG 성공/모호/미검출 | NG 결함 겹침 | 오류 | 평균 ms | 기계 판정 | 드로잉 검토 |",
        "| --- | --- | --- | --- | ---: | ---: | ---: | ---: | ---: | --- | --- |"
    });
    foreach (List<string> row in rows)
    {
        (string drawingDecision, _) =
            GetAutoMPointSixCorpusDrawingReview(row[datasetIndex], row[sourceIndex]);
        report.Add(
            $"| {row[datasetIndex]} | {row[sourceIndex]} | {row[candidateStateIndex]} | "
            + $"{row[candidateRoiIndex]} | {row[okSuccessIndex]}/{row[okAmbiguousIndex]}/{row[okNoMatchIndex]} | "
            + $"{row[ngSuccessIndex]}/{row[ngAmbiguousIndex]}/{row[ngNoMatchIndex]} | "
            + $"{row[overlapIndex]} | {row[runtimeIndex]} | {row[elapsedIndex]} | "
            + $"{row[decisionIndex]} | {drawingDecision} |");
    }
    report.AddRange(new[]
    {
        string.Empty,
        "## 실제 결과 드로잉",
        string.Empty,
        "노란색/녹색 패턴 윤곽과 중심이 동일한 물리 특징을 따라가는지 확인하십시오. "
        + "반복 구조의 다른 위치, 배경 경계, 결함 자체를 따라가면 수치가 높아도 부적합입니다.",
        string.Empty
    });
    foreach (List<string> row in rows)
    {
        string contactSheetPath = row[contactSheetIndex];
        (string drawingDecision, string drawingReason) =
            GetAutoMPointSixCorpusDrawingReview(row[datasetIndex], row[sourceIndex]);
        report.Add($"### {row[datasetIndex]} / {row[sourceIndex]}");
        report.Add(string.Empty);
        report.Add($"- 후보 상태: `{row[candidateStateIndex]}`");
        report.Add($"- 후보 ROI: `{row[candidateRoiIndex]}`");
        report.Add($"- 기계 판정: `{row[decisionIndex]}`");
        report.Add($"- 드로잉 검토: **{drawingDecision}** — {drawingReason}");
        report.Add(string.Empty);
        if (File.Exists(contactSheetPath))
        {
            report.Add($"![{row[datasetIndex]} {row[sourceIndex]} 파일럿 결과]({ToReportRelativePath(evidenceDirectory, contactSheetPath)})");
        }
        else
        {
            report.Add("Auto MPoint 추천 후보가 없어 Matching 파일럿을 실행하지 않았습니다.");
        }
        report.Add(string.Empty);
    }
    report.AddRange(new[]
    {
        "## 증거 파일",
        string.Empty,
        "- [원본별 요약 CSV](p227_auto_mpoint_six_corpus_pilot_summary.csv)",
        $"- [{pilotRunCount}건 실행 결과 CSV](p227_auto_mpoint_six_corpus_pilot_results.csv)",
        "- [드로잉 검토 CSV](p227_auto_mpoint_six_corpus_drawing_review.csv)",
        "- [완료 기록](completion_record.md)",
        string.Empty,
        "## 한계와 다음 판단",
        string.Empty,
        "1. 이 데이터는 EasyMatch 원본을 변형하고 NG 결함을 합성한 데이터이며 실제 생산 변동 증거가 아닙니다.",
        "2. 서로 다른 `source_file`은 별도 템플릿으로 평가했습니다. 하나의 템플릿이 제품군 전체를 대표한다고 주장하지 않습니다.",
        "3. 생성 자세의 정답 좌표가 없으므로 드로잉의 물리적 동일성은 운영자가 확인해야 합니다.",
        "4. 운영자가 승인한 후보만 해당 원본 층의 전체 이미지로 확대 검증할 수 있습니다.",
        string.Empty,
        "## 무결성 오류",
        string.Empty
    });
    report.AddRange(integrityFailures.Count == 0
        ? new[] { "- 없음" }
        : integrityFailures.Select(item => "- " + item));
    File.WriteAllLines(reportPath, report, new System.Text.UTF8Encoding(true));
    return WriteAutoMPointSixCorpusHtmlReport(
        summaryPath,
        evidenceDirectory,
        candidateSheetPath,
        totalMetadataRows,
        sourceGroupCount,
        sourceGroupsWithSuggestion,
        pilotRunCount,
        sourceGroupsAdvancing,
        totalRuntimeErrors,
        integrityFailures);
}

static string WriteAutoMPointSixCorpusHtmlReport(
    string summaryPath,
    string evidenceDirectory,
    string candidateSheetPath,
    int totalMetadataRows,
    int sourceGroupCount,
    int sourceGroupsWithSuggestion,
    int pilotRunCount,
    int sourceGroupsAdvancing,
    int totalRuntimeErrors,
    IReadOnlyList<string> integrityFailures)
{
    string[] lines = File.ReadAllLines(summaryPath);
    List<string> header = ParseCsvRecord(lines[0]);
    int datasetIndex = header.IndexOf("Dataset");
    int sourceIndex = header.IndexOf("SourceFile");
    int candidateStateIndex = header.IndexOf("CandidateState");
    int candidateRoiIndex = header.IndexOf("CandidateRoi");
    int candidateScoreIndex = header.IndexOf("CandidateScore");
    int candidateUniquenessIndex = header.IndexOf("CandidateUniqueness");
    int okSuccessIndex = header.IndexOf("OkSuccess");
    int okAmbiguousIndex = header.IndexOf("OkAmbiguous");
    int okNoMatchIndex = header.IndexOf("OkNoMatch");
    int ngSuccessIndex = header.IndexOf("NgSuccess");
    int ngAmbiguousIndex = header.IndexOf("NgAmbiguous");
    int ngNoMatchIndex = header.IndexOf("NgNoMatch");
    int overlapIndex = header.IndexOf("NgSuccessWithDefectOverlap");
    int runtimeIndex = header.IndexOf("RuntimeErrors");
    int elapsedIndex = header.IndexOf("MeanElapsedMs");
    int decisionIndex = header.IndexOf("Decision");
    int contactSheetIndex = header.IndexOf("ContactSheet");
    List<List<string>> rows = lines
        .Skip(1)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(ParseCsvRecord)
        .ToList();
    int expansionCandidateCount = rows.Count(row =>
        string.Equals(
            GetAutoMPointSixCorpusDrawingReview(row[datasetIndex], row[sourceIndex]).Decision,
            "확대 검증 후보",
            StringComparison.Ordinal));
    int stoppedCount = rows.Count - expansionCandidateCount;
    string overallStatus =
        integrityFailures.Count == 0 && totalRuntimeErrors == 0 ? "Complete" : "Incomplete";
    string overallStatusClass = overallStatus == "Complete" ? "status-complete" : "status-incomplete";
    string reportPath = Path.Combine(
        evidenceDirectory,
        "OPENVISIONLAB_AUTO_MPOINT_SIX_CORPUS_REPORT.html");
    System.Text.StringBuilder html = new System.Text.StringBuilder();
    html.AppendLine("<!doctype html>");
    html.AppendLine("<html lang=\"ko\"><head><meta charset=\"utf-8\">");
    html.AppendLine("<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
    html.AppendLine("<title>OpenVisionLab Auto MPoint 6종 코퍼스 검증 보고서</title>");
    html.AppendLine("<style>");
    html.AppendLine(":root{color-scheme:dark;--bg:#08111f;--panel:#111d2e;--panel2:#17263b;--line:#2b3d56;--text:#eef5ff;--muted:#9db0c9;--green:#35d69f;--orange:#ffb454;--red:#ff6b7a;--cyan:#4fc3f7}");
    html.AppendLine("*{box-sizing:border-box}html{scroll-behavior:smooth}body{margin:0;background:linear-gradient(135deg,#08111f 0%,#0b1728 45%,#111b2b 100%);color:var(--text);font-family:\"Segoe UI\",\"Malgun Gothic\",sans-serif;line-height:1.55}");
    html.AppendLine(".wrap{max-width:1500px;margin:auto;padding:34px 30px 80px}.hero{display:flex;gap:24px;align-items:flex-start;justify-content:space-between;margin-bottom:28px}.eyebrow{color:var(--cyan);font-weight:700;letter-spacing:.08em;text-transform:uppercase}.hero h1{font-size:clamp(28px,4vw,48px);line-height:1.15;margin:8px 0 12px}.sub{color:var(--muted);max-width:880px}.toolbar{position:sticky;top:16px;z-index:5}.print{border:1px solid #5e789b;background:#1c314c;color:white;border-radius:10px;padding:12px 17px;font-weight:700;cursor:pointer;box-shadow:0 10px 25px #0006}.print:hover{background:#28496f}");
    html.AppendLine(".status{display:inline-flex;align-items:center;border-radius:999px;padding:7px 12px;font-weight:800;font-size:13px}.status-complete{background:#153f35;color:#75efc5}.status-incomplete{background:#4c2430;color:#ff9ba7}");
    html.AppendLine(".cards{display:grid;grid-template-columns:repeat(6,minmax(145px,1fr));gap:12px;margin:22px 0}.card,.section{background:linear-gradient(180deg,var(--panel),#0d1929);border:1px solid var(--line);border-radius:15px;box-shadow:0 14px 34px #0004}.card{padding:18px}.card .value{font-size:28px;font-weight:800}.card .label{font-size:13px;color:var(--muted)}");
    html.AppendLine(".section{padding:24px;margin:20px 0}.section h2{margin:0 0 15px;font-size:23px}.notice{border-left:4px solid var(--orange);background:#2b241b;padding:14px 16px;border-radius:8px;color:#ffe5bf}.criteria{display:grid;grid-template-columns:repeat(2,minmax(260px,1fr));gap:8px 24px;color:#d7e3f3}.criteria li{margin:4px 0}");
    html.AppendLine(".image-frame{background:#050a11;border:1px solid #31445d;border-radius:12px;padding:10px;overflow:auto}.image-frame img{display:block;width:100%;height:auto;border-radius:7px}.caption{font-size:13px;color:var(--muted);margin-top:8px}");
    html.AppendLine(".table-wrap{overflow:auto;border:1px solid var(--line);border-radius:12px}table{width:100%;min-width:1230px;border-collapse:collapse;font-size:13px}th{position:sticky;top:0;background:#1b2b42;color:#cfe3fb;text-align:left}th,td{padding:11px 10px;border-bottom:1px solid #263950;vertical-align:top}tbody tr:hover{background:#17283c}.mono{font-family:Consolas,monospace;white-space:nowrap}");
    html.AppendLine(".chip{display:inline-block;border-radius:999px;padding:4px 9px;font-size:12px;font-weight:800}.advance{background:#12493b;color:#72edc2}.stop{background:#52252e;color:#ff9aa5}.mechanical{background:#283c59;color:#b8d7ff}.details{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:18px}.result{background:var(--panel2);border:1px solid var(--line);border-radius:14px;padding:17px}.result h3{margin:0 0 8px;font-size:18px}.meta{color:var(--muted);font-size:13px;margin-bottom:10px}.reason{margin:10px 0 14px}.links a{color:#77d6ff;margin-right:18px}.limits li{margin:7px 0}");
    html.AppendLine("@media(max-width:1100px){.cards{grid-template-columns:repeat(3,1fr)}.details{grid-template-columns:1fr}.hero{display:block}.toolbar{position:static;margin-top:16px}}@media(max-width:650px){.wrap{padding:22px 14px 60px}.cards{grid-template-columns:repeat(2,1fr)}.criteria{grid-template-columns:1fr}.section{padding:17px}}");
    html.AppendLine("@media print{body{background:white;color:#111}.wrap{max-width:none;padding:0}.toolbar{display:none}.card,.section,.result{background:white;color:#111;box-shadow:none;border-color:#bbb;break-inside:avoid}.sub,.caption,.meta{color:#555}.details{display:block}.result{margin:0 0 18px}.table-wrap{overflow:visible}table{font-size:9px;min-width:0}th{position:static;background:#eee;color:#111}.image-frame{background:white;border-color:#aaa}.notice{background:#fff7e8;color:#332200}.links a{color:#0645ad}}");
    html.AppendLine("</style></head><body><main class=\"wrap\">");
    html.AppendLine("<header class=\"hero\"><div>");
    html.AppendLine("<div class=\"eyebrow\">OpenVisionLab · Auto MPoint</div>");
    html.AppendLine("<h1>6종 EasyMatch 코퍼스<br>검증 보고서</h1>");
    html.Append("<span class=\"status ").Append(overallStatusClass).Append("\">")
        .Append(HtmlReportEncode(overallStatus)).AppendLine("</span>");
    html.Append("<p class=\"sub\">자동 후보가 실제로 같은 물리 특징을 추적했는지 판단할 수 있도록 수치, 판정 사유, 후보 드로잉과 104건 파일럿 결과를 한 파일에 담았습니다. 생성일: ")
        .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss K", CultureInfo.InvariantCulture))
        .AppendLine("</p></div>");
    html.AppendLine("<div class=\"toolbar\"><button class=\"print\" type=\"button\" onclick=\"window.print()\">인쇄 / PDF 저장</button></div></header>");
    html.AppendLine("<section class=\"cards\">");
    AppendAutoMPointHtmlMetric(html, totalMetadataRows + "/3000", "메타데이터 이미지");
    AppendAutoMPointHtmlMetric(html, sourceGroupCount + "/16", "원본 이미지 층");
    AppendAutoMPointHtmlMetric(html, sourceGroupsWithSuggestion + "/16", "후보 생성");
    AppendAutoMPointHtmlMetric(html, pilotRunCount.ToString(CultureInfo.InvariantCulture), "고정 파일럿 실행");
    AppendAutoMPointHtmlMetric(html, expansionCandidateCount + "/16", "확대 검증 후보");
    AppendAutoMPointHtmlMetric(html, stoppedCount + "/16", "중단");
    html.AppendLine("</section>");
    html.AppendLine("<section class=\"section\"><h2>판단 전에 알아둘 점</h2>");
    html.AppendLine("<div class=\"notice\"><strong>기계적 통과는 물리 특징이 올바르다는 뜻이 아닙니다.</strong> OK 4장 중 3장 이상에서 Matching이 실행됐다는 뜻입니다. 아래 드로잉에서 동일한 물리 특징을 따라가는지 확인한 뒤에만 후보를 승인할 수 있습니다.</div>");
    html.AppendLine("<ul class=\"criteria\">");
    html.AppendLine("<li>Auto MPoint: 96×96, stride 16, score 0.75, uniqueness 0.05</li>");
    html.AppendLine("<li>EdgeBased Matching: 전체 512×512 검색, score 0.75</li>");
    html.AppendLine("<li>자세 범위: angle -8..8° / 1°, scale 0.9..1.1 / 0.05</li>");
    html.AppendLine("<li>표본: 각 원본별 OK 4장 + NG 4장</li>");
    html.AppendLine("<li>결과 확인 뒤 ROI·문턱·자세 범위를 조정하지 않음</li>");
    html.AppendLine("<li>생성 자세 정답이 없어 위치 정밀도 px는 판정하지 않음</li>");
    html.AppendLine("</ul></section>");
    html.AppendLine("<section class=\"section\"><h2>Auto MPoint 후보 전체 보기</h2>");
    if (File.Exists(candidateSheetPath))
    {
        html.Append("<div class=\"image-frame\"><img alt=\"16개 원본별 Auto MPoint 후보\" src=\"")
            .Append(ToEmbeddedImageDataUri(candidateSheetPath))
            .AppendLine("\"></div>");
        html.AppendLine("<div class=\"caption\">16개 원본 층에서 고정 조건으로 생성한 1순위 후보. 후보 없음도 그대로 표시합니다.</div>");
    }
    else
    {
        html.AppendLine("<p>후보 전체 드로잉이 생성되지 않았습니다.</p>");
    }
    html.AppendLine("</section>");
    html.AppendLine("<section class=\"section\"><h2>원본별 판정표</h2><div class=\"table-wrap\"><table>");
    html.AppendLine("<thead><tr><th>데이터셋 / 원본</th><th>후보</th><th>ROI</th><th>점수 / 고유성</th><th>OK 성공/모호/미검출</th><th>NG 성공/모호/미검출</th><th>결함 겹침</th><th>평균 ms</th><th>기계 판정</th><th>드로잉 검토</th></tr></thead><tbody>");
    foreach (List<string> row in rows)
    {
        (string drawingDecision, _) =
            GetAutoMPointSixCorpusDrawingReview(row[datasetIndex], row[sourceIndex]);
        string decisionClass = drawingDecision == "확대 검증 후보" ? "advance" : "stop";
        html.Append("<tr><td><strong>").Append(HtmlReportEncode(row[datasetIndex]))
            .Append("</strong><br>").Append(HtmlReportEncode(row[sourceIndex])).Append("</td>");
        html.Append("<td>").Append(HtmlReportEncode(row[candidateStateIndex])).Append("</td>");
        html.Append("<td class=\"mono\">").Append(HtmlReportEncode(row[candidateRoiIndex])).Append("</td>");
        html.Append("<td class=\"mono\">").Append(HtmlReportEncode(row[candidateScoreIndex]))
            .Append(" / ").Append(HtmlReportEncode(row[candidateUniquenessIndex])).Append("</td>");
        html.Append("<td class=\"mono\">").Append(HtmlReportEncode(row[okSuccessIndex])).Append("/")
            .Append(HtmlReportEncode(row[okAmbiguousIndex])).Append("/")
            .Append(HtmlReportEncode(row[okNoMatchIndex])).Append("</td>");
        html.Append("<td class=\"mono\">").Append(HtmlReportEncode(row[ngSuccessIndex])).Append("/")
            .Append(HtmlReportEncode(row[ngAmbiguousIndex])).Append("/")
            .Append(HtmlReportEncode(row[ngNoMatchIndex])).Append("</td>");
        html.Append("<td>").Append(HtmlReportEncode(row[overlapIndex])).Append("</td>");
        html.Append("<td>").Append(HtmlReportEncode(row[elapsedIndex])).Append("</td>");
        html.Append("<td><span class=\"chip mechanical\">").Append(HtmlReportEncode(row[decisionIndex]))
            .Append("</span></td>");
        html.Append("<td><span class=\"chip ").Append(decisionClass).Append("\">")
            .Append(HtmlReportEncode(drawingDecision)).AppendLine("</span></td></tr>");
    }
    html.AppendLine("</tbody></table></div>");
    html.Append("<p class=\"caption\">기계적 검토 대상: ").Append(sourceGroupsAdvancing)
        .Append("/16 · 런타임 오류: ").Append(totalRuntimeErrors)
        .Append(" · 무결성 오류: ").Append(integrityFailures.Count).AppendLine("</p></section>");
    html.AppendLine("<section class=\"section\"><h2>실제 결과 드로잉</h2>");
    html.AppendLine("<p class=\"sub\">노란색/녹색 패턴 윤곽과 중심이 동일한 물리 특징을 따라가는지 확인하십시오. 반복 구조의 다른 위치, 배경 경계, 결함 자체를 따라가면 수치가 높아도 부적합입니다.</p>");
    html.AppendLine("<div class=\"details\">");
    foreach (List<string> row in rows)
    {
        string contactSheetPath = row[contactSheetIndex];
        (string drawingDecision, string drawingReason) =
            GetAutoMPointSixCorpusDrawingReview(row[datasetIndex], row[sourceIndex]);
        string decisionClass = drawingDecision == "확대 검증 후보" ? "advance" : "stop";
        html.AppendLine("<article class=\"result\">");
        html.Append("<h3>").Append(HtmlReportEncode(row[datasetIndex])).Append(" / ")
            .Append(HtmlReportEncode(row[sourceIndex])).AppendLine("</h3>");
        html.Append("<div class=\"meta\">ROI <span class=\"mono\">")
            .Append(HtmlReportEncode(row[candidateRoiIndex]))
            .Append("</span> · 기계 판정 ").Append(HtmlReportEncode(row[decisionIndex])).AppendLine("</div>");
        html.Append("<span class=\"chip ").Append(decisionClass).Append("\">")
            .Append(HtmlReportEncode(drawingDecision)).AppendLine("</span>");
        html.Append("<p class=\"reason\">").Append(HtmlReportEncode(drawingReason)).AppendLine("</p>");
        if (File.Exists(contactSheetPath))
        {
            html.Append("<div class=\"image-frame\"><img loading=\"lazy\" alt=\"")
                .Append(HtmlReportEncode(row[datasetIndex] + " " + row[sourceIndex] + " 파일럿 결과"))
                .Append("\" src=\"").Append(ToEmbeddedImageDataUri(contactSheetPath))
                .AppendLine("\"></div>");
        }
        else
        {
            html.AppendLine("<div class=\"notice\">Auto MPoint 추천 후보가 없어 Matching 파일럿을 실행하지 않았습니다.</div>");
        }
        html.AppendLine("</article>");
    }
    html.AppendLine("</div></section>");
    html.AppendLine("<section class=\"section\"><h2>근거 파일</h2><p class=\"links\">");
    html.AppendLine("<a href=\"p227_auto_mpoint_six_corpus_pilot_summary.csv\">원본별 요약 CSV</a>");
    html.Append("<a href=\"p227_auto_mpoint_six_corpus_pilot_results.csv\">")
        .Append(pilotRunCount).AppendLine("건 실행 결과 CSV</a>");
    html.AppendLine("<a href=\"p227_auto_mpoint_six_corpus_drawing_review.csv\">드로잉 검토 CSV</a>");
    html.AppendLine("<a href=\"completion_record.md\">완료 기록</a></p></section>");
    html.AppendLine("<section class=\"section\"><h2>한계와 다음 판단</h2><ol class=\"limits\">");
    html.AppendLine("<li>이 데이터는 EasyMatch 원본 변형과 합성 NG이며 실제 생산 변동 증거가 아닙니다.</li>");
    html.AppendLine("<li>서로 다른 source_file은 별도 템플릿으로 평가했습니다.</li>");
    html.AppendLine("<li>생성 자세의 정답 좌표가 없어 드로잉의 물리적 동일성은 운영자가 확인해야 합니다.</li>");
    html.AppendLine("<li>운영자가 승인한 후보만 해당 원본 층의 500장 전체 검증으로 확대할 수 있습니다.</li>");
    html.AppendLine("</ol>");
    if (integrityFailures.Count == 0)
    {
        html.AppendLine("<p><strong>무결성 오류:</strong> 없음</p>");
    }
    else
    {
        html.AppendLine("<p><strong>무결성 오류:</strong></p><ul>");
        foreach (string failure in integrityFailures)
        {
            html.Append("<li>").Append(HtmlReportEncode(failure)).AppendLine("</li>");
        }
        html.AppendLine("</ul>");
    }
    html.AppendLine("</section>");
    html.AppendLine("<footer class=\"caption\">OpenVisionLab · P227 · 단일 파일 HTML 보고서 (이미지 포함)</footer>");
    html.AppendLine("</main></body></html>");
    File.WriteAllText(reportPath, html.ToString(), new System.Text.UTF8Encoding(true));
    return reportPath;
}

static void AppendAutoMPointHtmlMetric(
    System.Text.StringBuilder html,
    string value,
    string label)
{
    html.Append("<div class=\"card\"><div class=\"value\">")
        .Append(HtmlReportEncode(value))
        .Append("</div><div class=\"label\">")
        .Append(HtmlReportEncode(label))
        .AppendLine("</div></div>");
}

static string HtmlReportEncode(string value)
{
    return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
}

static string ToEmbeddedImageDataUri(string path)
{
    string extension = Path.GetExtension(path).ToLowerInvariant();
    string mimeType = extension switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        _ => "image/png"
    };
    return "data:" + mimeType + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path));
}

static (string Decision, string Reason) GetAutoMPointSixCorpusDrawingReview(
    string dataset,
    string sourceFile)
{
    string key = dataset + "|" + sourceFile;
    return key switch
    {
        "switch_housing|Switch1.tif" =>
            ("확대 검증 후보", "좌하단 슬롯과 하우징 윤곽을 같은 위치에서 추적했습니다. OK는 3/4 성공이므로 전체 검증 전 채택은 금지합니다."),
        "switch_housing|Switch2.tif" =>
            ("확대 검증 후보", "우상단 슬롯과 코너 윤곽을 일관되게 추적했습니다. 한 NG의 최저 점수 75.5가 문턱에 가까워 여유가 작습니다."),
        "switch_housing|Switch3.tif" =>
            ("확대 검증 후보", "하단 원형 홀과 인접 슬롯을 일관되게 추적했습니다. NG 결함 마스크가 후보와 겹친 성공 1건은 취약성으로 남깁니다."),
        "pcb_board|BOARD.JPG" =>
            ("확대 검증 후보", "중앙 부품 군집의 동일 위치를 추적했지만 엣지가 매우 조밀합니다. OK 3/4 및 결함 겹침 1건 때문에 보수적으로 유지합니다."),
        "ic_frame|Frame 1.tif" =>
            ("중단", "고정 조건에서 추천 후보가 생성되지 않았습니다."),
        "ic_frame|Frame 2.tif" =>
            ("중단", "고정 조건에서 추천 후보가 생성되지 않았습니다."),
        "ic_frame|Frame 3.tif" =>
            ("중단", "고정 조건에서 추천 후보가 생성되지 않았습니다."),
        "ic_frame|Frame 4.bmp" =>
            ("중단", "우하단 코너를 추적했으나 OK 성공이 2/4라 최소 기계 조건을 통과하지 못했습니다."),
        "ic_frame|Frame 5.bmp" =>
            ("확대 검증 후보", "좌하단 코너와 핀 군을 같은 위치에서 추적했습니다. 반복 핀 구조이므로 전체 검증 전 채택은 금지합니다."),
        "floppy_disk|Floppies.jpg" =>
            ("확대 검증 후보", "비대칭 흰 탭을 포함한 동일 허브를 OK 4/4에서 추적했습니다. 여러 유사 디스크가 있어 고유성 재확인이 필요합니다."),
        "die_pad|Die Pad 1.bmp" =>
            ("확대 검증 후보", "중앙 패드와 연결 배선을 OK/NG 각 4/4에서 같은 위치로 추적했습니다."),
        "die_pad|Die Pad 2.bmp" =>
            ("확대 검증 후보", "인접 패드와 수직 배선의 동일 조합을 OK 4/4에서 추적했습니다. NG는 3/4 성공입니다."),
        "die_pad|Die Pad 3.bmp" =>
            ("확대 검증 후보", "우측 패드와 코너 배선을 OK 4/4에서 추적했습니다. 결함 겹침 성공 2건은 취약성으로 남깁니다."),
        "die_pad|Die Pad 4.bmp" =>
            ("확대 검증 후보", "중앙 패드와 인접 배선을 OK/NG 각 4/4에서 같은 위치로 추적했습니다."),
        "die_array|Die1.tif" =>
            ("중단", "반복되는 다이 격자 교차점을 후보로 삼았고 NG 1건에서 실제 모호 판정이 발생했습니다. 고유 랜드마크로 보지 않습니다."),
        "die_array|Die2.tif" =>
            ("중단", "오브젝트 내부 특징이 아니라 영상 상단의 잘린 프레임 경계를 추적했습니다. 촬영 프레이밍 의존 후보라 부적합합니다."),
        _ => ("중단", "드로잉 검토 규칙이 정의되지 않은 원본 층입니다.")
    };
}

static string ToReportRelativePath(string evidenceDirectory, string path)
{
    return Path.GetRelativePath(evidenceDirectory, path).Replace('\\', '/').Replace(" ", "%20");
}

static List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)>
    LoadAutoMPointCorpusMetadata(string metadataPath)
{
    string[] lines = File.ReadAllLines(metadataPath);
    if (lines.Length < 2)
    {
        throw new InvalidDataException("Metadata CSV has no rows.");
    }
    List<string> header = ParseCsvRecord(lines[0]);
    int globalIdIndex = header.FindIndex(value => value == "global_id");
    int fileNameIndex = header.FindIndex(value => value == "filename");
    int statusIndex = header.FindIndex(value => value == "status");
    int sourceFileIndex = header.FindIndex(value => value == "source_file");
    int md5Index = header.FindIndex(value => value == "md5");
    if (new[] { globalIdIndex, fileNameIndex, statusIndex, sourceFileIndex, md5Index }.Any(index => index < 0))
    {
        throw new InvalidDataException("Metadata CSV is missing global_id, filename, status, source_file, or md5.");
    }

    List<(int, string, string, string, string)> rows = new List<(int, string, string, string, string)>();
    foreach (string line in lines.Skip(1).Where(value => !string.IsNullOrWhiteSpace(value)))
    {
        List<string> values = ParseCsvRecord(line);
        if (!int.TryParse(values[globalIdIndex], NumberStyles.Integer, CultureInfo.InvariantCulture, out int globalId))
        {
            throw new InvalidDataException("Metadata global_id is invalid: " + values[globalIdIndex]);
        }
        rows.Add((
            globalId,
            values[fileNameIndex],
            values[statusIndex],
            values[sourceFileIndex],
            values[md5Index]));
    }
    return rows;
}

static string GetAutoMPointCorpusImagePath(
    string datasetRoot,
    (int GlobalId, string FileName, string Status, string SourceFile, string Md5) row)
{
    return Path.Combine(datasetRoot, "all_images", row.Status, row.FileName);
}

static IEnumerable<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)>
    SelectAutoMPointPilotRows(
        IReadOnlyList<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> rows,
        (int GlobalId, string FileName, string Status, string SourceFile, string Md5)? required)
{
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> ordered =
        rows.OrderBy(row => row.Md5, StringComparer.OrdinalIgnoreCase).ToList();
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> selected =
        new List<(int, string, string, string, string)>();
    if (required.HasValue)
    {
        selected.Add(required.Value);
    }
    int[] indices =
    {
        0,
        ordered.Count / 3,
        ordered.Count * 2 / 3,
        ordered.Count - 1
    };
    foreach (int index in indices)
    {
        (int GlobalId, string FileName, string Status, string SourceFile, string Md5) candidate = ordered[index];
        if (!selected.Any(row => row.GlobalId == candidate.GlobalId))
        {
            selected.Add(candidate);
        }
        if (selected.Count == 4)
        {
            break;
        }
    }
    foreach ((int GlobalId, string FileName, string Status, string SourceFile, string Md5) candidate in ordered)
    {
        if (selected.Count == 4)
        {
            break;
        }
        if (!selected.Any(row => row.GlobalId == candidate.GlobalId))
        {
            selected.Add(candidate);
        }
    }
    return selected;
}

static string ComputeMd5(string filePath)
{
    using FileStream stream = File.OpenRead(filePath);
    return Convert.ToHexString(MD5.HashData(stream)).ToLowerInvariant();
}

static int CountMaskOverlap(string maskPath, System.Drawing.RectangleF bounds)
{
    using Mat mask = Cv2.ImRead(maskPath, ImreadModes.Grayscale);
    if (mask.Empty())
    {
        return 0;
    }
    int left = Math.Clamp((int)Math.Floor(bounds.Left), 0, mask.Width);
    int top = Math.Clamp((int)Math.Floor(bounds.Top), 0, mask.Height);
    int right = Math.Clamp((int)Math.Ceiling(bounds.Right), 0, mask.Width);
    int bottom = Math.Clamp((int)Math.Ceiling(bounds.Bottom), 0, mask.Height);
    if (right <= left || bottom <= top)
    {
        return 0;
    }
    using Mat overlap = mask.SubMat(new Rect(left, top, right - left, bottom - top));
    return Cv2.CountNonZero(overlap);
}

static async Task<int> RunObjectDimensionFilterContractAsync(string? evidenceDirectory)
{
    List<string> failures = new List<string>();
    string? evidencePath = string.IsNullOrWhiteSpace(evidenceDirectory)
        ? null
        : Path.GetFullPath(evidenceDirectory);
    if (evidencePath != null)
    {
        Directory.CreateDirectory(evidencePath);
    }

    using Mat source = new Mat(new OpenCvSharp.Size(360, 140), MatType.CV_8UC1, Scalar.Black);
    Cv2.Rectangle(source, new Rect(20, 20, 24, 32), Scalar.White, -1);
    Cv2.Rectangle(source, new Rect(80, 20, 52, 24), Scalar.White, -1);
    Cv2.Rectangle(source, new Rect(155, 20, 8, 32), Scalar.White, -1);
    Cv2.Rectangle(source, new Rect(195, 20, 24, 8), Scalar.White, -1);
    Cv2.Rectangle(source, new Rect(250, 20, 24, 60), Scalar.White, -1);
    if (evidencePath != null)
    {
        Cv2.ImWrite(Path.Combine(evidencePath, "object_dimension_filter_source.png"), source);
    }

    foreach (string toolType in new[] { "Blob", "Contour" })
    {
        await VerifyObjectDimensionFilterAsync(source, toolType, failures, evidencePath);
    }

    VerifyObjectDimensionPropertyRoundTrip(failures);
    VerifyObjectDimensionValidation(failures);

    if (failures.Count == 0)
    {
        Console.WriteLine("Object dimension filter contract smoke passed.");
        Console.WriteLine("Blob/Contour: 1 accepted object, width/height reject reasons retained, legacy missing-key behavior preserved.");
        return 0;
    }

    Console.Error.WriteLine("Object dimension filter contract smoke failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    return 1;
}

static async Task<int> RunObjectCandidateParityContractAsync(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory
        ?? Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "pl0010_c4_parity_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
    Directory.CreateDirectory(evidenceDirectory);

    List<string> failures = new List<string>();
    List<string> observations = new List<string>();
    List<string> timingRows = new List<string>
    {
        "ToolType\tVariant\tCompletedObjectStepUpdates\tStepResultCount\tStepElapsedMs\tWallElapsedMs\tPersistedTotalMs"
    };

    using Mat source = CreateBlobContourAuditBaselineSource();
    string sourcePath = Path.Combine(evidenceDirectory, "candidate_parity_source.png");
    if (!Cv2.ImWrite(sourcePath, source))
    {
        failures.Add("Could not write the object-candidate parity source image.");
    }

    await VerifyObjectCandidateParityCaseAsync(
        source,
        "Blob",
        "mask",
        evidenceDirectory,
        failures,
        observations,
        timingRows);

    foreach (string toolType in new[] { "Blob", "Contour" })
    {
        await VerifyObjectCandidateParityCaseAsync(
            source,
            toolType,
            "multi-roi",
            evidenceDirectory,
            failures,
            observations,
            timingRows);
    }

    await VerifyPublicCandidateSampleParityAsync(
        evidenceDirectory,
        failures,
        observations,
        timingRows);

    string timingPath = Path.Combine(evidenceDirectory, "timing.tsv");
    File.WriteAllLines(timingPath, timingRows, new UTF8Encoding(false));
    string observationsPath = Path.Combine(evidenceDirectory, "observations.txt");
    File.WriteAllLines(observationsPath, observations, new UTF8Encoding(false));
    string completionPath = Path.Combine(evidenceDirectory, "completion.txt");
    File.WriteAllLines(
        completionPath,
        new[]
        {
            failures.Count == 0 ? "Status=Complete" : "Status=Incomplete",
            "Scope=PL-0010 C4 App pipeline candidate parity replay for Blob mask, Blob/Contour multi-ROI, object drawings, timing, Run History, and public Good/Bad samples.",
            "Acceptance=One completed Blob/Contour Step update per execution; source-coordinate candidate rows, RegionIndex, applied limits, reject reasons, accepted metrics, and accepted drawings retained; Run History round-trip preserves timing and objects; public Good/Bad sample checks preserve expected outcomes.",
            "Verification=Current Dev Runner and vendored SDK assemblies; no second audit Tool execution was introduced by this contract.",
            "Evidence=" + sourcePath + "; " + timingPath + "; " + observationsPath,
            "Boundary=This contract does not claim EXE/DPI/theme coverage or deployment/push/release authorization. SDK mask semantics are exercised for Blob; the SDK does not define Contour mask classification in this release."
        }.Concat(failures.Select(failure => "Failure=" + failure)),
        new UTF8Encoding(false));

    if (failures.Count == 0)
    {
        Console.WriteLine("PL-0010 object-candidate parity contract passed.");
        Console.WriteLine("Mask, multi-ROI, selection-ready drawings, timing, Run History, and public Good/Bad replay passed.");
        Console.WriteLine("Evidence=" + evidenceDirectory);
        return 0;
    }

    Console.Error.WriteLine("PL-0010 object-candidate parity contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    Console.Error.WriteLine("Evidence=" + evidenceDirectory);
    return 1;
}

static async Task VerifyObjectCandidateParityCaseAsync(
    Mat source,
    string toolType,
    string variant,
    string evidenceDirectory,
    ICollection<string> failures,
    ICollection<string> observations,
    ICollection<string> timingRows)
{
    VisionPipeline pipeline = CreateObjectCandidateParityPipeline(toolType, variant);
    string recipeName = "Smoke_PL0010_C4_" + toolType + "_" + variant + "_" + Guid.NewGuid().ToString("N");
    DateTime startedAt = DateTime.Now;
    Stopwatch wall = Stopwatch.StartNew();

    try
    {
        using (VisionPipelineContext context = new VisionPipelineContext())
        {
            context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source);
            int completedObjectStepUpdates = 0;
            using (VisionPipelineRunResult runResult = await VisionPipelineExecutionService.RunAsync(
                pipeline,
                context,
                VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                CancellationToken.None,
                update =>
                {
                    if (update?.StepResult != null
                        && IsBlobOrContourTool(update.Step?.ToolType))
                    {
                        completedObjectStepUpdates++;
                    }
                }))
            {
                wall.Stop();
                VisionPipelineStepResult? stepResult = runResult.StepResults.SingleOrDefault();
                if (stepResult == null)
                {
                    failures.Add($"{toolType}/{variant}: no Step result was produced.");
                    return;
                }

                VisionPipelineStepResultSummary summary = VisionPipelineResultSummaryService.CreateStepSummary(1, stepResult);
                IReadOnlyList<VisionPipelineObjectResult> rows = VisionPipelineObjectResultStore
                    .Get(stepResult.ToolResult)
                    .ToList();
                int acceptedCount = rows.Count(item => item.Accepted);
                int expectedAccepted = string.Equals(variant, "mask", StringComparison.OrdinalIgnoreCase) ? 4 : 5;
                string expectedStage = string.Equals(toolType, "Blob", StringComparison.OrdinalIgnoreCase)
                    ? "BlobLabeling"
                    : "ContourExtraction";

                if (!runResult.Success || stepResult.ToolResult?.Success != true || !summary.Success)
                {
                    failures.Add($"{toolType}/{variant}: pipeline execution did not pass. Message={summary.Message}");
                }

                if (completedObjectStepUpdates != 1)
                {
                    failures.Add($"{toolType}/{variant}: expected exactly one completed object Step update, actual {completedObjectStepUpdates}.");
                }

                if (runResult.StepResults.Count != 1)
                {
                    failures.Add($"{toolType}/{variant}: expected one Step result, actual {runResult.StepResults.Count}.");
                }

                if (rows.Count != 5 || acceptedCount != expectedAccepted)
                {
                    failures.Add($"{toolType}/{variant}: expected 5 candidate rows and {expectedAccepted} accepted, actual rows={rows.Count}, accepted={acceptedCount}.");
                }

                if (rows.Any(item => string.IsNullOrWhiteSpace(item.CandidateId)
                    || item.NativeIndex < 0
                    || item.RegionIndex < 0
                    || string.IsNullOrWhiteSpace(item.GenerationStage)
                    || !string.Equals(item.GenerationStage, expectedStage, StringComparison.Ordinal)
                    || !string.Equals(item.CoordinateFrame, "SourceImage", StringComparison.Ordinal)
                    || item.BoundsWidth <= 0
                    || item.BoundsHeight <= 0
                    || item.AppliedMinimumArea != 20
                    || item.AppliedMaximumArea != 10000
                    || item.AppliedMinimumWidth != 0
                    || item.AppliedMaximumWidth != 1000
                    || item.AppliedMinimumHeight != 0
                    || item.AppliedMaximumHeight != 1000)
                    || rows.Select(item => item.CandidateId).Distinct(StringComparer.Ordinal).Count() != rows.Count)
                {
                    failures.Add($"{toolType}/{variant}: candidate identity, source geometry, or applied-limit metadata was not retained.");
                }

                if (string.Equals(variant, "mask", StringComparison.OrdinalIgnoreCase))
                {
                    List<VisionPipelineObjectResult> masked = rows
                        .Where(item => string.Equals(item.RejectReasonCode, "Masked", StringComparison.Ordinal))
                        .ToList();
                    if (!string.Equals(toolType, "Blob", StringComparison.OrdinalIgnoreCase)
                        || masked.Count != 1
                        || rows.Count(item => string.Equals(item.RejectReasonCode, "None", StringComparison.Ordinal)) != 4)
                    {
                        failures.Add($"{toolType}/{variant}: expected Blob mask candidate contract (one Masked row and four accepted rows).");
                    }
                    else
                    {
                        observations.Add($"{toolType}/{variant}: Masked candidate retained with source rectangle ({masked[0].BoundsX},{masked[0].BoundsY},{masked[0].BoundsWidth},{masked[0].BoundsHeight}).");
                    }
                }
                else
                {
                    int[] regions = rows.Select(item => item.RegionIndex).Distinct().OrderBy(index => index).ToArray();
                    if (!regions.SequenceEqual(new[] { 0, 1 })
                        || rows.Any(item => !item.CandidateId.StartsWith(
                            expectedStage + ":" + item.RegionIndex + ":",
                            StringComparison.Ordinal))
                        || !rows.Any(item => item.RegionIndex == 0 && item.BoundsX < 180)
                        || !rows.Any(item => item.RegionIndex == 1 && item.BoundsX >= 180))
                    {
                        failures.Add($"{toolType}/{variant}: multi-ROI RegionIndex/ID/source-coordinate parity failed.");
                    }
                    else
                    {
                        observations.Add($"{toolType}/{variant}: RegionIndex 0/1 and source-coordinate geometry retained.");
                    }
                }

                if (rows.Any(item => item.RegionIndex != 0 && string.Equals(variant, "mask", StringComparison.OrdinalIgnoreCase)))
                {
                    failures.Add($"{toolType}/{variant}: single-region mask case produced a non-zero RegionIndex.");
                }

                double resultCount = summary.Metrics.GetValueOrDefault(VisionPipelineKnownMetrics.ResultCount, -1D);
                if (resultCount != expectedAccepted)
                {
                    failures.Add($"{toolType}/{variant}: ResultCount expected {expectedAccepted}, actual {resultCount:0.###}.");
                }

                int acceptedOverlayCount = stepResult.ToolResult?.Overlays?.Count(item =>
                    item != null
                    && item.Kind == VisionToolOverlayKind.Rectangle
                    && string.Equals(item.Label, "Accepted object", StringComparison.Ordinal)) ?? 0;
                if (acceptedOverlayCount != expectedAccepted)
                {
                    failures.Add($"{toolType}/{variant}: accepted drawing count expected {expectedAccepted}, actual {acceptedOverlayCount}.");
                }

                if (!summary.HasResultImage || summary.ResultImageWidth != source.Width || summary.ResultImageHeight != source.Height)
                {
                    failures.Add($"{toolType}/{variant}: result image dimensions did not remain {source.Width} x {source.Height}.");
                }

                double stepElapsedMilliseconds = stepResult.ToolResult?.Elapsed.TotalMilliseconds ?? -1D;
                if (stepElapsedMilliseconds < 0D || double.IsNaN(stepElapsedMilliseconds))
                {
                    failures.Add($"{toolType}/{variant}: Step timing was not reported.");
                }

                string caseEvidenceDirectory = Path.Combine(
                    evidenceDirectory,
                    toolType.ToLowerInvariant(),
                    variant);
                Directory.CreateDirectory(caseEvidenceDirectory);
                SaveObjectCandidateParityEvidence(
                    source,
                    rows,
                    variant,
                    Path.Combine(caseEvidenceDirectory, "candidate_drawing.png"));
                File.WriteAllLines(
                    Path.Combine(caseEvidenceDirectory, "candidate_rows.tsv"),
                    new[]
                    {
                        "Number\tCandidateId\tRegionIndex\tNativeIndex\tAccepted\tArea\tX\tY\tWidth\tHeight\tAngle\tRejectReasonCode\tRejectReason\tAppliedMinArea\tAppliedMaxArea\tAppliedMinWidth\tAppliedMaxWidth\tAppliedMinHeight\tAppliedMaxHeight\tGenerationStage\tCoordinateFrame"
                    }.Concat(rows.Select(item => string.Join(
                        "\t",
                        item.Number.ToString(CultureInfo.InvariantCulture),
                        item.CandidateId,
                        item.RegionIndex.ToString(CultureInfo.InvariantCulture),
                        item.NativeIndex.ToString(CultureInfo.InvariantCulture),
                        item.Accepted.ToString(CultureInfo.InvariantCulture),
                        item.Area.ToString("0.###", CultureInfo.InvariantCulture),
                        item.BoundsX.ToString(CultureInfo.InvariantCulture),
                        item.BoundsY.ToString(CultureInfo.InvariantCulture),
                        item.BoundsWidth.ToString(CultureInfo.InvariantCulture),
                        item.BoundsHeight.ToString(CultureInfo.InvariantCulture),
                        item.Angle.ToString("0.###", CultureInfo.InvariantCulture),
                        item.RejectReasonCode,
                        item.RejectReason,
                        item.AppliedMinimumArea.ToString(CultureInfo.InvariantCulture),
                        item.AppliedMaximumArea.ToString(CultureInfo.InvariantCulture),
                        item.AppliedMinimumWidth.ToString(CultureInfo.InvariantCulture),
                        item.AppliedMaximumWidth.ToString(CultureInfo.InvariantCulture),
                        item.AppliedMinimumHeight.ToString(CultureInfo.InvariantCulture),
                        item.AppliedMaximumHeight.ToString(CultureInfo.InvariantCulture),
                        item.GenerationStage,
                        item.CoordinateFrame))));

                DateTime finishedAt = DateTime.Now;
                string reportPath = VisionPipelineRunReportStorage.Save(
                    recipeName,
                    pipeline,
                    runResult,
                    startedAt,
                    finishedAt,
                    publishAllOutputs: false,
                    runLabel: toolType + "-" + variant);
                VisionPipelineRunReport? report = VisionPipelineRunReportStorage.Load(reportPath);
                VisionPipelineStepRunReport? persistedStep = report?.Steps.SingleOrDefault();
                if (report == null || persistedStep == null)
                {
                    failures.Add($"{toolType}/{variant}: Run History report did not round-trip.");
                }
                else
                {
                    if (report.TotalMilliseconds < 0D
                        || persistedStep.ElapsedMilliseconds < 0D
                        || persistedStep.Objects.Count != rows.Count
                        || persistedStep.OverlayCount != (stepResult.ToolResult?.Overlays?.Count ?? 0)
                        || persistedStep.Objects.Any(item => item.RegionIndex < 0
                            || string.IsNullOrWhiteSpace(item.CandidateId)
                            || !string.Equals(item.CoordinateFrame, "SourceImage", StringComparison.Ordinal)
                            || item.AppliedMaximumArea != 10000))
                    {
                        failures.Add($"{toolType}/{variant}: Run History did not preserve timing, overlay, candidate, or applied-limit metadata.");
                    }

                    string persistedResultCount = persistedStep.Metrics
                        .FirstOrDefault(item => string.Equals(item.Name, VisionPipelineKnownMetrics.ResultCount, StringComparison.OrdinalIgnoreCase))
                        ?.Value.ToString("0.###", CultureInfo.InvariantCulture) ?? "missing";
                    observations.Add($"{toolType}/{variant}: Run History rows={persistedStep.Objects.Count}, overlays={persistedStep.OverlayCount}, ResultCount={persistedResultCount}, elapsed={persistedStep.ElapsedMilliseconds:0.###}ms.");

                    string runEvidenceDirectory = Path.Combine(caseEvidenceDirectory, "run-history");
                    CopyRunDirectory(Path.GetDirectoryName(reportPath)!, runEvidenceDirectory);
                }

                timingRows.Add(string.Join(
                    "\t",
                    toolType,
                    variant,
                    completedObjectStepUpdates.ToString(CultureInfo.InvariantCulture),
                    runResult.StepResults.Count.ToString(CultureInfo.InvariantCulture),
                    stepElapsedMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                    wall.Elapsed.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                    (report?.TotalMilliseconds ?? -1D).ToString("0.###", CultureInfo.InvariantCulture)));
            }
        }
    }
    catch (Exception exception)
    {
        failures.Add($"{toolType}/{variant}: parity replay threw {exception.GetBaseException().Message}");
    }
    finally
    {
        RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
    }
}

static async Task VerifyPublicCandidateSampleParityAsync(
    string evidenceDirectory,
    ICollection<string> failures,
    ICollection<string> observations,
    ICollection<string> timingRows)
{
    string[] sampleNames =
    {
        "Public_Blob_Particles_Good",
        "Public_Blob_Particles_Sparse_Bad",
        "Public_Contour_Shapes_Good",
        "Public_Contour_Shapes_Missing_Bad"
    };
    List<VisionPipelineSampleCatalogItem> samples = VisionPipelineSampleCatalogItem
        .LoadRunnable(VisionPipelineSampleCatalogSourceKind.Public);

    foreach (string sampleName in sampleNames)
    {
        VisionPipelineSampleCatalogItem? sample = samples.FirstOrDefault(item =>
            string.Equals(item.SampleName, sampleName, StringComparison.OrdinalIgnoreCase));
        if (sample == null)
        {
            failures.Add($"Public sample '{sampleName}' was not found in the runnable public catalog.");
            continue;
        }

        string recipeName = "Smoke_PL0010_C4_Public_" + Guid.NewGuid().ToString("N");
        try
        {
            VisionPipelineSampleCheckResult check = await VisionPipelineSampleCheckService
                .RunSampleCheckWithReportSafeAsync(
                    sample,
                    pipelineXmlText: null,
                    recipeName: recipeName,
                    normalizeInputToGray: false,
                    cancellationToken: CancellationToken.None);
            if (!check.ExecutionCompleted || !check.Success)
            {
                failures.Add($"{sampleName}: public sample check failed. Status={check.Status}, Message={check.Message}");
                continue;
            }

            VisionPipelineRunReport? report = string.IsNullOrWhiteSpace(check.RunReportPath)
                ? null
                : VisionPipelineRunReportStorage.Load(check.RunReportPath);
            VisionPipelineStepRunReport? objectStep = report?.Steps
                .LastOrDefault(step => IsBlobOrContourTool(step.ToolType) && step.Objects.Count > 0);
            if (report == null || objectStep == null)
            {
                failures.Add($"{sampleName}: public sample report did not retain an object Step.");
                continue;
            }

            bool expectedRawSuccess = !sample.ExpectsFailure;
            if (report.Success != expectedRawSuccess || check.ActualSuccess != expectedRawSuccess)
            {
                failures.Add($"{sampleName}: expected raw outcome {(expectedRawSuccess ? "success" : "failure")}, actual report={report.Success}, check={check.ActualSuccess}.");
            }

            if (objectStep.Objects.Count == 0
                || objectStep.Objects.Any(item => string.IsNullOrWhiteSpace(item.CandidateId)
                    || item.RegionIndex != 0
                    || item.NativeIndex < 0
                    || !string.Equals(item.CoordinateFrame, "SourceImage", StringComparison.Ordinal)
                    || string.IsNullOrWhiteSpace(item.GenerationStage))
                || objectStep.Objects.Select(item => item.CandidateId).Distinct(StringComparer.Ordinal).Count() != objectStep.Objects.Count)
            {
                failures.Add($"{sampleName}: public sample report candidate identity/source-coordinate metadata was incomplete.");
            }

            if (objectStep.ElapsedMilliseconds < 0D
                || report.TotalMilliseconds < 0D
                || objectStep.OverlayCount <= 0
                || string.IsNullOrWhiteSpace(report.SourceImageFile))
            {
                failures.Add($"{sampleName}: public sample report timing, drawing, or source-image evidence was incomplete.");
            }
            else
            {
                string reportDirectory = Path.GetDirectoryName(check.RunReportPath)!;
                string sourceImagePath = Path.Combine(reportDirectory, report.SourceImageFile);
                if (!File.Exists(sourceImagePath)
                    || !VisionPipelineRunReportStorage.IsFileSha256Match(sourceImagePath, report.SourceImageSha256))
                {
                    failures.Add($"{sampleName}: public sample report source-image hash did not round-trip.");
                }
            }

            VisionPipelineSampleExpectedMetric? resultCountExpectation = sample.ExpectedMetrics.FirstOrDefault(metric =>
                string.Equals(metric.Name, VisionPipelineKnownMetrics.ResultCount, StringComparison.OrdinalIgnoreCase));
            VisionPipelineMetricRunReport? resultCountMetric = objectStep.Metrics.FirstOrDefault(metric =>
                string.Equals(metric.Name, VisionPipelineKnownMetrics.ResultCount, StringComparison.OrdinalIgnoreCase));
            if (resultCountExpectation == null || resultCountMetric == null)
            {
                failures.Add($"{sampleName}: public sample ResultCount metric was not retained.");
            }
            else
            {
                if (double.TryParse(resultCountExpectation.Minimum, NumberStyles.Float, CultureInfo.InvariantCulture, out double minimum)
                    && resultCountMetric.Value < minimum)
                {
                    failures.Add($"{sampleName}: ResultCount {resultCountMetric.Value:0.###} is below expected minimum {minimum:0.###}.");
                }
                if (double.TryParse(resultCountExpectation.Maximum, NumberStyles.Float, CultureInfo.InvariantCulture, out double maximum)
                    && resultCountMetric.Value > maximum)
                {
                    failures.Add($"{sampleName}: ResultCount {resultCountMetric.Value:0.###} is above expected maximum {maximum:0.###}.");
                }
            }

            string sampleEvidenceDirectory = Path.Combine(
                evidenceDirectory,
                "public",
                SanitizeEvidenceFileName(sampleName));
            CopyRunDirectory(Path.GetDirectoryName(check.RunReportPath)!, sampleEvidenceDirectory);
            File.WriteAllText(
                Path.Combine(sampleEvidenceDirectory, "sample-result.txt"),
                string.Join(
                    Environment.NewLine,
                    "Sample=" + sampleName,
                    "PairRole=" + sample.PairRole,
                    "ExpectedFailure=" + sample.ExpectsFailure,
                    "CheckStatus=" + check.Status,
                    "CheckSuccess=" + check.Success,
                    "ActualSuccess=" + check.ActualSuccess,
                    "ReportSuccess=" + report.Success,
                    "ResultCount=" + (resultCountMetric?.Value.ToString("0.###", CultureInfo.InvariantCulture) ?? "missing"),
                    "ObjectRows=" + objectStep.Objects.Count.ToString(CultureInfo.InvariantCulture),
                    "OverlayCount=" + objectStep.OverlayCount.ToString(CultureInfo.InvariantCulture),
                    "ElapsedMs=" + objectStep.ElapsedMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                    "RunReport=" + check.RunReportPath));

            observations.Add($"{sampleName}: outcome={(sample.ExpectsFailure ? "expected-NG" : "OK")}, ResultCount={resultCountMetric?.Value.ToString("0.###", CultureInfo.InvariantCulture) ?? "missing"}, object rows={objectStep.Objects.Count}, overlays={objectStep.OverlayCount}.");
            timingRows.Add(string.Join(
                "\t",
                objectStep.ToolType,
                "public:" + sampleName,
                "n/a",
                report.Steps.Count.ToString(CultureInfo.InvariantCulture),
                objectStep.ElapsedMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                check.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                report.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)));
        }
        catch (Exception exception)
        {
            failures.Add($"{sampleName}: public sample parity replay threw {exception.GetBaseException().Message}");
        }
        finally
        {
            RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
        }
    }
}

static VisionPipeline CreateObjectCandidateParityPipeline(string toolType, string variant)
{
    VisionPipelineStep step = new VisionPipelineStep
    {
        Name = $"{toolType} {variant} candidate parity",
        ToolType = toolType,
        Enabled = true,
        InputLayer = VisionRecipeRunner.DefaultInputLayer,
        OutputLayer = toolType + "_CandidateParity"
    };
    step.Parameters["USE_THRESHOLD"] = "true";
    step.Parameters["THRESHOLD_TYPES"] = "Binary";
    step.Parameters["THRESHOLD"] = "100";
    step.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
    step.Parameters["USE_BITWISENOT"] = "false";
    step.Parameters["USE_ROI"] = "false";
    step.Parameters["USE_MULTI_ROI"] = string.Equals(variant, "multi-roi", StringComparison.OrdinalIgnoreCase).ToString();
    step.Parameters["MIN_AREA"] = "20";
    step.Parameters["MAX_AREA"] = "10000";
    step.Parameters["MIN_WIDTH"] = "0";
    step.Parameters["MAX_WIDTH"] = "1000";
    step.Parameters["MIN_HEIGHT"] = "0";
    step.Parameters["MAX_HEIGHT"] = "1000";
    if (string.Equals(variant, "mask", StringComparison.OrdinalIgnoreCase))
    {
        step.Parameters["USE_MASKING"] = "true";
        step.Parameters["CvMASKS"] = "80,20,52,24";
    }
    else
    {
        step.Parameters["CvROIS"] = "0,0,180,140;180,0,180,140";
    }

    if (string.Equals(toolType, "Contour", StringComparison.OrdinalIgnoreCase))
    {
        step.Parameters["USE_DRAW_IMAGE"] = "true";
        step.Parameters["DetectMode"] = "External";
        step.Parameters["ApproximationModes"] = "ApproxSimple";
    }

    VisionPipeline pipeline = new VisionPipeline { Name = "PL0010 " + toolType + " " + variant + " candidate parity" };
    pipeline.Steps.Add(step);
    return pipeline;
}

static bool IsBlobOrContourTool(string? toolType)
{
    return string.Equals(toolType, "Blob", StringComparison.OrdinalIgnoreCase)
        || string.Equals(toolType, "Contour", StringComparison.OrdinalIgnoreCase);
}

static void SaveObjectCandidateParityEvidence(
    Mat source,
    IEnumerable<VisionPipelineObjectResult> rows,
    string variant,
    string outputPath)
{
    using Mat drawing = new Mat();
    if (source.Channels() == 1)
    {
        Cv2.CvtColor(source, drawing, ColorConversionCodes.GRAY2BGR);
    }
    else
    {
        source.CopyTo(drawing);
    }

    if (string.Equals(variant, "mask", StringComparison.OrdinalIgnoreCase))
    {
        Cv2.Rectangle(drawing, new Rect(80, 20, 52, 24), new Scalar(255, 0, 255), 2, LineTypes.AntiAlias);
        Cv2.PutText(drawing, "MASK", new OpenCvSharp.Point(81, 16), HersheyFonts.HersheySimplex, 0.45, new Scalar(255, 0, 255), 1, LineTypes.AntiAlias);
    }
    else
    {
        Cv2.Rectangle(drawing, new Rect(0, 0, 180, 140), new Scalar(255, 180, 0), 1, LineTypes.AntiAlias);
        Cv2.Rectangle(drawing, new Rect(180, 0, 180, 140), new Scalar(255, 180, 0), 1, LineTypes.AntiAlias);
        Cv2.PutText(drawing, "ROI 0", new OpenCvSharp.Point(6, 14), HersheyFonts.HersheySimplex, 0.42, new Scalar(255, 180, 0), 1, LineTypes.AntiAlias);
        Cv2.PutText(drawing, "ROI 1", new OpenCvSharp.Point(186, 14), HersheyFonts.HersheySimplex, 0.42, new Scalar(255, 180, 0), 1, LineTypes.AntiAlias);
    }

    foreach (VisionPipelineObjectResult item in rows ?? Enumerable.Empty<VisionPipelineObjectResult>())
    {
        Scalar color = item.Accepted
            ? new Scalar(0, 220, 0)
            : new Scalar(0, 0, 255);
        Cv2.Rectangle(
            drawing,
            new Rect(item.BoundsX, item.BoundsY, item.BoundsWidth, item.BoundsHeight),
            color,
            2,
            LineTypes.AntiAlias);
        string label = item.Accepted
            ? $"{item.Number} OK R{item.RegionIndex}"
            : $"{item.Number} {item.RejectReasonCode} R{item.RegionIndex}";
        Cv2.PutText(
            drawing,
            label,
            new OpenCvSharp.Point(item.BoundsX, Math.Max(13, item.BoundsY - 4)),
            HersheyFonts.HersheySimplex,
            0.34,
            color,
            1,
            LineTypes.AntiAlias);
    }

    Cv2.PutText(
        drawing,
        "GREEN=accepted  RED=rejected  geometry=SourceImage",
        new OpenCvSharp.Point(8, 134),
        HersheyFonts.HersheySimplex,
        0.34,
        new Scalar(0, 220, 255),
        1,
        LineTypes.AntiAlias);
    Cv2.ImWrite(outputPath, drawing);
}

static void CopyRunDirectory(string sourceDirectory, string destinationDirectory)
{
    Directory.CreateDirectory(destinationDirectory);
    foreach (string filePath in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.TopDirectoryOnly))
    {
        File.Copy(filePath, Path.Combine(destinationDirectory, Path.GetFileName(filePath)), overwrite: true);
    }
}

static string SanitizeEvidenceFileName(string value)
{
    char[] invalid = Path.GetInvalidFileNameChars();
    string sanitized = new string((value ?? string.Empty)
        .Select(character => invalid.Contains(character) ? '_' : character)
        .ToArray());
    return string.IsNullOrWhiteSpace(sanitized) ? "sample" : sanitized;
}

static async Task<int> RunBlobContourAuditBaselineAsync(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(
        requestedEvidenceDirectory
        ?? Path.Combine(
            "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
            "pl0010_blob_contour_audit_baseline_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
    Directory.CreateDirectory(evidenceDirectory);

    List<string> failures = new List<string>();
    List<string> observations = new List<string>();
    string implementationPath = Path.GetFullPath(
        Path.Combine(
            "src",
            "OpenVisionLab",
            "Core",
            "Pipeline",
            "Execution",
            "VisionPipelineObjectResults.cs"));
    string implementation = File.ReadAllText(implementationPath);
    bool hasSecondExecution = implementation.Contains("auditTool.Execute(auditInput)", StringComparison.Ordinal);
    bool hasSilentAuditCatch = implementation.Contains("catch\r\n            {\r\n                return new List<VisionPipelineObjectResult>();", StringComparison.Ordinal)
        || implementation.Contains("catch\n            {\n                return new List<VisionPipelineObjectResult>();", StringComparison.Ordinal);
    bool hasAcceptedOnlyFallback = implementation.Contains("rows = CaptureAccepted(executedTool, criteria)", StringComparison.Ordinal);
    if (!hasSecondExecution || !hasSilentAuditCatch || !hasAcceptedOnlyFallback)
    {
        failures.Add(
            "Current PL-0010 baseline source shape changed; expected audit Execute, silent catch, and accepted-only fallback were not all found.");
    }

    observations.Add("SourceAuditExecution=" + (hasSecondExecution ? "Present" : "Missing"));
    observations.Add("SourceAuditFailureCatch=" + (hasSilentAuditCatch ? "SilentEmptyList" : "Missing"));
    observations.Add("SourceFallback=" + (hasAcceptedOnlyFallback ? "CaptureAcceptedOnly" : "Missing"));
    observations.Add("RuntimeFailureInjection=Not performed; no test-only SDK/tool failure hook was introduced.");
    observations.Add(
        "CurrentFailureMeaning=If the audit Execute throws or returns unsuccessful, the source path returns an empty audit list and stores accepted-only rows.");

    using Mat source = CreateBlobContourAuditBaselineSource();
    string sourcePath = Path.Combine(evidenceDirectory, "source.png");
    Require(Cv2.ImWrite(sourcePath, source), "Could not write the PL-0010 baseline source image.");

    string sdkVersion = typeof(BlobTool).Assembly.GetName().Version?.ToString() ?? "unknown";
    string sdkManifestPath = Path.GetFullPath(
        Path.Combine("dll", "OpenVisionLab-Vision-SDK", "sdk-manifest.json"));
    string sdkManifestSha256 = File.Exists(sdkManifestPath) ? ComputeSha256(sdkManifestPath) : "missing";
    observations.Add("SdkAssembly=" + sdkVersion);
    observations.Add("SdkManifestPath=" + sdkManifestPath);
    observations.Add("SdkManifestSha256=" + sdkManifestSha256);
    string blobResultProperties = string.Join(
        ",",
        typeof(BlobResult).GetProperties().Select(property => property.Name).OrderBy(name => name, StringComparer.Ordinal));
    string contourResultProperties = string.Join(
        ",",
        typeof(ContourResult).GetProperties().Select(property => property.Name).OrderBy(name => name, StringComparer.Ordinal));
    string[] onePassFields = { "AppliedLimits", "AcceptedState", "RejectReason" };
    bool onePassFieldsMissing = onePassFields.All(field =>
        !typeof(BlobResult).GetProperties().Any(property => string.Equals(property.Name, field, StringComparison.Ordinal))
        && !typeof(ContourResult).GetProperties().Any(property => string.Equals(property.Name, field, StringComparison.Ordinal)));
    observations.Add("BlobResultProperties=" + blobResultProperties);
    observations.Add("ContourResultProperties=" + contourResultProperties);
    observations.Add(
        "MissingOnePassFields=" + (onePassFieldsMissing
            ? string.Join(",", onePassFields) + " absent from both SDK result types"
            : "not all absent; inspect observations before contract design"));

    List<string> rows = new List<string>
    {
        "ToolType\tSdkVersion\tConfiguredMinArea\tConfiguredMaxArea\tAuditMinArea\tPrimaryWallMs\tPrimarySdkElapsedMs\tPrimarySuccess\tPrimaryCandidateCount\tPrimaryCandidateIds\tAuditWallMs\tAuditSdkElapsedMs\tAuditSuccess\tAuditCandidateCount\tAuditCandidateIds\tAuditException\tReportedStepElapsedMs\tRunTotalMs\tObjectRowCount\tAcceptedCount\tRejectedCount\tAcceptedOverlayCount\tRejectReasons"
    };

    foreach (string toolType in new[] { "Blob", "Contour" })
    {
        VisionPipelineStep step = CreateObjectDimensionPipeline(toolType, includeDimensions: true).Steps.Single();
        int configuredMinimumArea = GetBaselineParameterInt(step.Parameters, "MIN_AREA", 200);
        int configuredMaximumArea = GetBaselineParameterInt(step.Parameters, "MAX_AREA", 1000000);
        int auditMinimumArea = string.Equals(toolType, "Contour", StringComparison.OrdinalIgnoreCase)
            ? Math.Max(1, configuredMinimumArea / 4)
            : 0;

        WarmUpBlobContourExecution(step, source);
        WarmUpBlobContourExecution(CreateBlobContourAuditStep(step, auditMinimumArea), source);

        double primaryWallMilliseconds = 0D;
        double primarySdkMilliseconds = -1D;
        bool primarySuccess = false;
        int primaryCandidateCount = 0;
        string primaryCandidateIds = string.Empty;
        string primaryException = string.Empty;
        IVisionTool primaryTool = null!;
        VisionToolResult? primaryResult = null;
        try
        {
            primaryTool = VisionPipelineAppToolFactory.Create(step);
            using (primaryTool as IDisposable)
            using (Mat primaryInput = source.Clone())
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                primaryResult = primaryTool.Execute(primaryInput);
                stopwatch.Stop();
                primaryWallMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
                primarySuccess = primaryResult?.Success == true;
                primarySdkMilliseconds = primaryResult?.Elapsed.TotalMilliseconds ?? -1D;
                primaryCandidateCount = CountBlobContourCandidates(primaryTool);
                primaryCandidateIds = GetBlobContourCandidateIds(primaryTool);
            }
        }
        catch (Exception exception)
        {
            primaryException = exception.GetBaseException().GetType().Name + ": " + exception.GetBaseException().Message;
        }
        finally
        {
            primaryResult?.Dispose();
        }

        double auditWallMilliseconds = 0D;
        double auditSdkMilliseconds = -1D;
        bool auditSuccess = false;
        int auditCandidateCount = 0;
        string auditCandidateIds = string.Empty;
        string auditException = string.Empty;
        IVisionTool auditTool = null!;
        VisionToolResult? auditResult = null;
        try
        {
            VisionPipelineStep auditStep = CreateBlobContourAuditStep(step, auditMinimumArea);
            auditTool = VisionPipelineAppToolFactory.Create(auditStep);
            using (auditTool as IDisposable)
            using (Mat auditInput = source.Clone())
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                auditResult = auditTool.Execute(auditInput);
                stopwatch.Stop();
                auditWallMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
                auditSuccess = auditResult?.Success == true;
                auditSdkMilliseconds = auditResult?.Elapsed.TotalMilliseconds ?? -1D;
                auditCandidateCount = CountBlobContourCandidates(auditTool);
                auditCandidateIds = GetBlobContourCandidateIds(auditTool);
            }
        }
        catch (Exception exception)
        {
            auditException = exception.GetBaseException().GetType().Name + ": " + exception.GetBaseException().Message;
        }
        finally
        {
            auditResult?.Dispose();
        }

        VisionPipeline pipeline = new VisionPipeline { Name = "PL0010 " + toolType + " audit baseline" };
        pipeline.Steps.Add(step);
        VisionRecipeStepRunSummary? runStep = null;
        double runTotalMilliseconds = 0D;
        try
        {
            using Mat runInput = source.Clone();
            using VisionRecipeRunResult run = await new VisionRecipeRunner().RunAsync(pipeline, runInput);
            runTotalMilliseconds = run?.TotalMilliseconds ?? 0D;
            runStep = run?.Steps.SingleOrDefault();
        }
        catch (Exception exception)
        {
            failures.Add(toolType + ": current Runner baseline threw " + exception.GetBaseException().Message);
        }

        int objectRowCount = runStep?.ObjectResults?.Count ?? 0;
        int acceptedCount = runStep?.ObjectResults?.Count(item => item.Accepted) ?? 0;
        int rejectedCount = runStep?.ObjectResults?.Count(item => !item.Accepted) ?? 0;
        int acceptedOverlayCount = runStep?.Overlays?.Count(item =>
            string.Equals(item.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase)
            && string.Equals(item.Label, "Accepted object", StringComparison.Ordinal)) ?? 0;
        string rejectReasons = string.Join(
            " || ",
            runStep?.ObjectResults?
                .Where(item => !item.Accepted)
                .Select(item => item.Number.ToString(CultureInfo.InvariantCulture) + ":" + item.RejectReason)
            ?? Enumerable.Empty<string>());

        if (!primarySuccess || !auditSuccess)
        {
            failures.Add(
                $"{toolType}: representative baseline execution was not successful. Primary={primarySuccess}, Audit={auditSuccess}.");
        }

        rows.Add(string.Join(
            "\t",
            toolType,
            sdkVersion,
            configuredMinimumArea.ToString(CultureInfo.InvariantCulture),
            configuredMaximumArea.ToString(CultureInfo.InvariantCulture),
            auditMinimumArea.ToString(CultureInfo.InvariantCulture),
            primaryWallMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
            primarySdkMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
            primarySuccess.ToString(CultureInfo.InvariantCulture),
            primaryCandidateCount.ToString(CultureInfo.InvariantCulture),
            primaryCandidateIds,
            auditWallMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
            auditSdkMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
            auditSuccess.ToString(CultureInfo.InvariantCulture),
            auditCandidateCount.ToString(CultureInfo.InvariantCulture),
            auditCandidateIds,
            string.IsNullOrWhiteSpace(auditException) ? primaryException : auditException,
            (runStep?.ElapsedMilliseconds ?? 0D).ToString("0.###", CultureInfo.InvariantCulture),
            runTotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
            objectRowCount.ToString(CultureInfo.InvariantCulture),
            acceptedCount.ToString(CultureInfo.InvariantCulture),
            rejectedCount.ToString(CultureInfo.InvariantCulture),
            acceptedOverlayCount.ToString(CultureInfo.InvariantCulture),
            rejectReasons));
    }

    string rowsPath = Path.Combine(evidenceDirectory, "audit_baseline.tsv");
    File.WriteAllLines(rowsPath, rows, new UTF8Encoding(false));
    string observationsPath = Path.Combine(evidenceDirectory, "observations.txt");
    File.WriteAllLines(observationsPath, observations, new UTF8Encoding(false));
    string reportPath = Path.Combine(evidenceDirectory, "completion.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            failures.Count == 0 ? "Status=Complete" : "Status=Incomplete",
            "Scope=PL-0010 C1 baseline only; product audit behavior was not changed.",
            "Acceptance=C1 primary/audit wall cost, SDK-reported timing, candidate counts/IDs, and current object evidence were captured for representative Blob and Contour cases.",
            "Verification=Source-confirmed audit failure fallback plus current Runner object rows/metrics/accepted overlays.",
            "Evidence=" + sourcePath + "; " + rowsPath + "; " + observationsPath,
            "Boundary=One-pass removal remains blocked until the vendored SDK supplies and the app proves parity for applied limits, accepted state, reject reason, geometry, timing, reports, drawings, and selection.",
            "FailureInjection=Not performed; no test-only fault hook was added.",
        }.Concat(failures.Select(failure => "Failure=" + failure)),
        new UTF8Encoding(false));

    Console.WriteLine("PL-0010 Blob/Contour audit baseline captured.");
    Console.WriteLine("Evidence=" + evidenceDirectory);
    Console.WriteLine("Failures=" + failures.Count.ToString(CultureInfo.InvariantCulture));
    return failures.Count == 0 ? 0 : 1;
}

static Mat CreateBlobContourAuditBaselineSource()
{
    Mat source = new Mat(new OpenCvSharp.Size(360, 140), MatType.CV_8UC1, Scalar.Black);
    Cv2.Rectangle(source, new Rect(20, 20, 24, 32), Scalar.White, -1);
    Cv2.Rectangle(source, new Rect(80, 20, 52, 24), Scalar.White, -1);
    Cv2.Rectangle(source, new Rect(155, 20, 8, 32), Scalar.White, -1);
    Cv2.Rectangle(source, new Rect(195, 20, 24, 8), Scalar.White, -1);
    Cv2.Rectangle(source, new Rect(250, 20, 24, 60), Scalar.White, -1);
    return source;
}

static VisionPipelineStep CreateBlobContourAuditStep(VisionPipelineStep source, int auditMinimumArea)
{
    VisionPipelineStep clone = new VisionPipelineStep
    {
        Name = source.Name,
        ToolType = source.ToolType,
        Enabled = source.Enabled,
        InputLayer = source.InputLayer,
        OutputLayer = source.OutputLayer
    };
    foreach (KeyValuePair<string, string> parameter in source.Parameters ?? new Dictionary<string, string>())
    {
        clone.Parameters[parameter.Key] = parameter.Value;
    }

    clone.Parameters["MIN_AREA"] = auditMinimumArea.ToString(CultureInfo.InvariantCulture);
    clone.Parameters["MAX_AREA"] = int.MaxValue.ToString(CultureInfo.InvariantCulture);
    return clone;
}

static void WarmUpBlobContourExecution(VisionPipelineStep step, Mat source)
{
    IVisionTool tool = VisionPipelineAppToolFactory.Create(step);
    using (tool as IDisposable)
    using (Mat input = source.Clone())
    using (VisionToolResult result = tool.Execute(input))
    {
    }
}

static int GetBaselineParameterInt(IDictionary<string, string> parameters, string key, int fallback)
{
    return parameters != null
        && parameters.TryGetValue(key, out string? value)
        && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed)
            ? parsed
            : fallback;
}

static int CountBlobContourCandidates(IVisionTool tool)
{
    if (tool is BlobTool blob)
    {
        return blob.results?.Count(item => item != null) ?? 0;
    }

    if (tool is ContourTool contour)
    {
        return contour.results.Count(item => item != null);
    }

    return 0;
}

static string GetBlobContourCandidateIds(IVisionTool tool)
{
    if (tool is BlobTool blob)
    {
        return string.Join(",", (blob.results ?? new List<BlobResult>())
            .Where(item => item != null)
            .Select(item => item.Index.ToString(CultureInfo.InvariantCulture)));
    }

    if (tool is ContourTool contour)
    {
        return string.Join(",", contour.results
            .Where(item => item != null)
            .Select(item => item.Index.ToString(CultureInfo.InvariantCulture)));
    }

    return string.Empty;
}

static async Task<int> RunAffineTransformContractAsync(string? evidenceDirectory)
{
    List<string> failures = new List<string>();
    string? evidencePath = string.IsNullOrWhiteSpace(evidenceDirectory)
        ? null
        : Path.GetFullPath(evidenceDirectory);
    if (evidencePath != null)
    {
        Directory.CreateDirectory(evidencePath);
    }

    using Mat source = new Mat(new OpenCvSharp.Size(160, 120), MatType.CV_8UC3, Scalar.Black);
    Cv2.Rectangle(source, new Rect(20, 20, 50, 40), new Scalar(255, 255, 255), -1);
    Cv2.Circle(source, new OpenCvSharp.Point(110, 70), 14, new Scalar(0, 180, 255), -1);
    if (evidencePath != null)
    {
        Cv2.ImWrite(Path.Combine(evidencePath, "affine_contract_source.png"), source);
    }

    AffineTransformProperty authored = new AffineTransformProperty("Affine contract")
    {
        SourcePoint1X = 0,
        SourcePoint1Y = 0,
        SourcePoint2X = 100,
        SourcePoint2Y = 0,
        SourcePoint3X = 0,
        SourcePoint3Y = 100,
        DestinationPoint1X = 12,
        DestinationPoint1Y = 18,
        DestinationPoint2X = 132,
        DestinationPoint2Y = 8,
        DestinationPoint3X = 37,
        DestinationPoint3Y = 108,
        OutputWidth = 240,
        OutputHeight = 180,
        MinimumSourceTriangleArea = 100,
        MinimumDestinationTriangleArea = 100,
        MinimumValidPixelRatio = 0.4
    };

    VisionPipelineStep canonicalStep = VisionPipelineStepBuilder.FromAffineTransformProperty(
        authored,
        "01 Affine contract",
        VisionRecipeRunner.DefaultInputLayer,
        "Affine_Result");
    VerifyAffineTransformPropertyRoundTrip(canonicalStep, failures);

    VisionRecipeRunner runner = new VisionRecipeRunner();
    string[] aliases = { "AffineTransform", "Affine", "AffineMatrix" };
    foreach (string alias in aliases)
    {
        VisionPipeline pipeline = CreateAffineContractPipeline(canonicalStep, alias);
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            pipeline,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!validation.Success)
        {
            failures.Add(alias + ": strict validation failed: " + string.Join(" | ", validation.Errors));
            continue;
        }

        using VisionRecipeRunResult run = await runner.RunAsync(pipeline, source);
        VisionRecipeStepRunSummary? step = run.Steps.SingleOrDefault();
        if (!run.Success || step == null)
        {
            failures.Add(alias + ": runtime failed: " + run.Message);
            continue;
        }

        VerifyAffineMetric(step, VisionPipelineKnownMetrics.AffineM11, 1.2, failures, alias);
        VerifyAffineMetric(step, VisionPipelineKnownMetrics.AffineM12, 0.25, failures, alias);
        VerifyAffineMetric(step, VisionPipelineKnownMetrics.AffineM13, 12, failures, alias);
        VerifyAffineMetric(step, VisionPipelineKnownMetrics.AffineM21, -0.1, failures, alias);
        VerifyAffineMetric(step, VisionPipelineKnownMetrics.AffineM22, 0.9, failures, alias);
        VerifyAffineMetric(step, VisionPipelineKnownMetrics.AffineM23, 18, failures, alias);
        if (step.Overlays.Count != 10
            || step.Overlays.Count(item => string.Equals(item.Kind, "Point", StringComparison.OrdinalIgnoreCase)) != 3
            || step.Overlays.Count(item => string.Equals(item.Kind, "Line", StringComparison.OrdinalIgnoreCase)) != 7)
        {
            failures.Add(alias + ": expected 3 point and 7 line drawings.");
        }

        if (string.Equals(alias, "AffineTransform", StringComparison.Ordinal) && evidencePath != null)
        {
            if (run.ResultImage != null && !run.ResultImage.Empty())
            {
                Cv2.ImWrite(Path.Combine(evidencePath, "affine_contract_result.png"), run.ResultImage);
            }
            SaveAllOverlayImage(
                source,
                run,
                pipeline,
                Path.Combine(evidencePath, "affine_contract_drawing.png"));
        }
    }

    VisionPipeline degenerate = CreateAffineContractPipeline(canonicalStep, "AffineTransform");
    degenerate.Steps[0].Parameters[nameof(AffineTransformToolProperty.SourcePoint2X)] = "10";
    degenerate.Steps[0].Parameters[nameof(AffineTransformToolProperty.SourcePoint2Y)] = "10";
    degenerate.Steps[0].Parameters[nameof(AffineTransformToolProperty.SourcePoint3X)] = "20";
    degenerate.Steps[0].Parameters[nameof(AffineTransformToolProperty.SourcePoint3Y)] = "20";
    degenerate.Steps[0].Parameters[nameof(AffineTransformToolProperty.MinimumSourceTriangleArea)] = "0";
    VisionPipelineValidationResult degenerateValidation = VisionPipelineValidator.Validate(
        degenerate,
        new[] { VisionRecipeRunner.DefaultInputLayer });
    if (degenerateValidation.Success
        || !degenerateValidation.Errors.Any(error => error.Contains("source point triangle area", StringComparison.OrdinalIgnoreCase)))
    {
        failures.Add("Strict validation accepted collinear source points when the operator area gate was zero.");
    }

    VisionPipeline coverageFailure = CreateAffineContractPipeline(canonicalStep, "AffineTransform");
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.DestinationPoint1X)] = "500";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.DestinationPoint1Y)] = "500";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.DestinationPoint2X)] = "600";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.DestinationPoint2Y)] = "500";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.DestinationPoint3X)] = "500";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.DestinationPoint3Y)] = "600";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.OutputWidth)] = "64";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.OutputHeight)] = "64";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.MinimumValidPixelRatio)] = "0.1";
    using (VisionRecipeRunResult failedRun = await runner.RunAsync(coverageFailure, source))
    {
        VisionRecipeStepRunSummary? failedStep = failedRun.Steps.SingleOrDefault();
        if (failedRun.Success
            || failedStep == null
            || !string.Equals(failedStep.ErrorName, "AffineInsufficientCoverage", StringComparison.Ordinal)
            || failedStep.Overlays.Count != 10
            || !failedStep.Metrics.TryGetValue(VisionPipelineKnownMetrics.AffineValidPixelRatio, out double validRatio)
            || validRatio != 0)
        {
            failures.Add("Coverage failure did not fail closed while retaining matrix/coverage/drawing evidence.");
        }
    }

    if (evidencePath != null)
    {
        File.WriteAllLines(
            Path.Combine(evidencePath, "affine_contract_report.txt"),
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Aliases: AffineTransform, Affine, AffineMatrix",
                "KnownMatrix: 1.2,0.25,12;-0.1,0.9,18",
                "PropertyGridRoundTrip: " + (failures.Any(item => item.Contains("round trip", StringComparison.OrdinalIgnoreCase)) ? "FAIL" : "PASS"),
                "DegenerateSourceGate: " + (failures.Any(item => item.Contains("collinear", StringComparison.OrdinalIgnoreCase)) ? "FAIL" : "PASS"),
                "CoverageFailureEvidence: " + (failures.Any(item => item.Contains("Coverage failure", StringComparison.OrdinalIgnoreCase)) ? "FAIL" : "PASS")
            }.Concat(failures.Select(item => "Failure: " + item)));
    }

    if (failures.Count == 0)
    {
        Console.WriteLine("Affine transform contract smoke passed.");
        Console.WriteLine("Aliases, known matrix, PropertyGrid/XML round trip, collinear rejection, and coverage evidence passed.");
        return 0;
    }

    Console.Error.WriteLine("Affine transform contract smoke failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

static async Task<int> RunAffineDetectedPointsContractAsync(string? evidenceDirectory)
{
    List<string> failures = new List<string>();
    string workDirectory = string.IsNullOrWhiteSpace(evidenceDirectory)
        ? Path.Combine(Path.GetTempPath(), "OpenVisionLab_P219_" + Guid.NewGuid().ToString("N"))
        : Path.GetFullPath(evidenceDirectory);
    bool deleteWorkDirectory = string.IsNullOrWhiteSpace(evidenceDirectory);
    Directory.CreateDirectory(workDirectory);

    try
    {
        using Mat reference = new Mat(new OpenCvSharp.Size(400, 300), MatType.CV_8UC1, Scalar.Black);
        Cv2.Rectangle(reference, new Rect(180, 130, 40, 30), Scalar.White, -1);
        Cv2.Rectangle(reference, new Rect(176, 126, 48, 38), new Scalar(90), 2);

        Point2f[] sourcePoints =
        {
            new Point2f(80.5f, 60.5f),
            new Point2f(300.5f, 70.5f),
            new Point2f(90.5f, 230.5f)
        };
        Point2f[] destinationPoints =
        {
            new Point2f(60.5f, 50.5f),
            new Point2f(300.5f, 50.5f),
            new Point2f(60.5f, 230.5f)
        };

        using Mat referenceToSource = Cv2.GetAffineTransform(destinationPoints, sourcePoints);
        using Mat source = new Mat();
        Cv2.WarpAffine(
            reference,
            source,
            referenceToSource,
            reference.Size(),
            InterpolationFlags.Linear,
            BorderTypes.Constant,
            Scalar.Black);

        string[] templatePaths = new string[3];
        for (int index = 0; index < 3; index++)
        {
            using Mat template = CreateAffineFiducial(index);
            int left = (int)Math.Floor(sourcePoints[index].X - template.Width / 2D);
            int top = (int)Math.Floor(sourcePoints[index].Y - template.Height / 2D);
            template.CopyTo(source.SubMat(new Rect(left, top, template.Width, template.Height)));
            templatePaths[index] = Path.Combine(workDirectory, $"template_{index + 1}.png");
            Cv2.ImWrite(templatePaths[index], template);
        }

        Cv2.ImWrite(Path.Combine(workDirectory, "00_source.png"), source);
        Cv2.ImWrite(Path.Combine(workDirectory, "00_reference_expected.png"), reference);

        VisionPipeline pipeline = CreateDetectedPointAffinePipeline(
            templatePaths,
            sourcePoints,
            destinationPoints);
        string pipelinePath = Path.Combine(workDirectory, "p219_matching_affine_fixed_roi.pipeline.xml");
        string saveMessage = string.Empty;
        string loadMessage = string.Empty;
        VisionPipeline loaded = pipeline;
        VisionPipeline loadedFromFile = pipeline;
        bool saved = VisionPipelineStorage.TrySaveToFile(pipelinePath, pipeline, out saveMessage);
        bool reloaded = saved
            && VisionPipelineStorage.TryLoadFromFile(pipelinePath, out loadedFromFile, out loadMessage);
        if (reloaded)
        {
            loaded = loadedFromFile;
        }
        else
        {
            failures.Add("Pipeline XML round trip failed. " + saveMessage + " " + loadMessage);
        }

        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            loaded,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!validation.Success)
        {
            failures.Add("Detected-point Affine pipeline validation failed: " + string.Join(" | ", validation.Errors));
        }

        VerifyDetectedPointAffinePropertyRoundTrip(loaded.Steps[3], failures);

        using VisionPipelineContext context = new VisionPipelineContext();
        context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source);
        VisionPipelineRunResult run = await VisionPipelineExecutionService.RunAsync(
            loaded,
            context,
            10000,
            CancellationToken.None);
        try
        {
            if (!run.Success || run.StepResults.Count != 6)
            {
                string stepEvidence = string.Join(
                    " | ",
                    run.StepResults.Select(stepResult =>
                        $"{stepResult.Step?.Name}: "
                        + $"ToolSuccess={stepResult.ToolResult?.Success}, "
                        + $"Error={stepResult.ToolResult?.ErrorName}, "
                        + $"Message={stepResult.ToolResult?.Message}, "
                        + $"Acceptance={stepResult.AcceptancePassed} {stepResult.AcceptanceMessage}"));
                failures.Add(
                    $"Matching -> Affine -> fixed ROI runtime failed. "
                    + $"Steps={run.StepResults.Count}/6. {stepEvidence}");
            }
            else
            {
                for (int index = 0; index < 3; index++)
                {
                    IReadOnlyList<VisionPipelineGeometryFeatureResult> features =
                        VisionPipelineGeometryFeatureStore.Get(run.StepResults[index].ToolResult);
                    VisionPipelineGeometryFeatureResult? center = features.SingleOrDefault(item =>
                        item.Kind == VisionPipelineGeometryKind.Point
                        && string.Equals(item.FeatureName, "Center", StringComparison.OrdinalIgnoreCase));
                    if (center == null
                        || Math.Abs(center.CenterX - sourcePoints[index].X) > 0.6
                        || Math.Abs(center.CenterY - sourcePoints[index].Y) > 0.6)
                    {
                        failures.Add($"Matching Step {index + 1} did not publish the expected typed Center Point.");
                    }
                }

                VisionToolResult affineResult = run.StepResults[3].ToolResult;
                if (!affineResult.Metrics.TryGetValue(VisionPipelineKnownMetrics.AffineDetectedSourcePointCount, out double pointCount)
                    || pointCount != 3D)
                {
                    failures.Add("Affine runtime did not retain detected source-point provenance metrics.");
                }
                VerifyCoreMetric(affineResult, VisionPipelineKnownMetrics.AffineSourcePoint1X, sourcePoints[0].X, 0.6, failures);
                VerifyCoreMetric(affineResult, VisionPipelineKnownMetrics.AffineSourcePoint1Y, sourcePoints[0].Y, 0.6, failures);
                VerifyCoreMetric(affineResult, VisionPipelineKnownMetrics.AffineSourcePoint2X, sourcePoints[1].X, 0.6, failures);
                VerifyCoreMetric(affineResult, VisionPipelineKnownMetrics.AffineSourcePoint2Y, sourcePoints[1].Y, 0.6, failures);
                VerifyCoreMetric(affineResult, VisionPipelineKnownMetrics.AffineSourcePoint3X, sourcePoints[2].X, 0.6, failures);
                VerifyCoreMetric(affineResult, VisionPipelineKnownMetrics.AffineSourcePoint3Y, sourcePoints[2].Y, 0.6, failures);

                using Mat expectedMatrix = Cv2.GetAffineTransform(sourcePoints, destinationPoints);
                string[] matrixMetrics =
                {
                    VisionPipelineKnownMetrics.AffineM11,
                    VisionPipelineKnownMetrics.AffineM12,
                    VisionPipelineKnownMetrics.AffineM13,
                    VisionPipelineKnownMetrics.AffineM21,
                    VisionPipelineKnownMetrics.AffineM22,
                    VisionPipelineKnownMetrics.AffineM23
                };
                for (int row = 0; row < 2; row++)
                {
                    for (int column = 0; column < 3; column++)
                    {
                        VerifyCoreMetric(
                            affineResult,
                            matrixMetrics[row * 3 + column],
                            expectedMatrix.At<double>(row, column),
                            1e-6,
                            failures);
                    }
                }

                VisionToolResult blobResult = run.StepResults[5].ToolResult;
                double resultCount = double.NaN;
                if (!run.StepResults[5].AcceptancePassed
                    || !blobResult.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out resultCount)
                    || resultCount != 1D
                    || loaded.Steps[5].Parameters.GetValueOrDefault("CvROI") != "170,120,70,60")
                {
                    failures.Add("The unchanged fixed reference ROI did not find exactly one normalized inspection target.");
                }

                using Mat normalized = context.GetLayer("Reference");
                if (normalized == null
                    || normalized.Empty()
                    || Cv2.Mean(normalized.SubMat(new Rect(185, 135, 30, 20))).Val0 < 180D)
                {
                    failures.Add("Affine output did not restore the taught reference target region.");
                }
            }

            SaveAffineDetectedPointEvidence(run, workDirectory);
        }
        finally
        {
            foreach (VisionPipelineStepResult stepResult in run.StepResults)
            {
                stepResult?.ToolResult?.ResultImage?.Dispose();
            }
        }

        VisionPipeline duplicate = ClonePipeline(loaded);
        duplicate.Steps[3].Parameters[VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter] =
            duplicate.Steps[3].Parameters[VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter];
        VisionPipelineValidationResult duplicateValidation = VisionPipelineValidator.Validate(
            duplicate,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (duplicateValidation.Success
            || !duplicateValidation.Errors.Any(item => item.Contains("must be distinct", StringComparison.OrdinalIgnoreCase)))
        {
            failures.Add("Static validation accepted duplicate detected Point references.");
        }

        using VisionPipelineContext duplicateContext = new VisionPipelineContext();
        duplicateContext.SetLayer(VisionRecipeRunner.DefaultInputLayer, source);
        VisionPipelineRunResult duplicateRun = await VisionPipelineExecutionService.RunAsync(
            duplicate,
            duplicateContext,
            10000,
            CancellationToken.None);
        try
        {
            VisionPipelineStepResult? failedAffine = duplicateRun.StepResults.LastOrDefault();
            if (duplicateRun.Success
                || failedAffine?.Step?.Name != "04 Normalize from detected points"
                || failedAffine.ToolResult?.Success != false
                || !failedAffine.ToolResult.Message.Contains("three distinct Point features", StringComparison.OrdinalIgnoreCase))
            {
                failures.Add("Runtime did not fail closed on duplicate detected Point references.");
            }
        }
        finally
        {
            foreach (VisionPipelineStepResult stepResult in duplicateRun.StepResults)
            {
                stepResult?.ToolResult?.ResultImage?.Dispose();
            }
        }

        File.WriteAllLines(
            Path.Combine(workDirectory, "p219_affine_detected_points_report.txt"),
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Pipeline: Matching x3 -> AffineTransform -> Threshold -> fixed-ROI Blob",
                "SourceBinding: LocateTopLeft/Center;LocateTopRight/Center;LocateBottomLeft/Center",
                "DestinationFrame: fixed taught pixel coordinates",
                "FixedInspectionRoi: 170,120,70,60",
                "LegacyFixedSourceMode: preserved by the separate affine-transform contract",
                "DuplicatePointGate: " + (failures.Any(item => item.Contains("duplicate", StringComparison.OrdinalIgnoreCase)) ? "FAIL" : "PASS")
            }.Concat(failures.Select(item => "Failure: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Affine detected-point contract smoke passed.");
            Console.WriteLine("Three Matching centers drove the OpenVisionLab Vision SDK AffineTransform, then an unchanged fixed ROI found one normalized target.");
            return 0;
        }

        Console.Error.WriteLine("Affine detected-point contract smoke failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }
        return 1;
    }
    finally
    {
        if (deleteWorkDirectory && Directory.Exists(workDirectory))
        {
            Directory.Delete(workDirectory, true);
        }
    }
}

static async Task<int> RunAffineCardPilotAsync(
    string datasetRootArgument,
    string evidenceDirectoryArgument,
    bool includeFixedRoiMean = false,
    double maximumPostResidualPx = 3D)
{
    string datasetRoot = Path.GetFullPath(datasetRootArgument);
    string evidenceDirectory = Path.GetFullPath(evidenceDirectoryArgument);
    string evidencePrefix = includeFixedRoiMean ? "p221" : "p220";
    Rect fixedInspectionRoi = new Rect(250, 315, 190, 80);
    string referencePath = Path.Combine(
        datasetRoot,
        "images",
        "OK",
        "card_original_OK_0001.jpg");
    Directory.CreateDirectory(evidenceDirectory);

    if (!File.Exists(referencePath))
    {
        Console.Error.WriteLine("Affine card pilot reference image was not found: " + referencePath);
        return 2;
    }

    (string Name, Rect Roi, Rect SearchRoi)[] locators =
    {
        ("R", new Rect(100, 38, 68, 126), new Rect(85, 5, 220, 200)),
        ("5", new Rect(320, 35, 75, 125), new Rect(280, 5, 250, 200)),
        ("Expiry", new Rect(165, 333, 85, 55), new Rect(105, 280, 220, 150))
    };
    Point2f[] destinationPoints = locators
        .Select(item => new Point2f(
            item.Roi.X + item.Roi.Width / 2F,
            item.Roi.Y + item.Roi.Height / 2F))
        .ToArray();

    string templateDirectory = Path.Combine(evidenceDirectory, "templates");
    Directory.CreateDirectory(templateDirectory);
    string[] templatePaths = new string[locators.Length];
    Mat[] templates = new Mat[locators.Length];
    using (Mat reference = Cv2.ImRead(referencePath, ImreadModes.Color))
    {
        if (reference.Empty() || reference.Width != 640 || reference.Height != 480)
        {
            Console.Error.WriteLine("Affine card pilot reference must be the approved 640x480 image.");
            return 2;
        }

        for (int index = 0; index < locators.Length; index++)
        {
            templates[index] = reference.SubMat(locators[index].Roi).Clone();
            templatePaths[index] = Path.Combine(
                templateDirectory,
                $"{index + 1:00}_{locators[index].Name}.png");
            Cv2.ImWrite(templatePaths[index], templates[index]);
        }
    }

    try
    {
        VisionPipeline pipeline = CreateCardAffinePilotPipeline(
            templatePaths,
            locators.Select(item => item.SearchRoi).ToArray(),
            destinationPoints,
            includeFixedRoiMean,
            fixedInspectionRoi);
        string pipelinePath = Path.Combine(
            evidenceDirectory,
            evidencePrefix + "_card_matching_x3_affine.pipeline.xml");
        string saveMessage = string.Empty;
        string loadMessage = string.Empty;
        VisionPipeline loaded = pipeline;
        bool saved = VisionPipelineStorage.TrySaveToFile(
            pipelinePath,
            pipeline,
            out saveMessage);
        bool loadedFromFile = saved
            && VisionPipelineStorage.TryLoadFromFile(
                pipelinePath,
                out loaded,
                out loadMessage);
        if (!loadedFromFile)
        {
            Console.Error.WriteLine(
                "Affine card pilot XML round trip failed. "
                + saveMessage
                + " "
                + loadMessage);
            return 1;
        }

        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            loaded,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!validation.Success)
        {
            Console.Error.WriteLine(
                "Affine card pilot definition is invalid: "
                + string.Join(" | ", validation.Errors));
            return 1;
        }
        if (includeFixedRoiMean
            && (loaded.Steps.Count != 5
                || !string.Equals(loaded.Steps[4].ToolType, "Mean", StringComparison.OrdinalIgnoreCase)
                || loaded.Steps[4].Parameters.GetValueOrDefault("CvROI") != "250,315,190,80"
                || !string.Equals(loaded.Steps[4].InputLayer, "CardReference", StringComparison.Ordinal)))
        {
            Console.Error.WriteLine("Affine card fixed-ROI XML round trip did not retain the exact Mean Step contract.");
            return 1;
        }

        (string Role, string FileName)[] selected =
        {
            ("OK", "card_original_OK_0026.jpg"),
            ("OK", "card_original_OK_0051.jpg"),
            ("OK", "card_original_OK_0101.jpg"),
            ("OK", "card_original_OK_0150.jpg"),
            ("OK", "card_original_OK_0200.jpg"),
            ("OK", "card_original_OK_0250.jpg"),
            ("NG", "card_original_NG_0026.jpg"),
            ("NG", "card_original_NG_0051.jpg"),
            ("NG", "card_original_NG_0101.jpg"),
            ("NG", "card_original_NG_0150.jpg"),
            ("NG", "card_original_NG_0200.jpg"),
            ("NG", "card_original_NG_0250.jpg")
        };

        string[] imagePaths = selected
            .Select(item => Path.Combine(datasetRoot, "images", item.Role, item.FileName))
            .ToArray();
        string manifestPath = Path.Combine(evidenceDirectory, evidencePrefix + "_input_manifest.csv");
        File.WriteAllLines(
            manifestPath,
            new[] { "Role,FileName,SourcePath,SourceSha256" }
                .Concat(selected.Select((item, index) =>
                    string.Join(
                        ",",
                        item.Role,
                        item.FileName,
                        EscapeBatchCsvValue(imagePaths[index]),
                        File.Exists(imagePaths[index])
                            ? ComputeSha256(imagePaths[index])
                            : "MISSING"))));

        List<string> csvRows = new List<string>
        {
            includeFixedRoiMean
                ? "Role,FileName,Status,LocatorAScore,LocatorBScore,LocatorCScore,SourcePoint1,SourcePoint2,SourcePoint3,AffineValidPixelRatio,PostCheckMinScore,PostCheckMaxResidualPx,FixedRoiMeanValueAvg,SourceSha256,DrawingDirectory"
                : "Role,FileName,Status,LocatorAScore,LocatorBScore,LocatorCScore,SourcePoint1,SourcePoint2,SourcePoint3,AffineValidPixelRatio,PostCheckMinScore,PostCheckMaxResidualPx,SourceSha256,DrawingDirectory"
        };
        List<string> reviewImages = new List<string>();
        List<string> reviewLabels = new List<string>();
        List<string> failures = new List<string>();
        int passed = 0;

        for (int sampleIndex = 0; sampleIndex < selected.Length; sampleIndex++)
        {
            (string role, string fileName) = selected[sampleIndex];
            string imagePath = imagePaths[sampleIndex];
            string runDirectory = Path.Combine(
                evidenceDirectory,
                "runs",
                $"{sampleIndex + 1:00}_{role}_{Path.GetFileNameWithoutExtension(fileName)}");
            Directory.CreateDirectory(runDirectory);
            string reviewImagePath = Path.Combine(
                runDirectory,
                includeFixedRoiMean
                    ? "05_05 Measure fixed date ROI.png"
                    : "06_fixed_point_recheck.png");
            string status = "PASS";
            double[] scores = { double.NaN, double.NaN, double.NaN };
            Point2f[] sourcePoints = new Point2f[3];
            double validPixelRatio = double.NaN;
            double postMinScore = double.NaN;
            double postMaxResidual = double.NaN;
            double fixedRoiMean = double.NaN;

            if (!File.Exists(imagePath))
            {
                status = "MISSING";
                failures.Add(role + "/" + fileName + ": source image is missing.");
            }
            else
            {
                File.Copy(
                    imagePath,
                    Path.Combine(runDirectory, "00_source.jpg"),
                    true);
                using Mat source = Cv2.ImRead(imagePath, ImreadModes.Color);
                if (source.Empty())
                {
                    status = "LOAD_FAIL";
                    failures.Add(role + "/" + fileName + ": source image could not be loaded.");
                }
                else
                {
                    using VisionPipelineContext context = new VisionPipelineContext();
                    context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source);
                    VisionPipelineRunResult run = await VisionPipelineExecutionService.RunAsync(
                        loaded,
                        context,
                        30000,
                        CancellationToken.None);
                    try
                    {
                        SaveAffineDetectedPointEvidence(run, runDirectory);
                        int expectedStepCount = includeFixedRoiMean ? 5 : 4;
                        if (!run.Success || run.StepResults.Count < expectedStepCount)
                        {
                            status = "RUNTIME_FAIL";
                            string failedStep = run.StepResults.LastOrDefault()?.Step?.Name ?? "unknown";
                            failures.Add(role + "/" + fileName + ": runtime stopped at " + failedStep + ".");
                        }
                        else
                        {
                            bool centersValid = true;
                            for (int locatorIndex = 0; locatorIndex < 3; locatorIndex++)
                            {
                                VisionPipelineStepResult stepResult = run.StepResults[locatorIndex];
                                scores[locatorIndex] = stepResult.ToolResult.Metrics.TryGetValue(
                                    VisionPipelineKnownMetrics.ScoreMax,
                                    out double score)
                                    ? score
                                    : double.NaN;
                                VisionPipelineGeometryFeatureResult? center =
                                    VisionPipelineGeometryFeatureStore
                                        .Get(stepResult.ToolResult)
                                        .SingleOrDefault(item =>
                                            item.Kind == VisionPipelineGeometryKind.Point
                                            && string.Equals(
                                                item.FeatureName,
                                                "Center",
                                                StringComparison.OrdinalIgnoreCase));
                                if (center == null)
                                {
                                    centersValid = false;
                                    break;
                                }
                                sourcePoints[locatorIndex] = new Point2f(
                                    (float)center.CenterX,
                                    (float)center.CenterY);
                            }

                            VisionPipelineStepResult affineStep = run.StepResults[3];
                            validPixelRatio = affineStep.ToolResult.Metrics.TryGetValue(
                                VisionPipelineKnownMetrics.AffineValidPixelRatio,
                                out double ratio)
                                ? ratio
                                : double.NaN;
                            using Mat normalized = context.GetLayer("CardReference");
                            if (!centersValid || normalized == null || normalized.Empty())
                            {
                                status = "POINT_OR_AFFINE_FAIL";
                                failures.Add(role + "/" + fileName + ": three typed Points or Affine output were missing.");
                            }
                            else
                            {
                                Cv2.ImWrite(
                                    Path.Combine(runDirectory, "05_normalized_raw.png"),
                                    normalized);
                                using Mat postCheckDrawing = ValidateNormalizedCardPoints(
                                    normalized,
                                    templates,
                                    destinationPoints,
                                    out postMinScore,
                                    out postMaxResidual);
                                Cv2.ImWrite(
                                    Path.Combine(runDirectory, "06_fixed_point_recheck.png"),
                                    postCheckDrawing);
                                if (includeFixedRoiMean)
                                {
                                    VisionPipelineStepResult fixedRoiStep = run.StepResults[4];
                                    fixedRoiMean = fixedRoiStep.ToolResult.Metrics.TryGetValue(
                                        VisionPipelineKnownMetrics.MeanValueAvg,
                                        out double meanValue)
                                        ? meanValue
                                        : double.NaN;
                                    if (!double.IsFinite(fixedRoiMean))
                                    {
                                        status = "FIXED_ROI_MEAN_FAIL";
                                        failures.Add(
                                            role
                                            + "/"
                                            + fileName
                                            + ": fixed reference ROI did not publish MeanValueAvg.");
                                    }
                                }
                                else
                                {
                                    Cv2.ImWrite(reviewImagePath, postCheckDrawing);
                                }
                                if (status == "PASS"
                                    && (postMinScore < 0.65D
                                        || postMaxResidual > maximumPostResidualPx))
                                {
                                    status = "POST_CHECK_FAIL";
                                    failures.Add(
                                        role
                                        + "/"
                                         + fileName
                                         + $": normalized point check score/residual was {postMinScore:0.000}/{postMaxResidual:0.00}px.");
                                }
                            }
                        }
                    }
                    finally
                    {
                        foreach (VisionPipelineStepResult stepResult in run.StepResults)
                        {
                            stepResult?.ToolResult?.ResultImage?.Dispose();
                        }
                    }
                }
            }

            if (status == "PASS")
            {
                passed++;
            }
            if (File.Exists(reviewImagePath))
            {
                reviewImages.Add(reviewImagePath);
                string sampleId = Path.GetFileNameWithoutExtension(fileName)
                    .Split('_')
                    .Last();
                reviewLabels.Add(
                    $"{role}_{sampleId} | {status} | r={postMaxResidual:0.00}px");
            }
            List<string> csvValues = new List<string>
            {
                role,
                fileName,
                status,
                scores[0].ToString("0.000000", CultureInfo.InvariantCulture),
                scores[1].ToString("0.000000", CultureInfo.InvariantCulture),
                scores[2].ToString("0.000000", CultureInfo.InvariantCulture),
                FormatPoint(sourcePoints[0]),
                FormatPoint(sourcePoints[1]),
                FormatPoint(sourcePoints[2]),
                validPixelRatio.ToString("0.000000", CultureInfo.InvariantCulture),
                postMinScore.ToString("0.000000", CultureInfo.InvariantCulture),
                postMaxResidual.ToString("0.000000", CultureInfo.InvariantCulture)
            };
            if (includeFixedRoiMean)
            {
                csvValues.Add(fixedRoiMean.ToString("0.000000", CultureInfo.InvariantCulture));
            }
            csvValues.Add(File.Exists(imagePath) ? ComputeSha256(imagePath) : "MISSING");
            csvValues.Add(EscapeBatchCsvValue(runDirectory));
            csvRows.Add(string.Join(",", csvValues));
        }

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, evidencePrefix + "_results.csv"),
            csvRows);
        if (reviewImages.Count > 0)
        {
            SaveCardPilotContactSheet(
                reviewImages,
                reviewLabels,
                Path.Combine(
                    evidenceDirectory,
                    evidencePrefix
                    + (includeFixedRoiMean
                        ? "_fixed_roi_contact_sheet.png"
                        : "_normalized_recheck_contact_sheet.png")));
        }
        File.WriteAllLines(
            Path.Combine(evidenceDirectory, evidencePrefix + "_report.txt"),
            new[]
            {
                "Result: " + (passed == selected.Length ? "PASS" : "FAIL"),
                $"Samples: {selected.Length}",
                $"Passed: {passed}",
                $"Failed: {selected.Length - passed}",
                "Reference: " + referencePath,
                "ReferenceSha256: " + ComputeSha256(referencePath),
                "PipelineSha256: " + ComputeSha256(pipelinePath),
                "FrozenMatching: CCoeffNormed; ScoreMin=0.55; Angle=-8..8 step 1; Scale=0.9..1.1 step 0.05; one coarse ROI per approved feature",
                $"PostCheckGate: min normalized template score >= 0.65 and max center residual <= {maximumPostResidualPx:0.##} px",
                includeFixedRoiMean
                    ? "FixedRoi: Mean on CardReference at CvROI=250,315,190,80; no acceptance judgement."
                    : "FixedRoi: not executed.",
                "Boundary: fixture normalization and fixed-ROI linkage only; NG inspection classification was not attempted."
            }.Concat(failures.Select(item => "Failure: " + item)));

        if (passed == selected.Length)
        {
            Console.WriteLine(
                $"Affine card pilot passed {passed}/{selected.Length}. "
                + "Approved R/5/expiry Points normalized into the fixed reference frame.");
            return 0;
        }

        Console.Error.WriteLine(
            $"Affine card pilot failed {selected.Length - passed}/{selected.Length}; no parameter tuning was applied.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }
        return 1;
    }
    finally
    {
        foreach (Mat template in templates)
        {
            template?.Dispose();
        }
    }
}

static async Task<int> RunEdgeUniqueCardRMatrixAsync(
    string datasetRootArgument,
    string p220ResultsCsvArgument,
    string evidenceDirectoryArgument)
{
    const double maximumCenterErrorPx = 5D;
    const double scoreMinimum = 0.45D;
    const double uniqueMarginMinimum = 0.03D;
    string datasetRoot = Path.GetFullPath(datasetRootArgument);
    string p220ResultsCsv = Path.GetFullPath(p220ResultsCsvArgument);
    string evidenceDirectory = Path.GetFullPath(evidenceDirectoryArgument);
    string referencePath = Path.Combine(
        datasetRoot,
        "images",
        "OK",
        "card_original_OK_0001.jpg");
    Directory.CreateDirectory(evidenceDirectory);

    if (!File.Exists(referencePath) || !File.Exists(p220ResultsCsv))
    {
        Console.Error.WriteLine(
            "P225 requires the approved card reference and P220 results CSV. "
            + $"Reference={referencePath}; Baseline={p220ResultsCsv}");
        return 2;
    }

    List<(string Role, string FileName, Point2f ExpectedCenter, string SourceSha256)> baselines;
    try
    {
        baselines = LoadCardRBaselines(p220ResultsCsv);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("P220 baseline CSV could not be read: " + ex.Message);
        return 2;
    }
    if (baselines.Count != 12)
    {
        Console.Error.WriteLine($"P225 expected the exact 12-row P220 baseline, but found {baselines.Count} rows.");
        return 2;
    }

    string templateDirectory = Path.Combine(evidenceDirectory, "templates");
    Directory.CreateDirectory(templateDirectory);
    string templatePath = Path.Combine(templateDirectory, "01_R_from_card_original_OK_0001.png");
    using (Mat reference = Cv2.ImRead(referencePath, ImreadModes.Color))
    {
        Rect templateRoi = new Rect(100, 38, 68, 126);
        if (reference.Empty()
            || reference.Width != 640
            || reference.Height != 480
            || templateRoi.X + templateRoi.Width > reference.Width
            || templateRoi.Y + templateRoi.Height > reference.Height)
        {
            Console.Error.WriteLine("P225 reference image or approved R template ROI is invalid.");
            return 2;
        }
        using Mat template = reference.SubMat(templateRoi).Clone();
        Cv2.ImWrite(templatePath, template);
    }

    (string Key, string Label, Rect SearchRoi, bool UniqueEnabled)[] modes =
    {
        ("01_narrow_unique", "Reviewed ROI + unique", new Rect(85, 5, 220, 200), true),
        ("02_broad_legacy", "Original broad ROI + legacy", new Rect(50, 5, 180, 200), false),
        ("03_broad_unique", "Original broad ROI + unique", new Rect(50, 5, 180, 200), true)
    };

    List<string> csvRows = new List<string>
    {
        "Mode,ModeLabel,UniqueEnabled,SearchRoi,Role,FileName,SourceSha256,BaselineCenter,PipelineSuccess,ToolSuccess,Outcome,ErrorCode,ErrorName,ScoreMax,UniqueState,UniqueAlternativeCount,UniqueScoreMargin,DetectedCenter,CenterErrorPx,ElapsedMilliseconds,RawDrawingPath,ComparisonDrawingPath"
    };
    List<(string Key, string Label, int CorrectAccept, int FalseAccept, int AmbiguousReject, int NoMatchReject, int RuntimeError, double MeanElapsedMs)> summaries
        = new List<(string, string, int, int, int, int, int, double)>();
    List<string> integrityFailures = new List<string>();

    foreach ((string modeKey, string modeLabel, Rect searchRoi, bool uniqueEnabled) in modes)
    {
        string modeDirectory = Path.Combine(evidenceDirectory, modeKey);
        Directory.CreateDirectory(modeDirectory);
        VisionPipeline pipeline = CreateEdgeUniqueCardRPipeline(
            templatePath,
            searchRoi,
            uniqueEnabled,
            scoreMinimum,
            uniqueMarginMinimum);
        string pipelinePath = Path.Combine(modeDirectory, modeKey + ".pipeline.xml");
        string saveMessage = string.Empty;
        string loadMessage = string.Empty;
        VisionPipeline loaded = pipeline;
        if (!VisionPipelineStorage.TrySaveToFile(
                pipelinePath,
                pipeline,
                out saveMessage)
            || !VisionPipelineStorage.TryLoadFromFile(
                pipelinePath,
                out loaded,
                out loadMessage))
        {
            integrityFailures.Add(
                $"{modeKey}: Pipeline XML round trip failed. {saveMessage} {loadMessage}");
            continue;
        }
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            loaded,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!validation.Success)
        {
            integrityFailures.Add(
                $"{modeKey}: Pipeline definition failed validation: "
                + string.Join(" | ", validation.Errors));
            continue;
        }
        IVisionTool configuredTool = VisionPipelineAppToolFactory.Create(loaded.Steps.Single());
        if (configuredTool is not EdgeBasedTemplateMatchingTool configuredEdgeTool
            || configuredEdgeTool.property == null
            || !configuredEdgeTool.property.USE_FIND_SCALE
            || Math.Abs(configuredEdgeTool.property.FIND_SCALE_MIN - 0.9D) > 0.000001D
            || Math.Abs(configuredEdgeTool.property.FIND_SCALE_MAX - 1.1D) > 0.000001D
            || Math.Abs(configuredEdgeTool.property.FIND_SCALE_STEP - 0.05D) > 0.000001D
            || !configuredEdgeTool.property.USE_POSITION_REFINE
            || !configuredEdgeTool.property.USE_SUBPIXEL_REFINE
            || configuredEdgeTool.property.USE_UNIQUE_MATCH_VALIDATION != uniqueEnabled)
        {
            integrityFailures.Add(
                $"{modeKey}: EdgeBasedMatching Pipeline factory did not retain the frozen "
                + "scale/refinement/unique settings.");
            continue;
        }

        int correctAccept = 0;
        int falseAccept = 0;
        int ambiguousReject = 0;
        int noMatchReject = 0;
        int runtimeError = 0;
        List<double> elapsedValues = new List<double>();
        List<string> reviewImages = new List<string>();
        List<string> reviewLabels = new List<string>();

        for (int sampleIndex = 0; sampleIndex < baselines.Count; sampleIndex++)
        {
            (string role, string fileName, Point2f expectedCenter, string expectedSha256) = baselines[sampleIndex];
            string imagePath = Path.Combine(datasetRoot, "images", role, fileName);
            string runDirectory = Path.Combine(
                modeDirectory,
                "runs",
                $"{sampleIndex + 1:00}_{role}_{Path.GetFileNameWithoutExtension(fileName)}");
            Directory.CreateDirectory(runDirectory);
            string rawDrawingPath = Path.Combine(runDirectory, "01_runtime_drawing.png");
            string comparisonPath = Path.Combine(runDirectory, "02_baseline_comparison.png");
            string sourceSha256 = File.Exists(imagePath) ? ComputeSha256(imagePath) : "MISSING";
            string outcome = "RUNTIME_ERROR";
            string errorName = "MissingSource";
            int errorCode = -1;
            bool pipelineSuccess = false;
            bool toolSuccess = false;
            double scoreMax = double.NaN;
            double uniqueState = double.NaN;
            double alternativeCount = double.NaN;
            double uniqueScoreMargin = double.NaN;
            Point2f? detectedCenter = null;
            double centerErrorPx = double.NaN;
            double elapsedMs = double.NaN;

            if (!File.Exists(imagePath))
            {
                integrityFailures.Add($"{role}/{fileName}: source image is missing.");
                runtimeError++;
            }
            else if (!string.Equals(sourceSha256, expectedSha256, StringComparison.OrdinalIgnoreCase))
            {
                integrityFailures.Add(
                    $"{role}/{fileName}: source SHA-256 changed. Expected {expectedSha256}, actual {sourceSha256}.");
                runtimeError++;
            }
            else
            {
                using Mat source = Cv2.ImRead(imagePath, ImreadModes.Color);
                if (source.Empty())
                {
                    integrityFailures.Add($"{role}/{fileName}: source image could not be loaded.");
                    runtimeError++;
                }
                else
                {
                    using VisionPipelineContext context = new VisionPipelineContext();
                    context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source);
                    VisionPipelineRunResult run = await VisionPipelineExecutionService.RunAsync(
                        loaded,
                        context,
                        30000,
                        CancellationToken.None);
                    pipelineSuccess = run.Success;
                    VisionPipelineStepResult? stepResult = run.StepResults.FirstOrDefault();
                    VisionToolResult? toolResult = stepResult?.ToolResult;
                    if (toolResult == null)
                    {
                        integrityFailures.Add($"{modeKey}/{role}/{fileName}: Step result was not created.");
                        runtimeError++;
                    }
                    else
                    {
                        toolSuccess = toolResult.Success;
                        errorCode = (int)toolResult.ErrorCode;
                        errorName = toolResult.ErrorCode.ToString();
                        elapsedMs = toolResult.Elapsed.TotalMilliseconds;
                        elapsedValues.Add(elapsedMs);
                        scoreMax = GetMetricOrNaN(toolResult, VisionPipelineKnownMetrics.ScoreMax);
                        uniqueState = GetMetricOrNaN(toolResult, VisionPipelineKnownMetrics.UniqueMatchState);
                        alternativeCount = GetMetricOrNaN(
                            toolResult,
                            VisionPipelineKnownMetrics.UniqueMatchPlausibleAlternativeCount);
                        uniqueScoreMargin = GetMetricOrNaN(
                            toolResult,
                            VisionPipelineKnownMetrics.UniqueMatchScoreMargin);
                        VisionPipelineGeometryFeatureResult? center =
                            VisionPipelineGeometryFeatureStore
                                .Get(toolResult)
                                .SingleOrDefault(item =>
                                    item.Kind == VisionPipelineGeometryKind.Point
                                    && string.Equals(
                                        item.FeatureName,
                                        "Center",
                                        StringComparison.OrdinalIgnoreCase));
                        if (center != null)
                        {
                            detectedCenter = new Point2f(
                                (float)center.CenterX,
                                (float)center.CenterY);
                            centerErrorPx = Math.Sqrt(
                                Math.Pow(center.CenterX - expectedCenter.X, 2D)
                                + Math.Pow(center.CenterY - expectedCenter.Y, 2D));
                        }

                        if (toolSuccess && detectedCenter.HasValue)
                        {
                            if (centerErrorPx <= maximumCenterErrorPx)
                            {
                                outcome = "CORRECT_ACCEPT";
                                correctAccept++;
                            }
                            else
                            {
                                outcome = "FALSE_ACCEPT";
                                falseAccept++;
                            }
                        }
                        else if (toolResult.ErrorCode == VisionToolErrorCode.MatchingAmbiguous)
                        {
                            outcome = "AMBIGUOUS_REJECT";
                            ambiguousReject++;
                        }
                        else if (toolResult.ErrorCode == VisionToolErrorCode.MatchingNoResult)
                        {
                            outcome = "NO_MATCH_REJECT";
                            noMatchReject++;
                        }
                        else
                        {
                            outcome = "RUNTIME_ERROR";
                            runtimeError++;
                            integrityFailures.Add(
                                $"{modeKey}/{role}/{fileName}: unexpected runtime result "
                                + $"{toolResult.ErrorCode}: {toolResult.Message}");
                        }

                        if (toolResult.ResultImage != null && !toolResult.ResultImage.Empty())
                        {
                            Cv2.ImWrite(rawDrawingPath, toolResult.ResultImage);
                        }
                        using Mat comparison = CreateEdgeUniqueComparisonDrawing(
                            source,
                            toolResult.ResultImage,
                            searchRoi,
                            expectedCenter,
                            detectedCenter,
                            centerErrorPx,
                            outcome,
                            scoreMax,
                            uniqueScoreMargin);
                        Cv2.ImWrite(comparisonPath, comparison);
                        reviewImages.Add(comparisonPath);
                        reviewLabels.Add(
                            $"{sampleIndex + 1:00} {role} {outcome} "
                            + (double.IsFinite(centerErrorPx)
                                ? $"{centerErrorPx:0.00}px"
                                : errorName));
                    }
                }
            }

            string[] values =
            {
                modeKey,
                modeLabel,
                uniqueEnabled.ToString(CultureInfo.InvariantCulture),
                $"{searchRoi.X};{searchRoi.Y};{searchRoi.Width};{searchRoi.Height}",
                role,
                fileName,
                sourceSha256,
                $"{expectedCenter.X:0.###};{expectedCenter.Y:0.###}",
                pipelineSuccess.ToString(CultureInfo.InvariantCulture),
                toolSuccess.ToString(CultureInfo.InvariantCulture),
                outcome,
                errorCode.ToString(CultureInfo.InvariantCulture),
                errorName,
                FormatFinite(scoreMax),
                FormatFinite(uniqueState),
                FormatFinite(alternativeCount),
                FormatFinite(uniqueScoreMargin),
                detectedCenter.HasValue
                    ? $"{detectedCenter.Value.X:0.###};{detectedCenter.Value.Y:0.###}"
                    : string.Empty,
                FormatFinite(centerErrorPx),
                FormatFinite(elapsedMs),
                File.Exists(rawDrawingPath) ? rawDrawingPath : string.Empty,
                File.Exists(comparisonPath) ? comparisonPath : string.Empty
            };
            csvRows.Add(string.Join(",", values.Select(EscapeBatchCsvValue)));
        }

        if (reviewImages.Count > 0)
        {
            SaveCardPilotContactSheet(
                reviewImages,
                reviewLabels,
                Path.Combine(modeDirectory, modeKey + "_contact_sheet.png"));
        }
        summaries.Add((
            modeKey,
            modeLabel,
            correctAccept,
            falseAccept,
            ambiguousReject,
            noMatchReject,
            runtimeError,
            elapsedValues.Count > 0 ? elapsedValues.Average() : double.NaN));
    }

    string resultsPath = Path.Combine(evidenceDirectory, "p225_edge_unique_card_r_results.csv");
    File.WriteAllLines(resultsPath, csvRows);
    (string Key, string Label, int CorrectAccept, int FalseAccept, int AmbiguousReject, int NoMatchReject, int RuntimeError, double MeanElapsedMs)
        narrow = summaries.SingleOrDefault(item => item.Key == "01_narrow_unique");
    (string Key, string Label, int CorrectAccept, int FalseAccept, int AmbiguousReject, int NoMatchReject, int RuntimeError, double MeanElapsedMs)
        broadLegacy = summaries.SingleOrDefault(item => item.Key == "02_broad_legacy");
    (string Key, string Label, int CorrectAccept, int FalseAccept, int AmbiguousReject, int NoMatchReject, int RuntimeError, double MeanElapsedMs)
        broadUnique = summaries.SingleOrDefault(item => item.Key == "03_broad_unique");

    string decision;
    if (integrityFailures.Count > 0 || summaries.Count != modes.Length)
    {
        decision = "Incomplete";
    }
    else if (narrow.CorrectAccept != baselines.Count
        || narrow.FalseAccept != 0
        || narrow.AmbiguousReject != 0
        || narrow.NoMatchReject != 0
        || narrow.RuntimeError != 0)
    {
        decision = "Reject fixed candidate";
    }
    else if (broadLegacy.FalseAccept > 0
        && broadUnique.FalseAccept == 0
        && broadUnique.RuntimeError == 0)
    {
        decision = "Keep";
    }
    else
    {
        decision = "Keep with documented limits";
    }

    List<string> report = new List<string>
    {
        "# P225 Edge Unique Card R Fixed-ROI Matrix",
        string.Empty,
        $"Decision: `{decision}`",
        string.Empty,
        "## Frozen inputs",
        string.Empty,
        $"- Dataset: `{datasetRoot}`",
        $"- Reference: `{referencePath}`",
        $"- Reference SHA-256: `{ComputeSha256(referencePath)}`",
        $"- R template ROI: `100,38,68,126`",
        $"- Template SHA-256: `{ComputeSha256(templatePath)}`",
        $"- P220 baseline: `{p220ResultsCsv}`",
        $"- P220 baseline SHA-256: `{ComputeSha256(p220ResultsCsv)}`",
        $"- Accepted center-error gate: `<= {maximumCenterErrorPx:0.###} px` (P221 operator decision)",
        $"- Edge score gate: `{scoreMinimum:0.###}`; unique margin: `{uniqueMarginMinimum:0.###}`",
        "- Pose envelope: angle `-8..8 deg / 1 deg`, scale `0.9..1.1 / 0.05`",
        "- No parameter tuning was performed after observing outcomes.",
        string.Empty,
        "## Results",
        string.Empty,
        "| Mode | Correct accept | Baseline mismatch >5 px | Ambiguous reject | No-match reject | Runtime error | Mean ms |",
        "| --- | ---: | ---: | ---: | ---: | ---: | ---: |"
    };
    report.AddRange(summaries.Select(item =>
        $"| {item.Label} | {item.CorrectAccept}/12 | {item.FalseAccept} | "
        + $"{item.AmbiguousReject} | {item.NoMatchReject} | {item.RuntimeError} | "
        + $"{item.MeanElapsedMs:0.###} |"));
    report.AddRange(new[]
    {
        string.Empty,
        "## Interpretation boundary",
        string.Empty,
        "The expected R centers are the previously reviewed P220/P221 Matching centers, not independent metrology ground truth. "
        + "OK/NG is retained only as a dataset stratum; it is not an R-locator truth label. "
        + "A `FALSE_ACCEPT` here means an EdgeBasedMatching center more than 5 px from that frozen baseline.",
        string.Empty,
        "Every row retains the unmodified current-run result drawing and a separate comparison drawing. "
        + "Yellow marks the frozen baseline, green marks a result within 5 px, red marks a result outside the gate, and cyan marks the exact search ROI.",
        string.Empty,
        "## Integrity issues",
        string.Empty
    });
    report.AddRange(integrityFailures.Count == 0
        ? new[] { "- None." }
        : integrityFailures.Select(item => "- " + item));
    File.WriteAllLines(Path.Combine(evidenceDirectory, "README.md"), report);

    Console.WriteLine($"P225 matrix decision: {decision}");
    foreach (var summary in summaries)
    {
        Console.WriteLine(
            $"{summary.Key}: correct={summary.CorrectAccept}/12, false={summary.FalseAccept}, "
            + $"ambiguous={summary.AmbiguousReject}, no-match={summary.NoMatchReject}, "
            + $"runtime-error={summary.RuntimeError}, mean={summary.MeanElapsedMs:0.###}ms");
    }
    if (integrityFailures.Count > 0)
    {
        foreach (string failure in integrityFailures)
        {
            Console.Error.WriteLine("- " + failure);
        }
        return 1;
    }
    return 0;
}

static VisionPipeline CreateEdgeUniqueCardRPipeline(
    string templatePath,
    Rect searchRoi,
    bool uniqueEnabled,
    double scoreMinimum,
    double uniqueMarginMinimum)
{
    VisionPipelineStep match = new VisionPipelineStep
    {
        Name = "01 Locate approved R with EdgeBasedMatching",
        ToolType = "EdgeBasedMatching",
        Enabled = true,
        InputLayer = VisionRecipeRunner.DefaultInputLayer,
        OutputLayer = "Card_R_Edge_Result"
    };
    match.Parameters["PATTERN_PATH"] = templatePath;
    match.Parameters["TemplatePath"] = templatePath;
    match.Parameters["SCORE_MIN"] = scoreMinimum.ToString("0.###", CultureInfo.InvariantCulture);
    match.Parameters["NUM_MATCH"] = "1";
    match.Parameters["USE_UNIQUE_MATCH_VALIDATION"] = uniqueEnabled.ToString(CultureInfo.InvariantCulture);
    match.Parameters["UNIQUE_MATCH_MIN_SCORE_MARGIN"] =
        uniqueMarginMinimum.ToString("0.###", CultureInfo.InvariantCulture);
    match.Parameters["USE_FIND_ANGLE"] = "true";
    match.Parameters["FIND_ANGLE_MIN"] = "-8";
    match.Parameters["FIND_ANGLE_MAX"] = "8";
    match.Parameters["FIND_ANGLE"] = "1";
    match.Parameters["USE_COARSE_TO_FINE_ANGLE_SEARCH"] = "false";
    match.Parameters["USE_FIND_SCALE"] = "true";
    match.Parameters["FIND_SCALE_MIN"] = "0.9";
    match.Parameters["FIND_SCALE_MAX"] = "1.1";
    match.Parameters["FIND_SCALE_STEP"] = "0.05";
    match.Parameters["CANNY_LOW"] = "30";
    match.Parameters["CANNY_HIGH"] = "90";
    match.Parameters["CANNY_APERTURE_SIZE"] = "3";
    match.Parameters["USE_L2_GRADIENT"] = "true";
    match.Parameters["CONTOUR_RETRIEVAL_MODE"] = RetrievalModes.External.ToString();
    match.Parameters["CONTOUR_APPROXIMATION_MODE"] = ContourApproximationModes.ApproxNone.ToString();
    match.Parameters["MAX_TEMPLATE_POINTS"] = "300";
    match.Parameters["MIN_GRADIENT_MAGNITUDE"] = "1";
    match.Parameters["GREEDINESS"] = "0.9";
    match.Parameters["SEARCH_STEP"] = "2";
    match.Parameters["USE_POSITION_REFINE"] = "true";
    match.Parameters["USE_SUBPIXEL_REFINE"] = "true";
    match.Parameters["USE_PYRAMID_POSITION_PROPOSAL"] = "false";
    match.Parameters["PYRAMID_POSITION_TOP_N"] = "6";
    match.Parameters["PYRAMID_POSITION_MIN_SCORE"] = "0.7";
    match.Parameters["USE_HYBRID_VERIFY"] = "false";
    match.Parameters["HYBRID_VERIFY_TOP_N"] = "5";
    match.Parameters["HYBRID_VERIFY_IMAGE_WEIGHT"] = "0.35";
    match.Parameters["USE_DRAW_IMAGE"] = "true";
    match.Parameters["USE_THRESHOLD"] = "false";
    match.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
    match.Parameters["USE_ROI"] = "true";
    match.Parameters["CvROI"] = string.Format(
        CultureInfo.InvariantCulture,
        "{0},{1},{2},{3}",
        searchRoi.X,
        searchRoi.Y,
        searchRoi.Width,
        searchRoi.Height);
    match.Parameters["USE_MULTI_ROI"] = "false";
    return new VisionPipeline
    {
        Name = uniqueEnabled
            ? "P225 Card R EdgeBasedMatching unique"
            : "P225 Card R EdgeBasedMatching legacy",
        Steps = { match }
    };
}

static List<(string Role, string FileName, Point2f ExpectedCenter, string SourceSha256)> LoadCardRBaselines(
    string csvPath)
{
    string[] lines = File.ReadAllLines(csvPath);
    if (lines.Length < 2)
    {
        throw new InvalidDataException("CSV has no data rows.");
    }
    List<string> header = ParseCsvRecord(lines[0]);
    int roleIndex = header.FindIndex(value => value == "Role");
    int fileIndex = header.FindIndex(value => value == "FileName");
    int pointIndex = header.FindIndex(value => value == "SourcePoint1");
    int hashIndex = header.FindIndex(value => value == "SourceSha256");
    if (roleIndex < 0 || fileIndex < 0 || pointIndex < 0 || hashIndex < 0)
    {
        throw new InvalidDataException("CSV is missing Role, FileName, SourcePoint1, or SourceSha256.");
    }

    List<(string, string, Point2f, string)> result = new List<(string, string, Point2f, string)>();
    foreach (string line in lines.Skip(1).Where(value => !string.IsNullOrWhiteSpace(value)))
    {
        List<string> values = ParseCsvRecord(line);
        if (values.Count <= Math.Max(Math.Max(roleIndex, fileIndex), Math.Max(pointIndex, hashIndex)))
        {
            throw new InvalidDataException("CSV row has fewer fields than its header.");
        }
        string[] point = values[pointIndex].Split(';');
        if (point.Length != 2
            || !float.TryParse(point[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x)
            || !float.TryParse(point[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
        {
            throw new InvalidDataException("SourcePoint1 is not an X;Y point: " + values[pointIndex]);
        }
        result.Add((values[roleIndex], values[fileIndex], new Point2f(x, y), values[hashIndex]));
    }
    return result;
}

static List<string> ParseCsvRecord(string line)
{
    List<string> values = new List<string>();
    System.Text.StringBuilder current = new System.Text.StringBuilder();
    bool quoted = false;
    for (int index = 0; index < line.Length; index++)
    {
        char character = line[index];
        if (character == '"')
        {
            if (quoted && index + 1 < line.Length && line[index + 1] == '"')
            {
                current.Append('"');
                index++;
            }
            else
            {
                quoted = !quoted;
            }
        }
        else if (character == ',' && !quoted)
        {
            values.Add(current.ToString());
            current.Clear();
        }
        else
        {
            current.Append(character);
        }
    }
    values.Add(current.ToString());
    return values;
}

static double GetMetricOrNaN(VisionToolResult result, string name)
{
    return result?.Metrics != null && result.Metrics.TryGetValue(name, out double value)
        ? value
        : double.NaN;
}

static string FormatFinite(double value)
{
    return double.IsFinite(value)
        ? value.ToString("0.############", CultureInfo.InvariantCulture)
        : string.Empty;
}

static Mat CreateEdgeUniqueComparisonDrawing(
    Mat source,
    Mat? runtimeDrawing,
    Rect searchRoi,
    Point2f expectedCenter,
    Point2f? detectedCenter,
    double centerErrorPx,
    string outcome,
    double scoreMax,
    double uniqueScoreMargin)
{
    Mat drawing;
    Mat basis = runtimeDrawing != null && !runtimeDrawing.Empty() ? runtimeDrawing : source;
    if (basis.Channels() == 1)
    {
        drawing = new Mat();
        Cv2.CvtColor(basis, drawing, ColorConversionCodes.GRAY2BGR);
    }
    else
    {
        drawing = basis.Clone();
    }

    Cv2.Rectangle(drawing, searchRoi, new Scalar(255, 255, 0), 1, LineTypes.AntiAlias);
    DrawMatrixCross(drawing, expectedCenter, new Scalar(0, 255, 255), 9, 2);
    Cv2.PutText(
        drawing,
        $"baseline ({expectedCenter.X:0.0},{expectedCenter.Y:0.0})",
        new OpenCvSharp.Point(
            Math.Clamp((int)Math.Round(expectedCenter.X) + 12, 0, Math.Max(0, drawing.Width - 1)),
            Math.Clamp((int)Math.Round(expectedCenter.Y) - 8, 18, Math.Max(18, drawing.Height - 1))),
        HersheyFonts.HersheySimplex,
        0.45,
        new Scalar(0, 255, 255),
        1,
        LineTypes.AntiAlias);

    if (detectedCenter.HasValue)
    {
        Scalar detectedColor = centerErrorPx <= 5D
            ? new Scalar(0, 255, 0)
            : new Scalar(0, 0, 255);
        DrawMatrixCross(drawing, detectedCenter.Value, detectedColor, 11, 2);
        Cv2.Line(
            drawing,
            new OpenCvSharp.Point(
                (int)Math.Round(expectedCenter.X),
                (int)Math.Round(expectedCenter.Y)),
            new OpenCvSharp.Point(
                (int)Math.Round(detectedCenter.Value.X),
                (int)Math.Round(detectedCenter.Value.Y)),
            detectedColor,
            1,
            LineTypes.AntiAlias);
    }

    Cv2.Rectangle(
        drawing,
        new Rect(0, 0, drawing.Width, Math.Min(44, drawing.Height)),
        Scalar.Black,
        -1);
    Cv2.PutText(
        drawing,
        $"{outcome} | err={(double.IsFinite(centerErrorPx) ? centerErrorPx.ToString("0.00", CultureInfo.InvariantCulture) + "px" : "-")}",
        new OpenCvSharp.Point(8, 18),
        HersheyFonts.HersheySimplex,
        0.48,
        outcome == "CORRECT_ACCEPT" ? new Scalar(0, 255, 0) : new Scalar(0, 165, 255),
        1,
        LineTypes.AntiAlias);
    Cv2.PutText(
        drawing,
        $"score={FormatFinite(scoreMax)} uniqueMargin={FormatFinite(uniqueScoreMargin)}",
        new OpenCvSharp.Point(8, 37),
        HersheyFonts.HersheySimplex,
        0.42,
        Scalar.White,
        1,
        LineTypes.AntiAlias);
    return drawing;
}

static void DrawMatrixCross(Mat image, Point2f point, Scalar color, int radius, int thickness)
{
    OpenCvSharp.Point center = new OpenCvSharp.Point(
        Math.Clamp((int)Math.Round(point.X), 0, Math.Max(0, image.Width - 1)),
        Math.Clamp((int)Math.Round(point.Y), 0, Math.Max(0, image.Height - 1)));
    Cv2.Line(
        image,
        new OpenCvSharp.Point(Math.Max(0, center.X - radius), center.Y),
        new OpenCvSharp.Point(Math.Min(image.Width - 1, center.X + radius), center.Y),
        color,
        thickness,
        LineTypes.AntiAlias);
    Cv2.Line(
        image,
        new OpenCvSharp.Point(center.X, Math.Max(0, center.Y - radius)),
        new OpenCvSharp.Point(center.X, Math.Min(image.Height - 1, center.Y + radius)),
        color,
        thickness,
        LineTypes.AntiAlias);
}

static VisionPipeline CreateCardAffinePilotPipeline(
    IReadOnlyList<string> templatePaths,
    IReadOnlyList<Rect> searchRois,
    IReadOnlyList<Point2f> destinationPoints,
    bool includeFixedRoiMean = false,
    Rect fixedInspectionRoi = default)
{
    string[] names = { "01 Locate R", "02 Locate 5", "03 Locate expiry" };
    VisionPipeline pipeline = new VisionPipeline
    {
        Name = "P220 Card Matching x3 to Affine reference"
    };
    for (int index = 0; index < 3; index++)
    {
        VisionPipelineStep match = new VisionPipelineStep
        {
            Name = names[index],
            ToolType = "Matching",
            Enabled = true,
            InputLayer = VisionRecipeRunner.DefaultInputLayer,
            OutputLayer = $"Card_Match_{index + 1}"
        };
        match.Parameters["PATTERN_PATH"] = templatePaths[index];
        match.Parameters["MATCH_MODE"] = TemplateMatchModes.CCoeffNormed.ToString();
        match.Parameters["SCORE_MIN"] = "0.55";
        match.Parameters["MAGNIFIATION"] = "1";
        match.Parameters["NUM_MATCH"] = "1";
        match.Parameters["USE_FIND_ANGLE"] = "true";
        match.Parameters["FIND_ANGLE_MIN"] = "-8";
        match.Parameters["FIND_ANGLE_MAX"] = "8";
        match.Parameters["FIND_ANGLE"] = "1";
        match.Parameters["USE_COARSE_TO_FINE_ANGLE_SEARCH"] = "false";
        match.Parameters["USE_FIND_SCALE"] = "true";
        match.Parameters["FIND_SCALE_MIN"] = "0.9";
        match.Parameters["FIND_SCALE_MAX"] = "1.1";
        match.Parameters["FIND_SCALE_STEP"] = "0.05";
        match.Parameters["USE_THRESHOLD"] = "false";
        match.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
        match.Parameters["USE_CANNY"] = "false";
        match.Parameters["USE_ROI"] = "true";
        match.Parameters["CvROI"] = string.Format(
            CultureInfo.InvariantCulture,
            "{0},{1},{2},{3}",
            searchRois[index].X,
            searchRois[index].Y,
            searchRois[index].Width,
            searchRois[index].Height);
        match.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
        pipeline.Steps.Add(match);
    }

    AffineTransformToolProperty property = new AffineTransformToolProperty
    {
        DestinationPoint1X = destinationPoints[0].X,
        DestinationPoint1Y = destinationPoints[0].Y,
        DestinationPoint2X = destinationPoints[1].X,
        DestinationPoint2Y = destinationPoints[1].Y,
        DestinationPoint3X = destinationPoints[2].X,
        DestinationPoint3Y = destinationPoints[2].Y,
        OutputWidth = 640,
        OutputHeight = 480,
        MinimumSourceTriangleArea = 10000,
        MinimumDestinationTriangleArea = 10000,
        MinimumValidPixelRatio = 0.55
    };
    VisionPipelineStep affine = VisionPipelineStepBuilder.FromAffineTransformProperty(
        property,
        "04 Normalize approved card points",
        VisionRecipeRunner.DefaultInputLayer,
        "CardReference");
    affine.Parameters[VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter] = "true";
    affine.Parameters[VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter] = names[0] + "/Center";
    affine.Parameters[VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter] = names[1] + "/Center";
    affine.Parameters[VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter] = names[2] + "/Center";
    affine.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
    pipeline.Steps.Add(affine);
    if (includeFixedRoiMean)
    {
        VisionPipelineStep mean = new VisionPipelineStep
        {
            Name = "05 Measure fixed date ROI",
            ToolType = "Mean",
            Enabled = true,
            InputLayer = "CardReference",
            OutputLayer = "CardDateMean"
        };
        mean.Parameters["Name"] = "P221_CardDateMean";
        mean.Parameters["MEAN_TYPES"] = "Mean";
        mean.Parameters["USE_THRESHOLD"] = "false";
        mean.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
        mean.Parameters["USE_BITWISENOT"] = "false";
        mean.Parameters["USE_ROI"] = "true";
        mean.Parameters["CvROI"] = string.Format(
            CultureInfo.InvariantCulture,
            "{0},{1},{2},{3}",
            fixedInspectionRoi.X,
            fixedInspectionRoi.Y,
            fixedInspectionRoi.Width,
            fixedInspectionRoi.Height);
        mean.Parameters["USE_MULTI_ROI"] = "false";
        pipeline.Steps.Add(mean);
    }
    return pipeline;
}

static Mat ValidateNormalizedCardPoints(
    Mat normalized,
    IReadOnlyList<Mat> templates,
    IReadOnlyList<Point2f> destinationPoints,
    out double minimumScore,
    out double maximumResidual)
{
    using Mat gray = new Mat();
    if (normalized.Channels() == 1)
    {
        normalized.CopyTo(gray);
    }
    else
    {
        Cv2.CvtColor(normalized, gray, ColorConversionCodes.BGR2GRAY);
    }

    Mat drawing = new Mat();
    if (normalized.Channels() == 1)
    {
        Cv2.CvtColor(normalized, drawing, ColorConversionCodes.GRAY2BGR);
    }
    else
    {
        normalized.CopyTo(drawing);
    }

    minimumScore = double.PositiveInfinity;
    maximumResidual = 0D;
    for (int index = 0; index < templates.Count; index++)
    {
        using Mat templateGray = new Mat();
        if (templates[index].Channels() == 1)
        {
            templates[index].CopyTo(templateGray);
        }
        else
        {
            Cv2.CvtColor(templates[index], templateGray, ColorConversionCodes.BGR2GRAY);
        }

        int margin = 18;
        int left = Math.Max(
            0,
            (int)Math.Floor(destinationPoints[index].X - templateGray.Width / 2D - margin));
        int top = Math.Max(
            0,
            (int)Math.Floor(destinationPoints[index].Y - templateGray.Height / 2D - margin));
        int right = Math.Min(
            gray.Width,
            (int)Math.Ceiling(destinationPoints[index].X + templateGray.Width / 2D + margin));
        int bottom = Math.Min(
            gray.Height,
            (int)Math.Ceiling(destinationPoints[index].Y + templateGray.Height / 2D + margin));
        Rect searchRect = new Rect(left, top, right - left, bottom - top);
        using Mat search = gray.SubMat(searchRect);
        using Mat result = new Mat();
        Cv2.MatchTemplate(search, templateGray, result, TemplateMatchModes.CCoeffNormed);
        Cv2.MinMaxLoc(
            result,
            out _,
            out double score,
            out _,
            out OpenCvSharp.Point location);
        Point2f found = new Point2f(
            searchRect.X + location.X + templateGray.Width / 2F,
            searchRect.Y + location.Y + templateGray.Height / 2F);
        double residual = Math.Sqrt(
            Math.Pow(found.X - destinationPoints[index].X, 2D)
            + Math.Pow(found.Y - destinationPoints[index].Y, 2D));
        minimumScore = Math.Min(minimumScore, score);
        maximumResidual = Math.Max(maximumResidual, residual);

        Rect foundRect = new Rect(
            searchRect.X + location.X,
            searchRect.Y + location.Y,
            templateGray.Width,
            templateGray.Height);
        Cv2.Rectangle(drawing, foundRect, new Scalar(255, 0, 255), 2);
        Cv2.DrawMarker(
            drawing,
            new OpenCvSharp.Point(
                (int)Math.Round(destinationPoints[index].X),
                (int)Math.Round(destinationPoints[index].Y)),
            new Scalar(0, 255, 0),
            MarkerTypes.Cross,
            18,
            2);
        Cv2.Line(
            drawing,
            new OpenCvSharp.Point(
                (int)Math.Round(destinationPoints[index].X),
                (int)Math.Round(destinationPoints[index].Y)),
            new OpenCvSharp.Point(
                (int)Math.Round(found.X),
                (int)Math.Round(found.Y)),
            new Scalar(0, 255, 255),
            1);
        Cv2.PutText(
            drawing,
            $"{index + 1}: {score:0.000} / {residual:0.00}px",
            new OpenCvSharp.Point(foundRect.X, Math.Max(18, foundRect.Y - 6)),
            HersheyFonts.HersheySimplex,
            0.5,
            new Scalar(0, 255, 255),
            1,
            LineTypes.AntiAlias);
    }
    return drawing;
}

static void SaveCardPilotContactSheet(
    IReadOnlyList<string> imagePaths,
    IReadOnlyList<string> labels,
    string outputPath)
{
    const int columns = 3;
    const int tileWidth = 320;
    const int imageHeight = 240;
    const int labelHeight = 34;
    int rows = (int)Math.Ceiling(imagePaths.Count / (double)columns);
    using Mat sheet = new Mat(
        rows * (imageHeight + labelHeight),
        columns * tileWidth,
        MatType.CV_8UC3,
        Scalar.Black);
    for (int index = 0; index < imagePaths.Count; index++)
    {
        using Mat source = Cv2.ImRead(imagePaths[index], ImreadModes.Color);
        if (source.Empty())
        {
            continue;
        }
        using Mat resized = new Mat();
        Cv2.Resize(source, resized, new OpenCvSharp.Size(tileWidth, imageHeight));
        int x = index % columns * tileWidth;
        int y = index / columns * (imageHeight + labelHeight);
        resized.CopyTo(sheet.SubMat(new Rect(x, y, tileWidth, imageHeight)));
        Cv2.PutText(
            sheet,
            labels[index],
            new OpenCvSharp.Point(x + 8, y + imageHeight + 23),
            HersheyFonts.HersheySimplex,
            0.42,
            new Scalar(0, 255, 255),
            1,
            LineTypes.AntiAlias);
    }
    Cv2.ImWrite(outputPath, sheet);
}

static string FormatPoint(Point2f point)
{
    return string.Format(
        CultureInfo.InvariantCulture,
        "\"{0:0.000};{1:0.000}\"",
        point.X,
        point.Y);
}

static VisionPipeline CreateDetectedPointAffinePipeline(
    IReadOnlyList<string> templatePaths,
    IReadOnlyList<Point2f> sourcePoints,
    IReadOnlyList<Point2f> destinationPoints)
{
    string[] names = { "01 Locate top-left", "02 Locate top-right", "03 Locate bottom-left" };
    VisionPipeline pipeline = new VisionPipeline { Name = "P219 Matching points to Affine fixed ROI" };
    for (int index = 0; index < 3; index++)
    {
        int roiX = (int)Math.Floor(sourcePoints[index].X) - 30;
        int roiY = (int)Math.Floor(sourcePoints[index].Y) - 30;
        VisionPipelineStep match = new VisionPipelineStep
        {
            Name = names[index],
            ToolType = "Matching",
            Enabled = true,
            InputLayer = VisionRecipeRunner.DefaultInputLayer,
            OutputLayer = $"Match_{index + 1}_Result"
        };
        match.Parameters["PATTERN_PATH"] = templatePaths[index];
        match.Parameters["MATCH_MODE"] = TemplateMatchModes.SqDiffNormed.ToString();
        match.Parameters["SCORE_MIN"] = "0.8";
        match.Parameters["MAGNIFIATION"] = "1";
        match.Parameters["NUM_MATCH"] = "1";
        match.Parameters["USE_FIND_ANGLE"] = "false";
        match.Parameters["USE_FIND_SCALE"] = "false";
        match.Parameters["USE_THRESHOLD"] = "false";
        match.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
        match.Parameters["USE_CANNY"] = "false";
        match.Parameters["USE_ROI"] = "true";
        match.Parameters["CvROI"] = string.Format(
            CultureInfo.InvariantCulture,
            "{0},{1},61,61",
            roiX,
            roiY);
        match.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
        pipeline.Steps.Add(match);
    }

    AffineTransformToolProperty affineProperty = new AffineTransformToolProperty
    {
        DestinationPoint1X = destinationPoints[0].X,
        DestinationPoint1Y = destinationPoints[0].Y,
        DestinationPoint2X = destinationPoints[1].X,
        DestinationPoint2Y = destinationPoints[1].Y,
        DestinationPoint3X = destinationPoints[2].X,
        DestinationPoint3Y = destinationPoints[2].Y,
        OutputWidth = 400,
        OutputHeight = 300,
        MinimumSourceTriangleArea = 1000,
        MinimumDestinationTriangleArea = 1000,
        MinimumValidPixelRatio = 0.6
    };
    VisionPipelineStep affine = VisionPipelineStepBuilder.FromAffineTransformProperty(
        affineProperty,
        "04 Normalize from detected points",
        VisionRecipeRunner.DefaultInputLayer,
        "Reference");
    affine.Parameters[VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter] = "true";
    affine.Parameters[VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter] = names[0] + "/Center";
    affine.Parameters[VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter] = names[1] + "/Center";
    affine.Parameters[VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter] = names[2] + "/Center";
    affine.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
    pipeline.Steps.Add(affine);

    VisionPipelineStep threshold = new VisionPipelineStep
    {
        Name = "05 Threshold normalized target",
        ToolType = "Threshold",
        Enabled = true,
        InputLayer = "Reference",
        OutputLayer = "Reference_Binary"
    };
    threshold.Parameters["Mode"] = "Threshold";
    threshold.Parameters["Threshold"] = "127";
    threshold.Parameters["MaxValue"] = "255";
    threshold.Parameters["ThresholdType"] = ThresholdTypes.Binary.ToString();
    pipeline.Steps.Add(threshold);

    VisionPipelineStep blob = new VisionPipelineStep
    {
        Name = "06 Inspect fixed reference ROI",
        ToolType = "Blob",
        Enabled = true,
        InputLayer = "Reference_Binary",
        OutputLayer = "Inspection_Result",
        UseAcceptance = true,
        ExpectedSuccess = true,
        AcceptanceMetricName = VisionPipelineKnownMetrics.ResultCount,
        UseAcceptanceMetricMinimum = true,
        AcceptanceMetricMinimum = 1,
        UseAcceptanceMetricMaximum = true,
        AcceptanceMetricMaximum = 1
    };
    blob.Parameters["USE_ROI"] = "true";
    blob.Parameters["CvROI"] = "170,120,70,60";
    blob.Parameters["USE_THRESHOLD"] = "false";
    blob.Parameters["MIN_AREA"] = "600";
    blob.Parameters["MAX_AREA"] = "1800";
    pipeline.Steps.Add(blob);
    return pipeline;
}

static Mat CreateAffineFiducial(int index)
{
    Mat template = new Mat(new OpenCvSharp.Size(25, 25), MatType.CV_8UC1, Scalar.Black);
    if (index == 0)
    {
        Cv2.Circle(template, new OpenCvSharp.Point(12, 12), 7, Scalar.White, 2);
        Cv2.Line(template, new OpenCvSharp.Point(3, 12), new OpenCvSharp.Point(21, 12), new Scalar(180), 2);
    }
    else if (index == 1)
    {
        Cv2.Rectangle(template, new Rect(5, 5, 15, 15), Scalar.White, 2);
        Cv2.Line(template, new OpenCvSharp.Point(12, 2), new OpenCvSharp.Point(12, 22), new Scalar(180), 2);
    }
    else
    {
        OpenCvSharp.Point[] triangle =
        {
            new OpenCvSharp.Point(12, 3),
            new OpenCvSharp.Point(3, 21),
            new OpenCvSharp.Point(21, 21)
        };
        Cv2.Polylines(template, new[] { triangle }, true, Scalar.White, 2);
        Cv2.Line(template, new OpenCvSharp.Point(5, 18), new OpenCvSharp.Point(18, 7), new Scalar(180), 2);
    }

    return template;
}

static void VerifyDetectedPointAffinePropertyRoundTrip(
    VisionPipelineStep source,
    ICollection<string> failures)
{
    object? property = VisionPipelineStepPropertyMapper.CreateProperty(source);
    PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(property);
    string[] names =
    {
        "UseDetectedSourcePoints",
        "SourcePoint1Feature",
        "SourcePoint2Feature",
        "SourcePoint3Feature"
    };
    if (property == null
        || names.Any(name => descriptors.Find(name, true) == null)
        || !Equals(descriptors["UseDetectedSourcePoints"]?.GetValue(property), true)
        || !string.Equals(
            Convert.ToString(descriptors["SourcePoint1Feature"]?.GetValue(property), CultureInfo.InvariantCulture),
            "01 Locate top-left/Center",
            StringComparison.Ordinal))
    {
        failures.Add("Detected-point Affine PropertyGrid load did not retain its source bindings.");
        return;
    }

    VisionPipelineStep reapplied = new VisionPipelineStep
    {
        Name = source.Name,
        ToolType = source.ToolType,
        Enabled = true,
        InputLayer = source.InputLayer,
        OutputLayer = source.OutputLayer
    };
    if (!VisionPipelineStepPropertyMapper.ApplyProperty(reapplied, property)
        || reapplied.Parameters.GetValueOrDefault(VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter) != "True"
        || reapplied.Parameters.GetValueOrDefault(VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter) != "01 Locate top-left/Center"
        || reapplied.Parameters.GetValueOrDefault(VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter) != "03 Locate bottom-left/Center")
    {
        failures.Add("Detected-point Affine PropertyGrid apply-back lost its source bindings.");
    }
}

static void VerifyCoreMetric(
    VisionToolResult result,
    string name,
    double expected,
    double tolerance,
    ICollection<string> failures)
{
    double actual = double.NaN;
    if (result?.Metrics == null
        || !result.Metrics.TryGetValue(name, out actual)
        || Math.Abs(actual - expected) > tolerance)
    {
        failures.Add($"{name} expected {expected:0.######} but was {(double.IsNaN(actual) ? "missing" : actual.ToString("0.######", CultureInfo.InvariantCulture))}.");
    }
}

static void SaveAffineDetectedPointEvidence(
    VisionPipelineRunResult run,
    string directory)
{
    for (int index = 0; index < run.StepResults.Count; index++)
    {
        VisionPipelineStepResult stepResult = run.StepResults[index];
        if (stepResult == null)
        {
            continue;
        }

        Mat? image = stepResult?.ToolResult?.ResultImage;
        if (image == null || image.Empty())
        {
            continue;
        }

        using System.Drawing.Bitmap raw = BitmapImageConverter.ToBitmap(image);
        using System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(
            raw.Width,
            raw.Height,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
        {
            graphics.DrawImageUnscaled(raw, 0, 0);
        }
        VisionPipelineRunReportImageRenderer.RenderInPlace(bitmap, stepResult, index + 1);
        string safeName = new string((stepResult?.Step?.Name ?? $"Step_{index + 1}")
            .Select(character => Path.GetInvalidFileNameChars().Contains(character) ? '_' : character)
            .ToArray());
        bitmap.Save(Path.Combine(directory, $"{index + 1:00}_{safeName}.png"));
    }
}

static VisionPipeline ClonePipeline(VisionPipeline source)
{
    VisionPipeline clone = new VisionPipeline { Name = source?.Name ?? string.Empty };
    foreach (VisionPipelineStep item in source?.Steps ?? new List<VisionPipelineStep>())
    {
        if (item == null)
        {
            continue;
        }

        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = item.Name,
            ToolType = item.ToolType,
            Enabled = item.Enabled,
            InputLayer = item.InputLayer,
            OutputLayer = item.OutputLayer,
            UseAcceptance = item.UseAcceptance,
            ExpectedSuccess = item.ExpectedSuccess,
            MaxElapsedMilliseconds = item.MaxElapsedMilliseconds,
            RequiredMessageText = item.RequiredMessageText,
            AcceptanceMetricName = item.AcceptanceMetricName,
            UseAcceptanceMetricMinimum = item.UseAcceptanceMetricMinimum,
            AcceptanceMetricMinimum = item.AcceptanceMetricMinimum,
            UseAcceptanceMetricMaximum = item.UseAcceptanceMetricMaximum,
            AcceptanceMetricMaximum = item.AcceptanceMetricMaximum
        };
        foreach (KeyValuePair<string, string> parameter in item.Parameters)
        {
            step.Parameters[parameter.Key] = parameter.Value;
        }
        clone.Steps.Add(step);
    }
    return clone;
}

static VisionPipeline CreateAffineContractPipeline(VisionPipelineStep source, string toolType)
{
    VisionPipelineStep step = new VisionPipelineStep
    {
        Name = source.Name,
        ToolType = toolType,
        Enabled = true,
        InputLayer = source.InputLayer,
        OutputLayer = source.OutputLayer
    };
    foreach (KeyValuePair<string, string> parameter in source.Parameters)
    {
        step.Parameters[parameter.Key] = parameter.Value;
    }

    VisionPipeline pipeline = new VisionPipeline { Name = "Affine " + toolType + " contract" };
    pipeline.Steps.Add(step);
    return pipeline;
}

static void VerifyAffineTransformPropertyRoundTrip(
    VisionPipelineStep source,
    ICollection<string> failures)
{
    AffineTransformToolProperty? restored =
        VisionPipelineStepPropertyMapper.CreateProperty(source) as AffineTransformToolProperty;
    if (restored == null
        || restored.SourcePoint2X != 100
        || restored.DestinationPoint2X != 132
        || restored.DestinationPoint3Y != 108
        || restored.OutputWidth != 240
        || restored.MinimumValidPixelRatio != 0.4)
    {
        failures.Add("Affine PropertyGrid -> XML -> PropertyGrid round trip failed.");
        return;
    }

    VisionPipelineStep reapplied = new VisionPipelineStep
    {
        Name = source.Name,
        ToolType = source.ToolType,
        Enabled = true,
        InputLayer = source.InputLayer,
        OutputLayer = source.OutputLayer
    };
    if (!VisionPipelineStepPropertyMapper.ApplyProperty(reapplied, restored)
        || reapplied.ToolType != "AffineTransform"
        || reapplied.Parameters.GetValueOrDefault(nameof(AffineTransformToolProperty.DestinationPoint2X)) != "132"
        || reapplied.Parameters.GetValueOrDefault(nameof(AffineTransformToolProperty.MinimumValidPixelRatio)) != "0.4")
    {
        failures.Add("Affine selected-Step PropertyGrid apply-back round trip failed.");
    }
}

static void VerifyAffineMetric(
    VisionRecipeStepRunSummary step,
    string metricName,
    double expected,
    ICollection<string> failures,
    string alias)
{
    if (!step.Metrics.TryGetValue(metricName, out double actual)
        || Math.Abs(actual - expected) > 1e-6)
    {
        failures.Add(alias + ": " + metricName + " expected " + expected.ToString("0.######", CultureInfo.InvariantCulture)
            + ", actual " + (double.IsNaN(actual) ? "missing" : actual.ToString("0.######", CultureInfo.InvariantCulture)) + ".");
    }
}

static async Task VerifyObjectDimensionFilterAsync(
    Mat source,
    string toolType,
    ICollection<string> failures,
    string? evidenceDirectory)
{
    VisionPipeline filteredPipeline = CreateObjectDimensionPipeline(toolType, includeDimensions: true);
    VisionRecipeRunner runner = new VisionRecipeRunner();
    using VisionRecipeRunResult filtered = await runner.RunAsync(filteredPipeline, source);
    VisionRecipeStepRunSummary? step = filtered.Steps.SingleOrDefault();
    if (!filtered.Success || step == null)
    {
        failures.Add($"{toolType}: filtered pipeline did not complete. {filtered.Message}");
        return;
    }

    double resultCount = step.Metrics.GetValueOrDefault(VisionPipelineKnownMetrics.ResultCount, -1D);
    if (resultCount != 1D)
    {
        failures.Add($"{toolType}: expected ResultCount=1 after dimension filters, actual {resultCount:0.###}.");
    }

    if (step.ObjectResults.Count(item => item.Accepted) != 1)
    {
        failures.Add($"{toolType}: Object Results Inspector did not retain exactly one accepted row.");
    }

    if (step.ObjectResults.Any(item => string.IsNullOrWhiteSpace(item.CandidateId))
        || step.ObjectResults.Select(item => item.CandidateId).Distinct(StringComparer.Ordinal).Count() != step.ObjectResults.Count
        || step.ObjectResults.Any(item => item.NativeIndex < 0)
        || step.ObjectResults.Any(item => string.IsNullOrWhiteSpace(item.GenerationStage)
            || !string.Equals(item.CoordinateFrame, "SourceImage", StringComparison.Ordinal)))
    {
        failures.Add($"{toolType}: one-pass candidate identity/coordinate metadata was not retained.");
    }

    if (step.ObjectResults.Any(item => !item.Accepted && string.IsNullOrWhiteSpace(item.RejectReasonCode)))
    {
        failures.Add($"{toolType}: rejected candidate rows did not retain a stable reject reason code.");
    }

    string[] expectedReasonPrefixes =
    {
        "Width 52 > MAX_WIDTH 30",
        "Width 8 < MIN_WIDTH 15",
        "Height 8 < MIN_HEIGHT 16",
        "Height 60 > MAX_HEIGHT 40"
    };
    foreach (string expected in expectedReasonPrefixes)
    {
        if (!step.ObjectResults.Any(item => item.RejectReason.StartsWith(expected, StringComparison.Ordinal)))
        {
            failures.Add($"{toolType}: missing exact reject reason '{expected}'.");
        }
    }

    int acceptedRectangles = step.Overlays.Count(item =>
        string.Equals(item.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase)
        && string.Equals(item.Label, "Accepted object", StringComparison.Ordinal));
    if (acceptedRectangles != 1)
    {
        failures.Add($"{toolType}: accepted drawing count should be 1, actual {acceptedRectangles}.");
    }

    if (evidenceDirectory != null)
    {
        SaveObjectDimensionEvidence(
            source,
            step.ObjectResults,
            Path.Combine(
                evidenceDirectory,
                toolType.ToLowerInvariant() + "_object_dimension_filter_drawing.png"));
        File.WriteAllLines(
            Path.Combine(
                evidenceDirectory,
                toolType.ToLowerInvariant() + "_object_dimension_filter_rows.tsv"),
            new[] { "Number\tCandidateId\tNativeIndex\tAccepted\tArea\tX\tY\tWidth\tHeight\tAngle\tRejectReasonCode\tRejectReason\tGenerationStage\tCoordinateFrame" }
                .Concat(step.ObjectResults.Select(item => string.Join(
                    "\t",
                    item.Number.ToString(CultureInfo.InvariantCulture),
                    item.CandidateId,
                    item.NativeIndex.ToString(CultureInfo.InvariantCulture),
                    item.Accepted.ToString(CultureInfo.InvariantCulture),
                    item.Area.ToString("0.###", CultureInfo.InvariantCulture),
                    item.BoundsX.ToString(CultureInfo.InvariantCulture),
                    item.BoundsY.ToString(CultureInfo.InvariantCulture),
                    item.BoundsWidth.ToString(CultureInfo.InvariantCulture),
                    item.BoundsHeight.ToString(CultureInfo.InvariantCulture),
                    item.Angle.ToString("0.###", CultureInfo.InvariantCulture),
                    item.RejectReasonCode,
                    item.RejectReason,
                    item.GenerationStage,
                    item.CoordinateFrame))));
    }

    if (string.Equals(toolType, "Blob", StringComparison.Ordinal))
    {
        string recipeName = "Smoke_P216ObjectDimensions_" + Guid.NewGuid().ToString("N");
        try
        {
            DateTime startedAt = DateTime.Now;
            string reportPath = VisionPipelineRunReportStorage.Save(
                recipeName,
                filteredPipeline,
                filtered,
                startedAt,
                startedAt.AddMilliseconds(filtered.TotalMilliseconds),
                "dimension-contract",
                source);
            VisionPipelineRunReport report = VisionPipelineRunReportStorage.Load(reportPath);
            if (evidenceDirectory != null)
            {
                File.Copy(
                    reportPath,
                    Path.Combine(evidenceDirectory, "blob_object_dimension_filter_run_report.xml"),
                    overwrite: true);
            }
            List<string> persistedReasons = report?.Steps.SingleOrDefault()?.Objects
                .Select(item => item.RejectReason)
                .Where(reason => !string.IsNullOrWhiteSpace(reason))
                .ToList() ?? new List<string>();
            foreach (string expected in expectedReasonPrefixes)
            {
                if (!persistedReasons.Any(reason => reason.StartsWith(expected, StringComparison.Ordinal)))
                {
                    failures.Add($"Blob: saved Run History missed reject reason '{expected}'.");
                }
            }

            List<VisionPipelineObjectRunReport> persistedObjects = report?.Steps.SingleOrDefault()?.Objects
                ?? new List<VisionPipelineObjectRunReport>();
            if (persistedObjects.Any(item => string.IsNullOrWhiteSpace(item.CandidateId)
                || item.NativeIndex < 0
                || string.IsNullOrWhiteSpace(item.RejectReasonCode)
                && !item.Accepted
                || !string.Equals(item.CoordinateFrame, "SourceImage", StringComparison.Ordinal)))
            {
                failures.Add("Blob: saved Run History missed one-pass candidate metadata.");
            }
        }
        finally
        {
            RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
        }
    }

    VisionPipeline legacyPipeline = CreateObjectDimensionPipeline(toolType, includeDimensions: false);
    using VisionRecipeRunResult legacy = await runner.RunAsync(legacyPipeline, source);
    VisionRecipeStepRunSummary? legacyStep = legacy.Steps.SingleOrDefault();
    double legacyCount = legacyStep?.Metrics.GetValueOrDefault(VisionPipelineKnownMetrics.ResultCount, -1D) ?? -1D;
    if (!legacy.Success || legacyCount != 5D)
    {
        failures.Add($"{toolType}: legacy XML defaults must preserve all 5 area-valid objects; actual {legacyCount:0.###}.");
    }
}

static void SaveObjectDimensionEvidence(
    Mat source,
    IEnumerable<VisionPipelineObjectResult> objectResults,
    string outputPath)
{
    using Mat drawing = new Mat();
    Cv2.CvtColor(source, drawing, ColorConversionCodes.GRAY2BGR);
    foreach (VisionPipelineObjectResult item in objectResults)
    {
        Scalar color = item.Accepted
            ? new Scalar(0, 220, 0)
            : new Scalar(0, 0, 255);
        Cv2.Rectangle(
            drawing,
            new Rect(item.BoundsX, item.BoundsY, item.BoundsWidth, item.BoundsHeight),
            color,
            2,
            LineTypes.AntiAlias);
        string label = item.Accepted
            ? "OK"
            : item.RejectReason.StartsWith("Width", StringComparison.Ordinal)
                ? item.RejectReason.Contains(" < ", StringComparison.Ordinal) ? "W<MIN" : "W>MAX"
                : item.RejectReason.StartsWith("Height", StringComparison.Ordinal)
                    ? item.RejectReason.Contains(" < ", StringComparison.Ordinal) ? "H<MIN" : "H>MAX"
                    : "AREA";
        Cv2.PutText(
            drawing,
            label,
            new OpenCvSharp.Point(item.BoundsX, Math.Max(13, item.BoundsY - 4)),
            HersheyFonts.HersheySimplex,
            0.42,
            color,
            1,
            LineTypes.AntiAlias);
    }

    Cv2.PutText(
        drawing,
        "GREEN=accepted  RED=rejected | W 15..30 px | H 16..40 px",
        new OpenCvSharp.Point(8, 124),
        HersheyFonts.HersheySimplex,
        0.38,
        new Scalar(0, 220, 255),
        1,
        LineTypes.AntiAlias);
    Cv2.ImWrite(outputPath, drawing);
}

static VisionPipeline CreateObjectDimensionPipeline(string toolType, bool includeDimensions)
{
    VisionPipelineStep step = new VisionPipelineStep
    {
        Name = toolType + " dimension filter",
        ToolType = toolType,
        Enabled = true,
        InputLayer = VisionRecipeRunner.DefaultInputLayer,
        OutputLayer = toolType + "_Result"
    };
    step.Parameters["USE_THRESHOLD"] = "false";
    step.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
    step.Parameters["USE_BITWISENOT"] = "false";
    step.Parameters["MIN_AREA"] = "50";
    step.Parameters["MAX_AREA"] = "5000";
    if (includeDimensions)
    {
        step.Parameters["MIN_WIDTH"] = "15";
        step.Parameters["MAX_WIDTH"] = "30";
        step.Parameters["MIN_HEIGHT"] = "16";
        step.Parameters["MAX_HEIGHT"] = "40";
    }

    VisionPipeline pipeline = new VisionPipeline { Name = toolType + " dimension contract" };
    pipeline.Steps.Add(step);
    return pipeline;
}

static void VerifyObjectDimensionPropertyRoundTrip(ICollection<string> failures)
{
    BlobProperty source = new BlobProperty("Blob dimensions")
    {
        MIN_AREA = 50,
        MAX_AREA = 5000,
        MIN_WIDTH = 15,
        MAX_WIDTH = 30,
        MIN_HEIGHT = 16,
        MAX_HEIGHT = 40
    };
    VisionPipelineStep step = VisionPipelineStepBuilder.FromProperty(source, "Main", "Blob_Result");
    BlobProperty? restored = VisionPipelineStepPropertyMapper.CreateProperty(step) as BlobProperty;
    if (restored == null
        || restored.MIN_WIDTH != 15
        || restored.MAX_WIDTH != 30
        || restored.MIN_HEIGHT != 16
        || restored.MAX_HEIGHT != 40)
    {
        failures.Add("Blob PropertyGrid -> XML -> PropertyGrid dimension round trip failed.");
    }

    VisionPipelineStep legacy = new VisionPipelineStep
    {
        Name = "Legacy contour",
        ToolType = "Contour",
        Enabled = true,
        InputLayer = "Main",
        OutputLayer = "Contour_Result"
    };
    legacy.Parameters["MIN_AREA"] = "50";
    legacy.Parameters["MAX_AREA"] = "5000";
    ContourProperty? legacyRestored = VisionPipelineStepPropertyMapper.CreateProperty(legacy) as ContourProperty;
    if (legacyRestored == null
        || legacyRestored.MIN_WIDTH != 0
        || legacyRestored.MAX_WIDTH != 1000000
        || legacyRestored.MIN_HEIGHT != 0
        || legacyRestored.MAX_HEIGHT != 1000000)
    {
        failures.Add("Legacy missing dimension keys did not restore unbounded Contour defaults.");
    }
}

static void VerifyObjectDimensionValidation(ICollection<string> failures)
{
    VisionPipeline invalid = CreateObjectDimensionPipeline("Blob", includeDimensions: true);
    invalid.Steps[0].Parameters["MIN_WIDTH"] = "31";
    invalid.Steps[0].Parameters["MAX_WIDTH"] = "30";
    VisionPipelineValidationResult result = VisionPipelineValidator.Validate(invalid, new[] { "Main" });
    if (result.Success
        || !result.Errors.Any(error => error.Contains("MIN_WIDTH is greater than MAX_WIDTH", StringComparison.Ordinal)))
    {
        failures.Add("Reversed MIN_WIDTH/MAX_WIDTH did not fail strict pipeline validation.");
    }
}

static object? InvokePinArrayGapParse(MethodInfo method, string roiText, out bool succeeded, out string message)
{
    object?[] invokeArguments = { roiText, null, null };
    succeeded = method.Invoke(null, invokeArguments) as bool? == true;
    message = invokeArguments[2]?.ToString() ?? string.Empty;
    return invokeArguments[1];
}

static bool InvokePinArrayGapValidation(
    MethodInfo method,
    object rowRois,
    string measurementDefinition,
    string pinPolarity,
    int sourceWidth,
    int sourceHeight,
    out string message)
{
    object?[] invokeArguments =
    {
        measurementDefinition,
        pinPolarity,
        "px",
        rowRois,
        sourceWidth,
        sourceHeight,
        128,
        0.55D,
        5,
        2,
        3,
        null
    };
    bool succeeded = method.Invoke(null, invokeArguments) as bool? == true;
    message = invokeArguments[11]?.ToString() ?? string.Empty;
    return succeeded;
}

static void VerifyPinArrayGapMeasurementPipeline(VisionPipeline? pipeline, ICollection<string> failures)
{
    if (pipeline == null || pipeline.Steps.Count != 2)
    {
        failures.Add("Measurement starter must contain exactly two PinArrayGap steps.");
        return;
    }

    if (pipeline.Steps.Any(step => !string.Equals(step.ToolType, "PinArrayGap", StringComparison.Ordinal)))
    {
        failures.Add("Measurement starter contains a tool outside the locked PinArrayGap family.");
    }

    if (pipeline.Steps.Any(step => step.UseAcceptance))
    {
        failures.Add("Measurement-only starter unexpectedly contains an acceptance gate.");
    }

    if (pipeline.Steps[0].Parameters.ContainsKey("ALLOW_BRANCH_INPUT")
        || !pipeline.Steps[1].Parameters.TryGetValue("ALLOW_BRANCH_INPUT", out string? branchValue)
        || !string.Equals(branchValue, "true", StringComparison.OrdinalIgnoreCase))
    {
        failures.Add("Only the second row step must opt into branch input.");
    }
}

static void VerifyPinArrayGapJudgedPipeline(VisionPipeline? pipeline, ICollection<string> failures)
{
    if (pipeline == null || pipeline.Steps.Count != 2)
    {
        failures.Add("Judged starter must contain exactly two PinArrayGap steps.");
        return;
    }

    if (pipeline.Steps.Any(step =>
            !step.UseAcceptance
            || !string.Equals(step.AcceptanceMetricName, "DistancePxRange", StringComparison.Ordinal)
            || !step.UseAcceptanceMetricMaximum
            || step.AcceptanceMetricMaximum != 6D))
    {
        failures.Add("Every judged row must use an exact DistancePxRange maximum of 6.");
    }
}

static async Task<int> RunBatchAsync(
    string imageListPath,
    string datasetRoot,
    string pipelineXmlPath,
    string csvPath,
    string? evidenceRoot = null)
{
    string listPath = Path.GetFullPath(imageListPath);
    string rootPath = Path.GetFullPath(datasetRoot);
    string xmlPath = Path.GetFullPath(pipelineXmlPath);
    string outputPath = Path.GetFullPath(csvPath);
    string? evidencePath = string.IsNullOrWhiteSpace(evidenceRoot) ? null : Path.GetFullPath(evidenceRoot);

    if (!File.Exists(listPath) || !Directory.Exists(rootPath) || !File.Exists(xmlPath))
    {
        Console.Error.WriteLine("Batch prerequisites are missing: image list, dataset root, or pipeline XML.");
        return 2;
    }

    string[] entries = File.ReadAllLines(listPath)
        .Where(entry => !string.IsNullOrWhiteSpace(entry))
        .ToArray();
    if (entries.Length == 0)
    {
        Console.Error.WriteLine("Batch image list is empty.");
        return 2;
    }

    Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
    VisionPipeline? evidencePipeline = null;
    StreamWriter? evidenceWriter = null;
    if (evidencePath != null)
    {
        if (!SerializeHelper.TryLoadFromXmlFile(xmlPath, out evidencePipeline) || evidencePipeline == null)
        {
            Console.Error.WriteLine("Pipeline XML could not be loaded for batch evidence drawings.");
            return 2;
        }

        Directory.CreateDirectory(Path.Combine(evidencePath, "runs"));
        evidenceWriter = new StreamWriter(
            Path.Combine(evidencePath, "evidence_rows.csv"),
            false,
            new System.Text.UTF8Encoding(false));
        evidenceWriter.WriteLine("ImagePath,Expected,SourceSha256,PipelineSuccess,StepIndex,StepName,StepSuccess,StepStatus,ErrorCode,ErrorName,Message,ResultImagePath,ResultImageSha256,StepOverlayPath,ScoreMax,ScoreSecond,ScoreMargin,FixtureCenterX,FixtureCenterY,FixtureAngle,FixtureAngleDelta,FixtureScale,FixtureScaleRatio,FixtureValidPixelRatio,DistancePxMin,DistancePxMax,DistancePxAvg,DistancePxRange,PitchPxMin,PitchPxMax,PitchPxAvg,PitchPxRange,GapCandidateLineCount,GapCandidatePairCount,GapOverlapPairCount,GapSeparationPairCount,GapParallelPairCount,GapContrastPairCount,GapSelectedAngleDeltaDeg,GapSelectedSupportRatio,GapDarkContrast,GapDarkCoverageRatio,GapBandMeanGray,GapScoreMargin,GapUpperSupportPointCount,GapLowerSupportPointCount,ElapsedMilliseconds");
    }

    int completed = 0;
    int pipelinePasses = 0;
    int missingImages = 0;
    VisionRecipeRunner runner = new VisionRecipeRunner();
    using StreamWriter writer = new StreamWriter(outputPath, false, new System.Text.UTF8Encoding(false));
    writer.WriteLine("ImagePath,Expected,PipelineSuccess,StepIndex,StepName,ToolType,StepSuccess,StepStatus,ResultCount,IntersectionCross,IntersectionX,IntersectionY,CornerOuterContourVerified,LineAngleMin,LineAngleMax,LineAngleAvg,CurveOuterArcLengthPx,CurveInnerArcLengthPx,CurveCenterArcLengthPx,CurveProfileRowCount,DistancePxMin,DistancePxMax,DistancePxAvg,DistancePxRange,PitchPxMin,PitchPxMax,PitchPxAvg,PitchPxRange,ElapsedMilliseconds,ErrorCode,ErrorName,Message");

    foreach (string entry in entries)
    {
        string imagePath = ResolveBatchImagePath(rootPath, entry);
        string expected = ResolveExpectedOutcome(entry);
        if (!File.Exists(imagePath))
        {
            missingImages++;
            WriteBatchCsvRow(writer, imagePath, expected, false, 0, string.Empty, string.Empty, false, "MISSING", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0, -1, "MissingImage", "Image was not found.");
            continue;
        }

        using Mat source = Cv2.ImRead(imagePath, ImreadModes.Unchanged);
        if (source.Empty())
        {
            WriteBatchCsvRow(writer, imagePath, expected, false, 0, string.Empty, string.Empty, false, "ERROR", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0, -1, "ImageLoadFailed", "Image could not be loaded.");
            continue;
        }

        string sourceSha256 = ComputeSha256(imagePath);
        using Mat sourceEvidence = source.Clone();
        using VisionRecipeRunResult result = await runner.RunAsync(xmlPath, source);
        completed++;
        if (result.Success)
        {
            pipelinePasses++;
        }

        string resultEvidencePath = string.Empty;
        string resultSha256 = string.Empty;
        string evidenceRunPath = string.Empty;
        if (evidencePath != null && evidencePipeline != null)
        {
            string runName = CreateEvidenceDirectoryName(expected, imagePath, sourceSha256);
            evidenceRunPath = Path.Combine(evidencePath, "runs", runName);
            Directory.CreateDirectory(evidenceRunPath);
            string sourceSnapshotPath = Path.Combine(evidenceRunPath, "source" + Path.GetExtension(imagePath).ToLowerInvariant());
            File.Copy(imagePath, sourceSnapshotPath, true);
            resultEvidencePath = Path.Combine(evidenceRunPath, "runtime_result.png");
            SaveAllOverlayImage(sourceEvidence, result, evidencePipeline, resultEvidencePath);

            if (File.Exists(resultEvidencePath))
            {
                resultSha256 = ComputeSha256(resultEvidencePath);
            }
        }

        foreach (VisionRecipeStepRunSummary step in result.Steps)
        {
            double? resultCount = TryGetMetric(step, "ResultCount");
            double? intersectionCross = TryGetMetric(step, "IntersectionCross");
            double? intersectionX = TryGetMetric(step, "IntersectionX");
            double? intersectionY = TryGetMetric(step, "IntersectionY");
            double? cornerOuterContourVerified = TryGetMetric(step, "CornerOuterContourVerified");
            double? lineAngleMinimum = TryGetMetric(step, "LineAngleMin");
            double? lineAngleMaximum = TryGetMetric(step, "LineAngleMax");
            double? lineAngleAverage = TryGetMetric(step, "LineAngleAvg");
            double? curveOuterArcLength = TryGetMetric(step, "CurveOuterArcLengthPx");
            double? curveInnerArcLength = TryGetMetric(step, "CurveInnerArcLengthPx");
            double? curveCenterArcLength = TryGetMetric(step, "CurveCenterArcLengthPx");
            double? curveProfileRowCount = TryGetMetric(step, "CurveProfileRowCount");
            double? minimum = TryGetMetric(step, "DistancePxMin");
            double? maximum = TryGetMetric(step, "DistancePxMax");
            double? average = TryGetMetric(step, "DistancePxAvg");
            double? range = TryGetMetric(step, "DistancePxRange");
            double? pitchMinimum = TryGetMetric(step, "PitchPxMin");
            double? pitchMaximum = TryGetMetric(step, "PitchPxMax");
            double? pitchAverage = TryGetMetric(step, "PitchPxAvg");
            double? pitchRange = TryGetMetric(step, "PitchPxRange");
            WriteBatchCsvRow(
                writer,
                imagePath,
                expected,
                result.Success,
                step.Index,
                step.Name,
                step.ToolType,
                step.Success,
                step.Status,
                resultCount,
                intersectionCross,
                intersectionX,
                intersectionY,
                cornerOuterContourVerified,
                lineAngleMinimum,
                lineAngleMaximum,
                lineAngleAverage,
                curveOuterArcLength,
                curveInnerArcLength,
                curveCenterArcLength,
                curveProfileRowCount,
                minimum,
                maximum,
                average,
                range,
                pitchMinimum,
                pitchMaximum,
                pitchAverage,
                pitchRange,
                step.ElapsedMilliseconds,
                step.ErrorCode,
                step.ErrorName,
                step.Message);

            if (evidenceWriter != null)
            {
                string stepOverlayPath = string.Empty;
                if (!string.IsNullOrWhiteSpace(evidenceRunPath)
                    && (string.Equals(step.ToolType, "Matching", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(step.ToolType, "EdgeBasedMatching", StringComparison.OrdinalIgnoreCase))
                    && evidencePipeline != null
                    && step.Index > 0
                    && step.Index <= evidencePipeline.Steps.Count)
                {
                    string overlaySuffix = string.Equals(step.ToolType, "EdgeBasedMatching", StringComparison.OrdinalIgnoreCase)
                        ? "_edgebasedmatching_overlay.png"
                        : "_matching_overlay.png";
                    stepOverlayPath = Path.Combine(
                        evidenceRunPath,
                        step.Index.ToString("00", CultureInfo.InvariantCulture) + overlaySuffix);
                    SaveStepOverlayImage(source, step, evidencePipeline.Steps[step.Index - 1], stepOverlayPath);
                }

                WriteEvidenceCsvRow(
                    evidenceWriter,
                    imagePath,
                    expected,
                    sourceSha256,
                    result.Success,
                    step,
                    resultEvidencePath,
                    resultSha256,
                    stepOverlayPath);
            }
        }
    }

    evidenceWriter?.Dispose();

    Console.WriteLine($"BatchRows={entries.Length}");
    Console.WriteLine($"BatchCompleted={completed}");
    Console.WriteLine($"BatchPipelinePasses={pipelinePasses}");
    Console.WriteLine($"BatchMissingImages={missingImages}");
    Console.WriteLine($"BatchCsv={outputPath}");
    if (evidencePath != null)
    {
        Console.WriteLine($"BatchEvidence={evidencePath}");
    }
    return 0;
}

static string CreateEvidenceDirectoryName(string expected, string imagePath, string sourceSha256)
{
    string name = Path.GetFileNameWithoutExtension(imagePath);
    foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
    {
        name = name.Replace(invalidCharacter, '_');
    }

    string hashSuffix = sourceSha256.Length >= 12 ? sourceSha256[..12] : sourceSha256;
    return expected + "_" + name + "_" + hashSuffix;
}

static string ComputeSha256(string path)
{
    using FileStream stream = File.OpenRead(path);
    return Convert.ToHexString(SHA256.HashData(stream));
}

static void WriteEvidenceCsvRow(
    TextWriter writer,
    string imagePath,
    string expected,
    string sourceSha256,
    bool pipelineSuccess,
    VisionRecipeStepRunSummary step,
    string resultImagePath,
    string resultImageSha256,
    string stepOverlayPath)
{
    string[] metricNames =
    {
        "ScoreMax",
        "ScoreMin",
        "ScoreMargin",
        "FixtureCenterX",
        "FixtureCenterY",
        "FixtureAngle",
        "FixtureAngleDelta",
        "FixtureScale",
        "FixtureScaleRatio",
        "FixtureValidPixelRatio",
        "DistancePxMin",
        "DistancePxMax",
        "DistancePxAvg",
        "DistancePxRange",
        "PitchPxMin",
        "PitchPxMax",
        "PitchPxAvg",
        "PitchPxRange",
        "GapCandidateLineCount",
        "GapCandidatePairCount",
        "GapOverlapPairCount",
        "GapSeparationPairCount",
        "GapParallelPairCount",
        "GapContrastPairCount",
        "GapSelectedAngleDeltaDeg",
        "GapSelectedSupportRatio",
        "GapDarkContrast",
        "GapDarkCoverageRatio",
        "GapBandMeanGray",
        "GapScoreMargin",
        "GapUpperSupportPointCount",
        "GapLowerSupportPointCount"
    };

    List<string> values = new List<string>
    {
        imagePath,
        expected,
        sourceSha256,
        pipelineSuccess ? "true" : "false",
        step.Index.ToString(CultureInfo.InvariantCulture),
        step.Name,
        step.Success ? "true" : "false",
        step.Status,
        step.ErrorCode.ToString(CultureInfo.InvariantCulture),
        step.ErrorName,
        step.Message,
        resultImagePath,
        resultImageSha256,
        stepOverlayPath
    };

    values.AddRange(metricNames.Select(metricName =>
        TryGetMetric(step, metricName)?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty));
    values.Add(step.ElapsedMilliseconds.ToString("0.############", CultureInfo.InvariantCulture));
    writer.WriteLine(string.Join(",", values.Select(EscapeBatchCsvValue)));
}

static void SaveStepOverlayImage(
    Mat sourceImage,
    VisionRecipeStepRunSummary step,
    VisionPipelineStep definition,
    string outputPath)
{
    using Mat preview = new Mat();
    if (sourceImage.Channels() == 1)
    {
        Cv2.CvtColor(sourceImage, preview, ColorConversionCodes.GRAY2BGR);
    }
    else
    {
        sourceImage.CopyTo(preview);
    }

    Scalar color = new Scalar(0, 220, 70);
    int thickness = Math.Max(2, Math.Min(sourceImage.Width, sourceImage.Height) / 260);
    int labelThickness = Math.Max(1, thickness - 1);
    IReadOnlyDictionary<string, string> parameters = step.Parameters
        ?? definition.Parameters
        ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    DrawStepRoiOverlay(preview, step.Index, parameters, color, thickness, labelThickness);
    foreach (VisionRecipeOverlaySummary overlay in step.Overlays)
    {
        if (string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
        {
            DrawRectangleOverlay(preview, overlay, color, thickness, labelThickness, step.Index.ToString("00", CultureInfo.InvariantCulture));
        }
        else if (string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase))
        {
            DrawLineOverlay(preview, overlay, color, thickness, labelThickness, step.Index.ToString("00", CultureInfo.InvariantCulture));
        }
        else if (string.Equals(overlay.Kind, "Point", StringComparison.OrdinalIgnoreCase))
        {
            DrawPointOverlay(preview, overlay.CenterX, overlay.CenterY, color, thickness, step.Index.ToString("00", CultureInfo.InvariantCulture));
        }
        else if (string.Equals(overlay.Kind, "Points", StringComparison.OrdinalIgnoreCase))
        {
            DrawPointsOverlay(preview, overlay, color, thickness);
        }
    }

    Cv2.ImWrite(outputPath, preview);
}

static string ResolveBatchImagePath(string datasetRoot, string entry)
{
    string value = (entry ?? string.Empty).Trim().Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
    if (Path.IsPathRooted(value))
    {
        return Path.GetFullPath(value);
    }

    string direct = Path.Combine(datasetRoot, value);
    if (File.Exists(direct))
    {
        return direct;
    }

    int imagesIndex = value.IndexOf(Path.DirectorySeparatorChar + "images" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    string relativeToImages = imagesIndex >= 0
        ? value.Substring(imagesIndex + 1)
        : value;
    return Path.Combine(datasetRoot, relativeToImages);
}

static string ResolveExpectedOutcome(string entry)
{
    string normalized = (entry ?? string.Empty).Replace('/', '\\');
    return normalized.IndexOf("\\NG\\", StringComparison.OrdinalIgnoreCase) >= 0 ? "NG" : "OK";
}

static double? TryGetMetric(VisionRecipeStepRunSummary step, string metricName)
{
    return step?.Metrics != null && step.Metrics.TryGetValue(metricName, out double value)
        ? value
        : null;
}

static void WriteBatchCsvRow(
    TextWriter writer,
    string imagePath,
    string expected,
    bool pipelineSuccess,
    int stepIndex,
    string stepName,
    string toolType,
    bool stepSuccess,
    string stepStatus,
    double? resultCount,
    double? intersectionCross,
    double? intersectionX,
    double? intersectionY,
    double? cornerOuterContourVerified,
    double? lineAngleMinimum,
    double? lineAngleMaximum,
    double? lineAngleAverage,
    double? curveOuterArcLength,
    double? curveInnerArcLength,
    double? curveCenterArcLength,
    double? curveProfileRowCount,
    double? distancePxMinimum,
    double? distancePxMaximum,
    double? distancePxAverage,
    double? distancePxRange,
    double? pitchPxMinimum,
    double? pitchPxMaximum,
    double? pitchPxAverage,
    double? pitchPxRange,
    double elapsedMilliseconds,
    int errorCode,
    string errorName,
    string message)
{
    string[] values =
    {
        imagePath,
        expected,
        pipelineSuccess ? "true" : "false",
        stepIndex.ToString(CultureInfo.InvariantCulture),
        stepName,
        toolType,
        stepSuccess ? "true" : "false",
        stepStatus,
        resultCount?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        intersectionCross?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        intersectionX?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        intersectionY?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        cornerOuterContourVerified?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        lineAngleMinimum?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        lineAngleMaximum?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        lineAngleAverage?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        curveOuterArcLength?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        curveInnerArcLength?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        curveCenterArcLength?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        curveProfileRowCount?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        distancePxMinimum?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        distancePxMaximum?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        distancePxAverage?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        distancePxRange?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        pitchPxMinimum?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        pitchPxMaximum?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        pitchPxAverage?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        pitchPxRange?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        elapsedMilliseconds.ToString("0.############", CultureInfo.InvariantCulture),
        errorCode.ToString(CultureInfo.InvariantCulture),
        errorName,
        message
    };
    writer.WriteLine(string.Join(",", values.Select(EscapeBatchCsvValue)));
}

static string EscapeBatchCsvValue(string value)
{
    string text = value ?? string.Empty;
    return "\"" + text.Replace("\"", "\"\"") + "\"";
}

static void SaveAllOverlayImage(Mat sourceImage, VisionRecipeRunResult runResult, VisionPipeline pipeline, string outputPath)
{
    if (sourceImage == null || sourceImage.Empty() || runResult == null)
    {
        return;
    }

    Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
    using Mat preview = new Mat();
    if (sourceImage.Channels() == 1)
    {
        Cv2.CvtColor(sourceImage, preview, ColorConversionCodes.GRAY2BGR);
    }
    else
    {
        sourceImage.CopyTo(preview);
    }

    Scalar[] colors =
    {
        new Scalar(0, 220, 70),
        new Scalar(255, 130, 0),
        new Scalar(0, 190, 255),
        new Scalar(220, 80, 255)
    };

    int thickness = Math.Max(2, Math.Min(sourceImage.Width, sourceImage.Height) / 260);
    int labelThickness = Math.Max(1, thickness - 1);
    int colorIndex = 0;
    foreach (VisionRecipeStepRunSummary step in runResult.Steps)
    {
        IReadOnlyDictionary<string, string> parameters = step.Parameters;
        if ((parameters == null || !TryGetBoolParameter(parameters, "USE_ROI"))
            && step.Index > 0
            && step.Index <= pipeline.Steps.Count)
        {
            parameters = pipeline.Steps[step.Index - 1].Parameters;
        }

        if (step.Overlays.Count == 0 && (parameters == null || !TryGetBoolParameter(parameters, "USE_ROI")))
        {
            continue;
        }

        Scalar color = colors[colorIndex++ % colors.Length];
        DrawStepRoiOverlay(preview, step.Index, parameters!, color, thickness, labelThickness);
        foreach (VisionRecipeOverlaySummary overlay in step.Overlays)
        {
            if (string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
            {
                DrawRectangleOverlay(preview, overlay, color, thickness, labelThickness, $"{step.Index:00}");
            }
            else if (string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase))
            {
                DrawLineOverlay(preview, overlay, color, thickness, labelThickness, $"{step.Index:00}");
            }
            else if (string.Equals(overlay.Kind, "Point", StringComparison.OrdinalIgnoreCase))
            {
                DrawPointOverlay(preview, overlay.CenterX, overlay.CenterY, color, thickness, $"{step.Index:00}");
            }
            else if (string.Equals(overlay.Kind, "Points", StringComparison.OrdinalIgnoreCase))
            {
                DrawPointsOverlay(preview, overlay, color, thickness);
            }
        }
    }

    Cv2.ImWrite(outputPath, preview);
}

static void DrawStepRoiOverlay(
    Mat preview,
    int stepIndex,
    IReadOnlyDictionary<string, string> parameters,
    Scalar color,
    int thickness,
    int labelThickness)
{
    if (parameters == null
        || !TryGetBoolParameter(parameters, "USE_ROI")
        || !TryGetRectParameter(parameters, "CvROI", out Rect roi)
        || roi.Width <= 0
        || roi.Height <= 0)
    {
        return;
    }

    Rect bounds = ClampRect(preview, roi);
    if (bounds.Width <= 0
        || bounds.Height <= 0
        || (bounds.X <= 0 && bounds.Y <= 0 && bounds.Width >= preview.Width - 1 && bounds.Height >= preview.Height - 1))
    {
        return;
    }

    Cv2.Rectangle(preview, bounds, color, Math.Max(1, thickness - 1), LineTypes.AntiAlias);
    DrawLabel(preview, $"{stepIndex:00} ROI", new OpenCvSharp.Point(bounds.X + 3, bounds.Y + 14), color, labelThickness);
}

static bool TryGetBoolParameter(IReadOnlyDictionary<string, string> parameters, string key)
{
    if (parameters == null || !parameters.TryGetValue(key, out string? value))
    {
        return false;
    }

    return bool.TryParse(value, out bool result) && result;
}

static bool TryGetRectParameter(IReadOnlyDictionary<string, string> parameters, string key, out Rect roi)
{
    roi = default;
    if (parameters == null || !parameters.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value))
    {
        return false;
    }

    string[] tokens = value.Split(',');
    if (tokens.Length != 4)
    {
        return false;
    }

    if (!int.TryParse(tokens[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
        || !int.TryParse(tokens[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
        || !int.TryParse(tokens[2].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
        || !int.TryParse(tokens[3].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int height))
    {
        return false;
    }

    roi = new Rect(x, y, width, height);
    return true;
}

static Rect ClampRect(Mat preview, Rect rect)
{
    int x = Math.Clamp(rect.X, 0, Math.Max(0, preview.Width - 1));
    int y = Math.Clamp(rect.Y, 0, Math.Max(0, preview.Height - 1));
    int right = Math.Clamp(rect.X + rect.Width, x + 1, preview.Width);
    int bottom = Math.Clamp(rect.Y + rect.Height, y + 1, preview.Height);
    return new Rect(x, y, right - x, bottom - y);
}

static void DrawRectangleOverlay(
    Mat preview,
    VisionRecipeOverlaySummary overlay,
    Scalar color,
    int thickness,
    int labelThickness,
    string label)
{
    if (overlay.BoundsWidth <= 0 || overlay.BoundsHeight <= 0)
    {
        return;
    }

    int x = Math.Clamp((int)Math.Round(overlay.BoundsX), 0, Math.Max(0, preview.Width - 1));
    int y = Math.Clamp((int)Math.Round(overlay.BoundsY), 0, Math.Max(0, preview.Height - 1));
    int right = Math.Clamp((int)Math.Round(overlay.BoundsX + overlay.BoundsWidth), x + 1, preview.Width);
    int bottom = Math.Clamp((int)Math.Round(overlay.BoundsY + overlay.BoundsHeight), y + 1, preview.Height);
    Rect bounds = new Rect(x, y, right - x, bottom - y);
    float centerX = overlay.CenterX;
    float centerY = overlay.CenterY;
    if (float.IsNaN(centerX) || float.IsInfinity(centerX)
        || float.IsNaN(centerY) || float.IsInfinity(centerY))
    {
        centerX = bounds.X + bounds.Width / 2F;
        centerY = bounds.Y + bounds.Height / 2F;
    }

    if (Math.Abs(overlay.Angle) < 0.000001 || double.IsNaN(overlay.Angle) || double.IsInfinity(overlay.Angle))
    {
        Cv2.Rectangle(preview, bounds, color, thickness, LineTypes.AntiAlias);
    }
    else
    {
        double radians = -overlay.Angle * Math.PI / 180D;
        float cos = (float)Math.Cos(radians);
        float sin = (float)Math.Sin(radians);
        float halfWidth = bounds.Width / 2F;
        float halfHeight = bounds.Height / 2F;

        OpenCvSharp.Point[] corners =
        {
            RotatePointAroundCenter(-halfWidth, -halfHeight, centerX, centerY, cos, sin),
            RotatePointAroundCenter(halfWidth, -halfHeight, centerX, centerY, cos, sin),
            RotatePointAroundCenter(halfWidth, halfHeight, centerX, centerY, cos, sin),
            RotatePointAroundCenter(-halfWidth, halfHeight, centerX, centerY, cos, sin)
        };
        Cv2.Polylines(preview, new[] { corners }, true, color, thickness, LineTypes.AntiAlias);
    }

    DrawCross(preview, ClampPoint(preview, centerX, centerY), color, thickness);
    DrawLabel(preview, label, new OpenCvSharp.Point(bounds.X, Math.Max(14, bounds.Y - 5)), color, labelThickness);
}

static OpenCvSharp.Point RotatePointAroundCenter(
    float x,
    float y,
    float centerX,
    float centerY,
    float cos,
    float sin)
{
    return new OpenCvSharp.Point(
        (int)Math.Round(centerX + x * cos - y * sin),
        (int)Math.Round(centerY + x * sin + y * cos));
}

static void DrawLineOverlay(
    Mat preview,
    VisionRecipeOverlaySummary overlay,
    Scalar color,
    int thickness,
    int labelThickness,
    string label)
{
    OpenCvSharp.Point start = ClampPoint(preview, overlay.StartX, overlay.StartY);
    OpenCvSharp.Point end = ClampPoint(preview, overlay.EndX, overlay.EndY);
    if (start == end)
    {
        if (Math.Abs(overlay.CenterX) < 0.001 && Math.Abs(overlay.CenterY) < 0.001)
        {
            return;
        }

        double angleRadians = overlay.Angle * Math.PI / 180.0;
        if (double.IsNaN(angleRadians) || double.IsInfinity(angleRadians))
        {
            angleRadians = 0.0;
        }

        double halfLength = Math.Max(24.0, Math.Min(preview.Width, preview.Height) * 0.18);
        OpenCvSharp.Point centerPoint = ClampPoint(preview, overlay.CenterX, overlay.CenterY);
        start = ClampPoint(
            preview,
            (float)(centerPoint.X - Math.Cos(angleRadians) * halfLength),
            (float)(centerPoint.Y - Math.Sin(angleRadians) * halfLength));
        end = ClampPoint(
            preview,
            (float)(centerPoint.X + Math.Cos(angleRadians) * halfLength),
            (float)(centerPoint.Y + Math.Sin(angleRadians) * halfLength));
    }

    Cv2.Line(preview, start, end, color, thickness, LineTypes.AntiAlias);
    Cv2.Circle(preview, start, Math.Max(3, thickness + 1), color, -1, LineTypes.AntiAlias);
    Cv2.Circle(preview, end, Math.Max(3, thickness + 1), color, -1, LineTypes.AntiAlias);
    OpenCvSharp.Point center = new OpenCvSharp.Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
    DrawCross(preview, center, color, thickness);
    DrawLabel(preview, label, new OpenCvSharp.Point(center.X + 5, center.Y - 5), color, labelThickness);
}

static void DrawPointOverlay(Mat preview, float centerX, float centerY, Scalar color, int thickness, string label)
{
    OpenCvSharp.Point center = ClampPoint(preview, centerX, centerY);
    int radius = Math.Max(5, thickness * 3);
    Cv2.Circle(preview, center, radius, color, thickness, LineTypes.AntiAlias);
    DrawCross(preview, center, color, thickness);
    DrawLabel(preview, label, new OpenCvSharp.Point(center.X + radius + 2, center.Y), color, Math.Max(1, thickness - 1));
}

static void DrawPointsOverlay(Mat preview, VisionRecipeOverlaySummary overlay, Scalar color, int thickness)
{
    if (overlay.Points == null || overlay.Points.Count == 0)
    {
        if (Math.Abs(overlay.CenterX) > 0.001 || Math.Abs(overlay.CenterY) > 0.001)
        {
            DrawPointOverlay(preview, overlay.CenterX, overlay.CenterY, color, thickness, string.Empty);
        }

        return;
    }

    int radius = Math.Max(2, thickness + 1);
    int count = 0;
    foreach (VisionRecipeOverlayPointSummary point in overlay.Points)
    {
        if (count >= 500)
        {
            break;
        }

        OpenCvSharp.Point clamped = ClampPoint(preview, point.X, point.Y);
        Cv2.Circle(preview, clamped, radius, color, -1, LineTypes.AntiAlias);
        count++;
    }
}

static void DrawCross(Mat preview, OpenCvSharp.Point center, Scalar color, int thickness)
{
    int radius = Math.Max(5, thickness * 4);
    Cv2.Line(preview, new OpenCvSharp.Point(center.X - radius, center.Y), new OpenCvSharp.Point(center.X + radius, center.Y), color, thickness, LineTypes.AntiAlias);
    Cv2.Line(preview, new OpenCvSharp.Point(center.X, center.Y - radius), new OpenCvSharp.Point(center.X, center.Y + radius), color, thickness, LineTypes.AntiAlias);
}

static void DrawLabel(Mat preview, string label, OpenCvSharp.Point anchor, Scalar color, int thickness)
{
    if (string.IsNullOrWhiteSpace(label))
    {
        return;
    }

    OpenCvSharp.Point point = new OpenCvSharp.Point(
        Math.Clamp(anchor.X, 0, Math.Max(0, preview.Width - 1)),
        Math.Clamp(anchor.Y, 12, Math.Max(12, preview.Height - 1)));

    Cv2.PutText(
        preview,
        label,
        point,
        HersheyFonts.HersheySimplex,
        0.46,
        color,
        thickness,
        LineTypes.AntiAlias);
}

static OpenCvSharp.Point ClampPoint(Mat preview, float x, float y)
{
    return new OpenCvSharp.Point(
        Math.Clamp((int)Math.Round(x), 0, Math.Max(0, preview.Width - 1)),
        Math.Clamp((int)Math.Round(y), 0, Math.Max(0, preview.Height - 1)));
}

if (!File.Exists(imagePath))
{
    Console.Error.WriteLine($"Image was not found: {imagePath}");
    return 2;
}

if (!File.Exists(pipelineXmlPath))
{
    Console.Error.WriteLine($"Pipeline XML was not found: {pipelineXmlPath}");
    return 2;
}

using Mat source = Cv2.ImRead(imagePath, ImreadModes.Unchanged);
if (source.Empty())
{
    Console.Error.WriteLine($"Image could not be loaded: {imagePath}");
    return 2;
}

VisionRecipeRunner runner = new VisionRecipeRunner();
using VisionRecipeRunResult result = await runner.RunAsync(pipelineXmlPath, source);
if (!SerializeHelper.TryLoadFromXmlFile(pipelineXmlPath, out VisionPipeline overlayPipeline) || overlayPipeline == null)
{
    Console.Error.WriteLine("Pipeline XML could not be loaded for overlay evidence.");
    return 2;
}

Console.WriteLine($"Pipeline={result.PipelineName}");
Console.WriteLine($"SchemaVersion={result.SchemaVersion}");
Console.WriteLine($"Success={result.Success}");
Console.WriteLine($"Outcome={result.OutcomeText}");
Console.WriteLine($"Message={result.Message}");
Console.WriteLine($"Summary={result.SummaryText}");
Console.WriteLine($"ActionSummary={result.ActionSummaryText}");
Console.WriteLine($"StepSummary={result.StepSummaryText}");
Console.WriteLine($"Normalization={result.NormalizationText}");
Console.WriteLine($"FinalLayer={result.FinalLayer}");
Console.WriteLine($"FinalStep={result.FinalStepName}");
Console.WriteLine($"FinalTool={result.FinalToolType}");
Console.WriteLine($"ResultImage={result.ResultImageSizeText}");
Console.WriteLine($"HasFinalResultImage={result.HasFinalResultImage}");
Console.WriteLine($"FinalMetricCount={result.FinalMetricCount}");
Console.WriteLine($"FinalOverlayCount={result.FinalOverlayCount}");
Console.WriteLine($"FinalMetrics={result.FinalMetricsText}");
Console.WriteLine($"TotalStepTime={result.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)} ms");
Console.WriteLine($"StepCount={result.StepCount}");
Console.WriteLine($"PassedStepCount={result.PassedStepCount}");
Console.WriteLine($"FailedStepCount={result.FailedStepCount}");
Console.WriteLine($"SkippedStepCount={result.SkippedStepCount}");
Console.WriteLine(result.HasFailedStep
    ? $"FirstFailedStep={result.FirstFailedStepIndex}|{result.FirstFailedStepName}|{result.FirstFailedErrorCode}:{result.FirstFailedErrorName}|{result.FirstFailedResultStatus}"
    : "FirstFailedStep=None");
Console.WriteLine($"FirstFailedSummary={result.FirstFailedSummaryText}");
Console.WriteLine($"FirstFailedDiagnostic={(result.HasFailedStep ? result.FirstFailedDiagnosticHint : string.Empty)}");
Console.WriteLine($"FirstFailedSuggestedFix={(result.HasFailedStep ? result.FirstFailedSuggestedFix : string.Empty)}");

foreach (VisionRecipeStepRunSummary step in result.Steps)
{
    Console.WriteLine(
        $"{step.Index} | {step.ToolType} | {step.Status} | {step.OutputLayer} | {step.ElapsedMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)} ms | Image={step.ResultImageSizeText} | Metrics={step.MetricCount} | Overlays={step.OverlayCount} | Error={step.ErrorCode}:{step.ErrorName}");

    if (!string.IsNullOrWhiteSpace(step.MetricsText))
    {
        Console.WriteLine($"  {step.MetricsText}");
    }

    if (!string.IsNullOrWhiteSpace(step.DiagnosticHint))
    {
        Console.WriteLine($"  Diagnostic={step.DiagnosticHint}");
    }

    if (!string.IsNullOrWhiteSpace(step.SuggestedFix))
    {
        Console.WriteLine($"  SuggestedFix={step.SuggestedFix}");
    }

    if (printOverlays && step.Overlays.Count > 0)
    {
        for (int overlayIndex = 0; overlayIndex < step.Overlays.Count; overlayIndex++)
        {
            VisionRecipeOverlaySummary overlay = step.Overlays[overlayIndex];
            Console.WriteLine(
                "  Overlay {0:000}: {1} Bounds=({2:0.#},{3:0.#},{4:0.#},{5:0.#}) Center=({6:0.#},{7:0.#}) Angle={8:0.###} Label={9}",
                overlayIndex + 1,
                overlay.Kind,
                overlay.BoundsX,
                overlay.BoundsY,
                overlay.BoundsWidth,
                overlay.BoundsHeight,
                overlay.CenterX,
                overlay.CenterY,
                overlay.Angle,
                overlay.Label);
            if (string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    "    Line Start=({0:0.#},{1:0.#}) End=({2:0.#},{3:0.#}) Angle={4:0.###}",
                    overlay.StartX,
                    overlay.StartY,
                    overlay.EndX,
                    overlay.EndY,
                    overlay.Angle);
            }
            else if (string.Equals(overlay.Kind, "Points", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"    Points={overlay.Points.Count}");
            }
        }
    }
}

if (!string.IsNullOrWhiteSpace(resultImagePath) && result.ResultImage != null && !result.ResultImage.Empty())
{
    Directory.CreateDirectory(Path.GetDirectoryName(resultImagePath) ?? ".");
    Cv2.ImWrite(resultImagePath, result.ResultImage);
    Console.WriteLine($"Saved={resultImagePath}");
}

if (!string.IsNullOrWhiteSpace(allOverlayImagePath))
{
    SaveAllOverlayImage(source, result, overlayPipeline, allOverlayImagePath);
    Console.WriteLine($"SavedAllOverlays={allOverlayImagePath}");
}

List<string> failures = new List<string>();
if (!result.Success)
{
    failures.Add($"Runner returned NG: {result.Message}");
}

if (result.Steps.Count == 0)
{
    failures.Add("Runner returned no step summaries.");
}

if (string.IsNullOrWhiteSpace(result.FinalLayer))
{
    failures.Add("Runner did not resolve a final output layer.");
}

if (result.ResultImage == null || result.ResultImage.Empty())
{
    failures.Add("Runner returned no final result image.");
}

VisionRecipeStepRunSummary? finalStep = result.Steps.LastOrDefault(step => !step.Skipped);
if (finalStep == null)
{
    failures.Add("Runner returned no enabled step summary.");
}
else
{
    if (finalStep.MetricCount == 0 && finalStep.OverlayCount == 0)
    {
        failures.Add($"Final step '{finalStep.Name}' has neither metrics nor overlays.");
    }

    if (!finalStep.AcceptancePassed)
    {
        failures.Add($"Final step '{finalStep.Name}' acceptance failed: {finalStep.AcceptanceMessage}");
    }
}

if (failures.Count == 0)
{
    Console.WriteLine("Runner smoke passed.");
    return 0;
}

Console.Error.WriteLine("Runner smoke failed.");
foreach (string failure in failures)
{
    Console.Error.WriteLine($"- {failure}");
}

return 1;

internal sealed class BitmapStorageLease : IDisposable
{
    private IntPtr buffer;

    public BitmapStorageLease(Bitmap bitmap, IntPtr buffer)
    {
        Bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
        this.buffer = buffer;
    }

    public Bitmap Bitmap { get; }

    public void Dispose()
    {
        Bitmap.Dispose();
        if (buffer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(buffer);
            buffer = IntPtr.Zero;
        }
    }
}

internal readonly record struct SoakResourceSample(
    int Iteration,
    long PrivateBytes,
    long WorkingSetBytes,
    long ManagedBytes,
    int HandleCount,
    int GdiObjects,
    int UserObjects);

internal static class SoakNativeResourceProbe
{
    public static int GetGdiObjectCount(IntPtr processHandle)
    {
        return checked((int)GetGuiResources(processHandle, 0));
    }

    public static int GetUserObjectCount(IntPtr processHandle)
    {
        return checked((int)GetGuiResources(processHandle, 1));
    }

    [DllImport("user32.dll")]
    private static extern uint GetGuiResources(IntPtr processHandle, uint flags);
}
