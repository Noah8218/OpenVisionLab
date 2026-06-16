# Library-Noah Public API Plan

Last updated: 2026-06-14

This document defines how Library-Noah should move from legacy internal code to a public OpenCVSharp algorithm library for OpenVisionLab.

The goal is to publish readable algorithm code, not hide it behind wrappers. Compatibility wrappers may exist temporarily, but they are not the final architecture.

## API Policy

| Status | Meaning | Rule |
| --- | --- | --- |
| Public | API that should be documented and shown to users | Keep, test, document, and improve names only with migration. |
| Internal | Implementation detail | Can be renamed or moved if public behavior is preserved. |
| Compatibility | Old API kept only so existing apps/recipes still build | Mark as obsolete after OpenVisionLab no longer uses it. Remove in a later major version or move to a compatibility package. |
| Remove | Unused old API | Remove after build and recipe compatibility checks pass. |

## Public API Direction

The public surface should be centered on these concepts:

- `IVisionTool`
- `OpenCvAlgorithmBase`
- `VisionToolResult`
- `VisionToolOverlay`
- `VisionPipeline`
- `VisionPipelineStep`
- `VisionPipelineRuntime`
- Tool classes: `ThresholdTool`, `MorphologyTool`, `ContourTool`, `BlobTool`, `LineGaugeTool`, `MatchingTool`, `MeanTool`, `SiftTool`
- Result classes: `ContourResult`, `BlobResult`, `LineGaugeResult`, `MatchingResult`, `MeanResult`

These names match the OpenVisionLab direction: tool-based OpenCVSharp algorithms that can be tested individually or chained in a pipeline.

## Compatibility Candidates

These names should not be part of the final public API:

| Legacy API | Target |
| --- | --- |
| `CVContour` | `ContourTool` |
| `CVBlob` | `BlobTool` |
| `CVMatching` | `MatchingTool` |
| `CVSIFT` | `SiftTool` or `FeatureMatchingTool` |
| `CVMean` | `MeanTool` |
| `CResultContour` | `ContourResult` |
| `CResultBlob` | `BlobResult` |
| `CResultMatching` | `MatchingResult` |
| `CResultMean` | `MeanResult` |
| `COpenCVAlgorithmBase` | `OpenCvAlgorithmBase` |
| `COpenCVHelper` | `OpenCvHelper` |
| `CImageConverter` | `BitmapImageConverter` |
| `CBitmapProcessing` | `BitmapProcessing` |
| `CBitmapHelper` | `BitmapHelper` |
| `CConverter` | `CommonConverter` or smaller geometry converters |
| `CFormula` | `FormulaUtil` or `GeometryMath` |
| `CUtil_UI` | `UiUtil` or move to OpenVisionLab UI |
| `LineGuage*` | `LineGauge*` |

`CLOG` is an exception. Keep the short name because it is practical, but simplify the internals and categories later.

## Current OpenVisionLab Reference Snapshot

Observed references from OpenVisionLab source:

| API | Reference count | Direction |
| --- | ---: | --- |
| `CLOG` | 61 | Keep short facade, clean categories later. |
| `BitmapImageConverter` | 51 | Public candidate. |
| `CUtil` | 47 | Split later; too broad to remove immediately. |
| `CommonConverter` | 23 | Public candidate or split geometry helpers. |
| `FormulaUtil` | 21 | Public candidate, may rename to geometry/math later. |
| `BlobResult` | 17 | Public candidate. |
| `LineGaugeTool` | 12 | Public candidate. |
| `ContourTool` | 11 | Public candidate. |
| `CResultBlob` | 0 active direct refs | Compatibility; keep for external/legacy callers. |
| `CVBlob` | 0 active direct refs | Compatibility; keep for external/legacy callers and DLL-based behavior parity. |
| `CVSIFT` | 9 | Compatibility; still used. |
| `CVMean` | 5 | Compatibility; still used. |
| `OpenCvHelper` | 5 | Public candidate. |
| `BlobTool` | 3 | Public candidate; active OpenVisionLab blob execution path. |
| `CImageConverter` | 3 | Compatibility; first app-side cleanup target. |
| `ContourResult` | 3 | Public candidate. |
| `MatchingTool` | 2 | Public candidate. |
| `MeanTool` | 2 | Public candidate. |
| `SiftTool` | 2 | Public candidate. |
| `CBitmapProcessing` | 1 | Compatibility; first app-side cleanup target. |
| `CConverter` | 1 | Compatibility; first app-side cleanup target. |
| `CVContour` | 0 | Compatibility wrapper only; OpenVisionLab does not directly use it. |
| `CResultContour` | 0 | Candidate for obsolete/remove after compatibility decision. |
| `CVLineGuage` | 0 | Candidate for obsolete/remove after XML compatibility decision. |
| `CVMatching` | 0 | Candidate for obsolete/remove after compatibility decision. |

## Work Completed

- `CVContour` now delegates to `ContourTool`, so there is one contour algorithm path.
- Contour sample baseline was captured and verified.
- `FormVision_EdgeDection` was moved from `CImageConverter` / `CBitmapProcessing` / `CConverter` to `BitmapImageConverter` / `BitmapProcessing` / `CommonConverter`.
- OpenVisionLab direct references to `CImageConverter`, `CBitmapProcessing`, and `CConverter` are now zero.
- OpenVisionLab build passed after the app-side helper cleanup.
- `BlobTool` code flow was cleaned without changing `OpenCvSharp.Blob.dll`, package versions, project references, or blob result semantics.
- `CVBlob` was intentionally left in place because it is part of the existing DLL-based compatibility surface.

## Next Refactor Order

1. Add a Blob sample baseline before changing blob behavior.
2. Keep `OpenCvSharp.Blob.dll` unchanged unless there is an explicit version-management task.
3. Treat `CVBlob` and `CResultBlob` as compatibility APIs, not immediate deletion targets.
4. Continue app-side removal of easy legacy helper references.
5. Repeat the public/compatibility classification for `CVSIFT`, `CVMean`, and typo-based `LineGuage` APIs.

## Important Rule

Do not create wrappers as the final design. Use them only when existing OpenVisionLab code or saved XML would otherwise break.

The final published library should expose clean, readable algorithms directly.
