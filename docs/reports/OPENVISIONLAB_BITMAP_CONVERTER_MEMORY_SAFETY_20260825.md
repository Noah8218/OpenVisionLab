# OpenVisionLab BitmapImageConverter Memory Safety

Date: 2026-08-25 KST  
Issue: PL-0006  
Repository: `C:\Git\OpenVisionLab_Dev`  

## Status

Complete for the supported `Bitmap`/`Mat` conversion contract. PL-0005 external-binary evidence remains a separate open release blocker.

## Scope

- `src/OpenVisionLab/Common/BitmapImageConverter.cs`
- `tools/VisionRecipeRunnerSmoke/Program.cs`
- No new test framework or external dependency
- Existing user dirty changes and the Dev/original repository boundary were preserved

## Root cause addressed

- The non-contiguous 8bpp path copied the padded Bitmap source stride into a destination Mat row whose visible capacity could be only the image width.
- 24/32bpp row copies used row-step-sized copies in paths that require only visible pixel bytes.
- Signed Bitmap strides and Mat submatrix row capacity were not validated consistently.
- Allocating `ToMat` and `ToBitmap` overloads did not dispose their newly allocated output when a later conversion operation threw.

## Implemented contract

- Copy only visible pixel bytes for 1bpp, 8bpp, 24bpp, and 32bpp formats.
- Advance source rows with the actual signed `BitmapData.Stride` and destination rows with `Mat.Step()`.
- Validate source stride, destination row step, Mat storage end, channel compatibility, and supported formats.
- Preserve indexed palettes and BGR/BGRA ordering.
- Support destination submatrices without writing into guard rows.
- Dispose newly allocated `Mat`/`Bitmap` outputs when conversion fails.
- Return `NotSupportedException` with a non-empty reason for unsupported format/channel combinations.
- Preserve the existing default `Format32bppRgb -> 3-channel Mat` behavior.

## Verification

- `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug` — 0 warnings, 0 errors.
- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"` — 0 warnings, 0 errors.
- `dotnet run --no-build --project tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -- --bitmap-converter-contract ...` — PASS.
- Focused contract covered odd-width indexed guard rows, positive/negative stride, custom indexed palettes, 24/32bpp, submatrices, 1/8/24/32 round trips, and unsupported formats.
- `--runtime-stability-contract` — PASS.
- `HistoryContractCheck` — `HistoryContract=OK`.
- Current public Matching sample — Preview OK, final layer/result image/metrics/overlays preserved.
- Current batch-evidence run — 1 row completed, 1 pipeline pass, source/result/report evidence written.
- `OpenVisionReadinessCheck` — 13/13 contracts passed.
- `TestExternalReferences.ps1` — passed; known PL-0005 blocked entries remain explicitly reported.
- `TestPublicSampleAssets.ps1` — `CatalogRows=33`, `ManifestAssets=229`, `Pipelines=17`, PASS.

## Evidence

- Focused contract: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0006_current_20260825\bitmap_converter_r2\bitmap_converter_contract.txt`
- Runtime stability: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0006_current_20260825\runtime_stability\runtime_stability_contract.txt`
- Public result image: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0006_current_20260825\public_matching_result.png`
- Batch report/evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0006_current_20260825\batch_evidence\`

## Boundary

This proves the supported converter and current deterministic runner/runtime paths. It does not close PL-0005 licensing/provenance evidence, perform release publication, or authorize work in the original repository.
