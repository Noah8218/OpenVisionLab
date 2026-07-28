# OpenVisionLab Threshold Teaching Suggestion

Date: 2026-07-28 KST
Backlog item: `CVR-07`
Status: Complete

## Outcome

The first bounded task-specific teaching suggestion is complete for the
existing Threshold Tool `Basic` mode.

After one successful explicit Preview, the operator may:

1. select `Analyze suggestion`;
2. review one exact orange cutoff candidate on the retained full-image source
   histogram;
3. read the candidate modes, class separation, class populations, source hash,
   region, and stable suggestion evidence ID;
4. select `Use T` explicitly;
5. recover the previous teaching value with `Undo`.

Analysis and candidate selection do not execute Preview/Run. `Use T` and
`Undo` are explicit teaching changes and reuse the existing debounced Preview
policy.

## Bounded Algorithm

This is not generic automatic threshold optimization.

- `Binary` requests a bright-object candidate. The analyzer selects the
  brightest significant gray mode and its immediately lower significant mode.
- `BinaryInv` mirrors the contract and selects the darkest significant mode
  and its immediately higher significant mode.
- The proposed cutoff is the deterministic midpoint of those two retained
  modes.
- Five-bin smoothing is used only to locate modes. A retained mode must reach
  at least 2% of the strongest smoothed peak, and nearby duplicate peaks are
  suppressed.
- Both resulting gray classes must contain at least 1% of the current
  full-image population. Otherwise the candidate is rejected and manual
  teaching remains unchanged.
- One-mode, empty, non-256-bin, or invalid evidence fails closed.

The suggestion identity binds the source signal evidence, source SHA-256,
region, Binary/BinaryInv intent, candidate, separation, and class populations.

## Bounded Correction Evidence

The first implementation used global Otsu and was rejected by the public
semantic replay:

- candidate: `T=73`;
- Good result: `ResultCount=0`;
- Bad result: `ResultCount=0`.

That split primarily separated the dominant background/board populations
rather than the intended bright-pad class. It was not retained as a successful
feature.

The one bounded correction changed the task contract from generic global Otsu
to the explicit bright/dark significant-mode target described above. On the
public `Public_Threshold_BandPads` pair, the current analyzer retained:

- candidate: `T=138`;
- significant modes: `97` and `178`;
- separation ratio: `0.530025303277949`;
- lower population: `0.96988011988012`;
- upper population: `0.03011988011988`;
- Good: `ResultCount=4`, Pipeline OK;
- Bad: `ResultCount=1`, Pipeline NG.

The exact suggested Pipeline, sources, result drawings, and evidence row are
under `artifacts\cvr07_threshold_suggestion_20260728\final`.

## Verification

Commands actually run:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target cvr07_threshold_suggestion artifacts\cvr07_threshold_suggestion_20260728\final
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_basic_tool artifacts\cvr07_threshold_suggestion_20260728\regression_basic
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\cvr07_threshold_suggestion_20260728\regression_full
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_threshold_signal_good_bad_replay artifacts\cvr07_threshold_suggestion_20260728\regression_good_bad
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target cvr06_matcher_diagnostic artifacts\cvr07_threshold_suggestion_20260728\regression_cvr06
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug
```

Final results:

- Debug solution build: zero warnings, zero errors;
- screenshot-runner build: zero warnings, zero errors;
- analyzer matrix: bright `T=150`, dark `T=75`, exact-repeat evidence ID,
  and single-mode rejection passed;
- UI Analyze/Use/Undo contract passed;
- Analyze caused zero Preview/Run, layer, active-layer, or route changes;
- Use and Undo each produced only the expected existing debounced Preview;
- public Good/Bad suggested-Pipeline replay passed at `4/1`;
- Threshold Basic, full Threshold Basic/Range/Adaptive, frozen CVR-02 public
  replay, CVR-06 matcher diagnostics, and readiness passed.

## Evidence

- current-source baseline:
  `artifacts\cvr07_threshold_suggestion_20260728\before`;
- final candidate UI, applied/undo UI, public sources/results, exact Pipeline,
  and evidence TSV:
  `artifacts\cvr07_threshold_suggestion_20260728\final`;
- focused related regressions:
  `artifacts\cvr07_threshold_suggestion_20260728\regression_basic`,
  `regression_full`, `regression_good_bad`, and `regression_cvr06`.

## Boundary

- Only Threshold `Basic` Binary/BinaryInv full-image current-Preview evidence
  is supported.
- `Range`, `Adaptive`, ROI histograms, Line, Circle, Blob/Contour, Matching,
  acceptance gates, and downstream recipe semantics are unchanged.
- The analyzer does not know the operator's physical inspection intent. The
  operator must review the exact candidate and choose `Use T`.
- No suggestion is auto-applied. No Pipeline step, acceptance, XML, report,
  layer, or route is changed by analysis.
- The public synthetic pair proves one bounded workflow only. It does not prove
  unseen-data robustness, production optimization, or field qualification.
- Additional teaching suggestions require a separately named repeated blocker
  or explicit user selection; this completion does not authorize a generic
  easyTouch system.

## Durable Completion Record

```text
Status: Complete
Scope: One bounded Threshold Basic bright/dark significant-mode suggestion with exact histogram marker, explanation, explicit Use, same-source Undo, and public Good/Bad replay.
Acceptance criteria: Candidate explanation/geometry -> pass; rejected single-mode evidence -> pass; Analyze zero execution/side effects -> pass; Use/Undo and recovery -> pass; public Good 4 / Bad 1 -> pass; Threshold/CVR-02/CVR-06 regressions -> pass.
Verification: Debug and screenshot-runner builds; cvr07_threshold_suggestion; Threshold Basic/full; frozen Good/Bad; CVR-06; readiness.
Evidence: docs/reports/OPENVISIONLAB_THRESHOLD_TEACHING_SUGGESTION_20260728.md and artifacts/cvr07_threshold_suggestion_20260728.
Boundary / next dependency: CVR-00 still requires three real novice participants. CVR-08 is the earliest conditional queue row and requires one qualified locator that must drive at least two downstream ROIs where P212/P219 cannot express the task, or another explicit user selection.
```
