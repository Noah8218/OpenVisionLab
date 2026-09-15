# OpenVisionLab OVL-07 Run History orchestration owner — 2026-09-08

## Work contract

- User goal: keep the existing Recipe and UI behavior while making the
  CommandSurface structure easier for a junior developer to follow.
- This execution completes one independently verifiable boundary: persisted
  Run History lookup and comparison orchestration.
- The attached `OpenVisionLab_Implementation_Tasks_604c7fb.md` is historical
  evidence. Current source, Handoff, stable contracts, and the focused checks
  are authoritative for this slice.
- Existing dirty changes were preserved. No reset, clean, Original edit,
  staging, commit, push, merge, release, or deployment was performed.
- No automatic model, agent, or scheduled follow-up is authorized; the
  `openvisionlab-2d` heartbeat remains `PAUSED`.

## Before and after

Before, `OpenVisionShellHostRecipeCommandSurface.RunHistory.cs` directly read
`VisionPipelineBatchRunSummaryStorage.List`, converted summaries to options,
and invoked the existing presenter. `Handlers.cs` directly loaded two summary
files, resolved the baseline, built comparison rows, and selected the default
row. This mixed persisted-data access with the Shell's binding state.

After, the Window-free concrete
`OpenVisionRecipeRunHistoryOrchestrationOwner` owns that orchestration:

- `BuildRecentRunSelection` loads and converts the current recipe/pipeline
  inventory while preserving the previous selection.
- `BuildBaselineRunSelection` reuses the existing presenter fallback policy.
- `BuildComparison` resolves the baseline, loads both summaries, builds rows,
  and chooses the default comparison row.

The Shell still owns `RecentBatchRunOptions`, baseline/current selection
properties, filter state, `PropertyChanged` notifications, command requery, and
all visible wording. `OpenVisionRecipeRunHistoryPresenter` remains the pure
selection/filter/comparison rule owner. Recipe execution, Validation Set
execution, Preview/Run explicitness, Recipe/XML, Layer routing, and UI layout
were not changed.

## Structural proof

- `RunHistory.cs` contains no direct `VisionPipelineBatchRunSummaryStorage.List`
  or `Load` call.
- `Handlers.cs` delegates comparison refresh to
  `runHistoryOrchestrationOwner.BuildComparison`.
- The new owner references only the existing storage and presenter and has no
  `Window`, `UserControl`, WPF, or Shell dependency.
- The owner returns selection/comparison data; the Shell remains the state owner
  for existing bindings, so notification and selection semantics stay at the
  existing boundary.
- The structural checks are recorded in
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-run-history-orchestration-20260908\structure-proof.txt`.

## Changed files

Production:

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeRunHistoryOrchestrationOwner.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.RunHistory.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs`

Focused regression check:

- `tools/VisionRecipeRunnerSmoke/RecipeRunHistoryOrchestrationContract.cs`
- `tools/VisionRecipeRunnerSmoke/Program.cs`

Navigation and durable records:

- `AGENTS.md`
- `docs/admin/CODEBASE_STRUCTURE.md`
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
- `docs/LLM_DOCUMENT_INDEX.json`
- this report

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-run-history-orchestration-20260908`.

- `dotnet build .\src\OpenVisionLab\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpfCustomBuildEnabled=false -m:1 -nr:false --no-restore` — PASS, 0 warnings, 0 errors.
- `dotnet build .\src\OpenVisionLab\OpenVisionLab.csproj -c Release -p:Platform=x64 -p:WpfCustomBuildEnabled=false -m:1 -nr:false --no-restore` — PASS, 0 warnings, 0 errors.
- `dotnet build .\tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform=x64 -p:WpfCustomBuildEnabled=false -p:UseAppHost=false -m:1 -nr:false --no-restore` — PASS, 0 warnings, 0 errors.
- `dotnet build .\tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Release -p:Platform=x64 -p:WpfCustomBuildEnabled=false -p:UseAppHost=false -m:1 -nr:false --no-restore` — PASS, 0 warnings, 0 errors.
- `dotnet exec ...\VisionRecipeRunnerSmoke.dll --recipe-run-history-orchestration-contract <D-drive evidence>` — PASS, 4/4 in Debug and Release. It covered two saved runs, previous selection restoration, adjacent baseline fallback, summary comparison projection, and the empty inventory placeholder.
- Static owner/call-path proof — PASS (`structure-proof.txt`).
- No XAML or View changed; runtime UI smoke was not run for this source-only boundary.

The first test assertion expected the comparison row name to be exactly the raw
sample name. The existing comparison contract includes the Variant prefix
(`Default | sample-1`), so the assertion was corrected to recognize the
existing format. No production behavior or expectation was relaxed.

## Completion record

Status: Complete

Scope: persisted Run History inventory, baseline selection, summary loading, and
comparison-row orchestration moved behind a concrete Window-free owner.

Acceptance criteria:

- Existing Shell binding/selection contract retained -> PASS (focused contract
  and app build).
- Direct storage orchestration removed from the Shell refresh paths -> PASS
  (`structure-proof.txt`).
- New owner independently executable without a Window -> PASS (4/4 contract
  cases in Debug and Release).
- Documentation and no-duplicate-work boundary recorded -> PASS (Handoff,
  CODEBASE_STRUCTURE, AGENTS, and index updated).

Verification: all commands above passed; `git diff --check` passed for the
changed paths. The branch and HEAD remain `codex/public-sample-ux-docs` /
`d875559577c85984d54900df973a6fb35fb20146` with the pre-existing dirty
worktree preserved.

Not verified: full regression suite, all Recipe XML fixtures, WPF themes,
Wide/Compact layouts, pressed/focus/keyboard matrix, 125–200% DPI, clean EXE
deployment, and field hardware. These are outside this source-only slice.

Next single priority: OVL-06b full quantitative audit instrumentation |
Recommended model: `gpt-5.6-luna` | Reasoning effort: `low`.
