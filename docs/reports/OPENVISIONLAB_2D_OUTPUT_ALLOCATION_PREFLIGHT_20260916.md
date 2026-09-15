# OpenVisionLab 2D Output Allocation Preflight (2026-09-16)

## Status

The safe pre-allocation boundary for 2D-036 is implemented and verified. The
product-level memory budget/cap portion remains blocked because 2D-034 is a
measurement baseline, not an approved user-facing limit.

## User problem and scope

Affine and RotateScale can derive a destination image larger than the input.
The required safety boundary is to reject impossible dimensions, non-finite
scales, and an unrepresentable `width × height × element-bytes` estimate before
the SDK/native tool is invoked. The change does not infer a RAM cap, downscale,
evict cache entries, or run work in parallel.

## Owner and call path

```text
Recipe Run
  -> VisionRecipeRunner
  -> VisionPipelineExecutionService
  -> VisionPipelineOutputAllocationGuard
  -> VisionPipelineAppToolFactory / vendored Vision SDK

Direct WPF Preview adapters
  -> VisionPipelineOutputAllocationGuard
  -> vendored Vision SDK
```

- Guard owner: `src/OpenVisionLab/Core/Pipeline/Validation/VisionPipelineOutputAllocationGuard.cs`
- Mutable result/layer writer remains `VisionPipelineContext.SetLayer`, which is
  reached only after a successful native result.
- Direct Preview owners are
  `OpenVisionNativeToolPreviewExecutor.ExecuteAffineTransformPreview` and
  `OpenVisionNativeSimplePreprocessPreviewExecutor.ExecuteRotateScalePreview`.
- The existing Affine maximum of `32768` per dimension is reused. RotateScale
  output dimensions are derived from the finite positive percentage scales using
  the SDK-observed rounded output rule and are bounded by the native integer
  dimension range.

## Contract and acceptance evidence

- Normal RotateScale and Affine Recipe executions complete with the source
  dimensions.
- Non-finite and overflowing RotateScale output is rejected before native
  execution with `RotateScaleInvalidScale`.
- Affine `32768 × 32768` remains an arithmetic-only accepted boundary, while
  `32769` is rejected with `AffineInvalidOutputSize`; the test does not allocate
  the maximum image.
- A rejected later step retains the previous successful result image/layer and
  does not call the native tool.
- Direct Affine and RotateScale Preview adapters use the same guard. Rendered
  WPF states are covered by the focused smoke below; the full supported UI
  matrix remains a separate runtime boundary.

Evidence artifacts:

- Debug contract: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d036-debug-20260916-final2\output-allocation-preflight-contract.txt`
- Release contract: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d036-release-20260916-final2\output-allocation-preflight-contract.txt`
- Debug focused WPF smoke: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d036-wpf-debug-20260916-run2\ui_precheck_report.md`
- Release focused WPF smoke: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d036-wpf-release-20260916-run1\ui_precheck_report.md`

## Verification

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" --no-restore` — 0 errors, 0 warnings.
- `dotnet build OpenVisionLab.sln -c Release -p:Platform="Any CPU" --no-restore` — 0 errors, 0 warnings.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform="Any CPU" --no-restore` — 0 errors, 19 existing warnings.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release -p:Platform="Any CPU" --no-restore` — 0 errors, 19 existing warnings.
- `VisionRecipeRunnerSmoke --output-allocation-preflight-contract` — Debug and Release exit `0`.
- `RunUiPrecheck.ps1` with `wpf_shell_host_rotate_scale_tool,wpf_shell_host_affine_transform_tool` — Debug and Release both `OK`; fresh screenshots show the RotateScale output layer/status and Affine review/overlay. The first Debug run exposed a cross-thread exception in this path; the two direct Preview adapters now marshal property snapshot and Affine result-review UI access through their View dispatcher.
- `git diff --check` — no whitespace errors; Git reported existing LF/CRLF
  normalization warnings for several already-dirty text files. The repository
  remains intentionally dirty with unrelated pre-existing changes.

## Remaining decision and runtime boundary

No approved product memory budget or configurable cap exists. The 2D-034
observations must not be converted into a universal limit by inference. To
complete the memory-budget portion, the product owner must choose one of:

1. define a documented per-operation or per-workspace memory budget and its
   failure message/setting scope; or
2. explicitly accept the arithmetic/int-dimension guard as the complete scope
   and defer operational low-memory policy.

Actual desktop EXE rendering, alternate themes/layouts/DPI/monitor/input
matrices, low-memory pressure, cancellation during native execution,
camera/hardware, and long-duration operation remain unverified. The focused
in-process WPF smoke covered the two changed Preview owners at the current
environment only. No SDK DLL was modified.
