# OpenGL/GPU/Viewer Coordinate Reliability Development Plan

Updated: 2026-08-24 KST

Status: Complete in Dev

Recommended first model: `gpt-5.6-luna` | Reasoning effort: `high`

Escalation model: `gpt-5.6-sol` | Reasoning effort: `high`, only when the
bounded Luna attempt cannot prove the native-context, driver-allocation, or
coordinate root cause. Do not lower the acceptance criteria to stay on Luna.

## 1. Authorized Goal

Continue the user-authorized reliability program in
`C:\Git\OpenVisionLab_Dev` and close the next active boundary:

1. release temporary OpenGL resources on every normal, early-return, and
   exceptional exit;
2. measure process GPU/driver allocation across repeatable 4512 x 4512 viewer
   cycles and prove a bounded plateau;
3. correct and runtime-prove Viewer pixel and region edge coordinates.

This plan authorizes the next chat to implement and verify this Dev-only slice.
It does not authorize original-repository promotion, commit, push, release, or
deployment.

## 2. Current Baseline And Boundary

- OpenVisionLab is an OpenCvSharp4 rule-based vision Recipe workbench. Its core
  workflow remains sample image -> PropertyGrid teaching -> Pipeline ->
  explicit Preview/Run -> drawings/metrics/layers -> N-sample validation ->
  saved Recipe.
- PL-0004 completed coordinated display-store/central/docked/popout lifetime.
  Its final exact 4512 five-cycle gate passed with matching store/dock/popout
  hashes, one live Layer Viewer, private range 16.8 MB, working-set range
  4.3 MB, managed range 0.1 MB, handle range 21 with positive growth 2, and
  GDI/USER range 0.
- That gate measured process private/working/managed memory and Windows object
  counts. It did not measure GPU VRAM, exact intra-operation peaks, native
  framebuffer pixels, or every OpenGL exception path.
- The current workstation exposes Windows `GPU Process Memory`,
  `GPU Adapter Memory`, and `GPU Engine` counters. It also has
  `nvidia-smi`; the inspected adapter is an NVIDIA GeForce GTX 1060 3GB with
  driver 582.28. Prefer these native facilities before adding a profiler
  dependency.
- Commercial-tool lessons to retain are deterministic configuration, visible
  result evidence, and reviewable coordinates. Camera, lighting, PLC/I/O, MES,
  3D, deployment, and provider/platform expansion remain out of scope.

Primary evidence source:
`docs/reports/OPENVISIONLAB_PROJECT_ANALYSIS_AND_RELIABILITY_PRIORITY_20260823.md`.

## 3. Non-Negotiable Contracts

- Preserve zoom, pan, drag, ROI overlays, template editing, layer comparison,
  and docking behavior.
- Preserve explicit Preview/Run. Visibility, selection, load, create, delete,
  and coordinate checks must not auto-run a Tool.
- Do not change the active layer or Pipeline input/output routing as a side
  effect of restoration, measurement, or test setup.
- Keep the existing SharpGL/ImageCanvas path. Do not introduce a renderer
  replacement, GPU framework, or speculative resource-manager abstraction.
- Delete OpenGL objects only while the owning or compatible render context is
  current. Cleanup must be idempotent and must not replace the original
  operation exception with a cleanup exception.
- Treat right and bottom image bounds as exclusive. Valid pixel indices are
  `0..width-1` and `0..height-1`; an outside point must remain outside rather
  than clamp silently to the last pixel.
- Put all generated tests, logs, screenshots, counter samples, and diagnostic
  images physically under
  `D:\OpenVisionLab-TestData\OpenVisionLab\opengl-gpu-coordinate-<timestamp>`.
- Preserve unrelated dirty worktree changes. Change only the minimum owning
  files and focused verification support required by this plan.

## 4. Inspected Source Candidates

These are source-level candidates, not runtime-proven defects. Reproduce or
force the path before calling each one fixed.

| Candidate | Current source observation | Required proof |
| --- | --- | --- |
| `OpenGlRenderer.SetupFrameAndRenderBuffers` | FBO/RBO deletion follows `action()` without a `finally`; an action exception can bypass cleanup. | Force the callback to throw, prove cleanup, then prove the same Canvas can render again. |
| `OpenGlRenderer.RenderTextureToBitmap` | Bitmap, FBO, RBO, and Texture are allocated before maximum-size and callback exits; cleanup is not exception-safe, and the success path does not visibly delete the RBO. | Cover maximum-size, lock/upload, callback, readback, and success exits with exact resource accounting. |
| `OpenGlRenderer.TextureToBitmap` | `Bitmap.LockBits`/`UnlockBits` and binding restoration are not protected from an intervening exception. | Force or simulate the intervening failure and prove Bitmap/binding cleanup. |
| `OpenGlRenderer.TextureToMat` and `ImageCanvasControl` texture reads | FBO/PBO/RBO creation, mapping, copying, and cleanup occur in linear code. | Prove every allocated ID is released when mapping, copying, or result construction fails. |
| `ImageCanvasControl.GetGrayValue`, `GetScreenColor`, `GetPixelColor` | Some screen-to-OpenGL reads use `height - y`, while other paths use `height - 1 - y`. | Compare all four edge pixels against a known source and native readback. |
| `OpenGlRenderer.RestorePartTexture` | Region restore mixes `oglY - 1`, `imgY + 1`, and `width/height + 1`. | Prove a 1 x 1 and edge-touching region changes exactly the requested half-open rectangle and no neighbor. |

Start searches from:

- `src/Libraries/OpenVisionLab.ImageCanvas/Engine/ImageCanvasControl.cs`
- `src/Libraries/OpenVisionLab.ImageCanvas/OpenGL/OpenGlRenderer.cs`
- `src/Libraries/OpenVisionLab.ImageCanvas/OpenGL/OpenGlOverlay*.cs`
- `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel*.cs`
- the existing focused WPF smoke targets in
  `tools/PipelineViewerScreenshotSmoke/Program.cs`

Before editing a method, search every caller and sibling allocation path.

## 5. Ordered Checkpoints

### CP0 - Reorientation And Frozen Baseline

1. Run `git status --short` and `git log --oneline -5`.
2. Read the required start/continue and
   `runtime_stability_and_resource_ownership` routes.
3. Inspect the current diff before touching dirty files.
4. Build the current source and run the existing focused 4512 lifetime gate
   before changing behavior. Record any baseline failure; do not overwrite it
   with a post-change result.

Exit: exact source state, test command, artifact root, and baseline outcome are
recorded.

### CP1 - Resource Census And Failure Matrix

Create one concise owner table covering Texture, FBO, RBO, PBO, display list,
Bitmap/BitmapData lock, binding state, render context, and the SharpGL timer.
For each allocation record its creator, owner, success cleanup, failure
cleanup, required current context, and focused reproducer.

Minimum forced exits:

1. incomplete framebuffer;
2. render callback throws;
3. maximum texture-size rejection after earlier allocation;
4. Bitmap lock/upload failure boundary;
5. framebuffer/texture readback failure boundary;
6. PBO map returns zero or the copy/result construction fails;
7. Canvas closes while queued refresh work exists;
8. cleanup is invoked twice.

Use the existing callback seam where it already reaches the failure. Add a
test hook only when a required reachable exit cannot otherwise be reproduced.

Exit: every generated resource has one named retirement path, and the failure
matrix can fail before the correction and pass after it.

### CP2 - Minimal Exceptional Cleanup Correction

Correct the shared owner methods with the smallest root-cause change. Prefer
local `try/finally` and explicit allocation flags/IDs over new abstractions.
Restore neutral bindings where the caller contract requires it. Keep the
original exception primary and record cleanup failure without swallowing it.

After each forced failure, render a known image again in the same supported
workflow. A cleanup test that leaves the Viewer unusable does not pass.

Exit: success, early-return, forced-exception, repeat-dispose, and subsequent
render checks pass.

### CP3 - GPU/Driver Allocation Gate

Extend the existing exact 4512 cycle evidence rather than creating an
unrelated performance harness. Correlate the OpenVisionLab process ID with
Windows `GPU Process Memory` instances and record dedicated/shared usage.
`nvidia-smi` may supplement adapter totals, but it is not the portable primary
contract.

Capture at least:

1. warmed process before Viewer/image load;
2. Main Viewer after texture upload;
3. docked and popout viewers after render;
4. every replacement-cycle peak and retained sample;
5. after popout close, layer deletion, Canvas/context disposal, GC, and a
   documented driver-settle interval;
6. adapter LUID/name, driver version, process PID, image format/dimensions,
   estimated texture bytes, and observed dedicated/shared deltas.

Do not require zero GPU usage while a live Viewer/context remains. Pass by a
bounded plateau and directional growth rule that tolerates asynchronous driver
release but fails sustained cycle-over-cycle retention. Define the numeric
ceiling from a clean baseline and expected live texture set before judging the
final run; do not choose a threshold after seeing a failure.

Exit: a reproducible CSV/text gate distinguishes warm-up, live allocation,
peak, delayed release, and retained growth.

### CP4 - Pixel And Region Coordinate Contract

Generate a diagnostic image whose four corners, last row/column, interior grid,
and tile boundaries have unique values. Retain the source and SHA-256 with the
runtime output.

Verify:

- `(0,0)`, `(width-1,0)`, `(0,height-1)`, and
  `(width-1,height-1)`;
- 1 x 1, full-image, right-edge, bottom-edge, and corner-touching regions;
- negative, `x == width`, and `y == height` outside inputs;
- 100% and non-integral zoom, fit, pan, resize, and tile boundaries;
- screen -> Canvas/OpenGL -> image -> screen round trips with a stated
  tolerance;
- cursor/readout value, ROI/hit region, native framebuffer readback, and source
  pixel identity;
- region restore changes exactly `[x, x + width) x [y, y + height)` and leaves
  every neighboring pixel unchanged.

Hosted OpenGL content may be black in `RenderTargetBitmap`; do not use such a
capture as pixel proof. Use native readback and focused actual-EXE evidence.
The broad all-theme/Wide/Compact/DPI/monitor matrix remains the next separate
priority unless this slice changes a shared visible style/layout.

Exit: exact pixel/region fixtures fail on the reproduced off-by-one case and
pass after the smallest shared conversion correction.

### CP5 - Regression And Durable Closure

Run the focused checks first, then the repository gates. Record only commands
actually run. Update this plan or a dated completion report, the current
handoff, and the machine-readable route with the final state and evidence.

Use exactly one final state:

- `Complete`: every acceptance criterion and required current-task gate passes;
- `Incomplete`: a required check fails or a defect remains;
- `Blocked`: an external hardware/driver/permission prerequisite is missing.

## 6. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| AC1 | The resource census names creator, owner, success/failure cleanup, current-context requirement, and retirement point for every admitted OpenGL/native object. |
| AC2 | Every forced exit releases only objects actually allocated, tolerates repeat cleanup, preserves the original exception, and allows a subsequent render. |
| AC3 | The exact 4512 repeated Viewer workflow reaches a predeclared GPU dedicated/shared-memory plateau without sustained retained growth. |
| AC4 | Native source/readback values match at every corner and region boundary; outside coordinates are rejected and region restoration changes no neighboring pixel. |
| AC5 | Preview/Run counts, active layer, layer lifecycle, command state, and Pipeline routing remain unchanged except for the explicit actions in the smoke. |
| AC6 | Focused Viewer/Template Editor/4512 checks and the required build, readiness, dependency, sample, and documentation gates pass. |
| AC7 | Fresh evidence contains exact source identity, GPU/driver identity, commands, counter logs, diagnostic images/readbacks, hashes, and the admitted boundary. |

## 7. Verification Commands

Use a fresh D-drive output folder and route local test `TEMP`/`TMP` there when
practical.

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"

powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 `
  -Targets "wpf_shell_host_image_4512_reliability,wpf_shell_host_image_4512_lifetime,wpf_shell_host_workspace_image_load,wpf_shell_host_workspace_quick_actions,wpf_shell_host_layer_management_commands,wpf_shell_host_layer_popout,wpf_shell_host_large_image,wpf_imagecanvas_owned_mat_load,wpf_template_editor_opengl" `
  -OutputDir "D:\OpenVisionLab-TestData\OpenVisionLab\opengl-gpu-coordinate-<timestamp>\focused" `
  -FailOnWarn

dotnet run --project tools\HistoryContractCheck\HistoryContractCheck.csproj -c Debug
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestDocumentationIndex.ps1
```

If an actual desktop EXE is launched, detect the current monitor topology
before launch, use the required test monitor from `AGENTS.md`, verify the final
window rectangle intersects it, and record monitor name/bounds. Build first and
use only the latest current-source EXE.

## 8. Stop And Escalation Conditions

Stop and report the exact evidence instead of widening scope when:

- the correct fix requires replacing SharpGL or changing the product renderer;
- GPU counters cannot be correlated to the process/adapter on the current
  machine;
- the coordinate convention is contradicted by an external public contract or
  a verified existing workflow;
- a focused smoke failure is unrelated and cannot be separated without an
  unauthorized change;
- completion would require original-repository, release, deployment, new
  hardware, or external participant authority.

Recommend `gpt-5.6-sol | high` if Luna has inspected the full shared path and
produced a focused reproduction but still cannot establish the correct
render-context cleanup order, driver-allocation interpretation, or coordinate
transform. Preserve all artifacts and the failed criterion for the handoff.

## 9. Reusable Completion Record

```text
Status: Complete | Blocked | Incomplete
Scope: OpenGL exceptional cleanup, GPU/driver allocation gate, and Viewer pixel/region coordinate edges in Dev only
Acceptance criteria: AC1..AC7 -> pass/fail with exact evidence
Verification: commands actually run and results
Evidence: D-drive artifact root plus source/report paths
Boundary / next dependency: unproved DPI/theme/multi-PC/field/original/release scope or exact blocker
```

## 10. Paste-Ready Next-Chat Prompt

```text
Work only in C:\Git\OpenVisionLab_Dev. Continue the authorized OpenGL/GPU/Viewer-coordinate reliability slice; do not stop at another analysis-only report when safe implementation and verification can proceed.

Use gpt-5.6-luna with reasoning effort high. First run git status --short and git log --oneline -5. Read AGENTS.md, docs/README.md, docs/LLM_DOCUMENT_INDEX.json, then the start_or_continue and runtime_stability_and_resource_ownership routes, especially docs/roadmap/OPENVISIONLAB_OPENGL_GPU_COORDINATE_RELIABILITY_PLAN_20260824.md and docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md. Preserve all unrelated dirty changes.

Before editing, state the current product identity, evidence-based maturity, commercial lessons to retain, out-of-scope platform areas, the immediate checkpoint, and the remaining project priority. The immediate work is CP0/CP1: freeze the current baseline, trace every caller, and build the OpenGL Texture/FBO/RBO/PBO/display-list/Bitmap-lock/context/timer owner and failure matrix. Then implement the smallest shared root-cause cleanup and continue through the GPU allocation and coordinate checkpoints when each prior checkpoint passes.

Do not add a renderer, GPU framework, speculative abstraction, new algorithm, concurrency, camera/PLC/I/O, or broad UI redesign. Preserve explicit Preview/Run, layer creation/deletion/selection, active-layer, command CanExecute, Pipeline routing, zoom/pan/ROI/template/docking contracts. Work in Dev only; do not touch C:\Git\OpenVisionLab, commit, push, release, or deploy.

Put test data and evidence under D:\OpenVisionLab-TestData\OpenVisionLab\opengl-gpu-coordinate-<timestamp>. For actual EXE tests, obey the dynamic monitor rule and use the latest current-source build. Prove forced OpenGL exception cleanup and subsequent rendering, per-process dedicated/shared GPU-memory plateau, and exact four-corner/edge/1x1/region pixel identity with native readback. Run the focused checks and repository gates named in the plan. Update durable documentation with exactly one state: Complete, Incomplete, or Blocked. If Luna produces a focused reproduction but cannot establish the native-context, driver-allocation, or coordinate root cause, preserve evidence and recommend gpt-5.6-sol with high rather than reducing scope or acceptance criteria.
```
