# OpenVisionLab OVL-07 Pipeline Review image/Layer lifetime owner boundary — 2026-09-08

## Scope

This slice moves Pipeline Review layer preview acquisition to the concrete,
Window-free `OpenVisionPipelineReviewLayerImageOwner`. The owner returns a
short-lived display snapshot or a cloned review-cache output, and callers own
that returned `Bitmap` with an explicit `using` boundary. It also closes the
same synchronous ownership gap in fixture projection and scale-calibration
callbacks.

The change is limited to image acquisition and lifetime. The execution
controller remains the owner of review-run summaries and cached output images;
this slice does not change its Reset/Close generation, cancellation, or stale
result policy.

## Responsibility and call-path change

| Concern | Before | Current |
| --- | --- | --- |
| Display preview lookup | `OpenVisionPipelineReviewDocument` directly called `GetLayerImage` through a local resolver | `OpenVisionPipelineReviewDocument` calls `OpenVisionPipelineReviewLayerImageOwner.AcquirePreview` |
| Step output lookup | Document returned the execution controller's cached `Bitmap` or display image | `AcquireOutputPreview` clones the cached output first and falls back to an owned display snapshot |
| Availability checks | Document resolved a borrowed image and checked for `null` | `HasPreview` acquires and disposes one owned snapshot |
| Fixture/scale consumers | Callbacks received borrowed `Bitmap` values with no local lifetime boundary | Fixture projection and scale callbacks dispose owner-returned snapshots after synchronous use |
| UI image lifetime | View/ViewModel clone into WPF or owned state and dispose on their existing lifecycle | unchanged |

The new call path is:

`Document -> LayerImageOwner -> DisplayManager.GetLayerImageSnapshot` or
`executionController.ResolveCachedOutput -> clone -> existing View/presenter`.

The owner rejects the existing display placeholder before falling back to a
cloned review-cache image. `AcquirePreview` keeps display-first behavior;
`AcquireOutputPreview` keeps review-cache-first behavior. No `Window`,
`UserControl`, View, Dispatcher, or persistent image field was introduced.

## Changed files

- `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Presenters/OpenVisionPipelineReviewLayerImageOwner.cs`
  - Adds the concrete image acquisition and clone owner.
- `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`
  - Routes selected-step, scale, fixture, reference-size, flow-availability,
    and readiness image reads through the owner; removes the old borrowed-image
    resolvers and scopes returned snapshots with `using`.
- `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.Events.cs`
  - Scopes scale-calibration snapshots to the synchronous callback.
- `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Presenters/OpenVisionPipelineReviewFixturePresenter.cs`
  - Disposes source and normalized resolver snapshots after fixture projection.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - Adds the no-window `--pipeline-review-layer-image-owner-contract` contract.
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
  - Records this completed slice and the next single priority.
- `docs/LLM_DOCUMENT_INDEX.json`
  - Routes Pipeline/ownership work to this report.

## Preserved contracts

- Existing Recipe/XML serialization and exchange remain unchanged.
- Input/output Layer names and explicit Preview/Run behavior remain unchanged.
- Existing placeholder handling, display-versus-review-cache preference, View
  setter order, WPF image cloning, and View/ViewModel disposal remain unchanged.
- Execution Reset/Close, cancellation, generation stamps, and stale-result
  suppression remain owned by the existing execution controller.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-layer-image-owner-20260908`

- OpenVisionLab app Debug and Release builds passed sequentially with 0
  warnings and 0 errors.
- VisionRecipeRunnerSmoke Debug and Release builds passed sequentially with 0
  warnings and 0 errors.
- `--pipeline-review-layer-image-owner-contract` passed in Debug and Release.
  It verified display-first preview, cache-first output, placeholder fallback,
  missing-layer fail-closed behavior, and that an acquired snapshot remains
  valid after layer replacement/removal.
- Existing ImageSpace lease snapshot, Pipeline Review execution, and Recipe
  pipeline exchange projection contracts passed in Release.
- `OpenVisionLab.sln` Debug/Any CPU build and `OpenVisionReadinessCheck` passed;
  readiness reported all 13/13 contracts passing.
- `PipelineViewerScreenshotSmoke` Debug/Release builds had 0 errors and retained
  the existing `Program.cs:10722` CS8600 warning (1 warning per build).
- Pipeline Review normal and acceptance-NG UI smoke passed in Debug and Release
  (`check=OK`, `layout=0|text=0|internal=0`, `1600x900`). Fresh captures were
  reviewed for normal and NG states.
- Dynamic monitor evidence recorded one monitor, `\\.\DISPLAY2`, bounds
  `0,0,1920x1080`, working area `0,0,1920x1032`; the smoke runner used its
  monitor-aware placement path and no target process remained afterward.
- Static ownership proof passed at
  `static-layer-image-owner-proof.log`, including concrete non-partial owner,
  lease snapshot use, cache cloning, removal of the old Document resolvers,
  and disposal at synchronous consumers.

No source reset, clean, checkout, commit, push, merge, release, or deployment
was performed. The full theme/layout/DPI (100/125/150/175/200%), multi-monitor,
hover/pressed/focus, resize, keyboard, and long-run UI matrix was not run. A
separate before-baseline capture was not available for this slice.

## Refactor proof

- **Current owner:** `OpenVisionPipelineReviewDocument` local resolvers and
  synchronous callers borrowed display/cache `Bitmap` instances.
- **Intended owner:** `OpenVisionPipelineReviewLayerImageOwner` owns acquisition,
  placeholder fallback, and cache cloning; each caller owns disposal of the
  returned snapshot.
- **Dependency direction:** Document/presenter callback -> concrete owner ->
  existing display manager or execution-controller cache provider; View remains
  the UI image owner.
- **State owner:** the owner has no workflow state or persistent `Bitmap`; the
  execution controller retains review-cache state and the View retains cloned
  UI state.
- **Observable contract:** display/cache preference, Layer routing,
  Recipe/XML, explicit Preview/Run, setter order, and execution lifetime are
  preserved.

Status: Complete
Scope: OVL-07 Pipeline Review image/Layer lifetime owner boundary.
Acceptance criteria: a concrete owner is used by every reviewed layer-image
read, borrowed Document resolvers are removed, synchronous consumers dispose
owned snapshots, display/cache and Layer contracts remain stable, and focused
Debug/Release plus representative UI verification pass.
Verification: app/runner/solution/readiness builds, focused owner contract,
existing ImageSpace/execution/Recipe contracts, static ownership proof, and
Debug/Release normal+NG UI smoke.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-layer-image-owner-20260908`.
Boundary / next dependency: review-cache retirement and Reset/Close disposal
ordering remain in `OpenVisionPipelineReviewExecutionController`; the next
single priority is `OVL-07 Pipeline Review review-cache retirement/Reset-Close
owner boundary` | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
