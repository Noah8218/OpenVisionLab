# OpenVisionLab Pipeline Review Image-First Layout

Date: 2026-08-27 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Branch: `codex/public-sample-ux-docs`
Source HEAD at verification: `54f219acf0006c565688780bcc9175b6d3376c2f`

## Status

`Complete` for the bounded Dev Pipeline Review layout slice.

This change makes the selected Step's input/output evidence the primary
workspace and turns the supporting Step Flow and review details into explicit,
reversible disclosures. It does not claim an EXE launch, release, deployment,
full theme/DPI matrix, or original-repository promotion.

## Design decision

The previous bottom-clipping correction made the detail area reachable, but its
fixed-height detail row still competed with the image canvas on shorter windows.
The operator's main question in Pipeline Review is “what image did this Step
receive and what did it produce?” Therefore the default layout now reserves the
star-sized center area for the two previews:

```text
Pipeline Review
  readiness / guide summary
  [optional Step Flow rail] | selected Step + Input / Output images
  [collapsed Review details drawer]
```

- The selected Step remains visible in the left rail, while its Input and
  Output previews occupy the central area at the largest available height.
- `Review details` is a closed drawer by default. Opening it exposes the
  existing Flow, Parameters, Validation, Result, Run Log, and diagnostic tabs;
  no information was deleted.
- `Step Flow` has an explicit collapse control. Collapsing it reduces the rail
  to a narrow icon strip so the image area can be inspected without changing
  the selected Step.
- On compact views below 650 pixels of view height, the summary cards are
  already omitted; the collapsed detail drawer preserves that image-first
  behavior instead of consuming the remaining preview height.

This is intentionally a one-selected-Step viewer rather than a full-size image
storyboard for every Pipeline step. A storyboard can be considered later only
if a named operator task demonstrates that one-up inspection is insufficient.

## Implemented scope

### `OpenVisionPipelineReviewView.xaml`

- Added a named Step Flow rail and a local `StepFlowToggleStyle` using the
  existing Pipeline Review semantic colors, including hover, pressed, checked,
  focused, and disabled visual states.
- Added `PipelineReviewDetailsToggle` as the compact drawer header and moved the
  existing detail `TabControl` into the drawer content row. The existing detail
  tabs and scrollable result/object content remain available when opened.
- Kept all existing image controls, zoom/pan, route cards, diagnostics, and
  result evidence intact.

### `OpenVisionPipelineReviewView.xaml.cs`

- Maintains detail-drawer and Step Flow state locally in the view; both controls
  update only layout and icon/tooltip presentation.
- Hides the detail content and splitter while the drawer is closed, leaving a
  34-pixel review-details header; restores the prior diagnostic-specific detail
  heights when opened.
- Collapses the Step Flow rail from 300 to 44 pixels without changing the
  selected Step, layer, route, or execution state.
- Reapplies the localized labels and tooltips after a language change.

### Localization

Added Korean/English catalog entries for the drawer and Step Flow tooltips in
`src/Libraries/OpenVisionLab.Localization/Resources/LocalizationCatalog.tsv`.

### Focused smoke contract

`tools/PipelineViewerScreenshotSmoke/Program.cs` now checks that:

- the detail drawer starts closed and the Input preview retains useful height;
- the drawer opens and reaches its existing detail content;
- the Step Flow rail collapses and restores;
- the toggles do not increment Preview/Run, change layer count or active layer,
  or change native input/output routing;
- the same assertions hold before and after OK/NG review and for the Object
  Results review target;
- a dedicated `1280x800` compact target exercises the compact layout.

## Current-build evidence

All captures below were generated after the latest source/build changes. They
are current-source WPF view captures, not EXE launch evidence.

| Check | Result | Evidence |
| --- | --- | --- |
| Korean normal OK, `1600x900` | Pass | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-image-first-20260827-final-normal\wpf_shell_host_pipeline_review.png` |
| Korean normal NG, `1600x900` | Pass | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-image-first-20260827-final-ng\wpf_shell_host_pipeline_review_ng.png` |
| Korean Object Results, `1600x900` | Pass | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-image-first-20260827-final-object\wpf_shell_host_workspace_sample_pipeline_review_metrics.png` |
| Korean compact, `1280x800` | Pass | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-image-first-20260827-final-compact\wpf_shell_host_pipeline_review_image_first_compact.png` |
| English Pipeline Review, `1600x900` | Pass | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-image-first-20260827-final-en\manual_en_pipeline_review.png` |
| Localization catalog contract | Pass | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-image-first-20260827-final-localization\localization_catalog_contract_check.png` |

## Commands and results

```powershell
dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" --nologo --verbosity:minimal
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestDocumentationIndex.ps1
git diff --check
```

Result: both builds completed with `0` warnings and `0` errors. Readiness,
vendored-reference, public-sample, documentation-index, and whitespace checks
also passed. The current smoke executable is:

`C:\Git\OpenVisionLab_Dev\tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.exe`

The following isolated targets all returned `OK`:

```text
wpf_shell_host_pipeline_review=OK ... size=1600x900
wpf_shell_host_pipeline_review_ng=OK ... size=1600x900
wpf_shell_host_workspace_sample_pipeline_review_metrics=OK ... size=1600x900
wpf_shell_host_pipeline_review_image_first_compact=OK ... size=1280x800
manual_en_pipeline_review=OK ... size=1600x900
localization_catalog_contract_check=OK ... size=760x420
```

## Regression and verification boundary

Verified in the current source build: Korean normal OK/NG/Object, Korean
compact `1280x800`, English `1600x900`, localized catalog presence, visible
drawer open/close, Step Flow collapse/restore, and no execution/layer/route
side effects from either toggle.

Not verified by this report: actual desktop EXE launch smoke, mouse hardware
interaction recording, dark/light theme matrix, Windows DPI `125%`, `150%`,
`175%`, `200%`, minimum/maximize/monitor movement, every diagnostic tab at
every compact size, original-repository changes, commit/push, release
publication, installation, rollout, and deployment.
