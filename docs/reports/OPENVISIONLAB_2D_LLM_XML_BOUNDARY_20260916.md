# OpenVisionLab 2D-052 LLM/XML Boundary Review

Date: 2026-09-16 KST
Repository: `C:\Git\2D\Dev`
Status: **Complete — maintenance-mode boundary verified; activation-level transaction/Undo is N/A and deferred**

## 1. Scope and decision

`2D-052` asks whether an LLM/XML proposal can be reviewed, validated, explicitly
applied, explicitly run, and reverted without silently executing or partially
mutating a Recipe. The attached plan explicitly allows an `N/A` close when the
LLM path is deprecated or maintenance-only.

The current product contract is maintenance-only. P196 froze planned LLM
expansion and retained only compatibility surfaces: Assistant/Guided Setup,
XML guide/catalog, strict validation, correction display, review/diff, and
explicit import. This review therefore verifies that boundary and does not
activate a provider, autonomous agent, prompt family, benchmark, or automatic
execution path.

The existing import operation creates a new uniquely named Pipeline and then
updates the active pointer; it does not overwrite the previous Pipeline. A
transaction spanning copied dependencies/reference images, the new Pipeline
XML, and the active pointer is not currently an owned contract, and no separate
Undo command exists for this path. We do not claim that unverified rollback
behavior. The transaction/Undo part is `N/A` for the frozen maintenance-mode
scope and becomes a new maintenance issue only if the product direction is
explicitly reopened.

## 2. User workflow and owner map

```text
LLM draft paste/load or review-bundle selection
  -> RecipeCommandSurface.ValidateLlmXmlDraftText(false)
  -> OpenVisionRecipeLlmDraftValidationService
  -> OpenVisionRecipeDependencyReviewService (read-only scan)
  -> OpenVisionRecipeLlmDraftReviewOwner (read-only import/diff projection)
  -> explicit Import command
  -> dependency/reference copy + new Pipeline Save + active-pointer Save
  -> explicit Preview/Run remains a separate operator action
```

- View/DataContext owner: Recipe Manager in `OpenVisionShellHostView.xaml`;
  command/state owner: `RecipeCommandSurface`.
- Validation/policy owner:
  `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeLlmDraftValidationService.cs`.
- Dependency side-effect owner:
  `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeDependencyReviewService.cs`.
- Read-only review owner:
  `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeLlmDraftReviewOwner.cs`.
- Persistence owner:
  `src/OpenVisionLab/Core/Pipeline/Storage/VisionPipelineStorage.cs`.
- Shortest reading order:
  `RecipeCommandSurface.ImportLlmXmlDraft` -> LLM validation service ->
  dependency review service -> draft review owner -> pipeline storage.

## 3. Acceptance disposition

| 2D-052 condition | Current result | Disposition |
| --- | --- | --- |
| Draft/paste/load, validation, review and diff do not run Preview/Run or mutate layers/routing | Current-source Guided Setup and review-bundle smoke pass; review owner contract is 3/3 | Verified |
| Invalid XML, unsupported/custom `Inspection.*`, missing/changed dependencies, and stale draft cannot be imported | Current-source Guided Setup assertions and review-bundle import pass; edited intent invalidates import readiness | Verified for the exercised cases |
| Valid draft requires an explicit Import, and execution remains a separate explicit action | Source call path and current-source UI smoke pass; no automatic Preview/Run observed | Verified |
| Manual import/save/reopen of the resulting Pipeline | Current-source Recipe Manager flow imports a uniquely named Pipeline and reloads it from storage | Verified for the exercised synthetic flow |
| Failed persistence or mid-copy apply restores all prior files/assets | No transaction owner or fault-injection contract exists for this path | **N/A/deferred under maintenance mode; not claimed** |
| Undo restores an original snapshot | Import creates a new Pipeline instead of replacing the active Pipeline; no LLM Import Undo command exists | **N/A/deferred; reopen only with an explicit product decision** |

The last two rows are deliberately not weakened into a pass. If future product
direction requires atomic import or operator Undo, reopen this boundary with a
new issue that first defines the transaction owner, asset staging/cleanup,
pointer recovery, and a deterministic fault-injection contract.

## 4. Verification performed

All generated outputs are under the D-drive test root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d052-llm-xml-review-20260916`.

- `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform='Any CPU'` — exit `0`, errors `0`; 19 existing nullable warnings remain in unrelated contracts.
- `VisionRecipeRunnerSmoke --llm-draft-review-owner-contract` — `passed=3`, `failed=0`.
- `OpenVisionReadinessCheck` — all 13 readiness contracts passed.
- Current-source WPF `wpf_shell_host_recipe_guided_setup` — `OK`, `1600x900`,
  no layout/text/internal findings. This target exercises draft creation,
  validation gates, stale invalidation, no Preview/Run/layer/routing side
  effects, and explicit import assertions.
- Current-source WPF `wpf_shell_host_recipe_review_bundle_import` — `OK`,
  `1600x900`, no layout/text/internal findings. Bundle read remains a dry-run;
  no extraction/copy/save/activate/Preview/Run side effect is allowed.
- `wpf_shell_host_recipe_manager_summary` precheck — **NG from a stale test
  expectation**: the target searched for `HostRecipeManagerCommandStrip`, which
  is absent from the current visible AutomationId set. The process produced a
  `0x0` capture and no LLM execution assertion; this is recorded as an
  unrelated UI-smoke limitation, not converted into a product pass.
- Monitor topology was queried before EXE launch. Windows reported one logical
  monitor (`\\.\DISPLAY2`, `1920x1080`, work area `1920x1032`), so the reported
  screen was used unchanged as required. No second-PC or offline-install run
  was possible on this workstation.

## 5. Self-evaluation and boundaries

The positive evidence is limited to the existing maintenance-mode contract and
the exercised synthetic/current-source flows. It does not prove a provider
quality benchmark, autonomous LLM behavior, physical inspection reliability,
atomic recovery after a storage failure, or an Undo UI that does not exist.
The source review also identifies a concrete future-risk boundary: dependency
and reference files are copied before `VisionPipelineStorage.Save` and
`SaveActivePipelineName`; a newly admitted transactional contract would need to
own cleanup on a later failure. No code change was made here because adding that
transaction would expand the frozen LLM scope without an explicit product
decision.

## 6. Completion record

Status: Complete
Scope: Verify the current LLM/XML draft review/validation/import/explicit-run
boundary and record the maintenance-mode N/A decision for activation-level
transaction/Undo.
Acceptance criteria: Existing no-auto-execution and fail-closed review/import
contracts are supported by the focused owner, readiness, and current-source UI
evidence above; unimplemented transaction/Undo behavior is explicitly not
claimed and has a documented reopen condition.
Verification: Debug smoke build, review-owner contract, readiness, current-source
Guided Setup and review-bundle WPF targets, and monitor-topology check listed
above.
Evidence: `.proofline/issues/PL-0106.json` and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d052-llm-xml-review-20260916`.
Boundary / next dependency: `2D-048` remains blocked without a new/offline PC;
`2D-049` requires independent participants; `2D-050` requires a named
hardware/long-run or measured bottleneck decision. Reopen LLM transaction/Undo
only by explicit product decision.
