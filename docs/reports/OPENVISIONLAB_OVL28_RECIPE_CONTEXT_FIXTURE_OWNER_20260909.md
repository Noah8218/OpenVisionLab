# OpenVisionLab OVL-28 — Recipe context fixture owner

- **Status:** Complete for one independently verifiable deterministic smoke-fixture construction boundary.
- **Scope:** The repeated `CreateRecipeContextSmokePipeline` helper in `tools/PipelineViewerScreenshotSmoke/Program.cs` moved to `RecipeContextFixture.CreatePipeline`. The owner creates the named Threshold-step pipeline and preserves the existing `Main`-to-preview layer sequence for every requested step.
- **Intentional boundary:** `Program` remains responsible for choosing fixture names/counts, modifying specialized tool parameters after construction, saving/loading Recipe XML, creating Shell views, running Preview/Run, and asserting UI behavior. The fixture owner does not persist files, open WPF, or execute a pipeline. Recipe/XML, Preview/Run, Layer/ImageSpace, PropertyGrid, and product UI behavior are unchanged.
- **Dependency direction:** Smoke target methods call the concrete fixture owner and then continue their existing orchestration. The owner depends only on `VisionPipeline`/`VisionPipelineStep` model types; it does not reference `Program`, WPF, Shell, or product services.
- **State/data owner:** The caller owns the returned pipeline and decides when to mutate or persist it. `RecipeContextFixture` retains no state between calls.
- **Observable contract:** Pipeline name, requested step count, step names, Threshold tool type, first `Main` input, subsequent preview inputs, preview output names, and zero/negative step behavior are preserved.
- **Duplicate-work rule:** A repository-wide search found the helper only in `PipelineViewerScreenshotSmoke/Program.cs` with 26 call sites and no existing equivalent owner. Do not recreate or re-split this owner without a newly reproduced fixture-shape/XML defect, changed smoke contract, or proven responsibility conflict. Do not split the remaining fixture execution, summary, or UI methods by file size.

## Focused proof

`RecipeContextFixtureContract` verifies:

1. All existing context-fixture call sites delegate to `RecipeContextFixture` and the old helper declaration is absent from `Program`.
2. The owner contains only deterministic Threshold pipeline construction and has no smoke-entry-point or WPF dependency.
3. A three-step fixture preserves its name, step count, names, tool type, and linked layer sequence.
4. Zero and negative step counts preserve the previous empty-fixture behavior.
5. The constructed pipeline round-trips through the existing Recipe XML serializer without changing layer routing.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl28-recipe-context-fixture-debug-rerun-20260909\recipe-context-fixture-contract.txt`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl28-recipe-context-fixture-release-contract-20260909\recipe-context-fixture-contract.txt`

## Verification

- Debug build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform='Any CPU' -m:1 -nr:false --nologo` — 0 errors; the existing `CS8600` warning remains at `Program.cs:10512`.
- Debug focused contract: `--recipe-context-fixture-contract` — `RECIPE_CONTEXT_FIXTURE_CONTRACT=PASS|checks=6`.
- Debug OVL-27/26/25/24/23/22 regression contracts — `7/7`, `10/10`, `7/7`, `8/8`, `7/7`, and `10/10` PASS.
- Release build: same command with `-c Release` — 0 errors; the existing `CS8600` warning remains at `Program.cs:10512`.
- Release focused contract and OVL-27/26/25/24/23/22 regressions — `6/6`, `7/7`, `10/10`, `7/7`, `8/8`, `7/7`, and `10/10` PASS.
- Refactor audit: `REFACTOR_AUDIT=PASS|CSharpFiles=801|XamlFiles=59|PartialDeclarations=108|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- Documentation index: `DocumentationIndex=PASS|IndexedPaths=247|Routes=13|RootRedirects=102` after registering this report.
- WPF product theme/DPI runtime matrix was not run; this slice changes a window-free smoke fixture owner and does not claim product UI qualification.

## Junior readability review

**PASS for this boundary.** A junior developer can follow fixture construction from one named concrete owner, see the complete model shape in one short method, and understand that callers still own parameter changes, persistence, and UI execution.

## Next slice

Inspect the remaining `PipelineViewerScreenshotSmoke` fixture execution or summary/UI orchestration groups for one independently testable owner. Do not repeat OVL-12 through OVL-28 or split `Program.cs` by file size.
