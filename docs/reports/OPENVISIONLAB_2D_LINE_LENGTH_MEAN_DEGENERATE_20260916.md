# OpenVisionLab 2D-033 — Line·Length·Mean 퇴화 입력과 유한 결과 계약

Status: Complete  
Date: 2026-09-16 KST  
Repository: `C:\Git\2D\Dev` (Dev only)

## Scope

이번 slice는 기존 `LineGauge`/`LineDistance`와 `MeanTool`의 실제 실행 경계를
검증하고, 퇴화 입력이 정상 계측으로 승격되지 않는지 확인하는 데 한정했습니다.
새 계측 모듈, 보정 알고리즘, 별도 후보 수집기, relaxed 재실행은 추가하지 않았습니다.

## Owner and call path

```text
VisionPipelineAppToolFactory
  -> existing LineGauge / VisionPipelineLineDistanceTool / MeanTool
  -> VisionToolResult (metrics, overlays, error)
  -> VisionPipelineMetricEnrichmentService + acceptance
  -> VisionRecipeRunner summary / Run History boundary
```

`VisionPipelineLineDistanceTool`은 이제 `CreateDistanceLines` 결과를
positive·finite 거리로 먼저 제한합니다. 0 길이/비유한 선만 남으면 기존
`LineGaugeEdgeNotFound` 경로로 돌아가며, `Min()`/`Average()`가 빈 목록에서
예외를 내거나 NaN/Infinity를 계측 결과로 노출하지 않습니다. 양의 거리는 기존
pixel/mm 변환 owner가 계속 작성합니다. Mean은 SDK `MeanTool`과 기존
`MeanValueMin/Max/Avg`·acceptance owner를 그대로 사용합니다.

## Acceptance and evidence

| Criterion | Result | Evidence |
| --- | --- | --- |
| 정상 LineDistance의 pixel/mm 유한 metric·overlay | PASS | Debug/Release contract report; normal Line has finite `DistancePx*`/`DistanceMm*` and line overlays. |
| 양·음 극성 경계 | PASS | Inverted public synthetic Line source with `BTOW` polarity retained finite pixel/mm metrics. |
| 정상 Mean 및 균일 dark Mean | PASS | Normal Mean metrics are finite; uniform dark Mean remains finite but fails the configured acceptance band. |
| 빈 입력/빈 ROI/no-edge/동일선 | PASS | Empty Mean returns explicit `Source image is not loaded`; empty ROI is rejected by Validator; uniform/no-edge and coincident same-line LineDistance return explicit reasons and no non-finite metric. |
| Debug/Release parity | PASS | Both configurations passed the new contract and focused regressions. |

## Commands and artifacts

- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform="Any CPU" --no-restore` — 0 errors; existing nullable warnings only.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release -p:Platform="Any CPU" --no-restore` — 0 errors; existing nullable warnings plus the smoke file's nullable diagnostics.
- `--line-length-mean-degenerate-contract` — PASS:
  - `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d033-debug-20260916-run4\line-length-mean-degenerate-contract.txt`
  - `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d033-release-20260916-run1\line-length-mean-degenerate-contract.txt`
- Focused Debug/Release regressions — all exit 0:
  - `--expected-failure-contract`
  - `--preview-run-reopen-equivalence-contract` (Mean, Blob, Contour, Matching, Line)
  - `--pixelpermm-finite-unit-contract`
  - Evidence roots are under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d033-debug-20260916-focused-*` and `2d033-release-20260916-focused-*`.

## Verification boundary

This is deterministic source/core/Runner evidence. Actual WPF EXE rendering,
theme/layout, keyboard/mouse interaction, DPI/monitor placement, camera/hardware,
long-duration operation, and field calibration remain unverified. The Line
`LineIntersection` presentation may still represent an explicit no-cross state;
this slice does not change that established contract.

## Durable closure

Status: Complete  
Scope: 2D-033 finite-result and degenerate-input contract plus the minimum
positive-finite guard in the existing LineDistance owner.  
Acceptance criteria: all three PL-0091 criteria pass with the artifacts above.  
Verification: Debug/Release builds, new contract, and focused regressions all
exited 0.  
Evidence: PL-0091 and this report.  
Boundary / next dependency: proceed to 2D-034 input-size memory/time/copy baseline;
WPF/runtime and hardware qualification remain separate.
