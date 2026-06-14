# OpenVisionLab Status And Next Steps

Updated: 2026-06-14

## Product Direction

OpenVisionLab is moving toward a rule-based vision recipe workbench.

The final shape should let a user:

- Load an image and inspect it through layers, ROI, coordinates, pixels, and zoom/pan.
- Tune OpenCvSharp tools with immediate preview.
- Build a pipeline where every step has a clear input image and output image.
- Validate the pipeline through overlays, metrics, acceptance criteria, and logs.
- Save the approved recipe as XML.
- Run the same XML from the main UI, pipeline UI, batch/samples, AI Recipe import, and an external runner/DLL.

The key UX principle is that every detail should reduce user uncertainty. A user should always know:

- Which image is being read.
- Which layer will be written.
- Whether a step is chained or intentionally branched.
- Whether the result is only a preview or published to the main workspace.
- Why a step is OK, NG, or needs review.

## Work Completed In This Pass

Latest pipeline clarity update:

- Add Step normal flow now treats the previous enabled step output as the default next input.
- Branch input confirmation now reads as `Allow branch input`.
- Pipeline Flow input labels are clearer:
  - `SOURCE`: first/source image input.
  - `PREV OUT`: normal chained input from the previous step output.
  - `BRANCH IN`: intentionally reading from a different layer.
- Duplicating a step now creates a chained copy after the selected step instead of preserving an ambiguous old input.
- Selected-step preview is clearer:
  - Preview caption now shows `Preview - MODE | Layer`.
  - Result Details has a `Viewing` row.
  - Clicking `Input image`, `Output image`, or `Overlays` switches the preview mode.
  - Pipeline Flow highlights the selected input/output pill more visibly.
- UI screenshot smoke was run for `pipeline_form`, `pipeline_form_branch`, `pipeline_add_step_form`, and `pipeline_add_step_branch_form`; all returned `OK`.

Latest sample catalog and platform validation update:

- Added `docs/samples/OpenVisionLab.SampleCatalog.csv` as the first shared benchmark catalog.
- Added `tools/RunVisionSampleCatalog.ps1` so sample images can be validated from the command line without opening the UI.
- Pipeline Samples now has a `Recipe Catalog` tab.
- Opening a catalog sample loads the sample image to `Main`, imports the recommended pipeline XML, shows the expected metric in the run log, and starts Run Preview.
- The existing saved workspace sample workflow remains available under `Saved Workspace`.
- The sample catalog now stores expected metric checks through:
  - `ExpectedMetricName`
  - `ExpectedMetricMinimum`
  - `ExpectedMetricMaximum`
- `tools/RunVisionSampleCatalog.ps1` now fails required samples when the expected metric is missing or outside the expected range.
- Added sample-family recipe baselines:
  - `docs/samples/Rice_Particle_Contour.pipeline.xml`
  - `docs/samples/Pin_Feature_Contour.pipeline.xml`
  - `docs/samples/BentPin_LargeContour.pipeline.xml`
  - `docs/samples/DiePad_Surface_Contour.pipeline.xml`
- `tools/RunVisionPlatformPrecheck.ps1` now runs build, XML compatibility, sample catalog validation, and selected UI smoke as one platform-level check.
- Default UI precheck coverage now includes:
  - `main_workspace`
  - `pipeline_form`
  - `pipeline_form_branch`
  - `pipeline_add_step_form`
  - `pipeline_add_step_branch_form`
  - `threshold_form`
  - `ai_recipe_form`
- Message box smoke targets are still available explicitly, but are no longer included in the default UI precheck.
- UI precheck should be scoped to the changed surface whenever possible. For example, main-view-only work should run `main_workspace` instead of every UI target.
- The LLM Recipe prompt now references the sample catalog and explicitly warns against accidentally branching back to `Main` or an older layer.
- Required sample catalog runs currently pass for:
  - `Contour_TextSymbols`
  - `Contour_Generic`
  - `Rice_Particle`
  - `Pins_Feature`
  - `BentPin_Large`
  - `DiePad1_Surface`
  - `DiePad2_Surface`
  - `DiePad3_Surface`
  - `DiePad4_Surface`

Current expected sample metrics:

| Sample | Recipe | Expected | Current |
| --- | --- | --- | --- |
| Contour_TextSymbols | Contour_TextSymbols | ResultCount 35-80 | 51 |
| Contour_Generic | Threshold_Morphology_Contour | ResultCount 10-30 | 21 |
| Rice_Particle | Rice_Particle_Contour | ResultCount 100-170 | 123 |
| Pins_Feature | Pin_Feature_Contour | ResultCount 40-70 | 54 |
| BentPin_Large | BentPin_LargeContour | ResultCount 1-5 | 2 |
| DiePad1_Surface | DiePad_Surface_Contour | ResultCount 8-25 | 11 |
| DiePad2_Surface | DiePad_Surface_Contour | ResultCount 8-25 | 14 |
| DiePad3_Surface | DiePad_Surface_Contour | ResultCount 8-25 | 16 |
| DiePad4_Surface | DiePad_Surface_Contour | ResultCount 8-25 | 14 |

1. Test process cleanup
   - Added `tools/StopUiSmoke.ps1`.
   - It only targets `PipelineViewerScreenshotSmoke.exe`.
   - It does not stop `OpenVisionLab.exe`.
   - Current environment still has smoke processes that cannot be stopped automatically because Windows returns access denied.

2. UI smoke execution safety
   - `tools/RunUiScreenshotSmoke.ps1` now builds the smoke executable first, runs selected targets by default, and applies a timeout.
   - `tools/RunUiPrecheck.ps1` no longer runs `--all` by default.
   - `-All` is now explicit.
   - The precheck report records targets, timeout, raw output, and image links.

3. Pipeline Check UX
   - Branch and duplicated preprocessing messages now use review language instead of a hard warning tone.
   - Check logs now use `CHECK REVIEW` for review items.
   - The UI message says the flow is valid but review is recommended when the pipeline has intentional branch-like behavior.

4. Branch flow stabilization
   - Branch input is treated as a review item when a step reads a different layer than the previous step output.
   - This matches the current UX direction: branching is allowed, but the user should confirm it intentionally.

5. Add Step / chain UX coverage
   - Smoke targets exist for:
     - `pipeline_add_step_form`
     - `pipeline_add_step_branch_form`
     - `pipeline_form_branch`
     - `pipeline_form_branch_check`
   - The branch check target validates branch-review behavior without opening the full pipeline form.

6. Threshold form coverage
   - `threshold_form` remains included in the default UI smoke target list.
   - Full visual confirmation is deferred until the stale smoke processes are manually closed.

7. Build verification
   - `tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj` builds successfully.
   - `OpenVisionLab.sln` builds successfully.
   - The first build attempt failed only because the sandbox blocked SDK cache access, not because of source errors.

## Current Verification Status

| Area | Status | Note |
| --- | --- | --- |
| Solution build | OK | Full Debug / Any CPU build passed. |
| Smoke project build | OK | Screenshot smoke tool builds. |
| UI smoke script safety | OK | Timeout and selected-target defaults added. |
| Stale smoke process cleanup | Partial | Script exists, but current processes return access denied. |
| Pipeline Check message logic | OK | Code confirms review wording and log level mapping. |
| Branch validation | OK | UI smoke target passed. |
| Add Step UX smoke targets | OK | UI smoke targets passed. |
| Pipeline Samples catalog UX | OK | Scoped smoke target `pipeline_samples_form` passed. |
| Threshold visual smoke | OK | Default UI precheck target passed. |
| AI Recipe form smoke | OK | Default UI precheck target passed. |
| Message box smoke | Optional | Available as explicit smoke targets, not included in default UI precheck. |
| Sample catalog runner | OK | 9 required sample rows passed. |
| Platform precheck | OK | Build, XML, samples, and UI precheck passed together. |

## Completion Estimate

These are practical estimates for product readiness, not code quantity.

| Area | Completion | Remaining Work |
| --- | ---: | --- |
| Main viewer and layer workspace | 70% | Final polish, empty states, consistency checks. |
| Tool standardization | 70% | Ensure all legacy forms use the same RunVisionStep path. |
| Pipeline UX | 75% | Input/output clarity, branch review, Add Step refinement. |
| Pipeline persistence and samples | 80% | Restart/load edge cases and sample image policy. |
| Result metrics and overlays | 60% | More consistent metric naming and overlay summaries. |
| Logging and message UX | 60% | Final level/type policy and simplified default mode. |
| Threshold/WPG editors | 60% | Shared editor templates and designer-safe controls. |
| AI Recipe workflow | 58% | Validation loop, guided XML generation, and catalog-driven examples. |
| External runner/DLL path | 50% | Stable public API and packaging/versioning policy. |
| Algorithm robustness | 58% | Add ROI, line/angle, and failure-case recipes per inspection goal. |
| Automated UI QA | 72% | More capture targets and visual regression thresholds. |

## Immediate Next Decisions

After the current UX pass is verified, choose one of these tracks.

1. Pipeline clarity track
   - Make input/output image flow even more explicit.
   - Add a step detail surface that shows input image, output image, output layer, and branch reason.
   - Improve Add Step so the recommended input defaults to the previous step output, while branch input requires explicit confirmation.

2. Algorithm reliability track
   - Use `Sample/Contour.jpg` as the first benchmark.
   - Create stable recipes for text/symbol contour detection.
   - Store expected metrics such as result count, area range, and elapsed time.

3. Threshold and WPG editor track
   - Move range threshold and threshold-with-invert editors into the shared WPG/control library.
   - Keep forms designer-friendly.
   - Make Threshold form and pipeline property grid use the same editor behavior.

4. AI Recipe track
   - Define the LLM prompt and XML schema contract.
   - Let LLM generate a first-pass pipeline XML.
   - OpenVisionLab validates, previews, and highlights review items before users accept it.

5. External execution track
   - Harden `VisionRecipeRunner`.
   - Define the DLL/API surface.
   - Guarantee that UI-created XML runs without UI dependencies.

## Recommended Next Step

The best next step is the Algorithm reliability track, while keeping the Pipeline clarity UX polished.

Reason:

- The pipeline UX now explains input/output flow well enough to start validating real inspection behavior.
- The sample catalog gives us repeatable images, XML, overlays, and metrics.
- AI Recipe quality depends on having reliable sample-backed recipes to imitate.

Recommended concrete work:

1. Add per-sample result review UI that shows expected metric versus actual metric after preview.
2. Add stronger recipes for bent pin and die pad using ROI, line/angle, or edge checks instead of contour count only.
3. Feed the same sample catalog into AI Recipe so generated XML can follow known good patterns.
4. Add a small `Learn` or `Recipe Guide` panel that explains why each sample uses its threshold/morphology/contour settings.
5. Keep UI smoke scoped to the changed surface; only run full capture when checking cross-window regressions.

## Manual Action Before Next UI Smoke

Close these stale processes from Task Manager if they are still present:

- `PipelineViewerScreenshotSmoke.exe`

Do not close `OpenVisionLab.exe` unless it is the user's test instance.

After that, run:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1
```

Use this only when full visual capture is needed:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -All
```

Use scoped targets for focused UI work:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -Targets main_workspace
```

Platform precheck can pass the same scoped UI target:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunVisionPlatformPrecheck.ps1 -UiTargets main_workspace
```
