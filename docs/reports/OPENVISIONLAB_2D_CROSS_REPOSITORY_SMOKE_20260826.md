# 2D Cross-Repository Producer/Consumer Smoke — 2026-08-26

Status: Complete for the process-level producer/consumer exchange scope.

Scope: run the existing Machine Studio publisher in one .NET process, then run
the Dev 2D consumer in a separate .NET process against the same local exchange
directory. The consumer validates the copied artifacts, acknowledges the
Handoff explicitly, decodes the copied PNG into `OpenCvSharp.Mat`, executes the
existing `VisionRecipeRunner`, and publishes a correlated v2 Result.

Implementation:

- Machine Studio: `tools/MachineIntegrationProducerSmoke/` calls the existing
  `MachineIntegrationHandoffPublisher` and writes a producer manifest.
- Dev: `tools/VisionRecipeRunnerSmoke/TwoDIntegrationCrossRepoSmoke.cs`
  consumes the published transaction and verifies identity, acknowledgement,
  artifact hashes, Run Record, and Result correlation.
- Dev orchestration:
  `tools/RunTwoDIntegrationCrossRepoSmoke.ps1` builds and launches the two
  processes sequentially against one D-drive evidence root.

Acceptance criteria:

- Machine Studio publisher creates a `TwoD/Image` Handoff -> pass.
- Producer and consumer process boundaries use the same transaction directory
  -> pass.
- Consumer acknowledgement does not auto-run inspection -> pass.
- Copied public PNG and recipe are hash-validated and consumed -> pass.
- Consumer publishes `Accepted`, `Completed`, `Pass`, a Run ID, and a Run
  Record -> pass.

Verification:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File `
  tools\RunTwoDIntegrationCrossRepoSmoke.ps1
```

Observed result: Machine producer build passed with 0 warnings and 0 errors;
Machine producer infrastructure tests passed `32/32`; Dev consumer build
passed with 0 warnings and 0 errors; readiness and public sample checks passed.
The separate-process smoke passed with:

- Transaction: `5fc2be1c-cd2d-4d57-9051-3d133e5ddeef`
- Outcome: `Pass`
- Result: `Completed`
- Run ID: `2d-73c5236986bdf58112fbd33ba6d394db4115f597738468482c892ff5d2639911`
- Metrics: `39`

Evidence:

`D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\2d\2d-cross-repo-20260826-172213-9e5e95ec12654124a755e501cf36ee34`

Identity boundary: the v2 Handoff validator accepts only a declared clean
source identity. Both Handoff identities therefore declare `Clean` with the
actual repository HEAD commits; the orchestration output also records that
both development worktrees were `Dirty`. This proves the exchange and
execution path, not clean-release provenance.

Boundary: no WPF button or actual Machine Studio EXE interaction was exercised,
no 3D HeightMap/C3D consumer was run, no performance gate was run, and no
commit, push, PC restart, or original `C:\Git\OpenVisionLab` mutation was
performed.
