# OpenVisionLab 2D-028 Mat·stride·ROI view 입력 소유권 계약

Status: `Complete` for the source and focused-contract scope.

## User outcome

The existing `BitmapImageConverter` boundary now has a focused 2D-028 contract
for the gap that remained after PL-0006: non-contiguous ROI views with padded
row steps, caller/output ownership after release, and disposed input rejection.
No production converter rewrite or zero-copy change was made.

## Owner and call path

- owner: `src/OpenVisionLab/Common/Imaging/BitmapImageConverter.cs`
- test owner: `tools/VisionRecipeRunnerSmoke/MatViewOwnershipContract.cs`
- entry point: `VisionRecipeRunnerSmoke --mat-view-ownership-contract <evidenceDirectory>`
- call path: `MatViewOwnershipContract` → `BitmapImageConverter.ToBitmap/ToMat` →
  existing row-byte/stride validation and clone-copy boundary
- mutable-state writer: the caller-owned Mat/Bitmap remains the caller's
  responsibility; allocating overloads own and return the new output
- lifetime owner: the caller disposes the input; the returned Bitmap/Mat is an
  independent allocation owned by the caller after conversion

## Contract

`Mat` ROI views are accepted when their visible row bytes fit within the padded
step. Pixel values are copied by visible row bytes and guard rows/columns remain
unchanged. Caller inputs are not disposed or changed by conversion. Allocated
outputs remain readable after the caller releases the input. Disposed Bitmap or
Mat inputs, and unsupported Bitmap pixel formats, are rejected before an output
is returned.

## Verification

- Debug solution/smoke build: 0 errors, 16 existing nullable warnings in
  `PixelPerMmFiniteUnitContract.cs` and `PreviewRunEquivalenceContract.cs`.
- Debug executable copied to
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d028-debug-20260915` and
  2D-028 contract: 3/3 pass.
- Debug evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d028-debug-20260915\mat-view-ownership\mat_view_ownership_contract.txt`.
- Release solution/smoke build: 0 errors, the same 16 existing nullable
  warnings.
- Release executable copied to
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d028-release-20260915` and
  2D-028 contract: 3/3 pass.
- Release evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d028-release-20260915\mat-view-ownership\mat_view_ownership_contract.txt`.
- A first attempt to force one global custom intermediate/output directory on
  every ProjectReference failed before compilation with multi-target restore and
  WinFX asset errors (`NETSDK1005`/missing WPF references). It was a build-path
  experiment only; the accepted Debug/Release builds used the repository's
  existing standard project output and copied the runnable executables to D:.
- PL-0006 regression contract also passed 5/5 in Debug and Release from the
  copied D: executables, covering odd-width indexed rows, signed Bitmap stride,
  padded/submatrix destinations, palette/color conversions, round trips, and
  unsupported formats.

## Boundary

This proves the converter source and focused process contracts only. It does
not prove actual WPF rendering, themes, DPI, monitor placement, hardware/SDK
field behavior, long-running memory baselines, or unsupported 16-bit/alpha
normalization. Those remain `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Completion record

Status: `Complete`

Scope: 2D-028 non-contiguous Mat/ROI, padded stride, disposed input, and
caller/output ownership contract.

Acceptance criteria: C1 pixel and guard preservation plus explicit rejection —
Debug/Release 2D-028 contract 3/3; C2 caller/output ownership — dedicated
ownership case PASS; C3 focused builds, regression contract, and documented
owner path — all recorded above.

Verification: Debug and Release `dotnet build` of
`tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj`; copied D:
executables ran `--mat-view-ownership-contract` and
`--bitmap-converter-contract`.

Evidence: PL-0086, the two D: contract reports above, and the stable feature
contract/handoff entries.

Boundary / next dependency: 2D-029 must separately establish Gray/BGR/BGRA/
16-bit input meaning; this slice does not infer 16-bit support.
