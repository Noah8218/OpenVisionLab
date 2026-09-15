# OpenVisionLab OVL-45 LLM draft review owner

Status: Complete for one independently verifiable Shell LLM XML draft-review
boundary. The wider refactoring program remains active.

## Scope

`OpenVisionShellHostRecipeCommandSurface`의 LLM XML 초안 검증 경로는 검증
결과를 UI에 투영하면서 현재 active Recipe Pipeline XML을 직접 읽고
`OpenVisionRecipePipelineComparisonPresenter`를 호출했습니다. 이번 slice는
그 읽기 전용 baseline 조회와 import/diff 검토 문구 조합을
`OpenVisionRecipeLlmDraftReviewOwner` concrete owner로 이동했습니다.

기존 Recipe/XML 저장 형식, `LlmXmlDraft*` 바인딩 이름, Import의 고유
Pipeline 저장·활성화, 명시적 Preview/Run 분리, locator 승인 gate는
변경하지 않았습니다. owner는 한 번의 검토 요청 동안만 active Pipeline을
읽고 결과를 반환하며 Recipe를 저장하거나 실행하지 않습니다.

## Refactor proof

### Before

- `OpenVisionShellHostRecipeCommandSurface.Handlers.cs`의
  `BuildLlmDraftReviewReport`와 `BuildLlmDraftDiffReport`가 Recipe 이름을
  정규화하고 active Pipeline 이름을 읽은 뒤 `VisionPipelineStorage.Load`를
  호출했습니다.
- 두 메서드는 `LlmXmlDraftWorkflow.ValidateLlmXmlDraftText`와 Import
  경로에서 사용되었지만, persisted baseline 조회와 UI 상태 projection이
  CommandSurface partial에 함께 있었습니다.

### After

- `OpenVisionRecipeLlmDraftReviewOwner.Build`가 Recipe 이름, active Pipeline
  선택 조회, persisted Pipeline 로드, 기존 비교 presenter 호출을 소유합니다.
- `OpenVisionRecipeLlmDraftReview`가 import-review/diff-review 두 읽기 전용
  결과를 함께 반환합니다.
- CommandSurface는 `LlmXmlDraftValidationReport`, dependency rows,
  review/diff text, `llmXmlDraftImportReady`, `StatusText`와
  `RefreshCommandState`를 계속 소유하며 새 WPF state나 wrapper chain은
  추가하지 않았습니다.
- former `BuildLlmDraftReviewReport`/`BuildLlmDraftDiffReport`와 그 active
  Pipeline storage call은 CommandSurface에서 제거되었습니다. 기존
  `BuildPipelineVariantComparisonReport`의 variant 비교 경로는 별도
  책임이므로 유지했습니다.

## Responsibility map

| 항목 | 현재 owner | 의도한 owner / 경계 |
| --- | --- | --- |
| LLM draft 검증 요청과 UI 상태 | `OpenVisionShellHostRecipeCommandSurface.LlmXmlDraftWorkflow` | CommandSurface가 `LlmXmlDraft*` 바인딩과 Import readiness를 계속 소유 |
| active baseline 조회와 draft import/diff 비교 조합 | 기존 CommandSurface helper | `OpenVisionRecipeLlmDraftReviewOwner` |
| 구조/parameter diff 문구 | `OpenVisionRecipePipelineComparisonPresenter` | 기존 presenter 유지 |
| Recipe XML 저장/활성 Pipeline 변경 | `VisionPipelineStorage` 및 기존 Import 경로 | 변경 없음 |
| lifetime / mutable state | owner는 무상태; CommandSurface가 projection state 보유 | 호출당 반환 객체만 사용, dispose/callback 없음 |

실제 call path:

```text
LLM draft creator/load/bundle dry-run
  -> ValidateLlmXmlDraftText
  -> TryBuildLlmDraftPipeline
  -> OpenVisionRecipeLlmDraftReviewOwner.Build
  -> VisionPipelineStorage.LoadActivePipelineName/Load
  -> OpenVisionRecipePipelineComparisonPresenter
  -> CommandSurface LlmXmlDraft* projection + existing Import command state
```

Dependency direction은 `CommandSurface -> Review owner -> Pipeline storage /
comparison presenter`입니다. owner는 WPF control, Window, Clipboard, file
dialog, execution session, Layer/ImageSpace를 참조하지 않습니다. active
Pipeline 읽기는 review owner에 남지만 저장과 실행은 기존 caller가 담당합니다.

## Developer reading order

1. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.LlmXmlDraftWorkflow.cs` — `ValidateLlmXmlDraftText`와 두 호출 지점
2. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeLlmDraftReviewOwner.cs` — active baseline 조회와 두 결과 반환
3. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipePipelineComparisonPresenter.cs` — read-only import/diff formatting
4. `tools/VisionRecipeRunnerSmoke/LlmDraftReviewOwnerContract.cs` — persisted baseline, no-mutation, null draft 계약
5. `tools/OpenVisionReadinessCheck/Program.cs` — CommandSurface가 기존 storage helper를 다시 소유하지 않는 정적 gate

한 번의 검색은 `llmDraftReviewOwner.Build`입니다. `LlmXmlDraft*`의 mutable
상태와 binding/public contract는 CommandSurface에서 찾고, active baseline과
비교 출력은 owner와 presenter 순서로 읽습니다.

## Verification evidence

모든 contract 산출물은 D:에 저장했습니다.

- `dotnet build tools\\VisionRecipeRunnerSmoke\\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform="Any CPU" --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- `dotnet build tools\\VisionRecipeRunnerSmoke\\VisionRecipeRunnerSmoke.csproj -c Release -p:Platform="Any CPU" --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- `dotnet build src\\OpenVisionLab\\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:OpenVisionLabEnableEmbeddedSmokeRunner=true --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- `dotnet build src\\OpenVisionLab\\OpenVisionLab.csproj -c Release -p:Platform=x64 -p:OpenVisionLabEnableEmbeddedSmokeRunner=true --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- `VisionRecipeRunnerSmoke --llm-draft-review-owner-contract` — Debug `3/3` pass at `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl45-llm-draft-review-owner-contract-debug-20260909-run2`; Release `3/3` pass at `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl45-llm-draft-review-owner-contract-release-20260909-run1`.
- Contract cases: persisted active baseline and import/diff text, Recipe/active selection unchanged, null draft safe output.
- `OpenVisionReadinessCheck` — pass after replacing the old helper assertions with the new owner delegation check.
- `Invoke-RefactorAudit.ps1 -Verify` — `REFACTOR_AUDIT=PASS|CSharpFiles=830|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- `TestDocumentationIndex.ps1 -RepoRoot C:\\Git\\2D\\Dev` — `DocumentationIndex=PASS IndexedPaths=264 Routes=13 RootRedirects=102`; JSON parse and scoped `git diff --check` passed (existing LF/CRLF normalization notices only).

## Runtime boundary and remaining work

이번 source/contract slice는 실제 Shell EXE를 띄우는 UI smoke가 아니라
WPF-free Recipe review owner와 storage isolation을 검증합니다. Desktop
LLM tab rendering, Clipboard path, alternate theme/DPI, locator Evidence
Packet UI, Import/Preview/Run runtime은 이번 slice에서 재실행하지 않았으며
기존 관련 runtime evidence가 계속 적용됩니다.

The owner has no persisted mutable state, so there is no shutdown or disposal
ordering change. The existing comparison presenter and variant-comparison
path remain canonical. A new draft-review defect, changed review contract, or
demonstrated dependency conflict is required before reopening or splitting this
owner. File length, model preference, or repeated continuation requests are not
reasons to recreate it.

Junior developer self-assessment: **PASS for this boundary**. The LLM draft
call path now has one searchable baseline-review owner while UI state and
explicit Import/Preview/Run actions remain at the Shell facade.

Next priority: select one remaining Shell validation/evidence or step-edit call
path only after its mutable-state boundary is proven; completed OVL-01/02/03/
04/05/06a/07/08/10/12-45 owners stay closed.
Recommended model: `gpt-5.6-terra`; reasoning effort: `high`.
