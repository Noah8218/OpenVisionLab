# 2D Image-Buffer Integration Runtime Check — 2026-08-26

Status: Complete

Scope: explicit 2D consumer execution for a v2 Handoff with an `Image`
artifact. The adapter validates the transaction, decodes the copied local
image into an actual `OpenCvSharp.Mat`, invokes the existing
`VisionRecipeRunner`, and publishes a correlated Run Record plus v2 Result.

Acceptance criteria:

- Handoff validation and artifact identity check -> pass.
- Explicit acknowledgement does not execute the recipe -> pass.
- Good public PNG -> `Completed/Pass` -> pass.
- Missing-shape public PNG -> `Completed/Ng` -> pass.
- Explicit `Rejected` acknowledgement blocks execution and publishes no Result
  -> pass.
- Tampered source bytes are rejected before acknowledgement/run -> pass,
  observed as `ArtifactLengthMismatch`.
- No PNG is interpreted as a 3D height map -> pass; 3D uses its separate
  `HeightMap`/C3D consumer path.

Implementation:

- `src/OpenVisionLab/Core/Integration/TwoDIntegrationExchange.cs`
- `tools/VisionRecipeRunnerSmoke/TwoDIntegrationSmoke.cs`
- `tools/VisionRecipeRunnerSmoke/Program.cs`
- `src/OpenVisionLab/OpenVisionLab.csproj`
- `NuGet.Config`

Verification:

```powershell
dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Release --nologo

$taskTestRoot = 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d-integration'
$env:TEMP = $taskTestRoot
$env:TMP = $taskTestRoot
dotnet run --project tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Release --no-build -- `
  --integration-2d $taskTestRoot `
  docs\samples\public\EdgeDetection_Shapes_Synthetic_OK.png `
  docs\samples\public\EdgeDetection_Shapes_Synthetic_Missing_NG.png `
  docs\samples\public\Public_EdgeDetection_Shapes.pipeline.xml
```

Observed result: build completed with 0 warnings and 0 errors. The smoke
reported `Good=Pass`, `Bad=Ng`, explicit rejection blocked execution without a
Result, and tamper rejection occurred before acknowledgement.
Evidence was written under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d-integration\two-d-integration-20260826-075941-47ac5b24302e457cad02c15b4f85059d`.

Boundary / next dependency: this proves the UI-agnostic 2D consumer adapter,
real OpenCV execution path, and explicit Accepted/Rejected acknowledgement
contract. The process-level Machine Studio publisher to Dev consumer smoke is
recorded separately in
`docs/reports/OPENVISIONLAB_2D_CROSS_REPOSITORY_SMOKE_20260826.md`. It still
does not prove a WPF button, current EXE runtime layout, a single shared byte
representation between 2D intensity and 3D raw height, or clean-release
provenance from the dirty development worktrees.
