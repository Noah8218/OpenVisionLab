# P290 Shared Project Analysis And Runtime Stability Review

Date: 2026-08-06 KST
Source review: <https://chatgpt.com/share/6a735633-ffc0-83ee-a52e-e599d6a27582>
Dev baseline: `ada7082e`

## Decision

The shared analysis is directionally correct about OpenVisionLab's identity and
commercial boundary. OpenVisionLab remains an OpenCvSharp4 rule-based vision
recipe workbench, not a replacement for a complete camera, PLC, I/O, MES, or
equipment-runtime platform.

Its code findings must not be adopted as one undifferentiated P0/P1 roadmap.
This review separates current source defects from unmeasured risks and from
optional product expansion.

## Accepted Current Defects

| Finding | Current source decision |
| --- | --- |
| Pipeline Step timeout returned while work still used run-owned state | Fix in-process ownership now. A timed-out or canceled Step is classified at the deadline, but the runner drains the already-started work before returning and disposing its Context. This is not hard termination. |
| Every WPF dispatcher exception was marked handled | Handle only expected cancellation. Unexpected unhandled UI exceptions remain fatal after logging instead of continuing in an unknown state. |
| Indexed Bitmap conversion retained three temporary native Mats and allocated a replacement managed array for every palette pass | Dispose all temporary Mats deterministically and apply the palette in place. Preserve conversion output. |
| OpenGL font bitmaps reserved one display list for 256 glyphs and cached failure | Reserve 256 contiguous lists, reject allocation/bitmap creation failure, record the exact count, and release lists with the owning canvas. |
| Sample checks wrapped a synchronous `GetResult` pipeline call in `Task.Run` | Keep synchronous file/image preparation off the UI thread, but await Recipe execution end to end and pass `CancellationToken` through the N-image path. Do not add parallel image execution. |
| Log retention cleanup swallowed every exception | Emit a trace warning. Do not add a new logging subsystem. |
| Every log message captured a full `StackTrace` | Capture only the required caller frame. Broader log redesign requires profiler evidence. |

## Findings Not Activated As Product Work

- `BackgroundLoopWorker` has no current caller. It is dormant deletion debt,
  not evidence of an active runtime failure.
- Recipe portability is not absent. Existing import copies operator-approved
  templates into Recipe storage and uses portable relative resolution. Review
  Bundle schema v1 intentionally records dependency hashes without copying
  private files.
- Schema versions and fail-closed checks already exist for qualified snapshots,
  validation sets, batch summaries, result contracts, and review bundles. A
  general Tool/Pipeline migration framework is needed only when an approved
  format change requires it.
- The clean-clone Release candidate gate is broader than a build-only CI job.
  Standard unit-test coverage and hosted GUI launch remain qualification gaps,
  but arbitrary 1,000/10,000-run or eight-hour gates are not adopted without a
  measured failure model.
- Installer, signing, update/rollback, self-contained packaging, SBOM, multi-PC
  qualification, plugin SDK, Worker Process, .NET Framework equipment adapter,
  and new algorithm families require separate product or distribution approval.

## Runtime Ownership Contract

OpenVisionLab cannot forcibly stop an arbitrary OpenCV/native call inside the
same process. The bounded in-process rule is therefore:

1. Record the requested timeout or cancellation deadline.
2. Stop the Pipeline from advancing to another Step.
3. Drain the already-started Step before disposing its input, Pipeline Context,
   late result image, or run result.
4. Return `StepTimeout` or `StepCanceled`; never present the late result as a
   successful Step.
5. Do not start parallel image execution as a workaround.

Process isolation is reconsidered only if a separately approved equipment
Runtime requires hard termination of a hung native operation.

## Changed Owners And Call Paths

- `VisionPipelineExecutionService` now owns deadline classification and late
  work draining. The former detached continuation no longer owns input cleanup.
- `VisionPipelineSampleCheckService` owns asynchronous Recipe awaiting and
  cancellation propagation. Existing synchronous smoke callers retain an
  explicit compatibility wrapper.
- `ImageCanvasControl` owns deletion of its cached font display lists.
- `OpenVisionLabUnhandledExceptionPolicy` owns the recoverable/fatal dispatcher
  distinction.

## Verification

- Focused Debug build of `VisionRecipeRunnerSmoke`: zero warnings and errors.
- `--runtime-stability-contract`: PASS for dispatcher classification, pre-cancel
  sample behavior, valid public Threshold sample execution, deadline drain,
  late result disposal, indexed Bitmap BGR conversion, and 256 glyph count.
- Full Debug solution build: zero warnings and errors.
- OpenVisionReadinessCheck: PASS for all 13 contracts.
- External reference check: PASS for the Vision SDK, OpenCvSharp native runtime,
  and WPF PropertyGrid bridge.
- Public sample asset check: PASS with 33 catalog rows, 229 manifest assets, and
  17 Pipelines.
- Documentation index check: PASS with 59 indexed paths, 12 routes, and 101 root
  redirects.
- Current-source `wpf_shell_host_pipeline_review` direct WPF render: PASS at
  1180 x 890. Visual inspection found no clipped text, clipped icons, hidden
  button content, blank input text, or incoherent overlap. This is a view
  lifecycle/render check, not an actual-EXE operator workflow claim.
- Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab\shared_analysis_stability_20260806\runtime-stability\runtime_stability_contract.txt`
  and
  `D:\OpenVisionLab-TestData\OpenVisionLab\shared_analysis_stability_20260806\imagecanvas-smoke\wpf_shell_host_pipeline_review.png`.

## Closure

```text
Status: Complete
Scope: Shared analysis decision record and bounded current-source stability corrections
Acceptance criteria: accepted source defects corrected -> pass; focused stability contract -> pass; full required project gates -> pass; current-source WPF lifecycle/render -> pass
Verification: Debug solution build 0 warnings/0 errors; runtime stability contract, readiness 13/13, external references, public assets, documentation index, and WPF view render PASS
Evidence: docs/reports/OPENVISIONLAB_SHARED_ANALYSIS_STABILITY_REVIEW_20260806.md and D:\OpenVisionLab-TestData\OpenVisionLab\shared_analysis_stability_20260806
Boundary / next dependency: in-process timeout is deadline classification followed by safe drain, not hard termination; Worker Process, parallel image execution, equipment integration, and commercial-platform expansion remain excluded
```
