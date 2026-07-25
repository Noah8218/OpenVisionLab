# EdgeBased Matching Performance Analysis

This document records the current EdgeBasedMatching performance state, measured bottlenecks, and vetted improvement directions. It is intended to prevent repeated speculative tuning that changes stable behavior without evidence.

## Scope

- Tool: `EdgeBasedMatching`
- Library path: `C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\EdgeBasedTemplateMatchingTool.cs`
- Benchmark harness: `.codex\EdgeBasedSampleRotationBenchmark`
- Current stable mode under analysis: edge search with optional Hybrid verification.
- Operator-facing defaults must remain conservative. Hybrid verification, coarse angle search, and position refine remain explicit options.

## Current Benchmark Artifact

Related research note:

- `docs/EDGE_BASED_MATCHING_IDEA_RESEARCH.md` records additional commercial/open algorithm ideas and the recommended no-code experiment order before the next algorithm patch.

Artifact:

```text
artifacts\edge_based_candidate_diagnostics_20260627
```

Files:

- `edge_image_rotation_results.csv`
- `edge_image_rotation_summary.csv`
- `edge_model_diagnostics_summary.csv`
- `candidate_diagnostics_rows.csv`
- `candidate_diagnostics_summary.csv`
- `phase_summary.csv`
- `edge_angle_probe.txt`
- `matching_robustness_probe.txt`
- `benchmark_console.txt`

Summary from the 10-sample EasyMatch rotation benchmark:

| Tool | Count | Pass | Avg ms | Median ms | Avg center error px | Avg angle error |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| EdgeHybrid:baseline | 50 | 50 | 124.935 | 114.588 | 0.518 | 0.2 |
| EdgeOptimized:baseline | 50 | 43 | 86.718 | 80.102 | 23.075 | 1.2 |
| ImageCoarse | 50 | 50 | 135.467 | 121.899 | 0.3 | 0.02 |
| ImageExhaustive | 50 | 50 | 139.637 | 124.082 | 0.3 | 0.02 |

Absolute timings can vary by machine load. Use the phase proportions below to choose optimization work.

## Phase Timing Findings

`EdgeBasedTemplateMatchingTool.CollectPhaseTimings` is an opt-in diagnostic flag. It must remain off by default and must not change recipe behavior or result scoring.

Current EdgeHybrid phase totals across 50 benchmark rows. After parallel Hybrid proposal, outer `HybridImageProposal` is wall time, while nested per-angle phases such as `HybridProposal.ScaledMatch` are cumulative worker time and should not be compared directly as wall time. After the Hybrid fast path, `SearchEdgeCandidate` and `HybridVerify` appear only on fallback rows.

| Phase | Total ms | Avg ms | Max ms |
| --- | ---: | ---: | ---: |
| HybridProposal.ScaledMatch | 4275.130 | 85.503 | 391.812 |
| SearchEdgeCandidate | 2311.894 | 72.247 | 201.165 |
| HybridImageProposal | 1941.206 | 38.824 | 135.065 |
| SourceGradient | 1107.611 | 22.152 | 42.613 |
| HybridProposal.RefineMatch | 799.765 | 15.995 | 157.479 |
| HybridVerify | 618.449 | 19.327 | 43.896 |
| HybridVerify.ImageScore | 326.625 | 10.207 | 36.445 |
| ModelCache | 142.675 | 2.854 | 10.355 |
| HybridProposal.ScaleSource | 120.086 | 2.402 | 16.208 |
| HybridProposal.RotateTemplate | 115.647 | 2.313 | 12.771 |

Conclusion:

1. The primary bottleneck is `SearchEdgeCandidate`.
2. The secondary bottleneck is `HybridImageProposal`, specifically `HybridProposal.ScaledMatch`.
3. Template/model creation, descriptor match, draw result, and preprocessing are not meaningful first optimization targets.

## 2026-06-27 Accepted Change

Accepted:

- Hybrid image proposal now evaluates independent angle proposals in parallel when the proposal angle count is at least 4.
- Template model cache and phase timing collection are protected with locks for safe parallel reads/writes.
- Result scoring remains unchanged. The public result `Score` is still the edge score.

Validation:

- `EdgeBasedSampleRotationBenchmark`: 50/50 EdgeHybrid pass, average 121.269 ms.
- `.codex\EdgeBasedAngleProbe`: all listed scenarios succeeded.
- `.codex\MatchingRobustnessProbe`: all listed scenarios succeeded.
- UI smoke: `artifacts\ui_precheck_edge_based_parallel_hybrid_proposal_20260627`.

## 2026-06-27 Search Hot Loop Change

Accepted:

- Candidate scoring now creates a per-model/per-source-width score context for repeated position scans.
- Rotated template models cache `offsetY * imageWidth + offsetX` index offsets.
- Candidate-invariant early-break thresholds are calculated once per score context instead of once per candidate point.
- Public score values and pass/fail semantics are unchanged.

Validation:

- `EdgeBasedSampleRotationBenchmark`: `EdgeHybrid` 50/50 pass, average 124.894 ms, median 107.545 ms.
- The directly targeted `SearchEdgeCandidate` phase improved from the previous accepted artifact's 72.855 ms average to 61.418 ms average in this run.
- `EdgeOptimized` average improved from 79.376 ms to 61.417 ms in this run.
- `.codex\EdgeBasedAngleProbe`: all listed scenarios succeeded.
- `.codex\MatchingRobustnessProbe`: all listed scenarios succeeded.
- UI smoke: `artifacts\ui_precheck_edge_based_score_context_20260627`.

Note:

- `EdgeHybrid` end-to-end average is still noisy because Hybrid proposal phases are parallel and sample-dependent. Use `SearchEdgeCandidate` phase timing to judge this specific hot-loop change.

## 2026-06-27 Hybrid Fast Path Change

Accepted:

- Hybrid verify may skip full edge candidate search only when all of these are true:
  - Hybrid verify is enabled.
  - The request is a single-match run.
  - The image proposal has `ImageVerifyScore >= 0.985`.
  - The recomputed edge score at that proposal is at least `max(SCORE_MIN, 0.70)`.
- If any condition fails, the code falls back to the existing full edge search plus Hybrid verification.
- Public result `Score` remains the recomputed edge score, not the image score.

Validation:

- `EdgeBasedSampleRotationBenchmark`: `EdgeHybrid` 50/50 pass, average 79.218 ms, median 80.660 ms.
- In the 50-row EdgeHybrid benchmark, 18 rows used the fast path and 32 rows used the fallback path.
- `.codex\EdgeBasedAngleProbe`: all listed scenarios succeeded.
- `.codex\MatchingRobustnessProbe`: all listed scenarios succeeded, including clutter distractor cases.
- UI smoke: `artifacts\ui_precheck_edge_based_hybrid_fast_path_20260627`.

Guardrail:

- Do not broaden this fast path to multi-match runs, lower the confidence thresholds, or use image score as public result score without new sample-backed validation.

## 2026-06-27 Model Quality Diagnostics Change

Accepted:

- `EdgeBasedTemplateMatchingTool.CollectMetrics()` now emits telemetry-only model diagnostics under the `Model.*` prefix.
- The diagnostics include template size, raw/final edge point counts, edge density, X/Y/area coverage, quadrant balance, point sample ratio, and simple 0/1 risk flags.
- The benchmark CSV now records `ModelMetrics` for edge-based rows so slow or unstable samples can be correlated with model quality.
- Runtime matching behavior, public result score, overlays, pipeline semantics, and recipe parameters are unchanged.

Current 10-sample diagnostic summary:

| Tool | Rows | Point limit hits | Low point risks | Low coverage risks | Avg raw points | Avg sampled points | Avg coverage area | Avg quadrant balance |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| EdgeOptimized:baseline | 50 | 45 | 0 | 0 | 1531.1 | 257.4 | 0.893 | 0.780 |
| EdgeHybrid:baseline | 50 | 45 | 0 | 0 | 1531.1 | 257.4 | 0.893 | 0.780 |

Validation:

- `Lib.Common.sln` Release x64 build: passed, 0 warnings, 0 errors.
- `EdgeBasedSampleRotationBenchmark`: `EdgeHybrid` 50/50 pass, average 71.738 ms, median 69.369 ms.
- `.codex\EdgeBasedAngleProbe`: all listed scenarios returned success.
- `.codex\MatchingRobustnessProbe`: Hybrid verify variants returned the true target on clutter distractor cases; pure edge step variants can still choose the distractor and must not be promoted as universal defaults.
- `OpenVisionLab.csproj` Debug x64 build: passed, 0 warnings, 0 errors.
- UI smoke: offscreen hit-test failed on the property-grid dialog button while listing the expected `...` button; visible capture passed at `artifacts\ui_precheck_edge_based_model_diagnostics_visible_20260627`.

Guardrail:

- `Model.*` diagnostics are telemetry only. Do not use them to reject matches, auto-change recipe parameters, or alter result scoring until a separate sample-backed acceptance rule is designed.
- The current diagnostics show that most samples hit `MAX_TEMPLATE_POINTS`; this is useful for tuning, but it is not proof that lowering `MAX_TEMPLATE_POINTS` is safe.

## 2026-06-27 Candidate Retention Diagnostics Change

Accepted:

- `EdgeBasedTemplateMatchingTool.CollectMetrics()` now emits telemetry-only candidate retention diagnostics under the `Candidate.*` prefix.
- The diagnostics record image proposal count, fast path count, fallback edge-search count, edge seed count, hybrid verification candidate count, verified count, image-proposal selection count, fallback selection count, and max proposal/search scores.
- `.codex\EdgeBasedSampleRotationBenchmark` now records `CandidateMetrics` in `edge_image_rotation_results.csv`.
- Runtime matching behavior, public result score, overlays, pipeline semantics, and recipe parameters are unchanged.

Current 10-sample EdgeHybrid candidate summary:

| Rows | Fast path rows | Fallback rows | Image proposal rows | Image proposal selected rows | Fallback selected rows | Avg edge seeds on fallback | Avg hybrid candidates on fallback | Avg verified on fallback |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 50 | 18 | 32 | 50 | 24 | 27 | 37.000 | 37.812 | 37.812 |

Interpretation:

- Fast path is already responsible for 18/50 rows. These are the safe high-confidence skips.
- Fallback still handles 32/50 rows and verifies about 38 candidates per fallback row. This is the next measurable optimization surface.
- Image proposal is available on all 50 rows and is selected on 24 rows. It is useful, but not dominant enough to replace edge search globally.
- Fallback is selected on 27 rows. This proves that a pure image-proposal-first shortcut would drop valid edge results.

Validation:

- `Lib.Common.sln` Release x64 build: passed, 0 warnings, 0 errors.
- `EdgeBasedSampleRotationBenchmark`: `EdgeHybrid` 50/50 pass, average 124.935 ms, median 114.588 ms. Absolute time was higher in this run, but relative order stayed: `EdgeHybrid` was still faster than `ImageCoarse` and `ImageExhaustive` on average.
- `.codex\EdgeBasedAngleProbe`: all listed scenarios returned success.
- `.codex\MatchingRobustnessProbe`: Hybrid verify variants returned the true target on clutter distractor cases; pure edge step variants can still choose the distractor.
- `OpenVisionLab.csproj` Debug x64 build: passed, 0 warnings, 0 errors.
- UI smoke: `artifacts\ui_precheck_edge_based_candidate_diagnostics_20260627` passed with visible capture.

Guardrail:

- `Candidate.*` diagnostics are telemetry only. Do not use them to skip fallback search or reduce candidate count until a separate candidate-retention rule is designed and validated.
- The current candidate summary shows why the previous coarse position pruning failed: many rows still need fallback edge search, and image proposal alone is not enough.

## 2026-06-27 Pyramid Diagnostics And Outline Drawing Change

Accepted:

- `EdgeBasedTemplateMatchingTool.CollectMetrics()` now emits telemetry-only `Model.Pyramid.*` metrics.
- The new diagnostics estimate level width/height, edge point count, edge density, coverage area, quadrant balance, usable flag, level count, and highest usable pyramid level.
- The diagnostics do not change search behavior, result scoring, recipe parameters, or fallback selection.
- Edge-based result drawing now renders the taught template edge model outline at the detected pose instead of drawing only a rotated rectangle.
- WPF preview overlay keeps the normal rotated box for image matching and feature matching, but does not add a second box over edge-based matching results.

Validation:

- `Lib.Common.sln` Release x64 build: passed with existing Lib.Common warnings only.
- `OpenVisionLab.csproj` Debug x64 build: passed, 0 warnings, 0 errors.
- `EdgeBasedSampleRotationBenchmark`: `EdgeHybrid` 50/50 pass, average 77.013 ms in `artifacts\edge_based_pyramid_outline_20260627\benchmark_console.csv`.
- `EdgeBasedAngleProbe`: all listed scenarios returned success.
- Visual result artifact: `artifacts\edge_based_pyramid_outline_20260627\EasyMatchDiePad1_edge_angle_on_outline.png`.

Guardrail:

- `Model.Pyramid.*` metrics are telemetry only. Do not use them to change pyramid levels, candidate counts, or score thresholds until a candidate survival audit proves the correct target remains alive.
- Edge-based matching output should remain template-edge-outline based. Do not replace it with a plain bounding box unless the user explicitly asks for a box display mode.

## 2026-06-27 Candidate Survival Audit

Accepted:

- `.codex\EdgeBasedSampleRotationBenchmark` now supports an opt-in `audit` mode.
- Normal benchmark output is unchanged unless `audit` or `--audit` is passed.
- Audit mode writes:
  - `candidate_survival_audit.csv`
  - `candidate_survival_summary.csv`
- The audit simulates coarse pyramid candidate retention by running downscaled source/template pairs and checking whether the expected target remains in top-N edge candidates.
- The audit separates `PositionSurvived` from `PoseSurvived` because a coarse level can keep the right location while reporting a poor angle that must be refined at a lower level.

Validation artifact:

```text
artifacts\edge_based_candidate_survival_audit_20260627
```

Summary:

| Level | Scale | Rows | Position survived | Pose survived | Notes |
| ---: | ---: | ---: | ---: | ---: | --- |
| 1 | 1/2 | 50 | 50/50 | 49/50 | Safe-looking candidate level on this sample set. |
| 2 | 1/4 | 50 | 40/50 | 36/50 | Switch templates are too small; Floppies keeps position but angle is unstable. |
| 3 | 1/8 | 50 | 0/50 | 0/50 | All templates are too small for this benchmark. |

Interpretation:

- A production pyramid candidate reducer must not start at 1/4 scale by default.
- 1/2 scale can be considered for a guarded position-candidate stage, but it still needs lower-level verification and fallback.
- 1/4 scale may be useful only when template dimensions and model diagnostics prove enough usable edge structure.
- Floppies shows why position and angle should be refined separately: the correct position survives, but the coarse angle can be wrong by 5 to 20 degrees.

Guardrail:

- Do not implement a hard pyramid candidate cutoff above 1/2 scale from this data.
- Do not remove original-resolution fallback.
- Do not treat coarse-level angle as final pose; use it only as a proposal that must be refined.
- Skip or downgrade coarse levels when the scaled template is below the minimum usable size.

## 2026-06-27 Pyramid Position Proposal Option

Accepted:

- `EdgeBasedMatchingProperty` now exposes an experimental `Pyramid proposal` option.
- The option is off by default.
- When enabled, the tool proposes candidate positions at 1/2 scale, verifies the proposals at full resolution, and falls back to the existing full-resolution search if the verified proposal is below `Pyramid proposal min score`.
- The default proposal count is `Pyramid proposal top N = 6`.
- The public result score remains the edge score. Hybrid verification still runs after the proposal path when enabled.

Validation artifact:

```text
artifacts\edge_based_pyramid_position_proposal_20260627
```

Focused benchmark comparison on the 10-sample EasyMatch rotation set:

| Variant | Tool | Pass | Avg ms | Median ms | Avg center error px | Avg angle error |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| Baseline | EdgeHybrid | 50/50 | 82.208 | 75.812 | 0.518 | 0.2 |
| Pyramid top6 | EdgeHybrid | 50/50 | 54.268 | 58.757 | 0.518 | 0.16 |
| Baseline | EdgeOptimized | 43/50 | 58.897 | 50.664 | 23.075 | 1.2 |
| Pyramid top6 | EdgeOptimized | 50/50 | 49.290 | 43.461 | 0.542 | 0.12 |
| Pyramid top4 | EdgeOptimized | 49/50 | 55.472 | 48.998 | 0.590 | 0.28 |

Proposal diagnostics for the recommended top6 run:

- `EdgeHybrid:pyramid`: 32 attempts, 31 accepted, 1 fallback.
- `EdgeOptimized:pyramid`: 50 attempts, 48 accepted, 2 fallback.
- Average `PyramidProposal.Search`: 13.214 ms for Hybrid rows, 24.556 ms for Optimized rows.
- Average `PyramidProposal.Verify`: 2.673 ms for Hybrid rows, 4.729 ms for Optimized rows.

Guardrail:

- Keep `Pyramid proposal` off by default until more real production samples are tested.
- Do not reduce `Pyramid proposal top N` below 6. Top4 already produced 49/50 on `EdgeOptimized`.
- Do not remove full-resolution fallback. The recommended top6 run still used fallback on some rows.
- Do not treat 1/2-scale angle as final. The proposal path must verify/refine at full resolution.
- Do not promote this to the only search path for small templates or heavily repeated patterns without a separate sample-backed check.

## 2026-06-27 Large Template Benchmark

Accepted:

- `.codex\EdgeBasedSampleRotationBenchmark` now supports a `large` / `--large` / `fullres` mode.
- Large mode uses full-resolution EasyMatch images and larger template windows instead of resizing to the default benchmark size.
- This mode is intended to check whether the pyramid proposal remains safe when the search image and template are larger.

Validation artifact:

```text
artifacts\edge_based_large_template_benchmark_20260627
```

Large benchmark summary:

| Variant | Tool | Pass | Avg ms | Median ms | Avg center error px | Avg angle error |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| Baseline | EdgeHybrid | 50/50 | 123.907 | 107.853 | 0.401 | 0.02 |
| Pyramid top6 | EdgeHybrid | 50/50 | 90.791 | 84.822 | 0.398 | 0.02 |
| Baseline | EdgeOptimized | 46/50 | 118.213 | 109.961 | 14.959 | 0.74 |
| Pyramid top6 | EdgeOptimized | 46/50 | 114.916 | 110.752 | 14.927 | 0.62 |
| Baseline | ImageCoarse | 50/50 | 178.027 | 186.224 | 0.341 | 0 |
| Pyramid run | ImageCoarse | 50/50 | 152.619 | 141.818 | 0.341 | 0 |

Interpretation:

- `EdgeHybrid + Pyramid proposal` is the best current edge-based recommendation for larger templates in this sample set.
- `EdgeOptimized` remains unsafe as a standalone production recommendation. All misses were `FloppiesLarge`, a repeated-pattern distractor case.
- Pyramid proposal improves Hybrid runtime while preserving 50/50 pass rate.
- Pyramid proposal does not fix repeated-pattern ambiguity without Hybrid verification.

Guardrail:

- Do not recommend `EdgeOptimized` alone for repeated-pattern scenes.
- Keep Hybrid verification available for large templates with repeated or similar edge structures.
- Use `large` mode when changing pyramid proposal, Hybrid verification, score thresholds, or candidate retention logic.

## 2026-06-28 Current Recommendation Recheck

Validation artifact:

```text
artifacts\edge_based_current_benchmark_20260628
```

Commands:

```powershell
dotnet build .codex\EdgeBasedSampleRotationBenchmark\EdgeBasedSampleRotationBenchmark.csproj -c Release -p:Platform=x64 -m:1 -nr:false
dotnet run --project .codex\EdgeBasedSampleRotationBenchmark\EdgeBasedSampleRotationBenchmark.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch" baseline
dotnet run --project .codex\EdgeBasedSampleRotationBenchmark\EdgeBasedSampleRotationBenchmark.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch" pyramid
dotnet run --project .codex\EdgeBasedSampleRotationBenchmark\EdgeBasedSampleRotationBenchmark.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch" baseline large
dotnet run --project .codex\EdgeBasedSampleRotationBenchmark\EdgeBasedSampleRotationBenchmark.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch" pyramid large
```

Default-size EasyMatch summary:

| Variant | Tool | Pass | Avg ms | Median ms | Avg center error px | Avg angle error |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| Baseline | EdgeHybrid | 50/50 | 87.766 | 83.737 | 0.518 | 0.2 |
| Pyramid top6 | EdgeHybrid | 50/50 | 108.223 | 88.519 | 0.518 | 0.16 |
| Baseline | EdgeOptimized | 43/50 | 57.131 | 48.609 | 23.075 | 1.2 |
| Pyramid top6 | EdgeOptimized | 50/50 | 81.645 | 68.423 | 0.542 | 0.12 |
| Baseline | ImageCoarse | 50/50 | 81.812 | 81.758 | 0.3 | 0.02 |
| Baseline | ImageExhaustive | 50/50 | 87.865 | 84.163 | 0.3 | 0.02 |

Large EasyMatch summary:

| Variant | Tool | Pass | Avg ms | Median ms | Avg center error px | Avg angle error |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| Baseline | EdgeHybrid | 50/50 | 100.499 | 98.912 | 0.401 | 0.02 |
| Pyramid top6 | EdgeHybrid | 50/50 | 104.093 | 95.568 | 0.398 | 0.02 |
| Baseline | EdgeOptimized | 46/50 | 114.013 | 95.666 | 14.959 | 0.74 |
| Pyramid top6 | EdgeOptimized | 46/50 | 137.262 | 132.89 | 14.927 | 0.62 |
| Baseline | ImageCoarse | 50/50 | 166.367 | 169.594 | 0.341 | 0 |
| Baseline | ImageExhaustive | 50/50 | 167.477 | 171.982 | 0.341 | 0 |

Current recommendation:

- Use `EdgeHybrid` as the safe edge-based preset when repeated or similar edge structures can exist.
- For large images/templates, `EdgeHybrid` is currently faster than the image matching baseline on this EasyMatch set while preserving 50/50 pass rate.
- `Pyramid proposal top N = 6` remains a useful explicit speed/reduction option, but it should stay operator-controlled. In this run it did not improve large `EdgeHybrid` average time, although median time was slightly lower.
- Do not promote `EdgeOptimized` as a production default. It is fast, and the default-size pyramid run passed 50/50 here, but large `FloppiesLarge` still failed 4/5 due to repeated-pattern ambiguity.
- Image matching remains the tighter precision baseline for center/angle error on these synthetic inserted-template samples.

## 2026-06-28 Pyramid Proposal Center Mapping Fix

Accepted:

- Pyramid proposal now maps the scaled proposal through `TemplateCenter`, then re-derives the full-resolution model origin for each refine angle.
- This fixes a large-template failure mode where the scaled model origin was reused directly at full resolution and the verification window shifted away from the true rotated candidate.
- Pyramid proposal acceptance also keeps a small weak-verified guard. If the verified full-resolution edge score barely clears the operator threshold, the tool falls back to the normal full search instead of locking in a weak proposal.
- Public score semantics remain unchanged. The reported result score is still the edge score.

Validation artifact:

```text
artifacts\edge_based_pyramid_center_mapping_guard_20260628
```

Default-size EasyMatch `pyramid` summary after the fix:

| Tool | Pass | Avg ms | Median ms | Avg center error px | Avg angle error |
| --- | ---: | ---: | ---: | ---: | ---: |
| EdgeHybrid:pyramid | 50/50 | 54.873 | 58.142 | 0.518 | 0.16 |
| EdgeOptimized:pyramid | 50/50 | 48.85 | 41.931 | 0.542 | 0.12 |
| ImageCoarse | 50/50 | 60.708 | 60.271 | 0.3 | 0.02 |
| ImageExhaustive | 50/50 | 65.465 | 64.255 | 0.3 | 0.02 |

Large EasyMatch `pyramid` summary after the fix:

| Tool | Pass | Avg ms | Median ms | Avg center error px | Avg angle error |
| --- | ---: | ---: | ---: | ---: | ---: |
| EdgeHybrid:pyramid | 50/50 | 61.405 | 59.705 | 0.398 | 0.02 |
| EdgeOptimized:pyramid | 50/50 | 78.613 | 71.542 | 0.497 | 0.02 |
| ImageCoarse | 50/50 | 107.013 | 105.261 | 0.341 | 0 |
| ImageExhaustive | 50/50 | 110.383 | 102.689 | 0.341 | 0 |

Interpretation:

- The coordinate mapping fix removes the observed `FloppiesLarge` repeated-pattern failures for `EdgeOptimized:pyramid` in the current 50-row large benchmark.
- `EdgeOptimized:pyramid` is now a valid speed preset candidate for this sample set, but it is still an explicit option. Do not make it the silent default without broader production samples.
- `EdgeHybrid:pyramid` remains the safer recommendation when repeated structures, low texture, or ambiguous edge-only candidates are expected.
- Image matching still has slightly tighter center/angle error on the synthetic inserted-template benchmark, but edge-based pyramid modes are now faster on these runs.

## Current Level Evaluation

Measured on the 10-sample EasyMatch rotation benchmark:

| Method | Pass | Avg ms | Median ms | Avg center error px | Avg angle error |
| --- | ---: | ---: | ---: | ---: | ---: |
| EdgeHybrid | 50/50 | 124.935 | 114.588 | 0.518 | 0.2 |
| EdgeOptimized | 43/50 | 86.718 | 80.102 | 23.075 | 1.2 |
| ImageCoarse | 50/50 | 135.467 | 121.899 | 0.3 | 0.02 |
| ImageExhaustive | 50/50 | 139.637 | 124.082 | 0.3 | 0.02 |

Assessment:

- `EdgeHybrid` is now slightly faster than the current OpenVisionLab image matching path on this sample set by average time and keeps the same 50/50 pass count.
- Image matching is still more precise on this benchmark. Its center and angle errors are lower because the generated benchmark uses inserted grayscale template patches, which naturally favors correlation-based matching.
- `EdgeOptimized` is faster but not acceptable as a production default because it still fails 7/50 cases.
- `EdgeHybrid` is the safer edge-based mode for repeated-edge distractors because it combines edge score with image proposal verification and keeps fallback search when the fast path is not high-confidence.

Compared with commercial geometric/shape matching:

- Current OpenVisionLab EdgeHybrid has several production-oriented pieces: gradient-direction scoring, rotated edge models, template/model cache, coarse angle search, Hybrid image proposal, high-confidence fast path, phase timing, and sample-backed guardrails.
- It is still below HALCON/Cognex/Euresys-class geometric matching. Missing pieces include a true multi-level edge model pyramid, candidate propagation across pyramid levels, model inspection diagnostics, coarse/fine acceptance diagnostics, scale/aspect handling, clutter/coverage/contrast/fit metrics, don't-care area handling, and model-quality warnings.
- HALCON shape matching stores a model over multiple pyramid levels and notes that the number of levels should be as large as possible while the model remains recognizable with enough points. It also supports pregeneration tradeoffs for rotations and model point optimization.
- Adaptive Vision documents pyramid search over position/orientation pairs and describes edge-based matching as matching gradient direction around edges, with model creation separated from runtime matching for reuse.
- Cognex PatMax uses a coarse candidate filter followed by a fine scoring step, and its documentation emphasizes that candidates rejected in the coarse step cannot be recovered by the fine step.
- Euresys EasyFind is feature-point based, advertises faster processing than normalized correlation, and supports rotation/scale invariance, pattern degradation tolerance, and don't-care areas.

Current practical grade:

- Against OpenVisionLab image matching: `A-` for pass-rate/speed balance on this benchmark, `B+` for precision because image matching is still tighter, `A-` for engineering diagnostics after adding `Model.*` and `Candidate.*` telemetry.
- Against commercial shape/geometric matching: `C+` to `B-`. The core direction is correct, but the model/pyramid/diagnostic layer is not yet comparable.

Next engineering target:

- Use `Model.*` and `Candidate.*` diagnostics to design candidate reduction safely. The rejected position-pyramid prototype showed that blindly reducing search can break correct candidates. Commercial-style coarse/fine search needs diagnostics that prove the correct candidate survives the coarse stage.

Rejected in the same cycle:

- A position-pyramid coarse search was tested and rejected.
- First attempt kept accuracy but worsened `EdgeHybrid` average to 188.144 ms.
- Reduced-seed attempt worsened accuracy to 49/50; `DiePad2` at +10 degrees returned angle 13 degrees and failed the angle tolerance.
- Do not reintroduce position-pyramid candidate search without a stronger design and sample-backed proof.

## External Algorithm References Checked

Relevant public references reviewed on 2026-06-27:

- HALCON shape-based matching uses stored multi-level image pyramids and warns that too many levels can lose the model while too few levels increase search time: https://www.mvtec.com/doc/halcon/12/en/create_shape_model.html
- HALCON `find_shape_model` treats interpolation as the usual pose-refinement tradeoff and notes that least-squares refinement adds computation: https://www.mvtec.com/doc/halcon/12/en/find_shape_model.html
- Cognex PatMax describes a two-step strategy: coarse candidate filtering first, then fine scoring only on retained candidates: https://docs.cognex.com/is_573/web/EN/ise/Content/Reference/PatMaxPatternTheoryOfOperation.htm
- Adaptive Vision documents pyramid and multi-angle matching, edge-based matching using edge/gradient direction, model creation separate from matching, and the need to limit angle/scale range: https://docs.adaptive-vision.com/5.1/studio/machine_vision_guide/TemplateMatching.html
- Euresys EasyFind markets geometric feature-point matching as faster and more robust than normalized correlation, with rotation/scale invariance and don't-care areas: https://www.euresys.com/en/products/software/easyfind/
- Continuous edge-gradient matching research describes orientation-aware edge similarity and FFT/GPU-style confidence map computation as a future direction, not a small local patch: https://d-nb.info/1037806964/34

## Recommended Implementation Order

1. Add a real coarse-to-fine pyramid path for `SearchEdgeCandidate`, but do not repeat the rejected scaled-position prototype.
   - Build source gradient pyramids and edge-model pyramids.
   - Search high pyramid levels first.
   - Keep only spatial/angle candidate seeds.
   - Refine candidates down to the original level.
   - This aligns with HALCON/Adaptive Vision/Cognex style candidate filtering and directly targets the largest measured bottleneck.
   - Required difference from the rejected attempt: preserve more spatial hypotheses with a validated diagnostic view of coarse-level misses before replacing full original search.

2. Reduce `HybridProposal.ScaledMatch` work.
   - Reuse source/template pyramid images where safe.
   - Restrict scaled image proposals to coarse candidate angle/position bands when enough edge candidates exist.
   - Keep image proposal fallback for repeated-edge samples such as Floppies.

3. Extend diagnostics before changing defaults.
   - Add pyramid-level template point count and highest usable pyramid level.
   - Add estimated candidate count from angle range and search area.
   - Use `Candidate.*` to prove the correct candidate survives each candidate-reduction stage.

4. Evaluate feature-point/geometric-model ideas only after CPU pyramid search is complete.
   - Useful for strong corners and distinctive shapes.
   - Risky for smooth edges or low-texture edges.

5. Evaluate GPU/OpenCL/FFT only as a separate prototype.
   - Current OpenVisionLab/OpenCvSharp path is CPU `Mat` based.
   - GPU work requires runtime and deployment decisions, not just one algorithm patch.

## Guardrails

- Do not change `SCORE_MIN` semantics. Public result `Score` remains the edge score for recipe compatibility.
- Do not auto-enable Hybrid verification, coarse angle search, or position refine.
- Do not change `HYBRID_VERIFY_TOP_N`, `MAX_TEMPLATE_POINTS`, pyramid, or angle defaults based on a single benchmark run.
- Do not reintroduce rotated verify-template or verify-template-edge caches without a same-sample benchmark win. A prior test preserved accuracy but worsened runtime.
- Do not optimize descriptor/draw/preprocess first; phase timing shows those are low-impact.
- Do not remove image-proposal fallback from Hybrid verification. It is required for repeated-edge distractor samples.

## 2026-06-27 Repeated Pattern Ambiguity Diagnostics

Added diagnostic-only metrics for repeated-pattern review:

- `Candidate.AmbiguousSelectionCount`
- `Candidate.AmbiguousAlternativeCount`
- `Candidate.MaxAmbiguousAlternativeScore`
- `Candidate.MinAmbiguousScoreGap`
- `Candidate.MaxAmbiguousDistance`

Definition:

- The selected result is compared with retained candidate seeds or Hybrid verification candidates.
- A candidate is counted as ambiguous when it is far enough from the selected template center and its edge score is within 0.03 of the selected score.
- These metrics do not change result selection, public `Score`, pass/fail, or recipe behavior.

Purpose:

- Repeated structures such as similar pads or repeated edge groups can produce multiple plausible edge candidates.
- Candidate-reduction work must preserve these diagnostics so we can see when a faster search is discarding or confusing plausible alternatives.
- Future UI can surface this as an operator warning, but the current implementation is intentionally telemetry-only.

## 2026-06-28 Scale Search And Subpixel Refinement

Implemented opt-in scale search for `EdgeBasedMatching`:

- `USE_FIND_SCALE`
- `FIND_SCALE_MIN`
- `FIND_SCALE_MAX`
- `FIND_SCALE_STEP`
- `USE_SUBPIXEL_REFINE`

Important implementation contract:

- Scale search builds scale-specific edge template models and searches angle x scale candidates.
- Public `Score` remains the edge score. `SCORE_MIN` semantics are unchanged.
- `MatchingResult.Scale` records the selected scale and is shown in result review when it is not 1.0.
- `MatchingResult.Center` for EdgeBasedMatching is the visual template center. Drawing code converts this center back to the rotated/scaled edge-model center before rendering the edge outline.
- Subpixel refine is a final-candidate local 3x3 score-peak refinement. It improves center reporting only and does not change threshold semantics.
- `Pyramid proposal` is bypassed when scale search is enabled. Scale search still verifies at the original working resolution and must not use a weak downscaled proposal as the final result.

Focused scale probe:

- Probe harness: `.codex\EdgeBasedScaleProbe`
- Sample directory: `C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch`
- Output directory: `C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch\EdgeBasedScaleProbe`
- Artifact: `artifacts\edge_based_scale_subpixel_20260628`
- Samples: 10 EasyMatch images
- Scale cases: 0.90x and 1.10x
- Pass criteria: selected candidate near the expected scaled template center and expected scale

Result:

| Count | Pass | Avg ms | Median ms | Avg center error px | Avg scale error |
| ---: | ---: | ---: | ---: | ---: | ---: |
| 20 | 20 | 189.566 | 155.292 | 0.717 | 0 |

Scale-image generation rule:

- The probe must resize the whole original image. It must not paste a scaled template into a separate background.
- This matters because pasted/composite images do not represent the user's requested scale scenario and can make a scale algorithm look better or worse for the wrong reason.
- Verified dimensions: `BOARD.JPG` 772x480, `Board_scale_0p90.bmp` 695x432, `Board_scale_1p10.bmp` 849x528. `Die1.tif` 500x512, `Die1_scale_0p90.bmp` 450x461, `Die1_scale_1p10.bmp` 550x563. `Die Pad 1.bmp` 640x484, `DiePad1_scale_0p90.bmp` 576x436, `DiePad1_scale_1p10.bmp` 704x532.

Rotation comparison after the same code changes:

| Method | Pass | Avg ms | Median ms | Avg center error px | Avg angle error |
| --- | ---: | ---: | ---: | ---: | ---: |
| EdgeHybrid:pyramid | 50/50 | 61.401 | 59.798 | 0.518 | 0.16 |
| EdgeOptimized:pyramid | 50/50 | 56.310 | 45.761 | 0.542 | 0.12 |
| ImageCoarse | 50/50 | 64.916 | 62.591 | 0.300 | 0.02 |
| ImageExhaustive | 50/50 | 70.378 | 67.739 | 0.300 | 0.02 |

Assessment:

- Scale search is functionally validated on whole-image resized samples. Wide scale ranges still increase the candidate count by the number of scale steps, so range/step must remain operator-controlled.
- Edge-based rotation speed is now competitive with image matching on the current EasyMatch rotation benchmark, while image matching remains more precise in generated patch tests.
- Before making scale search default-on, collect larger real samples and tune the scale range/step per product. Wide scale ranges are expensive by design.

Actual EXE comparison on the same whole-image 0.90x BOARD sample after adding Image Matching scale search:

- Smoke: `OpenVisionLab.exe --smoke matching-vs-edge-based-scale-comparison`
- Artifact: `artifacts\actual_exe_matching_vs_edge_scale_after_image_scale_20260628`
- Template: 120x90 patch from the original `BOARD.JPG`
- Source: whole original `BOARD.JPG` resized to 0.90x, 695x432
- Expected center: 189,76.5

| Tool | Result | Score | Center | Center error px | Box | Scale | Tact |
| --- | --- | ---: | --- | ---: | --- | ---: | ---: |
| Image Matching | Template Match | 94.458 | 189,76 | 0.500 | 108x80 | 0.9 | 166.7 ms |
| EdgeBasedMatching | Edge Match | 97.476 | 189.4,75.9 | 0.721 | 108x81 | 0.9 | 269.7 ms |

Post-pyramid-option actual EXE recheck:

- Smoke: `OpenVisionLab.exe --smoke matching-vs-edge-based-scale-comparison`
- Artifact: `artifacts\actual_exe_matching_vs_edge_scale_after_pyramid_20260628`
- Result: PASS
- Image Matching: `Score 94.458`, `Center 189,76`, `Box 108x80`, `Scale 0.9`, `Tact 228.0 ms`
- EdgeBasedMatching: `Score 97.476`, `Center 189.4,75.9`, `Box 108x81`, `Scale 0.9`, `Tact 257.1 ms`

Dedicated Image Matching pyramid proposal actual EXE smoke:

- Smoke: `OpenVisionLab.exe --smoke matching-pyramid-scale`
- Artifact: `artifacts\actual_exe_matching_pyramid_scale_20260628`
- Result: PASS
- XML persistence: `USE_PYRAMID_POSITION_PROPOSAL=true`, `PYRAMID_POSITION_TOP_N=8`, `PYRAMID_POSITION_MIN_SCORE=0.7`
- Preview: `Template Match / Count 5 / Score 94.458 / Center 189,76 / Box 108x80 / Angle 0 / Scale 0.9 / Tact 229.7 ms`
- Center error: `0.5 px`

Interpretation:

- Image Matching now has an opt-in target scale search. It creates scale-specific templates, reports result `Scale`, and should be used when normalized grayscale texture matching is suitable.
- EdgeBasedMatching also reported the correct scale and size. It still uses edge-score semantics, so its score is not directly comparable to Image Matching score.
- This comparison must not be read as a universal speed ranking. It is a focused actual-EXE check for scale-search behavior on one BOARD sample.

Latest 10-sample whole-image scale comparison after adding Image Matching pyramid position proposal:

- Probe: `.codex\MatchingScaleComparisonProbe`
- Artifact: `artifacts\matching_scale_pyramid_comparison_20260628`
- Samples: `BOARD.JPG`, `Die Pad 1.bmp`, `Die Pad 2.bmp`, `Die1.tif`, `Die2.tif`, `Frame 1.tif`, `Frame 4.bmp`, `Switch1.tif`, `Switch2.tif`, `Floppies.jpg`
- Source generation: whole original image resized to 0.90x and 1.10x. No pasted/composite scale targets.
- Template: high-edge patch selected from the original source per sample.
- Search: `USE_FIND_SCALE=true`, scale `0.85..1.15`, step `0.05`, angle search off, match count 5.
- ImageMatchingPyramid row additionally enables `USE_PYRAMID_POSITION_PROPOSAL=true`, `PYRAMID_POSITION_TOP_N=8`, and `PYRAMID_POSITION_MIN_SCORE=0.70`.

| Tool | Rows | Pass | Avg ms | Median ms | Avg center error px | Avg scale error | Avg score |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Image Matching | 20 | 20 | 626.925 | 570.830 | 0.845 | 0 | 97.425 |
| Image Matching + Pyramid proposal | 20 | 20 | 342.530 | 354.891 | 0.845 | 0 | 97.425 |
| EdgeBasedMatching | 20 | 20 | 236.045 | 227.583 | 0.717 | 0 | 94.539 |

Interpretation:

- Both tools found the correct scale on all 20 generated whole-image scale cases.
- Image Matching pyramid proposal reduced average scale-search time from 626.925 ms to 342.530 ms in the same run, while preserving pass count, score, center error, and scale error on this sample set.
- Pyramid proposal is an opt-in pruning path. It first proposes candidate locations on a 1/2 scale image, then verifies the proposed ROI at the original working resolution. If proposals are weak, the tool falls back to the normal full-resolution search.
- EdgeBasedMatching remains faster in this scale-only benchmark, but scores are not directly comparable across tools because edge score and normalized texture-match score have different semantics.
- Do not change public `SCORE_MIN`, `Scale`, or result score semantics to chase speed. Future speed work should extend candidate proposal, pyramid pruning, or ROI/operator guidance with sample-backed validation.

## 2026-06-28 EdgeBased Scale Multi-Match Seed Reuse

Accepted:

- `EdgeBasedMatching` now reuses the first full edge-search candidate seed pool for scale-search multi-match runs when all of these are true:
  - `USE_FIND_SCALE=true`
  - `USE_FIND_ANGLE=false`
  - `USE_HYBRID_VERIFY=true`
  - `USE_MULTI_ROI=false`
  - `NUM_MATCH > 1`
- If the retained seed pool cannot provide enough non-overlapping matches, the code falls back to the existing full edge search for the remaining results.
- Public result `Score`, `Center`, `Box`, `Angle`, and `Scale` semantics are unchanged.
- `Pyramid proposal` remains bypassed while scale search is enabled.

Why this was accepted:

- The prior scale path performed a full angle x scale x position search once per requested match. With `NUM_MATCH=5`, that meant up to five full scans over the same image.
- The accepted path keeps the first full-search candidate seeds and lets Hybrid verify choose multiple non-overlapping matches before running another full search.
- This is deliberately not a seed-only shortcut. The fallback full search remains the guardrail for samples where the first candidate pool does not retain enough valid alternatives.

Focused EdgeBased scale probe after this change:

- Probe: `.codex\EdgeBasedScaleProbe`
- Sample set: same 10 EasyMatch images, whole-image 0.90x and 1.10x resized sources.
- Result: 20/20 pass, average 170.187 ms, median 154.718 ms, average center error 0.717 px, average scale error 0.
- Previous accepted comparison baseline for EdgeBased scale in the same 20-row sample family was 236.045 ms average.
- Phase check from the probe: `SearchEdgeCandidate` average dropped to 75.718 ms.

Latest 10-sample whole-image scale comparison after seed reuse:

- Probe: `.codex\MatchingScaleComparisonProbe`
- Artifact: `artifacts\matching_scale_after_edge_seed_reuse_20260628`
- Result: 60/60 pass.

| Tool | Rows | Pass | Avg ms | Median ms | Avg center error px | Avg scale error | Avg score |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| EdgeBasedMatching | 20 | 20 | 116.962 | 117.169 | 0.717 | 0 | 94.539 |
| Image Matching | 20 | 20 | 586.619 | 586.511 | 0.845 | 0 | 97.425 |
| Image Matching + Pyramid proposal | 20 | 20 | 326.588 | 324.104 | 0.845 | 0 | 97.425 |

Actual EXE smoke:

- Smoke: `OpenVisionLab.exe --smoke edge-based-scale-matching`
- Artifact: `artifacts\actual_exe_edge_based_scale_after_speed_20260628`
- Result: PASS
- Preview: `Edge Match / Count 5 / Score 97.476 / Center 189.4,75.9 / Box 108x81 / Angle 0 / Scale 0.9 / Tact 113.5 ms`

Guardrail:

- Do not broaden this optimization to angle search or multi-ROI without a separate validation pass.
- Do not remove the fallback full search after seed-pool depletion.
- Do not change score thresholds, result score source, or result geometry to make this speed path look better.

## 2026-06-28 Pyramid Scale Candidate Survival Audit

Added a focused probe:

- Probe: `.codex\EdgeBasedPyramidScaleSurvivalProbe`
- Artifact: `artifacts\edge_based_pyramid_scale_survival_20260628`
- Samples: 10 EasyMatch images
- Target scales: `0.75`, `0.90`, `1.10`, `1.25`
- Search scale range: `0.70..1.30`, step `0.05`
- Pyramid levels audited: `1/2` and `1/4`
- Candidate target: whether the true center/scale remains in `Top 4`, `Top 8`, or `Top 12`
- Source generation: whole original image resize only. No pasted/composite scale images.

Result:

| Pyramid level | Rows | Tool success | Top4 survived | Top8 survived | Top12 survived | Avg ms |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| 1/2 | 40 | 40 | 37 | 37 | 37 | 247.408 |
| 1/4 | 40 | 32 | 25 | 25 | 25 | 44.894 |

Interpretation:

- A 1/2 pyramid candidate stage is promising, but not safe enough to become a candidate cutoff. It missed 3/40 whole-image scale cases even at Top12.
- A 1/4 pyramid stage is not currently viable as a first candidate stage for this sample set. Several small-template samples have no usable result at that level.
- This proves that a true pyramid/shape-model path needs candidate propagation plus fallback and probably model-quality gates. A hard low-resolution cutoff would create regressions.

Rejected production attempt:

- A guarded scale-enabled `Pyramid proposal` path was briefly tested and reverted.
- Artifact: `artifacts\matching_scale_edge_pyramid_proposal_20260628`
- Result: `EdgeBasedMatchingPyramid` passed only 19/20 rows and averaged 257.241 ms, while the existing `EdgeBasedMatching` path passed 20/20 and averaged 105.661 ms in that same run.
- Failure example: `Frame4` at target scale `1.10` reported center error `9.169 px`, exceeding the 8 px pass gate.
- Conclusion: keep `Pyramid proposal` bypassed when `USE_FIND_SCALE=true` until a redesigned candidate-propagation implementation proves both 20/20 accuracy and a speed win.

Post-revert verification:

- Artifact: `artifacts\matching_scale_after_pyramid_revert_check_20260628`
- Result: 60/60 pass.
- Actual EXE smoke: `artifacts\actual_exe_edge_based_scale_after_pyramid_audit_20260628`, PASS, `Scale 0.9`, tact `109.5 ms`.

| Tool | Rows | Pass | Avg ms | Median ms | Avg center error px | Avg scale error | Avg score |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| EdgeBasedMatching | 20 | 20 | 118.259 | 98.247 | 0.717 | 0 | 94.539 |
| Image Matching | 20 | 20 | 538.296 | 606.662 | 0.845 | 0 | 97.425 |
| Image Matching + Pyramid proposal | 20 | 20 | 298.040 | 256.760 | 0.845 | 0 | 97.425 |

## 2026-06-28 Pyramid Scale Gate Variant Probe

Added a follow-up probe to test whether a `1/2` working-level scale candidate gate can safely reduce the full-resolution search space:

- Probe: `.codex\EdgeBasedPyramidScaleGateProbe`
- Artifact: `artifacts\edge_based_pyramid_scale_gate_20260628`
- Samples: 10 EasyMatch images
- Target scales: `0.75`, `0.90`, `1.10`, `1.25`
- Search scale range: `0.70..1.30`, step `0.05`
- Working level: source/template resized to `1/2`
- Pass gate for candidate survival: center error `<= 16 px` in original-image coordinates and scale error `<= 0.051`
- Source generation: whole original image resize only. No pasted/composite scale images.

Result:

| Variant | Rows | Success | Survived | Rate | Avg ms | Median ms | Failures |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| `top12_step2_p220` | 40 | 40 | 37 | 0.925 | 242.006 | 216.635 | `DiePad1@0.9`, `DiePad1@1.25`, `DiePad2@1.1` |
| `top24_step1_p220` | 40 | 40 | 39 | 0.975 | 1465.468 | 1187.963 | `DiePad2@1.1` |
| `top24_step2_p220` | 40 | 40 | 37 | 0.925 | 381.293 | 320.850 | `DiePad1@0.9`, `DiePad1@1.25`, `DiePad2@1.1` |
| `top24_step2_p220_hybrid` | 40 | 40 | 39 | 0.975 | 464.210 | 388.924 | `Frame4@1.1` |
| `top24_step2_p320` | 40 | 40 | 38 | 0.950 | 469.376 | 334.735 | `DiePad1@1.25`, `DiePad2@1.1` |
| `top40_step2_p220` | 40 | 40 | 37 | 0.925 | 528.760 | 388.689 | `DiePad1@0.9`, `DiePad1@1.25`, `DiePad2@1.1` |

Interpretation:

- None of the tested `1/2` gate variants reached 40/40 survival.
- The hard failures are mostly repeated/similar-structure cases where scale is often correct but the center lands on a neighboring structure.
- `top24_step1_p220` and `top24_step2_p220_hybrid` each reached 39/40, but they are slower than the accepted EdgeBased scale path measured after seed reuse. They are not production candidates yet.
- This reinforces the current guardrail: do not enable EdgeBased `Pyramid proposal` when `USE_FIND_SCALE=true` until candidate propagation is redesigned and proves both full survival and a speed win.

## 2026-06-28 Scale Candidate Propagation Attempt

Tested a guarded production-style attempt to let EdgeBased scale search use the existing `Pyramid proposal` option:

- Attempted behavior: build `1/2` scale-aware proposal models, verify proposed candidates at original resolution, and fall back to the existing full search for weak or ambiguous proposals.
- Multi-match guard: `NUM_MATCH > 1` was forced back to the existing scale seed-reuse path because that path is already faster for repeated matches.
- Probe artifact before revert: `artifacts\matching_scale_edge_pyramid_guarded_single_multi_20260628`
- Revert verification artifact: `artifacts\matching_scale_after_candidate_propagation_revert_20260628`

Result before revert:

| Tool | Rows | Pass | Avg ms | Median ms | Notes |
| --- | ---: | ---: | ---: | ---: | --- |
| `EdgeBasedMatching` | 20 | 20 | 96.624 | 99.802 | existing multi-match seed-reuse path |
| `EdgeBasedMatchingPyramid` | 20 | 20 | 94.910 | 88.597 | proposal bypassed for multi-match after guard |
| `EdgeBasedMatchingSingle` | 20 | 19 | 63.387 | 60.069 | existing single-match path missed `DiePad2@0.9` under this probe's selected-template expectation |
| `EdgeBasedMatchingSinglePyramid` | 20 | 19 | 92.334 | 76.362 | slower and accepted a wrong repeated candidate for `DiePad2@0.9` |

Rejected failure:

- `EdgeBasedMatchingSinglePyramid`, sample `DiePad2`, target scale `0.9`
- Center error: `49.798 px`
- Candidate metrics showed `PyramidProposalAcceptedCount=1`, so the guarded proposal still accepted the wrong repeated structure before full search could correct it.

Decision:

- The production code was reverted to keep `Pyramid proposal` bypassed when `USE_FIND_SCALE=true`.
- Post-revert focused comparison passed 60/60 rows: EdgeBasedMatching 20/20, ImageMatching 20/20, ImageMatchingPyramid 20/20.
- Do not reintroduce scale candidate propagation through the current position-proposal path. A future attempt needs a stronger geometric/shape-model acceptance rule that can reject repeated structures before skipping full search.

## 2026-06-28 Scale Search Quality Metrics

Follow-up research direction:

- HALCON scaled shape matching treats scale as part of the returned pose and warns that overly high pyramid levels can select wrong instances when the model is not specific enough.
- Adaptive Vision describes pyramid matching as candidate propagation across levels, not a hard low-resolution cutoff.
- Cognex PatMax documentation emphasizes that robust patterns need unambiguous X/Y, rotation, and scale information; simple edge structures can be ambiguous for scale.
- Therefore the next safe step is diagnostics and acceptance metrics, not another direct speed shortcut.

Implemented diagnostic-only metrics in `EdgeBasedTemplateMatchingTool`:

- `Model.LowQuadrantBalanceRisk`
- `Model.ScaleCoverageWarningRisk`
- `Model.ScaleSearchRisk`
- `Candidate.ScaleSearchEnabled`
- `Candidate.SameScaleAmbiguousAlternativeCount`
- `Candidate.DifferentScaleAmbiguousAlternativeCount`
- `Candidate.MaxAmbiguousScaleDelta`
- `Candidate.ScaleAmbiguityRisk`

Probe update:

- Probe: `.codex\MatchingScaleComparisonProbe`
- Artifact: `artifacts\matching_scale_quality_metrics_20260628`
- Result: 60/60 pass.
- The CSV now contains `ModelMetrics` for EdgeBased rows in addition to `PhaseMetrics` and `CandidateMetrics`.

Observed example:

- `DiePad2@0.9` still passes the full EdgeBased scale path with center error `7.811 px`, but it reports heavy ambiguity:
  - `Candidate.AmbiguousAlternativeCount=52`
  - `Candidate.SameScaleAmbiguousAlternativeCount=13`
  - `Candidate.DifferentScaleAmbiguousAlternativeCount=39`
  - `Candidate.MaxAmbiguousScaleDelta=0.25`
  - `Candidate.MinAmbiguousScoreGap=-0.094`
- This is exactly the class of sample where a low-resolution proposal must not skip full search.

Guardrail:

- `ScaleAmbiguityRisk` is diagnostic only. It is intentionally conservative and can be true for samples that still pass full search.
- Do not reject matches, auto-tune recipe values, or accept pyramid proposals based on these metrics until a separate acceptance-rule probe proves both pass rate and speed benefit.

## 2026-06-28 Scale Proposal Acceptance Rule Probe

Goal:

- Re-check the rejected scale-aware pyramid proposal artifact offline before changing production behavior again.
- Identify whether candidate diagnostics can block the known wrong repeated-structure proposal while preserving correct proposal accepts.

Probe:

- Project: `.codex\EdgeBasedScaleAcceptanceRuleProbe`
- Input artifact: `artifacts\matching_scale_edge_pyramid_guarded_single_multi_20260628`
- Quality-metrics artifact: `artifacts\matching_scale_quality_metrics_20260628`
- Output artifact: `artifacts\edge_based_scale_acceptance_rule_probe_20260628`

Result:

| Rule | Wrong blocked | Good proposal blocked | Full-search recoverable | Decision |
| --- | ---: | ---: | ---: | --- |
| `BaselineNoGuard` | 0/1 | 0/15 | 0/1 | rejected |
| `ProposalAmbiguousAny` | 1/1 | 0/15 | 1/1 | viable |
| `ProposalAmbiguousAndGapLe003` | 1/1 | 0/15 | 1/1 | preferred candidate |
| `ProposalAmbiguousDistanceGe32` | 1/1 | 0/15 | 1/1 | viable, but distance threshold needs broader validation |
| `QualityAmbiguousGe20` | 1/1 | 3/15 | 1/1 | safe but higher fallback cost |
| `QualityDifferentScaleAmbiguousGe10` | 1/1 | 4/15 | 1/1 | safe but higher fallback cost |
| `QualityScaleAmbiguityRisk` | 1/1 | 12/15 | 1/1 | too conservative |

Known wrong proposal:

- `EdgeBasedMatchingSinglePyramid`, sample `DiePad2`, target scale `0.9`
- Proposal-accepted row failed with center error `49.798 px`.
- Proposal diagnostics: `Candidate.AmbiguousAlternativeCount=2`, `Candidate.MinAmbiguousScoreGap=-0.03`, `Candidate.MaxAmbiguousDistance=50.359`.
- The same sample/scale is recoverable by the existing full multi-match EdgeBased search, so a production guard must fall back to full search rather than fail the match.

Current decision:

- No production behavior was changed in this probe.
- If scale candidate propagation is retried, start with an acceptance guard equivalent to `ProposalAmbiguousAndGapLe003`: when a proposal-accepted candidate has ambiguous alternatives and the ambiguous score gap is `<= 0.03`, do not accept the proposal result; fall back to the existing full-resolution search.
- This rule is only validated against the current 20-row rejected artifact. Before production promotion, rerun it on the broader whole-image scale set and actual EXE smoke.

## 2026-06-28 Diverse Local Image Scale Probe

Goal:

- Expand scale-search validation beyond `EasyMatch`.
- Use the sample images under `C:\Git\OpenVisionLab_Dev\bin\Debug` with directory-balanced sampling.
- Keep production behavior unchanged and measure the current full-resolution EdgeBased scale path.

Probe:

- Project: `.codex\EdgeBasedDiverseScaleProbe`
- Root: `C:\Git\OpenVisionLab_Dev\bin\Debug`
- Artifact: `artifacts\edge_based_diverse_scale_probe_20260628_60`
- Selection: 60 images from multiple top-level folders, capped per folder, with whole-image `0.90x` and `1.10x` resized sources.
- Template selection: automatic high-edge crop for probe coverage only. This is not a replacement for operator-taught templates.

Result:

| Metric | Value |
| --- | ---: |
| Rows | 120 |
| Pass | 104 |
| Fail | 16 |
| Average elapsed | `177.090 ms` |
| Median elapsed | `116.792 ms` |
| Average passed center error | `0.525 px` |
| Average passed scale error | `0.002` |
| `Candidate.ScaleAmbiguityRisk=1` rows | 99 |
| Different-scale ambiguous rows | 95 |
| `Model.ScaleSearchRisk=1` rows | 0 |

Failure classification:

- 8 rows are template/model preparation failures from low-edge or reference/depth-map style images where the automatic crop found no usable edge template.
- 8 rows are geometry failures where the automatically selected crop was not specific enough, usually repeated structures or low-score large capture images:
  - `CAPTURE\_20230410_204554.jpeg @0.90x`
  - `CAPTURE\_20230410_204555.jpeg @0.90x`
  - `CAPTURE\_20230410_234037.jpeg @0.90x/@1.10x`
  - `EasyMatch\Die Pad 1.bmp @0.90x/@1.10x`
  - `EasyMatch\Die Pad 2.bmp @0.90x`
  - `EasyOCR2\MedicineBlister\Blister1.bmp @0.90x`

Phase average over passed rows:

| Phase | Average ms |
| --- | ---: |
| `SearchEdgeCandidate` | `87.602` |
| `HybridVerify` | `37.619` |
| `SourceGradient` | `35.802` |
| `DrawResult` | `4.998` |
| `ModelCache` | `2.367` |
| `Preprocess` | `0.260` |

Interpretation:

- The current full-resolution EdgeBased scale path is strong on diverse usable templates: pass rows average about half-pixel center error.
- The algorithm is not a universal auto-crop detector. Repeated or weakly specific automatically selected templates can match the wrong repeated location. Product UX should still guide operators toward unique template regions and/or ROI constraints.
- `ScaleAmbiguityRisk` is common even when the final full search passes. This reinforces that it must stay diagnostic and must not be used as a hard failure rule.
- The next speed target remains `SearchEdgeCandidate`, followed by `HybridVerify` and `SourceGradient`. Model cache and preprocess are not the broad-sample bottleneck.
- A future scale proposal shortcut must use ambiguity only to decide "fall back to full search", never to fail a match directly.

## Verification Commands

Use focused checks, not a broad regression suite, unless shared routing/UI code is changed.

```powershell
dotnet build C:\Git\Library-Noah\Lib.Common.sln -c Release -p:Platform=x64 -m:1 -nr:false
Copy-Item C:\Git\Library-Noah\Lib.OpenCV\bin\Release\netstandard2.0\Lib.OpenCV.dll .\dll\Library-Noah\Lib.OpenCV.dll -Force
dotnet build .\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
dotnet run --project .codex\EdgeBasedSampleRotationBenchmark\EdgeBasedSampleRotationBenchmark.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch" baseline
dotnet build .codex\EdgeBasedScaleProbe\EdgeBasedScaleProbe.csproj -c Release -p:Platform=x64 -m:1 -nr:false
dotnet run --project .codex\EdgeBasedScaleProbe\EdgeBasedScaleProbe.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch"
dotnet build .codex\EdgeBasedPyramidScaleSurvivalProbe\EdgeBasedPyramidScaleSurvivalProbe.csproj -c Release -p:Platform=x64 -m:1 -nr:false
$out = "artifacts\edge_based_pyramid_scale_survival_20260628"; dotnet run --project .codex\EdgeBasedPyramidScaleSurvivalProbe\EdgeBasedPyramidScaleSurvivalProbe.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch" $out
dotnet build .codex\EdgeBasedPyramidScaleGateProbe\EdgeBasedPyramidScaleGateProbe.csproj -c Release -p:Platform=x64 -m:1 -nr:false
$out = "artifacts\edge_based_pyramid_scale_gate_20260628"; dotnet run --project .codex\EdgeBasedPyramidScaleGateProbe\EdgeBasedPyramidScaleGateProbe.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch" $out
dotnet run --project .codex\MatchingScaleComparisonProbe\MatchingScaleComparisonProbe.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch" "artifacts\matching_scale_pyramid_comparison_20260628"
$out = "artifacts\matching_scale_after_edge_seed_reuse_20260628"; dotnet run --project .codex\MatchingScaleComparisonProbe\MatchingScaleComparisonProbe.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch" $out
$out = "artifacts\matching_scale_after_pyramid_revert_check_20260628"; dotnet run --project .codex\MatchingScaleComparisonProbe\MatchingScaleComparisonProbe.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch" $out
$out = "artifacts\matching_scale_after_candidate_propagation_revert_20260628"; dotnet run --project .codex\MatchingScaleComparisonProbe\MatchingScaleComparisonProbe.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch" $out
$out = "artifacts\matching_scale_quality_metrics_20260628"; dotnet run --project .codex\MatchingScaleComparisonProbe\MatchingScaleComparisonProbe.csproj -c Release -p:Platform=x64 -- "C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch" $out
$out = "artifacts\actual_exe_edge_based_scale_20260628_final"; Start-Process -FilePath .\bin\Debug\OpenVisionLab.exe -ArgumentList @("--smoke","edge-based-scale-matching","--output",(Resolve-Path $out)) -Wait
$out = "artifacts\actual_exe_edge_based_scale_after_speed_20260628"; Start-Process -FilePath .\bin\Debug\OpenVisionLab.exe -ArgumentList @("--smoke","edge-based-scale-matching","--output",(Resolve-Path $out)) -Wait
$out = "artifacts\actual_exe_matching_vs_edge_scale_after_image_scale_20260628"; Start-Process -FilePath .\bin\Debug\OpenVisionLab.exe -ArgumentList @("--smoke","matching-vs-edge-based-scale-comparison","--output",(Resolve-Path $out)) -Wait
$out = "artifacts\actual_exe_matching_pyramid_scale_20260628"; Start-Process -FilePath .\bin\Debug\OpenVisionLab.exe -ArgumentList @("--smoke","matching-pyramid-scale","--output",(Resolve-Path $out)) -Wait
powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_matching_pyramid_property_grid" -FailOnWarn -OutputDir "artifacts\ui_precheck_matching_pyramid_property_grid_20260628" -WpgCustomBuildEnabled false -TimeoutSeconds 180
powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_edge_based_matching_tool" -FailOnWarn -OutputDir "artifacts\ui_precheck_edge_based_hybrid_fast_path_20260627" -WpgCustomBuildEnabled false -TimeoutSeconds 420
```
