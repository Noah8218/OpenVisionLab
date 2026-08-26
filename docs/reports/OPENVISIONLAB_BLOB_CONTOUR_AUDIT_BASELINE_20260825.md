# OpenVisionLab Blob/Contour Audit-Execution Baseline

Date: 2026-08-25 KST  
Repository: `C:\Git\OpenVisionLab_Dev` (Dev only)  
Issue: `PL-0010`  
Status: C1 baseline complete; one-pass removal blocked

## Scope

This report measures the current Blob/Contour object-evidence path. It does
not remove the audit execution, change the vendored SDK, or change stable
object-review behavior.

The current application path is:

1. Execute the configured Blob or Contour Tool.
2. Apply the configured object filters to the primary result.
3. Clone the Step and input, relax only the area limits, execute a second
   audit Tool, and use its candidates to reconstruct rejected-object rows.
4. If the audit returns unsuccessfully or throws, return an empty audit list
   and fall back to accepted-only rows.

The implementation is in
`src/OpenVisionLab/Core/Pipeline/Execution/VisionPipelineObjectResults.cs`
at the `Capture`, `TryCaptureUnfiltered`, and `CaptureAccepted` paths.

## Current baseline

The focused contract uses a repository-local synthetic 360x140 image with
five rectangular candidates and the same Blob/Contour dimension limits used by
the existing object-dimension contract. Each direct Tool call is warmed once
before timing.

| Tool | Primary wall | Audit wall | Primary SDK elapsed | Audit SDK elapsed | Primary/Audit candidates | Review rows | Accepted / rejected | Accepted overlays |
| --- | ---: | ---: | ---: | ---: | --- | ---: | --- | ---: |
| Blob | 0.585 ms | 0.877 ms | 0.437 ms | 0.698 ms | 1,2,3,4,5 / 1,2,3,4,5 | 5 | 1 / 4 | 1 |
| Contour | 0.681 ms | 0.813 ms | 0.558 ms | 0.679 ms | 1,2,3,4,5 / 1,2,3,4,5 | 5 | 1 / 4 | 1 |

The raw direct-call measurements show that the current evidence path performs
two Tool executions per Blob/Contour Step. They are representative fixture
measurements, not a production-corpus performance qualification. The current
Runner report's `ReportedStepElapsedMs`/`RunTotalMs` remains the primary SDK
result timing; the audit timing is not merged into that reported Step timing.

The current rows retain these reject reasons in the same run as the accepted
row:

- `Height 8 < MIN_HEIGHT 16`
- `Width 52 > MAX_WIDTH 30`
- `Width 8 < MIN_WIDTH 15`
- `Height 60 > MAX_HEIGHT 40`

The source-confirmed failure behavior is intentionally retained in this
baseline: a failed audit is converted to an empty list and the caller stores
accepted-only evidence. No test-only fault injection hook was added.

## SDK contract inventory

The installed SDK is assembly version `3.0.0.0`; the vendored manifest records
SDK version `3.0.0`. The current runtime reflection inventory reports:

- `BlobResult`: `Angle`, `Area`, `Bounding`, `Center`, `Index`, `UseDefect`.
- `ContourResult`: `Angle`, `Area`, `Bounding`, `Center`, `Contours`, `Index`.
- Neither result type exposes `AppliedLimits`, `AcceptedState`, or
  `RejectReason`.

Therefore the current SDK result contract is not sufficient to replace the
audit run while preserving the existing object rows, applied limits, reject
reasons, drawings, metrics, reports, timing, and selection behavior. The
existing `Index` values matched between the two representative runs, but that
does not by itself qualify a stable one-pass candidate identity contract.

## Acceptance and decision

| Criterion | Result | Evidence / boundary |
| --- | --- | --- |
| C1. Measure primary/audit cost and current failure/evidence behavior for representative Blob and Contour cases | PASS | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010_baseline_20260825_r4\audit_baseline.tsv`, `observations.txt`, and `completion.txt` |
| C2. One SDK execution supplies IDs, measurements, applied limits, accepted state, and exact reject reasons | BLOCKED | Current SDK result inventory lacks applied limits, accepted state, and reject reason. Requires a coordinated SDK contract change and parity evidence. |
| C3. Remove the audit rerun only after SDK identity and manifest are updated in normal release order | BLOCKED | Current manifest remains SDK `3.0.0`; no updated vendored SDK package or approved release-order change is available. |
| C4. Preserve object rows, drawings, selection, reject reasons, metrics, reports, acceptance, and timing | DEFERRED | Stable behavior remains unchanged. This criterion must be re-run against the new SDK/app path before removing the audit execution. |

## Verification

- `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug` — PASS, 0 warnings, 0 errors.
- `dotnet run --no-build --project tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -- --blob-contour-audit-baseline D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010_baseline_20260825_r4` — PASS, `Failures=0`.
- The contract wrote all generated output under the D-drive test root.

## Boundary / next dependency

PL-0010 remains open and blocked for one-pass implementation. The next
meaningful action is to obtain or explicitly coordinate an updated Vision SDK
contract that returns per-candidate identity, raw measurements, applied limits,
accepted state, reject reason, and the geometry required by the current review
and report paths. Until then, do not delete `TryCaptureUnfiltered`, weaken the
silent-failure path into a different unverified behavior, or claim one-pass
evidence parity.
