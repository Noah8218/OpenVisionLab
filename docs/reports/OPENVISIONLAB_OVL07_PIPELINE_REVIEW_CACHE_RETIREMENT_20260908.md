# OpenVisionLab OVL-07 Pipeline Review review-cache retirement / Reset-Close owner boundary — 2026-09-08

## Scope

This slice keeps `OpenVisionPipelineReviewExecutionController` as the single
owner of review-run summaries and cached output images, but gives that owner one
consistent cache lifetime boundary. Cached output is now exposed only as a
caller-owned clone, and cache replacement plus Reset/Close retirement are
serialized with `executionSync`.

The existing execution generation, cancellation, stale callback rejection,
Recipe/XML exchange, Layer routing, and explicit Preview/Run behavior are not
redesigned here. Display-layer lease acquisition remains with the preceding
`OpenVisionPipelineReviewLayerImageOwner` slice.

## Responsibility and call-path change

| Concern | Before | Current |
| --- | --- | --- |
| Cached output read | `ResolveCachedOutput` returned the controller's internal `Bitmap` | `AcquireCachedOutputSnapshot` clones the entry while holding `executionSync` |
| Cache replacement | Existing image disposal and dictionary update were independent of cache reads | `ReplaceReviewLayerImage` performs conditional replacement and disposal under the same lock |
| Reset/Close retirement | `ClearState` mutated and disposed the dictionary without the cache read boundary | `ClearState` clears summaries and disposes cached images under `executionSync` |
| Summary dictionary lifecycle | Reads/writes could overlap Reset/Close clearing | Summary reads and writes use the same controller synchronization boundary |
| Pipeline Review image owner | Received a borrowed cache value | Receives the snapshot API; it disposes the provider snapshot after its own clone |

The resulting path is:

`Document -> LayerImageOwner -> ExecutionController.AcquireCachedOutputSnapshot -> caller-owned clone`

No second cache, interface, wrapper, View, Window, or dispatcher boundary was
introduced. The state owner remains the execution controller; its callers no
longer observe or dispose the internal cache image.

## Changed files

- `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Execution/OpenVisionPipelineReviewExecutionController.cs`
  - Replaces the borrowed cached-output resolver with a synchronized snapshot
    method, serializes cache replacement/retirement, and protects summary
    dictionary access during lifecycle clearing.
- `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Presenters/OpenVisionPipelineReviewLayerImageOwner.cs`
  - Consumes the controller snapshot provider and closes the provider clone
    after creating the owner-returned image.
- `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`
  - Wires the Layer image owner to `AcquireCachedOutputSnapshot`.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - Adds `--pipeline-review-cache-lifetime-contract`, updates the existing
    execution contract to the snapshot API, and keeps the layer-owner contract
    provider-owned.
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
  - Records the completed cache-lifetime slice and next priority.
- `docs/LLM_DOCUMENT_INDEX.json`
  - Routes Pipeline/ownership work to this report.

## Preserved contracts

- Review output replacement still prefers the current run's rendered result and
  only-if-missing completion fallback.
- A completed Review still populates the same summary and output Layer data.
- Reset and Close still invalidate the active generation and retire Review cache
  state; already returned snapshots remain usable by their callers.
- Recipe/XML serialization, Layer names, explicit Preview/Run, View setter
  order, and existing display-layer lease snapshots remain unchanged.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-cache-retirement-20260908`

- OpenVisionLab app Debug and Release builds passed sequentially with 0
  warnings and 0 errors. An initial intentionally concurrent build emitted a
  known OpenCvSharp file-lock warning; it was not used as the gate and the
  sequential reruns passed.
- VisionRecipeRunnerSmoke Debug and Release builds passed sequentially with 0
  warnings and 0 errors.
- `--pipeline-review-cache-lifetime-contract` passed in Debug and Release. It
  verified replacement, Reset retirement, Close retirement, and continued use
  of snapshots already returned to callers.
- Existing `--pipeline-review-layer-image-owner-contract` passed in Debug and
  Release after the provider API change.
- Existing Pipeline Review execution, ImageSpace lease snapshot, and Recipe
  pipeline exchange projection contracts passed in Release.
- `OpenVisionLab.sln` Debug/Any CPU and `OpenVisionReadinessCheck` Debug passed;
  readiness reported all 13/13 contracts passing.
- `PipelineViewerScreenshotSmoke` Release build passed with 0 warnings and 0
  errors.
- Release Pipeline Review normal and acceptance-NG UI smoke both passed with
  `check=OK`, `layout=0|text=0|internal=0`, `1600x900`. Fresh normal and NG
  captures were visually inspected.
- Dynamic monitor evidence recorded one `\\.\DISPLAY2`, bounds
  `0,0,1920x1080`, working area `0,0,1920x1032`; no target process remained
  after smoke.
- Static ownership proof passed at
  `static-cache-retirement-proof.log`, including synchronized snapshot clone,
  replacement, clear, summary access, owner wiring, and removal of the raw
  resolver.
- Documentation index validation passed with `IndexedPaths=181`, `Routes=13`,
  and `RootRedirects=102`; final `git diff --check` and untracked-target
  whitespace checks passed.

No source reset, clean, checkout, commit, push, merge, release, or deployment
was performed. The full theme/layout/DPI (100/125/150/175/200%), multi-monitor,
hover/pressed/focus, resize, keyboard, and long-run UI matrix was not run. A
separate before-baseline capture was not available for this slice.

## Refactor proof

- **Current owner:** the execution controller owned `reviewLayerImages`, but
  cache reads returned internal images while replacement and ClearState disposed
  them without a shared synchronization boundary.
- **Intended owner:** the same controller owns the cache and returns clones under
  `executionSync`; replacement and retirement use that lock; callers own only
  the returned snapshots.
- **Dependency direction:** Document -> Layer image owner -> controller snapshot
  API -> controller cache state. The View remains the UI clone/dispose owner.
- **State/data owner:** `reviewLayerImages` and `stepResultSummaries` remain
  controller state; no persistent image state moved into the Document or View.
- **Observable contract:** display/cache preference, Layer routing,
  Recipe/XML, explicit Preview/Run, execution generation, and stale-result
  behavior remain preserved.

Status: Complete
Scope: OVL-07 Pipeline Review review-cache retirement/Reset-Close owner boundary.
Acceptance criteria: internal cached images are never returned directly,
replacement and Reset/Close disposal share one synchronization boundary,
returned snapshots survive retirement, existing contracts remain stable, and
focused Debug/Release plus representative UI verification pass.
Verification: app/runner/solution/readiness builds, cache-lifetime and layer-owner
contracts, existing execution/ImageSpace/Recipe contracts, static ownership
proof, documentation-index validation, and Release normal+NG UI smoke.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-cache-retirement-20260908`.
Boundary / next dependency: atomic stale callback application across a Reset
that races an already-entered UI callback remains a separate proof slice. Next
single priority: `OVL-07 Pipeline Review stale callback atomic application
boundary` | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
