# Library-Noah Refactor Audit

Last updated: 2026-06-14

## Purpose

Library-Noah is now an external dependency candidate for OpenVisionLab. Before improving algorithms or renaming APIs, we need a clear refactor baseline because several APIs still look like legacy production code and are already referenced widely by OpenVisionLab.

The goal is not only to make names prettier. The goal is to make the library safe to maintain, easier to teach, and aligned with OpenVisionLab's direction as an OpenCVSharp rule-based vision recipe platform.

## Current Project Map

| Project | Role | Notes |
| --- | --- | --- |
| `Lib.Common` | Common utilities, image conversion, geometry, formula, logging | `netstandard2.0`; references `OpenCvSharp`, `System.Drawing.Common`, `System.Windows.Forms`, `WindowsBase`, `log4net`, `System.IO.Ports`. Contains both new and legacy duplicate helpers. |
| `Lib.OpenCV` | OpenCVSharp tools, properties, results, pipeline runtime | `netstandard2.0`; depends on `Lib.Common`; contains both new `*Tool` APIs and legacy `CV*` / `CResult*` APIs. |
| `Lib.OpenCV.Blob` | Blob tool and blob result model | `netstandard2.0`; depends on `Lib.Common` and `Lib.OpenCV`; contains both `BlobTool` and legacy `CVBlob`. |

Observed source inventory:

| Project | C# files |
| --- | ---: |
| `Lib.Common` | 25 |
| `Lib.OpenCV` | 59 |
| `Lib.OpenCV.Blob` | 5 |

Observed public/internal type inventory:

| Project | Classes | Interfaces | Enums | Structs |
| --- | ---: | ---: | ---: | ---: |
| `Lib.Common` | 11 | 0 | 7 | 2 |
| `Lib.OpenCV` | 44 | 12 | 5 | 0 |
| `Lib.OpenCV.Blob` | 4 | 1 | 0 | 0 |

## Existing Good Direction

The library already has the right new axis:

- `IVisionTool`
- `VisionToolResult`
- `VisionToolOverlay`
- `OpenCvAlgorithmBase`
- `VisionPipeline`
- `VisionPipelineRuntime`
- `VisionPipelineStep`
- `ContourTool`, `BlobTool`, `LineGaugeTool`, `MatchingTool`, `ThresholdTool`, `MorphologyTool`

This means the refactor does not need to start from zero. We should stabilize this newer API and slowly move legacy code behind it.

## Main Problems Found

### 1. Legacy API and new API coexist

Examples:

| Legacy style | Newer style |
| --- | --- |
| `CImageConverter` | `BitmapImageConverter` |
| `CBitmapProcessing` | `BitmapProcessing` |
| `CConverter` | `CommonConverter` |
| `COpenCVHelper` | `OpenCvHelper` |
| `COpenCVAlgorithmBase` | `OpenCvAlgorithmBase` |
| `CVContour` | `ContourTool` |
| `CVBlob` | `BlobTool` |
| `CVMatching` | `MatchingTool` |
| `CVSIFT` | `SiftTool` |
| `CResultContour` | `ContourResult` |
| `CResultBlob` | `BlobResult` |
| `CResultMatching` | `MatchingResult` |
| `CFormula` | `FormulaUtil` |
| `CUtil_UI` | `UiUtil` |

This is usable short term, but it creates two ways to do the same thing. It also makes OpenVisionLab harder to explain as a learning platform.

### 2. Typo and old naming are in public contracts

Examples:

- `LineGuage` typo appears in public names.
- `CVLineGuage_Resultt` has both `Guage` and `Resultt`.
- OpenVisionLab still has XML compatibility around `CPropertyLineGuage`.

These cannot be removed abruptly because saved recipes and property XML may depend on them. They should be handled with compatibility aliases and migration.

### 3. UI dependencies are inside core libraries

The library projects reference:

- `System.Windows.Forms`
- `WindowsBase`
- `System.Drawing.Common`

For an algorithm library, this is too UI-heavy. OpenVisionLab itself can use WinForms/WPF, but Library-Noah should aim for UI-neutral core contracts where possible.

Recommended direction:

- Keep image/geometry/result models in the library.
- Move editor/UI-specific helpers to an adapter package or OpenVisionLab.
- Avoid new direct WinForms dependencies in algorithm code.

### 4. Logging is still production-equipment shaped

`CLOG.LOG` currently contains categories such as:

- `NORMAL`
- `ABNORMAL`
- `COMM`
- `IO`
- `MOTION`
- `SEQ`
- `ALARM`
- `INTERLOCK`
- `DEVICE`
- `TEACHING`
- `CONFIG`
- `LOT`

This makes sense for equipment software, but OpenVisionLab's current direction needs simpler categories:

- `Info`
- `Warn`
- `Error`
- `Vision`
- `Pipeline`

Do not remove `CLOG` immediately. The short name is practical and already used. Instead, preserve `CLOG` as a facade and internally route to a cleaner logging model.

### 5. Exception handling and lifecycle need cleanup

Observed patterns:

- `catch (` appears 35 times.
- `throw new NotImplementedException` appears 4 times.
- `Thread.Sleep` appears 4 times.
- `DllImport` appears 6 times.
- `unsafe` appears 6 times.

Not all of these are wrong, but each one needs review. In particular, algorithm code should not silently swallow exceptions or only log them through `CLOG`; pipeline execution needs structured failure results.

### 6. Mat ownership is not explicit enough

Many tools keep mutable fields:

- `imageSource`
- `imageResult`
- `imageTemplate`
- `results`
- `property`

This is convenient for existing forms, but it makes preview, pipeline, retry, batch execution, and threading harder. New APIs should prefer:

- immutable input options,
- explicit `Mat` ownership rules,
- result objects that own or clone output images clearly,
- tool instances that can be reused safely or treated as single-run objects.

### 7. README encoding is broken

`C:\Git\Library-Noah\README.md` appears garbled when read from the current shell. Before publishing or package distribution, rewrite it in UTF-8 and align it with OpenVisionLab's current positioning.

## OpenVisionLab Reference Impact

OpenVisionLab references Library-Noah broadly:

- Main/teaching UI uses `Lib.Common`, `Lib.OpenCV`, `Lib.OpenCV.Pipeline`.
- Vision forms use `BlobTool`, `ContourTool`, `LineGaugeTool`, `MatchingTool`, `MeanTool`, `ThresholdTool`, `MorphologyTool`.
- Pipeline services use `VisionPipeline`, `VisionPipelineRuntime`, `VisionToolResult`, `VisionPipelineStep`.
- Legacy helpers are still referenced in places such as image edit, edge detection, account/lot utilities, and log view.

This means the safest migration strategy is:

1. Keep existing public APIs compiling.
2. Add cleaner APIs beside them.
3. Move OpenVisionLab call sites gradually.
4. Mark legacy APIs as `[Obsolete]` only after replacements are proven.
5. Remove only after recipe/XML compatibility is handled.

## Refactor Target Architecture

Recommended package direction:

| Future Area | Responsibility |
| --- | --- |
| `Noah.Vision.Core` | Geometry, ROI, image metadata, result contracts, metrics, overlays, logging abstraction. |
| `Noah.Vision.OpenCv` | OpenCvSharp image conversion, channel conversion, threshold, morphology, contour/blob/line/matching execution. |
| `Noah.Vision.Pipeline` | Pipeline model, runtime, validation, recipe import/export, acceptance evaluation. |
| `Noah.Vision.Compatibility` | Old `Lib.*`, `C*`, `CV*`, `LineGuage` compatibility wrappers and XML migration helpers. |
| `OpenVisionLab` | WinForms/WPF UI, property editors, image canvas, sample catalog, teaching UX. |

We do not need to rename projects immediately. This can start internally with folders/namespaces and later become packages.

## Suggested Rename Map

| Current | Target |
| --- | --- |
| `CLOG` | Keep as facade; internally introduce `VisionLog` / `LogEventCategory`. |
| `CLOG.LOG.NORMAL` | `Info` or `General` |
| `CLOG.LOG.ABNORMAL` | `Error` |
| `CLOG.LOG.INSP` | `Vision` |
| `CLOG.LOG.CONFIG` | `System` or `Config` |
| `CImageConverter` | `BitmapImageConverter` |
| `CBitmapHelper` | `BitmapHelper` |
| `CBitmapProcessing` | `BitmapProcessing` |
| `CConverter` | `CommonConverter` or split to `GeometryConverter` |
| `CFormula` | `FormulaUtil` or `GeometryMath` |
| `CUtil` | split into `PathUtil`, `EnumUtil`, `FileUtil`, `RecipePathUtil` |
| `CUtil_UI` | `UiUtil` or move out of Library-Noah |
| `COpenCVHelper` | `OpenCvHelper` |
| `COpenCVAlgorithmBase` | `OpenCvAlgorithmBase` |
| `CVContour` | `ContourTool` |
| `CVBlob` | `BlobTool` |
| `CVMatching` | `MatchingTool` |
| `CVSIFT` | `SiftTool` or `FeatureMatchTool` |
| `CVMean` | `MeanTool` |
| `CResultContour` | `ContourResult` |
| `CResultBlob` | `BlobResult` |
| `CResultMatching` | `MatchingResult` |
| `CVLineGuage_*` | `LineGauge*` |

## Algorithm Refactor Priority

### Priority 1: Contour

Reason:

- It is central to current LLM recipe experiments.
- It already participates in pipeline preview/result overlays.
- It has both old `CVContour` and new `ContourTool`.
- It exposes metrics that can become the baseline for acceptance rules.

Work:

- Extract threshold preprocessing into a shared helper.
- Make ROI handling explicit and tested.
- Make contour filtering output deterministic.
- Add result metrics: count, min/max/avg area, angle, bounding boxes.
- Keep `CVContour` as wrapper around `ContourTool` while migrating call sites.

### Priority 2: Blob

Reason:

- Similar shape to contour.
- Exists in a separate project, so dependency boundaries can be improved.

Work:

- Align result contract with `ContourResult`.
- Remove duplicate old/new execution logic where possible.
- Confirm OpenCvSharp.Blob dependency and replacement options.

### Priority 3: LineGauge

Reason:

- Public typo exists.
- Geometry/edge fitting is more fragile and needs tests before renaming.

Work:

- Introduce correctly named `LineGauge*` contracts.
- Keep XML compatibility for `LineGuage`.
- Add deterministic sample-based tests for line detection and intersection.

### Priority 4: Matching / SIFT

Reason:

- Likely more performance-sensitive.
- Template image ownership and rotation/scale search need clearer contracts.

Work:

- Split template setup from execution.
- Define score, angle, bounds, and failure messages consistently.
- Benchmark rotation/scale ranges.

### Priority 5: Common utilities and logging

Reason:

- Used everywhere.
- Must be done after algorithm baseline so behavior does not drift invisibly.

Work:

- Keep `CLOG` short name but simplify enum/category model.
- Split `CUtil` into smaller classes.
- Remove UI dependencies from common algorithm paths.
- Replace duplicate `C*` helpers with compatibility wrappers.

## Safety Plan

Before changing algorithm behavior:

1. Build Library-Noah standalone.
2. Build OpenVisionLab against Library-Noah.
3. Run sample catalog pipelines.
4. Save baseline reports for `Contour.jpg` and other sample images.
5. Capture expected result counts and key metrics.
6. Refactor one tool at a time.
7. Compare before/after metrics and preview overlays.

Any algorithm refactor without before/after sample metrics is risky because a small preprocessing change can make OpenVisionLab look broken even if the code is cleaner.

Current contour baseline:

- See `docs/LIBRARY_NOAH_CONTOUR_BASELINE.md`.
- Captured on 2026-06-14.
- All required sample catalog rows passed before contour refactoring.

Public API direction:

- See `docs/LIBRARY_NOAH_PUBLIC_API_PLAN.md`.
- Compatibility wrappers are temporary only.
- Final published algorithms should be exposed through clean `*Tool` and `*Result` APIs.

## Commercial / Provenance Note

If some code came from a previous company, do not rely on renaming as a cleanup strategy. Renaming does not remove copyright or trade-secret risk.

Recommended handling:

- Classify files by origin: authored here, adapted from public source, adapted from previous work, unknown.
- For unknown or previous-company-origin code, prefer clean-room rewrite of the algorithm based on public OpenCVSharp documentation and tests.
- Keep notes of what was rewritten and why.
- Get legal/business confirmation before publishing the library externally or using it commercially.

This is a technical risk note, not legal advice.

## Proposed Next Work

1. Create `Library-Noah API Inventory` with public type list and OpenVisionLab usage count.
2. Build a compatibility rename map for old names, especially `C*`, `CV*`, and `LineGuage`.
3. Add baseline sample tests for `Contour`, `Blob`, `LineGauge`, and `Matching`.
4. Refactor `ContourTool` first and make `CVContour` call the new implementation. Done for `CVContour` on 2026-06-14.
5. Clean up `CLOG` as a facade while keeping the short name and reducing categories.

Recommended first implementation task:

> Start with `ContourTool` because it is already part of the LLM recipe direction and current pipeline validation. Make the behavior measurable first, then refactor internals without changing the OpenVisionLab user experience.
