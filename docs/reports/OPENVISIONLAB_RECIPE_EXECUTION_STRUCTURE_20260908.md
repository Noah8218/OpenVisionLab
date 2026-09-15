# Recipe execution structure — 2026-09-08

## Work contract

- User goal: junior developers can follow feature ownership and execution paths
  without tracing unrelated members across a large Shell partial type.
- Overall direction: improve real feature boundaries in Recipe, Learn, and Shell;
  complete one independently verifiable boundary per execution. This report
  covers Recipe validation execution only, not completion of the whole program.
- Source: Dev `codex/public-sample-ux-docs`,
  `d875559577c85984d54900df973a6fb35fb20146`, including the existing dirty worktree.
- Included: five explicit validation workflows, execution flags, result summaries,
  existing binding notifications, focused regression coverage, and developer navigation.
- Excluded: UI redesign, guard-policy changes, new parallelism, algorithm/SDK or
  Recipe/XML changes, Learn implementation, Original, commit, push, and deployment.
- Prerequisites: installed .NET/SDK dependencies and D-drive test storage are
  available. No external approval or hardware is required for this boundary.

## Ownership and acceptance criteria

| Before | Intended owner / proof |
| --- | --- |
| CommandSurface reads XML, executes samples, saves batches, and stores latest results | Existing `OpenVisionRecipeExecutionSessionViewModel` owns all five workflows and their mutable run state; facade contains no execution loops or direct sample execution/batch-save calls |
| Execution session holds mostly running flags | Session executes from supplied selection values and owns results without a Shell/View reference; direct tests construct it without a Window |
| Facade binds and mutates run summaries | Binding names and notification order remain compatible through forwarding; existing UI local-set workflow passes |
| Junior readers navigate several large partials | Existing `CODEBASE_STRUCTURE.md` maps command entry, execution, storage, presentation, and test locations with a concrete debugging example |

Validation plan: preserve a source/hash baseline; run the existing focused local
validation smoke before and after; build changed application and runner; exercise
all five session workflows, saved results, partial stop, and error recovery using
isolated synthetic inputs; verify structure, documentation index, and final task-only
diff against the starting dirty worktree.

## Implementation and verification

Three production files changed: `OpenVisionShellHostRecipeCommandSurface.cs`,
`.Handlers.cs`, and existing `Recipe/Review/OpenVisionRecipeExecutionSessionViewModel.cs`.
The former owner no longer has the five execution bodies, two catalog helpers,
three latest-result backing fields, or local runner. Its direct sample execution
calls decreased from 4 to 0, direct batch-save calls from 3 to 0, and local runner
calls from 1 to 0. The session has those calls and owns their execution state.

The two facade files decreased from 7,488 to 7,216 lines. The existing session
increased from 140 to 513 lines because it now performs its named responsibility.
No new production class or partial was added. Source comparison and an independent
review found no change in guards, suite routing, culture/text, exception boundaries,
await context, summary `PropertyChanging`/`PropertyChanged` ordering, or synchronous
save/command notifications. This is evidence for this boundary only.

Tests are in the new feature-owned
`tools/VisionRecipeRunnerSmoke/RecipeExecutionSessionContract.cs`; `Program.cs`
adds only dispatch/help. Navigation is in `CODEBASE_STRUCTURE.md` section 9.1.

Physical evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-recipe-execution-20260908`.

- Baseline Release UI runner/application build: PASS, 0 warnings, 0 errors.
- Baseline `wpf_shell_host_recipe_local_validation_set`: PASS.
- Baseline `wpf_shell_host_recipe_review_bundle`: FAIL after export assertions.
  `CaptureShellHostRecipeReviewBundle` opens Recipe Manager but does not enable
  `recipeAdvancedReviewToggle`; XAML `HostRecipeAdvancedTransferCommands` hides
  `HostRecipeExportXmlButton` until that toggle is checked. The failure is an
  existing unrelated smoke precondition, not evidence of a new execution defect.
  Its test and expectations are preserved. This target is not an acceptance gate
  for the moved validation workflows.
- Build outputs resolve through existing D-drive junctions; process TEMP/TMP and
  baseline artifacts are under the evidence root. No storage migration was performed.
- Application Debug x64 build: PASS, 0 warnings, 0 errors.
- Focused execution runner Release x64 build: PASS, 0 warnings, 0 errors. The first
  build identified a missing `System.IO` import in the new test; it was corrected
  and the final build passed. Both logs are retained.
- Release UI runner rebuild: PASS, 0 errors, one existing CS8600 warning at
  `PipelineViewerScreenshotSmoke/Program.cs:10722`, an unchanged file.
- Isolated session contract: PASS, 11 cases plus the Recipe XML byte invariant.
  Synthetic 8x8 Mean inputs prove normal sample/no report, selected suite/report,
  Good/Bad ordering and expected rejection, 12-item Product catalog sorting and
  10/12 progress cadence, local complete/one-image partial save, and all five
  missing-XML error paths. Each checks running-to-idle state, status notifications,
  save notification placement, and persisted batch/report counts as applicable.
- Existing local-validation UI target: PASS before and after, 1600x900. Both
  captures were inspected. No View, XAML, style, layout, or visible wording changed.
  These captures do not qualify the complete theme/layout/DPI/interaction matrix.
- The final monitor-positioned UI run also passed with exit 0. Two displays were
  detected; the smaller left `\\.\DISPLAY2` has bounds `(-1920,365,1920,1080)`.
  Recorded main-window rectangle `(-1760,431)..(-160,1331)` and child-window
  rectangle `(-1420,551)..(-500,1211)` are inside its working area. Evidence is
  `after-ui-positioned-final/monitor-placement.json` and `stdout.log`.
  The first wrapper run captured correct placement and a passing smoke but failed
  to retain the process exit-code handle; the D-only wrapper was corrected and
  rerun. The earlier wrapper evidence is preserved.
- Structural search: PASS (`structure-proof.txt`); documentation index: PASS,
  185 indexed paths, 13 routes, 102 redirects; `git diff --check`: PASS.
- Issue-ledger registration was attempted but its CLI refuses the existing
  invalid `PL-0010.json` (`state.next_action` on a terminal issue). That unrelated
  record was preserved, no `PL-0013` was registered, and this report plus the live
  handoff remain the durable record for this slice.

Reproduction (from Dev, with TEMP/TMP assigned to the evidence root):

```powershell
dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore
dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -p:UseAppHost=false -m:1 -nr:false --no-restore
dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -p:UseAppHost=false -m:1 -nr:false --no-restore
```

Copy the built execution runner binaries/dependencies to a fresh `runtime-release`
directory inside a D-drive evidence directory. Exclude runtime data (`RECIPE`,
`CONFIG`, `Log`, `docs`). Run `dotnet exec <runtime-release>/VisionRecipeRunnerSmoke.dll
--recipe-execution-session-contract <evidence-directory>`. The contract requires
that isolated D runtime, creates two small fixture catalogs there, and removes
only those generated catalogs on exit. It does not execute the actual full catalog.

Not run: full regression suite, all existing Recipe XML fixtures, full themes,
Wide/Compact combinations, pointer/keyboard state matrix, 100/125/150/175/200%
DPI qualification, clean standalone EXE deployment, or field hardware tests.
Recipe execution Close cancellation remains an existing separate limitation;
this ownership change does not add or claim that lifetime guarantee.

## Closure

Status: Complete

Scope: Recipe validation execution ownership and junior developer navigation,
one slice of the broader structural-refactoring request.

Acceptance criteria: five workflows/state moved with stale execution coupling
removed -> PASS (`structure-proof.txt` and task-only source diffs); independent
session execution, storage, error, and partial-stop tests -> PASS
(`recipe-execution-session-contract.txt`); existing local UI behavior -> PASS
(baseline and final UI logs/captures); navigation/index -> PASS.

Verification: commands and their actual results are recorded above. The task-only
allowlist is three production files, two test files, and four documentation files
(this report, structure guide, live handoff, and document index). Source snapshots,
hashes, tested binary hashes, diffs, and git status are in the physical evidence root.
Branch and HEAD remain the source identity stated above.

During the final hash comparison, three changes outside this slice appeared in
the skill registry, skill-development work contract, and a new skill-prompt-alignment
report. This task did not edit them. They are recorded separately in
`task-changed-files.csv`; shared documentation edits preserved the current content.
No reset, clean, Original mutation, commit, push, merge, or deployment occurred.

Boundary: this completes the Recipe execution slice, not the whole large-scale
refactor, UI qualification, the unrelated baseline review-bundle smoke repair, or
Recipe Close-cancellation hardening.

## Continuation

The next structural candidate is Learn Matching topic state/presentation, retaining
the existing simulation models and Window UI lifecycle. It requires fresh runtime
UI evidence before extraction completion. Recommended model: `gpt-5.6-terra`;
Reasoning effort: `high`.

The previous Pipeline Review revision-guard candidate remains a recorded stability
investigation. It must be reconfirmed against current code before any future fix;
the completed stale-callback lock must not be repeated as new work.
