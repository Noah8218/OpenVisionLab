# OpenVisionLab Project Analysis And Reliability Priority

Updated: 2026-08-24 KST

## Analysis Basis

- Repository: C:\Git\OpenVisionLab_Dev
- Source baseline: branch codex/public-sample-ux-docs, commit 827a22e9
- Working-tree boundary: the pre-existing modification in
  tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs is
  user-owned and excluded from this work.
- Method: current source and Git history inspection, routed contract review,
  current command evidence, native SDK reflection/runtime probes, and focused
  call-path tracing.
- Evidence root:
  D:\OpenVisionLab-TestData\OpenVisionLab\analysis-20260823
- Confidence rule: source inspection, current-run verification, historical
  evidence, and unverified risk are kept separate below.

## 1. Executive Summary

OpenVisionLab is already more than an OpenCV sample viewer. It is a connected,
offline-first, rule-based vision Recipe workbench with PropertyGrid teaching,
Pipeline composition, explicit execution, result drawings and metrics, sample
validation, Recipe persistence, and review surfaces. The bounded deterministic
workflow and public Release Candidate evidence are credible. The completed
bounded 4512 x 4512 and frozen-Recipe 1,000-run gates add current large-image
and repeated-execution evidence, but do not prove commercial GA, calibrated
metrology, arbitrary-duration native-memory stability, GPU-memory behavior, or
field integration.

The highest-value next work is reliability, not another algorithm family. The
current analysis found a concrete native-resource ownership defect: three
production Pipeline paths create disposable SDK IVisionTool instances without
disposing them, and two composite Pipeline tools also leave child LineGaugeTool
instances and intermediate VisionToolResult objects undisposed. SDK 3.0 runtime
probes show that a returned result image and metrics remain valid after its Tool
is disposed, so the defect can be corrected without an extra image clone or a
new abstraction.

The ordered reliability program and current result are:

1. Complete - factory-created Pipeline and direct native UI Tool lifetime is
   creator-owned with unchanged bounded behavior.
2. Complete for the admitted owner boundary - ImageSpaceFrame declares
   Borrow/TakeOwnership, Canvas file load returns an owned OpenCvSharp Mat, the
   Emgu bridge is removed, and display-store replacement/removal now retires an
   image only after central, docked, and popout borrowers release or rebind.
3. Complete for the admitted scope - exact 4512 x 4512 current-source WPF and
   frozen-Recipe 1,000-run gates pass with retained hashes, metrics, timing,
   process memory, handle, GDI, and USER evidence.

## 2. Current Product Definition

The current product identity is an OpenCvSharp4-based rule-based vision Recipe
workbench. Its normal operator path is sample image -> direct PropertyGrid
teaching -> Pipeline composition -> explicit Preview/Run -> layer, drawing,
metric, and failure review -> N-sample validation -> saved Recipe.

LLM XML authoring is optional maintenance-mode assistance. Camera, lighting,
PLC/I/O, MES, deployment control, 3D inspection, and generalized equipment
integration are not current product scope.

Maturity is best described as a bounded Release Candidate workbench. Current
evidence supports connected deterministic workflows and packaged public
samples. It does not support a commercial-GA or field-qualification claim.

## 3. Repository Structure

The repository contains 1,909 tracked files, including 1,066 documentation
files. The solution contains 15 projects, while 24 C# project files exist in
the repository for the application, reusable libraries, verification tools,
and support utilities.

The source tree has increasingly clear ownership under src/OpenVisionLab/Core
and src/OpenVisionLab/UI, with reusable display, history, ImageSpace, Canvas,
Pipeline, localization, and MVVM projects under src/Libraries. Documentation is
well indexed but large; docs/README.md and docs/LLM_DOCUMENT_INDEX.json are the
correct entry points rather than a full-doc read.

There is no conventional unit-test directory or standard xUnit/NUnit/MSTest
suite. Most behavior is verified through purpose-built smoke executables.

## 4. Architecture

The intended responsibility split is sound:

- Tool Views configure one algorithm.
- Pipeline owns Step order, layer routing, gates, and explicit execution.
- Pipeline Review explains Step and result evidence.
- Recipe groups reusable Pipelines and validation references.
- Recipe Manager owns Recipe lifecycle and summary/advanced review navigation.

The runtime is not a plugin-host architecture. Tool registration is centralized
through VisionPipelineAppToolFactory with a fallback to the SDK Pipeline
factory. This is adequate for the current product boundary, but disposal and
result ownership must be explicit at the factory consumer.

The current priority does not require a new service, lease type, interface, or
dependency-injection layer. The method that creates a disposable native Tool is
the narrowest correct owner.

## 5. Dependency Analysis

The application uses the manifest-verified OpenVisionLab Vision SDK 3.0,
OpenCvSharp4, WPF, and SharpGL/ImageCanvas components. CanvasImageLoader now
uses Cv2.ImRead with ImreadModes.AnyColor and returns an independently owned
OpenCvSharp Mat. The SDK migration removed the former Library-Noah root and
duplicate managed OpenCvSharp payload; the later loader correction removed the
remaining Emgu managed/native payload.

Current high-risk dependency boundaries are:

- SDK IVisionTool implementations own native resources and implement
  IDisposable.
- OpenGL render paths allocate full-size textures and buffers whose exceptional
  cleanup and GPU storage are not yet fully proven. The exact 4512 gate now
  bounds three observed process-resource snapshots and proves viewer dimensions
  plus texture creation, but not native pixel readback, GPU VRAM, intra-SetMain
  peak, or every exceptional path.
- Package-lock, SBOM, legal, installer, signing, and update policy are separate
  distribution decisions, not reasons to expand the current runtime scope.

## 6. Application Flow

Startup restores the shell, Recipe context, workspace, and cached review state
without automatically running a Tool. The operator explicitly loads or selects
an image, configures a Tool, adds or edits Pipeline Steps, and chooses Preview
or Run.

Pipeline execution resolves an input layer, creates the Step Tool, executes it,
captures object/match/geometry details, publishes the result image to the
configured output layer, and records a Step result. The execution service owns
the temporary input Mat; the successful VisionToolResult crosses the method
boundary and is owned by the Pipeline result path.

## 7. User Workflow

The current workflow has real machine-vision value because it joins teaching,
routing, explicit execution, intermediate images, object evidence, rejection
reasons, validation sets, run history, and Recipe persistence. It is materially
different from a single-function OpenCV sample.

The strongest commercial lessons to retain are guided configuration, visible
intermediate/result/failure evidence, compact Recipe lifecycle management, and
repeatable validation. Commercial equipment integration breadth is not a
product requirement.

## 8. Image Pipeline

Images cross several representations:

1. System.Drawing.Bitmap for files, display layers, and parts of the WPF/Canvas
   boundary.
2. OpenCvSharp.Mat for algorithm execution.
3. ImageSpaceFrame as a display-layer carrier.
4. OpenGL textures and byte buffers for Canvas rendering.
5. BitmapSource/BitmapImage for WPF presentation.

ImageSpaceFrame now makes the first display handoff explicit: Borrow retains
caller Bitmap ownership, TakeOwnership transfers Bitmap disposal, and
DisplayManager consumes/disposes the frame synchronously after the presenter
creates one independent store clone. CanvasImageLoader returns an owned Mat and
does not alias another library's DataPointer. Ownership remains distributed
after the display store: replacement/removal, history, central presentation,
and docked/popout viewers still require a coordinated lease/retirement contract
before further full-image clones can be removed safely.

## 9. Image Ownership & Lifetime

Initial proven defects, now closed by the completion records below:

- VisionPipelineExecutionService.ExecuteStep creates a factory Tool and did not
  dispose it.
- VisionPipelineObjectResults.TryCaptureUnfiltered creates an audit Tool and
  disposed only the temporary result image.
- VisionPipelineMultiMatchMeanService creates a Mean Tool and disposed only the
  temporary result image.
- VisionPipelineLineDistanceTool and VisionPipelineLineIntersectionTool create
  two child LineGaugeTool instances and two intermediate results without an
  owner.

SDK 3.0 reflection found 14 concrete IVisionTool types; all implement
IDisposable through OpenCvAlgorithmBase. Threshold and Mean runtime probes
confirmed that Tool internal images and returned ResultImage objects have
different managed references, CvPtr values, and data pointers. Disposing the
Tool invalidated its internal image but did not invalidate the returned image,
input Mat, or captured Mean metric. Disposing VisionToolResult then released the
returned image; repeated disposal was safe in the probe.

Intended ownership after the first correction:

- The method that calls the factory owns and disposes the Tool.
- A successful returned VisionToolResult transfers to the Pipeline caller.
- A result that cannot be returned because capture fails is disposed locally.
- Audit, normalization, Mean, and child LineGauge results are local temporaries
  and are disposed in their local scope.
- Captured DTO values, metrics, and overlays are copied before Tool disposal.

Production Pipeline and direct native UI Tool lifetime are now closed. The
ImageSpaceFrame carrier and Canvas loader also have explicit owner contracts.
The remaining ownership slice begins at display-store replacement/removal and
long-lived presenter/viewer borrowing; it must be changed as one coordinated
lease or retire-after-rebind boundary to avoid use-after-dispose.

## 10. Viewer

The ImageCanvas retains zoom, pan, drag, ROI overlays, template editing, layer
comparison, and docking capabilities. Coordinate helpers distinguish Canvas,
OpenGL, and image coordinates in several paths.

Source inspection still identifies correctness and performance risks:

- ImageCanvasControl uses render-context height minus y without the usual minus
  one in several pixel reads.
- OpenGlRenderer restores regions with mixed minus/plus-one offsets and
  width/height plus one.
- RenderTextureToBitmap performs a full-size texture upload and framebuffer
  allocation; exceptional cleanup and maximum-size behavior need a focused
  native test.

These are source-level findings. Actual rendered pixel-coordinate evidence was
not produced in this analysis.

## 11. Algorithm Inventory

The application factory directly recognizes Blob, Contour, Line/LineGauge,
LineDistance, PinArrayGap, CurveBandProfile, experimental
OuterCornerIntersection, LineIntersection, CircleGauge, Matching,
EdgeBasedMatching, Mean, HSV mask, Edge, RotateScale, Feature/SIFT matching, and
ReferenceDifference. The SDK Pipeline factory supplies additional registered
tools such as Threshold, filtering, morphology, and normalization paths.

Public-sample verification currently reports 33 catalog rows, 229 assets, and
17 Pipeline definitions. This proves packaged sample integrity and current
registration coverage; it is not per-algorithm physical-part qualification.

OuterCornerIntersection remains experimental and outside default
recommendations until independent physical-boundary evidence exists.

## 12. Algorithm Architecture

Algorithm parameter objects are generated through PropertyGrid and converted to
SDK or application Tool instances by the factory. Pipeline-specific wrapper
Tools adapt application policies such as composite line measurements, fixtures,
result enrichment, and validation.

This is extensible enough for the current built-in product. Adding a new family
still requires a defined operator workflow, PropertyGrid parameters, result
model, drawings, metrics, failure contract, XML mapping, sample, and focused
smoke evidence. A generalized external plugin SDK is neither implemented nor
currently justified.

## 13. Parameter System

Tool Views use typed property objects, while stored Pipeline parameters are
string dictionaries parsed by factory helpers with invariant-culture handling
in important paths. This makes current Recipes editable and portable but leaves
schema/version evolution more implicit than a production migration system.

The next compatibility work should define explicit schema evolution,
unknown-parameter behavior, unit semantics, and migration fixtures before
expanding parameter breadth.

## 14. Recipe

Recipe Manager, Pipeline storage, sample validation references, XML
validation/import, snapshots, and clean runtime data-root behavior are
implemented. Qualified Recipe snapshot smoke evidence passed in the current
analysis.

Remaining confidence gaps are version migration, corrupted/partial-save
recovery across all Recipe artifacts, and deterministic hash identity for every
result payload. Existing evidence is strong for current schemas, not arbitrary
future compatibility.

## 15. Pipeline

Pipeline owns Step order, enabled state, input/output routing, acceptance gates,
and explicit execution. Review distinguishes missing input from downstream
WAIT state and preserves the no-auto-run contract.

The architecture is practically extensible through a central app factory and
SDK fallback. Its immediate flaw was creator-side Tool lifetime, not lack of
another abstraction. The current correction therefore preserves registration,
parameters, metrics, errors, and layer routing.

## 16. Result Model

VisionToolResult carries success/failure, error code, message, elapsed time,
result image, metrics, overlays, and exception context. Pipeline layers add
object, matching, geometry, validation, and review-specific result models.

The model supports explainable review, but numerical semantics need continued
discipline. Result hashes, exact units, circular-angle means, and metric
compatibility should be explicit before metrology claims. Execution count alone
does not prove semantic correctness.

## 17. ROI / Teaching

Rectangle and rotated/fixture-oriented teaching paths, template editors, layer
selection, and reference-coordinate ROI workflows exist. Matching ->
NormalizeImage -> reference-coordinate ROI is the correct deterministic
teachable direction.

ROI edits must remain visible and explicit and must not automatically run a
Tool or mutate unrelated routing. Source inspection cannot replace current
rendered evidence for coordinate edges, DPI, zoom, and pointer-hit behavior.

## 18. Performance

Recent current-build evidence supports responsive startup and Pipeline Review
navigation in bounded scenarios. N-image processing is intentionally sequential
unless a measured bottleneck and thread-safety audit justify parallel workers.

The exact 4512 x 4512 8bpp gate verifies source/store raw hash identity,
workspace and automatic-dock 4512 dimensions and texture creation, three
observed process private/working-set/managed snapshots, handles, GDI/USER
objects, and elapsed time. Removing one central refresh, one trailing dock
refresh, and two unused workspace mipmap generations preserved command state,
store identity, dimensions, texture creation, and visible behavior. Two
independent final-code runs reduced retained private growth from the earlier
624.1 MB baseline to 523.0/524.2 MB. SetMain improved from 3,572 ms to
1,589/1,319 ms; total elapsed improved from 6,645 ms to 4,744/4,063 ms. Maximum
observed private growth was 526.1/524.2 MB, working-set growth 490.6/489.8 MB,
managed growth 225.1/222.5 MB, and handle growth 13 in both runs. Retained
managed growth remained 51.2 MB. Peak inside SetMain and GPU VRAM remain
unmeasured, so this is not a complete allocation-churn or native-pixel proof.
The final rebuilt target also passed with exact scope text, 529.9 MB retained
and maximum-observed private growth, 1,254 ms SetMain, and 4,237 ms total.

The frozen-Recipe soak completed 1,000/1,000 runs with zero failures, metric or
image drift, unchanged Recipe/source hashes, 42.665 ms p95 and 313.284 ms max.
Maximum growth was 1.141 MB private, 4.523 MB working set, 0.217 MB managed,
16 handles, zero GDI, and three USER objects; every late plateau range was zero.
These are bounded current-machine gates, not a GPU limit matrix, 16K budget,
multi-PC benchmark, or arbitrary-duration guarantee.

## 19. Memory

The codebase uses many explicit using/dispose paths, history snapshots, cloned
display frames, and OpenGL cleanup helpers. That is positive but does not
establish one end-to-end owner.

Highest memory risks found by the initial source inspection were:

- factory-created Pipeline and direct native UI SDK Tool instances without an
  explicit creator-side owner;
- display-store replacement/removal and long-lived presenter/viewer borrowing;
- retained full-image store/viewer copies and hidden presentation allocations;
- full-image Bitmap/Mat/texture copies during display and rendering;
- OpenGL temporary texture, framebuffer, renderbuffer, and Bitmap cleanup on
  exceptional paths.

Tool lifetime, ImageSpaceFrame transfer, and the Emgu alias are closed in Dev.
The duplicate refresh/mipmap slice reduces measured 4512 private growth, but
does not close store/viewer lease ownership, hidden presentation allocations,
or exceptional OpenGL cleanup.

## 20. Async / Threading

Pipeline deadline draining, sample-check cancellation, and dispatcher exception
classification were corrected and verified by P290. Cancellation is cooperative
and does not hard-terminate a hung native call.

The current Tool lifetime change is method-local and adds no thread, Task,
parallel worker, or shared state. Parallel image execution remains gated on a
measured need plus isolated-worker equivalence and native thread-safety proof.

## 21. Stability

Current analysis gates passed for Debug build, readiness, external references,
public samples, documentation index, history, localization, Vision UI source
contracts, Recipe XML, qualified snapshot, runtime stability, six N-image Tool
runs, CVR-09, and CVR-10.

These gates now include bounded exact 4512 x 4512 store identity, viewer
dimensions, texture creation, and process-resource evidence plus a frozen-
Recipe 1,000-run soak. They still do not cover current actual-EXE
UI state across all themes/DPI/monitors, GPU VRAM, every OpenGL exception path,
multi-PC qualification, or field duration.

## 22. Logging / Profiling

Run reports and history retain outcome and elapsed evidence, with correctness
kept separate from performance. Baseline timing comparison is constrained by
suite identity and exact sample multiset.

There is no unified GPU/native allocation profiler. The new large-image and
soak gates retain private bytes, working set, managed heap, process handles,
GDI/USER objects, elapsed distribution, exact input/Recipe hashes, and drift
checks. GPU VRAM/driver allocations and exceptional cleanup still require a
separate admitted probe.

## 23. Batch Processing

Sequential N-image verification exists for Tool Views and Recipe workflows.
This analysis executed 30 samples for each of Threshold, Blob, Contour,
Matching, Edge, and Line, with 30/30 passing for each Tool.

In addition to those 180 family executions, the frozen Mean Recipe now has a
1,000-run one-process soak with exact result/metric/hash stability and late
resource plateaus. Concurrent workers remain intentionally unimplemented.

## 24. Tests

Strengths:

- purpose-built smoke runners exercise real Pipeline, SDK, storage, UI-source,
  sample, localization, and release contracts;
- current analysis passed readiness 13/13 and the focused runtime/fixture
  contracts;
- public assets and external DLL hashes are checked.

Weaknesses:

- there is no conventional unit-test framework, coverage report, or standard
  test discovery;
- many smoke tests are large executable contracts, so precise fault isolation
  is harder;
- source/reflection UI contracts are not runtime rendering evidence;
- actual-EXE DPI/theme/monitor, GPU-memory, exceptional cleanup, and standard
  coverage gates remain absent.

## 25. Code Quality

The current domain/folder direction is substantially clearer than a monolithic
WPF application. Core Pipeline responsibilities and smoke tools are searchable,
and stable contracts are explicit.

Remaining debt includes legacy naming, very large Canvas/WPF files, the
System.Drawing/OpenCvSharp/OpenGL boundary, display-store/viewer lease
ownership, and exceptional rendering cleanup. Direct SDK construction and
wrapper-child lifetime now have explicit owners. File length alone is not a
reason to split; each future refactor must establish a real owner and test seam.

## 26. WPF / MVVM

The product uses ViewModels, presenters, controllers, services, behaviors, and
domain-specific UI ownership. Some legacy code-behind remains, especially in
viewer/editor surfaces.

This reliability work made no intended visible layout, styling, parameter, or
operator-action change. Current-source WPF captures verify the affected load,
viewer, command, popout, large-image, and editor slices, but no current actual-
EXE theme/DPI/monitor matrix was produced. Source/current-view review is
complete for those bounded slices; actual Runtime EXE verification remains
required for a broader UI completion claim.

## 27. Save / Compatibility

Pipeline, Recipe, validation, run reports, snapshots, and localized Guide
manifests have explicit storage checks. Qualified snapshot and Recipe XML
contracts passed.

Source review still calls for stronger atomic-save/migration coverage and clear
truthfulness on Main/layer load/save operations. A successful UI command should
mean the intended bytes and display layer were actually updated, not merely
that a dialog closed or a request was issued.

## 28. Documentation vs Implementation

Implemented and currently evidenced:

- deterministic Tool/Pipeline/Recipe workflow;
- public samples and 17 Pipeline definitions;
- Pipeline Review, validation, run history, snapshots, and bilingual Guide;
- SDK 3.0 integration and Release Candidate packaging.

Implemented and bounded by current gates, but not broadly qualified:

- exact 4512 x 4512 Canvas/display behavior on the current workstation;
- one frozen-Recipe 1,000-run sequential native execution path;
- comprehensive runtime UI state/DPI coverage;
- schema migration and numeric/metrology semantics.

Planned or deliberately inactive:

- generalized plugin SDK;
- camera/lighting/PLC/MES integration;
- parallel batch workers;
- installer/signing/update/self-contained distribution;
- new algorithm families without an admitted operator need.

## 29. Critical Issues

P0:

- No current source finding was demonstrated as immediate data corruption,
  security compromise, or unavoidable crash on the verified normal workflow.

P1:

- Bitmap/Mat/display-store/Canvas/viewer ownership is not explicit end to end;
  store replacement/removal cannot safely dispose a Bitmap until all long-lived
  borrowers rebind or release a lease.
- Hidden and duplicate full-image presentation allocations still leave a high
  4512 retained process footprint despite the proved refresh/mipmap reduction.
- OpenGL exceptional cleanup and GPU/driver allocation need a dedicated
  contract beyond the current process-level 4512 gate.
- Viewer pixel and region-copy coordinate arithmetic has source-level
  off-by-one risk.
- Main/layer load and save commands need exact success/failure truthfulness.
- DirectSmokeRunner can exercise a path outside normal validator gates; it must
  not be treated as equivalent to the product execution contract.

P2:

- explicit Recipe/Pipeline schema migration;
- result hash, unit, and circular-angle semantics;
- standard test discovery and coverage;
- package lock/SBOM/legal decisions;
- current actual-EXE theme/DPI matrix;
- broader color/16-bit/16K, reload/cleanup, GPU, multi-PC, and field-duration
  reliability qualification.

P3:

- naming cleanup, dead/dormant helper removal, and localized code organization
  after higher-priority ownership proof.

## 30. Machine Vision Developer Value

OpenVisionLab is useful when an engineer needs to teach and inspect deterministic
2D rules, see intermediate layers and failure evidence, compare samples, and
save a reproducible Recipe without a cloud or LLM dependency.

Its value is not turnkey production-line integration. It is transparent,
developer-oriented composition and verification of rule-based image processing.

## 31. Competitive Position

The product should emulate commercial tools in guided setup, clear accepted and
rejected evidence, recipe/sample organization, compact review, and deterministic
replay. It should not imitate their camera, lighting, PLC, robotics, account,
deployment, and proprietary ecosystem breadth.

The present differentiator is a C#/.NET-native, OpenCvSharp-friendly,
offline-first, inspectable workbench with source-visible behavior and public
sample Recipes.

## 32. Vision SDK Integration

SDK 3.0 is the algorithm dependency boundary. The application maps Pipeline
parameters, creates Tools, captures application-level result details, and
publishes independent result images.

The ownership probe proves that the application can dispose an SDK Tool
immediately after execution/capture without cloning the returned result image.
The current correction must retain this transfer contract and avoid changing
the SDK interface or vendor DLL.

## 33. OpenVisionLab Ecosystem

OpenVisionLab can serve as the deterministic 2D Recipe workbench in a broader
OpenVisionLab ecosystem, provided integration occurs through stable Recipe,
result, and evidence artifacts rather than shared UI internals or hidden memory
ownership.

Labeling, machine orchestration, and 3D products are adjacent systems, not
requirements to merge into this repository.

## 34. Product Differentiation

The durable differentiators are:

- C#/.NET-native extensibility for Windows vision developers;
- OpenCV/OpenCvSharp concepts exposed through PropertyGrid teaching;
- offline deterministic execution;
- explicit intermediate layers, drawings, metrics, and rejection reasons;
- Recipe and sample validation evidence;
- transparent public source and contracts.

Adding more algorithms before reliability would weaken these differentiators by
making the evidence surface broader but less trustworthy.

## 35. Product Scorecard

| Area | Current assessment | Evidence boundary |
| --- | --- | --- |
| Product identity | Strong | Current contracts and connected workflow agree. |
| Tool/Pipeline/Recipe workflow | Strong in bounded scenarios | Current smokes and public samples; not field qualification. |
| Explainable results | Strong | Drawings, metrics, reasons, Pipeline Review, reports. |
| Algorithm breadth | Moderate to strong | Broad 2D families; physical-part qualification varies. |
| Architecture ownership | Strong in the admitted image path | Tool, ImageSpaceFrame, file-load, display-store, and central/docked/popout viewer owners are explicit. |
| Memory reliability | Bounded pass | Frozen 1,000-run plateau passes; not arbitrary-duration or every Tool family. |
| Large-image performance | Bounded pass | Exact 4512 current-source gate passes; no GPU, actual-EXE, 16K, or multi-PC qualification. |
| Test system | Moderate | Broad smoke contracts, no standard unit/coverage layer. |
| UI runtime qualification | Partial | Prior bounded evidence; no current full DPI/theme matrix. |
| Productization | Release Candidate | Unsigned framework-dependent pre-release, not GA. |

## 36. Recommended Roadmap

Phase 1 - Reliability foundation:

- complete: Pipeline/UI-preview SDK Tool ownership, ImageSpaceFrame transfer,
  owned Canvas file load, Emgu removal, exact 4512 gate, frozen 1,000-run soak,
  the first duplicate-refresh/mipmap reduction, and coordinated display-store/
  viewer lease retirement;
- prove OpenGL exceptional cleanup/GPU allocation and coordinate correctness.

Phase 2 - Algorithm and result foundation:

- lock result hashes, units, circular means, failure semantics, and per-family
  physical evidence;
- add a new algorithm only for an approved operator intent.

Phase 3 - Engineering UX:

- keep guided setup, Pipeline Review, Recipe summary, Learn, and explicit
  Preview/Run behavior coherent.

Phase 4 - Dataset and batch:

- improve deterministic corpus identity, storage, and comparison;
- consider parallel workers only after a measured sequential bottleneck.

Phase 5 - SDK/ecosystem:

- stabilize artifact boundaries before any external extension surface.

Phase 6 - Productization:

- decide installer, signing, update, SBOM/legal, self-contained, and multi-PC
  qualification only through a separate explicit distribution decision.

## 37. Top 15 Next Tasks

1. Complete - dispose all factory-created production Pipeline Tools and
   unreturned temporary results. Completion evidence is recorded below.
2. Complete - audit and close direct native UI preview/teaching SDK Tool
   lifetime. Completion evidence is recorded in the continuation below.
3. Complete - define ImageSpaceFrame Bitmap ownership and transfer semantics.
   Borrow/TakeOwnership and synchronous DisplayManager consumption are verified.
4. Complete for the admitted owner boundary - display-store replacement,
   deletion, reload, popout close, and shell disposal use coordinated leases;
   duplicate refreshes and unused workspace mipmaps are removed. Additional
   clone removal requires separate identity and GPU evidence.
5. Complete - replace the Emgu DataPointer alias with an owned OpenCvSharp load
   result and remove the inactive Emgu runtime payload.
6. Prove OpenGL temporary texture/framebuffer/renderbuffer cleanup on every
   return and exception. Recommended model: gpt-5.6-sol. Reasoning effort: high.
7. Correct and runtime-test viewer pixel/region off-by-one coordinates.
   Recommended model: gpt-5.6-sol. Reasoning effort: high.
8. Make Main/layer load/save success and failure exact and atomic.
   Recommended model: gpt-5.6-sol. Reasoning effort: high.
9. Ensure all direct/smoke runners invoke the same production validation gates
   or clearly identify diagnostic-only bypasses. Recommended model:
   gpt-5.6-sol. Reasoning effort: high.
10. Add exact Recipe/Pipeline schema and migration fixtures. Recommended model:
    gpt-5.6-sol. Reasoning effort: high.
11. Define result-image/hash, metric-unit, angle, and tolerance semantics.
    Recommended model: gpt-5.6-sol. Reasoning effort: high.
12. Add a focused standard unit-test layer only where it shortens fault
    isolation for pure logic. Recommended model: gpt-5.6-terra. Reasoning
    effort: medium.
13. Execute a current actual-EXE UI matrix for supported themes, layouts, and
    DPI values when a UI slice is admitted. Recommended model: gpt-5.6-sol.
    Reasoning effort: high.
14. Complete for the admitted scope - exact 4512 x 4512 store identity,
    observed process-resource, workspace/automatic-dock dimensions, and
    texture-creation gate. Native pixel readback, GPU/actual-EXE/16K
    qualification remains separate.
15. Complete for the admitted scope - frozen-Recipe 1,000-run soak with
    process/native/GDI/USER/handle plateau and exact result drift evidence.

## 38. Top 3 Immediate Priorities

1. Prove OpenGL exceptional cleanup, GPU/driver allocation, and viewer
   pixel/region coordinate edges with focused current-source and actual-EXE
   evidence. Recommended model: gpt-5.6-sol. Reasoning effort: high.

2. Execute the supported actual-EXE theme, Wide/Compact, DPI, resize, and
   monitor matrix only when the next visible UI slice is admitted. Recommended
   model: gpt-5.6-sol. Reasoning effort: high.

3. Keep CVR-00 deferred until three independent first-time participants and
   their unedited observations are available. Recommended model: none until
   observations exist. Reasoning effort: none until observations exist.

## 39. Final Assessment

OpenVisionLab has a credible rule-based 2D vision workbench core and a
well-connected bounded workflow. Its next maturity step is not broader platform
scope or more algorithm names. Tool lifetime, the first image-carrier/file-load
contracts, and bounded 4512/1,000-run gates are now executable evidence. The
coordinated display-store/viewer lifetime is now closed for the admitted path.
The next reliability boundary is exceptional OpenGL/GPU and coordinate
correctness, followed by actual-EXE UI qualification when a visible slice is
admitted.

No algorithm, parameter, Recipe, Pipeline routing, explicit Preview/Run action,
original-repository file, commit, or push is included in these Dev reliability
checkpoints.

## Durable Closure

Status: Complete

Scope: The 39-section project analysis and priority decision are recorded. In
Dev, production Pipeline factory-created disposable Tool lifetime is closed in
the main Step path, Blob/Contour audit replay, and MultiMatchMean. Composite
LineDistance and LineIntersection now own their child LineGauge Tools and
intermediate results. No UI, algorithm, parameter, Recipe, or routing contract
was changed.

Acceptance criteria:

- Three production factory consumers dispose disposable Tools on all lexical
  exits: pass by source ownership proof and zero-warning build.
- A successful VisionToolResult remains caller-owned and usable after Tool
  disposal: pass by SDK Threshold/Mean ownership probe and current Pipeline
  runs.
- An unreturned capture result is disposed locally: pass by the explicit
  ExecuteStep catch/Dispose path.
- Audit, NormalizeImage, Mean, and composite Line temporary results are locally
  disposed: pass by source ownership proof.
- Result metrics, overlays, exact audit reject reasons, and routing remain
  unchanged: pass by focused runtime, object-audit, CVR-10, and public
  LineDistance executions.
- Required repository build/readiness/reference/sample gates pass: pass.

Verification:

- dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU": PASS, zero
  warnings and zero errors.
- VisionRecipeRunnerSmoke --runtime-stability-contract: PASS.
- VisionRecipeRunnerSmoke --object-dimension-filter-contract: PASS; Blob and
  Contour each retained one accepted object and four exact reject reasons.
- OpenVisionFixtureSmoke --cvr10-multi-match-mean: PASS; stable I01..I04,
  4/4 and 3/4 aggregate gates, count/overlap fail-closed, PropertyGrid/XML/run
  report round trips.
- Public_Line_Pins_Distance.pipeline.xml with the public Good image: PASS,
  two of two Steps, 24 distances, DistanceMmAvg 0.222,
  DistanceMmRange 0.012, 27 overlays.
- OpenVisionReadinessCheck: PASS, 13/13.
- TestExternalReferences.ps1: PASS.
- TestPublicSampleAssets.ps1: PASS, 33 catalog rows, 229 manifest assets, and
  17 Pipelines.
- TestDocumentationIndex.ps1: PASS, 63 indexed paths, 12 routes, and 101 root
  redirects.
- git diff --check: PASS; only Git line-ending normalization warnings were
  emitted.

Evidence:

- D:\OpenVisionLab-TestData\OpenVisionLab\analysis-20260823\sdk-tool-result-ownership-probe.txt
- D:\OpenVisionLab-TestData\OpenVisionLab\analysis-20260823\app-factory-tool-lifetime-matrix.txt
- D:\OpenVisionLab-TestData\OpenVisionLab\analysis-20260823\factory-create-call-census.txt
- D:\OpenVisionLab-TestData\OpenVisionLab\tool-lifetime-20260823

Boundary at this checkpoint: Direct UI-preview and teaching code still
contained separate undisposed SDK Tool paths. The continuation below closes
that bounded slice. Bitmap/Mat/ImageSpace/Canvas ownership, the Emgu DataPointer
alias, 4512 x 4512 behavior, a 1,000-run soak, and current actual-EXE UI
theme/DPI evidence remained separate work.

### Direct Native UI Preview/Teaching Tool-Lifetime Continuation

Status: Complete

Scope: In Dev, all 15 active direct SDK Tool construction sites used by native
Tool Preview and Auto MPoint teaching now have a creator-side disposal owner.
Thirteen concrete call sites use lexical `using`; Filter and Morphology use the
shared custom-tool executor's `finally`, with constructor-side cleanup if
`SetProperty` fails. The Preview controller disposes the complete returned
`VisionToolResult`, Auto MPoint uses the SDK Tool/result disposal contract
instead of reaching into three Tool-owned Mats, and the unused
`Func<IVisionTool>` document constructor that could bypass ownership was
removed. No visible control, parameter, algorithm, result, layer, active-layer,
Pipeline, Recipe, or explicit Preview contract changed.

Acceptance criteria:

- Every active direct native UI SDK Tool creation has a local owner on success,
  null/failure, and exception exits: pass by the 15-site source contract (13
  direct `using`, two shared factory/finally owners).
- A Tool may be disposed before its returned result is published: pass by the
  earlier detached-result SDK probe and current Preview execution across all 14
  concrete SDK Tool types.
- Result lists and Auto MPoint candidate DTOs are copied/consumed before Tool
  disposal: pass by source ownership proof plus result-review and teaching
  smoke assertions.
- Preview result publication, drawings, measurements, review text, output
  layers, active-layer restoration, routing, and explicit action counts remain
  valid: pass by the current-source WPF view captures below.
- Required build, readiness, dependency, sample, UI contract, and runtime
  stability gates pass: pass.

Verification:

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"`: PASS,
  zero warnings and zero errors.
- UI SDK Tool lifetime source contract: PASS, 15 creation sites, 13 direct
  `using` owners, two Filter/Morphology shared-finally owners, two Preview
  result-owner `Dispose` sites, and zero dormant `createTool().Execute` sites.
- Current-source WPF view captures: PASS for
  `wpf_preprocess_output_preview_flow`,
  `wpf_direct_multi_tool_inspection`,
  `wpf_shell_host_affine_transform_tool`,
  `wpf_shell_host_line_measure_tool`,
  `wpf_algorithm_output_preview_flow`, and
  `wpf_shell_host_edge_based_matching_tool`.
- `VisionUiContractCheck`: PASS.
- `VisionRecipeRunnerSmoke --runtime-stability-contract`: PASS.
- `OpenVisionReadinessCheck`: PASS, 13/13.
- `TestExternalReferences.ps1`: PASS.
- `TestPublicSampleAssets.ps1`: PASS, 33 catalog rows, 229 manifest assets,
  and 17 Pipelines.
- Before/after images were inspected for Preview/result visibility, clipping,
  overlap, values, units, overlays, and route/status text. This was
  current-source WPF view rendering, not an `OpenVisionLab.exe` screen capture.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab\ui-tool-lifetime-20260823\before`
- `D:\OpenVisionLab-TestData\OpenVisionLab\ui-tool-lifetime-20260823\after`
- `D:\OpenVisionLab-TestData\OpenVisionLab\ui-tool-lifetime-20260823\after\gates\ui-sdk-tool-lifetime-source-contract.txt`
- `D:\OpenVisionLab-TestData\OpenVisionLab\analysis-20260823\sdk-tool-result-ownership-probe.txt`

Boundary / next dependency: This proves the direct native UI Preview and Auto
MPoint SDK Tool lifetime slice only. It does not prove end-to-end
Bitmap/Mat/ImageSpace/Canvas transfer ownership, removal of the Emgu
DataPointer alias, 4512 x 4512 allocation/native rendering, a frozen-Recipe
1,000-run soak, actual-EXE theme/DPI coverage, commercial GA, or hardware/field
qualification. Three stale/transient focused-smoke assertions discovered while
building the evidence are separated as `PL-0003`; passing targets above cover
the product paths without treating those test-text failures as product
failures. Original-repository promotion, commit, and push require separate user
authorization.

### ImageSpaceFrame Transfer Ownership

Status: Complete

Scope: `ImageSpaceFrame` now exposes only explicit `Borrow(Bitmap)` and
`TakeOwnership(Bitmap)` creation. DisplayManager synchronously consumes and
disposes every frame after the presenter clones an independent store Bitmap;
borrowed caller Bitmaps remain caller-owned and Mat conversion produces an
owned frame Bitmap without disposing the caller Mat.

Acceptance criteria:

- Borrow and transfer are distinguishable at every active factory call: pass.
- An owned Bitmap is disposed exactly once with its consumed frame: pass.
- A borrowed Bitmap and caller-owned source Mat remain valid: pass.
- A stored display image remains valid after frame/caller disposal: pass.
- Disposed frame access fails closed with `ObjectDisposedException`: pass.

Verification: ImageSpace owner/source census, DisplayManager ownership runtime
assertions, `HistoryContractCheck`, and current-source
`wpf_shell_host_workspace_image_load` passed.

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab\imagespace-frame-ownership-20260823`.

Boundary / next dependency: The store intentionally retains one independent
clone. Store replacement/removal and central/docked/popout presenter borrowing
must be changed together through leases or retire-after-rebind; immediate store
disposal would create a use-after-dispose window.

### Canvas Owned File Load And Emgu Removal

Status: Complete

Scope: `CanvasImageLoader.LoadMatFromFile` now returns a standalone
OpenCvSharp Mat from `Cv2.ImRead(path, ImreadModes.AnyColor)`. The ImageCanvas
project no longer references or copies Emgu, and the three inactive Emgu
managed/native binaries were removed from the Dev tree after SHA-256-verified
D-drive backup.

Acceptance criteria:

- Gray8, BGR8, BGRA8, and Gray16 load with exact Mat type/content: pass.
- The returned Mat survives forced GC and has one clear caller disposal owner:
  pass.
- Canvas upload/save remains valid after the loader Mat is disposed: pass.
- Clean output and source contain no active Emgu/cvextern dependency: pass.

Verification: Canvas owned-load runtime contract,
`wpf_imagecanvas_owned_mat_load`, `wpf_template_editor_opengl`, clean D-output
build inspection, and `TestExternalReferences.ps1` passed. The hosted OpenGL
region may be black in RenderTargetBitmap capture; exact saved Gray/BGR pixels
were opened and inspected from the same run.

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab\emgu-owned-loader-20260823`
and
`D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\refresh-mipmap-regressions`.

Boundary / next dependency: This proves the loader and current ImageCanvas
consumers, not every color-depth/display conversion or GPU-driver path. The
removed binaries remain recoverable from Git and the verified D-drive backup.

### Exact 4512 And Frozen-Recipe 1,000-Run Gates

Status: Complete

Scope: A current-source WPF gate loads one exact 4512 x 4512 8bpp image into
Main, verifies source/store raw identity plus workspace/automatic-dock viewer
dimensions and texture creation, then records three process-resource snapshots
and post-GC retention. A separate one-process gate
warms up and executes one frozen parsed Pipeline with one in-memory source
exactly 1,000 measured times.

Acceptance criteria:

- 4512 source/store raw SHA-256 identity and visible automatic-dock dimensions
  are exact: pass (`3728404C...F220`, `4512x4512`, one texture tile).
- The maximum of AfterSet, RenderedBeforeGc, and Retained resource snapshots
  plus the 30-second elapsed ceiling pass: pass in two independent optimized
  final-code runs.
- The soak completes 1,000/1,000 without failure, metric/image drift, or
  Recipe/source mutation: pass.
- The soak late resource plateaus, p95, max, first/last result hash, handles,
  GDI, and USER thresholds pass: pass.

Verification: `wpf_shell_host_image_4512_reliability` passed twice from the
explicit final-code `Any CPU` build output. The final soak rerun passed with
1,000/1,000 successes, 21.975 ms average, 18.840 ms median, 42.665 ms p95,
313.284 ms max, and zero late plateau ranges.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\refresh-mipmap-final-verified`
- `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\refresh-mipmap-final-verified-rerun`
- `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\final-current-build`
- `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\soak-rerun`

Boundary / next dependency: The 4512 proof is current-source WPF on this
workstation, not an exact native-pixel readback, actual `OpenVisionLab.exe`
DPI/theme/monitor matrix, intra-SetMain peak, or GPU VRAM budget. The soak
proves one frozen Mean Recipe and corpus, not
every Tool family, concurrency, arbitrary duration, multi-PC, or field use.

### Duplicate Refresh And Workspace Mipmap Reduction

Status: Complete

Scope: Main image application no longer performs a full selected-layer refresh
before `RefreshRows`; command CanExecute re-evaluation is preserved through a
separate callback. `RefreshRows` no longer performs a trailing dock refresh
after `RefreshWorkspace` already refreshed the viewers. The base-image texture
allocation/update paths no longer generate mipmaps that cannot be sampled by
their configured `GL_LINEAR` minification filter.

Acceptance criteria:

- The former central and trailing dock refresh owners are absent while one
  selected/workspace refresh remains: pass by source search and review.
- Workspace and layer command states still re-evaluate after Main load: pass by
  `wpf_shell_host_workspace_quick_actions` and layer-command smoke.
- Exact Main store hash, 4512 viewer dimensions, texture creation, and visible
  layout remain unchanged: pass.
- The strengthened three-snapshot 4512 catastrophe ceilings pass and retained
  private growth improves relative to the earlier 624.1 MB baseline: pass.

Verification: zero-warning focused build;
`wpf_shell_host_image_4512_reliability` final-code runs reported retained
private growth 523.0/524.2 MB, maximum observed private growth 526.1/524.2 MB,
and SetMain 1,589/1,319 ms; the final rebuilt run also passed at 529.9 MB
retained/maximum-observed private growth and 1,254 ms SetMain.
`wpf_shell_host_workspace_image_load`,
`wpf_shell_host_workspace_quick_actions`,
`wpf_shell_host_layer_management_commands`, `wpf_shell_host_layer_popout`,
`wpf_shell_host_large_image`, `wpf_imagecanvas_owned_mat_load`, and
`wpf_template_editor_opengl` passed.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\refresh-mipmap-final-verified`
- `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\refresh-mipmap-final-verified-rerun`
- `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\final-current-build`
- `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\refresh-command-final-regressions`
- `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\refresh-mipmap-regressions`

Boundary / next dependency: This closes only proved duplicate refreshes and the
two base-image mipmap calls. It does not remove the store/docked full-image
clones, hidden presentation allocations, viewer `Loaded` refresh, temporary
allocation peak inside SetMain, GPU VRAM, or exceptional cleanup. Those changes
require the coordinated store/viewer ownership slice rather than independent
disposal or clone removal. That remaining slice is tracked as `PL-0004`.

### PL-0004 Coordinated Display-Store And Viewer Lifetime

Status: Complete

Scope: `ImageSpaceService` is the single owner of each stored image through a
reference-counted `ImageSpaceImage`. Replacement, removal, and service disposal
release the store reference and retire the `Bitmap` only after the last
`ImageSpaceImageLease` releases it. The central workspace holds one lease while
its Canvas and fallback preview borrow the image. Docked and popout content
take a short lease while cloning into their viewer-owned image; replacement
refreshes those viewers, popout close and layer deletion dispose their owned
viewer state, and Shell disposal detaches commands/visuals before closing
viewers, releasing the central lease, and disposing the store.

Owner/borrower census:

| Path | Owner or borrower | Rebind/release point | Retirement point |
| --- | --- | --- | --- |
| Display store | `ImageSpaceService` / `ImageSpaceImage` | `SetImage`, `RemoveImage`, or `Dispose` releases the store reference | `ImageSpaceImage.Release` disposes the `Bitmap` at reference count zero |
| Central workspace | `OpenVisionShellHostWorkspacePreviewController.currentImageLease` | next layer/image bind; previous lease releases in `finally`; controller disposal releases the current lease | shared store retirement point above |
| Docked viewer | short store lease plus `OpenVisionLayerViewerView.ownedLayerImage` clone | `UpdateDocumentContent` clones under the lease; viewer/content disposal releases its clone | lease retirement for store image; viewer `Dispose` for clone |
| Popout/tool preview | short store lease plus viewer-owned clone | open/refresh clones under the lease; explicit close, delete, or Shell close disposes the viewer | lease retirement for store image; viewer `Dispose` for clone |
| OpenGL Canvas | `RoiImageCanvasViewModel` owns `ImageCanvasControl` | presenter/view disposal clears texture, stops SharpGL's unowned WinForms timer, destroys the render context, and disposes child/host | Canvas disposal; no per-cycle USER growth in the five-cycle gate |

Acceptance criteria:

- Source owner/borrower census covers replacement, deletion, reload, popout
  close, and Shell disposal: pass by the table above and source review.
- Five exact 4512 x 4512 cycles complete without disposed access or stale
  pixels: pass; store, docked viewer, and popout hashes match each replacement.
- Active layer returns to Main and Preview/Run count plus all Pipeline routes
  remain unchanged: pass.
- Retained process resources plateau: pass; private range 16.8 MB, working-set
  range 4.3 MB, managed range 0.1 MB, handle range 21 with positive growth 2,
  GDI range 0, USER range 0, and live Layer Viewer count remains one in every
  cycle. The handle gate records the full range but judges leak direction by
  maximum growth from an earlier low, so asynchronous cleanup is not counted as
  a leak; its growth ceiling is 12.
- Direct lease retirement holds the replaced/removed image alive until the
  last lease releases, then disposes it: pass by `HistoryContractCheck`.

Verification:

- `wpf_shell_host_image_4512_lifetime`: PASS, five cycles, 70.311 seconds.
- Focused same-process regression: PASS for existing 4512 reliability,
  workspace image load/quick actions, layer management/popout, large image,
  owned Mat load, and OpenGL template editor.
- `HistoryContractCheck`: PASS.
- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"`: PASS,
  zero warnings and errors.
- readiness, vendored references, and public sample asset contracts: PASS.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab\pl-0004-display-lifetime-20260823\before-final`
- `D:\OpenVisionLab-TestData\OpenVisionLab\pl-0004-display-lifetime-20260824\after-timer-stop-5cycles`
- `D:\OpenVisionLab-TestData\OpenVisionLab\pl-0004-display-lifetime-20260824\final-directional-handle-gate-5cycles`
- `D:\OpenVisionLab-TestData\OpenVisionLab\pl-0004-display-lifetime-20260824\final-focused-regression`

Boundary / next dependency: The gate measures current-process private/working
set, managed memory, handles, GDI, and USER objects after each cycle. It does
not measure GPU VRAM, exact intra-operation peaks, native framebuffer pixel
readback, every OpenGL exception path, actual `OpenVisionLab.exe` theme/DPI/
monitor behavior, arbitrary duration, or multi-PC/field qualification. The
next priority is focused OpenGL exceptional cleanup/GPU allocation and viewer
coordinate-edge evidence, not unproved clone removal.
